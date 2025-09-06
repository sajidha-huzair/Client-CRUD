# Mastering CRUD Operations with Clean Architecture and MediatR: A 4-Layer Approach in .NET 8

## Introduction

Building CRUD applications doesn't have to be messy. In this step-by-step guide, I'll walk you through creating a clean, maintainable CRUD API using the 4-layer Clean Architecture pattern combined with the powerful MediatR library. 

This isn't about building a complex business application—it's about mastering the fundamentals of organizing your CRUD operations in a way that scales and remains maintainable.

## What You'll Learn

By the end of this tutorial, you'll understand:
- How to structure CRUD operations across 4 distinct layers
- Why MediatR is a game-changer for organizing your business logic
- How to implement the Command Query Responsibility Segregation (CQRS) pattern
- Step-by-step implementation of Create and Read operations (POST & GET)

## The CRUD We're Building

We'll create a simple Client Management CRUD focusing on:
- **Create** (POST) - Add new clients
- **Read** (GET) - Get all clients

Client properties: `id`, `Name`, `Age`, `Gender`, `Email`, `Contact`

## Prerequisites

- .NET 8 SDK
- Basic C# and ASP.NET Core knowledge
- Understanding of dependency injection

## The 4-Layer Architecture Explained

Before diving into code, let's understand why we separate our CRUD into layers:

### 1. **Domain Layer** - The Heart
- Contains your core entities
- No dependencies on anything
- Pure business objects

### 2. **Application Layer** - The Brain  
- Houses your CRUD business logic
- Commands and Queries (CQRS pattern)
- Handlers that process your operations
- Repository interfaces

### 3. **Infrastructure Layer** - The Hands
- Implements repository interfaces
- Database access code
- External service integrations

### 4. **API Layer** - The Face
- Controllers that expose endpoints
- HTTP request/response handling
- Dependency injection setup

## Step 1: Setting Up the Project Structure

Create your solution with this folder structure:

```
ClientCRUD/
├── Aipazz.Domain/          # Layer 1: Core entities
├── Aipazz.Application/     # Layer 2: CRUD logic + MediatR
├── Aipazz.Infrastructure/  # Layer 3: Data access
└── Aipazz.API/            # Layer 4: Controllers
```

**Why this structure?** Each layer has a single responsibility, and dependencies flow inward (API → Application → Domain).

## Step 2: Domain Layer - Your Core Entity

Start with the simplest layer—your domain entity. This is just a plain C# class with no dependencies.

**Create `Client.cs`:**

```csharp
namespace Aipazz.Domain
{
    public class Client
    {
        public string id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
    }
}
```

**Key Insight:** The Domain layer knows nothing about databases, APIs, or frameworks. It's pure business logic.

## Step 3: Application Layer - Where MediatR Shines

This is where the magic happens. We'll use MediatR to implement the CQRS pattern, separating our CRUD operations into Commands (writes) and Queries (reads).

### Step 3.1: Define the Repository Contract

**Create `IClientRepository.cs`:**

```csharp
using Aipazz.Domain;

namespace Aipazz.Application
{
    public interface IClientRepository
    {
        Task<List<Client>> GetAllClients();
        Task AddClient(Client client);
        // Other methods: GetClientById, UpdateClient, DeleteClient
    }
}
```

### Step 3.2: Create Commands and Queries (CQRS Pattern)

**Query for Reading Data (GET):**
```csharp
using Aipazz.Domain;
using MediatR;

namespace Aipazz.Application.Queries
{
    public record GetAllClientsQuery() : IRequest<List<Client>>;
}
```

**Command for Writing Data (POST):**
```csharp
using Aipazz.Domain;
using MediatR;

namespace Aipazz.Application.Commands
{
    public record CreateClientCommand(
        string Id, 
        string Name, 
        int Age, 
        string Gender, 
        string Email, 
        string Contact
    ) : IRequest<Client>;
}
```

### Step 3.3: Create Handlers (The CRUD Logic)

**Read Handler (GET All Clients):**
```csharp
using Aipazz.Application.Queries;
using Aipazz.Domain;
using MediatR;

namespace Aipazz.Application.Handlers
{
    public class GetAllClientsHandler : IRequestHandler<GetAllClientsQuery, List<Client>>
    {
        private readonly IClientRepository _repository;

        public GetAllClientsHandler(IClientRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Client>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllClients();
        }
    }
}
```

**Write Handler (POST - Create Client):**
```csharp
using Aipazz.Application.Commands;
using Aipazz.Domain;
using MediatR;

namespace Aipazz.Application.Handlers
{
    public class CreateClientHandler : IRequestHandler<CreateClientCommand, Client>
    {
        private readonly IClientRepository _repository;

        public CreateClientHandler(IClientRepository repository)
        {
            _repository = repository;
        }

        public async Task<Client> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {
            var client = new Client
            {
                id = request.Id,
                Name = request.Name,
                Age = request.Age,
                Gender = request.Gender,
                Email = request.Email,
                Contact = request.Contact
            };

            await _repository.AddClient(client);
            return client;
        }
    }
}
```

**Pattern Insight:** 
- **Queries** = Read operations (GET)
- **Commands** = Write operations (POST, PUT, DELETE)
- Each has its own dedicated handler
- Handlers contain the actual business logic

**For UPDATE and DELETE operations:** The pattern is identical—create a command (like `UpdateClientCommand`, `DeleteClientCommand`) and its corresponding handler. The only difference is the repository method called inside the handler.

## Step 4: Infrastructure Layer - Implementing Data Access

The Infrastructure layer implements the repository interface. Here's a simplified version:

```csharp
using Aipazz.Application;
using Aipazz.Domain;
using Microsoft.Azure.Cosmos;

namespace Aipazz.Infrastructure
{
    public class ClientRepository : IClientRepository
    {
        private readonly Container _container;

        public ClientRepository(CosmosClient cosmosClient, string databaseName, string containerName)
        {
            var database = cosmosClient.GetDatabase(databaseName);
            _container = database.GetContainer(containerName);
        }

        public async Task<List<Client>> GetAllClients()
        {
            var query = new QueryDefinition("SELECT * FROM c");
            var iterator = _container.GetItemQueryIterator<Client>(query);
            List<Client> clients = new List<Client>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                clients.AddRange(response);
            }
            return clients;
        }

        public async Task AddClient(Client client)
        {
            await _container.CreateItemAsync(client, new PartitionKey(client.id));
        }

        // Other CRUD methods follow the same pattern
    }
}
```

**Layer Insight:** The Infrastructure layer implements the contract but doesn't dictate how the Application layer works.

## Step 5: API Layer - Exposing Your CRUD

The API layer routes HTTP requests to MediatR handlers:

```csharp
using Aipazz.Application.Commands;
using Aipazz.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/client
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllClientsQuery());
        return Ok(result);
    }

    // POST: api/client
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClientCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { Id = result.id }, result);
    }

    // PUT and DELETE follow the same pattern - just different commands
}
```

**Key Insight:** The controller is thin—all CRUD logic is in MediatR handlers.

## Step 6: Wiring Everything Together

Configure dependency injection in `Program.cs`:

```csharp
using Aipazz.Application;
using Aipazz.Infrastructure;
using MediatR;
using Microsoft.Azure.Cosmos;
using Aipazz.Application.Queries;

var builder = WebApplication.CreateBuilder(args);

// Register MediatR - This is crucial!
builder.Services.AddMediatR(typeof(GetAllClientsQuery).Assembly);

// Register Repository
builder.Services.AddSingleton<IClientRepository, ClientRepository>();

// Add other services...
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

## Testing Your Layered CRUD

**Create a client (POST):**
```bash
POST /api/client
{
  "id": "client-001",
  "name": "John Doe",
  "age": 30,
  "gender": "Male",
  "email": "john.doe@example.com",
  "contact": "+1234567890"
}
```

**Read all clients (GET):**
```bash
GET /api/client
```

## What You've Accomplished

You've built a CRUD application that demonstrates:

### **Layer Separation Benefits:**
- **Domain**: Pure business entities
- **Application**: CRUD logic isolated in handlers
- **Infrastructure**: Database concerns separated
- **API**: Thin controllers focused on HTTP

### **MediatR Pattern Benefits:**
- **Single Responsibility**: Each handler does one thing
- **Testability**: Easy to unit test individual CRUD operations
- **Maintainability**: Easy to add new operations without touching existing code
- **CQRS**: Clear separation between reads and writes

### **Real-World Advantages:**
- Want to change from Cosmos DB to SQL Server? Only touch the Infrastructure layer
- Need to add validation? Add it to specific handlers
- Want to cache read operations? Modify only the query handlers
- Need to audit operations? Add cross-cutting concerns via MediatR behaviors

## Key Takeaways

1. **Layers prevent chaos** - Each layer has a clear responsibility
2. **MediatR organizes CRUD logic** - No more fat controllers or services
3. **CQRS separates concerns** - Reads and writes are handled differently
4. **Dependency flow matters** - Always point inward (API → Application → Domain)
5. **Start simple** - This pattern scales from simple CRUD to complex business logic

## Complete Implementation

For the complete implementation including UPDATE and DELETE operations, along with error handling, validation, and additional features, check out the full source code:

**📁 GitHub Repository:** [https://github.com/yourusername/clean-crud-mediatr](https://github.com/yourusername/clean-crud-mediatr)

The repository includes:
- Complete CRUD operations (GET, POST, PUT, DELETE)
- Error handling and validation
- Unit tests for all handlers
- Docker setup
- API documentation

## Next Steps to Level Up

- Add **FluentValidation** to your command handlers
- Implement **MediatR Behaviors** for cross-cutting concerns (logging, validation)
- Add **Unit Tests** for each handler
- Explore **AutoMapper** for object mapping
- Implement **Result patterns** for better error handling

This layered approach to CRUD operations provides a solid foundation that can grow with your application's complexity while keeping your code organized and maintainable.

---

*Remember: Good architecture isn't about using every pattern—it's about organizing code in a way that makes sense and reduces complexity as your application grows.*

**Tags:** #dotnet #cleanarchitecture #mediatr #cqrs #crud #layers #csharp

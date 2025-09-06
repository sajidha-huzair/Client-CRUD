using Aipazz.Application;
using Aipazz.Infrastructure;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using Aipazz.Application.Queries;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMediatR(typeof(GetAllClientsQuery).Assembly);


// Register Cosmos DB connection
builder.Services.AddSingleton<CosmosClient>(serviceProvider =>
{
    // Fetch the Cosmos DB connection details from appsettings.json
    var configuration = builder.Configuration;
    string? accountEndpoint = configuration["CosmosDb:AccountEndpoint"];
    string? authKey = configuration["CosmosDb:AuthKey"];

    if (string.IsNullOrEmpty(accountEndpoint) || string.IsNullOrEmpty(authKey))
    {
        throw new InvalidOperationException("Cosmos DB connection details are not configured properly.");
    }

    // Create and return CosmosClient instance
    return new CosmosClient(accountEndpoint, authKey);
});

// Register IClientRepository and its implementation ClientRepository
builder.Services.AddSingleton<IClientRepository, ClientRepository>(serviceProvider =>
{
    var cosmosClient = serviceProvider.GetRequiredService<CosmosClient>();
    var configuration = builder.Configuration;
    string? databaseName = configuration["CosmosDb:DatabaseName"];
    string? containerName = configuration["CosmosDb:ContainerName"];

    if (string.IsNullOrEmpty(databaseName) || string.IsNullOrEmpty(containerName))
    {
        throw new InvalidOperationException("Cosmos DB database or container name is not configured properly.");
    }

    // Create and return ClientRepository instance
    return new ClientRepository(cosmosClient, databaseName, containerName);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

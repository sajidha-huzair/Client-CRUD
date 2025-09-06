using Aipazz.Application;
using Aipazz.Domain;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        // Implement GetAllClients
        public async Task<List<Client>> GetAllClients()
        {
            var query = new QueryDefinition("SELECT * FROM c");
            var iterator = _container.GetItemQueryIterator<Client>(query);
            List<Client> clients = new List<Client>();

            while (iterator.HasMoreResults)
            {
                try
                {
                    var response = await iterator.ReadNextAsync();
                    clients.AddRange(response);
                }
                catch (CosmosException ex)
                {
                    Console.WriteLine($"Error fetching clients: {ex.Message}");
                }
            }

            return clients;
        }

        // Implement GetClientById
        public async Task<Client> GetClientById(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<Client>(
                    id,
                    new PartitionKey(id) // Using Id as partition key for simplicity
                );
                return response.Resource;
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Error fetching client: {ex.Message}");
                return null;
            }
        }

        // Implement AddClient
        public async Task AddClient(Client client)
        {
            try
            {
                await _container.CreateItemAsync(client, new PartitionKey(client.Id));
                Console.WriteLine($"Successfully added client ID: {client.Id}");
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Error adding client: {ex.Message}");
            }
        }


        // Implement UpdateClient
        public async Task UpdateClient(Client client)
        {
            try
            {
                await _container.UpsertItemAsync(client, new PartitionKey(client.Id));
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Error updating client: {ex.Message}");
            }
        }

        // Implement DeleteClient
        public async Task DeleteClient(string id)
        {
            try
            {
                await _container.DeleteItemAsync<Client>(id, new PartitionKey(id));
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Error deleting client: {ex.Message}");
            }
        }
    }
}


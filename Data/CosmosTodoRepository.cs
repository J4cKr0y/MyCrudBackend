// Data/CosmosTodoRepository.cs
using Microsoft.Azure.Cosmos;
using MyCrudBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace MyCrudBackend.Data
{
    public class CosmosTodoRepository : ITodoRepository
    {
        private readonly Container _container;

        public CosmosTodoRepository(CosmosClient cosmosClient, string databaseName, string containerName)
        {
            // Récupération du conteneur Cosmos
            _container = cosmosClient.GetContainer(databaseName, containerName);
        }

        public async Task<IEnumerable<TodoItem>> GetAllAsync()
        {
            var query = _container.GetItemQueryIterator<TodoItem>("SELECT * FROM c");
            var results = new List<TodoItem>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task<TodoItem?> GetByIdAsync(int id)
        {
            try
            {
                // Utilisation de id.ToString() comme clé de partition
                ItemResponse<TodoItem> response = 
                    await _container.ReadItemAsync<TodoItem>(id.ToString(), new PartitionKey(id.ToString()));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<TodoItem> CreateAsync(TodoItem item)
        {
            // on peut générer un ID si nécessaire
            if (item.Id == 0)
            {
            // Todo: Ce mécanisme d'auto-génération est basique, utiliser un GUID.
                item.Id = new Random().Next(1, 1000000);
            }
            var response = await _container.CreateItemAsync(item, new PartitionKey(item.Id.ToString()));
            return response.Resource;
        }

        public async Task UpdateAsync(TodoItem item)
        {
            // La méthode Upsert permet de créer ou mettre à jour un item.
            await _container.UpsertItemAsync(item, new PartitionKey(item.Id.ToString()));
        }

        public async Task DeleteAsync(int id)
        {
            await _container.DeleteItemAsync<TodoItem>(id.ToString(), new PartitionKey(id.ToString()));
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Boozfinder.Models.Data;
using Boozfinder.Services.Interfaces;
using Microsoft.Azure.Cosmos;
using Serilog;
using User = Boozfinder.Models.Data.User;

namespace Boozfinder.Services
{
    public class CosmosDbService : ICosmosDbService
    {
        private Container _container;

        public CosmosDbService(CosmosClient dbClient, string databaseName, string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<Booze> GetItemAsync(string id)
        {
            try
            {
                ItemResponse<Booze> response = await _container.ReadItemAsync<Booze>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (CosmosException ex)
            {
                Log.Error($"Error in GetItemAsync() - Exception message: [{ex.Message}] - StackTrace: [{ex.StackTrace}]");
                throw;
            }
        }

        public async Task<User> GetUserAsync(string id)
        {
            try
            {
                ItemResponse<User> response = await _container.ReadItemAsync<User>($"u_{id}", new PartitionKey($"u_{id}"));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (CosmosException ex)
            {
                Log.Error($"Error in GetUserAsync() - Exception message: [{ex.Message}] - StackTrace: [{ex.StackTrace}]");
                throw;
            }
        }

        public async Task<IEnumerable<Booze>> GetItemsAsync(string queryString)
        {
            try
            {
                var query = _container.GetItemQueryIterator<Booze>(new QueryDefinition(queryString));
                var results = new List<Booze>();
                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();
                    results.AddRange(response.ToList().Where(r => !r.Id.StartsWith("u_")));
                }
                return results;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (CosmosException ex)
            {
                Log.Error($"Error in GetItemsAsync() - Exception message: [{ex.Message}] - StackTrace: [{ex.StackTrace}]");
                throw;
            }
        }

        public async Task AddItemAsync(Booze item)
        {
            try
            {
                await _container.CreateItemAsync(item, new PartitionKey(item.Id));
            }
            catch (CosmosException ex)
            {
                Log.Error($"Error in AddItemAsync() - Exception message: [{ex.Message}] - StackTrace: [{ex.StackTrace}]");
                throw;
            }
        }

        public async Task AddUserAsync(User user)
        {
            try
            {
                await _container.CreateItemAsync(user, new PartitionKey(user.Id));
            }
            catch (CosmosException ex)
            {
                Log.Error($"Error in AddUserAsync() - Exception message: [{ex.Message}] - StackTrace: [{ex.StackTrace}]");
                throw;
            }
        }

        public async Task UpdateItemAsync(string id, Booze item)
        {
            try
            {
                await _container.UpsertItemAsync(item, new PartitionKey(id));
            }
            catch (CosmosException ex)
            {
                Log.Error($"Error in UpdateItemAsync() - Exception message: [{ex.Message}] - StackTrace: [{ex.StackTrace}]");
                throw;
            }
        }

        public async Task UpdateUserAsync(string id, User user)
        {
            try
            {
                await _container.UpsertItemAsync(user, new PartitionKey(id));
            }
            catch (CosmosException ex)
            {
                Log.Error($"Error in UpdateUserAsync() - Exception message: [{ex.Message}] - StackTrace: [{ex.StackTrace}]");
                throw;
            }
        }

        public async Task DeleteItemAsync(string id)
        {
            try
            {
                await _container.DeleteItemAsync<Booze>(id, new PartitionKey(id));
            }
            catch (CosmosException ex)
            {
                Log.Error($"Error in DeleteItemAsync() - Exception message: [{ex.Message}] - StackTrace: [{ex.StackTrace}]");
                throw;
            }
        }
    }
}

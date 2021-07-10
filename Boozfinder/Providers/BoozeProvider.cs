using Boozfinder.Models.Data;
using Boozfinder.Providers.Interfaces;
using Boozfinder.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Boozfinder.Providers
{
    public class BoozeProvider : IBoozeProvider
    {
        private readonly ICosmosDbService _cosmosDbService;

        public BoozeProvider(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task<Booze> GetAsync(string id)
        {
            return await _cosmosDbService.GetItemAsync(id);
        }

        public async Task<IEnumerable<Booze>> GetAsync()
        {
            return await _cosmosDbService.GetItemsAsync("SELECT * FROM c");
        }

        public async Task CreateAsync(Booze item)
        {
            await _cosmosDbService.AddItemAsync(item);
        }

        public async Task UpdateAsync(Booze item)
        {
            await _cosmosDbService.UpdateItemAsync(item.Id, item);
        }

        public async Task DeleteAsync(Booze item)
        {
            await _cosmosDbService.DeleteItemAsync(item.Id);
        }

        public async Task<IEnumerable<Booze>> SearchAsync(string searchTerm, string type = "")
        {
            IEnumerable<Booze> itemList;
            if (!string.IsNullOrWhiteSpace(type))
            {
                itemList = await _cosmosDbService.GetItemsAsync($"SELECT * FROM c WHERE name LIKE {searchTerm} AND type = {type}");
            }
            else
            {
                itemList = await _cosmosDbService.GetItemsAsync($"SELECT * FROM c WHERE name LIKE {searchTerm}");
            }

            return itemList;
        }
    }
}

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
        private readonly IImageService _imageService;

        public BoozeProvider(ICosmosDbService cosmosDbService, IImageService imageService)
        {
            _cosmosDbService = cosmosDbService;
            _imageService = imageService;
        }

        public async Task<Booze> GetAsync(string id)
        {
            var item = await _cosmosDbService.GetItemAsync(id);
            if (item != null)
            {
                if (item.ImageData != null)
                {
                    var image = await _imageService.GetImageFromBlobStorageAsync(id);
                    item.ImageData = image;
                }
            }
            return item;
        }

        public async Task<IEnumerable<Booze>> GetAsync()
        {
            var itemList = await _cosmosDbService.GetItemsAsync("SELECT * FROM c");
            foreach (var item in itemList)
            {
                var image = await _imageService.GetImageFromBlobStorageAsync(item.Id);
                if (image != null)
                {
                    item.ImageData = image;
                }
            }
            return itemList;
        }

        public async Task SaveAsync(Booze item)
        {
            if (item.ImageData != null)
            {
                ;
                await _imageService.AddImageToBlobStorageAsync(item.ImageData, item.Id);
                item.ImageData = null;
            }
            await _cosmosDbService.AddItemAsync(item);
        }

        public async Task UpdateAsync(Booze item)
        {
            if (item.ImageData != null)
            {
                // First delete the existing image
                await _imageService.DeleteImageFromBlobStorageAsync(item.Id);

                // Then save the new image
                await _imageService.AddImageToBlobStorageAsync(item.ImageData, item.Id);
                item.ImageData = null;
            }
            await _cosmosDbService.UpdateItemAsync(item.Id, item);
        }

        public async Task DeleteAsync(Booze item)
        {
            if (item.ImageData != null)
            {
                await _imageService.DeleteImageFromBlobStorageAsync(item.Id);
            }
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

            foreach (var item in itemList)
            {
                var image = await _imageService.GetImageFromBlobStorageAsync(item.Id);
                if (image != null)
                {
                    item.ImageData = image;
                }
            }

            return itemList;
        }
    }
}

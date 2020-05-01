using Boozfinder.Services.Interfaces;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace Boozfinder.Services
{
    public class ImageService : IImageService
    {

        private readonly string _containerName;
        private readonly string _connectionString;

        public ImageService(string connectionString, string containerName)
        {
            _connectionString = connectionString;
            _containerName = containerName;
        }

        public async Task AddImageToBlobStorageAsync(string base64image, string imageName)
        {
            try
            {
                var container = GetImagesBlobContainer();
                var blockBlob = container.GetBlockBlobReference(imageName);
                var byteArrayImage = Base64ToByteArray(base64image);
                await blockBlob.UploadFromByteArrayAsync(byteArrayImage, 0, byteArrayImage.Length);
            }
            catch (Exception ex)
            {
                // TODO: Log error
                throw;
            }
        }

        public async Task<string> GetImageFromBlobStorageAsync(string imageName)
        {
            try
            {
                var downloadedImageByteArray = Array.Empty<byte>();
                var container = GetImagesBlobContainer();
                var blockBlob = container.GetBlockBlobReference(imageName);
                await blockBlob.DownloadToByteArrayAsync(downloadedImageByteArray, 0);
                return ByteArrayToBase64(downloadedImageByteArray);
            }
            catch (Exception ex)
            {
                // TODO: Log error
                throw;
            }
        }

        public async Task DeleteImageFromBlobStorageAsync(string imageName)
        {
            try
            {
                var container = GetImagesBlobContainer();
                var blockBlob = container.GetBlockBlobReference(imageName);
                await blockBlob.DeleteAsync();
            }
            catch (Exception ex)
            {
                // TODO: Log error
                throw;
            }
        }

        #region Private methods

        private byte[] Base64ToByteArray(string base64String)
        {
            return Convert.FromBase64String(base64String);
        }

        private string ByteArrayToBase64(byte[] byteArrayImage)
        {
            return Convert.ToBase64String(byteArrayImage, 0, byteArrayImage.Length);
        }

        private CloudBlobContainer GetImagesBlobContainer()
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(_containerName);
            
            if (container.CreateIfNotExists())
            {
                var permissions = container.GetPermissions();
                permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                container.SetPermissions(permissions);
            }
            
            return container;
        }

        #endregion
    }
}

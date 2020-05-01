using System.Threading.Tasks;

namespace Boozfinder.Services.Interfaces
{
    public interface IImageService
    {
        Task AddImageToBlobStorageAsync(string base64image, string imageName);
        Task<string> GetImageFromBlobStorageAsync(string imageName);
        Task DeleteImageFromBlobStorageAsync(string imageName);
    }
}

using Boozfinder.Models.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Boozfinder.Services.Interfaces
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<Booze>> GetItemsAsync(string query);
        Task<Booze> GetItemAsync(string id);
        Task<User> GetUserAsync(string id);
        Task AddItemAsync(Booze item);
        Task AddUserAsync(User user);
        Task UpdateItemAsync(string id, Booze item);
        Task UpdateUserAsync(string id, User user);
        Task DeleteItemAsync(string id);
    }
}

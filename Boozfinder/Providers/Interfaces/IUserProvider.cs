using Boozfinder.Models.Data;
using System.Threading.Tasks;

namespace Boozfinder.Providers.Interfaces
{
    public interface IUserProvider
    {
        Task<bool> CreateAsync(User user);
        Task<bool> HasAdminRoleAsync(string email);
        Task<bool> AuthenticateAsync(string email, string password);
    }
}

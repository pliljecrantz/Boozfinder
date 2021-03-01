using Boozfinder.Helpers;
using Boozfinder.Models.Data;
using Boozfinder.Providers.Interfaces;
using Boozfinder.Services.Interfaces;
using System.Threading.Tasks;

namespace Boozfinder.Providers
{
    public class UserProvider : IUserProvider
    {
        private readonly ICosmosDbService _cosmosDbService;

        public UserProvider(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task<bool> CreateAsync(User user)
        {
            var result = false;
            var existingUser = await _cosmosDbService.GetUserAsync(user.Email);
            if (existingUser == null)
            {
                var hashedAndSaltedPassword = HashUtility.HashPassword(user.Password);
                user.Password = hashedAndSaltedPassword;
                user.Role = Enums.Role.User.ToString().ToLower();
                await _cosmosDbService.AddUserAsync(user);
                result = true;
            }
            return result;
        }

        public async Task<bool> HasAdminRoleAsync(string email)
        {
            var user = await _cosmosDbService.GetUserAsync(email);
            return user != null && user.Role.ToLower() == Enums.Role.Admin.ToString().ToLower();
        }

        public async Task<bool> AuthenticateAsync(string email, string password)
        {
            var user = await _cosmosDbService.GetUserAsync(email);
            var verifiedPassword = HashUtility.VerifyPassword(password, user.Password);
            return user != null && verifiedPassword;
        }
    }
}

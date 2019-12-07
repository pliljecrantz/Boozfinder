using Boozfinder.Models.Data;

namespace Boozfinder.Providers.Interfaces
{
    public interface IAuthenticationProvider
    {
        bool AuthenticateUser(string email, string password);
    }
}

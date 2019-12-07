using Boozfinder.Models.Data;
using Boozfinder.Models.Responses;

namespace Boozfinder.Providers.Interfaces
{
    public interface IUserProvider
    {
        bool CreateUser(User user);
    }
}

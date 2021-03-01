using Boozfinder.Models.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Boozfinder.Providers.Interfaces
{
    public interface IBoozeProvider
    {
        Task<Booze> GetAsync(string id);
        Task<IEnumerable<Booze>> GetAsync();
        Task CreateAsync(Booze item);
        Task UpdateAsync(Booze item);
        Task DeleteAsync(Booze item);
        Task<IEnumerable<Booze>> SearchAsync(string text, string type = "");
    }
}

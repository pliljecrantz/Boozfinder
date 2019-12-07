using Boozfinder.Models.Data;
using System.Collections.Generic;

namespace Boozfinder.Providers.Interfaces
{
    public interface IBoozeProvider
    {
        Booze Get(int id);
        IEnumerable<Booze> Get();
        Booze Save(Booze booze);
        Booze Update(Booze booze);
        IEnumerable<Booze> Search(string searchTerm, string type = "");
    }
}

namespace Boozfinder.Providers.Interfaces
{
    public interface ICacheProvider
    {
        void Set(string token, string email);
        string Get(string cacheKey);
    }
}

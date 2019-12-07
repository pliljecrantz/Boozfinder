using Boozfinder.Providers.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Boozfinder.Providers
{
    public class MemoryCacheProvider : ICacheProvider
    {
        public MemoryCacheProvider(IMemoryCache cache)
        {
            Cache = cache;
        }

        public IMemoryCache Cache { get; }

        public void Set(string token, string email)
        {
            var cacheKey = $"__Token_{token}";
            var cacheValue = $"{token}:{email}";
            if (Cache.Get(cacheKey) != null)
            {
                Cache.Remove(cacheKey);
            }
            Cache.Set(cacheKey, cacheValue, TimeSpan.FromMinutes(30));
        }

        public string Get(string cacheKey)
        {
            if (Cache.TryGetValue(cacheKey, out string cachedItem))
            {
                return cachedItem;
            }
            return null;
        }
    }
}

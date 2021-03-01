using Boozfinder.Providers.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Boozfinder.Providers
{
    public class CacheProvider : ICacheProvider
    {
        private readonly IMemoryCache _cache;

        public CacheProvider(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Set(string token, string email)
        {
            var cacheKey = $"__Token_{token}";
            var cacheValue = $"{token}:{email}";
            if (_cache.Get(cacheKey) != null)
            {
                _cache.Remove(cacheKey);
            }
            _cache.Set(cacheKey, cacheValue, TimeSpan.FromDays(1));
        }

        public string Get(string cacheKey)
        {
            if (_cache.TryGetValue(cacheKey, out string cachedItem))
            {
                return cachedItem;
            }
            return null;
        }
    }
}

using Case.Application.Extensions;
using Case.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Case.Application.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public void Set(string key, string value)
        {
            _distributedCache.SetString(key, value);
        }

        public void Set<T>(string key, T value) where T : class
        {
            _distributedCache.SetString(key, value.ToJson());
        }

        public Task SetAsync(string key, object value)
        {
            return _distributedCache.SetStringAsync(key, value.ToJson());
        }

        public void Set(string key, object value, TimeSpan expiration)
        {
            _distributedCache.SetString(key, value.ToJson(), new DistributedCacheEntryOptions { AbsoluteExpiration = DateTime.UtcNow.Add(expiration) });
        }

        public Task SetAsync(string key, object value, TimeSpan expiration)
        {
            return _distributedCache.SetStringAsync(key, value.ToJson(), new DistributedCacheEntryOptions { AbsoluteExpiration = DateTime.UtcNow.Add(expiration) });
        }

        public T Get<T>(string key) where T : class
        {
            string value = _distributedCache.GetString(key);

            return value.ToObject<T>();
        }

        public string Get(string key)
        {
            return _distributedCache.GetString(key);
        }

        public async Task<T> GetAsync<T>(string key) where T : class
        {
            string value = await _distributedCache.GetStringAsync(key);

            return value.ToObject<T>();
        }

        public void Remove(string key)
        {
            _distributedCache.Remove(key);
        }

        public Task RemoveAsync(string key)
        {
           return _distributedCache.RemoveAsync(key);
        }
    }
}

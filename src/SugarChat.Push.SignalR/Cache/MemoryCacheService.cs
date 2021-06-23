using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Cache
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly static IDictionary<string, IDictionary<string, string>> _hash = new Dictionary<string, IDictionary<string, string>>();

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public Task<T> GetAsync<T>(string key)
        {
            return Task.FromResult(_cache.Get<T>(key));
        }

        public Task<T> HashGetAsync<T>(string hashId, string key)
        {
            if(_hash.TryGetValue(hashId, out IDictionary<string, string> kv))
            {
                if(kv.TryGetValue(key, out string dicValue))
                {
                    return Task.FromResult(JsonSerializer.Deserialize<T>(dicValue));
                }
            }
            return Task.FromResult(default(T));
        }

        public Task HashSetAsync<T>(string hashId, string key, T value)
        {
            var stringValue = value is string s ? s : JsonSerializer.Serialize(value);
            if (_hash.TryGetValue(hashId, out IDictionary<string, string> kv))
            {
                kv[key] = stringValue;
            }
            else
            {
                _hash[hashId] = new Dictionary<string, string> { { key, stringValue } };
            }
            return Task.CompletedTask;
        }

        public void Set<T>(string key, T value, TimeSpan? timeSpan = null)
        {
            if (timeSpan.HasValue)
            {
                _cache.Set(key, value, timeSpan.Value);
            }
            else
            {
                _cache.Set(key, value);
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? timeSpan = null)
        {
            if (timeSpan.HasValue)
            {
                _cache.Set(key, value, timeSpan.Value);
            }
            else
            {
                _cache.Set(key, value);
            }
            return Task.CompletedTask;
        }

        public Task<IDictionary<string, string>> GetHashAll(string hashId)
        {
            return Task.FromResult(_hash[hashId]);
        }
    }
}

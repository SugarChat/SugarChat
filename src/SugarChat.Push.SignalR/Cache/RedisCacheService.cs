using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public T Get<T>(string key)
        {
            var cachedResult = _redis.GetDatabase().StringGet(key);
            return !cachedResult.IsNullOrEmpty ? JsonSerializer.Deserialize<T>(cachedResult) : default(T);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var cachedResult = await _redis.GetDatabase().StringGetAsync(key).ConfigureAwait(false);
            return !cachedResult.IsNullOrEmpty ? JsonSerializer.Deserialize<T>(cachedResult) : default(T);
        }

        public async Task<IDictionary<string, string>> GetHashAll(string hashId)
        {
            var list = await _redis.GetDatabase().HashGetAllAsync(hashId);
            return list.ToStringDictionary();
        }

        public async Task<T> HashGetAsync<T>(string hashId, string key)
        {
            var redisValue = await _redis.GetDatabase().HashGetAsync(hashId, key).ConfigureAwait(false);
            if (redisValue.HasValue)
            {
                return JsonSerializer.Deserialize<T>(redisValue);
            }
            return default(T);
        }

        public async Task HashSetAsync<T>(string hashId, string key, T value)
        {
            var stringValue = value is string s ? s : JsonSerializer.Serialize(value);
            await _redis.GetDatabase().HashSetAsync(hashId, new[] { new HashEntry(key, stringValue) }).ConfigureAwait(false);
        }

        public void Set<T>(string key, T value, TimeSpan? timeSpan = null)
        {
            var stringValue = value is string s ? s : JsonSerializer.Serialize(value);
            if (timeSpan != null)
            {
                _redis.GetDatabase().StringSet(key, stringValue, timeSpan.Value);
            }
            else
            {
                _redis.GetDatabase().StringSet(key, stringValue);
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? timeSpan = null)
        {
            var stringValue = value is string s ? s : JsonSerializer.Serialize(value);
            if (timeSpan.HasValue)
            {
                await _redis.GetDatabase().StringSetAsync(key, stringValue, timeSpan.Value).ConfigureAwait(false);
            }
            else
            {
                await _redis.GetDatabase().StringSetAsync(key, stringValue).ConfigureAwait(false);
            }
        }
    }
}

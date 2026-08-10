using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using SugarChat.Push.SignalR.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Services.Caching
{
    public interface ICacheService
    {
        Task<T> GetByKeyFromRedis<T>(string key) where T : class;
        Task<T> GetHashByKeyFromRedis<T>(string key, string field) where T : class;
        Task<List<T>> GetByKeysFromRedis<T>(List<string> keys) where T : class;
        Task RemoveByKeyFromRedis(string key);
        Task RemoveByKeysFromRedis(List<string> keys);
        Task SetRedisByKey<T>(string key, T value, TimeSpan? expiry = null) where T : class;
        Task SetHashRedisByKey<T>(string key, string field, T value) where T : class;
        Task SetRedisByKeys<T>(Dictionary<string, T> values, TimeSpan? expiry = null) where T : class;
    }

    public class CacheService : ICacheService
    {
        private readonly IRedisSafeRunner _redisSafeRunner;

        public CacheService(IRedisSafeRunner redisSafeRunner)
        {
            _redisSafeRunner = redisSafeRunner;
        }

        public async Task SetRedisByKey<T>(string key, T value, TimeSpan? expiry = null) where T : class
        {
            expiry ??= TimeSpan.FromMinutes(5);

            await _redisSafeRunner.Execute(async redisConnection =>
            {
                if (value != null)
                {
                    var stringValue = value is string s ? s : JsonConvert.SerializeObject(value);
                    await redisConnection.GetDatabase().StringSetAsync(key, stringValue, expiry).ConfigureAwait(false);
                }
            }).ConfigureAwait(false);
        }

        public async Task SetHashRedisByKey<T>(string key, string field, T value) where T : class
        {
            await _redisSafeRunner.Execute(async redisConnection =>
            {
                if (value != null)
                {
                    var stringValue = value is string s ? s : JsonConvert.SerializeObject(value);
                    await redisConnection.GetDatabase().HashSetAsync(key, field, stringValue).ConfigureAwait(false);
                }
            }).ConfigureAwait(false);
        }

        public async Task SetRedisByKeys<T>(Dictionary<string, T> values, TimeSpan? expiry = null) where T : class
        {
            await _redisSafeRunner.Execute(async redisConnection =>
            {
                var kvList = values
                    .Select(x => new KeyValuePair<RedisKey, RedisValue>
                    (
                        x.Key,
                        x.Value is string value ? value : JsonConvert.SerializeObject(x.Value)
                    )).ToArray();

                if (kvList.Any())
                {
                    var transaction = redisConnection.GetDatabase().CreateTransaction();
                    expiry ??= DateTimeExtension.GetRedisExpireTimeSpanFromMinutes();
                    foreach (var (k, v) in kvList)
                    {
                        transaction.StringSetAsync(k, v, expiry);
                    }
                    await transaction.ExecuteAsync().ConfigureAwait(false);
                }
            }).ConfigureAwait(false);
        }

        public async Task<T> GetByKeyFromRedis<T>(string key) where T : class
        {
            return await _redisSafeRunner.Execute(async redisConnection =>
            {
                var cachedResult = await redisConnection.GetDatabase().StringGetAsync(key).ConfigureAwait(false);
                return !cachedResult.IsNullOrEmpty
                    ? typeof(T) == typeof(string) ? cachedResult.ToString() as T :
                    JsonConvert.DeserializeObject<T>(cachedResult)
                    : null;
            }).ConfigureAwait(false);
        }

        public async Task<T> GetHashByKeyFromRedis<T>(string key, string field) where T : class
        {
            return await _redisSafeRunner.Execute(async redisConnection =>
            {
                var cachedResult = await redisConnection.GetDatabase().HashGetAsync(key, field).ConfigureAwait(false);
                return !cachedResult.IsNullOrEmpty
                    ? typeof(T) == typeof(string) ? cachedResult.ToString() as T :
                    JsonConvert.DeserializeObject<T>(cachedResult)
                    : null;
            }).ConfigureAwait(false);
        }

        public async Task<T> GetJsonValueByKeyFromRedis<T>(string key, string field) where T : class
        {
            return await _redisSafeRunner.Execute(async redisConnection =>
            {
                var json = await GetByKeyFromRedis<JObject>(key).ConfigureAwait(false);
                if (json != null && json[field] != null)
                {
                    return JsonConvert.DeserializeObject<T>(json[field].ToString());
                }
                else return null;
            }).ConfigureAwait(false);
        }

        public async Task<List<T>> GetByKeysFromRedis<T>(List<string> keys) where T : class
        {
            keys = keys.Distinct().ToList();

            return await _redisSafeRunner.Execute(async redisConnection =>
            {
                var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
                return (await redisConnection.GetDatabase().StringGetAsync(redisKeys).ConfigureAwait(false))
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Select(x => JsonConvert.DeserializeObject<T>(x)).ToList();
            }).ConfigureAwait(false);
        }

        public async Task RemoveByKeyFromRedis(string key)
        {
            await _redisSafeRunner.Execute(async redisConnection =>
            {
                var db = redisConnection.GetDatabase();
                await db.KeyDeleteAsync(key);
            }).ConfigureAwait(false);
        }

        public async Task RemoveByKeysFromRedis(List<string> keys)
        {
            await _redisSafeRunner.Execute(async redisConnection =>
            {
                var db = redisConnection.GetDatabase();
                var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
                await db.KeyDeleteAsync(redisKeys);
            }).ConfigureAwait(false);
        }
    }
}

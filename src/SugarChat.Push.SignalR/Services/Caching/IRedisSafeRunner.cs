using Serilog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Services.Caching
{
    public interface IRedisSafeRunner
    {
        Task Execute(Func<ConnectionMultiplexer, Task> func);

        Task<T> Execute<T>(Func<ConnectionMultiplexer, Task<T>> func) where T : class;
    }

    public class RedisSafeRunner : IRedisSafeRunner
    {
        private readonly ConnectionMultiplexer _redis;

        public RedisSafeRunner(ConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task Execute(Func<ConnectionMultiplexer, Task> func)
        {
            try
            {
                await func(_redis).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogRedisException(ex);
            }
        }

        public async Task<T> Execute<T>(Func<ConnectionMultiplexer, Task<T>> func) where T : class
        {
            try
            {
                return await func(_redis).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogRedisException(ex);
                return default;
            }
        }

        public async Task<List<T>> Execute<T>(Func<ConnectionMultiplexer, Task<List<T>>> func) where T : class
        {
            try
            {
                return await func(_redis).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogRedisException(ex);
                return new List<T>();
            }
        }

        private void LogRedisException(Exception ex)
        {
            Log.Error(ex, "Redis occur error: {ErrorMessage}", ex.Message);
        }
    }
}

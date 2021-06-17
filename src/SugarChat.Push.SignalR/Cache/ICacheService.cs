using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Cache
{
    public interface ICacheService
    {
        Task SetAsync<T>(string key, T value, TimeSpan? timeSpan = null);
        void Set<T>(string key, T value, TimeSpan? timeSpan = null);
        Task<T> GetAsync<T>(string key);
        T Get<T>(string key);

        Task HashSetAsync<T>(string hashId, string key, T value);
        Task<T> HashGetAsync<T>(string hashId, string key);

        Task<IDictionary<string, string>> GetHashAll(string hashId);
    }
}

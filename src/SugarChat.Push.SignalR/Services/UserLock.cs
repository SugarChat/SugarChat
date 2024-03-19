using System.Collections.Concurrent;
using System.Threading;

namespace SugarChat.Push.SignalR.Services
{
    public class UserLock
    {
        public static readonly ConcurrentDictionary<string, SemaphoreSlim> UserSemaphores = new ConcurrentDictionary<string, SemaphoreSlim>();
    }
}

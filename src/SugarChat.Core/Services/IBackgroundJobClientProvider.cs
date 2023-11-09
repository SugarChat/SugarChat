using Hangfire;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SugarChat.Core.Services
{
    public interface IBackgroundJobClientProvider
    {
        string Enqueue(Expression<Func<Task>> methodCall);
    }

    public class BackgroundJobClientProvider : IBackgroundJobClientProvider
    {
        public string Enqueue(Expression<Func<Task>> methodCall)
        {
            return BackgroundJob.Enqueue(methodCall);
        }
    }
}

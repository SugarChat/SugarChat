using Hangfire;
using Serilog;
using System;
using System.Diagnostics;
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
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                var result = BackgroundJob.Enqueue(methodCall);
                sw.Stop();
                Log.Information("BackgroundJob.Enqueue Overall Elapsed:{@Ms}", sw.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "BackgroundJob.Enqueue Error");
                return "error";
            }
        }
    }
}

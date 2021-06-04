using Serilog.Events;
using SugarChat.Core.Basic;

namespace SugarChat.Core.Exceptions
{
    public interface IBusinessException
    {
        LogEventLevel LogLevel { get; }

        StatusCode Code { get; set; }

        string Message { get; }
    }
}
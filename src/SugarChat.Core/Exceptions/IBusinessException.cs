using Serilog.Events;

namespace SugarChat.Core.Exceptions
{
    public interface IBusinessException
    {
        LogEventLevel LogLevel { get; }

    }
}
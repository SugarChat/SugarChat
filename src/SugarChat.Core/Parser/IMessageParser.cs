using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Parser
{
    public interface IMessageParser
    {
        Task ParserMessage(CancellationToken cancellationToken);
    }
}

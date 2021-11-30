using SugarChat.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core
{
    public class RunTimeProvider
    {
        public RunTimeType Type { get; }
        public RunTimeProvider(RunTimeType type)
        {
            Type = type;
        }
    }
}

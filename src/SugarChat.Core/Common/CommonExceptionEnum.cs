using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Common
{
    public enum CommonExceptionEnum : int
    {
        Unknow = -1,
        InternalError = 5000,
        NotFound = 4004
    }
}

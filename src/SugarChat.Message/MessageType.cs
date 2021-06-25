using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message
{
    [Obsolete("使用int，方便其它系统兼容", true)]
    public enum MessageType
    {
        Text,
        Video,
        Image,
        Voice,
        File,
        Other
    }
}

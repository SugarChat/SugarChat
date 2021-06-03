using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message
{
    public enum MessageRemindType
    {
        ACPT_AND_NOTE,//接入侧做提示
        ACPT_NOT_NOTE,//接入侧不做提示
        DISCARD//拒收消息
    }
}
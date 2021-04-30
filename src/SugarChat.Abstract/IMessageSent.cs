using System;
using System.Collections.Generic;
using System.Text;

namespace SugarChat.Abstract
{
    public interface IMessageSent
    {
        Guid MessageId { get; set; }
    }
}

using System;

namespace SugarChat.Core.Domain
{
    public class RegisterInfo : ValueObject
    {
        public DateTime RegisterDateTime { get; private set; }
        public DateTime CloseDateTime { get; private set; }
        public bool IsActive { get; private set; }
    }
}
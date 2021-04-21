namespace SugarChat.Core.Domain
{
    public abstract class Receiver : Entity
    {
        public string AvatarUrl { get; protected set; }
        public RegisterInfo RegisterInfo { get; protected set; }
    }
}
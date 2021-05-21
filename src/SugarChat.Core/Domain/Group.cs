namespace SugarChat.Core.Domain
{
    public class Group : Entity
    {
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public string Description { get; set; }
        public bool IsDel { get; set; }
    }
}

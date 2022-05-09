namespace SugarChat.Core.Domain
{
    public class Emotion : Entity
    {
        public string UserId { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
    }
}
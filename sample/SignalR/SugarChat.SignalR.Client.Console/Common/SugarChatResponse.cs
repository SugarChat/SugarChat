namespace SugarChat.SignalR.Client.ConsoleSample
{
    public class SugarChatResponse<T>
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
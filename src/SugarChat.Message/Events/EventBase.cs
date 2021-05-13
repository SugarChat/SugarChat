using Mediator.Net.Contracts;
using SugarChat.Message.Event;

namespace SugarChat.Message.Events
{
    public class EventBase : IEvent
    {
        public EventStatus Status { get; set; }
        public object Infomation { get; set; }
    }
}
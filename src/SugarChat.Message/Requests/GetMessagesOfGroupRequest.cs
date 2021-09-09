using Mediator.Net.Contracts;
using SugarChat.Message.Paging;
using System;

namespace SugarChat.Message.Requests
{
    public class GetMessagesOfGroupRequest : IRequest
    {
        public string GroupId { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public PageSettings PageSettings { get; set; }
    }
}
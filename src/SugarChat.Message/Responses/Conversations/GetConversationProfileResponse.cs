﻿using Mediator.Net.Contracts;
using SugarChat.Message.Dtos.Conversations;

namespace SugarChat.Message.Responses.Conversations
{
    public class GetConversationProfileResponse : IResponse
    {
        public ConversationDto Result { get; set; }
    }
}

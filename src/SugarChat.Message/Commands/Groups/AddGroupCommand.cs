using System;
using System.Collections.Generic;
using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands.Groups
{
    public class AddGroupCommand : IdRequiredCommand
    {
        public string UserId { get; set; }

        public string Name { get; set; }

        public string AvatarUrl { get; set; }

        public string Description { get; set; }

        public Dictionary<string, string> CustomProperties { get; set; }

        public string CreatedBy { get; set; }
    }
}
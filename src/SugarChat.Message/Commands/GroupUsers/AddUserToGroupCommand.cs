﻿using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands.GroupUsers
{
    public class AddUserToGroupCommand : ICommand
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }
    }
}
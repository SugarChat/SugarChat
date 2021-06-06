﻿using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands.Groups
{
    public class DismissGroupCommand : ICommand
    {
        public string GroupId { get; set; }
    }
}
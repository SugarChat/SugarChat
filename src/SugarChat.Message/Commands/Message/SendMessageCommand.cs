﻿using System;
using System.Collections.Generic;
using System.Text;
using Mediator.Net.Contracts;

namespace SugarChat.Message.Command
{
    public class SendMessageCommand : ICommand
    {
        public string GroupId { get; set; }
        public string Content { get; set; }
        public MessageType Type { get; set; }
        public string SentBy { get; set; }
        public string AttachmentUrl { get; set; }
    }

    public enum MessageType
    {
        Text,
        Video,
        Image,
        Voice,
        File,
        Other
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Commands;
using SugarChat.Core.Services.Messages;
using SugarChat.Message.Messages.Events;

namespace SugarChat.Core.Services
{
    public class SendMessageService : ISendMessageService
    {
        private readonly IMapper _mapper;
        private IMessageDataProvider _messageDataProvider;

        public SendMessageService(IMapper mapper, IMessageDataProvider messageDataProvider)
        {
            _mapper = mapper;
            _messageDataProvider = messageDataProvider;
        }

        public async Task<MessageSentEvent> SendMessageAsync(SendMessageCommand command,
            CancellationToken cancellationToken = default)
        {
            await _messageDataProvider.AddAsync(new Domain.Message
            {
                GroupId = command.GroupId,
                Content = command.Content,
                Type = command.Type,
                SentBy = command.SentBy,
                SentTime = DateTime.Now,
                Payload = command.Payload
            }, cancellationToken).ConfigureAwait(false);

            return new MessageSentEvent();
        }
    }
}
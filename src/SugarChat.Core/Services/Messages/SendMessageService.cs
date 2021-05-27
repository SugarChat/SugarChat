﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Commands;

namespace SugarChat.Core.Services
{
    public class SendMessageService : ISendMessageService
    {
        private IRepository _repository;
        private IMapper _mapper;
        public SendMessageService(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public Task SendMessage(SendMessageCommand command, CancellationToken cancellationToken)
        {
            //todo  只是demo，未写完逻辑
            //todo  add parser to ParsedContent
            var message = _mapper.Map<Domain.Message>(command);
            return _repository.AddAsync(message, cancellationToken);
        }
    }
}

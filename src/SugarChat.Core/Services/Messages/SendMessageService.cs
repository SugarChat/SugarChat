using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Command;

namespace SugarChat.Core.Services
{
    public class SendMessageService : ISendMessageService
    {
        private IRepository _repository;
        public SendMessageService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task SendMessage(SendMessageCommand command, CancellationToken cancellationToken)
        {
            var users = from a in _repository.Query<User>()
                        join b in _repository.Query<GroupUser>() on a.Id equals b.UserId
                        where b.GroupId == command.GroupId
                        select a;
            foreach (var user in users)
            {
                await _repository.AddAsync(new Domain.Message
                {
                    GroupId = command.GroupId,
                    Content = command.Content,
                    Type = command.Type,
                    SentBy = "",
                    SentTime = DateTime.Now,
                    AttachmentUrl = command.AttachmentUrl
                }, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}

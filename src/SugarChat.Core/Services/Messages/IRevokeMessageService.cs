using SugarChat.Core.IRepositories;
using SugarChat.Message.Commands.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Messages
{
    public interface IRevokeMessageService : IService
    {
        Task RevokeMessage(RevokeMessageCommand command, CancellationToken cancellationToken);
    }

    public class RevokeMessageService : IRevokeMessageService
    {
        private IRepository _repository;
        public RevokeMessageService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task RevokeMessage(RevokeMessageCommand command, CancellationToken cancellationToken)
        {
            var message = await _repository.FirstOrDefaultAsync<Domain.Message>(x => x.Id == command.MessageId);
            if (message == null)
            {
                throw new Exception("message is not defind");
            }
            message.IsDel = true;
            await _repository.UpdateAsync(message);
        }
    }
}

using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Messages
{
    public class TranslateMessageDataProvider : ITranslateMessageDataProvider
    {
        private readonly IRepository _repository;

        public TranslateMessageDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<MessageTranslate> GetByMessageIdAndLuaguageCodeAsync(string messageId, string languageCode, CancellationToken cancellation)
        {
            return await _repository.SingleOrDefaultAsync<MessageTranslate>(x => x.MessageId == messageId && x.LanguageCode == languageCode);
        }

        public async Task AddAsync(MessageTranslate messageTranslate, CancellationToken cancellation)
        {
            await _repository.AddAsync(messageTranslate, cancellation);
        }
    }
}

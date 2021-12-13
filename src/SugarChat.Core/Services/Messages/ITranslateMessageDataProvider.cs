using SugarChat.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Messages
{
    public interface ITranslateMessageDataProvider : IDataProvider
    {
        Task<MessageTranslate> GetByMessageIdAndLuaguageCodeAsync(string messageId, string languageCode, CancellationToken cancellation);

        Task AddAsync(MessageTranslate messageTranslate, CancellationToken cancellation);
    }
}

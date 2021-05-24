using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Conversations
{
    public interface IConversationService : IService
    {
        Task<GetConversationListByUserIdResponse> GetConversationListByUserIdAsync(GetConversationListByUserIdRequest request, CancellationToken cancellation);
    }
}

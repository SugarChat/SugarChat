using SugarChat.Message.Commands.Conversations;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Conversations
{
    public interface IConversationService : IService
    {
        Task<GetConversationListByUserIdResponse> GetConversationListByUserIdAsync(GetConversationListByUserIdRequest request, CancellationToken cancellationToken);
        Task<GetConversationProfileByIdResponse> GetConversationProfileByIdRequestAsync(GetConversationProfileByIdRequest request, CancellationToken cancellationToken);
        Task DeleteConversationByIdAsync(DeleteConversationCommand command, CancellationToken cancellationToken);
        Task SetMessageReadByConversationIdAsync(SetMessageReadCommand command, CancellationToken cancellationToken);
    }
}

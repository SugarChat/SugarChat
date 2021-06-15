using System.Threading;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Requests;
using SugarChat.Message.Requests.SignalR;
using SugarChat.SignalR.ServerClient;

namespace SugarChat.Core.Mediator.RequestHandlers.SignalR
{
    public class GetConnectionUrlRequestHandler : IRequestHandler<GetConnectionUrlRequest, SugarChatResponse<string>>
    {
        private readonly IUserService _userService;
        private readonly IServerClient _client;

        public GetConnectionUrlRequestHandler(IUserService userService, IServerClient client)
        {
            _userService = userService;
            _client = client;
        }

        public async Task<SugarChatResponse<string>> Handle(IReceiveContext<GetConnectionUrlRequest> context,
            CancellationToken cancellationToken)
        {
            GetUserRequest request = new GetUserRequest {Id = context.Message.UserId};
            var response = await _userService.GetUserAsync(request, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<string>
                {Data = await _client.GetConnectionUrl(response.User.Id).ConfigureAwait(false)};
        }
    }
}
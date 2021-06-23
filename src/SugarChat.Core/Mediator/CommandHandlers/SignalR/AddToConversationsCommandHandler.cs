using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Groups;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.SignalR;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Events.SignalR;
using SugarChat.Message.Requests;
using SugarChat.SignalR.Enums;
using SugarChat.SignalR.Server.Models;
using SugarChat.SignalR.ServerClient;

namespace SugarChat.Core.Mediator.CommandHandlers.SignalR
{
    public class AddToConversationsCommandHandler : ICommandHandler<AddToConversationsCommand, SugarChatResponse>
    {
        private readonly IGroupService _groupService;
        private readonly IServerClient _client;
        private readonly IMapper _mapper;

        public AddToConversationsCommandHandler(IGroupService groupService, IServerClient client, IMapper mapper)
        {
            _groupService = groupService;
            _client = client;
            _mapper = mapper;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<AddToConversationsCommand> context, CancellationToken cancellationToken)
        {
            GetGroupIdsOfUserRequest request = new GetGroupIdsOfUserRequest {Id = context.Message.UserId};
            var response =
                await _groupService.GetGroupIdsOfUserAsync(request, cancellationToken).ConfigureAwait(false);

            await _client.Group(new GroupActionModel
                {Action = GroupAction.Add, GroupNames = response.GroupIds, UserIdentifier = request.Id});

            AddedToConversationsEvent addedToConversationsEvent =
                _mapper.Map<AddedToConversationsEvent>(context.Message);
            await context.PublishAsync(addedToConversationsEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}

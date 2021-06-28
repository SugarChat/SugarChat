using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.GroupUsers
{
    public class GetGroupMembersRequestHandler : IRequestHandler<GetGroupMembersRequest, SugarChatResponse<IEnumerable<string>>>
    {
        private readonly IGroupUserService _groupUserService;

        public GetGroupMembersRequestHandler(IGroupUserService groupUserService)
        {
            _groupUserService = groupUserService;
        }

        public async Task<SugarChatResponse<IEnumerable<string>>> Handle(IReceiveContext<GetGroupMembersRequest> context, CancellationToken cancellationToken)
        {
            var response = await _groupUserService.GetGroupMemberIdsAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<IEnumerable<string>>() { Data = response.MemberIds };
        }
    }
}

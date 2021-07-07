using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Groups;
using SugarChat.Message.Requests.Groups;
using SugarChat.Message.Dtos;
using SugarChat.Message.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.RequestHandlers.Groups
{
    public class GetGroupByCustomPropertiesRequestHandler : IRequestHandler<GetGroupByCustomPropertiesRequest, SugarChatResponse<IEnumerable<GroupDto>>>
    {
        private readonly IGroupService _groupService;

        public GetGroupByCustomPropertiesRequestHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task<SugarChatResponse<IEnumerable<GroupDto>>> Handle(IReceiveContext<GetGroupByCustomPropertiesRequest> context, CancellationToken cancellationToken)
        {
            var response = await _groupService.GetByCustomProperties(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<IEnumerable<GroupDto>>() { Data = response };
        }
    }
}

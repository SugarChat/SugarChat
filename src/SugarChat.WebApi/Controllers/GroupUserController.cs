using Mediator.Net;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Core.Basic;
using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Requests;
using SugarChat.Shared.Dtos.GroupUsers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SugarChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupUserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GroupUserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("getGroupMemberList"), HttpGet]
        public async Task<IActionResult> GetGroupMemberList([FromQuery] GetMembersOfGroupRequest request)
        {
            var response =
                 await _mediator
                     .RequestAsync<GetMembersOfGroupRequest, SugarChatResponse<IEnumerable<GroupUserDto>>>(request);

            return Ok(response);
        }

        [Route("setGroupMemberCustomField"), HttpPost]
        public async Task<IActionResult> SetGroupMemberCustomField(SetGroupMemberCustomFieldCommand command)
        {
            await _mediator.SendAsync(command);
            return Ok();
        }
    }
}

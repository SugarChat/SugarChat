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

        [Route("join"), HttpPost]
        public async Task<IActionResult> JoinGroup(JoinGroupCommand command)
        {
            var response = await _mediator.SendAsync<JoinGroupCommand, SugarChatResponse<object>>(command);
            return Ok(response);
        }

        [Route("quit"), HttpPost]
        public async Task<IActionResult> QuitGroup(QuitGroupCommand command)
        {
            var response = await _mediator.SendAsync<QuitGroupCommand, SugarChatResponse<object>>(command);
            return Ok(response);
        }

        [Route("changeGroupOwner"), HttpPost]
        public async Task<IActionResult> ChangeGroupOwner(ChangeGroupOwnerCommand command)
        {
            var response = await _mediator.SendAsync<ChangeGroupOwnerCommand, SugarChatResponse<object>>(command);
            return Ok(response);
        }

        [Route("addGroupMember"), HttpPost]
        public async Task<IActionResult> AddGroupMember(AddGroupMemberCommand command)
        {
            var response = await _mediator.SendAsync<AddGroupMemberCommand, SugarChatResponse<object>>(command);
            return Ok(response);
        }

        [Route("deleteGroupMember"), HttpPost]
        public async Task<IActionResult> DeleteGroupMember(AddGroupMemberCommand command)
        {
            var response = await _mediator.SendAsync<AddGroupMemberCommand, SugarChatResponse<object>>(command);
            return Ok(response);
        }

        [Route("setMessageRemindType"), HttpPost]
        public async Task<IActionResult> SetMessageRemindType(SetMessageRemindTypeCommand command)
        {
            var response = await _mediator.SendAsync<SetMessageRemindTypeCommand, SugarChatResponse<object>>(command);
            return Ok(response);
        }

        [Route("setGroupMemberRole"), HttpPost]
        public async Task<IActionResult> SetGroupMemberRole(SetGroupMemberRoleCommand command)
        {
            var response = await _mediator.SendAsync<SetGroupMemberRoleCommand, SugarChatResponse<object>>(command);
            return Ok(response);
        }
    }
}

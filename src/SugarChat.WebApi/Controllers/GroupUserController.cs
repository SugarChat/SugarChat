using Mediator.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Requests;
using SugarChat.Message.Requests.GroupUsers;
using SugarChat.Message.Dtos.GroupUsers;
using System.Collections.Generic;
using System.Threading.Tasks;
using SugarChat.Message.Basic;

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
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse<IEnumerable<GroupUserDto>>))]
        public async Task<IActionResult> GetGroupMemberList([FromQuery] GetMembersOfGroupRequest request)
        {
            var response =
                 await _mediator
                     .RequestAsync<GetMembersOfGroupRequest, SugarChatResponse<IEnumerable<GroupUserDto>>>(request);

            return Ok(response);
        }

        [Route("setGroupMemberCustomField"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> SetGroupMemberCustomField(SetGroupMemberCustomFieldCommand command)
        {
            var response = await _mediator.SendAsync<SetGroupMemberCustomFieldCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("join"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> JoinGroup(JoinGroupCommand command)
        {
            var response = await _mediator.SendAsync<JoinGroupCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("quit"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> QuitGroup(QuitGroupCommand command)
        {
            var response = await _mediator.SendAsync<QuitGroupCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("changeGroupOwner"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> ChangeGroupOwner(ChangeGroupOwnerCommand command)
        {
            var response = await _mediator.SendAsync<ChangeGroupOwnerCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("addGroupMember"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> AddGroupMember(AddGroupMemberCommand command)
        {
            var response = await _mediator.SendAsync<AddGroupMemberCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("deleteGroupMember"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> DeleteGroupMember(RemoveGroupMemberCommand command)
        {
            var response = await _mediator.SendAsync<RemoveGroupMemberCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("setMessageRemindType"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> SetMessageRemindType(SetMessageRemindTypeCommand command)
        {
            var response = await _mediator.SendAsync<SetMessageRemindTypeCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("setGroupMemberRole"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> SetGroupMemberRole(SetGroupMemberRoleCommand command)
        {
            var response = await _mediator.SendAsync<SetGroupMemberRoleCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("AddUserToGroup"), HttpPost]
        public async Task<IActionResult> AddUserToGroup(AddUserToGroupCommand command)
        {
            var response = await _mediator.SendAsync<AddUserToGroupCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("RemoveUserFromGroup"), HttpPost]
        public async Task<IActionResult> RemoveUserFromGroup(RemoveUserFromGroupCommand command)
        {
            var response = await _mediator.SendAsync<RemoveUserFromGroupCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("getGroupMemberIds"), HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse<IEnumerable<string>>))]
        public async Task<IActionResult> GetGroupMemberIds([FromQuery] GetGroupMembersRequest request)
        {
            var response = await _mediator.RequestAsync<GetGroupMembersRequest, SugarChatResponse<IEnumerable<string>>>(request);
            return Ok(response);
        }

        [Route("getUserIdsByGroupIds"), HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse<IEnumerable<string>>))]
        public async Task<IActionResult> GetUsersByGroupIdsForGet([FromQuery] GetUserIdsByGroupIdsRequest request)
        {
            var response = await _mediator.RequestAsync<GetUserIdsByGroupIdsRequest, SugarChatResponse<IEnumerable<string>>>(request);
            return Ok(response);
        }

        [Route("getUserIdsByGroupIds"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse<IEnumerable<string>>))]
        public async Task<IActionResult> GetUsersByGroupIdsForPost(GetUserIdsByGroupIdsRequest request)
        {
            var response = await _mediator.RequestAsync<GetUserIdsByGroupIdsRequest, SugarChatResponse<IEnumerable<string>>>(request);
            return Ok(response);
        }

        [Route("UpdateGroupUserData"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> UpdateGroupUserData(UpdateGroupUserDataCommand command)
        {
            var response = await _mediator.SendAsync<UpdateGroupUserDataCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        /// <summary>
        /// 迁移数据使用，一次性代码
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Route("MigrateCustomPropertyWhenRoleEqual0"), HttpPost]
        public async Task<IActionResult> MigrateCustomPropertyWhenRoleEqual0(MigrateGroupUserCustomPropertyCommand command)
        {
            var response = await _mediator.SendAsync<MigrateGroupUserCustomPropertyCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("JudgeUserInGroup"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse<bool>))]
        public async Task<IActionResult> JudgeUserInGroup(JudgeUserInGroupCommand command)
        {
            var response = await _mediator.SendAsync<JudgeUserInGroupCommand, SugarChatResponse<bool>>(command);
            return Ok(response);
        }
    }
}

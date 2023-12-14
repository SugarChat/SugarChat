using Mediator.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Core.Mediator.CommandHandlers.Groups;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Requests;
using SugarChat.Message.Requests.Groups;
using SugarChat.Message.Responses;
using SugarChat.Message.Responses.Groups;
using SugarChat.Message.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using SugarChat.Message.Paging;
using SugarChat.Message.Basic;

namespace SugarChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("create"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> CreateGroup(AddGroupCommand command)
        {
            var response = await _mediator.SendAsync<AddGroupCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("batchCreate"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> BatchCreateGroup(BatchAddGroupCommand command)
        {
            var response = await _mediator.SendAsync<BatchAddGroupCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("dismiss"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> DismissGroup(DismissGroupCommand command)
        {
            var response = await _mediator.SendAsync<DismissGroupCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("getGroupList"), HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse<PagedResult<GroupDto>>))]
        public async Task<IActionResult> GetGroupList([FromQuery] GetGroupsOfUserRequest request)
        {
            var response =
                  await _mediator
                      .RequestAsync<GetGroupsOfUserRequest, SugarChatResponse<PagedResult<GroupDto>>>(request);

            return Ok(response);
        }

        [Route("getGroupProfile"), HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse<GroupDto>))]
        public async Task<IActionResult> GetGroupProfile([FromQuery] GetGroupProfileRequest request)
        {
            var response =
                  await _mediator
                      .RequestAsync<GetGroupProfileRequest, SugarChatResponse<GroupDto>>(request);

            return Ok(response);
        }

        [Route("updateGroupProfile"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> UpdateGroupProfile(UpdateGroupProfileCommand command)
        {
            var response = await _mediator.SendAsync<UpdateGroupProfileCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("remove"), HttpPost]
        public async Task<IActionResult> RemoveGroup(RemoveGroupCommand command)
        {
            var response = await _mediator.SendAsync<RemoveGroupCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("getByCustomProperties"), HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse<IEnumerable<GroupDto>>))]
        public async Task<IActionResult> GetByCustomProperties([FromQuery] GetGroupByCustomPropertiesRequest request)
        {
            var response =await _mediator.RequestAsync<GetGroupByCustomPropertiesRequest, SugarChatResponse<IEnumerable<GroupDto>>>(request);
            return Ok(response);
        }

        [Route("MigrateCustomProperty"), HttpPost]
        public async Task<IActionResult> MigrateCustomProperty(MigrateGroupCustomPropertyCommand command)
        {
            var response = await _mediator.SendAsync<MigrateGroupCustomPropertyCommand, SugarChatResponse>(command);
            return Ok(response);
        }
    }
}

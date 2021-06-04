using Mediator.Net;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Core.Basic;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Requests;
using SugarChat.Message.Requests.Groups;
using SugarChat.Message.Responses;
using SugarChat.Message.Responses.Groups;
using SugarChat.Shared.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        [Route("getGroupList"), HttpGet]
        public async Task<IActionResult> GetGroupList([FromQuery] GetGroupsOfUserRequest request)
        {
            var response =
                  await _mediator
                      .RequestAsync<GetGroupsOfUserRequest, SugarChatResponse<IEnumerable<GroupDto>>>(request);

            return Ok(response);
        }

        [Route("getGroupProfile"), HttpGet]
        public async Task<IActionResult> GetGroupProfile([FromQuery] GetGroupProfileRequest request)
        {
            var response =
                  await _mediator
                      .RequestAsync<GetGroupProfileRequest, SugarChatResponse<GroupDto>>(request);

            return Ok(response);
        }

        [Route("updateGroupProfile"), HttpPost]
        public async Task<IActionResult> UpdateGroupProfile(UpdateGroupProfileCommand command)
        {
            var response = await _mediator.SendAsync<UpdateGroupProfileCommand, SugarChatResponse<object>>(command);
            return Ok(response);
        }

    }
}

using Mediator.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Core.Basic;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Requests;
using SugarChat.Message.Dtos;
using SugarChat.Message.Paging;
using System.Threading.Tasks;

namespace SugarChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("getUserProfile"), HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse<UserDto>))]
        public async Task<IActionResult> GetUserProfile([FromQuery] GetUserRequest request)
        {
            var response =
                  await _mediator
                      .RequestAsync<GetUserRequest, SugarChatResponse<UserDto>>(request);

            return Ok(response);
        }

        [Route("updateMyProfile"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> UpdateMyProfile(UpdateUserCommand command)
        {
            var response = await _mediator.SendAsync<UpdateUserCommand, SugarChatResponse>(command);
            return Ok(response);
        }
        
        [Route("create"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> CreateUser(AddUserCommand command)
        {
            var response = await _mediator.SendAsync<AddUserCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("add"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> AddUser(AddUserCommand command)
        {
            var response = await _mediator.SendAsync<AddUserCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("remove"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> RemoveUser(RemoveUserCommand command)
        {
            var response = await _mediator.SendAsync<RemoveUserCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("getCurrentUser"), HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse<UserDto>))]
        public async Task<IActionResult> GetCurrentUser([FromQuery] GetCurrentUserRequest request)
        {
            var response = await _mediator.RequestAsync<GetCurrentUserRequest, SugarChatResponse<UserDto>>(request);
            return Ok(response);
        }

        [Route("getFriendsOfUser"), HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse<PagedResult<UserDto>>))]
        public async Task<IActionResult> GetFriendsOfUser([FromQuery] GetFriendsOfUserRequest request)
        {
            var response = await _mediator.RequestAsync<GetFriendsOfUserRequest, SugarChatResponse<PagedResult<UserDto>>>(request);
            return Ok(response);
        }

        [Route("batchAddUsers"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse))]
        public async Task<IActionResult> AddUser(BatchAddUsersCommand command)
        {
            var response = await _mediator.SendAsync<BatchAddUsersCommand, SugarChatResponse>(command);
            return Ok(response);
        }
    }
}

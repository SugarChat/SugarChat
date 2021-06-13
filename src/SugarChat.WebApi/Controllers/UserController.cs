﻿using Mediator.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Core.Basic;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Requests;
using SugarChat.Shared.Dtos;
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
        public async Task<IActionResult> CreateUserAsync(AddUserCommand command)
        {
            var response = await _mediator.SendAsync<AddUserCommand, SugarChatResponse>(command);
            return Ok(response);
        }


    }
}

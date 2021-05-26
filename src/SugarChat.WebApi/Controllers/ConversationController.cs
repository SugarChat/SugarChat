﻿using Mediator.Net;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Message.Requests;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses;
using SugarChat.Message.Responses.Conversations;
using System.Threading.Tasks;

namespace SugarChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConversationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("getAllToUserFromGroup"), HttpGet]
        public async Task<IActionResult> GetAllToUserFromGroup(GetAllToUserFromGroupRequest request)
        {
            var response =
                 await _mediator
                     .RequestAsync<GetAllToUserFromGroupRequest, GetAllToUserFromGroupResponse>(request);

            return Ok(response);
        }

        [Route("getConversationList"), HttpGet]
        public async Task<IActionResult> GetConversationListByUserId([FromQuery] GetConversationListByUserIdRequest request)
        {
            var response =
                  await _mediator
                      .RequestAsync<GetConversationListByUserIdRequest, GetConversationListByUserIdResponse>(request);

            return Ok(response);
        }

    }
}
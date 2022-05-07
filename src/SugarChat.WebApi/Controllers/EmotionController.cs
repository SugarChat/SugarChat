using Mediator.Net;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.Friends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SugarChat.Message.Commands.Emotions;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Dtos.Emotions;
using SugarChat.Message.Requests.Emotions;

namespace SugarChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmotionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmotionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("add"), HttpPost]
        public async Task<IActionResult> AddFriend(AddEmotionCommand command)
        {
            var response = await _mediator.SendAsync<AddEmotionCommand, SugarChatResponse>(command);
            return Ok(response);
        }

        [Route("remove"), HttpPost]
        public async Task<IActionResult> RemoveFriend(RemoveEmotionCommand command)
        {
            var response = await _mediator.SendAsync<RemoveEmotionCommand, SugarChatResponse>(command);
            return Ok(response);
        }
        
        [Route("getUserEmotions"), HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse<IEnumerable<EmotionDto>>))]
        public async Task<IActionResult> GetUserEmotions([FromQuery] GetUserEmotionsRequest request)
        {
            var response =await _mediator.RequestAsync<GetUserEmotionsRequest, SugarChatResponse<IEnumerable<EmotionDto>>>(request);
            return Ok(response);
        }
        
        [Route("getEmotionByIds"), HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse<IEnumerable<EmotionDto>>))]
        public async Task<IActionResult> GetEmotionByIds([FromQuery] GetEmotionByIdsRequest request)
        {
            var response =await _mediator.RequestAsync<GetEmotionByIdsRequest, SugarChatResponse<IEnumerable<EmotionDto>>>(request);
            return Ok(response);
        }
    }
}

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
using SugarChat.Message.Dtos.Configurations;
using SugarChat.Message.Dtos.Emotions;
using SugarChat.Message.Requests.Configurations;
using SugarChat.Message.Requests.Emotions;

namespace SugarChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConfigurationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [Route("getConfigurations"), HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(SugarChatResponse<ServerConfigurationsDto>))]
        public async Task<IActionResult> GetConfigurations([FromQuery] GetServerConfigurationsRequest request)
        {
            var response =await _mediator.RequestAsync<GetServerConfigurationsRequest, SugarChatResponse<ServerConfigurationsDto>>(request);
            return Ok(response);
        }
    }
}

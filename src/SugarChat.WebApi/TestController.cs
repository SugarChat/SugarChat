using Mediator.Net;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Core.Mediator.CommandHandlers.Groups;
using SugarChat.Message.Commands.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SugarChat.WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("create"), HttpPost]
        public async Task<IActionResult> CreateGroup(AddGroupCommand command)
        {
            var response = await _mediator.SendAsync<AddGroupCommand, AddGroupResponse>(command);
            return Ok(response);
        }
    }
}

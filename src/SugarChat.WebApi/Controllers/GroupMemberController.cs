using Mediator.Net;
using Microsoft.AspNetCore.Mvc;
using SugarChat.Message.Commands.GroupMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SugarChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupMemberController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GroupMemberController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("addGroupMember"), HttpPost]
        public async Task<IActionResult> AddGroupMember(AddGroupMemberCommand command)
        {
            await _mediator.SendAsync(command);
            return Ok();
        }
    }
}

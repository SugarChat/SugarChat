using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SugarChat.SignalR.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok();
        }
        [HttpGet("ready")]
        public IActionResult Ready()
        {
            return Ok();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SugarChat.Core.Services.Admin;
using System.Threading.Tasks;

namespace SugarChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminDataProvider _adminDataProvider;

        public AdminController(IAdminDataProvider adminDataProvider)
        {
            _adminDataProvider = adminDataProvider;
        }

        [Route("RepairData"), HttpPost]
        public IActionResult RepairData()
        {
            _adminDataProvider.RepairData();
            return Ok();
        }
    }
}

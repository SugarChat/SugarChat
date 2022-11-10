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

        /// <summary>
        /// 修复数据，临时使用，不提供HttpClient
        /// </summary>
        /// <returns></returns>
        [Route("RepairData"), HttpPost]
        public IActionResult RepairData()
        {
            _adminDataProvider.RepairData();
            return Ok();
        }
    }
}

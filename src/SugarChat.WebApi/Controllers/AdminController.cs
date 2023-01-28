using Microsoft.AspNetCore.Mvc;
using SugarChat.Core.Services.Admin;
using SugarChat.Core.Services.GroupUsers;
using System.Threading.Tasks;

namespace SugarChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminDataProvider _adminDataProvider;
        private readonly IGroupUserService _groupUserService;

        public AdminController(IAdminDataProvider adminDataProvider, IGroupUserService groupUserService)
        {
            _adminDataProvider = adminDataProvider;
            _groupUserService = groupUserService;
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

        [Route("MigrateGroupCustomPropertyAsyncToGroupUser2"), HttpPost]
        public IActionResult MigrateGroupCustomPropertyAsyncToGroupUser2()
        {
            _groupUserService.MigrateGroupCustomPropertyAsyncToGroupUser2();
            return Ok();
        }

        /// <summary>
        /// 修复数据，临时使用，不提供HttpClient
        /// </summary>
        /// <returns></returns>
        [Route("LinqTest"), HttpPost]
        public IActionResult LinqTest()
        {
            _adminDataProvider.LinqTest();
            return Ok();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SugarChat.Core.Services.Admin;
using SugarChat.Core.Services.Groups;
using SugarChat.Core.Services.GroupUsers;
using System.Threading;

namespace SugarChat.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminDataProvider _adminDataProvider;
        private readonly IGroupUserService _groupUserService;
        private readonly IGroupService _groupService;

        public AdminController(IAdminDataProvider adminDataProvider, IGroupUserService groupUserService, IGroupService groupService)
        {
            _adminDataProvider = adminDataProvider;
            _groupUserService = groupUserService;
            _groupService = groupService;
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

        /// <summary>
        /// 修复数据，临时使用，不提供HttpClient
        /// </summary>
        /// <returns></returns>
        [Route("MigrateGroupCustomPropertyAsyncToGroupUser"), HttpGet]
        public IActionResult MigrateGroupCustomPropertyAsyncToGroupUser(int pageSize)
        {
            _groupUserService.MigrateGroupCustomPropertyAsyncToGroupUser(pageSize);
            return Ok();
        }

        /// <summary>
        /// 修复数据，临时使用，不提供HttpClient
        /// </summary>
        /// <returns></returns>
        [Route("MigrateDataToGroup2"), HttpPost]
        public IActionResult MigrateDataToGroups2(int pageSize)
        {
            _groupService.MigrateDataToGroups2(pageSize, CancellationToken.None);
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

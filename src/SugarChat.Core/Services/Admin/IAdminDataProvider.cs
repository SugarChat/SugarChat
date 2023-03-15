using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Admin
{
    public interface IAdminDataProvider : IDataProvider
    {
        void RepairData();

        Task LinqTest();
    }
}

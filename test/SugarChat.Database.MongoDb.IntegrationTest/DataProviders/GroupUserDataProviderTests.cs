using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Shared.Paging;
using Xunit;

namespace SugarChat.Database.MongoDb.IntegrationTest.DataProviders
{
    public class GroupUserDataProviderTests : ServiceFixture
    {
        private readonly IGroupUserDataProvider _groupUserDataProvider;

        public GroupUserDataProviderTests(DatabaseFixture dbFixture) : base(dbFixture)
        {
            _groupUserDataProvider = new GroupUserDataProvider(Repository);
        }
        
        
    }
}
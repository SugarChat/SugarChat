using Xunit;

namespace SugarChat.Database.MongoDb.IntegrationTest
{
    [CollectionDefinition("DatabaseCollection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
    }
}
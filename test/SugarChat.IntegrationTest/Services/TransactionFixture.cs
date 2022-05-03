using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.IntegrationTest.Services
{
    public class TransactionFixture : TestBase
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestTransaction(bool isThrow)
        {
            await Run<ITransactionManager, IRepository>(async (transactionManager, repository) =>
            {
                using (var transaction = await transactionManager.BeginTransactionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        var users = new List<User>();
                        var groups = new List<Group>();
                        for (int i = 0; i < 10; i++)
                        {
                            users.Add(new User { Id = i.ToString() });
                            groups.Add(new Group { Id = i.ToString() });
                        }
                        await repository.AddRangeAsync(users);
                        if (isThrow)
                        {
                            throw new Exception();
                        }
                        await repository.AddRangeAsync(groups);
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                    }
                }
                if (isThrow)
                {
                    (await repository.CountAsync<User>()).ShouldBe(0);
                    (await repository.CountAsync<Group>()).ShouldBe(0);
                }
                else
                {
                    (await repository.CountAsync<User>()).ShouldBe(10);
                    (await repository.CountAsync<Group>()).ShouldBe(10);
                }
            });
        }
    }
}

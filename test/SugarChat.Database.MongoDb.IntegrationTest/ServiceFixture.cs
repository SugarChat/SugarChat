using System;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using Xunit;

namespace SugarChat.Database.MongoDb.IntegrationTest
{
    public class ServiceFixture : TestBase
    {
        public DateTimeOffset BaseTime = new(2021, 1, 1, 0, 0, 0, default);
        
        public ServiceFixture()
        {
            StuffRepository();
        }

        private void StuffRepository()
        {
            Stuff().Wait();
        }

        private async Task Stuff()
        {
            await Client.DropDatabaseAsync(DbName);
            await AddUsers();
            await AddFriends();
            await AddGroups();
            await AddMessages();
        }

        private async Task AddMessages()
        {
            Core.Domain.Message messageOfGroupTomAndJerry1 = new()
            {
                Id = "1",
                Content = "Hello Jerry",
                SentBy = "1",
                SentTime = BaseTime,
                GroupId = "1"
            };
            
            Core.Domain.Message messageOfGroupTomAndJerry2 = new()
            {
                Id = "2",
                Content = "Hello Tom",
                SentBy = "2",
                SentTime = BaseTime,
                GroupId = "2"
            };
            
            Core.Domain.Message messageOfGroupTomAndJerryAndTyke1 = new()
            {
                Id = "3",
                Content = "Hello Jerry and Tyke",
                SentBy = "1",
                SentTime = BaseTime,
                GroupId = "2"
            };
            
            Core.Domain.Message messageOfGroupTomAndJerryAndTyke2 = new()
            {
                Id = "4",
                Content = "Hello Tom and Jerry",
                SentBy = "4",
                SentTime = BaseTime,
                GroupId = "2"
            };
            await Repository.AddAsync(messageOfGroupTomAndJerry1);
            await Repository.AddAsync(messageOfGroupTomAndJerry2);
            await Repository.AddAsync(messageOfGroupTomAndJerryAndTyke1);
            await Repository.AddAsync(messageOfGroupTomAndJerryAndTyke2);

        }

        private async Task AddGroups()
        {
            Group tomAndJerry = new()
            {
                Id = "1"
            };
            GroupUser tomInTomAndJerry = new()
            {
                Id = "1",
                UserId = "1",
                GroupId = "1"
            };
            GroupUser jerryInTomAndJerry = new()
            {
                Id = "2",
                UserId = "2",
                GroupId = "1"
            };
            await Repository.AddAsync(tomAndJerry);
            await Repository.AddAsync(tomInTomAndJerry);
            await Repository.AddAsync(jerryInTomAndJerry);
            
            
            Group tomAndJerryAndTyke = new()
            {
                Id = "2"
            };
            GroupUser tomInTomAndJerryAndTyke = new()
            {
                Id = "3",
                UserId = "1",
                GroupId = "2"
            };
            GroupUser jerryInTomAndJerryAndTyke = new()
            {
                Id = "4",
                UserId = "2",
                GroupId = "2"
            };
            GroupUser tykeInTomAndJerryAndTyke = new()
            {
                Id = "5",
                UserId = "4",
                GroupId = "2"
            };
            await Repository.AddAsync(tomAndJerryAndTyke);
            await Repository.AddAsync(tomInTomAndJerryAndTyke);
            await Repository.AddAsync(jerryInTomAndJerryAndTyke);
            await Repository.AddAsync(tykeInTomAndJerryAndTyke);
        }

        private async Task AddFriends()
        {
            Friend tomAndJerry = new()
            {
                Id = "1",
                UserId = "1",
                FriendId = "2"
            };
            Friend spikeAndTyke = new()
            {
                Id = "2",
                UserId = "3",
                FriendId = "4"
            };
            await Repository.AddAsync(tomAndJerry);
            await Repository.AddAsync(spikeAndTyke);
        }

        private async Task AddUsers()
        {
            User tom = new()
            {
                Id = "1",
                DisplayName = "Tom"
            };

            User jerry = new()
            {
                Id = "2",
                DisplayName = "Jerry"
            };
            User spike = new()
            {
                Id = "3",
                DisplayName = "Spike"
            };
            User tyke = new()
            {
                Id = "4",
                DisplayName = "Tyke"
            };
            await Repository.AddAsync(tom);
            await Repository.AddAsync(jerry);
            await Repository.AddAsync(spike);
            await Repository.AddAsync(tyke);
        }

        public override async ValueTask DisposeAsync()
        {
            await Client.DropDatabaseAsync(DbName);
            await base.DisposeAsync();
        }

        public override void Dispose()
        {
            Client.DropDatabase(DbName);
            base.Dispose();
        }
    }
}
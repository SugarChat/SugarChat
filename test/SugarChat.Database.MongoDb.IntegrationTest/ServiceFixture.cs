using System;
using System.Threading.Tasks;
using SugarChat.Core.Domain;

namespace SugarChat.Database.MongoDb.IntegrationTest
{
    public class ServiceFixture : TestBase
    {
        protected DateTimeOffset BaseTime = new(2021, 1, 1, 0, 0, 0, default);
        protected Core.Domain.Message MessageOfGroupTomAndJerry1;
        protected Core.Domain.Message MessageOfGroupTomAndJerry2;
        protected Core.Domain.Message MessageOfGroupTomAndJerryAndTyke1;
        protected Core.Domain.Message MessageOfGroupTomAndJerryAndTyke2;
        protected Group TomAndJerryGroup;
        protected GroupUser TomInTomAndJerry;
        protected GroupUser JerryInTomAndJerry;
        protected Group TomAndJerryAndTykeGroup;
        protected GroupUser TomInTomAndJerryAndTyke;
        protected GroupUser JerryInTomAndJerryAndTyke;
        protected GroupUser TykeInTomAndJerryAndTyke;
        protected Friend TomAndJerryFriend;
        protected Friend SpikeAndTyke;
        protected User Tom;
        protected User Jerry;
        protected User Spike;
        protected User Tyke;

        public ServiceFixture(DatabaseFixture dbFixture) : base(dbFixture)
        {
            Stuff().Wait();
        }

        private async Task Stuff()
        {
            await AddUsers();
            await AddFriends();
            await AddGroups();
            await AddMessages();
        }

        private async Task AddMessages()
        {
            MessageOfGroupTomAndJerry1 = new()
            {
                Id = "1",
                Content = "Hello Jerry",
                SentBy = "1",
                SentTime = BaseTime,
                GroupId = "1"
            };

            MessageOfGroupTomAndJerry2 = new()
            {
                Id = "2",
                Content = "Hello Tom",
                SentBy = "2",
                SentTime = BaseTime,
                GroupId = "2"
            };

            MessageOfGroupTomAndJerryAndTyke1 = new()
            {
                Id = "3",
                Content = "Hello Jerry and Tyke",
                SentBy = "1",
                SentTime = BaseTime,
                GroupId = "2"
            };

            MessageOfGroupTomAndJerryAndTyke2 = new()
            {
                Id = "4",
                Content = "Hello Tom and Jerry",
                SentBy = "4",
                SentTime = BaseTime,
                GroupId = "2"
            };
            await Repository.AddAsync(MessageOfGroupTomAndJerry1);
            await Repository.AddAsync(MessageOfGroupTomAndJerry2);
            await Repository.AddAsync(MessageOfGroupTomAndJerryAndTyke1);
            await Repository.AddAsync(MessageOfGroupTomAndJerryAndTyke2);
        }

        private async Task AddGroups()
        {
            TomAndJerryGroup = new()
            {
                Id = "1",
                Description = "Friend group of Tom and Jerry"
            };
            TomInTomAndJerry = new()
            {
                Id = "1",
                UserId = "1",
                GroupId = "1"
            };
            JerryInTomAndJerry = new()
            {
                Id = "2",
                UserId = "2",
                GroupId = "1"
            };
            await Repository.AddAsync(TomAndJerryGroup);
            await Repository.AddAsync(TomInTomAndJerry);
            await Repository.AddAsync(JerryInTomAndJerry);

            TomAndJerryAndTykeGroup = new()
            {
                Id = "2",
                Description = "Multi group of Tom and Jerry and Tyke"
            };
            TomInTomAndJerryAndTyke = new()
            {
                Id = "3",
                UserId = "1",
                GroupId = "2"
            };
            JerryInTomAndJerryAndTyke = new()
            {
                Id = "4",
                UserId = "2",
                GroupId = "2"
            };
            TykeInTomAndJerryAndTyke = new()
            {
                Id = "5",
                UserId = "4",
                GroupId = "2"
            };
            await Repository.AddAsync(TomAndJerryAndTykeGroup);
            await Repository.AddAsync(TomInTomAndJerryAndTyke);
            await Repository.AddAsync(JerryInTomAndJerryAndTyke);
            await Repository.AddAsync(TykeInTomAndJerryAndTyke);
        }

        private async Task AddFriends()
        {
            TomAndJerryFriend = new()
            {
                Id = "1",
                UserId = "1",
                FriendId = "2"
            };
            SpikeAndTyke = new()
            {
                Id = "2",
                UserId = "3",
                FriendId = "4"
            };
            await Repository.AddAsync(TomAndJerryFriend);
            await Repository.AddAsync(SpikeAndTyke);
        }

        private async Task AddUsers()
        {
            Tom = new()
            {
                Id = "1",
                DisplayName = "Tom"
            };

            Jerry = new()
            {
                Id = "2",
                DisplayName = "Jerry"
            };
            Spike = new()
            {
                Id = "3",
                DisplayName = "Spike"
            };
            Tyke = new()
            {
                Id = "4",
                DisplayName = "Tyke"
            };
            await Repository.AddAsync(Tom);
            await Repository.AddAsync(Jerry);
            await Repository.AddAsync(Spike);
            await Repository.AddAsync(Tyke);
        }
    }
}
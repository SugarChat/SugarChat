using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using SugarChat.Core.Services.Messages;
using Xunit;
using Shouldly;
using SugarChat.Core.Services.GroupUsers;

namespace SugarChat.Database.MongoDb.IntegrationTest.DataProviders
{
    public class MessageDataProviderTests : ServiceFixture
    {
        private readonly IMessageDataProvider _messageDataProvider;
        private readonly IGroupUserDataProvider _groupUserDataProvider;

        public MessageDataProviderTests(DatabaseFixture dbFixture) : base(dbFixture)
        {
            _messageDataProvider = Container.Resolve<IMessageDataProvider>();
            _groupUserDataProvider = Container.Resolve<IGroupUserDataProvider>();
        }

        [Fact]
        public async Task Should_Get_Exist_Message_By_Id()
        {
            Core.Domain.Message message = await _messageDataProvider.GetByIdAsync(MessageOfGroupTomAndJerry1.Id);
            message.ShouldBeEquivalentTo(MessageOfGroupTomAndJerry1);
        }

        [Fact]
        public async Task Should_Not_Get_None_Exist_Message_By_Id()
        {
            Core.Domain.Message message = await _messageDataProvider.GetByIdAsync("0");
            message.ShouldBeNull();
        }

        [Fact]
        public async Task Should_Add_None_Exist_Message()
        {
            Core.Domain.Message messageToNobody = new()
            {
                Id = "0",
                Content = "Hello Nobody",
                SentBy = "1",
                SentTime = BaseTime,
                GroupId = "1"
            };
            await _messageDataProvider.AddAsync(messageToNobody);
            Core.Domain.Message message = await _messageDataProvider.GetByIdAsync("0");
            message.ShouldBeEquivalentTo(messageToNobody);
        }

        [Fact]
        public async Task Should_Not_Add_Exist_Message()
        {
            Core.Domain.Message messageToNobody = new()
            {
                Id = "1",
                Content = "Hello Nobody",
                SentBy = "1",
                SentTime = BaseTime,
                GroupId = "1"
            };
            await Assert.ThrowsAnyAsync<Exception>(async () => await _messageDataProvider.AddAsync(messageToNobody));
        }

        [Fact]
        public async Task Should_Update_Exist_Message()
        {
            MessageOfGroupTomAndJerry1.Content = "It's modified.";
            await _messageDataProvider.UpdateAsync(MessageOfGroupTomAndJerry1);
            Core.Domain.Message message = await _messageDataProvider.GetByIdAsync(MessageOfGroupTomAndJerry1.Id);

            message.ShouldBeEquivalentTo(MessageOfGroupTomAndJerry1);
        }

        [Fact(Skip = "The IRepo is fixing the bug")]
        public async Task Should_Not_Update_None_Exist_Message()
        {
            Core.Domain.Message fakeMessage = new()
            {
                Id = "0",
                Content = "It's not exist"
            };
            await Assert.ThrowsAnyAsync<Exception>(async () => await _messageDataProvider.UpdateAsync(fakeMessage));
        }

        [Fact]
        public async Task Should_Remove_Exist_Message()
        {
            await _messageDataProvider.RemoveAsync(MessageOfGroupTomAndJerry1);
            Core.Domain.Message message = await _messageDataProvider.GetByIdAsync(MessageOfGroupTomAndJerry1.Id);

            message.ShouldBeNull();
        }

        [Fact(Skip = "The IRepo is fixing the bug")]
        public async Task Should_Not_Remove_None_Exist_Message()
        {
            Core.Domain.Message message = new()
            {
                Id = "0"
            };
            await Assert.ThrowsAnyAsync<Exception>(async () => await _messageDataProvider.RemoveAsync(message));
        }

        [Fact]
        public async Task Should_Get_Unread_To_User_With_Friend()
        {
            IEnumerable<Core.Domain.Message> messages =
                await _messageDataProvider.GetUnreadToUserWithFriendAsync(Tom.Id, Jerry.Id);
            messages.Count().ShouldBe(2);
            messages.OrderByDescending(o => o.SentTime).First().ShouldBeEquivalentTo(MessageOfGroupTomAndJerry2);
            messages.OrderByDescending(o => o.SentTime).Last().ShouldBeEquivalentTo(MessageOfGroupTomAndJerry1);
        }

        [Fact]
        public async Task Should_Get_All_Unread_To_User()
        {
            IEnumerable<Core.Domain.Message> messages = await _messageDataProvider.GetAllUnreadToUserAsync(Tom.Id);
            messages.Count().ShouldBe(4);
            messages.Where(o => o.GroupId == TomAndJerryGroup.Id).OrderByDescending(o => o.SentTime).Last()
                .ShouldBeEquivalentTo(MessageOfGroupTomAndJerry1);
            messages.Where(o => o.GroupId == TomAndJerryAndTykeGroup.Id).OrderByDescending(o => o.SentTime).Last()
                .ShouldBeEquivalentTo(MessageOfGroupTomAndJerryAndTyke1);

            await _groupUserDataProvider.SetMessageReadAsync(Tom.Id, TomAndJerryGroup.Id,
                MessageOfGroupTomAndJerry1.SentTime);
            messages = await _messageDataProvider.GetAllUnreadToUserAsync(Tom.Id);
            messages.Count().ShouldBe(3);
            messages.Where(o => o.GroupId == TomAndJerryGroup.Id).OrderByDescending(o => o.SentTime).Last()
                .ShouldBeEquivalentTo(MessageOfGroupTomAndJerry2);
            messages.Where(o => o.GroupId == TomAndJerryAndTykeGroup.Id).OrderByDescending(o => o.SentTime).Last()
                .ShouldBeEquivalentTo(MessageOfGroupTomAndJerryAndTyke1);
        }

        [Fact]
        public async Task Should_Get_All_History_To_User_With_Friend()
        {
            IEnumerable<Core.Domain.Message> messages = await _messageDataProvider.GetAllHistoryToUserWithFriendAsync(Tom.Id, Jerry.Id);
            messages.Count().ShouldBe(2);
            messages.OrderByDescending(o => o.SentTime).Last()
                .ShouldBeEquivalentTo(MessageOfGroupTomAndJerry1);
            
            await _groupUserDataProvider.SetMessageReadAsync(Tom.Id, TomAndJerryGroup.Id,
                MessageOfGroupTomAndJerry1.SentTime);
            messages = await _messageDataProvider.GetAllHistoryToUserWithFriendAsync(Tom.Id, Jerry.Id);
            messages.Count().ShouldBe(2);
            messages.OrderByDescending(o => o.SentTime).Last()
                .ShouldBeEquivalentTo(MessageOfGroupTomAndJerry1);
        }
        
        [Fact]
        public async Task Should_Get_All_History_To_User()
        {
            IEnumerable<Core.Domain.Message> messages = await _messageDataProvider.GetAllHistoryToUserAsync(Tom.Id);
            messages.Count().ShouldBe(4);
            messages.Where(o => o.GroupId == TomAndJerryGroup.Id).OrderByDescending(o => o.SentTime).Last()
                .ShouldBeEquivalentTo(MessageOfGroupTomAndJerry1);
            messages.Where(o => o.GroupId == TomAndJerryAndTykeGroup.Id).OrderByDescending(o => o.SentTime).Last()
                .ShouldBeEquivalentTo(MessageOfGroupTomAndJerryAndTyke1);

            await _groupUserDataProvider.SetMessageReadAsync(Tom.Id, TomAndJerryGroup.Id,
                MessageOfGroupTomAndJerry1.SentTime);
            messages = await _messageDataProvider.GetAllHistoryToUserAsync(Tom.Id);
            messages.Count().ShouldBe(4);
            messages.Where(o => o.GroupId == TomAndJerryGroup.Id).OrderByDescending(o => o.SentTime).Last()
                .ShouldBeEquivalentTo(MessageOfGroupTomAndJerry1);
            messages.Where(o => o.GroupId == TomAndJerryAndTykeGroup.Id).OrderByDescending(o => o.SentTime).Last()
                .ShouldBeEquivalentTo(MessageOfGroupTomAndJerryAndTyke1);
        }
        
                
        [Fact]
        public async Task Should_Get_Unread_To_User_From_Group()
        {
            IEnumerable<Core.Domain.Message> messages = await _messageDataProvider.GetUnreadToUserFromGroupAsync(Tom.Id, TomAndJerryGroup.Id);
            messages.Count().ShouldBe(2);
            messages.OrderByDescending(o => o.SentTime).Last()
                .ShouldBeEquivalentTo(MessageOfGroupTomAndJerry1);

            await _groupUserDataProvider.SetMessageReadAsync(Tom.Id, TomAndJerryGroup.Id,
                MessageOfGroupTomAndJerry1.SentTime);
            messages = await _messageDataProvider.GetUnreadToUserFromGroupAsync(Tom.Id, TomAndJerryGroup.Id);
            messages.Count().ShouldBe(1);
            messages.OrderByDescending(o => o.SentTime).Last()
                .ShouldBeEquivalentTo(MessageOfGroupTomAndJerry2);
        }
        
        [Fact]
        public async Task Should_Get_All_To_User_From_Group()
        {
            IEnumerable<Core.Domain.Message> messages = await _messageDataProvider.GetAllToUserFromGroupAsync(Tom.Id, TomAndJerryGroup.Id);
            messages.Count().ShouldBe(2);
            messages.OrderByDescending(o => o.SentTime).Last()
                .ShouldBeEquivalentTo(MessageOfGroupTomAndJerry1);

            await _groupUserDataProvider.SetMessageReadAsync(Tom.Id, TomAndJerryGroup.Id,
                MessageOfGroupTomAndJerry1.SentTime);
            messages = await _messageDataProvider.GetAllToUserFromGroupAsync(Tom.Id, TomAndJerryGroup.Id);
            messages.Count().ShouldBe(2);
            messages.OrderByDescending(o => o.SentTime).Last()
                .ShouldBeEquivalentTo(MessageOfGroupTomAndJerry1);
        }
        
        [Fact]
        public async Task Should_Get_Messages_Of_Group_Before()
        {
            IEnumerable<Core.Domain.Message> messages = await _messageDataProvider.GetMessagesOfGroupBeforeAsync(MessageOfGroupTomAndJerry2.Id, 1);
            messages.Count().ShouldBe(1);
            messages.First().ShouldBeEquivalentTo(MessageOfGroupTomAndJerry1);
        }
        
        [Fact]
        public async Task Should_Get_Messages_Of_Group()
        {
            IEnumerable<Core.Domain.Message> messages = await _messageDataProvider.GetMessagesOfGroupAsync(TomAndJerryGroup.Id, 1);
            messages.Count().ShouldBe(1);
            messages.First().ShouldBeEquivalentTo(MessageOfGroupTomAndJerry2);
            
            messages = await _messageDataProvider.GetMessagesOfGroupAsync(TomAndJerryGroup.Id, 2);
            messages.Count().ShouldBe(2);
            messages.OrderByDescending(o => o.SentTime).Last()
                .ShouldBeEquivalentTo(MessageOfGroupTomAndJerry1);
            messages.OrderByDescending(o => o.SentTime).First()
                .ShouldBeEquivalentTo(MessageOfGroupTomAndJerry2);
        }
        
        [Fact]
        public async Task Should_Get_Latest_Message_Of_Group()
        {
            Core.Domain.Message message = await _messageDataProvider.GetLatestMessageOfGroupAsync(TomAndJerryGroup.Id);
            message.ShouldBeEquivalentTo(MessageOfGroupTomAndJerry2);
            
            await _groupUserDataProvider.SetMessageReadAsync(Tom.Id, TomAndJerryGroup.Id,
                MessageOfGroupTomAndJerry1.SentTime);
            message = await _messageDataProvider.GetLatestMessageOfGroupAsync(TomAndJerryGroup.Id);
            message.ShouldBeEquivalentTo(MessageOfGroupTomAndJerry2);
        }
    }
}
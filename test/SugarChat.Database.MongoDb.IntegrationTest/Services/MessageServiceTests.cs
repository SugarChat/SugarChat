using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.Services.Messages;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Event;
using SugarChat.Message.Events.Messages;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using Xunit;

namespace SugarChat.Database.MongoDb.IntegrationTest.Services
{
    public class MessageServiceTests : ServiceFixture
    {
        private readonly IMessageService _messageService;

        public MessageServiceTests(DatabaseFixture dbFixture) : base(dbFixture)
        {
            _messageService = Container.Resolve<IMessageService>();
        }

        [Fact]
        public async Task Should_Get_All_Unread_Messages_To_User()
        {
            GetAllUnreadToUserRequest getAllUnreadToUserRequest = new()
            {
                UserId = Tom.Id
            };
            GetAllUnreadToUserResponse getAllUnreadToUserResponse =
                await _messageService.GetAllUnreadToUserAsync(getAllUnreadToUserRequest);
            getAllUnreadToUserResponse.Messages.Count().ShouldBe(4);
            getAllUnreadToUserResponse.Messages.Count(o => o.GroupId == TomAndJerryGroup.Id).ShouldBe(2);

            TomInTomAndJerry.LastReadTime = BaseTime;
            await Repository.UpdateAsync(TomInTomAndJerry);
            getAllUnreadToUserResponse =
                await _messageService.GetAllUnreadToUserAsync(getAllUnreadToUserRequest);
            getAllUnreadToUserResponse.Messages.Count().ShouldBe(3);
            getAllUnreadToUserResponse.Messages.Count(o => o.GroupId == TomAndJerryGroup.Id).ShouldBe(1);
        }

        [Fact]
        public async Task Should_Not_Get_All_Unread_Messages_To_User_Who_Dose_Not_Exists()
        {
            GetAllUnreadToUserRequest getAllUnreadToUserRequest = new()
            {
                UserId = "0"
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.GetAllUnreadToUserAsync(getAllUnreadToUserRequest));
        }

        [Fact]
        public async Task Should_Get_Unread_Messages_To_User_From_Friend()
        {
            GetUnreadToUserFromFriendRequest getAllUnreadToUserRequest = new()
            {
                UserId = Tom.Id,
                FriendId = Jerry.Id
            };
            GetUnreadToUserFromFriendResponse getUnreadToUserFromFriendResponse =
                await _messageService.GetUnreadToUserFromFriendAsync(getAllUnreadToUserRequest);
            getUnreadToUserFromFriendResponse.Messages.Count().ShouldBe(2);
            getUnreadToUserFromFriendResponse.Messages.Count(o => o.GroupId == TomAndJerryGroup.Id).ShouldBe(2);

            TomInTomAndJerry.LastReadTime = BaseTime;
            await Repository.UpdateAsync(TomInTomAndJerry);
            getUnreadToUserFromFriendResponse =
                await _messageService.GetUnreadToUserFromFriendAsync(getAllUnreadToUserRequest);
            getUnreadToUserFromFriendResponse.Messages.Count().ShouldBe(1);
            getUnreadToUserFromFriendResponse.Messages.Count(o => o.GroupId == TomAndJerryGroup.Id).ShouldBe(1);
        }

        [Fact]
        public async Task Should_Not_Get_Unread_Messages_To_User_From_Friend_When_Either_One_Dose_Not_Exists()
        {
            GetUnreadToUserFromFriendRequest getAllUnreadToUserRequest = new()
            {
                UserId = "0",
                FriendId = Jerry.Id
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.GetUnreadToUserFromFriendAsync(getAllUnreadToUserRequest));

            getAllUnreadToUserRequest = new()
            {
                UserId = Tom.Id,
                FriendId = "0"
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.GetUnreadToUserFromFriendAsync(getAllUnreadToUserRequest));
        }

        [Fact]
        public async Task Should_Not_Get_Unread_Messages_To_User_From_Friend_When_They_Are_Not_Friend()
        {
            GetUnreadToUserFromFriendRequest getAllUnreadToUserRequest = new()
            {
                UserId = Tom.Id,
                FriendId = Tyke.Id
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.GetUnreadToUserFromFriendAsync(getAllUnreadToUserRequest));
        }

        [Fact]
        public async Task Should_Get_All_History_To_User_From_Friend()
        {
            GetAllHistoryToUserFromFriendRequest getAllHistoryToUserFromFriendRequest = new()
            {
                UserId = Tom.Id,
                FriendId = Jerry.Id
            };
            GetAllHistoryToUserFromFriendResponse getAllHistoryToUserFromFriendResponse =
                await _messageService.GetAllHistoryToUserFromFriendAsync(getAllHistoryToUserFromFriendRequest);
            getAllHistoryToUserFromFriendResponse.Messages.Count().ShouldBe(2);
            getAllHistoryToUserFromFriendResponse.Messages.Count(o => o.GroupId == TomAndJerryGroup.Id).ShouldBe(2);

            TomInTomAndJerry.LastReadTime = BaseTime;
            await Repository.UpdateAsync(TomInTomAndJerry);
            getAllHistoryToUserFromFriendResponse =
                await _messageService.GetAllHistoryToUserFromFriendAsync(getAllHistoryToUserFromFriendRequest);
            getAllHistoryToUserFromFriendResponse.Messages.Count().ShouldBe(2);
            getAllHistoryToUserFromFriendResponse.Messages.Count(o => o.GroupId == TomAndJerryGroup.Id).ShouldBe(2);
        }

        [Fact]
        public async Task Should_Not_Get_All_History_To_User_From_Friend_When_Either_One_Dose_Not_Exists()
        {
            GetAllHistoryToUserFromFriendRequest getAllHistoryToUserFromFriendRequest = new()
            {
                UserId = "0",
                FriendId = Jerry.Id
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.GetAllHistoryToUserFromFriendAsync(getAllHistoryToUserFromFriendRequest));

            getAllHistoryToUserFromFriendRequest = new()
            {
                UserId = Tom.Id,
                FriendId = "0"
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.GetAllHistoryToUserFromFriendAsync(getAllHistoryToUserFromFriendRequest));
        }

        [Fact]
        public async Task Should_Not_Get_All_History_To_User_From_Friend_When_They_Are_Not_Friend()
        {
            GetAllHistoryToUserFromFriendRequest getAllHistoryToUserFromFriendRequest = new()
            {
                UserId = Tom.Id,
                FriendId = Tyke.Id
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.GetAllHistoryToUserFromFriendAsync(getAllHistoryToUserFromFriendRequest));
        }

        [Fact]
        public async Task Should_Get_All_History_To_User()
        {
            GetAllHistoryToUserRequest getAllHistoryToUserRequest = new()
            {
                UserId = Tom.Id
            };
            GetAllHistoryToUserResponse getAllHistoryToUserResponse =
                await _messageService.GetAllHistoryToUserAsync(getAllHistoryToUserRequest);
            getAllHistoryToUserResponse.Messages.Count().ShouldBe(4);
            getAllHistoryToUserResponse.Messages.Count(o => o.GroupId == TomAndJerryGroup.Id).ShouldBe(2);
            getAllHistoryToUserResponse.Messages.Count(o => o.GroupId == TomAndJerryAndTykeGroup.Id).ShouldBe(2);

            TomInTomAndJerry.LastReadTime = BaseTime;
            await Repository.UpdateAsync(TomInTomAndJerry);
            getAllHistoryToUserResponse =
                await _messageService.GetAllHistoryToUserAsync(getAllHistoryToUserRequest);
            getAllHistoryToUserResponse.Messages.Count().ShouldBe(4);
            getAllHistoryToUserResponse.Messages.Count(o => o.GroupId == TomAndJerryGroup.Id).ShouldBe(2);
            getAllHistoryToUserResponse.Messages.Count(o => o.GroupId == TomAndJerryAndTykeGroup.Id).ShouldBe(2);
        }

        [Fact]
        public async Task Should_Not_Get_All_History_To_User_Who_Dose_Not_Exists()
        {
            GetAllHistoryToUserRequest getAllHistoryToUserRequest = new()
            {
                UserId = "0"
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.GetAllHistoryToUserAsync(getAllHistoryToUserRequest));
        }

        [Fact]
        public async Task Should_Get_Unread_To_User_From_Group()
        {
            GetUnreadToUserFromGroupRequest getUnreadToUserFromGroupRequest = new()
            {
                UserId = Tom.Id,
                GroupId = TomAndJerryGroup.Id
            };
            GetUnreadToUserFromGroupResponse getUnreadToUserFromGroupResponse =
                await _messageService.GetUnreadToUserFromGroupAsync(getUnreadToUserFromGroupRequest);
            getUnreadToUserFromGroupResponse.Messages.Count().ShouldBe(2);
            getUnreadToUserFromGroupResponse.Messages.Count(o => o.GroupId == TomAndJerryGroup.Id).ShouldBe(2);

            TomInTomAndJerry.LastReadTime = BaseTime;
            await Repository.UpdateAsync(TomInTomAndJerry);
            getUnreadToUserFromGroupResponse =
                await _messageService.GetUnreadToUserFromGroupAsync(getUnreadToUserFromGroupRequest);
            getUnreadToUserFromGroupResponse.Messages.Count().ShouldBe(1);
            getUnreadToUserFromGroupResponse.Messages.Count(o => o.GroupId == TomAndJerryGroup.Id).ShouldBe(1);
            getUnreadToUserFromGroupResponse.Messages.All(o => o.SentTime > TomInTomAndJerry.LastReadTime)
                .ShouldBeTrue();
        }

        [Fact]
        public async Task Should_Not_Get_Unread_To_User_From_Group_When_User_Dose_Not_Exist()
        {
            GetUnreadToUserFromGroupRequest getUnreadToUserFromGroupRequest = new()
            {
                UserId = "0",
                GroupId = TomAndJerryGroup.Id
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.GetUnreadToUserFromGroupAsync(getUnreadToUserFromGroupRequest));
        }

        [Fact]
        public async Task Should_Not_Get_Unread_To_User_From_Group_When_Group_Dose_Not_Exist()
        {
            GetUnreadToUserFromGroupRequest getUnreadToUserFromGroupRequest = new()
            {
                UserId = Tom.Id,
                GroupId = "0"
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.GetUnreadToUserFromGroupAsync(getUnreadToUserFromGroupRequest));
        }

        [Fact]
        public async Task Should_Not_Get_Unread_To_User_From_Group_When_User_Is_Not_In()
        {
            GetUnreadToUserFromGroupRequest getUnreadToUserFromGroupRequest = new()
            {
                UserId = Tyke.Id,
                GroupId = TomAndJerryGroup.Id
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.GetUnreadToUserFromGroupAsync(getUnreadToUserFromGroupRequest));
        }

        [Fact]
        public async Task Should_Get_All_To_User_From_Group()
        {
            GetAllToUserFromGroupRequest getAllToUserFromGroupRequest = new()
            {
                UserId = Tom.Id,
                GroupId = TomAndJerryGroup.Id
            };
            GetAllToUserFromGroupResponse getAllToUserFromGroupResponse =
                await _messageService.GetAllToUserFromGroupAsync(getAllToUserFromGroupRequest);
            getAllToUserFromGroupResponse.Messages.Count().ShouldBe(2);
            getAllToUserFromGroupResponse.Messages.Count(o => o.GroupId == TomAndJerryGroup.Id).ShouldBe(2);

            TomInTomAndJerry.LastReadTime = BaseTime;
            await Repository.UpdateAsync(TomInTomAndJerry);
            getAllToUserFromGroupResponse =
                await _messageService.GetAllToUserFromGroupAsync(getAllToUserFromGroupRequest);
            getAllToUserFromGroupResponse.Messages.Count().ShouldBe(2);
            getAllToUserFromGroupResponse.Messages.Count(o => o.GroupId == TomAndJerryGroup.Id).ShouldBe(2);
        }

        [Fact]
        public async Task Should_Not_Get_All_To_User_From_Group_When_User_Dose_Not_Exist()
        {
            GetAllToUserFromGroupRequest getAllToUserFromGroupRequest = new()
            {
                UserId = "0",
                GroupId = TomAndJerryGroup.Id
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.GetAllToUserFromGroupAsync(getAllToUserFromGroupRequest));
        }

        [Fact]
        public async Task Should_Not_Get_All_To_User_From_Group_When_Group_Dose_Not_Exist()
        {
            GetAllToUserFromGroupRequest getAllToUserFromGroupRequest = new()
            {
                UserId = Tom.Id,
                GroupId = "0"
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.GetAllToUserFromGroupAsync(getAllToUserFromGroupRequest));
        }

        [Fact]
        public async Task Should_Not_Get_All_To_User_From_Group_When_User_Is_Not_In()
        {
            GetAllToUserFromGroupRequest getAllToUserFromGroupRequest = new()
            {
                UserId = Tyke.Id,
                GroupId = TomAndJerryGroup.Id
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.GetAllToUserFromGroupAsync(getAllToUserFromGroupRequest));
        }

        [Fact]
        public async Task Should_Get_Messages_Of_Group()
        {
            GetMessagesOfGroupRequest getMessagesOfGroupRequest = new()
            {
                GroupId = TomAndJerryGroup.Id,
                Count = 1
            };
            GetMessagesOfGroupResponse getMessagesOfGroupResponse =
                await _messageService.GetMessagesOfGroupAsync(getMessagesOfGroupRequest);
            getMessagesOfGroupResponse.Messages.Count().ShouldBe(1);
            getMessagesOfGroupResponse.Messages.Count(o => o.GroupId == TomAndJerryGroup.Id).ShouldBe(1);
            getMessagesOfGroupResponse.Messages.Last().Content.ShouldBe(MessageOfGroupTomAndJerry2.Content);
            getMessagesOfGroupResponse.Messages.Last().SentTime.ShouldBe(MessageOfGroupTomAndJerry2.SentTime);

            getMessagesOfGroupRequest = new()
            {
                GroupId = TomAndJerryGroup.Id,
                Count = 2
            };
            getMessagesOfGroupResponse =
                await _messageService.GetMessagesOfGroupAsync(getMessagesOfGroupRequest);
            getMessagesOfGroupResponse.Messages.Count().ShouldBe(2);
            getMessagesOfGroupResponse.Messages.Count(o => o.GroupId == TomAndJerryGroup.Id).ShouldBe(2);
            getMessagesOfGroupResponse.Messages.Last().Content.ShouldBe(MessageOfGroupTomAndJerry1.Content);
            getMessagesOfGroupResponse.Messages.Last().SentTime.ShouldBe(MessageOfGroupTomAndJerry1.SentTime);
        }

        [Fact]
        public async Task Should_Not_Get_Messages_Of_Group_When_Group_Dose_Not_Exist()
        {
            GetMessagesOfGroupRequest getMessagesOfGroupRequest = new()
            {
                GroupId = "0",
                Count = 2
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.GetMessagesOfGroupAsync(getMessagesOfGroupRequest));
        }

        [Fact]
        public async Task Should_Get_Messages_Of_Group_Before()
        {
            GetMessagesOfGroupBeforeRequest getMessagesOfGroupBeforeRequest = new()
            {
                MessageId = MessageOfGroupTomAndJerry2.Id,
                Count = 1
            };
            GetMessagesOfGroupBeforeResponse getMessagesOfGroupBeforeResponse =
                await _messageService.GetMessagesOfGroupBeforeAsync(getMessagesOfGroupBeforeRequest);
            getMessagesOfGroupBeforeResponse.Messages.Count().ShouldBe(1);
            getMessagesOfGroupBeforeResponse.Messages.Count(o => o.GroupId == TomAndJerryGroup.Id).ShouldBe(1);
            getMessagesOfGroupBeforeResponse.Messages.Last().Content.ShouldBe(MessageOfGroupTomAndJerry1.Content);
            getMessagesOfGroupBeforeResponse.Messages.Last().SentTime.ShouldBe(MessageOfGroupTomAndJerry1.SentTime);

            getMessagesOfGroupBeforeRequest = new()
            {
                MessageId = MessageOfGroupTomAndJerry2.Id,
                Count = 2
            };
            getMessagesOfGroupBeforeResponse =
                await _messageService.GetMessagesOfGroupBeforeAsync(getMessagesOfGroupBeforeRequest);
            getMessagesOfGroupBeforeResponse.Messages.Count().ShouldBe(1);
            getMessagesOfGroupBeforeResponse.Messages.Count(o => o.GroupId == TomAndJerryGroup.Id).ShouldBe(1);
            getMessagesOfGroupBeforeResponse.Messages.Last().Content.ShouldBe(MessageOfGroupTomAndJerry1.Content);
            getMessagesOfGroupBeforeResponse.Messages.Last().SentTime.ShouldBe(MessageOfGroupTomAndJerry1.SentTime);
        }

        [Fact]
        public async Task Should_Not_Get_Messages_Of_Group_Before_When_Message_Dose_Not_Exist()
        {
            GetMessagesOfGroupBeforeRequest getMessagesOfGroupBeforeRequest = new()
            {
                MessageId = "0",
                Count = 1
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.GetMessagesOfGroupBeforeAsync(getMessagesOfGroupBeforeRequest));
        }

        [Fact]
        public async Task Should_Set_Message_Read_By_User_Based_On_Message_Id()
        {
            SetMessageReadByUserBasedOnMessageIdCommand setMessageReadByUserBasedOnMessageIdCommand = new()
            {
                UserId = Tom.Id,
                MessageId = MessageOfGroupTomAndJerry1.Id
            };
            SetMessageReadByUserBasedOnMessageIdEvent getMessageReadByUserBasedOnMessageIdEvent =
                await _messageService.SetMessageReadByUserBasedOnMessageIdAsync(
                    setMessageReadByUserBasedOnMessageIdCommand);
            getMessageReadByUserBasedOnMessageIdEvent.Status.ShouldBe(EventStatus.Success);

            Core.Domain.Message message =
                await Repository.SingleAsync<Core.Domain.Message>(o =>
                    o.Id == setMessageReadByUserBasedOnMessageIdCommand.MessageId);
            GroupUser groupUser = await Repository.SingleAsync<GroupUser>(o =>
                o.UserId == setMessageReadByUserBasedOnMessageIdCommand.UserId && o.GroupId == message.GroupId);
            groupUser.LastReadTime.ShouldBe(MessageOfGroupTomAndJerry1.SentTime);
        }

        [Fact]
        public async Task Should_Not_Set_Message_Read_By_User_Based_On_Message_Id_When_User_Dose_Not_Exist()
        {
            SetMessageReadByUserBasedOnMessageIdCommand setMessageReadByUserBasedOnMessageIdCommand = new()
            {
                UserId = "0",
                MessageId = MessageOfGroupTomAndJerry1.Id
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.SetMessageReadByUserBasedOnMessageIdAsync(
                    setMessageReadByUserBasedOnMessageIdCommand));
        }

        [Fact]
        public async Task Should_Not_Set_Message_Read_By_User_Based_On_Message_Id_When_Message_Dose_Not_Exist()
        {
            SetMessageReadByUserBasedOnMessageIdCommand setMessageReadByUserBasedOnMessageIdCommand = new()
            {
                UserId = Tom.Id,
                MessageId = "0"
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.SetMessageReadByUserBasedOnMessageIdAsync(
                    setMessageReadByUserBasedOnMessageIdCommand));
        }

        [Fact]
        public async Task Should_Not_Set_Message_Read_By_User_Based_On_Message_Id_When_User_Dose_Not_In_The_Group()
        {
            SetMessageReadByUserBasedOnMessageIdCommand setMessageReadByUserBasedOnMessageIdCommand = new()
            {
                UserId = Tyke.Id,
                MessageId = MessageOfGroupTomAndJerry1.Id
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.SetMessageReadByUserBasedOnMessageIdAsync(
                    setMessageReadByUserBasedOnMessageIdCommand));
        }

        [Fact]
        public async Task
            Should_Not_Set_Message_Read_By_User_Based_On_Message_Id_When_Last_Read_Time_Later_Or_Equal_To_The_Message()
        {
            SetMessageReadByUserBasedOnMessageIdCommand setMessageReadByUserBasedOnMessageIdCommand = new()
            {
                UserId = Tom.Id,
                MessageId = MessageOfGroupTomAndJerry1.Id
            };
            TomInTomAndJerry.LastReadTime = MessageOfGroupTomAndJerry1.SentTime;
            await Repository.UpdateAsync(TomInTomAndJerry);
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.SetMessageReadByUserBasedOnMessageIdAsync(
                    setMessageReadByUserBasedOnMessageIdCommand));
        }

        [Fact]
        public async Task Should_Set_Message_Read_By_User_Based_On_Group_Id()
        {
            SetMessageReadByUserBasedOnGroupIdCommand setMessageReadByUserBasedOnGroupIdCommand = new()
            {
                UserId = Tom.Id,
                GroupId = TomAndJerryGroup.Id
            };
            SetMessageReadByUserBasedOnGroupIdEvent getMessageReadByUserBasedOnGroupIdEvent =
                await _messageService.SetMessageReadByUserBasedOnGroupIdAsync(
                    setMessageReadByUserBasedOnGroupIdCommand);
            getMessageReadByUserBasedOnGroupIdEvent.Status.ShouldBe(EventStatus.Success);

            GroupUser groupUser = await Repository.SingleAsync<GroupUser>(o =>
                o.UserId == setMessageReadByUserBasedOnGroupIdCommand.UserId &&
                o.GroupId == setMessageReadByUserBasedOnGroupIdCommand.GroupId);
            groupUser.LastReadTime.ShouldBe(MessageOfGroupTomAndJerry2.SentTime);
        }

        [Fact]
        public async Task Should_Not_Set_Message_Read_By_User_Based_On_Group_Id_When_User_Dose_Not_Exist()
        {
            SetMessageReadByUserBasedOnGroupIdCommand setMessageReadByUserBasedOnGroupIdCommand = new()
            {
                UserId = "0",
                GroupId = TomAndJerryGroup.Id
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.SetMessageReadByUserBasedOnGroupIdAsync(
                    setMessageReadByUserBasedOnGroupIdCommand));
        }

        [Fact]
        public async Task Should_Not_Set_Message_Read_By_User_Based_On_Group_Id_When_Message_Dose_Not_Exist()
        {
            SetMessageReadByUserBasedOnGroupIdCommand setMessageReadByUserBasedOnGroupIdCommand = new()
            {
                UserId = Tom.Id,
                GroupId = "0"
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.SetMessageReadByUserBasedOnGroupIdAsync(
                    setMessageReadByUserBasedOnGroupIdCommand));
        }

        [Fact]
        public async Task Should_Not_Set_Message_Read_By_User_Based_On_Group_Id_When_User_Dose_Not_In_The_Group()
        {
            SetMessageReadByUserBasedOnGroupIdCommand setMessageReadByUserBasedOnGroupIdCommand = new()
            {
                UserId = Tyke.Id,
                GroupId = TomAndJerryGroup.Id
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.SetMessageReadByUserBasedOnGroupIdAsync(
                    setMessageReadByUserBasedOnGroupIdCommand));
        }

        [Fact]
        public async Task
            Should_Not_Set_Message_Read_By_User_Based_On_Group_Id_When_Last_Read_Time_Later_Or_Equal_To_The_Message()
        {
            SetMessageReadByUserBasedOnGroupIdCommand setMessageReadByUserBasedOnGroupIdCommand = new()
            {
                UserId = Tom.Id,
                GroupId = TomAndJerryGroup.Id
            };
            TomInTomAndJerry.LastReadTime = MessageOfGroupTomAndJerry2.SentTime;
            await Repository.UpdateAsync(TomInTomAndJerry);
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _messageService.SetMessageReadByUserBasedOnGroupIdAsync(
                    setMessageReadByUserBasedOnGroupIdCommand));
        }
    }
}
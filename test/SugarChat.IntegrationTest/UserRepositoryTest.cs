using Microsoft.Extensions.Configuration;
using Shouldly;
using SugarChat.Core.Common;
using SugarChat.Core.Settings;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.IntegrationTest
{
    //todo fix it
    public class UserRepositoryTest : TestBase
    {
        //readonly IUserRepository userRepository;
        //public UserRepositoryTest()
        //{
        //    MongoDbSettings settings = new MongoDbSettings();
        //    _configuration.GetSection("MongoDb").Bind(settings);
        //    userRepository = new UserRepository(new SugarChatDbContext(settings));
        //}

        //[Fact]
        //public Task Should_User_Repo_Not_Null()
        //{
        //    userRepository.ShouldNotBeNull();
        //    return Task.CompletedTask;
        //}

        //[Fact]
        //public async Task Should_Add_One_User()
        //{
        //    var id = Guid.NewGuid();
        //    var newUser = new User1(id, "https://sugarchat.com", new RegisterInfo(), "sugarchat_publicid", "叶子", "叶子的描述", UserStatus.Hidden, "cooper", "huang", new DateTime(1994, 11, 17));
        //    await userRepository.AddAsync(newUser);
        //    var insertedUser = await userRepository.FindAsync(id);
        //    insertedUser.ShouldNotBeNull();
        //}

        //[Fact]
        //public async Task Should_Delete_One_User()
        //{
        //    var id = Guid.NewGuid();
        //    var newUser = new User1(id, "https://sugarchat.com", new RegisterInfo(), "sugarchat_publicid", "叶子", "叶子的描述", UserStatus.Hidden, "cooper", "huang", new DateTime(1994, 11, 17));
        //    await userRepository.AddAsync(newUser);
        //    var insertedUser = await userRepository.FindAsync(id);
        //    insertedUser.ShouldNotBeNull();
        //    await userRepository.DeleteAsync(insertedUser.Id);
        //    var deletedUser = await userRepository.FindAsync(id);
        //    deletedUser.ShouldBeNull();
        //}

        //[Fact]
        //public async Task Should_Update_One_User()
        //{
        //    var id = Guid.NewGuid();
        //    var newUser = new User1(id,
        //        "https://sugarchat.com",
        //        new RegisterInfo(),
        //        "sugarchat_publicid",
        //        "叶子",
        //        "叶子的描述",
        //        UserStatus.Hidden,
        //        "cooper",
        //        "huang",
        //        new DateTime(1994, 11, 17));
        //    await userRepository.AddAsync(newUser);
        //    var insertedUser = await userRepository.FindAsync(id);
        //    insertedUser.ShouldNotBeNull();
        //    string newNickName = "有一片叶子";
        //    DateTime newBirthday = new DateTime(1994, 12, 19);
        //    var newUpdateUser = new User1(insertedUser.Id,
        //        insertedUser.AvatarUrl,
        //        insertedUser.RegisterInfo,
        //        insertedUser.PublicId,
        //        newNickName,
        //        insertedUser.SelfDescription,
        //        insertedUser.Status,
        //        insertedUser.Forename,
        //        insertedUser.Surname,
        //        newBirthday
        //        );
        //    await userRepository.UpdateAsync(newUpdateUser, entity => new
        //    {
        //        entity.NickName,
        //        entity.Birthday
        //    });
        //    var updatedUser = await userRepository.FindAsync(id);
        //    updatedUser.NickName.ShouldBe(newNickName);
        //    Assert.Equal(updatedUser.Birthday.ToString("yyyyMMdd HH:mm:ss"), newBirthday.ToString("yyyyMMdd HH:mm:ss"));
        //    updatedUser.AvatarUrl.ShouldBe(insertedUser.AvatarUrl);
        //    updatedUser.RegisterInfo.CloseDateTime.ShouldBe(insertedUser.RegisterInfo.CloseDateTime);
        //    updatedUser.RegisterInfo.RegisterDateTime.ShouldBe(insertedUser.RegisterInfo.RegisterDateTime);
        //    updatedUser.RegisterInfo.IsActive.ShouldBe(insertedUser.RegisterInfo.IsActive);
        //    updatedUser.PublicId.ShouldBe(insertedUser.PublicId);
        //    updatedUser.SelfDescription.ShouldBe(insertedUser.SelfDescription);
        //    updatedUser.Status.ShouldBe(insertedUser.Status);
        //    updatedUser.Forename.ShouldBe(insertedUser.Forename);
        //    updatedUser.Surname.ShouldBe(insertedUser.Surname);
        //}

    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.Services.Emotions;
using SugarChat.Message.Commands.Emotions;
using SugarChat.Message.Commands.Friends;
using SugarChat.Message.Dtos.Emotions;
using SugarChat.Message.Event;
using SugarChat.Message.Events.Users;
using SugarChat.Message.Exceptions;
using SugarChat.Message.Requests.Emotions;
using Xunit;

namespace SugarChat.Database.MongoDb.IntegrationTest.Services
{
    public class EmotionServiceTests : ServiceFixture
    {
        private readonly IEmotionService _emotionService;

        public EmotionServiceTests(DatabaseFixture dbFixture) : base(dbFixture)
        {
            _emotionService = Container.Resolve<IEmotionService>();
        }
       
        
        [Fact]
        public async Task Should_Get_Emotions_By_Ids()
        {
            var emotions =
                (await _emotionService.GetEmotionByIdsAsync(new GetEmotionByIdsRequest(){Ids = new []{EmotionOfJerry.Id, EmotionOfTom1.Id, "5"}})).ToList();
            emotions.Count.ShouldBe(2);
            emotions.Single(o=>o.Id == EmotionOfJerry.Id).Name.ShouldBe("Jerry");
        }
        
        [Fact]
        public async Task Should_Get_User_Emotions()
        {
            var emotions =
                (await _emotionService.GetUserEmotionsAsync(new GetUserEmotionsRequest() { UserId = Tom.Id })).ToList();
            emotions.Count.ShouldBe(2);
            emotions.Single(o=>o.Id == "1").Name.ShouldBe("Tom1");
        }
        
        [Fact]
        public async Task Should_Add_Emotions()
        {
            await _emotionService.AddEmotionAsync(new AddEmotionCommand(){Id = "4", UserId = Tom.Id, Url = "www.tom.com/tom3", Name = "Tom3"});
            
            var emotions =
                (await _emotionService.GetEmotionByIdsAsync(new GetEmotionByIdsRequest(){Ids = new []{"4"}})).ToList();

            emotions.Count.ShouldBe(1);
            emotions.Single(o=>o.Id == "4").Name.ShouldBe("Tom3");
        }
        
        [Fact]
        public async Task Should_Remove_Existed_Emotion()
        {
            await _emotionService.RemoveEmotionAsync(new RemoveEmotionCommand(){Id = "1", UserId = Tom.Id});
            
            var emotions =
                (await _emotionService.GetEmotionByIdsAsync(new GetEmotionByIdsRequest(){Ids = new []{"1"}})).ToList();

            emotions.Count.ShouldBe(0);
            emotions.SingleOrDefault(o=>o.Id == "4").ShouldBeNull();
        }
        
        [Fact]
        public async Task Should_Not_Remove_None_Existed_Emotion()
        {
            await _emotionService.RemoveEmotionAsync(new RemoveEmotionCommand(){Id = "5", UserId = Tom.Id}).ShouldThrowAsync<BusinessException>();
        }
        
        [Fact]
        public async Task Should_Not_Remove_Emotion_Not_Belong_To_Self()
        {
            await _emotionService.RemoveEmotionAsync(new RemoveEmotionCommand(){Id = "1", UserId = Jerry.Id}).ShouldThrowAsync<BusinessException>();
        }
    }
}
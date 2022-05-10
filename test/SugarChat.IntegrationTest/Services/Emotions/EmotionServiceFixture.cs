using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mediator.Net;
using Shouldly;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.Emotions;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Dtos;
using SugarChat.Message.Dtos.Emotions;
using SugarChat.Message.Requests;
using SugarChat.Message.Requests.Emotions;
using Xunit;

namespace SugarChat.IntegrationTest.Services.Emotions
{
    public class EmotionServiceFixture : TestFixtureBase
    {
        [Fact]
        public async Task ShouldParseEmotionFormatAndGetUrl()
        {
            string emotionSeeId = Guid.NewGuid().ToString();
            string emotionSmileId = Guid.NewGuid().ToString();

            string content = $"Nice to [emotion:{emotionSeeId}] you guys.[emotion:{emotionSmileId}]";


            await Run<IMediator>(async mediator =>
            {
                var command = new AddEmotionCommand()
                {
                    Id = emotionSeeId,
                    UserId = users[7].Id,
                    Name = "see",
                    Url = "http://www.emotion.org/see"
                };
                await mediator.SendAsync(command);
            });

            await Run<IMediator>(async mediator =>
            {
                var command = new AddEmotionCommand()
                {
                    Id = emotionSmileId,
                    UserId = users[7].Id,
                    Name = "smile",
                    Url = "http://www.emotion.org/smile"
                };
                await mediator.SendAsync(command);
            });

            await Run<IMediator>(async (mediator) =>
            {
                SendMessageCommand command = new SendMessageCommand
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupId = groups[0].Id,
                    Content = content,
                    Type = 0,
                    SentBy = users[7].Id,
                    CreatedBy = users[7].Id
                };
                await mediator.SendAsync(command);
                var request = new GetUnreadMessagesFromGroupRequest()
                {
                    UserId = users[8].Id,
                    GroupId = groups[0].Id
                };
                var lastMessage =
                    (await mediator
                        .RequestAsync<GetUnreadMessagesFromGroupRequest,
                            SugarChatResponse<IEnumerable<MessageDto>>>(request)).Data.LastOrDefault();
                lastMessage.ShouldNotBeNull();
                lastMessage.Content.ShouldBe(content);

                #region simulate front-end parse emotion format

                Regex ex = new Regex(@"(?<=\[emotion:)(.+?)(?=\])");
                var emotionIds = ex.Matches(lastMessage.Content).Select(o => o.ToString()).ToList();
                emotionIds.Count.ShouldBe(2);
                emotionIds.First().ShouldBe(emotionSeeId);
                emotionIds.Last().ShouldBe(emotionSmileId);

                #endregion

                var getEmotionsRequest = new GetEmotionByIdsRequest()
                {
                    Ids = emotionIds,
                };
                var response =
                    await mediator.RequestAsync<GetEmotionByIdsRequest, SugarChatResponse<IEnumerable<EmotionDto>>>(
                        getEmotionsRequest);
                var emotionSee = response.Data.SingleOrDefault(o => o.Id == emotionSeeId);
                emotionSee.ShouldNotBeNull();
                emotionSee.Url.ShouldBe("http://www.emotion.org/see");
                var emotionSmile = response.Data.SingleOrDefault(o => o.Id == emotionSmileId);
                emotionSmile.ShouldNotBeNull();
                emotionSmile.Url.ShouldBe("http://www.emotion.org/smile");
            });
        }
    }
}
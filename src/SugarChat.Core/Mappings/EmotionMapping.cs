using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Message.Commands.Emotions;
using SugarChat.Message.Dtos.Emotions;

namespace SugarChat.Core.Mappings
{
    public class EmotionMapping: Profile
    {
        public EmotionMapping()
        {
            CreateMap<AddEmotionCommand, Emotion>();
            CreateMap<Emotion, EmotionDto>();
        }
    }
}
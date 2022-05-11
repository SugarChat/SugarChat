using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Core.Services.Configurations;
using SugarChat.Message.Commands.Emotions;
using SugarChat.Message.Dtos.Configurations;
using SugarChat.Message.Dtos.Emotions;

namespace SugarChat.Core.Mappings
{
    public class ConfigurationMapping: Profile
    {
        public ConfigurationMapping()
        {
            CreateMap<ServerConfigurations, ServerConfigurationsDto>();
        }
    }
}
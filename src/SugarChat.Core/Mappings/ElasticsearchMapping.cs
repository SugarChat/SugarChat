using AutoMapper;
using SugarChat.Message.Commands.Elasticsearchs;
using SugarChat.Message.Events.Elasticsearchs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Mappings
{
    public class ElasticsearchMapping : Profile
    {
        public ElasticsearchMapping()
        {
            CreateMap<SyncMessageToElasticsearchCommand, SyncMessageToElasticsearchEvent>();
        }
    }
}

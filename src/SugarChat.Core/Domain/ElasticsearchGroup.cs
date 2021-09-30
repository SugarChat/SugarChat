using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Domain
{
    [ElasticsearchType(IdProperty = nameof(Id))]
    public class ElasticsearchGroup
    {
        [Keyword(Name = "id")]
        public string Id { get; set; }

        [Text(Name = "name")]
        public string Name { get; set; }

        [Text(Name = "description")]
        public string Description { get; set; }

        [PropertyName("custom_properties")]
        public Dictionary<string, string> CustomProperties { get; set; }
    }
}

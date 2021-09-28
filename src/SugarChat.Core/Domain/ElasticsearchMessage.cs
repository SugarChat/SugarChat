using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Domain
{
    [ElasticsearchType(IdProperty = nameof(Id))]
    public class ElasticsearchMessage
    {
        [Keyword(Name = "id")]
        public string Id { get; set; }

        [Keyword(Name = "group_id")]
        public string GroupId { get; set; }

        [Text(Name = "content")]
        public string Content { get; set; }

        [Keyword(Name = "sent_by")]
        public string SentBy { get; set; }

        [Date(Name = "sent_time")]
        public DateTimeOffset SentTime { get; set; }

        [PropertyName("custom_properties")]
        public Dictionary<string, string> CustomProperties { get; set; }
    }
}

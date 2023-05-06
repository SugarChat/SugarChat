using System;
using System.Collections.Generic;

namespace SugarChat.Core.Domain
{
    public interface IEntity
    {
        public string Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string LastModifyBy { get; set; }
        public Dictionary<string, string> CustomProperties { get; set; }
        public DateTimeOffset LastModifyDate { get; set; }
    }
};
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Dtos
{
    public class MessageTranslateDto
    {
        public string MessageId { get; set; }

        public string Content { get; set; }

        public string LanguageCode { get; set; }
    }
}

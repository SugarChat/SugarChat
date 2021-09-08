using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Setting
{
    public class GoogleTranslateApiKeySetting : ConfigurationSetting<string>
    {
        private readonly IConfiguration _configuration;

        public GoogleTranslateApiKeySetting(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override string Value => _configuration.GetValue<string>("GoogleTranslateApiKey");
    }
}

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Settings
{
    public class ElasticsearchIsEnableSetting : ConfigurationSetting<bool>
    {
        private readonly IConfiguration _configuration;

        public ElasticsearchIsEnableSetting(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override bool Value => _configuration.GetValue<bool>("Elasticsearch:IsEnable");
    }
}

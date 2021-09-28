using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Settings
{
    public class ConfigurationSetting<T> : IConfigurationSetting
    {
        public ConfigurationSetting() { }

        public virtual T Value { get; set; }
    }
}

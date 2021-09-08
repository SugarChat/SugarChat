using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Setting
{
    public class ConfigurationSetting<T> : IConfigurationSetting
    {
        public virtual T Value { get; set; }
    }
}

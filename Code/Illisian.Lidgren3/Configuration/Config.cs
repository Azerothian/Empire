using Illisian.UnityUtil.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Illisian.Lidgren3.Configuration
{
    public class Config : ConfigurationSection
    {
        const string _configNetworkName = "Network";
        const string _configLogLevelName = "LogLevel";


        [ConfigurationProperty(_configLogLevelName, DefaultValue = LogType.Information, IsRequired = true)]
        public LogType LogLevel
        {
            get
            {
                return (LogType)this[_configLogLevelName];
            }
            set
            {
                this[_configLogLevelName] = value;
            }
        }
        [ConfigurationProperty(_configNetworkName)]
        public ConfigNetworkElement Communication
        {
            get
            {
                return (ConfigNetworkElement)this[_configNetworkName];
            }
            set
            {
                this[_configNetworkName] = value;
            }
        }
    }
}

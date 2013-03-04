using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;

namespace Illisian.Lidgren3.Configuration
{
    public class ConfigNetworkElement : ConfigurationElement
    {
        const string _configPortName = "Port";
        const string _configEnableUPNPName = "EnableUPNP";
        const string _configExternalIpName = "ExternalIp";
        const string _configIpName = "BindingIp";

        [ConfigurationProperty(_configIpName, IsRequired = false)]
        public string BindingIp
        {
            get
            {
                string ipcheck = (string)this[_configIpName];
                IPAddress ipaddr = null;
                if (!IPAddress.TryParse(ipcheck, out ipaddr))
                {
                    IPHostEntry hostentry = Dns.GetHostEntry(ipcheck);
                    return hostentry.AddressList.First().ToString();
                }
                else
                {
                    return ipcheck;
                }
            }
            set
            {
                this[_configIpName] = value;
            }
        }
        [ConfigurationProperty(_configExternalIpName, IsRequired = false)]
        public string ExternalIp
        {
            get
            {
                string ipcheck = (string)this[_configExternalIpName];
                IPAddress ipaddr = null;
                if (!IPAddress.TryParse(ipcheck, out ipaddr))
                {
                    IPHostEntry hostentry = Dns.GetHostEntry(ipcheck);
                    return hostentry.AddressList.First().ToString();
                }
                else
                {
                    return ipcheck;
                }
            }
            set
            {
                this[_configExternalIpName] = value;
            }
        }
        [ConfigurationProperty(_configPortName, IsRequired = false)]
        public int Port
        {
            get
            {
                return (int)this[_configPortName];
            }
            set
            {
                this[_configPortName] = value;
            }
        }
        [ConfigurationProperty(_configEnableUPNPName, DefaultValue = false, IsRequired = false)]
        public bool EnableUPNP
        {
            get
            {
                return (bool)this[_configEnableUPNPName];
            }
            set
            {
                this[_configEnableUPNPName] = value;
            }
        }
        public override string ToString()
        {
            return String.Format("BindingIp: {0} - Port: {1} - EnableUPNP: {2}", BindingIp, Port, EnableUPNP);
        }
    }
}

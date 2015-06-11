using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace mGet.Lib
{
    public class NIC
    {
        public static List<NetworkInterface> GetNetworkInterfaces()
        {
            var result = new List<NetworkInterface>();

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    result.Add(ni);
                }
            }

            return result;
        }

        public static List<NetworkInterface> GetNetworkInterfaces(Func<NetworkInterface,bool> predicate)
        {
            var result = new List<NetworkInterface>();

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    if (predicate(ni))
                    {
                        result.Add(ni);    
                    }
                }
            }

            return result;
        }

        public static List<UnicastIPAddressInformation> GetIPProperties(NetworkInterface ni)
        {
            var result = new List<UnicastIPAddressInformation>();

            foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    result.Add(ip);
                }
            }

            return result;
        }

        public static string GetIP(NetworkInterface ni)
        {
            var result = GetIPProperties(ni);

            return result[0].Address.ToString();
        }
    }
}

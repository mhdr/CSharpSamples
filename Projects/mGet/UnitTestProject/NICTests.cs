using System;
using System.Diagnostics;
using mGet.Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace UnitTestProject
{
    [TestClass]
    public class NICTests
    {
        [TestMethod]
        public void t0004_GetNetworkInterfaces()
        {
            var result = NIC.GetNetworkInterfaces();

            var output = JsonConvert.SerializeObject(result, Formatting.Indented);
            Debug.WriteLine(output);
        }

        [TestMethod]
        public void t0005_GetNetworkInterfaces()
        {
            var result = NIC.GetNetworkInterfaces(x=>x.Name.ToLower().Contains("ethernet"));

            var output = JsonConvert.SerializeObject(result, Formatting.Indented);
            Debug.WriteLine(output);
        }

        [TestMethod]
        public void t0006_GetNetworkInterfaces()
        {
            var result = NIC.GetNetworkInterfaces(x => x.Name.Equals("Wi-Fi"));

            var output = JsonConvert.SerializeObject(result, Formatting.Indented);
            Debug.WriteLine(output);
        }

        [TestMethod]
        public void t0007_GetIPProperties()
        {
            var nic = NIC.GetNetworkInterfaces(x => x.Name.ToLower().Contains("ethernet"));

            var result = NIC.GetIPProperties(nic[0]);

            Debug.WriteLine(result[0].Address);
        }

        [TestMethod]
        public void t0008_GetIP()
        {
            var nic = NIC.GetNetworkInterfaces(x => x.Name.ToLower().Contains("ethernet"));

            var result = NIC.GetIP(nic[0]);

            Debug.WriteLine(result);
        }
    }
}

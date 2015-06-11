using System;
using System.Diagnostics;
using mGet.Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace UnitTestProject
{
    [TestClass]
    public class HttpClientTests
    {
        [TestMethod]
        public void t0001_ExtractInfoFromUrl()
        {
            HttpClient httpClient = new HttpClient("http://80.84.55.200/proxy/OmegaRules_Auto_Switch.sorl");

            var output = JsonConvert.SerializeObject(httpClient, Formatting.Indented);
            Debug.WriteLine(output);
        }

        [TestMethod]
        public void t0002_GetFileSize()
        {
            HttpClient httpClient = new HttpClient("http://80.84.55.200/proxy/OmegaRules_Auto_Switch.sorl");
            long fileSize = httpClient.GetFileSize();

            Debug.WriteLine(fileSize);
        }

        [TestMethod]
        public void t0003_GetFileSize()
        {
            HttpClient httpClient = new HttpClient("http://80.84.55.200/proxy/");
            long fileSize = httpClient.GetFileSize();

            Debug.WriteLine(fileSize);
        }
    }
}

using System;
using System.Diagnostics;
using mGetLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace UnitTestProject
{
    [TestClass]
    public class HttpResponseTests
    {
        [TestMethod]
        public void t001_ExtractContentRange()
        {
            var result = HttpResponse.ExtractContentRange("Content-Range: bytes 0-595882/1191765");
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            Debug.Write(json);
        }
    }
}

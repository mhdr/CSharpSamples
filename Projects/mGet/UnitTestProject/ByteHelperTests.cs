using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Shared;

namespace UnitTestProject
{
    [TestClass]
    public class ByteHelperTests
    {
        [TestMethod]
        public void t001_SplitFileSize()
        {
            var result = ByteHelper.SplitFileSize(1000, 4);

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            Debug.WriteLine(json);
        }

        [TestMethod]
        public void t002_SplitFileSize()
        {
            var result = ByteHelper.SplitFileSize(1003, 4);

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            Debug.WriteLine(json);
        }

        [TestMethod]
        public void t003_SplitFileSize()
        {
            var result = ByteHelper.SplitFileSize(1157, 8);

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            Debug.WriteLine(json);
        }
    }
}

using System;
using System.Diagnostics;
using System.Threading;
using mGetLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class HttpDownloadManagerTests
    {
        [TestMethod]
        public void t001_StartDownloadAsync()
        {
            HttpDownloadManager downloadManager = new HttpDownloadManager("http://78.46.180.117/shadowsocks-local-linux64-1.1.4.gz");
            downloadManager.StartDownload(2);

            Console.ReadKey();
        }

        [TestMethod]
        public void t002_StartDownloadAsync()
        {
            HttpDownloadManager downloadManager = new HttpDownloadManager("http://78.46.180.117/shadowsocks-local-linux64-1.1.4.gz");
            downloadManager.StartDownload(1);

            Console.ReadKey();
        }

        [TestMethod]
        public void t003_GetFileSize()
        {
            HttpDownloadManager downloadManager = new HttpDownloadManager("http://filehippo.com/download/file/257fb4a25dbb3d60158ff79e2f8f2eeebd9542447fd874a7a89f8556612c3b9e/");
            downloadManager.GetFileSize();
        }

        [TestMethod]
        public void t004_GetLocationFromRedirect()
        {
            HttpDownloadManager downloadManager = new HttpDownloadManager("http://filehippo.com/download/file/257fb4a25dbb3d60158ff79e2f8f2eeebd9542447fd874a7a89f8556612c3b9e/");
            Debug.WriteLine(downloadManager.Url);
            Debug.WriteLine(downloadManager.ReferUrl);
        }

        [TestMethod]
        public void t005_StartDownloadAsync()
        {
            HttpDownloadManager downloadManager = new HttpDownloadManager("http://filehippo.com/download/file/257fb4a25dbb3d60158ff79e2f8f2eeebd9542447fd874a7a89f8556612c3b9e/");
            downloadManager.StartDownload(2);

            Thread.Sleep(new TimeSpan(0,2,0));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mGetLibrary;
using Shared;

namespace mGet
{
    class Program
    {
        public static HttpDownloadManager DownloadManager { get; private set; }

        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                string url = args[0];

                DownloadManager = new HttpDownloadManager(url);
                DownloadManager.DownloadProgress += DownloadManager_DownloadProgress;
                DownloadManager.DownloadCompleted += DownloadManager_DownloadCompleted;

                long fileSize = DownloadManager.GetFileSize();
                Console.WriteLine("{0}",DownloadManager.Url);

                ConsoleHelper.Write("File Name",ConsoleColor.Green);

                Console.Write(" : {0} , ",DownloadManager.FileName);

                ConsoleHelper.Write("File Size",ConsoleColor.Green);

                Console.Write(" : {0}", DownloadInfo.PrettySize(fileSize));
                Console.WriteLine();

                DownloadManager.StartDownload();
            }
        }

        static void DownloadManager_DownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            Console.WriteLine();
            ConsoleHelper.Write(e.OutputFilePath,ConsoleColor.Green);
            Console.WriteLine(" completed.");
        }

        static void DownloadManager_DownloadProgress(object sender, Shared.DownloadProgressEventArgs e)
        {
            ConsoleHelper.ClearCurrentConsoleLine();
            //Console.Write("{0} , Progress : {1:F2}% , Speed : {2}",DownloadManager.FileName,e.Value,DownloadInfo.PrettySize(e.Speed));

            Console.Write("{0} , ",DownloadManager.FileName);
            ConsoleHelper.Write("Progress", ConsoleColor.Green);
            Console.Write(" : {0:F2}% , ",e.Value);
            ConsoleHelper.Write("Speed", ConsoleColor.Green);
            Console.Write(" : {0}", DownloadInfo.PrettySize(e.Speed));
        }

    }
}

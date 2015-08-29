using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using mGetLibrary;
using Newtonsoft.Json;
using Shared;

namespace mGet
{
    class Program
    {
        public static HttpDownloadManager DownloadManager { get; private set; }
        public static Config Configurations;
        public static int fromRow = -1;
        public static int toRow = -1;

        static void Main(string[] args)
        {
            ReadConfigs();

            if (args.Length == 0)
            {
                ProcessCArg();
                return;
            }

            if (args.Length == 1)
            {
                if (args[0].ToLower().Equals("-C".ToLower()))
                {
                    ProcessCArg();
                    return;
                }

                if (args[0].ToLower().Equals("-Q".ToLower()))
                {
                    ProcessQArg();
                    return;
                }

                if (args[0].ToLower().Equals("-D".ToLower()))
                {
                    ProcessDArg();
                    return;
                }

                if (args[0].ToLower().Equals("-A".ToLower()))
                {
                    ProcessAArg();
                    return;
                }

                string url = args[0];


                try
                {
                    Uri uri = new Uri(url);
                    if (uri.IsValid())
                    {
                        ProcessDownload(url);
                    }
                    else
                    {
                        ConsoleHelper.Write("Invalid Url", ConsoleColor.Red);
                        Console.WriteLine();
                    }
                }
                catch (Exception)
                {
                    ConsoleHelper.Write("Invalid Url", ConsoleColor.Red);
                    Console.WriteLine();
                }

            }
        }

        private static void ProcessQArg()
        {
            QueueManager queueManager = new QueueManager();
            var qList = queueManager.GetQueueList();
            int id = 1;
            foreach (Tuple<string, string> tuple in qList)
            {
                ConsoleHelper.Write("Id", ConsoleColor.Green);
                Console.WriteLine(" : {0}", id);
                ConsoleHelper.Write("Url", ConsoleColor.Green);
                Console.WriteLine(" : {0}", tuple.Item2);
                ConsoleHelper.Write("Temporary Directory", ConsoleColor.Green);
                Console.WriteLine(" : {0}", tuple.Item1);
                Console.WriteLine(new string('-', Console.WindowWidth));

                id++;
            }
        }

        private static void ProcessAArg()
        {
            try
            {
                Console.Write("Enter url : ");
                string url = Console.ReadLine();

                Uri uri = new Uri(url);
                if (uri.IsValid())
                {
                    HttpDownloadManager httpDownloadManager=new HttpDownloadManager(url);

                    ConsoleHelper.Write(url,ConsoleColor.Green);
                    Console.Write(" is added to queue.");
                    Console.WriteLine();
                }
                else
                {
                    ConsoleHelper.Write("Invalid Url", ConsoleColor.Red);
                    Console.WriteLine();
                }
            }
            catch (Exception)
            {
                ConsoleHelper.Write("Invalid Url", ConsoleColor.Red);
                Console.WriteLine();
            }
        }

        private static void ProcessDArg()
        {
            QueueManager queueManager = new QueueManager();
            var qList = queueManager.GetQueueList();

            if (qList.Count > 0)
            {
                ProcessQArg();

                Console.Write("Enter 0 to exit or id to delete a download : ");

                string id = Console.ReadLine();

                int idNum = 0;

                try
                {
                    idNum = int.Parse(id);
                }
                catch (Exception)
                {
                    return;
                }

                if (idNum == 0)
                {
                    return;
                }

                if (idNum > qList.Count)
                {
                    ConsoleHelper.Write("Out of range id entered", ConsoleColor.Red);
                    Console.WriteLine();
                    return;
                }

                Tuple<string, string> q = queueManager.GetQueueList()[idNum - 1];

                ConsoleHelper.Write("Id", ConsoleColor.Green);
                Console.WriteLine(" : {0}", id);
                ConsoleHelper.Write("Url", ConsoleColor.Green);
                Console.WriteLine(" : {0}", q.Item2);
                ConsoleHelper.Write("Temporary Directory", ConsoleColor.Green);
                Console.WriteLine(" : {0}", q.Item1);

                Console.Write("Are you sure you want to delete?(Y/N) : ");
                string answer = Console.ReadLine();

                if (string.IsNullOrEmpty(answer))
                {
                    Console.Write("Enter 0 to exit or id to delete a download : ");
                    id = Console.ReadLine();
                    return;
                }

                if (answer.ToLower() == "y".ToLower())
                {
                    Directory.Delete(q.Item1, true);
                }
            }
        }

        private static void ProcessDownload(string url)
        {
            DownloadManager = new HttpDownloadManager(url);
            DownloadManager.DownloadProgress += DownloadManager_DownloadProgress;
            DownloadManager.DownloadCompleted += DownloadManager_DownloadCompleted;

            long fileSize = DownloadManager.GetFileSize();
            Console.WriteLine("{0}", DownloadManager.Url);

            ConsoleHelper.Write("File Name", ConsoleColor.Green);

            Console.Write(" : {0} , ", DownloadManager.FileName);

            ConsoleHelper.Write("File Size", ConsoleColor.Green);

            Console.Write(" : {0}", ByteHelper.PrettySize(fileSize));
            Console.WriteLine();

            DownloadManager.StartDownload(Configurations.NumberOfConnections);
        }

        private static void ProcessCArg()
        {
            PrintParameters();

            Console.Write("Enter 0 to exit or parameter id to set a new value : ");

            string id = Console.ReadLine();

            while (id != "0")
            {
                int idNum = 0;

                try
                {
                    idNum = int.Parse(id);
                }
                catch (Exception)
                {
                    return;
                }

                Parameter parameter = Parameter.GetParameter(x => x.Id == idNum);

                if (parameter == null)
                {
                    ConsoleHelper.Write("Incorect Id", ConsoleColor.Red);
                    Console.WriteLine();
                    Console.Write("Enter 0 to exit or parameter id to set a new value : ");
                    id = Console.ReadLine();
                    continue;
                }

                PrintParameter(x => x.Id == parameter.Id);
                string value = Console.ReadLine();

                Configurations.SetParamaterValue(parameter.Name, value);
                SaveConfigs();

                Console.Write("Enter 0 to exit or parameter id to set a new value : ");
                id = Console.ReadLine();
            }
        }

        static void PrintParameters()
        {
            var parameters = Parameter.GetParameters();

            foreach (Parameter parameter in parameters)
            {
                //string value = typeof(Config).GetProperty(parameter.Name).GetValue(Configurations).ToString();
                string value = Configurations.GetParameterValue(parameter.Name).ToString();

                Console.Write("     ");
                ConsoleHelper.Write(parameter.Id.ToString(), ConsoleColor.Magenta);
                Console.Write(" - ");
                ConsoleHelper.Write(parameter.Name, ConsoleColor.Green);
                Console.Write(" : ");
                Console.Write(value);

                Console.WriteLine();
                Console.Write("         ");
                Console.WriteLine(parameter.Description);

                Console.WriteLine(new string('.', Console.WindowWidth));
            }
        }


        static void PrintParameter(Func<Parameter, bool> predicate)
        {
            Parameter parameter = Parameter.GetParameter(predicate);

            Console.Write("     ");
            ConsoleHelper.Write(parameter.Id.ToString(), ConsoleColor.Magenta);
            Console.Write(" - ");
            ConsoleHelper.Write(parameter.Name, ConsoleColor.Green);
            Console.Write(" : ");
        }

        static void DownloadManager_DownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            Console.WriteLine();
            ConsoleHelper.Write(e.OutputFilePath, ConsoleColor.Green);
            Console.WriteLine(" completed.");
        }

        static void DownloadManager_DownloadProgress(object sender, Shared.DownloadProgressEventArgs e)
        {
            if (fromRow == -1 && toRow == -1)
            {
                ConsoleHelper.ClearCurrentConsoleLine();
            }

            if (fromRow != -1 && toRow != -1)
            {
                ConsoleHelper.ClearConsoleLine(fromRow, toRow);
            }

            //Console.Write("{0} , CN : {1} , Progress : {2:F2}% , Speed : {3}",DownloadManager.FileName,e.Value,DownloadInfo.PrettySize(e.Speed));

            fromRow = Console.CursorTop;

            Console.Write("{0} , ", DownloadManager.FileName);
            ConsoleHelper.Write("CN", ConsoleColor.Green);
            Console.Write(" : {0} , ", e.NumberOfConnections);
            ConsoleHelper.Write("Progress", ConsoleColor.Green);
            Console.Write(" : {0:F2}% , ", e.Value);
            ConsoleHelper.Write("Speed", ConsoleColor.Green);
            Console.Write(" : {0}", ByteHelper.PrettySize(e.Speed));

            toRow = Console.CursorTop;
        }

        private static void CreateDefaultConfigs()
        {
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fileName = "Config.json";
            string configFile = Path.Combine(directory, fileName);

            if (File.Exists(configFile))
            {
                return;
            }

            Config config = new Config();
            config.NumberOfConnections = 8;
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);

            FileStream fileStream = new FileStream(configFile, FileMode.CreateNew);
            StreamWriter streamWriter = new StreamWriter(fileStream);

            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();
            fileStream.Close();
        }

        private static void SaveConfigs()
        {
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fileName = "Config.json";
            string configFile = Path.Combine(directory, fileName);

            if (!File.Exists(configFile))
            {
                return;
            }

            var json = JsonConvert.SerializeObject(Configurations, Formatting.Indented);
            File.Delete(configFile);
            FileStream fileStream = new FileStream(configFile, FileMode.CreateNew);
            StreamWriter streamWriter = new StreamWriter(fileStream);

            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();
            fileStream.Close();
        }

        private static void ReadConfigs()
        {
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fileName = "Config.json";
            string configFile = Path.Combine(directory, fileName);

            if (!File.Exists(configFile))
            {
                CreateDefaultConfigs();
            }

            FileStream fileStream = new FileStream(configFile, FileMode.Open);
            StreamReader streamReader = new StreamReader(fileStream);

            var jsonStr = streamReader.ReadToEnd();
            Configurations = JsonConvert.DeserializeObject<Config>(jsonStr);

            streamReader.Close();
            fileStream.Close();
        }
    }
}

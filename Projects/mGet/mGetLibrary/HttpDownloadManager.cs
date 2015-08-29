using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConnectionManager;
using Http;
using Newtonsoft.Json;
using Shared;

namespace mGetLibrary
{
    public class HttpDownloadManager
    {
        private string _url;
        private string _referUrl;
        private string _host;
        private int _port;
        private string _filePath;
        private string _fileName;
        public event EventHandler<DownloadCompletedEventArgs> DownloadCompleted;
        public event EventHandler<DownloadProgressEventArgs> DownloadProgress;
        private Timer timer;

        private string _fileDirectory;
        private string _downloadFilePath;
        private bool _resume;
        private List<DownloadPart> _parts = new List<DownloadPart>();
        private long _fileSize;

        public HttpDownloadManager(string url, bool resume = false)
        {
            string location = this.GetLocationFromRedirect(url);

            if (string.IsNullOrEmpty(location))
            {
                this.Url = url;
                this.ReferUrl = "";
            }
            else
            {
                this.Url = location;
                this.ReferUrl = url;
            }

            this.Resume = resume;
            this.ExtractInfoFromUrl();

            this.FileSize = this.GetFileSize();

            this.CreateFileDirectory();

            DownloadInfoDb.SaveInfoToJSON(this.FileDirectory,this.Url,this.ReferUrl,this.FileSize);
        }

        public string GetLocationFromRedirect(string url)
        {
            try
            {
                Uri uri = new Uri(url);

                var port = uri.Port;
                var host = uri.Host;
                var filePath = uri.AbsolutePath;

                string methodLine = string.Format("HEAD {0} HTTP/1.1", filePath);
                string hostLine = string.Format("Host: {0}", host);
                string connectionLine = "Connection: Close";

                var requestArray = new string[]
                {
                methodLine,
                hostLine,
                connectionLine,
                "",
                "",
                };

                SocketManager socketManager = new SocketManager();
                var socket = socketManager.GetNextSocket();
                socket.Connect(host, port);

                var request = string.Join("\r\n", requestArray);
                var requestB = Encoding.Default.GetBytes(request);

                socket.Send(requestB);

                int readBytes = 0;
                byte[] buffer = new byte[1024];

                MemoryStream memoryStream = new MemoryStream();

                readBytes = socket.Receive(buffer);

                while (readBytes > 0)
                {
                    memoryStream.Write(buffer, 0, readBytes);

                    readBytes = socket.Receive(buffer);
                }

                socket.Close();

                var dataB = memoryStream.ToArray();

                if (dataB.Length == 0)
                {
                    throw new MGetException(1, "Empty response");
                }

                var data = Encoding.Default.GetString(dataB);

                var dataSplit = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                
                string locationLine = "";

                if (dataSplit.Any(x => x.StartsWith("Location")))
                {
                    locationLine = dataSplit.First(x => x.StartsWith("Location"));
                }
                else
                {
                    return "";
                }

                string location = locationLine.Substring(locationLine.IndexOf(":", StringComparison.Ordinal)+1).Trim();

                return location;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public string FileDirectory
        {
            get { return _fileDirectory; }
            set { _fileDirectory = value; }
        }


        public string DownloadFilePath
        {
            get { return _downloadFilePath; }
            set { _downloadFilePath = value; }
        }

        public bool Resume
        {
            get { return _resume; }
            set { _resume = value; }
        }

        private List<DownloadPart> Parts
        {
            get { return _parts; }
            set { _parts = value; }
        }

        private long FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value; }
        }

        public string ReferUrl
        {
            get { return _referUrl; }
            set { _referUrl = value; }
        }


        private void ExtractInfoFromUrl()
        {
            Uri uri = new Uri(Url);

            if (uri.Scheme != "http")
            {
                throw new Exception("non http scheme passed to http client");
            }

            this.Port = uri.Port;
            this.Host = uri.Host;
            this.FilePath = uri.AbsolutePath;
            this.FileName = Url.Substring(Url.LastIndexOf("/", StringComparison.Ordinal) + 1);
        }

        public void StartDownload()
        {
            Uri uri = new Uri(this.Url);

            if (uri.Scheme == "http")
            {
                long fileSize = this.FileSize;

                if (fileSize > 0)
                {
                    string fileName = this.DownloadFile();

                    if (string.IsNullOrEmpty(fileName))
                    {
                        return;
                    }

                    // save downloaded content
                    byte[] contentBytes = this.ExtractContentFromTemporaryFile(fileName);
                    this.SaveContent(contentBytes);
                    this.RemoveTemporaryFile(fileName);

                    string outputFile = this.MoveContentFromTemporaryFileToOutput();
                    OnDownloadCompleted(new DownloadCompletedEventArgs(outputFile));
                }
            }
        }

        public void StartDownload(int numOfConnections)
        {
            if (numOfConnections == 1)
            {
                this.StartDownload();
                return;
            }

            if (!this.SupportResume())
            {
                this.StartDownload();
                return;
            }

            long fileSize = this.FileSize;
            var splited = ByteHelper.SplitFileSize(fileSize, numOfConnections);

            long start = 0;
            int threadNumber = 1;

            timer = new Timer(TimerTicks, null, 0, 1000);

            foreach (long s in splited)
            {
                long startIndex = start;
                long endIndex = startIndex + s;
                start = endIndex + 1;

                string threadName = String.Format("dl{0}", threadNumber);

                DownloadPart downloadPart = new DownloadPart();
                downloadPart.ThreadName = threadName;
                downloadPart.StartByte = startIndex;
                downloadPart.EndByte = endIndex;
                Parts.Add(downloadPart);
                Thread thread = new Thread(() => DownloadFile(ref downloadPart));
                //thread.IsBackground = false;
                thread.Name = threadName;
                thread.Start();

                threadNumber++;
            }
        }

        private void TimerTicks(object state)
        {
            long speed = 0;
            long downloadedBytes = 0;
            double progress=0.0;

            int numberOfConnections = 0;

            for (int i = 0; i < Parts.Count; i++)
            {
                DownloadPart part = Parts[i];

                if (part.Status == DownloadStatus.Downloaing)
                {
                    speed += part.Speed;
                    numberOfConnections++;
                }

                downloadedBytes += part.DownloadedBytes;
            }

            progress =(double) downloadedBytes /(double) this.FileSize * 100;

            OnDownloadProgress(new DownloadProgressEventArgs(progress,speed,numberOfConnections));
        }

        private void DownloadChunkCompleted(DownloadPart part)
        {
            foreach (DownloadPart downloadPart in Parts)
            {
                if (downloadPart.Status != DownloadStatus.Completed)
                {
                    return;
                }

                // TODO if download has error
            }

            var sortedFiles= this.SortTemporaryFilesForExtraction();

            foreach (string file in sortedFiles)
            {
                // save downloaded content
                byte[] contentBytes = this.ExtractContentFromTemporaryFile(file);
                this.SaveContent(contentBytes,FileMode.Append);
                this.RemoveTemporaryFile(file);
            }

            string outputFile = this.MoveContentFromTemporaryFileToOutput();

            Thread.Sleep(1000);
            OnDownloadCompleted(new DownloadCompletedEventArgs(outputFile));
            // stop timer
            timer.Dispose();
        }

        private void VerifyDownloadIsCompleted()
        {
            // all parts should be completed before sorting
            throw new NotImplementedException();
        }

        private List<string> SortTemporaryFilesForExtraction()
        {
            // tuple : fileName,startIndex,EndIndex,Length
            List<Tuple<long,long,long,string>> ranges=new List<Tuple<long, long, long,string>>();

            foreach (string file in Directory.GetFiles(this.FileDirectory))
            {
                if (PathInfo.GetFileName(file) == "DownloadInfoDb.json")
                {
                    continue;  
                }

                var content = this.ExtractHeadersFromTemporaryFile(file);
                var t = HttpResponse.ExtractContentRange(content);
                Tuple<long,long,long,string> tuple=new Tuple<long,long,long,string>(t.Item1,t.Item2,t.Item3,file);
                ranges.Add(tuple);
            }

            var querySorted = from range in ranges
                orderby range.Item1 ascending 
                select range;

            return querySorted.Select(range => range.Item4).ToList();
        }

        public void StopDonwload()
        {
            throw new NotImplementedException();
        }

        public string MoveContentFromTemporaryFileToOutput()
        {
            // move file to output directory
            string destFile = Path.Combine(mGetEnvironment.ApplicationOutputDirectory, this.FileName);
            string outputFile = PathInfo.GetNextVacantNameForFile(destFile);
            File.Move(this.DownloadFilePath, outputFile);
            Directory.Delete(this.FileDirectory, true);

            return outputFile;
        }

        public long GetFileSize()
        {
            string methodLine = string.Format("HEAD {0} HTTP/1.1", this.FilePath);
            string hostLine = string.Format("Host: {0}", this.Host);
            string connectionLine = "Connection: Close";

            var requestArray = new string[]
            {
                methodLine,
                hostLine,
                connectionLine,
                "",
                "",
            };

            SocketManager socketManager = new SocketManager();
            var socket = socketManager.GetNextSocket();
            socket.Connect(this.Host, this.Port);

            var request = string.Join("\r\n", requestArray);
            var requestB = Encoding.Default.GetBytes(request);

            socket.Send(requestB);

            int readBytes = 0;
            byte[] buffer = new byte[1024];

            MemoryStream memoryStream = new MemoryStream();

            readBytes = socket.Receive(buffer);

            while (readBytes > 0)
            {
                memoryStream.Write(buffer, 0, readBytes);

                readBytes = socket.Receive(buffer);
            }

            socket.Close();

            var dataB = memoryStream.ToArray();

            if (dataB.Length == 0)
            {
                throw new MGetException(1, "Empty response");
            }

            var data = Encoding.Default.GetString(dataB);

            var dataSplit = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);


            string contentLengthLine = "";

            if (dataSplit.Any(x => x.StartsWith("Content-Length")))
            {
                contentLengthLine = dataSplit.First(x => x.StartsWith("Content-Length"));
            }
            else
            {
                throw new MGetException(2, "Unrecognized file size");
            }

            var contentLengthSplit = contentLengthLine.Split(new string[] { ":" }, StringSplitOptions.None);
            string lengthStr = contentLengthSplit[1].Trim();
            long length = long.Parse(lengthStr);

            return length;
        }

        public bool SupportResume()
        {
            string methodLine = string.Format("HEAD {0} HTTP/1.1", this.FilePath);
            string hostLine = string.Format("Host: {0}", this.Host);
            string connectionLine = "Connection: Close";
            string rangeLine = "Range: bytes=0-1";

            var requestArray = new string[]
            {
                methodLine,
                hostLine,
                connectionLine,
                rangeLine,
                "",
                "",
            };

            SocketManager socketManager = new SocketManager();
            var socket = socketManager.GetNextSocket();
            socket.Connect(this.Host, this.Port);

            var request = string.Join("\r\n", requestArray);
            var requestB = Encoding.Default.GetBytes(request);

            socket.Send(requestB);

            int readBytes = 0;
            byte[] buffer = new byte[1024];

            MemoryStream memoryStream = new MemoryStream();

            readBytes = socket.Receive(buffer);

            while (readBytes > 0)
            {
                memoryStream.Write(buffer, 0, readBytes);

                readBytes = socket.Receive(buffer);
            }

            socket.Close();

            var dataB = memoryStream.ToArray();

            HttpResponse httpResponse = HttpResponse.Parse(dataB);

            string rangeValue = httpResponse.Headers.FirstOrDefault(x => x.Key == "Accept-Ranges").Value;
            //string rangeValue = httpResponse.Headers["Accept-Ranges"];

            if (string.IsNullOrEmpty(rangeValue))
            {
                return false;
            }

            if (rangeValue == "none")
            {
                return false;
            }

            return true;
        }

        public string DownloadFile()
        {
            try
            {
                string methodLine = string.Format("GET {0} HTTP/1.1", this.FilePath);
                string hostLine = string.Format("Host: {0}", this.Host);
                string connectionLine = "Connection: Close";

                var requestArray = new string[]
            {
                methodLine,
                hostLine,
                connectionLine,
                "",
                "",
            };

                SocketManager socketManager = new SocketManager();
                var socket = socketManager.GetNextSocket();
                socket.Connect(this.Host, this.Port);

                var request = string.Join("\r\n", requestArray);
                var requestB = Encoding.Default.GetBytes(request);

                socket.Send(requestB);

                int readBytes = 0;
                byte[] buffer = new byte[1024];

                string fileName = "";

                if (FileDirectory.Length > 0)
                {
                    fileName = Path.Combine(FileDirectory, Guid.NewGuid().ToString());
                }
                else
                {
                    fileName = Guid.NewGuid().ToString();
                }

                FileStream fileStream = new FileStream(fileName, FileMode.CreateNew);
                BinaryWriter binaryWriter = new BinaryWriter(fileStream);

                double fileSize = this.FileSize;

                readBytes = socket.Receive(buffer);

                int indexOfContent = this.GetIndexOfContent(buffer);
                DownloadSpeed downloadSpeed = new DownloadSpeed();

                while (readBytes > 0)
                {
                    binaryWriter.Write(buffer, 0, readBytes);
                    binaryWriter.Flush();
                    downloadSpeed.IncreaseDownloadedSize(readBytes);

                    double length = fileStream.Length - indexOfContent;
                    double progress = length / fileSize * 100;

                    OnDownloadProgress(new DownloadProgressEventArgs(progress, downloadSpeed.Speed,1));

                    readBytes = socket.Receive(buffer);
                }

                socket.Close();
                binaryWriter.Close();
                fileStream.Close();

                return fileName;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string DownloadFile(ref DownloadPart part)
        {
            try
            {
                part.Status = DownloadStatus.Downloaing;

                string methodLine = string.Format("GET {0} HTTP/1.1", this.FilePath);
                string hostLine = string.Format("Host: {0}", this.Host);
                string connectionLine = "Connection: Close";
                string rangeLine = String.Format("Range: bytes={0}-{1}", part.StartByte, part.EndByte);

                var requestArray = new string[]
            {
                methodLine,
                hostLine,
                connectionLine,
                rangeLine,
                "",
                "",
            };

                SocketManager socketManager = new SocketManager();
                var socket = socketManager.GetNextSocket();
                socket.Connect(this.Host, this.Port);

                var request = string.Join("\r\n", requestArray);
                var requestB = Encoding.Default.GetBytes(request);

                socket.Send(requestB);

                int readBytes = 0;
                byte[] buffer = new byte[1024];

                string fileName = "";

                if (FileDirectory.Length > 0)
                {
                    fileName = Path.Combine(FileDirectory, Guid.NewGuid().ToString());
                }
                else
                {
                    fileName = Guid.NewGuid().ToString();
                }

                part.TemporaryFileName = fileName;

                FileStream fileStream = new FileStream(fileName, FileMode.CreateNew);
                BinaryWriter binaryWriter = new BinaryWriter(fileStream);

                //double fileSize = this.GetFileSize();

                readBytes = socket.Receive(buffer);

                int indexOfContent = this.GetIndexOfContent(buffer);
                DownloadSpeed downloadSpeed = new DownloadSpeed();

                while (readBytes > 0)
                {
                    binaryWriter.Write(buffer, 0, readBytes);
                    binaryWriter.Flush();

                    downloadSpeed.IncreaseDownloadedSize(readBytes);
                    long length = fileStream.Length - indexOfContent;

                    part.Speed = downloadSpeed.Speed;
                    part.DownloadedBytes = length;

                    //double progress = length / fileSize * 100;

                    readBytes = socket.Receive(buffer);
                }

                socket.Close();
                binaryWriter.Close();
                fileStream.Close();

                part.Status = DownloadStatus.Completed;
                DownloadChunkCompleted(part);

                return fileName;
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#endif
                part.Status = DownloadStatus.Error;
                DownloadChunkCompleted(part);
                return "";
            }
        }
        private int GetIndexOfEndOfHeader(byte[] bytes)
        {
            byte[] search = Encoding.Default.GetBytes("\r\n\r\n");
            int endOfHeader = bytes.IndexOf(search);

            return endOfHeader;
        }

        private int GetIndexOfContent(byte[] bytes)
        {
            int index = GetIndexOfEndOfHeader(bytes) + 4;

            return index;
        }

        public bool SaveContent(byte[] contents,FileMode fileMode=FileMode.CreateNew)
        {
            if (FileDirectory.Length > 0)
            {
                DownloadFilePath = Path.Combine(FileDirectory, this.FileName);
            }
            else
            {
                DownloadFilePath = Path.Combine(System.Reflection.Assembly.GetEntryAssembly().Location, this.FileName);
            }

            FileStream fileStream = new FileStream(DownloadFilePath, fileMode);
            fileStream.Write(contents, 0, contents.Length);
            fileStream.Flush();
            fileStream.Close();

            return true;
        }

        public void CreateFileDirectory()
        {
            string fileNameWithoutExtension = this.FileName.Substring(0, this.FileName.LastIndexOf(".", StringComparison.Ordinal));
            string fileDirectory = Path.Combine(mGetEnvironment.ApplicationDirectory, fileNameWithoutExtension);
            string fileDirectoryVacant = PathInfo.GetNextVacantNameForDirectory(fileDirectory);
            Directory.CreateDirectory(fileDirectoryVacant);

            this.FileDirectory = fileDirectoryVacant;
        }

        public bool SaveContent(byte[] contents, string filePath)
        {
            if (FileDirectory.Length > 0)
            {
                DownloadFilePath = Path.Combine(FileDirectory, filePath);
            }
            else
            {
                DownloadFilePath = Path.Combine(System.Reflection.Assembly.GetEntryAssembly().Location, this.FileName);
            }

            FileStream fileStream = new FileStream(DownloadFilePath, FileMode.CreateNew);
            fileStream.Write(contents, 0, contents.Length);
            fileStream.Flush();
            fileStream.Close();


            return true;
        }

        public byte[] ExtractContentFromTemporaryFile(string fileName)
        {
            byte[] fileBytes = File.ReadAllBytes(fileName);
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            BinaryReader binaryReader = new BinaryReader(fileStream);

            int headerLength = GetIndexOfEndOfHeader(fileBytes);
            byte[] headerBytes = binaryReader.ReadBytes(headerLength);
            HttpResponse httpResponse = HttpResponse.Parse(headerBytes);

            // because of \r\n\r\n
            binaryReader.ReadBytes(4);

            int contentIndex = this.GetIndexOfContent(fileBytes);
            byte[] contentBytes = binaryReader.ReadBytes(fileBytes.Length - contentIndex);

            binaryReader.Close();
            fileStream.Close();

            return contentBytes;
        }

        public HttpResponse ExtractHeadersFromTemporaryFile(string fileName)
        {
            byte[] fileBytes = File.ReadAllBytes(fileName);
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            BinaryReader binaryReader = new BinaryReader(fileStream);

            int headerLength = GetIndexOfEndOfHeader(fileBytes);
            byte[] headerBytes = binaryReader.ReadBytes(headerLength);
            HttpResponse httpResponse = HttpResponse.Parse(headerBytes);

            binaryReader.Close();
            fileStream.Close();

            return httpResponse;
        }

        private long GetContentSizeFromTemporaryFile(string fileName)
        {
            long size = this.ExtractContentFromTemporaryFile(fileName).Length;

            return size;
        }

        public bool RemoveTemporaryFile(string fileName)
        {
            File.Delete(fileName);

            return true;
        }


        protected virtual void OnDownloadProgress(DownloadProgressEventArgs e)
        {
            var handler = DownloadProgress;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnDownloadCompleted(DownloadCompletedEventArgs e)
        {
            var handler = DownloadCompleted;
            if (handler != null) handler(this, e);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConnectionManager;
using Shared;

namespace mGetLibrary
{
    public class HttpDownloadManager
    {
        private string _url;
        private string _host;
        private int _port;
        private string _filePath;
        private string _fileName;
        public event EventHandler<DownloadCompletedEventArgs> DownloadCompleted;
        public event EventHandler<DownloadProgressEventArgs> DownloadProgress;
        public event EventHandler<ChunkDownloadCompletedEventArgs> ChunkDownloadCompleted;
        public event EventHandler<ChunkDownloadProgressEventArgs> ChunkDownloadProgress;

        private string _fileDirectory;
        private string _downloadFilePath;
        private bool _resume;

        public HttpDownloadManager(string url,bool resume=false)
        {
            this.Url = url;
            this.Resume = resume;

            this.ExtractInfoFromUrl();
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
                long fileSize = this.GetFileSize();

                if (fileSize > 0)
                {
                    this.CreateFileDirectory();
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

        public void StopDonwload()
        {
            
        }

        public string MoveContentFromTemporaryFileToOutput()
        {
            // move file to output directory
            string destFile = Path.Combine(mGetEnvironment.ApplicationOutputDirectory, this.FileName);
            string outputFile = PathInfo.GetNextVacantNameForFile(destFile);
            File.Move(this.DownloadFilePath, outputFile);
            Directory.Delete(this.FileDirectory,true);

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

                double fileSize = this.GetFileSize();

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

                    OnDownloadProgress(new DownloadProgressEventArgs(progress, downloadSpeed.Speed));

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

        //public string DownloadFile(long startIndex, long endIndex)
        //{
        //    string methodLine = string.Format("GET {0} HTTP/1.1", this.FilePath);
        //    string hostLine = string.Format("Host: {0}", this.Host);
        //    string connectionLine = "Connection: Close";
        //    string rangeLine = String.Format("Range: bytes={0}-{1}", startIndex, endIndex);

        //    var requestArray = new string[]
        //    {
        //        methodLine,
        //        hostLine,
        //        rangeLine,
        //        connectionLine,
        //        "",
        //        "",
        //    };

        //    SocketManager socketManager = new SocketManager();
        //    var socket = socketManager.GetNextSocket();
        //    socket.Connect(this.Host, this.Port);

        //    var request = string.Join("\r\n", requestArray);
        //    var requestB = Encoding.Default.GetBytes(request);

        //    socket.Send(requestB);

        //    int readBytes = 0;
        //    byte[] buffer = new byte[1024];

        //    string fileName = "";

        //    if (FileDirectory.Length > 0)
        //    {
        //        fileName = Path.Combine(FileDirectory, Guid.NewGuid().ToString());
        //    }
        //    else
        //    {
        //        fileName = Guid.NewGuid().ToString();
        //    }

        //    FileStream fileStream = new FileStream(fileName, FileMode.CreateNew);
        //    BinaryWriter binaryWriter = new BinaryWriter(fileStream);

        //    double fileSize = this.GetFileSize();

        //    readBytes = socket.Receive(buffer);

        //    int indexOfContent = this.GetIndexOfContent(buffer);
        //    DownloadSpeed downloadSpeed = new DownloadSpeed();

        //    while (readBytes > 0)
        //    {
        //        binaryWriter.Write(buffer, 0, readBytes);
        //        binaryWriter.Flush();
        //        downloadSpeed.IncreaseFileSize(readBytes);

        //        double length = fileStream.Length - indexOfContent;
        //        double progress = length / fileSize * 100;
        //        OnDownloadProgress(new DownloadProgressEventArgs(progress, downloadSpeed.Speed));

        //        readBytes = socket.Receive(buffer);
        //    }

        //    socket.Close();
        //    binaryWriter.Close();
        //    fileStream.Close();

        //    OnDownloadCompleted(new DownloadCompletedEventArgs());

        //    return fileName;
        //}

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

        public bool SaveContent(byte[] contents)
        {
            if (FileDirectory.Length > 0)
            {
                DownloadFilePath = Path.Combine(FileDirectory, this.FileName);
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

        protected virtual void OnChunkDownloadCompleted(ChunkDownloadCompletedEventArgs e)
        {
            var handler = ChunkDownloadCompleted;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnChunkDownloadProgress(ChunkDownloadProgressEventArgs e)
        {
            var handler = ChunkDownloadProgress;
            if (handler != null) handler(this, e);
        }
    }
}

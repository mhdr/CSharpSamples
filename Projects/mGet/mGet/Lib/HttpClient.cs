using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace mGet.Lib
{
    public class HttpClient : INetClient
    {
        private string _url;
        private string _host;
        private int _port;
        private string _file;

        public HttpClient(string url)
        {
            this.Url = url;

            this.ExtractInfoFromUrl();
        }

        private string Url
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

        public string File
        {
            get { return _file; }
            set { _file = value; }
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
            this.File = uri.AbsolutePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// if returns -1 it means there is no Content-Length in response headers
        /// </returns>
        public long GetFileSize()
        {
            string methodLine = string.Format("HEAD {0} HTTP/1.1", this.File);
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

            SocketManager socketManager=new SocketManager();
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

            var data = Encoding.Default.GetString(dataB);

            var dataSplit = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);


            string contentLengthLine = "";

            if (dataSplit.Any(x => x.StartsWith("Content-Length")))
            {
                contentLengthLine= dataSplit.First(x => x.StartsWith("Content-Length"));    
            }
            else
            {
                return -1;
            }
            
            var contentLengthSplit = contentLengthLine.Split(new string[] { ":" }, StringSplitOptions.None);
            string lengthStr = contentLengthSplit[1].Trim();
            long length = long.Parse(lengthStr);

            return length;
        }
    }
}

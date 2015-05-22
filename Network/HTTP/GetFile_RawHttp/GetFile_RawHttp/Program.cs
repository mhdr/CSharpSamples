using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GetFile_RawHttp
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = "Keygen.jar";
            var getCmd = string.Format("GET /{0} HTTP/1.1", fileName);

            var host = "192.168.1.3";
            var hostCmd = string.Format("Host: {0}", host);
            string delimter = "\r\n";

            var requestArray = new string[]
            {
                getCmd,
                hostCmd,
                "Connection: Keep-Alive",
                "",
                "",
            };

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(host, 80);

            var request = string.Join(delimter, requestArray);
            var requestB = Encoding.Default.GetBytes(request);

            socket.Send(requestB);

            int readBytes = 0;
            byte[] buffer = new byte[1024];

            MemoryStream memoryStream = new MemoryStream();

            while ((readBytes = socket.Receive(buffer)) > 0)
            {
                memoryStream.Write(buffer, 0, readBytes);
            }

            var dataB = memoryStream.ToArray();
            var data = Encoding.Default.GetString(dataB);

            var splitedData = data.Split(new string[] { delimter + delimter }, StringSplitOptions.None);
            var header = splitedData[0];
            var body = splitedData[1];

            Console.WriteLine(header);

            var newFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
            var writer = new FileStream(newFilePath, FileMode.Create);
            var bodyB = Encoding.Default.GetBytes(body);
            writer.Write(bodyB, 0, bodyB.Length);
            writer.Flush();
            writer.Close();

            socket.Close();

            Console.ReadKey();
        }
    }
}

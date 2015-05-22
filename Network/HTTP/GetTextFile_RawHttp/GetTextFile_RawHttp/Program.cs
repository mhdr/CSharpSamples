using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GetTextFile_RawHttp
{
    class Program
    {
        static void Main(string[] args)
        {
            var requestArray = new string[]
            {
                "GET /hello.txt HTTP/1.1",
                "Host: 192.168.1.3",
                "Connection: Close",
                "",
                "",
            };

            var socket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            socket.Connect("192.168.1.3",80);

            var request = string.Join("\r\n", requestArray);
            var requestB = Encoding.ASCII.GetBytes(request);

            socket.Send(requestB);

            int totalBytesRead = 0;
            int readBytes = 0;
            byte[] buffer=new byte[1024];

            MemoryStream memoryStream=new MemoryStream();

            readBytes = socket.Receive(buffer);

            while (readBytes>0)
            {
                memoryStream.Write(buffer,0,readBytes);
                totalBytesRead += readBytes;

                readBytes = socket.Receive(buffer);
            }

            var dataB = memoryStream.ToArray();

            var data = Encoding.ASCII.GetString(dataB);

            //Console.WriteLine(data);

            var dataSplit = data.Split(new string[]{"\r\n\r\n"},StringSplitOptions.None);
            var header = dataSplit[0];
            var body = dataSplit[1];

            Console.WriteLine(header);
            Console.WriteLine(body);

            socket.Close();

            Console.ReadKey();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket socket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            socket.Connect("127.0.0.1",9011);

            Console.WriteLine("connected...");

            Person person=new Person(){Name = "Mahmood"};

            string jsonData = JsonConvert.SerializeObject(person);
            byte[] dataBytes = Encoding.Default.GetBytes(jsonData);

            socket.Send(dataBytes);

            Console.WriteLine("sent...");

            byte[] buffer=new byte[1024*4];
            int readBytes = socket.Receive(buffer);
            MemoryStream memoryStream = new MemoryStream();

            while (readBytes>0)
            {
                memoryStream.Write(buffer, 0, readBytes);

                if (socket.Available > 0)
                {
                    readBytes = socket.Receive(buffer);   
                }
                else
                {
                    break;
                }
            }

            Console.WriteLine("read...");

            byte[] totalBytes = memoryStream.ToArray();

            memoryStream.Close();

            string readData = Encoding.Default.GetString(totalBytes);

            Greeting response = JsonConvert.DeserializeObject<Greeting>(readData);

            Console.WriteLine(response.Msg);

            Console.ReadKey();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket socket=new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            EndPoint endPoint=new IPEndPoint(IPAddress.Any, 9011);

            socket.Bind(endPoint);
            socket.Listen(5);

            while (true)
            {
                Console.WriteLine("waiting for new connection...");

                Socket newSocket = socket.Accept();

                MemoryStream memoryStream = new MemoryStream();

                Console.WriteLine("new connection...");

                byte[] buffer=new byte[1024];

                int readBytes = newSocket.Receive(buffer);

                while (readBytes>0)
                {
                    memoryStream.Write(buffer,0,readBytes);

                    if (socket.Available > 0)
                    {
                        readBytes = newSocket.Receive(buffer);    
                    }
                    else
                    {
                        break;
                    }
                }

                Console.WriteLine("data received...");

                byte[] totalBytes = memoryStream.ToArray();

                memoryStream.Close();

                string readData = Encoding.Default.GetString(totalBytes);

                Person p= JsonConvert.DeserializeObject<Person>(readData);

                Greeting g = SayHello(p);

                string dataToSend = JsonConvert.SerializeObject(g);

                byte[] dataToSendBytes = Encoding.Default.GetBytes(dataToSend);

                newSocket.Send(dataToSendBytes);

                newSocket.Close();

                Console.WriteLine("data sent...");
            }
        }

        static Greeting SayHello(Person person)
        {
            string msg= string.Format("Hello {0}", person.Name);
            Greeting greeting=new Greeting();
            greeting.Msg = msg;

            return greeting;
        }
    }
}

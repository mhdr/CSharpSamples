using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint endPoint = new IPEndPoint(IPAddress.Any, 9011);

            socket.Bind(endPoint);
            socket.Listen(5);

            while (true)
            {
                Console.WriteLine("waiting for new connection...");

                Socket newSocket = socket.Accept();

                Console.WriteLine("new connection...");

                byte[] buffer = new byte[1024];

                int readBytes = newSocket.Receive(buffer);
                MemoryStream memoryStream = new MemoryStream();

                while (readBytes > 0)
                {
                    memoryStream.Write(buffer, 0, readBytes);

                    if (socket.Available>0)
                    {
                        readBytes = newSocket.Receive(buffer);
                    }
                    else
                    {
                        break;
                    }
                }

                Console.WriteLine("data received...");

                BinaryFormatter formatter=new BinaryFormatter();
                memoryStream.Position = 0;
                Person p = (Person) formatter.Deserialize(memoryStream);

                memoryStream.Close();

                Greeting g = SayHello(p);

                formatter=new BinaryFormatter();
                memoryStream=new MemoryStream();

                formatter.Serialize(memoryStream,g);

                newSocket.Send(memoryStream.ToArray());

                memoryStream.Close();
                newSocket.Close();

                Console.WriteLine("data sent...");
            }
        }

        static Greeting SayHello(Person person)
        {
            string msg = string.Format("Hello {0}", person.Name);
            Greeting greeting = new Greeting();
            greeting.Msg = msg;

            return greeting;
        }
    }
}

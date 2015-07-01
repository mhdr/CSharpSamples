using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect("127.0.0.1", 9011);

            Console.WriteLine("connected...");

            BinaryFormatter formatter=new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            Person person = new Person() { Name = "Mahmood" };

            formatter.Serialize(memoryStream,person);

            byte[] dataBytes = memoryStream.ToArray();
            socket.Send(dataBytes);

            Console.WriteLine("sent...");

            memoryStream = new MemoryStream();
            byte[] buffer = new byte[1024 * 4];
            int readBytes = socket.Receive(buffer);
            
            while (readBytes > 0)
            {
                memoryStream.Write(buffer, 0, readBytes);

                if (socket.Available>0)
                {
                    readBytes = socket.Receive(buffer);
                }
                else
                {
                    break;
                }
            }

            Console.WriteLine("read...");
            formatter=new BinaryFormatter();

            // set position to 0 or create a new stream
            memoryStream.Position = 0;
            Greeting response = (Greeting) formatter.Deserialize(memoryStream);

            Console.WriteLine(response.Msg);

            memoryStream.Close();
            socket.Close();

            Console.ReadKey();
        }
    }
}

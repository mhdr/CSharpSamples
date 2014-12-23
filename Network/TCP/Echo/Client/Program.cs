using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            int port = 9001;

            string msg = "Mahmood";
            byte[] msgB = Encoding.UTF8.GetBytes(msg);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(address,port);

            socket.Send(msgB);

            byte[] buffer=new byte[1024];

            socket.Receive(buffer);

            string data = Encoding.UTF8.GetString(buffer);

            Console.WriteLine(data);

            socket.Close();

            Console.ReadKey();
        }
    }
}

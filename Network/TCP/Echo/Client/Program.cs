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
            string host = "127.0.0.1";
            int port = 9001;
            IPAddress address = IPAddress.Parse(host);

            Socket socket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

            socket.Connect(address,port);

            string data = "Mahmood";
            byte[] dataB = Encoding.UTF8.GetBytes(data);

            socket.Send(dataB);

            byte[] buffer=new byte[1024];
            socket.Receive(buffer);

            socket.Close();

            string dataEcho = Encoding.UTF8.GetString(buffer);

            Console.WriteLine(dataEcho);

            Console.ReadKey();
        }
    }
}

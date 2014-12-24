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
            EndPoint endPoint=new IPEndPoint(address,port);

            Socket socket=new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(endPoint);

            string data = "";
            for (int i = 0; i < 1000; i++)
            {
                data += "Mahmood";
            }

            byte[] dataB = Encoding.UTF8.GetBytes(data);
            int length = dataB.Length;
            byte[] lengthB = BitConverter.GetBytes(length);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lengthB);
            }


        }
    }
}

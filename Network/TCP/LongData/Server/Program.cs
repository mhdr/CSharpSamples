using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = "";
            int port = 9001;

            IPAddress address = IPAddress.Any;
            EndPoint endPoint = new IPEndPoint(address, port);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(endPoint);
            socket.Listen(1);

            while (true)
            {
                Socket newSocket = socket.Accept();

                IPEndPoint remoteEndPoint = (IPEndPoint)newSocket.RemoteEndPoint;
                string newClientMsg = string.Format("new client connected form {0}:{1}", remoteEndPoint.Address.ToString(),
                    remoteEndPoint.Port);

                Console.WriteLine(newClientMsg);

                // first get length of data
                byte[] lengthB=new byte[4];
                newSocket.Receive(lengthB);

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(lengthB);
                }

                int length = BitConverter.ToInt32(lengthB,0);

                byte[] buffer = new byte[length];
                newSocket.Receive(buffer);

                string data = Encoding.UTF8.GetString(buffer);
                Console.WriteLine(data);

                newSocket.Send(buffer);

                newSocket.Close();
            }
        }
    }
}

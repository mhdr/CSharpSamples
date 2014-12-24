using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

                NewSocket clientSocket=new NewSocket(newSocket);

                Thread thread=new Thread(()=>clientSocket.PrintData());
                thread.Start();
            }
        }
    }

    public class NewSocket
    {
        public Socket Socket { get; private set; }

        public NewSocket(Socket socket)
        {
            this.Socket = socket;
        }

        public void PrintData()
        {
            IPEndPoint remoteEndPoint = (IPEndPoint)Socket.RemoteEndPoint;
            string newClientMsg = string.Format("new client connected form {0}:{1}", remoteEndPoint.Address.ToString(),
                remoteEndPoint.Port);

            Console.WriteLine(newClientMsg);

            // first get length of data
            byte[] lengthB = new byte[4];
            Socket.Receive(lengthB);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lengthB);
            }

            int length = BitConverter.ToInt32(lengthB, 0);

            byte[] buffer = new byte[length];
            Socket.Receive(buffer);

            string data = Encoding.UTF8.GetString(buffer);
            Console.WriteLine(data);

            Socket.Send(buffer);

            Socket.Close();
        }
    }
}

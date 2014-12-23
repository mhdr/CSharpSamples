using System;
using System.Collections.Generic;
using System.IO;
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
            IPAddress address=IPAddress.Parse(host);
            EndPoint endPoint=new IPEndPoint(address,port);

            string data = "";
            for (int i = 0; i < 1000; i++)
            {
                data += "Mahmood";
            }

            Console.WriteLine(data);

            byte[] dataB = Encoding.UTF8.GetBytes(data);
            // length of data
            int dataLength = dataB.Length;
            // length of data in bytes
            byte[] dataLengthB = BitConverter.GetBytes(dataLength);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(dataLengthB);
            }

            Socket socket=new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(endPoint);

            // send length of data first
            int dataToSendLength = dataLengthB.Length + dataB.Length;
            byte[] dataToSend=new byte[dataToSendLength];
            Buffer.BlockCopy(dataLengthB,0,dataToSend,0,4);
            Buffer.BlockCopy(dataB,0,dataToSend,dataLengthB.Length,dataB.Length);

            socket.Send(dataToSend);

            socket.Close();

            Console.ReadKey();
        }
    }
}

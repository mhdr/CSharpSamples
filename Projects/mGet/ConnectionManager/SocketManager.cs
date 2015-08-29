using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionManager
{
    public class SocketManager
    {
        public Socket GetNextSocket()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);

            return socket;
        }
    }
}

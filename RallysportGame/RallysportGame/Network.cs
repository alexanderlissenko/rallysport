using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Net;

namespace RallysportGame
{
    class Network
    {

        private Socket socket;

        public Network()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPAddress ip = IPAddress.Parse("192.168.1.255");
            //socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip));
            //socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);

            IPEndPoint ipep = new IPEndPoint(ip, 11245);
            socket.Connect(ipep);
        }

        public void sendData()
        {
            byte[] msg = Encoding.UTF8.GetBytes("BROADCAST");
            Console.WriteLine("network data sending");
            socket.Send(msg, msg.Length, SocketFlags.None);
        }

        public void recieveData()
        {
            byte[] b = new byte[1024];
            socket.Receive(b);
            string str = System.Text.Encoding.ASCII.GetString(b, 0, b.Length);
            Console.WriteLine(str.Trim());
        }

        public void closeSocket()
        {
            socket.Close();
        }

    }
}

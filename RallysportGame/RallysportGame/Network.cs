using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Net;

using OpenTK;

namespace RallysportGame
{
    class Network
    {

        private Socket socket,socket2;
        private EndPoint ep;
        public Network()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPAddress ip = IPAddress.Parse("192.168.1.255");
            //socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip));
            //socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);

            IPEndPoint ipep = new IPEndPoint(ip, 11245);
            socket.Connect(ipep);


            //Reciever

            socket2 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ipep2 = new IPEndPoint(IPAddress.Any, 11245);
            socket2.Bind(ipep2);

            ep = (EndPoint)ipep2;
        }

        public void sendData()
        {
            byte[] msg = Encoding.UTF8.GetBytes("BROADCAST " + DateTime.Now.ToString());
            Console.WriteLine("network data sending " + DateTime.Now.ToString());
            socket.Send(msg, msg.Length, SocketFlags.None);
        }
        public void sendData(Vector3 vector)
        {

            byte[] msg = Encoding.UTF8.GetBytes(vector.ToString()+ DateTime.Now.ToString());
            Console.WriteLine("network data sending" + DateTime.Now.ToString());
            socket.Send(msg, msg.Length, SocketFlags.None);
        }
        public void recieveData()
        {

            byte[] b = new byte[1024];
            if(socket2.Available != 0)
            {
                int recv = socket2.ReceiveFrom(b, ref ep);
                string str = System.Text.Encoding.ASCII.GetString(b, 0, recv);
                Console.WriteLine(str.Trim());
            }
            
        }

        public void closeSocket()
        {
            socket.Close();
            socket2.Close();
        }

    }
}

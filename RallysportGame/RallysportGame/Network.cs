﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

using OpenTK;

namespace RallysportGame
{
    class Network
    {

        private Socket socket;
        private EndPoint ep;

        private IPAddress multicastAddr = IPAddress.Parse("234.123.123.123");

        public Network()
        {
            IPAddress localIp = getLocalIp();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            IPEndPoint localEP = new IPEndPoint(localIp, 11245);
            socket.Bind(localEP);
            
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastAddr,localIp));
            socket.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoDelay, 1);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);

            ep = (EndPoint)localEP;
        }

        public void startSending()
        {

        }

        public void sendData()
        {
            IPEndPoint endpoint = new IPEndPoint(multicastAddr, 11245);
            byte[] msg = Encoding.UTF8.GetBytes("BROADCAST " + DateTime.Now.ToString());
            Console.WriteLine("network data sending " + DateTime.Now.ToString());
            socket.SendTo(msg,endpoint);
        }

        public void sendData(Vector3 vector)
        {
            IPEndPoint endpoint = new IPEndPoint(multicastAddr, 11245);
            byte[] msg = Encoding.UTF8.GetBytes(vector.ToString()+ DateTime.Now.ToString());
            Console.WriteLine("network data sending" + DateTime.Now.ToString());
            socket.SendTo(msg, endpoint);
        }

        public void recieveData()
        {

            byte[] b = new byte[1024];
            try
            {
                if (socket.Available != 0)
                {
                    int recv = socket.ReceiveFrom(b, ref ep);
                    string str = System.Text.Encoding.ASCII.GetString(b, 0, recv);
                    Console.WriteLine(str.Trim());
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
        }

        public void closeSocket()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        private IPAddress getLocalIp()
        {
            IPAddress localIP = IPAddress.None;
            //Should be one per network card
            NetworkInterface[] networkInterface = NetworkInterface.GetAllNetworkInterfaces();
       
            foreach(NetworkInterface net in networkInterface)
            {
                IPInterfaceProperties properties = net.GetIPProperties();
                foreach (IPAddressInformation address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;
                    if (IPAddress.IsLoopback(address.Address))
                        continue;
                    localIP = address.Address;
                }
            }

            return localIP;

        }

    }
}

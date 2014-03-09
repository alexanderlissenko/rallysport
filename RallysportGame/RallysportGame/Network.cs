﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;

using OpenTK;

namespace RallysportGame
{
    class Network
    {

        private Socket socket;
        private EndPoint ep;
        private IPAddress multicastAddr = IPAddress.Parse("234.123.123.123");


        private int userId;
        private bool isLeader = false;
        private int ids=1;

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
            Thread.Sleep(2000);
            if(socket.Available==0)
            {
                Console.WriteLine("Starting Network");
                userId = 0;
                isLeader = true;
            }
            else
            {
                sendData("0");
                Console.WriteLine("Conecting to network");
                for (int i = 0; i < 10; i++ )
                {
                    byte[] b = new byte[1024];
                    int recv = socket.ReceiveFrom(b, ref ep);
                    string str = System.Text.Encoding.ASCII.GetString(b, 0, recv);
                    Console.WriteLine(str);
                    if (str.Length > 1)
                    {
                        if (str.Substring(0, 1).Equals("1"))
                        {
                            userId = int.Parse(str.Substring(1));
                            break;
                        }
                    }
                }
            }
        }

        public void sendData()
        {
            IPEndPoint endpoint = new IPEndPoint(multicastAddr, 11245);
            byte[] msg = Encoding.UTF8.GetBytes("BROADCAST " + DateTime.Now.ToString());
            Console.WriteLine("network data sending " + DateTime.Now.ToString());
            socket.SendTo(msg,endpoint);
        }

        public void sendData(string str)
        {
            IPEndPoint endpoint = new IPEndPoint(multicastAddr, 11245);
            byte[] msg = Encoding.UTF8.GetBytes(str);
            socket.SendTo(msg, endpoint);
        }

        public void sendData(Vector3 vector)
        {
            IPEndPoint endpoint = new IPEndPoint(multicastAddr, 11245);
            byte[] msg = Encoding.UTF8.GetBytes("3"+userId+vector.ToString()+ DateTime.Now.ToString());
            Console.WriteLine("network data sending" + DateTime.Now.ToString());
            socket.SendTo(msg, endpoint);
        }

        public void recieveData()
        {

            byte[] b = new byte[1024];
            string str ="";
            try
            {
                if (socket.Available != 0)
                {
                    int recv = socket.ReceiveFrom(b, ref ep);
                    str = System.Text.Encoding.ASCII.GetString(b, 0, recv);
                    str = str.Trim();
                    Console.WriteLine(str);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            if (!str.Equals(""))
            {
                switch (str.Substring(0, 1))
                {
                    case "0":
                        if (isLeader)
                        {
                            sendData("1" + ids);
                            //ids++;
                        }
                        break;
                    case "1":
                        ids = int.Parse(str.Substring(1))+1;
                        break;
                    case "2":
                        if (int.Parse(str.Substring(1)) == userId)
                            isLeader = true;
                        break;
                    case "3":
                        break;
                    default:
                        break;
                }
            }
        }

        public void closeSocket()
        {
            sendData("2" + (ids - 1));
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
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

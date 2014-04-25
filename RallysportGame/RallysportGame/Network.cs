using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using System.Collections;

using OpenTK;
using BEPUphysics;

namespace RallysportGame
{
    class Network
    {
        //Multicast IP is 234.123.123.123 port is 11245

        private Socket socket;
        private EndPoint ep;
        private IPAddress multicastAddr = IPAddress.Parse("234.123.123.123");

        private bool networkStarted;
        private int userId;
        private bool isLeader = false;
        private int ids=1;
        private static Network instance;

        private ArrayList userList;
        Space space;
        private Network() { }
        private Network(Space space)
        {
            networkStarted = false;
            this.space = space;
            userList = new ArrayList();
            IPAddress localIp = IPAddress.Parse("127.0.0.1");// getLocalIp();//
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            IPEndPoint localEP = new IPEndPoint(localIp, 11245);
            socket.Bind(localEP);
            
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastAddr,localIp));
            socket.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoDelay, 1);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
            ep = (EndPoint)localEP;
        }

        public static void Init(Space space)
        {
            if (instance == null)
            {
                instance = new Network(space);
            }
        }

        public bool getStatus()
        {
            return networkStarted;
        }
        public static Network getInstance()
        {
            if (instance == null)
            {
                throw new Exception("Network not yet initilialized");
            }
            return instance;
        }

        public void startSending()
        {
            networkStarted = true;
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
                    string[] unParsedData = str.Split(';');
                    Console.WriteLine(str);
                    if (str.Length > 1)
                    {
                        if (unParsedData[0].Equals("1"))
                        {
                            userId = int.Parse(unParsedData[1]);
                            ids = userId + 1;
                            break;
                        }
                    }
                }
            }
        }

        public void sendData()
        {
            IPEndPoint endpoint = new IPEndPoint(multicastAddr, 11245);
            byte[] msg = Encoding.UTF8.GetBytes("BROADCAST");
            Console.WriteLine("network data sending");
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
            byte[] msg = Encoding.UTF8.GetBytes("3;"+userId+";"+vector.ToString());
            Console.WriteLine("network data sending userlist: " + userList.Count);
            socket.SendTo(msg, endpoint);
        }

        public void recieveData(ref ArrayList carList)
        {

            byte[] b = new byte[1024];
            string str ="";
            string[] unParsedData = null;
            try
            {
                if (socket.Available != 0)
                {
                    int recv = socket.ReceiveFrom(b, ref ep);
                    str = System.Text.Encoding.ASCII.GetString(b, 0, recv);
                    str = str.Trim();
                    unParsedData = str.Split(';');
                    Console.WriteLine(unParsedData[0]);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            if (unParsedData != null)
            {
                int id,index;
                switch (unParsedData[0])
                {
                    case "0":
                        if (isLeader)
                        {
                            sendData("1;" + ids);
                        }
                        break;
                    case "1":
                        ids++;
                        id = int.Parse(unParsedData[1]);
                        break;
                    case "2":
                        id = int.Parse(unParsedData[1]);
                        index = userList.IndexOf(id);
                        carList.RemoveAt(index);
                        userList.Remove(id);
                        if (id == userId)
                            isLeader = true;
                        break;
                    case "3":
                        id =int.Parse(unParsedData[1]);
                        index = userList.IndexOf(id);
                        if (id != userId)
                        {
                            if (index == -1)
                            {
                                userList.Add(id);
                                carList.Add(new Car(@"Mustang\mustang-no-wheels", @"Mustang\one-wheel-tex-scale", new Vector3(float.Parse(unParsedData[2].Substring(1)), float.Parse(unParsedData[3]), float.Parse(unParsedData[4].Remove(unParsedData[4].Length - 1))),space));
                                Console.WriteLine(carList.Count);
                            }
                            else
                            {
                                
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public void closeSocket()
        {
            userList.Reverse();
            if(userList.Count != 0)
                sendData("2;" + userList[0]);
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

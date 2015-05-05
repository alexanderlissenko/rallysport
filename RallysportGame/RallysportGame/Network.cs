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
        private IPAddress multicastAddr = IPAddress.Parse("233.234.234.234");

        private bool networkStarted;
        public int userId;
        private bool isLeader = false;
        private int ids=1;
        private static Network instance;
        private ArrayList userList;
        Space space;
        private Car car;
        private Network() { }
        private Network(Space space)
        {
            networkStarted = false;
            this.space = space;
            userList = new ArrayList();
            IPAddress localIp = IPAddress.Parse("127.0.0.1");//getLocalIp();//IPAddress.Parse("192.168.1.1");//
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            IPEndPoint localEP = new IPEndPoint(localIp, 11245);
            socket.Bind(localEP);
            
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastAddr,localIp));
            socket.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoDelay, 1);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 10);
            ep = (EndPoint)localEP;
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 0);
        }

        public static void Init(Space space)
        {
            if (instance == null)
            {
                instance = new Network(space);
            }
        }

        public void setCar(Car c)
        {
            this.car = c;
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
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer,1);
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
        public void sendData(Vector3 vector,Quaternion rot)
        {
            IPEndPoint endpoint = new IPEndPoint(multicastAddr, 11245);
            byte[] msg = Encoding.UTF8.GetBytes("3;" + userId + ";" + vector.X + ";" + vector.Y + ";" + vector.Z + ";" + rot.X + ";" + rot.Y + ";" + rot.Z + ";" + rot.W + ";");
            Console.WriteLine("network data sending userlist: " + userList.Count);
            socket.SendTo(msg, endpoint);
        }
        public void sendData(Vector3 vector, Quaternion rot,float acceleration)
        {
            IPEndPoint endpoint = new IPEndPoint(multicastAddr, 11245);
            byte[] msg = Encoding.UTF8.GetBytes("3;" + userId + ";" + vector.X + ";" + vector.Y + ";" + vector.Z + ";" + rot.X + ";" + rot.Y + ";" + rot.Z + ";" + rot.W + ";"+ acceleration+";");
            Console.WriteLine("network data sending userlist: " + userList.Count);
            socket.SendTo(msg, endpoint);
        }
        public void sendStart()
        {
            IPEndPoint endpoint = new IPEndPoint(multicastAddr, 11245);
            byte[] msg = Encoding.UTF8.GetBytes("4;"+userId+";0;");
            Console.WriteLine("raceing starts");
            socket.SendTo(msg, endpoint);
        }
        public void sendFinish()
        {
            IPEndPoint endpoint = new IPEndPoint(multicastAddr, 11245);
            byte[] msg = Encoding.UTF8.GetBytes("4;" + userId + ";1;");
            Console.WriteLine("Finish!");
            socket.SendTo(msg, endpoint);
        }
        public void sendPowerUp(string powerup, Vector3 vector, Quaternion rot, float acceleration)
        {
            int powerupInt = -1;
            switch(powerup)
            {
                case "SpeedBoost":
                    powerupInt = 1;
                    break;
                case "Missile":
                    powerupInt = 2;
                    break;
                case "LightsOut":
                    powerupInt = 3;
                    break;
                case "SmookeScreen":
                    powerupInt = 4;
                    break;
                default:
                    break;

            }
            if (powerupInt != -1)
            {
                IPEndPoint endpoint = new IPEndPoint(multicastAddr, 11245);
                byte[] msg = Encoding.UTF8.GetBytes("3;" + userId + ";" + powerupInt + ";" + vector.X + ";" + vector.Y + ";" + vector.Z + ";" + rot.X + ";" + rot.Y + ";" + rot.Z + ";" + rot.W + ";" + acceleration + ";");
                Console.WriteLine("powerup sent!");
                socket.SendTo(msg, endpoint);
            }
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
                        Car ca = carList[index] as Car;
                        ca.deleteCarFromSpace();
                        carList.RemoveAt(index);
                        userList.Remove(id);
                        if (int.Parse(unParsedData[2]) == userId)
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
                                carList.Add(new Car(@"Mustang\mustang-textured-scale_mini", @"Mustang\one_wheel_corected_normals_recenterd", new Vector3(float.Parse(unParsedData[2]), float.Parse(unParsedData[3]), float.Parse(unParsedData[4])), space,id));
                                Console.WriteLine(carList.Count);
                            }
                            else
                            {
                                object o = carList[index];
                                Car c = o as Car;
                                //c.setCarPos(new Vector3(float.Parse(unParsedData[2].Substring(1)), float.Parse(unParsedData[3]), float.Parse(unParsedData[4].Remove(unParsedData[4].Length - 1))));
                                c.setCarPos(new Vector3(float.Parse(unParsedData[2]), float.Parse(unParsedData[3]), float.Parse(unParsedData[4])),new Quaternion(float.Parse(unParsedData[5]),float.Parse(unParsedData[6]),float.Parse(unParsedData[7]),float.Parse(unParsedData[8])));
                                c.accelerate(float.Parse(unParsedData[9]));
                                c.networkAccel(float.Parse(unParsedData[9]));
                                //carList[index] = c;
                            }
                        }
                        break;
                    case "4":
                        int trigger = int.Parse(unParsedData[2]);
                        if (trigger == 0)
                        {
                            RaceState.StartRace(car,ref carList);
                        }
                        if (trigger == 1)
                        {
                            Console.WriteLine("player " + int.Parse(unParsedData[1]) + "has past the finnish line");
                        }
                        if(trigger == 2)
                        {
                            Car c = null; 
                            id = int.Parse(unParsedData[1]);
                            index = userList.IndexOf(id);
                            if (id != userId)
                            {
                                if (index == -1)
                                {
                                    userList.Add(id);
                                    carList.Add(new Car(@"Mustang\mustang-textured-scale_mini", @"Mustang\one_wheel_corected_normals_recenterd", new Vector3(float.Parse(unParsedData[2]), float.Parse(unParsedData[3]), float.Parse(unParsedData[4])), space, id));
                                    Console.WriteLine(carList.Count);
                                }
                                else
                                {
                                    object o = carList[index];
                                    c = o as Car;
                                    c.setCarPos(new Vector3(float.Parse(unParsedData[3]), float.Parse(unParsedData[4]), float.Parse(unParsedData[5])), new Quaternion(float.Parse(unParsedData[6]), float.Parse(unParsedData[7]), float.Parse(unParsedData[8]), float.Parse(unParsedData[9])));
                                    c.accelerate(float.Parse(unParsedData[10]));
                                }
                            }
                            if (c != null)
                            {
                                int powerup = int.Parse(unParsedData[2]);
                                switch (powerup)
                                {
                                    case 1:
                                        c.addPowerUp("SpeedBoost");
                                        break;
                                    case 2:
                                        c.addPowerUp("Missile");
                                        break;
                                    case 3:
                                        c.addPowerUp("LightsOut");
                                        break;
                                    case 4:
                                        c.addPowerUp("SmookeScreen");
                                        break;
                                    default:
                                        break;
                                }
                                c.usePowerUp();
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
                sendData("2;"+userId +";"+ userList[0]);
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

        public int getUserID()
        {
            return userId;
        }

        public ArrayList getUserList()
        {
            return userList;
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

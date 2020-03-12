﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Management;

namespace MoviePlayer.Protocol
{
    class UdpInit
    {
        public static IPEndPoint ipLocalPoint;
        public static IPEndPoint ipepBingding;
        public static EndPoint RemotePoint;
        public static EndPoint BroadcastRemotePoint;
        public static Socket mySocket;

        //public static EndPoint RemotePointBox;             //驱动器远程端
        //public static Socket mySocketBox;                  //驱动器Socket  
        /// <summary>
        /// udp初始化
        /// </summary>
        public void udpInit()
        {
            IPAddress ip;
            int port;
            //int portBox;         //驱动器端口
            string localIp;
            localIp = GetLocalIP();
            string[] splitIp = localIp.Split('.');

            if (localIp == "127.0.0.1")
            {
                MessageBox.Show("The network is not connected. Please connect to the network");
                Debug.Write("获取不到IP地址");
            }

            //判断能否获取到ip地址
            if (splitIp[0] == "169")
            {
                SetNetworkAdapter();
            }

            //得到本机IP，设置UDP端口号    
            bool b = IPAddress.TryParse(localIp, out ip);

            port = 1032;                                       //自动分配端口
            ipLocalPoint = new IPEndPoint(ip, port);

            //定义网络类型，数据连接类型和网络协议UDP  
            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //绑定网络地址 
            try
            {
                mySocket.Bind(ipLocalPoint);
            }
            catch
            {
                //MessageBox.Show("Network ports are occupied");
                //System.Environment.Exit(0);
            }
            uint IOC_IN = 0x80000000;
            uint IOC_VENDOR = 0x18000000;
            uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
            mySocket.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);

            if (MainWindow.PlayMac != "TRUE")
            {
                //设置广播地址
                ip = IPAddress.Broadcast;
                port = 1030;                                             //中控板端口
                IPEndPoint ipep = new IPEndPoint(ip, port);
                BroadcastRemotePoint = (EndPoint)(ipep);
                //设置客户机IP，默认为广播地址
                RemotePoint = (EndPoint)(ipep);
                //允许广播
                mySocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            }
            //portBox = 7408;                                          //驱动器端口
            //IPEndPoint ipepBox = new IPEndPoint(ip,portBox);
            //RemotePointBox = (EndPoint)(ipepBox);
            //mySocketBox.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.Broadcast,1);       
            UdpConnect myUdpConnect = new UdpConnect();
        }
   

        /// <summary>
        /// 修改静态IP地址
        /// </summary>
        private void SetNetworkAdapter()
        {
            //ManagementBaseObject inPar = null;
            //ManagementBaseObject outPar = null;
            //ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            //ManagementObjectCollection moc = mc.GetInstances();
            //foreach (ManagementObject mo in moc)
            //{
            //    if (!(bool)mo["IPEnabled"])
            //        continue;

            //    //设置ip地址和子网掩码 
            //    inPar = mo.GetMethodParameters("EnableStatic");
            //    inPar["IPAddress"] = new string[] { "192.168.1.112", "192.168.1.249" }; // 1.备用 2.IP
            //    inPar["SubnetMask"] = new string[] { "255.255.255.0", "255.255.255.0" };
            //    outPar = mo.InvokeMethod("EnableStatic", inPar, null);

            //    //设置网关地址 
            //    inPar = mo.GetMethodParameters("SetGateways");
            //    inPar["DefaultIPGateway"] = new string[] { "192.168.1.1", "192.168.16.254" }; // 1.网关;2.备用网关 
            //    outPar = mo.InvokeMethod("SetGateways", inPar, null);

            //    //设置DNS 
            //    inPar = mo.GetMethodParameters("SetDNSServerSearchOrder");
            //    inPar["DNSServerSearchOrder"] = new string[] { "211.97.168.129", "202.102.152.3" }; // 1.DNS 2.备用DNS 
            //    outPar = mo.InvokeMethod("SetDNSServerSearchOrder", inPar, null);
            //    break;
            //}
        }

        public static EndPoint transformIP(string strIP,string strPort)
        {
            IPAddress ip;
            IPAddress.TryParse(strIP,out ip);
            int port;
            Int32.TryParse(strPort,out port);
            IPEndPoint ipep = new IPEndPoint(ip, port);
            return (EndPoint)(ipep);
        }

        /// <summary>
        /// 获取本地ip地址
        /// </summary>
        public static  string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名  
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址  
                    //AddressFamily.InterNetwork表示此IP为IPv4,  
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型  
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "192.168.1.109";
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取本机IP出错:" + ex.Message);
                return "";
            }
        }
    }
}

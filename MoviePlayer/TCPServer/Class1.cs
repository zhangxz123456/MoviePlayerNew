using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace MoviePlayer.TCPServer
{
    class MyServer
        { 


        //用于通信的Socket
        Socket socketSend;
        //用于监听的SOCKET
        Socket socketWatch;

        //创建监听连接的线程
        Thread AcceptSocketThread;
        //接收客户端发送消息的线程
        Thread threadReceive;

        /// <summary>
        /// 开始监听
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public MyServer()
        {
            //当点击开始监听的时候 在服务器端创建一个负责监听IP地址和端口号的Socket
            socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //获取ip地址
            IPAddress ip = IPAddress.Parse("192.168.1.109");
            //创建端口号
            IPEndPoint point = new IPEndPoint(ip, 1038);
            //绑定IP地址和端口号
            socketWatch.Bind(point);
            //this.txt_Log.AppendText("监听成功" + " \r \n");
            //开始监听:设置最大可以同时连接多少个请求
            socketWatch.Listen(10);

            //创建线程
            AcceptSocketThread = new Thread(new ParameterizedThreadStart(StartListen));
            AcceptSocketThread.IsBackground = true;
            AcceptSocketThread.Start(socketWatch);
        }

        /// <summary>
        /// 等待客户端的连接，并且创建与之通信用的Socket
        /// </summary>
        /// <param name="obj"></param>
        private void StartListen(object obj)
        {
            Socket socketWatch = obj as Socket;
            while (true)
            {
                //等待客户端的连接，并且创建一个用于通信的Socket
                socketSend = socketWatch.Accept();
                //获取远程主机的ip地址和端口号
                string strIp = socketSend.RemoteEndPoint.ToString();
                string strMsg = "远程主机：" + socketSend.RemoteEndPoint + "连接成功";

                //定义接收客户端消息的线程
                threadReceive = new Thread(new ParameterizedThreadStart(ReceiveData));
                threadReceive.IsBackground = true;
                threadReceive.Start(socketSend);

            }
        }

        /// <summary>
        /// 服务器端不停的接收客户端发送的消息
        /// </summary>
        /// <param name="obj"></param>
        private void ReceiveData(object obj)
        {
            Socket socketSend = obj as Socket;
            while (true)
            {
                //客户端连接成功后，服务器接收客户端发送的消息
                byte[] buffer = new byte[2048];
                //实际接收到的有效字节数
                int count = socketSend.Receive(buffer);
                if (count == 0)//count 表示客户端关闭，要退出循环
                {
                    break;
                }
                else
                {
                    if(count==5)
                    {
                        if(buffer[0]==0xff && buffer[1]==0x30 && buffer[2] == 0x4a)
                        {
                            string index = buffer[3].ToString();
                            MessageBox.Show("播放第" + index + "部电影");                            
                        }
                    }
                    if(count==4)
                    {
                        if (buffer[0] == 0xff && buffer[1] == 0x31 && buffer[2] == 0x4b)
                        {
                            //string index = buffer[3].ToString();
                            MessageBox.Show("暂停");
                        }
                        if (buffer[0] == 0xff && buffer[1] == 0x32 && buffer[2] == 0x4c)
                        {
                            string index = buffer[3].ToString();
                            MessageBox.Show("停止");
                        }
                        if (buffer[0] == 0xff && buffer[1] == 0x33 && buffer[2] == 0x4d)
                        {
                            string index = buffer[3].ToString();
                            MessageBox.Show("恢复播放状态");
                        }
                    }
                    
                }
            }
        }

        /// <summary>
        /// 停止监听
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseTCPServer(object sender, EventArgs e)
        {
            socketWatch.Close();
            socketSend.Close();
            //终止线程
            AcceptSocketThread.Abort();
            threadReceive.Abort();
        }
    }
}

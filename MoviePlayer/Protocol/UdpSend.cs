using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.IO;

namespace MoviePlayer.Protocol
{
    class UdpSend
    {
        public static byte flagSend;
        public static double count;
        public static int rate;
        public static int tempRate;
        public static int range;
        public static int playCount;
        public static bool shakeFlag;
        public static bool circleFlag;
        public static int timeCodeTemp;
        public static bool movieStop;
        public static byte dataLight;

        public UdpSend()
        {
            Thread ThreadUdpSend = new Thread(new ThreadStart(Send));
            ThreadUdpSend.IsBackground = true; //设置为后台线程
            ThreadUdpSend.Start();

            //构造函数
        }

        public static void UdpSendData(byte[] data, int len, EndPoint ip)
        {
            UdpInit.mySocket.SendTo(data, len, SocketFlags.None, ip);
            Debug.WriteLine("Send Data{0}", count++);
            Debug.WriteLine(ModbusUdp.ByteToHexStr(data));
        }


        public static void SendWrite(double pos)
        {
            byte[] data;
            ushort addr;
            ushort len;
            byte[] array;          //data+addr+len 
            byte[] Data;           //最终发送的数据
            pos = pos * 2400;
            Debug.WriteLine(pos);
            addr = 0;
            len = 10;
            data = new byte[len];
            int num1 = 3 * (int)(pos / 50);                  //actionFile数组下标
            int num2 = 3 * (int)(pos / 50) + 1;
            int num3 = 3 * (int)(pos / 50) + 2;
            int num4 = 2 * (int)(pos / 50);                  //effectFile数组下标
            int num5 = 2 * (int)(pos / 50) + 1;
            if (num1 - timeCodeTemp > 9 && num1 - timeCodeTemp < 0)
            {
                num1 = timeCodeTemp + 3;
                num2 = timeCodeTemp + 4;
                num3 = timeCodeTemp + 5;
            }
            Debug.WriteLine(num1 + " " + num2 + " " + num3);
            try
            {
                if (num3 > Module.actionFile.Length)
                {
                    if ("2DOF".Equals(MainWindow.PlayDOF))
                    {
                        //两自由度数据
                        data[0] = 127;                      //1号缸            
                        data[1] = 127;                      //2号缸
                        data[2] = 127;                      //3号缸
                    }
                    else
                    {
                        //三自由度数据
                        data[0] = 0;                          //1号缸            
                        data[1] = 0;                          //2号缸
                        data[2] = 0;                          //3号缸
                    }

                }
                else
                {
                    data[0] = (byte)(Module.actionFile[num1] * MainWindow.PlayHeight / 100);      //1号缸            
                    data[1] = (byte)(Module.actionFile[num2] * MainWindow.PlayHeight / 100);      //2号缸
                    data[2] = (byte)(Module.actionFile[num3] * MainWindow.PlayHeight / 100);                     //3号缸                   

                    //data[0] = Module.actionFile[num1];
                    //data[1] = Module.actionFile[num2];
                    //data[2] = Module.actionFile[num3];
                }

                data[3] = 0;                                                    //4号缸
                data[4] = 0;                                                    //5号缸
                data[5] = 0;                                                    //6号缸
                //data[6] = 0;                                                  //保留
                //data[7] = 0;                                                  //保留

                if (Module.shakeFile != null)
                {
                    if (num5 > Module.shakeFile.Length)
                    {
                        data[6] = 0;                                                 //振幅  
                        data[7] = 0;                                                 //频率 
                    }
                    else
                    {
                        data[6] = Module.shakeFile[num4];
                        data[7] = Module.shakeFile[num5];
                    }
                }

                if (num5 > Module.effectFile.Length)
                {
                    data[8] = 0;                                                 //座椅特效  
                    //data[9] = 0;                                               //环境特效 
                    data[9] = dataLight;
                }
                else
                {

                    data[8] = Module.effectFile[num5];                          //座椅特效  
                    //data[9] = Module.effectFile[num4];                        //环境特效 
                    data[9] = (byte)(Module.effectFile[num4] | dataLight);
                }
                Debug.WriteLine((int)(3 * (pos / 50)));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            array = ModbusUdp.ArrayAdd(addr, len, data);
            Data = ModbusUdp.MBReqWrite(array);

            UdpSendData(Data, Data.Length, UdpInit.RemotePoint);
            timeCodeTemp = num1;
        }

        public static void SendWrite6DOF(double pos)
        {
            byte[] data;
            ushort addr;
            ushort len;
            byte[] array;          //data+addr+len 
            byte[] Data;           //最终发送的数据
            pos = pos * 2400;
            Debug.WriteLine(pos);
            addr = 0;
            len = 10;
            data = new byte[len];

            int num1 = 6 * (int)(pos / 50);                  //actionFile数组下标
            int num2 = 6 * (int)(pos / 50) + 1;
            int num3 = 6 * (int)(pos / 50) + 2;
            int num4 = 6 * (int)(pos / 50) + 3;
            int num5 = 6 * (int)(pos / 50) + 4;
            int num6 = 6 * (int)(pos / 50) + 5;

            int numEffect1 = 2 * (int)(pos / 50);                  //effectFile数组下标
            int numEffect2 = 2 * (int)(pos / 50) + 1;
            if (num1 - timeCodeTemp > 9 && num1 - timeCodeTemp < 0)
            {
                num1 = timeCodeTemp + 3;
                num2 = timeCodeTemp + 4;
                num3 = timeCodeTemp + 5;
            }
            Debug.WriteLine(num1 + " " + num2 + " " + num3);
            try
            {
                if (num6 > Module.actionFile.Length)
                {
                    //六自由度数据
                    data[0] = 0;                      //1号缸            
                    data[1] = 0;                      //2号缸
                    data[2] = 0;                      //3号缸
                    data[3] = 0;                      //4号缸            
                    data[4] = 0;                      //5号缸
                    data[5] = 0;                      //6号缸
                }
                else
                {
                    data[0] = (byte)(Module.actionFile[num1] * MainWindow.PlayHeight / 100);      //1号缸            
                    data[1] = (byte)(Module.actionFile[num2] * MainWindow.PlayHeight / 100);      //2号缸
                    data[2] = (byte)(Module.actionFile[num3] * MainWindow.PlayHeight / 100);      //3号缸                   
                    data[3] = (byte)(Module.actionFile[num4] * MainWindow.PlayHeight / 100);      //4号缸            
                    data[4] = (byte)(Module.actionFile[num5] * MainWindow.PlayHeight / 100);      //5号缸
                    data[5] = (byte)(Module.actionFile[num6] * MainWindow.PlayHeight / 100);      //6号缸    
                }
                if (Module.shakeFile != null)
                {
                    if (numEffect2 > Module.shakeFile.Length)
                    {
                        data[6] = 0;                                                 //振幅  
                        data[7] = 0;                                                 //频率 
                    }
                    else
                    {
                        data[6] = Module.shakeFile[numEffect1];
                        data[7] = Module.shakeFile[numEffect2];
                    }
                }

                if (numEffect2 > Module.effectFile.Length)
                {
                    data[8] = 0;                                                 //座椅特效  
                    //data[9] = 0;                                               //环境特效 
                    data[9] = dataLight;
                }
                else
                {
                    data[8] = Module.effectFile[numEffect2];                    //座椅特效  
                    //data[9] = Module.effectFile[num4];                        //环境特效 
                    data[9] = (byte)(Module.effectFile[numEffect1] | dataLight);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            array = ModbusUdp.ArrayAdd(addr, len, data);
            Data = ModbusUdp.MBReqWrite(array);

            UdpSendData(Data, Data.Length, UdpInit.RemotePoint);
            timeCodeTemp = num1;
        }

        public static void SendTotal(double pos)
        {
            byte[] data;
            ushort addr;
            ushort len;
            byte[] array;          //data+addr+len 
            byte[] Data;           //最终发送的数据
            switch (MainWindow.PlayFrame)
            {
                case 48:
                    pos = pos * 2400;
                    break;
                case 60:
                    pos = pos * 3000;
                    break;
                case 120:
                    pos = pos * 6000;
                    break; 
            }
            //5D模式下默认使用60帧
            //if (MainWindow.PlayType.Equals("5D"))
            //{
            //    pos = pos * 3000;
            //}
            Debug.WriteLine(pos);
            addr = 0;
            len = 10;
            data = new byte[len];
            int num1 = 3 * (int)(pos / 50);                  //actionFile2DOF与actionFile3DOF数组下标
            int num2 = 3 * (int)(pos / 50) + 1;
            int num3 = 3 * (int)(pos / 50) + 2;

            int numEffect1 = 2 * (int)(pos / 50);                  //effectFile与shakeFile数组下标
            int numEffect2 = 2 * (int)(pos / 50) + 1;
            try
            {
                switch (MainWindow.PlayDOF)
                {
                    case "2DOF":
                        if (Module.actionFile2DOF != null)
                        {
                            if (num3 > Module.actionFile2DOF.Length)
                            {
                                data[0] = 127;
                                data[1] = 127;
                                data[2] = 127;
                            }
                            else
                            {
                                data[0] = (byte)(Module.actionFile2DOF[num1] * MainWindow.PlayHeight / 100);      //1号缸            
                                data[1] = (byte)(Module.actionFile2DOF[num2] * MainWindow.PlayHeight / 100);      //2号缸
                                data[2] = (byte)(Module.actionFile2DOF[num3] * MainWindow.PlayHeight / 100);      //3号缸  
                            }
                        }
                        else
                        {
                            data[0] = 127;
                            data[1] = 127;
                            data[2] = 127;
                        }
                        break;
                    case "3DOF":
                        if (Module.actionFile3DOF != null)
                        {
                            if (num3 > Module.actionFile3DOF.Length)
                            {
                                data[0] = 0;
                                data[1] = 0;
                                data[2] = 0;
                            }
                            else
                            {
                                data[0] = (byte)(Module.actionFile3DOF[num1] * MainWindow.PlayHeight / 100);      //1号缸            
                                data[1] = (byte)(Module.actionFile3DOF[num2] * MainWindow.PlayHeight / 100);      //2号缸
                                data[2] = (byte)(Module.actionFile3DOF[num3] * MainWindow.PlayHeight / 100);      //3号缸  
                            }
                        }
                        break;
                    case "6DOF":
                        int num6DOF1 = 6 * (int)(pos / 50);                  //actionFile6DOF数组下标
                        int num6DOF2 = 6 * (int)(pos / 50) + 1;
                        int num6DOF3 = 6 * (int)(pos / 50) + 2;
                        int num6DOF4 = 6 * (int)(pos / 50) + 3;
                        int num6DOF5 = 6 * (int)(pos / 50) + 4;
                        int num6DOF6 = 6 * (int)(pos / 50) + 5;
                        if (Module.actionFile6DOF != null)
                        {
                            if (num6DOF6 > Module.actionFile6DOF.Length)
                            {
                                //六自由度数据
                                data[0] = 0;                      //1号缸            
                                data[1] = 0;                      //2号缸
                                data[2] = 0;                      //3号缸
                                data[3] = 0;                      //4号缸            
                                data[4] = 0;                      //5号缸
                                data[5] = 0;                      //6号缸
                            }
                            else
                            {
                                data[0] = (byte)(Module.actionFile6DOF[num6DOF1] * MainWindow.PlayHeight / 100);      //1号缸            
                                data[1] = (byte)(Module.actionFile6DOF[num6DOF2] * MainWindow.PlayHeight / 100);      //2号缸
                                data[2] = (byte)(Module.actionFile6DOF[num6DOF3] * MainWindow.PlayHeight / 100);      //3号缸                   
                                data[3] = (byte)(Module.actionFile6DOF[num6DOF4] * MainWindow.PlayHeight / 100);      //4号缸            
                                data[4] = (byte)(Module.actionFile6DOF[num6DOF5] * MainWindow.PlayHeight / 100);      //5号缸
                                data[5] = (byte)(Module.actionFile6DOF[num6DOF6] * MainWindow.PlayHeight / 100);      //6号缸    
                            }
                        }
                        break;
                }

                if (Module.shakeFile != null)
                {
                    if (numEffect2 > Module.shakeFile.Length)
                    {
                        data[6] = 0;                                                 //振幅  
                        data[7] = 0;                                                 //频率 
                    }
                    else
                    {
                        data[6] = Module.shakeFile[numEffect1];
                        data[7] = Module.shakeFile[numEffect2];
                    }
                }
                if (Module.effectFile != null)
                {
                    if (numEffect2 > Module.effectFile.Length)
                    {
                        data[8] = 0;
                        data[9] = dataLight;
                    }
                    else
                    {
                        data[8] = Module.effectFile[numEffect2];                         //座椅特效 
                        data[9] = (byte)(Module.effectFile[numEffect1] | dataLight);     //环境特效
                    }
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
           

            array = ModbusUdp.ArrayAdd(addr, len, data);
            Data = ModbusUdp.MBReqWrite(array);
            UdpSendData(Data, Data.Length, UdpInit.RemotePoint);
        }

        public static void SendTotalNew(double pos)
        {
            byte[] data;
            ushort addr;
            ushort len;
            byte[] array;          //data+addr+len 
            byte[] Data;           //最终发送的数据
            switch (MainWindow.PlayFrame)
            {
                case 48:
                    pos = pos * 2400;
                    break;
                case 60:
                    pos = pos * 3000;
                    break;
                case 120:
                    pos = pos * 6000;
                    break;
            }
            //5D模式下默认使用60帧
            //if (MainWindow.PlayType.Equals("5D"))
            //{
            //    pos = pos * 3000;
            //}
            Debug.WriteLine(pos);
            addr = 0;
            len = 42;
            data = new byte[len];
            int num1 = 3 * (int)(pos / 50);                  //actionFile2DOF与actionFile3DOF数组下标
            int num2 = 3 * (int)(pos / 50) + 1;
            int num3 = 3 * (int)(pos / 50) + 2;

            int numEffect1 = 2 * (int)(pos / 50);                  //effectFile与shakeFile数组下标
            int numEffect2 = 2 * (int)(pos / 50) + 1;
            try
            {
                switch (MainWindow.PlayDOF)
                {
                    case "2DOF":
                        if (Module.actionFile2DOF != null)
                        {
                            if (num3 > Module.actionFile2DOF.Length)
                            {
                                data[0] = 127;
                                data[1] = 127;
                                data[2] = 127;
                            }
                            else
                            {
                                data[0] = (byte)(Module.actionFile2DOF[num1] * MainWindow.PlayHeight / 100);      //1号缸            
                                data[1] = (byte)(Module.actionFile2DOF[num2] * MainWindow.PlayHeight / 100);      //2号缸
                                data[2] = (byte)(Module.actionFile2DOF[num3] * MainWindow.PlayHeight / 100);      //3号缸  
                            }
                        }
                        else
                        {
                            data[0] = 127;
                            data[1] = 127;
                            data[2] = 127;
                        }
                        break;
                    case "3DOF":
                        if (Module.actionFile3DOF != null)
                        {
                            if (num3 > Module.actionFile3DOF.Length)
                            {
                                data[0] = 0;
                                data[1] = 0;
                                data[2] = 0;
                            }
                            else
                            {
                                data[0] = (byte)(Module.actionFile3DOF[num1] * MainWindow.PlayHeight / 100);      //1号缸            
                                data[1] = (byte)(Module.actionFile3DOF[num2] * MainWindow.PlayHeight / 100);      //2号缸
                                data[2] = (byte)(Module.actionFile3DOF[num3] * MainWindow.PlayHeight / 100);      //3号缸  
                            }
                        }
                        break;
                    case "6DOF":
                        int num6DOF1 = 6 * (int)(pos / 50);                  //actionFile6DOF数组下标
                        int num6DOF2 = 6 * (int)(pos / 50) + 1;
                        int num6DOF3 = 6 * (int)(pos / 50) + 2;
                        int num6DOF4 = 6 * (int)(pos / 50) + 3;
                        int num6DOF5 = 6 * (int)(pos / 50) + 4;
                        int num6DOF6 = 6 * (int)(pos / 50) + 5;
                        if (Module.actionFile6DOF != null)
                        {
                            if (num6DOF6 > Module.actionFile6DOF.Length)
                            {
                                //六自由度数据
                                data[0] = 0;                      //1号缸            
                                data[1] = 0;                      //2号缸
                                data[2] = 0;                      //3号缸
                                data[3] = 0;                      //4号缸            
                                data[4] = 0;                      //5号缸
                                data[5] = 0;                      //6号缸
                            }
                            else
                            {
                                data[0] = (byte)(Module.actionFile6DOF[num6DOF1] * MainWindow.PlayHeight / 100);      //1号缸            
                                data[1] = (byte)(Module.actionFile6DOF[num6DOF2] * MainWindow.PlayHeight / 100);      //2号缸
                                data[2] = (byte)(Module.actionFile6DOF[num6DOF3] * MainWindow.PlayHeight / 100);      //3号缸                   
                                data[3] = (byte)(Module.actionFile6DOF[num6DOF4] * MainWindow.PlayHeight / 100);      //4号缸            
                                data[4] = (byte)(Module.actionFile6DOF[num6DOF5] * MainWindow.PlayHeight / 100);      //5号缸
                                data[5] = (byte)(Module.actionFile6DOF[num6DOF6] * MainWindow.PlayHeight / 100);      //6号缸    
                            }
                        }
                        break;
                }

                if (Module.shakeFile != null)
                {
                    if (numEffect2 > Module.shakeFile.Length)
                    {
                        data[6] = 0;                                                 //振幅  
                        data[7] = 0;                                                 //频率 
                    }
                    else
                    {
                        data[6] = Module.shakeFile[numEffect1];
                        data[7] = Module.shakeFile[numEffect2];
                    }
                }
                if (Module.effectFile != null)
                {
                    if (numEffect2 > Module.effectFile.Length)
                    {
                        data[8] = 0;
                        data[9] = dataLight;
                    }
                    else
                    {
                        data[8] = Module.effectFile[numEffect2];                         //座椅特效 
                        data[9] = (byte)(Module.effectFile[numEffect1] | dataLight);     //环境特效
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            byte[] data1 = new byte[30];
            for (int i = 0; i < 30; i++)
            {
                data1[i] = (byte)Module.dmx512File[(int)(pos / 50) * 30 + i];
            }
            data1.CopyTo(data, 12);           
            array = ModbusUdp.ArrayAdd(addr, len, data);
            Data = ModbusUdp.MBReqWrite(array);
            UdpSendData(Data, Data.Length, UdpInit.RemotePoint);
        }

        /// <summary>
        /// 影片结束发送复位指令
        /// </summary>
        public static void SendReset()
        {
            byte[] data;
            ushort addr;
            ushort len;
            byte[] array;          //data+addr+len 
            byte[] Data;           //最终发送的数据

            addr = 0;
            len = 10;
            data = new byte[len];
            if ("2DOF".Equals(MainWindow.PlayDOF))
            {
                //两自由度数据
                data[0] = 127;
                data[1] = 127;
                data[2] = 127;
            }
            else
            {
                //三自由度数据
                data[0] = 0;
                data[1] = 0;
                data[2] = 0;
            }
            //复位指令
            if (movieStop == true)
            {
                data[8] = 0;
            }
            else
            {
                data[8] = 1;
            }
            array = ModbusUdp.ArrayAdd(addr, len, data);
            Data = ModbusUdp.MBReqWrite(array);
            UdpSendData(Data, Data.Length, UdpInit.RemotePoint);
        }


        /// <summary>
        /// 影片结束发送复位指令
        /// </summary>
        public static void SendZero()
        {
            byte[] data;
            ushort addr;
            ushort len;
            byte[] array;          //data+addr+len 
            byte[] Data;           //最终发送的数据

            addr = 0;
            len = 10;
            data = new byte[len];
            array = ModbusUdp.ArrayAdd(addr, len, data);
            Data = ModbusUdp.MBReqWrite(array);
            UdpSendData(Data, Data.Length, UdpInit.RemotePoint);
        }

        /// <summary>
        /// int整型转换成byte数组
        /// </summary>
        /// <param name="value">传入的整型参数值</param>
        /// <returns></returns>
        public static byte[] intToBytes(int value)
        {
            byte[] src = new byte[4];
            src[0] = (byte)((value >> 24) & 0xFF);
            src[1] = (byte)((value >> 16) & 0xFF);
            src[2] = (byte)((value >> 8) & 0xFF);
            src[3] = (byte)(value & 0xFF);
            return src;
        }

        //#region MyRegion
        ///// <summary>
        ///// 三合一驱动器发送指令
        ///// </summary>
        //public static void UdpSendDataQuDong(byte[] data, int len, EndPoint ip)
        //{
        //    UdpInit.mySocket.SendTo(data, len, SocketFlags.None, ip);
        //    Debug.WriteLine("Send Data{0}", count++);
        //    Debug.WriteLine(ModbusUdp.ByteToHexStr(data));
        //}
        //public static void QuDong(double pos)
        //{
        //    byte[] data_buf = new byte[38];
        //    //int data_len = 0;
        //    byte[] src = new byte[4];

        //    pos = pos * 1000;
        //    Debug.WriteLine(pos);
        //    int num1 = 3 * (int)(pos / 50);                 //actionFile数组下标
        //    int num2 = 3 * (int)(pos / 50) + 1;
        //    int num3 = 3 * (int)(pos / 50) + 2;
        //    int X;                                         //实际脉冲数 
        //    int Y;
        //    int Z;

        //    //实际脉冲数

        //    if (num3 < Module.actionFile.Length)
        //    {
        //        X = Module.actionFile[num1] * 1089;
        //        Y = Module.actionFile[num2] * 1089;
        //        Z = Module.actionFile[num3] * 1089;
        //    }
        //    else
        //    {
        //        X = 0;
        //        Y = 0;
        //        Z = 0;
        //    }

        //    //震动脉冲数
        //    /*
        //    if (rate==2)
        //    {
        //        X = 170000;
        //        Y = 170000;
        //        Z = 170000;
        //        rate = 0;
        //    }
        //    else
        //    {
        //        X = 0;
        //        Y = 0;
        //        Z = 0;
        //        rate++;
        //    }
        //    */

        //    try
        //    {
        //        data_buf[0] = 0x55;
        //        data_buf[1] = 0xaa;
        //        data_buf[2] = 0;
        //        data_buf[3] = 0;

        //        data_buf[4] = 0x14;
        //        data_buf[5] = 0x01;
        //        data_buf[6] = 0;
        //        data_buf[7] = 0;

        //        data_buf[8] = 0xff;
        //        data_buf[9] = 0xff;
        //        data_buf[10] = 0xff;
        //        data_buf[11] = 0xff;

        //        src = intToBytes(Line++);
        //        Array.Copy(src, 0, data_buf, 12, 4);

        //        //data_buf[12] = 0;
        //        //data_buf[13] = 0;
        //        //data_buf[14] = 0;
        //        //data_buf[15] = 0x01;

        //        data_buf[16] = 0;
        //        data_buf[17] = 0;
        //        data_buf[18] = 0;
        //        data_buf[19] = 0x32;

        //        src = intToBytes(X);
        //        Array.Copy(src, 0, data_buf, 20, 4);

        //        src = intToBytes(Y);
        //        Array.Copy(src, 0, data_buf, 24, 4);

        //        src = intToBytes(Z);
        //        Array.Copy(src, 0, data_buf, 28, 4);

        //        //震动参数
        //        data_buf[32] = 0x0f;
        //        data_buf[33] = 0xff;
        //        data_buf[34] = 0x12;
        //        data_buf[35] = 0x34;
        //        data_buf[36] = 0x56;
        //        data_buf[37] = 0x78;

        //        /*
        //        //正常播放
        //        data_buf[32] = 0x12;
        //        data_buf[33] = 0x34;
        //        data_buf[34] = 0x56;
        //        data_buf[35] = 0x78;
        //        data_buf[36] = 0xab;
        //        data_buf[37] = 0xcd;
        //        */
        //    }
        //    catch (Exception err)
        //    {
        //        MessageBox.Show(err.Message, "发送失败");
        //    }
        //    UdpSendDataQuDong(data_buf, data_buf.Length, UdpInit.RemotePointBox);
        //    // return data_buf;
        //}
        //#endregion

        public static byte[] SendRead()
        {
            byte[] data;
            ushort addr;
            ushort len;
            byte[] array;          //data+addr+len 
            byte[] Data;           //最终发送的数据

            addr = 0;
            len = 10;
            data = new byte[0];

            array = ModbusUdp.ArrayAdd(addr, len, data);
            Data = ModbusUdp.MBReqRead(array);

            return Data;
        }

        public static byte[] SendWriteChip()
        {
            byte[] data;
            ushort addr;
            ushort len;
            byte[] array;          //data+addr+len 
            byte[] Data;           //最终发送的数据

            addr = 0;
            len = 8;
            data = new byte[len];
            data[0] = Module.deadlineOrPermanent;
            data[1] = Module.deadlineYY;
            data[2] = Module.deadlineMM;
            data[3] = Module.deadlineDD;
            data[4] = Module.YY;
            data[5] = Module.MM;
            data[6] = Module.DD;
            data[7] = 1;

            array = ModbusUdp.ArrayAdd(addr, len, data);
            Data = ModbusUdp.MBReqWriteChip(array);
            return Data;

        }

        /// <summary>
        /// 发送读芯片指令
        /// </summary>
        /// <returns></returns>
        public static byte[] SendReadChip()
        {
            byte[] data;
            ushort addr;
            ushort len;
            byte[] array;          //data+addr+len 
            byte[] Data;           //最终发送的数据

            addr = 0;
            len = 8;
            data = new byte[0];

            array = ModbusUdp.ArrayAdd(addr, len, data);
            Data = ModbusUdp.MBReqReadChip(array);
            return Data;
        }


        /// <summary>
        /// 发送获取id指令
        /// </summary>
        /// <returns></returns>
        public static byte[] SendGetId()
        {
            byte[] Data;           //最终发送的数据
            Data = ModbusUdp.MBReqUuid();
            return Data;
        }


        /// <summary>
        /// 发送获取时间码指令
        /// </summary>
        /// <returns></returns>
        public static byte[] SendGetTimeCode()
        {
            byte[] data;
            ushort addr;
            ushort len;
            byte[] array;          //data+addr+len 
            byte[] Data;           //最终发送的数据

            addr = 0;
            len = 4;
            data = new byte[0];

            array = ModbusUdp.ArrayAdd(addr, len, data);
            Data = ModbusUdp.MBReqGetTimeCode(array);
            return Data;
        }


        /// <summary>
        /// 根据功能码来判断要发送的指令
        /// </summary>
        public static void Send()
        {
            //byte[] data ;
            //ushort addr ;      
            //ushort len ;
            //byte[] array;          //data+addr+len 
            byte[] Data;           //最终发送的数据

            while (true)
            {
                if (UdpConnect.flagValue == true)   //连接成功标志
                {
                    switch (flagSend)              //发送标志
                    {
                        case 101:
                            Data = ModbusUdp.MBReqMonitor(1);
                            break;
                        case 102:                   //read_ram                           
                            Data = SendRead();
                            break;
                        //case 103:                  //write_ram
                        // Data = SendWrite(Window1.sliderPositionValue);
                        // break;
                        case 104:                //read_falsh                           
                            Data = SendReadChip();
                            flagSend = 0;
                            break;
                        case 105:               //write_falsh                           
                            Data = SendWriteChip();
                            flagSend = 0;
                            break;
                        case 106:             //GetId
                            //Data = Mcu.ModbusUdp.MBReqUuid();
                            Data = SendGetId();
                            flagSend = 0;
                            break;
                        case 107:            //GetTimeCode                           
                            Data = SendGetTimeCode();
                            break;
                        default:
                            Data = new byte[0];
                            break;
                    }
                    UdpSend.UdpSendData(Data, Data.Length, UdpInit.RemotePoint);
                }
                Thread.Sleep(1000);
            }
        }
    }
}


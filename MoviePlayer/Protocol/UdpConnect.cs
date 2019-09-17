using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Diagnostics;
using System.Windows.Threading;
using System.IO;
namespace MoviePlayer.Protocol
{
    public class UdpConnect
    {
        public static bool flagValue = false;
        public static double TimeCode;
        public static string strTimeCode;
        public static string strLongTimeCode;
        public static string uuid;
        public byte monitorTickRx;
        public byte monitorTickTimeOut;
        public static bool clickRegisterFlag;                 //是否点击注册按钮
        public static bool isRegistered;                      //是否已经成功注册
        public static bool isDebug;                           //是否打开调试界面  
        public static bool connectFlag;                       //是否连接
        public static string valDate;                         //有效期

        UdpSend mysend = new UdpSend();

        //Player p = new Player();
        public UdpConnect()
        {
            Thread ThreadUdpServer = new Thread(new ThreadStart(this.UdpServerTask));
            ThreadUdpServer.IsBackground = true;                                      //设置为后台线程
            ThreadUdpServer.Start();
            //构造函数
        }

        private void UdpServerTask()
        {
            //启动一个新的线程，执行方法this.ReceiveHandle，  
            //以便在一个独立的进程中执行数据接收的操作  
            byte monitorTick = 0;
            Thread thread = new Thread(new ThreadStart(this.ReceiveHandle));
            thread.IsBackground = true; //设置为后台线程
            thread.Start();

            //发送UDP数据包  
            byte[] data;

            while (true)
            {
                if (flagValue == false)
                {
                    data = ModbusUdp.MBReqConnect();
                    UdpSend.UdpSendData(data, data.Length, UdpInit.BroadcastRemotePoint);
                    Debug.WriteLine("Search server");
                    Debug.WriteLine("连接" + data);
                    //Module.WriteLogFile("重新连接");
                }

                else
                {
                    //发送UDP心跳包
                    if (monitorTickRx != monitorTick)
                    {
                        if (monitorTickRx >= 0)
                        {
                            monitorTickTimeOut++;
                        }
                    }

                    if (monitorTickTimeOut == 5)     //计时超过5秒，重新连接
                    {
                        flagValue = false;
                        connectFlag = false;
                        monitorTick = 0;
                        monitorTickRx = 0;
                        monitorTickTimeOut = 0;
                        Debug.WriteLine("Connect lose...");
                        Module.WriteLogFile("连接丢失");
                    }

                    if (monitorTick < 0xff)
                    {
                        monitorTick++;
                    }
                    else
                    {
                        monitorTick = 0;
                    }

                    data = ModbusUdp.MBReqMonitor(monitorTick);
                    UdpSend.UdpSendData(data, data.Length, UdpInit.RemotePoint);
                    Debug.WriteLine("Connect monitor...");
                }
                Thread.Sleep(1000);
            }
        }

        delegate void ReceiveCallback(int rlen, byte[] data);

        

        public void SetReceiveData(int rlen, byte[] data)
        {
            byte[] RecData;
            RecData = new byte[rlen];
            //byte[] realData;
            //realData = new byte[10];
            Array.Copy(data, 0, RecData, 0, rlen);
            
            //Module.controlCommand = System.Text.Encoding.Default.GetString(RecData);
            //Module.WriteLogFile(Module.controlCommand +"    Len:"+ Module.controlCommand.Length);
            //string str = System.Text.Encoding.ASCII.GetString(RecData);
            Debug.WriteLine(ModbusUdp.ByteToHexStr(RecData));
            //Debug.WriteLine("字符长度为：" + Module.controlCommand.Length);
            if (RecData[0] == 0xff && RecData[1] == 0x6c)
            {
                /*保存时间码信息*/
                //Array.Copy(RecData, 14, realData, 0, 10);
                //try
                //{
                //    string path= @"c:\" + Module.nowTimeStr+".txt";
                //    FileStream fs = new FileStream(path, FileMode.Append);
                //    StreamWriter sw = new StreamWriter(fs);
                //    string pwd = "";
                //    for (int i = 0; i < realData.Length; i++)
                //    {                       
                //        pwd = pwd + (realData[i].ToString("X").Length < 2 ? "0" + realData[i].ToString("X") : realData [i].ToString("X"));                        
                //    }
                //    sw.WriteLine(pwd);              
                //    sw.Close();
                //}
                //catch (IOException ex)
                //{
                //    Console.WriteLine(ex.Message);
                //    Console.ReadLine();
                //    return;
                //}

                //要发送数据格式
                double hours = (RecData[6]) * 60 * 60;
                double minutes = (RecData[7]) * 60;
                double seconds = RecData[8];
                double frame = RecData[9] / 24.000;

                // s[i].ToString("X").Length < 2 ? "0" + s[i].ToString("X") : s[i].ToString("X"));
                string strhours = RecData[6].ToString().Length < 2 ? "0" + RecData[6].ToString() : RecData[6].ToString();
                string strminutes = RecData[7].ToString().Length < 2 ? "0" + RecData[7].ToString() : RecData[7].ToString();
                string strseconds = RecData[8].ToString().Length < 2 ? "0" + RecData[8].ToString() : RecData[8].ToString();
                string strframe = RecData[9].ToString().Length < 2 ? "0" + RecData[9].ToString() : RecData[9].ToString();

                strTimeCode = strhours + ":" + strminutes + ":" + strseconds;
                strLongTimeCode = strhours + ":" + strminutes + ":" + strseconds + ":" + strframe;
                TimeCode = hours + minutes + seconds + frame;
                if(isRegistered == true && isDebug == false && "4DM".Equals(MainWindow.PlayType))
                {
                  UdpSend.SendWrite(TimeCode);              
                }
                //UdpSend.QuDong(TimeCode);               //发送驱动器指令
                //UdpSend.flagSend = (byte)Mcu.ModbusUdp.MBFunctionCode.Write;
            }

            if (RecData[0] == 0xff && RecData[1] == 0x65)        //判断心跳应答
            {
                monitorTickRx = RecData[2];
            }

            if (RecData[0] == 0xff && RecData[1] == 0x6a && RecData[2] == 0)       //判断uuid应答
            {
                uuid = "";
                for (int i = 0; i < RecData[3]; i++)
                {
                    uuid += ((Char)(RecData[i + 4])).ToString();
                }
                Debug.WriteLine("uuid:" + uuid);
                connectFlag = true;
                if (uuid == Module.uuidFile)      //判断uuid是否与初始的一致
                {                   
                    UdpSend.flagSend = (byte)ModbusUdp.MBFunctionCode.ReadChip;                       
                }
                //else
                //{
                //  MessageBox.Show ("uuid不正确");
                //}
            }

            if (RecData[0] == 0xff && RecData[1] == 0x68)     //校验日期
            {
                //MessageBox.Show("校验日期");
                //获取储存在芯片的期限时间
                Module.deadlineYY = RecData[7];
                Module.deadlineMM = RecData[8];
                Module.deadlineDD = RecData[9];
                string reyear;
                if (Module.deadlineYY > 9)
                {
                     reyear = "20" + Module.deadlineYY;
                }
                else
                {
                     reyear = "200" + Module.deadlineYY;
                }
                //string reyear = "20" + Module.deadlineYY;
                if (Module.deadlineMM > 31)
                {
                    Module.deadlineMM = 1;
                }
                string remonth = Module.deadlineMM.ToString();
                if (Module.deadlineDD > 31)
                {
                    Module.deadlineDD = 1;
                }
                string reday = Module.deadlineDD.ToString();
                string redate = reyear + "-" + remonth + "-" + reday;
                valDate = redate;
                //获取储存在芯片的当前时间
                string yearWrite;
                if (RecData[10] > 9)
                {
                    yearWrite = "20" + RecData[10];
                }
                else
                {
                    yearWrite = "200" + RecData[10];
                }

                string monthWrite = RecData[11].ToString();               
                string dayWrite = RecData[12].ToString();
                string dateWrite = yearWrite + "-" + monthWrite + "-" + dayWrite;
                
                //获取当前电脑的系统时间
                DateTime dateNow = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                DateTime getDate,getDateWrite;               
                try
                {
                    getDate = Convert.ToDateTime(redate);
                    getDateWrite = Convert.ToDateTime(dateWrite);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    getDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());                    
                    getDateWrite = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                }

                TimeSpan ts = getDate - dateNow;
                TimeSpan ts1 = dateNow - getDateWrite ;

                int getday = ts.Days;                //到期时间跟电脑时间对比
                int contractDay = ts1.Days;          //写进芯片的当前时间跟现在电脑时间对比 

                if (getday <= 0||contractDay<0)
                {
                    //isRegistered = false;
                    Module.deadlineYY = 0;
                    Module.deadlineMM = 1;
                    Module.deadlineDD = 1;
                    UdpSend.flagSend = (byte)ModbusUdp.MBFunctionCode.WriteChip;
                }
                else
                {
                    isRegistered = true;
                    UdpSend.flagSend = (byte)ModbusUdp.MBFunctionCode.WriteChip;
                    switch (getday)
                    {
                        case 9:
                            //MessageBox.Show("提示：使用期限还有10天");
                            Module.hintShow = true;
                            Module.deadlineDay = 10;
                            break;
                        case 5:
                            //MessageBox.Show("提示：使用期限还有6天");
                            Module.hintShow = true;
                            Module.deadlineDay = 5;
                            break;
                        case 2:
                            // MessageBox.Show("提示：使用期限还有3天");
                            Module.hintShow = true;
                            Module.deadlineDay = 3;
                            break;
                        case 1:
                            // MessageBox.Show("提示：使用期限还有2天");
                            Module.hintShow = true;
                            Module.deadlineDay = 2;
                            break;
                        case 0:
                            Module.hintShow = true;
                            Module.deadlineDay = 1;
                            //MessageBox.Show("提示：使用期限还有1天");
                            break;
                        default:
                            break;
                    }
                    //判断日期结束发送复位数据(两自由度)
                    //if (clickRegisterFlag == false)
                    //{
                    //    UdpSend.SendReset();
                    //}
                }
            }  //校验日期结尾


            RecData = ModbusUdp.MBRsp(RecData);
            Debug.WriteLine(ModbusUdp.ByteToHexStr(RecData));
            if (RecData != null)
            {
                if (RecData[0] == 0 && RecData[1] == 0 && RecData[2] == 0x01 && RecData[3] == 0x41)
                {
                    flagValue = true;
                    //要发送数据格式
                    UdpSend.flagSend = (byte)ModbusUdp.MBFunctionCode.GetId;
                }

                if (RecData[0] == 0 && RecData[1] == 0xff && RecData[2] == 0 && RecData[3] == 0x1)
                {
                    while (true)
                    {
                        byte[] Data = new byte[8];
                        Data[0] = 0xff;
                        Data[1] = 0x64;
                        Data[2] = 0x00;
                        Data[3] = 0x00;
                        Data[4] = 0x01;
                        Data[5] = 0xc8;
                        Data[6] = 0x65;
                        Data[7] = 0xda;
                        UdpSend.UdpSendData(Data, Data.Length, UdpInit.BroadcastRemotePoint);
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        private void ReceiveHandle()
        {
            //接收数据处理线程  
            byte[] data = new byte[1024];

            while (true)
            {
                if (UdpInit.mySocket == null || UdpInit.mySocket.Available < 1)
                {
                    Thread.Sleep(10);
                    continue;
                }

                //接收UDP数据报，引用参数RemotePoint获得源地址 
                try
                {
                    int rlen = UdpInit.mySocket.ReceiveFrom(data, ref UdpInit.RemotePoint);
                    ReceiveCallback tx = SetReceiveData;
                    tx(rlen, data);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    //data = Mcu.ModbusUdp.MBReqConnect();
                    UdpSend.UdpSendData(ModbusUdp.MBReqConnect(), ModbusUdp.MBReqConnect().Length, UdpInit.BroadcastRemotePoint);
                    Debug.WriteLine("Search server");
                }
            }
        }
    }
}

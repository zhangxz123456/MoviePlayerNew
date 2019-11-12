using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows;

namespace MoviePlayer.Class
{
    class SerialPortCom
    {
        public static List<string> List_Portnames = new List<string>();
        SerialPort Sp_com;
        private byte[] data;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data">byte[]字节数组</param>
        /// <param name="com_name">串口名字</param>
        public SerialPortCom(byte[] data, string com_name)
        {
            this.data = data;
            //设置通讯端口号及波特率、数据位、停止位和校验位。
            Sp_com = new SerialPort(com_name, 9600, Parity.None, 8, StopBits.One);
            //遍历串行端口名称数组
            //foreach (string port in System.IO.Ports.SerialPort.GetPortNames())
            //{
            // List_Portnames.Add(port);
            //}
            ContPort(com_name);
        }
        /// <summary>
        /// 连接串口
        /// </summary>
        private void ContPort(string com_name)
        {
            if (Sp_com.IsOpen)
            {
                Sp_com.DataReceived += new SerialDataReceivedEventHandler(Sp_com_DataSend);
            }
            else
            {
                try
                {
                    Sp_com.Open();
                    Sp_com.DataReceived += new SerialDataReceivedEventHandler(Sp_com_DataSend);
                }
                catch
                {
                    MessageBox.Show("串口连接失败！");
                    return;
                }
            }
        }
        /// <summary>
        /// 串口发送数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sp_com_DataSend(object sender, SerialDataReceivedEventArgs e)
        {
            //把byte转换成二进制字符串
            string strResult = string.Empty;
            string strTemp;
            for (int i = 0; i < data.Length; i++)
            {
                strTemp = System.Convert.ToString(data[i], 2);
                strTemp = strTemp.Insert(0, new string('0', 8 - strTemp.Length));
                strResult += strTemp;
            }
            Sp_com.Write(strResult);
            Console.Write(strResult);
       }

    }
}
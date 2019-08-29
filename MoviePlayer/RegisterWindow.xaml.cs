using MoviePlayer.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MoviePlayer
{
    /// <summary>
    /// RegisterWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public static string userCode;                  //用户码
        public static string registerCode;              //注册码
        public static string random;                    //随机数
        public byte[] md5Data = new byte[16];
        public string registerSuccessTips;
        public string registerFaileTips;
        public string registerAbnormalTips;


        delegate int PSQ_GET_DATA(String s, ref byte a);
        delegate byte GETMD5(String s, ref byte a);

        public RegisterWindow()
        {
            InitializeComponent();
            changWinRegisterLanguage();
        }


        private void changWinRegisterLanguage()
        {
            if ("CN".Equals(MainWindow.PlayLanguage))
            {                
                registerSuccessTips = "注册成功";
                registerFaileTips = "注册失败";
                registerAbnormalTips = "输入的注册码长度不正确，请检查";

            }
            else
            {
                registerSuccessTips = "Register Success";
                registerFaileTips = "Register failed";
                registerAbnormalTips = "Input the length of the registration code is not correct, please check";
            }
        }


        /// <summary>
        /// 点击获取用户码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetUserCode_Click(object sender, RoutedEventArgs e)
        {
            // Module.GetNowTime();
            Protocol.UdpSend.flagSend = (byte)Protocol.ModbusUdp.MBFunctionCode.GetId;
            Protocol.UdpConnect.clickRegisterFlag = true;
            change(txtUserCode);

        }

        /// <summary>
        /// 将用户码与随机数显示在text框上
        /// </summary>
        /// <param name="text1">显示在哪个文本框</param>
        public static void change(TextBox text1)
        {
            Random rd = new Random();
            random = rd.Next(255).ToString();

            userCode = random + Protocol.UdpConnect.uuid;
            text1.Text = random + Protocol.UdpConnect.uuid;
        }

        /// <summary>
        /// md5 32位加密
        /// </summary>
        /// <param name="传入的参数值password"></param>
        /// <returns>返回经过md5加密的字符串</returns>
        public static string MD5Encrypt32(string password)
        {
            string cl = password;
            string pwd = "";
            MD5 md5 = MD5.Create(); //实例化一个md5对像
                                    // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            try
            {
                byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
                // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
                for (int i = 0; i < s.Length; i++)
                {
                    // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                    pwd = pwd + (s[i].ToString("X").Length < 2 ? "0" + s[i].ToString("X") : s[i].ToString("X"));
                    //pwd = pwd + s[i].ToString("x");
                }
            }
            catch
            {
                MessageBox.Show("Please click to get the user code");
            }
            return pwd;
        }


        private void txtUserCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            //MessageBox.Show("进入md5加密");
            //registerCode = MD5Encrypt32(MD5Encrypt32(MD5Encrypt32(userCode)));
            if (txtUserCode.Text != "")
            {
                getMd5FromFile();
            }
            //MessageBox.Show(registerCode);
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string changeRegisterCode = txtRegister.Text;        //注册码文本框的字符串包含期限信息

                string str1 = changeRegisterCode.Substring(0, 4);
                string strThousands = changeRegisterCode.Substring(4, 1);

                string str2 = changeRegisterCode.Substring(5, 6);
                string strHundreds = changeRegisterCode.Substring(11, 1);

                string str3 = changeRegisterCode.Substring(12, 8);
                string strTens = changeRegisterCode.Substring(20, 1);

                string str4 = changeRegisterCode.Substring(21, 10);
                string strUnits = changeRegisterCode.Substring(31, 1);

                string strAdd = changeRegisterCode.Substring(32, 2);
                string str5 = changeRegisterCode.Substring(34, 4);

                //得到最终的注册码
                string SHUQEE = str1 + str2 + str3 + str4 + str5;
                //注册码的期限信息
                int dateAdd = Int32.Parse(strThousands) + Int32.Parse(strHundreds) + Int32.Parse(strTens) + Int32.Parse(strUnits);

                if (dateAdd != 36) //转换成实际的期限
                {
                    //Module.deadlineOrPermanent = 0x4;
                    int totalDays = 1000 * (Int32.Parse(strThousands)) + 100 * (Int32.Parse(strHundreds)) + 10 * (Int32.Parse(strTens)) + Int32.Parse(strUnits);
                    Module.deadlineYY = (byte)(totalDays / 365);
                    int remainDays = totalDays % 365;
                    Module.deadlineMM = (byte)(remainDays / 30);
                    Module.deadlineDD = (byte)(remainDays % 30);

                    Module.deadlineYY += Module.YY;
                    Module.deadlineMM += Module.MM;
                    Module.deadlineDD += Module.DD;

                    if (Module.deadlineDD > 30)
                    {
                        Module.deadlineMM += 1;
                        Module.deadlineDD -= 30;
                    }
                    if (Module.deadlineMM > 12)
                    {
                        Module.deadlineMM -= 12;
                        Module.deadlineYY += 1;
                    }
                }
                else     //永久码
                {
                    Module.deadlineYY = 60;
                    Module.deadlineMM = 1;
                    Module.deadlineDD = 1;
                }

                if (SHUQEE == registerCode && dateAdd == Int32.Parse(strAdd))
                {
                    MessageBox.Show(registerSuccessTips);
                    Protocol.UdpConnect.isRegistered = true;
                    Protocol.UdpConnect.clickRegisterFlag = false;
                    //UdpSend.flagSend = (byte)Mcu.ModbusUdp.MBFunctionCode.GetId;

                    File.WriteAllText(Directory.GetCurrentDirectory() + @"\shuqee.bin", Protocol.UdpConnect.uuid);
                    Protocol.UdpSend.flagSend = (byte)Protocol.ModbusUdp.MBFunctionCode.WriteChip;
                    //Module.GetChipId();
                    this.Hide();
                    //File.WriteAllBytes(@"C: \Users\shuqee\Desktop\shuqee.bin", Module.chipID);
                }
                else
                {
                    MessageBox.Show(registerFaileTips);
                }
            }
            catch
            {
                MessageBox.Show(registerAbnormalTips);
            }
            //textRegisterCode.Text = RegisterCode;
            //myClass.sendCheckData();
        }

        private void getMd5FromFile()
        {
            md5Data = loadDllFile("_fnMD5@8", 16, userCode);
            if (md5Data.Length > 0)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    registerCode = "";
                    for (int i = 0; i < md5Data.Length; i++)
                    {
                        // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符             
                        //pwd = pwd + s[i].ToString("X");
                        //利用三元表达式判断，字符串长度小于2，前面加上0，保证输出的所有码的长度都为32
                        registerCode = registerCode + (md5Data[i].ToString("X").Length < 2 ? "0" + md5Data[i].ToString("X") : md5Data[i].ToString("X"));
                        //pwd = pwd + s[i].ToString("x");
                    }
                }));
            }
        }

        /// <summary>
        /// 加载dll文件，并返回数据
        /// </summary>
        /// <param name="funcname">调用dll的函数名</param>
        /// <param name="k">自定义数组长度</param>
        /// <param name="userCode">传入的数据</param>
        /// <returns></returns>
        private byte[] loadDllFile(string funcname, int k, string userCode)
        {
            //int hModule = DllInvoke.LoadLibrary(@"D:\git\wpfPlayerUI\Wpf实例\bin\Release\USBJOY_DLL.dll");
            //IntPtr intPtr = DllInvoke.GetProcAddress(hModule, "GET_2DOF");
            //1. 动态加载C++ Dll
            int hModule = DllInvoke.LoadLibrary(@"md5.dll");
            //if (hModule == 0) return null;
            //2. 读取函数指针SQ_GET_2DOF
            IntPtr intPtr = DllInvoke.GetProcAddress(hModule, funcname);
            //3. 将函数指针封装成委托
            PSQ_GET_DATA addFunction = (PSQ_GET_DATA)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(PSQ_GET_DATA));
            //4. 测试
            byte[] aa = new byte[k];
            // this.Dispatcher.Invoke(new Action(() => {}));使用同步的方法来使用函数委托
            this.Dispatcher.Invoke(new Action(() => { addFunction(userCode, ref aa[0]); }));
            //DllInvoke.FreeLibrary(hModule);//释放Dll文件
            return aa;
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Win32;
using MoviePlayer.Class;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Timers;


namespace MoviePlayer.Class
{
     /// <summary>
     /// 自定义公共类
     /// </summary>
     public  class  UserControlClass
    {
       
        /// <summary>
        /// 登录的用户名
        /// </summary>
        public static string username="";
        /// <summary>
        /// 播放器实体类
        /// </summary>
        public static MediaPlayer MPPlayer = new MediaPlayer();
        /// <summary>
        /// 播放器状态
        /// </summary>
        public static MediaStatus MSStatus;
        /// <summary>
        /// Player的实例
        /// </summary>
       // public static Player pp = new Player();
        /// <summary>
        /// SecondScreen的实例
        /// </summary>
        public static SecondScreen sc2 = new SecondScreen();
        /// <summary>
        /// 当前播放文件名
        /// </summary>
        public static string FileName;

             
        /// <summary>
        /// 当前显示器屏幕无边框最大化
        /// </summary>
        /// <param name="the"></param>
        public static void NullBorderWin(Window the) {
            var handle = new WindowInteropHelper(the).Handle;
            //获取当前显示器屏幕
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromHandle(handle);
            //调整窗口最大化
            the.Left = 0;
            the.Top = 0;
            the.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            the.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
        }

        public static void NullBorderWin(Window the,int a,int b)
        {
            var handle = new WindowInteropHelper(the).Handle;
            //获取当前显示器屏幕
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromHandle(handle);
            //调整窗口最大化
            the.Left = 0;
            the.Top = 0;
            //UserControlClass.sc2.FInkCanvas_Player.Width = a;
            //UserControlClass.sc2.FInkCanvas_Player.Height = b;
            the.Width = screen.Bounds.Width;
            the.Height = screen.Bounds.Height;            
        }

        public static void NullBorderWin(Window the, int a, int b,int c,int d)
        {
            var handle = new WindowInteropHelper(the).Handle;
            //获取当前显示器屏幕
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromHandle(handle);
            //调整窗口最大化
            the.Left = a;
            the.Top = b;
            //UserControlClass.sc2.FInkCanvas_Player.Width = c;
            //UserControlClass.sc2.FInkCanvas_Player.Height = d;
            the.Width = c;
            the.Height = d;
        }
        /// <summary>
        /// 开机自动启动
        /// </summary>
        /// <param name="started">设置开机启动，或取消开机启动</param>
        /// <param name="exeName">注册表中的名称</param>
        /// <returns>开启或停用是否成功</returns>
        public static void RegRun(string appName, bool f)
        {
            RegistryKey HKCU = Registry.CurrentUser;
            // RegistryKey Run = HKCU.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            RegistryKey Run = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//打开注册表子项
            bool b = false;
            foreach (string i in Run.GetValueNames())
            {
                if (i == appName)
                {
                    b = true;
                    break;
                }
            }
            try
            {
                if (f)
                {
                    Run.SetValue(appName, System.Windows.Forms.Application.ExecutablePath);
                }
                else
                {
                    Run.DeleteValue(appName);
                }
            }
            catch
            {
            }
            HKCU.Close();
        }

    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MoviePlayer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        System.Threading.Mutex mutex;
        bool sysLang;
        public App()
        {
            //在异常由应用程序引发但未进行处理时发生。主要指的是UI线程。
           // this.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            //  当某个异常未被捕获时出现。主要指的是非UI线程
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            sysLang = IsChineseSimple();
            this.Startup += new StartupEventHandler(App_StartUp);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //可以记录日志并转向错误bug窗口友好提示用户
            if (e.ExceptionObject is System.Exception)
            {
                Exception ex = (System.Exception)e.ExceptionObject;
                //MessageBox.Show(ex.Message);
                Module.WriteLogFile(ex.Message);
            }
        }
        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //可以记录日志并转向错误bug窗口友好提示用户
            e.Handled = true;
            //MessageBox.Show("Error:" + e.Exception.Message);
            Module.WriteLogFile("消息:" + e.Exception.Message + "\r\n" + e.Exception.StackTrace);
        }

        void App_StartUp(object sender, StartupEventArgs e)
        {
            System.Threading.Thread.Sleep(500);
            bool ret;
            mutex = new System.Threading.Mutex(true, "MoviePlayer", out ret);
            if (!ret)
            {
                if (sysLang == true)
                {
                    MessageBox.Show("已有一个程序实例运行");
                }
                else
                {
                    MessageBox.Show("There is already a piece of software running");
                }
                Environment.Exit(0);
            }
        }

        public static bool IsChineseSimple()
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.Name == "zh-CN";
        }

        public static bool IsEnglish()
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.Name == "en-US";
        }
    }
}


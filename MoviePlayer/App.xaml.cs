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
            sysLang = IsChineseSimple();
            this.Startup += new StartupEventHandler(App_StartUp);
        }

        void App_StartUp(object sender, StartupEventArgs e)
        {
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


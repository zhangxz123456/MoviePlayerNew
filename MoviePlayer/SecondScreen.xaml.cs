using AxShockwaveFlashObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MoviePlayer.Class;

namespace MoviePlayer
{
    /// <summary>
    /// SecondScreen.xaml 的交互逻辑
    /// </summary>
    public partial class SecondScreen : Window
    {
        private MainWindow player ;
        /// <summary>
        /// 播放flash文件
        /// </summary>
        public AxShockwaveFlash FlashShock;
        /// <summary>
        /// 加载flash文件【AxShockwaveFlash】的类
        /// </summary>
        public WindowsFormsHost FlashFormHost;
        /// <summary>
        /// 显示器的数组
        /// </summary>
        public System.Windows.Forms.Screen[] sc;
        /// <summary>
        /// 为Player提供一个实例
        /// </summary>
        public MainWindow Player
        {
            get
            {
                return player;
            }
            set
            {
                player = value;
            }
        }
        public SecondScreen()
        {
            InitializeComponent();
            //全屏操作
            fullscreen();
        }
        /// <summary>
        /// 窗体最大化
        /// </summary>
        public void fullscreen()
        {
            //变成无边窗体
            this.WindowState = WindowState.Normal;//假如已经是Maximized，就不能进入全屏，所以这里先调整状态
            //this.WindowStyle = WindowStyle.None;
            //this.ResizeMode = ResizeMode.NoResize;
            //this.Topmost = true;//最大化后总是在最上面
            sc = System.Windows.Forms.Screen.AllScreens;//获取当前所有显示器的数组
            //如果有多个屏幕，就定义secondwindow的位置，让他在第二个屏幕全屏显示
            if (sc.Length > 1)
            {
                this.WindowStartupLocation = WindowStartupLocation.Manual;
                var workingArea = sc[1].WorkingArea;
                this.Left = workingArea.Left;
                this.Top = workingArea.Top;
                this.Width = workingArea.Width;
                this.Height = workingArea.Height;
                //this.Width = UserControlClass.MPPlayer.NaturalVideoWidth;
                //this.Width = UserControlClass.MPPlayer.NaturalVideoHeight;
                //FInkCanvas_Player.Width= workingArea.Width;
                //FInkCanvas_Player.Height = workingArea.Width * 9 / 16;
                // If window isn't loaded then maxmizing will result in the window displaying on the primary monitor
            }
            else
            {
                UserControlClass.NullBorderWin(this);
                //this.Width = screen.Bounds.Width;
                //this.Height = screen.Bounds.Height;
                //FInkCanvas_Player.Width = .Bounds.Width;
                //FInkCanvas_Player.Height = screen.Bounds.Width * 9 / 16;
                //this.WindowState = WindowState.Maximized;
            }
        }
        /// <summary>
        /// 单击鼠标左键暂停/播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (UserControlClass.MSStatus == MediaStatus.Play)
            {
               UserControlClass.MSStatus = MediaStatus.Pause;          
               UserControlClass.MPPlayer.Pause();
            }
            else
            {
               UserControlClass.MSStatus = MediaStatus.Play;               
               UserControlClass.MPPlayer.Play();
            }
        }

        /// <summary>
        /// Esc键退出全屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)//Esc键  
            {
                UserControlClass.MSStatus = MediaStatus.Pause;
                UserControlClass.MPPlayer.Pause();
                this.WindowState = WindowState.Minimized;
                player.Activate();
                player.WindowState = WindowState.Normal;
                //聚焦到player里面的Listview中
                if (UserControlClass.FileName != null)
                {
                    Player.ListView.SelectedValue = UserControlClass.FileName;
                }
            }
        }
        /// <summary>
        /// 双击退出全屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {            
            //UserControlClass.MSStatus = MediaStatus.Pause;
            //UserControlClass.MPPlayer.Pause();
            this.WindowState = WindowState.Minimized;
            player.Activate();
            player.WindowState = WindowState.Normal;
            if (UserControlClass.FileName != null)
            {
                Player.ListView.SelectedValue = UserControlClass.FileName;
            }
        }
        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            //相当于Player的Imgstop按键
            if (player != null)
            {
                UserControlClass.MSStatus = MediaStatus.Pause;
                player.ChangeShowPlay();
                UserControlClass.MPPlayer.Close();
                Player.sliderTime.Value = 0;
                UserControlClass.FileName = null;
                player.txtTime.Text = "";
                player.Activate();
                player.WindowState = WindowState.Normal;
            }
        }
        /// <summary>
        /// 鼠标右键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //UserControlClass.MSStatus = MediaStatus.Pause;
            //UserControlClass.MPPlayer.Pause();
            this.WindowState = WindowState.Minimized;
            Player.Activate();
            Player.WindowState = WindowState.Normal;
            //聚焦到player里面的Listview中
            if (UserControlClass.FileName != null)
            {
                player.ListView.SelectedValue = UserControlClass.FileName;  
            }
        }

        public Window returnWinSc2()
        {
            return this;
        }

        
    }
} 

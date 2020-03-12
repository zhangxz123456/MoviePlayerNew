using MoviePlayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MoviePlayer.Protocol;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Diagnostics;
using System.IO;
using System.Xml;
using MoviePlayer.Class;
using System.Timers;
using System.Xml.XPath;
using System.Windows.Interop;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;

namespace MoviePlayer
{
    /// <summary>
    /// 播放器的声音状态
    /// </summary>
    public enum SoundStatus
    {
        /// <summary>
        /// 声音
        /// </summary>
        Sound,
        /// <summary>
        /// 静音
        /// </summary>
        Mute
    }
    ///<summary>
    ///播放器的播放状态
    ///</summary>
    ///
    public enum MediaStatus
    {
        PlayInit,
        /// <summary>
        ///播放
        /// </summary>
        Play,
        /// <summary>
        ///暂停
        /// </summary>
        Pause
    }
    /// <summary>
    /// 视频\摄像屏幕显示与隐藏
    /// </summary>
    public enum PlayCamera
    {
        /// <summary>
        /// 视频屏幕
        /// </summary>
        inkMediaPlay,
        /// <summary>
        /// 摄像屏幕
        /// </summary>
        inkCameraPlay
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 变量
        private int tabSetFlag = 0;  //导航栏是否隐藏标志位
        public static ObservableCollection<Member> memberData;
        DispatcherTimer timerJudge = null;
        DispatcherTimer timerDebug = null;
        DispatcherTimer timerFilm = null;             //监控排片时间与当前时间
        Button[] btnNum;                              //缸
        Button[] btnEvEffect;                        //环境特效  调试界面
        Button[] btnChairEffect;                     //座椅特效
        CheckBox[] checkBoxEvEffect;                //环境特效  数据显示界面
        CheckBox[] checkBoxChairEffect;              //座椅特效
        CheckBox[] checkBoxProjector;
        TextBox[] textBoxIPProjector;
        TextBox[] textBoxPortProjector;

        public static string playerPath;
        public static string PlayType;               //播放器类型 4DM为4DM播放器 5D为5D播放器
        public static string PlayLanguage;           //播放器语言 EN为英文播放器 CN为中文播放器   
        public static string PlayDOF;                //自由度类型 2DOF为两自由度播放器 3DOF为三自由度播放器
        public static double PlayHeight;             //高度数据  100为原始数据 90为百分90行程数据
        public static string PlayProjector;          //设置播放画面显示在主屏还是副屏  参数分别为0或1
        public static string PlayPermission;         //用户权限 TRUE为管理员模式 FALSE为用户模式
        public static string PlayControl;            //控制功能 SERVER为TCP服务器 CLIENT为客户端
        public static string PlayControlServer;
        public static string PlayControlClient;
        public static int PlayFrame;                 //帧数控制
        public static string Play5DPicture;          //设置5D画面是否显示 参数为0或1 0显示 1不显示
        public static string PlayMac;                //绑定中控板mac地址 TRUE为绑定 FALSE为不绑定 
        public static string PlayProjector1IP;
        public static string PlayProjector1Port;
        public static string PlayProjector2IP;
        public static string PlayProjector2Port;
        public static string PlayProjector3IP;
        public static string PlayProjector3Port;
        public static string PlayProjector4IP;
        public static string PlayProjector4Port;
        public static string PlayProjector5IP;
        public static string PlayProjector5Port;
        public static string PlayProjector6IP;
        public static string PlayProjector6Port;
        public static string PlayProjector7IP;
        public static string PlayProjector7Port;
        public static string PlayProjector8IP;
        public static string PlayProjector8Port;
        public static string PlayProjectorBrand;
        public static string DelayIP;
        public static string DelayPort;
        public static string FuseIP;
        public static string FusePort;
        public static string FuseProtocol;

        private int isReset;                         //验证软件正常打开后发复位指令（只发第一次）
        private bool isSleep;
        int movieListIndex;                          //排片列表序号
        int movieListIndexs;                         //总共排片列表数 

        private string curPlayType;
        private string curPlayDOF;
        private string curPlayProjector;
        private string curPermission;
        private string curControl;
        private string curControlServer;
        private string curControlClient;
        private string curPlayFrame;
        private string cur5DPicture;
        private string curMac;

        byte[] dataNum = new byte[6];
        public static byte[] dataEvEffect = new byte[8];
        byte[] dataChairEffect = new byte[8];
        byte dataFrame;   //频率
        byte dataRate;    //幅度
        public static bool isLogin;
        //用于客户端的Socket 控制投影机
        Socket tcpClient;
        //用于服务器通信的Socket
        Socket socketSend;
        //用于监听的SOCKET
        Socket socketWatch;
        //用于客户端控制融合软件
        Socket tcpControlClient;
        //用于客户端控制继电器控制器
        Socket tcpRelayControlClient;

        //创建监听连接的线程
        Thread AcceptSocketThread;
        //接收客户端发送消息的线程
        Thread threadReceive;

        public class Member : INotifyPropertyChanged
        {
            string _movieNo;
            public string MovieNo
            {
                get { return _movieNo; }
                set { _movieNo = value; OnPropertyChanged("MovieNo"); }
            }

            string _start;

            /// <summary>
            /// 排片列表影片开始时间
            /// </summary>
            public string Start
            {
                get { return _start; }
                set { _start = value; OnPropertyChanged("Start"); }
            }

            string _end;

            /// <summary>
            /// 排片列表结束时间
            /// </summary>
            public string End
            {
                get { return _end; }
                set { _end = value; OnPropertyChanged("End"); }
            }

            string _movieName;

            /// <summary>
            /// 排片列表中电影文件夹名
            /// </summary>
            public string MovieName
            {
                get { return _movieName; }
                set { _movieName = value; OnPropertyChanged("MovieName"); }
            }

            string _fullMovieName;

            /// <summary>
            /// 电影文件夹全路径
            /// </summary>
            public string FullMovieName
            {
                get { return _fullMovieName; }
                set { _fullMovieName = value; OnPropertyChanged("FullMovieName"); }
            }

            //public int Age { get; set; }
            protected internal virtual void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            public event PropertyChangedEventHandler PropertyChanged;

        }

        #region   播放变量声明

        /// <summary>
        /// 列表-名称
        /// </summary>
        private string ListNodeName = "";
        private string[] ListNodeNameArr;
        /// <summary>
        /// 列表-路径
        /// </summary>
        private string ListNodePath = "";
        private string[] ListNodePathArr;
        /// <summary>
        /// 列表-长度
        /// </summary>
        private string ListNodeLength = "";
        /// <summary>
        /// 列表-播放时刻
        /// </summary>
        private string ListNodeTime = "";
        /// <summary>
        /// 播放模式选择
        /// </summary>
        private string ModePlayTag;


        /// <summary>
        /// 灯光模式选择
        /// </summary>
        private string ModeLightTag;

        /// <summary>
        /// 选中的影片的全路径名
        /// </summary>
        public static string fullPathName = "";
        /// <summary>
        /// 播放文件序号（上一部，下一部）
        /// </summary>
        private int number = 0;
        ///<summary>
        ///视频\摄像屏幕
        /// </summary>
        private PlayCamera PCink = PlayCamera.inkMediaPlay;
        ///<summary>
        ///播放器声音状态
        /// </summary>
        private SoundStatus SSStatus = SoundStatus.Sound;
        /// <summary>
        /// 播放器声音数值
        /// </summary>
        double sound = 0;
        /// <summary>
        /// 播放视频的总长时间
        /// </summary>
        private string MediaCountTime = string.Empty;
        /// <summary>
        /// 播放时间的响应的委托
        /// </summary>
        private delegate void DeleSetPosition();
        /// <summary>
        /// 用于定时计算，添加播放时间
        /// </summary>
        private System.Timers.Timer Timing = new System.Timers.Timer();
        /// <summary>
        /// 播放文件的总长时间
        /// </summary>
        private int MaxLen = 0;
        ///// <summary>
        ///// 播放flash文件
        ///// </summary>
        //public AxShockwaveFlash FlashShock;
        /// <summary>
        /// 加载flash文件【AxShockwaveFlash】的类
        /// </summary>
        //public WindowsFormsHost FlashFormHost;
        /// <summary>
        /// 初始化视频文件名【首次打开程序时使用】
        /// </summary>
        private object InitPlayerPath = System.Windows.Application.Current.Resources["InitArgs"] ?? null;
        /// <summary>
        /// 标识当前的时间滑动条是否可以操作
        /// </summary>
        private bool IsChangeValue = false;
        ///// <summary>
        ///// 摄像头调用类
        ///// </summary>
        //public CapPlayer CameraPlayer;
        /// <summary>
        /// 系统时间显示
        /// </summary>
        private DispatcherTimer DTimer;
        /// <summary>
        /// 标识是否记忆播放
        /// </summary>
        private bool memoryPlay = true;

        ///// <summary>
        ///// 第二个窗体
        ///// </summary>
        #endregion
        #endregion


        UdpInit myUdpInit = new UdpInit();
        public bool timerStart;
        int count;
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern int SystemParametersInfo(
          int uAction,
          int uParam,
          string lpvParam,
          int fuWinIni
     );

        #region 构造函数
        public MainWindow()
        {
            InitializeComponent();      
            GetPlayerPath();
            Module.GetNowTime();
            Module.readMacFile();
            Module.readUuidFile();          
            SystemParametersInfo(20, 0, Directory.GetCurrentDirectory() + @"\shuqee.bmp", 0x2);
            ReadType();
            ChangeLanguage(PlayLanguage);
            changeWinVersionLanguage();
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
            ControlRegister();
            TimerJudgeInit();
            UserControlClass.MPPlayer.MediaEnded += new EventHandler(MPPlayer_MediaEnded);
            UserControlClass.MPPlayer.MediaOpened += new EventHandler(MPPlayer_MediaOpened);
            Timing.Elapsed += new ElapsedEventHandler(Tim_Elapsed);
            ChangeShowPlay();
            ChangeshowInk();
            addMember();
            ReadFilmList();
            ReadMode();
            ReadVolume();
            SelectXml();
            SelectMode();
            NewLoaded();
            TypeShow();
            MenuModePlayTick();
            myUdpInit.udpInit();
            MyServer();
            UDPSocketClientInit();
            SelectXmlMovie();
            //TcpRelayControlClientInit();
        }

        private void TcpRelayControlClientInit()
        {
            //Thread thRelay = new Thread(() =>
            //{
            //    bool a = TcpRelayControlClientConnect(DelayIP, DelayPort);
            //    if (a)
            //    {
            //        this.Dispatcher.Invoke(new Action(() =>
            //        {
                        RelayControlInitSend();
            //        }));
            //    }
            //});
            //thRelay.Start();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            //SaveVolume();
            //SaveFilmList();
            //SaveFilmPlayList();
            RelayControlCloseSend();
            UdpSend.SendZero();
            CloseTCPServer();
            //System.Windows.Application.Current.Shutdown();
            //Environment.Exit(0);
            //Process.GetCurrentProcess().Kill();
            System.Environment.Exit(0);
        }

        #endregion

        #region   方法

        /// <summary>
        /// 获取软件当前路径
        /// </summary>
        private void GetPlayerPath()
        {
            playerPath = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 5);
        }

        /// <summary>
        /// 控件注册与初始化
        /// </summary>
        private void ControlRegister()
        {
            rb1.Checked += Rb_Checked;
            rb2.Checked += Rb_Checked;
            rb3.Checked += Rb_Checked;
            rb4.Checked += Rb_Checked;
            rb5.Checked += Rb_Checked;
            rb6.Checked += Rb_Checked;

            checkProjectorAll.Click += CheckProjector_Click;
            checkProjector1.Click += Projector_Click;
            checkProjector2.Click += Projector_Click;
            checkProjector3.Click += Projector_Click;
            checkProjector4.Click += Projector_Click;
            checkProjector5.Click += Projector_Click;
            checkProjector6.Click += Projector_Click;
            checkProjector7.Click += Projector_Click;
            checkProjector8.Click += Projector_Click;

            checkFuse.Click += CheckFuse_Click;

            cbDoor.Click += CbDoor_Click;
            cbLightning.Click += CbLightning_Click;
            cbBoosterPump.Click += CbBoosterPump_Click;
            cbRadiotube.Click += CbRadiotube_Click;

            rb1.Click += Rb_Click;
            rb2.Click += Rb_Click;
            rb3.Click += Rb_Click;
            rb4.Click += Rb_Click;
            rb5.Click += Rb_Click;
            rb6.Click += Rb_Click;

            tabControlShow.MouseDown += TabControlShow_MouseDown;

            btnLtcMode.Click += BtnMode_Click;
            btnLocalMode.Click += BtnMode_Click;

            btnRegister.Click += BtnAbout_Click;
            btnUpdateLog.Click += BtnAbout_Click;
            btnAboutUs.Click += BtnAbout_Click;

            btnLang.Click += BtnSet_Click;
            btnParamSet.Click += BtnSet_Click;
            btnProjectorSet.Click += BtnSet_Click;
            btnDelaySet.Click += BtnSet_Click;

            btnImgBack.MouseEnter += BtnImgBack_MouseEnter;
            btnImgBack.MouseLeave += BtnImgBack_MouseLeave;
            btnImgNext.MouseEnter += BtnImgBack_MouseEnter;
            btnImgNext.MouseLeave += BtnImgBack_MouseLeave;
            btnImgPlay.MouseEnter += BtnImgBack_MouseEnter;
            btnImgPlay.MouseLeave += BtnImgBack_MouseLeave;
            btnImgStop.MouseEnter += BtnImgBack_MouseEnter;
            btnImgStop.MouseLeave += BtnImgBack_MouseLeave;
            btnImgVadd.MouseEnter += BtnImgBack_MouseEnter;
            btnImgVadd.MouseLeave += BtnImgBack_MouseLeave;
            btnImgVReduce.MouseEnter += BtnImgBack_MouseEnter;
            btnImgVReduce.MouseLeave += BtnImgBack_MouseLeave;

            btnNum1.Click += BtnNum_Click;
            btnNum2.Click += BtnNum_Click;
            btnNum3.Click += BtnNum_Click;
            btnNum4.Click += BtnNum_Click;
            btnNum5.Click += BtnNum_Click;
            btnNum6.Click += BtnNum_Click;

            btnLightning.Click += BtnEvEffect_Click;
            btnWind.Click += BtnEvEffect_Click;
            btnBubble.Click += BtnEvEffect_Click;
            btnFog.Click += BtnEvEffect_Click;
            btnFire.Click += BtnEvEffect_Click;
            btnSnow.Click += BtnEvEffect_Click;
            btnLaser.Click += BtnEvEffect_Click;
            btnRain.Click += BtnEvEffect_Click;

            btnCA.Click += BtnChairEffect_Click;
            btnCB.Click += BtnChairEffect_Click;
            btnSmell.Click += BtnChairEffect_Click;
            btnVibration.Click += BtnChairEffect_Click;
            btnSweepLeg.Click += BtnChairEffect_Click;
            btnSprayWater.Click += BtnChairEffect_Click;
            btnSprayAir.Click += BtnChairEffect_Click;
            btnPushBack.Click += BtnChairEffect_Click;

            //btnBackUp1.Click += BtnBackUp_Click;
            //btnBackUp2.Click += BtnBackUp2_Click;
            //btnBackUp3.Click += BtnBackUp3_Click;
            //btnBackUp4.Click += BtnBackUp4_Click;

            btnConfirmProjector.Click += BtnConfirmProjector_Click;
            btnDefaultProjector.Click += BtnDefaultProjector_Click;
            btnConfirmDelay.Click += BtnConfirmDelay_Click;
            btnConfirmFuse.Click += BtnConfirmFuse_Click;
            btnConfirm.Click += BtnConfirm_Click;
            btnDefault.Click += BtnDefault_Click;
            btnEN.Click += BtnSetLang_Click;
            btnCN.Click += BtnSetLang_Click;

            btn2DOF.Click += BtnDOF_Click;
            btn3DOF.Click += BtnDOF_Click;
            btn6DOF.Click += BtnDOF_Click;

            btnFrame48.Click += BtnFrame_Click;
            btnFrame60.Click += BtnFrame_Click;
            btnFrame120.Click += BtnFrame_Click;

            btn4DM.Click += BtnType_Click;
            btn5D.Click += BtnType_Click;

            btnMain.Click += BtnScreen_Click;
            btnSecond.Click += BtnScreen_Click;

            btnUser.Click += BtnPermission_Click;
            btnAdmin.Click += BtnPermission_Click;

            btnServer.Click += BtnControl_Click;
            btnClient.Click += BtnControl_Click;

            btnShow.Click += Btn5DPicture_Click;
            btnNotShow.Click += Btn5DPicture_Click;

            btnMac.Click += BtnMac_Click;

            listMenuAdd.Click += ListMenu_Click;
            listMenuPlay.Click += ListMenu_Click;
            listMenuDel.Click += ListMenu_Click;
            listMenuDelAll.Click += ListMenu_Click;

            listModeSingle.Click += ListModeChoose_Click;
            listModeDefault.Click += ListModeChoose_Click;
            listModeLoop.Click += ListModeChoose_Click;

            btnNum = new Button[6] { btnNum1, btnNum2, btnNum3, btnNum4, btnNum5, btnNum6 };
            btnEvEffect = new Button[8] { btnLightning, btnWind, btnBubble, btnFog, btnFire, btnSnow, btnLaser, btnRain };
            btnChairEffect = new Button[8] { btnCA, btnCB, btnSmell, btnVibration, btnSweepLeg, btnSprayWater, btnSprayAir, btnPushBack };

            checkBoxEvEffect = new CheckBox[8] { cbEv1, cbEv2, cbEv3, cbEv4, cbEv5, cbEv6, cbEv7, cbEv8 };
            checkBoxChairEffect = new CheckBox[8] { cbCv7, cbCv8, cbCv1, cbCv2, cbCv3, cbCv4, cbCv5, cbCv6 };
            checkBoxProjector = new CheckBox[8] { checkProjector1,checkProjector2,checkProjector3,checkProjector4,checkProjector5,checkProjector6,checkProjector7,checkProjector8};
            textBoxIPProjector = new TextBox[8] { txtIpProjector1,txtIpProjector2,txtIpProjector3,txtIpProjector4,txtIpProjector5,txtIpProjector6,txtIpProjector7,txtIpProjector8};
            textBoxPortProjector = new TextBox[8] { txtPortProjector1,txtPortProjector2,txtPortProjector3,txtPortProjector4,txtPortProjector5,txtPortProjector6,txtPortProjector7,txtPortProjector8};
        }

        //private void BtnBackUp4_Click(object sender, RoutedEventArgs e)
        //{
        //    if (btnBackUp4.Opacity == 1)
        //    {
        //        btnBackUp4.Opacity = 0.9;
        //        btnBackUp4.Background = Brushes.Cyan;
        //        dataEvEffect[0] = 1;
        //        DMXLightning[0] = 5;
        //        DMXLightning[1] = 255;
        //        DMXLightning[2] = 255;
        //        DMXLightning[3] = 0;
        //        DMXLightning[4] = 0;
        //        DMXLightning[5] = 0;
        //        DMXLightning[6] = 0;
        //        DMXLightning[7] = 255;
        //    }
        //    else
        //    {
        //        dataEvEffect[0] = 0;
        //        Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x93, 0x93, 0x93));
        //        btnBackUp4.Opacity = 1;
        //        btnBackUp4.Background = brush;
        //        DMXLightning = new byte[10];
        //    }
        //}

        //private void BtnBackUp3_Click(object sender, RoutedEventArgs e)
        //{
        //    if (btnBackUp3.Opacity == 1)
        //    {
        //        btnBackUp3.Opacity = 0.9;
        //        btnBackUp3.Background = Brushes.Cyan;
        //        dataEvEffect[0] = 1;
        //        DMXLightning[0] = 5;
        //        DMXLightning[1] = 255;
        //        DMXLightning[2] = 255;
        //        DMXLightning[3] = 0;
        //        DMXLightning[4] = 0;
        //        DMXLightning[5] = 0;
        //        DMXLightning[6] = 255;
        //        DMXLightning[7] = 0;
        //    }
        //    else
        //    {
        //        dataEvEffect[0] = 0;
        //        Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x93, 0x93, 0x93));
        //        btnBackUp3.Opacity = 1;
        //        btnBackUp3.Background = brush;
        //        DMXLightning = new byte[10];
        //    }
        //}

        //private void BtnBackUp2_Click(object sender, RoutedEventArgs e)
        //{
        //    if (btnBackUp2.Opacity == 1)
        //    {
        //        btnBackUp2.Opacity = 0.9;
        //        btnBackUp2.Background = Brushes.Cyan;
        //        dataEvEffect[0] = 1;
        //        DMXLightning[0] = 5;
        //        DMXLightning[1] = 255;
        //        DMXLightning[2] = 255;
        //        DMXLightning[3] = 0;
        //        DMXLightning[4] = 0;
        //        DMXLightning[5] = 255;
        //        DMXLightning[6] = 0;
        //        DMXLightning[7] = 0;
        //    }
        //    else
        //    {
        //        dataEvEffect[0] = 0;
        //        Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x93, 0x93, 0x93));
        //        btnBackUp2.Opacity = 1;
        //        btnBackUp2.Background = brush;
        //        DMXLightning = new byte[10];
        //    }
        //}

        //private void BtnBackUp_Click(object sender, RoutedEventArgs e)
        //{
        //    if (btnBackUp1.Opacity == 1)
        //    {
        //        btnBackUp1.Opacity = 0.9;
        //        btnBackUp1.Background = Brushes.Cyan;
        //        dataEvEffect[0] = 1;
        //        DMXLightning[0] = 5;
        //        DMXLightning[1] = 255;
        //        DMXLightning[2] = 255;
        //        DMXLightning[3] = 0;
        //        DMXLightning[4] = 0;
        //        DMXLightning[5] = 255;
        //        DMXLightning[6] = 255;
        //        DMXLightning[7] = 100;
        //    }
        //    else
        //    {
        //        dataEvEffect[0] = 0;
        //        Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x93, 0x93, 0x93));
        //        btnBackUp1.Opacity = 1;
        //        btnBackUp1.Background = brush;
        //        DMXLightning = new byte[10];                
        //    }
        //}

        /// <summary>
        /// 选择语言文件
        /// </summary>
        /// <param name="_currentLan"></param>
        private void ChangeLanguage(String _currentLan)
        {
            ResourceDictionary dict = new ResourceDictionary();

            if (_currentLan == "EN")
            {
                dict.Source = new Uri(@"Language\en-US.xaml", UriKind.Relative);

                _currentLan = "CN";
            }
            else
            {
                dict.Source = new Uri(@"Language\zh-CN.xaml", UriKind.Relative);
                _currentLan = "EN";
            }

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);
            Application.Current.Resources.MergedDictionaries[0] = dict;
        }

        /// <summary>
        /// 软件重新启动方法
        /// </summary>
        private void RestartSoftWare()
        {
            if (PlayLanguage.Equals("CN"))
            {
                MessageBox.Show("修改成功，软件将重新启动");
            }
            else
            {
                MessageBox.Show("Modify success,the software will restart");
            }

            System.Windows.Forms.Application.Restart();
            Application.Current.Shutdown();
            
            //Thread.Sleep(1000);
            //Process p = new Process();
            //p.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + "MoviePlayer.exe";
            //p.StartInfo.UseShellExecute = false;
            //p.Start();
            //Application.Current.Shutdown();
            //Environment.Exit(0);
        }

        /// <summary>
        /// 保存参数设置数据
        /// </summary>
        private void SaveType()
        {
            string path = MainWindow.playerPath + @"\XML\" + "Type.xml";
            FileInfo finfo = new FileInfo(path);
            //if (btnServer.Background == Brushes.DodgerBlue)
            //{
            //    curControl = "SERVER";
            //}
            //if (btnClient.Background == Brushes.DodgerBlue)
            //{
            //    curControl = "CLIENT";
            //}

            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                XmlNode childNodes = xmlDoc.SelectSingleNode("Type");
                XmlElement element = (XmlElement)childNodes;
                element["DOF"].InnerText = curPlayDOF;
                element["Style"].InnerText = curPlayType;
                element["Projector"].InnerText = curPlayProjector;
                element["Height"].InnerText = txtHeight.Text;
                element["Permission"].InnerText = curPermission;
                //element["Control"].InnerText = curControl;
                element["Server"].InnerText = curControlServer;
                element["Client"].InnerText = curControlClient;
                element["Frame"].InnerText = curPlayFrame;
                element["PlayPicture"].InnerText = cur5DPicture;
                element["Mac"].InnerText = curMac;
                xmlDoc.Save(path);
            }
        }

        /// <summary>
        /// 保存投影机参数设置数据
        /// </summary>
        private void SaveProjector()
        {
            string path = MainWindow.playerPath + @"\XML\" + "Type.xml";
            FileInfo finfo = new FileInfo(path);
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                XmlNode childNodes = xmlDoc.SelectSingleNode("Type");
                XmlElement element1 = (XmlElement)childNodes;
                element1["ProjectorBrand"].InnerText = comboBoxBrand.Text;

                XmlNode childNodeNext = childNodes.SelectSingleNode("Projector1");
                XmlElement element = (XmlElement)childNodeNext;
                element["IP"].InnerText = txtIpProjector1.Text;
                element["Port"].InnerText = txtPortProjector1.Text;

                childNodeNext = childNodes.SelectSingleNode("Projector2");
                element = (XmlElement)childNodeNext;
                element["IP"].InnerText = txtIpProjector2.Text;
                element["Port"].InnerText = txtPortProjector2.Text;

                childNodeNext = childNodes.SelectSingleNode("Projector3");
                element = (XmlElement)childNodeNext;
                element["IP"].InnerText = txtIpProjector3.Text;
                element["Port"].InnerText = txtPortProjector3.Text;

                childNodeNext = childNodes.SelectSingleNode("Projector4");
                element = (XmlElement)childNodeNext;
                element["IP"].InnerText = txtIpProjector4.Text;
                element["Port"].InnerText = txtPortProjector4.Text;

                childNodeNext = childNodes.SelectSingleNode("Projector5");
                element = (XmlElement)childNodeNext;
                element["IP"].InnerText = txtIpProjector5.Text;
                element["Port"].InnerText = txtPortProjector5.Text;

                childNodeNext = childNodes.SelectSingleNode("Projector6");
                element = (XmlElement)childNodeNext;
                element["IP"].InnerText = txtIpProjector6.Text;
                element["Port"].InnerText = txtPortProjector6.Text;

                childNodeNext = childNodes.SelectSingleNode("Projector7");
                element = (XmlElement)childNodeNext;
                element["IP"].InnerText = txtIpProjector7.Text;
                element["Port"].InnerText = txtPortProjector7.Text;

                childNodeNext = childNodes.SelectSingleNode("Projector8");
                element = (XmlElement)childNodeNext;
                element["IP"].InnerText = txtIpProjector8.Text;
                element["Port"].InnerText = txtPortProjector8.Text;


                xmlDoc.Save(path);
            }
        }

        /// <summary>
        /// 保存继电器模块参数设置
        /// </summary>
        private void SaveDelayModule()
        {
            string path = MainWindow.playerPath + @"\XML\" + "Type.xml";
            FileInfo finfo = new FileInfo(path);
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                XmlNode childNodes = xmlDoc.SelectSingleNode("Type");
                XmlElement element1 = (XmlElement)childNodes;

                XmlNode childNodeNext = childNodes.SelectSingleNode("DelayModule");
                XmlElement element = (XmlElement)childNodeNext;
                element["IPDelay"].InnerText = txtIpDelay.Text;
                element["PortDelay"].InnerText = txtPortDelay.Text;
                xmlDoc.Save(path);
            }
        }

        private void SaveFuseModule()
        {
            string path = MainWindow.playerPath + @"\XML\" + "Type.xml";
            FileInfo finfo = new FileInfo(path);
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                XmlNode childNodes = xmlDoc.SelectSingleNode("Type");
                XmlElement element1 = (XmlElement)childNodes;

                XmlNode childNodeNext = childNodes.SelectSingleNode("FuseModule");
                XmlElement element = (XmlElement)childNodeNext;
                element["IPFuse"].InnerText =txtIpFuse.Text;
                element["PortFuse"].InnerText = txtPortFuse.Text;
                element["CommunicationMode"].InnerText = comboBoxNet.Text;
                xmlDoc.Save(path);
            }
        }

        /// <summary>
        /// 保存当前选择的语言状态
        /// </summary>
        private void SaveLang()
        {
            string path = MainWindow.playerPath + @"\XML\" + "Type.xml";
            FileInfo finfo = new FileInfo(path);
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                XmlNode childNodes = xmlDoc.SelectSingleNode("Type");
                XmlElement element = (XmlElement)childNodes;
                element["Language"].InnerText = PlayLanguage;
                xmlDoc.Save(path);
            }
        }

        /// <summary>
        /// 软件类型与属性状态显示
        /// </summary>
        private void TypeShow()
        {
            curPlayDOF = PlayDOF;
            curPlayProjector = PlayProjector;
            curPlayType = PlayType;
            curPermission = PlayPermission;
            curPlayFrame = PlayFrame.ToString();
            curControlClient = PlayControlClient;
            curControlServer = PlayControlServer;
            if (PlayLanguage.Equals("EN"))
            {
                btnEN.Background = Brushes.DodgerBlue;
            }
            else
            {
                btnCN.Background = Brushes.DodgerBlue;
            }

            switch (PlayDOF)
            {
                case "2DOF":
                    btn2DOF.Background = Brushes.DodgerBlue;
                    txtPbVal1.Text = "127";
                    txtPbVal3.Text = "127";
                    pb1.Value = 127;
                    pb3.Value = 127;
                    txtPbVal2.Visibility = Visibility.Hidden;
                    pb2.Visibility = Visibility.Hidden;                   
                    TypeShowAction();
                    break;
                case "3DOF":
                    btn3DOF.Background = Brushes.DodgerBlue;
                    TypeShowAction();
                    break;
                case "6DOF":
                    btn6DOF.Background = Brushes.DodgerBlue;
                    break;
            }


            if (PlayType.Equals("4DM"))
            {
                btn4DM.Background = Brushes.DodgerBlue;
            }
            else
            {
                btn5D.Background = Brushes.DodgerBlue;
            }

            if (PlayProjector.Equals("0"))
            {
                btnMain.Background = Brushes.DodgerBlue;
            }
            else
            {
                btnSecond.Background = Brushes.DodgerBlue;
            }
            if (PlayPermission.Equals("TRUE"))
            {
                btnAdmin.Background = Brushes.DodgerBlue;
            }
            else
            {
                btnUser.Background = Brushes.DodgerBlue;
            }

            Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x99, 0x99));
            //if (PlayControl.Equals("SERVER"))
            //{
            //    btnServer.Background = Brushes.DodgerBlue;
            //}
            //else if (PlayControl.Equals("CLIENT"))
            //{
            //    btnClient.Background = Brushes.DodgerBlue;
            //}
            //else
            //{
            //    btnServer.Background = brush;
            //    btnClient.Background = brush;
            //}

            if(PlayControlServer.Equals("TRUE"))
            {
                btnServer.Background = Brushes.DodgerBlue;
            }
            else
            {
                btnServer.Background = brush;
            }

            if (PlayControlClient.Equals("TRUE"))
            {
                btnClient.Background = Brushes.DodgerBlue;
            }
            else
            {
                btnClient.Background = brush;
            }

            if (Play5DPicture.Equals("0"))
            {
                btnShow.Background = Brushes.DodgerBlue;
            }
            else
            {
                btnNotShow.Background = Brushes.DodgerBlue;
            }

            if(PlayMac.Equals("TRUE"))
            {
                btnMac.Background = Brushes.DodgerBlue;
            }
            else
            {
                btnMac.Background = brush;
            }

            switch (PlayFrame)
            {
                case 48:
                    btnFrame48.Background = Brushes.DodgerBlue;
                    break;
                case 60:
                    btnFrame60.Background = Brushes.DodgerBlue;
                    break;
                case 120:
                    btnFrame120.Background = Brushes.DodgerBlue;
                    break;
            }

            txtHeight.Text = PlayHeight.ToString();
            txtIpProjector1.Text = PlayProjector1IP;
            txtPortProjector1.Text = PlayProjector1Port;
            txtIpProjector2.Text = PlayProjector2IP;
            txtPortProjector2.Text = PlayProjector2Port;
            txtIpProjector3.Text = PlayProjector3IP;
            txtPortProjector3.Text = PlayProjector3Port;
            txtIpProjector4.Text = PlayProjector4IP;
            txtPortProjector4.Text = PlayProjector4Port;
            txtIpProjector5.Text = PlayProjector5IP;
            txtPortProjector5.Text = PlayProjector5Port;
            txtIpProjector6.Text = PlayProjector6IP;
            txtPortProjector6.Text = PlayProjector6Port;
            txtIpProjector7.Text = PlayProjector7IP;
            txtPortProjector7.Text = PlayProjector7Port;
            txtIpProjector8.Text = PlayProjector8IP;
            txtPortProjector8.Text = PlayProjector8Port;
            txtIpDelay.Text = DelayIP;
            txtPortDelay.Text = DelayPort;
            txtIpFuse.Text = FuseIP;
            txtPortFuse.Text = FusePort;
            comboBoxNet.Text = FuseProtocol;
            if (PlayControlClient.Equals("TRUE"))
            {
                Brush brushOn = new SolidColorBrush(Color.FromArgb(0xff, 0x22, 0xAC, 0x38));
                cbLightning.Background = brushOn;
                cbLightning.Opacity = 0.9;
                //cbDoor.Background = brushOn;
                //cbDoor.Opacity = 0.9;
            }
            if (PlayLanguage.Equals("CN"))
            {
                comboBoxBrand.Text = PlayProjectorBrand;
                if (PlayProjectorBrand.Equals("Panasonic"))
                {
                    comboBoxBrand.Text = "松下";
                }
                if (PlayProjectorBrand.Equals("Sony"))   
                {
                    comboBoxBrand.Text = "索尼";
                }
            }
            else
            {
                comboBoxBrand.Text = PlayProjectorBrand;
                if (PlayProjectorBrand.Equals("松下"))
                {
                    comboBoxBrand.Text = "Panasonic";
                }
                if (PlayProjectorBrand.Equals("索尼"))
                {
                    comboBoxBrand.Text = "Snoy";
                }
            }
        }

        /// <summary>
        /// 动作数据栏显示切换
        /// </summary>
        private void TypeShowAction()
        {
            pb4.Visibility = Visibility.Hidden;
            pb5.Visibility = Visibility.Hidden;
            pb6.Visibility = Visibility.Hidden;
            txtPbVal4.Visibility = Visibility.Hidden;
            txtPbVal5.Visibility = Visibility.Hidden;
            txtPbVal6.Visibility = Visibility.Hidden;

            Grid.SetColumn(txtPbVal1, 0);
            Grid.SetColumnSpan(txtPbVal1, 2);
            Grid.SetColumn(txtPbVal2, 2);
            Grid.SetColumnSpan(txtPbVal2, 2);
            Grid.SetColumn(txtPbVal3, 4);
            Grid.SetColumnSpan(txtPbVal3, 2);

            Grid.SetColumn(pb1, 0);
            Grid.SetColumnSpan(pb1, 2);
            Grid.SetColumn(pb2, 2);
            Grid.SetColumnSpan(pb2, 2);
            Grid.SetColumn(pb3, 4);
            Grid.SetColumnSpan(pb3, 2);
        }

        /// <summary>
        /// 点击对应的缸改变背景颜色
        /// </summary>
        /// <param name="i"></param>
        private void changeBackground(int i)
        {
            if (btnNum[i].Opacity == 1)
            {
                btnNum[i].Opacity = 0.9;
                btnNum[i].Background = Brushes.Cyan;
                dataNum[i] = 255;
            }
            else
            {
                Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x93, 0x93, 0x93));
                btnNum[i].Opacity = 1;
                btnNum[i].Background = brush;
                dataNum[i] = 0;
            }
        }

        /// <summary>
        /// 点击对应的环境特效改变背景颜色
        /// </summary>
        /// <param name="i"></param>
        private void changeEvEffect(int i)
        {
            if (btnEvEffect[i].Opacity == 1)
            {
                btnEvEffect[i].Background = Brushes.Cyan;
                btnEvEffect[i].Opacity = 0.9;
                dataEvEffect[i] = (Byte)Math.Pow(2, i);
                if (i == 5)
                {
                    Module.DMXSnow = new byte[] { 0x6E, 0x0, 0x5A, 0x0, 0x0, 0x2D, 0x0, 0x0, 0x0, 0x0 };
                }
            }
            else
            {
                Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x93, 0x93, 0x93));
                btnEvEffect[i].Background = brush;
                btnEvEffect[i].Opacity = 1;
                dataEvEffect[i] = 0;
                if (i == 5)
                {
                    Module.DMXSnow = new byte[10];
                }
            }
        }

        /// <summary>
        /// 点击对应的座椅特效改变背景颜色
        /// </summary>
        /// <param name="i"></param>
        private void changeChairEffect(int i)
        {
            if (btnChairEffect[i].Opacity == 1)
            {
                btnChairEffect[i].Background = Brushes.Cyan;
                btnChairEffect[i].Opacity = 0.9;
                if (i == 1)
                {
                    dataFrame = 2;
                    dataRate = 40;
                }
                else
                {
                    dataChairEffect[i] = (Byte)Math.Pow(2, i);
                }
            }
            else
            {
                Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x93, 0x93, 0x93));
                btnChairEffect[i].Background = brush;
                btnChairEffect[i].Opacity = 1;
                if (i == 1)
                {
                    dataFrame = 0;
                    dataRate = 0;
                }
                else
                {
                    dataChairEffect[i] = 0;
                }
            }
        }

        /// <summary>
        /// 比较传入时间与当前系统时间
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        private bool compareDateTime(string str1, string str2)
        {
            try
            {
                DateTime dt1 = new DateTime();
                DateTime dt2 = new DateTime();
                DateTime dt3 = new DateTime();

                if (str1 != "" && str2 != "")
                {
                    dt1 = Convert.ToDateTime(str1);
                    dt2 = Convert.ToDateTime(str2);
                    dt3 = DateTime.Now;
                }
                if (str1 != "" && str2 == "")
                {
                    dt1 = Convert.ToDateTime(str1);
                    dt3 = DateTime.Now;
                    return DateTime.Compare(dt3, dt1) >= 0;
                }
                return DateTime.Compare(dt3, dt1) >= 0 && DateTime.Compare(dt3, dt2) < 0;

            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool compareDateTime(string str1)
        {
            try
            {
                DateTime dt1 = new DateTime();
                DateTime dt2 = new DateTime();
                dt1 = Convert.ToDateTime(str1);
                dt2 = DateTime.Now;
                return DateTime.Compare(dt2, dt1) < 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 4DM模式选择
        /// </summary>
        private void SelectMode()
        {
            if ("4DM".Equals(PlayType))
            {

                //if (memberData[0].Start != "" && memberData[0].End != "" && PlayControl != "SERVER")
                if (memberData[0].Start != "" && memberData[0].End != "" && PlayControlServer != "TRUE")
                {
                    TimerFilmInit();
                }
                // else if(memberData[0].MovieName != "" && PlayControl == "")
                else if (memberData[0].MovieName != "" && PlayControlServer == "FALSE")
                {
                    ReadFilmListFile(0);
                }
            }
        }

        /// <summary>
        /// 屏蔽部分按钮功能
        /// </summary>
        private void LockSoftWare()
        {
            rb1.IsEnabled = false;
            rb2.IsEnabled = false;
            rb3.IsEnabled = false;
            rb4.IsEnabled = false;
            //rb6.IsEnabled = false;
            ListView.IsEnabled = false;
            btnImgPlay.IsEnabled = false;
        }

        /// <summary>
        /// 开放按钮功能
        /// </summary>
        private void OpenSoftWare()
        {
            rb1.IsEnabled = true;
            rb2.IsEnabled = true;
            rb3.IsEnabled = true;
            rb4.IsEnabled = true;
            rb6.IsEnabled = true;
            ListView.IsEnabled = true;
            btnImgPlay.IsEnabled = true;
            if ("4DM".Equals(PlayType))
            {
                // ListView.IsEnabled = false;
                btnImgPlay.IsEnabled = false;
            }
        }

        /// <summary>
        /// 排片列表成员初始化
        /// </summary>
        private void addMember()
        {
            memberData = new ObservableCollection<Member>();
            for (int i = 0; i < 16; i++)
            {
                memberData.Add(new Member()
                {
                    MovieNo = "",
                    Start = "",
                    End = "",
                    MovieName = "",
                    FullMovieName = "",
                });
            }
            dataGrid.DataContext = memberData;
        }

        /// <summary>
        /// 检查输入的字符串是否符合要求
        /// </summary>
        /// <param name="str">要检查的字符串</param>
        /// <returns></returns>
        private string checkStrStartAndEnd(string str)
        {
            int s = str.IndexOf(':');
            try
            {
                string strHour = str.Substring(0, s);
                string strMinute = str.Substring(s + 1);
                int strHourValue = Convert.ToInt32(strHour);
                int strMinuteValue = Convert.ToInt32(strMinute);
                if (strHourValue > 24)
                {
                    System.Windows.MessageBox.Show(str + "输入有误:小时不能大于24");
                    str = "";
                }
                if (strHour.Length == 2 && strHourValue < 10)
                {
                    System.Windows.MessageBox.Show(str + "输入有误：小时前不能有0");
                    str = "";
                }
                if (strMinuteValue > 59)
                {
                    System.Windows.MessageBox.Show(str + "输入有误:分钟不能大于59");
                    str = "";
                }
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show(str + "输入有误");
                str = "";
            }

            return str;
        }

        /// <summary>
        /// 检查传入的字符串是否有中文字根
        /// </summary>
        /// <param name="srcString"></param>
        /// <returns></returns>
        static public bool CheckEncode(string srcString)
        {
            return System.Text.Encoding.UTF8.GetBytes(srcString).Length > srcString.Length;
        }

        /// <summary>
        /// 读取排片配置文件的内容
        /// </summary>
        private void ReadFilmList()
        {
            string xml = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 5) + @"\XML\" + "FilmList.xml";
            FileInfo finfo = new FileInfo(xml);
            if (finfo.Exists)
            {
                for (int i = 0; i < memberData.Count; i++)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(xml);
                    XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists").SelectSingleNode("List" + i.ToString());
                    XmlElement element = (XmlElement)xmlNode;
                    memberData[i].MovieNo = element["MovieNo"].InnerText;
                    memberData[i].Start = element["StartTime"].InnerText;
                    memberData[i].End = element["StopTime"].InnerText;
                    memberData[i].MovieName = element["MovieName"].InnerText;
                    memberData[i].FullMovieName = element["FullMoviePath"].InnerText;
                }
            }
        }

        /// <summary>
        /// 读取配置文件Type.xml,获取播放器类型
        /// </summary>
        private void ReadType()
        {
            FileInfo finfo = new FileInfo(playerPath + @"\XML\" + "Type.xml");
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(playerPath + @"\XML\" + "Type.xml");
                XmlNode childNodes = xmlDoc.SelectSingleNode("Type");
                XmlElement element = (XmlElement)childNodes;
                PlayType = element["Style"].InnerText;
                PlayLanguage = element["Language"].InnerText;
                PlayDOF = element["DOF"].InnerText;
                PlayHeight = Double.Parse(element["Height"].InnerText);
                PlayProjector = element["Projector"].InnerText;
                PlayPermission = element["Permission"].InnerText;
                //PlayControl = element["Control"].InnerText;
                PlayControlServer = element["Server"].InnerText;
                PlayControlClient=element["Client"].InnerText;
                PlayFrame = int.Parse(element["Frame"].InnerText);
                Play5DPicture = element["PlayPicture"].InnerText;
                PlayMac = element["Mac"].InnerText;
                PlayProjectorBrand = element["ProjectorBrand"].InnerText;

                XmlNode childNodeNext = childNodes.SelectSingleNode("Projector1");
                XmlElement elementNext = (XmlElement)childNodeNext;
                PlayProjector1IP = elementNext["IP"].InnerText;
                PlayProjector1Port = elementNext["Port"].InnerText;

                childNodeNext = childNodes.SelectSingleNode("Projector2");
                elementNext = (XmlElement)childNodeNext;
                PlayProjector2IP = elementNext["IP"].InnerText;
                PlayProjector2Port = elementNext["Port"].InnerText;

                childNodeNext = childNodes.SelectSingleNode("Projector3");
                elementNext = (XmlElement)childNodeNext;
                PlayProjector3IP = elementNext["IP"].InnerText;
                PlayProjector3Port = elementNext["Port"].InnerText;

                childNodeNext = childNodes.SelectSingleNode("Projector4");
                elementNext = (XmlElement)childNodeNext;
                PlayProjector4IP = elementNext["IP"].InnerText;
                PlayProjector4Port = elementNext["Port"].InnerText;

                childNodeNext = childNodes.SelectSingleNode("Projector5");
                elementNext = (XmlElement)childNodeNext;
                PlayProjector5IP = elementNext["IP"].InnerText;
                PlayProjector5Port = elementNext["Port"].InnerText;

                childNodeNext = childNodes.SelectSingleNode("Projector6");
                elementNext = (XmlElement)childNodeNext;
                PlayProjector6IP = elementNext["IP"].InnerText;
                PlayProjector6Port = elementNext["Port"].InnerText;

                childNodeNext = childNodes.SelectSingleNode("Projector7");
                elementNext = (XmlElement)childNodeNext;
                PlayProjector7IP = elementNext["IP"].InnerText;
                PlayProjector7Port = elementNext["Port"].InnerText;

                childNodeNext = childNodes.SelectSingleNode("Projector8");
                elementNext = (XmlElement)childNodeNext;
                PlayProjector8IP = elementNext["IP"].InnerText;
                PlayProjector8Port = elementNext["Port"].InnerText;

                childNodeNext = childNodes.SelectSingleNode("DelayModule");
                elementNext = (XmlElement)childNodeNext;
                DelayIP = elementNext["IPDelay"].InnerText;
                DelayPort = elementNext["PortDelay"].InnerText;

                childNodeNext = childNodes.SelectSingleNode("FuseModule");
                elementNext = (XmlElement)childNodeNext;
                FuseIP = elementNext["IPFuse"].InnerText;
                FusePort = elementNext["PortFuse"].InnerText;
                FuseProtocol = elementNext["CommunicationMode"].InnerText;
            }
        }

        /// <summary>
        /// 保存排片数据到FilmList.xml文件中
        /// </summary>
        public void SaveFilmList()
        {
            string xml = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 5) + @"\XML\" + "FilmList.xml";
            FileInfo finfo = new FileInfo(xml);
            if (finfo.Exists)
            {
                for (int i = 0; i < memberData.Count; i++)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(xml);
                    XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists").SelectSingleNode("List" + i.ToString());
                    XmlElement element = (XmlElement)xmlNode;
                    element["MovieNo"].InnerText = memberData[i].MovieNo;
                    element["StartTime"].InnerText = memberData[i].Start;
                    element["StopTime"].InnerText = memberData[i].End;
                    element["MovieName"].InnerText = memberData[i].MovieName;
                    element["FullMoviePath"].InnerText = memberData[i].FullMovieName;
                    xmlDoc.Save(xml);
                }
            }
        }
       
        /// <summary>
        /// 保存导入到播放列表的文件
        /// </summary>
        public void SaveFilmPlayList()
        {
            string xml = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 5) + @"\XML\" + "FilmList.xml";
            FileInfo finfo = new FileInfo(xml);
            if (finfo.Exists)
            {
                for (int i = 0; i < memberData.Count; i++)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(xml);
                    XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists").SelectSingleNode("List" + i.ToString());
                    XmlElement element = (XmlElement)xmlNode;
                    element["MovieNo"].InnerText = memberData[i].MovieNo;
                    //element["StartTime"].InnerText = memberData[i].Start;
                    //element["StopTime"].InnerText = memberData[i].End;
                    element["MovieName"].InnerText = memberData[i].MovieName;
                    element["FullMoviePath"].InnerText = memberData[i].FullMovieName;
                    xmlDoc.Save(xml);
                }
            }
        }

        /// <summary>
        /// 远程关机
        /// </summary>
        private void ShutDown()
        {
            Process commandProcess = new Process();
            try
            {
                commandProcess.StartInfo.FileName = "cmd.exe";
                commandProcess.StartInfo.UseShellExecute = false;
                commandProcess.StartInfo.CreateNoWindow = true;
                commandProcess.StartInfo.RedirectStandardError = true;
                commandProcess.StartInfo.RedirectStandardInput = true;
                commandProcess.StartInfo.RedirectStandardOutput = true;
                commandProcess.Start();
                string s = "shutdown /s /m" + " " + FuseIP + " /t 5 /f";
                commandProcess.StandardInput.WriteLine(s);
                commandProcess.StandardInput.WriteLine("exit");
                for (; !commandProcess.HasExited;)//等待cmd命令运行完毕
                {
                    System.Threading.Thread.Sleep(1);
                }
                //错误输出
                string tmpout = commandProcess.StandardError.ReadToEnd();
                string tmpout1 = commandProcess.StandardOutput.ReadToEnd();
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
                Module.WriteLogFile(e.Message);
            }
            finally
            {
                if (commandProcess != null)
                {
                    commandProcess.Dispose();
                    commandProcess = null;
                }
            }

        }

        /// <summary>
        /// 显示软件版本信息以及公司信息
        /// </summary>
        private void changeWinVersionLanguage()
        {
            if ("CN".Equals(MainWindow.PlayLanguage))
            {
                txtCompany.Text =
                              "网址：www.shuqee.cn \r\n" +
                              "电话：020 34885536  \r\n" +
                              "地址：广州市番禺区石基镇市莲路富城工业园3号楼 \r\n" +
                              "邮箱：shuqee@shuqee.com \r\n" +
                              "软件归广州数祺数字科技有限公司版权所有， 任何单位和个人不得复制本程序! \r\n\r\n" +
                              "软件类型：" + MainWindow.PlayType + "\r\n" +
                              "软件语言：" + MainWindow.PlayLanguage + "\r\n" +
                              "自由度：  " + MainWindow.PlayDOF + "\r\n" +
                              "行程高度：" + MainWindow.PlayHeight + "%" +"\r\n"+
                              "序列号:" + UdpConnect.uuid;
                txtUpdate.Text =
                           "shuqee版本更新信息：\r\n" +
                           "                   V7.1.8 \r\n" +
                           "更新日期：2020/3/11 \r\n" +
                           "更新内容：增加对中继继电器模块控制 \r\n" +
                           "               整合将所有数据文件 \r\n" +
                           "/**************************************/ \r\n" +
                           "shuqee版本更新信息：\r\n" +
                           "                   V7.1.7 \r\n" +
                           "更新日期：2019/10/18 \r\n" +
                           "更新内容：更改4DM文件导入方式 \r\n" +
                           "/**************************************/ \r\n" +
                           "shuqee版本更新信息：\r\n" +
                           "                   V7.1.6 \r\n" +
                           "更新日期：2019/10/12 \r\n" +
                           "更新内容：增加排片操作 \r\n" +
                           "               支持60帧数据 \r\n" +
                           "/**************************************/ \r\n" +
                           "shuqee版本更新信息：\r\n" +
                           "                   V7.1.5 \r\n" +
                           "更新日期：2019/10/8 \r\n" +
                           "更新内容：文件合并与中控板匹配 \r\n" +
                           "               可接收服务指令 \r\n" +
                           "/**************************************/ \r\n" +
                           "shuqee版本更新信息：\r\n" +
                           "                   V7.1.4 \r\n" +
                           "更新日期：2019/9/23 \r\n" +
                           "更新内容：增加对投影机的控制 \r\n" +
                           "               增加TCP客户端与服务器，可在参数中设置 \r\n" +
                           "/**************************************/ \r\n" +
                           "shuqee版本更新信息：\r\n" +
                           "                   V7.1.3 \r\n" +
                           "更新日期：2019/9/4 \r\n" +
                           "更新内容：增加六自由度，修改4DM喷气 \r\n" +
                           "/**************************************/ \r\n" +
                           "shuqee版本更新信息：\r\n" +
                           "                   V7.1.2 \r\n" +
                           "更新日期：2019/9/2 \r\n" +
                           "更新内容：修改软件打开提示，修改调试界面 \r\n" +
                           "                修复参数设置不正确，声音调节问题   \r\n" +
                           "/**************************************/ \r\n" +
                           "shuqee版本更新信息：\r\n" +
                           "                   V7.1.1 \r\n" +
                           "更新日期：2019/8/20 \r\n" +
                           "更新内容：更改软件界面 \r\n" +
                           "                增加控制台功能，增加排片功能，增加参数设置   \r\n" +
                           "/**************************************/ \r\n" +
                           "shuqee版本更新信息：\r\n" +
                           "                   V6.2.4 \r\n" +
                           "更新日期：2019/4/11 \r\n" +
                           "更新内容：优化界面，删除冗余代码 \r\n" +
                           "/**************************************/ \r\n" +
                           "shuqee版本更新信息：\r\n" +
                           "                   V6.2.3 \r\n" +
                           "更新日期：2019/3/27 \r\n" +
                           "更新内容：优化界面，将类型模块语言模块整合一起 \r\n" +
                           "/**************************************/ \r\n" +
                           "shuqee版本更新信息：\r\n" +
                           "                   V6.2.1 \r\n" +
                           "更新日期：2018/10/25 \r\n" +
                           "更新内容：软件整体升级，修改通信方式，增快数据发送频率，增加数据采集点";
            }
            else
            {
                txtCompany.Text =
                               "Website：www.shuqee.com \r\n" +
                               "Telephone：0086 020-34885536 \r\n" +
                               "Address：Bldg 3.Fucheng industrial park,shilian road,shiji village,shiji town,panyu district,guangzhou,CN.\r\n" +
                               "Email：shuqee@shuqee.com \r\n" +
                               "      Copyright by Guangzhou Shuqee Digital Tech. Co., Ltd. Any company or personal can not copy this software! \r\n\r\n" +
                               "Software Type: " + MainWindow.PlayType + "\r\n" +
                               "Software Language: " + MainWindow.PlayLanguage + "\r\n" +
                               "PlayDOF: " + MainWindow.PlayDOF + "\r\n" +
                               "Height: " + MainWindow.PlayHeight + "%" + "\r\n";

                txtUpdate.Text =
                           "Shuqee Version Update Information：\r\n" +
                          "                   V7.1.8 \r\n" +
                          "Updated Date：2020/3/11 \r\n" +
                          "Updated Content：Add the Delay Module \r\n" +
                          "                            Put All data file together \r\n" +
                          "/**************************************/ \r\n" +
                          "Shuqee Version Update Information：\r\n" +
                          "                   V7.1.7 \r\n" +
                          "Updated Date：2019/10/18 \r\n" +
                          "Updated Content：Change the way 4DM files are imported \r\n" +
                          "/**************************************/ \r\n" +
                          "Shuqee Version Update Information：\r\n" +
                          "                   V7.1.6 \r\n" +
                          "Updated Date：2019/10/12 \r\n" +
                          "Updated Content：Support 60 Frame \r\n" +
                          "/**************************************/ \r\n" +
                          "Shuqee Version Update Information：\r\n" +
                          "                   V7.1.5 \r\n" +
                          "Updated Date：2019/10/8 \r\n" +
                          "Updated Content：Merge File \r\n" +
                          "                            Server commands can be received \r\n" +
                          "/**************************************/ \r\n" +
                          "Shuqee Version Update Information：\r\n" +
                          "                   V7.1.4 \r\n" +
                          "Updated Date：2019/9/23 \r\n" +
                          "Updated Content：Control the projector \r\n" +
                          "                            Add the TCP Client and the TCP Server \r\n" +
                          "/**************************************/ \r\n" +
                          "Shuqee Version Update Information：\r\n" +
                          "                   V7.1.3 \r\n" +
                          "Updated Date：2019/9/4 \r\n" +
                          "Updated Content：Add the 6DOF \r\n" +
                          "/**************************************/ \r\n" +
                          "Shuqee Version Update Information：\r\n" +
                          "                   V7.1.2 \r\n" +
                          "Updated Date：2019/9/2 \r\n" +
                          "Updated Content：Modify the software tips,Modify the Debug interface \r\n" +
                          "                              Fixed incorrect parameter setting, sound adjustment problem  \r\n" +
                          "/**************************************/ \r\n" +
                          "Shuqee Version Update Information：\r\n" +
                          "                   V7.1.1 \r\n" +
                          "Updated Date：2019/8/20 \r\n" +
                          "Updated Content：Change the software interface \r\n" +
                          "                       Add the control,Add the Film Row Piece   \r\n" +
                          "/**************************************/ \r\n" +
                          "Shuqee Version Update Information：\r\n" +
                          "                   V6.2.4 \r\n" +
                          "Updated Date：2019/4/11 \r\n" +
                          "Updated Content：Optimize the interface and remove redundant code \r\n" +
                          "/**************************************/ \r\n" +
                          "Shuqee Version Update Information：\r\n" +
                          "                   V6.2.2 \r\n" +
                          "Updated Date：2018/12/5 \r\n" +
                          "Updated Content：Optimize the interface, change play video display problems \r\n" +
                          "/**************************************/ \r\n" +
                          "Shuqee Version Update Information：\r\n" +
                          "                   V6.2.1 \r\n" +
                          "Updated Date：2018/10/25 \r\n" +
                          "Updated Content：Software overall upgrade, modify communication mode,increase data collection points";
            }
        }

        #endregion

        #region 窗体发生事件

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            rb1.IsChecked = true;
        }

        #region 导航栏隐藏事件

        /// <summary>
        /// 设置导航栏
        /// </summary>
        /// <param name="visibility"></param>
        private void SetNavigationEnable(bool visibility)
        {
            if (visibility)
            {
                tabControl.Visibility = Visibility.Visible;
                Grid.SetColumn(tabControlShow, 2);
                Grid.SetColumnSpan(tabControlShow, 1);
                tabSetFlag = 1;
            }
            else
            {
                tabControl.Visibility = Visibility.Hidden;
                Grid.SetColumn(tabControlShow, 1);
                Grid.SetColumnSpan(tabControlShow, 2);
            }
        }

        private void Rb_Click(object sender, RoutedEventArgs e)
        {
            //SetNavigationEnable(true);
        }

        /// <summary>
        /// 切换tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rb_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb_temp = sender as RadioButton;
            int tag = Convert.ToInt32(rb_temp.Tag);
            switch (tag)
            {
                case 1:
                    tabControlShow.SelectedIndex = tag - 1;
                    SetNavigationEnable(false);
                    break;
                case 2:
                    if (UserControlClass.MSStatus != MediaStatus.Play && UdpConnect.TimeCode == 0 || PlayPermission == "TRUE")
                    {
                        tabControlShow.SelectedIndex = tag - 1;
                        SetNavigationEnable(false);
                        TimerDebugInit();
                        UdpConnect.isDebug = true;
                    }
                    break;
                case 3:
                    tabControlShow.SelectedIndex = tag - 1;
                    SetNavigationEnable(false);
                    break;
                case 4:
                    tabControlShow.SelectedIndex = tag - 1;
                    SetNavigationEnable(false);
                    break;
                case 5:
                    tabControl.SelectedIndex = 1;
                    tabControlShow.SelectedIndex = tag - 1;
                    SetNavigationEnable(true);
                    txtRegStr.Text = UdpConnect.valDate;
                    changeWinVersionLanguage();
                    break;
                case 6:
                    tabControl.SelectedIndex = 2;
                    tabControlShow.SelectedIndex = tag - 1;
                    SetNavigationEnable(true);
                    break;
            }
            if (tag != 2)
            {
                UdpSend.movieStop = true;
                UdpSend.SendReset();
                if (timerDebug != null)
                {
                    timerDebug.Stop();
                }
                UdpConnect.isDebug = false;
            }
        }

        private void TabControlShow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tabSetFlag == 1)
            {
                // SetNavigationEnable(false);
            }
        }

        #endregion

        #endregion

        #region 按钮事件
        /// <summary>
        /// 模式按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMode_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int tag = Convert.ToInt32(btn.Tag);

            //UI
            Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x54, 0x54, 0x54));
            if (tag == 1)
            {
                btnLtcModeGrid.Background = Brushes.DodgerBlue;
                btnLocModeGrid.Background = brush;
                tabControlShowData.SelectedIndex = 0;
            }
            else
            {
                btnLocModeGrid.Background = Brushes.DodgerBlue;
                btnLtcModeGrid.Background = brush;
                tabControlShowData.SelectedIndex = 1;
            }
        }

        /// <summary>
        /// 缸按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnNum_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int tag = Convert.ToInt32(btn.Tag);
            Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x93, 0x93, 0x93));
            //btnNum1.Background = brush;
            //btnNum2.Background = brush;
            switch (tag)
            {
                case 1:
                    changeBackground(0);
                    break;
                case 2:
                    changeBackground(1);
                    break;
                case 3:
                    changeBackground(2);
                    break;
                case 4:
                    if ("6DOF".Equals(PlayDOF))
                    {
                        changeBackground(3);
                    }
                    break;
                case 5:
                    if ("6DOF".Equals(PlayDOF))
                    {
                        changeBackground(4);
                    }
                    break;
                case 6:
                    if ("6DOF".Equals(PlayDOF))
                    {
                        changeBackground(5);
                    }
                    break;
            }
        }

        private void BtnChairEffect_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int tag = Convert.ToInt32(btn.Tag);
            if ("4DM".Equals(MainWindow.PlayType) && tag == 6)
            {
                changeChairEffect(6);
            }
            changeChairEffect(tag - 1);
        }

        private void BtnEvEffect_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int tag = Convert.ToInt32(btn.Tag);
            changeEvEffect(tag - 1);
            if (tag == 1)
            {
                WinLightning winLightning = new WinLightning();
                winLightning.Left = 900;
                winLightning.Top = 320;
                winLightning.ShowDialog();
            }
        }

        /// <summary>
        /// 播放列表菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListMenu_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem menu = (System.Windows.Controls.MenuItem)sender;
            int tag = Convert.ToInt32(menu.Tag);
            if (PlayType.Equals("5D"))
            {
                switch (tag)
                {
                    case 1:
                        btnPlayClickFun();
                        break;
                    case 2:
                        AddFiles();
                        break;
                    case 3:
                        RemoveFiles();
                        break;
                    case 4:
                        RemoveAllFiles();
                        break;
                }
            }
            else
            {
                switch (tag)
                {
                    case 1:
                        //btnPlayClickFun();
                        break;
                    case 2:
                        //AddActiconFile();
                        AddActionFileNew();
                        break;
                    case 3:
                        RemoveFileFilmList();
                        break;
                    case 4:
                        RemoveFileFilmList();
                        break;
                }
            }
        }

        /// <summary>
        /// 关于部分-按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int tag = Convert.ToInt32(btn.Tag);
            //UI
            Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x54, 0x54, 0x54));
            btnRegisterGrid.Background = brush;
            btnUpdateLogGrid.Background = brush;
            btnAboutUsGrid.Background = brush;
            //changeWinVersionLanguage();
            switch (tag)
            {
                case 1:
                    btnRegisterGrid.Background = Brushes.DodgerBlue;
                    RegisterWindow regWin = new RegisterWindow();
                    regWin.ShowDialog();
                    break;
                case 2:
                    btnUpdateLogGrid.Background = Brushes.DodgerBlue;
                    tabItemAbout2.IsSelected = true;
                    break;
                case 3:
                    btnAboutUsGrid.Background = Brushes.DodgerBlue;
                    tabItemAbout3.IsSelected = true;
                    break;
            }
        }
        /// <summary>
        /// 设置部分-按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSet_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int tag = Convert.ToInt32(btn.Tag);
            //UI
            Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x54, 0x54, 0x54));
            btnLangGrid.Background = brush;
            btnParamSetGrid.Background = brush;
            btnProjectorSet.Background = brush;
            btnDelaySet.Background = brush;
            switch (tag)
            {
                case 1:
                    btnLangGrid.Background = Brushes.DodgerBlue;
                    tabControlSet.SelectedIndex = 0;
                    break;
                case 2:
                    btnParamSetGrid.Background = Brushes.DodgerBlue;
                    if (isLogin == false)
                    {
                        OpenLoginWin();
                    }
                    if (isLogin == true)
                    {
                        tabControlSet.SelectedIndex = 1;
                    }
                    break;
                case 3:
                    btnProjectorSet.Background = Brushes.DodgerBlue;
                    tabControlSet.SelectedIndex = 2;
                    break;
                case 4:
                    btnDelaySet.Background = Brushes.DodgerBlue;
                    if (isLogin == true)
                    {
                        tabControlSet.SelectedIndex = 3;
                    }
                    break;
            }
        }

        private void BtnConfirmProjector_Click(object sender, RoutedEventArgs e)
        {
            SaveProjector();
            if (PlayLanguage.Equals("CN"))
            {
                MessageBox.Show("修改成功");
            }
            else
            {
                MessageBox.Show("Modify Successfully");
            }
        }

        private void BtnDefaultProjector_Click(object sender, RoutedEventArgs e)
        {
            if (PlayLanguage.Equals("CN"))
            {
                comboBoxBrand.Text = "松下";
            }
            else
            {
                comboBoxBrand.Text = "Panasonic";
            }
            txtIpProjector1.Text = "192.168.1.121";
            txtPortProjector1.Text = "1060";
            txtIpProjector2.Text = "192.168.1.122";
            txtPortProjector2.Text = "1061";
            txtIpProjector3.Text = "192.168.1.123";
            txtPortProjector3.Text = "1062";
            txtIpProjector4.Text = "192.168.1.124";
            txtPortProjector4.Text = "1063";
            txtIpProjector5.Text = "192.168.1.125";
            txtPortProjector5.Text = "1064";
            txtIpProjector6.Text = "192.168.1.126";
            txtPortProjector6.Text = "1065";
            txtIpProjector7.Text = "192.168.1.127";
            txtPortProjector7.Text = "1066";
            txtIpProjector8.Text = "192.168.1.128";
            txtPortProjector8.Text = "1067";
            SaveProjector();
        }

        private void BtnConfirmDelay_Click(object sender, RoutedEventArgs e)
        {
            SaveDelayModule();
            DelayIP = txtIpDelay.Text;
            DelayPort = txtPortDelay.Text;
            if(PlayLanguage.Equals("CN"))
            {
                MessageBox.Show("修改成功");
            }
            else
            {
                MessageBox.Show("Modify Successfully");
            }
        }

        private void BtnConfirmFuse_Click(object sender, RoutedEventArgs e)
        {
            SaveFuseModule();
            FuseIP = txtIpFuse.Text;
            FusePort = txtPortFuse.Text;
            FuseProtocol = comboBoxNet.Text;
            if (PlayLanguage.Equals("CN"))
            {
                MessageBox.Show("修改成功");
            }
            else
            {
                MessageBox.Show("Modify Successfully");
            }
        }

        private void OpenLoginWin()
        {
            LoginWindow loginWin = new LoginWindow();
            loginWin.ShowDialog();
        }

        private void BtnSetLang_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int tag = Convert.ToInt32(btn.Tag);
            //UI
            Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x73, 0x73, 0x73));
            btnEN.Background = brush;
            btnCN.Background = brush;
            switch (tag)
            {
                case 1:
                    btnEN.Background = Brushes.DodgerBlue;
                    ChangeLanguage("EN");
                    PlayLanguage = "EN";
                    break;
                case 2:
                    btnCN.Background = Brushes.DodgerBlue;
                    ChangeLanguage("CN");
                    PlayLanguage = "CN";
                    break;
            }
            changeWinVersionLanguage();
            SaveLang();
        }

        /// <summary>
        /// 鼠标进入变图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnImgBack_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = sender as Image;
            int tag = Convert.ToInt32(img.Tag);
            switch (tag)
            {
                case 1:
                    btnImgBack.Source = Common.ChangeBitmapToImageSource(Properties.Resources.Icon_Back_light);
                    break;
                case 2:
                    if (UserControlClass.MSStatus == MediaStatus.Play)
                    {
                        btnImgPlay.Source = Common.ChangeBitmapToImageSource(Properties.Resources.Icon_Pause_light);
                    }
                    else
                    {
                        btnImgPlay.Source = Common.ChangeBitmapToImageSource(Properties.Resources.Icon_Play_light);
                    }
                    break;
                case 3:
                    btnImgStop.Source = Common.ChangeBitmapToImageSource(Properties.Resources.Icon_Stop_light);
                    break;
                case 4:
                    btnImgNext.Source = Common.ChangeBitmapToImageSource(Properties.Resources.Icon_Forward_light);
                    break;
                case 5:
                    btnImgVadd.Source = Common.ChangeBitmapToImageSource(Properties.Resources.Icon_Vol_Add_light);
                    break;
                case 6:
                    btnImgVReduce.Source = Common.ChangeBitmapToImageSource(Properties.Resources.Icon_Vol_Reduce_light);
                    break;
            }
        }

        /// <summary>
        /// 鼠标离开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnImgBack_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = sender as Image;
            int tag = Convert.ToInt32(img.Tag);
            switch (tag)
            {
                case 1:
                    btnImgBack.Source = Common.ChangeBitmapToImageSource(Properties.Resources.Icon_Back);
                    break;
                case 2:
                    if (UserControlClass.MSStatus == MediaStatus.Play)
                    {
                        btnImgPlay.Source = Common.ChangeBitmapToImageSource(Properties.Resources.Icon_Pause);
                    }
                    else
                    {
                        btnImgPlay.Source = Common.ChangeBitmapToImageSource(Properties.Resources.Icon_Play);
                    }
                    break;
                case 3:
                    btnImgStop.Source = Common.ChangeBitmapToImageSource(Properties.Resources.Icon_Stop);
                    break;
                case 4:
                    btnImgNext.Source = Common.ChangeBitmapToImageSource(Properties.Resources.Icon_Forward);
                    break;
                case 5:
                    btnImgVadd.Source = Common.ChangeBitmapToImageSource(Properties.Resources.Icon_Vol_Add);
                    break;
                case 6:
                    btnImgVReduce.Source = Common.ChangeBitmapToImageSource(Properties.Resources.Icon_Vol_Reduce);
                    break;
            }
        }

        /// <summary>
        /// 上升
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            dataNum[0] = 255;
            dataNum[1] = 255;
            dataNum[2] = 255;
        }

        /// <summary>
        /// 下降
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            dataNum[0] = 0;
            dataNum[1] = 0;
            dataNum[2] = 0;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //System.Windows.Forms.FolderBrowserDialog m_Dialog = new System.Windows.Forms.FolderBrowserDialog();

            //System.Windows.Forms.DialogResult result = m_Dialog.ShowDialog();

            //if (result == System.Windows.Forms.DialogResult.Cancel)
            //{
            //    return;
            //}
            //string m_Dir = m_Dialog.SelectedPath.Trim();
            //DirectoryInfo info = new DirectoryInfo(m_Dir);
            //string s = info.Name;
            //string s1 = info.FullName;
            //memberData[dataGrid.SelectedIndex].MovieName = s;
            //memberData[dataGrid.SelectedIndex].FullMovieName = s1;
            //memberData[dataGrid.SelectedIndex].MovieNo = (dataGrid.SelectedIndex + 1).ToString();

            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = "-DTS";
            ofd.Filter = "DTS file|*-DTS";
            if (ofd.ShowDialog() == true)
            {
                string s = ofd.SafeFileName;
                string s1 = ofd.FileName;
                s = s.Replace("-DTS", "");
                memberData[dataGrid.SelectedIndex].MovieName = s;
                memberData[dataGrid.SelectedIndex].FullMovieName = s1;
                memberData[dataGrid.SelectedIndex].MovieNo = (dataGrid.SelectedIndex + 1).ToString();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < memberData.Count; i++)
            {
                if (memberData[i].Start != "")
                {
                    if (CheckEncode(memberData[i].Start))
                    {
                        System.Windows.MessageBox.Show(memberData[i].Start + "输入有误，不能包含中文字根");
                        memberData[i].Start = "";
                    }
                    else
                    {
                        memberData[i].Start = checkStrStartAndEnd(memberData[i].Start);
                    }
                }
                if (memberData[i].End != "")
                {
                    if (CheckEncode(memberData[i].End))
                    {
                        System.Windows.MessageBox.Show(memberData[i].End + "输入有误，不能包含中文字根");
                        memberData[i].End = "";
                    }
                    else
                    {
                        memberData[i].End = checkStrStartAndEnd(memberData[i].End);
                    }
                }
            }
            SaveFilmList();
            RestartSoftWare();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            memberData[dataGrid.SelectedIndex].MovieNo = "";
            memberData[dataGrid.SelectedIndex].Start = "";
            memberData[dataGrid.SelectedIndex].End = "";
            memberData[dataGrid.SelectedIndex].MovieName = "";
            memberData[dataGrid.SelectedIndex].FullMovieName = "";
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            if (PlayType.Equals("4DM"))
            {
                //AddActiconFile();
                AddActionFileNew();
            }
            else
            {
                AddFiles();
            }
        }

        private void AddActiconFile()
        {
            System.Windows.Forms.FolderBrowserDialog m_Dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = m_Dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string m_Dir = m_Dialog.SelectedPath.Trim();
            DirectoryInfo info = new DirectoryInfo(m_Dir);
            string s = info.Name;
            string s1 = info.FullName;
            ListView.Items.Add(s);
            ListView.SelectedValue = s;
            memberData[ListView.SelectedIndex].MovieName = s;
            memberData[ListView.SelectedIndex].FullMovieName = s1;
            if (ListView.SelectedIndex == 0)
            {
                ReadFilmListFile(0);
            }
        }

        private void AddActionFileNew()
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = "-DTS";
            ofd.Filter = "DTS file|*-DTS";
            if (ofd.ShowDialog() == true)
            {
                string s = ofd.SafeFileName;
                string s1 = ofd.FileName;
                s = s.Replace("-DTS","");
                ListView.Items.Add(s);
                ListView.SelectedValue = s;
                memberData[ListView.SelectedIndex].MovieName = s;
                memberData[ListView.SelectedIndex].FullMovieName = s1;
                SaveFilmPlayList();
            }
           // if (ListView.SelectedIndex == 0 && PlayControl!="SERVER")
           // {
           //     ReadFilmListFile(0);
           // }
            if (ListView.SelectedIndex == 0 && PlayControlServer != "TRUE")
            {
                ReadFilmListFile(0);
            }
        }

        private void SelectXmlMovie()
        {
            if (PlayType.Equals("4DM"))
            {
                ListView.Items.Clear();
                for (int i = 0; i < memberData.Count; i++)
                {
                    if (memberData[i].MovieName != "")
                    {
                        ListView.Items.Add(memberData[i].MovieName);
                    }
                }
            }
        }

        private void btnImgPlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            btnPlayClickFun();
        }

        private void btnImgBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (UserControlClass.FileName != null && UserControlClass.FileName != "")
            {
                UserControlClass.MPPlayer.Position = UserControlClass.MPPlayer.Position - TimeSpan.FromSeconds(20);
            }
        }

        private void btnImgNext_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (UserControlClass.FileName != null && UserControlClass.FileName != "")
            {
                UserControlClass.MPPlayer.Position = UserControlClass.MPPlayer.Position + TimeSpan.FromSeconds(20);
            }
        }

        private void btnImgStop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            btnStopClickFun();
        }

        private void btnImgVReduce_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (UserControlClass.FileName != null && UserControlClass.FileName != "")
            {
                sliderVol.Value = sliderVol.Value - 0.1;
                UserControlClass.MPPlayer.Volume = sliderVol.Value;
                SaveVolume();
            }

        }

        private void btnImgVadd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (UserControlClass.FileName != null && UserControlClass.FileName != "")
            {
                sliderVol.Value = sliderVol.Value + 0.1;
                UserControlClass.MPPlayer.Volume = sliderVol.Value;
                SaveVolume();
            }
        }

        private void btnDateTips_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow regwin = new RegisterWindow();
            regwin.ShowDialog();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            SaveType();
            RestartSoftWare();
        }

        private void ListModeChoose_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem menu = (System.Windows.Controls.MenuItem)sender;
            int tag = Convert.ToInt32(menu.Tag);
            switch (tag)
            {
                case 1:
                    listModeSingle.IsChecked = true;
                    listModeDefault.IsChecked = false;
                    listModeLoop.IsChecked = false;
                    ModePlayTag = "RepeatPlay";
                    break;
                case 2:
                    listModeSingle.IsChecked = false;
                    listModeDefault.IsChecked = true;
                    listModeLoop.IsChecked = false;
                    ModePlayTag = "DefaultPlay";
                    break;
                case 3:
                    listModeSingle.IsChecked = false;
                    listModeDefault.IsChecked = false;
                    listModeLoop.IsChecked = true;
                    ModePlayTag = "LoopPlay";
                    break;
            }
            SaveMode();
        }

        private void BtnScreen_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int tag = Convert.ToInt32(btn.Tag);
            //UI
            Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x99, 0x99));
            btnMain.Background = brush;
            btnSecond.Background = brush;
            switch (tag)
            {
                case 1:
                    btnMain.Background = Brushes.DodgerBlue;
                    curPlayProjector = "0";
                    break;
                case 2:
                    btnSecond.Background = Brushes.DodgerBlue;
                    curPlayProjector = "1";
                    break;
            }

        }

        private void BtnType_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int tag = Convert.ToInt32(btn.Tag);
            //UI
            Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x99, 0x99));
            btn4DM.Background = brush;
            btn5D.Background = brush;
            switch (tag)
            {
                case 1:
                    btn4DM.Background = Brushes.DodgerBlue;
                    curPlayType = "4DM";
                    break;
                case 2:
                    btn5D.Background = Brushes.DodgerBlue;
                    curPlayType = "5D";
                    break;
            }
        }

        private void BtnDefault_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void BtnDOF_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int tag = Convert.ToInt32(btn.Tag);
            //UI
            Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x99, 0x99));
            btn2DOF.Background = brush;
            btn3DOF.Background = brush;
            btn6DOF.Background = brush;
            switch (tag)
            {
                case 1:
                    btn2DOF.Background = Brushes.DodgerBlue;
                    curPlayDOF = "2DOF";
                    break;
                case 2:
                    btn3DOF.Background = Brushes.DodgerBlue;
                    curPlayDOF = "3DOF";
                    break;
                case 3:
                    btn6DOF.Background = Brushes.DodgerBlue;
                    curPlayDOF = "6DOF";
                    break;
            }
        }

        private void BtnPermission_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int tag = Convert.ToInt32(btn.Tag);
            Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x99, 0x99));
            btnAdmin.Background = brush;
            btnUser.Background = brush;
            switch (tag)
            {
                case 1:
                    btnAdmin.Background = Brushes.DodgerBlue;
                    curPermission = "TRUE";
                    break;
                case 2:
                    btnUser.Background = Brushes.DodgerBlue;
                    curPermission = "FALSE";
                    break;
            }
        }

        private void BtnControl_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int tag = Convert.ToInt32(btn.Tag);
            Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x99, 0x99));
            //btnServer.Background = brush;
            //btnClient.Background = brush;
            switch (tag)
            {
                case 1:
                    if (btnServer.Background == Brushes.DodgerBlue)
                    {
                        btnServer.Background = brush;
                       // curControl = "";
                        curControlServer = "FALSE";
                    }
                    else
                    {
                        btnServer.Background = Brushes.DodgerBlue;
                        curControlServer = "TRUE";
                    }
                    break;
                case 2:
                    if (btnClient.Background == Brushes.DodgerBlue)
                    {
                        btnClient.Background = brush;
                        curControlClient = "FALSE";
                    }
                    else
                    {
                        btnClient.Background = Brushes.DodgerBlue;
                        curControlClient = "TRUE";
                    }
                    break;
            }
        }

        private void BtnFrame_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int tag = Convert.ToInt32(btn.Tag);
            Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x99, 0x99));
            btnFrame48.Background = brush;
            btnFrame60.Background = brush;
            btnFrame120.Background = brush;
            switch (tag)
            {
                case 1:
                    btnFrame48.Background = Brushes.DodgerBlue;
                    curPlayFrame = "48";
                    break;
                case 2:
                    btnFrame60.Background = Brushes.DodgerBlue;
                    curPlayFrame = "60";
                    break;
                case 3:
                    btnFrame120.Background = Brushes.DodgerBlue;
                    curPlayFrame = "120";
                    break;
            }
        }

        private void Btn5DPicture_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int tag = Convert.ToInt32(btn.Tag);
            Brush brush = new SolidColorBrush(Color.FromArgb(0xff,0x99,0x99,0x99));
            btnShow.Background = brush;
            btnNotShow.Background = brush;
            switch (tag)
            {
                case 1:
                    btnShow.Background = Brushes.DodgerBlue;
                    cur5DPicture = "0";
                    break;
                case 2:
                    btnNotShow.Background = Brushes.DodgerBlue;
                    cur5DPicture = "1";
                    break;
            }
        }

        private void BtnMac_Click(object sender, RoutedEventArgs e)
        {
            
            Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x99, 0x99));
            if(btnMac.Background==Brushes.DodgerBlue)
            {
                btnMac.Background = brush;
                curMac = "FALSE";
            }
            else
            {
                btnMac.Background = Brushes.DodgerBlue;
                curMac = "TRUE";
            }
        }

        private void CheckFuse_Click(object sender, RoutedEventArgs e)
        {
            if (checkFuse.IsChecked == false)
            {
                MessageBoxResult dr;
                if (PlayLanguage.Equals("CN"))
                {
                    dr = MessageBox.Show("确定关闭融合主机吗?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                }
                else
                {
                    dr = MessageBox.Show("Are you sure to close fuse host?", "Tips", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                }
                if (dr == MessageBoxResult.OK)
                {
                    ShutDown();
                }
                else
                {
                    checkFuse.IsChecked = true;
                }
            }
        }
        #endregion

        #region 播放窗体
        ///<summary>
        ///定义系统时间显示事件
        /// </summary>
        private void timer_Tick(object sender, EventArgs e)
        {
            // txtTime.Text = DateTime.Now.Hour.ToString("D2") + " : " + DateTime.Now.Minute.ToString("D2") + " : " + DateTime.Now.Second.ToString("D2");

            //中控指令
            //if (Module.controlCommand.Length == 12 || Module.controlCommand.Length == 13)
            //{
            //    string str = Module.controlCommand.Remove(0, 11);
            //    try
            //    {
            //        ListView.SelectedIndex = int.Parse(str);
            //    }
            //    catch
            //    {
            //        ListView.SelectedIndex = 0;
            //    }                
            //    btnPlayClickFun();
            //    Module.WriteLogFile(Module.controlCommand);
            //    Module.controlCommand = "";
            //}

            //if (Module.controlCommand.Equals("video_play"))
            //{
            //    btnPlayClickFun();
            //    Module.WriteLogFile(Module.controlCommand);
            //    Module.controlCommand = "";
            //}

            //if (Module.controlCommand.Equals("video_pause"))
            //{
            //    btnPlayClickFun();
            //    Module.WriteLogFile(Module.controlCommand);
            //    Module.controlCommand = "";
            //}

            //if (Module.controlCommand.Equals("video_stop"))
            //{
            //    btnStopClickFun();
            //    Module.WriteLogFile(Module.controlCommand);
            //    Module.controlCommand = "";
            //}
        }

        /// <summary>
        /// 计时器响应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Tim_Elapsed(object sender, ElapsedEventArgs e)
        {
            sliderTime.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new DeleSetPosition(SetPosition));
        }

        /// <summary>
        /// 打开播放器响应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MPPlayer_MediaOpened(object sender, EventArgs e)
        {
            //UdpSend.range = int.Parse(textBox.Text);        //震动幅度
            //UdpSend.rate = int.Parse(textBox2.Text);        //震动频率
            if (UserControlClass.MPPlayer.NaturalDuration.HasTimeSpan)
            {
                MaxLen = (int)Math.Round(UserControlClass.MPPlayer.NaturalDuration.TimeSpan.TotalSeconds);
                sliderTime.Maximum = MaxLen;
                sliderTime.Minimum = 0;
                txtTime.Text = "00:00:00";
            }
            //画刷，把视频文件绘制在指定的矩形中
            VideoDrawing drawing = new VideoDrawing();
            //描述矩形的宽度，高度和位置
            Rect rect = new Rect(0.0, 0.0, UserControlClass.MPPlayer.NaturalVideoWidth, UserControlClass.MPPlayer.NaturalVideoHeight);
            //设置可在其中绘制视频的矩形区域
            drawing.Rect = rect;
            //设置与绘制关联的媒体播放器
            drawing.Player = UserControlClass.MPPlayer;
            //用Drawing绘制图形
            DrawingBrush brush = new DrawingBrush(drawing);
            //UserControlClass.sc2.FInkCanvas_Player.Background = brush;
            NewOpenPlay();
            //timerInit();
            if (timerStart == false)
            {
                timerInit();
            }
            else
            {
                Module.timerMovie.Start();
            }

            //int realityWidth = UserControlClass.MPPlayer.NaturalVideoWidth> Screen.PrimaryScreen.Bounds.Width? Screen.PrimaryScreen.Bounds.Width : UserControlClass.MPPlayer.NaturalVideoWidth;
            int realityWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            //int realityHeight = UserControlClass.MPPlayer.NaturalVideoHeight > Screen.PrimaryScreen.Bounds.Height ? Screen.PrimaryScreen.Bounds.Height*9/16 : UserControlClass.MPPlayer.NaturalVideoHeight;
            int realityHeight = realityWidth * UserControlClass.MPPlayer.NaturalVideoHeight / UserControlClass.MPPlayer.NaturalVideoWidth;
            System.Windows.Forms.Screen[] sc1 = System.Windows.Forms.Screen.AllScreens;//获取当前所有显示器的数组

            //如果有多个屏幕，就定义secondwindow的位置，让他在第二个屏幕全屏显示                                                      
            if (sc1.Length > 1)
            {
                var workingArea = sc1[1].Bounds;           //展厅投影系统
                if (MainWindow.PlayProjector.Equals("0"))
                {
                    workingArea = sc1[0].WorkingArea;
                }
                UserControlClass.NullBorderWin(UserControlClass.sc2.returnWinSc2(), workingArea.Left, workingArea.Top, workingArea.Width, workingArea.Height);
            }
            else
            {
                UserControlClass.NullBorderWin(UserControlClass.sc2.returnWinSc2(), realityWidth, realityHeight);
            }

        }

        /// <summary>
        /// 定时器初始化,用于调用发送数据
        /// </summary>
        private void timerInit()
        {
            if (Module.timerMovie == null)
            {
                Module.timerMovie = new DispatcherTimer();
            }
            if (Module.timerMovie.IsEnabled == false)
            {
                Module.timerMovie.Interval = TimeSpan.FromSeconds(0.01);
                Module.timerMovie.Tick += new EventHandler(timerMovie_tick);
                Module.timerMovie.Start();
            }
            timerStart = true;

        }

        private void timerMovie_tick(object sender, EventArgs e)
        {
            Debug.WriteLine("player发数据");
            if (UdpConnect.isDebug == false)
            {
                //if ("6DOF".Equals(PlayDOF))
                //{
                //    UdpSend.SendWrite6DOF(UserControlClass.MPPlayer.Position.TotalSeconds);
                //}
                //else
                //{
                //    UdpSend.SendWrite(UserControlClass.MPPlayer.Position.TotalSeconds);
                //}
                //UdpSend.SendTotal(UserControlClass.MPPlayer.Position.TotalSeconds);
                UdpSend.SendTotalNew(UserControlClass.MPPlayer.Position.TotalSeconds);
            }
            //showData();             //默认文件显示数据
            showTotalData();          //合并文件显示数据
            if (UserControlClass.MPPlayer.Position.TotalSeconds == 0)
            {
                count++;
                Debug.WriteLine("player发数据" + count);
                if (count == 100)
                {
                    Module.timerMovie.IsEnabled = false;
                    Module.timerMovie.Stop();
                }
            }
        }

        /// <summary>
        /// 清空数据显示栏的数据
        /// </summary>
        private void ClearData()
        {
            switch (PlayDOF)
            {
                case "2DOF":
                    pb1.Value = 127;
                    pb3.Value = 127;
                    txtPbVal1.Text = "127";
                    txtPbVal3.Text = "127";
                    break;
                case "3DOF":
                    pb1.Value = 0;
                    pb2.Value = 0;
                    pb3.Value = 0;
                    txtPbVal1.Text = "0";
                    txtPbVal2.Text = "0";
                    txtPbVal3.Text = "0";
                    break;
                case "6DOF":
                    pb1.Value = 0;
                    pb2.Value = 0;
                    pb3.Value = 0;
                    pb4.Value = 0;
                    pb5.Value = 0;
                    pb6.Value = 0;
                    txtPbVal1.Text = "0";
                    txtPbVal2.Text = "0";
                    txtPbVal3.Text = "0";
                    txtPbVal4.Text = "0";
                    txtPbVal5.Text = "0";
                    txtPbVal6.Text = "0";
                    break;
            }
            Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x99, 0x99));

            for (int i = 0; i < 8; i++)
            {
                checkBoxEvEffect[i].Background = brush;
            }

            for (int i = 0; i < 8; i++)
            {
                checkBoxChairEffect[i].Background = brush;
            }
        }

        /// <summary>
        /// 显示动作数据与特效数据
        /// </summary>
        private void showData()
        {
            int ii;

            if ("5D".Equals(MainWindow.PlayType))
            {

                ii = (int)Math.Round((UserControlClass.MPPlayer.Position.TotalSeconds * 2400), 0);
                ii = (int)Math.Round((ii / 50.0), 0);
            }
            else
            {
                ii = (int)Math.Round((UdpConnect.TimeCode * 2400), 0);
                ii = (int)Math.Round((ii / 50.0), 0);
                //label.Content = "TimeCode: " + UdpConnect.strLongTimeCode;
            }
            try
            {
                if ("6DOF".Equals(PlayDOF))
                {
                    this.txtPbVal1.Text = Module.actionFile[6 * ii].ToString();
                    this.txtPbVal2.Text = Module.actionFile[6 * ii + 1].ToString();
                    this.txtPbVal3.Text = Module.actionFile[6 * ii + 2].ToString();
                    this.txtPbVal4.Text = Module.actionFile[6 * ii + 3].ToString();
                    this.txtPbVal5.Text = Module.actionFile[6 * ii + 4].ToString();
                    this.txtPbVal6.Text = Module.actionFile[6 * ii + 5].ToString();

                    pb1.Value = Module.actionFile[6 * ii];
                    pb2.Value = Module.actionFile[6 * ii + 1];
                    pb3.Value = Module.actionFile[6 * ii + 2];
                    pb4.Value = Module.actionFile[6 * ii + 3];
                    pb5.Value = Module.actionFile[6 * ii + 4];
                    pb6.Value = Module.actionFile[6 * ii + 5];

                }
                else
                {
                    this.txtPbVal1.Text = Module.actionFile[3 * ii].ToString();
                    this.txtPbVal2.Text = Module.actionFile[3 * ii + 1].ToString();
                    this.txtPbVal3.Text = Module.actionFile[3 * ii + 2].ToString();

                    pb1.Value = Module.actionFile[3 * ii];
                    pb2.Value = Module.actionFile[3 * ii + 1];
                    pb3.Value = Module.actionFile[3 * ii + 2];
                }

                Boolean[] ev = new Boolean[8];
                Boolean[] cEffect = new Boolean[8];
                Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x99, 0x99));
               
                for (int i = 0, n = 1; i < 8; i++)
                {
                    ev[i] = ((Module.effectFile[2 * ii] & n) == 0 ? false : true);
                    n = n << 1;
                    if (ev[i] == true)
                    {
                        checkBoxEvEffect[i].Background = Brushes.DodgerBlue;
                    }
                    else
                    {
                        checkBoxEvEffect[i].Background = brush;
                    }
                }

                for (int i = 0, n = 1; i < 8; i++)
                {
                    cEffect[i] = ((Module.effectFile[2 * ii + 1] & n) == 0 ? false : true);
                    n = n << 1;
                    if (cEffect[i] == true)
                    {
                        checkBoxChairEffect[i].Background = Brushes.DodgerBlue;
                    }
                    else
                    {
                        checkBoxChairEffect[i].Background = brush;
                    }
                }

            }
            catch
            {
                this.txtPbVal1.Text = "0";
                this.txtPbVal2.Text = "0";
                this.txtPbVal3.Text = "0";
                this.txtPbVal4.Text = "0";
                this.txtPbVal5.Text = "0";
                this.txtPbVal6.Text = "0";

                pb1.Value = 0;
                pb2.Value = 0;
                pb3.Value = 0;
                pb4.Value = 0;
                pb5.Value = 0;
                pb6.Value = 0;
            }
        }

        /// <summary>
        /// 显示所有数据(合并文件)
        /// </summary>
        private void showTotalData()
        {
            int ii;
            double pos = 0;
            switch (MainWindow.PlayFrame)
            {
                case 48:
                    pos = 2400;
                    break;
                case 60:
                    pos = 3000;
                    break;
                case 120:
                    pos = 6000;
                    break;
            }

            if ("5D".Equals(MainWindow.PlayType))
            {

                ii = (int)Math.Round((UserControlClass.MPPlayer.Position.TotalSeconds * pos), 0);
                ii = (int)Math.Round((ii / 50.0), 0);
            }
            else
            {
                ii = (int)Math.Round((UdpConnect.TimeCode * pos), 0);
                ii = (int)Math.Round((ii / 50.0), 0);
                //label.Content = "TimeCode: " + UdpConnect.strLongTimeCode;
            }
            try
            {
                switch (PlayDOF)
                {
                    case "2DOF":
                        if (Module.actionFile2DOF.Length >= 3 * ii + 2)
                        {
                            this.txtPbVal1.Text = Module.actionFile2DOF[3 * ii].ToString();
                            this.txtPbVal2.Text = Module.actionFile2DOF[3 * ii + 1].ToString();
                            this.txtPbVal3.Text = Module.actionFile2DOF[3 * ii + 2].ToString();

                            pb1.Value = Module.actionFile2DOF[3 * ii];
                            pb2.Value = Module.actionFile2DOF[3 * ii + 1];
                            pb3.Value = Module.actionFile2DOF[3 * ii + 2];
                        }
                        break;
                    case "3DOF":
                        if (Module.actionFile3DOF.Length >= 3 * ii + 2)
                        {
                            this.txtPbVal1.Text = Module.actionFile3DOF[3 * ii].ToString();
                            this.txtPbVal2.Text = Module.actionFile3DOF[3 * ii + 1].ToString();
                            this.txtPbVal3.Text = Module.actionFile3DOF[3 * ii + 2].ToString();

                            pb1.Value = Module.actionFile3DOF[3 * ii];
                            pb2.Value = Module.actionFile3DOF[3 * ii + 1];
                            pb3.Value = Module.actionFile3DOF[3 * ii + 2];
                        }
                        break;
                    case "6DOF":
                        if (Module.actionFile6DOF.Length >= 6 * ii + 5)
                        {
                            this.txtPbVal1.Text = Module.actionFile6DOF[6 * ii].ToString();
                            this.txtPbVal2.Text = Module.actionFile6DOF[6 * ii + 1].ToString();
                            this.txtPbVal3.Text = Module.actionFile6DOF[6 * ii + 2].ToString();
                            this.txtPbVal4.Text = Module.actionFile6DOF[6 * ii + 3].ToString();
                            this.txtPbVal5.Text = Module.actionFile6DOF[6 * ii + 4].ToString();
                            this.txtPbVal6.Text = Module.actionFile6DOF[6 * ii + 5].ToString();

                            pb1.Value = Module.actionFile6DOF[6 * ii];
                            pb2.Value = Module.actionFile6DOF[6 * ii + 1];
                            pb3.Value = Module.actionFile6DOF[6 * ii + 2];
                            pb4.Value = Module.actionFile6DOF[6 * ii + 3];
                            pb5.Value = Module.actionFile6DOF[6 * ii + 4];
                            pb6.Value = Module.actionFile6DOF[6 * ii + 5];
                        }
                        break;
                }

                Boolean[] ev = new Boolean[8];
                Boolean[] cEffect = new Boolean[8];
                Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x99, 0x99));
                Brush brushOn = new SolidColorBrush(Color.FromArgb(0xff, 0x22, 0xAC, 0x38));
                if (Module.effectFile.Length >= 2 * ii + 1)
                {
                    for (int i = 0, n = 1; i < 8; i++)
                    {
                        ev[i] = ((Module.effectFile[2 * ii] & n) == 0 ? false : true);
                        n = n << 1;
                        if (ev[i] == true)
                        {
                            checkBoxEvEffect[i].Background = brushOn;
                        }
                        else
                        {
                            checkBoxEvEffect[i].Background = brush;
                        }
                    }

                    for (int i = 0, n = 1; i < 8; i++)
                    {
                        cEffect[i] = ((Module.effectFile[2 * ii + 1] & n) == 0 ? false : true);
                        n = n << 1;
                        if (cEffect[i] == true)
                        {
                            checkBoxChairEffect[i].Background = brushOn;
                        }
                        else
                        {
                            checkBoxChairEffect[i].Background = brush;
                        }
                    }
                }
                if (Module.shakeFile.Length >= 2 * ii)
                {
                    if (Module.shakeFile[2 * ii] !=0)
                    {
                        checkBoxChairEffect[1].Background = brushOn;
                    }
                    else
                    {
                        checkBoxChairEffect[1].Background = brush;
                    }
                }
            }
            catch
            {
                ClearData();
                //this.txtPbVal1.Text = "0";
                //this.txtPbVal2.Text = "0";
                //this.txtPbVal3.Text = "0";
                //this.txtPbVal4.Text = "0";
                //this.txtPbVal5.Text = "0";
                //this.txtPbVal6.Text = "0";

                //pb1.Value = 0;
                //pb2.Value = 0;
                //pb3.Value = 0;
                //pb4.Value = 0;
                //pb5.Value = 0;
                //pb6.Value = 0;
            }
        }

        /// <summary>
        /// 播放完成响应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MPPlayer_MediaEnded(object sender, EventArgs e)
        {
            //sliderTime.Value = 0;
            //UserControlClass.MPPlayer.Position = new TimeSpan(0, 0, 0);
            //UserControlClass.MPPlayer.Stop();
            //UserControlClass.MSStatus = MediaStatus.Pause;
            //ChangeShowPlay();
            ////重复播放
            if (ModePlayTag == "RepeatPlay" || ModePlayTag == "LoopPlay")
            {
               NextPlayer();
               string Currentname = UserControlClass.FileName;
                //string aa = fullPathName;
                ListPlay(Currentname);
                ChangeShowPlay();
            }
            else
            {
                //UserControlClass.sc2.Close();
                btnStopClickFun();
            }
            //if (PlayControlClient.Equals("TRUE"))
            //{
            //    //bool isConnect = TcpControlClientConnect(UdpInit.GetLocalIP(), "1037");
            //    if (FuseProtocol.Equals("TCP"))
            //    {
            //        bool isConnect = TcpControlClientConnect(FuseIP, FusePort);
            //        ControlStop(isConnect);
            //    }
            //    else
            //    {
            //        FuseStop();
            //    }
            //    RelayControlStopSend();
            //}
            //txtTime.Text = "";
            //Module.timerMovie.Stop();
            //UdpSend.movieStop = true;
            //ClearData();
            //UdpSend.SendReset();
            //btnStopClickFun();
        }

        /// <summary>
        /// 时间轴控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sliderTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (!string.IsNullOrEmpty(UserControlClass.FileName) && IsChangeValue)
                {
                    int PlayerTime = (int)Math.Round(e.NewValue);
                    if (UserControlClass.FileName.Contains(".swf"))
                    {
                    }
                    else
                    {
                        TimeSpan span = new TimeSpan(0, 0, PlayerTime);
                        UserControlClass.MPPlayer.Position = span;
                    }
                }
            }
            catch (Exception ChangeEx)
            {
                System.Windows.MessageBox.Show(ChangeEx.ToString());
            }
        }

        /// <summary>
        /// 时间轴控制（按下）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sliderTime_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsChangeValue = true;
        }

        /// <summary>
        /// 时间轴控制（松开）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sliderTime_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            IsChangeValue = false;
        }


        /// <summary>
        /// 声音轴控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sliderVol_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SSStatus == SoundStatus.Sound)
            {
                UserControlClass.MPPlayer.Volume = e.NewValue;
            }
            else
            {
                sound = e.NewValue;
            }
            SaveVolume();
        }


        /// <summary>
        /// ListView控件鼠标双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PlayType.Equals("5D"))
            {
                int index = ListView.SelectedIndex;
                try
                {
                    if (index != -1)
                    {
                        ScreenJug();
                        memoryPlay = true;
                        ListPlay(ListView.SelectedItem.ToString());
                        UserControlClass.MSStatus = MediaStatus.Pause;
                        //if (PlayControl.Equals("CLIENT"))
                        if (PlayControlClient.Equals("TRUE"))
                        {
                            //bool isConnect = TcpControlClientConnect(UdpInit.GetLocalIP(), "1037");
                            if (FuseProtocol.Equals("TCP"))
                            {
                                bool isConnect = TcpControlClientConnect(FuseIP, FusePort);
                                ControlPlay(isConnect, index);
                            }
                            else
                            {
                                FusePlay(index);
                            }
                            //RelayControlPlaySend();
                            RelayControlPlaySendLight();
                            RelayControlPlaySendLight();
                           // RelayControlPlaySendDoor();
                        }
                    }
                }
                catch(Exception error)
                {
                    Module.WriteLogFile(error.Message);
                }
            }
        }


        /// <summary>
        /// 给选中的菜单项打上勾
        /// </summary>
        /// <param name="modeTag">选中的播放模式</param>
        private void MenuModePlayTick()
        {
            switch (ModePlayTag)
            {
                case "RepeatPlay":
                    listModeSingle.IsChecked = true;
                    listModeDefault.IsChecked = false;
                    listModeLoop.IsChecked = false;
                    break;
                case "DefaultPlay":
                    listModeSingle.IsChecked = false;
                    listModeDefault.IsChecked = true;
                    listModeLoop.IsChecked = false;
                    break;
                case "LoopPlay":
                    listModeSingle.IsChecked = false;
                    listModeDefault.IsChecked = false;
                    listModeLoop.IsChecked = true;
                    break;
            }
            //if (menuLightTag.Equals("close"))
            //{
            //    MenuLight.IsChecked = false;
            //    UdpSend.dataLight = 0;
            //}
            //else
            //{
            //    MenuLight.IsChecked = true;
            //    UdpSend.dataLight = 64;
            //}

        }

        /// <summary>
        /// 窗体初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewLoaded()
        {
            try
            {
                if (InitPlayerPath != null && !String.IsNullOrEmpty(InitPlayerPath.ToString().Trim()))
                {
                    string[] paths = InitPlayerPath.ToString().Split('\\');
                    UserControlClass.FileName = paths[paths.Length - 1];

                    if (UserControlClass.FileName.Contains(".swf"))
                    {
                    }
                    else
                    {
                        Open(InitPlayerPath.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        /// <summary>
        /// 计算播放的时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetPosition()
        {
            if (string.IsNullOrEmpty(UserControlClass.FileName))
            {
                return;
            }
            if (UserControlClass.FileName.Contains(".swf"))
            {
            }
            else
            {
                int num = ((UserControlClass.MPPlayer.Position.Hours * 0xe10) + (UserControlClass.MPPlayer.Position.Minutes * 60)) + UserControlClass.MPPlayer.Position.Seconds;
                string str = SetTime(UserControlClass.MPPlayer.Position.Hours) + ":" + SetTime(UserControlClass.MPPlayer.Position.Minutes) + ":" + SetTime(UserControlClass.MPPlayer.Position.Seconds);
                txtTime.Text = string.Format("{0}/{1}", SetTime(UserControlClass.MPPlayer.Position.Hours) + ":" + SetTime(UserControlClass.MPPlayer.Position.Minutes) + ":" + SetTime(UserControlClass.MPPlayer.Position.Seconds), MediaCountTime);
                sliderTime.Value = num;
                string next = SetTime(UserControlClass.MPPlayer.Position.Hours) + ":" + SetTime(UserControlClass.MPPlayer.Position.Minutes) + ":" + SetTime(UserControlClass.MPPlayer.Position.Seconds);
                ChangeShowPlay();
            }
        }

        /// <summary>
        /// 显示与隐藏播放、暂停按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeShowPlay()
        {
            if (!string.IsNullOrEmpty(UserControlClass.FileName))
            {
                switch (UserControlClass.MSStatus)
                {
                    case MediaStatus.Pause:
                        btnImgPlay.Source = Common.ChangeBitmapToImageSource(Properties.Resources.Icon_Play);
                        break;
                    case MediaStatus.Play:
                        btnImgPlay.Source = Common.ChangeBitmapToImageSource(Properties.Resources.Icon_Pause);
                        break;
                }
            }
        }

        /// <summary>
        /// 列表播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListPlay(string name)
        {
            //创建打开xml文件的流
            FileInfo fileInfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "List.xml");
            if (fileInfo.Exists)//判断文件是否存在
            {
                XmlDocument xmlDoc = new XmlDocument();
                //加载路径的xml文件
                xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "List.xml");
                XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists");
                string lists = xmlNode.InnerText;
                if (!string.IsNullOrEmpty(lists))
                {
                    XmlNodeList nodes = xmlNode.SelectNodes("List");
                    foreach (XmlNode node in nodes)
                    {
                        XmlElement element = (XmlElement)node;
                        if (name.Equals(element["Name"].InnerText))
                        {
                            fullPathName = element["Path"].InnerText;
                        }
                    }
                }
            }
            PCink = PlayCamera.inkMediaPlay;
            ChangeshowInk();
            Microsoft.Win32.OpenFileDialog OpenFile = new Microsoft.Win32.OpenFileDialog();
            OpenFile.FileName = fullPathName;
            UserControlClass.FileName = OpenFile.SafeFileName;
            FileInfo finfo = new FileInfo("" + fullPathName + "");
            if (finfo.Exists)
            {
                UserControlClass.sc2.FInkCanvas_Player.Background = Brushes.White;
                UserControlClass.sc2.FInkCanvas_Player.Children.Clear();
                //播放flash文件判断
                if (UserControlClass.FileName.Contains(".swf"))
                {
                    return;
                }
                else
                {
                    Pause();
                    //播放视频文件
                    if (UserControlClass.MPPlayer.HasVideo)
                    {
                        UserControlClass.MPPlayer.Close();
                    }
                    ListNodeName = UserControlClass.FileName;
                    Open(fullPathName);
                    double time = 0;
                    int intTime = 0;
                    if (memoryPlay)
                    {
                        ListNodeTime = "";
                    }
                    if (!string.IsNullOrEmpty(ListNodeTime))
                    {
                        time = double.Parse(ListNodeTime);
                    }
                    intTime = (int)Math.Round(time);
                    TimeSpan span = new TimeSpan(0, 0, intTime);
                    UserControlClass.MPPlayer.Position = span;
                    UserControlClass.MPPlayer.Play();
                }
            }
            else
            {
                UserControlClass.MPPlayer.Close();
                sliderTime.Value = 0;
                UserControlClass.MSStatus = MediaStatus.Pause;
                UserControlClass.sc2.Close();
                UserControlClass.sc2.FInkCanvas_Player.Background = Brushes.White;
                PCink = PlayCamera.inkMediaPlay;
                ChangeshowInk();
                txtTime.Text = "";
                ChangeShowPlay();
                RemoveFiles();
            }
        }
        /// <summary>
        /// 开始播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Play()
        {
            try
            {
                if (string.IsNullOrEmpty(UserControlClass.FileName))
                {
                    return;
                }
                if (UserControlClass.FileName.Contains(".swf"))
                {
                }
                else
                {
                    //Module.readFile();
                    Module.readDEVFile();
                    UserControlClass.MPPlayer.Play();
                }
                Timing.Start();
            }
            catch (Exception PlayEx)
            {
                throw PlayEx;
            }
        }

        /// <summary>
        /// 暂停播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Pause()
        {
            try
            {
                if (string.IsNullOrEmpty(UserControlClass.FileName))
                {
                    return;
                }
                if (UserControlClass.FileName.Contains(".swf"))
                {
                }
                else
                {
                    UserControlClass.MPPlayer.Pause();
                    Timing.Stop();
                }
                UserControlClass.MSStatus = MediaStatus.Pause;
            }
            catch (Exception PauseEx)
            {
                throw PauseEx;
            }
        }

        /// <summary>
        /// 重新打开播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewOpenPlay()
        {
            Play();
            if ((UserControlClass.FileName.Contains(".mp3")) | (UserControlClass.FileName.Contains(".wma")) | (UserControlClass.FileName.Contains(".wav")) | (UserControlClass.FileName.Contains(".mid")))
            {
                //UserControlClass.sc2.FInkCanvas_Player.Background = Brushes.White;
                //UserControlClass.sc2.FInkCanvas_Camera.Visibility = Visibility.Hidden;
                //UserControlClass.sc2.FInkCanvas_Player.Visibility = Visibility.Visible;
            }
            MediaCountTime = UserControlClass.MPPlayer.NaturalDuration.ToString().Split('.')[0];
            UserControlClass.MPPlayer.Volume = sliderVol.Value;
            UserControlClass.MSStatus = MediaStatus.Play;
            ChangeShowPlay();

            //提取文件时间长度，并且保存在List.xml文件中
            ListNodeLength = UserControlClass.MPPlayer.NaturalDuration.ToString().Split('.')[0];
            if (!string.IsNullOrEmpty(ListNodeName) && !string.IsNullOrEmpty(ListNodePath) && !string.IsNullOrEmpty(ListNodeLength))
            {
                if (!IsExist())
                {
                    AddList();
                }
                SelectXml();
            }
            ListView.SelectedValue = UserControlClass.FileName;
        }

        /// <summary>
        /// 打开播放的文件内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Open(string fn)
        {
            try
            {
                FileInfo finfo = new FileInfo(fn);
                if (finfo.Exists)
                {
                    if (UserControlClass.MPPlayer.HasVideo)
                    {
                        UserControlClass.MPPlayer.Close();
                    }
                    UserControlClass.MPPlayer.Open(new Uri(fn, UriKind.Absolute));
                    VideoDrawing drawing = new VideoDrawing();
                    Rect rect = new Rect(0.0, 0.0, UserControlClass.sc2.FInkCanvas_Player.ActualWidth, UserControlClass.sc2.FInkCanvas_Player.ActualHeight);
                    drawing.Rect = rect;
                    drawing.Player = UserControlClass.MPPlayer;
                    DrawingBrush brush = new DrawingBrush(drawing);
                    UserControlClass.sc2.FInkCanvas_Player.Background = brush;
                    NewOpenPlay();
                }
                else
                {
                    RemoveFiles();
                }
            }
            catch (Exception OpenEx)
            {
                throw OpenEx;
            }
        }

        /// <summary>
        /// 显示与隐藏视频\摄像屏幕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeshowInk()
        {
            switch (PCink)
            {
                case PlayCamera.inkMediaPlay:
                    UserControlClass.sc2.FInkCanvas_Camera.Visibility = Visibility.Hidden;
                    UserControlClass.sc2.FInkCanvas_Player.Visibility = Visibility.Visible;
                    break;
                case PlayCamera.inkCameraPlay:
                    UserControlClass.sc2.FInkCanvas_Player.Visibility = Visibility.Hidden;
                    UserControlClass.sc2.FInkCanvas_Camera.Visibility = Visibility.Visible;
                    break;
            }
        }

        /// <summary>
        /// 显示时间重置方法
        /// </summary>
        /// <param name="currentTime">当前的时间值</param>
        /// <param>格式为：00 的时间</param>
        private string SetTime(int currentTime)
        {
            return currentTime < 10 ? "0" + currentTime.ToString() : currentTime.ToString();
        }

        /// <summary>
        /// 保存选择的播放模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveMode()
        {
            FileInfo finfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "Mode.xml");
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "Mode.xml");
                XmlNode childNode = xmlDoc.SelectSingleNode("Mode");
                XmlElement element = (XmlElement)childNode;
                element["Change"].InnerText = ModePlayTag;
                element["ModeLight"].InnerText = ModeLightTag;
                xmlDoc.Save(MainWindow.playerPath + @"\XML\" + "Mode.xml");
            }
        }

        /// <summary>
        /// 播放模式选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReadMode()
        {
            FileInfo finfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "Mode.xml");
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "Mode.xml");
                XmlNode childNode = xmlDoc.SelectSingleNode("Mode");
                XmlElement element = (XmlElement)childNode;
                ModePlayTag = element["Change"].InnerText;
                ModeLightTag = element["ModeLight"].InnerText;
            }
        }

        /// <summary>
        /// 读取声音大小信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadVolume()
        {
            FileInfo finfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "Volume.xml");
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "Volume.xml");
                XmlNode childNodes = xmlDoc.SelectSingleNode("Volume");
                XmlElement element = (XmlElement)childNodes;
                sliderVol.Value = double.Parse(element["Size"].InnerText);
            }
        }

        /// <summary>
        /// 保存声音大小信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveVolume()
        {
            FileInfo finfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "Volume.xml");
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "Volume.xml");
                XmlNode childNodes = xmlDoc.SelectSingleNode("Volume");
                XmlElement element = (XmlElement)childNodes;
                element["Size"].InnerText = sliderVol.Value.ToString().Trim();
                xmlDoc.Save(MainWindow.playerPath + @"\XML\" + "Volume.xml");
            }
        }

        /// <summary>
        /// 添加保存List.xml信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddList()
        {
            FileInfo finfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "List.xml");
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "List.xml");
                XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists");

                XmlNode ListNode = xmlDoc.CreateElement("List");

                XmlNode NameNode = xmlDoc.CreateElement("Name");
                XmlText NameText = xmlDoc.CreateTextNode(ListNodeName);
                NameNode.AppendChild(NameText);

                XmlNode PathNode = xmlDoc.CreateElement("Path");
                XmlText PathText = xmlDoc.CreateTextNode(ListNodePath);
                PathNode.AppendChild(PathText);

                XmlNode LengthNode = xmlDoc.CreateElement("Length");
                XmlText LengthText = xmlDoc.CreateTextNode(ListNodeLength);
                LengthNode.AppendChild(LengthText);

                XmlNode TimeNode = xmlDoc.CreateElement("Time");
                XmlText TimeText = xmlDoc.CreateTextNode(ListNodeTime);
                TimeNode.AppendChild(TimeText);

                ListNode.AppendChild(NameNode);
                ListNode.AppendChild(PathNode);
                ListNode.AppendChild(LengthNode);
                ListNode.AppendChild(TimeNode);

                xmlNode.AppendChild(ListNode);

                xmlDoc.Save(MainWindow.playerPath + @"\XML\" + "List.xml");
            }
        }

        /// <summary>
        /// 添加保存List.xml信息支持多选
        /// </summary>
        /// <param name="file"></param>
        private void AddList(int file)
        {
            FileInfo finfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "List.xml");
            if (finfo.Exists)
            {
                for (int i = 0; i < file; i++)
                {

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "List.xml");
                    XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists");

                    XmlNode ListNode = xmlDoc.CreateElement("List");

                    XmlNode NameNode = xmlDoc.CreateElement("Name");
                    XmlText NameText = xmlDoc.CreateTextNode(ListNodeNameArr[i]);
                    NameNode.AppendChild(NameText);

                    XmlNode PathNode = xmlDoc.CreateElement("Path");
                    XmlText PathText = xmlDoc.CreateTextNode(ListNodePathArr[i]);
                    PathNode.AppendChild(PathText);

                    ListNode.AppendChild(NameNode);
                    ListNode.AppendChild(PathNode);
                    xmlNode.AppendChild(ListNode);
                    xmlDoc.Save(MainWindow.playerPath + @"\XML\" + "List.xml");
                }

            }
        }
        /// <summary>
        /// 判定文件是否存在于List.xml文件中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public bool IsExist()
        {
            FileInfo finfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "List.xml");
            if (finfo.Exists)
            {
                XPathDocument xPath = new XPathDocument(MainWindow.playerPath + @"\XML\" + "List.xml");
                XPathNavigator navigator = xPath.CreateNavigator();
                XPathNodeIterator iterator = navigator.Select("/Lists/List[Name='" + ListNodeName + "']");
                if (iterator.Count == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 查询List.xml文件，并于播放列表显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectXml()
        {
            if (PlayType.Equals("5D"))
            {
                ListView.Items.Clear();
                FileInfo finfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "List.xml");
                if (finfo.Exists)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "List.xml");
                    XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists");
                    string lists = xmlNode.InnerText;
                    if (!string.IsNullOrEmpty(lists))
                    {
                        XmlNodeList nodes = xmlNode.SelectNodes("List");
                        foreach (XmlNode node in nodes)
                        {
                            XmlElement element = (XmlElement)node;
                            string[] subItems = new string[3];
                            subItems[0] = element["Name"].InnerText;
                            subItems[1] = element["Path"].InnerText;
                            System.Windows.Forms.ListViewItem listViewItem = new System.Windows.Forms.ListViewItem(subItems);
                            string listViewItems = listViewItem.ToString().Substring(15, listViewItem.ToString().Length - 16);
                            ListView.Items.Add(listViewItems);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 定义播放下一部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private string NextPlayer()
        {
            FileInfo fileInfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "List.xml");
            if (fileInfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "List.xml");
                XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists");
                string lists = xmlNode.InnerText;
                if (!string.IsNullOrEmpty(lists) && !string.IsNullOrEmpty(UserControlClass.FileName))
                {
                    XmlNodeList nodes = xmlNode.SelectNodes("List");
                    int i = 0;
                    foreach (XmlNode node in nodes)
                    {
                        XmlElement element = (XmlElement)node;
                        i++;
                        if (UserControlClass.FileName.Equals(element["Name"].InnerText))
                        {
                            number = i;
                        }
                    }
                    string name = "";
                    if (ModePlayTag.Equals("RepeatPlay"))
                    {
                        name = ListView.Items[number - 1].ToString();
                    }
                    else
                        if (ModePlayTag.Equals("OrderPlay"))
                    {
                        if (ListView.Items.Count == number)
                        {
                            name = "";
                        }
                        else
                        {
                            name = ListView.Items[number].ToString();
                        }
                    }
                    else
                            if (ModePlayTag.Equals("LoopPlay"))
                    {
                        if (ListView.Items.Count == number)
                        {
                            name = ListView.Items[0].ToString();
                        }
                        else
                        {
                            name = ListView.Items[number].ToString();
                        }
                    }
                    if (name == "")
                    {
                        fullPathName = "";
                    }
                    else
                    {
                        foreach (XmlNode node in nodes)
                        {
                            XmlElement element = (XmlElement)node;
                            if (name.Equals(element["Name"].InnerText))
                            {
                                fullPathName = element["Path"].InnerText;
                            }
                        }
                        UserControlClass.FileName = fullPathName;
                    }
                }

            }
            return fullPathName;
        }

        /// <summary>
        /// 定义添加文件（按钮）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFiles()
        {
            Microsoft.Win32.OpenFileDialog OpenFile = new Microsoft.Win32.OpenFileDialog();
            string from = "Media Files(*.wmv;*.avi;*.mp4;*.mkv;*.rmvb)|*.wmv;*.avi;*.mp4;*.mkv;*.rmvb;*.mp3;*.wma;*.wav;*.mid|Flash Files(*.swf)|*.swf|All Files(*.*)|*.*";
            OpenFile.FileName = "";
            OpenFile.Filter = from;
            OpenFile.Multiselect = true;
            OpenFile.ShowDialog();
            if (OpenFile.CheckFileExists && !string.IsNullOrEmpty(OpenFile.FileName))
            {
                ListNodeName = OpenFile.SafeFileName;
                ListNodePath = OpenFile.FileName;
                ListNodeNameArr = OpenFile.SafeFileNames;
                ListNodePathArr = OpenFile.FileNames;
                if (!IsExist())
                {
                    AddList(ListNodeNameArr.Length);
                    //AddList();
                }
                SelectXml();
                if (UserControlClass.FileName != null)
                {
                    ListView.SelectedValue = UserControlClass.FileName;
                }
            }
        }

        /// <summary>
        /// 定义删除列表选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveFiles()
        {
            int index = ListView.SelectedIndex;
            if (index != -1)
            {
                string name = ListView.SelectedItem.ToString();
                FileInfo finfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "List.xml");
                if (finfo.Exists)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "List.xml");
                    XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists");
                    XmlNodeList nodeList = xmlNode.SelectNodes("List");
                    foreach (XmlNode node in nodeList)
                    {
                        if (node.SelectSingleNode("Name").InnerText.Equals(name))
                        {
                            xmlNode.RemoveChild(node);
                            xmlDoc.Save(MainWindow.playerPath + @"\XML\" + "List.xml");
                            SelectXml();
                        }
                    }
                }
            }
            //聚焦到listview上
            if (UserControlClass.FileName != null)
            {
                ListView.SelectedValue = UserControlClass.FileName;
            }
        }

        private void RemoveFileFilmList()
        {
            //int index = ListView.SelectedIndex;
            //memberData[index].MovieName = "";
            for (int i = 0; i < memberData.Count; i++)
            {
                memberData[i].MovieName = "";
                memberData[i].FullMovieName = "";
            }
            SaveFilmList();
            SelectXmlMovie();
        }

        /// <summary>
        /// 定义删除列表所有项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveAllFiles()
        {
            //创建filestream对象流，读取文件
            FileInfo finfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "List.xml");
            if (finfo.Exists)
            {
                //实例一个xml文档对象
                XmlDocument xmlDoc = new XmlDocument();
                //加载url路径里面的文件
                xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "List.xml");
                //读取xml里面的lists单个节点
                XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists");
                //读取xml里面的list节点集合对象
                XmlNodeList nodeList = xmlNode.SelectNodes("List");
                foreach (XmlNode node in nodeList)
                {
                    //移除子节点
                    xmlNode.RemoveChild(node);
                    //将xml保存到指定的文件
                    xmlDoc.Save(MainWindow.playerPath + @"\XML\" + "List.xml");
                    SelectXml();
                }
            }
        }
        /// <summary>
        /// 判断是否有第二个屏
        /// </summary>
        private void ScreenJug()
        {
            if (Play5DPicture.Equals("0"))
            {
                System.Windows.Forms.Screen[] sc;
                sc = System.Windows.Forms.Screen.AllScreens;
                if (UserControlClass.sc2 == null || UserControlClass.sc2.IsVisible == false)
                {
                    UserControlClass.sc2 = new SecondScreen();
                    //把当前的player对象传给公共类
                    UserControlClass.sc2.Player = this;
                    UserControlClass.sc2.Show();
                }
                else
                {
                    //把当前的player对象传给公共类
                    UserControlClass.sc2.Player = this;
                    UserControlClass.sc2.Activate();
                    UserControlClass.sc2.WindowState = WindowState.Normal;
                }
                //获取当前所有显示器的数组
                //如果有多个屏幕，就定义secondwindow的位置，让他在第二个屏幕全屏显示   
                if (sc.Length == 1)
                {
                    this.WindowState = WindowState.Minimized;
                }
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ListView.SelectedItem != null)
                {
                    fullPathName = GetMovieFullPathName(ListView.SelectedItem.ToString());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取当前选中影片全路径名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetMovieFullPathName(string name)
        {
            //创建打开xml文件的流
            FileInfo fileInfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "List.xml");
            if (fileInfo.Exists)//判断文件是否存在
            {
                XmlDocument xmlDoc = new XmlDocument();
                //加载路径的xml文件
                xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "List.xml");
                XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists");
                string lists = xmlNode.InnerText;
                if (!string.IsNullOrEmpty(lists))
                {
                    XmlNodeList nodes = xmlNode.SelectNodes("List");
                    foreach (XmlNode node in nodes)
                    {
                        XmlElement element = (XmlElement)node;
                        if (name.Equals(element["Name"].InnerText))
                        {
                            fullPathName = element["Path"].InnerText;
                        }
                    }
                }
                return fullPathName;
            }
            return null;
        }

        /// <summary>
        /// 点击播放按钮触发函数
        /// </summary>
        private void btnPlayClickFun()
        {
            int index = ListView.SelectedIndex;
            if (ListView.SelectedValue != null)
            {
                string filename = ListView.SelectedValue.ToString();
                if (UserControlClass.FileName != filename)
                {
                    if (index != -1)
                    {
                        ScreenJug();
                        memoryPlay = true;
                        ListPlay(ListView.SelectedItem.ToString());
                        UserControlClass.MSStatus = MediaStatus.Pause;
                        //if (PlayControl.Equals("CLIENT"))
                        if (PlayControlClient.Equals("TRUE"))
                        {
                            //bool isConnect = TcpControlClientConnect(UdpInit.GetLocalIP(), "1037");
                            if (FuseProtocol.Equals("TCP"))
                            {
                                bool isConnect = TcpControlClientConnect(FuseIP, FusePort);
                                ControlPlay(isConnect, index);
                            }
                            else
                            {
                                FusePlay(index);
                            }
                            //RelayControlPlaySend();
                            RelayControlPlaySendLight();
                            RelayControlPlaySendLight();
                            //RelayControlPlaySendDoor();
                        }
                    }
                }
                else
                {
                    if (UserControlClass.MSStatus == MediaStatus.Pause)
                    {
                        UserControlClass.MSStatus = MediaStatus.Play;
                        //if (UserControlClass.FileName == filename && PlayControl.Equals("CLIENT"))
                        if (UserControlClass.FileName == filename && PlayControlClient.Equals("TRUE"))
                        {
                            //bool isConnect = TcpControlClientConnect(UdpInit.GetLocalIP(), "1037");
                            if (FuseProtocol.Equals("TCP"))
                            {
                                bool isConnect = TcpControlClientConnect(FuseIP, FusePort);
                                ControlPlayAgain(isConnect);
                            }
                            else
                            {
                                FusePlayAgain();
                            }
                        }
                        if (UserControlClass.sc2.WindowState == WindowState.Minimized)
                        {
                            UserControlClass.sc2.Activate();
                            UserControlClass.sc2.WindowState = WindowState.Normal;
                        }
                        Play();
                    }
                    else
                    {
                        UserControlClass.MSStatus = MediaStatus.Pause;
                        Pause();
                        //if (PlayControl.Equals("CLIENT"))
                        if (PlayControlClient.Equals("TRUE"))
                        {
                            //bool isConnect = TcpControlClientConnect(UdpInit.GetLocalIP(), "1037");
                            if (FuseProtocol.Equals("TCP"))
                            {
                                bool isConnect = TcpControlClientConnect(FuseIP, FusePort);
                                ControlPause(isConnect);
                            }
                            else
                            {
                                FusePause();
                            }
                        }
                    }
                }
                ChangeShowPlay();
            }
        }

        /// <summary>
        /// 点击停止按钮触发函数
        /// </summary>
        private void btnStopClickFun()
        {
            if (!string.IsNullOrEmpty(UserControlClass.FileName))
            {
                if (UserControlClass.FileName.Contains(".swf"))
                {
                }
                else
                {
                    UserControlClass.MSStatus = MediaStatus.Pause;
                    ChangeShowPlay();
                    //UserControlClass.MPPlayer.Close();
                    UserControlClass.MPPlayer.Stop();
                    sliderTime.Value = 0;
                    UserControlClass.FileName = null;
                    UserControlClass.sc2.Close();
                    UserControlClass.sc2.FInkCanvas_Player.Background = Brushes.White;
                    PCink = PlayCamera.inkMediaPlay;
                    ChangeshowInk();
                    txtTime.Text = "";
                    //if (PlayControl.Equals("CLIENT"))
                    if (PlayControlClient.Equals("TRUE"))
                    {
                        //bool isConnect = TcpControlClientConnect(UdpInit.GetLocalIP(), "1037");
                        if (FuseProtocol.Equals("TCP"))
                        {
                            bool isConnect = TcpControlClientConnect(FuseIP, FusePort);
                            ControlStop(isConnect);
                        }
                        else
                        {
                            FuseStop();
                        }
                        RelayControlStopSend();
                    }
                }
                if (Module.timerMovie != null)
                {
                    Module.timerMovie.Stop();
                    UdpSend.movieStop = true;
                    ClearData();
                    UdpSend.SendReset();
                }
            }


        }

        #endregion

        #region 定时器初始化与定时器响应方法

        /// <summary>
        /// 排片定时器初始化
        /// </summary>

        private void DTimerInit()
        {
            //定义系统时间计时器
            //初始化按指定优先级处理计时器事件
            DTimer = new DispatcherTimer(DispatcherPriority.Normal);
            //设置计时器的时间间隔
            DTimer.Interval = TimeSpan.FromMilliseconds(100);
            //超过计时器间隔时发生
            DTimer.Tick += new EventHandler(timer_Tick);
            //系统时间显示，线程开始
            DTimer.Start();

        }

        private void TimerFilmInit()
        {
            timerFilm = new DispatcherTimer();
            timerFilm.Interval = TimeSpan.FromSeconds(5);
            timerFilm.Tick += new EventHandler(TimerFilm_Tick);
            timerFilm.Start();

            //测试排片列表循环播放
            //for (int i = 0; i < memberData.Count; i++)
            //{
            //    if (memberData[i].MovieName != "")
            //    {
            //        movieListIndexs++;
            //    }
            //}
            //timerFilm.Interval = TimeSpan.FromSeconds(0.01);
            //timerFilm.Tick += new EventHandler(TimerFilmTest_Tick);
            //timerFilm.Start();
        }

        /// <summary>
        /// 排片响应方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerFilm_Tick(object sender, EventArgs e)
        {
            if ("4DM".Equals(PlayType))
            {
                string s = DateTime.Now.ToShortTimeString().ToString();
                //compareDateTime(FilmSetting.memberData[1].Start);
                for (int i = 0; i < 10; i++)
                {
                    if (memberData[i] != null)
                    {
                        //if (s.Equals(FilmSetting.memberData[i].Start))
                        if (compareDateTime(memberData[i].Start, memberData[i].End))
                        {
                            if (PlayLanguage.Equals("CN"))
                            {

                                labFilm1.Content = "当前影片：" + memberData[i].MovieName;
                                if (i < 9)
                                {
                                    labFilm2.Content = "下一场影片：" + memberData[i + 1].MovieName;
                                }
                            }
                            else
                            {
                                labFilm1.Content = "Current Movie:" + memberData[i].MovieName;
                                if (i < 9)
                                {
                                    labFilm2.Content = "Next Movie:" + memberData[i + 1].MovieName;
                                }
                            }
                            Module.DEVFile(memberData[i].FullMovieName);
                            //Module.DEVFile(memberData[i].FullMovieName,memberData[i].MovieName);
                        }
                        if (i < 9)
                        {
                            if (compareDateTime(memberData[i].End, memberData[i + 1].Start)||compareDateTime(memberData[0].Start))
                            {
                                labFilm1.Content = "";
                                labFilm2.Content = "";
                                Module.actionFile2DOF = null;
                                Module.actionFile3DOF = null;
                                Module.shakeFile = null;
                                Module.effectFile = null;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 排片响应方法(测试)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerFilmTest_Tick(object sender, EventArgs e)
        {
            if ("4DM".Equals(PlayType))
            {
                if (memberData[movieListIndex] != null)
                {
                    if (UdpConnect.TimeCode == 1)
                    {
                        int a = memberData.Count;
                        labFilm1.Content = "当前影片：" + memberData[movieListIndex].MovieName;
                        //Module.readDefultFile(memberData[movieListIndex].FullMovieName);
                        Module.DEVFile(memberData[movieListIndex].FullMovieName);
                        Thread.Sleep(100);
                        movieListIndex = movieListIndex + 1;
                        if (movieListIndex == movieListIndexs)
                        {
                            movieListIndex = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 判断定时器初始化
        /// </summary>
        private void TimerJudgeInit()
        {
            timerJudge = new DispatcherTimer();
            timerJudge.Interval = TimeSpan.FromSeconds(0.01);    //定时器周期为10ms 
            timerJudge.Tick += new EventHandler(TimerJudge_Tick);
            timerJudge.Start();
        }

        /// <summary>
        /// 判断软件当前的状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerJudge_Tick(object sender, EventArgs e)
        {
            if (isSleep == false)
            {
                Thread.Sleep(2000);
                isSleep = true;
            }

            if (UdpConnect.connectFlag == false)  //未与中控板连接    
            {
                if ("CN".Equals(PlayLanguage))
                {
                    labConnect.Content = "未连接";
                }
                else
                {
                    labConnect.Content = "Unconnected";
                }
                imgConnect.Source = Common.ChangeBitmapToImageSource(Properties.Resources.Icon_UnConnect);
                LockSoftWare();
            }
            else      //与中控板已连接
            {
                if (UdpConnect.isRegistered == false)  //软件到期或者未注册        
                {
                    if ("CN".Equals(PlayLanguage))
                    {
                        labConnect.Content = "未注册";
                    }
                    else
                    {
                        labConnect.Content = "UnRegistered";
                    }
                    imgConnect.Source = Common.ChangeBitmapToImageSource(Properties.Resources.Icon_UnRegister);
                    LockSoftWare();
                }
                else  //软件正常打开            
                {
                    OpenSoftWare();
                    if (isReset == 0)
                    {
                        UdpSend.SendReset();
                        TcpRelayControlClientInit();
                        isReset = 1;
                    }
                    imgConnect.Source = Common.ChangeBitmapToImageSource(Properties.Resources.Icon_Connected);
                    if ("CN".Equals(PlayLanguage))
                    {
                        labConnect.Content = "连接成功";
                    }
                    else
                    {
                        labConnect.Content = "Connected";
                    }
                    if (Module.hintShow == true)
                    {
                        btnDateTips.Visibility = Visibility.Visible;
                        if ("CN".Equals(PlayLanguage))
                        {
                            btnDateTips.Content = "提示：软件还有" + Module.deadlineDay + "天到期";
                        }
                        else
                        {
                            btnDateTips.Content = "Tips: The software expires in " + Module.deadlineDay + " days";
                        }
                    }
                    if ("4DM".Equals(PlayType))
                    {
                        labLTC.Content = "TimeCode: " + UdpConnect.strLongTimeCode;
                        if (UdpConnect.strLongTimeCode != null)
                        {
                            showTotalData();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 调试定时器初始化
        /// </summary>
        private void TimerDebugInit()
        {
            timerDebug = new DispatcherTimer();
            timerDebug.Interval = TimeSpan.FromSeconds(0.05);
            timerDebug.Tick += new EventHandler(TimerDebug_Tick);
            timerDebug.Start();
        }

        /// <summary>
        /// 调试定时器方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerDebug_Tick(object sender, EventArgs e)
        {
            byte eEffect = 0;
            byte cEffect = 0;
            byte dataNumOne = 0;                  //1号缸的数据
            byte dataNumTwo = 0;                  //2号缸的数据 
            byte dataNumThree = 0;                //3号缸的数据

            byte[] data1;
            byte[] array;          //data+addr+len 
            byte[] Data;           //最终发送的数据

            dataNumOne = (byte)(dataNum[2] * MainWindow.PlayHeight / 100);
            dataNumTwo = (byte)(dataNum[1] * MainWindow.PlayHeight / 100);
            dataNumThree = (byte)(dataNum[0] * MainWindow.PlayHeight / 100);

            for (int i = 0; i < 8; i++)
            {
                eEffect += dataEvEffect[i];
                //label9.Content = eEffect.ToString();
            }
            // eEffect = dataEvEffect[0];
            for (int i = 0; i < 8; i++)
            {
                cEffect += dataChairEffect[i];
            }
            if (dataEvEffect[0] == 0)
            {
                Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x93, 0x93, 0x93));
                btnLightning.Background = brush;
            }
            else
            {
                btnLightning.Background = Brushes.Cyan;
            }

            Debug.WriteLine(eEffect.ToString());

            data1 = new byte[12] { dataNumOne, dataNumTwo, dataNumThree, 0, 0, 0, dataRate, dataFrame, cEffect, eEffect, 0, 0 };
            byte[] data = new byte[data1.Length + 34];
            data1.CopyTo(data,0);
            Module.DMXDataLen = new byte[8] { 0x0a, 0x01, 0x01, 0x01, 0x01, 0x0a, 0x01, 0x01 };
            Module.DMXDataLen.CopyTo(data,12);
            Module.DMXLightning.CopyTo(data, 20);
            Module.DMXWind.CopyTo(data, 30);
            Module.DMXBubble.CopyTo(data, 31);
            Module.DMXFog.CopyTo(data, 32);
            Module.DMXFire.CopyTo(data, 33);
            Module.DMXSnow.CopyTo(data, 34);
            Module.DMXLaser.CopyTo(data, 44);
            Module.DMXRain.CopyTo(data, 45);
            array = Protocol.ModbusUdp.ArrayAdd(0, (ushort)data.Length, data);
            Data = Protocol.ModbusUdp.MBReqWrite(array);
            UdpSend.UdpSendData(Data, Data.Length, UdpInit.RemotePoint);
        }

        #endregion

        #region 控制台控制投影机
        private void Projector_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = sender as CheckBox;
            int tag = Convert.ToInt32(checkbox.Tag);
            string ip1 = txtIpProjector1.Text;
            string port1 = txtPortProjector1.Text;
            string ip2 = txtIpProjector2.Text;
            string port2 = txtPortProjector2.Text;
            string ip3 = txtIpProjector3.Text;
            string port3 = txtPortProjector3.Text;
            string ip4 = txtIpProjector4.Text;
            string port4 = txtPortProjector4.Text;
            string ip5 = txtIpProjector5.Text;
            string port5 = txtPortProjector5.Text;
            string ip6 = txtIpProjector6.Text;
            string port6 = txtPortProjector6.Text;
            string ip7 = txtIpProjector7.Text;
            string port7 = txtPortProjector7.Text;
            string ip8 = txtIpProjector8.Text;
            string port8 = txtPortProjector8.Text;
            switch (tag)
            {
                case 1:
                    bool ischeckProjector1;
                    if (checkProjector1.IsChecked == true)
                    {
                        ischeckProjector1 = true;
                    }
                    else
                    {
                        ischeckProjector1 = false;
                    }
                    Thread th1 = new Thread(() =>
                    {
                        if (ischeckProjector1 == true)
                        {
                            bool isConnect = TcpClientConnect(ip1, port1,1);                            
                            OpenProjector(isConnect);
                        }
                        else
                        {
                            bool isConnect = TcpClientConnect(ip1, port1,1);
                            CloseProjector(isConnect);
                        }

                    });
                    th1.Start();
                    break;
                case 2:
                    //if (checkProjector2.IsChecked == true)
                    //{
                    //    bool isConnect = TcpClientConnect(txtIpProjector2.Text, txtPortProjector2.Text);
                    //    OpenProjector(isConnect);
                    //}
                    //else
                    //{
                    //    bool isConnect = TcpClientConnect(txtIpProjector2.Text, txtPortProjector2.Text);
                    //    CloseProjector(isConnect);
                    //}
                    bool ischeckProjector2;
                    if (checkProjector2.IsChecked == true)
                    {
                        ischeckProjector2 = true;
                    }
                    else
                    {
                        ischeckProjector2 = false;
                    }
                    Thread th2 = new Thread(() =>
                    {
                        if (ischeckProjector2 == true)
                        {
                            bool isConnect = TcpClientConnect(ip2, port2,2);
                            OpenProjector(isConnect);
                        }
                        else
                        {
                            bool isConnect = TcpClientConnect(ip2, port2,2);
                            CloseProjector(isConnect);
                        }
                    });
                    th2.Start();
                    break;
                case 3:
                    bool ischeckProjector3;
                    if (checkProjector3.IsChecked == true)
                    {
                        ischeckProjector3 = true;
                    }
                    else
                    {
                        ischeckProjector3 = false;
                    }
                    Thread th3 = new Thread(() =>
                    {
                        if (ischeckProjector3 == true)
                        {
                            bool isConnect = TcpClientConnect(ip3, port3,3);
                            OpenProjector(isConnect);
                        }
                        else
                        {
                            bool isConnect = TcpClientConnect(ip3, port3,3);
                            CloseProjector(isConnect);
                        }
                    });
                    th3.Start();
                    break;
                case 4:
                    bool ischeckProjector4;
                    if (checkProjector4.IsChecked == true)
                    {
                        ischeckProjector4 = true;
                    }
                    else
                    {
                        ischeckProjector4 = false;
                    }
                    Thread th4 = new Thread(() =>
                    {
                        if (ischeckProjector4 == true)
                        {
                            bool isConnect = TcpClientConnect(ip4, port4,4);
                            OpenProjector(isConnect);
                        }
                        else
                        {
                            bool isConnect = TcpClientConnect(ip4, port4,4);
                            CloseProjector(isConnect);
                        }
                    });
                    th4.Start();
                    break;
                case 5:
                    bool ischeckProjector5;
                    if (checkProjector5.IsChecked == true)
                    {
                        ischeckProjector5 = true;
                    }
                    else
                    {
                        ischeckProjector5 = false;
                    }
                    Thread th5 = new Thread(() =>
                    {
                        if (ischeckProjector5 == true)
                        {
                            bool isConnect = TcpClientConnect(ip5, port5,5);
                            OpenProjector(isConnect);
                        }
                        else
                        {
                            bool isConnect = TcpClientConnect(ip5, port5,5);
                            CloseProjector(isConnect);
                        }
                    });
                    th5.Start();
                    break;
                case 6:
                    bool ischeckProjector6;
                    if (checkProjector6.IsChecked == true)
                    {
                        ischeckProjector6 = true;
                    }
                    else
                    {
                        ischeckProjector6 = false;
                    }
                    Thread th6 = new Thread(() =>
                    {
                        if (ischeckProjector6 == true)
                        {
                            bool isConnect = TcpClientConnect(ip6, port6,6);
                            OpenProjector(isConnect);
                        }
                        else
                        {
                            bool isConnect = TcpClientConnect(ip6, port6,6);
                            CloseProjector(isConnect);
                        }
                    });
                    th6.Start();
                    break;
                case 7:
                    bool ischeckProjector7;
                    if (checkProjector7.IsChecked == true)
                    {
                        ischeckProjector7 = true;
                    }
                    else
                    {
                        ischeckProjector7 = false;
                    }
                    Thread th7 = new Thread(() =>
                    {
                        if (ischeckProjector7 == true)
                        {
                            bool isConnect = TcpClientConnect(ip7, port7,7);
                            OpenProjector(isConnect);
                        }
                        else
                        {
                            bool isConnect = TcpClientConnect(ip7, port7,7);
                            CloseProjector(isConnect);
                        }
                    });
                    th7.Start();
                    break;
                case 8:
                    bool ischeckProjector8;
                    if (checkProjector8.IsChecked == true)
                    {
                        ischeckProjector8 = true;
                    }
                    else
                    {
                        ischeckProjector8 = false;
                    }
                    Thread th8 = new Thread(() =>
                    {
                        if (ischeckProjector8 == true)
                        {
                            bool isConnect = TcpClientConnect(ip8, port8,8);
                            OpenProjector(isConnect);
                        }
                        else
                        {
                            bool isConnect = TcpClientConnect(ip8, port8,8);
                            CloseProjector(isConnect);
                        }
                    });
                    th8.Start();
                    break;
            }
        }

        private void CheckProjector_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = sender as CheckBox;
            int tag = Convert.ToInt32(checkbox.Tag);
            string[] ip = new string[8];
            string[] port = new string[8];
            int projectorCount;
            Int32.TryParse(txtProjectorCount.Text, out projectorCount);
            if (projectorCount <= 0 || projectorCount > 8)
            {
                projectorCount = 1;
            }
            for (int i = 0; i < projectorCount; i++)
            {
                ip[i] = textBoxIPProjector[i].Text;
                port[i] = textBoxPortProjector[i].Text;
            }
            //string ip1 = txtIpProjector1.Text;
            //string port1 = txtPortProjector1.Text;
            //string ip2 = txtIpProjector2.Text;
            //string port2 = txtPortProjector2.Text;
            //string ip3 = txtIpProjector3.Text;
            //string port3 = txtPortProjector3.Text;
            //string ip4 = txtIpProjector4.Text;
            //string port4 = txtPortProjector4.Text;
            //string ip5 = txtIpProjector5.Text;
            //string port5 = txtPortProjector5.Text;
            //string ip6 = txtIpProjector6.Text;
            //string port6 = txtPortProjector6.Text;
            //string ip7 = txtIpProjector7.Text;
            //string port7 = txtPortProjector7.Text;
            //string ip8 = txtIpProjector8.Text;
            //string port8 = txtPortProjector8.Text;
            bool[] isConnect = new bool[8];
            bool[] isConnectClose = new bool[8];
            bool ischeckProjector;
            if (checkProjectorAll.IsChecked == true)
            {
                ischeckProjector = true;
            }
            else
            {
                ischeckProjector = false;
            }
            Thread thAll = new Thread(() =>
            {
                if (ischeckProjector == true)
                {
                    //isConnect[0] = TcpClientConnect(ip1, port1);
                    //OpenProjector(isConnect[0]);
                    ////checkProjector1.IsChecked = true;
                    //isConnect[1] = TcpClientConnect(ip2, port2);
                    //OpenProjector(isConnect[1]);
                    //isConnect[2] = TcpClientConnect(ip3, port3);
                    //OpenProjector(isConnect[2]);
                    //isConnect[3] = TcpClientConnect(ip4, port4);
                    //OpenProjector(isConnect[3]);
                    //isConnect[4] = TcpClientConnect(ip5, port5);
                    //OpenProjector(isConnect[4]);
                    //isConnect[5] = TcpClientConnect(ip6, port6);
                    //OpenProjector(isConnect[5]);
                    //isConnect[6] = TcpClientConnect(ip7, port7);
                    //OpenProjector(isConnect[6]);
                    //isConnect[7] = TcpClientConnect(ip8, port8);
                    //OpenProjector(isConnect[7]);
                    for (int i = 0; i < projectorCount; i++)
                    {
                        isConnect[i] = TcpClientConnect(ip[i], port[i],i+1);
                        OpenProjector(isConnect[i]);
                    }

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        for (int i = 0; i < projectorCount; i++)
                        {
                            if (isConnect[i] == true)
                            {
                                checkBoxProjector[i].IsChecked = true;
                            }
                        }
                    }));
                   
                }
                else
                {
                    //isConnectClose[0] = TcpClientConnect(ip1, port1);
                    //CloseProjector(isConnectClose[0]);
                    //isConnectClose[1] = TcpClientConnect(ip2, port2);
                    //CloseProjector(isConnectClose[1]);
                    //isConnectClose[2] = TcpClientConnect(ip3, port3);
                    //CloseProjector(isConnectClose[2]);
                    //isConnectClose[3] = TcpClientConnect(ip4, port4);
                    //CloseProjector(isConnectClose[3]);
                    //isConnectClose[4] = TcpClientConnect(ip5, port5);
                    //CloseProjector(isConnectClose[4]);
                    //isConnectClose[5] = TcpClientConnect(ip6, port6);
                    //CloseProjector(isConnectClose[5]);
                    //isConnectClose[6] = TcpClientConnect(ip7, port7);
                    //CloseProjector(isConnectClose[6]);
                    //isConnectClose[7] = TcpClientConnect(ip8, port8);
                    //CloseProjector(isConnectClose[7]);
                    for (int i = 0; i < projectorCount; i++)
                    {
                        isConnectClose[i] = TcpClientConnect(ip[i], port[i],i+1);
                        CloseProjector(isConnectClose[i]);
                    }
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        for (int i = 0; i < projectorCount; i++)
                        {
                            if (isConnectClose[i] == true)
                            {
                                checkBoxProjector[i].IsChecked = false;
                            }
                        }
                    }));
                }
            });
            thAll.Start();
          
        }

        private bool TcpClientConnect(string ip, string port,int index)
        {
            try
            {
                tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                tcpClient.Connect(UdpInit.transformIP(ip, port));
                return true;
            }
            catch (Exception e)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    checkBoxProjector[index - 1].IsChecked = false;
                }));
                MessageBox.Show("连接投影机" + index + "有误");
                Module.WriteLogFile("连接投影机" + index + "有误" + "\r\n" + e.Message);
                return false;
            }
        }

        private void TcpClientClose()
        {
            if (tcpClient.Connected)
            {
                tcpClient.Close();
            }
        }

        private void OpenProjector(bool isConnect)
        {
            if (isConnect)
            {
                //int brand = comboBoxBrand.SelectedIndex;      选中哪个品牌的投影机
                byte[] data1 = { 0x30, 0x30, 0x50, 0x4F, 0x4E, 0x0D };
               // byte[] data1 = {0xFE,0x0F,0x00,0x00,0x00,0x08,0x01,0xFF,0xF1,0xD1};
                tcpClient.Send(data1);
                TcpClientClose();
            }
        }

        private void CloseProjector(bool isConnect)
        {
            if (isConnect)
            {
                byte[] data2 = { 0x30, 0x30, 0x50, 0x4F, 0x46, 0x0D };
                //byte[] data2 = { 0xFE, 0x0F, 0x00, 0x00, 0x00, 0x08, 0x01, 0x00, 0xB1, 0x91 };
                tcpClient.Send(data2);
                TcpClientClose();
            }
        }

        #endregion

        #region TCP服务器
        /// <summary>
        /// 开始监听
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyServer()
        {
            //if (PlayControl.Equals("SERVER"))
            if (PlayControlServer.Equals("TRUE"))
            {
                //当点击开始监听的时候 在服务器端创建一个负责监听IP地址和端口号的Socket
                socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //获取ip地址
                IPAddress ip = IPAddress.Parse(UdpInit.GetLocalIP());
                //创建端口号
                IPEndPoint point = new IPEndPoint(ip, 1036);
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
                    if (count == 5)
                    {
                        if (buffer[0] == 0xff && buffer[1] == 0x30 && buffer[2] == 0x4a)  //播放列表中选择影片
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                int index = (int)buffer[3] - 1;
                                if (PlayType.Equals("5D"))
                                {
                                    ListView.SelectedIndex = index;
                                    btnPlayClickFun();
                                }
                                else
                                {
                                    ListView.SelectedValue = memberData[index].MovieName;
                                    ReadFilmListFile(index);
                                }
                            }));
                        }
                    }
                    if (count == 4)
                    {
                        if (buffer[0] == 0xff && buffer[1] == 0x31 && buffer[2] == 0x4b) //暂停
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                if (PlayType.Equals("5D"))
                                {
                                    btnPlayClickFun();
                                    //rb2.IsChecked = true;
                                }
                            }));
                        }
                        if (buffer[0] == 0xff && buffer[1] == 0x32 && buffer[2] == 0x4c) //停止
                        {
                            string index = buffer[3].ToString();
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                if (PlayType.Equals("5D"))
                                {
                                    btnStopClickFun();
                                }
                                else
                                {
                                    labFilm1.Content = "";
                                    Module.actionFile2DOF = null;
                                    Module.actionFile3DOF = null;
                                    Module.actionFile6DOF = null;
                                    Module.effectFile = null;
                                    Module.shakeFile = null;
                                }
                            }));
                        }
                        if (buffer[0] == 0xFF && buffer[1] == 0x33 && buffer[2] == 0x4D)  //恢复播放状态
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                if (PlayType.Equals("5D"))
                                {
                                    btnPlayClickFun();
                                }
                            }));
                        }
                    }
                    if (count == 6)
                    {
                        if(buffer[0] == 0xFF && buffer[1] == 0xFA)
                        {
                            if (buffer[2] == 0xFF && buffer[3] == 0xFF)
                            {
                                this.Dispatcher.Invoke(new Action(() =>
                                {
                                    rb2.IsChecked = true;
                                }));
                            }
                            else
                            {
                                this.Dispatcher.Invoke(new Action(() =>
                                {
                                    rb1.IsChecked = true;
                                }));
                            }
                        }

                        //接收调试座椅数据
                        if (buffer[0] == 0xFF && buffer[1]==0xFB)
                        {
                            switch (buffer[2])
                            {
                                case 255:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataNum[0] = 0xFF;
                                        dataNum[1] = 0xFF;
                                        dataNum[2] = 0xFF;
                                        dataNum[3] = 0xFF;
                                        dataNum[4] = 0xFF;
                                        dataNum[5] = 0xFF;
                                    }
                                    else
                                    {
                                        dataNum[0] = 0;
                                        dataNum[1] = 0;
                                        dataNum[2] = 0;
                                        dataNum[3] = 0;
                                        dataNum[4] = 0;
                                        dataNum[5] = 0;
                                    }
                                    break;
                                case 1:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataNum[0] = 0xFF;
                                    }
                                    else
                                    {
                                        dataNum[0] = 0;
                                    }
                                    break;
                                case 2:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataNum[1] = 0xFF;
                                    }
                                    else
                                    {
                                        dataNum[1] = 0;
                                    }
                                    break;
                                case 3:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataNum[2] = 0xFF;
                                    }
                                    else
                                    {
                                        dataNum[2] = 0;
                                    }
                                    break;
                                case 4:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataNum[3] = 0xFF;
                                    }
                                    else
                                    {
                                        dataNum[3] = 0;
                                    }
                                    break;
                                case 5:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataNum[4] = 0xFF;
                                    }
                                    else
                                    {
                                        dataNum[4] = 0;
                                    }
                                    break;
                                case 6:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataNum[5] = 0xFF;
                                    }
                                    else
                                    {
                                        dataNum[5] = 0;
                                    }
                                    break;
                            }
                        }
                        //接收调试座椅特效数据
                        if (buffer[0] == 0xFF && buffer[1] == 0xFC)
                        {
                            switch (buffer[2])
                            {
                                case 1:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataChairEffect[0] = 0x01;
                                    }
                                    else
                                    {
                                        dataChairEffect[0] = 0;
                                    }
                                    break;
                                case 2:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataChairEffect[1] = 0x02;
                                    }
                                    else
                                    {
                                        dataChairEffect[1] = 0;
                                    }
                                    break;
                                case 3:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataChairEffect[2] = 0x04;
                                    }
                                    else
                                    {
                                        dataChairEffect[3] = 0;
                                    }
                                    break;
                                case 4:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataChairEffect[3] = 0x08;
                                    }
                                    else
                                    {
                                        dataChairEffect[3] = 0;
                                    }
                                    break;
                                case 5:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataChairEffect[4] = 0x10;
                                    }
                                    else
                                    {
                                        dataChairEffect[4] = 0;
                                    }
                                    break;
                                case 6:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataChairEffect[5] = 0x20;
                                    }
                                    else
                                    {
                                        dataChairEffect[5] = 0;
                                    }
                                    break;
                                case 7:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataChairEffect[6] = 0x40;
                                    }
                                    else
                                    {
                                        dataChairEffect[6] = 0;
                                    }
                                    break;
                                case 8:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataChairEffect[7] = 0x80;
                                    }
                                    else
                                    {
                                        dataChairEffect[7] = 0;
                                    }
                                    break;
                            }
                        }
                        //接收调试环境特效数据
                        if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                        {
                            switch (buffer[2])
                            {
                                case 1:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataEvEffect[0] = 0x01;
                                    }
                                    else
                                    {
                                        dataEvEffect[0] = 0;
                                    }
                                    break;
                                case 2:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataEvEffect[1] = 0x02;
                                    }
                                    else
                                    {
                                        dataEvEffect[1] = 0;
                                    }
                                    break;
                                case 3:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataEvEffect[2] = 0x04;
                                    }
                                    else
                                    {
                                        dataEvEffect[3] = 0;
                                    }
                                    break;
                                case 4:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataEvEffect[3] = 0x08;
                                    }
                                    else
                                    {
                                        dataEvEffect[3] = 0;
                                    }
                                    break;
                                case 5:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataEvEffect[4] = 0x10;
                                    }
                                    else
                                    {
                                        dataEvEffect[4] = 0;
                                    }
                                    break;
                                case 6:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataEvEffect[5] = 0x20;
                                    }
                                    else
                                    {
                                        dataEvEffect[5] = 0;
                                    }
                                    break;
                                case 7:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataEvEffect[6] = 0x40;
                                    }
                                    else
                                    {
                                        dataEvEffect[6] = 0;
                                    }
                                    break;
                                case 8:
                                    if (buffer[3] == 0xFF)
                                    {
                                        dataEvEffect[7] = 0x80;
                                    }
                                    else
                                    {
                                        dataEvEffect[7] = 0;
                                    }
                                    break;
                            }
                        }


                    }
                }
            }
        }

        private void ReadFilmListFile(int i)
        {
            if (PlayLanguage.Equals("CN"))
            {
                labFilm1.Content = "当前影片：" + memberData[i].MovieName;
            }
            else
            {
                labFilm1.Content = "Current Movie:" + memberData[i].MovieName;
            }
            Module.DEVFile(memberData[i].FullMovieName);
            //Module.DEVFile(memberData[i].FullMovieName, memberData[i].MovieName);
        }

        /// <summary>
        /// 停止监听
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseTCPServer()
        {
            //AcceptSocketThread.Abort();
            //threadReceive.Abort();
            //socketWatch.Close();
            //if (socketSend != null)
            //{
            //    socketSend.Close();
            //}
            //终止线程
        }
        #endregion

        #region TCP客户端融合软件
        private bool TcpControlClientConnect(string ip, string port)
        {
            try
            {
                tcpControlClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                tcpControlClient.Connect(UdpInit.transformIP(ip, port));
                return true;
            }
            catch (Exception e)
            {
                //MessageBox.Show("连接有误"+e.Message);
                Module.WriteLogFile(e.Message);
                return false;
            }
        }

        private void TcpControlClientClose()
        {
            if (tcpControlClient.Connected)
            {
                tcpControlClient.Close();
            }
        }

        private void ControlPlay(bool isConnect, int num)
        {
            if (isConnect)
            {
                num = num + 1;
                //byte[] data1 = { 0xFF, 0x30, 0x4A, (byte)num, 0xEE };
                string data1 = "play_" + num;
                tcpControlClient.Send(System.Text.Encoding.Default.GetBytes(data1));
                //tcpControlClient.Send(data1);
                TcpControlClientClose();
            }
        }

        private void ControlPause(bool isConnect)
        {
            if (isConnect)
            {
                //byte[] data1 = { 0xFF, 0x31, 0x4B, 0xEE };
                string data1 = "pause";
                tcpControlClient.Send(System.Text.Encoding.Default.GetBytes(data1));
                TcpControlClientClose();
            }
        }

        private void ControlStop(bool isConnect)
        {
            if (isConnect)
            {
                //byte[] data1 = { 0xFF, 0x32, 0x4C, 0xEE };
                string data1 = "stop";
                tcpControlClient.Send(System.Text.Encoding.Default.GetBytes(data1));
                TcpControlClientClose();
            }
        }

        private void ControlPlayAgain(bool isConnect)
        {
            if (isConnect)
            {
                //byte[] data1 = { 0xFF, 0x33, 0x4D, 0xEE };
                string data1 = "play";
                tcpControlClient.Send(System.Text.Encoding.Default.GetBytes(data1));
                TcpControlClientClose();
            }
        }

        #endregion

        #region TCP客户端中控继电器
        /// <summary>
        /// 客户端连接中控继电器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        private bool TcpRelayControlClientConnect(string ip, string port)
        {
            try
            {
                //tcpRelayControlClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //tcpRelayControlClient.Connect(UdpInit.transformIP(ip, port));
                //EndPoint ipep = UdpInit.transformIP(ip, port);//IP和端口
                IPAddress ip1;
                IPAddress.TryParse(ip, out ip1);
                int port1;
                Int32.TryParse(port, out port1);
                IPEndPoint ipep = new IPEndPoint(ip1, port1);
                //Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                tcpRelayControlClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


                ConnectSocketDelegate connect = ConnectSocket;
                IAsyncResult asyncResult = connect.BeginInvoke(ipep, tcpRelayControlClient, null, null);

                bool connectSuccess = asyncResult.AsyncWaitHandle.WaitOne(2000, false);
                if (!connectSuccess)
                {
                    MessageBox.Show(string.Format("失败！错误信息：{0}", "连接超时,控制器未连接"));
                    return false;
                }

                string exmessage = connect.EndInvoke(asyncResult);
                if (!string.IsNullOrEmpty(exmessage))
                {
                    MessageBox.Show(string.Format("失败！错误信息：{0}", exmessage));
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                //MessageBox.Show("控制器未连接"+e.Message);
                Module.WriteLogFile(e.Message);
                return false;
            }
        }

        private delegate string ConnectSocketDelegate(IPEndPoint ipep, Socket sock);
        private string ConnectSocket(IPEndPoint ipep, Socket sock)
        {
            string exmessage = "";
            try
            {
                sock.Connect(ipep);
            }
            catch (System.Exception ex)
            {
                exmessage = ex.Message;
            }
            finally
            {
            }
            return exmessage;
        }

        /// <summary>
        /// 关闭中控继电器客户端
        /// </summary>
        private void TcpRelayControlClientClose()
        {
            if (tcpRelayControlClient != null)
            {
                if (tcpRelayControlClient.Connected)
                {
                    tcpRelayControlClient.Close();
                }
            }
        }

        /// <summary>
        /// 中控继电器初始打开的设备
        /// </summary>
        private void RelayControlInitSend()
        {
            if (PlayControlClient.Equals("TRUE"))
            {
                bool isRelayOpen;
                //空压机 点发
                byte[] data1 = { 0xFE, 0x10, 0x00, 0x0D, 0x00, 0x02, 0x04, 0x00, 0x04, 0x00, 0x0A, 0xC0, 0xE7 };
                isRelayOpen = SendRelayControl(data1);
                //Thread.Sleep(200);
                if (isRelayOpen)
                {
                    //电动球阀
                    byte[] data2 = { 0xFE, 0x05, 0x00, 0x03, 0xFF, 0x00, 0x68, 0x35 };
                    isRelayOpen = SendRelayControl(data2);
                    //Thread.Sleep(200);
                    //增压泵
                    //byte[] data3 = { 0xFE, 0x05, 0x00, 0x04, 0xFF, 0x00, 0xD9, 0xF4 };
                    //isRelayOpen = SendRelayControl(data3);
                    Brush brushOn = new SolidColorBrush(Color.FromArgb(0xff, 0x22, 0xAC, 0x38));
                    cbBoosterPump.Background = brushOn;
                    cbBoosterPump.Opacity = 0.9;
                    cbRadiotube.Background = brushOn;
                    cbRadiotube.Opacity = 0.9;
                    cbLightning.Background = brushOn;
                    cbLightning.Opacity = 0.9;
                   // cbDoor.Background = brushOn;
                   // cbDoor.Opacity = 0.9;
                }
            }
        }

        /// <summary>
        /// 发送继电器控制指令
        /// </summary>
        /// <param name="data1"></param>
        private bool SendRelayControl(byte[] data1)
        {
            //if (tcpRelayControlClient != null)
            //{
            //    if (tcpRelayControlClient.Connected == true)
            //    {
            //        tcpRelayControlClient.Send(data1);
            //        return true;
            //    }
            //    else
            //    {
            //        bool a = TcpRelayControlClientConnect(DelayIP, DelayPort);
            //        Thread.Sleep(200);
            //        if (a == false)
            //        {
            //            MessageBox.Show("控制器未连接，请检查！");
            //            return false;
            //        }
            //        else
            //        {
            //            tcpRelayControlClient.Send(data1);
            //            return true;
            //        }
            //    }
            // }
            //else
            //{
            //    return false;
            // }
            try
            {
                bool a = TcpRelayControlClientConnect(DelayIP, DelayPort);
                Thread.Sleep(300);
                if (a)
                {
                    tcpRelayControlClient.Send(data1);
                }
                else
                {
                    //MessageBox.Show("控制器未连接");
                    return false;
                }
                Thread.Sleep(300);
                tcpRelayControlClient.Close();
                return true;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// 中控继电器播放电影控制的设备
        /// </summary>
        private void RelayControlPlaySend()
        {
            bool isRelayOpen;
            if (tcpRelayControlClient != null)
            {
                // if (tcpRelayControlClient.Connected == true)
                // {
                //场灯
                byte[] data1 = { 0xFE, 0x05, 0x00, 0x00, 0xFF, 0x00, 0x98, 0x35 };
                isRelayOpen = SendRelayControl(data1);
                //Thread.Sleep(200);
                if (isRelayOpen)
                {
                    //门
                    byte[] data2 = { 0xFE, 0x10, 0x00, 0x08, 0x00, 0x02, 0x04, 0x00, 0x04, 0x00, 0x0A, 0x00, 0xD8 };
                    isRelayOpen = SendRelayControl(data2);
                    Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x99, 0x99));
                    cbLightning.Background = brush;
                    cbLightning.Opacity = 1;
                    cbDoor.Background = brush;
                    cbDoor.Opacity = 1;
                }
            }
        }

        private void RelayControlPlaySendLight()
        {
            bool isRelayOpen;
            if (tcpRelayControlClient != null)
            {
                byte[] data1 = { 0xFE, 0x05, 0x00, 0x00, 0xFF, 0x00, 0x98, 0x35 };
                isRelayOpen = SendRelayControl(data1);
                if (isRelayOpen)
                {
                    Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x99, 0x99));
                    cbLightning.Background = brush;
                    cbLightning.Opacity = 1;
                }
            }
        }

        private void RelayControlPlaySendDoor()
        {
            bool isRelayOpen;
            if (tcpRelayControlClient != null)
            {
                //门
                byte[] data2 = { 0xFE, 0x10, 0x00, 0x08, 0x00, 0x02, 0x04, 0x00, 0x04, 0x00, 0x0A, 0x00, 0xD8 };
                isRelayOpen = SendRelayControl(data2);
                if (isRelayOpen)
                {
                    Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x99, 0x99));
                    cbDoor.Background = brush;
                    cbDoor.Opacity = 1;
                }
            }
        }

        /// <summary>
        /// 中控继电器停止播放电影控制的设备
        /// </summary>
        private void RelayControlStopSend()
        {
            bool isRelayOpen;
            if (tcpRelayControlClient != null)
            {
                //if (tcpRelayControlClient.Connected == true)
                // {                  
                byte[] data1 = { 0xFE, 0x05, 0x00, 0x00, 0x00, 0x00, 0xD9, 0xC5 };
                isRelayOpen = SendRelayControl(data1);
                //Thread.Sleep(200);
                if (isRelayOpen)
                {
                    byte[] data2 = { 0xFE, 0x10, 0x00, 0x08, 0x00, 0x02, 0x04, 0x00, 0x04, 0x00, 0x0A, 0x00, 0xD8 };
                    isRelayOpen = SendRelayControl(data2);
                    Brush brushOn = new SolidColorBrush(Color.FromArgb(0xff, 0x22, 0xAC, 0x38));
                    cbLightning.Background = brushOn;
                    cbLightning.Opacity = 0.9;
                    cbDoor.Background = brushOn;
                    cbDoor.Opacity = 0.9;
                }
            }
        }

        private void RelayControlStopSendLight()
        {
            bool isRelayOpen;
            if (tcpRelayControlClient != null)
            {             
                byte[] data1 = { 0xFE, 0x05, 0x00, 0x00, 0x00, 0x00, 0xD9, 0xC5 };
                isRelayOpen = SendRelayControl(data1);
                if (isRelayOpen)
                {
                    Brush brushOn = new SolidColorBrush(Color.FromArgb(0xff, 0x22, 0xAC, 0x38));
                    cbLightning.Background = brushOn;
                    cbLightning.Opacity = 0.9;
                }
            }
        }

        private void RelayControlStopSendDoor()
        {
            bool isRelayOpen;
            if (tcpRelayControlClient != null)
            {                 
                byte[] data2 = { 0xFE, 0x10, 0x00, 0x08, 0x00, 0x02, 0x04, 0x00, 0x04, 0x00, 0x0A, 0x00, 0xD8 };
                isRelayOpen = SendRelayControl(data2);
                if (isRelayOpen)
                {                   
                    Brush brushOn = new SolidColorBrush(Color.FromArgb(0xff, 0x22, 0xAC, 0x38));
                    cbDoor.Background = brushOn;
                    cbDoor.Opacity = 0.9;
                }
            }
        }

        /// <summary>
        /// 关闭软件后关闭所有设备
        /// </summary>
        private void RelayControlCloseSend()
        {
            if (PlayControlClient.Equals("TRUE"))
            {
                if (tcpRelayControlClient != null)
                {
                    // if (tcpRelayControlClient.Connected == true)
                    // {
                    byte[] data1 = { 0xFE, 0x0F, 0x00, 0x00, 0x00, 0x08, 0x01, 0x00, 0xB1, 0x91 };
                    SendRelayControl(data1);

                    byte[] data2 = { 0xFE, 0x10, 0x00, 0x0D, 0x00, 0x02, 0x04, 0x00, 0x04, 0x00, 0x0A, 0xC0, 0xE7 };
                    SendRelayControl(data2);
                    // }
                }
            }
        }

        private void CbRadiotube_Click(object sender, RoutedEventArgs e)
        {
            bool isRelayOpen;
            Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x99, 0x99));
            Brush brushOn = new SolidColorBrush(Color.FromArgb(0xff, 0x22, 0xAC, 0x38));
            MessageBoxResult dr;
            if (cbRadiotube.Opacity == 1)
            {
                if (PlayLanguage.Equals("CN"))
                {
                    dr = MessageBox.Show("确定打开阀门吗?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                }
                else
                {
                    dr = MessageBox.Show("Are you sure to open the Radiotube?", "Tips", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                }
                if (dr == MessageBoxResult.OK)
                {
                    //byte[] data1 = { 0xFE, 0x10, 0x00, 0x0D, 0x00, 0x02, 0x04, 0x00, 0x04, 0x00, 0x0A, 0xC0, 0xE7 };
                    //isRelayOpen = SendRelayControl(data1);
                    //Thread.Sleep(200);
                    byte[] data2 = { 0xFE, 0x05, 0x00, 0x03, 0xFF, 0x00, 0x68, 0x35 };
                    isRelayOpen = SendRelayControl(data2);
                    if (isRelayOpen)
                    {
                        cbRadiotube.Opacity = 0.9;
                        cbRadiotube.Background = brushOn;
                    }
                }
            }
            else
            {
                if (PlayLanguage.Equals("CN"))
                {
                    dr = MessageBox.Show("确定关闭阀门吗?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                }
                else
                {
                    dr = MessageBox.Show("Are you sure to close the Radiotube?", "Tips", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                }
                if (dr == MessageBoxResult.OK)
                {
                    //byte[] data1 = { 0xFE, 0x10, 0x00, 0x0D, 0x00, 0x02, 0x04, 0x00, 0x04, 0x00, 0x0A, 0xC0, 0xE7 };
                    //isRelayOpen = SendRelayControl(data1);
                    //Thread.Sleep(200);
                    byte[] data2 = { 0xFE, 0x05, 0x00, 0x03, 0x00, 0x00, 0x29, 0xC5 };
                    isRelayOpen = SendRelayControl(data2);
                    if (isRelayOpen)
                    {
                        cbRadiotube.Opacity = 1;
                        cbRadiotube.Background = brush;
                    }
                }
            }
        }

        private void CbBoosterPump_Click(object sender, RoutedEventArgs e)
        {
            Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x99, 0x99));
            Brush brushOn = new SolidColorBrush(Color.FromArgb(0xff, 0x22, 0xAC, 0x38));
            MessageBoxResult dr;
            bool isRelayOpen;
            if (cbBoosterPump.Opacity == 1)
            {
                if (PlayLanguage.Equals("CN"))
                {
                    //dr = MessageBox.Show("确定打开增压泵吗?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    dr = MessageBox.Show("确定打开空压机吗?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                }
                else
                {
                    dr = MessageBox.Show("Are you sure to open the air compressor?", "Tips", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                }
                if (dr == MessageBoxResult.OK)
                {
                    //byte[] data1 = { 0xFE, 0x05, 0x00, 0x04, 0xFF, 0x00, 0xD9, 0xF4 };
                    byte[] data1 = { 0xFE, 0x10, 0x00, 0x0D, 0x00, 0x02, 0x04, 0x00, 0x04, 0x00, 0x0A, 0xC0, 0xE7 };
                    isRelayOpen = SendRelayControl(data1);
                    if (isRelayOpen)
                    {
                        cbBoosterPump.Opacity = 0.9;
                        cbBoosterPump.Background = brushOn;
                    }
                }
            }
            else
            {
                if (PlayLanguage.Equals("CN"))
                {
                    dr = MessageBox.Show("确定关闭空压机吗?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                }
                else
                {
                    dr = MessageBox.Show("Are you sure to close the air compressor?", "Tips", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                }
                if (dr == MessageBoxResult.OK)
                {
                    //byte[] data1 = { 0xFE, 0x05, 0x00, 0x04, 0x00, 0x00, 0x98, 0x04 };
                    byte[] data1 = { 0xFE, 0x10, 0x00, 0x0D, 0x00, 0x02, 0x04, 0x00, 0x04, 0x00, 0x0A, 0xC0, 0xE7 };
                    isRelayOpen = SendRelayControl(data1);
                    if (isRelayOpen)
                    {
                        cbBoosterPump.Opacity = 1;
                        cbBoosterPump.Background = brush;
                    }
                }
            }
        }

        private void CbLightning_Click(object sender, RoutedEventArgs e)
        {
            Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x99, 0x99));
            Brush brushOn = new SolidColorBrush(Color.FromArgb(0xff, 0x22, 0xAC, 0x38));
            MessageBoxResult dr;
            bool isRelayOpen;
            if (cbLightning.Opacity == 0.9)
            {
                if (PlayLanguage.Equals("CN"))
                {
                    dr = MessageBox.Show("确定关闭灯吗?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                }
                else
                {
                    dr = MessageBox.Show("Are you sure to turn on the light?", "Tips", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                }
                if (dr == MessageBoxResult.OK)
                {
                    byte[] data2 = { 0xFE, 0x05, 0x00, 0x00, 0xFF, 0x00, 0x98, 0x35 };
                    isRelayOpen = SendRelayControl(data2);
                    if (isRelayOpen)
                    {
                        cbLightning.Opacity = 1;
                        cbLightning.Background = brush;
                    }
                }
            }
            else
            {

                if (PlayLanguage.Equals("CN"))
                {
                    dr = MessageBox.Show("确定打开灯吗?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                }
                else
                {
                    dr = MessageBox.Show("Are you sure to turn off the light?", "Tips", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                }
                if (dr == MessageBoxResult.OK)
                {
                    byte[] data2 = { 0xFE, 0x05, 0x00, 0x00, 0x00, 0x00, 0xD9, 0xC5 };
                    isRelayOpen = SendRelayControl(data2);
                    if (isRelayOpen)
                    {
                        cbLightning.Opacity = 0.9;
                        cbLightning.Background = brushOn;
                    }
                }
            }
        }

        private void CbDoor_Click(object sender, RoutedEventArgs e)
        {
            Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0x99, 0x99));
            Brush brushOn = new SolidColorBrush(Color.FromArgb(0xff, 0x22, 0xAC, 0x38));
            MessageBoxResult dr;
            bool isRelayOpen;
            if (cbDoor.Opacity == 0.9)
            {
                if(PlayLanguage.Equals("CN"))
                {
                    dr = MessageBox.Show("确定关闭门吗?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                }
                else
                {
                    dr = MessageBox.Show("Are you sure to open the door?", "Tips", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                }
                if (dr == MessageBoxResult.OK)
                {
                    byte[] data2 = { 0xFE, 0x10, 0x00, 0x08, 0x00, 0x02, 0x04, 0x00, 0x04, 0x00, 0x0A, 0x00, 0xD8 };
                    isRelayOpen = SendRelayControl(data2);
                    if (isRelayOpen)
                    {
                        cbDoor.Opacity = 1;
                        cbDoor.Background = brush;
                    }
                }
            }
            else
            {

                if (PlayLanguage.Equals("CN"))
                {
                    dr = MessageBox.Show("确定打开门吗?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                }
                else
                {
                    dr = MessageBox.Show("Are you sure to close the door?", "Tips", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                }
                if (dr == MessageBoxResult.OK)
                {
                    byte[] data2 = { 0xFE, 0x10, 0x00, 0x08, 0x00, 0x02, 0x04, 0x00, 0x04, 0x00, 0x0A, 0x00, 0xD8 };
                    isRelayOpen = SendRelayControl(data2);
                    if (isRelayOpen)
                    {
                        cbDoor.Opacity = 0.9;
                        cbDoor.Background = brushOn;
                    }
                }
            }
        }

        #endregion

        #region UDP客户端融合软件
        static Socket UDPSocketClient;
        private void UDPSocketClientInit()
        {
            try
            {
                UDPSocketClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //UDPSocketClient.Bind(UdpInit.transformIP(FuseIP, FusePort));
            }
            catch (Exception e)
            {
                Module.WriteLogFile("UDP连接有误" + e.Message);
            }
        }

        private void FusePlay(int num)
        {
            EndPoint serverPoint = UdpInit.transformIP(FuseIP, FusePort);
            num = num + 1;
            string data1 = "play_" + num;
            UDPSocketClient.SendTo(Encoding.Default.GetBytes(data1), serverPoint);
        }

        private void FusePause()
        {
            EndPoint serverPoint = UdpInit.transformIP(FuseIP, FusePort);
            string data1 = "pause";
            UDPSocketClient.SendTo(Encoding.Default.GetBytes(data1), serverPoint);

        }

        private void FuseStop()
        {
            EndPoint serverPoint = UdpInit.transformIP(FuseIP, FusePort);
            string data1 = "stop";
            UDPSocketClient.SendTo(Encoding.Default.GetBytes(data1), serverPoint);
        }

        private void FusePlayAgain()
        {
            EndPoint serverPoint = UdpInit.transformIP(FuseIP, FusePort);
            string data1 = "play";
            UDPSocketClient.SendTo(Encoding.Default.GetBytes(data1), serverPoint);

        }
        #endregion
    }
}

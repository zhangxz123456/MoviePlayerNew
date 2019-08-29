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
using System.Windows.Shapes;

namespace MoviePlayer
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            this.Closed += LoginWindow_Closed;
        }

        private void LoginWindow_Closed(object sender, EventArgs e)
        {
            
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string s = txtPassWord.Password;
            if (txtUserName.Text.Equals("shuqee") && s == "123456")
            {
                MainWindow.isLogin = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("用户名或密码错误");
            }
        }


    }
}

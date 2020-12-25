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
    /// WinLightning.xaml 的交互逻辑
    /// </summary>
    public partial class WinLightning : Window
    {
        public WinLightning()
        {
            InitializeComponent();
            ButtonInit();
            this.Closed += WinLightning_Closed;
        }

        private void WinLightning_Closed(object sender, EventArgs e)
        {
            MainWindow.dataEvEffect[0] = 0;
            MainWindow.btnEvEffect[0].Opacity = 1;
            Module.DMXLightning = new byte[10];
        }

        private void ButtonInit()
        {
            btnWhite.Click += BtnColor_Click;
            btnRed.Click += BtnColor_Click;
            btnBlue.Click += BtnColor_Click;
            btnGreen.Click += BtnColor_Click;
        }

        private void BtnColor_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int tag = Convert.ToInt32(btn.Tag);
            Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x93, 0x93, 0x93));
            btnWhite.Background = brush;
            btnRed.Background = brush;
            btnBlue.Background = brush;
            btnGreen.Background = brush;
            switch (tag)
            {
                case 1:
                    if (btnWhite.Opacity == 1)
                    {
                        btnWhite.Opacity = 0.9;
                        btnWhite.Background = Brushes.Cyan;
                        btnRed.Opacity = 1;
                        btnBlue.Opacity = 1;
                        btnGreen.Opacity = 1; ;
                        MainWindow.dataEvEffect[0] = 1;
                        Module.DMXLightning[0] = 10;
                        Module.DMXLightning[1] = byte.Parse(textBox1.Text);
                        Module.DMXLightning[2] = byte.Parse(textBox2.Text);
                        Module.DMXLightning[3] = 0;
                        Module.DMXLightning[4] = 0;
                        Module.DMXLightning[5] = 0;
                        Module.DMXLightning[6] = byte.Parse(textBox3.Text);
                        Module.DMXLightning[7] = byte.Parse(textBox4.Text);
                        Module.DMXLightning[8] = byte.Parse(textBox5.Text);
                        Module.DMXLightning[9] = byte.Parse(textBox6.Text);

                    }
                    else
                    {
                        MainWindow.dataEvEffect[0] = 0;
                        //Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x93, 0x93, 0x93));
                        btnWhite.Opacity = 1;
                        btnWhite.Background = brush;
                        Module.DMXLightning = new byte[10];
                    }
                    break;
                case 2:
                    if (btnRed.Opacity == 1)
                    {
                        btnRed.Opacity = 0.9;
                        btnRed.Background = Brushes.Cyan;
                        btnWhite.Opacity = 1;
                        //btnRed.Opacity = 1;
                        btnBlue.Opacity = 1;
                        btnGreen.Opacity = 1;
                        MainWindow.dataEvEffect[0] = 1;
                        Module.DMXLightning[0] = 10;
                        Module.DMXLightning[1] = 255;
                        Module.DMXLightning[2] = 0;
                        Module.DMXLightning[3] = 0;
                        Module.DMXLightning[4] = 0;
                        Module.DMXLightning[5] = 0;
                        Module.DMXLightning[6] = 255;
                        Module.DMXLightning[7] = 0;
                        Module.DMXLightning[8] = 0;
                        Module.DMXLightning[9] = 0;
                    }
                    else
                    {
                        MainWindow.dataEvEffect[0] = 0;
                        //Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x93, 0x93, 0x93));
                        btnRed.Opacity = 1;
                        btnRed.Background = brush;
                        Module.DMXLightning = new byte[10];
                    }
                    break;
                case 3:
                    if (btnBlue.Opacity == 1)
                    {
                        btnBlue.Opacity = 0.9;
                        btnBlue.Background = Brushes.Cyan;
                        btnWhite.Opacity = 1;
                        btnRed.Opacity = 1;
                        //btnBlue.Opacity = 1;
                        btnGreen.Opacity = 1;
                        MainWindow.dataEvEffect[0] = 1;
                        Module.DMXLightning[0] = 10;
                        Module.DMXLightning[1] = 255;
                        Module.DMXLightning[2] = 0;
                        Module.DMXLightning[3] = 0;
                        Module.DMXLightning[4] = 0;
                        Module.DMXLightning[5] = 0;
                        Module.DMXLightning[6] = 0;
                        Module.DMXLightning[7] = 0;
                        Module.DMXLightning[8] = 255;
                        Module.DMXLightning[9] = 0;
                    }
                    else
                    {
                        MainWindow.dataEvEffect[0] = 0;
                        //Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x93, 0x93, 0x93));
                        btnBlue.Opacity = 1;
                        btnBlue.Background = brush;
                        Module.DMXLightning = new byte[10];
                    }
                    break;
                case 4:
                    if (btnGreen.Opacity == 1)
                    {
                        btnGreen.Opacity = 0.9;
                        btnGreen.Background = Brushes.Cyan;
                        btnWhite.Opacity = 1;
                        btnRed.Opacity = 1;
                        btnBlue.Opacity = 1;
                        //btnGreen.Opacity = 1;
                        MainWindow.dataEvEffect[0] = 1;
                        Module.DMXLightning[0] = 10;
                        Module.DMXLightning[1] = 255;
                        Module.DMXLightning[2] = 0;
                        Module.DMXLightning[3] = 0;
                        Module.DMXLightning[4] = 0;
                        Module.DMXLightning[5] = 0;
                        Module.DMXLightning[6] = 0;
                        Module.DMXLightning[7] = 255;
                        Module.DMXLightning[8] = 0;
                        Module.DMXLightning[9] = 0;
                    }
                    else
                    {
                        MainWindow.dataEvEffect[0] = 0;
                        //Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x93, 0x93, 0x93));
                        btnGreen.Opacity = 1;
                        btnGreen.Background = brush;
                        Module.DMXLightning = new byte[10];
                    }
                    break;

            }
        }

      
    }
}

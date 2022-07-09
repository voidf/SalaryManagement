using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace SalaryManagement
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class WelcomePage : Page
    {
        MainWindow mw;
        //int x = 0;
        public WelcomePage(string motd, MainWindow _m)
        {
            InitializeComponent();
            L.Content = motd;
            mw = _m;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //++x;
            //B.Content = x;
            if (!File.Exists(Utils.Constant.admin_auth_file))
                mw.navigate(new InitAdminPage(mw));
            else mw.navigate(new LoginPage(mw));

            //mw.indexFrame = new
        }
    }
}

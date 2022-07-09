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

namespace SalaryManagement
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    /// 


    public partial class InitAdminPage : Page
    {
        class ViewModel
        {
            public ViewModel(string _handle, string _password)
            {
                Handle = _handle;
                Password = _password;
                Confirm = "";
            }
            public string Handle { get; set; }
            public string Password { get; set; }
            public string Confirm { get; set; }
        }

        ViewModel v = new ViewModel("yaya", "");
        MainWindow mw;
        public InitAdminPage(MainWindow _mw)
        {
            InitializeComponent();
            this.DataContext = v;
            mw = _mw;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (v.Password != v.Confirm)
            {
                MessageBox.Show("确认密码和密码不一致呃...", "密码校验未通过");
                return;
            }

            if (!DataModel.Auth.CheckHandle(v.Handle) || !DataModel.Auth.CheckPassword(v.Password)) return;

            var a = new DataModel.Auth(v.Handle, v.Password);

            Utils.Protocol.dump<DataModel.Auth>(a, Utils.Constant.admin_auth_file);

            MessageBox.Show("您的凭据已经保存成功，请谨记管理员用户名和密码\n如果忘记需要删除本程序目录下" + Utils.Constant.admin_auth_file+"并重新初始化本程序");
            mw.navigate(new LoginPage(mw));
        }

        private void PasswordChange(object sender, RoutedEventArgs e)
        {
            v.Password = ((PasswordBox)sender).Password;
        }

        private void ConfirmChange(object sender, RoutedEventArgs e)
        {
            v.Confirm = ((PasswordBox)sender).Password;
        }

    }
}

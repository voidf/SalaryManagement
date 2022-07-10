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

using Utils;

namespace SalaryManagement
{
    /// <summary>
    /// LoginPage.xaml 的交互逻辑
    /// </summary>

    public partial class LoginPage : Page
    {
        class ViewModel
        {
            private bool[] _role  = new bool[] { 
                false,  // 管理
                true    // 员工
            }; 
            public bool[] Role { get { return _role; } }
            public int SelectedRole { get { return Array.IndexOf(_role, true); }}
            public string Handle { get; set; }
            public string Password { get; set; }
        }
        ViewModel v = new ViewModel();
        MainWindow mw;
        public LoginPage(MainWindow _mw)
        {
            InitializeComponent();

            mw = _mw;
            this.DataContext = v;
        }

        private void PasswordChange(object sender, RoutedEventArgs e)
        {
            v.Password = ((PasswordBox)sender).Password;
        }

        private void ClickLogin(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine(v.Role[0]);
            //Console.WriteLine(v.Role[1]); 
            //Console.WriteLine(v.SelectedRole);

            if (!DataModel.Auth.CheckHandle(v.Handle)) return;
            if(v.Role[0]) // 管理
            {
                MessageBox.Show("在做了~");
                return;
            }
            else // 员工
            {
                Constant.EnsurePath(Constant.user_path);

                var fp = Constant.UserDirectory(v.Handle);
                var fn = Constant.UserPath(v.Handle, DataModel.Auth.extension);
                if(!Directory.Exists(fp) || !File.Exists(fn))
                {
                    MessageBox.Show("用户名或密码错误");
                    return;
                }
                var saved = Protocol.load<DataModel.Auth>(fn);
                var current = new DataModel.Auth(v.Handle, v.Password);
                if(!current.Equals(saved))
                {
                    MessageBox.Show($"用户名或密码错误\n{saved.password}\n{current.password}");
                    return;
                }
                mw.navigate(new UserPage(mw, v.Handle));
                
            }
        }
        private void ClickRegister(object sender, RoutedEventArgs e)
        {
            if (!DataModel.Auth.CheckHandle(v.Handle)) return;
            if (v.Role[0]) // 管理
            {
                MessageBox.Show("管理员不支持注册哦~");
                return;
            }
            else // 员工
            {
                Constant.EnsurePath(Constant.user_path);

                var fp = Constant.UserDirectory(v.Handle);
                Constant.EnsurePath(fp);

                var fn = Constant.UserPath(v.Handle, DataModel.Auth.extension);
                var a = new DataModel.Auth(v.Handle, v.Password);

                Protocol.dump<DataModel.Auth>(a, fn);

                MessageBox.Show("注册成功！");

            }
        }
    }
}

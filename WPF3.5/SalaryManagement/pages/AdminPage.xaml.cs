using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// AdminPage.xaml 的交互逻辑
    /// </summary>
    public partial class AdminPage : Page
    {
        public string[] periodName = { "白班", "晚班", "夜班" };

        struct ProductEntity
        {
            public string user { get; set; }
            public int id { get; set; }

            public string type { get; set; }         // 机床型号
            public byte period { get; set; }          // 0: 白班
                                                      // 1: 晚班
                                                      // 2: 夜班
            public string process_name { get; set; }  // 工序名称
            public int count_begin { get; set; }
            public int count_end { get; set; }        // 数量序号
            public int count { get; set; }            // 加工数量
            public string ps { get; set; }            // 备注
            public int salary { get; set; }           // 工资结算（按分钱算，元需要/100）
            public string content { get; set; }       // 杂班内容
            public bool approved { get; set; }        // 审核通过
        }

        class ViewModel
        {
            public ObservableCollection<ProductEntity> li { get; set; } = new ObservableCollection<ProductEntity>();
            public ProductEntity selected {get;set;}
            //public List<DataModel.Product> li { get; set; } = new List<DataModel.Product>();

            public string type { get; set; }        // 机床型号
            public bool[] periodArr { get; set; } = { true, false, false };      // 0: 白班
                                                                                 // 1: 晚班
                                                                                 // 2: 夜班
            public int SelectedPeriod { get { return Array.IndexOf(periodArr, true); } }
            public string user { get; set; }
            public string process_name { get; set; } // 工序名称
            public int count_begin { get; set; }
            public int count_end { get; set; }       // 数量序号
            public int count { get; set; }          // 加工数量
            public string ps { get; set; }           // 备注
            public int salary { get; set; }         // 工资结算（按分钱算，元需要/100）
            public string content { get; set; }      // 杂班内容
            public bool approved { get; set; }


            public string password { get; set; }
            public string confirm { get; set; }
        }

        ViewModel v = new ViewModel();
        MainWindow mw;

        public AdminPage(MainWindow _mw)
        {
            InitializeComponent();
            mw = _mw;
            //Refresh();
            DataContext = v;
        }

        public void Refresh()
        {
            var userlist = DataModel.Auth.GetUserDirList();
            foreach (var i in userlist)
            {

            }
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); // 限制用户只能输入0-9
        private void NumberOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }


        private void RefreshBtn(object sender, RoutedEventArgs e){Refresh();}

        private void ChangePassword(object sender, RoutedEventArgs e)
        {

        }

        private void PasswordChanged(object sender, RoutedEventArgs e) { v.password = ((PasswordBox)sender).Password; }
        private void ConfirmChanged(object sender, RoutedEventArgs e) { v.confirm = ((PasswordBox)sender).Password; }

        private void Back(object sender, RoutedEventArgs e)
        {
            mw.navigate(new LoginPage(mw));
        }

        private void Submit(object sender, RoutedEventArgs e)
        {

        }
    }
}

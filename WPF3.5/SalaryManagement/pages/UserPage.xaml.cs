using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
using Utils;

namespace SalaryManagement
{
    /// <summary>
    /// UserPage.xaml 的交互逻辑
    /// </summary>
    /// 


    public partial class UserPage : Page
    {

        public string[] periodName = { "白班", "晚班", "夜班" };


        class ViewModel
        {
            public ObservableCollection<DataModel.Product> li { get; set; } = new ObservableCollection<DataModel.Product>();
            //public List<DataModel.Product> li { get; set; } = new List<DataModel.Product>();

            public string type { get; set; }        // 机床型号
            public bool[] periodArr { get; set; } = { true, false, false };      // 0: 白班
                                                                                 // 1: 晚班
                                                                                 // 2: 夜班
            public int SelectedPeriod { get { return Array.IndexOf(periodArr, true); } }
            public string process_name { get; set; } // 工序名称
            public int count_begin { get; set; }
            public int count_end { get; set; }       // 数量序号
            public int count { get; set; }          // 加工数量
            public string ps { get; set; }           // 备注
            public int salary { get; set; }         // 工资结算（按分钱算，元需要/100）
            public string content { get; set; }      // 杂班内容


            public string password { get; set; }
            public string confirm { get; set; }
        }

        ViewModel v = new ViewModel() { 
            type= "ArTerm_V6",
            process_name= "装备划时代的升压器",
            count_begin=114,
            count_end=514,
            ps= "敌方POW-40%(2回合)",
            salary=1919810,
            content= "短时间内歪曲敌人的认知。"
        };
        MainWindow mw;
        string handle;

        public void Refresh()
        {
            var newli = Utils.Protocol.LoadFromDirectory<DataModel.Product>(
                Utils.Constant.UserDirectory(handle),
                DataModel.Product.extension
            );
            newli.Sort();
            v.li.Clear();
            foreach(var i in newli)
                v.li.Add(i);
            
        }
        private void RefreshBtn(object sender, RoutedEventArgs e){Refresh();}

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); // 限制用户只能输入0-9
        private void NumberOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }


        public UserPage(MainWindow _mw, string _handle)
        {
            InitializeComponent();
            mw = _mw;
            handle = _handle;
            Refresh();
            DataContext = v;
        }

        private void Back(object sender, RoutedEventArgs e){mw.navigate(new LoginPage(mw));}

        private void Submit(object sender, RoutedEventArgs e)
        {
            var x = Utils.Protocol.DumpToDirectoryGetID<DataModel.Product>(
                Utils.Constant.UserDirectory(handle),
                DataModel.Product.extension
            );
            var p = new DataModel.Product() {
                id = x,
                type = v.type,
                period = (byte)v.SelectedPeriod,
                process_name=v.process_name,
                count_begin=v.count_begin,
                count_end=v.count_end,
                count=v.count_end-v.count_begin+1,
                ps=v.ps,
                salary=v.salary,
                content=v.content
            };
            if (p.count<=0)
            {
                MessageBox.Show($"警告：数量{p.count}非法");
            }
            MessageBoxResult res = MessageBox.Show(
                $"请确认您的填报信息: \n" +
                $"机床型号: {p.type}\n" +
                $"班次: {periodName[p.period]}\n" +
                $"工序名称: {p.process_name}\n" +
                $"数量序号: {p.count_begin}-{p.count_end}\n" +
                $"数量（计算得出）: {p.count}\n" +
                $"备注: {p.ps}\n" +
                $"工资结算: {p.salary}\n" +
                $"杂班内容: {p.content}\n" +
                $"如需返回修改可以选择否，确认无误选是则提交", "填报前最后的后悔药（", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.No) return;


            Utils.Protocol.DumpToDirectoryWithID<DataModel.Product>(
               p,
               Utils.Constant.UserDirectory(handle),
               DataModel.Product.extension,
               x
           );
            MessageBox.Show("填报成功");
            Refresh();
        }

        private void PasswordChanged(object sender, RoutedEventArgs e){v.password = ((PasswordBox)sender).Password;}
        private void ConfirmChanged(object sender, RoutedEventArgs e){v.confirm = ((PasswordBox)sender).Password;}

        private void ChangePassword(object sender, RoutedEventArgs e)
        {
            if(v.password != v.confirm)
            {
                MessageBox.Show("确认密码和密码不一致呃...", "密码校验未通过");
                return;
            }
            if (!DataModel.Auth.CheckPassword(v.password)) return;

            var fp = Constant.UserDirectory(handle);
            Constant.EnsurePath(fp);

            var fn = Constant.UserPath(handle, DataModel.Auth.extension);
            var a = new DataModel.Auth(handle, v.password);

            Protocol.dump<DataModel.Auth>(a, fn);

            MessageBox.Show("密码修改成功！");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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



        class ViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                var propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            public ObservableCollection<ProductEntity> li { get; set; } = new ObservableCollection<ProductEntity>();
            public ProductEntity selected { get; set; }
            //public List<DataModel.Product> li { get; set; } = new List<DataModel.Product>();


            private string _user;
            public string user { get => _user; set { _user = value; OnPropertyChanged("user"); } }

            private int _id;
            public int id { get => _id; set { _id = value; OnPropertyChanged("id"); } }

            private string _type;
            public string type { get => _type; set { _type = value; OnPropertyChanged("type"); } }        // 机床型号

            private bool[] _periodArr = { true, false, false };
            public bool[] periodArr { get => _periodArr; set { _periodArr = value; OnPropertyChanged("periodArr"); } }    // 0: 白班
                                                                                                                          // 1: 晚班
                                                                                                                          // 2: 夜班
            public int SelectedPeriod { get { return Array.IndexOf(periodArr, true); } }
            private string _process_name;
            public string process_name { get => _process_name; set { _process_name = value; OnPropertyChanged("process_name"); } } // 工序名称

            private int _count_begin;
            public int count_begin { get => _count_begin; set { _count_begin = value; OnPropertyChanged("count_begin"); } }

            private int _count_end;
            public int count_end { get => _count_end; set { _count_end = value; OnPropertyChanged("count_end"); } }       // 数量序号
            //public int count { get; set; }          // 加工数量

            private string _ps;
            public string ps { get => _ps; set { _ps = value; OnPropertyChanged("ps"); } }           // 备注

            private int _salary;
            public int salary { get => _salary; set { _salary = value; OnPropertyChanged("salary"); } }          // 工资结算（按分钱算，元需要/100）

            private string _content;
            public string content { get => _content; set { _content = value; OnPropertyChanged("content"); } }       // 杂班内容

            private bool _approved;
            public bool approved { get => _approved; set { _approved = value; OnPropertyChanged("approved"); } }

            private bool _commitenabled = false;
            public bool commitenabled { get => _commitenabled; set { _commitenabled = value; OnPropertyChanged("commitenabled"); } }

            public string password { get; set; }
            public string confirm { get; set; }
        }

        ViewModel v = new ViewModel();
        MainWindow mw;

        public AdminPage(MainWindow _mw)
        {
            InitializeComponent();
            mw = _mw;
            Refresh();
            DataContext = v;
        }

        public void Refresh()
        {
            var userlist = DataModel.Auth.GetUserDirListWithoutAuth();
            v.li.Clear();
            v.commitenabled = false;

            foreach (var i in userlist)
            {
                List<int> fns;
                var t = Utils.Protocol.LoadFromDirectory<DataModel.Product>(
                    i,
                    DataModel.Product.extension,
                    out fns
                );
                int idx = 0;
                foreach (var j in t)
                {
                    v.li.Add(
                        new ProductEntity()
                        {
                            user = i.Substring(Utils.Constant.user_path.Length + 1),
                            id = fns[idx++],
                            type = j.type,
                            period = j.period,
                            process_name = j.process_name,
                            count_begin = j.count_begin,
                            count_end = j.count_end,
                            count = j.count,
                            ps = j.ps,
                            salary = j.salary,
                            content = j.content,
                            approved = j.approved
                        }
                    );
                }
            }
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); // 限制用户只能输入0-9
        private void NumberOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }


        private void RefreshBtn(object sender, RoutedEventArgs e) { Refresh(); }

        private void ChangePassword(object sender, RoutedEventArgs e)
        {
            if (v.password != v.confirm)
            {
                MessageBox.Show("确认密码和密码不一致呃...", "密码校验未通过");
                return;
            }
            if (!DataModel.Auth.CheckPassword(v.password)) return;
            var x = Utils.Protocol.load<DataModel.Auth>(Utils.Constant.admin_auth_file);
            var nx = new DataModel.Auth(x.handle, v.password);
            Utils.Protocol.dump<DataModel.Auth>(nx, Utils.Constant.admin_auth_file);
            //Console.WriteLine($"{nx.handle} => {nx.password} +++ {x.password}");
            MessageBox.Show("密码修改成功！");
        }

        private void PasswordChanged(object sender, RoutedEventArgs e) { v.password = ((PasswordBox)sender).Password; }
        private void ConfirmChanged(object sender, RoutedEventArgs e) { v.confirm = ((PasswordBox)sender).Password; }

        private void Back(object sender, RoutedEventArgs e) { mw.navigate(new LoginPage(mw)); }

        private void CommitChange(object sender, RoutedEventArgs e)
        {
            var x = v.id;
            var p = new DataModel.Product()
            {
                id = x,
                type = v.type,
                period = (byte)v.SelectedPeriod,
                process_name = v.process_name,
                count_begin = v.count_begin,
                count_end = v.count_end,
                count = v.count_end - v.count_begin + 1,
                ps = v.ps,
                salary = v.salary,
                content = v.content,
                approved = v.approved
            };
            if (p.count <= 0)
            {
                MessageBox.Show($"警告：数量{p.count}非法");
            }
            MessageBoxResult res = MessageBox.Show(
                $"请确认您将要修改的填报信息: \n" +
                $"机床型号: {p.type}\n" +
                $"班次: {periodName[p.period]}\n" +
                $"工序名称: {p.process_name}\n" +
                $"数量序号: {p.count_begin}-{p.count_end}\n" +
                $"数量（计算得出）: {p.count}\n" +
                $"备注: {p.ps}\n" +
                $"工资结算: {p.salary}\n" +
                $"杂班内容: {p.content}\n" +
                $"确认通过: {p.approved}\n" +
                $"如需返回修改可以选择否，确认无误选是则提交", "修改前最后的后悔药（", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.No) return;


            Utils.Protocol.DumpToDirectoryWithID<DataModel.Product>(
               p,
               Utils.Constant.UserDirectory(v.user),
               DataModel.Product.extension,
               x
           );
            MessageBox.Show("填报成功");
            Refresh();
        }

        private void CreateNew(object sender, RoutedEventArgs e)
        {
            Constant.EnsurePath(Constant.UserDirectory(v.user));
            var x = Utils.Protocol.DumpToDirectoryGetID<DataModel.Product>(
                Utils.Constant.UserDirectory(v.user),
                DataModel.Product.extension
            );
            var p = new DataModel.Product()
            {
                id = x,
                type = v.type,
                period = (byte)v.SelectedPeriod,
                process_name = v.process_name,
                count_begin = v.count_begin,
                count_end = v.count_end,
                count = v.count_end - v.count_begin + 1,
                ps = v.ps,
                salary = v.salary,
                content = v.content,
                approved = v.approved
            };
            if (p.count <= 0)
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
                $"确认通过: {p.approved}\n" +
                $"如需返回修改可以选择否，确认无误选是则提交", "提交前最后的后悔药（", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.No) return;


            Utils.Protocol.DumpToDirectoryWithID<DataModel.Product>(
               p,
               Utils.Constant.UserDirectory(v.user),
               DataModel.Product.extension,
               x
           );
            MessageBox.Show("填报成功");
            Refresh();
        }

        private string formatStruct(ProductEntity p)
        {
            return
                $"id: {p.id}\n" +
                $"员工名: {p.user}\n" +
                $"机床型号: {p.type}\n" +
                $"班次: {periodName[p.period]}\n" +
                $"工序名称: {p.process_name}\n" +
                $"数量序号: {p.count_begin}-{p.count_end}\n" +
                $"数量（计算得出）: {p.count}\n" +
                $"备注: {p.ps}\n" +
                $"工资结算: {p.salary}\n" +
                $"杂班内容: {p.content}\n" +
                $"确认通过: {p.approved}\n";
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            v.id = v.selected.id;
            v.user = v.selected.user;
            v.type = v.selected.type;
            var x = new bool[] { false, false, false };

            x[v.selected.period] = true;

            v.periodArr = x;

            v.process_name = v.selected.process_name;
            v.count_begin = v.selected.count_begin;
            v.count_end = v.selected.count_end;
            v.salary = v.selected.salary;
            v.approved = v.selected.approved;
            v.ps = v.selected.ps;
            v.content = v.selected.content;
            v.commitenabled = true;
            //Console.WriteLine(formatStruct(v.selected));
            
        }
    }
}

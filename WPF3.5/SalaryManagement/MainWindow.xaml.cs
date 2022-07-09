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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        //WelcomePage welcomepage;

        public void ToWelcomePage(string msg ="")
        {
            indexFrame.Content = new WelcomePage(msg, this);
        }
        List<string> motdList = new List<string>{
            "海内存知己，天涯若比邻",
            "疯狂周四，v我50",
            "能不能别写客户端啊，赤佬",
            "这个界面纯粹为了过渡而展示..."
        };

        public void navigate(Page p)
        {
            indexFrame.Content = p;
        }

        public MainWindow()
        {
            InitializeComponent();
            string motd = motdList[(new Random()).Next(motdList.Count)];
            
            ToWelcomePage(motd);
        }
    
        
    }



}

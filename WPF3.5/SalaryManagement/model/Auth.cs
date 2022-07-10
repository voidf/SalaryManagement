using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.IO;

namespace DataModel
{
    [Serializable]
    struct Auth
    {
        public const string extension = ".auth";
        const string invalid_charset = "\\/\":|<>?*";

        public string handle, password;


        static string sha256(string source)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(source);
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            byte[] retVal = sha256.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();

            foreach (var i in retVal)
            {
                sb.Append(i.ToString("x2"));
            }

            return sb.ToString();
        }

        public static bool CheckHandle(string _handle)
        {
            if (_handle == null || _handle.Length == 0)
            {
                MessageBox.Show("用户名不能为空");
                return false;
            }
            foreach (var c in invalid_charset)
            {
                if (_handle.Contains(c))
                {
                    MessageBox.Show("用户名不能包含这些字符："+invalid_charset);
                    return false;
                }
            }
            return true;
        }

        public static bool CheckPassword(string _password)
        {
            if (_password == null) _password = "";
            if (_password.Length == 0)
            {
                MessageBoxResult res = MessageBox.Show("密码不建议为空呃...您确定要置空吗？", "空密码警告", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.No)
                    return false;
            }
            return true;
        }

        static string encrypt(string input)
        {
            return sha256(input + Utils.Constant.salt);
        }

        public Auth(string _handle, string _password)
        {
            handle = _handle;
            if (_password == null) _password = "";
            password = encrypt(_password);
        }

        public static List<string> GetUserDirList()
        {
            var li = new List<string>();
            foreach(var i in Directory.GetDirectories(Utils.Constant.user_path))
            {
                if(File.Exists($"{i}/{extension}"))
                {
                    li.Add(i);
                }
            }
            return li;
        }

    }
}

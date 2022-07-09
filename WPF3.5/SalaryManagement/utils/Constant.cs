using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Utils
{
    // 各种彩蛋常量
    public class Constant
    {
        public const string salt = "Sorawomiro"; // 狗管理密码盐值
        public const string admin_auth_file = "admin.bin"; // 狗管理密码验证文件路径
        public const string auth_extension = ".auth";
        public const string user_path = "userdata";

        public static string UserDirectory(string userhandle) { return $"{user_path}/{userhandle}"; }
        public static string UserPath(string userhandle, string filename) { return $"{user_path}/{userhandle}/{filename}"; }
        public static void EnsurePath(string path) {if (!Directory.Exists(path)) Directory.CreateDirectory(path);}
    }
}

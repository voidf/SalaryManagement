using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

using System.Runtime.Serialization.Formatters.Binary;
using DataModel;

namespace Utils
{

    public class Serializer
    {
        static string dumps<T>(T obj) { throw new Exception("implement me"); }
        static void dump<T>(T obj, string _fp) { throw new Exception("implement me"); }
        static T loads<T>(string src) { throw new Exception("implement me"); }
        static T load<T>(string _fp) { throw new Exception("implement me"); }
    }

    public class Xml : Serializer
    {
        static void _dump<T>(T obj, Stream stream)
        {
            var s = new DataContractSerializer(typeof(T));
            s.WriteObject(stream, obj);
            stream.Flush();
            stream.Position = 0;
        }
        public static string dumps<T>(T obj)
        {
            using (Stream stream = new MemoryStream())
            {
                _dump<T>(obj, stream);
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }
        public static T loads<T>(string src)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(src)))
            {
                var s = new DataContractSerializer(typeof(T));
                return (T)s.ReadObject(ms);
            }
        }
        public static void dump<T>(T obj, string _fp)
        {
            using (var stream = File.OpenWrite(_fp))
            {
                _dump<T>(obj, stream);
            }
        }
        public static T load<T>(string _fp)
        {
            using (var stream = File.OpenRead(_fp))
            {
                var s = new DataContractSerializer(typeof(T));
                return (T)s.ReadObject(stream);
            }
        }
    }



    public class Pickle: Serializer
    {
        static void _dump<T>(T obj, Stream stream)
        {
            var b = new BinaryFormatter();
            b.Serialize(stream, obj);
            stream.Flush();
            stream.Position = 0;
        }
        public static string dumps<T>(T obj)
        {
            using (Stream stream = new MemoryStream())
            {
                _dump<T>(obj, stream);
                var r = new StreamReader(stream);
                return r.ReadToEnd();
            }
        }
        public static T loads<T>(string src)
        {
            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(src)))
            {
                var b = new BinaryFormatter();
                return (T)b.Deserialize(stream);
            }
        }
        public static void dump<T>(T obj, string _fp)
        {
            using (var stream = File.OpenWrite(_fp))
            {
                _dump<T>(obj, stream);
            }
        }
        public static T load<T>(string _fp)
        {
            using (var stream = File.OpenRead(_fp))
            {
                var b = new BinaryFormatter();
                return (T)b.Deserialize(stream);
            }
        }
    }



    public class Protocol : Pickle  // 序列化协议选择
    {
        /* 
         * 从文件夹dir中读取一列T对象，我们认为扩展名或者说文件名以ext结尾的即为这类对象
         * 记录很多的时候可能会相当耗时，但C#异步我不太会搞，之后再说了
         * */
        public static List<T> LoadFromDirectory<T>(string dir, string ext) // ext大概可以用反射解决，但是懒得研究了
        {
            var li = new List<T>();
            var paths = Directory.GetFiles(dir);

            foreach (var f in paths)
            {
                if (f.EndsWith(ext))
                {
                    try
                    {
                        li.Add(load<T>(f));
                        Console.WriteLine($"loaded {typeof(T)} <{f}>");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"反序列化{typeof(T)}时出错：文件{f}有毒\n<{e}>");
                    }
                }
            }
            return li;
        }
        public static List<T> LoadFromDirectory<T>(string dir, string ext, out List<int> filenames) // ext大概可以用反射解决，但是懒得研究了
        {
            filenames = new List<int>();
            var li = new List<T>();
            var paths = Directory.GetFiles(dir);

            foreach (var f in paths)
            {
                if (f.EndsWith(ext))
                {
                    try
                    {
                        li.Add(load<T>(f));
                        filenames.Add(int.Parse(
                            f.Substring(dir.Length+1, f.Length - dir.Length-1-ext.Length))
                            );
                        Console.WriteLine($"loaded {typeof(T)} <{f}>");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"反序列化{typeof(T)}时出错：文件{f}有毒\n<{e}>");
                    }
                }
            }
            return li;
        }
        public static int DumpToDirectoryGetID<T>(string dir, string ext) 
        {
            var m = new HashSet<string>();

            foreach (var f in Directory.GetFiles(dir))
                if (f.EndsWith(ext))
                {
                    Console.WriteLine($"{f} Trim to {f.Substring(dir.Length + 1, f.Length - 1 - dir.Length - ext.Length)}");
                    m.Add(f.Substring(dir.Length + 1, f.Length - 1 - dir.Length - ext.Length));
                }
            int x = 1;
            while (x < 0x7fffffff)
            {
                var s = $"{x}{ext}";
                if (!m.Contains($"{x}"))
                {
                    Console.WriteLine($"Got ID: {x}");
                    return x;
                }
                ++x;
            }
            return -1;
        }
        /* 将一个对象obj存到文件中，参数说明类似LoadFromDirectory */
        public static void DumpToDirectoryWithID<T>(T obj, string dir, string ext, int x)
        {
            var s = $"{x}{ext}";
            Console.WriteLine($"Dumped: {dir}/{s}");
            dump<T>(obj, $"{dir}/{s}");
            return;
        }

        /* 将一个对象obj存到文件中，参数说明类似LoadFromDirectory */
        public static void DumpToDirectory<T>(T obj, string dir, string ext) // 还可以换个策略拿一个文件记自增，但不能复用删掉的id了
        {
            var m = new HashSet<string>();

            foreach (var f in Directory.GetFiles(dir))
                if (f.EndsWith(ext)) 
                {
                    Console.WriteLine($"{f} Trim to {f.Substring(dir.Length+1, f.Length - 1 - dir.Length - ext.Length)}");
                    m.Add(f.Substring(dir.Length+1, f.Length-1 - dir.Length - ext.Length));
                }
            int x = 1;
            while (x < 0x7fffffff)
            {
                var s = $"{x}{ext}";
                if (!m.Contains($"{x}"))
                {
                    Console.WriteLine($"Dumped: {dir}/{s}");
                    dump<T>(obj, $"{dir}/{s}");
                    return;
                }
                ++x;
            }
        }
    }
    
}

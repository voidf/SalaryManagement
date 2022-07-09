using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

using System.Runtime.Serialization.Formatters.Binary;

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

    public class Protocol : Pickle { } // 序列化协议选择
    
}

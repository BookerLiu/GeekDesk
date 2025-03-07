using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace GeekDesk.Util
{
    public static class DeepCopyUtil
    {
        // 使用序列化和反序列化实现深度拷贝
        public static T DeepCopy<T>(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            using (var memoryStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, obj); // 序列化
                memoryStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(memoryStream); // 反序列化
            }
        }
    }
}

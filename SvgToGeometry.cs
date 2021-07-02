using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GeekDesk
{
    class SvgToGeometry
    {
        static void Main(string[] args)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;//忽略文档里面的注释
            settings.DtdProcessing = DtdProcessing.Parse;
            XmlReader reader = XmlReader.Create("D:\\下载文件\\font_2642707_zjdi9qttb38\\iconfont.svg", settings);
            xmlDoc.Load(reader);

            XmlNodeList nodeList =  xmlDoc.SelectNodes("/svg/defs/font/glyph");

            string jsonFilePath = "D:\\下载文件\\font_2642707_zjdi9qttb38\\iconfont.json";
            JObject jo = ReadJson(jsonFilePath);
            JArray ja = JArray.Parse(jo["glyphs"].ToString());

            string value;

            for (int i=0; i<nodeList.Count; i++)
            {
                value = nodeList[i].Attributes["d"].Value;
                
            }

            foreach (XmlNode xmlNode in nodeList)
            {
                value = xmlNode.Attributes["d"].Value;
                Console.WriteLine(value);

               
                
            }
        }

        public static JObject ReadJson(string filePath)
        {
            using (System.IO.StreamReader file = System.IO.File.OpenText(filePath))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject o = (JObject)JToken.ReadFrom(reader);
                    return o;
                }
            }
        }

        public static string GetMd5Str(string ConvertString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
            t2 = t2.Replace("-", "");

            t2 = t2.ToLower();

            return t2;
        }
    }
}

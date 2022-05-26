using GeekDesk.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Xml;

namespace GeekDesk.Util
{
    class SvgToGeometry
    {

        public static List<IconfontInfo> GetIconfonts()
        {
            string svgPath = "/GeekDesk;component/Resource/Iconfont/iconfont.js";
            string jsonPath = "/GeekDesk;component/Resource/Iconfont/iconfont.json";

            Stream svgStream = Application.GetResourceStream(new Uri(svgPath, UriKind.Relative)).Stream;
            Stream jsonStream = Application.GetResourceStream(new Uri(jsonPath, UriKind.Relative)).Stream;

            StreamReader streamReader = new StreamReader(svgStream);
            string svgJsStr = streamReader.ReadToEnd();
            JObject jo = ReadJson(jsonStream);

            return GetIconfonts(svgJsStr, jo);
        }

        public static List<IconfontInfo> GetIconfonts(string svgJsStr, string jsonStr)
        {
            return GetIconfonts(svgJsStr, JObject.Parse(jsonStr));
        }

        public static List<IconfontInfo> GetIconfonts(string svgJsStr, JObject json)
        {

            svgJsStr = svgJsStr.Substring(svgJsStr.IndexOf("<svg>"),
                svgJsStr.Length - (svgJsStr.Length - (svgJsStr.IndexOf("</svg>") + "</svg>".Length)) - svgJsStr.IndexOf("<svg>"));

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(svgJsStr);
            XmlNodeList nodeList = xmlDoc.SelectNodes("/svg/symbol");

            JArray ja = JArray.Parse(json["glyphs"].ToString());

            List<IconfontInfo> listInfo = new List<IconfontInfo>();
            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlNodeList pathNodes = nodeList[i].SelectNodes("path");
                string text = "";
                foreach (XmlNode pathNode in pathNodes)
                {
                    text += pathNode.Attributes["d"].Value;
                }
                string name = JObject.Parse(ja[i].ToString())["name"].ToString();
                listInfo.Add(new IconfontInfo
                {
                    Text = text,
                    Name = name
                });
            }
            return listInfo;
        }


        public static JObject ReadJson(Stream stream)
        {
            using (StreamReader file = new StreamReader(stream))
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

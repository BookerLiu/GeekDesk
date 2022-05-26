using System.IO;
using System.Net;
using System.Text;

namespace GeekDesk.Util
{
    public class HttpUtil
    {
        #region Get请求
        public static string Get(string url)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //创建Web访问对  象
            WebRequest myRequest = WebRequest.Create(url);
            myRequest.ContentType = "text/plain; charset=utf-8";
            //通过Web访问对象获取响应内容
            WebResponse myResponse = myRequest.GetResponse();

            //通过响应内容流创建StreamReader对象，因为StreamReader更高级更快
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.GetEncoding("utf-8"));
            string returnStr = reader.ReadToEnd();//利用StreamReader就可以从响应内容从头读到尾
            reader.Close();
            myResponse.Close();
            return returnStr;
        }
        #endregion
    }
}

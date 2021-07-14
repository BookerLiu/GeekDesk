using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekDesk.Util
{
   public class StringUtil
    {


        public static bool IsEmpty(string str)
        {
            if (str == null || str.Length == 0 || str.Trim().Length == 0)
            {
                return true;
            }
            return false;
        }
    }
}

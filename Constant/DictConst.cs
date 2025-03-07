using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekDesk.Constant
{
    public class DictConst
    {
        public static readonly Dictionary<bool, string> batchMenuHeaderDict = new Dictionary<bool, string>();
        static DictConst() {
            batchMenuHeaderDict.Add(true, "取消批量操作");
            batchMenuHeaderDict.Add(false, "批量操作");
        }
    }
}

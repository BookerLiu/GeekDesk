using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekDesk.Constant
{
    public enum WidthTypeEnum
    {
        LEFT_CARD = 0,  //左侧托盘宽度
        RIGHT_CARD = 1, //右侧托盘宽度
        RIGHT_CARD_HALF = 2, //右侧托盘宽度的一半
        RIGHT_CARD_HALF_TEXT = 3, //右侧托盘宽度的一半 再减去左侧图像宽度
    }
}

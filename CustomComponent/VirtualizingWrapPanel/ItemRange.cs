using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekDesk.CustomComponent.VirtualizingWrapPanel
{
    public struct ItemRange
    {
        public int StartIndex { get; }
        public int EndIndex { get; }

        public ItemRange(int startIndex, int endIndex) : this()
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        public bool Contains(int itemIndex)
        {
            return itemIndex >= StartIndex && itemIndex <= EndIndex;
        }
    }
}

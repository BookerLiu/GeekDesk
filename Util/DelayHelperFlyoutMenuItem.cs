using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekDesk.Util
{
    public class DelayHelperFlyoutMenuItem
    {
        public DelayHelperFlyoutMenuItem()
        {
            TargetType = typeof(DelayHelperFlyoutMenuItem);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}
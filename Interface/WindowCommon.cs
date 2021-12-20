using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GeekDesk.Interface
{
    public interface IWindowCommon
    {
        void OnKeyDown(object sender, KeyEventArgs e);
    }
}

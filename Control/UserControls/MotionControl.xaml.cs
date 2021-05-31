using GeekDesk.Util;
using GeekDesk.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeekDesk.Control.UserControls
{
    /// <summary>
    /// 动作设置
    /// </summary>
    public partial class MotionControl : UserControl
    {
        private static bool controlKeyDown = false;
        private static AppConfig appConfig = MainWindow.appData.AppConfig;

        public MotionControl()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 热键按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HotKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control) 
                || e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Windows)
                || e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Alt)
                || (e.Key >= Key.A && e.Key <= Key.Z))
            {
                if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control)
                || e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Windows)
                || e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Alt))
                {
                    appConfig.HotkeyStr = "Ctrl + ";
                } else if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control)
                || e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Windows)
                || e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Alt)
                && (e.Key >= Key.A && e.Key <= Key.Z))
                {
                    appConfig.HotkeyStr += e.Key.ToString();
                }
            }
        }
    }
}

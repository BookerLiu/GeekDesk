using GeekDesk.Control.Windows;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using HandyControl.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeekDesk.Control.UserControls.Config
{
    /// <summary>
    /// 动作设置
    /// </summary>
    public partial class MotionControl : UserControl
    {
        public static bool hotkeyFinished = true; //热键设置结束
        private static KeyEventArgs prevKeyTemp; //上一个按键
        private static List<KeyEventArgs> keysTemp = new List<KeyEventArgs>();//存储一次快捷键集合
        private static AppConfig appConfig = MainWindow.appData.AppConfig;

        public MotionControl()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 注册热键按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HotKeyDown(object sender, KeyEventArgs e)
        {
            string tag = (sender as TextBox).Tag.ToString();

            bool main = false;
            if ("Main".Equals(tag))
            {
                main = true;
            }

            if (!e.IsRepeat)
            {
                if (hotkeyFinished)
                {
                    if (main)
                    {
                        appConfig.Hotkey = 0;
                        appConfig.HotkeyStr = "";
                        appConfig.HotkeyModifiers = 0;
                    } else
                    {
                        appConfig.ToDoHotkey = 0;
                        appConfig.ToDoHotkeyStr = "";
                        appConfig.ToDoHotkeyModifiers = 0;
                    }
                    hotkeyFinished = false;

                }
                //首次按下按键
                if ((main && (appConfig.HotkeyStr == null || appConfig.HotkeyStr.Length == 0)) 
                    || (!main && (appConfig.ToDoHotkeyStr == null || appConfig.ToDoHotkeyStr.Length == 0)))
                {
                    if (CheckModifierKeys(e))
                    {
                        //辅助键
                        if (main)
                        {
                            appConfig.HotkeyStr = GetKeyName(e);
                            appConfig.HotkeyModifiers = GetModifierKeys(e);
                        } else
                        {
                            appConfig.ToDoHotkeyStr = GetKeyName(e);
                            appConfig.ToDoHotkeyModifiers = GetModifierKeys(e);
                        }
                        prevKeyTemp = e;
                        keysTemp.Add(e);
                    }
                }
                else
                {
                    //非首次按下  需要判断前一个键值是否为辅助键
                    if (CheckModifierKeys(prevKeyTemp)
                        && ((e.Key >= Key.A && e.Key <= Key.Z)
                        || (e.Key >= Key.F1 && e.Key <= Key.F12)
                        || (e.Key >= Key.D0 && e.Key <= Key.D9)))
                    {
                        if (main)
                        {
                            appConfig.Hotkey = e.Key;
                            appConfig.HotkeyStr += e.Key.ToString();
                        } else
                        {
                            appConfig.ToDoHotkey = e.Key;
                            appConfig.ToDoHotkeyStr += e.Key.ToString();
                        }
                        prevKeyTemp = e;
                        keysTemp.Add(e);
                    }
                    else if (CheckModifierKeys(e))
                    {
                        if (main)
                        {
                            appConfig.HotkeyStr += GetKeyName(e);
                            appConfig.HotkeyModifiers |= GetModifierKeys(e);
                        } else
                        {
                            appConfig.ToDoHotkeyStr += GetKeyName(e);
                            appConfig.ToDoHotkeyModifiers |= GetModifierKeys(e);
                        }
                        
                        prevKeyTemp = e;
                        keysTemp.Add(e);
                    }
                }
            }
        }

        private string GetKeyName(KeyEventArgs e)
        {
            Key key = e.Key;
            if (key == Key.LeftCtrl || key == Key.RightCtrl)
            {
                return "Ctrl + ";
            } else if (key == Key.LWin || key == Key.RWin)
            {
                return "Win + ";
            }
            else if (key == Key.LeftShift || key == Key.RightShift)
            {
                return "Shift + ";
            }
            else
            {
                return "Alt + ";
            }

        }

        private HotkeyModifiers GetModifierKeys(KeyEventArgs e)
        {
            Key key = e.Key;
            if (key == Key.LeftCtrl || key == Key.RightCtrl)
            {
                return HotkeyModifiers.MOD_CONTROL;
            }
            else if (key == Key.LWin || key == Key.RWin)
            {
                return HotkeyModifiers.MOD_WIN;
            }
            else if (key == Key.LeftShift || key == Key.RightShift)
            {
                return HotkeyModifiers.MOD_SHIFT;
            }
            else
            {
                return HotkeyModifiers.MOD_ALT;
            }
        }

        private bool CheckModifierKeys(KeyEventArgs e)
        {
            Key key = e.Key;
            return key == Key.LeftCtrl || key == Key.RightCtrl
                || key == Key.LWin || key == Key.RWin
                || key == Key.LeftShift || key == Key.RightShift
                || key == Key.LeftAlt || key == Key.RightAlt;
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        private  void HotKeyUp(object sender, KeyEventArgs e)
        {
            string tag = (sender as TextBox).Tag.ToString();
            bool main = false;
            if ("Main".Equals(tag))
            {
                main = true;
            }
            lock(this)
            {
                bool allKeyUp = true;
                //判断所有键是否都松开
                foreach (KeyEventArgs key in keysTemp)
                {
                    if (key.KeyStates == KeyStates.Down)
                    {
                        allKeyUp = false;
                        break;
                    }
                }
                if (allKeyUp && !hotkeyFinished)
                {
                    keysTemp.Clear();
                    hotkeyFinished = true;

                    if (main)
                    {
                        if (MainWindow.hotKeyId != -1)
                        {
                            Hotkey.UnRegist(new WindowInteropHelper(MainWindow.mainWindow).Handle, Hotkey.keymap[MainWindow.hotKeyId]);
                        }
                        MainWindow.RegisterHotKey(false);
                    } else
                    {
                        if (MainWindow.toDoHotKeyId != -1)
                        {
                            Hotkey.UnRegist(new WindowInteropHelper(MainWindow.toDoInfoWindow).Handle, Hotkey.keymap[MainWindow.toDoHotKeyId]);
                        }
                        MainWindow.RegisterCreateToDoHotKey(false);
                    }

                    
                }
            }
        }

        //private void ShowApp(MainWindow mainWindow)
        //{
        //    if (appConfig.FollowMouse)
        //    {
        //        ShowAppAndFollowMouse(mainWindow);
        //    }
        //    else
        //    {
        //        this.Visibility = Visibility.Visible;
        //    }
        //    Keyboard.Focus(this);
        //}

        ///// <summary>
        ///// 随鼠标位置显示面板 (鼠标始终在中间)
        ///// </summary>
        //private void ShowAppAndFollowMouse(MainWindow mainWindow)
        //{
        //    //获取鼠标位置
        //    System.Windows.Point p = MouseUtil.GetMousePosition();
        //    double left = SystemParameters.VirtualScreenLeft;
        //    double top = SystemParameters.VirtualScreenTop;
        //    double width = SystemParameters.VirtualScreenWidth;
        //    double height = SystemParameters.VirtualScreenHeight;
        //    double right = width - Math.Abs(left);
        //    double bottom = height - Math.Abs(top);


        //    if (p.X - mainWindow.Width / 2 < left)
        //    {
        //        //判断是否在最左边缘
        //        mainWindow.Left = left;
        //    }
        //    else if (p.X + mainWindow.Width / 2 > right)
        //    {
        //        //判断是否在最右边缘
        //        mainWindow.Left = right - mainWindow.Width;
        //    }
        //    else
        //    {
        //        mainWindow.Left = p.X - mainWindow.Width / 2;
        //    }


        //    if (p.Y - mainWindow.Height / 2 < top)
        //    {
        //        //判断是否在最上边缘
        //        mainWindow.Top = top;
        //    }
        //    else if (p.Y + mainWindow.Height / 2 > bottom)
        //    {
        //        //判断是否在最下边缘
        //        mainWindow.Top = bottom - mainWindow.Height;
        //    }
        //    else
        //    {
        //        mainWindow.Top = p.Y - mainWindow.Height / 2;
        //    }

        //    mainWindow.Visibility = Visibility.Visible;
        //}
    }
}

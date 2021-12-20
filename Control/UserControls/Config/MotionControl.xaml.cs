using GeekDesk.Constant;
using GeekDesk.Control.Windows;
using GeekDesk.Thread;
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
using static GeekDesk.Util.GlobalHotKey;

namespace GeekDesk.Control.UserControls.Config
{
    /// <summary>
    /// 动作设置
    /// </summary>
    public partial class MotionControl : UserControl
    {
        public static bool hotkeyFinished = true; //热键设置结束
        private Key prevKeyTemp = Key.None; //上一个按键
        private readonly List<KeyEventArgs> keysTemp = new List<KeyEventArgs>();//存储一次快捷键集合
        private readonly AppConfig appConfig = MainWindow.appData.AppConfig;

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

            Key downKey = e.Key;
            if (downKey == Key.System)
            {
                downKey = e.SystemKey;
            }

            bool main = false;
            if ("Main".Equals(tag))
            {
                main = true;
            }

            if (prevKeyTemp == Key.None || prevKeyTemp!=downKey)
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
                    if (CheckModifierKeys(downKey))
                    {
                        //辅助键
                        if (main)
                        {
                            appConfig.HotkeyStr = GetKeyName(downKey);
                            appConfig.HotkeyModifiers = GetModifierKeys(downKey);
                        } else
                        {
                            appConfig.ToDoHotkeyStr = GetKeyName(downKey);
                            appConfig.ToDoHotkeyModifiers = GetModifierKeys(downKey);
                        }
                        prevKeyTemp = downKey;
                        keysTemp.Add(e);
                    }
                }
                else
                {
                    //非首次按下  需要判断前一个键值是否为辅助键
                    if (CheckModifierKeys(prevKeyTemp)
                        && ((downKey >= Key.A && downKey <= Key.Z)
                        || (downKey >= Key.F1 && downKey <= Key.F12)
                        || (downKey >= Key.D0 && downKey <= Key.D9)))
                    {
                        if (main)
                        {
                            appConfig.Hotkey = downKey;
                            appConfig.HotkeyStr += downKey.ToString();
                        } else
                        {
                            appConfig.ToDoHotkey = downKey;
                            appConfig.ToDoHotkeyStr += downKey.ToString();
                        }
                        prevKeyTemp = downKey;
                        keysTemp.Add(e);
                    }
                    else if (CheckModifierKeys(downKey))
                    {
                        if (main)
                        {
                            appConfig.HotkeyStr += GetKeyName(downKey);
                            appConfig.HotkeyModifiers |= GetModifierKeys(downKey);
                        } else
                        {
                            appConfig.ToDoHotkeyStr += GetKeyName(downKey);
                            appConfig.ToDoHotkeyModifiers |= GetModifierKeys(downKey);
                        }
                        
                        prevKeyTemp = downKey;
                        keysTemp.Add(e);
                    }
                }
            }
        }

        private string GetKeyName(Key key)
        {
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

        private HotkeyModifiers GetModifierKeys(Key key)
        {
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

        private bool CheckModifierKeys(Key key)
        {
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
                    prevKeyTemp = Key.None;
                    hotkeyFinished = true;

                    if (main)
                    {
                        if (MainWindow.hotKeyId != -1)
                        {
                            //Hotkey.UnRegist(new WindowInteropHelper(MainWindow.mainWindow).Handle, Hotkey.keymap[MainWindow.hotKeyId]);
                            GlobalHotKey.Dispose(MainWindow.hotKeyId);
                        }
                        MainWindow.RegisterHotKey(false);
                    } else
                    {
                        if (MainWindow.toDoHotKeyId != -1)
                        {
                            //Hotkey.UnRegist(new WindowInteropHelper(MainWindow.toDoInfoWindow).Handle, Hotkey.keymap[MainWindow.toDoHotKeyId]);
                            GlobalHotKey.Dispose(MainWindow.toDoHotKeyId);
                        }
                        MainWindow.RegisterCreateToDoHotKey(false);
                    }

                    
                }
            }
        }

        /// <summary>
        /// 移动窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragMove(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Window.GetWindow(this).DragMove();
            }
        }

        private void MarginHide_Changed(object sender, RoutedEventArgs e)
        {
            if (appConfig.MarginHide)
            {
                MainWindow.hide.TimerSet();
            } else
            {
                if (MainWindow.hide.timer != null)
                {
                    MainWindow.hide.TimerStop();
                }
            }
        }

        private void Animation_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindow.mainWindow.Visibility == Visibility.Collapsed)
            {
                MainWindow.mainWindow.Visibility = Visibility.Visible;
                // 执行一下动画 防止太过突兀
                MainWindow.FadeStoryBoard(0, (int)CommonEnum.WINDOW_ANIMATION_TIME, Visibility.Collapsed);
            }
        }


        /// <summary>
        /// 鼠标中键呼出 change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseMiddle_Changed(object sender, RoutedEventArgs e)
        {
            if (appConfig.MouseMiddleShow)
            {
                MouseHookThread.MiddleHook();
            } else
            {
                MouseHookThread.Dispose();
            }
        }

    }
}

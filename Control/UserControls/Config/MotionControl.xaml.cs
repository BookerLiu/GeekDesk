using GeekDesk.Constant;
using GeekDesk.MyThread;
using GeekDesk.Util;
using GeekDesk.ViewModel;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            lock (this)
            {
                HotKeyType hkType = (HotKeyType)(sender as TextBox).Tag;

                Key downKey = e.Key;
                if (downKey == Key.System)
                {
                    downKey = e.SystemKey;
                }

                if (!CheckIsEnable(hkType)) return;


                if (prevKeyTemp == Key.None || prevKeyTemp != downKey)
                {
                    if (hotkeyFinished)
                    {
                        switch (hkType)
                        {
                            case HotKeyType.Main:
                                appConfig.Hotkey = Key.None;
                                appConfig.HotkeyStr = "";
                                appConfig.HotkeyModifiers = GlobalHotKey.HotkeyModifiers.None;
                                break;
                            case HotKeyType.ToDo:
                                appConfig.ToDoHotkey = Key.None;
                                appConfig.ToDoHotkeyStr = "";
                                appConfig.ToDoHotkeyModifiers = GlobalHotKey.HotkeyModifiers.None;
                                break;
                            case HotKeyType.ColorPicker:
                                appConfig.ColorPickerHotkey = Key.None;
                                appConfig.ColorPickerHotkeyStr = "";
                                appConfig.ColorPickerHotkeyModifiers = GlobalHotKey.HotkeyModifiers.None;
                                break;
                        }
                        hotkeyFinished = false;
                    }

                    //首次按下按键
                    if ((HotKeyType.Main == hkType && (appConfig.HotkeyStr == null || appConfig.HotkeyStr.Length == 0))
                        || (HotKeyType.ToDo == hkType && (appConfig.ToDoHotkeyStr == null || appConfig.ToDoHotkeyStr.Length == 0))
                        || (HotKeyType.ColorPicker == hkType && (appConfig.ColorPickerHotkeyStr == null || appConfig.ColorPickerHotkeyStr.Length == 0))
                        )
                    {
                        if (CheckModifierKeys(downKey))
                        {
                            //辅助键
                            switch (hkType)
                            {
                                case HotKeyType.Main:
                                    appConfig.HotkeyStr = GetKeyName(downKey);
                                    appConfig.HotkeyModifiers = GetModifierKeys(downKey);
                                    break;
                                case HotKeyType.ToDo:
                                    appConfig.ToDoHotkeyStr = GetKeyName(downKey);
                                    appConfig.ToDoHotkeyModifiers = GetModifierKeys(downKey);
                                    break;
                                case HotKeyType.ColorPicker:
                                    appConfig.ColorPickerHotkeyStr = GetKeyName(downKey);
                                    appConfig.ColorPickerHotkeyModifiers = GetModifierKeys(downKey);
                                    break;
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
                            || (downKey >= Key.D0 && downKey <= Key.D9)
                            || downKey == Key.Oem3
                            ))
                        {
                            KeyUtil.KeyProp keyProp = new KeyUtil.KeyProp();
                            KeyUtil.KeyToChar(downKey, ref keyProp, true);
                            string downKeyStr = keyProp.character.ToString();
                            if (keyProp.character == '\x00')
                            {
                                downKeyStr = downKey.ToString();
                            }
                            switch (hkType)
                            {
                                case HotKeyType.Main:
                                    appConfig.Hotkey = downKey;
                                    appConfig.HotkeyStr += downKeyStr;
                                    break;
                                case HotKeyType.ToDo:
                                    appConfig.ToDoHotkey = downKey;
                                    appConfig.ToDoHotkeyStr += downKeyStr;
                                    break;
                                case HotKeyType.ColorPicker:
                                    appConfig.ColorPickerHotkey = downKey;
                                    appConfig.ColorPickerHotkeyStr += downKeyStr;
                                    break;
                            }
                            prevKeyTemp = downKey;
                            keysTemp.Add(e);
                        }
                        else if (CheckModifierKeys(downKey))
                        {
                            switch (hkType)
                            {
                                case HotKeyType.Main:
                                    appConfig.HotkeyStr += GetKeyName(downKey);
                                    appConfig.HotkeyModifiers |= GetModifierKeys(downKey);
                                    break;
                                case HotKeyType.ToDo:
                                    appConfig.ToDoHotkeyStr += GetKeyName(downKey);
                                    appConfig.ToDoHotkeyModifiers |= GetModifierKeys(downKey);
                                    break;
                                case HotKeyType.ColorPicker:
                                    appConfig.ColorPickerHotkeyStr += GetKeyName(downKey);
                                    appConfig.ColorPickerHotkeyModifiers |= GetModifierKeys(downKey);
                                    break;
                            }

                            prevKeyTemp = downKey;
                            keysTemp.Add(e);
                        }
                    }
                }
            }
        }

        private string GetKeyName(Key key)
        {
            if (key == Key.LeftCtrl || key == Key.RightCtrl)
            {
                return "Ctrl + ";
            }
            else if (key == Key.LWin || key == Key.RWin)
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


        //[MethodImpl(MethodImplOptions.Synchronized)]
        private void HotKeyUp(object sender, KeyEventArgs e)
        {
            lock (this)
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

                    HotKeyType hkType = (HotKeyType)(sender as TextBox).Tag;
                    if (!CheckIsEnable(hkType)) return;

                    switch (hkType)
                    {
                        case HotKeyType.Main:
                            if (MainWindow.hotKeyId != -1)
                            {
                                //Hotkey.UnRegist(new WindowInteropHelper(MainWindow.mainWindow).Handle, Hotkey.keymap[MainWindow.hotKeyId]);
                                GlobalHotKey.Dispose(MainWindow.hotKeyId);
                            }
                            MainWindow.RegisterHotKey(false);
                            break;
                        case HotKeyType.ToDo:
                            if (MainWindow.toDoHotKeyId != -1)
                            {
                                //Hotkey.UnRegist(new WindowInteropHelper(MainWindow.toDoInfoWindow).Handle, Hotkey.keymap[MainWindow.toDoHotKeyId]);
                                GlobalHotKey.Dispose(MainWindow.toDoHotKeyId);
                            }
                            MainWindow.RegisterCreateToDoHotKey(false);
                            break;
                        case HotKeyType.ColorPicker:
                            if (MainWindow.colorPickerHotKeyId != -1)
                            {
                                //Hotkey.UnRegist(new WindowInteropHelper(MainWindow.toDoInfoWindow).Handle, Hotkey.keymap[MainWindow.toDoHotKeyId]);
                                GlobalHotKey.Dispose(MainWindow.colorPickerHotKeyId);
                            }
                            MainWindow.RegisterColorPickerHotKey(false);
                            break;
                    }

                }
            }
        }

        private bool CheckIsEnable(HotKeyType hkType)
        {
            switch (hkType)
            {
                case HotKeyType.Main:
                    return true == appConfig.EnableAppHotKey;
                case HotKeyType.ToDo:
                    return true == appConfig.EnableTodoHotKey;
                case HotKeyType.ColorPicker:
                    return true == appConfig.EnableColorPickerHotKey;
            }
            return false;
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
                MarginHide.StartHide();
            }
            else
            {
                MarginHide.StopHide();
            }
        }



        /// <summary>
        /// 鼠标中键呼出 change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseMiddle_Changed(object sender, RoutedEventArgs e)
        {
            //if (appConfig.MouseMiddleShow)
            //{
            //    MouseHookThread.MiddleHook();
            //}
            //else
            //{
            //    MouseHookThread.DisposeMiddle();
            //}

            MouseHookThread.Dispose();
            MouseHookThread.Hook();
        }

        /// <summary>
        /// 启用热键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnableHotKey_Click(object sender, RoutedEventArgs e)
        {
            HotKeyType hkType = (HotKeyType)(sender as CheckBox).Tag;
            switch (hkType)
            {
                case HotKeyType.Main:
                    if (true == appConfig.EnableAppHotKey)
                    {
                        MainWindow.RegisterHotKey(false);
                    }
                    else
                    {
                        if (MainWindow.hotKeyId != -1)
                        {
                            GlobalHotKey.Dispose(MainWindow.hotKeyId);
                        }
                    }
                    break;
                case HotKeyType.ToDo:
                    if (true == appConfig.EnableTodoHotKey)
                    {
                        MainWindow.RegisterCreateToDoHotKey(false);
                    }
                    else
                    {
                        if (MainWindow.hotKeyId != -1)
                        {
                            GlobalHotKey.Dispose(MainWindow.toDoHotKeyId);
                        }
                    }
                    break;
                case HotKeyType.ColorPicker:
                    if (true == appConfig.EnableColorPickerHotKey)
                    {
                        MainWindow.RegisterColorPickerHotKey(false);
                    }
                    else
                    {
                        if (MainWindow.hotKeyId != -1)
                        {
                            GlobalHotKey.Dispose(MainWindow.colorPickerHotKeyId);
                        }
                    }
                    break;
            }
        }
    }
}

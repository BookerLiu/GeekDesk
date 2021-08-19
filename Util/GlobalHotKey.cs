using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace GeekDesk.Util
{
    public class GlobalHotKey
    {
        public enum HotkeyModifiers
        {
            MOD_ALT = 0x1,
            MOD_CONTROL = 0x2,
            MOD_SHIFT = 0x4,
            MOD_WIN = 0x8
        }

        private static int currentID;
        const int WM_HOTKEY = 0x312;
        private static Dictionary<int, Window> handleTemp = new Dictionary<int, Window>();
        public static Dictionary<int, HotKeyCallBackHanlder> callbackTemp = new Dictionary<int, HotKeyCallBackHanlder>();

        public delegate void HotKeyCallBackHanlder();
        /// <summary>
        /// Registers a global hotkey
        /// </summary>
        /// <param name="aKeyGestureString">e.g. Alt + Shift + Control + Win + S</param>
        /// <param name="callback">Action to be called when hotkey is pressed</param>
        /// <returns>true, if registration succeeded, otherwise false</returns>
        public static int RegisterHotKey(string aKeyGestureString, HotKeyCallBackHanlder callback)
        {
            var c = new KeyGestureConverter();
            KeyGesture aKeyGesture = (KeyGesture)c.ConvertFrom(aKeyGestureString);
            return RegisterHotKey((HotkeyModifiers)aKeyGesture.Modifiers, aKeyGesture.Key, callback);
        }

        public static int RegisterHotKey(HotkeyModifiers aModifier, Key key, HotKeyCallBackHanlder callback)
        {
            Window window = new Window
            {
                WindowStyle = WindowStyle.None,
                Height = 0,
                Width = 0,
                Visibility = Visibility.Collapsed,
                ShowInTaskbar = false
            };
            window.Show();
            IntPtr handle = new WindowInteropHelper(window).Handle;
            HwndSource hs = HwndSource.FromHwnd(handle);
            hs.AddHook(WndProc);
            currentID += 1;
            if (!RegisterHotKey(handle, currentID, aModifier, (uint)KeyInterop.VirtualKeyFromKey(key)))
            {
                window.Close();
                throw new Exception("RegisterHotKey Failed");
            }
            handleTemp.Add(currentID, window);
            callbackTemp.Add(currentID, callback);
            return currentID;
        }
        /// <summary>
        /// 快捷键消息处理
        /// </summary>
        static IntPtr WndProc(IntPtr windowHandle, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                if (callbackTemp.TryGetValue(id, out var callback))
                {
                    callback();
                }
            }
            return IntPtr.Zero;
        }

        public static void Dispose(int id)
        {
            bool test = UnregisterHotKey(new WindowInteropHelper(handleTemp[id]).Handle, id);
            GlobalHotKey.handleTemp[id].Close();
            GlobalHotKey.handleTemp.Remove(id);
            GlobalHotKey.callbackTemp.Remove(id);
            Console.WriteLine(test);
        }

        // Registers a hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, HotkeyModifiers fsModifiers, uint vk);
        // Unregisters the hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}

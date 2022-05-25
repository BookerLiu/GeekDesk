using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace GeekDesk.Util
{
    public class GlobalHotKey
    {
        public enum HotkeyModifiers
        {
            None = 0,
            MOD_ALT = 0x1,
            MOD_CONTROL = 0x2,
            MOD_SHIFT = 0x4,
            MOD_WIN = 0x8
        }

        private static int currentID;
        private static readonly Dictionary<int, InvisibleWindowForMessages> handleTemp = new Dictionary<int, InvisibleWindowForMessages>();

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
            InvisibleWindowForMessages window = new InvisibleWindowForMessages(callback);
            currentID += 1;
            if (!RegisterHotKey(window.Handle, currentID, aModifier, (uint)KeyInterop.VirtualKeyFromKey(key)))
            {
                window.Dispose();
                throw new Exception("RegisterHotKey Failed");
            }
            handleTemp.Add(currentID, window);
            return currentID;
        }

        public static void Dispose(int id)
        {
            try
            {
                UnregisterHotKey(handleTemp[id].Handle, id);
                GlobalHotKey.handleTemp[id].Dispose();
                GlobalHotKey.handleTemp.Remove(id);
            }
            catch
            {
                //nothing
            }


        }

        // Registers a hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, HotkeyModifiers fsModifiers, uint vk);
        // Unregisters the hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private class InvisibleWindowForMessages : NativeWindow, IDisposable
        {
            public event HotKeyCallBackHanlder callback;
            public InvisibleWindowForMessages(HotKeyCallBackHanlder callback)
            {
                CreateHandle(new CreateParams());
                this.callback += callback;
            }

            private static readonly int WM_HOTKEY = 0x0312;
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);
                if (m.Msg == WM_HOTKEY)
                {
                    callback();
                }
            }
            public void Dispose()
            {
                this.DestroyHandle();
            }
        }


    }
}

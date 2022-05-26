using System;

using System.Windows.Input;

namespace GeekDesk.Util
{
    public class KeyUtil
    {
        public class KeyProp
        {
            public Key key;
            public bool printable;
            public char character;
            public bool shift;
            public bool ctrl;
            public bool alt;
            public int type; //sideband
            public string s;    //sideband
        };

        public static void KeyToChar(Key key,
            ref KeyProp keyProp,
            bool downCap = false,
            bool downShift = false
            )
        {
            bool iscap;
            bool caplock = false;
            bool shift;

            keyProp.key = key;

            keyProp.alt = Keyboard.IsKeyDown(Key.LeftAlt) ||
                              Keyboard.IsKeyDown(Key.RightAlt);

            keyProp.ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) ||
                              Keyboard.IsKeyDown(Key.RightCtrl);

            keyProp.shift = Keyboard.IsKeyDown(Key.LeftShift) ||
                              Keyboard.IsKeyDown(Key.RightShift);

            if (keyProp.alt || keyProp.ctrl)
            {
                keyProp.printable = false;
                keyProp.type = 1;
            }
            else
            {
                keyProp.printable = true;
                keyProp.type = 0;
            }

            shift = downShift || keyProp.shift;
            //caplock = Console.CapsLock; //Keyboard.IsKeyToggled(Key.CapsLock);
            iscap = downCap || ((caplock && !shift) || (!caplock && shift));

            switch (key)
            {
                case Key.Enter: keyProp.character = '\n'; return;
                case Key.A: keyProp.character = (iscap ? 'A' : 'a'); return;
                case Key.B: keyProp.character = (iscap ? 'B' : 'b'); return;
                case Key.C: keyProp.character = (iscap ? 'C' : 'c'); return;
                case Key.D: keyProp.character = (iscap ? 'D' : 'd'); return;
                case Key.E: keyProp.character = (iscap ? 'E' : 'e'); return;
                case Key.F: keyProp.character = (iscap ? 'F' : 'f'); return;
                case Key.G: keyProp.character = (iscap ? 'G' : 'g'); return;
                case Key.H: keyProp.character = (iscap ? 'H' : 'h'); return;
                case Key.I: keyProp.character = (iscap ? 'I' : 'i'); return;
                case Key.J: keyProp.character = (iscap ? 'J' : 'j'); return;
                case Key.K: keyProp.character = (iscap ? 'K' : 'k'); return;
                case Key.L: keyProp.character = (iscap ? 'L' : 'l'); return;
                case Key.M: keyProp.character = (iscap ? 'M' : 'm'); return;
                case Key.N: keyProp.character = (iscap ? 'N' : 'n'); return;
                case Key.O: keyProp.character = (iscap ? 'O' : 'o'); return;
                case Key.P: keyProp.character = (iscap ? 'P' : 'p'); return;
                case Key.Q: keyProp.character = (iscap ? 'Q' : 'q'); return;
                case Key.R: keyProp.character = (iscap ? 'R' : 'r'); return;
                case Key.S: keyProp.character = (iscap ? 'S' : 's'); return;
                case Key.T: keyProp.character = (iscap ? 'T' : 't'); return;
                case Key.U: keyProp.character = (iscap ? 'U' : 'u'); return;
                case Key.V: keyProp.character = (iscap ? 'V' : 'v'); return;
                case Key.W: keyProp.character = (iscap ? 'W' : 'w'); return;
                case Key.X: keyProp.character = (iscap ? 'X' : 'x'); return;
                case Key.Y: keyProp.character = (iscap ? 'Y' : 'y'); return;
                case Key.Z: keyProp.character = (iscap ? 'Z' : 'z'); return;
                case Key.D0: keyProp.character = (shift ? ')' : '0'); return;
                case Key.D1: keyProp.character = (shift ? '!' : '1'); return;
                case Key.D2: keyProp.character = (shift ? '@' : '2'); return;
                case Key.D3: keyProp.character = (shift ? '#' : '3'); return;
                case Key.D4: keyProp.character = (shift ? '$' : '4'); return;
                case Key.D5: keyProp.character = (shift ? '%' : '5'); return;
                case Key.D6: keyProp.character = (shift ? '^' : '6'); return;
                case Key.D7: keyProp.character = (shift ? '&' : '7'); return;
                case Key.D8: keyProp.character = (shift ? '*' : '8'); return;
                case Key.D9: keyProp.character = (shift ? '(' : '9'); return;
                case Key.OemPlus: keyProp.character = (shift ? '+' : '='); return;
                case Key.OemMinus: keyProp.character = (shift ? '_' : '-'); return;
                case Key.OemQuestion: keyProp.character = (shift ? '?' : '/'); return;
                case Key.OemComma: keyProp.character = (shift ? '<' : ','); return;
                case Key.OemPeriod: keyProp.character = (shift ? '>' : '.'); return;
                case Key.OemOpenBrackets: keyProp.character = (shift ? '{' : '['); return;
                case Key.OemQuotes: keyProp.character = (shift ? '"' : '\''); return;
                case Key.Oem1: keyProp.character = (shift ? ':' : ';'); return;
                case Key.Oem3: keyProp.character = (shift ? '~' : '`'); return;
                case Key.Oem5: keyProp.character = (shift ? '|' : '\\'); return;
                case Key.Oem6: keyProp.character = (shift ? '}' : ']'); return;
                case Key.Tab: keyProp.character = '\t'; return;
                case Key.Space: keyProp.character = ' '; return;

                // Number Pad
                case Key.NumPad0: keyProp.character = '0'; return;
                case Key.NumPad1: keyProp.character = '1'; return;
                case Key.NumPad2: keyProp.character = '2'; return;
                case Key.NumPad3: keyProp.character = '3'; return;
                case Key.NumPad4: keyProp.character = '4'; return;
                case Key.NumPad5: keyProp.character = '5'; return;
                case Key.NumPad6: keyProp.character = '6'; return;
                case Key.NumPad7: keyProp.character = '7'; return;
                case Key.NumPad8: keyProp.character = '8'; return;
                case Key.NumPad9: keyProp.character = '9'; return;
                case Key.Subtract: keyProp.character = '-'; return;
                case Key.Add: keyProp.character = '+'; return;
                case Key.Decimal: keyProp.character = '.'; return;
                case Key.Divide: keyProp.character = '/'; return;
                case Key.Multiply: keyProp.character = '*'; return;

                default:
                    keyProp.type = 1;
                    keyProp.printable = false;
                    keyProp.character = '\x00';
                    return;
            } //switch  
        }


    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.WindowsCE.Forms;

namespace Krista.FM.Client.iMonitoringWM.Controls
{
    public class HWButtonsNotification : MessageWindow
    {
        #region События
        private event EventHandler _closingAplication;
        private event EventHandler _minimazeAplication;

        public event EventHandler ClosingApplication
        {
            add { _closingAplication += value; }
            remove { _closingAplication -= value; }
        }

        public event EventHandler MinimazeApplication
        {
            add { _minimazeAplication += value; }
            remove { _minimazeAplication -= value; }
        }
        #endregion

        protected const int WM_HOTKEY = 0x0312;
        protected const int WM_MINIMAZE = 49157;
        protected const int WM_QUIT = 786;

        protected const uint MOD_ALT = 0x0001;
        protected const uint MOD_CONTROL = 0x0002;
        protected const uint MOD_SHIFT = 0x0004;
        protected const uint MOD_WIN = 0x0008;
        protected const uint MOD_KEYUP = 0x1000;

        public enum HardwareKeys
        {
            kFirstHardwareKey = 193,
            kHardwareKey1 = kFirstHardwareKey,
            kHardwareKey2 = 194,
            kHardwareKey3 = 195,
            kHardwareKey4 = 196,
            kHardwareKey5 = 197,
            kLastHardwareKey = kHardwareKey5
        }

        [DllImport("coredll.dll")]
        protected static extern uint RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("coredll.dll")]
        protected static extern uint UnregisterFunc1(uint fsModifiers, int id);
        [DllImport("coredll.dll")]
        protected static extern short GetAsyncKeyState(int vKey);


        protected const byte kCurrentMask = 0x01;
        protected const byte kPreviousMask = 0x02;
        protected const byte kClearMask = 0xfc;
        protected const byte kNotPreviousMask = 0xfd;
        protected const int kCurToPrevLeftShift = 1;
        protected const int kNumKeys = 256;
        protected byte[] m_keyStates = new byte[kNumKeys];

        public HWButtonsNotification()
        {
            for (int i = (int)HardwareKeys.kFirstHardwareKey; i <= (int)HardwareKeys.kLastHardwareKey; i++)
            {
                UnregisterFunc1(MOD_WIN, i);
                RegisterHotKey(this.Hwnd, i, MOD_WIN, (uint)i);
            }
            for (int i = 0; i < kNumKeys; i++)
            {
                m_keyStates[i] = 0x00;
            }
        }
        protected override void WndProc(ref Message msg)
            {
                switch (msg.Msg)
                {
                    case WM_MINIMAZE:
                        {
                            //this.OnMinimazeApplication();
                            break;
                        }
                    case WM_QUIT:
                        {
                            //this.OnClosingApplication();
                            break;
                        }
                    default:
                        {
                            base.WndProc(ref msg);
                            break;
                        }
                }
            }

        public void Update()
        {
            for (int i = 0; i < kNumKeys; i++)
            {
                m_keyStates[i] = (byte)((m_keyStates[i] << kCurToPrevLeftShift) & kPreviousMask);
                if ((GetAsyncKeyState(i) & 0x8000) != 0)
                {
                    m_keyStates[i] |= kCurrentMask;
                }
            }
        }

        public bool KeyJustPressed(byte vKey)
        {
            if ((m_keyStates[vKey] & kCurrentMask) != 0 && (m_keyStates[vKey] & kPreviousMask) == 0)
                return true;
            return false;
        }

        public bool KeyJustReleased(byte vKey)
        {
            if ((m_keyStates[vKey] & kCurrentMask) == 0 && (m_keyStates[vKey] & kPreviousMask) != 0)
                return true;
            return false;
        }

        public bool KeyPressed(byte vKey)
        {
            if ((m_keyStates[vKey] & kCurrentMask) != 0)
                return true;
            return false;
        }

        public bool KeyReleased(byte vKey)
        {
            if ((m_keyStates[vKey] & kCurrentMask) == 0)
                return true;
            return false;
        }

        protected virtual void OnClosingApplication()
        {
            if (this._closingAplication != null)
                this._closingAplication(this, new EventArgs());
        }

        protected virtual void OnMinimazeApplication()
        {
            if (this._minimazeAplication != null)
                this._minimazeAplication(this, new EventArgs());
        }
    } 

}

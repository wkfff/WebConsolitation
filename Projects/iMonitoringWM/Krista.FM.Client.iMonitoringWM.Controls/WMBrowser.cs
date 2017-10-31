using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.WindowsCE.Forms;
using System.Drawing;
using Krista.FM.Client.iMonitoringWM.Common;

namespace Krista.FM.Client.iMonitoringWM.Controls
{
    public class WMBrowser : Old.OpenNETCF.Windows.Forms.WebBrowser, OpenNETCF.Windows.Forms.IMessageFilter
    {
        #region События
        private event MouseEventHandler _mouseDownEX;
        private event MouseEventHandler _mouseMoveEX;
        private event MouseEventHandler _mouseUpEX;

        public event MouseEventHandler MouseDownEX
        {
            add { _mouseDownEX += value; }
            remove { _mouseDownEX -= value; }
        }

        public event MouseEventHandler MouseMoveEX
        {
            add { _mouseMoveEX += value; }
            remove { _mouseMoveEX -= value; }
        }

        public event MouseEventHandler MouseUpEX
        {
            add { _mouseUpEX += value; }
            remove { _mouseUpEX -= value; }
        }
        #endregion

        private bool _isInteractive;

        private int iLBUTTONDOWN = (int)OpenNETCF.Win32.WM.LBUTTONDOWN;
        private int iLBUTTONUP = (int)OpenNETCF.Win32.WM.LBUTTONUP;
        private int iMOUSEMOVE = (int)OpenNETCF.Win32.WM.MOUSEMOVE;
        private int iKEYDOWN = (int)OpenNETCF.Win32.WM.KEYDOWN;
        private int iKEYUP = (int)OpenNETCF.Win32.WM.KEYUP;

        const int WM_VSCROLL = 0x115;
        const int SB_LINEUP = 0;
        const int SB_LINEDOWN = 1;
        const int SB_PAGEUP = 2;
        const int SB_PAGEDOWN = 3;

        /// <summary>
        /// Позоволять ли пользователю интерактивную работу с браузером
        /// </summary>
        public bool IsInteractive
        {
            get { return _isInteractive; }
            set { _isInteractive = value; }
        }
        /*
        /// <summary>
        /// Видимость браузера
        /// </summary>
        public new bool Visible
        {
            get { return base.Visible; }
            set 
            {
                base.Visible = value;
                //При скролирование содержимого браузера есть проблема, время от времени он теряет фокус
                //и теряется возможность скролировать кнопками устройства. Берем эту обязаность целиком на себя
                //выведем в интерфейс методы PageUp и PageDown, и будем вызывать их в ручную. 
                //Для отключения скролировать автоматом, вызваю Focus() помогает :)
                if (value)
                    this.Focus()
            }
        }*/

        public WMBrowser()
        {
            //Initialize Message Filter, using OpenNETCF's ApplicationEx
            OpenNETCF.Windows.Forms.Application2.AddMessageFilter(this);
            this.IsInteractive = true;
        }

        public new void ScaleControl(System.Drawing.SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);
        }

        public void HideProgressBar()
        {
            IntPtr hwndStatus = Win32Helper.FindHwndByClass("MSPIE Status", this.Handle);
            if (hwndStatus != IntPtr.Zero)
            {
                Win32Helper.RECT rectStatus = new Win32Helper.RECT();
                Win32Helper.GetClientRect(hwndStatus, out rectStatus);
                Win32Helper.DestroyWindow(hwndStatus);

                this.Height += rectStatus.Height;
            }
        }

        protected virtual void OnMouseDownEX(MouseEventArgs e)
        {
            if (this._mouseDownEX != null)
                this._mouseDownEX(this, e);
        }

        protected virtual void OnMouseMoveEX(MouseEventArgs e)
        {
            if (this._mouseMoveEX != null)
                this._mouseMoveEX(this, e);
        }

        protected virtual void OnMouseUpEX(MouseEventArgs e)
        {
            if (this._mouseUpEX != null)
                this._mouseUpEX(this, e);
        }

        public void LineUp()
        {
            IntPtr hwndStatus = Win32Helper.FindHwndByClass("PIEHTML", this.Handle);
            Win32Helper.SendMessage(hwndStatus, WM_VSCROLL, SB_LINEUP, 0);
        }

        public void LineDown()
        {
            IntPtr hwndStatus = Win32Helper.FindHwndByClass("PIEHTML", this.Handle);
            Win32Helper.SendMessage(hwndStatus, WM_VSCROLL, SB_LINEDOWN, 0);
        }

        public void PageUp()
        {
            IntPtr hwndStatus = Win32Helper.FindHwndByClass("PIEHTML", this.Handle);
            Win32Helper.SendMessage(hwndStatus, WM_VSCROLL, SB_PAGEUP, 0);
        }

        public void PageDown()
        {
            IntPtr hwndStatus = Win32Helper.FindHwndByClass("PIEHTML", this.Handle);
            Win32Helper.SendMessage(hwndStatus, WM_VSCROLL, SB_PAGEDOWN, 0);
        }

        public IntPtr NativeHandle;

        #region ------ IMessageFilter implementation ------
        public bool PreFilterMessage(ref Microsoft.WindowsCE.Forms.Message m)
        {
            if ((m.Msg == iLBUTTONDOWN || m.Msg == iLBUTTONUP || m.Msg == iMOUSEMOVE 
                || m.Msg == iKEYDOWN || m.Msg == iKEYUP)
                 && IsChildestWindow(this.Handle, m.HWnd))
            {
                //Игнорируем эти действия, т.к. после них происходит выделение рисунков на странице
                if (m.Msg == iKEYDOWN || m.Msg == iKEYUP)
                    return true;
                
                NativeHandle = m.HWnd;
                int xPos = (int)m.LParam & 0xFFFF;
                int yPos = ((int)m.LParam >> 16) & 0xFFFF;

                MouseEventArgs e = new MouseEventArgs(MouseButtons.Left, 1, xPos, yPos, 0);

                switch (m.Msg)
                {
                    case (int)OpenNETCF.Win32.WM.LBUTTONUP:
                        this.OnMouseUpEX(e);
                        break;
                    case (int)OpenNETCF.Win32.WM.LBUTTONDOWN:
                        this.OnMouseDownEX(e);
                        break;
                    case (int)OpenNETCF.Win32.WM.MOUSEMOVE:
                        this.OnMouseMoveEX(e);
                        break;
                }
                if (!this._isInteractive)
                    return true;
            }
            return false;
        }
        #endregion

        #region ------- Private functions -----------------
        /// <summary>
        /// Check whether <see cref="hCheck"/> is one of <see cref="hWnd"/>'s grandchildren.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="hCheck"></param>
        /// <returns></returns>
        static private bool IsChildestWindow(IntPtr hWnd, IntPtr hCheck)
        {
            IntPtr ret = hWnd;

            //Find "smallest" chidren
            while ((hWnd = Win32Helper.GetWindow(hWnd, (int)Win32Helper.GetWindowFlags.GW_CHILD)) != IntPtr.Zero)
            {
                ret = hWnd;
            }

            //goes through all of "smallest" grandchildren
            hWnd = ret;
            while ((ret != hCheck) &&
                ((hWnd = Win32Helper.GetWindow(ret, (int)Win32Helper.GetWindowFlags.GW_HWNDNEXT)) != IntPtr.Zero))
            {
                ret = hWnd;
            }

            return (hWnd != IntPtr.Zero);
        }

        #endregion
    }
}

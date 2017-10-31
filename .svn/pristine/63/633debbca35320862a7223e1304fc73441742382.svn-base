using System;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Krista.FM.Client.iMonitoringWM.Controls
{
    public partial class WMTextBox : TextBox, OpenNETCF.Windows.Forms.IMessageFilter
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

        private int iLBUTTONDOWN = (int)OpenNETCF.Win32.WM.LBUTTONDOWN;
        private int iLBUTTONUP = (int)OpenNETCF.Win32.WM.LBUTTONUP;
        private int iMOUSEMOVE = (int)OpenNETCF.Win32.WM.MOUSEMOVE;

        public WMTextBox()
        {
            InitializeComponent();
            //Initialize Message Filter, using OpenNETCF's ApplicationEx
            OpenNETCF.Windows.Forms.Application2.AddMessageFilter(this);
        }

        public virtual void OnMouseDownEX(MouseEventArgs e)
        {
            if (this._mouseDownEX != null)
                this._mouseDownEX(this, e);
        }

        public virtual void OnMouseMoveEX(MouseEventArgs e)
        {
            if (this._mouseMoveEX != null)
                this._mouseMoveEX(this, e);
        }

        public virtual void OnMouseUpEX(MouseEventArgs e)
        {
            if (this._mouseUpEX != null)
                this._mouseUpEX(this, e);
        }

        public IntPtr NativeHandle;

        #region ------ IMessageFilter implementation ------
        bool OpenNETCF.Windows.Forms.IMessageFilter.PreFilterMessage(ref Microsoft.WindowsCE.Forms.Message m)
        {
            if ((m.Msg == iLBUTTONDOWN || m.Msg == iLBUTTONUP || m.Msg == iMOUSEMOVE)
                && IsChildestWindow(this.Handle, m.HWnd))
            {
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
        private static bool IsChildestWindow(IntPtr hWnd, IntPtr hCheck)
        {
            IntPtr ret = hWnd;

            //Find "smallest" chidren
            while ((hWnd = GetWindow(hWnd, (int)GetWindowFlags.GW_CHILD)) != IntPtr.Zero)
            {
                ret = hWnd;
            }

            //goes through all of "smallest" grandchildren
            hWnd = ret;
            while ((ret != hCheck) &&
                ((hWnd = GetWindow(ret, (int)GetWindowFlags.GW_HWNDNEXT)) != IntPtr.Zero))
            {
                ret = hWnd;
            }

            return (hWnd != IntPtr.Zero);
        }

        #endregion

        #region -------- P/Invoke declerations ------------
        /// <summary>
        /// Get relative window with a given window.
        /// </summary>
        /// <param name="hwnd">the Given window</param>
        /// <param name="cmd">an <see cref="GetWindowFlags"/> value, indicates the relation.</param>
        /// <returns></returns>
        [DllImport("coredll.dll")]
        private static extern IntPtr GetWindow(IntPtr hwnd, int cmd);

        private enum GetWindowFlags : int
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_MAX = 5
        }
        #endregion
    }
}

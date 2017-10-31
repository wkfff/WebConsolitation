using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    internal class ClipboardChangeNotifier : NativeWindow, IDisposable
    {
        #region Unmanaged Code
        [DllImport("user32")]
        private extern static IntPtr SetClipboardViewer(IntPtr hWnd);
        [DllImport("user32")]
        private extern static int ChangeClipboardChain(IntPtr hWnd, IntPtr hWndNext);
        [DllImport("user32", CharSet = CharSet.Auto)]
        private extern static int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        private const int WM_DESTROY = 0x0002;
        private const int WM_DRAWCLIPBOARD = 0x308;
        private const int WM_CHANGECBCHAIN = 0x30D;
        #endregion

        #region Member Variables
        /// <summary>
        /// The next handle in the clipboard viewer chain when the 
        /// clipboard notification is installed, otherwise <see cref="IntPtr.Zero"/>
        /// </summary>
        protected IntPtr nextViewerHandle = IntPtr.Zero;

        private IntPtr nextClipboardViewer;
        /// <summary>
        /// Whether this class has been disposed or not.
        /// </summary>
        protected bool disposed = false;
        /// <summary>
        /// The Window clipboard change notification was installed for.
        /// </summary>
        protected IntPtr installedHandle = IntPtr.Zero;
        #endregion

        #region Events
        /// <summary>
        /// Notifies of a change to the clipboard's content.
        /// </summary>
        public event EventHandler ClipboardChanged;
        #endregion

        /// <summary>
        /// Provides default WndProc processing and responds to clipboard change notifications.
        /// </summary>
        /// <param name="e"></param>
        protected override void WndProc(ref Message e)
        {
            // if the message is a clipboard change notification
            switch (e.Msg)
            {
                /*
            case WM_CHANGECBCHAIN:
                if (e.WParam == nextClipboardViewer)
                {
                    //
                    // If wParam is the next clipboard viewer then it
                    // is being removed so update pointer to the next
                    // window in the clipboard chain
                    //
                    nextViewerHandle = e.LParam;
                }
                else
                {
                    SendMessage(nextViewerHandle, e.Msg, e.WParam, e.LParam);
                }

                /*
                // Store the changed handle of the next item in the clipboard chain:
                this.nextViewerHandle = e.LParam;
                if (!this.nextViewerHandle.Equals(IntPtr.Zero))
                {
                    // pass the message on:
                    SendMessage(this.nextViewerHandle, e.Msg, e.WParam, e.LParam);
                }*/
                // We have processed this message:
                //e.Result = IntPtr.Zero;
                //break;
                case WM_DRAWCLIPBOARD:
                    // content of clipboard has changed:
                    EventArgs clipChange = new EventArgs();
                    OnClipboardChanged(clipChange);
                    SendMessage(nextClipboardViewer, e.Msg, e.WParam, e.LParam);
                    // pass the message on:
                    /*
                    if (!nextViewerHandle.Equals(IntPtr.Zero))
                    {
                        
                    }*/
                    // We have processed this message:
                    //e.Result = IntPtr.Zero;
                    break;
                case WM_DESTROY:
                    // Very important: ensure we are uninstalled.
                    Uninstall();
                    // And call the superclass:
                    base.WndProc(ref e);
                    break;
                default:
                    // call the superclass implementation:
                    base.WndProc(ref e);
                    break;
            }
        }

        /// <summary>
        /// Responds to Window Handle change events and uninstalls
        /// the clipboard change notification if it is installed.
        /// </summary>
        protected override void OnHandleChange()
        {
            // If we did get to this point, and we're still installed then the chain will be broken.
            // The response to the WM_TERMINATE message should prevent this.
            Uninstall();
            base.OnHandleChange();
        }

        /// <summary>
        /// Installs clipboard change notification.  The <see cref="AssignHandle"/> method of 
        /// this class must have been called first.
        /// </summary>
        public void Install()
        {
            Uninstall();
            if (!Handle.Equals(IntPtr.Zero))
            {
                installedHandle = Handle;
                nextViewerHandle = SetClipboardViewer(Handle);
            }
        }

        /// <summary>
        /// Uninstalls clipboard change notification.
        /// </summary>
        public void Uninstall()
        {
            if (!installedHandle.Equals(IntPtr.Zero))
            {
                ChangeClipboardChain(installedHandle, nextViewerHandle);
                //int error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                /*Debug.Assert(error == 0,
                    String.Format("{0} Failed to uninstall from Clipboard Chain", this),
                    Win32Error.ErrorMessage(error));*/
                nextViewerHandle = IntPtr.Zero;
                installedHandle = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Raises the <c>ClipboardChanged</c> event.
        /// </summary>
        /// <param name="e">Blank event arguments.</param>
        protected virtual void OnClipboardChanged(EventArgs e)
        {
            if (ClipboardChanged != null)
            {
                ClipboardChanged(this, e);
            }
        }

        /// <summary>
        /// Uninstalls clipboard event notifications if necessary during dispose of this object.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                Uninstall();
                disposed = true;
            }
        }

        /// <summary>
        /// Constructs a new instance of this class.
        /// </summary>
        public ClipboardChangeNotifier()
            : base()
        {
            // intentionally blank
        }

    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Krista.FM.Client.iMonitoringWM.Common
{
    public class Win32Helper
    {
        #region Градиент
        public struct TRIVERTEX
        {
            public int x;
            public int y;
            public ushort Red;
            public ushort Green;
            public ushort Blue;
            public ushort Alpha;

            public TRIVERTEX(int x, int y, Color color)
                : this(x, y, color.R, color.G, color.B, color.A)
            {
            }

            public TRIVERTEX(
                int x, int y,
                ushort red, ushort green, ushort blue,
                ushort alpha)
            {
                this.x = x;
                this.y = y;
                this.Red = (ushort)(red << 8);
                this.Green = (ushort)(green << 8);
                this.Blue = (ushort)(blue << 8);
                this.Alpha = (ushort)(alpha << 8);
            }
        }

        public struct GRADIENT_RECT
        {
            public uint UpperLeft;
            public uint LowerRight;
            public GRADIENT_RECT(uint ul, uint lr)
            {
                this.UpperLeft = ul;
                this.LowerRight = lr;
            }
        }

        [DllImport("coredll.dll", SetLastError = true, EntryPoint = "GradientFill")]
        public extern static bool GradientFill(
            IntPtr hdc,
            TRIVERTEX[] pVertex,
            uint dwNumVertex,
            GRADIENT_RECT[] pMesh,
            uint dwNumMesh,
            uint dwMode);

        public const int GRADIENT_FILL_RECT_H = 0x00000000;
        public const int GRADIENT_FILL_RECT_V = 0x00000001;
        #endregion

        #region ScreenShot
        [DllImport("coredll.dll")]
        public static extern int BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth,
            int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);
        public const int SRCCOPY = 0x00CC0020;
        #endregion

        #region Свернуть приложение
        [DllImport("coredll.dll")]
        public static extern int ShowWindow(IntPtr hWnd, int nCmdShow);
        public const int SW_MINIMIZED = 6;
        #endregion

        #region Работа с окнами
        /// <summary>
        /// Get relative window with a given window.
        /// </summary>
        /// <param name="hwnd">the Given window</param>
        /// <param name="cmd">an <see cref="GetWindowFlags"/> value, indicates the relation.</param>
        /// <returns></returns>
        [DllImport("coredll.dll")]
        static public extern IntPtr GetWindow(IntPtr hwnd, int cmd);


        public enum GetWindowFlags : int
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_MAX = 5
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;
        }
        [DllImport("coredll.dll")]
        static public extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("coredll.dll")]
        static public extern bool DestroyWindow(IntPtr hwnd);

        [DllImport("coredll.dll")]
        static public extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        static public IntPtr FindHwndByClass(string strClass, IntPtr parentHwnd)
        {
            StringBuilder sbClass = new StringBuilder(256);
            if (0 != GetClassName(parentHwnd, sbClass, sbClass.Capacity) && sbClass.ToString() == strClass)
                return parentHwnd;

            IntPtr hwndChild = GetWindow(parentHwnd, (int)GetWindowFlags.GW_CHILD);
            while (hwndChild != IntPtr.Zero)
            {
                IntPtr result = FindHwndByClass(strClass, hwndChild);
                if (result != IntPtr.Zero)
                    return result;

                hwndChild = GetWindow(hwndChild, (int)GetWindowFlags.GW_HWNDNEXT);
            }

            return IntPtr.Zero;
        }

        [DllImport("coredll.dll")]
        static public extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("coredll.dll", EntryPoint = "SetBkColor", SetLastError = true)]
        static public extern uint SetBkColorCE(IntPtr hdc, int crColor);

        [DllImport("coredll.dll")]
        static public extern IntPtr GetForegroundWindow();
        #endregion

        #region Создание процесса
        [DllImport("Coredll.dll")]
        static public extern int CreateProcess(string pszImageName, string
            pszCmdLine, IntPtr psaProcess, IntPtr psaThread, bool fInheritHandles, int
            fdwCreate, IntPtr pvEnvironment, IntPtr pszCurDir, IntPtr psiStartInfo, IntPtr pProcInfo);
        #endregion
    }
}

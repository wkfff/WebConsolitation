using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ExcelContainer
{
    public class ExcelContainer : UserControl
    {
        [DllImport("user32.dll")]
        static extern int FindWindow(string strclassName, string strWindowName);

        [DllImport("user32.dll")]
        static extern int SetParent(int hWndChild, int hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern bool SetWindowPos(
            int hWnd,               
            int hWndInsertAfter,    
            int X,                  
            int Y,                  
            int cx,                 
            int cy,                 
            uint uFlags             
            );

        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        static extern bool MoveWindow(
            int hWnd,
            int X,
            int Y,
            int nWidth,
            int nHeight,
            bool bRepaint
            );

        [DllImport("user32.dll")]
        public static extern int FindWindowEx(
            int hwndParent, int hwndChildAfter, string lpszClass,
            int missing);

        const int SWP_DRAWFRAME = 0x20;
        const int SWP_NOMOVE = 0x2;
        const int SWP_NOZORDER = 0x4;

        public int excelWnd;
        private string filename;
        private object xl, workbook, workbooks;

        private readonly System.ComponentModel.Container components;

        public ExcelContainer()
        {
            InitializeComponent();
            excelWnd = 0;
        }

        private void InitializeComponent()
        {
            Name = "ExcelControl";
            Size = new System.Drawing.Size(400, 400);
            Resize += OnResize;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }

            base.Dispose(disposing);
        }

        public void CloseWorkbook()
        {
            try
            {
                if (workbook != null)
                {
                    workbook.GetType().InvokeMember(
                        "Close", 
                        BindingFlags.InvokeMethod, 
                        null, 
                        workbook, 
                        new object[] { true });

                    Marshal.ReleaseComObject(workbook);
                    workbook = null;
                }
            }
            catch
            {
            }
        }

        public void SaveWorkbook()
        {
            try
            {
                workbook = workbooks.GetType().InvokeMember(
                    "Save",
                    BindingFlags.InvokeMethod,
                    null, workbook,
                    null);
            }
            catch
            {
            }   
        }

        public void LoadWorkbook(string fileName)
        {
            filename = fileName;
            try
            {
                if (xl == null)
                {
                    xl = Activator.CreateInstance(Type.GetTypeFromProgID("Excel.Application"));
                }

                if (workbook != null)
                {
                    CloseWorkbook();
                }

                if (excelWnd == 0)
                {
                    excelWnd = FindWindow("XLMAIN", null);
                }

                if (excelWnd != 0)
                {
                    SetParent(excelWnd, Handle.ToInt32());

                    workbooks = xl.GetType().InvokeMember(
                        "Workbooks",
                        BindingFlags.GetProperty,
                        null,
                        xl,
                        null);

                    workbook = workbooks.GetType().InvokeMember(
                        "Open",
                        BindingFlags.InvokeMethod,
                        null, workbooks,
                        new object[] {filename, true});

                    xl.GetType().InvokeMember(
                        "UserControl",
                        BindingFlags.SetProperty,
                        null,
                        xl,
                        new object[] {true});

                    workbook.GetType().InvokeMember(
                        "Activate",
                        BindingFlags.InvokeMethod,
                        null,
                        workbook,
                        null);

                    xl.GetType().InvokeMember(
                        "Visible",
                        BindingFlags.SetProperty,
                        null,
                        xl,
                        new object[] {true});

                    SetWindowPos(excelWnd, Handle.ToInt32(), 0, 0, Bounds.Width, Bounds.Height,
                                 SWP_NOZORDER | SWP_NOMOVE | SWP_DRAWFRAME);
                    MoveWindow(excelWnd, 0, 0, Bounds.Width, Bounds.Height, true);
                    SendKeys.Send(Keys.F2.ToString());
                    SendKeys.Send("{ESC}");

                }
            }
            catch
            {
            }            
        }

        public void QuitExcel()
        {
            excelWnd = 0;
            SaveWorkbook();
            CloseWorkbook();

            if (xl != null)
            {
                try
                {
                    xl.GetType().InvokeMember(
                        "Quit", 
                        BindingFlags.InvokeMethod, 
                        null, 
                        xl, 
                        null);

                    Marshal.ReleaseComObject(xl);
                    
                    xl = null;
                    workbook = null;
                    workbooks = null;

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    System.Threading.Thread.Sleep(1000);
                }
                catch
                {
                }
            }
        }

        private void OnResize(object sender, EventArgs e)
        {
            MoveWindow(excelWnd, 0, 0, Bounds.Width, Bounds.Height, true);
        }
    }
}

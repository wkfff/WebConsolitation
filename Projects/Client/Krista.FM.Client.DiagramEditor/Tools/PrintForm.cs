using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Windows.Forms;
using Infragistics.Win.Printing;

using Krista.FM.Client.Design;
using Krista.FM.Client.DiagramEditor.Commands;

namespace Krista.FM.Client.DiagramEditor.Tools
{
    /// <summary>
    /// ������ ���������
    /// </summary>
    public class PrintForm
    {
        #region Fields

        /// <summary>
        /// �������� ��� ������
        /// </summary>
        private Metafile printImage;

        /// <summary>
        /// ���������� ��������
        /// </summary>
        private int currenNumberPage = 0;

        private DiargamEditor site;

        /// <summary>
        /// ��������� ������ PrintPreviewDialog
        /// </summary>
        private UltraPrintPreviewDialog ppd;
        
        /// <summary>
        /// ������� ������ �� ����� ��������
        /// </summary>
        private Command command;

        /// <summary>
        /// ������ �� �����/���������� ���������
        /// </summary>
        private bool isOnePage = false;

        #endregion Fields

        #region Constructor

        public PrintForm(DiargamEditor site)
        {
            this.site = site;

            Instance();
        }

        #endregion Constructor

        #region Properties

        public UltraPrintPreviewDialog Ppd
        {
            get { return ppd; }
            set { ppd = value; }
        }

        public Command Command
        {
            get { return command; }
            set { command = value; }
        }

        public bool IsOnePage
        {
            get { return isOnePage; }
            set { isOnePage = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// ��������� ������� ���������������� ���������
        /// </summary>
        public void Instance()
        {
            if (ppd != null)
            {
                ppd.Close();
            }

            ppd = new UltraPrintPreviewDialog();
            ppd.PageSetupDialogDisplaying += new PageSetupDialogDisplayingEventHandler(Ppd_PageSetupDialogDisplaying);
        }

        /// <summary>
        /// �������� ����� ������ ������ 
        /// </summary>
        public void Print()
        {
            this.Print(printImage);
        }

        /// <summary>
        /// ���������� ����� ������
        /// </summary>
        public void Print(Metafile printImage)
        {
            this.printImage = printImage;
 
            if (printImage == null)
            {
                return;
            }

            // ��������, ��������� �� ������
            PrintDocument printDoc = new PrintDocument();

            printDoc.BeginPrint += new PrintEventHandler(PrintDoc_BeginPrint);
            printDoc.PrintPage += new PrintPageEventHandler(PrintDoc_PrintPage);

            // ��������� �������� ��-���������
            printDoc.DefaultPageSettings = site.DefaultSettings.DafaultPageSettings;

            ppd.Document = printDoc;
            ppd.WindowState = FormWindowState.Maximized;
        }

        private void Ppd_PageSetupDialogDisplaying(object sender, PageSetupDialogDisplayingEventArgs e)
        {
            e.Dialog.PageSettings = site.DefaultSettings.DafaultPageSettings;

            if (e.Dialog.ShowDialog() == DialogResult.OK)
            {
                site.DefaultSettings.DafaultPageSettings = e.Dialog.PageSettings;

                e.Cancel = true;
                Update();
            }

            e.Cancel = true;
        }

        /// <summary>
        /// �����������, � ������ ����� �������� 
        /// </summary>
        private void Update()
        {
            Instance();

            // ���� �������� �� ����� ��������
            if (command != null)
            {
                ((CommandPrintOnePage)command).InitializeMetafile();
            }
            else
            {
                // ���� �������� �� ���������� 
                Print();
            }

            ppd.ShowDialog();
        }

        #endregion Methods

        #region Events

        /// <summary>
        /// ������ ������
        /// </summary>
        private void PrintDoc_BeginPrint(object sender, PrintEventArgs e)
        {
            currenNumberPage = 0;
        }

        /// <summary>
        /// ������ ���������
        /// </summary>
        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {            
            // �������� �������
            Point[] pointOffsets = GeneratePrintingOffsets(e.MarginBounds);

            // ������� ������, ������������ ��������� 
            e.Graphics.SetClip(e.MarginBounds);

            // ��������
            Point pointOffset = new Point(-pointOffsets[currenNumberPage].X, -pointOffsets[currenNumberPage].Y);
            pointOffset.Offset(e.MarginBounds.X, e.MarginBounds.Y);
            
            // ������ � ����������� �� �������� ������
            if (isOnePage)
            {
                // ...�� ����� ��������
                ScaleImageIsotropically(e.Graphics, printImage, e.Graphics.ClipBounds);

                // �� �� �� ����� �������� ��������!!
                e.HasMorePages = false;
            }
            else
            {
                // �� ����������
                e.Graphics.DrawImage(printImage, pointOffset);

                // �������� �� ������������� ������ ��������� ��������
                e.HasMorePages = currenNumberPage < pointOffsets.Length - 1;

                // �������� ��������
                currenNumberPage++;
            }
        }

        /// <summary>
        /// ��������� ����������� � ������������� � ����������� ���������
        /// </summary>
        private void ScaleImageIsotropically(Graphics graphics, Metafile image, RectangleF rectangleF)
        {
            SizeF sizef = new SizeF(
                image.Width / image.HorizontalResolution, 
                image.Height / image.VerticalResolution);

            float floatScale = Math.Min(
                rectangleF.Width / sizef.Width,
                rectangleF.Height / sizef.Height);

            sizef.Width *= floatScale;
            sizef.Height *= floatScale;

            graphics.DrawImage(
                image,
                rectangleF.X,
                rectangleF.Y,
                sizef.Width,
                sizef.Height);
        }

        #endregion Events

        #region HelperFunc

        /// <summary>
        /// ���������� ����� �������� (���� ������� ������� �� ������� ������)
        /// </summary>
        private Point[] GeneratePrintingOffsets(Rectangle rect)
        {
            if (printImage == null)
            {
                return null;
            }

            int x = (int)Math.Ceiling((double)printImage.Width / (double)rect.Width);
            int y = (int)Math.Ceiling((double)printImage.Height / (double)rect.Height);
            Point[] arrPoint = new Point[x * y];

            // ������ ��������
            if (arrPoint.Length == 0)
            {
                arrPoint = new Point[1];
                arrPoint[0] = Point.Empty;
                return arrPoint;
            }

            // Flood the array
            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    arrPoint[(i * x) + j] = new Point(j * rect.Width, i * rect.Height);
                }
            }

            return arrPoint;
        }

        #endregion Helper Func
    }
}

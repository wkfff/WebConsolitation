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
    /// Печать метафайла
    /// </summary>
    public class PrintForm
    {
        #region Fields

        /// <summary>
        /// Метафайл для печати
        /// </summary>
        private Metafile printImage;

        /// <summary>
        /// печатаемая страница
        /// </summary>
        private int currenNumberPage = 0;

        private DiargamEditor site;

        /// <summary>
        /// Экземпляр класса PrintPreviewDialog
        /// </summary>
        private UltraPrintPreviewDialog ppd;
        
        /// <summary>
        /// Команда печати на одной странице
        /// </summary>
        private Command command;

        /// <summary>
        /// печать на одной/нескольких страницах
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
        /// Экземпляр диалога предварительного просмотра
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
        /// Открытый метод вызова печати 
        /// </summary>
        public void Print()
        {
            this.Print(printImage);
        }

        /// <summary>
        /// Внутренний метод печати
        /// </summary>
        public void Print(Metafile printImage)
        {
            this.printImage = printImage;
 
            if (printImage == null)
            {
                return;
            }

            // Документ, выводимый на печать
            PrintDocument printDoc = new PrintDocument();

            printDoc.BeginPrint += new PrintEventHandler(PrintDoc_BeginPrint);
            printDoc.PrintPage += new PrintPageEventHandler(PrintDoc_PrintPage);

            // параметры страницы по-умолчанию
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
        /// Перерисовка, с учетом новых настроек 
        /// </summary>
        private void Update()
        {
            Instance();

            // если печатаем на одной странице
            if (command != null)
            {
                ((CommandPrintOnePage)command).InitializeMetafile();
            }
            else
            {
                // если печатаем на нескольких 
                Print();
            }

            ppd.ShowDialog();
        }

        #endregion Methods

        #region Events

        /// <summary>
        /// Начало печати
        /// </summary>
        private void PrintDoc_BeginPrint(object sender, PrintEventArgs e)
        {
            currenNumberPage = 0;
        }

        /// <summary>
        /// Печать диаграммы
        /// </summary>
        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {            
            // Смещение страниц
            Point[] pointOffsets = GeneratePrintingOffsets(e.MarginBounds);

            // Область печати, ограниченная отступами 
            e.Graphics.SetClip(e.MarginBounds);

            // смещение
            Point pointOffset = new Point(-pointOffsets[currenNumberPage].X, -pointOffsets[currenNumberPage].Y);
            pointOffset.Offset(e.MarginBounds.X, e.MarginBounds.Y);
            
            // Рисуем в зависимости от настроек печати
            if (isOnePage)
            {
                // ...на одной странице
                ScaleImageIsotropically(e.Graphics, printImage, e.Graphics.ClipBounds);

                // мы же на одной странице печатаем!!
                e.HasMorePages = false;
            }
            else
            {
                // на нескольких
                e.Graphics.DrawImage(printImage, pointOffset);

                // Проверка на необходимость вывода следующей страницы
                e.HasMorePages = currenNumberPage < pointOffsets.Length - 1;

                // следущая страница
                currenNumberPage++;
            }
        }

        /// <summary>
        /// Вписываем изображение в прямоугольник с сохранением пропорций
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
        /// Гинерируем точки смещения (если рисунок выходит за границы печати)
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

            // пустая страница
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

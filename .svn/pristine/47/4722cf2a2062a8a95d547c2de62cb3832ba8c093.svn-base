using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Exporter;
using Krista.FM.Client.MDXExpert.Grid.UserInterface;
using Krista.FM.Common.Xml;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.Win.UltraWinDock;
using Infragistics.Win;
using System.Drawing.Printing;
using System.Xml;
using Krista.FM.Client.MDXExpert.CommonClass;
using Krista.FM.Client.Common.Forms;


namespace Krista.FM.Client.MDXExpert
{
    public delegate void ReportElementEventHandler();

    public abstract class CustomReportElement: Panel
    {
        #region Поля

        private MainForm _mainForm;
        private PivotData _pivotData;
        private ReportElementType elementType;
        private bool isActive;
        private MessageAppearance nullDataMessage;

        private Panel captionPanel;
        private Panel commentAndElementPanel;
        private Panel elementPanel;
        private Panel commentPanel;
        private ElementCaption caption;
        private ElementComment comment;
        private Splitter captionSplitter;
        private Splitter commentSplitter;
        private ElementExporter _exporter;
        private bool _isCubeNotFond;
        private string _errorMessageText;
        private CellSet _cls;
        private string uniqName;

        //Поля для печати
        private Bitmap printableImage = null;
        private int curentPrintablePage = 0;

        #endregion    

        /// <summary>
        /// Имя куба
        /// </summary>
        public string CubeName
        {
            get
            {
                return !IsCustomMap ? this.PivotData.CubeName : String.Empty;
            }
            set { SetCubeName(value); }
        }


        public CustomReportElement(MainForm mainForm, ReportElementType elementType)
        {
            this.MainForm = mainForm;
            this.Name = "NewPane";
            this.Dock = DockStyle.Fill;
            this.isActive = false;
            PivotData.AdomdConn = mainForm.AdomdConn;
            this.uniqName = Guid.NewGuid().ToString();

            PivotDataType pDataType = PivotDataType.Table;

            switch (elementType)
            {
                case ReportElementType.eTable:
                    this.Exporter = new TableElementExporter(this);
                    pDataType = PivotDataType.Table;
                    break;
                case ReportElementType.eChart:
                    this.Exporter = new ChartElementExporter(this);
                    pDataType = PivotDataType.Chart;
                    break;
                case ReportElementType.eMap:
                    this.Exporter = new MapElementExporter(this);
                    pDataType = PivotDataType.Map;
                    break;
                case ReportElementType.eGauge:
                    this.Exporter = new GaugeElementExporter(this);
                    pDataType = PivotDataType.Gauge;
                    break;
                case ReportElementType.eMultiGauge:
                    this.Exporter = new GaugeElementExporter(this);
                    pDataType = PivotDataType.Chart;
                    break;
            }
            this.PivotData = new PivotData(pDataType);


            this.BackColor = Color.White;

            this.InitializeComponent();
            //Добавляем ссылки
            this.Comment.SetLinks(this.commentAndElementPanel);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.nullDataMessage = new MessageAppearance();

            this.captionPanel = new Panel();
            this.commentAndElementPanel = new Panel();
            this.elementPanel = new Panel();
            this.commentPanel = new Panel();
            this.captionSplitter = new Splitter();
            this.commentSplitter = new Splitter();
            this.caption = new ElementCaption(this, this.captionPanel, this.captionSplitter);
            this.comment = new ElementComment(this, this.commentPanel, this.commentSplitter);

            this.captionSplitter.Dock = DockStyle.Top;

            this.caption.Dock = DockStyle.Fill;
            this.caption.Appearance.FontData.SizeInPoints = 10;
            this.caption.Text = "Заголовок элемента отчета";
            this.caption.Appearance.TextHAlign = HAlign.Center;
            this.caption.Appearance.TextVAlign = VAlign.Middle;
            this.caption.Appearance.TextTrimming = TextTrimming.Character;
            this.caption.Visible = true;
            this.caption.Click += new EventHandler(caption_Click);
            this.caption.AfterEditText += new EventHandler(caption_AfterEditText);

            this.captionPanel.Controls.Add(this.caption);
            this.captionPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.captionPanel.Height = 30;

            this.elementPanel.Dock = DockStyle.Fill;
            this.elementPanel.Resize += new System.EventHandler(elementPanel_Resize);
            this.elementPanel.Paint += new System.Windows.Forms.PaintEventHandler(elementPanel_Paint);

            this.commentSplitter.Dock = DockStyle.Top;
            this.commentSplitter.Visible = false;

            this.comment.Dock = DockStyle.Fill;
            this.comment.BackColor = Color.FromKnownColor(KnownColor.Info);
            this.comment.Appearance.TextTrimming = TextTrimming.Character;
            this.comment.Appearance.FontData.SizeInPoints = 10;
            this.comment.Click += new EventHandler(comment_Click);
            this.comment.AfterEditText += new EventHandler(comment_AfterEditText);

            this.commentPanel.Controls.Add(this.comment);
            this.commentPanel.Dock = DockStyle.Top;
            this.commentPanel.Height = 30;
            this.commentPanel.Visible = false;

            this.commentAndElementPanel.Controls.Add(this.elementPanel);
            this.commentAndElementPanel.Controls.Add(this.commentSplitter);
            this.commentAndElementPanel.Controls.Add(this.commentPanel);
            this.commentAndElementPanel.Dock = System.Windows.Forms.DockStyle.Fill;

            this.Controls.Add(this.commentAndElementPanel);
            this.Controls.Add(this.captionSplitter);
            this.Controls.Add(this.captionPanel);

            this.ResumeLayout(false);
        }

        public virtual XmlNode Save()
        {
            XmlDocument dom = new XmlDocument();
            XmlNode elementProperties = dom.CreateElement(Common.Consts.properties);

            XmlHelper.SetAttribute(elementProperties, Common.Consts.titleElement, this.Title);
            XmlHelper.SetAttribute(elementProperties, Common.Consts.cubeName, this.PivotData.CubeName);
            XmlHelper.SetAttribute(elementProperties, Common.Consts.reportElemetType, this.ElementType.ToString());
            XmlHelper.SetAttribute(elementProperties, Common.Consts.isActive, this.IsActive.ToString());
            XmlHelper.SetAttribute(elementProperties, Common.Consts.uniqName, this.uniqName);
            this.Caption.Save(XmlHelper.AddChildNode(elementProperties, Common.Consts.elementCaption));
            this.Comment.Save(XmlHelper.AddChildNode(elementProperties, Common.Consts.elementComment));
            this.PivotData.Save(XmlHelper.AddChildNode(elementProperties, Common.Consts.pivotData));
            return elementProperties;
        }

        public virtual void Load(XmlNode reportElementNode, bool isForceDataUpdate) 
        {
            if (!this.PivotData.IsExistsConnection)
                this.MainForm.InitConnection();

            //При загрузке документа, имя куба должно содержаться в Xml, а при создании нового имя куба 
            //выставляется свойству this.PivotData.CubeName раньше, и равно самому себе
            this.PivotData.CubeName = XmlHelper.GetStringAttrValue(reportElementNode, Common.Consts.cubeName,
                this.PivotData.CubeName);
            //заголовок
            this.Title = XmlHelper.GetStringAttrValue(reportElementNode, Common.Consts.titleElement, this.PivotData.CubeName);

            if (reportElementNode == null)
                return;

            //уникальное имя
            this.uniqName = XmlHelper.GetStringAttrValue(reportElementNode, Common.Consts.uniqName, this.uniqName);
            //тип элемента
            string sElementType = XmlHelper.GetStringAttrValue(reportElementNode, Common.Consts.reportElemetType, string.Empty);
            this.ElementType = (ReportElementType)Enum.Parse(typeof(ReportElementType), sElementType);
            //Активность
            this.IsActive = XmlHelper.GetBoolAttrValue(reportElementNode, Common.Consts.isActive, false);
            //Заголовок
            this.Caption.Load(reportElementNode.SelectSingleNode(Common.Consts.elementCaption));
            //Комментарий
            this.Comment.Load(reportElementNode.SelectSingleNode(Common.Consts.elementComment));
        }

        public virtual void DoUndoRedoEvent(UndoRedoInfo undoInfo, EventType eventType)
        {
            bool fl = this.PivotData.IsDeferDataUpdating;

            bool isCompositeChart = false;
            if (this is ChartReportElement)
            {
                ChartReportElement chartElememt = ((ChartReportElement)this);
                isCompositeChart = (chartElememt.Chart.ChartType == Infragistics.UltraChart.Shared.Styles.ChartType.Composite);
            }

            bool isCustomData = false;
            if (this is MapReportElement)
            {
                MapReportElement mapElememt = ((MapReportElement)this);
                isCustomData = (mapElememt.DataSourceType == DataSourceType.Custom);
            }


            //запоминм было ли соединение
            bool isExistedConnection = this.MainForm.IsExistsConnection;

            if (isCompositeChart || isCustomData || this.CheckConnection())
            {
                try
                {
                    this.MainForm.Operation.StartOperation();
                    this.MainForm.Operation.Text = (eventType == EventType.Undo) ? 
                        "Отмена действия" : "Возврат действия";
                    this.Load(undoInfo.ElementProperties, false);
                    //Если соединение пропадало, насильно получаем данные с сервера.
                    //Т.к. ссылки на многомерную БД у старых данных по окончании сессии, 
                    //теряют актуальность.
                    UndoRedoEventType undoRedoEventType = isExistedConnection ? undoInfo.EventType : 
                        UndoRedoEventType.DataChange;

                    switch (undoRedoEventType)
                    {
                        case UndoRedoEventType.AppearanceChange:
                            {
                                if (!this.MainForm.Operation.Visible)
                                    this.MainForm.Operation.StartOperation();
                                this.MainForm.Operation.Text = "Отображение элемента";
                                this.MainForm.FieldListEditor.InitEditor(this);
                                this.PivotData.DoAppearanceChanged(true);
                                this.PivotData.DoStructureChanged();
                                break;
                            }
                        case UndoRedoEventType.DataChange:
                            {
                                this.PivotData.DoDataChanged();
                                break;
                            }
                    }

                    this.PivotData.IsDeferDataUpdating = fl;
                    this.MainForm.RefreshCompositeCharts();
                    this.MainForm.RefreshUserInterface(this);
                }
                finally
                {
                    this.MainForm.Operation.StopOperation();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.IsActive = false;
            }
            base.Dispose(disposing);
        }

        protected bool CheckConnection()
        {
            bool result = true;
            try
            {
                if (!this.MainForm.IsExistsConnection)
                {
                    result = MainForm.InitConnection();
                }
                else
                {
                    if ((PivotData.CubeName != string.Empty) && (PivotData.Cube == null))
                    {
                        //result = MainForm.InitConnection(); 
                        result = false;
                    }
                }
            }
            catch
            {
                result = false;
            }
            //Выставим признак 
            this.IsCubeNotFond = !result;

            return result;
        }

        /// <summary>
        /// Выполняем запрос
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public bool ExecuteMDXQuery(string query)
        {
            this.MainForm.Operation.StartOperation();
            this.MainForm.Operation.Text = "Загрузка данных";
            DateTime startQuery = DateTime.Now;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                this.PivotData.MDXQuery = query;
                SetMDXQuery(query);
            }
            catch
            {
                return false;
            }
            finally
            {
                Cursor.Current = Cursors.Default;

                //подщитываем сколько длился запрос (в секундах)
                TimeSpan timeSpan = DateTime.Now - startQuery;
                double duration = timeSpan.TotalMilliseconds / 1000d;
                this.MainForm.QueryTime = duration.ToString("N2");
                this.MainForm.Operation.StopOperation();
            }
            return true;
        }

        /// <summary>
        /// Провереяет есть ли сессия с которой работает элемент отчета
        /// </summary>
        /// <param name="mdxQueryBuilder"></param>
        /// <returns></returns>
        private bool CheckSession(MDXQueryBuilder mdxQueryBuilder)
        {
            bool result = true;
            //Запросим первую строчку из сессии
            string query = mdxQueryBuilder.BuildMDXQueryForFirstTupleOfRows(this.PivotData);
            try
            {
                this.MainForm.MdxCommand.ExecuteNonQuery(query);
            }
            catch (AdomdErrorResponseException e)
            {
                result = false;
            }
            return result;
        }

        /// <returns>Вернет элементы строки предпоследнего уровня</returns>
        private List<string> PenultimateLevelMembers()
        {
            List<string> result = new List<string>();
            MDXQueryBuilder mdxQueryBuilder = new MDXQueryBuilder();
            mdxQueryBuilder.ElementType = ReportElementType.eTable;
            string query = mdxQueryBuilder.BuildMDXQueryForPenultimateMembers(this.PivotData);

            if (string.IsNullOrEmpty(query))
                return result;

            try
            {
                this.MainForm.Operation.StartOperation();
                this.MainForm.Operation.Text = "Получение элементов для расчета среднего";
                CellSet cls = this.MainForm.MdxCommand.Execute(query, PivotData.AdomdConn);
                foreach (Position pos in cls.Axes[0].Positions)
                {
                    result.Add(pos.Members[0].UniqueName);
                }
                return result;
            }
            catch
            {
                #warning Исключения не обрабатываются!
                return result;
            }
            finally
            {
                this.MainForm.Operation.StopOperation();
            }
        }


        /// <summary>
        /// Обновить данные элемента (целиком) (В TableReportElement, данный метод перегружен, 
        /// ниже расположенный код будет выполняться только если таблица размещается на одной странице)
        /// </summary>
        protected virtual void RefreshData()
        {
            if (!CheckConnection())
                return;

            MDXQueryBuilder mdxQueryBuilder = new MDXQueryBuilder();
            if ((this.PivotData.AverageSettings.AverageType != AverageType.None)||(this.PivotData.MedianSettings.IsMedianCalculate))
            {
                mdxQueryBuilder.PenultimateLevelMembers = this.PenultimateLevelMembers();
            }

            mdxQueryBuilder.ElementType = this.elementType;
            mdxQueryBuilder.usePaging = false;

            /* Сейчас это проверка идет в CheckConnection
             * 
            if (this.MainForm.IsNeedReconnectAdomdConnection(this.PivotData.CubeName))
                //Для ресета подключения есть много причин описанных в задаче 10237 10812
                this.MainForm.ResetAdomdConnection();*/

            string mdxQuery = this.PivotData.IsCustomMDX ? this.PivotData.MDXQuery :
                mdxQueryBuilder.BuildMDXQuery(this.PivotData);
            //if (!String.IsNullOrEmpty(mdxQuery))
            
            this.ExecuteMDXQuery(mdxQuery);

            //если элемент синхронизируется с таблицей - не будет сохранять точку отката,
            //чтобы при отмене действия синронно отменялись события в таблице и синхр. элементе
            string boundTo = String.Empty;
            if (this.ElementType == ReportElementType.eChart)
            {
                boundTo = ((ChartReportElement)this).Synchronization.BoundTo;
            }
            else
                if (this.ElementType == ReportElementType.eMap)
                {
                    boundTo = ((MapReportElement) this).Synchronization.BoundTo;
                }
                else
                    if (this.ElementType == ReportElementType.eGauge)
                    {
                        boundTo = ((GaugeReportElement) this).Synchronization.BoundTo;
                    }
                    else
                        if (this.ElementType == ReportElementType.eMultiGauge)
                        {
                            boundTo = ((MultipleGaugeReportElement)this).Synchronization.BoundTo;
                        }

            if (boundTo != String.Empty)
            {
                if (this.MainForm.FindTableReportElement(boundTo) != null)
                    return;
            }

            this.MainForm.Saved = false;
            this.MainForm.UndoRedoManager.AddEvent(this, UndoRedoEventType.DataChange);
        }

        /// <summary>
        /// Обновить данные постранично, в указанном диапозоне
        /// </summary>
        /// <param name="lowRow"></param>
        protected void RefreshData(int lowRow, int pageSize)
        {
            if (!CheckConnection())
                return;

            //будем использовать режим сессионого сета для строк
            bool useSessionSet = false;//(lowRow > 0);

            MDXQueryBuilder mdxQueryBuilder = new MDXQueryBuilder();
            mdxQueryBuilder.ElementType = this.elementType;
            mdxQueryBuilder.usePaging = true;
            mdxQueryBuilder.lowBorder = lowRow;
            mdxQueryBuilder.pageSize = pageSize;

            this.MainForm.Operation.StartOperation();
            try
            {
                //Если таблица большая, принудительно включаем NonEmptyCrossJoin иначе будет торомозить
                //this.useNECJ = true;   

                //Если это не первая страница включаем режим сессионого множества строк.
                if (useSessionSet)
                {
                    this.MainForm.Operation.Text = "Создание сессионого множества";
                    string commandText = mdxQueryBuilder.BuildMDXQueryForRowsSessionSet(this.PivotData);
                    this.MainForm.MdxCommand.ExecuteNonQuery(commandText, PivotData.AdomdConn);
                    mdxQueryBuilder.useRowsSessionSet = true;

                    //Берем первый кортеж страницы
                    mdxQueryBuilder.headRowSet = string.Empty;

                    commandText = mdxQueryBuilder.BuildMDXQueryForFirstTupleOfRows(this.PivotData);
                    try
                    {
                        CellSet cls = this.MainForm.MdxCommand.Execute(commandText);
                        //на основании него, инициализируем начальное множество строк
                        mdxQueryBuilder.headRowSet = mdxQueryBuilder.HeaderSetForPage(cls.Axes[0].Positions[0], this.PivotData);
                    }
                    catch (Exception)
                    {
                        mdxQueryBuilder.headRowSet = string.Empty;
                    }
                }

                this.ExecuteMDXQuery(mdxQueryBuilder.BuildMDXQuery(this.PivotData));
                this.MainForm.Saved = false;
                this.MainForm.UndoRedoManager.AddEvent(this, UndoRedoEventType.DataChange);

                //нужно грохнуть все, что касается созданной сессии
                if (useSessionSet)
                {
                    this.DropSession(mdxQueryBuilder);
                }
            }
            finally
            {
                this.MainForm.Operation.StopOperation();
            }
        }

        private void DropSession(MDXQueryBuilder mdxQueryBuilder)
        {
            this.MainForm.MdxCommand.ExecuteNonQuery(mdxQueryBuilder.MDXDropRowsSessionSet(this.PivotData));
            this.MainForm.MdxCommand.ExecuteNonQuery(mdxQueryBuilder.MDXDropNonFilterRowSessionSet(this.PivotData));
            this.MainForm.MdxCommand.ExecuteNonQuery(mdxQueryBuilder.MDXDropVisualTotals(this.PivotData));
            for (int i = 0; i < this.PivotData.TotalAxis.Totals.Count; i++)
            {
                string dropString = mdxQueryBuilder.MDXDropCalulateMember(this.PivotData, i);
                if (dropString != string.Empty)
                    this.MainForm.MdxCommand.ExecuteNonQuery(dropString);
                
                dropString = mdxQueryBuilder.MDXDropLookupCubeMember(this.PivotData.TotalAxis.Totals[i]);
                if (dropString != string.Empty)
                    this.MainForm.MdxCommand.ExecuteNonQuery(dropString);
            }

            foreach (FieldSet fs in this.PivotData.FilterAxis.FieldSets)
            {
                string dropString = mdxQueryBuilder.MDXDropSessionFilterSet(this.PivotData, fs);
                if (dropString != string.Empty)
                    this.MainForm.MdxCommand.ExecuteNonQuery(dropString);
            }

            foreach (FieldSet fs in this.PivotData.ColumnAxis.FieldSets)
            {
                string dropString = mdxQueryBuilder.MDXDropSessionFilterSet(this.PivotData, fs);
                if (dropString != string.Empty)
                    this.MainForm.MdxCommand.ExecuteNonQuery(dropString);
            }
        }

        private void SetActive(bool value)
        {
            if (this.isActive == value)
            {
                return;
            }

            if (Parent == null)
            {
                return;
            }

            DockableControlPane pane = ((DockableWindow)Parent).Pane;
            CustomReportElement elem;

            foreach (DockableControlPane p in pane.Manager.ControlPanes) 
            {
                elem = ((CustomReportElement)p.Control);
                if (elem != this)
                {
                    if (elem.IsActive)
                    {
                        elem.IsActive = false;
                    }
                }
            }
            
            isActive = value;
            if (value)
            {
                pane.Show();
                pane.Activate();
                this.MainForm.SetActivityIndicator(pane);
            }
            else
            {
                pane.Settings.Appearance.Image = null;
            }
        }

        /// <summary>
        /// Получить изображение элемента из указанной области
        /// </summary>
        /// <param name="bitmapBounds"></param>
        /// <returns></returns>
        public virtual Bitmap GetBitmap(Rectangle bitmapBounds)
        {
            int sourceWidth = this.Width;
            int sourceHeight = this.Height;

            Bitmap bitmap = null;
            try
            {
                this.ElementPlace.AutoScroll = false;
                this.Width = bitmapBounds.Width + bitmapBounds.X;
                this.Height = bitmapBounds.Height + bitmapBounds.Y;
                
                bitmap = new Bitmap(this.Width, this.Height);
                bitmap.SetResolution(100, 100);

                //Вот здесть очень странная вещь происходит, после переноса изображения с 
                //нашего контрола в bimap, если элемент карта, включен заголовок и легенда, 
                //изображение обрезается... Выходи есть, срисовывать изображение в два подхода,
                //сначала с всего элемента, потом только с панели на которой находится карта...
                
                //Срисовывываем весь элемент
                this.DrawToBitmap(bitmap, bitmapBounds);
                //Определяем расположение панели на которой находиться карта/таблица/диаграмма
                bitmapBounds.Location = this.commentAndElementPanel.Location;
                bitmapBounds.Location = new Point(
                    this.commentAndElementPanel.Location.X + this.ElementPlace.Location.X,
                    this.commentAndElementPanel.Location.Y + this.ElementPlace.Location.Y);
                //Накладываем на раннее полученное изображение, картинку с панели с картой/таблицей/диаграммой
                this.ElementPlace.DrawToBitmap(bitmap, bitmapBounds);
            }
            finally
            {
                this.Width = sourceWidth;
                this.Height = sourceHeight;
                this.ElementPlace.AutoScroll = true;
            }
            return bitmap;
        }

        private void SetCubeName(string value)
        {
            /*
            if (String.IsNullOrEmpty(value))
            {
                MessageBox.Show("Имя куба не может быть пустым.", "MDX Эксперт", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            */

            this.PivotData.CubeName = value;
            this.MainForm.RefreshElementData(this);
        }

        /// <summary>
        /// Является ли элемент картой по пользовательским данным
        /// </summary>
        public bool IsCustomMap
        {
            get
            {
                if (this.ElementType == ReportElementType.eMap)
                {
                    return (((MapReportElement)this).DataSourceType == DataSourceType.Custom);
                }
                return false;
            }
        }


        #region Печать элемента
        private PrintDocument GetPrintDocument()
        {
            PrintDocument document = new PrintDocument();
            document.BeginPrint += new PrintEventHandler(document_BeginPrint);
            document.PrintPage += new PrintPageEventHandler(document_PrintPage);
            document.EndPrint += new PrintEventHandler(document_EndPrint);
            return document;
        }

        void document_BeginPrint(object sender, PrintEventArgs e)
        {
            this.curentPrintablePage = 0;
        }

        void document_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (this.printableImage == null)
            {
                this.printableImage = GetPrintableImage(new Rectangle(0, 0,
                    e.MarginBounds.Width, e.MarginBounds.Height));
            }

            e.HasMorePages = true;

            //Смещение страниц
            Point[] ptOffsets = GeneratePrintingOffsets(this.printableImage, e.MarginBounds);

            //Область печати, ограниченная отступами 
            e.Graphics.SetClip(e.MarginBounds);

            // смещение
            Point ptOffset = new Point(-ptOffsets[curentPrintablePage].X, -ptOffsets[curentPrintablePage].Y);
            ptOffset.Offset(e.MarginBounds.X, e.MarginBounds.Y);

            // на нескольких
            e.Graphics.DrawImage(this.printableImage, ptOffset);

            //Проверка на необходимость вывода следующей страницы
            e.HasMorePages = (curentPrintablePage < ptOffsets.Length - 1);

            //следущая страница
            curentPrintablePage++;
        }

        void document_EndPrint(object sender, PrintEventArgs e)
        {
            this.printableImage.Dispose();
            this.printableImage = null;
        }

        /// <summary>
        /// Гинерируем точки смещения (если рисунок выходит за границы печати)
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        private Point[] GeneratePrintingOffsets(Bitmap image, Rectangle rect)
        {
            if (image == null)
                return null;

            int x = (int)Math.Ceiling((double)(image.Width) / (double)(rect.Width));
            int y = (int)Math.Ceiling((double)(image.Height) / (double)(rect.Height));
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
                for (int j = 0; j < x; j++)
                    arrPoint[i * x + j] = new Point(j * rect.Width, i * rect.Height);

            return arrPoint;
        }
        #endregion

        #region Абстрактыне методы
        /// <summary>
        /// Получить полное (включая и область не отображающуюся на экране) изображение элемента
        /// </summary>
        /// <returns></returns>
        public abstract Bitmap GetBitmap();

        /// <summary>
        /// Получить изображение, для печати
        /// </summary>
        /// <param name="pageBounds">границы печатного листа</param>
        /// <returns></returns>
        public abstract Bitmap GetPrintableImage(Rectangle pageBounds);

        /// <summary>
        /// Инициализирует элемент отчета по CellSet
        /// </summary>
        /// <param name="cls"></param>
        public virtual void InitialByCellSet(CellSet cls)
        {
            Operation oparation = this.MainForm.Operation;
            bool isOperationAppear = oparation.Visible;
            oparation.StartOperation();
            oparation.Text = "Анализ полученных данных";
            try
            {
                this.PivotData.Initialize(cls);
                if (this.PivotData.IsCustomMDX)
                    this.MainForm.RefreshUserInterface(this);
            }
            finally
            {
                if (!isOperationAppear)
                    oparation.StopOperation();
            }
        }

        /// <summary>
        /// Передача запроса объекту, кот. отображает данные (таблице, диаграмме, карте)
        /// </summary>
        /// <param name="mdxQuery"></param>
        protected virtual CellSet SetMDXQuery(string mdxQuery)
        {
            try
            {
                this.CLS = this.MainForm.MdxCommand.Execute(mdxQuery, PivotData.AdomdConn);
            }
            catch (Exception exc)
            {
                Exception e = exc;
                if (AdomdExceptionHandler.IsAdomdException(e))
                {
                    if (AdomdExceptionHandler.ProcessOK(e))
                    {
                        AdomdExceptionHandler.IsRepeatedProcess = true;
                        this.CLS = SetMDXQuery(mdxQuery);
                        AdomdExceptionHandler.IsRepeatedProcess = false;
                        return this.CLS;
                    }
                    else
                    {
                        if (this.PivotData.IsCustomMDX)
                            e = new Exception("Ошибка в пользовательском MDX запросе: " + exc.Message,
                                exc.InnerException);
                    }
                }
                this.CLS = null;
                throw e;
            }
            return this.CLS;
        }

        /// <summary>
        /// Инициализация элемента отчета, по ранее полученому CellSet-у
        /// </summary>
        public void InitialByCellSet()
        {
            this.InitialByCellSet(this.CLS);
        }

        /// <summary>
        /// Устанавливает видимость элемента (таблицу, диаграмму, карту)
        /// </summary>
        public abstract void SetElementVisible(bool value);

        #endregion

        #region события

        //при деактивации элемента
        private ReportElementEventHandler _deactivated = null;

        public event ReportElementEventHandler Deactivated
        {
            add { _deactivated += value; }
            remove { _deactivated -= value; }
        }

        public void DoDeactivated()
        {
            if (_deactivated != null)
                _deactivated();
        }

        #endregion

        #region Обработчики событий

        private void caption_Click(object sender, EventArgs e)
        {
            this.PivotData.SetSelection(SelectionType.GeneralArea, string.Empty);
        }

        void comment_Click(object sender, EventArgs e)
        {
            this.PivotData.SetSelection(SelectionType.GeneralArea, string.Empty);
        }

        void caption_AfterEditText(object sender, EventArgs e)
        {
            this.MainForm.Saved = false;
        }

        void comment_AfterEditText(object sender, EventArgs e)
        {
            this.MainForm.Saved = false;
        }

        private void elementPanel_Paint(object sender, PaintEventArgs e)
        {
            this.SetElementVisible(!this.IsShowErrorMessage);
            if (this.IsShowErrorMessage)
            {
                e.Graphics.DrawString(this.ErrorMessageText,
                                      this.nullDataMessage.Font,
                                      this.nullDataMessage.Brush,
                                      this.ElementPlace.Bounds,
                                      this.nullDataMessage.Format);
            }
        }

        private void elementPanel_Resize(object sender, EventArgs e)
        {
            this.ElementPlace.Invalidate();
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Пользовательский интерфейс грида (намного удобнее чем проверять, является ли этот элемент
        /// таблицей, потом если является то приводим к элемент к TableReportElement, и только у него 
        /// получаем то что нам нужно), все остальные элементы (у которых нет данного интерфейса) просто 
        /// возвращают null
        /// </summary>
        public abstract IGridUserInterface GridUserInterface { get; }

        /// <summary>
        /// Структура данных элемента
        /// </summary>
        public PivotData PivotData
        {
            get
            {
                return _pivotData;
            }
            set
            {
                _pivotData = value;
            }
        }

        /// <summary>
        /// Тип элемента
        /// </summary>
        public ReportElementType ElementType
        {
            get { return elementType; }
            set { elementType = value; }
        }

        /// <summary>
        /// Активность элемента
        /// </summary>
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                if (isActive == value)
                {
                    return;
                }

                SetActive(value);

                if (!value)
                {
                    this.DoDeactivated();
                }
            }
        }

        public MainForm MainForm
        {
            get { return _mainForm; }
            set { _mainForm = value; }
        }

        /// <summary>
        /// Заголовок элемента
        /// </summary>
        public ElementCaption Caption
        {
            get { return this.caption; }
        }

        /// <summary>
        /// Комментарий элемента
        /// </summary>
        public ElementComment Comment
        {
            get { return this.comment; }
        }

        /// <summary>
        /// Панель на которой размещается елемент отчета
        /// </summary>
        public Panel ElementPlace
        {
            get { return this.elementPanel; }
        }

        /// <summary>
        /// Экспортер
        /// </summary>
        internal ElementExporter Exporter
        {
            get { return _exporter; }
            set { _exporter = value; }
        }

        /// <summary>
        /// Получить документ для печати
        /// </summary>
        public PrintDocument PrintDocumet
        {
            get { return this.GetPrintDocument(); }
        }

        /// <summary>
        /// Надпись на окне элемента
        /// </summary>
        public string Title
        {
            get 
            {
                string result = string.Empty;
                if ((this.Parent != null) && (this.Parent is DockableWindow))
                    result = ((DockableWindow)this.Parent).Pane.Text;
                return result;
            }
            set 
            {
                if ((this.Parent != null) && (this.Parent is DockableWindow))
                    ((DockableWindow)this.Parent).Pane.Text = value;
            }
        }

        /// <summary>
        /// Сообщение, выводимое при отсутствии куба
        /// </summary>
        public string NullDataMessage
        {
            get
            {
                string result = string.Empty;
                if (this.PivotData.Cube == null)
                {
                    result = "Нет данных. Куб \"" + this.PivotData.CubeName + "\" не найден.";
                }
                return result;
            }
        }

        /// <summary>
        /// Вместо элемента показывать ошибку
        /// </summary>
        public abstract bool IsShowErrorMessage { get; }

        /// <summary>
        /// Текст ошибки, отображаемые в окне элемента
        /// </summary>
        public string ErrorMessageText
        {
            get { return _errorMessageText; }
            set { _errorMessageText = value; }
        }

        /// <summary>
        /// Куб на который ссылается данный элемент отчета, отсутствует.
        /// </summary>
        public bool IsCubeNotFond
        {
            get { return _isCubeNotFond; }
            set 
            {
                this.ErrorMessageText = this.NullDataMessage;
                if (_isCubeNotFond != value)
                {
                    _isCubeNotFond = value;
                    this.Invalidate(true);
                }
            }
        }

        /// <summary>
        /// Данные полученные в результате последнего запроса
        /// </summary>
        public CellSet CLS
        {
            get { return _cls; }
            set { _cls = value; }
        }

        /// <summary>
        /// Уникальное имя
        /// </summary>
        public string UniqueName
        {
            get { return uniqName; }
            set { uniqName = value; }
        }

        #endregion
    }

    public class MessageAppearance
    {
        private Font font;
        private Brush brush;
        private StringFormat format;
        private string text;

        public Font Font
        {
            get { return font; }
            set { font = value; }
        }
        
        public Brush Brush
        {
            get { return brush; }
            set { brush = value; }
        }
        
        public StringFormat Format
        {
            get { return format; }
            set { format = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public MessageAppearance()
        {
            this.font = new Font("Serif", 20);
            this.brush = new SolidBrush(Color.Red);
            this.format = new StringFormat();
            this.format.Alignment = StringAlignment.Center;
            this.format.LineAlignment = StringAlignment.Center;
            this.text = "";
        }       
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Infragistics.UltraGauge.Resources;
using Infragistics.Win.Layout;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinGauge;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Controls;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Grid;
using Krista.FM.Client.MDXExpert.Grid.UserInterface;
using Krista.FM.Common.Xml;
using Microsoft.AnalysisServices.AdomdClient;
using System.Drawing;
using System.Xml;
using System.IO;
using AxisType = Krista.FM.Client.MDXExpert.Data.AxisType;

namespace Krista.FM.Client.MDXExpert
{
    public class MultipleGaugeReportElement : CustomReportElement
    {
        private ExpertGauge _mainGauge;
        private ExpertGaugeCollection _gauges;
        private GaugesLocation _gaugesLocation;
        private bool _isUpdatable = true;

        private string _presetName;
        private MultipleGaugeSynchronization _synchronization;
        //private GaugeSynchronization _synchronization;

        private bool _autoTickmarkCalculation;
        private bool _isNeedRefresh;

        private ExpertLegend _legend;
        private UltraGridBagLayoutPanel gridBagPanel;

        private const string seriesColumnName = "Series Name";

        private GaugeType _gaugeType;



        private EventHandler _rangeCollectionChanged = null;

        //Разделитель элементов сейрий
        private DataSeriesSeparator rowsSeparator = DataSeriesSeparator.eComma;
        //Разделитель элементов категорий
        private DataSeriesSeparator columnsSeparator = DataSeriesSeparator.eComma;

        private DataTable _sourceDT;
        /// <summary>
        /// в поле храним индексы кортежей с которых надо брать данные столбцам (категориям)
        /// </summary>
        List<int> columnPosNum = new List<int>();
        /// <summary>
        /// в поле храним индексы кортежей с которых надо брать данные строкам (сериям)
        /// </summary>
        List<int> rowPosNum = new List<int>();

        public GaugeType GaugeType
        {
            get { return this._gaugeType; }
            set
            {
                this._gaugeType = value;
                //SetGaugeType(value);
            }
        }

        /// <summary>
        /// Индикатор-прототип, на основе него строятся все остальные индикаторы
        /// </summary>
        public ExpertGauge MainGauge
        {
            get { return this._mainGauge; }
            set { this._mainGauge = value; }
        }

        public ExpertGaugeCollection Gauges
        {
            get { return this._gauges; }
            set { this._gauges = value; }
        }

        /// <summary>
        /// Имя текущей настройки для индикатора
        /// </summary>
        public string PresetName
        {
            get { return this._presetName; }
            set
            {
                this.ElementPlace.SuspendLayout();
                this._presetName = value;
                this.MainGauge.PresetName = value;
                foreach(ExpertGauge gauge in this.Gauges)
                {
                    gauge.PresetName = value;
                }
                this.ElementPlace.ResumeLayout();
                this.Refresh();
            }
        }

        /// <summary>
        /// Начальное значение
        /// </summary>
        public double StartValue
        {
            get { return GetStartValue(); }
            set { SetStartValue(value); }
        }

        /// <summary>
        /// Конечное значение
        /// </summary>
        public double EndValue
        {
            get { return GetEndValue(); }
            set { SetEndValue(value); }
        }

        /// <summary>
        /// Значение
        /// </summary>
        public double Value
        {
            get { return GetValue(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Подпись
        /// </summary>
        public string Text
        {
            get { return GetText(); }
            set { SetText(value); }
        }

        /// <summary>
        /// Шрифт подписи
        /// </summary>
        public Font TextFont
        {
            get { return GetTextFont(); }
            set { SetTextFont(value); }
        }

        /// <summary>
        /// Интервал между метками
        /// </summary>
        public double TickmarkInterval
        {
            get { return GetTickmarkInterval(); }
            set { SetTickmarkInterval(value); }
        }

        /// <summary>
        /// Автоматический расчет интервала между метками
        /// </summary>
        public bool AutoTickmarkCalculation
        {
            get { return GetAutoTickmarkCalculation(); }
            set { SetAutoTickmarkCalculation(value); }
        }

        /// <summary>
        /// Поля индикаторов
        /// </summary>
        public Margin Margins
        {
            get { return this.MainGauge.Margin; }
            set
            {
                this.MainGauge.Margin = value;
                foreach(ExpertGauge gauge in this.Gauges)
                {
                    gauge.Margin = value;
                }
            }
        }

        /// <summary>
        /// Синхронизация множественного индикатора с таблицей
        /// </summary>
        public MultipleGaugeSynchronization Synchronization
        {
            get { return _synchronization; }
            set { _synchronization = value; }
        }
        /*
        /// <summary>
        /// Синхронизация обычного индикатора с таблицей
        /// </summary>
        public GaugeSynchronization Synchronization
        {
            get { return _synchronization; }
            set { _synchronization = value; }
        }
        */

        /// <summary>
        /// Нужно ли перерисовывать индикатор
        /// </summary>
        public bool IsNeedRefresh
        {
            get { return _isNeedRefresh; }
            set { _isNeedRefresh = value; }
        }

        /// <summary>
        /// Цветовые интервалы
        /// </summary>
        public GaugeColorRangeCollection ColorRanges
        {
            get { return GetColorRanges(); }
            set { SetColorRanges(value); }
        }

        public List<GaugeRange> VisibleColorRanges
        {
            get { return GetVisibleColorRanges(); }
            set { SetVisibleColorRanges(value); }
        }



        public GaugesLocation GaugesLocation
        {
            get { return this._gaugesLocation; }
            set { this._gaugesLocation = value; }
        }

        /// <summary>
        /// Разделитель элементов серий
        /// </summary>
        public DataSeriesSeparator RowsSeparator
        {
            get { return rowsSeparator; }
            set
            {
                rowsSeparator = value;
                OnPivotDataChange();
            }
        }

        /// <summary>
        /// Разделитель элементов категорий
        /// </summary>
        public DataSeriesSeparator ColumnsSeparator
        {
            get { return columnsSeparator; }
            set
            {
                columnsSeparator = value;
                OnPivotDataChange();
            }
        }

        /// <summary>
        /// Признак что данные берутся из базы
        /// </summary>
        public bool IsUpdatable
        {
            get { return _isUpdatable; }
            set
            {
                if (_isUpdatable != value)
                {
                    _isUpdatable = value;
                    this.Invalidate(true);
                }
            }
        }


        public DataTable SourceDT
        {
            get { return _sourceDT; }
            set
            {
                _sourceDT = value;
                BindData();
            }
        }


        public MultipleGaugeReportElement(MainForm mainForm)
            : base(mainForm, ReportElementType.eMultiGauge)
        {
            
            string firstPreset = GetFirstPresetName();

            this.MainGauge = new ExpertGauge(firstPreset);
            this.Gauges = new ExpertGaugeCollection(this);

            this.PresetName = firstPreset;

            this.ElementPlace.SuspendLayout();
            this.ElementPlace.AutoScroll = true;

            this.gridBagPanel = new UltraGridBagLayoutPanel();
            this.gridBagPanel.Parent = this.ElementPlace;
            this.gridBagPanel.Dock = DockStyle.Fill;
            this.gridBagPanel.AutoSize = true;
            this.gridBagPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gridBagPanel.ExpandToFitHeight = true;
            this.gridBagPanel.ExpandToFitWidth = true;

            this.GaugesLocation = new GaugesLocation(this);
            this.GaugesLocation.Changed += new LocationChangedEventHandler(GaugesLocation_Changed);

            this._legend = new ExpertLegend();
            this._legend.Parent = this.ElementPlace;
            this._legend.Location = LegendLocation.Right;
            this._legend.LegendSize = 200;
            this._legend.Visible = false;

            //this.Gauges[0].MouseClick += new MouseEventHandler(gauge_MouseClick);

            this.ElementType = ReportElementType.eMultiGauge;

            this.PivotData.ColumnAxis.Caption = "Показатели";
            this.PivotData.RowAxis.Caption = "Ряды";


            this.Synchronization = new MultipleGaugeSynchronization(this);
            //this.Synchronization = new GaugeSynchronization(this);
            this.IsNeedRefresh = true;


            //this.AutoTickmarkCalculation = true;
            this.PutGauges(true);

            //Восстановим отрисовку
            this.ElementPlace.ResumeLayout();

            this.PivotData.DataChanged += new PivotDataEventHandler(OnPivotDataChange);
            this.PivotData.StructureChanged += new PivotDataEventHandler(PivotData_StructureChanged);
            //this.PivotData.ElementsOrderChanged += new PivotDataChangeOrderEventHandler(PivotData_ElemOrderChanged);
            //this.PivotData.ElementsSortChanged += new PivotDataChangeSortEventHandler(PivotData_ElemSortChanged);
            this.PivotData.AppearanceChanged += new PivotDataAppChangeEventHandler(PivotData_AppearanceChanged);

        }




        /// <summary>
        /// Привязать данные из таблицы источнисчника к индикаторам
        /// </summary>
        private void BindData()
        {
            try
            {
                this.MainForm.Operation.StartOperation();
                this.MainForm.Operation.Text = "Анализ полученных данных...";

                this.ElementPlace.SuspendLayout();


                Font labelsFont = null;
                string labelsFormatString = String.Empty;

                if (this.MainGauge != null)
                {
                    labelsFont = this.MainGauge.LabelsFont;
                    labelsFormatString = this.MainGauge.LabelsFormatString;
                }

                GaugeColorRangeCollection colorRanges = this.ColorRanges;

                this.SetMultiGaugeDataSource(this.SourceDT);
                this.GaugesLocation.Columns = this.GaugesLocation.Columns;

                foreach (ExpertGauge gauge in this.Gauges)
                {
                    if ((labelsFont != null))
                        gauge.LabelsFont = labelsFont;

                    if (!String.IsNullOrEmpty(labelsFormatString))
                        gauge.LabelsFormatString = labelsFormatString;
                }

                RefreshAnnotations();
                this.ColorRanges = colorRanges;
                this.Margins = this.MainGauge.Margin;
            }
            finally
            {
                this.ElementPlace.ResumeLayout();
                this.MainForm.Operation.StopOperation();
            }
        }


        /// <summary>
        /// Получение структуры из таблицы, если с ней синхронизирована карта
        /// </summary>
        /// <returns>true - если структура получена</returns>
        private bool GetSyncronizedPivotData(bool isForceDataUpdate)
        {
            if (this.Synchronization.BoundTo != String.Empty)
            {
                TableReportElement tableElement = this.MainForm.FindTableReportElement(this.Synchronization.BoundTo);
                if (tableElement != null)
                {
                    bool isDeferDataUpdating = this.PivotData.IsDeferDataUpdating;
                    this.PivotData.IsDeferDataUpdating = true;
                    Synchronize(tableElement.PivotData, false, !isForceDataUpdate);
                    this.PivotData.IsDeferDataUpdating = isDeferDataUpdating;
                    return true;
                }

            }
            return false;
        }


        public void OnPivotDataChange()
        {
            GetSyncronizedPivotData(false);
            //Получаем данные с сервера
            RefreshData();
        }

        void PivotData_StructureChanged()
        {
            //Обновляем индикаторы по ранее построенным данным
            if (this.IsUpdatable)
                this.InitialByCellSet();
        }

        private void PivotData_AppearanceChanged(bool isNeedRecalculateGrid)
        {
            if (this.IsUpdatable)
            {
                if (this.CLS == null)
                {
                    RefreshData();
                }
                else
                {
                    this.InitialByCellSet();
                }
            }
        }

        /// <summary>
        /// Количество индикаторов, которые будем отображать
        /// </summary>
        /// <returns></returns>
        private int GetGaugesCount()
        {
            if (this.SourceDT == null)
                return 0;

            if (this.IsSeriesExists())
            {
                return Math.Min(this.SourceDT.Columns.Count*this.SourceDT.Rows.Count, this.GaugesLocation.MaxCount);
            }
            else
            {
                return Math.Min((this.SourceDT.Columns.Count-1) * this.SourceDT.Rows.Count, this.GaugesLocation.MaxCount);
            }
        }

        /// <summary>
        /// Есть ли в таблице - источнике столбец с названиями серий
        /// </summary>
        /// <returns></returns>
        private bool IsSeriesExists()
        {
            bool result = false;
            if ((this.SourceDT != null) && (this.SourceDT.Columns.Count > 0))
                result = this.SourceDT.Columns[0].ColumnName == seriesColumnName;
            return result;
        }

        private double CalcStartValue()
        {
            double result = 0;
            int gaugesCount = GetGaugesCount();
            int k = 0;

            int startIndex = IsSeriesExists() ? 1 : 0;

            if (this.SourceDT != null)
            {
                for (int i = startIndex; i < this.SourceDT.Columns.Count; i++)
                {
                    foreach (DataRow row in this.SourceDT.Rows)
                    {
                        if (row[i] != DBNull.Value)
                        {
                            double value = Double.Parse(row[i].ToString());
                            result = Math.Min(result, value);
                        }
                        k++;
                        if (k >= gaugesCount)
                            return result;
                    }
                }
            }
            return result;
        }

        private double CalcEndValue()
        {
            double result = 0;
            int gaugesCount = GetGaugesCount();
            int k = 0;
            int startIndex = IsSeriesExists() ? 1 : 0;

            if (this.SourceDT != null)
            {
                for (int i = startIndex; i < this.SourceDT.Columns.Count; i++)
                {
                    foreach (DataRow row in this.SourceDT.Rows)
                    {
                        if (row[i] != DBNull.Value)
                        {
                            double value = Double.Parse(row[i].ToString());
                            result = Math.Max(result, value);
                        }
                        k++;
                        if (k >= gaugesCount)
                            return result;
                    }
                }
            }
            return result;
        }



        private void SetMultiGaugeDataSource(DataTable dt)
        {
            //this.IsNotValidData = this.GetNegativeOrEmptyValueExists(dt);
            this.Gauges.Clear();
            if (dt == null)
                return;

            double startValue = this.CalcStartValue();
            double endValue = this.CalcEndValue();

            //this.MainGauge.SetValues(startValue, endValue, startValue);
            this.MainGauge.StartValue = startValue;
            this.MainGauge.EndValue = endValue;

            int gaugesCount = GetGaugesCount();
            int k = 0;
            int startIndex = IsSeriesExists() ? 1 : 0;

            for (int i = startIndex; i < dt.Columns.Count; i++)
            {
                foreach(DataRow row in dt.Rows)
                {
                    ExpertGauge newGauge = new ExpertGauge(this.PresetName);
                    this.Gauges.Add(newGauge);
                    newGauge.AutoTickmarkCalculation = this.AutoTickmarkCalculation;
                    newGauge.Tag = GetTextForGauge(dt.Columns[i], row);


                    //newGauge.Text = GetTextForGauge(dt.Columns[i], row);

                    double currValue = row[i] != DBNull.Value ? Double.Parse(row[i].ToString()) : startValue;

                    newGauge.SetValuesWithoutCalculation(this.StartValue, this.EndValue, currValue, this.TickmarkInterval);

                    newGauge.SizeChanged += new EventHandler(newGauge_SizeChanged);

                    k++;
                    if (k >= gaugesCount)
                    {
                        return;
                    }
                }
            }
        }


        void newGauge_SizeChanged(object sender, EventArgs e)
        {
            ExpertGauge gauge = (ExpertGauge) sender;
            if (gauge.Tag != null)
                gauge.Text = WrapGaugeText((string)gauge.Tag, gauge.Width);
        }

        public void RefreshGaugesText()
        {
            foreach(ExpertGauge gauge in this.Gauges)
            {
                if (gauge.Tag != null)
                    gauge.Text = WrapGaugeText((string) gauge.Tag, gauge.Width);
            }
        }

        /// <summary>
        /// Получение текста надписи для индикатора в зависимости от того, 
        /// значение из какой ячейки таблицы он отображает
        /// </summary>
        /// <param name="col">столбец</param>
        /// <param name="row">строка</param>
        /// <param name="gaugeWidth">ширина индикатора</param>
        /// <returns></returns>
        private string GetTextForGauge(DataColumn col, DataRow row)
        {
            string gaugeText = String.Empty;

            if (IsSeriesExists())
            {
                if (row[0] != DBNull.Value)
                {
                    string separator = (!String.IsNullOrEmpty(row[0].ToString()) && !String.IsNullOrEmpty(col.Caption)) ? GetSeparatorStr(columnsSeparator) : String.Empty;
                    gaugeText = String.Format("{0}{1}{2}", row[0], separator, col.Caption);
                }
            }
            else
            {
                gaugeText = col.Caption;
            }

            if (!String.IsNullOrEmpty(gaugeText))
            {
                gaugeText = gaugeText.Replace(GetSeparatorStr(columnsSeparator),
                                              GetSeparatorStr(DataSeriesSeparator.eNewLine));
                gaugeText = gaugeText.Replace(GetSeparatorStr(rowsSeparator),
                                              GetSeparatorStr(DataSeriesSeparator.eNewLine));
            }

            return gaugeText;
        }

        private string WrapGaugeText(string text, int width)
        {
            string[] subStrs = text.Split('\n');
            string result = String.Empty;
            foreach (string str in subStrs)
            {
                result += String.IsNullOrEmpty(result) ? GetWrapString(str, width) : "\n" + GetWrapString(str, width);
            }
            return result;
        }

        /// <summary>
        /// Вставляет в строку переносы когда она достигает ширины индикатора
        /// </summary>
        /// <param name="text"></param>
        /// <param name="controlWidth"></param>
        /// <returns></returns>
        private string GetWrapString(string text, int controlWidth)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            Graphics graphics = this.MainGauge.CreateGraphics();
            Font font = this.TextFont;
            string result = String.Empty;
            string subStr = String.Empty;

            for (int i = 0; i < text.Length; i++)
            {   
                if ((int)graphics.MeasureString(subStr + text[i], font).Width < controlWidth)
                {
                    subStr += text[i];
                }
                else
                {
                    result += String.IsNullOrEmpty(result) ? subStr : "\n" + subStr;
                    subStr = text[i].ToString();
                }
            }
            result += String.IsNullOrEmpty(result) ? subStr : "\n" + subStr;

            return result;
        }



        /// <summary>
        /// Именно в этом методе происходит инициализация элемента отчета по CellSet-у 
        /// </summary>
        /// <param name="cls"></param>
        public override void InitialByCellSet(CellSet cls)
        {
            base.InitialByCellSet(cls);
            DataTable dt = new DataTable();
            this.PopulateDataTableFromCellset(cls, ref dt);
            this.SourceDT = dt;
        }


        void GaugesLocation_Changed(bool isColumnsChanged)
        {
            PutGauges(isColumnsChanged);
        }


        private void SetGaugeType(GaugeType value)
        {
            switch(value)
            {
                case GaugeType.Standart:
                    this.Gauges.Clear();
                    this.PutGauges(true);
                    break;
                case GaugeType.Multiple:
                    this.PutGauges(true);
                    break;
            }
        }

        /// <summary>
        /// Разместить индикаторы в матричном виде
        /// </summary>
        /// <param name="putByColumns">размещать по количеству колонок</param>
        private void PutGauges(bool putByColumns)
        {
            int col = 0;
            int row = 0;
            this.gridBagPanel.Controls.Clear();
            this.ElementPlace.SuspendLayout();

            //if (this.GaugeType == GaugeType.Multiple)
            {
                this.GaugesLocation.Refresh();
                int gaugeWidth = this.Width/this.GaugesLocation.Columns;
                int gaugeHeight = this.Height/this.GaugesLocation.Rows;
                int gaugesCount = Math.Min(this.GaugesLocation.MaxCount, this.Gauges.Count);

                for (int i = 0; i < gaugesCount; i++)
                {
                    UltraGauge gauge = this.Gauges[i];
                    PutGauge(gauge, col, row);

                    this.gridBagPanel.SetPreferredSize(gauge, new System.Drawing.Size(gaugeWidth, gaugeHeight));

                    if (putByColumns)
                    {
                        col++;
                        if (col >= this.GaugesLocation.Columns)
                        {
                            col = 0;
                            row++;
                        }
                    }
                    else
                    {
                        row++;
                        if (row >= this.GaugesLocation.Rows)
                        {
                            row = 0;
                            col++;
                        }
                    }
                }
            }

            this.ElementPlace.ResumeLayout();

            /*else
            {
                PutGauge(this.MainGauge, 0, 0);
                this.gridBagPanel.SetPreferredSize(this.MainGauge, new System.Drawing.Size(this.Width, this.Height));

            }*/

        }

        /// <summary>
        /// Разместить индикатор на панели в указанных столбце и строке
        /// </summary>
        /// <param name="gauge">индикатор</param>
        /// <param name="column">столбец</param>
        /// <param name="row">строка</param>
        private void PutGauge(UltraGauge gauge, int column, int row)
        {
            this.gridBagPanel.Controls.Add(gauge);
            var constraint = new GridBagConstraint
                                 {
                                     Fill = Infragistics.Win.Layout.FillType.Both,
                                     OriginX = column,
                                     OriginY = row
                                 };

            this.gridBagPanel.SetGridBagConstraint(gauge, constraint);
        }


        public void InitLegend()
        {
            this.Legend.Items.Clear();
            foreach (GaugeColorRange range in this.ColorRanges)
            {
                if (range.Color != null)
                {
                    this.Legend.Items.Add(range.Color, range.Text, range.StartValue, range.EndValue);
                }
            }
        }

        /// <summary>
        /// Проверка корректности размера интервала на шкале индикатора
        /// </summary>
        private void CheckTickMarkInterval()
        {
            if (!this.AutoTickmarkCalculation)
            {
                if (((this.EndValue - this.StartValue) / this.TickmarkInterval) > 100)
                {
                    this.AutoTickmarkCalculation = true;
                    MessageBox.Show("Текущее значение размера интервалов некорректно. Расчет интервалов переведен в автоматический режим.",
                        "MDX Эксперт", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }


        /// <summary>
        /// Получение первой настрой индикатора из списка
        /// </summary>
        /// <returns></returns>
        private string GetFirstPresetName()
        {
            if (Directory.Exists(Application.StartupPath + "\\GaugePresets"))
            {
                string[] files = Directory.GetFiles(Application.StartupPath + "\\GaugePresets\\", "*.xml");
                if (files.Length > 0)
                    return System.IO.Path.GetFileNameWithoutExtension(files[0]);
            }
            return String.Empty;
        }

        void gauge_MouseClick(object sender, MouseEventArgs e)
        {
            this.MainForm.RefreshUserInterface(this);
        }

        /// <summary>
        /// Получение начального значения шкалы индикатора
        /// </summary>
        /// <returns></returns>
        private double GetStartValue()
        {
            return this.MainGauge.StartValue; //(this.Gauges.Count > 0) ? this.Gauges[0].StartValue : 0;
        }

        /// <summary>
        /// Получение конечного значения шкалы индикатора
        /// </summary>
        /// <returns></returns>
        private double GetEndValue()
        {
            return this.MainGauge.EndValue; //(this.Gauges.Count > 0) ? this.Gauges[0].EndValue : 0;
        }

        /// <summary>
        /// Получение текущего значения идикатора
        /// </summary>
        /// <returns></returns>
        private double GetValue()
        {
            return this.MainGauge.Value;
        }

        /// <summary>
        /// Установка начального значения для шкалы индикатора
        /// </summary>
        /// <param name="value"></param>
        private void SetStartValue(double value)
        {
            this.MainGauge.StartValue = value;
            foreach(ExpertGauge gauge in this.Gauges)
            {
                if (gauge.AutoTickmarkCalculation != this.MainGauge.AutoTickmarkCalculation)
                {
                    BindData();
                    return;
                }
                gauge.StartValue = this.MainGauge.StartValue;
            }
            this.InitLegend();

        }

        /// <summary>
        /// Установка начального, конечного и текущего значений
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="value"></param>
        public void SetValues(double startValue, double endValue, double value)
        {
            for (int i = 0; i < this.Gauges.Count; i++)
            {
                this.Gauges[i].SetValues(startValue, endValue, value);
            }
            this.InitLegend();

        }

        private double[] sqr = new double[] { 1.414214, 3.162278, 7.071068 };
        private double[] vint = new double[] { 1.0, 2.0, 5.0, 10.0 };
        /// <summary>
        /// Вычисление значения для интервала
        /// </summary>
        /// <param name="xmin"></param>
        /// <param name="xmax"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public double CalcTickMarkInterval(ref double xmin, ref double xmax, int n)
        {
            if (n <= 0)
            {
                return 10;
            }

            if (xmin == xmax)
                return 10;

            double intervalCnt = n;
            double precision = 2E-05;
            double d = Math.Abs((double)(xmax - xmin)) / intervalCnt;
            int digitsCnt = (int)Math.Log10(d);
            if (d < 1.0)
            {
                digitsCnt--;
            }
            double num6 = d / Math.Pow(10.0, (double)digitsCnt);
            int index = 0;
            while ((index < 3) && (num6 >= sqr[index]))
            {
                index++;
            }
            double dataInterval = vint[index] * Math.Pow(10.0, (double)digitsCnt);
            double num9 = xmin / dataInterval;
            long num10 = (long)num9;
            if (num9 < 0.0)
            {
                num10 -= 1L;
            }
            if (Math.Abs((double)((num10 + 1.0) - num9)) < precision)
            {
                num10 += 1L;
            }
            double min = dataInterval * num10;
            double num13 = xmax / dataInterval;
            long num14 = (long)(num13 + 1.0);
            if (num13 < -1.0)
            {
                num14 -= 1L;
            }

            if (Math.Abs((double)((num13 + 1.0) - num14)) < precision)
            {
                num14 -= 1L;
            }
            double max = dataInterval * num14;
            if (xmin > min)
            {
                xmin = min;
            }
            if (xmax < max)
            {
                xmax = max;
            }
            while ((xmin + (dataInterval * n)) < xmax)
            {
                dataInterval *= 2.0;
                n = n / 2 + 1;
            }

            xmax = xmin + n * dataInterval;

            /*
            max = xmin + dataInterval;
            while (xmax - max) > precision)
                max += dataInterval;

            xmax = max;
            */

            return dataInterval;
        }



        /// <summary>
        /// Установка конечного значения для шкалы индикатора
        /// </summary>
        /// <param name="value"></param>
        private void SetEndValue(double value)
        {
            //double tickMarkInterval = (Math.Abs((value - this.StartValue)/10));
            this.MainGauge.EndValue = value;
            foreach (ExpertGauge gauge in this.Gauges)
            {
                if (gauge.AutoTickmarkCalculation != this.MainGauge.AutoTickmarkCalculation)
                {
                    BindData();
                    return;
                }
                gauge.EndValue = this.MainGauge.EndValue;
            }
            this.InitLegend();

        }


        /// <summary>
        /// Установка интервала между отметками
        /// </summary>
        /// <param name="value"></param>
        private void SetTickmarkInterval(double value)
        {
            this.MainGauge.TickmarkInterval = value;
            foreach (ExpertGauge gauge in this.Gauges)
            {
                if (gauge.AutoTickmarkCalculation != this.MainGauge.AutoTickmarkCalculation)
                {
                    BindData();
                    return;
                }

                gauge.TickmarkInterval = this.MainGauge.TickmarkInterval;
            }
            this.InitLegend();

        }


        /// <summary>
        /// Получить интервал между отметками
        /// </summary>
        /// <param name="value"></param>
        private double GetTickmarkInterval()
        {
            return this.MainGauge.TickmarkInterval;//(this.Gauges.Count > 0) ? this.Gauges[0].TickmarkInterval : 10;
        }


        /// <summary>
        /// Установка интервала между отметками
        /// </summary>
        /// <param name="value"></param>
        private void SetAutoTickmarkCalculation(bool value)
        {
            this.MainGauge.AutoTickmarkCalculation = value;
            if (value)
            {
                this.BindData();
            }
            else
            {
                foreach (ExpertGauge gauge in this.Gauges)
                {
                    gauge.AutoTickmarkCalculation = value;
                    gauge.TickmarkInterval = this.TickmarkInterval;
                }
                this.InitLegend();

            }
        }


        /// <summary>
        /// Получить интервал между отметками
        /// </summary>
        /// <param name="value"></param>
        private bool GetAutoTickmarkCalculation()
        {
            return this.MainGauge.AutoTickmarkCalculation;//(this.Gauges.Count > 0) ? this.Gauges[0].TickmarkInterval : 10;
        }




        /// <summary>
        /// Установка текущего значения индикатора
        /// </summary>
        /// <param name="value"></param>
        private void SetValue(double value)
        {
            this.MainGauge.Value = value;
        }

        /// <summary>
        /// Обновим значение в семисегментном индикаторе
        /// </summary>
        public void RefreshDigitalGauge()
        {
            this.MainGauge.RefreshDigitalGauge();
            foreach (ExpertGauge gauge in this.Gauges)
            {
                gauge.RefreshDigitalGauge();
            }
        }

        private string GetText()
        {
            return this.MainGauge.Text; 
        }

        private void SetText(string value)
        {
            this.MainGauge.Text = value; 
        }


        private Font GetTextFont()
        {
            return this.MainGauge.TextFont; //(this.Gauges.Count > 0) ? this.Gauges[0].TextFont : null;
        }

        private void SetTextFont(Font value)
        {
            if (value != null)
            {
                this.MainGauge.TextFont = value;
                foreach(ExpertGauge gauge in this.Gauges)
                {
                    gauge.TextFont = value;
                }
            }
        }



        public override System.Xml.XmlNode Save()
        {
            XmlNode result = base.Save();

            XmlHelper.SetAttribute(result, Consts.templateName, this.PresetName);

            this.SavePreset(XmlHelper.AddChildNode(result, Common.Consts.presets));

            XmlHelper.SetAttribute(result, Common.Consts.isUpdatable, this.IsUpdatable.ToString());

            XmlHelper.AddChildNode(result, Consts.synchronization,
                new string[2] { Consts.boundTo, this.Synchronization.BoundTo });
            this.SaveSourceDT(XmlHelper.AddChildNode(result, Common.Consts.sourceDT));

            XmlHelper.SetAttribute(result, Consts.autoTickmarkCalculation, this.AutoTickmarkCalculation.ToString());

            this.GaugesLocation.Save(XmlHelper.AddChildNode(result, Common.Consts.gaugesLocation));

            this.ColorRanges.Save(XmlHelper.AddChildNode(result, Consts.colorRanges));
            this.Legend.Save(XmlHelper.AddChildNode(result, Common.Consts.legend));

            return result;
        }

        private void RefreshAnnotations()
        {
            BoxAnnotation mainAnnotation = (BoxAnnotation) this.MainGauge.Annotations[0];
            foreach(ExpertGauge gauge in this.Gauges)
            {
                BoxAnnotation annotation = (BoxAnnotation) gauge.Annotations[0];
                annotation.Bounds = mainAnnotation.Bounds;
                annotation.Label.Font = mainAnnotation.Label.Font;
                ((SolidFillBrushElement)annotation.Label.BrushElement).Color =
                    ((SolidFillBrushElement)mainAnnotation.Label.BrushElement).Color;
                
                //annotation.BoundsMeasure = mainAnnotation.BoundsMeasure;
                
            }
        }

        public override void Load(System.Xml.XmlNode reportElement, bool isForceDataUpdate)
        {
            base.Load(reportElement, isForceDataUpdate);

            if (reportElement == null)
                return;
            this.ElementPlace.Visible = false;
            this.ElementPlace.SuspendLayout();
            try
            {
                this.MainGauge.IsLoading = true;

                //            this._presetName = XmlHelper.GetStringAttrValue(reportElement, Consts.templateName, string.Empty);
                this.PivotData.Load(reportElement.SelectSingleNode(Common.Consts.pivotData), true);

                this.LoadPreset(reportElement.SelectSingleNode(Common.Consts.presets));
                Margin margin = this.Margins;
                Rectangle annotationBounds = new Rectangle();
                if (this.MainGauge.Annotations.Count > 0)
                {
                    annotationBounds = ((BoxAnnotation) this.MainGauge.Annotations[0]).Bounds;
                }

                XmlNode colorRangesNode = reportElement.SelectSingleNode(Common.Consts.colorRanges);
                if (colorRangesNode != null)
                {
                    this.ColorRanges.Load(colorRangesNode);
                }
                else
                {
                    this.ColorRanges.InitByVisibleRanges(this.VisibleColorRanges);
                }


                this.PresetName = XmlHelper.GetStringAttrValue(reportElement, Consts.templateName, string.Empty);
                
                this.Margins = margin;
                if (this.MainGauge.Annotations.Count > 0)
                {
                    ((BoxAnnotation)this.MainGauge.Annotations[0]).Bounds = annotationBounds;
                }

                this.IsUpdatable = XmlHelper.GetBoolAttrValue(reportElement, Common.Consts.isUpdatable, true);
                this.LoadSourceDT(reportElement.SelectSingleNode(Common.Consts.sourceDT));

                XmlNode syncNode = reportElement.SelectSingleNode(Consts.synchronization);
                if (syncNode != null)
                {
                    this.Synchronization.BoundTo = XmlHelper.GetStringAttrValue(syncNode, Consts.boundTo, "");
                }

                this.AutoTickmarkCalculation = XmlHelper.GetBoolAttrValue(reportElement, Consts.autoTickmarkCalculation,
                                                                          true);

                //this.PivotData.Load(reportElement.SelectSingleNode(Common.Consts.pivotData), true);

                this.GaugesLocation.Load(reportElement.SelectSingleNode(Common.Consts.gaugesLocation));

                this.BindData();
                //RefreshAnnotations();


                this.Legend.Load(reportElement.SelectSingleNode(Common.Consts.legend));

                this.RefreshColorRanges();
                this.InitLegend();
            }
            finally
            {
                this.MainGauge.IsLoading = false;
                this.ElementPlace.ResumeLayout();
                this.ElementPlace.Visible = true;
                this.ElementPlace.Invalidate();


            }

        }

        private void SaveSourceDT(XmlNode sourceDTNode)
        {
            if ((this.SourceDT != null) && !this.IsUpdatable)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    this.SourceDT.TableName = "Таблица";
                    this.SourceDT.WriteXml(stream, XmlWriteMode.WriteSchema);
                    stream.Flush();

                    stream.Position = 0;
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        XmlHelper.AppendCDataSection(sourceDTNode, streamReader.ReadToEnd());
                    }
                }
            }
            else
            {
                XmlHelper.AppendCDataSection(sourceDTNode, string.Empty);
            }
        }

        private void LoadSourceDT(XmlNode sourceDTNode)
        {
            if ((sourceDTNode == null) || (sourceDTNode.FirstChild.Value == string.Empty))
                return;

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter streamWriter = new StreamWriter(stream))
                {
                    streamWriter.Write(sourceDTNode.FirstChild.Value);
                    streamWriter.Flush();

                    stream.Position = 0;
                    DataTable dataTable = new DataTable();
                    dataTable.ReadXml(stream);
                    this.SourceDT = dataTable;
                }
            }
        }


        private void SavePreset(XmlNode presetNode)
        {
            using (StringWriter strWriter = new StringWriter())
            {
                if (this.MainGauge != null)
                {
                    this.MainGauge.SavePreset(strWriter, "UltraGaugePreset", String.Empty, PresetType.All);
                    XmlHelper.AppendCDataSection(presetNode, strWriter.ToString());
                }
            }
        }

        private void LoadPreset(XmlNode presetNode)
        {
            if (presetNode == null)
                return;

            string reportElementPreset = presetNode.FirstChild.Value;

            if (reportElementPreset != string.Empty)
            {
                using (StringReader stringReader = new StringReader(reportElementPreset))
                {
                    this.MainGauge.LoadPreset(stringReader, true);
                    /*
                    foreach (UltraGauge gauge in this.Gauges)
                    {
                        gauge.LoadPreset(stringReader, true);
                    }*/
                }
            }
        }



        protected override void RefreshData()
        {
            if (this.IsUpdatable)
            {
                base.RefreshData();
            }
        }

        /// <summary>
        /// Построение диаграммы по MDX-запросу
        /// </summary>
        protected override CellSet SetMDXQuery(string mdxQuery)
        {
            CellSet cls = null;

            if (!IsUpdatable)
            {
                BindData();
                return cls;
            }

            try
            {
                cls = base.SetMDXQuery(mdxQuery);
                this.InitialByCellSet(cls);
            }
            catch (Exception e)
            {
                this.InitialByCellSet();
                Common.CommonUtils.ProcessException(e);
            }
            return cls;
        }


        public override void SetElementVisible(bool value)
        {
            if (this.gridBagPanel.Visible != value)
            {
                this.gridBagPanel.Visible = value;
                
                Application.DoEvents();
            }
        }


        /// <summary>
        /// Получить изображение для печати, если у диаграммы стоит признак растягивать, 
        /// значит уместим изображение в указаные границы, если его не стоит, значит получаем полное 
        /// изображение элемента (вероятно оно будет напечатано на нескольиких страницах)
        /// </summary>
        /// <param name="imageBounds"></param>
        /// <returns></returns>
        public override Bitmap GetPrintableImage(Rectangle pageBounds)
        {
            return (this.gridBagPanel.Dock == DockStyle.Fill) ? this.GetBitmap(pageBounds) : this.GetBitmap();
        }

        /// <summary>
        /// Получить полное изображение элемента
        /// </summary>
        /// <returns></returns>
        public override Bitmap GetBitmap()
        {
            Rectangle fullElementBounds = this.ClientRectangle;
            /*fullElementBounds.Width -= this.ElementPlace.Width;
            fullElementBounds.Height -= this.ElementPlace.Height;

            fullElementBounds.Width += this.Gauges[0].Size.Width;
            fullElementBounds.Height += this.Gauges[0].Size.Height;*/

            fullElementBounds.Width = Math.Max(this.ClientRectangle.Width, fullElementBounds.Width);
            fullElementBounds.Height = Math.Max(this.ClientRectangle.Height, fullElementBounds.Height);
            return base.GetBitmap(fullElementBounds);
        }

        public override string ToString()
        {
            return string.Empty;
        }

        /// <summary>
        /// Т.к. этот элемент отчета не содержит данного интерфейса, возвращаем null
        /// </summary>
        public override IGridUserInterface GridUserInterface
        {
            get { return null; }
        }



        public override bool IsShowErrorMessage
        {
            get { return (base.IsCubeNotFond && this.IsUpdatable); }
        }

        public ExpertLegend Legend
        {
            get { return _legend; }
            set { _legend = value; }
        }

        public EventHandler RangeCollectionChanged
        {
            get { return _rangeCollectionChanged; }
            set { _rangeCollectionChanged = value; }
        }


        private void LoadPreset(string fileName)
        {
            foreach (ExpertGauge gauge in this.Gauges)
            {
                gauge.LoadPreset(fileName);
            }
        }

        private GaugeColorRangeCollection GetColorRanges()
        {
            return this.MainGauge.ColorRanges; 
        }

        private void SetColorRanges(GaugeColorRangeCollection value)
        {
            this.MainGauge.ColorRanges = value;
            foreach (ExpertGauge gauge in this.Gauges)
            {
                gauge.ColorRanges = value;
            }
            InitLegend();
        }

        /// <summary>
        /// Обновление отображения цветовых индикаторов в индикаторе
        /// </summary>
        public void RefreshColorRanges()
        {
            SetColorRanges(this.ColorRanges);
        }

        private List<GaugeRange> GetVisibleColorRanges()
        {
            return this.MainGauge.VisibleColorRanges;
        }

        private void SetVisibleColorRanges(List<GaugeRange> value)
        {
            this.MainGauge.VisibleColorRanges = value;
            foreach (ExpertGauge gauge in this.Gauges)
            {
                gauge.VisibleColorRanges = value;
            }
            InitLegend();
        }




        /// <summary>
        /// Есть серии?
        /// </summary>
        private bool SeriesExist(CellSet cls)
        {
            if (cls != null)
            {
                return (cls.OlapInfo.AxesInfo.Axes.Count > 1);
            }
            else
            {
                return false;
            }
        }

        #region Вспомогательные классы

        private class DimensInfo
        {
            private List<DimInfo> _dimensionsInfo;
            private PivotData pivotData;

            public DimensInfo(PivotData pData)
            {
                this.pivotData = pData;
                this.DimensionsInfo = new List<DimInfo>();
            }

            public PivotData PivotData
            {
                get { return this.pivotData; }
            }

            public void AddInfo(Position pos)
            {
                if (pos == null)
                    return;

                foreach (Member mem in pos.Members)
                {
                    string dimUN = mem.ParentLevel.ParentHierarchy.UniqueName;
                    int levelNumber = mem.ParentLevel.LevelNumber;
                    bool isAllMember = IsAllMember(mem);

                    PivotTotal total = this.PivotData.TotalAxis.GetTotalByName(mem.UniqueName);
                    //string caption = (total != null) ? total.Caption : mem.Caption;
                    string caption = (total != null) ? total.Caption : CommonUtils.GetMemberCaptionWithoutID(mem);


                    this.SetLastMemberCaption(dimUN, levelNumber, caption, isAllMember);
                }
            }

            private bool IsAllMember(Member mbr)
            {
                return (mbr.ParentLevel.Name == "(All)");
            }

            private void SetLastMemberCaption(string dimUN, int levelNumber, string memberCaption, bool isAllMember)
            {
                foreach (DimInfo dimInfo in this.DimensionsInfo)
                {
                    if (dimInfo.UniqueName == dimUN)
                    {
                        dimInfo.SetLastMemberCaption(levelNumber, memberCaption, isAllMember);
                        return;
                    }
                }
                //Если нет информации о данном измерениии, добавим его
                DimInfo newDimInfo = new DimInfo(dimUN);
                newDimInfo.SetLastMemberCaption(levelNumber, memberCaption, isAllMember);
                this.DimensionsInfo.Add(newDimInfo);
            }

            /// <summary>
            /// Вернет имена всех родительских элементов
            /// </summary>
            /// <param name="mem"></param>
            /// <param name="separator"></param>
            /// <returns></returns>
            public string GetAllLastMemberCaption(Member mem, string separator)
            {
                string result = string.Empty;
                string[] captionArray = this.GetAllLastMemberCaption(mem.ParentLevel.ParentHierarchy.UniqueName,
                    mem.ParentLevel.LevelNumber);

                for (int i = 0; i < captionArray.Length; i++)
                {
                    if (i != 0)
                        result += separator;
                    result += captionArray[i];
                }
                return result;
            }

            public string[] GetAllLastMemberCaption(Member mem)
            {
                return this.GetAllLastMemberCaption(mem.ParentLevel.ParentHierarchy.UniqueName,
                    mem.ParentLevel.LevelNumber);
            }

            private string[] GetAllLastMemberCaption(string dimUN, int levelNumber)
            {
                foreach (DimInfo dimInfo in this.DimensionsInfo)
                {
                    if (dimInfo.UniqueName == dimUN)
                    {
                        return dimInfo.GetAllLastMemberCaption(levelNumber);
                    }
                }
                return new string[0];
            }

            public List<DimInfo> DimensionsInfo
            {
                get { return _dimensionsInfo; }
                set { _dimensionsInfo = value; }
            }
        }

        private class DimInfo
        {
            private string _uniqueName;
            private List<LevInfo> _levelsInfo;

            public DimInfo(string uniqueName)
            {
                this.UniqueName = uniqueName;
                this.LevelsInfo = new List<LevInfo>();
            }

            public void SetLastMemberCaption(int levelNumber, string memberCaption, bool isAllMember)
            {
                foreach (LevInfo levInfo in this.LevelsInfo)
                {
                    if (levInfo.LevelNumber == levelNumber)
                    {
                        levInfo.LastMemberCaption = memberCaption;
                        levInfo.IsAllLevel = isAllMember;
                        return;
                    }
                }
                //Если не нашли уровень с таким номером, создадим его
                this.LevelsInfo.Add(new LevInfo(levelNumber, memberCaption, isAllMember));
                //После добавления информации об новом уровне, упорядочим их по возростанию
                this.OrderLevels();
            }

            private void OrderLevels()
            {
                //сортируем уровни по возростанию
                for (int i = 1; i < this.LevelsInfo.Count; i++)
                {
                    int levelNumber = this.LevelsInfo[i].LevelNumber;
                    int k = i - 1;
                    while ((k >= 0) && (levelNumber < this.LevelsInfo[k].LevelNumber))
                    {
                        k--;
                    }
                    k++;
                    if ((k >= 0) && (k != i))
                    {
                        this.LevelsInfo.Insert(k, this.LevelsInfo[i]);
                        this.LevelsInfo.RemoveAt(i + 1);
                    }
                }
            }

            public string GetLastMemberCaption(int levelNumber)
            {
                foreach (LevInfo levInfo in this.LevelsInfo)
                {
                    if (levInfo.LevelNumber == levelNumber)
                    {
                        return levInfo.LastMemberCaption;
                    }
                }
                return string.Empty;
            }

            public string[] GetAllLastMemberCaption(int levelNumber)
            {
                List<string> result = new List<string>();

                foreach (LevInfo levInfo in this.LevelsInfo)
                {
                    if (levInfo.LevelNumber <= levelNumber)
                    {
                        if ((!levInfo.IsAllLevel) || (levelNumber == 0))
                            result.Add(levInfo.LastMemberCaption);
                    }
                }
                return result.ToArray();
            }

            public List<LevInfo> LevelsInfo
            {
                get { return _levelsInfo; }
                set { _levelsInfo = value; }
            }

            public string UniqueName
            {
                get { return _uniqueName; }
                set { _uniqueName = value; }
            }
        }

        private class LevInfo
        {
            private int _levelNumber;
            private string _lastMemberCaption;
            private bool _isAllLevel;

            public LevInfo(int levelNumber, string lastMemberCaption, bool isAllLevel)
            {
                this.LevelNumber = levelNumber;
                this.LastMemberCaption = lastMemberCaption;
                this.IsAllLevel = isAllLevel;
            }

            public int LevelNumber
            {
                get { return _levelNumber; }
                set { _levelNumber = value; }
            }

            public string LastMemberCaption
            {
                get { return _lastMemberCaption; }
                set { _lastMemberCaption = value; }
            }

            public bool IsAllLevel
            {
                get { return _isAllLevel; }
                set { _isAllLevel = value; }
            }
        }

        #endregion


        /// <summary>
        /// Перекачивает данные из селсета в стандартный датасет,
        /// который понимает диаграмма
        /// </summary>
        /// <param name="cls">Селлсет с данными запроса (входной формат)</param>
        /// <param name="dt">Наполняемый датасет (выходной формат)</param>
        private void PopulateDataTableFromCellset(CellSet cls, ref DataTable dt)
        {
            if (cls != null)
            {
                if (!this.PivotData.CheckConnection())
                    return;

                //Если есть серии, создаем столбец для их названий
                if (SeriesExist(cls))
                {
                    DataColumn dataColumn = dt.Columns.Add();
                    dataColumn.DataType = System.Type.GetType("System.String");
                    dataColumn.Caption = seriesColumnName; 
                    dataColumn.ColumnName = seriesColumnName;
                }
                List<Member> collapsedMembers = GetSyncTableCollapsedMembers();

                PopulateColumnsFromCellset(cls, dt, collapsedMembers);
                PopulateSeriesFromCellset(cls, dt, collapsedMembers);
                PopulateValuesFromCellset(cls, dt);

                //dt = CheckElemOrder(cls, dt);
            }
        }

        /// <summary>
        /// Получение схлопнутых элементов таблицы, по которой строится диаграмма
        /// </summary>
        /// <returns></returns>
        private List<Member> GetSyncTableCollapsedMembers()
        {
            List<Member> collapsedMembers = new List<Member>();
            if (this.Synchronization == null)
                return collapsedMembers;

            string boundTo = this.Synchronization.BoundTo;
            TableReportElement tableElement = this.MainForm.FindTableReportElement(boundTo);
            ExpertGrid expertGrid = null;
            if (tableElement != null)
            {
                expertGrid = tableElement.ExpertGrid;
                collapsedMembers = expertGrid.Row.AllCollapsedMembers();
                collapsedMembers.AddRange(expertGrid.Column.AllCollapsedMembers());
            }
            return collapsedMembers;
        }

        /// <summary>
        /// Перекачивает названия серий из селсета в датасет
        /// </summary>
        private void PopulateSeriesFromCellset(CellSet cls, DataTable dt, List<Member> collapsedMembers)
        {
            if (SeriesExist(cls))
            {
                string seriesName;
                int counter;
                int positionCount = cls.Axes[1].Positions.Count;
                DimensInfo dimensionsInfo = new DimensInfo(this.PivotData);
                //вычислим номера последних уровней
                int[] lastLevelNumbers = this.GetLastLevelNumbers(cls.Axes[1].Positions);
                //найдем UN всех листовых элементов
                List<string> leafMemberUN = this.GetLeafMemberUN(cls.Axes[1].Positions, lastLevelNumbers);
                this.rowPosNum.Clear();

                //добавим схлопнутые элементы к листовым
                foreach (Member mbr in collapsedMembers)
                {
                    leafMemberUN.Add(mbr.UniqueName);
                }

                for (int i = 0; i < cls.Axes[1].Positions.Count; i++)
                {
                    Position pos = cls.Axes[1].Positions[i];
                    counter = 0;
                    seriesName = string.Empty;
                    dimensionsInfo.AddInfo(pos);
                    
                    bool isAppendPos = false;
                    bool isHideMember = false;

                    //Если это общий итог, то и название соответствующее
                    if ((pos.Ordinal == positionCount - 1) && this.IsExistsGrandTotal(AxisType.atRows))
                        seriesName = Common.Consts.grandTotalCaption;
                    else
                    {
                        if (this.IsAppendPosition(pos, leafMemberUN))
                        {
                            isAppendPos = true;

                            foreach (Member mem in pos.Members)
                            {
                                if (IsHideMember(mem, collapsedMembers) || IsExceptedMember(mem, PivotData.RowAxis))
                                {
                                    seriesName = string.Empty;
                                    isHideMember = true;
                                    break;
                                }

                                if (UseDimInLabels(PivotData.RowAxis, mem.ParentLevel.ParentHierarchy.UniqueName))
                                {
                                    if (seriesName != string.Empty)
                                    {
                                        seriesName += GetSeparatorStr(rowsSeparator);
                                    }

                                    if (this.IncludeInLableParentMember(AxisType.atRows))
                                        seriesName += dimensionsInfo.GetAllLastMemberCaption(mem, GetSeparatorStr(rowsSeparator));
                                    else
                                    {
                                        PivotTotal total = this.PivotData.TotalAxis.GetTotalByName(mem.UniqueName);
                                        //seriesName += (total != null) ? total.Caption : mem.Caption;
                                        seriesName += (total != null) ? total.Caption : CommonUtils.GetMemberCaptionWithoutID(mem);

                                    }
                                }
                                counter++;
                            }
                        }
                    }

                    if (((seriesName != string.Empty)||(isAppendPos))&&(!isHideMember))
                    {
                        this.rowPosNum.Add(pos.Ordinal);
                        dt.Rows.Add(seriesName);
                    }
                }


            }
        }


        /// <summary>
        /// В датасете диаграммы по селсету создает столбцы (категории)
        /// Данными не заполняет, только пустые столбцы
        /// </summary>
        private void PopulateColumnsFromCellset(CellSet cls, DataTable dt, List<Member> collapsedMembers)
        {
            DataColumn dataColumn;
            int counter;
            if (cls.OlapInfo.AxesInfo.Axes.Count > 0)
            {
                //будем помечать ячейки которые не при каком раскладе не должны отображаться 
                //в диаграмме
                const string hideCell = "hideCell";
                string columnName;
                int positionCount = cls.Axes[0].Positions.Count;
                DimensInfo dimensionsInfo = new DimensInfo(this.PivotData);
                //вычислим номера последних уровней
                int[] lastLevelNumbers = this.GetLastLevelNumbers(cls.Axes[0].Positions);
                //найдем UN всех листовых элементов
                List<string> leafMemberUN = this.GetLeafMemberUN(cls.Axes[0].Positions, lastLevelNumbers);
                this.columnPosNum.Clear();

                //пометим схлопнутые элементы как листовые
                foreach (Member mbr in collapsedMembers)
                {
                    leafMemberUN.Add(mbr.UniqueName);
                }


                for (int i = 0; i < cls.Axes[0].Positions.Count; i++)
                {
                    //if (dt.Columns.Count >= this.GaugesLocation.MaxCount)
                    //    return;

                    Position pos = cls.Axes[0].Positions[i];

                    counter = 0;
                    columnName = string.Empty;
                    dimensionsInfo.AddInfo(pos);

                    //Если это общий итог, то и название соответствующее
                    if ((pos.Ordinal > 0)&&(pos.Ordinal == positionCount - 1) && this.IsExistsGrandTotal(AxisType.atColumns))
                        columnName = Common.Consts.grandTotalCaption;
                    else
                    {
                        if (this.IsAppendPosition(pos, leafMemberUN))
                        {
                            foreach (Member mem in pos.Members)
                            {
                                if (IsHideMember(mem, collapsedMembers) || IsExceptedMember(mem, PivotData.ColumnAxis))
                                {
                                    columnName = hideCell;
                                    break;
                                }

                                if (UseDimInLabels(PivotData.ColumnAxis, mem.ParentLevel.ParentHierarchy.UniqueName))
                                {
                                    if (columnName != string.Empty)
                                    {
                                        columnName += GetSeparatorStr(columnsSeparator);
                                    }

                                    if (this.IncludeInLableParentMember(AxisType.atColumns))
                                        columnName += dimensionsInfo.GetAllLastMemberCaption(mem,
                                                                                             GetSeparatorStr(
                                                                                                 columnsSeparator));
                                    else
                                    {
                                        PivotTotal total = this.PivotData.TotalAxis.GetTotalByName(mem.UniqueName);
                                        //columnName += (total != null) ? total.Caption : mem.Caption;
                                        columnName += (total != null) ? total.Caption : CommonUtils.GetMemberCaptionWithoutID(mem);

                                    }
                                }
                                counter++;
                            }
                        }
                        else
                            columnName = hideCell;
                    }

                    if (columnName != hideCell)
                    {
                        this.columnPosNum.Add(pos.Ordinal);
                        dataColumn = dt.Columns.Add();
                        dataColumn.DataType = typeof(Decimal);
                        dataColumn.Caption = this.GetColumnName(dt.Columns, columnName);
                        if (dataColumn.Caption != string.Empty)
                            dataColumn.ColumnName = dataColumn.Caption;
                    }
                }
            }
        }

        /// <summary>
        /// Перекачивает значения из селсета в датасет диаграммы. Ранее в columnPosNum, rowPosNum 
        /// сохранили номера кортежей из которых будем брать данные
        /// </summary>
        private void PopulateValuesFromCellset(CellSet cls, DataTable dt)
        {
            object[] values;

            if (SeriesExist(cls))
            {
                for (int r = 0; r < this.rowPosNum.Count; r++)
                {
                    values = dt.Rows[r].ItemArray;
                    for (int c = 0; c < this.columnPosNum.Count; c++)
                    {
                        values[c + 1] = this.GetCellValue(cls.Cells, this.columnPosNum[c], this.rowPosNum[r]);
                        
                        //если вдруг попалось строковое значение - записываем вместо него null
                        if ((values[c + 1] != null)&&(values[c + 1].GetType() == typeof(string)))
                            values[c + 1] = null;
                    }
                    try
                    {
                        dt.Rows[r].ItemArray = values;
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                if (cls.Cells.Count > 0)
                {
                    values = new object[this.columnPosNum.Count];
                    for (int c = 0; c < values.Length; c++)
                    {
                        values[c] = this.GetCellValue(cls.Cells, this.columnPosNum[c]);
                    }

#warning Нет обработки. см камент ниже...
                    //При занесении строки в датасет может возникнуть исключение, 
                    //если в фильтрах была задействована мера у которой не числовые
                    //значения (есть вычислимые меры со строковыми значениями).
                    //Идет исключение из-за того, что в дата-тэйбле проставлен
                    //числовой тип. Сейчас просто затраено, что делать пока не понятно.                    
                    try
                    {
                        dt.Rows.Add(values);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private string GetColumnName(DataColumnCollection columns, string columnName)
        {
            string result = columnName;
            if (columns == null)
                return result;

            bool isNameExist = false;
            int i = 1;
            do
            {
                isNameExist = (columns.IndexOf(result) != -1);
                if (isNameExist)
                {
                    result = string.Format("{0} ({1})", columnName, i.ToString());
                    i++;
                }
            }
            while (isNameExist);
            return result;
        }

        private object GetCellValue(CellCollection cells, int index1, int index2)
        {
            try
            {
                return cells[index1, index2].Value;
            }
            catch
            {
                return string.Empty;
            }
        }

        private object GetCellValue(CellCollection cells, int index)
        {
            try
            {
                return cells[index].Value;
            }
            catch
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// Получаем номера последних включенных в выборку уровней  у каждого измерения в оси
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        private int[] GetLastLevelNumbers(PositionCollection pc)
        {
            if ((pc != null) && (pc.Count > 0))
            {
                //получаем количество измерений
                int[] result = new int[pc[0].Members.Count];
                foreach (Position pos in pc)
                {
                    for (int i = 0; i < pos.Members.Count; i++)
                    {
                        Member mem = pos.Members[i];
                        result[i] = Math.Max(result[i], mem.ParentLevel.LevelNumber);
                    }
                }
                return result;
            }
            return new int[0];
        }

        /// <summary>
        /// Получить UN листовых элементов
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        private List<string> GetLeafMemberUN(PositionCollection pc, int[] lastLevelNumbers)
        {
            List<string> result = new List<string>();

            if ((pc != null) && (pc.Count > 0))
            {
                foreach (Position pos in pc)
                {
                    for (int i = 0; i < pos.Members.Count; i++)
                    {
                        Member mem = pos.Members[i];
                        if ((mem.ChildCount == 0) || (mem.ParentLevel.LevelNumber == lastLevelNumbers[i]))
                            result.Add(mem.UniqueName);
                    }
                }
            }
            return result;
        }


        private bool IsExistsGrandTotal(AxisType axisType)
        {
            //Если в запросе строки распологаются в оси колонок, то существование 
            //главного итога будем смотреть у строк
            if (this.RowsGoesAsColumns())
                axisType = AxisType.atRows;

            switch (axisType)
            {
                case AxisType.atColumns: return this.PivotData.ColumnAxis.GrandTotalExists;
                case AxisType.atRows: return this.PivotData.RowAxis.GrandTotalExists;
            }
            return false;
        }

        /// <summary>
        /// Нужно ли строки выбирать как столбцы?
        /// </summary>
        private bool RowsGoesAsColumns()
        {
            ///1) Это может понадобиться только тогда, когда в запросе должна быть ось строк,
            /// а ось столбцов отсутствует. Ситуация для MDX не допустимая.
            /// Поэтому, что бы хоть что-то выводить эти самые строки будем запихивать в 
            /// отсутствующие столбцы.
            /// 2) Делать это будем только для диаграммы, поскольку для таблицы в этой ситуации
            /// создается фиктивная мера-пустышка. Поэтому в таблице ось столбцов есть всегда
            return (this.PivotData.ColumnAxis.FieldSets.Count == 0);
        }

        private bool IncludeInLableParentMember(AxisType axisType)
        {
            //Если в запросе строки распологаются в оси колонок, то существование 
            //главного итога будем смотреть у строк
            if (this.RowsGoesAsColumns())
                axisType = AxisType.atRows;

            switch (axisType)
            {
                case AxisType.atColumns: return this.PivotData.ColumnAxis.IncludeInChartLabelParentMember;
                case AxisType.atRows: return this.PivotData.RowAxis.IncludeInChartLabelParentMember;
            }
            return false;
        }


        private string GetSeparatorStr(DataSeriesSeparator sp)
        {
            switch (sp)
            {
                case DataSeriesSeparator.eComma: return ", ";
                case DataSeriesSeparator.eDotComma: return "; ";
                case DataSeriesSeparator.eNewLine: return "\n";
                case DataSeriesSeparator.eSpace: return " ";
                case DataSeriesSeparator.eStick: return " | ";
            }
            return "; ";
        }


        private bool UseDimInLabels(Data.Axis ax, string hierarchy)
        {
            try
            {
                FieldSet fs = null;
                if ((ax.FieldSets.Count == 0) && (ax.AxisType == AxisType.atColumns))
                {
                    fs = ax.ParentPivotData.RowAxis.FieldSets.GetFieldSetByName(hierarchy);
                }
                else
                {
                    fs = ax.FieldSets.GetFieldSetByName(hierarchy);
                }
                return (fs != null) ? fs.UsedInChartLabels : false;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Проверяем есть ли у мембера схлопнутые предки 
        /// </summary>
        /// <param name="member"></param>
        /// <param name="collapsedMembers"></param>
        /// <returns></returns>
        private bool IsHideMember(Member member, List<Member> collapsedMembers)
        {
            foreach (Member mbr in collapsedMembers)
            {
                if (IsAncestor(mbr, member))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Отключен ли элемент в редакторе элементов измерения
        /// </summary>
        /// <param name="member"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        private bool IsExceptedMember(Member member, Data.PivotAxis axis)
        {
            foreach (FieldSet fs in axis.FieldSets)
            {
                if (fs.ExceptedMembers.Contains(member.UniqueName))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Если первый элемент является предком второго, вернет true
        /// </summary>
        /// <param name="ancestor">предок</param>
        /// <param name="descendant">предпологаемый потомок</param>
        /// <returns></returns>
        private bool IsAncestor(Member ancestor, Member descendant)
        {
            if (ancestor.UniqueName == descendant.UniqueName)
                return false;

            while ((descendant.Parent != null) && (ancestor.LevelDepth <= descendant.Parent.LevelDepth))
            {
                if (ancestor.UniqueName == descendant.Parent.UniqueName)
                    return true;
                descendant = descendant.Parent;
            }
            return false;
        }

        /// <summary>
        /// Проверяет на соответствие видимости нелистовых элементов (итогов) в каждом измерении оси
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="leafMemberUN"></param>
        /// <returns></returns>
        private bool IsAppendPosition(Position pos, List<string> leafMemberUN)
        {
            foreach (Member mem in pos.Members)
            {
                FieldSet fieldSet = this.PivotData.GetFieldSet(mem.ParentLevel.ParentHierarchy.UniqueName);
                if ((fieldSet != null) && !fieldSet.IsVisibleTotals)
                {
                    //если надо показывать только листовые элементы, проверим содержиться ли данный 
                    //элемент в списке таковых
                    if (!leafMemberUN.Contains(mem.UniqueName))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Синхронизация по структуре данных
        /// </summary>
        /// <param name="pivotData">структура</param>
        /// <param name="refreshFieldList">обновлять список полей или нет</param>
        /// <param name="silentMode">true - если не хотим обновлять данные</param>
        public void Synchronize(PivotData pivotData, bool refreshFieldList, bool silentMode)
        {
            bool isDeferDataUpdating = this.PivotData.IsDeferDataUpdating;
            this.PivotData.IsDeferDataUpdating = true;

            this.PivotData.Clear();

            this.PivotData.CubeName = pivotData.CubeName;

            foreach (FieldSet fs in pivotData.FilterAxis.FieldSets)
                this.PivotData.FilterAxis.FieldSets.CopyFieldSet(fs, AxisType.atFilters, false);

            foreach (FieldSet fs in pivotData.ColumnAxis.FieldSets)
                this.PivotData.ColumnAxis.FieldSets.CopyFieldSet(fs, AxisType.atColumns, false);

            //this.PivotData.ColumnAxis.GrandTotalVisible = pivotData.ColumnAxis.GrandTotalVisible;

            foreach (FieldSet fs in pivotData.RowAxis.FieldSets)
                this.PivotData.RowAxis.FieldSets.CopyFieldSet(fs, AxisType.atRows, false);

            //this.PivotData.RowAxis.GrandTotalVisible = pivotData.RowAxis.GrandTotalVisible;

            if (pivotData.TotalAxis.FieldSets.Count > 0)
            {
                this.PivotData.ColumnAxis.FieldSets.CopyFieldSet(pivotData.TotalAxis.FieldSets[0], AxisType.atColumns, false);
            }

            foreach (PivotTotal total in pivotData.TotalAxis.Totals)
                this.PivotData.TotalAxis.CopyTotal(total);


            if (refreshFieldList)
            {
                this.MainForm.FieldListEditor.PivotData = this.PivotData;
                this.MainForm.FieldListEditor.InitEditor(this);
            }

            this.PivotData.IsDeferDataUpdating = isDeferDataUpdating;

            if (!silentMode)
                this.RefreshData();
        }


        public void Synchronize()
        {
            PivotData pivotData = null;
            TableReportElement tableElement = null;

            if (!String.IsNullOrEmpty(this.Synchronization.BoundTo))
            {
                tableElement = this.MainForm.FindTableReportElement(this.Synchronization.BoundTo);
                if (tableElement != null)
                {
                    pivotData = tableElement.PivotData;
                }
            }
            if (pivotData == null)
            {
                this.MainForm.UndoRedoManager.AddEvent(this, UndoRedoEventType.DataChange);
                return;
            }

            Synchronize(pivotData, true, false);
            if (tableElement != null)
                this.MainForm.UndoRedoManager.AddEvent(tableElement, UndoRedoEventType.DataChange);

        }

        /// <summary>
        /// Синхронизация с возможностью обхода отложенного обновления данных
        /// </summary>
        /// <param name="ignoreDeferDataUpdating"></param>
        public void Synchronize(bool forceDataUpdating)
        {
            if (forceDataUpdating)
            {
                bool isDeferDataUpdating = this.PivotData.IsDeferDataUpdating;
                this.PivotData.IsDeferDataUpdating = false;
                Synchronize();
                this.PivotData.IsDeferDataUpdating = isDeferDataUpdating;
            }
            else
            {
                Synchronize();
            }
        }


    }


    /// <summary>
    /// Синхронизация индикатора с таблицей
    /// </summary>
    public class MultipleGaugeSynchronization
    {
        private string boundTo;
        private MultipleGaugeReportElement gaugeElement;

        public string BoundTo
        {
            get { return boundTo; }
            set
            {
                SetBoundTo(value);
            }
        }

        public MultipleGaugeReportElement GaugeElement
        {
            get { return gaugeElement; }
            set { gaugeElement = value; }
        }

        public MultipleGaugeSynchronization(MultipleGaugeReportElement gaugeElement)
        {
            this.gaugeElement = gaugeElement;
        }

        private void SetBoundTo(string key)
        {
            if (key == this.BoundTo)
                return;

            //удаляем у таблицы ссылку на индикатор
            if (!String.IsNullOrEmpty(this.BoundTo))
            {
                TableReportElement tableElement = this.GaugeElement.MainForm.FindTableReportElement(this.BoundTo);
                if (tableElement != null)
                    tableElement.AnchoredElements.Remove(this.GaugeElement.UniqueName);
            }

            //добавляем для таблицы ссылку на индикатор
            if (!String.IsNullOrEmpty(key))
            {
                TableReportElement tableElement = this.GaugeElement.MainForm.FindTableReportElement(key);
                if (tableElement != null)
                {
                    tableElement.AnchoredElements.Add(this.GaugeElement.UniqueName);
                }
            }

            this.boundTo = key;
        }

        public override string ToString()
        {
            if (!String.IsNullOrEmpty(this.BoundTo))
            {
                return this.GaugeElement.MainForm.GetReportElementText(this.BoundTo);
            }
            else
            {
                return "";
            }
        }
    }

    /*
    /// <summary>
    /// Синхронизация индикатора с таблицей
    /// </summary>
    public class GaugeSynchronization
    {
        private string _boundTo;
        private MultipleGaugeReportElement _gaugeElement;
        private string _measure;
        private bool _isCurrentColumnValues;

        /// <summary>
        /// Здесь прописывается уникальный ключ таблицы, с которой синхронизируется элемент
        /// </summary>
        public string BoundTo
        {
            get { return _boundTo; }
            set
            {
                SetBoundTo(value);
            }
        }

        /// <summary>
        /// Уникальное имя меры, значения для которой отображаются на индикаторе
        /// </summary>
        public string Measure
        {
            get { return _measure; }
            set { _measure = value; }
        }

        /// <summary>
        /// При получении минимального и максимального значений показателя учитывать значения только текущего столбца таблицы
        /// </summary>
        public bool IsCurrentColumnValues
        {
            get { return this._isCurrentColumnValues; }
            set { this._isCurrentColumnValues = value; }
        }

        public MultipleGaugeReportElement GaugeElement
        {
            get { return _gaugeElement; }
            set { _gaugeElement = value; }
        }


        public GaugeSynchronization(MultipleGaugeReportElement gaugeElement)
        {
            this._gaugeElement = gaugeElement;
            this._isCurrentColumnValues = false;
        }

        private void SetBoundTo(string key)
        {
            if (key == this.BoundTo)
                return;

            //удаляем у таблицы ссылку на индикатор
            if (!String.IsNullOrEmpty(this.BoundTo))
            {
                TableReportElement tableElement = this.GaugeElement.MainForm.FindTableReportElement(this.BoundTo);
                if (tableElement != null)
                    tableElement.AnchoredElements.Remove(this.GaugeElement.UniqueName);
            }

            //добавляем для таблицы ссылку на индикатор
            if (!String.IsNullOrEmpty(key))
            {
                TableReportElement tableElement = this.GaugeElement.MainForm.FindTableReportElement(key);
                if (tableElement != null)
                {
                    tableElement.AnchoredElements.Add(this.GaugeElement.UniqueName);
                    tableElement.UpdateAnchoredGauge();
                }
            }

            this._boundTo = key;
        }

        public override string ToString()
        {
            if (!String.IsNullOrEmpty(this.BoundTo))
            {
                return this.GaugeElement.MainForm.GetReportElementText(this.BoundTo);
            }
            else
            {
                return "";
            }
        }
    }
    */


    /// <summary>
    /// Тип индикатора
    /// </summary>
    public enum GaugeType
    {
        [Description("Обычный")]
        Standart,
        [Description("Множественный")]
        Multiple
    }


}

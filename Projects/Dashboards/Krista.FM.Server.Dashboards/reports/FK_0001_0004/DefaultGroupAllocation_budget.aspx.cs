using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Band;
using Infragistics.Documents.Reports.Report.Flow;
using Infragistics.Documents.Reports.Report.Grid;
using Infragistics.Documents.Reports.Report.Index;
using Infragistics.Documents.Reports.Report.QuickList;
using Infragistics.Documents.Reports.Report.QuickTable;
using Infragistics.Documents.Reports.Report.QuickText;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Segment;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.TOC;
using Infragistics.Documents.Reports.Report.Tree;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color=System.Drawing.Color;
using Font=System.Drawing.Font;
using Image=Infragistics.Documents.Reports.Graphics.Image;
using Rectangle=System.Drawing.Rectangle;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0004
{
    public partial class DefaultGroupAllocation_budget : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtChart = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private int currentYear = 2009;
        private int currentMonth = 1;
        private string[] leftGroupSubjects = new string[12];
        private string[] rightGroupSubjects = new string[12];

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            LeftChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.43);
            LeftChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.53);
            RightChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.55);
            RightChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.53);

            TopMaxGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.43);
            TopMinGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.55);
            TopMaxGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.2);
            TopMinGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.2);
            TopMaxGrid.DataBound += new EventHandler(TopGrid_DataBound);
            TopMinGrid.DataBound += new EventHandler(TopGrid_DataBound);

            #region Настройка диаграммы

            LeftChart.ChartType = ChartType.BarChart;
            LeftChart.BarChart.SeriesSpacing = 0;

            LeftChart.Border.Thickness = 0;
            LeftChart.Axis.X.Visible = false;
            LeftChart.Axis.Y2.Visible = true;
            LeftChart.Axis.Y2.Labels.Visible = false;
            LeftChart.Axis.Y2.Labels.SeriesLabels.Visible = false;
            LeftChart.Axis.Y2.Extent = 0;
            LeftChart.Axis.Y.Visible = true;
            LeftChart.Axis.Y.Labels.Visible = false;
            LeftChart.Axis.Y.Labels.SeriesLabels.Visible = false;
            LeftChart.Axis.Y.LineThickness = 0;
            LeftChart.Axis.Y.Extent = 12;

            RightChart.ChartType = ChartType.BarChart;
            RightChart.BarChart.SeriesSpacing = 0;

            RightChart.Border.Thickness = 0;
            RightChart.Axis.X.Visible = false;
            RightChart.Axis.Y.Extent = 124;
            RightChart.Axis.Y.Labels.Visible = false;
            RightChart.Axis.Y.Labels.SeriesLabels.Visible = true;
            RightChart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            RightChart.Axis.Y.Labels.SeriesLabels.FontColor = Color.Black;
            RightChart.Axis.Y.Labels.SeriesLabels.Layout.Padding = 24;
            RightChart.Axis.Y2.Visible = true;
            RightChart.Axis.Y2.Labels.Visible = false;
            RightChart.Axis.Y2.Labels.SeriesLabels.Visible = false;
            RightChart.Axis.Y2.LineThickness = 0;
            RightChart.Axis.Y2.Extent = 12;

            LeftChart.Tooltips.FormatString = "<ITEM_LABEL>";
            RightChart.Tooltips.FormatString = "<ITEM_LABEL>";

            LeftChart.Data.SwapRowsAndColumns = true;
            RightChart.Data.SwapRowsAndColumns = true;

            LeftChart.TitleTop.Visible = true;
            LeftChart.TitleTop.HorizontalAlign = StringAlignment.Center;
            LeftChart.TitleTop.VerticalAlign = StringAlignment.Near;
            LeftChart.TitleTop.Text = string.Empty;
            LeftChart.TitleTop.Font = new Font("Verdana", 10);
            LeftChart.TitleTop.FontColor = Color.Black;

            RightChart.TitleTop.Visible = true;
            RightChart.TitleTop.HorizontalAlign = StringAlignment.Center;
            RightChart.TitleTop.VerticalAlign = StringAlignment.Near;
            RightChart.TitleTop.Text = string.Empty;
            RightChart.TitleTop.Font = new Font("Verdana", 10);
            RightChart.TitleTop.Margins.Left = RightChart.Axis.Y.Extent;
            RightChart.TitleTop.FontColor = Color.Black;

            ChartTextAppearance appearance1 = new ChartTextAppearance();
            appearance1.Column = 0;
            appearance1.Row = -2;
            appearance1.VerticalAlign = StringAlignment.Center;
            appearance1.HorizontalAlign = StringAlignment.Far;
            appearance1.ItemFormatString = "<DATA_VALUE:N0>";
            appearance1.ChartTextFont = new Font("Verdana", 8);
            appearance1.Visible = true;
            appearance1.FontColor = Color.Black;
            LeftChart.BarChart.ChartText.Add(appearance1);
            
            ChartTextAppearance appearance2 = new ChartTextAppearance();
            appearance2.Column = 0;
            appearance2.Row = -2;
            appearance2.VerticalAlign = StringAlignment.Center;
            appearance2.HorizontalAlign = StringAlignment.Far;
            appearance2.ItemFormatString = "<DATA_VALUE:N0>";
            appearance2.ChartTextFont = new Font("Verdana", 8);
            appearance2.Visible = true;
            appearance2.FontColor = Color.Black;
            RightChart.BarChart.ChartText.Add(appearance2);

            LeftChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            RightChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            LeftChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            RightChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.ExcelExportButton.Visible = false;

            CrossLink1.Text = "Структурная&nbsp;динамика&nbsp;фактических&nbsp;доходов";
            CrossLink1.NavigateUrl = "~/reports/FK_0001_0004/DefaultCompareChart_budget.aspx";
            CrossLink2.Text = "Сравнение&nbsp;темпа&nbsp;роста&nbsp;фактических&nbsp;доходов";
            CrossLink2.NavigateUrl = "~/reports/FK_0001_0004/DefaultCompare_budget.aspx";
        }

        void TopGrid_DataBound(object sender, EventArgs e)
        {
            ((UltraWebGrid) sender).Width = Unit.Empty;
            ((UltraWebGrid) sender).Height = Unit.Empty;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FK_0001_0004_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(dtDate.Rows[0][3].ToString(), true);

                ComboKD.Width = 420;
                ComboKD.Title = "Вид дохода";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillShortKDIncludingList());
                ComboKD.SetСheckedState("Доходы ВСЕГО ", true);

                ComboSKIFLevel.Width = 250;
                ComboSKIFLevel.ParentSelect = true;
                ComboSKIFLevel.Title = "Уровень бюджета";
                ComboSKIFLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillSKIFLevels());
                ComboSKIFLevel.SetСheckedState("Консолидированный бюджет субъекта", true);
            }

            Page.Title = "Распределение субъектов РФ на группы по темпам роста доходов";
            Label1.Text = Page.Title;
            int monthNum = ComboMonth.SelectedIndex + 1;
            currentMonth = monthNum;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            currentYear = yearNum;
            LeftChart.TitleTop.Text = string.Format("за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum - 1);
            RightChart.TitleTop.Text = string.Format("за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            Label2.Text = string.Format("{0} ({1})", ComboKD.SelectedValue, ComboSKIFLevel.SelectedValue);

            TopMaxLabel.Text = "<b>7</b> субъектов РФ с <b>максимальным</b> темпом роста доходов:";
            TopMinLabel.Text = "<b>7</b> субъектов РФ с <b>минимальным</b> темпом роста доходов:";

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.KDGroup.Value = ComboKD.SelectedValue;
            UserParams.SKIFLevel.Value = ComboSKIFLevel.SelectedValue;

            LeftChart.DataBind();
            RightChart.DataBind();

            TopMaxGrid.DataBind();
            TopMinGrid.DataBind();

            AVGDataBind(ComboMonth.SelectedIndex + 1, Convert.ToInt32(ComboYear.SelectedValue));
        }

        private void AVGDataBind(int monthNum, int yearNum)
        {
            string query = DataProvider.GetQueryText("FK_0001_0004_avg_budget");
            DataTable dtAVG = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtAVG);

            Label3.Text = string.Empty;
            if (dtAVG.Rows.Count != 0 && 
                dtAVG.Rows[0][0] != DBNull.Value && dtAVG.Rows[0][0].ToString() != string.Empty)
            {
                double avgValue = Convert.ToDouble(dtAVG.Rows[0][0]);
                double medianValue = 0;
                if (dtAVG.Rows[0][1] != DBNull.Value && dtAVG.Rows[0][1].ToString() != string.Empty)
                {
                    medianValue = Convert.ToDouble(dtAVG.Rows[0][1]);
                }

                Label3.Text = string.Format("Средний по РФ темп роста доходов ({3}) за {0} {1} {2} года составляет <b>{4:P2}</b> (медиана <b>{5:P2}</b>).",
                    monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum, ComboKD.SelectedValue, avgValue, medianValue);
            }
        }

        #region Обработчики диаграммы

        protected void LeftChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0004_group_budget_prev");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtChart);

            int[] prevYearArray = new int[12];

            foreach (DataRow row in dtChart.Rows)
            {
                if (row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                {
                    double prevYearValue = Convert.ToDouble(row[1]);
                    int prevYearGroup = GetGroupNumber(prevYearValue);
                    prevYearArray.SetValue(Convert.ToInt32(prevYearArray.GetValue(prevYearGroup)) + 1, prevYearGroup);
                    
                    AddSubject(leftGroupSubjects, prevYearGroup, row[0].ToString());
                }
            }

            DataTable dt = new DataTable();
            DataColumn column1 = new DataColumn("PrevYear", typeof(double));
            dt.Columns.Add(column1);

            for (int i = 0; i < 12; i++)
            {
                DataRow row = dt.NewRow();
                row[0] = -prevYearArray[i];
                dt.Rows.Add(row);
            }

            NumericSeries series = CRHelper.GetNumericSeries(0, dt);
            LeftChart.Series.Clear();
            LeftChart.Series.Add(series);
            //LeftChart.DataSource = dt;
        }

        protected void RightChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0004_group_budget_curr");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtChart);

            int[] currYearArray = new int[12];

            foreach (DataRow row in dtChart.Rows)
            {
                if (row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                {
                    double currYearValue = Convert.ToDouble(row[2]);
                    int currYearGroup = GetGroupNumber(currYearValue);
                    currYearArray.SetValue(Convert.ToInt32(currYearArray.GetValue(currYearGroup)) + 1, currYearGroup);

                    AddSubject(rightGroupSubjects, currYearGroup, row[0].ToString());
                }
            }

            DataTable dt = new DataTable();
            DataColumn column1 = new DataColumn("Группа", typeof(string));
            dt.Columns.Add(column1);
            DataColumn column2 = new DataColumn("CurrYear", typeof(double));
            dt.Columns.Add(column2);

            for (int i = 0; i < 12; i++)
            {
                string rowName;

                if (i == 0)
                {
                    rowName = "менее 50%";
                }
                else
                {
                    if (i == 11)
                    {
                        rowName = "более 150%";
                    }
                    else
                    {
                        rowName = string.Format("от {0}% до {1}%", 10 * (i + 4), 10 * (i + 5));
                    }
                }

                DataRow row = dt.NewRow();
                row[0] = rowName.ToString();
                row[1] = currYearArray[i];
                dt.Rows.Add(row);
            }

            NumericSeries series = CRHelper.GetNumericSeries(1, dt);
            RightChart.Series.Clear();
            RightChart.Series.Add(series);
            //RightChart.DataSource = dt;
        }

        /// <summary>
        /// Добавление в список субъектов (для хинта)
        /// </summary>
        /// <param name="array">массив</param>
        /// <param name="index">индекс группы</param>
        /// <param name="fullName">полное имя субъекта</param>
        private static void AddSubject(string[] array, int index, string fullName)
        {
            if (array == null || index >= array.Length)
            {
                return;
            }

            string shortName = RegionsNamingHelper.ShortName(fullName);

            if (array[index] == null)
            {
                // первый элемент
                array[index] = shortName;
            }
            else
            {
                string[] strs = array[index].Split(',');
                // через каждые 5 делаем перенос строки
                if (strs.Length % 5 == 0)
                {
                    array[index] = string.Format("{0},\n{1}", array[index], shortName);
                }
                else
                {
                    array[index] = string.Format("{0}, {1}", array[index], shortName);
                }
            }
        }

        /// <summary>
        /// Получение номера группы
        /// </summary>
        /// <param name="value">значение</param>
        /// <returns>номер группы</returns>
        private static int GetGroupNumber(double value)
        {
//            // вычисляем десяток
//            double dec = 100 * value;
//            if (dec > 150)
//            {
//                return 11;
//            }
//            else
//            {
//                if (dec < 50)
//                {
//                    return 0;
//                }
//                else
//                {
//                    return Convert.ToInt32((dec - 40) / 10);
//                }
//            }

            value = 100*value;
            
            if (value < 50)
            {
                return 0;
            }

            if (value >= 50 && value < 60)
            {
                return 1;
            }

            if (value >= 60 && value < 70)
            {
                return 2;
            }

            if (value >= 70 && value < 80)
            {
                return 3;
            }

            if (value >= 80 && value < 90)
            {
                return 4;

            }

            if (value >= 90 && value < 100)
            {
                return 5;
            }

            if (value >= 100 && value < 110)
            {
                return 6;
            }

            if (value >= 110 && value < 120)
            {
                return 7;
            }

            if (value >= 120 && value < 130)
            {
                return 8;
            }

            if (value >= 130 && value < 140)
            {
                return 9;
            }

            if (value >= 140 && value <= 150)
            {
                return 10;
            }

            if (value > 150)
            {
                return 11;
            }

            return 0;
        }

        /// <summary>
        /// Получение имени интервала
        /// </summary>
        /// <param name="groupRowIndex">номер строки с группой</param>
        /// <returns></returns>
        private static string GetIntervalName(int groupRowIndex)
        {
            if (groupRowIndex == 0)
            {
                return "более 50%";
            }
            else
            {
                if (groupRowIndex == 11)
                {
                    return "более 50%";
                }
                else
                {
                    if (groupRowIndex > 5)
                    {
                        return string.Format("в интервале от {0}% до {1}%", 10 * (groupRowIndex + 4) - 100, 10 * (groupRowIndex + 5) - 100);
                    }
                    else
                    {
                        return string.Format("в интервале от {0}% до {1}%", 100 - 10 * (groupRowIndex + 5), 100 - 10 * (groupRowIndex + 4));
                    }
                }
            }
        }

        /// <summary>
        /// Склоняет множественное значение слова 'Субъект'
        /// </summary>
        /// <param name="subjectCount">количество субъектов</param>
        /// <returns>полученная форма</returns>
        private static string GetSubjectNameManyGenitive(int subjectCount)
        {
            int x = subjectCount % 10;
            if (x == 1)
            {
                return string.Format("{0} субъекта", subjectCount);
            }
            else
            {
                return string.Format("{0} субъектов", subjectCount);
            }
        }

        /// <summary>
        /// Получение списка субъектов
        /// </summary>
        /// <param name="chart">диаграмма</param>
        /// <param name="groupIndex">индекс списка</param>
        /// <returns>список субъектов</returns>
        private string GetSubjectList(UltraChart chart, int groupIndex)
        {
            if (chart == LeftChart)
            {
                return leftGroupSubjects[groupIndex];
            }
            else if (chart == RightChart)
            {
                return rightGroupSubjects[groupIndex];
            }

            return string.Empty;
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {
                        int value = Convert.ToInt32(box.Value);
                        if (value < 0)
                        {
                            value = -value;
                        }

                        // при этом условии как раз больше 100%
                        if (box.Row > 5)
                        {
                            box.DataPoint.Label = string.Format("Рост доходов {0}\nнаблюдается у {1} РФ \n({2})",
                                GetIntervalName(box.Row), GetSubjectNameManyGenitive(value), GetSubjectList((UltraChart)sender, box.Row));

                            box.PE.Fill = Color.Green;
                            box.PE.FillStopColor = Color.DarkGreen;
                        }
                        else
                        {
                            box.DataPoint.Label = string.Format("Снижение доходов {0}\nнаблюдается у {1} РФ \n({2})",
                                GetIntervalName(box.Row), GetSubjectNameManyGenitive(value), GetSubjectList((UltraChart)sender, box.Row));

                            box.PE.Fill = Color.Red;
                            box.PE.FillStopColor = Color.DarkRed;
                        }
                    }
                }

                if (sender == LeftChart && primitive is Text && primitive.Path == null)
                {
                    Text text = (Text)primitive;
                    if (i != 0 && e.SceneGraph[i - 1] is Box)
                    {
                        // сдвигаем текст по левой стороне предыдущей полоски
                        Box prevBox = (Box)e.SceneGraph[i - 1];
                        int boxLeft = prevBox.rect.Left;
                        text.bounds = new Rectangle(boxLeft, text.bounds.Top, text.bounds.Width, text.bounds.Height);
                    }

                    // переводим в положительное значение
                    string textString = text.GetTextString();
                    if (textString != string.Empty)
                    {
                        int value = Convert.ToInt32(textString);
                        if (value < 0)
                        {
                            text.SetTextString((-value).ToString());
                        }
                    }
                }
            }
        }

        #endregion

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = (sender == TopMaxGrid) ? DataProvider.GetQueryText("FK_0001_0004_top_max_budget") : DataProvider.GetQueryText("FK_0001_0004_top_min_budget");
            DataTable dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект РФ", dtGrid);

            ((UltraWebGrid)sender).DataSource = dtGrid;
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            ((UltraWebGrid)sender).Height = Unit.Empty;
        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width, bool hidden)
        {
            CRHelper.FormatNumberColumn(layout.Bands[bandIndex].Columns[columnIndex], format);
            layout.Bands[bandIndex].Columns[columnIndex].Hidden = hidden;
            layout.Bands[bandIndex].Columns[columnIndex].Width = CRHelper.GetColumnWidth(width);
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowRowNumberingDefault = RowNumbering.Continuous;

            if (e.Layout.Bands.Count == 0 || e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            SetColumnParams(e.Layout, 0, 0, "", 200, false);
            SetColumnParams(e.Layout, 0, 1, "P2", 100, false);
            SetColumnParams(e.Layout, 0, 2, "P2", 100, false);

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.Font.Bold = true;
            e.Layout.Bands[0].Columns[2].Header.Style.Font.ClearDefaults();
            e.Layout.Bands[0].Columns[2].HeaderStyle.Font.Bold = false;
            e.Layout.Bands[0].Columns[2].Header.Style.Font.Italic = true;
            e.Layout.Bands[0].Columns[2].CellStyle.Font.Italic = true;
            //e.Layout.Bands[0].Columns[2].Header.Style.ForeColor = Color.FromArgb(130, 130, 130);
            //e.Layout.Bands[0].Columns[2].CellStyle.ForeColor = Color.FromArgb(130, 130, 130);
 
            e.Layout.Bands[0].Columns[1].Header.Caption = string.Format("Темп роста в {0} году, %", currentYear);
            e.Layout.Bands[0].Columns[2].Header.Caption = string.Format("Темп роста в {0} году, %", currentYear - 1);
            e.Layout.Bands[0].Columns[1].Header.Title = string.Format("Темп роста доходов за {0} {1} {2} года в процентах", currentMonth, CRHelper.RusManyMonthGenitive(currentMonth), currentYear);
            e.Layout.Bands[0].Columns[2].Header.Title = string.Format("Темп роста доходов за {0} {1} {2} года в процентах", currentMonth, CRHelper.RusManyMonthGenitive(currentMonth), currentYear - 1);
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            foreach (UltraGridCell cell in e.Row.Cells)
            {
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        #endregion

        #region Экспорт в PDF

        private static string ClearTags(string text)
        {
            return text.Replace("<b>", " ").Replace("</b>", " ");
        }

        private static void SetTopGridStyle(UltraWebGrid grid)
        {
            grid.Width = 420;
            grid.Height = 220;

            if (grid.Columns.Count > 2)
            {
                grid.Columns[0].Width = 200;
                grid.Columns[1].Width = 100;
                grid.Columns[2].Width = 100;
            }
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            Report report = new Report();
            ReportSection section = new ReportSection(report, true);

            SetTopGridStyle(TopMaxGrid);
            UltraGridExporter1.PdfExporter.Export(TopMaxGrid, section);
            
            section.AddFlowColumnBreak();
            
            SetTopGridStyle(TopMinGrid);
            UltraGridExporter1.PdfExporter.Export(TopMinGrid, section);
        }

        private bool firstChartExported = false;

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            if (!firstChartExported)
            {
                AddTitle(e.Section);
                AddFirstChart(e.Section);
                firstChartExported = true;
                return;
            }

            AddSecondChart(e.Section);
        }

        private void AddFirstChart(ISection section)
        {
            LeftChart.Width = 555;
            LeftChart.Height = 450;
            Image img = UltraGridExporter.GetImageFromChart(LeftChart);
            section.AddImage(img);

            IText text = ((ReportSection)section).flow.AddText();
            text.Alignment.Horizontal = Alignment.Center;
            text.AddContent(ClearTags(TopMaxLabel.Text));
        }

        private void AddSecondChart(ISection section)
        {
            RightChart.Width = 660;
            RightChart.Height = 450;
            Image img = UltraGridExporter.GetImageFromChart(RightChart);
            section.AddImage(img);

            IText text = ((ReportSection)section).flow.AddText();
            text.Alignment.Horizontal = Alignment.Center;
            text.AddContent(ClearTags(TopMinLabel.Text));
        }

        private void AddTitle(ISection section)
        {
            IText title = section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text.Replace("&nbsp;", " "));

            title = ((ReportSection)section).bottomTitleFlow.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(ClearTags(Label3.Text));

            title = section.AddText();
            font = new Font("Verdana", 10);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Alignment.Horizontal = Alignment.Center;
            title.Alignment.Vertical = Alignment.Bottom;
            title.AddContent("Группировка субъектов по темпам роста доходов");
        }

        #endregion
    }

    public class ReportSection : ISection
    {
		private readonly bool withFlowColumns;
        private readonly ISection section;
        public IFlow flow;
        public IFlow bottomTitleFlow;
        private ITableCell titleCell;
       


        public ReportSection(Report report, bool withFlowColumns)
        {
            this.withFlowColumns = withFlowColumns;
            section = report.AddSection();
            ITable table = section.AddTable();
            ITableRow row = table.AddRow();
            titleCell = row.AddCell();
            if (this.withFlowColumns)
            {
                flow = section.AddFlow();
                IFlowColumn col = flow.AddColumn();
                col.Width = new FixedWidth(423);
                col.Alignment.Horizontal = Alignment.Center;
                col = flow.AddColumn();
                col.Width = new FixedWidth(500);
                col.Alignment.Horizontal = Alignment.Center;
            }

            bottomTitleFlow = section.AddFlow();
            IFlowColumn bottomTitleColumn = flow.AddColumn();
        }

        public void AddFlowColumnBreak()
        {
            if (flow != null)
                flow.AddColumnBreak();
        }

        public IBand AddBand()
        {
            if (flow != null)
                return flow.AddBand();
            return section.AddBand();
        }

        #region ISection members
        public ISectionHeader AddHeader()
        {
            throw new NotImplementedException();
        }

        public ISectionFooter AddFooter()
        {
            throw new NotImplementedException();
        }

        public IStationery AddStationery()
        {
            throw new NotImplementedException();
        }

        public IDecoration AddDecoration()
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage()
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage(PageSize size)
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage(float width, float height)
        {
            throw new NotImplementedException();
        }

        public ISegment AddSegment()
        {
            throw new NotImplementedException();
        }

        public IQuickText AddQuickText(string text)
        {
            throw new NotImplementedException();
        }

        public IQuickImage AddQuickImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            throw new NotImplementedException();
        }

        public IQuickList AddQuickList()
        {
            throw new NotImplementedException();
        }

        public IQuickTable AddQuickTable()
        {
            throw new NotImplementedException();
        }

        public IText AddText()
        {
            return this.titleCell.AddText();
        }

        public IText AddGridText()
        {
            return this.flow.AddText();
        }

        public IImage AddImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            if (flow != null)
                return flow.AddImage(image);
            return this.section.AddImage(image);
        }

        public IMetafile AddMetafile(Metafile metafile)
        {
            throw new NotImplementedException();
        }

        public IRule AddRule()
        {
            throw new NotImplementedException();
        }

        public IGap AddGap()
        {
            throw new NotImplementedException();
        }

        public IGroup AddGroup()
        {
            throw new NotImplementedException();
        }

        public IChain AddChain()
        {
            throw new NotImplementedException();
        }

        public ITable AddTable()
        {
            if (flow != null)
                return flow.AddTable();
            return this.section.AddTable();
        }

        public IGrid AddGrid()
        {
            throw new NotImplementedException();
        }

        public IFlow AddFlow()
        {
            throw new NotImplementedException();
        }

        public Infragistics.Documents.Reports.Report.List.IList AddList()
        {
            throw new NotImplementedException();
        }

        public ITree AddTree()
        {
            throw new NotImplementedException();
        }

        public ISite AddSite()
        {
            throw new NotImplementedException();
        }

        public ICanvas AddCanvas()
        {
            throw new NotImplementedException();
        }

        public IRotator AddRotator()
        {
            throw new NotImplementedException();
        }

        public IContainer AddContainer(string name)
        {
            throw new NotImplementedException();
        }

        public ICondition AddCondition(IContainer container, bool fit)
        {
            throw new NotImplementedException();
        }

        public IStretcher AddStretcher()
        {
            throw new NotImplementedException();
        }

        public void AddPageBreak()
        {
            throw new NotImplementedException();
        }

        public ITOC AddTOC()
        {
            throw new NotImplementedException();
        }

        public IIndex AddIndex()
        {
            throw new NotImplementedException();
        }

        public bool Flip
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public PageSize PageSize
        {
            get { throw new NotImplementedException(); }
            set { this.section.PageSize = new PageSize(1000, 1350); }
        }

        public PageOrientation PageOrientation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.ContentAlignment PageAlignment
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Borders PageBorders
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.Margins PageMargins
        {
            get { return this.section.PageMargins; }
            set { throw new NotImplementedException(); }
        }

        public Paddings PagePaddings
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Background PageBackground
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.Section.PageNumbering PageNumbering
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public SectionLineNumbering LineNumbering
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Report Parent
        {
            get { return this.section.Parent; }
        }
        
        public IEnumerable Content
        {
            get { throw new NotImplementedException(); }
        }       
		 
        #endregion
    }
}

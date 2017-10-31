using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs=Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using System.Collections;

namespace Krista.FM.Server.Dashboards.reports.STAT_0001_0006
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtGrid1;
        private DataTable dtChart;
        private int firstYear = 2007;
        private int endYear = 2009;
        private DateTime date;
        private double urfoAverage = 0;
        private int selectedPointIndex = -1;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 65);
            UltraWebGrid.Height = Unit.Empty;

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 70);
            UltraWebGrid1.Height = Unit.Empty;

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 70);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6 - 50);

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ScatterChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.Axis.X.Extent = 50;
            UltraChart.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.Extent = 40;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.Text = "Финансовая помощь федерального бюджета,\nтыс.руб. на единицу экономически активного населения";
            UltraChart.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Extent = 50;

            UltraChart.TitleBottom.Visible = true;
            UltraChart.TitleBottom.Text = "Численность экономически активного населения, чел.";
            UltraChart.TitleBottom.Font = new Font("Verdana", 10);
            UltraChart.TitleBottom.HorizontalAlign = StringAlignment.Center;

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.SpanPercentage = 12;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.DataAssociation = ChartTypeData.ScatterData;
            UltraChart.Legend.Margins.Top = 0;
            UltraChart.Legend.Margins.Right = Convert.ToInt32(UltraChart.Width.Value * 0.2);
            UltraChart.Legend.Font = new Font("Verdana", 10);

            UltraChart.ScatterChart.Icon = SymbolIcon.Square;
            UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;
            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>\nФинансовая помощь: <DATA_VALUE_Y:N3> тыс.руб. на единицу экономически активного населения\nЧисленность экономически активного населения: <DATA_VALUE_X:N0> чел.";
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            UltraChart.Style.Add("padding-top", "10px");

            #endregion

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

          //  UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);
                        
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <EndExportEventArgs>(PdfExporter_EndExport);
        }

        void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
         //   UserParams.SubjectFO.Value = ComboRegiones.SelectedValue == "Уральский федеральный округ"
         //                                    ? String.Empty
         //                                    : String.Format(".[{0}]", ComboRegiones.SelectedValue);
         //   UltraWebGrid1.DataBind();
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

          //  WebAsyncPanel.AddRefreshTarget(UltraWebGrid1);
          //  WebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("STAT_0001_0006_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query,"date", dtDate);
                date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);
                endYear = date.Year;

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                String month = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(date.Month));
                ComboMonth.SetСheckedState(month, true);

                FillComboRegions();
                ComboRegiones.Title = "Территория";
                ComboRegiones.Width = 400;
                ComboRegiones.SetСheckedState("Уральский федеральный округ", true);
                ComboRegiones.ParentSelect = true;
            }

            date = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), ComboMonth.SelectedIndex + 1, 1);
            UserParams.PeriodMonth.Value = CRHelper.PeriodMemberUName(String.Empty, date, 4);
            UserParams.PeriodLastYear.Value= CRHelper.PeriodMemberUName(String.Empty, date.AddYears(-1), 4);
            UserParams.Filter.Value = date.AddYears(-1).Year.ToString();
            UserParams.SubjectFO.Value = String.Empty;
            UserParams.PeriodYear.Value = date.Year.ToString();
            UserParams.SubjectFO.Value = ComboRegiones.SelectedValue == "Уральский федеральный округ"
                                             ? String.Empty
                                             : String.Format(".[{0}]", ComboRegiones.SelectedValue);

            Label1.Text = "Соотношение численности экономически активного населения и финансовой помощи";
            string firstMonth = ComboMonth.SelectedValue == "Январь" ? String.Empty : "январь-";
            Label2.Text =
                String.Format(
                    "Анализ соотношения численности экономически активного населения в субъектах Российской Федерации, входящих в Уральский федеральный округ и объемов финансовой помощи федерального бюджета на поддержку малого предпринимательства, на реализацию программ местного развития и обеспечения занятости за {0}{1} {2} года",
                    firstMonth, ComboMonth.SelectedValue.ToLower(), ComboYear.SelectedValue);

            UltraWebGrid.DataBind();
            UltraWebGrid1.DataBind();
            UltraChart.DataBind();

            gridElementCaption.Text =
                String.Format("Финансовая помощь в разрезе подстатей классификации доходов бюджета ({0})", ComboRegiones.SelectedValue);

            CRHelper.FindGridRow(UltraWebGrid, ComboRegiones.SelectedValue, 0);
        }

        #region Обработчики грида

        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {
            if (row == null) return;
            UserParams.Subject.Value = row.Cells[0].Text;
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0006_grid_1");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Территория", dtGrid);
            UltraWebGrid.DataSource = dtGrid;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.Wrap = true;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N3");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N3");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "N3");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[9], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[10], "N3");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[11], "N0");

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(130);
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(90);
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(86);
            e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(86);
            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(86);
            e.Layout.Bands[0].Columns[5].Width = CRHelper.GetColumnWidth(97);
            e.Layout.Bands[0].Columns[6].Width = CRHelper.GetColumnWidth(96);
            e.Layout.Bands[0].Columns[7].Width = CRHelper.GetColumnWidth(111);
            e.Layout.Bands[0].Columns[8].Width = CRHelper.GetColumnWidth(99);
            e.Layout.Bands[0].Columns[9].Width = CRHelper.GetColumnWidth(96);
            e.Layout.Bands[0].Columns[10].Width = CRHelper.GetColumnWidth(91);
            e.Layout.Bands[0].Columns[11].Width = CRHelper.GetColumnWidth(71);

            e.Layout.Bands[0].Columns[2].Header.Title =
                "Фактическая общая численность зарегистрированных безработных граждан, человек";
            e.Layout.Bands[0].Columns[3].Header.Title = "Уровень регистрируемой безработицы";
            e.Layout.Bands[0].Columns[4].Header.Title = "Ранг (место) по уровню регистрируемой безработицы среди субъектов УрФО";
            e.Layout.Bands[0].Columns[5].Header.Title = "Отношение числа безработных граждан к числу вакансий";
            e.Layout.Bands[0].Columns[6].Header.Title = "Ранг (место) по числу безработных на 1 вакансию среди субъектов УрФО";
            e.Layout.Bands[0].Columns[7].Header.Title = "План на год";
            e.Layout.Bands[0].Columns[8].Header.Title = "Фактическое исполнение нарастающим итогом с начала года";
            e.Layout.Bands[0].Columns[9].Header.Title = "Процент исполнения по доходам. Оценка равномерности исполнения (1/12 годового плана в месяц)";
            e.Layout.Bands[0].Columns[10].Header.Title = "Финансовая помощь федерального бюджета, тыс.руб. на единицу экономически активного населения";
            e.Layout.Bands[0].Columns[11].Header.Title = "Ранг (место) по сумме финансовой помощи в расчете на единицу экономически активного населения по субъектам УрФО";

            e.Layout.Bands[0].Columns[12].Hidden = true;
            e.Layout.Bands[0].Columns[13].Hidden = true;
            e.Layout.Bands[0].Columns[14].Hidden = true;
            e.Layout.Bands[0].Columns[15].Hidden = true;
            e.Layout.Bands[0].Columns[16].Hidden = true;

            e.Layout.Bands[0].RowStyle.Height = 27;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }

            SetRankImage(e.Row, 4, false);
            SetRankImage(e.Row, 6, false);
            SetRankImage(e.Row, 11, true);

            SetGrownImage(e.Row, 2);
            SetGrownImage(e.Row, 3);

            SetAssessionImage(e.Row, 9, 7);

            if (e.Row.Cells[3].Value != null && !String.IsNullOrEmpty(e.Row.Cells[3].Value.ToString()))
            {
                e.Row.Cells[3].Value = String.Format("{0:N3}%", e.Row.Cells[3].Value);
                e.Row.Cells[3].Style.HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[3].Style.Padding.Right = 5;
            }
        }

        private void SetGrownImage(UltraGridRow row, int i)
        {
            if (row.Cells[i].Value != null && row.Cells[i + 10].Value != null)
            {
                double value;
                if (Double.TryParse(row.Cells[i + 10].ToString(), out value))
                {
                    if (value > 1)
                    {
                        row.Cells[i].Style.BackgroundImage = "../../images/arrowRedUpBB.png";
                        row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left top;";

                        if (i == 1)
                        {
                            row.Cells[i].Title =
                                String.Format(
                                    "Рост к прошлому году. \nЧисло безработных за {0} {1}г. {2:N0} чел. \nТемп роста {3:P2}",
                                    CRHelper.ToLowerFirstSymbol(ComboMonth.SelectedValue), date.AddYears(-1).Year,
                                    row.Cells[i + 12].Value, value);
                        }
                        else
                        {
                            row.Cells[i].Title =
                                String.Format(
                                    "Рост к прошлому году. \nУровень безработицы за {0} {1}г. {2:N3}%. \nТемп роста {3:P2}",
                                    CRHelper.ToLowerFirstSymbol(ComboMonth.SelectedValue), date.AddYears(-1).Year,
                                    row.Cells[i + 12].Value, value);
                        }
                    }
                    else
                    {
                        row.Cells[i].Style.BackgroundImage = "../../images/arrowGreenDownBB.png";
                        row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left top;";
                        if (i == 1)
                        {
                            row.Cells[i].Title = String.Format(
                                "Снижение к прошлому году. \nЧисло безработных за {0} {1}г. {2:N0} чел. \nТемп роста {3:P2}",
                                CRHelper.ToLowerFirstSymbol(ComboMonth.SelectedValue), date.AddYears(-1).Year,
                                row.Cells[i + 12].Value, value);
                        }
                        else
                        {
                            row.Cells[i].Title = String.Format(
                                "Снижение к прошлому году. \nУровень безработицы за {0} {1}г. {2:N3}%. \nТемп роста {3:P2}",
                                CRHelper.ToLowerFirstSymbol(ComboMonth.SelectedValue), date.AddYears(-1).Year,
                                row.Cells[i + 12].Value, value);
                        }
                    }
                }
            }
        }

        private void SetAssessionImage(UltraGridRow row, int i, int lastYearColumnOffset)
        {
            if (row.Cells[i].Value != null)
            {
                string lastYear = string.Empty;
                double lastValue;
                if (row.Cells[i + lastYearColumnOffset] != null && 
                    row.Cells[i + lastYearColumnOffset].Value != null && 
                    Double.TryParse(row.Cells[i + lastYearColumnOffset].Value.ToString(), out lastValue))
                {
                    lastYear = String.Format(" Исполнено за {0} {1}г. {2:P2}", CRHelper.ToLowerFirstSymbol(ComboMonth.SelectedValue), date.AddYears(-1).Year, lastValue);
                }
                double value;
                if (Double.TryParse(row.Cells[i].ToString(), out value))
                {
                    if (value > AssessionEthalon())
                    {
                        row.Cells[i].Style.BackgroundImage = "../../images/BallGreenBB.png";
                        row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left top;";
                        
                        row.Cells[i].Title =
                            String.Format("Соблюдается условие равномерности ({0:P2}).{1}", AssessionEthalon(), lastYear);
                    }
                    else
                    {
                        row.Cells[i].Style.BackgroundImage = "../../images/BallRedBB.png";
                        row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left top;";
                        row.Cells[i].Title =
                               String.Format("Не соблюдается условие равномерности ({0:P2}).{1}", AssessionEthalon(), lastYear);
                    }
                }
            }
        }

        private static void SetRankImage(UltraGridRow row, int i, bool directAccess)
        {
            if (row.Cells[i].ToString() == "6")
            {
                row.Cells[i].Style.BackgroundImage = directAccess ? "../../images/StarGrayBB.png"  : "../../images/StarYellowBB.png";
                row.Cells[i].Style.CustomRules =
                    "background-repeat: no-repeat; background-position: left top;";
                if (i == 3)
                {
                    row.Cells[i].Title = "Самый низкий уровень безработицы среди субъектов УрФО";
                }
                else if (i == 5)
                {
                    row.Cells[i].Title = "Самое маленькое число безработных на 1 вакансию среди субъектов УрФО";
                } if (i == 10)
                {
                    row.Cells[i].Title = "Самая низкая финансовая помощь на 1 безработного среди субъектов УрФО";
                }

            }
            else if (row.Cells[i].ToString() == "1")
            {
                row.Cells[i].Style.BackgroundImage = directAccess ? "../../images/StarYellowBB.png" : "../../images/StarGrayBB.png";
                row.Cells[i].Style.CustomRules =
                    "background-repeat: no-repeat; background-position: left top;";
                if (i == 3)
                {
                    row.Cells[i].Title = "Самый высокий уровень безработицы среди субъектов УрФО";
                }
                else if (i == 5)
                {
                    row.Cells[i].Title = "Самое большое число безработных на 1 вакансию среди субъектов УрФО";
                } if (i == 10)
                {
                    row.Cells[i].Title = "Самая высокая финансовая помощь на 1 безработного среди субъектов УрФО";
                }
            }
            
        }

        private double AssessionEthalon()
        {
            return ((double)(ComboMonth.SelectedIndex + 1)) / 12.0; 
        }

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            dtGrid1 = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0006_grid_2");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Подстатья", dtGrid1);
            UltraWebGrid1.DataSource = dtGrid1;
        }

        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.Wrap = true;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N3");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N3");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P2");

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(450);
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(150);

            e.Layout.Bands[0].Columns[1].Header.Title = "План на год";
            e.Layout.Bands[0].Columns[2].Header.Title = "Фактическое исполнение нарастающим итогом с начала года";
            e.Layout.Bands[0].Columns[3].Header.Title = "Процент исполнения по доходам. Оценка равномерности исполнения (1/12 годового плана в месяц)";

            e.Layout.Bands[0].Columns[4].Hidden = true;
        }

        protected void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            SetAssessionImage(e.Row, 3, 1);
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            dtChart = new DataTable();

            string query = DataProvider.GetQueryText("STAT_0001_0006_chart1");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Территория", dtChart);

            DataRow[] rows = dtChart.Select(String.Format("Территория like '{0}'", ComboRegiones.SelectedValue));

            if (rows.Length > 0)
            {
                double finHelp;
                double unemployedCount;
                double.TryParse(rows[0][1].ToString(), out finHelp);
                double.TryParse(rows[0][2].ToString(), out unemployedCount);
                chartLabel.Text = String.Format("{0}:<br/>Финансовая помощь <b>{2:N3}</b> тыс. руб. на единицу экономически активного населения<br/>Численность экономически активного населения: <b>{1:N0}</b> чел.", ComboRegiones.SelectedValue, finHelp, unemployedCount);
            }

            if (dtChart.Rows[0][2] != DBNull.Value)
            {
                urfoAverage = Convert.ToDouble(dtChart.Rows[0][2]);
            }

            dtChart.Rows.RemoveAt(0);
            dtChart.AcceptChanges();

            selectedPointIndex = -1;
            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                if (dtChart.Rows[i][0] != DBNull.Value && dtChart.Rows[i][0].ToString() == ComboRegiones.SelectedValue)
                {
                    selectedPointIndex = i;
                    break;
                }
            }

            for (int i = 1; i < 6; i++)
            {
                DataColumn col = new DataColumn(dtChart.Rows[i][0].ToString(), typeof(double));
                dtChart.Columns.Add(col);
                dtChart.Rows[i][2 + i] = dtChart.Rows[i][2];
                dtChart.Rows[i][2] = DBNull.Value;
            }


            XYSeries series1 = CRHelper.GetXYSeries(1, 2, dtChart);
            series1.Label = "Курганская область";
            UltraChart.Series.Add(series1); 

            XYSeries series2 = CRHelper.GetXYSeries(1, 3, dtChart);
            series2.Label = "Свердловская область";
            UltraChart.Series.Add(series2);

            XYSeries series3 = CRHelper.GetXYSeries(1, 4, dtChart);
            series3.Label = "Тюменская область";
            UltraChart.Series.Add(series3);

            XYSeries series4 = CRHelper.GetXYSeries(1, 5, dtChart);
            series4.Label = "Ханты-Мансийский автономный округ";
            UltraChart.Series.Add(series4);

            XYSeries series5 = CRHelper.GetXYSeries(1, 6, dtChart);
            series5.Label = "Челябинская облась";
            UltraChart.Series.Add(series5);

            XYSeries series6 = CRHelper.GetXYSeries(1, 7, dtChart);
            series6.Label = "Ямало-Ненецкий автономный округ";
            UltraChart.Series.Add(series6);

        /*    XYSeries series2 = CRHelper.GetXYSeries(1, 3, dtChart);
            series2.Label = "УрФО";
            UltraChart.Series.Add(series2); */

         //   UltraChart.DataSource = dtChart;
        }

        void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            int xMin = (int)xAxis.MapMinimum;
            int yMin = (int)yAxis.MapMinimum;
            int xMax = (int)xAxis.MapMaximum;
            int yMax = (int)yAxis.MapMaximum;

            if (urfoAverage != 0)
            {
                int fmY = (int) yAxis.Map(urfoAverage);
                Line line = new Line();
                line.lineStyle.DrawStyle = LineDrawStyle.Dot;
                line.PE.Stroke = Color.DarkGray;
                line.PE.StrokeWidth = 2;
                line.p1 = new Point(xMin, fmY);
                line.p2 = new Point(xMax, fmY);
                e.SceneGraph.Add(line);

                /*   int sgmX = (int)xAxis.Map(sgmMedian);
                line = new Line();
                line.lineStyle.DrawStyle = LineDrawStyle.Dot;
                line.PE.Stroke = Color.DarkGray;
                line.PE.StrokeWidth = 2;
                line.p1 = new Point(sgmX, yMin);
                line.p2 = new Point(sgmX, yMax);
                e.SceneGraph.Add(line); */

                Text text = new Text();
                text.labelStyle.Font = new Font("Verdana", 10);
                text.PE.Fill = Color.Black;
                text.bounds = new Rectangle(xMax/2, fmY - 20, 780, 15);
                text.SetTextString(String.Format("В целом по УрФО: {0:N3} тыс.руб. на единицу экономически активного населения", urfoAverage));
                e.SceneGraph.Add(text);
            }
            //
            //            text = new Text();
            //            text.PE.Fill = Color.Black;
            //            text.bounds = new Rectangle(3 * xMax / 4, yMin + 55, 100, 20);
            //            text.SetTextString("Большая заболеваемость");
            //            e.SceneGraph.Add(text);
            //
            //            text = new Text();
            //            text.PE.Fill = Color.Black;
            //            text.bounds = new Rectangle(xMin - 65, (yMin - yMax) / 16, 20, 150);
            //            text.SetTextString("Большой коэффициент бюдж.обесп.");
            //            LabelStyle style = new LabelStyle();
            //            style.Orientation = TextOrientation.VerticalLeftFacing;
            //            text.SetLabelStyle(style);
            //            e.SceneGraph.Add(text);
            //
            //            text = new Text();
            //            text.PE.Fill = Color.Black;
            //            text.bounds = new Rectangle(xMin - 65, 6 * (yMin - yMax) / 8, 20, 160);
            //            text.SetTextString("Небольшой коэффициент бюдж.обесп.");
            //            style = new LabelStyle();
            //            style.Orientation = TextOrientation.VerticalLeftFacing;
            //            text.SetLabelStyle(style);
            //            e.SceneGraph.Add(text);

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Symbol)
                {
                    Symbol icon = primitive as Symbol;
                    if (icon.Path == "Legend")
                    {
                        Primitive prevPrimitive = e.SceneGraph[i - 1];
                        if (prevPrimitive is Text)
                        {
                            string legendText = ((Text)prevPrimitive).GetTextString();
                            icon.icon = GetIconType(legendText);
                            icon.iconSize = SymbolIconSize.Medium;
                        }
                    }
                    else if (icon.Series != null)
                    {
                        icon.icon = GetIconType(icon.Series.Label);
                    }
                }
            }

            foreach (Primitive primitive in e.SceneGraph)
            {
                if (selectedPointIndex == -1)
                {
                    break;
                }

                PointSet pointSet = primitive as PointSet;

                if (pointSet == null)
                {
                    continue;
                }

                foreach (DataPoint point in pointSet.points)
                {
                    if (point.Row == selectedPointIndex)
                    {
                        Symbol symbol = new Symbol(point.point, SymbolIcon.Diamond, pointSet.iconSize);
                        symbol.PE.Fill = Color.DarkOrange;
                        e.SceneGraph.Add(symbol);
                    }
                }

                break;
            }
        }

        private SymbolIcon GetIconType(string seriesName)
        {
           /* SymbolIcon iconType;
            switch (seriesName)
            {
                case "Субъекты УрФО":
                    {
                        iconType = SymbolIcon.Circle;
                        break;
                    }
                case "УрФО":
                    {
                        iconType = SymbolIcon.Square;
                        break;
                    }
                case "Российская Федерация":
                    {
                        iconType = SymbolIcon.Diamond;
                        break;
                    }
                default:
                    {
                        iconType = SymbolIcon.Random;
                        break;
                    }
            } */
            return SymbolIcon.Circle;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text;
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            if (e.CurrentWorksheet.Name == "Лист 1")
            {
                e.CurrentWorksheet.Columns[0].Width = 150 * 37;
                e.CurrentWorksheet.Columns[1].Width = 100 * 37;
                e.CurrentWorksheet.Columns[2].Width = 100 * 37;
                e.CurrentWorksheet.Columns[3].Width = 100 * 37;
                e.CurrentWorksheet.Columns[4].Width = 100 * 37;
                e.CurrentWorksheet.Columns[5].Width = 100 * 37;
                e.CurrentWorksheet.Columns[6].Width = 100 * 37;
                e.CurrentWorksheet.Columns[7].Width = 100 * 37;
                e.CurrentWorksheet.Columns[8].Width = 100 * 37;
                e.CurrentWorksheet.Columns[9].Width = 100 * 37;
                e.CurrentWorksheet.Columns[10].Width = 100 * 37;

           //     e.CurrentWorksheet.Columns[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;

                e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "#,##0";
                e.CurrentWorksheet.Columns[2].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
                e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "#,##0";
                e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "#,##0.000";
                e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "#,##0";
                e.CurrentWorksheet.Columns[6].CellFormat.FormatString = "#,##0.000";
                e.CurrentWorksheet.Columns[7].CellFormat.FormatString = "#,##0.000";
                e.CurrentWorksheet.Columns[8].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
                e.CurrentWorksheet.Columns[9].CellFormat.FormatString = "#,##0.000";
                e.CurrentWorksheet.Columns[10].CellFormat.FormatString = "#,##0";
            }
            else if (e.CurrentWorksheet.Name == "Лист 2")
            {
                e.CurrentWorksheet.Columns[0].Width = 450 * 37;
               /* e.CurrentWorksheet.Columns[1].Width = 100 * 37;
                e.CurrentWorksheet.Columns[2].Width = 100 * 37;
                e.CurrentWorksheet.Columns[3].Width = 400 * 37; */

            //    e.CurrentWorksheet.Columns[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;

                e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "#,##0.000";
                e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.000";
                e.CurrentWorksheet.Columns[2].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            }
            
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Лист 1");
            Worksheet sheet2 = workbook.Worksheets.Add("Лист 2");

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";

           // SetExportGridParams(CommentGrid);

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid1, sheet2);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {

        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

            Report report = new Report();
            ReportSection section1 = new ReportSection(report);

            IText title = section1.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = section1.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text);

            UltraChart.Width = 1100;
            section1.AddImage(UltraGridExporter.GetImageFromChart(UltraChart));
            section1.AddPageBreak();
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid, section1);
            IText text = section1.AddText();
            text.AddContent("     ");
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid1, section1);
        }

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(90);
            }
        }

        private void PdfExporter_EndExport(object sender, EndExportEventArgs e)
        {
            //    IText title = e.Section.AddText();
            //    Font font = new Font("Verdana", 14);
            //    title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            //    title.AddContent(chartHeaderLabel.Text);
           
        }



        #endregion

        private void FillComboRegions()
        {
            DataTable dtRegions = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0006_regions");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Район", dtRegions);

            Dictionary<string, int> regions = new Dictionary<string, int>();
            //regions.Add(dtRegions.Rows[0][1].ToString(), 0);
            foreach (DataRow row in dtRegions.Rows)
            {
                regions.Add(row[0].ToString(), 0);
            }
            regions.Add("Уральский федеральный округ", 0);
            ComboRegiones.FillDictionaryValues(regions);
        }
    }

    public class ReportSection : ISection
    {

        #region ISection Members

        private readonly ISection section;

        public ReportSection(Report report)
        {
            section = report.AddSection();
        }

        public Infragistics.Documents.Reports.Report.Band.IBand AddBand()
        {
            throw new Exception("The method or operation is not implemented.");

        }

        public Infragistics.Documents.Reports.Report.ICanvas AddCanvas()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.IChain AddChain()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.ICondition AddCondition(Infragistics.Documents.Reports.Report.IContainer container, bool fit)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.IContainer AddContainer(string name)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IDecoration AddDecoration()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.Flow.IFlow AddFlow()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ISectionFooter AddFooter()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.IGap AddGap()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.Grid.IGrid AddGrid()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.IGroup AddGroup()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ISectionHeader AddHeader()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.IImage AddImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            return this.section.AddImage(image);
        }

        public Infragistics.Documents.Reports.Report.Index.IIndex AddIndex()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.List.IList AddList()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.IMetafile AddMetafile(Infragistics.Documents.Reports.Graphics.Metafile metafile)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ISectionPage AddPage(float width, float height)
        {
            return this.section.AddPage(width, height);
        }

        public ISectionPage AddPage(Infragistics.Documents.Reports.Report.PageSize size)
        {
            return this.section.AddPage(size);
        }

        public ISectionPage AddPage()
        {
            return this.section.AddPage();
        }

        public void AddPageBreak()
        {
            section.AddPageBreak();
        }

        public Infragistics.Documents.Reports.Report.IQuickImage AddQuickImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.QuickList.IQuickList AddQuickList()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.QuickTable.IQuickTable AddQuickTable()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.QuickText.IQuickText AddQuickText(string text)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.IRotator AddRotator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.IRule AddRule()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.Segment.ISegment AddSegment()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.ISite AddSite()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IStationery AddStationery()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.IStretcher AddStretcher()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.TOC.ITOC AddTOC()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.Table.ITable AddTable()
        {
            return section.AddTable();
        }

        public IText AddText()
        {
            return section.AddText();
        }

        public Infragistics.Documents.Reports.Report.Tree.ITree AddTree()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Flip
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public SectionLineNumbering LineNumbering
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public Infragistics.Documents.Reports.Report.ContentAlignment PageAlignment
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public Infragistics.Documents.Reports.Report.Background PageBackground
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public Infragistics.Documents.Reports.Report.Borders PageBorders
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public Infragistics.Documents.Reports.Report.Margins PageMargins
        {
            get
            {
                return this.section.PageMargins;
            }
            set
            {
                this.section.PageMargins = value;
            }
        }

        public Infragistics.Documents.Reports.Report.Section.PageNumbering PageNumbering
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public Infragistics.Documents.Reports.Report.PageOrientation PageOrientation
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public Infragistics.Documents.Reports.Report.Paddings PagePaddings
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public Infragistics.Documents.Reports.Report.PageSize PageSize
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                this.section.PageSize = new PageSize(880, value.Height);
            }
        }

        public Infragistics.Documents.Reports.Report.Report Parent
        {
            get { return section.Parent; }
        }

        public IEnumerable Content
        {
            get { throw new NotImplementedException(); }
        }
        
        #endregion
    }
}

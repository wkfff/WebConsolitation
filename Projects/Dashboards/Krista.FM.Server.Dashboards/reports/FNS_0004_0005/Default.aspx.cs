using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Index;
using Infragistics.Documents.Reports.Report.TOC;
using Infragistics.Documents.Reports.Report.Tree;
using Infragistics.Documents.Reports.Report.Flow;
using Infragistics.Documents.Reports.Report.Grid;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.QuickTable;
using Infragistics.Documents.Reports.Report.QuickList;
using Infragistics.Documents.Reports.Report.QuickText;
using Infragistics.Documents.Reports.Report.Segment;
using Infragistics.Documents.Reports.Report.Band;
using System.IO;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using InitializeRowEventHandler = Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler;
using SerializationFormat = Dundas.Maps.WebControl.SerializationFormat;
using System.Globalization;
using Infragistics.Documents.Reports.Report.List;
using System.Drawing;
using System;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report.Tree;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Krista.FM.Server.Dashboards.Components;
using System.Globalization;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Band;
using Infragistics.Documents.Reports.Report.Flow;
using Infragistics.Documents.Reports.Report.Grid;
using Infragistics.Documents.Reports.Report.Index;
using Infragistics.Documents.Reports.Report.List;
using Infragistics.Documents.Reports.Report.QuickList;
using Infragistics.Documents.Reports.Report.QuickTable;
using Infragistics.Documents.Reports.Report.QuickText;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Segment;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.TOC;
using System.Collections;
using Infragistics.UltraChart.Shared.Styles;
using System.Collections.Generic;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Core;
using System.Collections.ObjectModel;

namespace Krista.FM.Server.Dashboards.reports.FNS_0004_0005
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtComments;
        private CustomParam Faces;
        private CustomParam Units;
        private CustomParam Year;
        private CustomParam month;
        private CustomParam mul;
        private DataTable candleDT;
        private DataTable chart1DT;
        private Dictionary<DateTime, string> candleLabelsDictionary;
        private Dictionary<DateTime, string> candleLabelsDictionary1;
        private DateTime currDateTime;
        private DateTime lastDateTime;
        private CustomParam ufo;
        private CustomParam currYear;
        private CustomParam years;
        private CustomParam monthlab;
        // Текущая дата
        private CustomParam periodCurrentDate;
        // На неделю назад
        private CustomParam periodLastWeekDate;
        private int endYear = 2009;
        private string endmonth = "Декабрь";
        private int indexOfRow = 0;
        private Collection<string> incomes = new Collection<string>();
        /// <summary>
        /// Выбраны ли
        /// федеральные округа
        /// </summary>
        ///
        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1200; }
        }

        private int minScreenWidth
        {
            get { return IsSmallResolution ? 850 : CustomReportConst.minScreenWidth; }
        }

        private int minScreenHeight
        {
            get { return IsSmallResolution ? 700 : CustomReportConst.minScreenHeight; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            int start = Environment.TickCount;
            base.Page_PreLoad(sender, e);
            CRHelper.SaveToUserAgentLog(String.Format("Базовый прелоад {0}", Environment.TickCount - start));
            base.Page_PreLoad(sender, e);
            if (periodCurrentDate == null)
            {
                periodCurrentDate = UserParams.CustomParam("period_current_date");
            }
            if (periodLastWeekDate == null)
            {
                periodLastWeekDate = UserParams.CustomParam("period_last_week_date");
            }
            start = Environment.TickCount;
            if (Faces == null)
            {
                Faces = UserParams.CustomParam("Faces");
            }
            if (monthlab == null)
            {
                monthlab = UserParams.CustomParam("monthlab");
            }
            if (Year == null)
            {
                Year = UserParams.CustomParam("Year");
            }
            if (month == null)
            {
                month = UserParams.CustomParam("month");
            }
            if (periodCurrentDate == null)
            {
                periodCurrentDate = UserParams.CustomParam("period_current_date");
            }
            if (currYear == null)
            {
                currYear = UserParams.CustomParam("currYear");
            }
            if (mul == null)
            {
                mul = UserParams.CustomParam("mul");
            }
            if (years == null)
            {
                years = UserParams.CustomParam("years");
            }
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.17);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.23);
            UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid1_DataBound);

            UltraGridExporter1.Visible = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
            <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
            <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);

            CRHelper.SaveToUserAgentLog(String.Format("Остальной прелоад {0}", Environment.TickCount - start));
            UltraGridExporter1.MultiHeader = false;
            UltraGridExporter1.HeaderChildCellHeight = 100;
        }

        void UltraChart_InterpolateValues(object sender, InterpolateValuesEventArgs e)
        {

        }

        string meas = string.Empty;
        protected override void Page_Load(object sender, EventArgs e)
        {

            int start = Environment.TickCount;
            base.Page_Load(sender, e);
            CRHelper.SaveToUserAgentLog(String.Format("Базовый лоад {0}", Environment.TickCount - start));
            start = Environment.TickCount;

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0004_0005_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            endYear = Convert.ToInt32(dtDate.Rows[0][0]);
            endmonth = dtDate.Rows[0][3].ToString();
            int firstYear = 2006;
            if (!Page.IsPostBack)
            {
                ComboYear.Width = 150;
                ComboYear.Title = "Годы";
                ComboYear.MultiSelect = true;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetAllСheckedState(true, true);

                ComboIncomes.Width = 650;
                ComboIncomes.Title = "Доходный источник";
                ComboIncomes.MultiSelect = false;
                ComboIncomes.TooltipVisibility = TooltipVisibilityMode.Shown;
                FillIncomes();
                ComboIncomes.FillValues(incomes);
                ComboIncomes.SelectLastNode();
            }
            string faces = string.Empty;
            RadioList.Visible = false;
            if (RadioList.SelectedIndex == 0)
            {
                mul.Value = Convert.ToString(1000);
                meas = "тыс.руб";
            }
            else
            {
                mul.Value = Convert.ToString(1000000);
                meas = "млн.руб";
            }
            Collection<string> selectedValues = ComboYear.SelectedValues;
            if (selectedValues.Count > 0)
            {
                string gridDescendants = String.Empty;
                string chartDescendants = String.Empty;
                years.Value = String.Empty;
                for (int i = 0; i < selectedValues.Count; i++)
                {
                    string year = selectedValues[i];
                    string sign = string.Empty;
                    if (i != selectedValues.Count - 1)
                    {
                        sign = ",";
                    }
                    years.Value += string.Format("{1}.[Данные всех периодов].[{0}]{2}",
                    year, "[Период].[Период]", sign);
                }
            }
            currDateTime = new DateTime(endYear, CRHelper.MonthNum(endmonth), 01);
            currDateTime = currDateTime.AddMonths(1);
            string inc = string.Empty;
            switch (ComboIncomes.SelectedValue)
            {
                case "Земельный налог по юридическим лицам":
                    {
                        inc = "Земельный налог по юридическим лицам (форма 5-МН)";
                        break;
                    }
                case "Земельный налог по физическим лицам":
                    {
                        inc = "Земельный налог по физическим лицам (форма 5-МН)";
                        break;
                    }
                case "Налог на имущество физических лиц":
                    {
                        inc = "Налог на имущество физических лиц (форма 5-МН)";
                        break;
                    }
                case "Налог на имущество организаций":
                    {
                        inc = "Налог на имущество организаций (форма 5-НИО)";
                        break;
                    }
                case "Транспортный налог":
                    {
                        inc = "Транспортный налог (форма 5-ТН)";
                        break;
                    }
                case "Налог на прибыль, зачисляемый в бюджет субъекта Российской Федерации":
                    {
                        inc = "Налог на прибыль, зачисляемый в бюджет субъекта Российской Федерации (форма 5-ПМ)";
                        break;
                    }
                case "Налог, уплачиваемый в связи с применением упрощенной системы налогообложения":
                    {
                        inc = "Налог, уплачиваемый в связи с применением упрощенной системы налогообложения (форма 5-УСН)";
                        break;
                    }
                case "Налог на доходы физических лиц, удерживаемый налоговыми агентами":
                    {
                        inc = "Налог на доходы физических лиц, удерживаемый налоговыми агентами (форма 5-НДФЛ)";
                        break;
                    }
                case "Единый сельскохозяйственный налог":
                    {
                        inc = "Единый сельскохозяйственный налог (форма 5-ЕСХН)";
                        break;
                    }
                case "Единый налог на вмененный доход для отдельных видов деятельности":
                    {
                        inc = "Единый налог на вмененный доход для отдельных видов деятельности (форма 5-ЕНВД)";
                        break;
                    }
            }
            Page.Title = "Анализ данных форм статистической налоговой отчетности";
            Label1.Text = String.Format("Анализ данных форм статистической налоговой отчетности ({0})", inc);
            if (selectedValues.Count > 0)
            {
                if (selectedValues.Count == 1)
                {
                    Label2.Text = String.Format("за {0} год", selectedValues[selectedValues.Count - 1]);
                }
                else
                {
                    Label2.Text = String.Format("за {0} - {1} гг.", selectedValues[0], selectedValues[selectedValues.Count - 1]);
                }
            }
            grid_Caption.Text = String.Format("Динамический анализ показателя по доходному источнику {0}, тыс.руб.", inc);
            UserParams.PeriodYear.Value = "2008";
            int defaultRowIndex = (CRHelper.MonthNum(endmonth) - 1) * 2;
            UltraWebGrid.DataBind();
            UltraWebGrid1.DataBind();
        }

        #region Обработчики грида

        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {

        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            int start = Environment.TickCount;
            string nameQuery = String.Format("FNS_0004_0005_compare_Grid_{0}", ComboIncomes.SelectedIndex + 1);
            string query = DataProvider.GetQueryText(nameQuery);
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателя", dtGrid);
            UltraWebGrid.DataSource = dtGrid;
        }

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            int start = Environment.TickCount;
            string nameQuery = String.Format("FNS_0004_0005_compare_Grid1");
            string query = DataProvider.GetQueryText(nameQuery);
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование доходного источника", dtGrid);
            UltraWebGrid1.DataSource = dtGrid;
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {

        }

        void UltraWebGrid1_DataBound(object sender, EventArgs e)
        {

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
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(110);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].AllowSorting = AllowSorting.No;
            if (e.Layout.Bands[0].Columns.Count > 1)
            {
                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    if (i == 0)
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 1;
                    }
                    else
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                        e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(800 / UltraWebGrid.Columns.Count);
                    }
                    string formatString = "N2";
                    e.Layout.Bands[0].Columns[i].Format = formatString;
                    if (i > 0)
                    {
                        if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("Значение"))
                        {
                            e.Layout.Bands[0].Columns[i].Header.Caption = e.Layout.Bands[0].Columns[i].Header.Caption.Substring(0, 4);
                            e.Layout.Bands[0].Columns[i].Header.Caption = String.Format("{0}{1}", e.Layout.Bands[0].Columns[i].Header.Caption, " год");
                        }
                        else if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("роста"))
                        {
                            e.Layout.Bands[0].Columns[i].Header.Caption = String.Format("Темп роста\n({0}/{1}), %", e.Layout.Bands[0].Columns[i - 1].Header.Caption.Split(' ')[0], Convert.ToInt32(e.Layout.Bands[0].Columns[i - 1].Header.Caption.Split(' ')[0]) - 1);
                        } 
                    }
                }
                int count = e.Layout.Bands[0].Columns.Count;
                e.Layout.Bands[0].Columns[0].Hidden = false;
                e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[1].CellStyle.Padding.Right = 5;
                for (int k = 1; k < e.Layout.Bands[0].Columns.Count; k++)
                {
                    e.Layout.Bands[0].Columns[count - k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    e.Layout.Bands[0].Columns[count - k].CellStyle.Padding.Right = 5;
                }
                Collection<string> cellsCaption = new Collection<string>();
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            }
        }


        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(110);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].AllowSorting = AllowSorting.No;
            if (e.Layout.Bands[0].Columns.Count > 1)
            {
                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    if (i == 0)
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 1;
                    }
                    else
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                        e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(800 / UltraWebGrid.Columns.Count);
                    }
                    string formatString = "N2";
                    e.Layout.Bands[0].Columns[i].Format = formatString;
                    if (i > 0)
                    {
                        if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("Значение за период"))
                        {
                            e.Layout.Bands[0].Columns[i].Header.Caption = e.Layout.Bands[0].Columns[i].Header.Caption.Substring(0, 4);
                            e.Layout.Bands[0].Columns[i].Header.Caption = String.Format("{0}{1}", e.Layout.Bands[0].Columns[i].Header.Caption, " год");
                        }
                        else
                        {
                            if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("Удельный вес"))
                            {
                                e.Layout.Bands[0].Columns[i].Header.Caption = "Удельный вес, %";
                            }
                            if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("Ранг"))
                            {
                                e.Layout.Bands[0].Columns[i].Header.Caption = "Ранг";
                                e.Layout.Bands[0].Columns[i].Format = "0";
                            }
                        }
                    }
                }
                int count = e.Layout.Bands[0].Columns.Count;
                e.Layout.Bands[0].Columns[0].Hidden = false;
                e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[1].CellStyle.Padding.Right = 5;
                for (int k = 1; k < e.Layout.Bands[0].Columns.Count; k++)
                {
                    e.Layout.Bands[0].Columns[count - k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    e.Layout.Bands[0].Columns[count - k].CellStyle.Padding.Right = 5;
                }
                Collection<string> cellsCaption = new Collection<string>();
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int k = 0;
            for (int i = 2; i < UltraWebGrid.Columns.Count; i++)
            {
                if (UltraWebGrid.Columns[i].Header.Caption.Contains("%"))
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        int value = Convert.ToInt32(e.Row.Cells[i].Value);
                        if (value < 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                            e.Row.Cells[i].Title = "Снижение показателя к аналогичному периоду прошлого года";
                            e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        }
                        else
                        {

                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "Рост показателя к аналогичному периоду прошлого года";
                            e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        }
                    }

                }
            }

        }

        protected void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            int lastRowIndex = 6;
            for (int i = 2; i < UltraWebGrid1.Columns.Count; i++)
            {
                if (UltraWebGrid1.Columns[i].Header.Caption.Contains("Ранг"))
                {
                    CRHelper.SaveToQueryLog("курсор:" + Convert.ToInt32(e.Row.Cells[i].Value));
                    CRHelper.SaveToQueryLog("худшийранг:" + Convert.ToInt32(UltraWebGrid1.Rows[UltraWebGrid1.Rows.Count - 1].Cells[i - 2].Value));
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = "Наибольший удельный вес в общем объеме предоставленных льгот";
                        }

                        else
                            if (Convert.ToInt32(e.Row.Cells[i].Value) == Convert.ToInt32(dtGrid.Rows[dtGrid.Rows.Count - 1][i - 2]))
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                                e.Row.Cells[i].Title = "Наименьший удельный вес в общем объеме предоставленных льгот";
                            }
                        e.Row.Cells[i].Style.CustomRules =
                        "background-repeat: no-repeat; background-position: left center; margin: 2px";

                    }
                }
            }
            if (e.Row.Index == dtGrid.Rows.Count - 1)
            {
                e.Row.Hidden = true;
            }

        }
        protected void FillIncomes()
        {
            incomes.Add("Земельный налог по юридическим лицам");
            incomes.Add("Земельный налог по физическим лицам");
            incomes.Add("Налог на имущество физических лиц");
            incomes.Add("Налог на имущество организаций");
            incomes.Add("Транспортный налог");
            incomes.Add("Налог на прибыль, зачисляемый в бюджет субъекта Российской Федерации");
            incomes.Add("Налог, уплачиваемый в связи с применением упрощенной системы налогообложения");
            incomes.Add("Налог на доходы физических лиц, удерживаемый налоговыми агентами");
            incomes.Add("Единый сельскохозяйственный налог");
            incomes.Add("Единый налог на вмененный доход для отдельных видов деятельности");
        }

        #endregion

        #region Обработчики диаграмы
        DataTable dtChart = new DataTable();


        void UltraChart_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {

        }
        #endregion

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }

        protected void UltraWebGrid1_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }
        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            ActiveGridRow(UltraWebGrid.Rows[3]);
            e.CurrentWorksheet.Rows[0].Cells[0].Value = String.Format("{0} {1}", Page.Title, Label2.Text);

        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {

        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {

            int columnCount = UltraWebGrid.Columns.Count;
            int width = 300;
            e.CurrentWorksheet.Rows[7].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.CurrentWorksheet.Columns[0].Width = width * 17;
            e.CurrentWorksheet.Columns[1].Width = width * 17;
            e.CurrentWorksheet.Columns[2].Width = width * 17;
            e.CurrentWorksheet.Columns[3].Width = width * 17;
            e.CurrentWorksheet.Columns[4].Width = width * 17;
            e.CurrentWorksheet.Columns[5].Width = width * 17;
            e.CurrentWorksheet.Columns[6].Width = width * 17;
            e.CurrentWorksheet.Columns[7].Width = width * 17;
            e.CurrentWorksheet.Columns[8].Width = width * 17;
            e.CurrentWorksheet.Columns[9].Width = width * 17;
            e.CurrentWorksheet.Columns[10].Width = width * 17;
            e.CurrentWorksheet.Columns[11].Width = width * 17;
            e.CurrentWorksheet.Columns[12].Width = width * 17;
            e.CurrentWorksheet.Columns[13].Width = width * 17;
            e.CurrentWorksheet.Columns[14].Width = width * 17;
            e.CurrentWorksheet.Columns[15].Width = width * 17;
            e.CurrentWorksheet.Columns[16].Width = width * 17;
            int columnCountt = UltraWebGrid.Columns.Count;
            int columnCounttt = UltraWebGrid.Columns.Count;
            for (int i = 1; i < columnCount; i = i + 1)
            {
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00";
                e.CurrentWorksheet.Columns[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
            }
        }



        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Исп. бюджета");
            Worksheet sheet2 = workbook.Worksheets.Add("Стр. анализ");
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid1, sheet2);
        }

        protected void RadioList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #endregion

        #region Экспорт в PDF
        ReportSection section1 = null;
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            Report report = new Report();
            section1 = new ReportSection(report, false);
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid, section1);
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid1, section1);
            string label = Label2.Text.Replace("<br/>", "");
            if (IsExported) return;
            IsExported = true;
            IText title = section1.AddText();
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 16);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

        }
        bool IsExported = false;
        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {

        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {



        }

        #endregion

        public class ReportSection : ISection
        {
            private readonly bool withFlowColumns;
            private readonly ISection section;
            private IFlow flow;
            private ITableCell titleCell;

            public ReportSection(Report report, bool withFlowColumns)
            {
                section = report.AddSection();
                ITable table = section.AddTable();
                ITableRow row = table.AddRow();
                titleCell = row.AddCell();
            }

            public void AddFlowColumnBreak()
            {
                if (flow != null)
                    flow.AddColumnBreak();
            }

            public Infragistics.Documents.Reports.Report.ContentAlignment PageAlignment
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public IBand AddBand()
            {
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

            public IImage AddImage(Infragistics.Documents.Reports.Graphics.Image image)
            {
                if (flow != null)
                    return flow.AddImage(image);
                return this.section.AddImage(image);
            }

            public IMetafile AddMetafile(Infragistics.Documents.Reports.Graphics.Metafile metafile)
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
                set { this.section.PageSize = new PageSize(2560, 1350); }
            }

            public PageOrientation PageOrientation
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
}
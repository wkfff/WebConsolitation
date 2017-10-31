using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using System.Drawing;
using Infragistics.UltraChart.Shared.Styles;
using System.Collections.Generic;
using System.IO;

namespace Krista.FM.Server.Dashboards.reports.STAT_0002_0005
{
    public enum SliceType
    {
        OKVED,
        OKOPF,
        OKFS
    }

    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable chartDt = new DataTable();
        private DataTable gridDt = new DataTable();
        private DateTime currentDate;
        private static MemberAttributesDigest periodDigest;
        private int firstYear = 2000;
        private CustomParam currentPeriod;
        private CustomParam lastPeriod;
        private CustomParam Finance;
        private CustomParam kind;
        private GridHeaderLayout headerLayout;
        private UltraGridRow gridrow;
        #endregion

        #region Параметры запроса

        // множество для среза данных
        private CustomParam sliceSet;
        private CustomParam period;
        private CustomParam lastmonth;
        private CustomParam lastquart;
        private CustomParam datasources;
        private CustomParam rows;

        #endregion


        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида
            UltraWebGrid.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.8 - 235);
            UltraWebGrid.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            UltraWebGrid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);
            UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(Grid_ActiveRowChange);
            UltraWebGrid.DataBinding += new System.EventHandler(UltraWebGrid_DataBinding);

            #endregion

            #region Настройка диаграммы динамики

            UltraChart1.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            UltraChart1.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.65 - 100);
            UltraChart1.ChartType = ChartType.SplineAreaChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.Y.Extent = 50;
            UltraChart1.Axis.X.Extent = 80;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.SpanPercentage = 15;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Axis.X.Labels.Font = new Font("Verdana", 9);
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 9);
            UltraChart1.Axis.X.Labels.WrapText = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.FontColor = Color.Black;
            UltraChart1.Axis.X.Labels.FontColor = Color.Black;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 9);
            UltraChart1.Axis.Y.Labels.FontColor = Color.Black;
            UltraChart1.Axis.X.Labels.Visible = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Data.SwapRowsAndColumns = false;
            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Font = new Font("Verdana", 8);
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
            UltraChart2.Height = CRHelper.GetChartHeight(350);
            UltraChart2.ChartType = ChartType.ColumnChart;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.Axis.Y.Extent = 50;
            UltraChart2.Axis.X.Extent = 23;
            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL> \nв <SERIES_LABEL> году\n<b><DATA_VALUE:N2></b> млн.руб.";
            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.SpanPercentage = 12;
            UltraChart2.Legend.Location = LegendLocation.Bottom;
            UltraChart2.Axis.X.Labels.Font = new Font("Verdana", 07);
            UltraChart2.Axis.X.Labels.WrapText = true;
            UltraChart2.Axis.X.Labels.SeriesLabels.FontColor = Color.Black;
            UltraChart2.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 12);
            UltraChart2.Axis.Y.Labels.FontColor = Color.Black;
            UltraChart2.Axis.X.Labels.Visible = false;
            UltraChart2.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart2.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart2.Data.SwapRowsAndColumns = false;
            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.Font = new Font("Verdana", 8);
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion

            #region Инициализация параметров запроса

            currentPeriod = UserParams.CustomParam("current_period");
            lastPeriod = UserParams.CustomParam("last_period");
            Finance = UserParams.CustomParam("finance");
            kind = UserParams.CustomParam("kind");
            period = UserParams.CustomParam("period");
            lastmonth = UserParams.CustomParam("lastmonth");
            lastquart = UserParams.CustomParam("lastquart");
            datasources = UserParams.CustomParam("data_sources");
            rows = UserParams.CustomParam("rows");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                DateTime lastDate = CubeInfo.GetLastDate(DataProvidersFactory.SpareMASDataProvider, "STAT_0002_0005_lastDate");

                ComboYear.PanelHeaderTitle = "Выберите период";
                ComboYear.Title = "Выберите период";
                ComboYear.Width = 290;
                ComboYear.ParentSelect = true;
                ComboYear.ShowSelectedValue = true;
                ComboYear.MultiSelect = false;
                periodDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "STAT_0002_0005_Date");
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodDigest.UniqueNames, periodDigest.MemberLevels));
                ComboYear.SetСheckedState("2012 год", true);

                chartWebAsyncPanel.AddRefreshTarget(UltraChart1);
                chartWebAsyncPanel.AddRefreshTarget(ChartCaption1);
                chartWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid.ClientID);
                radioGroupPanel.AddRefreshTarget(UltraChart2);
                radioGroupPanel.AddRefreshTarget(ChartCaption2);
                radioGroupPanel.AddLinkedRequestTrigger(KindList.ClientID);
            }
            string periodUniqueName = string.Empty;
            int yearNum = firstYear;
            switch (ComboYear.SelectedNode.Level)
            {
                case 0:
                    {
                        periodUniqueName = periodDigest.UniqueNames[ComboYear.GetLastNode(1).Text];
                        yearNum = Convert.ToInt32(ComboYear.SelectedValue.ToString().Split(' ')[0]);
                        period.Value = "Данные года";
                        datasources.Value = "[slicer]";
                        rows.Value = "rows_years";
                        UltraChart2.Visible = true;
                        ChartCaption2.Visible = true;
                        KindList.Visible = true;
                        DocLink.Visible = true;
                        lastmonth.Value = String.Format("{0}.[Полугодие 1].[Квартал 1].[Январь]", periodUniqueName);
                        lastquart.Value = String.Format("{0}.[Полугодие 1].[Квартал 1]", periodUniqueName);
                        break;
                    }
                case 1:
                    {
                        
                        periodUniqueName = periodDigest.UniqueNames[ComboYear.SelectedValue];
                        yearNum = Convert.ToInt32(ComboYear.SelectedValue.ToString().Split(' ')[2]);
                        period.Value = "Кварталы";
                        datasources.Value = "[Все источники данных].[СТАТ Отчетность - СТАТ Бюллетень ЦБ Тюменской]";
                        rows.Value = "rows_quarts";
                        UltraChart2.Visible = false;
                        ChartCaption2.Visible = false;
                        Label2.Text = String.Format("Анализ динамики основных показателей банковской деятельности, Ханты-Мансийский автономный округ-Югра, по состоянию на {0}", ComboYear.SelectedValue.ToString().ToLower());
                        KindList.Visible = false;
                        DocLink.Visible = false;
                        lastmonth.Value = String.Format("{0}.[Январь]", periodUniqueName);
                        lastquart.Value = String.Format("{0}", periodUniqueName);
                        CRHelper.SaveToErrorLog("уровень" + ComboYear.SelectedNode.Level.ToString());
                        break;
                    }
                case 2:
                    {
                        periodUniqueName = StringToMDXDate(ComboYear.SelectedValue);
                        yearNum = Convert.ToInt32(ComboYear.SelectedValue.ToString().Split(' ')[1]);
                        period.Value = "Месяцы";
                        datasources.Value = "[Все источники данных].[СТАТ Отчетность - СТАТ Бюллетень ЦБ Тюменской]";
                        rows.Value = "rows_months";
                        UltraChart2.Visible = false;
                        ChartCaption2.Visible = false;
                        Label2.Text = String.Format("Анализ динамики основных показателей банковской деятельности, Ханты-Мансийский автономный округ-Югра, по состоянию на {0}", ComboYear.SelectedValue.ToString().ToLower());
                        KindList.Visible = false;
                        DocLink.Visible = false;
                        lastmonth.Value = String.Format("{0}", periodUniqueName);
                        lastquart.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[2020].[Полугодие 1].[Квартал 1]";
                        break;
                    }
            }
            UserParams.PeriodYear.Value = yearNum.ToString();
            Page.Title = String.Format("Развитие банковской деятельности в регионе");
            Label1.Text = Page.Title;
            DocLink.Text = "Информация по кредитным организациям ХМАО-Югры";
            if (File.Exists(String.Format("//Documents/Информация_по_кредитным_организациям_ХМАО-Югры_за_{0}_год.doc", yearNum)))
            {
                DocLink.NavigateUrl = String.Format("Информация_по_кредитным_организациям_ХМАО-Югры_за_{0}_год.doc", yearNum);
            }

            else
            {
                DocLink.NavigateUrl = String.Format("Информация_по_кредитным_организациям_ХМАО-Югры_за_{0}_год.doc", 2011);
            }
            ChartCaption1.Text = String.Format("");
            ChartCaption2.Text = String.Format("Соотношение объемов вкладов физических лиц в СБРФ и коммерческие банки, млн.руб.");
            UserParams.PeriodLastYear.Value = (currentDate.Year - 1).ToString();
            kind.Value = KindList.SelectedValue;
            switch (kind.Value)
            {
                case "Общий объем вкладов":
                    {
                        ChartCaption2.Text = String.Format("Соотношение объемов вкладов физических лиц в СБРФ и коммерческие банки, млн.руб.");
                        UltraChart2.Tooltips.FormatString = "<ITEM_LABEL> \nв <SERIES_LABEL> году\n<b><DATA_VALUE:N2></b> млн.руб.";
                        break;
                    }
                case "Общий объем вкладов на душу населения":
                    {
                        ChartCaption2.Text = String.Format("Соотношение объемов вкладов на душу населения физических лиц в СБРФ и коммерческие банки, руб.");
                        UltraChart2.Tooltips.FormatString = "<ITEM_LABEL> \nв <SERIES_LABEL> году\n<b><DATA_VALUE:N2></b> руб.";
                        break;
                    }
            }
            if ((!radioGroupPanel.IsAsyncPostBack) && (!chartWebAsyncPanel.IsAsyncPostBack))
            {
                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();
                string patternValue = string.Empty;
                int defaultRowIndex = 0;
                patternValue = Finance.Value;
                UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, UltraWebGrid.Columns.Count - 1, defaultRowIndex);
                ActiveGridRow(row);
            }
            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                ChartDataBind2();
            }

        }

        #region Обработчики грида

        private void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0002_0005_grid");
            gridDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                if (gridDt.Columns.Count > 1)
                {
                    gridDt.Columns.RemoveAt(0);
                }
                UltraWebGrid.DataSource = gridDt;
            }
        }

        private void ActiveGridRow(UltraGridRow row)
        {
            if (row == null)
                return;
            switch (row.Index % 3)
            {
                case 0:
                    {
                        Finance.Value = row.Cells[row.Cells.Count - 1].Value.ToString();
                        break;
                    }
                case 1:
                    {
                        Finance.Value = row.PrevRow.Cells[row.Cells.Count - 1].Value.ToString();
                        break;
                    }
                case 2:
                    {
                        Finance.Value = row.PrevRow.PrevRow.Cells[row.Cells.Count - 1].Value.ToString();
                        break;
                    }
            }
            string yearlabel = string.Empty;
            switch (ComboYear.SelectedNode.Level)
            {
                case 0:
                    {
                        yearlabel = "год";
                        break;
                    }
                case 1:
                    {
                        yearlabel = "года";
                        break;
                    }
            }
            if (row.Cells[0].Value.ToString().Contains("всего") || row.Cells[0].Value.ToString().Contains("Число отделений (филиалов) действующих кредитных организаций,") ||
            row.Cells[0].Value.ToString().Contains("Задолженность по кредитам, выданным юридическим и физическим лицам,") || row.Cells[0].Value.ToString().Contains("Задолженность по кредитам, выданным юридическим и физическим лицам: по кредитам, предоставленным предприятия и организациям, миллион") ||
            row.Cells[0].Value.ToString().Contains("Пассивы кредитных организаций - всего") ||
            row.Cells[0].Value.ToString().Contains("Объемы бюджетных средств на счетах кредитных организаций,") ||
            row.Cells[0].Value.ToString().Contains("Средства организаций на счетах кредитных организаций,") ||
            row.Cells[0].Value.ToString().Contains("Объем вложений кредитных организаций в ценные бумаги,"))
            {
                if (row.Cells[row.Cells.Count - 1].Value.ToString().Contains("средства - всего"))
                {
                    ChartCaption1.Text = string.Format("Динамика объемов кредитования организаций, кредитных организаций и физических лиц, млн. рублей");
                }
                else
                    if (row.Cells[row.Cells.Count - 1].Value.ToString().Contains("кредитам - всего"))
                    {
                        ChartCaption1.Text = string.Format("Динамика объемов просроченной задолженности организаций, кредитных организаций и физических лиц, млн. рублей");
                    }
                    else
                    {
                        ChartCaption1.Text = string.Format("Динамика показателя \"{0}\", {1}", row.Cells[row.Cells.Count - 1].Value.ToString(), row.Cells[0].Value.ToString().Split(',')[row.Cells[0].Value.ToString().Split(',').Length - 1].ToString());
                    }
                UltraChart1.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomLinear;
                UltraChart1.ChartType = ChartType.StackColumnChart;
                if (row.Cells[0].Value.ToString().Contains("единица"))
                {
                    UltraChart1.Tooltips.FormatString = String.Format("<ITEM_LABEL> \nза <SERIES_LABEL> {0}\n<b><DATA_VALUE:N0></b> (единица)", yearlabel);
                }
                else
                {
                    UltraChart1.Tooltips.FormatString = String.Format("<ITEM_LABEL> \n за <SERIES_LABEL> {1}\n<b><DATA_VALUE:N1></b> ({0})", row.Cells[0].Value.ToString().Split(',')[row.Cells[0].Value.ToString().Split(',').Length - 1].ToString(), yearlabel);
                }
                UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
                UltraChart1.Data.SwapRowsAndColumns = false;
                UltraChart1.Axis.X.Labels.Visible = false;
            }
            else
            {
                switch (row.Index % 3)
                {
                    case 0:
                        {
                            ChartCaption1.Text = string.Format("Динамика показателя \"{0}\", {1}", row.Cells[row.Cells.Count - 1].Value.ToString(), row.Cells[0].Value.ToString().Split(',')[row.Cells[0].Value.ToString().Split(',').Length - 1].ToString());
                            break;
                        }
                    case 1:
                        {
                            ChartCaption1.Text = string.Format("Динамика показателя \"{0}\", {1}", row.PrevRow.Cells[row.PrevRow.Cells.Count - 1].Value.ToString(), row.PrevRow.Cells[0].Value.ToString().Split(',')[row.PrevRow.Cells[0].Value.ToString().Split(',').Length - 1].ToString());
                            break;
                        }
                    case 2:
                        {
                            ChartCaption1.Text = string.Format("Динамика показателя \"{0}\", {1}", row.PrevRow.PrevRow.Cells[row.PrevRow.PrevRow.Cells.Count - 1].Value.ToString(), row.PrevRow.PrevRow.Cells[0].Value.ToString().Split(',')[row.PrevRow.PrevRow.Cells[0].Value.ToString().Split(',').Length - 1].ToString());
                            break;
                        }
                }

                //ChartCaption1.Text = string.Format("Динамика показателя \"{0}\", {1}", row.Cells[row.Cells.Count - 1].Value.ToString(), row.Cells[0].Value.ToString().Split(',')[row.Cells[0].Value.ToString().Split(',').Length - 1].ToString());
                UltraChart1.ChartType = ChartType.SplineAreaChart;
                UltraChart1.ColorModel.ModelStyle = Infragistics.UltraChart.Shared.Styles.ColorModels.CustomSkin;
                Color color1 = Color.LightSkyBlue;
                UltraChart1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
                UltraChart1.ColorModel.Skin.ApplyRowWise = false;
                UltraChart1.Effects.Effects.Clear();
                GradientEffect effect = new GradientEffect();
                effect.Style = GradientStyle.ForwardDiagonal;
                effect.Enabled = true;
                UltraChart1.Effects.Enabled = true;
                UltraChart1.Effects.Effects.Add(effect);
                if (row.Cells[0].Value.ToString().Contains("единица"))
                {
                    UltraChart1.Tooltips.FormatString = string.Format("<SERIES_LABEL> \n за <ITEM_LABEL> {1}\n<b><DATA_VALUE:N0></b> ({0})", row.Cells[0].Value.ToString().Split(',')[row.Cells[0].Value.ToString().Split(',').Length - 1], yearlabel);
                }
                else
                {
                    UltraChart1.Tooltips.FormatString = string.Format("<SERIES_LABEL> \n за <ITEM_LABEL> {1}\n<b><DATA_VALUE:N1></b> ({0})", row.Cells[0].Value.ToString().Split(',')[row.Cells[0].Value.ToString().Split(',').Length - 1], yearlabel);
                }
                UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
                UltraChart1.Data.SwapRowsAndColumns = true;
                UltraChart1.Axis.X.Labels.Visible = true;
            }
            ChartDataBind1();
            CRHelper.FindGridRow(UltraWebGrid, Finance.Value, UltraWebGrid.Columns.Count - 1, 0);
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }
            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(340);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.Bands[0].Columns[0].MergeCells = true;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 2].Hidden = true;
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            headerLayout.AddCell("Наименование");

            string queryText = DataProvider.GetQueryText("STAT_0002_0005_header");
            DataTable headerDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(queryText, "Показатель", headerDt);

            for (int i = 1; i < columnCount - 2; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(130);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                if (ComboYear.SelectedNode.Level == 0)
                {
                    headerLayout.AddCell(String.Format("{0} год", e.Layout.Bands[0].Columns[i].Header.Caption.Split('(')[1].Split(' ')[0]));
                }
                else
                    headerLayout.AddCell(String.Format("{0}", headerDt.Rows[headerDt.Rows.Count - (columnCount - i - 2)][1]));
            }
 
            if (ComboYear.SelectedNode.Level == 0)
            {
                Label2.Text = String.Format("Анализ динамики основных показателей банковской деятельности, Ханты-Мансийский автономный округ-Югра, по состоянию на конец {0} года", headerLayout.Grid.Columns[headerLayout.Grid.Columns.Count - 3].Header.Caption.ToString().Split('(')[1].Split(' ')[0]);
            }
            headerLayout.ApplyHeaderInfo();
        }

        protected void Grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }
        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (UltraWebGrid.Rows.Count > 0)
            {
                int cellCount = e.Row.Cells.Count;
                int type = 0;
                if (e.Row.Cells[cellCount - 2].Value != null)
                {
                    type = Convert.ToInt32(e.Row.Cells[cellCount - 2].Value.ToString());
                }
                e.Row.Cells[0].Value = e.Row.Cells[0].Value.ToString().Replace(e.Row.Cells[0].Value.ToString().Split(',')[e.Row.Cells[0].Value.ToString().Split(',').Length - 1], e.Row.Cells[0].Value.ToString().Split(',')[e.Row.Cells[0].Value.ToString().Split(',').Length - 1].ToLower()); //Единицы со строчной буквы
                for (int i = 1; i < cellCount - 1; i++)
                {
                    UltraGridCell cell = e.Row.Cells[i];
                    cell.Style.Padding.Right = 3;
                    if (cell.Value != null)
                    {
                        if (e.Row.Cells[0].Value.ToString().Contains("Количество счетов вкладчиков физ. лиц в СБРФ") && (cell.Value.ToString() != string.Empty))
                        {
                            e.Row.Cells[0].Value = "Количество счетов вкладчиков физ. лиц в СБРФ, тысяча единиц";
                            cell.Value = Convert.ToDouble(cell.Value.ToString()) / 1000;
                        }
                    }
                    switch (type)
                    {
                        case 0:
                            {
                                if ((cell.Value != null) && (i != 0))
                                {
                                    if (e.Row.Cells[0].ToString().Contains("единица"))
                                    {
                                        cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N0");
                                    }
                                    else
                                    {
                                        cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N1");
                                    }
                                }

                                cell.Style.BorderDetails.WidthBottom = 0;
                                break;
                            }
                        case 1:
                            {
                                if ((cell.Value != null) && (i != 0))
                                {
                                   /* if (i == 1) // динамика первой колонки
                                    {
                                        //cell.Value = string.Empty;
                                        cell.Style.BorderDetails.WidthTop = 0;
                                        cell.Style.BorderDetails.WidthBottom = 0;
                                        break;
                                    }*/
                                    if (e.Row.Cells[0].ToString().Contains("единица"))
                                    {
                                        cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N0");
                                    }
                                    else
                                    {
                                        cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N1");
                                    }
                                }
                                cell.Style.BorderDetails.WidthTop = 0;
                                cell.Style.BorderDetails.WidthBottom = 0;
                                cell.Title = string.Format("Абсолютное отклонение к предыдущему периоду");//, Convert.ToInt32(" ") - UltraWebGrid.Columns.Count + i + 2);
                                break;
                            }
                        case 2:
                            {
                                if ((cell.Value != null) && (i != 0))
                                {
                                   /* if (i == 1)
                                    {
                                        //cell.Value = string.Empty;
                                        cell.Style.BorderDetails.WidthTop = 0;
                                        break;
                                    }*/
                                    if (e.Row.Cells[0].ToString().Contains("задолженность") || e.Row.Cells[0].ToString().Contains("Средневзвешенная ставка на"))
                                    {
                                        double growRate = Convert.ToDouble(cell.Value.ToString());
                                        cell.Value = growRate.ToString("P2");
                                        if (growRate > 0)
                                        {
                                            cell.Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                                            cell.Title = string.Format("Темп прироста к предыдущему периоду");//, Convert.ToInt32(" ") - UltraWebGrid.Columns.Count + i + 2);
                                        }
                                        else if (growRate < 0)
                                        {
                                            cell.Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                                            cell.Title = string.Format("Темп прироста к предыдущему периоду");//, Convert.ToInt32(" ") - UltraWebGrid.Columns.Count + i + 2);
                                        }
                                    }
                                    else
                                    {
                                        double growRate = Convert.ToDouble(cell.Value.ToString());
                                        cell.Value = growRate.ToString("P2");

                                        if (growRate > 0)
                                        {
                                            cell.Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                                            cell.Title = string.Format("Темп прироста к предыдущему периоду");//, Convert.ToInt32("") - UltraWebGrid.Columns.Count + i + 2);
                                        }
                                        else if (growRate < 0)
                                        {
                                            cell.Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                                            cell.Title = string.Format("Темп прироста к предыдущему периоду");//, Convert.ToInt32("") - UltraWebGrid.Columns.Count + i + 2);
                                        }
                                    }
                                    cell.Style.CustomRules = "background-repeat: no-repeat; background-position: center; margin: 2px";

                                }
                                cell.Style.BorderDetails.WidthTop = 0;
                                break;
                            }
                    }
                }

            }
        }


        #endregion

        #region Обработчики диаграммы

        private void ChartDataBind1()
        {
            string queryText = DataProvider.GetQueryText("STAT_0002_0005_chart1");
            chartDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(queryText, "Показатель", chartDt);
            if (chartDt.Rows.Count > 0)
            {
                UltraChart1.Series.Clear();
                foreach (DataRow row in chartDt.Rows)
                {
                    for (int i = 0; i < row.ItemArray.Length; i++)
                    {
                        if (i == 0 && row[i] != DBNull.Value)
                        {
                            row[i] = row[i].ToString().Replace("\"", "'");
                        }
                    }
                    if (row[0].ToString().Contains("Количество счетов вкладчиков физ. лиц в СБРФ"))
                    {
                        for (int i = 1; i < chartDt.Columns.Count; i++)
                        {
                            row[i] = (Convert.ToDouble(row[i]) / 1000).ToString();
                        }
                    }
                }

                string query = DataProvider.GetQueryText("STAT_0002_0005_chartheader");
                DataTable headerDt = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", headerDt);

                for (int i = 1; i < chartDt.Columns.Count; i++)
                {
                    if (ComboYear.SelectedNode.Level == 0)
                    {
                        chartDt.Columns[i].ColumnName = chartDt.Columns[i].ColumnName.ToString().Split('(')[1].Split(' ')[0].ToLower();
                    }
                    else
                    {
                        if (ComboYear.SelectedNode.Level == 1)
                        {
                            chartDt.Columns[i].ColumnName = headerDt.Rows[i - 1][1].ToString().Replace("года", " ");
                            chartDt.Columns[i].ColumnName = String.Format("{0} {1}\n{2}", chartDt.Columns[i].ColumnName.Split(' ')[0], chartDt.Columns[i].ColumnName.Split(' ')[1], chartDt.Columns[i].ColumnName.Split(' ')[2]);
                        }
                        else
                        {
                            chartDt.Columns[i].ColumnName = headerDt.Rows[i - 1][1].ToString().Replace("года", " ");
                            chartDt.Columns[i].ColumnName = String.Format("{0}\n{1}", chartDt.Columns[i].ColumnName.Split(' ')[0], chartDt.Columns[i].ColumnName.Split(' ')[1]);
                        }
                    }
                    NumericSeries series = CRHelper.GetNumericSeries(i, chartDt);
                    series.Label = chartDt.Columns[i].ColumnName;
                    UltraChart1.Series.Add(series);
                }
            }

        }

        private void ChartDataBind2()
        {

            string queryText = DataProvider.GetQueryText("STAT_0002_0005_chart2");
            chartDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(queryText, "Показатель", chartDt);
            if (chartDt.Rows.Count > 0)
            {
                UltraChart2.Series.Clear();
                foreach (DataRow row in chartDt.Rows)
                {
                    for (int i = 0; i < row.ItemArray.Length; i++)
                    {
                        if (i == 0 && row[i] != DBNull.Value)
                        {
                            row[i] = row[i].ToString().Replace("вкладов", "вкл.");
                            row[i] = row[i].ToString().Replace("депозитов", "депоз.");
                            row[i] = row[i].ToString().Replace("коммерческие", "ком.");
                            row[i] = row[i].ToString().Replace("\"", "'");
                            row[i] = row[i].ToString().Replace(" район", " р-н");
                        }
                    }
                }

                for (int i = 1; i < chartDt.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, chartDt);
                    series.Label = chartDt.Columns[i].ColumnName;
                    UltraChart2.Series.Add(series);
                }
            }
        }

        protected void FillComboDate(CustomMultiCombo combo, string queryName, int offset)
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            if (dtDate.Rows.Count == 0)
            {
                throw new Exception("Данные для построения отчета отсутствуют в кубе");
            }
            Dictionary<string, int> dictDate = new Dictionary<string, int>();
            for (int row = 0; row < dtDate.Rows.Count - offset; ++row)
            {
                string year = dtDate.Rows[row][0].ToString();
                string month = dtDate.Rows[row][3].ToString();
                // string day = dtDate.Rows[row][4].ToString();
                AddPairToDictionary(dictDate, year + " год", 0);
                AddPairToDictionary(dictDate, month + " " + year + " года", 1);
                //AddPairToDictionary(dictDate, day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года", 2);
            }
            combo.FillDictionaryValues(dictDate);
            combo.SelectLastNode();
        }

        protected void AddPairToDictionary(Dictionary<string, int> dict, string key, int value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }
        }

        public string StringToMDXDate(string str)
        {
            string template = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}]";
            string[] dateElements = str.Split(' ');
            int year = Convert.ToInt32(dateElements[1]);
            string month = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(CRHelper.MonthNum(dateElements[0])));
            int quarter = CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month));
            int halfYear = CRHelper.HalfYearNumByQuarterNum(quarter);
            //int day = Convert.ToInt32(dateElements[0]);
            return String.Format(template, year, halfYear, quarter, month);
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;
            SetExportGridParams(headerLayout.Grid);
            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            sheet1.Rows[3].Cells[8].Value = String.Empty;
            sheet1.Rows[4].Cells[8].Value = String.Empty;
            sheet1.Rows[3].Cells[8].CellFormat.FillPattern = FillPatternStyle.None;
            sheet1.Rows[3].Cells[8].CellFormat.BottomBorderColor = Color.LightGray;
            sheet1.Rows[3].Cells[8].CellFormat.LeftBorderColor = Color.LightGray;
            sheet1.Rows[3].Cells[8].CellFormat.RightBorderColor = Color.LightGray;
            sheet1.Rows[3].Cells[8].CellFormat.TopBorderColor = Color.LightGray;
            sheet1.Rows[4].Cells[8].CellFormat.BottomBorderColor = Color.LightGray;
            sheet1.Rows[4].Cells[8].CellFormat.TopBorderColor = Color.LightGray;
            sheet1.Rows[4].Cells[8].CellFormat.RightBorderColor = Color.LightGray;
            sheet1.Rows[4].Cells[8].CellFormat.LeftBorderColor = Color.LightGray;
            sheet1.Rows[4].Cells[8].CellFormat.FillPattern = FillPatternStyle.None;
            sheet1.Rows[2].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
            sheet1.Rows[2].CellFormat.Font.Name = "Verdana";
            for (int i = 0; i < UltraWebGrid.Columns.Count; i++)
            {
                if (i > 1)
                {
                    sheet1.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                }
                sheet1.Rows[4].Cells[i].CellFormat.Font.Name = "Verdana";
                sheet1.Rows[4].Cells[i].CellFormat.Font.Height = 200;
            }
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма1");
            UltraChart1.Width = Convert.ToInt32(UltraChart1.Width.Value * 0.7);
            UltraChart2.Width = Convert.ToInt32(UltraChart2.Width.Value * 0.7);
            ReportExcelExporter1.Export(UltraChart1, ChartCaption1.Text, sheet2, 3);
            Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма2");
            ReportExcelExporter1.Export(UltraChart2, ChartCaption2.Text, sheet3, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void ExportGridSetup()
        {
            for (int i = 0; i < UltraWebGrid.Rows.Count; i++)
            {
                UltraGridCell cell = UltraWebGrid.Rows[i].Cells[0];

                int groupIndex = i % 3;

                switch (groupIndex)
                {
                    case 0:
                        {
                            cell.Value = String.Empty;
                            cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
                            break;
                        }
                    case 1:
                        {
                            cell.Style.BorderDetails.StyleTop = BorderStyle.None;
                            cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
                            break;
                        }
                    case 2:
                        {
                            cell.Value = String.Empty;
                            cell.Style.BorderDetails.StyleTop = BorderStyle.None;
                            break;
                        }
                }
            }
        }
        private void SetExportGridParams(UltraWebGrid grid)
        {
            string exportFontName = "Verdana";
            int fontSize = 10;
            double coeff = 1.0;

            grid.Columns.Add("Безымяный столбик");
            foreach (UltraGridRow Row in grid.Rows)
            {
                if (Row.Index % 3 == 0)
                {
                    Row.Cells.FromKey("Безымяный столбик").Value = "Значение";
                    Row.NextRow.Cells.FromKey("Безымяный столбик").Value = "Абсолютное отклонение";
                    Row.NextRow.NextRow.Cells.FromKey("Безымяный столбик").Value = "Темп прироста";
                }
            }

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            headerLayout.AddCell("Наименование");

            // Заголовки
            GridHeaderCell lowcell = headerLayout;
            headerLayout.AddCell(" ");
            for (int i = 1; i < UltraWebGrid.Columns.Count - 3; i = i + 1)
            {
                headerLayout.AddCell(headerLayout.Grid.Columns[i].Header.Caption);
            }

            headerLayout.ApplyHeaderInfo();

            grid.Columns.FromKey("Безымяный столбик").Move(1);
            grid.Columns.FromKey("Безымяный столбик").Width = 180;

            foreach (UltraGridColumn column in grid.Columns)
            {
                column.Width = Convert.ToInt32(column.Width.Value * coeff);
                column.CellStyle.Font.Name = exportFontName;
                column.Header.Style.Font.Name = exportFontName;
                column.CellStyle.Font.Size = fontSize;
                column.Header.Style.Font.Size = fontSize;
            }
        }
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ExportGridSetup();
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            SetExportGridParams(headerLayout.Grid);

            ReportPDFExporter1.HeaderCellHeight = 60;
            Infragistics.Documents.Reports.Report.Text.IText header1 = section1.AddText();
            header1.Style.Font.Name = "Verdana";
            header1.Style.Font.Size = 15;
            header1.Style.Font.Bold = true;
            header1.AddContent(Label1.Text);

            Infragistics.Documents.Reports.Report.Text.IText header2 = section1.AddText();
            header2.Style.Font.Name = "Verdana";
            header2.Style.Font.Size = 13;
            header2.AddContent(Label2.Text);

            ReportPDFExporter1.Export(headerLayout, "", section1);

            ISection section2 = report.AddSection();
            UltraChart1.Width = Convert.ToInt32(UltraChart1.Width.Value * 0.8);
            ReportPDFExporter1.Export(UltraChart1, ChartCaption1.Text, section2);

            ISection section3 = report.AddSection();
            ReportPDFExporter1.Export(UltraChart2, ChartCaption2.Text, section3);
        }

        #endregion
    }
}
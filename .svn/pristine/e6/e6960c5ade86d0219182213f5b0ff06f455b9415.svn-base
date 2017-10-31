using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0035_0003_HMAO
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtChart = new DataTable();
        private DataTable dtGrid = new DataTable();
        private Dictionary<string, string> cropKindsDictinary;

        private Dictionary<string, string> CropKindsDictinary
        {
            get
            {
                if (cropKindsDictinary == null || cropKindsDictinary.Count == 0)
                {
                    FillKindsDictionary(dtChart);
                }
                return cropKindsDictinary;
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);


            PopupInformer1.HelpPageUrl = "Default.html";

            ultraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);
            ultraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenWidth * 0.65 - 130);
            ultraChart.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(ultraChart_FillSceneGraph);
            ultraChart.InvalidDataReceived +=new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.5 - 160);

            Link1.Visible = false;
            Link1.Text = "Исполнение&nbsp;кассового&nbsp;плана&nbsp;с&nbsp;динамикой";
            Link1.NavigateUrl = "~/reports/FO_0035_0002_HMAO/Default.aspx";

            Link2.Visible = true;
            Link2.Text = "Исполнение&nbsp;кассового&nbsp;плана";
            Link2.NavigateUrl = "~/reports/FO_0035_0001_HMAO/Default.aspx";

            Link3.Visible = true;
            Link3.Text = "Исполнение&nbsp;кассового&nbsp;плана&nbsp;(с&nbsp;процентом&nbsp;исполнения)";
            Link3.NavigateUrl = "~/reports/FO_0035_0003_2_HMAO/Default.aspx";

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
            UltraGridExporter1.MultiHeader = true;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                ComboCalendar.Visible = true;
                ComboCalendar.Width = 300;
                ComboCalendar.MultiSelect = false;
                ComboCalendar.ShowSelectedValue = false;
                ComboCalendar.ParentSelect = false;
                ComboCalendar.FillDictionaryValues(
                    CustomMultiComboDataHelper.FillCashPlanNonEmptyDays(DataDictionariesHelper.CashPlanNonEmptyDays));
                ComboCalendar.SelectLastNode();
                ComboCalendar.PanelHeaderTitle = "Выберите дату";

            }

            UserParams.PeriodCurrentDate.Value = GetDateUniqName(ComboCalendar.GetSelectedNodePath(), ComboCalendar.SelectedNode.Level);

            lbTitle.Text = "Исполнение кассового плана";

            lbSubTitle.Text = "на " + GetDateString(ComboCalendar.GetSelectedNodePath(), ComboCalendar.SelectedNode.Level) + " по бюджету автономного округа";

            UltraWebGrid.DataBind();
            ultraChart.DataBind();
        }

        public string GetDateUniqName(string source, int level)
        {
            string[] sts = source.Split('|');
            if (sts.Length > 0)
            {
                switch (level)
                {
                    // нулевой уровень выбрать нельзя
                    case 1:
                        {
                            string month = sts[1].TrimEnd(' ');
                            return string.Format(".[{0}].[Полугодие {1}].[Квартал {2}].[{3}]",
                                                 sts[0], CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(month)),
                                                 CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month)), month);
                        }
                    case 2:
                        {
                            string month = sts[1].TrimEnd(' ');
                            string day = sts[2].TrimEnd(' ');
                            return string.Format(".[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]",
                                                 sts[0], CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(month)),
                                                 CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month)), month,
                                                 day);
                        }
                }
            }
            return string.Empty;
        }

        public string GetDateString(string source, int level)
        {
            string[] sts = source.Split('|');
            if (sts.Length > 0)
            {
                switch (level)
                {
                    // нулевой уровень выбрать нельзя
                    case 1:
                        {
                            string month = sts[1].TrimEnd(' ');
                            return string.Format("{1} {0} года", sts[0], month.ToLower());
                        }
                    case 2:
                        {
                            string month = sts[1].TrimEnd(' ');
                            string day = sts[2].TrimEnd(' ');
                            if (day == "Заключительные обороты")
                            {
                                return string.Format("{1} {0} года", sts[0], month.ToLower());
                            }
                            else
                            {
                                return string.Format("{2} {1} {0} года", sts[0], CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)), day);
                            }
                        }
                }
            }
            return string.Empty;
        }

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            dtChart = new DataTable();
            string query = DataProvider.GetQueryText("ChartOutcome");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "name", dtChart);
            FillKindsDictionary(dtChart);
            CRHelper.NormalizeDataTable(dtChart);
            ReplaceQuotes(dtChart, 0);
            ultraChart.DataSource = dtChart;
            ultraChart.Data.SwapRowsAndColumns = true;
        }

        void ultraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        if (CropKindsDictinary.ContainsKey(box.DataPoint.Label))
                        {
                            box.DataPoint.Label = CropKindsDictinary[box.DataPoint.Label];
                        }
                    }
                }
            }
        }

        public static string CropString(string source, int letterCount)
        {
            if (source.Length > letterCount && letterCount > 3)
            {
                return source.Substring(0, letterCount - 3) + "...";
            }

            return source;
        }

        private void FillKindsDictionary(DataTable dt)
        {
            cropKindsDictinary = new Dictionary<string, string>();
            foreach (DataRow row in dt.Rows)
            {
                string kind = row[0].ToString();
                string shortKind = CropString(kind, 75);
                if (!cropKindsDictinary.ContainsKey(kind))
                {
                    cropKindsDictinary.Add(shortKind, kind);
                    row[0] = shortKind;
                }
            }
        }

        /// <summary>
        /// Заменяет кавычки на апострофы.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="colNum"></param>
        private static void ReplaceQuotes(DataTable dt, int colNum)
        {
            foreach(DataRow row in dt.Rows)
            {
                row[colNum] = row[colNum].ToString().Replace('"', '\'');
            }
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("GridOutcomeKinds");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "Наименование ГРБС", dtGrid);
            UltraWebGrid.DataSource = dtGrid;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(622);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 25;

                switch (i)
                {
                    case 1:
                    case 4:
                    case 8:
                        {
                            formatString = "N0";
                            widthColumn = 50;
                            break;
                        }
                    case 2:
                    case 6:
                        {
                            formatString = "N3";
                            widthColumn = 90;
                            break;
                        }

                    case 3:
                    case 7:
                        {
                            formatString = "P4";
                            widthColumn = 90;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
            }

            e.Layout.Bands[0].Columns[5].Hidden = true;
            e.Layout.Bands[0].Columns[9].Hidden = true;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 1)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 2;

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 4)
            {
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Сумма, млн.руб.", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Удельный вес", "Удельный вес в общей сумме расходов");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, "Ранг", "");

                string caption = string.Empty;
                string hint = string.Empty;
                int j = (i - 2) / 4;
                switch (j)
                {
                    case 0:
                        {
                            caption = "План";
                            //hint = string.Format("Объем бюджетных ассигнований ГРБС в {0} году согласно бюджетной росписи", year);
                            break;
                        }
                    case 1:
                        {
                            caption = "Факт";
                            //hint = string.Format("Кассовое исполнение расходов ГРБС за {0} {1} {2} года", monthNum,CRHelper.RusManyMonthGenitive(monthNum), year);
                            break;
                        }
                }

                ColumnHeader ch = CRHelper.AddHierarchyHeader(e.Layout.Grid,
                                                              0,
                                                              caption,
                                                              multiHeaderPos,
                                                              0,
                                                              3,
                                                              1);
                //ch.Title = hint;

                multiHeaderPos += 3;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            bool isChildRow = e.Row.PrevRow != null && e.Row.Cells[1] != null && e.Row.PrevRow.Cells[1] != null &&
                  e.Row.Cells[1].ToString() == e.Row.PrevRow.Cells[1].ToString();

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool rank = (i == 4 || i == 8);

                if (rank)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i + 1].Value != null &&
                        e.Row.Cells[i].Value.ToString() != string.Empty &&
                        e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                    {
                        string obj = "удельный вес";
                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = string.Format("Самый высокий {0}", obj);
                        }
                        else if (Convert.ToInt32(e.Row.Cells[i].Value) == Convert.ToInt32(e.Row.Cells[i + 1].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = string.Format("Самый низкий {0}", obj);
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (isChildRow)
                {
                    e.Row.Cells[i].Style.Font.Italic = true;
                }

                UltraGridCell c = e.Row.Cells[i];
                if (c.Value != null && c.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(c.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            c.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = lbTitle.Text + " " + lbSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            for (int i = 0; i < UltraWebGrid.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "#,##0.00;[Red]-#,##0.00";
                int widthColumn = 500;

                switch (i)
                {
                    case 1:
                    case 4:
                    case 7:
                        {
                            formatString = "#,##0";
                            widthColumn = 60;
                            break;
                        }
                    case 2:
                    case 5:
                        {
                            formatString = "#,##0.000;[Red]-#,##0.000";
                            widthColumn = 95;
                            break;
                        }
                    case 3:
                    case 6:
                        {
                            formatString = "#,##0.0000%";
                            widthColumn = 85;
                            break;
                        }
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";

            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            string caption;
            int i = (e.CurrentColumnIndex < 2) ? -1 : (e.CurrentColumnIndex - 2) / 3;
            switch (i)
            {
                case 0:
                    {
                        caption = "План";
                        break;
                    }
                case 1:
                    {
                        caption = "Факт";
                        break;
                    }
                default:
                    {
                        caption = e.HeaderText;
                        break;
                    }
            }

            e.HeaderText = caption;
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

            foreach (HeaderBase header in UltraWebGrid.Bands[0].HeaderLayout)
            {
                header.Caption = header.Caption.Replace("<br />", " ");
            }

            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(lbTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(lbSubTitle.Text);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                int widthColumn = 25;

                switch (i)
                {
                    case 1:
                    case 4:
                    case 8:
                        {
                            widthColumn = 52;
                            break;
                        }
                    case 2:
                    case 6:
                        {
                            widthColumn = 100;
                            break;
                        }

                    case 3:
                    case 7:
                        {
                            widthColumn = 100;
                            break;
                        }
                }
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
            }
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            ultraChart.Width = 1250;
            ultraChart.Height = 600;
            ultraChart.Legend.SpanPercentage = 40;
            Infragistics.Documents.Reports.Graphics.Image img1 = UltraGridExporter.GetImageFromChart(ultraChart);
            e.Section.AddImage(img1);
        }

        #endregion
    }
}

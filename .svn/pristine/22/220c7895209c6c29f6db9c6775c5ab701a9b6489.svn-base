using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Excel;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0007
{
    public partial class DefaultDetail : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtChart = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2003;
        private int endYear = 2011;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);


            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.SplineChart;
            UltraChart.BorderWidth = 0;

            UltraChart.Axis.X.Extent = 42;
            UltraChart.Axis.Y.Extent = 40;

            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
//            UltraChart.Axis.Y2.Extent = 50;
//            UltraChart.Axis.Y2.Visible = true;
//            UltraChart.Axis.Y2.Labels.Visible = false;
//            UltraChart.Axis.Y2.LineThickness = 0;
            
            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Right;
            UltraChart.Legend.SpanPercentage = 6;
            UltraChart.Legend.Margins.Bottom = Convert.ToInt32(UltraChart.Height.Value / 2);

            UltraChart.TitleLeft.Text = "Млн.руб.";
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Extent = 30;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent;
            UltraChart.TitleLeft.Visible = true;

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL> <SERIES_LABEL>г.\n<DATA_VALUE:N3> млн.руб.";

//            LineAppearance lineAppearance = new LineAppearance();
//            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
//            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Large;
//            lineAppearance.LineStyle.MidPointAnchors = false;
//            lineAppearance.Thickness = 3;
//            lineAppearance.SplineTension = (float)0.3;
//            UltraChart.SplineChart.LineAppearances.Add(lineAppearance);

//            EmptyAppearance emptyAppearance = new EmptyAppearance();
//            emptyAppearance.EnablePoint = true;
//            emptyAppearance.EnablePE = true;
//            emptyAppearance.EnableLineStyle = true;
//            emptyAppearance.PointStyle.Icon = SymbolIcon.Circle;
//            emptyAppearance.PointStyle.IconSize = SymbolIconSize.Large;
//            emptyAppearance.LineStyle.MidPointAnchors = true;
//            UltraChart.SplineChart.EmptyStyles.Add(emptyAppearance);

            //UltraChart.Data.ZeroAligned = true;
            UltraChart.SplineChart.NullHandling = NullHandling.DontPlot;
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion

            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.MultiHeader = true;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            int currentWidth = (int)Session["width_size"] - 20;
            UltraChart.Width = currentWidth - 20;

            int currentHeight = (int)Session["height_size"] - 192;
            UltraChart.Height = currentHeight / 2;

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth);
            //UltraWebGrid.Width = currentWidth;
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.5 - 120);

            if (!Page.IsPostBack)
            {
                WebAsyncPanel.AddRefreshTarget(UltraWebGrid);
                WebAsyncPanel.AddRefreshTarget(UltraChart);
                WebAsyncPanel.AddLinkedRequestTrigger(useStack.ClientID);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FK_0001_0007_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Width = 100;
                ComboYear.Title = "Годы";
                ComboYear.MultiSelect = true;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
                ComboYear.SetСheckedState((endYear - 1).ToString(), true);
                ComboYear.SetСheckedState((endYear - 2).ToString(), true);

                ComboKD.Width = 230;
                ComboKD.Title = "Вид дохода";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillShortKDIncludingList());
                ComboKD.SetСheckedState("Доходы ВСЕГО ", true);

                ComboSKIF.Width = 250;
                ComboSKIF.Title = "Уровень бюджета";
                ComboSKIF.FillDictionaryValues(CustomMultiComboDataHelper.FillSKIFLevels());
                ComboSKIF.ParentSelect = true;

                regionsCombo.Width = 250;
                regionsCombo.Title = "Субъект РФ";
                regionsCombo.FillDictionaryValues(CustomMultiComboDataHelper.FillRegions(RegionsNamingHelper.FoNames, RegionsNamingHelper.RegionsFoDictionary));
                regionsCombo.ParentSelect = false;

                if (!string.IsNullOrEmpty(UserParams.StateArea.Value))
                {
                    regionsCombo.SetСheckedState(UserParams.StateArea.Value, true);
                }
                else if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    regionsCombo.SetСheckedState(RegionSettings.Instance.Name, true);
                }
            }

            UserParams.Region.Value = regionsCombo.SelectedNodeParent;
            UserParams.StateArea.Value = regionsCombo.SelectedValue;
            UserParams.SKIFLevel.Value = ComboSKIF.SelectedValue;

            Page.Title = string.Format("Помесячная динамика поступления доходов");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Empty;

            Collection<string> selectedValues = ComboYear.SelectedValues;
            if (selectedValues.Count > 0)
            {
                string kd = ComboKD.SelectedValue;
                if (kd != "НДПИ " && kd != "НДФЛ ")
                {
                    kd = CRHelper.ToLowerFirstSymbol(kd);
                }

                PageSubTitle.Text = string.Format("{0}, {1} за {2} {3} ({4}), млн.руб.",
                    UserParams.StateArea.Value, kd, CRHelper.GetDigitIntervals(ComboYear.SelectedValuesString, ','),
                    ComboYear.SelectedValues.Count == 1 ? "год" : "годы", ComboSKIF.SelectedValue);

                string yearEquels = string.Empty;
                string yearDescedants = string.Empty;
                for (int i = 0; i < selectedValues.Count; i++)
                {
                    string year = selectedValues[i];

                    yearEquels += string.Format("[Период].[Период].CurrentMember.Parent.Parent.Parent.Name = \"{0}\" or ", year);
                    yearDescedants += string.Format("Descendants ([Период].[Период].[Данные всех периодов].[{0}],[Период].[Период].[Месяц],SELF) + ", year);
                }
                yearEquels = yearEquels.Remove(yearEquels.Length - 4, 3);
                yearDescedants = yearDescedants.Remove(yearDescedants.Length - 3, 2);
                UserParams.Filter.Value = string.Format("and ({0})", yearEquels);
                //UserParams.FKRFilter.Value = string.Format("Non Empty Filter({0}, [Measures].[Исполнено] <> 0)", yearDescedants);
                UserParams.FKRFilter.Value = string.Format("{1}{0}{2}", yearDescedants, '{', '}');
            }
            else
            {
                UserParams.Filter.Value = " ";
                UserParams.FKRFilter.Value = "{}";
            }
            UserParams.KDGroup.Value = ComboKD.SelectedValue;
            UserParams.SelectItem.Value = (!useStack.Checked) ? "Исполнено за период" : "Исполнено";

            UltraWebGrid.DataBind();
            UltraChart.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0007_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);

            // заполняем сразу дататейбл для диаграммы
            dtChart = new DataTable();
            dtChart.Columns.Add("Год", typeof(string));

            DataTable dtGrid2 = new DataTable();
            dtGrid2.Columns.Add("Год", typeof(string));

            for (int i = 1; i <= 12; i++)
            {
                dtChart.Columns.Add(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(i)), typeof(double));
                dtGrid2.Columns.Add(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(i)) + "; Исполнено", typeof(double));
                dtGrid2.Columns.Add(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(i)) + "; Темп роста", typeof(double));
            }

            List<int> inserts = new List<int>();

            DataRow chartRow = null;
            DataRow gridRow = null;
            for (int k = 0; k < dtGrid.Rows.Count; k++)
            {
                DataRow row = dtGrid.Rows[k];

                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if ((i == 2) && row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }
                }

                if ((row[0] != DBNull.Value && (row[0].ToString() == "Январь" || k == 0)))
                {
                    inserts.Add(k + inserts.Count);

                    chartRow = dtChart.NewRow();
                    dtChart.Rows.Add(chartRow);
                    chartRow[0] = row[1].ToString();

                    gridRow = dtGrid2.NewRow();
                    dtGrid2.Rows.Add(gridRow);
                    gridRow[0] = row[1].ToString();
                }

                string columnName = row[0].ToString();
                if (chartRow != null && columnName != string.Empty)
                {
                    if (row[2] != DBNull.Value)
                    {
                        chartRow[columnName] = row[2].ToString();
                        gridRow[columnName + "; Исполнено"] = row[2].ToString();
                    }
                    if (row[3] != DBNull.Value)
                    {
                        gridRow[columnName + "; Темп роста"] = row[3].ToString();
                    }
                }
            }

            for (int i = 0; i < inserts.Count; i++)
            {
                DataRow r = dtGrid.NewRow();
                r[0] = dtGrid.Rows[inserts[i]].ItemArray[1].ToString();
                dtGrid.Rows.InsertAt(r, inserts[i]);
            }

            if (dtGrid.Columns.Count > 2)
            {
                dtGrid.Columns.RemoveAt(1);
            }

            UltraWebGrid.DataSource = dtGrid2;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N1";
                int columnWidth = HttpContext.Current.Request.Browser.Browser == "IE" ? 45 : 47;

                if (i % 2 == 0)
                {
                    formatString = "P0";
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            int zeroColumnWidth = HttpContext.Current.Request.Browser.Browser == "IE" ? 26 : 28;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(zeroColumnWidth);

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 1;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
            {
                ColumnHeader ch = new ColumnHeader(true);
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                ch.Caption = captions[0].TrimEnd('_');

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Факт", string.Format("Фактические поступления {0}, млн.руб.", (useStack.Checked) ? "с начала года" : "за месяц"));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Темп роста", "Темп роста поступлений к аналогичному периоду предыдущего года");

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 2;
                ch.RowLayoutColumnInfo.SpanX = 2;
                ch.Style.Wrap = true;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (i % 2 == 0)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthLeft = 0;
                }
                else if (i % 2 != 0 && i != e.Row.Cells.Count - 1)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthRight = 0;
                }

                e.Row.Cells[i].Style.Padding.Left = 3;
                e.Row.Cells[i].Style.Padding.Right = 3;
                if ((i % 2 == 0 && i != 0) && e.Row.Cells[i].Value != null)
                {
                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "Рост к прошлому году";
                    }
                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "Снижение к прошлому году";
                    }
                    if (e.Row.Cells[i].Column.Width.Value < 60)
                    {
                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left center;";
                    }
                    else
                    {
                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; padding-left: 10px; background-position: 10px center;";
                    }
                }

                UltraGridCell cell = e.Row.Cells[i];
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

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid.Height = Unit.Empty;
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            // данные получаются из dtGrid в обработчике UltraWebGrid_DataBinding
            UltraChart.DataSource = dtChart;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
           
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            for (int i = 1; i < UltraWebGrid.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "#,##0.0";
                int widthColumn = 65;

                if (i % 2 == 0)
                {
                    formatString = UltraGridExporter.ExelPercentFormat;
                    widthColumn = 65;
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
            }
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            sheet1.Rows[0].Cells[0].Value = PageTitle.Text;
            sheet1.Rows[1].Cells[0].Value = PageSubTitle.Text.Replace("<br />", string.Empty);
            sheet1.Rows[2].Cells[0].Value = string.Format("Сумма {0}", (useStack.Checked) ? "с начала года с накоплением" : "за месяц");
            sheet2.Rows[0].Cells[0].Value = PageTitle.Text ;
            sheet2.Rows[1].Cells[0].Value = PageSubTitle.Text.Replace("<br />", string.Empty);
            sheet2.Rows[2].Cells[0].Value = string.Format("Сумма {0}", (useStack.Checked) ? "с начала года с накоплением" : "за месяц");
            UltraGridExporter.ChartExcelExport(sheet2.Rows[3].Cells[0], UltraChart);

            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1, 4, 0);
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            foreach (UltraGridRow row in UltraWebGrid.Rows)
            {
                foreach (UltraGridCell cell in row.Cells)
                {
                    try
                    {
                        //this is magicka by Ivan_Gusev )))))))))))))
                        cell.Value = string.Format("{0:N2}", cell.Value).Replace(" ", " ");
                    }
                    catch { }

                }
            }


            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(50);
            }

            IText title = e.Section.AddText();Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text.Replace("<br />", string.Empty));

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(string.Format("Сумма {0}", (useStack.Checked) ? "с начала года с накоплением" : "за месяц"));

            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);

            e.Section.AddPageBreak();

            title = e.Section.AddText();
            font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text.Replace("<br />", string.Empty));

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(string.Format("Сумма {0}", (useStack.Checked) ? "с начала года с накоплением" : "за месяц"));
        }


        #endregion
    }
}

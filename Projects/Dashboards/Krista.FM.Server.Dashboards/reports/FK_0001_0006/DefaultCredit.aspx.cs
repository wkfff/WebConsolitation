using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Excel;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0006
{
    public partial class DefaultCredit : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private DataTable dtChart1 = new DataTable();
        private DataTable dtChart2 = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2010;

        private string chartLabelText1 = string.Empty;
        private string chartLabelText2 = string.Empty;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.5 - 150);

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.4 - 15);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 150);
            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.6 - 15);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 150);

            UltraChart1.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

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

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FK_0001_0006_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                UserParams.PeriodYear.Value = endYear.ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(UserParams.PeriodYear.Value, true);

                regionsCombo.Width = 300;
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

                ComboSKIFLevel.Width = 250;
                ComboSKIFLevel.ParentSelect = true;
                ComboSKIFLevel.Title = "Уровень бюджета";
                ComboSKIFLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillSKIFLevels());
                ComboSKIFLevel.SetСheckedState("Консолидированный бюджет субъекта", true);

                UserParams.PeriodMonth.Value = dtDate.Rows[0][3].ToString();
            }

            UserParams.Region.Value = regionsCombo.SelectedNodeParent;
            UserParams.StateArea.Value = regionsCombo.SelectedValue; 

            Page.Title = string.Format("{0}: Кредиты кредитных организаций", UserParams.StateArea.Value);
            Label1.Text = Page.Title;
            Label2.Text = string.Format("Динамика получения и погашения кредитов в валюте РФ ({2}) в {0}-{1} гг", 
                (Convert.ToInt32(ComboYear.SelectedValue) - 1), ComboYear.SelectedValue, ComboSKIFLevel.SelectedValue);

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(UserParams.PeriodMonth.Value)));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(UserParams.PeriodMonth.Value)));
            UserParams.PeriodLastYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) - 1).ToString();
            UserParams.SKIFLevel.Value = ComboSKIFLevel.SelectedValue;

            UltraWebGrid.DataBind();
            UltraChart1.DataBind();
            UltraChart2.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0006_grid");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);

            List<int> inserts = new List<int>();

            for (int k = 0; k < dtGrid.Rows.Count; k++)
            {
                DataRow row = dtGrid.Rows[k];

                if ((row[0] != DBNull.Value && (row[0].ToString() == "Январь" || k == 0)))
                {
                    inserts.Add(k + inserts.Count);
                }

                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    int j = (i % 5);
                    if ((j == 2 || j == 3 || j == 4) && row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
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

            UltraWebGrid.DataSource = dtGrid;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (Page.IsPostBack)
                return;

            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 57;

                int j = (i - 1) % 5;

                switch (j)
                {
                    case 0:
                    case 1:
                    case 2:
                        {
                            formatString = "N3";
                            widthColumn = 90;
                            break;
                        }
                    case 3:
                        {
                            formatString = "P2";
                            widthColumn = 77;
                            break;
                        }
                    case 4:
                        {
                            formatString = "P2";
                            widthColumn = 118;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(100);

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 11)
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
           
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 5)
            {
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, (i != 11) ?
                    "Исполнено за период, млн.руб." : "Баланс, млн.руб.", 
                    (i != 11) ? "" : "Сумма исполнения по привлечению и погашению кредитов");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Исполнено, млн.руб.", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, "Назначено, млн.руб.", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, "Процент выполнения назначений", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 4, "Темп роста к аналогичному периоду предыдущего года", "");

                CRHelper.AddHierarchyHeader(e.Layout.Grid,
                    0,
                    captions[0].TrimEnd('_'),
                    multiHeaderPos,
                    0,
                    5,
                    1);

                multiHeaderPos += 5;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if ((i == 5 || i == 10) && e.Row.Cells[i].Value != null)
                {
                    if (100*Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "Рост к прошлому году";
                    }
                    else if (100*Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "Падение к прошлому году";
                    }
                    string style = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    e.Row.Cells[i].Style.CustomRules = style;
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

                        // года в нулевой колонке
                        if (i == 0)
                        {
                            cell.Style.Font.Bold = true;
                        }
                    }
                }
            }
        }

        #endregion

        #region Обработчики диаграмм

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            UltraChart2.ChartType = ChartType.SplineAreaChart;
            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N3> млн.руб.";
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart2.TitleLeft.Text = "              Млн. руб.";
            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.BorderWidth = 0;
            UltraChart2.Axis.Y.Extent = 30;
            UltraChart2.Data.SwapRowsAndColumns = true;
            
            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Top;
            UltraChart2.Legend.SpanPercentage = 13;

            UltraChart2.ColorModel.ModelStyle = ColorModels.CustomSkin;

            Color color1 = Color.Thistle;
            Color color2 = Color.PaleTurquoise;
            Color color3 = Color.DarkViolet;
            Color color4 = Color.SteelBlue;

            UltraChart2.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
            UltraChart2.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));
            UltraChart2.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color3, 20));
            UltraChart2.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color4, 20));

            UltraChart2.Effects.Effects.Clear();
            GradientEffect effect = new GradientEffect();
            effect.Coloring = GradientColoringStyle.Darken;
            effect.Enabled = true;
            UltraChart2.Effects.Enabled = true;
            UltraChart2.Effects.Effects.Add(effect);    

            string query = DataProvider.GetQueryText("FK_0001_0006_chart1");
            dtChart1 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtChart1);

            DataTable dt = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            for (int k = 0; k < dtChart1.Rows.Count; k++)
            {
                DataRow row = dtChart1.Rows[k];

                if (row[0] != DBNull.Value && dtChart1.Rows[k][0] != DBNull.Value && (row[0].ToString() == "Январь" || k == 0))
                {
                    row[0] = string.Format("{0} - {1}", dt.Rows[k][0], row[0]);
                }

                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }
                }
            }

            UltraChart2.DataSource = dtChart1;
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            UltraChart1.ChartType = ChartType.StackColumnChart;
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N3> млн.руб.";
            UltraChart1.Axis.X.Extent = 40;
            UltraChart1.Axis.X.Labels.SeriesLabels.WrapText = true;
            UltraChart1.Axis.Y.Extent = 30;
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Text = "              Млн. руб.";
            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.BorderWidth = 0;

            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Top;
            UltraChart1.Legend.SpanPercentage = 13;

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = 0;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = "<DATA_VALUE_ITEM:N3>";
            appearance.ChartTextFont = new Font("Verdana", 8);
            appearance.Visible = true;
            UltraChart1.ColumnChart.ChartText.Add(appearance);

            appearance = new ChartTextAppearance();
            appearance.Column = 1;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Near;
            appearance.ItemFormatString = "<DATA_VALUE_ITEM:N3>";
            appearance.ChartTextFont = new Font("Verdana", 8);
            appearance.Visible = true;
            UltraChart1.ColumnChart.ChartText.Add(appearance);

            string query = DataProvider.GetQueryText("FK_0001_0006_chart2");
            dtChart2 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtChart2);

            for (int k = 0; k < dtChart2.Rows.Count; k++)
            {
                DataRow row = dtChart2.Rows[k];
                row[0] = row[0].ToString().Split(';')[0];

                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }
                }
            }

            bool isPlan = true;

            if (Convert.ToInt32(UserParams.PeriodYear.Value) != endYear)
            {
                if (dtChart2.Rows.Count > 5)
                {
                    dtChart2.Rows.RemoveAt(0);
                    dtChart2.Rows.RemoveAt(0);
                    dtChart2.Rows.RemoveAt(0);
                    dtChart2.Rows.RemoveAt(0);

                    dtChart2.Rows[0][0] = string.Format("{0} год", UserParams.PeriodLastYear.Value);
                    dtChart2.Rows[1][0] = string.Format("{0} год", UserParams.PeriodYear.Value);
                }

                isPlan = false;
            }
            else
            {
                if (dtChart2.Rows.Count > 5)
                {
                    dtChart2.Rows.RemoveAt(4);
                    dtChart2.Rows.RemoveAt(4);

                    string months;
                    if (UserParams.PeriodMonth.Value == "Январь")
                    {
                        months = UserParams.PeriodMonth.Value; 
                    }
                    else
                    {
                        months = string.Format("Январь-{0}", UserParams.PeriodMonth.Value);                        
                    }

                    dtChart2.Rows[0][0] = string.Format("{0} год", UserParams.PeriodLastYear.Value);
                    dtChart2.Rows[1][0] = string.Format("План на {0} год", UserParams.PeriodYear.Value);
                    dtChart2.Rows[2][0] = string.Format("{1} {0} года", UserParams.PeriodLastYear.Value, months);
                    dtChart2.Rows[3][0] = string.Format("{1} {0} года", UserParams.PeriodYear.Value, months);
                }
            }

            double percent1 = 0;
            if (dtChart2.Rows[0][1] != DBNull.Value && dtChart2.Rows[1][1] != DBNull.Value && Convert.ToDouble(dtChart2.Rows[0][1]) != 0)
            {
                percent1 = Convert.ToDouble(dtChart2.Rows[1][1]) / Convert.ToDouble(dtChart2.Rows[0][1]);
            }

            double percent2 = 0;
            if (dtChart2.Rows[0][2] != DBNull.Value && dtChart2.Rows[1][2] != DBNull.Value && Convert.ToDouble(dtChart2.Rows[0][2]) != 0)
            {
                percent2 = Convert.ToDouble(dtChart2.Rows[1][2]) / Convert.ToDouble(dtChart2.Rows[0][2]);
            }

            string plan1 = (dtChart2.Rows[1][1] != DBNull.Value) ? Convert.ToDouble(dtChart2.Rows[1][1]).ToString("N3") : string.Empty;
            string plan2 = (dtChart2.Rows[1][2] != DBNull.Value) ? Convert.ToDouble(dtChart2.Rows[1][2]).ToString("N3") : string.Empty;

            percent1 = percent1 - 1;
            percent2 = percent2 - 1;

            string img1 = (percent1 > 0)
             ? "больше<img src = \"../../images/arrowGreenUpBB.png\">"
             :
               "меньше<img src = \"../../images/arrowRedDownBB.png\">";

            string img2 = (percent2 > 0)
             ? "больше<img src = \"../../images/arrowGreenUpBB.png\">"
             :
               "меньше<img src = \"../../images/arrowRedDownBB.png\">";

            string planText1 = isPlan ? "планируется привлечь" : "были привлечены";
            string planText2 = isPlan ? "планируется погасить" : "были погашены";

            chartLabel1.Text = string.Empty;
            chartLabel2.Text = string.Empty;
            if (plan1 != string.Empty && plan2 != string.Empty)
            {
                chartLabel1.Text =
                    string.Format(
                        "В {0} году {5} кредиты кредитных организаций на сумму <span style=\"font-weight:bold\">{2}</span> млн.руб., что на  <span style=\"font-weight:bold\">{3}</span> {4}, чем было привлечено в {1} году.",
                        UserParams.PeriodYear.Value, UserParams.PeriodLastYear.Value, plan1, Math.Abs(percent1).ToString("P2"), img1, planText1);

                chartLabelText1 = string.Format("В {0} году {5} кредиты кредитных организаций на сумму {2} млн.руб., что на  {3} {4}, чем было привлечено в {1} году.",
                        UserParams.PeriodYear.Value, UserParams.PeriodLastYear.Value, plan1, Math.Abs(percent1).ToString("P2"), img1.Split('<')[0], planText1);
                chartLabel2.Text =
                    string.Format(
                        "В {0} году {5} кредиты кредитных организаций на сумму <span style=\"font-weight:bold\">{2}</span> млн.руб., что на  <span style=\"font-weight:bold\">{3}</span> {4}, чем было погашено в {1} году.",
                        UserParams.PeriodYear.Value, UserParams.PeriodLastYear.Value, plan2, Math.Abs(percent2).ToString("P2"), img2, planText2);
                chartLabelText2 = string.Format(
                        "В {0} году {5} кредиты кредитных организаций на сумму {2} млн.руб., что на  {3} {4}, чем было погашено в {1} году.",
                        UserParams.PeriodYear.Value, UserParams.PeriodLastYear.Value, plan2, Math.Abs(percent2).ToString("P2"), img2.Split('<')[0], planText2);
            }
            UltraChart1.DataSource = dtChart2;
        }

        #endregion

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            //Label1.Text + " " + Label2.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            for (int i = 1; i < UltraWebGrid.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "#,##0";
                int widthColumn = 80;

                int j = (i - 1) % 5;

                switch (j)
                {
                    case 0:
                    case 1:
                    case 2:
                        {
                            formatString = "#,##0.000;[Red]-#,##0.000";
                            widthColumn = 113;
                            break;
                        }
                    case 3:
                        {
                            formatString = UltraGridExporter.ExelPercentFormat;
                            widthColumn = 100;
                            break;
                        }
                    case 4:
                        {
                            formatString = UltraGridExporter.ExelPercentFormat;
                            widthColumn = 140;
                            break;
                        }
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
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграммы");
            sheet1.Rows[0].Cells[0].Value = Label1.Text;
            sheet1.Rows[1].Cells[0].Value = Label2.Text;

            sheet2.Rows[0].Cells[0].Value = Label1.Text;
            sheet2.Rows[1].Cells[0].Value = Label2.Text;
            sheet2.Rows[2].Cells[0].Value = chartLabelText1;           
            sheet2.Rows[3].Cells[0].Value = chartLabelText2;
            UltraGridExporter.ChartExcelExport(sheet2.Rows[4].Cells[0],  UltraChart1);
            UltraGridExporter.ChartExcelExport(sheet2.Rows[4].Cells[11], UltraChart2);
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1, 2, 0);
            
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.GridElementCaption = Label1.Text + "\n" + Label2.Text;
            UltraGridExporter1.HeaderChildCellHeight = 60;
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);
            
            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(chartLabelText1);

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(chartLabelText2);

            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart2);
            e.Section.AddImage(img);
            
            img = UltraGridExporter.GetImageFromChart(UltraChart1);
            e.Section.AddImage(img);

            e.Section.AddPageBreak();
        }
    }
}


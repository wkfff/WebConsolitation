using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0004
{
    public partial class DefaultCompareChart_budget : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtChart = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 160);

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.StackAreaChart;
            UltraChart.Axis.X.Extent = 80;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.Y.Extent = 50;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 13;
            UltraChart.Legend.Margins.Right = Convert.ToInt32(UltraChart.Width.Value / 3);

            UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;
            UltraChart.Border.Thickness = 0;
            UltraChart.Data.SwapRowsAndColumns = true;

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.Text = "Млн.руб.";
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent + (UltraChart.Legend.SpanPercentage * Convert.ToInt32(UltraChart.Height.Value)) / 100;

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE_ITEM:N3> млн.руб.";

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Diamond;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            UltraChart.AreaChart.LineAppearances.Add(lineAppearance);

            UltraChart.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion

            UltraGridExporter1.ExcelExportButton.Visible = false;
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
            UltraGridExporter1.MultiHeader = true;

            CrossLink1.Text = "Сравнение&nbsp;темпа&nbsp;роста&nbsp;фактических&nbsp;доходов";
            CrossLink1.NavigateUrl = "~/reports/FK_0001_0004/DefaultCompare_budget.aspx";
            CrossLink2.Text = "Распределение&nbsp;на&nbsp;группы&nbsp;по&nbsp;темпам&nbsp;роста&nbsp;доходов";
            CrossLink2.NavigateUrl = "~/reports/FK_0001_0004/DefaultGroupAllocation_budget.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FK_0001_0004_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboSubject.Title = "Субъект РФ";
                ComboSubject.Width = 300;
                ComboSubject.MultiSelect = false;
                ComboSubject.Title = "Субъект РФ";
                ComboSubject.FillDictionaryValues(CustomMultiComboDataHelper.FillRegions(RegionsNamingHelper.FoNames, RegionsNamingHelper.RegionsFoDictionary));
                ComboSubject.ParentSelect = false;
                if (!string.IsNullOrEmpty(UserParams.StateArea.Value))
                {
                    ComboSubject.SetСheckedState(UserParams.StateArea.Value, true);
                }
                else if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    ComboSubject.SetСheckedState(RegionSettings.Instance.Name, true);
                }

                ComboSKIFLevel.Width = 250;
                ComboSKIFLevel.ParentSelect = true;
                ComboSKIFLevel.Title = "Уровень бюджета";
                ComboSKIFLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillSKIFLevels());
                ComboSKIFLevel.SetСheckedState("Консолидированный бюджет субъекта", true);
            }

            Page.Title = string.Format("Структурная динамика фактических доходов ({0})", ComboSubject.SelectedValue);
            Label1.Text = Page.Title;

            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            UserParams.PeriodEndYear.Value = yearNum.ToString();
            UserParams.PeriodYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodFirstYear.Value = (yearNum - 2).ToString();
            
            UserParams.Region.Value = ComboSubject.SelectedNodeParent;
            UserParams.StateArea.Value = ComboSubject.SelectedValue;

            UserParams.SKIFLevel.Value = ComboSKIFLevel.SelectedValue;

            UltraChart.DataBind();
        }

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0004_compare_chart_budget");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            foreach (DataColumn column in dtChart.Columns)
            {
                column.ColumnName = column.ColumnName.TrimEnd('_');
            }

            DataTable dt = new DataTable();
            query = DataProvider.GetQueryText("FK_0001_0004_compare_chart_budget");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            List<string> yearList = new List<string>();

            for (int j = 0; j < dtChart.Rows.Count; j++)
            {
                DataRow row = dtChart.Rows[j];
                // если строка первая, либо в первой ячейке Январь
                if (row[0] != DBNull.Value && dt.Rows[j][0] != DBNull.Value &&
                        (row[0].ToString() == "Январь" || j == 0))
                {
                    row[0] = string.Format("{0} - {1}", dt.Rows[j][0], row[0]);
                    yearList.Add(dt.Rows[j][0].ToString());
                }
                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }
                }
            }

            if (yearList.Count == 1)
            {
                Label2.Text = string.Format("Сравнение структуры помесячного поступления доходов ({1}) за {0} год",
                    yearList[0], ComboSKIFLevel.SelectedValue);
            }
            else if (yearList.Count > 1)
            {
                Label2.Text = string.Format("Сравнение структуры помесячного поступления доходов ({2}) за {0} - {1} годы",
                    yearList[0], yearList[yearList.Count - 1], ComboSKIFLevel.SelectedValue);
            }
            
            UltraChart.DataSource = dtChart;
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(new UltraWebGrid());
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
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        #endregion
    }
}

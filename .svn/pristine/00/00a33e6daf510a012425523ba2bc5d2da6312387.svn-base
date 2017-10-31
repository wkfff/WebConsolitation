using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Resources.Appearance;
using System.IO;
using Infragistics.UltraGauge.Resources;

namespace Krista.FM.Server.Dashboards.reports.FK_0004_0007
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable grbsGridDt = new DataTable();
        private DataTable restChartDt = new DataTable();
        private DataTable dynamicChartDt = new DataTable();
        private DataTable dynamicChartDtVvp = new DataTable();
        private DataTable dtChartLimit = new DataTable();
        private DataTable RealDynamicChartDt = new DataTable();
        private DataTable RealDynamicChartDtVvp = new DataTable();

        private DateTime currentDate;
        private DateTime lastDate;

        #endregion

        #region Параметры запроса

        // выбранный период
        private CustomParam selectedPeriod;
        // выбранный показатель таблицы
        private CustomParam selectedGridIndicator;

        #endregion

        private bool IsMlnRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string multiplierCaption;

        // выбранный множитель рублей
        private CustomParam rubMultiplier;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GRBSGridBrick.AutoSizeStyle = GridAutoSizeStyle.None;
            GRBSGridBrick.Height = 600;
            GRBSGridBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            GRBSGridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(GRBSGrid_InitializeLayout);
            GRBSGridBrick.Grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);

            #endregion

            #region Настройка диаграммы динамики

            multiplierCaption = IsMlnRubSelected ? "млн.руб." : "млрд.руб.";

            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");
            selectedGridIndicator = UserParams.CustomParam("selected_grid_indicator");

            #endregion
            rubMultiplier = UserParams.CustomParam("rub_multiplier");

            rubMultiplier.Value = IsMlnRubSelected ? "1000000" : "1000000000";


            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        private static Color GetColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.FromArgb(8, 103, 169);
                    }
                case 2:
                    {
                        return Color.FromArgb(230, 194, 33);
                    }
                case 3:
                    {
                        return Color.Red;
                    }
                case 4:
                    {
                        return Color.FromArgb(245, 136, 55);
                    }
                case 5:
                    {
                        return Color.Violet;
                    }
                case 6:
                    {
                        return Color.DarkTurquoise;
                    }
                case 7:
                    {
                        return Color.Crimson;
                    }
                default:
                    {
                        return Color.White;
                    }
            }
        }

        private static Color GetStopColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.FromArgb(8, 103, 169);
                    }
                case 2:
                    {
                        return Color.FromArgb(230, 194, 33);
                    }
                case 3:
                    {
                        return Color.Green;
                    }
                case 4:
                    {
                        return Color.FromArgb(245, 136, 55);
                    }
                case 5:
                    {
                        return Color.Violet;
                    }
                case 6:
                    {
                        return Color.DarkTurquoise;
                    }
                case 7:
                    {
                        return Color.Crimson;
                    }
                default:
                    {
                        return Color.White;
                    }
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                if (selectedPeriod.Value != null &&
                    selectedPeriod.Value != String.Empty)
                {
                    // Response.Write(selectedPeriod.Value);
                    // CustomCalendar1.WebCalendar.SelectedDate = CubeInfo.GetLastDate(DataProvidersFactory.SecondaryMASDataProvider, "FK_0004_0007_lastDate");
                    CustomCalendar1.WebCalendar.SelectedDate = CRHelper.PeriodDayFoDate("[Период]." + selectedPeriod.Value);
                }
                else
                {
                    CustomCalendar1.WebCalendar.SelectedDate = CubeInfo.GetLastDate(DataProvidersFactory.SecondaryMASDataProvider, "FK_0004_0007_lastDate");
                }
            }

            currentDate = CustomCalendar1.WebCalendar.SelectedDate;
            lastDate = currentDate.AddYears(-1);
            selectedPeriod.Value = CRHelper.PeriodMemberUName("[Период].[Период]", currentDate, 5);
            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            topBorder = 100 / 12 * currentDate.Month;
            bottomBorder = topBorder / 2.5;

            Page.Title = String.Format("Характеристика исполнения расходов главными распорядителями средств федерального бюджета ");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("по состоянию на <b>{0:dd.MM.yyyy} г., {1}</b>", currentDate, multiplierCaption);

           // DynamicChartCaption.Text = String.Format("Соотношение показателей исполнения бюджетов бюджетной системы РФ на {0:dd.MM.yyyy} г., {1}", currentDate.AddDays(1), multiplierCaption);

            GridDataBind();
          //  DynamicChartDataBind();
            BindInfoText();
        }

        private void BindInfoText()
        {
            string query = DataProvider.GetQueryText("Prog_0002_0001_top");
            DataTable dtInfo = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtInfo);

            lbInfo.Text = "<img src='../../images/StarGrayBB.png'>&nbsp;<b>Минимальный</b> процент исполнения:<table>";

            for (int i = 0; i < 5; i++)
            {
                lbInfo.Text = String.Format(@"{0}
<tr><td align='right'><b>{1:N2}%</b></td><td align='right'> {3}</td><td> {2}</td></tr>", lbInfo.Text, dtInfo.Rows[i][3], dtInfo.Rows[i][1], dtInfo.Rows[i][2]);
            }

            lbInfo.Text = String.Format(@"{0}</table><br/>
<img src='../../images/StarYellowBB.png'>&nbsp;<b>Максимальный</b> процент исполнения:<table>", lbInfo.Text);

            for (int i = 5; i < 10; i++)
            {
                lbInfo.Text = String.Format(@"{0}
 <tr><td align='right'><b>{1:N2}%</b></td><td align='right'> {3}</td><td> {2}</td></tr>", lbInfo.Text, dtInfo.Rows[i][3], dtInfo.Rows[i][1], dtInfo.Rows[i][2]);
            }
            lbInfo.Text = String.Format(@"{0}</table>", lbInfo.Text);
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FK_0004_0007_grid");
            grbsGridDt = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", grbsGridDt);

            if (grbsGridDt.Rows.Count > 0)
            {
                if (grbsGridDt.Columns.Count > 0)
                {
                    grbsGridDt.Columns.RemoveAt(0);
                }
                
                RankIndicatorRule rankRule = new RankIndicatorRule(6, 9);
                GRBSGridBrick.AddIndicatorRule(rankRule);

                FontRowLevelRule levelRule = new FontRowLevelRule(grbsGridDt.Columns.Count - 1);
                levelRule.AddFontLevel("1", GRBSGridBrick.BoldFont8pt);
                levelRule.AddFontLevel("2", GRBSGridBrick.Font8pt);
                GRBSGridBrick.AddIndicatorRule(levelRule);

                GRBSGridBrick.DataTable = grbsGridDt;
            }
        }

        double topBorder = 0;
        double bottomBorder = 0;

        protected string GetGaugeUrl(object oValue, string alt)
        {
            if (oValue == DBNull.Value)
                return String.Empty;
            double value = Convert.ToDouble(oValue);
            if (value > 100)
                value = 100;
            string path = "EO_0004_0007_gauge_" + value.ToString("N0") + ".png";
            string returnPath = String.Format("<img alt=\"{1}\" style=\" float: right; margin-left: 10px\" src=\"../../TemporaryImages/{0}\"/>", path, alt);
            string serverPath = String.Format("~/TemporaryImages/{0}", path);

            if (File.Exists(Server.MapPath(serverPath)))
            {
                return returnPath;
            }
            LinearGaugeScale scale = ((LinearGauge)Gauge.Gauges[0]).Scales[0];
            scale.Markers[0].Value = value;
            MultiStopLinearGradientBrushElement BrushElement = (MultiStopLinearGradientBrushElement)(scale.Markers[0].BrushElement);
            BrushElement.ColorStops.Clear();
            if (value >= topBorder)
            {

                BrushElement.ColorStops.Add(Color.FromArgb(223, 255, 192), 0);
                BrushElement.ColorStops.Add(Color.FromArgb(128, 255, 128), 0.41F);
                BrushElement.ColorStops.Add(Color.FromArgb(0, 192, 0), 0.428F);
                BrushElement.ColorStops.Add(Color.Green, 1);
            }
            else if (value < bottomBorder)
            {

                BrushElement.ColorStops.Add(Color.FromArgb(253, 119, 119), 0);
                BrushElement.ColorStops.Add(Color.FromArgb(239, 87, 87), 0.41F);
                BrushElement.ColorStops.Add(Color.FromArgb(224, 0, 0), 0.428F);
                BrushElement.ColorStops.Add(Color.FromArgb(199, 0, 0), 1);
            }
            else
            {
                BrushElement.ColorStops.Add(Color.FromArgb(255, 255, 128), 0);
                BrushElement.ColorStops.Add(Color.Yellow, 0.41F);
                BrushElement.ColorStops.Add(Color.Yellow, 0.428F);
                BrushElement.ColorStops.Add(Color.FromArgb(255, 128, 0), 1);
            }

            Size size = new Size(100, 40);
            Gauge.SaveTo(Server.MapPath(serverPath), GaugeImageType.Png, size);
            return returnPath;
        }

        void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            double value;
            if (e.Row.Cells[5].Value != null &&
                double.TryParse(e.Row.Cells[5].Value.ToString(), out value))
            {
                e.Row.Cells[5].Value = String.Format("<table style='width: 100%;'><tr><td style='border: none; width: 100%; align: right;'>{0:N2}% </td><td style='border: none'>{1}</td></tr></table>", value, GetGaugeUrl(value, String.Empty));
            }
        }

        protected void GRBSGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.Bands[0].Columns[0].CellStyle.Padding.Right = 10;

            int columnCount = e.Layout.Bands[0].Columns.Count;

            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 2].Hidden = true;

            for (int i = 2; i < columnCount - 2; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
            }
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "P2");

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(80);
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(110);
            e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(110);
            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(110);
            e.Layout.Bands[0].Columns[5].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[6].Width = CRHelper.GetColumnWidth(90);
            e.Layout.Bands[0].Columns[7].Width = CRHelper.GetColumnWidth(110);
            e.Layout.Bands[0].Columns[8].Width = CRHelper.GetColumnWidth(90);

            GridHeaderLayout headerLayout = GRBSGridBrick.GridHeaderLayout;

            headerLayout.AddCell("Код");
            headerLayout.AddCell("Главный распорядитель средств федерального бюджета");
            headerLayout.AddCell(String.Format("Первоначальный план, {0}", multiplierCaption));
            headerLayout.AddCell(String.Format("Уточненная бюджетная роспись, {0}", multiplierCaption));
            headerLayout.AddCell(String.Format("Кассовое исполнение нарастающим итогом, {0}", multiplierCaption));
            headerLayout.AddCell(String.Format("Процент исполнения годовых назначений"));
            headerLayout.AddCell(String.Format("Ранг по % исполнения"));
            headerLayout.AddCell(String.Format("Прирост расходов за месяц, {0}", multiplierCaption));
            headerLayout.AddCell(String.Format("Доля в расходах"));

            headerLayout.ApplyHeaderInfo();
        }

        #endregion

       

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(GRBSGridBrick.GridHeaderLayout, sheet1, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(GRBSGridBrick.GridHeaderLayout, section1);
        }

        #endregion
    }
}
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class Oil_0006_0003 : CustomReportPage
    {
        private DateTime currentDate;
        private DateTime lastDate;
        private DataTable gridDt;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            currentDate = CubeInfo.GetLastDate(DataProvidersFactory.SecondaryMASDataProvider, "Oil_0006_0003_lastDate");
            lastDate = new DateTime(currentDate.Year - 1, 12, 30);

            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период].[День]", currentDate, 5);
            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("[Период].[День]", lastDate, 5);

            #region Настройка грида

            GridBrick.BrowserSizeAdapting = false;
            GridBrick.Height = Unit.Empty;
            GridBrick.Width = Unit.Empty;
            GridBrick.RedNegativeColoring = false;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            //GridBrick.Grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);

            #endregion

            GridDataBind();

            lbDescription.Text = String.Format("Розничная цена на нефтепродукты по состоянию на&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>&nbsp;", currentDate);

            UltraChart1.ReportCode = "oil_0006_0003";
            UltraChart2.ReportCode = "oil_0006_0003";
            UltraChart3.ReportCode = "oil_0006_0003";

            UltraChart1.ReportDate = currentDate;
            UltraChart2.ReportDate = currentDate;
            UltraChart3.ReportDate = currentDate;

            UltraChart1.LastDate = lastDate;
            UltraChart2.LastDate = lastDate;
            UltraChart3.LastDate = lastDate;

            UltraChart1.OilName = "Бензин марки АИ-92";
            UltraChart2.OilName = "Бензин марки АИ-95";
            UltraChart3.OilName = "Дизельное топливо";

            UltraChart1.OilId = "2";
            UltraChart2.OilId = "3";
            UltraChart3.OilId = "4";

            UltraChart1.ReportPrefix = "";
            UltraChart2.ReportPrefix = "";
            UltraChart3.ReportPrefix = "";

//            UltraChart1.FoId = HttpContext.Current.Session["CurrentFOID"].ToString();
//            UltraChart2.FoId = HttpContext.Current.Session["CurrentFOID"].ToString();
//            UltraChart3.FoId = HttpContext.Current.Session["CurrentFOID"].ToString();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            gridDt = new DataTable();
            string query = DataProvider.GetQueryText("oil_0006_0003_grid");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", gridDt);

            foreach (DataRow row in gridDt.Rows)
            {
                for (int i = 1; i < gridDt.Columns.Count; i++)
                {
                    if (row[i] != DBNull.Value && row[i].ToString() != String.Empty)
                    {
                        row[i] = row[i].ToString().TrimEnd('_').Replace(".", ",");
                        string gaugeValue = Convert.ToDouble(row[i]).ToString("N2");
                        UltraGauge gauge = GetGauge(gaugeValue);
                        PlaceHolder.Controls.Add(gauge);

                        row[i] = String.Format("<img src='../../../TemporaryImages/Oil_gauge{0}.png'>", gaugeValue);
                    }
                }
            }

            GridBrick.DataTable = gridDt;
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            int columnCount = e.Layout.Bands[0].Columns.Count;
            if (columnCount == 0)
            {
                return;
            }
            
            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = 14;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = 100;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;
            headerLayout.AddCell("Вид топлива");

            for (int i = 1; i < columnCount; i = i + 1)
            {
                string columnCaption = e.Layout.Bands[0].Columns[i].Header.Caption;

                headerLayout.AddCell(columnCaption);

                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(155);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
            headerLayout.ApplyHeaderInfo();
        }

        #endregion

        #region Добавление цифрового гейджа

        private UltraGauge GetGauge(string markerValue)
        {
            UltraGauge gauge = new UltraGauge();
            gauge.Width = 100;
            gauge.Height = 50;
            gauge.DeploymentScenario.FilePath = "../../../TemporaryImages";
            gauge.DeploymentScenario.ImageURL = String.Format("../../../TemporaryImages/Oil_gauge{0}.png", markerValue);

            // Настраиваем гейдж
            SegmentedDigitalGauge SegmentedGauge = new SegmentedDigitalGauge();
            SegmentedGauge.BoundsMeasure = Measure.Percent;
            SegmentedGauge.Digits = 4;
            SegmentedGauge.InnerMarginString = "2, 2, 2, 2, Pixels";
            SegmentedGauge.MarginString = "2, 2, 2, 2, Pixels";
            SegmentedGauge.Text = markerValue.Replace(',', '.');

            SolidFillBrushElement brushUnlit = new SolidFillBrushElement();
            brushUnlit.Color = Color.FromArgb(10, 10, 10, 10);
            SegmentedGauge.UnlitBrushElements.Add(brushUnlit);

            SolidFillBrushElement brushFont = new SolidFillBrushElement();
            brushFont.Color = Color.PaleGoldenrod;
            SegmentedGauge.FontBrushElements.Add(brushFont);

            SolidFillBrushElement brush = new SolidFillBrushElement();
            brush.Color = Color.Black;
            SegmentedGauge.BrushElements.Add(brush);

            gauge.Gauges.Add(SegmentedGauge);
            return gauge;
        }

        #endregion
    }
}

using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0035 : CustomReportPage
    {
        private DataTable gridDt;
        private DateTime currentDate;
        private double gaugeIndicatorValue;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            #region Настройка грида

            GridBrick.HeaderVisible = false;
            GridBrick.Height = CustomReportConst.minScreenHeight - 235;
            GridBrick.Width = 300;
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);

            #endregion

            currentDate = CubeInfoHelper.FoBudgetOutcomesInfo.LastDate;

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodDayFO.Value = currentDate.Day.ToString();

            string regionName = RegionSettingsHelper.Instance.ShortName.Replace("ВологдО", "ВологО");

            Label1.Text = String.Format("Исполнение бюджета {1} по расходам на {0:dd.MM.yyyy} г., млн.руб.", currentDate, regionName);

            GridDataBind();
            GaugeDataBind();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0035_0035");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатели", gridDt);

            if (gridDt.Rows.Count > 1)
            {
                if (gridDt.Rows[0][1] != DBNull.Value && gridDt.Rows[0][1].ToString() != String.Empty)
                {
                    gaugeIndicatorValue = Convert.ToDouble(gridDt.Rows[0][1].ToString());
                }
                gridDt.Rows.RemoveAt(0);

                GridBrick.DataTable = gridDt;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(192);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(75);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
        }

        private void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            string indicatoName = String.Empty;
            if (e.Row.Cells[0].Value != null)
            {
                indicatoName = e.Row.Cells[0].Value.ToString();
                e.Row.Cells[0].Value = ReplaceDateParams(indicatoName);
            }

            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];

                decimal value;
                if (cell.Value != null && Decimal.TryParse(cell.Value.ToString(), out value))
                {
                    cell.Value = Convert.ToDecimal(value).ToString(GetIndicatorFormat(indicatoName));
                }
            }
        }

        private static string GetIndicatorFormat(string name)
        {
            return name.ToLower().Contains("исполнено за год на") ? "P2" : "N2";
        }

        private string ReplaceDateParams(string name)
        {
            name = name.Replace("<месяц>", CRHelper.RusMonth(currentDate.Month));
            name = name.Replace("<прошлый год>", (currentDate.Year - 1) + "&nbsp;г.");
            name = name.Replace("<текущая дата>", String.Format("{0:dd.MM}", currentDate));
            name = name.Replace("<текущая дата прошлого года>", String.Format("{0:dd.MM.yyyy}&nbsp;г.", currentDate.AddYears(-1)));
            name = name.Replace("В том числе", "   в том числе");

            return name;
        }

        #endregion

        #region Обработчики гейджа

        private void GaugeDataBind()
        {
            GaugeMonthLabel.Text = String.Format("Исполнено за {0}", CRHelper.RusMonth(currentDate.Month));
            GaugeValueLabel.Text = String.Format("{0:P2}", gaugeIndicatorValue);

            UltraGauge gauge = GetGauge(100 * gaugeIndicatorValue);
            GaugePlaceHolder.Controls.Add(gauge);
        }

        private static UltraGauge GetGauge(double markerValue)
        {
            UltraGauge gauge = new UltraGauge();
            gauge.Width = 300;
            gauge.Height = 61;
            gauge.DeploymentScenario.FilePath = "../../../TemporaryImages";
            gauge.DeploymentScenario.ImageURL = "../../../TemporaryImages/gauge_imfrf02_01_#SEQNUM(100).png";

            // Настраиваем гейдж
            LinearGauge linearGauge = new LinearGauge();
            linearGauge.CornerExtent = 10;
            linearGauge.MarginString = "2, 10, 2, 10, Pixels";

            // Выбираем максимум шкалы 
            double endValue = (Math.Max(100, markerValue));

            LinearGaugeScale scale = new LinearGaugeScale();
            scale.EndExtent = 98;
            scale.StartExtent = 2;
            scale.OuterExtent = 93;
            scale.InnerExtent = 52;

            SimpleGradientBrushElement gradientBrush = new SimpleGradientBrushElement();
            gradientBrush.EndColor = Color.FromArgb(0, 255, 255, 255);
            gradientBrush.StartColor = Color.FromArgb(120, 255, 255, 255);
            scale.BrushElements.Add(gradientBrush);
            linearGauge.Scales.Add(scale);
            AddMainScale(endValue, linearGauge, markerValue);
            AddMajorTickmarkScale(endValue, linearGauge);
            AddGradient(linearGauge);

            linearGauge.Margin.Top = 1;
            linearGauge.Margin.Bottom = 1;

            gauge.Gauges.Add(linearGauge);
            return gauge;
        }

        private const int ScaleStartExtent = 5;
        private const int ScaleEndExtent = 97;

        private static void AddMajorTickmarkScale(double endValue, LinearGauge linearGauge)
        {
            LinearGaugeScale scale = new LinearGaugeScale();
            scale.EndExtent = ScaleEndExtent;
            scale.StartExtent = ScaleStartExtent;
            scale.MajorTickmarks.EndWidth = 2;
            scale.MajorTickmarks.StartWidth = 2;
            scale.MajorTickmarks.EndExtent = 40;
            scale.MajorTickmarks.StartExtent = 25;
            scale.MajorTickmarks.BrushElements.Add(new SolidFillBrushElement(Color.White));
            scale.Axes.Add(new NumericAxis(0, endValue + endValue / 30, endValue / 10));
            linearGauge.Scales.Add(scale);
        }

        private static void AddGradient(LinearGauge linearGauge)
        {
            SimpleGradientBrushElement gradientBrush = new SimpleGradientBrushElement();
            gradientBrush.EndColor = Color.FromArgb(150, 150, 150);
            gradientBrush.StartColor = Color.FromArgb(10, 255, 255, 255);
            gradientBrush.GradientStyle = Gradient.BackwardDiagonal;
            linearGauge.BrushElements.Add(gradientBrush);
        }

        private static void AddMainScale(double endValue, LinearGauge linearGauge, double markerValue)
        {
            LinearGaugeScale scale = new LinearGaugeScale();
            scale.EndExtent = ScaleEndExtent;
            scale.StartExtent = ScaleStartExtent;
            scale.Axes.Add(new NumericAxis(0, endValue + endValue / 30, endValue / 5));

            AddMainScaleRange(scale, markerValue);
            SetMainScaleLabels(scale);
            linearGauge.Scales.Add(scale);
        }

        private static void SetMainScaleLabels(LinearGaugeScale scale)
        {

            scale.Labels.ZPosition = LinearTickmarkZPosition.AboveMarkers;
            scale.Labels.Extent = 9;
            Font font = new Font("Arial", 9);
            scale.Labels.Font = font;
            scale.Labels.EqualFontScaling = false;
            SolidFillBrushElement solidFillBrushElement = new SolidFillBrushElement(Color.White);
            solidFillBrushElement.RelativeBoundsMeasure = Measure.Percent;
            Rectangle rect = new Rectangle(0, 0, 80, 0);
            solidFillBrushElement.RelativeBounds = rect;
            scale.Labels.BrushElements.Add(solidFillBrushElement);
            scale.Labels.Shadow.Depth = 2;
            scale.Labels.Shadow.BrushElements.Add(new SolidFillBrushElement());
        }

        private static void AddMainScaleRange(LinearGaugeScale scale, double markerValue)
        {
            LinearGaugeRange range = new LinearGaugeRange();
            range.EndValue = markerValue;
            range.StartValue = 0;
            range.OuterExtent = 80;
            range.InnerExtent = 20;
            SimpleGradientBrushElement gradientBrush = new SimpleGradientBrushElement();
            gradientBrush.EndColor = Color.FromArgb(1, 51, 75);
            gradientBrush.StartColor = Color.FromArgb(8, 218, 164);
            gradientBrush.GradientStyle = Gradient.Vertical;
            range.BrushElements.Add(gradientBrush);
            scale.Ranges.Add(range);
        }

        #endregion
    }
}
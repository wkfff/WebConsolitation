using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGauge;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
	public partial class MFRF0201Gadget : GadgetControlBase
	{
		private DataTable dataTable = new DataTable();
        private DataTable dtDate = new DataTable();

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);

			CustomReportPage dashboard = CustumReportPage;

            string query = DataProvider.GetQueryText("MFRF_0002_0001_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            query = DataProvider.GetQueryText("MFRF_0002_0001");
			DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Series", dataTable);

			if (dataTable.Rows.Count > 0)
			{
				foreach(DataRow row in dataTable.Rows)
				{
					// Выбираем целочисленные индикаторы с нормативвом равным нулю
					if (Convert.ToString(row["Единицы измерения"]) == "раз" && 
						Convert.ToDecimal(row["Нормативное значение"]) == 0)
					{
						Label label = new Label();
						//label.Style.Add(HtmlTextWriterStyle.WhiteSpace, "nowrap");
						label.Text = String.Format("{0} - {1}", row["Series"], row["Значение Факт"]);
						mainHTMLTable.Rows[0].Cells[0].Controls.Add(label);
					}
					else
					{
						double marker = Convert.ToDouble(row["Нормативное значение"]);
						double value = Convert.ToDouble(row["Значение Факт"]);
						string code = Convert.ToString(row["Код"]);
						if (!String.IsNullOrEmpty(code))
							code = code.Split(new char[] { ' ', '(' })[0];

						string units = String.Empty;
						if (Convert.ToString(row["Единицы измерения"]) == "тыс. руб")
							units = "тыс.руб.";

						UltraGauge gauge = CreateGauge(value, marker, code, units);
						gauge.ToolTip = String.Format("{0} ({1} {2})", Convert.ToString(row["Series"]), Convert.ToDouble(row["Значение Факт"]).ToString("N"), Convert.ToString(row["Единицы измерения"]));

						mainHTMLTable.Rows[1].Cells[0].Controls.Add(gauge);
					}
				}
			}
			else
			{
				//Dashboards.Trace.TraceWarning(this.ToString(), "Нет данных по блоку MFRF_0002_0001");
			}
		}

		private UltraGauge CreateGauge(double markerValue, double normativeValue, string name, string units)
		{
			UltraGauge gauge = new UltraGauge();
			gauge.BackColor = Color.White;
			gauge.Height = Unit.Pixel(150);
			gauge.Width = Unit.Pixel(80);
			//gauge.Width = Unit.Percentage(100);
			gauge.DeploymentScenario.Mode = ImageDeploymentMode.Session;
						
			LinearGauge linearGauge = gauge.Gauges.AddLinearGauge();
			linearGauge.CornerExtent = 10;
			linearGauge.Orientation = LinearOrientation.Vertical;
			linearGauge.Margin = new Margin(2, 10, 2, 10, Measure.Pixels);

			LinearGaugeScale scale = new LinearGaugeScale();
			scale.EndExtent = 90;
			scale.StartExtent = 25;
			scale.MajorTickmarks.StartWidth = 2;
			scale.MajorTickmarks.EndWidth = 2;
			scale.MajorTickmarks.StartExtent = 40;
			scale.MajorTickmarks.EndExtent = 60;
			scale.MajorTickmarks.StrokeElement.Color = Color.Gray;
			scale.MajorTickmarks.BrushElements.Add(new SolidFillBrushElement(Color.White));
			scale.MinorTickmarks.Frequency = 0.2;
			scale.MinorTickmarks.StartExtent = 45;
			scale.MinorTickmarks.EndExtent = 55;
			scale.MinorTickmarks.StrokeElement.Color = Color.Gray;

			scale.Labels.ZPosition = LinearTickmarkZPosition.AboveMarkers;
			scale.Labels.Extent = 25;
			scale.Labels.FormatString = "<DATA_VALUE:0.##>";
			scale.Labels.Font = new Font("Microsoft Sans Serif", 7);
			scale.Labels.Shadow.Depth = 2;
			scale.Labels.Shadow.BrushElements.Add(new SolidFillBrushElement());
			scale.Labels.BrushElements.Add(new SolidFillBrushElement(Color.Black));

			LinearGaugeBarMarker marker = new LinearGaugeBarMarker();
			marker.SegmentSpan = 1;
			marker.StartExtent = -13;
			marker.BulbSpan = 35;
			marker.Value = 58;
			SimpleGradientBrushElement brush = new SimpleGradientBrushElement();
			brush.GradientStyle = Gradient.Horizontal;
			brush.StartColor = Color.FromArgb(200, 255, 0, 0);
			brush.EndColor = Color.FromArgb(200, 178, 34, 34);
			marker.BrushElements.Add(brush);
			marker.Value = markerValue;
			scale.Markers.Add(marker);

			LinearGaugeNeedle needle = new LinearGaugeNeedle();
			needle.MidExtent = 18;
			needle.EndExtent = 18;
			needle.StartWidth = 1;
			needle.MidWidth = 1;
			needle.EndWidth = 5;
			needle.Value = normativeValue;
			needle.StrokeElement.Color = Color.Blue;
			needle.BrushElements.Add(new SolidFillBrushElement(Color.Blue));
			scale.Markers.Add(needle);

			double topValue = Math.Max(markerValue, normativeValue);
			scale.Axes.Add(new NumericAxis(0, topValue, topValue / 3));

			linearGauge.Scales.Add(scale);

			MultiStopLinearGradientBrushElement mslGradientBrush = new MultiStopLinearGradientBrushElement();
			mslGradientBrush.Angle = 90;
			ColorStop colorStop = new ColorStop();
			colorStop.Color = Color.FromArgb(240, 240, 240);
			mslGradientBrush.ColorStops.Add(colorStop);
			colorStop = new ColorStop();
			colorStop.Color = Color.FromArgb(240, 240, 240);
			colorStop.Stop = (float)0.6791444;
			mslGradientBrush.ColorStops.Add(colorStop);
			colorStop = new ColorStop();
			colorStop.Color = Color.White;
			colorStop.Stop = 1;
			mslGradientBrush.ColorStops.Add(colorStop);
			linearGauge.BrushElements.Add(mslGradientBrush);

			linearGauge.StrokeElement.BrushElements.Add(new SolidFillBrushElement(Color.Silver));

			BoxAnnotation ba = new BoxAnnotation();
			ba.Bounds = new Rectangle(20, 112, 40, 20);
			ba.Label.FormatString = name;
			ba.Label.Font = new Font("Microsoft Sans Serif", 7, FontStyle.Bold);
			ba.Label.BrushElements.Add(new SolidFillBrushElement(Color.FromArgb(51, 51, 51)));
			gauge.Annotations.Add(ba);

			ba = new BoxAnnotation();
			ba.Bounds = new Rectangle(20, 124, 40, 20);
			ba.Label.FormatString = units;
			ba.Label.BrushElements.Add(new SolidFillBrushElement(Color.FromArgb(51, 51, 51)));
			gauge.Annotations.Add(ba);

		    gauge.DeploymentScenario.Mode = ImageDeploymentMode.FileSystem;
		    gauge.DeploymentScenario.FilePath = "../../TemporaryImages";
		    gauge.DeploymentScenario.ImageURL = "../../TemporaryImages/Gaude_mfrf_02_01_gg#SEQNUM(100).png";


			return gauge;
		}

		#region IWebPart Members

		public override string Title
		{
			get { return "Индикаторы БККУ по субъектам"; }
		}

		public override string TitleUrl 
		{ 
			get { return "~/reports/MFRF_0002_0001/Default.aspx"; } 
		}

		#endregion
	}
}
using System;
using System.Drawing;
using System.Web.UI.HtmlControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	public abstract class ChartHelpBase
	{
		protected ChartBase helper;

		public abstract void Init(int chartID, string queryName);
		public virtual void Init(int chartID, string queryName, Type chartClass)
		{
			UltraChart chart = new UltraChart();
			chart.DeploymentScenario.FilePath = "../../../TemporaryImages";
			chart.DeploymentScenario.ImageURL = String.Format("../../../TemporaryImages/Chart_skk{0}#SEQNUM(100).png", chartID);
			
			helper = (ChartBase)Activator.CreateInstance(chartClass, new[]{chart});
		}

		public virtual HtmlGenericControl GetItem()
		{
			HtmlGenericControl item = new HtmlGenericControl("div");
			item.Controls.Add(helper.Chart);
			return item;
		}
	}

	public abstract class ChartBase
	{
		protected const int minRowHeight = 20;
		protected static readonly Font defaultFont = new Font("Verdana", 9);

		public UltraChart Chart { set; get; }
		public double ChartHeight { get; private set; }
		public int LabelsExtent { set; get; }
		public int BottomExtent { set; get; }
		public string TextLabels { set; get; }
		public string TextHints { set; get; }
		public string TextBottom { set; get; }
		public string TitleNoData { set; get; }
		public string MarkFormat { set; get; }
		public string MarkSI { set; get; }

		public abstract void SetStyle();
		public abstract void SetData(string queryName);

		protected ChartBase(UltraChart chart)
		{
			Chart = chart;
			LabelsExtent = 15;
			TitleNoData = "Нет данных";
			MarkFormat = "N0";
			MarkSI = String.Empty;
		}

		public void SetChartHeight(double height)
		{
			ChartHeight = height;
			Chart.Height = CRHelper.GetChartHeight(height);
		}

		public virtual void SetStyle(double width, double height)
		{
			SetChartHeight(height);
			Chart.Width = CRHelper.GetChartWidth(width);
			Chart.EnableViewState = false;

			Chart.Data.ZeroAligned = true;
			Chart.Border.Thickness = 0;
			Chart.Tooltips.Font.Name = "Verdana";
			Chart.Tooltips.Font.Size = 9;

			Chart.TitleBottom.HorizontalAlign = StringAlignment.Center;
			Chart.TitleBottom.VerticalAlign = StringAlignment.Near;
			Chart.TitleBottom.Font = defaultFont;
			Chart.TitleLeft.HorizontalAlign = StringAlignment.Center;
			Chart.TitleLeft.Font = defaultFont;

			Chart.Axis.X.LineThickness = 1;
			Chart.Axis.X.LineColor = Color.DarkGray;
			Chart.Axis.X.Labels.Font = defaultFont;
			Chart.Axis.X.Labels.LabelStyle.Dy = 3;

			Chart.Axis.Y.LineThickness = 1;
			Chart.Axis.Y.LineColor = Color.DarkGray;
			Chart.Axis.Y.Labels.Font = defaultFont;
			Chart.Axis.Y.Labels.LabelStyle.Dx = -5;
			Chart.Axis.Y.Labels.SeriesLabels.Visible = false;

			Chart.InvalidDataReceived += InvalidDataReceived;
		}

		public virtual void InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
		{
			CRHelper.UltraChartInvalidDataReceived(sender, e);
			e.Text = "Нет данных";
		}
		
		protected void SetCommonHorizontalAxis(int xExtent, int yExtent)
		{
			Chart.Axis.X.Visible = true;
			Chart.Axis.X.Extent = xExtent;
			Chart.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";
			Chart.Axis.X.TickmarkStyle = AxisTickStyle.Smart;

			Chart.Axis.X.Margin.Far.Value = LabelsExtent;
			Chart.Axis.X.Margin.Far.MarginType = LocationType.Percentage;

			Chart.Axis.Y.Visible = true;
			Chart.Axis.Y.Extent = yExtent;
			Chart.Axis.Y.Labels.ItemFormatString = "<ITEM_LABEL>";
		}

		protected void SetCommonText(ChartTextCollection text, string format)
		{
			text.Clear();
			text.Add(
				new ChartTextAppearance
				{
					Column = -2,
					Row = -2,
					ItemFormatString = String.Format("<DATA_VALUE:{0}>", format),
					ChartTextFont = defaultFont,
					Visible = true
				});
		}

		protected void SetCommonHorizontalText(ChartTextCollection text, string format)
		{
			SetCommonText(text, format);
			text[0].VerticalAlign = StringAlignment.Center;
			text[0].HorizontalAlign = StringAlignment.Far;
		}

		protected void SetCommonVerticalText(ChartTextCollection text, string format)
		{
			SetCommonText(text, format);
			text[0].VerticalAlign = StringAlignment.Far;
			text[0].HorizontalAlign = StringAlignment.Center;
		}

		protected void SetCompareLegend(int percentage, double marginBottom)
		{
			Chart.Legend.Visible = true;
			Chart.Legend.Location = LegendLocation.Right;
			Chart.Legend.BorderThickness = 0;
			Chart.Legend.SpanPercentage = percentage;
			Chart.Legend.Margins.Bottom = Convert.ToInt32(marginBottom);
			Chart.Legend.Font = defaultFont;
			Chart.Legend.BackgroundColor = Color.White;
		}

		protected void SetCompareColorModel()
		{
			Chart.ColorModel.ModelStyle = ColorModels.CustomLinear;
			Chart.ColorModel.CustomPalette[0] = Color.RoyalBlue;
			Chart.ColorModel.CustomPalette[1] = Color.DarkRed;
		}

		public void SetVerticalAlignCenter(int smallHeight)
		{
			int margin = Convert.ToInt32((Chart.Height.Value - smallHeight) / 2);
			Chart.TitleTop.Visible = true;
			Chart.TitleTop.Text = String.IsNullOrEmpty(Chart.TitleTop.Text) ? " " : Chart.TitleTop.Text;
			Chart.TitleTop.Extent = margin;
			Chart.TitleBottom.Visible = true;
			Chart.TitleBottom.Text = String.IsNullOrEmpty(Chart.TitleBottom.Text) ? " " : Chart.TitleBottom.Text;
			Chart.TitleBottom.Extent = margin;
		}

		public void SetMarginTop(int margin)
		{
			Chart.TitleTop.Visible = true;
			Chart.TitleTop.Text = String.IsNullOrEmpty(Chart.TitleTop.Text) ? " " : Chart.TitleTop.Text;
			Chart.TitleTop.Extent = margin;
		}

		public void SetOuterHeight(HtmlGenericControl htmlPlace)
		{
			htmlPlace.Style.Remove("height");
			htmlPlace.Style.Add("height", Chart.Height.Value + "px");
		}

	}
}

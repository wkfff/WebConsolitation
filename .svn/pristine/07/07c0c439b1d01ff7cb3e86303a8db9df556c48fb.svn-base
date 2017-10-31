using System;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.SkkSupport
{

	public class ChartCmpPeople : ChartCmpBase
	{

		public ChartCmpPeople(UltraChart chart)
			: base(chart)
		{
			MaxValue = 10000000;
		}

		/// <summary>
		/// настройка внешнего вида
		/// </summary>
		public override void SetStyle()
		{
			SetStyle(SKKHelper.default2ItemsWidth, SKKHelper.defaultChartCompareHeight);
		}

		/// <summary>
		/// настройка внешнего вида
		/// </summary>
		public override void SetStyle(double width, double height)
		{
			base.SetStyle(width, height);
			SetCompareVerticalMonthsAxis(60, 70);
			Chart.TitleLeft.Text = "человек";
		}

		/// <summary>
		/// установить данные
		/// </summary>
		/// <param name="queryName"></param>
		public override void SetData(string queryName)
		{

			const string column1 = "\nДосмотрено лиц";
			const string column2 = "Выявлено больных,\nлиц с подозрением\nна инфекционные\nзаболевания";
			SetData(queryName, column1, column2);
		}

		/// <summary>
		/// доп обработка хинтов
		/// </summary>
		protected override void FillSceneGraphBoxHandler(Box box)
		{
			if (box.DataPoint.Label.ToLower().Contains("досмотрено"))
			{
				box.DataPoint.Label = box.DataPoint.Label.Trim();
			}
			else if (box.DataPoint.Label.ToLower().Contains("выявлено"))
			{
				box.DataPoint.Label = box.DataPoint.Label.Replace("\n ", String.Empty).Replace(" \n", String.Empty).Replace("\n", "&nbsp;\n&nbsp;");
			}
		}

	}

	public class ChartCmpTransport : ChartCmpBase
	{

		public ChartCmpTransport(UltraChart chart)
			: base(chart)
		{
			MaxValue = 1000000;
		}

		/// <summary>
		/// настройка внешнего вида
		/// </summary>
		public override void SetStyle()
		{
			SetStyle(SKKHelper.default2ItemsWidth, SKKHelper.defaultChartCompareHeight);
		}

		/// <summary>
		/// настройка внешнего вида
		/// </summary>
		public override void SetStyle(double width, double height)
		{
			base.SetStyle(width, height);
			SetCompareVerticalMonthsAxis(60, 50);
			Chart.TitleLeft.Text = "единиц";
		}

		/// <summary>
		/// установить данные
		/// </summary>
		/// <param name="queryName"></param>
		public override void SetData(string queryName)
		{
			const string column1 = "\nДосмотрено TC\n\n";
			const string column2 = " \nПриостановлен\nвъезд/выезд ТС\n ";
			SetData(queryName, column1, column2);
		}

		/// <summary>
		/// доп обработка хинтов
		/// </summary>
		protected override void FillSceneGraphBoxHandler(Box box)
		{
			if (box.DataPoint.Label.ToLower().Contains("досмотрено"))
			{
				box.DataPoint.Label = box.DataPoint.Label.Trim();
			}
			else if (box.DataPoint.Label.ToLower().Contains("приостановлен"))
			{
				box.DataPoint.Label = box.DataPoint.Label.Replace("\n ", String.Empty).Replace(" \n", String.Empty).Replace("\n", "&nbsp;\n&nbsp;");
			}
		}
	}

	public abstract class ChartCmpBase : ChartBase
	{
		public string Series1Label { set; get; }
		public string Series2Label { set; get; }
		protected string Column1Label { set; get; }
		protected string Column2Label { set; get; }
		protected double MaxValue { set; get; }

		protected abstract void FillSceneGraphBoxHandler(Box box);

		protected ChartCmpBase(UltraChart chart) 
			: base(chart)
		{
			MaxValue = 1000;
		}

		/// <summary>
		/// настройка внешнего вида
		/// </summary>
		public override void SetStyle(double width, double height)
		{
			base.SetStyle(width, height);

			Chart.ChartType = ChartType.ColumnChart;
			
			SetCommonVerticalText(Chart.ColumnChart.ChartText, "N0");
			SetCompareColorModel();
			SetCompareLegend(40, Chart.Height.Value*1/4.4);
			
			Chart.TitleLeft.Visible = true;
			Chart.Tooltips.FormatString = "&nbsp;<ITEM_LABEL>: <b><DATA_VALUE:N0></b>&nbsp;";

			Chart.FillSceneGraph += FillSceneGraph;
		}

		protected void SetCompareVerticalMonthsAxis(int xExtent, int yExtent)
		{
			Chart.Axis.X.Visible = true;
			Chart.Axis.X.Extent = xExtent;
			Chart.Axis.X.Labels.Visible = false;
			Chart.Axis.X.Labels.SeriesLabels.Visible = true;
			Chart.Axis.X.Labels.SeriesLabels.Font = defaultFont;
			Chart.Axis.X.Labels.SeriesLabels.WrapText = true;
			Chart.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.UseCollection;
			Chart.Axis.X.Labels.SeriesLabels.Layout.BehaviorCollection.Add(new ClipTextAxisLabelLayoutBehavior { Trimming = StringTrimming.Word });

			Chart.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			Chart.Axis.X.Margin.Near.Value = 20;
			Chart.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
			Chart.Axis.X.Margin.Far.Value = 20;

			Chart.Axis.Y.Visible = true;
			Chart.Axis.Y.Extent = yExtent;
			Chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
			Chart.Axis.Y.Labels.LabelStyle.Dx = -5;
			Chart.Axis.Y.TickmarkStyle = AxisTickStyle.Smart;

			Chart.Axis.Y.Margin.Far.Value = 10;
			Chart.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;

			Chart.Data.ZeroAligned = false;
			Chart.Axis.Y.SetLogarithmicAxis(MaxValue);
		}

		/// <summary>
		/// установить данные
		/// </summary>
		public void SetData(string queryName, string column1, string column2)
		{
			DataTable dtData = (new Query(queryName)).GetDataTable();
			
			DataTable dtChart = new DataTable();
			dtChart.Columns.Add(new DataColumn("показатель", typeof(string)));
			dtChart.Columns.Add(new DataColumn("отчетный", typeof(int)));
			dtChart.Columns.Add(new DataColumn("сравнение", typeof(int)));

			DataRow row;
			int value;

			// досмотрено
			row = dtChart.NewRow();
			row[0] = column1;
			row[1] = Int32.TryParse(dtData.Rows[0][0].ToString(), out value) ? value : 0;
			row[2] = Int32.TryParse(dtData.Rows[0][2].ToString(), out value) ? value : 0;
			dtChart.Rows.Add(row);

			MaxValue = Chart.Axis.Y.SetLogarithmicScale(Math.Max((int)row[1], (int)row[2]));

			// выявлено
			row = dtChart.NewRow();
			row[0] = column2;
			row[1] = Int32.TryParse(dtData.Rows[0][1].ToString(), out value) ? value : 0;
			row[2] = Int32.TryParse(dtData.Rows[0][3].ToString(), out value) ? value : 0;
			dtChart.Rows.Add(row);

			Chart.Series.Clear();

			NumericSeries series1 = CRHelper.GetNumericSeries(1, dtChart);
			series1.Label = Series1Label;
			Chart.Series.Add(series1);

			NumericSeries series2 = CRHelper.GetNumericSeries(2, dtChart);
			series2.Label = Series2Label;
			Chart.Series.Add(series2);
			
		}

		/// <summary>
		/// доп обработка
		/// </summary>
		private void FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];
				if (primitive is Box)
				{
					Box box = (Box)primitive;
					if (box.DataPoint != null)
					{
						FillSceneGraphBoxHandler(box);
					}
				}
			}
		}
		
	}
}

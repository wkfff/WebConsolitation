using System;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.FO_0002_0002_Settlement.Default.reports.FO_0002_0002_Settlement
{
	/// <summary>
	/// Обертка над диаграммами
	/// </summary>
	public abstract class ChartHelperBase
	{
		protected static readonly Font defaultFont = new Font("Verdana", 9);

		public UltraChart Chart { protected set; get; }
		public DataTable Data { protected set; get; }

		protected string QueryName { set; get; }
		protected DataProvider Provider { set; get; }

		public virtual void Init(UltraChart chart, string queryName, DataProvider provider)
		{
			QueryName = queryName;
			Provider = provider;

			Chart = chart;
			Chart.DataBinding += DataBinding;

			SetStyle();
			Chart.DataBind();
		}

		protected virtual void SetStyle()
		{
			Chart.EnableViewState = false;

			Chart.Data.ZeroAligned = true;
			Chart.Border.Thickness = 0;

			Chart.Axis.X.LineThickness = 1;
			Chart.Axis.X.LineColor = Color.DarkGray;
			Chart.Axis.X.Labels.Font = defaultFont;

			Chart.Axis.Y.LineThickness = 1;
			Chart.Axis.Y.LineColor = Color.DarkGray;
			Chart.Axis.Y.Labels.Font = defaultFont;

			Chart.Tooltips.Font.Name = "Verdana";
			Chart.Tooltips.Font.Size = 9;

			Chart.InvalidDataReceived += CRHelper.UltraChartInvalidDataReceived;
			Chart.FillSceneGraph += FillSceneGraph;
		}

		protected virtual void DataBinding(object sender, EventArgs e)
		{
			Data = DataProvider.GetDataTableForChart(QueryName, Provider);
			if (Data.Rows.Count > 0)
			{
				Chart.Series.Clear();
				Chart.Series.Add(CRHelper.GetNumericSeries(1, Data));
			}
		}

		protected virtual void FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{

		}
	}
}
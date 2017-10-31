using System;
using System.Data;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	// обертка над диаграммой по участкам границы

	public class ChartHelpBordersGoodsTon : ChartHelpBordersBase
	{
		public ChartHelpBordersGoodsTon()
		{
			TextBottom = "тонн";
			TextHints = "Досмотрен объем грузов";
			BottomExtent = 60;
			LabelsExtent = 26;
			MarkFormat = "N3";
			MarkSI = "тонн";
		}
	}

	public class ChartHelpBordersGoods : ChartHelpBordersBase
	{
		public ChartHelpBordersGoods()
		{
			TextBottom = "партий";
			TextHints = "Досмотрено партий грузов";
			BottomExtent = 40;
		}
	}

	public class ChartHelpBordersPeople : ChartHelpBordersBase
	{
		public ChartHelpBordersPeople()
		{
			TextBottom = "человек";
			TextHints = "Досмотрено лиц";
			BottomExtent = 50;
		}
	}

	public class ChartHelpBordersTransport : ChartHelpBordersBase
	{
		public ChartHelpBordersTransport()
		{
			TextBottom = "единиц";
			TextHints = "Досмотрено ТС";
			BottomExtent = 40;
		}
	}

	public abstract class ChartHelpBordersBase : ChartHelpBase
	{
		protected string TextBottom { set; get; }
		protected string TextHints { set; get; }
		protected int BottomExtent { set; get; }
		protected int LabelsExtent { set; get; }
		protected string MarkFormat { set; get; }
		protected string MarkSI { set; get; }

		protected ChartHelpBordersBase()
		{
			LabelsExtent = 22;
			MarkFormat = "N0";
			MarkSI = String.Empty;
		}

		public override void Init(int chartID, string queryName)
		{
			base.Init(chartID, queryName, typeof(ChartHelpBorders));

			helper.TextBottom = TextBottom;
			helper.TextHints = TextHints;
			helper.BottomExtent = BottomExtent;
			helper.LabelsExtent = LabelsExtent;
			helper.MarkFormat = MarkFormat;
			helper.MarkSI = MarkSI;

			helper.SetStyle(SKKHelper.defaultHelpItemWidth, SKKHelper.defaultChartBordersHeight);
			helper.Chart.Width = SKKHelper.defaultHelpItemWidth;
			helper.SetData(queryName);
		}
	}

	public class ChartHelpBorders : ChartBordersBase
	{
		public ChartHelpBorders(UltraChart chart) 
			: base(chart)
		{
			// empty	
		}

		/// <summary>
		/// постобработка данных, обязательно должна выполняться
		/// </summary>
		protected override void DataPostProcessing(DataTable dtChart)
		{
			if (dtChart.Rows.Count > 1)
			{
				dtChart.Rows.RemoveAt(0);
				dtChart.InvertRowsOrder();
				base.DataPostProcessing(dtChart);
			}
			else
			{
				SetChartHeight(SKKHelper.defaultChartNoDataHeight);
			}
		}

		/// <summary>
		/// подгон размеров под данные
		/// </summary>
		protected override void FitHeight(int countRows)
		{
			if (countRows < SKKHelper.maxBordersCount)
			{
				double newHeight = ChartHeight + BottomExtent - 50 - (SKKHelper.maxBordersCount - countRows) * minRowHeight;
				SetChartHeight(newHeight);
			}
		}

	}
}

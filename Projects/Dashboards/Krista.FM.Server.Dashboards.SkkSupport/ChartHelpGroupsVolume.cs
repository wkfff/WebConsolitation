using System;
using System.Data;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	// обертка над диаграммой объемов по группам товаров

	public class ChartHelpGroupsVolume : ChartHelpBase
	{
		public override void Init(int chartID, string queryName)
		{
			base.Init(chartID, queryName, typeof(ChartGroupsVolume));
			
			helper.SetStyle(SKKHelper.defaultHelpItemWidth, SKKHelper.defaultChartHeight);
			helper.Chart.Width = SKKHelper.defaultHelpItemWidth;
			helper.Chart.DoughnutChart.OthersCategoryPercent = 0;

			helper.SetData(queryName);
		}
	}

	public class ChartGroupsVolume : ChartBase
	{
		private DataTable Table { set; get; }

		public ChartGroupsVolume(UltraChart chart) 
			: base(chart)
		{
			// empty
		}

		/// <summary>
		/// настройка внешнего вида
		/// </summary>
		public override void SetStyle()
		{
			SetStyle(SKKHelper.default2ItemsWidth, SKKHelper.defaultChartHeight);
		}

		/// <summary>
		/// настройка внешнего вида
		/// </summary>
		public override void SetStyle(double width, double height)
		{
			base.SetStyle(width, height);

			Chart.ChartType = ChartType.DoughnutChart;

			CRHelper.FillCustomColorModelLight(Chart, 11, true);

            Chart.DoughnutChart.InnerRadius = 40;
			Chart.DoughnutChart.OthersCategoryPercent = 0;
			Chart.DoughnutChart.Labels.Font = defaultFont;
			Chart.DoughnutChart.Labels.FormatString = String.Format("<ITEM_LABEL>\n<DATA_VALUE:N3> тонн\n<PERCENT_VALUE:#0.00>%");
			Chart.Tooltips.FormatString = 
				String.Format(
				"&nbsp;<ITEM_LABEL> товаров,&nbsp;\n" +
				"&nbsp;Запрещено грузов: <b><DATA_VALUE:N3> тонн</b>&nbsp;\n" +
				"&nbsp;<b><PERCENT_VALUE:#0.00>%</b>&nbsp;");
		}

		/// <summary>
		/// установить данные
		/// </summary>
		public override void SetData(string queryName)
		{
			Table = (new Query(queryName)).GetDataTable();
			if (Table.Rows.Count == 0)
			{
				SetChartHeight(SKKHelper.defaultChartNoDataHeight);
				return;
			}
			Chart.Series.Clear();
			Chart.Series.Add(CRHelper.GetNumericSeries(1, Table));
		}

	}
}

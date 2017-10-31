using System;
using System.Data;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	public class ChartTransportPoints : ChartTransportBase
	{
		public ChartTransportPoints(UltraChart chart)
			: base(chart)
		{
			TextLabels = "Пунктов пропуска";
			TextHints = "Пунктов пропуска";
		}
	}

	public abstract class ChartTransportBase : ChartBase
	{
		protected ChartTransportBase(UltraChart chart) 
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

			CRHelper.FillCustomColorModelLight(Chart, 6, true);

			Chart.DoughnutChart.InnerRadius = 40;
			Chart.DoughnutChart.OthersCategoryPercent = 0;
			Chart.DoughnutChart.Labels.Font = defaultFont;
			Chart.DoughnutChart.Labels.FormatString = String.Format("<ITEM_LABEL>\n{0}: <DATA_VALUE:N0>\n<PERCENT_VALUE:#0.00>%", TextLabels);
			Chart.Tooltips.FormatString = String.Format("&nbsp;<ITEM_LABEL>&nbsp;\n&nbsp;{0}: <b><DATA_VALUE:N0></b>&nbsp;\n&nbsp;<b><PERCENT_VALUE:#0.00>%</b>&nbsp;", TextHints);
		}

		/// <summary>
		/// установить данные
		/// </summary>
		/// <param name="queryName"></param>
		public override void SetData(string queryName)
		{
			DataTable dtChart = (new Query(queryName)).GetDataTable();
			
			Chart.Visible = false;
			if (dtChart.Rows.Count > 2)
			{
				dtChart.Rows.RemoveAt(0);

				int positive = 0;
				foreach (DataRow row in dtChart.Rows)
				{
					int value;
					if (Int32.TryParse(row[1].ToString(), out value))
					{
						if(value > 0)
						{
							positive++;
						}
					}
				}

				if (positive > 1)
				{
					Chart.Visible = true;
					Chart.Series.Clear();
					Chart.Series.Add(CRHelper.GetNumericSeries(1, dtChart));
				}
			}
			
		}
	}
}

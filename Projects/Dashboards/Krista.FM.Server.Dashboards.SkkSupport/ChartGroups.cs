using System;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	public class ChartGroups : ChartBase
	{

		public ChartGroups(UltraChart chart)
			: base(chart)
		{
			// empty
		}

		/// <summary>
		/// настройка внешнего вида
		/// </summary>
		public override void SetStyle()
		{
			throw new Exception("Недопустимый вызов");
		}

		/// <summary>
		/// настройка внешнего вида
		/// </summary>
		public void SetStyle(string format)
		{
			base.SetStyle(SKKHelper.default2ItemsWidth, SKKHelper.defaultChartHeight); 
			
			Chart.ChartType = ChartType.BarChart;
			
			SetCommonHorizontalText(Chart.BarChart.ChartText, format);
			SetCommonHorizontalAxis(55, 60);
			
			Chart.TitleBottom.Visible = true;
		}

		/// <summary>
		/// установить данные
		/// </summary>
		public override void SetData(string queryName)
		{
			throw new Exception("Недопустимый вызов");
		}
	}
}

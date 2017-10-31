using Infragistics.WebUI.UltraWebChart;

namespace Krista.FM.Server.Dashboards.Common.Export
{
	/// <summary>
	/// Инкапсулирует параметры экспорта диаграммы
	/// </summary>
	public class ExportChartParams : ParamsMargins
	{
		/// <summary>
		/// Диаграмма отчета
		/// </summary>
		public UltraChart Chart { set; get; }
		
		public double ScaleFactor { set; get; }

		/// <summary>
		/// Инстанцирует класс параметров экспорта диаграммы
		/// </summary>
		public ExportChartParams(UltraChart chart)
		{
			Chart = chart;

			ScaleFactor = 1;
		}

	}
}

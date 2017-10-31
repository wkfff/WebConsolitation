using System.Data;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	/// <summary>
	/// диаграммы по группам товаров
	/// </summary>
	public class ChartGroupsCommon
	{
		public ChartGroups ChartCount { set; get; }
		public ChartGroups ChartVolume { set; get; }

		public ChartGroupsCommon(UltraChart chartCount, UltraChart chartVolume)
		{
			ChartCount = new ChartGroups(chartCount);
			ChartVolume = new ChartGroups(chartVolume);
		}

		/// <summary>
		/// настройка внешнего вида
		/// </summary>
		public virtual void SetStyle()
		{
			ChartCount.LabelsExtent = 10;
			ChartCount.SetStyle("N0");
			CRHelper.FillCustomColorModelLight(ChartCount.Chart, 12, false);
			ChartCount.Chart.TitleBottom.Text = "единиц";
			ChartCount.Chart.Tooltips.FormatString = "&nbsp;<ITEM_LABEL> товаров,&nbsp;\n&nbsp;Досмотрено партий грузов: <b><DATA_VALUE:N0></b>&nbsp;";

			ChartVolume.LabelsExtent = 20;
			ChartVolume.SetStyle("N3");
			CRHelper.CopyCustomColorModelBase(ChartCount.Chart, ChartVolume.Chart, true);
			ChartVolume.Chart.TitleBottom.Text = "тонн";
			ChartVolume.Chart.Tooltips.FormatString = "&nbsp;<ITEM_LABEL> товаров,&nbsp;\n&nbsp;Досмотрен объем грузов: <b><DATA_VALUE:N3> тонн</b>&nbsp;";

		}

		/// <summary>
		/// установить данные
		/// </summary>
		public virtual void SetData(string queryName)
		{
			DataTable dtChart = (new Query(queryName)).GetDataTable();
			
			ChartCount.Chart.Series.Clear();
			ChartCount.Chart.Series.Add(CRHelper.GetNumericSeries(1, dtChart));

			ChartVolume.Chart.Series.Clear();
			ChartVolume.Chart.Series.Add(CRHelper.GetNumericSeries(2, dtChart));
		}
	}
}

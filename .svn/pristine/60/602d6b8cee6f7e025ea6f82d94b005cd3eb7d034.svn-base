using System;
using System.Data;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	// обертка над диаграммой по участкам границы

	public class ChartHelpGroups : ChartHelpBase
	{
		private new ChartGroupsCommon helper;

		public override void Init(int chartID, string queryName)
		{
			UltraChart chartCount = new UltraChart();
			chartCount.DeploymentScenario.FilePath = "../../../TemporaryImages";
			chartCount.DeploymentScenario.ImageURL = String.Format("../../../TemporaryImages/Chart_skk{0}#SEQNUM(100).png", chartID);

			UltraChart chartVolume = new UltraChart();
			chartVolume.DeploymentScenario.FilePath = "../../../TemporaryImages";
			chartVolume.DeploymentScenario.ImageURL = String.Format("../../../TemporaryImages/Chart_skk{0}#SEQNUM(100).png", chartID * 10 + 1);

			helper = new ChartHelpGroupsCommon(chartCount, chartVolume);
			
			helper.SetStyle();
			helper.SetData(queryName);
		}

		public override HtmlGenericControl GetItem()
		{
			HtmlGenericControl item = new HtmlGenericControl("div");
			HtmlGenericControl subitem;

			subitem = new HtmlGenericControl("div");
			subitem.Style.Add("float", "left");
			//subitem.Style.Add("margin-right", "5px");
			subitem.Controls.Add(helper.ChartCount.Chart);
			item.Controls.Add(subitem);

			subitem = new HtmlGenericControl("div");
			subitem.Style.Add("float", "left");
			//subitem.Style.Add("margin-left", "5px");
			subitem.Controls.Add(helper.ChartVolume.Chart);
			item.Controls.Add(subitem);

			subitem = new HtmlGenericControl("div");
			subitem.Style.Add("clear", "both");
			item.Controls.Add(subitem);

			return item;
		}
	}

	public class ChartHelpGroupsCommon : ChartGroupsCommon
	{

		public ChartHelpGroupsCommon(UltraChart chartCount, UltraChart chartVolume)
			: base(chartCount, chartVolume)
		{
			// empty
		}

		/// <summary>
		/// настройка внешнего вида
		/// </summary>
		public override void SetStyle()
		{
			base.SetStyle();

			ChartCount.Chart.Width = Convert.ToInt32(SKKHelper.defaultHelpItemWidth * 0.65);
			ChartVolume.Chart.Width = Convert.ToInt32(SKKHelper.defaultHelpItemWidth * 0.65);
		}

		/// <summary>
		/// установить данные
		/// </summary>
		public override void SetData(string queryName)
		{
			DataTable dtChart = (new Query(queryName)).GetDataTable();

			dtChart.Rows.RemoveAt(0);
			dtChart.InvertRowsOrder();

			ChartCount.Chart.Series.Clear();
			ChartCount.Chart.Series.Add(CRHelper.GetNumericSeries(1, dtChart));

			ChartVolume.Chart.Series.Clear();
			ChartVolume.Chart.Series.Add(CRHelper.GetNumericSeries(2, dtChart));
		}
	}
	
}

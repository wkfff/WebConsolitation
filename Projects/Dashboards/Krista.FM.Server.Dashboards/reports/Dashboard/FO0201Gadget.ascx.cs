using System;
using System.Data;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
	public partial class FO0201Gadget : GadgetControlBase
	{
		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);

			DataTable dt1 = new DataTable();
			string query = DataProvider.GetQueryText("FO_0002_0001_1");
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dt1);
			Grid.Rows[0].Cells[1].Value = String.Format("{0}%", (Convert.ToDouble(dt1.Rows[0][0]) * 100).ToString("N"));
            Grid.Rows[0].Cells[2].Value = String.Format("{0} {1}%", dt1.Rows[0][2], (Convert.ToDouble(dt1.Rows[0][3]) * 100).ToString("N"));
            Grid.Rows[0].Cells[3].Value = String.Format("{0} {1}%", dt1.Rows[0][4], (Convert.ToDouble(dt1.Rows[0][5]) * 100).ToString("N"));

			DataTable dt2 = new DataTable();
			query = DataProvider.GetQueryText("FO_0002_0001_2");
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dt2);
			Grid.Rows[1].Cells[1].Value = String.Format("{0}%", (Convert.ToDouble(dt2.Rows[0][0]) * 100).ToString("N"));
            Grid.Rows[1].Cells[2].Value = String.Format("{0} {1}%", dt2.Rows[0][2], (Convert.ToDouble(dt2.Rows[0][3]) * 100).ToString("N"));
            Grid.Rows[1].Cells[3].Value = String.Format("{0} {1}%", dt2.Rows[0][4], (Convert.ToDouble(dt2.Rows[0][5]) * 100).ToString("N"));

			DataTable dt3 = new DataTable();
			query = DataProvider.GetQueryText("FO_0002_0001_3");
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dt3);
			Grid.Rows[2].Cells[1].Value = Convert.ToDouble(dt3.Rows[0][0]).ToString("N");
            Grid.Rows[2].Cells[2].Value = String.Format("{0} {1}", dt3.Rows[0][1], Convert.ToDouble(dt3.Rows[0][2]).ToString("N"));
            Grid.Rows[2].Cells[3].Value = String.Format("{0} {1}", dt3.Rows[0][3], Convert.ToDouble(dt3.Rows[0][4]).ToString("N"));

			DataTable dt4 = new DataTable();
			query = DataProvider.GetQueryText("FO_0002_0001_4");
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dt4);
			Grid.Rows[3].Cells[1].Value = Convert.ToDouble(dt4.Rows[0][0]).ToString("N");
			Grid.Rows[3].Cells[2].Value = String.Format("{0} {1}", dt4.Rows[0][1], Convert.ToDouble(dt4.Rows[0][2]).ToString("N"));
            Grid.Rows[3].Cells[3].Value = String.Format("{0} {1}", dt4.Rows[0][3], Convert.ToDouble(dt4.Rows[0][4]).ToString("N"));

            Grid.DisplayLayout.Bands[0].Columns[0].Width = 135;
            Grid.DisplayLayout.Bands[0].Columns[1].Width = 90;
            Grid.DisplayLayout.Bands[0].Columns[2].Width = 90;
            Grid.DisplayLayout.Bands[0].Columns[3].Width = 90;
		}

		#region IWebPart Members

		public override string Title
		{
			get { return "Исполнение бюджетов МО"; }
		}

		public override string TitleUrl
		{
			get { return "~/reports/ФО_0002_0001/Default.aspx"; }
		}

		#endregion
	}
}
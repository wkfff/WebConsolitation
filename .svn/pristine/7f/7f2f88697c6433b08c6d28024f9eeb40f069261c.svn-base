using System;
using System.Data;
using System.Globalization;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
	public partial class UFK1701Gadget : GadgetControlBase
	{
		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);

			CustomReportPage dashboard = CustumReportPage;

			try
			{
                DataTable dtDate = new DataTable();
                string query = DataProvider.GetQueryText("UFK_0017_0001_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

                Label3.Text = string.Format("данные на {0} {1} {2} года", dtDate.Rows[0][4], CRHelper.RusMonthGenitive(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())), dtDate.Rows[0][0]);

                DataTable dt = new DataTable();
				query = DataProvider.GetQueryText("UFK_0017_0001");
				DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dt);

				char percentGroupSeparator = CultureInfo.CurrentCulture.NumberFormat.PercentGroupSeparator[0];
				Label1.Text = String.Format(" {0} тыс.руб.", (Convert.ToDouble(dt.Rows[0][0]) / 1000).ToString("N").Replace(percentGroupSeparator, '\''));
				Label2.Text = String.Format(" {0} тыс.руб.", (Convert.ToDouble(dt.Rows[0][1]) / 1000).ToString("N").Replace(percentGroupSeparator, '\''));
			}
			catch (Exception ex)
			{
				Trace.Warn(this.ToString(), "Ошибка", ex);
			}
		}

		#region IWebPart Members

		public override string Title
		{
			get { return "Остатки средств"; }
		}

		public override string TitleUrl
		{
			get { return "~/reports/UFK_0017_0001/Default_target.aspx"; }
		}

		#endregion
	}
}
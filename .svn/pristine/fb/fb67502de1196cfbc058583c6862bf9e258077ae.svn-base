using System;
using System.Data;
using System.Configuration;
using System.Drawing;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.reports.Dashboard;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
	public partial class FNS0101Gadget : GadgetControlBase
	{
		private DataTable dataTable = new DataTable();
        private DataTable dtDate = new DataTable();
	    private DateTime dateCurrentYear;

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);

			Grid.DataBind();
		}

		protected void Grid_DataBinding(object sender, EventArgs e)
		{
			CustomReportPage dashboard = CustumReportPage;

            string query = DataProvider.GetQueryText("FNS_0001_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            int monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString()) + 1;
            if (monthNum > 12)
            {
                monthNum = 1;
                yearNum++;
            }

            dateCurrentYear = new DateTime(yearNum, monthNum, 1);

            dashboard.UserParams.PeriodMonth.Value = string.Format("{0}", dtDate.Rows[0][4]);
            dashboard.UserParams.PeriodYear.Value = (Convert.ToInt32(dtDate.Rows[0][0]) - 1).ToString();
    
	    	query = DataProvider.GetQueryText("FNS_0001_0001");
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Доходы", dataTable);

            Label1.Text = string.Format("Средний темп прироста: {0:P2}", dataTable.Rows[0][4]);
            Label2.Text = string.Format("данные на 1 {0} {1} года", CRHelper.RusMonthGenitive(monthNum), yearNum);

			foreach (DataRow row in dataTable.Rows)
			{
			    if (row[1] != DBNull.Value)
			    {
			        row[1] = Convert.ToDouble(row[1])/1000;
			    }
			    if (row[2] != DBNull.Value)
			    {
			        row[2] = Convert.ToDouble(row[2])*100;
			    }
                if (row[3] != DBNull.Value)
                {
                    row[3] = Convert.ToDouble(row[3]) / 1000;
                }
			}

			Grid.DataSource = dataTable;
		}

		protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
		{
            e.Layout.GroupByBox.Hidden = true;

            if (e.Layout.Bands.Count == 0)
                return;

            if (e.Layout.Bands[0].Columns.Count > 4)
            {
                e.Layout.Bands[0].Columns[4].Hidden = true;

                Grid.DisplayLayout.Bands[0].Columns[0].CellStyle.BackColor = Color.FromArgb(239, 243, 251);

                e.Layout.Bands[0].Columns[0].Header.Caption = "Регион (ГО)";
                e.Layout.Bands[0].Columns[1].Header.Caption = string.Format("Недоимка на {0:dd.MM.yyyy}, тыс. руб.", dateCurrentYear);
                e.Layout.Bands[0].Columns[2].Header.Caption = "Темп прироста недоимки с начала года, %";
                e.Layout.Bands[0].Columns[3].Header.Caption = "Прирост, тыс. руб.";

                e.Layout.Bands[0].Columns[0].Width = 110;
                e.Layout.Bands[0].Columns[1].Width = 95;
                e.Layout.Bands[0].Columns[2].Width = 95;
                e.Layout.Bands[0].Columns[3].Width = 95;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            }
        }

		protected void Grid_InitializeRow(object sender, RowEventArgs e)
		{
			if (Convert.ToDouble(e.Row.Cells[3].Value) > 1)
			{
				e.Row.Cells[2].Style.BackgroundImage = "~/images/ArrowUpRed.gif";
			}
			else if (Convert.ToDouble(e.Row.Cells[3].Value) < 1)
			{
				e.Row.Cells[2].Style.BackgroundImage = "~/images/ArrowDownGreen.gif";
			}
            e.Row.Cells[2].Style.CustomRules = "background-repeat: no-repeat; background-position: left; margin-left: 5px";
		}

		#region IWebPart Members

		public override string Title
		{
			get { return "Прирост недоимки"; }
		}

		public override string TitleUrl
		{
			get { return "~/reports/FNS_0001_0001/Default.aspx"; }
		}

		#endregion
	}
}
using System;
using System.Data;
using System.Drawing;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
	public partial class UFK0801Gadget : GadgetControlBase
	{
	    private CustomReportPage dashboard;
	    private int monthNum = 1;
	    private DateTime dateCurrentYear;
        private DateTime dateLastYear;

        protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);

			Grid.DataBind();
		}

		protected void Grid_DataBinding(object sender, EventArgs e)
		{
            dashboard = CustumReportPage;

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("UFK_0008_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
		    monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            dateCurrentYear = new DateTime(yearNum, monthNum, Convert.ToInt32(dtDate.Rows[0][4]));
            dateLastYear = new DateTime(yearNum - 1, monthNum, Convert.ToInt32(dtDate.Rows[0][4]));

            dashboard.UserParams.PeriodYear.Value = yearNum.ToString();
            dashboard.UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            dashboard.UserParams.PeriodMonth.Value = string.Format("[{0}].[{1}].[{2}]",
                                                               dtDate.Rows[0][1],
                                                               dtDate.Rows[0][2],
                                                               dtDate.Rows[0][3]);
            dashboard.UserParams.PeriodCurrentDate.Value = string.Format("[{0}].[{1}].[{2}].[{3}]",
                                                               dtDate.Rows[0][1],
                                                               dtDate.Rows[0][2],
                                                               dtDate.Rows[0][3],
                                                               dtDate.Rows[0][4]);

            Label1.Text = string.Format("данные на {0} {1} {2} года", dtDate.Rows[0][4], CRHelper.RusMonthGenitive(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())), dtDate.Rows[0][0]);

			DataTable dt = new DataTable();
			query = DataProvider.GetQueryText("UFK_0008_0001");
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Доходы", dt);
			Grid.DataSource = dt.DefaultView;
		}

		protected void Grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
		{
			Grid.DisplayLayout.GroupByBox.Hidden = true;

			if (Grid.DisplayLayout.Bands.Count == 0)
				return;

			if (Grid.DisplayLayout.Bands[0].Columns.Count > 3)
			{
				Grid.DisplayLayout.Bands[0].Columns[1].Header.Caption = string.Format("Темп роста плановый {0} к {1}",
                                                                                      dashboard.UserParams.PeriodYear.Value,
                                                                                      dashboard.UserParams.PeriodLastYear.Value);
				Grid.DisplayLayout.Bands[0].Columns[2].Header.Caption = string.Format("Темп роста плановый {2} {0} к {3} {1}",
                                                                                      dashboard.UserParams.PeriodYear.Value,
                                                                                      dashboard.UserParams.PeriodLastYear.Value,
                                                                                      CRHelper.RusMonth(monthNum),
                                                                                      CRHelper.RusMonthDat(monthNum));
                Grid.DisplayLayout.Bands[0].Columns[3].Header.Caption = string.Format("Темп роста фактический {0:dd.MM.yyyy} к {1:dd.MM.yyyy}",
                                                                                      dateCurrentYear,
                                                                                      dateLastYear);

				CRHelper.FormatNumberColumn(Grid.DisplayLayout.Bands[0].Columns[1], "P2");
				CRHelper.FormatNumberColumn(Grid.DisplayLayout.Bands[0].Columns[2], "P2");
				CRHelper.FormatNumberColumn(Grid.DisplayLayout.Bands[0].Columns[3], "P2");

				Grid.DisplayLayout.Bands[0].Columns[0].CellStyle.BackColor = Color.FromArgb(239, 243, 251);

				Grid.DisplayLayout.Bands[0].Columns[0].Width = 110;
				Grid.DisplayLayout.Bands[0].Columns[1].Width = 85;
				Grid.DisplayLayout.Bands[0].Columns[2].Width = 85;
				Grid.DisplayLayout.Bands[0].Columns[3].Width = 85;
			}
		}

		protected void Grid_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
		{
			if (Convert.ToDouble(e.Row.Cells[3].Value) < Convert.ToDouble(e.Row.Cells[1].Value))
			{
				e.Row.Cells[3].Style.BackgroundImage = "~/images/Red.png";
			}
			else if (Convert.ToDouble(e.Row.Cells[3].Value) > Convert.ToDouble(e.Row.Cells[1].Value))
			{
				e.Row.Cells[3].Style.BackgroundImage = "~/images/Green.png";
			}
            e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: left; margin-left: 5px";
		}

		#region IWebPart Members

		public override string Title
		{
			get { return "Темп роста налоговых доходов"; }
		}

		public override string TitleUrl
		{
			get { return "~/reports/UFK_0008_0001/Default.aspx"; }
		}

		#endregion
	}
}
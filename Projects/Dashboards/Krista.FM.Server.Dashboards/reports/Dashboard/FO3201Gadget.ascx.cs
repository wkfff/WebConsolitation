﻿using System;
using System.Data;
using Infragistics.UltraGauge.Resources;
using System.Globalization;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
	public partial class FO3201Gadget : GadgetControlBase
	{
		protected override void Page_Load(object sender, EventArgs e)
		{
            base.Page_Load(sender, e);

            CustomReportPage dashboard = CustumReportPage;

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0032_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            dashboard.UserParams.PeriodDayFO.Value = string.Format("{0}", dtDate.Rows[0][5]);
            dashboard.UserParams.PeriodYear.Value =
                string.Format("[Период].[Год Квартал Месяц].[Данные всех периодов].[{0}]", dtDate.Rows[0][0]);
            dashboard.UserParams.PeriodMonth.Value =
                string.Format("[Период].[Год Квартал Месяц].[Данные всех периодов].[{0}].[{1}].[{2}].[{3}]",
                dtDate.Rows[0][0], dtDate.Rows[0][1], dtDate.Rows[0][2], dtDate.Rows[0][3]);

            DataTable dt = new DataTable();
            query = DataProvider.GetQueryText("FO_0032_0001");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dt);

            int gaudeEndValue = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(dt.Rows[0][3]) * 100 / 50)) * 50;
            gaudeEndValue = gaudeEndValue < 100 ? 100 : gaudeEndValue;

            ((RadialGauge)Gauge.Gauges[0]).Scales[0].Axis.SetEndValue(gaudeEndValue);
            ((RadialGauge)Gauge.Gauges[0]).Scales[0].Markers[0].Value = Convert.ToDouble(dt.Rows[0][3]) * 100;

			Label1.Text = String.Format("{0} тыс.руб.", (Convert.ToDouble(dt.Rows[0][2]) / 1000).ToString("N"));
			Label2.Text = String.Format("{0} тыс.руб.", (Convert.ToDouble(dt.Rows[0][1]) / 1000).ToString("N"));
			Label3.Text = String.Format("{0} тыс.руб.", (Convert.ToDouble(dt.Rows[0][0]) / 1000).ToString("N"));
            Label4.Text = String.Format("{0} тыс.руб.", (Convert.ToDouble(dt.Rows[0][5]) / 1000).ToString("N"));
            Label5.Text = String.Format("{0} тыс.руб.", (Convert.ToDouble(dt.Rows[0][4]) / 1000).ToString("N"));
            Label6.Text = String.Format("{0}", (Convert.ToDouble(dt.Rows[0][6])).ToString("P2"));

            Label1_1.Text = String.Format("План на {0}:", CRHelper.RusMonth(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));
            Label2_1.Text = String.Format("Всего за {0}:", CRHelper.RusMonth(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));
            Label3_1.Text = String.Format("в том числе {0} {1}:", dtDate.Rows[0][4], CRHelper.RusMonthGenitive(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));
            Label4_1.Text = String.Format("План на {0} год:", dtDate.Rows[0][0]);
            Label5_1.Text = String.Format("Всего за {0} год:", dtDate.Rows[0][0]);
            Label6_1.Text = String.Format("Исполнено за {0} год:", dtDate.Rows[0][0]);

            topLabel.Text = string.Format("данные на {0} {1} {2} года", dtDate.Rows[0][4], CRHelper.RusMonthGenitive(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())), dtDate.Rows[0][0]);
            completeLabel.Text = string.Format("Исполнено {0:P2}", Convert.ToDouble(dt.Rows[0][3]));
            completeLabel.Font.Size = FontUnit.Parse("18px");
		}

		#region IWebPart Members

		public override string Title
		{
			get { return "Доходы областного бюджета"; }
		}

		public override string TitleUrl
		{
			get { return "~/reports/FO_0032_0001/Default.aspx"; }
		}

		#endregion
	}
}
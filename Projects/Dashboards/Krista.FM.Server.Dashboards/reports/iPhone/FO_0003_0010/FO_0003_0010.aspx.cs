using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image = System.Web.UI.WebControls.Image;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0003_0010 : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("fo_0003_0001_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "дата", dtDate);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][1].ToString();
            DateTime date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);

            UserParams.PeriodYear.Value = date.Year.ToString();
            UserParams.PeriodLastYear.Value = date.AddYears(-1).Year.ToString();
            CustomParam periodLastLastYear = UserParams.CustomParam("period_last_last_year");
            CustomParam periodThreeYearAgo = UserParams.CustomParam("period_3_last_year");
            CustomParam periodFourYearAgo = UserParams.CustomParam("period_4_last_year");
            periodLastLastYear.Value = date.AddYears(-2).Year.ToString();
            periodThreeYearAgo.Value = date.AddYears(-3).Year.ToString();
            periodFourYearAgo.Value = date.AddYears(-4).Year.ToString();

            lbDescription.Text = GetIndicatorText();
        }

        private string GetIndicatorText()
        {
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("fo_0003_0001_241");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель ", dt);

            switch (dt.Rows[0]["Внимание "].ToString())
            {
                case "0":
                    { return String.Empty; }
                case "1":
                    { return String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Наблюдается рост расходов по статье «Безвозмездные перечисления государственным и муниципальным предприятиям (КОСГУ 241)» и рост расходов по статьям «Заработная плата и начисления на неё (КОСГУ 211, 213)», «Оплата коммунальных услуг (КОСГУ 223)», «Услуги связи (КОСГУ 221)» и «Арендная плата за пользование имуществом (КОСГУ 224)».<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Темп к прошлому году («Безвозмездные перечисления государственным и муниципальным предприятиям (КОСГУ 241)»)&nbsp;<b>{0:P2}</b><br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Темп к прошлому году по показателям Заработная плата и начисления на неё (КОСГУ 211, 213)», «Оплата коммунальных услуг (КОСГУ 223)», «Услуги связи (КОСГУ 221)» и «Арендная плата за пользование имуществом (КОСГУ 224)»&nbsp;<b>{1:P2}</b>»", dt.Rows[0]["Темп к прошлому году по безвозмездным"], dt.Rows[0]["Темп к прошлому году "]); }
                case "2":
                    { return String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Рост расходов по статье «Безвозмездные перечисления государственным и муниципальным предприятиям (КОСГУ 241)» превышает снижение расходов по статьям «Заработная плата и начисления на неё (КОСГУ 211, 213)», «Оплата коммунальных услуг (КОСГУ 223)», «Услуги связи (КОСГУ 221)» и «Арендная плата за пользование имуществом (КОСГУ 224)»<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Темп к прошлому году («Безвозмездные перечисления государственным и муниципальным предприятиям (КОСГУ 241)»)&nbsp;<b>{0:P2}</b><br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Темп к прошлому году по показателям Заработная плата и начисления на неё (КОСГУ 211, 213)», «Оплата коммунальных услуг (КОСГУ 223)», «Услуги связи (КОСГУ 221)» и «Арендная плата за пользование имуществом (КОСГУ 224)»&nbsp;<b>{1:P2}</b>»", dt.Rows[0]["Темп к прошлому году по безвозмездным"], dt.Rows[0]["Темп к прошлому году "]); }
                case "3":
                    { return String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Наблюдается снижение расходов по статье «Безвозмездные перечисления государственным и муниципальным предприятиям (КОСГУ 241)» и снижение расходов по статьям «Заработная плата и начисления на неё (КОСГУ 211, 213)», «Оплата коммунальных услуг (КОСГУ 223)», «Услуги связи (КОСГУ 221)» и «Арендная плата за пользование имуществом (КОСГУ 224)»<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Темп к прошлому году («Безвозмездные перечисления государственным и муниципальным предприятиям (КОСГУ 241)»)&nbsp;<b>{0:P2}</b><br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Темп к прошлому году по показателям Заработная плата и начисления на неё (КОСГУ 211, 213)», «Оплата коммунальных услуг (КОСГУ 223)», «Услуги связи (КОСГУ 221)» и «Арендная плата за пользование имуществом (КОСГУ 224)»&nbsp;<b>{1:P2}</b>»", dt.Rows[0]["Темп к прошлому году по безвозмездным"], dt.Rows[0]["Темп к прошлому году "]); }
            }

            return String.Empty;
        }
    }
}

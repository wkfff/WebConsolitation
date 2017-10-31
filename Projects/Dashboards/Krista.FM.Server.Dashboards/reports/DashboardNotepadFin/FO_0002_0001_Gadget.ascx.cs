using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.DashboardNotepadFin.Dashboard.reports.DashboardNotepadFin
{
    public partial class FO_0002_0001_Gadget : GadgetControlBase
    {
        CustomReportPage dashboard;
        private int monthNum;
        private int yearNum;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            dashboard = GetCustomReportPage(this);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0001_incomes_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            //period_day_fk
            //[Период].[Период].[Данные всех периодов].[2010].[Полугодие 2].[Квартал 4].[Октябрь]
            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            DateTime CurrentDate = new DateTime(yearNum, monthNum, 1);
            dashboard.UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName("", CurrentDate, 4);
            dashboard.UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("", CurrentDate.AddYears(-1), 4);
            dashboard.UserParams.PeriodLastYear.Value = CRHelper.PeriodMemberUName("", new DateTime(yearNum - 1, 12, 1), 4);

            Label1.Text = string.Format("данные за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            UltraWebGrid1.DataBinding += new EventHandler(UltraWebGrid1_DataBinding);
            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);
            UltraWebGrid1.DataBind();

            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid1.Height = Unit.Empty;

            HyperLink1.Text = "Темп роста доходов";
            HyperLink2.Text = "Исполнение доходов";
            HyperLink3.Text = "Подробнее доходы области";
            HyperLink1.NavigateUrl = "~/reports/FO_0002_0003_Vologda/DefaultCompare.aspx";
            HyperLink2.NavigateUrl = "~/reports/FO_0002_0005_Vologda/DefaultCompare.aspx";
            HyperLink3.NavigateUrl = "~/reports/FO_0002_0003/DefaultDetail.aspx";
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            SetConditionBall(e, 3, true);
            SetConditionArrow(e, 5);
        }

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].HeaderStyle.Wrap = true;
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "P2");

            e.Layout.Bands[0].Columns[0].Width = 100;
            e.Layout.Bands[0].Columns[1].Width = 80;
            e.Layout.Bands[0].Columns[2].Width = 90;
            e.Layout.Bands[0].Columns[3].Width = 80;
            e.Layout.Bands[0].Columns[4].Width = 80;
            e.Layout.Bands[0].Columns[5].Width = 80;

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "Показатели", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Назначено, млн.руб.", "План на год");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, String.Format("Исполнено на<br/>1 {0}, млн.руб.", CRHelper.RusMonthGenitive(monthNum + 1)), "Фактическое исполнение нарастающим итогом с начала года");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "% исполнения", "Процент выполнения назначений. Оценка равномерности исполнения (1/12 годового плана в месяц)");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "% наполнения бюджета", "Процент наполнения бюджета за аналогичный период прошлого года");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "Темп роста %", "Темп роста исполнения к аналогичному периоду предыдущего года");
        }

        void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0001_grid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dt);

            UltraWebGrid1.DataSource = dt;
        }

        private void SetConditionBall(RowEventArgs e, int index, bool directAssesment)
        {
            if (e.Row.Cells[index] != null && e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string positiveImg = directAssesment ? "~/images/ballGreenBB.png" : "~/images/ballRedBB.png";
                string negativeImg = directAssesment ? "~/images/ballRedBB.png" : "~/images/ballGreenBB.png";
                string img;
                string title;
                if (value < CommonAssessionLimit())
                {
                    img = negativeImg;
                    title = "Не соблюдается условие равномерности";
                }
                else
                {
                    img = positiveImg;
                    title = "Соблюдается условие равномерности";
                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 2px center; padding-left: 2px";
                e.Row.Cells[index].Title = title;
            }
        }

        public static void SetConditionArrow(RowEventArgs e, int index)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string img = string.Empty;
                string title = string.Empty;
                if (value > 1)
                {
                    img = "~/images/arrowGreenUpBB.png";
                    title = "Рост к прошлому году";
                }
                else if (value < 1)
                {
                    img = "~/images/arrowRedDownBB.png";
                    title = "Снижение к прошлому году";
                }

                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 2px center; padding-left: 0px";
                e.Row.Cells[index].Title = title;
            }
        }

        /// <summary>
        /// Пороговое значение оценки остальных.
        /// </summary>
        /// <returns></returns>
        private double CommonAssessionLimit()
        {
            return 1.0 / 12.0 * (Double)monthNum;
        }

        #region IWebPart Members

        public override string Description
        {
            get { return ""; }
        }

        public override string Title
        {
            get { return "Поступление доходов"; }
        }

        public override string TitleUrl
        {
            get { return "~/reports/FO_0002_0003/DefaultDetail.aspx"; }
        }

        #endregion
    }
}
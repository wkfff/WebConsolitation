using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.DashboardNotepadFin
{
    public partial class FO_0002_0002_Gadget : GadgetControlBase
    {
        private DataTable dtGrid = new DataTable();

        CustomReportPage dashboard;
        private int monthNum;
        private int yearNum;
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            dashboard = GetCustomReportPage(this);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0002_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            //period_day_fk
            //[Период].[Период].[Данные всех периодов].[2010].[Полугодие 2].[Квартал 4].[Октябрь]
            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            DateTime CurrentDate = new DateTime(yearNum, monthNum, 1);
            dashboard.UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName("", CurrentDate, 4);
            dashboard.UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("", CurrentDate.AddYears(-1), 4);
            Label1.Text = string.Format("данные за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            query = DataProvider.GetQueryText("FO_0002_0002_incomes");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Имя", dtGrid);

            dtGrid.Rows.RemoveAt(0);

            query = DataProvider.GetQueryText("FO_0002_0002_outcomes");
            DataTable dtGrid2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Имя", dtGrid2);

            foreach (DataRow row in dtGrid2.Rows)
            {
                dtGrid.ImportRow(row);
            }

            query = DataProvider.GetQueryText("FO_0002_0002_deficite");
            dtGrid2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Имя", dtGrid2);

            foreach (DataRow row in dtGrid2.Rows)
            {
                dtGrid.ImportRow(row);
            }

            query = DataProvider.GetQueryText("FO_0002_0002_finsources");
            dtGrid2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Имя", dtGrid2);

            foreach (DataRow row in dtGrid2.Rows)
            {
                dtGrid.ImportRow(row);
            }

            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);

            UltraWebGrid1.DataSource = dtGrid;
            UltraWebGrid1.DataBind();

            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid1.Height = Unit.Empty;

            HyperLink1.Text = "Исполнение бюджета в разрезе отдельных показателей";
            HyperLink2.Text = "Динамика исполнения бюджета в разрезе отдельных показателей";
            HyperLink3.Text = "Подробнее исполнение бюджета";
            HyperLink1.NavigateUrl = "~/reports/FO_0002_0013_01/";
            HyperLink2.NavigateUrl = "~/reports/FO_0002_0013_02/";
            HyperLink3.NavigateUrl = "~/reports/FO_0002_0001/default.aspx";
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            SetConditionBall(e, 3, true);
            SetConditionBall(e, 4, true);
            SetConditionArrow(e, 6);
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
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P2");

            e.Layout.Bands[0].Columns[0].Width = 130;
            e.Layout.Bands[0].Columns[1].Width = 77;
            e.Layout.Bands[0].Columns[2].Width = 87;
            e.Layout.Bands[0].Columns[3].Width = 77;
            e.Layout.Bands[0].Columns[4].Width = 77;
            e.Layout.Bands[0].Columns[5].Width = 87;
            e.Layout.Bands[0].Columns[6].Width = 77;

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "Показатели", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Назначено, млн.руб.", "План на год");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, String.Format("Исполнено на 1 {0}, млн.руб.", CRHelper.RusMonthGenitive(monthNum + 1)), "Фактическое исполнение нарастающим итогом с начала года");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "% исполнения", "Процент выполнения назначений. Оценка равномерности исполнения (1/12 годового плана в месяц)");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "% исполнения прошлый год", "Процент выполнения назначений в прошлом году. Оценка равномерности исполнения (1/12 годового плана в месяц)");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "Исполнено прошлый год, млн. руб.", "Фактическое исполнение нарастающим итогом с начала прошлого года");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, "Темп роста %", "Темп роста исполнения к аналогичному периоду предыдущего года");
        }

        #region IWebPart Members

        public override string Description
        {
            get { return "Показатели исполнения бюджета"; }
        }

        public override string Title
        {
            get { return "Показатели исполнения бюджета"; }
        }

        public override string TitleUrl
        {
            get { return "~/reports/FO_0002_0001/default.aspx"; }
        }

        #endregion
    }
}
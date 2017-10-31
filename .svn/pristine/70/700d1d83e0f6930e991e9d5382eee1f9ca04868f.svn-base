using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.DashboardNotepadFin
{
    public partial class FO_0002_0006_Gadget : GadgetControlBase
    {
        private int endYear;
        private int monthNum;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0008_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            endYear = Convert.ToInt32(dtDate.Rows[0][0]);
            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());

            CustomReportPage dashboard = GetCustomReportPage(this);
            DateTime CurrentDate = new DateTime(endYear, monthNum, 1);
            dashboard.UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName("", CurrentDate, 4);
                        
            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);
            UltraWebGrid1.DataBinding += new EventHandler(UltraWebGrid1_DataBinding);
            UltraWebGrid1.DataBind();

            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid1.Height = Unit.Empty;

            topLabel.Text = string.Format("данные за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), endYear);

            HyperLink1.Text = "Структура расходов";
            HyperLink2.Text = "Исполнение расходов";
            HyperLink3.Text = "Подробнее расходы области";
            HyperLink1.NavigateUrl = "~/reports/FO_0002_0008/";
            HyperLink2.NavigateUrl = "~/reports/FO_0002_0007/DefaultCompare.aspx";
            HyperLink3.NavigateUrl = "~/reports/FO_0002_0006/DefaultDetail.aspx";
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
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "P2");

            e.Layout.Bands[0].Columns[0].Width = 150;
            e.Layout.Bands[0].Columns[1].Width = 75;
            e.Layout.Bands[0].Columns[2].Width = 75;
            e.Layout.Bands[0].Columns[3].Width = 85;
            e.Layout.Bands[0].Columns[4].Width = 85;
            e.Layout.Bands[0].Columns[5].Width = 75;

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "Показатели", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Назначено, млн.руб.", "План на год");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, String.Format("Исполнено на<br/>1 {0}, млн.руб.", CRHelper.RusMonthGenitive(monthNum + 1)), "Фактическое исполнение нарастающим итогом с начала года");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "% исполнения", "Процент выполнения назначений. Оценка равномерности исполнения (1/12 годового плана в месяц)");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "Исполнено прошлый год, млн. руб", "Фактическое исполнение нарастающим итогом с начала прошлого года");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "Темп роста %", "Темп роста исполнения к аналогичному периоду предыдущего года");
        }

        void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            string query = Session["rzprType"].ToString() == "rzpr" ? DataProvider.GetQueryText("FO_0002_0006_gadget_grid_fkr") : DataProvider.GetQueryText("FO_0002_0006_gadget_grid_kosgu");
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

        /// <summary>
        /// Пороговое значение оценки остальных.
        /// </summary>
        /// <returns></returns>
        private double CommonAssessionLimit()
        {
            return 1.0 / 12.0 * (Double)monthNum;

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

        #region IWebPart Members

        public override string Description
        {
            get { return "Расходы бюджета"; }
        }

        public override string Title
        {
            get { return "Расходы бюджета"; }
        }

        public override string TitleUrl
        {
            get { return "~/reports/FO_0002_0006/DefaultDetail.aspx"; }
        }

        #endregion
    }
}
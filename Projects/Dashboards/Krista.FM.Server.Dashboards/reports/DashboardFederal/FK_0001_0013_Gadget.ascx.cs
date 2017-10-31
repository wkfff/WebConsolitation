using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.DashboardFederal.Dashboard.reports.DashboardFederal
{
    public partial class FK_0001_0013_Gadget : GadgetControlBase
    {
        private DataTable dt;
        private int monthNum;
        private int year;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (String.IsNullOrEmpty(UserParams.Region.Value) ||
                String.IsNullOrEmpty(UserParams.StateArea.Value))
            {
                UserParams.Region.Value = "Дальневосточный федеральный округ";
                UserParams.StateArea.Value = "Камчатский край";
            }

            HyperLink1.Text = "Темп роста доходов";
            HyperLink1.NavigateUrl = CustumReportPage.GetReportFullName("ФК_0001_0004_01");

            HyperLink2.Text = "Отдельные показатели исполнения бюджетов";
            HyperLink2.NavigateUrl = TitleUrl;

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("iPad_0001_0001_incomes_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            year = Convert.ToInt32(dtDate.Rows[0][0].ToString());
            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());

            Label1.Text = String.Format("данные за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), year);

            UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
            UserParams.PeriodLastYear.Value = dtDate.Rows[0][0].ToString();
            UserParams.PeriodYear.Value = (Convert.ToInt32(dtDate.Rows[0][0]) + 1).ToString();
            UltraWebGridBudget.DisplayLayout.RowStyleDefault.Height = 10;
            InitializeBudget();
        }

        #region Бюджет

        private void InitializeBudget()
        {
            UltraWebGridBudget.Width = Unit.Empty;
            UltraWebGridBudget.Height = Unit.Empty;

            UltraWebGridBudget.DataBind();
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();

            string query = DataProvider.GetQueryText("iPad_0001_0001_budget_incomes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);

            DataTable source = new DataTable();

            query = DataProvider.GetQueryText("iPad_0001_0001_budget_outcomes_all");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", source);

            foreach (DataRow row in source.Rows)
            {
                DataRow newRow = dt.NewRow();
                for (int i = 0; i < source.Columns.Count; i++)
                {
                    newRow[i] = row[i];
                }
                dt.Rows.Add(newRow);
            }

            source = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_budget_outcomes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", source);

            foreach (DataRow row in source.Rows)
            {
                DataRow newRow = dt.NewRow();
                for (int i = 0; i < source.Columns.Count; i++)
                {
                    newRow[i] = row[i];
                }
                dt.Rows.Add(newRow);
            }

            source = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_budget_deficite");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", source);

            foreach (DataRow row in source.Rows)
            {
                DataRow newRow = dt.NewRow();
                for (int i = 0; i < source.Columns.Count; i++)
                {
                    newRow[i] = row[i];
                }
                dt.Rows.Add(newRow);
            }
            UltraWebGridBudget.DataSource = dt;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;

            if (e.Layout.Bands.Count == 0)
                return;

            e.Layout.Bands[0].Columns[0].Width = 200;
            if (Request.UserAgent.Contains("Chrome"))
            {
                e.Layout.Bands[0].Columns[1].Width = 85;
                e.Layout.Bands[0].Columns[2].Width = 85;
            }
            else
            {
                e.Layout.Bands[0].Columns[1].Width = 75;
                e.Layout.Bands[0].Columns[2].Width = 75;
            }
            e.Layout.Bands[0].Columns[3].Width = 75;
           

            for (int i = 1; i <= e.Layout.Bands[0].Columns.Count - 1; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = false;
            }
            
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].HeaderStyle.Height = 10;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            e.Layout.Bands[0].Columns[1].Header.Caption = String.Format("за {0} {1} {2}г. млн.руб.", monthNum, CRHelper.RusManyMonthGenitive(monthNum), year - 1);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("за {0} {1} {2}г. млн.руб.", monthNum, CRHelper.RusManyMonthGenitive(monthNum), year);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            e.Layout.Bands[0].Columns[3].Header.Caption = "Темп роста %";

            e.Layout.Bands[0].Columns[4].Hidden = true;
            e.Layout.Bands[0].Columns[5].Hidden = true;

            e.Layout.RowStyleDefault.Padding.Top = 1;
            e.Layout.RowStyleDefault.Padding.Bottom = 1;
            e.Layout.RowStyleDefault.Padding.Left = 1;
            e.Layout.RowStyleDefault.Padding.Right = 1;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[0].Style.CustomRules = "padding-top: 1px";
            e.Row.Cells[1].Style.CustomRules = "padding-top: 1px";
            e.Row.Cells[2].Style.CustomRules = "padding-top: 1px";
            e.Row.Cells[3].Style.CustomRules = "padding-top: 1px";

            if (e.Row.Cells[0].Value.ToString().ToLower() != "итого доходов " &&
                e.Row.Cells[0].Value.ToString().ToLower() != "итого расходов " &&
                e.Row.Cells[0].Value.ToString().ToLower() != "профицит(+)/дефицит(-) ")
            {
                e.Row.Cells[0].Style.Padding.Left = 10;
            }
            if (e.Row.Cells[3] != null &&
                e.Row.Cells[3].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[3].Value.ToString());
                string img;
                string title;
                if (value > 100)
                {
                    img = "~/images/arrowGreenUpBB.png";
                    title = "Рост к прошлому году";
                }
                else
                {
                    img = "~/images/arrowRedDownBB.png";
                    title = "Падение к прошлому году";
                }
                e.Row.Cells[3].Style.BackgroundImage = img;
                e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; padding-top: 0px";
                e.Row.Cells[3].Title = title;
            }

            if (e.Row.Cells[0].Value.ToString().ToLower() == "профицит(+)/дефицит(-) ")
            {
                if (e.Row.Cells[1] != null &&
                e.Row.Cells[1].Value != null)
                {
                    double value = Convert.ToDouble(e.Row.Cells[1].Value.ToString());
                    string img;
                    string title;
                    if (value > 0)
                    {
                        img = "~/images/CornerGreen.gif";
                        title = "Профицит";
                    }
                    else
                    {
                        img = "~/images/CornerRed.gif";
                        title = "Дефицит";
                    }
                    e.Row.Cells[1].Style.BackgroundImage = img;
                    e.Row.Cells[1].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; padding-top: 1px; padding-right: 4px";
                    e.Row.Cells[1].Title = title;
                }

                if (e.Row.Cells[2] != null &&
                e.Row.Cells[2].Value != null)
                {
                    double value = Convert.ToDouble(e.Row.Cells[2].Value.ToString());
                    string img;
                    string title;
                    if (value > 0)
                    {
                        img = "~/images/CornerGreen.gif";
                        title = "Профицит";
                    }
                    else
                    {
                        img = "~/images/CornerRed.gif";
                        title = "Дефицит";
                    }
                    e.Row.Cells[2].Style.BackgroundImage = img;
                    e.Row.Cells[2].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; padding-top: 1px; padding-right: 4px";
                    e.Row.Cells[2].Title = title;
                }
            }
        }

        #endregion

        #region IWebPart Members

        public override string Description
        {
            get { return ""; }
        }

        public override string Title
        {
            get { return "Основные показатели"; }
        }

        public override string TitleUrl
        {
            get { return "~/reports/FK_0001_0013/Default.aspx"; }
        }

        #endregion
    }
}
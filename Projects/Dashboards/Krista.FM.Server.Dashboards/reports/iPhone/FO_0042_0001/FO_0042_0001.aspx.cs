using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0042_0001 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();

        // выбранный период
        private CustomParam selectedPeriod;

        private DateTime date;
        private GridHeaderLayout headerLayout;

        protected override void Page_Load(object sender, EventArgs e)
        {
            HeraldImageContainer.InnerHtml = "<img style='margin-left: -30px; margin-right: 20px; height: 65px' src=\"../../../images/Heralds/77.png\">";

            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");

            #endregion

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("fo_0042_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            int endYear = Convert.ToInt32(dtDate.Rows[0][0]);
            int quarterNum = Convert.ToInt32(dtDate.Rows[0][2].ToString().Replace("Квартал ", string.Empty));
            string quarter = dtDate.Rows[0][2].ToString();

            date = new DateTime(endYear, CRHelper.QuarterLastMonth(quarterNum), 1);
            //date = new DateTime(2010, 12, 1);
            string periodDescription = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;по состоянию на&nbsp;<b><span class='DigitsValue'>{0:dd.MM.yyyy}</span></b>",
                                               date.AddMonths(1));

            UserParams.PeriodYear.Value = endYear.ToString();
            UserParams.PeriodHalfYear.Value = dtDate.Rows[0][1].ToString();
            UserParams.PeriodQuater.Value = quarter;

            selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);

            BestGRBSGrid.Width = Unit.Empty;
            BestGRBSGrid.Height = Unit.Empty;
            BestGRBSGrid.DisplayLayout.NoDataMessage = "Нет данных";

            WorseGRBSGrid.Width = Unit.Empty;
            WorseGRBSGrid.Height = Unit.Empty;
            WorseGRBSGrid.DisplayLayout.NoDataMessage = "Нет данных";

            BestGRBSGrid.Bands.Clear();
            BestGRBSGrid.DataBind();

            WorseGRBSGrid.Bands.Clear();
            WorseGRBSGrid.DataBind();

            BestGRBSGrid.Columns.RemoveAt(1);
            WorseGRBSGrid.Columns.RemoveAt(1);

            lbDescription.Text =
                string.Format(
                    "Результаты мониторинга качества финансового менеджмента, осуществляемого<br/>главными распорядителями средств бюджета автономного округа, главными администраторами доходов бюджета автономного округа, в соответствии<br/>с Приказом Департамента финансов ХМАО-Югры от 31.03.2011 №66-о<br/>{0}", periodDescription);

            MakeHtmlTableDetailTable();

            query = DataProvider.GetQueryText("fo_0042_0001_avg");
            DataTable dtData = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование ГРБС", dtData);

            double markerValue = Convert.ToDouble(dtData.Rows[0][0]);

            UltraGauge1.DeploymentScenario.FilePath = "../../../TemporaryImages";
            UltraGauge1.DeploymentScenario.ImageURL = String.Format("../../../TemporaryImages/FO_0042_0001_gauge{0}.png", markerValue);

            LinearGauge gauge = (LinearGauge)(UltraGauge1.Gauges[0]);
            gauge.Scales[0].Markers[0].Value = markerValue;

            IPadElementHeader4.Text = "Среднее значение итоговой оценки по всем ГРБС";
            lbAvgValue.Text = markerValue.ToString("N2");
        }

        private void MakeHtmlTableDetailTable()
        {
            detailTable.CssClass = "HtmlTableCompact";

            string query = DataProvider.GetQueryText("fo_0042_0001_group_avg");
            DataTable dtData = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование ГРБС", dtData);

            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                TableRow row = new TableRow();
                TableCell cell;

                cell = GetNameCell(dtData.Rows[i][0].ToString());
                row.Cells.Add(cell);

                cell = GetValueCell(string.Format("{0:N2}", dtData.Rows[i][1]));
                row.Cells.Add(cell);

                detailTable.Rows.Add(row);
            }
        }

        private TableCell GetValueCell(string value)
        {
            TableCell cell;
            Label lb;
            cell = new TableCell();
            cell.CssClass = "HtmlTableCompact";
            lb = new Label();
            lb.Text = value;
            lb.CssClass = "TableFont";
            cell.Controls.Add(lb);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Right;
            cell.Style.Add("padding-right", "3px");
            cell.Style.Add("padding-top", "5px");
            cell.Style.Add("padding-bottom", "5px");
            return cell;
        }

        private TableCell GetNameCell(string name)
        {
            TableCell cell;
            Label lb;
            cell = new TableCell();
            cell.CssClass = "HtmlTableCompact";
            lb = new Label();
            lb.Text = name;
            lb.CssClass = "TableFont";
            cell.Style.Add("padding-left", "3px");
            cell.Controls.Add(lb);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Left;
            cell.Style.Add("padding-top", "6px");
            cell.Style.Add("padding-bottom", "6px");
            return cell;
        }

        #region Обработчики грида

        protected void GRBSGrid_DataBinding(object sender, EventArgs e)
        {
            string query = sender == BestGRBSGrid
                ? DataProvider.GetQueryText("fo_0042_0001_bestGRBSGrid")
                : DataProvider.GetQueryText("fo_0042_0001_worseGRBSGrid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование ГРБС", dtGrid);
            foreach (DataRow dataRow in dtGrid.Rows)
            {
                dataRow[0] = dataRow[0].ToString().Replace("Ханты-Мансийского автономного округа – Югры", "ХМАО-Югры").Replace("Ханты-Мансийскому автономному округу – Югре", "ХМАО-Югре");
            }
            
            ((UltraWebGrid)sender).DataSource = dtGrid.Rows.Count > 0 ? dtGrid : null;
        }

        protected void GRBSGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            e.Layout.Bands[0].Columns[0].Width = 600;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = 55;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "000");

            e.Layout.Bands[0].Columns[2].Width = 155;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N1");

            headerLayout = new GridHeaderLayout(e.Layout.Grid);

            headerLayout.AddCell("Наименование ГРБС");
            headerLayout.AddCell("Код ГРБС");
            headerLayout.AddCell("Итоговая оценка");

            headerLayout.ApplyHeaderInfo();
        }

        protected void GRBSGrid_InitializeRow(object sender, RowEventArgs e)
        {

        }

        #endregion
    }
}

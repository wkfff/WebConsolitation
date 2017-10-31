using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.iPadBricks.iPadBricks;
using System.Drawing;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class Food_0001_0002_v : CustomReportPage
    {

        private static DataTable dtGrid;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            InitializeDate();

            DataLoad();

            tbHeader.Width = "3000px";
            MakeTable(tbHeader, dtGrid);

            tbFooter.Width = "3000px";
            MakeTable(tbFooter, dtGrid);

            UltraWebGridtbData.Width = "3000px";
            MakeTable(UltraWebGridtbData, dtGrid);

            UltraWebGrid1.DataSource = GetSmallTable();
            UltraWebGrid1.DataBind();

        }

        private DateTime currDate;
        private DateTime lastDate;

        private void InitializeDate()
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Food_0001_0002_incomes_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, " ", dtDate);

            UserParams.PeriodLastDate.Value = dtDate.Rows[0]["ДанныеНа"].ToString();
            UserParams.PeriodCurrentDate.Value = dtDate.Rows[1]["ДанныеНа"].ToString();

            currDate = CRHelper.DateByPeriodMemberUName(UserParams.PeriodCurrentDate.Value, 3);
            lastDate = CRHelper.DateByPeriodMemberUName(UserParams.PeriodLastDate.Value, 3);
        }

        #region Грид

        private void MakeTable(HtmlTable table, DataTable dtGrid)
        {
            // Создание заголовка
            HtmlTableRow row = new HtmlTableRow();
            row.Attributes.Add("Class", "HtmlTableHeader");
            row.Style.Add("text-align", "center");
            row.Height = "75px";
            foreach (DataColumn column in dtGrid.Columns)
            {
                HtmlTableCell cell = new HtmlTableCell();
                cell.Style.Add("border-color", "#3c3c3c");
                if (column.Caption == "Продукт")
                    cell.Width = "360px";
                else
                    cell.Width = "120px";
                cell.InnerHtml = column.Caption.Replace("Город ", String.Empty).Replace("муниципальный район", "МР").
                    Replace("Нижневартовск", "Нижне-<br/>вартовск").Replace("Нефтеюганск", "Нефте-<br/>юганск") + GetStarOrNot(column.Caption);
                row.Cells.Add(cell);
            }
            table.Rows.Add(row);

            // Заполнение данными
            for (int i = 0; i < dtGrid.Rows.Count; i += 3)
            {
                row = new HtmlTableRow();
                row.Style.Add("text-align", "right");
                HtmlTableCell cell = new HtmlTableCell();
                cell.Attributes.Add("Class", "HtmlTableHeader");
                cell.Style.Add("border-color", "#3c3c3c");
                cell.Style.Add("text-align", "left");
                cell.InnerHtml = String.Format("{0}", dtGrid.Rows[i][0]);
                row.Cells.Add(cell);
                for (int j = 1; j < dtGrid.Columns.Count; ++j)
                {
                    cell = new HtmlTableCell();

                    string cost = "-", delta = "-", growth = "-";

                    if (MathHelper.IsDouble(dtGrid.Rows[i][j]))
                        cost = String.Format("{0:N2}", Convert.ToDouble(dtGrid.Rows[i][j]));
                    if (MathHelper.IsDouble(dtGrid.Rows[i + 1][j]))
                    {
                        delta = String.Format("{0:N2}", Convert.ToDouble(dtGrid.Rows[i + 1][j]));
                        if (MathHelper.AboveZero(dtGrid.Rows[i + 1][j]))
                        {
                            cell.Attributes.Add("Class", "ArrowUpRed");
                        }
                        else if (MathHelper.SubZero(dtGrid.Rows[i + 1][j]))
                        {
                            cell.Attributes.Add("Class", "ArrowDownGreen");
                        }
                    }
                    if (MathHelper.IsDouble(dtGrid.Rows[i + 2][j]))
                        growth = String.Format("{0:P2}", Convert.ToDouble(dtGrid.Rows[i + 2][j]));

                    cell.InnerHtml = String.Format("{0}<br/>{1}<br/>{2}", cost, delta, growth);

                    row.Cells.Add(cell);
                }
                table.Rows.Add(row);
            }
        }

        private void MakeHeader(Table table)
        {
            TableHeaderRow headerRow = new TableHeaderRow();
            headerRow.BackColor = Color.Gray;
            headerRow.Height = Unit.Parse("75px");
            for (int i = 0; i < dtGrid.Columns.Count; ++i)
            {
                DataColumn column = dtGrid.Columns[i];
                TableHeaderCell cell = new TableHeaderCell();
                cell.Text = column.Caption.Replace("Город ", String.Empty).Replace("муниципальный район", "МР").
                    Replace("Нижневартовск", "Нижне-вартовск").Replace("Нефтеюганск", "Нефте-юганск") + GetStarOrNot(column.Caption);
                if (i == 0)
                {
                    cell.Width = Unit.Parse("300px");
                }
                else
                {
                    cell.Width = Unit.Parse("100px");
                }
                headerRow.Cells.Add(cell);
            }
            table.Rows.Add(headerRow);
        }

        private void MakeDataTable(Table table)
        {
            foreach (DataRow dataRow in dtGrid.Rows)
            {
                TableRow row = new TableRow();
                row.Height = Unit.Parse("33px");
                if (dtGrid.Rows.IndexOf(dataRow) % 3 == 0)
                {
                    TableCell cell = new TableCell();
                    cell.Text = String.Format("{0}", dataRow[0]);
                    cell.RowSpan = 3;
                    cell.Width = Unit.Parse("300px");
                    row.Cells.Add(cell);
                }
                for (int i = 1; i < dtGrid.Columns.Count; ++i)
                {
                    TableCell cell = new TableCell();

                    if (MathHelper.IsDouble(dataRow[i]))
                    {
                        if (dtGrid.Rows.IndexOf(dataRow) % 3 == 2)
                        {
                            if (MathHelper.AboveZero(dataRow[i]))
                                cell.CssClass = "ArrowUpRed";
                            else if (MathHelper.SubZero(dataRow[i]))
                                cell.CssClass = "ArrowDownGreen";
                            cell.Text = String.Format("{0:P2}", Convert.ToDouble(dataRow[i]));
                        }
                        else
                            cell.Text = String.Format("{0:N2}", Convert.ToDouble(dataRow[i]));
                    }
                    else
                    {
                        cell.Text = String.Format("{0}", dataRow[i]);
                    }
                    row.Cells.Add(cell);
                }
                table.Rows.Add(row);
            }
        }

        private void DataLoad()
        {
            dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("Food_0001_0002_v_grid_stat");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Продукт", dtGrid);
            foreach (DataRow row in dtGrid.Rows)
                row["Продукт"] = row["Продукт"].ToString().Split(';')[0];
            for (int i = 1; i < dtGrid.Columns.Count; ++i)
            {
                DataColumn column = dtGrid.Columns[i];
                string moName = column.ColumnName;
                if (IsStatMO(moName))
                    continue;
                UserParams.Mo.Value = moName;
                DataTable dtEcon = new DataTable();
                query = DataProvider.GetQueryText("Food_0001_0002_v_grid_econ");
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Продукт", dtEcon);
                for (int econRowIndex = 0; econRowIndex < dtEcon.Rows.Count - 1; econRowIndex += 2)
                {
                    object currCost = DBNull.Value, lastCost = DBNull.Value;
                    if (moName == "Белоярский муниципальный район")
                    {
                        object beloyarCurrCost = dtGrid.Rows[econRowIndex / 2 * 3 + 0][i];
                        object beloyarLastCost = MathHelper.Minus(dtGrid.Rows[econRowIndex / 2 * 3 + 0][i], dtGrid.Rows[econRowIndex / 2 * 3 + 1][i]);
                        currCost = MathHelper.GeoMean(dtEcon.Rows[econRowIndex], 1, 1, DBNull.Value, beloyarCurrCost);
                        lastCost = MathHelper.GeoMean(dtEcon.Rows[econRowIndex + 1], 1, 1, DBNull.Value, beloyarLastCost);
                    }
                    else
                    {
                        currCost = MathHelper.GeoMean(dtEcon.Rows[econRowIndex], 1, 1, DBNull.Value);
                        lastCost = MathHelper.GeoMean(dtEcon.Rows[econRowIndex + 1], 1, 1, DBNull.Value);
                    }
                    object delta = MathHelper.Minus(currCost, lastCost);
                    object grow = MathHelper.Grown(currCost, lastCost);
                    dtGrid.Rows[econRowIndex / 2 * 3 + 0][i] = currCost;
                    dtGrid.Rows[econRowIndex / 2 * 3 + 1][i] = delta;
                    dtGrid.Rows[econRowIndex / 2 * 3 + 2][i] = grow;
                }
            }
            dtGrid.AcceptChanges();
        }

        #endregion

        private string GetStarOrNot(string moName)
		{
			if (moName.Contains("Советский") || moName.Contains("Сургут") || moName.Contains("Нефтеюганск") || moName.Contains("Когалым") ||
				moName.Contains("Лангепас") || moName.Contains("Мегион") || (moName.Contains("Нижневартовск") && !moName.Contains("Нижневартовский")) ||
				moName.Contains("Нягань") || moName.Contains("Пыть-Ях") || moName.Contains("Югорск") || moName.Contains("ХМАО") || moName.Contains("Продукт"))
			{
				return String.Empty;
			}
			else
			{
				return "*";
			}
		}
		
		private bool IsStatMO(string moName)
		{
			if (moName == "Город Сургут" || moName == "Город Ханты-Мансийск" || moName == "Город Нижневартовск" ||
                moName == "Ханты-Мансийский автономный округ")
				return true;
			else
				return false;
		}

        private DataTable GetSmallTable()
        {
            DataTable table = new DataTable();

            table.Columns.Add("id", typeof(int));
            DataRow row = table.NewRow();
            row["id"] = 0;
            table.Rows.Add(row);

            return table;
        }

    }

}

using System;
using System.Data;

using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using Image = System.Web.UI.WebControls.Image;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0002_0065 : CustomReportPage
    {
        private DataTable gridDt = new DataTable();
        private DataTable gridDate = new DataTable();
        private DataTable gridAverrage = new DataTable();
        private Dictionary<string, string> localSettlementUniqueNames = new Dictionary<string, string>();
        private int j = 0;
        private DataTable gridDetailTables = new DataTable();
        private DataTable gridIndicators = new DataTable();
        private double avverage = 0, avverageProfit = 0;
        HtmlTable tableRegion = new HtmlTable();

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            string query = DataProvider.GetQueryText("FO_0002_0065_date");
            gridDate = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, gridDate);
            UserParams.PeriodYear.Value = gridDate.Rows[0][0].ToString();
            UserParams.PeriodHalfYear.Value = gridDate.Rows[0][1].ToString();
            UserParams.PeriodQuater.Value = gridDate.Rows[0][2].ToString();
            UserParams.PeriodMonth.Value = gridDate.Rows[0][3].ToString();
            string quarter = UserParams.PeriodQuater.Value;
            quarter = quarter.Replace("Квартал", "");

            Title.Text = "Исполнение бюджетов поселений Астраханской области по состоянию на&nbsp;";
            string sYear = UserParams.PeriodYear.Value;
            if ((CRHelper.MonthNum(UserParams.PeriodMonth.Value) + 1) == 13)
            {
                sYear = (Convert.ToInt16(UserParams.PeriodYear.Value) + 1).ToString();
                SubTitle.Text = string.Format("01.01.{0}", sYear);

            }
            else
            {
                DateTime datetime = new DateTime(Convert.ToInt16(UserParams.PeriodYear.Value), CRHelper.MonthNum(UserParams.PeriodMonth.Value) + 1, 1);
                SubTitle.Text = datetime.ToString("dd.MM.yyyy");
            }
            SubTitle.Style.Add("font-weight", "bold");
            SubTitle.Style.Add("color", "White");
            string query2 = DataProvider.GetQueryText("FO_0002_0065_grid");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query2, "Наименование", gridDt);
            gridDt.Columns.RemoveAt(0);


            HtmlTableRow htmlRow = new HtmlTableRow();
            HtmlTableCell htmlCell = new HtmlTableCell();

            tableRegion = new HtmlTable();

            for (int i = 0; i < gridDt.Rows.Count; i++)
            {
                HtmlTableRow row = new HtmlTableRow();
                HtmlTableCell cell1 = new HtmlTableCell();
                HtmlTableCell cell2 = new HtmlTableCell();
                UserParams.RegionDimension.Value = gridDt.Rows[i][1].ToString();
                string queryaverrage = DataProvider.GetQueryText("FO_0002_0065_Average");
                gridAverrage = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(queryaverrage, "Наименование", gridAverrage);
                GetAverrage(out avverageProfit, out avverage);
                cell1 = GetNameCellFirstLevel(i, gridDt, false);
                cell2 = GetListRegion(gridDt.Rows[i][1].ToString(), i);
                row.Cells.Add(cell1);
                row.Cells.Add(cell2);
                cell1.Style.Add("background-image", "url(../../../images/CollapseIpad.png)");
                cell1.Style.Add("background-repeat", "no-repeat;");
                cell1.Style.Add("background-position", "0px 5px");
                cell1.Style.Add("font-size", "50px");
                cell1.Attributes.Add("onclick", "resize(this, 'table" + i + "')");
                cell1.Style.Add("padding-left", "20px");
                cell1.Style.Add("padding-bottom", "5px");
                cell1.Style.Add("color", "White");
                cell1.Style.Add("vertical-align", "top");
                tableRegion.Style.Add("padding-left", "10px");
                tableRegion.Style.Add("padding-top", "20px");
                tableRegion.Rows.Add(row);
            }
            PlaceHolder1.Controls.Add(tableRegion);
        }


        private HtmlTableCell GetListRegion(string RegionUniqueName, int index)
        {
            HtmlTable tableRegion1 = new HtmlTable();
            HtmlTableCell cell = new HtmlTableCell();
            UserParams.RegionDimension.Value = RegionUniqueName;
            string query3 = DataProvider.GetQueryText("FO_0002_0065_gridTables");
            gridDetailTables = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query3, "Наименование", gridDetailTables);
            tableRegion1 = new HtmlTable();
            tableRegion1.ID = String.Format("table{0}", index);
            tableRegion1.Style.Add("display", "none");
            tableRegion1.Style.Add("padding-left", "10px");
            tableRegion1.Style.Add("margin-top", "20px");
            tableRegion1.Style.Add("margin-left", "-250px");
            for (int i = 0; i < gridDetailTables.Rows.Count; i++)
            {
                HtmlTableRow row = new HtmlTableRow();
                HtmlTableCell cell1 = new HtmlTableCell();
                HtmlTableCell cell2 = new HtmlTableCell();
                cell1 = GetNameCellFirstLevel(i, gridDetailTables, true);
                cell2 = GetTableGroup(gridDetailTables.Rows[i][2].ToString(), i);
                row.Cells.Add(cell1);
                row.Cells.Add(cell2);

                tableRegion1.Rows.Add(row);

            }
            cell.Controls.Add(tableRegion1);
            return cell;
        }

        /*  #region div green возвращает див с зеленым кружком
          private HtmlGenericControl GetDivGreen()
          {

              HtmlGenericControl divGreen = new HtmlGenericControl("div");

              divGreen.ID = String.Format("TagCloud_tag{0}_{1}", this.ID, j);

              image.ImageUrl = "../../../images/ballGreenBB.png";
              image.Style.Add("width", "20px");
              image.Style.Add("padding-left", "40px");
              if (image.ImageUrl != String.Empty)
              {
                  divGreen.Controls.Add(image);
              }
              TooltipHelper.AddToolTip(divGreen, "Процент исполнения выше среднего по поселениям", this.Page);
              j++;
              return divGreen;

          }
          #endregion*/

        /*  #region div green возвращает див с красным кружком
          private HtmlGenericControl GetDivRed()
          {
              HtmlGenericControl divRed = new HtmlGenericControl("div");
              divRed.ID = String.Format("TagCloud_tag{0}_{1}", this.ID, j);
              image.ImageUrl = "../../../images/ballRedBB.png";
              image.Style.Add("width", "20px");
              image.Style.Add("padding-left", "40px");
              if (image.ImageUrl != String.Empty)
              {
                  divRed.Controls.Add(image);
              }
              TooltipHelper.AddToolTip(divRed, "Процент исполнения ниже среднего по поселениям", this.Page);
              j++;
              return divRed;
          }
          #endregion*/

        #region считаем среднее по районам
        private void GetAverrage(out double profit, out double consumption)
        {
            profit = 0;
            consumption = 0;
            if (gridAverrage.Rows.Count > 0)
            {
                int countRowWithValue = 0;
                foreach (DataRow Row in gridAverrage.Rows)
                {
                    if (Row["Расходы_Исполнено, %"] != DBNull.Value && Row["% выполнения годовых назначений"] != DBNull.Value)
                    {
                        profit = profit + Convert.ToDouble(Row["% выполнения годовых назначений"].ToString());
                        consumption = consumption + Convert.ToDouble(Row["Расходы_Исполнено, %"].ToString());
                        countRowWithValue++;
                    }

                }
                profit = profit / countRowWithValue;
                consumption = consumption / countRowWithValue;
            }
        }
        #endregion
        private HtmlTableCell GetTableGroup(string RegionUniqueName, int index)
        {
            UserParams.Region.Value = RegionUniqueName;
            HtmlTable tableRegion1 = new HtmlTable();
            HtmlTableCell cell = new HtmlTableCell();
            string query4 = DataProvider.GetQueryText("FO_0002_0065_Group");
            gridIndicators = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query4, "Наименование", gridIndicators);
            HtmlTableRow row = new HtmlTableRow();
            HtmlTableCell cell0 = new HtmlTableCell();
            HtmlTableCell cell1 = new HtmlTableCell();
            HtmlTableCell cell2 = new HtmlTableCell();
            HtmlTableCell cell3 = new HtmlTableCell();
            HtmlTableCell cell4 = new HtmlTableCell();
            Label lb;
            if (index == 0)
            {
                lb = new Label();
                lb.CssClass = "TableFont";

                lb.Text = "";
                cell1.Style.Add("vertical-align", "middle");
                cell1.Style.Add("text-align", "right");
                cell1.Style.Add("padding-bottom", "5px");
                cell1.Style.Add("padding-top", "0px");
                cell1.Style.Add("width", "80px");
                cell1.Controls.Add(lb);
                lb = new Label();
                lb.CssClass = "TableFont";
                lb.Text = "Назначено, тыс. руб.";
                cell2.Style.Add("vertical-align", "middle");
                cell2.Style.Add("text-align", "right");
                cell2.Style.Add("padding-bottom", "5px");
                cell2.Style.Add("padding-top", "0px");
                cell2.Style.Add("width", "120px");
                cell2.Controls.Add(lb);

                lb = new Label();
                lb.CssClass = "TableFont";
                lb.Text = "Исполнено, тыс. руб.";
                cell3.Style.Add("vertical-align", "middle");
                cell3.Style.Add("text-align", "right");
                cell3.Style.Add("padding-bottom", "5px");
                cell3.Style.Add("padding-top", "0px");
                cell3.Style.Add("width", "120px");
                cell3.Controls.Add(lb);

                lb = new Label();
                lb.CssClass = "TableFont";
                lb.Text = "   ";
                cell0.Style.Add("vertical-align", "middle");
                cell0.Style.Add("text-align", "right");
                cell0.Style.Add("padding-top", "0px");
                cell0.Style.Add("width", "20px");
                cell0.Style.Add("padding-left", "40px");
                cell0.Controls.Add(lb);

                lb = new Label();
                lb.CssClass = "TableFont";
                lb.Text = "Исполнено, %";
                cell4.Style.Add("vertical-align", "middle");
                cell4.Style.Add("text-align", "center");
                cell4.Style.Add("padding-bottom", "5px");
                cell4.Style.Add("padding-top", "0px");
                cell4.Style.Add("width", "100px");
                cell4.Controls.Add(lb);

                row.Cells.Add(cell1);
                row.Cells.Add(cell2);
                row.Cells.Add(cell3);
                row.Cells.Add(cell0);
                row.Cells.Add(cell4);
                tableRegion1.Rows.Add(row);
            }

            for (int i = 0; i < gridIndicators.Rows.Count; i++)
            {

                Image imageGreen = new Image();
                Image imageRed = new Image();
                row = new HtmlTableRow();
                cell0 = new HtmlTableCell();
                cell1 = new HtmlTableCell();
                cell2 = new HtmlTableCell();
                cell3 = new HtmlTableCell();
                cell4 = new HtmlTableCell();

                lb = new Label();
                lb.CssClass = "TableFont";
                Boolean isProfit = false;
                if (i == 0)
                {

                    lb.Text = "Доходы";
                    isProfit = true;
                }
                else
                {
                    lb.Text = "Расходы";
                    isProfit = false;
                }

                lb.Text = string.Format("<span class='ServeText' style='font-size: 14pt'>{0}</span>", lb.Text);
                cell1.Controls.Add(lb);
                cell1.Style.Add("vertical-align", "middle");
                cell1.Style.Add("text-align", "right");
                cell1.Style.Add("padding-bottom", "5px");
                cell1.Style.Add("padding-top", "0px");
                cell1.Style.Add("width", "80px");

                lb = new Label();
                lb.CssClass = "TableFont";
                if ((object)gridIndicators.Rows[i][1] != DBNull.Value)
                {
                    lb.Text = String.Format("&nbsp;<b><span class='DigitsValueXLarge'>{0:N2}</span></b>&nbsp;", Convert.ToDouble(gridIndicators.Rows[i][1]));
                }
                cell2.Controls.Add(lb);
                cell2.Style.Add("vertical-align", "middle");
                cell2.Style.Add("text-align", "right");
                cell2.Style.Add("padding-bottom", "5px");
                cell2.Style.Add("padding-top", "0px");
                cell2.Style.Add("width", "120px");


                lb = new Label();
                lb.CssClass = "TableFont";
                if ((object)gridIndicators.Rows[i][2] != DBNull.Value)
                {
                    lb.Text = String.Format("&nbsp;<b><span class='DigitsValueXLarge'>{0:N2}</span></b>&nbsp;", Convert.ToDouble(gridIndicators.Rows[i][2]));

                }
                cell3.Controls.Add(lb);
                cell3.Style.Add("vertical-align", "middle");
                cell3.Style.Add("text-align", "right");
                cell3.Style.Add("padding-bottom", "5px");
                cell3.Style.Add("padding-top", "0px");
                cell3.Style.Add("width", "120px");
                HtmlGenericControl divGreen = new HtmlGenericControl("div");

                divGreen.ID = String.Format("TagCloud_tag{0}_{1}", this.ID, j);

                imageGreen.ImageUrl = "../../../images/ballGreenBB.png";
                imageGreen.Style.Add("width", "20px");
                imageGreen.Style.Add("padding-left", "40px");
                if (imageGreen.ImageUrl != String.Empty)
                {
                    divGreen.Controls.Add(imageGreen);
                }
                TooltipHelper.AddToolTip(divGreen, "Процент исполнения выше среднего по поселениям", this.Page);
                j++;

                HtmlGenericControl divRed = new HtmlGenericControl("div");
                divRed.ID = String.Format("TagCloud_tag{0}_{1}", this.ID, j);
                imageRed.ImageUrl = "../../../images/ballRedBB.png";
                imageRed.Style.Add("width", "20px");
                imageRed.Style.Add("padding-left", "40px");
                if (imageRed.ImageUrl != String.Empty)
                {
                    divRed.Controls.Add(imageRed);
                }
                TooltipHelper.AddToolTip(divRed, "Процент исполнения ниже среднего по поселениям", this.Page);
                j++;
                if (isProfit)
                {
                    if (((object)gridIndicators.Rows[i][3] != DBNull.Value))
                    {
                        if (Convert.ToDouble(gridIndicators.Rows[i][3]) > avverageProfit)
                        {

                            cell0.Controls.Add(divGreen);
                        }
                        else
                            if (Convert.ToDouble(gridIndicators.Rows[i][3]) < avverageProfit)
                            {
                                cell0.Controls.Add(divRed);
                            }
                    }
                }
                else
                {
                    if (((object)gridIndicators.Rows[i][3] != DBNull.Value))
                    {

                        if (Convert.ToDouble(gridIndicators.Rows[i][3]) > avverage)
                        {


                            cell0.Controls.Add(divGreen);
                        }
                        else
                            if (Convert.ToDouble(gridIndicators.Rows[i][3]) < avverage)
                            {
                                cell0.Controls.Add(divRed);
                            }
                    }
                }
                lb = new Label();
                lb.CssClass = "TableFont";
                if ((object)gridIndicators.Rows[i][3] != DBNull.Value)
                {
                    lb.Text = String.Format("&nbsp;<b><span class='DigitsValueXLarge'>{0:P2}</span></b>&nbsp;", Convert.ToDouble(gridIndicators.Rows[i][3]));
                }
                cell4.Controls.Add(lb);
                cell4.Style.Add("vertical-align", "middle");
                cell4.Style.Add("text-align", "right");
                cell4.Style.Add("padding-bottom", "5px");
                cell4.Style.Add("padding-top", "0px");
                cell4.Style.Add("width", "100px");


                row.Cells.Add(cell1);
                row.Cells.Add(cell2);
                row.Cells.Add(cell3);
                row.Cells.Add(cell0);
                row.Cells.Add(cell4);
                tableRegion1.Rows.Add(row);
            }
            //обработка пустых значений
            if (gridIndicators.Rows.Count == 0)
            {
                for (int i = 0; i <= 1; i++)
                {
                    row = new HtmlTableRow();
                    cell1 = new HtmlTableCell();
                    cell2 = new HtmlTableCell();
                    cell3 = new HtmlTableCell();
                    cell0 = new HtmlTableCell();
                    cell4 = new HtmlTableCell();
                    lb = new Label();
                    lb.CssClass = "TableFont";
                    if (i == 0)
                        lb.Text = "Доходы";

                    else
                        lb.Text = "Расходы";

                    lb.Text = string.Format("<span class='ServeText' style='font-size: 14pt'>{0}</span>", lb.Text);
                    cell1.Style.Add("vertical-align", "middle");
                    cell1.Style.Add("text-align", "right");
                    cell1.Style.Add("padding-bottom", "5px");
                    cell1.Style.Add("padding-top", "0px");
                    cell1.Style.Add("width", "80px");
                    cell1.Controls.Add(lb);
                    lb = new Label();
                    lb.CssClass = "TableFont";

                    lb.Text = String.Format("&nbsp;<b><span class='ServeText'>{0:N2}</span></b>&nbsp;", "Нет данных");
                    cell2.Style.Add("vertical-align", "middle");
                    cell2.Style.Add("text-align", "right");
                    cell2.Style.Add("padding-bottom", "5px");
                    cell2.Style.Add("padding-top", "0px");
                    cell2.Style.Add("width", "120px");
                    cell2.Controls.Add(lb);

                    lb = new Label();
                    lb.CssClass = "TableFont";
                    lb.Text = String.Format("&nbsp;<b><span class='ServeText'>{0:N2}</span></b>&nbsp;", "Нет данных");
                    cell3.Style.Add("vertical-align", "middle");
                    cell3.Style.Add("text-align", "right");
                    cell3.Style.Add("padding-bottom", "5px");
                    cell3.Style.Add("padding-top", "0px");
                    cell3.Style.Add("width", "120px");
                    cell3.Controls.Add(lb);

                    lb = new Label();

                    lb.CssClass = "TableFont";
                    lb.Text = "";
                    cell0.Style.Add("width", "20px");
                    cell0.Style.Add("padding-left", "40px");
                    cell0.Controls.Add(lb);

                    lb = new Label();
                    lb.CssClass = "TableFont";
                    lb.Text = String.Format("&nbsp;<b><span class='ServeText'>{0:P2}</span></b>&nbsp;", "Нет данных");
                    cell4.Style.Add("vertical-align", "middle");
                    cell4.Style.Add("text-align", "right");
                    cell4.Style.Add("padding-bottom", "5px");
                    cell4.Style.Add("padding-top", "0px");
                    cell4.Style.Add("width", "100px");
                    cell4.Controls.Add(lb);

                    row.Cells.Add(cell1);
                    row.Cells.Add(cell2);
                    row.Cells.Add(cell3);
                    row.Cells.Add(cell0);
                    row.Cells.Add(cell4);
                    tableRegion1.Rows.Add(row);
                }
            }
            cell.Controls.Add(tableRegion1);
            return cell;
        }
        private string SetValueDictionary(string key, string value)
        {
            if (localSettlementUniqueNames == null || localSettlementUniqueNames.Count == 0)
            { localSettlementUniqueNames.Add(key + " ", value); }
            else
            {
                if (localSettlementUniqueNames.ContainsKey(key))
                {
                    key = key + " ";
                    key = SetValueDictionary(key, value);
                }
                else
                {
                    localSettlementUniqueNames.Add(key, value);
                }
            }
            return key;
        }
        private HtmlTableCell GetNameCellFirstLevel(int i, DataTable Grid, bool Settlements)
        {
            Label lb;
            lb = new Label();
            lb.CssClass = "TableFont";

            HtmlTableCell cell = new HtmlTableCell();

            cell.Style.Add("padding-left", "3px");
            cell.Style.Add("vertical-align", "middle");
            cell.Style.Add("text-align", "left");
            cell.Style.Add("padding-bottom", "5px");
            cell.Style.Add("padding-top", "0px");
            cell.Style.Add("width", "230px");
            lb.Style.Add("font-size", "24px");
            string IDSettlement;
            lb.Text = string.Format("{0}", Grid.Rows[i][0]);
            lb.Text = SetValueDictionary(lb.Text, Grid.Rows[i][1].ToString());
            if (Settlements)
            {

                IDSettlement = CustomParams.GetSettlementIdByName(lb.Text);
                lb.Style.Add("font-size", "20px");
                lb.Text = String.Format("<a href='webcommand?showReport=FO_0002_0001_Settlement={0}'>&nbsp;{1}&nbsp;</a>", IDSettlement, lb.Text);
            }
            cell.Controls.Add(lb);

            return cell;
        }
    }
}
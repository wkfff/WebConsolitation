using System;
using System.IO;
using System.Web;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using System.Data;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Collections;
using System.Drawing;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class FST_0003_0003 : CustomReportPage
    {
        private DataTable gridDt = new DataTable();
        private DataTable gridRegionsRF = new DataTable();
        HtmlTable tableRegion = new HtmlTable();

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DateTime currentDate = CubeInfoHelper.FstTariffsAndRegulationsInfo.LastDate;
            DateTime lastDate = new DateTime(currentDate.Year - 1, 12, 1);

            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц]", currentDate, 4);
            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц]", lastDate, 4);

            Title.Text = String.Format("Рост/снижение тарифов на коммунальные услуги за&nbsp;<b><span class='DigitsValue'> {0} {1}</span></b>&nbsp;по сравнению с&nbsp;<b><span class='DigitsValue'> {2} {3} </span></b>&nbsp;по субъектам РФ, входящих в Центральный федеральный округ",
                CRHelper.RusMonth(currentDate.Month), currentDate.Year, CRHelper.RusMonthTvorit(lastDate.Month), lastDate.Year);

            #region Table

            string query2 = DataProvider.GetQueryText("FST_0003_0003_RegionsRF");
            gridRegionsRF = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query2, "Наименование", gridRegionsRF);

            #region вручную заполним словарь фиксированными значениями услуг
            #endregion
            if (gridRegionsRF.Rows.Count > 0)
            {
                tableRegion = new HtmlTable();
                for (int i = 0; i < gridRegionsRF.Rows.Count; i++)
                {
                    HtmlTableRow row = new HtmlTableRow();
                    HtmlTableCell cell1 = new HtmlTableCell();
                    HtmlTableCell cell2 = new HtmlTableCell();

                    UserParams.RegionDimension.Value = gridRegionsRF.Rows[i][2].ToString();
                    query2 = DataProvider.GetQueryText("FST_0003_0003_Table");
                    gridDt = new DataTable();
                    DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query2, "Наименование", gridDt);
                    cell1 = GetNameCellFirstLevel(gridRegionsRF.Rows[i][1].ToString());
                    cell2 = GetListServices();
                    row.Cells.Add(cell1);
                    row.Cells.Add(cell2);
                    cell1.Style.Add("padding-left", "20px");
                    cell1.Style.Add("color", "White");
                    cell1.Style.Add("vertical-align", "top");                   

                    tableRegion.Style.Add("padding-left", "10px");
                    tableRegion.Style.Add("padding-top", "20px");
                    tableRegion.Rows.Add(row);
                }
            }
            PlaceHolder1.Controls.Add(tableRegion);


            #endregion
        }

        private HtmlTableCell GetListServices()
        {
            HtmlTable tableServices = new HtmlTable();
            HtmlTableCell cell = new HtmlTableCell();
            tableServices = new HtmlTable();
            Label lb;
            HtmlTableRow row;
            HtmlTableCell cell1, cell2;
            tableServices.Style.Add("margin-left", "-220px");
            tableServices.Style.Add("margin-top", "40px");
            tableServices.Style.Add("margin-bottom", "10px");
            tableServices.Style.Add("border-collapse", "collapse");
            for (int j = 0; j < gridDt.Rows.Count; j++)
            {
                cell1 = new HtmlTableCell();
                cell2 = new HtmlTableCell();
                row = new HtmlTableRow();
                cell1.Style.Add("padding-left", "3px");
                cell1.Style.Add("vertical-align", "middle");
                cell1.Style.Add("text-align", "left");
                cell1.Style.Add("padding-bottom", "5px");
                cell1.Style.Add("padding-top", "0px");
                cell1.Style.Add("width", "120px");

                lb = new Label();
                lb.CssClass = "TableFont";
                lb.Text = CRHelper.ToUpperFirstSymbol(gridDt.Rows[j][0].ToString().Split(';')[1].Trim(' '));
                lb.Text = string.Format("<span class='ServeText' style='font-size: 14pt'>{0}</span>", lb.Text);
                cell1.Controls.Add(lb);

                lb = new Label();
                lb.CssClass = "TableFont";
                cell2.Style.Add("padding-left", "3px");
                cell2.Style.Add("vertical-align", "middle");
                cell2.Style.Add("text-align", "left");
                cell2.Style.Add("padding-bottom", "5px");
                cell2.Style.Add("padding-top", "0px");
                cell2.Style.Add("width", "120px");
                cell2 = GetIndicators(j);
                if (j < gridDt.Rows.Count-1)
                row.Style.Add("border-bottom","1px solid gray");
                row.Cells.Add(cell1);
                row.Cells.Add(cell2);
                tableServices.Rows.Add(row);
            }
           
            cell.Controls.Add(tableServices);
            return cell;
        }
        private HtmlTableCell GetIndicators(int index)
        {
            HtmlTable table = new HtmlTable();
            HtmlTableCell cell = new HtmlTableCell();
            table = new HtmlTable();
            Label lb;
            HtmlTableRow row, row2;
            HtmlTableCell cell1, cell2, cell3, cell4;

            if (gridDt.Rows.Count > 0)
            {
                cell1 = new HtmlTableCell();
                cell2 = new HtmlTableCell();
                cell3 = new HtmlTableCell();
                cell4 = new HtmlTableCell();

                row = new HtmlTableRow();
                row2 = new HtmlTableRow();
                cell2.Style.Add("padding-left", "3px");
                cell2.Style.Add("vertical-align", "middle");
                cell2.Style.Add("text-align", "left");
                cell2.Style.Add("padding-bottom", "5px");
                cell2.Style.Add("padding-top", "0px");
                cell2.Style.Add("width", "150px");

                cell4.Style.Add("padding-left", "3px");
                cell4.Style.Add("vertical-align", "middle");
                cell4.Style.Add("text-align", "left");
                cell4.Style.Add("padding-bottom", "5px");
                cell4.Style.Add("padding-top", "0px");
                cell4.Style.Add("width", "150px");
                if (gridDt.Rows[index][1].ToString() == "False")
                {
                    lb = new Label();
                    lb.CssClass = "TableFont";
                    cell1.Style.Add("width", "20px");
                    cell1.Style.Add("padding-left", "15px");
                    lb.Text = string.Format("<span class='ServeText' style='font-size: 12pt'>{0}</span>", "Нет роста");
                    cell2.Controls.Add(lb);
                }
                else
                {
                    lb = new Label();
                    lb.CssClass = "TableFont";
                    lb.Text = string.Format("<span class='ServeText' style='font-size: 12pt'>{0}</span>", "Есть рост");
                    cell1.Style.Add("width", "20px");
                    cell1.Style.Add("padding-left", "15px");
                    cell1.Style.Add("background-image", "url(../../../images/ballRedBB.png)");
                    cell1.Style.Add("background-repeat", "no-repeat");
                    cell1.Style.Add("background-position", "right center");
                    cell2.Controls.Add(lb);
                }
                if (gridDt.Rows[index][2].ToString() == "False")
                {
                    lb = new Label();
                    lb.CssClass = "TableFont";
                    cell3.Style.Add("width", "20px");
                    cell3.Style.Add("padding-left", "15px");
                    lb.Text = string.Format("<span class='ServeText' style='font-size: 12pt'>{0}</span>", "Нет снижения");
                    cell4.Controls.Add(lb);
                }
                else
                {
                    lb = new Label();
                    lb.CssClass = "TableFont";
                    lb.Text = string.Format("<span class='ServeText' style='font-size: 12pt'>{0}</span>", "Есть снижение");
                    cell3.Style.Add("width", "20px");
                    cell3.Style.Add("padding-left", "15px");
                    cell3.Style.Add("background-image", "url(../../../images/ballGreenBB.png)");
                    cell3.Style.Add("background-repeat", "no-repeat");
                    cell3.Style.Add("background-position", "right center");
                    cell4.Controls.Add(lb);
                }
                row.Cells.Add(cell1);
                row.Cells.Add(cell2);
                row.Cells.Add(cell3);
                row.Cells.Add(cell4);
                table.Rows.Add(row);
                //table.Rows.Add(row2);

            }
            cell.Controls.Add(table);
            return cell;
        }
        private HtmlTableCell GetNameCellFirstLevel(string RegionName)
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
            cell.Style.Add("width", "250px");
            lb.Style.Add("font-size", "24px");
            string IDSettlement;
            IDSettlement = CustomParams.GetSubjectIdByName(RegionName);
            lb.Style.Add("font-size", "20px");
            lb.Text = String.Format("<a href='webcommand?showReport=FST_0003_0001_{0}'>&nbsp;{1}&nbsp;</a>", IDSettlement, RegionName);
            //lb.Text = RegionName;
            cell.Controls.Add(lb);
            return cell;
        }
    }
}

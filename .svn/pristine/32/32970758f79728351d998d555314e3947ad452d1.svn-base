using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.WebControls;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Resources.Appearance;
using System.Web.UI.HtmlControls;
using Infragistics.UltraGauge.Resources;
using System.IO;
using Krista.FM.Server.Dashboards.Components.Components;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0016_0003: CustomReportPage
    {
        private DataTable gridDt = new DataTable();
        private DataTable gridDate = new DataTable();
        private DataTable gridDetailTables = new DataTable();
        private DataTable gridIndicators = new DataTable();
        private DataTable maxminValue = new DataTable();
        private DataTable gridAvg = new DataTable();
        HtmlTable tableMain = new HtmlTable();
        HtmlTable tableRegion = new HtmlTable();
        HtmlTable tableSecondLevel = new HtmlTable();
        HtmlTable tableValueRegion = new HtmlTable();
        HtmlTable tableThirdLevel = new HtmlTable();

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UltraWebGrid1.Grid.EnableAppStyling = DefaultableBoolean.False;
            UltraWebGrid1.BrowserSizeAdapting = false;
            UltraWebGrid1.Height = 90;
            UltraWebGrid1.AutoSizeStyle = Components.GridAutoSizeStyle.AutoWidth;
            UltraWebGrid1.Grid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            

            string query = DataProvider.GetQueryText("FO_0016_0003_date");
            gridDate = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, gridDate);
            UserParams.PeriodYear.Value = gridDate.Rows[0][0].ToString();
            UserParams.PeriodHalfYear.Value = gridDate.Rows[0][1].ToString();
            UserParams.PeriodQuater.Value = gridDate.Rows[0][2].ToString();

            string quarter = UserParams.PeriodQuater.Value;
            quarter = quarter.Replace("Квартал", "");
            int prevMonth = CRHelper.QuarterLastMonth(Convert.ToInt32(quarter));

            Title.Text = "Результаты мониторинга соблюдения муниципальными образованиями Ханты-Мансийского автономного округа – Югры требований Бюджетного кодекса Российской Федерации по состоянию на&nbsp;";
            Title.Style.Add("color", "White");
            if ((prevMonth + 1) >= 10)
            {
                SubTitle.Text = string.Format("01.{0}.{1}", prevMonth + 1, UserParams.PeriodYear.Value);
            }
            else
            {
                SubTitle.Text = string.Format("01.0{0}.{1}", prevMonth + 1, UserParams.PeriodYear.Value);
            }
            SubTitle.Style.Add("font-weight", "bold");
            SubTitle.Style.Add("color","White");
            string query2 = DataProvider.GetQueryText("FO_0016_0003_grid");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query2, "Наименование МР (ГО)", gridDt);
            gridDt.Columns.RemoveAt(0);

            string query3 = DataProvider.GetQueryText("FO_0016_0003_gridTables");
            gridDetailTables = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query3, "Наименование МР (ГО)", gridDetailTables);
            gridDetailTables.Columns.RemoveAt(0);

            string query4 = DataProvider.GetQueryText("FO_0016_0003_indicators");
            gridIndicators = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query4, "Наименование МР (ГО)", gridIndicators);

            string query5 = DataProvider.GetQueryText("FO_0016_0003_MaxMinValue");
            maxminValue = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query5, "Наименование МР (ГО)", maxminValue);

            GridDataBind();
            #region Главная таблица

                bool flag = false;
                int i = 0;
                if (i < gridDt.Rows.Count)
                {
                    flag = true;
                }

                while (flag)
                {
                    tableMain = new HtmlTable();

                    HtmlTableRow htmlRow = new HtmlTableRow();
                    HtmlTableCell htmlCell = new HtmlTableCell();
                    htmlCell = GetTableCell(ref i);
                    i++;
                    htmlRow.Cells.Add(htmlCell);
                    tableMain.Rows.Add(htmlRow);

                    PlaceHolder1.Controls.Add(tableMain);

                    if (i < gridDt.Rows.Count)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }

            #endregion
        }

        private void GridDataBind()
        {

            string query2 = DataProvider.GetQueryText("FO_0016_0003_gridAvg");
            gridAvg = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query2, "Тип МО", gridAvg);

            if (gridAvg.Rows.Count > 0)
            {
                UltraWebGrid1.DataTable = gridAvg;
            }
        }


        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(300);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(200);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
            }

            GridHeaderLayout headerLayout = UltraWebGrid1.GridHeaderLayout;
            headerLayout.AddCell("Тип МО");
            headerLayout.AddCell("Количество нарушений");
            headerLayout.ApplyHeaderInfo();
        }

        private HtmlTableCell GetTableCell(ref int i)
        {
            tableRegion = new HtmlTable();

            #region Уровень районов 

            HtmlTableRow row = new HtmlTableRow();
            HtmlTableCell cell1 = new HtmlTableCell();
            HtmlTableCell cell2 = new HtmlTableCell();
            HtmlTableCell cell3 = new HtmlTableCell();
            HtmlTableCell cell4 = new HtmlTableCell();
            HtmlTableCell cell5 = new HtmlTableCell();
            string region = gridDt.Rows[i][0].ToString();
            cell1 = GetNameCellFirstLevel(i);
            if (Convert.ToInt32(gridDt.Rows[i][1]) == Convert.ToInt32(maxminValue.Rows[0][1]))
            {
                cell2.Style.Add("background-image", "url(../../../images/starGrayBB.png)");
                cell2.Style.Add("background-repeat", "no-repeat;");
            }
            else
            {
                if (Convert.ToInt32(gridDt.Rows[i][1]) == Convert.ToInt32(maxminValue.Rows[0][2]))
                {
                    cell2.Style.Add("background-image", "url(../../../images/starYellowBB.png)");
                    cell2.Style.Add("background-repeat", "no-repeat;");

                }
            }
            cell2.Style.Add("width", "30px");

            Label lb;
            lb = new Label();
            lb.CssClass = "TableFont";
            lb.Text = "нарушения: ";
            cell3.Style.Add("padding-right", "3px");
            cell3.Style.Add("vertical-align", "middle");
            cell3.Style.Add("text-align", "right");
            cell3.Style.Add("padding-bottom", "5px");
            cell3.Style.Add("padding-top", "0px");
            cell3.Style.Add("width", "90px");
            cell3.Controls.Add(lb);

            cell4 = GetValueCellFirstLevel(i);
            cell5 = GetGaugeCell(i);

            row.Cells.Add(cell1);
            row.Cells.Add(cell2);
            row.Cells.Add(cell3);
            row.Cells.Add(cell4);
            row.Cells.Add(cell5);

            tableRegion.Rows.Add(row);
            tableRegion.Style.Add("background-image", "url(../../../images/CollapseIpad.png)");
            tableRegion.Style.Add("background-repeat", "no-repeat;");
            tableRegion.Style.Add("background-position", "0px 5px");
            tableRegion.Attributes.Add("onclick", "resize(this, \"table" + i + "\")");
            tableRegion.Style.Add("padding-left", "20px");
            tableRegion.Style.Add("padding-bottom", "5px");
            tableRegion.Style.Add("color", "White");
        #endregion

        #region Второй уровень списка

            #region Таблица нарушений второго уровня

            bool flag = false;
            tableSecondLevel = new HtmlTable();
            tableSecondLevel.ID = String.Format("table{0}", i);
            tableSecondLevel.Style.Add("display", "none");
            tableSecondLevel.Style.Add("padding-left", "20px");

            row = new HtmlTableRow();
            HtmlTableCell cell = new HtmlTableCell();
            cell = GetTableCellSecond(i);
            row.Controls.Add(cell);
            tableSecondLevel.Rows.Add(row);

            #endregion

            cell = new HtmlTableCell();
            cell.Controls.Add(tableRegion);
            cell.Controls.Add(tableSecondLevel);
            return cell;

        #endregion 

        }

        private HtmlTableCell GetNameCellFirstLevel(int i)
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
            lb.Style.Add("font-size", "20px");

            lb.Text = string.Format("{0}", gridDt.Rows[i][0]);
            cell.Controls.Add(lb);

            return cell;
        }

        private HtmlTableCell GetValueCellFirstLevel(int i)
        {
            Label lb1;
            lb1 = new Label();
            lb1.CssClass = "TableFont";


            HtmlTableCell cell = new HtmlTableCell();

            cell.Style.Add("padding-right", "3px");
            cell.Style.Add("vertical-align", "middle");
            cell.Style.Add("text-align", "right");
            cell.Style.Add("padding-bottom", "5px");
            cell.Style.Add("padding-top", "0px");
            cell.Style.Add("font-weight", "bold");
            cell.Style.Add("width", "20px");
            lb1.Style.Add("font-size", "20px");
            lb1.Text = string.Format("{0}", gridDt.Rows[i][1]);
            cell.Controls.Add(lb1);

            return cell;
        }

        private HtmlTableCell GetGaugeCell(int i)
        {
            Label lb;
            lb = new Label();
            lb.CssClass = "TableFont";
            lb.Text = GetGaugeUrl(i);
  
            HtmlTableCell cell = new HtmlTableCell();
            cell.Controls.Add(lb);

            return cell;
        }

        private string GetGaugeUrl(int i)
        {
            int value = Convert.ToInt32(gridDt.Rows[i][1]);
            string path = "FO_0016_0003_gauge_" + value.ToString("N0") + ".png";
            string returnPath = String.Format("<img style=\"FLOAT: left; margin-top: -5px;\" src=\"../../../TemporaryImages/{0}\"/>", path);
            string serverPath = Server.MapPath("~/TemporaryImages/" + path);


            if (File.Exists(serverPath))
                return returnPath;

            BarGaugeIndicator gauge = (BarGaugeIndicator)Page.LoadControl("../../../Components/Gauges/BarGaugeIndicator.ascx");
            gauge.Width = 200;
            gauge.Height = 30;
            gauge.SetRange(0, Convert.ToDouble(maxminValue.Rows[0][1]), 1);
            gauge.IndicatorValue = Convert.ToDouble(gridDt.Rows[i][1]);
            LinearGaugeScale scale = gauge.MinorScale;
            MultiStopLinearGradientBrushElement BrushElement = (MultiStopLinearGradientBrushElement)(scale.Markers[0].BrushElement);
            BrushElement.ColorStops.Clear();
            if (value > Convert.ToInt32(maxminValue.Rows[0][1])/2)
            {

                BrushElement.ColorStops.Add(Color.FromArgb(253, 119, 119), 0);
                BrushElement.ColorStops.Add(Color.FromArgb(239, 87, 87), 0.41F);
                BrushElement.ColorStops.Add(Color.FromArgb(224, 0, 0), 0.428F);
                BrushElement.ColorStops.Add(Color.FromArgb(199, 0, 0), 1);
            }
            else
            {
                BrushElement.ColorStops.Add(Color.FromArgb(255, 255, 128), 0);
                BrushElement.ColorStops.Add(Color.Yellow, 0.41F);
                BrushElement.ColorStops.Add(Color.Yellow, 0.428F);
                BrushElement.ColorStops.Add(Color.FromArgb(255, 128, 0), 1);
            }
            gauge.SaveAsImage(serverPath);
            return returnPath;
        }

        #region Таблица нарушений

        private HtmlTableCell GetTableCellSecond(int i)
        {
            HtmlTable table = new HtmlTable();
            table.Attributes.Add("class", "HtmlTableCompact");
            if (Convert.ToInt32(gridDt.Rows[i][1]) > 0)
            {
                HtmlTableRow row = new HtmlTableRow();
                HtmlTableCell cell = new HtmlTableCell();
                Label lb;
                lb = new Label();
                lb.CssClass = "TableFont";
                lb.Text = "Выявлены нарушения";
                cell = new HtmlTableCell();
                cell.Style.Add("text-align", "center");
                cell.Style.Add("padding-left", "5px");
                cell.Style.Add("padding-right", "5px");
                cell.Attributes.Add("class", "HtmlTableHeader");
                cell.Style.Add("font-size", "16px");
                cell.Controls.Add(lb);
                row.Controls.Add(cell);

                lb = new Label();
                lb.CssClass = "TableFont";
                lb.Text = "Значение показателя";
                cell = new HtmlTableCell();
                cell.Style.Add("text-align", "center");
                cell.Style.Add("padding-left", "5px");
                cell.Style.Add("padding-right", "5px");
                cell.Attributes.Add("class", "HtmlTableHeader");
                cell.Style.Add("font-size", "16px");
                cell.Controls.Add(lb);
                row.Controls.Add(cell);

                lb = new Label();
                lb.CssClass = "TableFont";
                lb.Text = "Нормативное значение";
                cell = new HtmlTableCell();
                cell.Style.Add("text-align", "center");
                cell.Style.Add("padding-left", "5px");
                cell.Style.Add("padding-right", "5px");
                cell.Attributes.Add("class", "HtmlTableHeader");
                cell.Style.Add("font-size", "16px");
                cell.Controls.Add(lb);
                row.Controls.Add(cell);

                table.Rows.Add(row);

                for (int j = 1; j < gridDetailTables.Columns.Count; j = j + 2)
                {
                    if (gridDetailTables.Rows[i][j] != DBNull.Value)
                    {
                        row = new HtmlTableRow();
                        cell = new HtmlTableCell();

                        string indicator = gridDetailTables.Columns[j].Caption;
                        int index = indicator.IndexOf(';');
                        indicator = indicator.Remove(index);
                        string fullName = gridIndicators.Rows[0][indicator].ToString();

                        lb = new Label();
                        lb.CssClass = "TableFont";
                        lb.Text = fullName;
                        cell = new HtmlTableCell();
                        cell.Attributes.Add("class", "HtmlTableCompact");
                        cell.Style.Add("text-align", "left");
                        cell.Style.Add("padding-left", "5px");
                        cell.Style.Add("padding-right", "5px");
                        cell.Controls.Add(lb);
                        row.Controls.Add(cell);

                        lb = new Label();
                        lb.CssClass = "TableFont";
                        lb.Text = gridDetailTables.Rows[i][j].ToString();
                        cell = new HtmlTableCell();
                        cell.Attributes.Add("class", "HtmlTableCompact");
                        cell.Style.Add("text-align", "right");
                        cell.Style.Add("padding-left", "5px");
                        cell.Style.Add("padding-right", "5px");
                        cell.Controls.Add(lb);
                        row.Controls.Add(cell);

                        lb = new Label();
                        lb.CssClass = "TableFont";
                        lb.Text = gridDetailTables.Rows[i][j + 1].ToString();
                        cell = new HtmlTableCell();
                        cell.Attributes.Add("class", "HtmlTableCompact");
                        cell.Style.Add("text-align", "right");
                        cell.Style.Add("padding-left", "5px");
                        cell.Style.Add("padding-right", "5px");
                        cell.Controls.Add(lb);
                        row.Controls.Add(cell);

                        table.Rows.Add(row);
                    }
                }
            }

            HtmlTableCell cellTable = new HtmlTableCell();
            cellTable.Controls.Add(table);

            return cellTable;
        }

        #endregion
    }
}
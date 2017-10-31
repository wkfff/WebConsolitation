using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image=System.Web.UI.WebControls.Image;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0017 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtData = new DataTable();

        private double othersEthalon;
        private double salaryEthalon;
        private double commonEthalon;

        private DateTime date;

        #region Параметры запроса

        // мера План
        private CustomParam measurePlan;
        // мера Факт
        private CustomParam measureFact;
        // мера Остаток на начало
        private CustomParam measureStartBalance;
        // мера Остаток на конец
        private CustomParam measureEndBalance;

        #endregion

        public bool IsQuaterPlanType
        {
            get
            {
                return RegionSettingsHelper.Instance.CashPlanType.ToLower() == "quarter";
            }
        }

        public bool IsYar
        {
            get
            {
                return RegionSettingsHelper.Instance.Name.ToLower() == "ярославская область";
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            #region Инициализация параметров запроса

            measurePlan = UserParams.CustomParam("measure_plan");
            measureFact = UserParams.CustomParam("measure_fact");
            measureStartBalance = UserParams.CustomParam("measure_start_balance");
            measureEndBalance = UserParams.CustomParam("measure_end_balance");

            #endregion

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][5].ToString();


            if (!dtDate.Rows[0][4].ToString().Contains("Заключительные обороты"))
            {
                date = new DateTime(
                   Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                   CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                   Convert.ToInt32(dtDate.Rows[0][4].ToString()));

                if (IsQuaterPlanType)
                {
                    lbQuater.Text = "&nbsp;за&nbsp;<span style='color: white;'>" + CRHelper.PeriodDescr(date, 3) + "</span>";
                    lbDate.Text = "&nbsp;(на&nbsp;<span style='color: white;'>" + date.ToString("dd.MM.yyyy") + "</span>) по главным распорядителям средств областного бюджета";
                }
                else
                {
                    lbQuater.Text = string.Empty;
                    lbDate.Text = "&nbsp;на&nbsp;<span style='color: white;'>" + date.ToString("dd.MM.yyyy") + "</span>, тыс.руб.";
                }
            }
            else
            {
                date = new DateTime(
                    Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                    CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                    CRHelper.MonthLastDay(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));

                if (IsQuaterPlanType)
                {
                    lbQuater.Text = "&nbsp;за " + CRHelper.PeriodDescr(date, 3);
                    lbDate.Text = "<br/>за&nbsp;<span style='color: white;'>" + dtDate.Rows[0][3].ToString().ToLower() + " " + dtDate.Rows[0][0] + " года</span>" + ", тыс.руб.";
                }
                else
                {
                    lbQuater.Text = string.Empty;
                    lbDate.Text = " за&nbsp;<span style='color: white;'>" + dtDate.Rows[0][3].ToString().ToLower() + " " + dtDate.Rows[0][0] + " года</span>" + ", тыс.руб.";
                }
            }

            if (IsQuaterPlanType)
            {
                measurePlan.Value = "План";
                measureFact.Value = "Факт";
            }
            else
            {
                measurePlan.Value = "План_Нарастающий итог";
                measureFact.Value = "Факт_Нарастающий итог";
            }

            measureStartBalance.Value = (IsYar) ? "Остаток средств на начало квартала" : RegionSettingsHelper.Instance.CashPlanBalance;
            measureEndBalance.Value = (measureStartBalance.Value == "Остаток средств")
                                          ? measureStartBalance.Value
                                          : "Остаток средств на конец квартала";

            query = DataProvider.GetQueryText("data");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "name", dtData);

            othersEthalon = CRHelper.QuarterDaysCountToDate(date) / CRHelper.QuarterDaysCount(date);
            commonEthalon = CommonAssessionLimit();
            salaryEthalon = SalaryAssessionLimit();
            
            MakeHtmlTableDetailTable();
        }

        private void MakeHtmlTableDetailTable()
        {
            detailTable.CssClass = "HtmlTableCompact";
            
            AddHeaderRow(detailTable);

            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                TableRow row = new TableRow();
                TableCell cell;
                
                cell = GetNameCell(dtData.Rows[i][0].ToString(), false);
                row.Cells.Add(cell);

                cell = GetGreyValueCell(string.Format("{0:N0}", dtData.Rows[i][6]));
                row.Cells.Add(cell);

                cell = GetValueCell(string.Format("{0:N0}", dtData.Rows[i][7]));
                row.Cells.Add(cell);

                cell = GetValueCell(string.Format("{0:P0}", dtData.Rows[i][8]));
                row.Cells.Add(cell);

                cell = GetCell(i, 1);
                row.Cells.Add(cell);
                cell = GetCell(i, 2);
                row.Cells.Add(cell);
                cell = GetCell(i, 3);
                row.Cells.Add(cell);
                cell = GetCell(i, 4);
                row.Cells.Add(cell);
                cell = GetCell(i, 5);
                row.Cells.Add(cell);
                detailTable.Rows.Add(row);
            }
        }

        private TableCell GetCell(int i, int column)
        {
            TableCell cell;
            Image image;
            cell = new TableCell();

            double ethalon;

            switch (column)
            {
                case 1:
                    {
                        ethalon = salaryEthalon;
                        break;
                    }
                case 5:
                    {
                        ethalon = othersEthalon;
                        break;
                    }
                default:
                    {
                        ethalon = commonEthalon;
                        break;
                    }
            }

            image = GetImage(ethalon, i, column);
            cell.Controls.Add(image);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            return cell;
        }

        private Image GetImage(double ethalon, int row, int column)
        {
            double value;
            Image image = new Image();
            if (Double.TryParse(dtData.Rows[row][column].ToString(), out value))
            {

                image.ImageUrl = value >= ethalon ? "~/images/ballGreenBB.png" : "~/images/ballRedBB.png";
            }
            return image;
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
            return cell;
        }

        private TableCell GetGreyValueCell(string value)
        {
            TableCell cell;
            Label lb;
            cell = new TableCell();
            cell.CssClass = "HtmlTableCompact";
            lb = new Label();
            lb.Text = value;
            lb.CssClass = "TableFontGrey";
            cell.Controls.Add(lb);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Right;
            cell.Style.Add("padding-right", "3px");
            return cell;
        }

        private TableCell GetNameCell(string name)
        {
            return GetNameCell(name, false);
        }

        private TableCell GetNameCell(string name, bool child)
        {
            TableCell cell;
            Label lb;
            cell = new TableCell();
            cell.CssClass = "HtmlTableCompact";
            lb = new Label();
            lb.Text = String.Format("<a href='webcommand?showPopoverReport=fo_0035_0008_GRBS={1}&width=690&height=500'>{0}</a>", name, CustomParams.GetGrbsIdByName(name));

            if (child)
            {
                lb.CssClass = "TableFontGrey";
                cell.Style.Add("padding-left", "10px");
            }
            else
            {
                lb.CssClass = "TableFont";
                cell.Style.Add("padding-left", "3px");
            }
            cell.Controls.Add(lb);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Left;
            cell.Style.Add("padding-top", "5px");
            cell.Style.Add("padding-bottom", "5px");
            return cell;
        }

        private void AddHeaderRow(Table table)
        {
            TableCell cell;
            TableRow row;

            row = new TableRow();
            cell = new TableCell();
            cell.Width = 180;
            cell.Text = "Главный распорядитель";

            SetupHeaderCell(cell);

            row.Cells.Add(cell);
            
            cell = new TableCell();
            cell.Width = 70;
            SetupHeaderCell(cell);
            cell.Text = "План, тыс.руб.";
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 70;
            SetupHeaderCell(cell);
            cell.Text = "Факт, тыс.руб.";
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 30;
            SetupHeaderCell(cell);
            cell.Text = "% исп.";  
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 30;
            SetupHeaderCell(cell);
            cell.Text = "Зараб. плата";
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 30;
            SetupHeaderCell(cell);
            cell.Text = "Норм. публич. обяз.";
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 30;
            SetupHeaderCell(cell);
            cell.Text = "Коммун. расходы";
           
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 30;
            SetupHeaderCell(cell);
            cell.Text = "Межбюдж. трансф";
            
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 30;
            SetupHeaderCell(cell);
            cell.Text = "Проч. расх.";
            
            row.Cells.Add(cell);

            table.Rows.Add(row);
        }

        private void SetupHeaderCell(TableCell cell)
        {
            cell.CssClass = "HtmlTableHeader";
            cell.Style.Add("font-size", "16px");
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
        }


        /// <summary>
        /// Пороговое значение оценки зарплаты.
        /// </summary>
        /// <returns></returns>
        private double SalaryAssessionLimit()
        {
            // Берем номер месяца в квартале
            double monthNum = CRHelper.MonthNumInQuarter(date.Month);
            int day = date.Day;
            if (day < 6)
            {
                // Выплат в этом месяце не было
                return (monthNum - 1) / 3;
            }
            if (day < 21)
            {
                // Была одна выплата
                return (monthNum - 1) / 3 + 1.0 / 6;
            }
            // Все выплаты
            return (monthNum) / 3;
        }

        /// <summary>
        /// Пороговое значение оценки остальных.
        /// </summary>
        /// <returns></returns>
        private double CommonAssessionLimit()
        {
            // Берем номер месяца в квартале
            double monthNum = CRHelper.MonthNumInQuarter(date.Month);
            // Если последний день месяца
            if (CRHelper.MonthLastDay(date.Month) ==
                date.Day)
            {
                return (monthNum) / 3;
            }
            else
            {
                return (monthNum - 1) / 3;
            }
        }

       
    }
}

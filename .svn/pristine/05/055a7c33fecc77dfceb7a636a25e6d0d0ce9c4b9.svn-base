using System;
using System.Data;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0135_0001_v : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtData = new DataTable();
        private DataTable dtOutcomes = new DataTable();
        private DateTime date;
        private double othersEthalon;
        private double salaryEthalon;
        private double commonEthalon;

        #region Параметры запроса

        // мера План
        private CustomParam measurePlan;
        // мера Факт
        private CustomParam measureFact;

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

        public bool IsOmsk
        {
            get
            {
                return RegionSettings.Instance.Id.ToLower() == "omsk";
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

                if (false)
                {
                    lbQuater.Text = "&nbsp;за " + CRHelper.PeriodDescr(date, 3);
                    lbDate.Text = "<br/>(на&nbsp;" + date.ToString("dd.MM.yyyy") + "), тыс.руб.";
                }
                else
                {
                    lbQuater.Text = string.Empty;
                    lbDate.Text = "&nbsp;на&nbsp;" + date.ToString("dd.MM.yyyy") + ", тыс.руб.";
                }
                Label.Text = string.Format("данные на {0} {1} {2} года", dtDate.Rows[0][4], CRHelper.RusMonthGenitive(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())), dtDate.Rows[0][0]);
            }
            else
            {
                date = new DateTime(
                    Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                    CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                    CRHelper.MonthLastDay(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));

                if (false)
                {
                    lbQuater.Text = "&nbsp;за " + CRHelper.PeriodDescr(date, 3);
                    lbDate.Text = "<br/>(за &nbsp;" + dtDate.Rows[0][3].ToString().ToLower() + " " + dtDate.Rows[0][0] + " года" + "), тыс.руб.";
                }
                else
                {
                    lbQuater.Text = string.Empty;
                    lbDate.Text = " за&nbsp;" + dtDate.Rows[0][3].ToString().ToLower() + " " + dtDate.Rows[0][0] + " года" + ", тыс.руб.";
                }
                Label.Text = string.Format("данные за {0} {1} года", dtDate.Rows[0][3].ToString().ToLower(), dtDate.Rows[0][0]);
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

            
            GetData();
            MakeHtmlTable();
            LabelDate.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
        }

        private void MakeHtmlTable()
        {
            table.CssClass = "HtmlTableCompact";
            TableCell cell;
            TableRow row;
            AddHeaderRow();
            
            for (int i = 0; i < dtData.Rows.Count; i++)
            {                
                row = new TableRow();
                cell = GetNameCell(dtData.Rows[i][0].ToString());

                if (dtData.Rows[i][0].ToString() ==
                        "в том числе:")
                {
                    cell.ColumnSpan = 3;
                    row.Cells.Add(cell);
                    table.Rows.Add(row);
                    continue;
                }
                row.Cells.Add(cell);
                table.Rows.Add(row);

                cell = GetIndicatorCell();
                row.Cells.Add(cell);
               
                cell = GetValuesCell(string.Format("{0:N0}<br/>", dtData.Rows[i][2]), string.Format("{0:P2}", dtData.Rows[i][3]));
                row.Cells.Add(cell);
                table.Rows.Add(row);
            }

            for (int i = 0; i < dtOutcomes.Rows.Count; i++)
            {
                row = new TableRow();
                cell = GetNameCell(dtOutcomes.Rows[i][0].ToString());

                if (dtOutcomes.Rows[i][0].ToString() ==
                        "в том числе:")
                {
                    cell.ColumnSpan = 3;
                    row.Cells.Add(cell);
                    table.Rows.Add(row);
                    continue;
                }
                row.Cells.Add(cell);
                table.Rows.Add(row);

                cell = GetIndicatorCell();
                row.Cells.Add(cell);

                cell = GetValuesCell(string.Format("{0:N0}<br/>", dtOutcomes.Rows[i][2]), string.Format("{0:P2}", dtOutcomes.Rows[i][3]));
                row.Cells.Add(cell);
                table.Rows.Add(row);
            }
        }

        private TableCell GetValuesCell(string valueFact, string valuePercent)
        {
            TableCell cell;
            Label lb;
            cell = new TableCell();
            cell.CssClass = "HtmlTableCompact";
            lb = new Label();
            lb.Text = valueFact;
            lb.CssClass = "TableFont";
            cell.Controls.Add(lb);
            lb = new Label();
            lb.Text = valuePercent;
            lb.CssClass = "TableFontGrey";
            cell.Controls.Add(lb);
            cell.Style["border-left-style"] = "none";
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Right;
            return cell;
        }

        private static TableCell GetIndicatorCell()
        {
            TableCell cell;
            cell = new TableCell();
            cell.CssClass = "HtmlTableCompact";
            cell.Width = 18;
            cell.Style["border-right-style"] = "none";
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Left;
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
            lb.CssClass = "TableFontGrey";
            cell.Controls.Add(lb);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Left;
            return cell;
        }

        private void AddHeaderRow()
        {
            TableCell cell;
            TableRow row;
            row = new TableRow();
            cell = new TableCell();
            cell.Width = 220;
            cell.CssClass = "HtmlTableHeader";
            cell.Text = "Показатель";
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 100;
            cell.CssClass = "HtmlTableHeader";
            cell.ColumnSpan = 2;
            cell.Text = "Факт и %&nbsp;исполнения";
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            row.Cells.Add(cell);

            table.Rows.Add(row);
        }

        private void addInnerTable(int i)
        {
            TableCell cell;
            TableRow row;

            row = new TableRow();
            cell = GetNameCell(dtData.Rows[i][0].ToString());
            row.Cells.Add(cell);

            cell = GetIndicatorCell();
            row.Cells.Add(cell);

            cell = GetValuesCell(
                    string.Format("{0:N0}<br/>", dtData.Rows[i][2]),
                    string.Format("{0:P2}", dtData.Rows[i][4]));
            row.Style["border-bottom"] = "#000000 1px solid";
            row.Cells.Add(cell);
            table.Rows.Add(row);

            if (IsYar || dtOutcomes.Rows.Count != 0)
            {
                row = new TableRow();
                cell = GetNameCell("в том числе:");
                cell.ColumnSpan = 4;
                row.Cells.Add(cell);

                row.Cells.Add(cell);
                table.Rows.Add(row);

                for (int outcomesCount = 1; outcomesCount < dtOutcomes.Columns.Count; outcomesCount += 3)
                {
                    row = new TableRow();
                    string name = dtOutcomes.Columns[outcomesCount].ColumnName.Split(';')[0];
                    cell = GetNameCell(name);
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.CssClass = "HtmlTableCompact";

                    double value;
                    string hintRowText = string.Empty;
                    double ethalon;
                    switch (name)
                    {
                        case ("Заработная плата"):
                            {
                                ethalon = salaryEthalon;
                                break;
                            }
                        case ("Прочие расходы"):
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
                    if (Double.TryParse(dtOutcomes.Rows[0][outcomesCount + 2].ToString(), out value))
                    {
                        Image image = new Image();
                        image.ImageUrl = value >= ethalon ? "~/images/green.png" : "~/images/red.png";
                        hintRowText = value >= ethalon
                                          ? String.Format("Соблюдается равномерность ({0:P2})", ethalon)
                                          : String.Format("Не соблюдается равномерность ({0:P2})", ethalon);
                        cell.Controls.Add(image);
                    }

                    cell.Style["border-right-style"] = "none";
                    cell.VerticalAlign = VerticalAlign.Middle;
                    cell.HorizontalAlign = HorizontalAlign.Left;
                    row.Cells.Add(cell);

                    cell = GetValuesCell(
                        string.Format("{0:N0}<br/>", dtOutcomes.Rows[0][outcomesCount + 1]),
                        string.Format("{0:P2}", dtOutcomes.Rows[0][outcomesCount + 2]));


                    row.Cells.Add(cell);
                    row.Style["border-bottom"] = "#000000 2px solid";
                    table.Rows.Add(row);

                    // Хинтовая строка
                    if (!String.IsNullOrEmpty(hintRowText))
                    {
                        row = new TableRow();
                        cell = new TableCell();
                        if (name == "Прочие расходы")
                        {
                            row.Style["border-bottom"] = "#323232 4px solid";
                        }
                        cell.CssClass = "HtmlTableCompact";
                        Label lb = new Label();
                        lb.Text = hintRowText;
                        lb.Font.Italic = true;
                        lb.CssClass = "ServeText";
                        cell.Controls.Add(lb);
                        cell.VerticalAlign = VerticalAlign.Middle;
                        cell.HorizontalAlign = HorizontalAlign.Right;
                        cell.ColumnSpan = 3;
                        cell.Style["border-top-style"] = "none";
                        row.Cells.Add(cell);
                        row.Style["border-top-style"] = "none";
                        table.Rows.Add(row);
                    }
                }
            }
        }


        private void GetData()
        {
            string query = DataProvider.GetQueryText("data_H");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "Показатель", dtData);           

            query = DataProvider.GetQueryText("Outcomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "Показатель", dtOutcomes);
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

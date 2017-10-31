using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image=System.Web.UI.WebControls.Image;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0001_h : CustomReportPage
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

        public int Devider
        {
            get
            {
                return  RegionSettingsHelper.Instance.Name.ToLower().Contains("ямало") ? 12 : 3;
            }
        }

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

                if (IsQuaterPlanType)
                {
                    lbQuater.Text = " за " + CRHelper.PeriodDescr(date, 3);
                    TextBox1.Text = "(на " + date.ToString("dd.MM.yyyy") + "), тыс.руб.";
                }
                else
                {
                    lbQuater.Visible = false;
                    TextBox1.Text = "на " + date.ToString("dd.MM.yyyy") + ", тыс.руб.";
                }
                Label2.Text = string.Format("данные на {0} {1} {2} года", dtDate.Rows[0][4], CRHelper.RusMonthGenitive(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())), dtDate.Rows[0][0]);
            }
            else
            {
                date = new DateTime(
                    Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                    CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                    CRHelper.MonthLastDay(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));

                if (IsQuaterPlanType)
                {
                    lbQuater.Text = " за " + CRHelper.PeriodDescr(date, 3);
                    TextBox1.Text = "(за " + dtDate.Rows[0][3].ToString().ToLower() + " " + dtDate.Rows[0][0] + " года" + "), тыс.руб.";
                }
                else
                {
                    lbQuater.Text = string.Empty;
                    TextBox1.Text = "за " + dtDate.Rows[0][3].ToString().ToLower() + " " + dtDate.Rows[0][0] + " года" + ", тыс.руб.";
                }
                Label2.Text = string.Format("данные за {0} {1} года", dtDate.Rows[0][3].ToString().ToLower(), dtDate.Rows[0][0]);
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
            if (IsQuaterPlanType)
            {
                othersEthalon = CRHelper.QuarterDaysCountToDate(date) / CRHelper.QuarterDaysCount(date);
            }
            else
            {
                othersEthalon = 1 - CommonAssessionLimit();
            }

            commonEthalon = 1 - CommonAssessionLimit();
            salaryEthalon = 1 - SalaryAssessionLimit();
            GetData();
            MakeHtmlTable();
            
            Label1.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
        }
        
        private TableCell GetPlanCell(string value)
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
            return cell;
        }

        private void MakeHtmlTable()
        {
            TableCell cell;
            TableRow row;
            AddHeaderRow();

            int startIndex = 1;

            if (IsQuaterPlanType)
            {
                row = new TableRow();
                cell = GetNameCell(dtData.Rows[0][0].ToString());
                row.Cells.Add(cell);

                cell = GetPlanCell(String.Format("{0:N0}", dtData.Rows[0][1]));
                row.Cells.Add(cell);

                cell = GetIndicatorCell();
                row.Cells.Add(cell);

                cell = GetValuesCell(string.Format("{0:N0}<br/>", dtData.Rows[0][2]), string.Empty);
                row.Cells.Add(cell);
                table.Rows.Add(row);
            }
            else
            {
                startIndex = 0;
            }

            for (int i = startIndex; i < dtData.Rows.Count; i++)
            {
                if (IsOmsk && dtData.Rows[i][0] != null)
                {
                    string rowName = dtData.Rows[i][0].ToString();

                    rowName = rowName.Replace("Остаток средств на начало квартала", "Остаток средств на начало года");
                    rowName = rowName.Replace("Остаток средств на конец квартала", "Остаток средств на дату");

                    dtData.Rows[i][0] = rowName;
                }

                if (dtData.Rows[i][0] != DBNull.Value)
                {
                    dtData.Rows[i][0] = dtData.Rows[i][0].ToString().Replace("областного", "окружного");
                }

                if (dtData.Rows[i][0].ToString() == "Расходы - всего")
                {
                    addInnerTable(i);
                    continue;
                }
                row = new TableRow();

                if (dtData.Rows[i][0].ToString() ==
                        "в том числе:")
                {
                    cell = GetTextBoxCell("в том числе:");
                    cell.ColumnSpan = 4;
                    row.Cells.Add(cell);
                    table.Rows.Add(row);
                    continue;
                }

                if (dtData.Rows[i][0].ToString() ==
                        "Поступления и выплаты из источников финансирования дефицита областного бюджета - всего")
                {
                    cell = GetTextBoxCell("Поступления и выплаты из источников финансирования", "дефицита областного бюджета - всего");
                    cell.ColumnSpan = 4;
                    row.Cells.Add(cell);
                    table.Rows.Add(row);
                    continue;
                }

                cell = GetNameCell(dtData.Rows[i][0].ToString());
                row.Cells.Add(cell);

                cell = GetPlanCell(String.Format("{0:N0}", dtData.Rows[i][1]));
                row.Cells.Add(cell);

                cell = GetIndicatorCell();
                row.Cells.Add(cell);
                switch (dtData.Rows[i][0].ToString())
                {
                    case ("КАССОВЫЕ ПОСТУПЛЕНИЯ - ВСЕГО"):
                    case ("КАССОВЫЕ ВЫПЛАТЫ - ВСЕГО"):
                        {
                            row.Style["border-top"] = "#323232 4px solid";
                            row.Style["border-bottom"] = "#323232 4px solid";
                            break;
                        }
                    case ("Доходы областного бюджета"):
                        {
                            row.Style["border-bottom"] = "transparent 1px solid";
                            break;
                        }
                    case ("Остаток средств на конец квартала"):
                        {
                            row.Style["border-top"] = "#323232 4px solid";
                            break;
                        }
                }
                cell = GetValuesCell(string.Format("{0:N0}<br/>", dtData.Rows[i][2]), string.Format("{0:P2}", dtData.Rows[i][4]));
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

        private TableCell GetTextBoxCell(string name)
        {
            TableCell cell;
            TextBox lb;
            cell = new TableCell();
            cell.Style["padding-left"] = "1px";
            cell.CssClass = "HtmlTableCompact";
            lb = new TextBox();
            lb.Text = name;
            lb.Height = 16;
            lb.ReadOnly = true;
            lb.SkinID = "TextBoxTableFontGrey";
            cell.Controls.Add(lb);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Left;
            return cell;
        }

        private TableCell GetTextBoxCell(string name1, string name2)
        {
            TableCell cell;
            TextBox lb;
            cell = new TableCell();
            cell.Style["padding-left"] = "1px";
            cell.CssClass = "HtmlTableCompact";
            lb = new TextBox();
            lb.Text = name1;
            lb.Width = 440;
            lb.Height = 12;
            lb.ReadOnly = true;
            lb.SkinID = "TextBoxTableFontGrey";
            cell.Controls.Add(lb);
            lb = new TextBox();
            lb.Text = name2;
            lb.Width = 400;
            lb.Height = 17;
            lb.ReadOnly = true;
            lb.SkinID = "TextBoxTableFontGrey";
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
            cell.Width = 80;
            cell.CssClass = "HtmlTableHeader";
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            cell.Text = "План";
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

            cell = GetPlanCell(String.Format("{0:N0}", dtData.Rows[i][1]));
            row.Cells.Add(cell);

            cell = GetIndicatorCell();
            row.Cells.Add(cell);

            cell = GetValuesCell(
                    string.Format("{0:N0}<br/>", dtData.Rows[i][2]),
                    string.Format("{0:P2}", dtData.Rows[i][4]));
            row.Cells.Add(cell);
            table.Rows.Add(row);

            if (IsYar || dtOutcomes.Rows.Count != 0)
            {
                row = new TableRow();
                cell = GetTextBoxCell("в том числе:");
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

                    cell = GetPlanCell(String.Format("{0:N0}", dtOutcomes.Rows[0][outcomesCount + 1]));
                    cell.Style["border-bottom-style"] = "none";
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
                    int textWidth = 347;
                    if (Double.TryParse(dtOutcomes.Rows[0][outcomesCount + 2].ToString(), out value))
                    {
                        Image image = new Image();
                        image.ImageUrl = value >= ethalon ? "~/images/green.png" : "~/images/red.png";
                        hintRowText = value >= ethalon
                                          ? String.Format("Соблюдается условие равномерности ({0:P2})", ethalon)
                                          : String.Format("Не соблюдается условие равномерности ({0:P2})", ethalon);
                        textWidth = value >= ethalon ? 327 : 347;
                        cell.Controls.Add(image);
                    }

                    cell.Style["border-right-style"] = "none";
                    cell.VerticalAlign = VerticalAlign.Middle;
                    cell.HorizontalAlign = HorizontalAlign.Left;
                    cell.Style["border-bottom-style"] = "none";
                    row.Cells.Add(cell);

                    cell = GetValuesCell(
                        string.Format("{0:N0}<br/>", dtOutcomes.Rows[0][outcomesCount + 1]),
                        string.Format("{0:P2}", dtOutcomes.Rows[0][outcomesCount + 2]));

                    cell.Style["border-bottom-style"] = "none";
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
                        TextBox lb = new TextBox();
                        lb.Text = hintRowText;
                        lb.Height = 16;
                        lb.Font.Italic = true;
                        lb.SkinID = "TextBoxServeText";
                        lb.ReadOnly = true;
                        lb.Width = textWidth;
                        cell.Controls.Add(lb);
                        cell.VerticalAlign = VerticalAlign.Middle;
                        cell.HorizontalAlign = HorizontalAlign.Right;
                        cell.ColumnSpan = 4;
                        cell.Style["padding-right"] = "1px";
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
            double factValue;
            DataRow row = dtData.Rows[0];


            
            if (IsYar)
            {
                row[1] = DBNull.Value;
            }
            else if (row[1] != DBNull.Value && row[1].ToString() != String.Empty)
            {
                row[1] = Convert.ToDouble(row[1]) / 1000;
            }
            if (Double.TryParse(row[2].ToString(), out factValue))
            {
                row[2] = factValue / 1000;
            }
            for (int i = 1; i < dtData.Rows.Count; i++)
            {
                row = dtData.Rows[i];
                double planValue;
                if (Double.TryParse(row[1].ToString(), out planValue))
                {
                    row[1] = planValue / 1000;
                }
                if (Double.TryParse(row[2].ToString(), out factValue))
                {
                    row[2] = factValue / 1000;
                }
            }

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
                return (monthNum - 1) / Devider;
            }
            if (day < 21)
            {
                // Была одна выплата
                return (monthNum - 1) / Devider + 1.0 / (Devider * 2);
            }
            // Все выплаты
            return (monthNum) / Devider;
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
                return (monthNum) / Devider;
            }
            else
            {
                return (monthNum - 1) / Devider;
            }
        }
    }
}

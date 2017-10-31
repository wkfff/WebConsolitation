using System;
using System.Data;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0021_v : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtData = new DataTable();
        private DataTable dtOutcomes = new DataTable();
        private DateTime date;
        private double othersEthalon;
        private double salaryEthalon;
        private double commonEthalon;

        #region ��������� �������

        // ���� ����
        private CustomParam measurePlan;
        // ���� ����
        private CustomParam measureFact;

        #endregion

        protected override void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            #region ������������� ���������� �������

            measurePlan = UserParams.CustomParam("measure_plan");
            measureFact = UserParams.CustomParam("measure_fact");

            #endregion

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][5].ToString();
            if (!dtDate.Rows[0][4].ToString().Contains("�������������� �������"))
            {
                date = new DateTime(
                       Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                       CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                       Convert.ToInt32(dtDate.Rows[0][4].ToString()));
               
                lbQuater.Text = string.Empty;
                lbDate.Text = "&nbsp;��&nbsp;" + date.ToString("dd.MM.yyyy") + ", ���.���.";
                
                Label.Text = string.Format("������ �� {0} {1} {2} ����", dtDate.Rows[0][4], CRHelper.RusMonthGenitive(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())), dtDate.Rows[0][0]);
            }
            else
            {
                date = new DateTime(
                    Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                    CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                    CRHelper.MonthLastDay(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));
                
                
                lbQuater.Text = string.Empty;
                lbDate.Text = " ��&nbsp;" + dtDate.Rows[0][3].ToString().ToLower() + " " + dtDate.Rows[0][0] + " ����" + ", ���.���.";
                
                Label.Text = string.Format("������ �� {0} {1} ����", dtDate.Rows[0][3].ToString().ToLower(), dtDate.Rows[0][0]);
            }
            
            othersEthalon = CRHelper.QuarterDaysCountToDate(date) / CRHelper.QuarterDaysCount(date);
            commonEthalon = CommonAssessionLimit();
            salaryEthalon = SalaryAssessionLimit();
          //  GetData();
            MakeHtmlTable();
            LabelDate.Text =
                string.Format("��������� {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
        }

        private void MakeHtmlTable()
        {
            AddHeaderRow();

            int startIndex = 0;

            dtData = new DataTable();
            string query = DataProvider.GetQueryText("data_H_incomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "����������", dtData);

            for (int i = startIndex; i < dtData.Rows.Count; i++)
            {
                TableRow row = new TableRow();
                TableCell cell;

                //if (dtData.Rows[i][0].ToString() == "������� - �����")
                //{
                //    addInnerTable(i);
                //    continue;
                //}

                //if (dtData.Rows[i][0].ToString() ==
                //        "� ��� �����:")
                //{
                //    cell = GetTextBoxCell("� ��� �����:");
                //    cell.ColumnSpan = 4;
                //    row.Cells.Add(cell);
                //    table.Rows.Add(row);
                //    continue;
                //}

                //if (dtData.Rows[i][0].ToString() ==
                //        "����������� � ������� �� ���������� �������������� �������� ���������� ������� - �����")
                //{
                //    cell = GetTextBoxCell("����������� � ������� �� ���������� ��������������", "�������� ���������� ������� - �����");
                //    cell.ColumnSpan = 4;
                //    row.Cells.Add(cell);
                //    table.Rows.Add(row);
                //    continue;
                //}

                //if (dtData.Rows[i][0].ToString() == "������� ������� �� ������ ��������")
                //{
                //    dtData.Rows[i][0] = "������� ������� �� ������ ����";
                //}

                //if (dtData.Rows[i][0].ToString() == "������� ������� �� ����� ��������")
                //{
                //    dtData.Rows[i][0] = "������� ������� ";
                //}

                cell = GetNameCell(dtData.Rows[i][0].ToString());
                row.Cells.Add(cell);

                cell = GetValuesCell(string.Format("{0:N0}<br/>", dtData.Rows[i][3]), string.Format("{0:P2}", dtData.Rows[i][4]));
                row.Cells.Add(cell);

                //cell = GetIndicatorCell();
                //row.Cells.Add(cell);
                //switch (dtData.Rows[i][0].ToString())
                //{
                //    case ("�������� ����������� - �����"):
                //    case ("�������� ������� - �����"):
                //        {
                //            row.Style["border-top"] = "#323232 4px solid";
                //            row.Style["border-bottom"] = "#323232 4px solid";
                //            break;
                //        }
                //    case ("������ ���������� �������"):
                //        {
                //            row.Style["border-bottom"] = "transparent 1px solid";
                //            break;
                //        }
                //    case ("������� ������� "):
                //        {
                //            row.Style["border-top"] = "#323232 4px solid";
                //            dtData.Rows[i][0] = "������� �������";
                //            break;
                //        }
                //}

                table.Rows.Add(row);
            }

            dtData = new DataTable();
            query = DataProvider.GetQueryText("data_H_outcomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "����������", dtData);

            FillTable(startIndex);

            dtData = new DataTable();
            query = DataProvider.GetQueryText("data_H_grbs");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "����������", dtData);

            FillTable(startIndex);

            dtData = new DataTable();
            query = DataProvider.GetQueryText("data_H_offset");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "����������", dtData);

            FillTable(startIndex);
        }

        private void FillTable(int startIndex)
        {
            for (int i = startIndex; i < dtData.Rows.Count; i++)
            {
                TableRow row = new TableRow();
                TableCell cell;

                cell = GetNameCell(dtData.Rows[i][0].ToString());
                row.Cells.Add(cell);

                cell = GetValuesCell(string.Format("{0:N0}<br/>", dtData.Rows[i][3]), string.Format("{0:P2}", dtData.Rows[i][4]));
                row.Cells.Add(cell);
                table.Rows.Add(row);
            }
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
            cell.Width = 170;
            cell.CssClass = "HtmlTableHeader";
            cell.Text = "����������";
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 70;
            cell.CssClass = "HtmlTableHeader";
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            cell.Text = "���� � %&nbsp;����������";
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

            if (false || dtOutcomes.Rows.Count != 0)
            {
                row = new TableRow();
                cell = GetNameCell("� ��� �����:");
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
                        case ("���������� �����"):
                            {
                                ethalon = salaryEthalon;
                                break;
                            }
                        case ("������ �������"):
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
                                          ? String.Format("����������� ������������� ({0:P2})", ethalon)
                                          : String.Format("�� ����������� ������������� ({0:P2})", ethalon);
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

                    // �������� ������
                    if (!String.IsNullOrEmpty(hintRowText))
                    {
                        row = new TableRow();
                        cell = new TableCell();
                        if (name == "������ �������")
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
                query, "����������", dtData);
            double factValue;
            DataRow row = dtData.Rows[0];
            row[1] = DBNull.Value;
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
                query, "����������", dtOutcomes);
        }


        /// <summary>
        /// ��������� �������� ������ ��������.
        /// </summary>
        /// <returns></returns>
        private double SalaryAssessionLimit()
        {
            // ����� ����� ������ � ��������
            double monthNum = CRHelper.MonthNumInQuarter(date.Month);
            int day = date.Day;
            if (day < 6)
            {
                // ������ � ���� ������ �� ����
                return (monthNum - 1) / 3;
            }
            if (day < 21)
            {
                // ���� ���� �������
                return (monthNum - 1) / 3 + 1.0 / 6;
            }
            // ��� �������
            return (monthNum) / 3;
        }

        /// <summary>
        /// ��������� �������� ������ ���������.
        /// </summary>
        /// <returns></returns>
        private double CommonAssessionLimit()
        {
            // ����� ����� ������ � ��������
            double monthNum = CRHelper.MonthNumInQuarter(date.Month);
            // ���� ��������� ���� ������
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

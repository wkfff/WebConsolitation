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
using Image = System.Web.UI.WebControls.Image;
using System.Web.UI.HtmlControls;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0135_0007 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtData = new DataTable();

        private double salaryEthalon;
        private double basicActivesEthalon;
        private double communalEthalon;
        private double mbtEthalon;

        private DateTime date;

        #region ��������� �������

        // ���� ����
        private CustomParam measurePlan;
        // ���� ����
        private CustomParam measureFact;
        // ���� ������� �� ������
        private CustomParam measureStartBalance;
        // ���� ������� �� �����
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
                return RegionSettingsHelper.Instance.Name.ToLower() == "����������� �������";
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            #region ������������� ���������� �������

            measurePlan = UserParams.CustomParam("measure_plan");
            measureFact = UserParams.CustomParam("measure_fact");
            measureStartBalance = UserParams.CustomParam("measure_start_balance");
            measureEndBalance = UserParams.CustomParam("measure_end_balance");

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

                if (IsQuaterPlanType)
                {
                    lbQuater.Text = "&nbsp;��&nbsp;<span style='color: white;'>" + CRHelper.PeriodDescr(date, 3) + "</span>";
                    lbQuater.Visible = false;
                    lbDate.Text = " �� ������� �������������� ������� ������� ����������� ������ ��&nbsp;<span style='color: white;'>" + date.ToString("dd.MM.yyyy") + "</span>";
                }
                else
                {
                    lbQuater.Text = string.Empty;
                    lbDate.Text = "&nbsp;��&nbsp;<span style='color: white;'>" + date.ToString("dd.MM.yyyy") + "</span>, ���.���.";
                }
            }
            else
            {
                date = new DateTime(Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                        CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                        CRHelper.MonthLastDay(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())));

                if (IsQuaterPlanType)
                {
                    lbQuater.Text = "&nbsp;�� " + CRHelper.PeriodDescr(date, 3);
                    lbDate.Text = "<br/>��&nbsp;<span style='color: white;'>" + dtDate.Rows[0][3].ToString().ToLower() + " " + dtDate.Rows[0][0] + " ����</span>" + ", ���.���.";
                }
                else
                {
                    lbQuater.Text = string.Empty;
                    lbDate.Text = " ��&nbsp;<span style='color: white;'>" + dtDate.Rows[0][3].ToString().ToLower() + " " + dtDate.Rows[0][0] + " ����</span>" + ", ���.���.";
                }
            }

            if (IsQuaterPlanType)
            {
                measurePlan.Value = "����";
                measureFact.Value = "����";
            }
            else
            {
                measurePlan.Value = "����_����������� ����";
                measureFact.Value = "����_����������� ����";
            }

            measureStartBalance.Value = (IsYar) ? "������� ������� �� ������ ��������" : RegionSettingsHelper.Instance.CashPlanBalance;
            measureEndBalance.Value = (measureStartBalance.Value == "������� �������")
                                          ? measureStartBalance.Value
                                          : "������� ������� �� ����� ��������";

            query = DataProvider.GetQueryText("data_v");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "name", dtData);

            salaryEthalon = Convert.ToDouble(dtData.Rows[dtData.Rows.Count - 1][1]);
            basicActivesEthalon = Convert.ToDouble(dtData.Rows[dtData.Rows.Count - 1][2]);
            communalEthalon = Convert.ToDouble(dtData.Rows[dtData.Rows.Count - 1][3]);
            mbtEthalon = Convert.ToDouble(dtData.Rows[dtData.Rows.Count - 1][4]);

            dtData.Rows.RemoveAt(dtData.Rows.Count - 1);

            MakeHtmlTableDetailTable();
        }

        private void MakeHtmlTableDetailTable()
        {
            TagClouddetailTable.CssClass = "HtmlTableCompact";

            AddHeaderRow(TagClouddetailTable);

            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                TableRow row = new TableRow();
                TableCell cell;

                cell = GetNameCell(dtData.Rows[i][0].ToString(), false);
                row.Cells.Add(cell);

                cell = GetGreyValueCell(string.Format("{0:N0}", dtData.Rows[i][5]));
                row.Cells.Add(cell);

                cell = GetValueCell(string.Format("{0:N0}", dtData.Rows[i][6]));
                row.Cells.Add(cell);

                cell = GetValueCell(string.Format("{0:P0}", dtData.Rows[i][7]));
                row.Cells.Add(cell);

                cell = GetCell(i, 1);
                row.Cells.Add(cell);
                cell = GetCell(i, 2);
                row.Cells.Add(cell);
                cell = GetCell(i, 3);
                row.Cells.Add(cell);
                cell = GetCell(i, 4);
                row.Cells.Add(cell);

                TagClouddetailTable.Rows.Add(row);
            }
        }

        private TableCell GetCell(int i, int column)
        {
            TableCell cell;
            Image image = new Image();
            cell = new TableCell();

            double ethalon;

            switch (column)
            {
                case 1:
                    {
                        ethalon = salaryEthalon;
                        break;
                    }
                case 2:
                    {
                        ethalon = basicActivesEthalon;
                        break;
                    }
                case 3:
                    {
                        ethalon = communalEthalon;
                        break;
                    }
                case 4:
                    {
                        ethalon = mbtEthalon;
                        break;
                    }
                default:
                    {
                        ethalon = mbtEthalon;
                        break;
                    }
            }

            HtmlGenericControl div = new HtmlGenericControl("div");

            div.ID = String.Format("TagCloud_tag{0}_{1}", i, column);

            image.ImageUrl = GetImageUrl(ethalon, i, column);
            if (image.ImageUrl != String.Empty)
            {
                div.Controls.Add(image);
            }

            TooltipHelper.AddToolTip(div, GetImageHint(ethalon, i, column), this.Page);

            cell.Controls.Add(div);

            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            return cell;
        }

        private string GetImageUrl(double ethalon, int row, int column)
        {
            double value;
            if (Double.TryParse(dtData.Rows[row][column].ToString(), out value))
            {
                return value >= ethalon ? "~/images/ballGreenBB.png" : "~/images/ballRedBB.png";
            }
            return String.Empty;
        }

        private string GetImageHint(double ethalon, int row, int column)
        {
            double value;
            if (Double.TryParse(dtData.Rows[row][column].ToString(), out value))
            {
                return value >= ethalon ?
                        String.Format("<span style=\"font-family: Arial; font-size: 14pt\">������� ����������&nbsp<b>{0:P0}</b>&nbsp���� �������� �� ���� (<b>{1:P0}</b>)</span>", dtData.Rows[row][column], ethalon) :
                        String.Format("<span style=\"font-family: Arial; font-size: 14pt\">������� ����������&nbsp<b>{0:P0}</b>&nbsp���� �������� �� ���� (<b>{1:P0}</b>)</span>", dtData.Rows[row][column], ethalon);
            }
            return String.Empty;
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
            lb.Text = String.Format("<a href='webcommand?showPopoverReport=fo_0135_0008_GRBS={1}&width=690&height=530&fitByHorizontal=true'>{0}</a>", name, CustomParams.GetGrbsIdByName(name));

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
            cell.Text = "������� �������������";

            SetupHeaderCell(cell);

            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 70;
            SetupHeaderCell(cell);
            cell.Text = "����, ���.���.";
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 70;
            SetupHeaderCell(cell);
            cell.Text = "����, ���.���.";
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 30;
            SetupHeaderCell(cell);
            cell.Text = "% ���.";
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 30;
            SetupHeaderCell(cell);
            cell.Text = "�����. �����";
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 30;
            SetupHeaderCell(cell);
            cell.Text = "���. ��������";
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 30;
            SetupHeaderCell(cell);
            cell.Text = "������. ������";

            row.Cells.Add(cell);

            cell = new TableCell();
            cell.Width = 30;
            SetupHeaderCell(cell);
            cell.Text = "�������. ������";

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

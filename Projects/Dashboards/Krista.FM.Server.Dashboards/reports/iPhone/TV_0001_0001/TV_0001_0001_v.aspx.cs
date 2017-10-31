using System;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class TV_0001_0001_v : CustomReportPage
    {
        private DataTable dtMain = new DataTable();
        private DateTime reportDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            reportDate = new DateTime(2010, 6, 30);

            string query = DataProvider.GetQueryText("TV_0001_0001_V");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "���", dtMain);

            HtmlTable tableTasks = new HtmlTable();
            HtmlTable tableActions = new HtmlTable();
            HtmlTable tableEvents = new HtmlTable();

            for (int i = 0; i < dtMain.Rows.Count; i++ )
            {
                DataRow row = dtMain.Rows[i];
                if (row["�������"].ToString() == "������")
                {
                    Label label = new Label();
                    label.CssClass = "ServeText";

                    DateTime dateStart;
                    DateTime dateEnd;

                    string realisationTime = String.Empty;

                    if (DateTime.TryParse(row["���� ���������� ������"].ToString(), out dateStart) &&
                       DateTime.TryParse(row["���� ���������� ���������"].ToString(), out dateEnd))
                    {
                        realisationTime = String.Format("<span style='color: white;'><b>���� ����������:</b></span>&nbsp; � {0:dd.MM.yyyy} �� {1:dd.MM.yyyy}", dateStart, dateEnd);
                    }

                    string financing = "���";
                    if (!ZeroFinancing(row))
                    {
                        financing = GetFinancing(row);
                    }

                    label.Text = String.Format("<img src=\"../../../images/television.png\">&nbsp;<span style='color: white; font-size: 18px'><b>{0}</b></span>&nbsp;<span style='color: white; font-size: 18px'><b>{1}</b></span><br/><span style='color: white;'><b>�����������:&nbsp;</b></span> {2}<br/>{3}<br/><span style='color: white;'><b>% ����������:</b></span>&nbsp;{8}&nbsp;����&nbsp;{4:N0}% (����&nbsp;{5:N0}%)<br/><span style='color: white;'><b>��������������:&nbsp;</b>{7}</span>{6}",
                        row["�����������"], row["���"], row["�����������"], realisationTime, row["������� ���������� ����"], row["������� ���������� ����"],
                        financing, LessFinance(row, String.Empty),
                        Math.Round(Convert.ToDouble(row["������� ���������� ����"])) < Math.Round(Convert.ToDouble(row["������� ���������� ����"])) ? "<span style='color: red;'><b>�������������</b>&nbsp;</span><img src=\"../../../images/bell.png\">&nbsp;" : "");

                    tableTasks = new HtmlTable();
                    PlaceHolder1.Controls.Add(tableTasks);

                    HtmlTableRow htmlRow = new HtmlTableRow();
                    HtmlTableCell htmlCell = new HtmlTableCell();
                    htmlRow.Cells.Add(htmlCell);
                    tableTasks.Rows.Add(htmlRow);
                    htmlCell.Style.Add("background-image", "url(../../../images/CollapseIpad.png)");
                    htmlCell.Style.Add("background-repeat", "no-repeat;");
                    htmlCell.Attributes.Add("onclick", "resize(this)");
                    htmlCell.Style.Add("padding-left", "20px");
                    htmlCell.Style.Add("padding-bottom", "15px");
                    htmlCell.Controls.Add(label);
                }
                else if (row["�������"].ToString() == "�����������")
                {
                    Label label = new Label();
                    label.CssClass = "ServeText";

                    DateTime dateStartPlan;
                    DateTime dateEndPlan;
                    DateTime dateStartFact;
                    DateTime dateEndFact;

                    string status = "���������";
                    string indication = "<img src=\"../../../images/date.png\">&nbsp;";

                    DateTime.TryParse(row["���� ���������� �������� ���������"].ToString(), out dateEndPlan);
                    DateTime.TryParse(row["���� ���������� �������� ������"].ToString(), out dateStartPlan);

                    string realisationTime = String.Format("<span style='color: white;'><b>���� ����������:</b></span>&nbsp;<br/>�������� � {0:dd.MM.yyy} �� {1:dd.MM.yyy}", dateStartPlan, dateEndPlan);

                    // ���� ����&nbsp;����������
                    if (DateTime.TryParse(row["���� ���������� ����������� ���������"].ToString(), out dateEndFact) &&
                        dateEndFact.Year > 2008)
                    {
                        if (DateTime.TryParse(row["���� ���������� �������� ���������"].ToString(), out dateEndPlan))
                        {
                            // � ����&nbsp;� ����&nbsp;������ �������
                            if (reportDate > dateEndPlan &&
                                reportDate > dateEndFact)
                            {
                                indication = "<img src=\"../../../images/accept.png\">&nbsp;";
                                status = "���������� � ����";

                                DateTime.TryParse(row["���� ���������� ����������� ������"].ToString(), out dateStartFact);
                                realisationTime = String.Format("<span style='color: white;'><b>���� ����������:</b></span><br/>� {0:dd.MM.yyy} �� {1:dd.MM.yyy}", dateStartFact, dateEndFact);
                            }
                        }
                    }

                    // ���� ����&nbsp;����������
                    if (DateTime.TryParse(row["���� ���������� ����������� ���������"].ToString(), out dateEndFact) &&
                        dateEndFact.Year > 2008)
                    {
                        if (DateTime.TryParse(row["���� ���������� �������� ���������"].ToString(), out dateEndPlan))
                        {
                            // � ����&nbsp;������ ������� ����� �����
                            if (reportDate > dateEndFact &&
                                dateEndFact > dateEndPlan)
                            {
                                indication = "<img src=\"../../../images/accept.png\">&nbsp;";
                                status = "���������� ������� ���������������� �����";

                                DateTime.TryParse(row["���� ���������� �������� ������"].ToString(), out dateStartPlan);
                                DateTime.TryParse(row["���� ���������� �������� ���������"].ToString(), out dateEndPlan);
                                DateTime.TryParse(row["���� ���������� ����������� ������"].ToString(), out dateStartFact);

                                realisationTime = String.Format("<span style='color: white;'><b>���� ����������:</b></span>&nbsp;<span style='color: red;'>�������&nbsp;</span><img src=\"../../../images/clock_red.png\"><br/>����������� c {0:dd.MM.yyyy} �� {1:dd.MM.yyyy}<br/>��������������� c {2:dd.MM.yyyy} �� {3:dd.MM.yyyy}", dateStartFact, dateEndFact, dateStartPlan, dateEndPlan);

                            }
                        }
                    }

                    // ��� ����� ����������
                    if (!DateTime.TryParse(row["���� ���������� ����������� ���������"].ToString(), out dateEndFact) ||
                        (DateTime.TryParse(row["���� ���������� ����������� ���������"].ToString(), out dateEndFact) && dateEndFact.Year < 2009))
                    {
                        if (DateTime.TryParse(row["���� ���������� �������� ���������"].ToString(), out dateEndPlan))
                        {
                            // � ����&nbsp;����� �������
                            if (reportDate > dateEndPlan)
                            {
                                indication = "<img src=\"../../../images/cancel.png\">&nbsp;";
                                status = "�� ����������";

                                DateTime.TryParse(row["���� ���������� ����������� ������"].ToString(), out dateStartPlan);
                                realisationTime = String.Format("<span style='color: white;'><b>���� ����������:</b></span><br/>��������������� � {0:dd.MM.yyy} �� {1:dd.MM.yyy}", dateStartPlan, dateEndPlan);
                            }
                        }

                        if (DateTime.TryParse(row["���� ���������� ����������� ������"].ToString(), out dateStartFact))
                        {
                            // � ����&nbsp;����� �������
                            if (reportDate > dateStartFact)
                            {
                                indication = "<img src=\"../../../images/inProc.png\">&nbsp;";
                                status = "����������� �����������";
                                DateTime.TryParse(row["���� ���������� ����������� ������"].ToString(), out dateStartPlan);
                                realisationTime = String.Format("<span style='color: white;'><b>���� ����������:</b></span><br/>��������������� � {0:dd.MM.yyy} �� {1:dd.MM.yyy}<br/>����������� � {2:dd.MM.yyy}", dateStartPlan, dateEndPlan, dateStartFact);
                            }
                        }
                    }

                    string financing = "���";
                    if (!ZeroFinancing(row))
                    {
                        financing = GetFinancing(row);
                    }

                    label.Text = String.Format("{7}<span style='color: white; font-size: 18px'><b>{0}</b></span>&nbsp;<span style='color: white; font-size: 16px'><b>{1}</b></span><br/><span style='color: white'><b>�����������:</b></span>&nbsp;{2} <span style='color: white'><b><br/>������:</b></span>&nbsp;{8}<br/>{3}<br/><span style='color: white'><b>% ����������:</b></span>&nbsp;{9}����&nbsp;{4:N0}% (����&nbsp;{5:N0}%)<br/><span style='color: white'><b>��������������:&nbsp;</b></span>{10}{6}",
                        row["�����������"], row["���"], row["�����������"], realisationTime, row["������� ���������� ����"], row["������� ���������� ����"],
                        financing, indication, status.ToLower(), Convert.ToDouble(row["������� ���������� ����"]) < Convert.ToDouble(row["������� ���������� ����"]) ? "<span style='color: red;'><b>�������������</b>&nbsp;</span><img src=\"../../../images/bell.png\">&nbsp;" : "", LessFinance(row, status));

                    tableActions = new HtmlTable();
                    HtmlTableRow htmlRow = new HtmlTableRow();
                    htmlRow.Attributes.Add("class", "ReportRowFirstState");
                    HtmlTableCell htmlCell = new HtmlTableCell();
                    htmlRow.Cells.Add(htmlCell);
                    tableTasks.Rows.Add(htmlRow);
                    htmlCell.Controls.Add(tableActions);

                    htmlRow = new HtmlTableRow();
                    htmlCell = new HtmlTableCell();
                    htmlRow.Cells.Add(htmlCell);
                    tableActions.Rows.Add(htmlRow);
                    if (dtMain.Rows[i + 1]["�������"].ToString() == "�������� �������")
                    {
                        htmlCell.Style.Add("background-image", "url(../../../images/CollapseIpad.png)");
                        htmlCell.Style.Add("background-repeat", "no-repeat;");
                        htmlCell.Attributes.Add("onclick", "resize(this)");
                    }
                    htmlCell.Style.Add("padding-left", "40px");
                    htmlCell.Style.Add("padding-bottom", "15px");
                    htmlCell.Style.Add("background-position", "20px top");
                    htmlCell.Controls.Add(label);
                }
                #region �������
                else if (row["�������"].ToString() == "�������� �������")
                {
                    Label label = new Label();
                    label.CssClass = "ServeText";

                    DateTime datePlan;
                    DateTime dateFact;
                    DateTime dateKeyEvent = new DateTime(1, 1, 1);
                    string statusKeyEvent = "���������";
                    string indication = "<img src=\"../../../images/date.png\">&nbsp;";

                    // ���� ����&nbsp;����������
                    if (DateTime.TryParse(row["���� ��������� ������� ���� "].ToString(), out dateFact))
                    {
                        if (DateTime.TryParse(row["���� ��������� ������� ���� "].ToString(), out datePlan))
                        {
                            // � ����&nbsp;� ����&nbsp;������ �������
                            if (reportDate > datePlan &&
                                reportDate > dateFact)
                            {
                                indication = "<img src=\"../../../images/accept.png\">&nbsp;";
                                statusKeyEvent = "���������� � ����";
                            }
                        }
                    }

                    // ���� ����&nbsp;����������
                    if (DateTime.TryParse(row["���� ��������� ������� ���� "].ToString(), out dateFact))
                    {
                        if (DateTime.TryParse(row["���� ��������� ������� ���� "].ToString(), out datePlan))
                        {
                            // � ����&nbsp;������ ������� ����� �����
                            if (reportDate > dateFact &&
                                datePlan < dateFact)
                            {
                                indication = "<img src=\"../../../images/accept.png\">&nbsp;";
                                statusKeyEvent = "���������� ������� ���������������� �����";
                            }
                        }
                    }

                    // ��� ����� ����������
                    if (!DateTime.TryParse(row["���� ��������� ������� ���� "].ToString(), out dateFact))
                    {
                        if (DateTime.TryParse(row["���� ��������� ������� ���� "].ToString(), out datePlan))
                        {
                            // � ����&nbsp;����� �������
                            if (reportDate >= datePlan)
                            {
                                indication = "<img src=\"../../../images/cancel.png\">&nbsp;";
                                statusKeyEvent = "�� ����������";
                            }
                        }
                    }

                    if (DateTime.TryParse(row["���� ��������� ������� ���� "].ToString(), out datePlan))
                    {
                        dateKeyEvent = datePlan;
                    }
                    if (DateTime.TryParse(row["���� ��������� ������� ���� "].ToString(), out dateFact))
                    {
                        dateKeyEvent = dateFact;
                    }

                    label.Text = String.Format("{3}<span style='color: white'><b>{4:dd.MM.yyyy}<br/>{0}&nbsp;{1}</span><br/><span style='color: white'><b>�����������:</b></span>&nbsp;{2}&nbsp;<br/><span style='color: white'><b>������:</b></span>&nbsp;{5}<br/>{6}",
                            row["�����������"], row["���"], row["�����������"], indication, dateKeyEvent, statusKeyEvent.ToLower(),
                            statusKeyEvent == "���������� ������� ���������������� �����" ? String.Format("<span style='color: white;'><b>���� ����������:</b></span>&nbsp;<span style='color: red;'>�������&nbsp;</span><img src=\"../../../images/clock_red.png\"><br/>��������������� {0:dd.MM.yyyy}<br/>����������� {1:dd.MM.yyyy}", datePlan, dateFact) : "");

                    tableEvents = new HtmlTable();
                    HtmlTableRow htmlRow = new HtmlTableRow();
                    htmlRow.Attributes.Add("class", "ReportRowFirstState");
                    HtmlTableCell htmlCell = new HtmlTableCell();
                    htmlRow.Cells.Add(htmlCell);
                    tableActions.Rows.Add(htmlRow);
                    htmlCell.Controls.Add(tableEvents);

                    htmlRow = new HtmlTableRow();
                    htmlCell = new HtmlTableCell();
                    htmlRow.Cells.Add(htmlCell);
                    tableEvents.Rows.Add(htmlRow);
                    htmlCell.Style.Add("padding-left", "60px");
                    htmlCell.Style.Add("background-position", "40px top");
                    htmlCell.Controls.Add(label);
                }
                #endregion
            }
        }
        
        private static string GetFinancing(DataRow row)
        {
            return
                String.Format(
                    "<br/>�����: ����&nbsp;<span style='color: white;'><b>{0:N2}</b>&nbsp;</span> ���.���. (����&nbsp;<span style='color: white;'><b>{1:N2}</b>&nbsp;</span> ���.���.)<br/>�� ������-���� 2010 ����: ����&nbsp;<span style='color: white;'><b>{2:N2}</b>&nbsp;</span> ���.���. (����&nbsp;<span style='color: white;'><b>{3:N2}</b>&nbsp;</span> ���.���.)<br/>�� ���� 2010 ����: ����&nbsp;<span style='color: white;'><b>{4:N2}</b>&nbsp;</span> ���.���. (����&nbsp;<span style='color: white;'><b>{5:N2}</b>&nbsp;</span> ���.���.)",
                    row["�������������� ���� �����"], row["�������������� ���� �����"],
                    row["�������������� ���� ������� ���"], row["�������������� ���� ������� ���"],
                    row["�������������� ���� ������� �����"], row["�������������� ���� ������� �����"]);
                                      
        }

        private static string LessFinance(DataRow row, string status)
        {
            bool less = (status != "���������" &&
                         Convert.ToDouble(row["�������������� ���� ������� ���"].ToString()) <
                         Convert.ToDouble(row["�������������� ���� ������� ���"].ToString()));
            return less ? String.Format("<span style='color: red;'><b>�������������</b>&nbsp;</span><img src=\"../../../images/money.png\">") : String.Empty;
        }

        private static bool ZeroFinancing(DataRow row)
        {
            return (Convert.ToDouble(row["�������������� ���� �����"].ToString()) == 0 &&
                    Convert.ToDouble(row["�������������� ���� �����"].ToString()) == 0 &&
                    Convert.ToDouble(row["�������������� ���� ������� ���"].ToString()) == 0 &&
                    Convert.ToDouble(row["�������������� ���� ������� ���"].ToString()) == 0 &&
                    Convert.ToDouble(row["�������������� ���� ������� �����"].ToString()) == 0 &&
                    Convert.ToDouble(row["�������������� ���� ������� �����"].ToString()) == 0);
        }
    }
}
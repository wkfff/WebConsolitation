using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image = System.Web.UI.WebControls.Image;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0003_0011 : CustomReportPage
    {
        DateTime date;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("fo_0003_0001_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "����", dtDate);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][1].ToString();
            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);

            UserParams.PeriodYear.Value = date.Year.ToString();
            UserParams.PeriodLastYear.Value = date.AddYears(-1).Year.ToString();
            CustomParam periodLastLastYear = UserParams.CustomParam("period_last_last_year");
            CustomParam periodThreeYearAgo = UserParams.CustomParam("period_3_last_year");
            CustomParam periodFourYearAgo = UserParams.CustomParam("period_4_last_year");
            periodLastLastYear.Value = date.AddYears(-2).Year.ToString();
            periodThreeYearAgo.Value = date.AddYears(-3).Year.ToString();
            periodFourYearAgo.Value = date.AddYears(-4).Year.ToString();

            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("", date.AddYears(-1), 4);

            lbDescription.Text = GetIndicatorText();
        }

        private string GetIncomesHintText(string attention, DataTable dt, string controlId)
        {
            string hintText = String.Empty;

            if (attention == "����� � �����")
            {
                hintText = String.Format("<b>��� �������� �����</b><br/>� �������� ��&nbsp;<b>�����������</b>&nbsp;����� ���������� ������� ���������� ������������������ ������� ��&nbsp;<b>{0:yyyy}</b>&nbsp;���&nbsp;<b>({1})</b>&nbsp;�� ��������� ��&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;�.<br/><br/>���������� ��������� ������������ ��������������� ������.", date, Core.CustomParam.CustomParamFactory("control").Value);
            }
            else if (attention == "����� � �����")
            {
                hintText = String.Format("<b>��������������</b><br/>��&nbsp;<b>{1:yyyy}-{4:yyyy}</b>&nbsp;�� � �������� ����������� ������� �� ������ ����������:<ul style='margin-bottom: -35px; margin-top: 0px'><li>{1:yyyy} ��� � {0:yyyy}&nbsp;<b>{5:P0}</b>&nbsp;</li><li>{2:yyyy} ��� � {1:yyyy}&nbsp;<b>{6:P0}</b>&nbsp;</li><li>{3:yyyy} ��� � {2:yyyy}&nbsp;<b>{7:P0}</b>&nbsp;</li><li>{4:yyyy} ��� � {3:yyyy}&nbsp;<b>{8:P0}</b>&nbsp;</li></ul><br/><br/>���������� �������� ��������� ������� ������ ��������.",
                    date.AddYears(-5), date.AddYears(-4), date.AddYears(-3), date.AddYears(-2), date.AddYears(-1),
                    dt.Rows[0]["���� � �������� ���� "], dt.Rows[1]["���� � �������� ���� "], dt.Rows[2]["���� � �������� ���� "], dt.Rows[3]["���� � �������� ���� "]);
            }
            else if (attention == "���������")
            {
                string executePercent = Convert.ToDouble(dt.Rows[0]["���� � ���������� "].ToString()) < 0.01 ? "����� 1%" : String.Format("{0:P0}", dt.Rows[0]["���� � ���������� "]);
                string planPercent = Convert.ToDouble(dt.Rows[0]["���� � ���������� "].ToString()) < 0.01 ? "����� 1%" : String.Format("{0:P0}", dt.Rows[0]["���� � ���������� ����� "]);
                hintText = String.Format("<b>������� ����</b><br/>���������&nbsp;<b>���������</b>&nbsp;����� ������������������ ������� �������� �� �� �������&nbsp;<b>({0})</b><br/><br/><div style='margin-top: -15px; margin-bottom: -33px'>������� ���� ���������� ��&nbsp;<b>{1:yyyy}-{2:yyyy}</b>&nbsp;��.:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{3:P0}</b><br/>���� ����������� ����� ��&nbsp;<b>{4:yyyy}</b>&nbsp;��� �� ��������� ��&nbsp;<b>{11:dd.MM.yyyy}</b>&nbsp;�.:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{5:P0}</b>&nbsp;<br/><b>����</b>&nbsp;�������� ����� ���������� ��&nbsp;<b>{1:yyyy}-{2:yyyy}</b>&nbsp;��.</div><br/><br/><b>��������� �������:</b>&nbsp;{6}<br/><br/><b>���������:</b><br/>�������������������� ���������� ����� ����������� ����� �� �������� ����� ���������� ��&nbsp;<b>{1:yyyy}-{2:yyyy}</b>&nbsp;��.&nbsp;<b>{7:P0}</b>&nbsp;<br/>����� ���������&nbsp;<b>{8:P0}</b>&nbsp;<br/>��������� ��&nbsp;<b>{2:yyyy}</b>&nbsp;��� &nbsp;<b>{9:N0}</b>&nbsp;���.���. (���� � �������&nbsp;<b>{12}</b>)<br/>���������� ����&nbsp;<b>{4:yyyy}</b>&nbsp;���� �� ��������� ��&nbsp;<b>{11:dd.MM.yyyy}</b>&nbsp;�.&nbsp;<b>{10:N0}</b>&nbsp;���.���. (���� � �������&nbsp;<b>{13}</b>)",
                    Core.CustomParam.CustomParamFactory("control").Value, date.AddYears(-4), date.AddYears(-1), dt.Rows[0]["������� ���� ���������������"], date, dt.Rows[0]["���� ����������� ����� � �������� ���� "], GetIncomesGrownReasons(controlId), dt.Rows[0]["����� ���������������"], dt.Rows[0]["������� �������"], dt.Rows[0]["��������� �� ������� ��� "], dt.Rows[0]["���������� ���� "], date.AddMonths(1), executePercent, planPercent);
            }
            else if (attention == "���������")
            {
                string executePercent = Convert.ToDouble(dt.Rows[0]["���� � ���������� "].ToString()) < 0.01 ? "����� 1%" : String.Format("{0:P0}", dt.Rows[0]["���� � ���������� "]);
                string planPercent = Convert.ToDouble(dt.Rows[0]["���� � ���������� "].ToString()) < 0.01 ? "����� 1%" : String.Format("{0:P0}", dt.Rows[0]["���� � ���������� ����� "]);
                hintText = String.Format("<b>������� ����</b><br/>���������&nbsp;<b>���������</b>&nbsp;����� ������������������ ������� �������� �� �� �������&nbsp;<b>({0})</b><br/><br/><div style='margin-top: -15px; margin-bottom: -33px'>������� ���� ���������� ��&nbsp;<b>{1:yyyy}-{2:yyyy}</b>&nbsp;��.:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{3:P0}</b><br/>���� ����������� ����� ��&nbsp;<b>{4:yyyy}</b>&nbsp;��� �� ��������� ��&nbsp;<b>{11:dd.MM.yyyy}</b>&nbsp;�.:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{5:P0}</b>&nbsp;<br/><b>����</b>&nbsp;�������� ����� ���������� ��&nbsp;<b>{1:yyyy}-{2:yyyy}</b>&nbsp;��.</div><br/><br/><b>��������� �������:</b>&nbsp;{6}<br/><br/><b>���������:</b><br/>�������������������� ���������� ����� ����������� ����� �� �������� ����� ���������� ��&nbsp;<b>{1:yyyy}-{2:yyyy}</b>&nbsp;��.&nbsp;<b>{7:P0}</b>&nbsp;<br/>����� ���������&nbsp;<b>{8:P0}</b>&nbsp;<br/>��������� ��&nbsp;<b>{2:yyyy}</b>&nbsp;��� &nbsp;<b>{9:N0}</b>&nbsp;���.���. (���� � �������&nbsp;<b>{12}</b>)<br/>���������� ����&nbsp;<b>{4:yyyy}</b>&nbsp;���� �� ��������� ��&nbsp;<b>{11:dd.MM.yyyy}</b>&nbsp;�.&nbsp;<b>{10:N0}</b>&nbsp;���.���. (���� � �������&nbsp;<b>{13}</b>)",
                    Core.CustomParam.CustomParamFactory("control").Value, date.AddYears(-4), date.AddYears(-1), dt.Rows[0]["������� ���� ���������������"], date, dt.Rows[0]["���� ����������� ����� � �������� ���� "], GetIncomesFallenReasons(controlId), dt.Rows[0]["����� ���������������"], dt.Rows[0]["������ �������"], dt.Rows[0]["��������� �� ������� ��� "], dt.Rows[0]["���������� ���� "], date.AddMonths(1), executePercent, planPercent);
            }

            return hintText;
        }

        private string GetIncomesGrownReasons(string controlId)
        {
            string reason = String.Empty;

            switch (controlId)
            {
                case "1":
                    {
                        reason = "���������� �������� ������ ��������� �������� ���������� ��� ��������� ��������� ������ ��������� �����.";
                        break;
                    }
                case "2":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>� ������� ���������������� ����� �����������?</li><li>� ������� ���� ������� �����������?</li><li>��������� � ��������� ����������������?</li><li>��������� �������� ����������������� �������?</li></ul>";
                        break;
                    }
                case "3":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>���������� ���������� �����?</li><li>���������� ������ ��������� ���������?</li><li>������� � ������ ���������?</li><li>��������� � ��������� ����������������?</li><li>��������� �������� ����������������� �������?</li></ul>";
                        break;
                    }
                case "4":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>���������� �������� ��������������?</li><li>���������� ������� ����������� ���������?</li><li>���������� ����������� ���������������������� � ������������ ������������?</li><li>��������� � ��������� � ��������� ����������������?</li><li>��������� �������� ����������������� �������?</li></ul>";
                        break;
                    }
                case "5":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>������������� �������� ��������� ������ � �������� �������������������?</li><li>��������� � ��������� � ��������� ����������������?</li><li>��������� �������� ����������������� �������?</li></ul>";
                        break;
                    }
                case "6":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>������������� �������� ��������� ������ � �������� �������������������?</li><li>���������� ����� �������������� ����������������?</li><li>��������� �������� ����������������� �������?</li><li>��������� � ��������� � ��������� ����������������?</li></ul>";
                        break;
                    }
                case "7":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>������������� �������� ��������� ��������� ��������� � �����������-���������������������������?</li><li>��������� � ��������� � ��������� ����������������?</li><li>��������� �������� ����������������� �������?</li></ul>";
                        break;
                    }
                case "8":
                    {
                        reason = String.Empty;
                        break;
                    }
                case "9":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>����������� �������������?</li><li>���������� �������� ������� ������� �����������?</li><li>���������� ������������ ����������� �������?</li><li>����������� ����� �����������?</li><li>��������� � ��������� � ��������� ����������������?</li><li>��������� �������� ����������������� �������?</li></ul>";
                        break;
                    }
                case "10":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>���������� ������������ �������, ������������������ � �������?</li><li>���������� ������ ����� ��������� �������?</li><li>��������� � ��������� � ��������� ����������������?</li><li>��������� �������� ����������������� �������?</li></ul>";
                        break;
                    }
                case "11":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>������������ �����?</li><li>��������� � ��������� � ��������� ����������������?</li><li>��������� �������� ����������������� �������?</li></ul>";
                        break;
                    }
                case "12":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>������� ����� ������������� �������� ����������?</li><li>���������� ������� ������ �������� ����������?</li><li>��������� �������� ����������������� �������?</li><li>��������� � ��������� � ��������� ����������������?</li></ul>";
                        break;
                    }
            }

            return reason;
        }

        private string GetIncomesFallenReasons(string controlId)
        {
            string reason = String.Empty;

            switch (controlId)
            {
                case "1":
                    {
                        reason = "���������� �������� ������ ��������� �������� ���������� ��� ��������� ��������� ������ ��������� �����.";
                        break;
                    }
                case "2":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>� ������� �������� ������� � �����������?</li><li>����������� �����������?</li><li>�������� �����������-������������������?</li><li>���������� �������� ������������ �������, ����� � �����?</li><li>��������� � ��������� ����������������?</li></ul>";
                        break;
                    }
                case "3":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>���������� ����� ������ �����?</li><li>���������� ������ ��������� ���������?</li><li>������� ����������� �� �������� ������� �����?</li><li>�������� ������� �����������?</li><li>������ ��������?</li><li>��������� � ��������� ����������������?</li><li>�������� ������?</li></ul>";
                        break;
                    }
                case "4":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>���������� �������� ��������������?</li><li>���������� ������� ����������� ���������?</li><li>���������� ����������� ���������������������� � ������������ ������������?</li><li>��������� � ��������� ����������������?</li></ul>";
                        break;
                    }
                case "5":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>���������� ������������ ����� � ������� �����������?</li><li>��������� � ��������� ����������������?</li></ul>";
                        break;
                    }
                case "6":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>���������� ����� �������������� ����������������?</li><li>��������� � ��������� � ��������� ����������������?</li></ul>";
                        break;
                    }
                case "7":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>���������� ����� �����������-���������������������������?</li><li>��������� � ��������� � ��������� ����������������?</li></ul>";
                        break;
                    }
                case "8":
                    {
                        reason = String.Empty;
                        break;
                    }
                case "9":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>���������� �����������?</li><li>�������� ������� �����������?</li><li>��������� � ��������� � ��������� ����������������?</li></ul>";
                        break;
                    }
                case "10":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>���������� ������������ �������, ������������������ � �������?</li><li>���������� ������ ����� ��������� �������?</li><li>��������� � ��������� � ��������� ����������������?</li></ul>";
                        break;
                    }
                case "11":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>��������� � ��������� � ��������� ����������������?</li></ul>";
                        break;
                    }
                case "12":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>������� ������������� �������� ����������?</li><li>���������� ������� ������ �������� ����������?</li><li>��������� � ��������� � ��������� ����������������?</li></ul>";
                        break;
                    }
            }

            return reason;
        }


        private string GetOutcomesHintText(string attention, DataTable dt, string controlId)
        {
            string hintText = String.Empty;

            if (attention == "����� � �����")
            {
                hintText = String.Format("<b>��� ����������</b><br/>� �������� ��&nbsp;<b>�����������</b>&nbsp;������� ������������������ �������&nbsp;<b>{1}</b>&nbsp;�� ��������� ��&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;�.<br/><br/>���������� ��������� ������������ ��������������� ������.", date, Core.CustomParam.CustomParamFactory("control").Value);
            }            
            else if (attention == "���������")
            {
                hintText = String.Format("<b>���� ����� ���� ��������</b><br/><b>{0}</b>. ���� ���������� ������������������ ������� �������� �� ��&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;�. � ������������ �������&nbsp;<b>{2:yyyy}</b>&nbsp;����&nbsp;<b>����</b>&nbsp;�������� ����� ���������� �� ��.<br/><br/>������� ���� ���������� �� �� ��&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;�.:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{3:P0}</b><br/>���� ���������� ��&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;�.:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{4:P0}</b><br/><b>����</b>&nbsp;�������� ����� ���������� �� ��.<br/><br/><b>��������� �������:</b>&nbsp;{10}<br/><br/><b>���������:</b><br/>�������������������� ���������� ����� ���������� �������� �� �� �������� ����� ���������� �� �� �� ��������� ��&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;�.&nbsp;<b>{5:P0}</b><br/>����� ���������&nbsp;<b>{6:P0}</b><br/>��������� ��&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;�.&nbsp;<b>{7:N0}</b>&nbsp;���.���.<br/>��������� ��&nbsp;<b>{9:dd.MM.yyyy}</b>&nbsp;�.&nbsp;<b>{8:N0}</b>&nbsp;���.���.",
                    Core.CustomParam.CustomParamFactory("control").Value, date.AddMonths(1), date.AddYears(-1), dt.Rows[0]["������� ���� �� ��"], dt.Rows[0]["���� � �������� ����"], dt.Rows[0]["�����"], dt.Rows[0]["������� �������"], dt.Rows[0]["��������� �� ���� ��� "], dt.Rows[0]["��������� �� ������� ��� "], date.AddMonths(1).AddYears(-1), GetOutcomesGrownReasons(controlId));
            }
            else if (attention == "���������")
            {
                hintText = String.Format("<b>���� ����� ���� ��������</b><br/><b>{0}</b>. ���� ���������� ������������������ ������� �������� �� ��&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;�. � ������������ �������&nbsp;<b>{2:yyyy}</b>&nbsp;����&nbsp;<b>����</b>&nbsp;�������� ����� ���������� �� ��.<br/><br/>������� ���� ���������� �� �� ��&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;�.:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{3:P0}</b><br/>���� ���������� ��&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;�.:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{4:P0}</b><br/><b>����</b>&nbsp;�������� ����� ���������� �� ��.<br/><br/><b>��������� �������:</b>&nbsp;{10}<br/><br/><b>���������:</b><br/>�������������������� ���������� ����� ���������� �������� �� �� �������� ����� ���������� �� �� �� ��������� ��&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;�.&nbsp;<b>{5:P0}</b><br/>����� ���������&nbsp;<b>{6:P0}</b><br/>��������� ��&nbsp;<b>{1:dd.MM.yyyy}</b>&nbsp;�.&nbsp;<b>{7:N0}</b>&nbsp;���.���.<br/>��������� ��&nbsp;<b>{9:dd.MM.yyyy}</b>&nbsp;�.&nbsp;<b>{8:N0}</b>&nbsp;���.���.",
                    Core.CustomParam.CustomParamFactory("control").Value, date.AddMonths(1), date.AddYears(-1), dt.Rows[0]["������� ���� �� ��"], dt.Rows[0]["���� � �������� ����"], dt.Rows[0]["�����"], dt.Rows[0]["������ �������"], dt.Rows[0]["��������� �� ���� ��� "], dt.Rows[0]["��������� �� ������� ��� "], date.AddMonths(1).AddYears(-1), GetOutcomesFallenReasons(controlId));
            }

            return hintText;
        }

        private string GetOutcomesGrownReasons(string controlId)
        {
            string reason = String.Empty;

            switch (controlId)
            {
                case "13":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>��������� ���������� �����?</li><li>���������� ����� ������ �����?</li><li>���� ������������ ������� ������ �����?</li><li>���������� ����������� ��������������� � ������������� ��������?</li><li>��������� � ��������� ����������������?</li></ul>";
                        break;
                    }
                case "14":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>���������� ����� ����������?</li><li>���������� ����������� ��������� ��������� �������, ���������� ���������� �����������?</li><li>������������� ���������� ������������ �������� �� ����������� ����������� ���������?</li><li>��������� � ��������� ����������������?</li></ul>";
                        break;
                    }
            }

            return reason;
        }

        private string GetOutcomesFallenReasons(string controlId)
        {
            string reason = String.Empty;

            switch (controlId)
            {
                case "13":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>����������� ��������� ����?</li><li>������� ��������� ���������� �� �������������� (���������� 83-��)?</li><li>��������� � ��������� ����������������?</li></ul>";
                        break;
                    }
                case "14":
                    {
                        reason = "<ul style='margin-bottom: -35px; margin-top: 0px'><li>���������� ����� ����������?</li><li>���������� ����������� ��������� ��������� �������, ���������� ���������� �����������?</li><li>��������� � ��������� ����������������?</li></ul>";
                        break;
                    }
            }

            return reason;
        }


        private string GetIndicatorText()
        {
            DataTable dt = new DataTable();

            string controlID = HttpContext.Current.Session["Current�ontrolID"].ToString();

            if (controlID == "13" ||
                controlID == "14")
            {
                string query = DataProvider.GetQueryText("fo_0003_0001_indicator_outcomes");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "���������� ", dt);

                return GetOutcomesHintText(dt.Rows[0]["��������"].ToString(), dt, controlID);
            }
            else if (controlID == "15")
            {
                string query = DataProvider.GetQueryText("fo_0003_0001_241");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "���������� ", dt);

                switch (dt.Rows[0]["�������� "].ToString())
                {
                    case "0":
                        { 
                            return String.Empty;
                        }
                    case "1":
                        {
                            return String.Format("<b>������������� ���� �������� �� ���������� ��������� ���������� � ���� ������������� ������������ ��������� �����������</b><br/><b>����</b>&nbsp;�������� ������������������ ������� �������� ��&nbsp;<b>�� ���������� ��������� ���������� (����� 211, 213, 224, 225)</b>&nbsp;�&nbsp;<b>�� ������������� ������������ ��������� ����������� (����� 241)</b>&nbsp;�� ��������� ��&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;�.<br/><br/>���� ���������� �� �������� �� ���������� ��������� ���������� ��&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;�. � ������������ �������&nbsp;<b>{1:yyyy}</b>&nbsp;����:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{2:P0}</b><br/>���� ���������� �� �������� �� ������������� ������������ ��������� ����������� ��&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;�. � ������������ �������&nbsp;<b>{1:yyyy}</b>&nbsp;����:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{3:P0}</b><br/><br/><b>��������� �������:</b><ul style='margin-bottom: -35px; margin-top: 0px'><li>���������� ���������� ��������� ����������?</li><li>������������ 83-��?</li><li>��������� � ��������� ����������������?</li></ul>",
                                    date.AddMonths(1), date.AddYears(-1), dt.Rows[0]["���� � �������� ���� "], dt.Rows[0]["���� � �������� ���� �� �������������"]);
                        }
                    case "2":
                        {
                            return String.Format("<b>����������� ���� ������������� ������������ ��������� �����������</b><br/><b>����������� ����</b>&nbsp;�������� ������������������ ������� �������� ��&nbsp;<b>�� ������������� ������������ ��������� ����������� (����� 241)</b>&nbsp;�� ��������� ��&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;�.<br/><br/>���� ���������� �� �������� �� ���������� ��������� ���������� ��&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;�. � ������������ �������&nbsp;<b>{1:yyyy}</b>&nbsp;����:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{2:P0}</b><br/>���� ���������� �� �������� �� ������������� ������������ ��������� ����������� ��&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;�. � ������������ �������&nbsp;<b>{1:yyyy}</b>&nbsp;����:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{3:P0}</b><br/><br/><b>��������� �������:</b><ul style='margin-bottom: -35px; margin-top: 0px'><li>�������� ��������� ����������� �������������, � ������� �� ������ �� �����������?</li><li>������������ 83-��?</li><li>��������� � ��������� ����������������?</li></ul>",
                                    date.AddMonths(1), date.AddYears(-1), dt.Rows[0]["���� � �������� ���� "], dt.Rows[0]["���� � �������� ���� �� �������������"]);
                        }
                    case "3":
                        {
                            return String.Format("<b>������������� �������� �������� �� ���������� ��������� ���������� � ���� ������������� ������������ ��������� �����������</b><br/><b>��������</b>&nbsp;�������� ������������������ ������� �������� ��&nbsp;<b>�� ���������� ��������� ���������� (����� 211, 213, 224, 225)</b>&nbsp;�&nbsp;<b>�� ������������� ������������ ��������� ����������� (����� 241)</b>&nbsp;�� ��������� ��&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;�.<br/><br/>���� ���������� �� �������� �� ���������� ��������� ���������� ��&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;�. � ������������ �������&nbsp;<b>{1:yyyy}</b>&nbsp;����:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{2:P0}</b><br/>���� ���������� �� �������� �� ������������� ������������ ��������� ����������� ��&nbsp;<b>{0:dd.MM.yyyy}</b>&nbsp;�. � ������������ �������&nbsp;<b>{1:yyyy}</b>&nbsp;����:<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>{3:P0}</b><br/><br/><b>��������� �������:</b><ul style='margin-bottom: -35px; margin-top: 0px'><li>������������ 83-��?</li><li>��������� � ��������� ����������������?</li></ul>",
                                   date.AddMonths(1), date.AddYears(-1), dt.Rows[0]["���� � �������� ���� "], dt.Rows[0]["���� � �������� ���� �� �������������"]);
                        }
                }
            }
            else
            {
                string query = DataProvider.GetQueryText("fo_0003_0001_indicator");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "���������� ", dt);

                return GetIncomesHintText(dt.Rows[0]["��������"].ToString(), dt, controlID);
            }
            return String.Empty;
        }
    }
}

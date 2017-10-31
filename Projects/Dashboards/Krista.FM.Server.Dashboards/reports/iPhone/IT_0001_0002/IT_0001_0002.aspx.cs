using System;
using System.Data;
using System.Drawing;
using System.Web;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class IT_0001_0002 : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (HttpContext.Current.Session["CurrentITID"] == null)
            {
                HttpContext.Current.Session["CurrentITID"] = 1;
            }

            DataTable dtMarketSegments = new DataTable();

            switch(HttpContext.Current.Session["CurrentITID"].ToString())
            {
                case "1":
                    {
                        lbMarketSegmentsDescription.Text = GetHardvareDetalization();
                        dtMarketSegments = GetHardvareDtMarketSegments();
                        break;
                    }
                case "2":
                    {
                        lbMarketSegmentsDescription.Text = GetSoftwareDetalization();
                        dtMarketSegments = GetSoftwareDtMarketSegments();
                        break;
                    }
                case "3":
                    {
                        lbMarketSegmentsDescription.Text = GetServeDetalization();
                        dtMarketSegments = GetServeDtMarketSegments();
                        break;
                    }
            }
                       
            UltraChartMarketSegments.Width = 480;
            SetUpPieChart(UltraChartMarketSegments);
            UltraChartMarketSegments.ChartType = ChartType.PieChart;
            UltraChartMarketSegments.DataSource = dtMarketSegments;
            UltraChartMarketSegments.DataBind();

            HyperLink1.NavigateUrl = "http://www.idcrussia.ru";
            HyperLink1.Text = "IDC";
            HyperLink1.ForeColor = Color.Black;
        }
              

        private string GetHardvareDetalization()
        {
            string detalization =
                String.Format(
                    "����� �������� ����������� ����������� �&nbsp;<span class=\"DigitsValueLargePopup\">2009</span>&nbsp;���� ��������&nbsp;<span class=\"DigitsValueLargePopup\">255,2</span>&nbsp;����.���. (<span class=\"DigitsValueLargePopup\">20%</span>&nbsp;�� ������ ������ ����������� ��-�����), � ��� �����:");
            return detalization;
        }

        private string GetSoftwareDetalization()
        {
            string detalization =
                String.Format(
                    "����� �������� ������������ ����������� �&nbsp;<span class=\"DigitsValueLargePopup\">2009</span>&nbsp;���� ��������&nbsp;<span class=\"DigitsValueLargePopup\">99,3</span>&nbsp;����.���. (<span class=\"DigitsValueLargePopup\">51,4%</span>&nbsp;�� ������ ������ ����������� ��-�����), � ��� �����:");
            return detalization;
        }

        private string GetServeDetalization()
        {
            string detalization =
                String.Format(
                    "����� �������� ��-����� �&nbsp;<span class=\"DigitsValueLargePopup\">2009</span>&nbsp;���� ��������&nbsp;<span class=\"DigitsValueLargePopup\">142</span>&nbsp;����.���. (<span class=\"DigitsValueLargePopup\">28,6%</span>&nbsp;�� ������ ������ ����������� ��-�����), � ��� �����:");
            return detalization;
        }

        private void SetUpPieChart(UltraChart chart)
        {
            chart.Border.Thickness = 0;
            chart.BackColor = Color.FromArgb(168, 168, 168);
            chart.Axis.Y.Extent = 5;
            chart.Axis.X.Extent = 5;
            chart.Tooltips.FormatString = "";
            chart.Legend.Visible = false;
            chart.Legend.Location = LegendLocation.Left;
            chart.Legend.SpanPercentage = 40;
            chart.Axis.X.Labels.SeriesLabels.Visible = false;
            chart.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            chart.Axis.X.Labels.Font = new Font("Verdana", 12);
            chart.Axis.X.Labels.FontColor = Color.White;
            chart.Data.ZeroAligned = true;
            chart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            chart.PieChart.Labels.FormatString = "<ITEM_LABEL>\n<PERCENT_VALUE:N0>%";
            chart.PieChart.Labels.Font = new Font("Arial", 14);

            chart.PieChart.Labels.FontColor = Color.Black;
            chart.PieChart.Labels.LeaderLineColor = Color.Black;
            chart.PieChart.Labels.LeaderDrawStyle = LineDrawStyle.Solid;
            chart.PieChart.Labels.LeaderEndStyle = LineCapStyle.DiamondAnchor;
            chart.PieChart.RadiusFactor = 70;

            //chart.PieChart.StartAngle = 85;

            chart.ColorModel.ModelStyle = ColorModels.CustomLinear;

            //PaintElementCollection pes = new PaintElementCollection();
            //foreach (PaintElement pe in chart.ColorModel.Skin.PEs)
            //{
            //    pes.Add(pe);
            //}
            //chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            //chart.ColorModel.Skin.PEs.Clear();
            //foreach (PaintElement pe in pes)
            //{
            //    chart.ColorModel.Skin.PEs.Add(pe);
            //}
        }


        private static DataTable GetHardvareDtMarketSegments()
        {
            DataTable dtMarketSegments = new DataTable();
            dtMarketSegments.Columns.Add(new DataColumn("�������", typeof(string)));
            //dtMarketSegments.Columns.Add(new DataColumn("%", typeof(double)));
            dtMarketSegments.Columns.Add(new DataColumn("%", typeof(double)));

            DataRow row = dtMarketSegments.NewRow();
            row[0] = "���������\n������������";
            row[1] = 26;
            dtMarketSegments.Rows.Add(row);

            row = dtMarketSegments.NewRow();
            row[0] = "������������\n������������";
            row[1] = 36;
            dtMarketSegments.Rows.Add(row);

            row = dtMarketSegments.NewRow();
            row[0] = "������������\n������������";
            row[1] = 9;
            dtMarketSegments.Rows.Add(row);

            row = dtMarketSegments.NewRow();
            row[0] = "�������������";
            row[1] = 29;
            dtMarketSegments.Rows.Add(row);

            return dtMarketSegments;
        }

        private static DataTable GetSoftwareDtMarketSegments()
        {
            DataTable dtMarketSegments = new DataTable();
            dtMarketSegments.Columns.Add(new DataColumn("�������", typeof(string)));
            //dtMarketSegments.Columns.Add(new DataColumn("%", typeof(double)));
            dtMarketSegments.Columns.Add(new DataColumn("%", typeof(double)));

            DataRow row = dtMarketSegments.NewRow();
            row[0] = "�� ������\n����������";
            row[1] = 40;
            dtMarketSegments.Rows.Add(row);

            row = dtMarketSegments.NewRow();
            row[0] = "�� ��������\n����������";
            row[1] = 35;
            dtMarketSegments.Rows.Add(row);

            row = dtMarketSegments.NewRow();
            row[0] = "�������� ��";
            row[1] = 25;
            dtMarketSegments.Rows.Add(row);

            return dtMarketSegments;
        }

        private static DataTable GetServeDtMarketSegments()
        {
            DataTable dtMarketSegments = new DataTable();
            dtMarketSegments.Columns.Add(new DataColumn("�������", typeof(string)));
            //dtMarketSegments.Columns.Add(new DataColumn("%", typeof(double)));
            dtMarketSegments.Columns.Add(new DataColumn("%", typeof(double)));

            DataRow row = dtMarketSegments.NewRow();
            row[0] = "���������\n����������";
            row[1] = 32;
            dtMarketSegments.Rows.Add(row);

            row = dtMarketSegments.NewRow();
            row[0] = "����������\n��������� ��";
            row[1] = 29;
            dtMarketSegments.Rows.Add(row);

            row = dtMarketSegments.NewRow();
            row[0] = "��-����������";
            row[1] = 12;
            dtMarketSegments.Rows.Add(row);

            row = dtMarketSegments.NewRow();
            row[0] = "��-���������";
            row[1] = 17;
            dtMarketSegments.Rows.Add(row);

            row = dtMarketSegments.NewRow();
            row[0] = "��-����������";
            row[1] = 6;
            dtMarketSegments.Rows.Add(row);

            row = dtMarketSegments.NewRow();
            row[0] = "��-��������";
            row[1] = 4;
            dtMarketSegments.Rows.Add(row);

            return dtMarketSegments;
        }

        
    }
}

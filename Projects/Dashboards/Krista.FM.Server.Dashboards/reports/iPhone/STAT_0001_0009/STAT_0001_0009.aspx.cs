using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;

using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.WebUI.UltraWebGrid;
using System.Web.UI.WebControls;
using System.Web;
using System.IO;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class STAT_0001_0009 : CustomReportPage
    {

        private CustomParam nsoDateParam;
        private CustomParam rfDateParam;
        private CustomParam nsoOutWorkDateParam;
        private CustomParam rfOutWorkDateParam;
        private CustomParam year;

        private DateTime date;
        private DateTime nsoOutWorkDate;
        private DateTime rfOutWorkDate;

        // �������� �����������
        private string[] rows = {
                            "������ ������������� ������������",
                            "� ��� �����:<br/>������ �������� ����������",
                            "�������������� ������������",
                            "������������ � ������������� ��������������, ���� � ����",
                            "������ ��������� ��������� ���������",
                            "����� �����, ����������� �� ���� ������������ ��������������",
                            "���� � �������� ����� �����",
                            "���������� ���������� ������ �����������",
                            "��������������� ���������� ������ �����������",
                            "������ ��������� ��������",
                            "����� ������� ����� ���������",
                            "�������� ����������� ���������� �����",
                            "�������� ������������� �������� ������ ���������",
                            "������ ��������������� ��� (� ������� ����������� ����)",
                            "������ ��� �������������� ������������ ������� (� ������� ����������� ����)",
                            "������� ���������� ������������������ ����������� (�� {0:dd.MM.yyyy})"
                        };

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            string filename = HttpContext.Current.Server.MapPath("~/TemporaryImages/STAT_0001_0009/") + "TouchElementBounds.xml";
            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/TemporaryImages/STAT_0001_0009/"));
            File.WriteAllText(filename, String.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?><touchElements><element id=\"STAT_0001_0009\" bounds=\"x=0;y=0;width=0;height=0\" openMode=\"incomes\"/></touchElements>"));

            if (!InitDates())
                throw new Exception("��� ������ ��� ���������� ������");

            #region ����

            UltraWebGrid1.Width = Unit.Parse("755px");
            UltraWebGrid1.DataBinding += new EventHandler(UltraWebGrid1_DataBinding);
            UltraWebGrid1.InitializeLayout += new Infragistics.WebUI.UltraWebGrid.InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid1_InitializeRow);
            UltraWebGrid1.DataBind();

            #endregion

            #region ���������

            UltraChart1.Width = Unit.Parse("755px");
            UltraChart1.Height = Unit.Parse("600px");
            UltraChart1.ChartType = ChartType.DoughnutChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.Y.Extent = 0;
            UltraChart1.Axis.X.Extent = 0;
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N2> ����. ���. (<PERCENT_VALUE:N2>%)";
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.FormatString = "<ITEM_LABEL>";
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 40;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart1.Data.ZeroAligned = true;
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.DoughnutChart.Labels.FormatString = "<PERCENT_VALUE:N2>%";
            UltraChart1.DoughnutChart.Labels.FontColor = Color.FromArgb(192, 192, 192);
            UltraChart1.DoughnutChart.Labels.LeaderLineColor = Color.FromArgb(192, 192, 192);
            UltraChart1.DoughnutChart.Labels.LeaderDrawStyle = LineDrawStyle.Solid;
            UltraChart1.DoughnutChart.Labels.LeaderEndStyle = LineCapStyle.Square;
            UltraChart1.DoughnutChart.Labels.Font = new Font("Verdana", 10);
            UltraChart1.DoughnutChart.OthersCategoryPercent = 0;
            UltraChart1.DoughnutChart.RadiusFactor = 110;

            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
            UltraChart1.DataBind();

            #endregion

        }

        #region ����

        private bool InitDates()
        {
            string query = DataProvider.GetQueryText("STAT_0001_0009_date");
            DataTable dtDate = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����", dtDate);

            if (dtDate.Rows.Count == 0)
                return false;

            nsoDateParam = UserParams.CustomParam("nso_date");
            rfDateParam = UserParams.CustomParam("rf_date");

            nsoDateParam.Value = dtDate.Rows[0]["����"].ToString();
            rfDateParam.Value = dtDate.Rows[0]["����"].ToString();

            date = CRHelper.DateByPeriodMemberUName(nsoDateParam.Value, 3);

            query = DataProvider.GetQueryText("STAT_0001_0009_out_work_date_nso");
            dtDate = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����", dtDate);

            nsoOutWorkDateParam = UserParams.CustomParam("nso_out_work_date");

            nsoOutWorkDateParam.Value = dtDate.Rows[0]["����"].ToString();

            nsoOutWorkDate = CRHelper.DateByPeriodMemberUName(nsoOutWorkDateParam.Value, 3);

            rfOutWorkDate = nsoOutWorkDate.AddMonths(-1);

            rfOutWorkDateParam = UserParams.CustomParam("rf_out_work_date");
            rfOutWorkDateParam.Value = CRHelper.PeriodMemberUName("[������__��� ������� �����].[������__��� ������� �����].[������ ���� ��������]", rfOutWorkDate, 4);

            query = DataProvider.GetQueryText("STAT_0001_0009_chart_date");
            dtDate = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����", dtDate);

            year = UserParams.CustomParam("year");

            year.Value = dtDate.Rows.Count == 0 ? "1998" : dtDate.Rows[0]["���_�����"].ToString();

            ChartHeader.Text = String.Format("��������� �������� ������������� �������� ������������� ������� � {0} ����", year.Value);

            return true;

        }

        #endregion

        #region ����

        private void UltraWebGrid1_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            if (e.Row.Index > 0 && e.Row.Index < 4)
                e.Row.Cells[0].Style.Padding.Left = Unit.Parse("50px");
        }

        private void UltraWebGrid1_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.NullTextDefault = "-";

            UltraGridBand band = e.Layout.Bands[0];

            band.Columns[0].Width = 500;
            band.Columns[1].Width = 125;
            band.Columns[2].Width = 125;

            string period = String.Empty;
            if (date.Month == 1)
            {
                period = String.Format("������ {0} ����", date.Year);
            }
            else
            {
                period = String.Format("������-{0} {1:yyyy} ����", CRHelper.RusMonth(date.Month), date);
            }

            GridHeaderLayout headerLayout = new GridHeaderLayout(e.Layout.Grid);
            headerLayout.AddCell("����������");
            GridHeaderCell headerCell = headerLayout.AddCell(String.Format("{0} � % � ������������ ������� ����������� ����", period));
            headerCell.AddCell("������������� �������");
            headerCell.AddCell("���������� ���������");

            headerLayout.ApplyHeaderInfo();

            band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            CRHelper.FormatNumberColumn(band.Columns[1], "N1");
            CRHelper.FormatNumberColumn(band.Columns[2], "N1");
        }

        private void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0009_grid_nso");
            DataTable dtGridNSO = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����", dtGridNSO);
            query = DataProvider.GetQueryText("STAT_0001_0009_grid_nso_out_work");
            DataTable dtGridNSOOutWork = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "��������", dtGridNSOOutWork);
            query = DataProvider.GetQueryText("STAT_0001_0009_grid_rf");
            DataTable dtGridRF = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "����", dtGridRF);
            query = DataProvider.GetQueryText("STAT_0001_0009_grid_rf_out_work");
            DataTable dtGridRFOutWork = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "��������", dtGridRFOutWork);

            DataTable dtGrid = new DataTable();
            dtGrid.Columns.Add("����������", typeof(string));
            dtGrid.Columns.Add("������������� �������", typeof(double));
            dtGrid.Columns.Add("���������� ���������", typeof(double));

            foreach (string param in rows)
            {
                DataRow row = dtGrid.NewRow();
                row["����������"] = String.Format(param, nsoOutWorkDate);
                dtGrid.Rows.Add(row);
            }

            for (int i = 0; i < dtGridNSO.Rows.Count; ++i)
            {
                dtGrid.Rows[i]["������������� �������"] = dtGridNSO.Rows[i]["��������"];
            }
            if (dtGridNSOOutWork.Rows.Count > 0)
                dtGrid.Rows[dtGrid.Rows.Count - 1]["������������� �������"] = dtGridNSOOutWork.Rows[0]["��������"];

            for (int i = 0; i < dtGridRF.Rows.Count; ++i)
            {
                dtGrid.Rows[i]["���������� ���������"] = dtGridRF.Rows[i]["��������"];
            }
            if (dtGridRFOutWork.Rows.Count > 0)
                dtGrid.Rows[dtGrid.Rows.Count - 1]["���������� ���������"] = dtGridRFOutWork.Rows[0]["��������"];

            UltraWebGrid1.DataSource = dtGrid;

        }

        #endregion

        #region ���������

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            
            DataTable dtChart = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0009_chart");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "�����", dtChart);

            CRHelper.FillCustomColorModel(UltraChart1, dtChart.Rows.Count, true);

            UltraChart1.DataSource = dtChart;
        }

        #endregion

    }
}

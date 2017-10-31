using System;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0016_0002
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtGauges;
        private DataTable dtDate;

        private int firstYear = 2000;
        private int endYear = 2011;
        private const int gaugeWidth = 450;
        private int gaugeRowCount = 2;

        #region ��������� �������

        // ��������� ������
        private CustomParam selectedRegion;

        #endregion


        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            double pageWidth = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth).Value;

            #region ������������� ���������� �������

            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }

            #endregion

            // ����� ������� � ������
            gaugeRowCount = Convert.ToInt32(pageWidth) / gaugeWidth;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0016_0002_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string baseQuarter = dtDate.Rows[0][2].ToString();

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodQuater.Value = baseQuarter;

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.Set�heckedState(endYear.ToString(), true);

                ComboQuarter.Title = "�������";
                ComboQuarter.Width = 170;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillQuaters(false));
                ComboQuarter.Set�heckedState(baseQuarter, true);

                ComboRegion.Visible = true;
                ComboRegion.Title = "����������";
                ComboRegion.Width = 280;
                ComboRegion.MultiSelect = false;
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillSettlements(RegionsNamingHelper.LocalSettlementTypes, false));
                ComboRegion.Set�heckedState("��������� �����", true);
            }

            Page.Title = "���������� ����������� �� � �� � ������� �����������";
            Label1.Text = string.Format("{0} ({1})", Page.Title, ComboRegion.SelectedValue);

            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            int quarterNum = ComboQuarter.SelectedIndex + 1;

            Label2.Text = string.Format("�� {0} ������� {1} ����", quarterNum, yearNum);

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByQuarterNum(quarterNum));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", quarterNum);
            selectedRegion.Value = RegionsNamingHelper.LocalSettlementUniqueNames[ComboRegion.SelectedValue];

            GaugesDataBind();
        }

        /// <summary>
        /// ��������� ��������� �������� � ���� ������ �� �������
        /// </summary>
        /// <param name="value">��������</param>
        /// <returns>�������� � ���� ������, ���� ������ ������</returns>
        private static string GetNonDBNullValue(object value)
        {
            return (value != DBNull.Value) ? value.ToString() : string.Empty;
        }

        private void GaugesDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0016_0002_indiacators");
            dtGauges = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "���������", dtGauges);

            GaugesTD.Controls.Clear();
            // ������� ������� ��� �������
            Table table = new Table();
            TableRow currentRow = new TableRow();

            for (int i = 0; i < dtGauges.Rows.Count; i++)
            {
                if (i % gaugeRowCount == 0)
                {
                    // ��������� �� ����� ������
                    currentRow = new TableRow();
                    table.Rows.Add(currentRow);
                }

                // ������� ������ � ������� ������
                TableCell cell = new TableCell();
                currentRow.Cells.Add(cell);

                DataRow row = dtGauges.Rows[i];
                Control indControl = Page.LoadControl("~/components/GaugeIndicator.ascx");
                GaugeIndicator gauge = (GaugeIndicator)indControl;
                gauge.GaugeImageURL = string.Format("../../TemporaryImages/gaugeIndicator_{0}_#SEQNUM(500).png", i);
                gauge.Width = gaugeWidth.ToString();
                gauge.Height = "300";

                gauge.FactValue = GetNonDBNullValue(row["��������"]);

                int rank = row["���� �� ��������"] != DBNull.Value ? Convert.ToInt32(row["���� �� ��������"]) : 0;
                int badRank = row["������ ���� �� ��������"] != DBNull.Value ? Convert.ToInt32(row["������ ���� �� ��������"]) : -1;
                string imgRank = string.Empty;
                if (rank == 1)
                {
                    imgRank = "<img src='../../images/starYellowBB.png'>";
                }
                else if (rank == badRank)
                {
                    imgRank = "<img src='../../images/starGrayBB.png'>";
                }

                gauge.RankValue = string.Format("{0}&nbsp;{1}", (rank == 0) ? string.Empty : rank.ToString(), imgRank);
                gauge.MinValue = GetNonDBNullValue(row["����������� �������� �� �������"]);
                gauge.MaxValue = GetNonDBNullValue(row["������������ �������� �� �������"]);

                gauge.Title = GetNonDBNullValue(row["���������"]);
                gauge.Name = GetNonDBNullValue(row["������������"]);
                gauge.Content = GetNonDBNullValue(row["����������"]);
                gauge.Formula = GetNonDBNullValue(row["�������"]);
                gauge.NormalValue = string.Format("{0}{1}", GetNonDBNullValue(row["������� ��������"]), GetNonDBNullValue(row["��������� ��������"]));

                if (row["��������"] != DBNull.Value)
                {
                    double indValue = Convert.ToDouble(row["��������"]);
                    double limitValue = row["��������� ��������"] != DBNull.Value
                                            ? Convert.ToInt32(row["��������� ��������"])
                                            : 0;

                    bool isFailure = row["���������� ���������"] != DBNull.Value ? (Convert.ToInt32(row["���������� ���������"]) == 1) : false;
                    gauge.BEStartColor = isFailure ? Color.Red : Color.LimeGreen;
                    gauge.BEEndColor = isFailure ? Color.Maroon : Color.DarkGreen;
                    gauge.ToolTip = indValue.ToString();

                    gauge.LimitValue = limitValue;
                    gauge.MarkerValue = indValue;
                    double minIndValue = row["����������� ��������"] != DBNull.Value ? Convert.ToDouble(row["����������� ��������"]) : 0;
                    double maxIndValue = row["������������ ��������"] != DBNull.Value ? Convert.ToDouble(row["������������ ��������"]) : 0;
                    gauge.AxisMin = Math.Min(Math.Min(minIndValue, indValue - 0.1), limitValue);
                    gauge.AxisMax = Math.Max(Math.Max(maxIndValue, indValue + 0.1), limitValue);
                    gauge.AxisTickInterval = (gauge.AxisMax - gauge.AxisMin) / 3;
                }
                else
                {
                    gauge.SetVisibleScales(false);
                    gauge.BEStartColor = Color.Transparent;
                    gauge.BEEndColor = Color.Transparent;
                    gauge.ToolTip = string.Empty;
                }

                cell.Controls.Add(gauge);
            }
            GaugesTD.Controls.Add(table);
        }
    }
}
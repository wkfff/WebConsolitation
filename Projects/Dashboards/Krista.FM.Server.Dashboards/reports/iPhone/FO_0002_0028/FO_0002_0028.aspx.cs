using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0002_0028 : CustomReportPage
    {
        private DataTable dtGrid = new DataTable();
        private DataTable populationDT;
        private DateTime lastDate;

        // ��������� ������
        private CustomParam lastPeriod;
        // ��������� ��������� ������
        private CustomParam rubMultiplier;
        //��� ������� ��� ������� �����������
        private CustomParam populationRegionName;

        protected override void Page_Load(object sender, EventArgs e)
        {
            #region ������������� ���������� �������

            lastPeriod = UserParams.CustomParam("last_period");
            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            populationRegionName = UserParams.CustomParam("population_region_name");

            #endregion

            lastDate = CubeInfoHelper.MonthReportIncomesInfo.LastDate;
            lastPeriod.Value = CRHelper.PeriodMemberUName("[������].[������]", lastDate, 4);
            UserParams.PeriodYear.Value = lastDate.Year.ToString();

            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            UserParams.IncomesKD30000000000000000.Value = RegionSettingsHelper.Instance.IncomesKD30000000000000000;
            rubMultiplier.Value = "1000";
            populationRegionName.Value = "[����������].[������������].[��� ����������].[����������  ���������].[��������� ����������� �����].[������ �������]";

            OutcomesGrid.Width = Unit.Empty;
            OutcomesGrid.Height = Unit.Empty;
            OutcomesGrid.DisplayLayout.NoDataMessage = "��� ������";

            lbPageTitle.Text = String.Format("�������� ���������� ���������� {1} ������ ������� �� ��������� ��&nbsp;<span style='color:white;font-weight:bold'>{0:dd.MM.yyyy}</span>&nbsp;�., ���.���.",
                lastDate.AddMonths(1), CustomParam.CustomParamFactory("budget_level").Value == "����.������ ��������" ? "������������������ �������" : "������������ �������");

            OutcomesGrid.Bands.Clear();
            OutcomesGrid.DataBind();
        }

        #region ����������� �����

        protected void OutcomesGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0028_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "����������", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                query = DataProvider.GetQueryText("FO_0002_0028_grid_population");
                populationDT = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "������������ �����������", populationDT);

                dtGrid = MergeDataTables(dtGrid, populationDT);

                OutcomesGrid.DataSource = dtGrid;
            }
        }

        private static DataTable MergeDataTables(DataTable dt1, DataTable dt2)
        {
            DataTable newDT = dt1.Copy();
            foreach (DataRow row in dt2.Rows)
            {
                DataRow newRow = newDT.NewRow();
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    newRow[i] = row[i];
                }
                newDT.Rows.Add(newRow);
            }
            newDT.AcceptChanges();

            return newDT;
        }

        protected void OutcomesGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;
            
            e.Layout.Bands[0].Columns[0].Width = 210;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = 130;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");

            e.Layout.Bands[0].Columns[2].Width = 140;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");

            e.Layout.Bands[0].Columns[3].Width = 140;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P1");

            e.Layout.Bands[0].Columns[4].Width = 140;
            e.Layout.Bands[0].Columns[4].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P1");

            e.Layout.Bands[0].Columns[5].Hidden = true;

            GridHeaderLayout headerLayout = new GridHeaderLayout(e.Layout.Grid);
            headerLayout.AddCell("������������ ����������");
            headerLayout.AddCell(String.Format("���������� ���� �� {0} ���", lastDate.Year));
            headerLayout.AddCell(String.Format("���������� � ������ ���� �� {0:dd.MM.yyy} �.", lastDate.AddMonths(1)));
            headerLayout.AddCell(String.Format("���� ����� ����������� ����� �� {0} �. � ���������� {1} �.", lastDate.Year, lastDate.Year - 1));
            headerLayout.AddCell(String.Format("���� ����� ���������� � ������ ���� �� {0:dd.MM.yyy} �.", lastDate.AddMonths(1)));
            headerLayout.ApplyHeaderInfo();
        }

        private static bool IsInvertIndication(string indicatorName)
        {
            switch (indicatorName)
            {
                case "������ ����� (����� 221)":
                case "������������ ������ (����� 222)":
                case "�������� ����� �� ����������� ���������� (����� 224)":
                case "2.3 ������� �� ������ �����":
                case "������, ������ �� ���������� ��������� (����� 225)":
                case "������ ������ � ������ (����� 226)":
                case "������������� ������������ ��������������� � ������������� ������������ (����� 241)":
                case
                    "������������� ������������ ������������, �� ����������� ��������������� � ������������� ����������� (����� 242)"
                    :
                case "������ ������� (����� 290)":
                case "3 �������":
                case "3.2 ������ ������� (�� ����. ����� 1, 2 � 3.1)":
                case "����� ��������":
                case "����� �������� ��� ����� ������� �� ���� ������������� �����������":
                case "������� �� ����������� ������ 3":
                case
                    "���������� �������� (�� ����������� ������ 3) ��� �������� (��� ����� ������������� ������������ �� ����������� ��������)"
                    :
                case "3.2 ������� �� ������������ �������":
                case "3.3 ������� ������������ �����":
                case "����������� ��������":
                case "3.4 ���� ���������":
                case "����� ���������� ��������������":
                case
                    "���������� �������� (�� ����������� ������ 3) ��� �������� (��� ����� ������������� ������������ �� ����������� ��������) � ������ ���������� �������������� �������� �������"
                    :
                case "���������� �������� ��� �������� � ������ ���������� �������������� �������� �������":
                case "IV ������������ ������������ ������������� - �����":
                case "�� ���������� �����":
                case "�� ����������� �� ������ �����":
                case "�� ������ ������������ �����":
                case "�� ����������� ��� ���������� ��������� ��������� ��������� �������":
                case "�� ��������  �� ������������ ����������� ����������� ������������� ���������":
                case "5.1 ����� ���������������� �����":
                case "5.2 ����������� ���������, ���.���.":
                    {
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        private static bool IsGroupRow(string rowName)
        {
            rowName = rowName.TrimEnd(' ');
            return (rowName == "I. ������" || rowName == "II. �������" ||
                    rowName == "III ��������� �������������� �������� �������" || rowName == "V.���������." ||
                    rowName.Contains("�� ���"));
        }

        protected void OutcomesGrid_InitializeRow(object sender, RowEventArgs e)
        {
            string indicatorName = String.Empty;
            if (e.Row.Cells[0].Value != null)
            {
                indicatorName = e.Row.Cells[0].Value.ToString().Replace("_", String.Empty);
                e.Row.Cells[0].Value = indicatorName;
            }

            bool isInvertIndication = IsInvertIndication(indicatorName.TrimEnd(' '));

            string level = String.Empty;
            int levelColumnIndex = e.Row.Cells.Count - 1;
            if (e.Row.Cells[levelColumnIndex] != null)
            {
                level = e.Row.Cells[levelColumnIndex].ToString();
            }

            e.Row.Style.Padding.Right = 5;

            if (IsGroupRow(indicatorName))
            {
                e.Row.Cells[0].ColSpan = e.Row.Cells.Count - 1;
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                string columnCaption = e.Row.Band.Columns[i].Header.Caption.ToLower();

                bool rate = columnCaption.Contains("���� �����");

                switch (level)
                {
                    case "0":
                        {
                            e.Row.Cells[i].Style.Font.Bold = true;
                            e.Row.Cells[i].Style.Font.Size = 14;
                            break;
                        }
                    case "1":
                        {
                            e.Row.Cells[i].Style.Font.Bold = true;
                            e.Row.Cells[i].Style.Font.Italic = true;
                            e.Row.Cells[i].Style.Font.Size = 12;
                            break;
                        }
                    case "2":
                        {
                            e.Row.Cells[i].Style.Font.Bold = false;
                            e.Row.Cells[i].Style.Font.Italic = true;
                            e.Row.Cells[i].Style.Font.Size = 12;
                            break;
                        }
                }

                if (rate)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        double currentValue = Convert.ToDouble(e.Row.Cells[i].Value);

                        if (currentValue > 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = isInvertIndication
                                                                       ? "~/images/arrowRedUpBB.png"
                                                                       : "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "����������� ���� ���������� ������������ ����������� ����";
                        }
                        else if (currentValue < 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = isInvertIndication
                                                                       ? "~/images/arrowGreenDownBB.png"
                                                                       : "~/images/arrowRedDownBB.png";
                            e.Row.Cells[i].Title = "����������� �������� ���������� ������������ ����������� ����";
                        }
                    }
                    e.Row.Cells[i].Style.Padding.Left = 10;
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px;";
                }
            }
        }
    }

    #endregion
}

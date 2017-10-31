using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0002_0005_HMAO : CustomReportPage
    {
        private DataTable dt;
        private DataTable newDt;

        private double avgIncomesTotal;
        private double populationTotal;

        private double avgIncomesMinValue;
        private double avgIncomesMaxValue;
        private string avgIncomesBestItem;
        private string avgIncomesWorseItem;

        #region ��������� �������

        // ������ �����
        private CustomParam incomesTotal;
        // ������� �� � ��
        private CustomParam regionsLevel;
        // ������� ������������������ �������
        private CustomParam regionsConsolidateLevel;
        // ��� ��������� ���� ��� ������������������ ������� ��������
        private CustomParam consolidateBudgetDocumentSKIFType;
        // ��� ��������� ���� ��� �������
        private CustomParam regionBudgetDocumentSKIFType;
        // ����������� ���������
        private CustomParam populationMeasure;
        // ��� ��� ����������� ���������
        private CustomParam populationMeasureYear;
        // ������� ������� ���� ��� �������
        private CustomParam regionBudgetSKIFLevel;

        #endregion

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            #region ������������� ���������� �������

            if (incomesTotal == null)
            {
                incomesTotal = UserParams.CustomParam("incomes_total");
            }
            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }
            if (regionsConsolidateLevel == null)
            {
                regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            }
            if (consolidateBudgetDocumentSKIFType == null)
            {
                consolidateBudgetDocumentSKIFType = UserParams.CustomParam("consolidate_budget_document_skif_type");
            }
            if (regionBudgetDocumentSKIFType == null)
            {
                regionBudgetDocumentSKIFType = UserParams.CustomParam("region_budget_document_skif_type");
            }
            if (populationMeasure == null)
            {
                populationMeasure = UserParams.CustomParam("population_measure");
            }
            if (populationMeasureYear == null)
            {
                populationMeasureYear = UserParams.CustomParam("population_measure_year");
            }
            if (regionBudgetSKIFLevel == null)
            {
                regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            }

            #endregion

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0005_HMAO_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            string month = dtDate.Rows[0][3].ToString();
            int monthNum = CRHelper.MonthNum(month);

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodMonth.Value = month;
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(monthNum));

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;
            consolidateBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");
            populationMeasure.Value = RegionSettingsHelper.Instance.PopulationMeasure;
            populationMeasureYear.Value = RegionSettingsHelper.Instance.PopulationMeasurePlanning ? (yearNum + 1).ToString() : yearNum.ToString();
            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");

            string regionName = RegionSettingsHelper.Instance.ShortName.Replace("�������", "������");

            Label1.Text = string.Format("����.������ {0} �� {1} {2} {3}�",
                                        regionName,
                                        monthNum,
                                        CRHelper.RusManyMonthGenitive(monthNum),
                                        yearNum);
            
            UltraWebGrid.DataBind();
        }

        #region ����������� �����

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();

            // ������� ����� �������
            newDt = new DataTable();
            newDt.Columns.Add("column1", typeof (string));
            newDt.Columns.Add("column2", typeof (double));

            string query = DataProvider.GetQueryText("FO_0002_0005_HMAO");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            if (dt.Columns.Count > 0 && dt.Rows.Count > 0)
            {
                AVGIncomesCalculate();

                dt.Rows[0]["����������� ����������� ���������"] = populationTotal;
                dt.Rows[0]["������������� ������"] = 1000 * avgIncomesTotal;
                dt.Rows[0]["������ �� ���������.������� ��"] = avgIncomesBestItem;
                if (avgIncomesMaxValue != double.MinValue)
                {
                    dt.Rows[0]["������ ���������.������ ��"] = avgIncomesMaxValue;
                }
                dt.Rows[0]["������ �� ���������.������� ��"] = avgIncomesWorseItem;
                if (avgIncomesMinValue != double.MaxValue)
                {
                    dt.Rows[0]["������ ���������.������ ��"] = avgIncomesMinValue;
                }

                dt.Columns.RemoveAt(0);
            }

            foreach (DataRow r in dt.Rows)
            {
                for (int i = 0; i < r.ItemArray.Length; i++)
                {
                    if ((i == 3 | i == 5 || i == 9 || i == 11) && r[i] != DBNull.Value)
                    {
                        r[i] = r[i].ToString().Replace("������������� �����������", "��");
                        r[i] = r[i].ToString().Replace("������������� �����", "��");
                        r[i] = r[i].ToString().Replace("������������� �����", "��");
                        r[i] = r[i].ToString().Replace(" �����", " �-�");
                    }
                }
            }

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                DataRow newRow = newDt.NewRow();

                double k = 1;
                string caption = string.Empty;

                switch (i)
                {
                    case 0:
                        {
                            k = 100;
                            caption = "���������";
                            break;
                        }
                    case 1:
                        {
                            k = 0.000001;
                            caption = "����, ���.���.";
                            break;
                        }
                    case 2:
                        {
                            k = 0.000001;
                            caption = "����, ���.���.";
                            break;
                        }
                    case 7:
                        {
                            caption = "����������� ����������� ���������, ���.���.";
                            break;
                        }
                    case 8:
                        {
                            k = 0.001;
                            caption = "������������� ������, ���.���./���.";
                            break;
                        }
                    case 13:
                        {
                            k = 100;
                            caption = "��������";
                            break;
                        }
                    case 14:
                        {
                            k = 100;
                            caption = "�����������";
                            break;
                        }
                }

                newRow[0] = caption;
                // ����� ���, ����� ���� �������
                if (k == 1 && i != 7)
                {
                    continue;
                }
                if (dt.Rows[0][i] != DBNull.Value)
                {
                    newRow[1] = Convert.ToDouble(dt.Rows[0][i]) * k;
                }

                newDt.Rows.Add(newRow);
            }

            // ������ ��� ��� � ���� �������� ����������
            DataRow row = newDt.NewRow();
            row[0] = string.Empty;
            newDt.Rows.InsertAt(row, 1);

            // ������ ��� ��� � ���� ������������� �������
            row = newDt.NewRow();
            row[0] = string.Empty;
            newDt.Rows.InsertAt(row, 6);

            // ������ ��� ��������� � ������ �����
            row = newDt.NewRow();
            row[0] = "���� ����� ������� � �������� ����";
            newDt.Rows.InsertAt(row, 7);

            UltraWebGrid.DataSource = newDt;
        }

        private static bool IsRankingRow(string rowName)
        {
            return rowName != "����������. ������";
        }

        private void AVGIncomesCalculate()
        {
            string query = DataProvider.GetQueryText("F0_0002_0005_iphone_incomes");
            DataTable incomesDT = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", incomesDT);

            if (incomesDT.Rows.Count > 0)
            {
                query = DataProvider.GetQueryText("FO_0002_0005_HMAO_population");
                DataTable populationDT = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����������� ���������", populationDT);
                
                RankCalculator avgExecuteRank = new RankCalculator(RankDirection.Asc);

                const string populationColumnName = "����������� ����������� ���������";
                const string executeColumnName = "���� ���.���.";

                if (incomesDT.Columns.Count > 1)
                {
                    incomesDT.PrimaryKey = new DataColumn[] { incomesDT.Columns[0] };

                    foreach (DataRow populationRow in populationDT.Rows)
                    {
                        if (populationRow[0] != DBNull.Value)
                        {
                            string rowName = populationRow[0].ToString();
                            if (populationRow[populationColumnName] != DBNull.Value &&
                                populationRow[populationColumnName].ToString() != String.Empty)
                            {
                                double population = Convert.ToDouble(populationRow[populationColumnName]);

                                DataRow incomesRow = incomesDT.Rows.Find(rowName);
                                if (incomesRow != null)
                                {
                                    if (population != 0 && incomesRow[executeColumnName] != DBNull.Value &&
                                        incomesRow[executeColumnName].ToString() != String.Empty)
                                    {
                                        double execute = Convert.ToDouble(incomesRow[executeColumnName]);
                                        double avgExecute = execute/(1000*population);

                                        if (IsRankingRow(rowName))
                                        {
                                            avgExecuteRank.AddItem(rowName, avgExecute);
                                        }
                                        else
                                        {
                                            populationTotal = population;
                                            avgIncomesTotal = avgExecute;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    avgIncomesMinValue = avgExecuteRank.GetMinValue();
                    avgIncomesMaxValue = avgExecuteRank.GetMaxValue();
                    avgIncomesBestItem = avgExecuteRank.GetBestItem();
                    avgIncomesWorseItem = avgExecuteRank.GetWorseItem();
                }
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.ColHeadersVisibleDefault = ShowMarginInfo.No;

            if (e.Layout.Bands.Count == 0)
                return;

            if (e.Layout.Bands[0].Columns.Count > 1)
            {
                e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

                e.Layout.Bands[0].Columns[0].Width = 208;
                e.Layout.Bands[0].Columns[1].Width = 100;
            }

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool executePercentRank = (e.Row.Index == 1);
                bool avgIncomesRank = (e.Row.Index == 6);

                e.Row.Cells[i].Style.BorderColor = Color.FromArgb(50, 50, 50);
                e.Row.Cells[i].Style.Padding.Bottom = 2;
                e.Row.Cells[i].Style.Padding.Top = 2;
                e.Row.Cells[1].Style.HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[1].Style.Padding.Right = 2;

                if (e.Row.Index != 0)
                {
                    e.Row.Cells[i].Style.BorderDetails.ColorTop = Color.Transparent;
                }
                else
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthTop = 3;
                    e.Row.Cells[i].Style.BorderDetails.ColorTop = Color.FromArgb(50, 50, 50);
                }

                if (executePercentRank || avgIncomesRank)
                {
                    e.Row.Cells[0].Style.Padding.Left = 10;
                    e.Row.Cells[0].ColSpan = 2;

                    int columnIndex = (executePercentRank) ? 3 : 9;
                    string valueFormat = (executePercentRank) ? "P2" : "N2";

                    string bestRegion = (dt.Rows[0][columnIndex] != DBNull.Value)
                                            ? dt.Rows[0][columnIndex].ToString()
                                            : string.Empty;

                    string bestRegionValue = (dt.Rows[0][columnIndex + 1] != DBNull.Value)
                                            ? Convert.ToDouble(dt.Rows[0][columnIndex + 1]).ToString(valueFormat)
                                            : string.Empty;

                    string pourRegion = (dt.Rows[0][columnIndex + 2] != DBNull.Value)
                                            ? dt.Rows[0][columnIndex + 2].ToString()
                                            : string.Empty;

                    string pourRegionValue = (dt.Rows[0][columnIndex + 3] != DBNull.Value)
                                            ? Convert.ToDouble(dt.Rows[0][columnIndex + 3]).ToString(valueFormat)
                                            : string.Empty;

                    e.Row.Cells[0].Value = string.Format("����&nbsp;&nbsp;<span style=\"color:white;\">{0}</span>&nbsp;</span>&nbsp;<img src=\"../../../images/starYellow.png\" width=\"15px\" height=\"15px\"/>&nbsp;{1}<br />" +
                                                         "���&nbsp;&nbsp;<span style=\"color:white;\">{2}</span>&nbsp;<img src=\"../../../images/starGray.png\" width=\"15px\" height=\"15px\"/>&nbsp;{3}",
                        bestRegionValue, bestRegion, pourRegionValue, pourRegion);
                }
                else if (e.Row.Index != 7)
                {
                    e.Row.Cells[0].Style.BorderDetails.ColorRight = Color.Transparent;
                    e.Row.Cells[1].Style.BorderDetails.ColorLeft = Color.Transparent;
                }

                if (e.Row.Index == 8 || e.Row.Index == 9)
                {
                    e.Row.Cells[0].Style.Padding.Left = 50;
                }

                if (e.Row.Index == 4 || e.Row.Index == 7)
                {
                    if (e.Row.Index == 7)
                    {
                        e.Row.Cells[0].ColSpan = 2;
                    }

                    e.Row.Cells[i].Style.BorderDetails.WidthTop = 3;
                    e.Row.Cells[i].Style.BorderDetails.ColorTop = Color.FromArgb(50, 50, 50);
                }

                if (e.Row.Index == 0 || e.Row.Index == 5 || e.Row.Index == 7)
                {
                     e.Row.Cells[i].Style.BorderDetails.ColorBottom = Color.Transparent;
                }

                if (e.Row.Index == newDt.Rows.Count - 1)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthBottom = 3;
                }

                if ((e.Row.Index == 0 || e.Row.Index == 8 || e.Row.Index == 9) &&
                    i == 1 && e.Row.Cells[i].Value != null)
                {
                    e.Row.Cells[i].Value = string.Format("{0:P2}", Convert.ToDouble(e.Row.Cells[i].Value) / 100);
                }
            }
        }

        #endregion
    }
}

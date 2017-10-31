using System;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0002_0006_v : CustomReportPage
    {
        #region ��������� �������

        // ������� �� � ��
        private CustomParam regionsLevel;
        // ������� ������������������ �������
        private CustomParam regionsConsolidateLevel;
        // ��� ��������� ���� ��� ������������������ ������� ��������
        private CustomParam consolidateBudgetDocumentSKIFType;
        // ��� ��������� ���� ��� �������
        private CustomParam regionBudgetDocumentSKIFType;
        // ������� ������� ���� ��� �������
        private CustomParam regionBudgetSKIFLevel;
        // ������� �����
        private CustomParam outcomesTotal;

        #endregion

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;

            #region ������������� ���������� �������

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
            if (regionBudgetSKIFLevel == null)
            {
                regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            }
            outcomesTotal = UserParams.CustomParam("outcomes_total");

            #endregion

            #region ��������� ���������

            UltraChart.Width = 310;
            UltraChart.Height = 1200;
            UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;
            ((GradientEffect)UltraChart.Effects.Effects[0]).Coloring = GradientColoringStyle.Lighten;

            UltraChart.ChartType = ChartType.StackBarChart;
            UltraChart.StackChart.StackStyle = StackStyle.Complete;
            UltraChart.Axis.Y.Extent = 120;
            UltraChart.Axis.Y.Labels.Visible = false;
            UltraChart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Custom;
            UltraChart.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Near;
            UltraChart.Axis.Y.Labels.SeriesLabels.WrapText = true;
            UltraChart.Axis.Y.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.None;

            UltraChart.Axis.X.Visible = false;
            UltraChart.Axis.X.Extent = 30;

            UltraChart.Data.UseRowLabelsColumn = true;

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 22;

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE_ITEM:P2>";
            UltraChart.FillSceneGraph +=new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #endregion

            DataTable dtDate = new DataTable();
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0006_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            string month = dtDate.Rows[0][3].ToString();
            int monthNum = CRHelper.MonthNum(month);

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodMonth.Value = month;
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(monthNum));
            UserParams.FOFKRCulture.Value = RegionSettingsHelper.Instance.FOFKRCulture;
            UserParams.FOFKRHelthCare.Value = RegionSettingsHelper.Instance.FOFKRHelthCare;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            consolidateBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");
            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");
            outcomesTotal.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;

            string monthStr;
            switch (monthNum)
            {
                case 1:
                    {
                        monthStr = "�����";
                        break;
                    }
                case 2:
                case 3:
                case 4:
                    {
                        monthStr = "������";
                        break;
                    }
                default:
                    {
                        monthStr = "�������";
                        break;
                    }
            }

            Label3.Text = string.Format("��������� �������� {0} �� {1}&nbsp;{2}&nbsp;{3}&nbsp;����", RegionSettingsHelper.Instance.ShortName, monthNum, monthStr, yearNum);

            if (monthNum == 12)
            {
                monthNum = 1;
                yearNum++;
            }
            else
            {
                monthNum++;
            }

            Label1.Text = string.Format("������ �� 1 {0} {1} ����", CRHelper.RusMonthGenitive(monthNum), yearNum);
            Label2.Text =
                string.Format("��������� {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);


            query = DataProvider.GetQueryText("FO_0002_0006_v");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������", dt);

            foreach (DataColumn column in dt.Columns)
            {
                column.ColumnName = GetShortRzPrName(column.ColumnName);
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0] != DBNull.Value)
                {
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("������������� �����������", "��");
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("������������� �����", "��");
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("������������� �����", "��");
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("�����", "�.");
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Replace("�����", "�-�");
                }
            }

            UltraChart.DataSource = dt;
            UltraChart.DataBind();
        }

        void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.Y"))
                {
                    Text text = (Text)primitive;
                    text.bounds = new Rectangle(text.bounds.Left, text.bounds.Top, 110, 25);
                    text.labelStyle.Font = new Font("Verdana", 8);
                    text.labelStyle.HorizontalAlign = StringAlignment.Near;
                    text.labelStyle.VerticalAlign = StringAlignment.Near;
                }
            }
        }

        private static string GetShortRzPrName(string fullName)
        {
            string shortName = fullName;

            switch (fullName)
            {
                case "������������������� �������":
                    {
                        return "���������.�������";
                    }
                case "������������ �������":
                    {
                        return "������������ �������";
                    }
                case "������������ ������������ � ������������������ ������������":
                    {
                        return "���.������������ � ������������.����.";
                    }
                case "������������ ���������":
                    {
                        return "������������ ���������";
                    }
                case "�������-������������ ���������":
                    {
                        return "���";
                    }
                case "������ ���������� �����":
                    {
                        return "������ �����.�����";
                    }
                case "�����������":
                    {
                        return "�����������";
                    }
                case "��������, ��������������":
                    {
                        return "�������� � ��������������";
                    }
                case "��������, ��������������, �������� �������� ����������":
                    {
                        return "��������,  ��������������, ���";
                    }
                case "�������� �������� ����������":
                    {
                        return "���";
                    }
                case "���������������":
                    {
                        return "���������������";
                    }
                case "���������������, ���������� �������� � �����":
                    {
                        return "�����., ���.�������� � �����";
                    }
                case "���������� �������� � �����":
                    {
                        return "���������� �������� � �����";
                    }
                case "���������� ��������":
                    {
                        return "���������� ��������";
                    }
                case "������������ ����������":
                    {
                        return "������������ ����������";
                    }
                case "������������ ���������� ������ ��������� �������� ��������� ���������� ��������� � ������������� �����������":
                    {
                        return "��� �������� ���.�� � ��";
                    }
                case "������������ ���������������� � �������������� �����":
                    {
                        return "������.���.� ���.�����";
                    }
            }
            return shortName;
        }
    }
}


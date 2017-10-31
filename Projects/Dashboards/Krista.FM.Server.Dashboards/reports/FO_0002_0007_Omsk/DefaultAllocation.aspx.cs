using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0007_Omsk
{
    public partial class DefaultAllocation : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtChart = new DataTable();
        private DataTable dtAVG = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private int selectedPointIndex;
        private string populationDate;

        #region ��������� �������

        // ������� �����
        private CustomParam outcomesTotal;
        // ������ ���
        private CustomParam fkrGroupName;
        // ������� �� � ��
        private CustomParam regionsLevel;
        // ������� ������������������ �������
        private CustomParam regionsConsolidateLevel;

        // ����������� ���������
        private CustomParam populationMeasure;
        // ��� ��� ����������� ���������
        private CustomParam populationMeasureYear;

        // ��� ��������� ���� ��� ������ �������
        private CustomParam regionDocumentSKIFType;

        // ������� ������� ���� ��� �������
        private CustomParam regionBudgetSKIFLevel;

        // ����������� ��������� �� ���� ��
        private CustomParam populationTotal;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region ������������� ���������� �������

            if (outcomesTotal == null)
            {
                outcomesTotal = UserParams.CustomParam("outcomes_total");
            }
            if (fkrGroupName == null)
            {
                fkrGroupName = UserParams.CustomParam("fkr_group_name");
            }
            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }
            if (regionsConsolidateLevel == null)
            {
                regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            }

            if (populationMeasure == null)
            {
                populationMeasure = UserParams.CustomParam("population_measure");
            }
            if (populationMeasureYear == null)
            {
                populationMeasureYear = UserParams.CustomParam("population_measure_year");
            }

            if (regionDocumentSKIFType == null)
            {
                regionDocumentSKIFType = UserParams.CustomParam("region_document_skif_type");
            }

            if (regionBudgetSKIFLevel == null)
            {
                regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            }

            populationTotal = UserParams.CustomParam("population_total");

            #endregion

            #region ��������� ���������

            UltraChart.ChartType = ChartType.ScatterChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.Axis.X.Extent = 30;
            UltraChart.Axis.Y.Extent = 50;
            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.Text = "����������� ��������� ��������������, ���./���.";
            UltraChart.TitleLeft.Font = new Font("Verdana", 12);
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;

            UltraChart.TitleBottom.Visible = true;
            UltraChart.TitleBottom.Text = "����������� ���������, ���.���.";
            UltraChart.TitleBottom.Font = new Font("Verdana", 12);
            UltraChart.TitleBottom.HorizontalAlign = StringAlignment.Center;

            UltraChart.ScatterChart.Icon = SymbolIcon.Square;
            UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<DATA_VALUE_X:N3> ���.���.\n<DATA_VALUE_Y:N2> ���./���.";
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #endregion

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 200);

            CrossLink1.Text = "�������&nbsp;����������&nbsp;��������&nbsp;�&nbsp;���������&nbsp;��������������";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0007_Omsk/DefaultCompare.aspx";

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            UltraGridExporter1.ExcelExportButton.Visible = false;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
                        
            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0007_Omsk_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.Set�heckedState(endYear.ToString(), true);

                ComboMonth.Title = "�����";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.Set�heckedState(dtDate.Rows[0][3].ToString(), true);

                ComboFKR.Width = 420;
                ComboFKR.Title = "����";
                ComboFKR.MultiSelect = false;
                ComboFKR.ParentSelect = true;
                ComboFKR.FillDictionaryValues(CustomMultiComboDataHelper.FillFOFKRNames(DataDictionariesHelper.OutcomesFOFKRTypes, DataDictionariesHelper.OutcomesFOFKRLevels, false));
                ComboFKR.Set�heckedState("������������������� �������", true);
                ComboFKR.Set�heckedState("������� ������� - �����", true);
                ComboFKR.Set�heckedState("������� ������� �����", true);

                ComboRegion.Visible = true;
                ComboRegion.Title = "����������";
                ComboRegion.Width = 280;
                ComboRegion.MultiSelect = false;
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillTerritories(RegionsNamingHelper.LocalBudgetTypes, false));

                selectedPointIndex = -1;
            }


            Page.Title = string.Format("������������� ������������� ����������� �� ������������ ��������� ��������������");
            Label1.Text = Page.Title;
            Label2.Text = string.Empty;

            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            outcomesTotal.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;
            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");

            populationMeasure.Value = RegionSettingsHelper.Instance.PopulationMeasure;
            populationMeasureYear.Value = RegionSettingsHelper.Instance.PopulationMeasurePlanning ? (yearNum + 1).ToString() : yearNum.ToString();

            regionDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionDocumentSKIFType");

            if (ComboFKR.SelectedValue == "������� ������� - �����")
            {
                fkrGroupName.Value = outcomesTotal.Value;
            }
            else
            {
                fkrGroupName.Value = DataDictionariesHelper.OutcomesFOFKRTypes[ComboFKR.SelectedValue];
            }

            populationDate = DataDictionariesHelper.GetOmskRegionPopulationDate(yearNum);

            UltraChart.DataBind();
        }
        
        #region ����������� ���������

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0007_Omsk_allocation_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "�������", dtChart);

            query = DataProvider.GetQueryText("FO_0002_0007_Omsk_allcation_population");
            DataTable populationDT = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "�����������", populationDT);

            // � ������ ������ �������� ����� ����������� ��� ���������� ��������
            if (populationDT.Columns.Count > 1 && populationDT.Rows.Count > 0)
            {
                if (populationDT.Rows[0][1] != DBNull.Value && populationDT.Rows[0][1].ToString() != String.Empty)
                {
                    populationTotal.Value = Convert.ToDouble(populationDT.Rows[0][1]).ToString().Replace(",", ".");
                    populationDT.Rows.RemoveAt(0);
                }
            }

            const string populationColumnName = "����������� ����������� ���������";
            const string avgExecuteColumnName = "����������� ��������� �������������� ���������";

            if (dtChart.Columns.Count > 1 && dtChart.Rows.Count > 0)
            {
                dtChart.PrimaryKey = new DataColumn[] { dtChart.Columns[0] };

                foreach (DataRow populationRow in populationDT.Rows)
                {
                    if (populationRow[0] != DBNull.Value)
                    {
                        string rowName = populationRow[0].ToString();
                        if (populationRow[populationColumnName] != DBNull.Value &&
                            populationRow[populationColumnName].ToString() != String.Empty)
                        {
                            double population = Convert.ToDouble(populationRow[populationColumnName]);

                            DataRow dtRow = dtChart.Rows.Find(rowName);
                            if (dtRow != null)
                            {
                                dtRow[populationColumnName] = population;

                                if (population != 0 && dtRow[avgExecuteColumnName] != DBNull.Value &&
                                    dtRow[avgExecuteColumnName].ToString() != String.Empty)
                                {
                                    double execute = Convert.ToDouble(dtRow[avgExecuteColumnName]);
                                    double avgExecute = execute / population;

                                    dtRow[avgExecuteColumnName] = avgExecute;
                                }
                            }
                        }
                    }
                }
            }

            query = DataProvider.GetQueryText("FO_0002_0007_Omsk_allocation_avg");
            dtAVG = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "�������", dtAVG);

            selectedPointIndex = -1;
            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                if (dtChart.Rows[i][0] != DBNull.Value && dtChart.Rows[i][0].ToString() == ComboRegion.SelectedValue)
                {
                    selectedPointIndex = i;
                    break;
                }
            }

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);

            subjectLabel.Text = string.Format("{0}:", ComboRegion.SelectedValue);
            populationLabel.Text = string.Empty;
            statisticLabel.Text = string.Empty;
            if (selectedPointIndex != -1 && dtChart.Rows[selectedPointIndex][1] != DBNull.Value &&
                dtChart.Rows[selectedPointIndex][2] != DBNull.Value)
            {
                populationLabel.Text = string.Format("����������� ����������� ��������� ({1}): <b>{0:N3}</b> ���.���.",
                        dtChart.Rows[selectedPointIndex][1], populationDate);

                double income = Convert.ToDouble(dtChart.Rows[selectedPointIndex][2]);
                statisticLabel.Text = string.Format("�� {1} {2} {3} ���� ��������� ������� �� ���� ��������� ({4}) ���������� <b>{0}</b>&nbsp;���./���.",
                    income.ToString("N2"), monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum, ComboFKR.SelectedValue);
            }

            UltraChart.DataSource = dtChart;
        }

        void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            foreach (Primitive primitive in e.SceneGraph)
            {
                if (selectedPointIndex == -1)
                {
                    break;
                }

                PointSet pointSet = primitive as PointSet;

                if (pointSet == null)
                {
                    continue;
                }

                foreach (DataPoint point in pointSet.points)
                {
                    if (point.Row == selectedPointIndex)
                    {
                        Symbol symbol = new Symbol(point.point, pointSet.icon, pointSet.iconSize);
                        symbol.PE.Fill = Color.DarkOrange;
                        e.SceneGraph.Add(symbol);
                    }
                }

                break;
            }

            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            int textWidht = 200;
            int textHeight = 12;
            int lineStart = (int)xAxis.MapMinimum;
            int lineLength = (int)xAxis.MapMaximum;

            int annotationColumnIndex = 0;
            double avgValue = 0;
            if (dtAVG.Rows[0].ItemArray.Length > annotationColumnIndex &&
                dtAVG.Rows[0][annotationColumnIndex] != DBNull.Value &&
                dtAVG.Rows[0][annotationColumnIndex].ToString() != string.Empty)
            {
                avgValue = Convert.ToDouble(dtAVG.Rows[0][annotationColumnIndex]);
            }

            Line line = new Line();
            line.lineStyle.DrawStyle = LineDrawStyle.Dash;
            line.PE.Stroke = Color.DarkGray;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point(lineStart, (int)yAxis.Map(avgValue));
            line.p2 = new Point(lineStart + lineLength, (int)yAxis.Map(avgValue));
            e.SceneGraph.Add(line);

            Text text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle(lineLength - textWidht, ((int)yAxis.Map(avgValue)) - textHeight, textWidht, textHeight);
            text.SetTextString(string.Format("������� �� ��: {0:N2} ���./���.", avgValue));
            e.SceneGraph.Add(text);
        }

        #endregion

        #region ������� � Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(new UltraWebGrid());
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(subjectLabel.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(RemoveBoldTags(populationLabel.Text));

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            string statisticsText = RemoveBoldTags(statisticLabel.Text);
            statisticsText = statisticsText.Replace("&nbsp;", " ");
            title.AddContent(RemoveBoldTags(statisticsText));

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.86));
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        private string RemoveBoldTags(string source)
        {
            string result = source.Replace("<b>", string.Empty);
            result = result.Replace("</b>", string.Empty);
            return result;
        }

        #endregion
    }
}
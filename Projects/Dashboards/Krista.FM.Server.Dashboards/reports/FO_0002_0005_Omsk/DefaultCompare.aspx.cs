using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs=Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Image=Infragistics.Documents.Reports.Graphics.Image;
using SerializationFormat=Dundas.Maps.WebControl.SerializationFormat;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0005_Omsk
{
    public partial class DefaultCompare : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2000;
        private int endYear = 2011;
        private int rubMultiplier;
        private string populationDate;

        // ��� ����� � ������� �������
        private string mapFolderName;
        // ��������� �����
        private double mapSizeProportion;
        // �������� �����
        private double mapZoomValue;

        #region ��������� �������

        // ������ �����
        private CustomParam incomesTotal;
        // ������ ��
        private CustomParam kdGroupName;
        // ������� �� � ��
        private CustomParam regionsLevel;
        // ������� ������������������ �������
        private CustomParam regionsConsolidateLevel;

        // ����������� ���������
        private CustomParam populationMeasure;
        // ��� ��� ����������� ���������
        private CustomParam populationMeasureYear;

        // ��� ��������� ���� ��� ������������������ ������
        private CustomParam consolidateDocumentSKIFType;
        // ��� ��������� ���� ��� ������ �������
        private CustomParam regionDocumentSKIFType;

        // ������� ������� ���� ��� �������
        private CustomParam regionBudgetSKIFLevel;

        // ��������� �������
        private CustomParam regionSet;

        // ����������� ������
        RankCalculator moRank = new RankCalculator(RankDirection.Asc);
        RankCalculator northZoneRank = new RankCalculator(RankDirection.Asc);
        RankCalculator northForestZoneRank = new RankCalculator(RankDirection.Asc);
        RankCalculator southForestZoneRank = new RankCalculator(RankDirection.Asc);
        RankCalculator southSteppeZoneRank = new RankCalculator(RankDirection.Asc);

        #endregion

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private bool UseClimaticZones
        {
            get { return useClimaticZones.Checked; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            mapFolderName = RegionSettingsHelper.Instance.GetPropertyValue("MapFolderName");
            mapZoomValue = Convert.ToDouble(RegionSettingsHelper.Instance.GetPropertyValue("MapZoomValue"));

            double value = 1;
            mapSizeProportion = value;
            if (Double.TryParse(RegionSettingsHelper.Instance.GetPropertyValue("MapSizeProportion"), NumberStyles.Any, NumberFormatInfo.CurrentInfo, out value))
            {
                mapSizeProportion = value;
            }
            
            #region ������������� ���������� �������

            incomesTotal = UserParams.CustomParam("incomes_total");
            kdGroupName = UserParams.CustomParam("kd_group_name");
            regionsLevel = UserParams.CustomParam("regions_level");
            regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            populationMeasure = UserParams.CustomParam("population_measure");
            populationMeasureYear = UserParams.CustomParam("population_measure_year");
            consolidateDocumentSKIFType = UserParams.CustomParam("consolidate_document_skif_type");
            regionDocumentSKIFType = UserParams.CustomParam("region_document_skif_type");
            regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            regionSet = UserParams.CustomParam("region_set");

            #endregion

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * mapSizeProportion - 225);
            UltraWebGrid.DisplayLayout.NoDataMessage = "��� ������";
            
            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight);

            CrossLink1.Text = "��������� ������������� �� �� ������������� �������";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0005_Omsk/DefaultAllocation.aspx";
            CrossLink2.Text = "������&nbsp;��������&nbsp;��&nbsp;���������";
            CrossLink2.NavigateUrl = "~/reports/FO_0002_0003_Omsk/DefaultDetail.aspx";

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <EndExportEventArgs>(PdfExporter_EndExport);

            PopupInformer1.HelpPageUrl = "DefaultCompare_MRRanking.html";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;

            UserParams.IncomesKD30000000000000000.Value = RegionSettingsHelper.Instance.IncomesKD30000000000000000;
            UserParams.IncomesKD11700000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11700000000000000;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            UserParams.IncomesKD10800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD10800000000000000;
            UserParams.IncomesKD11406000000000430.Value = RegionSettingsHelper.Instance.IncomesKD11406000000000430;
            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKD11402000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11402000000000000;
            UserParams.IncomesKD11403000000000410.Value = RegionSettingsHelper.Instance.IncomesKD11403000000000410;
            UserParams.IncomesKD11403000000000440.Value = RegionSettingsHelper.Instance.IncomesKD11403000000000440 !=
                                                          "null"
                                                              ? string.Format(",{2}.[{1}].[������ �� ������� ������������ � �������������� �������].[{0}],",
                                                                    RegionSettingsHelper.Instance.IncomesKD11403000000000440,
                                                                    RegionSettingsHelper.Instance.IncomesKDRootName,
                                                                    RegionSettingsHelper.Instance.IncomesKDAllLevel)
                                                              : ",";

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0005_Omsk_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = dtDate.Rows[0][3].ToString();
                UserParams.Filter.Value = "��� ����������� ������";
                UserParams.KDGroup.Value = "������ ������� - ����� ";

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.Set�heckedState(endYear.ToString(), true);

                ComboMonth.Title = "�����";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.Set�heckedState(UserParams.PeriodMonth.Value, true);

                ComboKD.Width = 300;
                ComboKD.Title = "��� ������";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(FillFullKDIncludingList());
                ComboKD.RenameTreeNodeByName("������ �� ������������������� ������������ ", String.Format("{0} ", UserParams.IncomesKD30000000000000000.Value));
                ComboKD.Set�heckedState(UserParams.KDGroup.Value, true);
            }

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            Page.Title = "���������� �������";
            Label1.Text = Page.Title;
            Label2.Text = string.Format("({3}) �� {0} {1} {2} ����", monthNum,
                CRHelper.RusManyMonthGenitive(monthNum), yearNum, ComboKD.SelectedValue.TrimEnd(' '));

            int year = Convert.ToInt32(ComboYear.SelectedValue);

            populationDate = DataDictionariesHelper.GetOmskRegionPopulationDate(yearNum);

            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            kdGroupName.Value = ComboKD.SelectedValue;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;

            populationMeasure.Value = RegionSettingsHelper.Instance.PopulationMeasure;
            populationMeasureYear.Value = RegionSettingsHelper.Instance.PopulationMeasurePlanning
                                              ? (year + 1).ToString()
                                              : year.ToString();

            consolidateDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            regionDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");
            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");

            rubMultiplier = IsThsRubSelected ? 1000 : 1000000;
            regionSet.Value = UseClimaticZones ? "������������� ����" : "������ ��";

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            string patternValue = UserParams.Subject.Value;
            int defaultRowIndex = 1;
            if (patternValue == string.Empty)
            {
                patternValue = UserParams.StateArea.Value;
                defaultRowIndex = 0;
            }

            UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
            ActiveGridRow(row);

            DundasMap.Shapes.Clear();
            if (Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("MapLoadFromBin")))
            {
                DundasMap.Serializer.Format = SerializationFormat.Binary;
                DundasMap.Serializer.Load((Server.MapPath(string.Format("../../maps/��������/{0}/{0}_�������.bin", mapFolderName))));
                SetMapSettings();
            }
            else
            {
                //DundasMap.ShapeFields.Clear();
                DundasMap.ShapeFields.Add("Name");
                DundasMap.ShapeFields["Name"].Type = typeof (string);
                DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
                DundasMap.ShapeFields.Add("CompletePercent");
                DundasMap.ShapeFields["CompletePercent"].Type = typeof (double);
                DundasMap.ShapeFields["CompletePercent"].UniqueIdentifier = false;

                SetMapSettings();
                AddMapLayer(DundasMap, mapFolderName, "��������", CRHelper.MapShapeType.Areas);
                //AddMapLayer(DundasMap, mapFolderName, "������", CRHelper.MapShapeType.Towns);
                //AddMapLayer(DundasMap, mapFolderName, "�������", CRHelper.MapShapeType.CalloutTowns);
            }

            // ��������� ����� �������
            FillMapData();
        }

        #region ����������� �����

        public void SetMapSettings()
        {
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.ZoomPanel.Dock = PanelDockStyle.Right;
            DundasMap.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.Viewport.Zoom = (float)mapZoomValue;

            DundasMap.Legends.Clear();

            Legend legend = new Legend("SubjectLegend");
            legend.Dock = PanelDockStyle.Left;
            legend.Visible = true;
            legend.BackColor = Color.White;
            legend.BackSecondaryColor = Color.Gainsboro;
            legend.BackGradientType = GradientType.DiagonalLeft;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Gray;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.Black;
            legend.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.Title = String.Empty;
            legend.AutoFitMinFontSize = 7;
            legend.ItemColumnSpacing = 100;
            
            LegendCellColumn column = new LegendCellColumn("���");
            column.HeaderFont = new Font("Verdana", 8, FontStyle.Bold);
            column.HeaderTextAlignment = StringAlignment.Far;
            legend.CellColumns.Add(column);

            column = new LegendCellColumn("����������");
            column.HeaderFont = new Font("Verdana", 8, FontStyle.Bold);
            column.HeaderTextAlignment = StringAlignment.Center;
            legend.CellColumns.Add(column);

            column = new LegendCellColumn("�������\n����������");
            column.HeaderFont = new Font("Verdana", 8, FontStyle.Bold);
            column.HeaderTextAlignment = StringAlignment.Far;
            legend.CellColumns.Add(column);

            DundasMap.Legends.Add(legend);

            Legend completelegend = new Legend("CompleteLegend");
            completelegend.Visible = true;
            completelegend.BackColor = Color.White;
            completelegend.BackSecondaryColor = Color.Gainsboro;
            completelegend.BackGradientType = GradientType.DiagonalLeft;
            completelegend.BackHatchStyle = MapHatchStyle.None;
            completelegend.BorderColor = Color.Gray;
            completelegend.BorderWidth = 1;
            completelegend.BorderStyle = MapDashStyle.Solid;
            completelegend.BackShadowOffset = 4;
            completelegend.TextColor = Color.Black;
            completelegend.Font = new System.Drawing.Font("MS Sans Serif", 7, FontStyle.Regular);
            completelegend.AutoFitText = true;
            completelegend.Title = "������� ����������";
            completelegend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Add(completelegend);

            // ��������� ������� ���������
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "CompletePercent";
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = 7;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Red;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Green;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";
            rule.LegendText = "#FROMVALUE{N0}% - #TOVALUE{N0}%";
            DundasMap.ShapeRules.Add(rule);
        }

        /// <summary>
        /// �������� �� ����� �������-��������
        /// </summary>
        /// <param name="shape">�����</param>
        /// <returns>true, ���� ��������</returns>
        public static bool IsCalloutTownShape(Shape shape)
        {
            return shape.Layer == CRHelper.MapShapeType.CalloutTowns.ToString();
        }

        /// <summary>
        /// ��������� ����� ����� (� ���������� ����� �� ������-�������)
        /// </summary>
        /// <param name="shape">�����</param>
        /// <returns>��� �����</returns>
        public static string GetShapeName(Shape shape)
        {
            string shapeName = shape.Name;
            if (IsCalloutTownShape(shape) && shape.Name.Split('_').Length > 1)
            {
                shapeName = shape.Name.Split('_')[0];
            }

            //shapeName = shapeName.Replace("������������� �����", "��");

            return shapeName;
        }

        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(string.Format("../../maps/��������/{0}/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;

            map.LoadFromShapeFile(layerName, "CODE", true);
            map.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
        }

        /// <summary>
        /// ����� ����� �����
        /// </summary>
        /// <param name="map">�����</param>
        /// <param name="patternValue">������� ��� �����</param>
        /// <returns>��������� �����</returns>
        public static ArrayList FindMapShape(MapControl map, string patternValue)
        {
            ArrayList shapeList = new ArrayList();
            foreach (Shape shape in map.Shapes)
            {
                if (GetShapeName(shape) == patternValue)
                {
                    shapeList.Add(shape);
                }
            }

            return shapeList;
        }


        public void FillMapData()
        {
            if (dtGrid == null || DundasMap == null)
            {
                return;
            }

//            foreach (Shape shape in DundasMap.Shapes)
//            {
//                string shapeName = GetShapeName(shape);
//                if (!RegionsNamingHelper.LocalBudgetUniqueNames.ContainsKey(shapeName))
//                {
//                    shape.Visible = false;
//                }
//            }

            int regionCodeColumnIndex = dtGrid.Columns.Count - 1;
            


            foreach (DataRow row in dtGrid.Rows)
            {
                // ��������� ����� �������
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[regionCodeColumnIndex] != DBNull.Value && row[regionCodeColumnIndex].ToString() != string.Empty &&
                    row[3] != DBNull.Value && row[3].ToString() != string.Empty)
                {
                    string subject = row[regionCodeColumnIndex].ToString();
                    double value = Convert.ToDouble(row[3]);
                    double climaticZoneValue = 0;

                    string climaticZoneName = String.Empty;
                    string zoneRegionsList = String.Empty;

                    if (UseClimaticZones && 
                        row[14] != DBNull.Value && row[14].ToString() != string.Empty &&
                        row[15] != DBNull.Value && row[15].ToString() != string.Empty &&
                        row[16] != DBNull.Value)
                    {
                        climaticZoneValue = Convert.ToDouble(row[14]);
                        climaticZoneName = row[15].ToString();
                        zoneRegionsList = row[16].ToString().Replace("br", "," + Environment.NewLine);
                    }

                    LegendItem item = new LegendItem();
                    LegendCell cell = new LegendCell(subject);
                    cell.Alignment = ContentAlignment.MiddleRight;
                    item.Cells.Add(cell);
                    cell = new LegendCell(row[0].ToString());
                    cell.Alignment = ContentAlignment.MiddleLeft;
                    item.Cells.Add(cell);
                    DundasMap.Legends["SubjectLegend"].Items.Add(item);

                    ArrayList shapeList = FindMapShape(DundasMap, subject);
                    foreach (Shape shape in shapeList)
                    {
                        string shapeName = GetShapeName(shape);
                        shape["Name"] = subject;
                        shape["CompletePercent"] = UseClimaticZones ? climaticZoneValue : value;
                        shape.ToolTip = string.Format("{0}\n#COMPLETEPERCENT{{N2}}%", 
                            UseClimaticZones
                                ? String.Format("{0}{1}({2})", climaticZoneName, Environment.NewLine, zoneRegionsList) 
                                : subject);

                        cell = new LegendCell(String.Format("{0:N2}%", value));
                        cell.Alignment = ContentAlignment.MiddleRight;
                        item.Cells.Add(cell);

                        shape.Text = string.Format("{0}\n{1:N2}%", shapeName.Replace(" ", "\n"), value);
                    }
                }
            }
        }

        #endregion

        #region ����������� �����

        /// <summary>
        /// ��������� ������ �����
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {
            if (row == null)
                return;

            UserParams.Subject.Value = row.Cells[0].Text;
        }
        
        private bool IsRankingRow(string rowName)
        {
            return rowName != "����������������� ������ ��������" && rowName != "������� �������" && rowName != "�.����" &&
                rowName != "��������� ������" && rowName != "������������� ������, �����" &&
                rowName != RegionSettingsHelper.Instance.OwnSubjectBudgetName;
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0005_Omsk_compare_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "�������", dtGrid);

            const string populationColumnName = "����������� ����������� ���������";
            const string executeColumnName = "����";
            const string avgExecuteColumnName = "������������� ������";
            string avgExecuteRankColumnName = "���� ��������� ��";
            string avgExecuteWorseRankColumnName = "������ ���� ��������� ��";

            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[1] != DBNull.Value)
                    {
                        row[1] = Convert.ToDouble(row[1]) / rubMultiplier;
                    }
                    if (row[2] != DBNull.Value)
                    {
                        row[2] = Convert.ToDouble(row[2]) / rubMultiplier;
                    }
                    if (row[3] != DBNull.Value)
                    {
                        row[3] = Convert.ToDouble(row[3]) * 100;
                    }
                }

                query = DataProvider.GetQueryText("FO_0002_0005_omsk_compare_population");
                DataTable populationDT = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "����������� ���������", populationDT);

                if (dtGrid.Columns.Count > 1 && dtGrid.Rows.Count > 0)
                {
                    dtGrid.PrimaryKey = new DataColumn[] { dtGrid.Columns[0] };

                    foreach (DataRow populationRow in populationDT.Rows)
                    {
                        if (populationRow[0] != DBNull.Value)
                        {
                            string rowName = populationRow[0].ToString();
                            if (populationRow[populationColumnName] != DBNull.Value && populationRow[populationColumnName].ToString() != String.Empty)
                            {
                                double population = Convert.ToDouble(populationRow[populationColumnName]);

                                DataRow dtRow = dtGrid.Rows.Find(rowName);
                                if (dtRow != null)
                                {
                                    dtRow[populationColumnName] = population;

                                    RankCalculator avgExecuteRank = GetRankCalculator(dtRow);

                                    if (population != 0 && dtRow[executeColumnName] != DBNull.Value && dtRow[executeColumnName].ToString() != String.Empty)
                                    {
                                        double execute = Convert.ToDouble(dtRow[executeColumnName]);
                                        double avgExecute = 1000 * execute / population;

                                        dtRow[avgExecuteColumnName] = avgExecute;

                                        if (IsRankingRow(rowName))
                                        {
                                            avgExecuteRank.AddItem(avgExecute);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    

                    // ��������� ��� ���, ����� ���������� �����
                    foreach (DataRow row in dtGrid.Rows)
                    {
                        string rowName = row[0].ToString();
                        if (IsRankingRow(rowName) && row[avgExecuteColumnName] != DBNull.Value && row[avgExecuteColumnName].ToString() != String.Empty)
                        {
                            double value = Convert.ToDouble(row[avgExecuteColumnName]);

                            RankCalculator avgExecuteRank = GetRankCalculator(row);
                            double avgExecuteWorseRank = avgExecuteRank.GetWorseRank();
                            int rank = avgExecuteRank.GetRank(value);
                            if (rank != 0)
                            {
                                row[avgExecuteRankColumnName] = rank;
                                row[avgExecuteWorseRankColumnName] = avgExecuteWorseRank;
                            }
                        }
                        else
                        {
                            row[avgExecuteRankColumnName] = DBNull.Value;
                        }
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        private RankCalculator GetRankCalculator(DataRow row)
        {
            if (!UseClimaticZones)
            {
                return moRank;
            }

            string regionType = String.Empty;
            if (row["���"] != DBNull.Value)
            {
                regionType = row["���"].ToString();
            }

            switch (regionType)
            {
                case "�������� ����":
                    {
                        return northZoneRank;
                    }
                case "�������� ����������� ����":
                    {
                        return northForestZoneRank;
                    }
                case "����� ����������� ����":
                    {
                        return southForestZoneRank;
                    }
                case "������� ����":
                    {
                        return southSteppeZoneRank;
                    }
                default:
                    {
                        return moRank;
                    }
            }
        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width, bool hidden)
        {
            CRHelper.FormatNumberColumn(layout.Bands[bandIndex].Columns[columnIndex], format);
            layout.Bands[bandIndex].Columns[columnIndex].Hidden = hidden;
            layout.Bands[bandIndex].Columns[columnIndex].Width = CRHelper.GetColumnWidth(width);
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(240);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            SetColumnParams(e.Layout, 0, 1, "N1", 100, false);
            SetColumnParams(e.Layout, 0, 2, "N1", 100, false);
            SetColumnParams(e.Layout, 0, 3, "N2", 80, false);
            SetColumnParams(e.Layout, 0, 4, "N0", 62, false);
            SetColumnParams(e.Layout, 0, 5, "N0", 62, true);
            SetColumnParams(e.Layout, 0, 6, "N0", 62, false);
            SetColumnParams(e.Layout, 0, 7, "N0", 62, true);
            SetColumnParams(e.Layout, 0, 8, "N1", 90, false);
            SetColumnParams(e.Layout, 0, 9, "N1", 100, false);
            SetColumnParams(e.Layout, 0, 10, "N0", 90, false);
            SetColumnParams(e.Layout, 0, 11, "N0", 90, true);
            SetColumnParams(e.Layout, 0, 12, "N2", 90, false);
            SetColumnParams(e.Layout, 0, 13, "N2", 90, false);

            SetColumnParams(e.Layout, 0, 14, "", 90, true);
            SetColumnParams(e.Layout, 0, 15, "", 90, true);
            SetColumnParams(e.Layout, 0, 16, "", 90, true);
            SetColumnParams(e.Layout, 0, 17, "", 90, true);

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, 
                String.Format("���������, {0}", RubMiltiplierButtonList.SelectedValue), "���� �� ���");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2,
                String.Format("���������, {0}", RubMiltiplierButtonList.SelectedValue), "����������� ���������� ����������� ������ � ������ ����");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "��������� %", "������� ���������� ����������. ������ ������������� ���������� (1/12 �������� ����� � �����)");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4,
                "���� %",
                "���� (�����) ������� �� �������� ���������� ���������� ����� ������������� �������/������������� ���");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6,
                "���� �� ������ ���-���",
                "���� (�����) �������������� ������/������������� ���� �� ������ ������������ ����������");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8, String.Format("����������� ����������� ��������� ({0}), ���.���.", populationDate), "����������� ����������� ���������");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 9, "������������� ������, ���./���.", "����� ������� ���������� ���� �� ���� ���������");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 10, 
                "���� �� ���������. ������",
                "���� (�����) ������� �� ������������� ������� ����� ������������� �������/������������� ���");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 12, "���� ��, %", "���� ������� ������� � ����� ����� ������� �������� ��, %");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 13, "���� ��, %", "���� ������� ������� � ����� ����� ������� ������������������ �������");
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.Padding.Top = 5;
                e.Row.Cells[i].Style.Padding.Bottom = 5;

                bool rank = (i == 6 || i == 4 || i == 10);
                bool complete = (i == 3);
                bool isClimaticZone = e.Row.Cells[0].ToString().Contains(" ����");

                if (!isClimaticZone && rank)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i + 1].Value != null &&
                        e.Row.Cells[i].Value.ToString() != string.Empty &&
                        e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                    {
                        string obj = (i == 10) ? "����� �� ���� ���������" : "������� ����������";

                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = (i == 6)
                                ? "����� � ����� ������� ��������� ������������ ����������"
                                : String.Format("����� ������� {0} ����� ��", obj);
                        }
                        else if (Convert.ToInt32(e.Row.Cells[i].Value) == Convert.ToInt32(e.Row.Cells[i + 1].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = (i == 6)
                                ? "����� � ����� ������� ��������� ������������ ����������"
                                : String.Format("����� ������ {0} ����� ��", obj);
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (complete)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        double percent = (ComboMonth.SelectedIndex + 1) * 100.0 / 12;
                        
                        if (Convert.ToDouble(e.Row.Cells[i].Value) < percent)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                            e.Row.Cells[i].Title = string.Format("�� ����������� ������� ������������� ({0:N2}%)", percent);
                        }
                        else
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                            e.Row.Cells[i].Title = string.Format("����������� ������� ������������� ({0:N2}%)", percent);
                        }
                        e.Row.Cells[i].Style.Padding.Right = 2;
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }

                if (e.Row.Cells[0].Value != null &&
                     (e.Row.Cells[0].Value.ToString().ToLower().Contains("������") || e.Row.Cells[0].Value.ToString().ToLower().Contains("�������") ||
                     e.Row.Cells[0].Value.ToString().ToLower().Contains(", �����") || e.Row.Cells[0].Value.ToString().ToLower().Contains(" ����")))
                {
                    foreach (UltraGridCell cell in e.Row.Cells)
                    {
                        cell.Style.Font.Bold = true;
                    }
                }
            }

            foreach (UltraGridCell cell in e.Row.Cells)
            {
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        #endregion

        #region ������� � Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text);
        }

        private void PdfExporter_EndExport(object sender, EndExportEventArgs e)
        {
            Image img = UltraGridExporter.GetImageFromMap(DundasMap);
            e.Section.AddImage(img);
        }
        
        #endregion

        #region ������� � Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text;
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            e.CurrentWorksheet.Columns[0].Width = 250 * 37;
            e.CurrentWorksheet.Columns[1].Width = 90 * 37;
            e.CurrentWorksheet.Columns[2].Width = 90 * 37;
            e.CurrentWorksheet.Columns[3].Width = 90 * 37;
            e.CurrentWorksheet.Columns[4].Width = 90 * 37;
            e.CurrentWorksheet.Columns[5].Width = 90 * 37;
            e.CurrentWorksheet.Columns[6].Width = 90 * 37;
            e.CurrentWorksheet.Columns[7].Width = 90 * 37;
            e.CurrentWorksheet.Columns[8].Width = 90 * 37;
            e.CurrentWorksheet.Columns[9].Width = 90 * 37;
            e.CurrentWorksheet.Columns[10].Width = 90 * 37;
            e.CurrentWorksheet.Columns[11].Width = 90 * 37;
            e.CurrentWorksheet.Columns[12].Width = 90 * 37;

            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "#,##0.0;[Red]-#,##0.0";
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.0;[Red]-#,##0.0";
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "##";
            e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "#,##0.0;[Red]-#,##0.0";
            e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "#,##0.0;[Red]-#,##0.0";
            e.CurrentWorksheet.Columns[6].CellFormat.FormatString = "##";
            e.CurrentWorksheet.Columns[7].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[8].CellFormat.FormatString = "##";
            e.CurrentWorksheet.Columns[9].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[10].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        #endregion

        /// <summary>
        /// ��������� ������ ������ �������� �� (� ���������� "� ��� �����")
        /// </summary>
        public static Dictionary<string, int> FillFullKDIncludingList()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("��������� ������ ", 0);
            valuesDictionary.Add("����� �� ������� ", 1);
            valuesDictionary.Add("���� ", 1);
            valuesDictionary.Add("������ �� ��������� ", 1);
            valuesDictionary.Add("����� �� ��������� ���.��� ", 2);
            valuesDictionary.Add("����� �� ��������� ����������� ", 2);
            valuesDictionary.Add("������������ ����� ", 2);
            valuesDictionary.Add("������������ ����� � ����������� ", 3);
            valuesDictionary.Add("������������ ����� � ���.��� ", 3);
            valuesDictionary.Add("��������� ����� ", 2);
            valuesDictionary.Add("������ ", 1);
            valuesDictionary.Add("������ �� ������������� ", 2);
            valuesDictionary.Add("������ �� �������� ", 2);
            valuesDictionary.Add("���� ", 1);
            valuesDictionary.Add("������ �� ���������� ����� ", 1);
            valuesDictionary.Add("��� ", 2);
            valuesDictionary.Add("���� ", 2);
            valuesDictionary.Add("���� ", 2);
            valuesDictionary.Add("���.������� ", 1);
            valuesDictionary.Add("������������� �� ���������� ������� ", 1);
            valuesDictionary.Add("����������� ������ ", 0);
            valuesDictionary.Add("������ �� ������������� ��������� ", 1);
            valuesDictionary.Add("��������� �� ������ ", 2);
            valuesDictionary.Add("�������� ����� �� ����� ", 2);
            valuesDictionary.Add("������ �� ����� � ������ ��������� ", 2);
            valuesDictionary.Add("������� �� ����� � ����� ", 2);
            valuesDictionary.Add("������� ��� ����������� ���������� ��������� ", 1);
            valuesDictionary.Add("����� �� ���������� ����������� �� �����.����� ", 2);
            valuesDictionary.Add("������� �� ����������� ������ ������ ", 2);
            valuesDictionary.Add("������� ��� ����������� ������� ", 2);
            valuesDictionary.Add("������ �� �������� ������� ����� ", 1);
            valuesDictionary.Add("������ �� ������� ������� ", 1);
            valuesDictionary.Add("������ �� ������� ������� (����� ���.��������) ", 2);
            valuesDictionary.Add("������ �� ������� ���. �������� ", 2);
            valuesDictionary.Add("���������������� ������� � ����� ", 1);
            valuesDictionary.Add("������ ", 1);
            valuesDictionary.Add("������ ", 1);
            valuesDictionary.Add("������ ����������� ������ ", 2);
            valuesDictionary.Add("������������ ����������� ", 2);
            valuesDictionary.Add("������ �������� �� �������� �������� ��� ������� ��� ", 1);
            valuesDictionary.Add("������� �������� ��� ������� ��� ", 1);
            valuesDictionary.Add("������ �� ���������� ����� ������������ ", 0);
            valuesDictionary.Add("��������� � ����������� ������ ", 0);
            valuesDictionary.Add("������������� ����������� ", 0);
            valuesDictionary.Add("������ ������� - ����� ", 0);
            return valuesDictionary;
        }
    }
}

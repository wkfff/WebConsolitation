using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using ContentAlignment=System.Drawing.ContentAlignment;

namespace Krista.FM.Server.Dashboards.reports.FO_0039_0005
{
    public enum ColoringRule
    {
        // ������� ��������� ����������
        SimpleRangeColoring,
        // ������� ��������� ����������
        ComplexRangeColoring,
        // �������������/�������������
        PositiveNegativeColoring
    }

    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable dtDate = new DataTable();
        private DataTable dtMap;
        private DataTable dtIndicatorDetail;
        private int firstYear = 2010;
        private int endYear = 2011;

        private string selectedIndicatorName;
        private int selectedQuarterIndex;
        private string indicatorName;
        private double beginQualityLimit;
        private double endQualityLimit;
        private string indicatorPeriod;

        private ColoringRule coloringRule;
        private Color beginMapColor;
        private Color middleMapColor;
        private Color endMapColor;
        private Color nullEmptyColor;
        private string nullEmptyText;
        private string mapFormatString;
        private string mapLegendCaption;
        
        #endregion

        // ��� ����� � ������� �������
        private string mapFolderName;
        // �������� �����
        private double mapZoomValue;

        private bool IsYearCompare
        {
            get { return selectedQuarterIndex == 4; }
        }

        private bool IsQualityDegree
        {
            get { return indicatorName == "�������� ������ ������ �������� �����"; }
        }

        private static bool IsMozilla
        {
            get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
        }

        public bool ValueSelected
        {
            get { return MeasureButtonList.SelectedIndex == 1; }
        }

        #region ��������� �������

        // ��������� ��������� ������
        private CustomParam selectedPeriod;
        // ��������� ���������
        private CustomParam selectedIndicator;
        // �������� ����
        private CustomParam selectedMeasure;
        // ������� �������
        private CustomParam regionsLevel;

        #endregion
        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            mapFolderName = RegionSettingsHelper.Instance.GetPropertyValue("MapFolderName");
            mapZoomValue = Convert.ToDouble(RegionSettingsHelper.Instance.GetPropertyValue("MapZoomValue"));

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 190);

            #region ������������� ���������� �������

            if (selectedPeriod == null)
            {
                selectedPeriod = UserParams.CustomParam("selected_period");
            }
            if (selectedIndicator == null)
            {
                selectedIndicator = UserParams.CustomParam("selected_indicator");
            }
            if (selectedMeasure == null)
            {
                selectedMeasure = UserParams.CustomParam("selected_measure");
            }
            regionsLevel = UserParams.CustomParam("regions_level");

            #endregion

            UltraGridExporter1.ExcelExportButton.Visible = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            CrossLink1.Visible = true;
            CrossLink1.Text = "����������&nbsp;������&nbsp;��������&nbsp;��";
            CrossLink1.NavigateUrl = "~/reports/FO_0039_0001/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "��������&nbsp;�����������&nbsp;������";
            CrossLink2.NavigateUrl = "~/reports/FO_0039_0002/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "�������&nbsp;��";
            CrossLink3.NavigateUrl = "~/reports/FO_0039_0003/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "�����.��������������&nbsp;���.&nbsp;�&nbsp;����.&nbsp;������";
            CrossLink4.NavigateUrl = "~/reports/FO_0039_0004/Default.aspx";

            CrossLink5.Visible = true;
            CrossLink5.Text = "����������&nbsp;������&nbsp;��&nbsp;���.����������";
            CrossLink5.NavigateUrl = "~/reports/FO_0039_0006/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0039_0005_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string quarter = dtDate.Rows[0][2].ToString();

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.Set�heckedState(endYear.ToString(), true);

                ComboQuarter.Title = "������ ��������";
                ComboQuarter.Width = 300;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillDateQuarters());
                ComboQuarter.Set�heckedState(GetParamQuarter(quarter), true);

                ComboQualityEvaluationIndicator.Title = "����������";
                ComboQualityEvaluationIndicator.Width = 300;
                ComboQualityEvaluationIndicator.MultiSelect = false;
                ComboQualityEvaluationIndicator.FillDictionaryValues(CustomMultiComboDataHelper.FillQualityEvaluationIndicatorList(DataDictionariesHelper.QualityEvaluationIndicatorList));
                ComboQualityEvaluationIndicator.Set�heckedState("�������� ������ ������ �������� �����", true);

                ComboQualityValueIndicator.Title = "����������";
                ComboQualityValueIndicator.Width = 300;
                ComboQualityValueIndicator.MultiSelect = false;
                ComboQualityValueIndicator.FillDictionaryValues(CustomMultiComboDataHelper.FillQualityValueIndicatorList(DataDictionariesHelper.QualityValueIndicatorList));
                ComboQualityValueIndicator.Set�heckedState("P1", true);
            }

            Page.Title = String.Format("����������� ����������� ������ �� ���������� ����������");
            PageTitle.Text = Page.Title;

            selectedQuarterIndex = ComboQuarter.SelectedIndex + 1;

            PageSubTitle.Text = (!IsYearCompare)
                ? String.Format("{0}.{1}", ComboQuarter.SelectedValue, ComboYear.SelectedValue)
                : String.Format("�� ������ {0} ����", ComboYear.SelectedValue);
            
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = String.Format("��������� {0}", CRHelper.HalfYearNumByQuarterNum(selectedQuarterIndex));
            UserParams.PeriodQuater.Value = String.Format("������� {0}", selectedQuarterIndex);

            selectedMeasure.Value = ValueSelected ? "��������" : "������ ����������";

            if (IsYearCompare)
            {
                selectedPeriod.Value = String.Format("[{0}]", UserParams.PeriodYear.Value);
            }
            else
            {
                selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);
            }

            if (ValueSelected)
            {
                valueComboTD.Visible = true;
                evaluationComboTD.Visible = false;

                selectedIndicatorName = ComboQualityValueIndicator.SelectedValue;
                selectedIndicator.Value = DataDictionariesHelper.QualityValueIndicatorList[ComboQualityValueIndicator.SelectedValue];
                ComboQualityEvaluationIndicator.Set�heckedState(selectedIndicatorName, true);
            }
            else
            {
                valueComboTD.Visible = false;
                evaluationComboTD.Visible = true;

                selectedIndicatorName = ComboQualityEvaluationIndicator.SelectedValue;
                selectedIndicator.Value = DataDictionariesHelper.QualityEvaluationIndicatorList[ComboQualityEvaluationIndicator.SelectedValue];
                ComboQualityValueIndicator.Set�heckedState(selectedIndicatorName, true);
            }
                        
            IndicatorDetailDataBind();
            mapElementCaption.Text = indicatorName == selectedIndicatorName
                ? String.Format("���������� �{0}�", indicatorName)
                : String.Format("���������� �{0}� ({1})", indicatorName, selectedIndicatorName);

            DundasMap.Shapes.Clear();
            if (Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("MapLoadFromBin")))
            {
                DundasMap.Serializer.Format = Dundas.Maps.WebControl.SerializationFormat.Binary;
                DundasMap.Serializer.Load((Server.MapPath(string.Format("../../maps/��������/{0}/{0}_�������.bin", mapFolderName))));
                SetMapSettings();
            }
            else
            {
                //DundasMap.ShapeFields.Clear();
                DundasMap.ShapeFields.Add("IndicatorName");
                DundasMap.ShapeFields["IndicatorName"].Type = typeof(string);
                DundasMap.ShapeFields["IndicatorName"].UniqueIdentifier = true;
                DundasMap.ShapeFields.Add("IndicatorValue");
                DundasMap.ShapeFields["IndicatorValue"].Type = typeof(double);
                DundasMap.ShapeFields["IndicatorValue"].UniqueIdentifier = false;

                SetMapSettings();
                AddMapLayer(DundasMap, mapFolderName, "��������", CRHelper.MapShapeType.Areas);
                AddMapLayer(DundasMap, mapFolderName, "������", CRHelper.MapShapeType.Towns);
                //AddMapLayer(DundasMap, mapFolderName, "�������", CRHelper.MapShapeType.CalloutTowns);
            }

            // ��������� ����� �������
            FillMapData();

        }

        /// <summary>
        /// �������� ������� ��������� �� �������� ��������������
        /// </summary>
        /// <param name="classQuarter">������� ��������������</param>
        /// <returns>�������� ���������</returns>
        private static string GetParamQuarter(string classQuarter)
        {
            switch (classQuarter)
            {
                case "������� 1":
                    {
                        return "�� ��������� �� 01.04";
                    }
                case "������� 2":
                    {
                        return "�� ��������� �� 01.07";
                    }
                case "������� 3":
                    {
                        return "�� ��������� �� 01.10";
                    }
                case "������� 4":
                case "������ ����":
                    {
                        return "�� ������ ����";
                    }
                default:
                    {
                        return classQuarter;
                    }
            }
        }

        #region ����������� �����

        public void SetMapSettings()
        {
            nullEmptyColor = Color.LightSkyBlue;
            nullEmptyText = (!IsYearCompare && indicatorPeriod.ToLower() == "���")
                ? "��� ������, �.�. ���������� ������������� ������ �� ������ ����"
                : "��� ������";
            
            switch (selectedIndicatorName)
            {
                case "�1":
                case "�2":
                case "�3":
                case "�4":
                case "�7":
                case "�8":
                case "�9":
                case "�13":
                case "�15":
                case "�16":
                case "�18":
                case "�20":
                case "�22":
                case "�29":
                case "�33":
                case "�34":
                case "�35":
                case "�36":
                case "�37":
                case "�38":
                case "�39":
                case "�40":
                case "�41":
                case "�42":
                    {
                        beginMapColor = ValueSelected ? Color.PaleGreen : Color.Salmon;
                        middleMapColor = ValueSelected ? Color.Thistle : Color.FromArgb(250, 251, 129);
                        endMapColor = ValueSelected ? Color.FromArgb(250, 251, 129) : Color.LightGreen;
                        mapFormatString = "N2";
                        coloringRule = ValueSelected ? ColoringRule.SimpleRangeColoring :  ColoringRule.PositiveNegativeColoring;
                        mapLegendCaption = ValueSelected ? "��������" : "������";
                        break;
                    }
                case "������� ��":
                    {
                        beginMapColor = Color.LightGreen;
                        middleMapColor = Color.FromArgb(250, 251, 129);
                        endMapColor = Color.Salmon;
                        mapFormatString = "N2";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        mapLegendCaption = "������� ��";
                        nullEmptyText = "�� ��������� � ��������";
                        break;
                    }
                case "������� ������ ������ �������� �����":
                    {
                        beginMapColor = Color.LightGreen;
                        middleMapColor = Color.FromArgb(250, 251, 129);
                        endMapColor = Color.Salmon;
                        mapFormatString = "N2";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        mapLegendCaption = "������� ������ ��������";
                        break;
                    }
                case "�������� ������ ������ �������� �����":
                    {
                        beginMapColor = Color.Salmon;
                        middleMapColor = Color.FromArgb(250, 251, 129);
                        endMapColor = Color.LightGreen;
                        mapFormatString = "N2";
                        coloringRule = ColoringRule.ComplexRangeColoring;
                        mapLegendCaption = "������� ��������";
                        break;
                    }
                default:
                    {
                        beginMapColor = ValueSelected ? Color.PaleGreen : Color.Salmon;
                        middleMapColor = ValueSelected ? Color.Thistle : Color.FromArgb(250, 251, 129);
                        endMapColor = ValueSelected ? Color.Khaki : Color.LightGreen;
                        mapFormatString = "N2";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        mapLegendCaption = ValueSelected ? "��������" : "������";
                        break;
                    }
            }

            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.Viewport.Zoom = (float)mapZoomValue;

            DundasMap.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap.ZoomPanel.Dock = PanelDockStyle.Right;

            // ��������� �������
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
            legend.Title = String.Empty;//"���� ����������";
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

            column = new LegendCellColumn(ValueSelected ? "��������" : "������");
            column.HeaderFont = new Font("Verdana", 8, FontStyle.Bold);
            column.HeaderTextAlignment = StringAlignment.Far;
            legend.CellColumns.Add(column);

//            legend.CellColumns.Add("���");
//            legend.CellColumns.Add("����������");
//            legend.CellColumns.Add("������");

            DundasMap.Legends.Add(legend);

            legend = new Legend("MapLegend");
            legend.Dock = PanelDockStyle.Right;
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
            legend.Title = mapLegendCaption;
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Add(legend);

            if (coloringRule == ColoringRule.SimpleRangeColoring)
            {
                // ��������� ������� ���������
                ShapeRule rule = new ShapeRule();
                rule.Name = "IndicatorRule";
                rule.Category = String.Empty;
                rule.ShapeField = "IndicatorValue";
                rule.DataGrouping = DataGrouping.Optimal;
                rule.ColorCount = 3;
                rule.ColoringMode = ColoringMode.ColorRange;
                rule.FromColor = beginMapColor;
                rule.MiddleColor = middleMapColor;
                rule.ToColor = endMapColor;
                rule.BorderColor = Color.FromArgb(50, Color.Black);
                rule.GradientType = GradientType.None;
                rule.HatchStyle = MapHatchStyle.None;
                rule.ShowInColorSwatch = false;
                rule.ShowInLegend = "MapLegend";
                rule.LegendText = (selectedIndicatorName == "������� ��")
                    ? String.Format("#FROMVALUE{{{0}}}.00 - #TOVALUE{{{0}}}.00", "N0")
                    : String.Format("#FROMVALUE{{{0}}} - #TOVALUE{{{0}}}", mapFormatString);
                DundasMap.ShapeRules.Add(rule);
            }
            else if (coloringRule == ColoringRule.ComplexRangeColoring)
            {
                if (beginQualityLimit != endQualityLimit)
                {
                    LegendItem item = new LegendItem();
                    item.Text = String.Format("����� {0} {1}", endQualityLimit.ToString(mapFormatString), IsQualityDegree ? "(III �������)" : String.Empty);
                    item.Color = beginMapColor;
                    DundasMap.Legends["MapLegend"].Items.Add(item);

                    item = new LegendItem();
                    item.Text = String.Format("{0} - {1} {2}", endQualityLimit.ToString(mapFormatString), beginQualityLimit.ToString(mapFormatString), 
                        IsQualityDegree ? "(II �������)" : String.Empty);
                    item.Color = Color.FromArgb(250, 251, 129);
                    DundasMap.Legends["MapLegend"].Items.Add(item);

                    item = new LegendItem();
                    item.Text = String.Format("����� {0} {1}", beginQualityLimit.ToString(mapFormatString), IsQualityDegree ? "(I �������)" : String.Empty);
                    item.Color = endMapColor;
                    DundasMap.Legends["MapLegend"].Items.Add(item);
                }
                else
                {
                    LegendItem item = new LegendItem();
                    item.Text = String.Format("{0}", beginQualityLimit.ToString(mapFormatString));
                    item.Color = endMapColor;
                    DundasMap.Legends["MapLegend"].Items.Add(item);
                }
           }
            else if (coloringRule == ColoringRule.PositiveNegativeColoring)
            {
                LegendItem item = new LegendItem();
                item.Text = "0.00";
                item.Color = beginMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = "1.00";
                item.Color = endMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);
            }
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

        private static bool NonNullValueGrid(DataTable dt, int columnIndex)
        {
            if (dt != null && dt.Columns.Count > columnIndex)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        if (row[columnIndex] != DBNull.Value)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            return false;
        }

        private static bool AreBinaryValues(DataTable dt, int columnIndex)
        {
            if (dt != null && dt.Columns.Count > columnIndex)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row[columnIndex] != DBNull.Value && row[columnIndex].ToString() != String.Empty)
                    {
                        double value = Convert.ToDouble(row[columnIndex]);
                        if (value == 0 || value == 1)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            return false;
        }

        public void FillMapData()
        {
            string query = DataProvider.GetQueryText("FO_0039_0005_map");
            dtMap = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query,
                                                                             "������������ �������������� �����������",
                                                                             dtMap);
            
            foreach (Shape shape in DundasMap.Shapes)
            {
                shape.Visible = false;
            }

            string valueSeparator = IsMozilla ? ";" : "\n";

            // ��� �������, ����� ������ ���� � �������
//            if (DundasMap.ShapeRules.Count > 0 && AreBinaryValues(dtMap, 2))
//            {
//                coloringRule = ColoringRule.PositiveNegativeColoring;
//                DundasMap.ShapeRules["IndicatorRule"].ColorCount = 2;
//                DundasMap.ShapeRules["IndicatorRule"].LegendText = string.Format("#FROMVALUE{{{0}}}", mapFormatString);
//            }

            if (!NonNullValueGrid(dtMap, 2))
            {
                if (coloringRule == ColoringRule.SimpleRangeColoring)
                {
                    DundasMap.ShapeRules["IndicatorRule"].ShowInLegend = "";
                }
                else
                {
                    DundasMap.Legends["MapLegend"].Items.Clear();
                }
            }
                       
            if (coloringRule == ColoringRule.SimpleRangeColoring)
            {
                int colorCount = CRHelper.GetMapIntervalCount(dtMap, 2, 3, true);
                if (colorCount == 0)
                {
                    DundasMap.ShapeRules["IndicatorRule"].ColorCount = 1;
                    DundasMap.ShapeRules["IndicatorRule"].LegendText = string.Format("#FROMVALUE{{{0}}}", mapFormatString);
                }
//                switch (colorCount)
//                {
//                    case -1:
//                        {
                            // ������ ������� ��������� �� �������
//                            DundasMap.ShapeRules["IndicatorRule"].ShowInLegend = string.Empty;
//                            break;
//                        }
//                    case 0:
//                        {
                            // �������� ������ ����
//                            DundasMap.ShapeRules["IndicatorRule"].ColorCount = 1;
//                            DundasMap.ShapeRules["IndicatorRule"].LegendText = string.Format("#FROMVALUE{{{0}}}", mapFormatString);
//                            break;
//                        }
//                    default:
//                        {
                            // �������� ���������
//                            DundasMap.ShapeRules["IndicatorRule"].ColorCount = colorCount;
//                            break;
//                        }
//                }
            }

            bool nullValueExists = false;

            foreach (DataRow row in dtMap.Rows)
            {
                string subjectName = row[0].ToString();
                string subjectCode = row[1].ToString();

                LegendItem item = new LegendItem();
                LegendCell cell = new LegendCell(subjectCode);
                cell.Alignment = ContentAlignment.MiddleRight;
                item.Cells.Add(cell);
                cell = new LegendCell(subjectName);
                cell.Alignment = ContentAlignment.MiddleLeft;
                item.Cells.Add(cell);
                DundasMap.Legends["SubjectLegend"].Items.Add(item);

                ArrayList shapeList = FindMapShape(DundasMap, subjectCode);
                foreach (Shape shape in shapeList)
                {
                    string shapeName = GetShapeName(shape);
                    shape.Visible = true;

                    shape.BorderWidth = 2;
                    shape.BorderStyle = MapDashStyle.Solid;
                    shape.BorderColor = Color.Black;

                    // ��������� ����� �������
                    if (row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                    {
                        double value = Convert.ToDouble(row[2]);

                        cell = new LegendCell(value.ToString(mapFormatString));
                        cell.Alignment = ContentAlignment.MiddleRight;
                        item.Cells.Add(cell);

                        shape["IndicatorName"] = subjectName;
                        shape["IndicatorValue"] = value;
                        shape.ToolTip = string.Format("{0}{1}{2}: #INDICATORVALUE{{{3}}}", subjectName, valueSeparator, selectedIndicatorName, mapFormatString);


                        if (coloringRule == ColoringRule.ComplexRangeColoring)
                        {
                            shape.Color = GetQualityColor(value);
                        }
                        else if (coloringRule == ColoringRule.PositiveNegativeColoring)
                        {
                            if (value == 0)
                            {
                                shape.Color = beginMapColor;
                            }
                            else if (value == 1)
                            {
                                shape.Color = endMapColor;
                            }
                        }

                         shape.Text = string.Format("{0}\n{1}", shapeName.Replace(" ", "\n"), value.ToString(mapFormatString));
                    }
                    else
                    {
                        cell = new LegendCell("-");
                        cell.Alignment = ContentAlignment.MiddleRight;
                        item.Cells.Add(cell);

                        shape.Color = nullEmptyColor;
                        shape.ToolTip = string.Format("{0}\n{1}", subjectName, nullEmptyText);
                        shape.Text = shapeName;

                        if (!nullValueExists)
                        {
                            nullValueExists = true;

                            LegendItem nullTextItem = new LegendItem();
                            nullTextItem.Text = nullEmptyText;
                            nullTextItem.Color = nullEmptyColor;
                            DundasMap.Legends["MapLegend"].Items.Add(nullTextItem);
                        }
                    }
                }
            }
        }

        #endregion

        #region ��������� ����������

        protected void IndicatorDetailDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0039_0003_indicator_detail");
            dtIndicatorDetail = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtIndicatorDetail);

            indicatorName = GetStringDTValue(dtIndicatorDetail, "������������");
            beginQualityLimit = GetDoubleDTValue(dtIndicatorDetail, "��������� ������� ���������");
            endQualityLimit = GetDoubleDTValue(dtIndicatorDetail, "�������� ������� ���������");
            indicatorPeriod = GetStringDTValue(dtIndicatorDetail, "������������� ������� ����������");
        }

        private Color GetQualityColor(double value)
        {
            if (value > beginQualityLimit)
            {
                return Color.LightGreen;
            }
            if (value <= endQualityLimit)
            {
                return Color.Salmon;
            }
            return Color.FromArgb(250, 251, 129);
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, 0);
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != String.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
        }

        private static string GetStringDTValue(DataTable dt, string columnName)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != String.Empty)
            {
                return dt.Rows[0][columnName].ToString();
            }
            return String.Empty;
        }

        #endregion

        #region ������� � Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = mapElementCaption.Text;

            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraGridExporter.MapExcelExport(e.CurrentWorksheet.Rows[3].Cells[0], DundasMap);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(emptyExportGrid);
        }

        #endregion

        #region ������� � PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            
            Report report = new Report();
            ISection section = report.AddSection();

            UltraGridExporter1.PdfExporter.Export(new UltraWebGrid(), section);
        }
        
        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);
            
            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(mapElementCaption.Text);

            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            DundasMap.ZoomPanel.Visible = false;
            DundasMap.NavigationPanel.Visible = false;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap);
            e.Section.AddImage(img);
        }

        #endregion
    }
}

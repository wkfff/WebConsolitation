using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.Misc;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using ContentAlignment=System.Drawing.ContentAlignment;

namespace Krista.FM.Server.Dashboards.reports.FO_0021_0004
{
    public enum ColoringRule
    {
        // ��������� ������ �������
        TopFiveColoring,
        // ��������� ������ �������
        BlueSolidColoring,
        // ��������� ������� � ������������� ��������
        PositiveZeroColoring,
        // ��������� ����������� ��������� �������� ������
        CorrectionIndicatorColoring
    }

    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable dtDate = new DataTable();
        private DataTable dtMap;
        private int firstYear = 2009;
        private int endYear = 2012;

        private string selectedIndicatorName;
        private string selectedIndicatorType;
        private string selectedIndicatorCode;

        private int selectedQuarterIndex;
        private int selectedYear;

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
        // ����� �� ��������� ������ �������-������� �����
        private double mapCalloutOffsetY;

        private static MemberAttributesDigest indicatorDigest;

        private bool IsYearSelected
        {
            get { return selectedQuarterIndex == 4; }
        }

        private bool IsQuarterIndicatorSelected
        {
            get
            {
                return selectedIndicatorType != "��������";
            }
        }
        
        private static bool IsMozilla
        {
            get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
        }

        #region ��������� �������

        // ��������� ������
        private CustomParam selectedPeriod;
        // ��������� ���������
        private CustomParam selectedIndicator;
        // ��� ���������� ����������
        private CustomParam selectedIndicatorCaption;
        // ��� ���������� ������� ��� �����
        private CustomParam sortingType;
        // ������ �� �����������
        private CustomParam directionFilter;

        #endregion
        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            mapFolderName = RegionSettingsHelper.Instance.GetPropertyValue("MapFolderName");
            mapZoomValue = Convert.ToDouble(RegionSettingsHelper.Instance.GetPropertyValue("MapZoomValue"));

            double value = 1;
            mapCalloutOffsetY = value;
            if (Double.TryParse(RegionSettingsHelper.Instance.GetPropertyValue("MapCalloutOffsetY"), NumberStyles.Any, NumberFormatInfo.CurrentInfo, out value))
            {
                mapCalloutOffsetY = value;
            }

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 200);

            #region ������������� ���������� �������

            selectedPeriod = UserParams.CustomParam("selected_period");
            selectedIndicator = UserParams.CustomParam("selected_indicator");
            sortingType = UserParams.CustomParam("sorting_type");
            selectedIndicatorCaption = UserParams.CustomParam("selected_indicator_caption");
            directionFilter = UserParams.CustomParam("direction_filter");

            #endregion

            UltraGridExporter1.ExcelExportButton.Visible = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            CrossLink1.Visible = true;
            CrossLink1.Text = "����������&nbsp;������&nbsp;��������";
            CrossLink1.NavigateUrl = "~/reports/FO_0021_0001/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "�������&nbsp;��";
            CrossLink2.NavigateUrl = "~/reports/FO_0021_0002/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "��������&nbsp;�����������&nbsp;������&nbsp;��������";
            CrossLink3.NavigateUrl = "~/reports/FO_0021_0003/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "���������&nbsp;��������&nbsp;������&nbsp;��������";
            CrossLink4.NavigateUrl = "~/reports/FO_0021_0005/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0021_0004_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string quarter = dtDate.Rows[0][2].ToString();

                selectedIndicatorCaption.Value = "�������";

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.Set�heckedState(endYear.ToString(), true);
                ComboYear.AutoPostBack = true;

                ComboQuarter.Title = "�������";
                ComboQuarter.Width = 300;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillEvaluaitonQuarters());
                ComboQuarter.Set�heckedState(GetParamQuarter(quarter), true);
                ComboQuarter.AutoPostBack = true;
            }

            selectedYear = Convert.ToInt32(ComboYear.SelectedValue);
            selectedQuarterIndex = ComboQuarter.SelectedIndex + 2;

            Page.Title = String.Format("����������� ����������� ����������� �� ���������� ����������");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = (selectedQuarterIndex == 4)
                ? String.Format("�� ������ {0} ����", selectedYear)
                : String.Format("�� ������ {0} �������� {1} ����", selectedQuarterIndex, selectedYear);

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = String.Format("��������� {0}", CRHelper.HalfYearNumByQuarterNum(selectedQuarterIndex));
            UserParams.PeriodQuater.Value = String.Format("������� {0}", selectedQuarterIndex);

            selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);

            directionFilter.Value = IsYearSelected ? "true" : "Measures.[��������� �����������]";

            if (ComboIndicator.SelectedValue != String.Empty)
            {
                selectedIndicatorCaption.Value = ComboIndicator.SelectedValue;
            }

            ComboIndicator.Title = "����������";
            ComboIndicator.Width = 510;
            ComboIndicator.MultiSelect = false;
            indicatorDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0021_0004_indicatorList");
            ComboIndicator.ClearNodes();
            ComboIndicator.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(indicatorDigest.UniqueNames, indicatorDigest.MemberLevels));
            ComboIndicator.SelectLastNode();
            ComboIndicator.Set�heckedState(selectedIndicatorCaption.Value, true);
            
            selectedIndicatorName = ComboIndicator.SelectedValue;
            selectedIndicator.Value = indicatorDigest.GetMemberUniqueName(selectedIndicatorName);
            selectedIndicatorType = indicatorDigest.GetMemberType(selectedIndicatorName);
            selectedIndicatorCode = indicatorDigest.GetShortName(selectedIndicatorName);

            mapElementCaption.Text = selectedIndicatorName == String.Empty ? String.Empty : String.Format("���������� �{0}�", selectedIndicatorName);

            sortingType.Value = selectedIndicatorName == "�������" ? "BASC" : "BDESC";

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
                AddMapLayer(DundasMap, mapFolderName, "�������", CRHelper.MapShapeType.CalloutTowns);
            }

            if (selectedIndicator.Value == String.Empty)
            {
                ShowNoDataMessage();
            }
            else
            {
                // ��������� ����� �������
                FillMapData();
            }
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
                case "������� 2":
                    {
                        return "������ ������� �� 01.07";
                    }
                case "������� 3":
                    {
                        return "������ �������� �� 01.10";
                    }
                case "������� 4":
                    {
                        return "������ �������� �� ������ ����";
                    }
                default:
                    {
                        return classQuarter;
                    }
            }
        }

        #region ����������� �����

        private string GetIndicatorFormatString(string indicatorCode)
        {
            switch (indicatorCode)
            {
                case "�������� ������":
                case "������� ������":
                    {
                        return "N1";
                    }
                case "�������":
                case "V34":
                case "V35":
                case "V36":
                    {
                        return "N0";
                    }
                default:
                    {
                        return "N3";
                    }
            }
        }
        
        public void SetMapSettings()
        {
            nullEmptyColor = Color.LightSkyBlue;
            nullEmptyText = "��� ������";
            mapFormatString = GetIndicatorFormatString(selectedIndicatorCode);

            switch (selectedIndicatorCode)
            {
                case "�������":
                case "�������� ������":
                    {
                        coloringRule = ColoringRule.TopFiveColoring;

                        beginMapColor = Color.DarkGreen;
                        middleMapColor = Color.LightGreen;
                        endMapColor = Color.Yellow;
                        mapLegendCaption = selectedIndicatorName;
                        break;
                    }
                case "������� ������":
                    {
                        coloringRule = ColoringRule.BlueSolidColoring;

                        beginMapColor = Color.LightSkyBlue;
                        middleMapColor = Color.LightSkyBlue;
                        endMapColor = Color.LightSkyBlue;
                        mapLegendCaption = selectedIndicatorName;
                        break;
                    }
                case "V34":
                case "V35":
                case "V36":
                    {
                        coloringRule = ColoringRule.CorrectionIndicatorColoring;
                        
                        beginMapColor = Color.Red;
                        middleMapColor = Color.Yellow;
                        endMapColor = Color.Green;
                        mapLegendCaption = "�������������";
                        break;
                    }
                default:
                    {
                        coloringRule = ColoringRule.PositiveZeroColoring;

                        beginMapColor = Color.Red;
                        middleMapColor =  Color.Yellow;
                        endMapColor = Color.Green;
                        mapLegendCaption = "��������\n���";
                        break;
                    }
            }

            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.Viewport.Zoom = (float)mapZoomValue;
            DundasMap.Viewport.ViewCenter.X -= 10;

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
            legend.Title = String.Empty;
            legend.AutoFitMinFontSize = 7;
            legend.ItemColumnSpacing = 100;

            LegendCellColumn column = new LegendCellColumn("����������");
            column.HeaderFont = new Font("Verdana", 8, FontStyle.Bold);
            column.HeaderTextAlignment = StringAlignment.Center;
            legend.CellColumns.Add(column);

            column = new LegendCellColumn(mapLegendCaption.Replace(' ', '\n'));
            column.HeaderFont = new Font("Verdana", 8, FontStyle.Bold);
            column.HeaderTextAlignment = StringAlignment.Center;
            legend.CellColumns.Add(column);

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

            if (coloringRule == ColoringRule.TopFiveColoring)
            {
                LegendItem item = new LegendItem();
                item.Text = String.Format("������� 1");
                item.Color = beginMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("������� 2 - 3");
                item.Color = middleMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("������� 4 - 5");
                item.Color = endMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("������� ���� 5");
                item.Color = Color.LightSkyBlue;
                DundasMap.Legends["MapLegend"].Items.Add(item);
            }
            else if (coloringRule == ColoringRule.PositiveZeroColoring)
            {
                LegendItem item = new LegendItem();
                item.Text = "������ ����� 0";
                item.Color = beginMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = "������ ������ 0";
                item.Color = endMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);
            }
            else if (coloringRule == ColoringRule.CorrectionIndicatorColoring)
            {
                LegendItem item = new LegendItem();
                item.Text = "����� 5%";
                item.Color = beginMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = "��� �������������";
                item.Color = endMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);
            }
            else if (coloringRule == ColoringRule.BlueSolidColoring)
            {
                DundasMap.Legends["MapLegend"].Items.Clear();
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
        /// �������� �� ����� �������
        /// </summary>
        /// <param name="shape">�����</param>
        /// <returns>true, ���� ��������</returns>
        public static bool IsTownShape(Shape shape)
        {
            string shapeName = shape.Name;
            if (RegionsNamingHelper.LocalBudgetTypes.ContainsKey(shapeName))
            {
                return RegionsNamingHelper.LocalBudgetTypes[shapeName].Contains("��");
            }

            return false;
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

            return shapeName;
        }

        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(string.Format("../../maps/��������/{0}/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;

            //map.LoadFromShapeFile(layerName, "CODE", true);
            map.LoadFromShapeFile(layerName, "NAME", true);
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

        private string GetShortRegionName(string fullRegionName)
        {
            string shortRegionName = fullRegionName.Replace("������������� �����������", "��");
            shortRegionName = shortRegionName.Replace("������������� �����", "��");
            return shortRegionName;
        }

        private void ShowNoDataMessage()
        {
            DundasMap.Visible = false;
            EmptyMapCaption.Visible = true;
            EmptyMapCaption.Text = "<br/>��� ������";
        }

        public void FillMapData()
        {
            DundasMap.Visible = true;
            EmptyMapCaption.Visible = false;

            string query = DataProvider.GetQueryText("FO_0021_0004_map");
            dtMap = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query,
                                                                             "������������ �������������� �����������",
                                                                             dtMap);

            if (!NonNullValueGrid(dtMap, 1))
            {
                ShowNoDataMessage();
                return;
            }

            foreach (Shape shape in DundasMap.Shapes)
            {
                shape.Visible = false;
            }

            string valueSeparator = IsMozilla ? ";" : "\n";
            
            for (int i = 0; i < dtMap.Rows.Count; i++)
            {
                DataRow row = dtMap.Rows[i];

                string subjectName = row[0].ToString();

                LegendItem item = new LegendItem();
                LegendCell cell = new LegendCell(GetShortRegionName(subjectName));
                cell.Alignment = ContentAlignment.MiddleLeft;
                item.Cells.Add(cell);
                DundasMap.Legends["SubjectLegend"].Items.Add(item);

                ArrayList shapeList = FindMapShape(DundasMap, subjectName);
                foreach (Shape shape in shapeList)
                {
                    string shapeName = GetShapeName(shape);
                    shape.Visible = true;

                    subjectName = GetShortRegionName(subjectName);

                    // ��������� ����� �������
                    if (row[1] != DBNull.Value && row[1].ToString() != String.Empty)
                    {
                        double value = Convert.ToDouble(row[1]);

                        if (!IsCalloutTownShape(shape))
                        {
                            cell = new LegendCell(value.ToString(mapFormatString));
                            cell.Margins.Right = 10;
                            cell.Alignment = ContentAlignment.MiddleRight;
                            item.Cells.Add(cell);
                        }

                        shape["IndicatorName"] = subjectName;
                        shape["IndicatorValue"] = value;
                        shape.ToolTip = string.Format("{0}{1}{2}: #INDICATORVALUE{{{3}}}", subjectName.Replace("\"", "&quot"), valueSeparator, selectedIndicatorName, mapFormatString);

                        switch (coloringRule)
                        {
                            case ColoringRule.BlueSolidColoring:
                                {
                                    shape.Color = beginMapColor;
                                    break;
                                }
                            case ColoringRule.PositiveZeroColoring:
                                {
                                    if (value == 0)
                                    {
                                        shape.Color = beginMapColor;
                                    }
                                    else if (value > 0)
                                    {
                                        shape.Color = endMapColor;
                                    }
                                    break;
                                }
                            case ColoringRule.TopFiveColoring:
                                {
                                    int ratinig = i + 1;
                                    switch (ratinig)
                                    {
                                        case 1:
                                            {
                                                shape.Color = beginMapColor;
                                                break;
                                            }
                                        case 2:
                                        case 3:
                                            {
                                                shape.Color = middleMapColor;
                                                break;
                                            }
                                        case 4:
                                        case 5:
                                            {
                                                shape.Color = endMapColor;
                                                break;
                                            }
                                        default:
                                            {
                                                shape.Color = nullEmptyColor;
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case ColoringRule.CorrectionIndicatorColoring:
                                {
                                    if (value == -5)
                                    {
                                        shape.Color = beginMapColor;
                                    }
                                    else
                                    {
                                        shape.Color = endMapColor;
                                    }
                                    break;
                                }
                        }


                        shape.TextVisibility = IsTownShape(shape) ? TextVisibility.Auto : TextVisibility.Shown;
                        
                        if (IsCalloutTownShape(shape))
                        {
                            shape.Text = string.Format("{0}\n{1}", subjectName, value.ToString(mapFormatString));
                            shape.TextAlignment = ContentAlignment.BottomCenter;

                            shape.CentralPointOffset.Y = mapCalloutOffsetY;
                        }
                        else
                        {
                            shape.Text = string.Format("{0}\n{1}", subjectName.Replace(" ", "\n"), value.ToString(mapFormatString));
                        }
                    }
                    else
                    {
                        if (IsCalloutTownShape(shape))
                        {
                            shape.Color = nullEmptyColor;
                            shape.ToolTip = subjectName.Replace("\"", "&quot");
                            shape.Text = shapeName;
                            shape.TextVisibility = TextVisibility.Shown;
                            shape.TextAlignment = ContentAlignment.BottomCenter;

                            shape.CentralPointOffset.Y = mapCalloutOffsetY;
                        }
                        else
                        {
                            cell = new LegendCell("-");
                            cell.Alignment = ContentAlignment.MiddleCenter;
                            item.Cells.Add(cell);

                            shape.Color = nullEmptyColor;
                            shape.ToolTip = string.Format("{0}\n{1}", subjectName.Replace("\"", "&quot"), nullEmptyText);
                            shape.Text = shapeName;
                        }
                    }
                }
            }
        }

        #endregion

        #region ������� � Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = mapElementCaption.Text;

            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth));
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

            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.9));
            DundasMap.ZoomPanel.Visible = false;
            DundasMap.NavigationPanel.Visible = false;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap);
            e.Section.AddImage(img);
        }

        #endregion
    }
}
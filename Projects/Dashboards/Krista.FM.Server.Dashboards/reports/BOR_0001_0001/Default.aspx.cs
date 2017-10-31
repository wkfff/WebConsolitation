using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;

namespace Krista.FM.Server.Dashboards.reports.BOR_0001_0001
{
    public partial class Default : CustomReportPage
    {
        private static bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 900; }
        }

        private static int MinScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }

        private static int MinScreenHeight
        {
            get { return IsSmallResolution ? 700 : CustomReportConst.minScreenHeight; }
        }

        #region Поля

        private DataTable dtGrid = new DataTable();
        private string formatString = "N0";
        private string measure = String.Empty;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = IsSmallResolution ? CRHelper.GetGridWidth(MinScreenWidth + 20) : CRHelper.GetGridWidth(MinScreenWidth - 30);
            UltraWebGrid.Height = IsSmallResolution ? CRHelper.GetGridHeight(MinScreenHeight - 300) : CRHelper.GetGridHeight(MinScreenHeight - 195);

            DundasMap1.Width = CRHelper.GetChartWidth(MinScreenWidth - 25);
            DundasMap1.Height = CRHelper.GetChartHeight(MinScreenHeight * 0.78);

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);

            UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);


            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.33 - 55);
            UltraChart2.Height = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.23 - 55);

            #region Настройка диаграммы

            SetupRadarChart(UltraChart, true);
            UltraChart.DataBinding += new EventHandler(UltraChart_DataBinding);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            SetupRadarChart(UltraChart1, false);
            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);

            SetupRadarChart(UltraChart3, false);
            UltraChart3.DataBinding += new EventHandler(UltraChart3_DataBinding);
            UltraChart3.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart3_FillSceneGraph);

            SetupRadarChart(UltraChart4, false);
            UltraChart4.DataBinding += new EventHandler(UltraChart4_DataBinding);
            UltraChart4.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart4_FillSceneGraph);

            SetupRadarChart(UltraChart5, false);
            UltraChart5.DataBinding += new EventHandler(UltraChart5_DataBinding);
            UltraChart5.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart5_FillSceneGraph);

            SetupRadarChart(UltraChart6, false);
            UltraChart6.DataBinding += new EventHandler(UltraChart6_DataBinding);
            UltraChart6.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart6_FillSceneGraph);

            SetupRadarChart(UltraChart7, false);
            UltraChart7.DataBinding += new EventHandler(UltraChart7_DataBinding);
            UltraChart7.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart7_FillSceneGraph);

            SetupRadarChart(UltraChart8, false);
            UltraChart8.DataBinding += new EventHandler(UltraChart8_DataBinding);
            UltraChart8.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart8_FillSceneGraph);

            SetupRadarChart(UltraChart9, false);
            UltraChart9.DataBinding += new EventHandler(UltraChart9_DataBinding);
            UltraChart9.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart9_FillSceneGraph);

            SetupRadarChart(UltraChart10, false);
            UltraChart10.DataBinding += new EventHandler(UltraChart10_DataBinding);
            UltraChart10.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart10_FillSceneGraph);

            SetupLegendChart();

            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 10);
            #endregion
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            WebAsyncPanel.AddRefreshTarget(UltraWebGrid);
            WebAsyncPanel.AddLinkedRequestTrigger(detail.ClientID);

            if (!Page.IsPostBack)
            {
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.ShowSelectedValue = false;
                ComboYear.ParentSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(2009, 2010));
                ComboYear.SelectLastNode();
                ComboYear.PanelHeaderTitle = "Год";
            }

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;

            FillComboIndicators();

            if (!Page.IsPostBack)
            {
                detail.Checked = false;
                ComboIndicator.Title = "Показатель";
                ComboIndicator.Width = IsSmallResolution ? 1000 : 1000;
                ComboIndicator.SetСheckedState("Комплексная (интегральная) оценка эффективности ОИВ", true);
                ComboIndicator.ParentSelect = true;
                UserParams.Filter.Value = "Объем валового регионального продукта на одного жителя";
            }

            UserParams.Filter.Value = ComboIndicator.SelectedValue;

            SetupMap();

            Page.Title = "Оценка внедрения механизмов БОР";
            PageTitle.Text = "Оценка внедрения механизмов БОР";
            PageSubTitle.Text = String.Format("Оценка внедрения механизмов БОР и реструктуризации и оптимизации сети подведомственных учреждений в рамках № 83-ФЗ в {0} году", ComboYear.SelectedValue);

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
            UltraChart.DataBind();
            UltraChart1.DataBind();
            UltraChart2.DataBind();
            UltraChart3.DataBind();
            UltraChart4.DataBind();

            UltraChart5.DataBind();
            UltraChart6.DataBind();
            UltraChart7.DataBind();

            UltraChart8.DataBind();
            UltraChart9.DataBind();
            UltraChart10.DataBind();

            // заполняем карту формами
            string regionStr = "УрФО";
            DundasMap1.Shapes.Clear();
            DundasMap1.LoadFromShapeFile(Server.MapPath(string.Format("../../maps/{0}/{0}.shp", RegionsNamingHelper.ShortName(regionStr))), "NAME", true);
            // заполняем карту данными
            FillMapData(DundasMap1);
        }


        #region Настройка карты

        private void SetupMap()
        {
            DundasMap1.Meridians.Visible = false;
            DundasMap1.Parallels.Visible = false;
            DundasMap1.ZoomPanel.Visible = !IsSmallResolution;
            DundasMap1.ZoomPanel.Dock = PanelDockStyle.Right;
            DundasMap1.NavigationPanel.Visible = !IsSmallResolution;
            DundasMap1.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap1.Viewport.EnablePanning = true;

            // добавляем поля для раскраски
            DundasMap1.ShapeFields.Clear();
            DundasMap1.ShapeFields.Add("Name");
            DundasMap1.ShapeFields["Name"].Type = typeof(string);
            DundasMap1.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap1.ShapeFields.Add("IndicatorValue");
            DundasMap1.ShapeFields["IndicatorValue"].Type = typeof(double);
            DundasMap1.ShapeFields["IndicatorValue"].UniqueIdentifier = false;

            // добавляем легенду
            DundasMap1.Legends.Clear();
            Legend legend1 = new Legend("CompleteLegend");
            legend1.Visible = true;
            legend1.Dock = PanelDockStyle.Left;
            legend1.BackColor = Color.White;
            legend1.BackSecondaryColor = Color.Gainsboro;
            legend1.BackGradientType = GradientType.DiagonalLeft;
            legend1.BackHatchStyle = MapHatchStyle.None;
            legend1.BorderColor = Color.Gray;
            legend1.BorderWidth = 1;
            legend1.BorderStyle = MapDashStyle.Solid;
            legend1.BackShadowOffset = 4;
            legend1.TextColor = Color.Black;
            legend1.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend1.AutoFitText = true;
            //legend1.Size.Width = 500f;
            legend1.AutoSize = true;
            legend1.MaxAutoSize = 500f;
            legend1.AutoFitMinFontSize = 7;

            //string query = DataProvider.GetQueryText("RG_0001_0002_measures");

            DataTable dtMeasures = new DataTable();
           // DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtMeasures);

            bool direct = true;
            string ruleLegengText = "#FROMVALUE{N1} - #TOVALUE{N1}";
            string element = String.Empty;

            if (dtMeasures.Rows.Count > 0)
            {
                measure = dtMeasures.Rows[0][0].ToString().ToLower();
                direct = dtMeasures.Rows[0][2].ToString() == "0";

                switch (measure)
                {
                    case "процент":
                        {
                            measure = "%";
                            formatString = "N2";
                            ruleLegengText = "#FROMVALUE{N0}% - #TOVALUE{N0}%";
                            break;
                        }
                    case "рубль":
                        {
                            measure = "руб.";
                            if (ComboIndicator.SelectedValue != "Оборот розничной торговли")
                            {
                                formatString = "N2";
                            }
                            break;
                        }
                    case "место":
                        {
                            measure = "мест";
                            formatString = "N0";
                            break;
                        }
                    case "год":
                        {
                            measure = "лет";
                            formatString = "N2";
                            break;
                        }
                    case "на 1000 человек":
                        {
                            formatString = "N2";
                            break;
                        }
                    case "на 10000 человек":
                        {
                            formatString = "N2";
                            break;
                        }
                    case "раз (отношение)":
                        {
                            formatString = "N3";
                            ruleLegengText = "#FROMVALUE{N2} - #TOVALUE{N2}";
                            break;
                        }
                    case "раз":
                        {
                            formatString = "N3";
                            ruleLegengText = "#FROMVALUE{N2} - #TOVALUE{N2}";
                            break;
                        }
                    case "доля":
                        {
                            formatString = "N3";
                            ruleLegengText = "#FROMVALUE{N2} - #TOVALUE{N2}";
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }


                string prefix = "за ";
                string filter = String.Format("ID='{0}'", dtMeasures.Rows[0][3]);
                

                element = GetElement(dtMeasures.Rows[0][1].ToString(), prefix);
            }

            // добавляем правила раскраски
            DundasMap1.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "IndicatorValueRule";
            rule.Category = String.Empty;
            rule.ShapeField = "IndicatorValue";
            rule.DataGrouping = DataGrouping.EqualInterval;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = direct ? Color.Red : Color.Green;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = direct ? Color.Green : Color.Red;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";

            rule.LegendText = ruleLegengText;
            DundasMap1.ShapeRules.Add(rule);

            legend1.Title = String.Format("{0}", GetWrappedText(ComboIndicator.SelectedValue));
            DundasMap1.Legends.Add(legend1);

            string measureInfo = measure != String.Empty ? String.Format(", {0}", measure) : String.Empty;
            mapElementCaption.Text = String.Format("{0} {1}{2}", UserParams.Filter.Value, element, measureInfo);
            //mapElementCaption.Text = String.Format("{0} по субъектам УрФО", UserParams.Filter.Value);
        }

        private string GetWrappedText(string text)
        {
            string result = string.Empty;
            int k = 0;
            for (int i = 0; i < text.Length; i++)
            {
                k++;
                Char ch = text[i];
                if (k > 80 && ch == ' ')
                {
                    result += Environment.NewLine;
                    k = 0;
                }
                else
                {
                    result += ch;
                }
            }
            return result;
        }

        /// <summary>
        /// Поиск формы карты
        /// </summary>
        /// <param name="map">карта</param>
        /// <param name="patternValue">искомое имя формы</param>
        /// <returns>найденная форма</returns>
        public static Shape FindMapShape(MapControl map, string patternValue)
        {
            string subject = patternValue.Replace("область", "обл.");
            subject = subject.Replace("автономный округ", "АО");

            ArrayList shapeList = map.Shapes.Find(subject, true, false);
            if (shapeList.Count > 0)
            {
                return (Shape)shapeList[0];
            }
            return null;
        }

        public void FillMapData(MapControl map)
        {
            if (map == null)
            {
                return;
            }
            // из таблички сделаем словарь и заполним карту.
            // два словаря.
            string code = indicatorCodeDictionary[ComboIndicator.SelectedValue];

            DataRow row = dtGrid.Rows.Find(code);

            Dictionary<string, double> values = InitMarkDictionary();
            Dictionary<string, double> ranks = InitMarkDictionary();

            for (int columnsCount = 3; columnsCount < 20; columnsCount += 3)
            {
                string region = dtGrid.Columns[columnsCount].ColumnName.Split(';')[0];
                if (row[columnsCount] != DBNull.Value)
                {
                    double value = Convert.ToDouble(row[columnsCount]);
                    values[region] = value;
                }
                if (row[columnsCount + 1] != DBNull.Value)
                {
                    double value = Convert.ToDouble(row[columnsCount + 1]);
                    ranks[region] = value;
                }
            }

            foreach (KeyValuePair<string, double> pair in values)
            {
                FillMapFromDictionary(map, pair, ranks[pair.Key], ComboIndicator.SelectedValue);
            }
        }

        private void FillMapFromDictionary(MapControl map, KeyValuePair<string, double> pair, double rank, string name)
        {
            string regionName = pair.Key;

            if (RegionsNamingHelper.IsSubject(regionName))
            {
                Shape shape = FindMapShape(map, regionName);
                if (shape != null)
                {
                    double indicatorValue = pair.Value;

                    shape["Name"] = regionName;
                    shape["IndicatorValue"] = indicatorValue;
                    shape.ToolTip = string.Format("#NAME \nранг по УрФО {0} \n{1} {2}", rank, name, indicatorValue.ToString("N3"));
                    shape.TextAlignment = ContentAlignment.MiddleCenter;
                    shape.Offset.X = -15;
                    if (!IsSmallResolution)
                    {
                        shape.Offset.Y = -30;
                    }

                    shape.Text =
                        string.Format("{0}\n{1} {2}", IsSmallResolution ? RegionsNamingHelper.ShortName(regionName) : shape.Name, indicatorValue.ToString("N3"),
                                      measure.Replace(" (отношение)", String.Empty));

                    shape.BorderWidth = 2;
                    shape.TextColor = Color.Black;
                    shape.Font = new Font("Verdana", 10);
                    shape.TextVisibility = TextVisibility.Shown;
                }
            }
        }
        #endregion

        #region Обработчики грида

        private static Dictionary<string, double> FillRanksDictionary(Dictionary<string, double> dictionary)
        {
            Dictionary<string, double> rankValues = new Dictionary<string, double>();

            double maxDouble = Double.MinValue;
            string maxRegion = String.Empty;
            int rank = 1;
            // Каждый раз ищем максимум среди оставшихся
            for (int i = 0; i < dictionary.Keys.Count; i++)
            {
                foreach (KeyValuePair<string, double> value in dictionary)
                {
                    if (value.Value != 0 &&
                        value.Value > maxDouble &&
                        !rankValues.ContainsKey(value.Key))
                    {
                        maxDouble = value.Value;
                        maxRegion = value.Key;
                    }
                }
                if (maxRegion != String.Empty)
                {
                    rankValues.Add(maxRegion, rank);

                    foreach (KeyValuePair<string, double> value in dictionary)
                    {
                        if (value.Value != 0 &&
                            value.Value == maxDouble &&
                            !rankValues.ContainsKey(value.Key))
                        {                           
                            rankValues.Add(value.Key, rank);
                        }
                    }

                    rank++;
                }
                maxDouble = Double.MinValue;
                maxRegion = String.Empty;
            }

            return rankValues;
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("BOR_0001_0001_Grid"));
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            dtGrid.Columns.RemoveAt(0);

            Dictionary<string, double> complexValues = InitMarkDictionary();
            markValuesDictionary.Add("10000", complexValues);

            int groupRowIndex = 1;
            Dictionary<string, double> markValues = InitMarkDictionary();

            for (int rowCount = 2; rowCount < dtGrid.Rows.Count; rowCount++)
            {
                DataRow row = dtGrid.Rows[rowCount];
                if (row[29].ToString() != "Уровень 1")
                {
                    RecalculateMarksValues(markValues, rowCount);
                }
                else
                {
                    FillGroupRow(groupRowIndex, markValues);
                    markValuesDictionary.Add(dtGrid.Rows[groupRowIndex][24].ToString(), markValues);
                    // Переинициализируем значения
                    groupRowIndex = rowCount;
                    markValues = InitMarkDictionary();
                }
            }

            // Последний словарь остался незаполнен
            FillGroupRow(groupRowIndex, markValues);
            markValuesDictionary.Add(dtGrid.Rows[groupRowIndex][24].ToString(), markValues);

            FillGroupRow(0, complexValues);

            dtGrid.Columns.RemoveAt(29);
            dtGrid.PrimaryKey = new DataColumn[] { dtGrid.Columns[24] };
            dtGrid.AcceptChanges();
            ((UltraWebGrid)sender).DataSource = dtGrid;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            UltraGridColumn col = e.Layout.Grid.Columns[24];
            e.Layout.Grid.Columns.RemoveAt(24);
            e.Layout.Grid.Columns.Insert(0, col);

            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.FilterOptionsDefault.AllowRowFiltering = RowFiltering.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(40, 1280);
            e.Layout.Bands[0].Columns[0].Header.Caption = "Код";
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[0], "N0");
            e.Layout.Bands[0].Columns[1].Header.Caption = "Показатель";

            e.Layout.Bands[0].Columns[1].Width = IsSmallResolution ? CRHelper.GetColumnWidth(160, 1280) : CRHelper.GetColumnWidth(195, 1280);
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 2].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 3].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 4].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 5].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 6].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 7].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 8].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 9].Hidden = true;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 1 || i == e.Layout.Bands[0].Columns.Count)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 2;

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count - 9; i = i + 3)
            {
                ColumnHeader ch = new ColumnHeader(true);
                ch.Caption = IsSmallResolution ? RegionsNamingHelper.ShortName(e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0].Trim(' ')) : e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[1], "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, e.Layout.Bands[0].Columns[i + 1].Header.Caption.Split(';')[1], "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, e.Layout.Bands[0].Columns[i + 2].Header.Caption.Split(';')[1], "");

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                ch.RowLayoutColumnInfo.SpanY = 1;
                multiHeaderPos += 2;
                ch.RowLayoutColumnInfo.SpanX = 2;
                e.Layout.Bands[0].HeaderLayout.Add(ch);

                // CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "N0");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 2], "N3");

                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(65, 1280);
                e.Layout.Bands[0].Columns[i + 1].Hidden = true;
                e.Layout.Bands[0].Columns[i + 2].Width = CRHelper.GetColumnWidth(80, 1280);
            }
            //}
        }

        private int shownRows = 0;

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Value != null &&
                e.Row.Cells[0].Value.ToString() != string.Empty)
            {
                string code = e.Row.Cells[0].Value.ToString();

                string parent = code.Remove(code.Length - 4, 4);
                string child = code.Substring(code.Length - 4, 2).TrimStart('0');
                e.Row.Cells[0].Value = child.Length > 0 ? String.Format("{0}.{1}", parent, child) : parent;
            }

            if (e.Row.Cells[1].Value == null ||
                e.Row.Cells[1].Value.ToString() == string.Empty ||
                e.Row.Cells[1].Value.ToString().Contains("Индикаторы, характеризующие ") ||
                 e.Row.Cells[1].Value.ToString().Contains("Комплексная (интегральная) оценка "))
            {
                SetupAssessValues(e);
                return;
            }
            else
            {
                if (!detail.Checked)
                {
                    e.Row.Hidden = true;
                    return;
                }
            }


            string measureLocal = e.Row.Cells[e.Row.Cells.Count - 5].Value.ToString().ToLower();
            string formatStringLocal = "N2";
            switch (measureLocal)
            {
                case "процент":
                    {
                        measureLocal = "%";
                        formatStringLocal = "N2";
                        break;
                    }
                case "рубль":
                    {
                        formatStringLocal = "N2";
                        measureLocal = "руб.";
                        break;
                    }
                case "место":
                    {
                        measureLocal = "мест";
                        formatStringLocal = "N0";
                        break;
                    }
                case "год":
                    {
                        measureLocal = "лет";
                        break;
                    }
                case "на 1000 человек":
                    {
                        formatStringLocal = "N2";
                        break;
                    }
                case "раз (отношение)":
                    {
                        formatStringLocal = "N3";
                        break;
                    }
                case "раз":
                    {
                        formatStringLocal = "N3";
                        break;
                    }
                case "доля":
                    {
                        formatStringLocal = "N3";
                        break;
                    }
            }

            e.Row.Cells[1].Value = String.Format("{0}, {1}", e.Row.Cells[1].Value, measureLocal);
            for (int i = 2; i < e.Row.Cells.Count - 7; i += 3)
            {
                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(e.Row.Cells[i].Value.ToString(), out value))
                    {
                        if (value < 0 ||
                            (measureLocal == "%" && e.Row.Cells[0].Value.ToString().ToLower().Contains("темп") && value < 100))
                        {
                            e.Row.Cells[i].Style.ForeColor = Color.Red;
                        }
                    }

                    if ((Request.Form.AllKeys.Length == 0) ||
                        (Request.Form["__EVENTTARGET"] != null && !Request.Form["__EVENTTARGET"].Contains("excel")))
                    {
                        if (e.Row.Cells[i] != null && e.Row.Cells[i].Value != null && measureLocal == "неизвестные данные")
                        {
                            if (e.Row.Cells[i].Value.ToString().Contains("1"))
                            {
                                e.Row.Cells[i].Value = "Да";
                            }
                            else
                            {
                                e.Row.Cells[i].Value = "Нет";

                            }
                        }
                        else
                        {
                            e.Row.Cells[i].Value = value.ToString(formatStringLocal);
                        }
                        if (e.Row.Cells[i + 2] != null && e.Row.Cells[i + 2].Value != null)
                        {
                            decimal integralValue;
                            if (decimal.TryParse(e.Row.Cells[i + 2].Value.ToString(), out integralValue))
                            {
                                e.Row.Cells[i + 2].Value = String.Format("{0}<br/>ранг {1}", integralValue.ToString("N3"), e.Row.Cells[i + 1].Value);
                            }
                        }
                        else
                        {
                            e.Row.Cells[i + 2].Value = String.Format("ранг {0}", e.Row.Cells[i + 1].Value);
                        }
                    }

                    if (e.Row.Cells[i + 1].Value != null && e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                    {
                        if (Convert.ToInt32(e.Row.Cells[i + 1].Value) == 1)
                        {
                            e.Row.Cells[i + 2].Style.BackgroundImage = "~/images/starYellowBB.png";
                        }
                        else if (e.Row.Cells[e.Row.Cells.Count - 6].Value != null && e.Row.Cells[e.Row.Cells.Count - 6].Value.ToString() != string.Empty
                            && Convert.ToInt32(e.Row.Cells[e.Row.Cells.Count - 6].Value) == Convert.ToInt32(e.Row.Cells[i + 1].Value))
                        {
                            e.Row.Cells[i + 2].Style.BackgroundImage = "~/images/starGrayBB.png";
                        }
                    }
                    e.Row.Cells[i + 2].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    e.Row.Cells[i + 1].Style.HorizontalAlign = HorizontalAlign.Right;
                }

                e.Row.Cells[i].Style.BorderDetails.WidthLeft = 2;
                e.Row.Cells[i + 2].Style.BorderDetails.WidthRight = 2;
            }
            e.Row.Cells[1].Style.BorderDetails.WidthRight = 2;
            shownRows++;

            
        }

        private void SetupAssessValues(RowEventArgs e)
        {
            int worseRank = 0;
            // Сначала пробежим по всем, посмотрим худший ранг, подчистим лишнее.
            decimal integralValue;
            for (int i = 2; i < e.Row.Cells.Count - 7; i += 3)
            {
                if (e.Row.Cells[i + 1] != null && e.Row.Cells[i + 1].Value != null && 
                    worseRank < Convert.ToInt32(e.Row.Cells[i + 1].Value))
                {
                    worseRank = Convert.ToInt32(e.Row.Cells[i + 1].Value);
                }
            }

            for (int i = 2; i < e.Row.Cells.Count - 7; i += 3)
            {
                if (e.Row.Cells[i + 1].Value != null && e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                {
                    if (Convert.ToInt32(e.Row.Cells[i + 1].Value) == 1)
                    {
                        e.Row.Cells[i + 2].Style.BackgroundImage = "~/images/starYellowBB.png";
                    }
                    else if (worseRank == Convert.ToInt32(e.Row.Cells[i + 1].Value) && worseRank > 1)
                    {
                        e.Row.Cells[i + 2].Style.BackgroundImage = "~/images/starGrayBB.png";
                    }
                }

                e.Row.Cells[i + 2].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                e.Row.Cells[i + 1].Style.HorizontalAlign = HorizontalAlign.Right;

                if ((Request.Form.AllKeys.Length == 0) ||
                    (Request.Form["__EVENTTARGET"] != null && !Request.Form["__EVENTTARGET"].Contains("excel")))
                {
                    if (e.Row.Cells[i + 2] != null && e.Row.Cells[i + 2].Value != null && decimal.TryParse(e.Row.Cells[i + 2].Value.ToString(), out integralValue) && integralValue != 0)
                    {
                        e.Row.Cells[i + 2].Value = String.Format("{0}<br/>ранг {1}", integralValue.ToString("N3"), e.Row.Cells[i + 1].Value);
                    }
                    else
                    {
                        e.Row.Cells[i + 2].Value = String.Empty;
                    }
                }

                e.Row.Cells[i].Style.BorderDetails.WidthLeft = 2;
                e.Row.Cells[i + 2].Style.BorderDetails.WidthRight = 2;
                e.Row.Cells[i + 2].Style.Font.Bold = true;
            }

            e.Row.Cells[0].Style.Font.Bold = true;
            e.Row.Cells[1].Style.Font.Bold = true;

            e.Row.Cells[1].Style.BorderDetails.WidthRight = 2;
        }

        private static string GetElement(string period, string prefix)
        {
            string element;
            string[] periodDimension = period.ToString().Split('.');
            int value;
            if (periodDimension[periodDimension.Length - 1].Length < 5 &&
                Int32.TryParse(periodDimension[periodDimension.Length - 1].Trim('[').Trim(']'), out value))
            {
                element = String.Format("{1}{0:dd.MM.yyyy}", new DateTime(Convert.ToInt32(periodDimension[3].Trim('[').Trim(']')), CRHelper.MonthNum(periodDimension[6].Trim('[').Trim(']')), value), prefix);
            }
            else if (period.Contains("квартала"))
            {
                if (prefix == "за 1-" && periodDimension[5].Contains("1"))
                {
                    prefix = "за ";
                }
                element = String.Format("{2}{0} квартал {1} года", periodDimension[5].Trim('[').Trim(']').Replace("Квартал", String.Empty), periodDimension[3].Trim('[').Trim(']'), prefix);
            }
            else if (period.Contains("Квартал"))
            {
                if (prefix == "за январь-" && periodDimension[6].Contains("Январь"))
                {
                    prefix = "за ";
                }
                element = String.Format("{2}{0} {1} года", periodDimension[6].Trim('[').Trim(']').ToLower(), periodDimension[3].Trim('[').Trim(']'), prefix);
            }
            else
            {
                element = String.Format("{1}{0} год", periodDimension[3].Trim('[').Trim(']'), prefix);
            }
            return element;
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (shownRows < 18)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int width = 100;
            e.CurrentWorksheet.Columns[0].Width = 50 * 37;
            e.CurrentWorksheet.Columns[1].Width = 250 * 37;
            e.CurrentWorksheet.Columns[2].Width = width * 37;
            e.CurrentWorksheet.Columns[3].Width = width * 37;
            e.CurrentWorksheet.Columns[4].Width = width * 37;
            e.CurrentWorksheet.Columns[5].Width = width * 37;
            e.CurrentWorksheet.Columns[6].Width = width * 37;
            e.CurrentWorksheet.Columns[7].Width = width * 37;
            e.CurrentWorksheet.Columns[8].Width = width * 37;
            e.CurrentWorksheet.Columns[9].Width = width * 37;
            e.CurrentWorksheet.Columns[10].Width = width * 37;
            e.CurrentWorksheet.Columns[11].Width = width * 37;
            e.CurrentWorksheet.Columns[12].Width = width * 37;
            e.CurrentWorksheet.Columns[13].Width = width * 37;
            e.CurrentWorksheet.Columns[14].Width = width * 37;

            e.CurrentWorksheet.Columns[0].CellFormat.FormatString = "0";
            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "";
            //e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            //e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            //e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            //e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            //e.CurrentWorksheet.Columns[6].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            //e.CurrentWorksheet.Columns[7].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[8].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[9].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[10].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[11].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[12].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[13].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[14].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[15].CellFormat.FormatString = "#,##0.00";

            int columnCount = UltraWebGrid.Columns.Count;
            int rowsCount = shownRows + 5;

            // расставляем стили у начальных колонок
            for (int i = 4; i < rowsCount; i++)
            {
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Center;

                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Height = 20 * 37;
            }

            // расставляем стили у ячеек
            for (int i = 2; i < columnCount; i += 2)
            {
                for (int j = 4; j < rowsCount; j++)
                {
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.FormatString = "#,##0.####;[Red]-#,##0.####";
                    double value;
                    if (e.CurrentWorksheet.Rows[j].Cells[i].Value != null && Double.TryParse(e.CurrentWorksheet.Rows[j].Cells[i].Value.ToString(), out value))
                    {
                        e.CurrentWorksheet.Rows[j].Cells[i].Value = value;
                    }

                    e.CurrentWorksheet.Rows[j].Cells[i + 1].CellFormat.Alignment = HorizontalCellAlignment.Right;
                    e.CurrentWorksheet.Rows[j].Cells[i + 1].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                    e.CurrentWorksheet.Rows[j].Cells[i + 1].CellFormat.FormatString = "#,##0";

                    int value1;
                    if (e.CurrentWorksheet.Rows[j].Cells[i + 1].Value != null && Int32.TryParse(e.CurrentWorksheet.Rows[j].Cells[i + 1].Value.ToString(), out value1))
                    {
                        e.CurrentWorksheet.Rows[j].Cells[i + 1].Value = value1;
                    }

                    //e.CurrentWorksheet.Workbook.Styles.
                }
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            foreach (UltraGridRow row in UltraWebGrid.Rows)
            {
                if (row.Cells[1] != null)
                {
                    row.Cells[1].Value = row.Cells[1].Value.ToString().Replace("<a href='../../reports/MFRF_0003_0003/Default.aspx'>>>></a>", string.Empty);
                }
            }

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            foreach (UltraGridRow row in UltraWebGrid.Rows)
            {
                if (row.Cells[1] != null)
                {
                    row.Cells[1].Value = row.Cells[1].Value.ToString().Replace("<a href='../../reports/MFRF_0003_0003/Default.aspx'>>>></a>", string.Empty);
                    for (int i = 0; i < row.Cells.Count; i++)
                    {
                        if (row.Cells[i].Value != null)
                        {
                            row.Cells[i].Value = row.Cells[i].Value.ToString().Replace("<br/>", "\n");
                        }
                    }
                }
            }

            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
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
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            e.Section.AddPageBreak();

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(mapElementCaption.Text);

            Infragistics.Documents.Reports.Graphics.Image img1 = UltraGridExporter.GetImageFromMap(DundasMap1);
            e.Section.AddImage(img1);
        }

        #endregion

        #region Сюда не смотрим

        private Dictionary<string, string> indicatorCodeDictionary = new Dictionary<string, string>();

        private void FillComboIndicators()
        {
            DataTable dtIndicators = new DataTable();
            string query = DataProvider.GetQueryText("RG_0001_0002_Indicators");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Район", dtIndicators);

            dtIndicators.Columns.RemoveAt(0);

            Dictionary<string, int> indicators = new Dictionary<string, int>();

            for (int i = 0; i < dtIndicators.Rows.Count; i++)
            {
                int level = dtIndicators.Rows[i]["Уровень"].ToString() == "Уровень 1" ? 0 : 1;

                indicators.Add(dtIndicators.Rows[i]["Показатель"].ToString(), level);

                indicatorCodeDictionary.Add(dtIndicators.Rows[i]["Показатель"].ToString(), dtIndicators.Rows[i]["Код"].ToString());
            }
            if (!Page.IsPostBack)
            {
                ComboIndicator.FillDictionaryValues(indicators);
            }
        }

        private static Dictionary<string, double> InitMarkDictionary()
        {
            Dictionary<string, double> markValues = new Dictionary<string, double>();

            foreach (string regionName in regions)
            {
                markValues.Add(regionName, 0);
            }

            return markValues;
        }

        private void SetupLegendChart()
        {
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
            UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);
            UltraChart2.Legend.Visible = true;
            AddLineAppearencesUltraChart(UltraChart2);

            UltraChart2.Legend.Location = LegendLocation.Bottom;
            UltraChart2.Legend.SpanPercentage = 100;
            UltraChart2.Legend.Font = new Font("Verdana", 10);
            UltraChart2.Border.Thickness = 0;

            UltraChart2.Tooltips.Display = TooltipDisplay.Never;

            UltraChart2.Tooltips.Font.Name = "Courier New";
            UltraChart2.Tooltips.Font.Bold = true;

            UltraChart2.Axis.X.Labels.FontColor = Color.Transparent;
            UltraChart2.Axis.X.Labels.SeriesLabels.FontColor = Color.Transparent;
            UltraChart2.Axis.X.StripLines.PE.Fill = Color.Transparent;
            UltraChart2.Axis.Y.StripLines.PE.Fill = Color.Transparent;
            UltraChart2.Axis.X.LineColor = Color.Transparent;
            UltraChart2.Axis.Y.LineColor = Color.Transparent;
            UltraChart2.Axis.Y.MajorGridLines.Visible = false;
            UltraChart2.Axis.Y.MinorGridLines.Visible = false;

            UltraChart2.Axis.X.MajorGridLines.Visible = false;
            UltraChart2.Axis.X.MinorGridLines.Visible = false;
        }

        private void SetupRadarChart(UltraChart chart, bool main)
        {
            int minusValue = main ? -50 : 50;
            chart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.33 - minusValue);
            chart.Height = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.33 - minusValue);

            chart.ChartType = ChartType.RadarChart;
            chart.Data.SwapRowsAndColumns = true;
            chart.Data.RowLabelsColumn = 0;
            chart.Data.UseRowLabelsColumn = false;

            chart.RadarChart.ColorFill = true;
            chart.RadarChart.LineThickness = 3;
            chart.RadarChart.LineEndCapStyle = LineCapStyle.Round;
            chart.RadarChart.NullHandling = NullHandling.Zero;

            chart.Tooltips.FormatString = "";
            chart.Tooltips.Display = TooltipDisplay.Never;

            chart.Axis.Y.LineThickness = 1;
            chart.Axis.Y.LineDrawStyle = LineDrawStyle.Solid;
            chart.Axis.Y.LineColor = Color.Transparent;
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            chart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            chart.Legend.Visible = false;
            AddLineAppearencesUltraChart(chart);
        }

        private static Collection<string> regions = InitRegionsCollection();

        private static Collection<string> InitRegionsCollection()
        {
            Collection<string> regionsCollection = new Collection<string>();

            regionsCollection.Add("Курганская область");
            regionsCollection.Add("Свердловская область");
            regionsCollection.Add("Тюменская область");
            regionsCollection.Add("Ханты-Мансийский автономный округ");
            regionsCollection.Add("Челябинская область");
            regionsCollection.Add("Ямало-Ненецкий автономный округ");

            return regionsCollection;
        }

        private void AddToComplexValues(int groupRowIndex, Dictionary<string, double> markValues)
        {
            double weigh = Convert.ToDouble(dtGrid.Rows[groupRowIndex][27]);
            {
                foreach (string regionName in regions)
                {
                    markValuesDictionary["10000"][regionName] += markValues[regionName] * weigh;
                }
            }
        }

        private void FillGroupRow(int groupRowIndex, Dictionary<string, double> markValues)
        {
            DataRow groupRow = dtGrid.Rows[groupRowIndex];
            Dictionary<string, double> ranksDictionary = FillRanksDictionary(markValues);
            if (groupRow[27] != DBNull.Value)
            {
                AddToComplexValues(groupRowIndex, markValues);
            }
            for (int columnsCount = 3; columnsCount < 20; columnsCount += 3)
            {
                groupRow[columnsCount] = markValues[groupRow.Table.Columns[columnsCount].ColumnName.Split(';')[0].Trim()];
                if (ranksDictionary.ContainsKey(groupRow.Table.Columns[columnsCount - 1].ColumnName.Split(';')[0].Trim()))
                {
                    groupRow[columnsCount - 1] =
                        ranksDictionary[groupRow.Table.Columns[columnsCount - 1].ColumnName.Split(';')[0].Trim()];
                }
            }
        }

        private void RecalculateMarksValues(Dictionary<string, double> markValues, int rowCount)
        {
            if (dtGrid.Rows[rowCount][27] != DBNull.Value)
            {
                double weigh = Convert.ToDouble(dtGrid.Rows[rowCount][27]);
                for (int columnsCount = 3; columnsCount < 20; columnsCount += 3)
                {
                    string region = dtGrid.Columns[columnsCount].ColumnName.Split(';')[0];
                    if (dtGrid.Rows[rowCount][columnsCount] != DBNull.Value)
                    {
                        double value = Convert.ToDouble(dtGrid.Rows[rowCount][columnsCount]);
                        markValues[region] = markValues[region] + value * weigh;
                    }
                }
            }
        }

        private Dictionary<string, Dictionary<string, double>> markValuesDictionary =
                        new Dictionary<string, Dictionary<string, double>>();

        private void AddLineAppearencesUltraChart(UltraChart chart)
        {
            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.ApplyRowWise = true;
            chart.ColorModel.Skin.PEs.Clear();

            for (int i = 1; i <= 7; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = GetColor(i);
                pe.Fill = color;
                pe.Stroke = color;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.StrokeWidth = 2;
                chart.ColorModel.Skin.PEs.Add(pe);
                chart.Border.Thickness = 0;
            }
        }

        private static Color GetColor(int i)
        {
            Color color = Color.White;
            switch (i)
            {
                case 1:
                    {
                        // Курганская
                        color = Color.Green;
                        break;
                    }
                case 2:
                    {
                        // Свердловска
                        color = Color.Gold;
                        break;
                    }
                case 3:
                    {
                        // Тюменская
                        color = Color.Black;
                        break;
                    }
                case 4:
                    {
                        // ХМАО
                        color = Color.LightSlateGray;
                        break;
                    }
                case 5:
                    {
                        // Челябинская
                        color = Color.Red;
                        break;
                    }
                case 6:
                    {
                        // ЯНАО
                        color = Color.Blue;
                        break;
                    }
                case 7:
                    {
                        color = Color.DarkViolet;
                        break;
                    }
            }
            return color;
        }

        private DataTable GetChartDataSource(string key)
        {
            DataTable dtChart2 = CreateDataTable();

            int j = 1;
            for (int i = 0; i < dtChart2.Rows.Count; i += 2)
            {
                dtChart2.Rows[i][j] = markValuesDictionary[key][RegionsNamingHelper.FullName(dtChart2.Rows[i][0].ToString())];
                dtChart2.Rows[i + 1][j] = markValuesDictionary[key][RegionsNamingHelper.FullName(dtChart2.Rows[i][0].ToString())];
                j++;
            }

            dtChart2.AcceptChanges();
            return dtChart2;
        }



        private static DataTable CreateDataTable()
        {
            DataTable dtChart2 = new DataTable();

            dtChart2.Columns.Add(new DataColumn("Субъект", typeof(string)));
            for (int i = 1; i <= 6; i++)
            {
                dtChart2.Columns.Add(new DataColumn(i.ToString(), typeof(double)));
            }

            foreach (string regionName in regions)
            {
                DataRow row = dtChart2.NewRow();
                row[0] = RegionsNamingHelper.ShortName(regionName);
                dtChart2.Rows.Add(row);
                row = dtChart2.NewRow();
                row[0] = RegionsNamingHelper.ShortName(regionName);
                dtChart2.Rows.Add(row);
            }

            return dtChart2;
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            DataTable dtChart3 = new DataTable();

            dtChart3.Columns.Add(new DataColumn("Субъект", typeof(string)));
            dtChart3.Columns.Add(new DataColumn("Комплексная (интегральная) оценка эффективности ОИВ", typeof(double)));
            dtChart3.Columns.Add(new DataColumn(" ", typeof(double)));

            foreach (string regionName in regions)
            {
                DataRow row = dtChart3.NewRow();
                row[0] = regionName;
                dtChart3.Rows.Add(row);
            }

            for (int i = 0; i < dtChart3.Rows.Count; i++)
            {
                dtChart3.Rows[i][1] = markValuesDictionary["10000"][dtChart3.Rows[i][0].ToString()];
                dtChart3.Rows[i][2] = markValuesDictionary["10000"][dtChart3.Rows[i][0].ToString()];
            }

            dtChart3.AcceptChanges();
            UltraChart2.DataSource = dtChart3;
        }

        protected void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive.Path != null &&
                    !primitive.Path.Contains("Legend"))
                {
                    primitive.Visible = false;
                }
            }
        }

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            UltraChart.DataSource = GetChartDataSource("10000");
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            UltraChart1.DataSource = GetChartDataSource("20000");
        }

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            UltraChart3.DataSource = GetChartDataSource("30000");
        }

        protected void UltraChart4_DataBinding(object sender, EventArgs e)
        {
            UltraChart4.DataSource = GetChartDataSource("40000");
        }

        protected void UltraChart5_DataBinding(object sender, EventArgs e)
        {
            UltraChart5.DataSource = GetChartDataSource("50000");
        }

        protected void UltraChart6_DataBinding(object sender, EventArgs e)
        {
            UltraChart6.DataSource = GetChartDataSource("60000");
        }

        protected void UltraChart7_DataBinding(object sender, EventArgs e)
        {
            UltraChart7.DataSource = GetChartDataSource("70000");
        }

        protected void UltraChart8_DataBinding(object sender, EventArgs e)
        {
            UltraChart8.DataSource = GetChartDataSource("80000");
        }

        protected void UltraChart9_DataBinding(object sender, EventArgs e)
        {
            UltraChart9.DataSource = GetChartDataSource("90000");
        }

        protected void UltraChart10_DataBinding(object sender, EventArgs e)
        {
            UltraChart10.DataSource = GetChartDataSource("100000");
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            RedrawChart(e, markValuesDictionary["10000"]);
        }

        protected void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            RedrawChart(e, markValuesDictionary["20000"]);
        }

        protected void UltraChart3_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            RedrawChart(e, markValuesDictionary["30000"]);
        }

        protected void UltraChart4_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            RedrawChart(e, markValuesDictionary["40000"]);
        }

        protected void UltraChart5_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            RedrawChart(e, markValuesDictionary["50000"]);
        }

        protected void UltraChart6_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            RedrawChart(e, markValuesDictionary["60000"]);
        }

        protected void UltraChart7_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            RedrawChart(e, markValuesDictionary["70000"]);
        }

        protected void UltraChart8_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            RedrawChart(e, markValuesDictionary["80000"]);
        }

        protected void UltraChart9_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            RedrawChart(e, markValuesDictionary["90000"]);
        }

        protected void UltraChart10_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            RedrawChart(e, markValuesDictionary["100000"]);
        }

        private static void RedrawChart(FillSceneGraphEventArgs e, Dictionary<string, double> marks)
        {
            Dictionary<string, double> x = InitMarkDictionary();
            Dictionary<string, double> y = InitMarkDictionary();

            int l = 1;
            bool q = false;
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Polygon)
                {
                    if (q)
                    {
                        primitive.PE.Fill = GetColor(l);
                        primitive.PE.FillStopColor = GetColor(l);
                        l++;
                    }
                    q = !q;
                }
                else if (primitive is Text)
                {
                    Text text = (Text)primitive;
                    string key = RegionsNamingHelper.FullName(text.GetTextString());
                    if (x.ContainsKey(key))
                    {
                        if (x[key] == 0)
                        {
                            x[key] = text.bounds.X;
                            y[key] = text.bounds.Y;
                            text.Visible = false;
                        }
                        else
                        {
                            text.bounds.X = (int)Math.Abs((x[key] + text.bounds.X) / 2);
                            text.bounds.Y = (int)Math.Abs((y[key] + text.bounds.Y) / 2);
                            text.SetTextString(text.GetTextString() + "\n" + marks[key].ToString("N3"));
                            text.labelStyle.VerticalAlign = StringAlignment.Near;
                            text.labelStyle.HorizontalAlign = StringAlignment.Near;
                            text.bounds.Width = 60;
                            text.bounds.Height = 55;
                            text.labelStyle.FontSizeBestFit = false;
                            text.labelStyle.Font = new Font("Verdana", 10);
                            text.PE.Fill = Color.Black;
                        }
                    }
                    else
                    {
                        text.Visible = false;
                    }
                }
            }
        }

        #endregion

        #endregion

    }
}
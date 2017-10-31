using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Image = Infragistics.Documents.Reports.Graphics.Image;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Documents.Excel;

namespace Krista.FM.Server.Dashboards.reports.MFRF_0002_0003
{
    public partial class Default : CustomReportPage
    {
        #region Поля и свойства

        private DataTable dtGrid;
        private DataTable dtComments;
        private static MemberAttributesDigest periodDigest;
        private int serial = 0;
        private bool CondittF = false;

        /// <summary>
        /// Выбраны ли 
        /// федеральные округа
        /// </summary>
        public bool AllFO
        {
            get { return ComboFO.SelectedIndex == 0; }
        }

        #endregion

        #region Параметры запроса

        // выбранный период
        private CustomParam selectedPeriod;
        // выбранный вариант
        private CustomParam bkkuVariant;
        // выбранный индикатор
        private CustomParam selectedIndicator;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = Unit.Empty;
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.5);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.7);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 550);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.73);

            gridTd.Style.Add("Height", String.Format("{0}px", CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.73 + 10)));
            chartTd.Style.Add("Height", String.Format("{0}px", CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.73 + 10)));

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.LineChart;
            UltraChart.Border.Thickness = 0;

            UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL> <DATA_VALUE:N4>";
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.Axis.X.Extent = 160;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N4>";
            //UltraChart.Legend.Margins.Right = Convert.ToInt32(UltraChart.Width.Value) / 2;
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 7;
            UltraChart.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Border.Thickness = 0;
            UltraChart.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            //UltraChart.Legend.FormatString.
            UltraChart.Axis.X.Margin.Near.Value = 2;
            UltraChart.Axis.Y.Margin.Near.Value = 2;
            //UltraChart.Axis.X.

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart.Height.Value) - UltraChart.Axis.X.Extent;

            EmptyAppearance item = new EmptyAppearance();
            item.EnableLineStyle = true;
            item.EnablePoint = false;
            LineStyle style = new LineStyle();
            style.DrawStyle = LineDrawStyle.Dash;
            style.MidPointAnchors = false;
            item.LineStyle = style;
            UltraChart.LineChart.EmptyStyles.Add(item);
            UltraChart.LineChart.NullHandling = NullHandling.InterpolateCustom;

            UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;

            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InterpolateValues += new InterpolateValuesEventHandler(UltraChart_InterpolateValues);

            LineAppearance lineAppearance3 = new LineAppearance();
            lineAppearance3.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance3.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance3.Thickness = 3;
            UltraChart.LineChart.LineAppearances.Add(lineAppearance3);
           // UltraChart.Tooltips.FormatString = "<ITEM_LABEL>";

            LineAppearance lineAppearance1 = new LineAppearance();
            
            lineAppearance1.Thickness = 0;
            UltraChart.LineChart.LineAppearances.Add(lineAppearance1);

            LineAppearance lineAppearance2 = new LineAppearance();
            lineAppearance2.IconAppearance.Icon = SymbolIcon.Circle;            
            lineAppearance2.Thickness = 0;
            UltraChart.LineChart.LineAppearances.Add(lineAppearance2);

            #endregion

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<EndExportEventArgs>(PdfExporter_EndExport);

            CrossLink1.Text = "Мониторинг&nbsp;соблюдения&nbsp;требований&nbsp;БК&nbsp;и&nbsp;КУ";
            CrossLink1.NavigateUrl = "~/reports/MFRF_0002_0002/Default.aspx";
            CrossLink2.Text = "Динамика&nbsp;индикаторов&nbsp;БК&nbsp;И&nbsp;КУ";
            CrossLink2.NavigateUrl = "~/reports/MFRF_0002_0004/Default.aspx";

            selectedPeriod = UserParams.CustomParam("selected_period");
            bkkuVariant = UserParams.CustomParam("bkku_Variant");
            selectedIndicator = UserParams.CustomParam("selected_indicator");
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            periodDigest = new MemberAttributesDigest(DataProvidersFactory.SecondaryMASDataProvider, "MFRF_0002_0003_periodDigest");

            if (!Page.IsPostBack)
            {
                ComboIndicator.Title = "Индикатор";
                ComboIndicator.Width = 300;
                ComboIndicator.MultiSelect = false;
                ComboIndicator.TooltipVisibility = TooltipVisibilityMode.Shown;
                ComboIndicator.FillDictionaryValues(CustomMultiComboDataHelper.FillIndicators(DataDictionariesHelper.IndicatorsTypes));
                ComboIndicator.SelectLastNode();
                ComboIndicator.SetСheckedState("Доп1(БК01) Отношение объема заимствований субъекта РФ в текущем финансовом году к сумме, направляемой в текущем финансовом году на финансирование дефицита бюджета и (или) погашение долговых обязательств бюджета субъекта РФ(0-соотв.,1-не соотв.)", true);

                ComboYear.Title = "Период";
                ComboYear.Width = 240;
                ComboYear.MultiSelect = false;
                ComboYear.ParentSelect = true;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodDigest.UniqueNames, periodDigest.MemberLevels));
                ComboYear.SelectLastNode();

                ComboFO.Title = "ФО";
                ComboFO.Width = 300;
                ComboFO.MultiSelect = false;
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));
                    
                if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                   
                   ComboFO.SetСheckedState(RegionsNamingHelper.GetFoBySubject(RegionSettings.Instance.Name), true);
                   
                }
            }
            ComboFO.RemoveTreeNodeByName("Все федеральные округа");

            Page.Title = string.Format("Отдельные индикаторы БК и КУ ({0})", ComboFO.SelectedValue.Replace("Все федеральные округа", "Российская Федерация").Replace("федеральный округ", "ФО"));
            Label1.Text = Page.Title;

            selectedPeriod.Value = periodDigest.GetMemberUniqueName(ComboYear.SelectedValue);
            selectedIndicator.Value = DataDictionariesHelper.IndicatorsTypes[ComboIndicator.SelectedValue];
            bkkuVariant.Value = ComboYear.SelectedValue.Contains("квартал") ? "По данным месячной отчетности" : "По данным годовой отчетности";

            Label2.Text = string.Format("{2}<br/>Данные Минфина РФ за {0} ({1})", ComboYear.SelectedValue, bkkuVariant.Value.ToLower(), ComboIndicator.SelectedValue);
            if (ComboFO.SelectedValue == "Все федеральные округа")
            {
                UserParams.Filter.Value = String.Empty;
                UserParams.SelectItem.Value = "Федеральный округ";
            }
            else
            {
                UserParams.Filter.Value = String.Format(
                       " and ( [Территории].[Сопоставимый].CurrentMember.Parent is [Территории].[Сопоставимый].[Все территории].[Российская  Федерация].[{0}])",
                       ComboFO.SelectedValue);
                UserParams.SelectItem.Value = "Субъект РФ";
            }

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
            UltraChart.DataBind();

            string patternValue = UserParams.StateArea.Value;
            int defaultRowIndex = 1;
            if (patternValue == string.Empty)
            {
                patternValue = UserParams.StateArea.Value;
                defaultRowIndex = 0;
            }

            UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
            ActiveGridRow(row);

            // заполняем карту формами
            string regionStr = (ComboFO.SelectedIndex == 0) ? "Дальневосточный федеральный округ" : ComboFO.SelectedValue;
            DundasMap.Shapes.Clear();

            DundasMap.LoadFromShapeFile(Server.MapPath(string.Format("../../maps/{0}/{0}.shp", RegionsNamingHelper.ShortName(regionStr))), "NAME", true);

            SetupMap();
            // заполняем карту данными
            FillMapData();
        }

        #region Обработчики карты

        /// <summary>
        /// Поиск формы карты
        /// </summary>
        /// <param name="map">карта</param>
        /// <param name="patternValue">искомое имя формы</param>
        /// <param name="searchFO">true, если ищем ФО</param>
        /// <returns>найденная форма</returns>
        public static Shape FindMapShape(MapControl map, string patternValue, bool searchFO)
        {
            string subject = string.Empty;
            bool isRepublic = patternValue.Contains("Республика");
            bool isTown = patternValue.Contains("г.");
            string[] subjects = patternValue.Split(' ');
            if (subjects.Length > 1)
            {
                // пока такой глупый способ сопоставления имен субъектов
                switch (subjects[0])
                {
                    case "Чеченская":
                        {
                            subject = "Чечня";
                            break;
                        }
                    case "Карачаево-Черкесская":
                        {
                            subject = "Карачаево-Черкессия";
                            break;
                        }
                    case "Кабардино-Балкарская":
                        {
                            subject = "Кабардино-Балкария";
                            break;
                        }
                    case "Удмуртская":
                        {
                            subject = "Удмуртия";
                            break;
                        }
                    case "Чувашская":
                        {
                            subject = "Чувашия";
                            break;
                        }
                    default:
                        {
                            subject = (isRepublic || isTown) ? subjects[1] : subjects[0];
                            break;
                        }
                }
            }

            ArrayList shapeList = map.Shapes.Find(subject, true, false);
            if (shapeList.Count > 0)
            {
                return (Shape)shapeList[0];
            }
            return null;
        }

        public void FillMapData()
        {
            if (dtGrid == null || DundasMap == null)
            {
                return;
            }

            string query = DataProvider.GetQueryText("MFRF_0002_0003_map");
            DataTable dtMap = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtMap);

            query = DataProvider.GetQueryText("MFRF_0002_0003_map_hint");
            DataTable dtMapHint = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtMapHint);

            foreach (DataRow row in dtMap.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty)
                {
                    string subject = row[0].ToString();

                    Shape shape = FindMapShape(DundasMap, subject, AllFO);
                    if (shape != null)
                    {
                        shape["Name"] = subject;
                        //shape["CrimesBK"] = Convert.ToDouble(row[1]);
                        //shape["CrimesKU"] = Convert.ToDouble(row[2]);
                        shape.ToolTip = string.Format("#NAME");
                        shape.TextVisibility = TextVisibility.Shown;
                        shape.TextAlignment = ContentAlignment.MiddleCenter;
                        shape.Text = String.Format("{0}", shape.Name);

                        if (row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                        {
                            // shape.Text = String.Format("{0} \nБК: {1:N0}", shape.Text, (row[1]));
                            shape.ToolTip = String.Format("{0}", shape.ToolTip, (row[1]));
                            //shape.Text = String.Format((row[1]));
                            double indicatorValue = Convert.ToDouble(row[1]);
                            shape["IndicatorValue"] = indicatorValue;
                        }


                        foreach (DataRow hintRow in dtMapHint.Rows)
                        {
                            //Shape shape1 = FindMapShape(DundasMap, subject, AllFO);
                    
                    
                            if (hintRow[0].ToString() == subject)
                            {
                                string Condition = "";

                                if (Convert.ToInt32(row[1]) == 1)
                                {
                                    Condition = "Не соблюдается условие";
                                    if (shape != null) 
                                    { 
                                        shape.Color = Color.Red; 
                                    }
                                }
                                else
                                {
                                    Condition = "Соблюдается условие";
                                    if (shape != null) 
                                    { 
                                        shape.Color = Color.Green;
                                        CondittF = true;

                                    }
                                }
                                shape.ToolTip = String.Format("{0}\n{1}\n{2:N4}\n{3}", shape.Text, ComboYear.SelectedValue, hintRow[1], Condition);
                                shape.Text = shape.Text +String.Format("\n{0:N4}",hintRow[1]);
                                
                                //String.Format("{0} \n({1})", shape.ToolTip, hintRow[1]);


                            }
                        }
                    }
                }

            }
        }

        #endregion

        #region Настройка карты

        private void SetupMap()
        {
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;

            AddMapFields();
            AddMapLegends();
            AddMapColoringRules();
        }

        private void AddMapColoringRules()
        {
            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.ShapeField = "IndicatorValue";
            rule.Category = String.Empty;
            rule.DataGrouping = DataGrouping.Optimal;
            rule.ColorCount = 2;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Green;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Red;
            rule.BorderColor = Color.FromArgb(70, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            //rule.ShowInLegend = "CrimesLegend";
            DundasMap.ShapeRules.Add(rule);
        }

        private void AddMapFields()
        {
            // добавляем поля
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("IndicatorValue");
            DundasMap.ShapeFields["IndicatorValue"].Type = typeof(double);
            DundasMap.ShapeFields["IndicatorValue"].UniqueIdentifier = false;
        }

        private void AddMapLegends()
        {
            DundasMap.Legends.Clear();

            // добавляем легенду
            Legend legend1 = new Legend("CrimesLegend");
            legend1.MaxAutoSize = 30;
            //legend1.TitleAlignment = StringAlignment.Near;
            legend1.Visible = true;
            legend1.BackColor = Color.White;
            legend1.BackSecondaryColor = Color.Gainsboro;
            legend1.BackGradientType = GradientType.DiagonalLeft;
            legend1.BackHatchStyle = MapHatchStyle.None;
            legend1.BorderColor = Color.Gray;
            legend1.BorderWidth = 1;
            legend1.BorderStyle = MapDashStyle.Solid;
            legend1.BackShadowOffset = 4;
            legend1.TextColor = Color.Black;
            legend1.Font = new System.Drawing.Font("MS Sans Serif", 8, FontStyle.Regular);
            legend1.Title = ComboIndicator.SelectedValue;


            LegendItem item = new LegendItem();
            item.Text = "Соблюдается условие";
            item.Color = Color.Green;
            legend1.Items.Add(item);

            item = new LegendItem();
            item.Text = "Не соблюдается условие";
            item.Color = Color.Red;
            legend1.Items.Add(item);


            legend1.AutoFitMinFontSize = 7;
            DundasMap.Legends.Add(legend1);
        }

        #endregion

        #region Обработчики грида

        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {
            if (row == null)
                return;

            UserParams.Subject.Value = row.Cells[0].Text;
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("MFRF_0002_0003_compare_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);

            UltraWebGrid.DataSource = dtGrid;
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (ComboFO.SelectedIndex != 0 || dtGrid.Rows.Count < 15)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(235);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count < 1)
            {
                return;
            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 4;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 3;
                }
            }

            // e.Layout.Bands[0].Columns[1].Header.Caption = e.Layout.Bands[0].Columns[1].Header.Caption.Split(';')[0];
            e.Layout.Bands[0].Columns[1].Width = 45;

            int multiHeaderPos = 1;

            ColumnHeader ch = new ColumnHeader(true);
            ch.Caption = e.Layout.Bands[0].Columns[1].Header.Caption.Split(';')[0];

            /*    if (ch.Caption == "Сумма просроченной (неурегулированной) задолженности по долговым обязательствам субъекта Российской Федерации")
                {
                    AddHeader(ch, e, 1, multiHeaderPos, "N2", 0, 2);


                    ch = new ColumnHeader(true);
                    ch.Caption = "в том числе:";

                    ch.RowLayoutColumnInfo.OriginY = 0;
                    ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                    ch.RowLayoutColumnInfo.SpanY = 1;
                    ch.RowLayoutColumnInfo.SpanX = 6;
                    e.Layout.Bands[0].HeaderLayout.Add(ch);

                    
                        ch = new ColumnHeader(true);
                        ch.Caption = e.Layout.Bands[0].Columns[1 + 2].Header.Caption.Split(';')[0];

                        AddHeader(ch, e, 1 + 2, multiHeaderPos, "N2", 1, 1);
       
               
                }
                else
                {  */
            AddHeader(ch, e, 1, multiHeaderPos, "N4", 0, 2);

            // }
        }


        private void AddHeader(ColumnHeader ch, LayoutEventArgs e, int i, int multiHeaderPos, string valueColumnFormat, int originY, int spanY)
        {
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Значение", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Нормативное значение", "");


            ch.RowLayoutColumnInfo.OriginY = originY;
            ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
            ch.RowLayoutColumnInfo.SpanY = spanY;
            ch.RowLayoutColumnInfo.SpanX = 2;
            ch.Style.BorderDetails.ColorBottom = Color.FromArgb(178, 178, 178);
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ColumnHeader chChild = new ColumnHeader(true);
            chChild.Caption = "Код:<br/>Условие:<br/>Единицы измерения:";
            chChild.RowLayoutColumnInfo.OriginY = originY + spanY;
            chChild.RowLayoutColumnInfo.OriginX = multiHeaderPos;
            chChild.RowLayoutColumnInfo.SpanY = 1;
            chChild.RowLayoutColumnInfo.SpanX = 1;
            chChild.Style.HorizontalAlign = HorizontalAlign.Left;
            chChild.Style.VerticalAlign = VerticalAlign.Bottom;
            chChild.Style.CustomRules = "font-size:xx-small; font-family: Verdana; font-weight: normal;";
            chChild.Style.BorderDetails.ColorRight = Color.FromArgb(178, 178, 178);
            chChild.Style.BorderDetails.ColorTop = Color.FromArgb(178, 178, 178);
            e.Layout.Bands[0].HeaderLayout.Add(chChild);

            chChild = new ColumnHeader(true);
            chChild.Caption = GetIndicatorComments(ch.Caption);
            chChild.RowLayoutColumnInfo.OriginY = originY + spanY;
            chChild.RowLayoutColumnInfo.OriginX = multiHeaderPos + 1;
            chChild.RowLayoutColumnInfo.SpanY = 1;
            chChild.RowLayoutColumnInfo.SpanX = 1;
            chChild.Style.HorizontalAlign = HorizontalAlign.Left;
            chChild.Style.VerticalAlign = VerticalAlign.Bottom;
            chChild.Style.CustomRules = "font-size:xx-small; font-family: Verdana; font-weight: normal;";
            chChild.Style.BorderDetails.ColorLeft = Color.FromArgb(178, 178, 178);
            chChild.Style.BorderDetails.ColorTop = Color.FromArgb(178, 178, 178);
            e.Layout.Bands[0].HeaderLayout.Add(chChild);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], valueColumnFormat);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "N2");

            e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(110, 1280);
            e.Layout.Bands[0].Columns[i + 1].Width = CRHelper.GetColumnWidth(100, 1280);

            e.Layout.Bands[0].Columns[i + 2].Hidden = true;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
         
            if (e.Row.Cells[1 + 2].ToString() != "Cell")
            {

                if (Convert.ToInt32(e.Row.Cells[1 + 2].Value) == 1)
                {
                    e.Row.Cells[1].Style.BackgroundImage = "~/images/CornerRed.gif";
                    e.Row.Cells[1].Title = string.Format("Не соблюдается условие");
                }
                else if (Convert.ToInt32(e.Row.Cells[1 + 2].Value) == 0)
                {
                    e.Row.Cells[1].Style.BackgroundImage = "~/images/CornerGreen.gif";
                    e.Row.Cells[1].Title = string.Format("Соблюдается условие");
                    serial++;
                }
            }

            e.Row.Cells[1].Style.CustomRules =
                "background-repeat: no-repeat; background-position: right top; margin: 0px";
        }



        #endregion

        #region Обработчики диаграмы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string queryName = "Chart_query";
            string query = DataProvider.GetQueryText(queryName);
            DataTable dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            if ((ComboFO.SelectedValue == "Северо-Кавказский федеральный округ") || (ComboFO.SelectedValue == "Уральский федеральный округ") || (ComboFO.SelectedValue == "Дальневосточный федеральный округ"))
            {
                UltraChart.Axis.X.Extent = 60;
                foreach (DataColumn column in dtChart.Columns)
                {
                    column.ColumnName = RegionsNamingHelper.ShortName(column.ColumnName);
                }
            } 

            UltraChart.Data.SwapRowsAndColumns = false; 
            UltraChart.DataSource = dtChart;
           
        }

        void UltraChart_InterpolateValues(object sender, InterpolateValuesEventArgs e)
        {
            for (int i = 0; i < e.NullValueIndices.Length; i++)
            {
                e.Values.SetValue(Double.MinValue, e.NullValueIndices[i]);
            }
        }

        void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            int count = 0;

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive.ToString() == "Infragistics.UltraChart.Core.Primitives.PointSet")
                {
                    PointSet ps = primitive as PointSet;
                    for (int j = 0; j < ps.points.Length; j++)
                    {

                        if (ps.points[j].Row == 1)
                        {

                            ps.points[j].Visible = false;
                            ps.points[j].hitTestRadius = 20;
                            DrawBox(e, ps.points[j].point, Color.Green);


                        }


                        if (ps.points[j].Row == 2)
                        {

                            ps.points[j].hitTestRadius = 20;
                            ps.points[j].Visible = false;
                            DrawBox(e, ps.points[j].point, Color.Red);
                        }


                        if (primitive is Text)
                        {
                            Text text = (Text)primitive;
                            if (text.Path.Contains("Border.Title.Grid.X"))
                            {
                                text.SetTextString(RegionsNamingHelper.ShortName(text.GetTextString()));
                            }
                        }

                    }


                }
                if (primitive.ToString() == "Infragistics.UltraChart.Core.Primitives.Polyline")
                {
                    primitive.PE.Fill = Color.Blue;
                }

                if (primitive is Box)
                {
                    Box box = (Box)primitive;

                    if (!(string.IsNullOrEmpty(box.Path)) && box.Path.EndsWith("Legend") &&
                         box.rect.Width == box.rect.Height)
                    {
                        Color color = Color.Aqua;
                        count++;

                        switch (count)
                        {
                            case 1:
                                {
                                    color = Color.Blue;
                                    break;
                                }
                            case 2:
                                {
                                    color = Color.Green;
                                    break;
                                }
                            case 3:
                                {
                                    color = Color.Red;
                                    break;
                                }
                        }

                        Box box1 = new Box(box.rect);
                        box1.PE.ElementType = PaintElementType.Gradient;
                        box1.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
                        box1.PE.Fill = color;
                        box1.PE.FillStopColor = color;
                        box1.PE.Stroke = Color.Black;
                        box1.PE.StrokeWidth = 1;
                        box1.Row = 0;
                        box1.Column = 2;
                        box1.Value = 42;
                        box1.Layer = e.ChartCore.GetChartLayer();
                        box1.Chart = this.UltraChart.ChartType;
                        e.SceneGraph.Add(box1);
                    }
                }

            }
            serial = 0;


        }

        private void DrawBox(FillSceneGraphEventArgs e, Point p, Color color)
        {
            Box box = new Box(new Point(p.X - 6, p.Y - 6), 13, 13);

            box.PE.ElementType = PaintElementType.Gradient;
            box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
            box.PE.Fill = color;
            box.PE.FillStopColor = color;
            box.PE.Stroke = Color.Black;
            box.PE.StrokeWidth = 1;
            box.Row = 0;
            box.Column = 2;
            box.Value = 42;
            box.Layer = e.ChartCore.GetChartLayer();
            box.Chart = this.UltraChart.ChartType;

            e.SceneGraph.Add(box);
        }

        #endregion

        #region Экспорт

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            //e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text;
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            e.CurrentWorksheet.Columns[0].Width = 250 * 37;
            e.CurrentWorksheet.Columns[1].Width = 50 * 37;
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

            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = UltraGridExporter.ExelNumericFormat;
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = UltraGridExporter.ExelNumericFormat;
            e.CurrentWorksheet.Columns[4].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "##";
            e.CurrentWorksheet.Columns[6].CellFormat.FormatString = "##";
            e.CurrentWorksheet.Columns[7].CellFormat.FormatString = UltraGridExporter.ExelNumericFormat;
            e.CurrentWorksheet.Columns[8].CellFormat.FormatString = UltraGridExporter.ExelNumericFormat;
            e.CurrentWorksheet.Columns[9].CellFormat.FormatString = "##";
            e.CurrentWorksheet.Columns[10].CellFormat.FormatString = "##";
            e.CurrentWorksheet.Columns[11].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[12].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            Worksheet sheet3 = workbook.Worksheets.Add("Карта");
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            sheet1.Rows[0].Cells[0].Value = Label1.Text;
            sheet1.Rows[1].Cells[0].Value = Label2.Text.Replace("<br/>", " ");
            sheet2.Rows[0].Cells[0].Value = Label1.Text;
            sheet2.Rows[1].Cells[0].Value = Label2.Text.Replace("<br/>", " ");
            sheet3.Rows[0].Cells[0].Value = Label1.Text;
            sheet3.Rows[1].Cells[0].Value = Label2.Text.Replace("<br/>", " ");
            UltraGridExporter.ChartExcelExport(sheet2.Rows[4].Cells[0],UltraChart);
            UltraGridExporter.MapExcelExport(sheet3.Rows[4].Cells[0],DundasMap);
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);

        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
            
        }

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
            title.Style.Font = new Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Font(font);
            title.AddContent(Label2.Text.Replace("<br/>", "\n"));
        }

        private Image img;
        private void PdfExporter_EndExport(object sender, EndExportEventArgs e)
        {
            e.Section.AddPageBreak();

            IText title = e.Section.AddText();
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
            title.Style.Font = new Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Font(font);
            title.AddContent(Label2.Text.Replace("<br/>","\n"));
            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth*0.8));
            MemoryStream imageStream = new MemoryStream();
            UltraChart.SaveTo(imageStream, ImageFormat.Png);
            Image image = (new Bitmap(imageStream)).ScaleImageIg(1.3);
            e.Section.AddImage(image);

            e.Section.AddPageBreak();

            title = e.Section.AddText();
            font = new System.Drawing.Font("Verdana", 16);
            title.Style.Font = new Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Font(font);
            title.AddContent(Label2.Text.Replace("<br/>", "\n"));
            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            img = UltraGridExporter.GetImageFromMap(DundasMap);
            e.Section.AddImage(img);
        }

        #endregion
        
        private string GetIndicatorComments(string name)
        {
            if (dtComments == null || dtComments.Columns.Count == 0)
            {
                dtComments = new DataTable();
                string query = DataProvider.GetQueryText("MFRF_0002_0003_compare_comment");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtComments);
            }

            for (int i = 1; i < dtComments.Columns.Count; i++)
            {
                if (dtComments.Columns[i].Caption == name)
                {
                    return String.Format("{0}<br/>{1}<br/>{2}",
                            dtComments.Rows[0][i],
                            dtComments.Rows[1][i],
                            dtComments.Rows[2][i]);
                }
            }
            return String.Empty;
        }
    }
}
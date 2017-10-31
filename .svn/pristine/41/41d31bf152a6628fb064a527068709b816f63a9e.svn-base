using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using Symbol = Dundas.Maps.WebControl.Symbol;
using Infragistics.Documents.Reports.Report.Text;

namespace Krista.FM.Server.Dashboards.reports.EO_0001_0001
{
    public partial class Default : CustomReportPage
    {
        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1200; }
        }

        private int MinScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }

        private int MinScreenHeight
        {
            get { return IsSmallResolution ? 600 : CustomReportConst.minScreenHeight; }
        }

        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtChart;


        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.60);

            DundasMap1.Width = CRHelper.GetChartWidth(MinScreenWidth - 30);
            DundasMap1.Height = CRHelper.GetChartHeight(MinScreenHeight * 0.85);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.63);

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.LineChart;
            UltraChart.Border.Thickness = 0;
           // UltraChart.Data.ZeroAligned = true;

            UltraChart.Axis.X.Extent = 60;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;

            UltraChart.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart.Axis.X.Margin.Near.Value = 3;
            UltraChart.Axis.X.Margin.Far.Value = 2;
            UltraChart.Axis.Y.Margin.Near.Value = 7;
            UltraChart.Axis.Y.Margin.Far.Value = 3;
            UltraChart.Axis.X.StripLines.PE.FillOpacity = 150;
            UltraChart.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
            UltraChart.Axis.X.StripLines.Interval = 2;
            UltraChart.Axis.X.StripLines.Visible = true;
            UltraChart.Axis.Y.Extent = 65;
            UltraChart.Axis.Y2.Visible = true;
            UltraChart.Axis.Y2.Extent = 65;
            UltraChart.Axis.Y2.LineThickness = 0;
            UltraChart.Axis.Y2.Margin.Near.Value = 7;
            UltraChart.Axis.Y2.Margin.Far.Value = 3;

            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart.Axis.X.Labels.WrapText = true;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            UltraChart.Axis.X.Labels.VerticalAlign = StringAlignment.Center;
            //UltraChart.Legend.Margins.Right = 3 * Convert.ToInt32(UltraChart.Width.Value) / 4;
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 12;

            //UltraChart.TitleLeft.Visible = true;
            //UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            //UltraChart.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart.Height.Value) / 4;
            //UltraChart.TitleLeft.Text = "Тыс.руб.";

            UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL> <DATA_VALUE:N3>";
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            AddLineAppearencesUltraChart1();

            #endregion

            #region Настройка карты 1

            DundasMap1.Meridians.Visible = false;
            DundasMap1.Parallels.Visible = false;
            DundasMap1.ZoomPanel.Visible = !IsSmallResolution;
            DundasMap1.ZoomPanel.Dock = PanelDockStyle.Right;
            DundasMap1.NavigationPanel.Visible = !IsSmallResolution;
            DundasMap1.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap1.Viewport.EnablePanning = true;

            // добавляем легенду
            DundasMap1.Legends.Clear();
            // добавляем легенду с символами
            //Legend legend2 = new Legend("SymbolLegend");
            //legend2.Visible = true;
            //legend2.Dock = PanelDockStyle.Left;
            //legend2.BackColor = Color.White;
            //legend2.BackSecondaryColor = Color.Gainsboro;
            //legend2.BackGradientType = GradientType.DiagonalLeft;
            //legend2.BackHatchStyle = MapHatchStyle.None;
            //legend2.BorderColor = Color.Gray;
            //legend2.BorderWidth = 1;
            //legend2.BorderStyle = MapDashStyle.Solid;
            //legend2.BackShadowOffset = 4;
            //legend2.TextColor = Color.Black;
            //legend2.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            //legend2.AutoFitText = true;
            //legend2.Title = IsSmallResolution ? "Ранг инвестиционной привлекательности" : "Ранг инвестиционной привлекательности";
            //legend2.AutoFitMinFontSize = 7;
            //DundasMap1.Legends.Add(legend2);

            // добавляем легенду раскраски
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
            legend1.Title = IsSmallResolution ? "Интегральный индекс\nинвестиционной привлекательности" : "Интегральный индекс\nинвестиционной привлекательности";
            legend1.AutoFitMinFontSize = 7;
            DundasMap1.Legends.Add(legend1);

            // добавляем поля для раскраски
            DundasMap1.ShapeFields.Clear();
            DundasMap1.ShapeFields.Add("Name");
            DundasMap1.ShapeFields["Name"].Type = typeof(string);
            DundasMap1.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap1.ShapeFields.Add("InvestmentsBeauty");
            DundasMap1.ShapeFields["InvestmentsBeauty"].Type = typeof(double);
            DundasMap1.ShapeFields["InvestmentsBeauty"].UniqueIdentifier = false;

            // добавляем поля для символов
            //DundasMap1.SymbolFields.Add("InvestmentsRank");
            //DundasMap1.SymbolFields["InvestmentsRank"].Type = typeof(double);
            //DundasMap1.SymbolFields["InvestmentsRank"].UniqueIdentifier = false;

            // добавляем правила раскраски
            DundasMap1.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "InvestmentsBeauty";
            rule.DataGrouping = DataGrouping.EqualInterval;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Red;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Green;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";
            rule.LegendText = "#FROMVALUE{N1} - #TOVALUE{N1}";

            DundasMap1.ShapeRules.Add(rule);

            // добавляем правила расстановки символов
            //DundasMap1.SymbolRules.Clear();
            //SymbolRule symbolRule = new SymbolRule();
            //symbolRule.Name = "SymbolRule";
            //symbolRule.Category = string.Empty;
            //symbolRule.DataGrouping = DataGrouping.EqualInterval;
            //symbolRule.SymbolField = "InvestmentsRank";
            //symbolRule.ShowInLegend = "SymbolLegend";
            //DundasMap1.SymbolRules.Add(symbolRule);

            //// звезды для легенды
            //for (int i = 3; i > 0; i--)
            //{
            //    PredefinedSymbol predefined = new PredefinedSymbol();
            //    predefined.Name = "PredefinedSymbol" + i;
            //    predefined.MarkerStyle = MarkerStyle.Star;
            //    predefined.Width = 5 + (i * 5);
            //    predefined.Height = predefined.Width;
            //    predefined.Color = Color.DarkViolet;
            //    DundasMap1.SymbolRules["SymbolRule"].PredefinedSymbols.Add(predefined);
            //}

            #endregion

            UltraGridExporter1.MultiHeader = true;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
        }

        private void AddLineAppearencesUltraChart1()
        {
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart.ColorModel.Skin.ApplyRowWise = true;
            UltraChart.ColorModel.Skin.PEs.Clear();

            for (int i = 1; i <= 7; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.Green;
                            break;
                        }
                    case 2:
                        {
                            color = Color.Gold;
                            break;
                        }
                    case 3:
                        {
                            color = Color.Black;
                            break;
                        }
                    case 4:
                        {
                            color = Color.LightSlateGray;
                            break;
                        }
                    case 5:
                        {
                            color = Color.Red;
                            break;
                        }
                    case 6:
                        {
                            color = Color.Blue;
                            break;
                        }
                    case 7:
                        {
                            color = Color.DarkViolet;
                            break;
                        }
                }
                pe.Fill = color;
                pe.StrokeWidth = 0;
                UltraChart.ColorModel.Skin.PEs.Add(pe);
                pe.Stroke = Color.Black;
                pe.StrokeWidth = 0;
                LineAppearance lineAppearance2 = new LineAppearance();

                //lineAppearance2.LineStyle.
                lineAppearance2.IconAppearance.Icon = SymbolIcon.Square;
                lineAppearance2.IconAppearance.IconSize = SymbolIconSize.Medium;
                lineAppearance2.IconAppearance.PE = pe;

                UltraChart.LineChart.LineAppearances.Add(lineAppearance2);

                UltraChart.LineChart.Thickness = 0;
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            WebAsyncPanel.AddRefreshTarget(UltraWebGrid);
            WebAsyncPanel.AddLinkedRequestTrigger(detail.ClientID);
            
            dtDate = new DataTable();

            UserParams.PeriodYear.Value = "[Период].[Период].[Данные всех периодов].[2010]";
            int year = 2010;
            string query = DataProvider.GetQueryText("EO_0001_0001_date_nonEmptyCrossJoin");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            if (dtDate.Rows.Count != 0)
            {
                year = Convert.ToInt32(dtDate.Rows[0][0]);
                UserParams.PeriodYear.Value = dtDate.Rows[0][1].ToString();
            }

            Page.Title = String.Format("Оценка инвестиционной привлекательности субъектов УрФО на 18.06.2010");
            PageTitle.Text = Page.Title;
            //chart1Label.Text = "Распределение по объему профицита(+)/дефицита(-) местных бюджетов";
            PageSubTitle.Text = "Оценка инвестиционной привлекательности субъектов УРФО на основе объективных предпосылок для осуществления инвестирования.";
            

            UltraChart.DataBind();
            UltraWebGrid.DataBind();

            // заполняем карту формами
            string regionStr = "УрФО";
            DundasMap1.Shapes.Clear();
            DundasMap1.LoadFromShapeFile(Server.MapPath(string.Format("../../maps/{0}/{0}.shp", RegionsNamingHelper.ShortName(regionStr))), "NAME", true);
            // заполняем карту данными
            FillMapData1(DundasMap1);
        }

        #region Обработчики карты

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

        public void FillMapData1(MapControl map)
        {
            if (map == null)
            {
                return;
            }

            string query = DataProvider.GetQueryText("EO_0001_0001_map");

            DataTable dtMap1 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtMap1);

            map.Symbols.Clear();

            foreach (DataRow row in dtMap1.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                {
                    string regionName = row[0].ToString();

                    if (RegionsNamingHelper.IsSubject(regionName))
                    {
                        Shape shape = FindMapShape(map, regionName);
                        if (shape != null)
                        {
                            double investmentsBeauty = Convert.ToDouble(row[1]);
                            double investmentsRank = Convert.ToDouble(row[2]);

                            shape["Name"] = regionName;
                            shape["InvestmentsBeauty"] = investmentsBeauty;
                            shape.ToolTip = string.Format("#NAME \nранг по УрФО: {0:N0} \nинвестиционная \nпривлекательность: {1:N3}",
                                    investmentsRank, investmentsBeauty);
                            shape.TextAlignment = ContentAlignment.MiddleCenter;
                            shape.Offset.X = -15;
                            if (!IsSmallResolution)
                            {
                                shape.Offset.Y = -30;
                            }

                            shape.Text = string.Format("{0}\n {1:N3}", shape.Name, investmentsBeauty);

                            shape.BorderWidth = 2;
                            shape.TextColor = Color.Black;
                            //shape.Font = new Font("Verdana", IsSmallResolution ? 7 : 8, FontStyle.Bold);
                            shape.TextVisibility = TextVisibility.Shown;

                            //Symbol symbol = new Symbol();
                            //symbol.Name = shape.Name + map.Symbols.Count;
                            //symbol.ParentShape = shape.Name;
                            //symbol["InvestmentsRank"] = investmentsRank;
                            //symbol.Offset.Y = -30;
                            //symbol.Color = Color.DarkViolet;
                            //symbol.MarkerStyle = MarkerStyle.Star;
                            //map.Symbols.Add(symbol);

                            if (IsSmallResolution)
                            {
                                if (shape.Name.Contains("Курган"))
                                {
                                    shape.TextAlignment = ContentAlignment.MiddleCenter;
                                }
                                if (shape.Name.Contains("Челябинск"))
                                {
                                    shape.TextAlignment = ContentAlignment.TopCenter;
                                    shape.Offset.X = -10;
                                    //symbol.Offset.Y = -10;
                                }
                                if (shape.Name.Contains("Тюмен"))
                                {
                                    //                                    shape.TextAlignment = ContentAlignment.TopCenter;
                                    //                                    symbol.Offset.Y = -10;
                                }
                                if (shape.Name.Contains("Свердловск"))
                                {
                                    shape.Offset.Y = -10;
                                    //symbol.Offset.Y = -10;
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string queryName = !detail.Checked ? "EO_0001_0001_grid" : "EO_0001_0001_grid_full";
            UltraWebGrid.Height = detail.Checked ? CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.60) : Unit.Empty;

            string query = DataProvider.GetQueryText(queryName);
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Регион", dtGrid);

            Collection<int> removingRows = new Collection<int>();
            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                if (dtGrid.Rows[i][0].ToString().Contains("Ранг региона"))
                {
                    removingRows.Add(i);
                    for (int j = 2; j < dtGrid.Columns.Count; j += 2)
                    {
                        dtGrid.Rows[i - 1][j] = dtGrid.Rows[i][j - 1];
                    }
                }
            }

            for (int i = 0; i < removingRows.Count; i++)
            {
                dtGrid.Rows.RemoveAt(removingRows[i] - i);
            }
            dtGrid.AcceptChanges();
            UltraWebGrid.DataSource = dtGrid;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            e.Layout.RowHeightDefault = 25;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(200);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 1;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 2)
            {
                ColumnHeader ch = new ColumnHeader(true);
                ch.Caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Значение", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Ранг", "");

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                ch.RowLayoutColumnInfo.SpanY = 1;
                multiHeaderPos += 2;
                ch.RowLayoutColumnInfo.SpanX = 2;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
                e.Layout.Bands[0].Columns[i].Width = 80;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N3");
                e.Layout.Bands[0].Columns[i + 1].Width = 70;

            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(195);
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }
            if (e.Row.Cells[2].Value != null &&
                e.Row.Cells[2].Value.ToString() != String.Empty)
            {
                for (int i = 2; i < dtGrid.Columns.Count; i += 2)
                {
                    if (e.Row.Cells[i].ToString() == "1")
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "../../images/StarYellowBB.png";
                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left center;";
                    }
                    else if (e.Row.Cells[i].ToString() == "6")
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "../../images/StarGrayBB.png";
                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left center;";
                    }
                }
            }
            else
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
            }

            //for (int i = 0; i < e.Row.Cells.Count; i++)
            //{
            //    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
            //    {
            //        if (e.Row.Cells[0].Value != null &&
            //            (e.Row.Cells[0].Value.ToString() == "Профицитные" ||
            //             e.Row.Cells[0].Value.ToString() == "Дефицитные" ||
            //             e.Row.Cells[0].Value.ToString() == "Сбалансированные"))
            //        {
            //            if (i == 0)
            //            {
            //                e.Row.Cells[i].Style.Padding.Left = 15;
            //            }
            //            else if (dtGrid != null)
            //            {
            //                int j = cellCount + (e.Row.Index - 1) * 6 + i - 1;

            //                string value = dtGrid.Rows[0][j].ToString().Replace("br", "\r");
            //                for (int k = 3; k <= 4; k++)
            //                {
            //                    value = RegionsNamingHelper.CheckMultiplyValue(value.Replace("br", "\r"), k);
            //                }
            //                value = value.Replace("муниципальный район", "МР");
            //                value = value.Replace("Муниципальный район", "МР");
            //                value = value.Replace("район", "р-н");
            //                e.Row.Cells[i].Title = value;
            //            }
            //        }
            //    }
            //}
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid.Height = Unit.Empty;
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("EO_0001_0001_chart");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);


            UltraChart.DataSource = dtChart;
        }

        private int GetMaxRowIndex(string col)
        {
            int result = 0;
            double value = 0;
            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                if (value < Convert.ToDouble(dtChart.Rows[i][col]))
                {
                    value = Convert.ToDouble(dtChart.Rows[i][col]);
                    result = i;
                }
            }
            return result;
        }

        private int GetMinRowIndex(string col)
        {
            int result = 0;
            double value = 10;
            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                if (value > Convert.ToDouble(dtChart.Rows[i][col]))
                {
                    value = Convert.ToDouble(dtChart.Rows[i][col]);
                    result = i;
                }
            }
            return result;
        }

        private string GetRegion(int row)
        {
            return dtChart.Rows[row][0].ToString();
        }

        private double GetValue(int row, string col)
        {
            return Convert.ToDouble(dtChart.Rows[row][col].ToString());
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            Collection<string> indicators = new Collection<string>();
            Collection<string> maxRegions = new Collection<string>();
            Collection<double> maxValue = new Collection<double>();
            Collection<string> minRegions = new Collection<string>();
            Collection<double> minValue = new Collection<double>();
            Collection<int> leftBound = new Collection<int>();

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null &&
                    primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    text.bounds.Width = 120;
                    text.bounds.Height = 50;
                    text.labelStyle.VerticalAlign = StringAlignment.Near;
                    text.labelStyle.FontSizeBestFit = false;
                    text.labelStyle.Font = new Font("Verdana", 8);
                    text.labelStyle.WrapText = true;

                    int maxRowIndex = GetMaxRowIndex(text.GetTextString());
                    int minRowIndex = GetMinRowIndex(text.GetTextString());

                    indicators.Add(text.GetTextString());
                    maxRegions.Add(GetRegion(maxRowIndex));
                    maxValue.Add(GetValue(maxRowIndex, text.GetTextString()));

                    minRegions.Add(GetRegion(minRowIndex));
                    minValue.Add(GetValue(minRowIndex, text.GetTextString()));

                    leftBound.Add(text.bounds.X);
                }
            }

            for (int i = 0; i < indicators.Count; i++)
            {
                Text newMaxText = new Text();
                newMaxText.labelStyle.Font = new Font("Verdana", 8);

                newMaxText.PE.Fill = Color.Black;
                newMaxText.bounds = new Rectangle(leftBound[i] + 35, (int)yAxis.Map(maxValue[i]) - 20, 50, 15);
                newMaxText.labelStyle.VerticalAlign = StringAlignment.Center;
                // newText.labelStyle.HorizontalAlign = StringAlignment.Center;
                newMaxText.SetTextString(RegionsNamingHelper.ShortName(maxRegions[i]));
                e.SceneGraph.Add(newMaxText);


                Text newMinText = new Text();
                newMinText.labelStyle.Font = new Font("Verdana", 8);
                newMinText.PE.Fill = Color.Black;
                newMinText.bounds = new Rectangle(leftBound[i] + 35, (int)yAxis.Map(minValue[i]) + 4, 50, 15);
                newMinText.labelStyle.VerticalAlign = StringAlignment.Center;
                // newText.labelStyle.HorizontalAlign = StringAlignment.Center;
                newMinText.SetTextString(RegionsNamingHelper.ShortName(minRegions[i]));
                e.SceneGraph.Add(newMinText);
            }

            //string textValue = GetRegion(GetMaxRowIndex(text.GetTextString()));




            //    if (primitive is Box)
            //    {
            //        Box box = (Box)primitive;
            //        if (box.DataPoint != null && box.Value != null)
            //        {
            //            string otherMeasureText = string.Empty;
            //            double otherValue = 0;
            //            if (dtChart != null && dtChart.Rows[box.Row][2] != DBNull.Value && 
            //                dtChart.Rows[box.Row][2].ToString() != string.Empty)
            //            {
            //                otherValue = Convert.ToDouble(dtChart.Rows[box.Row][2]);

            //            }

            //            double value = Convert.ToDouble(box.Value);
            //            if (value > 0)
            //            {

            //                box.PE.ElementType = PaintElementType.Gradient;
            //                box.PE.FillGradientStyle = GradientStyle.Horizontal;
            //                box.PE.Fill = Color.Green;
            //                box.PE.FillStopColor = Color.ForestGreen;
            //            }
            //            else
            //            {

            //                box.PE.ElementType = PaintElementType.Gradient;
            //                box.PE.FillGradientStyle = GradientStyle.Horizontal;
            //                box.PE.Fill = Color.Red;
            //                box.PE.FillStopColor = Color.Maroon;
            //            }
            //        }
            //        else if (box.Path != null && box.Path.ToLower().Contains("legend") && box.rect.Width < 20)
            //        {
            //            box.PE.ElementType = PaintElementType.CustomBrush;
            //            LinearGradientBrush brush = new LinearGradientBrush(box.rect, Color.Green, Color.Red, 45, false);
            //            box.PE.CustomBrush = brush;
            //        }
            //    }
            //}
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
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

            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "0.###";
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "##";
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "0.###";
            e.CurrentWorksheet.Columns[4].CellFormat.FormatString  = "##";
            e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "0.###";
            e.CurrentWorksheet.Columns[6].CellFormat.FormatString = "##";
            e.CurrentWorksheet.Columns[7].CellFormat.FormatString = "0.###";
            e.CurrentWorksheet.Columns[8].CellFormat.FormatString = "##";
            e.CurrentWorksheet.Columns[9].CellFormat.FormatString  = "0.###";
            e.CurrentWorksheet.Columns[10].CellFormat.FormatString = "##";
            e.CurrentWorksheet.Columns[11].CellFormat.FormatString = "0.###";
            e.CurrentWorksheet.Columns[12].CellFormat.FormatString = "##";
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
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
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            //IText title = e.Section.AddText();
            //Font font = new Font("Verdana", 12);
            //title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            //title.AddContent(chart1Label.Text);

            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap1);
            e.Section.AddImage(img);

            UltraChart.Width = (int) UltraChart.Width.Value - 100;
            img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        #endregion
    }
}

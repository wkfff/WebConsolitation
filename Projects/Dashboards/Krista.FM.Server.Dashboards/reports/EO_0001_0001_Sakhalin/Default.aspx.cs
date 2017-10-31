        using System;
using System.Collections;
        using System.Collections.Generic;
        using System.Collections.ObjectModel;
using System.Data;
    using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
        using Infragistics.Documents.Excel;
        using Infragistics.Documents.Reports.Report;
        using Infragistics.Documents.Reports.Report.Section;
        using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
        using Krista.FM.Server.Dashboards.Components.Components;
        using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using Symbol = Dundas.Maps.WebControl.Symbol;
using Infragistics.Documents.Reports.Report.Text;
        using ContentAlignment = System.Drawing.ContentAlignment;

namespace Krista.FM.Server.Dashboards.reports.EO_0001_0001_Sakhalin
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

        private int firstYear = 2009;
        private int endYear = 2012;
        private int year;
        private int count;
        private GridHeaderLayout headerLayout1;

        // имя папки с картами региона
        private string mapFolderName = "Сахалин";
        // масшбтаб карты
        private double mapZoomValue;
        // сдвиг по вертикали текста городов-выносок карты
        private double mapCalloutOffsetY;
        // пропорция карты
        private double mapSizeProportion;
        private bool useRegionCodes = true;

        #endregion

        #region параметры запроса

        private CustomParam years;
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);


            mapFolderName = "Сахалин";// RegionSettingsHelper.Instance.GetPropertyValue("MapFolderName");
            CRHelper.SaveToErrorLog(RegionSettingsHelper.Instance.GetPropertyValue("MapZoomValue"));
            mapZoomValue = 100; //Convert.ToDouble(RegionSettingsHelper.Instance.GetPropertyValue("MapZoomValue"));

            double value = 0.7;
            mapSizeProportion = value;

            value = 0.5;
            mapCalloutOffsetY = value;
          
            double scale = 0.9;

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.7);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.60);

            DundasMap1.Width = CRHelper.GetChartWidth(MinScreenWidth - 30);
            DundasMap1.Height = CRHelper.GetChartHeight(MinScreenHeight * 0.85);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.7);

            #region диаграмма 1

          /*  UltraChart1.ChartType = ChartType.LineChart;
            UltraChart1.Axis.X.Extent = 25;
            UltraChart1.Axis.Y.Extent = 50;
            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Text = "";
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL> \n Интегральный индекс \n инвестиционной привлекательности\n за <SERIES_LABEL> \n составляет <DATA_VALUE:N3>";
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N3>";

            UltraChart1.Axis.X.Margin.Near.Value = 3;
            UltraChart1.Axis.X.Margin.Far.Value = 3;
            UltraChart1.Axis.Y.Margin.Near.Value = 3;
            UltraChart1.Axis.Y.Margin.Far.Value = 3;
            
            UltraChart1.Data.SwapRowsAndColumns = true;
            UltraChart1.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart.Axis.X.Labels.HorizontalAlign = StringAlignment.Center;
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 20;
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = true;
            UltraChart1.ColorModel.Skin.PEs.Clear();

            for (int i = 1; i <= 19; i++)
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
                            color = Color.Beige;
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
                    case 8:
                        {
                            color = Color.Yellow;
                            break;
                        }
                    case 9:
                        {
                            color = Color.Pink;
                            break;
                        }
                    case 10:
                        {
                            color = Color.Cyan;
                            break;
                        }
                    case 11:
                        {
                            color = Color.Magenta;
                            break;
                        }
                    case 12:
                        {
                            color = Color.Orange;
                            break;
                        }
                    case 13:
                        {
                            color = Color.Brown;
                            break;
                        }
                    case 14:
                        {
                            color = Color.PaleVioletRed;
                            break;
                        }
                    case 15:
                        {
                            color = Color.DarkKhaki;
                            break;
                        }
                    case 16:
                        {
                            color = Color.Olive;
                            break;
                        }
                    case 17:
                        {
                            color = Color.LightCoral;
                            break;
                        }
                    case 18:
                        {
                            color = Color.BlueViolet;
                            break;
                        }
                    case 19:
                        {
                            color = Color.Chocolate;
                            break;
                        }
                }
                pe.Fill = color;
                pe.StrokeWidth = 0;
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
                pe.Stroke = Color.Black;
                pe.StrokeWidth = 0;

                LineAppearance lineAppearance1 = new LineAppearance();
                lineAppearance1.IconAppearance.Icon = SymbolIcon.Square;
                lineAppearance1.Thickness = 0;
                lineAppearance1.IconAppearance.PE.ElementType = PaintElementType.SolidFill;
                lineAppearance1.IconAppearance.PE = pe;
                UltraChart1.LineChart.LineAppearances.Add(lineAppearance1);
            }
           * 
           * 
           */

            UltraChart1.ChartType = ChartType.ScatterChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.Y.RangeType = AxisRangeType.Custom;
            UltraChart1.Axis.Y.RangeMax = 2011;
            UltraChart1.Axis.Y.RangeMin = 2009;

            UltraChart1.Axis.Y.TickmarkStyle = AxisTickStyle.DataInterval;
            UltraChart1.Axis.Y.TickmarkInterval = 1;
            
            UltraChart1.Axis.X.Extent = 100;
            UltraChart1.Axis.Y.Extent = 50;
            UltraChart1.Axis.Y.Margin.Far.Value = 3;
            UltraChart1.Axis.Y.Margin.Near.Value = 3;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0> год";
            UltraChart1.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N3>";

            UltraChart1.TitleLeft.Visible = false;
            UltraChart1.TitleLeft.Text = "Квартал";
            UltraChart1.TitleLeft.Font = new Font("Verdana", 12);
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;

            UltraChart1.TitleBottom.Visible = false;
            UltraChart1.TitleBottom.Text = "Значение индекса инвестиционной привлекательности";
            UltraChart1.TitleBottom.Font = new Font("Verdana", 12);
            UltraChart1.TitleBottom.HorizontalAlign = StringAlignment.Center;

            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 20;
            UltraChart1.ScatterChart.Icon = SymbolIcon.Square;
            UltraChart1.Tooltips.FormatString = "<SERIES_LABEL>\n Интегральный индекс инвестиционной привлекательности \n за <DATA_VALUE_Y> год \n составляет <DATA_VALUE_X:N3>";

            UltraChart1.ScatterChart.UseGroupByColumn = true;
            UltraChart1.ScatterChart.IconSize = SymbolIconSize.Medium;
            UltraChart1.Legend.SpanPercentage = 20;

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            int count = 20;
            for (int i = 1; i < count; i++)
            {
                Color color = GetCustomColor(i);
                UltraChart1.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color, 150));
            }
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            
            #endregion

            #region Диаграмма2

            UltraChart.ChartType = ChartType.LineChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.Data.ZeroAligned = false;

            UltraChart.Axis.X.Extent = 80;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;

            UltraChart.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart.Axis.X.Margin.Near.Value = 2;
            UltraChart.Axis.X.Margin.Far.Value = 7;
            UltraChart.Axis.Y.Margin.Near.Value = 10;
            UltraChart.Axis.Y.Margin.Far.Value = 4;
           
            UltraChart.Axis.X.StripLines.PE.FillOpacity = 150;
            UltraChart.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
            UltraChart.Axis.X.StripLines.Interval = 2;
            UltraChart.Axis.X.StripLines.Visible = true;
           
            UltraChart.Axis.Y.Extent = 40;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N3>";
            UltraChart.Axis.X.Labels.WrapText = true;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart.Axis.X.Labels.VerticalAlign = StringAlignment.Center;
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 20;

            UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL> <DATA_VALUE:N3>";
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            AddLineAppearencesUltraChart1();

            #endregion

            /* #region Настройка карты 1

            DundasMap1.Meridians.Visible = false;
            DundasMap1.Parallels.Visible = false;
            DundasMap1.ZoomPanel.Visible = !IsSmallResolution;
            DundasMap1.ZoomPanel.Dock = PanelDockStyle.Left ;
            DundasMap1.NavigationPanel.Visible = !IsSmallResolution;
            DundasMap1.NavigationPanel.Dock = PanelDockStyle.Left;
            DundasMap1.Viewport.EnablePanning = true;

            // добавляем легенду
            DundasMap1.Legends.Clear();
          
            // добавляем легенду раскраски
            Legend legend1 = new Legend("CompleteLegend");
            legend1.Visible = true;
            //legend1.Dock = PanelDockStyle.Right;
            legend1.DockAlignment = DockAlignment.Far;
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
            legend1.Title = "Интегральный индекс\nинвестиционной привлекательности";
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

           // добавляем правила раскраски
            DundasMap1.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "InvestmentsBeauty";
            rule.DataGrouping = DataGrouping.EqualInterval;
            rule.ColorCount = 3;
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

            #endregion */

            #region инициализация параметров запроса

            years = UserParams.CustomParam("years");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            ReportPDFExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
        }

        private static Color GetCustomColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.Cyan;
                    }
                case 2:
                    {
                        return Color.LightSkyBlue;
                    }
                case 3:
                    {
                        return Color.Gold;
                    }
                case 4:
                    {
                        return Color.Peru;
                    }
                case 5:
                    {
                        return Color.Indigo;
                    }
                case 6:
                    {
                        return Color.PeachPuff;
                    }
                case 7:
                    {
                        return Color.MediumSlateBlue;
                    }
                case 8:
                    {
                        return Color.ForestGreen;
                    }
                case 9:
                    {
                        return Color.HotPink;
                    }
                case 10:
                    {
                        return Color.Gray;
                    }
                case 11:
                    {
                        return Color.Blue;
                    }
                case 12:
                    {
                        return Color.Magenta;
                    }
                case 13:
                    {
                        return Color.DarkBlue;
                    }
                case 14:
                    {
                        return Color.DarkRed;
                    }
                case 15:
                    {
                        return Color.DarkSalmon;
                    }
                case 16:
                    {
                        return Color.DarkOrange;
                    }
                case 18:
                    {
                        return Color.RosyBrown;
                    }
                case 19:
                    {
                        return Color.DarkGray;
                    }

            }
            return Color.White;
        }

        
        private void AddLineAppearencesUltraChart1()
        {
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart.ColorModel.Skin.ApplyRowWise = true;
            UltraChart.ColorModel.Skin.PEs.Clear();

            for (int i = 1; i <= 19; i++)
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
                            color = Color.Beige;
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
                    case 8:
                        {
                            color = Color.Yellow;
                            break;
                        }
                    case 9:
                        {
                            color = Color.Pink;
                            break;
                        }
                    case 10:
                        {
                            color = Color.Cyan;
                            break;
                        }
                    case 11:
                        {
                            color = Color.Magenta;
                            break;
                        }
                    case 12:
                        {
                            color = Color.Orange;
                            break;
                        }
                    case 13:
                        {
                            color = Color.Brown;
                            break;
                        }
                    case 14:
                        {
                            color = Color.PaleVioletRed;
                            break;
                        }
                    case 15:
                        {
                            color = Color.DarkKhaki;
                            break;
                        }
                    case 16:
                        {
                            color = Color.Olive;
                            break;
                        }
                    case 17:
                        {
                            color = Color.LightCoral;
                            break;
                        }
                    case 18:
                        {
                            color = Color.BlueViolet;
                            break;
                        }
                    case 19:
                        {
                            color = Color.Chocolate;
                            break;
                        }
                }
                pe.Fill = color;
                pe.StrokeWidth = 0;
                UltraChart.ColorModel.Skin.PEs.Add(pe);
                pe.Stroke = Color.Black;
                pe.StrokeWidth = 0;

               LineAppearance lineAppearance2 = new LineAppearance();
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

            if (!Page.IsPostBack)
            {
                ComboYear.Title = "Период";
                ComboYear.Width = 200;
                ComboYear.MultiSelect = false;
                ComboYear.ParentSelect = false;
                FillComboDate(ComboYear, "EO_0001_0001_Date", 0);
                ComboYear.SetСheckedState("2010 год", true);
            }

            string[] dateElements = ComboYear.SelectedValue.Split(' ');
            year = Convert.ToInt32(dateElements[0]);
            UserParams.PeriodYear.Value = year.ToString();

            string str = string.Empty;
            for (int i = year - 3; i <= year; i++)
            {
                if (i!=2008)
                {
                    str += string.Format("[Период__Период].[Период__Период].[Данные всех периодов].[{0}],", i);
                }
            }

            years.Value = "[Период__Период].[Период__Период].[Данные всех периодов].[2009],[Период__Период].[Период__Период].[Данные всех периодов].[2010], [Период__Период].[Период__Период].[Данные всех периодов].[2011] ";//str.TrimEnd(',');

            Page.Title = String.Format("Ежегодная оценка инвестиционной привлекательности муниципальных образований Сахалинской области");
            PageTitle.Text = Page.Title;
            //chart1Label.Text = "Распределение по объему профицита(+)/дефицита(-) местных бюджетов";
            PageSubTitle.Text = string.Format("Оценка инвестиционной привлекательности муниципальных образований за {0} год", year);

            headerLayout1 = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            UltraChart1.DataBind();

            Label2.Text = string.Format("Интегральный индекс инвестиционной привлекательности за {0}", ComboYear.SelectedValue); 
            UltraChart.DataBind();
            
            Label3.Text = string.Format("Интегральный и частные индексы инвестиционной привлекательности муниципальных образований за {0}", ComboYear.SelectedValue);

            DundasMap1.Shapes.Clear();
            DundasMap1.ShapeFields.Clear();
            DundasMap1.ShapeFields.Add("Name");
            DundasMap1.ShapeFields["Name"].Type = typeof(string);
            DundasMap1.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap1.ShapeFields.Add("Complete");
            DundasMap1.ShapeFields["Complete"].Type = typeof(double);
            DundasMap1.ShapeFields["Complete"].UniqueIdentifier = false;

            SetMapSettings();
            AddMapLayer(DundasMap1, mapFolderName, "Территор", CRHelper.MapShapeType.Areas);

            // заполняем карту данными
            FillMapData();
            
        }

        protected void FillComboDate(CustomMultiCombo combo, string queryName, int offset)
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Таблица", dtDate);
            if (dtDate.Rows.Count > 0)
            {
               Dictionary<string, int> dictDate = new Dictionary<string, int>();
               for (int numRow = 0; numRow < dtDate.Rows.Count; numRow++)
                {
                    if (dtDate.Rows[numRow][3].ToString() == "0")
                    {
                      dictDate.Add(dtDate.Rows[numRow][1].ToString(), 0);
                    }
                }
                combo.FillDictionaryValues(dictDate);
                
            }
        }


        #region Обработчики карты

        public void SetMapSettings()
        {
            DundasMap1.Meridians.Visible = false;
            DundasMap1.Parallels.Visible = false;
            DundasMap1.ZoomPanel.Visible = true;
            DundasMap1.NavigationPanel.Visible = true;
            DundasMap1.Viewport.EnablePanning = true;
            DundasMap1.Viewport.Zoom = (float)mapZoomValue;

            // добавляем легенду
            Legend legend = new Legend("CompleteLegend");
            legend.Visible = true;
            legend.DockAlignment = DockAlignment.Far;
            legend.BackColor = Color.White;
            legend.BackSecondaryColor = Color.Gainsboro;
            legend.BackGradientType = GradientType.DiagonalLeft;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Gray;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.Black;
            legend.Font = new System.Drawing.Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.Title = "Интегральный индекс\nинвестиционной привлекательности";
            legend.AutoFitMinFontSize = 7;
            DundasMap1.Legends.Clear();
            DundasMap1.Legends.Add(legend);
            /*if (useRegionCodes)
            {
                Legend regionsLegend = new Legend("RegionCodeLegend");
                regionsLegend.Dock = PanelDockStyle.Right;
                regionsLegend.Visible = true;
                regionsLegend.BackColor = Color.White;
                regionsLegend.BackSecondaryColor = Color.Gainsboro;
                regionsLegend.BackGradientType = GradientType.DiagonalLeft;
                regionsLegend.BackHatchStyle = MapHatchStyle.None;
                regionsLegend.BorderColor = Color.Gray;
                regionsLegend.BorderWidth = 1;
                regionsLegend.BorderStyle = MapDashStyle.Solid;
                regionsLegend.BackShadowOffset = 4;
                regionsLegend.TextColor = Color.Black;
                regionsLegend.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
                regionsLegend.AutoFitText = true;
                regionsLegend.Title = "Коды территорий";
                regionsLegend.AutoFitMinFontSize = 7;
                regionsLegend.ItemColumnSpacing = 100;
                SetLegendPosition(regionsLegend, LegendPosition.RightBottom);
                DundasMap1.Legends.Add(regionsLegend);

            }*/
            // добавляем правила раскраски
            DundasMap1.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "Complete";
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = 3;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Red;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Green;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";
            rule.LegendText = "#FROMVALUE{N3} - #TOVALUE{N3}";
            DundasMap1.ShapeRules.Add(rule);



        }

        /// <summary>
        /// Является ли форма городом-выноской
        /// </summary>
        /// <param name="shape">форма</param>
        /// <returns>true, если является</returns>
        public static bool IsCalloutTownShape(Shape shape)
        {
            return shape.Layer == CRHelper.MapShapeType.CalloutTowns.ToString();
        }

        /// <summary>
        /// Получение имени формы (с выделением имени из города-выноски)
        /// </summary>
        /// <param name="shape">форма</param>
        /// <returns>имя формы</returns>
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
            string layerName = Server.MapPath(string.Format("../../maps/Субъекты/{0}/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;
            string mapField = useRegionCodes ? "CODE" : "NAME";
            map.LoadFromShapeFile(layerName, "NAME", true);
            map.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
        }

        /// <summary>
        /// Поиск формы карты
        /// </summary>
        /// <param name="map">карта</param>
        /// <param name="patternValue">искомое имя формы</param>
        /// <returns>найденные формы</returns>
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
            string query = DataProvider.GetQueryText("EO_0001_0001_map");
            DataTable dtMap1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtMap1);

            if (dtMap1 == null || DundasMap1 == null)
            {
                return;
            }

            foreach (Shape shape in DundasMap1.Shapes)
            {
                string shapeName = GetShapeName(shape);
                if (!RegionsNamingHelper.LocalBudgetUniqueNames.ContainsKey(shapeName))
                {
                    shape.Visible = false;
                }
            }

            bool nullShapesExists = false;
            foreach (DataRow row in dtMap1.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != String.Empty)
                {
                    string subject = row[0].ToString().Replace("Город Южно-Сахалинск", "г. Южно - Сахалинск");

                    ArrayList shapeList = FindMapShape(DundasMap1, subject);
                    foreach (Shape shape in shapeList)
                    {
                        string shapeName = GetShapeName(shape);
                        string shapeCode = string.Empty;
                        if (useRegionCodes && !IsCalloutTownShape(shape) && RegionsNamingHelper.LocalBudgetCodes.ContainsKey(shapeName))
                        {
                            /*shapeCode = RegionsNamingHelper.LocalBudgetCodes[shapeName];

                            LegendItem item = new LegendItem();
                            LegendCell cell = new LegendCell(shapeCode);
                            cell.Alignment = System.Drawing.ContentAlignment.MiddleRight;
                            item.Cells.Add(cell);
                            cell = new LegendCell(shapeName);
                            cell.Alignment = System.Drawing.ContentAlignment.MiddleLeft;
                            item.Cells.Add(cell);

                            DundasMap1.Legends["RegionCodeLegend"].Items.Add(item);*/
                        }
                        if (row[1] != DBNull.Value && row[1].ToString() != String.Empty)
                        {
                            double investmentsBeauty = Convert.ToDouble(row[1]);
                            double investmentsRank = Convert.ToDouble(row[2]);

                            shape["Name"] = subject;
                            shape["Complete"] = investmentsBeauty;
                            shape.ToolTip = String.Format("{2} \n ранг по Сахалинской области: {0:N0} \n инвестиционная \n привлекательность: {1:N3}", investmentsRank, investmentsBeauty, shapeName.Replace("\"", "'"));
                            //shape.ToolTip = string.Format("#NAME \n ранг по Сахалинской области: {0:N0} \n инвестиционная \n привлекательность: {1:N1}",investmentsRank, investmentsBeauty);

                            /*if (IsCalloutTownShape(shape))
                            {
                               // shape.Text = String.Format("{0}\n{1:N1} тыс.руб./чел.", shapeName, value);
                                shape.TextVisibility = TextVisibility.Shown;
                                shape.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;

                                shape.CentralPointOffset.Y = mapCalloutOffsetY;
                            }
                            else
                            {
                                //shape.Text = String.Format("{0}\n{1:N1} тыс.руб./чел.", shapeName.Replace(" ", "\n"), value);
                            }*/
                            if (IsCalloutTownShape(shape))
                            {
                                shape.Text = string.Format(string.Format("{0}\n#COMPLETE{{N3}}", shapeName));
                                shape.TextVisibility = TextVisibility.Shown;
                                shape.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;
                                // shape.CentralPointOffset.Y = calloutOffsetY;
                            }
                            else
                            {
                                string shapeText = shapeName.Replace("Городской округ", "").Replace("городской округ","").Replace("район", ""); //useRegionCodes ? shapeCode : shapeName.Replace(" ", "\n");
                                shape.Text = string.Format("{0}\n#COMPLETE{{N3}}", shapeText);
                                shape.TextVisibility = TextVisibility.Shown;
                                //shape.TextVisibility = shapeTextShown ? TextVisibility.Shown : TextVisibility.Auto;
                                if (shapeText.Contains("Тымовский") || shapeText.Contains("Анивский") || shapeText.Contains("Корсаковский") || shapeText.Contains("Вахрушев") || shapeText.Contains("Невельский"))
                                {
                                    shape.CentralPointOffset.Y = -0.5;
                                }

                                if (shapeText.Contains("Углегорский") || shapeText.Contains("Тамаринский") || shapeText.Contains("Невельский"))
                                {
                                    shape.CentralPointOffset.X = -1;
                                }
                                if (shapeText.Contains("Долинский") || shapeText.Contains("Вахрушев") )
                                {
                                    shape.CentralPointOffset.X = 1;
                                }
                                if (shapeText.Contains("Южно - Сахалинск"))
                                {
                                    shape.CentralPointOffset.X = 2;
                                }
                            }
                        }
                        else
                        {
                            if (!nullShapesExists && DundasMap1.Legends.Count > 0)
                            {
                                LegendItem item = new LegendItem();
                                item.Text = "Нет долга";
                                item.Color = Color.SkyBlue;
                                DundasMap1.Legends[0].Items.Add(item);
                            }
                            nullShapesExists = true;

                            shape.Color = Color.SkyBlue;
                            shape.ToolTip = String.Format("{0}\nнет долга", shapeName.Replace(" ", "\n").Replace("\"", "'"));
                            //shape.Text = shapeName.Replace(" ", "\n");
                            string shapeText = useRegionCodes ? shapeCode : shapeName.Replace(" ", "\n");
                            shape.Text = string.Format("{0}\nнет долга", shapeText);

                            if (IsCalloutTownShape(shape))
                            {
                                shape.TextVisibility = TextVisibility.Shown;
                                shape.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;
                                shape.CentralPointOffset.Y = mapCalloutOffsetY;
                            }
                        }
                    }
                }
            }
        }
        private void SetLegendPosition(Legend legend, LegendPosition position)
        {
            switch (position)
            {
                case LegendPosition.LeftTop:
                    {
                        legend.DockAlignment = DockAlignment.Near;
                        legend.Dock = PanelDockStyle.Left;
                        break;
                    }
                case LegendPosition.LeftBottom:
                    {
                        legend.DockAlignment = DockAlignment.Far;
                        legend.Dock = PanelDockStyle.Left;
                        break;
                    }
                case LegendPosition.RightTop:
                    {
                        legend.DockAlignment = DockAlignment.Near;
                        legend.Dock = PanelDockStyle.Right;
                        break;
                    }
                case LegendPosition.RightBottom:
                    {
                        legend.DockAlignment = DockAlignment.Far;
                        legend.Dock = PanelDockStyle.Right;
                        break;
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
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Территория", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                count = Convert.ToInt32(dtGrid.Rows[0][dtGrid.Columns.Count - 1]);

                for (int numCol = 1; numCol< dtGrid.Columns.Count; numCol++)
                {
                    dtGrid.Columns[numCol].ColumnName = dtGrid.Columns[numCol].ColumnName.Replace("\"", "'");
                }
                
                Collection<int> removingRows = new Collection<int>();
                for (int i = 0; i < dtGrid.Rows.Count; i++)
                {
                    if (dtGrid.Rows[i][0].ToString().Contains("Ранг по"))
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
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(195);

            
            for (int i = 1; i < count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(200);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
            }

            headerLayout1.AddCell("Территория");

          
            for (int i = 1; i <= count; i++)
            {
                string[] caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');

                headerLayout1.AddCell(string.Format("{0} <br/> Значение (ранг)", caption[0]));
               
                e.Layout.Bands[0].Columns[i].Width = 120;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N3");
             }

            for (int i = count+1; i < UltraWebGrid.Columns.Count; i++ )
            {
                e.Layout.Bands[0].Columns[i].Hidden = true;
            }

            headerLayout1.ApplyHeaderInfo();
            
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
          /*  if (e.Row.Index == 0)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }
            */
            if (e.Row.Cells[e.Row.Cells.Count - 2].Value != null && e.Row.Cells[e.Row.Cells.Count - 2].Value.ToString() != string.Empty && e.Row.Cells[e.Row.Cells.Count - 2].Value.ToString() =="0")
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }

            for (int i = 1; i <= count; i++)
            {
                if (e.Row.Cells[i+count].Value != null)
                {
                    if (e.Row.Cells[i+count].ToString() == "1")
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N3} (<img src='../../images/StarYellowBB.png'>{1:N0})", e.Row.Cells[i].Value, e.Row.Cells[i + count].Value);
                        e.Row.Cells[i].Title = "Самый высокий индекс";
                    }
                    else if (e.Row.Cells[i+count].ToString() == "19")
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N3} (<img src='../../images/StarGrayBB.png'>{1:N0})", e.Row.Cells[i].Value, e.Row.Cells[i + count].Value);
                        e.Row.Cells[i].Title = "Самый низкий индекс";
                    }
                    else
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N3} ({1:N0})", e.Row.Cells[i].Value, e.Row.Cells[i + count].Value);
                        
                    }
                }
            }

        }

       #endregion

        #region Обработчики диаграмм

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("EO_0001_0001_chart1");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            dtChart.Columns.RemoveAt(0);
            dtChart.AcceptChanges();

            if (dtChart.Rows.Count > 0)
            {
                for (int numCol = 0; numCol < dtChart.Rows.Count; numCol++)
                {
                    dtChart.Rows[numCol][0] = dtChart.Rows[numCol][0].ToString().Replace("\"", "'");
                    dtChart.Rows[numCol][0] = dtChart.Rows[numCol][0].ToString().Replace("городской округ", "ГО").Replace("Городской округ", "ГО").Replace("Город", "г.").Replace("район", "р-н");
                }
                /*
                for (int numRow = 0; numRow < dtChart.Rows.Count; numRow++)
                {
                    dtChart.Rows[numRow][0] = string.Format("{0} {1} г.", dtChart.Rows[numRow][0], year );
                }
                */

                UltraChart1.DataSource = dtChart;
            }
        }

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("EO_0001_0001_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            if (dtChart.Rows.Count > 0)
            {
                for (int numRow = 0; numRow < dtChart.Rows.Count; numRow ++ )
                {
                    dtChart.Rows[numRow][0] = dtChart.Rows[numRow][0].ToString().Replace("\"", "'");
                    dtChart.Rows[numRow][0] = dtChart.Rows[numRow][0].ToString().Replace("городской округ","ГО").Replace("Городской округ","ГО").Replace("Город", "г.").Replace("район", "р-н");
                }

                UltraChart.DataSource = dtChart;
            }
        }

        private int GetMaxRowIndex(string col)
        {
            int result = 0;
            double value = double.MinValue;
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
            double value = double.MaxValue;
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
                    text.labelStyle.HorizontalAlign = StringAlignment.Center;
                    text.labelStyle.FontSizeBestFit = false;
                    text.labelStyle.Font = new Font("Verdana", 8);
                    text.labelStyle.WrapText = true;

                    string s = text.GetTextString();
                    if (s.Contains("Интегральный индекс инвестиционной привлекательности"))
                    {
                        text.bounds.X = 40;
                    }

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
                int plus;
                if (i==0)
                {
                    plus = 10;
                }
                else
                {
                    plus = 10;
                }

                Text newMaxText = new Text();
                newMaxText.labelStyle.Font = new Font("Verdana", 8);
                newMaxText.PE.Fill = Color.Black;
                newMaxText.bounds = new Rectangle(leftBound[i] + plus, (int)yAxis.Map(maxValue[i]) - 20, 120,15);
                newMaxText.labelStyle.VerticalAlign = StringAlignment.Center;
                // newText.labelStyle.HorizontalAlign = StringAlignment.Center;
                newMaxText.SetTextString(maxRegions[i].Replace("ГО", "").Replace("р-н", "").Replace("'", ""));
                e.SceneGraph.Add(newMaxText);

                Text newMinText = new Text();
                newMinText.labelStyle.Font = new Font("Verdana", 8);
                newMinText.PE.Fill = Color.Black;
               if (minRegions[i].Contains("Александровск-Сахалинский"))
               {
                   CRHelper.SaveToErrorLog("OK");
                   newMinText.bounds = new Rectangle(leftBound[i]-50, (int)yAxis.Map(minValue[i])+10, 200, 15);
               }
               else
               {
                   newMinText.bounds = new Rectangle(leftBound[i] + plus, (int)yAxis.Map(minValue[i]) + 4, 120, 15);
               }
                newMinText.labelStyle.VerticalAlign = StringAlignment.Center;
                // newText.labelStyle.HorizontalAlign = StringAlignment.Center;
                newMinText.SetTextString(minRegions[i].Replace("ГО", "").Replace("р-н", "").Replace("'", ""));
                e.SceneGraph.Add(newMinText);
            }
        }

        #endregion

        #region Экспорт

        #region Экспорт в Excel

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.ScalingFactor = 40;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма 1");
            Worksheet sheet3 = workbook.Worksheets.Add("Карта");
            Worksheet sheet4 = workbook.Worksheets.Add("Диаграмма2");

            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            ReportExcelExporter1.RowsAutoFitEnable = true;

            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            for (int j = 0; j < UltraWebGrid.Rows.Count; j++)
            {
                for (int i = 1; i < UltraWebGrid.Columns.Count-1; i++)
                {
                    if (UltraWebGrid.Rows[j].Cells[i].Value != null && UltraWebGrid.Rows[j].Cells[i].Value.ToString() != string.Empty)
                    {
                        UltraWebGrid.Rows[j].Cells[i].Value = UltraWebGrid.Rows[j].Cells[i].Value.ToString().Replace(
                            "<img src='../../images/StarGrayBB.png'>", "").Replace(
                                "<img src='../../images/StarYellowBB.png'>", "");
                    }
                }
            }

            ReportExcelExporter1.Export(headerLayout1, sheet1, 3);
            ReportExcelExporter1.WorksheetTitle = "";
            ReportExcelExporter1.WorksheetSubTitle = "";
            ReportExcelExporter1.Export(UltraChart1, Label1.Text, sheet2, 3);
            ReportExcelExporter1.Export(DundasMap1, Label2.Text, sheet3, 3);
            ReportExcelExporter1.Export(UltraChart, Label3.Text, sheet4, 3);
        }

        #endregion

        #region Экспорт в Pdf

        void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();
            ISection section4 = report.AddSection();

            ReportPDFExporter1.HeaderCellHeight = 60;

            for (int j = 0; j < UltraWebGrid.Rows.Count; j++)
            {
                for (int i = 1; i < UltraWebGrid.Columns.Count; i++)
                {
                    if (UltraWebGrid.Rows[j].Cells[i].Value != null && UltraWebGrid.Rows[j].Cells[i].Value.ToString() != string.Empty )
                    {
                      UltraWebGrid.Rows[j].Cells[i].Value = UltraWebGrid.Rows[j].Cells[i].Value.ToString().Replace("<img src='../../images/StarGrayBB.png'>", "").Replace("<img src='../../images/StarYellowBB.png'>", "");
                    }
                }
            }
            ReportPDFExporter1.Export(headerLayout1, section1);
            UltraChart1.Width = 1000;
            UltraChart1.Legend.SpanPercentage = 30;
            ReportPDFExporter1.Export(UltraChart1, Label1.Text, section2);
            DundasMap1.NavigationPanel.Visible = false;
            DundasMap1.ZoomPanel.Visible = false;
            DundasMap1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            ReportPDFExporter1.Export(DundasMap1, Label2.Text, section3);
            UltraChart.Width = 1050;
            UltraChart.Legend.SpanPercentage = 30;
            ReportPDFExporter1.Export(UltraChart, Label3.Text, section4);
        }

        #endregion

        #endregion
    }
}

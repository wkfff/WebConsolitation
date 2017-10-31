using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Drawing;
using Microsoft.AnalysisServices.AdomdClient;
using Microsoft.VisualBasic;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebNavigator;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;
using Infragistics.WebUI.UltraWebGrid;
using Dundas.Maps.WebControl;
using System.Globalization;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report;
using System.Text.RegularExpressions;
using Infragistics.WebUI.Misc; 

using System.Collections.ObjectModel;
using System.Text;
using System.Net;
using System.IO;
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Image = Infragistics.Documents.Reports.Graphics.Image;
using Graphics = System.Drawing.Graphics;
using TextAlignment = Infragistics.Documents.Reports.Report.TextAlignment;
using System.Drawing.Imaging;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Server.Dashboards.reports.STAT_0003_0010
{
    public partial class _default : CustomReportPage
    {
        private string page_title = "Мониторинг видов проведенных операций (по состоянию на выбранную дату)";
        private string sub_page_title = "Данные ежегодного мониторинга видов проведенных операций в ХМАО-Югре, {0} год";
        private string map_title = "{0}, в {1} году по виду операции «{2}», единиц";
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam selectedDate { get { return (UserParams.CustomParam("selectedDate")); } }
        private CustomParam mapPok { get { return (UserParams.CustomParam("mapPok")); } }
        private CustomParam operationType { get { return (UserParams.CustomParam("operationType")); } }
        private string legendTitle = "";
        private string style = "";
        private object[] edIsm;
        private string mapFolderName = "";
        private GridHeaderLayout headerLayout;
        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1200; }
        }
        private int minScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }

        private int minScreenHeight
        {
            get { return IsSmallResolution ? 700 : CustomReportConst.minScreenHeight; }
        }
        public static bool IsCalloutTownShape(Shape shape)
        {
            return shape.Layer == CRHelper.MapShapeType.Towns.ToString();
        }
        private static bool IsMozilla
        {
            get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
        }
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            Grid.Height = Unit.Empty;
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.8);
            ComboDate.Width = 200;
            comboOperationType.Width = 350;
            if (IsSmallResolution)
            {
                Grid.Width = minScreenWidth;
                DundasMap.Width = minScreenWidth - 5;
            }
            else
            {
                Grid.Width = (int)(minScreenWidth - 15);
                DundasMap.Width = (int)(minScreenWidth - 20);
            }
            Label1.Text = page_title;
            Page.Title = page_title;
            CrossLink1.Text = "Мониторинг видов проведенных операций (по муниципальным образованиям)";
            CrossLink1.NavigateUrl = "~/reports/STAT_0003_0009/default.aspx";
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            baseRegion.Value = "[Территории__РФ].[Территории__РФ].[Все территории].[Российская Федерация].[Уральский федеральный округ].[Тюменская область с Ханты-Мансийским автономным округом, Ямало-Ненецким автономным округом].[Ханты-Мансийский автономный округ - Югра]";
            if (!Page.IsPostBack)
            {
                ComboDate.Title = "Период";
                ComboDate.FillDictionaryValues(DateLoad("Date"));
                ComboDate.SelectLastNode();
                comboOperationType.Title = "Вид операции";
                comboOperationType.FillDictionaryValues(OperationsLoad("operations"));
            }
            mapFolderName = "Субъекты/ХМАОDeer";
            operationType.Value = "[Здравоохранение__Виды операций].[Здравоохранение__Виды операций].[Все виды операций].["+comboOperationType.SelectedValue+"]";
            selectedDate.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[" + ComboDate.SelectedValue.Split(' ')[0] + "]";
            mapPok.Value = "[Здравоохранение__Заболеваемость населения].[Здравоохранение__Заболеваемость населения].[Все показатели].[Число проведенных операций]";
            headerLayout = new GridHeaderLayout(Grid);
            Grid.DataBind();
            if (Grid.DataSource != null)
            {
                FormText();
                Grid.Rows[0].Hidden = true;
                Grid.Rows[1].Hidden = true;
                Grid.Rows[2].Hidden = true;
                //SetSfereparam();
                SetMapSettings();
            }
            else
            {
                Label4.Text = "Нет данных";
            }
            Label2.Text = String.Format(sub_page_title, ComboDate.SelectedValue.Split(' ')[0]);
            Label3.Text = String.Format(map_title, "Число операций, проведенных в стационаре", ComboDate.SelectedValue.Split(' ')[0], comboOperationType.SelectedValue.ToLower());
        }

        Dictionary<string, int> OperationsLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.SpareMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 0; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
            }
            return d;
        }

        Dictionary<string, int> DateLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.SpareMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 0; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption + " год", 0);
            }
            return d;
        }



        protected string GenitiveTransform(double number, string s)
        {
            number = Math.Abs(number);
            if ((number % 10 == 1) && ((number % 100 == 1) || (number % 100 >= 21)))
            {

                if (s.EndsWith("ц"))
                {
                    return s + "у";
                }
                else
                {
                    return s + "а";
                }

            }
            else
            {
                if ((number % 10 <= 4) && (number % 10 != 0) && ((number % 100).ToString()[0] != '1'))
                {

                    if (s.EndsWith("ц"))
                    {
                        return s + "ы";
                    }
                    else
                    {
                        return s + "а";
                    }
                }
                else
                {

                    if (s.EndsWith("ц"))
                    {
                        return s;
                    }
                    else
                    {
                        return s;
                    }
                }
            }
        }

        protected string GenitiveTransform1(double number, string s)
        {
            number = Math.Abs(number);
            if ((number % 10 == 1) && ((number % 100 == 1) || (number % 100 >= 21)))
            {
                if (s.EndsWith("ц"))
                {
                    return s + "у";
                }
                else
                {
                    return s;
                }
            }
            else
            {
                if ((number % 10 <= 4) && (number % 10 != 0) && ((number % 100).ToString()[0] != '1'))
                {
                    if (s.EndsWith("ц"))
                    {
                        return s + "ы";
                    }
                    else
                    {
                        return s + "а";
                    }
                }
                else
                {
                    if (s.EndsWith("ц"))
                    {
                        return s;
                    }
                    else
                    {
                        return s;
                    }
                }
            }
        }


        #region Обработчики грида
        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid"), "Виды операций", dt);
            if (dt.Rows.Count > 1)
            {

                edIsm = new object[dt.Rows[0].ItemArray.Length];
                for (int i = 0; i < dt.Rows[0].ItemArray.Length; i++)
                {
                    if (dt.Rows[0].ItemArray[i].ToString().EndsWith("а"))
                    {
                        edIsm[i] = dt.Rows[0].ItemArray[i].ToString().Remove(dt.Rows[0].ItemArray[i].ToString().Length - 1);
                    }
                    else
                    {
                        edIsm[i] = dt.Rows[0].ItemArray[i];
                    }
                }
                dt.Rows.Remove(dt.Rows[0]);
                Grid.DataSource = dt;
            }
            else
            {
                Grid.DataSource = null;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.15);
            e.Layout.Bands[0].Columns[0].Header.Caption = "Виды операций";
            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = true;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            GridHeaderCell header = headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.13);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:5px";
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
                e.Layout.Bands[0].Columns[i].Header.Caption = e.Layout.Bands[0].Columns[i].Key + ", " + edIsm[i].ToString().ToLower();

                header = headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
            }
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (IsSmallResolution)
            { style = "background-repeat: no-repeat;background-position:30px"; }
            else
            { style = "background-repeat: no-repeat;background-position: 90px"; }

            if ((e.Row.Index + 1) % 3 == 0)
            {

                e.Row.Cells[0].Text = "";
                Grid.Rows[e.Row.Index - 1].Cells[0].Text = Grid.Rows[e.Row.Index - 2].Cells[0].Text.Split(';')[0];
                Grid.Rows[e.Row.Index - 2].Cells[0].Text = "";

                Grid.Rows[e.Row.Index - 2].Cells[0].Style.BackColor = Color.White;
                Grid.Rows[e.Row.Index - 1].Cells[0].Style.BackColor = Color.White;
                Grid.Rows[e.Row.Index - 1].Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
                Grid.Rows[e.Row.Index - 2].Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
                e.Row.Cells[0].Style.BackColor = Color.White;

                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    Grid.Rows[e.Row.Index - 2].Cells[i].Style.BorderDetails.StyleBottom = BorderStyle.None;
                    Grid.Rows[e.Row.Index - 1].Cells[i].Style.BorderDetails.StyleBottom = BorderStyle.None;
                    Grid.Rows[e.Row.Index - 2].Cells[i].Style.BackColor = Color.White;
                    Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackColor = Color.White;
                    e.Row.Cells[i].Style.BackColor = Color.White;
                    if (ComboDate.SelectedIndex != 0)
                    {
                        if (Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value) < 0)
                        {
                            Grid.Rows[e.Row.Index - 1].Cells[i].Title = "Снижение относительно " + ComboDate.SelectedNode.PrevNode.Text.Split(' ')[0] + " года";
                            if (Grid.Columns[i].Key.StartsWith("Умерло"))
                            {
                                Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            }
                            else
                            {
                                Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            }
                            Grid.Rows[e.Row.Index - 1].Cells[i].Style.CustomRules = style;

                        }
                        if (Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value) > 0)
                        {
                            Grid.Rows[e.Row.Index - 1].Cells[i].Title = "Рост относительно " + ComboDate.SelectedNode.PrevNode.Text.Split(' ')[0] + " года";
                            if (Grid.Columns[i].Key.StartsWith("Умерло"))
                            {
                                Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                            }
                            else
                            {
                                Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            }
                            Grid.Rows[e.Row.Index - 1].Cells[i].Style.CustomRules = style;

                        }

                        if ((e.Row.Cells[i].Value != null))
                        {
                            e.Row.Cells[i].Title = "Темп прироста к " + ComboDate.SelectedNode.PrevNode.Text.Split(' ')[0] + " году";
                            e.Row.Cells[i].Value = String.Format("{0:P2}", Convert.ToDouble(e.Row.Cells[i].Value));
                        }
                    }
                }
            }
        }
        #endregion

        #region Обработчики карты
        DataTable dtMap;
        bool smallLegend = false;
        bool revPok = false;
        public void SetMapSettings()
        {
            if (legendTitle == "")
            {
                legendTitle = "Число операций, проведенных в стационаре, " + edIsm[1].ToString().ToLower();
            }
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeRules.Clear();
            DundasMap.Shapes.Clear();
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.Viewport.Zoom = 98;
            DundasMap.ColorSwatchPanel.Visible = false;

            DundasMap.Legends.Clear();
            // добавляем легенду
            Legend legend = new Legend("CostLegend");
            legend.Visible = true;
            legend.BackColor = Color.White;
            legend.Dock = PanelDockStyle.Right;
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
            legend.Title = legendTitle;
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Add(legend);
            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();

            dtMap = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Map"), "Карта", dtMap);
            double[] values = new double[dtMap.Rows.Count];
            for (int i = 0; i < dtMap.Rows.Count; i++)
            {
                values[i] = Convert.ToDouble(dtMap.Rows[i].ItemArray[1]);
            }
            Array.Sort(values);
            if (legendTitle.StartsWith("Умерло после"))
            {
                revPok = true;
            }
            else
            {
                revPok = false;
            }
            if (Math.Abs(values[0] - values[values.Length - 1]) <= 4)
            {
                smallLegend = true;
                LegendItem item = new LegendItem();
                item.Text = String.Format("0");
                item.Color = GetColor(0);
                DundasMap.Legends["CostLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("1");
                item.Color = GetColor(1);
                DundasMap.Legends["CostLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("2");
                item.Color = GetColor(2);
                DundasMap.Legends["CostLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("3");
                item.Color = GetColor(3);
                DundasMap.Legends["CostLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("4");
                item.Color = GetColor(4);
                DundasMap.Legends["CostLegend"].Items.Add(item);
            }
            else
            {
                smallLegend = false;
                ShapeRule rule = new ShapeRule();
                rule.Name = "CostRule";
                rule.Category = String.Empty;
                rule.ShapeField = "Cost";
                rule.DataGrouping = DataGrouping.EqualDistribution;
                rule.ColorCount = 5;
                rule.ColoringMode = ColoringMode.ColorRange;
                if (revPok)
                {
                    rule.FromColor = Color.Green;
                    rule.MiddleColor = Color.Yellow;
                    rule.ToColor = Color.Red;
                }
                else
                {
                    rule.FromColor = Color.Red;
                    rule.MiddleColor = Color.Yellow;
                    rule.ToColor = Color.Green;
                }
                rule.BorderColor = Color.FromArgb(50, Color.Black);
                rule.GradientType = GradientType.None;
                rule.HatchStyle = MapHatchStyle.None;
                rule.ShowInLegend = "CostLegend";

                rule.LegendText = "#FROMVALUE{N0} - #TOVALUE{N0}";
                DundasMap.ShapeRules.Add(rule);

            }

            // добавляем поля
            DundasMap.Shapes.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Cost");
            DundasMap.ShapeFields["Cost"].Type = typeof(double);
            DundasMap.ShapeFields["Cost"].UniqueIdentifier = false;

            // добавляем правила расстановки символов

            // звезды для легенды


            AddMapLayer(DundasMap, mapFolderName, "Граница", CRHelper.MapShapeType.Areas);
            AddMapLayer(DundasMap, mapFolderName, "Территор", CRHelper.MapShapeType.CalloutTowns);
            FillMapData();

        }

        public static ArrayList FindMapShape(MapControl map, string patternValue, out bool hasCallout)
        {
            hasCallout = false;
            ArrayList shapeList = new ArrayList();
            foreach (Shape shape in map.Shapes)
            {
                if (shape.Name.ToLower() == patternValue.ToLower())
                {
                    shapeList.Add(shape);
                    if (IsCalloutTownShape(shape))
                    {
                        hasCallout = true;
                    }
                }
                else
                {
                    shape.TextVisibility = TextVisibility.Shown;
                }
            }

            return shapeList;
        }

        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(string.Format("../../maps/{0}/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;

            map.LoadFromShapeFile(layerName, "Name", true);
            map.Layers.Clear();
            map.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
        }

        public void FillMapData()
        {
            bool hasCallout;
            string valueSeparator = IsMozilla ? ". " : "\n";
            string shapeHint = "";

            if (mapPok.Value.Contains("Число проведенных операций"))
            {
                shapeHint = "Число операций, проведенных в стационаре {0}";
            }
            if (mapPok.Value.Contains("Число проведенных операций с применением высоких медицинских технологий"))
            {
                shapeHint = "Число операций, проведенных в стационаре, с применением высоких медицинских технологий {0}";
            }
            if (mapPok.Value.Contains("Умерло после проведенных операци"))
            {
                shapeHint = "Умерло после операций, проведенных в стационаре {0}";
            }
            if (mapPok.Value.Contains("Умерло после проведенных операций с применением высоких медицинских технологий"))
            {
                shapeHint = "Умерло после операций, проведенных в стационаре, с применением высоких медицинских технологий {0}";
            }

            Dictionary<string, int> colTown = new Dictionary<string, int>();
            int indTown = 1;
            for (int i = 0; i < dtMap.Rows.Count; i++)
            {
                if (dtMap.Rows[i].ItemArray[0].ToString().StartsWith("Город"))
                {
                    colTown.Add(dtMap.Rows[i].ItemArray[0].ToString().Replace(" муниципальный район", " р-н").Replace("Город ", "г. "), indTown);
                    indTown += 1;
                }
            }
            for (int j = 0; j < dtMap.Rows.Count; j++)
            {

                string subject = dtMap.Rows[j].ItemArray[0].ToString().Replace(" муниципальный район", " р-н").Replace("Город ", "г. ");
                double value = Convert.ToDouble(dtMap.Rows[j].ItemArray[1].ToString());
                ArrayList shapeList = FindMapShape(DundasMap, subject, out hasCallout);
                foreach (Shape shape in shapeList)
                {
                    shape.Visible = true;
                    shape["Name"] = subject;
                    shape["Cost"] = Convert.ToDouble(dtMap.Rows[j].ItemArray[1]);
                    if (subject.StartsWith("г."))
                    {
                        shape.ToolTip = String.Format(shapeHint, String.Format("{0:0}", Convert.ToDouble(dtMap.Rows[j].ItemArray[1]))) + ", " + subject.Split('.')[1];
                        if (shape.Name.Contains("Нижневартовск") || shape.Name.Contains("Ханты-Мансийск") || shape.Name.Contains("Сургут") || shape.Name.Contains("Нефтеюганск"))
                        {
                            shape.CentralPointOffset.Y += 5000;
                        }
                        else
                        {
                            shape.CentralPointOffset.Y += 2100;
                        }
                        shape.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
                    }
                    else
                    {
                        shape.ToolTip = String.Format(shapeHint, String.Format("{0:0}", Convert.ToDouble(dtMap.Rows[j].ItemArray[1]))) + ", " + subject.Replace("р-н", String.Empty) + "МР";
                    }
                    shape.Text = subject + "\n" + String.Format("{0:0}", Convert.ToDouble(dtMap.Rows[j].ItemArray[1]));
                    shape.Name = subject;
                    shape.TextVisibility = TextVisibility.Shown;
                    if (smallLegend)
                    {
                        shape.Color = GetColor(Convert.ToDouble(dtMap.Rows[j].ItemArray[1]));
                    }
                }
            }
        }

        Color GetColor(Double val)
        {
            if (val < 1)
            {
                if (revPok)
                {
                    return Color.Green;
                }
                else
                {
                    return Color.Red;
                }
            }

            if (val < 2)
            {
                if (revPok)
                {
                    return Color.FromArgb(128, 192, 0);
                }
                else
                {
                    return Color.Orange;
                }
            }
            if (val < 3)
            {

                return Color.Yellow;

            }
            if (val < 4)
            {
                if (revPok)
                {
                    return Color.Orange;
                }
                else
                {
                    return Color.FromArgb(128, 192, 0);
                }
            }
            if (revPok)
            {
                return Color.Red;
            }
            else
            {
                return Color.Green;
            }

        }
        #endregion

        #region Добавление checksbox
        static int ra = 1;
        void SetSfereparam()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            //DataTable dt = new DataTable();
            string[] buttonNames = new string[4];
            buttonNames[0] = "Число операций, проведенных в стационаре";
            buttonNames[1] = "Число операций, проведенных в стационаре, с применением высоких медицинских технологий";
            buttonNames[2] = "Умерло после операций, проведенных в стационаре";
            buttonNames[3] = "Умерло после операций, проведенных в стационаре, с применением высоких медицинских технологий";
            //   DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("AreasChart"), "dfdf", dt);
            if (PlaceHolder1.Controls.Count != 0)
            {
                RadioButton r1 = (RadioButton)(PlaceHolder1.Controls[0]);
                if (r1.Text != buttonNames[0])
                {
                    PlaceHolder1.Controls.Clear();
                    for (int i = 0; i < buttonNames.Length; i++)
                    {
                        Random r = new Random();
                        ra = ra++;
                        RadioButton rb = new RadioButton();
                        rb.Style.Add("font-size", "10pt");
                        rb.ID = "s" + ra.ToString() + "a" + i.ToString();//CRHelper.GetRandomColor().A.ToString() + CRHelper.GetRandomColor().B.ToString();
                        rb.Style.Add("font-family", "Verdana");
                        PlaceHolder1.Controls.Add(rb);
                        Label l = new Label();
                        l.Text = "<br>";
                        PlaceHolder1.Controls.Add(l);
                        rb.Text = buttonNames[i];
                        rb.GroupName = "sfere" + ra.ToString();
                        rb.ValidationGroup = rb.GroupName;
                        rb.CheckedChanged += new EventHandler(RadioButton1_CheckedChanged);
                        rb.AutoPostBack = 1 == 1;
                        rb.Checked = 1 == 2;
                    }
                    ((RadioButton)(PlaceHolder1.Controls[0])).Checked = true;
                }
            }
            else
            {
                PlaceHolder1.Controls.Clear();
                for (int i = 0; i < buttonNames.Length; i++)
                {
                    Random r = new Random();
                    ra = ra++;
                    RadioButton rb = new RadioButton();
                    rb.Style.Add("font-size", "10pt");

                    rb.ID = "s" + ra.ToString() + "a" + i.ToString();//CRHelper.GetRandomColor().A.ToString() + CRHelper.GetRandomColor().B.ToString();
                    rb.Style.Add("font-family", "Verdana");
                    PlaceHolder1.Controls.Add(rb);
                    Label l = new Label();
                    l.Text = "<br>";
                    PlaceHolder1.Controls.Add(l);
                    rb.Text = buttonNames[i];
                    rb.GroupName = "sfere" + ra.ToString();
                    rb.ValidationGroup = rb.GroupName;
                    rb.CheckedChanged += new EventHandler(RadioButton1_CheckedChanged);
                    rb.AutoPostBack = 1 == 1;
                    rb.Checked = 1 == 2;
                }
                ((RadioButton)(PlaceHolder1.Controls[0])).Checked = true;
            }
        }

        protected void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)(sender);
            rb.Checked = 1 == 1;


            if (rb.Text == "Число операций, проведенных в стационаре")
            {
                mapPok.Value = "[Здравоохранение__Заболеваемость населения].[Здравоохранение__Заболеваемость населения].[Все показатели].[Число проведенных операций]";
                legendTitle = rb.Text + ", " + edIsm[1].ToString().ToLower();

            }
            if (rb.Text == "Число операций, проведенных в стационаре, с применением высоких медицинских технологий")
            {
                mapPok.Value = "[Здравоохранение__Заболеваемость населения].[Здравоохранение__Заболеваемость населения].[Все показатели].[Число проведенных операций с применением высоких медицинских технологий]";
                legendTitle = "Число операций, проведенных в стационаре,\n с применением высоких медицинских технологий, " + edIsm[2].ToString().ToLower();

            }
            if (rb.Text == "Умерло после операций, проведенных в стационаре")
            {
                mapPok.Value = "[Здравоохранение__Заболеваемость населения].[Здравоохранение__Заболеваемость населения].[Все показатели].[Умерло после проведенных операци]";
                legendTitle = rb.Text + ", " + edIsm[3].ToString().ToLower();

            }
            if (rb.Text == "Умерло после операций, проведенных в стационаре, с применением высоких медицинских технологий")
            {
                mapPok.Value = "[Здравоохранение__Заболеваемость населения].[Здравоохранение__Заболеваемость населения].[Все показатели].[Умерло после проведенных операций с применением высоких медицинских технологий]";
                legendTitle = "Умерло после операций, проведенных в стационаре,\n с применением высоких медицинских технологий, " + edIsm[4].ToString().ToLower();

            }
            Label3.Text = String.Format(map_title, rb.Text, ComboDate.SelectedValue.Split(' ')[0], comboOperationType.SelectedValue.ToLower());
            SetMapSettings();
        }
        #endregion

        #region Формирование текста
        protected void FormText()
        {
            Label4.Text = "";
            string buf = "";
            string str1 = "Общее {0} в целом по ХМАО-Югре в <b>{1} году</b> ";
            string str2 = " по сравнению с <b>{0} годом</b>";
            string str3 = "{0} на <b>{1}</b> {2} и";
            string str4 = " составило <b>{0}</b> {1}";

            for (int i = 1; i < Grid.Columns.Count; i++)
            {

                buf = "";
                if (ComboDate.SelectedIndex != 0)
                {

                    if (Grid.Columns[i].Key.Remove(8) == Grid.Columns[i - 1].Key.Remove(8))
                    {
                        buf += ", в том числе " + String.Format(str1, Grid.Columns[i].Key.ToLower(), ComboDate.SelectedValue.Split(' ')[0]).ToLower();
                    }
                    else
                    {
                        buf += String.Format(str1, Grid.Columns[i].Key.ToLower(), ComboDate.SelectedValue.Split(' ')[0]);
                    }
                    buf += String.Format(str2, ComboDate.SelectedNode.PrevNode.Text.Split(' ')[0]);

                    if (Convert.ToDouble(Grid.Rows[1].Cells[i].Value) > 0)
                    {
                        if (Grid.Columns[i].Key.StartsWith("Умерло после"))
                        {
                            buf += String.Format(str3, "&nbsp;<img src='../../images/arrowRedUpBB.png'>выросло", Math.Abs(Convert.ToDouble(Grid.Rows[1].Cells[i].Value)), GenitiveTransform(Convert.ToDouble(Grid.Rows[1].Cells[i].Value), Grid.Columns[i].Header.Caption.Split(',')[1]));
                        }
                        else
                        {
                            buf += String.Format(str3, "&nbsp;<img src='../../images/arrowGreenUpBB.png'>выросло", Math.Abs(Convert.ToDouble(Grid.Rows[1].Cells[i].Value)), GenitiveTransform(Convert.ToDouble(Grid.Rows[1].Cells[i].Value), Grid.Columns[i].Header.Caption.Split(',')[1]));
                        }
                    }
                    else
                    {
                        if (Convert.ToDouble(Grid.Rows[1].Cells[i].Value) < 0)
                        {
                            if (Grid.Columns[i].Key.StartsWith("Умерло после"))
                            {
                                buf += String.Format(str3, "&nbsp;<img src='../../images/arrowGreenDownBB.png'>уменьшилось", Math.Abs(Convert.ToDouble(Grid.Rows[1].Cells[i].Value)), GenitiveTransform(Convert.ToDouble(Grid.Rows[1].Cells[i].Value), Grid.Columns[i].Header.Caption.Split(',')[1]));
                            }
                            else
                            {
                                buf += String.Format(str3, "&nbsp;<img src='../../images/arrowGreenDownBB.png'>уменьшилось", Math.Abs(Convert.ToDouble(Grid.Rows[1].Cells[i].Value)), GenitiveTransform(Convert.ToDouble(Grid.Rows[1].Cells[i].Value), Grid.Columns[i].Header.Caption.Split(',')[1]));
                            }
                        }
                        else
                        {
                            buf += " не изменилось";
                        }


                    }
                    buf += String.Format(str4, Grid.Rows[0].Cells[i].Text, GenitiveTransform1(Convert.ToDouble(Grid.Rows[0].Cells[i].Value), Grid.Columns[i].Header.Caption.Split(',')[1]));
                }
                else
                {
                    if (Grid.Columns[i].Key.Remove(8) == Grid.Columns[i - 1].Key.Remove(8))
                    {
                        buf += ", в том числе " + String.Format(str1, Grid.Columns[i].Key.ToLower(), ComboDate.SelectedValue.Split(' ')[0]).ToLower();
                        buf += String.Format(str4, Grid.Rows[0].Cells[i].Text, GenitiveTransform1(Convert.ToDouble(Grid.Rows[0].Cells[i].Value), Grid.Columns[i].Header.Caption.Split(',')[1]));
                    }
                    else
                    {
                        buf += String.Format(str1, Grid.Columns[i].Key.ToLower(), ComboDate.SelectedValue.Split(' ')[0]);
                        buf += String.Format(str4, Grid.Rows[0].Cells[i].Text, GenitiveTransform1(Convert.ToDouble(Grid.Rows[0].Cells[i].Value), Grid.Columns[i].Header.Caption.Split(',')[1]));
                    }
                }
                Label4.Text += buf;
            }

            Label4.Text = Label4.Text.Replace("Общее", "<br>&nbsp;&nbsp;&nbsp;Общее").Replace("умерло", "число умерших").Replace("хмао-югре", "ХМАО-Югре");
            Label4.Text = Label4.Text.Remove(0, 4);
        }
        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Карта");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
            ReportExcelExporter1.TitleStartRow = 3;
            DundasMap.ZoomPanel.Visible = false;
            DundasMap.NavigationPanel.Visible = false;
            DundasMap.Width = 900;
            DundasMap.Height = 600;
            sheet2.Rows[2].Cells[0].Value = Label3.Text;
            ReportExcelExporter.MapExcelExport(sheet2.Rows[3].Cells[0], DundasMap);
            ReportExcelExporter1.Export(headerLayout, sheet1, 6);
            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;
        }

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
        }
        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            e.Workbook.Worksheets["Карта"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Карта"].Rows[2].Cells[0].CellFormat.Font.Name = "Verdana";
            e.Workbook.Worksheets["Карта"].Rows[2].Cells[0].CellFormat.Font.Height = 200;
        }
        #endregion

        #region Экспорт в PDF
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            Label4.Text = Label4.Text.Replace("<br>", "\n");
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text + "\n\n" + Label4.Text + "\n\n";
            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 60;
            Grid.Width = 800;
            ReportPDFExporter1.Export(headerLayout, section1);
            DundasMap.ZoomPanel.Visible = false;
            DundasMap.NavigationPanel.Visible = false;
            section2.PageOrientation = PageOrientation.Landscape;
            section2.PagePaddings = new Paddings(20, 30);
            section2.PageAlignment = Infragistics.Documents.Reports.Report.ContentAlignment.Center;
            IText title = section2.AddText();
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 14);
            title.Style.Font = font;
            title.AddContent(Label3.Text + "\n\n   ");
            DundasMap.Width = 900;
            DundasMap.Height = 600;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap);
            section2.AddImage(img);
            Grid.Width = (int)((minScreenWidth) - 15);
        }
        void ultraWebGridDocumentExporter_RowExporting(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.RowExportingEventArgs e)
        {
            e.GridRow.Height = 500;
        }
        #endregion

    }
}
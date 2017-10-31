using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Infragistics.WebUI.Misc;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;
using System.Collections.ObjectModel;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using Infragistics.UltraChart.Core;
using Dundas.Maps.WebControl;
using Infragistics.UltraChart.Shared.Styles;
using System.Net;
using System.IO;
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Image = Infragistics.Documents.Reports.Graphics.Image;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using Graphics = System.Drawing.Graphics;
using TextAlignment = Infragistics.Documents.Reports.Report.TextAlignment;
using System.Drawing.Imaging;
using Microsoft.VisualBasic;
using Infragistics.WebUI.UltraWebNavigator;
using System.Globalization;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.UltraGauge.Resources;
using Infragistics.UltraChart.Core.Layers;
namespace Krista.FM.Server.Dashboards.reports.HMAO_ARC._0003
{
    public partial class _default : CustomReportPage
    {
        string page_title = "Мониторинг объема сброса сточных вод (по состоянию на выбранную дату)";
        string page_sub_title = "Данные ежегодного мониторинга объема сброса сточных вод в ХМАО-Югре по состоянию на {0} год";
        string map_title = "Объем сточных вод, сброшенных в поверхностные водные объекты за {0} год, {1}";
        string chart2_title = "Сравнительный анализ мощности очистных сооружений и объема сточных вод, требующих очистки, за {0} год, {1}";
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam selectedYear { get { return (UserParams.CustomParam("selectedYear")); } }
        private GridHeaderLayout headerLayout;
        private string edIsm = "";
        private string legendTitle = "";
        private string style = "";
        private string mapFolderName = "";
        Color[] colorLegendArrayChart2;
        Color[] colorLegendArrayChart1;
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
            Label1.Text = page_title;
            Page.Title = page_title;


            //Grid.Height = Unit.Empty;
            Grid.Height = 400;
            ComboYear.Width = 200;
            DundasMap.Height = 800;
            Grid.Width = CRHelper.GetScreenWidth - 50;
            Chart2.Width = CRHelper.GetScreenWidth - 50;
            DundasMap.Width = CRHelper.GetScreenWidth - 50;             

           /* if (IsSmallResolution)
            {
                Grid.Width = minScreenWidth;
                Chart2.Width = minScreenWidth;
                DundasMap.Width = minScreenWidth - 5;
            }
            else
            {
                Grid.Width = (int)(minScreenWidth-10);
                Chart2.Width = (int)(minScreenWidth - 15);
                DundasMap.Width = (int)(minScreenWidth - 20);
            }*/

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            if (!Page.IsPostBack)
            {
                ComboYear.Title = "Выберите год";
                ComboYear.FillDictionaryValues(YearsLoad("years"));
                ComboYear.SelectLastNode();
            }
            selectedYear.Value = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Год].[" + ComboYear.SelectedValue + "]";
            Label2.Text =String.Format(page_sub_title,ComboYear.SelectedValue);
            mapFolderName = "Субъекты/ХМАОDeer";
            headerLayout = new GridHeaderLayout(Grid);
            Grid.DataBind();
            if (Grid.DataSource != null)
            {
                Chart2.DataBind();
                SetMapSettings();
                Label3.Text = String.Format(map_title, ComboYear.SelectedValue, edIsm);
            }
            Grid.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed; 
        }

        Dictionary<string, int> YearsLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.SpareMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 0; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 0);
            }
            return d;
        }

        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataTable resDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid"), "МР ГО", dt);
            
            
            if (dt.Rows.Count <= 1)
            {
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    dt.Columns[i].ColumnName = dt.Columns[i].ColumnName + ", " + dt.Rows[0][i].ToString().ToLower();
                }
                edIsm = dt.Rows[0][1].ToString().ToLower();
                dt.Rows.Remove(dt.Rows[0]);
                Grid.DataSource = null;
            }
            else
            {
                for (int i = 3; i < dt.Rows.Count; i += 3)
                {

                    dt.Rows[i][dt.Columns.IndexOf("Сброшено воды без очистки")] = Convert.ToDouble(dt.Rows[i - 2][dt.Columns.IndexOf("Сброшено воды без очистки")]) / Convert.ToDouble(dt.Rows[i - 2][dt.Columns.IndexOf("Объем сточных вод, требующих очистки")]) + "_объема сточных вод, требующих очистки";
                    dt.Rows[i][dt.Columns.IndexOf("Сброшено недостаточно очищенной воды")] = Convert.ToDouble(dt.Rows[i - 2][dt.Columns.IndexOf("Сброшено недостаточно очищенной воды")]) / Convert.ToDouble(dt.Rows[i - 2][dt.Columns.IndexOf("Объем сточных вод, требующих очистки")]) + "_объема сточных вод, требующих очистки";
                    dt.Rows[i][dt.Columns.IndexOf("Сброшено нормативно очищенной воды")] = Convert.ToDouble(dt.Rows[i - 2][dt.Columns.IndexOf("Сброшено нормативно очищенной воды")]) / Convert.ToDouble(dt.Rows[i - 2][dt.Columns.IndexOf("Объем сточных вод, требующих очистки")]) + "_объема сточных вод, требующих очистки";
                    dt.Rows[i][dt.Columns.IndexOf("Мощность очистных сооружений")] = Convert.ToDouble(dt.Rows[i - 2][dt.Columns.IndexOf("Мощность очистных сооружений")]) / Convert.ToDouble(dt.Rows[i - 2][dt.Columns.IndexOf("Объем сточных вод, требующих очистки")]) + "_объема сточных вод, требующих очистки";
                    dt.Rows[i][dt.Columns.IndexOf("Объем сточных вод, требующих очистки")] = Convert.ToDouble(dt.Rows[i - 2][dt.Columns.IndexOf("Объем сточных вод, требующих очистки")]) / Convert.ToDouble(dt.Rows[i - 2][dt.Columns.IndexOf("Объем сточных вод, сброшенных в поверхностные водные объекты")]) + "_объема сточных вод, сброшенных в поверхностные водные объекты";
                    dt.Rows[i][4] = DBNull.Value;
                }
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    if (dt.Rows[0][i].ToString().ToLower() == "миллион кубических метров")
                    {
                        dt.Columns[i].ColumnName = dt.Columns[i].ColumnName + ", " + "млн.куб.м.";
                    }
                    else
                    {
                        dt.Columns[i].ColumnName = dt.Columns[i].ColumnName + ", " + dt.Rows[0][i].ToString().ToLower();
                    }
                }
                if (dt.Rows[0][1].ToString().ToLower() == "миллион кубических метров")
                {
                    edIsm = "млн.куб.м.";
                }
                else
                {
                    edIsm = dt.Rows[0][1].ToString().ToLower();
                }
                
                dt.Rows.Remove(dt.Rows[0]);
                Grid.DataSource = dt;
            }
        }

       
        protected void Chart2_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "МР ГО", dt);
            colorLegendArrayChart2 = new Color[dt.Columns.Count - 1];
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                string s = dt.Columns[i].ColumnName;
                switch (s)
                {
                    case "Мощность очистных сооружений": { colorLegendArrayChart2[i - 1] = Color.Green; break; }
                    case "Объем сточных вод, требующих очистки": { colorLegendArrayChart2[i - 1] = Color.Red; break; }
                }
            }
            if (dt.Rows.Count < 1)
            {
                Chart2.DataSource = null;
                Chart2.Tooltips.FormatString = "<ITEM_LABEL> <b><DATA_VALUE:### ##0.##></b> ";
                Label4.Text = String.Format(chart2_title, ComboYear.SelectedValue," ");
            }
            else
            {
                if (IsSmallResolution)
                {
                    Chart2.Tooltips.FormatString = "<ITEM_LABEL><br> <b><DATA_VALUE:### ##0.##></b> " + edIsm;
                }
                else
                {
                    Chart2.Tooltips.FormatString = "<ITEM_LABEL> <b><DATA_VALUE:### ##0.##></b> " + edIsm;
                }
                Label4.Text = String.Format(chart2_title, ComboYear.SelectedValue, edIsm);
                foreach (DataRow row in dt.Rows)
                {
                    row[0] = row[0].ToString().Split(';')[0].Replace("муниципальный район", " МР").Replace("Город", "г.");
                }
                if (dt.Columns.Count >= 3)
                {
                    for (int i = 1; i < dt.Rows.Count; i++)
                    {

                        if ((i - 1) % 3 == 0)
                        {
                            if (dt.Columns[dt.Columns.Count - 1].ColumnName.StartsWith("Объем"))
                            {
                                dt.Rows[i - 1][dt.Columns.Count - 1] = DBNull.Value;
                                dt.Rows[i][dt.Columns.Count - 2] = DBNull.Value;
                            }
                        }
                        if ((i + 1) % 3 == 0)
                        {
                            dt.Rows[i][0] = "Нет данных";
                        }
                    }
                }
                else
                {
                    for (int i = 1; i < dt.Rows.Count; i++)
                    {
                        if ((i + 1) % 3 == 0)
                        {
                            dt.Rows[i][0] = "Нет данных";
                        }
                    }
                }
                Chart2.DataSource = dt;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            headerLayout.childCells.Clear();
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.12);
            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = true;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = 1 == 1;
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.11);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:5px";
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Header.Caption = e.Layout.Bands[0].Columns[i].Key;
            }
            GridHeaderCell headerCell=null, headerCell1 = null;
            headerCell = headerLayout.AddCell(e.Layout.Bands[0].Columns[1].Key);
            headerCell.AddCell("Всего");
            headerCell1= headerCell.AddCell("Сброшено загрязненной воды, "+edIsm);
            headerCell1.AddCell(e.Layout.Bands[0].Columns[2].Key);
            headerCell1.AddCell(e.Layout.Bands[0].Columns[3].Key);
            headerCell1 = headerCell.AddCell("Сброшено чистой воды, "+edIsm);
            headerCell1.AddCell(e.Layout.Bands[0].Columns[4].Key);
            headerCell1.AddCell(e.Layout.Bands[0].Columns[5].Key);
            headerCell = headerLayout.AddCell(e.Layout.Bands[0].Columns[6].Key);
            headerCell = headerLayout.AddCell(e.Layout.Bands[0].Columns[7].Key);
            headerLayout.ApplyHeaderInfo();
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (IsSmallResolution)
            { style = "background-repeat: no-repeat;background-position:30px"; }
            else
            { style = "background-repeat: no-repeat;background-position: 40px"; }

            if ((e.Row.Index + 1) % 3 == 0)
            {
                e.Row.Cells[0].Text = "";
                Grid.Rows[e.Row.Index - 2].Cells[0].Text = "";
                Grid.Rows[e.Row.Index - 1].Cells[0].Text = Grid.Rows[e.Row.Index - 1].Cells[0].Text.Split(';')[0].Replace("муниципальный район","МР").Replace("Город","г.");

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
                    if (ComboYear.SelectedIndex != 0)
                    {
                        if (Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value) < 0)
                        {
                            Grid.Rows[e.Row.Index - 1].Cells[i].Title = "Снижение относительно " + ComboYear.SelectedNode.PrevNode.Text.Split(' ')[0] + " года";
                            if (Grid.Columns[i].Header.Caption.StartsWith("Сброшено нормативно чистой воды") || Grid.Columns[i].Header.Caption.StartsWith("Сброшено нормативно очищенной воды") || Grid.Columns[i].Header.Caption.StartsWith("Мощность очистных сооружений"))
                            {
                                Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                            }
                            else
                            {
                                Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            }
                            Grid.Rows[e.Row.Index - 1].Cells[i].Style.CustomRules = style;

                        }
                        if (Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value) > 0)
                        {
                            Grid.Rows[e.Row.Index - 1].Cells[i].Title = "Рост относительно " + ComboYear.SelectedNode.PrevNode.Text.Split(' ')[0] + " года";
                            if (Grid.Columns[i].Header.Caption.StartsWith("Сброшено нормативно чистой воды") || Grid.Columns[i].Header.Caption.StartsWith("Сброшено нормативно очищенной воды") || Grid.Columns[i].Header.Caption.StartsWith("Мощность очистных сооружений"))
                            {
                                Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            }
                            else
                            {
                                Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                            }
                            Grid.Rows[e.Row.Index - 1].Cells[i].Style.CustomRules = style;

                        }
                        Grid.Rows[e.Row.Index - 1].Cells[i].Value = String.Format("{0:N2}", Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value));
                        Grid.Rows[e.Row.Index - 2].Cells[i].Value = String.Format("{0:N2}", Convert.ToDouble(Grid.Rows[e.Row.Index - 2].Cells[i].Value));
                        if ((e.Row.Cells[i].Value != null))
                        {
                            if (Grid.Columns[i].Header.Caption == "Всего")
                            { e.Row.Cells[i].Title = "Темп прироста к " + ComboYear.SelectedNode.PrevNode.Text.Split(' ')[0] + " году"; }
                            else
                            {
                                e.Row.Cells[i].Title = "Доля от " + e.Row.Cells[i].Text.Split('_')[1];
                            }
                            e.Row.Cells[i].Value = e.Row.Cells[i].Text.Split('_')[0];
                            e.Row.Cells[i].Value = String.Format("{0:P2}", Convert.ToDouble(e.Row.Cells[i].Value));
                        }
                    }
                    else
                    {
                        if ((e.Row.Cells[i].Value != null))
                        {
                            if (Grid.Columns[i].Header.Caption == "Всего")
                            { 
                               // e.Row.Cells[i].Title = "Темп прироста к " + ComboYear.SelectedNode.PrevNode.Text.Split(' ')[0] + " году"; 
                            }
                            else
                            {
                                e.Row.Cells[i].Title = "Доля от " + e.Row.Cells[i].Text.Split('_')[1];
                            }
                            e.Row.Cells[i].Value = e.Row.Cells[i].Text.Split('_')[0];
                            e.Row.Cells[i].Value = String.Format("{0:P2}", Convert.ToDouble(e.Row.Cells[i].Value));
                        }
                    }
                }
            }
        }


        protected void SetErorFonn(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "В настоящий момент данные отсутствуют";
            e.LabelStyle.FontColor = System.Drawing.Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new System.Drawing.Font("Verdana", 30);
        }

        protected void Chart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            int colOper = 0;
            int colBox = 0;
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
                if (primitive is Infragistics.UltraChart.Core.Primitives.Box)
                {
                    Infragistics.UltraChart.Core.Primitives.Box box = (Infragistics.UltraChart.Core.Primitives.Box)primitive;
                    if (box.DataPoint != null)
                    {
                        if (box.DataPoint.Label == "Мощность очистных сооружений")
                        {
                            box.PE.Fill = Color.Green;
                            box.PE.ElementType = PaintElementType.SolidFill;
                            box.PE.FillOpacity = 255;
                        }
                        if (box.DataPoint.Label == "Объем сточных вод, требующих очистки")
                        {
                            box.PE.Fill = Color.Red;
                            box.PE.ElementType = PaintElementType.SolidFill;
                            box.PE.FillOpacity = 255;
                        }
                    }
                    else
                    {

                        if ((box.Column <= (colorLegendArrayChart2.Length - 1)) && (box.rect.Width < 60))
                        {
                            box.PE.Fill = colorLegendArrayChart2[colBox];
                            box.PE.ElementType = PaintElementType.SolidFill;
                            box.PE.FillOpacity = 255;
                            colBox += 1;
                        }
                        
                    }
                }
                if (primitive is Infragistics.UltraChart.Core.Primitives.Text)
                {
                    Infragistics.UltraChart.Core.Primitives.Text text = (Infragistics.UltraChart.Core.Primitives.Text)primitive;
                    if (text.Path.StartsWith("Border.Title.Grid.X"))
                    {
                        if ((colOper - 1) % 3 == 0)
                        {
                            text.labelStyle.HorizontalAlign = StringAlignment.Far;
                           // text.labelStyle.VerticalAlign = StringAlignment.Center;
                            text.labelStyle.Font = new System.Drawing.Font("Verdana", (float)(7.4));
                            if (IsSmallResolution)
                            {
                                text.bounds.X = text.bounds.X - 6;
                            }
                            else
                            {
                                text.bounds.X = text.bounds.X - 10;
                            }
                           // text.bounds.Y = text.bounds.Y + 9;
                        }
                        else
                        {
                            text.Visible = false;
                        }
                        colOper += 1;
                    }
                }
            }

        }

        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            Worksheet sheet3 = workbook.Worksheets.Add("Карта");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
            ReportExcelExporter1.TitleStartRow = 3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 6);
            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;
            int width = int.Parse(Chart2.Width.Value.ToString());
            Chart2.Width = 900;
            ReportExcelExporter1.Export(Chart2, Label4.Text, sheet2, 2);
            Chart2.Width = width;
            DundasMap.Width = 900;
            sheet3.Rows[2].Cells[0].Value = Label3.Text;
            ReportExcelExporter.MapExcelExport(sheet3.Rows[3].Cells[0], DundasMap);
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
            e.Workbook.Worksheets["Таблица"].Rows[3].Height = 900;
            e.Workbook.Worksheets["Таблица"].Rows[4].Height = 950;
            e.Workbook.Worksheets["Таблица"].Rows[4].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            e.Workbook.Worksheets["Диаграмма"].Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Distributed;
            e.Workbook.Worksheets["Диаграмма"].Rows[1].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            e.Workbook.Worksheets["Диаграмма"].Rows[1].Height = 800;
       /*     e.Workbook.Worksheets["Диаграмма2"].Rows[1].Height = 800;
            e.Workbook.Worksheets["Диаграмма2"].Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Distributed;
            e.Workbook.Worksheets["Диаграмма2"].Rows[1].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;*/

        }
        #endregion

        #region Экспорт в PDF
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 30;
            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();
            int width = int.Parse(Chart2.Width.Value.ToString());
            Chart2.Width = 1000;
            ReportPDFExporter1.HeaderCellHeight = 60;
            int width1 = int.Parse(Chart2.Width.Value.ToString());
            ReportPDFExporter1.Export(headerLayout, section1);
            Grid.Width = width1;
            ReportPDFExporter1.Export(Chart2, Label4.Text, section2);
            Chart2.Width = width;
            IText title = section3.AddText();
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 14);
            title.Style.Font = font;
            title.AddContent(Label3.Text + "\n\n   ");
            section3.PageSize = new PageSize(1000, 800);
            DundasMap.Viewport.LocationUnit = CoordinateUnit.Pixel;
            DundasMap.Viewport.ViewCenter.Y -= 10;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap);
            section3.AddImage(img);
        }
        #endregion
        #region Обработчики карты
        DataTable dtMap;
        bool smallLegend = false;
        bool revPok = false;
        public void SetMapSettings()
        {
            legendTitle = "Объем сточных вод, сброшенных в\n поверхностные водные объекты, " + edIsm;
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeRules.Clear();
            DundasMap.Shapes.Clear();
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.Viewport.Zoom = 88;
            DundasMap.ColorSwatchPanel.Visible = false;
            DundasMap.Viewport.LocationUnit = CoordinateUnit.Pixel;
            if (IsSmallResolution)
            {
                
                DundasMap.Viewport.ViewCenter.Y -= 10;
            }
            DundasMap.Legends.Clear();
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

            Legend legend2 = new Legend("SymbolLegend");
            legend2.Visible = true;
            legend2.Dock = PanelDockStyle.Right;
            legend2.BackColor = Color.White;
           // legend2.SetBoundsInPixels(new RectangleF(new PointF(50,50),new SizeF(50,50)));
            //legend2.SetSizeInPixels(new SizeF(200, 100));
            legend2.BackSecondaryColor = Color.Gainsboro;
            legend2.BackGradientType = GradientType.DiagonalLeft;
            legend2.BackHatchStyle = MapHatchStyle.None;
            legend2.BorderColor = Color.Gray;
            legend2.BorderWidth = 1;
            legend2.BorderStyle = MapDashStyle.Solid;
            legend2.BackShadowOffset = 4;
            legend2.TextColor = Color.Black;
            legend2.Font = new System.Drawing.Font("MS Sans Serif", 7, FontStyle.Regular);
            legend2.AutoFitText = true;
            legend2.Title = "Доля загрязненной воды\n в общем объеме сточных вод";
            legend2.AutoFitMinFontSize = 7;

            DundasMap.Legends.Add(legend2);
            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();

            dtMap = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("map"), "Карта", dtMap);
            double[] values = new double[dtMap.Rows.Count];
            for (int i = 0; i < dtMap.Rows.Count; i++)
            {
                values[i] = Convert.ToDouble(dtMap.Rows[i].ItemArray[1]);
            }
            Array.Sort(values);
            revPok = true;
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
                rule.FromColor = Color.PaleGreen;
                rule.MiddleColor = Color.Green;
                rule.ToColor = Color.DarkGreen;
                rule.BorderColor = Color.FromArgb(50, Color.Black);
                rule.GradientType = GradientType.None;
                rule.HatchStyle = MapHatchStyle.None;
                rule.ShowInLegend = "CostLegend";

                rule.LegendText = "#FROMVALUE{N2} - #TOVALUE{N2}";
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

            DundasMap.SymbolFields.Add("PollutedWater");
            DundasMap.SymbolFields["PollutedWater"].Type = typeof(double);
            DundasMap.SymbolFields["PollutedWater"].UniqueIdentifier = false;

            // добавляем правила расстановки символов
            DundasMap.SymbolRules.Clear();
            SymbolRule symbolRule = new SymbolRule();
            symbolRule.Name = "SymbolRule";
            symbolRule.Category = string.Empty;
            symbolRule.DataGrouping = DataGrouping.Optimal;
            symbolRule.SymbolField = "PollutedWater";
            symbolRule.ShowInLegend = "SymbolLegend";
            symbolRule.LegendText = "#FROMVALUE{N2}% - #TOVALUE{N2}%";
            DundasMap.SymbolRules.Add(symbolRule);

            // звезды для легенды
            for (int i = 1; i < 6; i++)
            {
                PredefinedSymbol predefined = new PredefinedSymbol();
                predefined.Name = "PredefinedSymbol" + i;
                predefined.MarkerStyle = MarkerStyle.Triangle;
                predefined.Width = 5 + (i * 5);
                predefined.Height = predefined.Width;
                predefined.Color = Color.Red;
                DundasMap.SymbolRules["SymbolRule"].PredefinedSymbols.Add(predefined);
            }

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
            string layerName = Server.MapPath(string.Format("../../../maps/{0}/{1}.shp", mapFolder, layerFileName));
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
            string shapeHint ="Объем сточных вод, сброшенных в поверхностные водные объекты в {0} г. {1} {2}";

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
                        shape.ToolTip = String.Format(shapeHint, ComboYear.SelectedValue, String.Format("{0:0.##}", Convert.ToDouble(dtMap.Rows[j].ItemArray[1])), edIsm) + ", " + subject.Split('.')[1];
                    }
                    else
                    {
                        shape.ToolTip = String.Format(shapeHint, ComboYear.SelectedValue, String.Format("{0:0.##}", Convert.ToDouble(dtMap.Rows[j].ItemArray[1])), edIsm) + ", " + subject.Replace("р-н", String.Empty) + "МР";
                    }
                    shape.Text = subject + "\n" + String.Format("{0:0.##}", Convert.ToDouble(dtMap.Rows[j].ItemArray[1]));
                    shape.Name = subject;
                    shape.TextVisibility = TextVisibility.Shown;
                    if (smallLegend)
                    {
                        shape.Color = GetColor(Convert.ToDouble(dtMap.Rows[j].ItemArray[1]));
                    }
                    Dundas.Maps.WebControl.Symbol symbol = new Dundas.Maps.WebControl.Symbol();
                    symbol.Name = shape.Name + DundasMap.Symbols.Count;
                    symbol.ParentShape = shape.Name;
                    symbol["PollutedWater"] = Convert.ToDouble(dtMap.Rows[j].ItemArray[1]) == 0 ? 0 : Convert.ToDouble(dtMap.Rows[j].ItemArray[2]) / Convert.ToDouble(dtMap.Rows[j].ItemArray[1]) * 100;
                    symbol.ToolTip = String.Format("Доля загрязненной воды в общем объеме сточных вод в {0} г. {1}%", ComboYear.SelectedValue, String.Format("{0:0.##}", Convert.ToDouble(symbol["PollutedWater"])));
                    symbol.Color = Color.Red;
                    symbol.MarkerStyle = MarkerStyle.Triangle;
                    symbol.Offset.Y = -20;
                    symbol.Layer = shape.Layer;
                    DundasMap.Symbols.Add(symbol);
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

    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Image = Infragistics.Documents.Reports.Graphics.Image;

namespace Krista.FM.Server.Dashboards.reports.IT_0002_0004
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;

        private CustomParam assessType;
        private CustomParam directAssess;

        #endregion

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
            get { return CustomReportConst.minScreenHeight; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            assessType = CustomParam.CustomParamFactory("assess_type");
            directAssess = CustomParam.CustomParamFactory("direct_assess");

            UltraWebGrid1.Width = Unit.Empty;
           

            UltraWebGrid1.Height = Unit.Empty;

            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
            //UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraGridExporter1.Visible = false;
            UltraGridExporter1.MultiHeader = false;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <DocumentExportEventArgs>(PdfExporter_BeginExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                ComboRegion.FillDictionaryValues(Regions);
                ComboRegion.Title = "Показатель";
                ComboRegion.Width = 400;
                ComboRegion.SetСheckedState("Выручка от реализации услуг", true);
            }

            assessType.Value = regionsDictionary[ComboRegion.SelectedValue];

            switch (assessType.Value)
            {
                case "Выручка от реализации услуг, тыс.руб.":
                    {
                       directAssess.Value = "BDESC";
                       break;
                    }
                case "Прибыль (убыток) до налогообложения, тыс.руб.":
                    {
                       directAssess.Value = "BDESC";
                       break;
                    }
                case "Рентабельность деятельности (к затратам)":
                    {
                       directAssess.Value = "BDESC";
                       break;
                    }
                case "Затраты на 100 рублей выручки, руб.":
                    {
                       directAssess.Value = "BASC";
                       break;
                    }
                case "Соотношение ФОТ к доходам, %":
                    {
                       directAssess.Value = "BASC";
                       break;
                    }
            }

            string query = DataProvider.GetQueryText(String.Format("IT_0004_0001_Grid"));
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtGrid);
            dtGrid.Rows[0]["Ранг "] = DBNull.Value;
            SetupMap();

            Page.Title = "Показатели оценки деятельности";
            PageTitle.Text = "Показатели оценки деятельности";
            PageSubTitle.Text = String.Format("Оценка деятельности консолидированного бюджета ФГУП «СВЯЗЬ-безопасность» и филиалов за 1 полугодие 2010 года по показателю «{0}»", assessType.Value);
            Label1.Text = assessType.Value;
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
        }


        #region Обработчики грида

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            foreach (DataRow row in dtGrid.Rows)
            {
                row[1] = String.Format("{1}&nbsp;<a href ='{0}'>&gt;&gt;&gt;</a>", row["Сайт"], row["Имя"]);
            }
            dtGrid.Columns.RemoveAt(9);
            dtGrid.Columns.RemoveAt(0);
            dtGrid.AcceptChanges();
            ((UltraWebGrid)sender).DataSource = dtGrid;
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid1.Height = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].CellStyle.Padding.Right = 5;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(400, 1280);
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++ )
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120, 1280);
            }
                e.Layout.Bands[0].Columns[7].Hidden = true;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N2");
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index > 0)
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
            }
            SetConditionArrow(e, 6, CustomParam.CustomParamFactory("direct_assess").Value == "BDESC");
            SetConditionBall(e, 5);
            SetRankImage(e, 4, 7, true);
        }

        private static void SetConditionArrow(RowEventArgs e, int index, bool direct)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string img;
                //string title;
                if (direct)
                {
                    if (value > 1)
                    {
                        img = "~/images/arrowGreenUpBB.png";
                    }
                    else
                    {
                        img = "~/images/arrowRedDownBB.png";
                    }
                }
                else
                {
                    if (value < 1)
                    {
                        img = "~/images/arrowGreenUpBB.png";
                    }
                    else
                    {
                        img = "~/images/arrowRedDownBB.png";
                    }
                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; padding-left: 2px";
                //   e.Row.Cells[3].Title = title;
            }
            if (e.Row.Index > 0)
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
            }
        }

        private static void SetConditionBall(RowEventArgs e, int index)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string img;
                //string title;
                if (value < 1)
                {
                    img = "~/images/BallRedBB.png";

                }
                else
                {
                    img = "~/images/BallGreenBB.png";

                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 5px center; padding-left: 2px";
                //   e.Row.Cells[3].Title = title;
            }
        }

        private static void SetRankImage(RowEventArgs e, int rankCellIndex, int worseRankCelIndex, bool direct)
        {
            if (e.Row.Cells[rankCellIndex] != null &&
                e.Row.Cells[rankCellIndex].Value != null &&
                e.Row.Cells[worseRankCelIndex] != null &&
                e.Row.Cells[worseRankCelIndex].Value != null)
            {
                int value = Convert.ToInt32(e.Row.Cells[rankCellIndex].Value.ToString());
                int worseRankValue = Convert.ToInt32(e.Row.Cells[worseRankCelIndex].Value.ToString());
                string img = String.Empty;
                //string title;
                if (direct)
                {
                    if (value == 1)
                    {
                        img = "~/images/StarYellowBB.png";
                    }
                    else if (value == worseRankValue)
                    {
                        img = "~/images/StarGrayBB.png";
                    }
                }
                else
                {
                    if (value == 1)
                    {
                        img = "~/images/StarGrayBB.png";
                    }
                    else if (value == worseRankValue)
                    {
                        img = "~/images/StarYellowBB.png";
                    }
                    e.Row.Cells[rankCellIndex].Value = worseRankValue - value + 1;
                }
                e.Row.Cells[rankCellIndex].Style.BackgroundImage = img;
                e.Row.Cells[rankCellIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: 20px center; padding-left: 2px; padding-right: 15px";
            }
        }

        #endregion

        
        private static Dictionary<string, string> regionsDictionary;
        private static Dictionary<string, int> regions;

        public static Dictionary<string, int> Regions
        {
            get
            {
                // если словарь пустой
                if (regions == null || regions.Count == 0)
                {
                    // заполняем его
                    FillRegions();
                }
                return regions;
            }
        }

        private static void FillRegions()
        {
            regionsDictionary = new Dictionary<string, string>();
            regions = new Dictionary<string, int>();

            regionsDictionary.Add("Выручка от реализации услуг", "Выручка от реализации услуг, тыс.руб.");
            regionsDictionary.Add("Прибыль (убыток) до налогообложения", "Прибыль (убыток) до налогообложения, тыс.руб.");
            regionsDictionary.Add("Рентабельность деятельности (к затратам)", "Рентабельность деятельности (к затратам), %");
            regionsDictionary.Add("Затраты на 100 рублей выручки", "Затраты на 100 рублей выручки, руб.");
            regionsDictionary.Add("Соотношение ФОТ к доходам", "Соотношение ФОТ к доходам, %");

            regions.Add("Выручка от реализации услуг", 0);
            regions.Add("Прибыль (убыток) до налогообложения", 0);
            regions.Add("Рентабельность деятельности (к затратам)", 0);
            regions.Add("Затраты на 100 рублей выручки", 0);
            regions.Add("Соотношение ФОТ к доходам", 0);
        }

        #region Карта

        private void SetupMap()
        {
            DundasMap1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth) -  50;
            DundasMap1.Height = CRHelper.GetChartWidth(500);

            #region Настройка карты 1

            DundasMap1.Meridians.Visible = false;
            DundasMap1.Parallels.Visible = false;
            DundasMap1.ZoomPanel.Visible = !IsSmallResolution;
            DundasMap1.ZoomPanel.Dock = PanelDockStyle.Right;
            DundasMap1.NavigationPanel.Visible = !IsSmallResolution;
            DundasMap1.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap1.Viewport.EnablePanning = true;
            DundasMap1.Viewport.OptimizeForPanning = true;

            // добавляем легенду
            DundasMap1.Legends.Clear();

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
            legend1.Font = new System.Drawing.Font("MS Sans Serif", 7, FontStyle.Regular);
            legend1.AutoFitText = true;
            legend1.Title = "";
            legend1.AutoFitMinFontSize = 7;
            DundasMap1.Legends.Add(legend1);

            // добавляем поля для раскраски
            DundasMap1.ShapeFields.Clear();
            DundasMap1.ShapeFields.Add("Name");
            DundasMap1.ShapeFields["Name"].Type = typeof (string);
            DundasMap1.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap1.ShapeFields.Add("UnemploymentLevel");
            DundasMap1.ShapeFields["UnemploymentLevel"].Type = typeof (double);
            DundasMap1.ShapeFields["UnemploymentLevel"].UniqueIdentifier = false;

            // добавляем правила раскраски
            DundasMap1.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "UnemploymentLevel";
            rule.DataGrouping = DataGrouping.Optimal;
            bool direct = CustomParam.CustomParamFactory("direct_assess").Value == "BDESC" ? true : false;
            rule.ColorCount = 3;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = direct ? Color.Red : Color.Green;
            rule.MiddleColor = Color.Orange;
            rule.ToColor = direct ? Color.Green : Color.Red;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";
            rule.LegendText = "#FROMVALUE{N1} - #TOVALUE{N1}";

            DundasMap1.ShapeRules.Add(rule);

            #endregion

            DundasMap1.Shapes.Clear();
            DundasMap1.LoadFromShapeFile(Server.MapPath("~/maps/_РФ/РФ.shp"), "NAME", true);
            // заполняем карту данными
            FillMapData1(DundasMap1, dtGrid);
        }

        #region Обработчики карты

        public static Shape FindMapShape(MapControl map, string patternValue, bool searchFO)
        {
            string[] subjects = patternValue.Split(' ');
            ArrayList shapeList = map.Shapes.Find(subjects[0], true, false);

            if (shapeList.Count > 0)
            {
                return (Shape)shapeList[0];
            }

            return null;
        }

        #endregion

        public void FillMapData1(MapControl map, DataTable dtMap)
        {
            foreach (DataRow row in dtMap.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[4] != DBNull.Value && row[4].ToString() != string.Empty)
                {
                    string subject = row[0].ToString().Replace("УФО", "УрФО");
                    if (RegionsNamingHelper.IsFO(RegionsNamingHelper.FullName(subject)))
                    {
                        Shape shape = FindMapShape(DundasMap1, RegionsNamingHelper.FullName(subject), true);
                        if (shape != null)
                        {
                            shape["Name"] = subject;
                            shape["UnemploymentLevel"] = Convert.ToDouble(row[4]);

                            shape.TextAlignment = ContentAlignment.MiddleCenter;
                            shape.Offset.X = -15;

                            shape.Text = string.Format("{0}\n{1:N2}", row[0], Convert.ToDouble(row[4]));
                           // shape.Font = new System.Drawing.Font("Arial", 12, FontStyle.Bold);
                            shape.ToolTip = string.Format("{0}\n {1:N2}", row[1], Convert.ToDouble(row[4]));
                            shape.BorderWidth = 2;
                            shape.TextColor = Color.Black;
                            shape.TextVisibility = TextVisibility.Shown;
                        }
                    }
                }
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid1.Columns.Count;
            int rowsCount = UltraWebGrid1.Rows.Count;

            for (int i = 0; i < columnCount; i++)
            {
                e.CurrentWorksheet.Columns[i].Width = 100 * 37;
            }

            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "#,##0";
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.00";

            // расставляем стили у начальных колонок
            for (int i = 4; i < rowsCount + 4; i++)
            {
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid1);
        }

        private int offset = 0;

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            UltraGridColumn col = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            while (col.Hidden)
            {
                offset++;
                col = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            }
            e.HeaderText = col.Header.Key.Split(';')[0];
        }

        #endregion

        #region Экспорт в PDF


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

            UltraGridExporter1.PdfExporter.Export(UltraWebGrid1);
        }

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
            title.Style.Font = new Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Font(font);
            title.AddContent(PageSubTitle.Text);

            e.Section.AddPageBreak();
        }

        #endregion
    }
}

using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report.Table;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
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
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using ContentAlignment = System.Drawing.ContentAlignment;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0020
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private DataTable dtMap = new DataTable();
        private GridHeaderLayout headerLayout;
        private int firstYear = 2006;
        private int endYear = 2010;
        private int currentYear;
        private DateTime currentDate;
        private CustomParam prevYear;
        // имя папки с картами региона
        private string mapFolderName;
        // пропорция карты
        private double mapSizeProportion;
        // масшбтаб карты
        private double mapZoomValue;
        // сдвиг по вертикали текста городов-выносок карты
        private double mapCalloutOffsetY;
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            if (prevYear == null)
            {
                prevYear = UserParams.CustomParam("prevYear");
            }
            mapFolderName = RegionSettingsHelper.Instance.GetPropertyValue("MapFolderName");
            mapZoomValue = Convert.ToDouble(RegionSettingsHelper.Instance.GetPropertyValue("MapZoomValue"));
            double value = 1;
            mapSizeProportion = value;
            if (Double.TryParse(RegionSettingsHelper.Instance.GetPropertyValue("MapSizeProportion"), NumberStyles.Any, NumberFormatInfo.CurrentInfo, out value))
            {
                mapSizeProportion = value;
            }
            value = 1;
            mapCalloutOffsetY = value;
            if (Double.TryParse(RegionSettingsHelper.Instance.GetPropertyValue("MapCalloutOffsetY"), NumberStyles.Any, NumberFormatInfo.CurrentInfo, out value))
            {
                mapCalloutOffsetY = value;
            }
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.6 - 225);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;
            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight);
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            ReportPDFExporter1.PdfExporter.EndExport += new EventHandler<EndExportEventArgs>(PdfExporter_EndExport);
            ReportPDFExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0020_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(dtDate.Rows[0][3].ToString(), true);
            }
            
            currentYear = Convert.ToInt32(ComboYear.SelectedValue);
            prevYear.Value = Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 1);
            int monthNum = CRHelper.MonthNum(ComboMonth.SelectedValue);
            currentDate = new DateTime(currentYear, monthNum, 1);
            Page.Title = String.Format("Анализ бюджетной обеспеченности доходами");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("Сравнение бюджетной обеспеченности  доходами бюджетов городских округов и муниципальных районов в {0} - {1} гг.",
                currentYear - 1, currentYear);
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UserParams.PeriodYear.Value = currentYear.ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
            mapElementCaption.Text = String.Format("Исполнение налоговых и неналоговых доходов с учетом дотаций на выравнивание бюджетной обеспеченности на {0:dd.MM.yyyy} г.", currentDate.AddMonths(1));
            DundasMap.Shapes.Clear();
            if (Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("MapLoadFromBin")))
            {
                DundasMap.Serializer.Format = Dundas.Maps.WebControl.SerializationFormat.Binary;
                DundasMap.Serializer.Load((Server.MapPath(string.Format("../../maps/Субъекты/{0}/{0}_выноски.bin", mapFolderName))));
                SetMapSettings();
            }
            else
            {
                //DundasMap.ShapeFields.Clear();
                DundasMap.ShapeFields.Add("Name");
                DundasMap.ShapeFields["Name"].Type = typeof(string);
                DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
                DundasMap.ShapeFields.Add("Complete");
                DundasMap.ShapeFields["Complete"].Type = typeof(double);
                DundasMap.ShapeFields["Complete"].UniqueIdentifier = false;

                SetMapSettings();
                AddMapLayer(DundasMap, mapFolderName, "Территор", CRHelper.MapShapeType.Areas);
                AddMapLayer(DundasMap, mapFolderName, "Города", CRHelper.MapShapeType.Towns);
                AddMapLayer(DundasMap, mapFolderName, "Выноски", CRHelper.MapShapeType.CalloutTowns);
            }

            // заполняем карту данными
            FillMapData();
        }
        
        #region Обработчики грида
        string[,] dtRang;
        
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0020_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtGrid);
            
            if (dtGrid.Rows.Count > 0)
            {
                dtRang = new string[dtGrid.Rows.Count, dtGrid.Columns.Count];
                for (int i = 0; i < dtGrid.Rows.Count; i++)
                {
                    for (int j = 3; j < dtGrid.Columns.Count; j = j + 3)
                    {

                        dtRang[i, j] = dtGrid.Rows[i][j].ToString();

                    }
                }
                int a = 0;
                for (int j = 3; j < dtGrid.Columns.Count + 3; j = j + 3)
                {
                    dtGrid.Columns.RemoveAt(j - a);
                    a++;
                }
                dtGrid.AcceptChanges();
                UltraWebGrid.DataSource = dtGrid;
            }
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {

        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

           for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 120;

                if (e.Layout.Bands[0].Columns[i].Header.Caption.ToLower().Contains("ранг"))
                {
                    formatString = "N0";
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);
            int countheader = 0;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 2; i = i + 2)
            {
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                GridHeaderCell cell0 = null;
                switch (countheader)
                {
                    case 0:
                        {
                            cell0 = headerLayout.AddCell(String.Format("{0} год (факт)", currentYear - 1));
                            break;
                        }
                    case 1:
                        {
                            cell0 = headerLayout.AddCell(String.Format("{0} год (план)", currentYear));
                            break;
                        }
                    case 2:
                    //case 3:
                        {
                            cell0 = headerLayout.AddCell(String.Format("Факт по состоянию на {0:dd.MM.yyyy} г.", currentDate.AddMonths(1)));
                            GridHeaderCell cell1 = cell0.AddCell(captions[1]);
                            cell1.AddCell("Всего, руб./чел.");
                            cell1.AddCell("Ранг");
                            captions = e.Layout.Bands[0].Columns[i + 2].Header.Caption.Split(';');
                            GridHeaderCell cell2 = cell0.AddCell(captions[1]);
                            cell2.AddCell("Всего, руб./чел.");
                            cell2.AddCell("Ранг");
                            break;
                        } 
                }
                    countheader++;
                    if (countheader != 3)
                    {
                        GridHeaderCell cell1 = cell0.AddCell(captions[1]);
                        cell1.AddCell("Всего, руб./чел.");
                        cell1.AddCell("Ранг");
                    }
                e.Layout.Bands[0].Columns[i + 2].Hidden = false;
            }
            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool rank = ((i - 1) % 2 == 1);
                int a = 0;
                switch (i) // сдвиг матрицы рангов, куда скопированы скрытые колонки, чтоб после удаления скрытых колонок адекватно считывались ранги.
                {
                       case 2:
                    {
                        a = 0;
                        break;
                    }
                        case 4:
                    {
                        a = 1;
                        break;
                    }
                        case 6:
                    {
                        a = 2;
                        break;
                    }
                        case 8:
                    {
                        a = 3;
                        break;
                    }
                }
                if (rank)   
                {
                    if (e.Row.Cells[i].Value != null && dtRang[e.Row.Index, i + 1 + a] != null &&
                        e.Row.Cells[i].Value.ToString() != string.Empty && dtRang[e.Row.Index, i + 1 + a].ToString() != string.Empty)
                    {
                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = "Самый высокий показатель бюджетной обеспеченности по МО";
                        }
                        else if (Convert.ToInt32(e.Row.Cells[i].Value) == Convert.ToInt32(dtRang[e.Row.Index, i + 1 + a]))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = "Самый низкий показатель бюджетной обеспеченности по МО";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (e.Row.Cells[0].Value != null && 
                   (e.Row.Cells[0].Value.ToString().Contains("Среднее") || e.Row.Cells[0].Value.ToString().Contains("Всего")))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }

                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != String.Empty)
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

        #region Обработчики карты

        public void SetMapSettings()
        {
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.Viewport.Zoom = (float)mapZoomValue;
            DundasMap.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap.ZoomPanel.Dock = PanelDockStyle.Right;
            Legend legend = new Legend("CompleteLegend");
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
            legend.Font = new System.Drawing.Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.Title = "Процент исполнения налоговых и неналоговых доходов\nс учетом дотаций на выравнивание\nбюджетной обеспеченности";
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Clear();
            DundasMap.Legends.Add(legend);

            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "Complete";
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
            rule.LegendText = string.Format("#FROMVALUE{{P2}} - #TOVALUE{{P2}}");
            DundasMap.ShapeRules.Add(rule);
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

            //shapeName = shapeName.Replace("муниципальный район", "МР");

            return shapeName;
        }

        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(string.Format("../../maps/Субъекты/{0}/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;

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
            string query = DataProvider.GetQueryText("FO_0002_0020_map");
            dtMap = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtMap);

            foreach (Shape shape in DundasMap.Shapes)
            {
                string shapeName = GetShapeName(shape);
                if (!RegionsNamingHelper.LocalBudgetUniqueNames.ContainsKey(shapeName))
                {
                    shape.Visible = false;
                }
            }

            foreach (DataRow row in dtMap.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                {
                    string subject = row[0].ToString();
                    double value = Convert.ToDouble(row[1]);
                    ArrayList shapeList = FindMapShape(DundasMap, subject);
                    foreach (Shape shape in shapeList)
                    {
                        string shapeName = GetShapeName(shape);
                        shape["Name"] = subject;
                        shape["Complete"] = value;
                        shape.ToolTip = string.Format("{0};\n#COMPLETE{{P2}}", shapeName);

                        if (IsCalloutTownShape(shape))
                        {
                            shape.Text = string.Format("{0}\n{1:P2}", shapeName, value);
                            shape.TextVisibility = TextVisibility.Shown;
                            shape.TextAlignment = ContentAlignment.BottomCenter;

                            shape.CentralPointOffset.Y = mapCalloutOffsetY;
                        }
                        else
                        {
                            shape.Text = string.Format("{0}\n{1:P2}", shapeName.Replace(" ", "\n"), value);
                        }
                    }
                }
            }
        }

        #endregion

        #region Экспорт

        private void PdfExporter_EndExport(object sender, EndExportEventArgs e)
        {

            e.Section.AddPageBreak();
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.AddContent(mapElementCaption.Text);

            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            DundasMap.ZoomPanel.Visible = false;
            DundasMap.NavigationPanel.Visible = false;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap);
            e.Section.AddImage(img);

        }
        private void PdfExportButton_Click(object sender, System.EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            ReportPDFExporter1.Export(headerLayout);

        }
       private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {

            
           
        }
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        #endregion
    }
}


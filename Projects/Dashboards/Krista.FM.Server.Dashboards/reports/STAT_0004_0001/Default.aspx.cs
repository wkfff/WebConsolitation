using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
using System.Collections.ObjectModel;
using Dundas.Maps.WebControl;

using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report;
using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Infragistics.Documents.Excel;

namespace Krista.FM.Server.Dashboards.reports.STAT_0004_0001
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtData;
        //параметры
        private CustomParam SelectedPeriod;

        private GridHeaderLayout headerLayout;
        private int year;
        private int quarter;
        private string date;
      
        private int GetScreenWidth
        {
            get
            {
                if (Request.Cookies != null)
                {
                    if (Request.Cookies[CustomReportConst.ScreenWidthKeyName] != null)
                    {
                        HttpCookie cookie = Request.Cookies[CustomReportConst.ScreenWidthKeyName];
                        int value = Int32.Parse(cookie.Value);
                        return value;
                    }
                }
                return (int)Session["width_size"];
            }
        }

        private bool IsSmallResolution
        {
            get { return GetScreenWidth < 1200; }
        }

        // имя папки с картами региона
        private const string mapFolderName = "ХМАО";

        private static bool IsMozilla
        {
            get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            UltraWebGrid.Width = IsSmallResolution ? 750 : CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.45);
            
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight *0.8);
            LabelMap.Width = DundasMap.Width;
            DundasMap.PostPaint += new MapPaintEvent(DundasMap_PostPaint);

            #region Инициализация параметров

            if (SelectedPeriod == null)
            {
                SelectedPeriod = UserParams.CustomParam("select_period");
            }
           
            #endregion

            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
       }
        
        protected override void Page_Load(Object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)   
            {
                ComboYear.Title = "Период";
                ComboYear.Width = 300;
                ComboYear.MultiSelect = false;
                ComboYear.ParentSelect = false;
                FillComboDate(ComboYear, "STAT_0004_0001_data", 0);
            }

            DundasMap.Height = 500;
            DundasMap.Width = CRHelper.GetScreenWidth - 45; 

            Page.Title = "Мониторинг аварийности на нефтепромысловых трубопроводах";
            Label1.Text = "Мониторинг аварийности на нефтепромысловых трубопроводах";
            
            string template ="[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}]";
            string[] dateElements = ComboYear.SelectedValue.Split(' ');
            year = Convert.ToInt32(dateElements[2]);
            quarter = Convert.ToInt32(dateElements[0]);
            int halfYear = CRHelper.HalfYearNumByQuarterNum(quarter);
            
            SelectedPeriod.Value = string.Format(template, year,halfYear,quarter);

            date = String.Format("{0} квартал {1} года", quarter,year);
            Label2.Text = string.Format("Данные ежеквартального мониторинга ситуации по аварийности на нефтепромысловых трубопроводах в ХМАО-Югре в разрезе предприятий, по состоянию на {0}", date);
            
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            #region Настройка карты 2

            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.ZoomPanel.Dock = PanelDockStyle.Left;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.NavigationPanel.Dock = PanelDockStyle.Left;
            DundasMap.Viewport.EnablePanning = true;

            // добавляем легенду
            DundasMap.Legends.Clear();

            // добавляем легенду раскраски
            Legend legend1 = new Legend("TensionLegend");
            legend1.Visible = true;
            legend1.Dock = PanelDockStyle.Right;
            legend1.BackColor = Color.White;
            legend1.BackSecondaryColor = Color.Gainsboro;
            legend1.BackGradientType = GradientType.DiagonalLeft;
            legend1.BackHatchStyle = MapHatchStyle.None;
            legend1.BorderColor = Color.Gray;
            legend1.BorderWidth = 1;
            legend1.BorderStyle = MapDashStyle.Solid;
            legend1.BackShadowOffset = 4;
            legend1.TextColor = Color.Black;
           // legend1.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend1.AutoFitText = true;
            legend1.Title = "Площадь загрязнения, га";
            legend1.AutoFitMinFontSize = 7;

            // добавляем легенду с символами
            Legend legend2 = new Legend("VacancyLegend");
            legend2.Visible = true;
            legend2.Dock = PanelDockStyle.Right;
            legend2.BackColor = Color.White;
            legend2.BackSecondaryColor = Color.Gainsboro;
            legend2.BackGradientType = GradientType.DiagonalLeft;
            legend2.BackHatchStyle = MapHatchStyle.None;
            legend2.BorderColor = Color.Gray;
            legend2.BorderWidth = 1;
            legend2.BorderStyle = MapDashStyle.Solid;
            legend2.BackShadowOffset = 4;
            legend2.TextColor = Color.Black;
           // legend2.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend2.AutoFitText = true;
            legend2.Title = "Масса загрязняющих веществ, тонн";  // наименование легенды 2
            legend2.AutoFitMinFontSize = 7;
            DundasMap.Legends.Add(legend2);
            DundasMap.Legends.Add(legend1);

            // добавляем поля для раскраски
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("TensionKoeff");
            DundasMap.ShapeFields["TensionKoeff"].Type = typeof(double);
            DundasMap.ShapeFields["TensionKoeff"].UniqueIdentifier = false;

            // добавляем поля для символов
            DundasMap.SymbolFields.Add("VacancyCount");
            DundasMap.SymbolFields["VacancyCount"].Type = typeof(double);
            DundasMap.SymbolFields["VacancyCount"].UniqueIdentifier = false;
            DundasMap.SymbolFields.Add("RedundantCount");
            DundasMap.SymbolFields["RedundantCount"].Type = typeof(double);
            DundasMap.SymbolFields["RedundantCount"].UniqueIdentifier = false;

            LegendItem item = new LegendItem();
            item.Text = "Масса ликвидированных загрязняющих веществ";
            item.Color = Color.DarkViolet;
            legend2.Items.Add(item);

            item = new LegendItem();
            item.Text = "Масса загрязняющих веществ после ликвидации аварии";
            item.Color = Color.Black;
            legend2.Items.Add(item);

            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "TensionKoeffRule";
            rule.Category = String.Empty;
            rule.ShapeField = "TensionKoeff";
            rule.DataGrouping = DataGrouping.EqualInterval;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Green;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Red;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "TensionLegend";
            rule.LegendText = "#FROMVALUE{N4} - #TOVALUE{N4}";
            DundasMap.ShapeRules.Add(rule);

            AddMapLayer(DundasMap, mapFolderName, "Выноски", CRHelper.MapShapeType.CalloutTowns);
            AddMapLayer(DundasMap, mapFolderName, "Территор", CRHelper.MapShapeType.Areas);

            FillMapData(DundasMap);

            #endregion
       
       }

        protected void FillComboDate(CustomMultiCombo combo, string queryName, int offset)
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Таблица", dtDate);
            if (dtDate.Rows.Count > 0)
            {
                Dictionary<string, int> dictDate = new Dictionary<string, int>();

                string str = string.Empty;
                for (int numRow = 0; numRow < dtDate.Rows.Count; numRow++)
                {
                    if (dtDate.Rows[numRow][3].ToString() == "0")
                    {
                        if (dtDate.Rows[numRow + 1] != null && dtDate.Rows[numRow + 1][3].ToString() == "1")
                        {
                            dictDate.Add(dtDate.Rows[numRow][1].ToString(), 0);
                            str = dtDate.Rows[numRow][1].ToString();
                        }
                    }
                    else
                    {
                        string[] nQuart = dtDate.Rows[numRow][1].ToString().Split(' ');
                        dictDate.Add(string.Format("{0} квартал {1}а", nQuart[1], str), 1);
                    }
                }
                combo.FillDictionaryValues(dictDate);
                combo.SelectLastNode();
            }
        }

       
        #region обработчик грида
       
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0004_0001_grid");
            dtData = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Федеральный округ", dtData);
            
            if (dtData.Rows.Count > 0)
            {
              dtData.Columns.RemoveAt(0);
              dtData.AcceptChanges();
              UltraWebGrid.DataSource = dtData;
            }
           
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
           
        }
        
       protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].MergeCells = true;

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

            int i;
           
            for (i = 1; i < e.Layout.Bands[0].Columns.Count-1; i++ )
             {
               e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
             }

            i = 1;
            headerLayout.AddCell("Предприятие, допустившее аварию");
            while (i<e.Layout.Bands[0].Columns.Count-1)
            {
                string[] columnCaption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                GridHeaderCell cell1 = headerLayout.AddCell(string.Format("{0}", columnCaption[0]));
                if (columnCaption[0] != "Площадь загрязнения")
                {
                    int j = 0;
                    for (int k = i; k < e.Layout.Bands[0].Columns.Count - 1; k++)
                    {
                        columnCaption = e.Layout.Bands[0].Columns[k].Header.Caption.Split(';');

                        if (cell1.Caption == columnCaption[0])
                        {
                            cell1.AddCell(string.Format("{0}", columnCaption[1]));
                            j++;
                        }
                    }
                  i = i + j;
               }
               else
               {
                 i++;
               }
            }
           
           headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[0].Style.BackColor = Color.White;
            int rowIndex = e.Row.Index;
            int p = (rowIndex % 3);
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[i].Style.Padding.Right = 10;
                e.Row.Cells[i].Style.BackColor = Color.White;
                switch (p)
                {
                    case 0:
                        {
                            if (e.Row.Cells[i].Value != null && e.Row.Cells[i].ToString() != string.Empty)
                                {
                                    double value = Convert.ToDouble(e.Row.Cells[i].Value);
                                    e.Row.Cells[i].Value = (i == 7) ? string.Format("{0:N4}", value) : (i == 5 || i == 6) ? string.Format("{0:N3}", value) : string.Format("{0:N0}", value);
                                }

                            
                            if (e.Row.Cells[e.Row.Cells.Count - 1] != null && e.Row.Cells[e.Row.Cells.Count - 1].ToString() =="1")
                            {
                                e.Row.Cells[0].Style.Font.Bold = true;
                            }
                            else if (e.Row.Cells[e.Row.Cells.Count - 1] != null && e.Row.Cells[e.Row.Cells.Count - 1].ToString() == "0")
                            {
                                e.Row.Cells[0].Style.Padding.Left = 15;
                            }
                            e.Row.Cells[i].Style.BorderDetails.WidthTop = 1;
                            e.Row.Cells[i].Style.BorderDetails.WidthBottom = 0;
                            break;
                        }
                    case 1:
                        {

                               //  e.Row.Cells[0].Value = " ";
                                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                                {
                                    object value = e.Row.Cells[i].Value;
                                    double doubleValue = Double.Parse(value.ToString(), NumberStyles.Any);

                                    if (doubleValue > 0)
                                    {

                                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                                       // e.Row.Cells[i].Title = "Рост к прошлому отчетному периоду";
                                        e.Row.Cells[i].Style.CustomRules =
                                            "background-repeat: no-repeat; background-position: left center; margin: 2px";
                                    }
                                    else if (doubleValue < 0)
                                    {
                                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                                      //  e.Row.Cells[i].Title = "Снижение к прошлому отчетному периоду";
                                        e.Row.Cells[i].Style.CustomRules =
                                            "background-repeat: no-repeat; background-position: left center; margin: 2px";
                                    }
                                }

                                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].ToString() != string.Empty)
                                {
                                    double dValue = Convert.ToDouble(e.Row.Cells[i].Value);
                                    e.Row.Cells[i].Value = (i == 7) ? string.Format("{0:N4}", dValue) : (i == 5 || i == 6) ? string.Format("{0:N3}", dValue) : string.Format("{0:N0}", dValue);
                                    e.Row.Cells[i].Title = "Отклонение от предыдущего периода";
                                }
                            e.Row.Cells[0].Value = e.Row.PrevRow.Cells[0].Value;
                            e.Row.Cells[i].Style.BorderDetails.WidthTop = 0;
                            e.Row.Cells[i].Style.BorderDetails.WidthBottom = 0;
                            break;
                        }
                    case 2:
                        {
                           
                               // e.Row.Cells[0].Value = " ";
                                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].ToString() != string.Empty)
                                {
                                    double dValue = Convert.ToDouble(e.Row.Cells[i].Value);
                                    e.Row.Cells[i].Value = string.Format("{0:P2}", dValue);
                                    e.Row.Cells[i].Title = "Темп прироста к предыдущему периоду";

                                }
                                e.Row.Cells[0].Value = e.Row.PrevRow.Cells[0].Value;
                                e.Row.Cells[i].Style.BorderDetails.WidthTop = 0;
                                e.Row.Cells[i].Style.BorderDetails.WidthBottom = 1;
                            break;
                        }
                    
                }
            }

          
        }

        #endregion

        #region Обработчики карты 2

        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(string.Format("../../maps/Субъекты/{0}/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;

            map.LoadFromShapeFile(layerName, "Name", true);
            map.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
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

        /// <summary>
        /// Поиск формы карты
        /// </summary>
        /// <param name="map">карта</param>
        /// <param name="patternValue">искомое имя формы</param>
        /// <returns>найденные формы</returns>
        public static ArrayList FindMapShape(MapControl map, string patternValue, out bool hasCallout)
        {
            hasCallout = false;
            ArrayList shapeList = new ArrayList();
            foreach (Shape shape in map.Shapes)
            {
                if (GetShapeName(shape).ToLower() == patternValue.ToLower())
                {
                    shapeList.Add(shape);
                    if (IsCalloutTownShape(shape))
                    {
                        hasCallout = true;
                    }
                }
            }

            return shapeList;
        }


        public void FillMapData(MapControl map)
        {
            LabelMap.Text = string.Format("Площадь загрязнения и масса загрязняющих веществ после ликвидации аварии {2} {0} квартале {1} года", quarter, year, quarter == 2 ? "во" : "в");
            
            if (map == null)
            {
                return;
            }

            string query = DataProvider.GetQueryText("STAT_0004_0001_map");
            DataTable dtMap = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtMap);

            foreach (DataRow row in dtMap.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty && row[1] != DBNull.Value && row[1].ToString() != string.Empty 
                    && row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                {
                    string regionName = row[0].ToString().Replace(" муниципальный район", String.Empty).Replace("Город ", "г.");

                    bool hasCallout;
                    ArrayList shapeList = FindMapShape(map, regionName, out hasCallout);
                    foreach (Shape shape in shapeList)
                    {
                        double area = Convert.ToDouble(row[1]);
                        double massDirt = Convert.ToDouble(row[2]);
                        double massDirtAfter = Convert.ToDouble(row[3]);
                        double totalCount = massDirtAfter + massDirt;

                        shape["Name"] = row[0].ToString();
                        shape["TensionKoeff"] = area;
                        if (!IsCalloutTownShape(shape) && (hasCallout))
                        {
                            shape.Visible = false;
                        }
                        else
                        {
                            shape.ToolTip = String.Format("{0} \n Площадь загрязнения {1:N4} га \n Масса ликвидированных загрязняющих веществ {2:N3} т \n Масса загрязняющих веществ после аварии {3:N3} т",
                                    row[0], area, massDirt, massDirtAfter);

                            shape.TextColor = Color.Black;
                            shape.BorderWidth = 2;
                            shape.TextVisibility = TextVisibility.Shown;
                            if (!IsCalloutTownShape(shape))
                            {
                                shape.Text = string.Format("{0} \n {1:N4}, га", shape.Name, area, massDirtAfter);
                                Symbol symbol = new Symbol();
                                symbol.Name = shape.Name + map.Symbols.Count;
                                symbol.ParentShape = shape.Name;
                                symbol["vacancyCount"] = totalCount == 0 ? 0 : massDirtAfter / totalCount * 100;
                                symbol["redundantCount"] = totalCount == 0 ? 0 : massDirt / totalCount * 100;
                                symbol.Offset.Y = -33;
                                symbol.MarkerStyle = MarkerStyle.Circle;
                                map.Symbols.Add(symbol);
                                if (shape.Name.Contains("Сургутский"))
                                {
                                    shape.CentralPointOffset.Y = 0.7;
                                }
                                else if (shape.Name.Contains("Белоярский") || shape.Name.Contains("Кондинский") || shape.Name.Contains("Нефтеюганский"))
                                {
                                    shape.CentralPointOffset.Y = 0.3;
                                }
                                else
                                {
                                    shape.CentralPointOffset.Y = 0.5;
                                }
                            }
                            else
                            {
                                shape.Visible = false;
                                //shape.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;
                                //shape.Text = string.Format("{0} {1:N2}", shape.Name, area, massDirtAfter);
                            }
                        }
                    }
                }
            }
        }

        void DundasMap_PostPaint(object sender, MapPaintEventArgs e)
        {
            Symbol symbol = e.MapElement as Symbol;
            if (symbol != null && symbol.Visible)
            {
                // Размер диаграммы
                int width = 30;
                int height = 30;

                // Get the symbol location in pixels.
                MapGraphics mg = e.Graphics;
                PointF p = symbol.GetCenterPointInContentPixels(mg);
                int x = (int)p.X - width / 2;
                int y = (int)p.Y - height / 2;
                symbol.Width = width;
                symbol.Height = height;

                int startAngle, sweepAngle1, sweepAngle2;
                if (symbol["redundantCount"].ToString() != "0" || symbol["vacancyCount"].ToString() != "0")
                {

                    // Делим углы соотвественно долям
                    startAngle = 0;
                    sweepAngle1 = Convert.ToInt32(3.6*Convert.ToInt32(symbol["redundantCount"]));
                    sweepAngle2 = Convert.ToInt32(3.6*Convert.ToInt32(symbol["vacancyCount"]));

                    // Поверх символа рисуем круговую диаграмму
                    Graphics g = mg.Graphics;
                    g.FillPie(new SolidBrush(Color.DarkViolet), x, y, width, height, startAngle, sweepAngle1);
                    startAngle += sweepAngle1;
                    g.FillPie(new SolidBrush(Color.Black), x, y, width, height, startAngle, sweepAngle2);

                    g.DrawEllipse(new Pen(Color.Gray, 1), x, y, width, height);
                }
            }
        }
        #endregion

        #region Экспорт

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 20;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
            
            ReportExcelExporter1.HeaderCellHeight = 20;
            ReportExcelExporter1.GridColumnWidthScale = 1.5;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(DundasMap, LabelMap.Text ,sheet2, 3);
         
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
         
            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();

            ReportPDFExporter1.HeaderCellHeight = 40;

            for (int i = 0; i < UltraWebGrid.Rows.Count; i+=3)
            {
                
                UltraWebGrid.Rows[i].Cells[0].Style.BorderDetails.WidthBottom = 0;
                UltraWebGrid.Rows[i + 1].Cells[0].Style.BorderDetails.WidthTop = 0;
                UltraWebGrid.Rows[i + 1].Cells[0].Style.BorderDetails.WidthBottom = 0;
                UltraWebGrid.Rows[i + 1].Cells[0].Value = "          ";
                UltraWebGrid.Rows[i + 2].Cells[0].Value = "          ";
            }
            
            ReportPDFExporter1.Export(headerLayout, section1);
            DundasMap.NavigationPanel.Visible = false;
            DundasMap.ZoomPanel.Visible = false;
            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            ReportPDFExporter1.Export(DundasMap, LabelMap.Text, section2);
        }

        #endregion

        #endregion

    }
}




using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SgmSupport;

namespace Krista.FM.Server.Dashboards.reports.SGM.SGM_0001
{
    public partial class SGM_0001 : CustomReportPage
    {
        // Размер диаграмки для субъекта на карте
        const int SymbolDiagramSize = 25;
        // Заголовки в таблице данных
        const string TableCaptionAll = "Все жители";
        const string TableCaption1 = "До 14 лет";
        const string TableCaption2 = "15-17 лет";
        const string TableCaption3 = "Территория";
        const string TableCaption4 = "ФО";
        const string TableCaption5 = "Абс.";
        const string TableCaption6 = "На 100 тыс.";
        const string TableCaption8 = "Взрослые";               
        // Имена элементов карты
        const string MapShapeName = "MapShapeInfo";
        const string MapRuleName = "MapRuleInfo";
        const string MapMeasureFieldName1 = "MapMeasureField1";
        const string MapSymbolFieldName1 = "f1";
        const string MapSymbolFieldName2 = "f2";
        const string MapSymbolFieldName3 = "f3";
        const string MapFNName = "NAME";
        string diesCodes = String.Empty;

        // Текущая дата
        private DataTable tblData = new DataTable();
        // Номера предыдущего года и месяца
        private string year1, year2;
        private string month;

        // словарик исключений имен субъектов для карты(для соответсвия имени на карте и в базе)
        protected Dictionary<string, string> difficultNamesDict = new Dictionary<string, string>();

        private readonly double[] maxChartValue = new double[3];
        private readonly double[] minChartValue = new double[3];

        private readonly SGMDataRotator dataRotator = new SGMDataRotator();
        private readonly SGMDataObject dataObject = new SGMDataObject();
        private readonly SGMRegionNamer regionNamer = new SGMRegionNamer();
        private readonly SGMSupport supportClass = new SGMSupport();
        private readonly SGMPdfExportHelper exportClass = new SGMPdfExportHelper();

        /// <summary>
        ///  Загрузка страницы
        /// </summary>
        /// <param name="sender">Страница</param>
        /// <param name="e">Параметры</param>
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            dataObject.reportFormRotator = dataRotator;
            dataRotator.CheckSubjectReport();
            dataRotator.formNumber = 1;
            // Получение таблиц субъектов и ФО
            dataObject.InitObject();
            dataRotator.FillRegionsDictionary(dataObject.dtAreaShort);
            regionNamer.FillFMtoSGMRegions();

            if (!Page.IsPostBack)
            {
                dataRotator.FillSGMMapList(ComboMap, dataObject.dtAreaShort, false);
                dataRotator.FillYearList(ComboYear);
                dataRotator.FillMonthListEx(ComboMonth, ComboYear.SelectedValue);
                dataRotator.FillDeseasesList(ComboDesease, 0);
            }
            else
            {
                dataRotator.FillSGMMapList(null, dataObject.dtAreaShort, false);
                dataRotator.FillDeseasesList(null, 0);            
            }

            year1 = ComboYear.SelectedValue;
            year2 = Convert.ToString(Convert.ToInt32(year1) - 1);
            month = dataRotator.GetMonthParamString(ComboMonth, ComboYear.SelectedValue);
            dataRotator.CheckFormNumber(Convert.ToInt32(year1), ref month);
            dataRotator.mapKind = GetMapIndex(ComboMap.SelectedValue);
            // Заполнение словарика исключений
            FillDifficultNames();
            FillMainData();
            ConfigureChart();
            // Загружаем фигуры на карту
            LoadMapData();
            // Заполнение данными полей фигур, символов, тултипов,...
            FillMapData();
            // Скрывание диаграммы по одиночному субъекту, если выбран тип деления по регионам или областяи одного субъекта
            SetComponentVisibilityAndSize();
            FillChartData();
            // Заполняем грид данными
            tblData = DeleteServiceColumns(tblData);
            dataRotator.FillMaxAndMin(tblData, 2);
            grid.DataSource = tblData;
            grid.DataBind();
            // Настройка заголовка и колонок грида
            ConfigureGrid();
            const string commonCaptionStr = "Структура заболеваемости по группам населения";
            LabelChartTitle.Text = dataRotator.mapKind == MapKindEnum.AllRegions ?
                String.Format("{0} (сравнение РФ и федеральных округов)", commonCaptionStr) :
                String.Format("{0} (сравнение территорий {1})", commonCaptionStr, supportClass.GetFOShortName(ComboMap.SelectedValue));

            Page.Title = String.Format("Сведения о заболеваемости - {0}", ComboDesease.SelectedValuesString);
            PageTitle.Text = Page.Title;
            LabelSubTitle.Text = String.Format("{0}, {1} {2}г.{3}",
                ComboMap.SelectedValue.Trim(),
                dataRotator.GetMonthParamLabel(ComboMonth, ComboYear.SelectedValue),
                ComboYear.SelectedValue,
                dataRotator.GetFormHeader());
        }

        /// <summary>
        /// Заполнение словаря исключений названий субъектов(сокращения, знаки препинания, слитное написание и тд)
        /// </summary>
        protected virtual void FillDifficultNames()
        {
            // Словарик исключений по именам субъектов
            difficultNamesDict.Add("Чеченская", "Чечня");
            difficultNamesDict.Add("Удмуртская", "Удмуртия");
            difficultNamesDict.Add("Чувашская", "Чувашия");
            difficultNamesDict.Add("Респ.Дагестан", "Дагестан");
            difficultNamesDict.Add("Ханты-МансийскАО", "Ханты-Мансийский АО");
            difficultNamesDict.Add("Ямало-НенецкийАО", "Ямало-Ненецкий АО");
            difficultNamesDict.Add("Ставропольск.кр.", "Ставропольский край");
            difficultNamesDict.Add("Ингушская", "Ингушетия");
            difficultNamesDict.Add("Кабард.-Балкария", "Кабардино-Балкария");
            difficultNamesDict.Add("Карач.-Черк.Респ", "Карачаево-Черкессия");
            difficultNamesDict.Add("Камчатская", "Камчатский");
            difficultNamesDict.Add("Пермская", "Пермский");
            difficultNamesDict.Add("г.Москва", "г. Москва");
            difficultNamesDict.Add("Калининград.обл.", "Калининградская");
        }

        #region MapRegion

        /// <summary>
        /// Возвращает данные по какой карте выводить(регион, все регионы, один субъект)
        /// </summary>
        protected virtual MapKindEnum GetMapIndex(string value)
        {
            if (dataRotator.mapList.IndexOf(value) == 0)
            {
                return MapKindEnum.AllRegions;
            }

            return MapKindEnum.SingleRegion;
        }

        /// <summary>
        /// Загрузка элементов на карту
        /// </summary>
        protected virtual void LoadMapData()
        {
            const string mapPath = "../../../maps/{0}/{0}.shp";
            map.Shapes.Clear();
            switch (dataRotator.mapKind)
            {
                case MapKindEnum.AllRegions:
                    map.LoadFromShapeFile(Server.MapPath(String.Format(mapPath, "РФ")), MapFNName, true);
                    break;
                case MapKindEnum.SingleRegion:
                    string regionStr = ComboMap.SelectedValue;
                    map.LoadFromShapeFile(Server.MapPath(String.Format(mapPath, supportClass.GetFOShortName(regionStr))), MapFNName, true);
                    break;
            }
        }
      
        /// <summary>
        /// Добавление служебного поля для символа фигуры
        /// </summary>
        /// <param name="name"> Имя добавляемого поля </param>
        protected virtual void AddServiceSymbolField(string name)
        {
            map.SymbolFields.Add(name);
            map.SymbolFields[name].Type = typeof(int);
            map.SymbolFields[name].UniqueIdentifier = false;
        }

        /// <summary>
        /// Добавление служебного поля для фигуры
        /// </summary>
        /// <param name="name">Имя добавляемого поля</param>
        protected virtual void AddServiceShapeField(string name)
        {
            map.ShapeFields.Add(name);
            map.ShapeFields[name].Type = typeof(int);
            map.ShapeFields[name].UniqueIdentifier = false;
        }

        /// <summary>
        /// Создание структуры для отображения данных на карте(поля, символы, правила раскраски)
        /// </summary>
        protected virtual void CreateMapShapes()
        {
            var mapFont = new Font("MS Sans Serif", 7, FontStyle.Regular);
            // добавляем легенду
            var legend = new Legend("MapLegend")
                             {
                                 Visible = true,
                                 BackColor = Color.White,
                                 BackSecondaryColor = Color.Gainsboro,
                                 BackGradientType = GradientType.DiagonalLeft,
                                 BackHatchStyle = MapHatchStyle.None,
                                 BorderColor = Color.Gray,
                                 BorderWidth = 1,
                                 BorderStyle = MapDashStyle.Solid,
                                 BackShadowOffset = 4,
                                 TextColor = Color.Black,
                                 Font = mapFont,
                                 AutoFitText = true,
                                 Title = "Заболеваемость на 100 тыс.чел.",
                                 AutoFitMinFontSize = 7
                             };

            map.Legends.Clear();
            legend.Dock = PanelDockStyle.Right;
            map.Legends.Add(legend);

            legend = new Legend("MapLegend2")
                         {
                             Visible = true,
                             BackColor = Color.White,
                             BackSecondaryColor = Color.Gainsboro,
                             BackGradientType = GradientType.DiagonalLeft,
                             BackHatchStyle = MapHatchStyle.None,
                             BorderColor = Color.Gray,
                             BorderWidth = 1,
                             BorderStyle = MapDashStyle.Solid,
                             BackShadowOffset = 4,
                             TextColor = Color.Black,
                             Font = mapFont,
                             AutoFitText = true,
                             Title = "Структура заболеваемости",
                             AutoFitMinFontSize = 7,
                             Dock = PanelDockStyle.Right
                         };

            var item = new LegendItem {Text = "Взрослые", Color = Color.DarkGray};
            legend.Items.Add(item);

            item = new LegendItem {Text = "Дети", Color = Color.DimGray};
            legend.Items.Add(item);

            item = new LegendItem {Text = "Подростки", Color = Color.LightGray};
            legend.Items.Add(item);

            map.Legends.Add(legend);

            // добавляем поля для фигур
            map.ShapeFields.Clear();
            map.ShapeFields.Add(MapShapeName);
            map.ShapeFields[MapShapeName].Type = typeof(string);
            map.ShapeFields[MapShapeName].UniqueIdentifier = true;
            map.ShapeFields.Add(MapMeasureFieldName1);
            map.ShapeFields[MapMeasureFieldName1].Type = typeof(double);
            map.ShapeFields[MapMeasureFieldName1].UniqueIdentifier = false;
           
            AddServiceShapeField(MapSymbolFieldName1);
            AddServiceShapeField(MapSymbolFieldName2);
            AddServiceShapeField(MapSymbolFieldName3);

            map.ShapeRules.Clear();
            var rule = new ShapeRule
                           {
                               Name = MapRuleName,
                               Category = String.Empty,
                               ShapeField = MapMeasureFieldName1,
                               DataGrouping = DataGrouping.EqualDistribution,
                               ColorCount = 5,
                               ColoringMode = ColoringMode.ColorRange,
                               FromColor = Color.Green,
                               MiddleColor = Color.Yellow,
                               ToColor = Color.Red,
                               BorderColor = Color.FromArgb(50, Color.Black),
                               GradientType = GradientType.None,
                               HatchStyle = MapHatchStyle.None,
                               ShowInColorSwatch = false,
                               ShowInLegend = "MapLegend"
                           };

            map.ShapeRules.Add(rule);

            // Добавляем поля для символов
            map.SymbolFields.Clear();
            map.SymbolFields.Add(MapShapeName);
            map.SymbolFields[MapShapeName].Type = typeof(string);
            map.SymbolFields[MapShapeName].UniqueIdentifier = true;
            map.SymbolFields.Add(MapMeasureFieldName1);
            map.SymbolFields[MapMeasureFieldName1].Type = typeof(double);
            map.SymbolFields[MapMeasureFieldName1].UniqueIdentifier = false;

            AddServiceSymbolField(MapSymbolFieldName1);
            AddServiceSymbolField(MapSymbolFieldName2);
            AddServiceSymbolField(MapSymbolFieldName3);
        }

        /// <summary>
        /// По типу измерителя заполняем нужные параметры фигуры
        /// </summary>
        protected virtual void FillShapeMeasuresData(Shape shape, DataRow row)
        {
            // Если данные есть, то запишем их в поле данных и добавим формат в всплывающую подсказку
            shape[MapMeasureFieldName1] = Convert.ToDouble(row[3]);
            shape.ToolTip = string.Format("{0}\n Средняя инфекционная заболеваемость составила #{1}{2}",
                                          shape.ToolTip,
                                          MapMeasureFieldName1.ToUpper(),
                                          "{N2} чел. на 100 тыс.чел.");
            // До 14
            shape[MapSymbolFieldName2] = Convert.ToInt32(row[11]);
            // 15-17
            shape[MapSymbolFieldName3] = Convert.ToInt32(row[21]);
            // Взрослые
            shape[MapSymbolFieldName1] = Convert.ToInt32(row[16]);
        }

        /// <summary>
        /// Поиск фигуры на карте по части имени
        /// </summary>
        protected virtual Shape FindMapShape(MapControl mapObj, string searchValue)
        {
            string subject = string.Empty;
            char splitter = ' ';

            // Пробуем сначала бить строку на части по пробелам(как показала практика это далеко не всегда правильно)
            string[] subjects = searchValue.Split(splitter);

            if (subjects.Length > 0)
            {
                // Сначал проверяем на соответствие исключениям
                if (difficultNamesDict.ContainsKey(subjects[0]))
                {
                    subject = difficultNamesDict[subjects[0]];
                }
                else
                {
                    // Простейший способ определения республики
                    bool isRepublic = supportClass.FindInStr(searchValue, "Республика");
                    if (!isRepublic)
                    {
                        // Теперь более сложные
                        isRepublic = supportClass.FindInStr(searchValue, "Рес.");
                        isRepublic = isRepublic || supportClass.FindInStr(searchValue, "Респ.");
                        isRepublic = isRepublic || supportClass.FindInStr(searchValue, "Р.");
                        if (isRepublic)
                        {
                            // Если один из сложных, то задаем новый разделитель вместо пробела
                            splitter = '.';
                            // И заново делим строку на части
                            subjects = searchValue.Split(splitter);
                        }
                    }
                    // Подстрока для поиска фигуры на карте
                    subject = (isRepublic) ? subjects[1] : subjects[0];
                }
            }

            // Ищем нужную фигуру
            for (int i = 0; i < mapObj.Shapes.Count; i++)
            {
                if (supportClass.FindInStr(mapObj.Shapes[i].Name, subject)) return mapObj.Shapes[i];
            }
            // Не нашли
            return null;
        }

        /// <summary>
        /// Заполнение данными всех фигур карты
        /// </summary>
        protected virtual void FillMapData()
        {
            string noDataText = String.Format("#{0} {1}", MapFNName, "<НЕТ ДАННЫХ>");
            // Сначала всем фигурам проставим подсказку что данных нет
            for (int i = 0; i < map.Shapes.Count; i++)
            {
                map.Shapes[i].ToolTip = noDataText;
            }

            foreach (DataRow row in tblData.Rows)
            {
                string searchValue = Convert.ToString(row[0]);
                searchValue = dataRotator.mapKind != MapKindEnum.AllRegions ? 
                    searchValue : 
                    searchValue.Substring(0, Math.Min(8, searchValue.Length));

                // Заполнение данных по субъектам
                Shape shape = FindMapShape(map, searchValue.Trim());
                if (shape != null)
                {
                    shape[MapShapeName] = Convert.ToString(row[0]).Trim();
                    shape.ToolTip = String.Format("#{0}", MapFNName);
                    FillShapeMeasuresData(shape, row);
                }
            }
            
            // Настраиваем фигуры карты
            foreach (Shape shape in map.Shapes)
            {
                // Текст названия скрываем
                shape.TextVisibility = TextVisibility.Hidden;
                // Создаем символы для отображения относительных показателей
                CreateSymbol(shape, Color.DarkGray);
            }

            if (Session["WordExportMode"] != null && (bool)Session["WordExportMode"])
            {
                map.ZoomPanel.Visible = false;
                map.NavigationPanel.Visible = false;
                map.RenderType = RenderType.ImageTag;
            }
        }

        /// <summary>
        ///  Создание символа отображения показателя относительной статистики
        /// </summary>
        /// <param name="shape">Фигура предок</param>
        /// <param name="color">Цвет символа</param>
        protected virtual void CreateSymbol(Shape shape, Color color)
        {
            // Поле данных должно быть заполнено
            if (shape[MapShapeName] != null)
            {
                var symbol = new Symbol
                                 {
                                     Name = String.Format("{0}{1}", MapShapeName, map.Symbols.Count),
                                     ParentShape = shape.Name
                                 };

                symbol[MapShapeName] = shape[MapShapeName];
                symbol[MapSymbolFieldName1] = shape[MapSymbolFieldName1];
                symbol[MapSymbolFieldName2] = shape[MapSymbolFieldName2];
                symbol[MapSymbolFieldName3] = shape[MapSymbolFieldName3];

                symbol.ToolTip = string.Format("#{0}:\n {1} - #{2}% \n {3} - #{4}%\n {5} - #{6}%",
                    MapShapeName.ToUpper(),
                    "взрослые ", MapSymbolFieldName1.ToUpper(),
                    "до 14 лет ", MapSymbolFieldName2.ToUpper(),
                    "15-17 лет ", MapSymbolFieldName3.ToUpper());
                symbol.Color = color;
                symbol.Width = SymbolDiagramSize;
                symbol.Height = symbol.Width;
                map.Symbols.Add(symbol);
            }

        }
        #endregion

        #region InterfaceRegion


        /// <summary>
        /// Отрисовка круговых диаграмм на фигурах карты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void map_PostPaint(object sender, MapPaintEventArgs e)
        {

            var symbol = e.MapElement as Symbol;
            if (symbol != null && symbol.Visible)
            {
                // Размер диаграммы
                const int width = SymbolDiagramSize;
                const int height = SymbolDiagramSize;

                // Get the symbol location in pixels.
                MapGraphics mg = e.Graphics;
                PointF p = symbol.GetCenterPointInContentPixels(mg);
                int x = (int)p.X - width / 2;
                int y = (int)p.Y - height / 2;
                symbol.Width = width;
                symbol.Height = height;

                // Делим углы соотвественно долям заболевших групп населения
                int startAngle = 0;
                int sweepAngle1 = Convert.ToInt32(3.6 * Convert.ToInt32(symbol[MapSymbolFieldName1]));
                int sweepAngle2 = Convert.ToInt32(3.6 * Convert.ToInt32(symbol[MapSymbolFieldName2]));
                int sweepAngle3 = Convert.ToInt32(3.6 * Convert.ToInt32(symbol[MapSymbolFieldName3]));

                // Поверх символа рисуем круговую диаграмму
                Graphics g = mg.Graphics;
                g.FillPie(new SolidBrush(Color.DarkGray), x, y, width, height, startAngle, sweepAngle1);
                startAngle += sweepAngle1;
                g.FillPie(new SolidBrush(Color.DimGray), x, y, width, height, startAngle, sweepAngle2);
                startAngle += sweepAngle2;
                g.FillPie(new SolidBrush(Color.LightGray), x, y, width, height, startAngle, sweepAngle3);
                g.DrawEllipse(new Pen(Color.Gray, 1), x, y, width, height);
            }
        }

        /// <summary>
        ///  Создание сгруппированных заголовков
        /// </summary>
        /// <param name="sender">Грид</param>
        /// <param name="e">Параметры</param>
        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            // Номер полосы для создания групповых заголовков в таблице
            const int BandIndex = 0;

            RowLayoutColumnInfo rlcInfo;
            e.Layout.GroupByBox.Hidden = true;

            for (int i = 0; i < e.Layout.Bands[BandIndex].Columns.Count; i++)
            {
                rlcInfo = e.Layout.Bands[BandIndex].Columns[i].Header.RowLayoutColumnInfo;
                if (i == 0 || i == 1)
                {
                    rlcInfo.OriginY = 0;
                    rlcInfo.SpanY = 2;
                }
                else
                {
                    rlcInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 2;
            var ch = new ColumnHeader(true)
                         {
                             Caption = TableCaptionAll,
                             RowLayoutColumnInfo =
                                 {
                                     OriginY = 0,
                                     OriginX = multiHeaderPos
                                 }
                         };

            multiHeaderPos += 3;
            ch.RowLayoutColumnInfo.SpanX = 3;
            ch.Style.Wrap = true;
            ch.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[BandIndex].HeaderLayout.Add(ch);

            for (int i = 5; i < e.Layout.Bands[BandIndex].Columns.Count; i += 4)
            {
                ch = new ColumnHeader(true);
                
                if (i == 05) ch.Caption = TableCaption1;
                if (i == 09) ch.Caption = TableCaption8;
                if (i == 13) ch.Caption = TableCaption2;

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 4;
                ch.RowLayoutColumnInfo.SpanX = 4;
                ch.Style.Wrap = true;
                ch.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[BandIndex].HeaderLayout.Add(ch);
            }
        }

        /// <summary>
        ///  Настройки грида(колонки - ширина, заголовки,форматирование данных)
        /// </summary>
        protected virtual void ConfigureGrid()
        {
            int colIndex = 1;
            // Настраиваем колонки Название субъекта и ФО
            supportClass.SetColumnWidthAndCaption(grid, 0, TableCaption3, 240, HorizontalAlign.Left, String.Empty);
            supportClass.SetColumnWidthAndCaption(grid, 1, TableCaption4, 50, HorizontalAlign.Left, String.Empty);

            string yearText = ComboYear.SelectedValue;
            string monthText = dataRotator.GetMonthParamLabel(ComboMonth, ComboYear.SelectedValue);
            string label1Text = String.Format("Абсолютное количество заболевших {0} {1} года", monthText, yearText);
            string label2Text = String.Format("Относительное количество заболевших на 100 тысяч населения территории {0} {1} года", monthText, yearText);
            const string label3Text = "Процент группы от общей заболеваемости";
            string label4Text = String.Format("{0} {1} года, на 100 тыс.человек", monthText, year2);

            // Настраиваем числовые колонки
            for (int i = 0; i < 4; i++)
            {
                colIndex++;
                supportClass.SetColumnWidthAndCaption(grid, colIndex, TableCaption5, 70, HorizontalAlign.Right, label1Text);
                CRHelper.FormatNumberColumn(grid.Columns[colIndex], "N0");
                colIndex++;
                supportClass.SetColumnWidthAndCaption(grid, colIndex, TableCaption6, 90, HorizontalAlign.Center, label2Text);
                CRHelper.FormatNumberColumn(grid.Columns[colIndex], "N2");
               
                colIndex++;
                supportClass.SetColumnWidthAndCaption(grid, colIndex, "Аналогичный период прошлого года, на 100 тыс.",
                    90, HorizontalAlign.Center, label4Text);
                CRHelper.FormatNumberColumn(grid.Columns[colIndex], "N2");

                if (i != 0)
                {
                    colIndex++;
                    supportClass.SetColumnWidthAndCaption(grid, colIndex, "Доля группы", 50, HorizontalAlign.Center, label3Text);
                    CRHelper.FormatNumberColumn(grid.Columns[colIndex], "N2");
                }
            }
        }
        
        protected void grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (dataRotator.mapKind != MapKindEnum.AllRegions)
            {
                if (e.Row.Index == 1 || e.Row.Index == 0) e.Row.Cells[0].Style.Font.Bold = true;
            }
            else
            {
                if (e.Row.Index == 0) e.Row.Cells[1].Style.Font.Bold = true;
            }

            bool hasData = false;
            foreach (string diesName in ComboDesease.SelectedValues)
            {
                string diesCode = dataRotator.deseasesRelation[diesName];
                hasData = hasData || supportClass.CheckDeseasePeriod(diesCode, Convert.ToInt32(year1), Convert.ToInt32(year2));

            }

            if (hasData)
            {
                supportClass.SetCellImageEx(e.Row, 4, 3, 4);
                for (int i = 0; i < 3; i++)
                {
                    int cellIndex = 07 + i*4;
                    supportClass.SetCellImageEx(e.Row, cellIndex, cellIndex - 1, cellIndex);
                }
            }

            supportClass.SetCellImageStar(e.Row, dataRotator.maxValues, dataRotator.minValues, 3, 3, 3);

            for (int i = 0; i < 3; i++)
            {
                supportClass.SetCellImageStar(e.Row, dataRotator.maxValues, dataRotator.minValues, 6 + i * 4, 6 + i * 4, 6 + i * 4);
            }
        }

        #endregion

        #region PageHandlers

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            CreateMapShapes();
            SetExportHandlers();
        }

        /// <summary>
        /// Разворачивает таблицу данных для представления ввиде графика(параметры датасеты по РФ, Региону, Субъекту)
        /// </summary>
        /// <param name="tblChart"></param>
        /// <returns></returns>
        protected virtual DataTable GetDataForChart(DataTable tblChart)
        {
            var dtResult = new DataTable();

            DataColumn dataColumn = dtResult.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");

            dataColumn = dtResult.Columns.Add();
            dataColumn.DataType = Type.GetType("System.Double");
            dataColumn.ColumnName = "Дети";

            dataColumn = dtResult.Columns.Add();
            dataColumn.DataType = Type.GetType("System.Double");
            dataColumn.ColumnName = "Взрослые";

            dataColumn = dtResult.Columns.Add();
            dataColumn.DataType = Type.GetType("System.Double");
            dataColumn.ColumnName = "Подростки";

            for (int i = 0; i < 3; i++)
            {
                maxChartValue[i] = Double.MinValue;
                minChartValue[i] = Double.MaxValue;
            }

            for (int i = 0; i < tblChart.Rows.Count; i++)
            {
                dtResult.Rows.Add();

                dtResult.Rows[i][0] = tblChart.Rows[i][0];
                if (dataRotator.mapKind == MapKindEnum.AllRegions)
                {
                    dtResult.Rows[i][0] = supportClass.GetFOShortName(Convert.ToString(dtResult.Rows[i][0]));                    
                }

                dtResult.Rows[i][1] = tblChart.Rows[i][11];
                dtResult.Rows[i][2] = tblChart.Rows[i][06];
                dtResult.Rows[i][3] = tblChart.Rows[i][21];

                maxChartValue[0] = Math.Max(maxChartValue[0], Convert.ToDouble(dtResult.Rows[i][1]));
                minChartValue[0] = Math.Min(minChartValue[0], Convert.ToDouble(dtResult.Rows[i][1]));
                maxChartValue[1] = Math.Max(maxChartValue[1], Convert.ToDouble(dtResult.Rows[i][2]));
                minChartValue[1] = Math.Min(minChartValue[1], Convert.ToDouble(dtResult.Rows[i][2]));
                maxChartValue[2] = Math.Max(maxChartValue[2], Convert.ToDouble(dtResult.Rows[i][3]));
                minChartValue[2] = Math.Min(minChartValue[2], Convert.ToDouble(dtResult.Rows[i][3]));
            }

            return dtResult;
        }

        protected virtual void SetComponentVisibilityAndSize()
        {

            double screenWidth = Convert.ToDouble(Session[CustomReportConst.ScreenWidthKeyName]);
            double screenHeight = Convert.ToDouble(Session[CustomReportConst.ScreenHeightKeyName]);
            int dirtyWidth = Convert.ToInt32(screenWidth / 3 - 10);
            int dirtyHeight = Convert.ToInt32((screenHeight - 200) / 3);

            map.Width = dirtyWidth * 3; map.Height = dirtyHeight * 2;
            grid.Width = map.Width;
            grid.Height = Unit.Empty;
            chart.Height = map.Height;
            chart.Width = map.Width;
            PageTitle.Width = (Unit)(map.Width.Value - 100);
            LabelSubTitle.Width = PageTitle.Width;
            LabelChartTitle.Width = map.Width;
        }

        /// <summary>
        /// Настройка сравнительной диаграммы
        /// </summary>
        protected virtual void ConfigureChart()
        {
            // Легенда            
            chart.Legend.Margins.Right = Convert.ToInt32(chart.Width.Value * 0.5);
            chart.Legend.SpanPercentage = 10;
            chart.Tooltips.FormatString = "<SERIES_LABEL>\n <ITEM_LABEL>\n <DATA_VALUE:N2>%";
            chart.Legend.FormatString = "<ITEM_LABEL>";
            chart.Data.ZeroAligned = true;
            chart.Axis.Y.Extent = 40;
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            chart.Axis.X.Extent = 120;
            chart.Axis.X.Labels.Visible = false;
            chart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            chart.Axis.X.Labels.SeriesLabels.Visible = true;
            chart.Visible = true;
            chart.Legend.Location = LegendLocation.Top;
            chart.TitleLeft.Text = "Заболеваемость на 100 тысяч населения";
            chart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            chart.TitleLeft.Visible = true;
            chart.TitleLeft.Font = new Font("Verdana", 10, FontStyle.Bold);

            // Числовое значения на барах
            var appearance = new ChartTextAppearance
                                 {
                                     Column = -2,
                                     Row = -2,
                                     HorizontalAlign = StringAlignment.Far,
                                     ItemFormatString = "<DATA_VALUE:N2>",
                                     ChartTextFont = new Font("Verdana", 8),
                                     Visible = true
                                 };

            chart.BarChart.ChartText.Add(appearance);
        }

        protected DataTable DeleteServiceColumns(DataTable tblDirtyData)
        {
            for (int i = 3; i >= 1; i--)
            {
                tblDirtyData.Columns.RemoveAt(i * 5 + 4);
            }

            tblDirtyData.Columns.RemoveAt(6);
            tblDirtyData.Columns.RemoveAt(4);

            return tblDirtyData;
        }

        protected void FillMainData()
        {
            FillDataList();
            DataTable tblFO = dataObject.FilterDataSet(tblData, String.Format("{0} = ''", tblData.Columns[1].ColumnName));


            if (dataRotator.mapKind == MapKindEnum.AllRegions)
            {
                tblData = tblFO;
            }
            else
            {
                string foName = supportClass.GetFOShortName(ComboMap.SelectedValue);

                DataTable tblSubjects = dataObject.FilterDataSet(tblData, String.Format("{0} = '{1}'", 
                    tblData.Columns[1].ColumnName, foName));

                DataRow rowFO = null;
                foreach (DataRow dataRow in tblFO.Rows)
                {
                    if (supportClass.GetFOShortName(Convert.ToString(dataRow[0])) == foName)
                    {
                        rowFO = dataRow;
                    }
                }

                DataTable tblResult = tblData.Clone();
                if (tblData.Rows.Count > 0)
                {
                    tblResult.ImportRow(tblData.Rows[0]);
                }
                if (rowFO != null)
                {
                    tblResult.ImportRow(rowFO);  
                }

                foreach (DataRow rowSubject in tblSubjects.Rows)
                {
                    tblResult.ImportRow(rowSubject);   
                }

                tblData = tblResult;
            }
        }

        protected void FillDataList()
        {
            diesCodes = String.Empty;

            foreach (string diesName in ComboDesease.SelectedValues)
            {
                diesCodes = String.Format("{0},{1}", diesCodes, dataRotator.deseasesRelation[diesName]);
            }
            
            diesCodes = diesCodes.Trim(',');

            // Структура данных по болезням
            string groupName1 = Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtAll));
            string groupName2 = Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtChild));
            string groupName3 = Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtMature));
            string groupName4 = Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtTeen));
            var group = new string[] {groupName1, groupName2, groupName3, groupName4};

            dataObject.mainColumn = SGMDataObject.MainColumnType.mctMapName;
            dataObject.ignoreRegionConversion = true;
            dataObject.useLongNames = true;
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctFO, "0");

            for (int j = 0; j < group.Length; j++)
            {
                int offset = 5 * j;
                dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                                     year1, month, String.Empty, group[j], diesCodes);
                dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                                     Convert.ToString(offset + 2));
                dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                                     year2, month, String.Empty, group[j], diesCodes);
                dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                                     Convert.ToString(offset + 4));
                dataObject.AddColumn(SGMDataObject.DependentColumnType.dctPercentFromTotal,
                    "8, 13, 18", Convert.ToString(offset + 3));
            }

            tblData = dataObject.FillData();
        }

        #endregion

        protected void FillChartData()
        {
            chart.DataSource = GetDataForChart(tblData);
            chart.DataBind();
            chart.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            if (dataRotator.mapKind == MapKindEnum.AllRegions)
            {
                chart.Axis.X.Extent = 30;
                var labelXFont = new Font("Verdana", 15, FontStyle.Regular);
                chart.Axis.X.Labels.SeriesLabels.Font = labelXFont;
                chart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
                
                var labelYFont = new Font("Verdana", 08, FontStyle.Regular);
                chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>%";
                chart.Axis.Y.Labels.Font = labelYFont;
            }
            else
            {
                chart.Axis.X.Extent = 210;
                var labelXFont = new Font("Verdana", 12, FontStyle.Regular);
                chart.Axis.X.Labels.SeriesLabels.Font = labelXFont;
                chart.Height = (Unit)(Convert.ToDouble(map.Height.Value) + 110);
            }
            
            chart.Legend.Margins.Right = Convert.ToInt32(chart.Width.Value * 0.5);
        }

        protected void chart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
                if (primitive is Infragistics.UltraChart.Core.Primitives.Box)
                {
                    var box = (Infragistics.UltraChart.Core.Primitives.Box)primitive;
                    if (box.DataPoint != null)
                    {
                        var dpt = (NumericDataPoint)box.DataPoint;                        
                        bool needMin = false, needMax = false;
                        if (dpt.Value != 0)
                        {
                            needMin = (box.DataPoint.Label == "Взрослые" && dpt.Value == minChartValue[1]);
                            needMax = (box.DataPoint.Label == "Взрослые" && dpt.Value == maxChartValue[1]);
                            needMin = needMin || (box.DataPoint.Label == "Дети" && dpt.Value == minChartValue[0]);
                            needMax = needMax || (box.DataPoint.Label == "Дети" && dpt.Value == maxChartValue[0]);
                            needMin = needMin || (box.DataPoint.Label == "Подростки" && dpt.Value == minChartValue[2]);
                            needMax = needMax || (box.DataPoint.Label == "Подростки" && dpt.Value == maxChartValue[2]);
                        }

                        if (needMin || needMax)
                        {
                            var sym = new Infragistics.UltraChart.Core.Primitives.Symbol
                                          {
                                              icon = SymbolIcon.Diamond,
                                              iconSize = SymbolIconSize.Large,
                                              point =
                                                  {
                                                      X = (box.rect.Left + box.rect.Right)/2,
                                                      Y = box.rect.Y
                                                  },
                                              PE = {FillGradientStyle = GradientStyle.ForwardDiagonal}
                                          };

                            if (needMin)
                            {
                                box.DataPoint.Label = string.Format("{0}\n Минимальный уровень заболеваемости", box.DataPoint.Label);
                                sym.PE.Fill = Color.Orange;
                                sym.PE.FillStopColor = Color.DarkOrange;
                            }
                            else
                            {
                                box.DataPoint.Label = string.Format("{0}\n Максимальный уровень заболеваемости", box.DataPoint.Label);
                                sym.PE.Fill = Color.Gray;
                                sym.PE.FillStopColor = Color.DarkGray;
                            }
                            e.SceneGraph.Add(sym);
                        }
                    }
                }
            }
        }

        #region PDFExport

        private void SetExportHandlers()
        {
            UltraGridExporter1.PdfExportButton.Click += PdfExportButton_Click;
            UltraGridExporter1.PdfExporter.BeginExport += PdfExporter_BeginExport;
            UltraGridExporter1.PdfExporter.EndExport += PdfExporter_EndExport;
            UltraGridExporter1.ExcelExportButton.Visible = false;
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.WordExportButton.Visible = true;
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            InitializeExportLayout(e);

            exportClass.ExportCaptionText(e, PageTitle.Text);
            exportClass.ExportSubCaptionText(e, LabelSubTitle.Text);
            exportClass.ExportMap(e, map);
        }


        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            exportClass.ExportSubCaptionText(e, LabelChartTitle.Text);
            exportClass.ExportChart(e, chart);
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = string.Format("0001.pdf");
            UltraGridExporter1.HeaderChildCellHeight = 40;
            UltraGridExporter1.PdfExporter.Export(grid);
        }

        protected virtual void InitializeExportLayout(Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            map.Width = (Unit)(map.Width.Value - 100);
        }

        #endregion
    }
}

using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs=Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Orientation=Dundas.Maps.WebControl.Orientation;
using SerializationFormat=Dundas.Maps.WebControl.SerializationFormat;

namespace Krista.FM.Server.Dashboards.reports.MFRF_0003_0003
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtMap;
        private int firstYear = 2007;
        private int endYear = 2010;
        private DateTime date;
        private string debtKindStr;
        private string shortDebtKindStr;

        #endregion

        public bool FederalDebtSelected
        {
            get { return DebtKindButtonList.SelectedIndex == 0; }
        }

        public bool RFSelected
        {
            get { return ComboFO.SelectedIndex == 0; }
        }

        public bool PlanSelected
        {
            get { return MeasureButtonList.SelectedIndex == 0; }
        }

        #region Параметры запроса

        // Выбранный федеральный округ
        private CustomParam selectedFO;

        // Выбранный вид долга
        private CustomParam debtKind;

        // Выбранная мера
        private CustomParam selectedMeasure;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.6 - 50);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.85);

            #region Инициализация параметров запроса

            if (selectedFO == null)
            {
                selectedFO = UserParams.CustomParam("selected_fo");
            }

            if (debtKind == null)
            {
                debtKind = UserParams.CustomParam("debt_kind");
            }

            if (selectedMeasure == null)
            {
                selectedMeasure = UserParams.CustomParam("selected_measure");
            }

            #endregion

            #region Настройка карты

            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.ZoomPanel.Dock = PanelDockStyle.Left;
            //DundasMap.ZoomPanel.Orientation = Orientation.Horizontal;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.NavigationPanel.Dock = PanelDockStyle.Left;
            DundasMap.Viewport.EnablePanning = true;

            // добавляем легенду
            DundasMap.Legends.Clear();
            // добавляем легенду раскраски
            Legend legend = new Legend("DebtLegend");
            legend.Visible = true;
            legend.Dock = PanelDockStyle.Right;
            legend.BackColor = Color.White;
            legend.BackSecondaryColor = Color.Gainsboro;
            legend.BackGradientType = GradientType.DiagonalLeft;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Gray;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.Black;
            legend.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Add(legend);

            // добавляем поля для раскраски
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("DebtValue");
            DundasMap.ShapeFields["DebtValue"].Type = typeof(double);
            DundasMap.ShapeFields["DebtValue"].UniqueIdentifier = false;

            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "DebtRule";
            rule.Category = String.Empty;
            rule.ShapeField = "DebtValue";
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
            rule.ShowInLegend = "DebtLegend";
            rule.LegendText = "#FROMVALUE{N4} - #TOVALUE{N4}";
            DundasMap.ShapeRules.Add(rule);

            #endregion
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler <DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler <EndExportEventArgs>(PdfExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("MFRF_0003_0003_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query,"date", dtDate);
                date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);
                endYear = date.Year;

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                String month = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(date.Month));
                ComboMonth.SetСheckedState(month, true);

                ComboFO.Title = "ФО";
                ComboFO.Width = 300;
                ComboFO.MultiSelect = false;
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));
                ComboFO.SetСheckedState("Все федеральные округа", true);

                if (!string.IsNullOrEmpty(UserParams.Region.Value))
                {
                    ComboFO.SetСheckedState(UserParams.Region.Value, true);
                }
                else if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    string foName = RegionSettings.Instance.Id.ToLower() == "urfo"
                                         ? "Уральский федеральный округ"
                                         : RegionsNamingHelper.GetFoBySubject(RegionSettings.Instance.Name);
                    ComboFO.SetСheckedState(foName, true);
                }
            }

            DebtKindButtonList.RepeatDirection = RepeatDirection.Horizontal;

            DebtKindButtonList.Width = 380;

            int year = Convert.ToInt32(ComboYear.SelectedValue);
            DateTime currentDate = new DateTime(year, ComboMonth.SelectedIndex + 1, 1);
            
            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodLastYear.Value = (currentDate.Year - 1).ToString();
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));

            debtKind.Value = FederalDebtSelected ? "Гос.долг" : "Муниц.долг";
            debtKindStr = FederalDebtSelected ? "государственного долга субъектов" : "муниципального долга";
            selectedFO.Value = RFSelected ? " " : string.Format(".[{0}]", ComboFO.SelectedValue);
            selectedMeasure.Value = PlanSelected ? "План по доходам" : "Факт по доходам";

            string shortSelectedFOName = RFSelected ? "РФ" : RegionsNamingHelper.ShortName(ComboFO.SelectedValue);

            DateTime nextMonthDate = currentDate.AddMonths(1);

            Label1.Text = "Оценка уровня долговой нагрузки";
            Page.Title = Label1.Text;

            string levelStr = FederalDebtSelected ? "Уровень долговой нагрузки на бюджеты субъектов РФ" : "Уровень долговой нагрузки на бюджеты муниципальных образований";
            string dateStr = PlanSelected
                                 ? string.Format("План на {1} год по состоянию на 1 {0} {1} года",
                                    CRHelper.RusMonthGenitive(nextMonthDate.Month), nextMonthDate.Year)
                                 : string.Format("Факт по состоянию на 1 {0} {1} года",
                                    CRHelper.RusMonthGenitive(nextMonthDate.Month), nextMonthDate.Year);

            Label2.Text = string.Format("{0}<br/>{1}", FederalDebtSelected ? "на бюджеты субъектов РФ" : "на бюджеты муниципальных образований", dateStr);
            mapElementCaption.Text = levelStr;
            
            if (DundasMap.Legends.Count != 0)
            {
                DundasMap.Legends[0].Title = string.Format("{0}", levelStr.Replace("нагрузки на", "нагрузки\nна"));
            }

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            // заполняем карту формами
            string regionStr = shortSelectedFOName;

            DundasMap.Shapes.Clear();
            DundasMap.LoadFromShapeFile(Server.MapPath(string.Format("../../maps/{0}/{0}.shp", RegionsNamingHelper.ShortName(regionStr))), "NAME", true);
            // заполняем карту данными
            FillMapData(DundasMap);
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("MFRF_0003_0003_grid");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Территория", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                    {
                        row[2] = Convert.ToDouble(row[2]) / 1000;
                    }
                    if (row[3] != DBNull.Value && row[3].ToString() != string.Empty)
                    {
                        row[3] = Convert.ToDouble(row[3]) / 1000;
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(300);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            SetColumnParams(e.Layout, 0, 1, "", 60, false);
            SetColumnParams(e.Layout, 0, 2, "N2", 160, false);
            SetColumnParams(e.Layout, 0, 3, "N2", 160, false);
            SetColumnParams(e.Layout, 0, 4, "N4", 110, false);
            SetColumnParams(e.Layout, 0, 5, "N0", 110, false);
            SetColumnParams(e.Layout, 0, 6, "N0", 70, true);
            SetColumnParams(e.Layout, 0, 7, "N4", 90, false);
            SetColumnParams(e.Layout, 0, 8, "P2", 120, false);

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "ФО", "Федеральный округ");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, string.Format("Объем {0}, тыс.руб.", debtKindStr), string.Format("Объем {0}", debtKindStr));
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, PlanSelected ? "План по доходам, тыс.руб." : "Факт по доходам, тыс.руб.",
                string.Format("{0} {1}", PlanSelected ? "Плановые годовые назначения" : " Фактическое исполнение", 
                                         FederalDebtSelected ? "бюджета субъекта РФ" : "муниципального образования"));
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "Уровень долговой нагрузки",
                string.Format("Отношение {0} к доходам", debtKindStr));
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "Ранг по долговой нагрузке", 
                string.Format("Ранг (место) по уровеню долговой нагрузки среди всех субъектов {0}", RFSelected ? "РФ" : "ФО"));
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, "Оценка", "Оценка уровня долговой нагрузки");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7, "Оценка", "Оценка уровня долговой нагрузки");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8, "Темп роста долговой нагрузки", "Темп роста долговой нагрузки к аналогичному пероиду прошлого года");
        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width, bool hidden)
        {
            CRHelper.FormatNumberColumn(layout.Bands[bandIndex].Columns[columnIndex], format);
            layout.Bands[bandIndex].Columns[columnIndex].Hidden = hidden;
            layout.Bands[bandIndex].Columns[columnIndex].Width = CRHelper.GetColumnWidth(width);
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (!RFSelected)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
        }
        
        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool rank = (i == 5);
                bool rate = (i == 8);
                
                bool debt = (i == 2);
                int rankFOIndex = 5;

                string zeroValueText = "Нет долга";
                string nullValueText = "Нет информации";

                if (rank)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i + 1].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty &&
                        e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                    {
                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = string.Format("Cамый маленький уровень долговой нагрузки по {0}", !RFSelected ? "ФО" : "РФ");
                        }
                        else if (Convert.ToInt32(e.Row.Cells[i].Value) == Convert.ToInt32(e.Row.Cells[i + 1].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png"; 
                            e.Row.Cells[i].Title = string.Format("Cамый большой уровень долговой нагрузки по {0}", !RFSelected ? "ФО" : "РФ");
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
                
                string subjectName = string.Empty;
                if (e.Row.Cells[0].Value != null)
                {
                    subjectName = e.Row.Cells[0].Value.ToString();
                }

                if (debt && RegionsNamingHelper.IsSubject(subjectName) && !(e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != String.Empty))
                {
                    e.Row.Cells[i].Value = nullValueText;
                }

                if (rate)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                            e.Row.Cells[i].Title = "Прирост к прошлому году";
                        }
                        else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            e.Row.Cells[i].Title = "Снижение к прошлому году";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() != string.Empty)
                {
                    if (!RegionsNamingHelper.IsSubject(e.Row.Cells[0].Value.ToString()))
                    {
                        e.Row.Cells[i].Style.Font.Bold = true;
                    }
                }
            }

            foreach (UltraGridCell cell in e.Row.Cells)
            {
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
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
            subject = subject.Replace("федеральный округ", "ФО");
            bool isRepublic = patternValue.Contains("Республика");
            bool isTown = patternValue.Contains("г.");

            string[] subjects = patternValue.Split(' ');
            // пока такой глупый способ сопоставления имен субъектов
            if (subjects.Length > 1)
            {
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

        public void FillMapData(MapControl map)
        {
            if (map == null)
            {
                return;
            }

            string query = DataProvider.GetQueryText("MFRF_0003_0003_map");

            dtMap = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtMap);

            bool nullValueExists = false;

            foreach (DataRow row in dtMap.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty)
                {
                    string regionName = row[0].ToString();
                    string levelName = row[2].ToString();

                    bool useSubject = (RFSelected && levelName == "Федеральный округ" ||
                                       !RFSelected && levelName == "Субъект РФ");

                    if (useSubject)
                    {
                        Shape shape = FindMapShape(map, regionName);
                        if (shape != null)
                        {
                            shape.TextVisibility = TextVisibility.Shown;
                            if (row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                            {
                                double debtValue = Convert.ToDouble(row[1]);
                                if (debtValue == 0)
                                {
                                    shape.Color = Color.LightSkyBlue;
                                    shape.ToolTip = string.Format("{0}\n{1}", shape.Name,
                                        FederalDebtSelected ? "Государственный долг отсутствует" : "Мунициапальный долг отсутствует");
                                    shape.Text = shape.Name;

                                    if (!nullValueExists && DundasMap.Legends.Count > 0)
                                    {
                                        nullValueExists = true;

                                        LegendItem item = new LegendItem();
                                        item.Text = "Долг отсутствует";
                                        item.Color = Color.LightSkyBlue;
                                        DundasMap.Legends[0].Items.Add(item);
                                    }
                                }
                                else
                                {
                                    shape["Name"] = regionName;
                                    shape["DebtValue"] = debtValue;
                                    shape.ToolTip =
                                        string.Format("#NAME \nУровень долговой нагрузки: {0:N4}", debtValue);
                                    shape.TextAlignment = ContentAlignment.MiddleCenter;
                                    shape.Offset.X = -15;

                                    shape.Text = string.Format("{0}\n{1:N4}", shape.Name, debtValue);

                                    shape.BorderWidth = 2;
                                    shape.TextColor = Color.Black;
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
           // e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text + Label2.Text.Replace("<br/>", "   ");
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;
            int rowsCount = UltraWebGrid.Rows.Count;

            e.CurrentWorksheet.Columns[0].Width = 250*37;
            e.CurrentWorksheet.Columns[1].Width = 50*37;
            e.CurrentWorksheet.Columns[2].Width = 130*37;
            e.CurrentWorksheet.Columns[3].Width = 130*37;
            e.CurrentWorksheet.Columns[4].Width = 110*37;
            e.CurrentWorksheet.Columns[5].Width = 110*37;
            e.CurrentWorksheet.Columns[6].Width = 110*37;
            e.CurrentWorksheet.Columns[7].Width = 110*37;

            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "";
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "#,####0.0000;[Red]-#,####0.0000";
            e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "#0";
            e.CurrentWorksheet.Columns[6].CellFormat.FormatString = "#,####0.0000;[Red]-#,####0.0000";
            e.CurrentWorksheet.Columns[7].CellFormat.FormatString = "0.00%";

            // расставляем стили у ячеек хидера
            for (int i = 0; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[2].Height = 23 * 37;
                e.CurrentWorksheet.Rows[2].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[2].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[2].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Карта");

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            sheet1.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text.Replace("<br/>", " ");

            sheet2.Rows[0].Cells[0].Value = Label1.Text+ " " +Label2.Text.Replace("<br/>", " ");
            sheet2.Rows[1].Cells[0].Value = mapElementCaption.Text;
            UltraGridExporter.MapExcelExport(sheet2.Rows[3].Cells[0], DundasMap);

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 2;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            CRHelper.SetHeaderCaption(UltraWebGrid, 0, 7, "Темп роста долговой нагрузки", "Темп роста долговой нагрузки к аналогичному пероиду прошлого года");

            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            UltraGridExporter1.GridElementCaption = Label1.Text + " " + Label2.Text.Replace("<br/>", " ");
        }

        private void PdfExporter_EndExport(object sender, EndExportEventArgs e)
        {
            e.Section.AddPageBreak();

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label1.Text + " " + Label2.Text.Replace("<br/>", " "));

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);

            title.AddContent(string.Format("{0}", mapElementCaption.Text));

            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap);
            e.Section.AddImage(img);
        }
        
        #endregion
    }
}

using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Infragistics.Documents.Excel;
using ContentAlignment = System.Drawing.ContentAlignment;

namespace Krista.FM.Server.Dashboards.reports.MFRF_0002_0002
{
    public partial class Default : CustomReportPage
    {
        #region Поля и свойства

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid, dtGrid_Indicator;

        private DataTable dtComments;


        private GridHeaderLayout headerLayout;

        private static MemberAttributesDigest periodDigest;

        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 900; }
        }

        private int MinScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }

        private int MinScreenHeight
        {
            get { return IsSmallResolution ? 600 : CustomReportConst.minScreenHeight; }
        }

        /// <summary>
        /// Выбраны ли все федеральные округа
        /// </summary>
        public bool AllFO
        {
            get { return ComboFO.SelectedIndex == 0; }
        }

        #endregion

        #region Параметры запроса

        // выбранный период
        private CustomParam selectedPeriod;
        // выбранный варинат
        private CustomParam bkkuVariant;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(MinScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(MinScreenHeight * 0.5);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            DundasMap.Width = CRHelper.GetChartWidth(MinScreenWidth - 25);
            DundasMap.Height = CRHelper.GetChartHeight(MinScreenHeight * 0.7);

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink1.Text = "Индикаторы&nbsp;БК&nbsp;И&nbsp;КУ";
            CrossLink1.NavigateUrl = "~/reports/MFRF_0002_0003/Default.aspx";
            CrossLink2.Text = "Динамика&nbsp;индикаторов&nbsp;БК&nbsp;И&nbsp;КУ";
            CrossLink2.NavigateUrl = "~/reports/MFRF_0002_0004/Default.aspx";

            selectedPeriod = UserParams.CustomParam("selected_period");
            bkkuVariant = UserParams.CustomParam("bkku_Variant");
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            periodDigest = new MemberAttributesDigest(DataProvidersFactory.SecondaryMASDataProvider, "MFRF_0002_0002_periodDigest");

            if (!Page.IsPostBack)
            {
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

            Page.Title = string.Format("Мониторинг соблюдения требований БК и КУ ({0})", ComboFO.SelectedValue.Replace("Все федеральные округа", "Российская Федерация").Replace("федеральный округ", "ФО"));
            Label1.Text = Page.Title;

            selectedPeriod.Value = periodDigest.GetMemberUniqueName(ComboYear.SelectedValue);

            bkkuVariant.Value = ComboYear.SelectedValue.Contains("квартал") ? "По данным месячной отчетности" : "По данным годовой отчетности";

            Label2.Text = string.Format("<br/>Данные Минфина РФ за {0} {1}", ComboYear.SelectedValue, bkkuVariant.Value.ToLower());
            mapElementCaption.Text = string.Format("{0}, количество нарушений требований БК РФ и условий качества управления бюджетами субъектов РФ", ComboFO.SelectedValue.Replace("Все федеральные округа", "Российская Федерация").Replace("федеральный округ", "ФО"));
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
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.DataBind();

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
            string regionStr = (ComboFO.SelectedIndex == 0) ? "Российская Федерация" : ComboFO.SelectedValue;
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

            string query = DataProvider.GetQueryText("MFRF_0002_0002_map");
            DataTable dtMap = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtMap);

            query = DataProvider.GetQueryText("MFRF_0002_0002_map_hint");
            DataTable dtMapHint = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtMapHint);

            foreach (Shape shape in DundasMap.Shapes)
            {
                shape.Text = String.Empty;
                shape.ToolTip = shape.Name;
            }

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
                            shape.Text = String.Format("{0} \nБК: {1:N0}", shape.Text, (row[1]));
                            shape.ToolTip = String.Format("{0} \nнарушений БК: {1:N0}", shape.ToolTip, (row[1]));
                            if (Convert.ToDouble(row[1]) != 0)
                            {
                                Symbol symbol1 = new Symbol();
                                symbol1.Name = shape.Name + DundasMap.Symbols.Count;
                                symbol1.ParentShape = shape.Name;
                                symbol1["CrimesBK"] = Convert.ToDouble(row[1]);
                                symbol1.Offset.Y = -30;
                                symbol1.Offset.X = -5;
                                symbol1.Color = Color.DarkViolet;
                                symbol1.MarkerStyle = MarkerStyle.Circle;
                                DundasMap.Symbols.Add(symbol1);
                            }
                        }

                        if (row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                        {
                            shape.Text = String.Format("{0} КУ: {1:N0}", shape.Text, (row[2]));
                            shape.ToolTip = String.Format("{0} \nнарушений КУ: {1:N0}", shape.ToolTip, (row[2]));
                            if (Convert.ToDouble(row[2]) != 0)
                            {
                                Symbol symbol2 = new Symbol();
                                symbol2.Name = shape.Name + DundasMap.Symbols.Count;
                                symbol2.ParentShape = shape.Name;
                                symbol2["CrimesKU"] = Convert.ToDouble(row[2]);
                                symbol2.Offset.Y = -30;
                                symbol2.Offset.X = 15;
                                symbol2.Color = Color.Yellow;
                                symbol2.MarkerStyle = MarkerStyle.Circle;
                                DundasMap.Symbols.Add(symbol2);
                            }
                        }

                        foreach (DataRow hintRow in dtMapHint.Rows)
                        {
                            if (hintRow[0].ToString() == subject)
                            {
                                shape.ToolTip =
                                    String.Format("{0} \n({1})", shape.ToolTip, hintRow[1]);
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
            DundasMap.ZoomPanel.Visible = !IsSmallResolution;
            DundasMap.NavigationPanel.Visible = !IsSmallResolution;
            DundasMap.ZoomPanel.Dock = PanelDockStyle.Right;
            DundasMap.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap.Viewport.EnablePanning = true;

            AddMapFields();
            AddMapSymbols();
            AddMapLegends();
            AddMapColoringRules();
        }

        private void AddMapColoringRules()
        {
            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = 1;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.LightBlue;
            rule.MiddleColor = Color.LightBlue;
            rule.ToColor = Color.LightBlue;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            DundasMap.ShapeRules.Add(rule);
        }

        private void AddMapSymbols()
        {
            // добавляем правила расстановки символов
            DundasMap.SymbolRules.Clear();

            SymbolRule symbolRule1 = new SymbolRule();
            symbolRule1.Name = "SymbolRuleBK";
            symbolRule1.Category = string.Empty;
            symbolRule1.DataGrouping = DataGrouping.EqualInterval;
            symbolRule1.FromValue = "1";
            symbolRule1.SymbolField = "CrimesBK";
            symbolRule1.ShowInLegend = "CrimesBKLegend";
            DundasMap.SymbolRules.Add(symbolRule1);

            // звезды для легенды
            for (int i = 1; i < 4; i++)
            {
                PredefinedSymbol predefined = new PredefinedSymbol();
                predefined.Name = "PredefinedSymbolBK" + i;
                predefined.MarkerStyle = MarkerStyle.Circle;
                predefined.Width = 5 + (i * 10);
                predefined.Height = predefined.Width;
                predefined.Color = Color.DarkViolet;
                DundasMap.SymbolRules["SymbolRuleBK"].PredefinedSymbols.Add(predefined);
            }

            SymbolRule symbolRule2 = new SymbolRule();
            symbolRule2.Name = "SymbolRuleKU";
            symbolRule2.Category = string.Empty;
            symbolRule2.DataGrouping = DataGrouping.EqualInterval;
            symbolRule2.FromValue = "1";
            symbolRule2.SymbolField = "CrimesKU";
            symbolRule2.ShowInLegend = "CrimesKULegend";
            DundasMap.SymbolRules.Add(symbolRule2);

            // звезды для легенды
            for (int i = 1; i < 4; i++)
            {
                PredefinedSymbol predefined = new PredefinedSymbol();
                predefined.Name = "PredefinedSymbolKU" + i;
                predefined.MarkerStyle = MarkerStyle.Circle;
                predefined.Width = 5 + (i * 10);
                predefined.Height = predefined.Width;
                predefined.Color = Color.Yellow;
                DundasMap.SymbolRules["SymbolRuleKU"].PredefinedSymbols.Add(predefined);
            }
        }

        private void AddMapFields()
        {
            // добавляем поля
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.SymbolFields.Add("CrimesBK");
            DundasMap.SymbolFields["CrimesBK"].Type = typeof(double);
            DundasMap.SymbolFields["CrimesBK"].UniqueIdentifier = false;
            DundasMap.SymbolFields.Add("CrimesKU");
            DundasMap.SymbolFields["CrimesKU"].Type = typeof(double);
            DundasMap.SymbolFields["CrimesKU"].UniqueIdentifier = false;
        }

        private void AddMapLegends()
        {
            DundasMap.Legends.Clear();

            // добавляем легенду
            Legend legend1 = new Legend("CrimesBKLegend");
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
            legend1.Font = new System.Drawing.Font("MS Sans Serif", 7, FontStyle.Regular);
            legend1.AutoFitText = true;
            legend1.Title = "Количество нарушений БК";
            legend1.AutoFitMinFontSize = 7;
            legend1.Dock = PanelDockStyle.Left;
            DundasMap.Legends.Add(legend1);

            Legend legend2 = new Legend("CrimesKULegend");
            legend2.Visible = true;
            legend2.BackColor = Color.White;
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
            legend2.Title = "Количество нарушений КУ";
            legend2.AutoFitMinFontSize = 7;
            legend2.Dock = PanelDockStyle.Left;
            DundasMap.Legends.Add(legend2);
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
            string query = DataProvider.GetQueryText("MFRF_0002_0002_compare_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);

            string query2 = DataProvider.GetQueryText("MFRF_0002_0002_compare_Grid_Indicator");
            dtGrid_Indicator = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query2, "Субъект", dtGrid_Indicator);

            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = dtGrid;
            }

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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(180);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            /*for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 1)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 4;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 3;
                }
            }
            */
            e.Layout.Bands[0].Columns[1].Width = 40;
            headerLayout.AddCell("Субъект");
            headerLayout.AddCell("ФО");
            for (int i = 2; i < 5; i++)
            {
                string caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];
                CRHelper.SaveToErrorLog(caption);
                headerLayout.AddCell(caption).AddCell("Значение", 2);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                e.Layout.Bands[0].Columns[i].Width = 100;
               
            }
            for (int i = 5; i < e.Layout.Bands[0].Columns.Count; i=i+2)
            {
                string Caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];
                GridHeaderCell cell = headerLayout.AddCell(string.Format("{0}", Caption));
                GridHeaderCell cell2 =
                    cell.AddCell(
                        string.Format(
                "{0}",
                // "<span style = 'font-size:xx-small; font-family: Verdana; font-weight: normal;text-align: left'>{0}</span>",
                            GetIndicatorComments(Caption)));

                cell2.AddCell("Значение", "");
                cell2.AddCell("Нормативное значение", "");
                e.Layout.Bands[0].Columns[i].Width = 100;
                e.Layout.Bands[0].Columns[i + 1].Width = 100;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N4");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i+1], "N0");
            }
            headerLayout.ApplyHeaderInfo();
           
        }

       

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int j = 5;
            for (int i = 5; i < e.Row.Cells.Count - 1; i = i + 2)
            {

                if ((dtGrid_Indicator.Rows[e.Row.Index][j + 2] != null) && (dtGrid_Indicator.Rows[e.Row.Index][j + 2].ToString() != string.Empty))
                {
                    if (Convert.ToInt32(dtGrid_Indicator.Rows[e.Row.Index][j + 2].ToString()) == 1)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerRed.gif";
                        e.Row.Cells[i].Title = string.Format("Не соблюдается условие");
                    }
                    else
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerGreen.gif";
                        e.Row.Cells[i].Title = string.Format("Соблюдается условие");
                    }
                    e.Row.Cells[i].Style.CustomRules =
                    "background-repeat: no-repeat; background-position: right top; margin: 0px";
                }
                j = j + 3;
            }
        }

        #endregion

        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Карта");
            sheet1.Rows[0].Cells[0].Value = Label1.Text;
            sheet1.Rows[1].Cells[0].Value = Label2.Text.Replace("<br/>", "");
            sheet2.Rows[0].Cells[0].Value = mapElementCaption.Text;
            sheet2.Rows[1].Cells[0].Value = Label2.Text.Replace("<br/>", "");
            ReportExcelExporter1.HeaderCellHeight = 70;
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 4);
            ReportExcelExporter1.Export(DundasMap, sheet2, 4);
        }
        #endregion

        #region Экспорт в PDF
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
			ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.HeaderCellHeight = 90;
            Report report = new Report();
            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(headerLayout, Label2.Text.Replace("<br/>", ""), section1);
            ISection section2 = report.AddSection();
            IText title = section2.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = font;
            title.AddContent(mapElementCaption.Text);
            ReportPDFExporter1.Export(DundasMap, Label2.Text.Replace("<br/>", ""), section2);
        }
        #endregion
   
        private string GetIndicatorComments(string name)
        {
            if (dtComments == null || dtComments.Columns.Count == 0)
            {
                dtComments = new DataTable();
                string query = DataProvider.GetQueryText("MFRF_0002_0002_compare_comment");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtComments);
            }

            for (int i = 1; i < dtComments.Columns.Count; i++)
            {
                if (dtComments.Columns[i].Caption == name)
                {
                    return String.Format("Код:{0}<br/>Условие:{1}<br/>Единицы измерения:{2}",
                            dtComments.Rows[0][i],
                            dtComments.Rows[1][i],
                            dtComments.Rows[2][i]);
                }
            }
            return String.Empty;
        }
    }
}
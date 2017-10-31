using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs=Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Font=Infragistics.Documents.Reports.Graphics.Font;
using Image=Infragistics.Documents.Reports.Graphics.Image;
using SerializationFormat=Dundas.Maps.WebControl.SerializationFormat;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0002_Vologda
{
    public partial class DefaultCompare : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2000;
        private int endYear = 2011;
        private string populationDate;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.7 -  225);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.7);

            CrossLink1.Text = "Диаграмма распределения субъектов по среднедушевым доходам";
            CrossLink1.NavigateUrl = "~/reports/FK_0001_0002_Vologda/DefaultAllocation.aspx";
            CrossLink2.Text = "Доходы&nbsp;субъекта&nbsp;РФ&nbsp;подробнее";
            CrossLink2.NavigateUrl = "~/reports/FK_0001_0004/DefaultDetail.aspx";

            #region Настройка карты

            #endregion

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <EndExportEventArgs>(PdfExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            if (!Page.IsPostBack)
            {
                mapWebAsyncPanel.AddRefreshTarget(DundasMap);
                mapWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FK_0001_0002_Vologda_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = dtDate.Rows[0][3].ToString();
                UserParams.Filter.Value = "Все федеральные округа";
                UserParams.KDGroup.Value = "Доходы ВСЕГО ";

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(UserParams.PeriodMonth.Value, true);

                ComboFO.Title = "Субъект РФ";
                ComboFO.Width = 300;
                ComboFO.MultiSelect = true;
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillRegions(RegionsNamingHelper.FoNames, RegionsNamingHelper.RegionsFoDictionary));

                string foName = RegionsNamingHelper.GetFoBySubject(RegionSettings.Instance.Name);
                UserParams.StateArea.Value = RegionSettings.Instance.Name;
                if (foName != String.Empty)
                {
                    ComboFO.SetСheckedState(foName, true);
                }
                else
                {
                    ComboFO.SetAllСheckedState(true, false);
                }
                
                ComboKD.Width = 420;
                ComboKD.Title = "Вид дохода";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillFullKDIncludingList());
                ComboKD.RenameTreeNodeByName("Доходы бюджета - Итого ", "Доходы ВСЕГО ");
                ComboKD.SetСheckedState(UserParams.KDGroup.Value, true);
            }

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            Page.Title = string.Format("Исполнение доходов ({0})", ComboFO.SelectedIndex == 0
                                                                       ? "РФ"
                                                                       :
                                                                   RegionsNamingHelper.ShortName(ComboFO.SelectedValue));
            Label1.Text = Page.Title;
            Label2.Text =
                string.Format(
                    "Исполнение консолидированных бюджетов субъектов РФ по доходам ({3}) за {0} {1} {2} года", monthNum,
                    CRHelper.RusManyMonthGenitive(monthNum), yearNum, ComboKD.SelectedValue);
           
            int year = Convert.ToInt32(ComboYear.SelectedValue);

            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodHalfYear.Value =
                string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value =
                string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.KDGroup.Value = ComboKD.SelectedValue;

            populationDate = DataDictionariesHelper.GetFederalPopulationDate();
            
            string subjectList = String.Empty;
            foreach (string subject in ComboFO.SelectedValues)
            {
                subjectList += String.Format("[Территории].[Сопоставимый].[{0}],", subject);
            }
            UserParams.Subject.Value = subjectList.TrimEnd(',');
            
            UltraWebGrid.DataBind();

            string patternValue = UserParams.StateArea.Value;
            int defaultRowIndex = 0;
            if (patternValue == string.Empty)
            {
                patternValue = UserParams.StateArea.Value;
                defaultRowIndex = 0;
            }

            UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
            ActiveGridRow(row);
        }

        #region Обработчики карты

        private void SetupMap()
        {
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;

            // добавляем легенду
            Legend legend = new Legend("CompleteLegend");
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
            legend.Title = "Процент исполнения";
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Clear();
            DundasMap.Legends.Add(legend);

            // добавляем поля
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Complete");
            DundasMap.ShapeFields["Complete"].Type = typeof(double);
            DundasMap.ShapeFields["Complete"].UniqueIdentifier = false;
            DundasMap.ShapeFields.Add("CompletePercent");
            DundasMap.ShapeFields["CompletePercent"].Type = typeof(double);
            DundasMap.ShapeFields["CompletePercent"].UniqueIdentifier = false;

            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "CompletePercent";
            rule.DataGrouping = DataGrouping.EqualDistribution;
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
            rule.LegendText = "#FROMVALUE{N2} - #TOVALUE{N2}";
            DundasMap.ShapeRules.Add(rule);
        }

        /// <summary>
        /// Поиск формы карты
        /// </summary>
        /// <param name="map">карта</param>
        /// <param name="patternValue">искомое имя формы</param>
        /// <param name="searchFO">true, если ищем ФО</param>
        /// <returns>найденная форма</returns>
        public static Shape FindMapShape(MapControl map, string patternValue)
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
                            subject = (isRepublic || isTown ) ? subjects[1] : subjects[0];
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

            foreach (Shape shape in DundasMap.Shapes)
            {
                shape.Text = String.Empty;
            }

            foreach (DataRow row in dtGrid.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty)
                {
                    string subject = row[0].ToString();
                    if (RegionsNamingHelper.IsFO(subject) || RegionsNamingHelper.IsSubject(subject))
                    {
                        Shape shape = FindMapShape(DundasMap, subject);
                        if (shape != null)
                        {
                            if (row[4] != DBNull.Value && row[4].ToString() != string.Empty)
                            {
                                double value = 100*Convert.ToDouble(row[4]);

                                shape["Name"] = subject;
                                shape["CompletePercent"] = value;
                                shape.ToolTip = "#NAME #COMPLETEPERCENT{N2}%";
                                shape.TextVisibility = TextVisibility.Shown;

                                shape.Text = String.Format("{0}\n{1:N2}%", subject, value);

                                if (subject == UserParams.StateArea.Value)
                                {
                                    shape.BorderColor = Color.Black;
                                    shape.BorderWidth = 3;
                                }
                            }
                            else
                            {
                                shape.Text = subject;
                                shape.ToolTip = subject;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Обработчики грида


        void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }

        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {
            if (row == null || row.Cells.Count == 0)
                return;

            string subject = row.Cells[0].Text;
            UserParams.StateArea.Value = subject;

            string regionStr = String.Empty;
            if (RegionsNamingHelper.IsRF(subject))
            {
                regionStr = "Российская Федерация";
            }
            else if (RegionsNamingHelper.IsFO(subject))
            {
                regionStr = subject;
            }
            else
            {
                regionStr = RegionsNamingHelper.GetFoBySubject(subject);
            }

            string mapFileName = String.Format("../../maps/{0}/{0}.shp", RegionsNamingHelper.ShortName(regionStr));

            try
            {
                // заполняем карту формами
                DundasMap.Shapes.Clear();

                DundasMap.LoadFromShapeFile(Server.MapPath(mapFileName), "NAME", true);

//                DundasMap.Serializer.Format = SerializationFormat.Binary;
//                DundasMap.Serializer.Load(Server.MapPath(mapFileName));

                SetupMap();
                // заполняем карту данными
                FillMapData();
            }
            catch(Exception e)
            {

            }
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0002_Vologda_compare_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);

            foreach (DataRow row in dtGrid.Rows)
            {
                if (row[2] != DBNull.Value)
                {
                    row[2] = Convert.ToDouble(row[2]) / 1000000;
                }
                if (row[3] != DBNull.Value)
                {
                    row[3] = Convert.ToDouble(row[3]) / 1000000;
                }
            }

            if (dtGrid.Columns.Count > 2)
            {
                dtGrid.Columns[1].ColumnName = "ФО";
            }
            
            UserParams.Filter.Value = ComboFO.SelectedValue;
            UltraWebGrid.DataSource = dtGrid;
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (dtGrid.Rows.Count < 15)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width, bool hidden)
        {
            CRHelper.FormatNumberColumn(layout.Bands[bandIndex].Columns[columnIndex], format);
            layout.Bands[bandIndex].Columns[columnIndex].Hidden = hidden;
            layout.Bands[bandIndex].Columns[columnIndex].Width = CRHelper.GetColumnWidth(width);
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(235);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            SetColumnParams(e.Layout, 0, 1, "", 43, false);
            SetColumnParams(e.Layout, 0, 2, "N3", 85, false);
            SetColumnParams(e.Layout, 0, 3, "N3", 85, false);
            SetColumnParams(e.Layout, 0, 4, "P2", 70, false);
            SetColumnParams(e.Layout, 0, 5, "N0", 62, false);
            SetColumnParams(e.Layout, 0, 6, "N0", 62, true);
            SetColumnParams(e.Layout, 0, 7, "N0", 60, false);
            SetColumnParams(e.Layout, 0, 8, "N0", 70, true);
            SetColumnParams(e.Layout, 0, 9, "N3", 85, false);
            SetColumnParams(e.Layout, 0, 10,"N3", 95, false);
            SetColumnParams(e.Layout, 0, 11,"N0", 70, false);
            SetColumnParams(e.Layout, 0, 12,"N0", 70, true);
            SetColumnParams(e.Layout, 0, 13,"N0", 70, false);
            SetColumnParams(e.Layout, 0, 14,"N0", 70, true);
            SetColumnParams(e.Layout, 0, 15,"P2", 66, false);
            SetColumnParams(e.Layout, 0, 16,"P2", 66, false);

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "ФО", "Федеральный округ, которому принадлежит субъект");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Исполнено, млн.руб", "Фактическое исполнение нарастающим итогом с начала года");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "Назначено, млн.руб", "План на год");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "Исполнено %", "Процент выполнения назначений. Оценка равномерности исполнения (1/12 годового плана в месяц)");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "Ранг % ФО", "Ранг (место) субъекта по проценту исполнения назначений среди субъектов его федерального округа");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7, "Ранг % РФ", "Ранг (место) по проценту исполнения назначений среди всех субъектов");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 9, string.Format("Численность постоянного населения {0}, тыс.чел.", populationDate), "Численность постоянного населения");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 10, "Среднедушевые доходы, руб./чел.", "Сумма доходов выбранного вида на душу населения");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 11, "Ранг среднедуш. ФО", "Ранг (место) субъекта по среднедушевым доходам среди субъектов его федерального округа");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 13, "Ранг среднедуш. РФ", "Ранг (место) по среднедушевым доходам среди всех субъектов");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 15, "Доля ФО", "Доля доходов субъекта в общей сумме доходов федерального округа");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 16, "Доля РФ", "Доля доходов субъекта (округа) в общей сумме доходов всех субъектов РФ");
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Cells[5].Value = string.Empty;
                e.Row.Cells[6].Value = string.Empty;
                e.Row.Cells[11].Value = string.Empty;
                e.Row.Cells[12].Value = string.Empty;
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.Padding.Top = 5;
                e.Row.Cells[i].Style.Padding.Bottom = 5;

                bool rank = (i == 5 || i == 7 || i == 11 || i == 13);
                bool complete = (i == 4);

                if (rank)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i + 1].Value != null &&
                        e.Row.Cells[i].Value.ToString() != string.Empty &&
                        e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                    {
                        string region = (i == 7 || i == 13) ? "РФ" : "федеральном округе";
                        string obj = (i == 11 || i == 13) ? "доход на душу населения" : "процент исполнения";
                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = string.Format("Самый высокий {1} в {0}", region, obj);
                        }
                        else if (Convert.ToInt32(e.Row.Cells[i].Value) == Convert.ToInt32(e.Row.Cells[i + 1].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = string.Format("Самый низкий {1} в {0}", region, obj);
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (complete)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        double percent = (ComboMonth.SelectedIndex + 1) * 100.0 / 12;
                        
                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < percent)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                            e.Row.Cells[i].Title = string.Format("Не соблюдается условие равномерности ({0:N2}%)", percent);
                        }
                        else
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                            e.Row.Cells[i].Title = string.Format("Соблюдается условие равномерности ({0:N2}%)", percent);
                        }
                        e.Row.Cells[i].Style.Padding.Right = 2;
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
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

        #region Экспорт

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text;
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            e.CurrentWorksheet.Columns[0].Width = 250 * 37;
            e.CurrentWorksheet.Columns[1].Width = 50 * 37;
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

            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = UltraGridExporter.ExelNumericFormat;
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = UltraGridExporter.ExelNumericFormat;
            e.CurrentWorksheet.Columns[4].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "##";
            e.CurrentWorksheet.Columns[6].CellFormat.FormatString = "##";
            e.CurrentWorksheet.Columns[7].CellFormat.FormatString = UltraGridExporter.ExelNumericFormat;
            e.CurrentWorksheet.Columns[8].CellFormat.FormatString = UltraGridExporter.ExelNumericFormat;
            e.CurrentWorksheet.Columns[9].CellFormat.FormatString = "##";
            e.CurrentWorksheet.Columns[10].CellFormat.FormatString = "##";
            e.CurrentWorksheet.Columns[11].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[12].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
            title.Style.Font = new Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Font(font);
            title.AddContent(Label2.Text);
        }

        private void PdfExporter_EndExport(object sender, EndExportEventArgs e)
        {
            Image img = UltraGridExporter.GetImageFromMap(DundasMap);
            e.Section.AddImage(img);
        }

        #endregion
    }
}

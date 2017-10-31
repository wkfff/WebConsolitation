using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.SB_003
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable gridDt;
        private DataTable gridHintDt;
        private DataTable chartDt;

        private DataTable bankTerritory;
        private DataTable sberbankFacility;
        private DataTable facilityEvaluation;

        private List<string> bankList;
        private List<string> facilityList;

        private Dictionary<string, int> bankDicitionary;
        private Dictionary<string, int> facilityDicitionary;

        private Dictionary<string, string> regionBankDicitionary;
        private Dictionary<string, string> facilityEvaluaitonDictionary;
        private Dictionary<string, string> facilityDescriptionDictionary;

        private string federalSubjectColumn = "federal-subject";
        private string sberbankFacilityColumn = "sberbank-facility";
        private string evaluationColumn = "evaluation";
        private string descriptionColumn = "description";
        private string bankColumn = "bank";
        private string levelColumn = "level";

        #endregion

        public bool IsAllBankSelected
        {
            get { return ComboBank.SelectedNode.Level == 0; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.7 - 235);
            GridBrick.Width = CustomReportConst.minScreenWidth - 15;
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.AutoHeightRowLimit = 15;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);

            #endregion

            #region Настройка диаграммы

            PieChartBrick.Width = 300;
            PieChartBrick.Height = 300;

            PieChartBrick.DataFormatString = "P2";
            PieChartBrick.TooltipFormatString = "<ITEM_LABEL> <DATA_VALUE:N0> субъектов (<PERCENT_VALUE:N2>%)";
            PieChartBrick.Legend.Visible = false;
            PieChartBrick.ColorModel = ChartColorModel.ExtendedFixedColors;
            PieChartBrick.SwapRowAndColumns = true;
            PieChartBrick.Chart.PieChart.Labels.Font = new Font("Verdana", 8);

            PieChartBrick.Legend.Visible = true;
            PieChartBrick.Legend.SpanPercentage = 23;
            PieChartBrick.Legend.Location = LegendLocation.Bottom;

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            LoadXmlData();

            if (!Page.IsPostBack)
            {
                FillComboBank();
                ComboBank.Title = "Тербанки";
                ComboBank.Width = 260;
                ComboBank.MultiSelect = false;
                ComboBank.ParentSelect = true;
                ComboBank.FillDictionaryValues(bankDicitionary);
                ComboBank.SetСheckedState("Все тербанки", true);
                //ComboBank.SetСheckedState("Байкальский банк", true);

                FillComboFacility();
                ComboFacility.Title = "Механизмы";
                ComboFacility.Width = 260;
                ComboFacility.MultiSelect = false;
                ComboFacility.ParentSelect = true;
                ComboFacility.FillDictionaryValues(facilityDicitionary);
                ComboFacility.SetСheckedState("Все механизмы", true);
            }

            PageTitle.Text = "Используемые механизмы и инструменты взаимодействия";
            PageSubTitle.Text = "Оценка реализуемых механизмов и инструментов взаимодействия субъекта РФ и территориального банка в регионе";
            Page.Title = PageTitle.Text;

            BroadCast.Text = String.Format("Главная » Территориальный банк » <a href='../../reports/sb_004/Default.aspx'>Планирование и текущая деятельность</a> » Используемые механизмы и инструменты взаимодействия");
            BroadCast.Font.Italic = true;

            gridElementCaption.Text = "Используемые механизмы и инструменты взаимодействия";
            FacilityCaption.Text = String.Format("{0} ({1})", ComboFacility.SelectedValue, ComboBank.SelectedValue);

            GridDataBind();
         
            if (IsAllBankSelected)
            {
                ChartDataBind();
                GaugeRibbonDataBind();
                GaugeRibbonTable.Visible = true;
                HeraldTable.Visible = false;
                ChartTable.Visible = true;
            }
            else
            {
                GenerateHeraldIcons();
                GaugeDataBind();
                HeraldTable.Visible = true;
                GaugeRibbonTable.Visible = false;
                ChartTable.Visible = false;
            }
            
        }

        #region Обработчики грида
        
        private void GridDataBind()
        {
            // создаем грид для значений
            gridDt = new DataTable();
            DataColumn column = new DataColumn(federalSubjectColumn, typeof(String));
            gridDt.Columns.Add(column);

            foreach (string facility in facilityList)
            {
                column = new DataColumn(facility, typeof(double));
                gridDt.Columns.Add(column);
            }

            column = new DataColumn(levelColumn, typeof(String));
            gridDt.Columns.Add(column);

            // создаем грид для хинтов
            gridHintDt = new DataTable();
            column = new DataColumn(federalSubjectColumn, typeof(String));
            gridHintDt.Columns.Add(column);

            foreach (string facility in facilityList)
            {
                column = new DataColumn(facility, typeof(String));
                gridHintDt.Columns.Add(column);
            }

            // забиваем гриды строками
            if (IsAllBankSelected)
            {
                foreach (string bank in bankList)
                {
                    string sqlFilter = String.Format("[{0}] = '{1}'", bankColumn, bank);
                    DataRow[] terrRows = bankTerritory.Select(sqlFilter);

                    // добавляем строку банка в грид
                    DataRow bankRow = gridDt.NewRow();
                    bankRow[federalSubjectColumn] = bank;

                    // считаем среднее по регионам банка
                    AvgCalculator avgCalculator = new AvgCalculator();
                    foreach (string facility in facilityList)
                    {
                        avgCalculator.Reset();
                        foreach (DataRow terrRow in terrRows)
                        {
                            string subject = terrRow[federalSubjectColumn].ToString();
                            string key = String.Format("{0};{1}", subject, facility);
                            if (facilityEvaluaitonDictionary.ContainsKey(key))
                            {
                                int value;
                                if (Int32.TryParse(facilityEvaluaitonDictionary[key], out value))
                                {
                                    avgCalculator.AddValue(value);
                                }
                            }
                        }
                        bankRow[facility] = Math.Round(avgCalculator.GetAverage());
                    }
                   
                    bankRow[levelColumn] = "1";
                    gridDt.Rows.Add(bankRow);
                    gridDt.AcceptChanges();

                    FillFacilityGridRows(ref gridDt, terrRows, facilityEvaluaitonDictionary);

                    // добавляем строку банка в грид хинтов
                    bankRow = gridHintDt.NewRow();
                    bankRow[federalSubjectColumn] = bank;
                    gridHintDt.Rows.Add(bankRow);
                    gridHintDt.AcceptChanges();

                    FillFacilityGridRows(ref gridHintDt, terrRows, facilityDescriptionDictionary);
                }
            }
            else
            {
                string sqlFilter = String.Format("[{0}] = '{1}'", bankColumn, ComboBank.SelectedValue);
                DataRow[] facilityRows = bankTerritory.Select(sqlFilter);

                FillFacilityGridRows(ref gridDt, facilityRows, facilityEvaluaitonDictionary);
                FillFacilityGridRows(ref gridHintDt, facilityRows, facilityDescriptionDictionary);
            }

            FontRowLevelRule levelRule = new FontRowLevelRule(gridDt.Columns.Count - 1);
            levelRule.AddFontLevel("1", GridBrick.BoldFont8pt);
            GridBrick.AddIndicatorRule(levelRule);

            GridBrick.Grid.DataSource = gridDt;
        }

        /// <summary>
        /// Перекачка строк в грид
        /// </summary>
        /// <param name="dt">грид</param>
        /// <param name="rowCollection">набор строк</param>
        /// <param name="itemDictionary">словарь элементов</param>
        private void FillFacilityGridRows(ref DataTable dt, DataRow[] rowCollection, Dictionary<string, string> itemDictionary)
        {
            foreach (DataRow row in rowCollection)
            {
                string subject = row[federalSubjectColumn].ToString();

                DataRow newRow = dt.NewRow();
                newRow[federalSubjectColumn] = subject;

                foreach (string facility in facilityList)
                {
                    if (dt.Columns.Contains(facility))
                    {
                        string key = String.Format("{0};{1}", subject, facility);
                        if (itemDictionary.ContainsKey(key))
                        {
                            newRow[facility] = itemDictionary[key];
                        }
                    }
                }

                dt.Rows.Add(newRow);
            }
            dt.AcceptChanges();
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.RowStyleDefault.Wrap = true;
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(160);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                string columnName = e.Layout.Bands[0].Columns[i].Header.Caption;
                
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(GetColumnWidth(columnName));
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], GetColumnFormat(columnName));
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = GetColumnAlignment(columnName);
            }

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;
            headerLayout.AddCell("Субъект РФ / Тербанк");

            foreach (string facility in facilityList)
            {
                headerLayout.AddCell(facility);
            }

            headerLayout.ApplyHeaderInfo();
        }

        private void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 1; i < e.Row.Cells.Count - 1; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];

                if (cell.Value == null)
                {
                    cell.Value = "0";
                }

                if (gridHintDt != null)
                {
                    if (e.Row.Index < gridHintDt.Rows.Count && i < gridHintDt.Columns.Count &&
                        gridHintDt.Rows[e.Row.Index][i] != DBNull.Value)
                    {
                        cell.Title = gridHintDt.Rows[e.Row.Index][i].ToString();
                    }
                }
            }
        }

        private int GetColumnWidth(string columnName)
        {
            return 105;
        }

        private string GetColumnFormat(string columnName)
        {
            return "N0";
        }

        private HorizontalAlign GetColumnAlignment(string columnName)
        {
            return HorizontalAlign.Right;
        }

        #endregion

        #region Лента гербов

        private void GenerateHeraldIcons()
        {
            string sqlFilter = String.Format("[{0}] = '{1}'", bankColumn, ComboBank.SelectedValue);
            DataRow[] terrRows = bankTerritory.Select(sqlFilter);

            foreach (DataRow row in terrRows)
            {
                string subject = row[federalSubjectColumn].ToString().TrimEnd(' ');
                string key = String.Format("{0};{1}", subject, ComboFacility.SelectedValue);

                string description = String.Empty;
                if (facilityDescriptionDictionary.ContainsKey(key))
                {
                    description = facilityDescriptionDictionary[key];
                }

                bool usedFacility = (facilityEvaluaitonDictionary.ContainsKey(key));

                HeraldTd.Controls.Add(NewHeraldIconCell(subject, description, !usedFacility));
            }
        }

        private Control NewHeraldIconCell(string subjectName, string description, bool transparent)
        {
            Control heraldIcon = Page.LoadControl("../../Components/HeraldIcon.ascx");

            ((HeraldIcon)heraldIcon).Image = GetRegionImg(subjectName, "~");
            ((HeraldIcon)heraldIcon).Title = subjectName.Replace(" ", "<br/>");
            ((HeraldIcon)heraldIcon).Description = description;
            ((HeraldIcon)heraldIcon).Width = "120px";

            ((HeraldIcon)heraldIcon).SetOpacity(transparent ? 0.4 : 1);

            HtmlTableCell cell = new HtmlTableCell();
            cell.Controls.Add(heraldIcon);

            return cell;
        }

        private static string GetRegionImg(string subjectName, string prefixPath)
        {
            subjectName = subjectName.Replace("Республика Татарстан", "Республика Татарстан (Татарстан)");
            subjectName = subjectName.Replace("Ханты-Мансийский автономный округ - Югра", "Ханты-Мансийский автономный округ");
            subjectName = subjectName.Replace("Удмуртская Республика", "Удмуртская республика");
            subjectName = subjectName.Replace("Астраханская область", "Астраханская область");
            subjectName = subjectName.Replace("Республика Северная Осетия -Алания", "Республика Северная Осетия-Алания");
            subjectName = subjectName.Replace("Чеченская Республика", "Чеченская республика");
            subjectName = subjectName.Replace("Республика Адыгея", "Республика Адыгея (Адыгея)");
            subjectName = subjectName.Replace("Чувашская Республика", "Чувашская Республика-Чувашия");

            string subjectId = CustomParams.GetSubjectIdByName(subjectName);
            return String.Format("{1}/images/Heralds/{0}.png", subjectId, prefixPath);
        }

        #endregion

        #region Обработчики диаграммы

        private void ChartDataBind()
        {
            DataRow[] terrRows = bankTerritory.Select();

            int usedRegionCount = 0;
            foreach (DataRow row in terrRows)
            {
                string subject = row[federalSubjectColumn].ToString().TrimEnd(' ');
                string key = String.Format("{0};{1}", subject, ComboFacility.SelectedValue);

                if (facilityEvaluaitonDictionary.ContainsKey(key))
                {
                    usedRegionCount++;
                }
            }

            chartDt = new DataTable();
            DataColumn column = new DataColumn("Используют инструмент", typeof(double));
            chartDt.Columns.Add(column);
            column = new DataColumn("Не используют инструмент", typeof(double));
            chartDt.Columns.Add(column);

            int regionCount = terrRows.Length;

            DataRow newRow = chartDt.NewRow();
            if (regionCount != 0)
            {
                newRow[0] = usedRegionCount;
                newRow[1] = regionCount - usedRegionCount;
            }
            chartDt.Rows.Add(newRow);
            chartDt.AcceptChanges();

            PieChartBrick.DataTable = chartDt;
        }

        #endregion

        #region Обработчики гейджа

        private void GaugeDataBind()
        {
            AvgCalculator avgCalculator = new AvgCalculator();
            
            string sqlFilter = String.Format("[{0}] = '{1}'", bankColumn, ComboBank.SelectedValue);
            DataRow[] terrRows = bankTerritory.Select(sqlFilter);

            foreach (DataRow terrRow in terrRows)
            {
                string subject = terrRow[federalSubjectColumn].ToString().TrimEnd(' ');
                string key = String.Format("{0};{1}", subject, ComboFacility.SelectedValue);

                if (facilityEvaluaitonDictionary.ContainsKey(key))
                {
                    avgCalculator.AddValue(Convert.ToDouble(facilityEvaluaitonDictionary[key]));
                }
            }

            double avgValue = avgCalculator.GetAverage();
            string gaugeTitle = String.Format("{0}", ComboBank.SelectedValue);

            HeraldTr.Controls.AddAt(0, NewLinearGauge(gaugeTitle, avgValue, 0));
        }

        #endregion

        #region Лента гейджев

        private void GaugeRibbonDataBind()
        {
            int bankIndex = 0;
            HtmlTableRow tableRow = new HtmlTableRow();
            foreach (string bank in bankList)
            {
                AvgCalculator avgCalculator = new AvgCalculator();

                string sqlFilter = String.Format("[{0}] = '{1}'", bankColumn, bank);
                DataRow[] terrRows = bankTerritory.Select(sqlFilter);

                foreach (DataRow terrRow in terrRows)
                {
                    string subject = terrRow[federalSubjectColumn].ToString().TrimEnd(' ');
                    string key = String.Format("{0};{1}", subject, ComboFacility.SelectedValue);

                    if (facilityEvaluaitonDictionary.ContainsKey(key))
                    {
                        avgCalculator.AddValue(Convert.ToDouble(facilityEvaluaitonDictionary[key]));
                    }
                }

                double avgValue = avgCalculator.GetAverage();
                Control gauge = NewLinearGauge(String.Format("{0}", bank), avgValue, bankIndex);

                if (bankIndex % 4 == 0)
                {
                    tableRow = new HtmlTableRow();
                    GaugeRibbonTable.Rows.Add(tableRow);
                }

                tableRow.Controls.Add(gauge);
                bankIndex++;
            }
        }

        private Control NewLinearGauge(string bankName, double value, int index)
        {
            LinearGaugeIndicator gauge = (LinearGaugeIndicator)Page.LoadControl("../../Components/Gauges/LinearGaugeIndicator.ascx");

            gauge.Width = 200;
            gauge.Height = 60;
            gauge.SetRange(0, 5, 1);
            gauge.MarkerPrecision = 0.01;
            gauge.IndicatorValue = value;
            gauge.SetMarkerAnnotation(value);
            gauge.TitleText = bankName;
            gauge.SetImageUrl(index);
            gauge.GaugeContainer.Width = "300px";
            gauge.GaugeContainer.Height = "120px";
            gauge.Tooltip = String.Format("Средняя оценка: {0:N1}", value);

            HtmlTableCell cell = new HtmlTableCell();
            cell.Controls.Add(gauge);

            return cell;
        }

        #endregion

        #region Эагрузка из XML

        public void LoadXmlData()
        {
            string dataPath = Server.MapPath("../../data/Sberbank/");
            string xmlFileName = String.Format("{0}/dataset.xml", dataPath);
            string xsdFileName = String.Format("{0}/dataset.xsd", dataPath);

            DataSet ds = new DataSet();
            ds.ReadXmlSchema(xsdFileName);

            XmlDataDocument xmlDataDocument = new XmlDataDocument(ds);
            xmlDataDocument.Load(xmlFileName);

            bankTerritory = xmlDataDocument.DataSet.Tables["bank-territory"];
            sberbankFacility = xmlDataDocument.DataSet.Tables["sberbank-facility"];
            facilityEvaluation = GetFacilityEvaluation(xmlDataDocument.DataSet.Tables["facility-evaluation"]);

            // заполняем список банков
            bankList = new List<string>();
            foreach (DataRow row in bankTerritory.Rows)
            {
                string bank = row[bankColumn].ToString().TrimEnd(' ');

                if (!bankList.Contains(bank))
                {
                    bankList.Add(bank);
                }
            }

            // заполняем список механизмов
            facilityList = new List<string>();
            foreach (DataRow row in sberbankFacility.Rows)
            {
                string facility = row["name"].ToString().TrimEnd(' ');

                if (!facilityList.Contains(facility))
                {
                    facilityList.Add(facility);
                }
            }
            
            // заполняем словарь оценок и описаний механизмов
            facilityEvaluaitonDictionary = new Dictionary<string, string>();
            facilityDescriptionDictionary = new Dictionary<string, string>();
            foreach (DataRow row in facilityEvaluation.Rows)
            {
                string facility = row[sberbankFacilityColumn].ToString();
                string subject = row[federalSubjectColumn].ToString().TrimEnd(' ');
                string evaluation = row[evaluationColumn].ToString();
                string description = row[descriptionColumn].ToString();

                string key = String.Format("{0};{1}", subject, facility);

                if (!facilityEvaluaitonDictionary.ContainsKey(key))
                {
                    facilityEvaluaitonDictionary.Add(key, evaluation);
                }

                if (!facilityDescriptionDictionary.ContainsKey(key))
                {
                    facilityDescriptionDictionary.Add(key, description);
                }
            }
        }

        private DataTable GetFacilityEvaluation(DataTable xmlDt)
        {
            DataTable dt = xmlDt.Copy();

            // дописываем колонку с банками
            DataColumn newBankColumn = new DataColumn(bankColumn, typeof(String));
            dt.Columns.Add(newBankColumn);

            regionBankDicitionary = new Dictionary<string, string>();
            foreach (DataRow row in bankTerritory.Rows)
            {
                string subject = row[federalSubjectColumn].ToString();
                string bank = row[bankColumn].ToString();
                if (!regionBankDicitionary.ContainsKey(subject))
                {
                    regionBankDicitionary.Add(subject, bank);
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                string subject = row[federalSubjectColumn].ToString();
                if (regionBankDicitionary.ContainsKey(subject))
                {
                    row[bankColumn] = regionBankDicitionary[subject];
                }
            }
            
            return dt;
        }

        private void FillComboBank()
        {
            bankDicitionary = new Dictionary<string, int>();
            bankDicitionary.Add("Все тербанки", 0);
            foreach (string bank in bankList)
            {
                if (!bankDicitionary.ContainsKey(bank))
                {
                    bankDicitionary.Add(bank, 1);
                }
            }
        }

        private void FillComboFacility()
        {
            facilityDicitionary = new Dictionary<string, int>();
            //facilityDicitionary.Add("Все механизмы", 0);
            foreach (string facility in facilityList)
            {
                if (!facilityDicitionary.ContainsKey(facility))
                {
                    facilityDicitionary.Add(facility, 0);
                }
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);
        }

        #endregion
    }

    public class AvgCalculator
    {
        private double summary = 0;
        private int count = 0;

        public void AddValue(double value)
        {
            summary += value;
            count++;
        }

        public double GetAverage()
        {
            if (count != 0)
                return summary / count;
            return 0;
        }

        public void Reset()
        {
            summary = 0;
            count = 0;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Xml;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.SB_001
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable gridDt;
        private DataTable bankTerritory;
        private DataTable investmentProjectGoal;
        private DataTable sberbankFacility;
        private DataTable activityEvaluation;
        private DataTable priority;
        private DataTable facilityEvaluation;

        private List<string> bankList;
        private Dictionary<string, int> bankDicitionary;
        private Dictionary<string, string> regionBankDicitionary;

        #endregion

        public bool IsAllSelected
        {
            get { return ComboBank.SelectedNode.Level == 0; }
        }

        public bool IsFederalEvaluation
        {
            get { return EvaluationTypeButtonList.SelectedIndex == 0; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 190);

            #region Настройка грида

            GridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.7 - 235);
            GridBrick.Width = CustomReportConst.minScreenWidth - 15;
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.AutoHeightRowLimit = 15;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);

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
            }

            PageTitle.Text = "Анализ конкурентной среды";
            PageSubTitle.Text = "Оценка деятельности федеральных и региональных банков";
            Page.Title = PageTitle.Text;

            BroadCast.Text = String.Format("Главная » Территориальный банк » <a href='../../reports/sb_004/Default.aspx'>Планирование и текущая деятельность</a> » Анализ конкурентной среды");
            BroadCast.Font.Italic = true;

            mapElementCaption.Text = String.Format("{0} ({1})", ComboBank.SelectedValue, EvaluationTypeButtonList.SelectedValue.ToLower());
            gridElementCaption.Text = "Оценка деятельности конкурентов";

            GridDataBind();

            string mapPath = Server.MapPath(string.Format("../../maps/Сбербанк/{0}/{0}.shp", 
                IsAllSelected ? "РФ по тербанкам" : ComboBank.SelectedValue));
            
            DundasMap.Shapes.Clear();
            DundasMap.LoadFromShapeFile(mapPath, "NAME", true);

            SetMapSettings();
            FillMapData();
        }

        string federalSubjectColumn = "federal-subject";
        string bankColumn = "bank";
        string federalActivityEvaluationColumn = "federal-banks-activity-evaluation";
        string federalActivityCharacteristicColumn = "federal-banks-activity-characteristic";
        string regionalActivityEvaluationColumn = "regional-banks-activity-evaluation";
        string regionalActivityCharacteristicColumn = "regional-banks-activity-characteristic";
        string levelColumn = "level";

        private string mapEvaluationColumn;
        private string mapCharacteristicColumn;

        #region Обработчики грида
        
        private void GridDataBind()
        {
            gridDt = new DataTable();

            DataColumn column = new DataColumn(federalSubjectColumn, typeof(String));
            gridDt.Columns.Add(column);
            column = new DataColumn(federalActivityEvaluationColumn, typeof(double));
            gridDt.Columns.Add(column);
            column = new DataColumn(federalActivityCharacteristicColumn, typeof(String));
            gridDt.Columns.Add(column);
            column = new DataColumn(regionalActivityEvaluationColumn, typeof(double));
            gridDt.Columns.Add(column);
            column = new DataColumn(regionalActivityCharacteristicColumn, typeof(String));
            gridDt.Columns.Add(column);
            column = new DataColumn(levelColumn, typeof(String));
            gridDt.Columns.Add(column);

            if (IsAllSelected)
            {
                foreach (string bank in bankList)
                {
                    string sqlFilter = String.Format("[{0}] = '{1}'", bankColumn, bank);
                    DataRow[] terrRows = activityEvaluation.Select(sqlFilter);

                    AvgCalculator avgFederalCalculator = new AvgCalculator();
                    AvgCalculator avgRegionCalculator = new AvgCalculator();
                    foreach (DataRow terrRow in terrRows)
                    {
                        if (terrRow[federalActivityEvaluationColumn] != DBNull.Value && terrRow[federalActivityEvaluationColumn].ToString() != String.Empty)
                        {
                            avgFederalCalculator.AddValue(Convert.ToDouble(terrRow[federalActivityEvaluationColumn].ToString()));
                        }

                        if (terrRow[regionalActivityEvaluationColumn] != DBNull.Value && terrRow[regionalActivityEvaluationColumn].ToString() != String.Empty)
                        {
                            avgRegionCalculator.AddValue(Convert.ToDouble(terrRow[regionalActivityEvaluationColumn].ToString()));
                        }
                    }

                    DataRow bankRow = gridDt.NewRow();
                    bankRow[federalSubjectColumn] = bank;
                    bankRow[federalActivityEvaluationColumn] = avgFederalCalculator.GetAverage();
                    bankRow[regionalActivityEvaluationColumn] = avgRegionCalculator.GetAverage();
                    bankRow[levelColumn] = "1";
                    gridDt.Rows.Add(bankRow);
                    gridDt.AcceptChanges();

                    FillGridRows(ref gridDt, terrRows);
                }
            }
            else
            {
                string sqlFilter = String.Format("[{0}] = '{1}'", bankColumn, ComboBank.SelectedValue);
                FillGridRows(ref gridDt, activityEvaluation.Select(sqlFilter));
            }

            FontRowLevelRule levelRule = new FontRowLevelRule(gridDt.Columns.Count - 1);
            levelRule.AddFontLevel("1", GridBrick.BoldFont8pt);
            GridBrick.AddIndicatorRule(levelRule);

            GridBrick.Grid.DataSource = gridDt;
        }

        private static void FillGridRows(ref DataTable dt, DataRow[] rowCollection)
        {
            foreach (DataRow row in rowCollection)
            {
                DataRow newRow = dt.NewRow();
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    string columnName = row.Table.Columns[i].ColumnName;

                    if (dt.Columns.Contains(columnName))
                    {
                        newRow[columnName] = row[columnName];
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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
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

            GridHeaderCell federalCell = headerLayout.AddCell("Федеральные банки");
            federalCell.AddCell("Оценка");
            federalCell.AddCell("Характеристика");

            GridHeaderCell regionCell = headerLayout.AddCell("Региональные банки");
            regionCell.AddCell("Оценка");
            regionCell.AddCell("Характеристика");

            headerLayout.ApplyHeaderInfo();
        }

        private static int GetColumnWidth(string columnName)
        {
            if (columnName.Contains("evaluation"))
            {
                return 100;
            }
            return 330;
        }

        private static string GetColumnFormat(string columnName)
        {
            if (columnName.Contains("evaluation"))
            {
                return "N0";
            }
            return String.Empty;
        }

        private static HorizontalAlign GetColumnAlignment(string columnName)
        {
            if (columnName.Contains("evaluation"))
            {
                return HorizontalAlign.Right;
            }
            return HorizontalAlign.Justify;
        }

        #endregion

        #region Обработчики карты

        private void SetMapSettings()
        {
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add("subjectName");
            DundasMap.ShapeFields["subjectName"].Type = typeof(string);
            DundasMap.ShapeFields["subjectName"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("evaluation");
            DundasMap.ShapeFields["evaluation"].Type = typeof(int);
            DundasMap.ShapeFields["evaluation"].UniqueIdentifier = false;
            DundasMap.ShapeFields.Add("characteristic");
            DundasMap.ShapeFields["characteristic"].Type = typeof(string);
            DundasMap.ShapeFields["characteristic"].UniqueIdentifier = true;

            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;
            //DundasMap.Viewport.Zoom = (float)100;

            DundasMap.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap.ZoomPanel.Dock = PanelDockStyle.Right;


            // добавляем легенду
            DundasMap.Legends.Clear();

            Legend legend = new Legend("MapLegend");
            legend.Dock = PanelDockStyle.Left;
            legend.DockAlignment = DockAlignment.Far;
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
            legend.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.Title = "Оценка";
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Add(legend);

            LegendItem item = new LegendItem();
            item.Text = "5 – агрессивная экстенсивная политика";
            item.Color = Color.Red;
            DundasMap.Legends["MapLegend"].Items.Add(item);

            item = new LegendItem();
            item.Text = "4 – высокая конкуренция";
            item.Color = Color.OrangeRed;
            DundasMap.Legends["MapLegend"].Items.Add(item);

            item = new LegendItem();
            item.Text = "3 – средняя конкуренция";
            item.Color = Color.Coral;
            DundasMap.Legends["MapLegend"].Items.Add(item);

            item = new LegendItem();
            item.Text = "2 – умеренная конкуренция";
            item.Color = Color.LightCoral;
            DundasMap.Legends["MapLegend"].Items.Add(item);

            item = new LegendItem();
            item.Text = "1 – низкая конкуренция";
            item.Color = Color.MistyRose;
            DundasMap.Legends["MapLegend"].Items.Add(item);

            item = new LegendItem();
            item.Text = "0 –  представлены, не представляют конкуренции";
            item.Color = Color.LightSkyBlue;
            DundasMap.Legends["MapLegend"].Items.Add(item);
        }

        private void FillMapData()
        {
            foreach (Shape shape in DundasMap.Shapes)
            {
                if (shape.Name.Contains("Shape"))
                {
                    shape.Text = String.Empty;
                }
            }

            mapEvaluationColumn = IsFederalEvaluation ? federalActivityEvaluationColumn : regionalActivityEvaluationColumn;
            mapCharacteristicColumn = IsFederalEvaluation ? federalActivityCharacteristicColumn : regionalActivityCharacteristicColumn;

            foreach (DataRow row in gridDt.Rows)
            {
                string subjectName = row[federalSubjectColumn].ToString();

                ArrayList shapeList = FindMapShape(DundasMap, subjectName);
                foreach (Shape shape in shapeList)
                {
                    shape.Visible = true;

                    shape.BorderWidth = 1;
                    shape.BorderStyle = MapDashStyle.Solid;
                    shape.BorderColor = Color.Black;

                    int evaluation = Int32.MinValue;
                    if (row[mapEvaluationColumn] != DBNull.Value && row[mapEvaluationColumn].ToString() != String.Empty)
                    {
                        evaluation = Convert.ToInt32(Convert.ToDouble(row[mapEvaluationColumn].ToString()));
                    }

                    string characteristic = String.Empty;
                    if (row[mapCharacteristicColumn] != DBNull.Value)
                    {
                        characteristic = row[mapCharacteristicColumn].ToString();
                    }

                    if (evaluation != Int32.MinValue)
                    {
                        shape["subjectName"] = subjectName;
                        shape["evaluation"] = evaluation;
                        shape["characteristic"] = characteristic;
                        shape.TextVisibility = TextVisibility.Shown;
                        shape.ToolTip = string.Format("{0}\nОценка деятельности: {1:N0}\n{2}", subjectName, evaluation, characteristic);

                        if (evaluation == 0)
                        {
                            shape.Color = Color.LightSkyBlue;
                        }
                        else if (evaluation == 5)
                        {
                            shape.Color = Color.Red;
                        }
                        else if (evaluation == 4)
                        {
                            shape.Color = Color.OrangeRed;
                        }
                        else if (evaluation == 3)
                        {
                            shape.Color = Color.Coral;
                        }
                        else if (evaluation == 2)
                        {
                            shape.Color = Color.LightCoral;
                        }
                        else if (evaluation == 1)
                        {
                            shape.Color = Color.MistyRose;
                        }

                        shape.Text = String.Format("{0}\n{1}", subjectName.Replace(" ", "\n"), evaluation.ToString("N0"));
                    }
                }
            }
        }

        public static ArrayList FindMapShape(MapControl map, string patternValue)
        {
            patternValue = patternValue.TrimEnd(' ');
            patternValue = patternValue.Replace("Чувашская Республика", "Республика Чувашия");
            patternValue = patternValue.Replace("Еврейская автономная область", "Еврейская АО");
            patternValue = patternValue.Replace("Ханты-Мансийский автономный округ - Югра", "Ханты-Мансийский АО");
            patternValue = patternValue.Replace("Ямало-Ненецкий автономный округ", "Ямало-Ненецкий АО");
            patternValue = patternValue.Replace("Удмуртская Республика", "Республика Удмуртия");
            patternValue = patternValue.Replace("Ненецкий автономный округ", "Ненецкий АО");
            patternValue = patternValue.Replace("Чукотский автономный округ", "Чукотский АО");
            patternValue = patternValue.Replace("Чеченская Республика", "Республика Чечня");
            patternValue = patternValue.Replace("Карачаево-Черкесская Республика", "Республика Карачаево-Черкессия");
            patternValue = patternValue.Replace("Кабардино-Балкарская Республика", "Республика Кабардино-Балкария");
            patternValue = patternValue.Replace("Республика Северная Осетия -Алания", "Республика Северная Осетия");

            ArrayList shapeList = new ArrayList();
            foreach (Shape shape in map.Shapes)
            {
                if (shape.Name == patternValue)
                {
                    shapeList.Add(shape);
                }
            }

            return shapeList;
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
            investmentProjectGoal = xmlDataDocument.DataSet.Tables["investment-project-goal"];
            sberbankFacility = xmlDataDocument.DataSet.Tables["sberbank-facility"];
            activityEvaluation = GetActivityEvaluation(xmlDataDocument.DataSet.Tables["activity-evaluation"]);
            priority = xmlDataDocument.DataSet.Tables["priority"];
            facilityEvaluation = xmlDataDocument.DataSet.Tables["facility-evaluation"];

            bankList = new List<string>();
            foreach (DataRow row in bankTerritory.Rows)
            {
                string bank = row[bankColumn].ToString().TrimEnd(' ');

                if (!bankList.Contains(bank))
                {
                    bankList.Add(bank);
                }
            }
        }

        private DataTable GetActivityEvaluation(DataTable xmlDt)
        {
            DataTable dt = xmlDt.Copy();

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

            Worksheet sheet2 = workbook.Worksheets.Add("Карта");
            ReportExcelExporter1.Export(DundasMap, mapElementCaption.Text, sheet2, 3);
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

            ISection section2 = report.AddSection();
            DundasMap.Width = Convert.ToInt32(DundasMap.Width.Value * 0.8);
            ReportPDFExporter1.Export(DundasMap, mapElementCaption.Text, section2);
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
                return summary/count;
            return 0;
        }

        public void Reset()
        {
            summary = 0;
            count = 0;
        }
    }
}
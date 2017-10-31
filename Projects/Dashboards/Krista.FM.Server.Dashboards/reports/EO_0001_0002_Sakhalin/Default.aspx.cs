using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.EO_0001_0002_Sakhalin
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DateTime PeriodYear;
        private GridHeaderLayout headerLayout;
        private string SelectState;
        private string SelectStateCust;
        #endregion

        #region Параметры запроса

        private CustomParam CustomersValues;

        #endregion

        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1200; }
        }

        private int MinScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }

        private int MinScreenHeight
        {
            get { return IsSmallResolution ? 600 : CustomReportConst.minScreenHeight; }
        }


        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 200);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.9 - 550);
            //UltraWebGrid.Width = CRHelper.GetGridWidth(MinScreenWidth);
            //UltraWebGrid.Height = CRHelper.GetGridHeight(MinScreenHeight - 400);//IsSmallResolution ? CRHelper.GetGridHeight(MinScreenHeight - 270) : CRHelper.GetGridHeight(MinScreenHeight - 210);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров запроса

            if (CustomersValues == null)
            {
                CustomersValues = UserParams.CustomParam("Customers");
            }

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }
        private MemberAttributesDigest Period;
        private MemberAttributesDigest Customers;
        private MemberAttributesDigest Territory;
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("EO_0001_0002_Sakhalin_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            PeriodYear = new DateTime(Convert.ToInt32(dtDate.Rows[0][0]), 1, 1);
            UserParams.KDLevel.Value = ComboCustomers.SelectedValue;
            UserParams.PeriodLastYear.Value = ComboPeriod.SelectedValue;
            Territory = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "EO_0001_0002_Sakhalin_Territory");
            if (!Page.IsPostBack)
            {
                ComboTerritory.Title = "МО";
                ComboTerritory.Width = 250;
                ComboTerritory.MultiSelect = false;
                ComboTerritory.AutoPostBack = true;
                ComboTerritory.FillDictionaryValues(
                    CustomMultiComboDataHelper.FillMemberUniqueNameList(Territory.UniqueNames, Territory.MemberLevels));
                ComboTerritory.SetSelectedNode(ComboTerritory.GetLastNode(0), true);
                SelectState = string.Empty;
                UserParams.KDLevel.Value = "Все заказчики";
            }
            UserParams.Mo.Value = Territory.GetMemberUniqueName(ComboTerritory.SelectedValue);

            Period = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "EO_0001_0002_Sakhalin_Period");

            ComboPeriod.Title = "Период";
            ComboPeriod.Width = 280;
            ComboPeriod.ClearNodes();
            ComboPeriod.MultiSelect = false;
            ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(
                Period.UniqueNames, Period.MemberLevels));

            if (!Page.IsPostBack)
            {
                ComboPeriod.SelectLastNode();
            }
            else
            {
                if (Period.GetMemberUniqueName(UserParams.PeriodLastYear.Value) != string.Empty)
                {
                    ComboPeriod.SetСheckedState(UserParams.PeriodLastYear.Value, true);
                }
                else
                {
                    ComboPeriod.SelectLastNode();
                }
            }
            SelectState = ComboPeriod.SelectedValue;
            Customers = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider,
                                                       "EO_0001_0002_Sakhalin_Customers");

            ComboCustomers.Title = "Заказчики";
            ComboCustomers.Width = 560;
            ComboCustomers.MultiSelect = false;
            ComboCustomers.ClearNodes();
            ComboCustomers.FillDictionaryValues(
                CustomMultiComboDataHelper.FillMemberUniqueNameList(Customers.UniqueNames, Customers.MemberLevels));
            if (!Page.IsPostBack)
            {
                ComboCustomers.SetСheckedState(UserParams.KDLevel.Value, true);

            }
            else
            {
                if ((UserParams.KDLevel.Value == string.Empty) || (Customers.GetMemberUniqueName(UserParams.KDLevel.Value) == string.Empty))
                {
                    ComboCustomers.SetСheckedState("Все заказчики", true);

                }
                else
                {
                    ComboCustomers.SetСheckedState(UserParams.KDLevel.Value, true);

                }
            }

            PeriodYear = CRHelper.DateByPeriodMemberUName(Period.GetMemberUniqueName(ComboPeriod.SelectedValue), 3);
            UserParams.PeriodYear.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", PeriodYear, 1);
            UserParams.PeriodQuater.Value = Period.GetMemberUniqueName(ComboPeriod.SelectedValue);

            string cust = string.Empty;
            if (ComboCustomers.SelectedValue == "Все заказчики")
            {
                CustomersValues.Value = "[Исполнение расходов__Заказчики].[Исполнение расходов__Заказчики].[Все заказчики].children";
                cust = "по всем заказчикам";
            }
            else
            {
                CustomersValues.Value = Customers.GetMemberUniqueName(ComboCustomers.SelectedValue);
                cust = "заказчика «" + ComboCustomers.SelectedValue + "»";
            }

            Page.Title = PageTitle.Text = "Исполнение муниципальных целевых программ";
            PageSubTitle.Text = IsSmallResolution ? string.Format("Анализ исполнения муниципальных целевых программ <br/> {0}, {1}, за {2} ",
                cust, ComboTerritory.SelectedValue, ComboPeriod.SelectedValue) : string.Format("Анализ исполнения муниципальных целевых программ {0}, {1}, за {2} ",
                cust, ComboTerritory.SelectedValue, ComboPeriod.SelectedValue);
            UltraWebGrid.Bands.Clear();
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.DataBind();
        }

        protected string GetGaugeUrl(object oValue)
        {
            if (oValue == DBNull.Value)
                return String.Empty;
            double value = Convert.ToDouble(oValue) * 100;
            if (value > 100)
                value = 100;
            string path = "EO_0001_0002_Sakhalin_gauge_" + value.ToString("N0") + ".png";
            string returnPath = String.Format("../../TemporaryImages/{0}", path);
            string serverPath = String.Format("~/TemporaryImages/{0}", path);

            if (File.Exists(Server.MapPath(serverPath)))
                return returnPath;
            LinearGaugeScale scale = ((LinearGauge)Gauge.Gauges[0]).Scales[0];
            scale.Markers[0].Value = value;
            MultiStopLinearGradientBrushElement BrushElement = (MultiStopLinearGradientBrushElement)(scale.Markers[0].BrushElement);
            BrushElement.ColorStops.Clear();

            if (value > 70)
            {
                BrushElement.ColorStops.Add(Color.FromArgb(223, 255, 192), 0);
                BrushElement.ColorStops.Add(Color.FromArgb(128, 255, 128), 0.41F);
                BrushElement.ColorStops.Add(Color.FromArgb(0, 192, 0), 0.428F);
                BrushElement.ColorStops.Add(Color.Green, 1);
            }
            else
            {
                if (value < 50)
                {

                    BrushElement.ColorStops.Add(Color.FromArgb(253, 119, 119), 0);
                    BrushElement.ColorStops.Add(Color.FromArgb(239, 87, 87), 0.41F);
                    BrushElement.ColorStops.Add(Color.FromArgb(224, 0, 0), 0.428F);
                    BrushElement.ColorStops.Add(Color.FromArgb(199, 0, 0), 1);
                }
                else
                {
                    BrushElement.ColorStops.Add(Color.FromArgb(255, 255, 128), 0);
                    BrushElement.ColorStops.Add(Color.Yellow, 0.41F);
                    BrushElement.ColorStops.Add(Color.Yellow, 0.428F);
                    BrushElement.ColorStops.Add(Color.FromArgb(255, 128, 0), 1);
                }
            }

            Size size = new Size(150, 30);
            Gauge.SaveTo(Server.MapPath(serverPath), GaugeImageType.Png, size);
            return returnPath;

        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {

            string query = DataProvider.GetQueryText("EO_0001_0002_Sakhalin_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtGrid);
            if (dtGrid.Rows.Count >= 1)
            {
                DataTable dtNewTable = new DataTable();



                dtNewTable.Columns.Add("Наименование целевой программы", typeof(string));
                dtNewTable.Columns.Add("План", typeof(double));
                dtNewTable.Columns.Add("Факт", typeof(double));
                dtNewTable.Columns.Add("% Исполнения", typeof(double));
                dtNewTable.Columns.Add("Ранг", typeof(string));
                foreach (DataRow row in dtGrid.Rows)
                {

                    string customerName = GetStringDT(row, 1);
                    string programName = GetStringDT(row, 2);
                    double planValue = GetDoubleDT(row, 3);
                    double factValue = GetDoubleDT(row, 4);
                    double percent = GetDoubleDT(row, 5);
                    string customerRank = GetStringDT(row, 6);
                    if (customerRank != string.Empty && customerRank == "0")
                    {

                        DataRow newGroupRow = dtNewTable.NewRow();
                        newGroupRow[0] = customerName;
                        newGroupRow[4] = customerRank;
                        dtNewTable.Rows.Add(newGroupRow);
                    }


                    DataRow newRow = dtNewTable.NewRow();
                    newRow[0] = programName;
                    newRow[1] = planValue;
                    newRow[2] = factValue;
                    newRow[3] = percent;
                    dtNewTable.Rows.Add(newRow);


                }
                dtNewTable.AcceptChanges();
                UltraWebGrid.DataSource = dtNewTable;
            }
            FillDynamicText();
        }
        void FillDynamicText()
        {
            string[] splitPeriodQuarter = ComboPeriod.SelectedValue.Split(" ");
            string stateOn = splitPeriodQuarter[0] + " " + splitPeriodQuarter[1] + " " + PeriodYear.Year;
            int countRecord = 0;
            double sumPlan = 0;
            double sumFact = 0;
            double percentFact = 0;
            string query = DataProvider.GetQueryText("EO_0001_0002_Sakhalin_DynamicText");
            DataTable dtDynamicTextInfo = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtDynamicTextInfo);
            if (dtDynamicTextInfo.Rows.Count > 1)
            {
                object objPlan, objFact;

                for (int i = 0; i < dtDynamicTextInfo.Rows.Count; i++)
                {
                    objPlan = dtDynamicTextInfo.Rows[i][1];
                    objFact = dtDynamicTextInfo.Rows[i][2];
                    if (objPlan != DBNull.Value)
                    {
                        sumPlan = sumPlan + Convert.ToDouble(objPlan);
                    }
                    if (objFact != DBNull.Value)
                    {
                        sumFact = sumFact + Convert.ToDouble(objFact);
                    }

                    countRecord++;
                }
                if (sumPlan != 0)
                {
                    percentFact = sumFact / sumPlan;
                }
                CreateDictionaryWords(countRecord);

                DynamicText.Text =
                    string.Format(
                        "<span style = \" \">В <b>{0}</b> году в <b>{6}</b> действуют <b>{1}</b> {7} на общую сумму <b>{2:N2}</b> руб.<br/> За <b>{3}</b> фактически исполнено по муниципальным целевым программам <b>{4:N2}</b> руб. (<b>{5:P2}</b>)</br>",
                        PeriodYear.Year, countRecord, sumPlan, ComboPeriod.SelectedValue, sumFact, percentFact,
                        ComboTerritory.SelectedValue.Replace("Город", "г."), dataDictionary[countRecord]);
                query = DataProvider.GetQueryText("EO_0001_0002_Sakhalin_Top5Min");
                DataTable dtTop5Min = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtTop5Min);
                dtTop5Min.Columns.RemoveAt(0);
                string sTop5Min = string.Empty;
                if (dtTop5Min.Rows.Count >= 1)
                {
                    for (int i = 0; i < dtTop5Min.Rows.Count; i++)
                    {
                        sTop5Min = sTop5Min +
                                   string.Format("<b>{0:P2}</b> {1} ({2})</br>", dtTop5Min.Rows[i]["% исполнения"],
                                                 dtTop5Min.Rows[i]["Имя программы"], dtTop5Min.Rows[i]["Имя заказчика"]);
                    }
                    DynamicText.Text = DynamicText.Text +
                                       string.Format(
                                           "<img src=\"../../images/starGrayBB.png\" style=\"background-position: 30% center;margin-top: 15px;\">  <b>Минимальный</b> процент исполнения:</br>{0}</img> ",
                                           sTop5Min);
                }
                query = DataProvider.GetQueryText("EO_0001_0002_Sakhalin_Top5Max");
                DataTable dtTop5Max = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtTop5Max);
                dtTop5Max.Columns.RemoveAt(0);
                string sTop5Max = string.Empty;
                if (dtTop5Max.Rows.Count >= 1)
                {
                    for (int i = 0; i < dtTop5Max.Rows.Count; i++)
                    {
                        sTop5Max = sTop5Max +
                                   string.Format("<b>{0:P2}</b> {1} ({2})</br>", dtTop5Max.Rows[i]["% исполнения"],
                                                 dtTop5Max.Rows[i]["Имя программы"], dtTop5Max.Rows[i]["Имя заказчика"]);
                    }
                    DynamicText.Text = DynamicText.Text +
                                       string.Format(
                                           "<img src=\"../../images/starYellowBB.png\" style=\"background-position: 30% center;margin-top: 15px;\">  <b>Максимальный</b> процент исполнения:</br>{0}</img> ",
                                           sTop5Max);
                }
            }
        }

        private Dictionary<int, string> dataDictionary = new Dictionary<int, string>();
        private void CreateDictionaryWords(int countRec)
        {
            int i = 0;
            while (i <= countRec)
            {
                dataDictionary.Add(i, "муниципальных целевых программ");
                dataDictionary.Add(i + 1, "муниципальная целевая программа");
                dataDictionary.Add(i + 2, "муниципальные целевые программы");
                dataDictionary.Add(i + 3, "муниципальные целевые программы");
                dataDictionary.Add(i + 4, "муниципальные целевые программы");
                dataDictionary.Add(i + 5, "муниципальных целевых программ");
                dataDictionary.Add(i + 6, "муниципальных целевых программ");
                dataDictionary.Add(i + 7, "муниципальных целевых программ");
                dataDictionary.Add(i + 8, "муниципальных целевых программ");
                dataDictionary.Add(i + 9, "муниципальных целевых программ");
                i = i + 10;
            }
        }

        private string GetStringDT(DataRow row, int indexColumn)
        {

            if (row[indexColumn] != DBNull.Value && row[indexColumn] != string.Empty)
            {
                return row[indexColumn].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        private double GetDoubleDT(DataRow row, int indexColumn)
        {
            if (row[indexColumn] != DBNull.Value && row[indexColumn] != string.Empty)
            {
                return Convert.ToDouble(row[indexColumn].ToString());
            }
            else
            {
                return 0;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(400);
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
            }
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 2].Width = CRHelper.GetColumnWidth(245);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P2");
            headerLayout.AddCell("Наименование целевой программы");
            headerLayout.AddCell("План на " + PeriodYear.Year + " год, руб.");
            headerLayout.AddCell("Факт за  " + ComboPeriod.SelectedValue + ", руб.");
            headerLayout.AddCell("Процент исполнения");
            headerLayout.ApplyHeaderInfo();
        }
        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int cellCount = e.Row.Cells.Count - 1;
            int rowType = -1;
            if (e.Row.Cells[cellCount].Value != null && e.Row.Cells[cellCount].Value.ToString() != String.Empty)
            {
                rowType = Convert.ToInt32(e.Row.Cells[cellCount].Value);
            }

            if (rowType == 0)
            {
                e.Row.Style.BorderDetails.WidthTop = 3;
                e.Row.Cells[0].ColSpan = 4;
                e.Row.Cells[0].Style.Font.Size = 10;
                e.Row.Cells[0].Style.Font.Bold = true;
            }
            else
            {
                #region Gauge

                e.Row.Cells[3].Style.BackgroundImage = GetGaugeUrl(e.Row.Cells[3].Value);
                e.Row.Cells[3].Style.CustomRules =
                    "background-repeat: no-repeat; background-position: 30% center; margin: 2px";


                #endregion
            }
        }
        #endregion

        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text.Replace("<br/>", string.Empty);
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.HeaderCellHeight = 70;
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            Report report = new Report();
            ISection section1 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 40;
            ReportPDFExporter1.Export(headerLayout, PageSubTitle.Text.Replace("<br/>", "/n"), section1);
        }

        #endregion
    }
}

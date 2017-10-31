using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0001_0021
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private string query;
        private int firstYear = 2001;
        private int endYear;
        private string month;
        private DateTime currentDate;
        private static MemberAttributesDigest territoryDigest;


        private GridHeaderLayout headerLayout1;
        private GridHeaderLayout headerLayout2;
        private GridHeaderLayout headerLayout3;

        #endregion

        #region Параметры запроса

        private CustomParam territory;
        private CustomParam rubMultiplier;
        private CustomParam curYear;


        #endregion


        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string RubMultiplierCaption
        {
            get { return IsThsRubSelected ? "тыс.руб." : "млн.руб."; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 2.0);
            UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid2.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.9 - 120);
            UltraWebGrid2.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid2.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid3.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid3.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.9 - 120);
            UltraWebGrid3.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid3.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров запроса

            territory = UserParams.CustomParam("territory");
            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            curYear = UserParams.CustomParam("cur_year");
            

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                CustomCalendar1.Visible = true;
                // Получаем последнюю дату
                query = DataProvider.GetQueryText("FO_0001_0021_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                query = DataProvider.GetQueryText("FO_0001_0021_dateCalendar");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                DateTime date = new DateTime(Convert.ToInt32(dtDate.Rows[0][0].ToString()), CRHelper.MonthNum(dtDate.Rows[0][3].ToString()), Convert.ToInt32(dtDate.Rows[0][4])); 
               
                // Инициализируем календарь
                CustomCalendar1.WebCalendar.SelectedDate = date;
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 150;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(month, true);
                
                territoryDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0001_0021_territory");
                ComboTerritory.Title = "Территория";
                ComboTerritory.Width = 300;
                ComboTerritory.MultiSelect = false;
                ComboTerritory.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(territoryDigest.UniqueNames, territoryDigest.MemberLevels));
                ComboTerritory.SetСheckedState("", true);
            }

            Label1.Text = string.Empty;
            Label2.Text = string.Empty;
            Label3.Text = string.Empty;

            Page.Title = string.Format("Справка об отдельных показателях исполнения бюджета");
            PageTitle.Text = Page.Title;
            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), CRHelper.MonthNum(ComboMonth.SelectedValue), 1);
            PageSubTitle.Text = string.Format("{0} по состоянию на {1:dd.MM.yyyy} год, {2}", ComboTerritory.SelectedValue,currentDate, RubMultiplierCaption);

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodLastYear.Value = (Convert.ToInt32(UserParams.PeriodYear.Value) - 1).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(ComboMonth.SelectedValue)));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(ComboMonth.SelectedValue)));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName(String.Empty, CustomCalendar1.WebCalendar.SelectedDate, 5); ;
            curYear.Value = CustomCalendar1.WebCalendar.SelectedDate.Year.ToString();
            territory.Value = territoryDigest.GetMemberUniqueName(ComboTerritory.SelectedValue);
            rubMultiplier.Value = IsThsRubSelected ? "1000" : "1000000";

            DataTable dtTitle = new DataTable();
            query = DataProvider.GetQueryText("FO_0001_0021_Title");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query,"Заголовок", dtTitle);

            if (dtTitle.Rows.Count> 0)
            {
               if (dtTitle.Rows[0][1] != DBNull.Value && dtTitle.Rows[0][1].ToString() != string.Empty )
                {
                    Label1.Text = string.Format("численность населения на 01.01.{0} (человек) - {1}", ComboYear.SelectedValue, dtTitle.Rows[0][1]);
                }
               if (dtTitle.Rows[0][2] != DBNull.Value && dtTitle.Rows[0][2].ToString() != string.Empty)
               {
                   Label2.Text = string.Format("количество населенных пунктов - {0}", dtTitle.Rows[0][2]);
               }
               if (dtTitle.Rows[0][3] != DBNull.Value && dtTitle.Rows[0][3].ToString() != string.Empty)
               {
                   Label3.Text = string.Format("городских и сельских поселений - {0}", dtTitle.Rows[0][3]);
               }
            }
            headerLayout1 = new GridHeaderLayout(UltraWebGrid1);
            headerLayout2 = new GridHeaderLayout(UltraWebGrid2);
            headerLayout3 = new GridHeaderLayout(UltraWebGrid3);

            UltraWebGrid1.Bands.Clear();
            UltraWebGrid2.Bands.Clear();
            UltraWebGrid3.Bands.Clear();

            UltraWebGrid1.DataBind();
            UltraWebGrid2.DataBind();
            UltraWebGrid3.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
           
            string query = DataProvider.GetQueryText("FO_0001_0021_Grid1");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
              UltraWebGrid1.DataSource = dtGrid;
            }
        }
        
        protected void UltraWebGrid2_DataBinding(object sender, EventArgs e)
        {

            string query = DataProvider.GetQueryText("FO_0001_0021_Grid2");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid2.DataSource = dtGrid;
            }
        }
        
        protected void UltraWebGrid3_DataBinding(object sender, EventArgs e)
        {

            string query = DataProvider.GetQueryText("FO_0001_0021_Grid3");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                for (int i= 0 ; i< dtGrid.Rows.Count; i++)
                {
                    if (dtGrid.Rows[i][dtGrid.Columns.Count - 1] != DBNull.Value && dtGrid.Rows[i][dtGrid.Columns.Count - 1].ToString() != string.Empty)
                    {
                        dtGrid.Rows[i][0] = dtGrid.Rows[i][dtGrid.Columns.Count - 1];
                    }
                }
                UltraWebGrid3.DataSource = dtGrid;
            }
        }

        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(300);

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

            headerLayout1.AddCell("Показатели");
            headerLayout1.AddCell(string.Format("Годовой план на {0:dd.MM.yyyy}", currentDate ), "Уточненные годовые назначения");
            headerLayout1.AddCell(string.Format("Исполнено на {0:dd.MM.yyyy}", currentDate ), "Сумма фактического исполнения с начала года на первое число текущего месяца");
            headerLayout1.AddCell("% исполнения", "Процент исполнения уточненных годовых назначений на текущий месяц");
            string area = ComboTerritory.SelectedValue.Contains("район") ? "районам" : "городам";
            headerLayout1.AddCell(string.Format("Средний по {0}", area), string.Format("Средний процент исполнения уточненных годовых назначений среди {0} субъекта", ComboTerritory.SelectedValue.Contains("район") ? "районов" : "городов"));
            GridHeaderCell cell = headerLayout1.AddCell("Темп роста/снижения к прошлому году, %");
            cell.AddCell(string.Format("По {0}", ComboTerritory.SelectedValue.Contains("район") ? "району" : "городу"), "Темп роста фактического исполнения на текущую дату к сумме за аналогичный период прошлого года");
            cell.AddCell(string.Format("Средний по {0}", area),string.Format("Средний темп роста фактического исполнения среди {0} субъекта", ComboTerritory.SelectedValue.Contains("район") ? "районов" : "городов"));
           
           for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++ )
             {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
             }

            headerLayout1.ApplyHeaderInfo();
        }


        private static bool IsInvertIndication(string indicatorName) // красная стрелка вверх, зеленая вниз
        {
            switch (indicatorName)
            {
                case "Всего расходов, в том числе:":
                case "оплата коммунальных услуг":
                case "работы, услуги по содержанию имущества":
                case "прочие расходы":
                case "Муниципальный долг":
                case "Кредиторская задолженность, всего":
                case "по заработной плате":
                case "по начислениям на выплаты по оплате труда":
                case "по оплате коммунальных услуг бюджетными учреждениями":
                case "Недоимка по местным налогам и налогам со спец.налоговыми режимами":
                case "Дебиторская задолженность предприятий ЖКХ, всего":
                case "в том числе неплатежи населения":
                    {
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }
        
        protected void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            string indicatorName = String.Empty;
            if (e.Row.Cells[0].Value != null)
            {
                indicatorName = e.Row.Cells[0].Value.ToString();
            }

            bool isInvertIndication = IsInvertIndication(indicatorName.TrimEnd(' '));

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (i == 5 || i == 6)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {

                        if (Convert.ToDouble(e.Row.Cells[i].Value) >= 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = isInvertIndication ? "~/images/arrowRedUpBB.png" : "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "рост относительно прошлого года";
                        }
                        else if (Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = isInvertIndication ? "~/images/arrowGreenDownBB.png" : "~/images/arrowRedDownBB.png";
                            e.Row.Cells[i].Title = "снижение относительно прошлого года";
                        }

                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }
            }

            string level = string.Empty;
            if (e.Row.Cells[e.Row.Cells.Count - 1] != null && e.Row.Cells[e.Row.Cells.Count - 1].ToString() != string.Empty)
            {
                level = e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString();
            }

            if (e.Row.Cells[0] != null && e.Row.Cells[0].ToString() != string.Empty)
            {
                for (int i = 0; i < 8; i++)
                {
                    e.Row.Cells[0].Value = e.Row.Cells[0].Value.ToString().Replace("_", "");
                }
                switch (level)
                {
                    case "1":
                        {
                            e.Row.Cells[0].Style.Padding.Left = 10;
                            break;
                        }
                    case "2":
                        {
                            e.Row.Cells[0].Style.Padding.Left = 20;
                            break;
                        }
                }
            }


           

        }

        protected void UltraWebGrid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(300);

            headerLayout2.AddCell("Показатели");
            headerLayout2.AddCell(string.Format("Исполнено на 01.01.{0}", ComboYear.SelectedValue));
            headerLayout2.AddCell(string.Format("Исполнено на {0:dd.MM.yyyy}", currentDate));
            headerLayout2.AddCell(string.Format("Изменение"));
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
            }

            headerLayout2.ApplyHeaderInfo();
        }

        protected void UltraWebGrid2_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Hidden = true;
            }
            string indicatorName = String.Empty;
            if (e.Row.Cells[0].Value != null)
            {
                indicatorName = e.Row.Cells[0].Value.ToString();
            }

            bool isInvertIndication = IsInvertIndication(indicatorName.TrimEnd(' '));

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (i == 3)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {

                        if (Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = isInvertIndication ? "~/images/arrowRedUpBB.png" : "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "Рост показателя относительно начала года";
                        }
                        else if (Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = isInvertIndication ? "~/images/arrowGreenDownBB.png" : "~/images/arrowRedDownBB.png";
                            e.Row.Cells[i].Title = "Снижение показателя относительно начала года";
                        }

                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }
            }
            
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
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

           if (e.Row.Index == 5 || e.Row.Index == 3 || e.Row.Index == 4 || e.Row.Index == 8 || e.Row.Index == 11 || e.Row.Index == 10 )
           {
               e.Row.Cells[0].Style.Padding.Left = 20;
           }
        }

        protected void UltraWebGrid3_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(300);
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

            headerLayout3.AddCell("Справочная информация");
            headerLayout3.AddCell(string.Format("Годовой план на {0}", CustomCalendar1.WebCalendar.SelectedDate.ToString("dd.MM.yyyy")));
            headerLayout3.AddCell(string.Format("Исполнено на {0}", CustomCalendar1.WebCalendar.SelectedDate.ToString("dd.MM.yyyy")));
            headerLayout3.AddCell(string.Format("% исполнения"));
            headerLayout3.AddCell(string.Format("Средний по {0}", ComboTerritory.SelectedValue.Contains("район") ? "районам" : " городам"));

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
            }

            headerLayout3.ApplyHeaderInfo();
        }

        protected void UltraWebGrid3_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count - 1; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
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

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if ( ((UltraWebGrid) sender).Rows.Count < 13)
            {
                ((UltraWebGrid) sender).Height = Unit.Empty;
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
            Worksheet sheet3 = workbook.Worksheets.Add("sheet3");

            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
            ReportExcelExporter1.Export(headerLayout1, sheet1, 3);
            ReportExcelExporter1.Export(headerLayout2, sheet2, 3);
            ReportExcelExporter1.Export(headerLayout3, sheet3, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(Object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();

            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout1, section1);
            ReportPDFExporter1.Export(headerLayout2, section2);
            ReportPDFExporter1.Export(headerLayout3, section3);


        }
      
        #endregion
    }
}

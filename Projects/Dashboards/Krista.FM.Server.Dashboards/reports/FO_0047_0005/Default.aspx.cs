using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0047_0005
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable gridDt = new DataTable();
        private int firstYear = 2010;
        private DateTime currentDate;
        private int currentQuarterIndex;

        private static MemberAttributesDigest debtKindDigest;
        private static MemberAttributesDigest meansTypeDigest;
        private static MemberAttributesDigest directionTypeDigest;
        private static MemberAttributesDigest kosguDigest;

        #endregion

        public bool IsArrearsDebtSelected
        {
            get { return ArrearsCheckBox.Checked; }
        }

        #region Параметры запроса

        // множество строк грида
        private CustomParam gridRowSet;
        // вид задолженности
        private CustomParam debtKind;
        // тип средств
        private CustomParam meansType;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GridBrick.Height = CustomReportConst.minScreenHeight - 300;
            GridBrick.Width = CustomReportConst.minScreenWidth - 15;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);

            #endregion

            #region Инициализация параметров запроса

            gridRowSet = UserParams.CustomParam("grid_row_set");
            debtKind = UserParams.CustomParam("debt_kind");
            meansType = UserParams.CustomParam("means_type");
  
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink.Visible = false;
            CrossLink.Text = "Сравнение&nbsp;плановых&nbsp;показателей по&nbsp;основным&nbsp;параметрам&nbsp;бюджетов";
            CrossLink.NavigateUrl = "~/reports/FO_0002_0032/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                DateTime lastDate = CubeInfoHelper.FoDebtKzDz.LastDate;
                string lastQuarter = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(lastDate.Month));
                
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastDate.Year));
                ComboYear.SetСheckedState(lastDate.Year.ToString(), true);

                ComboQuarter.Title = "Квартал";
                ComboQuarter.Width = 160;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillQuaters(false));
                ComboQuarter.RemoveTreeNodeByName("Квартал 1");
                ComboQuarter.SetСheckedState(lastQuarter, true);
                
                ComboDebt.Title = "Задолженность";
                ComboDebt.Width = 300;
                ComboDebt.MultiSelect = false;
                debtKindDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0047_0005_debtKind");
                ComboDebt.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(debtKindDigest.UniqueNames, debtKindDigest.MemberLevels));
                ComboDebt.SetСheckedState("Дебиторская задолженность", true);
                
                ComboMeans.Title = "Тип средств";
                ComboMeans.Width = 300;
                ComboMeans.MultiSelect = true;
                meansTypeDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0047_0005_meansType");
                ComboMeans.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(meansTypeDigest.UniqueNames, meansTypeDigest.MemberLevels));
                ComboMeans.SetСheckedState("Всего по бюджетным и внебюджетным средствам", true);

                kosguDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0047_0005_kosguDigest");
            }
            
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            currentQuarterIndex = ComboQuarter.SelectedIndex + 2;
            currentDate = new DateTime(yearNum, 3 * currentQuarterIndex, 1);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));

            Page.Title = String.Format("Информация о {0}{1} муниципальных образований в разрезе КОСГУ",
                IsArrearsDebtSelected ? "просроченной " : String.Empty,
                ComboDebt.SelectedValue.Contains("Дебиторская")
                                  ? "дебиторской задолженности"
                                  : "кредиторской задолженности");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("{0}, тыс.руб.", GetQuarterDateText(currentDate));

            debtKind.Value = String.Format("{0}{1}", IsArrearsDebtSelected ? "Просроченная " : String.Empty, ComboDebt.SelectedValue);

            meansType.Value = String.Empty;
            foreach (string value in ComboMeans.SelectedValues)
            {
                meansType.Value += String.Format(",{0}", meansTypeDigest.GetMemberUniqueName(value));
            }
            meansType.Value = meansType.Value.Trim(',');

            GridDataBind();
        }

        private static string GetQuarterDateText(DateTime dateTime)
        {
            int quarterIndex = CRHelper.QuarterNumByMonthNum(dateTime.Month);

            switch (quarterIndex)
            {
                case 2:
                    {
                        return String.Format("по состоянию на 01.07.{0} года", dateTime.Year);
                    }
                case 3:
                    {
                        return String.Format("по состоянию на 01.10.{0} года", dateTime.Year);
                    }
                case 4:
                    {
                        return String.Format("по состоянию на 01.01.{0} года", dateTime.Year + 1);
                    }
                default:
                    {
                        return String.Empty;
                    }
            }
        }

        #region Обработчики грида
        
        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0047_0005_grid");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                FontRowLevelRule levelRule = new FontRowLevelRule(gridDt.Columns.Count - 1);
                levelRule.AddFontLevel("0", GridBrick.BoldFont8pt);
                GridBrick.AddIndicatorRule(levelRule);

                foreach (DataRow row in gridDt.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = row[0].ToString().Replace("муниципальное образование", "МО");
                        row[0] = row[0].ToString().Replace("муниципальный район", "МР");
                    }
                }

                GridBrick.DataTable = gridDt;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
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
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            headerLayout.AddCell("Муниципальные образования");

            string currentDateStr = GetQuarterDateText(currentDate);

            string currentGroupName = String.Empty;
            GridHeaderCell groupCell = null;
            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                string columnCaption = e.Layout.Bands[0].Columns[i].Header.Caption;
                string[] captionParts = columnCaption.Split(';');
                if (captionParts.Length > 1)
                {
                    string groupName = captionParts[0];
                    string indicatorName = captionParts[1].Trim(' ');
                    string indicatorCode = kosguDigest.GetMemberType(indicatorName);
                    string columnName = indicatorCode != String.Empty 
                        ? String.Format("{0} ({1})", indicatorName, indicatorCode)
                        : indicatorName;
                    
                    if (currentGroupName != groupName)
                    {
                        groupCell = headerLayout.AddCell(groupName);
                        currentGroupName = groupName;
                    }
                    if (groupCell != null)
                    {
                        groupCell.AddCell(columnName,
                            String.Format("Задолженность {0}", currentDateStr));
                    }
                }
            }
            
            GridBrick.GridHeaderLayout.ApplyHeaderInfo();
        }


       #endregion
        
        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
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
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);
        }

        #endregion
    }
}
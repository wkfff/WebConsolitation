using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0046_0001
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable gridDt = new DataTable();
        private string formTableName;

        private static MemberAttributesDigest periodDigest;
        private static MemberAttributesDigest formDigest;

        #endregion

        #region Свойства

        public FormTableType FormTableType
        {
            get
            {
                switch (formTableName)
                {
                    case "Таблица 1":
                    case "Таблица 8":
                    case "Таблица 9":
                        {
                            return FormTableType.AllBudgetFact;
                        }
                    case "Таблица 2.1":
                        {
                            return FormTableType.SettlementPlanFact;
                        }
                    case "Таблица 3":
                        {
                            return FormTableType.LocalBudgetFact;
                        }
                    case "Таблица 10":
                        {
                            return FormTableType.AllBudgetRegionSplit;
                        }
                    default:
                        {
                            return FormTableType.AllBudgetPlanFact;
                        }
                }
            }
        }

        public bool IsRegionSplit
        {
            get { return FormTableType == FormTableType.AllBudgetRegionSplit; }
        }

        #endregion

        #region Параметры запроса

        // выбранный период
        private CustomParam selectedPeriod;
        // код формы
        private CustomParam formNumber;
        // множество бюджетов
        private CustomParam budgetSet;
        // множество мер
        private CustomParam measureSet;

        #endregion
        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GridBrick.Height = CustomReportConst.minScreenHeight - 235;
            GridBrick.Width = CustomReportConst.minScreenWidth - 15;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.DataBound += new EventHandler(Grid_DataBound);

            #endregion

            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");
            formNumber = UserParams.CustomParam("form_number");
            budgetSet = UserParams.CustomParam("budget_set");
            measureSet = UserParams.CustomParam("measure_set");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected void Grid_DataBound(object sender, EventArgs e)
        {
            
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                ComboPeriod.Title = "Период";
                ComboPeriod.Width = 300;
                ComboForm.ParentSelect = false;
                ComboPeriod.MultiSelect = false;
                periodDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0046_0001_periodDigest");
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodDigest.UniqueNames, periodDigest.MemberLevels));
                ComboPeriod.SelectLastNode();

                ComboForm.Title = "Форма";
                ComboForm.Width = 750;
                ComboForm.MultiSelect = false;
                ComboForm.TooltipVisibility = TooltipVisibilityMode.Shown;
                formDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0046_0001_formDigest");
                ComboForm.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(formDigest.UniqueNames, formDigest.MemberLevels));
            }

            selectedPeriod.Value = periodDigest.GetMemberUniqueName(ComboPeriod.SelectedValue);
            formNumber.Value = formDigest.GetMemberType(ComboForm.SelectedValue);
            formTableName = formDigest.GetMemberUniqueName(ComboForm.SelectedValue);
            budgetSet.Value = GetBudgetSetValue();
            measureSet.Value = GetMeasureSetValue();

            Page.Title = "Результаты мониторинга местных бюджетов";
            Label1.Text = Page.Title;
            Label2.Text = GetHalfYearDateText(selectedPeriod.Value);
            gridCaptionElement.Text = String.Format("{0} ({1})", ComboForm.SelectedValue, formTableName);

            GridDataBind();
        }

        private static string GetHalfYearDateText(string uniqueName)
        {
            DateTime date = CRHelper.PeriodDayFoDate(uniqueName);

            if (uniqueName.Contains("Полугодие 2"))
            {
                return String.Format("на 01.01.{0} года", date.Year + 1);
            }
            return String.Format("на 01.07.{0} года", date.Year);
        }

        private string GetBudgetSetValue()
        {
            switch (FormTableType)
            {
                case FormTableType.AllBudgetFact:
                case FormTableType.AllBudgetPlanFact:
                    {
                        return "[Все территории]";
                    }
                case FormTableType.SettlementPlanFact:
                    {
                        return "[Послеления]";
                    }
                case FormTableType.LocalBudgetFact:
                    {
                        return "[Местные бюджеты]";
                    }
                case FormTableType.AllBudgetRegionSplit:
                    {
                        return "[Все территории для выравнивания]";
                    }
            }
            return "[Все территории]";
        }

        private string GetMeasureSetValue()
        {
            switch (FormTableType)
            {
                case FormTableType.AllBudgetFact:
                case FormTableType.LocalBudgetFact:
                    {
                        return "[мера Факт]";
                    }
                case FormTableType.AllBudgetPlanFact:
                case FormTableType.SettlementPlanFact:
                    {
                        return "[мера План и факт]";
                    }
                case FormTableType.AllBudgetRegionSplit:
                    {
                        return "[меры До выравниваия и После выравнивания]";
                    }
            }
            return "[мера Факт]";
        }

        #region Обработчики грида
        
        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText(IsRegionSplit ? "FO_0046_0001_grid_regionSplit" : "FO_0046_0001_grid");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                if (gridDt.Columns.Count > 0)
                {
                    gridDt.Columns.RemoveAt(0);
                }
                gridDt.AcceptChanges();

                foreach (DataRow row in gridDt.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = row[0].ToString().TrimEnd('_');
                    }
                    if (row[1] != DBNull.Value)
                    {
                        row[1] = TrimCode(row[1].ToString());
                    }
                }

                GridBrick.DataTable = gridDt;
            }
        }

        private static string TrimCode(string name)
        {
            for (int i = 0; i < name.Length; i++)
            {
                decimal value;
                if (!Decimal.TryParse(name[i].ToString(), out value))
                {
                    return name.Substring(i);
                }
            }
            return String.Empty;
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(50);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(230);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            
            int columnCount = e.Layout.Bands[0].Columns.Count;

            for (int i = 2; i < columnCount; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(130);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            SetHeaderLayout();
        }

        private void SetHeaderLayout()
        {
            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            headerLayout.AddCell("№ п/п");

            switch (FormTableType)
            {
                case FormTableType.AllBudgetFact:
                    {
                        headerLayout.AddCell("Наименование показателя").AddCell("1");
                        headerLayout.AddCell("Всего").AddCell("2");
                        headerLayout.AddCell("Городские округа").AddCell("3");
                        headerLayout.AddCell("Муниципальные районы").AddCell("4");
                        headerLayout.AddCell("Городские поселения").AddCell("5");
                        headerLayout.AddCell("Сельские поселения").AddCell("6");
                        headerLayout.AddCell("Внутригородские муниципальные образования").AddCell("7");

                        break;
                    }
                case FormTableType.SettlementPlanFact:
                    {
                        headerLayout.AddCell("Наименование показателя", 2).AddCell("1");
                        AddPlanFactGroup("Всего", 2);
                        AddPlanFactGroup("Городские поселения", 4);
                        AddPlanFactGroup("Сельские поселения", 6);

                        break;
                    }
                case FormTableType.LocalBudgetFact:
                    {
                        headerLayout.AddCell("Наименование показателя").AddCell("1");
                        headerLayout.AddCell("Городские округа").AddCell("2");
                        headerLayout.AddCell("Муниципальные районы").AddCell("3");
                        headerLayout.AddCell("Городские поселения").AddCell("4");
                        headerLayout.AddCell("Сельские поселения").AddCell("5");

                        break;
                    }
                case FormTableType.AllBudgetPlanFact:
                    {
                        headerLayout.AddCell("Наименование показателя", 2).AddCell("1");
                        AddPlanFactGroup("Всего", 2);
                        AddPlanFactGroup("Городские округа", 4);
                        AddPlanFactGroup("Муниципальные районы", 6);
                        AddPlanFactGroup("Городские поселения", 8);
                        AddPlanFactGroup("Сельские поселения", 10);
                        AddPlanFactGroup("Внутригородские муниципальные образования", 12);

                        break;
                    }
                case FormTableType.AllBudgetRegionSplit:
                    {
                        headerLayout.AddCell("Наименование показателя", 2).AddCell("1");
                        AddAlignmentGroup("До выравнивания", 2);
                        AddAlignmentGroup("После выравнивания", 9);
                        break;
                    }
            }

            GridBrick.GridHeaderLayout.ApplyHeaderInfo();
        }

        private void AddPlanFactGroup(string groupName, int columnIndex)
        {
            GridHeaderCell groupCell = GridBrick.GridHeaderLayout.AddCell(groupName);
            groupCell.AddCell("План").AddCell(columnIndex.ToString());
            groupCell.AddCell("Факт").AddCell((columnIndex + 1).ToString());
        }

        private void AddAlignmentGroup(string groupName, int columnIndex)
        {
            GridHeaderCell groupCell = GridBrick.GridHeaderLayout.AddCell(groupName);
            groupCell.AddCell("Городской округ").AddCell(columnIndex.ToString());
            groupCell.AddCell("Муниципальный район").AddCell((columnIndex + 1).ToString());
            groupCell.AddCell("Городское поселение").AddCell((columnIndex + 2).ToString());
            groupCell.AddCell("Сельское поселение").AddCell((columnIndex + 3).ToString());
            groupCell.AddCell("Внутригородское МО").AddCell((columnIndex + 4).ToString());
            groupCell.AddCell("В целом по муниципальным образованиям").AddCell((columnIndex + 5).ToString());
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

    public enum FormTableType
    {
        // по всем бюджетам, факт
        AllBudgetFact,
        // по всем бюджетам, факт и план
        AllBudgetPlanFact,
        // по поселениям, факт и план
        SettlementPlanFact,
        // по местным бюджетам, факт и план
        LocalBudgetFact,
        // по всем бюджетам с расщеплением по районам (с мерами выравнивания)
        AllBudgetRegionSplit
    }

}
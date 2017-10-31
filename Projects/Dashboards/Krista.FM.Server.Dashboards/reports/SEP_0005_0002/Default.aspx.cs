using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.WebUI.UltraWebNavigator;

namespace Krista.FM.Server.Dashboards.reports.SEP_0005_0002
{
    public partial class Default : CustomReportPage
    {

        #region Поля

        private DataTable gridDt;

        private static MemberAttributesDigest periodDigest;
        private static MemberAttributesDigest sphereDigest;

        private static int columnWidth = 100;

        #endregion

        #region Параметры запроса

        private CustomParam selectedPeriod;
        private CustomParam selectedYear;
        private CustomParam sphere;

        #endregion

        private static int Resolution
        {
            get { return CRHelper.GetScreenWidth; }
        }

        private static int Height
        {
            get { return CRHelper.GetScreenHeight; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            // Установка размеров
            if (Resolution < 900)
            {
                UltraWebGrid.Width = Unit.Parse("780px");
            }
            else if (Resolution < 1200)
            {
                UltraWebGrid.Width = Unit.Parse("950px");
            }
            else
            {
                UltraWebGrid.Width = Unit.Parse("1260px");
            }

            UltraWebGrid.RedNegativeColoring = false;
            UltraWebGrid.Height = Unit.Parse(String.Format("570px"));
            UltraWebGrid.Grid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.Grid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            
            #endregion

            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");
            selectedYear = UserParams.CustomParam("selected_year");
            sphere = UserParams.CustomParam("sphere");

            #endregion
            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            if (!Page.IsPostBack)
            {
                ComboPeriod.Title = "Выберите год";
                ComboPeriod.ParentSelect = false;
                ComboPeriod.MultiSelect = false;
                ComboPeriod.Width = 300;
                periodDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "SEP_0005_0002_date");
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodDigest.UniqueNames, periodDigest.MemberLevels));
                ComboPeriod.SelectLastNode();

                ComboSphere.Title = "Сфера";
                ComboSphere.ParentSelect = true;
                ComboSphere.MultiSelect = true;
                ComboSphere.MultipleSelectionType = Components.MultipleSelectionType.CascadeMultiple;
                ComboSphere.Width = 350;
                sphereDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "SEP_0005_0002_indicators");
                ComboSphere.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(sphereDigest.UniqueNames, sphereDigest.MemberLevels));
                ComboSphere.SetСheckedState("Все", true);
            }

            if (periodDigest.GetMemberType(ComboPeriod.SelectedValue) == "Год")
            {
                selectedYear.Value = periodDigest.GetShortName(ComboPeriod.SelectedValue).Replace(" год", String.Empty);
            }
            else
            {
                selectedPeriod.Value = periodDigest.GetMemberUniqueName(ComboPeriod.SelectedValue);
            }

            sphere.Value = String.Empty;
            foreach (Node node in ComboSphere.GetLastNode(0).Nodes)
            {
                if (node.Parent.Checked && node.Checked)
                {
                    if (String.IsNullOrEmpty(sphere.Value))
                    {
                        sphere.Value = String.Format("{0} + {0}.Children", sphereDigest.GetMemberUniqueName(node.Text));
                    }
                    else
                    {
                        sphere.Value += String.Format(" + {0} + {0}.Children", sphereDigest.GetMemberUniqueName(node.Text));
                    }
                }
            }
            //CRHelper.SaveToUserAgentLog(sphere.Value);

            Page.Title = String.Format("Итоги социально-экономического развития ХМАО-Югры");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("Мониторинг показателей на основе формы «Итоги социально-экономического развития», Ханты-Мансийский автономный округ – Югра, за {0}", ComboPeriod.SelectedValue.ToLower());

            GridDataBind();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query;
            if (periodDigest.GetMemberType(ComboPeriod.SelectedValue) == "Год")
                query = DataProvider.GetQueryText("SEP_0005_0002_grid_year");
            else
                query = DataProvider.GetQueryText("SEP_0005_0002_grid_month");
            gridDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатели", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                UltraWebGrid.DataTable = gridDt;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];

            if (band.Columns.Count == 0)
            {
                return;
            }

            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowDeleteDefault = AllowDelete.No;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            band.Columns[0].CellStyle.Wrap = true;
            band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            band.Columns[1].CellStyle.Wrap = true;
            band.Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            band.Columns[0].Width = Unit.Parse("350px");
            band.Columns[1].Width = Unit.Parse(String.Format("{0}px", columnWidth));
            for (int i = 2; i < band.Columns.Count; ++i)
            {
                band.Columns[i].Width = Unit.Parse(String.Format("{0}px", columnWidth));
                if (band.Columns[i].Key.Contains("темп роста"))
                    CRHelper.FormatNumberColumn(band.Columns[i], "P2");
                else
                    CRHelper.FormatNumberColumn(band.Columns[i], "N2");
            }

            if (periodDigest.GetMemberType(ComboPeriod.SelectedValue) == "Год")
            {
                int year = Convert.ToInt32(periodDigest.GetShortName(ComboPeriod.SelectedValue).Replace(" год", String.Empty));

                GridHeaderLayout headerLayout = UltraWebGrid.GridHeaderLayout;

                headerLayout.AddCell("Показатели");
                headerLayout.AddCell("Единица измерения");
                GridHeaderCell cell = headerLayout.AddCell("Россия");
                cell.AddCell(String.Format("{0} год", year - 2));
                cell.AddCell(String.Format("Темп роста {0} к {1} году, %", year - 2, year - 3));
                cell.AddCell(String.Format("{0} год", year - 1));
                cell.AddCell(String.Format("Темп роста {0} к {1} году, %", year - 1, year - 2));
                cell.AddCell(String.Format("{0} год", year));
                cell.AddCell(String.Format("Темп роста {0} к {1} году, %", year, year - 1));
                cell = headerLayout.AddCell("Ханты-Мансийский автономный округ - Югра");
                cell.AddCell(String.Format("{0} год", year - 2));
                cell.AddCell(String.Format("Темп роста {0} к {1} году, %", year - 2, year - 3));
                cell.AddCell(String.Format("{0} год", year - 1));
                cell.AddCell(String.Format("Темп роста {0} к {1} году, %", year - 1, year - 2));
                cell.AddCell(String.Format("{0} год", year));
                cell.AddCell(String.Format("Темп роста {0} к {1} году, %", year, year - 1));

                headerLayout.ApplyHeaderInfo();
            }
            else
            {
                int monthNum = CRHelper.MonthNum(periodDigest.GetShortName(ComboPeriod.SelectedValue).Replace(" год", String.Empty));

                int year = Convert.ToInt32(ComboPeriod.SelectedValue.Split(' ')[1]);

                GridHeaderLayout headerLayout = UltraWebGrid.GridHeaderLayout;

                headerLayout.AddCell("Показатели");
                headerLayout.AddCell("Единица измерения");
                
                GridHeaderCell cell = headerLayout.AddCell("Россия");
                cell.AddCell(String.Format("январь-{0} {1}&nbsp;г", CRHelper.RusMonth(monthNum), year - 1));
                cell.AddCell(String.Format("Темп роста января-{0} {1}&nbsp;г к январю-{2} {3}&nbsp;г, %", CRHelper.RusMonthGenitive(monthNum), year - 1, CRHelper.RusMonthDat(monthNum), year - 2));
                cell.AddCell(String.Format("январь-{0} {1}&nbsp;г", CRHelper.RusMonth(monthNum), year));
                cell.AddCell(String.Format("Темп роста января-{0} {1}&nbsp;г к январю-{2} {3}&nbsp;г, %", CRHelper.RusMonthGenitive(monthNum), year, CRHelper.RusMonthDat(monthNum), year - 1));
                cell.AddCell(String.Format("оценка {0}&nbsp;г", year));
                cell.AddCell(String.Format("Темп роста {0}&nbsp;г к {1}&nbsp;г, %", year, year - 1));
                
                cell = headerLayout.AddCell("Ханты-Мансийский автономный округ - Югра");
                cell.AddCell(String.Format("январь-{0} {1}&nbsp;г", CRHelper.RusMonth(monthNum), year - 1));
                cell.AddCell(String.Format("Темп роста января-{0} {1}&nbsp;г к январю-{2} {3}&nbsp;г, %", CRHelper.RusMonthGenitive(monthNum), year - 1, CRHelper.RusMonthDat(monthNum), year - 2));
                cell.AddCell(String.Format("{0}&nbsp;г", year - 1));
                cell.AddCell(String.Format("Темп роста {0}&nbsp;г к {1}&nbsp;г, %", year - 1, year - 2));
                cell.AddCell(String.Format("январь-{0} {1}&nbsp;г", CRHelper.RusMonth(monthNum), year));
                cell.AddCell(String.Format("Темп роста января-{0} {1}&nbsp;г к январю-{2} {3}&nbsp;г, %", CRHelper.RusMonthGenitive(monthNum), year, CRHelper.RusMonthDat(monthNum), year - 1));
                cell.AddCell(String.Format("оценка {0}&nbsp;г", year));
                cell.AddCell(String.Format("Темп роста {0}&nbsp;г к {1}&nbsp;г, %", year, year - 1));

                headerLayout.ApplyHeaderInfo();

            }

        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[1].GetText() == "Уровень 1")
            {
                e.Row.Style.Font.Bold = true;
                e.Row.Cells[0].ColSpan = e.Row.Cells.Count;
            }
        }

        #endregion
        
        #region Экспорт в Excel

        private void RemoveTags()
        {
            for (int i = 0; i < UltraWebGrid.Grid.Columns.Count; i++)
            {
                foreach (UltraGridRow row in UltraWebGrid.Grid.Rows)
                {
                    UltraGridCell cell = row.Cells[i];
                    if (cell.Value != null)
                    {
                        cell.Value = cell.Value.ToString().Replace("&gt;", String.Empty);
                        cell.Value = Regex.Replace(cell.Value.ToString(), "<[^>]*?>", String.Empty);
                    }
                }
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            RemoveTags();

            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            ReportExcelExporter1.SheetColumnCount = UltraWebGrid.Grid.Columns.Count;
            ReportExcelExporter1.GridColumnWidthScale = 1;

            for (int i = 0; i < UltraWebGrid.Grid.Rows.Count; i++)
            {
                if (UltraWebGrid.Grid.Rows[i].Cells[1].GetText() == "Уровень 1")
                {
                    for (int j = 1; j < UltraWebGrid.Grid.Columns.Count; ++j)
                        UltraWebGrid.Grid.Rows[i].Cells[j].Value = null;
                }
            }

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(UltraWebGrid.GridHeaderLayout, String.Empty, sheet1, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            RemoveTags();

            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            for (int i = 0; i < UltraWebGrid.Grid.Rows.Count; i++)
            {
                if (UltraWebGrid.Grid.Rows[i].Cells[1].GetText() == "Уровень 1")
                {
                    for (int j = 1; j < UltraWebGrid.Grid.Columns.Count; ++j)
                        UltraWebGrid.Grid.Rows[i].Cells[j].Value = null;
                }
            }

            Report report = new Report();
            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(UltraWebGrid.GridHeaderLayout, String.Empty, section1);
        }

        #endregion
    }
}
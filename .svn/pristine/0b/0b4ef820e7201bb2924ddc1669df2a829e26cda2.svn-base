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

namespace Krista.FM.Server.Dashboards.reports.FNS_0008_0002
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable gridDt = new DataTable();
        private DateTime currentDate;
        private int firstYear = 2005;
        private int baseYear;

        // недоимка именительный падеж
        private string nomStr;
        private string genStr;

        private MemberAttributesDigest fnsKdDigest;
        private MemberAttributesDigest normativeKdDigest;
        private MemberAttributesDigest okvedDigest;
        
        #endregion

        #region Параметры запроса

        // выбранный код доходов для ФНС
        private CustomParam selectedFnsKd;
        // выбранный код доходов для норматива
        private CustomParam selectedNormativeKd;
        // выбранный базовый год
        private CustomParam selectedBaseYear;
        // выбранный ОКВЭД
        private CustomParam selectedOkved;
        // список кодов доходов для норматива
        private CustomParam normativeKdList;

        private CustomParam rubMultiplier;
        // показатель
        private CustomParam index;
        // код налогоплательщика
        private CustomParam taxPayer;

        #endregion


        private static MemberAttributesDigest indexDigest;
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

            #region Настройка грида

            GridBrick.AutoSizeStyle = GridAutoSizeStyle.None;
            GridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight - 270);
            GridBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 20);
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            
            #endregion

            #region Инициализация параметров запроса

            selectedFnsKd = UserParams.CustomParam("selected_fns_kd");
            selectedNormativeKd = UserParams.CustomParam("selected_normative_kd");
            selectedBaseYear = UserParams.CustomParam("selected_base_year");
            selectedOkved = UserParams.CustomParam("selected_okved");
            normativeKdList = UserParams.CustomParam("normative_kd_list");
            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            index = UserParams.CustomParam("index");
            taxPayer = UserParams.CustomParam("tax_payer");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            normativeKdList.Value = "True";
            normativeKdDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FNS_0008_0002_fnsKdDigest");

            normativeKdList.Value = "False";
            fnsKdDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FNS_0008_0002_fnsKdDigest");

            okvedDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FNS_0008_0002_okvedDigest");

            if (!Page.IsPostBack)
            {
                DateTime lastDate = CubeInfoHelper.Fns28nNonSplitInfo.LastDate;

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastDate.Year));
                ComboYear.SetСheckedState(lastDate.Year.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(lastDate.Month)), true);

                ComboKD.Title = "Вид дохода";
                ComboKD.Width = 350;
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = false;
                ComboKD.TooltipVisibility = TooltipVisibilityMode.Shown;
                ComboKD.AllowSelectionType = AllowedSelectionType.LeafNodes;
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(fnsKdDigest.UniqueNames, fnsKdDigest.MemberLevels));
                ComboKD.SetСheckedState("НДФЛ ", true);

                ComboOKVED.Title = "Вид деятельности";
                ComboOKVED.Width = 250;
                ComboOKVED.MultiSelect = false;
                ComboOKVED.ParentSelect = true;
                ComboOKVED.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(okvedDigest.UniqueNames, okvedDigest.MemberLevels));
                ComboOKVED.SetСheckedState("Все коды ОКВЭД", true);

                ComboBaseYear.Title = "Базовый год";
                ComboBaseYear.Width = 150;
                ComboBaseYear.MultiSelect = false;
                ComboBaseYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastDate.Year));
                ComboBaseYear.SetСheckedState(lastDate.Year.ToString(), true);

                indexDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FNS_0001_0001_Digest");
                ComboIndex.Title = "Показатели";
                ComboIndex.Width = 400;
                ComboIndex.MultiSelect = false;
                ComboIndex.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(indexDigest.UniqueNames, indexDigest.MemberLevels));
                ComboIndex.SetСheckedState("Недоимка, неурегулиров. задолж. Всего", true);
            }

            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), ComboMonth.SelectedIndex + 1, 1);
            baseYear = Convert.ToInt32(ComboBaseYear.SelectedValue);

            selectedFnsKd.Value = fnsKdDigest.GetMemberUniqueName(ComboKD.SelectedValue);
            selectedNormativeKd.Value = normativeKdDigest.GetMemberUniqueName(ComboKD.SelectedValue);
            selectedBaseYear.Value = baseYear.ToString();
            selectedOkved.Value = okvedDigest.GetMemberUniqueName(ComboOKVED.SelectedValue);

            UserParams.PeriodLastYear.Value = (currentDate.Year - 1).ToString();
            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));

            rubMultiplier.Value = IsThsRubSelected ? "1000" : "1000000";
            index.Value = indexDigest.GetMemberUniqueName(ComboIndex.SelectedValue);
            switch (ComboIndex.SelectedIndex)
            {
                case 0:
                    {
                        taxPayer.Value = "[ФНС_28н_Коды налогоплательщиков].[Основные налогоплательщики ]";
                        nomStr = "едоимка, неурегулированная задолженность";
                        genStr = "едоимки, неурегулированной задолженности";
                        break;
                    }
                case 1:
                    {
                        taxPayer.Value = "[ФНС_28н_Коды налогоплательщиков].[Основные налогоплательщики ]";
                        nomStr = "едоимка";
                        genStr = "едоимки";
                        break;
                    }
                case 2:
                    {
                        taxPayer.Value = "[ФНС_28н_Коды налогоплательщиков].[Основные налогоплательщики ]";
                        nomStr = "еурегулированная задолженность";
                        genStr = "еурегулированной задолженности";
                        break;
                    }
                case 3:
                    {
                        taxPayer.Value = " [ФНС_28н_Коды налогоплательщиков].[Все налогоплательщики]";
                        nomStr = "едоимка, неурегулированная задолженность";
                        genStr = "едоимки, неурегулированной задолженности";
                        break;
                    }
                case 4:
                    {
                        taxPayer.Value = " [ФНС_28н_Коды налогоплательщиков].[Все налогоплательщики]";
                        nomStr = "едоимка";
                        genStr = "едоимки";
                        break;
                    }
                case 5:
                    {
                        taxPayer.Value = " [ФНС_28н_Коды налогоплательщиков].[Все налогоплательщики]";
                        nomStr = "еурегулированная задолженность";
                        genStr = "еурегулированной задолженности";
                        break;
                    }
            }
            string nameIndex = indexDigest.GetFullName(ComboIndex.SelectedValue);
            Page.Title = String.Format("Анализ {0} и поступлений в бюджеты в сопоставимых условиях (с учетом нормативов отчислений доходов)", nameIndex);
            Label1.Text = Page.Title;
            Label2.Text = String.Format("по состоянию на {0:dd.MM.yyy}, {1}, {2}, базовый год {3}", currentDate.AddMonths(1), ComboKD.SelectedValue.TrimEnd(), ComboOKVED.SelectedValue, baseYear);

            GridDataBind();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FNS_0008_0002_grid");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                FontRowLevelRule fontRule = new FontRowLevelRule(gridDt.Columns.Count - 1);
                fontRule.AddFontLevel("0", GridBrick.BoldFont8pt);
                fontRule.AddFontLevel("1", GridBrick.BoldFont8pt);
                GridBrick.AddIndicatorRule(fontRule);

                PaddingRule paddingRule = new PaddingRule(0, "Уровень", 10);
                GridBrick.AddIndicatorRule(paddingRule);
                
                GridBrick.DataTable = gridDt;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            
            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                string columnName = e.Layout.Bands[0].Columns[i].Header.Caption;

                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], GetColumnFormat(columnName));
            }

            string appgPeriod = String.Format("на {0:dd.MM.yyy} г.", currentDate.AddYears(-1).AddMonths(1));
            string beginYearPeriod = String.Format("на начало {0} г.", currentDate.Year);
            string currPeriod = String.Format("на {0:dd.MM.yyy} г.", currentDate.AddMonths(1));

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;
            headerLayout.AddCell("Бюджет");

            GridHeaderCell groupCell = headerLayout.AddCell(string.Format("Н{0} в контингенте", nomStr));
            groupCell.AddCell(appgPeriod, String.Format("Н{1} по территории в контингенте {0}", appgPeriod, nomStr));
            groupCell.AddCell(beginYearPeriod, String.Format("Н{1} по территории в контингенте {0}", beginYearPeriod, nomStr));
            groupCell.AddCell(currPeriod, String.Format("Н{1} по территории в контингенте {0}", currPeriod, nomStr));

            headerLayout.AddCell(String.Format("Норматив отчисления доходов (на {0} г.)", baseYear), "Норматив отчисления доходов базового года");

            groupCell = headerLayout.AddCell(string.Format("Н{0} с учетом норматива отчислений", nomStr));
            groupCell.AddCell(appgPeriod, String.Format("Н{1} с учетом норматива отчислений {0}", appgPeriod, nomStr));
            groupCell.AddCell(beginYearPeriod, String.Format("Н{1} с учетом норматива отчислений {0}", beginYearPeriod, nomStr));
            groupCell.AddCell(currPeriod, String.Format("Н{1} с учетом норматива отчислений {0}", currPeriod, nomStr));

            groupCell = headerLayout.AddCell(string.Format("Прирост (снижение) н{0} с начала года", genStr));
            groupCell.AddCell(String.Format("в {0}", RubMultiplierCaption), String.Format("Прирост (снижение) н{1} с начала года в {0}", RubMultiplierCaption, genStr));
            groupCell.AddCell("в %", string.Format("Прирост (снижение) н{0} с начала года в %", genStr));

            groupCell = headerLayout.AddCell(string.Format("Прирост (снижение) н{0} к аналогичному периоду прошлого года", genStr));
            groupCell.AddCell(String.Format("в {0}", RubMultiplierCaption), String.Format("Прирост (снижение) н{1} к аналогичному периоду прошлого года в {0}", RubMultiplierCaption, genStr));
            groupCell.AddCell("в %", string.Format("Прирост (снижение) н{0} к аналогичному периоду прошлого года в %", genStr));

            headerLayout.AddCell(String.Format("Поступления {0} в контингенте", currPeriod), String.Format("Поступления {0} в контингенте", currPeriod));
            headerLayout.AddCell(String.Format("Поступления {0} с учетом нормативов отчислений", currPeriod), String.Format("Поступления {0} с учетом нормативов отчислений", currPeriod));
            headerLayout.AddCell(String.Format("Удельный вес н{1} в поступлениях {0} с учетом нормативов отчислений", currPeriod, genStr), String.Format("Удельный вес н{1} в поступлениях {0} с учетом нормативов отчислений", currPeriod, genStr));

            headerLayout.ApplyHeaderInfo();
        }

        private static string GetColumnFormat(string columnName)
        {
            columnName = columnName.ToLower();
            if (columnName.Contains("удельный вес") || columnName.Contains("%") || columnName.Contains("% исполнения"))
            {
                return "P1";
            }
            if (columnName.Contains("норматив отчисления доходов"))
            {
                return "P2";
            }
            return "N1";
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            GridBrick.Grid.DisplayLayout.SelectedRows.Clear(); 
            
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
            ReportPDFExporter1.HeaderCellHeight = 80;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);
        }

        #endregion
    }
}
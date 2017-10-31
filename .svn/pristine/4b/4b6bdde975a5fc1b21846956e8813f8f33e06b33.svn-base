using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Web.UI.WebControls;
using System.Xml;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FNS_0008_0001
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable gridDt = new DataTable();
        private DateTime currentDate;
        private int firstYear = 2009;
        private DateTime fnsLastDateAPPG;
        private DateTime asBudgetLastDateAPPG;

        #endregion

        #region Параметры запроса

        // последняя дата в кубе ФНС
        private CustomParam fnsAPPGPeriod;
        // последняя дата в кубе АС Бюджет
        private CustomParam asBudgetAPPGPeriod;
        // секция объявления лукапов мемберов
        private CustomParam lookupMemberDeclaration;
        // множество лукапов мемберов
        private CustomParam lookupMemberSet;
        // секция объявления показателей ОКВЭД
        private CustomParam okvedMemberDeclaration;
        // множество показателей ОКВЭД
        private CustomParam okvedMemberSet;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            fnsAPPGPeriod = UserParams.CustomParam("fns_appg_period");
            asBudgetAPPGPeriod = UserParams.CustomParam("as_budget_appg_period");
            lookupMemberDeclaration = UserParams.CustomParam("lookup_member_declaration");
            okvedMemberDeclaration = UserParams.CustomParam("okved_member_declaration");
            lookupMemberSet = UserParams.CustomParam("lookup_member_set");
            okvedMemberSet = UserParams.CustomParam("okved_member_set");

            #endregion

            #region Настройка грида

            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.9 - 220);
            GridBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 20);
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                DateTime lastDate = CubeInfoHelper.Fns28nSplitInfo.LastDate;

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastDate.Year));
                ComboYear.SetСheckedState(lastDate.Year.ToString(), true);
            }

            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), 1, 1);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            fnsLastDateAPPG = CubeInfo.GetLastDate(DataProvidersFactory.PrimaryMASDataProvider, "FNS_0008_0001_fnsLastDateAPPG");
            asBudgetLastDateAPPG = CubeInfo.GetLastDate(DataProvidersFactory.PrimaryMASDataProvider, "FNS_0008_0001_asBudgetLastDateAPPG");

            fnsAPPGPeriod.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период]", fnsLastDateAPPG, 4);
            asBudgetAPPGPeriod.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период]", asBudgetLastDateAPPG, 5);

            Page.Title = String.Format("Сравнительный анализ динамики поступления налоговых и неналоговых доходов бюджета Новосибирской области и динамики бюджетного финансирования по отдельным отраслям экономической деятельности");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("Данные за {1} - {2} годы по состоянию на {0:dd.MM.yyy}, тыс.руб.", asBudgetLastDateAPPG.AddYears(1),
                currentDate.Year - 2, currentDate.Year);

            GenerateQueryParams();

            GridDataBind();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FNS_0008_0001_grid");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                GridBrick.AddIndicatorRule(new GrowRateRule(String.Format("Темп роста доходов в {0} г.", currentDate.Year)));
                GridBrick.AddIndicatorRule(new GrowRateRule(String.Format("Темп роста доходов в {0} г.", currentDate.Year - 1)));

                AddInverseGrowRateIndicator(String.Format("Темп роста бюджетного финансирования за {0} г.", currentDate.Year));
                AddInverseGrowRateIndicator(String.Format("Темп роста бюджетного финансирования за {0} г.", currentDate.Year - 1));

                AddPlusGrowRateIndicator(String.Format("Сравнение темпов роста доходов и бюджетного финансирования", currentDate.Year - 1));
                AddPlusGrowRateIndicator(String.Format("Сравнение темпов роста доходов и бюджетного финансирования ", currentDate.Year));

                GridBrick.DataTable = gridDt;
            }
        }

        private void AddInverseGrowRateIndicator(string columnName)
        {
            GrowRateRule rateRule = new GrowRateRule(columnName);
            rateRule.IncreaseImg = "~/images/arrowRedUpBB.png";
            rateRule.DecreaseImg = "~/images/arrowGreenDownBB.png";
            GridBrick.AddIndicatorRule(rateRule);
        }
        
        private void AddPlusGrowRateIndicator(string columnName)
        {
            GrowRateRule rateRule = new GrowRateRule(columnName);
            rateRule.IncreaseImg = "~/images/plusGreenBB.png";
            rateRule.IncreaseText = "Темп роста доходов превышает темп роста бюджетного финансирования";
            rateRule.DecreaseImg = "~/images/plusRedBB.png";
            rateRule.DecreaseText = "Темп роста бюджетного финансирования превышает темп роста доходов";
            GridBrick.AddIndicatorRule(rateRule);
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;

            for (int i = 1; i < columnCount; i = i + 1)
            {
                string columnName = e.Layout.Bands[0].Columns[i].Header.Caption;

                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], GetColumnFormat(columnName));
            }

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            headerLayout.AddCell("Отрасль", "Отрасли экономической деятельности");
            headerLayout.AddCell(String.Format("Доходы за {0} г.", currentDate.Year - 2), 
                String.Format("Фактическое исполнение по налоговым и неналоговым доходам за {0} г.", currentDate.Year - 2));
            headerLayout.AddCell(String.Format("Доходы за {0} г.", currentDate.Year - 1),
                String.Format("Фактическое исполнение по налоговым и неналоговым доходам за {0} г.", currentDate.Year - 1));
            headerLayout.AddCell(String.Format("Темп роста доходов в {0} г.", currentDate.Year - 1),
                "Темп роста фактического исполнения по доходам первого отчетного года к предыдущему");
            headerLayout.AddCell(String.Format("Бюджетное финансирование отрасли за {0} г.", currentDate.Year - 2),
                String.Format("Бюджетное финансирование по отраслям за {0} г.", currentDate.Year - 2));
            headerLayout.AddCell(String.Format("Бюджетное финансирование отрасли за {0} г.", currentDate.Year - 1),
                String.Format("Бюджетное финансирование по отраслям за {0} г.", currentDate.Year - 1));
            headerLayout.AddCell(String.Format("Темп роста бюджетного финансирования за {0} г.", currentDate.Year - 1),
                "Темп роста финансирования первого отчетного года к предыдущему году");
            headerLayout.AddCell(String.Format("Сравнение темпов роста доходов и бюджетного финансирования"),
                "Отношение темпа роста фактического исполнения по доходам к темпу роста бюджетного финансирования в предыдущем году");

            headerLayout.AddCell(String.Format("Доходы по состоянию на {0:dd.MM.yyyy} г.", fnsLastDateAPPG.AddMonths(1)),
                "Фактическое исполнение нарастающим итогом с начала года");
            headerLayout.AddCell(String.Format("Доходы по состоянию на {0:dd.MM.yyyy} г.", fnsLastDateAPPG.AddYears(1).AddMonths(1)), 
                "Фактическое исполнение по налоговым и неналоговым доходам за текущий год (данные приводятся на последний месяц, на который есть данные, нарастающим итогом с начала года)");

            headerLayout.AddCell(String.Format("Темп роста доходов в {0} г.", currentDate.Year), 
                "Темп роста фактического поступления доходов за текущий год");

            headerLayout.AddCell(String.Format("Бюджетное финансирование отрасли по состоянию на {0:dd.MM.yyyy} г.", asBudgetLastDateAPPG),
                "Бюджетное финансирование за аналогичный период предыдущего года");
            headerLayout.AddCell(String.Format("Бюджетное финансирование отрасли по состоянию на {0:dd.MM.yyyy} г.", asBudgetLastDateAPPG.AddYears(1)),
                "Бюджетное финансирование за текущий год, (данные приводятся на последний месяц, за который есть данные, нарастающим итогом с начала года)");

            headerLayout.AddCell(String.Format("Темп роста бюджетного финансирования за {0} г.", currentDate.Year),
                "Темп роста бюджетного финансирования в текущем году по отношению к предыдущему году");
            headerLayout.AddCell(String.Format("Сравнение темпов роста доходов и бюджетного финансирования "),
                "Отношение темпа роста доходов к темпу роста бюджетного финансирования в текущем году");

            headerLayout.ApplyHeaderInfo();
        }

        private static string GetColumnFormat(string columnName)
        {
            columnName = columnName.ToLower();
            if (columnName.Contains("темп роста") || columnName.Contains("сравнение темпов роста"))
            {
                return "P1";
            }
            return "N1";
        }

        #endregion

        #region Генерация запроса

        private Collection<string> baseOutcomesSet = new Collection<string>();
        private Collection<string> adminSet = new Collection<string>();
        private Collection<string> kosguSet = new Collection<string>();
        private Collection<string> okvedSet = new Collection<string>();

        private Collection<string> lookupMemberCollection = new Collection<string>();
        private Collection<string> okvedMemberCollection = new Collection<string>();

        private void GenerateQueryParams()
        {
            lookupMemberDeclaration.Value = String.Empty;
            lookupMemberSet.Value = String.Empty;
            okvedMemberDeclaration.Value = String.Empty;
            okvedMemberSet.Value = String.Empty;
            lookupMemberCollection.Clear();
            okvedMemberCollection.Clear();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(RegionSettingsHelper.GetReportConfigFullName());
            XmlNode root = GetXmlSettingsRoot(xmlDoc, "ReportIndicators");

            if (root != null)
            {
                foreach (XmlNode indicator in root.ChildNodes)
                {
                    string indicatorName = GetStringAttrValue(indicator, "name", String.Empty);

                    baseOutcomesSet.Clear();
                    adminSet.Clear();
                    kosguSet.Clear();
                    okvedSet.Clear();

                    GenerateMembers(indicator);

                    lookupMemberDeclaration.Value += String.Format("\n{0}", GenerateLookupMember(indicatorName));
                    okvedMemberDeclaration.Value += String.Format("\n{0}", GenerateOkvedMember(indicatorName));

                    lookupMemberCollection.Add(String.Format("[Measures].[{0} ]", indicatorName));
                    okvedMemberCollection.Add(String.Format("[ОКВЭД__Сопоставимый].[ОКВЭД__Сопоставимый].[{0} ]", indicatorName));
                }
                
                lookupMemberSet.Value = SetToString(lookupMemberCollection, String.Empty);
                okvedMemberSet.Value = SetToString(okvedMemberCollection, String.Empty);
            }
        }

        private static XmlNode GetXmlSettingsRoot(XmlDocument xmlDoc, string rootName)
        {
            foreach (XmlNode node in xmlDoc.ChildNodes)
            {
                if (node.Name.Equals(rootName))
                {
                    return node;
                }
            }
            return null;
        }

        private void GenerateMembers(XmlNode rootNode)
        {
            foreach (XmlNode indicatorNode in rootNode.ChildNodes)
            {
                string filterDimension = indicatorNode.Name;
 
                foreach (XmlNode codeNode in indicatorNode.ChildNodes)
                {
                    string code = codeNode.InnerText;
                    if (!String.IsNullOrEmpty(codeNode.InnerText))
                    {
                        switch (filterDimension)
                        {
                            case "Расходы.Базовый":
                                {
                                    baseOutcomesSet.Add(GetDimensionElement(CubeInfoHelper.BudgetOutocmesFactInfo, filterDimension, code));
                                    break;
                                }
                            case "Администратор.Сопоставим":
                                {
                                    adminSet.Add(GetDimensionElement(CubeInfoHelper.BudgetOutocmesFactInfo, filterDimension, code));
                                    break;
                                }
                            case "КОСГУ.Сопоставимый":
                                {
                                    kosguSet.Add(GetDimensionElement(CubeInfoHelper.BudgetOutocmesFactInfo, filterDimension, code));
                                    break;
                                }
                            case "ОКВЭД.Сопоставимый":
                                {
                                    okvedSet.Add(GetDimensionElement(CubeInfoHelper.Fns28nSplitInfo, filterDimension, code));
                                    break;
                                }
                        }
                    }
                }
            }
        }

        private static string GetDimensionElement(CubeInfo cubeInfo, string filterDimension, string code)
        {
            string element = cubeInfo.GetDimensionElement(filterDimension, code);
            if (element == String.Empty)
            {
                CRHelper.SaveToErrorLog(String.Format("В измерении {0} не найден элемент с кодом '{1}'", filterDimension, code));
            }
            return element;
        }

        private string GenerateLookupMember(string memberName)
        {
            return String.Format(@"
                    member [Measures].[{0} ]
                    as ' 
                    LookupCube  
                    (
                        ""[ФО_АС Бюджет_КазнИсп_Факт расхода]"",
                        ""  
                        (
                            Aggregate  
                            (
                                {1},
                                Aggregate  
                                (
                                    {2},
                                    Aggregate  
                                    (
                                        {3},
                                        (
                                            [Тип средств__Сопоставимый].[Тип средств__Сопоставимый].[Все типы средств].[Бюджетные средства],
                                            [Measures].[Расход с возвратом нараст итог],
                                            "" + MemberToStr  
                                            (
                                                [Период__Период].[Период__Период].CurrentMember  
                                            ) + "" 
                                        )  
                                    )  
                                )  
                            )  
                        )""  
                    ) / 1000'", memberName,
                              SetToString(baseOutcomesSet, "[Расходы__Базовый].[Расходы__Базовый].DefaultMember").Replace("\"", "\"\""),
                              SetToString(adminSet, "[Администратор__Сопоставим].[Администратор__Сопоставим].DefaultMember").Replace("\"", "\"\""),
                              SetToString(kosguSet, "[КОСГУ__Сопоставимый].[КОСГУ__Сопоставимый].DefaultMember").Replace("\"", "\"\""));
        }

        private string GenerateOkvedMember(string memberName)
        {
            return String.Format(@"
                    member [ОКВЭД__Сопоставимый].[ОКВЭД__Сопоставимый].[{0} ]
                    as ' 
                    Aggregate    
                    (
                       {1}  
                    )'", memberName, SetToString(okvedSet, "[ОКВЭД__Сопоставимый].[ОКВЭД__Сопоставимый].DefaultMember"));
        }

        private static string SetToString(Collection<string> collection, string defaultMember)
        {
            string set = String.Empty;

            if (collection.Count == 0)
            {
                set = defaultMember;
            }
            else
            {
                foreach (string item in collection)
                {
                    if (item != String.Empty)
                    {
                        set += String.Format("{0},", item);
                    }
                }
            }

            return String.Format("{{{0}}}", set.TrimEnd(','));
        }

        public static string GetStringAttrValue(XmlNode xn, string attrName, string defaultValue)
        {
            try
            {
                if (xn == null || xn.Attributes == null)
                {
                    return defaultValue;
                }
                if (xn.Attributes.GetNamedItem(attrName) == null)
                {
                    return defaultValue;
                }

                string value = xn.Attributes[attrName].Value;
                if (value != String.Empty)
                {
                    return value;
                }
            }
            catch
            {
                return defaultValue;
            }
            return defaultValue;

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
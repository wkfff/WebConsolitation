using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core.MemberDigests;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0003
{
    public partial class DefaultDetail : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";
        private string multiplierCaption;

        #region Параметры запроса

        private CustomParam selectedRegion;
        // доходы Итого
        private CustomParam incomesTotal;
        // уровень консолидированного бюджета
        private CustomParam regionsConsolidateLevel;
        // тип документа СКИФ для консолидированного уровня
        private CustomParam consolidateDocumentSKIFType;
        // тип документа СКИФ для уровня районов
        private CustomParam regionDocumentSKIFType;
        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;
        // уровень МО
        private CustomParam regionsLevel;
        // выбранный множитель рублей
        private CustomParam rubMultiplier;

        // тип документа СКИФ для местных бюджетов
        private CustomParam localBudgetDocumentSKIFType;
        // уровень бюджета СКИФ для местных бюджетов
        private CustomParam localBudgetSKIFLevel;

        #endregion

        private MemberAttributesDigest budgetDigest;

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }
            if (incomesTotal == null)
            {
                incomesTotal = UserParams.CustomParam("incomes_total");
            }
            if (regionsConsolidateLevel == null)
            {
                regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            }
            if (consolidateDocumentSKIFType == null)
            {
                consolidateDocumentSKIFType = UserParams.CustomParam("consolidate_document_skif_type");
            }
            if (regionDocumentSKIFType == null)
            {
                regionDocumentSKIFType = UserParams.CustomParam("region_document_skif_type");
            }
            if (regionBudgetSKIFLevel == null)
            {
                regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            }
            
            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            regionsLevel = UserParams.CustomParam("regions_level");

            localBudgetDocumentSKIFType = UserParams.CustomParam("local_budget_document_skif_type");
            localBudgetSKIFLevel = UserParams.CustomParam("local_budget_skif_level");

            #endregion

            multiplierCaption = IsThsRubSelected ? "тыс.руб." : "млн.руб.";
            rubMultiplier.Value = IsThsRubSelected ? "1000" : "1000000";

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 160);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 7);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            CrossLink1.Text = "Информация&nbsp;об&nbsp;исполнении&nbsp;собственных&nbsp;доходов";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0011_01/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.IncomesKDSubSectionLevel.Value = RegionSettingsHelper.Instance.IncomesKDSubSectionLevel;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            UserParams.KDInternalCircualtionExtruding.Value = RegionSettingsHelper.Instance.KDInternalCircualtionExtruding;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            UserParams.RegionsLocalBudgetLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionsLocalBudgetLevel");

            firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));

            budgetDigest = MemberDigestHelper.Instance.LocalBudgetDigest;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0003_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = month;

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(UserParams.PeriodYear.Value, true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(UserParams.PeriodMonth.Value, true);
                
                ComboRegion.Title = "Бюджет";
                ComboRegion.Width = 400;
                ComboRegion.MultiSelect = false;
                ComboRegion.ParentSelect = true;
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(budgetDigest.UniqueNames, budgetDigest.MemberLevels));
                ComboRegion.SetСheckedState("Консолидированный бюджет субъекта", true);
            }
            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);

            Page.Title = string.Format("Доходы ({0})", ComboRegion.SelectedValue);
            Label1.Text = Page.Title;
            Label2.Text = string.Format("Информация за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            switch (ComboRegion.SelectedValue)
            {
                case "Консолидированный бюджет субъекта":
                    {
                        selectedRegion.Value = String.Format("{0}.[Консолидированный бюджет субъекта ]", RegionSettingsHelper.Instance.RegionDimension);
                        break;
                    }
                case "Муниципальные районы":
                    {
                        selectedRegion.Value = String.Format("{0}.[Муниципальные районы ]", RegionSettingsHelper.Instance.RegionDimension);
                        break;
                    }
                case "Городские округа":
                    {
                        selectedRegion.Value = String.Format("{0}.[Городские округа ]", RegionSettingsHelper.Instance.RegionDimension);
                        break;
                    }
                default:
                    {
                        selectedRegion.Value = budgetDigest.GetMemberUniqueName(ComboRegion.SelectedValue);
                        break;
                    }
            }

            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodEndYear.Value = (yearNum - 2).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;

            consolidateDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateDocumentSKIFType");
            regionDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionDocumentSKIFType");
            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");

            localBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("LocalBudgetDocumentSKIFType");
            localBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("LocalBudgetSKIFLevel");

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0003_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = dtGrid;
            }
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            //            UltraWebGrid.Height = Unit.Empty;
            //            UltraWebGrid.Width = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }
            
            e.Layout.Bands[0].HeaderStyle.Wrap = true;
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[9].Hidden = true;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            int lastYearNum = Convert.ToInt32(ComboYear.SelectedValue) - 1;
            int lastLastYearNum = Convert.ToInt32(ComboYear.SelectedValue) - 2;

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "КД", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Код", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, 
                String.Format("Назначено, {0}", multiplierCaption), "Плановые назначения на год");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, 
                String.Format("Исполнено, {0}", multiplierCaption), "Фактическое исполнение нарастающим итогом с начала года");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "Исполнено %", "Процент выполнения назначений/ Оценка равномерности исполнения (1/12 годового плана в месяц)");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "Средний % исполнения",
                string.Format("Средний % исполнения за {0} {1} {2} и {3} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), lastLastYearNum, lastYearNum));
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, "% исп. – отклонение от среднего",
                string.Format("Отклонение от среднего % исполнения за {0} {1} {2} и {3} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), lastLastYearNum, lastYearNum));
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7, 
                String.Format("Исполнено прошлый год, {0}", multiplierCaption), "Исполнено за аналогичный период прошлого года");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8, "Темп роста к прошлому году", "Темп роста исполнения к аналогичному периоду прошлого года");

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = (i == 0)
                                                                             ?
                                                                         HorizontalAlign.Left
                                                                             :
                                                                         HorizontalAlign.Right;
                double width;
                switch (i)
                {
                    case 0:
                        {
                            width = 345;
                            break;
                        }
                    case 2:
                    case 3:
                    case 7:
                        {
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                            width = 105;
                            break;
                        }
                    case 4:
                    case 5:
                    case 6:
                    case 8:
                        {
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                            width = 81;
                            break;
                        }
                    default:
                        {
                            width = 120;
                            e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
                            break;
                        }
                }
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(width);
            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 1)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            ColumnHeader ch = new ColumnHeader(true);
            ch.Caption = string.Format("За {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 2;
            ch.RowLayoutColumnInfo.SpanX = 5;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = "Сравнение с прошлым годом";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 7;
            ch.RowLayoutColumnInfo.SpanX = 2;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool complete = (i == 4);
                bool growRate = (i == 8);
                bool deviation = (i == 6);
                int levelColumn = 9;

                if (complete && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
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
                }

                if (growRate && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "Рост к прошлому году";
                    }
                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "Снижение к прошлому году";
                    }
                }

                if (deviation && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "% исполнения выше среднего";
                    }
                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "% исполнения ниже среднего";
                    }
                }

                if (e.Row.Cells[levelColumn] != null && e.Row.Cells[levelColumn].Value.ToString() != string.Empty && i != 1)
                {
                    string level = e.Row.Cells[levelColumn].Value.ToString();
                    int fontSize = 8;
                    bool bold = false;
                    bool italic = false;
                    switch (level)
                    {
                        case "Группа":
                            {
                                fontSize = 10;
                                bold = true;
                                italic = false;
                                break;
                            }
                        case "Подгруппа":
                            {
                                fontSize = 10;
                                bold = false;
                                italic = true;
                                break;
                            }
                        case "Статья":
                            {
                                fontSize = 8;
                                bold = false;
                                italic = false;
                                break;
                            }
                    }
                    e.Row.Cells[i].Style.Font.Size = fontSize;
                    e.Row.Cells[i].Style.Font.Bold = bold;
                    e.Row.Cells[i].Style.Font.Italic = italic;
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
                }

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

        #endregion

        #region Экспорт в Pdf

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            //title = e.Section.AddText();
            //font = new Font("Verdana", 14);
            //title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            //title.AddContent(Label2.Text);
        }
        
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.GridElementCaption = Label2.Text;
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;

            for (int i = 0; i < columnCount; i++)
            {
                switch (i)
                {
                    case 0:
                        {
                            e.CurrentWorksheet.Columns[i].Width = 200*37;
                            break;
                        }
                    case 2:
                    case 3:
                    case 7:
                        {
                            e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00";
                            e.CurrentWorksheet.Columns[i].Width = 100 * 37;
                            break;
                        }
                    case 4:
                    case 5:
                    case 6:
                    case 8:
                        {
                            e.CurrentWorksheet.Columns[i].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
                            e.CurrentWorksheet.Columns[i].Width = 81 * 37;
                            break;
                        }
                    default:
                        {
                            e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#0";
                            e.CurrentWorksheet.Columns[i].Width = 120 * 37;
                            break;
                        }
                }
            }
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);

            switch (e.CurrentColumnIndex)
            {
                case 0:
                    {
                        e.HeaderText = "КД";
                        break;
                    }
                case 1:
                    {
                        e.HeaderText = "Код";
                        break;
                    }
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    {
                        e.HeaderText = string.Format("За {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);
                        break;
                    }
                case 7:
                case 8:
                    {
                        e.HeaderText = "Сравнение с прошлым годом";
                        break;
                    }
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        #endregion
    }
}

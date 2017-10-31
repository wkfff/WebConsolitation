using System;
using System.Collections.Generic;
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

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0013_01_HMAO
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable avgDt;
        private int firstYear = 2008;
        private int endYear = 2011;

        private Dictionary<string, double> avgValues;

        #endregion

        #region Параметры запроса

        // Уровень районов
        private CustomParam regionsLevel;
        // консолидированный бюджет субъекта
        private CustomParam regionsConsolidateBudget;
        // доходы всего
        private CustomParam incomesAll;
        // расходы всего
        private CustomParam outcomesAll;

        // тип документа СКИФ для консолидированного бюджета субъекта
        private CustomParam consolidateBudgetDocumentSKIFType;
        // тип документа СКИФ для районов
        private CustomParam regionBudgetDocumentSKIFType;

        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;

        #endregion

        private double rubMultiplier;

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string RubMultiplierCaption
        {
            get { return IsThsRubSelected ? "Тыс.руб." : "Млн.руб."; }
        }
        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.85 - 120);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров запроса

            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }
            if (regionsConsolidateBudget == null)
            {
                regionsConsolidateBudget = UserParams.CustomParam("regions_consolidate_budget");
            }
            if (incomesAll == null)
            {
                incomesAll = UserParams.CustomParam("incomes_all");
            }
            if (outcomesAll == null)
            {
                outcomesAll = UserParams.CustomParam("outcomes_all");
            }

            if (consolidateBudgetDocumentSKIFType == null)
            {
                consolidateBudgetDocumentSKIFType = UserParams.CustomParam("consolidate_budget_document_skif_type");
            }
            if (regionBudgetDocumentSKIFType == null)
            {
                regionBudgetDocumentSKIFType = UserParams.CustomParam("region_budget_document_skif_type");
            }

            if (regionBudgetSKIFLevel == null)
            {
                regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            }

            #endregion
            
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.FKRAllLevel.Value = RegionSettingsHelper.Instance.FKRAllLevel;
            UserParams.FKRSectionLevel.Value = RegionSettingsHelper.Instance.FKRSectionLevel;
            UserParams.FKRSubSectionLevel.Value = RegionSettingsHelper.Instance.FKRSubSectionLevel;
            UserParams.EKRDimension.Value = RegionSettingsHelper.Instance.EKRDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0013_01_HMAO_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(dtDate.Rows[0][3].ToString(), true);
            }

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            regionsConsolidateBudget.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            incomesAll.Value = RegionSettingsHelper.Instance.IncomeTotal;
            outcomesAll.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;

            consolidateBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");

            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");

            Page.Title = "Исполнение бюджета в разрезе отдельных показателей";
            PageTitle.Text = Page.Title;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            PageSubTitle.Text = string.Format("Информация за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString(); 
            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;

            rubMultiplier = IsThsRubSelected ? 1000 : 1000000;

            AVGDataBind();

            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0013_01_HMAO_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    for (int i = 1; i < row.ItemArray.Length - 1; i++)
                    {
                        // номер в тройке
                        int j = (i - 1) % 3;
                        if ((j == 0 || j == 1) && row[i] != DBNull.Value)
                        {
                            row[i] = Convert.ToDouble(row[i]) / rubMultiplier;
                        }
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 1)
            {
                string formatString = "P2";
                int widthColumn = 80;

               // номер в тройке
                int j = (i - 1) % 3;
                switch (j)
                {
                    case 0:
                    case 1:
                        {
                            formatString = "N2";
                            widthColumn = 100;
                            break;
                        }
                    case 2:
                        {
                            formatString = "P2";
                            widthColumn = 85;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = 220;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 1;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 3)
            {
                ColumnHeader ch = new ColumnHeader(true);

                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                ch.Caption = captions[0];

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, String.Format("Уточненные бюджетные назначения, {0}", RubMultiplierCaption.ToLower()), "Плановые назначения на год");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, String.Format("Исполнено, {0}", RubMultiplierCaption.ToLower()), "Фактическое исполнение нарастающим итогом с начала года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, "Исполнено %", "Процент выполнения назначений/ Оценка равномерности исполнения (1/12 годового плана в месяц)");

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 3;
                ch.RowLayoutColumnInfo.SpanX = i == e.Layout.Bands[0].Columns.Count - 3 ? 2 : 3;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
 
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count - 1; i++)
            {
                int j = (i - 1) % 3;
                
                bool complete = (j == 2);

                if (complete)
                {
                    string kdGroupName = string.Empty;
                    string[] nameParts = dtGrid.Columns[i].ColumnName.Split(';');
                    if (nameParts.Length > 1)
                    {
                        kdGroupName = nameParts[0];
                    }

                    double avgValue = GetAvgValue(kdGroupName);

                    if (avgValue != Int32.MinValue && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        int value = Convert.ToInt32(100 * Convert.ToDouble(e.Row.Cells[i].Value));

                        if (value < avgValue)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerRed.gif";
                            e.Row.Cells[i].Title = string.Format("Ниже среднего исполнения по субъекту РФ ({0:N2}%)", avgValue);
                        }
                        else if (value > avgValue)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerGreen.gif";
                            e.Row.Cells[i].Title = string.Format("Выше среднего исполнения по субъекту РФ ({0:N2}%)", avgValue);
                        }
                        e.Row.Cells[i].Style.Padding.Right = 10;
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; margin: 2px";
                    }
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

                    if (e.Row.Cells[0].Value != null && 
                        (e.Row.Cells[0].Value.ToString().ToLower().Contains("бюджет") || 
                         e.Row.Cells[0].Value.ToString().ToLower().Contains("городские округа") ||
                         e.Row.Cells[0].Value.ToString().ToLower().Contains("муниципальные районы") ||
                         e.Row.Cells[0].Value.ToString().ToLower().Contains("среднее значение")))
                    {
                        cell.Style.Font.Bold = true;
                    }
                }
            }
        }

        #endregion

        #region Среднее значение процента исполнения

        protected void AVGDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0013_01_HMAO_avg");
            avgDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Среднее", avgDt);

            avgValues = new Dictionary<string, double>();

            if (avgDt.Rows.Count > 0)
            {
                foreach (DataColumn column in avgDt.Columns)
                {
                    double avgValue = GetDoubleDTValue(avgDt, column.ColumnName);
                    avgValues.Add(column.ColumnName.Split(';')[0], avgValue);
                }
            }
        }

        private static double GetDoubleDTValue(DataTable dt, string columnName)
        {
            if (dt != null && dt.Rows.Count > 0 &&
                dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != String.Empty)
            {
                return Convert.ToDouble(100 * Convert.ToDouble(dt.Rows[0][columnName]));
            }
            return Double.MinValue;
        }

        private double GetAvgValue(string kdGroupName)
        {
            if (avgValues != null && avgValues.ContainsKey(kdGroupName))
            {
                return avgValues[kdGroupName];
            }

            return Double.MinValue;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count - 1;

            int width = 101;

            for (int i = 1; i < columnCount; i++)
            {
                string formatString = "P2";

                // номер в тройке
                int j = (i - 1) % 3;
                switch (j)
                {
                    case 0:
                    case 1:
                        {
                            formatString = UltraGridExporter.ExelNumericFormat;
                            break;
                        }
                    case 2:
                        {
                            formatString = UltraGridExporter.ExelPercentFormat;
                            break;
                        }
                }
                e.CurrentWorksheet.Columns[i].Width = width * 37;
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(115);
            }

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {

        }

        #endregion
    }
}

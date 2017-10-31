using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0039_0001_Kostroma
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtIndicatorDescription;
        private int firstYear = 2010;
        private int endYear = 2011;
        private static Dictionary<string, string> indicatorDescripitonList;
        private static Dictionary<string, string> indicatorExportDescripitonList;
        private static Dictionary<string, string> indicatorUnitList;
        private static Dictionary<string, string> indicatorNameList;
        private static Dictionary<string, string> indicatorDirectionList;

        #endregion

        public bool ValueSelected
        {
            get { return MeasureButtonList.SelectedIndex == 1; }
        }

        private static bool IsMozilla
        {
            get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
        }

        #region Параметры запроса

        // выбранная мера
        private CustomParam selectedMeasure;
        // выбранный период
        private CustomParam selectedPeriod;
        // уровень районов
        private CustomParam regionsLevel;

        #endregion

        private GridHeaderLayout headerLayout;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            double scale = IsMozilla ? 0.65 : 0.55;

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * scale);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров запроса

            if (selectedMeasure == null)
            {
                selectedMeasure = UserParams.CustomParam("selected_maasure");
            }
            if (selectedPeriod == null)
            {
                selectedPeriod = UserParams.CustomParam("selected_period");
            }
            regionsLevel = UserParams.CustomParam("regions_level");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Динамика&nbsp;результатов&nbsp;оценки";
            CrossLink1.NavigateUrl = "~/reports/FO_0039_0002_Kostroma/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Рейтинг&nbsp;МР(ГО)";
            CrossLink2.NavigateUrl = "~/reports/FO_0039_0003_Kostroma/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Картограмма";
            CrossLink3.NavigateUrl = "~/reports/FO_0039_0005_Kostroma/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "Диаграмма&nbsp;динамики&nbsp;по&nbsp;отд.показателю";
            CrossLink4.NavigateUrl = "~/reports/FO_0039_0007_Kostroma/Default.aspx";

            CrossLink5.Visible = true;
            CrossLink5.Text = "Диаграмма&nbsp;динамики&nbsp;результатов&nbsp;оценки";
            CrossLink5.NavigateUrl = "~/reports/FO_0039_0006_Kostroma/Default.aspx";

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
                
            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0039_0001_Kostroma_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                string quarter = "Квартал 4";
                if (dtDate.Rows.Count > 0)
                {
                    endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                    quarter = dtDate.Rows[0][2].ToString();
                }

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboQuarter.Title = "Оценка качества";
                ComboQuarter.Width = 300;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillDateQuarters());
                ComboQuarter.SetСheckedState(GetParamQuarter(quarter), true);
            }

            Page.Title = String.Format("Результат оценки качества организации и осуществления бюджетного процесса в МР(ГО) Костромской области");
            PageTitle.Text = Page.Title;

            int quarterNum = ComboQuarter.SelectedIndex + 1;

            PageSubTitle.Text = (quarterNum != 4) 
                ? String.Format("{0}.{1}", ComboQuarter.SelectedValue, ComboYear.SelectedValue) 
                : String.Format("по итогам {0} года", ComboYear.SelectedValue);

            selectedMeasure.Value = ValueSelected ? "Значение" : "Оценка показателя";

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(quarterNum));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", quarterNum);

            selectedPeriod.Value = (quarterNum != 4)
                ? String.Format("[{0}].[{1}].[{2}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value)
                : String.Format("[{0}]", UserParams.PeriodYear.Value);

            IndicatorDescriptionDataBind();

            headerLayout = new GridHeaderLayout(UltraWebGrid);

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        /// <summary>
        /// Получить элемент параметра по значению классификатора
        /// </summary>
        /// <param name="classQuarter">элемент классификатора</param>
        /// <returns>значение параметра</returns>
        private static string GetParamQuarter(string classQuarter)
        {
            switch (classQuarter)
            {
                case "Квартал 1":
                    {
                        return "по состоянию на 01.04";
                    }
                case "Квартал 2":
                    {
                        return "по состоянию на 01.07";
                    }
                case "Квартал 3":
                    {
                        return "по состоянию на 01.10";
                    }
                case "Квартал 4":
                case "Данные года":
                    {
                        return "по итогам года";
                    }
                default:
                    {
                        return classQuarter;
                    }
            }
        }

        #region Обработчики грида

        private void IndicatorDescriptionDataBind()
        {
            indicatorUnitList = new Dictionary<string, string>();
            indicatorNameList = new Dictionary<string, string>();
            indicatorDirectionList = new Dictionary<string, string>();

            string query = DataProvider.GetQueryText("FO_0039_0001_Kostroma_indicatorDescription");
            dtIndicatorDescription = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtIndicatorDescription);

            foreach (DataRow row in dtIndicatorDescription.Rows)
            {
                string code = row[0].ToString();
                string name = row[1].ToString();
                string direction = row[2].ToString();
                string unit = ValueSelected ? row[3].ToString() : "Баллы";

                indicatorUnitList.Add(code, unit);
                indicatorNameList.Add(code, name);
                indicatorDirectionList.Add(code, direction);
            }
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0039_0001_Kostroma_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);
            if (dtGrid.Rows.Count > 0 && dtGrid.Columns.Count > 2)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = row[0].ToString().Replace("район", "р-н");
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            if (e.Layout.Bands[0].Columns.Count < 2)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("Ранг") || 
                    e.Layout.Bands[0].Columns[i].Header.Caption.Contains("Степерь качества")) ? "N0" : "N2";
                int widthColumn = (!ValueSelected && i > e.Layout.Bands[0].Columns.Count - 4) ? 150 : 250;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            int lastIndicatorIndex = ValueSelected ? e.Layout.Bands[0].Columns.Count : e.Layout.Bands[0].Columns.Count - 3;
           
            string measureCaption = ValueSelected ? "Значение" : "Оценка";

            string currentCaption = String.Empty;
            GridHeaderCell headerCell = new GridHeaderCell();

            headerLayout.AddCell("Наименование муниципальных образований Костромской области");
            for (int i = 1; i < lastIndicatorIndex; i = i + 1)
            {
                string caption = e.Layout.Bands[0].Columns[i].Header.Caption;
                if (caption != String.Empty)
                {
                    string headerCaption = indicatorDirectionList[caption];

                    string indicatorCode = caption;
                    string indicatorName = indicatorNameList[indicatorCode];
                    String indicatorUnit = indicatorUnitList[indicatorCode];

                    if (currentCaption != headerCaption)
                    {
                        headerCell = headerLayout.AddCell(headerCaption);
                        currentCaption = headerCaption;
                    }

                    GridHeaderCell indicatorCell = headerCell.AddCell(String.Format("«{0}» {1}", indicatorCode, indicatorName));
                    indicatorCell.AddCell(measureCaption, indicatorUnit);
                }
            }

            for (int i = lastIndicatorIndex; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption);
            }

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != String.Empty)
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

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {

        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            ReportExcelExporter1.HeaderCellHeight = 20;
            ReportExcelExporter1.Export(headerLayout, 3);
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            ReportPDFExporter1.HeaderCellHeight = 70;
            ReportPDFExporter1.Export(headerLayout);
        }

        #endregion
    }
}

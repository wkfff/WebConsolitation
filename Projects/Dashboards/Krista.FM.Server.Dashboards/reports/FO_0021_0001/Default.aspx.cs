using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0021_0001
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtIndicatorDescription;
        private int firstYear = 2009;
        private int endYear = 2012;
        private int selectedQuarterIndex;
        private int selectedYear;

        private static Dictionary<string, string> indicatorNameList;

        private GridHeaderLayout headerLayout;

        #endregion

        #region Параметры запроса

        // выбранный период
        private CustomParam selectedPeriod;
        // фильтр по направлению
        private CustomParam directionFilter;
        // выбранная мера
        private CustomParam selectedMeasure;

        #endregion

        private MeasureType MeasureType
        {
            get
            {
                switch (MeasureButtonList.SelectedIndex)
                {
                    case 0:
                        {
                            return MeasureType.Density;
                        }
                    case 1:
                        {
                            return MeasureType.Evaluation;
                        }
                    default:
                        {
                            return MeasureType.Value;
                        }
                }
            }
        }

        private bool IsYearEvaluaitonSelected
        {
            get
            {
                return selectedQuarterIndex == 4;
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            double scale = 0.9;

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * scale - 220);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");
            directionFilter = UserParams.CustomParam("direction_filter");
            selectedMeasure = UserParams.CustomParam("selected_measure");
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            CrossLink1.Visible = true;
            CrossLink1.Text = "Рейтинг&nbsp;МО";
            CrossLink1.NavigateUrl = "~/reports/FO_0021_0002/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Динамика&nbsp;результатов&nbsp;оценки&nbsp;качества";
            CrossLink2.NavigateUrl = "~/reports/FO_0021_0003/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Картограмма&nbsp;результатов&nbsp;оценки&nbsp;качества";
            CrossLink3.NavigateUrl = "~/reports/FO_0021_0004/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "Диаграмма&nbsp;динамики&nbsp;оценки&nbsp;качества";
            CrossLink4.NavigateUrl = "~/reports/FO_0021_0005/Default.aspx";

            MeasureButtonList.Items[0].Text = "<span title='Оценка индикатора умноженная на его вес относительной значимости (в методике это (MixWi)). Различия в величинах весов обусловлены разной степенью влияния отражаемых индикаторами факторов на общий уровень финансового положения и качество управления финансами'>Удельный вес</span>";
            MeasureButtonList.Items[1].Text = "<span title='Оценка полученного значения (в методике это Mi - оценка по индикатору, рассчитанное на основе Vi и критических значений индикатора)'>Оценка</span>";
            MeasureButtonList.Items[2].Text = "<span title='Непосредственно считается значение по формуле (в методике это расчетное значения каждого индикатора - Vi)'>Значение</span>";
        }
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0021_0001_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string quarter = dtDate.Rows[0][2].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
                
                ComboQuarter.Title = "Квартал";
                ComboQuarter.Width = 300;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillEvaluaitonQuarters());
                ComboQuarter.SetСheckedState(GetParamQuarter(quarter), true);
            }
            
            selectedYear = Convert.ToInt32(ComboYear.SelectedValue);
            selectedQuarterIndex = ComboQuarter.SelectedIndex + 2;

            Page.Title = String.Format("Результаты оценки качества управления финансами муниципальных образований Вологодской области");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = GetQuarterText(selectedQuarterIndex);

            UserParams.PeriodYear.Value = selectedYear.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(selectedQuarterIndex));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", selectedQuarterIndex);

            selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", selectedYear, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);
            directionFilter.Value = IsYearEvaluaitonSelected ? "true" : "Measures.[Учитывать квартальный]";

            switch (MeasureType)
            {
                case MeasureType.Density:
                    {
                        selectedMeasure.Value = "Взвешенное значение";
                        break;
                    }
                case MeasureType.Evaluation:
                    {
                        selectedMeasure.Value = "Оценка индикатора";
                        break;
                    }
                case MeasureType.Value:
                    {
                        selectedMeasure.Value = "Значение";
                        break;
                    }
            }

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
                case "Квартал 2":
                    {
                        return "Оценка качетва на 01.07";
                    }
                case "Квартал 3":
                    {
                        return "Оценка качества на 01.10";
                    }
                case "Квартал 4":
                    {
                        return "Оценка качества по итогам года";
                    }
                default:
                    {
                        return classQuarter;
                    }
            }
        }

        private string GetQuarterText(int selectedQuarter)
        {
            switch (selectedQuarter)
            {
                case 2:
                    {
                        return String.Format("Оценка качества по состоянию на 01.07.{0}", selectedYear);
                    }
                case 3:
                    {
                        return String.Format("Оценка качества по состоянию на 01.10.{0}", selectedYear);
                    }
                case 4:
                    {
                        return String.Format("Оценка качества по итогам {0} года", selectedYear);
                    }
            }
            return String.Empty;
        }

        #region Обработчики грида
        
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0021_0001_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtGrid);
            if (dtGrid.Rows.Count > 0 && dtGrid.Columns.Count > 1)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = row[0].ToString().Replace("муниципальное образование", "МО");
                        row[0] = row[0].ToString().Replace("муниципальный район", "МР");
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }
        
        private string GetIndicatorFormatString(string indicatorName)
        {
            switch(indicatorName)
            {
                case "V15":
                case "V23":
                case "V24":
                case "V25":
                case "V26":
                case "V27":
                case "V28":
                case "V29":
                case "V30":
                case "V31":
                case "V32":
                case "V33":
                    {
                        return (MeasureType == MeasureType.Density) ? "N3" : "N0";
                    }
                case "Итоговая оценка":
                case "Средняя оценка":
                    {
                        return "N1";
                    }
                case "Рейтинг":
                    {
                        return "N0";
                    }
                default:
                    {
                        return "N3";
                    }
            }
        }

        private static int GetIndicatorColumnWidth(string indicatorName)
        {
            switch (indicatorName)
            {
                case "V1":
                    {
                        return 180;
                    }
                case "V15":
                case "V20":
                case "V21":
                case "V22":
                case "V23":
                case "V24":
                case "V31":
                case "V33":
                case "V34":
                case "V35":
                case "V36":
                    {
                        return 230;
                    }
                case "Итого баллов":
                case "Несопоставленные данные":
                case "Итоговая оценка":
                case "Рейтинг":
                case "Средняя оценка":
                    {
                        return 90;
                    }
                case "V19":
                    {
                        return 200;
                    }
                default:
                    {
                        return 120;
                    }
            }
        }

        private bool IsCorrectoinIndicator(string indicatorName)
        {
            return MeasureType == MeasureType.Density && (indicatorName == "V34" || indicatorName == "V35" || indicatorName == "V36");
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(120);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string indicatorName = e.Layout.Bands[0].Columns[i].Header.Caption;
                string formatString = GetIndicatorFormatString(indicatorName);
                int widthColumn = GetIndicatorColumnWidth(indicatorName);

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);
            GridHeaderCell correctionGroupCell = null;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string indicatorCode = e.Layout.Bands[0].Columns[i].Header.Caption;
                string indicatorName = indicatorCode;
                if (indicatorNameList.ContainsKey(indicatorCode))
                {
                    indicatorName = indicatorNameList[indicatorCode];
                }

                if (IsCorrectoinIndicator(indicatorCode))
                {
                    if (correctionGroupCell == null)
                    {
                        correctionGroupCell = headerLayout.AddCell("Корректировка итоговой оценки");
                    }

                    correctionGroupCell.AddCell(indicatorName);
                }
                else
                {
                    headerLayout.AddCell(indicatorName);
                }
            }

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            string rowName = String.Empty;
            if (e.Row.Cells[0].Value != null)
            {
                rowName = e.Row.Cells[0].Value.ToString();
            }

            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
                cell.Title = rowName;
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

        #region Показатели

        private void IndicatorDescriptionDataBind()
        {
            indicatorNameList = new Dictionary<string, string>();

            string query = DataProvider.GetQueryText("FO_0021_0001_indicatorDescription");
            dtIndicatorDescription = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtIndicatorDescription);

            foreach (DataRow row in dtIndicatorDescription.Rows)
            {
                string code = row[0].ToString();
                string name = row[1].ToString();

                indicatorNameList.Add(code, name);
            }
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            ReportPDFExporter1.HeaderCellHeight = (IsYearEvaluaitonSelected && MeasureType == MeasureType.Density) ? 50 : 100;
            ReportPDFExporter1.Export(headerLayout);
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            //ReportExcelExporter1.HeaderCellHeight = (IsYearEvaluaitonSelected && MeasureType == MeasureType.Density) ? 50 : 100;
            ReportExcelExporter1.Export(headerLayout, 3);
        }

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {

        }
        
        #endregion
    }

    public enum MeasureType
    {
        Density,
        Value,
        Evaluation
    }
}
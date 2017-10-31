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

namespace Krista.FM.Server.Dashboards.reports.FO_0021_0001_Saratov
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtIndicatorDescription;
        private int firstYear = 2010;
        private int endYear = 2012;
        private int selectedQuarterIndex;
        private int selectedYear;

        private static Dictionary<string, string> indicatorNameList;

        private GridHeaderLayout headerLayout;

        #endregion

        #region Параметры запроса

        // выбранный период
        private CustomParam selectedPeriod;
        // множество индикаторв
        private CustomParam indicatorSet;
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
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 1.4);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");
            indicatorSet = UserParams.CustomParam("indicator_set");
            selectedMeasure = UserParams.CustomParam("selected_measure");
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            CrossLink1.Visible = true;
            CrossLink1.Text = "Рейтинг&nbsp;МО";
            CrossLink1.NavigateUrl = "~/reports/FO_0021_0002_Saratov/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Динамика&nbsp;результатов&nbsp;оценки&nbsp;качества";
            CrossLink2.NavigateUrl = "~/reports/FO_0021_0003_Saratov/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Диаграмма&nbsp;динамики&nbsp;оценки&nbsp;качества";
            CrossLink3.NavigateUrl = "~/reports/FO_0021_0005_Saratov/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "Картограмма&nbsp;результатов&nbsp;оценки&nbsp;качества";
            CrossLink4.NavigateUrl = "~/reports/FO_0021_0004_Saratov/Default.aspx";

        }
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0021_0001_Saratov_date");
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
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillEvaluaitonQuarters(true));
                ComboQuarter.SetСheckedState(GetParamQuarter(quarter), true);
            }
            
            selectedYear = Convert.ToInt32(ComboYear.SelectedValue);
            selectedQuarterIndex = ComboQuarter.SelectedIndex + 1;

            Page.Title = String.Format("Результаты оценки качества управления финансами муниципальных образований Саратовской области");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = GetQuarterText(selectedQuarterIndex);

            UserParams.PeriodYear.Value = selectedYear.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(selectedQuarterIndex));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", selectedQuarterIndex);

            selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", selectedYear, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);

            switch (MeasureType)
            {
                case MeasureType.Density:
                    {
                        selectedMeasure.Value = "Взвешенное значение";
                        indicatorSet.Value = IsYearEvaluaitonSelected ? "Показатели для удельного веса": "Показатели для удельного веса, квартал" ;
                        break;
                    }
                case MeasureType.Evaluation:
                    {
                        selectedMeasure.Value = "Оценка индикатора";
                        indicatorSet.Value = IsYearEvaluaitonSelected ? "Показатели для оценки": "Показатели для оценки, квартал";
                        break;
                    }
                case MeasureType.Value:
                    {
                        selectedMeasure.Value = "Значение";
                        indicatorSet.Value = IsYearEvaluaitonSelected ? "Показатели для значения" : "Показатели для значения, квартал";
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
                case "Квартал 1":
                    {
                        return "Оценка качетва на 01.04";
                    }
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
                case 1:
                    {
                        return String.Format("Оценка качества по состоянию на 01.04.{0}", selectedYear);
                    }
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
            string query = DataProvider.GetQueryText("FO_0021_0001_Saratov_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtGrid);
            if (dtGrid.Rows.Count > 0)
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
                case "P1":
                case "P2":
                case "P9":
                case "P10":
                case "P11":
                case "P12":
                case "P14":
                case "P16":
                case "P17":
                case "P18":
                case "P19":
                case "P21":
                    {
                        return "N3";
                    }
                case "P3":
                case "P4":
                case "P5":
                case "P6":
                case "P7":
                case "P8":
                case "P13":
                case "P15":
                case "P20":
                    {
                        return (MeasureType == MeasureType.Value) ? "N3" : "N0";
                    }
                case "Итоговая оценка":
                case "Средняя оценка":
                    {
                        return "N1";
                    }
                case "Ранг":
                case "Группа (МО)":
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
                
                case "P9":
                case "P10":
                case "P11":
                case "P12":
                case "P14":
                case "P16":
                case "P17":
                case "P18":
                case "P19":
                case "P21":
                case "P13":
                case "P15":
                case "P20":
                    {
                        return 200;
                    }
                case "P1":
                case "P2":
                case "P3":
                case "P4":
                case "P5":
                case "P6":
                case "P7":
                case "P8":
            
                    {
                        return  100;
                    }
              
                case "Несопоставленные данные":
                case "Итоговая оценка":
                case "Ранг":
                case "Средняя оценка":
                case "Группа (МО)":
                    {
                        return 90;
                    }
                default:
                    {
                        return 150;
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

            if (e.Layout.Bands[0].Columns.Count < 2)
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
                cell.Title = string.Format("{0}", e.Row.Cells[0].Value);
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

            ReportExcelExporter1.HeaderCellHeight = (IsYearEvaluaitonSelected && MeasureType == MeasureType.Density) ? 50 : 100;
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
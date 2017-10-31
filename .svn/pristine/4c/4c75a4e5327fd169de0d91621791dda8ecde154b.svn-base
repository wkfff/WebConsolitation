using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0042_0004_HMAO
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtIndicatorDescription;
        private int firstYear = 2009;
        private int endYear = 2012;
        private static Dictionary<string, string> indicatorUnitList;
        private GridHeaderLayout headerLayout;
        private string prevPeriodCaption;
        private string currPeriodCaption;
        private string currMeasureCaption;

        #endregion

        public bool ValueSelected
        {
            get { return MeasureButtonList.SelectedIndex == 1; }
        }

        #region Параметры запроса

        // выбранная мера
        private CustomParam selectedMeasure;
        // множество показателей
        private CustomParam indicatorSet;
        // выбранный период
        private CustomParam selectedPeriod;
        // выбранный предыдущий период
        private CustomParam selectedPrevPeriod;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            double scale = 0.6;
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * scale);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            #region Инициализация параметров запроса

            selectedMeasure = UserParams.CustomParam("selected_maasure");
            indicatorSet = UserParams.CustomParam("indicator_set");
            selectedPeriod = UserParams.CustomParam("selected_period");
            selectedPrevPeriod = UserParams.CustomParam("selected_prev_period");

            #endregion

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;оценки&nbsp;качества";
            CrossLink1.NavigateUrl = "~/reports/FO_0042_0001_HMAO/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Рейтинг&nbsp;ГРБС";
            CrossLink2.NavigateUrl = "~/reports/FO_0042_0002_HMAO/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0042_0004_HMAO_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string quarter = dtDate.Rows[0][2].ToString();

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

            int quarterNum = ComboQuarter.SelectedIndex + 1;
            int currentYear = Convert.ToInt32(ComboYear.SelectedValue);

            Page.Title = String.Format("Динамика результатов мониторинга качества финансового менеджмента, осуществляемого главными распорядителями средств бюджета автономного округа, главными администраторами доходов бюджета ХМАО-Югры");
            PageTitle.Text = Page.Title;

            PageSubTitle.Text = (quarterNum != 4)
                ? String.Format("{0}.{1}", ComboQuarter.SelectedValue, ComboYear.SelectedValue)
                : String.Format("по итогам {0} года", ComboYear.SelectedValue);
            
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(quarterNum));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", quarterNum);

            selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);
            currPeriodCaption = GetShortQuarterText(quarterNum, currentYear);

            if (quarterNum == 1)
            {
                selectedPrevPeriod.Value = String.Format("[{0}].[Полугодие 2].[Квартал 4]", currentYear - 1);
                prevPeriodCaption = GetShortQuarterText(4, currentYear - 1);
            }
            else
            {
                selectedPrevPeriod.Value = String.Format("[{0}].[{1}].[Квартал {2}]", UserParams.PeriodYear.Value, String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(quarterNum - 1)), quarterNum - 1);
                prevPeriodCaption = GetShortQuarterText(quarterNum - 1, currentYear);
            }

            currMeasureCaption = ValueSelected ? "Значение" : "Оценка качества";
            selectedMeasure.Value = ValueSelected ? "Значение" : "Оценка показателя ";
            indicatorSet.Value = ValueSelected ? "Показатели для значения" : "Показатели для оценки";
            
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
        
        private static string GetShortQuarterText(int selectedQuarter, int selectedYear)
        {
            switch (selectedQuarter)
            {
                case 1:
                    {
                        return String.Format("на 01.04.{0}", selectedYear);
                    }
                case 2:
                    {
                        return String.Format("на 01.07.{0}", selectedYear);
                    }
                case 3:
                    {
                        return String.Format("на 01.10.{0}", selectedYear);
                    }
                case 4:
                    {
                        return String.Format("по итогам {0} года", selectedYear);
                    }
            }
            return String.Empty;
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0042_0004_HMAO_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование администратора", dtGrid);
            if (dtGrid.Rows.Count > 0 && dtGrid.Columns.Count > 2)
            {
                dtGrid.Columns.RemoveAt(0);

                dtGrid.Columns[0].ColumnName = "Код ГРБС";

                DataTable sortingDT = dtGrid.Clone();
                DataRow[] sortingRows = dtGrid.Select("", "Код ГРБС ASC");
                foreach (DataRow row in sortingRows)
                {
                    sortingDT.ImportRow(row);
                }
                sortingDT.Columns.Remove("Код ГРБС");
                sortingDT.AcceptChanges();

                UltraWebGrid.DataSource = sortingDT;
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
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            if (e.Layout.Bands[0].Columns.Count < 2)
            {
                return;
            }

            UltraGridColumn numberColumn = e.Layout.Bands[0].Columns[0];
            numberColumn.Header.Caption = "№ п/п";
            numberColumn.Width = CRHelper.GetColumnWidth(30);
            numberColumn.CellStyle.Padding.Right = 5;
            numberColumn.CellStyle.BackColor = numberColumn.Header.Style.BackColor;
            numberColumn.CellStyle.Font.Bold = true;
            numberColumn.SortingAlgorithm = SortingAlgorithm.NotSet;
            numberColumn.CellStyle.HorizontalAlign = HorizontalAlign.Right;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;

            headerLayout.AddCell("№ п/п");
            headerLayout.AddCell("Наименование ГРБС");

            string formatString = "N2";
            int width = 100;

            int beginIndex = 2;
            for (int i = beginIndex; i < e.Layout.Bands[0].Columns.Count; i = i + 3)
            {
                SetColumnFormat(e.Layout.Bands[0].Columns[i], width, formatString, HorizontalAlign.Right);
                SetColumnFormat(e.Layout.Bands[0].Columns[i + 1], width, formatString, HorizontalAlign.Right);
                SetColumnFormat(e.Layout.Bands[0].Columns[i + 2], width, formatString, HorizontalAlign.Right);

                string indicatorName = String.Empty;
                string[] columnHeaderParts = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                if (columnHeaderParts.Length > 1)
                {
                    indicatorName = columnHeaderParts[0];
                }

                string indicatorUnit = String.Empty;
                if (indicatorUnitList.ContainsKey(indicatorName) || indicatorName == "Итоговая оценка")
                {
                    indicatorUnit = ValueSelected ? indicatorUnitList[indicatorName] : "Баллы";
                }
                
                GridHeaderCell cell = headerLayout.AddCell(indicatorName);

                cell.AddCell(String.Format("{0} {1}", currMeasureCaption, prevPeriodCaption), indicatorUnit);
                cell.AddCell(String.Format("{0} {1}", currMeasureCaption, currPeriodCaption), indicatorUnit);
                cell.AddCell("Отклонение", indicatorUnit);
            }

            headerLayout.ApplyHeaderInfo();
        }

        private static void SetColumnFormat(UltraGridColumn column, int columnWidth, string columnFormatString, HorizontalAlign columnAlignment)
        {
            CRHelper.FormatNumberColumn(column, columnFormatString);
            column.Width = CRHelper.GetColumnWidth(columnWidth);
            column.CellStyle.HorizontalAlign = columnAlignment;
            column.CellStyle.Wrap = true;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[0].Value = Convert.ToInt32(e.Row.Index + 1).ToString("N0");

            for (int i = 1; i < e.Row.Cells.Count; i++)
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
                        if (value == -100500)
                        {
                            cell.Value = "*";
                            cell.Style.ForeColor = Color.Black;
                            cell.Style.Font.Size = 12;
                            
                        }
                    }
                }
            }
        }

        private void IndicatorDescriptionDataBind()
        {
            indicatorUnitList = new Dictionary<string, string>();

            string query = DataProvider.GetQueryText("FO_0042_0004_HMAO_indicatorDescription");
            dtIndicatorDescription = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtIndicatorDescription);

            foreach (DataRow row in dtIndicatorDescription.Rows)
            {
                if (row[0] != DBNull.Value && row[1] != DBNull.Value)
                {
                    string name = row[0].ToString();
                    string unit = row[1].ToString();

                    indicatorUnitList.Add(name, unit);
                }
            }
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            ReportPDFExporter1.HeaderCellHeight = 70;
            ReportPDFExporter1.Export(headerLayout);
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            
            ReportExcelExporter1.HeaderCellHeight = 60;
            
            ReportExcelExporter1.Export(headerLayout, 3);
        }

        #endregion
    }
}

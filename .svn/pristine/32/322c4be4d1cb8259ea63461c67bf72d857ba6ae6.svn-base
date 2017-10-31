using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0021_0003_Saratov
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();

        private DataTable dtGrid;
        private DataTable dtIndicatorDescription;
      

        private string prevPeriodCaption;
        private string currPeriodCaption;
        private static MemberAttributesDigest periodDigest;
        private static MemberAttributesDigest periodDigest2;

        private static Dictionary<string, string> indicatorNameList;

        private GridHeaderLayout headerLayout;

        #endregion

        #region Параметры запроса

        // выбранный начальный период
        private CustomParam selectedPeriod;
        // выбранный предыдущий период
        private CustomParam selectedPrevPeriod;
        // выбранная мера
        private CustomParam selectedMeasure;
        // первый год
        private CustomParam firstYear;
        // последний год
        private CustomParam endYear;

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
                        return "на 01.04.";
                    }
                case "Квартал 2":
                    {
                        return "на 01.07.";
                    }
                case "Квартал 3":
                    {
                        return "на 01.10.";
                    }
                case "Квартал 4":
                    {
                        return "по итогам ";
                    }
                default:
                    {
                        return classQuarter;
                    }
            }
        }

        private static string GetQuarterText(string classQuarter)
        {
            switch (classQuarter)
            {
                case "Квартал 1":
                    {
                        return "состоянием на 01.04.";
                    }
                case "Квартал 2":
                    {
                        return "состоянием на 01.07.";
                    }
                case "Квартал 3":
                    {
                        return "состоянием на 01.10.";
                    }
                case "Квартал 4":
                    {
                        return "итогами ";
                    }
                default:
                    {
                        return classQuarter;
                    }
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
            selectedPrevPeriod = UserParams.CustomParam("selected_prev_period");
            selectedMeasure = UserParams.CustomParam("selected_measure");
            firstYear = UserParams.CustomParam("first_year");
            endYear = UserParams.CustomParam("end_year");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            CrossLink1.Visible = true;
            CrossLink1.Text = "Рейтинг&nbsp;МО";
            CrossLink1.NavigateUrl = "~/reports/FO_0021_0002_Saratov/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Результаты&nbsp;оценки&nbsp;качества";
            CrossLink2.NavigateUrl = "~/reports/FO_0021_0001_Saratov/Default.aspx";

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
            firstYear.Value = "2009";

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0021_0003_Saratov_endYear");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear.Value = dtDate.Rows[0][0].ToString();

		        ComboPeriod.Width = 200;
                ComboPeriod.MultiSelect = false;
                ComboPeriod.ShowSelectedValue = false;
                ComboPeriod.ParentSelect = false;
                periodDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0021_0003_Saratov_date");
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodDigest.UniqueNames, periodDigest.MemberLevels));
                ComboPeriod.SelectLastNode();
                ComboPeriod.PanelHeaderTitle = "Выберите дату";

                ComboPeriodCompare.Width = 300;
                ComboPeriodCompare.MultiSelect = false;
                ComboPeriodCompare.ShowSelectedValue = false;
                ComboPeriodCompare.ParentSelect = false;
                periodDigest2 = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0021_0003_Saratov_dateCompare");
                ComboPeriodCompare.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodDigest2.UniqueNames, periodDigest2.MemberLevels));
                ComboPeriodCompare.SelectLastNode();
                ComboPeriodCompare.PanelHeaderTitle = "Выберите дату для сравнения";       
            }

            Page.Title = String.Format("Динамика результатов оценки качества управления финансами и платежеспособности муниципальных образований Саратовской области");
            PageTitle.Text = Page.Title;
       
            selectedPeriod.Value = periodDigest.GetMemberUniqueName(ComboPeriod.SelectedValue);
            selectedPrevPeriod.Value = periodDigest2.GetMemberUniqueName(ComboPeriodCompare.SelectedValue);

            string quarter = GetParamQuarter(ComboPeriod.SelectedNode.ToString().Trim());
            string quarterCompare = GetParamQuarter(ComboPeriodCompare.SelectedNode.ToString().Trim());

            prevPeriodCaption = String.Format("{0}{1}а", quarterCompare, ComboPeriodCompare.SelectedNodeParent);
            currPeriodCaption = String.Format("{0}{1}а", quarter, ComboPeriod.SelectedNodeParent);

            PageSubTitle.Text = String.Format("Оценка качества {0} по сравнению с {1}{2}а", currPeriodCaption, GetQuarterText(ComboPeriodCompare.SelectedNode.ToString().Trim()), ComboPeriodCompare.SelectedNodeParent);

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

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0021_0003_Saratov_grid");
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
            switch (indicatorName)
            {
                case "P1":
                case "P2":
                case "P3":
                case "P4":
                case "P5":
                case "P6":
                case "P7":
                case "P8":
                case "P9":
                case "P10":
                case "P11":
                case "P12":
                case "P13":
                case "P14":
                case "P15":
                case "P16":
                case "P17":
                case "P18":
                case "P19":
                case "P20":
                case "P21":
                    {
                        return (MeasureType == MeasureType.Density) ? "N3" : "N0";
                    }
                case "Итоговая оценка":
                case "Средняя оценка":
                    {
                        return "N1";
                    }
                case "Ранг":
                    {
                        return "N0";
                    }
                default:
                    {
                        return "N3";
                    }
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

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(120);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 120;

                string[] columnCaptionParts = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                if (columnCaptionParts.Length > 1)
                {
                    string indicatorName = columnCaptionParts[0];
                    formatString = GetIndicatorFormatString(indicatorName);
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 3)
            {
                string columnCaption = e.Layout.Bands[0].Columns[i].Header.Caption;
                string indicatorName = columnCaption;

                string[] columnCaptionParts = columnCaption.Split(';');
                if (columnCaptionParts.Length > 1)
                {
                    string code = columnCaptionParts[0];
                    if (indicatorNameList.ContainsKey(code))
                    {
                        indicatorName = indicatorNameList[code];
                    }
                    else
                    {
                        indicatorName = code;
                    }
                }

                GridHeaderCell indicatorCell = headerLayout.AddCell(indicatorName);

                indicatorCell.AddCell(prevPeriodCaption);
                indicatorCell.AddCell(currPeriodCaption);
                indicatorCell.AddCell("Отклонение");
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

            string query = DataProvider.GetQueryText("FO_0021_0003_Saratov_indicatorDescription");
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

            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout);
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            ReportExcelExporter1.HeaderCellHeight = 50;
            ReportExcelExporter1.Export(headerLayout, 3);
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
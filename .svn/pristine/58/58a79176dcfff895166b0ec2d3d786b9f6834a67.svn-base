using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0040_0002
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtIndicatorDescription;
        private int firstYear = 2010;
        private int endYear = 2011;
        private static Dictionary<string, string> indicatorNameList;
        private static Dictionary<string, string> indicatorFormatList;
        private static Dictionary<string, string> indicatorCodeNameList;
        private int selectedQuarterIndex;
        private int selectedYear;

        #endregion

        private bool IsFirstQuarter
        {
            get { return selectedQuarterIndex == 1; }
        }

        #region Параметры запроса

        // выбранный начальный период
        private CustomParam selectedPeriod;
        // выбранный предыдущий период
        private CustomParam selectedPrevPeriod;
        // множество периодов
        private CustomParam periodSet;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            double heightScale = 0.65;
            Server.ScriptTimeout = 300; 
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * heightScale);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров запроса

            if (selectedPeriod == null)
            {
                selectedPeriod = UserParams.CustomParam("selected_period");
            }
            if (selectedPrevPeriod == null)
            {
                selectedPrevPeriod = UserParams.CustomParam("selected_prev_period");
            }
            if (periodSet == null)
            {
                periodSet = UserParams.CustomParam("period_set");
            }
            
            #endregion

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;мониторинга&nbsp;качества ";
            CrossLink1.NavigateUrl = "~/reports/FO_0040_0001/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Картограмма&nbsp;результатов&nbsp;мониторинга";
            CrossLink2.NavigateUrl = "~/reports/FO_0040_0003/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Диаграмма&nbsp;динамики&nbsp;основных&nbsp;показателей";
            CrossLink3.NavigateUrl = "~/reports/FO_0040_0004/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0040_0002_date");
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
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillMonitoringQuarters());
                ComboQuarter.SetСheckedState(GetParamQuarter(quarter), true);
            }

            selectedYear = Convert.ToInt32(ComboYear.SelectedValue);
            selectedQuarterIndex = ComboQuarter.SelectedIndex + 1;

            Page.Title = String.Format("Динамика основных показателей мониторинга качества управления бюджетным процессом и соблюдения требований бюджетного законодательства РФ муниципальными образованиями Самарской области");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("по итогам {0} квартала {1} года", selectedQuarterIndex, selectedYear);
                        
            UserParams.PeriodYear.Value = selectedYear.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(selectedQuarterIndex));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", selectedQuarterIndex);

            selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", selectedYear, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);

            if (IsFirstQuarter)
            {
                selectedPrevPeriod.Value = String.Format("[{0}].[Полугодие 2].[Квартал 4]", selectedYear - 1);
            }
            else
            {
                string prevQuarter = String.Format("Квартал {0}", selectedQuarterIndex - 1);
                string prevHalfYear = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(selectedQuarterIndex - 1));
                selectedPrevPeriod.Value = String.Format("[{0}].[{1}].[{2}]", selectedYear, prevHalfYear, prevQuarter);
            }

            IndicatorDescriptionDataBind();

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
                        return "мониторинг за 1 квартал";
                    }
                case "Квартал 2":
                    {
                        return "мониторинг за 2 квартал";
                    }
                case "Квартал 3":
                    {
                        return "мониторинг за 3 квартал";
                    }
                case "Квартал 4":
                    {
                        return "мониторинг за 4 квартал (по итогам года)";
                    }
                default:
                    {
                        return classQuarter;
                    }
            }
        }

        private static string GetDateQuarterText(int quarterIndex, int year)
        {
            return String.Format("{0} квартал {1} года", quarterIndex, year);
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0040_0002_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = row[0].ToString().Replace("район", "р-н");
                    }
                }

                for (int i = dtGrid.Columns.Count - 2; i > 1; i = i - 3)
                {
                    bool isNull = true;
                    int j = 0;
                    while ((isNull) && (j < dtGrid.Rows.Count))
                    {
                        if (dtGrid.Rows[j][i] != DBNull.Value)
                        {
                            isNull = false;
                        }
                        j++;
                    }
                    if (isNull)
                    {
                        dtGrid.Columns.RemoveAt(i + 1);
                        dtGrid.Columns.RemoveAt(i);
                        dtGrid.Columns.RemoveAt(i - 1);
                    }
                }

                if (dtGrid.Columns.Count > 1)
                {
                    UltraWebGrid.DataSource = dtGrid;
                }
                else
                {
                    UltraWebGrid.DataSource = null;
                }
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        private static string GetIndicatorFormatString(string indicatorName)
        {
            if ((indicatorName == "Рейтинг") || (indicatorName == "Средняя оценка"))
            {
                return "N2";
            }
            else
            {
                if (indicatorName == "Группа муниципального образования")
                {
                    return "N0";
                }
                else
                {
                    if (indicatorFormatList[indicatorName] == "ДА/НЕТ")
                    {
                        return "N0";
                    }
                    else
                    {
                        return "N2";
                    }
                }
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count < 2)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, e.Layout.Bands[0].Columns[1].Header.Caption.Split(';')[0], "");

            int beginIndex = 1;

            for (int i = beginIndex; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string indicatorName = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];
                string formatString = GetIndicatorFormatString(indicatorName);

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(75);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

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

            int multiHeaderPos = beginIndex;

            for (int i = beginIndex; i < e.Layout.Bands[0].Columns.Count; i = i + 3)
            {
                ColumnHeader ch = new ColumnHeader(true);
                string caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];

                if (indicatorNameList.ContainsKey(caption))
                {
                    string key = caption;
                    if (selectedYear == 2011)
                    {
                        if (selectedQuarterIndex >= 3)
                        {
                            caption = CorrectHeader(caption, key);
                        }
                    }
                    else
                    {
                        if (selectedYear > 2011)
                        {
                            caption = CorrectHeader(caption, key);
                        }
                    }      
                    //caption = CorrectHeader(caption, key);
                    caption = String.Format("{0} {1}", caption, indicatorNameList[key]);
                }
                ch.Caption = caption;

                int prevQuarterIndex = (selectedQuarterIndex == 1) ? 4 : selectedQuarterIndex - 1;
                int prevYear = (selectedQuarterIndex == 1) ? selectedYear - 1 : selectedYear;

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, GetDateQuarterText(prevQuarterIndex, prevYear), "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, GetDateQuarterText(selectedQuarterIndex, selectedYear), "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, "Отклонение", "");

                CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, caption, multiHeaderPos, 0, 3, 1);

                multiHeaderPos += 3;
            }
        }

        protected string CorrectHeader(string caption, string code)
        {
            int index = caption.IndexOf(")");
            if ((index + 1) == caption.Length-1)
            {
                caption = caption.Remove(index + 1, 1);
            }
            index = caption.IndexOf("(") + 1;          
            while (caption[index] != ')')
            {
                caption = caption.Remove(index, 1);
            }
            caption = caption.Insert(caption.IndexOf("(") + 1, indicatorCodeNameList[code]);
            return caption;
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

        #region Показатели

        private void IndicatorDescriptionDataBind()
        {
            indicatorNameList = new Dictionary<string, string>();
            indicatorFormatList = new Dictionary<string, string>();
            indicatorCodeNameList = new Dictionary<string, string>();

            string query = DataProvider.GetQueryText("FO_0040_0002_indicatorDescription");
            dtIndicatorDescription = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtIndicatorDescription);

            foreach (DataRow row in dtIndicatorDescription.Rows)
            {
                string code = row[0].ToString();
                string name = row[1].ToString();
                string format = row[2].ToString();
                string codeName = row[3].ToString();

                indicatorNameList.Add(code, name);
                indicatorFormatList.Add(code, format);
                indicatorCodeNameList.Add(code, codeName);
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;

            e.CurrentWorksheet.Columns[0].CellFormat.FormatString = "";
            e.CurrentWorksheet.Columns[0].Width = 150 * 37;
            
            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[3].Height = 40 * 37;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;

                e.CurrentWorksheet.Rows[4].Height = 20 * 37;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }

            for (int i = 0; i < columnCount; i = i + 1)
            {
                string formatString = "#,##0.00";

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = 100 * 37;
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
            string key = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
            if (indicatorNameList.ContainsKey(key))
            {
                e.HeaderText = String.Format("{0} {1}", key, indicatorNameList[key]);
            }
            else
            {
                e.HeaderText = key;
            }
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
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
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

        #endregion
    }
}
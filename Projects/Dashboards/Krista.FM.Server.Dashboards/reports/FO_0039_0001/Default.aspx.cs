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

namespace Krista.FM.Server.Dashboards.reports.FO_0039_0001
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

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            double scale = IsMozilla ? 0.6 : 0.5;

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

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.HeaderChildCellHeight = 120;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Динамика&nbsp;результатов&nbsp;оценки";
            CrossLink1.NavigateUrl = "~/reports/FO_0039_0002/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Рейтинг&nbsp;МР";
            CrossLink2.NavigateUrl = "~/reports/FO_0039_0003/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Сравн.характеристика&nbsp;мин.&nbsp;и&nbsp;макс.&nbsp;оценок";
            CrossLink3.NavigateUrl = "~/reports/FO_0039_0004/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "Картограмма";
            CrossLink4.NavigateUrl = "~/reports/FO_0039_0005/Default.aspx";

            CrossLink5.Visible = true;
            CrossLink5.Text = "Результаты&nbsp;оценки&nbsp;по&nbsp;отд.показателю";
            CrossLink5.NavigateUrl = "~/reports/FO_0039_0006/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0039_0001_date");
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

            Page.Title = String.Format("Результат оценки качества организации и осуществления бюджетного процесса в МР Омской области");
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

        private static string GetIndicatorCode(string description)
        {
            if (indicatorDescripitonList != null && indicatorDescripitonList.Count != 0)
            {
                foreach (string key in indicatorDescripitonList.Keys)
                {
                    if (indicatorDescripitonList[key] == description)
                    {
                        return key;
                    }
                }
            }
            return description;
        }

        private void IndicatorDescriptionDataBind()
        {
            indicatorDescripitonList = new Dictionary<string, string>();
            indicatorExportDescripitonList = new Dictionary<string, string>();
            indicatorUnitList = new Dictionary<string, string>();
            indicatorNameList = new Dictionary<string, string>();
            indicatorDirectionList = new Dictionary<string, string>();

            string query = DataProvider.GetQueryText("FO_0039_0001_indicatorDescription");
            dtIndicatorDescription = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtIndicatorDescription);

            foreach (DataRow row in dtIndicatorDescription.Rows)
            {
                string code = row[0].ToString();
                string name = row[1].ToString();
                string direction = row[2].ToString();
                string unit = ValueSelected ? row[3].ToString() : "Баллы";
                string measure = ValueSelected ? "Значение" : "Оценка показателя";

                string descriptionTable =
                    String.Format(@"<table width='100%' height='100%' style='font-weight:normal;'>
                                        <tr>
                                           <td align='left' valign='top' width='90px' > 
                                             Код
                                           </td>
                                           <td align='right' valign='top' width='*'> 
                                             {0}
                                           </td>
                                        </tr>
                                        <tr>
                                           <td align='left' valign='top' width='90px' > 
                                             Единицы измерения
                                           </td>
                                           <td align='right' valign='top' width='*'> 
                                             {1}
                                           </td>
                                        </tr>
                                        <tr>
                                           <td align='middle' valign='bottom' width='90px' colspan='2'
                                            style='font-weight: bold; border-top: white thin solid;'> 
                                             {2}
                                           </td>
                                        </tr>
                                    </table>", code, unit, measure);

                string exportDescriptionTable = String.Format(@"Код: {0}{3}Единицы измерения: {1}{3}{3}{2}", code, unit, measure, Environment.NewLine);

                string nameTable =
                    String.Format(@"<table width='100%' height='100%'>
                                        <tr>
                                           <td height='100px'> 
                                             {0}
                                           </td>
                                        </tr>
                                        <tr>
                                           <td valing='top'> 
                                             {1}
                                           </td>
                                        </tr>
                                    </table>", name, descriptionTable);

                string exportNameTable = String.Format(@"{0}{2}{2}{1}", name, exportDescriptionTable, Environment.NewLine);

                indicatorDescripitonList.Add(code, nameTable);
                indicatorExportDescripitonList.Add(code, exportNameTable);
                indicatorUnitList.Add(code, unit);
                indicatorNameList.Add(code, name);
                indicatorDirectionList.Add(code, direction);
            }
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0039_0001_grid");
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

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            if (!ValueSelected)
            {
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(ValueSelected ? 50 : 75);
                e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            }

            int beginIndex = ValueSelected ? 1 : 2;

            for (int i = beginIndex; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N2";
                int widthColumn = 250;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || (i == 1 && !ValueSelected))
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
            int spanX = 1;
            string currentCaption = String.Empty;

            for (int i = beginIndex; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string caption = e.Layout.Bands[0].Columns[i].Header.Caption;
                bool lastColumn = i == e.Layout.Bands[0].Columns.Count - 1;
                if (caption != String.Empty)
                {
                    string headerCaption = indicatorDirectionList[caption];
                    string columnCaption = indicatorDescripitonList[caption];

                    e.Layout.Bands[0].Columns[i].Key = headerCaption;

                    if (i == beginIndex)
                    {
                        currentCaption = headerCaption;
                    }
                    else
                    {
                        if (currentCaption != headerCaption)
                        {
                            CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, currentCaption, multiHeaderPos, 0, spanX, 1);

                            multiHeaderPos += spanX;
                            currentCaption = headerCaption;
                            spanX = 1;
                        }
                        else
                        {
                            spanX++;
                        }
                    }

                    string hint = String.Empty;
                    if (indicatorUnitList.ContainsKey(caption))
                    {
                        hint = indicatorUnitList[caption];
                    }
                    else if (!ValueSelected)
                    {
                        hint = "Баллы";
                    }
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, columnCaption, hint);
                }
            }

            CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, currentCaption, multiHeaderPos, 0, spanX, 1);
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

        private static void SetExportGridParams(UltraWebGrid grid)
        {
            foreach (UltraGridColumn column in grid.Columns)
            {
                string key = GetIndicatorCode(column.Header.Caption);
                if (indicatorExportDescripitonList.ContainsKey(key))
                {
                    column.HeaderText = indicatorExportDescripitonList[key];
                }
            }
        }

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
                e.CurrentWorksheet.Rows[3].Height = 20 * 37;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Left;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;

                e.CurrentWorksheet.Rows[4].Height = 80 * 37;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }

            for (int i = 1; i < columnCount; i = i + 1)
            {
                string formatString = "#,##0.00";
                int columnWidth = (i == 1 && !ValueSelected) ? 50 : 250;

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = columnWidth * 37;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            SetExportGridParams(UltraWebGrid);
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
            SetExportGridParams(UltraWebGrid);
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
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

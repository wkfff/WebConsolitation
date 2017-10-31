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

namespace Krista.FM.Server.Dashboards.reports.FO_0042_0001_Gub
{
    public partial class _default : CustomReportPage
    { 
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtIndicatorDescription;
        private int firstYear = 2008;
        private int endYear = 2011;
        private static Dictionary<string, string> indicatorDescripitonList;
        private static Dictionary<string, string> indicatorExportDescripitonList;
        private static Dictionary<string, string> indicatorUnitList;
        private int currentYear; 

        #endregion

        public bool ValueSelected   
        {
            get { return MeasureButtonList.SelectedIndex == 1; }
        }

        #region Параметры запроса

        // выбранная мера
        private CustomParam selectedMeasure;
        private CustomParam selectedQuater;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            
            double scale = 0.6;
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * scale);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            ComboQuater.Width = 300;
            #region Инициализация параметров запроса

            if (selectedMeasure == null)
            {
                selectedMeasure = UserParams.CustomParam("selected_maasure");
                selectedQuater = UserParams.CustomParam("selected_quater");
            }

            #endregion
            
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.HeaderCellHeight = 100;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            
            CrossLink1.Visible = true;
            CrossLink1.Text = "Рейтинг&nbsp;ГРБС";
            CrossLink1.NavigateUrl = "~/reports/FO_0042_0002_Gub/default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Анализ&nbsp;оценки&nbsp;качества";
            CrossLink2.NavigateUrl = "~/reports/FO_0042_0003_Gub/default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0042_0001_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
            //    ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.FillDictionaryValues(YearsLoad());
                ComboYear.SelectLastNode();
               // ComboYear.SetСheckedState(endYear.ToString(), true);
                ComboQuater.Title = "Оценка качества";
                ComboQuater.FillDictionaryValues(QuaterLoad());
                ComboQuater.SelectLastNode();
            }

            currentYear = Convert.ToInt32(ComboYear.SelectedValue);
            if (ComboQuater.SelectedIndex == 1)
            { 
                selectedQuater.Value = "Данные года";
                Page.Title = String.Format("Результаты оценки качества финансового менеджмента, осуществляемого главными распорядителями бюджетных средств в муниципальном образовании город Губкинский по состоянию на  {0}", "1.01." + (int.Parse(ComboYear.SelectedValue)+1).ToString() + " года");
            }
            else
            {
                selectedQuater.Value = "Остатки на начало года";
                Page.Title = String.Format("Результаты оценки качества финансового менеджмента, осуществляемого главными распорядителями бюджетных средств в муниципальном образовании город Губкинский по состоянию на  {0}", "1.07." + ComboYear.SelectedValue + " года");
            }
            
            PageTitle.Text = Page.Title;
            selectedMeasure.Value = ValueSelected ? "Значение" : "Оценка показателя";

            UserParams.PeriodYear.Value = currentYear.ToString();

            IndicatorDescriptionDataBind();

            UltraWebGrid.Bands.Clear();     
            UltraWebGrid.DataBind();
        }

        Dictionary<string, int> QuaterLoad()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add("по состоянию на 1.07",0);
            d.Add("по итогам года", 0);
            return d;
        }

        Dictionary<string, int> YearsLoad()
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("FO_0042_0001_date"), "Дата", dt);
            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                d.Add(dt.Rows[i][0].ToString(), 0);
            }
            return d;
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

            string query = DataProvider.GetQueryText("FO_0042_0001_indicatorDescription");
            dtIndicatorDescription = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtIndicatorDescription);

            int descriptionCellHeight = 70;

            foreach (DataRow row in dtIndicatorDescription.Rows)
            {
                string name = row[0].ToString();
                string code = row[1].ToString();
                string unit = row[2].ToString();

                string descriptionTable =
                    String.Format(@"<table width='100%' height='100%' style='font-weight:normal;'>
                                        <tr>
                                           <td align='left' valign='top' width='{1}px' > 
                                             Обозначение
                                           </td>
                                           <td align='right' valign='top' width='*'> 
                                             {0}
                                           </td>
                                        </tr>
                                    </table>", code, descriptionCellHeight);

                string exportDescriptionTable = String.Format(@"Обозначение: {0}", code);

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

                indicatorDescripitonList.Add(name, nameTable);
                indicatorExportDescripitonList.Add(name, exportNameTable);
                indicatorUnitList.Add(name, unit);
            }
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0042_0001_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование администратора", dtGrid);
            if (dtGrid.Rows.Count > 0 && dtGrid.Columns.Count > 2)
            {
                dtGrid.Columns.RemoveAt(0);

                for (int i = 0; i < dtGrid.Columns.Count; i++)
                {
                    dtGrid.Columns[i].ColumnName = dtGrid.Columns[i].ColumnName.ToString().Replace(@" 
", " ");
                }

                DataTable sortingDT = dtGrid.Clone();
                DataRow[] sortingRows = dtGrid.Select("", "Код ГРБС ASC");
                foreach (DataRow row in sortingRows)
                {
                    sortingDT.ImportRow(row);
                }
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

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(50);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "000");

            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[2].CellStyle.Wrap = true;

            int beginIndex = 3;

            if (!ValueSelected)
            {
                beginIndex = 5;

                e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(100);
                e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");

                e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(100);
                e.Layout.Bands[0].Columns[4].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");

                e.Layout.Bands[0].Columns[5].Width = CRHelper.GetColumnWidth(150);
                e.Layout.Bands[0].Columns[5].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");
            }

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
                if (i == 0 || i == 1 || i == 2 || (!ValueSelected && (i == 3 || i == 4 )))
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

            for (int i = beginIndex; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                ColumnHeader ch = new ColumnHeader(true);
                string caption = e.Layout.Bands[0].Columns[i].Header.Caption;

                if (indicatorDescripitonList.ContainsKey(caption))
                {
                    ch.Caption = indicatorDescripitonList[caption];
                }


                string hint = String.Empty;
                if (!ValueSelected)
                {
                    hint = "Баллы";
                }
                else if (indicatorUnitList.ContainsKey(caption))
                {
                    hint = indicatorUnitList[caption];
                }

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, selectedMeasure.Value, 
                    String.Format("Единицы измерения: {0}", hint));

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 1;
                ch.RowLayoutColumnInfo.SpanX = 1;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
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
                            cell.Value = null;
                        }
                    }
                }
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text ;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            
            int columnCount = UltraWebGrid.Columns.Count;
            int rowsCount = UltraWebGrid.Rows.Count;

            e.CurrentWorksheet.Columns[0].CellFormat.FormatString = "0";
            e.CurrentWorksheet.Columns[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            e.CurrentWorksheet.Columns[0].Width = 50 * 37;

            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "0";
            e.CurrentWorksheet.Columns[1].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            e.CurrentWorksheet.Columns[1].Width = 50 * 37;

            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "";
            e.CurrentWorksheet.Columns[2].Width = 150 * 37;

            // расставляем стили у ячеек хидера
            for (int i = 2; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[3].Height = 80 * 37;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Left;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;

                e.CurrentWorksheet.Rows[4].Height = 15 * 37;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;

                int width = 250;
                if (!ValueSelected && (i == 2 || i == 3 || i == 4))
                {
                    width = 100;
                }

                string formatString = (!ValueSelected && i == 3) ? "#,###0;[Red]-#,###0" : "#,###0.00;[Red]-#,###0.00";
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = width * 37;
            }

            // расставляем стили у начальных колонок
            for (int i = 5; i < rowsCount + 5; i++)
            {
                e.CurrentWorksheet.Rows[i].Height = 20*37;
                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.Alignment = HorizontalCellAlignment.Left;

                for (int j = 3; j < columnCount; j++)
                {
                    e.CurrentWorksheet.Rows[i].Cells[j].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                    e.CurrentWorksheet.Rows[i].Cells[j].CellFormat.Alignment = HorizontalCellAlignment.Right;
                }
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
            if (indicatorExportDescripitonList.ContainsKey(key))
            {
                //e.HeaderText = string.Format("{0} ({1})", indicatorNameList[key], key);
                e.HeaderText = indicatorExportDescripitonList[key];
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
            foreach (HeaderBase header in UltraWebGrid.DisplayLayout.Bands[0].HeaderLayout)
            {
                header.Caption = GetIndicatorCode(header.Caption);
            }

            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
//            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
//            {
//                e.Layout.Bands[0].Columns[i].Width = i == 1 ? CRHelper.GetColumnWidth(250) : CRHelper.GetColumnWidth(150);
//            }

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
        }

        #endregion
    }
}

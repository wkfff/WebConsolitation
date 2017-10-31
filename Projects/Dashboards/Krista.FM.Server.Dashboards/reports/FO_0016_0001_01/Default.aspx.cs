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
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0016_0001_01
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtIndicatorDescription;
        private int firstYear = 2009;
        private int endYear = 2011;
        private static Dictionary<string, string> indicatorDescripitonList;

        #endregion

        #region Параметры запроса

        // выбранный регион
        private CustomParam selectedRegion;
        // выбранная группа МО
        private CustomParam selectedMOGroup;
        // уровень районов
        private CustomParam regionLevel;
        // уровень поселений
        private CustomParam settlementLevel;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 350);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров запроса

            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }

            if (selectedMOGroup == null)
            {
                selectedMOGroup = UserParams.CustomParam("selected_mo_group");
            }
            regionLevel = UserParams.CustomParam("region_level");
            settlementLevel = UserParams.CustomParam("settlement_level");

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
            regionLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            settlementLevel.Value = RegionSettingsHelper.Instance.SettlementLevel;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0016_0001_01_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string baseQuarter = dtDate.Rows[0][2].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboQuarter.Title = "Квартал";
                ComboQuarter.Width = 170;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillQuaters(false));
                ComboQuarter.SetСheckedState(baseQuarter, true);

                ComboMOGroup.Title = "Группа МО";
                ComboMOGroup.Width = 200;
                ComboMOGroup.MultiSelect = false;
                ComboMOGroup.ParentSelect = true;
                ComboMOGroup.FillDictionaryValues(CustomMultiComboDataHelper.FillBKKUGroupMO());
                ComboMOGroup.SetСheckedState("Все группы ", true);
            }

            selectedRegion.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;

            if (ComboMOGroup.SelectedValue == "Все группы ")
            {
                selectedMOGroup.Value = "(true)";
            }
            else
            {
                selectedMOGroup.Value = string.Format("([Показатели__БККУ_Сопоставимый].[Показатели__БККУ_Сопоставимый].[Данные всех источников].[{1}], [Measures].[Значение ]) = {0}",
                    ComboMOGroup.SelectedIndex,  ComboYear.SelectedValue.Contains("2010") ? "Группа МО" : "БК 1");
            }

            Page.Title = string.Format("Результаты мониторинга БК и КУ в разрезе районов и поселений");
            PageTitle.Text = Page.Title;
            
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            int quarterNum = ComboQuarter.SelectedIndex + 1;

            PageSubTitle.Text = string.Format("за {0} квартал {1} года", quarterNum, yearNum);
//            CommentTextLabel.Text = (yearNum == 2009)
//                                        ? "Результаты мониторинга местных бюджетов в соответствии с Приказом управления финансов и налоговой политики Новосибирской области от 17.03.2008 №100"
//                                        : "Результаты мониторинга местных бюджетов в соответствии с Приказом Департамента финансов и налоговой политики Новосибирской области от 31.03.2010 №90";
            CommentTextLabel.Visible = false;

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(quarterNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", quarterNum);

            IndicatorDescriptionDataBind();

            UltraWebGrid.Bands.Clear();     
            UltraWebGrid.DataBind();
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

            string query = DataProvider.GetQueryText("FO_0016_0001_01_indicatorDescription");
            dtIndicatorDescription = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtIndicatorDescription);

            foreach (DataRow row in dtIndicatorDescription.Rows)
            {
                string code = row[0].ToString();
                string name = row[1].ToString();
                string limit = row[2].ToString();
                string condition = row[3].ToString();
                
                string descriptionTable =
                    string.Format(@"<table width='100%' height='100%' style='font-weight:normal;'>
                                        <tr>
                                           <td align='left' width='150px' > 
                                             Код
                                           </td>
                                           <td align='right' width='*'> 
                                             {0}
                                           </td>
                                        </tr>
                                        <tr>
                                           <td align='left' width='150px' > 
                                             Пороговое значение
                                           </td>
                                           <td align='right' width='*'> 
                                             {1}
                                           </td>
                                        </tr>
                                        <tr>
                                           <td align='left'  width='150px' > 
                                             Условие
                                           </td>
                                           <td align='right' width='*'> 
                                             {2}
                                           </td>
                                        </tr>
                                    </table>", code, limit, condition);

                string nameTable =
                    string.Format(@"<table width='100%' height='100%'>
                                        <tr>
                                           <td height='100px'> 
                                             {0}
                                           </td>
                                        </tr>
                                        <tr>
                                           <td valing='bottom'> 
                                             {1}
                                           </td>
                                        </tr>
                                    </table>", name, descriptionTable);

                indicatorDescripitonList.Add(code, nameTable);
            }
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0016_0001_01_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[1] != DBNull.Value)
                    {
                        row[1] = row[1].ToString().Replace("район", "р-н");
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

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                
                if (i > 3 && i % 2 == 0)
                {
                    formatString = "N2";
                }
                
                int widthColumn = 150;

                switch(i)
                {
                    case 2:
                        {
                            widthColumn = 80;
                            break; 
                        }
                    case 3:
                        {
                            widthColumn = 120;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(110);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 1 || i == 2 || i == 3)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Район", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Группа МО", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "Общее количество нарушений", "");

            int multiHeaderPos = 4;

            for (int i = 4; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 2)
            {
                ColumnHeader ch = new ColumnHeader(true);
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                ch.Caption = indicatorDescripitonList[captions[0]];
                
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Значение", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Количество", "");

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 2;
                ch.RowLayoutColumnInfo.SpanX = 2;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                int levelColumnIndex = e.Row.Cells.Count - 1;
                bool crimeCountColumn = (i > 3 && i % 2 != 0);

                if (e.Row.Cells[levelColumnIndex].Value != null)
                {
                    string level = e.Row.Cells[levelColumnIndex].Value.ToString();
                    if (level == "МР" || level == "ГО")
                    {
                        e.Row.Cells[i].Style.Font.Bold = true;
                    }
                }

                if (crimeCountColumn && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    int value = Convert.ToInt32(e.Row.Cells[i].Value);
                    if (value == 1)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerRed.gif";
                        e.Row.Cells[i].Title = string.Format("Не соблюдается условие");
                    }
                    else if (value == 0)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerGreen.gif";
                        e.Row.Cells[i].Title = string.Format("Соблюдается условие");
                    }
                    e.Row.Cells[i].Style.CustomRules =
                        "background-repeat: no-repeat; background-position: right top; margin: 0px";
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

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = CommentTextLabel.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;
            int rowsCount = UltraWebGrid.Rows.Count;

            e.CurrentWorksheet.Columns[0].CellFormat.FormatString = "";
            e.CurrentWorksheet.Columns[0].Width = 150 * 37;
            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "";
            e.CurrentWorksheet.Columns[1].Width = 150 * 37;
            
            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[3].Height = 15 * 37;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;

                e.CurrentWorksheet.Rows[4].Height = 20 * 37;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }

            for (int i = 2; i < columnCount; i = i + 1)
            {
                string formatString = (i != 2 && i % 2 == 0) ? "#,##0.00" : "0";

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = 100*37;
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
            foreach (HeaderBase header in UltraWebGrid.DisplayLayout.Bands[0].HeaderLayout)
            {
                header.Caption = GetIndicatorCode(header.Caption);
            }

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

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(CommentTextLabel.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {

        }

        #endregion
    }
}

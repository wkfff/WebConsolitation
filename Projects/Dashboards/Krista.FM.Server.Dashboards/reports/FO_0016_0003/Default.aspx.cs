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

namespace Krista.FM.Server.Dashboards.reports.FO_0016_0003
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtCommentText;
        private int firstYear = 2009;
        private int endYear = 2011;
        
        #endregion

        #region Параметры запроса

        // выбранный регион
        private CustomParam selectedRegion;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 330);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            #region Инициализация параметров запроса

            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }

            #endregion

            UltraGridExporter1.MultiHeader = false;
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

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0016_0003_date");
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

                ComboRegion.Visible = true;
                ComboRegion.Title = "МО";
                ComboRegion.Width = 280;
                ComboRegion.MultiSelect = false;
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillSettlements(RegionsNamingHelper.LocalSettlementTypes, false));
                ComboRegion.SetСheckedState("Баганский район", true); 
            }

            selectedRegion.Value = RegionsNamingHelper.LocalSettlementUniqueNames[ComboRegion.SelectedValue];

            Page.Title = string.Format("Результаты мониторинга БК и КУ в разрезе показателей");
            PageTitle.Text = Page.Title;

            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            int quarterNum = ComboQuarter.SelectedIndex + 1;

            PageSubTitle.Text = string.Format("{2} за {0} квартал {1} года", quarterNum, yearNum, ComboRegion.SelectedValue);
//            CommentTextLabel.Text = (yearNum == 2009)
//                            ? "Результаты мониторинга местных бюджетов в соответствии с Приказом управления финансов и налоговой политики Новосибирской области от 17.03.2008 №100"
//                            : "Результаты мониторинга местных бюджетов в соответствии с Приказом Департамента финансов и налоговой политики Новосибирской области от 31.03.2010 №90";
            CommentTextLabel.Visible = false;

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(quarterNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", quarterNum);

            UltraWebGrid.Bands.Clear();     
            UltraWebGrid.DataBind();

            CommentTextDataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0016_0003_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Обозначение", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = dtGrid;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid.Height = Unit.Empty;
            UltraWebGrid.Width = Unit.Empty;
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

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "";
                
                int widthColumn = 150;
                HorizontalAlign horizontalAlign = HorizontalAlign.Right;
                switch(i)
                {
                    case 1:
                        {
                            widthColumn = 810;
                            e.Layout.Bands[0].Columns[i].CellStyle.Wrap = true;
                            horizontalAlign = HorizontalAlign.Left;
                            break;
                        }
                    case 2:
                    case 3:
                        {
                            widthColumn = 120;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = horizontalAlign;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(80);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool crimeCountColumn = (i == e.Row.Cells.Count - 1);
                int crimeValueColumnIndex = 2;

                if (crimeCountColumn && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    int value = Convert.ToInt32(e.Row.Cells[i].Value);
                    if (value == 1)
                    {
                        e.Row.Cells[crimeValueColumnIndex].Style.BackgroundImage = "~/images/ballRedBB.png";
                        e.Row.Cells[crimeValueColumnIndex].Title = string.Format("Не соблюдается условие");
                    }
                    else if (value == 0)
                    {
                        e.Row.Cells[crimeValueColumnIndex].Style.BackgroundImage = "~/images/ballGreenBB.png";
                        e.Row.Cells[crimeValueColumnIndex].Title = string.Format("Соблюдается условие");
                    }
                    e.Row.Cells[crimeValueColumnIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
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

        #region Комментарии к гриду

        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, 0);
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != string.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
        }

        private static string GetStringDTValue(DataTable dt, string columnName)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != string.Empty)
            {
                return dt.Rows[0][columnName].ToString();
            }
            return string.Empty;
        }

        private void CommentTextDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0016_0003_commentText");
            dtCommentText = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtCommentText);

            CommentText.Text = string.Empty;
            if (dtCommentText.Rows.Count > 0)
            {
                double moGroup = GetDoubleDTValue(dtCommentText, "Группа МО");
                string regionName = GetStringDTValue(dtCommentText, "Район");
                double crimeCount = GetDoubleDTValue(dtCommentText, "Общее количество нарушений");
                
                if (regionName != string.Empty)
                {
                    PageSubTitle.Text = PageSubTitle.Text.Replace(ComboRegion.SelectedValue,
                        string.Format("{0} ({1})", ComboRegion.SelectedValue, regionName));
                }

                CommentText.Text = string.Format(
                    "<span style='padding-left:15px;'><b>Группа МО:</b>&nbsp;{0}</span><br/>" +
                    "<span style='padding-left:15px;'><b>Общее количество нарушений:</b>&nbsp;{1}</span><br/>",
                        moGroup, crimeCount);

            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;

            e.CurrentWorksheet.Rows[2].Cells[0].Value = CommentTextLabel.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;
            int rowsCount = UltraWebGrid.Rows.Count;

            e.CurrentWorksheet.Columns[0].CellFormat.FormatString = "";
            e.CurrentWorksheet.Columns[0].Width = 150 * 37;

            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[3].Height = 15 * 37;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }

            // расставляем стили у начальных колонок
            for (int i = 4; i < rowsCount + 4; i++)
            {
                for (int j = 1; j < columnCount; j++)
                {
                    e.CurrentWorksheet.Rows[i].Height = 20*37;
                    e.CurrentWorksheet.Rows[i].Cells[j].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                    e.CurrentWorksheet.Rows[i].Cells[j].CellFormat.Alignment = HorizontalCellAlignment.Left;
                    e.CurrentWorksheet.Rows[i].Cells[j].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                }
            }

            for (int i = 1; i < columnCount; i = i + 1)
            {
                string formatString = "";
                int width = 100;

                switch (i)
                {
                    case 1:
                        {
                            width = 600;
                            break;
                        }
                    case 2:
                    case 3:
                        {
                            width = 120;
                            formatString = "#,##0.00;[Red]-#,##0.00";
                            break;
                        }
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = width * 37;
            }

            e.CurrentWorksheet.Rows[rowsCount + 6].Height = 20 * 37;
            e.CurrentWorksheet.Rows[rowsCount + 6].Cells[0].Value = CommentTextExportsReplaces(CommentText.Text);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
        }

        private static string CommentTextExportsReplaces(string source)
        {
            string commentText = source;

            commentText = commentText.Replace("<\n>", "");
            commentText = commentText.Replace("<\r>", "");
            commentText = commentText.Replace(@"
", " ");
            commentText = commentText.Replace("&nbsp;", " ");
            commentText = commentText.Replace("<br/>", "\n");
            commentText = commentText.Replace("<b>", "");
            commentText = commentText.Replace("</b>", "");
            commentText = commentText.Replace("<span style='padding-left:15px;'>", "  ");
            commentText = commentText.Replace("</span>", "");
            return commentText;
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
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(CommentTextExportsReplaces(CommentTextLabel.Text));

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(CommentTextExportsReplaces(CommentText.Text));
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
        }

        #endregion
    }
}

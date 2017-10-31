using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.MFRF_0004_0001
{
    public partial class DefaultAppropriation : CustomReportPage
    {
        private DataTable dtGrid = new DataTable();
        private DataTable dtDetails = new DataTable();
        private int year = 2008;
        private string month = "Декабрь";

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 270);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 10);

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);

            UltraGridExporter1.ExcelExporter.RowExporting += new RowExportingEventHandler(ExcelExporter_RowExporting);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            PageTitle.Text = string.Format("Оценка объема неисполненных бюджетных ассигнований ГРБС за {0} год", year);
            Page.Title = PageTitle.Text;
            PageSubTitle1.Text = "Оценка исполнения бюджета ГРБС в части расходов согласно Положению об организации проведения мониторинга качества финансового менеджмента, осуществляемого ГРБС";

            UserParams.PeriodMonth.Value = month;
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(month)));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month)));

            UltraWebGrid.DataBind();
            DetailsDataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("MFRF_0004_0001_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtGrid);

            foreach (DataRow row in dtGrid.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value && (i == 2 || i == 3))
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }
                }
            }

            UltraWebGrid.DataSource = dtGrid;
        }

        private void DetailsDataBind()
        {
            string query = DataProvider.GetQueryText("MFRF_0004_0001_details");
            dtDetails = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtDetails);

            int countGRBS = dtDetails.Rows[0][5] != DBNull.Value ? Convert.ToInt32(dtDetails.Rows[0][5]) : 0;

            int violation123nCount = dtDetails.Rows[0][0] != DBNull.Value ? Convert.ToInt32(dtDetails.Rows[0][0]) : 0;
            ViolationCount123nLabel.Text = string.Format("Количество нарушителей согласно Приказу 123н: <b>{0}</b> из <b>{1}</b> оцененных", violation123nCount, countGRBS);

            int violation34nCount = dtDetails.Rows[0][1] != DBNull.Value ? Convert.ToInt32(dtDetails.Rows[0][1]) : 0;
            ViolationCount34nLabel.Text = string.Format("Количество нарушителей согласно Приказу 34н: <b>{0}</b> из <b>{1}</b> оцененных", violation34nCount, countGRBS);

            double avgAppropriation = dtDetails.Rows[0][2] != DBNull.Value ? Convert.ToDouble(dtDetails.Rows[0][2]) : 0;
            AVGAppropiationLabel.Text = string.Format("Средний объем неисполненных бюджетных ассигнований: <b>{0:P2}</b>", avgAppropriation);

            double maxAppropriation = dtDetails.Rows[0][3] != DBNull.Value ? Convert.ToDouble(dtDetails.Rows[0][3]) : 0;
            string maxAppropriationGRBS = dtDetails.Rows[0][4] != DBNull.Value ? dtDetails.Rows[0][4].ToString() : string.Empty;
            MaxAppropiationLabel.Text = string.Format("Максимальный объем неисполненных бюджетных ассигнований: <b>{0:P2} ({1})</b>", maxAppropriation, DataDictionariesHelper.GetShortGRBSName(maxAppropriationGRBS));
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(530);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 100;

                switch (i)
                {
                    case 1:
                        {
                            formatString = "N0";
                            widthColumn = 48;
                            break;
                        }
                    case 2:
                    case 3:
                        {
                            formatString = "N3";
                            widthColumn = 113;
                            break;
                        }
                    case 4:
                        {
                            formatString = "P2";
                            widthColumn = 118;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
            }

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "Наименование ГРБС ФБ", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Код", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, string.Format("Бюджетные ассигнования на {0} год, млн.руб.", year), "Объем бюджетных ассигнований ГРБС в отчетном\nфинансовом году согласно бюджетной росписи");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, string.Format("Кассовое исполнение за {0} год, млн.руб.", year), "Кассовое исполнение расходов ГРБС в отчетном году");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "Объем неисполненных бюджетных ассигнований", "Объем неисполненных бюджетных ассигнований\nна конец отчетного финансового года");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "Целевой ориентир - объем неисполненных ассигнований не более 0,5%", "Согласно действующему Приказу Минфина РФ №123н от 10 декабря 2007 года");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, "Целевой ориентир - объем неисполненных ассигнований не более 0%", "Согласно Приказу Минфина РФ №34н от 13 апреля 2009 года");
        }

        /// <summary>
        /// Получение фоновой картинки для ячейки по значению
        /// </summary>
        /// <param name="cellValue">значение ячейки</param>
        /// <param name="hintText">хинт к ячейке</param>
        /// <param name="brightColor">использовать ли яркие цвета</param>
        /// <returns>ссылка на фоновую картинку ячейки</returns>
        private static string GetIndicatorStyle(string cellValue, ref string hintText, bool brightColor)
        {
            string dim = (!brightColor) ? "dim" : string.Empty;
            string colorStyle = string.Empty;

            switch (cellValue)
            {
                case "Соответствует":
                    {
                        colorStyle = "ballGreenBB";
                        hintText = string.Format("Соответствует критерию (Приказ №{0}н)", brightColor ? 123 : 34);
                        break;
                    }
                case "Не соответствует":
                    {
                        colorStyle = "ballRedBB";
                        hintText = string.Format("Не соответствует критерию (Приказ №{0}н)", brightColor ? 123 : 34);
                        break;
                    }
                case "Исключение":
                    {
                        colorStyle = "ballYellowBB";
                        hintText = "Кассовое исполнение превысило объем бюджетных ассигнований";
                        break;
                    }
                default:
                    {
                        return string.Empty;
                    }
            }
            
            return string.Format("~/images/{0}{1}.png", colorStyle, dim);
        }
        
        /// <summary>
        /// Получение текста индикатора по фоновой картинке
        /// </summary>
        /// <param name="backgroundImage">фоновая картинка</param>
        /// <returns>текст индикатора</returns>
        private static string GetIndicatorText(string backgroundImage)
        {
            if (backgroundImage.Contains("ballGreenBB"))
            {
                return "Соответствует";
            }

            if (backgroundImage.Contains("ballRedBB"))
            {
                return "Не соответствует";
            }

            if (backgroundImage.Contains("ballYellowBB"))
            {
                 return "Исключение";
            }

            return string.Empty;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() == "Всего по ГРБС")
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                    if (i == 1 || i == 5 || i == 6)
                    {
                        e.Row.Cells[i].Value = string.Empty;
                    }
                }

                if (e.Row.Cells[i].Value != null && (i == 5 || i == 6))
                {
                    string hint = string.Empty;
                    e.Row.Cells[i].Style.BackgroundImage = GetIndicatorStyle(e.Row.Cells[i].Value.ToString(), ref hint, i == 5);
                    e.Row.Cells[i].Title = hint;
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: center center; margin: 2px";

                    if (e.Row.Cells[i].Style.BackgroundImage.Contains("Red"))
                    {
                        e.Row.Cells[i].Value = "  ";
                    }
                    else if (e.Row.Cells[i].Style.BackgroundImage.Contains("Green"))
                    {
                        e.Row.Cells[i].Value = " ";
                    }
                    else
                    {
                        e.Row.Cells[i].Value = string.Empty;
                    }
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
            UltraWebGrid.Bands[0].Columns[4].Hidden = true;
            UltraWebGrid.Bands[0].Columns[5].Hidden = true;

            UltraWebGrid.Bands[0].Columns[6].Header.Caption = "Объем неиспользованных бюджетных ассигнований";

            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle1.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            for (int i = 0; i < UltraWebGrid.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "#,##0";
                int widthColumn = 200;

                switch (i)
                {
                    case 1:
                        {
                            formatString = "#,##0";
                            widthColumn = 50;
                            break;
                        }
                    case 2:
                    case 3:
                        {
                            formatString = "#,##0.000;[Red]-#,##0.000";
                            widthColumn = 100;
                            break;
                        }
                    case 4:
                        {
                            formatString = UltraGridExporter.ExelPercentFormat;
                            widthColumn = 90;
                            break;
                        }
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
            }
        }

        private void ExcelExporter_RowExporting(object sender, RowExportingEventArgs e)
        {
            e.CurrentWorksheet.Rows[e.GridRow.Index + 4].Cells[5].Value = GetIndicatorText(e.GridRow.Cells[5].Style.BackgroundImage);
            e.CurrentWorksheet.Rows[e.GridRow.Index + 4].Cells[6].Value = GetIndicatorText(e.GridRow.Cells[6].Style.BackgroundImage);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        #endregion

        #region Экспорт в Pdf

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
            title.AddContent(PageSubTitle1.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(ViolationCount123nLabel.Text.Replace("<b>", "").Replace("</b>", ""));

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(ViolationCount34nLabel.Text.Replace("<b>", "").Replace("</b>", ""));

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(AVGAppropiationLabel.Text.Replace("<b>", "").Replace("</b>", ""));

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(MaxAppropiationLabel.Text.Replace("<b>", "").Replace("</b>", ""));
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        #endregion
    }
}

using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0005
{
    public partial class DefaultDetail_budget : CustomReportPage
    {
        private DataTable dtGrid = new DataTable();
        private DataTable dtDate = new DataTable();
        private int firstYear = 2008;
        private int endYear = 2011;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 240);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);

            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.MultiHeader = true;

            UltraGridExporter1.PdfExportButton.Visible = true;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (Page.IsPostBack)
            {
                return;
            }

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0005_date_budget");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            endYear = Convert.ToInt32(dtDate.Rows[0][0]);

            string endMonth =  dtDate.Rows[0][3].ToString();
            string monthStr = (endMonth == "Январь") ?
                endMonth.ToLower() : string.Format("январь - {0}", endMonth.ToLower());

            Page.Title = "Группы субъектов по доле межбюджетных трансфертов";
            PageTitle.Text = Page.Title;
            PageSubTitle1.Text = "В соответствии со статьей 130 Бюджетного кодекса РФ";
            PageSubTitle2.Text = string.Format("Распределение субъектов в группы по доле межбюджетных трансфертов из федерального бюджета (за исключением субвенций) в собственных доходах консолидированного бюджета в течение двух из трех последних отчетных финансовых лет.<br/>" +
            "Фактические показатели {0} года приведены по итогам исполнения за {1}.", endYear, monthStr);

            group1.Text = "Группа 1 <img src=\"../../images/ballRedBB.png\"> доля более 60%";
            group2.Text = "Группа 2 <img src=\"../../images/ballOrangeBB.png\"> доля от 20% до 60%";
            group3.Text = "Группа 3 <img src=\"../../images/ballYellowBB.png\"> доля от 5% до 20%";
            group4.Text = "Группа 4 <img src=\"../../images/ballGreenBB.png\"> доля менее 5%";

            UserParams.PeriodFirstYear.Value = firstYear.ToString();
            UserParams.PeriodEndYear.Value = endYear.ToString();

            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0005_grid_budget");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);

            foreach (DataRow row in dtGrid.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (i >= 2 && i < 12 && row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }

                    bool isFO = row[0] != DBNull.Value && RegionsNamingHelper.IsFO(row[0].ToString());

                    if (!isFO && (i >= 17 && i <= 21) && row[i - 5] != DBNull.Value && row[i - 5].ToString() != string.Empty)
                    {
                        row[i] = GetGroupNumber(Convert.ToDouble(row[i - 5].ToString()));
                    }
                }
            }

            UltraWebGrid.DataSource = dtGrid;
        }

        /// <summary>
        /// Получение номера сложившейся группы
        /// </summary>
        /// <param name="value">доля</param>
        /// <returns>номер группы</returns>
        private static int GetGroupNumber(double value)
        {
            if (value > 0.6)
            {
                return 1;
            }
            if (value < 0.05)
            {
                return 4;
            }
            return (value > 0.2) ? 2 : 3;
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (Page.IsPostBack)
                return;

            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            UltraGridColumn column = new UltraGridColumn();
            column.Header.Caption = "Присво- енная группа";
            UltraWebGrid.Columns.Add(column);
            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 60;

                int j = (i - 2) / 5;

                switch (j)
                {
                    case 0:
                        {
                            formatString = "N2";
                            widthColumn = 59;
                            break;
                        }
                    case 1:
                        {
                            formatString = "N2";
                            widthColumn = 58;
                            break;
                        }
                    case 2:
                        {
                            formatString = "P2";
                            widthColumn = 46;
                            break;
                        }
                    case 3:
                        {
                            formatString = "N0";
                            widthColumn = i == 17 ? 33 : 40;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);

                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(190);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Header.Caption = "ФО";
            e.Layout.Bands[0].Columns[1].Header.Title = "Федеральный округ, которому принадлежит субъект";
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(38);
            e.Layout.Bands[0].Columns[22].Width = CRHelper.GetColumnWidth(49);

            // удаляем ненужные колонки
            e.Layout.Bands[0].Columns.RemoveAt(2);
            e.Layout.Bands[0].Columns.RemoveAt(6);
            e.Layout.Bands[0].Columns.RemoveAt(10);

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 1 || i == 19)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 2;

            for (int i = 2; i < 18; i = i + 4)
            {
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');

                int year = (i == 14) ? 2008 : 2009;
                int spanX = (i == 14) ? 5 : 4;
                                                
                if (i == 14)
                {
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, string.Format("{0}", year), "");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, string.Format("{0}", year + 1), "");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, string.Format("{0}", year + 2), "");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, string.Format("{0} факт", year + 3), "");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 4, string.Format("{0} план", year + 3), "");
                }
                else
                {
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, string.Format("{0}", year), "");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, string.Format("{0}", year + 1), "");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, string.Format("{0} факт", year + 2), "");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, string.Format("{0} план", year + 2), "");
                }

                ColumnHeader ch = CRHelper.AddHierarchyHeader(e.Layout.Grid,
                    0,
                    captions[0].TrimEnd('_'),
                    multiHeaderPos,
                    0,
                    spanX,
                    1);

                multiHeaderPos += spanX;

                string hint = string.Empty;
                int j = (i - 2) / 4;
                switch (j)
                {
                    case 0:
                        {
                            hint = "Собственные доходы консолидированного бюджета субъекта, определяемые в соответствии со статьей 47 Бюджетного кодекса РФ.";
                            break;
                        }
                    case 2:
                        {
                            hint = "Межбюджетные трансферты из федерального бюджета (за исключением субвенций)";
                            break;
                        }
                    case 3:
                        {
                            hint = "Группа, сложившаяся по доле в отдельных годах";
                            break;
                        }
               }

                ch.Title = hint;
            }

            e.Layout.Bands[0].Columns[1].Header.Title = "Федеральный округ, к которому принадлежит субъект";
            e.Layout.Bands[0].Columns[19].Header.Title = string.Format("Группа, присвоенная по доле в течение двух из трех последних отчетных финансовых лет (по {0}-{1} годам)",
                UserParams.PeriodFirstYear.Value, Convert.ToInt32(UserParams.PeriodEndYear.Value) - 1);
        }

        /// <summary>
        /// Получение присвоенной группы
        /// </summary>
        private static int GetAssignGroup(int p1, int p2, int p3)
        {
            if (p1 == p2 && p2 == p3)
            {
                return p1;
            }

            if (p1 == p2 || p2 == p3)
            {
                return p2;
            }

            if (p1 == p2 || p1 == p3)
            {
                return p1;
            }

            if (p1 == p3 || p2 == p3)
            {
                return p3;
            }

            return Convert.ToInt32(Math.Round(Convert.ToDecimal(p1 + p2 + p3) / 3));
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[14].Value != null && e.Row.Cells[15].Value != null && e.Row.Cells[16].Value != null)
            {
                e.Row.Cells[19].Value = GetAssignGroup(
                        Convert.ToInt32(e.Row.Cells[14].Value),
                        Convert.ToInt32(e.Row.Cells[15].Value),
                        Convert.ToInt32(e.Row.Cells[16].Value));

                switch (e.Row.Cells[19].Value.ToString())
                {
                    case "1":
                        {
                            e.Row.Cells[19].Style.BackgroundImage = "~/images/ballRedBB.png";
                            break;
                        }
                    case "2":
                        {
                            e.Row.Cells[19].Style.BackgroundImage = "~/images/ballOrangeBB.png";
                            break;
                        }
                    case "3":
                        {
                            e.Row.Cells[19].Style.BackgroundImage = "~/images/ballYellowBB.png";
                            break;
                        }
                    case "4":
                        {
                            e.Row.Cells[19].Style.BackgroundImage = "~/images/ballGreenBB.png";
                            break;
                        }
                }
                e.Row.Cells[19].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.Padding.Left = 1;
                e.Row.Cells[i].Style.Padding.Right = i < 15 ? 3 : 5;

                if (i >= 15 && i < 19 && e.Row.Cells[i].Value != null)
                {
                    int k = (i == 18) ? i - 2 : i - 1;

                    Int32 value;
                    
                    int curValue = 0;
                    if (e.Row.Cells[i].Value != null && Int32.TryParse(e.Row.Cells[i].Value.ToString(), NumberStyles.Any, NumberFormatInfo.CurrentInfo,
                                             out value))
                    {
                        curValue = Convert.ToInt32(e.Row.Cells[i].Value.ToString());
                    }

                    int prevValue = 0;
                    if (e.Row.Cells[k].Value != null && Int32.TryParse(e.Row.Cells[k].Value.ToString(), NumberStyles.Any, NumberFormatInfo.CurrentInfo,
                                             out value))
                    {
                        prevValue = Convert.ToInt32(e.Row.Cells[k].Value.ToString());
                    }


                    if (curValue > prevValue)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                    }
                    else if (curValue < prevValue)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
                
                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() != string.Empty)
                {
                    if (!RegionsNamingHelper.IsSubject(e.Row.Cells[0].Value.ToString()))
                    {
                        if (i == 0 || i == 1)
                        {
                            e.Row.Cells[i].Style.Font.Bold = true;
                        }
                        else
                        {
                            e.Row.Cells[i].Value = string.Empty;
                        }
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

        #region Экпорт в EXCEL

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle1.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            for (int i = 2; i < UltraWebGrid.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "#,##0";
                int widthColumn = 80;

                int j = (i - 2) / (i > 17 ? 5 : 4);

                switch (j)
                {
                    case 0:
                        {
                            formatString = UltraGridExporter.ExelNumericFormat;
                            widthColumn = 90;
                            break;
                        }
                    case 1:
                        {
                            formatString = UltraGridExporter.ExelNumericFormat;
                            widthColumn = 90;
                            break;
                        }
                    case 2:
                        {
                            formatString = UltraGridExporter.ExelPercentFormat;
                            widthColumn = 90;
                            break;
                        }
                    case 3:
                        {
                            formatString = "#,##0";
                            widthColumn = 72;
                            break;
                        }
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
            }
            e.CurrentWorksheet.Columns[1].Width = 40 * 37;

            e.CurrentWorksheet.Rows[UltraGridExporter1.ExcelExporter.ExcelStartRow].CellFormat.FormatString = "0";
        }

        private int offset = 0;

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            UltraGridColumn col = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            e.HeaderText = col.Header.Key.Split(';')[0];
            if (col.Hidden)
            {
                offset++;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        #endregion

        #region Экпорт в PDF

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
            title.AddContent(PageSubTitle2.Text.Replace("<br/>", "\n"));

            ITable table = e.Section.AddTable();
            ITableRow row = table.AddRow();
            ITableCell cell = row.AddCell();
            cell.Height = new FixedHeight(10);

            row = table.AddRow();
            AddTextCell(row, "Группа 1 ", 60);
            AddImage("~/images/ballRedBB.png", row);
            AddTextCell(row, " доля более 60%", 140);

            AddTextCell(row, "Группа 2 ", 60);
            AddImage("~/images/ballOrangeBB.png", row);
            AddTextCell(row, " доля от 20% до 60%", 160);

            AddTextCell(row, "Группа 3 ", 60);
            AddImage("~/images/ballYellowBB.png", row);
            AddTextCell(row, " доля от 5% до 20%", 150);

            AddTextCell(row, "Группа 4 ", 60);
            AddImage("~/images/ballGreenBB.png", row);
            AddTextCell(row, " доля менее 5%", 140);
        }

        private static void AddImage(string imagePath, ITableRow row)
        {
            ITableCell cell = row.AddCell();
            cell.AddImage(UltraGridExporter.GetImage(imagePath));
            cell.Width = new FixedWidth(20);
        }

        private static void AddTextCell(ITableRow row, string text, int cellWidth)
        {
            ITableCell cell = row.AddCell();
            IText title = cell.AddText();
            cell.Width = new FixedWidth(cellWidth);
            Font font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(text);
            title.Alignment = TextAlignment.Left;
            title.KeepSolid = true;

        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

//            List<int> hideColumns = new List<int>();
//
//            for (int i = 0; i < UltraWebGrid.Columns.Count; i++)
//            {
//                UltraGridColumn column = UltraWebGrid.Columns[i];
//                if (column.Hidden)
//                {
//                    hideColumns.Add(i - hideColumns.Count);
//                }
//            }
//
//            foreach (int index in hideColumns)
//            {
//                UltraWebGrid.Columns.RemoveAt(index);
//            }

            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        #endregion 
    }
}

using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0005
{
    public partial class DefaultDetail : CustomReportPage
    {
        private DataTable dtGrid = new DataTable();
        private DataTable dtDate = new DataTable();
        private int firstYear = 2008;
        private int endYear = 2011;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 245);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);

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


        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (Page.IsPostBack)
            {
                return;
            }

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0005_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            endYear = Convert.ToInt32(dtDate.Rows[0][0]);
            //endYear = 2009;

            string endMonth = dtDate.Rows[0][3].ToString();
            string monthStr = (endMonth == "Январь") ?
                endMonth.ToLower() : string.Format("январь - {0}", endMonth.ToLower());

            Page.Title = "Группы субъектов по доле межбюджетных трансфертов";
            PageTitle.Text = Page.Title;
            PageSubTitle1.Text = "В соответствии со статьей 130 Бюджетного кодекса РФ";
            PageSubTitle2.Text = string.Format("Распределение субъектов в группы по доле межбюджетных трансфертов из федерального бюджета (за исключением субвенций) в собственных доходах консолидированного бюджета в течение двух из трех последних отчетных финансовых лет.<br/>" +
            "Фактические показатели {0} года приведены по итогам исполнения за {1}.", endYear, monthStr);

            group1.Text = "Группа 1 <img src=\"../../images/ballRedBB.png\"> доля более 60%";
            group2.Text = "Группа 2 <img src=\"../../images/ballOrangeBB.png\"> доля от 20% до 60%";
            group3.Text = "Группа 3 <img src=\"../../images/ballYellowBB.png\"> доля от 20% до 5%";
            group4.Text = "Группа 4 <img src=\"../../images/ballGreenBB.png\"> доля менее 5%";

            UserParams.PeriodFirstYear.Value = firstYear.ToString();
            UserParams.PeriodEndYear.Value = endYear.ToString();

            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0005_grid");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);

            foreach (DataRow row in dtGrid.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (i >= 12 && row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }

                    if ((i >= 2 && i <= 6) && row[i + 5] != DBNull.Value && row[i + 5].ToString() != string.Empty)
                    {
                        row[i] = GetGroupNumber(Convert.ToDouble(row[i + 5].ToString()));
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
            //            UltraWebGrid.Height = Unit.Empty;
            //            UltraWebGrid.Width = Unit.Empty;
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
            UltraWebGrid.Columns.Insert(2, column);
            int expandWidth = 1462;
            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 60;

                int j = (i - 3) / 5;

                switch (j)
                {
                    case 0:
                        {
                            formatString = "N0";
                            widthColumn = 42;
                            break;
                        }
                    case 1:
                        {
                            formatString = "P2";
                            widthColumn = 51;
                            break;
                        }
                    case 2:
                        {
                            formatString = "N2";
                            widthColumn = 60;
                            break;
                        }
                    case 3:
                        {
                            formatString = "N2";
                            widthColumn = 60;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);

                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn, expandWidth);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(202, expandWidth);

            e.Layout.Bands[0].Columns[1].Header.Caption = "Федер. округ";
            e.Layout.Bands[0].Columns[1].Header.Title = "Федеральный округ, которому принадлежит субъект";
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(43, expandWidth);

            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(50, expandWidth);

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 1 || i == 2)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 3;

            for (int i = 3; i < e.Layout.Bands[0].Columns.Count; i = i + 5)
            {
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "2008", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "2009", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, "2010", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, "2011 факт", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 4, "2011 план", "");

                ColumnHeader ch = CRHelper.AddHierarchyHeader(e.Layout.Grid,
                    0,
                    captions[0].TrimEnd('_'),
                    multiHeaderPos,
                    0,
                    5,
                    1);

                multiHeaderPos += 5;

                string hint = string.Empty;
                int j = (i - 3) / 5;
                switch (j)
                {
                    case 0:
                        {
                            hint = "Группа, сложившаяся по доле в отдельных годах";
                            break;
                        }
                    case 2:
                        {
                            hint = "Межбюджетные трансферты из федерального бюджета (за исключением субвенций)";
                            break;
                        }
                    case 3:
                        {
                            hint = "Собственные доходы консолидированного бюджета субъекта, определяемые в соответствии со статьей 47 Бюджетного кодекса РФ.";
                            break;
                        }
                }

                ch.Title = hint;
            }

            e.Layout.Bands[0].Columns[1].Header.Title = "Федеральный округ, к которому принадлежит субъект";
            e.Layout.Bands[0].Columns[2].Header.Title = string.Format("Группа, присвоенная по доле в течение двух из трех последних отчетных финансовых лет (по {0}-{1} годам)",
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
            if (e.Row.Cells[3].Value != null && e.Row.Cells[4].Value != null && e.Row.Cells[5].Value != null)
            {
                e.Row.Cells[2].Value = GetAssignGroup(
                        Convert.ToInt32(e.Row.Cells[3].Value),
                        Convert.ToInt32(e.Row.Cells[4].Value),
                        Convert.ToInt32(e.Row.Cells[5].Value));

                switch (e.Row.Cells[2].Value.ToString())
                {
                    case "1":
                        {
                            e.Row.Cells[2].Style.BackgroundImage = "~/images/ballRedBB.png";
                            break;
                        }
                    case "2":
                        {
                            e.Row.Cells[2].Style.BackgroundImage = "~/images/ballOrangeBB.png";
                            break;
                        }
                    case "3":
                        {
                            e.Row.Cells[2].Style.BackgroundImage = "~/images/ballYellowBB.png";
                            break;

                        }
                    case "4":
                        {
                            e.Row.Cells[2].Style.BackgroundImage = "~/images/ballGreenBB.png";
                            break;
                        }
                }
                e.Row.Cells[2].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (i >= 4 && i < 8)
                {
                    int k = (i == 7) ? i - 2 : i - 1;
                    int curValue = Convert.ToInt32(e.Row.Cells[i].Value.ToString());
                    int prevValue = Convert.ToInt32(e.Row.Cells[k].Value.ToString());

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

                UltraGridCell cell = e.Row.Cells[i];
                if (cell != null && cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    decimal value;
                    try
                    {
                        if (
                            decimal.TryParse(cell.Value.ToString(), NumberStyles.Any, NumberFormatInfo.CurrentInfo,
                                             out value))
                        {
                            if (value < 0)
                            {
                                cell.Style.ForeColor = Color.Red;
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

        #endregion

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle1.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle2.Text.Replace("<br/>", " ");
            e.CurrentWorksheet.Rows[2].Cells[0].Value = group1.Text.Replace("<img src=\"../../images/ballRedBB.png\">", " - ") + ". " + group2.Text.Replace("<img src=\"../../images/ballOrangeBB.png\">", " - ") + ". " + group3.Text.Replace("<img src=\"../../images/ballYellowBB.png\">", " - ") + ". " + group4.Text.Replace("<img src=\"../../images/ballGreenBB.png\">", " - ") + ".";
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            for (int i = 2; i < UltraWebGrid.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "#,##0";
                int widthColumn = 80;

                int j = (i - 3) / 5;

                switch (j)
                {
                    case 0:
                        {
                            formatString = "#,##0";
                            widthColumn = 72;
                            break;
                        }
                    case 1:
                        {
                            formatString = UltraGridExporter.ExelPercentFormat;
                            widthColumn = 81;
                            break;
                        }
                    case 2:
                        {
                            formatString = UltraGridExporter.ExelNumericFormat;
                            widthColumn = 90;
                            break;
                        }
                    case 3:
                        {
                            formatString = "#,##0.00;[Red]-#,##0.00";
                            widthColumn = 90;
                            break;
                        }
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
            }
            e.CurrentWorksheet.Rows[UltraGridExporter1.ExcelExporter.ExcelStartRow].CellFormat.FormatString = "0";
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
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
            title.AddContent(PageSubTitle1.Text);

            //title = e.Section.AddText();
            //font = new Font("Verdana", 12);
            //title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            //title.AddContent(PageSubTitle2.Text.Replace("<br/>", "\n"));

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
            AddTextCell(row, " доля от 20% до 5%", 150);

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
            UltraGridExporter1.GridElementCaption = PageSubTitle2.Text.Replace("<br/>", "\n");
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }
    }
}

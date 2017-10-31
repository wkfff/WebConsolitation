using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Drawing;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Dundas.Maps.WebControl;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report;


namespace Krista.FM.Server.Dashboards.reports.MO.MO_0002._0004
{
    public partial class dafault1 : CustomReportPage
    {
        // --------------------------------------------------------------------

        // параметр для последней актуальной даты
        private CustomParam last_year { get { return (UserParams.CustomParam("last_year")); } }
        // параметр запроса для последней актуальной даты
        private CustomParam region { get { return (UserParams.CustomParam("region")); } }
        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }

        private CustomParam oneV { get { return (UserParams.CustomParam("oneV")); } }
        // выбранный столбец
        private CustomParam gridColumnSelected { get { return (UserParams.CustomParam("gridColumnSelected")); } }

        private CustomParam position { get { return (UserParams.CustomParam("position")); } }

        private string[] heder4 = { "Доля неэффективных расходов, (%)",
                                    "Объем неэффективных расходов, (тыс. руб.)",
                                    "Уровень эффективности расходования бюджетных средств",
                                    "Ранг" };
        private string[] heder = {  "Здоровье",
                                    "Образование (общее)",
                                    "Жилищно-коммунальное хозяйство",
                                    "Организация муниципального управления" };

        // ширина экрана в пикселях
        private Int32 screen_width { get { return (int)Session["width_size"]; } }
        // --------------------------------------------------------------------

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                base.Page_PreLoad(sender, e);
                // установка размеров
                //web_grid1.Width = (int)((screen_width - 20));
                web_grid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 7);

                //Chart1.Width = (int)((screen_width - 20));
                Chart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);

                UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
                UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
                UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
                UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

                UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
                UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                        <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
                UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
                UltraGridExporter1.MultiHeader = true;
            }
            catch (Exception)
            {
                // установка размеров не удалась ...
            }
        }

        // --------------------------------------------------------------------
        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                base.Page_Load(sender, e);
                if (!Page.IsPostBack)
                {   // опрерации которые должны выполняться при только первой загрузке страницы
                    RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(web_grid1);
                    #region Информация
                    string innerText = string.Empty;
                    try
                    {
                        string filePath = HttpContext.Current.Server.MapPath("defaultCity.html");
                        System.IO.StreamReader sr = new System.IO.StreamReader(filePath);
                        string line;
                        while ((line = sr.ReadLine()) != null)
                            innerText = innerText + line;
                    }
                    catch (Exception) { }

                    Control container = Page.LoadControl("~/Components/ContainerPanel.ascx");
                    HtmlTable htmlTable = new HtmlTable();
                    HtmlTableRow htmlRow = new HtmlTableRow();
                    HtmlTableCell htmlCell = new HtmlTableCell();
                    Label shortBody = new Label();
                    shortBody.Text = innerText;
                    shortBody.CssClass = "ReportDescription";
                    htmlCell.Controls.Add(shortBody);
                    htmlRow.Cells.Add(htmlCell);
                    htmlTable.Rows.Add(htmlRow);

                    ((ContainerPanel)container).AddContent(htmlTable);
                    ((ContainerPanel)container).AddHeader("Информация");
                    ContactInformationPlaceHolder.Controls.Add(container);
                    #endregion
                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                    last_year.Value = UserComboBox.getLastBlock(getLastDate());



                    Collection<string> years = new Collection<string>();
                    //years.Add((Convert.ToInt16(last_year.Value) - 1).ToString());
                    years.Add(last_year.Value);

                    ComboYear.Title = "Год";
                    ComboYear.Width = 100;
                    ComboYear.MultiSelect = false;
                    ComboYear.FillValues(getDates("dates"));
                    ComboYear.SetСheckedState(last_year.Value, true);

                    gridColumnSelected.Value = "1";
                }

                last_year.Value = ComboYear.SelectedValue;
                web_grid1.DataBind();

                //if (!Page.IsPostBack)
                if (web_grid1.Rows.Count > 0)
                    grid1Manual_ActiveCellChange(Convert.ToInt16(gridColumnSelected.Value.ToString()));
            }
            catch (Exception ex)
            {
                // неудачная загрузка ...
                throw new Exception(ex.Message, ex);
            }
        }

        public static Collection<string> getDates(string q)
        {
            CellSet CS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(q));
            Collection<string> C = new Collection<string>();
            for (int i = 0; i < CS.Axes[1].Positions.Count; i++)
            {
                C.Add(CS.Axes[1].Positions[i].Members[0].Caption);
            }
            return C;
        }

        // --------------------------------------------------------------------

        /** <summary>
         *  Метод получения последней актуальной даты 
         *  </summary>
         */
        private String getLastDate()
        {
            try
            {
                CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("last_year"));
                return cs.Axes[1].Positions[0].Members[0].ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
                //return null;
            }
        }


        protected void web_grid1_DataBinding(object sender, EventArgs e)
        {
            try
            {
                CellSet grid_set = null;
                DataTable grid_table = new DataTable();
                // Загрузка таблицы цен и товаров в CellSet
                grid_set = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid2"));
                // Добавление столбцов в таблицу и заполнение данных в DataTable
                int columnsCount = grid_set.Axes[0].Positions.Count;
                int rowsCount = grid_set.Axes[1].Positions.Count;
                if ((columnsCount == 0) & (rowsCount == 0)) return;

                grid_table.Columns.Add("Город");

                for (int i = 1; i < columnsCount + 1; i++)
                {
                    grid_table.Columns.Add(grid_set.Axes[0].Positions[i - 1].Members[0].Caption.Replace("\"", "&quot;"));
                    grid_table.Columns[grid_table.Columns.Count - 1].Caption = grid_set.Axes[0].Positions[i - 1].Members[0].Caption;
                    if ((i % 3 == 0) & (i > 0))
                    {
                        grid_table.Columns.Add();
                        grid_table.Columns[grid_table.Columns.Count - 1].Caption = "Ранг";
                    }
                }

                object[,] cells = new object[rowsCount, columnsCount + 5];

                for (int i = 0; i < rowsCount; ++i)
                {
                    int k = 0;
                    cells[i, 0] = grid_set.Axes[1].Positions[i].Members[0].Caption.Replace("муниципальный район", "район"); ;
                    for (int j = 0; j < columnsCount; ++j)
                    {
                        if ((j % 3 == 0) & (j > 0)) k = k + 1;
                        cells[i, j + k + 1] = grid_set.Cells[j, i].Value;
                    }
                }

                #region Ранг
                string[,] range = new string[columnsCount + 5, rowsCount];
                for (int i = 3; i < columnsCount + 5; i = i + 4)
                {
                    Boolean[] checkedVal = new Boolean[rowsCount];
                    double realMax = 0;
                    double max = 0;
                    int rangeCount = 0;
                    int minIndex_ = 0;

                    for (int j = 0; j < rowsCount; ++j)
                    {
                        rangeCount++;
                        //поиск максимального элемента
                        Boolean first = true;
                        double min = 0;
                        int minIndex = 0;
                        for (int k = 0; k < rowsCount; ++k)
                        {
                            if (checkedVal[k]) continue;
                            if (first)
                            {
                                min = Convert.ToDouble(cells[k, i]);
                                first = false;
                                minIndex = k;
                            }
                            if (Convert.ToDouble(cells[k, i]) > min)
                            {
                                min = Convert.ToDouble(cells[k, i]);
                                minIndex = k;
                            }
                        }
                        if (!checkedVal[minIndex])
                        {
                            realMax = Convert.ToDouble(cells[minIndex, i]);
                            range[i, minIndex] = rangeCount.ToString();
                            checkedVal[minIndex] = true;
                            minIndex_ = minIndex;
                        }

                        // поиск равных элементов
                        for (int k = 0; k < rowsCount; ++k)
                        {
                            if (checkedVal[k]) continue;
                            if (Convert.ToDouble(cells[k, i]) == realMax)
                            {

                                range[i, k] = rangeCount.ToString();
                                checkedVal[k] = true;
                            }
                        }
                    }
                    string maxR = range[i, minIndex_];
                    for (int k = 0; k < rowsCount; k++)
                        if (range[i, k] == maxR)
                            range[i, k] = "+" + maxR;
                }
                #endregion

                for (int i = 0; i < rowsCount; ++i)
                    for (int j = 3; j < columnsCount + 5; j = j + 4)
                        cells[i, j + 1] = range[j, i];
    
                for (int i = 0; i < rowsCount; ++i)
                {
                    object[] values = new object[columnsCount + 5];
                    for (int j = 0; j < columnsCount + 5; ++j)
                    {
                        if ((j > 0) & ((j + 1) % 4 == 0))
                            values[j] = String.Format("{0:N4}", cells[i, j]);
                        else
                            values[j] = String.Format("{0:N2}", cells[i, j]);

                    }
                    grid_table.Rows.Add(values);
                }
                web_grid1.DataSource = grid_table.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                throw new Exception(exception.Message, exception);
            }

        }

        protected void web_grid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.Font.Bold = true;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                
                if (i % 3 == 0)
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N4");
                }
                else
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                }

                if (i % 4 == 0)
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(87 / 2);
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
                }
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(100);

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
            
            int multiHeaderPos = 1;
            int multiHeaderCount = 4;

            for (int i = multiHeaderPos; i < e.Layout.Bands[0].Columns.Count; i = i + multiHeaderCount)
            {
                ColumnHeader ch = new ColumnHeader(true);
                //string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                ch.Caption = heder[(i - 1) / multiHeaderCount];

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, heder4[0], "1");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, heder4[1], "2");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, heder4[2], "3");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, heder4[3], "4");

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += multiHeaderCount;
                ch.RowLayoutColumnInfo.SpanX = multiHeaderCount;
                ch.Style.Wrap = true;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }

        }

        protected void web_grid1_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 4; i < e.Row.Cells.Count; i += 4)
            {
                if (e.Row.Cells[i].Text == "1")
                {
                    e.Row.Cells[i - 1].Style.BackgroundImage = "~/images/greenRect.gif";
                    e.Row.Cells[i - 1].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
                if (e.Row.Cells[i].Text.Substring(0, 1) == "+")
                {
                    e.Row.Cells[i].Text = e.Row.Cells[i].Text.Substring(1);
                    e.Row.Cells[i - 1].Style.BackgroundImage = "~/images/redRect.gif";
                    e.Row.Cells[i - 1].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px; ";
                }
            }
        }


        protected void Chart1_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable grid_master = new DataTable();
                string s = DataProvider.GetQueryText("chart2");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(s, "_", grid_master);
                string subject = string.Empty;
                /*
                foreach (DataRow row in grid_master.Rows)
                {
                    subject = row[0].ToString();
                    string[] subjects = subject.Split(' ');
                    row[0] = subjects[0];
                }
                 * */
                Chart1.DataSource = grid_master.DefaultView;
            }
            catch (Exception exception) // блок для обработки исключений
            {
                // TODO: надо обработать исключение и выдать какое нибудь приемлимое сообщение
            }
        }

        protected void InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            ORG_0003_0001._default.setChartErrorFont(e);
        }

        /// <summary>
        /// Обновление данных для UltraChart при изменении активной ячейки в UltraWebGrid
        /// </summary>
        /// <param name="CellIndex">индекс активной ячейки</param>
        protected void grid1Manual_ActiveCellChange(int CellIndex)
        {
            //web_grid1.Columns[CellIndex].Selected = true;
            gridColumnSelected.Value = CellIndex.ToString();
            chart1_caption.Text = web_grid1.Columns[CellIndex].Header.Key.ToString();
            position.Value = web_grid1.Columns[CellIndex].Header.Key.Replace("&quot;", "\"");

            oneV.Value = "2";
            if ((CellIndex == 5) | (CellIndex == 10))
            {
                //oneV.Value = "1";
            }
            Chart1.DataBind();

        }

        protected void web_grid1_Click(object sender, Infragistics.WebUI.UltraWebGrid.ClickEventArgs e)
        {
            int CellIndex;
            if (e.Cell != null)
                CellIndex = e.Cell.Column.Index;
            else
                CellIndex = e.Column.Index;

            if (CellIndex > 0)
            {
                if (CellIndex % 4 == 0)
                {
                    CellIndex--;
                }
                grid1Manual_ActiveCellChange(CellIndex);
            }
            else
            {
                grid1Manual_ActiveCellChange(1);
            }
        }

        protected void web_grid1_SortColumn(object sender, SortColumnEventArgs e)
        {
            int CellIndex = e.ColumnNo;
            if (CellIndex > 0)
            {
                if (CellIndex % 4 == 0)
                {
                    CellIndex--;
                }
                grid1Manual_ActiveCellChange(CellIndex);
            }
            else
            {
                grid1Manual_ActiveCellChange(1);
            }

        }

        #region Обработчик EXCEL и PDF

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text;
            e.CurrentWorksheet.Rows[0].Cells[0].CellFormat.Font.Bold = Infragistics.Documents.Excel.ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = ComboYear.SelectedValue + " год";
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            string formatString = "#,##0.00;[Red]-#,##0.00";
            for (int i = 1; i < web_grid1.Bands[0].Columns.Count; i = i + 1)
            {
                int widthColumn = 100;
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                if (i % 4 == 0)
                    e.CurrentWorksheet.Columns[i].Width = widthColumn / 2 * 37;
                else
                    e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
            }
            e.CurrentWorksheet.Columns[0].Width = 100 * 37;
            //e.CurrentWorksheet.Rows[4].Hidden = true;
        }

        private int offset = 0;

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            UltraGridColumn col = web_grid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            //e.HeaderText = e.HeaderText.Replace("&quot;", "\"");
            //e.HeaderText = web_grid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
            int columnIndex = e.CurrentColumnIndex;
            if (columnIndex > 0)
                e.HeaderText = heder[(columnIndex - 1) / 4];
            //e.HeaderText = col.Header.Key.Split(';')[0];
            if (col.Hidden)
            {
                offset++;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(web_grid1);
        }


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(web_grid1);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(ComboYear.SelectedValue + " год");
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            ITable table = e.Section.AddTable();
            ITableRow row = table.AddRow();
            ITableCell cell = row.AddCell();

            IText title = cell.AddText();
            Font font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(" ");

            row = table.AddRow();
            cell = row.AddCell();
            title = cell.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(chart1_caption.Text.Replace("&quot;", "\""));

            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(Chart1);
            cell.AddImage(img);
            cell.Width = new FixedWidth((float)Chart1.Width.Value);

        }
        
        #endregion

    }
}

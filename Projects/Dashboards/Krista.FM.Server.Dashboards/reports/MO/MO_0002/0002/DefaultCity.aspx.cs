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

/**
 *  Оценка экономического развития в муниципальных районах Рязанской области.
 */
namespace Krista.FM.Server.Dashboards.reports.MO.MO_0002._0002_
{
    public partial class Default : CustomReportPage
    {
        // --------------------------------------------------------------------

        // параметр для последней актуальной даты
        private CustomParam last_year { get { return (UserParams.CustomParam("last_year")); } }
        // параметр запроса для последней актуальной даты
        private CustomParam region { get { return (UserParams.CustomParam("region")); } }
        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        // выбранный столбец
        private CustomParam gridColumnSelected { get { return (UserParams.CustomParam("gridColumnSelected")); } }

        private CustomParam position { get { return (UserParams.CustomParam("position")); } }

        // Заголовок страницы
        private static String headerText = "Оценка экономического развития в городских округах {0}.";

        // ширина экрана в пикселях
        private Int32 screen_width { get { return (int)Session["width_size"]; } }

        // --------------------------------------------------------------------

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                base.Page_PreLoad(sender, e);
                // установка размеров
                //web_grid1.Width = (int)((screen_width - 55));
                web_grid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 7);
                //Chart1.Width = (int)((screen_width - 55));
                Chart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);

                UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
                UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
                UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
                UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
                //UltraGridExporter1.MultiHeader = true;

                UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
                UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                        <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
                UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
                //UltraGridExporter1.MultiHeader = true;
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

                    baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                    last_year.Value = UserComboBox.getLastBlock(getLastDate());

                    LabelHeader.Text = String.Format(headerText, RegionSettingsHelper.Instance.RegionNameGenitive);

                    WebAsyncRefreshPanel1.AddLinkedRequestTrigger(web_grid1);

                    Collection<string> years = new Collection<string>();
                    
                    years.Add(last_year.Value);

                    ComboYear.Title = "Год";
                    ComboYear.Width = 100;
                    ComboYear.MultiSelect = false;
                    ComboYear.FillValues(Krista.FM.Server.Dashboards.reports.MO.MO_0002._0004.default2.getDates("dates"));
                    
                    ComboYear.SetСheckedState(last_year.Value, true);

                    gridColumnSelected.Value = "1";

                }
                last_year.Value = ComboYear.SelectedValue;
                web_grid1.DataBind();
                grid1Manual_ActiveCellChange(Convert.ToInt16(gridColumnSelected.Value.ToString()));
            }
            catch (Exception ex)
            {
                // неудачная загрузка ...
                throw new Exception(ex.Message, ex);
            }
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
                //DataTable grid1_table = new DataTable();
                //DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid1"), "Область", grid1_table);
                //web_grid1.DataSource = grid1_table.DefaultView;


                CellSet grid_set = null;
                DataTable grid_table = new DataTable();
                // Загрузка таблицы цен и товаров в CellSet
                grid_set = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("grid2"));
                // Добавление столбцов в таблицу и заполнение данных в DataTable
                int columnsCount = grid_set.Axes[0].Positions.Count;
                int rowsCount = grid_set.Axes[1].Positions.Count;
                grid_table.Columns.Add("Город");
                for (int i = 0; i < columnsCount; ++i)
                {
                    grid_table.Columns.Add(grid_set.Axes[0].Positions[i].Members[0].Caption.Replace("\"", "&quot;"));
                    grid_table.Columns.Add();
                    grid_table.Columns[grid_table.Columns.Count - 1].Caption = "Ранг";
                }

                object[,] cells = new object[rowsCount, columnsCount * 2 + 1];
                for (int i = 0; i < rowsCount; ++i)
                {
                    cells[i, 0] = grid_set.Axes[1].Positions[i].Members[0].Caption;
                    for (int j = 0; j < columnsCount; ++j)
                    {
                        cells[i, j * 2 + 1] = grid_set.Cells[j, i].Value;
                    }
                }


                string[,] range = new string[columnsCount, rowsCount];
                for (int i = 0; i < columnsCount; ++i)
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
                                min = Convert.ToDouble(cells[k, i * 2 + 1]);
                                first = false;
                                minIndex = k;
                            }
                            if (Convert.ToDouble(cells[k, i * 2 + 1]) > min)
                            {
                                min = Convert.ToDouble(cells[k, i * 2 + 1]);
                                minIndex = k;
                            }
                        }
                        if (!checkedVal[minIndex])
                        {
                            realMax = Convert.ToDouble(cells[minIndex, i * 2 + 1]);
                            range[i, minIndex] = rangeCount.ToString();
                            checkedVal[minIndex] = true;
                            minIndex_ = minIndex;
                        }

                        // поиск равных элементов
                        //                        Boolean zeroFind = false;
                        for (int k = 0; k < rowsCount; ++k)
                        {
                            if (checkedVal[k]) continue;
                            if (Convert.ToDouble(cells[k, i * 2 + 1]) == realMax)
                            {
                                //                                if (realMax == 0)
                                //                                    zeroFind = true;
                                range[i, k] = rangeCount.ToString();
                                checkedVal[k] = true;
                            }
                        }
                        //                        if (zeroFind) rangeCount++;


                    }
                    string maxR = range[i, minIndex_];
                    for (int k = 0; k < rowsCount; k++)
                        if (range[i, k] == maxR)
                            range[i, k] = "+" + maxR;
                }


                for (int i = 0; i < rowsCount; ++i)
                {
                    int t = 0;
                    for (int j = 2; j < columnsCount * 2 + 1; j = j + 2)
                    {
                        cells[i, j] = range[t, i];
                        t++;
                    }
                }

                for (int i = 0; i < rowsCount; ++i)
                {
                    object[] values = new object[columnsCount * 2 + 1];
                    for (int j = 0; j < columnsCount * 2 + 1; ++j)
                    {
                        values[j] = String.Format("{0:N4}", cells[i, j]);

                    }
                    grid_table.Rows.Add(values);
                }
                /*            
                                //grid_rows_skip
                                int cellsCount = grid_set.Axes[1].Positions.Count;
                                int j = 0;
                                int skipCount = 0;
                                for (int i = 0; i < grid_rows.Length; ++i)
                                {
                                    object[] values = new object[columnsCount + 1];
                                    if (grid_rows_skip[skipCount] == i)
                                    {
                                        values[0] = grid_rows[i];
                                        skipCount++;
                                    }
                                    else
                                    {
                                        values[0] = grid_rows[i];
                                        values[1] = grid_set.Cells[0, grid_set.Axes[1].Positions[j].Ordinal].FormattedValue.ToLower();
                                        double val1 = Convert.ToDouble(grid_set.Cells[1, grid_set.Axes[1].Positions[j].Ordinal].Value);
                                        double val2 = Convert.ToDouble(grid_set.Cells[2, grid_set.Axes[1].Positions[j].Ordinal].Value);

                                        values[2] = grid_set.Cells[1, grid_set.Axes[1].Positions[j].Ordinal].FormattedValue.ToString();
                                        //values[2] = grid_set.Cells[2, grid_set.Axes[1].Positions[j].Ordinal].FormattedValue.ToString();
                                        //values[1] = Math.Round(val1, 2).ToString();
                                        if (val1 == 0)
                                            values[3] = " "; 
                                        else
                                            values[3] = (Math.Round(val2 / val1 * 100, 2)).ToString() + " %"; 
                                        for (int k = 3; k < columnsCount; ++k)
                                        {
                                            values[k + 1] = grid_set.Cells[k, grid_set.Axes[1].Positions[j].Ordinal].FormattedValue.ToString();
                                        }
                                        j++;
                                    }
                                    grid_table.Rows.Add(values);
                                }
                 */
                web_grid1.DataSource = grid_table.DefaultView;

            }
            catch (Exception exception) // блок для обработки исключений
            {
                throw new Exception(exception.Message, exception);
            }

        }


        protected void web_grid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            try
            {
                e.Layout.HeaderStyleDefault.Font.Bold = true;
                // настройка столбцов
                double tempWidth = e.Layout.FrameStyle.Width.Value - 6;
                e.Layout.RowSelectorStyleDefault.Width = 0;
                e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth) * 0.15) - 4;

                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    // установка формата отображения данных в UltraWebGrid
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "### ### ### ##0.0000");
                    if (i % 2 == 0)
                    {
                        e.Layout.Bands[0].Columns[i].Width = (int)((tempWidth) * 0.85 / 2 / (e.Layout.Bands[0].Columns.Count - 1)) - 4;
                        e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    }
                    else
                        e.Layout.Bands[0].Columns[i].Width = (int)((tempWidth) * 0.85 * 1.5 / (e.Layout.Bands[0].Columns.Count - 1)) - 4;
                    e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                    e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
                    //e.Layout.Bands[0].Columns[i].Header.Caption = e.Layout.Bands[0].Columns[i].Header.Caption.Replace("\"", "+");
                    //e.Layout.Bands[0].Columns[i].Header.Key = e.Layout.Bands[0].Columns[i].Header.Key.Replace("\"", "+");

                }
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            }
            catch
            {
                // ошибка инициализации
            }
        }

        protected void web_grid1_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 2; i < e.Row.Cells.Count; i++)
            {
                if ((e.Row.Cells[i].Text == "1") & (i % 2 == 0))
                {
                    e.Row.Cells[i - 1].Style.BackgroundImage = "~/images/greenRect.gif";
                    e.Row.Cells[i - 1].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
                if ((e.Row.Cells[i].Text.Substring(0, 1) == "+") & (i % 2 == 0))
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
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "_", grid_master);
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


        protected void web_grid1_Click(object sender, Infragistics.WebUI.UltraWebGrid.ClickEventArgs e)
        {
            int CellIndex = e.Column.Index;
            if (CellIndex > 0)
            {
                if (CellIndex % 2 == 0)
                    CellIndex--;
                grid1Manual_ActiveCellChange(CellIndex);
            }
            else
            {
                grid1Manual_ActiveCellChange(1);
            }

        }

        /// <summary>
        /// Обновление данных для UltraChart при изменении активной ячейки в UltraWebGrid
        /// </summary>
        /// <param name="CellIndex">индекс активной ячейки</param>
        protected void grid1Manual_ActiveCellChange(int CellIndex)
        {
            gridColumnSelected.Value = CellIndex.ToString();
            chart1_caption.Text = web_grid1.Columns[CellIndex].Header.Caption.ToString();
            position.Value = web_grid1.Columns[CellIndex].Header.Caption.Replace("&quot;", "\"");

            //gridColumnSelected.Value = CellIndex.ToString();
            //web_grid1.Columns[CellIndex].Header.Key.ToString()
            Chart1.DataBind();
            //chart1_caption.Text = String.Format(chart1_title_caption, gridColumnSelected.Value.ToString());
            //web_grid1.Columns[CellIndex].Selected = true;
        }

        protected void web_grid1_ActiveCellChange(object sender, CellEventArgs e)
        {
            UltraGridCell cell = e.Cell;
            int CellIndex = cell.Column.Index;
            if (CellIndex > 0)
            {
                if (CellIndex % 2 == 0)
                    CellIndex--;
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
                if (CellIndex % 2 == 0)
                    CellIndex--;
                grid1Manual_ActiveCellChange(CellIndex);
            }
            else
            {
                grid1Manual_ActiveCellChange(1);
            }
        }

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = LabelHeader.Text;
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
                if (i % 2 == 0)
                    e.CurrentWorksheet.Columns[i].Width = widthColumn / 2 * 37;
                else
                    e.CurrentWorksheet.Columns[i].Width = widthColumn * 3 / 2 * 37;
            }
            e.CurrentWorksheet.Columns[0].Width = 150 * 37;
            //e.CurrentWorksheet.Rows[4].Hidden = true;
        }

        private int offset = 0;

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            UltraGridColumn col = web_grid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            e.HeaderText = e.HeaderText.Replace("&quot;", "\"");
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
            title.AddContent(LabelHeader.Text);

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


    }

}


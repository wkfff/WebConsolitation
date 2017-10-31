using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs=Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;

namespace Krista.FM.Server.Dashboards.reports.IT_0002_0003
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 50);
            UltraWebGrid1.Height = Unit.Empty;

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
            UltraChart1.Height = CRHelper.GetChartHeight(350);

            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
            //UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraGridExporter1.Visible = false;
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <DocumentExportEventArgs>(PdfExporter_BeginExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            Page.Title = "Среднедневные остатки";
            PageTitle.Text = "Среднедневные остатки";
            PageSubTitle.Text = "Помесячная динамика и структура среднедневных остатков на счетах коммерческих банков";

            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();

            SetBarChartAppearance();
            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraChart1.DataBind();
        }

        void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("IT_0002_0003_Chart"));
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtGrid);
            dtGrid.Columns.RemoveAt(0);
            foreach (DataColumn col in dtGrid.Columns)
            {
                col.ColumnName = col.ColumnName.Split(';')[0].Replace('"', '\'');
            }
            UltraChart1.DataSource = dtGrid;
        }
        
        #region Обработчики грида

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("IT_0002_0003"));
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtGrid);
            dtGrid.Columns.RemoveAt(0);
            foreach (DataColumn col in dtGrid.Columns)
            {
                col.ColumnName = col.ColumnName.Replace('"', '\'');
            }
            ((UltraWebGrid)sender).DataSource = dtGrid;
        }
        
        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid1.Height = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
           e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;

            e.Layout.Bands[0].Columns[0].Width = 131;
            e.Layout.Bands[0].Columns[1].Width = 126;
            e.Layout.Bands[0].Columns[2].Width = 125; 
            e.Layout.Bands[0].Columns[3].Width = 125;
            e.Layout.Bands[0].Columns[4].Width = 125;
            e.Layout.Bands[0].Columns[5].Width = 125;
           

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "P2");

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 1)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }
            e.Layout.Bands[0].Columns[0].Header.Caption = "Период";
            e.Layout.Bands[0].Columns[1].Header.Caption = "Всего, тыс.руб.";
            int multiHeaderPos = 2;

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 2)
            {
                ColumnHeader ch = new ColumnHeader(true);

                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                ch.Caption = captions[0];

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, captions[1], String.Empty);
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Доля", String.Empty);

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 2;
                ch.RowLayoutColumnInfo.SpanX = 2;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
           
        }

        private string GetColumnHeaderTooltip(int index)
        {
            return string.Empty;
        }

        
        #endregion

        private void SetBarChartAppearance()
        {
            UltraChart1.ChartType = ChartType.StackColumnChart;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Custom;
            UltraChart1.Axis.X.Labels.SeriesLabels.OrientationAngle = 90;
            UltraChart1.Axis.X.Labels.Visible = false;
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL><br /><SERIES_LABEL><br /><b><DATA_VALUE:N2></b>&nbsp;тыс.руб.";
           
            UltraChart1.Legend.SpanPercentage = 18;
            
          //  UltraChart1.Axis.X.Labels.SeriesLabels.FontSizeBestFit = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.Font = new Font("Verdana", 8);

        //    UltraChart1.Axis.X.Labels.FontSizeBestFit = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;

            UltraChart1.Axis.Y.Extent = 90;
            UltraChart1.Axis.X.Extent = 140;
           
            SetupTitleLeft();
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart1.Legend.Font = new Font("Verdana", 8);
            UltraChart1.Legend.FontColor = Color.White;
            
            //UltraChart1.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart1_ChartDrawItem);
        }

        private string axisMonth = String.Empty;

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            SceneGraph grahp = e.SceneGraph;

            for (int i = 0; i < grahp.Count; i++)
            {
                Primitive primitive = grahp[i];
                if (primitive is Text)
                {
                    string text = ((Text)primitive).GetTextString();
                    text = text.Trim();
                    // Проверяем формат
                    string[] textArray = text.Split();
                    if (textArray.Length == 2)
                    {
                        int year;
                        if (Int32.TryParse(textArray[1], out year) && CRHelper.IsMonthCaption(textArray[0]))
                        {
                            if (axisMonth == textArray[1])
                            {
                                ((Text)primitive).SetTextString(textArray[0]);
                            }
                            else
                            {
                                ((Text)primitive).SetTextString(String.Format("{0}-{1}",
                                              year ,CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(CRHelper.MonthNum(textArray[1])))));
                                axisMonth = textArray[1];
                            }
                        }
                    }
                }
            }
        }
        
        private void SetupTitleLeft()
        {
            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Text = "тыс.руб.";
            UltraChart1.TitleLeft.Font = new Font("Verdana", 8);
           // UltraChart1.TitleLeft.FontColor = Color.Black;
            UltraChart1.TitleLeft.VerticalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = (int)UltraChart1.Height.Value / 4 * 3 - 30;
        }

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid1.Columns.Count;
            int rowsCount = UltraWebGrid1.Rows.Count;

            for (int i = 0; i < columnCount; i++)
            {
                e.CurrentWorksheet.Columns[i].Width = 100 * 37;
            }

            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "#,##0";
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.00";

            // расставляем стили у начальных колонок
            for (int i = 4; i < rowsCount + 4; i++)
            {
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid1);
        }

        private int offset = 0;

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            UltraGridColumn col = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            while (col.Hidden)
            {
                offset++;
                col = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            }
            e.HeaderText = col.Header.Key.Split(';')[0];
        }

        #endregion

        #region Экспорт в PDF


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

            UltraGridExporter1.PdfExporter.Export(UltraWebGrid1);
        }

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
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

            e.Section.AddPageBreak();
        }

        #endregion
    }
}

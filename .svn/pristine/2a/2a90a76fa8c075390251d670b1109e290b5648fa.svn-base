using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs=Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;

namespace Krista.FM.Server.Dashboards.reports.IT_0002_0001
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtGridIncomes;
        private DataTable dtGridOutcomes;
        private DataTable dtChart;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid2.Width = Unit.Empty;

            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid2.Height = Unit.Empty;

            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
            //UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraChart1.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);

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

        void UltraChart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            SceneGraph grahp = e.SceneGraph;

            for (int i = 0; i < grahp.Count; i++)
            {
                Primitive primitive = grahp[i];
                if (primitive is Box)
                {
                    Box box = (Box) primitive;
                    if (box.DataPoint != null)
                    {
                        int gridColumn = box.Row == 0 || box.Row == 2 ? 15 : 12;
                        int gridRow =  box.Column + 2;

                        string part = String.Format("Доля в бюджете ФГУП <b>{0:P2}</b>", box.Row < 2 ? dtGridIncomes.Rows[gridRow][gridColumn] : dtGridOutcomes.Rows[gridRow][gridColumn]);

                        box.DataPoint.Label = String.Format("{0}<br/>{1}<br/><b>{2:N2}</b>&nbsp;тыс.руб.<br/>{3}", box.DataPoint.Label, box.Series.Label, box.Value, part);
                    }
                }
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            //dtDate = new DataTable();
            //string query = DataProvider.GetQueryText("IT_0002_0001_date");
            //DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            //UserParams.PeriodYear.Value = dtDate.Rows[0][0].ToString();


            Page.Title = "Доходы и расходы";
            PageTitle.Text = "Доходы и расходы";
            PageSubTitle.Text = "Исполнение бюджета по доходам и расходам за 1 полугодие 2010 года";

            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();

            UltraWebGrid2.Bands.Clear();
            UltraWebGrid2.DataBind();

            SetBarChartAppearance();
            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
            UltraChart1.DataBind();

            gridCaptionElement.Text = "Доходы за 1 полугодие 2010 года";
            Label2.Text = "Расходы за 1 полугодие 2010 года";

        }

        void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("IT_0004_0001_Chart"));
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtChart);

            UltraChart1.Series.Clear();

            for (int i = 1; i < dtChart.Columns.Count; i++ )
            {
                UltraChart1.Series.Add(CRHelper.GetNumericSeries(i, dtChart));
            }

            //UltraChart1.DataSource = dtChart;
        }
        
        #region Обработчики грида

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("IT_0002_0001_incomes"));
            dtGridIncomes = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtGridIncomes);
            ((UltraWebGrid)sender).DataSource = dtGridIncomes;
        }

        protected void UltraWebGrid2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("IT_0002_0001_outcomes"));
            dtGridOutcomes = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtGridOutcomes);
            ((UltraWebGrid)sender).DataSource = dtGridOutcomes;
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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(126, 1280);

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

            ColumnHeader ch = new ColumnHeader(true);
            string[] captions = e.Layout.Bands[0].Columns[4].Header.Caption.Split(';');
            ch.Caption = captions[0];
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 1;
            ch.RowLayoutColumnInfo.SpanX = 4;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            captions = e.Layout.Bands[0].Columns[5].Header.Caption.Split(';');
            ch.Caption = captions[0];
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 5;
            ch.RowLayoutColumnInfo.SpanX = 9;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, captions[1], GetColumnHeaderTooltip(i));
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(81, 1280);
            }

            e.Layout.Bands[0].Columns[13].Width = CRHelper.GetColumnWidth(68, 1280);
            e.Layout.Bands[0].Columns[14].Hidden = true;
            e.Layout.Bands[0].Columns[15].Hidden = true;
           
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P2");

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[9], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[10], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[11], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[12], "P2");

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[13], "N0");
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index > 1)
            {
                SetRankImage(e, 13, 14, true);
            }
            else
            {
                e.Row.Cells[13].Value = String.Empty;
            }
            if (e.Row.Index > 0)
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
            }
            e.Row.Cells[5].Style.BorderDetails.ColorLeft = Color.FromArgb(192, 192, 192);

            SetConditionBall(e, 4);
            SetConditionBall(e, 8);
            SetConditionArrow(e, 9);
            SetConditionArrow(e, 10);
            SetConditionArrow(e, 11);
        }

        private string GetColumnHeaderTooltip(int index)
        {
            switch (index)
            {
                case 1:
                    {
                        return "Первоначальный план";
                    }
                case 2:
                    {
                        return "Уточненный план";
                    }
                case 3:
                    {
                        return "Фактическое исполнение бюджета за аналогичный период предыдущего года";
                    }
                case 4:
                    {
                        return "Исполнение первоначального плана";
                    }
                case 5:
                    {
                        return "Первоначальный план";
                    }
                case 6:
                    {
                        return "Уточненный план";
                    }
                case 7:
                    {
                        return "Фактическое исполнение бюджета за аналогичный период предыдущего года";
                    }
                case 8:
                    {
                        return "Исполнение первоначального плана";
                    }
                case 9:
                    {
                        return "Фактический темп роста к аналогичному периоду предыдущего года";
                    }
                case 10:
                    {
                        return "Темп роста к аналогичному периоду предыдущего года по первоначальному плану";
                    }
                case 11:
                    {
                        return "Темп роста к аналогичному периоду предыдущего года по первоначальному плану";
                    }
                case 12:
                    {
                        return "Доля в консолидированном бюджете";
                    }
                case 13:
                    {
                        return "Ранг по доле в консолидированном бюджете";
                    }
            }
            return String.Empty;
        }

        private static void SetConditionArrow(RowEventArgs e, int index)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string img;
                //string title;
                if (value > 1)
                {
                    img = "~/images/arrowGreenUpBB.png";

                }
                else
                {
                    img = "~/images/arrowRedDownBB.png";

                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; padding-left: 2px";
                //   e.Row.Cells[3].Title = title;
            }
            if (e.Row.Index > 0)
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
            }
        }

        private static void SetRankImage(RowEventArgs e, int rankCellIndex, int worseRankCelIndex, bool direct)
        {
            if (e.Row.Cells[rankCellIndex] != null &&
                e.Row.Cells[rankCellIndex].Value != null &&
                e.Row.Cells[worseRankCelIndex] != null &&
                e.Row.Cells[worseRankCelIndex].Value != null)
            {
                int value = Convert.ToInt32(e.Row.Cells[rankCellIndex].Value.ToString());
                int worseRankValue = Convert.ToInt32(e.Row.Cells[worseRankCelIndex].Value.ToString());
                string img = String.Empty;
                //string title;
                if (direct)
                {
                    if (value == 1)
                    {
                        img = "~/images/StarYellowBB.png";
                    }
                    else if (value == worseRankValue)
                    {
                        img = "~/images/StarGrayBB.png";
                    }
                }
                else
                {
                    if (value == 1)
                    {
                        img = "~/images/StarGrayBB.png";
                    }
                    else if (value == worseRankValue)
                    {
                        img = "~/images/StarYellowBB.png";
                    }
                    e.Row.Cells[rankCellIndex].Value = worseRankValue - value + 1;
                }
                e.Row.Cells[rankCellIndex].Style.BackgroundImage = img;
                e.Row.Cells[rankCellIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: 20px center; padding-left: 2px; padding-right: 15px";
            }
        }

        private static void SetConditionBall(RowEventArgs e, int index)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string img;
                //string title;
                if (value < 1)
                {
                    img = "~/images/BallRedBB.png";

                }
                else
                {
                    img = "~/images/BallGreenBB.png";

                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 5px center; padding-left: 2px";
                //   e.Row.Cells[3].Title = title;
            }
        }

        #endregion

        private void SetBarChartAppearance()
        {
            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth) - 50;
            UltraChart1.Height = CRHelper.GetChartWidth(350);

            UltraChart1.ChartType = ChartType.StackBarChart;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.Y.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Near;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 8);
            //UltraChart1.Axis.Y.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.None;

            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>";

            UltraChart1.Legend.SpanPercentage = 18;

            //  UltraChart1.Axis.X.Labels.SeriesLabels.FontSizeBestFit = false;
            UltraChart1.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart1.Axis.Y.Extent = 190;
            UltraChart1.Axis.X.Extent = 20;

            SetupTitleLeft();
            
            UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;
            //UltraChart1.Legend.Font = new Font("Verdana", 12);
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Top;
            UltraChart1.Legend.SpanPercentage = 15;
            UltraChart1.Legend.Font = new Font("Verdana", 10);
        }

        private void SetupTitleLeft()
        {
            UltraChart1.TitleBottom.Visible = true;
            UltraChart1.TitleBottom.Text = "тыс.руб.";
            UltraChart1.TitleBottom.Font = new Font("Verdana", 10);
            UltraChart1.TitleBottom.VerticalAlign = StringAlignment.Near;
            UltraChart1.TitleBottom.HorizontalAlign = StringAlignment.Center;
            //UltraChart1.TitleBottom.Margins.Bottom = 100;
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

using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SgmSupport;
using System.Drawing.Text;

namespace Krista.FM.Server.Dashboards.reports.SGM.SGM_0008
{
    public partial class sgm_0008 : CustomReportPage
    {
        readonly DataTable dtChart = new DataTable();
        string mapName = string.Empty;
        string year = string.Empty;
        private DataTable tblFullData;
        private int minIndex, maxIndex, maxIndexAvg, minIndexAvg;

        private readonly SGMDataRotator dataRotator = new SGMDataRotator();
        private readonly SGMDataObject dataObject = new SGMDataObject();
        private readonly SGMSupport supportClass = new SGMSupport();
        private readonly SGMPdfExportHelper exportClass = new SGMPdfExportHelper();

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            chart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 60);
            chart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 300);
            grid.Width = chart.Width;
            grid.Height = Unit.Empty;
            LabelTitle.Width = (Unit)(chart.Width.Value - 100);
            LabelSubTitle.Width = LabelTitle.Width;
            SetExportHandlers();
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            dataObject.reportFormRotator = dataRotator;
            dataRotator.CheckSubjectReport();
            base.Page_Load(sender, e);
            dataRotator.formNumber = 1;
            dataObject.InitObject();

            if (!Page.IsPostBack)
            {
                dataRotator.FillSGMMapList(ComboMap, dataObject.dtAreaShort, true);
                supportClass.FillPeopleGroupList(ComboGroup);
                dataRotator.FillYearList(ComboYear);
                dataRotator.FillYearList(ComboStartYear);
                ComboStartYear.SetСheckedState(Convert.ToString(Convert.ToInt32(ComboStartYear.SelectedValue) - 3), true);
                dataRotator.FillDeseasesList(ComboDesease, 1);
            }
            else
            {
                dataRotator.FillSGMMapList(null, dataObject.dtAreaShort, true);
                dataRotator.FillDeseasesList(null, 1);
            }

            DataColumn dataColumn = dtChart.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            for (int i = 1; i < 13; i++)
            {
                dataColumn = dtChart.Columns.Add();
                dataColumn.DataType = Type.GetType("System.Double");
                dataColumn.ColumnName = CRHelper.RusMonth(i);
            }

            mapName = ComboMap.SelectedValue;
            year = ComboYear.SelectedValue;

            DataRow rowCur = dtChart.Rows.Add();
            rowCur[0] = String.Format("{0} год", year);
            DataRow rowAvg = dtChart.Rows.Add();
            rowAvg[0] = "Средний уровень";
            DataRow rowMax = dtChart.Rows.Add();
            rowMax[0] = "Верхняя граница";
            DataRow rowLim = dtChart.Rows.Add();
            rowLim[0] = "Верхний предел";

            int yearMin = Convert.ToInt32(ComboStartYear.SelectedValue);
            int yearMax = dataRotator.GetLastYear();
            int yearCount = yearMax - yearMin;

            string groupName = Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtAll));
            if (ComboGroup.SelectedIndex == 1)
            {
                groupName = Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtMature));                
            }
            if (ComboGroup.SelectedIndex == 2)
            {
                groupName = Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtChild));
            }
            if (ComboGroup.SelectedIndex == 3)
            {
                groupName = Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtTeen));
            }

            string deseasesCodes = dataRotator.deseasesCodes[ComboDesease.SelectedIndex];

            dataObject.InitObject();
            dataObject.ignoreRegionConversion = true;
            dataObject.mainColumn = SGMDataObject.MainColumnType.mctMapName;
            for (int i = yearMax; i >= yearMin; i--)
            {
                for (int j = 1; j < 13; j++)
                {
                    dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                                         Convert.ToString(i), Convert.ToString(j), String.Empty, groupName, deseasesCodes);
                    dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                                         Convert.ToString((yearMax - i) * 24 + (j - 1) * 2 + 1));
                }
            }

            tblFullData = dataObject.FillData();
            DataRow rowData = supportClass.FindDataRowEx(tblFullData, mapName, tblFullData.Columns[0].ColumnName);

            double avgCount = 0;
            double sigma = 0;
            int dataCount = 0;
            for (int i = yearMax; i >= yearMin; i--)
            {
                for (int j = 1; j < 13; j++)
                {
                    avgCount += Convert.ToDouble(rowData[(yearMax - i) * 24 + (j - 1) * 2 + 2]);
                    dataCount++;
                }
            }

            avgCount /= dataCount;
            var monthAvg = new double[12];
            var monthMax = new double[12];

            for (int i = yearMax; i >= yearMin; i--)
            {
                for (int j = 1; j < 13; j++)
                {
                    double diesValue = Convert.ToDouble(rowData[(yearMax - i) * 24 + (j - 1) * 2 + 2]);
                    monthAvg[j - 1] += diesValue; 
                    sigma += Math.Pow(diesValue - avgCount, 2);
                    dataCount++;
                }
            }

            sigma = Math.Sqrt(sigma / (12 * (yearMax - yearMin + 1)));
            double monthLim = avgCount + 0.3 * sigma;

            double minAvg = Double.MaxValue, maxAvg = 0, minValue = Double.MaxValue, maxValue = 0;

            for (int j = 0; j < 12; j++)
            {
                monthAvg[j] /= (yearMax - yearMin + 1);
                monthMax[j] = monthAvg[j] + 2.6 * sigma;

                double diesValue = Convert.ToDouble(rowData[j * 2 + 2]);

                rowCur[j + 1] = diesValue;
                rowAvg[j + 1] = monthAvg[j];
                rowMax[j + 1] = monthMax[j];
                rowLim[j + 1] = monthLim;

                if (diesValue > 0)
                {
                    minIndex = diesValue < minValue ? j + 1 : minIndex;
                    maxIndex = diesValue > maxValue ? j + 1 : maxIndex;

                    minValue = Math.Min(minValue, diesValue);
                    maxValue = Math.Max(maxValue, diesValue);
                }

                if (monthAvg[j] > 0)
                {
                    minIndexAvg = monthAvg[j] < minAvg ? j + 1 : minIndexAvg;
                    maxIndexAvg = monthAvg[j] > maxAvg ? j + 1 : maxIndexAvg;

                    minAvg = Math.Min(minAvg, monthAvg[j]);
                    maxAvg = Math.Max(maxAvg, monthAvg[j]);
                }
            }

            chart.Data.ZeroAligned = true;
            chart.Legend.SpanPercentage = Convert.ToInt32(100 * 140 / chart.Width.Value);
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            chart.Tooltips.FormatString = "<SERIES_LABEL>\n <ITEM_LABEL>\n <DATA_VALUE:N0> на 100 тыс.чел.";
            chart.Legend.Margins.Bottom = Convert.ToInt32(chart.Height.Value * 0.8);
            chart.Border.Thickness = 0;
            chart.RadarChart.LineThickness = 3;
            chart.RadarChart.LineEndCapStyle = LineCapStyle.RoundAnchor;
            chart.Legend.Location = LegendLocation.Right;
            chart.Legend.Visible = true;
            chart.ColorModel.ModelStyle = ColorModels.PureRandom;
            chart.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            FillChartData();

            var pe = new PaintElement {Fill = Color.DarkGreen, ElementType = PaintElementType.SolidFill};
            chart.Series[3].PEs.Add(pe);

            pe = new PaintElement {Fill = Color.Blue, ElementType = PaintElementType.SolidFill};
            chart.Series[1].PEs.Add(pe);

            pe = new PaintElement {Fill = Color.Red, ElementType = PaintElementType.SolidFill};
            chart.Series[2].PEs.Add(pe);            
            
            pe = new PaintElement {Fill = Color.BlueViolet, ElementType = PaintElementType.SolidFill};
            chart.Series[0].PEs.Add(pe);

            grid.DisplayLayout.RowHeightDefault = 25;
            grid.DataSource = dtChart;
            grid.DataBind();

            if (yearCount > 0) yearCount--;
            string yearCaption = "лет";
            if (yearCount < 5)
            {
                yearCaption = "года";
                if (yearCount == 1) yearCaption = "год";
                if (yearCount == 0) yearCaption = "лет";
            }

            Page.Title = String.Format("Помесячная динамика заболеваемости");
            LabelTitle.Text = Page.Title;
            LabelSubTitle.Text = String.Format(
                "{0}, {1}, за {2} год, группа населения: {3}. Расчет выполнен по данным за {4} {5}.", 
                ComboMap.SelectedValue.Trim(), 
                ComboDesease.SelectedValue, 
                ComboYear.SelectedValue, 
                ComboGroup.SelectedValue, 
                yearCount, 
                yearCaption);

            LabelComments0.Text = String.Format("<b>{0} год - </b>Показатель заболеваемости за {0} год на 100 тыс. населения.", ComboYear.SelectedValue);
            LabelComments1.Text = String.Format("<b>Средний уровень - </b>Среднемесячный многолетний уровень <br>Рассчитывается по формуле среднеарифметического для каждого месяца по многолетним данным (отношение суммы показателей заболеваемости этого месяца к числу анализируемых лет).");
            LabelComments2.Text = String.Format("<b> Верхняя граница - </b>Верхняя граница доверительного интервала <br>Рассчитывается по формуле Среднее + 2.6 средней ошибки среднеарифметического.");
            LabelComments3.Text = String.Format("<b> Верхний предел - </b>Верхний предел круглогодичной заболеваемости <br>Рассчитывается по формуле Среднемноголетний взвешенный минимальный показатель заболеваемости + 3 средних ошибки среднемноголетнего показателя.");
        }

        protected void FillChartData()
        {
            var dt = new DataTable();

            DataColumn dataColumn = dt.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            for (int i = 1; i < 5; i++)
            {
                dataColumn = dt.Columns.Add();
                dataColumn.DataType = Type.GetType("System.Double");
            }
            dt.Columns[0].ColumnName = String.Format("Месяц");
            dt.Columns[1].ColumnName = String.Format("{0} год", ComboYear.SelectedValue);
            dt.Columns[2].ColumnName = String.Format("Средний уровень");
            dt.Columns[3].ColumnName = String.Format("Верхняя граница");
            dt.Columns[4].ColumnName = String.Format("Верхний предел");

            for (int i = 0; i < 12; i++)
            {
                DataRow dr = dt.Rows.Add();
                dr[0] = CRHelper.RusMonth(i + 1);
            }

            for (int i = 0; i < 12; i++)
            {
                for (int j = 1; j < 5; j++)
                {
                    dt.Rows[i][j] = dtChart.Rows[j - 1][i + 1];
                }
            }

            for (int i = 0; i < 4; i++)
            {
                NumericSeries series1 = CRHelper.GetNumericSeries(i + 1, dt);                
                series1.Label = dt.Columns[i + 1].ColumnName;
                chart.Series.Add(series1);
            }    
        }

        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            supportClass.SetColumnWidthAndCaption(grid, 0, "Показатель", 95, HorizontalAlign.Left, String.Empty);
            for (int i = 1; i < grid.Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(grid.Columns[i], "N2");
                supportClass.SetColumnWidthAndCaption(grid, i, grid.Columns[i].Header.Caption, 75, HorizontalAlign.Right, String.Empty);
            }

            supportClass.CalculateGridColumnsWidth(grid, 1);
        }

        public void DrawImage(UltraGridCell cell, string cssName, string tooltip)
        {
            string physicalAppPath = HttpContext.Current.Request.PhysicalApplicationPath;
            string fileName = String.Format("{0}TemporaryImages\\CellImageTemp{1}{2}.png", physicalAppPath, cssName, cell.Style.CssClass);

            var bitmap = new Bitmap(85, 25);
            bitmap.SetResolution(72, 72);
            Graphics graphics = Graphics.FromImage(bitmap);

            int offsetX = 0;
            if (cell.Style.CssClass != String.Empty)
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(String.Format("../../../images/{0}.png", cell.Style.CssClass)));
                graphics.DrawImage(image, offsetX, 0);
                offsetX += 26;
            }

            if (cssName != String.Empty)
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(String.Format("../../../images/{0}.png", cssName)));
                graphics.DrawImage(image, offsetX, 3);
            }

            bitmap.Save(fileName, ImageFormat.Png);
            cell.Title = String.Format("{0} \n{1}", cell.Title, tooltip);
            cell.Style.BackgroundImage = String.Format("~/TemporaryImages/CellImageTemp{0}{1}.png", cssName, cell.Style.CssClass);
            cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin-left: 5px";
                cell.Style.CssClass = cssName;
        }

        protected void grid_InitializeRow(object sender, RowEventArgs e)
        {
            const int colWidthWithOne = 75;
            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Title = string.Format("Показатель заболеваемости за {0} год на 100 тыс. населения", ComboYear.SelectedValue);
                DrawImage(e.Row.Cells[minIndex], "StarYellow", String.Empty);
                e.Row.Cells[minIndex].Title = "Минимальный уровень заболеваемости";
                DrawImage(e.Row.Cells[maxIndex], "StarGray", String.Empty);
                e.Row.Cells[maxIndex].Title = "Максимальный уровень заболеваемости";
                if (grid.Columns[minIndexAvg].Width.Value < colWidthWithOne) grid.Columns[minIndexAvg].Width = colWidthWithOne;
                if (grid.Columns[maxIndexAvg].Width.Value < colWidthWithOne) grid.Columns[maxIndexAvg].Width = colWidthWithOne;                
                for (int i = 1; i < 13; i++)
                {
                    if (dtChart.Rows[0][i].ToString() != String.Empty)
                    {
                        if (Convert.ToDouble(dtChart.Rows[0][i]) > Convert.ToDouble(dtChart.Rows[2][i]))
                        {
                            if (e.Row.Cells[i].Style.CssClass == String.Empty)
                            {
                                if (grid.Columns[i].Width.Value < colWidthWithOne) grid.Columns[i].Width = colWidthWithOne;
                                DrawImage(e.Row.Cells[i], "BallRedBB", String.Empty);
                                e.Row.Cells[i].Title = String.Format("Вспышка заболеваемости");
                            }
                            else
                            {
                                grid.Columns[i].Width = 100;
                                DrawImage(e.Row.Cells[i], "BallRedBB", "Вспышка заболеваемости");
                            }
                        }
                    }
                }
            }
            if (e.Row.Index == 1)
            {
                DrawImage(e.Row.Cells[minIndexAvg], "StarYellow", String.Empty);
                e.Row.Cells[minIndexAvg].Title = "Минимальный уровень заболеваемости";
                DrawImage(e.Row.Cells[maxIndexAvg], "StarGray", String.Empty);
                e.Row.Cells[maxIndexAvg].Title = "Максимальный уровень заболеваемости";
                if (grid.Columns[minIndexAvg].Width.Value < colWidthWithOne) grid.Columns[minIndexAvg].Width = colWidthWithOne;
                if (grid.Columns[maxIndexAvg].Width.Value < colWidthWithOne) grid.Columns[maxIndexAvg].Width = colWidthWithOne;

                e.Row.Cells[0].Title = String.Format("Среднемесячный многолетний уровень заболеваемости");
                for (int i = 1; i < 13; i++)
                {
                    if (Convert.ToDouble(dtChart.Rows[1][i]) > Convert.ToDouble(dtChart.Rows[3][i]))
                    {
                        if (e.Row.Cells[i].Style.CssClass == String.Empty)
                        {
                            if (grid.Columns[i].Width.Value < colWidthWithOne) grid.Columns[i].Width = colWidthWithOne;
                            DrawImage(e.Row.Cells[i], "BallRedBBDim", "Сезонная заболеваемость");
                        }
                        else
                        {
                            grid.Columns[i].Width = 100;
                            DrawImage(e.Row.Cells[i], "BallRedBBDim", "Сезонная заболеваемость");
                        }
                    }
                }
            }
            if (e.Row.Index == 2)
            {
                e.Row.Cells[0].Title = String.Format("Верхняя граница доверительного интервала");
            }            
            if (e.Row.Index == 3)
            {
                e.Row.Cells[0].Title = String.Format("Верхний предел круглогодичной заболеваемости");
            }
        }

        #region PDFExport

        protected virtual void SetExportHandlers()
        {
            UltraGridExporter1.PdfExportButton.Click += PdfExportButton_Click;
            UltraGridExporter1.PdfExporter.BeginExport += PdfExporter_BeginExport;
            UltraGridExporter1.PdfExporter.EndExport += PdfExporter_EndExport;
            UltraGridExporter1.ExcelExportButton.Visible = false;
            UltraGridExporter1.WordExportButton.Visible = true;
        }

        protected virtual void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            InitializeExportLayout(e);
            exportClass.ExportCaptionText(e, LabelTitle.Text);
            exportClass.ExportSubCaptionText(e, LabelSubTitle.Text);
            exportClass.ExportChart(e, chart);
        }

        protected virtual void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = String.Format("0008.pdf");
            UltraGridExporter1.PdfExporter.Export(grid);
        }

        protected virtual void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            exportClass.ExportCommentText(e, LabelComments0.Text);
            exportClass.ExportCommentText(e, LabelComments1.Text);
            exportClass.ExportCommentText(e, LabelComments2.Text);
            exportClass.ExportCommentText(e, LabelComments3.Text);
        }

        protected virtual void InitializeExportLayout(Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            chart.Width = (Unit)(chart.Width.Value - 100);
        }

        #endregion
    }
}

using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections.Generic; 
using Image = System.Drawing.Image;

namespace Krista.FM.Server.Dashboards.reports.FO_0042_0002_Gub
{
    public partial class Default : CustomReportPage
    {
        #region Поля
         
        private DataTable dtDate = new DataTable();
        private DataTable dtChartAVG;
        private DataTable dtChart;
        private int firstYear = 2008;
        private int endYear = 2011;

        private int currentYear;
        private double avgValue;

        private int firstGroupCount;
        private int secondGroupCount;
        private int thirdGroupCount;

        #endregion

    

        #region Параметры запроса

        // множество администраторов ГРБС
        private CustomParam grbsSet;
        private CustomParam selectedQuater;
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.9);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(UltraChart_InvalidDataReceived);
            ComboQuater.Width = 300;
            #region Инициализация параметров запроса

            if (grbsSet == null)
            {
                grbsSet = UserParams.CustomParam("grbs_set");
                selectedQuater= UserParams.CustomParam("selected_quater");
            }

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Border.Thickness = 0;

            UltraChart.ColumnChart.SeriesSpacing = 1;
            UltraChart.ColumnChart.ColumnSpacing = 1;

            UltraChart.Axis.X.Extent = 280;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart.Axis.X.StripLines.PE.FillOpacity = 150;
            UltraChart.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
            UltraChart.Axis.X.StripLines.Interval = 2;
            UltraChart.Axis.X.StripLines.Visible = true;
            UltraChart.Axis.Y.Extent = 50;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>%";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);

            UltraChart.Axis.X2.Visible = true;
            UltraChart.Axis.X2.Labels.Visible = false;
            UltraChart.Axis.X2.Labels.SeriesLabels.Visible = false;
            UltraChart.Axis.X2.LineColor = Color.Transparent;
            UltraChart.Axis.X2.Extent = 20;

            UltraChart.Axis.X.Labels.WrapText = true;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL>";

            UltraChart.ColorModel.ModelStyle = ColorModels.LinearRange;
            UltraChart.ColorModel.ColorBegin = Color.Green;
            UltraChart.ColorModel.ColorEnd = Color.Red;

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = "<DATA_VALUE:N2>";
            appearance.ChartTextFont = new Font("Verdana", 8);
            appearance.Visible = true;
            UltraChart.ColumnChart.ChartText.Add(appearance);

           // UltraChart.Tooltips.FormatString = "<ITEM_LABEL><br><SERIES_LABEL><br><DATA_VALUE:0.##>%";
            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>";

            #endregion

            UltraGridExporter1.MultiHeader = false;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;оценки&nbsp;качества";
            CrossLink1.NavigateUrl = "~/reports/FO_0042_0001_Gub/default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Анализ&nbsp;оценки&nbsp;качества";
            CrossLink2.NavigateUrl = "~/reports/FO_0042_0003_Gub/default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(UltraChart);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0042_0002_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);


                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                //ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.FillDictionaryValues(YearsLoad());
                ComboYear.SelectLastNode();
                ComboQuater.Title = "Оценка качества";
                ComboQuater.FillDictionaryValues(QuaterLoad());
                ComboQuater.SelectLastNode();
            }

            currentYear = Convert.ToInt32(ComboYear.SelectedValue);
            if (ComboQuater.SelectedIndex == 1)
            {
                selectedQuater.Value = "Данные года";
                Page.Title = String.Format("Рейтинг  главных распорядителей бюджетных средств в муниципальном образовании город Губкинский по результатам оценки качества финансового менеджмента по состоянию {0}", "1.01." + (int.Parse(ComboYear.SelectedValue) + 1).ToString() + " года");
            }
            else
            {
                selectedQuater.Value = "Остатки на начало года";
                Page.Title = String.Format("Рейтинг главных распорядителей средств областного бюджета, сформированный по результатам оценки качества финансового менеджмента по состоянию {0}", "1.07." + ComboYear.SelectedValue + " года");
            }
            
            PageTitle.Text = Page.Title;

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;

            grbsSet.Value = "Cписок администраторов";

            
            UltraChart.DataBind();
        }

        System.Collections.Generic.Dictionary<string, int> QuaterLoad()
        {
            System.Collections.Generic.Dictionary<string, int> d = new System.Collections.Generic.Dictionary<string, int>();
            d.Add("по состоянию на 1.07", 0);
            d.Add("по итогам года", 0);
            return d;
        }

        System.Collections.Generic.Dictionary<string, int> YearsLoad()
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("FO_0042_0002_date"), "Дата", dt);
            System.Collections.Generic.Dictionary<string, int> d = new System.Collections.Generic.Dictionary<string, int>();
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                d.Add(dt.Rows[i][0].ToString(), 0);
            }
            return d;
        }

        #region Обработчики диаграммы

        private static string GetShortGRBSNames(string fullNames)
        {
            string shortNames = String.Empty;

            string[] names = fullNames.Split(';');
            foreach (string s in names)
            {
                if (s != String.Empty)
                {
                    shortNames += String.Format("{0}, ", GetShortGRBSName(s));
                }
            }

            return shortNames.TrimEnd(' ').TrimEnd(',');
        }

        public static Dictionary<string, string> ShortGRBSNames
        {
            get
            {
                // если словарь пустой
                if (shortGRBSNames == null || shortGRBSNames.Count == 0)
                {
                    // заполняем его
                    FillShortGRBSNames();
                }
                return shortGRBSNames;
            }
        }

        public static string GetShortGRBSName(string fullName)
        {
            if (ShortGRBSNames.ContainsKey(fullName))
            {
                return ShortGRBSNames[fullName];
            }
            return fullName;
        }

        private static Dictionary<string, string> shortGRBSNames = new Dictionary<string, string>();
        private static Dictionary<string, string> fullGRBSNames = new Dictionary<string, string>();
        private static Dictionary<string, string> shortGRBSCodes = new Dictionary<string, string>();

        private static void FillShortGRBSNames()
        {
            //   shortGRBSNames = new Dictionary<string, string>();
            //   shortGRBSCodes = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("shortGRBSNames");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Краткие наименования ГРБС", dt);

            foreach (DataRow row in dt.Rows)
            {
                shortGRBSNames.Add(row[0].ToString(), row[1].ToString());
                shortGRBSCodes.Add(row[1].ToString(), row[2].ToString());

                fullGRBSNames.Add(row[1].ToString(), row[0].ToString());
            }
        }
 


        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0042_0002_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            avgValue=0;
            foreach (DataRow row in dtChart.Rows)
            {
                if (row[0] != DBNull.Value)
                {
                    avgValue += double.Parse(row[1].ToString());
                }
            }
            avgValue= avgValue / dtChart.Rows.Count;
            UltraChart.DataSource = dtChart;
            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>, <DATA_VALUE:N2>%";
           /* UltraChart.Series.Clear();
            for (int i = 1; i < dtChart.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                series.Label = dtChart.Columns[i].ColumnName;
                UltraChart.Series.Add(series);
            }*/
        //    UltraChart.Tooltips.FormatString = "<ITEM_LABEL>, <b><DATA_VALUE:0.##></b>";
         //   UltraChart.Tooltips.Display = TooltipDisplay.MouseMove;
        }

        protected void UltraChart_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text ="Нет данных";
            e.LabelStyle.FontColor = Color.Black;
            e.LabelStyle.FontSizeBestFit = false;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
        }

        

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

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            int row = 0;
                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Primitive primitive = e.SceneGraph[i];
                    if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                    {
                        Text axisText = (Text)primitive;
                        axisText.bounds.Width = 30;
                        axisText.labelStyle.VerticalAlign = StringAlignment.Near;
                        axisText.labelStyle.FontSizeBestFit = false;
                        axisText.labelStyle.Font = new Font("Verdana", 8);
                        axisText.labelStyle.WrapText = false;
                        axisText.SetTextString(GetShortGRBSName(axisText.GetTextString()));
                    }
                    if (primitive is Box)
                    {
                        Box box = (Box)primitive;
                        if (box.DataPoint != null && box.Value != null)
                        {
                            box.DataPoint.Label = box.DataPoint.Label.Replace("\"", "\'");
                        }
                    }
                }

                IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
                IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

                if (xAxis == null || yAxis == null)
                    return;

                int textWidht = 200;
                int textHeight = 12;
                int lineStart = (int)xAxis.MapMinimum;
                int lineLength = (int)xAxis.MapMaximum;

                Line line = new Line();
                line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                line.PE.Stroke = Color.DarkGray;
                line.PE.StrokeWidth = 2;


                //     line.p1 = new Point(lineStart, (int)yAxis.Map(100 * avgValue));
                //      line.p2 = new Point(lineStart + lineLength, (int)yAxis.Map(100 * avgValue));
                line.p1 = new Point(lineStart, (int)yAxis.Map(avgValue));
                line.p2 = new Point(lineStart + lineLength, (int)yAxis.Map(avgValue));
                e.SceneGraph.Add(line);

                Text text = new Text();
                text.PE.Fill = Color.Black;
                text.bounds = new Rectangle(lineLength - textWidht, ((int)yAxis.Map(avgValue)) - textHeight, textWidht, textHeight);
                text.SetTextString(string.Format("Средняя оценка: {0:P2}", avgValue / 100));
                e.SceneGraph.Add(text);
        
        }

        #endregion

        #region Экспорт в Excel 

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart.Legend.Margins.Right = 5;

            UltraGridExporter.ChartExcelExport(e.CurrentWorksheet.Rows[2].Cells[0], UltraChart);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(emptyExportGrid);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

            UltraGridExporter1.PdfExporter.Export(new UltraWebGrid());
        }


        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            
            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart.Legend.Margins.Right = 5;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        #endregion
    }
}

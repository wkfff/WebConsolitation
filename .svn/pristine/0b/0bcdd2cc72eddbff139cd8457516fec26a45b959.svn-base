using System;
using System.Collections.Generic;
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
using Image=System.Drawing.Image;

namespace Krista.FM.Server.Dashboards.reports.FO_0042_0002_HMAO
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtChartAVG;
        private DataTable dtChart;
        private int firstYear = 2009;
        private int endYear = 2011;

        private double avgValue;
        private static MemberAttributesDigest grbsDigest;

        #endregion

        #region Параметры запроса

        // выбранный период
        private CustomParam selectedPeriod;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            
            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.75);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += CRHelper.UltraChartInvalidDataReceived;

            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Border.Thickness = 0;

            UltraChart.ColumnChart.SeriesSpacing = 1;
            UltraChart.ColumnChart.ColumnSpacing = 1;

            UltraChart.Axis.X.Extent = 240;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart.Axis.X.StripLines.PE.FillOpacity = 150;
            UltraChart.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
            UltraChart.Axis.X.StripLines.Interval = 2;
            UltraChart.Axis.X.StripLines.Visible = true;
            UltraChart.Axis.Y.Extent = 50;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>%";
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
            appearance.ItemFormatString = "<DATA_VALUE:N1>";
            appearance.ChartTextFont = new Font("Verdana", 8);
            appearance.Visible = true;
            UltraChart.ColumnChart.ChartText.Add(appearance);

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:N1>%";

            #endregion

            UltraGridExporter1.MultiHeader = false;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;оценки&nbsp;качества";
            CrossLink1.NavigateUrl = "~/reports/FO_0042_0001_HMAO/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Динамика&nbsp;результатов&nbsp;оценки&nbsp;качества";
            CrossLink3.NavigateUrl = "~/reports/FO_0042_0004_HMAO/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0042_0002_HMAO_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string quarter = dtDate.Rows[0][2].ToString();
                
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboQuarter.Title = "Оценка качества";
                ComboQuarter.Width = 300;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillDateQuarters());
                ComboQuarter.SetСheckedState(GetParamQuarter(quarter), true);

                grbsDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0042_0002_HMAO_grbsList");
            }

            int quarterNum = ComboQuarter.SelectedIndex + 1;

            Page.Title = String.Format("Рейтинг главных распорядителей средств бюджета автономного округа, главных администраторов доходов бюджета ХМАО-Югры, сформированный по результатам мониторинга качества финансового менеджмента");
            PageTitle.Text = Page.Title;

            PageSubTitle.Text = (quarterNum != 4)
                ? String.Format("{0}.{1}", ComboQuarter.SelectedValue, ComboYear.SelectedValue)
                : String.Format("по итогам {0} года", ComboYear.SelectedValue);

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(quarterNum));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", quarterNum);

            selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);

            FillGRBSDictionary();
            AVGChartDataBind();
            UltraChart.DataBind();
        }

        /// <summary>
        /// Получить элемент параметра по значению классификатора
        /// </summary>
        /// <param name="classQuarter">элемент классификатора</param>
        /// <returns>значение параметра</returns>
        private static string GetParamQuarter(string classQuarter)
        {
            switch (classQuarter)
            {
                case "Квартал 1":
                    {
                        return "по состоянию на 01.04";
                    }
                case "Квартал 2":
                    {
                        return "по состоянию на 01.07";
                    }
                case "Квартал 3":
                    {
                        return "по состоянию на 01.10";
                    }
                case "Данные года":
                    {
                        return "по итогам года";
                    }
                default:
                    {
                        return classQuarter;
                    }
            }
        }

        #region Обработчики диаграммы

        private Dictionary<string, string> grbsDictionary;

        private void FillGRBSDictionary()
        {
            grbsDictionary = new Dictionary<string, string>();

            grbsDictionary.Add("10", "Дума Югры");
            grbsDictionary.Add("20", "Правительство Югры");
            grbsDictionary.Add("30", "Представительство Югры");
            grbsDictionary.Add("40", "Представительство Югры в г. Тюмени");
            grbsDictionary.Add("120", "РСТ Югры");
            grbsDictionary.Add("130", "Здравнадзор Югры");
            grbsDictionary.Add("160", "Ветуправление Югры");
            grbsDictionary.Add("170", "Гостехнадзор Югры");
            grbsDictionary.Add("177", "ГУ по делам ГО и ЧС Югры");
            grbsDictionary.Add("180", "Депдорхоз и транспорта Югры");
            grbsDictionary.Add("188", "УВД Югры");
            grbsDictionary.Add("210", "Депинвест и технологий Югры");
            grbsDictionary.Add("230", "Депобразования и молодежи Югры");
            grbsDictionary.Add("240", "Депкультуры Югры");
            grbsDictionary.Add("250", "Департамент общественных связей Югры");
            grbsDictionary.Add("260", "Депздрав Югры");
            grbsDictionary.Add("270", "Депспорт Югры");
            grbsDictionary.Add("280", "Комитет по молодежной политике Югры");
            grbsDictionary.Add("290", "Депсоцразвития Югры");
            grbsDictionary.Add("340", "Депэкологии Югры");
            grbsDictionary.Add("350", "Дептруда и занятости Югры");
            grbsDictionary.Add("360", "Дорожный департамент Югры");
            grbsDictionary.Add("370", "Департамент гражданской защиты населения Югры");
            grbsDictionary.Add("380", "Депюстиции Югры");
            grbsDictionary.Add("390", "Представительство Югры в г. Санкт-Петербурге");
            grbsDictionary.Add("400", "Депприродресурс и несырьевого сектора экономики Югры");
            grbsDictionary.Add("410", "Обрнадзор Югры");
            grbsDictionary.Add("420", "Жилстройнадзор Югры");
            grbsDictionary.Add("430", "Депимущества Югры");
            grbsDictionary.Add("440", "Избирательная комиссия Югры");
            grbsDictionary.Add("450", "Комитет инфмониторинга Югры");
            grbsDictionary.Add("460", "Депстрой Югры");
            grbsDictionary.Add("470", "Управление АПК Югры");
            grbsDictionary.Add("480", "Депстройэнергетики и ЖКК Югры");
            grbsDictionary.Add("490", "Депмалочислнародов Севера Югры");
            grbsDictionary.Add("500", "Депфин Югры");
            grbsDictionary.Add("510", "Депнедра Югры");
            grbsDictionary.Add("520", "Госкультохрана Югры");
            grbsDictionary.Add("530", "Природнадзор Югры");
            grbsDictionary.Add("540", "Представительство Югры в г. Екатеринбурге");
            grbsDictionary.Add("550", "Архивная служба Югры");
            grbsDictionary.Add("560", "Депгосзаказа Югры");
            grbsDictionary.Add("570", "Депинформтехнологий Югры");
            grbsDictionary.Add("580", "Депполитики Югры");
            grbsDictionary.Add("590", "Департамент управделами Югры");
            grbsDictionary.Add("600", "Депэкономики Югры");
            grbsDictionary.Add("610", "Депжилполитики Югры");
            grbsDictionary.Add("620", "Депздрав Югры");
            grbsDictionary.Add("630", "Ветслужба Югры");
            grbsDictionary.Add("640", "Правительство Югры");
        }

        private string GetShortGRBSName(string grbsName)
        {
            string grbsCode = grbsDigest.GetMemberType(grbsName);
            if (grbsDictionary.ContainsKey(grbsCode))
            {
                return grbsDictionary[grbsCode];
            }
            return grbsName;
        }

        private static string GetWrapText(string text, int charCount)
        {
            bool wrap = false;
            string wrapText = String.Empty;
            int rowLenght = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (!wrap)
                {
                    wrap = (rowLenght == charCount);
                }

                if (wrap && text[i] == ' ')
                {
                    wrapText += '\n';
                    wrap = false;
                    rowLenght = 0;
                }
                else
                {
                    rowLenght++;
                    wrapText += text[i];
                }
            }

            return wrapText;
        }

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0042_0002_HMAO_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            UltraChart.Series.Clear();
            for (int i = 1; i < dtChart.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                series.Label = dtChart.Columns[i].ColumnName;
                UltraChart.Series.Add(series);
            }
        }

        protected void AVGChartDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0042_0002_HMAO_avg_chart");
            dtChartAVG = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChartAVG);

            avgValue = GetDoubleDTValue(dtChartAVG, "Средняя оценка");
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
                    axisText.labelStyle.WrapText = true;

                    axisText.SetTextString(GetShortGRBSName(axisText.GetTextString()));
                }
                
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {
                        box.DataPoint.Label = GetWrapText(box.DataPoint.Label, 65);
                    }
                }
            }
            
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            int textWidht = 200;
            int textHeight = 12;
            double lineStart = xAxis.MapMinimum;
            double lineLength = xAxis.MapMaximum;

            Line line = new Line();
            line.lineStyle.DrawStyle = LineDrawStyle.Dash;
            line.PE.Stroke = Color.DarkGray;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point((int)lineStart, (int)yAxis.Map(avgValue));
            line.p2 = new Point((int)lineStart + (int)lineLength, (int)yAxis.Map(avgValue));
            e.SceneGraph.Add(line);

            Text text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle((int)lineLength - (int)textWidht, ((int)yAxis.Map(avgValue)) - textHeight, textWidht, textHeight);
            text.SetTextString(string.Format("Средняя оценка: {0:N1}", avgValue));
            e.SceneGraph.Add(text);
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;

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

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);

            UltraChart.Width = Unit.Pixel((int) (CustomReportConst.minScreenWidth * 0.8));
            UltraChart.Legend.Margins.Right = 5;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0040_0001
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtIndicatorDescription;
        private DataTable dtChart;
        private DataTable dtChartAVG;
        private int firstYear = 2010;
        private int endYear = 2011;
        private int selectedQuarterIndex;
        private int selectedYear;

        private static Dictionary<string, string> indicatorNameList;
        private static Dictionary<string, string> indicatorDirectionList;
        private static Dictionary<string, string> indicatorCodeNameList;
        private static Dictionary<string, string> indicatorFormatList;

        private double beginQualityLimit;
        private double endQualityLimit;
        private double avgValue;

        private int beginQualityIndex;
        private int endQualityIndex;

        private GridHeaderLayout headerLayout;

        #endregion

        private bool MOGroupDisplay
        {
            get { return MOGroupDisplayCheckBox.Checked; }
        }

        #region Параметры запроса

        // выбранный начальный период
        private CustomParam selectedPeriod;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            double scale = 0.75;

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * scale - 220);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.63);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #region Инициализация параметров запроса

            if (selectedPeriod == null)
            {
                selectedPeriod = UserParams.CustomParam("selected_period");
            }

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.Data.ZeroAligned = true;

            UltraChart.ColumnChart.SeriesSpacing = 1;
            UltraChart.ColumnChart.ColumnSpacing = 1;

            UltraChart.Axis.X.Extent = 140;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart.Axis.X.StripLines.PE.FillOpacity = 150;
            UltraChart.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
            UltraChart.Axis.X.StripLines.Interval = 2;
            UltraChart.Axis.X.StripLines.Visible = true;
            UltraChart.Axis.Y.Extent = 20;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);

            UltraChart.Axis.X.Labels.WrapText = true;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL>";

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart.Height.Value) / 4;
            UltraChart.TitleLeft.Text = "Баллы";
            UltraChart.TitleLeft.Font = new Font("Verdana", 8);

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

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>\n<SERIES_LABEL>: <DATA_VALUE:N2>";

            #endregion
            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            CrossLink1.Visible = true;
            CrossLink1.Text = "Динамика&nbsp;основных&nbsp;показателей";
            CrossLink1.NavigateUrl = "~/reports/FO_0040_0002/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Картограмма&nbsp;результатов&nbsp;мониторинга";
            CrossLink2.NavigateUrl = "~/reports/FO_0040_0003/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Диаграмма&nbsp;динамики&nbsp;основных&nbsp;показателей";
            CrossLink3.NavigateUrl = "~/reports/FO_0040_0004/Default.aspx";
        }



        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(UltraChart);
                chartWebAsyncPanel.AddLinkedRequestTrigger(MOGroupDisplayCheckBox.ClientID);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0040_0001_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string quarter = dtDate.Rows[0][2].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboQuarter.Title = "Квартал";
                ComboQuarter.Width = 300;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillMonitoringQuarters());
                ComboQuarter.SetСheckedState(GetParamQuarter(quarter), true);
            }
            
            selectedYear = Convert.ToInt32(ComboYear.SelectedValue);
            selectedQuarterIndex = ComboQuarter.SelectedIndex + 1;
            
            Page.Title = String.Format("Результаты мониторинга качества управления бюджетным процессом и соблюдения требований бюджетного законодательства РФ муниципальными образованиями Самарской области");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = GetDateQuarterText(selectedQuarterIndex, selectedYear);
            chartElementCaption.Text = "Рейтинг муниципальных образований Самарской области по результатам мониторинга";

            UserParams.PeriodYear.Value = selectedYear.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(selectedQuarterIndex));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", selectedQuarterIndex);

            selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", selectedYear, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);
            
            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                IndicatorDescriptionDataBind();
                headerLayout = new GridHeaderLayout(UltraWebGrid);
                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();
            }
            
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
                        return "мониторинг за 1 квартал";
                    }
                case "Квартал 2":
                    {
                        return "мониторинг за 2 квартал";
                    }
                case "Квартал 3":
                    {
                        return "мониторинг за 3 квартал";
                    }
                case "Квартал 4":
                    {
                        return "мониторинг за 4 квартал (по итогам года)";
                    }
                default:
                    {
                        return classQuarter;
                    }
            }
        }

        private static string GetDateQuarterText(int quarterIndex, int year)
        {
            return String.Format("по итогам {0} квартала {1} года", quarterIndex, year);
        }

        #region Обработчики грида
        
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0040_0001_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtGrid);
            if ((dtGrid.Rows.Count > 0) && (dtGrid.Columns.Count > 1))
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = row[0].ToString().Replace("район", "р-н");
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }
        
        private static string GetIndicatorFormatString(string indicatorName)
        {
            //switch(indicatorName)
            //{
            //    case "I (1)":
            //    case "I (2)":
            //    case "I (3)":
            //    case "I (5)":
            //    case "II (2)":
            //    case "II (3)":
            //    case "II (5)":
            //    case "II (6)":
            //    case "II (7)":
            //    case "III (5)":
            //    case "Рейтинг":
            //    case "Средняя оценка":
            //        {
            //            return "N2";
            //        }
            //    default:
            //        {
            //            return "N0";
            //        }
            //}
            string format = indicatorFormatList[indicatorName];
            if (format == "ДА/НЕТ")
            {
                return "N0";
            }
            else
            {
                return "N2";
            }
        }

        private static int GetIndicatorColumnWidth(string indicatorName)
        {
            switch (indicatorName)
            {
                case "I (1)":
                case "I (2)":
                case "I (3)":
                case "I (5)":
                case "II (2)":
                case "II (3)":
                case "II (5)":
                case "II (6)":
                case "II (7)":
                case "III (5)":
                    {
                        return 40;
                    }
                default:
                    {
                        return 30;
                    }
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            if (e.Layout.Bands[0].Columns.Count < 1)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(120);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            int beginIndex = 1;
            int lastColumnIndex = e.Layout.Bands[0].Columns.Count - 1;
            string formatString;
            for (int i = beginIndex; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string indicatorName = e.Layout.Bands[0].Columns[i].Header.Caption;
                if (i == (lastColumnIndex - 1))
                {
                    formatString = "N2";
                }
                else
                {
                    formatString = (i == lastColumnIndex) ? "N0" : GetIndicatorFormatString(indicatorName);
                }

                //string formatString = (i == lastColumnIndex) ? "N0" : GetIndicatorFormatString(indicatorName);
                int widthColumn = (i > lastColumnIndex - 4) ? 58 : GetIndicatorColumnWidth(indicatorName);

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[lastColumnIndex].MergeCells = true;
            e.Layout.Bands[0].Columns[lastColumnIndex].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);

            string currentCaption = String.Empty;
            GridHeaderCell headerCell = new GridHeaderCell();

            for (int i = beginIndex; i < e.Layout.Bands[0].Columns.Count - 2; i = i + 1)
            {
                string caption = e.Layout.Bands[0].Columns[i].Header.Caption;
                if (caption != String.Empty)
                {
                    string headerCaption = indicatorDirectionList[caption];
                    string headerHint = indicatorNameList[caption];
                    string codeName = indicatorCodeNameList[caption];

                    e.Layout.Bands[0].Columns[i].Key = headerCaption;

                    if (currentCaption != headerCaption)
                    {
                        headerCell = headerLayout.AddCell(headerCaption);
                        currentCaption = headerCaption;
                    }
                    if (selectedYear < 2011)
                    {
                        headerCell.AddCell(caption, headerHint);
                    }
                    else
                    {
                        if (selectedQuarterIndex < 3)
                        {
                            headerCell.AddCell(caption, headerHint);
                        }
                        else
                        {
                            headerCell.AddCell(codeName, headerHint);
                        }
                    }                   
                }
            }

            headerLayout.AddCell(e.Layout.Bands[0].Columns[lastColumnIndex - 1].Header.Caption);
            headerLayout.AddCell(e.Layout.Bands[0].Columns[lastColumnIndex].Header.Caption);

            headerLayout.ApplyHeaderInfo();
        }

        private static string GetIndicatorCode(string source)
        {
            string[] strs = source.Split(' ');
            if (strs.Length > 0)
            {
                return strs[1].TrimStart('(').TrimEnd(')');
            }
            return source;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != String.Empty)
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

        #region Показатели

        private void IndicatorDescriptionDataBind()
        {
            indicatorNameList = new Dictionary<string, string>();
            indicatorDirectionList = new Dictionary<string, string>();
            indicatorCodeNameList = new Dictionary<string, string>();
            indicatorFormatList = new Dictionary<string, string>();

            string query = DataProvider.GetQueryText("FO_0040_0001_indicatorDescription");
            dtIndicatorDescription = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtIndicatorDescription);

            foreach (DataRow row in dtIndicatorDescription.Rows)
            {
                string code = row[0].ToString();
                string name = row[1].ToString();
                string direction = row[2].ToString();
                string codeName = row[3].ToString();
                string format = row[4].ToString();

                indicatorNameList.Add(code, name);
                indicatorDirectionList.Add(code, direction);
                indicatorCodeNameList.Add(code, codeName);
                indicatorFormatList.Add(code, format);
            }
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0040_0001_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            foreach (DataRow row in dtChart.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (i == 0 && row[i] != DBNull.Value)
                    {
                        row[i] = row[i].ToString().Replace("муниципальный район", "МР");
                        row[i] = row[i].ToString().Replace("Муниципальный район", "МР");
                        row[i] = row[i].ToString().Replace("\"", "'");
                        row[i] = row[i].ToString().Replace(" район", " р-н");
                    }
                }
            }

            UltraChart.Series.Clear();
            for (int i = 1; i < dtChart.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                series.Label = dtChart.Columns[i].ColumnName;
                UltraChart.Series.Add(series);
            }

            if (dtChart != null)
            {
                int currentDegree = 0;
                for (int i = 0; i < dtChart.Rows.Count; i++)
                {
                    if (dtChart.Rows[i][1] != DBNull.Value && dtChart.Rows[i][1].ToString() != String.Empty)
                    {
                        double value = Convert.ToDouble(dtChart.Rows[i][1]);

                        if (i == 0 || currentDegree < GetQualityDegree(value))
                        {
                            currentDegree = GetQualityDegree(value);
                            
                            if (i != 0)
                            {
                                if (currentDegree == 2)
                                {
                                    beginQualityIndex = i - 1;
                                }
                                else
                                {
                                    endQualityIndex = i - 1;
                                }
                            }
                        }
                }
                    }
            }
        }

        protected void AVGChartDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0040_0001_avg_chart");
            dtChartAVG = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChartAVG);

            avgValue = GetDoubleDTValue(dtChartAVG, "Средняя оценка");
            beginQualityLimit = 1;
            endQualityLimit = 0;
        }

        private int GetQualityDegree(double value)
        {
            if (value > beginQualityLimit)
            {
                return 1;
            }
            if (value <= endQualityLimit)
            {
                return 3;
            }
            return 2;
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
                    axisText.labelStyle.WrapText = false;
                }
                if (MOGroupDisplay && primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {
                        box.PE.ElementType = PaintElementType.Gradient;
                        box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;

                        double value = Convert.ToDouble(box.Value);
                        if (value > beginQualityLimit)
                        {
                            box.PE.Fill = Color.LimeGreen;
                            box.PE.FillStopColor = Color.ForestGreen;
                        }
                        else if (value <= endQualityLimit)
                        {
                            box.PE.Fill = Color.OrangeRed;
                            box.PE.FillStopColor = Color.Red;
                        }
                        else
                        {
                            box.PE.Fill = Color.SkyBlue;
                            box.PE.FillStopColor = Color.DeepSkyBlue;
                        }
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
            text.bounds = new Rectangle((int)lineStart + textWidht, ((int)yAxis.Map(avgValue)) - textHeight, textWidht, textHeight);
            text.SetTextString(string.Format("Средняя оценка: {0:N2}", avgValue));
            e.SceneGraph.Add(text);

            if (MOGroupDisplay)
            {
                double xMin = xAxis.MapMinimum;
                double xMax = xAxis.MapMaximum;
                double yMin = yAxis.MapMinimum;
                double yMax = yAxis.MapMaximum;

                double axisStep = (xAxis.Map(1) - xAxis.Map(0)) / 2;

                double beginLineAxisX = xAxis.Map(2 * beginQualityIndex + 1) + axisStep;
                line = new Line();
                line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                line.PE.Stroke = Color.DarkGray;
                line.PE.StrokeWidth = 2;
                line.p1 = new Point((int)beginLineAxisX, (int)yMin);
                line.p2 = new Point((int)beginLineAxisX, (int)yMax - textHeight);
                e.SceneGraph.Add(line);

                double endLineAxisX = xAxis.Map(2 * endQualityIndex + 1) + axisStep;
                line = new Line();
                line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                line.PE.Stroke = Color.DarkGray;
                line.PE.StrokeWidth = 2;
                line.p1 = new Point((int)endLineAxisX, (int)yMin);
                line.p2 = new Point((int)endLineAxisX, (int)yMax - textHeight);
                e.SceneGraph.Add(line);
                
                LabelStyle labelStyle = new LabelStyle();
                labelStyle.HorizontalAlign = StringAlignment.Center;
                labelStyle.Font = new Font("Verdana", 8);
                labelStyle.FontColor = Color.Black;

                text = new Text();
                text.bounds = new Rectangle((int)xMin, (int)yMax - textHeight, (int)(beginLineAxisX - xMin), textHeight);
                text.SetTextString("I группа");
                text.SetLabelStyle(labelStyle);
                e.SceneGraph.Add(text);

                text = new Text();
                text.bounds = new Rectangle((int)beginLineAxisX, (int)yMax - textHeight, (int)(endLineAxisX - beginLineAxisX), textHeight);
                text.SetTextString("II группа");
                text.SetLabelStyle(labelStyle);
                e.SceneGraph.Add(text);

                text = new Text();
                text.bounds = new Rectangle((int)endLineAxisX, (int)yMax - textHeight, (int)(xMax - endLineAxisX), textHeight);
                text.SetTextString("III группа");
                text.SetLabelStyle(labelStyle);
                e.SceneGraph.Add(text);
            }
        }

        #endregion
        
        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

            ReportPDFExporter1.HeaderCellHeight = 70;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart, chartElementCaption.Text, section2);
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");

            SetExportGridParams(headerLayout.Grid);

            ReportExcelExporter1.HeaderCellHeight = 50;
            ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 14);
            ReportExcelExporter1.TitleFont = new Font("Verdana", 16, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new Font("Verdana", 14);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
            ReportExcelExporter1.TitleStartRow = 3;

            ReportExcelExporter1.Export(headerLayout, sheet1, 6);

            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;

            ReportExcelExporter1.Export(UltraChart, chartElementCaption.Text, sheet2, 3);
        }

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
        }

        private static void SetExportGridParams(UltraWebGrid grid)
        {
            string exportFontName = "Verdana";
            int fontSize = 12;
            double coeff = 2.3;
            foreach (UltraGridColumn column in grid.Columns)
            {
                column.Width = Convert.ToInt32(column.Width.Value * coeff);
                column.CellStyle.Font.Name = exportFontName;
                column.Header.Style.Font.Name = exportFontName;
                column.CellStyle.Font.Size = fontSize;
                column.Header.Style.Font.Size = fontSize;
            }
        }

        #endregion
    }
}
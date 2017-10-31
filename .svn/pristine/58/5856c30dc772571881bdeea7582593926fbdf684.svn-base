using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0004_Vologda
{
    public partial class DefaultCompare : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private DataTable dtChart = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 7);
            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.6 - 220);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4 - 65);

            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.StackAreaChart;
            UltraChart.Axis.X.Extent = 80;
            UltraChart.Axis.Y.Extent = 60;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Left;
            UltraChart.Legend.SpanPercentage = 19;
            UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;
            UltraChart.Border.Thickness = 0;
            UltraChart.Data.SwapRowsAndColumns = true;
            UltraChart.TitleBottom.Visible = true;
            UltraChart.TitleBottom.HorizontalAlign = StringAlignment.Center;
            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE_ITEM:N3> млн.руб.";

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Diamond;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            UltraChart.AreaChart.LineAppearances.Add(lineAppearance);

            UltraChart.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion

            //GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

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

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(UltraChart);
                chartWebAsyncPanel.AddRefreshTarget(lbSubject);
                chartWebAsyncPanel.AddRefreshTarget(lbSubjectSub);
                chartWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid);

                gridWebAsyncPanel.AddRefreshTarget(UltraWebGrid);
                gridWebAsyncPanel.AddLinkedRequestTrigger(ComboFO);

                string query = DataProvider.GetQueryText("FK_0001_0004_Vologda_date");

                dtDate = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = month;
                UserParams.Filter.Value = "Российская  Федерация";

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(UserParams.PeriodYear.Value, true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(UserParams.PeriodMonth.Value, true);

                ComboFO.Title = "Субъект РФ";
                ComboFO.Width = 300;
                ComboFO.MultiSelect = true;
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillRegions(RegionsNamingHelper.FoNames, RegionsNamingHelper.RegionsFoDictionary));

                string foName = RegionsNamingHelper.GetFoBySubject(RegionSettings.Instance.Name);
                if (foName != String.Empty)
                {
                    ComboFO.SetСheckedState(foName, true);
                }
                else 
                {
                    ComboFO.SetAllСheckedState(true, false);
                }

                ComboSKIFLevel.Width = 250;
                ComboSKIFLevel.ParentSelect = true;
                ComboSKIFLevel.Title = "Уровень бюджета";
                ComboSKIFLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillSKIFLevels());
                ComboSKIFLevel.SetСheckedState("Консолидированный бюджет субъекта", true);

                ComboIncomes.Width = 300;
                ComboIncomes.Title = "Вид дохода";
                ComboIncomes.MultiSelect = true;
                ComboIncomes.MultipleSelectionType = MultipleSelectionType.SimpleMultiple;
                ComboIncomes.ParentSelect = true;
                ComboIncomes.FillDictionaryValues(FillFullKDIncludingMultipleList());
                ComboIncomes.SetAllСheckedState(true, false);

                lbSubject.Text = string.Empty;
                lbSubjectSub.Text = string.Empty;
            }

            Page.Title = string.Format("Темп роста доходов ({0})", ComboFO.SelectedIndex == 0 ? "РФ" : RegionsNamingHelper.ShortName(ComboFO.SelectedValue));
            Label1.Text = Page.Title; 
            
            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            Label2.Text = string.Format("Сравнение темпа роста фактических доходов по субъектам РФ ({3}) за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum,
                ComboSKIFLevel.SelectedValue);

            int year = Convert.ToInt32(ComboYear.SelectedValue);

            UserParams.KDTotal.Value = (year < 2003) ? "Доходы бюджета - ВСЕГО" : "Доходы - всего в том числе:";
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodEndYear.Value = year.ToString();
            UserParams.PeriodYear.Value = (year - 1).ToString();
            UserParams.PeriodFirstYear.Value = (year - 2).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.SKIFLevel.Value = ComboSKIFLevel.SelectedValue;

            string subjectList = String.Empty;
            foreach (string subject in ComboFO.SelectedValues)
                {
                subjectList += String.Format("[Территории].[Сопоставимый].[{0}],", subject);
            }
            UserParams.Subject.Value = subjectList.TrimEnd(',');

            string kdList = String.Empty;
            foreach (string kd in ComboIncomes.SelectedValues)
            {
                kdList += String.Format("[КД].[Сопоставимый].[{0}],", kd);
            }
            UserParams.KDGroup.Value = kdList.TrimEnd(',');

            lbSubjectSub.Text = "Структурная динамика помесячного поступления фактических доходов";

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            string patternValue = lbSubject.Text;
            int defaultRowIndex = 1;
            if (patternValue == string.Empty)
            {
                patternValue = UserParams.StateArea.Value;
                defaultRowIndex = 0;
            }

            UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
            ActiveGridRow(row);

            if (dtGrid.Rows.Count != 0 && UserParams.KDGroup.Value != String.Empty)
            {
                UltraChart.DataBind();
            }
        }

        #region Обработчики грида

        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {
            if (row == null)
                return;

            string subject = row.Cells[0].Text;
            lbSubject.Text = subject;

            if (RegionsNamingHelper.IsRF(subject))
            {
                UserParams.Subject.Value = "]";
                UserParams.SubjectFO.Value = "]";
            }
            else if (RegionsNamingHelper.IsFO(subject))
            {
                UserParams.Region.Value = subject;
                UserParams.Subject.Value = string.Format("].[{0}]", UserParams.Region.Value);
                UserParams.SubjectFO.Value = string.Format("].[{0}]", UserParams.Region.Value);
            }
            else
            {
                UserParams.Region.Value = RegionsNamingHelper.FullName(row.Cells[1].Text);
                UserParams.StateArea.Value = subject;
                UserParams.Subject.Value = string.Format("].[{0}].[{1}]", UserParams.Region.Value, UserParams.StateArea.Value);
                UserParams.SubjectFO.Value = string.Format("].[{0}]", UserParams.Region.Value);
            }

            if (UserParams.KDGroup.Value != String.Empty)
            {
                UltraChart.DataBind();
            }
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0004_Vologda_compare_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);
            
            if (dtGrid.Columns.Count > 2)
            {
                dtGrid.Columns[1].ColumnName = "ФО";
            }

            UserParams.Filter.Value = ComboFO.SelectedValue;
            UltraWebGrid.DataSource = dtGrid;
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (ComboFO.SelectedIndex != 0 && UltraWebGrid.Rows.Count < 13)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 60;

                int j = (i - 1) % 7;
                switch (j)
                {
                    case 1:
                    case 2:
                        {
                            formatString = "N3";
                            widthColumn = 85;
                            break;
                        }
                    case 3:
                    case 4:
                        {
                            formatString = "P2";
                            widthColumn = 80;
                            break;
                        }
                    case 0:
                    case 5:
                    case 6:
                        {
                            formatString = "P2";
                            widthColumn = 75;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = 250;
            
            e.Layout.Bands[0].Columns[1].Header.Caption = "ФО";
            e.Layout.Bands[0].Columns[1].Header.Title = "Федеральный округ, которому принадлежит субъект";
            e.Layout.Bands[0].Columns[1].Width = 45;

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

            int multiHeaderPos = 2;

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 7)
            {
                ColumnHeader ch = new ColumnHeader(true);
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                ch.Caption = captions[0].TrimEnd('_');

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Исполнено, млн.руб.", "Фактическое исполнение нарастающим итогом с начала года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Исполнено прошлый год, млн.руб.", "Исполнено за аналогичный период прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, "% исполнения", "Процент выполнения годовых назначений");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, "% наполнения бюджета", "Процент наполнения бюджета за аналогичный период прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 4, "Темп роста к прошлому году", "Темп роста исполнения к аналогичному периоду прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 5, "Доля", "Доля дохода в общей сумме доходов выбранного субъекта");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 6, "Доля в прошлом году", "Доля дохода в общей сумме фактических доходов в прошлом году");

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 7;
                ch.RowLayoutColumnInfo.SpanX = 7;
                ch.Style.Wrap = true;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 6; i < e.Row.Cells.Count; i = i + 7)
            {
                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "Падение к прошлому году";
                    }
                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "Рост к прошлому году";
                    }
                }

                if (e.Row.Cells[i + 1].Value != null && e.Row.Cells[i + 1].Value.ToString() != string.Empty &&
                    e.Row.Cells[i + 2].Value != null && e.Row.Cells[i + 2].Value.ToString() != string.Empty)
                {
                    if (Convert.ToDouble(e.Row.Cells[i + 1].Value) < Convert.ToDouble(e.Row.Cells[i + 2].Value))
                    {
                        e.Row.Cells[i + 1].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i + 1].Title = "Доля упала с прошлого года";
                    }
                    else if (Convert.ToDouble(e.Row.Cells[i + 1].Value) > Convert.ToDouble(e.Row.Cells[i + 2].Value))
                    {
                        e.Row.Cells[i + 1].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i + 1].Title = "Доля выросла с прошлого года";
                    }
                }
                string style = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                e.Row.Cells[i].Style.CustomRules = style;
                e.Row.Cells[i + 1].Style.CustomRules = style;
            }

            if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() != string.Empty)
            {
                if (!RegionsNamingHelper.IsSubject(e.Row.Cells[0].Value.ToString()))
                {
                    foreach (UltraGridCell cell in e.Row.Cells)
                    {
                        cell.Style.Font.Bold = true;
                    }
                }
            }

            foreach (UltraGridCell cell in e.Row.Cells)
            {
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

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0004_Vologda_compare_chart");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            foreach (DataColumn column in dtChart.Columns)
            {
                if (column.ColumnName == "Доходы от предпринимательской деятельности ")
                {
                    column.ColumnName = "Доходы от предпринимательской деят.";
                }
            }

            DataTable dt = new DataTable();
            query = DataProvider.GetQueryText("FK_0001_0004_Vologda_compare_chart");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            List<string> yearList = new List<string>();

//            locationList.Clear();
//            double chartWidth = UltraChart.Width.Value;
//            double chartHeight = UltraChart.Height.Value;
//            double legendWidth = 0.01 * UltraChart.Legend.SpanPercentage * chartWidth;
//            double axisYExtent = UltraChart.Axis.Y.Extent;
//            double axisY2Extent = UltraChart.Axis.Y2.Extent;
//            double axisXExtent = UltraChart.Axis.X.Extent;
//
//            double x0 = legendWidth + axisYExtent;
//            double stepX = (chartWidth - x0 - axisY2Extent)/dt.Rows.Count;
//            double y0 = chartHeight - axisXExtent;

            for (int j = 0; j < dtChart.Rows.Count; j++)
            {
                DataRow row = dtChart.Rows[j];
                // если строка первая, либо в первой ячейке Январь
                if (row[0] != DBNull.Value && dt.Rows[j][0] != DBNull.Value && 
                        (row[0].ToString() == "Январь" || j == 0))
                {
                    row[0] = string.Format("{0} - {1}", dt.Rows[j][0], row[0]);
                    yearList.Add(dt.Rows[j][0].ToString());

//                    int x = Convert.ToInt32(x0 + j*stepX);
//                    int y = Convert.ToInt32(y0);
//                    Point p = new Point(x, y);
//                    locationList.Add(p);
                }
                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }
                }
            }

            if (yearList.Count == 1)
            {
                lbSubjectSub.Text = string.Format("Структурная динамика помесячного поступления фактических доходов за {0} год, млн.руб.",
                    yearList[0]);
            }
            else if (yearList.Count > 1)
            {
                lbSubjectSub.Text = string.Format("Структурная динамика помесячного поступления фактических доходов за {0} - {1} годы, млн.руб.",
                    yearList[0], yearList[yearList.Count - 1]);
            }


            UltraChart.DataSource = dtChart;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            for (int i = 2; i < UltraWebGrid.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "#,##0.00;[Red]-#,##0.00";
                int widthColumn = 70;

                int j = (i - 2) % 7;
                switch (j)
                {
                    case 0:
                    case 1:
                        {
                            formatString = "#,##0.000;[Red]-#,##0.000";
                            widthColumn = 95;
                            break;
                        }
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        {
                             formatString = "0.00%";
                            widthColumn = 75;
                            break;
                        }
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
            }
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

        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
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
            title.AddContent(Label2.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(lbSubject.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(lbSubjectSub.Text);

            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        #endregion

        /// Заполнить полный список доходами КД (с элементами "в том числе") для множественного выбора
        private static Dictionary<string, int> FillFullKDIncludingMultipleList()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();

            valuesDictionary.Add("Налоговые доходы ", 0);
            valuesDictionary.Add("Налог на прибыль ", 1);
            valuesDictionary.Add("НДФЛ ", 1);
            valuesDictionary.Add("Налоги на имущество ", 1);
            valuesDictionary.Add("Акцизы ", 1);
            valuesDictionary.Add("НДПИ ", 1);
            valuesDictionary.Add("Налоги на совокупный доход ", 1);
            valuesDictionary.Add("Неналоговые доходы ", 0);
            valuesDictionary.Add("Доходы от приносящей доход деятельности ", 0);
            valuesDictionary.Add("Налоговые и неналоговые доходы ", 0);
            valuesDictionary.Add("Безвозмездные поступления ", 0);
            return valuesDictionary;
        }
    }
}

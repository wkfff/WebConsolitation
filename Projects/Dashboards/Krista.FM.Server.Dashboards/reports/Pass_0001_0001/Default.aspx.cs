using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;

using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebNavigator;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.Pass_0001_0001
{

    public partial class Default : CustomReportPage
    {

        #region Поля

        private static DataTable dtGrid;

        private static Dictionary<string, int> dictYear;
        private static Dictionary<string, int> dictQuarter;
        private static Dictionary<string, int> dictMonth;

        private static Dictionary<string, string> paramInfo;

        private static DataProvider dp;

        private static string oldPeriodType = String.Empty;
        
        private static GridHeaderLayout headerLayout;

        private string query;

        private static int selectedRow = 0;

        private static string param = String.Empty;

        private static string unit = String.Empty;

        private string[] prevDates;
        
        #endregion

        #region Параметры запроса

        private CustomParam selectedPeriod;
        private CustomParam periodType;
        private CustomParam selectedParam;

        #endregion

        private static int Resolution
        {
            get { return CRHelper.GetScreenWidth; }
        }
        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");
            selectedParam = UserParams.CustomParam("selected_parameter");
            periodType = UserParams.CustomParam("period_type");

            #endregion

            dp = DataProvidersFactory.SpareMASDataProvider;

            #region Формирование периодов

            DataTable dtData;

            dtData = new DataTable();
            query = DataProvider.GetQueryText("Pass_0001_0001_date");
            dp.GetDataTableForChart(query, periodType.Value, dtData);
            if (dtData.Rows.Count > 0)
            {
                DateTime lastDate = CRHelper.DateByPeriodMemberUName(dtData.Rows[0]["Год"].ToString(), 3);
                dictYear = CustomMultiComboDataHelper.FillYearValues(lastDate.Year - 4, lastDate.Year);

                lastDate = GetQuarterByUName(dtData.Rows[0]["Квартал"].ToString());
                dictQuarter = new Dictionary<string, int>();
                for (DateTime date = lastDate.AddMonths(-21); date <= lastDate; date = date.AddMonths(3))
                {
                    string year = String.Format("{0}", date.Year);
                    string quarter = String.Format("{0} квартал {1:yyyy} года", CRHelper.QuarterNumByMonthNum(date.Month), date);
                    if (!dictQuarter.ContainsKey(year))
                    {
                        dictQuarter.Add(year, 0);
                    }
                    dictQuarter.Add(quarter, 1);
                }

                lastDate = CRHelper.DateByPeriodMemberUName(dtData.Rows[0]["Месяц"].ToString(), 3);
                dictMonth = new Dictionary<string, int>();
                for (DateTime date = lastDate.AddMonths(-11); date <= lastDate; date = date.AddMonths(1))
                {
                    string year = String.Format("{0}", date.Year);
                    string month = String.Format("{0:MMMM yyyy} года", date);
                    if (!dictMonth.ContainsKey(year))
                    {
                        dictMonth.Add(year, 0);
                    }
                    dictMonth.Add(month, 1);
                }
            }
            else
                throw new Exception("Нет данных для построения отчета");

            #endregion

            #region Грид

            UltraWebGrid.Height = Unit.Parse("450px");
            UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #endregion

            #region Диаграмма

            UltraChart1.ChartType = ChartType.ColumnChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Data.ZeroAligned = true;

            UltraChart1.Axis.X.Extent = 20;
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart1.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart1.Axis.X.StripLines.PE.FillOpacity = 150; 
            UltraChart1.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
            UltraChart1.Axis.X.StripLines.Interval = 2;
            UltraChart1.Axis.X.StripLines.Visible = true;
            UltraChart1.Axis.Y.Extent = 40;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart1.Axis.X.Labels.WrapText = true;
            UltraChart1.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Axis.X.Labels.HorizontalAlign = StringAlignment.Center;

            UltraChart1.Legend.Visible = false;

            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>\n<b><DATA_VALUE:N2></b>";
            UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);

            #endregion

            #region Экспорт

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            #endregion

            // Установка размеров
            if (Resolution < 900)
            {
                UltraWebGrid.Width = Unit.Parse("725px");
            }
            else if (Resolution < 1200)
            {
                UltraWebGrid.Width = Unit.Parse("950px");
            }
            else if (Resolution < 1800)
            {
                UltraWebGrid.Width = Unit.Parse("1200px");
            }
            else
            {
                UltraWebGrid.Width = Unit.Parse("1800px");
            }
            UltraChart1.Width = UltraWebGrid.Width;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                ComboPeriodType.Title = "Периодичность";
                ComboPeriodType.Width = 200;
                ComboPeriodType.MultiSelect = false;
                Dictionary<string, int> dict = new Dictionary<string, int>();
                dict.Add("Год", 0);
                dict.Add("Квартал", 0);
                dict.Add("Месяц", 0);
                ComboPeriodType.FillDictionaryValues(dict);
                ComboPeriodType.SetСheckedState("Год", true);
                ComboPeriodType.AutoPostBack = true;

                PanelCharts.AddLinkedRequestTrigger(UltraWebGrid);
                PanelCharts.AddRefreshTarget(UltraChart1);
            }

            periodType.Value = ComboPeriodType.SelectedValue;

            Page.Title = "Паспорт Ямало-Ненецкого автономного округа";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = "Анализ социально-экономического положения Ямало-Ненецкого автономного округа по основным направлениям развития";
            
            if (!PanelCharts.IsAsyncPostBack && Page.Request.Form.Get("__EVENTTARGET") != "undefined")
            {
                oldPeriodType = periodType.Value;
                ComboPeriod.Title = "Период";
                ComboPeriod.Width = 450;
                ComboPeriod.MultiSelect = true;
                ComboPeriod.ShowSelectedValue = true;
                ComboPeriod.ClearNodes();
                switch (periodType.Value)
                {
                    case "Год":
                        {
                            ComboPeriod.FillDictionaryValues(dictYear);
                            break;
                        }
                    case "Квартал":
                        {
                            ComboPeriod.FillDictionaryValues(dictQuarter);
                            break;
                        }
                    case "Месяц":
                        {
                            ComboPeriod.FillDictionaryValues(dictMonth);
                            break;
                        }
                }
                ComboPeriod.SetAllСheckedState(true, false);
                selectedRow = 0;
            }

            #region Формирование параметра с датами

            selectedPeriod.Value = String.Empty;
            switch (periodType.Value)
            {
                case "Год":
                    foreach (Node node in ComboPeriod.SelectedNodes)
                    {
                        selectedPeriod.Value = String.IsNullOrEmpty(selectedPeriod.Value) ? GetMDXYear(node.Text) : selectedPeriod.Value + ",\n" + GetMDXYear(node.Text);
                    }
                    break;
                case "Квартал":
                    foreach (Node node in ComboPeriod.SelectedNodes)
                    {
                        if (node.Nodes.Count == 0)
                        {
                            selectedPeriod.Value = String.IsNullOrEmpty(selectedPeriod.Value) ? GetMDXQuarter(node.Text) : selectedPeriod.Value + ",\n" + GetMDXQuarter(node.Text);
                        }
                        else
                        {
                            foreach (Node qNode in node.Nodes)
                                selectedPeriod.Value = String.IsNullOrEmpty(selectedPeriod.Value) ? GetMDXQuarter(qNode.Text) : selectedPeriod.Value + ",\n" + GetMDXQuarter(qNode.Text);
                        }
                    }
                    break;
                case "Месяц":
                    foreach (Node node in ComboPeriod.SelectedNodes)
                    {
                        if (node.Nodes.Count == 0)
                        {
                            selectedPeriod.Value = String.IsNullOrEmpty(selectedPeriod.Value) ? GetMDXMonth(node.Text) : selectedPeriod.Value + ",\n" + GetMDXMonth(node.Text);
                        }
                        else
                        {
                            foreach (Node qNode in node.Nodes)
                                selectedPeriod.Value = String.IsNullOrEmpty(selectedPeriod.Value) ? GetMDXMonth(qNode.Text) : selectedPeriod.Value + ",\n" + GetMDXMonth(qNode.Text);
                        }
                    }
                    break;
            }

            #endregion

            if (!PanelCharts.IsAsyncPostBack)
            {
                UltraWebGrid.Bands.Clear();
                headerLayout = new GridHeaderLayout(UltraWebGrid);
                UltraWebGrid.DataBind();
                UltraWebGrid_MergeCells(UltraWebGrid);
                if (UltraWebGrid.Rows.Count > 0)
                {
                    UltraWebGrid.Rows[selectedRow].Activated = UltraWebGrid.Rows[selectedRow].Selected = true;
                    UltraWebGrid_ChangeRow(UltraWebGrid.Rows[selectedRow]);
                }
            }
        }

        private string GetMDXMonth(string date)
        {
            string[] dateElemets = date.Split(' ');
            string month = dateElemets[0];
            int monthNum = CRHelper.MonthNum(month);
            int quarter = CRHelper.QuarterNumByMonthNum(monthNum);
            int halfYear = CRHelper.HalfYearNumByMonthNum(monthNum);
            string year = dateElemets[1];
            return String.Format("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}]",
                year, halfYear, quarter, month);
        }

        private string GetMDXQuarter(string date)
        {
            string[] dateElements = date.Split(' ');
            int quarter = Convert.ToInt32(dateElements[0]);
            int halfYear = CRHelper.HalfYearNumByQuarterNum(quarter);
            string year = dateElements[2];
            return String.Format("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}]",
                year, halfYear, quarter);
        }

        private string GetMDXYear(string year)
        {
            return String.Format("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[{0}]", year);
        }

        private DateTime GetQuarterByUName(string uniqueName)
        {
			if (String.IsNullOrEmpty(uniqueName))
			{
				return DateTime.MinValue;
			}
			int year = 2008;
			int month = 1;
			int day = 1;

			string[] uName = uniqueName.Split('.');

			int pos = 3;
			DateTime result;
			try
			{
				if (pos < uName.Length)
				{
					year = Convert.ToInt32(uName[pos].Trim('[').Trim(']'));
				}
				if (pos + 2 < uName.Length)
				{
					month = Convert.ToInt32(uName[pos + 2].Trim('[').Trim(']').Split(' ')[1]) * 3;
				}

				result = new DateTime(year, month, day);
			}
			catch (Exception e)
			{
				return DateTime.MinValue;
			}
			return result;
		}

        #region Диаграмма

        private void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            LabelChart1.Text = String.Format("Динамика показателя «{0}», {1}", param, unit);

            query = DataProvider.GetQueryText("Pass_0001_0001_chart");
            DataTable dtChart = new DataTable();
            dp.GetDataTableForChart(query, "Диаграмма", dtChart);

            query = DataProvider.GetQueryText("Pass_0001_0001_chart_info");
            DataTable dtChartInfo = new DataTable();
            dp.GetDataTableForChart(query, "Диаграмма", dtChartInfo);


            if (periodType.Value != "Год")
            {
                for (int i = 1; i < dtChart.Columns.Count; ++i)
                {
                    DataColumn c = dtChart.Columns[i];
                    c.ColumnName = c.Caption = String.Format("{0}\n{1} года",
                        c.ColumnName, CRHelper.DateByPeriodMemberUName(dtChartInfo.Rows[0][i].ToString(), 3).Year);
                }
            }

            UltraChart1.Data.SwapRowsAndColumns = false;
            UltraChart1.DataSource = dtChart;

        }

        private void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
        }

        #endregion

        #region Грид

        private void UltraWebGrid_MergeCells(UltraWebGrid grid)
        {
            if (grid.Rows.Count == 0)
                return;
            UltraGridCell startCell = grid.Rows[0].Cells[8];
            string prevRow = String.Format("{0}!{1}", grid.Rows[0].Cells[1].Value, grid.Rows[0].Cells[8].Value);
            foreach (UltraGridRow row in grid.Rows)
            {
                if (row.Index == 0)
                    continue;
                string currRow = String.Format("{0}!{1}", row.Cells[1].Value, row.Cells[8].Value);
                if (currRow != prevRow)
                {
                    startCell.RowSpan = row.Index - startCell.Row.Index;
                    prevRow = currRow;
                    startCell = row.Cells[8];
                }
                if (row.Index == grid.Rows.Count - 1)
                {
                    startCell.RowSpan = row.Index - startCell.Row.Index + 1;
                }
            }
        }

        void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            UltraWebGrid_ChangeRow(e.Row);
        }

        protected void UltraWebGrid_ChangeRow(UltraGridRow row)
        {
            if (row == null)
                return;
            selectedRow = row.Index;
            selectedParam.Value = row.Cells[3].GetText();
            param = row.Cells[1].GetText();
            unit = row.Cells[8].GetText();
            UltraChart1.DataBind();
        }

        private void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            UltraGridRow row = e.Row;
            string rowType = row.Cells[6].GetText();
            string cellFormat = rowType == "Темп прироста" ? "{0:P2}" : "{0:N2}";
            bool isPercentRow = row.Cells[5].GetText().Contains("процент");
            if (row.Cells[9].Value != null)
                row.Cells[1].Title = row.Cells[9].GetText();
            for (int i = 10; i < row.Cells.Count; ++i)
            {
                UltraGridCell cell = row.Cells[i];
                cell.Style.Font.Bold = rowType == "Значение";
                if (MathHelper.IsDouble(cell.Value))
                {
                    double cellValue = Convert.ToDouble(cell.Value);
                    if (isPercentRow && rowType == "Значение" && cellValue != 100)
                    {
                        if (cellValue < 100)
                        {
                            if (IsReverseParam(row.Cells[4].GetText()))
                            {
                                cell.Title = "Положительное снижение";
                                cell.Style.CssClass = "BallGreen";
                            }
                            else
                            {
                                cell.Title = "Отрицательное снижение";
                                cell.Style.CssClass = "BallRed";
                            }
                        }
                        else
                        {
                            if (IsReverseParam(row.Cells[4].GetText()))
                            {
                                cell.Title = "Отрицательный рост";
                                cell.Style.CssClass = "BallRed";
                            }
                            else
                            {
                                cell.Title = "Положительный рост";
                                cell.Style.CssClass = "BallGreen";
                            }
                        }

                    }
                    cell.Value = String.Format(cellFormat, cellValue);
                    if (rowType == "Прирост")
                    {
                        cell.Title = String.Format("Прирост {0}", prevPeriod[i - 9]);
                    }
                    else if (rowType == "Темп прироста")
                    {
                        cell.Title = String.Format("Темп роста {0}", prevPeriod[i - 9]);
                    }
                    if (rowType == "Темп прироста")
                        if (cellValue > 1)
                        {
                            if (IsReverseParam(row.Cells[4].GetText()))
                                cell.Style.CssClass = "ArrowUpRed";
                            else
                                cell.Style.CssClass = "ArrowUpGreen";
                        }
                        else if (cellValue < 1)
                        {
                            if (IsReverseParam(row.Cells[4].GetText()))
                                cell.Style.CssClass = "ArrowDownGreen";
                            else
                                cell.Style.CssClass = "ArrowDownRed";
                        }
                }
            }
        }

        private void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            UltraWebGrid grid = sender as UltraWebGrid;
            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowDeleteDefault = AllowDelete.No;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.CellClickActionDefault = CellClickAction.RowSelect;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.SelectTypeRowDefault = SelectType.Single;
            e.Layout.HeaderStyleDefault.Wrap = true;

            UltraGridBand band = e.Layout.Bands[0];

            band.Columns[0].MergeCells = true;
            band.Columns[1].MergeCells = true;
            //band.Columns[8].MergeCells = true;
            band.Columns[0].CellStyle.Wrap = true;
            band.Columns[1].CellStyle.Wrap = true;
            band.Columns[8].CellStyle.Wrap = true;
            band.Columns[0].Width = Unit.Parse("100px");
            band.Columns[1].Width = Unit.Parse("200px");
            band.Columns[8].Width = Unit.Parse("150px");
            for (int i = 10; i < band.Columns.Count; ++i)
            {
                band.Columns[i].Width = Unit.Parse("100px");
                band.Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                band.Columns[i].CellStyle.VerticalAlign = VerticalAlign.Middle;
            }

            headerLayout.AddCell("Направление");
            headerLayout.AddCell("Показатель");
            headerLayout.AddCell("Показатель без единицы измерения");
            headerLayout.AddCell("MDX");
            headerLayout.AddCell("Примечание");
            headerLayout.AddCell("Просто единица измерения");
            headerLayout.AddCell("Тип строки");
            headerLayout.AddCell("Краткое наименование");
            headerLayout.AddCell("Единица измерения");
            headerLayout.AddCell("Методология");
            string prevYear = String.Empty;
            GridHeaderCell yearCell = new GridHeaderCell();
            for (int i = 10; i < band.Columns.Count; ++i)
            {
                if (periodType.Value == "Год")
                {
                    headerLayout.AddCell(band.Columns[i].Key);
                }
                else
                {
                    string[] separator = { " год " };
                    string year = band.Columns[i].Key.Split(separator, StringSplitOptions.None)[1];
                    string period = band.Columns[i].Key.Split(separator, StringSplitOptions.None)[0];
                    if (year != prevYear)
                    {
                        prevYear = year;
                        yearCell = headerLayout.AddCell(year);
                    }
                    yearCell.AddCell(period);
                }
            }
            headerLayout.ApplyHeaderInfo();
            
            band.Columns[2].Hidden = true;
            band.Columns[3].Hidden = true;
            band.Columns[4].Hidden = true;
            band.Columns[5].Hidden = true;
            band.Columns[6].Hidden = true;
            band.Columns[7].Hidden = true;
            //band.Columns[8].Hidden = true;
            band.Columns[9].Hidden = true;
        }

        private string[] prevPeriod;

        private void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            query = DataProvider.GetQueryText("Pass_0001_0001_grid_info");
            DataTable dtInfo = new DataTable();
            dp.GetDataTableForChart(query, "Показатель", dtInfo);
            paramInfo = new Dictionary<string, string>();
            string[] separator = { "<br/>" };
            foreach (DataRow row in dtInfo.Rows)
            {
                paramInfo.Add(row["Показатель"].ToString(), String.Format("{0}{7}{1}{7}{2}{7}{3}{7}{4}{7}{5}{7}{6}",
                    row["Единица измерения"], row["Направление"], row["MDX"], row["Примечание"], row["Доп единицы изм"], row["Краткое наименование"], row["Методология"], separator[0]));
            }

            query = DataProvider.GetQueryText("Pass_0001_0001_grid_year");
            dtGrid = new DataTable();
            dp.GetDataTableForChart(query, "Таблица", dtGrid);

            if (periodType.Value != "Год")
            {
                for (int i = 11; i < dtGrid.Columns.Count; ++i)
                {
                    dtGrid.Columns[i].ColumnName = dtGrid.Columns[i].Caption = String.Format("{0} год {1}",
                        dtGrid.Columns[i].ColumnName, CRHelper.DateByPeriodMemberUName(dtGrid.Rows[0][i].ToString(), 3).Year);
                }
            }
            dtGrid.Rows.RemoveAt(0);
            dtGrid.AcceptChanges();

            prevPeriod = new string[dtGrid.Columns.Count - 10];
            for (int i = 0; i < prevPeriod.Length; ++i)
            {
                DateTime prevDate = CRHelper.DateByPeriodMemberUName(dtGrid.Rows[0][i + 10].ToString(), 3);
                if (periodType.Value == "Год")
                {
                    prevPeriod[i] = String.Format("к {0} году", prevDate.Year);
                }
                else if (periodType.Value == "Квартал")
                {
                    prevPeriod[i] = String.Format("к {0} кварталу {1} года",
                        CRHelper.QuarterNumByMonthNum(GetQuarterByUName(dtGrid.Rows[0][i + 10].ToString()).Month), prevDate.Year);
                }
                else
                {
                    prevPeriod[i] = String.Format("к {0} {1} года", CRHelper.RusMonthDat(prevDate.Month), prevDate.Year);
                }
            }
            dtGrid.Rows.RemoveAt(0);
            dtGrid.AcceptChanges();

            int rowIndex = 0;
            foreach (DataRow row in dtGrid.Rows)
            {
                string paramName = row["Таблица"].ToString().Split(';')[0];
                string info;
                paramInfo.TryGetValue(paramName, out info);
                row["Направление"] = info.Split(separator, StringSplitOptions.None)[1];
                row["Показатель"] = info.Split(separator, StringSplitOptions.None)[5];
                row["Показатель без единицы измерения"] = paramName;
                row["MDX"] = info.Split(separator, StringSplitOptions.None)[2];
                row["Примечание"] = info.Split(separator, StringSplitOptions.None)[3];
                row["Единица измерения"] = info.Split(separator, StringSplitOptions.None)[0];
                row["Доп единицы изм"] = info.Split(separator, StringSplitOptions.None)[4];
                row["Методология"] = info.Split(separator, StringSplitOptions.None)[6];
                if (rowIndex % 3 == 0)
                {
                    row["Тип строки"] = "Значение";
                }
                else if (rowIndex % 3 == 1)
                {
                    row["Тип строки"] = "Прирост";
                }
                else
                {
                    row["Тип строки"] = "Темп прироста";
                }
                ++rowIndex;
            }
            dtGrid.Columns.Remove("Таблица");
            dtGrid.AcceptChanges();

            for (rowIndex = dtGrid.Rows.Count - 3; rowIndex >= 0; rowIndex -= 3)
            {
                if (IsEmptyRow(dtGrid.Rows[rowIndex], 10))
                {
                    dtGrid.Rows.RemoveAt(rowIndex + 2);
                    dtGrid.Rows.RemoveAt(rowIndex + 1);
                    dtGrid.Rows.RemoveAt(rowIndex);
                    continue;
                }
                if (!dtGrid.Rows[rowIndex]["Единица измерения"].ToString().Contains("процент"))
                    continue;
                dtGrid.Rows.RemoveAt(rowIndex + 2);
                dtGrid.Rows.RemoveAt(rowIndex + 1);
            }
            dtGrid.AcceptChanges();

            UltraWebGrid.DataSource = dtGrid;

        }
        
        private bool IsEmptyRow(DataRow row, int columnFrom)
        {
            bool result = true;
            DataTable table = row.Table;
            for (int i = columnFrom; i < row.ItemArray.Length; ++i)
            {
                result &= row[i] == DBNull.Value;
            }
            return result;
        }

        private bool IsReverseParam(string info)
        {
            if (info.Contains("обратный"))
                return true;
            else
                return false;
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

            for (int i = 9; i >= 2; --i)
            {
                if (i == 8)
                    continue;
                UltraWebGrid.Columns.RemoveAt(i);
                headerLayout.childCells.RemoveAt(i);
            }

            string oldGroup = String.Empty;
            string oldParam = String.Empty;
            string oldUnit = String.Empty;
            foreach (UltraGridRow row in UltraWebGrid.Rows)
            {
                if (row.Cells[2].Text == oldUnit && row.Cells[0].Text == oldGroup && row.Cells[1].Text == oldParam)
                    row.Cells[2].Text = String.Empty;
                else
                    oldUnit = row.Cells[2].Text;
                if (row.Cells[0].Text == oldGroup)
                    row.Cells[0].Text = String.Empty;
                else
                    oldGroup = row.Cells[0].Text;
                if (row.Cells[1].Text == oldParam)
                    row.Cells[1].Text = String.Empty;
                else
                    oldParam = row.Cells[1].Text;
            }

            ReportPDFExporter1.HeaderCellHeight = 20;
            ReportPDFExporter1.Export(headerLayout, section1);

            UltraChart1.Width = Unit.Point((int)(UltraChart1.Width.Value * 0.5));
            ReportPDFExporter1.Export(UltraChart1, LabelChart1.Text, section2);
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            Worksheet sheet3 = workbook.Worksheets.Add("Пустая страница");

            SetExportGridParams(headerLayout.Grid);

            ReportExcelExporter1.HeaderCellHeight = 25;
            ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Left;
            ReportExcelExporter1.TitleStartRow = 0;

            for (int i = 9; i >= 2; --i)
            {
                if (i == 8)
                    continue;
                UltraWebGrid.Columns.RemoveAt(i);
                headerLayout.childCells.RemoveAt(i);
            }

            foreach (UltraGridRow row in UltraWebGrid.Rows)
                row.Selected = row.Activated = false;

            ReportExcelExporter1.Export(headerLayout, sheet1, 2);
            
            string oldGroup = String.Empty;
            string oldParam = String.Empty;
            string oldUnit = String.Empty;
            int startCellRow = -1;
            foreach (UltraGridRow row in UltraWebGrid.Rows)
            {
                if (!(row.Cells[2].Text == oldUnit && row.Cells[1].Text == oldParam && row.Cells[0].Text == oldGroup))
                {
                    if (startCellRow != -1)
                    {
                        if (periodType.Value == "Год")
                            sheet1.MergedCellsRegions.Add(startCellRow + 3, 2, row.Index + 2, 2);
                        else
                            sheet1.MergedCellsRegions.Add(startCellRow + 4, 2, row.Index + 3, 2);
                    }
                    oldGroup = row.Cells[0].Text;
                    oldParam = row.Cells[1].Text;
                    oldUnit = row.Cells[2].Text;
                    startCellRow = row.Index;
                }
                if (row.Index == UltraWebGrid.Rows.Count - 1)
                    sheet1.MergedCellsRegions.Add(startCellRow + 4, 2, row.Index + 4, 2);
            }

            sheet1.Rows[0].Cells[0].CellFormat.ShrinkToFit = sheet1.Rows[1].Cells[0].CellFormat.ShrinkToFit = ExcelDefaultableBoolean.True;
            
            //UltraChart1.Width = Unit.Point((int)(UltraChart1.Width.Value * 0.5));
            UltraChart1.Width = Unit.Pixel((int)(UltraChart1.Width.Value * 0.75));
            ReportExcelExporter1.Export(UltraChart1, LabelChart1.Text, sheet2, 3);
            sheet2.Columns[0].Width = (int)(UltraChart1.Width.Value * 0.5 * 256);
            sheet2.Columns[0].CellFormat.ShrinkToFit = ExcelDefaultableBoolean.True;
            sheet2.MergedCellsRegions.Clear();

            ReportExcelExporter1.Export(new GridHeaderLayout(emptyExportGrid), sheet3, 0);
            workbook.Worksheets.Remove(sheet3);
        }

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0.25;
            //e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
        }

        private static void SetExportGridParams(UltraWebGrid grid)
        {
            string exportFontName = "Verdana";
            int fontSize = 10;
            double coeff = 1.1;
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
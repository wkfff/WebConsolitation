using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
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
using InitializeRowEventHandler=Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler;

namespace Krista.FM.Server.Dashboards.reports.FO_0035_0002
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtExecute = new DataTable();
        private DataTable dtExecuteOutcome = new DataTable();
        private DataTable dtChart = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int lastDataElementIndex = 0;
        private UltraWebGrid grid = new UltraWebGrid();
        private DateTime lastDataDate;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            gridPlaceHolder.Controls.Clear();

            grid = new UltraWebGrid();
            grid.EnableViewState = false;
            grid.DisplayLayout.Reset();
            grid.Bands.Clear();
            grid.SkinID = "UltraWebGrid";
            gridPlaceHolder.Controls.Add(grid);

            grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 50);
            ultraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
            ultraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenWidth * 0.6 - 130);
            grid.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenWidth * 0.6 - 130);
            grid.DisplayLayout.RowHeightDefault = 23;

            ComboYear.Width = 100;
            ComboYear.Title = "Год";
            ComboQuater.Title = "Квартал";

            ComboQuater.Width = 150;
            Parameter.Width = 600;

            Link1.Visible = true;
            Link1.Text = "Исполнение&nbsp;кассового&nbsp;плана";
            Link1.NavigateUrl = "~/reports/FO_0035_0001/Default.aspx";

            Link2.Visible = true;
            Link2.Text = "Структура&nbsp;расходов&nbsp;областного&nbsp;бюджета&nbsp;по&nbsp;ведомствам";
            Link2.NavigateUrl = "~/reports/FO_0035_0003/Kinds.aspx";

            Link3.Visible = true;
            Link3.Text = "Исполнение&nbsp;кассового&nbsp;плана&nbsp;(с&nbsp;процентом&nbsp;исполнения)";
            Link3.NavigateUrl = "~/reports/FO_0035_0003/Executed.aspx";

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.MultiHeader = true;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            // Получаем последнюю дату
            lastDataDate = GetLastDataDate();

            if (!Page.IsPostBack)
            {
                InitializeComboDate(lastDataDate);
                UserParams.CubeName.Value = "[ФО_Исполнение кассового плана]";
                lbChartTitle.Text = "Остаток средств бюджета";
            }
            // колотим дату из календаря
            DateTime date = GetDateFromParametrs();

            bool fullQuater = date < lastDataDate;
            // Выбираем последнюю дату с данными.
            lastDataDate = fullQuater ? date : lastDataDate;

            // Инициализируем параметры даты из последней даты
            UserParams.PeriodQuater.Value =
                CRHelper.PeriodMemberUName(String.Empty, lastDataDate, 3);

            // Заполним дерево
            if (!IsPostBack)
            {
                Dictionary<string, int> parametrs = GetParametrsCollection();
                Parameter.FillDictionaryValues(parametrs);
                Parameter.SetСheckedState("Остаток средств на конец квартала", true);
            }
            Parameter.ParentSelect = true;
            // Выставляем параметры.
            SetParamsByGroup();
            // биндим данные.
            ultraChart.DataBind();
            grid.DataBind();

            // Если в гриде больше 5 колонок
            if (grid.Columns.Count > 5)
            {
                // Нужны дополнительные настройки
                AdditionalSetupGrid();
            }
            // настраиваем лейблы
            SetupTitle(fullQuater);
        }

        private DateTime GetDateFromParametrs()
        {
            int selectedQuaterNum = ComboQuater.SelectedIndex + 1;
            int monthNum = CRHelper.QuarterLastMonth(selectedQuaterNum);
            return new DateTime(
                Int32.Parse(ComboYear.SelectedValue),
                monthNum, CRHelper.MonthLastDay(monthNum));
        }

        private void InitializeComboDate(DateTime date)
        {
            ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(2009, 2011));
            ComboQuater.FillDictionaryValues(CustomMultiComboDataHelper.FillQuaters());
            // В первый раз инициализируем календарь из даты.
            ComboYear.SetСheckedState(date.Year.ToString(), true);
            int quaterNum = CRHelper.QuarterNumByMonthNum(date.Month);
            ComboQuater.SetСheckedState(String.Format("Квартал {0}", quaterNum), true);
        }

        private void SetupTitle(bool fullQuater)
        {
            // Данные или на последний день квартала или на последний день данных
            lbTitle.Text =
                CRHelper.ToUpperFirstSymbol(String.Format("Динамика исполнения кассового плана за {0}",
                                                          CRHelper.PeriodDescr(lastDataDate, 3)));
            string subTitle = string.Empty;
            if (!fullQuater)
            {
                subTitle = "(на " + lastDataDate.ToString("dd.MM.yyyy") + "г.)";
            }
            lbSubTitle.Text = subTitle + " по областному бюджету";
            Parameter.Title = "Показатель";
        }

        private DateTime GetLastDataDate()
        {
            string query = DataProvider.GetQueryText("date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(
                query, dtDate);
            return new DateTime(
                Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                Convert.ToInt32(dtDate.Rows[0][4].ToString()));
        }

        protected void ultraChart_InterpolateValues(object sender, InterpolateValuesEventArgs e)
        {
            double value;
            double.TryParse(dtChart.Rows[0][2].ToString(), out value);
            for (int i = 0; i < e.NullValueIndices.Length; i++)
            {
                if (e.NullValueIndices[i] == 0)
                {
                    e.Values.SetValue(value, e.NullValueIndices[i]);
                }
                else
                    if (e.NullValueIndices[i] > 0 && e.NullValueIndices[i] < lastDataElementIndex)
                    {
                        e.Values.SetValue(e.Values.GetValue(e.NullValueIndices[i] - 1), e.NullValueIndices[i]);
                    }
                    else
                    {
                        e.Values.SetValue(null, e.NullValueIndices[i]);
                    }
            }
        }

        protected void ultraChart_FillSceneGraphRests(object sender, FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;
            Polyline lastLine = null;
            Polyline previousLine = null;
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive pr = e.SceneGraph[i];
                if (pr is Polyline)
                {
                    Polyline line = ((Polyline)pr);
                    previousLine = lastLine;
                    lastLine = ((Polyline)pr);
                    line.Visible = line.points[0].point.X < line.points[line.points.Length - 1].point.X;
                }
            }
            if (previousLine != null)
            {
                previousLine.Visible = false;
                previousLine.points[1].Visible = false;
                previousLine.points[1].hitTestRadius = 0;
                previousLine.points[0].Visible = true;
                previousLine.points[0].hitTestRadius = 6;
            }
            double value;
            double.TryParse(dtChart.Rows[0][2].ToString(), out value);
            Box box = new Box(new Rectangle(
                (int)xAxis.Map(0) - 6, (int)yAxis.Map(value) - 6, 13, 13));

            box.PE.ElementType = PaintElementType.Gradient;
            box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
            box.PE.Fill = Color.FromArgb(101, 162, 203);
            box.PE.FillStopColor = Color.FromArgb(8, 106, 172);
            box.PE.Stroke = Color.Black;
            box.PE.StrokeWidth = 1;
            box.Row = 0;
            box.Column = 2;
            box.Value = 42;
            box.Layer = e.ChartCore.GetChartLayer();
            box.Chart = this.ultraChart.ChartType;
            e.SceneGraph.Add(box);

            Color restsFillColor = Color.Empty;
            Color restsFillStopColor = Color.Empty;
            int restEndIndex = 0;
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text)
                {
                    string legendText = ((Text)primitive).GetTextString();
                    Primitive prevPrimitive = e.SceneGraph[i - 1];
                    if (legendText == "Остаток на конец квартала")
                    {
                        if (prevPrimitive is Box)
                        {
                            restsFillColor = prevPrimitive.PE.Fill;
                            restsFillStopColor = prevPrimitive.PE.FillStopColor;
                            restEndIndex = i - 1;
                            ((Text)primitive).SetTextString("Остаток на начало квартала");
                        }
                    }
                    else if (legendText == "Остаток на начало квартала")
                    {
                        if (prevPrimitive is Box)
                        {
                            e.SceneGraph[restEndIndex].PE.Fill = prevPrimitive.PE.Fill;
                            e.SceneGraph[restEndIndex].PE.FillStopColor = prevPrimitive.PE.FillStopColor;
                            prevPrimitive.PE.Fill = restsFillColor;
                            prevPrimitive.PE.FillStopColor = restsFillStopColor;
                            ((Text)primitive).SetTextString("Остаток на конец квартала");
                        }
                    }
                }
            }
            //  ReplaceAxisLabels(e.SceneGraph);
        }

        private void ultraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            ReplaceAxisLabels(e.SceneGraph);
        }

        private static void ReplaceAxisLabels(SceneGraph grahp)
        {
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
                        int day;
                        if (Int32.TryParse(textArray[0], out day) && CRHelper.IsMonthCaption(textArray[1]))
                        {
                            string primitiveText = day == 1
                                                       ? string.Format("{0}-1", CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(CRHelper.MonthNum(textArray[1]))))
                                                       : day.ToString();
                            ((Text)primitive).SetTextString(primitiveText);
                        }
                    }
                }
            }
        }

        private Dictionary<string, int> GetParametrsCollection()
        {
            Dictionary<string, int> result = new Dictionary<string, int>();

            string query = DataProvider.GetQueryText("Execute");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "name", dtExecute);
            query = DataProvider.GetQueryText("ExecuteOutcome");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "Наименование показателя", dtExecuteOutcome);

            // сначала набиваем первую часть первой таблицы
            int rowsCounter = 0;
            while (!dtExecute.Rows[rowsCounter][0].ToString().Equals("Расходы - всего"))
            {
                string cellValue = dtExecute.Rows[rowsCounter][1].ToString();
                if (cellValue != "31")
                {
                    int level = cellValue[cellValue.Length - 1] == '0' ? 0 : 1;
                    result.Add(dtExecute.Rows[rowsCounter][0].ToString(), level);
                }
                rowsCounter++;
            }
            // потом вторую таблицу
            result.Add(dtExecuteOutcome.Rows[0][0].ToString(), 0);
            for (int i = 2; i < dtExecuteOutcome.Rows.Count; i++)
            {
                result.Add(dtExecuteOutcome.Rows[i][0].ToString(), 1);
            }
            // снова первую, пропустим строчку про расходы
            rowsCounter++;
            for (; rowsCounter < dtExecute.Rows.Count; rowsCounter++)
            {
                string cellValue = dtExecute.Rows[rowsCounter][1].ToString();
                int level = cellValue[cellValue.Length - 1] == '0' ? 0 : 1;
                result.Add(dtExecute.Rows[rowsCounter][0].ToString(), level);
            }

            return result;
        }

        private void RemoveRedudantRows(DataTable dtSource, int monthColumnIndex)
        {
            string lastDataMonth = CRHelper.RusMonth(lastDataDate.Month);
            int lastDataDay = lastDataDate.Day;
            List<int> removingRows = new List<int>();
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                int day;
                // Если день это не день
                if (!Int32.TryParse(dtSource.Rows[i][0].ToString(), out day))
                {
                    removingRows.Add(i - removingRows.Count);
                }
                else
                {
                    int monthNum = CRHelper.MonthNum(dtSource.Rows[i][monthColumnIndex].ToString());
                    string monthGenitive = CRHelper.RusMonthGenitive(monthNum);
                    string month = CRHelper.RusMonth(monthNum);
                    // Допишем месяц
                    dtSource.Rows[i][0] = String.Format("{0} {1}", dtSource.Rows[i][0], monthGenitive);
                    if (lastDataDay == day &&
                        lastDataMonth.ToLower() == month.ToLower())
                    {
                        lastDataElementIndex = i - removingRows.Count;
                    }
                }
            }
            // Поудаляем дни, которые не дни.
            foreach (int index in removingRows)
            {
                dtSource.Rows.RemoveAt(index);
            }
            dtSource.AcceptChanges();
        }

        private static string GetMonth(string month)
        {
            month = month.Trim('(');
            month = month.Replace(" ДАННЫЕ)", String.Empty);
            return month;
        }

        private void SetParamsByGroup()
        {
            string value;
            if ((Parameter.SelectedNode.Parent == null ||
                Parameter.SelectedNode.Parent.Text != "Расходы- всего") &&
                Parameter.SelectedValue != "Расходы- всего")
            {
                UserParams.CubeName.Value = "[ФО_Исполнение кассового плана]";
                if (Rests())
                {
                    // Остатки.
                    SetRestsParams();
                }
                else
                {
                    // Выплаты, поступления, доходы обл бюджета
                    switch (Parameter.SelectedValue)
                    {
                        case ("КАССОВЫЕ ПОСТУПЛЕНИЯ - ВСЕГО"):
                            {
                                ultraChart.DataBinding += new EventHandler(ultraChart_DataBindingIncomesAll);
                                SetGridHandlers();
                                break;
                            }
                        case ("КАССОВЫЕ ВЫПЛАТЫ - ВСЕГО"):
                            {
                                ultraChart.DataBinding += new EventHandler(ultraChart_DataBindingOutcomesAll);
                                SetGridHandlers();
                                break;
                            }
                        case ("Доходы областного бюджета"):
                            {
                                ultraChart.DataBinding += new EventHandler(ultraChart_DataBindingBudget);
                                SetGridHandlers();
                                break;
                            }
                        case ("Поступления и выплаты из источников финансирования дефицита областного бюджета - всего"):
                            {
                                ultraChart.DataBinding += new EventHandler(ultraChart_DataBindingDeficite);
                                SetDeficiteGridHandlers();
                                break;
                            }
                        default:
                            {
                                ultraChart.DataBinding += new EventHandler(ultraChart_DataBindingIncomes);
                                SetGridHandlers();
                                break;
                            }
                    }

                }
                value = Parameter.SelectedValue == "Расходы- всего" ? "Расходы - всего" : Parameter.SelectedValue;
                value = CRHelper.ToUpperFirstSymbol(value);
                SetUpChartLabel(value);
                UserParams.SelectItem.Value = value;
                value = String.Format("[Показатели].[ИспКасПлан].[Все].[{0}]", value);
            }
            else
            {
                if (Parameter.SelectedValue != "Расходы- всего")
                {
                    // Расходы
                    SetOutcomesParams();
                    value = String.Format("[Администратор].[ИспКасПланСопост].[Все].[{0}]", Parameter.SelectedValue);
                }
                else
                {
                    value = "Расходы - всего";
                    SetOutcomesTotalParams();
                }
            }
            UserParams.Organization.Value = value;
            ultraChart.InvalidDataReceived += CRHelper.UltraChartInvalidDataReceived;
            ultraChart.FillSceneGraph += new FillSceneGraphEventHandler(ultraChart_FillSceneGraph);
        }

        private void SetOutcomesTotalParams()
        {
            grid.DataBinding += new EventHandler(grid_DataBindingOutcomesTotal);
            grid.InitializeLayout += new InitializeLayoutEventHandler(grid_InitializeLayoutOutcomes);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExportOutcomes);
            ultraChart.DataBinding += new EventHandler(ultraChart_DataBindingOutcomesTotal);
            grid.InitializeRow += new InitializeRowEventHandler(grid_InitializeRowOutcomes);
            lbChartTitle.Text = "Расходы - всего";
        }

        private bool Rests()
        {
            return Parameter.SelectedValue == "Остаток средств на начало квартала" ||
                   Parameter.SelectedValue == "Остаток средств на конец квартала" ||
                   Parameter.SelectedIndex == -1;
        }

        private void SetDeficiteGridHandlers()
        {
            grid.DataBinding += new EventHandler(grid_DataBindingDeficite);
            grid.InitializeLayout += new InitializeLayoutEventHandler(grid_InitializeLayoutIncomes);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExportIncomes);
            grid.InitializeRow += new InitializeRowEventHandler(grid_InitializeRowIncomes);
        }

        private void SetGridHandlers()
        {
            grid.DataBinding += new EventHandler(grid_DataBindingIncomes);
            grid.InitializeLayout += new InitializeLayoutEventHandler(grid_InitializeLayoutIncomes);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExportIncomes);
            grid.InitializeRow += new InitializeRowEventHandler(grid_InitializeRowIncomes);
        }

        private void ultraChart_DataBindingDeficite(object sender, EventArgs e)
        {
            SetStackChartAppearanceUnic();
            ultraChart.Legend.SpanPercentage = 24;
            ultraChart.Legend.Margins.Right = (int)ultraChart.Width.Value / 3;
            BindChartData("ChartDeficite", 7);
        }

        private void SetUpChartLabel(string value)
        {
            if (Parameter.SelectedNode.Parent != null)
            {
                string parentText = Parameter.SelectedNodeParent.ToLower();
                parentText = CRHelper.ToUpperFirstSymbol(parentText);
                parentText = parentText.Replace("- всего", String.Empty);
                lbChartTitle.Text = parentText + " &#8212 " + value;
            }
            else
            {
                lbChartTitle.Text = value;
            }
        }

        private void SetOutcomesParams()
        {
            grid.DataBinding += new EventHandler(grid_DataBindingOutcomes);
            grid.InitializeLayout += new InitializeLayoutEventHandler(grid_InitializeLayoutOutcomes);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExportOutcomes);
            ultraChart.DataBinding += new EventHandler(ultraChart_DataBindingOutcomes);
            grid.InitializeRow += new InitializeRowEventHandler(grid_InitializeRowOutcomes);
            UserParams.CubeName.Value = "[ФО_Исполнение кассового плана_Расходы]";
            lbChartTitle.Text = "Расходы &#8212 " + Parameter.SelectedValue;
        }

        private void SetRestsParams()
        {
            lbChartTitle.Text = "Остаток средств бюджета";
            UserParams.CubeName.Value = "[ФО_Исполнение кассового плана";
            grid.DataBinding += new EventHandler(grid_DataBindingRests);
            grid.InitializeLayout += new InitializeLayoutEventHandler(grid_InitializeLayoutRests);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExportRests);
            grid.InitializeRow += new InitializeRowEventHandler(grid_InitializeRowRests);
            ultraChart.DataBinding += new EventHandler(ultraChart_DataBindingRests);
            ultraChart.InterpolateValues += new InterpolateValuesEventHandler(ultraChart_InterpolateValues);
            ultraChart.LineChart.NullHandling = NullHandling.InterpolateCustom;
            ultraChart.FillSceneGraph += new FillSceneGraphEventHandler(ultraChart_FillSceneGraphRests);
        }

        private void grid_InitializeRowOutcomes(object sender, RowEventArgs e)
        {
            TrimMonthCell(e.Row.Cells[0]);
            TrimMonthCell(e.Row.Cells[e.Row.Cells.Count - 1]);
            HighlightNegativeCells(e.Row);
        }

        private void grid_InitializeRowIncomes(object sender, RowEventArgs e)
        {
            TrimMonthCell(e.Row.Cells[0]);
            HighlightNegativeCells(e.Row);
        }

        private static void HighlightNegativeCells(UltraGridRow row)
        {
            for (int i = 0; i < row.Cells.Count; i++)
            {
                UltraGridCell cell = row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    double cellValue;
                    if (Double.TryParse(cell.Value.ToString(), out cellValue))
                    {
                        if (cellValue < 0)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        private static void TrimMonthCell(UltraGridCell cell)
        {
            TrimMonthCell(cell, 0);
        }

        private static void TrimMonthCell(UltraGridCell cell, int index)
        {
            int value;
            if (!Int32.TryParse(cell.Value.ToString(), out value))
            {
                cell.Value = GetMonth(cell.Value.ToString());
                cell.ColSpan = cell.Row.Cells.Count - 1 - index > 0 ? cell.Row.Cells.Count - 1 - index : 1;
                cell.Style.Font.Bold = true;
                cell.Row.Style.BorderDetails.ColorTop = Color.FromArgb(192, 192, 192);
            }
        }


        private void grid_InitializeRowRests(object sender, RowEventArgs e)
        {
            int value;
            if (!Int32.TryParse(e.Row.Cells[0].Value.ToString(), out value))
            {
                e.Row.Cells[0].Value = GetMonth(e.Row.Cells[0].Value.ToString());
                e.Row.Cells[0].ColSpan = e.Row.Cells.Count;
                e.Row.Cells[0].Style.Font.Bold = true;
                e.Row.Style.BorderDetails.ColorTop = Color.FromArgb(192, 192, 192);
            }
            else
            {
                // рисуем стрелку
                UltraGridCell cell = e.Row.Cells[3];
                double curVal;
                if (cell.Value != null && double.TryParse(cell.Value.ToString(), out curVal))
                {
                    if (curVal > 0)
                    {
                        cell.Style.CssClass = "ArrowUpGreen";
                        cell.Title = String.Format("Остаток увеличился");
                    }
                    else if (curVal < 0)
                    {
                        cell.Style.CssClass = "ArrowDownRed";
                        cell.Title = String.Format("Остаток уменьшился");
                    }
                }
            }
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    double cellValue;
                    if (Double.TryParse(cell.Value.ToString(), out cellValue))
                    {
                        if (cellValue < 0)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        private void SeriesToUpperFirstSymbol()
        {
            foreach (DataColumn col in dtChart.Columns)
            {
                col.ColumnName = CRHelper.ToUpperFirstSymbol(col.ColumnName);
            }
        }

        private void ultraChart_DataBindingOutcomes(object sender, EventArgs e)
        {
            BindChartDataOutcomes("ChartOutcome");
        }

        private void BindChartDataOutcomes(string queryId)
        {
            SetStackChartAppearance();
            ultraChart.Legend.SpanPercentage = 12;
            ultraChart.Legend.Margins.Right = (int)ultraChart.Width.Value / 3;
            BindChartData(queryId, 6);
        }

        private void ultraChart_DataBindingOutcomesTotal(object sender, EventArgs e)
        {
            BindChartDataOutcomes("ChartOutcomeTotal");
        }

        private void grid_DataBindingOutcomesTotal(object sender, EventArgs e)
        {
            dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("GridOutcomeTotal");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "День", dtGrid);
            // Удалим пустые группы
            RemoveEmptyGroups();
            grid.DataSource = dtGrid;
        }

        private void BindChartData(string queryId)
        {
            BindChartData(queryId, 2);
        }

        private void BindChartData(string queryId, int monthColumnIndex)
        {
            dtChart = new DataTable();
            string query = DataProvider.GetQueryText(queryId);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "name", dtChart);
            RemoveRedudantRows(dtChart, monthColumnIndex);
            SplitColumnNames();
            SeriesToUpperFirstSymbol();
            dtChart.AcceptChanges();
            ultraChart.DataSource = dtChart;
            ultraChart.Data.SwapRowsAndColumns = false;
        }

        private void ultraChart_DataBindingBudget(object sender, EventArgs e)
        {
            SetStackChartAppearance();
            BindChartData("ChartBudgetIncomes", 3);
        }

        private void ultraChart_DataBindingOutcomesAll(object sender, EventArgs e)
        {
            SetStackChartAppearanceUnic();
            BindChartData("ChartOutcomesAll", 5);
        }

        private void ultraChart_DataBindingIncomesAll(object sender, EventArgs e)
        {
            SetStackChartAppearanceUnic();
            BindChartData("ChartIncomesAll", 5);
        }

        private void ultraChart_DataBindingIncomes(object sender, EventArgs e)
        {
            SetColumnChartAppearance();
            BindChartData("ChartIncomes");
        }

        private void ultraChart_DataBindingRests(object sender, EventArgs e)
        {
            SetLineChartAppearance();
            dtChart = new DataTable();
            string query = DataProvider.GetQueryText("ChartRests");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "name", dtChart);
            DataTable dtCopy = dtChart.Copy();
            DataColumn monthColumn = dtChart.Columns[2];
            dtChart.Columns.RemoveAt(2);
            DataColumn col = new DataColumn("Остаток на начало квартала", typeof(double));
            dtChart.Columns.Add(col);
            dtChart.Columns.Add(monthColumn);
            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                dtChart.Rows[i][3] = dtCopy.Rows[i][2];
            }
            RemoveRedudantRows(dtChart, 3);
            SplitColumnNames();
            SeriesToUpperFirstSymbol();
            double valueRest = 0;
            int rowNum = 0;
            while (valueRest == 0 && rowNum < dtChart.Rows.Count)
            {
                if (double.TryParse(dtChart.Rows[rowNum][1].ToString(), out valueRest))
                {
                    dtChart.Rows[0][2] = valueRest;
                    dtChart.Rows[1][2] = valueRest;
                }
                rowNum++;
            }
            ultraChart.DataSource = dtChart;
            ultraChart.Data.SwapRowsAndColumns = true;
        }


        private void grid_DataBindingIncomes(object sender, EventArgs e)
        {
            dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("GridIncomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "День", dtGrid);
            grid.DataSource = dtGrid;
        }

        private void grid_DataBindingDeficite(object sender, EventArgs e)
        {
            dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("GridDeficite");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "День", dtGrid);
            grid.DataSource = dtGrid;
        }

        private void grid_DataBindingRests(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("GridRests");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "День", dtGrid);

            double valueRest = 0;
            int rowNum = 0;

            while (valueRest == 0 && rowNum < dtGrid.Rows.Count)
            {
                if (double.TryParse(dtGrid.Rows[rowNum][2].ToString(), out valueRest))
                {
                    double value;
                    if (double.TryParse(dtGrid.Rows[rowNum][3].ToString(), out value))
                    {
                        dtGrid.Rows[rowNum][3] = value - valueRest;
                    }
                }
                rowNum++;
            }

            grid.DataSource = dtGrid;
        }

        private void grid_DataBindingOutcomes(object sender, EventArgs e)
        {
            dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("GridOutcome");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "День", dtGrid);
            // Удалим пустые группы
            RemoveEmptyGroups();
            grid.DataSource = dtGrid;
        }

        private void RemoveEmptyGroups()
        {
            List<int> removingColumnIndexes = new List<int>();
            for (int groupCount = 0; groupCount < dtGrid.Columns.Count - 8; groupCount += 4)
            {
                if (EmptyGroup(groupCount))
                {
                    AddRemovingGroup(groupCount, removingColumnIndexes);
                }
            }
            for (int i = 0; i < removingColumnIndexes.Count; i++)
            {
                dtGrid.Columns.RemoveAt(removingColumnIndexes[i]);
            }
        }

        private static void AddRemovingGroup(int groupCount, List<int> removingColumnIndexes)
        {
            removingColumnIndexes.Add(groupCount + 1 - removingColumnIndexes.Count);
            removingColumnIndexes.Add(groupCount + 2 - removingColumnIndexes.Count);
            removingColumnIndexes.Add(groupCount + 3 - removingColumnIndexes.Count);
            removingColumnIndexes.Add(groupCount + 4 - removingColumnIndexes.Count);
        }

        private bool EmptyGroup(int groupCount)
        {
            return EmptyCol(dtGrid, groupCount + 1) &&
                   EmptyCol(dtGrid, groupCount + 2) &&
                   EmptyCol(dtGrid, groupCount + 3) &&
                   EmptyCol(dtGrid, groupCount + 4);
        }

        private bool EmptyCol(DataTable dt, int columnIndex)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (row[columnIndex] != DBNull.Value)
                {
                    return false;
                }
            }
            return true;
        }


        private void grid_InitializeLayoutRests(object sender, LayoutEventArgs e)
        {
            SetUpGrid(e);
        }

        private void grid_InitializeLayoutIncomes(object sender, LayoutEventArgs e)
        {
            SetUpGrid(e);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P2");
        }

        private static void SetUpGrid(LayoutEventArgs e)
        {
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            e.Layout.Bands[0].Columns[1].CellStyle.BorderDetails.ColorLeft =
                Color.FromArgb(192, 192, 192);

            e.Layout.Bands[0].Columns[0].Width = 50;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
            }
        }

        private void grid_InitializeLayoutOutcomes(object sender, LayoutEventArgs e)
        {
            ReplaceColumns(e.Layout.Bands[0]);

            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(45, 1900);

            AddDateColumns(e.Layout.Bands[0]);
            SetupHeaders(e.Layout.Bands[0]);
        }

        private static void AddDateColumns(UltraGridBand band)
        {
            band.Columns[0].Header.Caption = "День";

            UltraGridColumn col = new UltraGridColumn();
            col = col.CopyFrom(band.Columns[0]);
            band.Columns.Insert(band.Columns.Count, col);
            band.Columns[band.Columns.Count - 1].Width = CRHelper.GetColumnWidth(45, 1900);
            band.Columns[band.Columns.Count - 1].CellStyle.BorderDetails.ColorLeft =
                Color.FromArgb(192, 192, 192);
        }

        private static void ReplaceColumns(UltraGridBand band)
        {
            UltraGridColumn col = band.Columns[band.Columns.Count - 4];
            band.Columns.RemoveAt(band.Columns.Count - 4);
            band.Columns.Insert(1, col);

            col = band.Columns[band.Columns.Count - 3];
            band.Columns.RemoveAt(band.Columns.Count - 3);
            band.Columns.Insert(2, col);

            col = band.Columns[band.Columns.Count - 2];
            band.Columns.RemoveAt(band.Columns.Count - 2);
            band.Columns.Insert(3, col);

            col = band.Columns[band.Columns.Count - 1];
            band.Columns.RemoveAt(band.Columns.Count - 1);
            band.Columns.Insert(4, col);
        }

        private static void SetupHeaders(UltraGridBand band)
        {
            band.Columns[0].Header.RowLayoutColumnInfo.OriginY = 1;
            for (int i = 1; i < band.Columns.Count; i++)
            {
                band.Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                band.Columns[i].Width = 75;
            }

            CRHelper.AddHierarchyHeader(band.Grid, 0, "День", 0, 0, 1, 2);
            CRHelper.AddHierarchyHeader(band.Grid, 0, "День", band.Columns.Count - 1, 0, 1, 2);

            int multiHeaderPos = 1;
            for (int i = 1; i < band.Columns.Count - 2; i = i + 4)
            {
                band.Columns[i].CellStyle.BorderDetails.ColorLeft =
                        Color.FromArgb(192, 192, 192);

                string[] captions = band.Columns[i].Header.Caption.Split(';');
                CRHelper.SetHeaderCaption(band.Grid, 0, i, captions[1], captions[1]);
                CRHelper.FormatNumberColumn(band.Columns[i], "N0");
                band.Columns[i].Header.Title = "Квартальные назначения";

                captions = band.Columns[i + 1].Header.Caption.Split(';');
                CRHelper.SetHeaderCaption(band.Grid, 0, i + 1, captions[1], captions[1]);
                CRHelper.FormatNumberColumn(band.Columns[i + 1], "N0");
                band.Columns[i + 1].Header.Title = "Исполнено с начала квартала";

                captions = band.Columns[i + 2].Header.Caption.Split(';');
                CRHelper.SetHeaderCaption(band.Grid, 0, i + 2, captions[1], captions[1]);
                CRHelper.FormatNumberColumn(band.Columns[i + 2], "P2");

                captions = band.Columns[i + 3].Header.Caption.Split(';');
                CRHelper.SetHeaderCaption(band.Grid, 0, i + 3, captions[1], captions[1]);
                CRHelper.FormatNumberColumn(band.Columns[i + 3], "N0");
                band.Columns[i + 3].Header.Title = "Изменение суммы остатка за день";

                CRHelper.AddHierarchyHeader(band.Grid, 0, captions[0] + ", тыс.руб.", multiHeaderPos, 0, 4, 1);

                string assessionText = string.Empty;
                string[] headerKey = band.Columns[i].Header.Key.Split(';');
                switch (headerKey[0])
                {
                    // Итого
                    case "Итого":
                        {
                            band.Columns[i + 2].Header.Title = "Процент исполнения назначений";
                            break;
                        }
                    // Заработная плата
                    case "Заработная плата":
                        {
                            assessionText = "выплаты до 6 и 21 числа каждого месяца не менее 1/6 квартального плана";
                            break;
                        }
                    // Публичные обязательства
                    // Коммунальные расходы
                    // Межбюджетные трансферты
                    case "Нормативные публичные обязательства":
                    case "Коммунальные расходы":
                    case "Межбюджетные трансферты":
                        {
                            assessionText = "на конец месяца должно быть исполнено 1/3 квартального плана";
                            break;
                        }
                    // Прочие
                    case "Прочие расходы":
                        {
                            assessionText = "равномерное расходование по числу дней квартала";
                            break;
                        }
                }
                if (i != 1)
                {
                    band.Columns[i + 2].Header.Title =
                        String.Format("Процент исполнения назначений и оценка равномерности расходования \r({0})",
                                      assessionText);
                }
                multiHeaderPos += 4;
            }
        }

        private void SetStackChartAppearance()
        {
            ultraChart.ChartType = ChartType.StackColumnChart;
            SetChartAppearance();
            ultraChart.Legend.SpanPercentage = 7;
            ultraChart.Legend.Margins.Right = (int)ultraChart.Width.Value * 2 / 3;
        }

        private void SetChartAppearance()
        {
            ultraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            ultraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            ultraChart.ColorModel.ModelStyle = ColorModels.PureRandom;
            ultraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            ultraChart.Axis.X.Labels.Visible = false;
            ultraChart.Tooltips.FormatString = "<ITEM_LABEL> <br />  <SERIES_LABEL> <br /> <DATA_VALUE:N0> тыс.руб.";
            SetLabelsClipTextBehavior(ultraChart.Axis.X.Labels.SeriesLabels.Layout);
            ultraChart.Axis.Y.Extent = 40;
        }

        private void SetStackChartAppearanceUnic()
        {
            SetStackChartAppearance();
            ultraChart.Legend.SpanPercentage = 17;
            ultraChart.Legend.Margins.Right = (int)ultraChart.Width.Value / 3;
        }

        private void SetColumnChartAppearance()
        {
            ultraChart.ChartType = ChartType.StackColumnChart;
            SetChartAppearance();
            ultraChart.Legend.Margins.Right = (int)ultraChart.Width.Value / 4;
            ultraChart.Legend.SpanPercentage = 7;
        }

        private void SetLineChartAppearance()
        {
            ultraChart.ChartType = ChartType.LineChart;
            ultraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            ultraChart.Axis.Y.Extent = 45;
            ultraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            ultraChart.LineChart.NullHandling = NullHandling.InterpolateCustom;
            EmptyAppearance item = new EmptyAppearance();
            item.EnableLineStyle = true;
            LineStyle style = new LineStyle();
            style.DrawStyle = LineDrawStyle.Dot;
            item.LineStyle = style;
            ultraChart.LineChart.EmptyStyles.Add(item);
            ultraChart.ColorModel.ModelStyle = ColorModels.CustomLinear;
            ultraChart.Legend.SpanPercentage = 7;
            ultraChart.Legend.Margins.Right = (int)ultraChart.Width.Value / 2;
            ultraChart.Tooltips.FormatString = "<ITEM_LABEL> <br />  <SERIES_LABEL> <br /> <DATA_VALUE:N0> тыс.руб.";
            ultraChart.Axis.X.Labels.SeriesLabels.FormatString = "<SERIES_LABEL>";
            ultraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            ultraChart.Axis.X.Labels.Visible = true;
        }

        private void SetLabelsClipTextBehavior(AxisLabelLayoutAppearance layout)
        {
            layout.Behavior = AxisLabelLayoutBehaviors.UseCollection;
            layout.BehaviorCollection.Clear();
            ClipTextAxisLabelLayoutBehavior behavior = new ClipTextAxisLabelLayoutBehavior();
            behavior.ClipText = false;
            behavior.Enabled = true;
            behavior.Trimming = StringTrimming.None;
            behavior.UseOnlyToPreventCollisions = false;
            layout.BehaviorCollection.Add(behavior);
        }

        private void SplitColumnNames()
        {
            foreach (DataColumn col in dtChart.Columns)
            {
                string[] names = col.ColumnName.Split(';');
                if (names.Length == 2)
                {
                    col.ColumnName = names[0];
                }
            }
        }

        private void AdditionalSetupGrid()
        {
            int month = lastDataDate.Month;
            for (int rowNum = 0; rowNum < grid.Rows.Count; rowNum++)
            {
                // Проверим первую колонку. Если там месяц, запомним его.
                UltraGridCell cell = grid.Rows[rowNum].Cells[0];
                int value = 1;
                if (cell != null && cell.Value != null)
                {
                    if (!Int32.TryParse(cell.ToString(), out value))
                    {
                        month = CRHelper.MonthNum(cell.ToString());
                        value = 1;
                    }
                }
                DateTime date = GetDate(lastDataDate.Year, month, value);
                
                

                for (int colNum = 7; colNum < grid.Columns.Count; colNum += 4)
                {
                    string[] headerKey = grid.Columns[colNum].Header.Key.Split(';');
                    cell = grid.Rows[rowNum].Cells[colNum];
                    if (cell == null || cell.Value == null) continue;
                    try
                    {
                        double ethalon = 0;
                        switch (headerKey[0])
                        {
                            case "Заработная плата":
                                {
                                    ethalon = SalaryAssessionLimit(date);
                                    break;
                                }
                            case "Нормативные публичные обязательства":
                            case "Коммунальные расходы":
                            case "Межбюджетные трансферты":
                                {
                                    ethalon = CommonAssessionLimit(date);
                                    break;
                                }
                            case "Прочие расходы":
                                {
                                    ethalon = CRHelper.QuarterDaysCountToDate(date) / CRHelper.QuarterDaysCount(date);
                                    break;
                                }
                        }

                        double curVal;
                        curVal = double.Parse(cell.Value.ToString());
                        if (curVal >= ethalon)
                        {
                            cell.Style.CssClass = "BallGreen";
                            cell.Title = String.Format("Соблюдается условие равномерности ({0:P2})", ethalon);
                        }
                        else
                        {
                            cell.Style.CssClass = "BallRed";
                            cell.Title = String.Format("Не соблюдается условие равномерности ({0:P2})", ethalon);
                        }
                    }
                    catch
                    {
                        // не удалось покрасить, ну да пойдем дальше
                    }
                }
            }
        }

        private DateTime GetDate(int year, int month, int day)
        {
            DateTime date;
            try
            {
                date = new DateTime(year, month, day);
            }
            catch (ArgumentOutOfRangeException)
            {
                date = GetDate(year, month, day - 1);
            }
            return date;
        }

        /// <summary>
        /// Пороговое значение оценки зарплаты.
        /// </summary>
        /// <returns></returns>
        private double SalaryAssessionLimit(DateTime date)
        {
            // Берем номер месяца в квартале
            double monthNum = CRHelper.MonthNumInQuarter(date.Month);
            int day = date.Day;
            if (day < 6)
            {
                // Выплат в этом месяце не было
                return (monthNum - 1) / 3;
            }
            if (day < 21)
            {
                // Была одна выплата
                return (monthNum - 1) / 3 + 1.0 / 6;
            }
            // Все выплаты
            return (monthNum) / 3;
        }

        /// <summary>
        /// Пороговое значение оценки остальных.
        /// </summary>
        /// <returns></returns>
        private double CommonAssessionLimit(DateTime date)
        {
            // Берем номер месяца в квартале
            double monthNum = CRHelper.MonthNumInQuarter(date.Month);
            // Если последний день месяца
            if (CRHelper.MonthLastDay(date.Month) ==
                date.Day)
            {
                return (monthNum) / 3;
            }
            else
            {
                return (monthNum - 1) / 3;
            }
        }

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = lbTitle.Text + " " + lbSubTitle.Text;
        }

        private void ExcelExporter_EndExportRests(object sender, EndExportEventArgs e)
        {
            for (int i = 1; i < grid.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "#,##0";
                int widthColumn = 80;
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
            }
        }

        private void ExcelExporter_EndExportOutcomes(object sender, EndExportEventArgs e)
        {
            for (int i = 1; i < grid.Bands[0].Columns.Count - 2; i = i + 4)
            {
                string formatString = "#,##0";
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i + 1].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i + 2].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
                e.CurrentWorksheet.Columns[i + 3].CellFormat.FormatString = formatString;
            }
        }

        private void ExcelExporter_EndExportIncomes(object sender, EndExportEventArgs e)
        {
            for (int i = 1; i < grid.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "#,##0";
                int widthColumn = 80;
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
            }
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = grid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(grid);
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(grid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(lbTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(lbSubTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(lbChartTitle.Text.Replace("&#8212", "-"));

            ultraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.82);
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(ultraChart);
            e.Section.AddImage(img);
        }
    }
}
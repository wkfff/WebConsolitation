using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.reports.SGM;
using Krista.FM.Server.Dashboards.SgmSupport; 

namespace Krista.FM.Server.Dashboards.reports.sgm.sgm_0010
{
    public partial class sgm_0010 : CustomReportPage
    {
        private DataTable dtGrid;
        private DataTable dtChart;
        private double fmMedian;
        private double sgmMedian;
        private int maxMonthNum;
        private DataTable dtSGMData, dtFMData;

        private string year, months, dies;

        private readonly SGMDataRotator dataRotator = new SGMDataRotator();
        private readonly SGMDataObject dataObject = new SGMDataObject();
        private readonly SGMSupport supportClass = new SGMSupport();
        private readonly SGMPdfExportHelper exportClass = new SGMPdfExportHelper();

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 70);
            UltraWebGrid.Height = Unit.Empty;
            UltraWebGrid.DataBound += UltraWebGrid_DataBound;

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 70);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6 - 50);

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ScatterChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.Axis.X.Extent = 30;
            UltraChart.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.Extent = 40;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.Text = "Бюджетные расходы на душу населения, \n млн.руб. на 100 тыс.чел.";
            UltraChart.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Extent = 50;

            UltraChart.TitleBottom.Visible = true;
            UltraChart.TitleBottom.Text = "Заболеваемость на 100 тыс.чел.";
            UltraChart.TitleBottom.Font = new Font("Verdana", 10);
            UltraChart.TitleBottom.HorizontalAlign = StringAlignment.Center;

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.SpanPercentage = 8;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.DataAssociation = ChartTypeData.ScatterData;
            UltraChart.Legend.Margins.Top = 0;
            UltraChart.Legend.Margins.Right = Convert.ToInt32(UltraChart.Width.Value * 0.5);

            UltraChart.ScatterChart.Icon = SymbolIcon.Square;
            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE_X:N3> заболеваний на 100 тыс.чел.\n<DATA_VALUE_Y:N3> млн.руб. на 100 тыс.чел.";
            UltraChart.InvalidDataReceived += CRHelper.UltraChartInvalidDataReceived;
            UltraChart.FillSceneGraph += UltraChart_FillSceneGraph;

            #endregion
            SetExportHandlers();
        }

        private void SetFKRState()
        {
            ComboFKR.SetСheckedState("ЗДРАВООХРАНЕНИЕ, ФИЗИЧЕСКАЯ КУЛЬТУРА И СПОРТ", true);
            ComboFKR.SetСheckedState("Стационарная медицинская помощь", true);
            ComboFKR.SetСheckedState("Амбулаторная помощь", true);
            ComboFKR.SetСheckedState("Медицинская помощь в дневных стационарах всех типов", true);
            ComboFKR.SetСheckedState("Скорая медицинская помощь", true);
            ComboFKR.SetСheckedState("Санаторно-оздоровительная помощь", true);
            ComboFKR.SetСheckedState("Заготовка, переработка, хранение и обеспечение безопасности донорской крови и ее компонентов", true);
            ComboFKR.SetСheckedState("Санитарно-эпидемиологическое благополучие", true);
        }

        protected string FormDeseaseFilter()
        {
            string result = String.Empty;
            foreach (string key in ComboDesease.SelectedValues)
            {

                result = String.Format("{0},{1}", result, dataRotator.deseasesRelation[key]);
            }
            return result.Trim(',');
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            dataObject.reportFormRotator = dataRotator;
            dataRotator.CheckSubjectReport();
            base.Page_PreLoad(sender, e);
            dataRotator.formNumber = 1;
            dataObject.InitObject();
            dataRotator.FillRegionsDictionary(dataObject.dtAreaShort);            
            if (!Page.IsPostBack)
            {
                dataRotator.FillYearList(ComboYear);
                dataRotator.FillMonthListEx(ComboMonth, ComboYear.SelectedValue);
                ComboMonth.SetСheckedState("январь", true);
                dataRotator.FillSGMMapList(ComboFO, dataObject.dtAreaShort, false);
                dataRotator.FillDeseasesList(ComboDesease, 0);
                ComboFKR.FillDictionaryValues(CustomMultiComboDataHelper.FillFKRNames(
                        DataDictionariesHelper.OutcomesFKRTypes,
                        DataDictionariesHelper.OutcomesFKRLevels));
                SetFKRState();
                UserParams.Filter.Value = ComboFO.SelectedValue;
                UserParams.KDGroup.Value = DataDictionariesHelper.OutcomesFKRTypes[ComboFKR.SelectedValue];
                UserParams.Subject.Value = String.Empty;
            }
            else
            {
                dataRotator.FillSGMMapList(null, dataObject.dtAreaShort, false);
                dataRotator.FillDeseasesList(null, -1);
            }

            var dtDate = new DataTable();
            string query = DataProvider.GetQueryText("sgm_0010_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            maxMonthNum = 12;
            if (Convert.ToInt32(ComboYear.SelectedValue) == DateTime.Now.Year)
            {
                maxMonthNum = Math.Min(
                    dataRotator.GetLastMonth(ComboYear.SelectedValue),
                    CRHelper.MonthNum(dtDate.Rows[0][3].ToString()));
            }
            maxMonthNum = Math.Min(maxMonthNum, ComboMonth.SelectedIndex + 1);
            year = ComboYear.SelectedValue;
            months = dataRotator.GetMonthParamString(ComboMonth, year);
            months = string.Empty;
            for (int i = 1; i <= maxMonthNum; i++)
            {
                months = String.Format("{0},{1}", months, i);
            }
            months = months.TrimStart(',');
            dies = FormDeseaseFilter(); 

            if (ComboFKR.SelectedValues.Count == 0) SetFKRState();
            string fkrGroup = String.Empty;
            foreach (string group in ComboFKR.SelectedValues)
            {
                fkrGroup = String.Format("{0},{1}", fkrGroup, DataDictionariesHelper.OutcomesFKRTypes[group]);
            }
            fkrGroup = fkrGroup.TrimStart(',');
            UserParams.PeriodLastYear.Value = year;
            UserParams.PeriodYear.Value = Convert.ToString(Convert.ToInt32(year) - 1);
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(
                Convert.ToInt32(months.Split(',')[months.Split(',').Length - 1]));
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(maxMonthNum));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(maxMonthNum));
            UserParams.KDGroup.Value = fkrGroup;

            if (ComboDesease.SelectedValues.Count == 0)
                ComboDesease.SetСheckedState(dataRotator.deseasesNames[0], true);

            string monthCaption = "январь";
            if (maxMonthNum > 1) monthCaption = String.Format("{0}-{1}", monthCaption, CRHelper.RusMonth(maxMonthNum));

            Page.Title = string.Format("Соотношение объема расходов на здравоохранение и уровня заболеваемости ({0})", ComboFO.SelectedIndex == 0 ? "РФ" :
                supportClass.GetFOShortName(ComboFO.SelectedValue));
            Label1.Text = Page.Title;
            Label2.Text = String.Format("за {0} {1} г.",
                monthCaption, year);
            Label3.Text = String.Format("<b>Заболевания:</b> {0}", ComboDesease.SelectedValuesString);
            Label4.Text = String.Format("<b>Группы расходов:</b> {0}", ComboFKR.SelectedValuesString);

            UltraWebGrid.DataBind();
            UltraChart.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("sgm_0010_grid");
            dtFMData = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtFMData);
            foreach (DataRow drFM in dtFMData.Rows)
            {
                drFM[0] = supportClass.GetFOShortName(drFM[0].ToString().ToUpper());
                drFM[0] = drFM[0].ToString().Split('(')[0];
                if (drFM[3] != DBNull.Value) drFM[3] = Convert.ToDouble(drFM[3]) / 1000000;
                if (drFM[4] != DBNull.Value) drFM[4] = Convert.ToDouble(drFM[4]) / 1000000;
                if (drFM[7] != DBNull.Value) drFM[7] = Convert.ToDouble(drFM[7]) / 1000000;
            }
            dataObject.ClearParams();
            dataObject.InitObject();
            dataObject.useLongNames = true;
            dataObject.mainColumn = SGMDataObject.MainColumnType.mctMapName;
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctFO, "0");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year, months, String.Empty, "0", dies);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation, "2");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRank, "3");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                Convert.ToString(Convert.ToInt32(year) - 1), months, String.Empty, "0", dies);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation, "5");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRank, "6");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctPopulation, "5");
            dtSGMData = dataObject.FillData();

            foreach (DataRow drSGM in dtSGMData.Rows)
            {
                drSGM[0] = drSGM[0].ToString().Split('(')[0];
            }

            dtGrid = MergeTables(dtSGMData, dtFMData);
            FillChartData();

            UltraWebGrid.DataSource = dtGrid;
        }

        private void FillChartData()
        {
            dtChart = new DataTable();
            dtChart.Columns.Add("Субъект", typeof(string));
            dtChart.Columns.Add("Заболеваемость", typeof(double));
            dtChart.Columns.Add("Расходы", typeof(double));
            dtChart.Columns.Add("Расходы ФО", typeof(double));
            dtChart.Columns.Add("Расходы РФ", typeof(double));

            foreach (DataRow dr in dtGrid.Rows)
            {
                DataRow chartRow = dtChart.Rows.Add();
                chartRow[0] = dr[0];
                chartRow[1] = dr[4];
                int columnIndex = 2;
                if (dr[1].ToString().Length == 0)
                {
                    columnIndex = 3;
                }

                if (dr[0].ToString() == dataObject.GetRootMapName())
                {
                    columnIndex = 4;
                }

                chartRow[columnIndex] = dr[9];
            }            
        }

        private void SetColumnParams(UltraGridLayout layout, int columnIndex, string format, int width)
        {
            UltraGridColumn col = layout.Bands[0].Columns[columnIndex];
            CRHelper.FormatNumberColumn(col, format);
            col.Width = CRHelper.GetColumnWidth(width);
        }

        private bool CheckRow(DataRow dr)
        {
            bool singleFO = ComboFO.SelectedIndex > 0;
            if (!singleFO) return true;
            string foName = supportClass.GetFOShortName(ComboFO.SelectedValue);
            string currentName = dr[0].ToString();
            string currentFO = dr[1].ToString();
            return currentName == dataObject.GetRootMapName() || currentName == foName || currentFO == foName;
        }

        private DataTable MergeTables(DataTable dtSGM, DataTable dtFM)
        {
            var dtResult = new DataTable();
            dtResult.Columns.Add("ColumnSubjectName", typeof(string));
            dtResult.Columns.Add("ColumnFOName", typeof(string));
            
            for (int i = 0; i < 9; i++)
            {
                dtResult.Columns.Add(String.Format("DataColumn{0}", i), typeof(double));
            }

            foreach (DataRow rowSGM in dtSGM.Rows)
            {
                if (CheckRow(rowSGM))
                {
                    DataRow drResult = dtResult.Rows.Add();
                    // субъект
                    drResult[00] = rowSGM[0];
                    // фо
                    drResult[01] = rowSGM[1];
                    // абс заболеваемость
                    drResult[02] = rowSGM[2];
                    // население
                    drResult[03] = rowSGM[8];
                    // отн заболеваемость
                    drResult[04] = rowSGM[3];
                    // ранк
                    drResult[05] = rowSGM[4];
                    // ищем строчку субъекта\фо в таблице данных ФМ
                    DataRow drFind = supportClass.FindDataRowEx(dtFM, rowSGM[0].ToString(), dtFM.Columns[0].ColumnName);
                    if (drFind != null)
                    {
                        drResult[06] = drFind[3];
                        drResult[07] = drFind[4];
                        drResult[08] = drFind[6];
                        if (drFind[4] != DBNull.Value && rowSGM[8] != DBNull.Value)
                        {
                            drResult[09] = 100000 * Convert.ToDouble(drFind[4]) / Convert.ToDouble(rowSGM[8]);
                        }
                    }
                }
            }

            // Простановка рангов по среднедушевым доходам
            DataRow[] drsSelectSgm = dtResult.Select(
                string.Format("{0} <> '' and {1} <> 0", dtResult.Columns[1].ColumnName, dtResult.Columns[4].ColumnName),
                string.Format("{0} asc", dtResult.Columns[4].ColumnName));
            if (drsSelectSgm.Length > 0)
                sgmMedian = Convert.ToDouble(drsSelectSgm[drsSelectSgm.Length / 2][4]);

            // Простановка рангов по среднедушевым доходам
            DataRow[] drsSelect = dtResult.Select(
                string.Format("{0} <> '' and {1} <> 0", dtResult.Columns[1].ColumnName, dtResult.Columns[9].ColumnName),
                string.Format("{0} asc", dtResult.Columns[9].ColumnName));
            if (drsSelect.Length > 0)
                fmMedian = Convert.ToDouble(drsSelect[drsSelect.Length / 2][9]);
            
            foreach (DataRow drResult in dtResult.Rows)
            {
                if (drResult[1].ToString() != string.Empty)
                {
                    int rank = -1;
                    for (int i = 0; i < drsSelect.Length; i++)
                    {
                        if (drsSelect[i][0].ToString() == drResult[0].ToString()) rank = i + 1;
                    }
                    if (rank != -1) drResult[10] = rank;
                }
            }

            dataRotator.FillMaxAndMin(dtResult, 2, true);

            return dtResult;
        }

        private void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (ComboFO.SelectedIndex != 0 || dtGrid.Rows.Count < 15)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 00, "Территория", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 01, "ФО", "Федеральный округ, которому принадлежит субъект");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 02, "Абсолютное число заболеваний", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 03, "Численность постоянного населения", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 04, "Заболеваемость на 100 тыс.чел. населения", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 05, "Ранг заболев. РФ", "Ранг (место) по заболеваниям на 100 тыс.чел. среди всех субъектов");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 06, "Запланированная сумма расходов, млн.руб.", "План на год");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 07, "Фактическая сумма расходов, млн.руб.", "Фактическое исполнение нарастающим итогом с начала года");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 08, "Процент исполнения по расходам", "Процент выполнения назначений. Оценка равномерности исполнения (1/12 годового плана в месяц)");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 09, "Бюджетные расходы на душу населения, млн.руб./100 тыс.чел.", "Бюджетные расходы на душу населения, млн.руб. на 100 тыс.чел. населения");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 10, "Ранг бюдж. обеспеч.", "Ранг (место) по бюджетным расходам на душу населения");

            SetColumnParams(e.Layout, 01, "", 45);
            SetColumnParams(e.Layout, 02, "N0", 85);
            SetColumnParams(e.Layout, 03, "N0", 85);
            SetColumnParams(e.Layout, 04, "N3", 100);
            SetColumnParams(e.Layout, 05, "N0", 57);
            SetColumnParams(e.Layout, 06, "N3", 110);
            SetColumnParams(e.Layout, 07, "N3", 110);
            SetColumnParams(e.Layout, 08, "P2", 95);
            SetColumnParams(e.Layout, 09, "N3", 136);
            SetColumnParams(e.Layout, 10, "N0", 70);

        }

        protected string GetSGMDeseaseToolTip(DataRow dr)
        {
            int absValue1 = Convert.ToInt32(dr[2]);
            int absValue2 = Convert.ToInt32(dr[5]);
            double relValue1 = Convert.ToDouble(dr[3]);
            double relValue2 = Convert.ToDouble(dr[6]);
            bool isHigh = (relValue1 > relValue2) || (relValue1 == relValue2 && absValue1 > absValue2);

            string caption = "Падение";
            if (isHigh) caption = "Рост";

            return String.Format("{0} к прошлому году; \nза аналогичный период \nпрошлого года: {1:N2} на 100 тыс.чел.; \nТемп роста: {2}",
                caption, relValue2, supportClass.GetDifferenceTextEx(absValue1, absValue2, relValue1, relValue2, false, false));
        }

        protected string GetFMDeseaseToolTip(DataRow dr)
        {
            double relValue1 = 0;
            if (dr[4] != DBNull.Value) relValue1 = Convert.ToDouble(dr[4]);
            double relValue2 = 0;
            if (dr[7] != DBNull.Value) relValue2 = Convert.ToDouble(dr[7]);
            double percent = 0;
            if (dr[5] != DBNull.Value) percent = 100 * Convert.ToDouble(dr[5]);
            bool isHigh = (relValue1 > relValue2);
            string caption = "Падение";
            if (isHigh) caption = "Рост";
            return String.Format("{0} к прошлому году; \nза аналогичный период \nпрошлого года: {1:N2} млн.руб.; \nТемп роста: {2:N2}%",
                caption, relValue2, percent);
        }


        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[1].ToString() == "Cell") e.Row.Style.Font.Bold = true;
            int imageCellIndex = 5;
            string caption = String.Empty;
            int cellImageType = supportClass.SetCellImageStar(e.Row, dataRotator.maxValues, dataRotator.minValues, imageCellIndex, imageCellIndex, imageCellIndex, true);
            if (cellImageType != 0)
            {
                if (cellImageType == 2) caption = "высокий";
                if (cellImageType == 1) caption = "низкий";
                e.Row.Cells[imageCellIndex].Title = string.Format("Самый {0} уровень заболеваемости на 100 тысяч человек", caption);
            }
            imageCellIndex = 10;
            cellImageType = supportClass.SetCellImageStar(e.Row, dataRotator.maxValues, dataRotator.minValues, imageCellIndex, imageCellIndex, imageCellIndex);
            caption = String.Empty;
            if (cellImageType != 0)
            {
                if (cellImageType == 1) caption = "высокий";
                if (cellImageType == 2) caption = "низкий";
                e.Row.Cells[imageCellIndex].Title = string.Format("Самый {0} уровень бюджетных расходов на душу населения", caption);
            }
            e.Row.Cells[4].Title = GetSGMDeseaseToolTip(dtSGMData.Rows[e.Row.Index]);
            DataRow drFind = supportClass.FindDataRowEx(
                dtFMData, e.Row.Cells[0].ToString(), dtFMData.Columns[0].ColumnName);
            if (drFind != null) e.Row.Cells[7].Title = GetFMDeseaseToolTip(drFind);

            UltraGridCell cell = e.Row.Cells[8];
            if (cell.ToString() != "Cell")
            {
                double percent1 = Convert.ToDouble(maxMonthNum) * 100 / 12;
                double percent2 = percent1;
                if (drFind != null) percent2 = 100 * Convert.ToDouble(drFind[6]);
                if (percent1 > percent2)
                {
                    cell.Style.BackgroundImage = "~/images/ballRedBB.png";
                    caption = "Не соблюдается";
                }
                else
                {
                    cell.Style.BackgroundImage = "~/images/ballGreenBB.png";
                    caption = "Соблюдается";
                }
                cell.Title = String.Format("{0} условие равномерности({1:N2}% из {2:N2}%)", caption, percent2, percent1);
                cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
            }
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            XYSeries series1 = CRHelper.GetXYSeries(1, 2, dtChart);
            series1.Label = "Субъекты";
            UltraChart.Series.Add(series1);

            XYSeries series2 = CRHelper.GetXYSeries(1, 3, dtChart);
            series2.Label = "Федеральные округа";
            UltraChart.Series.Add(series2);

            XYSeries series3 = CRHelper.GetXYSeries(1, 4, dtChart);
            series3.Label = "Российская Федерация";
            UltraChart.Series.Add(series3);
        }

        void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            var xAxis = (IAdvanceAxis)e.Grid["X"];
            var yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            var xMin = (int)xAxis.MapMinimum;
            var yMin = (int)yAxis.MapMinimum;
            var xMax = (int)xAxis.MapMaximum;
            var yMax = (int)yAxis.MapMaximum;

            var fmY = (int) yAxis.Map(fmMedian);
            var line = new Line
                           {
                               lineStyle = {DrawStyle = LineDrawStyle.Dot},
                               PE =
                                   {
                                       Stroke = Color.DarkGray,
                                       StrokeWidth = 2
                                   },
                               p1 = new Point(xMin, fmY),
                               p2 = new Point(xMax, fmY)
                           };

            e.SceneGraph.Add(line);

            var sgmX = (int) xAxis.Map(sgmMedian);
            line = new Line
                       {
                           lineStyle = {DrawStyle = LineDrawStyle.Dot},
                           PE =
                               {
                                   Stroke = Color.DarkGray,
                                   StrokeWidth = 2
                               },
                           p1 = new Point(sgmX, yMin),
                           p2 = new Point(sgmX, yMax)
                       };

            e.SceneGraph.Add(line);

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Symbol)
                {
                    var icon = primitive as Symbol;
                    if (icon.Path == "Legend")
                    {
                        Primitive prevPrimitive = e.SceneGraph[i - 1];
                        if (prevPrimitive is Text)
                        {
                            string legendText = ((Text)prevPrimitive).GetTextString();
                            icon.icon = GetIconType(legendText);
                            icon.iconSize = SymbolIconSize.Medium;
                        }
                    }
                    else if (icon.Series != null)
                    {
                        icon.icon = GetIconType(icon.Series.Label);
                    }
                }
            }
        }

        private SymbolIcon GetIconType(string seriesName)
        {
            SymbolIcon iconType;
            switch (seriesName)
            {
                case "Субъекты":
                    {
                        iconType = SymbolIcon.Circle;
                        break;
                    }
                case "Федеральные округа":
                    {
                        iconType = SymbolIcon.Square;
                        break;
                    }
                case "Российская Федерация":
                    {
                        iconType = SymbolIcon.Diamond;
                        break;
                    }
                default:
                    {
                        iconType = SymbolIcon.Random;
                        break;
                    }
            }
            return iconType;
        }

        #endregion

        #region PDFExport

        protected virtual void SetExportHandlers()
        {
            UltraGridExporter1.PdfExportButton.Click += PdfExportButton_Click;
            UltraGridExporter1.PdfExporter.BeginExport += PdfExporter_BeginExport;
            UltraGridExporter1.ExcelExportButton.Visible = false;
        }

        protected virtual void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            InitializeExportLayout(e);
            exportClass.ExportCaptionText(e, Label1.Text);
            exportClass.ExportSubCaptionText(e, Label2.Text);
            exportClass.ExportSubCaptionText(e, Label3.Text);
            exportClass.ExportSubCaptionText(e, Label4.Text);
            exportClass.ExportChart(e, UltraChart);
        }

        protected virtual void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = string.Format("0010.pdf");
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        protected virtual void InitializeExportLayout(Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            UltraChart.Width = (Unit)(UltraChart.Width.Value - 100);
        }

        #endregion
    }
}


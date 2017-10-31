using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using BoxAnnotation=Infragistics.UltraGauge.Resources.BoxAnnotation;
using Line=Infragistics.UltraChart.Core.Primitives.Line;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0010
{
    public partial class DefaultEvenness : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid1;
        private DataTable dtGrid2;
        private int firstYear = 2000;
        private int endYear = 2011;

        private double populTotal = 0;
        private double chargeTotal = 0;
//        private double chargeMin = 0;
//        private double chargeMax = 0;
        
        private double lorencKoeff = 0;
        private double jinyIndex = 0;

        private double avgMin = 0;
        private double avgMax = 0;
        private double avgStep = 0;

        private string populationDate;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraChart.Width = CRHelper.GetChartWidth(650);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight / 2 + 70);
            JinyIndexLabelContainer.Style.Add("width", CRHelper.GetChartWidth(652) + "px");

            UltraWebGrid.Width = CRHelper.GetGridWidth(575);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 2 - 10);

            UnitWeightGrid.Width = CRHelper.GetGridWidth(575);
            UnitWeightGrid.Height = CRHelper.GetGridHeight((CustomReportConst.minScreenHeight / 2 - 200) );
            UnitWeightGrid.DataBound += new EventHandler(UnitWeightGrid_DataBound);

            UltraGauge1.DeploymentScenario.FilePath = "../../TemporaryImages";
            UltraGauge1.DeploymentScenario.ImageURL = "../../TemporaryImages/gauge_fk01_10_#SEQNUM(100).png";

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ScatterChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.Axis.X.Extent = 30;
            UltraChart.Axis.Y.Extent = 50;

            UltraChart.Data.ZeroAligned = true;

            UltraChart.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:P0>";
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P0>";

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.Text = "Кумулятивный ряд доходов, %";
            UltraChart.TitleLeft.Font = new Font("Verdana", 12);
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;

            UltraChart.TitleBottom.Visible = true;
            UltraChart.TitleBottom.Text = "Кумулятивный ряд численности, %";
            UltraChart.TitleBottom.Font = new Font("Verdana", 12);
            UltraChart.TitleBottom.HorizontalAlign = StringAlignment.Center;

            UltraChart.ScatterChart.ConnectWithLines = true;
            UltraChart.ScatterChart.Icon = SymbolIcon.Square;
            UltraChart.ScatterChart.IconSize = SymbolIconSize.Small;
            UltraChart.ScatterChart.LineAppearance.SplineTension = (float)0.1;

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.HorizontalAlign = StringAlignment.Far;
            appearance.VerticalAlign = StringAlignment.Near;
            appearance.ItemFormatString = "<DATA_VALUE_X:P1>;<DATA_VALUE_Y:P1>";
            appearance.ChartTextFont = new Font("Verdana", 8);
            appearance.Visible = true;
            UltraChart.ScatterChart.ChartText.Add(appearance);

            UltraChart.Tooltips.FormatString = "Группа доходов: <SERIES_LABEL>\n<DATA_VALUE_X:P1>\n<DATA_VALUE_Y:P1>";
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #endregion
        }

        void UnitWeightGrid_DataBound(object sender, EventArgs e)
        {
            UnitWeightGrid.Height = Unit.Empty;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0010_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            endYear = Convert.ToInt32(dtDate.Rows[0][0]);

            if (!Page.IsPostBack)
            {
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboKD.Width = 420;
                ComboKD.Title = "Вид дохода";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillShortKDIncludingList());
                ComboKD.SetСheckedState("Доходы ВСЕГО ", true);
            }

            //int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            int monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            if (monthNum == 12)
            {
                monthNum = 1;
            }
            else
            {
                monthNum++;
            }

            Page.Title = string.Format("Оценка равномерности распределения доходов РФ");
            Label1.Text = Page.Title;
            string currentDate = (ComboYear.SelectedValue == endYear.ToString())
                                     ? string.Format(" по состоянию&nbsp;на&nbsp;1&nbsp;{0}",
                                                     CRHelper.RusMonthGenitive(monthNum))
                                     : string.Empty;
            Label2.Text = string.Format("Информация по среднедушевым доходам ({0})&nbsp;за&nbsp;{1}&nbsp;год{2}",
                              ComboKD.SelectedValue, ComboYear.SelectedValue, currentDate);

            int year = Convert.ToInt32(ComboYear.SelectedValue);

            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.KDGroup.Value = ComboKD.SelectedValue;

            UserParams.Filter.Value = (useTowns.Checked)
                                          ? "- {[Территории].[Сопоставимый].[Все территории].[Российская  Федерация].[Центральный федеральный округ].[г. Москва], " +
                                            "[Территории].[Сопоставимый].[Все территории].[Российская  Федерация].[Северо-Западный федеральный округ].[г. Санкт-Петербург]}"
                                          : " ";

            UserParams.BudgetLevelEnum.Value = (BudgetButtonList.SelectedIndex == 0)
                                                   ? " "
                                                   : ",[Уровни бюджета].[СКИФ].[Внебюдж.фонд]";

            populationDate = DataDictionariesHelper.GetFederalPopulationDate();

            UltraWebGrid.DataBind();
            UnitWeightGrid.DataBind();
            UltraChart.DataBind();
        }

        #region Методы расчета

        /// <summary>
        /// Получение номера группы налога по доходам
        /// </summary>
        /// <param name="avg">величина налога</param>
        /// <returns>номер группы</returns>
        private int GetGroupNumber(double avg)
        {
            int groupNumber = 1;
            double limit = avgStep;

            while (avg >= limit && avg <= avgMax)
            {
                limit += avgStep;
                groupNumber++;
            }

            return groupNumber > groupCount ? groupCount : groupNumber;
        }

        /// <summary>
        /// Получение правой границы группы
        /// </summary>
        /// <param name="groupNumber">номер группы</param>
        /// <returns>граница</returns>
        private double GetGroupLimit(int groupNumber)
        {
            return groupNumber * avgStep;
        }

        private double GetGroupAverage(int groupNumber)
        {
            return (GetGroupLimit(groupNumber) + GetGroupLimit(groupNumber - 1)) / 2;
        }

        /// <summary>
        /// Получение наименования группы
        /// </summary>
        /// <param name="groupNumber">номер группы</param>
        /// <returns>наименование группы</returns>
        private string GetGroupName(int groupNumber)
        {
            string groupName = string.Empty;
            switch (groupNumber)
            {
                case 1:
                    {
                        groupName = string.Format("не более {0:N1}", GetGroupLimit(groupNumber));
                        break;
                    }
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    {
                        groupName = string.Format("от {0:N1} до {1:N1}", GetGroupLimit(groupNumber - 1), GetGroupLimit(groupNumber));
                        break;
                    }
                case 7:
                    {
                        groupName = string.Format("не менее {0:N1}", GetGroupLimit(groupNumber - 1));
                        break;
                    }
            }
            return groupName;
        }

        /// <summary>
        /// Получение таблицы удельных весов численности и доходов
        /// </summary>
        /// <returns>таблица</returns>
        private DataTable GetUnitWeightTable()
        {
            DataTable table = new DataTable();

            DataColumn column1 = new DataColumn("Группа", typeof(string));
            table.Columns.Add(column1);
            DataColumn column2 = new DataColumn("Удельный вес численности населения, %", typeof(double));
            table.Columns.Add(column2);
            DataColumn column3 = new DataColumn("Кумулятивный ряд численности, %", typeof(double));
            table.Columns.Add(column3);
            DataColumn column4 = new DataColumn("Середина интервала, тыс.руб.", typeof(double));
            table.Columns.Add(column4);
            DataColumn column5 = new DataColumn("Групповые доходы (процентные числа), тыс.руб.", typeof(double));
            table.Columns.Add(column5);
            DataColumn column6 = new DataColumn("Удельный вес групп в общем доходе, %", typeof(double));
            table.Columns.Add(column6);
            DataColumn column7 = new DataColumn("Кумулятивный ряд доходов, %", typeof(double));
            table.Columns.Add(column7);

            double[] populGroupTotals = new double[groupCount];
            double[] chargeGroupTotals = new double[groupCount];
            //double[] averages = new double[groupCount];

            for (int i = 1; i < dtGrid1.Rows.Count; i++)
            {
                int groupIndex = 0;
                if (dtGrid1.Rows[i][3] != DBNull.Value)
                {
                    groupIndex = GetGroupNumber(Convert.ToDouble(dtGrid1.Rows[i][3])) - 1;

                    chargeGroupTotals[groupIndex] += Convert.ToDouble(dtGrid1.Rows[i][3]) / chargeTotal;
                }
                if (dtGrid1.Rows[i][1] != DBNull.Value)
                {
                    populGroupTotals[groupIndex] += Convert.ToDouble(dtGrid1.Rows[i][1]) / populTotal;
                }
            }

            lorencKoeff = 0;
            jinyIndex = 0;
            double sum = 0;
            for (int i = 0; i < groupCount; i++)
            {
                DataRow row = table.NewRow();

                row[0] = GetGroupName(i + 1);
                row[1] = populGroupTotals[i];
                row[2] = (i == 0) ? populGroupTotals[i] : Convert.ToDouble(table.Rows[table.Rows.Count - 1][2]) + populGroupTotals[i];
                row[3] = GetGroupAverage(i + 1);
                row[4] = Convert.ToDouble(row[3]) * populGroupTotals[i];

                sum += Convert.ToDouble(row[4]);

                table.Rows.Add(row);

//                lorencKoeff += Math.Abs(chargeGroupTotals[i] - populGroupTotals[i]);

            }

            for (int i = 0; i < groupCount; i++)
            {
                DataRow row = table.Rows[i];

                row[5] = (sum == 0) ? 0 : Convert.ToDouble(row[4]) / sum;
                row[6] = (i == 0) ? Convert.ToDouble(row[5]) : Convert.ToDouble(table.Rows[i - 1][6]) + Convert.ToDouble(row[5]);
                jinyIndex += 2 * Convert.ToDouble(row[1]) * (Convert.ToDouble(row[6]) - Convert.ToDouble(row[5]) / 2);
            }

//            lorencKoeff = lorencKoeff / 2;
            jinyIndex = 1 - jinyIndex;

            LinearGaugeScale scale = ((LinearGauge) UltraGauge1.Gauges[0]).Scales[4];
            scale.Markers[0].Precision = 0.01;
            scale.Markers[0].Value = jinyIndex;
            double x = scale.Axes[0].Map(jinyIndex, 6, 85);

            ((Infragistics.UltraGauge.Resources.BoxAnnotation) UltraGauge1.Annotations[2]).Label.FormatString = jinyIndex.ToString("N2");
            Rectangle rec = ((Infragistics.UltraGauge.Resources.BoxAnnotation) UltraGauge1.Annotations[2]).Bounds;
            ((Infragistics.UltraGauge.Resources.BoxAnnotation)UltraGauge1.Annotations[2]).Bounds = new Rectangle(Convert.ToInt32(x), rec.Y, rec.Width, rec.Height);

            string eveness = (jinyIndex < 0.5) ? "равномерности" : "неравномерности";

            JinyIndexLabel.Text = string.Format("Индекс Джини характеризует концентрацию доходов бюджетов субъектов РФ по группам территорий. Чем больше значение индекса Джини, тем больше неравенство между территориями. Значение индекса <b>{0:N2}</b> говорит о <b>{1}</b> распределения доходного источника.",
                jinyIndex, eveness);

            return table;
        }

        private DataTable GetGrouppingTable(DataTable dt)
        {
            DataTable table = dt.Copy();

            int groupIndex = 0;
            Dictionary<int, int> groupEnds = new Dictionary<int, int>();

            for (int i = 1; i < table.Rows.Count; i++)
            {
                if (table.Rows[i][3] != DBNull.Value && GetGroupNumber(Convert.ToDouble(table.Rows[i][3])) > groupIndex)
                {
                    groupEnds.Add(i + groupEnds.Count, GetGroupNumber(Convert.ToDouble(table.Rows[i][3])));
                    groupIndex = GetGroupNumber(Convert.ToDouble(table.Rows[i][3]));
                }
            }

            foreach (KeyValuePair<int, int> item in groupEnds )
            {
                DataRow row = table.NewRow();
                row[0] = string.Format("Группа {1}: {0}", GetGroupName(item.Value), item.Value);
                table.Rows.InsertAt(row, item.Key);
            }

            return table;
        }

        #endregion

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0010_evenness_Grid");
            dtGrid1 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid1);

            double sum = 0;
            for (int i = 1; i < dtGrid1.Rows.Count; i++)
            {
                DataRow row = dtGrid1.Rows[i];

                if (row[1] != DBNull.Value)
                {
                    double value = Convert.ToDouble(row[1]);
                    sum += value;
                }
            }
            dtGrid1.Rows[0][1] = sum;
            dtGrid1.Rows[0][3] = DBNull.Value;

            if (dtGrid1.Rows[0][1] != DBNull.Value)
            {
                populTotal = Convert.ToDouble(dtGrid1.Rows[0][1]);
            }
            if (dtGrid1.Rows[0][2] != DBNull.Value)
            {
                chargeTotal = Convert.ToDouble(dtGrid1.Rows[0][2]);
            }

            if (dtGrid1.Rows.Count > 3 && dtGrid1.Rows[dtGrid1.Rows.Count - 1][3] != DBNull.Value)
            {
                avgMin = Convert.ToDouble(dtGrid1.Rows[dtGrid1.Rows.Count - 1][3]);
                dtGrid1.Rows.RemoveAt(dtGrid1.Rows.Count - 1);
            }
            if (dtGrid1.Rows.Count > 3 && dtGrid1.Rows[dtGrid1.Rows.Count - 1][3] != DBNull.Value)
            {
                avgMax = Convert.ToDouble(dtGrid1.Rows[dtGrid1.Rows.Count - 1][3]);
                dtGrid1.Rows.RemoveAt(dtGrid1.Rows.Count - 1);
            }

            volume = dtGrid1.Rows.Count - 1;

            groupCount = Convert.ToInt32(1 + 3.322*Math.Log10(volume));

            avgStep = (avgMax - avgMin) / groupCount;

            UltraWebGrid.DataSource = GetGrouppingTable(dtGrid1);
            //UltraWebGrid.DataSource = dtGrid1;
        }

        private int volume = 0;
        private int groupCount = 0;

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width, bool hidden)
        {
            CRHelper.FormatNumberColumn(layout.Bands[bandIndex].Columns[columnIndex], format);
            layout.Bands[bandIndex].Columns[columnIndex].Hidden = hidden;
            layout.Bands[bandIndex].Columns[columnIndex].Width = CRHelper.GetColumnWidth(width);
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(196);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            SetColumnParams(e.Layout, 0, 1, "N3", 95, false);
            SetColumnParams(e.Layout, 0, 2, "N3", 95, false);
            SetColumnParams(e.Layout, 0, 3, "N3", 95, false);

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, string.Format("Численность постоянного населения {0}, тыс.чел.", populationDate), "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Сумма налога, млн.руб.", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "Среднедушевые доходы, руб./чел.", "");
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Index == 0)
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }

            if (e.Row.Cells[0].Value.ToString().Contains("Группа"))
            {
                e.Row.Cells[0].ColSpan = 3;
                e.Row.Cells[0].Style.Font.Bold = true;
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
        
        protected void UnitWeightGrid_DataBinding(object sender, EventArgs e)
        {
            dtGrid2 = GetUnitWeightTable();
            UnitWeightGrid.DataSource = dtGrid2;
        }

        protected void UnitWeightGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowRowNumberingDefault = RowNumbering.Continuous;

            if (e.Layout.Bands[0].Columns.Count < 7)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(118);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            
            SetColumnParams(e.Layout, 0, 1, "P2", 92, false);
            SetColumnParams(e.Layout, 0, 2, "P2", 92, false);
            SetColumnParams(e.Layout, 0, 3, "N3", 92, true);
            SetColumnParams(e.Layout, 0, 4, "N3", 92, true);
            SetColumnParams(e.Layout, 0, 5, "P2", 92, false);
            SetColumnParams(e.Layout, 0, 6, "P2", 92, false);

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Удельный вес численности населения, %", "Доля численности населения, входящего\nв группу доходов, в общей численности\nпостоянного населения РФ");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Кумулятивный ряд численности, %", "Накопленные частоты по удельному весу\nчисленности населения группы");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "Середина интервала, тыс.руб.", "Доля суммарного дохода группы в общем объеме доходов РФ");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "Групповые доходы (процентные числа), тыс.руб.", "Накопленные частоты по удельному весу дохода группы");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "Удельный вес групп в общем доходе, %", "Доля суммарного дохода группы в общем объеме доходов РФ");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, "Кумулятивный ряд доходов, %", "Накопленные частоты по удельному весу дохода группы");
        }

        protected void UnitWeightGrid_InitializeRow(object sender, RowEventArgs e)
        {
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

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            if (dtGrid2 == null)
            {
                return;
            }

            UltraChart.ScatterChart.ColumnX = 2;
            UltraChart.ScatterChart.ColumnY = 6;

            DataRow nullRow = dtGrid2.NewRow();
            nullRow[2] = 0;
            nullRow[6] = 0;
            dtGrid2.Rows.InsertAt(nullRow, 0);

            UltraChart.DataSource = dtGrid2;
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            // рисуем биссектрису
            Line line = new Line();
            line.lineStyle.DrawStyle = LineDrawStyle.Dash;
            line.PE.Stroke = Color.DarkGray;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point((int)xAxis.MapMinimum, (int)yAxis.MapMinimum);
            line.p2 = new Point((int)xAxis.MapMaximum, (int)yAxis.MapMaximum);
            e.SceneGraph.Add(line);
        }

        #endregion

    }
}

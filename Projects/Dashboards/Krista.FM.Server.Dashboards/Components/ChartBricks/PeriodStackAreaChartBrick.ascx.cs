using System;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.Components.Components.ChartBricks
{
    public partial class PeriodStackAreaChartBrick : UltraChartBrick
    {
        private string yAxisLabelFormat;
        private TimeSpan periodSpan = new TimeSpan(0);
        private int periodMonthSpan = 0;
        private SymbolIconSize iconSize = SymbolIconSize.Medium;
        private string periodFormat = "dd.MM.yyyy";
        private bool axisXAutoFormat = true;

        public string YAxisLabelFormat
        {
            get { return yAxisLabelFormat; }
            set { yAxisLabelFormat = value; }
        }

        public TimeSpan PeriodSpan
        {
            get { return periodSpan; }
            set { periodSpan = value; }
        }

        public int PeriodMonthSpan
        {
            get { return periodMonthSpan; }
            set { periodMonthSpan = value; }
        }

        public SymbolIconSize IconSize
        {
            get { return iconSize; }
            set { iconSize = value; }
        }

        public string PeriodFormat
        {
            get { return periodFormat; }
            set { periodFormat = value; }
        }

        public bool AxisXAutoFormat
        {
            get { return axisXAutoFormat; }
            set { axisXAutoFormat = value; }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
        }

        protected override void SetChartAppearance()
        {
            base.SetChartAppearance();

            ChartControl.ChartType = ChartType.StackAreaChart;

            ChartControl.Axis.X.Labels.SeriesLabels.Visible = true;
            ChartControl.Axis.Y.Labels.ItemFormatString = String.Format("<DATA_VALUE:{0}>", YAxisLabelFormat);

            foreach(PaintElement pe in ChartControl.ColorModel.Skin.PEs)
            {
                LineAppearance lineAppearance = new LineAppearance();
                lineAppearance.IconAppearance.Icon = SymbolIcon.Diamond;
                lineAppearance.IconAppearance.IconSize = iconSize;
                lineAppearance.IconAppearance.PE = pe;
                ChartControl.AreaChart.LineAppearances.Add(lineAppearance);
            }
            
            ChartControl.TitleLeft.Visible = true;
        }

        public override void DataBind()
        {
            if (AxisXAutoFormat && DataTable.Rows.Count > 0 && DataTable.Columns.Count > 1)
            {
                DataTable.Columns.RemoveAt(0);
                ReplaceDateFormat(DataTable, 0, periodSpan);
            }

            base.DataBind();
        }

        private void ReplaceDateFormat(DataTable dt, int dateColumnIndex, TimeSpan timeSpan)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (row[dateColumnIndex] != DBNull.Value)
                {
                    DateTime dataTime = CRHelper.PeriodDayFoDate(row[dateColumnIndex].ToString());
                    dataTime = dataTime.Add(timeSpan);
                    dataTime = dataTime.AddMonths(periodMonthSpan);
                    row[dateColumnIndex] = dataTime.ToString(periodFormat);
                }
            }
        }
    }
}
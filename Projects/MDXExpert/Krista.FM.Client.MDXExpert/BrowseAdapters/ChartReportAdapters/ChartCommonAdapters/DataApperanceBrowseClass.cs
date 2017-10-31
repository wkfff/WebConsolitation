using System;
using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DataApperanceBrowseClass : FilterablePropertyBase
    {
        #region ����

        private DataAppearance dataAppearance;
        private EmptyStyleBrowseClass emptyStyleBrowse;
        private DataMinMaxBrowseClass dataMinMaxBrowse;
        private DataRowLabelsColumnBrowseClass dataRowLabelsColumnBrowse;
        private UltraChart chart;

        #endregion

        #region ��������

        /// <summary>
        /// ��� ���������
        /// </summary>
        [Browsable(false)]
        public ChartType ChartType
        {
            get { return chart.ChartType; }
        }
        
        /// <summary>
        /// ����������� ��������� ����������� ������
        /// </summary>
        [Browsable(false)]
        public bool MinMaxEnable
        {
            get
            {
                return (ChartType == ChartType.AreaChart || ChartType == ChartType.SplineAreaChart ||
                        ChartType == ChartType.StepAreaChart || ChartType == ChartType.BarChart ||
                        ChartType == ChartType.StackBarChart || ChartType == ChartType.ColumnChart ||
                        ChartType == ChartType.StackColumnChart || ChartType == ChartType.ColumnChart3D ||
                        ChartType == ChartType.ParetoChart || ChartType == ChartType.BubbleChart ||
                        ChartType == ChartType.ColumnLineChart || ChartType == ChartType.GanttChart ||
                        ChartType == ChartType.CylinderColumnChart3D || ChartType == ChartType.ProbabilityChart ||
                        ChartType == ChartType.LineChart || ChartType == ChartType.SplineChart ||
                        ChartType == ChartType.StepLineChart || ChartType == ChartType.ScatterChart ||
                        ChartType == ChartType.ScatterLineChart || ChartType == ChartType.ConeChart3D ||
                        ChartType == ChartType.FunnelChart3D || ChartType == ChartType.FunnelChart ||
                        ChartType == ChartType.PyramidChart3D || ChartType == ChartType.PyramidChart);
            }
        }

        /// <summary>
        /// ������������ ����� � �������
        /// </summary>
        [Category("���")]
        [Description("��������, ������������� � ������� \"���������\", ����� �������������� ��� \"����\", � �������� ����� ����� �������������� ��� ���������")]
        [DisplayName("������������ ����� � ���������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool SwapRowsAndColumns
        {
            get { return dataAppearance.SwapRowsAndColumns; }
            set { dataAppearance.SwapRowsAndColumns = value; }
        }

        /// <summary>
        /// ������������ �� ����
        /// </summary>
        [Category("���")]
        [Description("������������ �� ����")]
        [DisplayName("������������ �� ����")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool ZeroAligned
        {
            get { return dataAppearance.ZeroAligned; }
            set { dataAppearance.ZeroAligned = value; }
        }

        /// <summary>
        /// ����� ������ ��������
        /// </summary>
        [Category("���")]
        [Description("����� ������ ��������")]
        [DisplayName("����� ������ ��������")]
        [Browsable(true)]
        public EmptyStyleBrowseClass EmptyStyleBrowse
        {
            get { return emptyStyleBrowse; }
            set { emptyStyleBrowse = value; }
        }

        /// <summary>
        /// �����������
        /// </summary>
        [Category("���")]
        [Description("����������� �� ������������ � ������������� ���������")]
        [DisplayName("�����������")]
        [DynamicPropertyFilter("MinMaxEnable", "True")]
        [Browsable(true)]
        public DataMinMaxBrowseClass DataMinMaxBrowse
        {
            get { return dataMinMaxBrowse; }
            set { dataMinMaxBrowse = value; }
        }
        /*
        [Category("���")]
        [Description("����� �����")]
        [DisplayName("����� �����")]
        [Browsable(true)]
        public string[] RowLabels
        {
            get { return dataAppearance.GetRowLabels(); }
            set { dataAppearance.SetRowLabels(value); }
        }


        [Category("���")]
        [Description("����� ���������")]
        [DisplayName("����� ���������")]
        [Browsable(true)]
        public string[] ColumnLabels
        {
            get { return dataAppearance.GetColumnLabels(); }
            set { dataAppearance.SetColumnLabels(value); }
        }

        [Category("���")]
        [Description("TEST")]
        [DisplayName("TEST")]
        [Browsable(true)]
        public DataAppearance Data
        {
            get { return dataAppearance; }
            set { dataAppearance = value; }
        }
        */

        /*/// <summary>
        /// ����������� ���������
        /// </summary>
        [Category("���")]
        [Description("����������� ���������")]
        [DisplayName("����������� ���������")]
        [Browsable(true)]
        public DataRowLabelsColumnBrowseClass DataRowLabelsColumnBrowse
        {
            get { return dataRowLabelsColumnBrowse; }
            set { dataRowLabelsColumnBrowse = value; }
        }*/

        #endregion

        public DataApperanceBrowseClass(DataAppearance dataAppearance, UltraChart chart)
        {
            this.dataAppearance = dataAppearance;
            this.chart = chart;

            emptyStyleBrowse = new EmptyStyleBrowseClass(dataAppearance.EmptyStyle);
            dataMinMaxBrowse = new DataMinMaxBrowseClass(dataAppearance);
            dataRowLabelsColumnBrowse = new DataRowLabelsColumnBrowseClass(dataAppearance);
            
        }

        public override string ToString()
        {
            return MinMaxEnable ? dataMinMaxBrowse.ToString() : String.Empty;
        }
    }
}
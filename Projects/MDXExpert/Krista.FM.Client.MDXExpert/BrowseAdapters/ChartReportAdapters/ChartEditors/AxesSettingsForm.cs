using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Win.UltraWinChart;
using Infragistics.Win.UltraWinListView;

namespace Krista.FM.Client.MDXExpert
{
    public partial class AxesSettingsForm : Form
    {
        private AxiesBrowseClass axesBrowse;

        public AxesSettingsForm(UltraChart chart)
        {
            InitializeComponent();
            this.axesBrowse = new AxiesBrowseClass(chart.Axis, chart);

            InitAxesList();
        }

        private void InitAxesList()
        {
            if (this.axesBrowse == null)
                return;
            lvAxes.Items.Clear();

            switch (axesBrowse.ChartType)
            {
                case ChartType.AreaChart3D:
                case ChartType.BarChart3D:
                case ChartType.BubbleChart3D:
                case ChartType.ColumnChart3D:
                case ChartType.CylinderBarChart3D:
                case ChartType.CylinderColumnChart3D:
                case ChartType.CylinderStackBarChart3D:
                case ChartType.CylinderStackColumnChart3D:
                case ChartType.HeatMapChart3D:
                case ChartType.LineChart3D:
                case ChartType.PointChart3D:
                case ChartType.SplineAreaChart3D:
                case ChartType.SplineChart3D:
                case ChartType.Stack3DBarChart:
                case ChartType.Stack3DColumnChart:
                    lvAxes.Items.Add("XAxis", "Ось X").Tag = axesBrowse.XAxisBrowse;
                    lvAxes.Items.Add("YAxis", "Ось Y").Tag = axesBrowse.YAxisBrowse;
                    lvAxes.Items.Add("ZAxis", "Ось Z").Tag = axesBrowse.ZAxisBrowse;
                    break;
                case ChartType.ColumnChart:
                case ChartType.BarChart:
                case ChartType.AreaChart:
                case ChartType.LineChart:
                case ChartType.BubbleChart:
                case ChartType.ScatterChart:
                case ChartType.HeatMapChart:
                case ChartType.StackBarChart:
                case ChartType.StackColumnChart:
                case ChartType.SplineChart:
                case ChartType.SplineAreaChart:
                case ChartType.ColumnLineChart:
                case ChartType.ScatterLineChart:
                case ChartType.ParetoChart:
                case ChartType.StackAreaChart:
                case ChartType.StackLineChart:
                case ChartType.StackSplineChart:
                case ChartType.StackSplineAreaChart:
                case ChartType.HistogramChart:
                case ChartType.ProbabilityChart:
                case ChartType.BoxChart:
                case ChartType.GanttChart:
                    lvAxes.Items.Add("XAxis", "Ось X").Tag = axesBrowse.XAxisBrowse;
                    lvAxes.Items.Add("YAxis", "Ось Y").Tag = axesBrowse.YAxisBrowse;
                    lvAxes.Items.Add("X2Axis", "Ось X2").Tag = axesBrowse.X2AxisBrowse;
                    lvAxes.Items.Add("Y2Axis", "Ось Y2").Tag = axesBrowse.Y2AxisBrowse;
                    break;
            }

        }

        private void lvAxes_ItemSelectionChanged(object sender, Infragistics.Win.UltraWinListView.ItemSelectionChangedEventArgs e)
        {
            if (e.SelectedItems.Count > 0)
            {
                if (e.SelectedItems[0].Tag != null)
                {
                    propertyGrid.SelectedObject = (AxisBrowseClass) e.SelectedItems[0].Tag;
                    lProperty.Text = String.Format("Свойства '{0}':", e.SelectedItems[0].Text); 
                }
            }
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            propertyGrid.Refresh();
        }



    }
}
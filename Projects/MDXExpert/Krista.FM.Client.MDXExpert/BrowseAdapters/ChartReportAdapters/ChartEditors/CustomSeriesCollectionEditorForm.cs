using System;
using System.Collections;
using System.ComponentModel;
using Infragistics.UltraChart.Data;
using Infragistics.UltraChart.Data.Series;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.Win.UltraWinChart;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomSeriesCollectionEditorForm : CustomChartCollectionEditorBaseForm
    {

        private CustomNumericSeries _numericSeriesBrowse;
        private CustomXYSeries _xySeriesBrowse;
        private CustomXYZSeries _xyzSeriesBrowse;
        private CustomCandleSeries _candleSeriesBrowse;
        private CustomGanttSeries _ganttSeriesBrowse;
        private CustomNumericTimeSeries _numericTimeSeriesBrowse;
        private CustomBoxSetSeries _boxSetSeriesBrowse;
        private CustomFourDimensionalNumericSeries _fourDimensionalNumericSeriesBrowse;

        public CustomSeriesCollectionEditorForm(IChartCollection collection, PropertyDescriptor property)
            : base(collection, property)
        {
            InitializeComponent();
            this.listBox.SelectedIndexChanged += new EventHandler(this.listBox_SelectedIndexChanged);
            this.okButton.Click += new EventHandler(okButton_Click);
            // показываем модальное окно
            this.ShowDialog(MainForm.ActiveForm);

        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox.SelectedItem is NumericSeries)
            {
                this._numericSeriesBrowse = new CustomNumericSeries((NumericSeries)this.listBox.SelectedItem);
                this.propertyGrid.SelectedObject = this._numericSeriesBrowse;
            }
            else if (this.listBox.SelectedItem is XYSeries)
            {
                this._xySeriesBrowse = new CustomXYSeries((XYSeries)this.listBox.SelectedItem);
                this.propertyGrid.SelectedObject = this._xySeriesBrowse;
            }
            else if (this.listBox.SelectedItem is XYZSeries)
            {
                this._xyzSeriesBrowse = new CustomXYZSeries((XYZSeries)this.listBox.SelectedItem);
                this.propertyGrid.SelectedObject = this._xyzSeriesBrowse;
            }
            else if (this.listBox.SelectedItem is CandleSeries)
            {
                this._candleSeriesBrowse = new CustomCandleSeries((CandleSeries)this.listBox.SelectedItem);
                this.propertyGrid.SelectedObject = this._candleSeriesBrowse;
            }
            else if (this.listBox.SelectedItem is GanttSeries)
            {
                this._ganttSeriesBrowse =
                    new CustomGanttSeries((GanttSeries)this.listBox.SelectedItem);
                this.propertyGrid.SelectedObject = this._ganttSeriesBrowse;
            }
            else if (this.listBox.SelectedItem is NumericTimeSeries)
            {
                this._numericTimeSeriesBrowse =
                    new CustomNumericTimeSeries((NumericTimeSeries)this.listBox.SelectedItem);
                this.propertyGrid.SelectedObject = this._numericTimeSeriesBrowse;
            }
            else if (this.listBox.SelectedItem is BoxSetSeries)
            {
                this._boxSetSeriesBrowse =
                    new CustomBoxSetSeries((BoxSetSeries)this.listBox.SelectedItem);
                this.propertyGrid.SelectedObject = this._boxSetSeriesBrowse;
            }
            else if (this.listBox.SelectedItem is FourDimensionalNumericSeries)
            {
                this._fourDimensionalNumericSeriesBrowse =
                    new CustomFourDimensionalNumericSeries((FourDimensionalNumericSeries)this.listBox.SelectedItem);
                this.propertyGrid.SelectedObject = this._fourDimensionalNumericSeriesBrowse;
            }
            else
            {
                this.propertyGrid.SelectedObject = this.listBox.SelectedItem;
            }
        }

        protected override void OnCancel()
        {
            SeriesCollection oldCollection = this.OldCollection as SeriesCollection;
            SeriesCollection collection = base.Collection as SeriesCollection;
            if (base.Collection.ChartComponent != null)
            {
                CompositeChartAppearance chartAppearance = base.Collection.ChartComponent.GetChartAppearance(ChartAppearanceTypes.Composite) as CompositeChartAppearance;
                if (chartAppearance != null)
                {
                    foreach (ChartLayerAppearance appearance2 in (IEnumerable)chartAppearance.ChartLayers)
                    {
                        foreach (ISeries series in collection)
                        {
                            if (appearance2.Series.Contains(series) && (oldCollection.FromKey(series.Key) != null))
                            {
                                int index = appearance2.Series.IndexOf(series);
                                if ((index > -1) && (index < appearance2.Series.Count))
                                {
                                    appearance2.Series[index] = oldCollection.FromKey(series.Key);
                                }
                            }
                        }
                    }
                }
            }
            base.OnCancel();
        }

        protected override void RefreshChart()
        {
            if (((base.Collection != null) && (base.Collection.ChartComponent != null)) && (base.Collection.ChartComponent is UltraChart))
            {
                ((DataAppearance)base.Collection.ChartComponent.GetChartAppearance(ChartAppearanceTypes.Data)).SetDirty(true);
            }
            base.RefreshChart();
        }

        void okButton_Click(object sender, EventArgs e)
        {
            MainForm.Instance.Saved = false;
        }

        protected override Type[] ItemTypes
        {
            get
            {
                return new Type[] { typeof(NumericSeries), typeof(XYSeries), typeof(XYZSeries), typeof(CandleSeries), typeof(GanttSeries), typeof(NumericTimeSeries), typeof(BoxSetSeries), typeof(FourDimensionalNumericSeries) };
            }
        }

        protected override string[] TypeNames
        {
            get
            {
                return new string[] { "Numeric Series", "XY Series", "XYZ Series", "Candle Series", "Gantt Series", "Numeric Time Series", "Box Set Series", "Four-Dimensional Numeric Series" };
            }
        }

        private void InitializeComponent()
        {
            this.Name = "CustomSeriesCollectionEditorForm";
            this.Text = "Серии";

        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using Infragistics.UltraChart.Core.Layers;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Data.Series;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Форма редактора слоев
    /// </summary>
    public class ChartLayerCollectionEditorForm : CustomChartCollectionEditorBaseForm
    {
        // Fields
        private Container components;

        private ChartLayerAppearanceBrowseClass _chartLayerBrowse;

        // Methods
        public ChartLayerCollectionEditorForm(IChartCollection collection, PropertyDescriptor property)
            : base(collection, property)
        {
            this.InitializeComponent();
            this.listBox.SelectedIndexChanged += new EventHandler(this.listBox_SelectedIndexChanged);
            // показываем модальное окно
            this.ShowDialog(MainForm.ActiveForm);

        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox.SelectedItem is ChartLayerAppearance)
            {
                this._chartLayerBrowse = new ChartLayerAppearanceBrowseClass((ChartLayerAppearance)this.listBox.SelectedItem);
                this.propertyGrid.SelectedObject = this._chartLayerBrowse;
            }
            else
            {
                this.propertyGrid.SelectedObject = this.listBox.SelectedItem;
            }
        }

        protected override void OnCancel()
        {
            /*
            ChartLayerCollection oldCollection = this.OldCollection as ChartLayerCollection;
            ChartLayerCollection collection = base.Collection as ChartLayerCollection;

            if (base.Collection.ChartComponent != null)
            {
                CompositeChartAppearance chartAppearance =
                    base.Collection.ChartComponent.GetChartAppearance(ChartAppearanceTypes.Composite) as
                    CompositeChartAppearance;
                if (chartAppearance != null)
                {
                    for (int i = 0; i < collection.Count; i++ )
                    {
                        if (chartAppearance.ChartLayers.Contains(collection[i]) && (oldCollection.FromKey(collection[i].Key) != null))
                        {
                            int index = chartAppearance.ChartLayers.IndexOf(collection[i]);
                            if ((index > -1) && (index < chartAppearance.ChartLayers.Count))
                            {
                                chartAppearance.ChartLayers[index] = oldCollection.FromKey(collection[i].Key);
                            }
                        }
                    }
                }
            }
            base.OnCancel();*/
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.Name = "ChartLayerCollectionEditorForm";
            this.Text = "Коллекция слоев";
        }


        protected override Type[] ItemTypes
        {
            get
            {
                return new Type[] { typeof(ChartLayerAppearance) };
            }
        }

    }
}


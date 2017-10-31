using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Форма редактора подписей данных
    /// </summary>
    public class CustomChartTextCollectionEditorForm : CustomChartCollectionEditorBaseForm
    {
        // Fields
        private Container components;
        // Адаптер для подписей данных
        private ChartAppearanceBrowseClass chartAppearanceBrowse;

        // Methods
        public CustomChartTextCollectionEditorForm(IChartCollection collection, PropertyDescriptor property)
            : base(collection, property)
        {
            this.InitializeComponent();
            this.listBox.SelectedIndexChanged += new EventHandler(this.listBox_SelectedIndexChanged);
            // показываем модальное окно
            this.ShowDialog(MainForm.ActiveForm);
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox.SelectedItem is ChartTextAppearance)
            {
                chartAppearanceBrowse = new ChartAppearanceBrowseClass((ChartTextAppearance)this.listBox.SelectedItem, this.Collection.ChartComponent);
                this.propertyGrid.SelectedObject = chartAppearanceBrowse;
            }
            else
            {
                this.propertyGrid.SelectedObject = this.listBox.SelectedItem;
            }

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
            this.Name = "CustomChartTextCollectionEditorForm";
            this.Text = "Коллекция подписей";
        }

        // Properties
        protected override Type[] ItemTypes
        {
            get
            {
                return new Type[] { typeof(ChartTextAppearance) };
            }
        }
    }
}

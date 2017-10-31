using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Форма редактора стиля линий
    /// </summary>
    public class EmptyAppearanceCollectionEditorForm : CustomChartCollectionEditorBaseForm
    {
        // Fields
        private Container components;

        //Стиль линии
        private EmptyAppearanceBrowseClass _emptyAppearanceBrowse;

        // Methods
        public EmptyAppearanceCollectionEditorForm(IChartCollection collection, PropertyDescriptor property)
            : base(collection, property)
        {
            this.InitializeComponent();
            this.listBox.SelectedIndexChanged += new EventHandler(this.listBox_SelectedIndexChanged);
            // показываем модальное окно
            this.ShowDialog(MainForm.ActiveForm);

        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox.SelectedItem is EmptyAppearance)
            {
                _emptyAppearanceBrowse = new EmptyAppearanceBrowseClass((EmptyAppearance)this.listBox.SelectedItem);
                this.propertyGrid.SelectedObject = _emptyAppearanceBrowse;
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
            this.Name = "EmptyAppearanceCollectionEditorForm";
            this.Text = "Коллекция стилей отображения пустых значений";
        }


        protected override Type[] ItemTypes
        {
            get
            {
                return new Type[] { typeof(EmptyAppearance) };
            }
        }

    }
}


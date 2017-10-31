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
    /// Форма редактора аннотаций
    /// </summary>
    public class AnnotationCollectionEditorForm : CustomChartCollectionEditorBaseForm
    {
        // Fields
        private Container components;

        //аннотации
        private CustomLineAnnotation lineAnnotationBrowse;
        private CustomEllipseAnnotation ellipseAnnotationBrowse;
        private CustomCalloutAnnotation calloutAnnotationBrowse;
        private CustomBoxAnnotation boxAnnotationBrowse;
        private CustomLineImageAnnotation lineImageAnnotationBrowse;

        // Methods
        public AnnotationCollectionEditorForm(IChartCollection collection, PropertyDescriptor property)
            : base(collection, property)
        {
            this.InitializeComponent();
            this.listBox.SelectedIndexChanged += new EventHandler(this.listBox_SelectedIndexChanged);
            // показываем модальное окно
            this.ShowDialog(MainForm.ActiveForm);

        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox.SelectedItem is LineAnnotation)
            {
                lineAnnotationBrowse = new CustomLineAnnotation((LineAnnotation) this.listBox.SelectedItem);
                this.propertyGrid.SelectedObject = lineAnnotationBrowse;
            }
            else if (this.listBox.SelectedItem is EllipseAnnotation)
            {
                ellipseAnnotationBrowse = new CustomEllipseAnnotation((EllipseAnnotation) this.listBox.SelectedItem);
                this.propertyGrid.SelectedObject = ellipseAnnotationBrowse;
            }
            else if (this.listBox.SelectedItem is CalloutAnnotation)
            {
                calloutAnnotationBrowse = new CustomCalloutAnnotation((CalloutAnnotation) this.listBox.SelectedItem);
                this.propertyGrid.SelectedObject = calloutAnnotationBrowse;
            }
            else if (this.listBox.SelectedItem is BoxAnnotation)
            {
                boxAnnotationBrowse = new CustomBoxAnnotation((BoxAnnotation) this.listBox.SelectedItem);
                this.propertyGrid.SelectedObject = boxAnnotationBrowse;
            }
            else if (this.listBox.SelectedItem is LineImageAnnotation)
            {
                lineImageAnnotationBrowse =
                    new CustomLineImageAnnotation((LineImageAnnotation) this.listBox.SelectedItem);
                this.propertyGrid.SelectedObject = lineImageAnnotationBrowse;
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
            this.Name = "AnnotationCollectionEditorForm";
            this.Text = "Коллекция аннотаций";
        }


        protected override Type[] ItemTypes
        {
            get
            {
                return new Type[] { typeof(LineAnnotation), typeof(EllipseAnnotation), typeof(CalloutAnnotation), typeof(BoxAnnotation), typeof(LineImageAnnotation) };
            }
        }

        protected override string[] TypeNames
        {
            get
            {
                return new string[] { "линия", "эллипс", "выноска", "прямоугольник", "изображение"};
            }
        }
    }
}


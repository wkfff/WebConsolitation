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
    /// Форма редактора эффектов
    /// </summary>
    public class CustomEffectsCollectionEditorForm : CustomChartCollectionEditorBaseForm
    {
        // Fields
        private Container components;

        private CustomGradientEffect gradientEffectBrowse;
        private CustomShadowEffect shadowEffectBrowse;
        private CustomStrokeEffect strokeEffectBrowse;
        private CustomTextureEffect textureEffectBrowse;
        private CustomThreeDEffect threeDEffectBrowse;

        // Methods
        public CustomEffectsCollectionEditorForm(IChartCollection collection, PropertyDescriptor property)
            : base(collection, property)
        {
            this.InitializeComponent();
            this.listBox.SelectedIndexChanged += new EventHandler(this.listBox_SelectedIndexChanged);
            // показываем модальное окно
            this.ShowDialog(MainForm.ActiveForm);
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox.SelectedItem is GradientEffect)
            {
                gradientEffectBrowse = new CustomGradientEffect((GradientEffect)this.listBox.SelectedItem);
                this.propertyGrid.SelectedObject = gradientEffectBrowse;
            }
            else if (this.listBox.SelectedItem is ShadowEffect)
            {
                shadowEffectBrowse = new CustomShadowEffect((ShadowEffect)this.listBox.SelectedItem);
                this.propertyGrid.SelectedObject = shadowEffectBrowse;
            }
            else if (this.listBox.SelectedItem is StrokeEffect)
            {
                strokeEffectBrowse = new CustomStrokeEffect((StrokeEffect)this.listBox.SelectedItem);
                this.propertyGrid.SelectedObject = strokeEffectBrowse;
            }
            else if (this.listBox.SelectedItem is TextureEffect)
            {
                textureEffectBrowse = new CustomTextureEffect((TextureEffect)this.listBox.SelectedItem);
                this.propertyGrid.SelectedObject = textureEffectBrowse;
            }
            else if (this.listBox.SelectedItem is ThreeDEffect)
            {
                threeDEffectBrowse =
                    new CustomThreeDEffect((ThreeDEffect)this.listBox.SelectedItem);
                this.propertyGrid.SelectedObject = threeDEffectBrowse;
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
            this.Name = "CustomEffectsCollectionEditorForm";
            this.Text = "Коллекция эффектов";
        }


        protected override Type[] ItemTypes
        {
            get
            {
                return new Type[] { typeof(GradientEffect), typeof(TextureEffect), typeof(ShadowEffect), typeof(ThreeDEffect), typeof(StrokeEffect) };
            }
        }

        protected override string[] TypeNames
        {
            get
            {
                return new string[] { "эффект градиента", "эффект текстуры", "эффект тени", "эффект объема", "эффект штриховки"};
            }
        }
    }
}


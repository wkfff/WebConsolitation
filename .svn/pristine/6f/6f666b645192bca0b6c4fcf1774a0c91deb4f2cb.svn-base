using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using Infragistics.UltraChart.Core.Layers;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.Win.Misc;

namespace Krista.FM.Client.MDXExpert
{
    public class ChartLayerPickerEditor : UITypeEditor
    {
        // Fields
        private Form _designForm;

        // Methods
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            ChartLayerCollection layers = value as ChartLayerCollection;
            if (((context != null) && (context.Instance != null)) && ((provider != null) && (layers != null)))
            {
                PropertyInfo property = context.Instance.GetType().GetProperty("ChartLayers");
                if (property == null)
                {
                    return value;
                }
                IChartCollection charts = property.GetValue(context.Instance, null) as IChartCollection;
                if (charts == null)
                {
                    return value;
                }
                ChartLayerCollection chartLayers = ((CompositeChartAppearance)charts.ChartComponent.GetChartAppearance(ChartAppearanceTypes.Composite)).ChartLayers;
                this._designForm = new Form();
                ListBox box = new ListBox();
                box.SelectionMode = SelectionMode.MultiExtended;
                UltraLabel label = new UltraLabel();
                label.Text = "Выберите слои, которые нужно отображать в легенде";
                label.Dock = DockStyle.Bottom;
                label.Height = 0x12;
                UltraButton button = new UltraButton();
                button.Text = "OK";
                this._designForm.AcceptButton = button;
                button.Dock = DockStyle.Bottom;
                this._designForm.Text = "Выбор слоев";
                this._designForm.Show();
                this._designForm.Opacity = 0.0;
                this._designForm.Controls.Add(label);
                this._designForm.Controls.Add(button);
                this._designForm.Controls.Add(box);
                box.Dock = DockStyle.Fill;
                button.Click += new EventHandler(this.OkButtonClick);
                box.Items.AddRange(chartLayers.ToArray());
                for (int i = 0; i < box.Items.Count; i++)
                {
                    ChartLayerAppearance appearance = box.Items[i] as ChartLayerAppearance;
                    if ((appearance != null) && (layers.FromKey(appearance.Key) != null))
                    {
                        box.SetSelected(i, true);
                    }
                }
                this._designForm.Visible = false;
                this._designForm.Opacity = 100.0;
                if (this._designForm.ShowDialog() != DialogResult.OK)
                {
                    return value;
                }
                layers.Clear();
                foreach (ChartLayerAppearance appearance2 in box.SelectedItems)
                {
                    layers.Add(appearance2);
                }
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if ((context != null) && (context.Instance != null))
            {
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(context);
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return false;
        }

        private void OkButtonClick(object sender, EventArgs e)
        {
            this._designForm.DialogResult = DialogResult.OK;
            this._designForm.Close();
        }
    }

 

}

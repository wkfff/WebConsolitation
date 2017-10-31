using System.Windows.Forms;
using Infragistics.UltraChart.Resources.Editor;
//using Infragistics.UltraChart.Design;
using System.ComponentModel;
using System;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.Win.UltraWinTabControl;

namespace Krista.FM.Client.MDXExpert
{
    public class ChartTypeEditor : ChartUIEditorBase
    {
        // Methods
        public ChartTypeEditor()
            : base(typeof(ChartTypeCtrl))
        {
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object val)
        {
            if (base.EditorControl.Parent != null)
            {
                this.RecreateEditorControl();
            }
            base.EditorControl.Width = ChartTypeCtrl.CONTROL_WIDTH;
            ChartTypeCtrl editorControl = base.EditorControl as ChartTypeCtrl;
            editorControl.SetValue(val);
            editorControl.ResetTabs();
            object obj2 = null;
            if ((context != null) && (editorControl != null))
            {
                editorControl.SetComposite(context.Instance is ChartLayerAppearance);
            }
            obj2 = base.EditValue(context, provider, val);
            if (editorControl != null)
            {
                editorControl.ResetTabs();
            }
            return obj2;
        }

    }
}
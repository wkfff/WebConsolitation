using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace Krista.FM.Client.MDXExpert.MapTuner
{
    public class MapTemplateEditor : UITypeEditor
    {
        /// <summary>
        /// Реализация метода редактирования
        /// </summary>
        public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, Object value)
        {
            if ((context != null) && (provider != null))
            {
                IWindowsFormsEditorService svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (svc != null)
                {

                    //MapTemplateBrowser mtCtrl = new MapTemplateBrowser(mainForm.MapRepositoryPath, (string)value);
                    MapTemplateBrowser mtCtrl = new MapTemplateBrowser(Consts.mapRepositoryPath, (string)value);
                    mtCtrl.Tag = svc;
                    
                    svc.DropDownControl(mtCtrl);
                    value = mtCtrl.MapTemplatePath;
                }
            }

            return value; //result 
        }

        /// <summary>
        /// Возвращаем стиль редактора - выпадающее окно
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
        {
            return false;
        }

        public override void PaintValue(System.Drawing.Design.PaintValueEventArgs e)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Common.RegistryUtils;

namespace Krista.FM.Client.MDXExpert
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

                    Utils regUtils = new Utils(typeof(MapReportElement), true);
                    string curTemplate = regUtils.GetKeyValue(Consts.mapTemplateNameRegKey);

                    MapTemplateBrowser mtCtrl = new MapTemplateBrowser(MapReportElement.MapRepositoryPath, curTemplate);
                    mtCtrl.Tag = svc;
                    
                    svc.DropDownControl(mtCtrl);
                    if (mtCtrl.ValueChanged)
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Dundas.Maps.WinControl;

namespace Krista.FM.Client.MDXExpert
{
    public class OffsetPointEditor : UITypeEditor
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
                    OffsetPointControl offsetCtrl = new OffsetPointControl((MapPoint)value);
                    offsetCtrl.Tag = svc;

                    svc.DropDownControl(offsetCtrl);
                    value = offsetCtrl.OffsetPoint;


                }
            }

            return base.EditValue(context, provider, value);  
        }

        /// <summary>
        /// Возвращаем стиль редактора - выпадающее окно
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                return UITypeEditorEditStyle.DropDown;
            }
            else
            {
                return base.GetEditStyle(context);
            }
        }

    }

}

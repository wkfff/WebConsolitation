using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace Krista.FM.Client.MDXExpert
{
    public class MapSeriesCollectionEditor : UITypeEditor
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
                    MapSeriesCollectionEditorForm mapSeriesForm = new MapSeriesCollectionEditorForm((MapSerieCollection)value);
                    //mapSeriesForm.InitEditor();

                    mapSeriesForm.ShowDialog();
                }
            }

            return base.EditValue(context, provider, value); //result 
        }

        /// <summary>
        /// Возвращаем стиль редактора - модальная форма
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                return UITypeEditorEditStyle.Modal;
            }
            else
            {
                return base.GetEditStyle(context);
            }

        }

    }

}
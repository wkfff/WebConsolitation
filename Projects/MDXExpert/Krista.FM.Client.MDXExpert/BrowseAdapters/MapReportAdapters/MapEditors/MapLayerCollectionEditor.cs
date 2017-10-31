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
    public class MapLayerCollectionEditor : UITypeEditor
    {
        private static MainForm mainForm;


        public static MainForm MainForm
        {
            get { return mainForm; }
            set { mainForm = value; }
        }


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
                    MapReportElement mapElement = null;
                    if (mainForm != null)
                    {
                        mapElement = mainForm.ActiveMapElement;
                    }

                    MapLayerCollectionEditorForm mapLayersForm = new MapLayerCollectionEditorForm(mapElement);
                    mapLayersForm.ShowDialog();
                    if (MainForm != null)
                    {
                        MainForm.Saved = false;
                    }
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
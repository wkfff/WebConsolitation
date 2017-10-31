using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Krista.FM.Client.MDXExpert.Controls;

namespace Krista.FM.Client.MDXExpert
{
    public class CubeChooserEditor : UITypeEditor
    {

        private CustomReportElement element;

        /// <summary>
        /// Реализация метода редактирования
        /// </summary>
        public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, Object value)
        {
            if ((context != null) && (provider != null))
            {
                IWindowsFormsEditorService svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));


                bool isCustomMap = false;

                if (context.Instance != null)
                {
                    if (context.Instance is CustomReportElementBrowseAdapter)
                    {
                        this.element = ((CustomReportElementBrowseAdapter) context.Instance).ReportElement;
                        if (this.element.IsCustomMap)
                        {

                            if (MessageBox.Show("В случае выбора куба в качестве источника данных для карты пользовательские данные будут утеряны. Продолжить?",
                                "MDX Expert", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                            {
                                return base.EditValue(context, provider, value);
                            }
                            else
                            {
                                //запоминаем что задаем куб для карты по пользовательским данным
                                isCustomMap = true;
                                ((MapReportElement) element).DataSourceType = DataSourceType.Cube;
                            }
                        }

                    }
                }


                if (svc != null)
                {
                    CubeChooser cubeChooser = new CubeChooser(); 
                    cubeChooser.LoadMetadata();
                    string cubeName = cubeChooser.SelectCube((string)value);

                    if (!String.IsNullOrEmpty(cubeName))
                    {
                        value = cubeName;
                        if (isCustomMap)
                        {
                            ((MapReportElement)element).MainForm.FieldListEditor.PivotDataContainer.Refresh();
                        }
                    }
                    else
                    {
                        if (isCustomMap)
                        {
                            ((MapReportElement) element).DataSourceType = DataSourceType.Custom;
                        }
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
                if (context.PropertyDescriptor.IsReadOnly)
                {
                    return UITypeEditorEditStyle.None;
                }

                return UITypeEditorEditStyle.Modal;
            }
            else
            {
                return UITypeEditorEditStyle.None;
            }

        }

    }

}
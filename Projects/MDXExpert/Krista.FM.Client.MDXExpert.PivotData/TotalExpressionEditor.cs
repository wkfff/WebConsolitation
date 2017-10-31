using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Krista.FM.Client.MDXExpert.CommonClass;

namespace Krista.FM.Client.MDXExpert.Data
{
    public class TotalExpressionEditor : UITypeEditor
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
                    TotalExpressionForm expressionFrm = new TotalExpressionForm((PivotTotal)value);
                    if (expressionFrm.ShowDialog() == DialogResult.OK)
                    {
                        ((PivotTotal)value).MeasureSource = expressionFrm.MeasureSource;
                        ((PivotTotal)value).FormulaType = expressionFrm.FormulaType;
                        ((PivotTotal)value).IsLookupMeasure = expressionFrm.IsLookupMeasure;
                        ((PivotTotal)value).LookupCubeName = expressionFrm.LookupCubeName;
                        ((PivotTotal)value).Filters.InnerXml = (expressionFrm.Filters != null) ? expressionFrm.Filters.InnerXml : null;
                        ((PivotTotal)value).Caption = expressionFrm.TotalCaption;
                        ((PivotTotal)value).Expression = expressionFrm.Expression;

                        ((PivotTotal) value).ParentPivotData.SetSelection(SelectionType.SingleObject,
                                                                          ((PivotTotal) value).UniqueName);
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
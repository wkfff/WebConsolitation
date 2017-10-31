using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Dundas.Maps.WinControl;
using Krista.FM.Client.MDXExpert.Controls;

namespace Krista.FM.Client.MDXExpert
{
    public class MapRangeEditor : UITypeEditor
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
                    int digitCount = 2;
                    double totalMinimum = Double.MaxValue;
                    double totalMaximum = Double.MinValue;
                    SerieRule serieRule = null;

                    if (context.Instance is SymbolRuleBrowseAdapter)
                    {
                        SymbolRuleBrowseAdapter symbolRule = (SymbolRuleBrowseAdapter)context.Instance;
                        serieRule = symbolRule.GetSerieRule();
                    }
                    else
                        if (context.Instance is ShapeRuleBrowseAdapter)
                        {
                            ShapeRuleBrowseAdapter shapeRule = (ShapeRuleBrowseAdapter)context.Instance;
                            serieRule = shapeRule.GetSerieRule();
                        }

                    if (serieRule != null)
                    {
                        digitCount = serieRule.DigitCount;
                        totalMinimum = MapHelper.GetMinimumRangeLimit(serieRule.Serie.Table, serieRule.Name);
                        totalMaximum = MapHelper.GetMaximumRangeLimit(serieRule.Serie.Table, serieRule.Name);
                    }

                    if (totalMinimum > totalMaximum)
                    {
                        totalMinimum = 0;
                        totalMaximum = 100;
                    }
                    RangeCollectionControl mrCtrl = new RangeCollectionControl((List<double>)value, totalMinimum, totalMaximum);
                    mrCtrl.DigitCount = digitCount;
                    mrCtrl.Tag = svc;

                    svc.DropDownControl(mrCtrl);

                    value = mrCtrl.RangeLimits;

                    if (context.Instance is SymbolRuleBrowseAdapter)
                    {
                        SymbolRuleBrowseAdapter symbolRule = (SymbolRuleBrowseAdapter)context.Instance;
                        symbolRule.GetSerieRule().DigitCount = mrCtrl.DigitCount;
                        symbolRule.GetSerieRule().RangeLimits = (List<double>)value;
                        symbolRule.RangeLimits = (List<double>)value;
                        symbolRule.GetSerieRule().Serie.Series.Element.MainForm.Saved = false;

                    }
                    else
                        if (context.Instance is ShapeRuleBrowseAdapter)
                        {
                            ShapeRuleBrowseAdapter shapeRule = (ShapeRuleBrowseAdapter)context.Instance;
                            shapeRule.GetSerieRule().DigitCount = mrCtrl.DigitCount;
                            shapeRule.GetSerieRule().RangeLimits = (List<double>)value;
                            shapeRule.RangeLimits = (List<double>)value;
                            shapeRule.GetSerieRule().Serie.Series.Element.MainForm.Saved = false;
                        }

                }
            }

            return base.EditValue(context, provider, value); 
            //return value; 
        }
        

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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Dundas.Maps.WinControl;
using Infragistics.UltraGauge.Resources;
using Krista.FM.Client.MDXExpert.Controls;

namespace Krista.FM.Client.MDXExpert
{
    public class GaugeRangeEditor : UITypeEditor
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

                    GaugeReportElement gaugeElement = null;


                    if (context.Instance is GaugeColorRangeBrowseClass)
                    {
                        GaugeColorRangeBrowseClass radialGaugeRangeBrowse = (GaugeColorRangeBrowseClass)context.Instance;
                        gaugeElement = radialGaugeRangeBrowse.GaugeElement;
                    }


                    totalMinimum = gaugeElement.StartValue;
                    totalMaximum = gaugeElement.EndValue;

                    if (totalMinimum > totalMaximum)
                    {
                        totalMinimum = 0;
                        totalMaximum = 100;
                    }

                    List<double> rangeLimits = new List<double>();
                    List<Color> colors = new List<Color>();
                    List<string> rangeTexts = new List<string>();

                    if (value is GaugeColorRangeCollection)
                    {
                        GaugeColorRangeCollection ranges = (GaugeColorRangeCollection)value;
                        rangeLimits = GetRangeLimits(ranges);
                        foreach (GaugeColorRange range in ranges)
                        {
                            colors.Add(range.Color);
                            rangeTexts.Add(range.Text);
                        }
                    }


                    RangeCollectionControl rcCtrl = new RangeCollectionControl(rangeLimits, colors, rangeTexts, totalMinimum, totalMaximum);



                    rcCtrl.DigitCount = digitCount;
                    rcCtrl.Tag = svc;

                    svc.DropDownControl(rcCtrl);

                    int i = 0;
                    if (value is GaugeColorRangeCollection)
                    {
                        GaugeColorRangeCollection ranges = (GaugeColorRangeCollection)value;
                        foreach (GaugeColorRange range in ranges)
                        {
                            range.StartValue = rcCtrl.RangeLimits[i];
                            range.EndValue = rcCtrl.RangeLimits[i + 1];
                            range.Color = rcCtrl.RangeColors[i];
                            range.Text = rcCtrl.RangeTexts[i];
                            i++;
                        }
                    }

                    gaugeElement.RefreshColorRanges();
                    gaugeElement.InitLegend();
                }
            }

            return base.EditValue(context, provider, value);

        }


        private List<double> GetRangeLimits(GaugeColorRangeCollection ranges)
        {
            List<double> result = new List<double>();

            if (ranges.Count > 0)
            {
                foreach (GaugeColorRange range in ranges)
                {
                    result.Add(range.StartValue);
                }
                result.Add(ranges[ranges.Count - 1].EndValue);
            }
            return result;
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

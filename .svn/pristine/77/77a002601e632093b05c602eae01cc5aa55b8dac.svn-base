using System;
using Infragistics.UltraGauge.Resources;
using Infragistics.UltraGauge.Resources.Editor;

namespace Krista.FM.Client.MDXExpert
{
    public class RadialGaugeRangeCollectionEditor : CustomGaugeCollectionEditorBase
    {

        public RadialGaugeRangeCollectionEditor()
        {
        }

        protected override string CollectionPropertyName
        {
            get { return "RangeColors"; }
        }

        protected override string[] TypeNames
        {
            get { return new string[] { "Интервал" }; }
        }

        protected override Type[] ItemTypes
        {
            get { return new Type[] {typeof (RadialGaugeRange)}; }
        }

        protected override string FormCaption
        {
            get { return "Интервалы цветовой шкалы индикатора"; }
        }
    }
}
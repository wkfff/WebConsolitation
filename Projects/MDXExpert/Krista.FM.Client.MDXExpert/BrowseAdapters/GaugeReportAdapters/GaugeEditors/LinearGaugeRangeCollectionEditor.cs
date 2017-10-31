using System;
using Infragistics.UltraGauge.Resources;
using Infragistics.UltraGauge.Resources.Editor;

namespace Krista.FM.Client.MDXExpert
{
    public class LinearGaugeRangeCollectionEditor : CustomGaugeCollectionEditorBase
    {

        public LinearGaugeRangeCollectionEditor()
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
            get { return new Type[] {typeof (LinearGaugeRange)}; }
        }

        protected override string FormCaption
        {
            get { return "Интервалы цветовой шкалы индикатора"; }
        }
    }
}
using System.ComponentModel;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomThreeDEffect : ThreeDEffect
    {
        /// <summary>
        /// Применить
        /// </summary>
        [Category("Свойства")]
        [Description("Применить")]
        [DisplayName("Применить")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public new bool Enabled
        {
            get { return base.Enabled; }
            set { base.Enabled = value; }
        }

        public CustomThreeDEffect(IChartComponent component)
            : base(component)
        {

        }

        public override string ToString()
        {
            return "Объем";
        }
    }
}
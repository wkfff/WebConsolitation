using System.ComponentModel;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomThreeDEffect : FilterablePropertyBase
    {
        private ThreeDEffect _threeDEffect;
        /// <summary>
        /// ���������
        /// </summary>
        [Category("��������")]
        [Description("���������")]
        [DisplayName("���������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Enabled
        {
            get { return this._threeDEffect.Enabled; }
            set { this._threeDEffect.Enabled = value; }
        }

        public CustomThreeDEffect(ThreeDEffect threeDEffect)
        {
            this._threeDEffect = threeDEffect;
        }

        public override string ToString()
        {
            return "�����";
        }
    }
}
using System.ComponentModel;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomGradientEffect : GradientEffect
    {
        /// <summary>
        /// �����
        /// </summary>
        [Category("��������")]
        [Description("�����")]
        [DisplayName("�����")]
        [Browsable(true)]
        public new GradientColoringStyle Coloring
        {
            get { return base.Coloring; }
            set { base.Coloring = value; }
        }

        /// <summary>
        /// �����
        /// </summary>
        [Category("��������")]
        [Description("�����")]
        [DisplayName("�����")]
        [Browsable(true)]
        public new GradientStyle Style
        {
            get { return base.Style; }
            set { base.Style = value; }
        }

        /// <summary>
        /// ���������
        /// </summary>
        [Category("��������")]
        [Description("���������")]
        [DisplayName("���������")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public new bool Enabled
        {
            get { return base.Enabled; }
            set { base.Enabled = value; }
        }

        public CustomGradientEffect(IChartComponent component)
            : base(component)
        {

        }

        public override string ToString()
        {
            return "��������";
        }
    }
}
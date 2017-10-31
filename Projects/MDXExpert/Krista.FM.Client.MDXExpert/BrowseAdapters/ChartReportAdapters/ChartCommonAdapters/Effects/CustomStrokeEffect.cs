using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    public class CustomStrokeEffect : FilterablePropertyBase
    {
        private StrokeEffect _strokeEffect;
        /// <summary>
        /// ������������ ������
        /// </summary>
        [Category("��������")]
        [Description("������������ ������")]
        [DisplayName("������������")]
        [Browsable(true)]
        public byte StrokeOpacity
        {
            get { return this._strokeEffect.StrokeOpacity; }
            set { this._strokeEffect.StrokeOpacity = value; }
        }

        /// <summary>
        /// ������ ������
        /// </summary>
        [Category("��������")]
        [Description("������ ������")]
        [DisplayName("������")]
        [Browsable(true)]
        public int StrokeWidth
        {
            get { return this._strokeEffect.StrokeWidth; }
            set { this._strokeEffect.StrokeWidth = value; }
        }

        /// <summary>
        /// ���� ������
        /// </summary>
        [Category("��������")]
        [Description("���� ������")]
        [DisplayName("����")]
        [Browsable(true)]
        public Color StrokeColor
        {
            get { return this._strokeEffect.StrokeColor; }
            set { this._strokeEffect.StrokeColor = value; }
        }

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
            get { return this._strokeEffect.Enabled; }
            set { this._strokeEffect.Enabled = value; }
        }

        public CustomStrokeEffect(StrokeEffect strokeEffect)
        {
            this._strokeEffect = strokeEffect;
        }

        public override string ToString()
        {
            return "���������";
        }
    }
}
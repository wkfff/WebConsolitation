using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MarginsBrowseClass
    {
        #region ����

        private MarginsAppearance margins;

        #endregion

        #region ��������

        /// <summary>
        /// ����� ����
        /// </summary>
        [Description("����� ����")]
        [DisplayName("�����")]
        [DefaultValue(5)]
        [Browsable(true)]
        public int Left
        {
            get { return margins.Left; }
            set { margins.Left = value; }
        }

        /// <summary>
        /// ������ ����
        /// </summary>
        [Description("������ ����")]
        [DisplayName("������")]
        [DefaultValue(5)]
        [Browsable(true)]
        public int Right
        {
            get { return margins.Right; }
            set { margins.Right = value; }
        }

        /// <summary>
        /// ������� ����
        /// </summary>
        [Description("������� ����")]
        [DisplayName("�������")]
        [DefaultValue(5)]
        [Browsable(true)]
        public int Top
        {
            get { return margins.Top; }
            set { margins.Top = value; }
        }

        /// <summary>
        /// ������ ����
        /// </summary>
        [Description("������ ����")]
        [DisplayName("������")]
        [DefaultValue(5)]
        [Browsable(true)]
        public int Bottom
        {
            get { return margins.Bottom; }
            set { margins.Bottom = value; }
        }

        #endregion

        public MarginsBrowseClass(MarginsAppearance margins)
        {
            this.margins = margins;
        }

        public override string ToString()
        {
            return Top + "; " + Left + "; " + Bottom + "; " + Right;
        }
    }
}
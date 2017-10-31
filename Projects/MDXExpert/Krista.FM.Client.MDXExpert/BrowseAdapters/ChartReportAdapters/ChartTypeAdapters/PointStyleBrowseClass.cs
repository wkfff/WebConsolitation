using System;
using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PointStyleBrowseClass
    {
        #region ����

        private PointStyle pointStyle;

        #endregion

        #region ��������

        /// <summary>
        /// ������ �����
        /// </summary>
        [Description("������ �����")]
        [DisplayName("������ �����")]
        [DefaultValue("A")]
        [Browsable(true)]
        public Char Character
        {
            get { return pointStyle.Character; }
            set { pointStyle.Character = value; }
        }

        /// <summary>
        /// ����� �������
        /// </summary>
        [Description("����� �������")]
        [DisplayName("����� �������")]
        [TypeConverter(typeof(FontTypeConverter))]
        [DefaultValue(typeof(Font), "Microsoft Sans Serif, 7.8pt")]
        [Browsable(true)]
        public Font CharacterFont
        {
            get { return pointStyle.CharacterFont; }
            set { pointStyle.CharacterFont = value; }
        }

        /// <summary>
        /// ��� ������
        /// </summary>
        [Description("��� ������")]
        [DisplayName("������")]
        [TypeConverter(typeof(SymbolIconTypeConverter))]
        [DefaultValue(SymbolIcon.X)]
        [Browsable(true)]
        public SymbolIcon Icon
        {
            get { return pointStyle.Icon; }
            set { pointStyle.Icon = value; }
        }

        /// <summary>
        /// ������ ������
        /// </summary>
        [Description("������ ������")]
        [DisplayName("������ ������")]
        [TypeConverter(typeof(SymbolIconSizeTypeConverter))]
        [DefaultValue(SymbolIconSize.Medium)]
        [Browsable(true)]
        public SymbolIconSize IconSize
        {
            get { return pointStyle.IconSize; }
            set { pointStyle.IconSize = value; }
        }

        #endregion

        public PointStyleBrowseClass(PointStyle pointStyle)
        {
            this.pointStyle = pointStyle;
        }

        public override string ToString()
        {
            return SymbolIconTypeConverter.ToString(Icon) + "; " + SymbolIconSizeTypeConverter.ToString(IconSize);
        }
    }
}
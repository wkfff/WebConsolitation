using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Editor;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// ����� ����� � ��������� PolarChart
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SplineApperanceBrowseClass
    {
        #region ����

        private SplineAppearance splineAppearance;

        #endregion

        #region ��������

        /// <summary>
        /// ����� �����
        /// </summary>
        [Category("�����")]
        [Description("����� �����")]
        [DisplayName("�����")]
        [Editor(typeof(LineStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(LineDrawStyleTypeConverter))]
        [Browsable(true)]
        public LineDrawStyle DrawStyle
        {
            get { return splineAppearance.DrawStyle; }
            set { splineAppearance.DrawStyle = value; }
        }

        /// <summary>
        /// ������� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("������� �����")]
        [DisplayName("�������")]
        [Editor(typeof(LineThicknessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        public int Thickness
        {
            get { return splineAppearance.Thickness; }
            set { splineAppearance.Thickness = value; }
        }

        /// <summary>
        /// ��������� �����
        /// </summary>
        [Category("������� ���������")]
        [Description("��������� �����")]
        [DisplayName("���������")]
        [Browsable(true)]
        public float SplineTension
        {
            get { return splineAppearance.SplineTension; }
            set { splineAppearance.SplineTension = value; }
        }

        #endregion

        public SplineApperanceBrowseClass(SplineAppearance splineAppearance)
        {
            this.splineAppearance = splineAppearance;
        }

        public override string ToString()
        {
            return LineDrawStyleTypeConverter.ToString(DrawStyle) + "; " + Thickness + "; " + SplineTension;
        }
    }
}
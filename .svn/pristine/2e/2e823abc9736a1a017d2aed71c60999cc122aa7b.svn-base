using System.ComponentModel;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Настройка 3D вида
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Transform3DBrowseClass
    {
        #region Поля

        private View3DAppearance view3DAppearance;

        #endregion

        #region Свойства

        /// <summary>
        /// Подсветка
        /// </summary>
        [Category("Настройка 3D вида")]
        [Description("Подсветка")]
        [DisplayName("Подсветка")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Light
        {
            get { return view3DAppearance.Light; }
            set { view3DAppearance.Light = value; }
        }

        /// <summary>
        /// Контур
        /// </summary>
        [Category("Настройка 3D вида")]
        [Description("Контур")]
        [DisplayName("Контур")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool OutlLine
        {
            get { return view3DAppearance.Outline; }
            set { view3DAppearance.Outline = value; }
        }

        /// <summary>
        /// Процент перспективы
        /// </summary>
        [Category("Настройка 3D вида")]
        [Description("Процент перспективы")]
        [DisplayName("Процент перспективы")]
        [Browsable(true)]
        public float Perspective
        {
            get { return view3DAppearance.Perspective; }
            set { view3DAppearance.Perspective = value; }
        }

        /// <summary>
        /// Процент шкалы
        /// </summary>
        [Category("Настройка 3D вида")]
        [Description("Процент шкалы")]
        [DisplayName("Процент шкалы")]
        [Browsable(true)]
        public float Scale
        {
            get { return view3DAppearance.Scale; }
            set { view3DAppearance.Scale = value; }
        }

        /// <summary>
        /// Угол поворота по оси X
        /// </summary>
        [Category("Настройка 3D вида")]
        [Description("Угол поворота по оси X")]
        [DisplayName("Угол поворота по оси X")]
        [Browsable(true)]
        public float XRotation
        {
            get { return view3DAppearance.XRotation; }
            set { view3DAppearance.XRotation = value; }
        }

        /// <summary>
        /// Угол поворота по оси Y
        /// </summary>
        [Category("Настройка 3D вида")]
        [Description("Угол поворота по оси Y")]
        [DisplayName("Угол поворота по оси Y")]
        [Browsable(true)]
        public float YRotation
        {
            get { return view3DAppearance.YRotation; }
            set { view3DAppearance.YRotation = value; }
        }

        /// <summary>
        /// Угол поворота по оси Z
        /// </summary>
        [Category("Настройка 3D вида")]
        [Description("Угол поворота по оси Z")]
        [DisplayName("Угол поворота по оси Z")]
        [Browsable(true)]
        public float ZRotation
        {
            get { return view3DAppearance.ZRotation; }
            set { view3DAppearance.ZRotation = value; }
        }

        /// <summary>
        /// Срез углов
        /// </summary>
        [Category("Настройка 3D вида")]
        [Description("Срез углов")]
        [DisplayName("Срез углов")]
        [Browsable(true)]
        public float EdgeSize
        {
            get { return view3DAppearance.EdgeSize; }
            set { view3DAppearance.EdgeSize = value; }
        }

        #endregion

        public Transform3DBrowseClass(View3DAppearance view3DAppearance)
        {
            this.view3DAppearance = view3DAppearance;
        }

        public override string ToString()
        {
            return "";// Location + "; " + BackColor.Name + "; " + Format;
        }
    }
}
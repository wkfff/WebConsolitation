using System.ComponentModel;
using System.Drawing.Design;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Resources.Effects;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class EffectsBrowseClass
    {
        #region ѕол€

        private EffectsAppearance effectAppearance;

        #endregion

        #region —войства

        /// <summary>
        /// Ёффекты диаграммы
        /// </summary>
        [Description("Ёффекты диаграммы")]
        [DisplayName("Ёффекты диаграммы")]
        [Editor(typeof(CustomEffectsCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public EffectsCollection Effects
        {
            get { return effectAppearance.Effects; }
        }

        /// <summary>
        /// ќтображение эффектов
        /// </summary>
        [Description("ќтображение эффектов")]
        [DisplayName("ќтображение эффектов")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Enabled
        {
            get { return effectAppearance.Enabled; }
            set { effectAppearance.Enabled = value; }
        }

        #endregion

        public EffectsBrowseClass(EffectsAppearance effectAppearance)
        {
            this.effectAppearance = effectAppearance;
        }

        public override string ToString()
        {
            return BooleanTypeConverter.ToString(Enabled);
        }
    }
}
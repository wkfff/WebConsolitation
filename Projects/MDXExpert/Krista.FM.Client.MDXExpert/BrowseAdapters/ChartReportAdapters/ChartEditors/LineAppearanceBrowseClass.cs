using System.ComponentModel;
using System.Drawing;
using Infragistics.UltraChart.Core.ColorModel;
using Infragistics.UltraChart.Core.Layers;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Data.Series;
using Infragistics.Win.UltraWinChart;

namespace Krista.FM.Client.MDXExpert
{
    public class LineAppearanceBrowseClass : FilterablePropertyBase
    {
        private LineAppearance _lineAppearance;
        private LineStyleBrowseClass _lineStyleBrowse;
        private IconAppearanceBrowseClass _iconAppearanceBrowse;

        [Category("Свойства")]
        [Description("Вид иконки")]
        [DisplayName("Вид иконки")]
        [Browsable(true)]
        public IconAppearanceBrowseClass IconAppearance
        {
            get
            {
                return this._iconAppearanceBrowse;
            }
        }

        [Category("Свойства")]
        [Description("Стиль линии")]
        [DisplayName("Стиль линии")]
        [Browsable(true)]
        public LineStyleBrowseClass LineStyle
        {
            get { return this._lineStyleBrowse; }
            set { this._lineStyleBrowse = value; }
        }


        [Category("Свойства")]
        [Description("Натяжение сплайна")]
        [DisplayName("Натяжение сплайна")]
        [Browsable(true)]
        public float SplineTension
        {
            get { return this._lineAppearance.SplineTension; }
            set { this._lineAppearance.SplineTension = value; }
        }

        [Category("Свойства")]
        [Description("Толщина")]
        [DisplayName("Толщина")]
        [Browsable(true)]
        public int Thickness
        {
            get { return this._lineAppearance.Thickness; }
            set { this._lineAppearance.Thickness = value; }
        }
       
        public LineAppearanceBrowseClass(LineAppearance lineAppearance)
        {
            this._lineAppearance = lineAppearance;
            this._lineStyleBrowse = new LineStyleBrowseClass(lineAppearance.LineStyle);
            this._iconAppearanceBrowse = new IconAppearanceBrowseClass(lineAppearance.IconAppearance);
        }

        public override string ToString()
        {
            return "";
        }
    }
}
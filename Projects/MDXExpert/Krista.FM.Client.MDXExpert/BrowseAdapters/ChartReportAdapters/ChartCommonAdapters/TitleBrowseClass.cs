using System.ComponentModel;
using Infragistics.Win.UltraWinChart;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Надписи
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TitleBrowseClass
    {
        #region Поля

        private TitleSideBrowseClass titleLeftBrowse;
        private TitleSideBrowseClass titleRightBrowse;
        private TitleSideBrowseClass titleTopBrowse;
        private TitleSideBrowseClass titleBottomBrowse;

        #endregion

        #region Свойства

        /// <summary>
        /// Надпись по левому краю
        /// </summary>
        [Category("Надписи")]
        [Description("Надпись по левому краю")]
        [DisplayName("Надпись по левому краю")]
        [Browsable(true)]
        public TitleSideBrowseClass TitleLeftBrowse
        {
            get { return titleLeftBrowse; }
            set { titleLeftBrowse = value; }
        }

        /// <summary>
        /// Надпись по правому краю
        /// </summary>
        [Category("Надписи")]
        [Description("Надпись по правому краю")]
        [DisplayName("Надпись по правому краю")]
        [Browsable(true)]
        public TitleSideBrowseClass TitleRightBrowse
        {
            get { return titleRightBrowse; }
            set { titleRightBrowse = value; }
        }

        /// <summary>
        /// Надпись по верхнему краю
        /// </summary>
        [Category("Надписи")]
        [Description("Надпись по верхнему краю")]
        [DisplayName("Надпись по верхнему краю")]
        [Browsable(true)]
        public TitleSideBrowseClass TitleTopBrowse
        {
            get { return titleTopBrowse; }
            set { titleTopBrowse = value; }
        }

        /// <summary>
        /// Надпись по нижнему краю
        /// </summary>
        [Category("Надписи")]
        [Description("Надпись по нижнему краю")]
        [DisplayName("Надпись по нижнему краю")]
        [Browsable(true)]
        public TitleSideBrowseClass TitleBottomBrowse
        {
            get { return titleBottomBrowse; }
            set { titleBottomBrowse = value; }
        }

        #endregion

        public TitleBrowseClass(UltraChart ultraChart)
        {
            titleLeftBrowse = new TitleSideBrowseClass(ultraChart.TitleLeft);
            titleRightBrowse = new TitleSideBrowseClass(ultraChart.TitleRight);
            titleTopBrowse = new TitleSideBrowseClass(ultraChart.TitleTop);
            titleBottomBrowse = new TitleSideBrowseClass(ultraChart.TitleBottom);
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
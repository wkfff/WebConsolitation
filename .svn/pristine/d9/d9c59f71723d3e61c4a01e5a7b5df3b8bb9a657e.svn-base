using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Dundas.Maps.WinControl;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DistanceScalePanelBrowseAdapter : MapPanelBrowseAdapterBase
    {
        #region Поля

        private DistanceScalePanel distanceScalePanel;

        #endregion

        #region Свойства
        
        [Description("Шрифт")]
        [DisplayName("Шрифт")]
        [TypeConverter(typeof(FontTypeConverter))]
        [Browsable(true)]
        public Font Font
        {
            get { return distanceScalePanel.Font; }
            set { distanceScalePanel.Font = value; }
        }

        [Description("Цвет метки")]
        [DisplayName("Цвет метки")]
        [Browsable(true)]
        public Color LabelColor
        {
            get { return distanceScalePanel.LabelColor; }
            set { distanceScalePanel.LabelColor = value; }
        }

        [Description("Цвет бордюра шкалы")]
        [DisplayName("Цвет бордюра шкалы")]
        [Browsable(true)]
        public Color ScaleBorderColor
        {
            get { return distanceScalePanel.ScaleBorderColor; }
            set { distanceScalePanel.ScaleBorderColor = value; }
        }

        [Description("Цвет шкалы")]
        [DisplayName("Цвет шкалы")]
        [Browsable(true)]
        public Color ScaleForeColor
        {
            get { return distanceScalePanel.ScaleForeColor; }
            set { distanceScalePanel.ScaleForeColor = value; }
        }
        
        
        #endregion

        public DistanceScalePanelBrowseAdapter(DistanceScalePanel distanceScalePanel) : base(distanceScalePanel)
        {
            this.distanceScalePanel = distanceScalePanel;
        }

        public override string ToString()
        {
            return "";
        }


    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Dundas.Maps.WinControl;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MapPanelAppearanceBrowseAdapter
    {
        #region Поля

        private Panel mapPanel;

        #endregion

        #region Свойства


        [Description("Начальный цвет фона")]
        [DisplayName("Начальный цвет")]
        [Browsable(true)]
        public Color BackColor
        {
            get { return mapPanel.BackColor; }
            set { mapPanel.BackColor = value; }
        }

        [Description("Тип градиента фона")]
        [DisplayName("Тип градиента")]
        [TypeConverter(typeof(GradientTypeConverter))]
        [Browsable(true)]
        public GradientType BackGradientType
        {
            get { return mapPanel.BackGradientType; }
            set { mapPanel.BackGradientType = value; }
        }

        [Description("Фоновый узор")]
        [DisplayName("Фоновый узор")]
        [TypeConverter(typeof(MapHatchStyleConverter))]
        [Browsable(true)]
        public MapHatchStyle BackHatchStyle
        {
            get { return mapPanel.BackHatchStyle; }
            set { mapPanel.BackHatchStyle = value; }
        }

        [Description("Конечный цвет фона")]
        [DisplayName("Конечный цвет")]
        [Browsable(true)]
        public Color BackSecondaryColor
        {
            get { return mapPanel.BackSecondaryColor; }
            set { mapPanel.BackSecondaryColor = value; }
        }

        [Description("Смещение тени фона")]
        [DisplayName("Смещение тени")]
        [Browsable(true)]
        public int BackShadowOffset
        {
            get { return mapPanel.BackShadowOffset; }
            set { mapPanel.BackShadowOffset = value; }
        }

        [Description("Цвет бордюра")]
        [DisplayName("Цвет бордюра")]
        [Browsable(true)]
        public Color BorderColor
        {
            get { return mapPanel.BorderColor; }
            set { mapPanel.BorderColor = value; }
        }

        [Description("Стиль бордюра")]
        [DisplayName("Стиль бордюра")]
        [TypeConverter(typeof(MapDashStyleConverter))]
        [Browsable(true)]
        public MapDashStyle BorderStyle
        {
            get { return mapPanel.BorderStyle; }
            set { mapPanel.BorderStyle = value; }
        }

        [Description("Толщина бордюра")]
        [DisplayName("Толщина бордюра")]
        [Browsable(true)]
        public int BorderWidth
        {
            get { return mapPanel.BorderWidth; }
            set { mapPanel.BorderWidth = value; }
        }
        /*
        [Description("Расположение панели")]
        [DisplayName("Расположение")]
        [Browsable(true)]
        public MapLocation Location
        {
            get { return mapPanel.Location; }
            set { mapPanel.Location = value; }
        }

        [Description("Единицы расположения")]
        [DisplayName("Единицы расположения")]
        [Browsable(true)]
        public CoordinateUnit LocationUnit
        {
            get { return mapPanel.LocationUnit; }
            set { mapPanel.LocationUnit = value; }
        }

        [Description("Границы панели")]
        [DisplayName("Границы")]
        [Browsable(true)]
        public PanelMargins Margins
        {
            get { return mapPanel.Margins; }
            set { mapPanel.Margins = value; }
        }

        [Description("Размер панели")]
        [DisplayName("Размер")]
        [Browsable(true)]
        public MapSize Size
        {
            get { return mapPanel.Size; }
            set { mapPanel.Size = value; }
        }

        [Description("Единицы размера")]
        [DisplayName("Единицы размера")]
        [Browsable(true)]
        public CoordinateUnit SizeUnit
        {
            get { return mapPanel.SizeUnit; }
            set { mapPanel.SizeUnit = value; }
        }

        */


        #endregion

        public MapPanelAppearanceBrowseAdapter(Panel mapPanel)
        {
            this.mapPanel = mapPanel;
        }

        public override string ToString()
        {
            return "";
        }


    }
}

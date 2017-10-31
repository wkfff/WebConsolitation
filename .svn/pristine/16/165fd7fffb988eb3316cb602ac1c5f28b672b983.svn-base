using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Dundas.Maps.WinControl;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ZoomPanelBrowseAdapter : MapPanelBrowseAdapterBase
    {
        #region Поля

        private ZoomPanel zoomPanel;

        #endregion

        #region Свойства

        [Description("Тип увеличения")]
        [DisplayName("Тип увеличения")]
        [TypeConverter(typeof(ZoomTypeConverter))]
        [Browsable(true)]
        public ZoomType ZoomType
        {
            get { return zoomPanel.ZoomType; }
            set { zoomPanel.ZoomType = value; }
        }

        [Description("Цвет бордюра кнопок")]
        [DisplayName("Цвет бордюра кнопок")]
        [Browsable(true)]
        public Color ButtonBorderColor
        {
            get { return zoomPanel.ButtonBorderColor; }
            set { zoomPanel.ButtonBorderColor = value; }
        }

        [Description("Цвет кнопок")]
        [DisplayName("Цвет кнопок")]
        [Browsable(true)]
        public Color ButtonColor
        {
            get { return zoomPanel.ButtonColor; }
            set { zoomPanel.ButtonColor = value; }
        }

        [Description("Ориентация")]
        [DisplayName("Ориентация")]
        [TypeConverter(typeof(MapOrientationTypeConverter))]
        [Browsable(true)]
        public Orientation Orientation
        {
            get { return zoomPanel.Orientation; }
            set { zoomPanel.Orientation = value; }
        }

        [Description("Вид")]
        [DisplayName("Вид")]
        [TypeConverter(typeof(ZoomPanelStyleConverter))]
        [Browsable(true)]
        public ZoomPanelStyle PanelStyle
        {
            get { return zoomPanel.PanelStyle; }
            set { zoomPanel.PanelStyle = value; }
        }

        [Description("Реверс увеличения")]
        [DisplayName("Реверс увеличения")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool Reversed
        {
            get { return zoomPanel.Reversed; }
            set { zoomPanel.Reversed = value; }
        }

        [Description("Цвет бордюра шкалы")]
        [DisplayName("Цвет бордюра шкалы")]
        [Browsable(true)]
        public Color SliderBarBorderColor
        {
            get { return zoomPanel.SliderBarBorderColor; }
            set { zoomPanel.SliderBarBorderColor = value; }
        }

        [Description("Цвет шкалы")]
        [DisplayName("Цвет шкалы")]
        [Browsable(true)]
        public Color SliderBarColor
        {
            get { return zoomPanel.SliderBarColor; }
            set { zoomPanel.SliderBarColor = value; }
        }

        [Description("Привязка к отметкам")]
        [DisplayName("Привязка к отметкам")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool SnapToTickMarks
        {
            get { return zoomPanel.SnapToTickMarks; }
            set { zoomPanel.SnapToTickMarks = value; }
        }

        [Description("Цвет бордюра значков")]
        [DisplayName("Цвет бордюра значков")]
        [Browsable(true)]
        public Color SymbolBorderColor
        {
            get { return zoomPanel.SymbolBorderColor; }
            set { zoomPanel.SymbolBorderColor = value; }
        }

        [Description("Цвет значков")]
        [DisplayName("Цвет значков")]
        [Browsable(true)]
        public Color SymbolColor
        {
            get { return zoomPanel.SymbolColor; }
            set { zoomPanel.SymbolColor = value; }
        }

        [Description("Цвет бордюра бегунка")]
        [DisplayName("Цвет бордюра бегунка")]
        [Browsable(true)]
        public Color ThumbBorderColor
        {
            get { return zoomPanel.ThumbBorderColor; }
            set { zoomPanel.ThumbBorderColor = value; }
        }

        [Description("Цвет бегунка")]
        [DisplayName("Цвет бегунка")]
        [Browsable(true)]
        public Color ThumbColor
        {
            get { return zoomPanel.ThumbColor; }
            set { zoomPanel.ThumbColor = value; }
        }

        [Description("Цвет бордюра отметок")]
        [DisplayName("Цвет бордюра отметок")]
        [Browsable(true)]
        public Color TickBorderColor
        {
            get { return zoomPanel.TickBorderColor; }
            set { zoomPanel.TickBorderColor = value; }
        }

        [Description("Цвет отметок")]
        [DisplayName("Цвет отметок")]
        [Browsable(true)]
        public Color TickColor
        {
            get { return zoomPanel.TickColor; }
            set { zoomPanel.TickColor = value; }
        }

        [Description("Количество отметок")]
        [DisplayName("Количество отметок")]
        [Browsable(true)]
        public int TickCount
        {
            get { return zoomPanel.TickCount; }
            set { zoomPanel.TickCount = value; }
        }

        [Description("Показывать кнопки")]
        [DisplayName("Показывать кнопки")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool ZoomButtonsVisible
        {
            get { return zoomPanel.ZoomButtonsVisible; }
            set { zoomPanel.ZoomButtonsVisible = value; }
        }



        #endregion

        public ZoomPanelBrowseAdapter(ZoomPanel zoomPanel) : base(zoomPanel) 
        {
            this.zoomPanel = zoomPanel;
        }

        public override string ToString()
        {
            return "";
        }


    }
}

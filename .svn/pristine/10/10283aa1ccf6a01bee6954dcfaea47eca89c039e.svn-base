using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Dundas.Maps.WinControl;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert
{
    public class MapSymbolBrowseAdapter
    {
        #region Поля

        private PredefinedSymbol symbol;
        private SymbolTextAppearance textAppearance;

        #endregion

        #region Свойства
        
        
        [Description("Цвет бордюра")]
        [DisplayName("Цвет бордюра")]
        [Browsable(true)]
        public Color BorderColor
        {
            get { return symbol.BorderColor; }
            set { symbol.BorderColor = value; }
        }
        
        [Description("Вид бордюра")]
        [DisplayName("Вид бордюра")]
        [TypeConverter(typeof(MapDashStyleConverter))]
        [Browsable(true)]
        public MapDashStyle BorderStyle
        {
            get { return symbol.BorderStyle; }
            set { symbol.BorderStyle = value; }
        }
        
        [Description("Толщина бордюра")]
        [DisplayName("Толщина бордюра")]
        [Browsable(true)]
        public int BorderWidth
        {
            get { return symbol.BorderWidth; }
            set { symbol.BorderWidth = value; }
        }
        
        [Description("Начальный цвет")]
        [DisplayName("Начальный цвет")]
        [Browsable(true)]
        public Color Color
        {
            get { return symbol.Color; }
            set { symbol.Color = value; }
        }
        
        [Description("Шрифт")]
        [DisplayName("Шрифт")]
        [TypeConverter(typeof(FontTypeConverter))]
        [Browsable(true)]
        public Font Font
        {
            get { return symbol.Font; }
            set { symbol.Font = value; }
        }
        
        [Description("Градиент")]
        [DisplayName("Градиент")]
        [TypeConverter(typeof(GradientTypeConverter))]
        [Browsable(true)]
        public GradientType GradientType
        {
            get { return symbol.GradientType; }
            set { symbol.GradientType = value; }
        }
        
        [Description("Узор")]
        [DisplayName("Узор")]
        [TypeConverter(typeof(MapHatchStyleConverter))]
        [Browsable(true)]
        public MapHatchStyle HatchStyle
        {
            get { return symbol.HatchStyle; }
            set { symbol.HatchStyle = value; }
        }
        
        [Description("Обозначение")]
        [DisplayName("Обозначение")]
        [TypeConverter(typeof(MarkerTypeConverter))]
        [Browsable(true)]
        public MarkerStyle MarkerStyle
        {
            get { return symbol.MarkerStyle; }
            set { symbol.MarkerStyle = value; }
        }
        
        [Description("Конечный цвет")]
        [DisplayName("Конечный цвет")]
        [Browsable(true)]
        public Color SecondaryColor
        {
            get { return symbol.SecondaryColor; }
            set { symbol.SecondaryColor = value; }
        }
        
        [Description("Смещение тени")]
        [DisplayName("Смещение тени")]
        [Browsable(true)]
        public int ShadowOffset
        {
            get { return symbol.ShadowOffset; }
            set { symbol.ShadowOffset = value; }
        }
        
        [Description("Подсказки")]
        [DisplayName("Подсказки")]
        [Browsable(true)]
        public string ToolTip
        {
            get { return symbol.ToolTip; }
            set { symbol.ToolTip = value; }
        }
        
        [Description("Подписи")]
        [DisplayName("Подписи")]
        [Browsable(true)]
        public SymbolTextAppearance TextAppearance
        {
            get { return textAppearance; }
            set { textAppearance = value; }
        }
        

        #endregion

        public MapSymbolBrowseAdapter(PredefinedSymbol symbol)
        {
            this.symbol = symbol;
            textAppearance = new SymbolTextAppearance(symbol);
        }

        public override string ToString()
        {
            return "";
        }


    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SymbolTextAppearance
    {
        #region Поля

        private PredefinedSymbol symbol;

        #endregion

        #region Свойства

        [Description("Расположение подписи")]
        [DisplayName("Расположение подписи")]
        [TypeConverter(typeof(TextAlignmentConverter))]
        [Browsable(true)]
        public TextAlignment Alignment
        {
            get { return symbol.TextAlignment; }
            set { symbol.TextAlignment = value; }
        }

        [Description("Текст подписи")]
        [DisplayName("Текст")]
        [Browsable(true)]
        public string Text
        {
            get { return symbol.Text; }
            set { symbol.Text = value; }
        }

        [Description("Цвет подписи")]
        [DisplayName("Цвет")]
        [Browsable(true)]
        public Color Color
        {
            get { return symbol.TextColor; }
            set { symbol.TextColor = value; }
        }

        [Description("Смещение тени подписи")]
        [DisplayName("Смещение тени")]
        [Browsable(true)]
        public int ShadowOffset
        {
            get { return symbol.TextShadowOffset; }
            set { symbol.TextShadowOffset = value; }
        }


        #endregion


        public SymbolTextAppearance(PredefinedSymbol symbol)
        {
            this.symbol = symbol;
        }

        public override string ToString()
        {
            return "";
        }
    }

}

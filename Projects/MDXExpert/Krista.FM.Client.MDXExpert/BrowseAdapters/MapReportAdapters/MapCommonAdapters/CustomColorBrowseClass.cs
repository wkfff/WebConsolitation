using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Dundas.Maps.WinControl;
using System.Drawing;
using System.Drawing.Design;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CustomColorBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private CustomColor customColor;

        #endregion

        #region Свойства
        
        [Description("Цвет")]
        [DisplayName("Цвет")]
        [Browsable(true)]
        public Color Color
        {
            get { return customColor.Color; }
            set { customColor.Color = value; }
        }
        
        #endregion

        public CustomColorBrowseClass(CustomColor customColor)
        {
            this.customColor = customColor;
        }


        public override string ToString()
        {
            return "";
        }

    }


}

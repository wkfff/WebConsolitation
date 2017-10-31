using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Dundas.Maps.WinControl;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class NavigationPanelBrowseAdapter : MapPanelBrowseAdapterBase
    {
        #region Поля

        private NavigationPanel navigationPanel;

        #endregion

        #region Свойства

        
        [Description("Цвет бордюра кнопок")]
        [DisplayName("Цвет бордюра кнопок")]
        [Browsable(true)]
        public Color ButtonBorderColor
        {
            get { return navigationPanel.ButtonBorderColor; }
            set { navigationPanel.ButtonBorderColor = value; }
        }

        [Description("Цвет кнопок")]
        [DisplayName("Цвет кнопок")]
        [Browsable(true)]
        public Color ButtonColor
        {
            get { return navigationPanel.ButtonColor; }
            set { navigationPanel.ButtonColor = value; }
        }
        
        [Description("Вид")]
        [DisplayName("Вид")]
        [Browsable(true)]
        public NavigationPanelStyle PanelStyle
        {
            get { return navigationPanel.PanelStyle; }
            set { navigationPanel.PanelStyle = value; }
        }
        
        [Description("Шаг скроллирования")]
        [DisplayName("Шаг скроллирования")]
        [Browsable(true)]
        public double ScrollStep
        {
            get { return navigationPanel.ScrollStep; }
            set { navigationPanel.ScrollStep = value; }
        }
        
        [Description("Цвет бордюра значка")]
        [DisplayName("Цвет бордюра значка")]
        [Browsable(true)]
        public Color SymbolBorderColor
        {
            get { return navigationPanel.SymbolBorderColor; }
            set { navigationPanel.SymbolBorderColor = value; }
        }

        [Description("Цвет значка")]
        [DisplayName("Цвет значка")]
        [Browsable(true)]
        public Color SymbolColor
        {
            get { return navigationPanel.SymbolColor; }
            set { navigationPanel.SymbolColor = value; }
        }
        

        #endregion

        public NavigationPanelBrowseAdapter(NavigationPanel navigationPanel) : base(navigationPanel)
        {
            this.navigationPanel = navigationPanel;
        }

        public override string ToString()
        {
            return "";
        }


    }
}

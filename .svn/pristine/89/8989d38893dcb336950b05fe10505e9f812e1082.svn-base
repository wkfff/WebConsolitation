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
    public class SymbolIntervalBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private PredefinedSymbol symbol;
        private SymbolRule rule;

        #endregion

        #region Свойства
        
        [Description("Наименование")]
        [DisplayName("Наименование")]
        [Browsable(true)]
        public string Name
        {
            get { return symbol.LegendText; }
            set { symbol.LegendText = value; }
        }

        [Description("Значения интервала")]
        [DisplayName("Значения")]
        [Browsable(true)]
        public string Values
        {
            get { return GetValues(); }
        }


        #endregion

        public SymbolIntervalBrowseClass(PredefinedSymbol symbol)
        {
            this.symbol = symbol;
            this.rule = (SymbolRule) this.symbol.ParentElement;
        }

        /// <summary>
        /// Получаем значения интервала. Сейчас можно получить только через легенду.
        /// </summary>
        /// <returns></returns>
        private string GetValues()
        {
            NamedElement legend = ((MapCore) this.rule.ParentElement).Legends.GetByName(this.rule.ShowInLegend);
            if (legend != null)
            {
                int rangeIndex = this.rule.PredefinedSymbols.IndexOf(this.symbol);
                return ((Legend) legend).Items[rangeIndex].Text;
            }
            return String.Empty;
        }


        public override string ToString()
        {
            return "";
        }

    }


}

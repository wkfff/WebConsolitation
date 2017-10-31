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
    public class ColorIntervalBrowseClass : FilterablePropertyBase
    {
        #region Поля

        private CustomColor customColor;
        private ShapeRule rule;
        private string values;
        private MapReportElement mapElement;

        #endregion

        #region Свойства
        
        [Description("Наименование")]
        [DisplayName("Наименование")]
        [Browsable(true)]
        public string Name
        {
            get { return GetIntervalName(); }
            set { SetIntervalName(value); }
        }

        [Description("Значения интервала")]
        [DisplayName("Значения")]
        [Browsable(true)]
        public string Values
        {
            get { return this.values; }
        }


        #endregion

        public ColorIntervalBrowseClass(CustomColor customColor, MapReportElement mapElement)
        {
            this.customColor = customColor;
            this.rule = (ShapeRule)this.customColor.ParentElement;
            this.values = GetValues();
            this.mapElement = mapElement;
        }

        /// <summary>
        /// Получаем значения интервала. Сейчас можно получить только через легенду.
        /// </summary>
        /// <returns></returns>
        private string GetValues()
        {
            NamedElement legend = ((MapCore)this.rule.ParentElement).Legends.GetByName(this.rule.ShowInLegend);
            if (legend != null)
            {
                int rangeIndex = this.rule.CustomColors.IndexOf(this.customColor);
                return ((Legend)legend).Items[rangeIndex].Text;
            }
            return String.Empty;
        }

        /// <summary>
        /// Получаем имя интервала
        /// </summary>
        /// <returns></returns>
        private string GetIntervalName()
        {
            int rangeIndex = this.rule.CustomColors.IndexOf(this.customColor);
            if (rangeIndex < this.mapElement.ColorIntervalNames.Count)
            {
                return this.mapElement.ColorIntervalNames[rangeIndex];
            }
            return String.Empty;
        }


        /// <summary>
        /// Получаем имя интервала
        /// </summary>
        /// <returns></returns>
        private void SetIntervalName(string value)
        {
            int rangeIndex = this.rule.CustomColors.IndexOf(this.customColor);
            if (rangeIndex < this.mapElement.ColorIntervalNames.Count)
            {
                this.mapElement.ColorIntervalNames[rangeIndex] = value;
            }
        }


        public override string ToString()
        {
            return "";
        }

    }


}

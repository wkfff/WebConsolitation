using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Grid;
using Krista.FM.Client.MDXExpert.Grid.Style;

namespace Krista.FM.Client.MDXExpert
{
    [TypeConverter(typeof(PropertySorter))]
    public class ColorRuleBrowseClass : FilterablePropertyBase
    {
        private ColorRule _rule;

        public ColorRuleBrowseClass(ColorRule rule)
        {
            this._rule = rule;
        }

        #region Свойства

        [PropertyOrder(10)]
        [Category("Общие")]
        [DisplayName("Условие")]
        [Description("Условие правила раскраски")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(true)]
        public ColorCondition Condition
        {
            get { return this._rule.Condition; }
            set { this._rule.Condition = value; }
        }

        [PropertyOrder(20)]
        [Category("Общие")]
        [DisplayName("Значение")]
        [Description("Значение для условия раскраски")]
        [Browsable(true)]
        public decimal Value1
        {
            get { return this._rule.Value1; }
            set { this._rule.Value1 = value; }
        }

        [PropertyOrder(30)]
        [Category("Общие")]
        [DisplayName("Значение 2")]
        [Description("Значение для условия раскраски")]
        [DynamicPropertyFilter("Condition", "Interval")]
        [Browsable(true)]
        public decimal Value2
        {
            get { return this._rule.Value2; }
            set { this._rule.Value2 = value; }
        }

        //градиент
        [PropertyOrder(1)]
        [Category("Вид")]
        [DisplayName("Градиент")]
        [Description("Градиент")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [Browsable(true)]
        public bool IsGradient
        {
            get { return this._rule.IsGradient; }
            set { this._rule.IsGradient = value; }
        }

        //цвет фона
        [PropertyOrder(2)]
        [Category("Вид")]
        [DisplayName("Цвет фона")]
        [Description("Цвет фона у ячеек мер")]
        [Browsable(true)]
        public Color BackColorStart
        {
            get { return this._rule.BackColorStart; }
            set { this._rule.BackColorStart = value; }
        }

        //цвет фона
        [PropertyOrder(3)]
        [Category("Вид")]
        [DisplayName("Завершающий цвет фона")]
        [Description("Завершающий цвет фона у ячеек мер")]
        [DynamicPropertyFilter("IsGradient", "True")]
        [Browsable(true)]
        public Color BackColorEnd
        {
            get { return this._rule.BackColorEnd; }
            set { this._rule.BackColorEnd = value; }
        }

        #endregion
    }


}

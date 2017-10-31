using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Grid.Style;
using Krista.FM.Common.Xml;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Правило условной раскраски
    /// </summary>
    public class ColorRule
    {

        private ColorCondition _condition;
        private decimal _value1;
        private decimal _value2;
        private CellStyle _appearance;
        private ColorRuleArea _area;
        private string _measureName;
        private string _measureCaption;
        private ColorRuleCollection _parentCollection;

        /// <summary>
        /// Условие
        /// </summary>
        public ColorCondition Condition
        {
            get { return _condition; }
            set
            {
                if (value != _condition)
                {
                    _condition = value;
                    DoColorRuleChange();
                }
            }
        }

        /// <summary>
        /// Значение 1
        /// </summary>
        public decimal Value1
        {
            get { return _value1; }
            set 
            { 
                if (value != _value1)
                {
                    _value1 = value;
                    DoColorRuleChange();
                }

            }
        }

        /// <summary>
        /// Значение 2
        /// </summary>
        public decimal Value2
        {
            get { return _value2; }
            set 
            {
                if (value != _value2)
                {
                    _value2 = value;
                    DoColorRuleChange();
                }
            }
        }

        /// <summary>
        /// Стиль ячеек
        /// </summary>
        public CellStyle Appearance
        {
            get { return _appearance; }
            set { _appearance = value; }
        }

        /// <summary>
        /// Заливка ячеек градиентом
        /// </summary>
        public bool IsGradient
        {
            get { return this.GetGradient(); }
            set
            {
                this.SetGradient(value);
                DoColorRuleChange();
            }
        }

        public Color BackColorStart
        {
            get { return this._appearance.BackColorStart; }
            set
            {
                if (value != this._appearance.BackColorStart)
                {
                    this._appearance.BackColorStart = value;
                    DoColorRuleChange();
                }
            }
        }

        public Color BackColorEnd
        {
            get { return this._appearance.BackColorEnd; }
            set
            {
                if (value != this._appearance.BackColorEnd)
                {
                    this._appearance.BackColorEnd = value;
                    DoColorRuleChange();
                }
            }
        }

        /// <summary>
        /// Область применения правила
        /// </summary>
        public ColorRuleArea Area
        {
            get { return _area; }
            set
            {
                if (value != _area)
                {
                    _area = value;
                    DoColorRuleChange();
                }

            }
        }

        /// <summary>
        /// Имя меры, к которой применяется раскраска
        /// </summary>
        public string MeasureName
        {
            get { return _measureName; }
            set
            {
                if (value != _measureName)
                {
                    _measureName = value;
                    DoColorRuleChange();
                }
            }
        }

        /// <summary>
        /// Заголовок меры
        /// </summary>
        public string MeasureCaption
        {
            get { return GetMeasureCaption(); }
        }

        /// <summary>
        /// Родительская коллекция
        /// </summary>
        public ColorRuleCollection ParentCollection
        {
            get { return _parentCollection; }
            set { _parentCollection = value; }
        }

        private ColorRule()
        {
        }


        public ColorRule(string measureName, ColorCondition condition, decimal value1, decimal value2)
        {
            this._measureName = measureName;
            this._condition = condition;
            this._value1 = value1;
            this._value2 = value2;
            this._area = ColorRuleArea.Cells;

            this._appearance = new CellStyle(null, Color.White, Color.White, Color.Black, Color.Black);
            this._appearance.SetDefaultStyle();
            this._appearance.BackColorStart = Color.White;
            this._appearance.BackColorEnd = Color.White;
        }

        /// <summary>
        /// вызвать событие изменения
        /// </summary>
        private void DoColorRuleChange()
        {
            if (this.ParentCollection == null)
                return;
            this.ParentCollection.Grid.DrawGrid(AreaSet.All);
            this.ParentCollection.DoColorRulesChange();
        }

        /// <summary>
        /// получить заголовок меры
        /// </summary>
        /// <returns></returns>
        private string GetMeasureCaption()
        {
            if ((this.ParentCollection != null)&&(this.ParentCollection.Grid != null))
            {
                PivotTotal total = this.ParentCollection.Grid.PivotData.TotalAxis.GetTotalByName(this.MeasureName);
                if (total != null)
                {
                    return total.Caption;
                }
                else
                {
                    foreach (Measure measure in this.ParentCollection.PivotData.Cube.Measures)
                    {
                        if (measure.UniqueName == this.MeasureName)
                            return measure.Caption;
                    }
                }

            }
            return this.MeasureName != null ? this.MeasureName : String.Empty;
        }

        /// <summary>
        /// действует ли правило раскраски на ячейку
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool IsAffectedCell(MeasureCell cell)
        {
            if (cell.MeasureData.MeasureCaption.UniqueName != this.MeasureName)
                return false;

            if ((cell.Value != null) && (cell.Value.Value != null))
            {
                if (cell.IsTotal)
                {
                    if ((this.Area & ColorRuleArea.Totals) != ColorRuleArea.Totals)
                        return false;
                } 
                else
                {
                    if ((this.Area & ColorRuleArea.Cells) != ColorRuleArea.Cells)
                        return false;
                }

                decimal cellValue;
                if (decimal.TryParse(cell.Value.Value.ToString(), out cellValue))
                {

                    switch (this.Condition)
                    {
                        case ColorCondition.Equal:
                            return (cellValue == this.Value1);
                        case ColorCondition.Less:
                            return cellValue < this.Value1;
                        case ColorCondition.LessOrEqual:
                            return cellValue <= this.Value1;
                        case ColorCondition.Greater:
                            return cellValue > this.Value1;
                        case ColorCondition.GreaterOrEqual:
                            return cellValue >= this.Value1;
                        case ColorCondition.Interval:
                            return ((cellValue >= this.Value1) && (cellValue <= this.Value2));
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Сохранение настроек правила
        /// </summary>
        /// <param name="ruleNode"></param>
        public void Save(XmlNode ruleNode)
        {
            if (ruleNode == null)
                return;

            XmlHelper.SetAttribute(ruleNode, GridConsts.measureName, this.MeasureName);
            XmlHelper.SetAttribute(ruleNode, GridConsts.colorRuleArea, this.Area.ToString());
            XmlHelper.SetAttribute(ruleNode, GridConsts.colorRuleCondition, this.Condition.ToString());
            XmlHelper.SetAttribute(ruleNode, GridConsts.colorRuleValue1, this.Value1.ToString());
            XmlHelper.SetAttribute(ruleNode, GridConsts.colorRuleValue2, this.Value2.ToString());

            this.Appearance.Save(XmlHelper.AddChildNode(ruleNode, GridConsts.style));
        }

        /// <summary>
        /// Создание правила из xml - узла
        /// </summary>
        /// <param name="ruleNode"></param>
        /// <param name="isLoadTemplate"></param>
        public static ColorRule Load(ExpertGrid grid, XmlNode ruleNode, bool isLoadTemplate)
        {
            if (ruleNode == null)
                return null;

            ColorRule rule = new ColorRule();

            rule.MeasureName = XmlHelper.GetStringAttrValue(ruleNode, GridConsts.measureName, "");
            rule.Condition = (ColorCondition)Enum.Parse(typeof(ColorCondition),
                        XmlHelper.GetStringAttrValue(ruleNode, GridConsts.colorRuleCondition, "Equal"));
            rule.Area = (ColorRuleArea)Enum.Parse(typeof(ColorRuleArea),
                        XmlHelper.GetStringAttrValue(ruleNode, GridConsts.colorRuleArea, "None"));
            rule.Value1 = (decimal)XmlHelper.GetFloatAttrValue(ruleNode, GridConsts.colorRuleValue1, 0);
            rule.Value2 = (decimal)XmlHelper.GetFloatAttrValue(ruleNode, GridConsts.colorRuleValue2, 0);

            rule.Appearance = new CellStyle(grid, Color.White, Color.White, Color.Black, Color.Black);
            rule.Appearance.SetDefaultStyle();
            rule.Appearance.Load(ruleNode.SelectSingleNode(GridConsts.style), isLoadTemplate);

            return rule;
        }

        private bool GetGradient()
        {
            return (this._appearance.Gradient != Gradient.None);
        }

        private void SetGradient(bool value)
        {
            this._appearance.Gradient = value ? Gradient.Horizontal : Gradient.None;
        }

        public override string ToString()
        {
            string result = String.Empty;
            
            switch(this.Condition)
            {
                case ColorCondition.Equal:
                case ColorCondition.Less:
                case ColorCondition.LessOrEqual:
                case ColorCondition.Greater:
                case ColorCondition.GreaterOrEqual:
                    result = String.Format("\'{0}\' {1} {2}", this.MeasureCaption, CommonUtils.GetEnumValueDescription(typeof(ColorCondition), this.Condition), this.Value1);
                    break;
                case ColorCondition.Interval:
                    result = String.Format("\'{0}\' между {1} и {2}", this.MeasureCaption, this.Value1, this.Value2);
                    break;
            }

            return result;
        }
    }

    /// <summary>
    /// Условие раскраски ячейки
    /// </summary>
    public enum ColorCondition
    {
        [Description("равно")]
        Equal,
        [Description("меньше")]
        Less,
        [Description("меньше или равно")]
        LessOrEqual,
        [Description("больше")]
        Greater,
        [Description("больше или равно")]
        GreaterOrEqual,
        [Description("интервал")]
        Interval
    }

    /// <summary>
    /// Область применения правила раскраски
    /// </summary>
    [Flags]
    public enum ColorRuleArea
    {
        None,
        //ячейки с данными
        Cells,
        //итоги
        Totals
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Client.MDXExpert.Grid.Style;
using Krista.FM.Common.Xml;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Коллекция правил условной раскраски
    /// </summary>
    public class ColorRuleCollection : IEnumerable
    {
        #region поля

        private List<ColorRule> _rules;
        private ExpertGrid _grid;
        //флаг, указывающий, что сейчас идет обновление свойств (загрузка из xml)
        private bool _isUpdating = false;

        #endregion 

        #region Свойства

        public ColorRule this[int index]
        {
            get
            {
                return _rules[index];
            }
        }

        public int Count
        {
            get { return this._rules.Count; }
        }


        public PivotData PivotData
        {
            get
            {
                return this._grid != null ? this._grid.PivotData : null;
            }
        }

        public ExpertGrid Grid
        {
            get { return _grid; }
            set { _grid = value; }
        }

        #endregion

        public ColorRuleCollection(ExpertGrid grid)
        {
            this._rules = new List<ColorRule>();
            this._grid = grid;

        }

        public void Add(ColorRule rule)
        {
            this._rules.Add(rule);
            rule.ParentCollection = this;
            DoColorRulesChange();
        }

        public void Delete(ColorRule rule)
        {
            this._rules.Remove(rule);
            this.Grid.DrawGrid(AreaSet.All);
            DoColorRulesChange();
        }

        public void Clear()
        {
            this._rules.Clear();
        }
        
        /// <summary>
        /// вызвать событие изменения условной раскраски
        /// </summary>
        public void DoColorRulesChange()
        {
            if (this._isUpdating)
                return;

            if (this.Grid != null)
            {
                this.Grid.OnColorRulesChanged();
            }
        }

        /// <summary>
        /// Действует ли на ячейку какое либо правило раскраски 
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool IsAffectedCell(MeasureCell cell)
        {
            foreach(ColorRule rule in this)
            {
                if (rule.IsAffectedCell(cell))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Получить внешний вид ячейки, если на нее действует правило раскраски
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public CellStyle GetStyleForAffectedCell(MeasureCell cell)
        {
            foreach(ColorRule rule in this._rules)
            {
                if (rule.IsAffectedCell(cell))
                {
                    return rule.Appearance;
                }
            }
            return null;
        }

        /// <summary>
        /// Применить правило раскраски к стилю ячейки, если ячейка подпадает под это правило
        /// </summary>
        /// <param name="cell">ячейка</param>
        /// <param name="currentStyle">текущий стиль ячейки</param>
        /// <returns></returns>
        public CellStyle ApplyColorRuleForCell(MeasureCell cell, CellStyle currentStyle)
        {
            CellStyle style = GetStyleForAffectedCell(cell);
            if (style != null)
            {
                style.BorderColor = currentStyle.BorderColor;
                style.Font = currentStyle.Font;
                style.ForeColor = currentStyle.ForeColor;
                style.StringFormat = currentStyle.StringFormat;
                style.BackColorEnd = (style.Gradient == Gradient.None) ? style.BackColorStart : style.BackColorEnd;
                return style;
            }

            return currentStyle;
        }

        public void Save(XmlNode collectionNode)
        {
            if (collectionNode == null)
                return;

            foreach(ColorRule rule in this)
            {
                rule.Save(XmlHelper.AddChildNode(collectionNode, GridConsts.colorRule));
            }
        }

        public void Load(XmlNode collectionNode, bool isLoadTemplate)
        {
            if (collectionNode == null)
                return;
            this._isUpdating = true;

            this.Clear();
            XmlNodeList ruleNodes = collectionNode.SelectNodes(GridConsts.colorRule);

            foreach (XmlNode ruleNode in ruleNodes)
            {
                ColorRule rule = ColorRule.Load(this.Grid, ruleNode, isLoadTemplate);
                
                this.Add(rule);
            }
            this._isUpdating = false;
            
        }

        public IEnumerator GetEnumerator()
        {
            return this._rules.GetEnumerator();
        }

        public override string ToString()
        {
            return String.Empty;
        }
    }
}

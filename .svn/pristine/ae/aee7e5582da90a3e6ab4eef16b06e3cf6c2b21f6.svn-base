using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;

namespace Krista.FM.Update.Framework.Conditions
{
    [Serializable]
    public sealed class BooleanCondition : UpdateCondition, IBooleanCondition
    {
        #region Condition types

        /// <summary>
        /// Получает способ связывания условий между собой
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ConditionType ConditionTypeFromString(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                switch (type.ToLower())
                {
                    case "and":
                        return ConditionType.AND;
                    case "or":
                        return ConditionType.OR;
                    case "not":
                    case "and-not":
                        return ConditionType.AND | ConditionType.NOT;
                    case "or-not":
                        return ConditionType.OR | ConditionType.NOT;
                }
            }

            // Делаем AND условием по умолчанию
            return ConditionType.AND;
        }
        #endregion

        [Serializable]
        private class ConditionItem : IConditionItem
        {
            private IUpdateCondition _condition;
            private ConditionType _conditionType;
            public ConditionItem(IUpdateCondition cnd, ConditionType typ)
            {
                this._Condition = cnd;
                this._ConditionType = typ;
            }

            public IUpdateCondition _Condition
            {
                get { return _condition; }
                set { _condition = value; }
            }
                
            public ConditionType _ConditionType
            {
                get { return _conditionType; }
                set { _conditionType = value; }
            }
            
        }

        /// <summary>
        /// Связанный лист вложенных условий
        /// </summary>
        private LinkedList<IConditionItem> ChildConditions { get; set; }
        /// <summary>
        /// Количество вложенных условий
        /// </summary>
        public int ChildConditionsCount { get { if (ChildConditions != null) return ChildConditions.Count; return 0; } }

        /// <summary>
        /// Добавляем условие в конец связанного списка. Тип условия по умолчанию AND 
        /// </summary>
        /// <param name="cnd"></param>
        public void AddCondition(IUpdateCondition cnd)
        {
            AddCondition(cnd, ConditionType.AND);
        }

        /// <summary>
        /// Добавляем условие с указанным типом объекдинения условий
        /// </summary>
        /// <param name="cnd"></param>
        /// <param name="type"></param>
        public void AddCondition(IUpdateCondition cnd, ConditionType type)
        {
            if (ChildConditions == null) ChildConditions = new LinkedList<IConditionItem>();
            ChildConditions.AddLast(new ConditionItem(cnd, type));
        }

        public IUpdateCondition Degrade()
        {
            if (ChildConditionsCount == 1 && (ChildConditions.First.Value._ConditionType & ConditionType.NOT) == 0)
                return ChildConditions.First.Value._Condition;

            return this;
        }

        #region IUpdateCondition Members
        
        /// <summary>
        /// Проверяет выполнение всех условий с учетом правил связывания условий 
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public override bool IsMet(IUpdateTask task)
        {
            if (ChildConditions == null)
                return true;

            bool Passed = true, firstRun = true;
            foreach (ConditionItem item in ChildConditions)
            {
                if (!firstRun)
                {
                    if (Passed && (item._ConditionType & ConditionType.OR) > 0)
                        continue;
                }
                else { firstRun = false; }

                if (!Passed)
                {
                    if ((item._ConditionType & ConditionType.OR) > 0)
                    {
                        bool checkResult = item._Condition.IsMet(task);
                        Passed = (item._ConditionType & ConditionType.NOT) > 0 ? !checkResult : checkResult;
                    }
                }
                else
                {
                    bool checkResult = item._Condition.IsMet(task);
                    Passed = (item._ConditionType & ConditionType.NOT) > 0 ? !checkResult : checkResult;
                }
            }

            return Passed;
        }

        public override XElement ToXml()
        {
            var conditions = new XElement("Conditions");

            foreach (var childCondition in ChildConditions)
            {
                conditions.Add(childCondition._Condition.ToXml());
            }

            return conditions;
        }

        #endregion
    }
}

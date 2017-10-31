using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Krista.FM.Update.Framework.Conditions;

namespace Krista.FM.Update.Framework.UpdateObjects.Tasks
{
    /// <summary>
    /// Базовая реализация задачи для обновления
    /// </summary>
    [Serializable]
    public class UpdateTask : IUpdateTask
    {
        #region Constructor

        public UpdateTask(IUpdatePatch owner)
        {
            this.Owner = owner;

            Attributes = new Dictionary<string, string>();
            UpdateConditions = new BooleanCondition();
        }

        #endregion

        #region IUpdateTask Members

        public IDictionary<string, string> Attributes { get; private set; }

        public string Description { get; set; }

        public IBooleanCondition UpdateConditions { get; set; }

        /// <summary>
        /// Проверка готовности задачи к выполнению,
        /// чтобы не обламывать весь процесс обновления ,
        /// когда обломилась подготовка одной задачи
        /// </summary>
        public PrepareState IsPrepared { get; protected set; }

        public IUpdatePatch Owner { get; set; }

        public virtual int OrderByFactor
        {
            get { return 1; }
        }

        public virtual bool Prepare(IUpdateSource source)
        {
            return false;
        }

        public virtual ExecuteState Execute()
        {
            return ExecuteState.ExecuteWithError;
        }

        public virtual bool Rollback()
        {
            return false;
        }

        public virtual XElement ToXml()
        {
            XElement updateTaskEl = new XElement(this.GetType().Name);

            foreach (KeyValuePair<string, string> keyValuePair in Attributes)
            {
                updateTaskEl.SetAttributeValue(keyValuePair.Key, keyValuePair.Value);
            }

            updateTaskEl.Add(new XElement("Description", Description));

            if (UpdateConditions.ChildConditionsCount != 0)
            {
                updateTaskEl.Add(UpdateConditions.ToXml());
            }

            return updateTaskEl;
        }

        #endregion
    }
}

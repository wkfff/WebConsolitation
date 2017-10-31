using System;
using System.Collections.Generic;
using System.Xml;

using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Modifications;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Правило сопоставления
    /// </summary>
    internal class AssociateRule : MinorObject, IAssociateRule
    {
        private string name = String.Empty;
        private bool useConversionTable = true;
        private bool useFieldCoincidence = true;
        private bool addNewRecords = false;
        private bool readOnly = true;
        private StringElephanterSettings settings = null;
        /// <summary>
        /// true - автоматически сопоставлять все записи; false - сопоставлять только несопоставленные
        /// </summary>
        private bool reAssociate = true;

        /// <summary>
        /// Маппинг полей
        /// </summary>
        private AssociateMappingCollection mappings;


        /// <summary>
        /// Правило сопоставления
        /// </summary>
        public AssociateRule(string key, ServerSideObject owner, ServerSideObjectStates state)
            : base(key, owner, state)
        {
            mappings = new AssociateMappingCollection(this, state);
        }

        /// <summary>
        /// Маппинг полей
        /// </summary>
        public IAssociateMappingCollection Mappings
        {
            get { return mappings; }
        }

        /// <summary>
        /// Проверяет возможность редактирования свойства
        /// </summary>
        private void CheckAccess()
        {
            if (readOnly)
                throw new Exception("Свойство доступно только для чтения.");
        }

        /// <summary>
        /// Наименование правила
        /// </summary>
        public string Name
        {
            get { return name; }
            set { CheckAccess(); name = value; }
        }

        /// <summary>
        /// Уникальное наименование объекта (старый ключ).
        /// Это свойство после выпуска версии 2.4.1 необходимо удалить.
        /// </summary>
        public override string ObjectOldKeyName
        {
            get { return Name; }
        }

        /// <summary>
        /// true - сопоставление с использованием таблицы перекодировок; false - без таблицы перекодировок
        /// </summary>
        public bool UseConversionTable
        {
            get { return useConversionTable; }
            set { CheckAccess(); useConversionTable = value; }
        }

        /// <summary>
        /// true - сопоставление по совпадению полей; false - не учитывать совпадение.
        /// </summary>
        public bool UseFieldCoincidence
        {
            get { return useFieldCoincidence; }
            set { CheckAccess(); useFieldCoincidence = value; }
        }

        /// <summary>
        /// true - Автоматически добавлять новые записи в сопоставимый; false - не добавлять новые записи
        /// </summary>
        public bool AddNewRecords
        {
            get { return addNewRecords; }
            set { CheckAccess(); addNewRecords = value; }
        }

        /// <summary>
        /// true - автоматически сопоставлять все записи; false - сопоставлять только несопоставленные
        /// </summary>
        public bool ReAssociate
        {
            get { return reAssociate; }
            set { CheckAccess(); reAssociate = value; }
        }

        /// <summary>
        /// Определяет можно ли изменять свойства правила сопоставления
        /// </summary>
        public bool ReadOnly
        {
            get { return readOnly; }
            set { readOnly = value; }
        }

        /// <summary>
        /// Если автоматически добавляются новые записи в сопоставимый 
        /// и хотя бы одно сравнение имеет релевантность менее 100%, 
        /// то возвращает true, иначе false
        /// </summary>
        public bool IsSample
        {
            get
            {
                //if (UseConversionTable)
                //    return false;

                if (AddNewRecords)
                    return false;

                foreach (AssociateMapping mapping in Mappings)
                {
                    if (mapping.Relevant != 100)
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Настройки сопоставления для строковых полей, если null, то используются значения по умолчанию
        /// </summary>
        public StringElephanterSettings Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        /// <summary>
        /// Создает копию правила доступного для редактирования
        /// </summary>
        /// <returns>Копия правила сопоставления</returns>
        /// <remarks>По окончанию работы с копией правила необходимо вызвать метод Dispose!</remarks>
        public IAssociateRule CloneRule()
        {
            AssociateRule associateRule = new AssociateRule(this.ObjectKey, this.Owner, this.State);
            associateRule.ReadOnly = false;
            associateRule.Name = this.Name;
            associateRule.UseConversionTable = this.UseConversionTable;
            associateRule.UseFieldCoincidence = this.UseFieldCoincidence;
            associateRule.AddNewRecords = this.AddNewRecords;
            associateRule.ReAssociate = this.ReAssociate;
            associateRule.mappings = this.mappings;

            return associateRule;
        }

        internal override void Save2Xml(XmlNode node)
        {
            base.Save2Xml(node);

            XmlHelper.SetAttribute(node, "name", Name);
            if (!UseConversionTable)
                XmlHelper.SetAttribute(node, "conversionTable", "false");
            if (!UseFieldCoincidence)
                XmlHelper.SetAttribute(node, "fieldCoincidence", "false");
            if (AddNewRecords)
                XmlHelper.SetAttribute(node, "addNew", "true");

            foreach (AssociateMapping item in Mappings)
            {
                XmlNode xmlMapping = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Mapping", null);

                if (!String.IsNullOrEmpty(item.ObjectKey) && item.ObjectKey != Guid.Empty.ToString())
                {
                    XmlHelper.SetAttribute(xmlMapping, "objectKey", item.ObjectKey);
                }

                if (item.Relevant != 100)
                    XmlHelper.SetAttribute(node, "relevant", Convert.ToString(item.Relevant));

                XmlNode xmlMapValue = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Data", null);
                item.DataAttribute.SaveToXml(xmlMapValue);
                xmlMapping.AppendChild(xmlMapValue);

                xmlMapValue = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Bridge", null);
                item.BridgeAttribute.SaveToXml(xmlMapValue);
                xmlMapping.AppendChild(xmlMapValue);

                node.AppendChild(xmlMapping);
            }
        }

        #region IModifiable Members

        public override IModificationItem GetChanges(IModifiable toObject)
        {
            MinorObjectModificationItem root = new MinorObjectModificationItem(ModificationTypes.Modify, Name, this, toObject, null);

            AssociateRule toAssociateRule = (AssociateRule)toObject;

            //
            // Уникальный ключ объекта
            //
            IModificationItem keyModificationItem = base.GetChanges(toAssociateRule);
            if (keyModificationItem != null)
            {
                root.Items.Add(keyModificationItem.Key, keyModificationItem);
                ((ModificationItem)keyModificationItem).Parent = root;
            }

            //
            // Маппинг полей
            //

            ModificationItem mi = (ModificationItem)this.mappings.GetChanges(toAssociateRule.mappings);
            if (mi.Items.Count > 0)
            {
                root.Items.Add(mi.Key, mi);
            }

            return root;
        }

        #endregion

        #region IMinorModifiable Members

        public void Update(IModifiable toObject)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IServerSideObject Members

        /// <summary>
        /// Состояние серверного объекта во времени его существования
        /// </summary>
        public override ServerSideObjectStates State
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return base.State; }
            set
            {
                if (base.State != value)
                {
                    base.State = value;
                    mappings.State = value;
                }
            }
        }

        public override IServerSideObject Lock()
        {
            BridgeAssociation cloneAssociation = (BridgeAssociation)Owner.Lock();
            return (ServerSideObject)cloneAssociation.AssociateRules[GetKey(this.ObjectKey, this.Name)];
        }

        public override void Unlock()
        {
            mappings.Unlock();
            base.Unlock();
        }

        public override object Clone()
        {
            AssociateRule associateRule = (AssociateRule)base.Clone();
            associateRule.mappings = (AssociateMappingCollection)mappings.Clone();

            return associateRule;
        }

        #endregion IServerSideObject Members
    }

    /// <summary>
    /// Коллекция правил сопоставления
    /// </summary>
    internal class AssociateRuleCollection : ModifiableCollection<string, IAssociateRule>, IAssociateRuleCollection
    {
        private string associateRuleDefault;

        internal AssociateRuleCollection(Association owner, ServerSideObjectStates state)
            : base(owner, state)
        {
        }
        
        public string AssociateRuleDefault
        {
            get { return Instance.associateRuleDefault; }
            set { SetInstance.associateRuleDefault = value; }
        }

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected new AssociateRuleCollection Instance
        {
            get { return (AssociateRuleCollection)GetInstance(); }
        }

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь, для установки значений свойств
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected AssociateRuleCollection SetInstance
        {
            get { return SetterMustUseClone() ? (AssociateRuleCollection)CloneObject : this; }
        }

        public override IServerSideObject Lock()
        {
            BridgeAssociation cloneAssociation = (BridgeAssociation)Owner.Lock();
            return (ServerSideObject)cloneAssociation.AssociateRules;
        }

        protected override ModificationItem GetCreateModificationItem(KeyValuePair<string, IAssociateRule> item, CollectionModificationItem root)
        {
            return new CreateMinorModificationItem(Convert.ToString(item.Key), Owner, item.Value, root);
        }

        protected override ModificationItem GetRemoveModificationItem(KeyValuePair<string, IAssociateRule> item, CollectionModificationItem root)
        {
            return new RemoveMinorModificationItem(Convert.ToString(item.Key), item.Value, root);
        }

        public override IAssociateRule CreateItem()
        {
            AssociateRule r = new AssociateRule(Guid.NewGuid().ToString(), Owner, ServerSideObjectStates.New);
            r.ReadOnly = false;
            r.Name = String.Format("Новое правило сопоставления {0}", this.Count + 1); ;
            this.Add(r.Name, r);
            SetAssociateRuleDefault(r); 
            return r;
        }
        
        internal void AddRule(AssociateRule rule)
        {
            this.Add(KeyIdentifiedObject.GetKey(rule.ObjectKey, rule.Name), rule);
            SetAssociateRuleDefault(rule);
        }

        /// <summary>
        /// Установки правила сопоставления по-умолчанию
        /// </summary>
        /// <param name="rule">Правило сопоставления</param>
        private void SetAssociateRuleDefault(AssociateRule rule)
        {
            if (String.IsNullOrEmpty(this.AssociateRuleDefault))
                this.AssociateRuleDefault = rule.Name;
        }

        /// <summary>
        /// Ищет объект в коллекции по ключу (ключем может быть как GUID так и FullName).
        /// </summary>
        /// <param name="item">Объект для поиска.</param>
        /// <returns>Если объект найден, то возвращает true.</returns>
        protected override IAssociateRule ContainsObject(KeyValuePair<string, IAssociateRule> item)
        {
            string key = item.Key;

            try
            {
                new Guid(item.Key);
                if (!this.ContainsKey(item.Key))
                {
                    foreach (IAssociateRule аssociateRule in this.Values)
                    {
                        if (аssociateRule.Name == item.Value.Name)
                            return аssociateRule;
                    }
                }
            }
            catch (FormatException)
            {
                key = item.Key;
            }

            if (this.ContainsKey(key))
                return this[key];
            else
            {
                return null;
            }
        }
    }
}

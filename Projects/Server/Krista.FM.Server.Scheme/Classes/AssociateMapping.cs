using System;
using System.Collections.Generic;

using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Modifications;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Маппинг для формирования и сопоставления классификаторов
    /// </summary>
    internal class AssociateMapping : MinorObject, IAssociateMapping
    {
        /// <summary>
        /// Атрибут в коллекции атрибутов сопоставляемого классификатора
        /// </summary>
        internal MappingValue DataAttribute;

        /// <summary>
        /// Атрибут в коллекции атрибутов сопоставимого классификатора
        /// </summary>
        internal MappingValue BridgeAttribute;

        /// <summary>
        /// Значение % релевантности при сравнении
        /// </summary>
        private int relevant = 100;

        /// <summary>
        /// Маппинг для формирования и сопоставления классификаторов
        /// </summary>
        public AssociateMapping(string key, ServerSideObject owner, ServerSideObjectStates state)
            : base(key, owner, state)
        {
        }

        /// <summary>
        /// Уникальное наименование объекта (старый ключ).
        /// Это свойство после выпуска версии 2.4.1 необходимо удалить.
        /// </summary>
        public override string ObjectOldKeyName
        {
            get { return AssociateMappingCollection.GetMappingKey(this); }
        }

        /// <summary>
        /// Вычисляет типы атрибутов.
        /// </summary>
        internal void InitializeMappingValuesTypes()
        {
            if (DataAttribute.Attribute != null)
            {
                DataAttribute.ValueType = DataAttribute.Attribute.Type;
            }
            else
            {
                // TODO: Переделать алгоритм определения типа данных вычисляемого атрибута
                try
                {
                    foreach (string attrName in DataAttribute.SourceAttributes)
                    {

                        DataAttribute attr = DataAttributeCollection.GetAttributeByKeyName(
                            ((BridgeAssociation)this.Owner.Owner).RoleA.Attributes, attrName, attrName);
                        if (attr != null)
                        {
                            DataAttribute.ValueType = attr.Type;
                            break;
                        }
                    }
                }
                catch (NullReferenceException)
                {
                    DataAttribute.ValueType = DataAttributeTypes.dtInteger;
                }
            }

            if (BridgeAttribute.Attribute != null)
            {
                BridgeAttribute.ValueType = BridgeAttribute.Attribute.Type;
            }
            else
            {
                // TODO: Переделать алгоритм определения типа данных вычисляемого атрибута
                try
                {
                    foreach (string attrName in BridgeAttribute.SourceAttributes)
                    {
                        DataAttribute attr = DataAttributeCollection.GetAttributeByKeyName(
                            ((BridgeAssociation) this.Owner.Owner).RoleB.Attributes,
                            attrName, attrName);
                        //DataAttribute attr = (DataAttribute)((BridgeAssociation)this.Owner.Owner).RoleB.Attributes[attrName];
                        if (attr != null)
                        {
                            BridgeAttribute.ValueType = attr.Type;
                            break;
                        }
                    }
                }
                catch (NullReferenceException)
                {
                    DataAttribute.ValueType = DataAttributeTypes.dtInteger;
                }
            }
        }

        /// <summary>
        /// Значение % релевантности при сравнении
        /// </summary>
        public int Relevant
        {
            get { return Instance.relevant; }
            set
            {
                if (value < 1 || value > 100)
                    throw new Exception(" Свойство Relevant должно иметь значение от 1 до 100.");
                SetInstance.relevant = value;
            }
        }

        #region IAssociateMapping Members

        /// <summary>
        /// Значение со стороны классификатора данных
        /// </summary>
        public IMappingValue DataValue
        {
            get { return Instance.DataAttribute; }
        }

        /// <summary>
        /// Значение со стороны сопоставимого классификатора данных
        /// </summary>
        public IMappingValue BridgeValue
        {
            get { return Instance.BridgeAttribute; }
        }

        #endregion

        #region ServerSideObject

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected AssociateMapping Instance
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return (AssociateMapping)GetInstance(); }
        }

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь, для установки значений свойств
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected AssociateMapping SetInstance
        {
            get
            {
                if (SetterMustUseClone())
                    return (AssociateMapping)CloneObject;
                else
                    return this;
            }
        }

        public override IServerSideObject Lock()
        {
            if (Owner is BridgeAssociation)
            {
                BridgeAssociation ownerClone = (BridgeAssociation)Owner.Lock();
                int indx = ownerClone.Mappings.IndexOf(Instance);
                return ownerClone.Mappings[indx];
            }
            else if (Owner is AssociateRule)
            {
                AssociateRule ownerClone = (AssociateRule)Owner.Lock();
                int indx = ownerClone.Mappings.IndexOf(Instance);
                return ownerClone.Mappings[indx];
            }
            else
                throw new Exception(String.Format("Правило формирования \"{0}\" имеет неверного родителя \"{1}\".", this, Owner));
        }

        public override void Unlock()
        {
            BridgeAttribute.Unlock();
            DataAttribute.Unlock();
            base.Unlock();
        }

        public override object Clone()
        {
            AssociateMapping m = (AssociateMapping)base.Clone();
            m.DataAttribute = DataAttribute.Clone() as MappingValue;
            m.BridgeAttribute = BridgeAttribute.Clone() as MappingValue;
            return m;
        }

        #endregion ServerSideObject

        public override string ToString()
        {
            return String.Format("{0} -> {1} : {2}", Instance.DataAttribute, Instance.BridgeAttribute, base.ToString());
        }
    }

    /// <summary>
    /// Коллекция правил формирования
    /// </summary>
    internal class AssociateMappingCollection : ListBase<IAssociateMapping>, IMinorModifiable, IAssociateMappingCollection
    {
        public AssociateMappingCollection(ServerSideObject owner, ServerSideObjectStates state)
            : base(owner, state)
        {
        }

        public override IServerSideObject Lock()
        {
            if (Owner is AssociateRule)
            {
                AssociateRule clone = (AssociateRule)Owner.Lock();
                return (ServerSideObject)clone.Mappings;
            }
            else if (Owner is BridgeAssociation)
            {
                BridgeAssociation clone = (BridgeAssociation)Owner.Lock();
                return (ServerSideObject)clone.Mappings;
            }
            else
                throw new Exception(String.Format("Коллекция правил формирования имеет недопустимого предка {0}.", Owner));
        }

        /// <summary>
        /// Создание нового объекта
        /// </summary>
        /// <returns>Новый созданный объект</returns>
        public IAssociateMapping CreateItem()
        {
            AssociateMapping am = new AssociateMapping(Guid.NewGuid().ToString(), Owner, ServerSideObjectStates.New);

            // Получаем предка ассоциацию
            ServerSideObject sso = Owner;
            while (!(sso is Association))
            {
                sso = sso.Owner;
                if (sso is Package)
                    throw new Exception("Коллекция правил формирования не имеет предка ассоциацию.");
            }
            Association association = (Association)sso;

            am.DataAttribute = new MappingValue(am, (DataAttribute)association.RoleA.Attributes[DataAttribute.IDColumnName]);
            am.BridgeAttribute = new MappingValue(am, (DataAttribute)association.RoleB.Attributes[DataAttribute.IDColumnName]);
            this.Add(am);
            am.InitializeMappingValuesTypes();
            return am;
        }

        public bool Contains(string dataName, string bridgeName)
        {
            foreach (AssociateMapping item in this)
            {
                if (item.DataAttribute.Name == dataName && item.BridgeAttribute.Name == bridgeName)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Ищет объект в коллекции по ключу (ключем может быть как GUID так и FullName).
        /// </summary>
        /// <param name="mapping">Объект для поиска.</param>
        protected AssociateMapping ContainsObject(AssociateMapping mapping)
        {
            foreach (AssociateMapping item in this)
            {
                if (item.ObjectKey == mapping.ObjectKey)
                    return item;
            }

            foreach (AssociateMapping item in this)
            {
                if (item.DataAttribute.Name == mapping.DataAttribute.Name && item.BridgeAttribute.Name == mapping.BridgeAttribute.Name)
                    return item;
            }

            return null;
        }

        #region IModifiable Members

        internal static string GetMappingKey(AssociateMapping item)
        {
            return item.DataAttribute.Name + "->" + item.BridgeAttribute.Name;
        }

        public IModificationItem GetChanges(IModifiable toObject)
        {
            MinorObjectModificationItem root = new MinorObjectModificationItem(ModificationTypes.Modify, "Правила формирования", this, toObject, null);

            try
            {
                Dictionary<string, AssociateMapping> processedItems = new Dictionary<string, AssociateMapping>();

                foreach (AssociateMapping item in this)
                    processedItems.Add(
                        KeyIdentifiedObject.GetKey(item.ObjectKey, item.ObjectOldKeyName),
                        /*GetMappingKey(item),*/ item);

                AssociateMappingCollection toAssociateMappingCollection = (AssociateMappingCollection) toObject;
                foreach (AssociateMapping item in toAssociateMappingCollection)
                {
                    AssociateMapping originalObject = ContainsObject(item);
                    if (originalObject != null)
                        //if (this.Contains(item.DataAttribute.Name, item.BridgeAttribute.Name))
                    {
                        // Модификация
                        IModificationItem modifyItem = originalObject.GetChanges(item);
                        if (modifyItem != null)
                        {
                            root.Items.Add(modifyItem.Key, modifyItem);
                        }

                        processedItems.Remove(KeyIdentifiedObject.GetKey(originalObject.ObjectKey,
                                                                         originalObject.ObjectOldKeyName));
                    }
                    else
                    {
                        // Создание новых
                        ModificationItem createItem =
                            new CreateMinorModificationItem(
                                KeyIdentifiedObject.GetKey(item.ObjectKey, item.ObjectOldKeyName), this, item, root);
                        root.Items.Add(createItem.Key, createItem);
                    }
                }

                // Удаление старых
                foreach (KeyValuePair<string, AssociateMapping> item in processedItems)
                {
                    ModificationItem removeItem = new RemoveMinorModificationItem(item.Key, item.Value, root);
                    root.Items.Add(removeItem.Key, removeItem);
                }

                return root;
            }
            catch (Exception e)
            {
                Trace.TraceError("В процессе поиска отличий для коллекции правил формирования произошла ошибка: {0}", Krista.Diagnostics.KristaDiagnostics.ExpandException(e));
                throw new Exception(e.Message, e);
            }
        }

        #endregion

        #region IMinorModifiable Members

        public void Update(IModifiable toObject)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}

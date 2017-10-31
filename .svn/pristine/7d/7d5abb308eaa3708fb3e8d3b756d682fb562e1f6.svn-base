using System;
using System.Collections.Generic;

using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Modifications;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme
{
    internal abstract class ModifiableCollection<TKey, TValue> : DictionaryBase<TKey, TValue>, IModifiableCollection<TKey, TValue> where TValue : IKeyIdentifiedObject, ICloneable 
    {
        public ModifiableCollection(ServerSideObject owner, ServerSideObjectStates state)
            : base(owner, state)
        {
        }

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
                    foreach (TValue item in Values)
                        ((ServerSideObject)(IServerSideObject)item).State = value;
                }
            }
        }

        /// <summary>
        /// Определяет должен ли элемынт коллекции быть обработанных.
        /// </summary>
        /// <returns>По умолчанию возвращает true.</returns>
        protected virtual bool CanProsessItem(KeyValuePair<TKey, TValue> item)
        {
            return true;
        }

        protected virtual ModificationItem GetCreateModificationItem(KeyValuePair<TKey, TValue> item, CollectionModificationItem root)
        {
            return new CreateModificationItem(Convert.ToString(item.Key), Owner, item.Value, root);
        }

        protected virtual ModificationItem GetRemoveModificationItem(KeyValuePair<TKey, TValue> item, CollectionModificationItem root)
        {
            return new RemoveMajorModificationItem(Convert.ToString(item.Key), item.Value, root);
        }

        /// <summary>
        /// Ищет объект в коллекции по ключу (ключем может быть как GUID так и FullName).
        /// </summary>
        /// <param name="item">Объект для поиска.</param>
        /// <returns>Если объект найден, то возвращает true.</returns>
        protected virtual TValue ContainsObject(KeyValuePair<TKey, TValue> item)
        {
            if (this.ContainsKey(item.Key))
                return this[item.Key];
            else
            {
                return (TValue)(object)null;
            }
        }

        public virtual ModificationItem GetChanges(IModifiableCollection<TKey, TValue> toCollection)
        {
            CollectionModificationItem root = new CollectionModificationItem("Изменение коллекции", this, toCollection);
            
            // Список элементов, которые подлежат обработке
            Dictionary<TKey, TValue> processedItems = new Dictionary<TKey, TValue>();

            // Определяем элементы подлежащие обработке в текущей коллекции
            foreach (KeyValuePair<TKey, TValue> item in this.list) 
                if (CanProsessItem(item))
                    processedItems.Add(item.Key, item.Value);

            foreach (KeyValuePair<TKey, TValue> item in toCollection)
            {
                if (CanProsessItem(item))
                {
                    object originalObject = ContainsObject(item);
                    if (originalObject != null)
                    {
                        // Модификация
                        ModificationItem modifyItem = (ModificationItem)((IModifiable)originalObject).GetChanges((IModifiable)item.Value);
                        if (modifyItem.Items.Count > 0)
                        {
                            modifyItem.Parent = root;
                            root.Items.Add(modifyItem.Key, modifyItem);
                        }

                        processedItems.Remove(
                            (TKey)(object)Classes.KeyIdentifiedObject.GetKey(
                                ((TValue) originalObject).ObjectKey, 
                                ((TValue)originalObject).ObjectOldKeyName));
                    }
                    else
                    {
                        // Создание новых
                        ModificationItem createItem = GetCreateModificationItem(item, root);
                        root.Items.Add(createItem.Key, createItem);
                    }
                }
            }

            // Удаление старых
            foreach (KeyValuePair<TKey, TValue> item in processedItems)
            {
                ModificationItem removeItem = GetRemoveModificationItem(item, root);
                root.Items.Add(removeItem.Key, removeItem);
            }

            return root;
        }

        #region IModifiableCollection<TKey,TValue> Members

        public virtual TValue CreateItem()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}

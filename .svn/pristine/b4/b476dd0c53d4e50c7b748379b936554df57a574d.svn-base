using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using Krista.FM.Server.Scheme.Modifications;

using Krista.FM.ServerLibrary;
using Krista.FM.Server.Common;
using Krista.FM.Common;


namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Коллекция аттрибутов объекта
    /// </summary>
    internal class DataAttributeCollection : ModifiableCollection<string, IDataAttribute>, IDataAttributeCollection
    {
        private readonly Entity parent;

        public Entity Parent
        {
            get { return parent; }
        }

        public DataAttributeCollection(Entity parent, ServerSideObjectStates state)
            : base(parent, state)
        {
            this.parent = parent;
        }

        public override IServerSideObject Lock()
        {
            Entity cloneEntity = (Entity)Owner.Lock();
            return cloneEntity.Attributes;
        }

        public override bool ContainsKey(string key)
        {
            if (base.ContainsKey(key))
            {
                return true;
            }
            else
            {
                foreach (IDataAttribute item in list.Values)
                {
                    if (item.Name == key)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public override IDataAttribute this[string key]
        {
            get
            {
                if (base.ContainsKey(key))
                {
                    return base[key];
                }
                else
                {
                    foreach (IDataAttribute item in list.Values)
                    {
                        if (item.Name == key)
                        {
                            return item;
                        }
                    }

                    throw new KeyNotFoundException(String.Format("Атрибут с именем \"{0}\" не найден в коллекции.", key));
                }
            }
        }

        [System.Diagnostics.DebuggerStepThrough]
        public void Add(IDataAttribute dataAttribute)
        {
            this.Add(KeyIdentifiedObject.GetKey(dataAttribute.ObjectKey, dataAttribute.Name), dataAttribute);
        }

        public override void Add(string key, IDataAttribute value)
        {
            if (value.Position == 0
                && (value.Class != DataAttributeClassTypes.Fixed && value.Class != DataAttributeClassTypes.System))
                value.Position = GetLastPosition() + 1;

            base.Add(key, value);
            OnAfterAdd(value);
            ((DataAttribute)value).OnAfterAdd();
        }

        private void OnAfterAdd(IDataAttribute value)
        {
            if (!(Owner is IEntity))
                return;

            foreach (IPresentation presentation in ((IEntity)Owner).Presentations.Values)
            {
                if (!presentation.Attributes.ContainsKey(value.Name))
                {
                    IDataAttribute attribute;
                    if (value.Kind == DataAttributeKindTypes.Regular)
                    {
                        attribute = ((DataAttribute)value).CloneStatic();
                        attribute.Visible = false;
                    }
                    else
                    {
                        attribute = value;
                    }

                    if (!presentation.Attributes.ContainsKey(value.Name))
                        presentation.Attributes.Add(attribute);
                }
            }
        }

        public override bool Remove(string key)
        {
            ((DataAttribute)this[key]).OnBeforeRemove();
            if (IsClone)
                ((DataAttribute)this[key]).RepositionAttribute(GetLastPosition());

            bool result = base.Remove(key);
            return result;
        }

        #region Массовые операции при добавлении и удалении атрибута

        private void OnBeforeRemove(IDataAttribute value)
        {
            if (!(Owner is IEntity))
                return;

            foreach (IPresentation presentation in ((IEntity)this.Owner).Presentations.Values)
            {
                if (presentation.Attributes.ContainsKey(value.Name))
                    presentation.Attributes.Remove(value.ObjectKey);
            }
        }

        /// <summary>
        /// Получает максимальную позицию в коллекции пользовательских атрибутов
        /// </summary>
        /// <returns></returns>
        public int GetLastPosition()
        {
            int last = 0;

            foreach (KeyValuePair<string, IDataAttribute> pair in Instance)
            {
                if (last < pair.Value.Position)
                    last = pair.Value.Position;
            }

            return last;
        }

        #endregion Массовые операции при добавлении и удалении атрибута

        protected override bool CanProsessItem(KeyValuePair<string, IDataAttribute> item)
        {
            if (Owner is IPresentation)
                return item.Value.Class == DataAttributeClassTypes.Typed || DataAttribute.IsFixedDivideCodeAttribute(item.Value.Name) || item.Value.Class == DataAttributeClassTypes.Reference;
            return item.Value.Class == DataAttributeClassTypes.Typed || DataAttribute.IsFixedDivideCodeAttribute(item.Value.Name);
        }

        protected override ModificationItem GetCreateModificationItem(KeyValuePair<string, IDataAttribute> item, CollectionModificationItem root)
        {
            return new CreateMinorModificationItem(Convert.ToString(item.Key), Parent, item.Value, root);
        }

        protected override ModificationItem GetRemoveModificationItem(KeyValuePair<string, IDataAttribute> item, CollectionModificationItem root)
        {
            return new RemoveMinorModificationItem(Convert.ToString(item.Key), item.Value, root);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clon"></param>
        /// <param name="cloneItems"></param>
        protected override void CloneItems(Common.DictionaryBase<string, IDataAttribute> clon, bool cloneItems)
        {
            foreach (KeyValuePair<string, IDataAttribute> item in this.list)
            {
                ((DataAttributeCollection)clon).list.Add(item.Key,
                                                          cloneItems &&
                                                          (item.Value.Class == DataAttributeClassTypes.Typed ||
                                                           (Owner is IPresentation &&
                                                            item.Value.Class == DataAttributeClassTypes.Reference))
                                                              ? (IDataAttribute)item.Value.Clone()
                                                              : item.Value);
            }
        }

        #region ICollection2DataTable Members

        public DataTable GetDataTable()
        {
            DataColumn[] columns = new DataColumn[1];
            columns[0] = new DataColumn("Attribute", typeof(object));
            /*            columns[1] = new DataColumn("ClassType", typeof(ClassTypes));
                        columns[2] = new DataColumn("Semantic", typeof(String));
                        columns[3] = new DataColumn("Caption", typeof(String));
                        columns[4] = new DataColumn("Description", typeof(String));
            */
            DataTable dt = new DataTable(GetType().Name);
            dt.Columns.AddRange(columns);

            foreach (IDataAttribute item in this.Values)
            {
                DataRow row = dt.NewRow();

                XmlSerializer serializer = new XmlSerializer(typeof(DataAttribute));
                // Create an XmlTextWriter using a FileStream.
                Stream fs = new FileStream(@"d:\test.xml", FileMode.Create);
                XmlWriter writer = new XmlTextWriter(fs, Encoding.Unicode);
                // Serialize using the XmlTextWriter.
                serializer.Serialize(writer, item);
                writer.Close();

                // Create an instance of the XmlSerializer specifying type and namespace.
                XmlSerializer serializer2 = new XmlSerializer(typeof(DataAttribute));

                // A FileStream is needed to read the XML document.
                FileStream fs2 = new FileStream(@"d:\test.xml", FileMode.Open);
                XmlReader reader = new XmlTextReader(fs2);

                // Declare an object variable of the type to be deserialized.
                try
                {
                    // Use the Deserialize method to restore the object's state.
                    serializer2.Deserialize(reader);
                }
                finally
                {
                    reader.Close();
                    fs2.Close();
                }

                row[0] = item;
                /*                row[1] = item.ClassType;
                                row[2] = ((Entity)(IEntity)item).SemanticCaption;
                                row[3] = item.Caption;
                                row[4] = item.Description;*/
                dt.Rows.Add(row);
            }

            return dt;
        }

        #endregion

        public override IDataAttribute CreateItem()
        {
            return CreateItem(AttributeClass.Regular);
        }

        public IDataAttribute CreateItem(AttributeClass attributeClass)
        {
            string key = Guid.NewGuid().ToString();
            EntityDataAttribute attribute = EntityDataAttribute.CreateAttribute(key,
                                                                                ServerSideObject.GetNewName(key),
                                                                                Owner, attributeClass,
                                                                                ServerSideObjectStates.New);
            attribute.Caption = String.Format("Новый атрибут {0}", this.Count + 1);
            attribute.Class = DataAttributeClassTypes.Typed;
            attribute.IsNullable = false;
            attribute.Description = String.Empty;
            attribute.Visible = true;
            attribute.Kind = DataAttributeKindTypes.Regular;
            attribute.IsReadOnly = false;
            attribute.GroupParentAttribute = String.Empty;

            if (attributeClass == AttributeClass.Document)
            {
                attribute.Type = DataAttributeTypes.dtBLOB;
            }
            else if (((Entity)Owner).ClassType == ClassTypes.clsFactData)
            {
                attribute.Type = DataAttributeTypes.dtDouble;
                attribute.Size = 17;
                attribute.Scale = 2;
            }
            else
            {
                attribute.Type = DataAttributeTypes.dtInteger;
                attribute.Size = 10;
                attribute.Mask = String.Empty;
            }

            this.Add(attribute);
            return attribute;
        }

        /// <summary>
        /// Ищет объект в коллекции по ключу (ключем может быть как GUID так и FullName).
        /// </summary>
        /// <param name="item">Объект для поиска.</param>
        /// <returns>Если объект найден, то возвращает true.</returns>
        protected override IDataAttribute ContainsObject(KeyValuePair<string, IDataAttribute> item)
        {
            if (this.ContainsKey(item.Key))
                return this[item.Key];

            return null;
        }

        /// <summary>
        /// Возвращает аттрибут из коллекции по ключу или по паименованию.
        /// </summary>
        /// <param name="collection">Коллекция атрибутов.</param>
        /// <param name="key">Уникальный ключ объекта.</param>
        /// <param name="name">Наименование объекта.</param>
        internal static DataAttribute GetAttributeByKeyName(IDataAttributeCollection collection, string key, string name)
        {
            if (collection.ContainsKey(key))
            {
                return (DataAttribute)collection[key];
            }
            else
            {
                IDataAttribute attr = null;
                foreach (IDataAttribute item in collection.Values)
                {
                    if (item.Name == name)
                    {
                        attr = item;
                        break;
                    }
                }
                return (DataAttribute)attr;
            }
        }

        #region IEnumerable Members

        protected override System.Collections.IEnumerator Enumerator()
        {
            return new AttributesEnumerator(list);
        }

        #endregion
    }
}
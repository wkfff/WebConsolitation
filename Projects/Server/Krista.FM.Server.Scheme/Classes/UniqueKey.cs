using System;
using System.Collections.Generic;
using System.Xml;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Modifications;
using Krista.FM.Server.Scheme.ScriptingEngine.Classes;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes
{
    internal class UniqueKey : CommonObject, IUniqueKey
    {
        //Столбцы таблицы, включенные в уникальный ключ
        private List<string> _fields;

        private bool _hashable; // = false;
 
        ///// <summary>
        ///// Конструктор
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="owner"></param>

        public UniqueKey(string key, ServerSideObject owner, ServerSideObjectStates state) 
            : base(key, owner, "UniqueKey", state)
        {
            CheckOwnerTypes(owner);
        }

       
        public IEntity Parent
        {
            get { return (IEntity)Instance.Owner; }
        }


        public List<string> Fields
        {
            get { return Instance._fields; }
            set
            {
                CheckNullOrEmpty(value);
                CheckFieldsExists(value);
                CheckUniqFields(value);
                CheckSuchKeyExist(value);
                CheckForbiddenFields(value);
                value.Sort();
                SetInstance._fields = value;
            }
        }

        public bool Hashable
        {
            get { return Instance._hashable; }
            set 
            {
                if (value
                     && ExistOtherHashableUniqueKey()
                    )
                {
                     throw new ArgumentOutOfRangeException("hashable", "Есть другой ключ, у которого вычисляется хэш.");
                }
                SetInstance._hashable = value;

                SinchronizeTableDataAttibute(value);
            }
        }


        public override string FullDBName
        {
            get
            {  
                List<string> sortedFields = this.Fields;
                sortedFields.Sort();
                string ukName = "UK_" + ((Entity)this.Parent).FullDBShortName + String.Join(string.Empty, sortedFields.ToArray());
                
                // TODO: придется вводить shortname, если будет длинное имя и несколько похожих ключей
                return (ukName.Length > 30 ? ukName.Substring(0, 30) : ukName);
            }
        }


        /// <summary>
        /// Русское наименование объекта выводимое в интерфейсе
        /// </summary>
        public override string Caption
        {
            get { return GetterMustUseClone() ? ((CommonObject)CloneObject).Caption : base.Caption; }
            set
            {
                if (SetterMustUseClone())
                    ((CommonObject)CloneObject).Caption = value;
                else
                    base.Caption = value;
            }
        }

        //Возвращает имя триггера для поля с хэшем
        public string HashFieldTriggerDBName
        {
            get
            {
                string name = ((Entity)this.Parent).FullDBShortName + DataAttribute.SystemHashUK.Name;
                name = name.Length > (30 - 6) ? name.Substring(0, (30 - 6)) : name;
                return String.Format("T_{0}_BIU", name);
            }
        }

        #region ServerSideObject


        /// <summary>
        /// Результат сравнения с другим уникальным ключем 
        /// </summary>
        [Flags]
        private enum CompareResult : int
        {
            /// <summary>
            /// Нет отличий
            /// </summary>
            None = 0,

            /// <summary>
            /// Отличие только в наименовании
            /// </summary>
            Caption = 1,

            /// <summary>
            /// Отличается количеством полей
            /// </summary>
            Field = 2,

            /// <summary>
            /// Отличается списком полей
            /// </summary>
            FieldCollection = 4,

            /// <summary>
            ////Отличается признаком "хешировать"
            /// </summary>
            Hashable = 8
        }

        /// <summary>
        /// Сравнивает текущий объект с переданным
        /// </summary>
        /// <returns>0 - объекты идентичны</returns>
        private CompareResult CompareTo(IUniqueKey other)
        {
            CompareResult result = CompareResult.None;

            //Сравниваем имя
            if (this.Caption != other.Caption)
            {
                result = CompareResult.Caption;
            }
            
            //Сравниваем кол-во полей у ключа
            if (this._fields.Count != other.Fields.Count)
            {
                result = result | CompareResult.Field;
            }
            
            //Сравниваем поля между собой
            foreach (string fieldName in this._fields)
            {
                string findedFieldName = other.Fields.Find(delegate(string st) { return st.ToLower() == fieldName.ToLower(); });
                if (findedFieldName == null)
                {
                    result = result | CompareResult.FieldCollection;
                }
            }

            //Сравниваем признак хеширования
            if (this._hashable != other.Hashable)
            {
                result = result | CompareResult.Hashable;
            }

            return result;
        }


        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected UniqueKey Instance
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return (UniqueKey)GetInstance(); }
        }


        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь, для установки значений свойств
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected UniqueKey SetInstance
        {
            get { return SetterMustUseClone() ? (UniqueKey)CloneObject : this; }
        }

        internal override void Save2Xml(XmlNode uniqueKeyNode)
        {
            XmlHelper.SetAttribute(uniqueKeyNode, "objectKey", ObjectKey);
            if (!String.IsNullOrEmpty(Caption))
            {
                XmlHelper.SetAttribute(uniqueKeyNode, "caption", Caption);
            }
            
            if (Hashable)
            {
                XmlHelper.SetAttribute(uniqueKeyNode, "hashable", Boolean.TrueString.ToLower());
            }
            
            foreach (string field in this._fields)
            {
                XmlNode fieldNode = XmlHelper.AddChildNode(uniqueKeyNode, "Field");
                XmlHelper.SetAttribute(fieldNode, "name", field);
                uniqueKeyNode.AppendChild(fieldNode);
            }
        }

        public override IModificationItem GetChanges(IModifiable toObject)
        {
            MinorObjectModificationItem root = new MinorObjectModificationItem(ModificationTypes.Modify, "Измение уникального ключа", this, toObject, null);

            UniqueKey toUniqKey = (UniqueKey)toObject;
            CompareResult compareResult = this.CompareTo(toUniqKey);

            if (compareResult == CompareResult.None)
            {
            }
            
            if (compareResult == CompareResult.Caption)
            {
                PropertyModificationItem changeItem = new PropertyModificationItem("Caption", this.Caption, toUniqKey.Caption, root);
                root.Items.Add(changeItem.Key, changeItem);
            }
            else 
            {
                ModificationItem removeItem = new RemoveMinorModificationItem(String.Format("UK: {0}", this.Caption), this, root);
                root.Items.Add(removeItem.Key, removeItem);
                ModificationItem createItem = new CreateMinorModificationItem(String.Format("UK: {0}", toUniqKey.Caption), this.Parent, toUniqKey, root);
                root.Items.Add(createItem.Key, createItem);
            }

            return root;
        }


        /// <summary>
        /// Формирует скрипт на удаление уникального ключа из БД
        /// </summary>
        internal List<string> GetDropUniqConstraintScript()
        {
            Entity entity = (Entity)this.Parent;
            return ((EntityScriptingEngine)entity.ScriptingEngine).DropUniqueKeyScript(this);
        }

        /// <summary>
        /// Формирует скрипт на создание хэш-поля и триггера его обновления в БД
        /// </summary>
        /// <returns></returns>
        internal List<string> GetDropHashScript()
        {
            Entity entity = (Entity)this.Parent;
            List<string> sql = ((EntityScriptingEngine)entity.ScriptingEngine).DropUniqueKeyHashScript(this);
            return sql;
        }


        /// <summary>
        /// Формирует скрипт на создание уникального ключа в БД
        /// </summary>
        /// <returns></returns>
        internal List<string> GetCreateUniqConstraintScript()
        {
            Entity entity = (Entity)this.Parent;
            return ((EntityScriptingEngine)entity.ScriptingEngine).CreateUniqueKeyScript(this);
        }


        /// <summary>
        /// Формирует скрипт на создание хэш-поля и триггера его обновления в БД
        /// </summary>
        /// <returns></returns>
        internal List<string> GetCreateHashScript()
        {
            Entity entity = (Entity)this.Parent;
            List<string> sql = ((EntityScriptingEngine) entity.ScriptingEngine).CreateUniqueKeyHashScript(this);
            return sql;
        }



        public override string ToString()
        {
            return String.Format("{0} : {1}", this.Caption, base.ToString());
        }
        
        #endregion ServerSideObject


        
        /// <summary>
        /// Проверяет допустимость использования в качестве родителя данный тип объектов(таблиц)
        /// </summary>
        /// <param name="owner"></param>
        private static void CheckOwnerTypes(ServerSideObject owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner", "Не указан владелец.");
            }

            if (!(owner is Entity))
            {
                throw new ArgumentOutOfRangeException("owner", "Недопустимый тип родительского объекта.");
            }

            if (((Entity)owner).ClassType != ClassTypes.clsBridgeClassifier
                && ((Entity)owner).ClassType != ClassTypes.clsDataClassifier
                && ((Entity)owner).ClassType != ClassTypes.Table
                )
            {
                throw new ArgumentOutOfRangeException("owner", "Для данного типа таблиц уникальные ключи не предусмотрены.");
            }

        }

        /// <summary>
        /// Проверяет на NotNull и наличие значений в списке
        /// </summary>
        /// <param name="fields">Перечень полей в UK</param>
        private static void CheckNullOrEmpty(List<string> fields)
        {
            if (fields == null)
            {
                throw new ArgumentNullException("fields", "Параметр не может быть null");
            }

            if (fields.Count == 0)
            {
                throw new ArgumentNullException("fields", "Параметр должен иметь значения");
            }
            
        }


        /// <summary>
        /// Проверяет наличие у таблицы полей, указанных в UK
        /// </summary>
        /// <param name="fields">Перечень полей в UK</param>
        private void CheckFieldsExists(List<string> fields)
        {
            IEntity entity = this.Parent;
            
            //Проходим по всем полям из списка
            foreach (string field in fields)
            {
                bool found = false;
                
                //Проверяем наличие среди атрибутов этой же таблицы
                foreach (IDataAttribute dataAttribute in entity.Attributes.Values)
                { 
                    if (dataAttribute.Name.Equals(field,StringComparison.CurrentCultureIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    //Проверяем среди ассоциаций, которые висят на этой же таблице
                    foreach (IEntityAssociation association in entity.Associations.Values)
                    {  
                        if ( association.FullDBName.Equals(field,StringComparison.CurrentCultureIgnoreCase))
                        {
                           found = true;
                           break;
                        }
                    }

                    if (!found)
                    {
                        throw new ArgumentOutOfRangeException("fields", String.Format("Поле {0} не принадлежит данной таблице.", field));
                    }
                }

            }
        }


        /// <summary>
        /// Проверка уникальности полей в списке
        /// </summary>
        /// <param name="fields"></param>
        private static void CheckUniqFields(List<string> fields)
        {
            Dictionary<string, string> uniqFields = new Dictionary<string, string>(fields.Count);
            
            foreach (string field in fields)
            {
                try
                {
                    uniqFields.Add(field.ToLower(), field);
                }
                catch (ArgumentException)
                {
                    throw new ArgumentOutOfRangeException("fields", "Поля должны быть уникальными");
                }
            }

        }

        /// <summary>
        /// Проверяет наличие уже существующего ключа с таким же набором полей (без учета их порядка!)
        /// </summary>
        private void CheckSuchKeyExist(List<string> fields)
        {
            if (this.Parent.UniqueKeys != null)
            {
                //Проходим по всем уникальным ключам родительской таблицы
                foreach (IUniqueKey otherUniqueKey in this.Parent.UniqueKeys.Values)
                {
                    if (!otherUniqueKey.Equals(this))
                    {
                        //Сравниваем набор полей на их количество
                        List<string> otherFields = otherUniqueKey.Fields;
                        if (otherFields.Count == fields.Count)
                        {
                            //Сравниваем наборы полей по содержимому без учета их порядка
                            bool equivalent = true;
                            foreach (string field in fields)
                            {
                                string findedField =
                                    otherFields.Find(delegate(string st) { return st.ToLower() == field.ToLower(); });
                                if (findedField == null)
                                {
                                    equivalent = false;
                                    break;
                                }
                            }
                            if (equivalent)
                            {
                                throw new ArgumentOutOfRangeException("fields",
                                                                      "Существует другой уникальный ключ с таким же набором полей!");
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Проверяет отсутствие в списке запрещенных полей
        /// </summary>
        /// <param name="fields"></param>
        private static void CheckForbiddenFields(List<string> fields)
        {
            foreach (string field in fields)
            {
                if (field == DataAttribute.HashUKColumnName)
                {
                    throw new ArgumentOutOfRangeException("fields", "Поле с хэшем не может быть включено в уникальный ключ!");
                }
            }
        }


        /// <summary>
        /// Проверяет включено ли хеширование только у данного ключа
        /// </summary>
        private bool ExistOtherHashableUniqueKey()
        {
            IEntity entity = this.Parent;
            foreach (IUniqueKey otherUniqueKey in entity.UniqueKeys.Values)
            {
              if ( (! otherUniqueKey.Equals(this)) 
                   && otherUniqueKey.Hashable
                  )
              {
                  return true;
              }  
            }
            return false;
        }

        /// <summary>
        /// Проверяет, содержится ли в уникальном ключе данное поле
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        internal bool ContainField(string field)
        {
            string findedFieldName = Fields.Find(delegate(string st) { return st.ToLower() == field.ToLower(); });
            if (findedFieldName == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Добавляет,удаляет или оставляет у родительской таблицы системное поле HashUK для хранения хэша
        /// </summary>
        /// <param name="hashable"></param>
        protected void SinchronizeTableDataAttibute (bool hashable)
        {
            IEntity entity = this.Parent;
            
            if (hashable
                && !entity.Attributes.ContainsKey(GetKey(DataAttribute.SystemHashUK.ObjectKey, DataAttribute.SystemHashUK.Name))
                )
            {
                entity.Attributes.Add(DataAttribute.SystemHashUK);
            }
            
            //Необходимо удалить системное поле, если оно не используется другим ключем
            else if (!hashable 
                     && !ExistOtherHashableUniqueKey()
                     && entity.Attributes.ContainsKey(GetKey(DataAttribute.SystemHashUK.ObjectKey, DataAttribute.SystemHashUK.Name))
                    )
            {
                entity.Attributes.Remove(GetKey(DataAttribute.SystemHashUK.ObjectKey, DataAttribute.SystemHashUK.Name));
            }

        }

    }


}


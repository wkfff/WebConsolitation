using System;
using System.Data;

using Krista.FM.Common;
using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;
using System.Collections.Generic;


namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Коллекция классификаторов
    /// </summary>
    internal class EntityCollection<TItemIntf> : MajorObjecModifiableCollection<string, TItemIntf>, IEntityCollection<TItemIntf> where TItemIntf : IEntity
    {
        public EntityCollection(ServerSideObject owner, ServerSideObjectStates state)
            : base(owner, state)
        {
        }

       
        public override IServerSideObject Lock()
        {
            Package clonePackage = (Package)Owner.Lock();
            return (ServerSideObject)clonePackage.Classes;
        }

/*        private bool useExtendedSearch = true;

        public override bool ContainsKey(string key)
        {
            if (base.ContainsKey(key))
            {
                return true;
            }
            else
            {
                if (useExtendedSearch)
                {
                    foreach (TItemIntf item in list.Values)
                    {
                        if (item.FullName == key)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public override TItemIntf this[string key]
        {
            get
            {
                if (base.ContainsKey(key))
                {
                    return base[key];
                }
                else
                {
                    if (useExtendedSearch)
                    {
                        foreach (TItemIntf item in list.Values)
                        {
                            if (item.FullName == key)
                            {
                                return item;
                            }
                        }
                    }

                    throw new KeyNotFoundException(String.Format("Сущность с именем \"{0}\" не найдена в коллекции.", key));
                }
            }
        }
        */
		
		

        public virtual TItemIntf CreateItem(ClassTypes classType, SubClassTypes subClassType)
        {
            LogicalCallContextData callerContext = LogicalCallContextData.GetContext();
            TItemIntf entity;
            try
            {
                string semantic;
                if (classType == ClassTypes.clsFixedClassifier)
                    semantic = "FX";
                else if ((classType == ClassTypes.clsFactData) || (classType == ClassTypes.Table))
                    semantic = "F";
                else
                    semantic = "KD";

            	string newObjectKey = Guid.NewGuid().ToString();
				string name = GetNewName(newObjectKey);

                entity = (TItemIntf)(IEntity)SchemeClass.EntityFactory.CreateEntity(
                    newObjectKey, Owner, -1, semantic, name, 
                    classType, subClassType, String.Empty, ServerSideObjectStates.New
                    );

                ((Entity)(IEntity)entity).DbObjectState = DBObjectStateTypes.New;
                entity.Caption = String.Format("Новый класс {0}", Count + 1);
                entity.Description = String.Empty;
                ((Entity)(IEntity)entity).Configuration = entity.ConfigurationXml;
                ((Entity)(IEntity)entity).Initialize();
            }
            finally
            {
                LogicalCallContextData.SetContext(callerContext);
            }
            Add(entity.ObjectKey, entity);

            // для классификаторов инициализируем иерархию
            if (entity is Classifier)
                ((Classifier)(IEntity)entity).InitializeDefaultHierarchy();

            return entity;
        }

        /// <summary>
        /// Ищет объект в коллекции по ключу (ключем может быть как GUID так и FullName).
        /// </summary>
        /// <param name="item">Объект для поиска.</param>
        /// <returns>Если объект найден, то возвращает true.</returns>
        protected override TItemIntf ContainsObject(KeyValuePair<string, TItemIntf> item)
        {
            string key = item.Key;

            try
            {
                new Guid(item.Key);
                if (!ContainsKey(item.Key))
                {
                    foreach (TItemIntf entity in Values)
                    {
                        if (entity.FullName == item.Value.FullName)
                            return entity;
                    }
                }
            }
            catch (FormatException)
            {
                key = item.Key;
            }

            if (ContainsKey(key))
                return this[key];
        	return (TItemIntf)(object)null;
        }

        #region ICollection2DataTable Members

        public virtual DataTable GetDataTable()
        {
            DataColumn[] columns = new DataColumn[6];
            columns[0] = new DataColumn("FullName", typeof(String));
            columns[1] = new DataColumn("ClassType", typeof(ClassTypes));
            columns[2] = new DataColumn("Semantic", typeof(String));
            columns[3] = new DataColumn("Caption", typeof(String));
            columns[4] = new DataColumn("Description", typeof(String));
            columns[5] = new DataColumn("ObjectKey", typeof(String));

            DataTable dt = new DataTable(GetType().Name);
            dt.Columns.AddRange(columns);

            foreach (TItemIntf item in Values)
            {
                if (((Entity)(IEntity)item).CurrentUserCanViewThisObject())
                {
                    DataRow row = dt.NewRow();
                    row[0] = KeyIdentifiedObject.GetKey(item.ObjectKey, item.FullName);
                    row[1] = item.ClassType;
                    row[2] = ((Entity)(IEntity)item).SemanticCaption;
                    row[3] = item.Caption;
                    row[4] = item.Description;
                    row[5] = KeyIdentifiedObject.GetKey(item.ObjectKey, item.FullName);
                    dt.Rows.Add(row);
                }
            }

            return dt;
        }

        #endregion

        /// <summary>
        /// Возвращает аттрибут из коллекции по ключу или по паименованию.
        /// </summary>
        /// <param name="collection">Коллекция атрибутов.</param>
        /// <param name="key">Уникальный ключ объекта.</param>
        /// <param name="name">Наименование объекта.</param>
        [Obsolete]
        internal static TItemIntf GetByKeyName(IEntityCollection<TItemIntf> collection, string key, string name)
        {
            if (collection.ContainsKey(key))
            {
                return collection[key];
            }

			return (TItemIntf)(object)null;
        }
    }
}

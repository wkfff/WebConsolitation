using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Classes.Visitors;
using Krista.FM.Server.Scheme.Modifications;

using Krista.FM.ServerLibrary;
using Krista.FM.Server.Scheme.Classes.PresentationLayer;


namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Класс представляющий UML-пакет
    /// </summary>
    internal partial class Package : CommonDBObject, IPackage, IValidable
    {
        internal const string SystemPackageName = "Системные объекты";

        #region Поля

        /// <summary>
        /// Определяет необходимо ли сохранять файл на диск
        /// </summary>
        private bool neelFlash = false;

        /// <summary>
        /// Локальный путь к файлу пакета, если не указан, то пакет встроенный
        /// </summary>
        private string privatePath;

        /// <summary>
        /// Коллекция пакетов
        /// </summary>
        private PackageCollection packages;

        /// <summary>
        /// Классы пакета
        /// </summary>
        private EntityCollection<IEntity> classes;

        /// <summary>
        /// Ассоциации пакета
        /// </summary>
        private EntityAssociationCollection associations;

        /// <summary>
        /// Документы пакета
        /// </summary>
        private DocumentCollection documents;

        #endregion Поля

        /// <summary>
        /// Класс представляющий UML-пакет
        /// </summary>
        public Package(string key, ServerSideObject owner, string name, ServerSideObjectStates state)
            : base(key, owner, "Package", name, state, null)
        {
            //privatePath = name + ".xml";
            packages = new PackageCollection(this, state);
            classes = new EntityCollection<IEntity>(this, state);
            associations = new EntityAssociationCollection(this, state);
            documents = new DocumentCollection(this, state);
        }

        internal static Package CreatePackage(string key, ServerSideObject owner, int ID, string name, string configuration, ServerSideObjectStates state)
        {
            Package package = new Package(key, owner, name, state);
            package.ID = ID;
            package.Configuration = configuration;
            return package;
        }

        /// <summary>
        /// Возвращает локальный путь относительно сземы в котором находится пакет
        /// </summary>
        /// <returns>Путь к каталогу</returns>
        public string GetLocalPath()
        {
            if (Name == "Корневой пакет")
            {
                return "Packages";
            }
            else if (!String.IsNullOrEmpty(privatePath))
            {
                return Path.GetDirectoryName(String.Format("{0}\\{1}", Parent.GetLocalPath(), privatePath));
            }
            else
            {
                return Path.GetDirectoryName(String.Format("{0}\\{1}.xml", Parent.GetLocalPath(), Name));
            }
        }

        /// <summary>
        /// Возвращает путь к каталогу в котором находится пакет
        /// </summary>
        /// <returns>Путь к каталогу</returns>
        public string GetDir()
        {
            return SchemeClass.Instance.BaseDirectory + '\\' + GetLocalPath();
        }

        #region ServerSideObject

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected new Package Instance
        {
            [DebuggerStepThrough]
            get { return (Package)GetInstance(); }
        }

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь, для установки значений свойств
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected new Package SetInstance
        {
            get
            {
                if (SetterMustUseClone())
                    return (Package)CloneObject;
                else
                    return this;
            }
        }

        public override object Clone()
        {
            Package clone = (Package)base.Clone();

            clone.packages = (PackageCollection)packages.Clone();
            clone.classes = (EntityCollection<IEntity>)classes.Clone();
            clone.associations = (EntityAssociationCollection)associations.Clone();
            clone.documents = (DocumentCollection)documents.Clone();

            return clone;
        }

        public override IServerSideObject Lock()
        {
            if (Name == SystemPackageName)
                return this;

            if (IsEndPoint)
            {
                // TODO Запросить у пользователя комментарии
                CheckOut(String.Empty);

                return base.Lock() as Package;
            }
            else
            {
                Package clonePackage = (Package)base.Lock();
                return (ServerSideObject)clonePackage.Packages[this.ObjectKey];
            }
        }

        /// <summary>
        /// Снимает блокировку с объекта
        /// </summary>
        public override void Unlock()
        {
            if (IsLocked || State == ServerSideObjectStates.New)
            {
                packages.Unlock();
                classes.Unlock();
                associations.Unlock();
                documents.Unlock();
                neelFlash = false;
                base.Unlock();

                if (Owner != null)
                    Owner.Unlock();
            }
        }

        /// <summary>
        /// Состояние серверного объекта во времени его существования
        /// </summary>
        public override ServerSideObjectStates State
        {
            [DebuggerStepThrough]
            get { return base.State; }
            set
            {
                if (base.State != value)
                {
                    base.State = value;
                    packages.State = value;
                    classes.State = value;
                    associations.State = value;
                    documents.State = value;
                }
            }
        }

        #endregion ServerSideObject

        /// <summary>
        /// Устанавливает английское наименование
        /// </summary>
        /// <param name="oldValue">Старое наименование</param>
        /// <param name="value">Новое наименование</param>
        protected override void SetFullName(string oldValue, string value)
        {
            if (State == ServerSideObjectStates.New)
            {
                Package package = (Package)Owner;

                Package newObject = Instance;
                Package oldObject = (Package)package.Packages[oldValue];
                try
                {
                    package.Packages.Remove(oldValue);
                    package.Packages.Add(
                        GetKey(newObject.ObjectKey, newObject.FullName),
                        newObject);
                }
                catch (Exception e)
                {
                    package.Packages.Add(oldValue, oldObject);
                    throw new Exception(e.Message, e);
                }
            }
        }

        #region Инициализация

        /// <summary>
        /// Инициализация свойств пакета (Наименование, локальный путь и т.п.)
        /// </summary>
        /// <param name="doc">XML-Документ с конфигурацией пакета</param>
        private void InitializeProperties(XmlNode doc)
        {
            XmlNode xmlPackage = doc.SelectSingleNode("/DatabaseConfiguration/Package");
            if (xmlPackage.Attributes["name"] != null)
            {
                if (String.IsNullOrEmpty(Name))
                    Name = xmlPackage.Attributes["name"].Value;
            }
            else if (xmlPackage.Attributes["privatePath"] != null)
            {
                privatePath = xmlPackage.Attributes["privatePath"].Value;
            }
            else
            {
                if (String.IsNullOrEmpty(Name))
                    throw new Exception("У пакета отсутствует обязательный атрибут name или privatePath");
            }
        }

        private void InitializePackageCollection(XmlNode doc)
        {
            packages.Initialize(doc.SelectNodes("/DatabaseConfiguration/Package/Packages/Package"));
        }

        private void LoadFromXMLClass(XmlNodeList xmlClasses, ClassTypes classType)
        {
            foreach (XmlNode xmlObject in xmlClasses)
            {
                string objectKey = Guid.Empty.ToString();
                if (xmlObject.Attributes["objectKey"] != null)
                {
                    objectKey = xmlObject.Attributes["objectKey"].Value;
                }

                Entity entity = (Entity)SchemeClass.EntityFactory.CreateEntity(
                    objectKey,
                    this, -1,
                    xmlObject.Attributes["semantic"].Value,
                    xmlObject.Attributes["name"].Value,
                    classType,
                    SchemeClass.GetTakeMethodSubClassTypes(xmlObject.Attributes["takeMethod"]),
                    SchemeClass.Enclose2DatabaseConfiguration(xmlObject.OuterXml),
                    ServerSideObjectStates.New);

                string sourceKey = GetKey(entity.ObjectKey, entity.FullName);

                entity.Initialize();
                entity.DbObjectState = DBObjectStateTypes.New;

                Classes.Add(sourceKey, entity);
            }
        }

        private void InitializeEntityCollection(XmlNode doc)
        {
            XmlNode xmlClasses = doc.SelectSingleNode("/DatabaseConfiguration/Package/Classes");
            if (xmlClasses != null)
            {
                LoadFromXMLClass(xmlClasses.SelectNodes("FixedCls"), ClassTypes.clsFixedClassifier);
                LoadFromXMLClass(xmlClasses.SelectNodes("BridgeCls"), ClassTypes.clsBridgeClassifier);
                LoadFromXMLClass(xmlClasses.SelectNodes("DataCls"), ClassTypes.clsDataClassifier);
                LoadFromXMLClass(xmlClasses.SelectNodes("DataTable"), ClassTypes.clsFactData);
                LoadFromXMLClass(xmlClasses.SelectNodes("Table"), ClassTypes.Table);
				LoadFromXMLClass(xmlClasses.SelectNodes("DocumentEntity"), ClassTypes.DocumentEntity);
			}
        }

        private void InitializeDocumentCollection(XmlNode doc)
        {
            XmlNode xmlNodes = doc.SelectSingleNode("/DatabaseConfiguration/Package/Documents");
            if (xmlNodes != null)
            {
                foreach (XmlNode xmlNode in xmlNodes.SelectNodes("Document"))
                {
                    DocumentTypes documentType;
                    switch (xmlNode.SelectSingleNode("@type").Value)
                    {
                        case "Diagram": documentType = DocumentTypes.Diagram; break;
						case "DocumentEntityDiagram": documentType = DocumentTypes.DocumentEntityDiagram; break;
						case "DocumentURL": documentType = DocumentTypes.URL; break;
                        default: throw new Exception(String.Format("Неизвестный тип документа {0}", xmlNode.SelectSingleNode("RoleData/@type").Value));
                    }

                    string objectKey = Guid.Empty.ToString();
                    if (xmlNode.Attributes["objectKey"] != null)
                    {
                        objectKey = xmlNode.Attributes["objectKey"].Value;
                    }

                    Document document = new Document(
                        objectKey,
                        this,
                        -1, xmlNode.SelectSingleNode("@name").Value,
                        documentType,
                        ServerSideObjectStates.New
                    );

                    string sourceKey = GetKey(document.ObjectKey, document.FullName);

                    document.Configuration = SchemeClass.Enclose2DatabaseConfiguration(xmlNode.OuterXml);
                    document.DbObjectState = DBObjectStateTypes.New;
                    Documents.Add(sourceKey, document);
                }
            }
        }

        private void LoadFromXMLAssociations(XmlNodeList xmlAssociations, AssociationClassTypes associationClassType)
        {
            foreach (XmlNode xmlObject in xmlAssociations)
            {
                string name = xmlObject.SelectSingleNode("RoleData/@name").Value + "." + xmlObject.Attributes["name"].Value;
                string semanticName = xmlObject.SelectSingleNode("RoleData/@semantic").Value;

                string nameRoleData = SchemeClass.ExtractRoleName(xmlObject, "RoleData");
                string nameRoleBridge = SchemeClass.ExtractRoleName(xmlObject, "RoleBridge");

                XmlNode keyRoleDataNode = xmlObject.SelectSingleNode("RoleData/@objectKey");
                string keyRoleData = keyRoleDataNode != null ? keyRoleDataNode.Value : nameRoleData;

                XmlNode keyRoleBridgeNode = xmlObject.SelectSingleNode("RoleBridge/@objectKey");
                string keyRoleBridge = keyRoleBridgeNode != null ? keyRoleBridgeNode.Value : nameRoleBridge;

                Entity ra = GetEntityByName(keyRoleData, nameRoleData); // GetEntityByName(keyRoleData);
                Entity rb = GetEntityByName(keyRoleBridge, nameRoleBridge); // GetEntityByName(keyRoleBridge);

                if (ra == null)
                    throw new Exception(String.Format("Не найден объект {0} на который ссылается RoleData в ассоциации {1}.{2}", nameRoleData, semanticName, name));
                if (rb == null)
                    throw new Exception(String.Format("Не найден объект {0} на который ссылается RoleBridge в ассоциации {1}.{2}", nameRoleBridge, semanticName, name));

                if (ra.ObjectKey.Equals(rb.ObjectKey))
                {
                    associationClassType = AssociationClassTypes.BridgeBridge;
                }

                // Проверка на совместимость указанных ролей
                AssociationClassTypes newAssociationClassType = SchemeClass.CheckRolesClasses(associationClassType, ra.ClassType, rb.ClassType, String.Format("{0}.{1}", semanticName, name));

                XmlNode keyNode = xmlObject.Attributes["objectKey"];
                string key = keyNode != null ? keyNode.Value : Guid.Empty.ToString();

                EntityAssociation entityAssociation = (EntityAssociation)SchemeClass.EntityAssociationFactory.CreateAssociation(
                    key,
                    this, -1,
                    semanticName,
                    name,
                    newAssociationClassType,
                    ra,
                    rb,
                    SchemeClass.Enclose2DatabaseConfiguration(xmlObject.OuterXml),
                    ServerSideObjectStates.New);

                string sourceKey = GetKey(entityAssociation.ObjectKey, entityAssociation.FullName);

                entityAssociation.Initialize();
                entityAssociation.DbObjectState = DBObjectStateTypes.New;

                Associations.Add(sourceKey, entityAssociation);
            }
        }

        public IEntity FindEntityByName(string name)
        {
            if (Classes.ContainsKey(name))
                return Classes[name];
            else
            {
                IEntity entity = null;
                foreach (Package item in Packages.Values)
                {
                    if (item.Name == this.Name)
                        continue;

                    entity = item.FindEntityByName(name);
                    if (entity != null)
                        break;
                }
                return entity;
            }
        }

        /// <summary>
        /// Ищет объект в текущем пакете и в подпакетах.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEntity FindEntityByKeyName(string key, string name)
        {
            if (Classes.ContainsKey(key))
                return Classes[key];
            else if (Classes.ContainsKey(name))
                return Classes[name];
            else
            {
                IEntity entity = null;
                foreach (Package item in Packages.Values)
                {
                    if (item.Name == this.Name)
                        continue;

                    entity = item.FindEntityByKeyName(key, name);
                    if (entity != null)
                        break;
                }
                return entity;
            }
        }

        public IEntityAssociation FindAssociationByName(string name)
        {
            if (Associations.ContainsKey(name))
                return Associations[name];
            else
            {
                IEntityAssociation association = null;
                foreach (Package item in Packages.Values)
                {
                    if (item.Name == this.Name)
                        continue;

                    association = item.FindAssociationByName(name);
                    if (association != null)
                        break;
                }
                return association;
            }
        }

        private Entity GetEntityByName(string key, string name)
        {
            {// TODO: Убрать эту заглушку после выпуска версии 2.4.1
                switch (key)
                {
                    case "fx.Date.YearDayUNV": key = SystemSchemeObjects.YearDayUNV_ENTITY_KEY; break;
                    case "fx.Date.YearDay": key = SystemSchemeObjects.YearDay_ENTITY_KEY; break;
                    case "fx.Date.YearMonth": key = SystemSchemeObjects.YearMonth_ENTITY_KEY; break;
                    case "fx.Date.Year": key = SystemSchemeObjects.Year_ENTITY_KEY; break;
                }
                switch (name)
                {
                    case "fx.Date.YearDayUNV": name = SystemSchemeObjects.YearDayUNV_ENTITY_KEY; break;
                    case "fx.Date.YearDay": name = SystemSchemeObjects.YearDay_ENTITY_KEY; break;
                    case "fx.Date.YearMonth": name = SystemSchemeObjects.YearMonth_ENTITY_KEY; break;
                    case "fx.Date.Year": name = SystemSchemeObjects.Year_ENTITY_KEY; break;
                }
            }

            // Сначала ищем в текущем пакете
            if (Classes.ContainsKey(key))
                return (Entity)Classes[key];
            if (Classes.ContainsKey(name))
                return (Entity)Classes[name];
            else
            {
                // Если в текущем не нашли, то ишем в соседних "братьях"
                Entity entity = null;

                Package parentPackage = Parent;
                while (entity == null)
                {
                    foreach (Package item in parentPackage.Packages.Values)
                    {
                        if (item.Name == this.Name)
                            continue;

                        entity = (Entity)item.FindEntityByKeyName(key, name);
                        if (entity != null)
                            break;
                    }
                    if (parentPackage.Parent == null)
                        break;
                    
                    // Спускаемся на уровень ниже к потомкам
                    parentPackage = parentPackage.Parent;
                }
                return entity;
            }
        }

        private void InitializeAssociationCollection(XmlNode doc)
        {
            XmlNode xmlAssociations = doc.SelectSingleNode("/DatabaseConfiguration/Package/Associations");
            if (xmlAssociations != null)
            {
                LoadFromXMLAssociations(xmlAssociations.SelectNodes(BridgeAssociation.TagElementName), AssociationClassTypes.Bridge);
                LoadFromXMLAssociations(xmlAssociations.SelectNodes(Association.TagElementName), AssociationClassTypes.Link);
                LoadFromXMLAssociations(xmlAssociations.SelectNodes(MasterDetailAssociation.TagElementName), AssociationClassTypes.MasterDetail);
            }
        }

        public void Initialize(string fileName)
        {
            // Читаем содержимое текстового файла
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(fileName, Encoding.Default))
            {
                while (sr.Peek() >= 0)
                {
                    sb.Append(sr.ReadLine());
                }
            }

            Configuration = sb.ToString();

            // Инициализируем пакет
            Initialize();
        }

        /// <summary>
        /// Инициализация пакета по его XML описанию
        /// </summary>
        internal override XmlDocument Initialize()
        {
            Trace.TraceInformation("Инициализация пакета \"{0}\" {1}", this.FullName, privatePath);
            Trace.Indent();
            try
            {
                if (String.IsNullOrEmpty(Configuration))
                    return null;

                XmlDocument doc = base.Initialize();

                InitializeDeveloperDescription(doc, "Package");
                InitializeProperties(doc);

                InitializeEntityCollection(doc);
                InitializeAssociationCollection(doc);
                InitializePackageCollection(doc);
                InitializeDocumentCollection(doc);

                return doc;
            }
            finally
            {
                Trace.Unindent();
            }
        }

        internal override XmlDocument PostInitialize()
        {
            foreach (EntityAssociation item in this.Associations.Values)
            {
                item.PostInitialize();
            }

            foreach (Package item in this.Packages.Values)
            {
                item.PostInitialize();
            }

            foreach (Entity item in this.Classes.Values)
            {
                if (item.ClassType == ClassTypes.clsDataClassifier || item.ClassType == ClassTypes.clsFactData)
                item.PostInitializePresentation();
            }

            return null;
        }

        /// <summary>
        /// Инициализация списка сущностей из базы данных
        /// </summary>
		private void InitializeEntities(DataTable metaObjects)
        {
			DataRow[] rows = metaObjects.Select("SubClass <> 4 and RefPackages = " + ID);
			foreach (DataRow row in rows)
			{
                Entity entity = (Entity)SchemeClass.EntityFactory.CreateEntity(
					Convert.ToString(row["ObjectKey"]),
					this,
					Convert.ToInt32(row["ID"]),
					Convert.ToString(row["Semantic"]),
					Convert.ToString(row["Name"]),
					(ClassTypes)Convert.ToInt32(row["Class"]),
					(SubClassTypes)Convert.ToInt32(row["SubClass"]),
					Convert.ToString(row["Configuration"]),
					ServerSideObjectStates.Consistent
					);

				try
				{
					entity.Initialize();

					entity.DbObjectState = DBObjectStateTypes.InDatabase;

					classes.Add(GetKey(entity.ObjectKey, entity.FullName), entity);
				}
				catch (Exception e)
				{
					throw new ServerException(String.Format("Ошибка при инициализации объекта \"{0}\"", entity.FullName), e);
				}
			}
        }

        /// <summary>
        /// Инициализация списка ассоциаций из базы данных
        /// </summary>
		private void InitializeAssociations(DataTable metaLinks)
        {
			DataRow[] rows = metaLinks.Select("RefPackages = " + ID);
			foreach (DataRow row in rows)
			{
				try
				{
                    EntityAssociation association = (EntityAssociation)SchemeClass.EntityAssociationFactory.CreateAssociation(
						Convert.ToString(row["ObjectKey"]),
						this,
						Convert.ToInt32(row["ID"]),
						Convert.ToString(row["Semantic"]),
						Convert.ToString(row["Name"]),
						(AssociationClassTypes)Convert.ToInt32(row["Class"]),
						GetEntityByName(GetKey(Convert.ToString(row["RoleAObjectKey"]), Convert.ToString(row["RoleAName"])), Convert.ToString(row["RoleAName"])),
						GetEntityByName(GetKey(Convert.ToString(row["RoleBObjectKey"]), Convert.ToString(row["RoleBName"])), Convert.ToString(row["RoleBName"])),
						Convert.ToString(row["Configuration"]),
						ServerSideObjectStates.Consistent
						);

					association.Initialize();
					association.DbObjectState = DBObjectStateTypes.InDatabase;
					associations.Add(GetKey(association.ObjectKey, association.FullName), association);
				}
				catch (Exception e)
				{
					throw new Exception(String.Format("Ошибка при инициализации ассоциации {0} -> {1}: {2}", Convert.ToString(row["RoleAName"]), Convert.ToString(row["RoleBName"]), e.Message), e);
				}
			}
        }

        /// <summary>
        /// Инициализация списка документов из базы данных
        /// </summary>
        private void InitializeDocuments(DataTable metaDocuments)
        {
            DataRow[] rows = metaDocuments.Select("RefPackages = " + ID);
            foreach (DataRow row in rows)
            {
                try
                {
                    Document document = new Document(
                        Convert.ToString(row["ObjectKey"]),
                        this,
                        Convert.ToInt32(row["ID"]),
                        Convert.ToString(row["Name"]),
                        (DocumentTypes)Convert.ToInt32(row["DocType"]),
                        ServerSideObjectStates.Consistent);

                    document.DbObjectState = DBObjectStateTypes.InDatabase;
                    documents.Add(GetKey(document.ObjectKey, document.FullName), document);
                }
                catch(Exception e)
                {
                    throw new Exception(String.Format("Ошибка при инициализации документа {0}", Convert.ToString(row["Name"])));
                }
            }
        }

        /// <summary>
        /// Инициализация пакета из базы данных
        /// </summary>
		internal void InitializeFromDB(DataSet metaData)
        {
			Trace.TraceInformation("Инициализация пакета \"{0}\"", FullName);
			Trace.Indent();
			InitializeEntities(metaData.Tables[1]);
			InitializeAssociations(metaData.Tables[2]);
            try
            {
				DataRow[] rows = metaData.Tables[0].Select("RefParent = " + ID);

				foreach (DataRow row in rows)
				{
					Package package = CreatePackage(
						Convert.ToString(row["ObjectKey"]), 
						this,
						Convert.ToInt32(row["ID"]),
						Convert.ToString(row["Name"]), 
						String.Empty, ServerSideObjectStates.Consistent);

					if (!(row["PrivatePath"] is DBNull))
						package.PrivatePath = Convert.ToString(row["PrivatePath"]);

					packages.Add(GetKey(package.ObjectKey, package.FullName), package);
					package.InitializeFromDB(metaData);
					package.DbObjectState = DBObjectStateTypes.InDatabase;
				}

                InitializeDocuments(metaData.Tables["MetaDocuments"]);
			}
            finally
			{
				Trace.Unindent();
			}
        }

        #endregion Инициализация

        /// <summary>
        /// Возвращает максимальное значение поля OrderBy
        /// </summary>
        /// <param name="db">Объект доступа к базе данных</param>
        /// <returns>Возвращает максимальное значение поля OrderBy</returns>
        private static int GetMaxOrderNumber(Database db)
        {
            return Convert.ToInt32(db.ExecQuery("select max(OrderBy) from MetaPackages", QueryResultTypes.Scalar));
        }

        /// <summary>
        /// Записывает в каталог метаданных информацию о реляционной таблице
        /// </summary>
        protected void InsertMetaData(ModificationContext context)
        {
            Database db = context.Database;

            // получаем значение генератора
            int resultID;
            if (this.ID <= 0)
                resultID = db.GetGenerator("g_MetaPackages");
            else
                resultID = this.ID;



            object parentID = DBNull.Value;
            if (Parent.Name != "Корневой пакет")
                parentID = Parent.ID;

            // вставляем запись в каталог метаданных
            db.ExecQuery("insert into MetaPackages (ID, ObjectKey, Name, BuiltIn, PrivatePath, RefParent, OrderBy, Configuration) values (?, ?, ?, ?, ?, ?, ?, ?)", QueryResultTypes.NonQuery,
                db.CreateParameter("ID", resultID),
                db.CreateParameter("ObjectKey", this.ObjectKey),
                db.CreateParameter("Name", this.Name),
                db.CreateParameter("BuiltIn", 0),
                db.CreateParameter("PrivatePath", String.IsNullOrEmpty(PrivatePath) ? DBNull.Value : (object)PrivatePath, DbType.AnsiString),
                db.CreateParameter("RefParent", parentID, DbType.Int32),
                db.CreateParameter("OrderBy", GetMaxOrderNumber(db) + 1, DbType.Int32),
                db.CreateParameter("Configuration", this.Configuration, DbType.AnsiString));

            this.ID = resultID;
        }

        /// <summary>
        /// Удаляет из каталога метаданных информацию о пакете
        /// </summary>
        private void RemoveMetaData(ModificationContext context)
        {
            Database db = context.Database;

            db.ExecQuery("delete from MetaPackages where ID = ?", QueryResultTypes.NonQuery,
                db.CreateParameter("ID", this.ID));
        }

        /// <summary>
        /// Сохранение XML-конфигурации в базе данных
        /// </summary>
        internal override void SaveConfigurationToDatabase(ModificationContext context)
        {
            IDatabase db = context.Database;

            string confXmlString = ConfigurationXml;
            confXmlString = confXmlString.Replace("<DatabaseConfiguration xmlns=\"xmluml\">", "<DatabaseConfiguration>");
            // TODO: валидация XML перед сохранением в БД 
            // вставляем запись в каталог метаданных
            db.ExecQuery(
                "update MetaPackages set Configuration = ?, ObjectKey = ? where ID = ?",
                QueryResultTypes.NonQuery,
                db.CreateParameter("Config", confXmlString, DbType.AnsiString),
                db.CreateParameter("ObjectKey", ObjectKey, DbType.AnsiString),
                db.CreateParameter("ID", this.ID));

            this.Configuration = confXmlString;
        }

        /// <summary>
        /// Создание метаданных в базе данных и кубах
        /// </summary>
        internal override void Create(ModificationContext context)
        {
            bool updatingState = InUpdating;
            InUpdating = true;
            try
            {
                Trace.TraceInformation("Создание {0} {1}", this, DateTime.Now);
                context.SendMessage(String.Format(this.FullName));
                Trace.Indent();

                /*try
                {
                    this.Drop(context);
                }
                catch (Exception e)
                {
                    Trace.TraceError("При удалении структур пакета \"{0}\" произошла ошибка: {1}", this, Krista.Diagnostics.KristaDiagnostics.ExpandException(e));
                }
                */
                base.Create(context);

                if (!Parent.Packages.ContainsKey(GetKey(this.ObjectKey, this.Name)))
                    Parent.Packages.Add(GetKey(this.ObjectKey, this.Name), this);

                InsertMetaData(context);

                foreach (Entity item in classes.Values)
                    item.Create(context);

                // Сначала создаем ассоциации сопоставления
                foreach (EntityAssociation item in associations.Values)
                {
                    if (item.AssociationClassType == AssociationClassTypes.Bridge)
                    {
                        item.Create(context);
                    }
                }

                // затем все остальные
                foreach (EntityAssociation item in associations.Values)
                {
                    if (item.AssociationClassType != AssociationClassTypes.Bridge)
                    {
                        item.Create(context);
                    }
                }
                
                foreach (Document item in documents.Values)
                    item.Create(context);

                // Создаем встроенные пакеты
                foreach (Package item in packages.Values)
                {
                    if (String.IsNullOrEmpty(item.PrivatePath))
                    {
                        item.Create(context);
                    }
                }

                DbObjectState = DBObjectStateTypes.InDatabase;

                // Создаем внешние дочерние подпакеты
                foreach (Package item in packages.Values)
                {
                    if (!String.IsNullOrEmpty(item.PrivatePath))
                    {
                        item.Create(context);
                    }
                }
            }
            catch (Exception e)
            {
                if (DbObjectState != DBObjectStateTypes.InDatabase)
                {
                    if (Parent.Packages.ContainsKey(GetKey(this.ObjectKey, this.Name)))
                        Parent.Packages.Remove(GetKey(this.ObjectKey, this.Name));

                    RemoveMetaData(context);
                }
                Trace.TraceError(e.Message);
                throw new Exception(e.Message, e);
            }
            finally
            {
                Trace.Unindent();
                InUpdating = updatingState;
            }
        }

        /// <summary>
        /// Удаление объекта из базы и метаданных
        /// </summary>
        internal override void Drop(ModificationContext context)
        {
            bool updatingState = InUpdating;
            InUpdating = true;
            try
            {
                Trace.WriteLine(String.Format("Удаление пакета {0}", this));
                Trace.Indent();
                try
                {
                    try
                    {
                        Dictionary<string, CommonDBObject> removedWithErrors = new Dictionary<string, CommonDBObject>();

                        // Удаляем ассоциации
                        foreach (EntityAssociation item in Associations.Values)
                        {
                            try
                            {
                                SchemeClass.Instance.DDLDatabase.RunScript(item.DropScript(), false);
                            }
                            catch (Exception e)
                            {
                                Trace.TraceError("При удалении ассоциации \"{0}\" произошла ошибка: {1}", item.FullName, e.Message);
                                removedWithErrors.Add(item.ObjectKey, item);
                            }
                        }

                        // Удаляем таблицы
                        foreach (Entity item in Classes.Values)
                        {
                            try
                            {
                                SchemeClass.Instance.DDLDatabase.RunScript(item.DropScript(), false);
                            }
                            catch (Exception e)
                            {
                                Trace.TraceError("При удалении класса \"{0}\" произошла ошибка: {1}", item.FullName, e.Message);
                                removedWithErrors.Add(item.ObjectKey, item);
                            }
                        }

                        // Пытаемся повторно удалить объекты которые не удалось удалить с первого раза
                        foreach (CommonDBObject item in removedWithErrors.Values)
                        {
                            try
                            {
                                if (item is Entity)
                                {
                                    SchemeClass.Instance.DDLDatabase.RunScript(((Entity)item).DropScript(), false);
                                }
                                if (item is EntityAssociation)
                                {
                                    SchemeClass.Instance.DDLDatabase.RunScript(((EntityAssociation)item).DropScript(), false);
                                }
                            }
                            catch (Exception e)
                            {
                                Trace.TraceError("При повторной попытке удалить объект \"{0}\" произошла ошибка: {1}", item.FullName, e.Message);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("При удалении структур данных пакета \"{0}\" произошла ошибка: {1}", this, Krista.Diagnostics.KristaDiagnostics.ExpandException(e));
                    }

                    RemoveMetaData(context);

                    if (Parent.Packages.ContainsKey(GetKey(this.ObjectKey, this.FullName)))
                        Parent.Packages.Remove(GetKey(this.ObjectKey, this.FullName));

                    this.DbObjectState = DBObjectStateTypes.Unknown;
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.Message);
                    throw new Exception(e.Message, e);
                }

                base.Drop(context);
            }
            finally
            {
                InUpdating = updatingState;
            }
        }

        #region Получение изменений

        public override IModificationItem GetChanges(IModifiable toObject)
        {
            UpdateMajorObjectModificationItem root = (UpdateMajorObjectModificationItem)base.GetChanges(toObject);

            if (State == ServerSideObjectStates.New)
                return root;

            Package toPackage = (Package)toObject;

            //
            // Основные свойства
            //

            if (this.Name != toPackage.Name)
                throw new Exception(string.Format("У пакета {0} свойство Name недоступно для изменения.", this.FullName));

            if (this.Caption != toPackage.Caption)
            {
                ModificationItem item = new PropertyModificationItem("Caption", this.Caption, toPackage.Caption, root);
                root.Items.Add(item.Key, item);
            }

            if (this.Description != toPackage.Description)
            {
                ModificationItem item = new PropertyModificationItem("Description", this.Description, toPackage.Description, root);
                root.Items.Add(item.Key, item);
            }

            //
            // Коллекция классов
            //

            ModificationItem classesModificationItem = classes.GetChanges((EntityCollection<IEntity>)toPackage.Classes);

            if (classesModificationItem.Items.Count > 0)
            {
                classesModificationItem.Parent = root;
                root.Items.Add(classesModificationItem.Key, classesModificationItem);
            }

            //
            // Коллекция ассоциаций
            //

            ModificationItem associationsModificationItem = associations.GetChanges((EntityAssociationCollection)toPackage.Associations);

            if (associationsModificationItem.Items.Count > 0)
            {
                associationsModificationItem.Parent = root;
                root.Items.Add(associationsModificationItem.Key, associationsModificationItem);
            }

            //
            // Коллекция пакетов
            //

            ModificationItem packagesModificationItem = packages.GetChanges(toPackage.Packages);

            if (packagesModificationItem.Items.Count > 0)
            {
                packagesModificationItem.Parent = root;
                root.Items.Add(packagesModificationItem.Key, packagesModificationItem);
            }

            //
            // Коллекция документов
            //

            ModificationItem documentsModificationItem = documents.GetChanges(toPackage.Documents);

            if (documentsModificationItem.Items.Count > 0)
            {
                documentsModificationItem.Parent = root;
                root.Items.Add(documentsModificationItem.Key, documentsModificationItem);
            }

            return root;
        }

        #endregion Получение изменений

        /// <summary>
        /// Устанавливает признак необходимости сохранения XML-файла на внешние носители
        /// </summary>
        internal void NeedFlash()
        {
            neelFlash = true;
        }

        /// <summary>
        /// Сохранение пакета в репозиторий.
        /// </summary>
        internal void SaveToDisk(bool force)
        {
            foreach (Package item in Packages.Values)
                item.SaveToDisk(force);

            if (IsEndPoint && (neelFlash || force))
            {
                string filePath;

                filePath = String.Format("{0}\\{1}.xml", GetDir(),
                    FullName == "Корневой пакет" ?
                        "SchemeConfiguration" :
                        Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(PrivatePath)));

                XmlDocument doc = Validator.LoadDocument(ConfigurationXml);

                //CheckOut(String.Empty);

                XmlHelper.Save(doc, filePath);

                //CheckIn("Всем объектам присвоены GUID-ы");
            }
        }

        /// <summary>
        /// Сохранение пакета в репозиторий.
        /// </summary>
        public void SaveToDisk()
        {
            SaveToDisk(true);
        }

        /// <summary>
        /// Отмена редактирования объекта
        /// </summary>
        public override void CancelEdit()
        {
            UndoCheckOut();

            base.CancelEdit();
        }

        /// <summary>
        /// Завершение редактирования с применением всех изменений
        /// </summary>
        public override void EndEdit()
        {
            EndEdit(String.Empty);
        }

        /// <summary>
        /// Завершение редактирования с применением всех изменений
        /// </summary>
        /// <param name="comments">Комментарии к сделанным изменениям</param>
        public override void EndEdit(string comments)
        {
            SaveToDisk(false);

            { // Выкладываем изменения в SourceSafe
                foreach (Package item in Packages.Values)
                    item.CheckIn(comments);

                if (IsEndPoint && neelFlash)
                    CheckIn(comments);
            }

            base.EndEdit();
        }

        public void FillCollections(IEntityCollection<IClassifier> classesCollection, IEntityCollection<IFactTable> factCollection, IEntityAssociationCollection bridgeAssociationCollection, IEntityAssociationCollection associationCollection)
        {
            foreach (Package item in packages.Values)
                item.FillCollections(classesCollection, factCollection, bridgeAssociationCollection, associationCollection);

            foreach (IEntity entity in Classes.Values)
            {
                if (entity.ClassType == ClassTypes.clsDataClassifier
                    || entity.ClassType == ClassTypes.clsBridgeClassifier
                    || entity.ClassType == ClassTypes.clsFixedClassifier)
                {
                    try
                    {
                        classesCollection.Add(GetKey(entity.ObjectKey, entity.FullName), (IClassifier)entity);
                        continue;
                    }
                    catch
                    {
                        Trace.WriteLine("Классификатор уже присутствует " + entity.FullName);
                    }
                }
                if (entity.ClassType == ClassTypes.clsFactData)
                {
                    try
                    {
                        factCollection.Add(GetKey(entity.ObjectKey, entity.FullName), (IFactTable)entity);
                        continue;
                    }
                    catch
                    {
                        Trace.WriteLine("Таблица фактов уже присутствует " + entity.FullName);
                    }
                }
            }

            foreach (IEntityAssociation item in associations.Values)
            {
                try
                {
                    associationCollection.Add(GetKey(item.ObjectKey, item.FullName), item);
                }
                catch
                {
                    Trace.WriteLine("Ассоциация уже присутствует " + item.FullName);
                }

                if (item.AssociationClassType == AssociationClassTypes.Bridge
                    || item.AssociationClassType == AssociationClassTypes.BridgeBridge)
                {
                    try
                    {
                        bridgeAssociationCollection.Add(GetKey(item.ObjectKey, item.FullName), item);
                    }
                    catch
                    {
                        Trace.WriteLine("Ассоциация уже присутствует " + item.FullName);
                    }
                }
            }
        }

        #region Проверка объектов

        public DataTable Validate()
        {
            IValidationVisitor visitor = new MetaDataValidationVisitor();
            ((IValidable)this).AcceptVisitor(visitor);
            return ((ValidationVisitor)visitor).ResultTable.Rows.Count > 0 ? ((ValidationVisitor)visitor).ResultTable : null;
        }

        void IValidable.AcceptVisitor(IValidationVisitor visitor)
        {
            visitor.Visit(this);
        }

        #endregion Проверка объектов

        #region Поиск объектов

        /// <summary>
        /// Поиск объектов в дереве
        /// </summary>
        /// <param name="searchParam">Параметры поиска</param>
        /// <param name="searchTable">Таблица результатов</param>
        /// <return>True - если нашли</return>
        public bool Search(SearchParam searchParam, ref DataTable searchTable)
        {
            // Если поиск нечуствителен к регистру
            if (!searchParam.UseCaseSense)
            {
                // то приводим параметры к верхнему регистру.
                searchParam.pattern = searchParam.pattern.ToUpper();
            }

            // Обход дерева.
            foreach (Package package in SchemeClass.Instance.Packages.Values)
            {
                SearchInPackage(package, searchParam, ref searchTable);
            }

            return (searchTable.Rows.Count != 0);
        }

        /// <summary>
        /// Поиск в пакете и его подпакетах.
        /// </summary>
        /// <param name="package">Пакет, в котором ищем</param>
        /// <param name="searchParam">Параметры поиска</param>
        /// <param name="searchTable">Таблица результатов</param>
        private void SearchInPackage(Package package, SearchParam searchParam, ref DataTable searchTable)
        {
            // Ищем в текущем пакете.
            if (searchParam.FindPackages)
            {
                if (CheckMatch(searchParam, package.Name))
                {
                    AddTable(ref searchTable, package.Key, package.Name, package.FullName, package.GetType().Name,
                             "Name");
                }
                if (CheckMatch(searchParam, package.PrivatePath))
                {
                    AddTable(ref searchTable, package.Key, package.Name, package.FullName, package.GetType().Name,
                             "PrivatePath");
                }
                if (CheckMatch(searchParam, package.Description))
                {
                    AddTable(ref searchTable, package.Key, package.Name, package.FullName, package.GetType().Name,
                             "Description");
                }
                if (CheckMatch(searchParam, package.ObjectKey))
                {
                    AddTable(ref searchTable, package.Key, package.Name, package.FullName, package.GetType().Name,
                             "ObjectKey");
                }
            }

            // Ищем в классификаторах текущего пакета.
            if (searchParam.FindClassifiers || searchParam.FindAttributes || searchParam.FindHierarchyLevel)
            {
                foreach (Entity classifiers in package.Classes.Values)
                {
                    if (searchParam.FindClassifiers)
                    {
                        if (CheckMatch(searchParam, classifiers.Caption))
                        {
                            AddTable(ref searchTable, classifiers.Key, classifiers.OlapName, classifiers.FullName,
                                     classifiers.GetType().Name, "Caption");
                        }
                        if (CheckMatch(searchParam, classifiers.FullName))
                        {
                            AddTable(ref searchTable, classifiers.Key, classifiers.OlapName, classifiers.FullName,
                                     classifiers.GetType().Name, "FullName");
                        }
                        if (CheckMatch(searchParam, classifiers.FullDBName))
                        {
                            AddTable(ref searchTable, classifiers.Key, classifiers.OlapName, classifiers.FullName,
                                     classifiers.GetType().Name, "FullDBName");
                        }
                        if (CheckMatch(searchParam, classifiers.Description))
                        {
                            AddTable(ref searchTable, classifiers.Key, classifiers.OlapName, classifiers.FullName,
                                     classifiers.GetType().Name, "Description");
                        }
                        if (CheckMatch(searchParam, classifiers.OlapName))
                        {
                            AddTable(ref searchTable, classifiers.Key, classifiers.OlapName, classifiers.FullName,
                                     classifiers.GetType().Name, "OlapName");
                        }
                        if (CheckMatch(searchParam, classifiers.ObjectKey))
                        {
                            AddTable(ref searchTable, classifiers.Key, classifiers.OlapName, classifiers.FullName,
                                     classifiers.GetType().Name, "ObjectKey");
                        }
                    }

                    // Ищем в атрибутах текущего классификатора.
                    if (searchParam.FindAttributes)
                    {
                        foreach (EntityDataAttribute attribute in classifiers.Attributes.Values)
                        {
                            if (attribute.Owner != null)
                            {
                                if (CheckMatch(searchParam, attribute.Caption))
                                {
                                    AddTable(ref searchTable, attribute.Key, attribute.Caption, attribute.FullName,
                                             attribute.GetType().Name, "Caption");
                                }
                                if (CheckMatch(searchParam, attribute.Name))
                                {
                                    AddTable(ref searchTable, attribute.Key, attribute.Caption, attribute.FullName,
                                             attribute.GetType().Name, "Name");
                                }
                                if (CheckMatch(searchParam, attribute.SQLDefinition))
                                {
                                    AddTable(ref searchTable, attribute.Key, attribute.Caption, attribute.FullName,
                                             attribute.GetType().Name, "SQLDefinition");
                                }
                                if (CheckMatch(searchParam, attribute.Description))
                                {
                                    AddTable(ref searchTable, attribute.Key, attribute.Caption, attribute.FullName,
                                             attribute.GetType().Name, "Description");
                                }
                                if (CheckMatch(searchParam, attribute.ObjectKey))
                                {
                                    AddTable(ref searchTable, attribute.Key, attribute.Caption, attribute.FullName,
                                             attribute.GetType().Name, "ObjectKey");
                                }
                            }
                        }
                    }

                    // Ищем в уровнях иерархии текущего классификатора.
                    if (searchParam.FindHierarchyLevel && !(classifiers is FactTable) && !(classifiers is TableEntity) && !(classifiers is SystemDataSourcesClassifier) && !(classifiers is DocumentEntity))
                    {
                        foreach (DimensionLevel level in ((IClassifier)classifiers).Levels.Values)
                        {
                            if (CheckMatch(searchParam, level.Name))
                            {
                                AddTable(ref searchTable,
                                         level.Parent.Key + "::" + level.GetType().Name + ":" + level.Name, level.Name,
                                         level.Name,
                                         level.GetType().Name, "Name");
                            }
                            if (CheckMatch(searchParam, level.Description))
                            {
                                AddTable(ref searchTable,
                                         level.Parent.Key + "::" + level.GetType().Name + ":" + level.Name, level.Name,
                                         level.Name,
                                         level.GetType().Name, "Description");
                            }
                            if (CheckMatch(searchParam, level.ObjectKey))
                            {
                                AddTable(ref searchTable,
                                         level.Parent.Key + "::" + level.GetType().Name + ":" + level.Name, level.Name,
                                         level.Name,
                                         level.GetType().Name, "ObjectKey");
                            }
                            if (level is ParentChildLevel)
                            {
                                if (CheckMatch(searchParam, level.LevelNamingTemplate))
                                {
                                    AddTable(ref searchTable,
                                             level.Parent.Key + "::" + level.GetType().Name + ":" + level.Name,
                                             level.Name,
                                             level.Name,
                                             level.GetType().Name, "LevelNamingTemplate");
                                }
                            }
                        }
                    }
                }
            }

            // Ищем в ассоциациях текущего пакета.
            if (searchParam.FindAssocitions || searchParam.FindAssociateRule || searchParam.FindAssociateMapping)
            {
                foreach (EntityAssociation association in package.Associations.Values)
                {
                    if (searchParam.FindAssocitions)
                    {
                        if (CheckMatch(searchParam, association.Caption))
                        {
                            AddTable(ref searchTable, association.Key, association.FullCaption, association.FullName,
                                     association.GetType().Name, "Caption");
                        }
                        if (CheckMatch(searchParam, association.FullName))
                        {
                            AddTable(ref searchTable, association.Key, association.FullCaption, association.FullName,
                                     association.GetType().Name, "FullName");
                        }
                        if (CheckMatch(searchParam, association.FullDBName))
                        {
                            AddTable(ref searchTable, association.Key, association.FullCaption, association.FullName,
                                     association.GetType().Name, "FullDBName");
                        }
                        if (CheckMatch(searchParam, association.FullCaption))
                        {
                            AddTable(ref searchTable, association.Key, association.FullCaption, association.FullName,
                                     association.GetType().Name, "FullCaption");
                        }
                        if (CheckMatch(searchParam, association.ObjectKey))
                        {
                            AddTable(ref searchTable, association.Key, association.FullCaption, association.FullName,
                                     association.GetType().Name, "ObjectKey");
                        }
                    }

                    // Ищем в правилах сопоставления и формирования текущей ассоциации.
                    if ((association is BridgeAssociation) && (searchParam.FindAssociateRule || searchParam.FindAssociateMapping))
                    {
                        if (searchParam.FindAssociateRule)
                        {
                            foreach (AssociateRule rule in ((BridgeAssociation)association).AssociateRules.Values)
                            {
                                if (CheckMatch(searchParam, rule.Name))
                                {
                                    AddTable(ref searchTable, association.Key + "::" + rule.GetType().Name + ":" + rule.Name, rule.Name, rule.Name,
                                             rule.GetType().Name, "Name");
                                }
                                if (CheckMatch(searchParam, rule.ObjectKey))
                                {
                                    AddTable(ref searchTable, association.Key + "::" + rule.GetType().Name + ":" + rule.Name, rule.Name, rule.Name,
                                             rule.GetType().Name, "ObjectKey");
                                }
                            }
                        }
                        if (searchParam.FindAssociateMapping)
                        {
                            foreach (AssociateMapping map in ((BridgeAssociation)association).Mappings)
                            {
                                if (CheckMatch(searchParam, map.DataValue.Attribute.Caption + " -> " +
                                             map.BridgeValue.Attribute.Caption))
                                {
                                    AddTable(ref searchTable,
                                             association.Key + "::" + map.GetType().Name + ":" +
                                             map.DataValue.Attribute.Name + "." + map.BridgeValue.Attribute.Name,
                                             map.DataValue.Attribute.Caption + " -> " +
                                             map.BridgeValue.Attribute.Caption,
                                             map.DataValue.Attribute.Name + "." + map.BridgeValue.Attribute.Name,
                                             map.GetType().Name, "Name");
                                }
                            }
                        }
                    }
                }
            }

            // Ищем в документах текущего пакета.
            if (searchParam.FindDocuments)
            {
                foreach (Document document in package.Documents.Values)
                {
                    if (CheckMatch(searchParam, document.Name))
                    {
                        AddTable(ref searchTable, document.Key, document.Name, document.FullName,
                                 document.GetType().Name, "Name");
                    }
                    if (CheckMatch(searchParam, document.Description))
                    {
                        AddTable(ref searchTable, document.Key, document.Name, document.FullName,
                                 document.GetType().Name, "Description");
                    }
                    if (CheckMatch(searchParam, document.ObjectKey))
                    {
                        AddTable(ref searchTable, document.Key, document.Name, document.FullName,
                                 document.GetType().Name, "ObjectKey");
                    }
                }
            }

            // Вызываем рекурсивно метод для подпакетов.
            foreach (Package subPackage in package.Packages.Values)
            {
                SearchInPackage(subPackage, searchParam, ref searchTable);
            }
        }

        /// <summary>
        /// Добавление в таблицу результатов
        /// </summary>
        /// <param name="dataTable">Ссылка на таблицу результатов</param>
        /// <param name="key">Ключ</param>
        /// <param name="caption">Наименование</param>
        /// <param name="path">Полное имя</param>
        /// <param name="type">Тип объекта</param>
        /// <param name="property">Свойство объекта, в котором найдено соответствие</param>
        private static void AddTable(ref DataTable dataTable, string key, string caption, string path, string type, string property)
        {
            // Создаем и добавляем строку в таблицу.
            DataRow row = dataTable.NewRow();
            row["key"] = key;
            row["caption"] = caption;
            row["path"] = path;
            row["type"] = type;
            row["property"] = property;
            dataTable.Rows.Add(row);
        }

        /// <summary>
        /// Процедура проверки соответствия найденной строки искомой строке.
        /// </summary>
        /// <param name="searchParam"></param>
        /// <param name="KeyStr">Искомая строка</param>
        /// <returns>Результат сравнения</returns>
        private bool CheckMatch(SearchParam searchParam, string KeyStr)
        {
            // Если искомая строка пустая
            if (String.IsNullOrEmpty(KeyStr))
            {
                // то выходим из метода.
                return false;
            }

            // Проверка и корректировка строки в соответствии с условием чувствительности регистра.
            KeyStr = (!searchParam.UseCaseSense) ? KeyStr.ToUpper() : KeyStr;

            // Если включена проверка регулярных выражений и найдено соответствие
            // или имя узла содержит искомую подстроку.
            return
                (searchParam.UseRegExp && CheckRegExp(KeyStr, searchParam.pattern) ||
                  CompareStr(searchParam, KeyStr, searchParam.pattern));
        }

        /// <summary>
        /// Сравнивает строки с учетом флага поиска целого вхождения.
        /// </summary>
        /// <param name="searchParam"></param>
        /// <param name="inStr">Входная строка для проверки</param>
        /// <param name="patternStr">Шаблон сравнения</param>
        /// <returns>Результат сравнения</returns>
        private bool CompareStr(SearchParam searchParam, string inStr, string patternStr)
        {
            if (searchParam.UseWholeWord) // Если флажок установлен
            {
                return inStr.Equals(patternStr); // то проверяем, совпадают ли строки
            }
            else
            {
                return inStr.Contains(patternStr); // иначе проверяем содержит ли узел искомую строку.
            }
        }

        /// <summary>
        /// Проверка входной строки на соответствие регулярному выражению.
        /// </summary>
        /// <param name="inStr">Входящая строка для проверки</param>
        /// <param name="patternStr">Шаблон сравнения</param>
        /// <returns>True - если проверка прошла успешно</returns>
        private bool CheckRegExp(string inStr, string patternStr)
        {
            try
            {
                Regex regExp = new Regex(patternStr);
                return regExp.Match(inStr).Success;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw new Exception(e.Message, e);
            }
        }

        #endregion

        #region Поиск зависимых данных по источникам.
        /// <summary>
        /// Создает Datatable для результатов.
        /// </summary>
        /// <returns></returns>
        private DataTable CreateResultDT()
        {
            DataTable dt = new DataTable("DependedData");
            DataColumn colFullCaption = new DataColumn("FullCaption", Type.GetType("System.String"));
            DataColumn colFullDBName = new DataColumn("FullDBName", Type.GetType("System.String"));
            DataColumn colObjectType = new DataColumn("ObjectType", Type.GetType("System.String"));
            DataColumn colName = new DataColumn("Name", Type.GetType("System.String"));
            DataColumn colServeRowCount = new DataColumn("ServeRowCount", Type.GetType("System.Int32"));
            DataColumn colUserRowCount = new DataColumn("UserRowCount", Type.GetType("System.Int32"));

            dt.Columns.Add(colObjectType);
            dt.Columns.Add(colFullCaption);
            dt.Columns.Add(colFullDBName);
            dt.Columns.Add(colName);
            dt.Columns.Add(colServeRowCount);
            dt.Columns.Add(colUserRowCount);

            return dt;
        }

        /// <summary>
        /// Возвращает зависимые данные по источникам.
        /// </summary>
        /// <param name="sID">ID источника.</param>
        /// <returns>DataTable с названиями таблиц и количеством зависимых данных (если есть)</returns>
        public DataTable GetSourcesDependedData(int sID)
        {
            DataTable dt = CreateResultDT();
            IDatabase db = null;
            try
            {
                db = SchemeClass.Instance.SchemeDWH.DB;
                foreach (Package package in SchemeClass.Instance.Packages.Values)
                {
                    SearchDependedData(package, sID, ref dt, db);
                }
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
            return dt;
        }

        /// <summary>
        /// Рекурсивно обходит схему и формирует таблицу с зависимыми данными.
        /// </summary>
        /// <param name="package">Пакет, в котором ищем.</param>
        /// <param name="sourceID">ID источника.</param>
        /// <param name="dt">Таблица с результатами.</param>
        private void SearchDependedData(Package package, int sourceID, ref DataTable dt, IDatabase db)
        {
            // Ищем в текущем пакете.
            foreach (Entity obj in package.Classes.Values)
            {
                try
                {
                    int serveRowCount = 0;
                    int userRowCount = 0;
                    // Если это классификатор
                    if (obj.ClassType == ClassTypes.clsDataClassifier ||
                        obj.ClassType == ClassTypes.clsBridgeClassifier)
                    {
                        // И он делится по источникам
                        if (((DataSourceDividedClass)obj).IsDivided)
                        {
                            // запросом получаем количество записей по источнику
                            serveRowCount = RecordsCount(sourceID, SourceDependedRowType.Serve, obj.FullDBName, db);
                            userRowCount = RecordsCount(sourceID, SourceDependedRowType.Users, obj.FullDBName, db);
                        }
                    }
                    // Если это таблица фактов
                    if (obj.ClassType == ClassTypes.clsFactData)
                    {
                        // запросом получаем количество записей по источнику, системных записей нет.
                        userRowCount = RecordsCount(sourceID, SourceDependedRowType.Indifferently, obj.FullDBName, db);
                    }
                    // Если есть такой источник
                    if (serveRowCount > 0 || userRowCount > 0)
                    {
                        // Формируем и добавляем строку в таблицу.
                        DataRow row = GetDependedDataRow(dt, obj, serveRowCount, userRowCount);
                        dt.Rows.Add(row);
                    }
                }
                catch (Exception)
                {

                }
            }
            // Вызываем рекурсивно метод для подпакетов.
            foreach (Package subPackage in package.Packages.Values)
            {
                SearchDependedData(subPackage, sourceID, ref dt, db);
            }
        }

        private enum SourceDependedRowType
        {
            Serve,
            Users,
            Indifferently
        }

        /// <summary>
        /// Выбирает количество записей по источнику.
        /// </summary>
        /// <param name="sourceID"></param>
        /// <param name="rowType"></param>
        /// <returns></returns>
        private int RecordsCount(int sourceID, SourceDependedRowType rowType, string objectName, IDatabase db)
        {
            string selectCondition = String.Empty;
            switch(rowType)
            {
                case SourceDependedRowType.Serve:
                    selectCondition = string.Format("and {0} <> 0", DataAttribute.RowTypeColumnName);
                    break;
                case SourceDependedRowType.Users:
                    selectCondition = string.Format("and {0} = 0", DataAttribute.RowTypeColumnName);
                    break;
                case SourceDependedRowType.Indifferently:
                    // оставляем пустым.
                    break;
            }
            string selectQuery = string.Format("select count(*) from {0} where SourceID = ? {1}",
                objectName , selectCondition);
            
            return Convert.ToInt32(db.ExecQuery(selectQuery, QueryResultTypes.Scalar, 
                db.CreateParameter("sourceID", sourceID)));
        }

        /// <summary>
        /// Формирует строку для добавления в таблицу результатов.
        /// </summary>
        /// <param name="dt">Таблица.</param>
        /// <param name="obj">Объект, информацию о котором добавляем.</param>
        /// <param name="count">Количество записей.</param>
        /// <returns>Строка для добавления.</returns>
        private static DataRow GetDependedDataRow(DataTable dt, Entity obj, int serveRowCount, int userRowCount)
        {
            DataRow row = dt.NewRow();
            row["ObjectType"] = obj.GetObjectType();
            row["Name"] = obj.FullName;
            row["ServeRowCount"] = serveRowCount;
            row["UserRowCount"] = userRowCount;
            row["FullDBName"] = obj.FullDBName;
            row["FullCaption"] = obj.FullCaption;
            return row;
        }

        #endregion

        #region Свойства

        /// <summary>
        /// true - Объект является целой частью, false - является частью составного объекта 
        /// </summary>
        public override bool IsEndPoint
        {
            [DebuggerStepThrough]
            get { return !String.IsNullOrEmpty(privatePath); }
            set { base.IsEndPoint = value; }
        }

        /// <summary>
        /// Полное имя объекта
        /// </summary>
        public override string FullName
        {
            [DebuggerStepThrough]
            get { return Name; }
        }

        /// <summary>
        /// Локальный путь к файлу пакета, если не указан, то пакет встроенный
        /// </summary>
        public string PrivatePath
        {
            get { return Instance.privatePath; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    SetInstance.privatePath = String.Empty;
                else
                    SetInstance.privatePath = value.Trim();
            }
        }

        public IPackageCollection Packages
        {
            [DebuggerStepThrough]
            get { return Instance.packages; }
        }

        public IEntityCollection<IEntity> Classes
        {
            [DebuggerStepThrough]
            get { return Instance.classes; }
        }

        public IEntityAssociationCollection Associations
        {
            [DebuggerStepThrough]
            get { return Instance.associations; }
        }

        /// <summary>
        /// Документы пакета
        /// </summary>
        public IDocumentCollection Documents
        {
            [DebuggerStepThrough]
            get { return Instance.documents; }
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            return String.Format("{0} : {1}", FullName, base.ToString());
        }

        #endregion Свойства

        #region Права

        /// <summary>
        /// Проверяет права на просмотр объекта для текущего пользователя
        /// </summary>
        /// <returns>true - если у пользлвателя есть права на просмотр</returns>
        public override bool CurrentUserCanViewThisObject()
        {
            return true;
        }

        #endregion Права

        #region Утилитариум
        /*
        /// <summary>
        /// Выкладывает пакет в SourceSafe.
        /// Делает CheckOut, 
        /// затем сохраняет пакет на диск
        /// и делает CheckIn в SourceSafe.
        /// </summary>
        internal void StoreInVSS()
        {
            try
            {
                string comment = "Во все атрибуты добавляем признак разыменовки";
                Trace.TraceInformation("Сохраняем пакет \"{0}\"", this.Name);
                CheckOut("");
                SaveToDisk();
                CheckIn(comment);

                foreach (IPackage item in Packages.Values)
                    ((Package)item).StoreInVSS();
            }
            catch (Exception e)
            {
                Trace.TraceError("{0}", e.ToString());
            }
        }
*/
        #endregion Утилитариум

        #region Методы для работы с поиском зависимостей между пакетами

        /// <summary>
        /// Получаем таблицу конфликтных зависимостей между пакетами
        /// </summary>
        /// <returns></returns>
        public DataTable GetConflictPackageDependents()
        {
            return PackageDependents.GetConflictPackageDependents(this);
        }

        /// <summary>
        /// Для пакета определяем от каких пакетов в схеме он зависит, включая зависимые через промежуточный пакет
        /// </summary>
        /// <returns></returns>
        public List<string> GetDependentsByPackage()
        {
            return PackageDependents.GetDependentsByPackage(this);
        }

        /// <summary>
        /// Сортировка объектов внутри пакета с учетом внутренних зависимостей
        /// </summary>
        /// <returns>Отсортированная коллекция объектов</returns>
        public List<IEntity> GetSortEntitiesByPackage()
        {
            return PackageDependents.GetSortEntitiesByPackage(this);
        }

        #endregion

    }
}

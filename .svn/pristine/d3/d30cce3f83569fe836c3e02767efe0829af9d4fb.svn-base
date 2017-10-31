using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Classes;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme
{
    internal partial class SchemeClass : SMOSerializable, IScheme
    {
        private IEntityAssociationCollection bridgeAssociationCollection;
        private IEntityAssociationCollection associationCollection;
        // Коллекция классификаторов
        private IEntityCollection<IClassifier> classifierCollection;
        // Коллекция таблиц фактов
        private IEntityCollection<IFactTable> factTableCollection;


        #region Инициализация объектов схемы

        private static FixedClassifier CreateEntityYearDay(Package parentPackage)
        {
            FixedClassifier cls = new FixedClassifier(SystemSchemeObjects.YearDay_ENTITY_KEY, parentPackage, "FX", "YearDay", ServerSideObjectStates.Consistent);
            cls.Semantic = "Date";
            cls.SubClassType = SubClassTypes.System;
            cls.ID = 1;
            cls.Caption = "Год Месяц День";
            cls.Description = "Фиксированный классификатор \"Год Месяц День\"";
            return cls;
        }

        private static FixedClassifier CreateEntityYearMonth(Package parentPackage)
        {
            FixedClassifier cls = new FixedClassifier(SystemSchemeObjects.YearMonth_ENTITY_KEY, parentPackage, "FX", "YearMonth", ServerSideObjectStates.Consistent);
            cls.Semantic = "Date";
            cls.SubClassType = SubClassTypes.System;
            cls.ID = 2;
            cls.Caption = "Год Месяц";
            cls.Description = "Фиксированный классификатор \"Год Месяц\"";
            return cls;
        }

        private static FixedClassifier CreateEntityYear(Package parentPackage)
        {
            FixedClassifier cls = new FixedClassifier(SystemSchemeObjects.Year_ENTITY_KEY, parentPackage, "FX", "Year", ServerSideObjectStates.Consistent);
            cls.Semantic = "Date";
            cls.SubClassType = SubClassTypes.System;
            cls.ID = 3;
            cls.Caption = "Год";
            cls.Description = "Фиксированный классификатор \"Год\"";
            return cls;
        }

        private static FixedClassifier CreateEntityYearDayUNV(Package parentPackage)
        {
            FixedClassifier cls = new FixedClassifier(SystemSchemeObjects.YearDayUNV_ENTITY_KEY, parentPackage, "FX", "YearDayUNV", ServerSideObjectStates.Consistent);
            cls.Semantic = "Date";
            cls.SubClassType = SubClassTypes.System;
            cls.ID = 0;
            cls.Caption = "Год Квартал Месяц";
            cls.Description = "Фиксированный классификатор \"Год Квартал Месяц\"";
            return cls;
        }

        private static SystemDataSourcesClassifier CreateSystemDataSources(Package parentPackage)
        {
            return new SystemDataSourcesClassifier(parentPackage);
        }

        internal static Package CreateSystemPackage(ServerSideObject owner)
        {
            Package systemPackage = new Package(SystemSchemeObjects.SYSTEM_PACKAGE_KEY, owner, "Системные объекты", ServerSideObjectStates.Consistent);
            systemPackage.DbObjectState = DBObjectStateTypes.InDatabase;

            Entity cls = CreateEntityYearDayUNV(systemPackage);
            cls.DbObjectState = DBObjectStateTypes.InDatabase;
            systemPackage.Classes.Add(cls.ObjectKey, cls);

            cls = CreateEntityYearDay(systemPackage);
            cls.DbObjectState = DBObjectStateTypes.InDatabase;
            systemPackage.Classes.Add(cls.ObjectKey, cls);

            cls = CreateEntityYearMonth(systemPackage);
            cls.DbObjectState = DBObjectStateTypes.InDatabase;
            systemPackage.Classes.Add(cls.ObjectKey, cls);

            cls = CreateEntityYear(systemPackage);
            cls.DbObjectState = DBObjectStateTypes.InDatabase;
            systemPackage.Classes.Add(cls.ObjectKey, cls);

            cls = CreateSystemDataSources(systemPackage);
            cls.DbObjectState = DBObjectStateTypes.InDatabase;
            systemPackage.Classes.Add(cls.ObjectKey, cls);

            return systemPackage;
        }

        /// <summary>
        /// Инициализация пакетов из базы данных
        /// </summary>
        private void InitializePackages(DataSet metaData)
        {
            rootPackage = new Package(SystemSchemeObjects.ROOT_PACKAGE_KEY, this, "Корневой пакет", ServerSideObjectStates.Consistent);
            rootPackage.PrivatePath = "SchemeConfiguration.xml";

            //Trace.Indent();
            Trace.TraceEvent(TraceEventType.Information, "Инициализация пакетов");

			DataRow[] rows = metaData.Tables[0].Select("RefParent is null", "OrderBy");

			foreach (DataRow row in rows)
			{
				Package package = Package.CreatePackage(
					Convert.ToString(row["ObjectKey"]), 
					rootPackage,
					Convert.ToInt32(row["ID"]),
					Convert.ToString(row["Name"]),
					String.Empty, ServerSideObjectStates.Consistent);
				
				if (!(row["PrivatePath"] is DBNull))
					package.PrivatePath = Convert.ToString(row["PrivatePath"]);

				rootPackage.Packages.Add(
					KeyIdentifiedObject.GetKey(package.ObjectKey, package.FullName),
					package);

				package.InitializeFromDB(metaData);
				package.DbObjectState = DBObjectStateTypes.InDatabase;
				if (package.ID == 1)
				{
					package.Classes.Add(SystemSchemeObjects.YearDay_ENTITY_KEY, CreateEntityYearDay(package));
					package.Classes.Add(SystemSchemeObjects.YearMonth_ENTITY_KEY, CreateEntityYearMonth(package));
					package.Classes.Add(SystemSchemeObjects.Year_ENTITY_KEY, CreateEntityYear(package));
					package.Classes.Add(SystemSchemeObjects.YearDayUNV_ENTITY_KEY, CreateEntityYearDayUNV(package));
					package.Classes.Add(SystemSchemeObjects.SystemDataSources_ENTITY_KEY, CreateSystemDataSources(package));
				}
			}

            //Trace.Unindent();
        }

        /// <summary>
        /// Инициализация объектов схемы по метаданным из базы данных 
        /// (факты, классификаторы, сопоставимые, ассоциации)
        /// </summary>
        /// <returns></returns>
        public bool InitializeObjects()
        {
			DataSet metaDataDataSet = new DataSet("MetaData");
			
			string sqlSelectMetaPackages = "select ID, ObjectKey, Name, BuiltIn, RefParent, PrivatePath, OrderBy from MetaPackages order by 7";
			string sqlSelectMetaObjects = "select ID, Semantic, Name, Class, SubClass, Configuration, Name, RefPackages, ObjectKey from MetaObjects order by 1";
			string sqlSelectMetaLinks = "select ID, Semantic, Name, Class, RoleAName, RoleBName, Configuration, ObjectKey, RoleAObjectKey, RoleBObjectKey, RefPackages from MetaLinksWithRolesNames order by 1";
            string sqlSelectMetaDocuments = "select ID, Name, DocType, ObjectKey, RefPackages from MetaDocuments order by 1";

			using (IDatabase db = Scheme.SchemeDWH.Instance.DB)
			{
				Trace.TraceVerbose("Чтение данных пакетов...");
				DataTable metaPackages = (DataTable)db.ExecQuery(sqlSelectMetaPackages, QueryResultTypes.DataTable);
				metaPackages.TableName = "MetaPackages";
				
				Trace.TraceVerbose("Чтение данных объектов...");
				DataTable metaObjects = (DataTable)db.ExecQuery(sqlSelectMetaObjects, QueryResultTypes.DataTable);
				metaObjects.TableName = "MetaObjects";

				Trace.TraceVerbose("Чтение данных ассоциаций...");
				DataTable metaLinks = (DataTable)db.ExecQuery(sqlSelectMetaLinks, QueryResultTypes.DataTable);
				metaLinks.TableName = "MetaLinks";

				Trace.TraceVerbose("Чтение данных документов...");
				DataTable metaDocuments = (DataTable)db.ExecQuery(sqlSelectMetaDocuments, QueryResultTypes.DataTable);
			    metaDocuments.TableName = "MetaDocuments";

				metaDataDataSet.Tables.AddRange(new DataTable[] { metaPackages, metaObjects, metaLinks, metaDocuments });
			}

			// Инициализируем пакеты
			InitializePackages(metaDataDataSet);

			foreach (DataTable table in metaDataDataSet.Tables)
			{
				table.Clear();
			}
			metaDataDataSet.Tables.Clear();
			metaDataDataSet.Clear();
			metaDataDataSet.Dispose();
            //rootPackage.Validate();

            return true;
        }

        /// <summary>
        /// Инициализация таблиц перекодировок из базы данных
        /// </summary>
        internal void InitializeConversionTables()
        {
            using (IDatabase db = SchemeDWH.DB)
            {
                // Формирование таблиц перекодировок
                foreach (KeyValuePair<string, IEntityAssociation> assItem in Associations)
                {
                    Association association = (Association)assItem.Value;
                    if (association.AssociationClassType == AssociationClassTypes.Bridge)
                    {
                        foreach (KeyValuePair<string, IAssociateRule> associateRuleItem in ((BridgeAssociation)association).AssociateRules)
                        {
                            AssociateRule associateRule = (AssociateRule)associateRuleItem.Value;
                            try
                            {
                                //ConversionTable ct = (ConversionTable)this.ConversionTables[association.FullName + "." + associateRule.Name];
                                ConversionTable ct = (ConversionTable)ConversionTableCollection.Instance[association, KeyIdentifiedObject.GetKey(associateRule.ObjectKey, associateRule.Name)];
                                if (ct.IsDetached)
                                    ct.AttachToDB(db);

                            }
                            catch (Exception e)
                            {
                                Trace.WriteLine("Ошибка при Формировании таблицы перекодировок " + association.FullName + "." + associateRule.Name + ": " + e.Message);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Создает коллекции объектов схемы 
        /// </summary>
        private void InitializeObjectsCollections()
        {
            classifierCollection = new EntityCollection<IClassifier>(this, ServerSideObjectStates.Consistent);
            factTableCollection = new EntityCollection<IFactTable>(this, ServerSideObjectStates.Consistent);
            bridgeAssociationCollection = new EntityAssociationCollection(this, ServerSideObjectStates.Consistent);
            associationCollection = new EntityAssociationCollection(this, ServerSideObjectStates.Consistent);
            rootPackage.FillCollections(classifierCollection, factTableCollection, bridgeAssociationCollection, associationCollection);
            
            usersManager.SynchronizeObjectsWithScheme();
        }

        #endregion Инициализация объектов схемы

        #region Реализация интерфейса IScheme

        private static ICommonDBObject GetObjectByPathName(IPackage container, string[] parts, int index)
        {
            string name = parts[index].Split(':')[1];
            string type = parts[index].Split(':')[0];
            try
            {
                switch (type)
                {
                    case "Package":
                        if (index + 1 == parts.Length)
                            return container.Packages[name];
                        else
                            return GetObjectByPathName(container.Packages[name], parts, index + 1);
                    case "BridgeClassifier":
                    case "DataClassifier":
                    case "FixedClassifier":
                    case "VariantDataClassifier":
                    case "FactTable":
                    case "DetailClassifier":
					case "TableEntity":
					case "DocumentEntity":
                        return container.Classes[name];
                    case "FactAssociation":
                    case "BridgeAssociation":
                    case "MasterDetailAssociation":
                    case "BridgeAssociationItSelf":
                        return container.Associations[name];
                    case "Document":
                        return container.Documents[name];
                    default:
                        throw new Exception(String.Format("В пути встретился неизвестный модификатор {0}", parts[index]));
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("В функции Scheme.GetObjectByPathName(...) произошла ошибка: {0}", e.Message);
                return null;
            }
        }

        /// <summary>
        /// Возвращает объект по его уникальному ключу
        /// </summary>
        /// <param name="key">Уникальный ключ объекта</param>
        /// <returns>Объект схемы</returns>
        public ICommonDBObject GetObjectByKey(string key)
        {
            string[] parts = key.Split(new string[] { "::" }, StringSplitOptions.None);
            return GetObjectByPathName(RootPackage, parts, 1);
        }


#warning Удалить метод после выпуска версии 2.4.1
        private static string GetGuidPathByPathName(IPackage container, string[] parts, int index)
        {
            string name = parts[index].Split(':')[1];
            string type = parts[index].Split(':')[0];
            try
            {
                switch (type)
                {
                    case "Package":
                        if (index + 1 == parts.Length)
                            return "Package:" + container.Packages[name].ObjectKey;
                        else
                        {
                            foreach (IPackage item in container.Packages.Values)
                            {
                                if (item.FullName == name)
                                    return String.Format("Package:{0}::{1}", item.ObjectKey, GetGuidPathByPathName(item, parts, index + 1));
                            }
                            return null;
                        }
                    case "BridgeClassifier":
                    case "DataClassifier":
                    case "FixedClassifier":
                    case "VariantDataClassifier":
                    case "FactTable":
                    case "DetailClassifier":
                    case "TableEntity":
                        foreach (IEntity item in container.Classes.Values)
                        {
                            if (item.FullName == name)
                                return String.Format("{0}:{1}", type, item.ObjectKey);
                        }
                        return null;
                    case "FactAssociation":
                    case "BridgeAssociation":
                    case "MasterDetailAssociation":
                        foreach (IEntityAssociation item in container.Associations.Values)
                        {
                            if (item.FullName == name)
                                return String.Format("{0}:{1}", type, item.ObjectKey);
                        }
                        return null;
                    case "Document":
                        return String.Format("{0}:{1}", type, container.Documents[name]);
                    default:
                        throw new Exception(String.Format("В пути встретился неизвестный модификатор {0}", parts[index]));
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("В функции Scheme.GetObjectByPathName(...) произошла ошибка: {0}", e.Message);
                return null;
            }
        }

#warning Удалить метод после выпуска версии 2.4.1
        public string GetGuidPathByPathName(string key)
        {
            try
            {
                string[] parts = key.Split(new string[] {"::"}, StringSplitOptions.None);
                return String.Format("Package:{0}::{1}",
                                     SystemSchemeObjects.ROOT_PACKAGE_KEY,
                                     GetGuidPathByPathName(RootPackage, parts, 1));
            }
            catch(Exception e)
            {
                Trace.TraceError("Путь не найден: {0}", key);
                return String.Empty;
            }
        }

        /// <summary>
        /// Коллекцию классификаторов
        /// </summary>
        public IEntityCollection<IClassifier> Classifiers
        {
            get 
            {
                return classifierCollection; 
            }
        }

        public IEntityCollection<IFactTable> FactTables
        {
            get { return factTableCollection; }
            set { factTableCollection = value; }
        }
        
        public IEntityAssociationCollection Associations
        {
            get { return bridgeAssociationCollection; }
            set { bridgeAssociationCollection = value; }
        }

        public IEntityAssociationCollection AllAssociations
        {
            get { return associationCollection; }
            set { associationCollection = value; }
        }

        public IPackageCollection Packages
        {
            get { return rootPackage.Packages; }
        }

        #endregion Реализация интерфейса IScheme
    }
}

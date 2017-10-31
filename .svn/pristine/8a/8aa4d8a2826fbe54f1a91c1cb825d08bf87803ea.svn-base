using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;

using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Modifications;
using Krista.FM.Server.Scheme.ScriptingEngine;
using Krista.FM.Server.Scheme.ScriptingEngine.Classes;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Базовый класс объектов делящихся по источникам
    /// </summary>
    internal class DataSourceDividedClass : Entity, IDataSourceDividedClass
    {
        private bool supportDivide;
        /// <summary>
        /// Убрать после того, как методологи заполнят коллекцию видов поступающей информации
        /// </summary>
        private string dataSourceParameter;
        /// <summary>
        /// Информация о коллекции видов ичточника в формате {[имя поставщика]\[код вида];...}
        /// </summary>
        private string dataSourceKinds = string.Empty;

        /// <summary>
        /// Конструстор базового класса объектов делящихся по источникам
        /// </summary>
        /// <param name="key"></param>
        /// <param name="owner"></param>
        /// <param name="semantic"></param>
        /// <param name="name">Английское наименование объекта</param>
        /// <param name="classType">Класс объекта</param>
        /// <param name="subClassType">Подкласс объекта</param>
        /// <param name="isSupportDivide"></param>
        /// <param name="state"></param>
        /// <param name="scriptingEngine"></param>
        public DataSourceDividedClass(string key, ServerSideObject owner, string semantic, string name, ClassTypes classType, SubClassTypes subClassType, bool isSupportDivide, ServerSideObjectStates state, ScriptingEngineAbstraction scriptingEngine)
            : base(key, owner, semantic, name, classType, subClassType, state, scriptingEngine)
        {
            this.supportDivide = isSupportDivide;
        }

        #region ServerSideObject

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected new DataSourceDividedClass Instance
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return (DataSourceDividedClass)GetInstance(); }
        }

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь, для установки значений свойств
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected new DataSourceDividedClass SetInstance
        {
            get
            {
                if (SetterMustUseClone())
                    return (DataSourceDividedClass)CloneObject;
                else
                    return this;
            }
        }

        #endregion ServerSideObject

        /// <summary>
        /// Инициализация свойств объекта 
        /// </summary>
        /// <param name="doc">Документ с XML настройкой</param>
        /// <param name="atagElementName">наименование тега с настройками объекта</param>
        protected override void InitializeProperties(XmlDocument doc, string atagElementName)
        {
            base.InitializeProperties(doc, atagElementName);

            if (supportDivide)
            {
                // Убрать после того, как методологи заполнят коллекцию видов поступающей информации
                XmlNode xmlDataSourceParameter = doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/@dataSourceParameter", atagElementName));
                if (xmlDataSourceParameter != null)
                {
                    this.dataSourceParameter = xmlDataSourceParameter.Value;
                }

                // набор видов источника
                XmlNode xmlDataSourceKinds = doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/@dataSourceKinds", atagElementName));
                if (xmlDataSourceKinds != null)
                {
                    this.dataSourceKinds = xmlDataSourceKinds.Value;
                }
            }
        }

        public override IModificationItem GetChanges(IModifiable toObject)
        {
            MajorObjectModificationItem root = (MajorObjectModificationItem)base.GetChanges(toObject);

            if (supportDivide)
            {
                DataSourceDividedClass toDataClassifier = (DataSourceDividedClass)toObject;

                if (this.DataSourceKinds != toDataClassifier.DataSourceKinds)
                {
                    ModificationItem item = new PropertyModificationItem("DataSourceKinds", this.DataSourceKinds, toDataClassifier.DataSourceKinds, root);
                    root.Items.Add(item.Key, item);
                }
            }

            return root;
        }

        /// <summary>
        /// Применение изменений. Приводит текущий объект к виду объекта toObject
        /// </summary>
        /// <param name="context"></param>
        /// <param name="toObject">Объект к виду которого будет приведен текущий объект</param>
        public override void Update(ModificationContext context, IModifiable toObject)
        {
            string rollbakDataSourceParameter = dataSourceParameter;
            string rollbakDataSourceKinds = dataSourceKinds;
            try
            {
                base.Update(context, toObject);

                if (supportDivide)
                {
                    DataSourceDividedClass toDataClassifier = (DataSourceDividedClass)toObject;

                    // Деление по источникам
                    UpdateDivide(context, toDataClassifier);

                    // Убрать после того, как методологи заполнят коллекцию видов поступающей информации
                    if (this.dataSourceParameter != toDataClassifier.dataSourceParameter)
                    {
                        Trace.WriteLine(String.Format("У объекта \"{0}\" изменился Параметр источника c \"{1}\" на \"{2}\"",
                            FullName, dataSourceParameter, toDataClassifier.dataSourceParameter));
                        this.dataSourceParameter = toDataClassifier.dataSourceParameter;
                    }

                    // Параметр источника
                    if (this.DataSourceKinds != toDataClassifier.DataSourceKinds)
                    {
                        Trace.WriteLine(String.Format("У объекта \"{0}\" изменился список видов поступающей информации c \"{1}\" на \"{2}\"",
                            FullName, DataSourceKinds, toDataClassifier.DataSourceKinds));
                        
                        this.DataSourceKinds = toDataClassifier.DataSourceKinds;
                    }

                    SchemeClass.Instance.DDLDatabase.RunScript(
                        _scriptingEngine.CreateDependentScripts(this, null).
                            ToArray());
                }
            }
            catch (Exception e)
            {
                this.dataSourceParameter = rollbakDataSourceParameter;
                this.DataSourceKinds = rollbakDataSourceKinds;

                Trace.TraceError(String.Format("При изменении параметра деления по источникам возникло исключение: {0}", e.Message));
            }
        }

        /// <summary>
        /// Создание копии данных источника.
        /// </summary>
        /// <param name="db">Объект доступа к базе данных</param>
        /// <param name="fromSourceID">Исходный источник данных</param>
        /// <param name="toSourceID">Целевой источник данных</param>
        private void CopyDataSource(Database db, int fromSourceID, int toSourceID)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Изменение деления по источникам
        /// </summary>
        /// <param name="context"></param>
        /// <param name="toClassifier">Объект к виду которого будет приведен текущий объект</param>
        // TODO СУБД зависимый код
        private void UpdateDivide(ModificationContext context, DataSourceDividedClass toClassifier)
        {
            Database db = context.Database;

            if (IsDivided != toClassifier.IsDivided)
            {
                Trace.WriteLine(String.Format("У объекта \"{0}\" изменилось Деление по источникам c \"{1}\" на \"{2}\"",
                                              FullName, IsDivided, toClassifier.IsDivided));

                // Устанавливаем деление по источникам
                if (toClassifier.IsDivided)
                {
                    try
                    {
                        List<string> script = new List<string>();

                        Attributes.Add(DataAttribute.SystemSourceID);
                        script.AddRange(((EntityDataAttribute) DataAttribute.SystemSourceID).AddScript(this, false, true));

                        if (toClassifier is DataClassifier &&
                            ((DataClassifier) toClassifier).Levels.HierarchyType == HierarchyType.ParentChild &&
                            ((DataClassifier) this).Levels.HierarchyType == HierarchyType.ParentChild)
                        {
                            Attributes.Add(DataAttribute.SystemCubeParentID);
                            script.AddRange(((EntityDataAttribute) DataAttribute.SystemCubeParentID).AddScript(this,
                                                                                                               true,
                                                                                                               true));
                        }

                        if (0 ==
                            Convert.ToInt32(db.ExecQuery(String.Format("select count(*) from {0}", this.FullDBName),
                                                         QueryResultTypes.Scalar)))
                        {
                            script.AddRange(
                                EntityDataAttribute.ScriptingEngine.ModifyScript(
                                    (EntityDataAttribute) DataAttribute.SystemSourceID, this, false, true, false));
                            SchemeClass.Instance.DDLDatabase.RunScript(script.ToArray());
                        }
                        else
                        {
                            // Key - Таблица в которой есть данные по источнику; Value - список источников
                            Dictionary<EntityAssociation, List<int>> entitiesReferenced =
                                new Dictionary<EntityAssociation, List<int>>();

                            // Получаем список источников из таблиц, которые ссылаются обновляемый объект
                            foreach (EntityAssociation entityAssociation in Associated.Values)
                            {
                                if (entityAssociation.RoleA is DataSourceDividedClass &&
                                    ((DataSourceDividedClass) entityAssociation.RoleA).IsDivided)
                                {
                                    List<int> sourcesIDs =
                                        ((DataSourceDividedClass) entityAssociation.RoleA).GetDataSourcesID(db);
                                    if (sourcesIDs.Count > 0)
                                        entitiesReferenced.Add(entityAssociation, sourcesIDs);
                                }
                            }

                            if (entitiesReferenced.Count == 0)
                                throw new Exception(
                                    "Невозможно добавить обятательный атрибут SourceID, так как в таблице присутствуют данные.");

                            int firstSourceID = 0;

                            // Формируем список источников
                            List<int> referencedSources = new List<int>();
                            foreach (List<int> item in entitiesReferenced.Values)
                            {
                                foreach (int srcID in item)
                                    if (!referencedSources.Contains(srcID))
                                        referencedSources.Add(srcID);
                            }

                            foreach (int srcID in referencedSources)
                            {
                                firstSourceID = srcID;
                                break;
                            }

                            //referencedSources.Remove(firstSourceID);

                            // Добавляем поле без ограничения обязательности
                            script.AddRange(((EntityDataAttribute) DataAttribute.SystemSourceID).AddScript(this, false,
                                                                                                           true));
                            // Устанавливаем значение по умолчанию
                            script.Add(String.Format("UPDATE {0} set {1} = {2}", this.FullDBName,
                                                     DataAttribute.SourceIDColumnName, firstSourceID));
                            // Делаем поле обязательным
                            script.AddRange(
                                EntityDataAttribute.ScriptingEngine.ModifyScript(
                                    (EntityDataAttribute) DataAttribute.SystemSourceID, this, false, true, false));
                            SchemeClass.Instance.DDLDatabase.RunScript(script.ToArray());

                            UpdateFixedRows(db, firstSourceID);

                            DataTable sourceTable = new DataTable();
                            DataTable sourceTableEnumerable = new DataTable();
                            IDataUpdater du = this.GetDataUpdater(db);
                            du.Fill(ref sourceTable);
                            du.Fill(ref sourceTableEnumerable);

                            foreach (int sourceID in referencedSources)
                            {
                                // - Копируем данные источника построчно
                                if (sourceID == firstSourceID)
                                    continue;

                                UpdateFixedRows(db, firstSourceID);

                                foreach (DataRow row in sourceTableEnumerable.Rows)
                                {
                                    if (Convert.ToInt32(row[DataAttribute.RowTypeColumnName]) != 0)
                                        continue;

                                    DataRow newRow = sourceTable.NewRow();

                                    foreach (DataColumn col in sourceTable.Columns)
                                        newRow[col.ColumnName] = row[col.ColumnName];

                                    int prevID = Convert.ToInt32(newRow[DataAttribute.IDColumnName]);
                                    newRow[DataAttribute.IDColumnName] = this.GetGeneratorNextValue;
                                    newRow[DataAttribute.SourceIDColumnName] = sourceID;
                                    if (Attributes.ContainsKey(DataAttribute.ParentIDColumnName))
                                        newRow[DataAttribute.ParentIDColumnName] = DBNull.Value;
                                    if (Attributes.ContainsKey(DataAttribute.CubeParentIDColumnName))
                                        newRow[DataAttribute.CubeParentIDColumnName] = DBNull.Value;

                                    sourceTable.Rows.Add(newRow);

                                    du.Update(ref sourceTable);

                                    // - Перекидываем ссылки
                                    foreach (KeyValuePair<EntityAssociation, List<int>> item in entitiesReferenced)
                                    {
                                        if (item.Value.Contains(sourceID))
                                        {
                                            db.ExecQuery(String.Format(
                                                             "update {0} set {1} = ? where {1} = ? and {2} = ?",
                                                             item.Key.RoleA.FullDBName, item.Key.FullDBName,
                                                             DataAttribute.SourceIDColumnName),
                                                         QueryResultTypes.NonQuery,
                                                         db.CreateParameter(item.Key.FullDBName + "1",
                                                                            Convert.ToInt32(
                                                                                newRow[DataAttribute.IDColumnName])),
                                                         db.CreateParameter(item.Key.FullDBName + "2", prevID),
                                                         db.CreateParameter(DataAttribute.SourceIDColumnName, sourceID));
                                        }
                                    }
                                }
                            }

                            if (this is DataClassifier)
                            {
                                if (Attributes.ContainsKey(DataAttribute.CubeParentIDColumnName))
                                    ((DataClassifier) this).FormCubesHierarchy();
                            }
                            //throw new Exception("Невозможно добавить обятательный атрибут SourceID, так как в таблице присутствуют данные.");
                        }

                        // !!! Если классификатор не пустой, то для применения необходим ID источника для установки значения
                        Trace.WriteLine("Атрибут SourceID был успешно добавлен.");
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(
                            String.Format("При изменении параметра деления по источникам возникло исключенеи: {0}",
                                          e.Message));

                        if (Attributes.ContainsKey(DataAttribute.SourceIDColumnName))
                            Attributes.Remove(DataAttribute.SourceIDColumnName);

                        if (toClassifier is DataClassifier &&
                            ((DataClassifier) toClassifier).Levels.HierarchyType == HierarchyType.ParentChild)
                            if (Attributes.ContainsKey(DataAttribute.CubeParentIDColumnName))
                                Attributes.Remove(DataAttribute.CubeParentIDColumnName);
                    }
                }
                    // Убираем деление по источникам
                else
                {
                    try
                    {
                        string[] script =
                            (((DataSourceDividedClassScriptingEngine) _scriptingEngine).
                                CreateDataSourceLockTriggersScript(this, DataAttribute.SystemSourceID)).ToArray();
                        SchemeClass.Instance.DDLDatabase.RunScript(script);
                        dataSourceParameter = null;
                        dataSourceKinds = String.Empty;
                        script = ((EntityDataAttribute) DataAttribute.SystemSourceID).DropScript(this).ToArray();
                        SchemeClass.Instance.DDLDatabase.RunScript(script);
                        Attributes.Remove(DataAttribute.SourceIDColumnName);
                        Trace.WriteLine("Деление по источникам было успешно убрано.");
                    }
                    catch
                    {
                        if (!Attributes.ContainsKey(DataAttribute.SourceIDColumnName))
                            Attributes.Add(DataAttribute.SystemSourceID);
                    }
                }
            }
        }

        internal override void Save2Xml(XmlNode node)
        {
            base.Save2Xml(node);

            if (supportDivide)
            {
                if (!(this is IBridgeClassifier))
                {
                    XmlHelper.SetAttribute(node, "takeMethod", SchemeClass.GetTakeMethodString(SubClassType));
                }

                // Убрать после того, как методологи заполнят коллекцию видов поступающей информации
                if (!String.IsNullOrEmpty(Instance.dataSourceParameter))
                    XmlHelper.SetAttribute(node, "dataSourceParameter", Instance.dataSourceParameter);
 
                if (!String.IsNullOrEmpty(Instance.dataSourceKinds))
                    XmlHelper.SetAttribute(node, "dataSourceKinds", Instance.dataSourceKinds);
            }
        }

        internal override void Save2XmlDocumentation(XmlNode node)
        {
            base.Save2XmlDocumentation(node);

            if (supportDivide)
            {
                if (!(this is IBridgeClassifier))
                {
                    XmlHelper.SetAttribute(node, "takeMethod", SchemeClass.GetTakeMethodString(SubClassType));
                }

                if (!String.IsNullOrEmpty(Instance.dataSourceParameter))
                    XmlHelper.SetAttribute(node, "dataSourceParameter", Instance.dataSourceParameter);
            }
        }

        /// <summary>
        /// Определяет делится ли объект по источникам или нет
        /// </summary>
        public virtual bool IsDivided
        {
            get
            {
                if (!supportDivide)
                    throw new Exception("Объект не поддерживает свойство DataSourceParameter");

               if (!String.IsNullOrEmpty(Instance.dataSourceKinds))
                   return true;

                // Убрать после того, как методологи заполнят коллекцию видов поступающей информации
                return !String.IsNullOrEmpty(dataSourceParameter);
            }
        }

        /// <summary>
        /// Парамерты источника по которым делятся данные
        /// Вычисляемый параметр
        /// </summary>
        public ParamKindTypes DataSourceParameter(int sourceID)
        {
            if (!supportDivide)
                throw new Exception("Объект не поддерживает свойство DataSourceParameter");
            
            // будем вычислять по указанным видам поступающей информации
            IDataSource dataSource = SchemeClass.Instance.DataSourceManager.DataSources[sourceID];
            if (dataSource == null)
                throw new Exception(String.Format("Источник с ID {0} не найден", sourceID));

            return dataSource.ParametersType;
        }

        /// <summary>
        /// Коллекции видов источников информации
        /// </summary>
        public virtual string DataSourceKinds
        {
            get
            {
                return Instance.dataSourceKinds;
            }
            set
            {
                SetInstance.dataSourceKinds = value;
            }
        }       

        /// <summary>
        /// Проверяет права на просмотр объекта для текущего пользователя
        /// </summary>
        /// <returns>true - если у пользлвателя есть права на просмотр</returns>
        public override bool CurrentUserCanViewThisObject()
        {
            return false;
        }

        public virtual int UpdateFixedRows(IDatabase db, int sourceID) { throw new NotImplementedException(); }

        public virtual int UpdateFixedRows(int sourceID)
        {
            IDatabase db = null;
            try
            {
                db = SchemeClass.Instance.SchemeDWH.DB;
                return UpdateFixedRows(db, sourceID);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        /// <summary>
        /// Восвращает массив ID источников данных, по которым заведены данные, без записи с SourceID -1
        /// </summary>
        /// <param name="db"></param>
        /// <returns>Массив ID источников данных</returns>
        internal List<int> GetDataSourcesID(Database db)
        {
            List<int> list = new List<int>();
            if (IsDivided)
            {
                DataTable dt = (DataTable)db.ExecQuery(String.Format("select distinct SourceID from {0}", FullDBName), QueryResultTypes.DataTable);
                foreach (DataRow row in dt.Rows)
                    if (-1 != Convert.ToInt32(row[0]))
                        list.Add(Convert.ToInt32(row[0]));
            }
            return list;
        }

        /// <summary>
        /// Создает в таблице для каждого источника фиксироыанные записи
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sourcesID">Список ID источников данных для которых необходимо создать фиксированные записи</param>
        internal void FillDataSourceFixedRows(Database db, List<int> sourcesID)
        {
            if (IsDivided)
                foreach (int sourceID in sourcesID)
                    UpdateFixedRows(db, sourceID);
        }

        public override Dictionary<string, string> GetSQLMetadataDictionary()
        {
            Dictionary<string, string> sqlMetadata = base.GetSQLMetadataDictionary();

            if (IsDivided)
                sqlMetadata.Add(SQLMetadataConstants.DataSourceLockTrigger, DataSourceDividedClassScriptingEngine.GetDataSourceLockTriggerName(FullDBName, ShortName, DataAttribute.SourceIDColumnName));

            return sqlMetadata;
        }

        internal override XmlDocument PostInitialize()
        {
            XmlDocument doc = base.PostInitialize();
            if (ID > 0 && (ClassType == ClassTypes.clsDataClassifier || ClassType == ClassTypes.clsFactData) && IsDivided)
            {
                Database db = null;
                try
                {
                    db = (Database)SchemeClass.Instance.SchemeDWH.DB;
                    List<string> script = new List<string>();
                    if (!_scriptingEngine.ExistsObject(DataSourceDividedClassScriptingEngine.GetDataSourceLockTriggerName(FullDBName, FullDBShortName, DataAttribute.SourceIDColumnName), ObjectTypes.Trigger)
                        || (SchemeDWH.Instance.DatabaseVersion == "2.4.0.3" && SchemeClass.Instance.NeedUpdateScheme))
                    {
                        script = ((DataSourceDividedClassScriptingEngine)_scriptingEngine).CreateDataSourceLockTriggersScript(this, DataAttribute.SystemDummy);
                    }
                    db.RunScript(script.ToArray(), false);
                }
                catch (Exception e)
                {
                    Trace.TraceError("Ошибка при создании триггера блокировки источников {0}: {1}", DataSourceDividedClassScriptingEngine.GetDataSourceLockTriggerName(FullDBName, ShortName, DataAttribute.SourceIDColumnName), e.Message);
                }
                finally
                {
                    if (db != null) db.Dispose();
                }
            }
            return doc;
        }       
    }
}

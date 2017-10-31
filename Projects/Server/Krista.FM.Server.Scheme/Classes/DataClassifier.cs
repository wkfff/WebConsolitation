using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Modifications;
using Krista.FM.Server.Scheme.ScriptingEngine;
using Krista.FM.Server.Scheme.ScriptingEngine.Classes;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Классификатор данных
    /// </summary>
    internal class DataClassifier : Classifier 
	{
        /// <summary>
        /// Инициализация классификатора данных
        /// </summary>
        /// <param name="owner">Родительский объект</param>
        /// <param name="semantic">Семантика</param>
        /// <param name="name">Имя</param>
        /// <param name="subClassType">Подкласс</param>
        /// <param name="state">Состояние</param>
        public DataClassifier(string key, ServerSideObject owner, string semantic, string name, SubClassTypes subClassType, ServerSideObjectStates state)
            : this(key, owner, semantic, name, subClassType, state, SchemeClass.ScriptingEngineFactory.ClassifierEntityScriptingEngine)
        {
        }

        /// <summary>
        /// Конструктор объекта
        /// </summary>
        /// <param name="name">Имя объекта</param>
        public DataClassifier(string key, ServerSideObject owner, string semantic, string name, SubClassTypes subClassType, ServerSideObjectStates state, ScriptingEngine.ScriptingEngineAbstraction scriptingEngine)
            : base(key, owner, semantic, name, ClassTypes.clsDataClassifier, subClassType, true, state, scriptingEngine)
		{
            tagElementName = "DataCls";
        }

        /// <summary>
        /// Инициализирует коллекцию атрибутов объекта по информации из XML настроек 
        /// </summary>
        /// <param name="doc">Документ с XML настройкой</param>
        /// <param name="atagElementName">наименование тега с настройками объекта</param>
        protected override void InitializeAttributes(XmlDocument doc, string atagElementName)
        {
            // Если делится по источникам, то добавляем поле источника
            if (IsDivided)
                Attributes.Add(DataAttribute.SystemSourceID);

            // Добавляем системные атрибуты
            if (SubClassType == SubClassTypes.Pump)
            {
                Attributes.Add(DataAttribute.SystemPumpID);
                Attributes.Add(DataAttribute.SystemSourceKey);
            }
            else if (SubClassType == SubClassTypes.PumpInput)
            {
                Attributes.Add(DataAttribute.SystemPumpIDDefault);
                Attributes.Add(DataAttribute.SystemSourceKey);
            }

            base.InitializeAttributes(doc, atagElementName);
        }

        /// <summary>
        /// Инициализация плоской иерархии
        /// </summary>
        protected override void InitializeDefaultRegularHierarchy()
        {
            base.InitializeDefaultRegularHierarchy();

            if (Attributes.ContainsKey(DataAttribute.CubeParentIDColumnName))
                Attributes.Remove(DataAttribute.CubeParentIDColumnName);
        }

        /// <summary>
        /// Инициализация деревянной иерархии
        /// </summary>
        protected override void InitializeDefaultParentChildHierarchy()
        {
            Levels.Clear();

            Levels.HierarchyType = HierarchyType.ParentChild;

            // Уровень All
            string allLevelName = "Все";
            string parentKeyName = DataAttribute.ParentIDColumnName;
            if (IsDivided)
            {
                allLevelName = "Данные всех источников";
                parentKeyName = DataAttribute.CubeParentIDColumnName;

                if (!Attributes.ContainsKey(DataAttribute.CubeParentIDColumnName))
                    Attributes.Add(DataAttribute.SystemCubeParentID);
            }
            RegularLevel allLevel = new RegularLevel(Guid.Empty.ToString(), this);
            allLevel.LevelType = LevelTypes.All;
            allLevel.Name = allLevelName;
            Levels.Add(allLevel.Name, allLevel);

            if (!Attributes.ContainsKey(DataAttribute.ParentIDColumnName))
                Attributes.Add(DataAttribute.SystemParentID);

            // Деревянный уровень
            ParentChildLevel level = new ParentChildLevel(Guid.NewGuid().ToString(), this);
            level.Name = this.SemanticCaption;
            level.LevelType = LevelTypes.Regular;
            level.MemberKey = Attributes[DataAttribute.IDColumnName];
            level.MemberName = Attributes[DataAttribute.IDColumnName];
            level.ParentKey = DataAttributeCollection.GetAttributeByKeyName(Attributes, parentKeyName, parentKeyName);
            level.LevelNamingTemplate = "Уровень 1; Уровень 2; Уровень 3; Уровень *";
            Levels.Add(level.ObjectKey, level);
        }

        internal override XmlDocument Initialize()
        {
            XmlDocument doc = base.Initialize();

            if (Levels.HierarchyType == HierarchyType.ParentChild && IsDivided)
                if (!Attributes.ContainsKey(DataAttribute.CubeParentIDColumnName))
                {
                    Debugger.Break();
                    Attributes.Add(DataAttribute.SystemCubeParentID);
                }
            return doc;
        }

        /// <summary>
        /// Устанавливает подкласс объекта, изменяя его структуру
        /// </summary>
        /// <param name="value">Устанавливаемый подкласс</param>
        protected override void ChangeSubClassType(SubClassTypes value)
        {
            // Input -> Pump
            if (value == SubClassTypes.Pump && SubClassType == SubClassTypes.Input)
            {
                Attributes.Add(DataAttribute.SystemPumpID);
                Attributes.Add(DataAttribute.SystemSourceKey);
            }
            // Input -> PumpInput
            else if (value == SubClassTypes.PumpInput && SubClassType == SubClassTypes.Input)
            {
                Attributes.Add(DataAttribute.SystemPumpIDDefault);
                Attributes.Add(DataAttribute.SystemSourceKey);
            }
            // Pump -> Input
            else if (value == SubClassTypes.Input && SubClassType == SubClassTypes.Pump)
            {
                Attributes.Remove(DataAttribute.PumpIDColumnName);
                Attributes.Remove(DataAttribute.SourceKeyColumnName);
            }
            // Pump -> PumpInput
            else if (value == SubClassTypes.PumpInput && SubClassType == SubClassTypes.Pump)
            {
            }
            // PumpInput -> Pump
            else if (value == SubClassTypes.Pump && SubClassType == SubClassTypes.PumpInput)
            {
            }
            // PumpInput -> Input
            else if (value == SubClassTypes.Input && SubClassType == SubClassTypes.PumpInput)
            {
                Attributes.Remove(DataAttribute.PumpIDColumnName);
                Attributes.Remove(DataAttribute.SourceKeyColumnName);
            }
            else
                throw new ArgumentException("Неверное значение свойства. Свойство может принимать значения Input, Pump или PumpInput.");
        }

#if ScriptingEengine
        /// <summary>
        /// Формирует определение выражений для короткого наименования
        /// </summary>
        /// <param name="shortNameHeader">Часть определения атрибута</param>
        /// <param name="shortNameSelectPart">Часть выражения select</param>
        /// <returns></returns>
        private bool GenerateShortNameParts(out string shortNameHeader, out string shortNameSelectPart)
        {
            string memberName = String.Empty;
            foreach (IDimensionLevel level in Levels.Values)
            {
                if (level.LevelType == LevelTypes.All)
                    continue;
                memberName = level.MemberName.Name;
            }

            shortNameHeader = String.Empty;
            shortNameSelectPart = String.Empty;
            if (memberName != String.Empty)
            {
                if (Attributes[memberName].Size > 255)
                {
                    shortNameHeader = ", Short_Name";
                    shortNameSelectPart = String.Format(", cast(T.{0} as varchar2(255))", memberName);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Создание дополнительных представлений для построения кубов
        /// </summary>
        /// <param name="withoutAttribute">Атрибут, который не будет учитываться при создании зависимых объектов</param>
        /// <returns>DDL-определения представоений</returns>
        protected override string[] CreateViews(DataAttribute withoutAttribute)
        {
            string shortNameHeader;
            string shortNameSelectPart;

            string[] baseViews = base.CreateViews(withoutAttribute);

            bool generateShortName = GenerateShortNameParts(out shortNameHeader, out shortNameSelectPart);

            // Создаем представление только если классификатор делится 
            // по источникам и имеет плоскую иерархию
            if (!((Levels.HierarchyType == HierarchyType.Regular && IsDivided) || generateShortName))
                return baseViews;

            string[] views = new string[baseViews.GetLength(0) + 1];
            
            baseViews.CopyTo(views, 0);

            List<string> attributesNames = new List<string>();

            // Перебираем все атрибуты классификатора, кроме ссылочных
            foreach (DataAttribute item in Attributes.Values)
            {
                if (item.Class == DataAttributeClassTypes.Reference)
                    continue;

                // Пропускаем атрибут, который не нужно учитывать
                if (item == withoutAttribute)
                    continue;

                attributesNames.Add(item.Name);
            }

            // Перебираем все ассоциации
            foreach (EntityAssociation item in Associations.Values)
            {
                // Пропускаем атрибут, который не нужно учитывать
                if (item.FullDBName == withoutAttribute.Name)
                    continue;

                if (item.DbObjectState == DBObjectStateTypes.InDatabase 
                    || item.DbObjectState == DBObjectStateTypes.Changed 
                    || item.InUpdating)
                {
                    attributesNames.Add(item.FullDBName);
                }
            }

            string dataSourceNameHeader = String.Empty;
            string dataSourceNameSelect = String.Empty;
            string dataSourceJoinClause = String.Empty;
            if (Levels.HierarchyType == HierarchyType.Regular && IsDivided)
            {
                dataSourceNameHeader = ", DataSourceName";
                dataSourceNameSelect = ",\nDS.\"SUPPLIERCODE\" ||' '|| DS.\"DATANAME\" ||' - '|| CASE DS.\"KINDSOFPARAMS\" WHEN 0 THEN DS.\"NAME\" || ' ' || DS.\"YEAR\"  WHEN 1 THEN cast(DS.\"YEAR\" as varchar(4)) WHEN 2 THEN DS.\"YEAR\" || ' ' || DS.\"MONTH\"  WHEN 3 THEN DS.\"YEAR\" || ' ' || DS.\"MONTH\" || ' ' || DS.\"VARIANT\"  WHEN 4 THEN DS.\"YEAR\" || ' ' || DS.\"VARIANT\" WHEN 5 THEN DS.\"YEAR\" || ' ' || DS.\"QUARTER\" WHEN 6 THEN DS.\"YEAR\" || ' ' || DS.\"TERRITORY\" END";
                dataSourceJoinClause = " LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID)";
            }

            string viewName = "DV" + this.FullDBName.Substring(1);

            viewName = viewName.Substring(0, viewName.Length > 30 ? 30 : viewName.Length);

            string view = String.Format(
                "CREATE OR REPLACE VIEW {0} ({1}{5}{6})\nAS " + 
                "SELECT T.{2}{3}\n{7}\nFROM {4} T{8}",
                viewName,                                       // 0 - Наименование представления
                String.Join(", ", attributesNames.ToArray()),   // 1 - Атрибуты классификатора
                String.Join(", T.", attributesNames.ToArray()), // 2 - Атрибуты классификатора
                dataSourceNameSelect,                           // 3 - Вычисляемое поле DataSourceName
                this.FullDBName,                                // 4 - Наименование таблицы классификатора
                dataSourceNameHeader,                           // 5 - 
                shortNameHeader,
                shortNameSelectPart,
                dataSourceJoinClause);           

            views[baseViews.GetLength(0)] = view;

            return views;
        }
#endif
        internal override void Create(ModificationContext context)
        {
            // Добавляем служебные поля 
            if (Semantic == VariantDataClassifier.VariantSemantic)
            {
                if (!Attributes.ContainsKey(DataAttribute.SystemVariantCompleted.Name))
                    Attributes.Add(DataAttribute.SystemVariantCompleted);
                if (!Attributes.ContainsKey(DataAttribute.SystemVariantComment.Name))
                    Attributes.Add(DataAttribute.SystemVariantComment);
            }

            base.Create(context);

            RegisterObject(ObjectKey, FullCaption, SysObjectsTypes.DataClassifier);

            // Для классификаторов, которые делятся по источникам фиксированные записи не создаются
            // Фиксированные записи должны вставляться при создании ассоциации, 
            // т.е. только в этот момент можно получить список источников для которых нужно создавать фиксированные записи
            //if (!IsDivided)
                InsertData(context);
        }

        public const int FixedDataSourcesRowsShift = 1000;

        internal static int GetFixedRowID(int sourceID)
        {
            return -sourceID - FixedDataSourcesRowsShift;
        }

        /// <summary>
        /// Обновляет фиксированные записи в БД
        /// </summary>
        /// <param name="DB"></param>
        public override int UpdateFixedRows(IDatabase db, int sourceID)
        {
            try
            {
                if (0 == Convert.ToInt32(db.ExecQuery(
                    String.Format(
                        "select count(ID) from {0} where ID = ? and {1} <> 0",
                        FullDBName, DataAttribute.RowTypeColumnName),
                    QueryResultTypes.Scalar, db.CreateParameter("ID", GetFixedRowID(sourceID)))))
                {

                    List<string> attribeteNames = new List<string>();
                    List<string> attribeteValues = new List<string>();
                    // Обращаемся к коллекции атрибутов без учета представлений
                    foreach (EntityDataAttribute attr in dataAttributeCollection.Values)
                    {
                        if (attr is EntityAssociationAttribute)
                        {
                            if (attr.Owner.State == ServerSideObjectStates.New)
                                continue;
                        }
                        if (attr.Name == "ID")
                        {
                            attribeteNames.Add(attr.Name);
                            attribeteValues.Add(Convert.ToString(GetFixedRowID(sourceID)));
                        }
                        else if (attr.Class == DataAttributeClassTypes.Reference)
                        {
                            DataSourceDividedClass dsdClass = ((EntityAssociation)attr.Owner).RoleB as DataSourceDividedClass;
                            int fixedRowID = dsdClass != null && dsdClass.IsDivided
                                                 ? dsdClass.UpdateFixedRows(db, sourceID)
                                                 : (attr.DefaultValue == null ? -1 : (int) attr.DefaultValue);
                            attribeteNames.Add(attr.Name);
                            attribeteValues.Add(Convert.ToString(fixedRowID));
                        }
                        else if (attr.Name == "SourceID")
                        {
                            attribeteNames.Add(attr.Name);
                            attribeteValues.Add(Convert.ToString(sourceID));
                        }
                        else if (attr.Name == "CodeStr")
                        {
                            attribeteNames.Add(attr.Name);
                            attribeteValues.Add("'0'");
                        }
                        else if (attr.Name == DataAttribute.RowTypeColumnName)
                        {
                            attribeteNames.Add(attr.Name);
                            attribeteValues.Add("1");
                        }
                        else if (!attr.IsNullable)
                        {
                            attribeteNames.Add(attr.Name);
                            switch (attr.Type)
                            {
                                case DataAttributeTypes.dtInteger:
                                case DataAttributeTypes.dtDouble:
                                case DataAttributeTypes.dtBoolean:
                                    attribeteValues.Add("0");
                                    break;
                                case DataAttributeTypes.dtString:
                                    string unknownData = "Неизвестные данные";
                                    attribeteValues.Add("'" + (attr.Size < unknownData.Length ? unknownData.Substring(0, attr.Size) : unknownData) + "'");
                                    break;
                            }
                        }
                    }

                    string queryText = String.Format("insert into {0} ({1}) values ({2})",
                        this.FullDBName,
                        String.Join(", ", attribeteNames.ToArray()),
                        String.Join(", ", attribeteValues.ToArray()));

                    db.ExecQuery(queryText, QueryResultTypes.NonQuery);
                }
                return GetFixedRowID(sourceID);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw new Exception(e.Message, e);
            }
            finally
            {
            }
        }

        public override string TablePrefix
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get
            {
                if (ClassType == ClassTypes.clsDataClassifier)
                    return "d";
                else if (ClassType == ClassTypes.clsFixedClassifier)
                    return "fx";
                else
                    throw new Exception("BUG: У классификатора данных установлен недопустимый класс " + ClassType.ToString());
            }
        }

        /// <summary>
        /// Полное имя объекта
        /// </summary>
        public override string FullName
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return "d." + base.FullName; }
        }

        #region Формирование иерархии для кубов

        /// <summary>
        /// Находит корневую запись для указанного источника, если она найдена, то возвращает её ID, иначе null
        /// </summary>
        /// <param name="dt">Таблица в которой производится поиск</param>
        /// <param name="sourceID">ID источника данных</param>
        /// <returns>если найдена корневая запись, то возвращает её ID, иначе null</returns>
        private int? FindRootRowHierarchy(DataTable dt, int sourceID)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToInt32(row["SourceID"]) == sourceID)
                    return Convert.ToInt32(row["ID"]);
            }
            return null;
        }

        /// <summary>
        /// Фопмирует иерархию для построения по ней измерений
        /// </summary>
        /// <returns>количество обработанных записей</returns>
        public override int FormCubesHierarchy()
        {
            int reсordsAffected = 0;

            if (!(Levels.HierarchyType == HierarchyType.ParentChild && IsDivided))
                return 0;

            string errorMessage = "Для корректной работы метода FormCubesHierarchy необходимо чтобы у объекта был атрибут ";
            if (!Attributes.ContainsKey(DataAttribute.CubeParentIDColumnName))
                throw new Exception(errorMessage + DataAttribute.CubeParentIDColumnName);
            //if (!Attributes.ContainsKey("Code") && !Attributes.ContainsKey("CodeStr") && !Attributes.ContainsKey("LongCode")) throw new Exception(errorMessage + "Code, CodeStr или LongCode");
            if (DataAttributeCollection.GetAttributeByKeyName(Attributes, "Name", "Name") == null) 
                throw new Exception(errorMessage + "Name");

            Database DB = (Database)SchemeClass.Instance.SchemeDWH.DB;

            try
            {
                DB.BeginTransaction();
                DB.CommandTimeout = 60*60;

                SchemeClass.Instance.DDLDatabase.RunScript(ScriptingEngine.DisableAllTriggers(FullDBName).ToArray());

                // Удаляем все ранее созданные корневые записи для которых выполняется условие SourceID = -ParentID
                DB.ExecQuery(String.Format("delete from {0} where SourceID = -ParentID and ID = CubeParentID and RowType = 2", FullDBName), QueryResultTypes.NonQuery);

                // Получаем все ранее созданные корневые записи
                DataTable dtRootRows = (DataTable)DB.ExecQuery(String.Format("select ID, SourceID from {0} where /*SourceID = -ParentID and*/ ID = CubeParentID and RowType = 2", FullDBName), QueryResultTypes.DataTable);                     
                                                                              
                // Сбрасываем ранее установленную иерархию у всех источников
                // исключаем из запроса служебные корневые записи
                DB.ExecQuery(String.Format("update {0} set CubeParentID = null where not (/*SourceID = -ParentID and*/ ID = CubeParentID and RowType = 2)", FullDBName), QueryResultTypes.NonQuery);
                
                // Находим все источники у которых будем устанавливать иерархию
                DataTable dt = (DataTable)DB.ExecQuery(
                    String.Format("select distinct SourceID, SourceID as NewID from {0} where (SourceID is not null) and (ParentID is null) and (CubeParentID is null) and ((ID > 0) or (ID < -1000)) and RowType <> 2", FullDBName), QueryResultTypes.DataTable);
                                
                foreach (DataRow row in dt.Rows)
                {
                    // ID берем из генератора, MemberKey и ParentKey устанавливаем равным -SourceID
                    int sourceID = Convert.ToInt32(row[0]);
                    
                    int? rootID = FindRootRowHierarchy(dtRootRows, sourceID);

                    int newID = -1;

                    if (rootID == null)
                        newID = this.GetGeneratorNextValue;
                    else
                        newID = (int)rootID;
                    
                    /*object CodeAttr;
                    string CodeAttrName;

                    if (Attributes.ContainsKey("Code"))
                    {
                        CodeAttr = sourceID; 
                        CodeAttrName = "Code";
                    }
                    else if (Attributes.ContainsKey("CodeStr"))
                    {
                        CodeAttr = Convert.ToString(sourceID);
                        CodeAttrName = "CodeStr";
                    }
                    else if (Attributes.ContainsKey("LongCode"))
                    {
                        CodeAttr = Convert.ToString(sourceID);
                        CodeAttrName = "LongCode";
                    }
                    else
                        throw new Exception("Классификатор не содержит атрибут Code или CodeStr или LongCode.");
                    */
                    // Установка значений для всех обязательных полей
                    List<string> attribeteNames = new List<string>();
                    List<string> attribeteValues = new List<string>();
                    foreach (EntityDataAttribute attr in Attributes.Values)
                    {
                        if (attr.Name == "ID" || attr.Name == "RowType" || attr.Name == "SourceID" || /*attr.Name == "Code" || attr.Name == "CodeStr" 
                                || attr.Name == "LongCode" ||*/ attr.Name == "Name" || attr.Name == "ParentID" || attr.Name == "CubeParentID")
                            continue;
                        if (!attr.IsNullable)
                        {
                            attribeteNames.Add(attr.Name);

                            string defaultValue = attr.GetDefaultValue;

                            if (attr.Class == DataAttributeClassTypes.Reference)
                            {
                                IClassifier cls = (IClassifier) ((IAssociation) attr.Owner).RoleBridge;
                                if (cls.ClassType == ClassTypes.clsDataClassifier && cls.IsDivided)
                                {
                                    defaultValue = Convert.ToString(cls.UpdateFixedRows(DB, sourceID));
                                }
                                else
                                {
                                    if (String.IsNullOrEmpty(defaultValue))
                                    {
                                        defaultValue = "-1";
                                    }
                                }
                            }
                            else
                            {
                                if (defaultValue == String.Empty)
                                {
                                    switch (attr.Type)
                                    {
                                        case DataAttributeTypes.dtBoolean:
                                        case DataAttributeTypes.dtInteger:
                                        case DataAttributeTypes.dtDouble:
                                            defaultValue = "-1";
                                            break;
                                        case DataAttributeTypes.dtChar:
                                        case DataAttributeTypes.dtString:
                                            defaultValue = "'*'";
                                            break;
                                        case DataAttributeTypes.dtDate:
                                        case DataAttributeTypes.dtDateTime:
                                            defaultValue = "SYSDATE";
                                            break;
                                        default:
                                            throw new Exception("Тип данных не поддерживается.");
                                    }
                                }
                            }
                            attribeteValues.Add(defaultValue);
                        }
                    }

                    if (rootID == null)
                    {
                        // Вставляем корневую запись для иерархии источника.
                        DB.ExecQuery(String.Format(
                            "insert into {0} (ID, RowType, SourceID, Name, ParentID, CubeParentID{1}) values (?, ?, ?, ?, ?, ?{2})",
                                FullDBName, 
                                attribeteNames.Count == 0 ? String.Empty : ", " + String.Join(", ", attribeteNames.ToArray()),
                                attribeteValues.Count == 0 ? String.Empty : ", " + String.Join(", ", attribeteValues.ToArray())),
                            QueryResultTypes.NonQuery,
                            DB.CreateParameter("ID", newID),
                            DB.CreateParameter("RowType", 2),
                            DB.CreateParameter("SourceID", sourceID),
                            DB.CreateParameter("Name", SchemeClass.Instance.DataSourceManager.GetDataSourceName(sourceID)),
                            DB.CreateParameter("ParentID", newID),
                            DB.CreateParameter("CubeParentID", newID));
                    }

                    // Копируем иерархию
                    reсordsAffected += Convert.ToInt32(DB.ExecQuery(
                        String.Format("update {0} set CubeParentID = ParentID where CubeParentID is null and SourceID = ? and RowType <> 2", FullDBName),
                        QueryResultTypes.NonQuery, DB.CreateParameter("SourceID", Convert.ToInt32(row[0]))));

                    // Устанавливаем ссылку на служебную корневую запись источника
                    reсordsAffected += Convert.ToInt32(DB.ExecQuery(
                        String.Format("update {0} set CubeParentID = ? where ParentID is null and SourceID = ?", FullDBName),
                        QueryResultTypes.NonQuery, 
                        DB.CreateParameter("FixedRecordID", Convert.ToInt32(newID)),
                        DB.CreateParameter("SourceID", Convert.ToInt32(row[0]))));
                }

                // Удаляем лишние записи 'Неизвестные данные' в источниках по которым нет данных
                DB.ExecQuery(String.Format("delete from {0} where Name = 'Неизвестные данные' and CubeParentID is null and RowType = 1 and ID <> -999", FullDBName), QueryResultTypes.NonQuery);

                DB.Commit();
                return reсordsAffected;
            }
            catch (Exception e)
            {
                Trace.WriteLine(Krista.Diagnostics.KristaDiagnostics.ExpandException(e));
                DB.Rollback();
                throw new Exception(e.Message, e);
            }
            finally
            {
                SchemeClass.Instance.DDLDatabase.RunScript(ScriptingEngine.EnableAllTriggersForObject(FullDBName).ToArray());
                DB.Dispose();
            }
        }

        #endregion Формирование иерархии для кубов

        /// <summary>
        /// Выполняет дополнительную обработку данных в таблице.
        /// </summary>
        public override bool ProcessObjectData()
        {
            if ((Levels.HierarchyType == HierarchyType.ParentChild && IsDivided))
            {
                DateTime startTime = DateTime.Now;
                Trace.TraceVerbose(
                    "{0} Формирование иерархии измерения для объекта \"{1}\"",
                    Authentication.UserDate, this.FullName);

                FormCubesHierarchy();

                Trace.TraceVerbose(
                    "{0} Формирование иерархии измерения для объекта \"{1}\" завершена. Время выполнения {2}",
                    Authentication.UserDate, this.FullName, DateTime.Now - startTime);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Проверяет права на просмотр объекта для текущего пользователя
        /// </summary>
        /// <returns>true - если у пользлвателя есть права на просмотр</returns>
        public override bool CurrentUserCanViewThisObject()
        {
            try
            {
                return SchemeClass.Instance.UsersManager.CheckPermissionForSystemObject(ObjectKey, (int)DataClassifiesOperations.ViewClassifier, false);
            }
            catch
            {
                return true;
            }
        }

        public override Dictionary<string, string> GetSQLMetadataDictionary()
        {
            Dictionary<string, string> sqlMetadata = base.GetSQLMetadataDictionary();

            sqlMetadata.Add(SQLMetadataConstants.DeveloperLockTrigger, ScriptingEngineImpl.DeveloperLockTriggerName(FullDBName));

            return sqlMetadata;
        }

        public override bool UniqueKeyAvailable
        {
            get { return true; }
        }
    }
}

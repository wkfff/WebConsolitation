using System;
using System.Collections.Generic;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Modifications;
using Krista.FM.Server.Scheme.ScriptingEngine;
using Krista.FM.Server.Scheme.ScriptingEngine.Classes;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
	/// <summary>
	/// Базовый класс для классификаторов данных и сопоставимых
	/// </summary>
    internal abstract partial class Classifier : DataSourceDividedClass, IClassifier
    {
        #region Поля

        // Поля для описания иерархии
        private DimensionLevelCollection levels;

        #endregion Поля

        /// <summary>
        /// Конструстор базового класса классификаторов данных и сопоставимых
        /// </summary>
        /// <param name="key"></param>
        /// <param name="owner"></param>
        /// <param name="semantic"></param>
        /// <param name="name">Английское наименование объекта</param>
        /// <param name="classType">Класс объекта</param>
        /// <param name="subClassType">Подкласс объекта</param>
        /// <param name="supportDivide"></param>
        /// <param name="state"></param>
        /// <param name="scriptingEngine"></param>
        public Classifier(string key, ServerSideObject owner, string semantic, string name, ClassTypes classType, SubClassTypes subClassType, bool supportDivide, ServerSideObjectStates state, ScriptingEngine.ScriptingEngineAbstraction scriptingEngine)
            : base(key, owner, semantic, name, classType, subClassType, supportDivide, state, scriptingEngine)
		{
            LogicalCallContextData callerContext = LogicalCallContextData.GetContext();
            try
            {
                SessionContext.SetSystemContext();

                // У всех классификаторов должно присутствовать поле определяющее тип записи
                Attributes.Add(DataAttribute.SystemRowType);

                levels = new DimensionLevelCollection(this, state);
            }
            finally
            {
                LogicalCallContextData.SetContext(callerContext);
            }
        }

        #region ServerSideObject

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected new Classifier Instance
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return (Classifier)GetInstance(); }
        }

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь, для установки значений свойств
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected new Classifier SetInstance
        {
            get
            {
                if (SetterMustUseClone())
                    return (Classifier)CloneObject;
                else
                    return this;
            }
        }

        /// <summary>
        /// Создает новый объект, являющийся копией текущего экземпляра. 
        /// </summary>
        /// <returns>Новый объект, являющийся копией текущего экземпляра.</returns>
        public override Object Clone()
        {
            Classifier clon = (Classifier)base.Clone();
            clon.levels = (DimensionLevelCollection)levels.Clone();
            return clon;
        }

        /// <summary>
        /// Снимает блокировку с объекта
        /// </summary>
        public override void Unlock()
        {
            levels.Unlock();
            base.Unlock();
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
                    levels.State = value;
                }
            }
        }

        #endregion ServerSideObject

        /// <summary>
        /// Инициализирует коллекцию атрибутов объекта по информации из XML настроек 
        /// </summary>
        /// <param name="doc">Документ с XML настройкой</param>
        /// <param name="atagElementName">наименование тега с настройками объекта</param>
        protected override void InitializeAttributes(XmlDocument doc, string atagElementName)
        {
            bool DividePresent = false;
            XmlNodeList xmlAttributes = doc.SelectNodes(String.Format("/DatabaseConfiguration/{0}/Attributes/Attribute", atagElementName));
            foreach (XmlNode xmlAttribute in xmlAttributes)
            {
                EntityDataAttribute attr = EntityDataAttribute.CreateAttribute(this, xmlAttribute, this.State);
                Attributes.Add(attr);

                if (attr.Divide != String.Empty)
                {
                    if (DividePresent)
                        throw new Exception("Свойство divide не может быть указано только у нескольких атрибутов.");

                    List<DataAttribute> list = InitializeDivideAttributes(this, attr);
                    foreach (DataAttribute item in list)
                    {
                        if (DataAttributeCollection.GetAttributeByKeyName(Attributes, item.Name, item.Name) == null)
                            Attributes.Add(item);
                    }
                    DividePresent = true;
                }
            }
        }

        internal static List<DataAttribute> InitializeDivideAttributes(Entity entity, EntityDataAttribute attr)
	    {
            List<DataAttribute> list = new List<DataAttribute>();

	        string[] divide = attr.Divide.Split('.');
	        if (divide.Length > 11)
	            throw new Exception("Количество элементов в свойстве divide не должно превышать 11.");
	        for (int i = 1; i <= divide.Length; i++)
	        {
                DataAttribute divideAttr = EntityDataAttribute.CreateAttribute(Guid.Empty.ToString(), "Code" + i, entity, AttributeClass.Regular, entity.State);
	            divideAttr.Caption = "Код " + i;
	            divideAttr.Class = DataAttributeClassTypes.Fixed;
	            divideAttr.Description = "Элемент " + i + "-й расщепленного кода";
	            divideAttr.Type = DataAttributeTypes.dtInteger;
	            divideAttr.Size = Math.Abs(Convert.ToInt32(divide[i - 1]));
	            divideAttr.IsNullable = true;
	            divideAttr.DefaultValue = 0;
	            divideAttr.Mask = String.Empty.PadLeft(divideAttr.Size, '#');
	            divideAttr.Kind = DataAttributeKindTypes.Serviced;
	            divideAttr.IsReadOnly = true;
                list.Add(divideAttr);
	        }
            return list;
	    }

	    private void InitializeFixedRows(XmlNode xmlNode)
        {
            if ((xmlNode == null) || (String.IsNullOrEmpty(xmlNode.InnerXml)))
                return;

            if (xmlNode.SelectSingleNode("File") != null)
            {
                XmlNode xmlFileNode = xmlNode.SelectSingleNode("File/@privatePath");
                XmlNode xmlValuesNode = GetFixedRowsNodeListFromFile(xmlFileNode.Value);
                xmlFixedRowsData = xmlValuesNode.OuterXml;
            }
            else
                xmlFixedRowsData = xmlNode.InnerXml;
        }

        internal override XmlDocument Initialize()
        {
            XmlDocument doc = base.Initialize();

            levels.InitializeHierarchy(doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/Hierarchy", tagElementName)));

            InitializeFixedRows(doc.SelectSingleNode(String.Format("/DatabaseConfiguration/{0}/Data", tagElementName)));

            if (Levels.HierarchyType == HierarchyType.ParentChild)
            {
                if (!Attributes.ContainsKey(DataAttribute.ParentIDColumnName))
                    Attributes.Add(DataAttribute.SystemParentID);
            }

            return doc;
        }

        /// <summary>
        /// Метод вызывается после инициализации всех объектов схемы
        /// </summary>
        internal override XmlDocument PostInitialize()
        {
            XmlDocument doc = base.PostInitialize();

            if (ID > 0)
            {
                Database db = null;
                try
                {
                    db = (Database)SchemeClass.Instance.SchemeDWH.DB;
                    List<string> script = new List<string>();
#if false //Пересоздание представлений
                    script = ((ScriptingEngine.Classes.ClassifierEntityScriptingEngine)this._scriptingEngine).CreateDependentScripts(this, DataAttribute.SystemDummy);
#endif
                    if (ClassType == ClassTypes.clsBridgeClassifier || ClassType == ClassTypes.clsDataClassifier)
                    {
						// Если нет триггера каскадного удаления, или требуется обновление
                        if (!_scriptingEngine.ExistsObject(ClassifierEntityScriptingEngine.GetCascadeDeleteTriggerName(this), ObjectTypes.Trigger) ||
                                                    SchemeDWH.Instance.DatabaseVersion == "2.4.1.0" && SchemeClass.Instance.NeedUpdateScheme)
                        {
                            // Создаем триггер каскадного удаления.
                            script.AddRange(
								((ClassifierEntityScriptingEngine)_scriptingEngine).GetCascadeDeleteTrigerScript(this));
                        }

						// Добавление индексов для поля ParentID. 
						// Добавлено в версии базы данных 2.6.0.0 и 2.5.0.10
						if (SchemeClass.Instance.NeedUpdateScheme && (
							SchemeDWH.Instance.DatabaseVersion == "2.6.0.0" ||
							SchemeDWH.Instance.DatabaseVersion == "2.5.0.12"))
						{
							script.AddRange(
								((ClassifierEntityScriptingEngine) _scriptingEngine).CreateIfNotExistsParentIdIndexScript(this));
						}
                    }
                    db.RunScript(script.ToArray(), false);
                }
                catch (Exception e)
                {
                    Trace.TraceError("Ошибка при создании в базе данных зависимых объектов для {0}: {1}", FullName, e.Message);
                }
                finally
                {
                    if (db != null) 
                        db.Dispose();
                }
            }
            
            return doc;
        }

        public override IModificationItem GetChanges(IModifiable toObject)
        {
            MajorObjectModificationItem root = (MajorObjectModificationItem)base.GetChanges(toObject);

            Classifier toClassifier = (Classifier)toObject;

            ModificationItem hierarchyModificationItem = (ModificationItem)this.Levels.GetChanges(toClassifier.Levels);
            if (hierarchyModificationItem != null)
            {
                hierarchyModificationItem.Parent = root;
                root.Items.Add(hierarchyModificationItem.Key, hierarchyModificationItem);
            }

            ModificationItem miFixedRows = GetChangesFixedRows(toClassifier);
            if (miFixedRows != null)
            {
                miFixedRows.Parent = root;
                root.Items.Add(miFixedRows.Key, miFixedRows);
            }

            return root;
        }

        internal override void Save2Xml(XmlNode node)
        {
            base.Save2Xml(node);
            
            //
            // Иерархия
            //
            ((DimensionLevelCollection)Levels).HierarchySave2Xml(node);

            //
            // Фиксированные значения
            //
            if (!String.IsNullOrEmpty(xmlFixedRowsData))
            {
                XmlNode xmlData = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Data", null);
                xmlData.InnerXml = xmlFixedRowsData;
                node.AppendChild(xmlData);
            }

            //Уникальные ключи
            if ((UniqueKeys != null) && (UniqueKeys.Count > 0))
            {
                XmlNode uniqueKeysNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "UniqueKeyList", null);
                ((UniqueKeyCollection)UniqueKeys).Save2Xml(uniqueKeysNode);
                node.AppendChild(uniqueKeysNode);
            }
        }

        internal override void Save2XmlDocumentation(XmlNode node)
        {
            base.Save2XmlDocumentation(node);

            //
            // Иерархия
            //
            ((DimensionLevelCollection)Levels).HierarchySave2Xml(node);

            //
            // Фиксированные значения
            //
            if (!String.IsNullOrEmpty(xmlFixedRowsData))
            {
                XmlNode xmlData = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Data", null);
                xmlData.InnerXml = xmlFixedRowsData;
                node.AppendChild(xmlData);
            }
        }

        protected override void OnAfterUpdate()
        {
            try
            {
                SchemeClass.Instance.Processor.InvalidateDimension(
                    this,
                    "Krista.FM.Server.Scheme.Classes.Classifier",
                    Krista.FM.Server.ProcessorLibrary.InvalidateReason.ClassifierChanged,
                    OlapName);
            }
            catch (Exception e)
            {
                Trace.TraceError(
                    "При установки признака необходимости расчета для объекта \"{0}\" произошла ошибка: {1}", 
                    this.FullName, e.Message);
            }
        }

        /// <summary>
        /// Коллекция уровней
        /// </summary>
        public IDimensionLevelCollection Levels
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return Instance.levels; }
        }

        /// <summary>
        /// Возвращает список источников по которым сформирован классификатор
        /// </summary>
        /// <returns>Key - ID источника данных; Value - описание источника</returns>
        public virtual Dictionary<int, string> GetDataSourcesNames()
        {
            if (Attributes.ContainsKey(DataAttribute.SourceIDColumnName))
                return ((DataSourcesManager.DataSourceManager)SchemeClass.Instance.DataSourceManager).GetDataSourcesNames(FullDBName, "ID <> -1 and RowType = 0");
            else
                throw new Exception(String.Format("Невозможно получить список источников у объекта в котором нет атрибута {0}.", DataAttribute.SourceIDColumnName));
        }

        /// <summary>
        /// Фопмирует иерархию для построения по ней измерений
        /// </summary>
        /// <returns>количество обработанных записей</returns>
        public virtual int FormCubesHierarchy()
        {
            throw new NotImplementedException("функция FormCubesHierarchy не реализована.");
        }

        public override Dictionary<string, string> GetSQLMetadataDictionary()
        {
            Dictionary<string, string> sqlMetadata = base.GetSQLMetadataDictionary();

            sqlMetadata.Add(SQLMetadataConstants.ParentIDForeignKeyConstraint, ClassifierEntityScriptingEngine.ParentIDForeignKeyConstraintName(FullDBName));

            return sqlMetadata;
        }

        #region Работа с иерархией

        /// <summary>
        /// Инициализация плоской иерархии
        /// </summary>
        protected virtual void InitializeDefaultRegularHierarchy()
        {
            Levels.Clear();

            Levels.HierarchyType = HierarchyType.Regular;

            RegularLevel allLevel = new RegularLevel(Guid.Empty.ToString(), this);
            allLevel.Name = "Все";
            allLevel.LevelType = LevelTypes.All;
            Levels.Add(allLevel.Name, allLevel);

            RegularLevel level = new RegularLevel(Guid.NewGuid().ToString(), this);
            level.Name = this.SemanticCaption;
            level.LevelType = LevelTypes.Regular;
            level.MemberKey = Attributes[DataAttribute.IDColumnName];
            level.MemberName = Attributes[DataAttribute.IDColumnName];
            Levels.Add(level.ObjectKey, level);

            if (Attributes.ContainsKey(DataAttribute.ParentIDColumnName))
                Attributes.Remove(DataAttribute.ParentIDColumnName);
        }

        /// <summary>
        /// Инициализация деревянной иерархии
        /// </summary>
        protected virtual void InitializeDefaultParentChildHierarchy()
        {
            Levels.HierarchyType = HierarchyType.ParentChild;
            Levels.Clear();

            // Уровень All
            RegularLevel allLevel = new RegularLevel(Guid.Empty.ToString(), this);
            allLevel.Name = "Все";
            allLevel.LevelType = LevelTypes.All;
            Levels.Add(allLevel.Name, allLevel);

            if (!Attributes.ContainsKey(DataAttribute.ParentIDColumnName))
                Attributes.Add(DataAttribute.SystemParentID);

            // Деревянный уровень
            ParentChildLevel level = new ParentChildLevel(Guid.NewGuid().ToString(), this);
            level.Name = this.SemanticCaption;
            level.LevelType = LevelTypes.Regular;
            level.MemberKey = Attributes[DataAttribute.IDColumnName];
            level.MemberName = Attributes[DataAttribute.IDColumnName];
            level.ParentKey = Attributes[DataAttribute.ParentIDColumnName];
            level.LevelNamingTemplate = "Уровень 1; Уровень 2; Уровень 3; Уровень *";
            Levels.Add(level.ObjectKey, level);
        }

        /// <summary>
        /// Инициализация иерархии классификатора
        /// </summary>
        internal void InitializeDefaultHierarchy()
        {
            if (Levels.HierarchyType == HierarchyType.Regular)
                InitializeDefaultRegularHierarchy();
            else
                InitializeDefaultParentChildHierarchy();
        }
        
        #endregion Работа с иерархией
    }
}

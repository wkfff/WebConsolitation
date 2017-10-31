using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Xml;

using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.ScriptingEngine;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
	/// <summary>
	/// Сопоставимый классификатор данных
	/// </summary>
    internal class BridgeClassifier : Classifier, IBridgeClassifier
	{
		/// <summary>
		/// Конструктор объекта
		/// </summary>
		/// <param name="key"></param>
		/// <param name="owner"></param>
		/// <param name="semantic"></param>
		/// <param name="name">Имя объекта</param>
        /// <param name="state"></param>
        public BridgeClassifier(string key, ServerSideObject owner, string semantic, string name, ServerSideObjectStates state)
            : base(key, owner, semantic, name, ClassTypes.clsBridgeClassifier, SubClassTypes.Regular, true, state, SchemeClass.ScriptingEngineFactory.ClassifierEntityScriptingEngine)
		{
            tagElementName = "BridgeCls";
        }

        public override string TablePrefix
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return "b"; }
        }

        /// <summary>
        /// Полное имя объекта
        /// </summary>
        public override string FullName
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return TablePrefix + "." + base.FullName; }
        }

	    /// <summary>
        /// Инициализирует коллекцию атрибутов объекта по информации из XML настроек 
        /// </summary>
        /// <param name="doc">Документ с XML настройкой</param>
        /// <param name="atagElementName">наименование тега с настройками объекта</param>
        protected override void InitializeAttributes(XmlDocument doc, string atagElementName)
        {
            this.Attributes.Add(DataAttribute.FixedSourceID);
            base.InitializeAttributes(doc, atagElementName);
        }

        /// <summary>
        /// Определяет делится ли объект по источникам или нет
        /// </summary>
        public override bool IsDivided
        {
            get { return false; }
        }

        /// <summary>
        /// Создание Текущей версии для нового классификатора
        /// </summary>
        private void CreateDefaultVersion()
        {
            IDataVersion version = SchemeClass.Instance.DataVersionsManager.DataVersions.Create();
            version.Name = String.Format("{0}.{1}", FullCaption, DataSourcesManager.DataSourceManager.DefaultCurrentVersionName);
            version.PresentationKey = Guid.Empty.ToString();
            version.ObjectKey = ObjectKey;
            version.SourceID = 0;
            version.IsCurrent = true;
            SchemeClass.Instance.DataVersionsManager.DataVersions.Add(version);
        }

        internal override void Create(Modifications.ModificationContext сontext)
        {
            base.Create(сontext);
            UpdateFixedRows(сontext.Database);
            RegisterObject(GetKey(ObjectKey, FullName), FullCaption, SysObjectsTypes.AssociatedClassifier);
            CreateDefaultVersion();
        }

        internal override void Drop(Krista.FM.Server.Scheme.Modifications.ModificationContext context)
        {
            base.Drop(context);
            UnRegisterObject(GetKey(ObjectKey, FullName));
            SchemeClass.Instance.DataVersionsManager.DataVersions.RemoveObject(ObjectKey);
        }

        /*/// <summary>
        /// Сопоставимый классификотор не делиться по источникам
        /// Используем переопределенное свойство при сериализации объекта
        /// </summary>
        public override string DataSourceKinds
        {
            get
            {
                return String.Empty;
            }
            set
            {
                throw new Exception("У данного класса нельзя менять деление по источникам");
            }
        }*/

        /// <summary>
        /// Обновлает фиксированные записи в БД
        /// </summary>
        /// <param name="db"></param>
        private int UpdateFixedRows(IDatabase db)
        {
        	if (null != SchemeClass.Instance.Server.GetConfigurationParameter(SchemeClass.UseNullScriptingEngineImplParamName))
        		return -1;

            try
            {
                if (0 == Convert.ToInt32(db.ExecQuery(String.Format("select count(ID) from {0} where ID = ?", FullDBName),
                    QueryResultTypes.Scalar, db.CreateParameter("ID", -1))))
                {
                    List<string> attributeNames = new List<string>();
                    List<string> attribeteValues = new List<string>();
                    foreach (DataAttribute attr in Attributes.Values)
                    {
                        if (attr.IsNullable || attr.Class == DataAttributeClassTypes.Reference)
                            continue;

                        attributeNames.Add(attr.Name);

                        if (attr.Class == DataAttributeClassTypes.Reference || attr.Name == "ID")
                            attribeteValues.Add("-1");
                        else if (attr.Name == "CodeStr")
                            attribeteValues.Add("'0'");
                        else if (attr.Name == DataAttribute.RowTypeColumnName)
                            attribeteValues.Add("1");
                        else if (!attr.IsNullable)
                            attribeteValues.Add(Convert.ToString(DataAttribute.GetStandardDefaultValue(attr, "Несопоставленные данные")));
                    }

                    string queryText = String.Format("insert into {0} ({1}) values ({2})",
                        this.FullDBName,
                        String.Join(", ", attributeNames.ToArray()),
                        String.Join(", ", attribeteValues.ToArray()));

                    db.ExecQuery(queryText, QueryResultTypes.NonQuery);
                }
                return -1;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw new ServerException(e.Message, e);
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
                return SchemeClass.Instance.UsersManager.CheckPermissionForSystemObject(ObjectKey, (int)AssociatedClassifierOperations.ViewClassifier, false);
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

        public override Dictionary<int, string> GetDataSourcesNames()
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            if (Attributes.ContainsKey(DataAttribute.SourceIDColumnName))
            {
                dictionary = 
                    ((DataSourcesManager.DataSourceManager) SchemeClass.Instance.DataSourceManager).GetDataSourcesNames(
                        FullDBName, "ID <> -1 and RowType = 0");
                int? currentSourceID =
                    SchemeClass.Instance.DataVersionsManager.DataVersions.FindCurrentVersion(ObjectKey);
                if (currentSourceID != null)
                {
                    string dataSourceName =
                        ((DataSourcesManager.DataSourceManager) SchemeClass.Instance.DataSourceManager).
                            GetDataSourceName(Convert.ToInt32(currentSourceID));

                    if (!dictionary.ContainsKey(Convert.ToInt32(currentSourceID)))
                    {
                        dictionary.Add(Convert.ToInt32(currentSourceID), dataSourceName);
                    }
                }

                if (dictionary.ContainsKey(0))
                {
                    dictionary[0] = DataSourcesManager.DataSourceManager.DefaultCurrentVersionName;
                }
                
                return dictionary;
            }
            else
                throw new Exception(String.Format("Невозможно получить список источников у объекта в котором нет атрибута {0}.", DataAttribute.SourceIDColumnName));
        }
    }
}

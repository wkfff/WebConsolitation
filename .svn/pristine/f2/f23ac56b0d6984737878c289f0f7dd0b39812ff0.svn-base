using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;
using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Modifications;

using Krista.FM.ServerLibrary;
using NUnit.Framework;


namespace Krista.FM.Server.Scheme.Classes
{
	/// <summary>
	/// Ассоциация сопоставления между классификатором данных и сопоставимым.
	/// </summary>
    internal class BridgeAssociation : Association, IBridgeAssociation
	{
        internal const string TagElementName = "Data2Bridge";

        /// <summary>
        /// Маппинг полей
        /// </summary>
        internal AssociateMappingCollection mappings;

        /// <summary>
        /// Правила сопоставления
        /// </summary>
        private AssociateRuleCollection associateRules;

	    protected int formSourceID = -1;

        /// <summary>
        /// Ассоциация сопоставления между классификатором данных и сопоставимым
        /// </summary>
        /// <param name="key"></param>
        /// <param name="owner"></param>
        /// <param name="semantic">Семантика</param>
        /// <param name="name">Наименование</param>
        /// <param name="state"></param>
        public BridgeAssociation(string key, ServerSideObject owner, string semantic, string name, ServerSideObjectStates state)
            : base(key, owner, semantic, name, AssociationClassTypes.Bridge, state, SchemeClass.ScriptingEngineFactory.EntityAssociationScriptingEngine)
        {
            tagElementName = BridgeAssociation.TagElementName;
            MandatoryRoleData = true;
            RoleDataAttribute.DefaultValue = -1;
            
            // TODO СУБД зависимый код
            if (SchemeClass.Instance.SchemeDWH.FactoryName == ProviderFactoryConstants.SqlClient)
                onDeleteAction = Krista.FM.Server.Scheme.ScriptingEngine.OnDeleteAction.None;
            else
                onDeleteAction = Krista.FM.Server.Scheme.ScriptingEngine.OnDeleteAction.SetNull;

            mappings = new AssociateMappingCollection(this, state);
            associateRules = new AssociateRuleCollection(this, state);
        }

        #region ServerSideObject

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected new BridgeAssociation Instance
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return (BridgeAssociation)GetInstance(); }
        }

        /// <summary>
        /// Возвращает экземпляр объекта с которым должен работать текущий пользователь, для установки значений свойств
        /// </summary>
        /// <returns>Экземпляр объекта</returns>
        protected new BridgeAssociation SetInstance
        {
            get
            {
                if (SetterMustUseClone())
                    return (BridgeAssociation)CloneObject;
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
            BridgeAssociation clon = (BridgeAssociation)base.Clone();
            clon.mappings = (AssociateMappingCollection)mappings.Clone();
            clon.associateRules = (AssociateRuleCollection)associateRules.Clone();
            return clon;
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
                    mappings.State = value;
                    associateRules.State = value;
                }
            }
        }

        public override void Unlock()
        {
            mappings.Unlock();
            associateRules.Unlock();
            base.Unlock();
        }

        #endregion ServerSideObject


        #region Инициализация

        /// <summary>
        /// Инициализация маппинга формирования классификаторов
        /// </summary>
        protected void InitializeMappings(XmlDocument doc)
        {
            foreach (XmlNode xmlMapping in doc.SelectNodes(String.Format("/DatabaseConfiguration/{0}/Mappings/Mapping", tagElementName)))
            {
                DataAttribute fromAttribute = DataAttributeCollection.GetAttributeByKeyName(RoleA.Attributes, String.Empty, xmlMapping.Attributes["from"].Value);
                DataAttribute toAttribute = DataAttributeCollection.GetAttributeByKeyName(RoleB.Attributes, String.Empty, xmlMapping.Attributes["to"].Value);
                if (fromAttribute ==null || toAttribute ==null)
                {
                    Trace.TraceWarning("В правиле формирования сопосавимого указан неверный аттрибут.");
                }
                else
                {
                    if (mappings.Contains(xmlMapping.Attributes["from"].Value, xmlMapping.Attributes["to"].Value))
                    {
                        throw new FM.Common.ServerException(String.Format(""));
                    }
                    XmlNode keyNode = xmlMapping.Attributes["objectKey"];
                    string key = keyNode != null ? keyNode.Value : Guid.Empty.ToString();

                    if (state == ServerSideObjectStates.New && key == Guid.Empty.ToString())
                        key = Guid.NewGuid().ToString();

                    AssociateMapping mapping = new AssociateMapping(key, this, this.State);
                    mapping.DataAttribute = new MappingValue(mapping, fromAttribute);
                    mapping.BridgeAttribute = new MappingValue(mapping, toAttribute);

                    mappings.Add(mapping);
                }
            }
        }

        /// <summary>
        /// Инициализация маппинга сопоставления
        /// </summary>
        protected void InitializeAssociateMappings(XmlDocument doc)
        {
            AssociateRules.Clear();

            foreach (XmlNode xmlBridgeMappings in doc.SelectNodes(String.Format("/DatabaseConfiguration/{0}/BridgeMappings", tagElementName)))
            {
                if (xmlBridgeMappings.Attributes["associateRuleDefault"] != null)
                {
                    AssociateRules.AssociateRuleDefault = xmlBridgeMappings.Attributes["associateRuleDefault"].Value;
                	defaultAssociateRule = AssociateRules.AssociateRuleDefault;
                }
                break;
            }

            int ruleNo = 0;
            foreach (XmlNode xmlBridgeMapping in doc.SelectNodes(String.Format("/DatabaseConfiguration/{0}/BridgeMappings/BridgeMapping", tagElementName)))
            {
                XmlNode keyNode = xmlBridgeMapping.Attributes["objectKey"];
                string key = keyNode != null ? keyNode.Value : Guid.Empty.ToString();

                if (state == ServerSideObjectStates.New && key == Guid.Empty.ToString())
                    key = Guid.NewGuid().ToString();

                AssociateRule associateRule = new AssociateRule(key, this, this.State);
                associateRule.ReadOnly = false;

                if (xmlBridgeMapping.Attributes["name"] != null)
                    associateRule.Name = xmlBridgeMapping.Attributes["name"].Value;
                else
                    associateRule.Name = "AssociateRule" + ruleNo++;

                if (xmlBridgeMapping.Attributes["conversionTable"] != null)
                    associateRule.UseConversionTable = (xmlBridgeMapping.Attributes["conversionTable"].Value == "true");

                if (xmlBridgeMapping.Attributes["fieldCoincidence"] != null)
                    associateRule.UseFieldCoincidence = (xmlBridgeMapping.Attributes["fieldCoincidence"].Value == "true");

                if (xmlBridgeMapping.Attributes["addNew"] != null)
                    associateRule.AddNewRecords = (xmlBridgeMapping.Attributes["addNew"].Value == "true");

                foreach (XmlNode xmlMapping in xmlBridgeMapping.SelectNodes("Mapping"))
                {
                    keyNode = xmlMapping.Attributes["objectKey"];
                    key = keyNode != null ? keyNode.Value : Guid.Empty.ToString();

                    if (state == ServerSideObjectStates.New && key == Guid.Empty.ToString())
                        key = Guid.NewGuid().ToString();

                    AssociateMapping associateMapping = new AssociateMapping(key, associateRule, associateRule.State);

                    if (xmlMapping.Attributes["relevant"] != null)
                        associateMapping.Relevant = Convert.ToInt32(xmlMapping.Attributes["relevant"].Value);

                    associateMapping.DataAttribute = new MappingValue(associateMapping, xmlMapping.SelectSingleNode("Data"), RoleData);
                    associateMapping.BridgeAttribute = new MappingValue(associateMapping, xmlMapping.SelectSingleNode("Bridge"), RoleBridge);


                    DataAttribute dataAttribute = DataAttributeCollection.GetAttributeByKeyName(RoleA.Attributes, associateMapping.DataAttribute.Name, associateMapping.DataAttribute.Name);
                    if (associateMapping.DataAttribute.IsSample && dataAttribute == null)
                    {
                        Trace.TraceWarning("В правиле сопоставления со стороны классификатора данных не найден атрибут {0}", associateMapping.DataAttribute.Name);
                    }

                    dataAttribute = DataAttributeCollection.GetAttributeByKeyName(RoleB.Attributes, associateMapping.BridgeAttribute.Name, associateMapping.BridgeAttribute.Name);
                    if (associateMapping.BridgeAttribute.IsSample && dataAttribute == null)
                    {
                        Trace.TraceWarning("В правиле сопоставления со стороны сопоставимого классификатора не найден атрибут {0}", associateMapping.BridgeAttribute.Name);
                    }

                    associateRule.Mappings.Add(associateMapping);
                    associateMapping.InitializeMappingValuesTypes();
                }
                associateRule.ReadOnly = true;
                AssociateRules.Add(GetKey(associateRule.ObjectKey, associateRule.Name), associateRule);

                if (String.IsNullOrEmpty(this.AssociateRules.AssociateRuleDefault))
                    this.AssociateRules.AssociateRuleDefault = associateRule.Name;
            }
        }

        /*/// <summary>
        /// Инициализации объекта по XML описанию
        /// </summary>
        internal override XmlDocument Initialize()
        {
            XmlDocument doc = base.Initialize();

            InitializeAssociateMappings(doc);

            return doc;
        }*/

        internal override XmlDocument PostInitialize()
        {
            XmlDocument doc = base.PostInitialize();

            InitializeMappings(doc);
            InitializeAssociateMappings(doc);

            return doc;
        }

        #endregion Инициализация

        internal override void Save2Xml(XmlNode node)
        {
            base.Save2Xml(node);

            //
            // маппинг формирования классификаторов
            //

            if (Instance.mappings.Count > 0)
            {
                XmlNode xmlMappings = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Mappings", null);

                foreach (AssociateMapping item in Instance.mappings)
                {
                    XmlNode xmlMapping = node.OwnerDocument.CreateNode(XmlNodeType.Element, "Mapping", null);

                    if (!String.IsNullOrEmpty(item.ObjectKey) && item.ObjectKey != Guid.Empty.ToString())
                    {
                        XmlHelper.SetAttribute(xmlMapping, "objectKey", item.ObjectKey);
                    }

                    XmlHelper.SetAttribute(xmlMapping, "from", item.DataAttribute.Name);
                    XmlHelper.SetAttribute(xmlMapping, "to", item.BridgeAttribute.Name);

                    xmlMappings.AppendChild(xmlMapping);
                }

                node.AppendChild(xmlMappings);
            }

            //
            // маппинг сопоставления
            //

            if (AssociateRules.Count > 0)
            {
                XmlNode xmlBridgeMappings = node.OwnerDocument.CreateNode(XmlNodeType.Element, "BridgeMappings", null);

                if (!String.IsNullOrEmpty(AssociateRules.AssociateRuleDefault))
                {
                    XmlHelper.SetAttribute(xmlBridgeMappings, "associateRuleDefault",
                                           AssociateRules.AssociateRuleDefault);
                }

                foreach (AssociateRule rule in AssociateRules.Values)
                {
                    XmlNode xmlBridgeMapping = node.OwnerDocument.CreateNode(XmlNodeType.Element, "BridgeMapping", null);

                    rule.Save2Xml(xmlBridgeMapping);

                    xmlBridgeMappings.AppendChild(xmlBridgeMapping);
                }

                node.AppendChild(xmlBridgeMappings);
            }

        }

        public override IModificationItem GetChanges(IModifiable toObject)
        {
            ModificationItem root = (ModificationItem)base.GetChanges(toObject);

            BridgeAssociation toAssociation = (BridgeAssociation)toObject;

            //
            // Правила формирования 
            //

            ModificationItem mappingsModificationItem = (ModificationItem)this.mappings.GetChanges(toAssociation.mappings);
            if (mappingsModificationItem.Items.Count > 0)
            {
                mappingsModificationItem.Parent = root;
                root.Items.Add(mappingsModificationItem.Key, mappingsModificationItem);
            }

            //
            // Правила сопоставления
            //

            ModificationItem associateRulesModificationItem =
                ((AssociateRuleCollection)this.AssociateRules).GetChanges((AssociateRuleCollection)toAssociation.AssociateRules);

            if (Instance.AssociateRules.AssociateRuleDefault != toAssociation.AssociateRules.AssociateRuleDefault)
            {
                ModificationItem ruleDefault = new PropertyModificationItem("AssociateRuleDefault", Instance.AssociateRules.AssociateRuleDefault, toAssociation.AssociateRules.AssociateRuleDefault, associateRulesModificationItem);
                associateRulesModificationItem.Items.Add(ruleDefault.Key, ruleDefault);
            } 

            if (associateRulesModificationItem.Items.Count > 0)
            {
                associateRulesModificationItem.Parent = root;
                root.Items.Add(associateRulesModificationItem.Key, associateRulesModificationItem);
            }

            return root;
        }

        internal override void Create(ModificationContext сontext)
        {
            base.Create(сontext);
        	
			// Сохраняем в базе данных правило сопоставления используемое по умолчанию
			if (AssociateRules.Count > 0)
				SetDefaultAssociateRule(сontext.Database, AssociateRules.AssociateRuleDefault);
            
			RegisterObject(GetKey(ObjectKey, FullName), FullCaption, SysObjectsTypes.Associate);
        }

        internal override void Drop(ModificationContext context)
        {
            base.Drop(context);

            UnRegisterObject(GetKey(ObjectKey, FullName));
        }


        /// <summary>
        /// Применение изменений. Приводит текущий объект к виду объекта toObject
        /// </summary>
        /// <param name="context"></param>
        /// <param name="toObject">Объект к виду которого будет приведен текущий объект</param>
        public override void Update(ModificationContext context, IModifiable toObject)
        {
            base.Update(context, toObject);

            BridgeAssociation toAssociation = (BridgeAssociation)toObject;

            this.Configuration = toAssociation.Configuration;

            /*this.mappings.Clear();
            this.InitializeMappings();

            this.AssociateRules.Clear();
            this.InitializeAssociateMappings();*/
        }

        #region Из класса Association.cs и Association.сопоставление.cs

        #region Сопоставление

        // Таблица перекодировки
        private string FormConversionTableQuery(int conversionTypeID, AssociateRule rule)
        {
            List<string> inputAttributes = new List<string>();
            List<string> outputAttributes = new List<string>();
            List<string> joinInputAttributes = new List<string>();
            List<string> joinOutputAttributes = new List<string>();

            foreach (AssociateMapping item in ((AssociateRule)this.AssociateRules[rule.Name]).Mappings)
            {
                string valueInputName;
                if (this.RoleA.Attributes[item.DataAttribute.Name].Type == DataAttributeTypes.dtString)
                    valueInputName = "ValueStr";
                else if (this.RoleA.Attributes[item.DataAttribute.Name].Type == DataAttributeTypes.dtInteger
                    || this.RoleA.Attributes[item.DataAttribute.Name].Type == DataAttributeTypes.dtDouble)
                    valueInputName = "ValueNum";
                else
                    throw new Exception(String.Format("В таблице перекодировок могут использоваться только числовые или строковые атрибуты. Ассоциация {0} правило {1}", this.Name, rule.Name));

                inputAttributes.Add(String.Format("I_{0}.{1} as Input{0}", item.DataAttribute.Name, valueInputName));
                joinInputAttributes.Add(String.Format("join ConversionInputAttributes I_{0} on (I_{0}.TypeID = {1} and T.ID = I_{0}.ID and I_{0}.Name = '{0}')", item.DataAttribute.Name, conversionTypeID));

                string valueOutputName;
                if (this.RoleB.Attributes[item.BridgeAttribute.Name].Type == DataAttributeTypes.dtString)
                    valueOutputName = "ValueStr";
                else if (this.RoleB.Attributes[item.BridgeAttribute.Name].Type == DataAttributeTypes.dtInteger
                    || this.RoleB.Attributes[item.BridgeAttribute.Name].Type == DataAttributeTypes.dtDouble)
                    valueOutputName = "ValueNum";
                else
                    throw new Exception(String.Format("В таблице перекодировок могут использоваться только числовые или строковые атрибуты. Ассоциация {0} правило {1}", this.Name, rule.Name));

                outputAttributes.Add(String.Format("O_{0}.{1} as Output{0}", item.BridgeAttribute.Name, valueOutputName));
                joinOutputAttributes.Add(String.Format("join ConversionOutAttributes O_{0} on (O_{0}.TypeID = {1} and T.ID = O_{0}.ID and O_{0}.Name = '{0}')", item.BridgeAttribute.Name, conversionTypeID));
            }

            return String.Format("select\n\t{0},\n\t{1}\nfrom ConversionTable T join MetaConversionTable M on (M.ID = T.TypeID)\n\t{2}\n\t{3}\nwhere T.TypeID = {4}",
                String.Join(", ", inputAttributes.ToArray()),       // 0 Исходные значения
                String.Join(", ", outputAttributes.ToArray()),      // 1 Выходные значения
                String.Join("\n", joinInputAttributes.ToArray()),   // 2 join-ы для исходных значений
                String.Join("\n", joinOutputAttributes.ToArray()),  // 3 join-ы для выходных значений
                conversionTypeID                                    // 4 ID типа перекодировки
            );
        }

        // Сопоставимый с перекодировкой
        private string FormBridgeAndConversionTableQuery(int conversionTypeID, AssociateRule rule)
        {
            List<string> inputAttributes = new List<string>();
            List<string> conditions = new List<string>();

            if (rule != null)
                foreach (AssociateMapping item in ((AssociateRule)this.AssociateRules[rule.Name]).Mappings)
                {
                    inputAttributes.Add(String.Format("ConvTable.Input{0}", item.DataAttribute.Name));
                    conditions.Add(String.Format("(Bridge.{0} = ConvTable.Output{0})", item.BridgeAttribute.Name));
                }
            else
                foreach (AssociateMapping item in Instance.mappings)
                {
                    inputAttributes.Add(String.Format("ConvTable.Input{0}", item.DataAttribute.Name));
                    conditions.Add(String.Format("(Bridge.{0} = ConvTable.Output{0})", item.BridgeAttribute.Name));
                }

            return String.Format("select distinct\n\tBridge.ID, {0}\nfrom {1} Bridge join ({2}) ConvTable on ({3})",
                String.Join(", ", inputAttributes.ToArray()),       // 0 Исходные значения
                this.RoleB.FullDBName,                              // 1 Имя сопоставимого
                FormConversionTableQuery(conversionTypeID, rule),   // 2 Таблица перекодировки
                String.Join(" and ", conditions.ToArray())          // 3 Условие соединения сопоставимого и таблицы перекодировки
            );
        }

        /// <summary>
        /// Формирует запрос возвращающий ID сопоставимого с использованием таблицы перекодировок
        /// </summary>
        /// <param name="conversionTypeID">ID таблицы перекодировок</param>
        /// <param name="rule">Правило сопоставления</param>
        /// <returns>SQL-запрос select distinct ID from ...</returns>
        private string FormBridgeQuery(int conversionTypeID, AssociateRule rule)
        {
            List<string> conditions = new List<string>();
            List<string> conditionsNonUnique = new List<string>();

            // Условия соединения классификатора данных с перекодировками сопоставимого
            if (rule != null)
                foreach (AssociateMapping item in ((AssociateRule)this.AssociateRules[rule.Name]).Mappings)
                {
                    conditions.Add(String.Format("(DataCls.{0} = ConvBridge.Input{0})", item.DataAttribute.Name/*, item.BridgeAttributeName*/));
                    conditionsNonUnique.Add(String.Format("(P.Input{0} = ConvBridge.Input{0}) ", item.DataAttribute.Name/*, item.BridgeAttributeName*/));
                }
            else
                foreach (AssociateMapping item in Instance.mappings)
                {
                    conditions.Add(String.Format("(DataCls.{0} = ConvBridge.Input{0})", item.DataAttribute.Name));
                    conditionsNonUnique.Add(String.Format("(P.Input{0} = ConvBridge.Input{0}) ", item.DataAttribute.Name));
                }

            // Условие отсечения дублирующихся записей в классификаторе данных
            string removeNonUnique = String.Format(
                "not exists (select P.ID from ({0}) P where (P.ID <> ConvBridge.ID and {1}))",
                FormBridgeAndConversionTableQuery(conversionTypeID, rule),  // 0 Сопоставимый с перекодировкой
                String.Join(" and ", conditionsNonUnique.ToArray()));       // 1 Условие соединения с внешним запросом

            return String.Format("select distinct ConvBridge.ID from ({0}) ConvBridge where ({1}) and ({2})",
                FormBridgeAndConversionTableQuery(conversionTypeID, rule),  // 0 Сопоставимый с перекодировкой
                String.Join(" and ", conditions.ToArray()),                 // 1 Условие соединения с внешним запросом
                removeNonUnique                                             // 2 Условие отсечения дублирующихся записей
            );
        }

        /// <summary>
        /// Формирует условие равенства полей классификатора данных и сопоставимого
        /// </summary>
        /// <param name="dataKey">Английское наименование поля классификатора данных</param>
        /// <param name="bridgeKey">Английское наименование поля сопоставимого</param>
        /// <returns>SQL-условие</returns>
        private string FormSampleCondition(string dataKey, string bridgeKey)
        {
            if (!RoleA.Attributes.ContainsKey(dataKey))
                throw new Exception(String.Format("В правиле сопоставления указан несуществующий атрибут со стороны классификатора данных {0}.{1}", RoleA.FullName, dataKey));
            if (!RoleB.Attributes.ContainsKey(bridgeKey))
                throw new Exception(String.Format("В правиле сопоставления указан несуществующий атрибут со стороны сопоставимого классификатора {0}.{1}", RoleB.FullName, bridgeKey));
            IDataAttribute dataAttribute = RoleA.Attributes[dataKey];
            IDataAttribute bridgeAttribute = RoleB.Attributes[bridgeKey];
            string dataAttributeText = String.Format("DataCls.{0}", dataAttribute.Name);
            string bridgeAttributeText = String.Format("Bridge.{0}", bridgeAttribute.Name);

            // Если типы атрибутов отличаются, то приводим их к строковому типу
            if (dataAttribute.Type != bridgeAttribute.Type)
            {
                int maxSize;
                if (dataAttribute.Size > bridgeAttribute.Size)
                    maxSize = dataAttribute.Size;
                else
                    maxSize = bridgeAttribute.Size;

                if (dataAttribute.Type != DataAttributeTypes.dtString)
                    dataAttributeText = String.Format("cast({0} as varchar2({1}))", dataAttributeText, maxSize);
                else if (bridgeAttribute.Type != DataAttributeTypes.dtString)
                    bridgeAttributeText = String.Format("cast({0} as varchar2({1}))", bridgeAttributeText, maxSize);
                else
                    throw new Exception("Данный вид сопоставления не реализован.");
            }

            return String.Format("({0} = {1}) ", dataAttributeText, bridgeAttributeText);
        }

        // Запрос возвращающий ID сопоставимого
        private string FormSampleBridgeQuery(int conversionTypeID, AssociateRule rule)
        {
            List<string> conditions = new List<string>();
            List<string> conditionsNonUnique = new List<string>();

            if (rule != null)
                // Используем правило сопоставления
                foreach (AssociateMapping item in ((AssociateRule)this.AssociateRules[rule.Name]).Mappings)
                {
                    conditions.Add(FormSampleCondition(item.DataAttribute.Name, item.BridgeAttribute.Name));
                    conditionsNonUnique.Add(String.Format("(P.{0} = Bridge.{0}) ", item.BridgeAttribute.Name));
                }
            else
                // Используем маппинг
                foreach (AssociateMapping item in Instance.mappings)
                {
                    conditions.Add(FormSampleCondition(item.DataAttribute.Name, item.BridgeAttribute.Name));
                    conditionsNonUnique.Add(String.Format("(P.{0} = Bridge.{0}) ", item.BridgeAttribute.Name));
                }

            if (conditions.Count == 0)
                throw new Exception(String.Format("Для ассоциации {0} не найдено ни одного правила сопоставления или формирования классификатора.", this.FullName));

            // Условие отсечения дублирующихся записей
            string removeNonUnique = String.Format(
                "not exists (select P.ID from {0} P where (P.id <> Bridge.ID and {1}))",
                this.RoleBridge.FullDBName,
                String.Join(" and ", conditionsNonUnique.ToArray()));

            return String.Format(
                "select distinct Bridge.ID from {0} Bridge where ({1}) and (Bridge.ID > 0) and ({2})",
                this.RoleBridge.FullDBName,                 // 0 Сопоставимый 
                String.Join(" and ", conditions.ToArray()), // 1 Условие соединения с внешним запросом
                removeNonUnique);                           // 2 Условие отсечения дублирующихся записей
        }

        /// <summary>
        /// Формирует текст SQL-запроса для сопоставления
        /// </summary>
        /// <param name="isSample">true - без использования таблиц перекодировок</param>
        /// <param name="conversionTypeID">ID таблицы перекодировки</param>
        /// <param name="rule">правило сопоставления</param>
        /// <returns>SQL-запрос</returns>
        private string FormAssociateQuery(bool isSample, int conversionTypeID, AssociateRule rule)
        {
            String bridgeQuery;
            if (isSample)
                bridgeQuery = FormSampleBridgeQuery(conversionTypeID, rule);
            else
                bridgeQuery = FormBridgeQuery(conversionTypeID, rule);

            return String.Format(
                "update {0} DataCls\nset DataCls.{1} = ({2})\nwhere (DataCls.{1} = -1 or DataCls.{1} is null) and exists ({2})",
                this.RoleA.FullDBName,  // 0 Классификатор данных
                this.FullDBName,        // 1 Наименование поля-ссылки
                bridgeQuery);           // 2 Запрос возвращающий ID сопоставимого
        }

        /// <summary>
        /// Простое сопоставление по равенству атрибутов без использования таблицы перекодировок
        /// </summary>
        /// <param name="dataClsSourceID">ID источника по данные которого будет сопоставляться</param>
        /// <returns>Количество обработанных(сопоставленных) записей</returns>
        /// <param name="pumpID"></param>
        protected int SampleAssociate(int dataClsSourceID, int bridgeClsSourceID, int pumpID)
        {
            return Associate2(dataClsSourceID, bridgeClsSourceID, pumpID, null);
        }

        internal IConversionTable ConversionTable
        {
            get { return ConversionTableCollection.Instance[this.FullName]; }
        }


        private string FormConversionTableQuery2(int conversionTypeID, IAssociateRule rule)
        {
            List<string> inputAttributes = new List<string>();
            List<string> outputAttributes = new List<string>();
            List<string> joinInputAttributes = new List<string>();
            List<string> joinOutputAttributes = new List<string>();

            foreach (AssociateMapping item in ((AssociateRule)rule).Mappings)
            {
                // Для слпоставляемого
                string valueInputName;
                if (this.RoleA.Attributes[item.DataAttribute.Name].Type == DataAttributeTypes.dtString)
                    valueInputName = "ValueStr";
                else if (this.RoleA.Attributes[item.DataAttribute.Name].Type == DataAttributeTypes.dtInteger
                    || this.RoleA.Attributes[item.DataAttribute.Name].Type == DataAttributeTypes.dtDouble)
                    valueInputName = "ValueNum";
                else
                    throw new Exception(String.Format("В таблице перекодировок могут использоваться только числовые или строковые атрибуты. Ассоциация {0} правило {1}", this.Name, rule.Name));

                inputAttributes.Add(String.Format("I_{0}.{1} as Input{0}", item.DataAttribute.Name, valueInputName));
                joinInputAttributes.Add(String.Format("join ConversionInputAttributes I_{0} on (I_{0}.TypeID = {1} and T.ID = I_{0}.ID and I_{0}.Name = '{0}')", item.DataAttribute.Name, conversionTypeID));

                // для сопоставимого
                if (item.BridgeAttribute.IsSample)
                {
                    string valueOutputName;
                    if (this.RoleB.Attributes[item.BridgeAttribute.Name].Type == DataAttributeTypes.dtString)
                        valueOutputName = "ValueStr";
                    else if (this.RoleB.Attributes[item.BridgeAttribute.Name].Type == DataAttributeTypes.dtInteger
                        || this.RoleB.Attributes[item.BridgeAttribute.Name].Type == DataAttributeTypes.dtDouble)
                        valueOutputName = "ValueNum";
                    else
                        throw new Exception(String.Format("В таблице перекодировок могут использоваться только числовые или строковые атрибуты. Ассоциация {0} правило {1}", this.Name, rule.Name));

                    outputAttributes.Add(String.Format("O_{0}.{1} as Output{0}", item.BridgeAttribute.Name, valueOutputName));
                    joinOutputAttributes.Add(String.Format("join ConversionOutAttributes O_{0} on (O_{0}.TypeID = {1} and T.ID = O_{0}.ID and O_{0}.Name = '{0}')", item.BridgeAttribute.Name, conversionTypeID));
                }
                else
                {
                    foreach (string attrName in item.BridgeAttribute.SourceAttributes)
                    {
                        string valueOutputName;
                        if (this.RoleB.Attributes[attrName].Type == DataAttributeTypes.dtString)
                            valueOutputName = "ValueStr";
                        else if (this.RoleB.Attributes[attrName].Type == DataAttributeTypes.dtInteger
                            || this.RoleB.Attributes[attrName].Type == DataAttributeTypes.dtDouble)
                            valueOutputName = "ValueNum";
                        else
                            throw new Exception(String.Format("В таблице перекодировок могут использоваться только числовые или строковые атрибуты. Ассоциация {0} правило {1}", this.Name, rule.Name));

                        outputAttributes.Add(String.Format("O_{0}.{1} as Output{0}", attrName, valueOutputName));
                        joinOutputAttributes.Add(String.Format("join ConversionOutAttributes O_{0} on (O_{0}.TypeID = {1} and T.ID = O_{0}.ID and O_{0}.Name = '{0}')", attrName, conversionTypeID));
                    }
                }
            }

            return String.Format("select\n\t{0},\n\t{1}\nfrom ConversionTable T join MetaConversionTable M on (M.ID = T.TypeID)\n\t{2}\n\t{3}\nwhere T.TypeID = {4}",
                String.Join(", ", inputAttributes.ToArray()),       // 0 Исходные значения
                String.Join(", ", outputAttributes.ToArray()),      // 1 Выходные значения
                String.Join("\n", joinInputAttributes.ToArray()),   // 2 join-ы для исходных значений
                String.Join("\n", joinOutputAttributes.ToArray()),  // 3 join-ы для выходных значений
                conversionTypeID                                    // 4 ID типа перекодировки
            );
        }

        private DataTable GetBridgeTable(IDatabase db, IAssociateRule rule)
        {
            List<string> attributeNames = GetAttributeNames(rule);
            
            string query = String.Format("select ID, {0} from {1} where ID > 0",
                String.Join(", ", attributeNames.ToArray()), // 0 Выбираемые атрибуты
                this.RoleB.FullDBName);

            DataTable dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
            return dt;
        }

        private DataTable GetBridgeTable(IDatabase db, IAssociateRule rule, int bridgeClsSourceID)
        {
            List<string> attributeNames = GetAttributeNames(rule);

            string query = String.Format("select ID, {0} from {1} where ID > 0 and sourceID = {2}",
                String.Join(", ", attributeNames.ToArray()), // 0 Выбираемые атрибуты
                this.RoleB.FullDBName,
                bridgeClsSourceID);

            DataTable dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
            return dt;
        }

        private DataTable GetBridgeTable(IDatabase db, int sourceID)
        {
            string query = String.Format(
                "select ID, {0} from {1} where ID > 0 and sourceID = {2}",
                FullDBName,
                this.RoleB.FullDBName,
                sourceID);

            return (DataTable) db.ExecQuery(query, QueryResultTypes.DataTable);
        }

	    protected virtual List<string> GetAttributeNames(IAssociateRule rule)
	    {
	        List<string> attributeNames = new List<string>();

	        foreach (AssociateMapping item in ((AssociateRule)rule).Mappings)
	        {
	            if (item.BridgeAttribute.IsSample)
	                attributeNames.Add(item.BridgeAttribute.Name);
	            else
	            {
	                foreach (string attrName in item.BridgeAttribute.SourceAttributes)
	                    attributeNames.Add(attrName);
	            }
	        }
	        return attributeNames;
	    }

	    /// <summary>
        /// Добавление условия соединения
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="attrName">Наименование атрибута</param>
        private void Add2Conditions(List<string> conditions, string attrName)
        {
            string formatString = "(Bridge.{0} = ConvTable.Output{0})";
            if (this.RoleB.Attributes[attrName].IsNullable)
            {
                formatString = "(" + formatString + " or (Bridge.{0} is null and ConvTable.Output{0} is null))";
            }
            conditions.Add(String.Format(formatString, attrName));
        }

        /// <summary>
        /// Возвращает сопоставимый соединенный с таблицей перекодировок
        /// </summary>
        /// <param name="db"></param>
        /// <param name="rule">Правило сопоставления</param>
        private DataTable GetBridgeConversionTable(IDatabase db, IAssociateRule rule, int bridgeClsSourceID)
        {
            List<string> attributeNames = new List<string>();

            foreach (AssociateMapping item in ((AssociateRule)rule).Mappings)
            {
                //attributeNames.Add(item.BridgeAttribute.Name);
                attributeNames.Add(item.DataAttribute.Name);
            }

            List<string> inputAttributes = new List<string>();
            List<string> conditions = new List<string>();

            foreach (AssociateMapping item in ((AssociateRule)rule).Mappings)
            {
                //inputAttributes.Add(String.Format("ConvTable.Input{0} as {1}", item.DataAttribute.Name, item.BridgeAttribute.Name));
                inputAttributes.Add(String.Format("ConvTable.Input{0} as {1}", item.DataAttribute.Name, item.DataAttribute.Name));

                if (item.BridgeAttribute.IsSample)
                {
                    Add2Conditions(conditions, item.BridgeAttribute.Name);
                }
                else
                {
                    foreach (string attrName in item.BridgeAttribute.SourceAttributes)
                    {
                        Add2Conditions(conditions, attrName);
                    }
                }
            }

            int conversionTypeID = ((ConversionTable)ConversionTableCollection.Instance[this.ObjectKey + "." + rule.ObjectKey]).ID;

            string subQuery = String.Format(
                "select distinct Bridge.ID, {0} from {1} Bridge join ({2}) ConvTable on ({3}) where Bridge.ID > 0 and Bridge.SourceID = {4}",
                String.Join(", ", inputAttributes.ToArray()), // 0 Значения атрибутов сопоставимого соответствующие классификатору данных
                this.RoleB.FullDBName,                                      // 1 Имя сопоставимого
                FormConversionTableQuery2(conversionTypeID, rule),          // 2 Таблица перекодировки
                String.Join(" and ", conditions.ToArray()),                  // 3 Условие соединения сопоставимого и таблицы перекодировки
                bridgeClsSourceID
            );

            string query = String.Format("select /*+ RULE */ ID, {0} from ({1}) CTBridge",
                String.Join(", ", attributeNames.ToArray()), // 0 Выбираемые атрибуты
                subQuery  // 1 
            );

            DataTable dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
            return dt;
        }

        private DataUpdater GetDataClsDataUpdater(IDatabase db, int sourceID, AssociateRule rule)
        {
            string filter = rule.ReAssociate ? String.Empty : String.Format(" and {0} = -1", this.FullDBName);

            if (sourceID != -1)
                return (DataUpdater)RoleA.GetDataUpdater(db, "RowType = 0 and SourceID = ?" + filter, null,
                    db.CreateParameter("SourceID", sourceID));
            else
                return (DataUpdater)RoleA.GetDataUpdater(db, "RowType = 0" + filter, null);
        }

        /// <summary>
        /// Формирует строковое значение аттрибута учавствующего в сопоставлении
        /// </summary>
        /// <returns>Строковое значение аттрибута</returns>
        private string GetMappingValue(MappingValue mappingValue, DataRow dataRow, bool withConversionTable, int size, StringElephanterSettings settings)
        {
            if (withConversionTable)
            {
                return MappingValue.Parameter.GetStringValue(dataRow[mappingValue.Name],
                    mappingValue.Name,
                    dataRow[mappingValue.Name] is decimal ? DataAttributeTypes.dtInteger : DataAttributeTypes.dtString,
                    mappingValue.IsNumericContain, size, settings);
                //return Convert.ToString(dataRow[mappingValue.Name]);
            }
            else
                return mappingValue.GetValue(dataRow, size, settings);
        }

        /// <summary>
        /// Формирует строковое представление записи классификатора
        /// </summary>
        /// <param name="dataRow">Запись классификатора</param>
        /// <param name="rule">Правило сопоставления</param>
        /// <param name="isBridge">Является ли классификатор сопоставимым</param>
        /// <returns>Строковое представление записи</returns>
        protected  virtual string GetGluedString(DataRow dataRow, AssociateRule rule, bool isBridge, bool withConversionTable)
        {
            string glued = String.Empty;
            foreach (AssociateMapping mapping in rule.Mappings)
            {
                MappingValue attr = isBridge ? mapping.BridgeAttribute : mapping.DataAttribute;
                MappingValue attrReverse = isBridge ? mapping.DataAttribute : mapping.BridgeAttribute;

                attr.IsNumericContain =
                    (attr.ValueType == DataAttributeTypes.dtString &&
                    attrReverse.ValueType == DataAttributeTypes.dtInteger) ||
                    (attr.ValueType == DataAttributeTypes.dtInteger &&
                    attrReverse.ValueType == DataAttributeTypes.dtString);

                // При использовании таблицы перекодировок
                // атрибут в сопоставимом заменяется на атрибут из классификатора данных
                if (isBridge && withConversionTable)
                    attr = mapping.DataAttribute;

                // Определяем максимальный размер отрибута 
                int maxNumberSize = Math.Max(
                    mapping.BridgeAttribute.Attribute == null ? 0 : mapping.BridgeAttribute.Attribute.Size,
                    mapping.DataAttribute.Attribute == null ? 0 : mapping.DataAttribute.Attribute.Size);

                string val = GetMappingValue(attr, dataRow, withConversionTable, maxNumberSize, rule.Settings);

                glued += val.ToUpper();
            }
            
            return glued;
        }

        /// <summary>
        /// (Оптимизированный вариант) Сопоставляет два классификатора с использование правила сопоставления
        /// </summary>
        /// <param name="dataClsTable">Сопоставляемый классификатор</param>
        /// <param name="bridgeTable">Сопоставимый классификатор</param>
        /// <param name="rule">Правило сопоставления</param>
        /// <param name="withConversionTable"></param>
        private void AssociateFast(DataTable dataClsTable, DataTable bridgeTable, AssociateRule rule, bool withConversionTable)
        {
            if (bridgeTable.Rows.Count == 0)
                return;

            /*// Дополнительная обработка для расходы.базовый при сопоставлении
            if (IsRBaseBridge() && !withConversionTable)
            {
                bridgeTable.Columns.Add("HierarchyLevel");
                foreach (DataRow row in bridgeTable.Rows)
                {
                    row["HierarchyLevel"] = GetHierarchyLevel(row);
                }
                bridgeTable.AcceptChanges();

                dataClsTable.Columns.Add("HierarchyLevel");
                foreach (DataRow row in dataClsTable.Rows)
                {
                    row["HierarchyLevel"] = GetHierarchyLevel(row);
                }
                dataClsTable.AcceptChanges();
            }*/

            Dictionary<string, int> bridge = new Dictionary<string, int>(bridgeTable.Rows.Count);

            // Формируем значения сопоставимого
            int columnIndex = dataClsTable.Columns[EntityDataAttribute.IDColumnName].Ordinal;
            foreach (DataRow row in bridgeTable.Rows)
            {
                string gluedString = GetGluedString(row, rule, true, withConversionTable);
                if (!bridge.ContainsKey(gluedString))
                    bridge.Add(gluedString, Convert.ToInt32(row[columnIndex]));
            }


            columnIndex = dataClsTable.Columns[this.FullDBName].Ordinal;
            foreach (DataRow row in dataClsTable.Rows)
            {
                #warning Временное решение, нужно только для Новосибирска
                if (RoleData.ObjectKey == "369ba18a-8f2a-499d-a8d3-12df53229d0a" &&
                    Convert.ToInt16(row["Direction"]) != 0 && !withConversionTable)
                {
                    row[columnIndex] = -1;
                    continue;
                }

                // Формируем значения сопоставляемого
                string glued = GetGluedString(row, rule, false, withConversionTable);

                // Ищем соответствие
                int? ID = null;
                try
                {
                    ID = bridge[glued];
                }
                catch
                {
                }

                if (ID != null)
                    row[columnIndex] = ID;
                else if (rule.ReAssociate)
                    row[columnIndex] = -1;
            }
        }

        /// <summary>
        /// Собственно производит сопоставление
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dataClsSourceID"></param>
        /// <param name="rule"></param>
        /// <returns>Количество обработанных(сопоставленных) записей</returns>
        protected int Associate3(IDatabase db, int dataClsSourceID, int bridgeClsSourceID, AssociateRule rule)
        {
            // Получаем классификатор данных
            DataUpdater du = GetDataClsDataUpdater(db, dataClsSourceID, rule);
            DataTable dataClsTable = new DataTable("DataCls");
            du.Fill(ref dataClsTable);

            if (rule.UseFieldCoincidence)
            {
                // Производим сопоставление по совпадению полей.
                AssociateWithFieldCoincidence(dataClsTable, bridgeClsSourceID, db, rule);
            }
            if (rule.UseConversionTable && ConversionTableCollection.Instance.ContainsKey(this, rule.ObjectKey))
            {
                // Производим сопоставление по таблице перекодировки.
                AssociateWithConversionTable(dataClsTable, bridgeClsSourceID, db, rule);
            }
            // Сохраняем данные
            return du.Update(ref dataClsTable);
        }

        private void AssociateWithFieldCoincidence(DataTable dataClsTable, int bridgeClsSourceID, IDatabase db, AssociateRule rule)
        {
            // Получаем сопоставимый классификатор
            DataTable bridgeTable = GetBridgeTable(db, rule, bridgeClsSourceID);
            // Производим сопоставление без перекодировок
            AssociateFast(dataClsTable, bridgeTable, rule, false);
        }

        private void AssociateWithConversionTable(DataTable dataClsTable, int bridgeClsSourceID, IDatabase db, AssociateRule rule)
        {
            // Получаем сопоставимый классификатор
            DataTable bridgeTable = GetBridgeConversionTable(db, rule, bridgeClsSourceID);
            // Производим сопоставление с перекодировками
            AssociateFast(dataClsTable, bridgeTable, rule, true);
        }

        /// <summary>
        /// Возвращает правило сопоставления
        /// </summary>
        /// <param name="associateRule"></param>
        /// <returns></returns>
        private AssociateRule GetAssociateRule(AssociateRule associateRule)
        {
            // По умолчанию берем правило из параметра
            AssociateRule rule = associateRule;
            if (rule == null)
            {
                if (AssociateRules.Count == 0)
                    throw new Exception("Невозможно выполнить операцию сопоставления, т.к. для данной ассоциации не найдено ни одного правила сопоставоения.");

                string defaultAssociateRule = GetDefaultAssociateRule();
                if (!String.IsNullOrEmpty(defaultAssociateRule))
                {
                    foreach (AssociateRule item in associateRules.Values)
                    {
                        if (string.Compare(defaultAssociateRule, item.Name, true) == 0)
                        {
                            rule = item;
                            break;
                        }
                    }
                }

                if (rule == null)
                {
                    if (!string.IsNullOrEmpty(AssociateRules.AssociateRuleDefault))
                        // Если указано правило по умолчанию, берем его.
                    {
                        foreach (AssociateRule item in this.associateRules.Values)
                        {
                            if (item.Name == AssociateRules.AssociateRuleDefault)
                            {
                                rule = item;
                                break;
                            }
                        }
                    }
                    if (rule == null)
                        // Иначе берем первое правило сопоставления.
                    {
                        foreach (AssociateRule item in this.AssociateRules.Values)
                        {
                            rule = item;
                            break;
                        }   
                    }
                }
            }

            // Если настройки для текстовых полей не указаны, то берем настройки по умолчанию
            if (rule.Settings == null)
                rule.Settings = new StringElephanterSettings();

            return rule;
        }

        /// <summary>
        /// Возвращает правило сопоставления по уникальному ключу
        /// </summary>
        /// <param name="associateRuleObjectKey"></param>
        /// <returns></returns>
        private IAssociateRule GetAssociateRule(string associateRuleObjectKey, StringElephanterSettings stringSettings, AssociationRuleParams ruleParams)
        {
            // По умолчанию берем правило из параметра
            IAssociateRule rule = AssociateRules[associateRuleObjectKey].CloneRule();
            rule.ReAssociate = ruleParams.ReAssiciate;
            rule.AddNewRecords = ruleParams.AddNewRecords;
            rule.UseConversionTable = ruleParams.UseConversionTable;
            rule.UseFieldCoincidence = ruleParams.UseFieldCoincidence;

            // Если настройки для текстовых полей не указаны, то берем настройки по умолчанию
            rule.Settings = stringSettings;
            return rule;
        }

        /// <summary>
        /// Сопоставление по равенству атрибутов или с использования таблицы перекодировок
        /// </summary>
        /// <param name="dataClsSourceID">ID источника по данные которого будет сопоставляться</param>
        /// <param name="pumpID"></param>
        /// <param name="associateRule">Правило сопоставления</param>
        /// <returns>Количество обработанных(сопоставленных) записей</returns>
        public int Associate2(int dataClsSourceID, int bridgeClsSourceID, int pumpID, IAssociateRule associateRule)
        {
            int recordsAffected = 0;
            IBridgeOperationsProtocol protocol = (IBridgeOperationsProtocol)SchemeClass.Instance.GetProtocol(Assembly.GetExecutingAssembly().ManifestModule.Name);
            AssociateRule rule = GetAssociateRule((AssociateRule)associateRule);

            IDatabase db = SchemeClass.Instance.SchemeDWH.DB;
            try
            {
                db.BeginTransaction();

                recordsAffected = Associate3(db, dataClsSourceID, bridgeClsSourceID, rule);

                db.Commit();

                if (recordsAffected > 0)
                {
                    SchemeClass.Instance.Processor.InvalidatePartition(
                        RoleB,
                        "Krista.FM.Server.Scheme.Classes.BridgeAssociation",
                        Krista.FM.Server.ProcessorLibrary.InvalidateReason.AssociationChanged,
                        RoleB.OlapName);
                }

                try
                {
                    RefreshRecordsCount();
                    protocol.WriteEventIntoBridgeOperationsProtocol(
                        BridgeOperationsEventKind.boeInformation,
                        this.RoleA.OlapName, this.RoleB.OlapName,
                        string.Format("Всего записей: {0}; сопоставлено записей: {1}; не сопоставлено: {2}; сопоставление по правилу \"{3}\"",
                        GetRecordsCountByCurrentDataSource(dataClsSourceID), GetAssociateRecordsCount(dataClsSourceID), GetUnassociateRecordsCountByCurrentDataSource(dataClsSourceID), rule.Name),
                        pumpID, dataClsSourceID);
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.ToString());
                }

                this.RefreshRecordsCount();

                return recordsAffected;
            }
            catch (Exception e)
            {
                db.Rollback();

                protocol.WriteEventIntoBridgeOperationsProtocol(
                    BridgeOperationsEventKind.boeFinishedWithError,
                    this.RoleA.OlapName, this.RoleB.OlapName,
                    String.Format("Сопоставление по правилу \"{0}\" закончено с ошибками: {1}", rule.Name, e.Message),
                    pumpID, dataClsSourceID);

                Trace.TraceError(e.ToString());

                throw new Exception(e.Message, e);
            }
            finally
            {
                if (db != null)
                    db.Dispose();

                if (protocol != null)
                    protocol.Dispose();
            }
        }

        /// <summary>
        /// Возвращает количество записей, сопоставленных с данной версией сопоставимого
        /// </summary>
        /// <param name="sourceID">Версия сопоставимого</param>
        /// <returns>Количество сопоставленных записей по всем классификаторам</returns>
        public int GetCountBridgeRow(int sourceID)
        {
            if (AssociationClassType != AssociationClassTypes.BridgeBridge)
            {
                return 0;
            }

            IDatabase db = SchemeClass.Instance.SchemeDWH.DB;
            try
            {
                DataTable bridgeClsTable = GetBridgeTable(db, sourceID);

                List<string> bridgeList = new List<string>(bridgeClsTable.Rows.Count);

                bridgeList.AddRange(from DataRow dataRow in bridgeClsTable.Rows select dataRow[0].ToString());

                return RoleA.Associated
                    .Where(keyValuePair => keyValuePair.Key != this.ObjectKey)
                    .Sum(keyValuePair => GetCountBridgeRowForDataCls(
                        db,
                        keyValuePair.Value,
                        bridgeList));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());

                throw new Exception(e.Message, e);
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
        }

	    private static int GetCountBridgeRowForDataCls(IDatabase db, IEntityAssociation bridgeAssociation, List<string> bridgeList)
	    {
            /*// Избегаем ошибки ORA-01795: maximum number of expressions in a list is 1000
	        int size = bridgeList.Count;
	        int toIndex = size > 1000 ? 1000 : size;
            int fromIndex = 0;
            List<string> subList = (List<string>)bridgeList.Skip(fromIndex).Take(toIndex);
	        size -= toIndex;

            string filter = String.Format(" and ({0} in ({1})", bridgeAssociation.FullDBName, String.Join(", ", subList.ToArray()));

            while (size > 0)
            {
                fromIndex = toIndex;
                toIndex = size > 1000 ? 1000 : size;
                subList = (List<string>) bridgeList.Skip(fromIndex).Take(toIndex);
                filter += String.Format(" or {0} in ({1})", bridgeAssociation.FullDBName, String.Join(", ", subList.ToArray()));
                size -= toIndex;
            }

	        filter += ")";*/

            // Выбираем только сопоставленные записи
            string filter = String.Format(" and {0} <> -1", bridgeAssociation.FullDBName);

            // Получаем классификатор данных
            using (DataUpdater du = (DataUpdater)((Entity)bridgeAssociation.RoleData).GetDataUpdater(db, "RowType = 0" + filter, null))
            {
                DataTable dataClsTable = new DataTable("DataCls");
                du.Fill(ref dataClsTable);

                return dataClsTable.Rows.Count;
            }
	    }

        /// <summary>
        /// Перенос ссылок на новую версию классификатора
        /// </summary>
        /// <param name="sourceID">ID источника новой версии</param>
        /// <param name="pumpID"></param>
        /// <returns>Количество перенесенных данных</returns>
	    public int ReplaceLinkToNewVersionCls(int sourceID, int pumpID, int oldSourceID)
        {
            IBridgeOperationsProtocol protocol = (IBridgeOperationsProtocol)SchemeClass.Instance.GetProtocol(Assembly.GetExecutingAssembly().ManifestModule.Name);

            protocol.WriteEventIntoBridgeOperationsProtocol(
                    BridgeOperationsEventKind.boeStart,
                    RoleA.OlapName,
                    RoleB.OlapName,
                    String.Format("Старт процедуры переноса ссылок на версию сопоставимого классификатора {0}.", SchemeClass.Instance.DataSourceManager.GetDataSourceName(sourceID)),
                    pumpID,
                    sourceID);

            IDatabase db = SchemeClass.Instance.SchemeDWH.DB;
            db.BeginTransaction();
            try
            {

                int brSourceID = GetAssociateBridgeSourceID(db, oldSourceID);
                if (brSourceID == -1)
                {
                    throw new Exception(
                        "Операция переноса ссылок возможна только при полном сопоставлении версий классификатора или при отсутствии зависимых данных");
                }

                if (brSourceID != sourceID)
                {
                    throw new Exception(
                        String.Format(
                            "Текущая версия классификатора сопоставлена с {0}. Для переноса ссылок она должна быть сопоставлена с {1}",
                            SchemeClass.Instance.DataSourceManager.GetDataSourceName(brSourceID),
                            SchemeClass.Instance.DataSourceManager.GetDataSourceName(sourceID)));
                }

                int oldBridgeRowsCoont = GetCountBridgeRow(oldSourceID);

                if (AssociationClassType != AssociationClassTypes.BridgeBridge)
                {
                    throw new Exception(
                        String.Format(
                            "Процедура переноса ссылок на новую версию классификатора недопустима для ассоциации с типом {0}",
                            AssociationClassType));
                }

                DataTable bridgeClsTable = GetBridgeTable(db, oldSourceID);

                Dictionary<int, int> bridgeDictionary = new Dictionary<int, int>(bridgeClsTable.Rows.Count);
                foreach (DataRow dataRow in bridgeClsTable.Rows)
                {
                    if (Convert.ToInt32(dataRow[1]) == -1)
                    {
                        // Проверим еще наличие зависимых данных
                        DataSet set = RoleData.GetDependedData(Convert.ToInt32(dataRow[0]), false);
                        if (set.Tables[0].Rows.Count > 0)
                        {
                            throw new Exception(
                                "Операция переноса ссылок возможна только при полном сопоставлении версий классификатора или при отсутствии зависимых данных");
                        }
                    }

                    if (!bridgeDictionary.ContainsKey(Convert.ToInt32(dataRow[0])))
                    {
                        bridgeDictionary.Add(Convert.ToInt32(dataRow[0]), Convert.ToInt32(dataRow[1]));
                    }
                }

                int handleRowCount = RoleA.Associated
                    .Where(entityAssosiation => entityAssosiation.Key != ObjectKey)
                    .Sum(entityAssosiation => ReplaceLinkToNewVersionForDataCls(
                        db,
                        entityAssosiation.Value,
                        bridgeDictionary,
                        protocol,
                        sourceID));

                // Делаем версию классифиатора текущей
                IDataVersion dataVersion =
                    SchemeClass.Instance.DataVersionsManager.DataVersions[RoleB.ObjectKey, sourceID];
                if (dataVersion != null)
                {
                    dataVersion.IsCurrent = true;
                    SchemeClass.Instance.DataVersionsManager.DataVersions.UpdatePresentation(Guid.Empty.ToString(),
                                                                                             sourceID.ToString(),
                                                                                             RoleB.ObjectKey, true);
                }

                // Делаем предыдущую версию не текущей
                // Делаем версию классифиатора текущей
                IDataVersion olddataVersion =
                    SchemeClass.Instance.DataVersionsManager.DataVersions[RoleB.ObjectKey, oldSourceID];
                if (olddataVersion != null)
                {
                    olddataVersion.IsCurrent = false;
                    SchemeClass.Instance.DataVersionsManager.DataVersions.UpdatePresentation(Guid.Empty.ToString(),
                                                                                             oldSourceID.ToString(),
                                                                                             RoleB.ObjectKey, false);
                }

                // Очищаем сопоставление старой текущей версии
                ClearAssociationReference(oldSourceID);

                db.Commit();

                protocol.WriteEventIntoBridgeOperationsProtocol(
                    BridgeOperationsEventKind.boeSuccefullFinished,
                    RoleA.OlapName,
                    RoleB.OlapName,
                    String.Format(
                        "Процедура переноса ссылок на версию сопоставимого классификатора {0} закончена успешно. Записей было во всех классификаторах - {1}. Обработано записей во всех классификаторах: {2}.",
                        SchemeClass.Instance.DataSourceManager.GetDataSourceName(sourceID), oldBridgeRowsCoont,
                        handleRowCount),
                    pumpID,
                    sourceID);

                return handleRowCount;
            }
            catch (Exception e)
            {
                db.Rollback();

                protocol.WriteEventIntoBridgeOperationsProtocol(
                    BridgeOperationsEventKind.boeFinishedWithError,
                    RoleA.OlapName,
                    RoleB.OlapName,
                    String.Format("Процедура переноса ссылок на новую версию сопоставимого классификатора закончена с ошибками: {0}", e.Message),
                    pumpID,
                    sourceID);

                Trace.TraceError(e.ToString());

                throw new Exception(e.Message, e);
            }
            finally
            {
                if (db != null)
                {
                    db.Dispose();
                }

                if (protocol != null)
                {
                    protocol.Dispose();
                }
            }
        }

        private int ReplaceLinkToNewVersionForDataCls(IDatabase db, IEntityAssociation bridgeAssociation, Dictionary<int, int> bridgeDictionary, IBridgeOperationsProtocol protocol, int sourceID)
	    {
            // Выбираем только сопоставленные записи
            string filter = String.Format(" and {0} <> -1", bridgeAssociation.FullDBName);

            // Получаем классификатор данных
	        using (DataUpdater du = (DataUpdater) ((Entity) bridgeAssociation.RoleData).GetDataUpdater(db, "RowType = 0" + filter, null))
	        {
	            DataTable dataClsTable = new DataTable("DataCls");
	            du.Fill(ref dataClsTable);

	            // Для всех выбранных записей классификатора данных переносим ссылку на новую версию сопоставимого
	            foreach (DataRow dataRow in dataClsTable.Rows)
	            {
	                int refBridge = Convert.ToInt32(dataRow[bridgeAssociation.FullDBName]);
                    if (!bridgeDictionary.ContainsKey(refBridge))
                    {
                        throw new Exception(
                            String.Format("Версия {0} классификатора {1} сопоставлена не с текущей версией.",
                                          SchemeClass.Instance.DataSourceManager.GetDataSourceName(
                                              Convert.ToInt32(dataRow[DataAttribute.SourceIDColumnName])),
                                          bridgeAssociation.RoleData.FullCaption));
                    }

                    dataRow[bridgeAssociation.FullDBName] = bridgeDictionary[refBridge];
	            }

	            // Сохраняем данные
	            int updateRowCount = du.Update(ref dataClsTable);

                protocol.WriteEventIntoBridgeOperationsProtocol(
                        BridgeOperationsEventKind.boeInformation,
                        bridgeAssociation.RoleData.OlapName,
                        bridgeAssociation.RoleBridge.OlapName,
                        string.Format("Обработка классификатора {0}. Найдено записей всего - {1}. Обработано записей - {2}", bridgeAssociation.RoleData.FullCaption, dataClsTable.Rows.Count, updateRowCount),
                        0, sourceID);

	            return updateRowCount;
	        }
	    }

	    /// <summary>
        /// Возвращает true, если есть праволо формирования сопставимого
        /// </summary>
        public bool MappingRuleExist
        {
            get
            {
                return Instance.mappings.Count != 0;
            }
        }

        /// <summary>
        /// Привила формирования
        /// </summary>
        public IAssociateMappingCollection Mappings
        {
            get { return Instance.mappings; }
        }

        #endregion Сопоставление


        #endregion 

        /// <summary>
        /// Формирует запрос для вставки записи в сопоставимый классификатор,
        /// использую правила формирования сопоставимого
        /// </summary>
        /// <returns>SQL-запрос</returns>
        private string FormInsertQuery()
        {
            // Собираем запрос вставки
            List<string> attrNames = new List<string>();
            attrNames.Add(DataAttribute.IDColumnName);
            attrNames.Add(DataAttribute.SourceIDColumnName);

            foreach (AssociateMapping item in mappings)
                attrNames.Add(item.BridgeAttribute.Name);

            foreach (KeyValuePair<string, IDataAttribute> kvp in NotNullAttrubutes)
            {
                attrNames.Add(kvp.Key);
            }

            List<string> attrValues = new List<string>();
            for (int i = 0; i < attrNames.Count; attrValues.Add("?"), i++);

            string queryText = String.Format("insert into {0} ({1}) values ({2})",
                RoleBridge.FullDBName, 
                String.Join(", ", attrNames.ToArray()), 
                String.Join(", ", attrValues.ToArray()));
            
            return queryText;
        }

        private Dictionary<string, IDataAttribute> notNullAttributes;
        /// <summary>
        /// возвращает атрибуты сопоставимого классификатора, которые описывают поля,
        ///  обязательные для заполнения, но не указанные в правиле формирования 
        /// </summary>
        private Dictionary<string, IDataAttribute> NotNullAttrubutes
        {
            get
            {
                if (notNullAttributes != null)
                    return notNullAttributes;
                notNullAttributes = new Dictionary<string, IDataAttribute>();
                foreach (IDataAttribute attr in RoleBridge.Attributes.Values)
                {
                    if (!attr.IsNullable && attr.Kind == DataAttributeKindTypes.Regular)
                    {
                        notNullAttributes.Add(attr.Name, attr);
                    }
                }
                foreach (AssociateMapping item in mappings)
                {
                    if (notNullAttributes.ContainsKey(item.BridgeAttribute.Attribute.Name))
                        notNullAttributes.Remove(item.BridgeAttribute.Attribute.Name);
                }
                return notNullAttributes;
            }
        }

        /// <summary>
        /// Вставляет строку в сопоставимый классификатор 
        /// и устанавливает сопоставление со строкой классификатора данных.
        /// </summary>
        /// <param name="queryText">SQL-запрос на вставку</param>
        /// <param name="DB">Объект Database к контексте которого будут происводится операции с БД</param>
        /// <param name="dataSourceID">ID источника данных вставляемой записи</param>
        /// <param name="row">Собственно сама строка</param>
        /// <param name="copyID">true - копирует строки в сопоставимый теми же ID из классификатора данных; false - копирует с новыми ID из генератора</param>
        /// <returns>Количество обработанных строк</returns>
        /// <param name="addedRowID"></param>
        private int InsertRow2Bridge(string queryText, Database DB, int bridgeSourceID, DataRow row, bool copyID, ref int addedRowID, int delta)
        {
            // Подготавливаем параметры
            IDbDataParameter[] parameters = DB.GetParameters(mappings.Count + 2 + NotNullAttrubutes.Count);
            parameters[0].SourceColumn = "ID";
            parameters[0].ParameterName = "ID";
            parameters[1].SourceColumn = "SourceID";
            parameters[1].ParameterName = "SourceID";

            int paramNo = 2;
            foreach (AssociateMapping pair in mappings)
            {
                parameters[paramNo].SourceColumn = pair.BridgeAttribute.Name;
                parameters[paramNo++].ParameterName = pair.BridgeAttribute.Name;
            }

            foreach (KeyValuePair<string, IDataAttribute> kvp in NotNullAttrubutes)
            {
                parameters[paramNo].SourceColumn = kvp.Key;
                parameters[paramNo++].ParameterName = kvp.Key;
            }

            // Заполняем папаметры значениями
            //  ID берем из классификатора
            object newID = parameters[0].Value = copyID ? Convert.ToInt32(row["ID"]) + delta : DB.GetGenerator(((Classifier)RoleBridge).GeneratorName);
            
            // Если источник не указан (-1), то вставляем null
            if (bridgeSourceID == -1)
                parameters[1].Value = DBNull.Value;
            else
                parameters[1].Value = bridgeSourceID;

            //  Получаем значения полей классификатора данных
            int i = 2;
            foreach (AssociateMapping item in mappings)
            {
                parameters[i++].Value = row[item.DataAttribute.Name];
            }
            foreach (KeyValuePair<string, IDataAttribute> kvp in NotNullAttrubutes)
            {
                parameters[i++].Value = kvp.Value.DefaultValue;
            }

            // Вставляем строку
            int recodsAffected = Convert.ToInt32(DB.ExecQuery(queryText, QueryResultTypes.NonQuery, parameters));

            addedRowID = Convert.ToInt32(parameters[0].Value);

            /* Устанавливаем соответствие только если версия сопоставимого классификатора является текущей
               или ессли это интерфейс сопоставления версий*/
            int dataClsSourceID = -1;
            if (RoleA is DataSourceDividedClass && ((DataSourceDividedClass)RoleA).IsDivided)
            {
                if (row[DataAttribute.SourceIDColumnName] != null)
                {
                    dataClsSourceID = Convert.ToInt32(row[DataAttribute.SourceIDColumnName]);
                }
            }

            if (NeedUpdateRefBridge(DB, dataClsSourceID, bridgeSourceID))
            {
                Convert.ToInt32(
                    DB.ExecQuery(String.Format("update {0} set {1} = ? where ID = ?", RoleData.FullDBName, FullDBName),
                                 QueryResultTypes.NonQuery,
                                 DB.CreateParameter("RefBridge", parameters[0].Value),
                                 DB.CreateParameter("ID", Convert.ToInt32(row["ID"]))));
            }

            return recodsAffected;
        }

        /// <summary>
        /// Определяет необходимо или нет сопоставлять добавленную запись
        /// </summary>
        /// <param name="dataClsSourceID"> Выбранный источник классификатора данных</param>
        /// <param name="bridgeClsSourceID"> Выбранный источник сопоставимого классификатора</param>
        /// <returns></returns>
	    private bool NeedUpdateRefBridge(IDatabase db, int dataClsSourceID, int bridgeClsSourceID)
	    {
            if (SchemeClass.Instance.DataVersionsManager.DataVersions[RoleBridge.ObjectKey, bridgeClsSourceID] != null &&
                   SchemeClass.Instance.DataVersionsManager.DataVersions[RoleBridge.ObjectKey, bridgeClsSourceID].IsCurrent)
	        {
	            return true;
	        }

            if (this.AssociationClassType == AssociationClassTypes.BridgeBridge)
            {
                int bridgeSource = this.GetAssociateBridgeSourceID(db, dataClsSourceID);
                if (bridgeSource == -1)
                {
                    return true;
                }

                if (bridgeSource == bridgeClsSourceID)
                {
                    return true;
                }
            }

	        return false;
	    }

	    /// <summary>
        /// Выбирает данные из классификатора данных
        /// </summary>
        /// <param name="dataID">ID строки, или ID источника данных в зависимости от параметра byRowID</param>
        /// <param name="byRowID">true - возвращает таблицу с одной строкой</param>
        /// <param name="db"></param>
        /// <returns></returns>
        protected DataTable GetSourceRows(int dataID, bool byRowID, Database db)
        {
            List<string> attrNames = new List<string>();

            foreach (AssociateMapping item in mappings) 
                attrNames.Add(item.DataAttribute.Name);

            if (((Classifier)RoleA).Levels.HierarchyType == HierarchyType.ParentChild
                && ((Classifier)RoleB).Levels.HierarchyType == HierarchyType.ParentChild)
            {
                attrNames.Add(DataAttribute.ParentIDColumnName);
            }

            string filter = String.Empty;
            if (((Classifier)RoleData).Levels.HierarchyType == HierarchyType.ParentChild)
                filter = String.Format(" and ({0} = 0)", DataAttribute.RowTypeColumnName); // фильтруем системные записи

            DataTable dt;
            if (!byRowID)
            {
                if ((RoleA is DataSourceDividedClass && ((DataSourceDividedClass)RoleA).IsDivided)
                    || AssociationClassType == AssociationClassTypes.BridgeBridge)
                {
                    // Выбираем данные по определенному источнику
                    dt = (DataTable)db.ExecQuery(String.Format("select ID, {0}, SourceID from {1} where ID > 0 and SourceID = ? {2}", String.Join(", ", attrNames.ToArray()), RoleData.FullDBName, filter),
                        QueryResultTypes.DataTable, db.CreateParameter("SourceID", dataID));
                }
                else
                    dt = (DataTable)db.ExecQuery(String.Format("select ID, {0} from {1} where ID > 0 {2}", String.Join(", ", attrNames.ToArray()), RoleData.FullDBName, filter),
                        QueryResultTypes.DataTable);
            }
            else
                // Выбираем данные без привязки к источнику
                dt = (DataTable)db.ExecQuery(String.Format("select ID, {0}, SourceID from {1} where ID = ?", String.Join(", ", attrNames.ToArray()), RoleData.FullDBName),
                    QueryResultTypes.DataTable, db.CreateParameter("ID", dataID));
            return dt;
        }

        /// <summary>
        /// Формирует сопоставимый классификатор по классификатору данных
        /// </summary>
        /// <param name="dataSourceID">ID источника по данным которого будет формироваться сопоставимый классификатор</param>
        /// <returns>Количество сформированных записей</returns>
        public virtual int FormBridgeClassifier(int dataSourceID, int bridgeSourceID)
        {
            if (mappings.Count == 0)
                throw new Exception("Правила формирования не определены.");

            // выбираем данные из классификатора данных
            Database db = (Database)SchemeClass.Instance.SchemeDWH.DB;
            // протоколируем 
            IClassifiersProtocol protocol = (IClassifiersProtocol)SchemeClass.Instance.GetProtocol(Assembly.GetExecutingAssembly().ManifestModule.Name);
            try
            {
                //protocol.WriteEventIntoPreviewDataProtocol(ClassifiersEventKind.ceCreateBridge, RoleBridge.OlapName, -1, -1, 
                //    "Начало операции формирования сопоставимого классификатора.");
                
                int recodsAffected = 0;

                db.BeginTransaction();

                if (Convert.ToInt32(db.ExecQuery(String.Format("Select count(*) from {0} where sourceID = {1}", RoleBridge.FullDBName, bridgeSourceID), QueryResultTypes.Scalar)) != 0)
                {
                    // Есть записи в текущей версии сопоставимо по выбранному источнику. Создаем новый источник и версию
                    IDataSource source = SchemeClass.Instance.DataSourceManager.DataSources[dataSourceID];
                    int newDataSourceID;
                    if (source == null)
                    {
                        newDataSourceID = 0;
                    }
                    else
                    {
                        IDataSource newDadaSource = SchemeClass.Instance.DataSourceManager.DataSources.CreateElement();
                        newDadaSource.DataName = "Классификаторы";
                        newDadaSource.Year = source.Year;
                        newDadaSource.Month = source.Month;
                        newDadaSource.Quarter = source.Quarter;
                        newDadaSource.SupplierCode = "ФО";
                        newDadaSource.DataCode = "22";
                        newDadaSource.ParametersType = ParamKindTypes.YearMonthVariant;
                        newDadaSource.Variant = String.Format("{0}_{1}", source.SupplierCode, source.DataName);
                        newDataSourceID =
                            SchemeClass.Instance.DataSourceManager.DataSources.FindDataSource(newDadaSource) ??
                            SchemeClass.Instance.DataSourceManager.DataSources.Add(newDadaSource);
                    }

                    // Создаем новую версию
                    IDataVersion newDataVersion = SchemeClass.Instance.DataVersionsManager.DataVersions.Create();
                    newDataVersion.IsCurrent = false;
                    newDataVersion.ObjectKey = RoleB.ObjectKey;
                    newDataVersion.PresentationKey = Guid.Empty.ToString();
                    newDataVersion.SourceID = newDataSourceID;
                    newDataVersion.Name = String.Format("{0}.{1}", RoleB.FullCaption,
                                                        SchemeClass.Instance.DataSourceManager.GetDataSourceName(
                                                            newDataSourceID));
                    SchemeClass.Instance.DataVersionsManager.DataVersions.Add(newDataVersion);

                    bridgeSourceID = formSourceID = newDataSourceID;
                }
                else
                {
                    formSourceID = bridgeSourceID;
                }

                // Получаем все строки по источнику dataSourceID
                DataTable dt = GetSourceRows(dataSourceID, false, db);

                // Удаляем все данные из сопоставимого по указанному источнику
                ((Classifier)RoleBridge).DeleteData("where ID <> ? and SourceID = ?", true, -1, bridgeSourceID);

                int rowId = -1;
                string insertQuery = FormInsertQuery();
                int minID = 0;
                if (dataSourceID >= 0)
                {
                    string minIDQuery = String.Format("select min(ID) from {0} where {1} = ?",
                                                    RoleA.FullDBName, DataAttribute.SourceIDColumnName);
                    minID = Convert.ToInt32(db.ExecQuery(minIDQuery, QueryResultTypes.Scalar,
                                                db.CreateParameter(DataAttribute.SourceIDColumnName,
                                                dataSourceID)));
                }
                else
                {
                    string minIDQuery = String.Format("select min(ID) from {0}",
                                                    RoleA.FullDBName);
                    minID = Convert.ToInt32(db.ExecQuery(minIDQuery, QueryResultTypes.Scalar));
                }
                int nextID = db.GetGenerator(RoleB.GeneratorName);
                int delta = nextID - minID;
                foreach (DataRow row in dt.Rows)
                {
                    row[DataAttribute.IDColumnName] = Convert.ToInt32(row[DataAttribute.IDColumnName]);
                    recodsAffected += InsertRow2Bridge(insertQuery, db, bridgeSourceID, row, true, ref rowId, delta);
                }

                // Копируем иерархию);
                if (((Classifier)RoleA).Levels.HierarchyType == HierarchyType.ParentChild
                    && ((Classifier)RoleB).Levels.HierarchyType == HierarchyType.ParentChild)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        int ID = Convert.ToInt32(row[DataAttribute.IDColumnName]);
                        if (!(row[DataAttribute.ParentIDColumnName] is DBNull))
                        {
                            object ParentID = Convert.ToInt32(row[DataAttribute.ParentIDColumnName]);
                            DataRow[] rows = dt.Select(String.Format("ID = {0}", ParentID));
                            if (rows.Length == 0 || String.IsNullOrEmpty(ParentID.ToString()))
                                ParentID = DBNull.Value;
                            else
                            {
                                ParentID = Convert.ToInt32(Convert.ToInt32(ParentID) + delta);
                            }

                            string query = String.Format("update {0} set {1} = ? where {2} = ?",
                                                         RoleB.FullDBName,
                                                         DataAttribute.ParentIDColumnName,
                                                         DataAttribute.IDColumnName);

                            int newID = Convert.ToInt32(ID + delta);
                            db.ExecQuery(query, QueryResultTypes.NonQuery,
                                         db.CreateParameter(DataAttribute.ParentIDColumnName, ParentID),
                                         db.CreateParameter(DataAttribute.IDColumnName, newID));
                        }
                    }
                }

                // Устанавливаем значение генератора
                string maxIDQuery = String.Format("select max(ID) from {0} where {1} = ?",
                                                    RoleB.FullDBName, DataAttribute.SourceIDColumnName);
                int maxID = Convert.ToInt32(db.ExecQuery(maxIDQuery, QueryResultTypes.Scalar,
                                                db.CreateParameter(DataAttribute.SourceIDColumnName,
                                                                    bridgeSourceID)));
                db.SetGenerator(RoleB.GeneratorName, maxID + 1);
                
                db.Commit();

                protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceInformation, RoleBridge.OlapName, -1, -1, (int)RoleBridge.ClassType,
                    string.Format("Формирование сопоставимого классификатора. Обработано записей: {0}", recodsAffected));
                //protocol.WriteEventIntoPreviewDataProtocol(ClassifiersEventKind.ceSuccefullFinished, RoleBridge.OlapName, -1, -1,
                //    "Формирование сопоставимого классификатора успешно завершено.");
                protocol.Dispose();
                return recodsAffected;
            }
            catch (Exception e)
            {
                db.Rollback();
                protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceError, RoleBridge.OlapName, -1, -1, (int)RoleBridge.ClassType,
                    string.Format("Формирование сопоставимого классификатора завершено с ошибкой {0}", e.Message));
                throw new Exception(e.Message, e);
            }
            finally
            {
                db.Dispose();
                protocol.Dispose();
            }
        }

        /// <summary>
        /// Копирует запись из классификатора данных в сопоставимый и устанавливает соответствие сопоставления
        /// </summary>
        /// <param name="rowID">ID записи в классификаторе данных</param>
        public virtual int CopyAndAssociateRow(int rowID, int bridgeSourceID)
        {
            if (mappings.Count == 0)
                throw new Exception("Правила формирования не определены.");

            Database db = (Database)SchemeClass.Instance.SchemeDWH.DB;
            try
            {
                db.BeginTransaction();

                // Получаем строку по rowID
                DataTable dt = GetSourceRows(rowID, true, db);
                if (dt.Rows.Count < 1)
                    throw new Exception("Запись не найдена");

                int addedRowID = -1;

                InsertRow2Bridge(FormInsertQuery(), db, bridgeSourceID, dt.Rows[0], false, ref addedRowID, 0);
                
                db.Commit();

                return addedRowID;
            }
            catch (Exception e)
            {
                db.Rollback();
                throw new Exception(e.Message, e);
            }
            finally
            {
                db.Dispose();
            }
        }

	    public int Associate()
        {
            return Associate(-1, -1);
        }

        public int Associate(int dataClsSourceID, int bridgeClsSourceID)
        {
            return Associate2(dataClsSourceID, bridgeClsSourceID, -1, null);
        }
        
        public int Associate(int dataClsSourceId, int bridgeClsSourceID, int pumpID)
        {
            return Associate(dataClsSourceId, bridgeClsSourceID, pumpID, false, false);
        }

        public int Associate(int dataClsSourceId, int bridgeClsSourceID, int pumpID, bool allowDigits, bool reAssociate)
        {
            IAssociateRule associateRule = GetAssociateRule((AssociateRule)null).CloneRule();
            associateRule.ReAssociate = reAssociate;
            associateRule.Settings = new StringElephanterSettings();
            associateRule.Settings.AllowDigits = allowDigits;
            return Associate2(dataClsSourceId, bridgeClsSourceID, pumpID, associateRule);
        }

        /// <summary>
        /// Сопоставление по равенству атрибутов или с использования таблицы перекодировок
        /// </summary>
        /// <param name="dataClsSourceId">ID источника по данные которого будет сопоставляться</param>
        /// <param name="associateRule">Правило сопоставления</param>
        /// <returns>Количество обработанных(сопоставленных) записей</returns>
        public int Associate(int dataClsSourceId, int bridgeClsSourceID, IAssociateRule associateRule)
        {
            return Associate2(dataClsSourceId, bridgeClsSourceID, -1, associateRule);
        }

        /// <summary>
        /// Сопоставление по равенству атрибутов или с использования таблицы перекодировок
        /// </summary>
        /// <param name="dataClsSourceId"></param>
        /// <param name="associateRuleObjectKey"></param>
        /// <param name="stringSettings"></param>
        /// <param name="ruleParams"></param>
        /// <returns></returns>
        public int Associate(int dataClsSourceId, int bridgeClsSourceID, string associateRuleObjectKey, StringElephanterSettings stringSettings, AssociationRuleParams ruleParams)
        {
            IAssociateRule associateRule = GetAssociateRule(associateRuleObjectKey, stringSettings, ruleParams);
            return Associate2(dataClsSourceId, bridgeClsSourceID, -1, associateRule);
        }

        /// <summary>
        /// Очистка сопоставления
        /// </summary>
        /// <param name="sourceID">ID источника, если -1, то очищать по всем источникам</param>
        public void ClearAssociationReference(int sourceID)
        {
            IDatabase db = SchemeClass.Instance.SchemeDWH.DB;
            try
            {
                string query;

                if (sourceID >= 0)
                {
                    query = string.Format("Update {0} set {1} = -1 where SourceID = {2}", RoleData.FullDBName,
                                          this.FullDBName, sourceID);
                }
                else
                {
                    query = string.Format("Update {0} set {1} = -1", RoleData.FullDBName,
                                          this.FullDBName);
                }

                db.ExecQuery(query, QueryResultTypes.NonQuery);
                // если все нормально, тоустановим значения по записям для обновления
                RefreshRecordsCount();
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Правила сопоставления
        /// </summary>
        public IAssociateRuleCollection AssociateRules
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return Instance.associateRules; }
        }

		/// <summary>
		/// Правило сопоставления используемое по умолчанию.
		/// </summary>
		private string defaultAssociateRule;
		
		/// <summary>
		/// Возвращает правило сопоставления используемое по умолчанию.
		/// </summary>
		/// <returns>Имя правила сопоставления.</returns>
		public string GetDefaultAssociateRule()
		{
			if (String.IsNullOrEmpty(defaultAssociateRule))
			{
				using (IDatabase db = SchemeClass.Instance.SchemeDWH.DB)
				{
					defaultAssociateRule = Convert.ToString(db.ExecQuery(
						"select DefaultAssociateRule from MetaLinks where ObjectKey = ?",
						QueryResultTypes.Scalar,
						db.CreateParameter("ObjectKey", ObjectKey)), CultureInfo.InvariantCulture);
					return defaultAssociateRule;
				}
			}
			else
				return defaultAssociateRule;
		}

		/// <summary>
		/// Устанавливает правило сопоставления используемое по умолчанию.
		/// </summary>
		/// <param name="key">Имя правила сопоставления.</param>
		public void SetDefaultAssociateRule(string key)
		{
			Assert.IsFalse(String.IsNullOrEmpty(key));

			using (IDatabase db = SchemeClass.Instance.SchemeDWH.DB)
			{
				SetDefaultAssociateRule(db, key);
			}
		}

		private void SetDefaultAssociateRule(IDatabase db, string key)
		{
			Assert.IsFalse(String.IsNullOrEmpty(key));

			db.ExecQuery(
				"update MetaLinks set DefaultAssociateRule = ? where ObjectKey = ?",
				QueryResultTypes.Scalar,
				db.CreateParameter("DefaultAssociateRule", key),
				db.CreateParameter("ObjectKey", ObjectKey));
			defaultAssociateRule = key;
		}

		#region Статистика сопоставления

        private int allRecordsCount = -1;
        private int allUnassociateRecordsCount = -1;
        private int recordsCountByDataSource = -1;
        private int unassociateRecordsCountByDataSource = -1;

		/// <summary>
		/// Процент сопоставленных данных.
		/// </summary>
		internal decimal? AssociatedRecordsPercent
		{
			get
			{
				if (allRecordsCount > 0)
				{
					return (allRecordsCount - allUnassociateRecordsCount) / (decimal)allRecordsCount * 100;
				}
				else
					return null;
			}
		}

        /// <summary>
        /// устанавливает все количества записей в -1 для обновления этих значений при новом обращении к свойствам
        /// </summary>
        public void RefreshRecordsCount()
        {
            allRecordsCount = -1;
            allUnassociateRecordsCount = -1;
            recordsCountByDataSource = -1;
            unassociateRecordsCountByDataSource = -1;
        }

        /// <summary>
        /// Возвращаяет SourceID версии сопоставимого, с которым сопоставлен версия сопоставляемого, 
        /// -1 - если нет сопоставления
        /// </summary>
        /// <param name="dataClsSourceID"></param>
        /// <returns></returns>
	    public int GetAssociateBridgeSourceID(IDatabase db, int dataClsSourceID)
        {
            bool needDispose = false;
            if (db == null)
            {
                db = SchemeClass.Instance.SchemeDWH.DB;
                needDispose = true;
            }

            try
            {
                DataTable dataTable = (DataTable) db.ExecQuery
                                                      (String.Format(
                                                          "select distinct t2.sourceid from {0} t1 inner join {1} t2 on t1.{2} = t2.id where t1.{2} <> -1 and t1.sourceid = ?",
                                                          RoleA.FullDBName, RoleB.FullDBName, FullDBName),
                                                       QueryResultTypes.DataTable,
                                                       db.CreateParameter("SourceID", dataClsSourceID));
                if (dataTable.Rows.Count == 0)
                {
                    return -1;
                }

                return Convert.ToInt32(dataTable.Rows[0][0]);
            }
            finally
            {
                if(needDispose)
                {
                    db.Dispose();
                }
            }
	    }

	    /// <summary>
        /// Получает количество всех или несопоставленных записей сопоставляемого классификатора или таблицы фактов
        /// по определенному источнику данных
        /// </summary>
		/// <param name="sourceID">Источник данных, если -1, то по всему классификатору</param>
        /// <param name="allRecords">true - все, false - только несопоставленные записи</param>
        /// <returns>возвращаемое количество записей</returns>
        public int GetDataCLSRecordCount(int sourceID, bool allRecords)
        {
            object returnRowsCount = null;
            IDatabase db = SchemeClass.Instance.SchemeDWH.DB;

            try
            {
                // получаем запрос по количеству записей
                string allRecCountSQLQuery = string.Empty;
                //string DataClassifierName = this.RoleData.FullDBName;
                // для таблиц фактов
                if (this.RoleData.ClassType == ClassTypes.clsFactData)
                {
                    // для таблиц фактов ничего не делаем
                    /*
                    if (sourceID <= 0)
                    {
                        allRecCountSQLQuery = string.Format("select count(*) from {0}", this.RoleData.FullDBName);
                        if (!allRecords)
                            allRecCountSQLQuery = String.Concat(allRecCountSQLQuery, string.Format(" where {0} <= 0", this.FullDBName));
                    }
                    else
                        allRecCountSQLQuery = string.Format("select count(*) from {0} where SourceID = {1}", this.RoleData.FullDBName, sourceID);
                    */
                }
                // для классификаторов
                else
                {
                    if (sourceID < 0)
                        allRecCountSQLQuery = string.Format("select count(*) from {0} where RowType = 0", this.RoleData.FullDBName);
                    else
                        allRecCountSQLQuery = string.Format("select count(*) from {0} where RowType = 0 and SourceID = {1}", this.RoleData.FullDBName, sourceID);
                }
                if (!allRecords)
                    allRecCountSQLQuery = String.Concat(allRecCountSQLQuery, string.Format(" and {0} = -1", this.FullDBName));

                returnRowsCount = db.ExecQuery(allRecCountSQLQuery, QueryResultTypes.Scalar);
            }
            finally
            {
                db.Dispose();
            }
            return Convert.ToInt32(returnRowsCount);
        }

        
        /// <summary>
        /// Количество всех записей в сопоставляемом классификаторе
        /// </summary>
        public int GetAllRecordsCount()
        {
            if (allRecordsCount < 0)
                allRecordsCount = GetDataCLSRecordCount(-1, true);

            return allRecordsCount;
        }

        
        /// <summary>
        /// Количество всех несопоставленных записей в сопоставляемом классификаторе
        /// </summary>
        public int GetAllUnassociateRecordsCount()
        {
            if (allUnassociateRecordsCount < 0)
                allUnassociateRecordsCount = GetDataCLSRecordCount(-1, false);

            return allUnassociateRecordsCount;
        }

        
        /// <summary>
        /// Количество записей в сопоставляемом классификаторе по текущему источнику
        /// </summary>
        public int GetRecordsCountByCurrentDataSource(int sourceID)
        {
            if (recordsCountByDataSource < 0)
                recordsCountByDataSource = GetDataCLSRecordCount(sourceID, true);

            return recordsCountByDataSource;
        }

        
        /// <summary>
        /// Количество несопоставленных записей в сопоставляемом классификаторе по текущему источнику
        /// </summary>
        public int GetUnassociateRecordsCountByCurrentDataSource(int sourceID)
        {

            if (unassociateRecordsCountByDataSource < 0)
                unassociateRecordsCountByDataSource = GetDataCLSRecordCount(sourceID, false);

            return unassociateRecordsCountByDataSource;
        }

        /// <summary>
        /// количество сопоставленных данных
        /// </summary>
        /// <param name="sourceID"></param>
        /// <returns></returns>
        private int GetAssociateRecordsCount(int sourceID)
        {
            // можно вычислить простым вычитанием из всех записей несопоставленные записи
            return GetRecordsCountByCurrentDataSource(sourceID) - GetUnassociateRecordsCountByCurrentDataSource(sourceID);
        }

        #endregion Статистика сопоставления


        #region Права

        /// <summary>
        /// Проверяет права на просмотр объекта для текущего пользователя
        /// </summary>
        /// <returns>true - если у пользлвателя есть права на просмотр</returns>
        public override bool CurrentUserCanViewThisObject()
        {
            try
            {
                return SchemeClass.Instance.UsersManager.CheckPermissionForSystemObject(ObjectKey, (int)Krista.FM.ServerLibrary.AssociateOperations.Associate, false); ;
            }
            catch
            {
                return true;
            }
        }

        #endregion Права
       
    }

}

// ******************************************************************
// Модуль содержит реализацию основных объектов программы закачки
// ******************************************************************

using System;
using System.Xml;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Krista.FM.ServerLibrary;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.Common.Xml;

namespace Krista.FM.Server.DataPumps.BudgetLayersDataPump
{
    #region Объекты

    #region Layer
    /// <summary>
    /// Класс для представления одного слоя
    /// </summary>
    public sealed class Layer : ProcessObjectWithSynonym
    {
        // данные слоя
        internal DataTable layerData;

        public Layer(PumpProgram parentProgram)
            : base(parentProgram, String.Empty)
        {
        }

        /// <summary>
        /// Загрузить данные слоя из каталога
        /// </summary>
        /// <param name="dir">Полный путь к каталогу</param>
        internal void LoadFromDir(string dir)
        {
            // если имя слоя до сих пор не задано - ошибка
            if (String.IsNullOrEmpty(this.name))
                throw new Exception(String.Format("Не задано имя слоя"));
            // формируем полный путь к данным слоя
            string layerFileName = dir + "\\" + this.name + ".xml";
            // пытаемся загрузить данные слоя
            ADODBRecordsetReader.LoadRecordset(ref layerData, layerFileName);
        }

        /// <summary>
        /// Освободить ресурсы
        /// </summary>
        internal override void Clear()
        {
            base.Clear();
            layerData.Clear();
            layerData = null;
        }
    }
    #endregion

    #region FMObject
    /// <summary>
    /// Класс для представения одного нашего объекта
    /// </summary>
    public sealed class FMObject : ProcessObjectWithSynonym
    {
        // тип объекта (классификатор или таблица фактов)
        internal FmObjectsTypes objectType;
        // обобщенная ссылка на объект схемы
        internal object obj;
        // Адаптер для манипуляций с данными объекта
        internal IDbDataAdapter adapter;
        // Собственно данные объекта
        internal DataSet objData;
        // ID последней закачанной записи по объекту, используется при проставлении ссылок на объект
        // в связанных таблицах фактов
        internal object pumpedValueForCurrentLayerRow;
        // список правил обработки аттрибутов объекта. Правила применяются к каждой новой записи
        internal ProcessRulesList processRules;
        // источник данных на который закачиваются данны объекта
        internal DataSource dataSource = null;

        
        public FMObject(PumpProgram parentProgram) : base(parentProgram, String.Empty)
        {
            processRules = new ProcessRulesList(this.parentProgram);
        }

        /// <summary>
        /// Освобождение используемых ресурсов
        /// </summary>
        internal override void Clear()
        {
            base.Clear();
            if (obj != null)
            {
                obj = null;
            }
            if (adapter != null)
            {
                adapter = null;
            }
            if (objData != null)
            {
                objData.Clear();
                objData = null;
            }
            processRules.Clear();
        }

        internal override void LoadFromXml(XmlNode node)
        {
            base.LoadFromXml(node);
            this.objectType = XmlConstsToEnumConverter.StringToFmObjectsTypes(
                XmlHelper.GetStringAttrValue(node, XmlConsts.fmObjectTypeAttr, String.Empty));

            XmlNode xn = node.SelectSingleNode(XmlConsts.dataSourceNodeTag);
            if (xn != null)
            {
                dataSource = new DataSource(this.parentProgram);
                dataSource.LoadFromXml(xn);
            }
            XmlNode rulesNode = node.SelectSingleNode(XmlConsts.processRulesTag);
            if (rulesNode != null)
                processRules.LoadFromXml(rulesNode);
        }

        private const string OBJECT_NOT_FIND_TEMPLATE = "{0} '{1}' не найден в коллекции объектов схемы'";

        //internal Dictionary<string, FMObject> referencesMapping = new Dictionary<string, FMObject>();

        /// <summary>
        /// Проверка корректности объекта (объект с таким имененм должен присутствовать в схеме)
        /// </summary>
        /// <returns>текст ошибки (если она была)</returns>
        internal override string Validate()
        {
            obj = null;
            try
            {
                DataPumpModuleBase pumpModule = parentProgram.pumpModule;
                IScheme scheme = pumpModule.Scheme;
                switch (objectType)
                {
                    case FmObjectsTypes.cls:
                    case FmObjectsTypes.fixedCls:
                        if (!scheme.Classifiers.ContainsKey(this.name))
                            return String.Format(OBJECT_NOT_FIND_TEMPLATE, "Классификатор", this.name);
                        else
                            obj = scheme.Classifiers[this.name];
                        break;
                    case FmObjectsTypes.factTable:
                        if (!scheme.FactTables.ContainsKey(this.name))
                            return String.Format(OBJECT_NOT_FIND_TEMPLATE, "Таблица фактов", this.name);
                        else
                            obj = scheme.FactTables[this.name];
                        break;
                }
                return base.Validate();
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        /// <summary>
        /// Запрос закачанных данных объекта
        /// </summary>
        internal void QueryData()
        {
            int sourceIdInt = parentProgram.pumpModule.SourceID;
            if (dataSource != null)
                sourceIdInt = dataSource.id;
            switch (objectType)
            {
                case FmObjectsTypes.fixedCls:
                    // ???
                    //pumpModule.InitClsDataSet(ref adapter, ref objData, (IClassifier)obj);
                    break;
                case FmObjectsTypes.cls:
                    parentProgram.pumpModule.InitClsDataSet(ref adapter, ref objData, (IClassifier)obj, false, string.Empty, sourceIdInt);
                    break;
                case FmObjectsTypes.factTable:
                    parentProgram.pumpModule.InitFactDataSet(ref adapter, ref objData, (IFactTable)obj);
                    break;
            }
        }
    }
    #endregion

    #region DataSource
    /// <summary>
    /// Класс для представления источника данных. В данный момент практически не используется. 
    /// Нужно дорабатывать
    /// </summary>
    public sealed class DataSource : ProcessObject
    {
        internal string supplierCode;
        internal string dataCode;
        internal string kindsOfParams;
        internal string year;
        internal string month;
        internal string quarter;
        internal string variant;
        internal int id;

        public DataSource(PumpProgram parentProgram) : base(parentProgram, String.Empty)
        {
        }

        internal override void LoadFromXml(XmlNode node)
        {
            supplierCode = XmlHelper.GetStringAttrValue(node, XmlConsts.supplierCodeAttr, String.Empty);
            dataCode = XmlHelper.GetStringAttrValue(node, XmlConsts.dataCodeAttr, String.Empty);
            kindsOfParams = XmlHelper.GetStringAttrValue(node, XmlConsts.kindsOfParamsAttr, String.Empty);
            year = XmlHelper.GetStringAttrValue(node, XmlConsts.yearAttr, "0");
            month = XmlHelper.GetStringAttrValue(node, XmlConsts.monthAttr, "0");
            quarter = XmlHelper.GetStringAttrValue(node, XmlConsts.quarterAttr, "0");
            variant = XmlHelper.GetStringAttrValue(node, XmlConsts.variantAttr, String.Empty);

            id = parentProgram.pumpModule.AddDataSource(supplierCode, dataCode, (ParamKindTypes)Convert.ToInt32(kindsOfParams),
                string.Empty, Convert.ToInt32(year), Convert.ToInt32(month), variant, Convert.ToInt32(quarter), string.Empty).ID;
        }

        internal override string Validate()
        {
            return base.Validate();
        }
    }
    #endregion

    #region ProcessRule
    /// <summary>
    /// Класс для представления правила обработки
    /// </summary>
    public sealed class ProcessRule : ProcessObject
    {
        internal AttributeProcessRules processRule;
        internal object dataValue;

        public ProcessRule(PumpProgram parentProgram)
            : base(parentProgram, XmlConsts.attributeNameAttr)
        {
        }

        internal override void LoadFromXml(XmlNode node)
        {
            base.LoadFromXml(node);
            processRule = XmlConstsToEnumConverter.StringToAttributeProcessRules(
                XmlHelper.GetStringAttrValue(node, XmlConsts.attributeProcessRuleTag, String.Empty)
            );
            dataValue = XmlHelper.GetStringAttrValue(node, XmlConsts.dataValueAttr, String.Empty);
        }

        /// <summary>
        /// Скомпилировать правило обработки (поместить в DataValue фактическое значение)
        /// </summary>
        internal void Build()
        {
            DataPumpModuleBase pumpModule = parentProgram.pumpModule;
            switch (processRule)
            {
                case AttributeProcessRules.constant:
                    //  в DataValue и так содержится нужное значение
                    break;
                case AttributeProcessRules.globalConstant:
                    // получить значение глобальной константы
                    IGlobalConst cnst = pumpModule.Scheme.GlobalConstsManager.Consts.ConstByName(dataValue.ToString());
                    dataValue = cnst.Value;
                    //cnst.Dispose(); // надо ли?
                    break;
                case AttributeProcessRules.param:
                    // получить значение параметра закачки
                    string prmValue = pumpModule.GetParamValueByName(pumpModule.ProgramConfig, dataValue.ToString(), String.Empty);
                    // пытаемся привести значение типа параметра к типу атрибута объекта схемы
                    //DataColumn clmn = fmObject.objData.Tables[0].Columns[pr.name];
                    //if (clmn == null)
                    //    throw new Exception(String.Format("Аттрибут '{0}' не найден", this.name));
                    //dataValue = Convert.ChangeType(prmValue, clmn.DataType);
                    dataValue = prmValue;
                    break;
                case AttributeProcessRules.refCls:
                    // ыы, не хватает сцылки на родительсткий объект
                    // придется такие правила обрабатывать на верхних уровнях где такие ссылки есть
                    break;
            }
        }

        /// <summary>
        /// Применить правило обработки к строке данных
        /// </summary>
        /// <param name="row">строка данных</param>
        internal void Applay(DataRow row)
        {
            switch (processRule)
            {
                case AttributeProcessRules.constant:
                case AttributeProcessRules.globalConstant:
                case AttributeProcessRules.param:
                    row[this.name] = dataValue;
                    break;
                case AttributeProcessRules.generatorValue:
                    row[this.name] = parentProgram.pumpModule.GetGeneratorNextValue((string)dataValue);
                    break;
                case AttributeProcessRules.fmObjectRef:
                    row[this.name] = ((FMObject)dataValue).pumpedValueForCurrentLayerRow;
                    break;
            }
        }
    }
    #endregion

    #region Step
    /// <summary>
    /// Класс для представления одного шага закачки
    /// </summary>
    public sealed class PumpStep : ProcessObjectWithSynonym
    {
        internal FieldsGroupsList fieldsGroups;

        public PumpStep(PumpProgram parentProgram)
            : base(parentProgram, String.Empty)
        {
            fieldsGroups = new FieldsGroupsList(this.parentProgram);
        }

        internal override void LoadFromXml(XmlNode node)
        {
            base.LoadFromXml(node);
            this.synonym = XmlHelper.GetStringAttrValue(node, XmlConsts.layerSynonymAttr, String.Empty);

            fieldsGroups.LoadFromXml(node);
        }

        internal override void Clear()
        {
            base.Clear();
            fieldsGroups.Clear();
        }
    }
    #endregion

    #region LayerField
    /// <summary>
    /// Класс джля представления одного поля слоя
    /// </summary>
    public sealed class LayerField : ProcessObject
    {
        internal ProcessTypes processType;
        internal string destinationObjectSynonym;
        internal string destinationAttributeName;
        internal bool destinationAttributeUseInSearch;
        internal ConvertValueModes convertValueMode = ConvertValueModes.none;
        internal string convertValueModeParam1;
        internal string convertValueModeParam2;
        internal string refLayerSynonym;
        internal string refLayerFieldName;

        internal ProcessRulesList processRules;

        public LayerField(PumpProgram parentProgram)
            : base(parentProgram, String.Empty)
        {
            processRules = new ProcessRulesList(this.parentProgram);
        }

        internal override void LoadFromXml(XmlNode node)
        {
            base.LoadFromXml(node);
            this.processType = XmlConstsToEnumConverter.StringToProcessTypes(
                XmlHelper.GetStringAttrValue(node, XmlConsts.processTypeAttr, String.Empty));
            this.destinationObjectSynonym = XmlHelper.GetStringAttrValue(node, XmlConsts.destinationObjectSynonymAttr, String.Empty);
            this.destinationAttributeName = XmlHelper.GetStringAttrValue(node, XmlConsts.destinationAttributeAttr, String.Empty);
            this.destinationAttributeUseInSearch = XmlHelper.GetBoolAttrValue(node, XmlConsts.destinationAttributeUseInSearchAttr, false);
            string convValModeStr = XmlHelper.GetStringAttrValue(node, XmlConsts.convertValueModeAttr, String.Empty);
            if (!String.IsNullOrEmpty(convValModeStr))
            {
                this.convertValueMode = XmlConstsToEnumConverter.StringToConvertValueVodes(convValModeStr);
            }
            refLayerSynonym = XmlHelper.GetStringAttrValue(node, XmlConsts.refLayerSynonymAttr, String.Empty);
            refLayerFieldName = XmlHelper.GetStringAttrValue(node, XmlConsts.refLayerFieldNameAttr, String.Empty);
            convertValueModeParam1 = XmlHelper.GetStringAttrValue(node, XmlConsts.convertValueModeParam1Attr, String.Empty);
            convertValueModeParam2 = XmlHelper.GetStringAttrValue(node, XmlConsts.convertValueModeParam2Attr, String.Empty);

            XmlNode rulesNode = node.SelectSingleNode(XmlConsts.processRulesTag);
            if (rulesNode != null)
                processRules.LoadFromXml(rulesNode);
        }

        internal override void Clear()
        {
            base.Clear();
            processRules.Clear();
        }
    }
    #endregion

    #region FieldsGroup
    /// <summary>
    /// Класс для представления группы полей слоя
    /// </summary>
    public sealed class FieldsGroup : ProcessObject
    {
        internal FieldsGroupsType groupType;

        internal LayerFieldsList layerFields;

        public FieldsGroup(PumpProgram parentProgram)
            : base(parentProgram, String.Empty)
        {
            layerFields = new LayerFieldsList(this.parentProgram);
        }

        internal override void LoadFromXml(XmlNode node)
        {
            groupType = XmlConstsToEnumConverter.StringToFieldsGroupsType(
                XmlHelper.GetStringAttrValue(node, XmlConsts.fieldsGroupTypeAttr, String.Empty)
            );

            layerFields.LoadFromXml(node);
        }

        internal override void Clear()
        {
            base.Clear();
            layerFields.Clear();
        }

        internal override string Validate()
        {
            StringBuilder sb = new StringBuilder();
            string firstDestinationObject = ((LayerField)layerFields[0]).destinationObjectSynonym;
            foreach (LayerField field in layerFields)
            {
                // проверяем правильность задания синонима объекта схемы
                string syn = field.destinationObjectSynonym;
                try
                {
                    FMObject fo = parentProgram.fmObjects[syn] as FMObject;
                }
                catch
                {
                    sb.AppendLine(String.Format("Не найден объект системы с синонимом '{0}'", syn));
                }
                // проверяем принадлежность все полей группы обному объекту схемы
                if (field.destinationObjectSynonym != firstDestinationObject)
                    sb.AppendLine("В рамках одной группы все поля должны принадлежать одному объекту системы");
            }
            return sb.ToString();
        }
    }
    #endregion
    #endregion

}
using System;

namespace Krista.FM.Server.DataPumps.BudgetLayersDataPump
{
    /// <summary>
    /// Возможные типы объектов системы
    /// </summary>
    public enum FmObjectsTypes : int
    {
        /// <summary>
        /// классификатор
        /// </summary>
        cls = 0,
        /// <summary>
        /// фиксированный классификатор
        /// </summary>
        fixedCls = 1,
        /// <summary>
        /// таблица фактов
        /// </summary>
        factTable = 2
    }

    /// <summary>
    /// Правила обработки атрибутов
    /// </summary>
    public enum AttributeProcessRules : int
    {
        /// <summary>
        /// Константное значение
        /// </summary>
        constant = 0,
        /// <summary>
        /// Значение глобальной константы
        /// </summary>
        globalConstant = 1,
        /// <summary>
        /// Значение параметра закачки
        /// </summary>
        param = 2,
        /// <summary>
        /// Ссылка на классификатор
        /// </summary>
        refCls = 3,
        // внутренние, в xml не встречаются
        /// <summary>
        /// Значение генератора
        /// </summary>
        generatorValue = 100,
        /// <summary>
        /// ID последней закачаной записи объекта системы
        /// </summary>
        fmObjectRef = 101
    }

    /// <summary>
    /// Типы групп полей
    /// </summary>
    public enum FieldsGroupsType : int
    {
        /// <summary>
        /// Строка классификатора
        /// </summary>
        clsRow = 0,
        /// <summary>
        /// Строка таблицы фактов
        /// </summary>
        factTableRow = 1
    }

    /// <summary>
    /// Типы интерпретации группы полей
    /// </summary>
    public enum ProcessTypes : int
    {
        /// <summary>
        /// Прямая (непосредственная) закачка
        /// </summary>
        directPump = 0,
        /// <summary>
        /// Ссылка на другой слой
        /// </summary>
        layerRef = 1
    }

    /// <summary>
    /// Режим преобразования значений
    /// </summary>
    public enum ConvertValueModes : int
    {
        /// <summary>
        /// Не установлен
        /// </summary>
        none = 0,
        /// <summary>
        /// Удалить маску
        /// </summary>
        removeMask = 1,
        /// <summary>
        /// Взять часть значения
        /// </summary>
        partValue = 2,
        /// <summary>
        /// добавить к значению - фикс строку
        /// </summary>
        concatValue = 3, 
        /// <summary>
        /// дополнить значение слева 
        /// </summary>
        padLeft = 4
    }

    /// <summary>
    /// Вспомогательный класс для работы с перечислениями
    /// </summary>
    public struct XmlConstsToEnumConverter
    {
        private static object StringToEnum(Type enumType, string val)
        {
            if ((enumType == null) || (val == null))
                throw new Exception("Аргумент не должен быть равен null");

            if (!enumType.IsEnum)
                throw new Exception(String.Format("Тип '{0}' не является перечислением", enumType.FullName));

            string[] names = Enum.GetNames(enumType);
            Array values = Enum.GetValues(enumType);
            for (int ind = 0; ind < names.Length; ind++)
            {
                if (String.Compare(names[ind], val, true) == 0)
                {
                    return values.GetValue(ind);
                }
            }

            throw new Exception(String.Format("Элемент '{0}' в перечислении '{1}' не найден", val, enumType.FullName));
        }

        public static FmObjectsTypes StringToFmObjectsTypes(string val)
        {
            return (FmObjectsTypes)StringToEnum(typeof(FmObjectsTypes), val);
        }

        public static AttributeProcessRules StringToAttributeProcessRules(string val)
        {
            return (AttributeProcessRules)StringToEnum(typeof(AttributeProcessRules), val);
        }

        public static FieldsGroupsType StringToFieldsGroupsType(string val)
        {
            return (FieldsGroupsType)StringToEnum(typeof(FieldsGroupsType), val);
        }

        public static ProcessTypes StringToProcessTypes(string val)
        {
            return (ProcessTypes)StringToEnum(typeof(ProcessTypes), val);
        }

        public static ConvertValueModes StringToConvertValueVodes(string val)
        {
            return (ConvertValueModes)StringToEnum(typeof(ConvertValueModes), val);
        }
    }

    /// <summary>
    /// Назвния тэгов и атрибутов XML
    /// </summary>
    public struct XmlConsts
    {
        #region Перечисления

        #region fmObjectTypes

        //public const string clsEnumElem = "cls";
        //public const string fixedClsEnumElem = "fixedCls";
        //public const string factTableEnumElem = "factTable";

        #endregion

        #region attributeProcessRules

        //public const string constEnumElem = "const";
        //public const string globalConstEnumElem = "globalConst";
        //public const string paramEnumElem = "param";

        #endregion

        #region fieldsGroupsTypes

        //public const string clsRowEnumElem = "clsRow";
        //public const string factTableRowEnumElem = "factTableRow";

        #endregion

        #endregion

        #region Общие тэги и аттрибуты

        public const string nameAttr = "name";
        public const string synonymAttr = "synonym";

        #endregion

        #region Корневой узел

        public const string rootNodeTag = "layerDataPump";
        public const string versionAttr = "version";

        #endregion

        #region Секция настройки слоев

        public const string layersNodeTag = "usedLayers";
        public const string layerNodeTag = "layer";

        #endregion

        #region Источники данных

        public const string defaultDataSourceNodeTag = "defaultDataSource";

        public const string supplierCodeAttr = "supplierCode";
        public const string dataCodeAttr = "dataCode";
        public const string kindsOfParamsAttr = "kindsOfParams";
        public const string yearAttr = "year";
        public const string monthAttr = "month";
        public const string quarterAttr = "quarter";
        public const string variantAttr = "variant";
        public const string dataSourceNodeTag = "dataSource";

        #endregion

        #region Секция настройки наших объектов

        public const string fmObjectsTag = "usedFMObjects";
        public const string fmObjectTag = "fmObject";
        public const string fmObjectTypeAttr = "fmObjectType";

        #endregion

        #region Правила обработки 

        public const string processRulesTag = "processRules";
        public const string processRuleTag = "processRule";
        public const string attributeNameAttr = "attributeName";
        public const string attributeProcessRuleTag = "attributeProcessRule";
        public const string dataValueAttr = "dataValue";

        #endregion

        #region Секция шагов

        public const string stepsNodeTag = "steps";
        public const string stepNodeTag = "step";
        public const string layerSynonymAttr = "layerSynonym";
        public const string fieldsGroupTag = "fieldsGroup";
        public const string fieldsGroupTypeAttr = "fieldsGroupType";
        public const string layerFieldNodeTag = "layerField";
        public const string processTypeAttr = "processType";
        public const string destinationObjectSynonymAttr = "destinationObjectSynonym";
        public const string destinationAttributeAttr = "destinationAttribute";
        public const string destinationAttributeUseInSearchAttr = "destinationAttributeUseInSearch";
        public const string convertValueModeAttr = "convertValueMode";
        public const string convertValueModeParam1Attr = "convertValueModeParam1";
        public const string convertValueModeParam2Attr = "convertValueModeParam2";
        public const string refLayerSynonymAttr = "refLayerSynonym";
        public const string refLayerFieldNameAttr = "refLayerFieldName";
        #endregion

    }
}
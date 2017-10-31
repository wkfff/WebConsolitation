using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Server.XmlExportImporter
{
    public struct XmlConsts
    {
        // корневой элемент
        public const string rootNode = "xmlExport";
        public const string versionAttribute = "version";
        // параметры импорта
        public const string settingsElement = "settings";
  
        public const string useOldIDAttribute = "useOldID";
        public const string deleteDataBeforeImportAttributeError = "deleteDataDataBeforeImport";
        public const string deleteDataBeforeImportAttribute = "deleteDataBeforeImport";
        public const string deleteDevelopData = "deleteDevelopData";
        public const string useInnerUniqueAttributesAttribute = "useInnerUniqueAttributes";
        public const string useSchemeUniqueAttributesAttribute = "useSchemeUniqueAttributes";
        public const string restoreDataSourceAttribute = "restoreDataSource";
        // корневой элелемент для метаданных всех объектов схемы
        public const string metadataRootNode = "metadata";
        // тип объекта (классификатор данных, сопоставимый, перекодировка и т.д.)
        public const string exportObjectTypeAttribute = "objectType";

        public const string conversionRuleAttribute = "conversionRule";

        public const string dataClassifier = "dataClassifier";
        public const string bridgeClassifier = "bridgeClassifier";
        public const string fixedClassifier = "fixedClassifier";
        public const string factTable = "factData";
        public const string conversionTable = "conversionTable";

        public const string attributeElement = "attribute";
        // атрибуты, используемые в метаданных
        public const string tableMetadataNode = "tableMetadata";
        public const string nameAttribute = "name";
        public const string rusNameAttribute = "rusName";
        public const string semanticAttribute = "semantic";
        public const string rusSemanticAttribute = "rusSemantic";
        // метаданные для таблиц перекодировок
        public const string dataRoleNode = "dataRole";
        public const string bridgeRoleNode = "bridgeRole";
        // атрибуты объекта схемы
        public const string uniqueAttributesAttribute = "uniqueAttributes";
        public const string sizeAttribute = "size";
        public const string typeAttribute = "type";
        public const string nullableAttribute = "nullable";
        public const string defaultValueAttribute = "defaultValue";

        // названия и атрибуты, используемые для объявления данных
        public const string dataNode = "data";
        public const string dataTableNode = "dataTable";
        public const string rowCountAttribute = "rowCount";
        public const string rowNode = "row";
        public const string cellElement = "cell";

        // названия узлов и атрибутов для объявления параметров источника данных
        public const string usedDataSourcesNode = "usedDataSources";
        public const string dataSourceElement = "dataSource";
        public const string idAttribute = "id";
        public const string suppplierCodeAttribute = "suppplierCode";
        public const string dataCodeAttribute = "dataCode";
        public const string dataNameAttribute = "dataName";
        public const string kindsOfParamsAttribute = "kindsOfParams";
        public const string yearAttribute = "year";
        public const string monthAttribute = "month";
        public const string variantAttribute = "variant";
        public const string quarterAttribute = "quarter";
        public const string territoryAttribute = "territory";
        public const string dataSourceNameAttribute = "dataSourceNameValue";
    }
}

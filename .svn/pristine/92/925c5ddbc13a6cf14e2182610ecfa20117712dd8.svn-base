using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.XmlExportImporter
{
    public partial class XmlExportImporter : DisposableObject, IXmlExportImporter
    {
        /// <summary>
        /// проверяет, существует ли источник данных из XML
        /// </summary>
        /// <returns>true, если источник существует</returns>
        private bool AddDataSource(XMLDataSource loadedDataSource)
        {
            IDataSourceManager sourceManager = this._scheme.DataSourceManager;

            IDataSource ds = sourceManager.DataSources.CreateElement();
            ds.BudgetName = loadedDataSource.name;
            ds.DataCode = loadedDataSource.dataCode;
            ds.DataName = loadedDataSource.dataName;
            ds.Month = Convert.ToInt32(loadedDataSource.month);
            ds.ParametersType = GetParamsType(loadedDataSource.kindsOfParams);
            ds.Quarter = Convert.ToInt32(loadedDataSource.quarter);
            ds.SupplierCode = loadedDataSource.suppplierCode;
            ds.Territory = loadedDataSource.territory;
            ds.Variant = loadedDataSource.variant;
            ds.Year = Convert.ToInt32(loadedDataSource.year);
            if (!sourceManager.DataSources.Contains(ds))
            {
                // если источника нету, то добавляем его
                importerObjectDataSource = sourceManager.DataSources.Add(ds);
                return true;
            }
            else
            {
                // если есть, то палучаем его ID
                importerObjectDataSource = Convert.ToInt32(sourceManager.DataSources.FindDataSource(ds));
                return false;
            }
        }

        /// <summary>
        /// получение атрибутов перекодировки
        /// </summary>
        /// <param name="associationName"></param>
        /// <param name="ruleName"></param>
        private void GetConversionTableAttributes(string associationName, string ruleName)
        {
            conversionRule = ruleName;

            conversionTable = this._scheme.ConversionTables[String.Format("{0}.{1}", associationName, ruleName)];

            IAssociation association = (IAssociation)this._scheme.Associations[associationName];
            IAssociateRule rule = ((IBridgeAssociation)association).AssociateRules[ruleName];
            dataRoleAttributes = new List<IDataAttribute>();
            bridgeRoleAttributes = new List<IDataAttribute>();

            // получение атрибутов сопоставляемого и сопоставимого

            foreach (IAssociateMapping map in rule.Mappings)
            {
                if (map.DataValue.IsSample)
                    dataRoleAttributes.Add(map.DataValue.Attribute);
                else
                    foreach (string sourceName in map.DataValue.SourceAttributes)
                    {
                        dataRoleAttributes.Add(association.RoleData.Attributes[sourceName]);
                    }
            }

            foreach (IAssociateMapping map in rule.Mappings)
            {
                if (map.BridgeValue.IsSample)
                    bridgeRoleAttributes.Add(map.BridgeValue.Attribute);
                else
                {
                    foreach (string sourceName in map.BridgeValue.SourceAttributes)
                    {
                        bridgeRoleAttributes.Add(association.RoleBridge.Attributes[sourceName]);
                    }
                }
            }

            // получаем семантики и названия классификаторов, которые учавствуют в сопоставлении
            objectName = new List<string>();
            objectName.Add(association.RoleData.Name);
            objectName.Add(association.RoleBridge.Name);
            rusObjectName = new List<string>();
            rusObjectName.Add(association.RoleData.Caption);
            rusObjectName.Add(association.RoleBridge.Caption);

            objectSemantic = new List<string>();
            objectSemantic.Add(association.RoleData.Semantic);
            objectSemantic.Add(association.RoleBridge.Semantic);
            rusObjectSemantic = new List<string>();
            rusObjectSemantic.Add(GetDataObjSemanticRus(association.RoleData));
            rusObjectSemantic.Add(GetDataObjSemanticRus(association.RoleBridge));
        }

        /// <summary>
        /// получение объекта схемы по ключу и получение атрибутов объекта
        /// </summary>
        /// <param name="objectUniqueKey"></param>
        private void GetSchemeObject(string objectUniqueKey)
        {
            // работает только для классификаторов и таблиц перекодировок
            // получаем объект схемы по ключу
            schemeObject = _scheme.GetObjectByKey(objectUniqueKey);
            // пытаемся получить атрибуты объекта
            if (schemeObject is IEntity)
            {
                classifierAttributes = new List<IDataAttribute>();
                foreach (IDataAttribute attr in ((IEntity)schemeObject).Attributes.Values)
                {
                    classifierAttributes.Add(attr);
                }
            }

            // если объект - классификатор, получаем параметры иерархии
            if (schemeObject is IClassifier)
            {
                IClassifier classifier = (IClassifier)schemeObject;
                string levelWithTemplateName = string.Empty;
                switch (classifier.Levels.HierarchyType)
                {
                    case HierarchyType.ParentChild:
                        foreach (IDimensionLevel item in classifier.Levels.Values)
                        {
                            if (item.LevelType != LevelTypes.All)
                            {
                                levelWithTemplateName = item.Name;
                                break;
                            }
                        }
                        if (String.IsNullOrEmpty(levelWithTemplateName))
                            break;
                        refParentColumnName = classifier.Levels[levelWithTemplateName].ParentKey.Name;
                        parentColumnName = classifier.Levels[levelWithTemplateName].MemberKey.Name;
                        break;
                    case HierarchyType.Regular:
                        break;
                }
            }

            if (parentColumnName != string.Empty && refParentColumnName != string.Empty)
                isHierarchy = true;
            else
                isHierarchy = false;
            // наименование объекта
            objectName = new List<string>();
            objectName.Add(schemeObject.Name);
            rusObjectName = new List<string>();
            rusObjectName.Add(schemeObject.Caption);
            // семантика объекта
            objectSemantic = new List<string>();
            objectSemantic.Add(schemeObject.Semantic);
            rusObjectSemantic = new List<string>();
            rusObjectSemantic.Add(GetDataObjSemanticRus(schemeObject));
        }

        private string GetDataObjSemanticRus(ICommonObject cmnObj)
        {
            string semanticText;
            if (_scheme.Semantics.TryGetValue(cmnObj.Semantic, out semanticText))
                return semanticText;
            else
                return cmnObj.Semantic;
        }

        private static string GetBridgeClsCaptionByRefName(IEntity activeDataObject, string refName)
        {
            IAssociationCollection assCol = (IAssociationCollection)(activeDataObject).Associations;
            string fullName = String.Concat(activeDataObject.Name, ".", refName);
            IEntity bridgeCls = null;
            foreach (IAssociation item in assCol.Values)
            {
                if (String.Compare(item.Name, fullName, true) == 0)
                {
                    bridgeCls = item.RoleBridge;
                    break;
                }
            }
            return bridgeCls.Caption;
        }

        private string GetObjectType(ObjectType objecType, bool toObject)
        {
            switch (objecType)
            {
                case ObjectType.Classifier:
                    switch (((IEntity)schemeObject).ClassType)
                    {
                        case ClassTypes.clsBridgeClassifier:
                            if (toObject)
                                return "сопоставимого классификатора";
                            else
                                return "сопоставимом классификаторе";;
                        case ClassTypes.clsDataClassifier:
                            if (toObject)
                                return "классификатора данных";
                            else 
                                return "классификаторе данных";
                    }
                    break;
                case ObjectType.FactTable:
                    if (toObject)
                        return "таблицы фактов";
                    else
                        return "таблице фактов";
                case ObjectType.ConversionTable:
                    if (toObject)
                        return "таблицы перекодировок";
                    else
                        return "таблице перекодировок";;
            }
            return string.Empty;
        }

        private string GetObjectType(ObjectType objecType)
        {
            switch (objecType)
            {
                case ObjectType.Classifier:
                    switch (((IEntity)schemeObject).ClassType)
                    {
                        case ClassTypes.clsBridgeClassifier:
                            return "Сопоставимый классификатор";
                        case ClassTypes.clsDataClassifier:
                            return "Классификатор данных";
                        case ClassTypes.clsFixedClassifier:
                            return "Фиксированый классификатор";
                    }
                break;
                case ObjectType.FactTable:
                    return "Таблица фактов";
                case ObjectType.ConversionTable:
                    return "Таблица перекодировки";
            }
            return string.Empty;
        }

        private string GetObjectType(string xmlObjectType)
        {
            switch (xmlObjectType)
            {
                case XmlConsts.factTable:
                    return "Таблица фактов";
                case XmlConsts.fixedClassifier:
                    return "Фиксированый классификатор";
                case XmlConsts.dataClassifier:
                    return "Классификатор данных";
                case XmlConsts.bridgeClassifier:
                    return "Сопоставимый классификатор";
                case XmlConsts.conversionTable:
                    return "Таблица перекодировки";
            }
            return string.Empty;
        }
        
    }
}

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
    public abstract partial class ExportImporterBase : DisposableObject
    {
        internal string GetDataObjSemanticRus(ICommonObject cmnObj)
        {
            string semanticText;
            if (_scheme.Semantics.TryGetValue(cmnObj.Semantic, out semanticText))
                return semanticText;
            else
                return cmnObj.Semantic;
        }

        internal static string GetBridgeClsCaptionByRefName(IEntity activeDataObject, string refName)
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

        internal static string GetObjectType(ObjectType objecType, ClassTypes classType, bool toObject)
        {
            switch (objecType)
            {
                case ObjectType.Classifier:
                    switch (classType)
                    {
                        case ClassTypes.clsBridgeClassifier:
                            if (toObject)
                                return "сопоставимого классификатора";
                            else
                                return "сопоставимом классификаторе"; ;
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
                        return "таблице перекодировок"; ;
            }
            return string.Empty;
        }

        internal static string GetObjectType(ObjectType objecType, ClassTypes classType)
        {
            switch(objecType)
            {
                case ObjectType.Classifier:
                    switch (classType)
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

        internal static string GetObjectType(string xmlObjectType)
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

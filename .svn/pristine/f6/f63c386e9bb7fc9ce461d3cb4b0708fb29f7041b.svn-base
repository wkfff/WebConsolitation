using System;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Infragistics.Win.UltraWinGrid;

using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls
{
    public abstract partial class BaseClsUI : BaseViewObj, IInplaceClsView
    {

        #region Внутренние вспомогательные функции для работы с объектами схемы
        /// <summary>
        /// Разыменовка семантической принадлежности объекта
        /// </summary>
        /// <param name="cmnObj">объект</param>
        /// <returns>русское название семантической принадлежности</returns>
        public string GetDataObjSemanticRus(IEntity cmnObj)
        {
            return cmnObj.SemanticCaption;
        }

        /// <summary>
        /// Возвращает русское наименование классификатора 
        /// </summary>
        /// <returns></returns>
        public string GetClsRusName()
        {
            if (ActiveDataObj.ClassType == ClassTypes.clsFixedClassifier)
            {
                if (GetDataObjSemanticRus(ActiveDataObj).ToUpper() == "ФИКСИРОВАННЫЙ")
                    return ActiveDataObj.Caption;
                else
                    return GetDataObjSemanticRus(ActiveDataObj) + '.' + ActiveDataObj.Caption;
            }
            else
                return ActiveDataObj.SemanticCaption + '.' + ActiveDataObj.Caption;
        }

        /// <summary>
        /// Получение сопоставимого классификатора по имени из таблицы ассоциаций
        /// </summary>
        /// <param name="refName">имя классификатора</param>
        /// <returns>классификатор</returns>
        private IEntity GetBridgeClsByRefName(string refName)
        {
            return GetBridgeClsByRefName(ActiveDataObj, refName);
        }

        public static IEntity GetBridgeClsByRefName(IEntity activeDataObject, string refName)
        {
            IAssociationCollection assCol = (IAssociationCollection)(activeDataObject).Associations;

            if (assCol.ContainsKey(refName))
            {
                return assCol[refName].RoleBridge;
            }
            return null;

            string fullName = String.Concat(activeDataObject.Name, ".", refName);
            IEntity bridgeCls = null;
            foreach (IEntityAssociation item in assCol.Values)
            {
                if (String.Compare(item.Name, fullName, true) == 0)
                {
                    bridgeCls = item.RoleBridge;
                    break;
                }
            }
            return bridgeCls;
        }

        /// <summary>
        /// Явлется ли ссылочный аттрибут лукапом? (для инициализации грида)
        /// </summary>
        /// <param name="dataObj">Активный объект-поставщик данных</param>
        /// <param name="referenceAttr">Обрабатываемый аттрибут</param>
        /// <returns>true/false</returns>
        public static bool AttrIsLookup(IEntity dataObj, IDataAttribute referenceAttr, ref string lookupObjectName)
        {
            // если аттрибут не ссылка - он и не лукап
            if (referenceAttr.Class != DataAttributeClassTypes.Reference)
                return false;

            // получаем классификатор на который ссылаемся
            IEntity cls = BaseClsUI.GetBridgeClsByRefName(dataObj, referenceAttr.ObjectKey);
            lookupObjectName = cls.ObjectKey;

            // если у классификатора есть хотя бы одно поле для лукапа - исходный аттрибут является лукапом
            foreach (IDataAttribute attr in cls.Attributes.Values)
            {
                if ((attr.LookupType == LookupAttributeTypes.Primary) || (attr.LookupType == LookupAttributeTypes.Secondary))
                    return true;
            }

            // если все аттрибуты перебраны - исходный аттрибут не лукап
            return false;
        }

        public static bool GetLookupParams(IEntity dataObj, IDataAttribute referenceAttr, 
            /*ref string lookupAttributeName,*/ ref string lookupObjectName, ref string lookupObjectDBName, 
            ref Dictionary<string, string> mainFieldsNames, ref Dictionary<string, string> additionalFieldsNames)
        {
            if (referenceAttr.Class != DataAttributeClassTypes.Reference)
                return false;

            //lookupAttributeName = referenceAttr.Name;

            IEntity cls = BaseClsUI.GetBridgeClsByRefName(dataObj, referenceAttr.ObjectKey);
            lookupObjectName = cls.ObjectKey;
            lookupObjectDBName = cls.FullDBName;
            mainFieldsNames = new Dictionary<string, string>();
            additionalFieldsNames = new Dictionary<string, string>();
            foreach (IDataAttribute attr in cls.Attributes.Values)
            {
                switch (attr.LookupType)
                {
                    case LookupAttributeTypes.Primary:
                        mainFieldsNames.Add(attr.Name, attr.Caption);
                        break;
                    case LookupAttributeTypes.Secondary:
                        additionalFieldsNames.Add(attr.Name, attr.Caption);
                        break;
                    case LookupAttributeTypes.None:
                        break;
                }
            }
            return mainFieldsNames.Count + additionalFieldsNames.Count > 0;
        }

        public static IDataAttribute GetAttrByName(IDataAttributeCollection attrs, string attrName)
        {
            IDataAttribute attr = null;
            foreach (IDataAttribute item in attrs.Values)
            {
                string itemName = item.Name;
                if ((string.Compare(itemName, attrName, true) == 0))
                {
                    attr = item;
                    break;
                }
            }
            return attr;
        }

        /// <summary>
        /// получение атрибута по имени
        /// </summary>
        /// <param name="attrName"></param>
        /// <returns></returns>
        private IDataAttribute GetAttrByName(string attrName, IEntity activeObject)
        {
            return GetAttrByName(activeObject.Attributes, attrName);
        }

        private IClassifier GetClassifierByName(string classifierName)
        {
            return GetClassifierByName(Workplace.ActiveScheme, classifierName);
        }

        public static IClassifier GetClassifierByName(IScheme scheme, string classifierName)
        {
            if (scheme.Classifiers.ContainsKey(classifierName))
                return scheme.Classifiers[classifierName] as IClassifier;
            else
                return null;
        }

        public string GetReferenceAttributeRenaming(string attrName, int attrVal)
        {
            string res = "Код не найден";
            // получаем соответсвующий сопоставимый классификатор
            IEntity bridgeCls = GetBridgeClsByRefName(attrName);
            // если не нашли - исключение
            if (bridgeCls == null)
                // не все ссылки лукапы
                return string.Empty;
                //throw new InvalidOperationException("Классификатор не найден. Невозможно произвести разыменовку значения.");

            // формируем список имен столбцов (атрибутов) классификатора
            List<string> attributesList = new List<string>();
            // список названий колонок
            List<string> attributesCaptionsList = new List<string>();
            foreach (IDataAttribute bridgeAttr in bridgeCls.Attributes.Values)
            {
                attributesList.Add(bridgeAttr.Name);
                string baCaption = bridgeAttr.Caption;
                if (!String.IsNullOrEmpty(baCaption))
                    attributesCaptionsList.Add(bridgeAttr.Caption);
                else
                    attributesCaptionsList.Add(bridgeAttr.Name);
            }
            // формируем запрос для команды выборки данных
            string selectText = String.Format("select {0} from {1} where ID = {2}",
                String.Join(", ", attributesList.ToArray()), bridgeCls.FullDBName, attrVal);

            // выполяем выборку данных
            IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB;
            DataTable searchRow;
            try
            {
                searchRow = (DataTable)db.ExecQuery(selectText, QueryResultTypes.DataTable);
            }
            finally
            {
                db.Dispose();
            }

            // если искомая запись найдена
            if ((searchRow != null) && (searchRow.Rows.Count > 0))
            {
                // формируем текст тултипа
                StringBuilder sb = new StringBuilder(255);
                for (int i = 0; i < searchRow.Columns.Count; i++)
                {
                    sb.AppendLine(string.Format("{0} : {1}", attributesCaptionsList[i], searchRow.Rows[0][i]));
                }
                res = sb.ToString();
            }
            return res;
        }


        #endregion
    }

}
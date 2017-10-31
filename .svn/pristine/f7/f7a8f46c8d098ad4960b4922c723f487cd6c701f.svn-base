using System;
using System.Xml;
using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Classes;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme
{
    internal partial class SchemeClass : SMOSerializable, IScheme
    {
        #region Создание временных объектов из XML

        internal static string Enclose2DatabaseConfiguration(string inner)
        {
            return "<?xml version=\"1.0\" encoding=\"windows-1251\"?><DatabaseConfiguration>" + 
                inner + "</DatabaseConfiguration>";
        }

        static public SubClassTypes GetTakeMethodSubClassTypes(XmlAttribute xmlAttribute)
        {
            if (xmlAttribute == null)
                return SubClassTypes.Regular;

            string subClass = xmlAttribute.Value;
            if (subClass == "ИМПОРТ")
                return SubClassTypes.Pump;
            else if (subClass == "ВВОД")
                return SubClassTypes.Input;
            else if (subClass == "ИМПОРТ-ВВОД")
                return SubClassTypes.PumpInput;
            else
                throw new Exception("Неизвестный подкласс(метод получения информации) " + subClass);
        }

        static public string GetTakeMethodString(SubClassTypes subClassType)
        {
            if (subClassType == SubClassTypes.Pump)
                return "ИМПОРТ";
            else if (subClassType == SubClassTypes.Input)
                return "ВВОД";
            else if (subClassType == SubClassTypes.PumpInput)
                return "ИМПОРТ-ВВОД";
            else
                throw new Exception("Неизвестный подкласс(метод получения информации) " + subClassType);
        }

        /// <summary>
        /// Получаем имя по типу класса
        /// </summary>
        /// <param name="type">Тип класса</param>
        /// <returns>Имя класса</returns>
        public static string GetNameByClassType(ClassTypes type)
        {
            switch (type)
            {
                case ClassTypes.clsFixedClassifier: return "FixedCls";
                case ClassTypes.clsBridgeClassifier: return "BridgeCls";
                case ClassTypes.clsDataClassifier: return "DataCls";
                case ClassTypes.clsFactData: return "DataTable";
                case ClassTypes.Table: return "Table";
				case ClassTypes.DocumentEntity: return "DocumentEntity";
				default:
                    throw new Exception("Неизвестный тип объекта");
            }
        }

        /// <summary>
        /// Получаем имя по типу ассоциации
        /// </summary>
        /// <param name="type">Тип ассоциации</param>
        /// <returns>Имя ассоциации</returns>
        public static string GetNameByAssociationClassType(AssociationClassTypes type)
        {
            switch (type)
            {
                case AssociationClassTypes.Link: return Association.TagElementName;
                case AssociationClassTypes.Bridge:
                case AssociationClassTypes.BridgeBridge:
                    return BridgeAssociation.TagElementName;
                case AssociationClassTypes.MasterDetail: return MasterDetailAssociation.TagElementName;
                default:
                    throw new Exception("Неизвестный тип объекта");
            }
        }

        internal static AssociationClassTypes CheckRolesClasses(AssociationClassTypes associationClassType, ClassTypes roleDataClassType, ClassTypes roleBridgeClassType, string objFullName)
        {
            switch (associationClassType)
            {
				// Допустимы следующие ассоциации сопоставления:
				// Классификатор данных -> Сопоставимый
				// Сопоставимый         -> Сопоставимый
				case AssociationClassTypes.Bridge:
                    if (!(roleDataClassType == ClassTypes.clsDataClassifier || roleDataClassType == ClassTypes.clsBridgeClassifier))
                        throw new ServerException("Role Data in association \"" + objFullName + "\" must reference to DataCls or BridgeCls");

                    if (roleBridgeClassType != ClassTypes.clsBridgeClassifier)
						throw new ServerException("Role Bridge in association \"" + objFullName + "\" must reference to BridgeCls");
                    break;

                // Допустимы следующие ассоциации сопоставления:
                // Сопоставимый         -> Сопоставимый (objectKey == objectKey)
                case AssociationClassTypes.BridgeBridge:
                    if (roleDataClassType != ClassTypes.clsBridgeClassifier)
                    {
                        throw new ServerException("Role Data in association \"" + objFullName +
                                                  "\" must reference to BridgeCls");
                    }

                    if (roleBridgeClassType != ClassTypes.clsBridgeClassifier)
                    {
                        throw new ServerException("Role Bridge in association \"" + objFullName +
                                                  "\" must reference to BridgeCls");
                    }
                    break;

				// Допустимы следующие ассоциации:
				// Таблица фактов -> Сопоставимый (автоматически преобрадуется в AssociationClassTypes.Fact2Bridge)
				// Классификатор  -> Фиксированный | Классификатор
				// Сопоставимый   -> Фиксированный | Сопоставимый | Классификатор
				// Таблица данных -> Фиксированный | Сопоставимый | Классификатор | Факты | Таблица данных
				// Табличный документ -> Табличный документ | Фиксированный | Классификатор
				case AssociationClassTypes.Link:

					if (roleDataClassType == ClassTypes.Table && roleBridgeClassType == ClassTypes.DocumentEntity)
						throw new ServerException("Таблица не может ссылаться на табличный документ: " + objFullName);

					if (roleDataClassType == ClassTypes.DocumentEntity)
					{
						if (roleBridgeClassType != ClassTypes.DocumentEntity && roleBridgeClassType != ClassTypes.clsDataClassifier && roleBridgeClassType != ClassTypes.clsFixedClassifier)
						{
							throw new ServerException(
								"Табличный документ должен ссылаться на табличный документ, классификатор данных или фиксированный классификатор: " +
								objFullName);
						}
						return AssociationClassTypes.Link;
					}

            		if (roleDataClassType == ClassTypes.DocumentEntity && roleBridgeClassType == ClassTypes.DocumentEntity)
						return AssociationClassTypes.Link;

            		if (roleDataClassType == ClassTypes.Table && roleBridgeClassType == ClassTypes.Table)
						return AssociationClassTypes.Link;

					if (roleDataClassType == ClassTypes.clsBridgeClassifier &&
						(roleBridgeClassType != ClassTypes.clsFixedClassifier && roleBridgeClassType != ClassTypes.clsBridgeClassifier && roleBridgeClassType != ClassTypes.clsDataClassifier))
						throw new ServerException("Сопоставимый классификатор должен ссылаться на фиксированный классификатор, сопоставимый или классификатор данных: " + objFullName);

					if (roleDataClassType == ClassTypes.clsDataClassifier &&
						(/*roleBridgeClassType == ClassTypes.clsBridgeClassifier ||*/ roleBridgeClassType == ClassTypes.clsFactData || roleBridgeClassType == ClassTypes.DocumentEntity))
						throw new ServerException("Сопоставимый классификатор должен ссылаться на фиксированный классификатор или классификатор данных: " + objFullName);

					if (roleDataClassType != ClassTypes.clsFactData)
                    {
						if (roleDataClassType == ClassTypes.clsDataClassifier ||
							roleDataClassType == ClassTypes.clsBridgeClassifier ||
							roleDataClassType == ClassTypes.Table)
						{
							associationClassType = AssociationClassTypes.Link;
						}
						else
							throw new ServerException("Role Data in association \"" + objFullName + "\" must reference to FactTable");
                    }

                    if (roleBridgeClassType != ClassTypes.clsDataClassifier &&
                        roleBridgeClassType != ClassTypes.clsFixedClassifier &&
                        roleBridgeClassType != ClassTypes.clsBridgeClassifier &&
                        roleBridgeClassType != ClassTypes.clsFactData &&
					    roleBridgeClassType != ClassTypes.DocumentEntity)
							throw new ServerException("Role Bridge in association \"" + objFullName + "\" must reference to DataCls or FixedCls");
                    break;

				// Допустимы следующие ассоциации мастер-деталь:
				// Таблица данных -> Все возможные объекты
				case AssociationClassTypes.MasterDetail:
                    if (roleDataClassType != ClassTypes.Table)
						throw new ServerException("Role Data in association \"" + objFullName + "\" must reference to Table");
					if (roleBridgeClassType == ClassTypes.DocumentEntity)
						throw new ServerException("Role Bridge in association \"" + objFullName + "\" can not reference to DocumentEntity");
					break;
            }
            return associationClassType;
        }

        internal static string ExtractRoleName(XmlNode xmlObject, string roleName)
        {
            string nameRoleData = String.Format("{0}.{1}.{2}",
                xmlObject.SelectSingleNode(roleName + "/@class").Value,
                xmlObject.SelectSingleNode(roleName + "/@semantic").Value,
                xmlObject.SelectSingleNode(roleName + "/@name").Value);
            return nameRoleData;
        }

        #endregion Создание временных объектов из XML
    }
}
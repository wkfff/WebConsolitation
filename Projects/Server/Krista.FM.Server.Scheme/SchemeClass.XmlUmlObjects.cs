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
        #region �������� ��������� �������� �� XML

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
            if (subClass == "������")
                return SubClassTypes.Pump;
            else if (subClass == "����")
                return SubClassTypes.Input;
            else if (subClass == "������-����")
                return SubClassTypes.PumpInput;
            else
                throw new Exception("����������� ��������(����� ��������� ����������) " + subClass);
        }

        static public string GetTakeMethodString(SubClassTypes subClassType)
        {
            if (subClassType == SubClassTypes.Pump)
                return "������";
            else if (subClassType == SubClassTypes.Input)
                return "����";
            else if (subClassType == SubClassTypes.PumpInput)
                return "������-����";
            else
                throw new Exception("����������� ��������(����� ��������� ����������) " + subClassType);
        }

        /// <summary>
        /// �������� ��� �� ���� ������
        /// </summary>
        /// <param name="type">��� ������</param>
        /// <returns>��� ������</returns>
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
                    throw new Exception("����������� ��� �������");
            }
        }

        /// <summary>
        /// �������� ��� �� ���� ����������
        /// </summary>
        /// <param name="type">��� ����������</param>
        /// <returns>��� ����������</returns>
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
                    throw new Exception("����������� ��� �������");
            }
        }

        internal static AssociationClassTypes CheckRolesClasses(AssociationClassTypes associationClassType, ClassTypes roleDataClassType, ClassTypes roleBridgeClassType, string objFullName)
        {
            switch (associationClassType)
            {
				// ��������� ��������� ���������� �������������:
				// ������������� ������ -> ������������
				// ������������         -> ������������
				case AssociationClassTypes.Bridge:
                    if (!(roleDataClassType == ClassTypes.clsDataClassifier || roleDataClassType == ClassTypes.clsBridgeClassifier))
                        throw new ServerException("Role Data in association \"" + objFullName + "\" must reference to DataCls or BridgeCls");

                    if (roleBridgeClassType != ClassTypes.clsBridgeClassifier)
						throw new ServerException("Role Bridge in association \"" + objFullName + "\" must reference to BridgeCls");
                    break;

                // ��������� ��������� ���������� �������������:
                // ������������         -> ������������ (objectKey == objectKey)
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

				// ��������� ��������� ����������:
				// ������� ������ -> ������������ (������������� ������������� � AssociationClassTypes.Fact2Bridge)
				// �������������  -> ������������� | �������������
				// ������������   -> ������������� | ������������ | �������������
				// ������� ������ -> ������������� | ������������ | ������������� | ����� | ������� ������
				// ��������� �������� -> ��������� �������� | ������������� | �������������
				case AssociationClassTypes.Link:

					if (roleDataClassType == ClassTypes.Table && roleBridgeClassType == ClassTypes.DocumentEntity)
						throw new ServerException("������� �� ����� ��������� �� ��������� ��������: " + objFullName);

					if (roleDataClassType == ClassTypes.DocumentEntity)
					{
						if (roleBridgeClassType != ClassTypes.DocumentEntity && roleBridgeClassType != ClassTypes.clsDataClassifier && roleBridgeClassType != ClassTypes.clsFixedClassifier)
						{
							throw new ServerException(
								"��������� �������� ������ ��������� �� ��������� ��������, ������������� ������ ��� ������������� �������������: " +
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
						throw new ServerException("������������ ������������� ������ ��������� �� ������������� �������������, ������������ ��� ������������� ������: " + objFullName);

					if (roleDataClassType == ClassTypes.clsDataClassifier &&
						(/*roleBridgeClassType == ClassTypes.clsBridgeClassifier ||*/ roleBridgeClassType == ClassTypes.clsFactData || roleBridgeClassType == ClassTypes.DocumentEntity))
						throw new ServerException("������������ ������������� ������ ��������� �� ������������� ������������� ��� ������������� ������: " + objFullName);

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

				// ��������� ��������� ���������� ������-������:
				// ������� ������ -> ��� ��������� �������
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

        #endregion �������� ��������� �������� �� XML
    }
}
using System;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes.Visitors
{
    internal class MetaDataValidationVisitor : ValidationVisitor, IValidationVisitor 
    {
        #region IValidationVisitor Members

        public void Visit(IPackage node)
        {
            foreach (IEntity item in node.Classes.Values)
            {
                if (item as IBridgeClassifier != null)
                {
                    Visit(item as IBridgeClassifier);
                }
                else if (item as IClassifier != null)
                {
                    Visit(item as IClassifier);
                }
                else if (item as IFactTable != null)
                {
                    Visit(item as IFactTable);
                }
            }

            foreach (IEntityAssociation item in node.Associations.Values)
            {
                if (item as IBridgeAssociation != null)
                {
                    Visit(item as IBridgeAssociation);
                }
                else
                {
                    Visit(item);
                }
            }

            foreach (IPackage item in node.Packages.Values)
            {
                Visit(item);
            }
        }

        private void Visit(IBridgeClassifier node)
        {
            Visit(node as IClassifier);
        }

        private void Visit(IClassifier node)
        {
            CheckNameLength(node);

            if (node is SystemDataSourcesClassifier)
                return;

            // � �������������� ��� �������� ��������� ������ ���� ��������� ������� ��� ��������
            foreach (IDimensionLevel item in node.Levels.Values)
            {
                if (item.LevelType != LevelTypes.All && item.MemberName.Name == DataAttribute.IDColumnName)
                {
                    LogError(ErrorType.MetaDataError, node.ParentPackage, node,
                        String.Format("� �������������� \"{0}\" ��� ������ �������� \"{1}\" �� ������ ������� ��� ����������� ����� ��������",
                            node.FullCaption, item.Name));
                }

                if (node.Levels.HierarchyType == HierarchyType.ParentChild &&
                    item.LevelType != LevelTypes.All &&
                    String.IsNullOrEmpty(item.LevelNamingTemplate))
                {
                    LogError(ErrorType.MetaDataError, node.ParentPackage, node,
                        String.Format("� �������������� \"{0}\" � ��������� ��� ������ \"{1}\" �� ������� ������� ����������� �������",
                            node.FullCaption, item.Name));
                }
            }

            // ���� ������������� ������� �� ������� � �� ������ ��� �������, ���������� ������ ��������� ����� ����������� ����������,
            // ����� � ���������� �� ������� ������� ������ ��������������
            if (node.IsDivided && node.SubClassType != SubClassTypes.Pump)
            {
                if (String.IsNullOrEmpty(node.DataSourceKinds))
                    LogError(ErrorType.MetaDataError, node.ParentPackage, node,
                        String.Format("� �������������� \"{0}\", ������� �������� �� �������, �� ������ �� ���� �� ����� ����������� ����������",
                            node.FullCaption));
            }

            Visit(node as IEntity);
        }

        private void Visit(IFactTable node)
        {
            CheckNameLength(node);

            // � ������ ������ ��� ������ ������ ���� �������������
            foreach (IEntityAssociation item in node.Associations.Values)
            {
                if (item.RoleDataAttribute.IsNullable)
                {
                    LogError(ErrorType.MetaDataError, item.ParentPackage, item,
                        String.Format("��� ������� ������ \"{0}\" ������ �� ������������� \"{1}\" ������ ���� ������������",
                            node.FullCaption, item.RoleBridge.FullCaption));
                }
            }

            // � ������ ������ ��������������� ��� ����� � ������ ��� ���� ��� ������ ���� ���������������
            if (node.SubClassType == SubClassTypes.Input/* || node.SubClassType == SubClassTypes.PumpInput*/)
            {
                int userAttributesCount = 0;
                foreach (IDataAttribute item in node.Attributes.Values)
                {
                    if (item.Class == DataAttributeClassTypes.Typed)
                    {
                        userAttributesCount++;
                    }
                }

                if (userAttributesCount > 1)
                {
                    foreach (IDataAttribute item in node.Attributes.Values)
                    {
                        if (item.Class == DataAttributeClassTypes.Typed && !item.IsNullable)
                        {
                            LogError(ErrorType.MetaDataError, node.ParentPackage, node,
                                String.Format("� ������� ������ \"{0}\", ��������������� ��� ����� ������ � ������, ������� \"{1}\" ������ ���� ��������������",
                                    node.FullCaption, item.Caption));
                        }
                    }
                }
            }

            Visit(node as IEntity);
        }

        public void Visit(IEntity node)
        {
        }

        public void Visit(IBridgeAssociation node)
        {
            // ��������� ������� ������ �������������
            if (node.AssociateRules.Count == 0)
            {
                LogError(ErrorType.MetaDataError, node.ParentPackage, node, "�� ���������� �� ������ ������� �������������");
            }

            // ��������� ������������ ����� ��������� ��� ������ ������������
            foreach (IAssociateMapping item in node.Mappings)
            {
                if (item.BridgeValue.IsSample && item.DataValue.IsSample)
                {
                    if (!DataAttribute.IsConvertableTypes(item.DataValue.Attribute.Type, item.BridgeValue.Attribute.Type))
                    {
                        LogError(ErrorType.MetaDataError, node.ParentPackage, node,
                            "� ������� ������������ \"{0} -> {1}\" ��� ���� ��������� �������������� �� ����������������� � ��� ����� ������������. �������� ������ ��� ������������ ��������������",
                            item.DataValue.Name, item.BridgeValue.Name);
                    }
                    if (item.DataValue.Attribute.Size > item.BridgeValue.Attribute.Size)
                    {
                        LogError(ErrorType.MetaDataError, node.ParentPackage, node,
                            "� ������� ������������ \"{0} -> {1}\" ����������� ���� ������������ �������������� ������ ������� ��������� �����. �������� ������ ��� ������������ ��������������",
                            item.DataValue.Name, item.BridgeValue.Name);
                    }
                }
                else
                {
                }
            }

            // ��������� ������� ������� ������������� �� ���������
            if (String.IsNullOrEmpty(node.AssociateRules.AssociateRuleDefault))
            {
                LogError(ErrorType.MetaDataError, node.ParentPackage, node, "� ���������� �� ������� ������� ������������� �� ���������.");
            }

            // ��������� ������� ������������ � ������ �������������
            foreach (IAssociateRule item in node.AssociateRules.Values)
            {
                if (item.Mappings.Count == 0)
                {
                    LogError(ErrorType.MetaDataError, node.ParentPackage, node,
                        "� ������� ������������� \"{0}\" �� ���������� �� ������ ������� ������������",
                        item.Name);
                }

                // �������� ������������� �� ������ ���� ��������.
                foreach (AssociateMapping mapping in item.Mappings)
                {
                    if (mapping.BridgeAttribute.IsSample && mapping.BridgeAttribute.Attribute == null
                        || mapping.BridgeAttribute.IsSample && mapping.BridgeAttribute.Attribute != null && mapping.BridgeAttribute.Attribute.Class == DataAttributeClassTypes.Reference)
                    {
                        LogError(ErrorType.MetaDataError, node.ParentPackage, node,
                            String.Format("� ������� ������������� \"{0} \\ {1}\" �� ������� ������������� " +
                                "�������������� ����������� ������������ �������-������ ({2})",
                                node.FullCaption, item.Name, mapping.BridgeAttribute.Name),
                            item.Name);
                    }

                    if (mapping.DataAttribute.IsSample && mapping.DataAttribute.Attribute == null
                        || mapping.DataAttribute.IsSample && mapping.DataAttribute.Attribute != null && mapping.DataAttribute.Attribute.Class == DataAttributeClassTypes.Reference)
                    {
                        LogError(ErrorType.MetaDataError, node.ParentPackage, node,
                            String.Format("� ������� ������������� \"{0} \\ {1}\" �� ������� �������������� " +
                                "������ ����������� ������������ �������-������ ({2})",
                                node.FullCaption, item.Name, mapping.DataAttribute.Name),
                            item.Name);
                    }
                }
            }

            Visit(node as IEntityAssociation);
        }

        public void Visit(IEntityAssociation node)
        {
        }

        public void Visit(IDocument node)
        {
        }

        public void Visit(IDataAttribute node)
        {
        }

        #endregion

        private void CheckNameLength(IEntity node)
        {
            if (node.Semantic.Length + node.Name.Length > 16)
            {
                var message = String.Format(
                    "� ������� \"{0}\" ������� ������� ���������� ������������ \"{1}\". ���������� ��������� ����� ����������� ����� ��� ���������.",
                    node.FullCaption, 
                    node.FullDBName);
                
                LogError(ErrorType.MetaDataError, node.ParentPackage, node, message);
            }
        }
    }
}

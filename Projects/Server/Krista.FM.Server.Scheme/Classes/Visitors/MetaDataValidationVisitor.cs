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

            // У классификатора для иерархии измерения должны быть корректно указаны все атрибуты
            foreach (IDimensionLevel item in node.Levels.Values)
            {
                if (item.LevelType != LevelTypes.All && item.MemberName.Name == DataAttribute.IDColumnName)
                {
                    LogError(ErrorType.MetaDataError, node.ParentPackage, node,
                        String.Format("У классификатора \"{0}\" для уровня иерархии \"{1}\" не указан атрибут для разыменовки имени элемента",
                            node.FullCaption, item.Name));
                }

                if (node.Levels.HierarchyType == HierarchyType.ParentChild &&
                    item.LevelType != LevelTypes.All &&
                    String.IsNullOrEmpty(item.LevelNamingTemplate))
                {
                    LogError(ErrorType.MetaDataError, node.ParentPackage, node,
                        String.Format("У классификатора \"{0}\" с иерархией для уровня \"{1}\" не указано правило разыменовки уровней",
                            node.FullCaption, item.Name));
                }
            }

            // Если классификатор делится по версиям и не только для закачки, необходимо задать каллекцию видов поступающей информации,
            // иначе в воркплейсе не удастся создать версию классификатора
            if (node.IsDivided && node.SubClassType != SubClassTypes.Pump)
            {
                if (String.IsNullOrEmpty(node.DataSourceKinds))
                    LogError(ErrorType.MetaDataError, node.ParentPackage, node,
                        String.Format("У классификатора \"{0}\", который делиться по версиям, не указан ни один из видов поступающей информации",
                            node.FullCaption));
            }

            Visit(node as IEntity);
        }

        private void Visit(IFactTable node)
        {
            CheckNameLength(node);

            // У таблиц фактов все ссылки должны быть обязательными
            foreach (IEntityAssociation item in node.Associations.Values)
            {
                if (item.RoleDataAttribute.IsNullable)
                {
                    LogError(ErrorType.MetaDataError, item.ParentPackage, item,
                        String.Format("Для таблицы фактов \"{0}\" ссылка на классификатор \"{1}\" должна быть обязательной",
                            node.FullCaption, item.RoleBridge.FullCaption));
                }
            }

            // У таблиц фактов предназначенных для ввода с листов все поля мер должны быть необязательными
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
                                String.Format("У таблицы фактов \"{0}\", предназначенной для ввода данных с листов, атрибут \"{1}\" должен быть необязательным",
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
            // Проверяем наличие правил сопоставления
            if (node.AssociateRules.Count == 0)
            {
                LogError(ErrorType.MetaDataError, node.ParentPackage, node, "Не определено ни одного правила сопоставления");
            }

            // Проверяем соответствие типов атрибутов для правил формирования
            foreach (IAssociateMapping item in node.Mappings)
            {
                if (item.BridgeValue.IsSample && item.DataValue.IsSample)
                {
                    if (!DataAttribute.IsConvertableTypes(item.DataValue.Attribute.Type, item.BridgeValue.Attribute.Type))
                    {
                        LogError(ErrorType.MetaDataError, node.ParentPackage, node,
                            "В правиле формирования \"{0} -> {1}\" тип поля исходного классификатора не преобразовывается в тип проля формируемого. Возможны ошибки при формировании классификатора",
                            item.DataValue.Name, item.BridgeValue.Name);
                    }
                    if (item.DataValue.Attribute.Size > item.BridgeValue.Attribute.Size)
                    {
                        LogError(ErrorType.MetaDataError, node.ParentPackage, node,
                            "В правиле формирования \"{0} -> {1}\" размерность поля формируемого классификатора меньше размера исходного проля. Возможны ошибки при формировании классификатора",
                            item.DataValue.Name, item.BridgeValue.Name);
                    }
                }
                else
                {
                }
            }

            // Проверяем наличие правила сопоставления по умолчанию
            if (String.IsNullOrEmpty(node.AssociateRules.AssociateRuleDefault))
            {
                LogError(ErrorType.MetaDataError, node.ParentPackage, node, "У ассоциации не указано правило сопоставления по умолчанию.");
            }

            // Проверяем наличие соответствий у правил сопоставления
            foreach (IAssociateRule item in node.AssociateRules.Values)
            {
                if (item.Mappings.Count == 0)
                {
                    LogError(ErrorType.MetaDataError, node.ParentPackage, node,
                        "У правила сопоставления \"{0}\" не определено ни одного правила соответствия",
                        item.Name);
                }

                // Атрибуты сопоставления не должны быть ссылками.
                foreach (AssociateMapping mapping in item.Mappings)
                {
                    if (mapping.BridgeAttribute.IsSample && mapping.BridgeAttribute.Attribute == null
                        || mapping.BridgeAttribute.IsSample && mapping.BridgeAttribute.Attribute != null && mapping.BridgeAttribute.Attribute.Class == DataAttributeClassTypes.Reference)
                    {
                        LogError(ErrorType.MetaDataError, node.ParentPackage, node,
                            String.Format("В правиле сопоставления \"{0} \\ {1}\" со стороны сопоставимого " +
                                "классификатора использован недопустимый атрибут-ссылка ({2})",
                                node.FullCaption, item.Name, mapping.BridgeAttribute.Name),
                            item.Name);
                    }

                    if (mapping.DataAttribute.IsSample && mapping.DataAttribute.Attribute == null
                        || mapping.DataAttribute.IsSample && mapping.DataAttribute.Attribute != null && mapping.DataAttribute.Attribute.Class == DataAttributeClassTypes.Reference)
                    {
                        LogError(ErrorType.MetaDataError, node.ParentPackage, node,
                            String.Format("В правиле сопоставления \"{0} \\ {1}\" со стороны классификатора " +
                                "данных использован недопустимый атрибут-ссылка ({2})",
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
                    "У таблицы \"{0}\" слишком длинное английское наименование \"{1}\". Попробуйте уменьшить длину английского имени или семантики.",
                    node.FullCaption, 
                    node.FullDBName);
                
                LogError(ErrorType.MetaDataError, node.ParentPackage, node, message);
            }
        }
    }
}

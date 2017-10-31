using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    /// <summary>
    /// Строит sql-запрос для сущности. 
    /// В запрос включены значения по умолчанию и lookup-поля.
    /// </summary>
    public class DefaultValuesQueryBuilder : IEntityDataQueryBuilder
    {
        public StringBuilder BuildQuery(IEntity entity, string concatenateChar)
        {
            int aliasCounter = 0;
            StringBuilder fields = new StringBuilder();
            List<string> joins = new List<string>();

            // добавляем в запрос все поля из сущности
            foreach (var attribute in entity.Attributes.Values)
            {
                fields.Append(",");

                if (attribute.Name == "ID")
                {
                    fields.Append("-1");
                }
                else if (attribute.DefaultValue != null)
                {
                    fields.Append(attribute.DefaultValue);
                }
                else
                {
                    fields.Append("null");
                }

                fields.Append(" as ")
                    .Append(attribute.Name);
            }

            fields.Remove(0, 1);

            // первичная таблица
            joins.Add("dual t{0}".FormatWith(aliasCounter++));

            // добавляем лукапы
            foreach (var association in entity.Associations.Values)
            {
                string tableName = association.RoleBridge.FullDBName;
                string refField = association.RoleDataAttribute.Name;
                string aliasName = "t{0}".FormatWith(aliasCounter++);

                if (association.RoleDataAttribute.DefaultValue != null)
                {
                    // подцепляем таблицу для вычисления лукапа
                    joins.Add("left outer join {0} {2} on ({1} = {2}.ID)"
                        .FormatWith(tableName, association.RoleDataAttribute.DefaultValue, aliasName));
                }

                // добавляем lookup-поля
                List<string> primaryLookups = new List<string>();
                List<string> secondaryLookups = new List<string>();
                foreach (var attribute in association.RoleBridge.Attributes.Values)
                {
                    string attributeNameClause = "{0}.{1}".FormatWith(aliasName, attribute.Name);
                    if (association.RoleDataAttribute.DefaultValue == null)
                    {
                        attributeNameClause = "null";
                    }

                    switch (attribute.LookupType)
                    {
                        case LookupAttributeTypes.Primary:
                            primaryLookups.Add("cast({0} as varchar({1}))"
                                .FormatWith(attributeNameClause, attribute.Size == 0 ? 25 : attribute.Size));
                            break;
                        case LookupAttributeTypes.Secondary:
                            secondaryLookups.Add("cast({0} as varchar({1}))"
                                .FormatWith(attributeNameClause, attribute.Size == 0 ? 25 : attribute.Size));
                            break;
                    }
                }

                if (primaryLookups.Count > 0)
                {
                    fields.Append(",")
                        .Append(String.Join("{0}'; '{0}".FormatWith(concatenateChar), primaryLookups.ToArray()))
                        .Append(" as lp_")
                        .Append(refField);
                }

                if (secondaryLookups.Count > 0)
                {
                    fields.Append(",")
                        .Append(String.Join("{0}'; '{0}".FormatWith(concatenateChar), secondaryLookups.ToArray()))
                        .Append(" as ls_")
                        .Append(refField);
                }
            }

            StringBuilder query = new StringBuilder();
            query.Append("select ")
                .Append(fields)
                .Append(" from ")
                .Append(String.Join(" ", joins.ToArray()));
            return query;
        }
    }
}

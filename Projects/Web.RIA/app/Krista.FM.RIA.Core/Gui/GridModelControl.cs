using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Helpers;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core.Gui
{
    public class GridModelControl : GridControl
    {
        private ViewPage page;

        public IEntity Entity { get; set; }
        
        public override List<Component> Build(ViewPage page)
        {
            this.page = page;
            IEnumerable<IDataAttribute> attributes = Presentation == null
                ? Entity.Attributes.Values
                : Presentation.Attributes.Values;
            ObjectKey = Entity.ObjectKey;
            LookUpNames = new Dictionary<string, ReferencerInfo>();
            IsDivided = Entity is IDataSourceDividedClass
                            ? ((IDataSourceDividedClass)Entity).IsDivided
                            : false;

            CreateLookupsAndRecordFields(attributes);

            Columns = new List<ColumnBase>();

            // Если иерархический, добавляем поле для иерархии
            if (ParentId.IsNullOrEmpty())
            {
                MasterColumnId = null;
            }
            else
            {
                var attr = MasterColumnName(attributes);
                MasterColumnId = attr == null ? MasterColumnName() : attr.Name.ToUpper();

                // Создаем главную колонку - дерево
                if (attr != null)
                {
                    var column = CreateGridColumn(attr);
                    column.Hideable = false;
                    column.Width = 150;
                    Columns.Add(column);
                }
            }

            // Создаем остальные колонки
            CreateColumnModel(attributes, Entity.Associations.Values);
            RecordFields = CreateRecords();

            CreateRefFieldsStylesAndMetadata(Id, attributes);

            Filters = CreateFilters(attributes);

            return base.Build(page);
        }

        protected static ColumnBase AddColumn(IDataAttribute attribute)
        {
            return AddColumn(attribute.Name, attribute.Caption, attribute.Type, attribute.IsNullable ? Mandatory.Nullable : Mandatory.NotNull);
        }

        protected virtual void CreateColumnModel(IEnumerable<IDataAttribute> attributes, ICollection<IEntityAssociation> associations)
        {
            // Системные и фиксированные поля не отображаются
            Columns.AddRange(attributes.Where(FilterAttributes).Select(CreateGridColumn).ToList());
            if (associations == null)
            {
                return;
            }

            foreach (var association in associations)
            {
                var associationRda = association.RoleDataAttribute;
                var columnId = associationRda.Name;
                var dataIndex = associationRda.Name.ToUpper();

                if (dataIndex.ToUpper().Equals(MasterColumnId))
                {
                    continue;
                }

                if (associationRda.Class == DataAttributeClassTypes.Reference)
                {
                    dataIndex = "LP_{0}".FormatWith(dataIndex);
                }

                var column = AddColumn(
                    columnId,
                    dataIndex,
                    association.RoleBridge.FullCaption,
                    associationRda.Type,
                    associationRda.IsNullable ? Mandatory.Nullable : Mandatory.NotNull);

                if (!Readonly)
                {
                    List<ReferencerInfo.FieldInfo> primary = new List<ReferencerInfo.FieldInfo>();
                    List<ReferencerInfo.FieldInfo> secondary = new List<ReferencerInfo.FieldInfo>();
                    foreach (var dataAttribute in association.RoleBridge.Attributes.Values)
                    {
                        if (dataAttribute.LookupType == LookupAttributeTypes.Primary)
                        {
                            primary.Add(new ReferencerInfo.FieldInfo
                                            {
                                                Caption = dataAttribute.Caption, 
                                                Name = dataAttribute.Name
                                            });
                        }
                        else if (dataAttribute.LookupType == LookupAttributeTypes.Secondary)
                        {
                            secondary.Add(new ReferencerInfo.FieldInfo
                                              {
                                                  Caption = dataAttribute.Caption, 
                                                  Name = dataAttribute.Name
                                              });
                        }
                    }
                    
                    if (primary.Count > 0)
                    {
                        LookUpNames.Add(columnId, new ReferencerInfo(association.RoleBridge.ObjectKey, primary, secondary));
                    }

                    ((Column)column).Commands.Add(new ImageCommand
                                                       {
                                                           CommandName = "EditRefCell",
                                                           Icon = Icon.ApplicationFormEdit,
                                                           Style = "margin-left:5px !important;",
                                                           Hidden = false
                                                       });
                }

                Columns.Add(column);
            }
        }

        protected virtual bool FilterAttributes(IDataAttribute attribute)
        {
           return IsBook
                       ? (attribute.LookupType != LookupAttributeTypes.None && !attribute.Name.ToUpper().Equals(MasterColumnId))
                       : !attribute.Name.ToUpper().Equals(MasterColumnId) && attribute.Class != DataAttributeClassTypes.Reference;
        }

        protected virtual ColumnBase CreateGridColumn(IDataAttribute attribute)
        {
            var column = AddColumn(attribute);

            // Видимые  поля:
            // Для справочников:  у которых установлено свойство LookupType,
            // Для не справочников: свойсто visible = true
            column.Hidden = IsBook ? false
                : !(attribute.Visible && 
                    (attribute.Class == DataAttributeClassTypes.Reference || 
                    attribute.Class == DataAttributeClassTypes.Typed));

            if (!Readonly)
            {
                column.SetEditable(attribute);
            }

            return column;
        }

        /// <summary>
        /// Определяет атрибут для иерархии: name, code или дополнительный столбец
        /// </summary>
        /// <param name="values">Список параметров (имен столбцов)</param>
        /// <returns> Данные об атрибуте, по которому производится иерархия</returns>        
        protected IDataAttribute MasterColumnName(IEnumerable<IDataAttribute> values)
        {
            IDataAttribute returnvalue = values.First();
            string name;
            foreach (var attribute in values)
            {
                name = attribute.Name.ToUpper();
                if (name.Equals("NAME"))
                {
                    return attribute;
                }

                if (name.Equals("CODE"))
                {
                    returnvalue = attribute;
                }
            }

            return returnvalue;
        }

        private static List<GridFilter> CreateFilters(IEnumerable<IDataAttribute> attributes)
        {
            var result = new List<GridFilter>();
            foreach (var attribute in attributes)
            {
                if (attribute.Name == "ID")
                {
                    continue;
                }

                // В гриде видны поля у которых установлено свойство LookupType 
                if (attribute.LookupType == LookupAttributeTypes.None)
                {
                    continue;
                }

                GridFilter filter = null;
                switch (attribute.Type)
                {
                    case DataAttributeTypes.dtDouble:
                        filter = new NumericFilter { DataIndex = attribute.Name.ToUpper() };
                        break;
                    case DataAttributeTypes.dtInteger:
                        filter = new NumericFilter { DataIndex = attribute.Name.ToUpper() };
                        break;
                    case DataAttributeTypes.dtString:
                        filter = new StringFilter { DataIndex = attribute.Name.ToUpper() };
                        break;
                    case DataAttributeTypes.dtDateTime:
                        filter = new DateFilter { DataIndex = attribute.Name.ToUpper() };
                        break;
                }

                if (filter != null)
                {
                    if (attribute.Class == DataAttributeClassTypes.Reference)
                    {
                        filter = new StringFilter { DataIndex = "LP_{0}".FormatWith(attribute.Name.ToUpper()) };
                    }

                    result.Add(filter);
                }
            }

            return result;
        }

        private static ColumnBase AddColumn(string columnId, string header, DataAttributeTypes attributeType, Mandatory mandatory)
        {
            return AddColumn(columnId, columnId.ToUpper(), header, attributeType, mandatory);
        }

        private static ColumnBase AddColumn(string columnId, string dataIndex, string header, DataAttributeTypes attributeType, Mandatory mandatory)
        {
            ColumnBase column = ColumnFactory(attributeType);
            column.ColumnID = columnId;
            column.DataIndex = dataIndex;
            column.Header = header;
            column.Wrap = true;

            if (mandatory == Mandatory.Nullable)
            {
                column.CustomConfig.Add(new ConfigItem("AllowBlank", "true"));
                column.Css = "font-style: italic;";
                column.Header = String.Format("<i>{0}</i>", column.Header);
            }
            else
            {
                column.CustomConfig.Add(new ConfigItem("AllowBlank", "false"));
            }

            return column;
        }

        private static ColumnBase ColumnFactory(DataAttributeTypes attributeType)
        {
            switch (attributeType)
            {
                case DataAttributeTypes.dtBoolean:
                    return new CheckColumn();
                case DataAttributeTypes.dtDate:
                case DataAttributeTypes.dtDateTime:
                    return new Column
                    {
                        Renderer = new Renderer(@"
if(value){
    if(typeof(value) == 'string'){
        if (/\d{2}.\d{2}.\d{4}/.test(value)){
            return Date.parseDate(value, 'd.m.Y').format('d.m.Y');
        };
        if (/\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}.\d{3}/.test(value)){
            return Date.parseDate(value, 'Y-m-dTH:i:s.u').format('d.m.Y');
        };        
    }
    return value.format('d.m.Y');
}
return value;")
                    };
                case DataAttributeTypes.dtDouble:
                    return new NumberColumn();
                default:
                    return new Column();
            }
        }

        private RecordFieldCollection CreateRecords()
        {
            var result = new RecordFieldCollection();
            foreach (IDataAttribute attribute in Entity.Attributes.Values)
            {
                result.AddRecordField(attribute);
            }

            return result;
        }

        private void CreateLookupsAndRecordFields(IEnumerable<IDataAttribute> attributes)
        {
            var sb1 = new StringBuilder();

            RecordFields = new RecordFieldCollection();    

            if (Readonly)
            {
                // Добавляем поля в Store);
                foreach (var attribute in attributes)
                {
                    // Формируем метаданные лукапов
                    if (attribute.LookupType == LookupAttributeTypes.Primary)
                    {
                        sb1.AppendLine("Extension.entityBook.lookup.primary.push('{0}');".FormatWith(attribute.Name.ToUpper()));
                    }

                    if (attribute.LookupType == LookupAttributeTypes.Secondary)
                    {
                        sb1.AppendLine("Extension.entityBook.lookup.secondary.push('{0}');".FormatWith(attribute.Name.ToUpper()));
                    }

                    if (attribute.Name == "ID")
                    {
                        continue;
                    }

                    RecordFields.AddRecordField(attribute);
                }
            }

            Lookups = sb1.ToString();
        }

        /// <summary>
        /// Создаем стили и метаданные для ссылочных колонок.
        /// </summary>
        private void CreateRefFieldsStylesAndMetadata(string gridPanelId, IEnumerable<IDataAttribute> attributes)
        {
            var sb = new StringBuilder();
            var style = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("Ext.ns('MetaData.{0}');".FormatWith(gridPanelId));
            sb.AppendLine("MetaData.{0} = {{".FormatWith(gridPanelId));
            foreach (var attribute in attributes
                .Where(x => x.Class == DataAttributeClassTypes.Reference))
            {
                // Стиль для ссылочных полей
                style.Append(".x-grid3-col-")
                    .Append(attribute.Name)
                    .AppendLine("{white-space: normal;}");

                IEntityAssociation association = Entity.Associations[attribute.ObjectKey];
                sb.AppendLine(String.Format(
                    "  {0} : {{ objectKey : '{1}', caption : '{2}'}},",
                    attribute.Name.ToUpper(),
                    association.RoleBridge.ObjectKey,
                    association.RoleBridge.FullCaption));
            }

            if (sb[sb.Length - 3] == ',')
            {
                sb.Remove(sb.Length - 3, 1);
            }

            sb.AppendLine("};");
            
            foreach (KeyValuePair<string, string> param in Params)
            {
                sb.Append(gridPanelId).Append(".").Append(param.Key).Append(" = '")
                    .Append(param.Value).AppendLine("';");
            }

            sb.AppendLine("{0}.ModelKey = '{1}';".FormatWith(gridPanelId, Entity.FullDBName));
            sb.AppendLine("{0}.ModelObjectKey = '{1}';".FormatWith(gridPanelId, Entity.ObjectKey));

            MetaData = sb.ToString();
            RefFieldStyles = style.ToString();
        }
    }
}

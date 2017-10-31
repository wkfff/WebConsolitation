using System;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;
using Krista.FM.RIA.Core.ViewModel;

namespace Krista.FM.RIA.Core
{
    public static class ExtNetЫещкуExtensions
    {
        public static RecordField AddRecordField(this RecordFieldCollection collection, IDataAttribute attribute)
        {
            RecordField rf = new RecordField();

            if (attribute.DefaultValue != null)
                rf.DefaultValue = Convert.ToString(attribute.DefaultValue);
            
            if (attribute.Class == DataAttributeClassTypes.Reference)
            {
                // Для ссылочных полей добавляем два поля:
                // реальное значение поля
                rf.Name = attribute.Name.ToUpper();
                collection.Add(rf);

                // разименованное значение поля
                rf = new RecordField();
                rf.Name = "LP_{0}".FormatWith(attribute.Name.ToUpper());

                if (attribute.DefaultValue != null)
                    rf.DefaultValue = Convert.ToString(attribute.DefaultValue);
                
                collection.Add(rf);

                return rf;
            }

            rf.Name = attribute.Name.ToUpper();

            if (attribute.Type == DataAttributeTypes.dtDouble)
            {
                rf.Type = RecordFieldType.Float;
            }

            if (attribute.Type == DataAttributeTypes.dtDate || attribute.Type == DataAttributeTypes.dtDateTime)
            {
                rf.Type = RecordFieldType.Date;
                //rf.Convert.Handler = @"if(value){return new Date(value).format('d-m-Y');}";
                //rf.DateFormat = "d-m-Y";
            }

            collection.Add(rf);
            return rf;
        }
    }

    public static class ExtNetGridPanelExtensions
    {
        private static ColumnBase ColumnFactory(IDataAttribute attribute)
        {
            switch (attribute.Type)
            {
                case DataAttributeTypes.dtBoolean:
                    return new CheckColumn();
                case DataAttributeTypes.dtDate:
                case DataAttributeTypes.dtDateTime:
                    return new DateColumn
                    {
                        Renderer = new Renderer(
                            "if(value){return new Date(value).format('d.m.Y');}return value;")
                    };
                case DataAttributeTypes.dtDouble:
                    return new NumberColumn();
                default:
                    return new Column();
            }
        }

        public static ColumnBase AddColumn(this ColumnModel cm, IDataAttribute attribute)
        {
            ColumnBase column = ColumnFactory(attribute);
            column.ColumnID = attribute.Name;
            column.DataIndex = attribute.Name.ToUpper();
            column.Header = attribute.Caption;
            column.Wrap = true;

            if (attribute.IsNullable)
            {
                column.Css = "font-style: italic;";
                column.Header = String.Format("<i>{0}</i>", column.Header);
            }

            cm.Columns.Add(column);

            return column;
        }

        public static ColumnBase SetEditable(this ColumnBase column, IDataAttribute attribute)
        {
            if (column != null)
            {
                column.Editable = true;

                if (attribute.Class == DataAttributeClassTypes.Typed)
                {
                    Field field = null;
                    switch (attribute.Type)
                    {
                        case DataAttributeTypes.dtDouble:
                            field = new NumberField
                            {
                                AllowBlank = attribute.IsNullable,
                                AllowDecimals = true,
                                DecimalPrecision = attribute.Scale,
                                
                            };
                            column.Align = Alignment.Right;
                            break;
                        case DataAttributeTypes.dtInteger:
                            field = new NumberField
                            {
                                AllowBlank = attribute.IsNullable,
                                AllowDecimals = false
                            };
                            break;
                        case DataAttributeTypes.dtString:
                            field = new TextArea
                            {
                                AllowBlank = attribute.IsNullable
                            };
                            break;
                        //case DataAttributeTypes.dtBoolean:
                        //    field = new Checkbox();
                        //    break;
                        case DataAttributeTypes.dtDate:
                        case DataAttributeTypes.dtDateTime:
                            field = new DateField
                            {
                                AllowBlank = attribute.IsNullable,
                                Format = "d.m.Y"
                            };
                            break;
                    }
                    if (field != null)
                        column.Editor.Add(field);
                }
            }
            return column;
        }


        public static ColumnBase SetGroup(this ColumnBase column, string groupName)
        {
            if (column != null)
            {
                column.Groupable = true;
                column.GroupName = groupName;
            }
            return column;
        }

        public static ColumnBase SetWidth(this ColumnBase column, int width)
        {
            if (column != null)
                column.Width = width;
            return column;
        }

        public static ColumnBase SetHidden(this ColumnBase column, bool value)
        {
            if (column != null)
                column.Hidden = value;
            return column;
        }

        public static ColumnBase SetLocked(this ColumnBase column)
        {
            if (column != null)
                column.Locked = true;
            return column;
        }
    }

    public static class ExtNetExtensions
    {
        public static Field AddFormField(this ItemsCollection<Component> items, IDataAttribute attribute, ColumnState columnState)
        {
            Field field = null;

            if (attribute.Class == DataAttributeClassTypes.Reference)
            {
                field = new TriggerField
                            {
                                ID = "LP_{0}".FormatWith(attribute.Name.ToUpper()),
                                FieldLabel = attribute.Caption,
                                AllowBlank = attribute.IsNullable
                            };

                if (columnState != null)
                    field.Width = columnState.Width;

                IEntityAssociation association = attribute.OwnerObject as IEntityAssociation;
                if (association == null)
                    association = ((IEntity) attribute.OwnerObject.OwnerObject).Associations[attribute.ObjectKey];
                string bookObjectKey = association.RoleBridge.ObjectKey;
                
                field.AddListener("TriggerClick", new JFunction("triggerClick(args, '{0}');".FormatWith(bookObjectKey), "args"));
                items.Add(field);
                return field;
            }

            switch (attribute.Type)
            {
                case DataAttributeTypes.dtString:
                    field = new TextField();
                    break;
                case DataAttributeTypes.dtInteger:
                    field = new NumberField();
                    break;
                case DataAttributeTypes.dtDouble:
                    field = new NumberField();
                    break;
                case DataAttributeTypes.dtDate:
                    field = new DateField();
                    break;
                case DataAttributeTypes.dtBoolean:
                    field = new Checkbox();
                    break;
            }
            if (field != null)
            {
                field.ID = attribute.Name.ToUpper();
                field.FieldLabel = attribute.Caption;
                if (field is TextFieldBase)
                    ((TextFieldBase) field).AllowBlank = attribute.IsNullable;

                if (columnState != null)
                    field.Width = columnState.Width;

                items.Add(field);
                return field;
            }
            throw new Exception("Невозможно создать поле формы для атрибута \"{0}\".".FormatWith(attribute.Name));
        }

        public static Field AddHiddenFormField(this ItemsCollection<Component> items, IDataAttribute attribute)
        {
            Field field = new Hidden { ID = attribute.Name.ToUpper(), Hidden = true };
            items.Add(field);
            return field;
        }
    }
}

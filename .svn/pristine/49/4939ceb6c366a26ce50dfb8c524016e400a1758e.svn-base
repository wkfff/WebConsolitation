using System;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.ExtNet.Plugins.CurrencyField;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core
{
    public static class ExtNetFormPanelExtensions
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
                    AllowBlank = attribute.IsNullable,
                    TriggerIcon = TriggerIcon.Ellipsis,
                    ReadOnly = attribute.IsReadOnly
                };

                if (columnState != null)
                {
                    field.Width = columnState.Width;
                }

                IEntityAssociation association = attribute.OwnerObject as IEntityAssociation 
                    ?? ((IEntity)attribute.OwnerObject.OwnerObject).Associations[attribute.ObjectKey];
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
                    field = new NumberField { DecimalSeparator = ",", DecimalPrecision = attribute.Scale };
                    field.Plugins.Add(new CurrencyField());
                    break;
                case DataAttributeTypes.dtDate:
                    field = new DateField { Format = "dd.m.Y" };
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
                {
                    ((TextFieldBase)field).AllowBlank = attribute.IsNullable;
                }

                if (columnState != null)
                {
                    field.Width = columnState.Width;
                }

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

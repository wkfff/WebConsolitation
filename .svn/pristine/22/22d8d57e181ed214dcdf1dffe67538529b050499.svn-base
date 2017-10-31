using System;
using System.Linq.Expressions;

using Ext.Net;

using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Extensions
{
    public static class FormPanelExtensions
    {
        public static FormPanel GetDefaultFormPanel(string formId)
        {
            return new FormPanel
                       {
                            ID = formId,
                            Border = false,
                            AutoScroll = true,
                            Padding = 6,
                            MonitorValid = true,
                            DefaultAnchor = "98%",
                            Icon = Icon.PageWhiteText
                       };
        }

        public static Field AddFormField(IDataAttribute attribute)
        {
            Field field = null;

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

                    break;
                case DataAttributeTypes.dtDate:
                case DataAttributeTypes.dtDateTime:
                    field = new DateField { Format = "dd.MM.yyyy" };
                    break;
                case DataAttributeTypes.dtBoolean:
                    field = new Checkbox();
                    break;
            }

            if (field != null)
            {
                field.ID = attribute.Name;
                field.DataIndex = attribute.Name;
                field.FieldLabel = attribute.Description.IsNullOrEmpty() ? attribute.Caption : attribute.Description;
                if (field is TextFieldBase)
                {
                    ((TextFieldBase)field).AllowBlank = attribute.IsNullable;
                    if (attribute.Size != 0)
                    {
                        ((TextFieldBase)field).MaxLength = attribute.Size;
                    }
                    
                    ((TextFieldBase)field).SetValue(attribute.DefaultValue);
                }
                
                return field;
            }

            throw new Exception("Невозможно создать поле формы для атрибута \"{0}\".".FormatWith(attribute.Name));
        }

        public static Field AddFormField(this ItemsCollection<Component> items, IDataAttribute attribute)
        {
            var field = AddFormField(attribute);
            items.Add(field);
            return field;
        }

        /// <summary>
        /// Задает заголовок поля
        /// </summary>
        public static Field SetFieldLabel(this Field field, string label)
        {
            field.FieldLabel = label;
            return field;
        }

        public static Button AddSaveButton(this FormPanel form)
        {
            var handler = @"
window.{0}.form.submit({{
    waitMsg: 'Сохранение...',
    success: function (form, action) {{
        if (action.result && action.result.extraParams && action.result.extraParams.msg)
            window.Ext.net.Notification.show({{
                iconCls: 'icon-information',
                html: action.result.extraParams.msg,
                title: 'Сохранение',
                hideDelay: 2000
            }});
    }},
    failure: function (form, action) {{
        if (action.result && action.result.extraParams && action.result.extraParams.msg)
            window.Ext.Msg.show({{
                title: 'Ошибка сохранения',
                msg: action.result.extraParams.msg,
                buttons: window.Ext.Msg.OK,
                icon: window.Ext.MessageBox.ERROR,
                maxWidth: 1000
            }});
        window.{0}Store.reload();
    }}
}});".FormatWith(form.ID);

            return form.Toolbar().AddIconButton("{0}SaveBtn".FormatWith(form.ID), Icon.Disk, "Сохранить изменения", handler);
        }

        public static Button AddRefreshButton(this FormPanel form)
        {
            return form.Toolbar().AddIconButton("{0}RefreshBtn".FormatWith(form.ID), Icon.PageRefresh, "Обновить", "window.{0}Store.reload();".FormatWith(form.ID));
        }

        public static CompositeField GetCompositeField(string label)
        {
            return new CompositeField { FieldLabel = label, AnchorHorizontal = "100%" };
        }

        public static CompositeField GetCompositeField<T>(Expression<Func<T>> expr)
        {
            return new CompositeField { FieldLabel = UiBuilders.DescriptionOf(expr), AnchorHorizontal = "100%" };
        }
    }
}

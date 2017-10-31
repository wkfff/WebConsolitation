using System;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core
{
    public static class ExtNetStoreExtensions
    {
        public static RecordField AddRecordField(this RecordFieldCollection collection, IDataAttribute attribute)
        {
            RecordField rf = new RecordField();

            if (attribute.DefaultValue != null)
            {
                rf.DefaultValue = Convert.ToString(attribute.DefaultValue);
                if (attribute.Type == DataAttributeTypes.dtString)
                {
                    rf.DefaultValue = "'{0}'".FormatWith(rf.DefaultValue);
                }
            }

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
                {
                    rf.DefaultValue = Convert.ToString(attribute.DefaultValue);
                }

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
                rf.Convert.Handler = "if(value){return Date.parseDate(value, 'Y-m-dTH:i:s.u').format('d.m.Y');}";
            }

            collection.Add(rf);
            return rf;
        }

        public static RecordField SetDefaultValue(this RecordField field, bool defaultValue)
        {
            return field.SetDefaultValue(defaultValue ? "true" : "false");
        }

        public static RecordField SetDefaultValue(this RecordField field, int defaultValue)
        {
            return field.SetDefaultValue(Convert.ToString(defaultValue));
        }

        public static RecordField SetDefaultValue(this RecordField field, string defaultValue)
        {
            field.DefaultValue = defaultValue;

            return field;
        }

        public static RecordField IsDate(this RecordField field)
        {
            field.Type = RecordFieldType.Date;
            field.DateFormat = "Y-m-dTH:i:s";

            return field;
        }

        public static RecordField IsDateTime(this RecordField field)
        {
            field.Type = RecordFieldType.Date;
            field.Convert.Handler = "if(value){return Date.parseDate(value, 'Y-m-dTH:i:s.u').format('d.m.Y H:i');}";

            return field;
        }

        public static RecordField IsBoolean(this RecordField field)
        {
            field.Type = RecordFieldType.Boolean;

            return field;
        }

        public static RecordField IsYesNo(this RecordField field)
        {
            field.Type = RecordFieldType.Boolean;
            field.Convert.Handler = "if(value){ return 'Да'; } return 'Нет';";

            return field;
        }

        public static Store SetRestController(this Store store, string controller)
        {
            store.Restful = true;

            var url = "/{0}/".FormatWith(controller);
            store.Proxy.Add(new HttpProxy 
            { 
                RestAPI =
                {
                    CreateUrl = url + "Create",
                    ReadUrl = url + "Read",
                    UpdateUrl = url + "Update",
                    DestroyUrl = url + "Destroy",
                } 
            });

            return store;
        }

        public static Store SetHttpProxy(this Store store, string url)
        {
            return store.SetHttpProxy(url, HttpMethod.GET);
        }

        public static Store SetHttpProxy(this Store store, string url, HttpMethod httpMethod)
        {
            store.Proxy.Add(new HttpProxy { Url = url, Method = httpMethod });
            return store;
        }

        public static Store SetWriteHttpProxy(this Store store, string url)
        {
            store.UpdateProxy.Add(new HttpWriteProxy { Url = url, Method = HttpMethod.POST });
            return store;
        }

        public static Store SetJsonReader(this Store store)
        {
            return store.SetJsonReader("ID", "data");
        }

        public static Store SetJsonReader(this Store store, string idProperty, string root)
        {
            store.Reader.Add(new JsonReader { IDProperty = idProperty, Root = root });
            return store;
        }

        public static Store AddField(this Store store, string name)
        {
            store.AddField(new RecordField(name));
            return store;
        }

        public static Store AddField(this Store store, string name, RecordField.Config config)
        {
            config.Name = name;
            store.AddField(new RecordField(config));
            return store;
        }

        public static Store SetBaseParams(this Store store, string key, string value, ParameterMode mode)
        {
            store.BaseParams.Add(new Parameter(key, value, mode));
            return store;
        }

        public static Store SetBaseParams(this Store store, string key, int value)
        {
            store.BaseParams.Add(new Parameter(key, Convert.ToString(value), ParameterMode.Value));
            return store;
        }

        public static Store SetWriteBaseParams(this Store store, string key, string value, ParameterMode mode)
        {
            store.WriteBaseParams.Add(new Parameter(key, value, mode));
            return store;
        }

        public static Store SetSort(this Store store, string field, SortDirection sortDirection)
        {
            store.Sort(field, sortDirection);
            return store;
        }

        public static Store SetSaveLoadNotifications(this Store store)
        {
            store.Listeners.Save.Handler = "{0}.commitChanges(); {0}.reload();".FormatWith(store.ID) + "Ext.net.Notification.show({iconCls: 'icon-information', html: 'Изменения сохранены.', title: 'Уведомление', hideDelay: 2500});";
            store.Listeners.LoadException.Handler = "var msg = ''; if (e != null && e != undefined) msg = e.message; else msg = response.statusText; Ext.Msg.alert('Ошибка при загрузке данных', msg);";
            store.Listeners.SaveException.Handler = @"
if (response.extraParams != undefined && response.extraParams.responseText != undefined) {
    Ext.Msg.alert('Внимание', response.extraParams.responseText);
} else {
    var responseParams = Ext.decode(response.responseText);
    if (responseParams != undefined && responseParams.extraParams != undefined && responseParams.extraParams.responseText != undefined) {
        Ext.Msg.alert('Внимание', responseParams.extraParams.responseText);
    } else {
        Ext.Msg.alert('Внимание', 'Server failed');
    }
};
";
            return store;
        }

        public static Store OnLoad(this Store store, string scope, string functionName)
        {
            if (store != null)
            {
                store.Listeners.Load.Fn(scope, functionName);
            }

            return store;
        }

        public static Store OnLoad(this Store store, string handler)
        {
            if (store != null)
            {
                store.Listeners.Load.Handler = handler;
            }

            return store;
        }
    }
}

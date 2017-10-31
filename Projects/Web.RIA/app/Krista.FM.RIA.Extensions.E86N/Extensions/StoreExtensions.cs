using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Models;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.RestControllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;

namespace Krista.FM.RIA.Extensions.E86N.Extensions
{
    public static class StoreExtensions
    {
        /// <summary>
        /// Создает Store с полями из модели и добавляет на страницу
        /// </summary>
        public static Store StoreCreateDefault(
           string storeId,
           bool autoLoad,
           Type controllerType,
           ViewModelBase model,
           ViewPage page,
           string readActionName = "Read",
           string createActionName = "Create",
           string updateActionName = "Update",
           string deleteActionName = "Delete")
        {
            var store = StoreCreateDefault(
                                            "{0}Store".FormatWith(storeId),
                                            autoLoad,
                                            UiBuilders.GetControllerID(controllerType),
                                            readActionName,
                                            createActionName,
                                            updateActionName,
                                            deleteActionName);

            store.AddFieldsByClass(model);
            page.Controls.Add(store);

            return store;
        }

        /// <summary>
        /// Создает Store с полями из модели и добавляет на страницу
        /// </summary>
        public static Store StoreCreateDefault<T>(
           string storeId,
           bool autoLoad,
           Type controllerType,
           T model,
           ViewPage page,
           string readActionName = "Read",
           string createActionName = "Create",
           string updateActionName = "Update",
           string deleteActionName = "Delete")
        {
            var store = StoreCreateDefault(
                                            "{0}Store".FormatWith(storeId),
                                            autoLoad,
                                            UiBuilders.GetControllerID(controllerType),
                                            readActionName,
                                            createActionName,
                                            updateActionName,
                                            deleteActionName);

            store.AddFieldsByClass(model);
            page.Controls.Add(store);

            return store;
        }

        public static Store StoreCreateDefault(
            string storeId,
            bool autoLoad,
            Type controllerType,
            string readActionName = "Read",
            string createActionName = "Create",
            string updateActionName = "Update",
            string deleteActionName = "Delete")
        {
            return StoreCreateDefault(
                storeId,
                autoLoad,
                UiBuilders.GetControllerID(controllerType),
                readActionName,
                createActionName,
                updateActionName,
                deleteActionName);
        }

        public static Store StoreCreateDefault(
            string storeId,
            bool autoLoad,
            string restControllerName,
            string readActionName = "Read",
            string createActionName = "Create",
            string updateActionName = "Update",
            string deleteActionName = "Delete")
        {
            return new Store
                       {
                           ID = storeId,
                           ShowWarningOnFailure = true,
                           WarningOnDirty = true,
                           DirtyWarningText =
                               "Есть несохраненные изменения, которые будут потеряны при обновлении. Вы уверены, что хотите обновить данные?",
                           DirtyWarningTitle = @"Несохраненные изменения",
                           AutoLoad = autoLoad,
                           Restful = true,
                           Proxy =
                               {
                                   new HttpProxy
                                       {
                                           RestAPI =
                                               {
                                                   ReadUrl =
                                                       "/" + restControllerName + "/"
                                                       + readActionName,
                                                   CreateUrl =
                                                       "/" + restControllerName + "/"
                                                       + createActionName,
                                                   UpdateUrl =
                                                       "/" + restControllerName + "/"
                                                       + updateActionName,
                                                   DestroyUrl =
                                                       "/" + restControllerName + "/"
                                                       + deleteActionName
                                               }
                                       }
                               },
                           Reader =
                               {
                                   new JsonReader
                                       {
                                           IDProperty = "ID",
                                           Root = "data",
                                           SuccessProperty = "success"
                                       }
                               },
                           Listeners =
                               {
                                   LoadException =
                                       {
                                           Handler = @"Ext.Msg.alert('Ошибка загрузки', response.responseText);"
                                       },

                                   Save =
                                       {
                                           Handler =
                                               @"Ext.net.Notification.show({ iconCls : 'icon-information', html : arg.raw.message, title : 'Сохранение', hideDelay  : 2000});"
                                       },
                                   Destroy =
                                       {
                                           Handler =
                                               @"Ext.net.Notification.show({ iconCls : 'icon-information', html : arg.raw.message, title : 'Удаление', hideDelay  : 2000});"
                                       },
                                   SaveException =
                                       {
                                           Handler =
                                               @"Ext.Msg.alert('Ошибка сохранения', response.responseText);"
                                       },
                                   Exception =
                                       {
                                           Handler =
                                               @"if (response.raw != undefined && response.raw.message != undefined){Ext.Msg.alert('Общая ошибка', response.raw.message);}else{Ext.Msg.alert('Неизвестная ошибка', response.responseText);}"
                                       }
                               }
                       };
        }

        /// <summary>
        /// Создает Store с настраиваемыми контроллерами и экшенами
        /// </summary>
        public static Store StoreUrlCreateDefault(
            string storeId,
            bool autoLoad,
            string readUrl,
            string createUrl,
            string updateUrl,
            string destroyUrl)
        {
            return new Store
            {
                ID = storeId,
                ShowWarningOnFailure = true,
                WarningOnDirty = true,
                DirtyWarningText =
                               "Есть несохраненные изменения, которые будут потеряны при обновлении. Вы уверены, что хотите обновить данные?",
                DirtyWarningTitle = @"Несохраненные изменения",
                AutoLoad = autoLoad,
                Restful = true,
                Proxy =
                               {
                                   new HttpProxy
                                       {
                                           RestAPI =
                                               {
                                                   ReadUrl = readUrl,
                                                   CreateUrl = createUrl,
                                                   UpdateUrl = updateUrl,
                                                   DestroyUrl = destroyUrl
                                               }
                                       }
                               },
                Reader =
                               {
                                   new JsonReader
                                       {
                                           IDProperty = "ID",
                                           Root = "data",
                                           SuccessProperty = "success"
                                       }
                               },
                Listeners =
                               {
                                   LoadException =
                                       {
                                           Handler = @"Ext.Msg.alert('Ошибка загрузки', response.responseText);"
                                       },

                                   Save =
                                       {
                                           Handler =
                                               @"Ext.net.Notification.show({ iconCls : 'icon-information', html : arg.raw.message, title : 'Сохранение', hideDelay  : 2000});"
                                       },
                                   Destroy =
                                       {
                                           Handler =
                                               @"Ext.net.Notification.show({ iconCls : 'icon-information', html : arg.raw.message, title : 'Удаление', hideDelay  : 2000});"
                                       },
                                   SaveException =
                                       {
                                           Handler =
                                               @"Ext.Msg.alert('Ошибка сохранения', response.responseText);"
                                       },
                                   Exception =
                                       {
                                           Handler =
                                               @"if (response.raw != undefined && response.raw.message != undefined){Ext.Msg.alert('Общая ошибка', response.raw.message);}else{Ext.Msg.alert('Неизвестная ошибка', response.responseText);}"
                                       }
                               }
            };
        }

        /// <summary>
        /// Создает Store по модели с добавлением полей из модели и дефалтными значениями для полей
        /// </summary>
        public static Store StoreUrlCreateDefault<T>(
            string storeId,
            bool autoLoad,
            ViewPage page,
            T model,
            Dictionary<string, string> defaultValues = null)
        {
            var store = StoreUrlCreateDefault(
                                                "{0}Store".FormatWith(storeId),
                                                autoLoad,
                                                UiBuilders.GetUrl<DocCommonController>("ReadAction"),
                                                UiBuilders.GetUrl<DocCommonController>("SaveAction"),
                                                UiBuilders.GetUrl<DocCommonController>("SaveAction"),
                                                UiBuilders.GetUrl<DocCommonController>("DeleteAction"));

            store.SetBaseParams("modelType", model.GetType().AssemblyQualifiedName, ParameterMode.Value);
            store.SetWriteBaseParams("modelType", model.GetType().AssemblyQualifiedName, ParameterMode.Value);

            if (defaultValues == null)
            {
                store.AddFieldsByClass(model);
            }
            else
            {
                foreach (var info in model.GetType().GetProperties())
                {
                    if (defaultValues.ContainsKey(info.Name))
                    {
                        store.AddField(info.Name, defaultValues[info.Name]);
                    }
                    else
                    {
                        store.AddField(info.Name);
                    }
                }
            }
     
            page.Controls.Add(store);

            return store;
        }

        /// <summary>
        /// Добавление полей в стор из перечисления
        /// </summary>
        /// <returns> стор с полями </returns>
        public static Store AddFieldsByEnum(this Store store, Enum fieldsEnum)
        {
            var reader = (JsonReader)store.Reader.Reader;
            reader.Fields.Clear();

            foreach (var field in Enum.GetNames(fieldsEnum.GetType()))
            {
                reader.Fields.Add(field);
            }

            return store;
        }

        /// <summary>
        /// Добавление полей в стор из свойств класса
        /// </summary>
        /// <returns> стор с полями </returns>
        public static Store AddFieldsByClass(this Store store, ViewModelBase viewModel)
        {
            foreach (var info in viewModel.GetType().GetProperties())
            {
                store.AddField(info.Name);
            }

            return store;
        }

        /// <summary>
        /// Добавление полей в стор из свойств класса
        /// </summary>
        /// <returns> стор с полями </returns>
        public static Store AddFieldsByClass<T>(this Store store, T viewModel)
        {
            foreach (var info in viewModel.GetType().GetProperties())
            {
                store.AddField(info.Name);
            }

            return store;
        }
        
        /// <summary>
        /// Добавление поля с значением по умолчанию
        /// </summary>
        public static Store AddField(this Store store, string name, string defaultValue)
        {
            var cfg = new RecordField.Config
                          {
                            DefaultValue = defaultValue
                          };
            store.AddField(name, cfg);
            return store;
        }
    }
}

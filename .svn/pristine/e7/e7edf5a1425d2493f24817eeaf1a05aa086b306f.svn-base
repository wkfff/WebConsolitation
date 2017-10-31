using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Utils
{
    public static class CUtils
    {
        /// <summary>
        ///   Добавление лукаповского поля
        /// </summary>
        /// <param name="id"> Идентификатор поля </param>
        /// <param name="label"> Заголовок окна </param>
        /// <param name="dataIndex"> Отображаемое поле </param>
        /// <param name="entity"> Ключь таблицы </param>
        /// <returns> Возвращает триггер филд </returns>
        public static Component LookUpFld(string id, string label, string dataIndex, string entity)
        {
            return LookUpFld(id, label, dataIndex, entity, false, string.Empty);
        }

        public static Component LookUpFld(string id, string label, string dataIndex, string entity, bool allowblank)
        {
            return LookUpFld(id, label, dataIndex, entity, allowblank, string.Empty);
        }

        public static Component LookUpFld(string id, string label, string dataIndex, string entity, bool allowblank, string filter)
        {
            string handler = "showHandBook('{0}', '{1}'".FormatWith(id, entity);
            if (filter.Length > 0)
            {
                handler += ", {0}".FormatWith(filter);
            }

            handler += ");";

            string triggerHandler = @" switch (index) {{
                                        case 0:
                                            this.triggers[0].hide();
                                            IDFld = Ext.getCmp('{1}'.substring(0, '{1}'.length - 4));
                                            {1}.setValue('');
                                            IDFld.setValue(''); 
                                            break;
                                        case 1:
                                            {0}
                                            this.triggers[0].show();  
                                            break;}};".FormatWith(handler, id);

            Component lookupFld = new TriggerField
                {
                    ID = id, 
                    Editable = false, 
                    FieldLabel = label, 
                    DataIndex = dataIndex, 
                    AllowBlank = allowblank, 
                    Triggers =
                        {
                            new FieldTrigger { Icon = TriggerIcon.Clear, Qtip = "Очистить", HideTrigger = true }, 
                            new FieldTrigger { Icon = TriggerIcon.Ellipsis, Qtip = "Редактировать" }
                        }, 
                    Listeners =
                        {
                            TriggerClick = { Handler = triggerHandler }
                        }
                };

            return lookupFld;
        }

        /// <summary>
        ///   Создает стор для лукапов
        /// </summary>
        /// <param name="id"> Айдишник стора </param>
        /// <param name="controller"> контроллер загрузки стора </param>
        /// <param name="idField"> Имя колонки с айдишниками </param>
        /// <param name="nameField"> Имя колонки с именами </param>
        /// <returns> Возвращается настроенный стор </returns>
        public static Store GetLookupStore(string id, string controller, string idField, string nameField)
        {
            var store = new Store { ID = id, AutoLoad = false };
            store.SetHttpProxy(controller);
            store.SetJsonReader();
            store.AddField(idField);
            store.AddField(nameField);
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке лукапа', response.responseText);";
            return store;
        }

        /// <summary>
        ///   Создает стор для лукапов с произвольным списком полей
        /// </summary>
        /// <param name="id"> Айдишник стора </param>
        /// <param name="controller"> контроллер загрузки стора </param>
        /// <param name="idField"> поле возвращаемого значения </param>
        /// <param name="nameField"> поле расшифровки вариантов выбора </param>
        /// <param name="extraFields"> список дополнительных полей </param>
        /// <returns> Возвращается настроенный стор </returns>
        public static Store GetLookupStore(string id, string controller, string idField, string nameField, IList<string> extraFields)
        {
            var store = GetLookupStore(id, controller, "ID", "Name");
            foreach (var extraField in extraFields)
            {
                store.AddField(extraField);
            }

            return store;
        }

        public static Store GetLookupStore(string id, string controller)
        {
            return GetLookupStore(id, controller, "ID", "Name");
        }

        /// <summary>
        ///   Делает колонку лукаповской с вызовом справочника
        /// </summary>
        public static ColumnBase SetHbLookup(this ColumnBase column, string entytyKey)
        {
            return SetHbLookup(column, entytyKey, string.Empty, string.Empty);
        }

        public static ColumnBase SetHbLookup(this ColumnBase column, string entytyKey, string filter)
        {
            return SetHbLookup(column, entytyKey, filter, string.Empty);
        }

        public static ColumnBase SetHbLookup(this ColumnBase column, string entytyKey, string filter, string fn)
        {
            if (column != null)
            {
                column.Editable = true;

                string handler = "showHandBookGrid('{0}','{1}','{2}','{3}'".FormatWith(column.ParentGrid.ID, column.ColumnID, entytyKey, column.Header);

                if (fn.Length > 0)
                {
                    handler += ", {0}".FormatWith(fn);
                }
                else
                {
                    handler += ", SelectRowGrid";
                }

                if (filter.Length > 0)
                {
                    handler += ", {0}".FormatWith(filter);
                }

                handler += ");";

                column.Editor.Add(
                    new TriggerField
                        {
                            ID = column.ColumnID,
                            Editable = false,
                            AllowBlank = column.CustomConfig.Contains("AllowBlank"),
                            TriggerIcon = TriggerIcon.Ellipsis,
                            Listeners = { TriggerClick = { Handler = handler } } 
                        });
            }

            return column;
        }

        /// <summary>
        ///   Получение маски поля из "дизайнера". Почему тоне везде работает!
        /// </summary>
        public static string GetMaskByTableField(string tableKey, string maskField)
        {
            try
            {
                var scheme = Resolver.Get<IScheme>();
                IEntity entity = scheme.RootPackage.FindEntityByName(tableKey);
                IDataAttribute field = entity.Attributes.Values.ToList().Find(x => x.Name == maskField);
                return field.Mask;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }

    /// <summary>
    ///   Заготовка для Рест контроллера
    /// </summary>
    /// <typeparam name="T"> Главная таблица </typeparam>
    public abstract class RestBaseController<T> : SchemeBoundController
    {
        public ILinqRepository<T> TableRepository { get; set; }

        public T RecordDataUpdate { get; set; }

        public JsonObject JoDataUpdate { get; set; }

        public abstract void ConfigUpdates();

        [HttpPost]
        [ValidateInput(false)]
        [Transaction]
        public virtual RestResult Update(int id, string data)
        {
            try
            {
                JoDataUpdate = JSON.Deserialize<JsonObject>(data);

                RecordDataUpdate = TableRepository.FindOne(id);

                ConfigUpdates();

                TableRepository.DbContext.CommitChanges();

                return new RestResult { Success = true, Message = "Запись обновлена" };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpDelete]
        [Transaction]
        public virtual RestResult Destroy(int id)
        {
            try
            {
                var formData = TableRepository.FindOne(id);
                TableRepository.Delete(formData);
                TableRepository.DbContext.CommitChanges();

                return new RestResult { Success = true, Message = "Запись удалена" };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models;
using Krista.FM.RIA.Extensions.E86N.Utils;
using Krista.FM.ServerLibrary;

using LinqKit;

using Component = Ext.Net.Component;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controls
{
    public static class UiBuilders
    {
        #region UI templates

        public static Component GetTabbedDetails(IEnumerable<Component> details)
        {
            var tabPanel =
                new TabPanel
                    {
                        Border = false,
                        EnableTabScroll = true
                };

            foreach (Component detail in details)
            {
                tabPanel.Add(detail);
            }

            return tabPanel;
        }

        /// <summary>
        /// Создает грид по заданной модели.
        /// </summary>
        public static GridPanel CreateGridPanel(string id, Store store, ViewModelBase model, bool readOnly = false)
        {
            var gp = CreateGridPanel(id, store);
            var gridFilters = new GridFilters { Local = false };
            model.GetType().GetProperties().Where(x => !x.Name.Equals("ID")).ForEach(
                info =>
                {
                    gp.ColumnModel.AddColumn(info, readOnly);
                    gridFilters.Filters.Add(FilterFactory(info));
                });

            gp.Plugins.Add(gridFilters);
            return gp;
        }

        /// <summary>
        /// Создает грид отображающий данные из стора.
        /// </summary>
        public static GridPanel CreateGridPanel(string id, Store store)
        {
            var table = new GridPanel
                {
                    ID = id, 
                    StoreID = store.ID, 
                    Border = false, 
                    TrackMouseOver = true, 
                    AutoScroll = true, 
                    Icon = Icon.DatabaseTable, 
                    LoadMask =
                        {
                            ShowMask = true, 
                            Msg = "Загрузка..."
                        }, 
                    Layout = LayoutType.Fit.ToString(), 
                    ClearEditorFilter = false, 
                    StripeRows = true
                };

            table.View.Add(new GridView { ForceFit = true });
            var selMod = new RowSelectionModel { SingleSelect = true };
            table.SelectionModel.Add(selMod);
            table.ColumnModel.AddColumn("ID", "ID", DataAttributeTypes.dtInteger).SetHidden(true);
            return table;
        }

        /// <summary>
        /// Фабрика фильтров
        /// </summary>
        public static GridFilter FilterFactory(PropertyInfo info)
        {
            if (info.PropertyType == typeof(double) || info.PropertyType == typeof(decimal) || info.PropertyType == typeof(int)
                || info.PropertyType == typeof(double?) || info.PropertyType == typeof(decimal?) || info.PropertyType == typeof(int?))
            {
                return new NumericFilter { DataIndex = info.Name };
            }

            if (info.PropertyType == typeof(DateTime) || info.PropertyType == typeof(DateTime?))
            {
                return new DateFilter { DataIndex = info.Name };
            }

            if (info.PropertyType == typeof(bool) || info.PropertyType == typeof(bool?))
            {
                return new BooleanFilter { DataIndex = info.Name };
            }

            return new StringFilter { DataIndex = info.Name };
        }

        #endregion

        #region Stores
        
        /// <summary>  Получение описания свойства </summary>
        /// <typeparam name="T"> Несущественный параметр </typeparam>
        /// <example> DescriptionOf(() => consumerModel.RefCPotrName) </example>
        /// <returns> Значение атрибута свойства </returns>
        public static string DescriptionOf<T>(Expression<Func<T>> expr)
        {
            var descriptionAttribute = ((MemberExpression)expr.Body).Member.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (descriptionAttribute.Length != 0)
            {
                var attribute = descriptionAttribute[0] as DescriptionAttribute;
                if (attribute != null)
                {
                    return attribute.Description;
                }
            }

            return SchemeDescriptionOf(expr);
        }

        /// <summary> Получение описания свойства из методанных </summary>
        /// <typeparam name="T"> Несущественный параметр </typeparam>
        /// <returns> Значение атрибута Description или если Description пуст то Caption из методанных </returns>
        public static string SchemeDescriptionOf<T>(Expression<Func<T>> expr)
        {
            return SchemeOf(expr) == null ? string.Empty : SchemeOf(expr).Description ?? SchemeOf(expr).Caption;
        }

        /// <summary>  Получение методанных для поля</summary>
        /// <typeparam name="T"> Несущественный параметр </typeparam>
        /// <returns> Значение атрибута поля </returns>
        public static IDataAttribute SchemeOf<T>(Expression<Func<T>> expr)
        {
            return GetDataAttribute(((MemberExpression)expr.Body).Member);
        }
        
        /// <summary>  Получение методанных для поля модели </summary>
        /// <returns> Значение атрибута поля </returns>
        public static IDataAttribute GetDataAttribute(MemberInfo memberInfo)
        {
            var attributes = memberInfo.GetCustomAttributes(typeof(DataBaseBindingFieldAttribute), false);
            if (attributes.Any())
            {
                return ((DataBaseBindingFieldAttribute)attributes[0]).Info;
            }

            attributes = memberInfo.GetCustomAttributes(typeof(DataBaseBindingTableAttribute), false);
            if (attributes.Any())
            {
                return ((DataBaseBindingTableAttribute)attributes[0]).GetInfo(memberInfo.Name);
            }

            return null;
        }

        /// <summary>
        /// Возвращает ID описания, если это описание содержится в БД
        /// </summary>
        /// <typeparam name="T"> Несущественный параметр </typeparam>
        /// <example> DescriptionIdOf(() => consumerModel.RefCPotrName) </example>
        /// <returns> Значение поля ID атрибута свойства </returns>
        public static int DescriptionIdOf<T>(Expression<Func<T>> expr)
        {
            var descriptionAttribute = ((MemberExpression)expr.Body).Member.GetCustomAttributes(typeof(DataBaseDescriptionAttribute), false);
            if (descriptionAttribute.Length == 0)
            {
                throw new ArgumentException("DataBaseDescriptionAttribute is not set.");
            }

            var dataBaseDescriptionAttribute = descriptionAttribute[0] as DataBaseDescriptionAttribute;
            if (dataBaseDescriptionAttribute != null)
            {
                return dataBaseDescriptionAttribute.ID;
            }

            throw new Exception("Атрибут DataBaseDescriptionAttribute не найден");
        }

        /// <summary>  Получение имени свойства </summary>
        /// <typeparam name="T"> Несущественный параметр </typeparam>
        /// <example> NameOf(() => consumerModel.RefCPotrName) </example>
        /// <returns> Имя указанного свойства </returns>
        public static string NameOf<T>(Expression<Func<T>> expr)
        {
            var constantExpression = expr.Body as ConstantExpression;
            if (constantExpression != null)
            {
                return constantExpression.Value.ToString();
            }

            return ((MemberExpression)expr.Body).Member.Name;
        }

        /// <summary>
        /// Получение идентификатора контроллера
        /// </summary>
        /// <example> GetContrillerID( typeof(ServicesController) ) </example>
        /// <exception cref="ArgumentException"> Если в имени контроллера нет подстраки Controller </exception>
        /// <returns> Идентификатор класса контроллера </returns>
        public static string GetControllerID(Type type)
        {
            return type.Name.Replace("Controller", string.Empty);
        }

        /// <summary>
        /// Получение идентификатора контроллера
        /// </summary>
        /// <typeparam name="T">Тип контроллера, чьё имя необходимо получить</typeparam>
        /// <returns>Имя контроллера</returns>
        public static string GetControllerID<T>() where T : Controller
        {
            return GetControllerID(typeof(T));
        }

        /// <summary>
        /// Формирование URL адреса конкретной функции
        /// </summary>
        /// <typeparam name="T">Контроллер в котором находится функция</typeparam>
        /// <param name="nameOfFunction">Имя функции</param>
        /// <param name="param">Параметры, необходимые для работы</param>
        /// <exception> Если в классе-контроллере нет указанной функции. </exception>
        /// <returns>URL адрес</returns>
        /// <example>UiBuilders.GetUrl<EntityController>("DataWithServerFilter", new Dictionary<string, object> { { "objectKey", D_Services_Platnost.Key } })</example>
        public static string GetUrl<T>(string nameOfFunction, Dictionary<string, object> param = null) where T : Controller
        {
            string urlParam = string.Empty;
            if (typeof(T).GetMethod(nameOfFunction) == null)
            {
                throw new ArgumentException("Не найден метод действия \" {0}\" контроллера \" {1}\".".FormatWith(nameOfFunction, typeof(T).Name));
            }

            if (param != null)
            {
                urlParam = "?";
                foreach (KeyValuePair<string, object> info in param)
                {
                    urlParam += "{0}={1}&".FormatWith(info.Key, info.Value);
                }

                urlParam = urlParam.Remove(urlParam.Length - 1);
            }

            return "/{0}/{1}{2}".FormatWith(GetControllerID<T>(), nameOfFunction, urlParam);
        }

        #endregion

        #region Special
        
        /// <summary>
        ///   Построение интерфейса документа с пользовательским тулбаром(настраиваемой системой состояний)
        /// </summary>
        public static BorderLayout RenderBasicDocumentLayout(
            ViewPage page, 
            string documentId, 
            IList<Component> details, 
            Toolbar toolBar = null)
        {
            details.Add(new DocsDetailControl(Convert.ToInt32(documentId)).BuildComponent(page));

            return
                new BorderLayout
                    {
                        North =
                            {
                                Items =
                                    {
                                         new ParamDocPanelControl(Convert.ToInt32(documentId), toolBar).BuildComponent(page)
                                    }
                            }, 
                        Center =
                            {
                                Items =
                                    {
                                        GetTabbedDetails(details)
                                    }
                            }
                    };
        }

        #endregion
    }
}

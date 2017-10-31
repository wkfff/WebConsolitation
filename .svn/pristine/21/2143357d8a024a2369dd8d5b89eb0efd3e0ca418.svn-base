using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.Abstract
{
    // todo переделать все модели на это решение

    /// <summary>
    /// Базовый класс для моделей данных
    /// </summary>
    public abstract class AbstractModelBase<TModel, TDomain> where TDomain : DomainObject where TModel : AbstractModelBase<TModel, TDomain>, new()
    {
        /// <summary>
        /// Идентификатор сущности
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Ссылка для мастер-детальных сущностей
        /// </summary>
        public int? RefParent { get; set; }

        /// <summary>
        /// Получение сервайся для работы с БД
        /// </summary>
        public INewRestService GetNewRestService()
        {
            return Resolver.Get<INewRestService>();
        }
        
        /// <summary>  Получение описания свойства </summary>
        /// <typeparam name="T"> Несущественный параметр </typeparam>
        /// <example> DescriptionOf(() => consumerModel.RefCPotrName) </example>
        /// <returns> Значение атрибута свойства </returns>
        public string DescriptionOf<T>(Expression<Func<T>> expr)
        {
            return UiBuilders.DescriptionOf(expr);
        }

        /// <summary> Получение описания свойства</summary>
        /// <param name="field"> Имя свойства</param>
        /// <returns> Значение атрибута свойства</returns>
        public string DescriptionOf(string field)
        {
            var prop = GetType().GetProperty(field);
            if (prop != null)
            {
                if (prop.GetCustomAttributes(typeof(DescriptionAttribute), false).Length > 0)
                {
                    return (prop.GetCustomAttributes(typeof(DescriptionAttribute), false)[0] as DescriptionAttribute).Description;
                }

                if (prop.GetCustomAttributes(typeof(DataBaseBindingFieldAttribute), false).Length > 0)
                {
                    return (prop.GetCustomAttributes(typeof(DataBaseBindingFieldAttribute), false)[0] as DataBaseBindingFieldAttribute).Info.Caption;
                }
            }

            return string.Empty;
        }

        /// <summary>  Получение описания из методанных </summary>
        /// <typeparam name="T"> Несущественный параметр </typeparam>
        /// <example> DescriptionOf(() => consumerModel.RefCPotrName) </example>
        /// <returns> Значение атрибута свойства </returns>
        public string DescriptionFromSchemeOf<T>(Expression<Func<T>> expr)
        {
            return UiBuilders.SchemeDescriptionOf(expr);
        }

        /// <summary>  Получение имени свойства </summary>
        /// <typeparam name="T"> Несущественный параметр </typeparam>
        /// <example> NameOf(() => consumerModel.RefCPotrName) </example>
        /// <returns> Имя указанного свойства </returns>
        public string NameOf<T>(Expression<Func<T>> expr)
        {
            return UiBuilders.NameOf(expr);
        }

        /// <summary>
        /// Метод валидации данных
        /// </summary>
        public abstract string ValidateData();
        
        /// <summary>
        /// Метод преобразования DomainObject в модель
        /// </summary>
        public abstract TModel GetModelByDomain(TDomain domain);

        /// <summary>
        /// Метод преобразования модели в DomainObject
        /// </summary>
        public abstract TDomain GetDomainByModel();
        
        /// <summary>
        /// Десериализатор модели
        /// </summary>
        public TModel Serialize(string data)
        {
            return JavaScriptDomainConverter<TModel>.DeserializeSingle(data);
        }

        /// <summary>
        /// Метод получения данных для модели
        /// </summary>
        public abstract IQueryable<TModel> GetModelData(NameValueCollection paramsList);
    }
}
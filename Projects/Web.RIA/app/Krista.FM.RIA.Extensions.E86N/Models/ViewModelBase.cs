using System;
using System.ComponentModel;
using System.Linq.Expressions;

using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models
{
    /// <summary>
    /// Базовый класс для моделей данных
    /// </summary>
    public class ViewModelBase
    {
        /// <summary>
        /// Получение сервайся для работы с БД
        /// </summary>
        public static INewRestService GetNewRestService()
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
            var prop = this.GetType().GetProperty(field);
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
        public virtual string ValidateData(int docId)
        {
            throw new NotImplementedException("ViewModelBase: Метод валидации данных");
        }

        // todo переделать все модели с таким методом

        /// <summary>
        /// Метод преобразования домена в модель
        /// </summary>
        public virtual ViewModelBase GetModelByDomain(DomainObject domain)
        {
            throw new NotImplementedException("ViewModelBase: Метод преобразования домена в модель");
        }
    }
}
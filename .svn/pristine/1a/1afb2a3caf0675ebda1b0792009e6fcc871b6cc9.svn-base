using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Krista.FM.Domain;
using Krista.FM.ServerLibrary;
using LinqKit;

namespace Krista.FM.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        ///   Накладывает ограничение(выполняет поиск) строки запроса во всех переданных полях
        /// </summary>
        /// <typeparam name="T"> шаблонный тип данных as DomainObject </typeparam>
        /// <param name="data"> исходная выборка </param>
        /// <param name="query"> строка ограничения(поиска) </param>
        /// <param name="fields"> поля ограничения(поиска) </param>
        /// <param name="entity"> описание данных </param>
        /// <returns> если строка ограничения не пустая возвращает ограниченую выборку, иначе исходную </returns>
        /// <see cref="Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.Entity.EntityExtController.DataWithCustomSearch" />
        public static IQueryable<T> ApplyQuery<T>(
            this IQueryable<T> data, 
            string query, 
            IEnumerable<string> fields, 
            IEntity entity) where T : DomainObject
        {
            if (query.IsNotNullOrEmpty())
            {
                ParameterExpression target = Expression.Parameter(typeof(T), "target");
                Expression<Func<T, bool>> predicate = fields
                    .Where(s => entity.Attributes.Values.Any(attribute => attribute.Name.Equals(s)))
                    .Where(s => !s.Equals("ID", StringComparison.OrdinalIgnoreCase))
                    .Aggregate(
                        PredicateBuilder.False<T>(), 
                        (current, field) => current.Or(
                            Expression.Lambda<Func<T, bool>>(
                                Expression.Call(
                                    Expression.Call(Expression.Lambda(Expression.PropertyOrField(target, field), target), "ToString", null), 
                                    typeof(string).GetMethod("Contains", new[] { typeof(string) }), 
                                    Expression.Constant(query, typeof(string))), 
                                target)));

                return data.AsExpandable().Where(predicate);
            }

            return data;
        }
    }
}

using System;

using NHibernate.Cfg;

namespace Krista.FM.Domain.Reporitory.NHibernate.ConfigurationCache
{
    /// <summary>
    /// Кеш конфигурации мапинга NHibernate.
    /// </summary>
    public interface IConfigurationCache
    {
        /// <summary>
        /// Время последнего обновления кеша.
        /// </summary>
        DateTime GetTimeStamp();

        /// <summary>
        /// Возвращает конфигурации мапинга NHibernate из кеша.
        /// </summary>
        /// <returns>Конфигурации мапинга NHibernate.</returns>
        Configuration Get();

        /// <summary>
        /// Сохраняет в кеше конфигурацию мапинга NHibernate.
        /// </summary>
        /// <param name="configuration">Конфигурация мапинга NHibernate.</param>
        void Set(Configuration configuration);
    }
}
using System;

using NHibernate.Cfg;

namespace Krista.FM.Domain.Reporitory.NHibernate.ConfigurationCache
{
    /// <summary>
    /// Реализация пустого кеша.
    /// </summary>
    public class NullConfigurationCache : IConfigurationCache
    {
        public DateTime GetTimeStamp()
        {
            return DateTime.MinValue;
        }

        /// <summary>
        /// Этот метод не должен вызываться.
        /// </summary>
        /// <exception cref="T:System.NotImplementedException"></exception>
        public Configuration Get()
        {
            throw new NotImplementedException();
        }

        public void Set(Configuration configuration)
        {
        }
    }
}
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Krista.FM.Domain.Reporitory.NHibernate.ConfigurationCache;

using NHibernate.Cfg;

namespace Krista.FM.RIA.Core.NHibernate
{
    /// <summary>
    /// Реализация кеша конфигурации NHibernate во временном файле.
    /// </summary>
    public class WebConfigurationCache : IConfigurationCache
    {
        internal static readonly string SerializedConfigurationFile = "nhConfiguration.cache";

        private readonly string filePath;

        public WebConfigurationCache(string dir)
        {
            filePath = Path.Combine(dir, SerializedConfigurationFile);
        }

        public DateTime GetTimeStamp()
        {
            return new FileInfo(filePath).LastWriteTime;
        }

        public Configuration Get()
        {
            using (FileStream file = File.Open(filePath, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (Configuration)bf.Deserialize(file);
            }
        }

        public void Set(Configuration configuration)
        {
            using (FileStream file = File.Open(filePath, FileMode.Create))
            {
                IFormatter bf = new BinaryFormatter();
                bf.Serialize(file, configuration);
            }
        }
    }
}
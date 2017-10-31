using System.Collections.Generic;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Reports
{
    public static class ConvertorSchemeLink
    {
        private static readonly Dictionary<string, IEntity> cacheEntity = new Dictionary<string, IEntity>();
        public static IScheme scheme;

        public static IEntity GetEntity(string key)
        {
            if (!cacheEntity.ContainsKey(key))
            {
                var entity = scheme.RootPackage.FindEntityByName(key);
                cacheEntity.Add(key, entity);
            }

            return cacheEntity[key];
        }
    }
}

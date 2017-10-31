using System;
using System.Collections.Generic;
using System.Reflection;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SmoObjectsCache
    {
        private static readonly Dictionary<int, ServerManagedObjectAbstract> cache;

        static SmoObjectsCache()
        {
            cache = new Dictionary<int, ServerManagedObjectAbstract>(10000);
        }

        /// <summary>
        /// Извлекает объект их кеша клиентских SMO-объектов.
        /// Если объекта к кеше нет, то он создается и сохраняется к кеше.
        /// </summary>
        /// <param name="type">Тип объекта, используется для создания нового экземпляра SMO-объекта.</param>
        /// <param name="objectData">Закешированные данныен объекта.</param>
        /// <returns></returns>
        public static ServerManagedObjectAbstract GetSmoObject(Type type, SMOSerializationInfo objectData)
        {
            int identifier = Convert.ToInt32(objectData["Identifier"]);
            if (cache.ContainsKey(identifier))
            {
                // берем объект из кеша
                return cache[identifier];
            }
            else
            {
                // создаем соответствующий объект и помещяем в кеш
                ServerManagedObjectAbstract smoObject =
                    (ServerManagedObjectAbstract)type.Assembly.CreateInstance(
                        type.FullName, false, BindingFlags.CreateInstance, null,
                        new object[] { objectData }, null, null);

                cache.Add(identifier, smoObject);
                return smoObject;
            }
        }

        /// <summary>
        /// Извлекает объект их кеша клиентских SMO-объектов.
        /// Если объекта к кеше нет, то он создается и сохраняется к кеше.
        /// </summary>
        /// <param name="type">Тип объекта, используется для создания нового экземпляра SMO-объекта.</param>
        /// <param name="serverObject">Ссылка на серверный объект.</param>
        /// <returns></returns>
        public static ServerManagedObjectAbstract GetSmoObject(Type type, ISMOSerializable serverObject)
        {
            int identifier = serverObject.Identifier;
            if (cache.ContainsKey(identifier))
            {
                // берем объект из кеша
                return cache[identifier];
            }
            else
            {
                // создаем соответствующий объект и помещяем в кеш
                ServerManagedObjectAbstract smoObject =
                    (ServerManagedObjectAbstract)type.Assembly.CreateInstance(
                        type.FullName, false, BindingFlags.CreateInstance, null,
                        new object[] { serverObject.GetSMOObjectData() }, null, null);

                cache.Add(identifier, smoObject);
                return smoObject;
            }
        }
    }
}

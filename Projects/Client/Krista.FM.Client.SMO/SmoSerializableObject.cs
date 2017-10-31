using System;
using System.ComponentModel;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SMOSerializableObject<T> : SmoServerSideObject<T>, ISMOSerializable where T : ISMOSerializable
    {
        /// <summary>
        /// Определяет способ получения значений свойств объекта: 
        /// false - значения получаются от объекта расположенного на сервере,
        /// true - значения получаются из кеш копии серверного объекта расположенного на клиенте
        /// </summary>
        protected bool cached = false;

        /// <summary>
        /// Кеш значений [Имя свойства, Значение].
        /// Каждый объект представляется в виде коллекции, первые два элемента - это ссылка на 
        /// серверный объект "this" и идентификационный номер серверного объекта "Identifier".
        /// Далее следуют остальные свойства объекта. 
        /// </summary>
        private SMOSerializationInfo cachedValues;

        public SMOSerializableObject(T serverObject)
            : base(serverObject)
        {
        }

        public SMOSerializableObject(T serverControl, bool cached)
            : this(serverControl)
        {
            this.serverControl = serverControl;
            this.cached = cached;

            if (cached)
            {
                // Сериализуем серверный объект и создаем его копию на клиенте
                cachedValues = serverControl.GetSMOObjectData();
            }
        }

        public SMOSerializableObject(SMOSerializationInfo cache)
            : this((T)cache["this"])
        {
            cached = true;
            cachedValues = cache;
        }
        /// <summary>
        /// Возвращает значение свойства из кеша.
        /// </summary>
        /// <param name="name">Имя свойства.</param>
        protected object GetCachedValue(string name)
        {
            if (cachedValues[name] is Exception)
                throw (Exception) cachedValues[name];

            return cachedValues[name];
        }

        /// <summary>
        /// Возвращает объект из кеша.
        /// </summary>
        /// <param name="name">Имя свойства, которое возвращает объект.</param>
        /// <returns></returns>
        protected SMOSerializationInfo GetCachedObject(string name)
        {
            if (cachedValues[name] is Exception)
                throw (Exception)cachedValues[name];

            return (SMOSerializationInfo)cachedValues[name];
        }

        /// <summary>
        /// Кеш значений [Имя свойства, Значение].
        /// Каждый объект представляется в виде коллекции, первые два элемента - это ссылка на 
        /// серверный объект "this" и идентификационный номер серверного объекта "Identifier".
        /// Далее следуют остальные свойства объекта. 
        /// </summary>
		public SMOSerializationInfo CachedValues
        {
            get { return cachedValues; }
            set { cachedValues = value; }
        }

        public SMOSerializationInfo GetSMOObjectData()
        {
            return cachedValues;
        }

        #region ISMOSerializable Members

        public SMOSerializationInfo GetSMOObjectData(LevelSerialization level)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override string ToString()
        {
            return String.Format("SMO identifier - ({0}) : Server identifier - ({1}) : {2}", identifier, serverControl.Identifier, base.ToString());
        }
    }
}

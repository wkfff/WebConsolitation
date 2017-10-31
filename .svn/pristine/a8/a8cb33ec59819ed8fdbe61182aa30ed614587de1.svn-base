using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Krista.FM.RIA.Extensions.E86N.Services.Params
{
    /// <summary>
    /// Коллекция параметров
    /// </summary>
    [DataContract]
    public class ParamsMap : IParamsMap
    {
        public ParamsMap()
        {
            ParamsMapDictionary = new Dictionary<string, object>();
        }

        [DataMember]
        public Dictionary<string, object> ParamsMapDictionary { get; private set; }

        /// <summary>
        /// Добавить параметр
        /// </summary>
        /// <param name="key"> Ключ параметра</param>
        /// <param name="value"> Заначение параметра </param>
        /// <returns> Коллекция параметров </returns>
        public ParamsMap SetParam(string key, object value)
        {
            if (!HasParam(key))
            {
                ParamsMapDictionary.Add(key, value);
            }
            else
            {
                ParamsMapDictionary[key] = value;    
            }
            
            return this;
        }

        /// <summary>
        /// Получить параметр по ключу
        /// </summary>
        /// <typeparam name="T"> тип параметра </typeparam>
        /// <param name="key"> ключ параметра</param>
        /// <returns> параметр заданого типа</returns>
        public T GetParam<T>(string key)
        {
            return (T)ParamsMapDictionary[key];
        }

        /// <summary>
        /// Проверка на наличие параметра в коллекции
        /// </summary>
        /// <param name="key"> ключ значения</param>
        /// <returns> true - ключ присутствует в коллекции, иначе - false</returns>
        public bool HasParam(string key)
        {
            return ParamsMapDictionary.ContainsKey(key);
        }
    }
}

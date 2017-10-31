using Krista.FM.ServerLibrary;

namespace Krista.FM.Common
{
    public class DataAttributeHelper
    {
        /// <summary>
        /// Возвращает аттрибут из коллекции по ключу или по паименованию.
        /// </summary>
        /// <param name="collection">Коллекция атрибутов.</param>
        /// <param name="key">Уникальный ключ объекта.</param>
        /// <param name="name">Наименование объекта.</param>
        public static IDataAttribute GetAttributeByKeyName(IDataAttributeCollection collection, string key, string name)
        {
            if (collection.ContainsKey(key))
            {
                return collection[key];
            }
            else
            {
                return GetByName(collection, name);
            }
        }

        /// <summary>
        /// Возвращает аттрибут из коллекции по ключу или по паименованию.
        /// </summary>
        /// <param name="collection">Коллекция атрибутов.</param>
        /// <param name="name">Наименование объекта.</param>
        public static IDataAttribute GetByName(IDataAttributeCollection collection, string name)
        {
            IDataAttribute attr = null;
            foreach (IDataAttribute item in collection.Values)
            {
                if (item.Name == name)
                {
                    attr = item;
                    break;
                }
            }
            return attr;
        }
    }
}

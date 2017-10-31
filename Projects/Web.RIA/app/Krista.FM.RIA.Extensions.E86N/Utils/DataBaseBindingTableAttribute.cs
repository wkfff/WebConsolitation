using System;

using Krista.FM.RIA.Core;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Utils
{
    /// <summary>
    /// Атрибут связывает поле модели с таблицей БД.
    /// Используется для настройки полей. Насройки берутся из методанных.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DataBaseBindingTableAttribute : Attribute
    {
        /// <summary>
        /// Данный вариант в качестве наименования поля подразумевает нименование своства к которому применен атрибут
        /// </summary>
        public DataBaseBindingTableAttribute(Type table)
        {
            Key = table.GetField("Key").GetValue(null).ToString();
        }

        public string Key { get; set; }
        
        public IDataAttribute GetInfo(string field)
        {
            using (new ServerContext())
            {
               return Resolver.Get<IScheme>().RootPackage.FindEntityByName(Key).Attributes[field];
            }
        }
    }
}

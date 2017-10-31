using System;

using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Utils
{
    /// <summary>
    /// Атрибут связывает поле модели с полем в таблице БД.
    /// Используется для настройки поля. Насройки берутся из методанных.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DataBaseBindingFieldAttribute : DataBaseBindingTableAttribute
    {
        public DataBaseBindingFieldAttribute(Type table, string field)
            : base(table)
        {
            Field = field;
        }

        public string Field { get; set; }

        public IDataAttribute Info
        {
            get
            {
                if (Field.IsNotNullOrEmpty())
                {
                    return GetInfo(Field);
                }

                return null;
            }
        }
    }
}

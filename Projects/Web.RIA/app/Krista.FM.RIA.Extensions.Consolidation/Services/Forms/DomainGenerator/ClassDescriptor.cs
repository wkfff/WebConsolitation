using System;
using System.Collections.Generic;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DomainGenerator
{
    /// <summary>
    /// Дескриптор доменного объекта таблиц формы сбора.
    /// </summary>
    public abstract class ClassDescriptor
    {
        public ClassDescriptor()
        {
            Properties = new List<PropertyDescriptor>();
        }

        /// <summary>
        /// Полное наименование таблицы в базе данных.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Базовый тип от которого должен быть унаследован класс.
        /// </summary>
        public abstract Type BaseType { get; }

        /// <summary>
        /// Свойства/поля объекта
        /// </summary>
        public List<PropertyDescriptor> Properties { get; set; }
    }
}

using System;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DomainGenerator
{
    /// <summary>
    /// Дескриптор свойства класса.
    /// </summary>
    public class PropertyDescriptor
    {
        /// <summary>
        /// Английский идентификатор поля.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Тип хранимого поля.
        /// </summary>
        public Type DataType { get; set; }
        
        /// <summary>
        /// Должен иметь значение или может быть пустым.
        /// </summary>
        public bool Required { get; set; }
    }
}


namespace Krista.FM.RIA.Extensions.Consolidation.Models
{
    public class TaskProtocolDetailViewModel
    {
        /// <summary>
        /// Виды записей в детализированном протоколе
        /// </summary>
        public enum ProtocolDetailAttributeType : int
        {
            /// <summary>
            /// Значение не указано
            /// </summary>
            Undefined = -1,

            /// <summary>
            /// Запись является изменением значения Атрибута
            /// </summary>
            Attibute = 0,

            /// <summary>
            /// Запись является изменением изменением Файла
            /// </summary>
            File = 1
        }

        public int ID { get; set; }

        public string Attribute { get; set; }

        public int AttributeType { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }
    }
}

using System.ComponentModel;

namespace Krista.FM.RIA.Extensions.E86N.Utils
{
    /* Существует ситуация когда описания полей хранятся в БД в тиблицах типа FX_FX_StateValue.
     * И соответственно каждое описание имеет ID
     * Базируясь на предположении что в таблице с префиксом FX никогда ничего не поменяется вынес описания вместе с их ID в этот атрибут.
     * Далее при создании записей беру этот ID методом DescriptionIdOf
    */
    public class DataBaseDescriptionAttribute : DescriptionAttribute
    {
        public DataBaseDescriptionAttribute(string description, int id) : base(description)
        {
            ID = id;
        }

        public int ID { get; private set; }
    }
}

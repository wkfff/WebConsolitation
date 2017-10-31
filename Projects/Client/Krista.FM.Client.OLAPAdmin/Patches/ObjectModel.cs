namespace Krista.FM.Client.OLAPAdmin
{
    public interface ICommonObject
    {
        /// <summary>
        /// Уникальный идентификатор объекта
        /// </summary>
        string ID { get; set;}
        /// <summary>
        /// Имя объекта
        /// </summary>
        string Name { get; set;}
    }
    /// <summary>
    /// Объект патча
    /// </summary>
    public interface IBaseObjForScript
    {
        /// <summary>
        /// Имя многомерной базы
        /// </summary>
        string Database { get; set;}
        /// <summary>
        /// Метод обновления объекта
        /// </summary>
        string Method { get; set;}
        /// <summary>
        /// Описание объекта
        /// </summary>
        string Description { get; set;}
    }

    public class BaseAMOObject : ICommonObject, IBaseObjForScript
    {
        public string FullName { get; set; }

        public string Name { get; set; }

        #region IBaseObjForScript Members

        public string Description { get; set; }

        public string ID { get; set; }

        #endregion

        public string Method { get; set; }

        public string Database { get; set; }

        public string DsvName { get; set; }

        public string QueryDefinition { get; set; }

        public string JoinClause { get; set; }
    }

    public class AMODatabase : BaseAMOObject
    {
        public string Version { get; set; }
    }

    public class AMOCube : BaseAMOObject
    {
        
    }

    public class AMODimension : BaseAMOObject
    {
        
    }

    public class AMODSV : BaseAMOObject
    {
        
    }

}

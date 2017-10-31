using System;
using Krista.FM.ServerLibrary;
using System.Collections.Generic;

namespace Krista.FM.Utils.DTSGenerator
{
    interface ISSISCommonObject
    {
        /// <summary>
        /// Уникальный идентификатор объекта
        /// </summary>
        Guid ID { get; set;}

        /// <summary>
        /// Имя объекта
        /// </summary>
        string Name { get; set;}
    }

    public class CommonObject : ISSISCommonObject
    {
        #region Fields

        private Guid id;

        private string name;

        /// <summary>
        /// Объект сервера
        /// </summary>
        private IServerSideObject controlObject;

        #endregion

        #region Constructor

        public CommonObject(IServerSideObject controlObject)
        {
            this.controlObject = controlObject;
        }


        public CommonObject(Guid id, string name, IServerSideObject controlObject)
            :this(controlObject)
        {
            this.id = id;
            this.name = name;
        }

        #endregion

        #region Properties

        public IServerSideObject ControlObject
        {
            get { return controlObject; }
            set { controlObject = value; }
        }

        #endregion

        #region ICommonObject Members

        public Guid ID
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        #endregion
    }

    /// <summary>
    /// Объект - пакет
    /// </summary>
    public class SSISPackageObject : CommonObject
    {
        public SSISPackageObject(Guid id, string name, IPackage controlObject) 
            : base(id, name, controlObject)
        {
        }

        /// <summary>
        /// Коллекция объектов-сущностей
        /// </summary>
        private Dictionary<string, SSISEntitiesObject> ssisEntities = new Dictionary<string, SSISEntitiesObject>();

        public Dictionary<string, SSISEntitiesObject> SsisEntities
        {
            get { return ssisEntities; }
            set { ssisEntities = value; }
        }
    }

    /// <summary>
    /// Объект - сущность
    /// </summary>
    public class SSISEntitiesObject : CommonObject
    {
        /// <summary>
        /// SQL-выражение на выборку данных
        /// </summary>
        private string sqlExpession;

        public SSISEntitiesObject(Guid id, string name, IEntity controlObject) 
            : base(id, name, controlObject)
        {
            sqlExpession = String.Format("select * from {0}", controlObject.FullDBName);
        }

        /// <summary>
        /// SQL-выражение на выборку данных
        /// </summary>
        public string SqlExpession
        {
            get { return sqlExpession; }
            set { sqlExpession = value; }
        }
    }

    /// <summary>
    /// Объект - атрибут
    /// </summary>
    public class SSISAttributeObject : CommonObject
    {
        public SSISAttributeObject(Guid id, string name, IDataAttribute controlObject) 
            : base(id, name, controlObject)
        {
        }
    }
}
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace Krista.FM.Utils.DTSGenerator.SMOObjects
{
    public class SSISSMO
    {

    }

    public class SSISSMOObjectsBase<T> : SSISSMO
    {
        private T controlObject;

        public SSISSMOObjectsBase(T controlObject)
        {
            this.controlObject = controlObject;
        }

        public T ControlObject
        {
            get { return controlObject; }
            set { controlObject = value; }
        }
    }

    public class SSISSMOPackage : SSISSMOObjectsBase<SSISPackageObject>
    {
        public SSISSMOPackage(SSISPackageObject controlObject) 
            : base(controlObject)
        {
        }

        [Category("Свойства объекта")]
        [DisplayName("ID пакета")]
        [Description("ID пакета")]
        [Browsable(true)]
        public Guid ID
        {
            get { return ControlObject.ID; }
        }

        [Category("Свойства объекта")]
        [DisplayName("Имя пакета")]
        [Description("Имя пакета")]
        [Browsable(true)]
        public string Name
        {
            get { return ControlObject.Name; }
        }
    }

    public class SSISSMOEntity : SSISSMOObjectsBase<SSISEntitiesObject>
    {
        private string connectionString;
        public SSISSMOEntity(SSISEntitiesObject controlObject, string connectionString) 
            : base(controlObject)
        {
            this.connectionString = connectionString;
        }

        [Browsable(false)]
        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        [Category("Свойства объекта")]
        [DisplayName("ID объекта")]
        [Description("ID объекта")]
        [Browsable(true)]
        public Guid ID
        {
            get { return ControlObject.ID; }
        }

        [Category("Свойства объекта")]
        [DisplayName("Имя объекта")]
        [Description("Имя объекта")]
        [Browsable(true)]
        public string Name
        {
            get { return ControlObject.Name; }
        }

        [Category("Свойства объекта")]
        [DisplayName("SQL-выражение на выборку данных")]
        [Description("SQL-выражение на выборку данных")]
        [Browsable(true)]
        public string SQLExpression
        {
            get { return ControlObject.SqlExpession; }
            set { ControlObject.SqlExpession = value; }
        }
    }

    public class SSISSMOAttribute : SSISSMOObjectsBase<SSISAttributeObject>
    {
        public SSISSMOAttribute(SSISAttributeObject controlObject) 
            : base(controlObject)
        {
        }

        [Category("Свойства объекта")]
        [DisplayName("ID атрибута")]
        [Description("ID атрибута")]
        [Browsable(true)]
        public Guid ID
        {
            get { return ControlObject.ID; }
        }

        [Category("Свойства объекта")]
        [DisplayName("Имя атрибута")]
        [Description("Имя атрибута")]
        [Browsable(true)]
        public string Name
        {
            get { return ControlObject.Name; }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Krista.FM.Client.OLAPAdmin;
using Microsoft.AnalysisServices;

namespace Krista.FM.Client.OLAPAdmin
{
    /// <summary>
    /// Базовый класс для свойств
    /// </summary>
    public class SMO
    {
        
    }

    public class SMOObjectsBase<T> : SMO
    {
        private T controlObject;

        public SMOObjectsBase(T controlObject)
        {
            this.controlObject = controlObject;
        }

        public T ControlObject
        {
            get { return controlObject; }
            set { controlObject = value; }
        }
    }

    public class SMOCube : SMOObjectsBase<AMOCube>
    {
        public SMOCube(AMOCube controlObject) 
            : base(controlObject)
        {
        }

        [Category("Свойства обновляемого объекта")]
        [DisplayName("Многомерная база данных")]
        [Description("При изменении активной многомерной базы, проверьте подключение к ней")]
        [Browsable(true)]
        public string Database
        {
            get { return ControlObject.Database; }
        }

        [Category("Свойства обновляемого объекта")]
        [DisplayName("Метод обновления объекта")]
        [Description("Метод обновления объекта")]
        [Browsable(true)]
        public string Method
        {
            get { return ControlObject.Method; }
        }

        [Category("Свойства обновляемого объекта")]
        [DisplayName("Имя объекта")]
        [Description("Имя объекта")]
        [Browsable(true)]
        public string Name
        {
            get { return ControlObject.Name;}
        }

        [Category("Свойства обновляемого объекта")]
        [DisplayName("Описание объекта")]
        [Description("Описание объекта")]
        [Browsable(true)]
        public string Description
        {
            get { return ControlObject.Description; }
        }

        [Category("Свойства обновляемого объекта")]
        [DisplayName("ID объекта")]
        [Description("ID объекта")]
        [Browsable(true)]
        public string ID
        {
            get { return ControlObject.ID; }
        }

        [Category("Свойства обновляемого объекта")]
        [DisplayName("DSVName")]
        [Description("DSVName")]
        [Browsable(true)]
        public string DSVName
        {
            get { return ControlObject.DsvName; }
        }
    }

    public class SMODimension : SMOObjectsBase<AMODimension>
    {
        public SMODimension(AMODimension controlObject) 
            : base(controlObject)
        {
        }
        [Category("Свойства обновляемого объекта")]
        [DisplayName("Многомерная база данных")]
        [Description("При изменении активной многомерной базы, проверьте подключение к ней")]
        [Browsable(true)]
        public string Database
        {
            get { return ControlObject.Database; }
        }

        [Category("Свойства обновляемого объекта")]
        [DisplayName("Метод обновления объекта")]
        [Description("Метод обновления объекта")]
        [Browsable(true)]
        public string Method
        {
            get { return ControlObject.Method; }
        }

        [Category("Свойства обновляемого объекта")]
        [DisplayName("Имя объекта")]
        [Description("Имя объекта")]
        [Browsable(true)]
        public string Name
        {
            get { return ControlObject.Name; }
        }

        [Category("Свойства обновляемого объекта")]
        [DisplayName("Описание объекта")]
        [Description("Описание объекта")]
        [Browsable(true)]
        public string Description
        {
            get { return ControlObject.Description; }
        }

        [Category("Свойства обновляемого объекта")]
        [DisplayName("ID объекта")]
        [Description("ID объекта")]
        [Browsable(true)]
        public string ID
        {
            get { return ControlObject.ID; }
        }

        [Category("Свойства обновляемого объекта")]
        [DisplayName("DSVName")]
        [Description("DSVName")]
        [Browsable(true)]
        public string DSVName
        {
            get { return ControlObject.DsvName; }
        }

        [Category("Свойства обновляемого объекта")]
        [DisplayName("QueryDefinition")]
        [Description("QueryDefinition")]
        [Browsable(true)]
        public string QueryDefinition
        {
            get { return ControlObject.QueryDefinition; }
        }
    }
    public class SMODSV : SMOObjectsBase<AMODSV>
    {
        public SMODSV(AMODSV controlObject)
            : base(controlObject)
        {
        }
        [Category("Свойства обновляемого объекта")]
        [DisplayName("Многомерная база данных")]
        [Description("При изменении активной многомерной базы, проверьте подключение к ней")]
        [Browsable(true)]
        public string Database
        {
            get { return ControlObject.Database; }
        }

        [Category("Свойства обновляемого объекта")]
        [DisplayName("Метод обновления объекта")]
        [Description("Метод обновления объекта")]
        [Browsable(true)]
        public string Method
        {
            get { return ControlObject.Method; }
        }

        [Category("Свойства обновляемого объекта")]
        [DisplayName("Имя объекта")]
        [Description("Имя объекта")]
        [Browsable(true)]
        public string Name
        {
            get { return ControlObject.Name; }
        }

        [Category("Свойства обновляемого объекта")]
        [DisplayName("Описание объекта")]
        [Description("Описание объекта")]
        [Browsable(true)]
        public string Description
        {
            get { return ControlObject.Description; }
        }

        [Category("Свойства обновляемого объекта")]
        [DisplayName("ID объекта")]
        [Description("ID объекта")]
        [Browsable(true)]
        public string ID
        {
            get { return ControlObject.ID; }
        }
    }
    public class SMODatabase : SMOObjectsBase<AMODatabase>
    {
        public SMODatabase(AMODatabase controlObject)
            : base(controlObject)
        {
        }
      
       
        [Category("Свойства обновляемого объекта")]
        [DisplayName("Имя объекта")]
        [Description("Имя объекта")]
        [Browsable(true)]
        public string Name
        {
            get { return ControlObject.Name; }
        }

        [Category("Свойства обновляемого объекта")]
        [DisplayName("Версия базы данных")]
        [Description("Версия базы данных")]
        [Browsable(true)]
        public string Version
        {
            get { return ControlObject.Version; }
        }
       
        [Category("Свойства обновляемого объекта")]
        [DisplayName("ID объекта")]
        [Description("ID объекта")]
        [Browsable(true)]
        public string ID
        {
            get { return ControlObject.ID; }
        }
    }
}

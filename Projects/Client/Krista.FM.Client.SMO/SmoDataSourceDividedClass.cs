using System;
using System.ComponentModel;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SMO
{
    public class SmoDataSourceDividedClass : SmoEntity, IDataSourceDividedClass
    {
        public SmoDataSourceDividedClass(IEntity serverObject) : base(serverObject)
        {
        }

        public SmoDataSourceDividedClass(SMOSerializationInfo cache)
            : base(cache)
        {
        }

        #region IDataSourceDividedClass Members

        
        public ParamKindTypes DataSourceParameter(int sourceID)
        {
            return ((IDataSourceDividedClass)serverControl).DataSourceParameter(sourceID);
        }

        internal const string IsDividedPropertyName = "IsDivided";
        public bool IsDivided
        {
            get {
                return
                    cached
                        ? Convert.ToBoolean(GetCachedValue("IsDivided"))
                        : ((IDataSourceDividedClass) ServerControl).IsDivided; }
        }

        internal const string DataSourceKindsPropertyName = "DataSourceKinds";
        public virtual string DataSourceKinds
        {
            get
            {
                return 
                    cached
                    ? GetCachedValue("DataSourceKinds").ToString()
                    : ((IDataSourceDividedClass)ServerControl).DataSourceKinds;
            }
            set
            {
                ((IDataSourceDividedClass)ServerControl).DataSourceKinds = value;
                CallOnChange();
            }
        }

        #endregion
    }
}

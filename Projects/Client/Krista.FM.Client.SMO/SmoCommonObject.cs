using System;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SMO
{
    public abstract class SmoCommonObject : SmoKeyIdentifiedObject<ICommonObject>, ICommonObject 
    {
        public SmoCommonObject(ICommonObject serverObject)
            : base(serverObject)
        {
        }

        public SmoCommonObject(SMOSerializationInfo cache)
            : base(cache)
        {
        }

        #region ICommonObject Members

        public virtual string Name
        {
            get
            {
                return cached ? (string)GetCachedValue("Name") : serverControl.Name;
            }
            set
            {
                if (ReservedWordsClass.CheckName(value))
                {
                    serverControl.Name = value;
                    CallOnChange();
                }
            }
        }

        public virtual string FullName
        {
            get { return cached ? (string)GetCachedValue("FullName") : serverControl.FullName; }
        }

        public virtual string FullDBName
        {
            get { return cached ? (string)GetCachedValue("FullDBName") : serverControl.FullDBName; }
        }

        [Obsolete]
        public bool IsValid
        {
            get { return true; }
        }

        public virtual string Caption
        {
            get
            {
                return cached ? (string)GetCachedValue("Caption") : serverControl.Caption;
            }
            set
            {
                serverControl.Caption = value;
                CallOnChange();
            }
        }

        public string Description
        {
            get
            {
                return cached ? (string)GetCachedValue("Description") : serverControl.Description;
            }
            set
            {
                serverControl.Description = value;
                CallOnChange();
            }
        }

        public string ConfigurationXml
        {
            get { return ServerControl.ConfigurationXml; }
        }
       
        #endregion
    }
}

using System;
using System.ComponentModel;
using Krista.FM.Common;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SMO
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SmoAssociateMappingValue : SmoMappingValue
    {
        public SmoAssociateMappingValue(IMappingValue serverObject, bool isBridge)
            : base(serverObject, isBridge)
        {
        }

        public override string AttributeName
        {
            get { return SmoAttribute.GetAttributeCaption(serverControl.Attribute); }
            set { serverControl.Attribute = DataAttributeHelper.GetByName(Parent.Attributes, value); CallOnChange(); }
        }

    }

    public class SmoMappingValue : SMOSerializableObject<IMappingValue>, IMappingValue
    {
        /// <summary>
        /// True - значение со стороны сопоставимого, False - значение со стороны классификатора данных
        /// </summary>
        private bool isBridge;

        public SmoMappingValue(IMappingValue serverObject, bool isBridge)
            : base(serverObject)
        {
            this.isBridge = isBridge;
        }

        #region IMappingValue Members

        public string Name
        {
            get { return cached ? (string)GetCachedValue("Name") : ServerControl.Name; }
        }

        public bool IsSample
        {
            get { return cached ? !(bool)GetCachedValue("IsSample") : !ServerControl.IsSample; }
        }

        public IDataAttribute Attribute
        {
            get
            {
                if (ServerControl.Attribute == null)
                    return null;

                return
                    cached
                        ? (IDataAttribute) SmoObjectsCache.GetSmoObject(typeof (SmoAttribute), ServerControl.Attribute)
                        : ServerControl.Attribute;
            }
            set { ServerControl.Attribute = value; CallOnChange(); }
        }

        public IEntity Parent
        {
            get 
            {
                try
                {
                    // Получаем предка ассоциацию
                    IServerSideObject sso = OwnerObject;
                    while (!(sso is IAssociation))
                    {
                        sso = sso.OwnerObject;
                        if (sso is IPackage)
                            throw new Exception("Коллекция правил формирования не имеет предка ассоциацию.");
                    }
                    IAssociation association = (IAssociation)sso;
                    return isBridge ? association.RoleBridge : association.RoleData;
                }
                catch
                {
                    return null;
                }
            }
        }

        public virtual string AttributeName
        {
            get { return SmoAttribute.GetAttributeCaption(serverControl.Attribute); }
            set { serverControl.Attribute = DataAttributeHelper.GetByName(Parent.Attributes, value); CallOnChange(); }
        }

        public string[] SourceAttributes
        {
            get { return cached ? (string[])GetCachedValue("SourceAttributes") : ServerControl.SourceAttributes; }
        }

        #endregion

        public override string ToString()
        {
            if (ServerControl.Attribute != null)
                return ServerControl.Attribute.Caption;
            else
                return ServerControl.Name;
        }
    }
}

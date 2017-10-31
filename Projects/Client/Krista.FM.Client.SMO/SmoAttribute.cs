using System;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SMO
{
    public class SmoAttribute : SmoKeyIdentifiedObject<IDataAttribute>, IDataAttribute
    {
        public SmoAttribute(IDataAttribute serverObject)
            : this(serverObject, false)
        {
        }

        public SmoAttribute(IDataAttribute serverObject, bool cached)
            : base(serverObject, cached)
        {
        }


        public SmoAttribute(SMOSerializationInfo cache) 
            : base(cache)
        {
        }


        public static string GetAttributeCaption(IDataAttribute attribute)
        {
            return String.Format("{0} ({1})", attribute.Caption, attribute.Name);
        }

        /// <summary>
        /// Фабричный метод, для создания SMO атрибутов.
        /// </summary>
        /// <param name="attribute">Атрибут.</param>
        /// <returns>SMO атрибут конкретного типа.</returns>
        public static SmoAttribute SmoAttributeFactory(IDataAttribute attribute)
        {
            if (attribute.Class == DataAttributeClassTypes.Typed)
            {
                if (attribute is IDocumentAttribute)
                {
                    return new SmoAttributeDocument(attribute);
                }
				else if (attribute is IDocumentEntityAttribute)
                {
					return new SmoAttributeDocumentEntity((IDocumentEntityAttribute)attribute);
                }
                return new SmoAttribute(attribute);
            }
            return new SmoAttributeReadOnly(attribute);
        }

        #region IDataAttribute Members

        public string Name
        {
            get { return cached ? (string)GetCachedValue("Name") : serverControl.Name; }
            set 
            { 
                if (ReservedWordsClass.CheckName(value)) 
                { 
                    serverControl.Name = value;
                    CallOnChange();
                } 
            }
        }

        public string Caption
        {
            get { return cached ? (string)GetCachedValue("Caption") : serverControl.Caption; }
            set { serverControl.Caption = value; CallOnChange(); }
        }

        public string Description
        {
            get { return cached ? (string)GetCachedValue("Description") : serverControl.Description; }
            set { serverControl.Description = value; CallOnChange(); }
        }

        internal const string TypePropertyName = "Type";

        public DataAttributeTypes Type
        {
            get { return cached ? (DataAttributeTypes)GetCachedValue("Type") : serverControl.Type; }
            set { serverControl.Type = value; CallOnChange(); }
        }

        internal const string SizePropertyName = "Size";

        public int Size
        {
            get { return cached ? (int)GetCachedValue("Size") : serverControl.Size; }
            set { serverControl.Size = value; CallOnChange(); }
        }

        internal const string ScalePropertyName = "Scale";

        public int Scale
        {
            get { return cached ? (int)GetCachedValue("Scale") : serverControl.Scale; }
            set { serverControl.Scale = value; CallOnChange(); }
        }

        internal const string DefaultPropertyName = "Default";

        public string Default
        {
            get { return cached ? Convert.ToString((string)GetCachedValue("DefaultValue")) : Convert.ToString(serverControl.DefaultValue); }
            set { serverControl.DefaultValue = value; CallOnChange(); }
        }

        public object DefaultValue
        {
            get { return cached ? GetCachedValue("DefaultValue") : Convert.ToString(serverControl.DefaultValue); }
            set { serverControl.DefaultValue = value; CallOnChange(); }
        }

        internal const string MaskPropertyName = "Mask";

        public string Mask
        {
            get { return cached ? (string)GetCachedValue("Mask") : serverControl.Mask; }
            set { serverControl.Mask = value; CallOnChange(); }
        }

        internal const string DividePropertyName = "Divide";

        public string Divide
        {
            get { return cached ? (string)GetCachedValue("Divide") : serverControl.Divide; }
            set { serverControl.Divide = value; CallOnChange(); }
        }

        public bool Visible
        {
            get { return cached ? (bool)GetCachedValue("Visible") : serverControl.Visible; }
            set { serverControl.Visible = value; CallOnChange(); }
        }

        public bool IsNullable
        {
            get { return cached ? (bool)GetCachedValue("IsNullable") : serverControl.IsNullable; }
            set { serverControl.IsNullable = value; CallOnChange(); }
        }

        [Obsolete]
        public bool Fixed
        {
            get { return serverControl.Fixed; }
        }

        internal const string LookupTypePropertyName = "LookupType";

        public LookupAttributeTypes LookupType
        {
            get { return cached ? (LookupAttributeTypes)GetCachedValue("LookupType") : serverControl.LookupType; }
            set { serverControl.LookupType = value; CallOnChange(); }
        }

        public bool IsLookup
        {
            get { return cached ? (bool)GetCachedValue("IsLookup") : serverControl.IsLookup; }
        }

        public string LookupObjectName
        {
            get { return cached ? (string)GetCachedValue("LookupObjectName") : serverControl.LookupObjectName; }
        }

        public string LookupAttributeName
        {
            get 
            {
                string caption = String.Empty;
                try
                {
                    caption = SmoAttribute.GetAttributeCaption(((IAssociation)Parent).RoleBridge.Attributes[serverControl.LookupAttributeName]);
                }
                catch
                {
                }
                return cached ? String.Format("{0}({1})", caption, GetCachedValue("LookupAttributeName")) : String.Format("{0}({1})", caption, serverControl.LookupAttributeName);
            }
            set { serverControl.LookupAttributeName = value; CallOnChange(); }
        }

        public DataAttributeClassTypes Class
        {
            get { return cached ? (DataAttributeClassTypes)GetCachedValue("Class") : serverControl.Class; }
        }

        public DataAttributeKindTypes Kind
        {
            get { return cached ? (DataAttributeKindTypes)GetCachedValue("Kind") : serverControl.Kind; }
        }

        public bool IsReadOnly
        {
            get { return cached ? (bool)GetCachedValue("IsReadOnly") : serverControl.IsReadOnly; }
            set { serverControl.IsReadOnly = value; CallOnChange(); }
        }

        internal const string StringIdentifierPropertyName = "StringIdentifier";

        public bool StringIdentifier
        {
            get { return cached ? (bool)GetCachedValue("StringIdentifier") : serverControl.StringIdentifier; }
            set { serverControl.StringIdentifier = value; CallOnChange(); }
        }

        public string SQLDefinition
        {
            get { return cached ? (string)GetCachedValue("SQLDefinition") : serverControl.SQLDefinition; }
        }

        public string DeveloperDescription
        {
            get { return cached ? (string)GetCachedValue("DeveloperDescription") : serverControl.DeveloperDescription; }
            set { serverControl.DeveloperDescription = value; CallOnChange(); }
        }

        public bool IsDocument
        {
            get { return serverControl is IDocumentAttribute; }
        }

        public ICommonDBObject Parent
        {
            get { return ((ICommonDBObject)serverControl.OwnerObject); }
        }

        public int Position
        {
            get { return serverControl.Position; }
            set { serverControl.Position = value; CallOnChange(); }
        }

        public string PositionCalc
        {
            get { return serverControl.GetCalculationPosition(); }
        }

        public string GroupParentAttribute
        {
            get { return serverControl.GroupParentAttribute; }
            set { serverControl.GroupParentAttribute = value; CallOnChange(); }
        }

        #endregion

        #region IDataAttribute Members


        public string GetCalculationPosition()
        {
            return serverControl.GetCalculationPosition();
        }

        public string GroupTags
        {
            get { return cached ? (string) GetCachedValue("GroupTags") : serverControl.GroupTags; }
            set { serverControl.GroupTags = value; CallOnChange(); }
        }

        #endregion
    }
}

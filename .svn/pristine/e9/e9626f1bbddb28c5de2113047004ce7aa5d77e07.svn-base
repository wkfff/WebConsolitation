using System;
using System.ComponentModel;

using Krista.FM.Common;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SMO
{
    public class SmoDimensionLevel : SmoKeyIdentifiedObject<IDimensionLevel>, IDimensionLevel
    {
        public SmoDimensionLevel(IDimensionLevel serverObject)
            : base(serverObject)
        {
        }


        public SmoDimensionLevel(SMOSerializationInfo cache) 
            : base(cache)
        {
        }

        #region IDimensionLevel Members

        public IClassifier Parent
        {
            get
            {
                return
                    cached
                        ? (IClassifier) SmoObjectsCache.GetSmoObject(typeof (SmoClassifier), serverControl.Parent)
                        : serverControl.Parent;
            }
        }

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

        public string Description
        {
            get { return cached ? (string)GetCachedValue("Description") : serverControl.Description; }
            set { serverControl.Description = value; CallOnChange(); }
        }

        public LevelTypes LevelType
        {
            get { return cached ? (LevelTypes)GetCachedValue("LevelType") : serverControl.LevelType; }
        }

        public virtual string LevelNamingTemplate
        {
            get { return cached ? (string)GetCachedValue("LevelNamingTemplate") : serverControl.LevelNamingTemplate; }
            set { serverControl.LevelNamingTemplate = value; CallOnChange(); }
        }

        public IDataAttribute MemberKey
        {
            get {
                return
                    cached
                        ? (IDataAttribute) SmoObjectsCache.GetSmoObject(typeof (SmoAttribute), serverControl.MemberKey)
                        : serverControl.MemberKey; }
            set { serverControl.MemberKey = value; CallOnChange(); }
        }

        public IDataAttribute MemberName
        {
            get
            {
                return
                    cached
                        ? (IDataAttribute)SmoObjectsCache.GetSmoObject(typeof(SmoAttribute), serverControl.MemberName)
                        : serverControl.MemberName;
            }
            set { serverControl.MemberName = value; CallOnChange(); }
        }

        public IDataAttribute ParentKey
        {
            get
            {
                return
                    cached
                        ? (IDataAttribute)SmoObjectsCache.GetSmoObject(typeof(SmoAttribute), serverControl.ParentKey)
                        : serverControl.ParentKey;
            }
            set { serverControl.ParentKey = value; CallOnChange(); }
        }

        public virtual string MemberKeyName
        {
            get { return SmoAttribute.GetAttributeCaption(serverControl.MemberKey); }
            set { serverControl.MemberKey = DataAttributeHelper.GetByName(Parent.Attributes, value); CallOnChange(); }
        }

        public virtual string MemberNameName
        {
            get { return SmoAttribute.GetAttributeCaption(serverControl.MemberName); }
            set { serverControl.MemberName = DataAttributeHelper.GetByName(Parent.Attributes, value); CallOnChange(); }
        }

        public virtual string ParentKeyName
        {
            get { return SmoAttribute.GetAttributeCaption(serverControl.ParentKey); }
            set { serverControl.ParentKey = DataAttributeHelper.GetByName(Parent.Attributes, value); CallOnChange(); }
        }

        #endregion
    }

    public class SmoDimensionLevelAll : SmoDimensionLevel
    {
        public SmoDimensionLevelAll(IDimensionLevel serverObject)
            : base(serverObject)
        {
        }

        public SmoDimensionLevelAll(SMOSerializationInfo cache) 
            : base(cache)
        {
        }

        [Browsable(false)]
        public override string MemberKeyName
        {
            get { return String.Empty; }
            set {  }
        }

        [Browsable(false)]
        public override string MemberNameName
        {
            get { return String.Empty; }
        }

        [Browsable(false)]
        public override string ParentKeyName
        {
            get { return String.Empty; }
            set { }
        }

        [Browsable(false)]
        public override string LevelNamingTemplate
        {
            get { return String.Empty; }
            set {  }
        }
    }

    public class SmoDimensionLevelRegular : SmoDimensionLevel
    {
        public SmoDimensionLevelRegular(IDimensionLevel serverObject)
            : base(serverObject)
        {
        }


        public SmoDimensionLevelRegular(SMOSerializationInfo cache) 
            : base(cache)
        {
        }

        [Browsable(false)]
        public override string ParentKeyName
        {
            get { return String.Empty; }
            set { }
        }
    }
}

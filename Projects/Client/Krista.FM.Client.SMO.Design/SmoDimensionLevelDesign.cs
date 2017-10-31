using System;
using System.ComponentModel;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    public class SmoDimensionLevelDesign : SmoKeyIdentifiedObjectDesign<IDimensionLevel>, IDimensionLevel
    {
        public SmoDimensionLevelDesign(IDimensionLevel serverControl)
            : base(serverControl)
        {
        }

        #region IDimensionLevel Members

        [Browsable(false)]
        public IClassifier Parent
        {
            get
            {
                return
                    serverControl.Parent;
            }
        }

        public string Name
        {
            get { return serverControl.Name; }
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
            get { return serverControl.Description; }
            set { serverControl.Description = value; CallOnChange(); }
        }

        [TypeConverter(typeof(EnumTypeConverter))]
        public LevelTypes LevelType
        {
            get { return serverControl.LevelType; }
        }

        public virtual string LevelNamingTemplate
        {
            get { return serverControl.LevelNamingTemplate; }
            set { serverControl.LevelNamingTemplate = value; CallOnChange(); }
        }

        [Browsable(false)]
        public IDataAttribute MemberKey
        {
            get
            {
                return
                    serverControl.MemberKey;
            }
            set { serverControl.MemberKey = value; CallOnChange(); }
        }

        [Browsable(false)]
        public IDataAttribute MemberName
        {
            get
            {
                return
                    serverControl.MemberName;
            }
            set { serverControl.MemberName = value; CallOnChange(); }
        }

        [Browsable(false)]
        public IDataAttribute ParentKey
        {
            get
            {
                return
                    serverControl.ParentKey;
            }
            set { serverControl.ParentKey = value; CallOnChange(); }
        }

        [TypeConverter(typeof(EntityAttributesListConverter))]
        public virtual string MemberKeyName
        {
            get { return SmoAttributeDesign.GetAttributeCaption(serverControl.MemberKey); }
            set { serverControl.MemberKey = DataAttributeHelper.GetByName(Parent.Attributes, EntityAttributesListConverter.ExtractName(value)); CallOnChange(); }
        }

        [TypeConverter(typeof(EntityAttributesListConverter))]
        public virtual string MemberNameName
        {
            get { return SmoAttributeDesign.GetAttributeCaption(serverControl.MemberName); }
            set { serverControl.MemberName = DataAttributeHelper.GetByName(Parent.Attributes, EntityAttributesListConverter.ExtractName(value)); CallOnChange(); }
        }

        [TypeConverter(typeof(EntityAttributesListConverter))]
        public virtual string ParentKeyName
        {
            get { return SmoAttributeDesign.GetAttributeCaption(serverControl.ParentKey); }
            set { serverControl.ParentKey = DataAttributeHelper.GetByName(Parent.Attributes, EntityAttributesListConverter.ExtractName(value)); CallOnChange(); }
        }

        #endregion
    }

    public class SmoDimensionLevelAllDesign : SmoDimensionLevelDesign
    {
        public SmoDimensionLevelAllDesign(IDimensionLevel serverControl)
            : base(serverControl)
        {
        }
       
        [Browsable(false)]
        public override string MemberKeyName
        {
            get { return String.Empty; }
            set { }
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
            set { }
        }
    }

    public class SmoDimensionLevelRegularDesign : SmoDimensionLevelDesign
    {
        public SmoDimensionLevelRegularDesign(IDimensionLevel serverControl)
            : base(serverControl)
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

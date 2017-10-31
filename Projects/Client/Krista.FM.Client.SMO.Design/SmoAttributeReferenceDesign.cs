using System;
using System.ComponentModel;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    public class SmoAttributeReferenceDesign : SmoAttributeDesign
    {
        public SmoAttributeReferenceDesign(IDataAttribute serverObject)
            : base(serverObject)
        {
        }
       
        #region IDataAttribute Members

        public new string Name
        {
            get { return serverControl.Name; }
            set { serverControl.Name = value; CallOnChange(); }
        }

        public new string Caption
        {
            get { return serverControl.Caption; }
            set { serverControl.Caption = value; CallOnChange(); }
        }

        public new string Description
        {
            get { return serverControl.Description; }
            set { serverControl.Description = value; CallOnChange(); }
        }

        public new DataAttributeTypes Type
        {
            get { return serverControl.Type; }
            set { serverControl.Type = value; CallOnChange(); }
        }

        public new int Size
        {
            get { return serverControl.Size; }
            set { serverControl.Size = value; CallOnChange(); }
        }

        public new int Scale
        {
            get { return serverControl.Scale; }
            set { serverControl.Scale = value; CallOnChange(); }
        }

        public new string Default
        {
            get { return Convert.ToString(serverControl.DefaultValue); }
            set { serverControl.DefaultValue = value; CallOnChange(); }
        }

        public new string Mask
        {
            get { return serverControl.Mask; }
            set { serverControl.Mask = value; CallOnChange(); }
        }

        public new string Divide
        {
            get { return serverControl.Divide; }
            set { serverControl.Divide = value; CallOnChange(); }
        }

        public new bool Visible
        {
            get { return serverControl.Visible; }
            set { serverControl.Visible = value; CallOnChange(); }
        }

        public new bool IsNullable
        {
            get { return serverControl.IsNullable; }
            set { serverControl.IsNullable = value; CallOnChange(); }
        }

        public new bool Fixed
        {
            get { return serverControl.Fixed; }
        }

        public new bool IsLookup
        {
            get { return serverControl.IsLookup; }
        }

        public new string LookupObjectName
        {
            get { return serverControl.LookupObjectName; }
        }

        public new string LookupAttributeName
        {
            get
            {
                string returnValue = String.Empty;
                string lookupAttributeName = serverControl.LookupAttributeName;

                if (!String.IsNullOrEmpty(lookupAttributeName))
                    returnValue = SmoAttributeDesign.GetAttributeCaption(
                        DataAttributeHelper.GetByName(((IAssociation)Parent).RoleBridge.Attributes, serverControl.LookupAttributeName));
                return returnValue;
            }
            set
            {
                serverControl.LookupAttributeName = value;
                CallOnChange();
            }
        }

        public new DataAttributeClassTypes Class
        {
            get { return serverControl.Class; }
        }

        public new DataAttributeKindTypes Kind
        {
            get { return serverControl.Kind; }
        }

        public new bool IsReadOnly
        {
            get { return serverControl.IsReadOnly; }
            set { serverControl.IsReadOnly = value; CallOnChange(); }
        }

        public new string SQLDefinition
        {
            get { return serverControl.SQLDefinition; }
        }

        [DisplayName("Позиция (Position)")]
        [Description("Позиция атрибута в коллекции атрибутов")]
        [Browsable(false)]
        public new int Position
        {
            get { return serverControl.Position; }
            set { serverControl.Position = value; CallOnChange(); }
        }

        [DisplayName("Вычисляемая позиция (PositionCalc)")]
        [Description("Вычисляемая позиция атрибута в коллекции атрибутов")]
        public new string PositionCalc
        {
            get { return serverControl.GetCalculationPosition(); }
        }

        [DisplayName("Тэги для группировки атрибутов (GroupParentAttribute)")]
        [Description("Тэги для группировки атрибутов")]
        public new string GroupTags
        {
            get { return serverControl.GroupTags; }
            set { serverControl.GroupTags = value; CallOnChange(); }
        }

        #endregion

        #region ICloneable Members

        public new object Clone()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }

    public class SmoAttributeReferenceReadOnlyDesign : SmoAttributeReferenceDesign
    {
        public SmoAttributeReferenceReadOnlyDesign(IDataAttribute serverObject)
            : base(serverObject)
        {
        }
    }
}

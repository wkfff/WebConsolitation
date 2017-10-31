using System;
using System.ComponentModel;
using System.Runtime.Serialization;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    /// <summary>
    /// �� ���� ��� SMO �������������, ��� � ��� �������� ��������, ������ ��� ��
    /// �������� ��������, ������� ��� ������������� �� ����������� + ��������� ��������
    /// ��� ��������� �������� �������
    /// </summary>
    [TypeConverter(typeof(SmoAttributePresentationPropertyVisibleSwitchTypeConverter))]
    public class SmoAttributePresentationDesign : SmoAttributeDesign
    {
        public SmoAttributePresentationDesign(IDataAttribute serverObject)
            : base(serverObject)
        {
        }

        /// <summary>
        /// ��������� �����, ��� �������� SMO ���������.
        /// </summary>
        /// <param name="attribute">�������.</param>
        /// <returns>SMO ������� ����������� ����.</returns>
        new public static SmoAttributeDesign SmoAttributeFactory(IDataAttribute attribute)
        {
            if (attribute.Class == DataAttributeClassTypes.Typed)
            {
                if (attribute is IDocumentAttribute)
                {
                    return new SmoAttributeDocumentDesign(attribute);
                }

                return new SmoAttributePresentationDesign(attribute);
            }

            if (attribute.Class == DataAttributeClassTypes.Reference)
                return new SmoAttributePresentationReferenceDesign(attribute);

            return new SmoAttributeReadOnlyDesign(attribute);
        }


        #region IDataAttribute Members

        [DisplayName(@"��� �������� (Name)")]
        [Description("��� �������� � ��")]
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(false)]
        new public string Name
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

        [DisplayName(@"������������ �������� (Caption)")]
        [Description("������������ ��������, ��������� � ����������")]
        new public string Caption
        {
            get { return serverControl.Caption; }
            set { serverControl.Caption = value; CallOnChange(); }
        }

        [DisplayName(@"�������� (Description)")]
        [Description("�������� ��������")]
        [Browsable(false)]
        new public string Description
        {
            get { return serverControl.Description; }
            set { serverControl.Description = value; CallOnChange(); }
        }

        [DisplayName(@"������ (Size)")]
        [Description("������ ������ ��������, �� ������ ��������� ������ ������ �������� ��������")]
        new public int Size
        {
            // ��� ����� �������� 
            get { return serverControl.Size; }
            set
            {
                //����������� �� ��������� ������� � �������� �������������
                IEntity entity = serverControl.OwnerObject.OwnerObject as IEntity;
                if (entity != null)
                {
                    if (entity.Attributes.ContainsKey(serverControl.ObjectKey))
                        if (entity.Attributes[serverControl.ObjectKey].Size < value)
                            throw new Exception(
                                String.Format(
                                    "���������� �������� ����������� �������� ������������� {0} c {1} �� {2}. ������ ����� ������ ���������",
                                    serverControl.Name, serverControl.Size, value));

                    serverControl.Size = value;
                    CallOnChange();
                }
            }
        }

        [RefreshProperties(RefreshProperties.All)]
        [DisplayName(@"�������� ������ (Scale)")]
        [Description("�������� ������ ��������")]
        new public int Scale
        {
            get { return serverControl.Scale; }
            set
            {
                IEntity entity = serverControl.OwnerObject.OwnerObject as IEntity;
                if (entity != null)
                {
                    if (entity.Attributes.ContainsKey(serverControl.ObjectKey))
                        if (entity.Attributes[serverControl.ObjectKey].Scale < value)
                            throw new Exception(
                                String.Format(
                                    "���������� �������� �������� �������� ������������� {0} c {1} �� {2}. �������� ����� ������ ���������",
                                    serverControl.Name, serverControl.Size, value));
                    serverControl.Scale = value;
                    CallOnChange();    
                }
            }
        }
        
        [DisplayName(@"�������� �������� �� ��������� (Default)")]
        [Description("�������� �������� �� ���������")]
        [Browsable(false)]
        new public string Default
        {
            get { return Convert.ToString(serverControl.DefaultValue); }
            set
            {
                // TODO ����������� �� ��������� �������� �� ��������� � �������� �������������

                serverControl.DefaultValue = value;
                CallOnChange();
            }
        }


        [DisplayName(@"����� (Mask)")]
        [Description("����� ��� ����� ��������")]
        new public string Mask
        {
            get { return serverControl.Mask; }
            set
            {
                serverControl.Mask = value;
                CallOnChange();
            }
        }

        [DisplayName(@"����� ����������� �������� ���� (Divide)")]
        [Description("����� ����������� �������� ����")]
        new public string Divide
        {
            get { return serverControl.Divide; }
            set
            {
                IEntity entity = serverControl.OwnerObject.OwnerObject as IEntity;
                try
                {
                    if (entity != null)
                    {
                        if (entity.Attributes.ContainsKey(serverControl.ObjectKey))
                            CompareMask(entity.Attributes[serverControl.ObjectKey].Divide, value);

                        serverControl.Divide = value;
                        CallOnChange();
                    }
                }
                catch (CompareDivideException e)
                {
                    throw new Exception(String.Format("���������� �������� ����� ����������� �������� ������������� {0} c {1} �� {2}. ������������ ����� ����������� {3}. {4}",
                                        serverControl.Name, serverControl.Divide, value, entity.Attributes[serverControl.ObjectKey].Divide, e.Message));
                }
            }
        }

        /// <summary>
        /// ��������� ����� �����������
        /// </summary>
        /// <param name="originalMask"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private void CompareMask(string originalMask, string value)
        {
            try
            {
                if (String.IsNullOrEmpty(value))
                    return;

                if (String.IsNullOrEmpty(originalMask))
                    throw new CompareDivideException("������������ ����� ����������� �� ������");

                string[] originalParts = originalMask.Split('.');
                string[] valueParts = value.Split('.');

                if (originalParts.Length < valueParts.Length)
                    throw new CompareDivideException("��������� ������ ����� ����������� �������� ������������� ��������� ����� ������ ������������ �����");

                for (int i = 0; i < originalParts.Length; i++)
                {
                    if (valueParts.Length > i)
                        if (Math.Abs(Int32.Parse(valueParts[i])) > Math.Abs(Int32.Parse(originalParts[i])))
                            throw new CompareDivideException("����� ����� ����������� �������� ������������� ������ ����� ������������ �����");
                }

            }
            catch (ArgumentNullException e)
            {
                throw new Exception(e.Message);
            }
            catch (ArgumentException e)
            {
                throw new Exception(e.Message);
            }
        }

        private class CompareDivideException : Exception
        {
            public CompareDivideException()
            {
            }

            public CompareDivideException(string message)
                : base(message)
            {
            }

            public CompareDivideException(string message, Exception innerException)
                : base(message, innerException)
            {
            }

            protected CompareDivideException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
        }

        [DisplayName(@"������-������������� (StringIdentifier)")]
        [Description("���� ���� �������� ��������� ���������������, �� � ��� ������������� " +
            "��������� ��������� � �������� �������, ������������������ �� ���������� �������� " +
            "���������� �����, ��������� ��� ������� ����� ������ � ������� �������� �� ����� ������." +
            "��������� �������������� ������ ������������ ��� ������������, " +
            "����������� � ������� ������������, ��� ����� ������������ � �������� ����������� " +
            "��� ���� ��������� ���������.")]
        [TypeConverter(typeof(BooleanTypeConvertor))]
        [Browsable(false)]
        new public bool StringIdentifier
        {
            get { return serverControl.StringIdentifier; }
            set { serverControl.StringIdentifier = value; CallOnChange(); }
        }

        [DisplayName(@"��������� �������� (Visible)")]
        [Description("���������� ��������� �������� � ���������������� ����������.")]
        [TypeConverter(typeof(BooleanTypeConvertor))]
        new public bool Visible
        {
            get { return serverControl.Visible; }
            set { serverControl.Visible = value; CallOnChange(); }
        }

        [DisplayName(@"����� ��������� ������ �������� (IsNullable)")]
        [Description("���������� �������������� ��������, �.�. ����� �� ������� ����� �������������� ��������.")]
        [TypeConverter(typeof(BooleanTypeConvertor))]
        new public bool IsNullable
        {
            get { return serverControl.IsNullable; }
            set
            {
                IEntity entity = serverControl.OwnerObject.OwnerObject as IEntity;
                if (entity != null)
                {
                    if (entity.Attributes.ContainsKey(serverControl.ObjectKey))
                        if (entity.Attributes[serverControl.ObjectKey].IsNullable == false
                            && value)
                            throw new Exception("������� ������������� �� ����� ��������� ������ ��������, ���� ������� �������� ����������� ��� ����������");
                    serverControl.IsNullable = value;
                    CallOnChange();
                }
            }
        }

        [DisplayName(@"������ ��� ������ (IsReadOnly)")]
        [Description("���������� ����������� �������������� �������� ������������� (��� ����������������� ����������)")]
        [TypeConverter(typeof(BooleanTypeConvertor))]
        new public bool IsReadOnly
        {
            get { return serverControl.IsReadOnly; }
            set { serverControl.IsReadOnly = value; CallOnChange(); }
        }


        [DisplayName(@"SQL (SQLDefinition)")]
        [Description("SQL-����������� ��������")]
        [Browsable(false)]
        new public string SQLDefinition
        {
            get { return serverControl.SQLDefinition; }
        }

        [DisplayName(@"������� � ����������� (LookupType)")]
        [Description("���������� ��������� �� ������� � ������������ ��� ���. ���� ���������, �� ����� ���������� � �������� ������� ����������� ��� � �������������� �������.")]
        [TypeConverter(typeof(EnumTypeConverter))]
        new public LookupAttributeTypes LookupType
        {
            get { return serverControl.LookupType; }
            set { serverControl.LookupType = value; CallOnChange(); }
        }

        [RefreshProperties(RefreshProperties.All)]
        [DisplayName(@"��� ������ (Type)")]
        [Description("��� ������ ��������")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(false)]
        new public DataAttributeTypes Type
        {
            get { return serverControl.Type; }
            set { serverControl.Type = value; CallOnChange(); }
        }

        [DisplayName(@"����������� ������� (PositionCalc)")]
        [Description("����������� ������� �������� � ��������� ���������")]
        new public string PositionCalc
        {
            get { return serverControl.GetCalculationPosition(); }
        }

        [DisplayName(@"������������ ������� ��� ����������� (GroupParentAttribute)")]
        [Description("������������ ������� ��� �����������")]
        [Browsable(false)]
        new public string GroupParentAttribute
        {
            get { return serverControl.GroupParentAttribute; }
            set { serverControl.GroupParentAttribute = value; CallOnChange(); }
        }

        [DisplayName("���� ��� ����������� ��������� (GroupTags)")]
        [Description("���� ��� ����������� ���������. ������� ����� ������ ';'.")]
        new public string GroupTags
        {
            get { return serverControl.GroupTags; }
            set
            {
                serverControl.GroupTags = value; CallOnChange();
            }
        }

        #endregion
    }

    internal class SmoAttributePresentationPropertyVisibleSwitchTypeConverter : CustomPropertiesTypeConverter<SmoAttributeDesign>
    {
        protected override Attribute[] GetPropertyAttributes(SmoAttributeDesign component, PropertyDescriptor property)
        {
            if (property.Name == "ObjectKey")
            {
                return new Attribute[] { ReadOnlyAttribute.Yes };
            }

            return EmptyAttributes;
        }
    }
}

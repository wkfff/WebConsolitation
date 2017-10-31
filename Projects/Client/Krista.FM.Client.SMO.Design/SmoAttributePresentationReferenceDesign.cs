using System;
using System.ComponentModel;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    [TypeConverter(typeof(SmoAttributePresentationPropertyVisibleSwitchTypeConverter))]
    public class SmoAttributePresentationReferenceDesign : SmoAttributeDesign
    {
        public SmoAttributePresentationReferenceDesign(IDataAttribute serverObject)
            : base(serverObject)
        {
        }

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

        [DisplayName("Наименование атрибута (Caption)")]
        [Description("Наименование атрибута, выводимое в интерфейсе")]
        new public string Caption
        {
            get { return serverControl.Caption; }
            set { serverControl.Caption = value; CallOnChange(); }
        }

        [Browsable(false)]
        new public string Description
        {
            get { return serverControl.Description; }
            set { serverControl.Description = value; CallOnChange(); }
        }

        [Browsable(false)]
        new public int Size
        {
            // При смене значения 
            get { return serverControl.Size; }
            set
            {
                serverControl.Size = value;
                CallOnChange();
            }
        }


        [Browsable(false)]
        new public string Default
        {
            get { return Convert.ToString(serverControl.DefaultValue); }
            set
            {
                // TODO ограничение на изменение значения по умолчанию у атрибута представления

                serverControl.DefaultValue = value;
                CallOnChange();
            }
        }

        [Browsable(false)]
        new public string Mask
        {
            get { return serverControl.Mask; }
            set
            {
                serverControl.Mask = value;
                CallOnChange();
            }
        }

        [Browsable(false)]
        new public string Divide
        {
            get { return serverControl.Divide; }
            set
            {
                serverControl.Divide = value;
                CallOnChange();
            }
        }

        [Browsable(false)]
        new public bool StringIdentifier
        {
            get { return serverControl.StringIdentifier; }
            set { serverControl.StringIdentifier = value; CallOnChange(); }
        }

        [DisplayName("Видимость атрибута (Visible)")]
        [Description("Определяет видимость атрибута в пользовательском интерфейсе.")]
        [TypeConverter(typeof(BooleanTypeConvertor))]
        new public bool Visible
        {
            get { return serverControl.Visible; }
            set { serverControl.Visible = value; CallOnChange(); }
        }

        [DisplayName(@"Может принимать пустые значения (IsNullable)")]
        [Description("Определяет обязательность значения, т.е. может ли атрибут иметь неопределенные значения.")]
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
                            throw new Exception("Атрибут представления не может принимать пустые значения, если атрибут сущности обязательнй для заполнения");
                    serverControl.IsNullable = value;
                    CallOnChange();
                }
            }
        }

        [DisplayName(@"Только для чтения (IsReadOnly)")]
        [Description("Определяет возможность редактирования атрибута пользователем (для пользовательского интерфейса)")]
        [TypeConverter(typeof(BooleanTypeConvertor))]
        new public bool IsReadOnly
        {
            get { return serverControl.IsReadOnly; }
            set { serverControl.IsReadOnly = value; CallOnChange(); }
        }


        [Browsable(false)]
        new public string SQLDefinition
        {
            get { return serverControl.SQLDefinition; }
        }

        [Browsable(false)]
        new public int Scale
        {
            get { return serverControl.Scale; }
            set { serverControl.Scale = value; CallOnChange(); }
        }

        [DisplayName(@"Участие в разыменовке (LookupType)")]
        [Description("Определяет участвует ли атрибут в разыменовках или нет. Если участвует, то может находиться в основной области разыменовки или в дополнительной области.")]
        [TypeConverter(typeof(EnumTypeConverter))]
        new public LookupAttributeTypes LookupType
        {
            get { return serverControl.LookupType; }
            set { serverControl.LookupType = value; CallOnChange(); }
        }

        [Browsable(false)]
        new public DataAttributeTypes Type
        {
            get { return serverControl.Type; }
            set { serverControl.Type = value; CallOnChange(); }
        }

        [DisplayName("Вычисляемая позиция (PositionCalc)")]
        [Description("Вычисляемая позиция атрибута в коллекции атрибутов")]
        new public string PositionCalc
        {
            get { return serverControl.GetCalculationPosition(); }
        }

        [Browsable(false)]
        new public string GroupParentAttribute
        {
            get { return serverControl.GroupParentAttribute; }
            set { serverControl.GroupParentAttribute = value; CallOnChange(); }
        }

        [DisplayName("Тэги для группировки атрибутов (GroupTags)")]
        [Description("Тэги для группировки атрибутов")]
        new public string GroupTags
        {
            get { return serverControl.GroupTags; }
            set { serverControl.GroupTags = value; CallOnChange(); }
        }
    }
}

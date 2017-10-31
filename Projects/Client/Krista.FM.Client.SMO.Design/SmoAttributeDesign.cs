using System;
using System.ComponentModel;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    [TypeConverter(typeof(SmoAttributePropertyVisibleSwitchTypeConverter))]
    public class SmoAttributeDesign : SmoKeyIdentifiedObjectDesign<IDataAttribute>, IDataAttribute
    {
        public SmoAttributeDesign(IDataAttribute serverControl)
            : base(serverControl)
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
        public static SmoAttributeDesign SmoAttributeFactory(IDataAttribute attribute)
        {
            if (attribute.Class == DataAttributeClassTypes.Typed)
            {
                if (attribute is IDocumentAttribute)
                {
                    return new SmoAttributeDocumentDesign(attribute);
                }
                else if (attribute is IDocumentEntityAttribute)
                {
                    return new SmoAttributeDocumentEntityDesign((IDocumentEntityAttribute)attribute);
                }
                return new SmoAttributeDesign(attribute);
            }
            return new SmoAttributeReadOnlyDesign(attribute);
        }

        #region IDataAttribute Members

        [DisplayName(@"Имя атрибута (Name)")]
        [Description("Имя атрибута в БД")]
        [RefreshProperties(RefreshProperties.All)]
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

        [DisplayName(@"Наименование атрибута (Caption)")]
        [Description("Наименование атрибута, выводимое в интерфейсе")]
        public string Caption
        {
            get { return serverControl.Caption; }
            set { serverControl.Caption = value; CallOnChange(); }
        }

        [DisplayName(@"Описание (Description)")]
        [Description("Описание атрибута")]
        public string Description
        {
            get { return serverControl.Description; }
            set { serverControl.Description = value; CallOnChange(); }
        }

        internal const string TypePropertyName = "Type";

        [RefreshProperties(RefreshProperties.All)]
        [DisplayName(@"Тип данных (Type)")]
        [Description("Тип данных атрибута")]
        [TypeConverter(typeof(EnumTypeConverter))]
        public DataAttributeTypes Type
        {
            get { return serverControl.Type; }
            set { serverControl.Type = value; CallOnChange(); }
        }

        internal const string SizePropertyName = "Size";

        [DisplayName(@"Размер (Size)")]
        [Description("Размер данных атрибута")]
        public int Size
        {
            get { return serverControl.Size; }
            set { serverControl.Size = value; CallOnChange(); }
        }

        internal const string ScalePropertyName = "Scale";

        [RefreshProperties(RefreshProperties.All)]
        [DisplayName(@"Точность данных (Scale)")]
        [Description("Точность данных атрибута")]
        public int Scale
        {
            get { return serverControl.Scale; }
            set { serverControl.Scale = value; CallOnChange(); }
        }

        internal const string DefaultPropertyName = "Default";

        [DisplayName(@"Значение атрибута по умолчанию (Default)")]
        [Description("Значение атрибута по умолчанию")]
        public string Default
        {
            get { return Convert.ToString(serverControl.DefaultValue); }
            set { serverControl.DefaultValue = value; CallOnChange(); }
        }

        [Browsable(false)]
        public object DefaultValue
        {
            get { return Convert.ToString(serverControl.DefaultValue); }
            set { serverControl.DefaultValue = value; CallOnChange(); }
        }

        internal const string MaskPropertyName = "Mask";

        [DisplayName(@"Маска (Mask)")]
        [Description("Маска для ввода значения")]
        public string Mask
        {
            get { return serverControl.Mask; }
            set { serverControl.Mask = value; CallOnChange(); }
        }

        internal const string DividePropertyName = "Divide";

        [DisplayName(@"Маска расщепления значения кода (Divide)")]
        [Description("Маска расщепления значения кода")]
        public string Divide
        {
            get { return serverControl.Divide; }
            set { serverControl.Divide = value; CallOnChange(); }
        }

        [DisplayName(@"Видимость атрибута (Visible)")]
        [Description("Определяет видимость атрибута в пользовательском интерфейсе.")]
        [TypeConverter(typeof(BooleanTypeConvertor))]
        public bool Visible
        {
            get { return serverControl.Visible; }
            set { serverControl.Visible = value; CallOnChange(); }
        }

        [DisplayName(@"Может принимать пустые значения (IsNullable)")]
        [Description("Определяет обязательность значения, т.е. может ли атрибут иметь неопределенные значения.")]
        [TypeConverter(typeof(BooleanTypeConvertor))]
        public bool IsNullable
        {
            get { return serverControl.IsNullable; }
            set { serverControl.IsNullable = value; CallOnChange(); }
        }

        [DisplayName(@"Фиксированный атрибут или нет (Fixed)")]
        [Description("Определяет фиксированный атрибут или нет")]
        [Browsable(false)]
        public bool Fixed
        {
            get { return serverControl.Fixed; }
        }

        internal const string LookupTypePropertyName = "LookupType";

        [DisplayName(@"Участие в разыменовке (LookupType)")]
        [Description("Определяет участвует ли атрибут в разыменовках или нет. Если участвует, то может находиться в основной области разыменовки или в дополнительной области.")]
        [TypeConverter(typeof(EnumTypeConverter))]
        public LookupAttributeTypes LookupType
        {
            get { return serverControl.LookupType; }
            set { serverControl.LookupType = value; CallOnChange(); }
        }

        [DisplayName(@"Является ли атрибут атрибутом типа Lookup (IsLookup)")]
        [Description("Определяет является ли атрибут атрибутом типа Lookup")]
        [Browsable(false)]
        public bool IsLookup
        {
            get { return serverControl.IsLookup; }
        }

        [DisplayName(@"Полное наименование объекта (LookupObjectName)")]
        [Description("Полное наименование объекта в котором находится ключ (ID),на который ссылается значение хранимое в атрибуте")]
        [Browsable(false)]
        public string LookupObjectName
        {
            get { return serverControl.LookupObjectName; }
        }

        [DisplayName(@"Наименование аттрибута лукап-объекта (LookupAttributeName)")]
        [Description("Наименование аттрибута лукап-объекта, значение которого отображается")]
        [TypeConverter(typeof(LookupAttributesListConverter))]
        [Browsable(false)]
        public string LookupAttributeName
        {
            get
            {
                string caption = String.Empty;
                try
                {
                    caption =
                        GetAttributeCaption(
                            ((IAssociation)Parent).RoleBridge.Attributes[serverControl.LookupAttributeName]);
                }
                catch
                {
                }
                return serverControl.LookupAttributeName;
            }
            set { serverControl.LookupAttributeName = AttributesListConverter.ExtractName(value); CallOnChange(); }
        }

        [DisplayName(@"Системный класс (Class)")]
        [Description("Системный класс")]
        [Browsable(false)]
        public DataAttributeClassTypes Class
        {
            get { return serverControl.Class; }
        }

        [DisplayName(@"Вид атрибута (Kind)")]
        [Description("Вид атрибута (для пользовательского интерфейса)")]
        [Browsable(false)]
        public DataAttributeKindTypes Kind
        {
            get { return serverControl.Kind; }
        }

        [DisplayName(@"Только для чтения (IsReadOnly)")]
        [Description("Определяет возможность редактирования атрибута пользователем (для пользовательского интерфейса)")]
        [TypeConverter(typeof(BooleanTypeConvertor))]
        public bool IsReadOnly
        {
            get { return serverControl.IsReadOnly; }
            set { serverControl.IsReadOnly = value; CallOnChange(); }
        }

        internal const string StringIdentifierPropertyName = "StringIdentifier";

        [DisplayName(@"Строка-идентификатор (StringIdentifier)")]
        [Description("Если поле является строковым идентификатором, то в нем автоматически " +
            "удаляются начальные и конечные пробелы, последовательность из нескольких пробелов " +
            "заменяется одним, удаляются все символы конца строки и символы перехода на новую строку." +
            "Строковые идентификаторы обычно используются для наименовыний, " +
            "обозначений и кратких наименований, для полей используемых в качестве разыменовки " +
            "для имен элементов измерения.")]
        [TypeConverter(typeof(BooleanTypeConvertor))]
        public bool StringIdentifier
        {
            get { return serverControl.StringIdentifier; }
            set { serverControl.StringIdentifier = value; CallOnChange(); }
        }

        [DisplayName(@"SQL (SQLDefinition)")]
        [Description("SQL-определение атрибута")]
        public string SQLDefinition
        {
            get { return serverControl.SQLDefinition; }
        }

        [Browsable(false)]
        public string DeveloperDescription
        {
            get { return serverControl.DeveloperDescription; }
            set { serverControl.DeveloperDescription = value; CallOnChange(); }
        }

        [DisplayName(@"Атрибут-документ (IsDocument)")]
        [Description("Определяет является ли атрибут документом.")]
        [TypeConverter(typeof(BooleanTypeConvertor))]
        [Browsable(false)]
        public bool IsDocument
        {
            get { return serverControl is IDocumentAttribute; }
        }

        [Browsable(false)]
        public ICommonDBObject Parent
        {
            get { return ((ICommonDBObject)serverControl.OwnerObject); }
        }

        [DisplayName(@"Позиция (Position)")]
        [Description("Позиция атрибута в коллекции атрибутов")]
        [Browsable(false)]
        public int Position
        {
            get { return serverControl.Position; }
            set { serverControl.Position = value; CallOnChange(); }
        }

        [DisplayName(@"Вычисляемая позиция (PositionCalc)")]
        [Description("Вычисляемая позиция атрибута в коллекции атрибутов")]
        public string PositionCalc
        {
            get { return serverControl.GetCalculationPosition(); }
        }

        [DisplayName(@"Родительский атрибут при группировке (GroupParentAttribute)")]
        [Description("Родительский атрибут при группировке")]
        [Browsable(false)]
        public string GroupParentAttribute
        {
            get { return serverControl.GroupParentAttribute; }
            set { serverControl.GroupParentAttribute = value; CallOnChange(); }
        }

        [DisplayName("Тэги для группировки атрибутов (GroupTags)")]
        [Description("Тэги для группировки атрибутов")]
        public string GroupTags
        {
            get { return serverControl.GroupTags; }
            set { serverControl.GroupTags = value; CallOnChange(); }
        }

        #endregion

        #region IDataAttribute Members


        public string GetCalculationPosition()
        {
            return serverControl.GetCalculationPosition();
        }

        #endregion
    }

    internal class SmoAttributePropertyVisibleSwitchTypeConverter : CustomPropertiesTypeConverter<SmoAttributeDesign>
    {
        protected override Attribute[] GetPropertyAttributes(SmoAttributeDesign component, PropertyDescriptor property)
        {
            if (property.Name == "Name" && component.ServerControl is IDocumentEntityAttribute)
            {
                return new Attribute[] { BrowsableAttribute.No };
            }

            if (property.Name == "IsNullable" && component.ServerControl is IDocumentEntityAttribute)
            {
                return new Attribute[] { BrowsableAttribute.No };
            }

            if (property.Name == "SQLDefinition" && component.ServerControl is IDocumentEntityAttribute)
            {
                return new Attribute[] { BrowsableAttribute.No };
            }

            if (property.Name == SmoAttributeDesign.TypePropertyName)
            {
                if (component.ServerControl is IDocumentAttribute ||
                    component.ServerControl is IDocumentEntityAttribute)
                    return new Attribute[] { BrowsableAttribute.No };
                else
                    return new Attribute[] { BrowsableAttribute.Yes, };
            }

            if (property.Name == SmoAttributeDesign.ScalePropertyName)
            {
                if (component.ServerControl is IDocumentEntityAttribute)
                    return new Attribute[] { BrowsableAttribute.No };

                switch (component.Type)
                {
                    case DataAttributeTypes.dtDouble:
                        return new Attribute[] { BrowsableAttribute.Yes };
                    default:
                        return new Attribute[] { BrowsableAttribute.No };
                }
            }

            if (property.Name == SmoAttributeDesign.SizePropertyName)
            {
                if (component.ServerControl is IDocumentEntityAttribute)
                    return new Attribute[] { BrowsableAttribute.No };

                switch (component.Type)
                {
                    case DataAttributeTypes.dtBoolean:
                    case DataAttributeTypes.dtDate:
                    case DataAttributeTypes.dtDateTime:
                    case DataAttributeTypes.dtBLOB:
                        return new Attribute[] { BrowsableAttribute.No };
                    default:
                        return new Attribute[] { BrowsableAttribute.Yes };
                }
            }

            if (property.Name == SmoAttributeDesign.StringIdentifierPropertyName)
            {
                if (component.ServerControl is IDocumentEntityAttribute)
                    return new Attribute[] { BrowsableAttribute.No };

                switch (component.Type)
                {
                    case DataAttributeTypes.dtString:
                    case DataAttributeTypes.dtChar:
                        return new Attribute[] { BrowsableAttribute.Yes };
                    default:
                        return new Attribute[] { BrowsableAttribute.No };
                }
            }

            if (property.Name == SmoAttributeDesign.DefaultPropertyName ||
                property.Name == SmoAttributeDesign.LookupTypePropertyName)
            {
                if (component.ServerControl is IDocumentEntityAttribute)
                    return new Attribute[] { BrowsableAttribute.No };

                if (component.Type == DataAttributeTypes.dtBLOB)
                    return new Attribute[] { BrowsableAttribute.No };
                else
                    return new Attribute[] { BrowsableAttribute.Yes };
            }

            return EmptyAttributes;
        }
    }
}

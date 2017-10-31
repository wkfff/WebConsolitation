using System;
using System.ComponentModel;
using System.Runtime.Serialization;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    /// <summary>
    /// По сути это SMO представление, как и для простого атрибута, только тут мы
    /// скрываем свойства, которые для представления не редактируем + добавляем контроль
    /// при изменении значений свойств
    /// </summary>
    [TypeConverter(typeof(SmoAttributePresentationPropertyVisibleSwitchTypeConverter))]
    public class SmoAttributePresentationDesign : SmoAttributeDesign
    {
        public SmoAttributePresentationDesign(IDataAttribute serverObject)
            : base(serverObject)
        {
        }

        /// <summary>
        /// Фабричный метод, для создания SMO атрибутов.
        /// </summary>
        /// <param name="attribute">Атрибут.</param>
        /// <returns>SMO атрибут конкретного типа.</returns>
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

        [DisplayName(@"Имя атрибута (Name)")]
        [Description("Имя атрибута в БД")]
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

        [DisplayName(@"Наименование атрибута (Caption)")]
        [Description("Наименование атрибута, выводимое в интерфейсе")]
        new public string Caption
        {
            get { return serverControl.Caption; }
            set { serverControl.Caption = value; CallOnChange(); }
        }

        [DisplayName(@"Описание (Description)")]
        [Description("Описание атрибута")]
        [Browsable(false)]
        new public string Description
        {
            get { return serverControl.Description; }
            set { serverControl.Description = value; CallOnChange(); }
        }

        [DisplayName(@"Размер (Size)")]
        [Description("Размер данных атрибута, не должен превышать размер данных атрибута сущности")]
        new public int Size
        {
            // При смене значения 
            get { return serverControl.Size; }
            set
            {
                //ограничение на изменение размера у атрибута представления
                IEntity entity = serverControl.OwnerObject.OwnerObject as IEntity;
                if (entity != null)
                {
                    if (entity.Attributes.ContainsKey(serverControl.ObjectKey))
                        if (entity.Attributes[serverControl.ObjectKey].Size < value)
                            throw new Exception(
                                String.Format(
                                    "Невозможно изменить размерность атрибута представления {0} c {1} на {2}. Размер можно только уменьшить",
                                    serverControl.Name, serverControl.Size, value));

                    serverControl.Size = value;
                    CallOnChange();
                }
            }
        }

        [RefreshProperties(RefreshProperties.All)]
        [DisplayName(@"Точность данных (Scale)")]
        [Description("Точность данных атрибута")]
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
                                    "Невозможно изменить точность атрибута представления {0} c {1} на {2}. Точность можно только уменьшить",
                                    serverControl.Name, serverControl.Size, value));
                    serverControl.Scale = value;
                    CallOnChange();    
                }
            }
        }
        
        [DisplayName(@"Значение атрибута по умолчанию (Default)")]
        [Description("Значение атрибута по умолчанию")]
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


        [DisplayName(@"Маска (Mask)")]
        [Description("Маска для ввода значения")]
        new public string Mask
        {
            get { return serverControl.Mask; }
            set
            {
                serverControl.Mask = value;
                CallOnChange();
            }
        }

        [DisplayName(@"Маска расщепления значения кода (Divide)")]
        [Description("Маска расщепления значения кода")]
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
                    throw new Exception(String.Format("Невозможно изменить маску расщепления атрибута представления {0} c {1} на {2}. Оригинальная маска расщепления {3}. {4}",
                                        serverControl.Name, serverControl.Divide, value, entity.Attributes[serverControl.ObjectKey].Divide, e.Message));
                }
            }
        }

        /// <summary>
        /// Сравнение маски расщепления
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
                    throw new CompareDivideException("Оригинальная маска расщепления не задана");

                string[] originalParts = originalMask.Split('.');
                string[] valueParts = value.Split('.');

                if (originalParts.Length < valueParts.Length)
                    throw new CompareDivideException("Количесво частей маски расщепления атрибута представления превышает число частей оригинальной маски");

                for (int i = 0; i < originalParts.Length; i++)
                {
                    if (valueParts.Length > i)
                        if (Math.Abs(Int32.Parse(valueParts[i])) > Math.Abs(Int32.Parse(originalParts[i])))
                            throw new CompareDivideException("Часть маски расщепления атрибута представления больше части оригинальгой маски");
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

        [DisplayName(@"Строка-идентификатор (StringIdentifier)")]
        [Description("Если поле является строковым идентификатором, то в нем автоматически " +
            "удаляются начальные и конечные пробелы, последовательность из нескольких пробелов " +
            "заменяется одним, удаляются все символы конца строки и символы перехода на новую строку." +
            "Строковые идентификаторы обычно используются для наименовыний, " +
            "обозначений и кратких наименований, для полей используемых в качестве разыменовки " +
            "для имен элементов измерения.")]
        [TypeConverter(typeof(BooleanTypeConvertor))]
        [Browsable(false)]
        new public bool StringIdentifier
        {
            get { return serverControl.StringIdentifier; }
            set { serverControl.StringIdentifier = value; CallOnChange(); }
        }

        [DisplayName(@"Видимость атрибута (Visible)")]
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


        [DisplayName(@"SQL (SQLDefinition)")]
        [Description("SQL-определение атрибута")]
        [Browsable(false)]
        new public string SQLDefinition
        {
            get { return serverControl.SQLDefinition; }
        }

        [DisplayName(@"Участие в разыменовке (LookupType)")]
        [Description("Определяет участвует ли атрибут в разыменовках или нет. Если участвует, то может находиться в основной области разыменовки или в дополнительной области.")]
        [TypeConverter(typeof(EnumTypeConverter))]
        new public LookupAttributeTypes LookupType
        {
            get { return serverControl.LookupType; }
            set { serverControl.LookupType = value; CallOnChange(); }
        }

        [RefreshProperties(RefreshProperties.All)]
        [DisplayName(@"Тип данных (Type)")]
        [Description("Тип данных атрибута")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [Browsable(false)]
        new public DataAttributeTypes Type
        {
            get { return serverControl.Type; }
            set { serverControl.Type = value; CallOnChange(); }
        }

        [DisplayName(@"Вычисляемая позиция (PositionCalc)")]
        [Description("Вычисляемая позиция атрибута в коллекции атрибутов")]
        new public string PositionCalc
        {
            get { return serverControl.GetCalculationPosition(); }
        }

        [DisplayName(@"Родительский атрибут при группировке (GroupParentAttribute)")]
        [Description("Родительский атрибут при группировке")]
        [Browsable(false)]
        new public string GroupParentAttribute
        {
            get { return serverControl.GroupParentAttribute; }
            set { serverControl.GroupParentAttribute = value; CallOnChange(); }
        }

        [DisplayName("Тэги для группировки атрибутов (GroupTags)")]
        [Description("Тэги для группировки атрибутов. Вводить через символ ';'.")]
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

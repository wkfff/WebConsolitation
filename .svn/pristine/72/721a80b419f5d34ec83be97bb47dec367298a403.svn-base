using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using System.Text;


namespace Krista.FM.Client.MDXExpert.Common
{
    #region GenericDictionaryEditor
    /// <summary>
    /// Provides attributes for the Keys or Values in the dictionary.
    /// </summary>
    public abstract class AttributeProvider
    {
        /// <summary>
        /// Returns a collection of attributes for the specified type. 
        /// </summary>
        /// <param name="type">The type of the key or value to provide attributes for.</param>
        /// <returns>An <see cref="AttributeCollection"/> with the attributes for the component.</returns>
        public abstract AttributeCollection GetAttributes(Type type);
    }

    /// <summary>
    /// Used to specify whether the requested default value is to be used as Key or Value of a dictionary entry
    /// </summary>
    /// <remarks>If the default value is to be used as Key it may NOT be null (because the Dictionary doesn't allow null as Key)</remarks>
    public enum DefaultUsage
    {
        /// <summary>
        /// The requested default value is to be used as Key in the dictionary
        /// </summary>
        Key,
        /// <summary>
        /// The requested default value is to be used as Value in the dictionary
        /// </summary>
        Value
    }

    /// <summary>
    /// Provides default values for the Key or Value properties for new dictionary entries
    /// </summary>
    /// <typeparam name="T">The type of the Key or Value</typeparam>
    public class DefaultProvider<T>
    {
        /// <summary>
        /// Returns a default value for the Key or Value properties for new dictionary entries
        /// </summary>
        /// <param name="usage">Specifies if the desired default value is to be used as Key or Value</param>
        /// <returns>Returns a value of type T to be used as the default</returns>
        /// <remarks>If the default value is to be used as Key it may NOT be null (because the Dictionary doesn't allow null as Key)</remarks>
        public virtual T GetDefault(DefaultUsage usage)
        {
            Type t = typeof(T);
            if (t.IsPrimitive || t.IsEnum)
                return default(T);
            else if (t == typeof(string))
                return (T)(object)String.Empty;
            else
                return Activator.CreateInstance<T>();
        }
    }

    internal class EditableKeyValuePair<TKey, TValue> : CustomTypeDescriptor
    {
        [DisplayName("1.Имя свойства")]
        public TKey Key { get; set; }
        [DisplayName("2.Значение")]
        public TValue Value { get; set; }

        public EditableKeyValuePair(TKey key, TValue value, GenericDictionaryEditorAttribute editorAttribute)
        {
            this.Key = key;
            this.Value = value;

            if (editorAttribute == null)
                throw new ArgumentNullException("editorAttribute");

            m_EditorAttribute = editorAttribute;
        }

        private GenericDictionaryEditorAttribute m_EditorAttribute;
        public GenericDictionaryEditorAttribute EditorAttribute
        {
            get { return m_EditorAttribute; }
            set { m_EditorAttribute = value; }
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
            KeyValueDescriptor KeyDescriptor = new KeyValueDescriptor(TypeDescriptor.CreateProperty(this.GetType(), "Key", typeof(TKey)), this.EditorAttribute.KeyConverterType, this.EditorAttribute.KeyEditorType, this.EditorAttribute.KeyAttributeProviderType);
            properties.Add(KeyDescriptor);

            KeyValueDescriptor ValueDescriptor = new KeyValueDescriptor(TypeDescriptor.CreateProperty(this.GetType(), "Value", typeof(TValue)), this.EditorAttribute.ValueConverterType, this.EditorAttribute.ValueEditorType, this.EditorAttribute.ValueAttributeProviderType);
            properties.Add(ValueDescriptor);
            
            return new PropertyDescriptorCollection(properties.ToArray());
        }

        public override object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
    }

    /// <summary>
    /// A <see cref="System.Drawing.Design.UITypeEditor">UITypeEditor</see> for editing generic dictionaries in the <see cref="System.Windows.Forms.PropertyGrid">PropertyGrid</see>.
    /// </summary>
    /// <typeparam name="TKey">The type of the Keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the Values in the dictionary.</typeparam>
    public class GenericDictionaryEditor<TKey, TValue> : CollectionEditor
    {
        /// <summary>
        /// Initializes a new instance of the GenericDictionaryEditor class using the specified collection type.
        /// </summary>
        /// <param name="type">The type of the collection for this editor to edit.</param>
        public GenericDictionaryEditor(Type type)
            : base(type)
        {
        }

        private GenericDictionaryEditorAttribute m_EditorAttribute;

        private CollectionEditor.CollectionForm m_Form;

        /// <summary>
        /// Creates a new form to display and edit the current collection.
        /// </summary>
        /// <returns>A <see cref="CollectionEditor.CollectionForm"/>  to provide as the user interface for editing the collection.</returns>
        protected override CollectionEditor.CollectionForm CreateCollectionForm()
        {
            m_Form = base.CreateCollectionForm();
            m_Form.Text = this.m_EditorAttribute.Title; 

            // Die Eigenschaft "CollectionEditable" muss hier per Reflection gesetzt werden (ist protected)
            Type formType = m_Form.GetType();
            PropertyInfo pi = formType.GetProperty("CollectionEditable", BindingFlags.NonPublic | BindingFlags.Instance);
            pi.SetValue(m_Form, true, null);

            return m_Form;
        }

        /// <summary>
        /// Edits the value of the specified object using the specified service provider and context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that can be used to gain additional context information. </param>
        /// <param name="provider">A service provider object through which editing services can be obtained.</param>
        /// <param name="value">The object to edit the value of.</param>
        /// <returns>The new value of the object. If the value of the object has not changed, this should return the same object it was passed.</returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            GenericDictionaryEditorAttribute attribute = context.PropertyDescriptor.Attributes[typeof(GenericDictionaryEditorAttribute)] as GenericDictionaryEditorAttribute;
            if (attribute != null)
            {
                this.m_EditorAttribute = attribute;

                if (this.m_EditorAttribute.KeyDefaultProviderType == null)
                    this.m_EditorAttribute.KeyDefaultProviderType = typeof(DefaultProvider<TKey>);

                if (this.m_EditorAttribute.ValueDefaultProviderType == null)
                    this.m_EditorAttribute.ValueDefaultProviderType = typeof(DefaultProvider<TValue>);
            }
            else
            {
                this.m_EditorAttribute = new GenericDictionaryEditorAttribute();
                this.m_EditorAttribute.KeyDefaultProviderType = typeof(DefaultProvider<TKey>);
                this.m_EditorAttribute.ValueDefaultProviderType = typeof(DefaultProvider<TValue>);
            }

            return base.EditValue(context, provider, value);
        }

        /// <summary>
        /// Gets the data type that this collection contains.
        /// </summary>
        /// <returns>The data type of the items in the collection, or an Object if no Item property can be located on the collection.</returns>
        protected override Type CreateCollectionItemType()
        {
            return typeof(EditableKeyValuePair<TKey, TValue>);
        }

        /// <summary>
        /// Creates a new instance of the specified collection item type.
        /// </summary>
        /// <param name="itemType">The type of item to create.</param>
        /// <returns>A new instance of the specified type.</returns>
        protected override object CreateInstance(Type itemType)
        {
            TKey key;
            TValue value;

            DefaultProvider<TKey> KeyDefaultProvider = Activator.CreateInstance(this.m_EditorAttribute.KeyDefaultProviderType) as DefaultProvider<TKey>;
            if (KeyDefaultProvider != null)
                key = KeyDefaultProvider.GetDefault(DefaultUsage.Key);
            else
                key = default(TKey);

            DefaultProvider<TValue> ValueDefaultProvider = Activator.CreateInstance(this.m_EditorAttribute.ValueDefaultProviderType) as DefaultProvider<TValue>;
            if (ValueDefaultProvider != null)
                value = ValueDefaultProvider.GetDefault(DefaultUsage.Value);
            else
                value = default(TValue);

            return new EditableKeyValuePair<TKey, TValue>(key, value, this.m_EditorAttribute);
        }

        /// <summary>
        /// Gets an array of objects containing the specified collection.
        /// </summary>
        /// <param name="editValue">The collection to edit.</param>
        /// <returns>An array containing the collection objects, or an empty object array if the specified collection does not inherit from ICollection.</returns>
        protected override object[] GetItems(object editValue)
        {
            if (editValue == null)
            {
                return new object[0];
            }
            Dictionary<TKey, TValue> dictionary = editValue as Dictionary<TKey, TValue>;
            if (dictionary == null)
            {
                throw new ArgumentNullException("editValue");
            }
            object[] objArray = new object[dictionary.Count];
            int num = 0;
            foreach (KeyValuePair<TKey, TValue> entry in dictionary)
            {
                EditableKeyValuePair<TKey, TValue> entry2 = new EditableKeyValuePair<TKey, TValue>(entry.Key, entry.Value, this.m_EditorAttribute);
                objArray[num++] = entry2;
            }
            return objArray;
        }

        /// <summary>
        /// Sets the specified array as the items of the collection.
        /// </summary>
        /// <param name="editValue">The collection to edit.</param>
        /// <param name="value">An array of objects to set as the collection items.</param>
        /// <returns>The newly created collection object or, otherwise, the collection indicated by the <paramref name="editValue"/> parameter.</returns>
        protected override object SetItems(object editValue, object[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            IDictionary<TKey, TValue> dictionary = editValue as IDictionary<TKey, TValue>;
            if (dictionary == null)
            {
                throw new ArgumentNullException("editValue");
            }
            dictionary.Clear();
            foreach (EditableKeyValuePair<TKey, TValue> entry in value)
            {
                dictionary.Add(new KeyValuePair<TKey, TValue>(entry.Key, entry.Value));
            }
            return dictionary;
        }

        /// <summary>
        /// Retrieves the display text for the given list item.
        /// </summary>
        /// <param name="value">The list item for which to retrieve display text.</param>
        /// <returns>he display text for <paramref name="value"/>.</returns>
        protected override string GetDisplayText(object value)
        {
            EditableKeyValuePair<TKey, TValue> pair = value as EditableKeyValuePair<TKey, TValue>;
            if (pair != null)
                return string.Format(CultureInfo.CurrentCulture, "{0}={1}", pair.Key, pair.Value);
            else
                return base.GetDisplayText(value);
        }
    }

    /// <summary>
    /// Provides configuration options for the GenericDictionaryEditor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class GenericDictionaryEditorAttribute : Attribute
    {
        /// <summary>
        /// Specifies what type to use as a <see cref="DefaultProvider{T}"/> for the keys in the dictionary.
        /// </summary>
        public Type KeyDefaultProviderType { get; set; }

        /// <summary>
        /// Specifies what type to use as a <see cref="System.ComponentModel.TypeConverter">converter</see> for the keys in the dictionary.
        /// </summary>
        public Type KeyConverterType { get; set; }

        /// <summary>
        /// Specifies what type to use as an <see cref="System.Drawing.Design.UITypeEditor">editor</see> to change the key of a dictionary entry.
        /// </summary>
        public Type KeyEditorType { get; set; }

        /// <summary>
        /// Specifies what type to use as an <see cref="AttributeProvider"/> for the keys in the dictionary.
        /// </summary>
        public Type KeyAttributeProviderType { get; set; }

        /// <summary>
        /// Specifies what type to use as a <see cref="DefaultProvider{T}"/> for the values in the dictionary.
        /// </summary>
        public Type ValueDefaultProviderType { get; set; }

        /// <summary>
        /// Specifies what type to use as a <see cref="System.ComponentModel.TypeConverter">converter</see> for the values in the dictionary.
        /// </summary>
        public Type ValueConverterType { get; set; }

        /// <summary>
        /// Specifies what type to use as an <see cref="System.Drawing.Design.UITypeEditor">editor</see> to change the value of a dictionary entry.
        /// </summary>
        public Type ValueEditorType { get; set; }

        /// <summary>
        /// Specifies what type to use as <see cref="AttributeProvider"/> for the values in the dictionary.
        /// </summary>
        public Type ValueAttributeProviderType { get; set; }

        /// <summary>
        /// The title for the editor window.
        /// </summary>
        public String Title { get; set; }
    }

    internal class KeyValueDescriptor : PropertyDescriptor
    {
        private PropertyDescriptor _pd;

        private Type m_ConverterType;
        private Type m_EditorType;
        private Type m_AttributeProviderType;

        public KeyValueDescriptor(PropertyDescriptor pd, Type converterType, Type editorType, Type attributeProviderType)
            : base(pd)
        {
            this._pd = pd;


            m_ConverterType = converterType;
            m_EditorType = editorType;
            m_AttributeProviderType = attributeProviderType;
        }

        public override bool CanResetValue(object component)
        {
            return _pd.CanResetValue(component);
        }

        public override Type ComponentType
        {
            get { return _pd.ComponentType; }
        }

        public override object GetValue(object component)
        {
            return _pd.GetValue(component);
        }

        public override bool IsReadOnly
        {
            get { return _pd.IsReadOnly; }
        }

        public override Type PropertyType
        {
            get { return _pd.PropertyType; }
        }

        public override void ResetValue(object component)
        {
            _pd.ResetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            _pd.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return _pd.ShouldSerializeValue(component);
        }

        public override TypeConverter Converter
        {
            get
            {
                if (m_ConverterType != null)
                    return Activator.CreateInstance(m_ConverterType) as TypeConverter;
                else
                {
                    return TypeDescriptor.GetConverter(PropertyType);
                }
            }
        }

        public override object GetEditor(Type editorBaseType)
        {
            if (m_EditorType != null)
                return Activator.CreateInstance(m_EditorType) as UITypeEditor;
            else
                return TypeDescriptor.GetEditor(PropertyType, typeof(UITypeEditor));
        }

        /*
        public override AttributeCollection Attributes
        {
            get
            {
                if (m_AttributeProviderType != null)
                {
                    return (Activator.CreateInstance(m_AttributeProviderType) as AttributeProvider).GetAttributes(this.PropertyType);
                }
                else
                {
                    return TypeDescriptor.GetAttributes(PropertyType);
                }
            }
        }*/
    }

    #endregion 

    #region Класс для задания порядка следования свойств в проперти грид

    public class PropertySorter : ExpandableObjectConverter
    {
        #region Методы

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Возвращает упорядоченный список свойств
        /// </summary>
        public override PropertyDescriptorCollection GetProperties(
          ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection pdc =
              TypeDescriptor.GetProperties(value, attributes);

            ArrayList orderedProperties = new ArrayList();

            foreach (PropertyDescriptor pd in pdc)
            {
                Attribute attribute = pd.Attributes[typeof(PropertyOrderAttribute)];

                if (attribute != null)
                {
                    // атрибут есть - используем номер п/п из него
                    PropertyOrderAttribute poa = (PropertyOrderAttribute)attribute;
                    orderedProperties.Add(new PropertyOrderPair(pd.Name, poa.Order));
                }
                else
                {
                    // атрибута нет – считаем, что 0
                    orderedProperties.Add(new PropertyOrderPair(pd.Name, 0));
                }
            }

            // сортируем по Order-у
            orderedProperties.Sort();

            // формируем список имен свойств
            ArrayList propertyNames = new ArrayList();

            foreach (PropertyOrderPair pop in orderedProperties)
                propertyNames.Add(pop.Name);

            // возвращаем
            return pdc.Sort((string[])propertyNames.ToArray(typeof(string)));
        }

        #endregion
    }

    #region PropertyOrder Attribute

    /// <summary>
    /// Атрибут для задания сортировки
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyOrderAttribute : Attribute
    {
        private int _order;
        public PropertyOrderAttribute(int order)
        {
            _order = order;
        }

        public int Order
        {
            get { return _order; }
        }
    }

    #endregion

    #region PropertyOrderPair

    /// <summary>
    /// Пара имя/номер п/п с сортировкой по номеру
    /// </summary>
    public class PropertyOrderPair : IComparable
    {
        private int _order;
        private string _name;

        public string Name
        {
            get { return _name; }
        }

        public PropertyOrderPair(string name, int order)
        {
            _order = order;
            _name = name;
        }

        /// <summary>
        /// Собственно метод сравнения
        /// </summary>
        public int CompareTo(object obj)
        {
            int otherOrder = ((PropertyOrderPair)obj)._order;

            if (otherOrder == _order)
            {
                // если Order одинаковый - сортируем по именам
                string otherName = ((PropertyOrderPair)obj)._name;
                return string.Compare(_name, otherName);
            }
            else if (otherOrder > _order)
                return -1;

            return 1;
        }
    }

    #endregion

    #endregion


}

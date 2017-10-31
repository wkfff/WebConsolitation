using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Krista.FM.Client.SMO.Design
{
    public abstract class CustomPropertiesTypeConverter<TComponent> : TypeConverter
    {
        #region Fields

        protected static readonly Attribute[] EmptyAttributes = new Attribute[0];

        #endregion Fields

        #region Constructors\Finalizer

        protected CustomPropertiesTypeConverter()
        {
        }

        #endregion Constructors\Finalizer

        #region Methods

        protected abstract Attribute[] GetPropertyAttributes(TComponent component, PropertyDescriptor property);

        #endregion Methods

        #region Overrides

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] filter)
        {
            if (value != null)
            {
                TComponent componet = (TComponent)value;
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, true);
                List<PropertyDescriptor> descriptors = new List<PropertyDescriptor>(properties.Count);
                foreach (PropertyDescriptor property in properties)
                {
                    Attribute[] propertyAttributes = GetPropertyAttributes(componet, property);
                    AttributeCollection attributeCollection = AttributeCollection.FromExisting(property.Attributes, propertyAttributes);
                    if (attributeCollection.Contains(filter))
                    {
                        Attribute[] attrs = new Attribute[attributeCollection.Count];
                        attributeCollection.CopyTo(attrs, 0);
                        descriptors.Add(new CustomPropertyDescriptor(property, attrs));
                    }//if
                }//for
                return new PropertyDescriptorCollection(descriptors.ToArray(), ((IList)properties).IsReadOnly);
            }
            else
            {
            }//if
            return TypeDescriptor.GetProperties(typeof(TComponent), filter, true);
        }

        #endregion Overrides

        /// <summary>
        /// Необходим для настройки (замены) аттрибутов свойства.
        /// </summary>
        private sealed class CustomPropertyDescriptor : SimplePropertyDescriptor
        {
            #region Fields

            private readonly PropertyDescriptor inner;

            #endregion Fields

            #region Constructors\Finalizer

            public CustomPropertyDescriptor(PropertyDescriptor inner, Attribute[] attributes)
                : base(inner != null ? inner.ComponentType : null,
                     inner != null ? inner.Name : null,
                     inner != null ? inner.PropertyType : null,
                     attributes)
            {
                if (inner == null)
                {
                    throw new ArgumentNullException("inner");
                }

                this.inner = inner;
            }

            #endregion Constructors\Finalizer

            #region Properties

            private PropertyDescriptor InnerPropertyDescriptor
            {
                [DebuggerStepThrough]
                get { return inner; }
            }

            #endregion Properties

            #region Overrides

            public override object GetValue(object component)
            {
                return InnerPropertyDescriptor.GetValue(component);
            }

            public override void SetValue(object component, object value)
            {
                InnerPropertyDescriptor.SetValue(component, value);
            }

            #endregion Overrides
        }
    }
}

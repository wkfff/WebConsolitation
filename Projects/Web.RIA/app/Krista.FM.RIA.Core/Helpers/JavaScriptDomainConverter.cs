using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Web.Script.Serialization;

using Krista.FM.Extensions;

namespace Krista.FM.RIA.Core
{
    /// <summary>
    /// Преобразует JSON данные от Ext.Net.Store в модель AjaxStoreSaveDomainModel.
    /// </summary>
    /// <typeparam name="T">Тип объекта доменной модели.</typeparam>
    public class JavaScriptDomainConverter<T> : JavaScriptConverter where T : new()
    {
        private readonly IEnumerable<Type> supportedTypes;

        private JavaScriptDomainConverter(IEnumerable<Type> supportedTypes)
        {
            this.supportedTypes = supportedTypes;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return supportedTypes;
            }
        }

        /// <summary>
        /// Преобразует JSON данные от Ext.Net.Store в модель AjaxStoreSaveDomainModel.
        /// </summary>
        /// <param name="jsonText">JSON пришедший от Ext.Net.Store в запросе на сохранение.</param>
        public static AjaxStoreSaveDomainModel<T> Deserialize(string jsonText)
        {
            var types = new List<Type>();

            types.Add(typeof(T));
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                if (propertyInfo.PropertyType.FullName.StartsWith("Krista.FM.Domain"))
                {
                    types.Add(propertyInfo.PropertyType);
                }
            } 

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new List<JavaScriptConverter> { new JavaScriptDomainConverter<T>(types) });
            return serializer.Deserialize<AjaxStoreSaveDomainModel<T>>(jsonText);
        }

        /// <summary>
        /// Преобразует JSON данные от Ext.Net.Store в модель TDomain.
        /// </summary>
        /// <param name="jsonText">JSON пришедший от Ext.Net.Store в запросе на сохранение.</param>
        public static T DeserializeSingle(string jsonText)
        {
            var types = new List<Type>();

            types.Add(typeof(T));
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                if (propertyInfo.PropertyType.FullName.StartsWith("Krista.FM.Domain"))
                {
                    types.Add(propertyInfo.PropertyType);
                }
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new List<JavaScriptConverter> { new JavaScriptDomainConverter<T>(types) });
            return serializer.Deserialize<T>(jsonText);
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            var o = Activator.CreateInstance(type);

            List<string> list = new List<string>(dictionary.Keys);
            foreach (string memberName in list)
            {
                if (memberName == "Id")
                {
                    continue;
                }

                object propertyValue = dictionary[memberName];
                PropertyInfo property = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property != null)
                {
                    MethodInfo setMethod = property.GetSetMethod();
                    if (setMethod != null)
                    {
                        if (propertyValue is Dictionary<string, object>)
                        {
                            propertyValue = Deserialize((Dictionary<string, object>)propertyValue, property.PropertyType, serializer);
                        }
                        else if (!ConvertObjectToType(propertyValue, property.PropertyType, out propertyValue))
                        {
                            throw new InvalidOperationException(
                                String.Format("Заначение \"{0}\" свойства {1} невозможно преобразовать в тип {2}", propertyValue, memberName, property.PropertyType.FullName));
                        }

                        setMethod.Invoke(o, new[] { propertyValue });
                    }
                }
            }

            return o;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private static bool ConvertObjectToType(object o, Type type, out object convertedObject)
        {
            if (o == null)
            {
                if (type == typeof(char))
                {
                    convertedObject = '\0';
                    return true;
                }

                convertedObject = null;
                return true;
            }

            if (o.GetType() == type)
            {
                convertedObject = o;
                return true;
            }

            if ((type == null) || (o.GetType() == type))
            {
                convertedObject = o;
                return true;
            }

            TypeConverter converter = TypeDescriptor.GetConverter(type);
            if (converter.CanConvertFrom(o.GetType()))
            {
                try
                {
                    convertedObject = converter.ConvertFrom(null, CultureInfo.InvariantCulture, o);
                    return true;
                }
                catch
                {
                    convertedObject = null;
                    return false;
                }
            }

            if (converter.CanConvertFrom(typeof(string)))
            {
                TypeConverter converter2 = TypeDescriptor.GetConverter(o);
                try
                {
                    string text = converter2.ConvertToInvariantString(o);
                    convertedObject = converter.ConvertFromInvariantString(text);
                    return true;
                }
                catch
                {
                    convertedObject = null;
                    return false;
                }
            }

            if (type.IsAssignableFrom(o.GetType()))
            {
                convertedObject = o;
                return true;
            }

            int id;
            if (int.TryParse(Convert.ToString(o), out id) && type.Namespace == "Krista.FM.Domain")
            {
                convertedObject = Activator.CreateInstance(type);
                convertedObject.GetType().InvokeMember(
                    "ID",
                    BindingFlags.SetProperty,
                    null,
                    convertedObject,
                    new object[] { Convert.ToInt32(o) },
                    null);
                return true;
            }
            
            if (type.Namespace == "Krista.FM.Domain" && (o is int || Convert.ToString(o) == String.Empty))
            {
                if (String.IsNullOrEmpty(Convert.ToString(o)))
                {
                    convertedObject = null;
                    return true;
                }
                
                convertedObject = Activator.CreateInstance(type);
                convertedObject.GetType().InvokeMember(
                    "ID",
                    BindingFlags.SetProperty,
                    null,
                    convertedObject,
                    new object[] { Convert.ToInt32(o) },
                    null);
                return true;
            }

            convertedObject = null;
            return false;
        }
    }
}

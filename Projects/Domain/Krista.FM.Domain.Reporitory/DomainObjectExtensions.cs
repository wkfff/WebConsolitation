using System;
using System.Reflection;

namespace Krista.FM.Domain.Reporitory
{
    public static class DomainObjectExtensions
    {
        public static string GetKey(this DomainObject obj)
        {
            Type objectType = obj.GetType();

            // Получаем метаданные объекта по его уникальному ключу
            return Convert.ToString(objectType.InvokeMember("Key",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField,
                null, objectType, null));
        }

        public static void SetPersisted(this DomainObject obj)
        {
            var fieldInfo = FindIsNewField(obj.GetType());
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(obj, false);
            }
        }

        private static FieldInfo FindIsNewField(Type t)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            if (t == typeof(DomainObject))
            {
                foreach (var field in t.GetFields(flags))
                {
                    // Ignore inherited fields.
                    if (field.Name == "isNew")
                        return field;
                }
            }

            var baseType = t.BaseType;
            if (baseType != null)
                return FindIsNewField(baseType);

            return null;
        }

        public static T Copy<T>(this T sourceObj) where T : DomainObject, new()
        {
            if (sourceObj == null)
            {
                throw new ArgumentException("Невозможно скопировать доменный объект, т.к. не задан исходный объект.", "sourceObj");
            }

            T newObj  = new T();

            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                object value = propertyInfo.GetValue(sourceObj, BindingFlags.GetProperty, null, null, null);
                propertyInfo.SetValue(newObj, value, BindingFlags.SetProperty, null, null, null);
            }

            newObj.ID = 0;

            return newObj;
        }
    }
}

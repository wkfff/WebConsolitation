using System;
using System.Collections.Generic;
using System.Reflection;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Common
{
    /// <summary>
    /// Базовый класс для всех сериализуемых объектов сервера
    /// </summary>
    public class SMOSerializable : ServerSideObject, ISMOSerializable
    {
        public SMOSerializable(ServerSideObject owner)
            : base(owner)
        {}

        public SMOSerializable(ServerSideObject owner, ServerSideObjectStates state)
            : base(owner, state)
        {
            
        }

        #region ISMOSerializable Members

        public virtual SMOSerializationInfo GetSMOObjectData()
        {
            SMOSerializationInfo info = new SMOSerializationInfo(new Dictionary<string, object>());

            info.Add("this", this);
            info.Add("Identifier", identifier);

            CollectSMOObjectData(info, this, GetInterfaceType(this));

            return info;
        }

        /// <summary>
        /// Сериализация объекта с учетом глубины сериализации. Применяется для коллекций
        /// </summary>
        /// <param name="levelSerialization"> Глубина сериализации</param>
        /// <returns> Сериализованная коллекция</returns>
        public virtual SMOSerializationInfo GetSMOObjectData(LevelSerialization levelSerialization)
        {
            return GetSMOObjectData();
        }

        /// <summary>
        /// Сериализация конкретного интерфейса
        /// </summary>
        /// <param name="info"></param>
        /// <param name="obj">The object whose property value will be returned</param>
        /// <param name="objectType"></param>
        public void CollectSMOObjectData(SMOSerializationInfo info, object obj, Type objectType)
        {
            // сериализация базовых свойств
            foreach (Type type in objectType.GetInterfaces())
                if (CheckType(type))
                    CollectSMOProperties(type, obj, info);

            CollectSMOProperties(objectType, obj, info);
        }

        private void CollectSMOProperties(Type objectType, object obj, SMOSerializationInfo info)
        {
            foreach (PropertyInfo propertyInfo in objectType.GetProperties())
            {
                // непосредсвенная сериализация свойства
                CollectSMOProperty(obj, info, propertyInfo);
            }
        }

        /// <summary>
        /// Непосредсвенная сериализация свойства
        /// </summary>
        /// <param name="info"></param>
        /// <param name="obj">The object whose property value will be returned</param>
        /// <param name="propertyInfo"></param>
        public void CollectSMOProperty(object obj, SMOSerializationInfo info, PropertyInfo propertyInfo)
        {
            object[] smoSerilizableAttributes = propertyInfo.GetCustomAttributes(typeof(SMOSerializableAttribute), false);

            if (smoSerilizableAttributes.Length != 0)
            {
                SMOSerializableAttribute attribute = (SMOSerializableAttribute)smoSerilizableAttributes[0];

                switch (attribute.ReturnType)
                {
                    case ReturnType.Value:
                        {
                            AddValueType(propertyInfo, info, obj);
                            break;
                        }
                    case ReturnType.Object:
                        {
                            AddObjectType(propertyInfo, attribute, info, obj);
                            break;
                        }
                    case ReturnType.Dictionary:
                        {
                            AddDictionaryType(propertyInfo, attribute, info, obj);
                            break;
                        }
                    default:
                        throw new Exception(string.Format("Тип атрибута {0} не поддерживается", attribute.ReturnType));
                }
            }
        }

        /// <summary>
        /// Проверка на то, что интерфейс реализует интерфейс ISMOSerializable
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool CheckType(Type type)
        {
            if (type == typeof(ISMOSerializable))
                return true;

            foreach (Type baseType in type.GetInterfaces())
            {
                if (baseType == typeof(ISMOSerializable))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Метод возвращает тип интерфейса, в котором реализованы SMO атрибуты
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static Type GetInterfaceType(object obj)
        {
            Type[] interfaces = obj.GetType().GetInterfaces();

            for (int i = interfaces.Length - 1; i >= 0; i-- )
            {
                if (interfaces[i].GetCustomAttributes(typeof(SMOInterfaceAttribute), false).Length != 0)
                    return interfaces[i];
            }

            throw new Exception(String.Format("Тип {0} не реализует ни один из интерфейсов со SMO атрибутами", obj.GetType().ToString()));
        }

        /// <summary>
        /// Сериализация коллекции объектов
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="attribute"></param>
        /// <param name="info"></param>
        /// <param name="obj">The object whose property value will be returned</param>
        private static void AddDictionaryType(PropertyInfo propertyInfo, SMOSerializableAttribute attribute, SMOSerializationInfo info, object obj)
        {
            try
            {
                if (attribute.LevelSerialization == LevelSerialization.None)
                    info.Add(propertyInfo.Name, propertyInfo.GetValue(obj, null));
                else
                {
                    ISMOSerializable smoObj = (ISMOSerializable)propertyInfo.GetValue(obj, null);

                    info.Add(propertyInfo.Name, smoObj.GetSMOObjectData(attribute.LevelSerialization));
                }
            }
            catch (TargetException te)
            {
                throw new TargetException(
                    String.Format(
                        "Объект не может быть приведен к целевому типу, или объект, для которого вызвано свойство не существует. {0}",
                        te.Message));
            }
            catch (Exception e)
            {
                info.Add(propertyInfo.Name, e);
            }
        }

        /// <summary>
        /// Сериализация типа-значения
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="info"></param>
        /// <param name="obj">The object whose property value will be returned</param>
        private void AddValueType(PropertyInfo propertyInfo, SMOSerializationInfo info, object obj)
        {
            try
            {
                info.Add(propertyInfo.Name, propertyInfo.GetValue(obj, null));
            }
            catch(Exception e)
            {
                info.Add(propertyInfo.Name, e);
            }
        }

        /// <summary>
        /// Сериализация типа-ссылки
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="attribute"></param>
        /// <param name="info"></param>
        private static void AddObjectType(PropertyInfo propertyInfo, SMOSerializableAttribute attribute, SMOSerializationInfo info, object obj)
        {
            try
            {
                if (attribute.LevelSerialization == LevelSerialization.None)
                    info.Add(propertyInfo.Name, propertyInfo.GetValue(obj, null));
                else
                {
                    if (propertyInfo.GetValue(obj, null) is ISMOSerializable)
                    {
                        SMOSerializable smoObj = (SMOSerializable)propertyInfo.GetValue(obj, null);
                        // записываем коллекцию объектов
                        info.Add(propertyInfo.Name, smoObj.GetSMOObjectData());
                    }
                }
            }
            catch (TargetException te)
            {
                throw new TargetException(String.Format("Объект не может быть приведен к целевому типу, или объект, для которого вызвано свойство не существует. {0}", te.Message));
            }
            catch (Exception e)
            {
                info.Add(propertyInfo.Name, e);
            }
        }

        #endregion
    }
}

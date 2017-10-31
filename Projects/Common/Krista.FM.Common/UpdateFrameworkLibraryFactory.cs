using System;
using System.IO;
using System.Reflection;

namespace Krista.FM.Common
{
    public abstract class UpdateFrameworkLibraryFactory
    {
        #region Загрузка сборки для автоматического обновления

        private static Type theSingletonType;

        private static Type LoadInstance()
        {
            Assembly asm;
            string location = Path.GetDirectoryName(typeof(UpdateFrameworkLibraryFactory).Assembly.Location);
            string path = Path.Combine(location,
                                       String.Format("Krista.FM.Update.Framework.dll"));

            if (!File.Exists(path))
            {
                // Работаем без автообновления
                return null;
            }

            asm = Assembly.LoadFrom(path);

            Type[] types = asm.GetTypes();
            Type theSingletonType = null;
            foreach (Type ty in types)
            {
                if (ty.FullName.Equals("Krista.FM.Update.Framework.UpdateManager"))
                {
                    theSingletonType = ty;
                    break;
                }
            }

            if (theSingletonType == null)
            {
                return null;
            }

            return theSingletonType;
        }

        public static void InvokeMethod(string methodName)
        {
            if (theSingletonType == null)
            {
                theSingletonType = LoadInstance();
            }

            if (theSingletonType != null)
            {
                try
                {
                    MethodInfo method = theSingletonType.GetMethod(methodName);

                    PropertyInfo property = theSingletonType.GetProperty("Instance",
                           BindingFlags.Static | BindingFlags.Public);

                    object instance = property.GetValue(null, null);

                    method.Invoke(instance, null);
                }
                catch (AmbiguousMatchException e)
                {
                    Trace.TraceError(String.Format("При выполнении метода {0} возникло исключение {1}", methodName,
                                                   e.Message));
                }
                catch (ArgumentNullException e)
                {
                    Trace.TraceError(String.Format("При выполнении метода {0} возникло исключение {1}", methodName,
                                                   e.Message));
                }
                catch (TargetException e)
                {
                    Trace.TraceError(String.Format("При выполнении метода {0} возникло исключение {1}", methodName,
                                                   e.Message));
                }
                catch (TargetInvocationException e)
                {
                    Trace.TraceError(String.Format("При выполнении метода {0} возникло исключение {1}", methodName,
                                                   e.Message));
                }
                catch (TargetParameterCountException e)
                {
                    Trace.TraceError(String.Format("При выполнении метода {0} возникло исключение {1}", methodName,
                                                   e.Message));
                }
                catch (MemberAccessException e)
                {
                    Trace.TraceError(String.Format("При выполнении метода {0} возникло исключение {1}", methodName,
                                                   e.Message));
                }
                catch (InvalidOperationException e)
                {
                    Trace.TraceError(String.Format("При выполнении метода {0} возникло исключение {1}", methodName,
                                                   e.Message));
                }
            }
        }

        public static void SetPropertyValue(string propertyName, object propertyValue)
        {
            if (theSingletonType == null)
            {
                theSingletonType = LoadInstance();
            }

            if (theSingletonType != null)
            {
                try
                {
                    PropertyInfo property = theSingletonType.GetProperty("Instance",
                           BindingFlags.Static | BindingFlags.Public);

                    object instance = property.GetValue(null, null);

                    theSingletonType.GetProperty(propertyName).SetValue(instance, propertyValue, null);
                }
                catch (AmbiguousMatchException e)
                {
                    Trace.TraceError(String.Format("При установке значения свойства {0} возникло исключение {1}",
                                                   propertyName,
                                                   e.Message));
                }
                catch (ArgumentNullException e)
                {
                    Trace.TraceError(String.Format("При установке значения свойства {0} возникло исключение {1}",
                                                   propertyName,
                                                   e.Message));
                }
                catch (TargetException e)
                {
                    Trace.TraceError(String.Format("При установке значения свойства {0} возникло исключение {1}",
                                                   propertyName,
                                                   e.Message));
                }
                catch (TargetInvocationException e)
                {
                    Trace.TraceError(String.Format("При установке значения свойства {0} возникло исключение {1}",
                                                   propertyName,
                                                   e.Message));
                }
                catch (TargetParameterCountException e)
                {
                    Trace.TraceError(String.Format("При установке значения свойства {0} возникло исключение {1}",
                                                   propertyName,
                                                   e.Message));
                }
            }
        }

        #endregion
    }
}
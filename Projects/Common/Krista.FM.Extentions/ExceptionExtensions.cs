using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Krista.FM.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Разворачивает содержание исколючительной ситуации в нестролько строк
        /// с учетом InnerException
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string ExpandException(this Exception e)
        {
            return e.ExpandException(new PlainExceptionFormatter());
        }

        /// <summary>
        /// Разворачивает содержание исколючительной ситуации в нестролько строк
        /// с учетом InnerException
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string ExpandException(this Exception e, ExceptionFormatter formatter)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Exception scannedExeption = e;
            Exception ex = new Exception(e.Message, e.InnerException);

            BooleanSwitch stackTraceSwitch = new BooleanSwitch("StackTrace", "", "true");

            formatter.WriteBegin(stringBuilder);
            while (scannedExeption != null)
            {
                try
                {
                    formatter.WriteExceptionBegin(stringBuilder);

                    // Для удобочитаемоси выделяем тип исключения
                    formatter.ExceptionType(stringBuilder, scannedExeption.GetType().ToString());

                    PropertyInfo[] props = scannedExeption.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                    if (props.Length > 0)
                    {
                        formatter.WriteExceptionPropertiesBegin(stringBuilder);
                        foreach (PropertyInfo pi in props)
                        {
                            // Выводим значения не только строковых полей, но и знечения других полезных свойств-значений.
                            if (pi.PropertyType.IsAssignableFrom(typeof (string)) || pi.PropertyType.IsValueType)
                            {
                                try
                                {
                                    if (stackTraceSwitch.Enabled || (pi.Name != "StackTrace"))
                                    {
                                        object value = pi.GetValue(scannedExeption, null);
                                        formatter.WriteProperty(stringBuilder, pi.Name, value);
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }

                        formatter.WriteExceptionPropertiesEnd(stringBuilder);
                    }

                    if (scannedExeption.Data != null && scannedExeption.Data.Count > 0)
                    {
                        formatter.WriteExeptionDataBegin(stringBuilder);
                        foreach (DictionaryEntry dataItem in scannedExeption.Data)
                        {
                            formatter.WriteExeptionDataItem(stringBuilder, dataItem.Key, dataItem.Value);
                        }

                        formatter.WriteExeptionDataEnd(stringBuilder);
                    }
                }
                catch (Exception oops)
                {
                    formatter.WriteOops(stringBuilder, oops);
                }
                
                scannedExeption = scannedExeption.InnerException;

                // Добавляем разрыв для отделения вложенных исключений друг от друга.
                formatter.WriteExceptionEnd(stringBuilder);
            }
            
            try
            {
                if (ex.TargetSite != null)
                {
                    formatter.WriteTargetSite(stringBuilder, ex.TargetSite);
                }
            }
            catch
            {
                // Здесь иногда водникает исключение - просто глушим его.
            }

            formatter.WriteEnd(stringBuilder);
            
            return stringBuilder.ToString();
        }

        public abstract class ExceptionFormatter
        {
            public abstract void WriteBegin(StringBuilder stringBuilder);
            
            public abstract void WriteEnd(StringBuilder stringBuilder);

            public abstract void WriteExceptionBegin(StringBuilder stringBuilder);

            public abstract void WriteExceptionEnd(StringBuilder stringBuilder);
            
            public abstract void ExceptionType(StringBuilder stringBuilder, string toString);

            public abstract void WriteExceptionPropertiesBegin(StringBuilder stringBuilder);
            
            public abstract void WriteProperty(StringBuilder stringBuilder, string name, object value);

            public abstract void WriteExceptionPropertiesEnd(StringBuilder stringBuilder);
            
            public abstract void WriteExeptionDataBegin(StringBuilder stringBuilder);
            
            public abstract void WriteExeptionDataItem(StringBuilder stringBuilder, object key, object value);

            public abstract void WriteExeptionDataEnd(StringBuilder stringBuilder);

            public abstract void WriteTargetSite(StringBuilder stringBuilder, MethodBase targetSite);

            public abstract void WriteOops(StringBuilder stringBuilder, Exception exception);
        }

        public class PlainExceptionFormatter : ExceptionFormatter
        {
            public override void WriteBegin(StringBuilder stringBuilder)
            {
            }

            public override void WriteEnd(StringBuilder stringBuilder)
            {
            }

            public override void WriteExceptionBegin(StringBuilder stringBuilder)
            {
            }

            public override void WriteExceptionEnd(StringBuilder stringBuilder)
            {
                stringBuilder.AppendLine();
            }

            public override void ExceptionType(StringBuilder stringBuilder, string exceptionType)
            {
                stringBuilder.Append("---------- ");
                stringBuilder.Append(exceptionType);
                stringBuilder.AppendLine(" ----------");
            }

            public override void WriteExceptionPropertiesBegin(StringBuilder stringBuilder)
            {
            }

            public override void WriteProperty(StringBuilder stringBuilder, string name, object value)
            {
                stringBuilder.AppendLine(String.Format("{0}={1}", name, (value == null ? String.Empty : value.ToString())));
            }

            public override void WriteExceptionPropertiesEnd(StringBuilder stringBuilder)
            {
            }

            public override void WriteExeptionDataBegin(StringBuilder stringBuilder)
            {
                stringBuilder.AppendLine("Data items:");
            }

            public override void WriteExeptionDataItem(StringBuilder stringBuilder, object key, object value)
            {
                stringBuilder.AppendLine(String.Format("  key: {0}, value: {1}", key, value));
            }

            public override void WriteExeptionDataEnd(StringBuilder stringBuilder)
            {
            }

            public override void WriteTargetSite(StringBuilder stringBuilder, MethodBase targetSite)
            {
                stringBuilder.Append("Метод:" + targetSite.ReflectedType + "." + targetSite.Name);
            }

            public override void WriteOops(StringBuilder stringBuilder, Exception exception)
            {
                stringBuilder.AppendLine(String.Format("********** Исключительная ситуация при обработке исключительной ситуации. *********** {0}: {1}", exception.GetType(), exception.Message));
            }
        }
    }
}

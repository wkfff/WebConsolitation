using System;
using System.Reflection;
using System.Text;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Core.Helpers
{
    public class HtmlExceptionFormatter : ExceptionExtensions.ExceptionFormatter
    {
        private const string Break = "<br/>";

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
            stringBuilder.Append(Break);
        }

        public override void ExceptionType(StringBuilder stringBuilder, string exceptionType)
        {
            stringBuilder.Append("---------- ");
            stringBuilder.Append(exceptionType);
            stringBuilder.Append(" ----------");
            stringBuilder.Append(Break);
        }

        public override void WriteExceptionPropertiesBegin(StringBuilder stringBuilder)
        {
        }

        public override void WriteProperty(StringBuilder stringBuilder, string name, object value)
        {
            string stringValue;
            if (value != null)
            {
                stringValue = Convert.ToString(value);
                stringValue = stringValue.Replace(Environment.NewLine, Break);
            }
            else
            {
                stringValue = String.Empty;
            }

            stringBuilder.Append(String.Format("{0}={1}", name, stringValue));
            stringBuilder.Append(Break);
        }

        public override void WriteExceptionPropertiesEnd(StringBuilder stringBuilder)
        {
        }

        public override void WriteExeptionDataBegin(StringBuilder stringBuilder)
        {
            stringBuilder.Append("Data items:");
            stringBuilder.Append(Break);
        }

        public override void WriteExeptionDataItem(StringBuilder stringBuilder, object key, object value)
        {
            stringBuilder.Append(String.Format("  key: {0}, value: {1}", key, value));
            stringBuilder.Append(Break);
        }

        public override void WriteExeptionDataEnd(StringBuilder stringBuilder)
        {
        }

        public override void WriteTargetSite(StringBuilder stringBuilder, MethodBase targetSite)
        {
            stringBuilder.Append("Метод:" + targetSite.ReflectedType + "." + targetSite.Name);
            stringBuilder.Append(Break);
        }

        public override void WriteOops(StringBuilder stringBuilder, Exception exception)
        {
            stringBuilder.Append(String.Format("********** Исключительная ситуация при обработке исключительной ситуации. *********** {0}: {1}", exception.GetType(), exception.Message));
            stringBuilder.Append(Break);
        }
    }
}

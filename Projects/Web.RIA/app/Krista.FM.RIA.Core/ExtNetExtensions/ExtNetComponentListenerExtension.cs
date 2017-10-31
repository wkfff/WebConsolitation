using System.Text;
using Ext.Net;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Core
{
    public static class ExtNetComponentListenerExtension
    {
        /// <summary>
        /// Добавляет обработчик перед существующим обработчиком.
        /// </summary>
        public static ComponentListener AddBefore(this ComponentListener listener, string handler)
        {
            if (listener.Handler.IsNullOrEmpty())
            {
                listener.Handler = handler;
                return listener;
            }

            StringBuilder sb = new StringBuilder()
                .AppendLine(handler)
                .Append(listener.Handler);

            listener.Handler = sb.ToString();
            return listener;
        }

        /// <summary>
        /// Добавляет обработчик после существующего обработчика.
        /// </summary>
        public static ComponentListener AddAfter(this ComponentListener listener, string handler)
        {
            if (listener.Handler.IsNullOrEmpty())
            {
                listener.Handler = handler;
                return listener;
            }

            StringBuilder sb = new StringBuilder()
                .AppendLine(listener.Handler)
                .Append(handler);

            listener.Handler = sb.ToString();
            return listener;
        }

        public static ComponentListener Fn(this ComponentListener listener, string scope, string fn)
        {
            listener.Fn = scope + '.' + fn;
            listener.Scope = scope;

            return listener;
        }
    }
}

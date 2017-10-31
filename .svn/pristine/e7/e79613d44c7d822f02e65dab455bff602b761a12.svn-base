using System;
using System.Web;

namespace Krista.FM.RIA.Core
{
    public static class FirstRequestInitialization
    {
        private static readonly object objLock = new object();
        private static string host;
        
        public static string Host
        {
            get { return host; }
        }

        public static string Initialize(HttpContext context)
        {
            if (string.IsNullOrEmpty(host))
            {
                lock (objLock)
                {
                    if (string.IsNullOrEmpty(host))
                    {
                        Uri uri = HttpContext.Current.Request.Url;
                        host = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
                    }
                }
            }

            return host;
        }
    }
}
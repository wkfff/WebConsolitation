using System;
using System.IO;
using System.Web;
using Ext.Net;

namespace Krista.FM.RIA.Core
{
    public static class FileHelper
    {
        public static string GetDownloadableFileName(string name)
        {
            if (RequestManager.IsIE)
            {
                string urlEncode = HttpContext.Current.Server.UrlEncode(name);
                if (urlEncode != null)
                {
                    return urlEncode.Replace("+", " ");
                }
            }

            return name;
        }

        /// <summary>
        /// Возвращает путь к каталогу с временными файлами.
        /// </summary>
        public static string GetTempPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
        }
    }
}

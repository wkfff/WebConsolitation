using System;
using System.IO;
using System.Text;
using System.Web;

namespace Krista.FM.Server.Dashboards.Core.HttpModules
{
    public class HTTPModuleScriptCleaner : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PostResolveRequestCache += new EventHandler(PostResolveRequestCache);
        }

        public void Dispose()
        {

        }

        private static void PostResolveRequestCache(object sender, EventArgs e)
        {
            string resourcePath = ResourcePath();
            if (string.IsNullOrEmpty(resourcePath) &&
                HttpContext.Current.Response.ContentType == "text/html" &&
                HttpContext.Current.Request.Url.LocalPath.ToLower().Contains(".axd"))
            {
                HttpContext.Current.Response.End();
            }
            if (string.IsNullOrEmpty(resourcePath))
            {
                return;
            }
            StreamReader sr = new StreamReader(resourcePath);
            string script = sr.ReadToEnd();
            byte[] Output = ResourceCompressor.GetOutput(script, Encoding.ASCII);
            // Отправим клиенту
            SendOutput(Output);
        }

        private static string ResourcePath()
        {
            string resourcePath = String.Empty;
            try
            {
                HttpRequest Request = HttpContext.Current.Request;
                string appPath = Request.PhysicalApplicationPath;
                foreach (string resourceName in ScriptPatterns.PatternResourceDictionary.Values)
                {
                    if (Request.Url.LocalPath.ToLower().Contains(string.Format("{0}.axd", resourceName.ToLower())))
                    {
                        resourcePath = string.Format("{0}CustomResources\\{1}.js", appPath, resourceName);
                    }
                }
            }
            catch (NullReferenceException)
            {
                return String.Empty;
            }
            return resourcePath;
        }

        /// <summary>
        /// Отправляет контент клиенту.
        /// </summary>
        /// <param name="Output">Массив байт, который нужно отправить.</param>
        private static void SendOutput(byte[] Output)
        {
            HttpResponse Response = HttpContext.Current.Response;
            Response.ContentType = "application/x-javascript";

            if (!HttpContext.Current.IsDebuggingEnabled)
            {
                Response.ExpiresAbsolute = DateTime.UtcNow.AddDays(1);
                Response.Cache.SetLastModified(DateTime.UtcNow);
                Response.Cache.SetCacheability(HttpCacheability.Public);
            }

            Response.BinaryWrite(Output);
            Response.End();
        }
    }
}

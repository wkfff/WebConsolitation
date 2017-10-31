using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using Ext.Net.Utilities;

namespace Krista.FM.RIA.Core
{
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    internal class ResourceManager : Page, IHttpHandler, IRequiresSessionState
    {
        private static readonly Dictionary<string, Assembly> nameAssemblyCache = new Dictionary<string, Assembly>(StringComparer.InvariantCultureIgnoreCase);

        public override void ProcessRequest(HttpContext context)
        {
            // url файла, для которого создается "метафора"
            string file = context.Request.RawUrl;

            // имя web-ресурса
            string webResource = GetWebResourceName(file);

            try
            {
#if DEBUG
                WriteNoCacheFileResource(context, file, webResource);
                context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
#else
                WriteEmbeddedResource(context, file, webResource);
#endif
            }
            catch (Exception)
            {
                var redirect = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), webResource);
                context.Response.Redirect(redirect);
            }
        }

        private static void WriteNoCacheFileResource(HttpContext context, string file, string webResource)
        {
            using (var stream = GetFile(file))
            {
                WriteResponce(context, webResource, stream);
            }
        }

        private static void WriteEmbeddedResource(HttpContext context, string file, string webResource)
        {
            SetResponseCache(context);

            Assembly assembly = GetAssembly(file);

            using (var stream = assembly.GetManifestResourceStream(webResource))
            {
                WriteResponce(context, webResource, stream);
            }
        }

        private static void WriteResponce(HttpContext context, string webResource, Stream stream)
        {
            // определяем тип ресурса
            switch (StringUtils.RightOfRightmostOf(webResource, '.'))
            {
                case "js":
                    WriteFile("text/javascript", stream, context, webResource);
                    break;
                case "css":
                    WriteFile("text/css", stream, context, webResource);
                    break;
                case "gif":
                    WriteImage("image/gif", stream, context, webResource);
                    break;
                case "png":
                    WriteImage("image/png", stream, context, webResource);
                    break;
                case "jpg":
                case "jpeg":
                    WriteImage("image/jpg", stream, context, webResource);
                    break;
            }
        }

        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        private static long GetAssemblyTime(Assembly assembly)
        {
            AssemblyName assemblyName = assembly.GetName();
            return File.GetLastWriteTime(new Uri(assemblyName.CodeBase).LocalPath).Ticks;
        }

        private static Stream GetFile(string file)
        {
            var appPath = HttpContext.Current.Server.MapPath("/");

            var projectFolder = appPath.Replace("\\Krista.FM.RIA.Web\\", String.Empty);
            var localPath = StringUtils.LeftOfRightmostOf(file, "/extention.axd").Remove(0, 1).Replace('/', '\\');
            var filePath = Path.Combine(projectFolder, localPath);

            return File.OpenRead(filePath);
        }

        private static Assembly GetAssembly(string file)
        {
            string[] parts = file.Split(new[] { '/' }, 3);

            // TODO: should assert and sanitize 'parts' first
            string assemblyName = parts[1] + ".dll";

            Assembly assembly;
            lock (nameAssemblyCache)
            {
                if (!nameAssemblyCache.TryGetValue(assemblyName, out assembly))
                {
                    var assemblyPath = Path.Combine(HttpRuntime.BinDirectory, assemblyName);
                    assembly = Assembly.LoadFrom(assemblyPath);

                    // TODO: Assert is not null
                    nameAssemblyCache[assemblyName] = assembly;
                }
            }

            return assembly;
        }

        private static void SetResponseCache(HttpContext context)
        {
            HttpCachePolicy cache = context.Response.Cache;

            cache.SetLastModified(new DateTime(GetAssemblyTime(typeof(ResourceManager).Assembly)));
            cache.SetOmitVaryStar(true);
            cache.SetVaryByCustom("v");
            cache.SetExpires(DateTime.UtcNow.AddYears(1));
            cache.SetMaxAge(TimeSpan.FromDays(365));
            cache.SetValidUntilExpires(true);
            cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            cache.SetCacheability(HttpCacheability.Public);
        }

        private static void WriteFile(string responseType, Stream stream, HttpContext context, string webResource)
        {
#if !DEBUG
            var isGZip = CompressionUtils.IsGZipSupported;
            var output = GetCache(context, webResource, isGZip);

            if (output != null)
            {
                if (isGZip)
                {
                    HttpResponse response = context.Response;
                    response.AppendHeader("Content-Encoding", "gzip");
                    response.Charset = "utf-8";
                }

                WriteResponce(output, context, responseType);
                
                return;
            }
#endif

            WriteAndCacheCompressedResponce(stream, context, webResource, responseType, true);
        }

        private static void WriteImage(string responseType, Stream stream, HttpContext context, string webResource)
        {
            var output = GetCache(context, webResource, false);

            if (output != null)
            {
                WriteResponce(output, context, responseType);
                return;
            }

            WriteAndCacheCompressedResponce(stream, context, webResource, responseType, false);
        }

        private static void WriteAndCacheCompressedResponce(Stream stream, HttpContext context, string webResource, string responseType, bool compress)
        {
            var length = Convert.ToInt32(stream.Length);
            var content = new byte[length];
            stream.Read(content, 0, length);

            WriteAndCacheCompressedResponce(content, context, webResource, responseType, compress);
        }

        private static void WriteAndCacheCompressedResponce(byte[] content, HttpContext context, string webResource, string responseType, bool compress)
        {
            byte[] data = content;
            HttpResponse response = context.Response;

            if (compress && CompressionUtils.IsGZipSupported)
            {
                data = CompressionUtils.GZip(content);

                response.AppendHeader("Content-Encoding", "gzip");
                response.Charset = "utf-8";

                SetCache(data, context, webResource, true);
            }
            else
            {
                SetCache(data, context, webResource, false);
            }

            response.ContentType = responseType;
            response.BinaryWrite(data);
        }

        private static void WriteResponce(byte[] content, HttpContext context, string responseType)
        {
            HttpResponse response = context.Response;

            response.ContentType = responseType;
            response.BinaryWrite(content);
        }

        private static byte[] GetCache(HttpContext context, string webResource, bool gzip)
        {
            if (gzip)
            {
                webResource += ".gzip";
            }

            return context.Cache[webResource] as byte[];
        }

        private static void SetCache(byte[] output, HttpContext context, string webResource, bool gzip)
        {
            if (gzip)
            {
                webResource += ".gzip";
            }

            context.Cache.Insert(webResource, output, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromDays(30));
        }

        private static string GetWebResourceName(string filePath)
        {
            // /Krista.FM.RIA.Extensions.Tasks/Presentation/js/TaskNav.Index.js/extention.axd
            // /Krista.FM.RIA.Extensions.Entity/Presentation/js/Entity.Index.js/extention.axd
            string[] parts = filePath.Split(new[] { '/' }, 3);

            StringBuilder sb = new StringBuilder(100);
            sb.Append(parts[1])
                .Append('.')
                .Append(StringUtils.LeftOfRightmostOf(parts[2], "/extention.axd").Replace('/', '.'));

            return sb.ToString();
        }
    }
}

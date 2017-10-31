using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Core
{
    public static class ResourceRegister
    {
        private static readonly Dictionary<string, string> assembliesVersions = new Dictionary<string, string>();
        private static Assembly[] executedAssemblies;

        /// <summary>
        /// Форпирует url скрипта с маркером версии ресурса.
        /// </summary>
        public static string Script(string url)
        {
            return AddVersionMarker(url);
        }

        /// <summary>
        /// Форпирует url стиля с маркером версии ресурса.
        /// </summary>
        public static string Style(string url)
        {
            return AddVersionMarker(url);
        }

        /// <summary>
        /// Регистрирует скрипт с маркером версии ресурса.
        /// </summary>
        public static void RegisterScript(this Ext.Net.ResourceManager resourceManager, string key, string url)
        {
            resourceManager.RegisterClientScriptInclude(key, AddVersionMarker(url));
        }

        /// <summary>
        /// Добавляет на страницу скрипт из ресурсов сборки.
        /// </summary>
        /// <typeparam name="T">Класс представления смежный со встроенным скриптом.</typeparam>
        public static void RegisterEmbeddedScript<T>(this Ext.Net.ResourceManager resourceManager, string resourceName)
        {
            var res = typeof(T).Namespace + '.' + resourceName;
            resourceManager.RegisterClientScriptBlock(resourceName, GetEmbeddedResource(res, typeof(T)));
        }

        /// <summary>
        /// Добавляет на страницу тили из ресурсов сборки.
        /// </summary>
        /// <typeparam name="T">Класс представления смежный со встроенными стилями.</typeparam>
        public static void RegisterEmbeddedStyle<T>(this Ext.Net.ResourceManager resourceManager, string resourceName)
        {
            var res = typeof(T).Namespace + '.' + resourceName;
            resourceManager.RegisterClientStyleBlock(resourceName, GetEmbeddedResource(res, typeof(T)));
        }

        /// <summary>
        /// Регистрирует стиль с маркером версии ресурса.
        /// </summary>
        public static void RegisterStyle(this Ext.Net.ResourceManager resourceManager, string key, string url)
        {
            resourceManager.RegisterClientStyleInclude(key, AddVersionMarker(url));
        }

        /// <summary>
        /// Сброс кеша версий сборок.
        /// </summary>
        public static void ResetCache()
        {
            executedAssemblies = null;
            assembliesVersions.Clear();
        }

        private static string AddVersionMarker(string url)
        {
            string assemblyName;

            var parts = url.Split(new[] { '/' });
            var lastPart = parts[parts.GetLength(0) - 1];
            if (lastPart == "extention.axd")
            {
                assemblyName = parts[1] + ".DLL";
            }
            else if (lastPart.StartsWith("ext.axd"))
            {
                assemblyName = String.Empty;
            }
            else
            {
                assemblyName = "Krista.FM.RIA.DLL";
            }

            string version = GetAssemblyVersion(assemblyName);
            if (version.IsNullOrEmpty())
            {
                return url;
            }

            return "{0}?v={1}".FormatWith(url, version);
        }

        private static string GetAssemblyVersion(string assemblyName)
        {
            if (executedAssemblies == null)
            {
                executedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            }

            string version;
            if (assembliesVersions.TryGetValue(assemblyName, out version))
            {
                return version;
            }

            Assembly assembly = executedAssemblies
                .Where(x => x.ManifestModule.Name.ToUpper() == assemblyName.ToUpper())
                .FirstOrDefault();

            version = assembly == null 
                ? String.Empty 
                : new AssemblyName(assembly.FullName).Version.ToString();

            assembliesVersions.Add(assemblyName, version);

            return version;
        }

        private static string GetEmbeddedResource(string res, Type type)
        {
#if DEBUG
            // TODO: Реализовать кеш ресурсов и монитор файловой системы
            var appPath = HttpContext.Current.Server.MapPath("/");

            var ns = type.Assembly.GetName().Name;
            var projPath = appPath.Replace("Krista.FM.RIA.Web", ns);
            var resPath = res.Replace(ns + '.', String.Empty).Split(new[] { '.' });

            var file = projPath;
            for (int i = 0; i < resPath.Length - 1; i++)
            {
                file = Path.Combine(file, resPath[i]);
            }

            file = file + '.' + resPath.Last();

            using (var f = File.OpenText(file))
            {
                return f.ReadToEnd();
            }
#else
            return new StreamReader(type.Assembly.GetManifestResourceStream(res)).ReadToEnd();
#endif
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace Krista.FM.RIA.Core
{
    // From http://www.codeproject.com/KB/aspnet/ASP2UserControlLibrary.aspx
    public class AssemblyResourceVirtualPathProvider : VirtualPathProvider
    {
        /// <summary>
        /// Имя каталога виртуальных ресурсов.
        /// </summary>
        public const string AppResourceName = "~/App_Resource/";

        private static readonly Dictionary<string, Assembly> NameAssemblyCache = new Dictionary<string, Assembly>(StringComparer.InvariantCultureIgnoreCase);
        
        private static readonly Dictionary<string, object> ResourcesCache = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

        public override bool FileExists(string virtualPath)
        {
            if (IsAppResourcePath(virtualPath))
            {
                string path = VirtualPathUtility.ToAppRelative(virtualPath);
                string[] parts = path.Split(new[] { '/' }, 4);
                string assemblyFileName = parts[2];
                string resourceName = parts[3].Replace('/', '.');

                Assembly assembly;
                lock (NameAssemblyCache)
                {
                    if (!NameAssemblyCache.TryGetValue(assemblyFileName, out assembly))
                    {
                        var assemblyName = assemblyFileName.Substring(0, assemblyFileName.Length - 4);
                        assembly = AppDomain.CurrentDomain.GetAssemblies()
                            .FirstOrDefault(x => x.FullName.StartsWith(assemblyName));
                        if (assembly != null)
                        {
                            NameAssemblyCache[assemblyFileName] = assembly;
                        }
                    }
                }

                if (assembly != null)
                {
                    bool found;
                    lock (ResourcesCache)
                    {
                        if (ResourcesCache.ContainsKey(resourceName))
                        {
                            return true;
                        }

                        string[] resourceList = assembly.GetManifestResourceNames();
                        found = Array.Exists(resourceList, r => r.Equals(resourceName));
                        if (found)
                        {
                            ResourcesCache.Add(resourceName, null);
                        }
                    }

                    return found;
                }

                return false;
            }
            
            return base.FileExists(virtualPath);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            Trace.TraceVerbose("GetFile {0}", virtualPath);
            
            if (IsAppResourcePath(virtualPath))
            {
                Trace.TraceVerbose("GetFile AssemblyResourceFile {0}", virtualPath);
                return new AssemblyResourceFile(NameAssemblyCache, virtualPath);
            }

            return base.GetFile(virtualPath);
        }

        public override CacheDependency GetCacheDependency(
            string virtualPath,
            IEnumerable virtualPathDependencies,
            DateTime utcStart)
        {
            if (IsAppResourcePath(virtualPath))
            {
                // ~/App_Resource/WikiExtension.dll/WikiExtension/Presentation/Views/Wiki/Index.aspx
                string[] parts = virtualPath.Split(new[] { '/' }, 4);
                string assemblyName = parts[2];
                return new CacheDependency(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.Combine("bin", assemblyName)));
            }

            return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        private bool IsAppResourcePath(string virtualPath)
        {
            string checkPath = VirtualPathUtility.ToAppRelative(virtualPath);

            bool res = checkPath.StartsWith(AppResourceName, StringComparison.InvariantCultureIgnoreCase);

            Trace.TraceVerbose("IsAppResourcePath {0} {1}", res, virtualPath);

            return res;
        }

        private class AssemblyResourceFile : VirtualFile
        {
            private readonly IDictionary<string, Assembly> nameAssemblyCache;
            private readonly string assemblyPath;

            public AssemblyResourceFile(IDictionary<string, Assembly> nameAssemblyCache, string virtualPath) :
                base(virtualPath)
            {
                this.nameAssemblyCache = nameAssemblyCache;
                assemblyPath = VirtualPathUtility.ToAppRelative(virtualPath);
            }

            public override Stream Open()
            {
                Trace.TraceVerbose("Open {0}", assemblyPath);

                // ~/App_Resource/WikiExtension.dll/WikiExtension/Presentation/Views/Wiki/Index.aspx
                string[] parts = assemblyPath.Split(new[] { '/' }, 4);

                // TODO: should assert and sanitize 'parts' first
                string assemblyName = parts[2];
                string resourceName = parts[3].Replace('/', '.');

                Assembly assembly;

                lock (nameAssemblyCache)
                {
                    if (!nameAssemblyCache.TryGetValue(assemblyName, out assembly))
                    {
                        assembly = Assembly.LoadFrom(
                            Path.Combine(HttpRuntime.BinDirectory, assemblyName));

                        // TODO: Assert is not null
                        nameAssemblyCache[assemblyName] = assembly;
                    }
                }

                Stream resourceStream = null;

                if (assembly != null)
                {
                    resourceStream = assembly.GetManifestResourceStream(resourceName);
                }

                return resourceStream;
            }
        }

        private class AssemblyResourceDirectory : VirtualDirectory
        {
            public AssemblyResourceDirectory(string virtualDir)
                : base(virtualDir)
            {
            }

            public override IEnumerable Children
            {
                get { return new List<string>(); }
            }

            public override IEnumerable Directories
            {
                get { return new List<string>(); }
            }

            public override IEnumerable Files
            {
                get { return new List<string>(); }
            }
        }
    }
}
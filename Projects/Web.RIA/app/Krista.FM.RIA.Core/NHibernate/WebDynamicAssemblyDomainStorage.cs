using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using Krista.FM.Domain.Reporitory.NHibernate.ConfigurationCache;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Core.NHibernate
{
    /// <summary>
    /// Кешируемое на диске хранилище динамически генерируемых доменных сборок.
    /// </summary>
    public class WebDynamicAssemblyDomainStorage : IDynamicAssemblyDomainStorage
    {
        private readonly string dir;

        private Dictionary<string, Assembly> cache;

        public WebDynamicAssemblyDomainStorage(string dir)
        {
            this.dir = dir;
        }

        /// <summary>
        /// Возвращает сборку из хранилища.
        /// </summary>
        /// <param name="name">Полное имя сборки (FullName).</param>
        public Assembly Get(string name)
        {
            if (cache != null)
            {
                return cache.Where(x => x.Value.FullName == name).Select(x => x.Value).Single();
            }

            return GetAll().FirstOrDefault(x => x.FullName == name);
        }

        /// <summary>
        /// Возвращает все сборки из хранилища.
        /// </summary>
        public Assembly[] GetAll()
        {
            return GetCache().Select(x => x.Value).ToArray();
        }

        /// <summary>
        /// Добавляет сборку в хранилище.
        /// </summary>
        /// <param name="assembly">Сборка добавляемая в хранилище.</param>
        public void Add(Assembly assembly)
        {
            if (GetCache().ContainsKey(assembly.FullName))
            {
                return;
            }

            if (assembly is AssemblyBuilder)
            {
                ((AssemblyBuilder)assembly).Save(assembly.GetName().Name + ".dll");
                
                // Загружает вторую копию сборки с диска в память!
                assembly = Assembly.LoadFile(dir.PathCombine(assembly.GetName().Name + ".dll"));
            }
            else
            {
                File.Copy(assembly.CodeBase, Path.Combine(dir, Path.GetFileName(assembly.CodeBase)));
            }

            cache.Add(assembly.FullName, assembly);
        }

        /// <summary>
        /// Удаляет сборку из хранилища.
        /// </summary>
        /// <param name="assembly">Удаляемая сборка.</param>
        public void Remove(Assembly assembly)
        {
            // Не даем удалять сборку Krista.FM.Domain, т.к. она всегда должна присутствовать
            if (assembly.GetName().Name == "Krista.FM.Domain")
            {
                return;
            }

            if (!GetCache().ContainsKey(assembly.FullName))
            {
                return;
            }

            // Удалить файл с диска не получится, т.к. он используется системой,
            // так что просто помечаем его, при следующем запуске приложения он будет удален.
            MarkForRemove(assembly);

            // Удаляем из памяти
            cache.Remove(assembly.FullName);
        }

        private Dictionary<string, Assembly> GetCache()
        {
            if (cache == null)
            {
                // Удаляем сборки с диска помеченные на удаление 
                RemoveMarkedFiles();

                // Загружаем в память с диска все сборки
                cache = Directory
                    .GetFiles(dir, "Krista.FM.Domain.Gen.*.dll", SearchOption.TopDirectoryOnly)
                    .Select(Assembly.LoadFile)
                    .Union(new[] { typeof(Domain.DomainObject).Assembly })
                    .ToDictionary(x => x.FullName);
            }

            return cache;
        }

        private void MarkForRemove(Assembly assembly)
        {
            using (var file = File.CreateText(dir.PathCombine("removelist.txt")))
            {
                file.WriteLine(assembly.GetName().Name);
            }
        }

        private void RemoveMarkedFiles()
        {
            if (!File.Exists(Path.Combine(dir, "removelist.txt")))
            {
                return;
            }

            using (var file = File.OpenText(Path.Combine(dir, "removelist.txt")))
            {
                do
                {
                    var fileName = file.ReadLine();
                    if (fileName.IsNotNullOrEmpty() && File.Exists(dir.PathCombine(fileName + ".dll")))
                    {
                        File.Delete(dir.PathCombine(fileName + ".dll"));
                    }
                } 
                while (!file.EndOfStream);
            }

            if (File.Exists(Path.Combine(dir, WebConfigurationCache.SerializedConfigurationFile)))
            {
                // Удаляем файл кеша конфигурации мапинга NHibernate
                File.Delete(Path.Combine(dir, WebConfigurationCache.SerializedConfigurationFile));
            }

            File.Delete(Path.Combine(dir, "removelist.txt"));
        }
    }
}
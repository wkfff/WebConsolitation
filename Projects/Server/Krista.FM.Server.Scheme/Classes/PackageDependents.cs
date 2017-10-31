using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Статические методы для работы с зависимостями между пакетами.
    /// </summary>
    public class PackageDependents
    {

        #region Внешние методы для работы с зависимостями между пакетами

        /// <summary>
        /// Получаем таблицу конфликтных зависимостей между пакетами для корневого пакета
        /// </summary>
        /// <returns> Таблица зависимостей</returns>
        public static DataTable GetConflictPackageDependents(IPackage package)
        {
            Dictionary<string, GFPackage> packages = new Dictionary<string, GFPackage>();

            FillGrafNodesCollection(packages, package);

            return ReseiveTable(packages);
        }

        /// <summary>
        /// По входящему пакету определяем от каких пакетов в схеме он зависит, включая зависимые через промежуточный пакет
        /// </summary>
        /// <param name="package"> Пакет, для которого хотим определить зависимости</param>
        /// <returns> Коллекция зависимых пакетов</returns>
        public static List<string> GetDependentsByPackage(IPackage package)
        {
            List<string> globalNames = new List<string>();

            if (!globalNames.Contains(package.Name))
                globalNames.Add(package.Name);

            Dependents(package, globalNames);

            return globalNames;
        }

        /// <summary>
        /// Коллекция зависимых пакетов, не включая зависимые через промежуточные пакеты
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public static List<string> GetInnerDependents(IPackage package)
        {
            List<string> globalNames = new List<string>();

            foreach (IEntity entity in package.Classes.Values)
            {
                foreach (IEntityAssociation association in entity.Associations.Values)
                {
                    if (!globalNames.Contains(association.RoleBridge.ParentPackage.Name))
                    {
                        globalNames.Add(association.RoleBridge.ParentPackage.Name);
                    }
                }
            }

            return globalNames;
        }

        /// <summary>
        /// Коллекция пакетов, которые зависят от данного пакета
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public static List<string> GetOutDependents(IPackage package)
        {
            List<string> globalNames = new List<string>();

            foreach (IEntity entity in package.Classes.Values)
            {
                foreach (IEntityAssociation association in entity.Associated.Values)
                {
                    if (!globalNames.Contains(association.RoleBridge.ParentPackage.Name))
                    {
                        globalNames.Add(association.RoleData.ParentPackage.Name);
                    }
                }
            }

            return globalNames;
        }

        /// <summary>
        /// Для пакета получаем коллекцию объектов, отсортированных с учетом внутренних связей
        /// </summary>
        /// <param name="package">Пакет</param>
        /// <returns>Отсортированная коллекция объектов</returns>
        public static List<IEntity> GetSortEntitiesByPackage(IPackage package)
        {
            List<IEntity> sortNames = new List<IEntity>();

            Sort(sortNames, package);

            return sortNames;
        }
        
        #endregion

        #region Локальные методы для работы с зависимостями между пакетами

        /// <summary>
        /// Сортировка объектов в рамках пакета
        /// </summary>
        /// <param name="names"></param>
        /// <param name="package"></param>
        private static void Sort(List<IEntity> names, IPackage package)
        {
            foreach (KeyValuePair<string, IEntity> pair in package.Classes)
            {
                SortEntitiesByAssociation(names, pair.Value);
            }
        }

        /// <summary>
        /// Сортировка на основе связей
        /// </summary>
        /// <param name="names"></param>
        /// <param name="entity"></param>
        private static void SortEntitiesByAssociation(List<IEntity> names, IEntity entity)
        {
            foreach (IEntityAssociation association in entity.Associations.Values)
                if (entity.ParentPackage.Classes.Values.Contains(association.RoleBridge))
                    SortEntitiesByAssociation(names, association.RoleBridge);

            if (!names.Contains(entity))
                names.Add(entity);
        }

        /// <summary>
        /// Метод наполняет граф узлами
        /// </summary>
        /// <param name="packages"></param>
        /// <param name="rootPackage"></param>
        private static void FillGrafNodesCollection(Dictionary<string, GFPackage> packages, IPackage rootPackage)
        {
            foreach (KeyValuePair<string, IPackage> pair in rootPackage.Packages)
            {
                FillGrafNodesCollection(packages, pair.Value);
            }

            FillGrafNodeDependents(packages, rootPackage);
        }

        /// <summary>
        /// Метод наполняет узел графа зависимостями
        /// </summary>
        /// <param name="packages"></param>
        /// <param name="package"></param>
        private static void FillGrafNodeDependents(Dictionary<string, GFPackage> packages, IPackage package)
        {
            GFPackage gfPackge = new GFPackage(package.Name);

            foreach (KeyValuePair<string, IEntity> entity in package.Classes)
            {
                FillDependent(entity.Value, gfPackge);
            }

            packages.Add(package.Name, gfPackge);
        }

        /// <summary>
        /// Метод наполняет зависимость ассоциациями
        /// </summary>
        /// <param name="value"></param>
        /// <param name="packge"></param>
        private static void FillDependent(IEntity value, GFPackage packge)
        {
            foreach (KeyValuePair<string, IEntityAssociation> association in value.Associations)
            {
                // добавляем зависимые пакеты, себя не добавляем
                if (!packge.Dependents.ContainsKey(association.Value.RoleBridge.ParentPackage.Name) && association.Value.RoleBridge.ParentPackage.Name != packge.Name)
                {
                    packge.Dependents.Add(association.Value.RoleBridge.ParentPackage.Name, new GFDependent(association.Value.RoleBridge.ParentPackage.Name));
                }

                if (packge.Dependents.ContainsKey(association.Value.RoleBridge.ParentPackage.Name))
                    packge.Dependents[association.Value.RoleBridge.ParentPackage.Name].GFAssosiations.Add(association.Value.FullCaption);
            }
        }

        /// <summary>
        /// Получаем таблицу зависимостей
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        private static DataTable ReseiveTable(Dictionary<string, GFPackage> collection)
        {
            DataTable table = new DataTable();

            // поля
            DataColumn columnNumber = new DataColumn("number");
            DataColumn columnPath = new DataColumn("path");
            DataColumn cocumnDependent = new DataColumn("dependent");
            DataColumn columnAssociation = new DataColumn("association");

            table.Columns.AddRange(new DataColumn[] { columnNumber, columnPath, cocumnDependent, columnAssociation });

            CollectConflictTable(table, collection);

            return table;
        }

        /// <summary>
        /// Наполняем таблизцу
        /// </summary>
        /// <param name="table"></param>
        /// <param name="collection"></param>
        private static void CollectConflictTable(DataTable table, Dictionary<string, GFPackage> collection)
        {
            int i = 1;
            foreach (KeyValuePair<string, GFPackage> pair in collection)
            {
                foreach (KeyValuePair<string, GFDependent> dependent in pair.Value.Dependents)
                {
                    // проверка на конфликт
                    if (collection[dependent.Value.Name].Dependents.ContainsKey(pair.Value.Name))
                    {
                        foreach (string s in dependent.Value.GFAssosiations)
                        {
                            DataRow row = table.NewRow();

                            row[0] = i;
                            row[1] = pair.Value.Name + " -> " + dependent.Value.Name;
                            row[2] = "Зависимость 1";
                            row[3] = s;

                            table.Rows.Add(row);
                        }

                        foreach (string s in collection[dependent.Value.Name].Dependents[pair.Value.Name].GFAssosiations)
                        {
                            DataRow row = table.NewRow();

                            row[0] = i;
                            row[1] = dependent.Value.Name + " -> " + pair.Value.Name;
                            row[2] = "Зависимость 2";
                            row[3] = s;

                            table.Rows.Add(row);
                        }
                        i++;
                    }
                }
            }
        }

        /// <summary>
        /// Узел графа
        /// </summary>
        [DebuggerStepThrough()]
        internal class GFPackage
        {
            /// <summary>
            /// Имя пакета
            /// </summary>
            private string name;
            /// <summary>
            /// Коллекция зависимостей
            /// </summary>
            private Dictionary<string, GFDependent> dependents = new Dictionary<string, GFDependent>();

            public GFPackage(string name)
            {
                this.name = name;
            }

            public Dictionary<string, GFDependent> Dependents
            {
                get { return dependents; }
                set { dependents = value; }
            }

            public string Name
            {
                get { return name; }
                set { name = value; }
            }
        }

        /// <summary>
        /// Зависимость
        /// </summary>
        [DebuggerStepThrough()]
        internal class GFDependent
        {
            /// <summary>
            /// Имя зависимости
            /// </summary>
            private string name;
            /// <summary>
            /// Коллекция ассоциаций
            /// </summary>
            private List<string> assosiations = new List<string>();


            public GFDependent(string name)
            {
                this.name = name;
            }


            public List<string> GFAssosiations
            {
                get { return assosiations; }
                set { assosiations = value; }
            }

            public string Name
            {
                get { return name; }
                set { name = value; }
            }
        }

        private static void Dependents(IPackage package, List<string> globalNames)
        {
            Dictionary<string, IPackage> dic = new Dictionary<string, IPackage>();

            DependentsEntity(package, dic, globalNames);

            foreach (KeyValuePair<string, IPackage> pair in dic)
                Dependents(pair.Value, globalNames);

        }

        private static void DependentsEntity(IPackage package, Dictionary<string, IPackage> dic, List<string> globalNames)
        {
            foreach (KeyValuePair<string, IEntity> pair in package.Classes)
            {
                DependentsEntityAssociation(pair.Value, dic, globalNames);
            }
        }

        private static void DependentsEntityAssociation(IEntity entity, Dictionary<string, IPackage> dic, List<string> globalNames)
        {
            foreach (KeyValuePair<string, IEntityAssociation> pair in entity.Associations)
            {
                // добавляем зависимые пакеты, себя не добавляем
                if (!globalNames.Contains(pair.Value.RoleBridge.ParentPackage.Name))
                {
                    dic.Add(pair.Value.RoleBridge.ParentPackage.Name, pair.Value.RoleBridge.ParentPackage);
                    globalNames.Add(pair.Value.RoleBridge.ParentPackage.Name);
                }
            }
        }

        #endregion

    }
}

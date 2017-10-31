using System;
using System.Collections.Generic;
using System.Data;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Common
{
    class ClientSystemInfo : SystemInfo
    {
        private DataTable collectedInfo;
        private IScheme scheme;

        public ClientSystemInfo(IScheme scheme)
            : base("Клиент")
        {
            this.scheme = scheme;
        }

        protected override DataTable CollectInfo()
        {
            collectedInfo = base.CollectInfo();

            Collect();

            return collectedInfo;
        }

        private void Collect()
        {
            string categoryName = "Информация о клиенте";

            AddRow(categoryName, "SchemeName", "Имя схемы", scheme.Name, null);
            AddRow(categoryName, "ClientBaseDirectory", "Каталог запуска", AppDomain.CurrentDomain.BaseDirectory, null);
            AddRow(categoryName, "ClientAppName", "Имя приложения", AppDomain.CurrentDomain.SetupInformation.ApplicationName, null);

            CollectVersionInfo(categoryName);
        }

        /// <summary>
        /// TODO: вынести в базовый класс
        /// </summary>
        /// <param name="category"></param>
        /// <param name="filter"></param>
        /// <param name="parentID"></param>
        private void CollectVersionInfo(string category, string filter, object parentID)
        {
            Dictionary<string, string> commonVer = AppVersionControl.GetAssemblyesVersions(filter);
            foreach (string commonVersion in commonVer.Keys)
            {
                 AddRow(category, "Version", commonVersion, commonVer[commonVersion], parentID);
            }
        }

        private void CollectVersionInfo(string categoryName)
        {
            Dictionary<string, string> assClientInfo =
                AppVersionControl.GetAssemblyesVersions(AppVersionControl.ClientAssemblyesSearchMaskDll);
            foreach (KeyValuePair<string, string> assemblyesVersion in AppVersionControl.GetAssemblyesVersions(AppVersionControl.ClientAssemblyesSearchMaskExe))
            {
                assClientInfo.Add(assemblyesVersion.Key, assemblyesVersion.Value);
            }

            Guid versionID = AddRow(categoryName, "ClientVersions", "Версии сборок", String.Format("Число сборок - {0}", assClientInfo.Count), null);
            CollectVersionInfo(categoryName, AppVersionControl.ClientAssemblyesSearchMaskDll, versionID);
            CollectVersionInfo(categoryName, AppVersionControl.ClientAssemblyesSearchMaskExe, versionID);
            CollectVersionInfo(categoryName, "Krista.Diagnostics.dll", versionID);
        }
    }
}

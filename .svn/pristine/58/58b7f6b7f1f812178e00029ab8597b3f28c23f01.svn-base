using System;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;

using Krista.FM.Common;

namespace Krista.FM.Server.Common
{
    /*public class AssemblyesInfo
    {
        private static Dictionary<string, string> serverAssemblyes = null;
        private static string serverLibraryVersion = String.Empty;

        /// <summary>
        /// Метод проверки версий серверных сборок.
        /// </summary>
        /// <returns>Список названий и версий серверных сборок</returns>
        public static Dictionary<string, string> GetServerAssemblyesInfo()
        {
            if (serverAssemblyes == null)
                serverAssemblyes = AssemblyesInfo.GetAssemblyesInfo("Krista.FM.Server.*.dll");
            return serverAssemblyes;
        }

        /// <summary>
        /// Метод проверки версий серверных сборок.
        /// </summary>
        /// <returns>Список названий и версий серверных сборок</returns>
        public static Dictionary<string, string> GetAssemblyesInfo(string filter)
        {
            return AppVersionControl.GetAssembliesVersions(filter);
        }

        
        /// <summary>
        /// Метод проверки версий серверных сборок.
        /// </summary>
        /// <returns>Список названий и версий серверных сборок</returns>
        public static string GetServerLibraryVersion()
        {
            if (String.IsNullOrEmpty(serverLibraryVersion))
            {
                Dictionary<string, string> serverLibraryAssembly = AppVersionControl.GetAssembliesVersions(serverLibraryAssemblyName);
                if (!serverLibraryAssembly.ContainsKey(AppVersionControl.ServerLibraryAssemblyName))
                    throw new Exception(String.Format("Сборка {0} не найдена.", serverLibraryAssemblyName));
                serverLibraryVersion = serverLibraryAssembly[serverLibraryAssemblyName];
            }
            return serverLibraryVersion;
        }
        
    }*/

    public struct Authentication
    {
        [DebuggerStepThrough()]
        private static LogicalCallContextData GetContextData()
        {
            LogicalCallContextData data = (LogicalCallContextData)CallContext.GetData("Authorization");
            if (data == null)
            {
                LogicalCallContextData.SetAuthorization("SYSTEM Destroyer");
                data = (LogicalCallContextData)CallContext.GetData("Authorization");
            }
            return data;
        }

        [DebuggerStepThrough()]
        private static IPrincipal GetPrincipal()
        {
            return GetContextData().Principal;
        }

        public static string UserName
        {
            [DebuggerStepThrough()]
            get { return GetPrincipal().Identity.Name; }
        }

        public static int? UserID
        {
            [DebuggerStepThrough()]
            get
            {
                return (int?)GetContextData()["UserID"];
            }
            [DebuggerStepThrough()]
            set
            {
                GetContextData()["UserID"] = value;
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        public static string UserDate
        {
            [DebuggerStepThrough()]
            get { return GetPrincipal().Identity.Name + " " + DateTime.Now; }
        }

        /// <summary>
        /// Если пользователь не SYSTEM, то генерит исключение UnauthorizedAccessException
        /// </summary>
        [DebuggerStepThrough()]
        public static void CheckSystemRole()
        {
            if (UserName != "SYSTEM" && UserName != "SYSTEM Destroyer")
                throw new UnauthorizedAccessException("Недостаточно привилегий.");
        }

        /// <summary>
        /// Если пользователь не SYSTEM, то возвращает false; иначе true
        /// </summary>
        /// <returns>false или true в зависимости от принадлежности к системной роли</returns>
        [DebuggerStepThrough()]
        public static bool IsSystemRole()
        {
            return UserName == "SYSTEM" || UserName == "SYSTEM Destroyer";
        }

        /// <summary>
        /// Возвращает true, если пользователь Supervisor
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough()]
        public static bool IsSupervisor()
        {
            object value = GetContextData()["Supervisor"];
            return value != null ? (bool)value : false;
        }
    }
}

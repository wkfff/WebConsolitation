using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Security;
using Krista.FM.Common;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Domain.Reporitory.NHibernate.ConfigurationCache;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core.Controllers
{
    public class AccountMembershipService : IMembershipService
    {
        private string lastError;

        public int MinPasswordLength
        {
            get { return 5; }
        }

        public bool ValidateUser(string userName, string password)
        {
            try
            {
                ConnectionHelper.Connect(
                    AuthenticationType.adPwdSHA512, 
                    userName, 
                    password, 
                    HttpContext.Current.Session);

                IScheme scheme = HttpContext.Current.Session[ConnectionHelper.SchemeKeyName] as IScheme;
                if (scheme != null)
                {
                    var lccd = HttpContext.Current.Session[ConnectionHelper.LogicalCallContextDataKeyName] as LogicalCallContextData;
                    if (lccd != null)
                    {
                        lccd["edServerName"] = scheme.Server.GetConfigurationParameter("WebServiceUrl");
                    }

#if DEBUG
                    // Проверяем к одной и той же ли базе данных подключен NHibernate и сервер приложений
                    var connectionString = new ConnectionString();
                    connectionString.Parse(ConfigurationManager.ConnectionStrings["DWH"].ConnectionString);
                    var schemeDataSource = scheme.SchemeDWH.ServerName;
                    if (String.Compare(schemeDataSource, connectionString.DataSource, StringComparison.InvariantCultureIgnoreCase) != 0)
                    {
                        lastError = "Строка подключения в Web.config не соответствует базе данных к которой подключен сервер приложений.";
                        return false;
                    }
#endif

#if !DEBUG
                    var connectionString = scheme.SchemeDWH.ConnectionString;
                    var factoryName = scheme.SchemeDWH.FactoryName;
                    var serverVersion = scheme.SchemeDWH.ServerVersion;

                    // Получаем путь к каталогу где хранится кеш конфигурации
                    var cfgCachePath = Path.Combine(FileHelper.GetTempPath(), "DomainStore");

#if DEBUG
                    var webConfigurationCache = new WebConfigurationCache(cfgCachePath);
#else
                    var webConfigurationCache = new NullConfigurationCache();
#endif

                    NHibernateInitializer.Instance().InitializeNHibernateOnce(
                        () => NHibernateSession.InitializeNHibernateSession(
                            ((MvcApplication)HttpContext.Current.ApplicationInstance).WebSessionStorage,
                            webConfigurationCache, 
                            new WebDynamicAssemblyDomainStorage(cfgCachePath),
                            connectionString, 
                            factoryName, 
                            serverVersion));
#endif
                }

                return true;
            }
            catch (Exception e)
            {
                lastError = e.Message;
                Trace.TraceError(Diagnostics.KristaDiagnostics.ExpandException(e));
                return false;
            }
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            return MembershipCreateStatus.UserRejected;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            return false;
        }

        public string LastError()
        {
            return lastError;
        }
    }
}
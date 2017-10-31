using System;
using Krista.FM.Common;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Domain.Reporitory.NHibernate.ConfigurationCache;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Tests.Common
{
    public static class NHibernateHelper
    {
        public static void SetupNHibernateOracle(string tnsName)
        {   
            LogicalCallContextData.SetAuthorization("nunit");
            LogicalCallContextData.GetContext()["SessionID"] = "nunit-session-id";
            ClientSession.CreateSession(SessionClientType.Server);

            NHibernateSession.InitializeNHibernateSession(
                new SimpleSessionStorage(),
                String.Format("Password=DV;Persist Security Info=True;User ID=DV;Data Source={0}", tnsName),
                "Oracle",
                "10");
        }

        public static void SetupNHibernateOracle(string tnsName, IConfigurationCache configurationCache, IDynamicAssemblyDomainStorage domainStorage)
        {
            LogicalCallContextData.SetAuthorization("nunit");
            LogicalCallContextData.GetContext()["SessionID"] = "nunit-session-id";
            ClientSession.CreateSession(SessionClientType.Server);

            NHibernateSession.InitializeNHibernateSession(
                new SimpleSessionStorage(),
                configurationCache,
                domainStorage,
                String.Format("Password=DV;Persist Security Info=True;User ID=DV;Data Source={0}", tnsName),
                "Oracle",
                "10");
        }

        public static void SetupNHibernateMsSql(string server, string userName)
        {
            LogicalCallContextData.SetAuthorization("nunit");
            LogicalCallContextData.GetContext()["SessionID"] = "nunit-session-id";
            ClientSession.CreateSession(SessionClientType.Server);

            NHibernateSession.InitializeNHibernateSession(
                new SimpleSessionStorage(),
                String.Format("Data Source={0};User ID={1};Password=dv;Persist Security Info=True", server, userName),
                "SQL",
                "9");
        }

        public static void SetupNHibernateMsSql(string server, string userName, IConfigurationCache configurationCache, IDynamicAssemblyDomainStorage domainStorage)
        {
            LogicalCallContextData.SetAuthorization("nunit");
            LogicalCallContextData.GetContext()["SessionID"] = "nunit-session-id";
            ClientSession.CreateSession(SessionClientType.Server);

            NHibernateSession.InitializeNHibernateSession(
                new SimpleSessionStorage(),
                configurationCache,
                domainStorage,
                String.Format("Data Source={0};User ID={1};Password=dv;Persist Security Info=True", server, userName),
                "SQL",
                "9");
        }
    }
}

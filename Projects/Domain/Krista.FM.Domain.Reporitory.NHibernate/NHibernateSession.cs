using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

using Krista.FM.Domain.MappingAttributes;
using Krista.FM.Domain.Reporitory.NHibernate.Automapping;
using Krista.FM.Domain.Reporitory.NHibernate.ConfigurationCache;
using Krista.FM.Domain.Reporitory.NHibernate.FluentMappings.System;

using NHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Krista.FM.Common;
using Krista.FM.Extensions;

using NH = NHibernate;

using NHibernate.Caches.SysCache;
using NHibernate.Cfg;

namespace Krista.FM.Domain.Reporitory.NHibernate
{
    public static class NHibernateSession
    {
        public static readonly string DefaultFactoryKey;
        
        public static readonly string SerializedConfigurationFile;

        /// <summary>
        /// Кеш конфигурации мапинга NHibernate.
        /// </summary>
        private static IConfigurationCache ConfigurationCache { get; set; }
        
        public static ISessionFactory SessionFactory { get; set; }
        
        public static ISessionStorage Storage { get; set; }
        
        /// <summary>
        /// Хранилище доменных сборок.
        /// </summary>
        public static IDynamicAssemblyDomainStorage DomainStorage { get; set; }

        static NHibernateSession()
        {
            DefaultFactoryKey = "nhibernate.current_session";
            SerializedConfigurationFile = "nhConfiguration.cache";
        }

        public static void InitializeNHibernateSession(
            ISessionStorage sessionStorage,
            string connectionString,
            string factoryName,
            string serverVersion)
        {
            InitializeNHibernateSession(
                sessionStorage, 
                new NullConfigurationCache(), 
                new DefaultDynamicAssemblyDomainStorage(), 
                connectionString, 
                factoryName, 
                serverVersion);
        }

        public static void InitializeNHibernateSession(
            ISessionStorage sessionStorage, 
            IConfigurationCache configurationCache,
            IDynamicAssemblyDomainStorage domainStorage,
            string connectionString, 
            string factoryName, 
            string serverVersion)
        {
            try
            {
#if DEBUG
                if (Storage == null)
                {
                    Storage = sessionStorage;
                }
#else
                Storage = sessionStorage;
#endif

                ConfigurationCache = configurationCache;
                DomainStorage = domainStorage;
                SessionFactory = CreateSessionFactory(connectionString, factoryName, serverVersion);
            }
            catch(Exception e)
            {
                Trace.TraceError(Diagnostics.KristaDiagnostics.ExpandException(e));
                
                throw new ApplicationException(e.Message, e);
            }
        }

        public static void RebuildNHibernateSessionFactory(
            string connectionString, 
            string factoryName, 
            string serverVersion)
        {
            try
            {
                // Если кеш конфигурации соответствует доменным сборкам, то ничего не делаем
                if (ConfigurationCache.GetTimeStamp() < GetSourceCodeTimeStamp(DomainStorage.GetAll()))
                {
                    CloseSession();
                    SessionFactory = CreateSessionFactory(connectionString, factoryName, serverVersion);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(Diagnostics.KristaDiagnostics.ExpandException(e));

                throw new ApplicationException(e.Message, e);
            }
        }

        public static ISession Current
        {
            get
            {
                Check.Require(Storage != null, "ISessionStorage не был настроен.");

                ISession session = Storage.Session;
                if (session == null)
                {
                    session = SessionFactory.OpenSession();

                    session.SetDatabaseContext(
                        Storage.GetServerSessionId(),
                        Storage.GetServerUserName()); 
                    
                    Storage.Session = session;
                }
                else if (session.Connection is SqlConnection && !session.Transaction.IsActive)
                {
                    session.SetDatabaseContext(
                        Storage.GetServerSessionId(),
                        Storage.GetServerUserName()); 
                }

                return session;
            }
        }

        /// <summary>
        /// This method is used by application-specific session storage implementations
        /// and unit tests. Its job is to walk thru existing cached sessions and Close() each one.
        /// </summary>
        public static void CloseSession()
        {
            if (Storage != null)
            {
                var session = Storage.Session;
                if (session != null)
                {
                    if (session.IsOpen)
                    {
                        session.Close();
                    }
                    
                    Storage.Session = null;
                }
            }
        }

        /// <summary>
        /// Configure NHibernate. This method returns an ISessionFactory instance that is
        /// populated with mappings created by Fluent NHibernate.
        /// 
        /// Line 1:   Begin configuration
        ///      2+3: Configure the database being used (SQLite file db)
        ///      4+5: Specify what mappings are going to be used (Automappings from the CreateAutomappings method)
        ///      6:   Expose the underlying configuration instance to the BuildSchema method,
        ///           this creates the database.
        ///      7:   Finally, build the session factory.
        /// </summary>
        /// <returns></returns>
        private static ISessionFactory CreateSessionFactory(string connectionString, string factoryName, string serverVersion)
        {
            Assembly[] assemblies = DomainStorage.GetAll();
            return ConfigureNHibernate(connectionString, factoryName, serverVersion, assemblies)
                .BuildSessionFactory();
        }

        /// <summary>
        /// Пытается найти динамическую сборку в хранилие доменных динамических сборок.
        /// Это необходимо для того, чтобы NHibernate смог разрешить ссылки на сгенерированные доменные типы.
        /// </summary>
        private static Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("Krista.FM.Domain"))
            {
                return DomainStorage.Get(args.Name);
            }

            return null;
        }

        /// <summary>
        /// Берет конфигурацию из кеша или создает новую.
        /// </summary>
        private static Configuration ConfigureNHibernate(string connectionString, string factoryName, string serverVersion, Assembly[] assemblies)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;

            try
            {
                // Если кеш конфигурации соответствует доменным сборкам, то конфигурацию берем из кеша
                if (ConfigurationCache.GetTimeStamp() >= GetSourceCodeTimeStamp(assemblies))
                {
                    var configuration = ConfigurationCache.Get();
                    // Если переданная строка подключения и полученная из кеша совпадают,
                    // то используем конфигурацию из кеша
                    if (configuration.Properties["connection.connection_string"] == connectionString)
                    {
                        return configuration;
                    }
                }

                // Строим новую конфигурацию
                var cfg = BuildMappingConfiguration(connectionString, factoryName, serverVersion, assemblies);

                // Сохраняем конфигурацию в кеш
                ConfigurationCache.Set(cfg);

                return cfg;
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainAssemblyResolve;
            }
        }

        /// <summary>
        /// Определяет время последнего изменения сборок.
        /// </summary>
        private static DateTime GetSourceCodeTimeStamp(IEnumerable<Assembly> assemblies)
        {
            var sourceCodeTime = DateTime.MinValue;
            foreach (var assembly in assemblies)
            {
                var lwt = new FileInfo(new Uri(assembly.CodeBase).LocalPath).LastWriteTime;
                if (lwt > sourceCodeTime)
                {
                    sourceCodeTime = lwt;
                }
            }

            return sourceCodeTime;
        }

        /// <summary>
        /// Создает конфигурацию при помощи FluentNHibernate.
        /// </summary>
        private static Configuration BuildMappingConfiguration(string connectionString, string factoryName, string serverVersion, Assembly[] assemblies)
        {
            IPersistenceConfigurer config;

            if (assemblies == null || assemblies.Length == 0)
            {
                assemblies = new[] {typeof (DomainObject).Assembly};
            }

            AutoPersistenceModel autoPersistenceModel = AutoMap
                .Assemblies(new AutomappingConfiguration(), assemblies)
                .Conventions.Add<ClassConvention>()
                .Conventions.Add<ReferenceForeignKeyConvention>()
                .Conventions.Add<CascadeConvention>()
                .Conventions.Add<JoinedSubclassConvention>()
                .Conventions.Add<EnumConvention>()
                .OverrideAll(map => map.IgnoreProperties(x => x.MemberInfo.GetCustomAttributes(typeof(IgnorePropertyAttribute), false).GetLength(0) > 0));

            if (factoryName.ToUpper().Contains("ORACLE"))
            {
                if (serverVersion.Contains("9"))
                {
                    config = OracleClientConfiguration.Oracle9;
                }
                else
                {
                    config = OracleClientConfiguration.Oracle10;
                }

                ConfigureAutomappingsOracle(autoPersistenceModel);

                ((OracleClientConfiguration)config).ConnectionString(c => c.Is(connectionString));
#if !DEBUG                
                ((OracleClientConfiguration)config).UseReflectionOptimizer();
#else
                ((OracleClientConfiguration)config).ShowSql();
#endif

                // http://blog.hazzik.ru/post/26845235551/nhibernate-oracle-and-postgresql-tip
                ((OracleClientConfiguration)config).ConfigureProperties(new Configuration().SetProperty(NH.Cfg.Environment.WrapResultSets, "true"));
            }
            else if (factoryName.ToUpper().Contains("SQL"))
            {
                if (serverVersion.Contains("10"))
                {
                    config = MsSqlConfiguration.MsSql2008;
                }
                else
                {
                    config = MsSqlConfiguration.MsSql2005;
                }

                ConfigureAutomappingsMsSql(autoPersistenceModel);

                ((MsSqlConfiguration)config).ConnectionString(c => c.Is(connectionString));
#if !DEBUG                
                ((MsSqlConfiguration)config).UseReflectionOptimizer();
#else
                ((MsSqlConfiguration)config).ShowSql();
#endif
            }
            else
            {
                throw new NotImplementedException("Провайдер {0} не поддерживается.".FormatWith(factoryName));
            }

            return Fluently.Configure()
                .Database(config)
                .Cache(x => x.ProviderClass<SysCacheProvider>()
                    .UseSecondLevelCache())
                .Mappings(m =>
                    {
                        m.AutoMappings.Add(autoPersistenceModel);
                        m.FluentMappings.Add<HashObjectsNamesMap>();
                    }
                )
                .BuildConfiguration();
        }

        private static AutoPersistenceModel ConfigureAutomappingsOracle(AutoPersistenceModel persistenceModel)
        {
            return persistenceModel
                .Conventions.Add<OracleIdConvention>()
                .Conventions.Add<AnsiStringConvention>();
        }

        private static AutoPersistenceModel ConfigureAutomappingsMsSql(AutoPersistenceModel persistenceModel)
        {
            return persistenceModel
                .Conventions.Add<MsSqlIdConvention>()
                .Conventions.Add<BlobColumnLengthConvention>();
        }
    }
}

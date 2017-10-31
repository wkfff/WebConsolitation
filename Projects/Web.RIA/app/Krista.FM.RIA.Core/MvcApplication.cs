using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Web;

using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Domain.Reporitory.NHibernate.ConfigurationCache;
using Krista.FM.RIA.Core.Helpers;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.ServerLibrary;
using Microsoft.Practices.Unity;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace Krista.FM.RIA.Core
{
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// Событие, сигнализирующее о завершении инициализации NHibernate.
        /// </summary>
        public static readonly AutoResetEvent NHibernateInitializedEvent = new AutoResetEvent(false);

        /// <summary>
        /// Планировщик. Реализован как переменная класса. 
        /// О событиях жизненного цикла см. http://msdn.microsoft.com/ru-ru/library/ms178473%28v=vs.100%29.aspx
        /// </summary>
        private static IScheduler scheduler;

        private WebSessionStorage webSessionStorage;

        /// <summary>
        /// Признак успешной инициализации NHibernate.
        /// </summary>
        public static bool NHibernateInitialized { get; private set; }

        public WebSessionStorage WebSessionStorage
        {
            get { return webSessionStorage; }
        }

        public override void Init()
        {
            base.Init();

            // The WebSessionStorage must be created during the Init() to tie in HttpApplication events
            webSessionStorage = new WebSessionStorage(this);
        }

        protected virtual void Application_Start()
        {
#if DEBUG            
            Thread t = new Thread(this.InitializeNHibernateSession);
            t.Start();
#endif
            // Инициализация планировщика
            InitializeScheduler();
        }

        protected virtual void Application_End()
        {
            if (scheduler != null && scheduler.IsStarted)
            {
                scheduler.Shutdown(true);
            }
        }

        protected virtual void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IScheme, SchemeProxy>();
        }

        protected virtual void Application_BeginRequest(object sender, EventArgs e)
        {
#if DEBUG            
            NHibernateInitializer.Instance().InitializeNHibernateOnce(() =>
                {
                    NHibernateSession.Storage = webSessionStorage;
                });
#endif

            FirstRequestInitialization.Initialize(((HttpApplication)sender).Context);
        }

        private void InitializeNHibernateSession()
        {
            NHibernateInitializedEvent.Reset();

            // Определяем вендора и версию базы данных
            var databadeInfo = DatabaseInfo.Detect(ConfigurationManager.ConnectionStrings["DWH"].ConnectionString);

            // Получаем путь к каталогу где хранится кеш конфигурации
            var cfgCachePath = Path.Combine(FileHelper.GetTempPath(), "DomainStore");

#if DEBUG
            var webConfigurationCache = new WebConfigurationCache(cfgCachePath);
#else
            var webConfigurationCache = new NullConfigurationCache();
#endif

            // Инициализация NHibernate
            NHibernateSession.InitializeNHibernateSession(
                null,
                webConfigurationCache,
                new WebDynamicAssemblyDomainStorage(cfgCachePath),
                databadeInfo.ConnectionString.ToString(),
                databadeInfo.FactoryName,
                databadeInfo.ServerVersion);

            // TODO: Обработать ошибки
            NHibernateInitialized = true;
            NHibernateInitializedEvent.Set();
        }

        private void InitializeScheduler()
        {
            try
            {
                ISchedulerFactory factory = new StdSchedulerFactory();
                scheduler = factory.GetScheduler();

                var globalJobListener = new GlobalJobListener();
                scheduler.ListenerManager.AddJobListener(globalJobListener);

                scheduler.Start();

                foreach (var triggerGroupName in scheduler.GetTriggerGroupNames())
                {
                    Trace.TraceInformation("Информация о задачах в группе {0}.", triggerGroupName);
                    Trace.Indent();
                    try
                    {
                        foreach (var jobKey in scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(triggerGroupName)))
                        {
                            var jobDetail = scheduler.GetJobDetail(jobKey);
                            if (jobDetail != null)
                            {
                                Trace.TraceInformation("Наименование задачи - {0}", jobDetail.Key);
                                Trace.TraceInformation("Тип задачи - {0}", jobDetail.JobType);
                                Trace.TraceInformation("Описание задачи - {0}", jobDetail.Description);
                            }
                        }
                    }
                    finally
                    {
                        Trace.Unindent();
                    }

                    Trace.TraceInformation("Информация о триггерах.");
                    Trace.Indent();
                    try
                    {
                        foreach (var triggerKey in scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals(triggerGroupName)))
                        {
                            var trigger = scheduler.GetTrigger(triggerKey);
                            if (trigger != null)
                            {
                                Trace.TraceInformation("Наименование триггера - {0}", trigger.Key);
                                Trace.TraceInformation("Описание триггера - {0}", trigger.Description);
                                Trace.TraceInformation("Задача триггера - {0}", trigger.JobKey);
                                Trace.TraceInformation("Время запуска - {0}", trigger.StartTimeUtc);
                                Trace.TraceInformation("Время следующего выполенния задачи - {0}", trigger.GetNextFireTimeUtc());
                            }
                        }
                    }
                    finally
                    {
                        Trace.Unindent();
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceInformation(String.Format("При инициализации планировщика заданий возикло исключение: {0}", e.Message));
            }
        }
    }
}

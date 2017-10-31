using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using Krista.FM.Update.Framework;

using Quartz;
using Quartz.Impl;


namespace Krista.FM.Update.ShedulerUpdateService
{
    partial class UpdateService : ServiceBase
    {
        private IUpdateManager manager;
        public UpdateService()
        {
#if DEBUG
            //Debugger.Launch();
#endif
            InitializeComponent();

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }


        protected override void OnStart(string[] args)
        {
            // Основная служба
            ServiceController sc = new ServiceController(UpdateManager.ServiceName);
            // Зависимости
            foreach (ServiceController scc in sc.ServicesDependedOn)
            {
                if (scc.Status != ServiceControllerStatus.StartPending &&
                    scc.Status != ServiceControllerStatus.Running)
                    scc.Start();

                // запуск зависимой службы
                scc.WaitForStatus(ServiceControllerStatus.Running);
                // UpdateManager initialization

            }

            try
            {
                manager = UpdateManagerFactory.CreateServerUpdateManager();
            }
            catch (FrameworkRemotingException e)
            {
                Console.WriteLine(e);
            }
            manager.Activate();

            InitializeScheduler();
            // проверяем не запущен ли уже клиентский процесс, в случае чего, прибиваем его
            StartClientApp();

            Thread.Sleep(10000);
            // проверяем обновления
            manager.StartUpdates();
        }

        protected override void OnStop()
        {
            // TODO: Добавьте код, выполняющий подготовку к остановке службы.
            // Вырубаем клиентский интерфейс службы
            //KillClientProcess();
        }

        private static void StartClientApp()
        {
            // Клиента добавляем в автозагрузку
            //KillClientProcess();

            String applicationName = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
                             "Krista.FM.Update.NotifyIconApp.exe");

            #warning  Пока не удалось найти оптимального механизма взаимодействия службы и клиентского приложения для взаимодействия со службой. В ОС Виста и выше взаимодействие порти UAC
            
            // launch the application
            /*ApplicationLoader.PROCESS_INFORMATION procInfo;
            int error = ApplicationLoader.StartProcessAndBypassUAC(applicationName, out procInfo);
            if (error != 0)
            {
                 using (var writer =
                    new StreamWriter(string.Format("{0}\\ApplicationLoaderCrashLog.txt",
                                                   Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName))))
                {
                    writer.Write(error);
                }
            }*/
            /*ProcessStarter processStarter = new ProcessStarter("notify", applicationName);
            processStarter.Run();*/
        }

        private static void KillClientProcess()
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.StartsWith("Krista.FM.Update.NotifyIconApp"))
                {
                    clsProcess.Kill();
                }
            }
        }

        #region Scheduling

        private StdSchedulerFactory schedFact;
        /// <summary>
        /// Настройка расписания обновления
        /// </summary>
        private void InitializeScheduler()
        {
            if (schedFact != null)
                return;

            // TODO позволить настраивать расписание обновления
            schedFact = new StdSchedulerFactory();

            IScheduler sched = schedFact.GetScheduler();
            sched.Start();

            // задача обновления
            var jobDetail = new JobDetail("updateJob", null, typeof(UpdateJob));
            jobDetail.JobDataMap.Add("updater", manager);

            /*// расписание обновления
            // ежедневно в 0:00
            Trigger trigger = TriggerUtils.MakeDailyTrigger(0, 0);
            trigger.StartTimeUtc = TriggerUtils.GetEvenHourDate(DateTime.UtcNow);
            trigger.Name = "dailyTriggerUpdate";
            sched.ScheduleJob(jobDetail, trigger);

            // Тестовое расписание - ежеминутное для проверки работоспособности
            // TODO: убрать!!
            Trigger minTrigger = TriggerUtils.MakeMinutelyTrigger(1);
            minTrigger.StartTimeUtc = TriggerUtils.GetEvenMinuteDate(DateTime.UtcNow);
            minTrigger.Name = "minutelyTriggerUpdate";
            minTrigger.JobName = "updateJob";
            sched.ScheduleJob(minTrigger);*/


            string cronTrigger = ReadConfiguration();
            if (String.IsNullOrEmpty(cronTrigger))
                cronTrigger = "0 0/1 * * * ?";
            Trigger t = new CronTrigger("Cron trigger", SchedulerConstants.DefaultGroup, cronTrigger);
            t.Name = "CronTriger";
            sched.ScheduleJob(jobDetail, t);
        }

        private static string ReadConfiguration()
        {
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap
                                                  {
                                                      ExeConfigFilename =
                                                          Path.Combine(
                                                              Path.GetDirectoryName(
                                                                  Process.GetCurrentProcess().
                                                                      MainModule.FileName),
                                                              "Krista.FM.Update.ShedulerUpdateService.exe.config")
                                                  };

            Configuration config;
            try
            {
                config =
                    ConfigurationManager.OpenMappedExeConfiguration(fileMap,
                                                                    ConfigurationUserLevel.None);
            }
            catch (ConfigurationErrorsException e)
            {
                return string.Empty;
            }
            foreach (KeyValueConfigurationElement appSetting in config.AppSettings.Settings)
            {
                switch (appSetting.Key)
                {
                    case "CronTrigger":
                        return appSetting.Value;
                    default:
                        continue;
                }
            }

            return string.Empty;
        }

        public class UpdateJob : IJob
        {
            public void Execute(JobExecutionContext context)
            {
                IUpdateManager manager = context.JobDetail.JobDataMap["updater"] as IUpdateManager;
                if (manager != null) manager.StartUpdates();
            }
        }

        #endregion
    }
}

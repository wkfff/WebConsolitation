using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Krista.FM.Update.Framework;
using Krista.FM.Update.ShedulerUpdateService;
using Quartz;
using Quartz.Impl;

namespace Krista.FM.Update.DebugApp
{
    public class DebugApp
    {
        private static IUpdateManager manager;

        static void Main(string[] args)
        {
            manager = UpdateManagerFactory.CreateServerUpdateManager();
            manager.Activate();

            InitializeScheduler();
            // проверяем не запущен ли уже клиентский процесс, в случае чего, прибиваем его
            StartClientApp();

            manager.StartUpdates();
        }
        private static void StartClientApp()
        {
            KillClientProcess();

            String applicationName = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
                             "Krista.FM.Update.NotifyIconApp.exe");

            // launch the application
            ApplicationLoader.PROCESS_INFORMATION procInfo;
            ApplicationLoader.StartProcessAndBypassUAC(applicationName, out procInfo);
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

        private static StdSchedulerFactory schedFact;
        /// <summary>
        /// Настройка расписания обновления
        /// </summary>
        private static void InitializeScheduler()
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
    }
}

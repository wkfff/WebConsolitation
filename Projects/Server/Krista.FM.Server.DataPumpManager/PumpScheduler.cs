using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Lifetime;
using System.Threading;

using Krista.FM.Common;
using Krista.FM.Common.Handling;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumpManagement
{
     /// <summary>
    /// Класс запуска программ закачек по расписанию
    /// </summary>
    public class PumpScheduler : DisposableObject, IPumpScheduler
    {
        #region Поля

        private IScheme activeScheme;
        private Timer pollTimer;
        private TimerCallback pollTimerCallback;
        private SortedList<string, ScheduleSettings> scheduledPrograms = new SortedList<string, ScheduleSettings>(20);
        private PumpScheduleHelper pumpScheduleHelper;

        #endregion Поля


        #region Константы

        /// <summary>
        /// Период опроса расписаний закачек (в миллисекундах)
        /// </summary>
        private const int pollTimerPeriod = 60000;

        #endregion Константы


        #region Инициализация

        /// <summary>
        /// Конструктор
        /// </summary>
        public PumpScheduler(IScheme scheme)
        {
            activeScheme = scheme;

            pumpScheduleHelper = new PumpScheduleHelper();
        }

        /// <summary>
        /// Деструктор
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (pollTimer != null) pollTimer.Dispose();
                if (scheduledPrograms != null) scheduledPrograms.Clear();
            }

            base.Dispose(disposing);
        }

        #endregion Инициализация


        #region Общие функции

        /// <summary>
        /// Считывает настройки расписания для указанной закачки
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void InitScheduledPump(SortedList<string, ScheduleSettings> scheduledPrograms,
            string programIdentifier)
        {
            ScheduleSettings ss = LoadScheduleSettings(programIdentifier);
            if (ss == null)
                return;
            if (ss.Enabled)
            {
                if (scheduledPrograms.ContainsKey(programIdentifier))
                {
                    scheduledPrograms[programIdentifier] = ss;
                }
                else
                {
                    scheduledPrograms.Add(programIdentifier, ss);
                }
            }
        }

        /// <summary>
        /// Заполняет коллекцию программами закачки с установленным расписанием
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void InitScheduledPumpList(SortedList<string, ScheduleSettings> scheduledPrograms)
        {
            scheduledPrograms.Clear();

        	using (IDatabase db = activeScheme.SchemeDWH.DB)
        	{
				foreach (DataRow row in DataPumpInfo.PumpRegistryDataTable(db, String.Empty).Rows)
				{
					InitScheduledPump(scheduledPrograms, Convert.ToString(row["ProgramIdentifier"]));
				}
        	}
        }

        /// <summary>
        /// Запускает шедулер
        /// </summary>
        public void StartScheduler()
        {
            Trace.WriteLine("Запуск диспетчера программ закачки", "PumpScheduler");

            pollTimerCallback = new TimerCallback(PollPumpSchedule);
            pollTimer = new Timer(pollTimerCallback, null, 0, pollTimerPeriod);

            // Вызываем объект управления временeм жизни таймера
            ILease lease = (ILease)pollTimer.InitializeLifetimeService();
            if (lease.CurrentState == LeaseState.Initial)
            {
                // Он будет жить вечно!
                lease.InitialLeaseTime = TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Функция опроса расписаний закачек
        /// </summary>
        [System.Diagnostics.DebuggerStepThrough()]
        private void PollPumpSchedule(object state)
        {
            try
            {
                InitScheduledPumpList(scheduledPrograms);
                DateTime now = DateTime.Now;

                for (int i = scheduledPrograms.Keys.Count - 1; i >= 0; i--)
                {
                    string progID = scheduledPrograms.Keys[i];
                    ScheduleSettings ss = scheduledPrograms[progID];

                    bool startPump = false;

                    // Если время запуска закачки прошло, то анализируем тип запуска: при однократном - запрещаем
                    if ((ss.StartDate < now.Date || (ss.StartDate == now.Date &&
                        ss.StartTime < Convert.ToDateTime(now.ToShortTimeString()))) &&
                        ss.Periodicity == SchedulePeriodicity.Once)
                    {
                        DisableScheduleForPump(progID, ss);
                        continue;
                    }
                    else if (ss.Periodicity == SchedulePeriodicity.Hour)
                    {
                        DateTime startDateTime = new DateTime(ss.StartDate.Year, ss.StartDate.Month, ss.StartDate.Day,
                            ss.StartTime.TimeOfDay.Hours, ss.StartTime.TimeOfDay.Minutes, 0);
                        if ((DateTime.Now >= startDateTime) && (startDateTime.Minute == DateTime.Now.Minute))
                        {
                            HourSchedule hs = (HourSchedule)ss.Schedule;
                            // Если закачка запланирована ежечасно, то и запускаем ее каждый час. 
                            // Если закачка запланирована с периодом, то вычисляем число часов от даты старта
                            // и если оно кратно периоду, то запускаем
                            TimeSpan difDate = DateTime.Now - startDateTime;
                            int hoursDif = Convert.ToInt32(difDate.TotalHours);
                            if ((hs.HourPeriod == 1) || (hoursDif == 0) || (hoursDif % hs.HourPeriod == 0))
                                startPump = true;
                        }
                    }
                    else if (ss.StartTime.TimeOfDay.Hours == now.TimeOfDay.Hours &&
                        ss.StartTime.TimeOfDay.Minutes == now.TimeOfDay.Minutes)
                    {
                        // Анализируем время запуска, текущее время, тип запуска и запускаем нужную закачку
                        switch (ss.Periodicity)
                        {
                            case SchedulePeriodicity.Daily:
                                DailySchedule ds = (DailySchedule)ss.Schedule;

                                // Если закачка запланирована ежедневно, то и запускаем ее каждый день. 
                                // Если закачка запланирована с периодом, то вычисляем число дней от даты старта
                                // и если оно кратно периоду, то запускаем
                                if (ds.DayPeriod == 1 ||
                                    (GetDaysDifference(ss.StartDate, now.Date) % ds.DayPeriod == 0))
                                {
                                    startPump = true;
                                }

                                break;

                            case SchedulePeriodicity.Monthly:
                                MonthlySchedule ms = (MonthlySchedule)ss.Schedule;

                                // Если текущего месяца нет среди назначенных в расписании, то пропускаем
                                if (!ms.Months.Contains(now.Month - 1))
                                {
                                    continue;
                                }

                                switch (ms.MonthlyScheduleKind)
                                {
                                    case MonthlyScheduleKind.ByDayNumbers:
                                        MonthlyByDayNumbers mdn = (MonthlyByDayNumbers)ms.Schedule;

                                        // Старуем закачку, если совпадает день, или это последний день месяца при
                                        // указанном 31 дне (т.е. закачивается в последний день).
                                        if (now.Day == mdn.Day ||
                                            (DateTime.DaysInMonth(now.Year, now.Month) == now.Day ||
                                            mdn.Day == 31))
                                        {
                                            startPump = true;
                                        }

                                        break;

                                    case MonthlyScheduleKind.ByWeekDays:
                                        MonthlyByWeekDays mwd = (MonthlyByWeekDays)ms.Schedule;

                                        double weekRatio = Math.Truncate((double)GetDaysDifference(
                                            new DateTime(now.Year, now.Month, 1), now) / 7);

                                        // Проверяем, входит ли текущий день недели в список назначенных
                                        // Проверяем какя неделя по счету и если это совпадает с данными расписания - запускаем
                                        if (mwd.Day == (int)now.DayOfWeek || weekRatio == mwd.Week)
                                        {
                                            startPump = true;
                                        }

                                        break;
                                }

                                break;

                            case SchedulePeriodicity.Once:
                                // Один раз запустили, запретили и удалили. Вот так.
                                if (DateTime.Compare(ss.StartDate, now.Date) == 0)
                                {
                                    startPump = true;
                                    DisableScheduleForPump(progID, ss);
                                }

                                break;

                            case SchedulePeriodicity.Weekly:
                                WeeklySchedule ws = (WeeklySchedule)ss.Schedule;

                                // Если текущий день недели не входит в список запланированных - пропускаем
                                if (!ws.WeekDays.Contains((int)now.DayOfWeek))
                                {
                                    continue;
                                }

                                int days = GetDaysDifference(ss.StartDate, now);
                                if (days >= 0)
                                {
                                    // Считаем количество недель от даты старта расписания до текущей даты
                                    int weeks = (int)Math.Truncate((double)days / 7);

                                    // Проверяем, какая неделя по счету, и, если это совпадает с данными расписания, запускаем
                                    startPump = (weeks % ws.Week) == 0;
                                }

                                break;
                        }
                    }


                    if (startPump)
                    {
                        StartScheduledPump(progID);
                    }


                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("CRITICAL ERROR: " + ex.ToString(), "PumpScheduler");
            }
        }

        /// <summary>
        /// Возвращает количество дней от одной даты до другой
        /// </summary>
        /// <param name="startDate">Дата, от которой считать</param>
        /// <param name="endDate">Дата, до которой считать</param>
        /// <returns>Количество дней</returns>
        private int GetDaysDifference(DateTime startDate, DateTime endDate)
        {
            int startMonth = startDate.Month;
            int endMonth = endDate.Month;
            int result = 0;

            if (endMonth - startMonth == 0)
            {
                return endDate.Day - startDate.Day;
            }
            else
            {
                if (startDate.Year > endDate.Year)
                {
                    for (int i = startMonth - 1; i >= 1; i--)
                    {
                        result -= DateTime.DaysInMonth(startDate.Year, i);
                    }
                    for (int i = endMonth + 1; i <= 12; i++)
                    {
                        result += DateTime.DaysInMonth(endDate.Year, i);
                    }
                    result -= startDate.Day + DateTime.DaysInMonth(endDate.Year, endDate.Month) - endDate.Day;
                }
                else if (startDate.Year == endDate.Year)
                {
                    for (int i = startMonth + 1; i < endMonth - 1; i++)
                    {
                        result += DateTime.DaysInMonth(startDate.Year, i);
                    }
                    result += DateTime.DaysInMonth(startDate.Year, startDate.Month) - startDate.Day + endDate.Day;
                }
                else
                {
                    for (int i = startMonth + 1; i <= 12; i++)
                    {
                        result += DateTime.DaysInMonth(startDate.Year, i);
                    }
                    for (int i = 1; i <= endMonth - 1; i++)
                    {
                        result += DateTime.DaysInMonth(endDate.Year, i);
                    }
                    result += DateTime.DaysInMonth(startDate.Year, startDate.Month) - startDate.Day + endDate.Day;
                }
            }

            return result;
        }

        private const string SHEDULER_USER_NAME = "Закачка_данных";
        private string GetShedulerUserParams()
        {
            string userName = SHEDULER_USER_NAME;
            string userHost = Environment.MachineName;
            string sessionID = "-";
            return string.Format("{0} {1} {2}", userName, userHost, sessionID);
        }

        /// <summary>
        /// Запускает закачку
        /// </summary>
        /// <param name="progID">ИД закачки</param>
        private void StartScheduledPump(string progID)
        {
            Trace.WriteLine(string.Format("Запуск закачки {0} по расписанию.", progID), "PumpScheduler");
            activeScheme.DataPumpManager.StartPumpProgram(progID, PumpProcessStates.PreviewData, GetShedulerUserParams());
        }

        /// <summary>
        /// Запрещает расписание для указанной закачки
        /// </summary>
        /// <param name="progID">ИД закачки</param>
        /// <param name="ss">Данные расписания</param>
        private void DisableScheduleForPump(string progID, ScheduleSettings ss)
        {
            ss.Enabled = false;
            SaveScheduleSettings(progID, ss);
            scheduledPrograms.Remove(progID);

            Trace.WriteLine(string.Format("Расписание закачки {0} остановлено.", progID), "PumpScheduler");
        }

        #endregion Общие функции


        #region Реализация IPumpScheduler

        /// <summary>
        /// Считывает настройки расписания для указанной закачки
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ScheduleSettings LoadScheduleSettings(string programIdentifier)
        {
            try
            {
                IPumpRegistryElement pre = this.activeScheme.DataPumpManager.DataPumpInfo.PumpRegistry[programIdentifier];

                return pumpScheduleHelper.LoadScheduleSettings(pre.Schedule);
            }
            catch (Exception ex)
            {
                Trace.TraceError(string.Format("Ошибка при загрузке данных расписания {0}: {1}", programIdentifier, ex));
                return null;
            }
        }

        /// <summary>
        /// Сохраняет настройки расписания указанной закачки
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SaveScheduleSettings(string programIdentifier, ScheduleSettings ss)
        {
            try
            {
                IPumpRegistryElement pre = this.activeScheme.DataPumpManager.DataPumpInfo.PumpRegistry[programIdentifier];

                pre.Schedule = pumpScheduleHelper.SaveScheduleSettings(ss);

                if (ss.Enabled)
                {
                    if (scheduledPrograms.ContainsKey(programIdentifier))
                    {
                        scheduledPrograms[programIdentifier] = ss;
                    }
                    else
                    {
                        scheduledPrograms.Add(programIdentifier, ss);
                    }
                }

                EventsProcessing.OnGetStringDelegateEvent(ref this.ScheduleIsChanged, programIdentifier);
            }
            catch (Exception ex)
            {
                Trace.TraceError(string.Format("Ошибка при сохранении данных расписания {0}: {1}", programIdentifier, ex));
            }
        }

        /// <summary>
        /// Событие изменения настроек расписания
        /// </summary>
        public event GetStringDelegate ScheduleIsChanged;

        #endregion Реализация IPumpScheduler
    }
}
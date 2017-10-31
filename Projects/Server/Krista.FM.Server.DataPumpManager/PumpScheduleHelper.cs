using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

using Krista.FM.Common.Xml;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumpManagement
{
    /// <summary>
    /// Класс для загрузки и сохранения параметров расписания запуска закачек
    /// </summary>
    public class PumpScheduleHelper
    {
        #region Константы

        private const string tagPumpSchedule = "PumpSchedule";
        private const string tagScheduleSettings = "ScheduleSettings";
        private const string tagOnceSchedule = "OnceSchedule";
        private const string tagDailySchedule = "DailySchedule";
        private const string tagHourSchedule = "HourSchedule";
        private const string tagWeeklySchedule = "WeeklySchedule";
        private const string tagMonthlySchedule = "MonthlySchedule";
        private const string tagMonthlyByDayNumbers = "MonthlyByDayNumbers";
        private const string tagMonthlyByWeekDays = "MonthlyByWeekDays";

        private const string attrEnabled = "Enabled";
        private const string attrPeriodicity = "Periodicity";
        private const string attrStartTime = "StartTime";
        private const string attrStartDate = "StartDate";
        private const string attrDayPeriod = "DayPeriod";
        private const string attrHourPeriod = "HourPeriod";
        private const string attrWeekDays = "WeekDays";
        private const string attrMonthlyScheduleKind = "MonthlyScheduleKind";
        private const string attrDay = "Day";
        private const string attrWeek = "Week";
        private const string attrMonths = "Months";

        #endregion Константы


        #region Загрузка данных из XML

        /// <summary>
        /// Разбирает хмл и формирует структуру классов, описывающих расписание запуска закачки
        /// </summary>
        /// <param name="scheduleXml">хмл с расписанием</param>
        /// <returns>Класс расписания</returns>
        public ScheduleSettings LoadScheduleSettings(string scheduleXml)
        {
            ScheduleSettings result = new ScheduleSettings();

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(scheduleXml);

            LoadScheduleSettings(xd, result);

            // В зависимости от периодичности выполнения расписания загружаем тэги
            switch (result.Periodicity)
            {
                case SchedulePeriodicity.Daily: 
                    LoadDailySchedule(xd, result);
                    break;

                case SchedulePeriodicity.Hour:
                    LoadHourSchedule(xd, result);
                    break;

                case SchedulePeriodicity.Monthly: 
                    LoadMonthlySchedule(xd, result);

                    if (result.Schedule != null)
                    {
                        MonthlySchedule ms = (MonthlySchedule)result.Schedule;

                        switch (ms.MonthlyScheduleKind)
                        {
                            case MonthlyScheduleKind.ByDayNumbers:
                                LoadMonthlyByDayNumbers(xd, ms);
                                break;

                            case MonthlyScheduleKind.ByWeekDays:
                                LoadMonthlyByWeekDays(xd, ms);
                                break;
                        }
                    }
                    break;

                case SchedulePeriodicity.Once: 
                    break;

                case SchedulePeriodicity.Weekly: 
                    LoadWeeklySchedule(xd, result);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Преобразует строку в SchedulePeriodicity (периодичность выполнения расписания закачки)
        /// </summary>
        private SchedulePeriodicity StringToSchedulePeriodicity(string value)
        {
            string str = value.ToUpper();

            if (str == "DAILY")
            {
                return SchedulePeriodicity.Daily;
            }
            else if (str == "HOUR")
            {
                return SchedulePeriodicity.Hour;
            }
            else if (str == "WEEKLY")
            {
                return SchedulePeriodicity.Weekly;
            }
            else if (str == "MONTHLY")
            {
                return SchedulePeriodicity.Monthly;
            }

            return SchedulePeriodicity.Once;
        }

        /// <summary>
        /// Загружает данные тэга ScheduleSettings
        /// </summary>
        /// <param name="xd">хмл-документ</param>
        /// <param name="scheduleSettings">Класс с данными тэга</param>
        private void LoadScheduleSettings(XmlDocument xd, ScheduleSettings scheduleSettings)
        {
            XmlNode xn = xd.SelectSingleNode(string.Format("//{0}", tagScheduleSettings));
            if (xn == null) return;

            scheduleSettings.Enabled = XmlHelper.GetBoolAttrValue(xn, attrEnabled, false);

            scheduleSettings.Periodicity = StringToSchedulePeriodicity(
                XmlHelper.GetStringAttrValue(xn, attrPeriodicity, "Once"));
            
            IFormatProvider culture = new CultureInfo("ru-RU", true);

            int startDate = XmlHelper.GetIntAttrValue(xn, attrStartDate, 0);
            if (startDate == 0)
            {
                scheduleSettings.StartDate = DateTime.Now;
            }
            else
            {
                scheduleSettings.StartDate = new DateTime(startDate / 10000, (startDate / 100) % 100, startDate % 100);
            }

            int startTime = XmlHelper.GetIntAttrValue(xn, attrStartTime, 0);
            if (startTime == 0)
            {
                scheduleSettings.StartTime = DateTime.Now;
            }
            else
            {
                scheduleSettings.StartTime = new DateTime(
                    scheduleSettings.StartDate.Year, scheduleSettings.StartDate.Month, scheduleSettings.StartDate.Day,
                    startTime / 100, startTime % 100, 0);
            }
        }

        /// <summary>
        /// Загружает данные тэга dailySchedule
        /// </summary>
        /// <param name="xd">хмл-документ</param>
        private void LoadDailySchedule(XmlDocument xd, ScheduleSettings scheduleSettings)
        {
            XmlNode xn = xd.SelectSingleNode(string.Format("//{0}", tagDailySchedule));
            if (xn == null) return;

            DailySchedule dailySchedule = new DailySchedule();
            scheduleSettings.Schedule = dailySchedule;
            
            dailySchedule.DayPeriod = XmlHelper.GetIntAttrValue(xn, attrDayPeriod, 1);
        }

        /// <summary>
        /// Загружает данные тэга hourSchedule
        /// </summary>
        /// <param name="xd">хмл-документ</param>
        private void LoadHourSchedule(XmlDocument xd, ScheduleSettings scheduleSettings)
        {
            XmlNode xn = xd.SelectSingleNode(string.Format("//{0}", tagHourSchedule));
            if (xn == null) return;

            HourSchedule hourSchedule = new HourSchedule();
            scheduleSettings.Schedule = hourSchedule;

            hourSchedule.HourPeriod = XmlHelper.GetIntAttrValue(xn, attrHourPeriod, 1);
        }

        /// <summary>
        /// Преобразует строку в MonthlyScheduleKind
        /// </summary>
        private MonthlyScheduleKind StringToMonthlyScheduleKind(string value)
        {
            string str = value.ToUpper();

            if (str == "BYWEEKDAYS")
            {
                return MonthlyScheduleKind.ByWeekDays;
            }

            return MonthlyScheduleKind.ByDayNumbers;
        }

        /// <summary>
        /// Загружает данные тэга monthlySchedule
        /// </summary>
        /// <param name="xd">хмл-документ</param>
        private void LoadMonthlySchedule(XmlDocument xd, ScheduleSettings scheduleSettings)
        {
            XmlNode xn = xd.SelectSingleNode(string.Format("//{0}", tagMonthlySchedule));
            if (xn == null) return;

            MonthlySchedule monthlySchedule = new MonthlySchedule();
            scheduleSettings.Schedule = monthlySchedule;

            monthlySchedule.MonthlyScheduleKind = StringToMonthlyScheduleKind(
                XmlHelper.GetStringAttrValue(xn, attrMonthlyScheduleKind, string.Empty));

            monthlySchedule.Months = new List<int>(12);
            string nodeValue = XmlHelper.GetStringAttrValue(xn, attrMonths, string.Empty);

            if (nodeValue != string.Empty)
            {
                string[] months = nodeValue.Split(';');
                for (int i = 0; i < months.GetLength(0); i++)
                {
                    monthlySchedule.Months.Add(Convert.ToInt32(months[i]));
                }
            }
        }

        /// <summary>
        /// Загружает данные тэга monthlyByDayNumbers
        /// </summary>
        /// <param name="xd">хмл-документ</param>
        private void LoadMonthlyByDayNumbers(XmlDocument xd, MonthlySchedule monthlySchedule)
        {
            XmlNode xn = xd.SelectSingleNode(string.Format("//{0}", tagMonthlyByDayNumbers));
            if (xn == null) return;

            MonthlyByDayNumbers monthlyByDayNumbers = new MonthlyByDayNumbers();
            monthlySchedule.Schedule = monthlyByDayNumbers;

            monthlyByDayNumbers.Day = XmlHelper.GetIntAttrValue(xn, attrDay, 1);
        }

        /// <summary>
        /// Загружает данные тэга monthlyByWeekDays
        /// </summary>
        /// <param name="xd">хмл-документ</param>
        private void LoadMonthlyByWeekDays(XmlDocument xd, MonthlySchedule monthlySchedule)
        {
            XmlNode xn = xd.SelectSingleNode(string.Format("//{0}", tagMonthlyByWeekDays));
            if (xn == null) return;

            MonthlyByWeekDays monthlyByWeekDays = new MonthlyByWeekDays();
            monthlySchedule.Schedule = monthlyByWeekDays;

            monthlyByWeekDays.Day = XmlHelper.GetIntAttrValue(xn, attrDay, 0);
            monthlyByWeekDays.Week = XmlHelper.GetIntAttrValue(xn, attrWeek, 0);
        }

        /// <summary>
        /// Загружает данные тэга weeklySchedule
        /// </summary>
        /// <param name="xd">хмл-документ</param>
        private void LoadWeeklySchedule(XmlDocument xd, ScheduleSettings scheduleSettings)
        {
            XmlNode xn = xd.SelectSingleNode(string.Format("//{0}", tagWeeklySchedule));
            if (xn == null) return;

            WeeklySchedule weeklySchedule = new WeeklySchedule();
            scheduleSettings.Schedule = weeklySchedule;

            weeklySchedule.WeekDays = new List<int>(7);
            string nodeValue = XmlHelper.GetStringAttrValue(xn, attrWeekDays, string.Empty);

            if (nodeValue != string.Empty)
            {
                string[] weekDays = nodeValue.Split(';');
                for (int i = 0; i < weekDays.GetLength(0); i++)
                {
                    weeklySchedule.WeekDays.Add(Convert.ToInt32(weekDays[i]));
                }
            }

            weeklySchedule.Week = XmlHelper.GetIntAttrValue(xn, attrWeek, 1);
        }

        #endregion Загрузка данных из XML


        #region Сохранение данных в XML

        /// <summary>
        /// Сохраняет настройки расписания в хмл
        /// </summary>
        /// <param name="scheduleSettings">Настройки</param>
        /// <returns>Строка с кодом хмл</returns>
        public string SaveScheduleSettings(ScheduleSettings scheduleSettings)
        {
            XmlDocument xd = new XmlDocument();

            XmlElement root = xd.CreateElement(tagPumpSchedule);
            xd.AppendChild(root);

            XmlElement xe = xd.CreateElement(tagScheduleSettings);
            root.AppendChild(xe);

            SaveScheduleSettings(xe, scheduleSettings);

            // В зависимости от периодичности выполнения расписания загружаем тэги
            switch (scheduleSettings.Periodicity)
            {
                case SchedulePeriodicity.Daily:
                    SaveDailySchedule(xe, scheduleSettings);
                    break;

                case SchedulePeriodicity.Hour:
                    SaveHourSchedule(xe, scheduleSettings);
                    break;

                case SchedulePeriodicity.Monthly:
                    XmlElement xeMonthly = xd.CreateElement(tagMonthlySchedule);
                    xe.AppendChild(xeMonthly);

                    SaveMonthlySchedule(xeMonthly, scheduleSettings);

                    if (scheduleSettings.Schedule != null)
                    {
                        MonthlySchedule ms = (MonthlySchedule)scheduleSettings.Schedule;

                        switch (ms.MonthlyScheduleKind)
                        {
                            case MonthlyScheduleKind.ByDayNumbers:
                                SaveMonthlyByDayNumbers(xeMonthly, ms);
                                break;

                            case MonthlyScheduleKind.ByWeekDays:
                                SaveMonthlyByWeekDays(xeMonthly, ms);
                                break;
                        }
                    }
                    break;

                case SchedulePeriodicity.Once:
                    break;

                case SchedulePeriodicity.Weekly:
                    SaveWeeklySchedule(xe, scheduleSettings);
                    break;
            }
/*
            string validateErr = string.Empty;
            if (!Validator.Validate(xd.InnerXml, "PumpSchedule.xsd", "", out validateErr))
            {
                throw new Exception(validateErr);
            }
*/
            return xd.InnerXml;
        }

        /// <summary>
        /// Преобразует SchedulePeriodicity в строку
        /// </summary>
        private string SchedulePeriodicityToString(SchedulePeriodicity schedulePeriodicity)
        {
            switch (schedulePeriodicity)
            {
                case SchedulePeriodicity.Daily: return "Daily";

                case SchedulePeriodicity.Hour: return "Hour";

                case SchedulePeriodicity.Monthly: return "Monthly";

                case SchedulePeriodicity.Weekly: return "Weekly";
            }

            return "Once";
        }

        /// <summary>
        /// Сохраняет данные о расписании в тэг ScheduleSettings
        /// </summary>
        /// <param name="xeRoot">хмл-элемент, куда добавлять данные</param>
        /// <param name="scheduleSettings">Класс с данными расписания</param>z
        private void SaveScheduleSettings(XmlElement xeRoot, ScheduleSettings scheduleSettings)
        {
            XmlAttribute xa = xeRoot.OwnerDocument.CreateAttribute(attrEnabled);
            xa.Value = Convert.ToString(scheduleSettings.Enabled).ToLower();
            xeRoot.Attributes.Append(xa);

            xa = xeRoot.OwnerDocument.CreateAttribute(attrPeriodicity);
            xa.Value = SchedulePeriodicityToString(scheduleSettings.Periodicity);
            xeRoot.Attributes.Append(xa);

            xa = xeRoot.OwnerDocument.CreateAttribute(attrStartDate);
            xa.Value = string.Format(
                "{0, -4:0000}{1, -2:00}{2, -2:00}", 
                scheduleSettings.StartDate.Year,
                scheduleSettings.StartDate.Month,
                scheduleSettings.StartDate.Day);
            xeRoot.Attributes.Append(xa);

            xa = xeRoot.OwnerDocument.CreateAttribute(attrStartTime);
            xa.Value = string.Format(
                "{0, -2:00}{1, -2:00}",
                scheduleSettings.StartTime.Hour, 
                scheduleSettings.StartTime.Minute);
            xeRoot.Attributes.Append(xa);
        }

        /// <summary>
        /// Сохраняет данные о расписании в тэг DailySchedule
        /// </summary>
        /// <param name="xeRoot">хмл-элемент, куда добавлять данные</param>
        /// <param name="scheduleSettings">Класс с данными расписания</param>
        private void SaveDailySchedule(XmlElement xeRoot, ScheduleSettings scheduleSettings)
        {
            XmlElement xeDaily = xeRoot.OwnerDocument.CreateElement(tagDailySchedule);
            xeRoot.AppendChild(xeDaily);

            DailySchedule dailySchedule = (DailySchedule)scheduleSettings.Schedule;

            XmlAttribute xa = xeRoot.OwnerDocument.CreateAttribute(attrDayPeriod);
            xa.Value = Convert.ToString(dailySchedule.DayPeriod);
            xeDaily.Attributes.Append(xa);
        }

        /// <summary>
        /// Сохраняет данные о расписании в тэг HourSchedule
        /// </summary>
        /// <param name="xeRoot">хмл-элемент, куда добавлять данные</param>
        /// <param name="scheduleSettings">Класс с данными расписания</param>
        private void SaveHourSchedule(XmlElement xeRoot, ScheduleSettings scheduleSettings)
        {
            XmlElement xeHour = xeRoot.OwnerDocument.CreateElement(tagHourSchedule);
            xeRoot.AppendChild(xeHour);

            HourSchedule hourSchedule = (HourSchedule)scheduleSettings.Schedule;

            XmlAttribute xa = xeRoot.OwnerDocument.CreateAttribute(attrHourPeriod);
            xa.Value = Convert.ToString(hourSchedule.HourPeriod);
            xeHour.Attributes.Append(xa);
        }

        /// <summary>
        /// Преобразует MonthlyScheduleKind в строку
        /// </summary>
        private string MonthlyScheduleKindToString(MonthlyScheduleKind monthlyScheduleKind)
        {
            switch (monthlyScheduleKind)
            {
                case MonthlyScheduleKind.ByDayNumbers: return "ByDayNumbers";

                case MonthlyScheduleKind.ByWeekDays: return "ByWeekDays";
            }

            return "ByDayNumbers";
        }

        /// <summary>
        /// Сохраняет данные о расписании в тэг MonthlySchedule
        /// </summary>
        /// <param name="xeRoot">хмл-элемент, куда добавлять данные</param>
        /// <param name="scheduleSettings">Класс с данными расписания</param>
        private void SaveMonthlySchedule(XmlElement xeRoot, ScheduleSettings scheduleSettings)
        {
            MonthlySchedule monthlySchedule = (MonthlySchedule)scheduleSettings.Schedule;

            XmlAttribute xa = xeRoot.OwnerDocument.CreateAttribute(attrMonthlyScheduleKind);
            xa.Value = MonthlyScheduleKindToString(monthlySchedule.MonthlyScheduleKind);
            xeRoot.Attributes.Append(xa);

            xa = xeRoot.OwnerDocument.CreateAttribute(attrMonths);
            for (int i = 0; i < monthlySchedule.Months.Count; i++)
            {
                xa.Value += monthlySchedule.Months[i].ToString() + ";";
            }
            xa.Value = xa.Value.Trim(';');
            if (xa.Value == string.Empty)
            {
                xa.Value = Convert.ToString(DateTime.Now.Month - 1);
            }

            xeRoot.Attributes.Append(xa);
        }

        /// <summary>
        /// Сохраняет данные о расписании в тэг MonthlyByDayNumbers
        /// </summary>
        /// <param name="xeRoot">хмл-элемент, куда добавлять данные</param>
        /// <param name="scheduleSettings">Класс с данными расписания</param>
        private void SaveMonthlyByDayNumbers(XmlElement xeRoot, MonthlySchedule ms)
        {
            XmlElement xe = xeRoot.OwnerDocument.CreateElement(tagMonthlyByDayNumbers);
            xeRoot.AppendChild(xe);

            MonthlyByDayNumbers monthlyByDayNumbers = (MonthlyByDayNumbers)ms.Schedule;

            XmlAttribute xa = xeRoot.OwnerDocument.CreateAttribute(attrDay);
            xa.Value = Convert.ToString(monthlyByDayNumbers.Day);
            xe.Attributes.Append(xa);
        }

        /// <summary>
        /// Сохраняет данные о расписании в тэг MonthlyByWeekDays
        /// </summary>
        /// <param name="xeRoot">хмл-элемент, куда добавлять данные</param>
        /// <param name="scheduleSettings">Класс с данными расписания</param>
        private void SaveMonthlyByWeekDays(XmlElement xeRoot, MonthlySchedule ms)
        {
            XmlElement xe = xeRoot.OwnerDocument.CreateElement(tagMonthlyByWeekDays);
            xeRoot.AppendChild(xe);

            MonthlyByWeekDays monthlyByWeekDays = (MonthlyByWeekDays)ms.Schedule;

            XmlAttribute xa = xeRoot.OwnerDocument.CreateAttribute(attrWeek);
            xa.Value = Convert.ToString(monthlyByWeekDays.Week);
            xe.Attributes.Append(xa);

            xa = xeRoot.OwnerDocument.CreateAttribute(attrDay);
            xa.Value = monthlyByWeekDays.Day.ToString();
            xe.Attributes.Append(xa);
        }

        /// <summary>
        /// Сохраняет данные о расписании в тэг WeeklySchedule
        /// </summary>
        /// <param name="xeRoot">хмл-элемент, куда добавлять данные</param>
        /// <param name="scheduleSettings">Класс с данными расписания</param>
        private void SaveWeeklySchedule(XmlElement xeRoot, ScheduleSettings scheduleSettings)
        {
            XmlElement xeWeekly = xeRoot.OwnerDocument.CreateElement(tagWeeklySchedule);
            xeRoot.AppendChild(xeWeekly);

            WeeklySchedule weeklySchedule = (WeeklySchedule)scheduleSettings.Schedule;

            XmlAttribute xa = xeRoot.OwnerDocument.CreateAttribute(attrWeek);
            xa.Value = Convert.ToString(weeklySchedule.Week);
            xeWeekly.Attributes.Append(xa);

            xa = xeRoot.OwnerDocument.CreateAttribute(attrWeekDays);
            for (int i = 0; i < weeklySchedule.WeekDays.Count; i++)
            {
                xa.Value += weeklySchedule.WeekDays[i].ToString() + ";";
            }
            xa.Value = xa.Value.Trim(';').PadLeft(1, '0');
            xeWeekly.Attributes.Append(xa);
        }

        #endregion Сохранение данных в XML
    }
}
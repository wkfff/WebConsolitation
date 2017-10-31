using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
//
// Интерфейсы управления программами закачки
//
namespace Krista.FM.ServerLibrary
{
    #region Делегаты

    /// <summary>
    /// Делегат для события изменения состояния процесса закачки
    /// </summary>
    /// <param name="state">Состояние закачки</param>
    [Serializable]
    public delegate void GetPumpStateDelegate(PumpProcessStates state);

    /// <summary>
    /// Делегат для функций типа void
    /// </summary>
    [Serializable]
    public delegate void GetVoidDelegate();

    /// <summary>
    /// Делегат для функций типа string
    /// </summary>
    [Serializable]
    public delegate void GetStringDelegate(string str);

    /// <summary>
    /// Делегат для функций типа int
    /// </summary>
    [Serializable]
    public delegate int GetIntDelegate();

    /// <summary>
    /// Делегат для функций удаления закачанных данных
    /// </summary>
    /// <param name="pumpID">ИД закачки</param>
    /// <param name="sourceID">ИД источника</param>
    /// <returns>Строка ошибки</returns>
    [Serializable]
    public delegate string DeleteDataDelegate(int pumpID, int sourceID);

    /// <summary>
    /// Делегат для функций типа bool
    /// </summary>
    [Serializable]
    public delegate bool GetBoolDelegate();

    /// <summary>
    /// Делегат для события смены состояния закачки
    /// </summary>
    [Serializable]
    public delegate void PumpProcessStateChangedDelegate(PumpProcessStates prevState, PumpProcessStates currState);

    #endregion Делегаты


    #region Структуры, перечисления

    /// <summary>
    /// Идентификаторы программ закачки
    /// </summary>
    public enum PumpProgramID
    {
        // АДМИН
        GRBSOutcomesProjectPump,
        ADMIN6Pump,
        ADMIN7Pump,
        ADMIN8Pump,
        ADMIN9Pump,
        // УФК
        Form16Pump,
        FUVaultPump,
        LeasePump,
        Form14Pump,
        Form10Pump,
        TaxesRegulationDataPump,
        Form1NApp7MonthPump,
        Form13Pump,
        Form1NDPPump,
        Form1NApp7DayPump,
        IncomesDistributionPump,
        BudgetCashReceiptsPump,
        UFK10Pump,
        UFK14Pump,
        UFK15Pump,
        UFK16Pump,
        UFK17Pump,
        UFK18Pump,
        UFK19Pump,
        UFK20Pump,
        UFK21Pump,
        UFK22Pump,
        UFK25Pump,
        // ФК
        FKMonthRepPump,
        FK2Pump,
        FK4Pump,
        FK5Pump,
        FK6Pump,
        FK7Pump,
        FK8Pump,
        FK9Pump,
        FK10Pump,
        // ФНС
        FNS28nDataPump,
        Form1NMPump,
        Form5NIOPump,
        Form1OBLPump,
        Form4NMPump,
        Form1NOMPump,
        FNS23Pump,
        FNS24Pump,
        FNS10Pump,
        FNS11Pump,
        FNS22Pump,
        FNS14Pump,
        FNS7Pump,
        FNS8Pump,
        FNS9Pump,
        FNS12Pump,
        FNS13Pump,
        FNS17Pump,
        FNS18Pump,
        FNS19Pump,
        FNS25Pump,
        FNS26Pump,
        FNS27Pump,
        FNS28Pump,
        FNS29Pump,
        FNS30Pump,
        // ФНС РФ
        FNSRF1Pump,
        FNSRF3Pump,
        FNSRF4Pump,
        // ФО
        SKIFMonthRepPump,
        BudgetDataPump,
        SKIFYearRepPump,
        BudgetLayersDataPump,
        BudgetVaultPump,
        FO24Pump,
        FO18Pump,
        FO25Pump,
        FO35Pump,
        FO28Pump,
        FO36Pump,
        FO37Pump,
        FO42Pump,
        FO99Pump,
        FO30Pump,
        FO53Pump,
        FO47Pump,
        // МОФО
        MOFO4Pump,
        MOFO15Pump,
        MOFO16Pump,
        MOFO18Pump,
        MOFO20Pump,
        MOFO21Pump,
        MOFO22Pump,
        MOFO23Pump,
        MOFO24Pump,
        MOFO25Pump,
        MOFO26Pump,
        MOFO27Pump,
        MOFO28Pump,
        MOFO29Pump,
        MOFO31Pump,
        MOFO33Pump,
        // ЭО
        EO5Pump,
        EO7Pump,
        EO8Pump,
        EO9Pump,
        EO19Pump,
        EO20Pump,
        // РОСНЕДВИЖИМ
        RNDV1Pump,
        // ОРГАНИЗАЦИИ
        ORG3Pump,
        ORG5Pump,
        // МФРФ
        MFRF5Pump,
        MFRF4Pump,
        MFRF3Pump,
        // СТАТ
        STAT3Pump,
        STAT31Pump,
        // ФСТ
        FST1Pump,
        FST2Pump,
        // МИНЗДРАВ
        MINZDRAV1Pump,
        // ЛЕCХОЗ
        LESHOZ1Pump,
        LESHOZ2Pump,
        // ГВФ
        GVF1Pump,
        GVF2Pump,
        GVF3Pump,
        // ООС
        OOS1Pump,
        // РКЦ
        RKC1Pump,
        // херня
        Unknown
    }

    /// <summary>
	/// Состояния процесса закачки (порядок не менять!!!)
	/// </summary>
	[Serializable]
	public enum PumpProcessStates
	{
        /// <summary>
        /// Программа закачки готова к работе
        /// </summary>
		Prepared		= 0,

        /// <summary>
        /// Запустить этап предпросмотра
        /// </summary>
        PreviewData     = 1,

        /// <summary>
        /// Запустить этап закачки
        /// </summary>
		PumpData		= 2,

        /// <summary>
        /// Запустить этап обработки
        /// </summary>
		ProcessData		= 3,

        /// <summary>
        /// Запустить этап сопоставления
        /// </summary>
		AssociateData	= 4,

        /// <summary>
        /// Запустить этап расчета кубов
        /// </summary>
		ProcessCube		= 5,

        /// <summary>
        /// Запустить этап проверки данных
        /// </summary>
		CheckData		= 6,

        /// <summary>
        /// Программа закачки завершила работу
        /// </summary>
		Finished		= 7,

        /// <summary>
        /// Запустить этап удаления данных
        /// </summary>
        DeleteData      = 8,

        /// <summary>
        /// Программа закачки запущена (этап выполняется)
        /// </summary>
		Running			= 9,

        /// <summary>
        /// Программа закачки приостановлена
        /// </summary>
		Paused			= 10,

        /// <summary>
        /// Прервать закачку
        /// </summary>
		Aborted			= 11,

        /// <summary>
        /// Пропустить текущий этап
        /// </summary>
		Skip			= 12
	}


	/// <summary>
	/// Состояние этапа
	/// </summary>
	[Serializable]
	public enum StageState
	{
		/// <summary>
        /// Операция в очереди
		/// </summary>
		InQueue = 0,
        
        /// <summary>
        /// Операция не в очереди. 
        /// Сюда попадают операции, предшествующие первой запущенной, и все в случае простоя закачки
        /// </summary>
        OutOfQueue = 1,
		
        /// <summary>
        /// Операция выполняется
		/// </summary>
		InProgress = 2,
		
        /// <summary>
        /// Операция пропущена
		/// </summary>
		Skipped = 3,
		
        /// <summary>
        /// Успешное окончание операции 
		/// </summary>
		SuccefullFinished = 4,
		
        /// <summary>
        /// Завершение операции с ошибками
		/// </summary>
		FinishedWithErrors = 5,

        /// <summary>
        /// Этап заблокирован
        /// </summary>
        Blocked = 6
	}


    /// <summary>
    /// Кодировка символов в файле
    /// </summary>
    public enum CharacterSet
    {
        /// <summary>
        /// Кодировка DOS
        /// </summary>
        OEM,

        /// <summary>
        /// Кодировка Win
        /// </summary>
        ANSI
    }


    /// <summary>
    /// Параметры текстовых отчетов, которые всегда находятся в одном и том же месте (даты и т.п.)
    /// </summary>
    [Serializable]
    public struct FixedParameter
    {
        /// <summary>
        /// Человеческое название параметра, выводимое в форме предварительного просмотра
        /// </summary>
        public string Caption;

        /// <summary>
        /// Значение параметра
        /// </summary>
        public string Value;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="caption">Значение заголовка фиксированного параметра</param>
        /// <param name="value">Значение параметра</param>
        public FixedParameter(string caption, string value)
        {
            this.Caption = caption;
            this.Value = value;
        }
    }

    #endregion Структуры, перечисления


    #region Шедулер

    /// <summary>
    /// Расписание закачки
    /// </summary>
    [Serializable]
    public class ScheduleSettings
    {
        #region Поля

        private bool enabled;
        private SchedulePeriodicity periodicity;
        private DateTime startDate;
        private DateTime startTime;
        private object schedule;

        #endregion Поля


        #region Свойства класса

        /// <summary>
        /// Разрешено ведение расписания или нет
        /// </summary>
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
            }
        }

        /// <summary>
        /// Периодичность выполнения
        /// </summary>
        public SchedulePeriodicity Periodicity
        {
            get
            {
                return periodicity;
            }
            set
            {
                periodicity = value;
            }
        }

        /// <summary>
        /// Дата старта задания
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                return startDate;
            }
            set
            {
                startDate = value;
            }
        }

        /// <summary>
        /// Время старта задания
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                startTime = value;
            }
        }

        /// <summary>
        /// Объект, описывающий детали расписания. В зависимости от Periodicity приводить к соответствующему классу
        /// </summary>
        public object Schedule
        {
            get
            {
                return schedule;
            }
            set
            {
                schedule = value;
            }
        }

        #endregion Свойства класса
    }


    /// <summary>
    /// Периодичность расписания
    /// </summary>
    public enum SchedulePeriodicity
    {
        /// <summary>
        /// Запускается один раз
        /// </summary>
        Once,

        /// <summary>
        /// Запускается каждый час
        /// </summary>
        Hour,

        /// <summary>
        /// Запускается каждый день
        /// </summary>
        Daily,

        /// <summary>
        /// Запускается каждую неделю
        /// </summary>
        Weekly,

        /// <summary>
        /// Запускается каждый месяц
        /// </summary>
        Monthly
    }

    /// <summary>
    /// Класс с настройками расписания, выполняемого ежедневно
    /// </summary>
    [Serializable]
    public class DailySchedule
    {
        #region Поля

        private int dayPeriod;

        #endregion Поля


        #region Свойства класса

        /// <summary>
        /// Период в днях, с каким будет выполняться задание
        /// </summary>
        public int DayPeriod
        {
            get
            {
                return dayPeriod;
            }
            set
            {
                dayPeriod = value;
            }
        }

        #endregion Свойства класса
    }

    /// <summary>
    /// Класс с настройками расписания, выполняемого ежечасно
    /// </summary>
    [Serializable]
    public class HourSchedule
    {
        #region Поля

        private int hourPeriod;

        #endregion Поля


        #region Свойства класса

        /// <summary>
        /// Период в часах, с каким будет выполняться задание
        /// </summary>
        public int HourPeriod
        {
            get
            {
                return hourPeriod;
            }
            set
            {
                hourPeriod = value;
            }
        }

        #endregion Свойства класса
    }

    /// <summary>
    /// Класс с настройками расписания, выполняемого еженедельно
    /// </summary>
    [Serializable]
    public class WeeklySchedule
    {
        #region Поля

        private int week;
        private List<int> weekDays;

        #endregion Поля


        /// <summary>
        /// Деструктор
        /// </summary>
        ~WeeklySchedule()
        {
            if (weekDays != null) weekDays.Clear();
        }


        #region Свойства класса

        /// <summary>
        /// Период в неделях, с каким будет выполняться задание
        /// </summary>
        public int Week
        {
            get
            {
                return week;
            }
            set
            {
                week = value;
            }
        }

        /// <summary>
        /// Список дней недели, по которым будет выполняться задание
        /// </summary>
        public List<int> WeekDays
        {
            get
            {
                return weekDays;
            }
            set
            {
                weekDays = value;
            }
        }

        #endregion Свойства класса
    }


    /// <summary>
    /// Класс с настройками расписания, выполняемого ежемесячно
    /// </summary>
    [Serializable]
    public class MonthlySchedule
    {
        #region Поля

        private MonthlyScheduleKind monthlyScheduleKind;
        private List<int> months;
        private object schedule;

        #endregion Поля


        /// <summary>
        /// Деструктор
        /// </summary>
        ~MonthlySchedule()
        {
            if (months != null) months.Clear();
        }


        #region Свойства класса

        /// <summary>
        /// Указывает, как определяется периодичность выполнения ежемесячного расписания
        /// </summary>
        public MonthlyScheduleKind MonthlyScheduleKind
        {
            get
            {
                return monthlyScheduleKind;
            }
            set
            {
                monthlyScheduleKind = value;
            }
        }

        /// <summary>
        /// Список месяцев, по которым выполнять задание
        /// </summary>
        public List<int> Months
        {
            get
            {
                return months;
            }
            set
            {
                months = value;
            }
        }

        /// <summary>
        /// Объект, описывающий детали расписания. В зависимости от MonthlyScheduleKind приводить 
        /// к соответствующему классу
        /// </summary>
        public object Schedule
        {
            get
            {
                return schedule;
            }
            set
            {
                schedule = value;
            }
        }

        #endregion Свойства класса
    }


    /// <summary>
    /// Указывает, как определяется периодичность выполнения ежемесячного расписания
    /// </summary>
    public enum MonthlyScheduleKind
    {
        /// <summary>
        /// По номеру дня
        /// </summary>
        ByDayNumbers,

        /// <summary>
        /// По дням недели
        /// </summary>
        ByWeekDays
    }


    /// <summary>
    /// Класс расписания, выполняемого по номерам дней месяца
    /// </summary>
    [Serializable]
    public class MonthlyByDayNumbers
    {
        #region Поля

        private int day;

        #endregion Поля


        #region Свойства класса

        /// <summary>
        /// Номер дня (число), в который выполнять задание
        /// </summary>
        public int Day
        {
            get
            {
                return day;
            }
            set
            {
                day = value;
            }
        }

        #endregion Свойства класса
    }


    /// <summary>
    /// Класс расписания, выполняемого по дням недели месяца
    /// </summary>
    [Serializable]
    public class MonthlyByWeekDays
    {
        #region Поля

        private int week;
        private int day;

        #endregion Поля


        #region Свойства класса

        /// <summary>
        /// Номер недели, в которую выполнять задание (5 - последняя неделя)
        /// </summary>
        public int Week
        {
            get
            {
                return week;
            }
            set
            {
                week = value;
            }
        }

        /// <summary>
        /// День недели, в который выполнять задание
        /// </summary>
        public int Day
        {
            get
            {
                return day;
            }
            set
            {
                day = value;
            }
        }

        #endregion Свойства класса
    }

    #endregion Шедулер


    #region Реестр программ закачки

    /// <summary>
    /// Элемент очереди этапов закачки (собственно этап)
    /// </summary>
    public interface IStagesQueueElement : IDisposable
    {
        /// <summary>
        /// Этап
        /// </summary>
        PumpProcessStates State { get; }

        /// </summary>
        /// Признак, что этап был выполнен
        /// </summary>
        bool IsExecuted { get; set; }

        /// <summary>
        /// Время начала выполнения
        /// </summary>
        DateTime StartTime { get; set; }

        /// <summary>
        /// Время окончания выполнения
        /// </summary>
        DateTime EndTime { get; set; }

        /// <summary>
        /// Начальное состояние этапа
        /// </summary>
        StageState StageInitialState { get; set; }

        /// <summary>
        /// Текущее состояние этапа
        /// </summary>
        StageState StageCurrentState { get; set; }

        /// <summary>
        /// Комментарий к этапу
        /// </summary>
        string Comment { get; }

        /// <summary>
        /// Устанавливает начальное значение состояния этапа (с сохранением в хмл в базу)
        /// </summary>
        /// <param name="ss">Состояние (InQueue или Skipped)</param>
        void SetInitialStageState(StageState ss);
    }


    /// <summary>
    /// Очередь этапов закачки
    /// </summary>
    public interface IStagesQueue : IDisposable
    {
        /// <summary>
        /// Возвращает класс состояния указанного этапа закачки
        /// </summary>
        /// <param name="state">Этап закачки</param>
        /// <returns>Состояние этапа</returns>
        IStagesQueueElement this[PumpProcessStates state] { get; set; }

        /// <summary>
        /// Блокировка очереди.
        /// true - текущий этап будет закончен, далее закачка будет продолжена только по установке false
        /// </summary>
        bool Locked { get; set; }

        /// <summary>
        /// Возвращает выполняющийся этап
        /// </summary>
        /// <returns></returns>
        IStagesQueueElement GetInProgressQueueElement();

        /// <summary>
        /// Возвращет последний выполненный этап закачки
        /// </summary>
        IStagesQueueElement GetLastExecutedQueueElement();

        /// <summary>
        /// Возвращет следующий выполняемый этап закачки
        /// </summary>
        IStagesQueueElement GetNextExecutableQueueElement();

        /// <summary>
        /// Очищает данные о выполнении этапов
        /// </summary>
        void ClearExecutingInformation();

        /// <summary>
        /// Проверка на присутствие указанного этапа в очереди закачки
        /// </summary>
        /// <param name="state">Этап</param>
        /// <returns>Присутствует или нет</returns>
        bool ContainsStage(PumpProcessStates state);
    }


    /// <summary>
    /// Элемент реестра закачек.
    /// </summary>
    public interface IPumpRegistryElement
    {
        /// <summary>
        /// Сохраняет измененные свойства в базе данных
        /// </summary>
        void Update();

        /// <summary>
        /// Возвращает значения всех измененных свойств в прежние значения
        /// </summary>
        void Revert();

        /// <summary>
        /// Код поставщика. Текстовый идентификатор 8 символов
        /// </summary>
        string SupplierCode { get; }

        /// <summary>
        /// Порядковый номер поступающей информации, число 4 знака
        /// </summary>
        string DataCode { get; }

        /// <summary>
        /// Идентификатор программы закачки. Максимальная длина 38 символов.
        /// </summary>
        string ProgramIdentifier { get; }

        /// <summary>
        /// Конфигурационные параметры для закачки (в формате XML)
        /// </summary>
        string ProgramConfig { get; set; }

        /// <summary>
        /// Текстовое описание элемента реестра закачек. Максимальная длина 2048 символов.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Название операции закачки
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Настройки этапов закачки
        /// </summary>
        string StagesParameters { get; }

        /// <summary>
        /// Расписание закачки
        /// </summary>
        string Schedule { get; set; }

        /// <summary>
        /// Выполненные операции закачки 
        /// </summary>
        IPumpHistoryCollection PumpHistoryCollection { get; }

        /// <summary>
        /// Очередь этапов закачки
        /// </summary>
        IStagesQueue StagesQueue { get; }

        /// <summary>
        /// История текущей закачки
        /// </summary>
        DataTable PumpHistory { get; }

        /// <summary>
        /// Все закачанные источники текущей закачки
        /// </summary>
        DataTable DataSources { get; }

        /// <summary>
        /// Каталог, где находятся файлы для закачки данной программой (UNC path)
        /// </summary>
        string DataSourcesUNCPath { get; }

        /// <summary>
        /// Каталог, где находятся файлы для закачки данной программой (local path)
        /// </summary>
        string DataSourcesLocalPath { get; }

        /// <summary>
        /// Программа закачки (для универсальных закачек)
        /// </summary>
        string PumpProgram { get; }
    }


    /// <summary>
    /// Коллекция реестра закачек
    /// </summary>
    public interface IPumpRegistryCollection
    {
        /// <summary>
        /// Создает элемент коллекции
        /// </summary>
        /// <returns>Созданный элемент</returns>
        IPumpRegistryElement CreateElement();

        /// <summary>
        /// Индексатор возвращает элемент реестра закачек с указанным ключом,
        /// если ключа нет, то возвращает null. key - ProgramIdentifier закачки.
        /// </summary>
        IPumpRegistryElement this[string key] { get; }
    }

    #endregion Реестр программ закачки


    #region История закачки

    /// <summary>
    /// Выполненная операция закачки
    /// </summary>
    public interface IPumpHistoryElement
    {
        /// <summary>
        /// Удаляет все данные закаченные по этой закачке
        /// </summary>
        void DeleteData();

        /// <summary>
        /// ID источника данных
        /// </summary>
        int ID { get; }

        /// <summary>
        /// Идентификатор программы закачки. Максимальная длина 38 символов.
        /// </summary>
        string ProgramIdentifier { get; set; }

        /// <summary>
        /// Конфигурационные параметры для закачки (в формате XML)
        /// </summary>
        string ProgramConfig { get; set; }

        /// <summary>
        /// Версия системы в формате XX.XX.XX
        /// </summary>
        string SystemVersion { get; set; }

        /// <summary>
        /// Версия программы(модуля) закачки в формате XX.XX.XX
        /// </summary>
        string ProgramVersion { get; set; }

        /// <summary>
        /// Дата и время когда была запущена закачка
        /// </summary>
        DateTime PumpDate { get; set; }

        /// <summary>
        /// Запущена пользователем или планировщиком по расписанию
        /// </summary>
        int StartedBy { get; set; }

        /// <summary>
        /// Текстовое описание элемента реестра закачек. Максимальная длина 2048 символов.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Источники, закачанные по текущей записи истории
        /// </summary>
        DataTable DataSources { get; }

        /// <summary>
        /// Guid пакета обработки кубов
        /// </summary>
        string BatchID { get; set; }

        /// <summary>
        /// имя пользователя
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// машина пользователя
        /// </summary>
        string UserHost { get; set; }

        /// <summary>
        /// идентификатор сессии
        /// </summary>
        string SessionID { get; set; }
    }


    /// <summary>
    /// Коллекция источников данных
    /// </summary>
    public interface IPumpHistoryCollection : IEnumerable
    {
        /// <summary>
        /// Добавить запись истории
        /// </summary>
        /// <param name="value">Объект</param>
        /// <returns>ИД записи</returns>
        int Add(object value);

        /// <summary>
        /// Создает элемент коллекции
        /// </summary>
        /// <returns>Созданный элемент</returns>
        IPumpHistoryElement CreateElement(string programIdentifier);

        /// <summary>
        /// Количество записей в истории
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Удалить запись истории
        /// </summary>
        /// <param name="index">ИД записи</param>
        /// <returns>Сооббщение об ошибке</returns>
        string RemoveAt(int index);

        /// <summary>
        /// Индексатор возвращает элемент реестра закачек с указанным ключом,
        /// если ключа нет, то возвращает null
        /// </summary>
        IPumpHistoryElement this[int key] { get; }
    }

    #endregion История закачки


    #region Информационная часть закачки

    /// <summary>
    /// Информация о ходе закачки. Интерфейс стороны клиента
    /// </summary>
    public interface IDataPumpProgress
    {
        /// <summary>
        /// Обновить данные о состоянии закачки
        /// </summary>
        void Refresh();

        /// <summary>
        /// Состояние процесса закачки 
        /// </summary>
        PumpProcessStates State { get; set; }

        /// <summary>
        /// Максимальное значение прогресса
        /// </summary>
        int ProgressMaxPos { get; set; }

        /// <summary>
        /// Текущее значение прогресса
        /// </summary>
        int ProgressCurrentPos { get; set; }

        /// <summary>
        /// Сообщение прогресса
        /// </summary>
        string ProgressMessage { get; set; }

        /// <summary>
        /// Текст прогресса, который будет писаться на нем самом
        /// </summary>
        string ProgressText { get; set; }

        /// <summary>
        /// Признак, закачка жива или нет
        /// </summary>
        bool PumpIsAlive { get; }

        /// <summary>
        /// Признак выполнения закачки
        /// </summary>
        bool PumpInProgress { get; }

        /// <summary>
        /// Событие смены состояния процесса закачки
        /// </summary>
        event PumpProcessStateChangedDelegate PumpProcessStateChanged;

        /// <summary>
        /// Событие начала этапа
        /// </summary>
        event GetPumpStateDelegate StageStarted;

        /// <summary>
        /// Событие окончания этапа
        /// </summary>
        event GetPumpStateDelegate StageFinished;

        /// <summary>
        /// Событие приостановки этапа
        /// </summary>
        event GetPumpStateDelegate StagePaused;

        /// <summary>
        /// Событие возобновления этапа
        /// </summary>
        event GetPumpStateDelegate StageResumed;

        /// <summary>
        /// Событие остановки этапа
        /// </summary>
        event GetPumpStateDelegate StageStopped;

        /// <summary>
        /// Событие пропуска этапа
        /// </summary>
        event GetPumpStateDelegate StageSkipped;

        /// <summary>
        /// Событие, возникающее при критическом сбое закачки
        /// </summary>
        event GetStringDelegate PumpFailure;
    }


    /// <summary>
    /// Информация о ходе закачки. Интерфейс стороны сервера
    /// </summary>
    public interface IServerSideDataPumpProgress
    {
        /// <summary>
        /// Событие смены состояния процесса закачки
        /// </summary>
        void OnPumpProcessStateChanged(PumpProcessStates prevState, PumpProcessStates currState);

        /// <summary>
        /// Событие начала этапа
        /// </summary>
        void OnStageStarted(PumpProcessStates state);

        /// <summary>
        /// Событие окончания этапа
        /// </summary>
        void OnStageFinished(PumpProcessStates state);

        /// <summary>
        /// Событие приостановки этапа
        /// </summary>
        void OnStagePaused(PumpProcessStates state);

        /// <summary>
        /// Событие возобновления этапа
        /// </summary>
        void OnStageResumed(PumpProcessStates state);

        /// <summary>
        /// Событие остановки этапа
        /// </summary>
        void OnStageStopped(PumpProcessStates state);

        /// <summary>
        /// Событие пропуска этапа
        /// </summary>
        void OnStageSkipped(PumpProcessStates state);

        /// <summary>
        /// Событие, возникающее при критическом сбое закачки
        /// </summary>
        void OnPumpFailure(string str);

        /// <summary>
        /// Событие смены состояния закачки
        /// </summary>
        event GetPumpStateDelegate SetState;

        /// <summary>
        /// Событие получение признака того, что закачка до сих пор работает, а не отвалилась нафиг
        /// </summary>
        event GetBoolDelegate GetPumpLiveStatus;
    }


    /// <summary>
    /// Интерфейс информационной части закачки
    /// </summary>
    public interface IDataPumpInfo
    {
		/// <summary>
		/// Возвращает таблицу с реестром закачек.
		/// </summary>
		DataTable GetPumpRegistryInfo();

		/// <summary>
        /// Удаляет запись информации о закачке
        /// </summary>
        /// <param name="key">ИД закачки</param>
        void Remove(string key);

        /// <summary>
        /// Реестр программ закачки
        /// </summary>
        IPumpRegistryCollection PumpRegistry { get; }

        /// <summary>
        /// Коллекция выполяемых программ закачки. Ключ - ИД программы, значение - информация о ходе закачки
        /// </summary>
        IDataPumpProgress this[string key] { get; }

        /// <summary>
        /// Преобразует строку в состояние процесса закачки
        /// </summary>
        /// <param name="state">Состояние</param>
        /// <returns>Строка</returns>
        PumpProcessStates StringToPumpProcessStates(string state);

        /// <summary>
        /// Преобразует строку в состояние этапа закачки
        /// </summary>
        /// <param name="ss">Строка</param>
        /// <returns>Состояние этапа</returns>
        StageState StringToStageState(string ss);

        /// <summary>
        /// Создает объект информации о ходе закачки
        /// </summary>
        IDataPumpProgress CreateDataPumpProgress();
    }

    #endregion Информационная часть закачки


    #region Менеджер программ закачки

    /// <summary>
    /// Интерфейс менеджера запуска закачек по расписанию
    /// </summary>
    public interface IPumpScheduler : IDisposable
    {
        /// <summary>
        /// Считывает настройки расписания для указанной закачки
        /// </summary>
        ScheduleSettings LoadScheduleSettings(string programIdentifier);

        /// <summary>
        /// Сохраняет настройки расписания указанной закачки
        /// </summary>
        void SaveScheduleSettings(string programIdentifier, ScheduleSettings ss);

        /// <summary>
        /// Событие изменения настроек расписания
        /// </summary>
        event GetStringDelegate ScheduleIsChanged;
    }


    /// <summary>
    /// Интерфейс менеджера программ закачки
    /// </summary>
    public interface IDataPumpManager : IDisposable
    {
        /// <summary>
        /// Запускает шедулер
        /// </summary>
        void StartScheduler();

        /// <summary>
        /// Создание, загрузка и запуск программы закачки
        /// </summary>
        /// <param name="programIdentifier">Идентификатор программы звкачки данных</param>
        /// <param name="startState">Стартовое состояние закачки</param>
        string StartPumpProgram(string programIdentifier, PumpProcessStates startState, string userParams);

        /// <summary>
        /// Запуск удаления закачанных данных
        /// </summary>
        /// <param name="programIdentifier">Идентификатор программы звкачки данных</param>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="sourceID">ИД источника</param>
        /// <returns>Результат выполнения</returns>
        string DeleteData(string programIdentifier, int pumpID, int sourceID);
        
        /// <summary>
        /// Интерфейс менеджера запуска закачек по расписанию
        /// </summary>
        IPumpScheduler PumpScheduler { get; }

        /// <summary>
        /// Интерфейс информационной части закачки
        /// </summary>
        IDataPumpInfo DataPumpInfo { get; }

        /// <summary>
        /// Инициализация полей
        /// </summary>
        void Initialize();
    }

    #endregion Менеджер программ закачки


    #region Интерфейсы закачек

    /// <summary>
	/// Интерфейс для модуля (программы) закачки
	/// </summary>
	public interface IDataPumpModule : IDisposable
	{
		/// <summary>
		/// Удалить данные закачки по ИД закачки и/или источника
		/// </summary>
		/// <param name="pumpID">ИД закачки (-1 - игнорировать)</param>
		/// <param name="sourceID">ИД источника (-1 - игнорировать)</param>
		/// <returns>Строка ошибки</returns>
		string DeleteData(int pumpID, int sourceID);

		/// <summary>
		/// Состояние процесса закачки 
		/// </summary>
		PumpProcessStates State { get; set; }

		/// <summary>
		/// Уникальный идентификатор модуля закачки
		/// </summary>
        string ProgramIdentifier
        {
            get;
        }

        /// <summary>
        /// Уникальный идентификатор модуля закачки
        /// </summary>
        PumpProgramID PumpProgramID
        {
            get;
        }

        /// <summary>
        /// Версия системы
        /// </summary>
        string SystemVersion { get; }

		/// <summary>
		/// Версия программы закачки
		/// </summary>
        string ProgramVersion { get; }

		/// <summary>
		/// Интерфейс для доступа к объектам схемы
		/// </summary>
		IScheme Scheme { get; }

		/// <summary>
		/// true - выгрузка модуля после выполнения одного этапа, иначе - после всех
		/// </summary>
		bool AutoSuicide { get; set; }

        /// <summary>
        /// Если установлено, то закачка ждет завершения текущего этапа и ждет явного указания (через State)
        /// следующего
        /// </summary>
        bool Locked { get; set; }

        /// <summary>
        /// ИД сессии закачки
        /// </summary>
        string SessionID { get; }
	}


    /// <summary>
    /// Интерфейс закачки из текстовых файлов
    /// </summary>
    public interface ITextRepPump : IDataPumpModule
    {
        /// <summary>
        /// Таблица с данными текстовых файлов
        /// </summary>
        DataSet ResultDataSet { get; set; }

        /// <summary>
        /// Список файлов отчетов. По индексу файла в списке фильтруются его данные в ResultTable
        /// </summary>
        List<FileInfo[]> RepFilesLists { get; }

        /// <summary>
        /// Список фиксированных параметров (номер файла - наименование параметра - значение)
        /// </summary>
        Dictionary<int, Dictionary<string, FixedParameter>> FixedParameters { get; set; }

        /// <summary>
        /// Наименование столбца в ResultTable, по которому определяется принадлежность данных конкретному файлу
        /// </summary>
        string FileIndexFieldName { get; }

        /// <summary>
        /// Наименование столбца в таблице с данными отчета, по которому определяется принадлежность данных 
        /// таблице файла отчета
        /// </summary>
        string TableIndexFieldName { get; }

        /// <summary>
        /// Кодировка файлов отчетов
        /// </summary>
        CharacterSet FilesCharacterSet { get; }
    }

    #endregion Интерфейсы закачек

}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
//
// ���������� ���������� ����������� �������
//
namespace Krista.FM.ServerLibrary
{
    #region ��������

    /// <summary>
    /// ������� ��� ������� ��������� ��������� �������� �������
    /// </summary>
    /// <param name="state">��������� �������</param>
    [Serializable]
    public delegate void GetPumpStateDelegate(PumpProcessStates state);

    /// <summary>
    /// ������� ��� ������� ���� void
    /// </summary>
    [Serializable]
    public delegate void GetVoidDelegate();

    /// <summary>
    /// ������� ��� ������� ���� string
    /// </summary>
    [Serializable]
    public delegate void GetStringDelegate(string str);

    /// <summary>
    /// ������� ��� ������� ���� int
    /// </summary>
    [Serializable]
    public delegate int GetIntDelegate();

    /// <summary>
    /// ������� ��� ������� �������� ���������� ������
    /// </summary>
    /// <param name="pumpID">�� �������</param>
    /// <param name="sourceID">�� ���������</param>
    /// <returns>������ ������</returns>
    [Serializable]
    public delegate string DeleteDataDelegate(int pumpID, int sourceID);

    /// <summary>
    /// ������� ��� ������� ���� bool
    /// </summary>
    [Serializable]
    public delegate bool GetBoolDelegate();

    /// <summary>
    /// ������� ��� ������� ����� ��������� �������
    /// </summary>
    [Serializable]
    public delegate void PumpProcessStateChangedDelegate(PumpProcessStates prevState, PumpProcessStates currState);

    #endregion ��������


    #region ���������, ������������

    /// <summary>
    /// �������������� �������� �������
    /// </summary>
    public enum PumpProgramID
    {
        // �����
        GRBSOutcomesProjectPump,
        ADMIN6Pump,
        ADMIN7Pump,
        ADMIN8Pump,
        ADMIN9Pump,
        // ���
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
        // ��
        FKMonthRepPump,
        FK2Pump,
        FK4Pump,
        FK5Pump,
        FK6Pump,
        FK7Pump,
        FK8Pump,
        FK9Pump,
        FK10Pump,
        // ���
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
        // ��� ��
        FNSRF1Pump,
        FNSRF3Pump,
        FNSRF4Pump,
        // ��
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
        // ����
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
        // ��
        EO5Pump,
        EO7Pump,
        EO8Pump,
        EO9Pump,
        EO19Pump,
        EO20Pump,
        // �����������
        RNDV1Pump,
        // �����������
        ORG3Pump,
        ORG5Pump,
        // ����
        MFRF5Pump,
        MFRF4Pump,
        MFRF3Pump,
        // ����
        STAT3Pump,
        STAT31Pump,
        // ���
        FST1Pump,
        FST2Pump,
        // ��������
        MINZDRAV1Pump,
        // ��C���
        LESHOZ1Pump,
        LESHOZ2Pump,
        // ���
        GVF1Pump,
        GVF2Pump,
        GVF3Pump,
        // ���
        OOS1Pump,
        // ���
        RKC1Pump,
        // �����
        Unknown
    }

    /// <summary>
	/// ��������� �������� ������� (������� �� ������!!!)
	/// </summary>
	[Serializable]
	public enum PumpProcessStates
	{
        /// <summary>
        /// ��������� ������� ������ � ������
        /// </summary>
		Prepared		= 0,

        /// <summary>
        /// ��������� ���� �������������
        /// </summary>
        PreviewData     = 1,

        /// <summary>
        /// ��������� ���� �������
        /// </summary>
		PumpData		= 2,

        /// <summary>
        /// ��������� ���� ���������
        /// </summary>
		ProcessData		= 3,

        /// <summary>
        /// ��������� ���� �������������
        /// </summary>
		AssociateData	= 4,

        /// <summary>
        /// ��������� ���� ������� �����
        /// </summary>
		ProcessCube		= 5,

        /// <summary>
        /// ��������� ���� �������� ������
        /// </summary>
		CheckData		= 6,

        /// <summary>
        /// ��������� ������� ��������� ������
        /// </summary>
		Finished		= 7,

        /// <summary>
        /// ��������� ���� �������� ������
        /// </summary>
        DeleteData      = 8,

        /// <summary>
        /// ��������� ������� �������� (���� �����������)
        /// </summary>
		Running			= 9,

        /// <summary>
        /// ��������� ������� ��������������
        /// </summary>
		Paused			= 10,

        /// <summary>
        /// �������� �������
        /// </summary>
		Aborted			= 11,

        /// <summary>
        /// ���������� ������� ����
        /// </summary>
		Skip			= 12
	}


	/// <summary>
	/// ��������� �����
	/// </summary>
	[Serializable]
	public enum StageState
	{
		/// <summary>
        /// �������� � �������
		/// </summary>
		InQueue = 0,
        
        /// <summary>
        /// �������� �� � �������. 
        /// ���� �������� ��������, �������������� ������ ����������, � ��� � ������ ������� �������
        /// </summary>
        OutOfQueue = 1,
		
        /// <summary>
        /// �������� �����������
		/// </summary>
		InProgress = 2,
		
        /// <summary>
        /// �������� ���������
		/// </summary>
		Skipped = 3,
		
        /// <summary>
        /// �������� ��������� �������� 
		/// </summary>
		SuccefullFinished = 4,
		
        /// <summary>
        /// ���������� �������� � ��������
		/// </summary>
		FinishedWithErrors = 5,

        /// <summary>
        /// ���� ������������
        /// </summary>
        Blocked = 6
	}


    /// <summary>
    /// ��������� �������� � �����
    /// </summary>
    public enum CharacterSet
    {
        /// <summary>
        /// ��������� DOS
        /// </summary>
        OEM,

        /// <summary>
        /// ��������� Win
        /// </summary>
        ANSI
    }


    /// <summary>
    /// ��������� ��������� �������, ������� ������ ��������� � ����� � ��� �� ����� (���� � �.�.)
    /// </summary>
    [Serializable]
    public struct FixedParameter
    {
        /// <summary>
        /// ������������ �������� ���������, ��������� � ����� ���������������� ���������
        /// </summary>
        public string Caption;

        /// <summary>
        /// �������� ���������
        /// </summary>
        public string Value;

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="caption">�������� ��������� �������������� ���������</param>
        /// <param name="value">�������� ���������</param>
        public FixedParameter(string caption, string value)
        {
            this.Caption = caption;
            this.Value = value;
        }
    }

    #endregion ���������, ������������


    #region �������

    /// <summary>
    /// ���������� �������
    /// </summary>
    [Serializable]
    public class ScheduleSettings
    {
        #region ����

        private bool enabled;
        private SchedulePeriodicity periodicity;
        private DateTime startDate;
        private DateTime startTime;
        private object schedule;

        #endregion ����


        #region �������� ������

        /// <summary>
        /// ��������� ������� ���������� ��� ���
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
        /// ������������� ����������
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
        /// ���� ������ �������
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
        /// ����� ������ �������
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
        /// ������, ����������� ������ ����������. � ����������� �� Periodicity ��������� � ���������������� ������
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

        #endregion �������� ������
    }


    /// <summary>
    /// ������������� ����������
    /// </summary>
    public enum SchedulePeriodicity
    {
        /// <summary>
        /// ����������� ���� ���
        /// </summary>
        Once,

        /// <summary>
        /// ����������� ������ ���
        /// </summary>
        Hour,

        /// <summary>
        /// ����������� ������ ����
        /// </summary>
        Daily,

        /// <summary>
        /// ����������� ������ ������
        /// </summary>
        Weekly,

        /// <summary>
        /// ����������� ������ �����
        /// </summary>
        Monthly
    }

    /// <summary>
    /// ����� � ����������� ����������, ������������ ���������
    /// </summary>
    [Serializable]
    public class DailySchedule
    {
        #region ����

        private int dayPeriod;

        #endregion ����


        #region �������� ������

        /// <summary>
        /// ������ � ����, � ����� ����� ����������� �������
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

        #endregion �������� ������
    }

    /// <summary>
    /// ����� � ����������� ����������, ������������ ��������
    /// </summary>
    [Serializable]
    public class HourSchedule
    {
        #region ����

        private int hourPeriod;

        #endregion ����


        #region �������� ������

        /// <summary>
        /// ������ � �����, � ����� ����� ����������� �������
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

        #endregion �������� ������
    }

    /// <summary>
    /// ����� � ����������� ����������, ������������ �����������
    /// </summary>
    [Serializable]
    public class WeeklySchedule
    {
        #region ����

        private int week;
        private List<int> weekDays;

        #endregion ����


        /// <summary>
        /// ����������
        /// </summary>
        ~WeeklySchedule()
        {
            if (weekDays != null) weekDays.Clear();
        }


        #region �������� ������

        /// <summary>
        /// ������ � �������, � ����� ����� ����������� �������
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
        /// ������ ���� ������, �� ������� ����� ����������� �������
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

        #endregion �������� ������
    }


    /// <summary>
    /// ����� � ����������� ����������, ������������ ����������
    /// </summary>
    [Serializable]
    public class MonthlySchedule
    {
        #region ����

        private MonthlyScheduleKind monthlyScheduleKind;
        private List<int> months;
        private object schedule;

        #endregion ����


        /// <summary>
        /// ����������
        /// </summary>
        ~MonthlySchedule()
        {
            if (months != null) months.Clear();
        }


        #region �������� ������

        /// <summary>
        /// ���������, ��� ������������ ������������� ���������� ������������ ����������
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
        /// ������ �������, �� ������� ��������� �������
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
        /// ������, ����������� ������ ����������. � ����������� �� MonthlyScheduleKind ��������� 
        /// � ���������������� ������
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

        #endregion �������� ������
    }


    /// <summary>
    /// ���������, ��� ������������ ������������� ���������� ������������ ����������
    /// </summary>
    public enum MonthlyScheduleKind
    {
        /// <summary>
        /// �� ������ ���
        /// </summary>
        ByDayNumbers,

        /// <summary>
        /// �� ���� ������
        /// </summary>
        ByWeekDays
    }


    /// <summary>
    /// ����� ����������, ������������ �� ������� ���� ������
    /// </summary>
    [Serializable]
    public class MonthlyByDayNumbers
    {
        #region ����

        private int day;

        #endregion ����


        #region �������� ������

        /// <summary>
        /// ����� ��� (�����), � ������� ��������� �������
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

        #endregion �������� ������
    }


    /// <summary>
    /// ����� ����������, ������������ �� ���� ������ ������
    /// </summary>
    [Serializable]
    public class MonthlyByWeekDays
    {
        #region ����

        private int week;
        private int day;

        #endregion ����


        #region �������� ������

        /// <summary>
        /// ����� ������, � ������� ��������� ������� (5 - ��������� ������)
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
        /// ���� ������, � ������� ��������� �������
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

        #endregion �������� ������
    }

    #endregion �������


    #region ������ �������� �������

    /// <summary>
    /// ������� ������� ������ ������� (���������� ����)
    /// </summary>
    public interface IStagesQueueElement : IDisposable
    {
        /// <summary>
        /// ����
        /// </summary>
        PumpProcessStates State { get; }

        /// </summary>
        /// �������, ��� ���� ��� ��������
        /// </summary>
        bool IsExecuted { get; set; }

        /// <summary>
        /// ����� ������ ����������
        /// </summary>
        DateTime StartTime { get; set; }

        /// <summary>
        /// ����� ��������� ����������
        /// </summary>
        DateTime EndTime { get; set; }

        /// <summary>
        /// ��������� ��������� �����
        /// </summary>
        StageState StageInitialState { get; set; }

        /// <summary>
        /// ������� ��������� �����
        /// </summary>
        StageState StageCurrentState { get; set; }

        /// <summary>
        /// ����������� � �����
        /// </summary>
        string Comment { get; }

        /// <summary>
        /// ������������� ��������� �������� ��������� ����� (� ����������� � ��� � ����)
        /// </summary>
        /// <param name="ss">��������� (InQueue ��� Skipped)</param>
        void SetInitialStageState(StageState ss);
    }


    /// <summary>
    /// ������� ������ �������
    /// </summary>
    public interface IStagesQueue : IDisposable
    {
        /// <summary>
        /// ���������� ����� ��������� ���������� ����� �������
        /// </summary>
        /// <param name="state">���� �������</param>
        /// <returns>��������� �����</returns>
        IStagesQueueElement this[PumpProcessStates state] { get; set; }

        /// <summary>
        /// ���������� �������.
        /// true - ������� ���� ����� ��������, ����� ������� ����� ���������� ������ �� ��������� false
        /// </summary>
        bool Locked { get; set; }

        /// <summary>
        /// ���������� ������������� ����
        /// </summary>
        /// <returns></returns>
        IStagesQueueElement GetInProgressQueueElement();

        /// <summary>
        /// ��������� ��������� ����������� ���� �������
        /// </summary>
        IStagesQueueElement GetLastExecutedQueueElement();

        /// <summary>
        /// ��������� ��������� ����������� ���� �������
        /// </summary>
        IStagesQueueElement GetNextExecutableQueueElement();

        /// <summary>
        /// ������� ������ � ���������� ������
        /// </summary>
        void ClearExecutingInformation();

        /// <summary>
        /// �������� �� ����������� ���������� ����� � ������� �������
        /// </summary>
        /// <param name="state">����</param>
        /// <returns>������������ ��� ���</returns>
        bool ContainsStage(PumpProcessStates state);
    }


    /// <summary>
    /// ������� ������� �������.
    /// </summary>
    public interface IPumpRegistryElement
    {
        /// <summary>
        /// ��������� ���������� �������� � ���� ������
        /// </summary>
        void Update();

        /// <summary>
        /// ���������� �������� ���� ���������� ������� � ������� ��������
        /// </summary>
        void Revert();

        /// <summary>
        /// ��� ����������. ��������� ������������� 8 ��������
        /// </summary>
        string SupplierCode { get; }

        /// <summary>
        /// ���������� ����� ����������� ����������, ����� 4 �����
        /// </summary>
        string DataCode { get; }

        /// <summary>
        /// ������������� ��������� �������. ������������ ����� 38 ��������.
        /// </summary>
        string ProgramIdentifier { get; }

        /// <summary>
        /// ���������������� ��������� ��� ������� (� ������� XML)
        /// </summary>
        string ProgramConfig { get; set; }

        /// <summary>
        /// ��������� �������� �������� ������� �������. ������������ ����� 2048 ��������.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// �������� �������� �������
        /// </summary>
        string Name { get; }

        /// <summary>
        /// ��������� ������ �������
        /// </summary>
        string StagesParameters { get; }

        /// <summary>
        /// ���������� �������
        /// </summary>
        string Schedule { get; set; }

        /// <summary>
        /// ����������� �������� ������� 
        /// </summary>
        IPumpHistoryCollection PumpHistoryCollection { get; }

        /// <summary>
        /// ������� ������ �������
        /// </summary>
        IStagesQueue StagesQueue { get; }

        /// <summary>
        /// ������� ������� �������
        /// </summary>
        DataTable PumpHistory { get; }

        /// <summary>
        /// ��� ���������� ��������� ������� �������
        /// </summary>
        DataTable DataSources { get; }

        /// <summary>
        /// �������, ��� ��������� ����� ��� ������� ������ ���������� (UNC path)
        /// </summary>
        string DataSourcesUNCPath { get; }

        /// <summary>
        /// �������, ��� ��������� ����� ��� ������� ������ ���������� (local path)
        /// </summary>
        string DataSourcesLocalPath { get; }

        /// <summary>
        /// ��������� ������� (��� ������������� �������)
        /// </summary>
        string PumpProgram { get; }
    }


    /// <summary>
    /// ��������� ������� �������
    /// </summary>
    public interface IPumpRegistryCollection
    {
        /// <summary>
        /// ������� ������� ���������
        /// </summary>
        /// <returns>��������� �������</returns>
        IPumpRegistryElement CreateElement();

        /// <summary>
        /// ���������� ���������� ������� ������� ������� � ��������� ������,
        /// ���� ����� ���, �� ���������� null. key - ProgramIdentifier �������.
        /// </summary>
        IPumpRegistryElement this[string key] { get; }
    }

    #endregion ������ �������� �������


    #region ������� �������

    /// <summary>
    /// ����������� �������� �������
    /// </summary>
    public interface IPumpHistoryElement
    {
        /// <summary>
        /// ������� ��� ������ ���������� �� ���� �������
        /// </summary>
        void DeleteData();

        /// <summary>
        /// ID ��������� ������
        /// </summary>
        int ID { get; }

        /// <summary>
        /// ������������� ��������� �������. ������������ ����� 38 ��������.
        /// </summary>
        string ProgramIdentifier { get; set; }

        /// <summary>
        /// ���������������� ��������� ��� ������� (� ������� XML)
        /// </summary>
        string ProgramConfig { get; set; }

        /// <summary>
        /// ������ ������� � ������� XX.XX.XX
        /// </summary>
        string SystemVersion { get; set; }

        /// <summary>
        /// ������ ���������(������) ������� � ������� XX.XX.XX
        /// </summary>
        string ProgramVersion { get; set; }

        /// <summary>
        /// ���� � ����� ����� ���� �������� �������
        /// </summary>
        DateTime PumpDate { get; set; }

        /// <summary>
        /// �������� ������������� ��� ������������� �� ����������
        /// </summary>
        int StartedBy { get; set; }

        /// <summary>
        /// ��������� �������� �������� ������� �������. ������������ ����� 2048 ��������.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// ���������, ���������� �� ������� ������ �������
        /// </summary>
        DataTable DataSources { get; }

        /// <summary>
        /// Guid ������ ��������� �����
        /// </summary>
        string BatchID { get; set; }

        /// <summary>
        /// ��� ������������
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// ������ ������������
        /// </summary>
        string UserHost { get; set; }

        /// <summary>
        /// ������������� ������
        /// </summary>
        string SessionID { get; set; }
    }


    /// <summary>
    /// ��������� ���������� ������
    /// </summary>
    public interface IPumpHistoryCollection : IEnumerable
    {
        /// <summary>
        /// �������� ������ �������
        /// </summary>
        /// <param name="value">������</param>
        /// <returns>�� ������</returns>
        int Add(object value);

        /// <summary>
        /// ������� ������� ���������
        /// </summary>
        /// <returns>��������� �������</returns>
        IPumpHistoryElement CreateElement(string programIdentifier);

        /// <summary>
        /// ���������� ������� � �������
        /// </summary>
        int Count { get; }

        /// <summary>
        /// ������� ������ �������
        /// </summary>
        /// <param name="index">�� ������</param>
        /// <returns>���������� �� ������</returns>
        string RemoveAt(int index);

        /// <summary>
        /// ���������� ���������� ������� ������� ������� � ��������� ������,
        /// ���� ����� ���, �� ���������� null
        /// </summary>
        IPumpHistoryElement this[int key] { get; }
    }

    #endregion ������� �������


    #region �������������� ����� �������

    /// <summary>
    /// ���������� � ���� �������. ��������� ������� �������
    /// </summary>
    public interface IDataPumpProgress
    {
        /// <summary>
        /// �������� ������ � ��������� �������
        /// </summary>
        void Refresh();

        /// <summary>
        /// ��������� �������� ������� 
        /// </summary>
        PumpProcessStates State { get; set; }

        /// <summary>
        /// ������������ �������� ���������
        /// </summary>
        int ProgressMaxPos { get; set; }

        /// <summary>
        /// ������� �������� ���������
        /// </summary>
        int ProgressCurrentPos { get; set; }

        /// <summary>
        /// ��������� ���������
        /// </summary>
        string ProgressMessage { get; set; }

        /// <summary>
        /// ����� ���������, ������� ����� �������� �� ��� �����
        /// </summary>
        string ProgressText { get; set; }

        /// <summary>
        /// �������, ������� ���� ��� ���
        /// </summary>
        bool PumpIsAlive { get; }

        /// <summary>
        /// ������� ���������� �������
        /// </summary>
        bool PumpInProgress { get; }

        /// <summary>
        /// ������� ����� ��������� �������� �������
        /// </summary>
        event PumpProcessStateChangedDelegate PumpProcessStateChanged;

        /// <summary>
        /// ������� ������ �����
        /// </summary>
        event GetPumpStateDelegate StageStarted;

        /// <summary>
        /// ������� ��������� �����
        /// </summary>
        event GetPumpStateDelegate StageFinished;

        /// <summary>
        /// ������� ������������ �����
        /// </summary>
        event GetPumpStateDelegate StagePaused;

        /// <summary>
        /// ������� ������������� �����
        /// </summary>
        event GetPumpStateDelegate StageResumed;

        /// <summary>
        /// ������� ��������� �����
        /// </summary>
        event GetPumpStateDelegate StageStopped;

        /// <summary>
        /// ������� �������� �����
        /// </summary>
        event GetPumpStateDelegate StageSkipped;

        /// <summary>
        /// �������, ����������� ��� ����������� ���� �������
        /// </summary>
        event GetStringDelegate PumpFailure;
    }


    /// <summary>
    /// ���������� � ���� �������. ��������� ������� �������
    /// </summary>
    public interface IServerSideDataPumpProgress
    {
        /// <summary>
        /// ������� ����� ��������� �������� �������
        /// </summary>
        void OnPumpProcessStateChanged(PumpProcessStates prevState, PumpProcessStates currState);

        /// <summary>
        /// ������� ������ �����
        /// </summary>
        void OnStageStarted(PumpProcessStates state);

        /// <summary>
        /// ������� ��������� �����
        /// </summary>
        void OnStageFinished(PumpProcessStates state);

        /// <summary>
        /// ������� ������������ �����
        /// </summary>
        void OnStagePaused(PumpProcessStates state);

        /// <summary>
        /// ������� ������������� �����
        /// </summary>
        void OnStageResumed(PumpProcessStates state);

        /// <summary>
        /// ������� ��������� �����
        /// </summary>
        void OnStageStopped(PumpProcessStates state);

        /// <summary>
        /// ������� �������� �����
        /// </summary>
        void OnStageSkipped(PumpProcessStates state);

        /// <summary>
        /// �������, ����������� ��� ����������� ���� �������
        /// </summary>
        void OnPumpFailure(string str);

        /// <summary>
        /// ������� ����� ��������� �������
        /// </summary>
        event GetPumpStateDelegate SetState;

        /// <summary>
        /// ������� ��������� �������� ����, ��� ������� �� ��� ��� ��������, � �� ���������� �����
        /// </summary>
        event GetBoolDelegate GetPumpLiveStatus;
    }


    /// <summary>
    /// ��������� �������������� ����� �������
    /// </summary>
    public interface IDataPumpInfo
    {
		/// <summary>
		/// ���������� ������� � �������� �������.
		/// </summary>
		DataTable GetPumpRegistryInfo();

		/// <summary>
        /// ������� ������ ���������� � �������
        /// </summary>
        /// <param name="key">�� �������</param>
        void Remove(string key);

        /// <summary>
        /// ������ �������� �������
        /// </summary>
        IPumpRegistryCollection PumpRegistry { get; }

        /// <summary>
        /// ��������� ���������� �������� �������. ���� - �� ���������, �������� - ���������� � ���� �������
        /// </summary>
        IDataPumpProgress this[string key] { get; }

        /// <summary>
        /// ����������� ������ � ��������� �������� �������
        /// </summary>
        /// <param name="state">���������</param>
        /// <returns>������</returns>
        PumpProcessStates StringToPumpProcessStates(string state);

        /// <summary>
        /// ����������� ������ � ��������� ����� �������
        /// </summary>
        /// <param name="ss">������</param>
        /// <returns>��������� �����</returns>
        StageState StringToStageState(string ss);

        /// <summary>
        /// ������� ������ ���������� � ���� �������
        /// </summary>
        IDataPumpProgress CreateDataPumpProgress();
    }

    #endregion �������������� ����� �������


    #region �������� �������� �������

    /// <summary>
    /// ��������� ��������� ������� ������� �� ����������
    /// </summary>
    public interface IPumpScheduler : IDisposable
    {
        /// <summary>
        /// ��������� ��������� ���������� ��� ��������� �������
        /// </summary>
        ScheduleSettings LoadScheduleSettings(string programIdentifier);

        /// <summary>
        /// ��������� ��������� ���������� ��������� �������
        /// </summary>
        void SaveScheduleSettings(string programIdentifier, ScheduleSettings ss);

        /// <summary>
        /// ������� ��������� �������� ����������
        /// </summary>
        event GetStringDelegate ScheduleIsChanged;
    }


    /// <summary>
    /// ��������� ��������� �������� �������
    /// </summary>
    public interface IDataPumpManager : IDisposable
    {
        /// <summary>
        /// ��������� �������
        /// </summary>
        void StartScheduler();

        /// <summary>
        /// ��������, �������� � ������ ��������� �������
        /// </summary>
        /// <param name="programIdentifier">������������� ��������� ������� ������</param>
        /// <param name="startState">��������� ��������� �������</param>
        string StartPumpProgram(string programIdentifier, PumpProcessStates startState, string userParams);

        /// <summary>
        /// ������ �������� ���������� ������
        /// </summary>
        /// <param name="programIdentifier">������������� ��������� ������� ������</param>
        /// <param name="pumpID">�� �������</param>
        /// <param name="sourceID">�� ���������</param>
        /// <returns>��������� ����������</returns>
        string DeleteData(string programIdentifier, int pumpID, int sourceID);
        
        /// <summary>
        /// ��������� ��������� ������� ������� �� ����������
        /// </summary>
        IPumpScheduler PumpScheduler { get; }

        /// <summary>
        /// ��������� �������������� ����� �������
        /// </summary>
        IDataPumpInfo DataPumpInfo { get; }

        /// <summary>
        /// ������������� �����
        /// </summary>
        void Initialize();
    }

    #endregion �������� �������� �������


    #region ���������� �������

    /// <summary>
	/// ��������� ��� ������ (���������) �������
	/// </summary>
	public interface IDataPumpModule : IDisposable
	{
		/// <summary>
		/// ������� ������ ������� �� �� ������� �/��� ���������
		/// </summary>
		/// <param name="pumpID">�� ������� (-1 - ������������)</param>
		/// <param name="sourceID">�� ��������� (-1 - ������������)</param>
		/// <returns>������ ������</returns>
		string DeleteData(int pumpID, int sourceID);

		/// <summary>
		/// ��������� �������� ������� 
		/// </summary>
		PumpProcessStates State { get; set; }

		/// <summary>
		/// ���������� ������������� ������ �������
		/// </summary>
        string ProgramIdentifier
        {
            get;
        }

        /// <summary>
        /// ���������� ������������� ������ �������
        /// </summary>
        PumpProgramID PumpProgramID
        {
            get;
        }

        /// <summary>
        /// ������ �������
        /// </summary>
        string SystemVersion { get; }

		/// <summary>
		/// ������ ��������� �������
		/// </summary>
        string ProgramVersion { get; }

		/// <summary>
		/// ��������� ��� ������� � �������� �����
		/// </summary>
		IScheme Scheme { get; }

		/// <summary>
		/// true - �������� ������ ����� ���������� ������ �����, ����� - ����� ����
		/// </summary>
		bool AutoSuicide { get; set; }

        /// <summary>
        /// ���� �����������, �� ������� ���� ���������� �������� ����� � ���� ������ �������� (����� State)
        /// ����������
        /// </summary>
        bool Locked { get; set; }

        /// <summary>
        /// �� ������ �������
        /// </summary>
        string SessionID { get; }
	}


    /// <summary>
    /// ��������� ������� �� ��������� ������
    /// </summary>
    public interface ITextRepPump : IDataPumpModule
    {
        /// <summary>
        /// ������� � ������� ��������� ������
        /// </summary>
        DataSet ResultDataSet { get; set; }

        /// <summary>
        /// ������ ������ �������. �� ������� ����� � ������ ����������� ��� ������ � ResultTable
        /// </summary>
        List<FileInfo[]> RepFilesLists { get; }

        /// <summary>
        /// ������ ������������� ���������� (����� ����� - ������������ ��������� - ��������)
        /// </summary>
        Dictionary<int, Dictionary<string, FixedParameter>> FixedParameters { get; set; }

        /// <summary>
        /// ������������ ������� � ResultTable, �� �������� ������������ �������������� ������ ����������� �����
        /// </summary>
        string FileIndexFieldName { get; }

        /// <summary>
        /// ������������ ������� � ������� � ������� ������, �� �������� ������������ �������������� ������ 
        /// ������� ����� ������
        /// </summary>
        string TableIndexFieldName { get; }

        /// <summary>
        /// ��������� ������ �������
        /// </summary>
        CharacterSet FilesCharacterSet { get; }
    }

    #endregion ���������� �������

}
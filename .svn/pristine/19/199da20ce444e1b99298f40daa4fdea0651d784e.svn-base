using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Security.Principal;

using Infragistics.Win;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinProgressBar;
using Infragistics.Win.UltraWinStatusBar;
using Infragistics.Win.UltraWinTabControl;

using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Common.Handling;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.DataPumpUI
{
	/// <summary>
	/// Интерфейс подсистемы закачки
	/// </summary>
	public partial class DataPumpUI : BaseViewObj
    {
        #region Делегаты
        
        delegate void SetUpbPositionDelegate(UltraProgressBar upb, int Position);
		delegate void SetUpbTextDelegate(UltraProgressBar upb, string Text);
		delegate void SetUpbVisibleDelegate(UltraProgressBar upb, bool Visible);
		delegate void SetUlTextDelegate(UltraLabel ul, string Text);
		delegate void SetUlVisibleDelegate(UltraLabel ul, bool Visible);
		delegate void SetUbtnEnableDelegate(UltraButton ubtn, bool Enable);
		//delegate void SetUbtnImageDelegate(UltraButton ubtn, Bitmap bmp);
        //delegate void SetUbtnTextDelegate(UltraButton ubtn, string text);
        //delegate void SetCursorDelegate(Cursor cursor);
        //delegate DialogResult ShowDialogFormDelegate(Form form);
        delegate void SetSelectedTabDelegate(UltraTabControl utc, UltraTab tab);

        #endregion Делегаты


        #region Поля

        private IDataSourceManager dataSourcesManager;
        private IDataPumpManager dataPumpManager;
		private DataPumpView dpv;
		private IPumpRegistryElement pumpRegistryElement = null;
        // ***
		private IDataPumpProgress dataPumpProgress = null;
        /*private IDataPumpModule currentPumpModule
        {
            get 
            {
                try
                {
                    string teststr = _currentPumpModule.Caption;
                }
                catch
                {
                    // модуль бул выгружен
                    _currentPumpModule = null;
                }
                return _currentPumpModule;
            }
            set
            {
                _currentPumpModule = value;
            }
        }*/
        // ***
        private ScheduleSettings currentScheduleSettings = null;
        private DataSet dsPumpHistory = null;
        private DataSet dsDataSources = null;

		private SetUpbPositionDelegate upb_SetPositionDelegate;
		private SetUpbTextDelegate upb_SetTextDelegate;
		private SetUpbVisibleDelegate upb_SetVisibleDelegate;
		private SetUlTextDelegate ul_SetTextDelegate;
		private SetUlVisibleDelegate ul_SetVisibleDelegate;
		private SetUbtnEnableDelegate ubtn_SetEnableDelegate;
		//private SetUbtnImageDelegate ubtn_SetImageDelegate;
        //private SetUbtnTextDelegate ubtn_SetTextDelegate;
        //private SetCursorDelegate setCursorDelegate;
        //private ShowDialogFormDelegate showDialogForm;
        private SetSelectedTabDelegate setSelectedTab;

        private PumpParamsDisplaying pumpParams = new PumpParamsDisplaying();

		private IInplaceProtocolView ipvPumpData;
		private IInplaceProtocolView ipvProcessData;
        private IInplaceProtocolView ipvAssociateData;
		private IInplaceProtocolView ipvDeleteData;
        private IInplaceProtocolView ipvProcessCube;
        private IInplaceProtocolView ipvCheckData;
        private IInplaceProtocolView ipvClassifiers;

        private DataPumpProgressHandling dataPumpProgressHandling;
        private PumpSchedulerHandling pumpSchedulerHandling;

		private bool dpvIsLoaded = false;
		private bool pumpHistoryIsLoaded = false;
		private string programIdentifier;

        // Для работы с формой предварительного просмотра результатов разбора текстовых файлов
        private ITextRepPump textRepPump = null;
        private DataSet textRepResultDataSet;
        private int fileIndex = 0;
        private Dictionary<int, Dictionary<string, FixedParameter>> fixedParams;

        private ArrayList leftPathsVisited = new ArrayList(12);
        private ArrayList rightPathsVisited = new ArrayList(12);
        private int leftCurrentPathIndex = -1;
        private int rightCurrentPathIndex = -1;
        private FolderViewMode leftBrowserViewMode = FolderViewMode.FVM_LIST;
        private FolderViewMode rightBrowserViewMode = FolderViewMode.FVM_LIST;

        private bool startPumpEnabled = false;
        private bool stopPumpEnabled = false;
        private bool deletePumpEnabled = false;

        private bool pumpParamsSaved = true;
        private bool pumpScheduleSaved = true;

        private ScheduleMonths scheduleMonths = new ScheduleMonths();
        private bool scheduleMonthsChanged = false;
        private bool scheduleChanged = false;
        private bool scheduleInitializing = false;
        private bool scheduleEnabled = false;
        //private bool scheduleChangedWarning = false;

        // Список соответствия истории закачки источникам данных
        private Dictionary<int, List<int>> history2DataSourcesMapping;
        // Список соответствия источников данных истории закачки
        private Dictionary<int, List<int>> dataSources2HistoryMapping;

        private int timerPeriod = 2000;
        private System.Threading.Timer timer;
        private TimerCallback timerDelegate;
        private int timerDueTime = Timeout.Infinite;

        //private LogicalCallContextData clientContext;
        
        #endregion Поля


        #region Инициализация

		protected override void SetViewCtrl()
		{
			fViewCtrl = new DataPumpView();
			dpv = (DataPumpView)fViewCtrl;
            pumpParams.workplace = Workplace;
		}

        public override Icon Icon
        {
            get { return Icon.FromHandle(Properties.Resources.pump_DataPump_16.GetHicon()); }
        }

        public override Image TypeImage16
		{
			get { return Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.pump_DataPump_16; }
		}

		public override Image TypeImage24
		{
			get { return Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.pump_DataPump_24; }
		}

		/// <summary>
		/// Конструктор
		/// </summary>
		public DataPumpUI(string programIdentifier)
			: base(programIdentifier)
        {
			Caption = "Закачка данных";
			this.programIdentifier = programIdentifier;
	    }

        /// <summary>
        /// Инициализация таймера опроса сервера
        /// </summary>
        private void InitTimer()
        {
            if (timer == null)
            {
                timerDelegate = new TimerCallback(OnTimerElapsed);
                timerDueTime = Timeout.Infinite;
                timer = new System.Threading.Timer(timerDelegate, null, timerDueTime, timerPeriod);

                // Вызываем объект управления временeм жизни таймера
                ILease lease = (ILease)timer.InitializeLifetimeService();
                if (lease.CurrentState == LeaseState.Initial)
                {
                    // Он будет жить вечно!
                    lease.InitialLeaseTime = TimeSpan.Zero;
                }
            }
        }

        /// <summary>
        /// Инициализация интерфейса
        /// </summary>
		public override void Initialize()
		{
            base.Initialize();

            InfragisticsRusification.LocalizeAll();

			dataSourcesManager = Workplace.ActiveScheme.DataSourceManager;
            dataPumpManager = Workplace.ActiveScheme.DataPumpManager;
            
			// Назначение обработчиков
            SetFormsEvents();

			// Гриды
            SetGridsEvents();
 
			// Кнопки
            SetButtonsEvents();

			// События мыши
            SetMouseEvents();

            // События контролов файлового менеджера
            SetFileManagerEvents();

            // События табконтролов
            SetTabControlsEvents();

            // События контролов шедулера
            SetSchedulerEvents();

            // Инициализация делегатов
            InitDelegates();

            // Заполнение выпадающих списков дисков
            InitDrivesBtnDropDown();

            // Инициализация таймера опроса сервера
            InitTimer();

            dpv.utcViews.Style = UltraTabControlStyle.Wizard;
		}

        /// <summary>
        /// Устанавливает события шедулера
        /// </summary>
        private void SetSchedulerEvents()
        {
            dpv.uceSchedulePeriod.SelectionChanged += new EventHandler(uceSchedulePeriod_SelectionChanged);
            dpv.ubtnScheduleMonths.Click += new EventHandler(ubtnScheduleMonths_Click);
            dpv.rbMonthlyByDayNumbers.CheckedChanged += new EventHandler(rbMonthlyByDayNumbers_CheckedChanged);
            dpv.rbMonthlyByWeekDays.CheckedChanged += new EventHandler(rbMonthlyByWeekDays_CheckedChanged);
            scheduleMonths.Shown += new EventHandler(scheduleMonths_Shown);
            dpv.udteScheduleStartTime.ValueChanged += new EventHandler(udteScheduleStartTime_ValueChanged);
            dpv.udteScheduleStartDate.ValueChanged += new EventHandler(udteScheduleStartTime_ValueChanged);
            dpv.nudMonthlyByDays.ValueChanged += new EventHandler(udteScheduleStartTime_ValueChanged);
            dpv.nudScheduleByDay.ValueChanged += new EventHandler(udteScheduleStartTime_ValueChanged);
            dpv.nudScheduleByWeek.ValueChanged += new EventHandler(udteScheduleStartTime_ValueChanged);
            dpv.uceMonday.CheckedChanged += new EventHandler(uceMonday_CheckedChanged);
            dpv.uceTuesday.CheckedChanged += new EventHandler(uceMonday_CheckedChanged);
            dpv.uceWednesday.CheckedChanged += new EventHandler(uceMonday_CheckedChanged);
            dpv.uceThursday.CheckedChanged += new EventHandler(uceMonday_CheckedChanged);
            dpv.uceFriday.CheckedChanged += new EventHandler(uceMonday_CheckedChanged);
            dpv.uceSaturday.CheckedChanged += new EventHandler(uceMonday_CheckedChanged);
            dpv.uceSunday.CheckedChanged += new EventHandler(uceMonday_CheckedChanged);
            dpv.uceWeekDay.SelectionChanged += new EventHandler(uceWeekDay_SelectionChanged);
            dpv.uceWeekNumber.SelectionChanged += new EventHandler(uceWeekDay_SelectionChanged);
        }

        /// <summary>
        /// Инициализация делегатов
        /// </summary>
        private void InitDelegates()
        {
            // Назначение делегатов для прогрессов
            upb_SetPositionDelegate = new SetUpbPositionDelegate(upb_SetPositionHandler);
            upb_SetTextDelegate = new SetUpbTextDelegate(upb_SetTextHandler);
            upb_SetVisibleDelegate = new SetUpbVisibleDelegate(upb_SetVisibleHandler);

            // Назначение делегатов для лабелов
            ul_SetTextDelegate = new SetUlTextDelegate(ul_SetTextHandler);
            ul_SetVisibleDelegate = new SetUlVisibleDelegate(ul_SetVisibleHandler);

            // Назначение делегатов для кнопок
            ubtn_SetEnableDelegate = new SetUbtnEnableDelegate(ubtn_SetEnableHandler);
            //ubtn_SetImageDelegate = new SetUbtnImageDelegate(ubtn_SetImageHandler);
            //ubtn_SetTextDelegate = new SetUbtnTextDelegate(ubtn_SetTextHandler);

            //setCursorDelegate = new SetCursorDelegate(SetControlCursorHandler);

            //showDialogForm = new ShowDialogFormDelegate(this.ShowDialogHandler);

            setSelectedTab = new SetSelectedTabDelegate(SetSelectedTabHandler);
        }

        /// <summary>
        /// Устанавливает события табконтролов
        /// </summary>
        private void SetTabControlsEvents()
        {
            dpv.utcViews.PropertyChanged += new Infragistics.Win.PropertyChangedEventHandler(utcViews_PropertyChanged);

            dpv.utcPumpControl.SelectedTabChanged += new SelectedTabChangedEventHandler(utcPumpControl_SelectedTabChanged);
            dpv.utcViews.SelectedTabChanged += new SelectedTabChangedEventHandler(utcViews_SelectedTabChanged);
        }

        /// <summary>
        /// Устанавливает события контролов файлового менеджера
        /// </summary>
        private void SetFileManagerEvents()
        {
            dpv.wbLeftBrowser.Navigated += new WebBrowserNavigatedEventHandler(wbLeftBrowser_Navigated);
            dpv.wbRightBrowser.Navigated += new WebBrowserNavigatedEventHandler(wbRightBrowser_Navigated);

            dpv.scBrowsers.SplitterMoved += new SplitterEventHandler(scBrowsers_SplitterMoved);

            dpv.tsbLeftBack.ButtonClick += new EventHandler(tsbLeftBack_ButtonClick);
            dpv.tsbLeftBack.DropDownItemClicked += new ToolStripItemClickedEventHandler(tsbLeftBack_DropDownItemClicked);
            dpv.tsbLeftBack.DropDownOpening += new EventHandler(tsbLeftBack_DropDownOpening);
            dpv.tsbLeftForward.ButtonClick += new EventHandler(tsbLeftForward_ButtonClick);
            dpv.tsbLeftForward.DropDownItemClicked += new ToolStripItemClickedEventHandler(tsbLeftBack_DropDownItemClicked);
            dpv.tsbLeftForward.DropDownOpening += new EventHandler(tsbLeftForward_DropDownOpening);
            dpv.tsbLeftRefresh.Click += new EventHandler(tsbLeftRefresh_Click);
            dpv.tsbLeftHome.Click += new EventHandler(tsbLeftHome_Click);
            dpv.tsbLeftDrives.ButtonClick += new EventHandler(tsbLeftDrives_ButtonClick);
            dpv.tsbLeftDrives.DropDownItemClicked += new ToolStripItemClickedEventHandler(tsbLeftDrives_DropDownItemClicked);
            dpv.tsbLeftUp.Click += new EventHandler(tsbLeftUp_Click);
            dpv.tsbLeftView.DropDownItemClicked += new ToolStripItemClickedEventHandler(tsbLeftView_DropDownItemClicked);
            dpv.tsbLeftHome.Click += new EventHandler(tsbLeftHome_Click);

            dpv.tsbRightBack.ButtonClick += new EventHandler(tsbRightBack_ButtonClick);
            dpv.tsbRightBack.DropDownItemClicked += new ToolStripItemClickedEventHandler(tsbRightBack_DropDownItemClicked);
            dpv.tsbRightBack.DropDownOpening += new EventHandler(tsbRightBack_DropDownOpening);
            dpv.tsbRightForward.ButtonClick += new EventHandler(tsbRightForward_ButtonClick);
            dpv.tsbRightForward.DropDownItemClicked += new ToolStripItemClickedEventHandler(tsbRightBack_DropDownItemClicked);
            dpv.tsbRightForward.DropDownOpening += new EventHandler(tsbRightForward_DropDownOpening);
            dpv.tsbRightRefresh.Click += new EventHandler(tsbRightRefresh_Click);
            dpv.tsbRightHome.Click += new EventHandler(tsbRightHome_Click);
            dpv.tsbRightDrives.ButtonClick += new EventHandler(tsbRightDrives_ButtonClick);
            dpv.tsbRightDrives.DropDownItemClicked += new ToolStripItemClickedEventHandler(tsbRightDrives_DropDownItemClicked);
            dpv.tsbRightUp.Click += new EventHandler(tsbRightUp_Click);
            dpv.tsbRightView.DropDownItemClicked += new ToolStripItemClickedEventHandler(tsbRightView_DropDownItemClicked);
            dpv.tsbRightHome.Click += new EventHandler(tsbRightHome_Click);
        }

        /// <summary>
        /// Устанавливает события форм
        /// </summary>
        private void SetFormsEvents()
        {
            dpv.Load += new EventHandler(dpv_Load);
            dpv.SizeChanged += new EventHandler(dpv_SizeChanged);
        }

        /// <summary>
        /// Устанавливает события гридов
        /// </summary>
        private void SetGridsEvents()
        {
            //dpn.ugPumpRegistry.AfterRowActivate += new EventHandler(ugPumpRegistry_AfterRowActivate);
            //dpn.ugPumpRegistry.BeforeRowDeactivate += new CancelEventHandler(ugPumpRegistry_BeforeRowDeactivate);

            //CheckBoxOnHeader checkBoxOnHeader = new CheckBoxOnHeader(typeof(string), CheckState.Checked, dpn.ugPumpRegistry);
            //dpn.ugPumpRegistry.CreationFilter = checkBoxOnHeader;

            //dpv.ugPumpHistory.AfterRowActivate += new EventHandler(ugPumpHistory_AfterRowActivate);
            dpv.ugPumpHistory.ClickCellButton += new CellEventHandler(ugPumpHistory_ClickCellButton);
            //dpv.ugPumpHistory.InitializeRow += new InitializeRowEventHandler(ugPumpHistory_InitializeRow);

            //dpv.ugDataSources.AfterRowActivate += new EventHandler(ugDataSources_AfterRowActivate);
            dpv.ugDataSources.ClickCellButton += new CellEventHandler(ugDataSources_ClickCellButton);
            //dpv.ugDataSources.InitializeRow += new InitializeRowEventHandler(ugDataSources_InitializeRow);

            dpv.ugReportData.AfterRowActivate += new EventHandler(ugReportData_AfterRowActivate);
            dpv.ugFixedParameters.CellChange += new CellEventHandler(ugFixedParameters_CellChange);
        }

        /// <summary>
        /// Устанавливает события кнопок
        /// </summary>
        private void SetButtonsEvents()
        {
            dpv.ubtnStartPreviewData.Click += new EventHandler(ubtnStartPreviewData_Click);
            dpv.ubtnPausePreviewData.Click += new EventHandler(ubtnPauseCurrent_Click);
            dpv.ubtnStopPreviewData.Click += new EventHandler(ubtnStopCurrent_Click);
            dpv.ubtnSkipPreviewData.Click += new EventHandler(ubtnSkipPreviewData_Click);

            dpv.ubtnStartPumpData.Click += new EventHandler(ubtnStartPumpData_Click);
            dpv.ubtnPausePumpData.Click += new EventHandler(ubtnPauseCurrent_Click);
            dpv.ubtnStopPumpData.Click += new EventHandler(ubtnStopCurrent_Click);
            dpv.ubtnSkipPumpData.Click += new EventHandler(ubtnSkipPumpData_Click);

            dpv.ubtnStartProcessData.Click += new EventHandler(ubtnStartProcessData_Click);
            dpv.ubtnPauseProcessData.Click += new EventHandler(ubtnPauseCurrent_Click);
            dpv.ubtnStopProcessData.Click += new EventHandler(ubtnStopCurrent_Click);
            dpv.ubtnSkipProcessData.Click += new EventHandler(ubtnSkipProcessData_Click);

            dpv.ubtnStartAssociateData.Click += new EventHandler(ubtnStartAssociateData_Click);
            dpv.ubtnPauseAssociateData.Click += new EventHandler(ubtnPauseCurrent_Click);
            dpv.ubtnStopAssociateData.Click += new EventHandler(ubtnStopCurrent_Click);
            dpv.ubtnSkipAssociateData.Click += new EventHandler(ubtnSkipAssociateData_Click);

            dpv.ubtnStartProcessCube.Click += new EventHandler(ubtnStartProcessCube_Click);
            dpv.ubtnPauseProcessCube.Click += new EventHandler(ubtnPauseCurrent_Click);
            dpv.ubtnStopProcessCube.Click += new EventHandler(ubtnStopCurrent_Click);
            dpv.ubtnSkipProcessCube.Click += new EventHandler(ubtnSkipProcessCube_Click);

            dpv.ubtnStartCheckData.Click += new EventHandler(ubtnStartCheckData_Click);
            dpv.ubtnPauseCheckData.Click += new EventHandler(ubtnPauseCurrent_Click);
            dpv.ubtnStopCheckData.Click += new EventHandler(ubtnStopCurrent_Click);
            dpv.ubtnSkipCheckData.Click += new EventHandler(ubtnSkipCheckData_Click);

            dpv.ubtnApplySchedule.Click += new EventHandler(ubtnApplySchedule_Click);
            dpv.ubtnCancelSchedule.Click += new EventHandler(ubtnCancelSchedule_Click);

            dpv.ubtnApplyPreview.Click += new EventHandler(ubtnApplyPreview_Click);
            dpv.ubtnCancelPreview.Click += new EventHandler(ubtnCancelPreview_Click);

            dpv.tsbRefresh.Click += new EventHandler(tsbRefresh_Click);
        }

        

        /// <summary>
        /// Устанавливает события мыши
        /// </summary>
        private void SetMouseEvents()
        {
            for (int i = 0; i < dpv.utpcPumpRuling.Controls.Count; i++)
            {
                if (dpv.utpcPumpRuling.Controls[i] is UltraButton)
                {
                    UltraButton btn = (UltraButton)dpv.utpcPumpRuling.Controls[i];
                    if (btn.Tag != null)
                    {
                        btn.MouseEnter += new EventHandler(ubtnStartPumpData_MouseEnter);
                        btn.MouseLeave += new EventHandler(ubtnStartPumpData_MouseLeave);
                    }
                }
                if (dpv.utpcPumpRuling.Controls[i] is Infragistics.Win.UltraWinEditors.UltraPictureBox)
                {
                    Infragistics.Win.UltraWinEditors.UltraPictureBox pic =
                        (Infragistics.Win.UltraWinEditors.UltraPictureBox)dpv.utpcPumpRuling.Controls[i];
                    if (pic.Tag != null)
                    {
                        pic.MouseEnter += new EventHandler(upicPumpData_MouseEnter);
                        pic.MouseLeave += new EventHandler(ubtnStartPumpData_MouseLeave);
                    }
                }
            }
        }

        /// <summary>
        /// Метод, вызываемый при активации/деактивации интерфейса
        /// </summary>
        public override void Activate(bool Activated)
        {
            base.Activate(Activated);

            //clientContext = LogicalCallContextData.GetContext();

            dpv.utcViews.Style = UltraTabControlStyle.Wizard;

            SyncTabs(Activated);

            if (dataPumpProgress != null)
            {
                Workplace.ViewObjectCaption = pumpRegistryElement.Name;
            }
        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
		public override void InternalFinalize()
		{
            if (pumpParams != null)
                pumpParams.workplace = null;

            if (scheduleMonths != null)
                scheduleMonths.Dispose();

            if (timer != null)
                timer.Dispose();

			base.InternalFinalize();
		}

		#endregion Инициализация


        #region Общие функции

        
        /// <summary>
        /// Показывает окно с сообщением об ошибке
        /// </summary>
        /// <param name="msg">Сообщение</param>
        private static void ShowErrorMessage(string msg)
        {
            MessageBox.Show(msg, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Массив для преобразования номеров месяцев в названия
        /// </summary>
        private string[] MonthByNumber = new string[12] {
            "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь",
            "Ноябрь", "Декабрь" };

        /// <summary>
        /// Массив для преобразования номера типа источника данных в название
        /// </summary>
        private string[] KindsOfParamsByNumber = new string[7] { 
            "Финансовый орган, год", "Год", "Год, месяц", "Год, месяц, вариант", "Год, вариант", 
            "Год, квартал", "Год, территория" };

        /// <summary>
        /// Запускает таймер
        /// </summary>
        public void StartTimer()
        {
            timerDueTime = 0;
            timer.Change(timerDueTime, timerPeriod);
        }

        /// <summary>
        /// Останавливает таймер
        /// </summary>
        public void StopTimer()
        {
            timerDueTime = Timeout.Infinite;
            timer.Change(timerDueTime, timerPeriod);
        }

        /*/// <summary>
        /// Устанавливает период таймера
        /// </summary>
        /// <param name="period">Период таймера в миллисекундах</param>
        private void SetTimerPeriod(int period)
        {
            timerPeriod = period;

            if (timerDueTime != Timeout.Infinite)
            {
                timer.Change(timerPeriod, timerPeriod);
            }
            else
            {
                timer.Change(timerDueTime, timerPeriod);
            }
        }*/

        #endregion Общие функции


        #region Инициализация программ закачки

        /// <summary>
        /// Очистка обработчиков событий 
        /// </summary>
        private void ClearDataPumpProgressHandling()
        {
            if (dataPumpProgressHandling != null)
            {
                dataPumpProgressHandling.PumpProcessStateChanged -=
                    new PumpProcessStateChangedDelegate(dataPumpProgressHandling_PumpProcessStateChanged);
                dataPumpProgressHandling.StageFinished -= new GetPumpStateDelegate(dataPumpProgressHandling_StageFinished);
                dataPumpProgressHandling.StagePaused -= new GetPumpStateDelegate(dataPumpProgressHandling_StagePaused);
                dataPumpProgressHandling.StageResumed -= new GetPumpStateDelegate(dataPumpProgressHandling_StageResumed);
                dataPumpProgressHandling.StageSkipped -= new GetPumpStateDelegate(dataPumpProgressHandling_StageSkipped);
                dataPumpProgressHandling.StageStarted -= new GetPumpStateDelegate(dataPumpProgressHandling_StageStarted);
                dataPumpProgressHandling.StageStopped -= new GetPumpStateDelegate(dataPumpProgressHandling_StageStopped);
                dataPumpProgressHandling.PumpFailure -= new Krista.FM.ServerLibrary.GetStringDelegate(dataPumpProgressHandling_PumpFailure);

                dataPumpProgressHandling.Dispose();
            }
        }

        /// <summary>
        /// Назначение обработчиков событий закачки
        /// </summary>
        private void InitDataPumpProgressHandling()
        {
            ClearDataPumpProgressHandling();

            dataPumpProgressHandling = new DataPumpProgressHandling();
            dataPumpProgressHandling.DataPumpProgress = dataPumpProgress;

            dataPumpProgressHandling.PumpProcessStateChanged +=
                new PumpProcessStateChangedDelegate(dataPumpProgressHandling_PumpProcessStateChanged);
            dataPumpProgressHandling.StageFinished += new GetPumpStateDelegate(dataPumpProgressHandling_StageFinished);
            dataPumpProgressHandling.StagePaused += new GetPumpStateDelegate(dataPumpProgressHandling_StagePaused);
            dataPumpProgressHandling.StageResumed += new GetPumpStateDelegate(dataPumpProgressHandling_StageResumed);
            dataPumpProgressHandling.StageSkipped += new GetPumpStateDelegate(dataPumpProgressHandling_StageSkipped);
            dataPumpProgressHandling.StageStarted += new GetPumpStateDelegate(dataPumpProgressHandling_StageStarted);
            dataPumpProgressHandling.StageStopped += new GetPumpStateDelegate(dataPumpProgressHandling_StageStopped);
            dataPumpProgressHandling.PumpFailure += new Krista.FM.ServerLibrary.GetStringDelegate(dataPumpProgressHandling_PumpFailure);
        }

        /// <summary>
        /// Назначение обработчиков событий пула закачек
        /// </summary>
        private void InitPumpSchedulerHandling()
        {
            if (pumpSchedulerHandling != null)
            {
                pumpSchedulerHandling.ScheduleIsChanged -= new Krista.FM.ServerLibrary.GetStringDelegate(pumpSchedulerHandling_ScheduleIsChanged);

                pumpSchedulerHandling.Dispose();
            }

            pumpSchedulerHandling = new PumpSchedulerHandling();
            pumpSchedulerHandling.PumpScheduler = dataPumpManager.PumpScheduler;

            pumpSchedulerHandling.ScheduleIsChanged += new Krista.FM.ServerLibrary.GetStringDelegate(pumpSchedulerHandling_ScheduleIsChanged);
        }

        /// <summary>
        /// Инициализация модуля закачки
        /// </summary>
        private void InitializePumpModule()
        {
            try
            {
                if (pumpRegistryElement == null) return;

                Workplace.OperationObj.Text = "Инициализация интерфейса закачки...";
                Workplace.OperationObj.StartOperation();

                // Назначение обработчиков событий закачки
                InitDataPumpProgressHandling();

                // Назначение обработчиков событий расписания закачки
                InitPumpSchedulerHandling();

                // Устанавливаем контролы на выбранной закладке
                SyncTabs(true);

                if (dataPumpProgress.PumpInProgress)
                {
                    StartTimer();
                }
            }
            finally
            {
                Workplace.OperationObj.StopOperation();
            }
        }

        /// <summary>
        /// Проверяет права
        /// </summary>
        private void CheckPermissions()
        {
            if (dataPumpProgress == null) return;

            startPumpEnabled = Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject(
                pumpRegistryElement.ProgramIdentifier, (int)DataPumpOperations.StartPump, false);

            stopPumpEnabled = Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject(
                pumpRegistryElement.ProgramIdentifier, (int)DataPumpOperations.StopPump, false);

            deletePumpEnabled = Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject(
                pumpRegistryElement.ProgramIdentifier, (int)DataPumpOperations.DeletePump, false);
        }

        /*
        private bool PingHost(string hostName)
        {
            return true;
/*
 * Убрано по просьбе Барковой
 */
            /*
            // Получаем имя хоста
            string host = hostName.TrimStart('\\');
            if (host.Trim() == string.Empty)
            {
                return false;
            }

            string[] strings = host.Split('\\');
            if (strings.GetLength(0) > 0)
            {
                Ping ping = new Ping();
                try
                {
                    PingReply reply = ping.Send(strings[0], 10);
                    if (reply.Status == IPStatus.Success)
                    {
                        return true;
                    }
                }
                finally
                {
                    ping.Dispose();
                }
            }

            return false;
        }*/

        /// <summary>
        /// Возвращает путь к источникам данных
        /// </summary>
        private string GetDataSourcesUNCPath()
        {
            string path = pumpRegistryElement.DataSourcesUNCPath;

            
            /*if (!PingHost(path))
            {
                throw new Exception(string.Format("Каталог источников данных {0} недоступен", path));
            }*/

            return path;
        }

        #endregion Инициализация программ закачки


        #region Функции установки параметров контролов

        #region Функции установки свойств прогрессов

        /// <summary>
        /// Установка текущей позиции прогресса
        /// </summary>
        /// <param name="upb">Компонент</param>
        /// <param name="position">Позиция</param>
		public void upb_SetPositionHandler(UltraProgressBar upb, int position)
		{
            if (position <= upb.Maximum)
            {
                upb.Value = position;
            }
		}

		/// <summary>
        /// Установка текста прогресса
		/// </summary>
		/// <param name="upb">Компонент</param>
		/// <param name="text">Текст</param>
		public void upb_SetTextHandler(UltraProgressBar upb, string text)
		{
            if (text == string.Empty)
            {
                upb.Text = "[Formatted]";
            }
            else
            {
                upb.Text = text;
            }
		}

		/// <summary>
        /// Установка видимости прогресса
		/// </summary>
        /// <param name="upb">Компонент</param>
		/// <param name="visible">Видимость</param>
		public void upb_SetVisibleHandler(UltraProgressBar upb, bool visible)
		{
            if (upb.Visible != visible)
            {
                upb.Visible = visible;
            }
        }

        #endregion Функции установки свойств прогрессов


        #region Функции установки свойств лабелов

        /// <summary>
        /// Установка текста лабела
		/// </summary>
		/// <param name="ul">Компонент</param>
		/// <param name="text">Текст</param>
		public void ul_SetTextHandler(UltraLabel ul, string text)
		{
			if (text == string.Empty) ul.Text = "Нет данных";
			else ul.Text = text;
		}

		/// <summary>
        /// Установка видимости лабела
		/// </summary>
		/// <param name="ul">Компонент</param>
		/// <param name="visible">Видимость</param>
		public void ul_SetVisibleHandler(UltraLabel ul, bool visible)
		{
			if (ul.Visible != visible) ul.Visible = visible;
        }

        #endregion Функции установки свойств лабелов


        #region Функции установки свойств кнопок

        /// <summary>
		/// Установка состояния разрешения кнопки
		/// </summary>
		/// <param name="ubtn">Компонент</param>
		/// <param name="enable">Разрешение</param>
		public void ubtn_SetEnableHandler(UltraButton ubtn, bool enable)
		{
			if (ubtn.Enabled != enable) ubtn.Enabled = enable;
		}

		/// <summary>
        /// Установка иконки кнопки
		/// </summary>
		/// <param name="ubtn">Компонент</param>
		/// <param name="bmp">Иконка</param>
		public void ubtn_SetImageHandler(UltraButton ubtn, Bitmap bmp)
		{
			if (ubtn.Appearance.Image != bmp) ubtn.Appearance.Image = bmp;
        }

        /// <summary>
        /// Установка текста кнопки
        /// </summary>
        /// <param name="ubtn">Компонент</param>
        /// <param name="text">Текст</param>
        public void ubtn_SetTextHandler(UltraButton ubtn, string text)
        {
            if (ubtn.Text != text) ubtn.Text = text;
        }

        #endregion Функции установки свойств кнопок


        #region Функции установки общих свойств контролов

        /// <summary>
        /// Устанавливает курсор контрола
        /// </summary>
        /// <param name="cursor">Курсор</param>
        public void SetControlCursorHandler(Cursor cursor)
        {
            dpv.Cursor = cursor;
        }

        /// <summary>
        /// Показывает форму как диалог
        /// </summary>
        /// <param name="form">Форма</param>
        /// <returns>Результат диалога</returns>
        public DialogResult ShowDialogHandler(Form form)
        {
            return form.ShowDialog();
        }

        #endregion Функции установки общих свойств контролов


        #region Функции установки свойств формы

        public void SetSelectedTabHandler(UltraTabControl utc, UltraTab tab)
        {
            utc.SelectedTab = tab;
        }

        #endregion Функции установки свойств формы

        #endregion Функции установки параметров контролов


        #region Функции управления состоянием контролов


        #region Функции управления кнопками

        /*/// <summary>
		/// Устанавливает рисунок на кнопке
		/// </summary>
		/// <param name="btn">Кнопка</param>
		/// <param name="bmp">Рисунок</param>
		private void SetButtonImage(UltraButton btn, Bitmap bmp)
		{
			if (btn == null) return;
			btn.Invoke(ubtn_SetImageDelegate, new object[] { btn, bmp });
		}*/

		/// <summary>
		/// Устанавливает состояние кнопки
		/// </summary>
		/// <param name="btn">Кнопка</param>
		/// <param name="enabled">Состояние</param>
		private void SetButtonEnabled(UltraButton btn, bool enabled)
		{
			if (btn == null) return;
			if (btn.Enabled != enabled)	btn.Invoke(ubtn_SetEnableDelegate, new object[] { btn, enabled });
		}

        /*
        /// <summary>
        /// Устанавливает текст кнопки
        /// </summary>
        /// <param name="btn">Кнопка</param>
        /// <param name="text">Текст</param>
        private void SetButtonText(UltraButton btn, string text)
        {
            if (btn == null) return;
            btn.Invoke(ubtn_SetTextDelegate, new object[] { btn, text });
        }*/

		/*
        /// <summary>
		/// Устанавливает состояние кнопок для указанной закладки
		/// </summary>
		/// <param name="ut">Закладка</param>
		/// <param name="enabled">Состояние</param>
		private void SetAllButtonsEnabledForTab(UltraTab ut, bool enabled)
		{
			UltraButton btn;

			for (int i = 0; i < ut.TabPage.Controls.Count; i++)
			{
				if (ut.TabPage.Controls[i] is UltraButton)
				{
					btn = (UltraButton)ut.TabPage.Controls[i];
					SetButtonEnabled(btn, enabled);
				}
			}
		}*/

		/// <summary>
		/// Устанавливает состояние кнопок, соответствующих состоянию закачки
		/// </summary>
		/// <param name="state">Состояние закачки</param>
		private void SetAllButtonsForPumpState(PumpProcessStates state)
		{
			switch (state)
			{
				case PumpProcessStates.Aborted:
                    break;

				case PumpProcessStates.Finished:
				case PumpProcessStates.Prepared:
                    SetStageButtonsGroupEnabled(PumpProcessStates.PreviewData, true, false, false, true);
                    SetStageButtonsGroupEnabled(PumpProcessStates.PumpData, true, false, false, true);
                    SetStageButtonsGroupEnabled(PumpProcessStates.ProcessData, true, false, false, true);
                    SetStageButtonsGroupEnabled(PumpProcessStates.AssociateData, true, false, false, true);
                    SetStageButtonsGroupEnabled(PumpProcessStates.ProcessCube, true, false, false, true);
                    SetStageButtonsGroupEnabled(PumpProcessStates.CheckData, true, false, false, true);
					break;

                case PumpProcessStates.PreviewData:
                    SetStageButtonsGroupEnabled(PumpProcessStates.PreviewData, false, true, true, false);
                    SetStageButtonsGroupEnabled(PumpProcessStates.PumpData, false, false, false, true);
                    SetStageButtonsGroupEnabled(PumpProcessStates.ProcessData, false, false, false, true);
                    SetStageButtonsGroupEnabled(PumpProcessStates.AssociateData, false, false, false, true);
                    SetStageButtonsGroupEnabled(PumpProcessStates.ProcessCube, false, false, false, true);
                    SetStageButtonsGroupEnabled(PumpProcessStates.CheckData, false, false, false, true);
                    break;

				case PumpProcessStates.PumpData:
                    SetStageButtonsGroupEnabled(PumpProcessStates.PreviewData, false, false, false, false);
                    SetStageButtonsGroupEnabled(PumpProcessStates.PumpData, false, true, true, false);
                    SetStageButtonsGroupEnabled(PumpProcessStates.ProcessData, false, false, false, true);
                    SetStageButtonsGroupEnabled(PumpProcessStates.AssociateData, false, false, false, true);
                    SetStageButtonsGroupEnabled(PumpProcessStates.ProcessCube, false, false, false, true);
                    SetStageButtonsGroupEnabled(PumpProcessStates.CheckData, false, false, false, true);
					break;

				case PumpProcessStates.ProcessData:
                    SetStageButtonsGroupEnabled(PumpProcessStates.PreviewData, false, false, false, false);
                    SetStageButtonsGroupEnabled(PumpProcessStates.PumpData, false, false, false, false);
                    SetStageButtonsGroupEnabled(PumpProcessStates.ProcessData, false, true, true, false);
                    SetStageButtonsGroupEnabled(PumpProcessStates.AssociateData, false, false, false, true);
                    SetStageButtonsGroupEnabled(PumpProcessStates.ProcessCube, false, false, false, true);
                    SetStageButtonsGroupEnabled(PumpProcessStates.CheckData, false, false, false, true);
					break;

				case PumpProcessStates.AssociateData:
                    SetStageButtonsGroupEnabled(PumpProcessStates.PreviewData, false, false, false, false);
                    SetStageButtonsGroupEnabled(PumpProcessStates.PumpData, false, false, false, false);
                    SetStageButtonsGroupEnabled(PumpProcessStates.ProcessData, false, false, false, false);
                    SetStageButtonsGroupEnabled(PumpProcessStates.AssociateData, false, true, true, false);
                    SetStageButtonsGroupEnabled(PumpProcessStates.ProcessCube, false, false, false, true);
                    SetStageButtonsGroupEnabled(PumpProcessStates.CheckData, false, false, false, true);
					break;

				case PumpProcessStates.ProcessCube:
                    SetStageButtonsGroupEnabled(PumpProcessStates.PreviewData, false, false, false, false);
                    SetStageButtonsGroupEnabled(PumpProcessStates.PumpData, false, false, false, false);
                    SetStageButtonsGroupEnabled(PumpProcessStates.ProcessData, false, false, false, false);
                    SetStageButtonsGroupEnabled(PumpProcessStates.AssociateData, false, false, false, false);
                    SetStageButtonsGroupEnabled(PumpProcessStates.ProcessCube, false, true, true, false);
                    SetStageButtonsGroupEnabled(PumpProcessStates.CheckData, false, false, false, true);
					break;

				case PumpProcessStates.CheckData:
                    SetStageButtonsGroupEnabled(PumpProcessStates.PreviewData, false, false, false, false);
                    SetStageButtonsGroupEnabled(PumpProcessStates.PumpData, false, false, false, false);
                    SetStageButtonsGroupEnabled(PumpProcessStates.ProcessData, false, false, false, false);
                    SetStageButtonsGroupEnabled(PumpProcessStates.AssociateData, false, false, false, false);
                    SetStageButtonsGroupEnabled(PumpProcessStates.ProcessCube, false, false, false, false);
                    SetStageButtonsGroupEnabled(PumpProcessStates.CheckData, false, true, true, false);
					break;

				case PumpProcessStates.DeleteData:
					break;
			}
		}

        /// <summary>
        /// Устанавливает состояние группы кнопок указанного этапа
        /// </summary>
        /// <param name="state"></param>
        /// <param name="startEnabled"></param>
        /// <param name="pauseEnabled"></param>
        /// <param name="stopEnabled"></param>
        /// <param name="skipEnabled"></param>
        private void SetStageButtonsGroupEnabled(PumpProcessStates state, bool startEnabled, bool pauseEnabled,
			bool stopEnabled, bool skipEnabled)
		{
            IStagesQueueElement sqe = pumpRegistryElement.StagesQueue[state];

            if (sqe.StageInitialState == StageState.Blocked)
            {
                startEnabled = false;
                pauseEnabled = false;
                stopEnabled = false;
                skipEnabled = false;
            }
            else if (sqe.StageInitialState == StageState.Skipped)
            {
                startEnabled = false;
                pauseEnabled = false;
                stopEnabled = false;
                skipEnabled = true;
            }
            else
            {
                if (startEnabled)
                {
                    startEnabled = startPumpEnabled;
                }

                if (stopEnabled)
                {
                    stopEnabled = stopPumpEnabled;
                }
            }

			switch (state)
			{
                    // кнопки пауза и стоп нихуя не работают, нажмешь на них и песда :), временно дизэйблю 
                case PumpProcessStates.PreviewData:
                    SetButtonEnabled(dpv.ubtnStartPreviewData, startEnabled);
                    SetButtonEnabled(dpv.ubtnPausePreviewData, false);
                    SetButtonEnabled(dpv.ubtnStopPreviewData, false);
                    SetButtonEnabled(dpv.ubtnSkipPreviewData, skipEnabled);
                    break;

				case PumpProcessStates.PumpData:
					SetButtonEnabled(dpv.ubtnStartPumpData, startEnabled);
                    SetButtonEnabled(dpv.ubtnPausePumpData, false);
                    SetButtonEnabled(dpv.ubtnStopPumpData, false);
					SetButtonEnabled(dpv.ubtnSkipPumpData, skipEnabled);
					break;

				case PumpProcessStates.ProcessData:
					SetButtonEnabled(dpv.ubtnStartProcessData, startEnabled);
                    SetButtonEnabled(dpv.ubtnPauseProcessData, false);
                    SetButtonEnabled(dpv.ubtnStopProcessData, false);
					SetButtonEnabled(dpv.ubtnSkipProcessData, skipEnabled);
					break;

				case PumpProcessStates.AssociateData:
					SetButtonEnabled(dpv.ubtnStartAssociateData, startEnabled);
                    SetButtonEnabled(dpv.ubtnPauseAssociateData, false);
					SetButtonEnabled(dpv.ubtnStopAssociateData, false);
					SetButtonEnabled(dpv.ubtnSkipAssociateData, skipEnabled);
					break;

				case PumpProcessStates.ProcessCube:
					SetButtonEnabled(dpv.ubtnStartProcessCube, startEnabled);
					SetButtonEnabled(dpv.ubtnPauseProcessCube, false);
					SetButtonEnabled(dpv.ubtnStopProcessCube, false);
					SetButtonEnabled(dpv.ubtnSkipProcessCube, skipEnabled);
					break;

				case PumpProcessStates.CheckData:
					SetButtonEnabled(dpv.ubtnStartCheckData, startEnabled);
					SetButtonEnabled(dpv.ubtnPauseCheckData, false);
					SetButtonEnabled(dpv.ubtnStopCheckData, false);
					SetButtonEnabled(dpv.ubtnSkipCheckData, skipEnabled);
					break;

				case PumpProcessStates.DeleteData:
					break;
			}
		}

		/// <summary>
        /// Устанавливает состояние кнопок выполненных ранее этапов
		/// </summary>
		/// <param name="state"></param>
		/// <param name="startEnabled"></param>
		/// <param name="pauseEnabled"></param>
		/// <param name="stopEnabled"></param>
		/// <param name="skipEnabled"></param>
        private void SetAllStageButtonsGroupEnabled(PumpProcessStates state, bool startEnabled, bool pauseEnabled,
			bool stopEnabled, bool skipEnabled)
		{
            if (state <= PumpProcessStates.PreviewData)
            {
                SetStageButtonsGroupEnabled(
                    PumpProcessStates.PreviewData, startEnabled, pauseEnabled, stopEnabled, skipEnabled);
            }

            if (state <= PumpProcessStates.PumpData)
            {
                SetStageButtonsGroupEnabled(
                    PumpProcessStates.PumpData, startEnabled, pauseEnabled, stopEnabled, skipEnabled);
            }

            if (state <= PumpProcessStates.ProcessData)
            {
                SetStageButtonsGroupEnabled(
                    PumpProcessStates.ProcessData, startEnabled, pauseEnabled, stopEnabled, skipEnabled);
            }

            if (state <= PumpProcessStates.AssociateData)
            {
                SetStageButtonsGroupEnabled(
                    PumpProcessStates.AssociateData, startEnabled, pauseEnabled, stopEnabled, skipEnabled);
            }

            if (state <= PumpProcessStates.ProcessCube)
            {
                SetStageButtonsGroupEnabled(
                    PumpProcessStates.ProcessCube, startEnabled, pauseEnabled, stopEnabled, skipEnabled);
            }

            if (state <= PumpProcessStates.CheckData)
            {
                SetStageButtonsGroupEnabled(
                    PumpProcessStates.CheckData, startEnabled, pauseEnabled, stopEnabled, skipEnabled);
            }
		}

		/// <summary>
		/// Устанавливает состояние кнопок области просмотра (enable/disable)
		/// </summary>
		private void SetButtonsState()
		{
			if (dataPumpProgress == null) return;

			PumpProcessStates st = dataPumpProgress.State;

			switch (dataPumpProgress.State)
			{
				case PumpProcessStates.Prepared:
					break;

				case PumpProcessStates.Running:
				case PumpProcessStates.Paused:
                    IStagesQueueElement sqe = pumpRegistryElement.StagesQueue.GetInProgressQueueElement();
                    if (sqe != null)
                    {
                        st = sqe.State;
                    }
					break;

				case PumpProcessStates.Finished:
					break;

				case PumpProcessStates.Aborted:
					break;
			}

			// Разрешаем какие надо
			SetAllButtonsForPumpState(st);
		}

		#endregion Функции управления кнопками


		#region Функции управления прогрессами

		/// <summary>
		/// Устанавливает видимость прогресса через делегат
		/// </summary>
		/// <param name="upb">Прогресс</param>
		/// <param name="visible">Видимость</param>
		private void SetProgressBarVisible(UltraProgressBar upb, bool visible)
		{
			if (upb.Visible != visible) upb.Invoke(upb_SetVisibleDelegate, new object[] { upb, visible });
		}

		/// <summary>
		/// Устанавливает текст прогресса через делегат
		/// </summary>
		/// <param name="upb">Прогресс</param>
		/// <param name="text">Делегат</param>
		private void SetProgressBarText(UltraProgressBar upb, string text)
		{
			upb.Invoke(upb_SetTextDelegate, new object[] { upb, text });
		}

		/// <summary>
		/// Устанавливает позицию прогресса через делегат
		/// </summary>
		/// <param name="upb">Прогресс</param>
		/// <param name="position">Позиция</param>
		private void SetProgressBarPosition(UltraProgressBar upb, int position)
		{
			upb.Invoke(upb_SetPositionDelegate, new object[] { upb, position });
		}

		/// <summary>
		/// Сбрасывает все прогрессы
		/// </summary>
		private void ClearProgressBars()
		{
            SetProgressBarPosition(dpv.upbPreviewData, 0);
            SetProgressBarText(dpv.upbPreviewData, "[Formatted]");

			SetProgressBarPosition(dpv.upbPumpData, 0);
			SetProgressBarText(dpv.upbPumpData, "[Formatted]");

			SetProgressBarPosition(dpv.upbCheckData, 0);
			SetProgressBarText(dpv.upbCheckData, "[Formatted]");

			SetProgressBarPosition(dpv.upbProcessData, 0);
			SetProgressBarText(dpv.upbProcessData, "[Formatted]");

			SetProgressBarPosition(dpv.upbAssociateData, 0);
			SetProgressBarText(dpv.upbAssociateData, "[Formatted]");

			SetProgressBarPosition(dpv.upbProcessCube, 0);
			SetProgressBarText(dpv.upbProcessCube, "[Formatted]");
		}

		/// <summary>
		/// Скрывает все прогрессы
		/// </summary>
		private void HideProgressBars()
		{
            SetProgressBarVisible(dpv.upbPreviewData, false);
			SetProgressBarVisible(dpv.upbPumpData, false);
			SetProgressBarVisible(dpv.upbCheckData, false);
			SetProgressBarVisible(dpv.upbProcessData, false);
			SetProgressBarVisible(dpv.upbAssociateData, false);
			SetProgressBarVisible(dpv.upbProcessCube, false);
		}

		/// <summary>
		/// Устанавливает состояние прогресса, соответствующего состоянию закачки
		/// </summary>
		/// <param name="state">Состояние закачки</param>
		/// <param name="maxPos">Макс. позиция</param>
		/// <param name="currPos">Текущая позиция</param>
		/// <param name="message">Надпись над прогрессом</param>
		/// <param name="text">Надпись на прогрессe</param>
		private void SetProgressForPumpState(PumpProcessStates state, int maxPos, int currPos, string message, 
            string text)
		{
			switch (state)
			{
                case PumpProcessStates.PreviewData:
                    dpv.upbPreviewData.Maximum = maxPos;
                    SetProgressBarPosition(dpv.upbPreviewData, currPos);
                    SetLabelText(dpv.ulPreviewDataMessage, message);
                    SetProgressBarText(dpv.upbPreviewData, text);
                    break;

				case PumpProcessStates.PumpData:
					dpv.upbPumpData.Maximum = maxPos;
					SetProgressBarPosition(dpv.upbPumpData, currPos);
					SetLabelText(dpv.ulPumpDataMessage, message);
					SetProgressBarText(dpv.upbPumpData, text);
					break;

				case PumpProcessStates.ProcessData:
					dpv.upbProcessData.Maximum = maxPos;
					SetProgressBarPosition(dpv.upbProcessData, currPos);
					SetLabelText(dpv.ulProcessDataMessage, message);
					SetProgressBarText(dpv.upbProcessData, text);
					break;

				case PumpProcessStates.AssociateData:
					dpv.upbAssociateData.Maximum = maxPos;
					SetProgressBarPosition(dpv.upbAssociateData, currPos);
					SetLabelText(dpv.ulAssociateDataMessage, message);
					SetProgressBarText(dpv.upbAssociateData, text);
					break;

				case PumpProcessStates.ProcessCube:
					dpv.upbProcessCube.Maximum = maxPos;
					SetProgressBarPosition(dpv.upbProcessCube, currPos);
					SetLabelText(dpv.ulProcessCubeMessage, message);
					SetProgressBarText(dpv.upbProcessCube, text);
					break;

				case PumpProcessStates.CheckData:
					dpv.upbCheckData.Maximum = maxPos;
					SetProgressBarPosition(dpv.upbCheckData, currPos);
					SetLabelText(dpv.ulCheckDataMessage, message);
					SetProgressBarText(dpv.upbCheckData, text);
					break;

				case PumpProcessStates.DeleteData:
                    Workplace.ProgressObj.Caption = message;
                    Workplace.ProgressObj.MaxProgress = maxPos;
                    Workplace.ProgressObj.Position = currPos;
                    Workplace.ProgressObj.Text = text;
					break;
			}
		}

		/// <summary>
		/// Устанавливает видимость прогресса в зависимости от этапа закачки
		/// </summary>
		/// <param name="state">Этап</param>
		/// <param name="visible">Видимость</param>
		private void SetProgressVisibleForPumpState(PumpProcessStates state, bool visible)
		{
			switch (state)
			{
                case PumpProcessStates.PreviewData:
                    SetProgressBarVisible(dpv.upbPreviewData, visible);
                    break;

				case PumpProcessStates.PumpData:
					SetProgressBarVisible(dpv.upbPumpData, visible);
					break;

				case PumpProcessStates.ProcessData:
					SetProgressBarVisible(dpv.upbProcessData, visible);
					break;

				case PumpProcessStates.AssociateData:
					SetProgressBarVisible(dpv.upbAssociateData, visible);
					break;

				case PumpProcessStates.ProcessCube:
					SetProgressBarVisible(dpv.upbProcessCube, visible);
					break;

				case PumpProcessStates.CheckData:
					SetProgressBarVisible(dpv.upbCheckData, visible);
					break;

				case PumpProcessStates.DeleteData:
					break;
			}
		}

		/*
        /// <summary>
		/// Устанавливает состояние прогрессов области просмотра
		/// </summary>
		private void SetProgressState()
		{
			if (dataPumpProgress.State == PumpProcessStates.Aborted)
			{
				ClearProgressBars();
			}

			SetProgressForPumpState(dataPumpProgress.State, dataPumpProgress.ProgressMaxPos,
				dataPumpProgress.ProgressCurrentPos, dataPumpProgress.ProgressMessage,
				dataPumpProgress.ProgressText);
		}*/

		#endregion Функции управления прогрессами


		#region Функции управления лабелами

		/// <summary>
		/// Устанавливает текст лабела через делегат
		/// </summary>
		/// <param name="ul">Лабел</param>
		/// <param name="text">Текст</param>
		private void SetLabelText(UltraLabel ul, string text)
		{
			ul.Invoke(ul_SetTextDelegate, new object[] { ul, text });
		}

		/// <summary>
		/// Устанавливает видимость лабела через делегат
		/// </summary>
		/// <param name="ul">Лабел</param>
		/// <param name="visible">Видимость</param>
		private void SetLabelVisible(UltraLabel ul, bool visible)
		{
            if (ul.Visible != visible)
            {
                ul.Invoke(ul_SetVisibleDelegate, new object[] { ul, visible });
            }
		}

		/// <summary>
		/// Сбрасывает все лабелы
		/// </summary>
		private void ClearLabels()
		{
			for (int i = 0; i < dpv.utpcPumpRuling.Controls.Count; i++)
			{
				if (dpv.utpcPumpRuling.Controls[i].GetType() == typeof(UltraLabel))
				{
					UltraLabel ul = (UltraLabel)dpv.utpcPumpRuling.Controls[i];
                    if (ul.Tag != null)
                    {
                        SetLabelText(ul, "Нет данных");
                    }
				}
			}
		}

		/// <summary>
		/// Скрывает лабелы с сообщениями закачки
		/// </summary>
		private void HideLabels()
		{
			for (int i = 0; i < dpv.utpcPumpRuling.Controls.Count; i++)
			{
				if (dpv.utpcPumpRuling.Controls[i].GetType() == typeof(UltraLabel))
				{
					UltraLabel ul = (UltraLabel)dpv.utpcPumpRuling.Controls[i];
                    if (ul.Tag != null)
                    {
                        SetLabelVisible(ul, false);
                    }
				}
			}
		}

		/// <summary>
		/// Устанавливает видимость лабелов сообщения прогресса в зависимости от этапа закачки
		/// </summary>
		/// <param name="state">Этап</param>
		/// <param name="visible">Видимость</param>
		private void SetMessageLabelsVisibleForPumpState(PumpProcessStates state, bool visible)
		{
			switch (state)
			{
                case PumpProcessStates.PreviewData:
                    SetLabelVisible(dpv.ulPreviewDataMessage, visible);
                    break;

				case PumpProcessStates.PumpData:
					SetLabelVisible(dpv.ulPumpDataMessage, visible);
					break;

				case PumpProcessStates.ProcessData:
					SetLabelVisible(dpv.ulProcessDataMessage, visible);
					break;

				case PumpProcessStates.AssociateData:
					SetLabelVisible(dpv.ulAssociateDataMessage, visible);
					break;

				case PumpProcessStates.ProcessCube:
					SetLabelVisible(dpv.ulProcessCubeMessage, visible);
					break;

				case PumpProcessStates.CheckData:
					SetLabelVisible(dpv.ulCheckDataMessage, visible);
					break;

				case PumpProcessStates.DeleteData:
					break;
			}
		}

		/// <summary>
		/// Устанавливает видимость лабелов времени начала этапа в зависимости от этапа закачки
		/// </summary>
		/// <param name="state">Этап</param>
		/// <param name="visible">Видимость</param>
		private void SetStartTimeLabelsVisibleForPumpState(PumpProcessStates state, bool visible)
		{
			switch (state)
			{
                case PumpProcessStates.PreviewData:
                    SetLabelVisible(dpv.ulPreviewDataStartTime, visible);
                    break;

				case PumpProcessStates.PumpData:
					SetLabelVisible(dpv.ulPumpDataStartTime, visible);
					break;

				case PumpProcessStates.ProcessData:
					SetLabelVisible(dpv.ulProcessDataStartTime, visible);
					break;

				case PumpProcessStates.AssociateData:
					SetLabelVisible(dpv.ulAssociateDataStartTime, visible);
					break;

				case PumpProcessStates.ProcessCube:
					SetLabelVisible(dpv.ulProcessCubeStartTime, visible);
					break;

				case PumpProcessStates.CheckData:
					SetLabelVisible(dpv.ulCheckDataStartTime, visible);
					break;

				case PumpProcessStates.DeleteData:
					break;
			}
		}

		/// <summary>
		/// Устанавливает видимость лабелов времени окончания этапа в зависимости от этапа закачки
		/// </summary>
		/// <param name="state">Этап</param>
		private void SetEndTimeLabelsVisibleForPumpState(PumpProcessStates state/*, bool visible*/)
		{
			switch (state)
			{
                case PumpProcessStates.PreviewData:
                    SetLabelVisible(dpv.ulPreviewDataEndTime, true);
                    break;

				case PumpProcessStates.PumpData:
					SetLabelVisible(dpv.ulPumpDataEndTime, true);
					break;

				case PumpProcessStates.ProcessData:
					SetLabelVisible(dpv.ulProcessDataEndTime, true);
					break;

				case PumpProcessStates.AssociateData:
					SetLabelVisible(dpv.ulAssociateDataEndTime, true);
					break;

				case PumpProcessStates.ProcessCube:
					SetLabelVisible(dpv.ulProcessCubeEndTime, true);
					break;

				case PumpProcessStates.CheckData:
					SetLabelVisible(dpv.ulCheckDataEndTime, true);
					break;

				case PumpProcessStates.DeleteData:
					break;
			}
		}

		/// <summary>
		/// Устанавливет значение лабела времени начала этапа
		/// </summary>
		/// <param name="state">Этап закачки</param>
		private void SetStartTimeLabelsForPumpState(PumpProcessStates state)
		{
			if (dataPumpProgress == null) return;

            IStagesQueueElement sqe = pumpRegistryElement.StagesQueue[state];

            if (sqe == null || sqe.StartTime == DateTime.MinValue) return;

            SetStartTimeLabelsVisibleForPumpState(state, true);

			switch (state)
			{
                case PumpProcessStates.PreviewData:
                    SetLabelText(dpv.ulPreviewDataStartTime, sqe.StartTime.ToString());
                    break;

				case PumpProcessStates.PumpData:
                    SetLabelText(dpv.ulPumpDataStartTime, sqe.StartTime.ToString());
					break;

				case PumpProcessStates.ProcessData:
                    SetLabelText(dpv.ulProcessDataStartTime, sqe.StartTime.ToString());
					break;

				case PumpProcessStates.AssociateData:
                    SetLabelText(dpv.ulAssociateDataStartTime, sqe.StartTime.ToString());
					break;

				case PumpProcessStates.ProcessCube:
                    SetLabelText(dpv.ulProcessCubeStartTime, sqe.StartTime.ToString());
					break;

				case PumpProcessStates.CheckData:
                    SetLabelText(dpv.ulCheckDataStartTime, sqe.StartTime.ToString());
					break;

				case PumpProcessStates.DeleteData:
					break;
			}
		}

		/// <summary>
		/// Устанавливет значение лабела времени окончания этапа
		/// </summary>
		/// <param name="state">Этап закачки</param>
		private void SetEndTimeLabelsForPumpState(PumpProcessStates state)
		{
			if (dataPumpProgress == null) return;

            IStagesQueueElement sqe = pumpRegistryElement.StagesQueue[state];

            if (sqe == null || sqe.EndTime == DateTime.MinValue) return;

            SetEndTimeLabelsVisibleForPumpState(state/*, true*/);

			switch (state)
			{
                case PumpProcessStates.PreviewData:
                    SetLabelText(dpv.ulPreviewDataEndTime, sqe.EndTime.ToString());
                    break;

				case PumpProcessStates.PumpData:
                    SetLabelText(dpv.ulPumpDataEndTime, sqe.EndTime.ToString());
					break;

				case PumpProcessStates.ProcessData:
                    SetLabelText(dpv.ulProcessDataEndTime, sqe.EndTime.ToString());
					break;

				case PumpProcessStates.AssociateData:
                    SetLabelText(dpv.ulAssociateDataEndTime, sqe.EndTime.ToString());
					break;

				case PumpProcessStates.ProcessCube:
                    SetLabelText(dpv.ulProcessCubeEndTime, sqe.EndTime.ToString());
					break;

				case PumpProcessStates.CheckData:
                    SetLabelText(dpv.ulCheckDataEndTime, sqe.EndTime.ToString());
					break;

				case PumpProcessStates.DeleteData:
					break;
			}
		}

        /// <summary>
		/// Устанавливает видимость лабелов времени окончания и начала этапа в зависимости от этапа закачки
		/// </summary>
		/// <param name="state">Этап</param>
        private void SetAllTimeLabelsForPumpState(PumpProcessStates state/*, bool visible*/)
        {
            SetStartTimeLabelsForPumpState(state);
            SetEndTimeLabelsForPumpState(state);
        }

		#endregion Функции управления лабелами


		#region Функции управления картинками

        /*
		/// <summary>
		/// Очищает картинки
		/// </summary>
		private void ClearInitialImages()
		{
            dpv.upicPreviewData.Image = null;
			dpv.upicPumpData.Image = null;
			dpv.upicProcessData.Image = null;
			dpv.upicAssociateData.Image = null;
			dpv.upicProcessCube.Image = null;
			dpv.upicCheckData.Image = null;
		}*/

        /// <summary>
        /// Очищает картинки
        /// </summary>
        private void ClearCurrentImages()
        {
            dpv.upicPreviewDataResult.Image = null;
            dpv.upicPumpDataResult.Image = null;
            dpv.upicProcessDataResult.Image = null;
            dpv.upicAssociateDataResult.Image = null;
            dpv.upicProcessCubeResult.Image = null;
            dpv.upicCheckDataResult.Image = null;
        }

		/*
        /// <summary>
		/// Проставляет иконки результата выполнения отдельного этапа
		/// </summary>
		/// <param name="sqe">Этап</param>
		private void SetCurrentImagesForQueueElement(IStagesQueueElement sqe)
		{
            if (sqe.StageCurrentState != StageState.InProgress || !sqe.IsExecuted) return;

            if (sqe.StageCurrentState == StageState.InProgress)
			{
				SetImagesForPumpCurrentState(sqe.State, Properties.Resources.Start);
			}
            else if (sqe.StageCurrentState == StageState.Skipped)
			{
				SetImagesForPumpCurrentState(sqe.State, Properties.Resources.Skipped);
			}
            else if (sqe.StageCurrentState == StageState.FinishedWithErrors)
            {
                SetImagesForPumpCurrentState(sqe.State, Properties.Resources.Cross2);
            }
            else if (sqe.StageCurrentState == StageState.SuccefullFinished)
            {
                SetImagesForPumpCurrentState(sqe.State, Properties.Resources.Check3);
            }
            else if (sqe.StageCurrentState == StageState.OutOfQueue)
            {
                SetImagesForPumpCurrentState(sqe.State, null);
            }
            else if (sqe.StageCurrentState == StageState.Blocked)
            {
                SetImagesForPumpCurrentState(sqe.State, Properties.Resources.Lock);
            }
		}*/

		/*
        /// <summary>
		/// Проставляет иконки резудьтатов выполнения этапов в очереди
		/// </summary>
		private void SetCurrentImagesForStagesQueue()
		{
			if (dataPumpProgress == null) return;

            SetCurrentImagesForQueueElement(pumpRegistryElement.StagesQueue[PumpProcessStates.PreviewData]);
			SetCurrentImagesForQueueElement(pumpRegistryElement.StagesQueue[PumpProcessStates.PumpData]);
			SetCurrentImagesForQueueElement(pumpRegistryElement.StagesQueue[PumpProcessStates.ProcessData]);
			SetCurrentImagesForQueueElement(pumpRegistryElement.StagesQueue[PumpProcessStates.AssociateData]);
			SetCurrentImagesForQueueElement(pumpRegistryElement.StagesQueue[PumpProcessStates.ProcessCube]);
			SetCurrentImagesForQueueElement(pumpRegistryElement.StagesQueue[PumpProcessStates.CheckData]);
		}*/

        /// <summary>
        /// Проставляет иконки начального состояния отдельного этапа
        /// </summary>
        /// <param name="sqe">Этап</param>
        private void SetInitialImagesForQueueElement(IStagesQueueElement sqe)
        {
            if (sqe.StageInitialState == StageState.InQueue)
            {
                SetImagesForPumpInitialState(sqe.State, Properties.Resources.InQueue);
            }
            else if (sqe.StageInitialState == StageState.Skipped)
            {
                SetImagesForPumpInitialState(sqe.State, Properties.Resources.Skipped);
            }
            else if (sqe.StageInitialState == StageState.Blocked)
            {
                SetImagesForPumpInitialState(sqe.State, Properties.Resources.Lock);
            }
        }

        /// <summary>
        /// Проставляет иконки начального состояния этапов в очереди
        /// </summary>
        private void SetInitialImagesForStagesQueue()
        {
            if (dataPumpProgress == null) return;

            SetInitialImagesForQueueElement(pumpRegistryElement.StagesQueue[PumpProcessStates.PreviewData]);
            SetInitialImagesForQueueElement(pumpRegistryElement.StagesQueue[PumpProcessStates.PumpData]);
            SetInitialImagesForQueueElement(pumpRegistryElement.StagesQueue[PumpProcessStates.ProcessData]);
            SetInitialImagesForQueueElement(pumpRegistryElement.StagesQueue[PumpProcessStates.AssociateData]);
            SetInitialImagesForQueueElement(pumpRegistryElement.StagesQueue[PumpProcessStates.ProcessCube]);
            SetInitialImagesForQueueElement(pumpRegistryElement.StagesQueue[PumpProcessStates.CheckData]);
        }

		/// <summary>
		/// Устанавливает иконки результата выполнения этапа в зависимости от состояния закачки
		/// </summary>
		/// <param name="state">Состояние закачки</param>
		/// <param name="bmp">Картинка</param>
		private void SetImagesForPumpCurrentState(PumpProcessStates state, Bitmap bmp)
		{
            //if (!dataPumpProgress.PumpIsAlive && !dataPumpProgress.PumpInProgress) return;

            //if (!pumpRegistryElement.StagesQueue[state].IsExecuted ||
            //    pumpRegistryElement.StagesQueue[state].StageCurrentState != StageState.InProgress) return;

			switch (state)
			{
                case PumpProcessStates.PreviewData: dpv.upicPreviewDataResult.Image = bmp;
                    break;

				case PumpProcessStates.PumpData: dpv.upicPumpDataResult.Image = bmp;
					break;

				case PumpProcessStates.ProcessData: dpv.upicProcessDataResult.Image = bmp;
					break;

				case PumpProcessStates.AssociateData: dpv.upicAssociateDataResult.Image = bmp;
					break;

				case PumpProcessStates.ProcessCube: dpv.upicProcessCubeResult.Image = bmp;
					break;

				case PumpProcessStates.CheckData: dpv.upicCheckDataResult.Image = bmp;
					break;

				case PumpProcessStates.DeleteData:
					break;
			}
		}

        /// <summary>
        /// Устанавливает иконки начального состояния этапа в зависимости от состояния закачки
        /// </summary>
        /// <param name="state">Состояние закачки</param>
        /// <param name="bmp">Картинка</param>
        private void SetImagesForPumpInitialState(PumpProcessStates state, Bitmap bmp)
        {
            switch (state)
            {
                case PumpProcessStates.PreviewData: dpv.upicPreviewData.Image = bmp;
                    break;

                case PumpProcessStates.PumpData: dpv.upicPumpData.Image = bmp;
                    break;

                case PumpProcessStates.ProcessData: dpv.upicProcessData.Image = bmp;
                    break;

                case PumpProcessStates.AssociateData: dpv.upicAssociateData.Image = bmp;
                    break;

                case PumpProcessStates.ProcessCube: dpv.upicProcessCube.Image = bmp;
                    break;

                case PumpProcessStates.CheckData: dpv.upicCheckData.Image = bmp;
                    break;

                case PumpProcessStates.DeleteData:
                    break;
            }
        }

		#endregion Функции управления картинками


        #region Функции управления формой

        /// <summary>
        /// Устанавливает выбранную закладку табконтрола
        /// </summary>
        /// <param name="utc">Табконтрол</param>
        /// <param name="tab">Закладка</param>
        private void SetSelectedTab(UltraTabControl utc, UltraTab tab)
        {
            utc.Invoke(setSelectedTab, new object[] { utc, tab });
        }

        /// <summary>
        /// Устанавливает выбранную закладку табконтрола
        /// </summary>
        /// <param name="utc">Табконтрол</param>
        /// <param name="utpc">Закладка</param>
        private void SetSelectedTab(UltraTabControl utc, UltraTabPageControl utpc)
        {
            utc.Invoke(setSelectedTab, new object[] { utc, utpc.Tab });
        }

        #endregion Функции управления формой


        #region Общие функции управления контролами

        /// <summary>
		/// Устанавливает состояние контролов согласно очереди закачки
		/// </summary>
		private void SetControlsFromStagesQueue()
		{
            if (dataPumpProgress == null) return;

            dataPumpProgress.Refresh();

            HideLabels();
            HideProgressBars();
            SetInitialImagesForStagesQueue();
            ClearCurrentImages();

			for (int i = 1; i <= 6; i++)
			{
				PumpProcessStates st = (PumpProcessStates)i;
                IStagesQueueElement sqe = pumpRegistryElement.StagesQueue[st];

                StageState stageInitialState = sqe.StageInitialState;
                StageState stageCurrentState = sqe.StageCurrentState;

                if (dataPumpProgress.PumpIsAlive && dataPumpProgress.PumpInProgress)
                {
                    if (sqe.IsExecuted)
                    {
                        SetStageButtonsGroupEnabled(st, false, false, false, false);
                    }
                }

                SetAllTimeLabelsForPumpState(st/*, true*/);

                // Устанавливаем контролы по начальному состоянию этапа
                switch (stageInitialState)
				{
					case StageState.InQueue:
                        if (dataPumpProgress.PumpIsAlive && dataPumpProgress.PumpInProgress)
                        {
                            SetStageButtonsGroupEnabled(st, false, false, false, true);
                        }
                        else
                        {
                            SetStageButtonsGroupEnabled(st, true, false, false, true);
                        }
						break;

                    case StageState.OutOfQueue:
                        if (dataPumpProgress.PumpIsAlive && dataPumpProgress.PumpInProgress)
                        {
                            SetStageButtonsGroupEnabled(st, false, false, false, false);
                        }
                        else
                        {
                            SetStageButtonsGroupEnabled(st, true, false, false, false);
                        }
                        break;

					case StageState.Skipped:
                        SetStageButtonsGroupEnabled(st, false, false, false, true);
						break;

                    case StageState.Blocked:
                        SetStageButtonsGroupEnabled(st, false, false, false, false);
                        break;
				}

                // Устанавливаем контролы по текущему состоянию этапа
                switch (stageCurrentState)
                {
                    case StageState.FinishedWithErrors:
                        SetImagesForPumpCurrentState(st, Properties.Resources.Cross2);
                        break;

                    case StageState.InProgress:
                        if (dataPumpProgress.PumpInProgress && dataPumpProgress.PumpInProgress)
                        {
                            SetStageButtonsGroupEnabled(st, false, true, true, true);
                            SetImagesForPumpCurrentState(st, Properties.Resources.Start);
                            SetMessageLabelsVisibleForPumpState(st, true);

                            SetProgressVisibleForPumpState(st, true);
                            SetProgressForPumpState(dataPumpProgress.State, dataPumpProgress.ProgressMaxPos,
                                dataPumpProgress.ProgressCurrentPos, dataPumpProgress.ProgressMessage,
                                dataPumpProgress.ProgressText);
                        }
                        break;

                    case StageState.Skipped:
                        if (stageInitialState != StageState.Blocked && stageInitialState != StageState.Skipped)
                        {
                            SetImagesForPumpCurrentState(st, Properties.Resources.Skipped);
                        }
                        break;

                    case StageState.SuccefullFinished:
                        SetImagesForPumpCurrentState(st, Properties.Resources.Check3);
                        break;
                }
			}
		}

        /*
        /// <summary>
        /// Устанавливает курсор у указанного контрола
        /// </summary>
        /// <param name="cursor">Курсор</param>
        private void SetCursorForControl(Cursor cursor)
        {
            dpv.Invoke(setCursorDelegate, new object[] { cursor });
        }*/

		#endregion Общие функции управления контролами


		#endregion Функции управления состоянием контролов


        #region Функции управления табконтролами закачки

        #region Функции для работы с историей закачки

        /// <summary>
        /// Формирует коллекцию с данными о соответствии источников данных истории закачки
        /// </summary>
        /// <returns>Коллекция соответствия источников данных истории закачки</returns>
        private void FillHistory2DataSourcesMapping()
        {
            if (history2DataSourcesMapping == null)
            {
                history2DataSourcesMapping = new Dictionary<int, List<int>>(1000);
            }
            else
            {
                history2DataSourcesMapping.Clear();
            }

            if (dataSources2HistoryMapping == null)
            {
                dataSources2HistoryMapping = new Dictionary<int, List<int>>(1000);
            }
            else
            {
                dataSources2HistoryMapping.Clear();
            }

            IDatabase db2 = Workplace.ActiveScheme.SchemeDWH.DB;

            try
            {
                DataTable dt = (DataTable)db2.ExecQuery("select * from DATASOURCES2PUMPHISTORY", QueryResultTypes.DataTable);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int pumpID = Convert.ToInt32(dt.Rows[i]["REFPUMPHISTORY"]);
                    int sourceID = Convert.ToInt32(dt.Rows[i]["REFDATASOURCES"]);

                    // Добавляем соответствие истории источникам
                    if (!history2DataSourcesMapping.ContainsKey(pumpID))
                    {
                        history2DataSourcesMapping.Add(pumpID, new List<int>(100));
                    }
                    history2DataSourcesMapping[pumpID].Add(sourceID);

                    // Добавляем соответствие источников истории
                    if (!dataSources2HistoryMapping.ContainsKey(sourceID))
                    {
                        dataSources2HistoryMapping.Add(sourceID, new List<int>(100));
                    }
                    dataSources2HistoryMapping[sourceID].Add(pumpID);
                }
            }
            finally
            {
                db2.Dispose();
            }
        }

        /// <summary>
        /// Добавляет поля истории закачки и источников в таблицы датасета
        /// </summary>
        /// <param name="ds">Датасет</param>
        private void AddColumnsToHistoryDS(DataSet ds)
        {
            Type intFieldsType = null;
            string factoryName = Workplace.ActiveScheme.SchemeDWH.FactoryName;
            switch (factoryName)
            {
                case ProviderFactoryConstants.OracleClient:
				case ProviderFactoryConstants.MSOracleDataAccess:
					intFieldsType = typeof(decimal);
                    break;
                case ProviderFactoryConstants.OracleDataAccess:
                    intFieldsType = typeof(Int64);
                    break;
                case ProviderFactoryConstants.SqlClient:
                    intFieldsType = typeof(Int32);
                    break;
                default:
                    throw new Exception(string.Format("Данный тип провайдера не поддерживается ({0}).", factoryName));
            }

            ds.Tables["PUMPHISTORY"].Columns.Add("SOURCEID", intFieldsType);
            ds.Tables["PUMPHISTORY"].Columns.Add("GOTOLOG", typeof(string));
            ds.Tables["PUMPHISTORY"].Columns.Add("DELETEDATA", typeof(string));
            ds.Tables["PUMPHISTORY"].Columns.Add("STARTEDBY_STR", typeof(string));
                        
            /*ds.Tables["PUMPHISTORY"].Columns.Add("PUMPID", typeof(int));
            ds.Tables["PUMPHISTORY"].Columns.Add("SYSTEMVERSION", typeof(string));
            ds.Tables["PUMPHISTORY"].Columns.Add("PROGRAMVERSION", typeof(string));
            ds.Tables["PUMPHISTORY"].Columns.Add("PUMPDATE", typeof(DateTime));
            ds.Tables["PUMPHISTORY"].Columns.Add("STARTEDBY", typeof(string));
            ds.Tables["PUMPHISTORY"].Columns.Add("COMMENTS", typeof(string));*/

            ds.Tables["DATASOURCES"].Columns.Add("PUMPID", intFieldsType);
            ds.Tables["DATASOURCES"].Columns.Add("DELETEDATA", typeof(string));
            ds.Tables["DATASOURCES"].Columns.Add("KINDSOFPARAMS_STR", typeof(string));
            ds.Tables["DATASOURCES"].Columns.Add("MONTH_STR", typeof(string));
            /*ds.Tables["DATASOURCES"].Columns.Add("SOURCEID", typeof(int));
            ds.Tables["DATASOURCES"].Columns.Add("SUPPLIERCODE", typeof(string));
            ds.Tables["DATASOURCES"].Columns.Add("DATACODE", typeof(string));
            ds.Tables["DATASOURCES"].Columns.Add("DATANAME", typeof(string));
            ds.Tables["DATASOURCES"].Columns.Add("KINDSOFPARAMS", typeof(string));
            ds.Tables["DATASOURCES"].Columns.Add("BUDGETNAME", typeof(string));
            ds.Tables["DATASOURCES"].Columns.Add("YEAR", typeof(int));
            ds.Tables["DATASOURCES"].Columns.Add("MONTH", typeof(string));
            ds.Tables["DATASOURCES"].Columns.Add("VARIANT", typeof(string));
            ds.Tables["DATASOURCES"].Columns.Add("QUARTER", typeof(int));*/
        }

        /// <summary>
        /// Копирует строку в указанную таблицу
        /// </summary>
        /// <param name="sourceRow">Исходная строка</param>
        /// <param name="destRow">Строка, куда копировать</param>
        private static void CopyRowToRow(DataRow sourceRow, DataRow destRow)
        {
            if (sourceRow == null || destRow == null) return;

            for (int i = 0; i < destRow.Table.Columns.Count; i++)
            {
                if (sourceRow.Table.Columns.Contains(destRow.Table.Columns[i].ColumnName))
                {
                    destRow[i] = sourceRow[destRow.Table.Columns[i].ColumnName];
                }
            }
        }

        /// <summary>
        /// Заполняет поле PUMPID таблицы источников данных
        /// </summary>
        private void FillPumpIDForDataSources(DataTable dt)
        {
            DataRow[] rows = dt.Select();

            for (int i = 0; i < rows.GetLength(0); i++)
            {
                int sourceID = Convert.ToInt32(rows[i]["ID"]);
                if (dataSources2HistoryMapping.ContainsKey(sourceID))
                {
                    // Т.к. один источник может быть закачан несколько раз с разными записями истории закачки,
                    // то добавляем столько же источников
                    List<int> history = dataSources2HistoryMapping[sourceID];
                    for (int j = 0; j < history.Count; j++)
                    {
                        if (j == 0)
                        {
                            rows[i]["PUMPID"] = history[j];
                        }
                        else
                        {
                            DataRow row = dt.NewRow();
                            CopyRowToRow(rows[i], row);
                            row["PUMPID"] = history[j];
                            dt.Rows.Add(row);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Очищает датасет с данными истории закачки
        /// </summary>
        /// <param name="ds">Датасет</param>
        private static void ClearHistoryDataSet(ref DataSet ds)
        {
            if (ds != null)
            {
                ds.Clear();
            }
            ds = new DataSet();
        }

        // при закачке слоев возникает проблема - в таблицу DataSources попадают лишние записи 
        // (потому что все закачки слоев по одному типу источника)
        // поэтому удаляем из таблицы DataSources лишние записи
        private void DeleteIncorrectDataSources()
        {
            string filter = string.Empty;
            foreach (DataRow row in dsPumpHistory.Tables["PUMPHISTORY"].Rows)
                filter += string.Format("{0},", row["Id"]);
            filter = filter.Remove(filter.Length - 1);
            filter = string.Format("PUMPID not in ({0})", filter);
            DataRow[] dsRows = dsPumpHistory.Tables["DATASOURCES"].Select(filter);
            foreach (DataRow row in dsRows)
                row.Delete();
            dsPumpHistory.AcceptChanges();
        }

        /// <summary>
        /// Инициализирует датасет истории закачки
        /// </summary>
        private void InitPumpHistoryDS()
        {
            dpv.ugPumpHistory.InitializeRow -= new InitializeRowEventHandler(ugPumpHistory_InitializeRow);

            // Очищаем датасет с данными истории закачки
            ClearHistoryDataSet(ref dsPumpHistory);

            dsPumpHistory.Tables.Add(pumpRegistryElement.PumpHistory);
            dsPumpHistory.Tables[0].TableName = "PUMPHISTORY";

            dsPumpHistory.Tables.Add(pumpRegistryElement.DataSources);
            dsPumpHistory.Tables[1].TableName = "DATASOURCES";

            // Добавляем поля истории закачки и источников в таблицы датасета
            AddColumnsToHistoryDS(dsPumpHistory);

            // Заполняем поле PUMPID таблицы источников данных
            FillPumpIDForDataSources(dsPumpHistory.Tables["DATASOURCES"]);

            DeleteIncorrectDataSources();

            dsPumpHistory.Relations.Add(new DataRelation(
                "PH2DS",
                dsPumpHistory.Tables["PUMPHISTORY"].Columns["ID"],
                dsPumpHistory.Tables["DATASOURCES"].Columns["PUMPID"]));

            dpv.ugPumpHistory.InitializeRow += new InitializeRowEventHandler(ugPumpHistory_InitializeRow);
        }

        /// <summary>
        /// Заполняет поле SOURCEID таблицы истории закачки
        /// </summary>
        private void FillSourceIDForPumpHistory(DataTable dt)
        {
            DataRow[] rows = dt.Select();

            for (int i = 0; i < rows.GetLength(0); i++)
            {
                int pumpID = Convert.ToInt32(rows[i]["ID"]);
                if (history2DataSourcesMapping.ContainsKey(pumpID))
                {
                    // Т.к. по одной записи истории закачки может быть несколько источников, 
                    // то добавляем столько же записей
                    List<int> sources = history2DataSourcesMapping[pumpID];
                    for (int j = 0; j < sources.Count; j++)
                    {
                        if (j == 0)
                        {
                            rows[i]["SOURCEID"] = sources[j];
                        }
                        else
                        {
                            DataRow row = dt.NewRow();
                            CopyRowToRow(rows[i], row);
                            row["SOURCEID"] = sources[j];
                            dt.Rows.Add(row);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Инициализирует датасет истории закачки
        /// </summary>
        private void InitDataSourcesDS()
        {
            dpv.ugDataSources.InitializeRow -= new InitializeRowEventHandler(ugDataSources_InitializeRow);

            // Очищаем датасет с данными истории закачки
            ClearHistoryDataSet(ref dsDataSources);

            dsDataSources.Tables.Add(pumpRegistryElement.DataSources);
            dsDataSources.Tables[0].TableName = "DATASOURCES";

            dsDataSources.Tables.Add(pumpRegistryElement.PumpHistory);
            dsDataSources.Tables[1].TableName = "PUMPHISTORY";

            // Добавляем поля истории закачки и источников в таблицы датасета
            AddColumnsToHistoryDS(dsDataSources);

            // Заполняем поле SOURCEID таблицы истории закачки
            FillSourceIDForPumpHistory(dsDataSources.Tables["PUMPHISTORY"]);

            dsDataSources.Relations.Add(new DataRelation(
                "DS2PH",
                dsDataSources.Tables["DATASOURCES"].Columns["ID"],
                dsDataSources.Tables["PUMPHISTORY"].Columns["SOURCEID"], false));

            dpv.ugDataSources.InitializeRow += new InitializeRowEventHandler(ugDataSources_InitializeRow);
        }

        /*
        /// <summary>
        /// Преобразует тип источника данных в строку
        /// </summary>
        private string KindsOfParamsToString(ParamKindTypes paramType)
        {
            switch (paramType)
            {
                case ParamKindTypes.Budget: return "Бюджет";

                case ParamKindTypes.Year: return "Год";

                case ParamKindTypes.YearMonth: return "Год, месяц";

                case ParamKindTypes.YearMonthVariant: return "Год, месяц, вариант";

                case ParamKindTypes.YearQuarter: return "Год, вариант";

                case ParamKindTypes.YearVariant: return "Год, квартал";
            }

            return string.Empty;
        }*/

        private static bool CheckColumn(UltraGrid ug, int band, string columnName, ref UltraGridColumn clmn)
        {
            try
            {
                clmn = ug.DisplayLayout.Bands[band].Columns[columnName];
            }
            catch
            {
                clmn = null;
            }

            return (clmn != null);
        }

        private static void CustomizeButtonCell(UltraGridColumn column, /*, Bitmap icon,*/ int pos, int imageIndex)
        {
            UltraGridHelper.SetLikelyButtonColumnsStyle(column, imageIndex);
            column.Header.VisiblePosition = pos;
        }

        /// <summary>
        /// Настраивает вид грида истории закачки
        /// </summary>
        /// <param name="ug"></param>
        /// <param name="band"></param>
        private static void CustomizePumpHistory(UltraGrid ug, int band)
        {
            // Настраиваеv ячейку грида как кнопку
            CustomizeButtonCell(ug.DisplayLayout.Bands[band].Columns["GOTOLOG"], /*Properties.Resources.Arrow,*/ 0, 0);
            CustomizeButtonCell(ug.DisplayLayout.Bands[band].Columns["DELETEDATA"], /*Properties.Resources.Cross2,*/ 1, 1);
            ug.DisplayLayout.Bands[band].Columns["DELETEDATA"].CellActivation = Activation.Disabled;

            UltraGridColumn column = null;

            if (CheckColumn(ug, band, "ID", ref column))
            {
                column.Header.Caption = "ID закачки";
                column.Hidden = false;
            }

            if (CheckColumn(ug, band, "PROGRAMIDENTIFIER", ref column))
            {
                column.Hidden = true;
            }

            if (CheckColumn(ug, band, "PROGRAMCONFIG", ref column))
            {
                column.Hidden = true;
            }

            if (CheckColumn(ug, band, "SYSTEMVERSION", ref column))
            {
                column.Header.Caption = "Версия системы";
                column.Hidden = false;
            }

            if (CheckColumn(ug, band, "PROGRAMVERSION", ref column))
            {
                column.Header.Caption = "Версия закачки";
                column.Hidden = false;
            }

            if (CheckColumn(ug, band, "PUMPDATE", ref column))
            {
                column.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Edit;
                column.Header.Caption = "Дата закачки";
                column.Hidden = false;
                column.Width = 150;
                column.MaskInput = "{date} {time}";
            }

            if (CheckColumn(ug, band, "STARTEDBY", ref column))
            {
                column.Hidden = true;
            }

            if (CheckColumn(ug, band, "STARTEDBY_STR", ref column))
            {
                column.Header.Caption = "Кем запущена";
                column.Header.VisiblePosition = 8;
                column.Hidden = true;
            }

            if (CheckColumn(ug, band, "COMMENTS", ref column))
            {
                column.Header.Caption = "Примечания";
                column.Hidden = false;
                column.Width = 600;
            }

            if (CheckColumn(ug, band, "REFPUMPREGISTRY", ref column))
            {
                column.Hidden = true;
            }

            if (CheckColumn(ug, band, "SOURCEID", ref column))
            {
                column.Hidden = true;
            }

            if (CheckColumn(ug, band, "BATCHID", ref column))
            {
                column.Header.Caption = "ID пакета";
                column.Hidden = false;
                column.Width = 300;
            }

            if (CheckColumn(ug, band, "USERNAME", ref column))
            {
                column.Header.Caption = "Пользователь";
                column.Hidden = false;
                column.Width = 200;
            }

            if (CheckColumn(ug, band, "USERHOST", ref column))
            {
                column.Header.Caption = "Машина пользователя";
                column.Hidden = false;
                column.Width = 200;
            }

            if (CheckColumn(ug, band, "SESSIONID", ref column))
            {
                column.Header.Caption = "ID сессии";
                column.Hidden = false;
                column.Width = 200;
            }

        }

        /// <summary>
        /// Устанавливает разрешения на кнопки удаления данных истории закачки
        /// </summary>
        private void SetEnableDeleteDataForGrids(bool enable)
        {
            if (enable)
            {
                if (dpv.ugPumpHistory.DisplayLayout.Bands.Count > 0 && 
                    dpv.ugPumpHistory.DisplayLayout.Bands[0].Columns.Count > 0)
                {
                    dpv.ugPumpHistory.DisplayLayout.Bands[0].Columns["DELETEDATA"].CellActivation = Activation.Disabled;
                    dpv.ugDataSources.DisplayLayout.Bands[0].Columns["DELETEDATA"].CellActivation = Activation.Disabled;
                }

                if (dpv.ugPumpHistory.DisplayLayout.Bands.Count > 1 && 
                    dpv.ugPumpHistory.DisplayLayout.Bands[1].Columns.Count > 0)
                {
                    dpv.ugPumpHistory.DisplayLayout.Bands[1].Columns["DELETEDATA"].CellActivation = Activation.Disabled;
                    dpv.ugDataSources.DisplayLayout.Bands[1].Columns["DELETEDATA"].CellActivation = Activation.Disabled;
                }
            }
            else
            {
                if (dpv.ugPumpHistory.DisplayLayout.Bands.Count > 0 && 
                    dpv.ugPumpHistory.DisplayLayout.Bands[0].Columns.Count > 0)
                {
                    dpv.ugPumpHistory.DisplayLayout.Bands[0].Columns["DELETEDATA"].CellActivation = Activation.NoEdit;
                    dpv.ugDataSources.DisplayLayout.Bands[0].Columns["DELETEDATA"].CellActivation = Activation.NoEdit;
                }

                if (dpv.ugPumpHistory.DisplayLayout.Bands.Count > 1 && 
                    dpv.ugPumpHistory.DisplayLayout.Bands[1].Columns.Count > 0)
                {
                    dpv.ugPumpHistory.DisplayLayout.Bands[1].Columns["DELETEDATA"].CellActivation = Activation.NoEdit;
                    dpv.ugDataSources.DisplayLayout.Bands[1].Columns["DELETEDATA"].CellActivation = Activation.NoEdit;
                }
            }
        }

        /// <summary>
        /// Настраивает грид источников (подчиненный истории закачки)
        /// </summary>
        private static void CustomizeDataSources(UltraGrid ug, int band)
        {
            // Настраиваеv ячейку грида как кнопку
            CustomizeButtonCell(ug.DisplayLayout.Bands[band].Columns["DELETEDATA"], /*Properties.Resources.Cross2,*/ 0, 1);
            ug.DisplayLayout.Bands[band].Columns["DELETEDATA"].CellActivation = Activation.Disabled;

            UltraGridColumn column = null;

            if (CheckColumn(ug, band, "ID", ref column))
            {
                column.Header.Caption = "ID источника";
                column.Hidden = false;
            }

            if (CheckColumn(ug, band, "PUMPID", ref column))
            {
                column.Hidden = true;
            }

            if (CheckColumn(ug, band, "SUPPLIERCODE", ref column))
            {
                column.Header.Caption = "Код поставщика";
                column.Hidden = false;
            }

            if (CheckColumn(ug, band, "DATACODE", ref column))
            {
                column.Header.Caption = "Код данных";
                column.Hidden = false;
            }

            if (CheckColumn(ug, band, "DATANAME", ref column))
            {
                column.Hidden = true;
            }

            if (CheckColumn(ug, band, "KINDSOFPARAMS", ref column))
            {
                column.Hidden = true;
            }

            if (CheckColumn(ug, band, "KINDSOFPARAMS_STR", ref column))
            {
                column.Header.Caption = "Вид источника";
                column.Hidden = false;
                column.Header.VisiblePosition = 6;
                column.Width = 150;
            }

            if (CheckColumn(ug, band, "NAME", ref column))
            {
                column.Header.Caption = "Бюджет";
                column.Hidden = false;
            }

            if (CheckColumn(ug, band, "YEAR", ref column))
            {
                column.Header.Caption = "Год";
                column.Hidden = false;
            }

            if (CheckColumn(ug, band, "MONTH", ref column))
            {
                column.Hidden = true;
            }

            if (CheckColumn(ug, band, "MONTH_STR", ref column))
            {
                column.Header.Caption = "Месяц";
                column.Hidden = false;
                column.Header.VisiblePosition = 10;
            }

            if (CheckColumn(ug, band, "VARIANT", ref column))
            {
                column.Header.Caption = "Вариант";
                column.Hidden = false;
                column.Width = 150;
            }

            if (CheckColumn(ug, band, "QUARTER", ref column))
            {
                column.Header.Caption = "Квартал";
                column.Hidden = false;
            }

            if (CheckColumn(ug, band, "TERRITORY", ref column))
            {
                column.Header.Caption = "Территория";
                column.Hidden = false;
                column.Width = 300;
            }

            if (CheckColumn(ug, band, "DATASOURCENAME", ref column))
            {
                column.Hidden = true;
            }

            if (CheckColumn(ug, band, "LOCKED", ref column))
            {
                column.Header.Caption = string.Empty;
                column.Header.VisiblePosition = 1;
                column.Width = 16;
                column.AutoSizeMode = ColumnAutoSizeMode.None;
            }
        }

        /// <summary>
        /// Преобразвует числовые значения полей таблиц источников закачки в символьные значения
        /// </summary>
        /// <param name="ds">Датасет истории</param>
        private void ConvertDataSourcesTablesColumns(DataSet ds)
        {
            DataTable dt = ds.Tables["DATASOURCES"];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];

                if (!row.IsNull("KINDSOFPARAMS"))
                {
                    int value = Convert.ToInt32(row["KINDSOFPARAMS"]);
                    if (value < KindsOfParamsByNumber.GetLength(0))
                    {
                        row["KINDSOFPARAMS_STR"] = KindsOfParamsByNumber[value];
                    }
                }

                if (!row.IsNull("MONTH"))
                {
                    int value = Convert.ToInt32(row["MONTH"]);
                    if (value >= 1 && value <= 12)
                    {
                        row["MONTH_STR"] = MonthByNumber[value - 1];
                    }
                }
            }
        }

        /// <summary>
        /// Преобразвует числовые значения полей таблиц истории закачки в символьные значения
        /// </summary>
        /// <param name="ds">Датасет истории</param>
        private static void ConvertPumpHistoryTablesColumns(DataSet ds)
        {
            DataTable dt = ds.Tables["PUMPHISTORY"];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];

                if (Convert.ToInt32(row["STARTEDBY"]) == 0)
                {
                    row["STARTEDBY_STR"] = "Планировшиком";
                }
                else
                {
                    row["STARTEDBY_STR"] = "Пользователем";
                }
            }
        }

        /// <summary>
        /// Настраивает столбцы и вид грида истонии закачки
        /// </summary>
        /// <param name="ug">Грид истории</param>
        /// <param name="ds">Датасет с историей</param>
        /// <param name="historyBand"></param>
        /// <param name="dataSourcesBand"></param>
        private void MakeHistoryGrid(UltraGrid ug, DataSet ds, int historyBand, int dataSourcesBand)
        {
            // Преобразуем числовые значения полей таблиц истории закачки в символьные значения
            ConvertPumpHistoryTablesColumns(ds);
            // Преобразуем числовые значения полей таблиц источников закачки в символьные значения
            ConvertDataSourcesTablesColumns(ds);

            ug.DataSource = null;
            ug.DataSource = ds;

            // Настраиваем вид грида истории закачки
            CustomizePumpHistory(ug, historyBand);
            // Настраиваем грид источников (подчиненный истории закачки)
            CustomizeDataSources(ug, dataSourcesBand);

            ug.DisplayLayout.Bands[historyBand].Header.Caption = "Закачки данных";
            ug.DisplayLayout.Bands[historyBand].Header.Appearance.TextHAlign = HAlign.Left;
            ug.DisplayLayout.Bands[historyBand].HeaderVisible = true;
            ug.DisplayLayout.Bands[historyBand].Columns["ID"].SortIndicator = SortIndicator.Descending;
            ug.DisplayLayout.Bands[historyBand].SortedColumns.Add("ID", true, false);

            ug.DisplayLayout.Bands[dataSourcesBand].Header.Caption = "Источники данных";
            ug.DisplayLayout.Bands[dataSourcesBand].Header.Appearance.TextHAlign = HAlign.Left;
            ug.DisplayLayout.Bands[dataSourcesBand].HeaderVisible = true;
            ug.DisplayLayout.Bands[dataSourcesBand].Columns["ID"].SortIndicator = SortIndicator.Ascending;
            ug.DisplayLayout.Bands[dataSourcesBand].SortedColumns.Add("ID", false, false);

            if (ug.Rows.Count > 0)
            {
                ug.ActiveRow = ug.Rows[0];
            }
        }

        /// <summary>
        /// Заполняет грид истории закачки
        /// </summary>
        private void FillHistoryDataSets()
        {
            if (dataPumpProgress == null || pumpRegistryElement == null) return;

            if (pumpHistoryIsLoaded)
            {
                SyncControlsByPumpState(dataPumpProgress.State);
                return;
            }

            pumpHistoryIsLoaded = true;

            Workplace.OperationObj.Text = "Запрос данных истории закачки...";
            Workplace.OperationObj.StartOperation();

            try
            {
                dpv.ugPumpHistory.BeginUpdate();
                dpv.ugDataSources.BeginUpdate();

                dpv.ugPumpHistory.AfterRowActivate -= new EventHandler(ugPumpHistory_AfterRowActivate);
                dpv.ugDataSources.AfterRowActivate -= new EventHandler(ugDataSources_AfterRowActivate);

                // Формируем коллекцию с данными о соответствии источников данных истории закачки
                FillHistory2DataSourcesMapping();

                // Инициализируем датасет истории закачки
                InitPumpHistoryDS();
                // Настраиваем столбцы и вид гридов истонии закачки
                MakeHistoryGrid(dpv.ugPumpHistory, dsPumpHistory, 0, 1);

                // Инициализируем датасет истории закачки
                InitDataSourcesDS();
                // Настраиваем столбцы и вид гридов истонии закачки
                MakeHistoryGrid(dpv.ugDataSources, dsDataSources, 1, 0);

                // Устанавливаем разрешения на кнопки удаления данных истории закачки
                SetEnableDeleteDataForGrids(!deletePumpEnabled);

                dpv.ugPumpHistory.AfterRowActivate += new EventHandler(ugPumpHistory_AfterRowActivate);
                dpv.ugDataSources.AfterRowActivate += new EventHandler(ugDataSources_AfterRowActivate);

                AfterGridRowActivate(dpv.ugPumpHistory, dsPumpHistory);
            }
            /* Исключения должны обрабатываться централизовано
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
            */
            finally
            {
                Workplace.OperationObj.StopOperation();
                dpv.ugPumpHistory.EndUpdate();
                dpv.ugDataSources.EndUpdate();
            }
        }

        /// <summary>
        /// Устанавливает текст лабела описания записи истории закачки
        /// </summary>
        /// <param name="text">Описание</param>
        private void SetHistoryInfoLabelText(string text)
        {
            SetLabelText(dpv.ulHistoryInfo, text);
        }

        #endregion Функции для работы с историей закачки


        #region Функции для работы с логами

        /// <summary>
        /// Добавляет в статусбар панель информации о закачке
        /// </summary>
        /// <returns>Панель</returns>
        private UltraStatusPanel AddDataPumpInfoStatusBarPanel()
        {
            // Ищем панель среди добавленных вручную - через Contains она не находится :(
            foreach (UltraStatusPanel panel in Workplace.MainStatusBar.Panels)
            {
                if (String.Compare(panel.Key, "DataPumpInfo", true) == 0)
                {
                    return panel;
                }
            }

            Workplace.MainStatusBar.Panels.Add("DataPumpInfo", PanelStyle.Text);

            return Workplace.MainStatusBar.Panels["DataPumpInfo"];
        }

        /// <summary>
        /// Устанавливает текст лабела информации о закачке
        /// </summary>
        private void SetPumpInfoText()
        {
            if (pumpRegistryElement == null) return;

            AddDataPumpInfoStatusBarPanel().Text =
                string.Format("Закачек: {0}", pumpRegistryElement.PumpHistoryCollection.Count);
        }

        /// <summary>
        /// Формирует название файла протокола
        /// </summary>
        /// <param name="mt">Тип протокола</param>
        /// <returns>Название файла</returns>
        // данный метод не задействован
        private string GetProtocolFileName(ModulesTypes mt)
        {
            string fileNamePattern = string.Format("Протокол_{0}", pumpRegistryElement.ProgramIdentifier);

            switch (mt)
            {
                case ModulesTypes.BridgeOperationsModule:
                    return string.Format("{0}_Сопоставление данных", fileNamePattern);

                case ModulesTypes.DataPumpModule:
                    return string.Format("{0}_Закачка данных", fileNamePattern);

                case ModulesTypes.DeleteDataModule:
                    return string.Format("{0}_Удаление данных", fileNamePattern);

                case ModulesTypes.MDProcessingModule:
                    return string.Format("{0}_Расчет кубов", fileNamePattern);

                case ModulesTypes.ProcessDataModule:
                    return string.Format("{0}_Обработка данных", fileNamePattern);

                case ModulesTypes.ReviseDataModule:
                    return string.Format("{0}_Проверка данных", fileNamePattern);
            }

            return fileNamePattern;
        }

        private void GetPumpLogConstraint(IDatabase db, int pumpID, int sourceID, ref string constraint, ref IDbDataParameter[] dbParams)
        {
            if (pumpID > 0)
            {
                constraint += "PumpHistoryID = ?";
                dbParams = new IDbDataParameter[] { db.CreateParameter("PumpHistoryID", pumpID) };
            }
            if (sourceID > 0)
            {
                if (constraint != string.Empty)
                    constraint += " and ";
                constraint += "DataSourceID = ?";
                if (dbParams != null)
                    dbParams = new IDbDataParameter[] { db.CreateParameter("PumpHistoryID", pumpID), 
                                                        db.CreateParameter("DataSourceID", sourceID) };
                else
                    dbParams = new IDbDataParameter[] { db.CreateParameter("DataSourceID", sourceID) };
            }
            if ((pumpID < 0) && (sourceID < 0))
            {
                constraint = "PumpHistoryID = ? and DataSourceID = ?";
                dbParams = new IDbDataParameter[] { db.CreateParameter("PumpHistoryID", pumpID), 
                                                    db.CreateParameter("DataSourceID", sourceID) };
            }
        }

        /// <summary>
        /// Обновляет данные внедренного лога
        /// </summary>
        /// <param name="ctrl">Контрол, в который инплэейсить</param>
        /// <param name="mt">Вид лога</param>
        /// <param name="ipv">Внедренный грид</param>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="batchID">ИД пакета обработки кубов</param>
        private void RefreshPumpLog(Control ctrl, ModulesTypes mt, ref IInplaceProtocolView ipv,
            int pumpID, int sourceID, string batchID)
        {
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string fileName = pumpRegistryElement.ProgramIdentifier;
                string constraint = string.Empty;
                IDbDataParameter[] dbParams = null;
                // страница обработки кубов завязана не на PumpID, а на BatchID
                if (mt == ModulesTypes.MDProcessingModule)
                {
                    // если batchID пустой - ничего не отображаем
                    if (batchID == string.Empty)
                    {
                        constraint = "1 <> 1";
                        dbParams = new IDbDataParameter[] {};
                    }
                    else
                    {
                        constraint = "BatchID = ?";
                        dbParams = new IDbDataParameter[] {db.CreateParameter("BatchID", batchID, DbType.AnsiString)};
                    }
                    if (ipv == null)
                    {
                        ipv = Workplace.ProtocolsInplacer;
                        if (ipv == null)
                            return;
                        ipv.AttachViewObject(mt, ctrl, fileName, constraint, dbParams);
                    }
                    else
                        ipv.RefreshAttachData(fileName, constraint, dbParams);
                    return;
                }
                // получаем ограничение выборки
                GetPumpLogConstraint(db, pumpID, sourceID, ref constraint, ref dbParams);
                // отображаем данные
                if (ipv == null)
                {
                    ipv = Workplace.ProtocolsInplacer;
                    if (ipv == null)
                        return;
                    ipv.AttachViewObject(mt, ctrl, fileName, constraint, dbParams);
                }
                else
                    ipv.RefreshAttachData(fileName, constraint, dbParams);
            }
        }

        /// <summary>
        /// Заполняет гриды логов
        /// </summary>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="batchID">ИД пакета обработки кубов</param>
        private void FillLogGrids(int pumpID, string batchID)
        {
            RefreshPumpLog(dpv.utpcPumpDataLog, ModulesTypes.DataPumpModule,
                ref ipvPumpData, pumpID, -1, batchID);

            RefreshPumpLog(dpv.utpcProcessDataLog, ModulesTypes.ProcessDataModule,
                ref ipvProcessData, pumpID, -1, batchID);

            RefreshPumpLog(dpv.utpcAssociateDataLog, ModulesTypes.BridgeOperationsModule,
                ref ipvAssociateData, pumpID, -1, batchID);

            RefreshPumpLog(dpv.utpcProcessCubeLog, ModulesTypes.MDProcessingModule,
                ref ipvProcessCube, pumpID, -1, batchID);

            RefreshPumpLog(dpv.utpcCheckDataLog, ModulesTypes.ReviseDataModule,
                ref ipvCheckData, pumpID, -1, batchID);

            RefreshPumpLog(dpv.utpcDeleteDataLog, ModulesTypes.DeleteDataModule,
                ref ipvDeleteData, pumpID, -1, batchID);

            RefreshPumpLog(dpv.utpcClassifiers, ModulesTypes.ClassifiersModule,
                ref ipvClassifiers, pumpID, -1, batchID);

            Workplace.ViewObjectCaption = pumpRegistryElement.Name;
        }

        /// <summary>
        /// Очищает гриды протоколов
        /// </summary>
        private void ClearLogGrids()
        {
            dpv.utpcPumpDataLog.Controls.Clear();
            dpv.utpcProcessDataLog.Controls.Clear();
            dpv.utpcAssociateDataLog.Controls.Clear();
            dpv.utpcProcessCubeLog.Controls.Clear();
            dpv.utpcCheckDataLog.Controls.Clear();
            dpv.utpcDeleteDataLog.Controls.Clear();
            dpv.utpcClassifiers.Controls.Clear();

            ipvPumpData = null;
            ipvProcessData = null;
            ipvAssociateData = null;
            ipvProcessCube = null;
            ipvCheckData = null;
            ipvDeleteData = null;
            ipvClassifiers = null;
        }

        /// <summary>
        /// Заполняет логи по выбранной записи истории
        /// </summary>
        private void FillLogs()
        {
            pumpHistoryIsLoaded = true;

            if (dataPumpProgress == null) return;

            if (dsPumpHistory.Tables["PUMPHISTORY"].Rows.Count == 0)
            {
                ClearLogGrids();

                SetHistoryInfoLabelText("Нет данных истории закачки");

                return;
            }

            Workplace.OperationObj.Text = "Запрос данных протоколов...";
            Workplace.OperationObj.StartOperation();

            try
            {
                if ((dpv.ugPumpHistory.ActiveRow.Cells.Contains("ID")) || (dpv.ugPumpHistory.Rows.Count > 0))
                {
                    int pumpID = Convert.ToInt32(dpv.ugPumpHistory.ActiveRow.Cells["ID"].Text);
                    string batchID = dpv.ugPumpHistory.ActiveRow.Cells["BATCHID"].Text;
                    FillLogGrids(pumpID, batchID);
                }
            }
            finally
            {
                Workplace.OperationObj.StopOperation();
            }
        }

        #endregion Функции для работы с логами


        #region Функции для работы с параметрами закачки

        /// <summary>
        /// Загружает параметры закачки
        /// </summary>
        private void CreatePumpParams()
        {
            if (dataPumpProgress == null || pumpRegistryElement == null) return;

            if (pumpRegistryElement.ProgramConfig != string.Empty)
            {
#pragma warning disable 197
                string str = pumpParams.CreateControls(ref dpv.pGeneralPumpParams,
                    ref dpv.pIndividualPumpParams, pumpRegistryElement.ProgramConfig);
#pragma warning restore 197

                if (str != string.Empty)
                {
                    throw new Exception(str);
                }

                dpv.scGeneralParams.SplitterDistance = 30;
                dpv.scIndividualParams.SplitterDistance = 30;
            }
        }

        /// <summary>
        /// Сохраняет параметры закачки
        /// </summary>
        private void SavePumpParams()
        {
            if (pumpRegistryElement != null)
            {
                string pump_params = pumpRegistryElement.ProgramConfig;
                string str = pumpParams.SaveControls(
                    dpv.pGeneralPumpParams, dpv.pIndividualPumpParams, ref pump_params);

                if (str != string.Empty)
                {
                    throw new Exception(str);
                }
                else
                {
                    if (pump_params == string.Empty)
                    {
                        throw new Exception("Ошибка при сохранении параметров закачки");
                    }
                    else
                    {
                        pumpRegistryElement.ProgramConfig = pump_params;
                    }
                }
            }
        }

        #endregion Функции для работы с параметрами закачки


        #region Функции общего управления табконтролами

        /// <summary>
        /// Заполняет лабелы комментариев закладки управления закачкой
        /// </summary>
        private void SetPumpTabInfoLabels()
        {
            if (dataPumpProgress == null) return;

            SetPumpInfoText();

            SetLabelText(dpv.ulPreviewDataComment,
                pumpRegistryElement.StagesQueue[PumpProcessStates.PreviewData].Comment);
            SetLabelText(dpv.ulPumpDataComment,
                pumpRegistryElement.StagesQueue[PumpProcessStates.PumpData].Comment);
            SetLabelText(dpv.ulProcessDataComment,
                pumpRegistryElement.StagesQueue[PumpProcessStates.ProcessData].Comment);
            SetLabelText(dpv.ulAssociateDataComment,
                pumpRegistryElement.StagesQueue[PumpProcessStates.AssociateData].Comment);
            SetLabelText(dpv.ulProcessCubeComment,
                pumpRegistryElement.StagesQueue[PumpProcessStates.ProcessCube].Comment);
            SetLabelText(dpv.ulCheckDataComment,
                pumpRegistryElement.StagesQueue[PumpProcessStates.CheckData].Comment);
        }

        /// <summary>
        /// Устанавливает видимость и текст лабелов с сообщениями об отсутствии прав
        /// </summary>
        private void SetPermissionsMsg()
        {
            if (!startPumpEnabled || !stopPumpEnabled)
            {
                if (!startPumpEnabled && !stopPumpEnabled)
                {
                    SetLabelText(dpv.ulPumpRuleMsg, "Нет прав на запуск и прекращение закачки.");
                }
                else if (!startPumpEnabled)
                {
                    SetLabelText(dpv.ulPumpRuleMsg, "Нет прав на запуск закачки.");
                }
                else if (!stopPumpEnabled)
                {
                    SetLabelText(dpv.ulPumpRuleMsg, "Нет прав на прекращение закачки.");
                }

                if (dpv.ulPumpRuleMsg.Text != string.Empty)
                {
                    SetLabelVisible(dpv.ulPumpRuleMsg, true);
                }
            }
            else
            {
                SetLabelVisible(dpv.ulPumpRuleMsg, false);
            }

            if (!deletePumpEnabled)
            {
                SetLabelVisible(dpv.ulHistoryMsg, true);
                SetLabelText(dpv.ulHistoryMsg, "Нет прав на удаление выполненных операций закачки.");
            }
            else
            {
                SetLabelVisible(dpv.ulHistoryMsg, false);
            }
        }

        /// <summary>
        /// Изменяет размеры контролов файлового менеджера в зависимости от размеров формы
        /// </summary>
        private void ResizeBrowsersControls()
        {
            dpv.tstbLeftBrowserCaption.Width = dpv.tsLeftBrowserCaption.Width - 10;
            dpv.tstbRightBrowserCaption.Width = dpv.tsRightBrowserCaption.Width - 10;
        }

        /// <summary>
        /// Инициализация файлового менеджера
        /// </summary>
        private void InitBrowsers()
        {
            if (dataPumpProgress == null) return;

            try
            {
                LeftCurrentPath = GetDataSourcesUNCPath();
            }
            catch
            {
                LeftCurrentPath = "C:\\";
            }

            RightCurrentPath = "C:\\";

            LeftBrowserViewMode = FolderViewMode.FVM_LIST;
            RightBrowserViewMode = FolderViewMode.FVM_LIST;

            ResizeBrowsersControls();
        }

        /// <summary>
        /// Синхронизирует содержимое закладок с записью реестра закачек
        /// </summary>
        /// <param name="activated">true - активация интрефейса, false - деактивация (переход на другой интерфейс)</param>
        private void SyncTabs(bool activated)
        {
            if (pumpRegistryElement == null || dataPumpProgress == null) return;

            //try
            //{
                if (!activated)
                {
                    // Если находимся на закладке параметров, то загружаем их
                    if (dpv.utcPumpControl.SelectedTab == dpv.utpcPumpParams.Tab)
                    {
                        SavePumpParams();
                    }
                    // Загрузка параметров расписания текущей закачки
                    else if (dpv.utcPumpControl.SelectedTab == dpv.utpcSchedule.Tab)
                    {
                        SaveScheduleSettingsWithDialog();
                    }

                    return;
                }
                else
                {
                    // Получаем информацию о правах
                    CheckPermissions();

                    // Устанавливаем текст сообщений об отсутствии прав
                    SetPermissionsMsg();
                }

                if (dpv.utcPumpControl.SelectedTab == dpv.utpcPumpRuling.Tab)
                {
                    SetPumpTabInfoLabels();
                    SetControlsFromStagesQueue();
                }
                // Инициализация файлового менеджера
                else if (dpv.utcPumpControl.SelectedTab == dpv.utpcFileManager.Tab)
                {
                    InitBrowsers();
                }
                // Если мы стоим на закладке истории, то подгружаем их
                else if (dpv.utcPumpControl.SelectedTab == dpv.utpcExecutedOps.Tab)
                {
                    FillHistoryDataSets();
                }
                // Если мы стоим на закладке логов, то подгружаем их
                else if (dpv.utcPumpControl.SelectedTab == dpv.utpcLog.Tab)
                {
                    FillHistoryDataSets();
                    FillLogs();
                }

                // Если находимся на закладке параметров, то загружаем их
                if (dpv.utcPumpControl.SelectedTab == dpv.utpcPumpParams.Tab)
                {
                    CreatePumpParams();
                    pumpParamsSaved = false;
                }
                else if (!pumpParamsSaved)
                {
                    SavePumpParams();
                    pumpParamsSaved = true;
                }

                // Загрузка параметров расписания текущей закачки
                if (dpv.utcPumpControl.SelectedTab == dpv.utpcSchedule.Tab)
                {
                    LoadScheduleSettings();
                    pumpScheduleSaved = false;
                }
                else if (!pumpScheduleSaved)
                {
                    SaveScheduleSettingsWithDialog();
                    pumpScheduleSaved = true;
                }
            //}
            /* Исключения должны обрабатываться централизовано
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
            */
        }

        #endregion Функции общего управления табконтролами

        #endregion Функции управления табконтролами закачки


        #region Функции управления контролами файлового менеджера

        /// <summary>
        /// Возвращает заголовок для списка дисков, описывающий указанный диск
        /// </summary>
        private static string GetDriveCaption(DriveInfo drive)
        {
            try
            {
                switch (drive.DriveType)
                {
                    case DriveType.CDRom:
                        return string.Format("{0} [{1}]", drive.Name, "CD-ROM");

                    case DriveType.Removable:
                        if (drive.Name == "A:\\")
                        {
                            return string.Format("{0} [{1}]", drive.Name, "Диск 3.5\"");
                        }
                        else
                        {
                            return string.Format("{0} [{1}]", drive.Name, "Съемный");
                        }

                    default:
                        return string.Format("{0} [{1}]", drive.Name, drive.VolumeLabel);
                }
            }
            catch
            {
                return string.Format("{0}", drive.Name);
            }
        }

        /// <summary>
        /// Возвращает корневой каталог диска из его заголовка
        /// </summary>
        private static string GetRootPathFromDriveCaption(string driveCaption)
        {
            return driveCaption.Split('[')[0].Trim();
        }

        /// <summary>
        /// Заполняет список дисков компа
        /// </summary>
        private static void FillDisksList(ToolStripItemCollection collection)
        {
            collection.Clear();

            // Получаем список дисков компа
            DriveInfo[] drives = DriveInfo.GetDrives();

            // Формируем выпадающий список
            foreach (DriveInfo drive in drives)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(GetDriveCaption(drive));

                item.ImageTransparentColor = Color.Black;
                item.Tag = drive.Name;

                switch (drive.DriveType)
                {
                    case DriveType.CDRom:
                        item.Image = Properties.Resources.CD16;
                        break;

                    case DriveType.Removable:
                        if (drive.Name == "A:\\")
                        {
                            item.Image = Properties.Resources.Floppy16;
                        }
                        else
                        {
                            item.Image = Properties.Resources.CD16;
                        }
                        break;

                    case DriveType.Fixed:
                    case DriveType.Unknown:
                        item.Image = Properties.Resources.Drive16;
                        break;

                    default:
                        item.Image = Properties.Resources.SharedDrive16;
                        break;
                }

                collection.Add(item);
            }
        }

        /// <summary>
        /// Инициализирует выпадающие списки кнопок смены текущего диска
        /// </summary>
        private void InitDrivesBtnDropDown()
        {
            DriveInfo drive = new DriveInfo("C:\\");

            FillDisksList(dpv.tsbLeftDrives.DropDownItems);
            dpv.tsbLeftDrives.Text = GetDriveCaption(drive); 

            FillDisksList(dpv.tsbRightDrives.DropDownItems);
            dpv.tsbRightDrives.Text = GetDriveCaption(drive);
        }

        /// <summary>
        /// Устанавливает состояние кнопок левого тулбара файлового менеджера
        /// </summary>
        private void SetFileManagerLeftToolbarStatus()
        {
            dpv.tsbLeftBack.Enabled = (LeftCurrentPathIndex > 0);
            dpv.tsbLeftForward.Enabled = (LeftCurrentPathIndex < leftPathsVisited.Count - 1);
            dpv.tsbLeftUp.Enabled = (Directory.GetParent(LeftCurrentPath) != null);
        }

        /// <summary>
        /// Устанавливает состояние кнопок правого тулбара файлового менеджера
        /// </summary>
        private void SetFileManagerRightToolbarStatus()
        {
            dpv.tsbRightBack.Enabled = (RightCurrentPathIndex > 0);
            dpv.tsbRightForward.Enabled = (RightCurrentPathIndex < rightPathsVisited.Count - 1);
            dpv.tsbRightUp.Enabled = (Directory.GetParent(RightCurrentPath) != null);
        }

        #endregion Функции управления контролами файлового менеджера


        #region Функции управления контролами шедулера

        //private string[] WeekDayByNumber = new string[] { "Вс", "Пн", "Вт", "Ср", "Чт", "Пт", "Сб" };

        /// <summary>
        /// Возвращает строку, содержащую информацию об отмеченных днях недели
        /// </summary>
        /// <returns></returns>
        private string MakeWeekDaysInfo()
        {
            List<int> weekDays = GetCheckedWeekDaysList();
            string result = string.Empty;

            for (int i = 0; i < weekDays.Count; i++)
            {
                result += dpv.uceWeekDay.Items[weekDays[i]].DataValue + ", ";
            }

            return result.Trim().Trim(',');
        }

        /// <summary>
        /// Массив месяцев
        /// </summary>
        private string[] DeclMonthByNumber = new string[12] {
            "января", "февраля", "марта", "апреля", "мая", "июня", "июля", "августа", "сентября", "октября",
            "ноября", "декабря" };

        /// <summary>
        /// Возвращает строку, содержащую информацию об отмеченных месяцах
        /// </summary>
        /// <returns></returns>
        private string MakeMonthsInfo()
        {
            List<int> months = GetCheckedMonthsList(false);
            if (months.Count == 0)
            {
                return "числа";
            }
            else if (months.Count == 12)
            {
                return "каждого месяца";
            }

            string result = string.Empty;

            for (int i = 0; i < months.Count; i++)
            {
                result += DeclMonthByNumber[months[i]] + ", ";
            }

            return result.Trim().Trim(',');
        }

        /// <summary>
        /// Создает строку, дающую сводную информацию о расписании
        /// </summary>
        private void MakeSchedulerInfo()
        {
            if (!scheduleEnabled)
            {
                SetLabelText(dpv.ulSchedulerInfo, "Расписание не установлено");
                return;
            }

            switch (dpv.uceSchedulePeriod.SelectedIndex)
            {
                case 0:
                    SetLabelText(dpv.ulSchedulerInfo, string.Format(
                        "Закачка будет запущена в {0}, {1}",
                        dpv.udteScheduleStartTime.DateTime.ToShortTimeString(),
                        dpv.udteScheduleStartDate.DateTime.ToShortDateString()));
                    break;

                case 1:
                    if (dpv.nudScheduleByDay.Value == 1)
                    {
                        SetLabelText(dpv.ulSchedulerInfo, string.Format(
                            "Закачка будет запущена в {0}, ежедневно, начиная с {1}",
                            dpv.udteScheduleStartTime.DateTime.ToShortTimeString(),
                            dpv.udteScheduleStartDate.DateTime.ToShortDateString()));
                    }
                    else
                    {
                        SetLabelText(dpv.ulSchedulerInfo, string.Format(
                            "Закачка будет запущена в {0}, каждый {1} день, начиная с {2}",
                            dpv.udteScheduleStartTime.DateTime.ToShortTimeString(),
                            dpv.nudScheduleByDay.Value,
                            dpv.udteScheduleStartDate.DateTime.ToShortDateString()));
                    }
                    break;

                case 2:
                    string weekDaysInfo = MakeWeekDaysInfo();
                    if (weekDaysInfo != string.Empty)
                    {
                        weekDaysInfo = " по " + weekDaysInfo;
                    }

                    if (dpv.nudScheduleByWeek.Value == 1)
                    {
                        SetLabelText(dpv.ulSchedulerInfo, string.Format(
                            "Закачка будет запущена в {0}, каждую неделю{1}, начиная с {2}",
                            dpv.udteScheduleStartTime.DateTime.ToShortTimeString(),
                            weekDaysInfo,
                            dpv.udteScheduleStartDate.DateTime.ToShortDateString()));
                    }
                    else
                    {
                        SetLabelText(dpv.ulSchedulerInfo, string.Format(
                            "Закачка будет запущена в {0}, каждую {1} неделю{2}, начиная с {3}",
                            dpv.udteScheduleStartTime.DateTime.ToShortTimeString(),
                            dpv.nudScheduleByWeek.Value,
                            weekDaysInfo,
                            dpv.udteScheduleStartDate.DateTime.ToShortDateString()));
                    }
                    break;

                case 3:
                    string monthsInfo = MakeMonthsInfo();
                    
                    string str = string.Empty;
                    if (monthsInfo == "каждого месяца")
                    {
                        str = " числа";
                    }

                    if (dpv.rbMonthlyByDayNumbers.Checked)
                    {
                        SetLabelText(dpv.ulSchedulerInfo, string.Format(
                            "Закачка будет запущена в {0}, {1}{2} {3}, начиная с {4}",
                            dpv.udteScheduleStartTime.DateTime.ToShortTimeString(),
                            dpv.nudMonthlyByDays.Value,
                            str,
                            monthsInfo,
                            dpv.udteScheduleStartDate.DateTime.ToShortDateString()));
                    }
                    else
                    {
                        SetLabelText(dpv.ulSchedulerInfo, string.Format(
                            "Закачка будет запущена в {0}, {1} {2} {3}, начиная с {4}",
                            dpv.udteScheduleStartTime.DateTime.ToShortTimeString(),
                            dpv.uceWeekNumber.Value,
                            dpv.uceWeekDay.Value,
                            monthsInfo,
                            dpv.udteScheduleStartDate.DateTime.ToShortDateString()));
                    }
                   break;

                case 4:
                   if (dpv.nudScheduleByHour.Value == 1)
                   {
                       SetLabelText(dpv.ulSchedulerInfo, string.Format(
                           "Закачка будет запущена в {0}, ежечасно, начиная с {1}",
                           dpv.udteScheduleStartTime.DateTime.ToShortTimeString(),
                           dpv.udteScheduleStartDate.DateTime.ToShortDateString()));
                   }
                   else
                   {
                       SetLabelText(dpv.ulSchedulerInfo, string.Format(
                           "Закачка будет запущена в {0}, каждый {1} час, начиная с {2}",
                           dpv.udteScheduleStartTime.DateTime.ToShortTimeString(),
                           dpv.nudScheduleByHour.Value,
                           dpv.udteScheduleStartDate.DateTime.ToShortDateString()));
                   }

                   break;
            }
        }

        /// <summary>
        /// Загружает параметры расписания
        /// </summary>
        private void LoadScheduleSettings()
        {
            if (dataPumpProgress == null) return;

            scheduleMonthsChanged = false;
            scheduleInitializing = true;

            dpv.utcSchedulePeriod.Style = UltraTabControlStyle.Wizard;
            dpv.uceSchedulePeriod.SelectedIndex = 0;
            dpv.uceWeekDay.SelectedIndex = 0;
            dpv.uceWeekNumber.SelectedIndex = 0;

            // Загружаем данные о расписании текущей закачки
            currentScheduleSettings = dataPumpManager.PumpScheduler.LoadScheduleSettings(pumpRegistryElement.ProgramIdentifier);

            // Иницилизируем контролы по данным расписания
            InitScheduleControls(currentScheduleSettings);

            MakeSchedulerInfo();

            scheduleInitializing = false;
        }

        /// <summary>
        /// Инициализация контролов по данным расписания
        /// </summary>
        /// <param name="ss">Данные расписания</param>
        private void InitScheduleControls(ScheduleSettings ss)
        {
            scheduleEnabled = currentScheduleSettings.Enabled;
            //SetSchedulerControlsEnabled();

            switch (ss.Periodicity)
            {
                case SchedulePeriodicity.Once:
                    break;

                case SchedulePeriodicity.Daily: InitDailySchedule(ss);
                    break;

                case SchedulePeriodicity.Weekly: InitWeeklySchedule(ss);
                    break;

                case SchedulePeriodicity.Monthly: InitMonthlySchedule(ss);
                    break;

                case SchedulePeriodicity.Hour: InitHourSchedule(ss);
                    break;
            }

            dpv.udteScheduleStartDate.DateTime = ss.StartDate;
            dpv.udteScheduleStartTime.DateTime = ss.StartTime;
        }

        /// <summary>
        /// Инициализирует контролы расписания, запускаемого ежедневно
        /// </summary>
        /// <param name="ss">Данные расписания</param>
        private void InitDailySchedule(ScheduleSettings ss)
        {
            dpv.utcSchedulePeriod.SelectedTab = dpv.utcSchedulePeriod.Tabs["Daily"];
            dpv.uceSchedulePeriod.SelectedIndex = 1;

            DailySchedule ds = ss.Schedule as DailySchedule;

            if (ds != null)
            {
                dpv.nudScheduleByDay.Value = ds.DayPeriod;
            }
        }

        /// <summary>
        /// Инициализирует контролы расписания, запускаемого ежечасно
        /// </summary>
        /// <param name="ss">Данные расписания</param>
        private void InitHourSchedule(ScheduleSettings ss)
        {
            dpv.utcSchedulePeriod.SelectedTab = dpv.utcSchedulePeriod.Tabs["Hour"];
            dpv.uceSchedulePeriod.SelectedIndex = 4;

            HourSchedule ds = ss.Schedule as HourSchedule;

            if (ds != null)
            {
                dpv.nudScheduleByHour.Value = ds.HourPeriod;
            }
        }

        /// <summary>
        /// Инициализирует контролы расписания, запускаемого еженедельно
        /// </summary>
        /// <param name="ss">Данные расписания</param>
        private void InitWeeklySchedule(ScheduleSettings ss)
        {
            dpv.utcSchedulePeriod.SelectedTab = dpv.utcSchedulePeriod.Tabs["Weekly"];
            dpv.uceSchedulePeriod.SelectedIndex = 2;

            WeeklySchedule ws = ss.Schedule as WeeklySchedule;

            if (ws != null)
            {
                dpv.nudScheduleByWeek.Value = ws.Week;

                for (int i = 0; i < ws.WeekDays.Count; i++)
                {
                    switch (ws.WeekDays[i])
                    {
                        case 1: dpv.uceMonday.Checked = true;
                            break;

                        case 2: dpv.uceTuesday.Checked = true;
                            break;

                        case 3: dpv.uceWednesday.Checked = true;
                            break;

                        case 4: dpv.uceThursday.Checked = true;
                            break;

                        case 5: dpv.uceFriday.Checked = true;
                            break;

                        case 6: dpv.uceSaturday.Checked = true;
                            break;

                        case 0: dpv.uceSunday.Checked = true;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Инициализирует контролы расписания, запускаемого ежемесячно
        /// </summary>
        /// <param name="ss">Данные расписания</param>
        private void InitMonthlySchedule(ScheduleSettings ss)
        {
            dpv.utcSchedulePeriod.SelectedTab = dpv.utcSchedulePeriod.Tabs["Monthly"];
            dpv.uceSchedulePeriod.SelectedIndex = 3;

            MonthlySchedule ms = ss.Schedule as MonthlySchedule;

            if (ms != null)
            {
                switch (ms.MonthlyScheduleKind)
                {
                    case MonthlyScheduleKind.ByDayNumbers:
                        dpv.rbMonthlyByDayNumbers.Checked = true;
                        MonthlyByDayNumbers mdn = (MonthlyByDayNumbers)ms.Schedule;
                        dpv.nudScheduleByWeek.Value = mdn.Day;
                        break;

                    case MonthlyScheduleKind.ByWeekDays:
                        dpv.rbMonthlyByWeekDays.Checked = true;
                        MonthlyByWeekDays mwd = (MonthlyByWeekDays)ms.Schedule;
                        dpv.uceWeekNumber.SelectedIndex = mwd.Week;
                        dpv.uceWeekDay.SelectedIndex = mwd.Day;
                        break;
                }
            }
        }

        /// <summary>
        /// Сохраняет настройки расписания
        /// </summary>
        private void SaveScheduleSettings()
        {
            if (dataPumpProgress == null || currentScheduleSettings == null) return;

            // Сохраняем состояние контролов
            SaveScheduleControls(/*currentScheduleSettings*/);

            //scheduleChangedWarning = true;

            dataPumpManager.PumpScheduler.SaveScheduleSettings(
                pumpRegistryElement.ProgramIdentifier, currentScheduleSettings);

            scheduleChanged = false;
        }

        /// <summary>
        /// Сохраняет настройки расписания с вопросом о сохранении пользователю
        /// </summary>
        private void SaveScheduleSettingsWithDialog()
        {
            if (!scheduleChanged) return;

            if (MessageBox.Show("Расписание было изменено. Сохранить изменения?", "", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                SaveScheduleSettings();
            }
            else
            {
                scheduleChanged = false;
            }
        }

        /// <summary>
        /// Сохраняет состояние контролов расписания
        /// </summary>
        private void SaveScheduleControls(/*ScheduleSettings ss*/)
        {
            currentScheduleSettings.Enabled = scheduleEnabled;
            currentScheduleSettings.StartDate = dpv.udteScheduleStartDate.DateTime;
            currentScheduleSettings.StartTime = dpv.udteScheduleStartTime.DateTime;

            // Сохраняем состояние контролов
            switch (dpv.uceSchedulePeriod.SelectedIndex)
            {
                case 0: 
                    currentScheduleSettings.Periodicity = SchedulePeriodicity.Once;
                    break;

                case 1: 
                    currentScheduleSettings.Periodicity = SchedulePeriodicity.Daily;
                    SaveDailySchedule(currentScheduleSettings);
                    break;

                case 2: 
                    currentScheduleSettings.Periodicity = SchedulePeriodicity.Weekly;
                    SaveWeeklySchedule(currentScheduleSettings);
                    break;

                case 3: 
                    currentScheduleSettings.Periodicity = SchedulePeriodicity.Monthly;
                    SaveMonthlySchedule(currentScheduleSettings);
                    break;

                case 4:
                    currentScheduleSettings.Periodicity = SchedulePeriodicity.Hour;
                    SaveHourSchedule(currentScheduleSettings);
                    break;

            }
        }

        /// <summary>
        /// Сохраняет состояние контролов расписания, выполняемого ежедневно
        /// </summary>
        /// <param name="ss">Данные расписания</param>
        private void SaveDailySchedule(ScheduleSettings ss)
        {
            DailySchedule ds = new DailySchedule();
            ss.Schedule = ds;

            ds.DayPeriod = (int)dpv.nudScheduleByDay.Value;
        }

        /// <summary>
        /// Сохраняет состояние контролов расписания, выполняемого ежечасно
        /// </summary>
        /// <param name="ss">Данные расписания</param>
        private void SaveHourSchedule(ScheduleSettings ss)
        {
            HourSchedule ds = new HourSchedule();
            ss.Schedule = ds;

            ds.HourPeriod = (int)dpv.nudScheduleByHour.Value;
        }

        /// <summary>
        /// Сохраняет состояние контролов расписания, выполняемого еженедельно
        /// </summary>
        /// <param name="ss">Данные расписания</param>
        private void SaveWeeklySchedule(ScheduleSettings ss)
        {
            WeeklySchedule ws = new WeeklySchedule();
            ss.Schedule = ws;

            ws.Week = (int)dpv.nudScheduleByWeek.Value;

            ws.WeekDays = GetCheckedWeekDaysList();
        }

        /// <summary>
        /// Создает список отмеченных дней недели
        /// </summary>
        private List<int> GetCheckedWeekDaysList()
        {
            List<int> result = new List<int>(7);

            if (dpv.uceMonday.Checked) result.Add(1);
            if (dpv.uceTuesday.Checked) result.Add(2);
            if (dpv.uceWednesday.Checked) result.Add(3);
            if (dpv.uceThursday.Checked) result.Add(4);
            if (dpv.uceFriday.Checked) result.Add(5);
            if (dpv.uceSaturday.Checked) result.Add(6);
            if (dpv.uceSunday.Checked) result.Add(0);

            return result;
        }

        /// <summary>
        /// Сохраняет состояние контролов расписания, выполняемого ежемесячно
        /// </summary>
        /// <param name="ss">Данные расписания</param>
        private void SaveMonthlySchedule(ScheduleSettings ss)
        {
            MonthlySchedule ms = new MonthlySchedule();
            ss.Schedule = ms;

            if (dpv.rbMonthlyByDayNumbers.Checked)
            {
                ms.MonthlyScheduleKind = MonthlyScheduleKind.ByDayNumbers;

                MonthlyByDayNumbers mdn = new MonthlyByDayNumbers();
                ms.Schedule = mdn;

                mdn.Day = (int)dpv.nudMonthlyByDays.Value;
            }
            else
            {
                ms.MonthlyScheduleKind = MonthlyScheduleKind.ByWeekDays;

                MonthlyByWeekDays mwd = new MonthlyByWeekDays();
                ms.Schedule = mwd;

                mwd.Day = dpv.uceWeekDay.SelectedIndex;
                mwd.Week = dpv.uceWeekNumber.SelectedIndex;
            }

            ms.Months = GetCheckedMonthsList(true);
        }

        /// <summary>
        /// Возвращает список отмеченных месяцев для расписания
        /// </summary>
        /// <param name="fromMonthsForm">true - список месяцев составляется по данным формы выбора, иначе по
        /// данным класса</param>
        /// <returns>Список месяцев</returns>
        private List<int> GetCheckedMonthsList(bool fromMonthsForm)
        {
            List<int> result = new List<int>(12);

            if (scheduleMonthsChanged || fromMonthsForm)
            {
                if (scheduleMonths.uceJanuary.Checked) result.Add(0);
                if (scheduleMonths.uceFebruary.Checked) result.Add(1);
                if (scheduleMonths.uceMarch.Checked) result.Add(2);
                if (scheduleMonths.uceApril.Checked) result.Add(3);
                if (scheduleMonths.uceMay.Checked) result.Add(4);
                if (scheduleMonths.uceJune.Checked) result.Add(5);
                if (scheduleMonths.uceJuly.Checked) result.Add(6);
                if (scheduleMonths.uceAugust.Checked) result.Add(7);
                if (scheduleMonths.uceSeptember.Checked) result.Add(8);
                if (scheduleMonths.uceOctober.Checked) result.Add(9);
                if (scheduleMonths.uceNovember.Checked) result.Add(10);
                if (scheduleMonths.uceDecember.Checked) result.Add(11);
            }
            else
            {
                MonthlySchedule ms = currentScheduleSettings.Schedule as MonthlySchedule;
                if (ms != null)
                {
                    result = ms.Months;
                }
            }

            return result;
        }

        /*
        /// <summary>
        /// Устанавливает разрешения на контролы закладки шедулера
        /// </summary>
        private void SetSchedulerControlsEnabled()
        {
            dpv.uceSchedulePeriod.Enabled = scheduleEnabled;
            dpv.udteScheduleStartDate.Enabled = scheduleEnabled;
            dpv.udteScheduleStartTime.Enabled = scheduleEnabled;
            dpv.gbScheduleDaily.Enabled = scheduleEnabled;
            dpv.gbScheduleMonthly.Enabled = scheduleEnabled;
            dpv.gbScheduleWeekly.Enabled = scheduleEnabled;
            dpv.utcSchedulePeriod.Enabled = scheduleEnabled;
        }*/

        #endregion Функции управления контролами шедулера


        #region Обработчики событий

        #region Обработчики событий формы

        /// <summary>
        /// Обработчик события загрузки контрола
        /// </summary>
        private void dpv_Load(object sender, EventArgs e)
        {
            dpvIsLoaded = true;
            //SetSelectedTab(dpv.utcViews, dpv.utpcUnknownPump.Tab);
        }

        /// <summary>
        /// Обработчик события изменения размеров формы
        /// </summary>
        private void dpv_SizeChanged(object sender, EventArgs e)
        {
            // Ресайз контролов файлового менеджера
            if (dpv.utcPumpControl.SelectedTab == dpv.utpcFileManager.Tab)
            {
                ResizeBrowsersControls();
            }

            /*if (dpv.Size.Width >= dpv.MinimumSize.Width)
            {
                const int indent = 30;

                dpv.upbPreviewData.Width = dpv.utpcPumpRuling.Width - dpv.upbPreviewData.Location.X - indent;
                dpv.upbPumpData.Width = dpv.utpcPumpRuling.Width - dpv.upbPumpData.Location.X - indent;
                dpv.upbProcessData.Width = dpv.utpcPumpRuling.Width - dpv.upbProcessData.Location.X - indent;
                dpv.upbAssociateData.Width = dpv.utpcPumpRuling.Width - dpv.upbAssociateData.Location.X - indent;
                dpv.upbProcessCube.Width = dpv.utpcPumpRuling.Width - dpv.upbProcessCube.Location.X - indent;
                dpv.upbCheckData.Width = dpv.utpcPumpRuling.Width - dpv.upbCheckData.Location.X - indent;

                dpv.ulPreviewDataMessage.Width = dpv.utpcPumpRuling.Width - dpv.ulPreviewDataMessage.Location.X - indent;
                dpv.ulPumpDataMessage.Width = dpv.utpcPumpRuling.Width - dpv.ulPumpDataMessage.Location.X - indent;
                dpv.ulProcessDataMessage.Width = dpv.utpcPumpRuling.Width - dpv.ulProcessDataMessage.Location.X - indent;
                dpv.ulAssociateDataMessage.Width = dpv.utpcPumpRuling.Width - dpv.ulAssociateDataMessage.Location.X - indent;
                dpv.ulProcessCubeMessage.Width = dpv.utpcPumpRuling.Width - dpv.ulProcessCubeMessage.Location.X - indent;
                dpv.ulCheckDataMessage.Width = dpv.utpcPumpRuling.Width - dpv.ulCheckDataMessage.Location.X - indent;
            }*/
        }

        #endregion Обработчики событий формы


        #region Обработчики событий гридов

		/// <summary>
		/// Наполнение интерфейса закачки данными.
		/// </summary>
		internal void LoadData()
		{
			if (!dpvIsLoaded) return;

			pumpHistoryIsLoaded = false;

			//try
			//{
				dpv.pGeneralPumpParams.Controls.Clear();
				dpv.pIndividualPumpParams.Controls.Clear();

				HideProgressBars();
				HideLabels();
				SetButtonsState();

                SetSelectedTab(dpv.utcViews, dpv.utpcPumpControl.Tab);

				pumpRegistryElement = dataPumpManager.DataPumpInfo.PumpRegistry[programIdentifier];
				dataPumpProgress = dataPumpManager.DataPumpInfo[programIdentifier];

                InitializePumpModule();

                Workplace.ViewObjectCaption = pumpRegistryElement.Name;

                tsbLeftHome_Click(dpv.tsbLeftHome, null);
			//}
            /* Исключения должны обрабатываться централизовано
			catch (Exception ex)
			{
                ShowErrorMessage(string.Format("Ошибка при инициализации закачки: {0}", ex.Message));
			}
            */
		}

        /// <summary>
        /// Обработчик события дактивации строки грида реестра закачек
        /// </summary>
        private void ugPumpRegistry_BeforeRowDeactivate(object sender, CancelEventArgs e)
        {
            SyncTabs(false);
        }

        /// <summary>
        /// Обработчик перемещения курсора по строкам грида навигатора лога
        /// </summary>
        /// <param name="grid">Грид лога</param>
        /// <param name="ds">Датасет лога</param>
        private void AfterGridRowActivate(UltraGrid grid, DataSet ds)
        {
            if (ds.Tables["PUMPHISTORY"].Rows.Count == 0 || grid.ActiveRow == null ||
                string.Compare(grid.ActiveRow.Band.Key, "DATASOURCES", true) == 0)
                return;

            if (grid.ActiveRow.Cells != null)
            {
                UltraGridRow row;
                if (string.Compare(grid.ActiveRow.Band.Key, "PH2DS", true) == 0)
                {
                    row = grid.ActiveRow.ParentRow;
                }
                else
                {
                    row = grid.ActiveRow;
                }
                int pumpID = Convert.ToInt32(row.Cells["ID"].Text);

                // Заполняем лабел информации об истории закачки
                SetHistoryInfoLabelText(string.Format("ID закачки: {0}, дата закачки: {1}, примечание: {2}.",
                    pumpID, row.Cells["PUMPDATE"].Text, row.Cells["COMMENTS"].Text));
            }
        }

		/// <summary>
		/// Обработчик перемещения курсора по строкам грида навигатора лога
		/// </summary>
		private void ugPumpHistory_AfterRowActivate(object sender, EventArgs e)
		{
            AfterGridRowActivate(dpv.ugPumpHistory, dsPumpHistory);
		}

        /// <summary>
        /// Обработчик перемещения курсора по строкам грида навигатора лога
        /// </summary>
        private void ugDataSources_AfterRowActivate(object sender, EventArgs e)
        {
            AfterGridRowActivate(dpv.ugDataSources, dsDataSources);
        }

        /// <summary>
        /// Обрабатывает клик на кнопках в гридах истории закачки
        /// </summary>
        private void HandleHistoryButtonClick(CellEventArgs e)
        {
            switch (e.Cell.Column.Key.ToUpper())
            {
                case "DELETEDATA":
                    if (dataPumpProgress == null)
                    {
                        throw new Exception("Ошибка при инициализации программы закачки.");
                    }

                    if (MessageBox.Show("Удалить закачанные данные?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        Workplace.OperationObj.Text = "Старт удаления данных...";
                        Workplace.OperationObj.StartOperation();

                        int pumpID = -1;
                        int sourceID = -1;

                        // Если нажали на удаление источника, то удаляться должен весь источник целиком и 
                        // все закачки, которые по нему закачивались
                        if (e.Cell.Band.Key.ToUpper() == "DATASOURCES" || e.Cell.Band.Key.ToUpper() == "PH2DS")
                        {
                            sourceID = Convert.ToInt32(e.Cell.Row.Cells["ID"].Text);
                            
                            // Если удаление источника нажато на закладке истории, то удаляем данные источника
                            // с ограничением по PumpID
                            if (e.Cell.Row.ParentRow != null)
                            {
                                pumpID = Convert.ToInt32(e.Cell.Row.ParentRow.Cells["ID"].Text);
                            }
                        }
                        // Если нажали на удаление записи истории, то удаляются все источники, которые качались 
                        // в данной закачке c ограничением по текущему PumpID
                        else if (e.Cell.Band.Key.ToUpper() == "PUMPHISTORY" || e.Cell.Band.Key.ToUpper() == "DS2PH")
                        {
                            pumpID = Convert.ToInt32(e.Cell.Row.Cells["ID"].Text);
                            
                            // Если удаление истории закачки нажато на закладке источников, то удаляем данные 
                            // истории с ограничением по SourceID
                            if (e.Cell.Row.ParentRow != null)
                            {
                                sourceID = Convert.ToInt32(e.Cell.Row.ParentRow.Cells["ID"].Text);
                            }
                        }

                        // Удаляем данные
                        string err = dataPumpManager.DeleteData(pumpRegistryElement.ProgramIdentifier, pumpID, sourceID);
                        if (err != string.Empty)
                        {
                            ShowErrorMessage("При удалении данных произошла ошибка: \n" + err);
                        }
                    }
                    break;

                case "GOTOLOG":
                    dpv.utcPumpControl.SelectedTab = dpv.utcPumpControl.Tabs["Logs"];
                    break;
            }
        }

        /// <summary>
        /// Обработчик клика на ячейке грида лога
        /// </summary>
        /// <param name="grid">Грид лога</param>
        /// <param name="ds">Датасет лога</param>
        /// <param name="e">Аргументы события</param>
        private void ClickGridCellButton(UltraGrid grid, DataSet ds, CellEventArgs e)
        {
            e.Cell.Activated = false;

            if (ds.Tables["PUMPHISTORY"].Rows.Count == 0)
                return;
            if (grid.ActiveRow == null)
                return;

            HandleHistoryButtonClick(e);
        }

		/// <summary>
		/// Обработчик клика на ячейке
		/// </summary>
		private void ugPumpHistory_ClickCellButton(object sender, CellEventArgs e)
		{
            ClickGridCellButton(dpv.ugPumpHistory, dsPumpHistory, e);
		}

        /// <summary>
        /// Обработчик клика на ячейке
        /// </summary>
        private void ugDataSources_ClickCellButton(object sender, CellEventArgs e)
        {
            ClickGridCellButton(dpv.ugDataSources, dsDataSources, e);
        }

        /// <summary>
        /// Инициализация строки
        /// </summary>
        private static void ugPumpHistory_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Band.Index == 0)
            {
                e.Row.Cells["GOTOLOG"].ToolTipText = "Перейти в лог";
                if (e.Row.ChildBands != null)
                {
                    e.Row.Cells["DELETEDATA"].ToolTipText =
                        string.Format("Удалить данные (источников данных: {0})", e.Row.ChildBands[0].Rows.Count);
                }
            }
            if (e.Row.Band.Index == 1)
            {
                e.Row.Cells["DELETEDATA"].ToolTipText = "Удалить данные";
            }

            if (e.Row.Band.Index == 1)
            {
                if (e.Row.Cells["Deleted"].Value.ToString() == "1")
                {
                    // Если источник удален, скрываем строку и выходим.
                    e.Row.Hidden = true;
                    return;
                }

                UltraGridCell lockCell = e.Row.Cells["Locked"];
                lockCell.Column.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;

                if (!(Convert.ToBoolean(lockCell.Value)))
                {
                    lockCell.Appearance.ImageBackground = Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.Check;
                    lockCell.ToolTipText = "Источник открыт для изменений";
                }
                else
                {
                    lockCell.Appearance.ImageBackground = Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.ProtectForm;
                    lockCell.ToolTipText = "Источник закрыт от изменений";
                }
            }
        }

        /// <summary>
        /// Инициализация строки
        /// </summary>
        private static void ugDataSources_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Band.Index == 0 && e.Row.ChildBands != null)
            {
                e.Row.Cells["DELETEDATA"].ToolTipText =
                    string.Format("Удалить данные (записей закачки: {0})", e.Row.ChildBands[0].Rows.Count);
            }
            if (e.Row.Band.Index == 1)
            {
                e.Row.Cells["GOTOLOG"].ToolTipText = "Перейти в лог";
                e.Row.Cells["DELETEDATA"].ToolTipText = "Удалить данные";
            }

            if (e.Row.Band.Index == 0)
            {
                if (e.Row.Cells["Deleted"].Value.ToString() == "1")
                {
                    // Если источник удален, скрываем строку и выходим.
                    e.Row.Hidden = true;
                    return;
                }

                UltraGridCell lockCell = e.Row.Cells["Locked"];
                lockCell.Column.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;

                if (!(Convert.ToBoolean(lockCell.Value)))
                {
                    lockCell.Appearance.ImageBackground = Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.Check;
                    lockCell.ToolTipText = "Источник открыт для изменений";
                }
                else
                {
                    lockCell.Appearance.ImageBackground = Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.ProtectForm;
                    lockCell.ToolTipText = "Источник закрыт от изменений";
                }
            }
        }

		#endregion Обработчики событий гридов


		#region Обработчики событий мыши

		/// <summary>
		/// Обработчик прохождения курсора мыши над кнопкой
		/// </summary>
		private void ubtnStartPumpData_MouseEnter(object sender, EventArgs e)
		{
            if (!dpvIsLoaded || dataPumpProgress == null) return;

			UltraButton ubtn = (UltraButton)sender;
			if (ubtn == null) return;

            int btnIndex = Convert.ToInt32(ubtn.Tag);
            PumpProcessStates st = (PumpProcessStates)(Convert.ToInt32(ubtn.Tag) / 10);

            if (pumpRegistryElement.StagesQueue[st] == null) return;

			switch (btnIndex % 10)
			{
				case 0:
					dpv.ttHint.Show("Старт", dpv.utpcPumpRuling,
						ubtn.Location.X + ubtn.Width, ubtn.Location.Y + ubtn.Height, 1000);
					break;

				case 1:
					dpv.ttHint.Show("Пауза", dpv.utpcPumpRuling,
						ubtn.Location.X + ubtn.Width, ubtn.Location.Y + ubtn.Height, 1000);
					break;

				case 2:
					dpv.ttHint.Show("Стоп", dpv.utpcPumpRuling,
						ubtn.Location.X + ubtn.Width, ubtn.Location.Y + ubtn.Height, 1000);
					break;

				case 3:
                    if (pumpRegistryElement.StagesQueue[st].StageInitialState == StageState.Skipped)
                    {
                        dpv.ttHint.Show("В очередь", dpv.utpcPumpRuling,
                            ubtn.Location.X + ubtn.Width, ubtn.Location.Y + ubtn.Height, 1000);
                    }
                    else
                    {
                        dpv.ttHint.Show("Пропустить", dpv.utpcPumpRuling,
                            ubtn.Location.X + ubtn.Width, ubtn.Location.Y + ubtn.Height, 1000);
                    }
					break;
			}
		}

		/// <summary>
		/// Обработчик прохождения курсора мыши над кнопкой
		/// </summary>
		private void ubtnStartPumpData_MouseLeave(object sender, EventArgs e)
		{
			if (!dpvIsLoaded) return;

			dpv.ttHint.Hide(dpv.utpcPumpRuling);
		}

		/// <summary>
		/// Обработчик прохождения курсора мыши над картинкой
		/// </summary>
		private void upicPumpData_MouseEnter(object sender, EventArgs e)
		{
            if (!dpvIsLoaded || dataPumpProgress == null || pumpRegistryElement.StagesQueue == null)
            {
                return;
            }

			Infragistics.Win.UltraWinEditors.UltraPictureBox pic = 
				(Infragistics.Win.UltraWinEditors.UltraPictureBox)sender;
            if (pic == null || pic.Image == null)
            {
                return;
            }

            int tag = Convert.ToInt32(pic.Tag);
            PumpProcessStates st = (PumpProcessStates)(tag % 10);
            IStagesQueueElement sqe = pumpRegistryElement.StagesQueue[st];

            if (sqe == null)
            {
                return;
            }

            StageState ss = sqe.StageInitialState;
            if (tag / 10 == 2)
            {
                ss = sqe.StageCurrentState;
            }

            switch (ss)
			{
				case StageState.FinishedWithErrors:
					dpv.ttHint.Show("Закончено с ошибками", dpv.utpcPumpRuling,
						pic.Location.X + pic.Width, pic.Location.Y + pic.Height, 1000);
					break;

				case StageState.InProgress:
					dpv.ttHint.Show("Выполняется", dpv.utpcPumpRuling,
						pic.Location.X + pic.Width, pic.Location.Y + pic.Height, 1000);
					break;

				case StageState.InQueue:
					dpv.ttHint.Show("В очереди", dpv.utpcPumpRuling,
						pic.Location.X + pic.Width, pic.Location.Y + pic.Height, 1000);
					break;

				case StageState.Skipped:
					dpv.ttHint.Show("Пропущено", dpv.utpcPumpRuling,
						pic.Location.X + pic.Width, pic.Location.Y + pic.Height, 1000);
					break;

				case StageState.SuccefullFinished:
					dpv.ttHint.Show("Закончено успешно", dpv.utpcPumpRuling,
						pic.Location.X + pic.Width, pic.Location.Y + pic.Height, 1000);
					break;

                case StageState.Blocked:
                    dpv.ttHint.Show("Этап заблокирован", dpv.utpcPumpRuling,
                        pic.Location.X + pic.Width, pic.Location.Y + pic.Height, 1000);
                    break;
			}
		}

		#endregion Обработчики событий мыши


        #region Обработчики событий кнопок общего назначения

        /// <summary>
        /// Обработчик нажатия на кнопку обновления
        /// </summary>
        private void tsbRefresh_Click(object sender, EventArgs e)
        {
            // Обновление и,или перезагрузка контролов текущей закладки

            if (dpv.utcPumpControl.SelectedTab == dpv.utpcPumpRuling.Tab)
            {
                LoadData();
            }
            else if (dpv.utcPumpControl.SelectedTab == dpv.utpcPumpParams.Tab)
            {
                SavePumpParams();
                pumpParamsSaved = true;
            }
            else if (dpv.utcPumpControl.SelectedTab == dpv.utpcSchedule.Tab)
            {
                SaveScheduleSettingsWithDialog();
                pumpScheduleSaved = true;
            }
            else if (dpv.utcPumpControl.SelectedTab == dpv.utpcFileManager.Tab)
            {
                InitBrowsers();
            }
            else if (dpv.utcPumpControl.SelectedTab == dpv.utpcExecutedOps.Tab)
            {
                FillHistoryDataSets();
            }
            else if (dpv.utcPumpControl.SelectedTab == dpv.utpcLog.Tab)
            {
                FillHistoryDataSets();
                FillLogs();
            }
        }

        #endregion Обработчики событий кнопок общего назначения


        #region Обработчики кнопок управления закачкой

        private string GetUserParams()
        {
            string userHost = Environment.MachineName;
            LogicalCallContextData lccd = LogicalCallContextData.GetContext();
            string sessionID = Convert.ToString(lccd["SessionID"]);
            string userName = lccd.Principal.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                userName = WindowsIdentity.GetCurrent().Name;
            return string.Format("{0} {1} {2}", userName, userHost, sessionID);
        }

        /// <summary>
		/// Выполняет подготовительные действия и запускает этап
		/// </summary>
		/// <param name="state">Этап</param>
		private void RunStage(PumpProcessStates state)
		{
            if (dataPumpProgress.State == state) return;

            HideProgressBars();
            HideLabels();
            ClearLabels();
            ClearProgressBars();
            ClearCurrentImages();
            string userParams = GetUserParams();
            string err = dataPumpManager.StartPumpProgram(pumpRegistryElement.ProgramIdentifier, state, userParams);
            if (err != string.Empty)
            {
                ShowErrorMessage(err);
            }
            else
            {
                SetAllButtonsForPumpState(state);
            }

            SetImagesForPumpCurrentState(state, Properties.Resources.Start);
		}

        /// <summary>
        /// Обработчик нажатия на кнопку запуска предпросмотра
        /// </summary>
        private void ubtnStartPreviewData_Click(object sender, EventArgs e)
        {
            RunStage(PumpProcessStates.PreviewData);
        }

		/// <summary>
		/// Обработчик нажатия на кнопку запуска закачки
		/// </summary>
		private void ubtnStartPumpData_Click(object sender, EventArgs e)
		{
			RunStage(PumpProcessStates.PumpData);
		}

		/// <summary>
		/// Обработчик нажатия на кнопку запуска проверки закачанных данных
		/// </summary>
		private void ubtnStartCheckData_Click(object sender, EventArgs e)
		{
			RunStage(PumpProcessStates.CheckData);
		}

		/// <summary>
		/// Обработчик нажатия на кнопку запуска проверки закачанных данных
		/// </summary>
		private void ubtnStartProcessData_Click(object sender, EventArgs e)
		{
			RunStage(PumpProcessStates.ProcessData);
		}

		/// <summary>
		/// Обработчик нажатия на кнопку запуска сопоставления
		/// </summary>
		private void ubtnStartAssociateData_Click(object sender, EventArgs e)
		{
			RunStage(PumpProcessStates.AssociateData);
		}

		/// <summary>
		/// Обработчик нажатия на кнопку запуска расчета кубов
		/// </summary>
		private void ubtnStartProcessCube_Click(object sender, EventArgs e)
		{
			RunStage(PumpProcessStates.ProcessCube);
		}

		/*
        /// <summary>
		/// Обработчик нажатия на кнопку запуска удаления данных
		/// </summary>
		private void ubtnStartDeleteData_Click(object sender, EventArgs e)
		{
			HideProgressBars();
			HideLabels();
			ClearLabels();
			ClearProgressBars();

            string err = dataPumpManager.DeleteData(pumpRegistryElement.ProgramIdentifier, -1, -1);
            if (err != string.Empty)
            {
                ShowErrorMessage(err);
            }
		}*/

		/// <summary>
		/// Обработчик нажатия на кнопку прекращения закачки
		/// </summary>
		private void ubtnStopCurrent_Click(object sender, EventArgs e)
		{
			//try
			//{
				if (dataPumpProgress == null) return;
				dataPumpProgress.State = PumpProcessStates.Aborted;
			//}
			//catch (Exception exp)
			//{
			//}
		}

		/// <summary>
		/// Обработчик нажатия на кнопку временной приостановки закачки
		/// </summary>
		private void ubtnPauseCurrent_Click(object sender, EventArgs e)
		{
			if (dataPumpProgress == null) return;

            SetAllStageButtonsGroupEnabled(dataPumpProgress.State, true, false, true, true);

			dataPumpProgress.State = PumpProcessStates.Paused;
		}

		/// <summary>
		/// Устанавливает признак пропуска этапа
		/// </summary>
		/// <param name="st">Этап</param>
		/// <param name="isSkipped">Признак</param>
		protected void SetQueueElementSkipped(PumpProcessStates st, bool isSkipped)
		{
            IStagesQueueElement sqe = pumpRegistryElement.StagesQueue[st];

            if (sqe == null) return;

            if (isSkipped)
            {
                sqe.StageInitialState = StageState.Skipped;
            }
            else
            {
                sqe.StageInitialState = StageState.InQueue;
            }
		}

		/// <summary>
		/// Выполняет действия по пропуску этапа
		/// </summary>
		/// <param name="state">Этап</param>
		private void SkipStage(PumpProcessStates state)
		{
            IStagesQueueElement sqe = pumpRegistryElement.StagesQueue[state];

            if (sqe.StageInitialState != StageState.Blocked && sqe.StageInitialState != StageState.Skipped)
            {
                SetQueueElementSkipped(state, true);
                SetImagesForPumpInitialState(state, Properties.Resources.Skipped);

                if (dataPumpProgress.State == state)
                {
                    dataPumpProgress.State = PumpProcessStates.Skip;
                }

                if (dataPumpProgress.State < PumpProcessStates.PreviewData ||
                    dataPumpProgress.State > PumpProcessStates.CheckData)
                {
                    SetStageButtonsGroupEnabled(state, false, false, false, true);
                    sqe.SetInitialStageState(StageState.Skipped);
                }
            }
            else
            {
                SetInitialImagesForQueueElement(sqe);
            }
		}

        /// <summary>
        /// Выполняет действия по отмене пропуска этапа
        /// </summary>
        /// <param name="state">Этап</param>
        private void UnSkipStage(PumpProcessStates state)
        {
            IStagesQueueElement sqe = pumpRegistryElement.StagesQueue[state];

            if (sqe.StageInitialState != StageState.Blocked && sqe.StageInitialState != StageState.InQueue && 
                dataPumpProgress.State != state)
            {
                SetQueueElementSkipped(state, false);
                SetImagesForPumpInitialState(state, Properties.Resources.InQueue);

                if (dataPumpProgress.State < PumpProcessStates.PreviewData ||
                   dataPumpProgress.State > PumpProcessStates.CheckData)
                {
                    SetStageButtonsGroupEnabled(state, true, false, false, true);
                    sqe.SetInitialStageState(StageState.InQueue);
                }
            }
            else
            {
                SetInitialImagesForQueueElement(sqe);
            }
        }

        /// <summary>
        /// Устанавливает состояние кнопок пропуска этапов
        /// </summary>
        /// <param name="state">Этап закачки</param>
        private void SetSkipButtonsState(PumpProcessStates state)
        {
            if (dataPumpProgress == null) return;

            if (pumpRegistryElement.StagesQueue[state].StageInitialState == StageState.Skipped)
            {
                UnSkipStage(state);
            }
            else
            {
                SkipStage(state);
            }
        }

        /// <summary>
        /// Обработчик нажатия на кнопку пропуска этапа предпросмотра
        /// </summary>
        private void ubtnSkipPreviewData_Click(object sender, EventArgs e)
        {
            SetSkipButtonsState(PumpProcessStates.PreviewData);
        }

		/// <summary>
		/// Обработчик нажатия на кнопку пропуска этапа выполнения программы закачки
		/// </summary>
		private void ubtnSkipPumpData_Click(object sender, EventArgs e)
		{
            SetSkipButtonsState(PumpProcessStates.PumpData);
		}

		/// <summary>
		/// Обработчик нажатия на кнопку пропуска этапа выполнения программы закачки
		/// </summary>
		private void ubtnSkipProcessData_Click(object sender, EventArgs e)
		{
            SetSkipButtonsState(PumpProcessStates.ProcessData);
		}

		/// <summary>
		/// Обработчик нажатия на кнопку пропуска этапа выполнения программы закачки
		/// </summary>
		private void ubtnSkipAssociateData_Click(object sender, EventArgs e)
		{
            SetSkipButtonsState(PumpProcessStates.AssociateData);
		}

		/// <summary>
		/// Обработчик нажатия на кнопку пропуска этапа выполнения программы закачки
		/// </summary>
		private void ubtnSkipProcessCube_Click(object sender, EventArgs e)
		{
            SetSkipButtonsState(PumpProcessStates.ProcessCube);
		}

		/// <summary>
		/// Обработчик нажатия на кнопку пропуска этапа выполнения программы закачки
		/// </summary>
		private void ubtnSkipCheckData_Click(object sender, EventArgs e)
		{
            SetSkipButtonsState(PumpProcessStates.CheckData);
		}

		#endregion Обработчики кнопок управления закачкой


        #region Обработчики событий файлового менеджера

        #region Обработчики событий кнопок файлового менеджера

        private void tsbLeftHome_Click(object sender, EventArgs e)
        {
            if (dataPumpProgress == null) return;

            string path = GetDataSourcesUNCPath();

            try
            {
                if (path != string.Empty)
                {
                    LeftCurrentPath = path;
                    //dpv.tslLeftCaption.Text = "Данные для закачки";
                }
                else
                {
                    ShowErrorMessage("Указанный каталог источников данных не существует или недоступен.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Ошибка при попытке доступа по пути \"{0}\"", path), ex);
                //ShowErrorMessage(string.Format("Ошибка при попытке доступа по пути \"{0}\": {1}", path, ex.Message));
            }
        }

        private void tsbRightHome_Click(object sender, EventArgs e)
        {
            if (dataPumpProgress == null) return;

            string path = GetDataSourcesUNCPath();

            try
            {
                if (path != string.Empty)
                {
                    RightCurrentPath = path;
                    //dpv.tslRightCaption.Text = "Данные для закачки";
                }
                else
                {
                    ShowErrorMessage("Указанный каталог источников данных не существует или недоступен.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Ошибка при попытке доступа по пути \"{0}\"", path), ex);
                //ShowErrorMessage(string.Format("Ошибка при попытке доступа по пути \"{0}\": {1}", path, ex.Message));
            }
        }

        private void tsbLeftBack_ButtonClick(object sender, EventArgs e)
        {
            if ((LeftCurrentPathIndex - 1) > -1 &&
                (LeftCurrentPathIndex - 1) < leftPathsVisited.Count)
            {
                LeftCurrentPath = leftPathsVisited[LeftCurrentPathIndex - 1] as string;
            }
        }

        private void tsbLeftBack_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            LeftCurrentPath = e.ClickedItem.Text;
        }

        private void tsbLeftBack_DropDownOpening(object sender, EventArgs e)
        {
            // Заполняем выпадающий список
            dpv.tsbLeftBack.DropDownItems.Clear();
            for (int i = LeftCurrentPathIndex - 1; i > -1; i--)
            {
                dpv.tsbLeftBack.DropDownItems.Add(leftPathsVisited[i] as string);
            }
        }

        private void tsbLeftForward_ButtonClick(object sender, EventArgs e)
        {
            if ((LeftCurrentPathIndex + 1) > -1 &&
                (LeftCurrentPathIndex + 1) < leftPathsVisited.Count)
            {
                LeftCurrentPath = leftPathsVisited[LeftCurrentPathIndex + 1] as string;
            }
        }

        private void tsbLeftForward_DropDownOpening(object sender, EventArgs e)
        {
            // Заполняем выпадающий список
            dpv.tsbLeftForward.DropDownItems.Clear();
            for (int i = LeftCurrentPathIndex + 1; i < leftPathsVisited.Count; i++)
            {
                dpv.tsbLeftForward.DropDownItems.Add(leftPathsVisited[i] as string);
            }
        }

        private void tsbLeftRefresh_Click(object sender, EventArgs e)
        {
            dpv.wbLeftBrowser.Refresh();
            FillDisksList(dpv.tsbLeftDrives.DropDownItems);
        }

        private void tsbLeftUp_Click(object sender, EventArgs e)
        {
            // Получаем родительский каталог текущего каталога
            DirectoryInfo parentDirectory = Directory.GetParent(LeftCurrentPath);

            if (parentDirectory != null)
            {
                LeftCurrentPath = parentDirectory.FullName;
            }
        }

        private void tsbLeftView_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "Значки": LeftBrowserViewMode = FolderViewMode.FVM_ICON;
                    break;

                case "Список": LeftBrowserViewMode = FolderViewMode.FVM_LIST;
                    break;

                case "Таблица": LeftBrowserViewMode = FolderViewMode.FVM_DETAILS;
                    break;
            }
        }

        private void tsbRightBack_ButtonClick(object sender, EventArgs e)
        {
            if ((RightCurrentPathIndex - 1) > -1 &&
                (RightCurrentPathIndex - 1) < rightPathsVisited.Count)
            {
                RightCurrentPath = rightPathsVisited[RightCurrentPathIndex - 1] as string;
            }
        }

        private void tsbRightBack_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            RightCurrentPath = e.ClickedItem.Text;
        }

        private void tsbRightBack_DropDownOpening(object sender, EventArgs e)
        {
            // Заполняем выпадающий список
            dpv.tsbRightBack.DropDownItems.Clear();
            for (int i = RightCurrentPathIndex - 1; i > -1; i--)
            {
                dpv.tsbRightBack.DropDownItems.Add(rightPathsVisited[i] as string);
            }
        }

        private void tsbRightForward_ButtonClick(object sender, EventArgs e)
        {
            if ((RightCurrentPathIndex + 1) > -1 &&
                (RightCurrentPathIndex + 1) < rightPathsVisited.Count)
            {
                RightCurrentPath = rightPathsVisited[RightCurrentPathIndex + 1] as string;
            }
        }

        private void tsbRightForward_DropDownOpening(object sender, EventArgs e)
        {
            // Заполняем выпадающий список
            dpv.tsbRightForward.DropDownItems.Clear();
            for (int i = RightCurrentPathIndex + 1; i < rightPathsVisited.Count; i++)
            {
                dpv.tsbRightForward.DropDownItems.Add(rightPathsVisited[i] as string);
            }
        }

        private void tsbRightRefresh_Click(object sender, EventArgs e)
        {
            dpv.wbRightBrowser.Refresh();
            FillDisksList(dpv.tsbRightDrives.DropDownItems);
        }

        private void tsbRightUp_Click(object sender, EventArgs e)
        {
            // Получаем родительский каталог текущего каталога
            DirectoryInfo parentDirectory = Directory.GetParent(RightCurrentPath);

            if (parentDirectory != null)
            {
                RightCurrentPath = parentDirectory.FullName;
            }
        }

        private void tsbRightView_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "Значки": RightBrowserViewMode = FolderViewMode.FVM_ICON;
                    break;

                case "Список": RightBrowserViewMode = FolderViewMode.FVM_LIST;
                    break;

                case "Таблица": RightBrowserViewMode = FolderViewMode.FVM_DETAILS;
                    break;
            }
        }

        private void tsbLeftDrives_ButtonClick(object sender, EventArgs e)
        {
            LeftCurrentPath = GetRootPathFromDriveCaption(dpv.tsbLeftDrives.Text);
        }

        private void tsbLeftDrives_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            dpv.tsbLeftDrives.Text = e.ClickedItem.Text;
            dpv.tsbLeftDrives.Image = e.ClickedItem.Image;

            LeftCurrentPath = Convert.ToString(e.ClickedItem.Tag);
        }

        private void tsbRightDrives_ButtonClick(object sender, EventArgs e)
        {
            RightCurrentPath = GetRootPathFromDriveCaption(dpv.tsbRightDrives.Text);
        }

        private void tsbRightDrives_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            dpv.tsbRightDrives.Text = e.ClickedItem.Text;
            dpv.tsbRightDrives.Image = e.ClickedItem.Image;

            RightCurrentPath = Convert.ToString(e.ClickedItem.Tag);
            
        }

        #endregion Обработчики событий кнопок файлового менеджера


        #region Обработчики событий тулбаров файлового менеджера

        /// <summary>
        /// Событие передвижения разделителя панелей файлового менеджера
        /// </summary>
        private void scBrowsers_SplitterMoved(object sender, SplitterEventArgs e)
        {
            ResizeBrowsersControls();
        }

        #endregion Обработчики событий тулбаров файлового менеджера


        #region Обработчики событий панелей файлового менеджера

        /// <summary>
        /// Обработчик события перехода браузера по указанному адресу
        /// </summary>
        private void wbLeftBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            LeftCurrentPath = e.Url.OriginalString;
            LeftBrowserViewMode = LeftBrowserViewMode;
        }

        /// <summary>
        /// Обработчик события перехода браузера по указанному адресу
        /// </summary>
        private void wbRightBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            RightCurrentPath = e.Url.OriginalString;
            RightBrowserViewMode = RightBrowserViewMode;
        }

        #endregion Обработчики событий панелей файлового менеджера

		#endregion Обработчики событий файлового менеджера


        #region Обработчики событий шедулера

        /// <summary>
        /// Обработчик выбора элемента в выпадающем списке
        /// </summary>
        private void uceSchedulePeriod_SelectionChanged(object sender, EventArgs e)
        {
            switch (dpv.uceSchedulePeriod.SelectedIndex)
            {
                case 0: dpv.utcSchedulePeriod.SelectedTab = dpv.utpcScheduleOnce.Tab;
                    break;

                case 1: dpv.utcSchedulePeriod.SelectedTab = dpv.utpcScheduleDaily.Tab;
                    break;

                case 2: dpv.utcSchedulePeriod.SelectedTab = dpv.utpcScheduleWeekly.Tab;
                    break;

                case 3: 
                    dpv.utcSchedulePeriod.SelectedTab = dpv.utpcScheduleMonthly.Tab;
                    rbMonthlyByDayNumbers_CheckedChanged(null, null);
                    rbMonthlyByWeekDays_CheckedChanged(null, null);
                    break;

                case 4:
                    dpv.utcSchedulePeriod.SelectedTab = dpv.utpcScheduleHour.Tab;
                    break;
            }

            if (!scheduleInitializing) scheduleChanged = true;
            //MakeSchedulerInfo();
        }

        /// <summary>
        /// Обработчик кнопки вызова формы выбора месяцев
        /// </summary>
        private void ubtnScheduleMonths_Click(object sender, EventArgs e)
        {
            try
            {
                if (scheduleMonths.ShowDialog() == DialogResult.OK)
                {
                    scheduleMonthsChanged = true;
                    //MakeSchedulerInfo();
                }
            }
            finally
            {
                scheduleMonths.Hide();
            }
        }

        /// <summary>
        /// Обработчик события показа формы выбора месяцев
        /// </summary>
        private void scheduleMonths_Shown(object sender, EventArgs e)
        {
            if (scheduleMonthsChanged) return;

            if (currentScheduleSettings.Periodicity != SchedulePeriodicity.Monthly)
            {
                scheduleMonths.uceAllMonths.Checked = true;
                scheduleMonths.SetMonths(true);
                return;
            }

            scheduleMonths.uceAllMonths.Checked = false;
            scheduleMonths.SetMonths(false);

            MonthlySchedule ms = currentScheduleSettings.Schedule as MonthlySchedule;

            if (ms != null)
            {
                for (int i = 0; i < ms.Months.Count; i++)
                {
                    switch (ms.Months[i])
                    {
                        case 0: scheduleMonths.uceJanuary.Checked = true;
                            break;

                        case 1: scheduleMonths.uceFebruary.Checked = true;
                            break;

                        case 2: scheduleMonths.uceMarch.Checked = true;
                            break;

                        case 3: scheduleMonths.uceApril.Checked = true;
                            break;

                        case 4: scheduleMonths.uceMay.Checked = true;
                            break;

                        case 5: scheduleMonths.uceJune.Checked = true;
                            break;

                        case 6: scheduleMonths.uceJuly.Checked = true;
                            break;

                        case 7: scheduleMonths.uceAugust.Checked = true;
                            break;

                        case 8: scheduleMonths.uceSeptember.Checked = true;
                            break;

                        case 9: scheduleMonths.uceOctober.Checked = true;
                            break;

                        case 10: scheduleMonths.uceNovember.Checked = true;
                            break;

                        case 11: scheduleMonths.uceDecember.Checked = true;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Обработчик события выбора ежемесячного расписания по номеру дня
        /// </summary>
        private void rbMonthlyByDayNumbers_CheckedChanged(object sender, EventArgs e)
        {
            if (dpv.rbMonthlyByDayNumbers.Checked)
            {
                dpv.nudMonthlyByDays.Enabled = true;
                dpv.uceWeekDay.Enabled = false;
                dpv.uceWeekNumber.Enabled = false;

                if (!scheduleInitializing) scheduleChanged = true;
                //MakeSchedulerInfo();
            }
        }

        /// <summary>
        /// Обработчик события выбора ежемесячного расписания по дню недели
        /// </summary>
        private void rbMonthlyByWeekDays_CheckedChanged(object sender, EventArgs e)
        {
            if (dpv.rbMonthlyByWeekDays.Checked)
            {
                dpv.nudMonthlyByDays.Enabled = false;
                dpv.uceWeekDay.Enabled = true;
                dpv.uceWeekNumber.Enabled = true;

                if (!scheduleInitializing) scheduleChanged = true;
                //MakeSchedulerInfo();
            }
        }

        /// <summary>
        /// Обработчик события изменения времени запуска закачки
        /// </summary>
        private void udteScheduleStartTime_ValueChanged(object sender, EventArgs e)
        {
            if (!scheduleInitializing) scheduleChanged = true;
            //MakeSchedulerInfo();
        }

        /// <summary>
        /// Обработчик события установки дня запуска закачки
        /// </summary>
        private void uceMonday_CheckedChanged(object sender, EventArgs e)
        {
            if (!scheduleInitializing) scheduleChanged = true;
            //MakeSchedulerInfo();
        }

        /// <summary>
        /// Обработчик события выбора для недели
        /// </summary>
        private void uceWeekDay_SelectionChanged(object sender, EventArgs e)
        {
            if (!scheduleInitializing) scheduleChanged = true;
            //MakeSchedulerInfo();
        }

        /// <summary>
        /// Обработчик события клика на кнопке "Принять расписание"
        /// </summary>
        private void ubtnApplySchedule_Click(object sender, EventArgs e)
        {
            scheduleEnabled = true;

            MakeSchedulerInfo();
            //SetSchedulerControlsEnabled();

            SaveScheduleSettings();
        }

        /// <summary>
        /// Обработчик события клика на кнопке "Отменить расписание"
        /// </summary>
        private void ubtnCancelSchedule_Click(object sender, EventArgs e)
        {
            scheduleEnabled = false;

            MakeSchedulerInfo();
            //SetSchedulerControlsEnabled();

            SaveScheduleSettings();
        }

        #endregion Обработчики событий шедулера


        #region Обработчики событий табконтролов

        /// <summary>
        /// Обработчик события смены стиля табконтрола
        /// </summary>
        private void utcViews_PropertyChanged(object sender, Infragistics.Win.PropertyChangedEventArgs e)
        {
            if (dpv.utcViews.Style != UltraTabControlStyle.Wizard)
            {
                dpv.utcViews.Style = UltraTabControlStyle.Wizard;
            }
        }

		/// <summary>
		/// Обработчик события переключения закладок табконтрола управления закачкой
		/// </summary>
		private void utcPumpControl_SelectedTabChanged(object sender, SelectedTabChangedEventArgs e)
		{
            SyncTabs(true);
		}

        /// <summary>
        /// Обработчик события переключения закладок табконтрола управления видами формы
        /// </summary>
        private void utcViews_SelectedTabChanged(object sender, SelectedTabChangedEventArgs e)
        {
            if (dpv.utcViews.SelectedTab == dpv.utpcUnknownPump.Tab)
            {
                Workplace.ViewObjectCaption = string.Empty;
            }
        }

		#endregion Обработчики событий табконтролов


		#region Обработчики событий закачки

        #region Обработка общих событий закачки

        /// <summary>
        /// Синхронизирует состояние контролов с состоянием закачки
        /// </summary>
        private void SyncControlsByPumpState(PumpProcessStates state)
        {
            if (state == PumpProcessStates.Finished)
            {
                SetAllButtonsForPumpState(state);
                //SetControlsFromStagesQueue();
            }

            // Если выполняется какой-либо из этапов закачки, запрещаем удаление данных
            if (state >= PumpProcessStates.PreviewData && state <= PumpProcessStates.CheckData)
            {
                SetEnableDeleteDataForGrids(!deletePumpEnabled);
                SetInitialImagesForQueueElement(pumpRegistryElement.StagesQueue[state]);
            }
        }

		/// <summary>
		/// Событие смены состояния процесса закачки
		/// </summary>
        public void dataPumpProgressHandling_PumpProcessStateChanged(
            PumpProcessStates prevState, PumpProcessStates currState)
		{
            //LogicalCallContextData serverContext = LogicalCallContextData.GetContext();

            try
            {
                //LogicalCallContextData.SetContext(clientContext);

                if (currState == PumpProcessStates.DeleteData)
                {
                    StartTimer();
                    pumpHistoryIsLoaded = false;
                    Workplace.OperationObj.StartOperation();
                }
                else if (prevState == PumpProcessStates.DeleteData && currState == PumpProcessStates.Finished)
                {
                    StopTimer();
                    Workplace.OperationObj.StopOperation();

                    FillHistoryDataSets();
                    SetPumpInfoText();
                    pumpHistoryIsLoaded = false;
                }
                else
                {
                    SyncControlsByPumpState(currState);
                }
            }
            catch
            {
                return;
            }
            finally
            {
                //LogicalCallContextData.SetContext(serverContext);

                if (currState == PumpProcessStates.DeleteData)
                {
                    //StartTimer();
                }
            }
		}

        /// <summary>
        /// Метод, вызываемый при генерировании события таймера
        /// </summary>
        private void OnTimerElapsed(object stateInfo)
        {
            try
            {
                if (dataPumpProgress.PumpIsAlive)
                {
                    if (dataPumpProgress.State == PumpProcessStates.DeleteData)
                    {
                        if (!String.IsNullOrEmpty(dataPumpProgress.ProgressMessage))
                        {
                            Workplace.OperationObj.Text = string.Format(
                                "{0} ({1})", dataPumpProgress.ProgressMessage, dataPumpProgress.ProgressText);
                        }
                        else
                        {
                            Workplace.OperationObj.Text = "Удаление данных...";
                        }
                    }
                    else if (dataPumpProgress.PumpInProgress)
                    {
                        // Установка прогрессов
                        SetProgressForPumpState(dataPumpProgress.State, dataPumpProgress.ProgressMaxPos,
                            dataPumpProgress.ProgressCurrentPos, dataPumpProgress.ProgressMessage,
                            dataPumpProgress.ProgressText);
                    }
                }
                else
                {
                    StopTimer();
                }
            }
            catch
            {
                return;
            }
        }

		/// <summary>
		/// Событие начала этапа
		/// </summary>
		public void dataPumpProgressHandling_StageStarted(PumpProcessStates state)
		{
			try
			{
                StartTimer();

				// Установка состояния кнопок, картинок и лабелов
				SetStageButtonsGroupEnabled(state, false, true, true, false);
                SetImagesForPumpCurrentState(state, Properties.Resources.Start);
				SetProgressVisibleForPumpState(state, true);
				SetMessageLabelsVisibleForPumpState(state, true);

				SetStartTimeLabelsForPumpState(state);
				//SetStartTimeLabelsVisibleForPumpState(state, true);

				pumpHistoryIsLoaded = false;

                //setCursorDelegate(Cursors.Default);
			}
			catch
			{
				return;
			}
		}

		/// <summary>
		/// Событие окончания этапа
		/// </summary>
		public void dataPumpProgressHandling_StageFinished(PumpProcessStates state)
		{
			try
			{
                StopTimer();

                if (!dataPumpProgress.PumpInProgress || !dataPumpProgress.PumpIsAlive)
                {
                    SetControlsFromStagesQueue();
                    return;
                }

                IStagesQueueElement sqe = pumpRegistryElement.StagesQueue[state];

                if (dataPumpProgress == null || sqe.StageInitialState == StageState.Skipped) return;

				// Установка состояния кнопок, картинок и лабелов
				SetStageButtonsGroupEnabled(state, false, false, false, false);
                if (sqe.StageCurrentState == StageState.FinishedWithErrors)
                {
                    SetImagesForPumpCurrentState(state, Properties.Resources.Cross2);
                }
                else if (sqe.StageCurrentState == StageState.SuccefullFinished)
                {
                    SetImagesForPumpCurrentState(state, Properties.Resources.Check3);
                }
                //SetCurrentImagesForQueueElement(sqe);

				SetProgressVisibleForPumpState(state, false);
				SetMessageLabelsVisibleForPumpState(state, false);
				SetEndTimeLabelsForPumpState(state);
				//SetEndTimeLabelsVisibleForPumpState(state, false);
                SetPumpInfoText();

				pumpHistoryIsLoaded = false;

                //PreparePreviewControls();
			}
			catch
			{
				return;
			}
		}

		/// <summary>
		/// Событие приостановки этапа
		/// </summary>
		public void dataPumpProgressHandling_StagePaused(PumpProcessStates state)
		{
			// Установка состояния кнопок, картинок и лабелов
			try
			{
				SetStageButtonsGroupEnabled(state, true, false, false, true);
			}
			catch
			{
				return;
			}
		}

		/// <summary>
		/// Событие возобновления этапа
		/// </summary>
		public void dataPumpProgressHandling_StageResumed(PumpProcessStates state)
		{
			// Установка состояния кнопок, картинок и лабелов
			try
			{
				SetStageButtonsGroupEnabled(state, false, true, true, true);
			}
			catch
			{
				return;
			}
		}

		/// <summary>
		/// Событие остановки этапа
		/// </summary>
		public void dataPumpProgressHandling_StageStopped(PumpProcessStates state)
		{
			try
			{
                //SetControlsFromStagesQueue();

				pumpHistoryIsLoaded = false;
			}
			catch
			{
				return;
			}
		}

		/// <summary>
		/// Событие пропуска этапа
		/// </summary>
		public void dataPumpProgressHandling_StageSkipped(PumpProcessStates state)
		{
			// Установка состояния кнопок, картинок и лабелов
			try
			{
                SetProgressVisibleForPumpState(state, false);
                SetMessageLabelsVisibleForPumpState(state, false);
                SetStageButtonsGroupEnabled(state, false, false, false, false);

                IStagesQueueElement sqe = pumpRegistryElement.StagesQueue[state];

                if (sqe.StageInitialState != StageState.Blocked && sqe.StageInitialState != StageState.Skipped)
                {
                    SetImagesForPumpCurrentState(state, Properties.Resources.Skipped);

                    if (sqe.IsExecuted)
                    {
                        SetEndTimeLabelsForPumpState(state);
                        //SetEndTimeLabelsVisibleForPumpState(state, true);
                    }
                }

				pumpHistoryIsLoaded = false;
			}
			catch
			{
				return;
			}
        }

        /// <summary>
        /// Событие, возникающее при критическом сбое закачки
        /// </summary>
        public void dataPumpProgressHandling_PumpFailure(string str)
		{
            //LogicalCallContextData serverContext = LogicalCallContextData.GetContext();

            try
            {
                StopTimer();
                
                //LogicalCallContextData.SetContext(clientContext);

                Workplace.ProgressObj.StopProgress();

                ShowErrorMessage(str);
                SetControlsFromStagesQueue();
            }
            catch
            {
                return;
            }
        }

        #endregion Обработка общих событий закачки


        #region Обработка событий расписания закачек

        /// <summary>
        /// Событие изменения расписания программы закачки
        /// </summary>
        /// <param name="str">ИД программы закачки</param>
        public void pumpSchedulerHandling_ScheduleIsChanged(string str)
        {
            return;
            /*
            if (dataPumpProgress == null || pumpRegistryElement.ProgramIdentifier != str ||
                dpv.utcPumpControl.SelectedTab != dpv.utpcSchedule.Tab || scheduleChangedWarning)
            {
                scheduleChangedWarning = false;
                return;
            }

            if (MessageBox.Show(
                "Расписание текущей закачки было изменено другим пользователем. Обновить данные расписания?",
                "Изменение расписания", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                LoadScheduleSettings();
            }*/
        }

        #endregion Обработка событий расписания закачек


        #region Обработка событий закачки текстовых файлов

        /// <summary>
        /// Устанавливает видимость и текст необходимых лабелов для предпросмотра
        /// </summary>
        private void SetControlsForPreview()
        {
            dpv.utpcPreviewData.Enabled = true;

            SetLabelVisible(dpv.ulPumpRuleMsg, true);
            SetLabelText(dpv.ulPumpRuleMsg, "Для продолжения закачки перейдите на закладку \"Предпросмотр\"");

            SetLabelText(dpv.ulPreviewMsg, 
                "Для продолжения закачки нажмите \"Принять\" (сохранить данные) или \"Отменить\" (не сохранять)");
        }

        /// <summary>
		/// Инициализирует контролы закладки предварительного просмотра
		/// </summary>
        public void PreparePreviewControls()
		{
            Workplace.OperationObj.Text = "Создание формы предварительного просмотра...";
            Workplace.OperationObj.StartOperation();

            try
            {
                textRepPump = (ITextRepPump)dataPumpProgress;
                if (textRepPump == null)
                {
                    throw new Exception("Ошибка при подключении к программе закачки текстовых отчетов.");
                }

                SetControlsForPreview();

                // Инициализация контролов формы
                dpv.dsReportData.Clear();
                textRepResultDataSet = textRepPump.ResultDataSet;
                fixedParams = textRepPump.FixedParameters;

                dpv.ugReportData.DataSource = textRepResultDataSet;

                InitBands();

                SetSelectedTab(dpv.utcPumpControl, dpv.utpcPreviewData);

                pumpRegistryElement.StagesQueue.Locked = true;
            }
            finally
            {
                Workplace.OperationObj.StopOperation();
            }
		}

        /// <summary>
        /// Устанавливает видимость и текст необходимых лабелов для предпросмотра
        /// </summary>
        private void ResetControlsForPreview()
        {
            SetLabelVisible(dpv.ulPumpRuleMsg, false);
            SetLabelText(dpv.ulPumpRuleMsg, string.Empty);

            SetLabelText(dpv.ulPreviewMsg, "Закладка используется только на этапе предварительного просмотра");

            dpv.utpcPreviewData.Enabled = false;
        }

        /// <summary>
        /// Обработчик события клика на кнопке "Принять" на закладке предпросмотра
        /// </summary>
        private void ubtnApplyPreview_Click(object sender, EventArgs e)
        {
            dpv.ugReportData.UpdateData();
            textRepPump.ResultDataSet = textRepResultDataSet;
            textRepPump.FixedParameters = fixedParams;

            ResetControlsForPreview();

            SetSelectedTab(dpv.utcPumpControl, dpv.utpcPumpRuling);

            pumpRegistryElement.StagesQueue.Locked = false;
        }

        /// <summary>
        /// Обработчик события клика на кнопке "Отменить" на закладке предпросмотра
        /// </summary>
        private void ubtnCancelPreview_Click(object sender, EventArgs e)
        {
            SetSelectedTab(dpv.utcPumpControl, dpv.utpcPumpRuling);

            ResetControlsForPreview();

            pumpRegistryElement.StagesQueue.Locked = false;
        }

        /// <summary>
        /// Настройка иерархического грида
        /// </summary>
        private void InitBands()
        {
            if (textRepResultDataSet != null && textRepResultDataSet.Tables.Count > 0)
            {
                UltraGridBand band = dpv.ugReportData.DisplayLayout.Bands[0];
                band.Override.AllowDelete = DefaultableBoolean.False;
                band.Override.AllowUpdate = DefaultableBoolean.False;
                band.Override.AllowAddNew = AllowAddNew.No;
                band.Columns[textRepPump.FileIndexFieldName].Hidden = true;
                band.Columns[textRepPump.TableIndexFieldName].Hidden = true;

                // Устанавливаем заголовки столбцов
                DataTable dt = textRepResultDataSet.Tables[0];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    DataColumn col = dt.Columns[i];
                    band.Columns[col.ColumnName].Header.Caption = col.Caption;
                }

                if (textRepResultDataSet.Tables.Count > 1)
                {
                    dt = textRepResultDataSet.Tables[1];
                    for (int j = 1; j < dpv.ugReportData.DisplayLayout.Bands.Count; j++)
                    {
                        // Скрываем столбец с индексами файлов отчетов
                        UltraGridBand childBand = dpv.ugReportData.DisplayLayout.Bands[j];
                        childBand.Override.AllowAddNew = AllowAddNew.No;
                        childBand.Columns[textRepPump.FileIndexFieldName].Hidden = true;
                        childBand.Columns[textRepPump.TableIndexFieldName].Hidden = true;

                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            DataColumn col = dt.Columns[i];
                            childBand.Columns[col.ColumnName].Header.Caption = col.Caption;
                            //childBand.ColHeaderLines = 2;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Обработчик активации строки грида с данными отчетов
        /// </summary>
        private void ugReportData_AfterRowActivate(object sender, EventArgs e)
        {
            if (dpv.ugReportData.ActiveRow.ChildBands != null)
            {
                fileIndex = dpv.ugReportData.ActiveRow.Index;
                FillFixedParamsGrid();
                FillFileTextBoxes();
            }
        }

        /// <summary>
        /// Обработчик события редактирования ячейки грида фиксированных параметров
        /// </summary>
        private void ugFixedParameters_CellChange(object sender, CellEventArgs e)
        {
            string fixParamName = dpv.ugFixedParameters.ActiveRow.Cells["Name"].Text;
            if (fixedParams.ContainsKey(fileIndex))
            {
                if (fixedParams[fileIndex].ContainsKey(fixParamName))
                {
                    FixedParameter fp = fixedParams[fileIndex][fixParamName];
                    fp.Value = e.Cell.Text;
                    fixedParams[fileIndex][fixParamName] = fp;
                }
            }
        }

        /// <summary>
        /// Загружает данные фиксированных параметров в грид формы предварительного просмотра
        /// </summary>
        private void FillFixedParamsGrid()
        {
            dpv.dsFixedParameters.Tables[0].Rows.Clear();

            if (textRepPump.FixedParameters.ContainsKey(fileIndex))
            {
                foreach (string key in textRepPump.FixedParameters[fileIndex].Keys)
                {
                    FixedParameter fp = textRepPump.FixedParameters[fileIndex][key];
                    if (fp.Caption != string.Empty)
                    {
                        dpv.dsFixedParameters.Tables[0].Rows.Add(fp.Caption, fp.Value);
                    }
                    else
                    {
                        dpv.dsFixedParameters.Tables[0].Rows.Add(key, fp.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Скрывает все закладки редакторов текстового отчета
        /// </summary>
        private void HideTextRepParts()
        {
            for (int i = 0; i < dpv.utcReportParts.Tabs.Count; i++)
            {
                dpv.utcReportParts.Tabs[i].Visible = false;
            }
        }

        /// <summary>
        /// Загружает файлы отчета в редактор
        /// </summary>
        private void FillFileTextBoxes()
        {
            Encoding en = Encoding.GetEncoding(866);
            HideTextRepParts();

            for (int i = 0; i < textRepPump.RepFilesLists.Count; i++)
            {
                // Загружаем файл в файлстрим
                FileInfo file = ConvertLocalPathToUNC(textRepPump.RepFilesLists[i][fileIndex]);
                if (!file.Exists) continue;

                FileStream fs = file.OpenRead();
                StreamReader sr;
                if (textRepPump.FilesCharacterSet == CharacterSet.ANSI) 
                    sr = new StreamReader(fs);
                else 
                    sr = new StreamReader(fs, en);
                
                // Содержимое стрима загружаем в редактор
                string[] strings = sr.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.None);

                switch (i)
                {
                    case 0: dpv.rtbPart1.Lines = strings;
                        break;

                    case 1: dpv.rtbPart2.Lines = strings;
                        break;

                    case 2: dpv.rtbPart3.Lines = strings;
                        break;
                }

                if (i < dpv.utcReportParts.Tabs.Count) dpv.utcReportParts.Tabs[i].Visible = true;
            }
        }

        /// <summary>
        /// Преобразует массив байтов в строку
        /// </summary>
        /// <param name="byteArray">Массив байтов</param>
        /// <returns>Строка</returns>
        public static string BytesArrayToString(byte[] byteArray)
        {
            string result = string.Empty;
            for (int i = 0; i < byteArray.GetLength(0); i++)
            {
                result += Convert.ToString((char)byteArray[i]);
            }
            return result;
        }

        /// <summary>
        /// Преобразует локальный путь файла текстовых отчетов в путь UNC (чтобы достать его с клиента)
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns>Файл с путем UNC</returns>
        private FileInfo ConvertLocalPathToUNC(FileInfo file) 
        {
            string dataSourcesPath = GetDataSourcesUNCPath();

            string[] oldPath = file.FullName.Split(new string[] { "\\" }, StringSplitOptions.None);
            string[] uncPath = dataSourcesPath.Split(new string[] { "\\" }, StringSplitOptions.None);
            string[] newPath = new string[uncPath.GetLength(0)];
            Array.Copy(uncPath, 0, newPath, 0, uncPath.GetLength(0));

            string sourceName = uncPath[uncPath.GetLength(0) - 1];
            bool fl = false;
            for (int i = 0; i < oldPath.GetLength(0); i++)
            {
                if (fl)
                {
                    Array.Resize(ref newPath, newPath.GetLength(0) + 1);
                    newPath[newPath.GetLength(0) - 1] = oldPath[i];
                }

                if (oldPath[i].ToUpper() == sourceName.ToUpper())
                {
                    fl = true;
                }
            }
            
            return new FileInfo(string.Join("\\", newPath).TrimEnd('\\').Trim());
        }

        #endregion Обработка событий закачки текстовых файлов

        #endregion Обработчики событий закачки

        #endregion Обработчики событий


        #region Свойства класса

        /// <summary>
        /// Синхронизирует состояние списка посещенных путей в зависимости от нового текущего пути
        /// </summary>
        /// <param name="pathsVisited">Список посещенных путей</param>
        /// <param name="currentPathIndex">Индекс текущего посещенного пути</param>
        /// <param name="path">Текущий путь</param>
        private static void SyncPathsVisited(ArrayList pathsVisited, ref int currentPathIndex, string path)
        {
            bool pathAlreadyVisited = false;

            // Ищем путь среди посещенных путей. Если он там есть, корректируем индекс пути среди посещенных
            for (int i = 0; i < pathsVisited.Count; i++)
            {
                if (string.Compare(path, pathsVisited[i] as string, true) == 0)
                {
                    currentPathIndex = i;
                    pathAlreadyVisited = true;
                }
            }

            // Если путь уже был посещен, то удалям историю путей после него
            if (pathAlreadyVisited == false)
            {
                for (int i = currentPathIndex + 1; i < pathsVisited.Count - 1; i++)
                {
                    pathsVisited.RemoveAt(i);
                }

                currentPathIndex = pathsVisited.Add(path);
            }
        }

        /// <summary>
        /// Текущий путь левой панели файлового менеджера
        /// </summary>
        private string LeftCurrentPath
        {
            get
            {
                if (LeftCurrentPathIndex < 0 || LeftCurrentPathIndex > (leftPathsVisited.Count - 1))
                {
                    return string.Empty;
                }

                return leftPathsVisited[LeftCurrentPathIndex] as string;
            }
            set
            {
                if (value != LeftCurrentPath)
                {
                    dpv.wbLeftBrowser.Navigate(value);
                    dpv.tstbLeftBrowserCaption.Text = value;
                    dpv.tstbLeftBrowserCaption.ToolTipText = value;

                    SyncPathsVisited(leftPathsVisited, ref leftCurrentPathIndex, value);
                    LeftCurrentPathIndex = leftCurrentPathIndex;
                }
            }
        }

        /// <summary>
        /// Текущий путь правой панели файлового менеджера
        /// </summary>
        private string RightCurrentPath
        {
            get
            {
                if (RightCurrentPathIndex < 0 || RightCurrentPathIndex > (rightPathsVisited.Count - 1))
                {
                    return string.Empty;
                }

                return rightPathsVisited[RightCurrentPathIndex] as string;
            }
            set
            {
                if (value != RightCurrentPath)
                {
                    dpv.wbRightBrowser.Navigate(value);
                    dpv.tstbRightBrowserCaption.Text = value;
                    dpv.tstbRightBrowserCaption.ToolTipText = value;

                    SyncPathsVisited(rightPathsVisited, ref rightCurrentPathIndex, value);
                    RightCurrentPathIndex = rightCurrentPathIndex;
                }
            }
        }

        /// <summary>
        /// Индекс текущего пути в списке посещенных путей левой панели
        /// </summary>
        private int LeftCurrentPathIndex
        {
            get
            { 
                return leftCurrentPathIndex; 
            }
            set
            {
                leftCurrentPathIndex = value;

                // Устанавливаем состояние кнопок тулбара
                SetFileManagerLeftToolbarStatus();
            }
        }

        /// <summary>
        /// Индекс текущего пути в списке посещенных путей левой панели
        /// </summary>
        private int RightCurrentPathIndex
        {
            get
            {
                return rightCurrentPathIndex;
            }
            set
            {
                rightCurrentPathIndex = value;

                // Устанавливаем состояние кнопок тулбара
                SetFileManagerRightToolbarStatus();
            }
        }

        /// <summary>
        /// Тип представления каталога в левом браузере
        /// </summary>
        private FolderViewMode LeftBrowserViewMode
        {
            get
            {
                return leftBrowserViewMode;
            }
            set
            {
                leftBrowserViewMode = value;
                FileManager.SetFolderView(dpv.wbLeftBrowser.Handle, leftBrowserViewMode);
            }
        }

        /// <summary>
        /// Тип представления каталога в правом браузере
        /// </summary>
        private FolderViewMode RightBrowserViewMode
        {
            get
            {
                return rightBrowserViewMode;
            }
            set
            {
                rightBrowserViewMode = value;
                FileManager.SetFolderView(dpv.wbRightBrowser.Handle, rightBrowserViewMode);
            }
        }

        #endregion Свойства класса
    }
}
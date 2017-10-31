using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

using Krista.FM.Common;
using Krista.FM.Common.Handling;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumpManagement
{
    /// <summary>
    /// Информация о ходе закачки
    /// </summary>
    public sealed class DataPumpProgress : DisposableObject, IDataPumpProgress, IServerSideDataPumpProgress
    {
        #region Поля

        private int progressMaxPos = 0;
        private int progressCurrentPos = 0;
        private string progressMessage = string.Empty;
        private string progressText = string.Empty;
        private PumpProcessStates currentState = PumpProcessStates.Prepared;

        #endregion Поля


        #region Инициализация

        /// <summary>
        /// Конструктор
        /// </summary>
        public DataPumpProgress()
        {
        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Инициализация полей
        /// </summary>
        public void Initialize()
        {

        }

        #endregion Инициализация


        #region Реализация IDataPumpProgress

        #region Реализация методов

        /// <summary>
        /// Обновить данные о состоянии закачки
        /// </summary>
        public void Refresh()
        {
            if (!this.PumpIsAlive)
            {
                this.State = PumpProcessStates.Finished;
                this.ProgressCurrentPos = 0;
                this.ProgressMaxPos = 0;
                this.ProgressMessage = string.Empty;
                this.ProgressText = string.Empty;
            }
        }

        #endregion Реализация методов


        #region Реализация свойств

        /// <summary>
        /// Состояние процесса закачка 
        /// </summary>
        public PumpProcessStates State
        {
            get
            {
                return currentState;
            }
            set
            {
                EventsProcessing.OnGetPumpStateDelegateEvent(ref this.SetState, value);
                currentState = value;
            }
        }

        /// <summary>
        /// Максимальное значение прогресса
        /// </summary>
        public int ProgressMaxPos
        {
            get
            {
                return progressMaxPos;
            }
            set
            {
                progressMaxPos = value;
            }
        }

        /// <summary>
        /// Текущее значение прогресса
        /// </summary>
        public int ProgressCurrentPos
        {
            get
            {
                return progressCurrentPos;
            }
            set
            {
                progressCurrentPos = value;
            }
        }

        /// <summary>
        /// Сообщение прогресса
        /// </summary>
        public string ProgressMessage
        {
            get
            {
                return progressMessage;
            }
            set
            {
                progressMessage = value;
            }
        }

        /// <summary>
        /// Текст прогресса, который будет писаться на нем самом
        /// </summary>
        public string ProgressText
        {
            get
            {
                return progressText;
            }
            set
            {
                progressText = value;
            }
        }

        /// <summary>
        /// Признак, закачка жива или нет
        /// </summary>
        public bool PumpIsAlive
        {
            get
            {
                return EventsProcessing.OnGetBoolDelegateEvent(ref this.GetPumpLiveStatus);
            }
        }

        /// <summary>
        /// Признак выполнения закачки
        /// </summary>
        public bool PumpInProgress
        {
            get
            {
                return this.State >= PumpProcessStates.PreviewData &&
                    this.State <= PumpProcessStates.CheckData;
            }
        }

        #endregion Реализация свойств


        #region Реализация событий

        /// <summary>
        /// Событие смены состояния процесса закачки
        /// </summary>
        public event PumpProcessStateChangedDelegate PumpProcessStateChanged;

        /// <summary>
        /// Событие начала этапа
        /// </summary>
        public event GetPumpStateDelegate StageStarted;

        /// <summary>
        /// Событие окончания этапа
        /// </summary>
        public event GetPumpStateDelegate StageFinished;

        /// <summary>
        /// Событие приостановки этапа
        /// </summary>
        public event GetPumpStateDelegate StagePaused;

        /// <summary>
        /// Событие возобновления этапа
        /// </summary>
        public event GetPumpStateDelegate StageResumed;

        /// <summary>
        /// Событие остановки этапа
        /// </summary>
        public event GetPumpStateDelegate StageStopped;

        /// <summary>
        /// Событие пропуска этапа
        /// </summary>
        public event GetPumpStateDelegate StageSkipped;

        /// <summary>
        /// Событие, возникающее при критическом сбое закачки
        /// </summary>
        public event GetStringDelegate PumpFailure;

        #endregion Реализация событий

        #endregion Реализация IDataPumpProgress


        #region Реализация IServerSideDataPumpProgress

        #region Реализация методов

        /// <summary>
        /// Событие смены состояния процесса закачки
        /// </summary>
        public void OnPumpProcessStateChanged(PumpProcessStates prevState, PumpProcessStates currState)
        {
            currentState = currState;
            EventsProcessing.OnPumpProcessStateChangedDelegateEvent(
                ref this.PumpProcessStateChanged, prevState, currState);
        }

        /// <summary>
        /// Событие начала этапа
        /// </summary>
        public void OnStageStarted(PumpProcessStates state)
        {
            EventsProcessing.OnGetPumpStateDelegateEvent(ref this.StageStarted, state);
        }

        /// <summary>
        /// Событие окончания этапа
        /// </summary>
        public void OnStageFinished(PumpProcessStates state)
        {
            EventsProcessing.OnGetPumpStateDelegateEvent(ref this.StageFinished, state);
        }

        /// <summary>
        /// Событие приостановки этапа
        /// </summary>
        public void OnStagePaused(PumpProcessStates state)
        {
            EventsProcessing.OnGetPumpStateDelegateEvent(ref this.StagePaused, state);
        }

        /// <summary>
        /// Событие возобновления этапа
        /// </summary>
        public void OnStageResumed(PumpProcessStates state)
        {
            EventsProcessing.OnGetPumpStateDelegateEvent(ref this.StageResumed, state);
        }

        /// <summary>
        /// Событие остановки этапа
        /// </summary>
        public void OnStageStopped(PumpProcessStates state)
        {
            EventsProcessing.OnGetPumpStateDelegateEvent(ref this.StageStopped, state);
        }

        /// <summary>
        /// Событие пропуска этапа
        /// </summary>
        public void OnStageSkipped(PumpProcessStates state)
        {
            EventsProcessing.OnGetPumpStateDelegateEvent(ref this.StageSkipped, state);
        }

        /// <summary>
        /// Событие, возникающее при критическом сбое закачки
        /// </summary>
        public void OnPumpFailure(string str)
        {
            EventsProcessing.OnGetStringDelegateEvent(ref this.PumpFailure, str);
        }

        #endregion Реализация методов


        #region Реализация событий

        /// <summary>
        /// Событие смены состояния закачки
        /// </summary>
        public event GetPumpStateDelegate SetState;

        /// <summary>
        /// Событие получение признака того, что закачка до сих пор работает, а не отвалилась нафиг
        /// </summary>
        public event GetBoolDelegate GetPumpLiveStatus;

        #endregion Реализация событий

        #endregion Реализация IServerSideDataPumpProgress
    }
}
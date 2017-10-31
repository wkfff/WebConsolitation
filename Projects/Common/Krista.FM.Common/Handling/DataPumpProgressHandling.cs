using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Common.Handling
{
    /// <summary>
    /// Класс для перенаправления событий закачки сервера клиенту
    /// </summary>
    public class DataPumpProgressHandling : DisposableObject
    {
        #region Поля

        private IDataPumpProgress dataPumpProgress;

        #endregion Поля


        #region Инициализация

        /// <summary>
        /// Деструктор
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.dataPumpProgress != null)
            {
                dataPumpProgress.PumpProcessStateChanged -=
                    new PumpProcessStateChangedDelegate(dataPumpProgress_PumpProcessStateChanged);
                dataPumpProgress.StageFinished -= new GetPumpStateDelegate(dataPumpProgress_StageFinished);
                dataPumpProgress.StagePaused -= new GetPumpStateDelegate(dataPumpProgress_StagePaused);
                dataPumpProgress.StageSkipped -= new GetPumpStateDelegate(dataPumpProgress_StageSkipped);
                dataPumpProgress.StageStarted -= new GetPumpStateDelegate(dataPumpProgress_StageStarted);
                dataPumpProgress.StageStopped -= new GetPumpStateDelegate(dataPumpProgress_StageStopped);
                dataPumpProgress.StageResumed -= new GetPumpStateDelegate(dataPumpProgress_StageResumed);
            }

            base.Dispose(disposing);
        }

        #endregion Инициализация


        #region Реализация свойств

        /// <summary>
        /// Модуль закачки
        /// </summary>
        public IDataPumpProgress DataPumpProgress
        {
            get
            {
                return dataPumpProgress;
            }
            set
            {
                dataPumpProgress = value;
                dataPumpProgress.PumpProcessStateChanged +=
                    new PumpProcessStateChangedDelegate(dataPumpProgress_PumpProcessStateChanged);
                dataPumpProgress.StageFinished += new GetPumpStateDelegate(dataPumpProgress_StageFinished);
                dataPumpProgress.StagePaused += new GetPumpStateDelegate(dataPumpProgress_StagePaused);
                dataPumpProgress.StageSkipped += new GetPumpStateDelegate(dataPumpProgress_StageSkipped);
                dataPumpProgress.StageStarted += new GetPumpStateDelegate(dataPumpProgress_StageStarted);
                dataPumpProgress.StageStopped += new GetPumpStateDelegate(dataPumpProgress_StageStopped);
                dataPumpProgress.StageResumed += new GetPumpStateDelegate(dataPumpProgress_StageResumed);
                dataPumpProgress.PumpFailure += new GetStringDelegate(dataPumpProgress_PumpFailure);
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


        #region Обработчики событий закачки

        /// <summary>
        /// Событие смены состояния процесса закачки
        /// </summary>
        public void dataPumpProgress_PumpProcessStateChanged(PumpProcessStates prevState, PumpProcessStates currState)
        {
            EventsProcessing.OnPumpProcessStateChangedDelegateEvent(
                ref this.PumpProcessStateChanged, prevState, currState);
        }

        /// <summary>
        /// Событие начала этапа
        /// </summary>
        public void dataPumpProgress_StageStarted(PumpProcessStates state)
        {
            EventsProcessing.OnGetPumpStateDelegateEvent(ref this.StageStarted, state);
        }

        /// <summary>
        /// Событие окончания этапа
        /// </summary>
        public void dataPumpProgress_StageFinished(PumpProcessStates state)
        {
            EventsProcessing.OnGetPumpStateDelegateEvent(ref this.StageFinished, state);
        }

        /// <summary>
        /// Событие приостановки этапа
        /// </summary>
        public void dataPumpProgress_StagePaused(PumpProcessStates state)
        {
            EventsProcessing.OnGetPumpStateDelegateEvent(ref this.StagePaused, state);
        }

        /// <summary>
        /// Событие возобновления этапа
        /// </summary>
        public void dataPumpProgress_StageResumed(PumpProcessStates state)
        {
            EventsProcessing.OnGetPumpStateDelegateEvent(ref this.StageResumed, state);
        }

        /// <summary>
        /// Событие остановки этапа
        /// </summary>
        public void dataPumpProgress_StageStopped(PumpProcessStates state)
        {
            EventsProcessing.OnGetPumpStateDelegateEvent(ref this.StageStopped, state);
        }

        /// <summary>
        /// Событие пропуска этапа
        /// </summary>
        public void dataPumpProgress_StageSkipped(PumpProcessStates state)
        {
            EventsProcessing.OnGetPumpStateDelegateEvent(ref this.StageSkipped, state);
        }

        /// <summary>
        /// Событие, возникающее при критическом сбое закачки
        /// </summary>
        public void dataPumpProgress_PumpFailure(string str)
        {
            EventsProcessing.OnGetStringDelegateEvent(ref this.PumpFailure, str);
        }

        #endregion Обработчики событий закачки
    }
}

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
    public class ServerSideDataPumpProgressHandling : DisposableObject
    {
        #region Поля

        private IServerSideDataPumpProgress serverSideDataPumpProgress;

        #endregion Поля


        #region Инициализация

        /// <summary>
        /// Деструктор
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.serverSideDataPumpProgress != null)
            {
                serverSideDataPumpProgress.SetState -= new GetPumpStateDelegate(dataPumpProgress_SetState);
                serverSideDataPumpProgress.GetPumpLiveStatus -= new GetBoolDelegate(serverSideDataPumpProgress_GetPumpLiveStatus);
            }

            base.Dispose(disposing);
        }

        #endregion Инициализация


        #region Реализация свойств

        /// <summary>
        /// Модуль закачки
        /// </summary>
        public IServerSideDataPumpProgress ServerSideDataPumpProgress
        {
            get
            {
                return serverSideDataPumpProgress;
            }
            set
            {
                serverSideDataPumpProgress = value;
                serverSideDataPumpProgress.SetState += new GetPumpStateDelegate(dataPumpProgress_SetState);
                serverSideDataPumpProgress.GetPumpLiveStatus += new GetBoolDelegate(serverSideDataPumpProgress_GetPumpLiveStatus);
            }
        }

        #endregion Реализация свойств


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


        #region Обработчики событий закачки

        /// <summary>
        /// Событие остановки этапа
        /// </summary>
        public void dataPumpProgress_SetState(PumpProcessStates state)
        {
            EventsProcessing.OnGetPumpStateDelegateEvent(ref this.SetState, state);
        }

        /// <summary>
        /// Событие получение признака того, что закачка до сих пор работает, а не отвалилась нафиг
        /// </summary>
        public bool serverSideDataPumpProgress_GetPumpLiveStatus()
        {
            return EventsProcessing.OnGetBoolDelegateEvent(ref this.GetPumpLiveStatus);
        }

        #endregion Обработчики событий закачки
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Common.Handling
{
    /// <summary>
    /// Класс для перенаправления событий шедулера закачки сервера клиенту
    /// </summary>
    public sealed class PumpSchedulerHandling : DisposableObject
    {
        #region Поля

        private IPumpScheduler pumpScheduler;

        #endregion Поля


        #region Инициализация

        /// <summary>
        /// Деструктор
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.pumpScheduler != null)
            {
                pumpScheduler.ScheduleIsChanged -= new GetStringDelegate(pumpScheduler_ScheduleIsChanged);
            }

            base.Dispose(disposing);
        }

        #endregion Инициализация


        #region Реализация свойств

        /// <summary>
        /// Пул закачки
        /// </summary>
        public IPumpScheduler PumpScheduler
        {
            get
            {
                return pumpScheduler;
            }
            set
            {
                pumpScheduler = null;
                pumpScheduler = value;
                pumpScheduler.ScheduleIsChanged += new GetStringDelegate(pumpScheduler_ScheduleIsChanged);
            }
        }

        #endregion Реализация свойств


        #region Реализация событий

        /// <summary>
        /// Событие изменения настроек расписания
        /// </summary>
        public event GetStringDelegate ScheduleIsChanged;

        /// <summary>
        /// Метод, вызываемый при генерировании события
        /// </summary>
        private void OnGetStringDelegateEvent(ref GetStringDelegate evt, string progID)
        {
            // Все обработчики события нужно вызывать вручную для обнаружения отключившихся клиентов
            if (evt != null)
            {
                Delegate[] delegate_list = evt.GetInvocationList();
                foreach (Delegate dlg in delegate_list)
                {
                    GetStringDelegate handler = (GetStringDelegate)dlg;
                    try
                    {
                        handler.Invoke(progID);
                    }
                    catch
                    {
                        evt -= handler;
                    }
                }
            }
        }

        #endregion Реализация событий


        #region Обработчики событий пула закачки

        /// <summary>
        /// Событие изменения настроек расписания
        /// </summary>
        /// <param name="str">ИД программы закачки</param>
        public void pumpScheduler_ScheduleIsChanged(string str)
        {
            this.OnGetStringDelegateEvent(ref this.ScheduleIsChanged, str);
        }

        #endregion Обработчики событий пула закачки
    }
}

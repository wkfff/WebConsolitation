using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumpManagement
{
    /// <summary>
    /// Очередь этапов закачки
    /// </summary>
    public sealed class StagesQueue : DisposableObject, IStagesQueue
    {
        #region Поля

        private Dictionary<PumpProcessStates, StagesQueueElement> stagesQueueElements;
        private PumpRegistryElement pumpRegistryElement = null;
        private IScheme scheme;

        #endregion Поля


        #region Инициализация

        /// <summary>
        /// Конструктор
        /// </summary>
        public StagesQueue(IScheme scheme, IPumpRegistryElement pumpRegistryElement)
        {
            this.pumpRegistryElement = (PumpRegistryElement)pumpRegistryElement;
            this.scheme = scheme;

            LoadStagesQueue();
        }

        /// <summary>
        /// Деструктор
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (stagesQueueElements != null) stagesQueueElements.Clear();
            }
        }

        #endregion Инициализация


        #region Общие функции

        /// <summary>
        /// Устанавливает очередь этапов в начальное состояние
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void ResetStagesQueue()
        {
            if (stagesQueueElements == null)
            {
                stagesQueueElements = new Dictionary<PumpProcessStates, StagesQueueElement>(6);
            }

            stagesQueueElements.Clear();

            stagesQueueElements.Add(PumpProcessStates.PreviewData,
                new StagesQueueElement(this.scheme, PumpProcessStates.PreviewData, pumpRegistryElement, this));
            stagesQueueElements.Add(PumpProcessStates.PumpData,
                new StagesQueueElement(this.scheme, PumpProcessStates.PumpData, pumpRegistryElement, this));
            stagesQueueElements.Add(PumpProcessStates.ProcessData,
                new StagesQueueElement(this.scheme, PumpProcessStates.ProcessData, pumpRegistryElement, this));
            stagesQueueElements.Add(PumpProcessStates.AssociateData,
                new StagesQueueElement(this.scheme, PumpProcessStates.AssociateData, pumpRegistryElement, this));
            stagesQueueElements.Add(PumpProcessStates.ProcessCube,
                new StagesQueueElement(this.scheme, PumpProcessStates.ProcessCube, pumpRegistryElement, this));
            stagesQueueElements.Add(PumpProcessStates.CheckData,
                new StagesQueueElement(this.scheme, PumpProcessStates.CheckData, pumpRegistryElement, this));
        }

        /// <summary>
        /// Загружает настройки очереди из базы
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LoadStagesQueue()
        {
            ResetStagesQueue();

            foreach (KeyValuePair<PumpProcessStates, StagesQueueElement> kvp in stagesQueueElements)
            {
                kvp.Value.LoadFromXML();
            }
        }

        /// <summary>
        /// Сохраняет состояние очереди в базу
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SaveStagesQueue()
        {
            foreach (KeyValuePair<PumpProcessStates, StagesQueueElement> kvp in stagesQueueElements)
            {
                kvp.Value.SaveToXML();
            }
        }

        #endregion Общие функции


        #region Свойства класса

        /// <summary>
        /// Список элементов очереди
        /// </summary>
        public Dictionary<PumpProcessStates, StagesQueueElement> StagesQueueElements
        {
            get
            {
                return stagesQueueElements;
            }
        }

        #endregion Свойства класса


        #region Реализация IStagesQueue

        #region Реализация методов

        /// <summary>
        /// Очищает данные о выполнении этапов
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ClearExecutingInformation()
        {
            foreach (KeyValuePair<PumpProcessStates, StagesQueueElement> kvp in stagesQueueElements)
            {
                kvp.Value.ClearExecutingInformation();
            }

            LoadStagesQueue();
        }

        #endregion Реализация методов


        #region Реализация свойств

        /// <summary>
        /// Возвращает класс состояния указанного этапа закачки
        /// </summary>
        /// <param name="state">Этап закачки</param>
        /// <returns>Состояние этапа</returns>
        public IStagesQueueElement this[PumpProcessStates state]
        {
            get
            {
                if (!stagesQueueElements.ContainsKey(state))
                {
                    return null;
                }
                else
                {
                    stagesQueueElements[state].LoadFromXML();
                    return stagesQueueElements[state] as IStagesQueueElement;
                }
            }
            set
            {
                if (stagesQueueElements.ContainsKey(state))
                {
                    stagesQueueElements[state] = (StagesQueueElement)value;
                    stagesQueueElements[state].SaveToXML();
                }
            }
        }

        /// <summary>
        /// Блокировка очереди.
        /// true - текущий этап будет закончен, далее закачка будет продолжена только по установке false
        /// </summary>
        public bool Locked
        {
            get
            {
                return pumpRegistryElement.DataPumpModule.Locked;
            }
            set
            {
                pumpRegistryElement.DataPumpModule.Locked = value;
                if (!value)
                {
                    IStagesQueueElement nextElement = GetNextExecutableQueueElement();
                    if (nextElement == null)
                    {
                        pumpRegistryElement.DataPumpModule.State = PumpProcessStates.Finished;
                    }
                    else
                    {
                        pumpRegistryElement.DataPumpModule.State = nextElement.State;
                    }
                }
            }
        }

        #endregion Реализация свойств


        #region Реализация методов

        /// <summary>
        /// Возвращает выполняющийся этап
        /// </summary>
        /// <returns></returns>
        public IStagesQueueElement GetInProgressQueueElement()
        {
            IStagesQueueElement result = null;

            for (int i = stagesQueueElements.Count; i > 0; i--)
            {
                PumpProcessStates st = (PumpProcessStates)i;
                if (stagesQueueElements[st].StageCurrentState == StageState.InProgress)
                {
                    result = (IStagesQueueElement)stagesQueueElements[st];
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Возвращет последний выполненный этап закачки
        /// </summary>
        public IStagesQueueElement GetLastExecutedQueueElement()
        {
            IStagesQueueElement result = null;

            for (int i = stagesQueueElements.Count; i > 0; i--)
            {
                PumpProcessStates st = (PumpProcessStates)i;
                if (stagesQueueElements[st].IsExecuted)
                {
                    result = (IStagesQueueElement)stagesQueueElements[st];
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Возвращет следующий выполняемый этап закачки
        /// </summary>
        public IStagesQueueElement GetNextExecutableQueueElement()
        {
            IStagesQueueElement result = null;

            IStagesQueueElement lastExecuted = GetLastExecutedQueueElement();
            if (lastExecuted == null)
            {
                lastExecuted = this[PumpProcessStates.PumpData];
            }

            for (int i = (int)lastExecuted.State; i <= stagesQueueElements.Count; i++)
            {
                PumpProcessStates st = (PumpProcessStates)i;

                if (stagesQueueElements[st].StageInitialState == StageState.InQueue)
                {
                    result = (IStagesQueueElement)stagesQueueElements[st];
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Проверка на присутствие указанного этапа в очереди закачки
        /// </summary>
        /// <param name="state">Этап</param>
        /// <returns>Присутствует или нет</returns>
        public bool ContainsStage(PumpProcessStates state)
        {
            return stagesQueueElements.ContainsKey(state);
        }

        #endregion Реализация методов

        #endregion Реализация IStagesQueue
    }
}
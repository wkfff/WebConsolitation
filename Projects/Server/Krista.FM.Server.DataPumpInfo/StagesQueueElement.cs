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
    /// Элемент очереди закачки. Нужен для управления последовательностью выполнения этапов и хранения информации
    /// о выполненных этапах.
    /// </summary>
    public sealed class StagesQueueElement : DisposableObject, IStagesQueueElement
    {
        #region Поля

        private PumpProcessStates state;
        private bool isExecuted;
        private DateTime startTime;
        private DateTime endTime;
        private StageState stageInitialState;
        private StageState stageCurrentState;
        private PumpRegistryElement pumpRegistryElement = null;
        private string stageComment;
        private IStagesQueue stagesQueue;
        private IScheme scheme;

        #endregion Поля


        #region Константы

        private const string nodeDataPumpStagesParams = "DataPumpStagesParams";
        private const string attrIsExecuted = "IsExecuted";
        private const string attrStartTime = "StartTime";
        private const string attrEndTime = "EndTime";
        private const string attrStageInitialState = "StageInitialState";
        private const string attrStageCurrentState = "StageCurrentState";
        private const string attrComment = "Comment";

        #endregion Константы


        #region Инициализация

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="state">Состояние закачки</param>
        public StagesQueueElement(IScheme scheme, PumpProcessStates state, IPumpRegistryElement pumpRegistryElement, 
            IStagesQueue stagesQueue)
        {
            this.state = state;
            this.isExecuted = false;
            this.startTime = DateTime.MinValue;
            this.endTime = DateTime.MinValue;
            this.stageInitialState = StageState.Skipped;
            this.stageCurrentState = StageState.OutOfQueue;
            this.pumpRegistryElement = (PumpRegistryElement)pumpRegistryElement;
            this.stagesQueue = stagesQueue;
            this.scheme = scheme;
        }

        #endregion Инициализация


        #region Общие методы

        /// <summary>
        /// Загружает состояние элемента очереди из хмл
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LoadFromXML()
        {
            string stagesParams = pumpRegistryElement.StagesParameters;

            if (stagesParams != string.Empty)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(stagesParams);

                XmlNode xn = xmlDoc.SelectSingleNode(string.Format("//{0}", this.state));

                string initialStateValue = XmlHelper.GetStringAttrValue(xn, attrStageInitialState, "Blocked");
                this.stageInitialState = this.scheme.DataPumpManager.DataPumpInfo.StringToStageState(initialStateValue);
                this.stageCurrentState = this.scheme.DataPumpManager.DataPumpInfo.StringToStageState(
                    XmlHelper.GetStringAttrValue(xn, attrStageCurrentState, initialStateValue));
                this.stageComment = XmlHelper.GetStringAttrValue(xn, attrComment, "Никаких действий не выполняется");

                // Для заблокированных этапов нет смысла загружать данные об их выполнении
                if (this.stageInitialState != StageState.Blocked)
                {
                    this.isExecuted = XmlHelper.GetBoolAttrValue(xn, attrIsExecuted, false);
                    DateTime.TryParse(XmlHelper.GetStringAttrValue(
                        xn, attrStartTime, Convert.ToString(DateTime.MinValue)), out this.startTime);
                    DateTime.TryParse(XmlHelper.GetStringAttrValue(
                        xn, attrEndTime, Convert.ToString(DateTime.MinValue)), out this.endTime);
                }
            }
        }

        /// <summary>
        /// Сохраняет состояние элемента очереди в хмл
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SaveToXML()
        {
            // Для заблокированных этапов нет смысла сохранять данные об их выполнении
            if (this.StageInitialState == StageState.Blocked) return;

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(pumpRegistryElement.StagesParameters);

            XmlNode root = xd.SelectSingleNode(nodeDataPumpStagesParams);
            if (root == null)
            {
                root = xd.CreateElement(nodeDataPumpStagesParams);
                xd.AppendChild(root);
            }

            string stateStr = Convert.ToString(this.state);
            // Если элемента этапа нет - создаем
            XmlNode xn = root.SelectSingleNode(stateStr);
            if (xn == null)
            {
                xn = xd.CreateElement(stateStr);
                root.AppendChild(xn);
            }

            XmlHelper.SetAttribute(xn, attrStageInitialState, Convert.ToString(this.StageInitialState));
            XmlHelper.SetAttribute(xn, attrStageCurrentState, Convert.ToString(this.StageCurrentState));
            XmlHelper.SetAttribute(xn, attrComment, this.stageComment);
            XmlHelper.SetAttribute(xn, attrEndTime, Convert.ToString(this.EndTime));
            XmlHelper.SetAttribute(xn, attrIsExecuted, Convert.ToString(this.IsExecuted));
            XmlHelper.SetAttribute(xn, attrStartTime, Convert.ToString(this.StartTime));

            pumpRegistryElement.StagesParameters = xd.InnerXml;
        }

        /// <summary>
        /// Очищает данные о выполнении этапа
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ClearExecutingInformation()
        {
            string stagesParams = pumpRegistryElement.StagesParameters;

            if (stagesParams != string.Empty)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(stagesParams);

                XmlNode xn = xmlDoc.SelectSingleNode(string.Format("//{0}", this.state));

                // Для заблокированных этапов нет смысла загружать данные об их выполнении
                if (this.stageInitialState != StageState.Blocked)
                {
                    XmlHelper.RemoveAttribute(xn, attrStageCurrentState);
                    XmlHelper.RemoveAttribute(xn, attrIsExecuted);
                    XmlHelper.RemoveAttribute(xn, attrStartTime);
                    XmlHelper.RemoveAttribute(xn, attrEndTime);
                }

                pumpRegistryElement.StagesParameters = xmlDoc.InnerXml;
            }
        }

        #endregion Общие методы


        #region Реализация IStagesQueueElement

        #region Свойства

        /// <summary>
        /// Этап
        /// </summary>
        public PumpProcessStates State
        {
            get
            {
                return this.state;
            }
            set
            {
                this.state = value;
            }
        }

        /// </summary>
        /// Признак, что этап был выполнен
        /// </summary>
        public bool IsExecuted
        {
            get
            {
                return this.isExecuted;
            }
            set
            {
                if (this.stageInitialState != StageState.Blocked)
                {
                    this.isExecuted = value;
                    SaveToXML();
                }
            }
        }

        /// <summary>
        /// Время начала выполнения
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return this.startTime;
            }
            set
            {
                if (this.stageInitialState != StageState.Blocked)
                {
                    this.startTime = value;
                    SaveToXML();
                }
            }
        }

        /// <summary>
        /// Время окончания выполнения
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return this.endTime;
            }
            set
            {
                if (this.stageInitialState != StageState.Blocked)
                {
                    this.endTime = value;
                    SaveToXML();
                }
            }
        }

        /// <summary>
        /// Состояние этапа
        /// </summary>
        public StageState StageInitialState
        {
            get
            {
                return this.stageInitialState;
            }
            set
            {
                if (this.stageInitialState != StageState.Blocked)
                {
                    this.stageInitialState = value;
                    SaveToXML();
                }
            }
        }

        /// <summary>
        /// Состояние этапа
        /// </summary>
        public StageState StageCurrentState
        {
            get
            {
                return this.stageCurrentState;
            }
            set
            {
                if (this.stageInitialState != StageState.Blocked)
                {
                    this.stageCurrentState = value;
                    SaveToXML();
                }
            }
        }

        /// <summary>
        /// Комментарий к этапу
        /// </summary>
        public string Comment
        {
            get
            {
                return this.stageComment;
            }
            set
            {
                this.stageComment = value;
                SaveToXML();
            }
        }

        #endregion Свойства


        #region Методы

        /// <summary>
        /// Устанавливает начальное значение состояния этапа (с сохранением в хмл в базу)
        /// </summary>
        /// <param name="ss">Состояние (InQueue или Skipped)</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetInitialStageState(StageState ss)
        {
            if (ss == StageState.InQueue || ss == StageState.Skipped)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(pumpRegistryElement.StagesParameters);

                XmlNode xn = xmlDoc.SelectSingleNode(string.Format("//{0}", this.state));
                if (xn != null)
                {
                    XmlHelper.SetAttribute(xn, attrStageInitialState, Convert.ToString(ss));
                    pumpRegistryElement.StagesParameters = xmlDoc.InnerXml;
                }
            }
        }

        #endregion Методы

        #endregion Реализация IStagesQueueElement
    }
}
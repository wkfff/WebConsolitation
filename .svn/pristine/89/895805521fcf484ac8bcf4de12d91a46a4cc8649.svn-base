using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
    /// <summary>
    /// Тип сообщения о результате обработки источника
    /// </summary>
    public enum DataSourceProcessingResult
    {
        /// <summary>
        /// Источник успешно обработан
        /// </summary>
        SuccessfulProcessed,

        /// <summary>
        /// Источник обработан, во время обработки были предупреждения
        /// </summary>
        ProcessedWithWarnings,

        /// <summary>
        /// Источник обработан, во время обработки были ошибки
        /// </summary>
        ProcessedWithErrors
    }


    /// <summary>
    /// Структура с данными предварительного просмотра источника
    /// </summary>
    public class PreviewDataResult : DisposableObject
    {

    }

    /// <summary>
    /// Настройки обработки источника данных
    /// </summary>
    public class DataSourceProcessingSettings : DisposableObject
    {
        #region Поля

        private string queryConstraint;

        #endregion Поля


        #region Инициализация

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="queryConstr">Ограничение выборки</param>
        public DataSourceProcessingSettings(string queryConstr)
        {
            this.queryConstraint = queryConstr;
        }

        #endregion Инициализация


        #region Свойства класса

        /// <summary>
        /// Ограничение выборки данных источника
        /// </summary>
        public string QueryConstraint
        {
            get
            {
                return queryConstraint;
            }
            set
            {
                queryConstraint = value;
            }
        }

        #endregion Свойства класса
    }


    /// <summary>
    /// Информация о результатах обработки источников данных закачки
    /// </summary>
    public sealed class DataSourcesProcessingResult : DisposableObject
    {
        #region Поля

        private Dictionary<int, DataSet> previewDataSources = new Dictionary<int, DataSet>(20);
        private Dictionary<int, string> pumpedSources = new Dictionary<int, string>(20);
        private SortedList<int, string> processedSources = new SortedList<int, string>(20);
        private Dictionary<int, DataSourceProcessingSettings> dataSourcesProcessingSettings =
            new Dictionary<int, DataSourceProcessingSettings>(20);
        private IScheme scheme;

        #endregion Поля


        #region Инициализация

        /// <summary>
        /// Конструктор
        /// </summary>
        public DataSourcesProcessingResult(IScheme scheme)
        {
            this.scheme = scheme;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (previewDataSources != null)
                    previewDataSources.Clear();

                if (pumpedSources != null)
                    pumpedSources.Clear();

                if (processedSources != null)
                    processedSources.Clear();

                if (dataSourcesProcessingSettings != null)
                {
                    dataSourcesProcessingSettings.Clear();
                    dataSourcesProcessingSettings = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion Инициализация


        #region Свойства класса

        /// <summary>
        /// Список датасетов с обработанными данными источников для предпросмотра и последующей закачки
        /// </summary>
        public Dictionary<int, DataSet> PreviewDataSources
        {
            get
            {
                return previewDataSources;
            }
        }

        /// <summary>
        /// Список закачанных источников.
        /// Ключ - ИД источника, значение - сообщение о результатах
        /// </summary>
        public Dictionary<int, string> PumpedSources
        {
            get
            {
                return pumpedSources;
            }
        }

        /// <summary>
        /// Список обработанных источников.
        /// Ключ - ИД источника, значение - сообщение о результатах закачки источника
        /// </summary>
        public SortedList<int, string> ProcessedSources
        {
            get
            {
                return processedSources;
            }
        }

        /// <summary>
        /// Настройки обработки источников данных
        /// Ключ - ИД источника, значение - Настройки обработки источника данных
        /// </summary>
        public Dictionary<int, DataSourceProcessingSettings> DataSourcesProcessingSettings
        {
            get
            {
                return dataSourcesProcessingSettings;
            }
        }

        #endregion Свойства класса


        #region Методы класса

        /// <summary>
        /// Добавляет запись в список предварительного просмотра данных источников
        /// </summary>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="ds">Датасет с данными источника</param>
        public void AddToPreviewDataSources(int sourceID, DataSet ds)
        {
            if (!previewDataSources.ContainsKey(sourceID))
            {
                previewDataSources.Add(sourceID, ds);
            }
            else
            {
                previewDataSources[sourceID] = ds;
            }
        }

        /// <summary>
        /// Добавляет запись в список закачанных источников
        /// </summary>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="comment">сообщение о результатах</param>
        public void AddToPumpedSources(int sourceID, string comment)
        {
            if (!pumpedSources.ContainsKey(sourceID))
            {
                pumpedSources.Add(sourceID, comment);
            }
        }

        /// <summary>
        /// Добавляет запись в список обработанных источников
        /// </summary>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="msg">Сообщение</param>
        public void AddToProcessedSources(int sourceID, DataSourceProcessingResult msg)
        {
            string message = string.Empty;

            switch (msg)
            {
                case DataSourceProcessingResult.ProcessedWithErrors: message = "Обработан с ошибками";
                    break;

                case DataSourceProcessingResult.ProcessedWithWarnings: message = "Обработан с предупреждениями";
                    break;

                case DataSourceProcessingResult.SuccessfulProcessed: message = "Обработан успешно";
                    break;
            }

            if (processedSources.ContainsKey(sourceID))
            {
                processedSources[sourceID] = message;
            }
            else
            {
                processedSources.Add(sourceID, message);
            }
        }

        /// <summary>
        /// Добавляет запись в список обработанных источников
        /// </summary>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="errMsg">Сообщение об ошибке. Пустая строка - успешно обработан</param>
        public void AddToProcessedSources(int sourceID, string errMsg)
        {
            string message = string.Empty;

            if (errMsg == string.Empty)
            {
                message = "Успешно обработан";
            }
            else
            {
                message = errMsg;
            }

            if (processedSources.ContainsKey(sourceID))
            {
                processedSources[sourceID] = message;
            }
            else
            {
                processedSources.Add(sourceID, message);
            }
        }

        /// <summary>
        /// Добавляет запись в список Настройки обработки источников данных
        /// </summary>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="errMsg">Сообщение об ошибке. Пустая строка - успешно обработан</param>
        public void AddToDataSourcesProcessingSettings(int sourceID, DataSourceProcessingSettings settings)
        {
            if (!dataSourcesProcessingSettings.ContainsKey(sourceID))
            {
                dataSourcesProcessingSettings.Add(sourceID, settings);
            }
        }

        #endregion Методы класса
    }
}
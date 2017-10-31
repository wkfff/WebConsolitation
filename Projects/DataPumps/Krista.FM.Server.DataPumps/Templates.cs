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
    // Модуль шаблонов наиболее употребимых функций

    /// <summary>
    /// Базовый класс для всех закачек.
    /// </summary>
    public abstract partial class DataPumpModuleBase : DisposableObject
    {
        /// <summary>
        /// Функция обработки файла отчета
        /// </summary>
        /// <param name="fileInfo">Файл</param>
        protected delegate void ProcessFileDelegate(FileInfo fileInfo);

        /// <summary>
        /// Шаблон функции обработки файлов источника данных
        /// </summary>
        /// <param name="dir">Каталог источника</param>
        /// <param name="searchPattern">Маска файлов</param>
        /// <param name="emptyDirException">Генерить исключение при пустом каталоге или нет</param>
        /// <param name="searchOption">Параметр поиска файлов: обходить вложенные каталоги или нет</param>
        protected void ProcessFilesTemplate(DirectoryInfo dir, string searchPattern, 
            ProcessFileDelegate processFile, bool emptyDirException, SearchOption searchOption)
        {
            WriteToTrace(String.Format("Запрос списка файлов по маске {0}", searchPattern), TraceMessageKind.Information);
            FileInfo[] files = dir.GetFiles(searchPattern, searchOption);
            if (files.GetLength(0) == 0 && emptyDirException)
            {
                throw new Exception("В каталоге источника нет данных для закачки");
            }

            string sourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int totalFiles = files.GetLength(0);

            // Обрабатываем файлы
            for (int i = 0; i < totalFiles; i++)
            {
                SetProgress(totalFiles, i + 1,
                    string.Format("Обработка файла {0}\\{1}...", sourcePath, files[i].Name),
                    string.Format("Файл {0} из {1}", i + 1, totalFiles), true);

                if (!files[i].Exists)
                    continue;

                if (files[i].Directory.Name.StartsWith("__"))
                    continue;

                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping,
                    string.Format("Старт закачки файла {0}.", files[i].FullName));

                try
                {
                    processFile(files[i]);

                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeSuccessfullFinishFilePump,
                        string.Format("Закачка файла {0} успешно завершена.", files[i].FullName));
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishFilePumpWithError,
                        string.Format("Закачка файла {0} завершена с ошибками.", files[i].FullName), ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Шаблон функции обработки файлов источника данных
        /// </summary>
        /// <param name="dir">Каталог источника</param>
        /// <param name="searchPattern">Маска файлов</param>
        protected void ProcessFilesTemplate(DirectoryInfo dir, string searchPattern,
            ProcessFileDelegate processFile)
        {
            ProcessFilesTemplate(dir, searchPattern, processFile, true);
        }

        /// <summary>
        /// Шаблон функции обработки файлов источника данных
        /// </summary>
        /// <param name="dir">Каталог источника</param>
        /// <param name="searchPattern">Маска файлов</param>
        /// <param name="emptyDirException">Генерить исключение при пустом каталоге или нет</param>
        protected void ProcessFilesTemplate(DirectoryInfo dir, string searchPattern,
            ProcessFileDelegate processFile, bool emptyDirException)
        {
            ProcessFilesTemplate(dir, searchPattern, processFile, emptyDirException, SearchOption.AllDirectories);
        }

        /// <summary>
        /// Делегат функции обработки полученных данных
        /// </summary>
        /// <param name="row">Строка с данными для обработки</param>
        protected delegate void DataPartRowProcessing(DataRow row);

        /// <summary>
        /// Шаблон функции обработки больших объемов данных по частям
        /// </summary>
        /// <param name="obj">Объект таблицы</param>
        /// <param name="constr">Ограничение для выборки</param>
        /// <param name="constMaxQueryRecords">Интервал первичного ключа для запроса</param>
        /// <param name="dataPartProcessing">Делегат функции обработки полученных данных</param>
        protected void PartialDataProcessingTemplate(IEntity obj, string constr, int constMaxQueryRecords,
            DataPartRowProcessing dataPartProcessing, string message)
        {
            IDbDataAdapter da = null;
            DataSet ds = null;

            if (!string.IsNullOrEmpty(constr))
                constr += " and ";

            constr += string.Format("SOURCEID = {0}", this.SourceID);

            try
            {
                int totalRecs = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                    "select count(id) from {0} where {1}", obj.FullDBName, constr), QueryResultTypes.Scalar));
                if (totalRecs == 0)
                {
                    WriteToTrace(string.Format("Нет данных {0} для обработки по текущему источнику.", obj.FullCaption), TraceMessageKind.Warning);
                    return;
                }

                int firstID = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                    "select min(id) from {0} where {1}", obj.FullDBName, constr), QueryResultTypes.Scalar));
                int lastID = firstID + constMaxQueryRecords - 1;
                int processedRecCount = 0;

                do
                {
                    // Ограничение запроса для выборки порции данных
                    string idConstr = string.Format("ID >= {0} and ID <= {1} and {2}", firstID, lastID, constr);
                    firstID = lastID + 1;
                    lastID += constMaxQueryRecords;

                    InitDataSet(ref da, ref ds, obj, idConstr);
                    DataTable dt = ds.Tables[0];

                    if (dt.Rows.Count == 0)
                    {
                        continue;
                    }

                    int recCount = dt.Rows.Count;

                    // Устанавливаем соответствие
                    for (int i = 0; i < recCount; i++)
                    {
                        processedRecCount++;
                        SetProgress(totalRecs, processedRecCount,
                            string.Format("{0} для источника {1}...", message, this.SourceID),
                            string.Format("Строка {0} из {1}", processedRecCount, totalRecs));

                        DataRow row = dt.Rows[i];

                        dataPartProcessing(row);
                    }

                    UpdateDataSet(da, ds, obj);
                }
                while (processedRecCount < totalRecs);

                UpdateDataSet(da, ds, obj);
            }
            finally
            {
                ClearDataSet(ref ds);
            }
        }
    }
}
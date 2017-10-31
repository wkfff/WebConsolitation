using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;

using Krista.FM.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
    /// <summary>
    /// Базовый класс для закачек из текстовых файлов
    /// </summary>
    public abstract class TextRepPumpModuleBase : CorrectedPumpModuleBase, ITextRepPump
    {
        #region Делегаты

        /// <summary>
        /// Делегат функции обработки данных отчета
        /// </summary>
        /// <param name="reportFilesRow">Строка таблицы с описанием файлов отчетов</param>
        /// <param name="reportRows">Массив строк отчета</param>
        public delegate void ProcessTextReportData(DataRow reportFilesRow, DataRow[] reportRows);

        #endregion Делегаты


        #region Поля

        private DataSet resultDataSet;
        private List<FileInfo[]> repFilesLists;
        private Dictionary<int, Dictionary<string, FixedParameter>> fixedParameters;
        private string fileIndexFieldName;
        private string tableIndexFieldName;
        private CharacterSet filesCharacterSet;

        #endregion Поля


        #region Константы

        protected const string xmlSettingsFileDirName = "TextRepSettings";
        protected const string xmlSettingsForm14 = "Form14.xml";
        protected const string xmlSettingsForm16 = "Form16.xml";
        protected const string xmlSettingsForm1OBL_Regions = "Form1OBL_Regions.xml";
        protected const string xmlSettingsForm1OBL_Total = "Form1OBL_Total.xml";
        protected const string xmlSettingsForm5NIO_Regions = "Form5NIO_Regions.xml";
        protected const string xmlSettingsForm5NIO_Total = "Form5NIO_Total.xml";
        protected const string xmlSettingsForm13 = "Form13.xml";
        protected const string xmlSettingsForm10 = "Form10.xml";
        protected const string xmlSettingsForm5 = "Form5.xml";
        protected const string xmlSettingsForm1NMSvod2005 = "Form1NMSvod2005.xml";
        protected const string xmlSettingsForm1NMStr2005 = "Form1NMStr2005.xml";
        protected const string xmlSettingsForm1NMSvod2005Karelya = "Karelya\\Form1NMSvod2005.xml";
        protected const string xmlSettingsForm1NMStr2005Karelya = "Karelya\\Form1NMStr2005.xml";
        protected const string xmlSettingsForm1NMSvod2005Saratov = "Saratov\\Form1NMSvod2005.xml";
        protected const string xmlSettingsForm1NMStr2005Saratov = "Saratov\\Form1NMStr2005.xml";
        protected const string xmlSettingsForm1NMSvod2006 = "Form1NMSvod2006.xml";
        protected const string xmlSettingsForm1NMStr2006 = "Form1NMStr2006.xml";
        protected const string xmlSettingsUFK14 = "UFK14.xml";

        #endregion Константы


        #region Общие функции

        /// <summary>
        /// Вызывает разборщик текстовых файлов
        /// </summary>
        /// <param name="settingsFileName">Файл с хмл-настройками</param>
        /// <param name="repFilesPath">Путь к текстовым файлам отчетов</param>
        protected void CallTXTSorcerer(string settingsFileName, string repFilesDir)
        {
            //if (this.StagesQueue[PumpProcessStates.PreviewData].IsExecuted) return;

            string xmlSettings = string.Format(
                "{0}\\{1}\\{2}", 
                CommonRoutines.GetCurrentDir().FullName, xmlSettingsFileDirName, settingsFileName);

            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, 
                string.Format("Старт разбора файлов. Файл настроек {0}.", xmlSettings));

            this.FileIndexFieldName = "_FILEINDEX_";
            this.TableIndexFieldName = "_TABLEINDEX_";

            TXTSorcerer sorc = new TXTSorcerer(
                xmlSettings,
                repFilesDir,
                this.FileIndexFieldName,
                this.TableIndexFieldName);

            try
            {
                sorc.GetDataFromFiles(this, ref this.resultDataSet, ref this.repFilesLists, ref this.fixedParameters,
                    out this.filesCharacterSet);

                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Разбор файлов закончен.");
            }
            catch (FilesNotFoundException ex)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "Разбор файлов закончен с предупреждениями", ex);
                throw;
            }
            catch (Exception ex)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, "Разбор файлов закончен с ошибками", ex);
                throw;
            }
            finally
            {
                sorc.Dispose();
                sorc = null;
            }
        }

        /// <summary>
        /// Возвращает количество строк во всех таблицах текстовых отчетов
        /// </summary>
        /// <returns>Количество строк</returns>
        protected int GetTotalRecs()
        {
            int result = 0;
            for (int i = 1; i < this.ResultDataSet.Tables.Count; i++)
            {
                result += this.ResultDataSet.Tables[i].Rows.Count;
            }
            return result;
        }

        /// <summary>
        /// Функция обработки датасета с данными отчетов источника
        /// </summary>
        /// <param name="processTextReportData">Делегат функции обработки данных отчета</param>
        protected void ProcessResultDataSet(ProcessTextReportData processTextReportData)
        {
            // Обходим записи первой таблицы, содержащей данные о закачанный файлах
            DataTable filesTable = this.ResultDataSet.Tables[0];
            for (int i = 0; i < filesTable.Rows.Count; i++)
            {
                // Получаем все релейшены строки и по ним подчиненные таблицы с данными отчетов
                for (int j = 0; j < this.ResultDataSet.Relations.Count; i++)
                {
                    DataRow[] reportRows = filesTable.Rows[i].GetChildRows(this.ResultDataSet.Relations[j]);

                    // Вызываем функцию обработки строк отчета
                    processTextReportData(filesTable.Rows[i], reportRows);
                }
            }
        }

        #endregion Общие функции


        #region Реализация ITextRepPump

        #region Свойства

        /// <summary>
        /// Таблица с данными текстовых файлов
        /// </summary>
        public DataSet ResultDataSet
        {
            get
            {
                return resultDataSet;
            }
            set
            {
                resultDataSet = value;
            }
        }

        /// <summary>
        /// Список фиксированных параметров (номер файла - наименование параметра - значение)
        /// </summary>
        public Dictionary<int, Dictionary<string, FixedParameter>> FixedParameters
        {
            get
            {
                return fixedParameters;
            }
            set
            {
                fixedParameters = value;
            }
        }

        /// <summary>
        /// Список файлов отчетов. По индексу файла в списке фильтруются его данные в ResultTable
        /// </summary>
        public List<FileInfo[]> RepFilesLists
        {
            get 
            { 
                return repFilesLists; 
            }
            set 
            { 
                repFilesLists = value; 
            }
        }

        /// <summary>
        /// Наименование столбца в ResultDataSet, по которому определяется принадлежность данных конкретному файлу
        /// </summary>
        public string FileIndexFieldName
        {
            get 
            { 
                return fileIndexFieldName; 
            }
            set 
            { 
                fileIndexFieldName = value; 
            }
        }

        /// <summary>
        /// Наименование столбца в таблице с данными отчета, по которому определяется принадлежность данных 
        /// таблице файла отчета
        /// </summary>
        public string TableIndexFieldName
        {
            get 
            { 
                return tableIndexFieldName; 
            }
            set 
            { 
                tableIndexFieldName = value; 
            }
        }

        /// <summary>
        /// Кодировка файлов отчетов
        /// </summary>
        public CharacterSet FilesCharacterSet
        {
            get 
            { 
                return filesCharacterSet; 
            }
            set 
            { 
                filesCharacterSet = value; 
            }
        }

        #endregion Свойства

        #endregion Реализация ITextRepPump
    }
}

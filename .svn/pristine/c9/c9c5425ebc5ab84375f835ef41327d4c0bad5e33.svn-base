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
    // Модуль шаблонов наиболее употребимых функций этапа закачки

    /// <summary>
    /// Базовый класс для всех закачек.
    /// </summary>
    public abstract partial class DataPumpModuleBase : DisposableObject
    {

        #region Константы

        /// <summary>
        /// Префикс названия каталога источника, который будет пропущен без ошибки
        /// </summary>
        protected const string constSkippedDirectoryPrefix = "__";

        #endregion Константы


        #region Общие функции обработки источников данных

        /// <summary>
        /// Инициализация объектов схемы.
        /// Здесь формируются свойства UsedClassifiers и UsedFacts
        /// </summary>
        protected virtual void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[0];
            this.UsedFacts = new IFactTable[0];
        }

        /// <summary>
        /// Функция запроса данных из базы
        /// </summary>
        protected virtual void QueryData()
        {

        }

        #region добавление версий объекта

        private string GetClsPresentationGuid(IClassifier cls)
        {
            SortedDictionary<int, string> sortedPresents = new SortedDictionary<int, string>();
            try
            {
                // получаем сортированный по году (возрастание) список представлений
                foreach (KeyValuePair<string, IPresentation> presentation in cls.Presentations)
                {
                    string name = presentation.Value.Name;
                    // год - последние 4 символа
                    int presentationYear = Convert.ToInt32(name.Substring(name.Length - 4));
                    if (!sortedPresents.ContainsKey(presentationYear))
                        sortedPresents.Add(presentationYear, presentation.Key);
                }
                // получаем нужный гвид представления
                string guid = string.Empty;
                foreach (KeyValuePair<int, string> item in sortedPresents)
                {
                    if (guid == string.Empty)
                        guid = item.Value;
                    if (item.Key == this.DataSource.Year)
                    {
                        guid = item.Value;
                        break;
                    }
                    if (item.Key > this.DataSource.Year)
                        break;
                    guid = item.Value;
                }
                return guid;
            }
            finally
            {
                sortedPresents.Clear();
            }
        }

        private void SetClsVersion(IClassifier cls)
        {
            string presentationGuid = GetClsPresentationGuid(cls);
            string dataSourceDescr = this.Scheme.DataSourceManager.GetDataSourceName(this.SourceID);
            string versionName = string.Format("{0}.{1}", cls.FullCaption, dataSourceDescr);
            // ищем нужную версию
            string query = string.Format("select count(*) from objectVersions where SourceId = {0} and ObjectKey = '{1}'",
                this.SourceID, cls.ObjectKey, presentationGuid);
            int versionCount = Convert.ToInt32(this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { }));
            if (versionCount != 0)
            {
                // апдейтим версию
                query = string.Format("Update objectVersions set PresentationKey = '{0}', Name = '{1}' where SourceId = {2} and ObjectKey = '{3}'",
                    presentationGuid, versionName, this.SourceID, cls.ObjectKey);
            }
            else
            {
                // вставляем версию
                query = string.Format("{0} ({1}, '{2}', '{3}', '{4}')",
                    "Insert into objectVersions (SourceId, ObjectKey, PresentationKey, Name) values",
                    this.SourceID, cls.ObjectKey, presentationGuid, versionName);
            }
            this.DB.ExecQuery(query, QueryResultTypes.NonQuery, new IDbDataParameter[] { });
        }

        protected void SetPresentationContext(IClassifier cls)
        {
            LogicalCallContextData context = LogicalCallContextData.GetContext();
            context[String.Format("{0}.Presentation", cls.FullDBName)] = GetClsPresentationGuid(cls);
            LogicalCallContextData.SetContext(context);
        }

        protected void ClearPresentationContext(IClassifier cls)
        {
            LogicalCallContextData context = LogicalCallContextData.GetContext();
            context[String.Format("{0}.Presentation", cls.FullDBName)] = null;
            LogicalCallContextData.SetContext(context);
        }

        protected virtual void SetClsVersion()
        {
            if (VersionClassifiers == null)
                return;
            foreach (IClassifier cls in VersionClassifiers)
                SetClsVersion(cls);
        }

        #endregion добавление версий объекта

        /// <summary>
        /// Функция обработки файлов источника данных
        /// </summary>
        /// <param name="dir">Каталог источника</param>
        protected virtual void ProcessFiles(DirectoryInfo dir)
        {

        }

        /// <summary>
        /// Функция обработки данных после этапа предварительного просмотра
        /// </summary>
        /// <param name="ds">Датасет с данными</param>
        protected virtual void ProcessPreviewData(DataSet ds)
        {

        }

        /// <summary>
        /// Функция сохранения закачанных данных в базу
        /// </summary>
        protected virtual void UpdateData()
        {

        }

        /// <summary>
        /// Функция выполнения завершающих действий закачки
        /// </summary>
        protected virtual void PumpFinalizing()
        {

        }

        /// <summary>
        /// Закачивает данные, полученные на этапе предпросмотра
        /// </summary>
        private void PumpPreviewData()
        {
            // Если все источники были обработаны с ошибками, то пропускаем все следующие этапы
            if (this.PreviewDataSources.Count == 0)
            {
                throw new Exception("Нет корректно обработанных источников, данные из которых могли бы быть закачаны.");
            }

            foreach (KeyValuePair<int, DataSet> kvp in this.PreviewDataSources)
            {
                SetDataSource(kvp.Key);
                ProcessPreviewData(kvp.Value);
            }
        }

        /// <summary>
        /// Проверяет на корректность год источника
        /// </summary>
        /// <param name="dir">Каталог года</param>
        /// <returns>Год (-1 - не прошел проверку)</returns>
        protected int CheckDataSourceYearDir(DirectoryInfo dir)
        {
            if (!dir.Exists) return -1;

            if (dir.Name.StartsWith(constSkippedDirectoryPrefix))
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, 
                    string.Format("Каталог источника {0} пропущен по указанию пользователя.", dir.Name));
                return -1;
            }

            try
            {
                return Convert.ToInt32(dir.Name);
            }
            catch
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeCriticalError, 
                    string.Format("Год источника {0} задан некорректно.", dir.Name));
            }

            return -1;
        }

        /// <summary>
        /// Проверяет на корректность месяц источника
        /// </summary>
        /// <param name="dir">Каталог месяца</param>
        /// <returns>Год (-1 - не прошел проверку)</returns>
        protected int CheckDataSourceMonthDir(DirectoryInfo dir)
        {
            if (!dir.Exists) return -1;

            if (dir.Name.StartsWith(constSkippedDirectoryPrefix))
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, 
                    string.Format("Каталог источника {0} пропущен по указанию пользователя.", dir.Name));
                return -1;
            }

            int month = -1;

            try
            {
                month = Convert.ToInt32(dir.Name);
            }
            catch
            {
            }

            if (month == -1 || (month < 1 || month > 12))
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeCriticalError, 
                    string.Format("Месяц источника {0} задан некорректно.", dir.Name));
            }

            return month;
        }

        /// <summary>
        /// Проверяет на корректность квартал источника
        /// </summary>
        /// <param name="dir">Каталог квартала</param>
        /// <returns>квартал (-1 - не прошел проверку)</returns>
        protected int CheckDataSourceQuarterDir(DirectoryInfo dir)
        {
            if (!dir.Exists) 
                return -1;
            if (dir.Name.StartsWith(constSkippedDirectoryPrefix))
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Каталог источника {0} пропущен по указанию пользователя.", dir.Name));
                return -1;
            }
            int quarter = -1;
            try
            {
                quarter = Convert.ToInt32(dir.Name);
            }
            catch
            {
                quarter = -1;
            }

            if (quarter == -1 || (quarter < 1 || quarter > 4))
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeCriticalError,
                    string.Format("Квартал источника {0} задан некорректно.", dir.Name));

            return quarter;
        }

        /// <summary>
        /// Проверяет на корректность каталог источника
        /// </summary>
        /// <param name="dir">Каталог</param>
        /// <returns>Название (пустая строка - не прошел проверку)</returns>
        protected string CheckDataSourceCommonDir(DirectoryInfo dir)
        {
            if (!dir.Exists) return string.Empty;

            if (dir.Name.StartsWith(constSkippedDirectoryPrefix))
            {
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeWarning, string.Format("Каталог источника {0} пропущен по указанию пользователя.", dir.Name));
                return string.Empty;
            }

            return dir.Name;
        }

        #endregion Общие функции обработки источников данных


        #region Функции обработки источника данных "Год"
     
        /// <summary>
        /// Закачивает источник "Год"
        /// </summary>
        private void PumpYDataSourceData()
        {
            // Список каталогов с годами
            DirectoryInfo[] dir_years = this.RootDir.GetDirectories("*", SearchOption.TopDirectoryOnly);
            // Если не найден ни один источник в этом каталоге, то пишем об этом в лог
            if (dir_years.GetLength(0) == 0)
            {
                throw new PumpDataFailedException(string.Format(
                    "В каталоге {0} не найдено ни одного источника.", this.RootDir.FullName));
            }

            // Обходим все каталоги и закачиваем файлы
            for (int i = 0; i < dir_years.GetLength(0); i++)
            {
                this.DataSource = null;

                int sourceYear = CheckDataSourceYearDir(dir_years[i]);
                if (sourceYear < 0) continue;

                // Добавляем источник
                SetDataSource(ParamKindTypes.Year, string.Empty, sourceYear, 0, string.Empty, 0, string.Empty);

                PumpDataSource(dir_years[i]);
            }
        }

        /// <summary>
        /// Шаблон для функции закачки из источника "Год"
        /// </summary>
        protected void PumpDataYTemplate()
        {
            // Если был выполнен этап предпросмотра, то вызываем закачку обработанных данных
            //if (this.StagesQueue[PumpProcessStates.PreviewData].IsExecuted)
            {
            //    PumpPreviewData();
            }
            // иначе закачиваем исходные данные
            //else
            {
                PumpYDataSourceData();
            }
        }

        #endregion Функции обработки источника данных "Год"


        #region Функции обработки источника данных "Год-Месяц"

        /// <summary>
        /// Шаблон для функции закачки из источника "Год-Месяц"
        /// </summary>
        protected void PumpDataYMTemplate()
        {
            string str = string.Empty;

            // Список каталогов с годами
            DirectoryInfo[] dir_years = this.RootDir.GetDirectories("*", SearchOption.TopDirectoryOnly);
            // Если не найден ни один источник в этом каталоге, то пишем об этом в лог
            if (dir_years.GetLength(0) == 0)
            {
                throw new PumpDataFailedException(string.Format("В каталоге {0} не найдено ни одного источника.", this.RootDir.FullName));
            }

            // Обходим все каталоги и закачиваем файлы
            for (int i = 0; i < dir_years.GetLength(0); i++)
            {
                this.DataSource = null;

                int sourceYear = CheckDataSourceYearDir(dir_years[i]);
                if (sourceYear < 0) continue;

                // Выбираем каталоги с месяцами
                DirectoryInfo[] dir_months = dir_years[i].GetDirectories();
                // Если не найден ни один источник в этом каталоге, то пишем об этом в лог
                if (dir_months.GetLength(0) == 0)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeCriticalError, 
                        string.Format("В каталоге {0} не найдено ни одного источника.", dir_years[i].FullName));
                    continue;
                }

                for (int j = 0; j < dir_months.GetLength(0); j++)
                {
                    int sourceMonth = CheckDataSourceMonthDir(dir_months[j]);
                    if (sourceMonth < 0) continue;

                    // Добавляем источник
                    // Закачанные ранее данные здесь не удаляются - это дело отрабатывается ниже
                    SetDataSource(ParamKindTypes.YearMonth, string.Empty, sourceYear, sourceMonth, string.Empty, 0, string.Empty);
                    
                    PumpDataSource(dir_months[j]);
                }
            }
        }

        #endregion Функции обработки источника данных "Год-Месяц"



        #region Функции обработки источника данных "Год-Квартал"

        /// <summary>
        /// Шаблон для функции закачки из источника "Год-Квартал"
        /// </summary>
        protected void PumpDataYQTemplate()
        {
            string str = string.Empty;

            // Список каталогов с годами
            DirectoryInfo[] dir_years = this.RootDir.GetDirectories("*", SearchOption.TopDirectoryOnly);
            // Если не найден ни один источник в этом каталоге, то пишем об этом в лог
            if (dir_years.GetLength(0) == 0)
                throw new PumpDataFailedException(string.Format("В каталоге {0} не найдено ни одного источника.", this.RootDir.FullName));

            // Обходим все каталоги и закачиваем файлы
            for (int i = 0; i < dir_years.GetLength(0); i++)
            {
                this.DataSource = null;

                int sourceYear = CheckDataSourceYearDir(dir_years[i]);
                if (sourceYear < 0) 
                    continue;

                // Выбираем каталоги с месяцами
                DirectoryInfo[] dir_quarters = dir_years[i].GetDirectories();
                // Если не найден ни один источник в этом каталоге, то пишем об этом в лог
                if (dir_quarters.GetLength(0) == 0)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeCriticalError,
                        string.Format("В каталоге {0} не найдено ни одного источника.", dir_years[i].FullName));
                    continue;
                }

                for (int j = 0; j < dir_quarters.GetLength(0); j++)
                {
                    int sourceQuarter = CheckDataSourceQuarterDir(dir_quarters[j]);
                    if (sourceQuarter < 0) 
                        continue;

                    // Добавляем источник
                    // Закачанные ранее данные здесь не удаляются - это дело отрабатывается ниже
                    SetDataSource(ParamKindTypes.YearQuarter, string.Empty, sourceYear, 0, string.Empty, sourceQuarter, string.Empty);
                    PumpDataSource(dir_quarters[j]);
                }
            }
        }

        #endregion Функции обработки источника данных "Год-Квартал"


        #region Функции обработки источника данных "Год-Месяц-Вариант"

        /// <summary>
        /// Шаблон для функции закачки из источника "Год-Месяц-Вариант"
        /// </summary>
        protected void PumpDataYMVTemplate()
        {
            string str = string.Empty;

            // Список каталогов с годами
            DirectoryInfo[] dir_years = this.RootDir.GetDirectories("*", SearchOption.TopDirectoryOnly);
            // Если не найден ни один источник в этом каталоге, то пишем об этом в лог
            if (dir_years.GetLength(0) == 0)
            {
                throw new PumpDataFailedException(string.Format(
                    "В каталоге {0} не найдено ни одного источника.", this.RootDir.FullName));
            }

            // Обходим все каталоги с годами
            for (int i = 0; i < dir_years.GetLength(0); i++)
            {
                this.DataSource = null;

                int sourceYear = CheckDataSourceYearDir(dir_years[i]);
                if (sourceYear < 0) continue;

                DirectoryInfo[] dir_months = dir_years[i].GetDirectories("*", SearchOption.TopDirectoryOnly);
                // Если не найден ни один источник в этом каталоге, то пишем об этом в лог
                if (dir_months.GetLength(0) == 0)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeCriticalError, 
                        string.Format("В каталоге {0} не найдено ни одного источника.", dir_years[i].FullName));
                    continue;
                }

                // Обходим все каталоги с месяцами
                for (int j = 0; j < dir_months.GetLength(0); j++)
                {
                    int sourceMonth = CheckDataSourceMonthDir(dir_months[j]);
                    if (sourceMonth < 0) continue;

                    DirectoryInfo[] dir_variants = dir_months[j].GetDirectories("*", SearchOption.TopDirectoryOnly);
                    // Если не найден ни один источник в этом каталоге, то пишем об этом в лог
                    if (dir_variants.GetLength(0) == 0)
                    {
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, 
                            string.Format("В каталоге {0} не найдено ни одного источника.", dir_months[j].FullName));
                        continue;
                    }

                    // Обходим все каталоги с вариантами
                    for (int k = 0; k < dir_variants.GetLength(0); k++)
                    {
                        string variant = CheckDataSourceCommonDir(dir_variants[k]);
                        if (variant == string.Empty) continue;

                        // Добавляем источник
                        SetDataSource(ParamKindTypes.YearMonthVariant, string.Empty, sourceYear, sourceMonth, variant, 0, string.Empty);

                        PumpDataSource(dir_variants[k]);
                    }
                }
            }
        }

        #endregion Функции обработки источника данных "Год-Месяц-Вариант"


        #region Функции обработки источника данных "Год-Территория"

        /// <summary>
        /// Шаблон для функции закачки из источника "Год-Территория"
        /// </summary>
        protected void PumpDataYTTemplate()
        {
            string str = string.Empty;

            // Список каталогов с годами
            DirectoryInfo[] dir_years = this.RootDir.GetDirectories("*", SearchOption.TopDirectoryOnly);
            // Если не найден ни один источник в этом каталоге, то пишем об этом в лог
            if (dir_years.GetLength(0) == 0)
            {
                throw new PumpDataFailedException(string.Format(
                    "В каталоге {0} не найдено ни одного источника.", this.RootDir.FullName));
            }

            // Обходим все каталоги и закачиваем файлы
            for (int i = 0; i < dir_years.GetLength(0); i++)
            {
                this.DataSource = null;

                int sourceYear = CheckDataSourceYearDir(dir_years[i]);
                if (sourceYear < 0) continue;

                // Выбираем каталоги с месяцами
                DirectoryInfo[] dir_territory = dir_years[i].GetDirectories();
                // Если не найден ни один источник в этом каталоге, то пишем об этом в лог
                if (dir_territory.GetLength(0) == 0)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeCriticalError, 
                        string.Format("В каталоге {0} не найдено ни одного источника.", dir_years[i].FullName));
                    continue;
                }

                for (int j = 0; j < dir_territory.GetLength(0); j++)
                {
                    string territory = CheckDataSourceCommonDir(dir_territory[j]);
                    if (territory == string.Empty) continue;

                    // Добавляем источник
                    // Закачанные ранее данные здесь не удаляются - это дело отрабатывается ниже
                    SetDataSource(ParamKindTypes.YearTerritory, string.Empty, sourceYear, 0, string.Empty, 0, territory);

                    PumpDataSource(dir_territory[j]);
                }
            }
        }

        #endregion Функции обработки источника данных "Год-Территория"


        #region Функции обработки источника данных "Финорган-Год"

        /// <summary>
        /// Шаблон для функции закачки из источника "ФО-Год"
        /// </summary>
        protected void PumpDataFYTemplate()
        {
            string str = string.Empty;

            // Список каталогов с годами
            DirectoryInfo[] dir_fo = this.RootDir.GetDirectories("*", SearchOption.TopDirectoryOnly);
            // Если не найден ни один источник в этом каталоге, то пишем об этом в лог
            if (dir_fo.GetLength(0) == 0)
            {
                throw new PumpDataFailedException(string.Format(
                    "В каталоге {0} не найдено ни одного источника.", this.RootDir.FullName));
            }

            // Обходим все каталоги и закачиваем файлы
            for (int i = 0; i < dir_fo.GetLength(0); i++)
            {
                this.DataSource = null;

                string fo = CheckDataSourceCommonDir(dir_fo[i]);
                if (fo == string.Empty) continue;

                // Выбираем каталоги с годами
                DirectoryInfo[] dir_years = dir_fo[i].GetDirectories();
                // Если не найден ни один источник в этом каталоге, то пишем об этом в лог
                if (dir_years.GetLength(0) == 0)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeCriticalError, 
                        string.Format("В каталоге {0} не найдено ни одного источника.", dir_fo[i].FullName));
                    continue;
                }

                for (int j = 0; j < dir_years.GetLength(0); j++)
                {
                    int sourceYear = CheckDataSourceYearDir(dir_years[j]);
                    if (sourceYear < 0) continue;

                    // Добавляем источник
                    // Закачанные ранее данные здесь не удаляются - это дело отрабатывается ниже
                    SetDataSource(ParamKindTypes.Budget, dir_fo[i].Name, sourceYear, 0, string.Empty, 0, string.Empty);

                    PumpDataSource(dir_years[j]);
                }
            }
        }

        #endregion Функции обработки источника данных "Финорган-Год"


        #region Функции обработки источника данных "Год-Вариант"

        /// <summary>
        /// Шаблон для функции закачки из источника "Год-Вариант"
        /// </summary>
        protected void PumpDataYVTemplate()
        {
            string str = string.Empty;

            // Список каталогов с годами
            DirectoryInfo[] dir_years = this.RootDir.GetDirectories("*", SearchOption.TopDirectoryOnly);
            // Если не найден ни один источник в этом каталоге, то пишем об этом в лог
            if (dir_years.GetLength(0) == 0)
            {
                throw new PumpDataFailedException(string.Format("В каталоге {0} не найдено ни одного источника.", this.RootDir.FullName));
            }

            // Обходим все каталоги и закачиваем файлы
            for (int i = 0; i < dir_years.GetLength(0); i++)
            {
                this.DataSource = null;

                int sourceYear = CheckDataSourceYearDir(dir_years[i]);
                if (sourceYear < 0)
                    continue;

                // Выбираем каталоги с месяцами
                DirectoryInfo[] dir_variant = dir_years[i].GetDirectories();
                // Если не найден ни один источник в этом каталоге, то пишем об этом в лог
                if (dir_variant.GetLength(0) == 0)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeCriticalError,
                        string.Format("В каталоге {0} не найдено ни одного источника.", dir_years[i].FullName));
                    continue;
                }

                for (int j = 0; j < dir_variant.GetLength(0); j++)
                {

                    if (dir_variant[j].Name.StartsWith(constSkippedDirectoryPrefix))
                    {
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                            string.Format("Каталог источника {0} пропущен по указанию пользователя.", dir_variant[j].Name));
                        continue;
                    }

                    // Добавляем источник
                    // Закачанные ранее данные здесь не удаляются - это дело отрабатывается ниже
                    SetDataSource(ParamKindTypes.YearVariant, string.Empty, sourceYear, 0, dir_variant[j].Name, 
                        0, string.Empty);

                    PumpDataSource(dir_variant[j]);                    
                }
            }
        }

        #endregion Функции обработки источника данных "Год-Вариант"


        #region Функции обработки источника данных "Вариант"

        /// <summary>
        /// Шаблон для функции закачки из источника "Вариант"
        /// </summary>
        protected void PumpDataVTemplate()
        {
            string str = string.Empty;

            // Выбираем каталоги с месяцами
            DirectoryInfo[] dir_variant = this.RootDir.GetDirectories("*", SearchOption.TopDirectoryOnly);
            // Если не найден ни один источник в этом каталоге, то пишем об этом в лог
            if (dir_variant.GetLength(0) == 0)
            {
                throw new PumpDataFailedException(string.Format("В каталоге {0} не найдено ни одного источника.", this.RootDir.FullName));
            }

            for (int j = 0; j < dir_variant.GetLength(0); j++)
            {

                if (dir_variant[j].Name.StartsWith(constSkippedDirectoryPrefix))
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                        string.Format("Каталог источника {0} пропущен по указанию пользователя.", dir_variant[j].Name));
                    continue;
                }

                // Добавляем источник
                // Закачанные ранее данные здесь не удаляются - это дело отрабатывается ниже
                SetDataSource(ParamKindTypes.Variant, string.Empty, 0, 0, dir_variant[j].Name, 0, string.Empty);

                PumpDataSource(dir_variant[j]);
            }
        }

        #endregion Функции обработки источника данных "Вариант"


        #region Функции обработки файлов источника

        /// <summary>
        /// Функция закачки данных источника
        /// </summary>
        /// <param name="dir">Каталог источника</param>
        protected virtual void PumpDataSource(DirectoryInfo dir)
        {
            string dirFullName = string.Empty;
            if (dir != null)
                dirFullName = dir.FullName;
            // Пишем в протокол о начале закачки из источника
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartDataSourceProcessing, string.Format(
                "Старт закачки из источника {0} (ID источника {1}).",
                dirFullName, this.SourceID));
            BeginTransaction();
            try
            {
                if ((dir != null) && this.CheckSourceDirToEmpty)
                    // Проверяем, есть здесь что качать
                    if (dir.GetFiles("*.*", SearchOption.AllDirectories).GetLength(0) == 0)
                        throw new DataSourceIsCorruptException(string.Format("Каталог {0} пуст.", dir.FullName));
                // Удаляем ранее закачанные данные по текущему источнику
                DeleteEarlierPumpedData();
                // для закачек 28н и бюджет - коммитим транзакцию
                // вызвано этим: если удаляется много данных (например 1 000 000), 
                // все удаленное "висит" в памяти для возможности отката
                // закачка тормозит и ваще становится невозможной для выполнения (тормозит именно сохранение)
                if ((this.PumpProgramID == PumpProgramID.FNS28nDataPump) || 
                    (this.PumpProgramID == PumpProgramID.BudgetDataPump))
                {
                    CommitTransaction();
                    BeginTransaction();
                }

                SetPresentationContexts();
                try
                {
                    // Запрос данных
                    SetProgress(0, 0, "Запрос данных...", string.Empty, true);
                    WriteToTrace("Запрос данных...", TraceMessageKind.Information);
                    QueryData();
                    CollectGarbage();
                    WriteToTrace("Запрос данных окончен.", TraceMessageKind.Information);
                    // добавление версий объектов
                    WriteToTrace("Добавление версий классификаторов...", TraceMessageKind.Information);
                    SetClsVersion();
                    WriteToTrace("Добавление версий классификаторов окончено.", TraceMessageKind.Information);
                    // Обработка файлов источника
                    ProcessFiles(dir);
                }
                finally
                {
                    ClearPresentationContexts();
                }

                // Сохранение данных
                WriteToTrace("Сохранение данных...", TraceMessageKind.Information);
                UpdateData();
                SetProgress(-1, -1, "Данные сохранены.", string.Empty, true);
                WriteToTrace("Данные сохранены.", TraceMessageKind.Information);
                CommitTransaction();
                // Перемещаем файлы источника в архив
                if (dir != null)
                    MoveFilesToArchive(dir);
                this.DataSourcesProcessingResult.AddToPumpedSources(this.SourceID, string.Empty);
                this.DataSourcesProcessingResult.AddToProcessedSources(this.SourceID, this.DataSourceProcessingResult);
                switch (this.DataSourceProcessingResult)
                {
                    case DataSourceProcessingResult.ProcessedWithErrors:
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeSuccessfullFinishDataSourceProcess,
                            string.Format("Закачка из источника {0} закончена. В процессе закачки были ошибки.", dirFullName));
                        break;
                    case DataSourceProcessingResult.ProcessedWithWarnings:
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeSuccessfullFinishDataSourceProcess,
                            string.Format("Закачка из источника {0} закончена. В процессе закачки были предупреждения.", dirFullName));
                        break;
                    case DataSourceProcessingResult.SuccessfulProcessed:
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeSuccessfullFinishDataSourceProcess,
                            string.Format("Закачка из источника {0} успешно закончена.", dirFullName));
                        break;
                }
            }
            catch (ThreadAbortException)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishDataSourceProcessingWithError,
                    string.Format("Закачка из источника {0} закончена с ошибками: операция прервана пользователем.", dirFullName));
                RollbackTransaction();
                throw;
            }
            catch (Exception ex)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishDataSourceProcessingWithError,
                    string.Format("Закачка из источника {0} закончена с ошибками. \nДанные не сохранены.", dirFullName), ex);
                this.DataSourcesProcessingResult.AddToProcessedSources(this.SourceID, ex.Message);
                RollbackTransaction();
            }
            finally
            {
                PumpFinalizing();
                this.DataSource = null;
                CollectGarbage();
            }
        }

        #endregion Функции обработки файлов источника
    }
}
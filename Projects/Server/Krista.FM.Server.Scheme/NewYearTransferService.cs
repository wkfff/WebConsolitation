using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Krista.FM.Server.Common;
using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme
{
    #region Класс, отвечающий за перевод базы на новый год

    public class NewYearTransferService
    {
        #region Поля

        /// <summary>
        /// Источник ФО_0029_Проект бюджета – ОФГ
        /// </summary>
        private int fo_0029_sourceid = -1;
        /// <summary>
        /// Источник ФО_0006_Анализ данных – ТГ
        /// </summary>
        private int fo_0006_sourceid = -1;
        /// <summary>
        /// ФО_0016_Мониторинг БК и КУ – ТГ 0, 1, 2, 3, 4 квартал
        /// </summary>
        private int fo_0016_sourceid = -1;
        /// <summary>
        /// ФО_0021_Мониторинг ФП и КУ – ТГ 0, 1, 2, 3, 4 квартал.
        /// </summary>
        private int fo_0021_sourceid = -1;
        /// <summary>
        /// ФО_0039_Оценка качества ОиОБП – ТГ 0, 1, 2, 3, 4 квартал
        /// </summary>
        private int fo_0039_sourceid = -1;
        /// <summary>
        /// ФО_0027_Мониторинг месячного отчета – ТГ
        /// </summary>
        private int fo_0027_sourceid = -1;
        /// <summary>
        /// ФО_0042_ Оценка качества ФМ – ТГ
        /// </summary>
        private int fo_0042_sourceid = -1;
        /// <summary>
        /// МОФО_0019_Анализ МБТ – ТГ
        /// </summary>
        private int mofo_0019_sourceid = -1;
        /// <summary>
        /// ФНС_0015_Форма 1НОМ – ТГ, 0 месяц
        /// </summary>
        private int fns_0015_sourceid = -1;
        /// <summary>
        /// ФНС_0006_ Форма 4НМ – ТГ, 0 месяц
        /// </summary>
        private int fns_0006_sourceid = -1;
        /// <summary>
        /// ФНС_0023_ Форма 4НОМ – ТГ, 0 месяц
        /// </summary>
        private int fns_0023_sourceid = -1;
        /// <summary>
        /// ФО_0051_ Финансовый паспорт МО_ТГ
        /// </summary>
        private int fo_0051_sourceid = -1;
        /// <summary>
        /// ФО_0046_Мониторинг местных бюджетов  – ТГ, 0 квартал
        /// </summary>
        private int fo_0046_sourceid = -1;

        #endregion

        #region Функция перевода базы на новый год

        public TransferDBToNewYearState TransferDBToNewYear(int currentYear)
        {
            // Год 1900-2099
            if (!Regex.IsMatch(currentYear.ToString(), @"(19|20)\d{2}"))
            {
                throw new ArgumentOutOfRangeException(string.Format("Некорректный год - {0}", currentYear));    
            }

            WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeBegin, "Начало работы функции перевода базы на новый год", 0);

            TransferDBToNewYearState addDataSourceState = AddNewDataSources(currentYear);
            TransferDBToNewYearState exportState = ExportImportClassifiers();
            TransferDBToNewYearState invalidateState = InvalidateObjects();

            WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeEnd, "Окончание работы функции перевода базы на новый год", 0);

            if (exportState == TransferDBToNewYearState.Error || addDataSourceState == TransferDBToNewYearState.Error || invalidateState == TransferDBToNewYearState.Error)
            {
                return TransferDBToNewYearState.Error;
            }

            if (exportState == TransferDBToNewYearState.SuccessfullyWithWarning || addDataSourceState == TransferDBToNewYearState.SuccessfullyWithWarning || invalidateState == TransferDBToNewYearState.SuccessfullyWithWarning)
            {
                return TransferDBToNewYearState.SuccessfullyWithWarning;
            }

            return TransferDBToNewYearState.Successfully;
        }

        #endregion Функция перевода базы на новый год

        #region Добавление источников

        private TransferDBToNewYearState AddNewDataSources(int currentYear)
        {
            bool withWarning = false;
            bool withError = false;

            // Очередной финансовый год
            int nextFinancialYear = currentYear + 1;

            #region ФО_0029_Проект бюджета – ОФГ

            fo_0029_sourceid = CreateDataSource(
                "Проект бюджета"
                , "29"
                , "ФО"
                , nextFinancialYear
                , ParamKindTypes.Year
                , 0
                , 0
                , ref withWarning
                , ref withError);

            #endregion


            #region ФО_0006_Анализ данных – ТГ

            fo_0006_sourceid = CreateDataSource(
                "Анализ данных"
                , "6"
                , "ФО"
                , currentYear
                , ParamKindTypes.Year
                , 0
                , 0
                , ref withWarning
                , ref withError);

            #endregion

            #region ФО_0016_Мониторинг БК и КУ – ТГ 0, 1, 2, 3, 4 квартал

            fo_0016_sourceid = CreateDataSource(
                    "Мониторинг БК и КУ"
                    , "16"
                    , "ФО"
                    , currentYear
                    , ParamKindTypes.YearQuarter
                    , 0
                    , 0
                    , ref withWarning
                    , ref withError);

            for (int i = 1; i < 5; i++)
            {
                CreateDataSource(
                    "Мониторинг БК и КУ"
                    , "16"
                    , "ФО"
                    , currentYear
                    , ParamKindTypes.YearQuarter
                    , i
                    , 0
                    , ref withWarning
                    , ref withError);
            }

            #endregion

            #region ФО_0021_Мониторинг ФП и КУ – ТГ 0, 1, 2, 3, 4 квартал.

            fo_0021_sourceid = CreateDataSource(
                   "Мониторинг ФП и КУ"
                   , "21"
                   , "ФО"
                   , currentYear
                   , ParamKindTypes.YearQuarter
                   , 0
                   , 0
                   , ref withWarning
                   , ref withError);

            for (int i = 1; i < 5; i++)
            {
                CreateDataSource(
                    "Мониторинг ФП и КУ"
                    , "21"
                    , "ФО"
                    , currentYear
                    , ParamKindTypes.YearQuarter
                    , i
                    , 0
                    , ref withWarning
                    , ref withError);
            }

            #endregion

            #region ФО_0039_Оценка качества ОиОБП – ТГ 0, 1, 2, 3, 4 квартал

            fo_0039_sourceid = CreateDataSource(
                    "Оценка качества ОиОБП"
                    , "39"
                    , "ФО"
                    , currentYear
                    , ParamKindTypes.YearQuarter
                    , 0
                    , 0
                    , ref withWarning
                    , ref withError);

            for (int i = 1; i < 5; i++)
            {
                CreateDataSource(
                    "Оценка качества ОиОБП"
                    , "39"
                    , "ФО"
                    , currentYear
                    , ParamKindTypes.YearQuarter
                    , i
                    , 0
                    , ref withWarning
                    , ref withError);
            }

            #endregion

            #region ФО_0027_Мониторинг месячного отчета – ТГ месяц

            fo_0027_sourceid = CreateDataSource(
                    "Мониторинг месячного отчета"
                    , "27"
                    , "ФО"
                    , currentYear
                    , ParamKindTypes.YearMonth
                    , 0
                    , 0
                    , ref withWarning
                    , ref withError);

            for (int i = 1; i < 13; i++)
            {
                CreateDataSource(
                    "Мониторинг месячного отчета"
                    , "27"
                    , "ФО"
                    , currentYear
                    , ParamKindTypes.YearMonth
                    , 0
                    , i
                    , ref withWarning
                    , ref withError);
            }

            #endregion

            #region ФО_0042_Оценка качества ФМ – ТГ

            fo_0042_sourceid = CreateDataSource(
                    "Оценка качества ФМ"
                    , "42"
                    , "ФО"
                    , currentYear
                    , ParamKindTypes.YearQuarter
                    , 0
                    , 0
                    , ref withWarning
                    , ref withError);

            for (int i = 1; i < 5; i++)
            {
                CreateDataSource(
                    "Оценка качества ФМ"
                    , "42"
                    , "ФО"
                    , currentYear
                    , ParamKindTypes.YearQuarter
                    , i
                    , 0
                    , ref withWarning
                    , ref withError);
            }

            #endregion

            #region МОФО_0019_Анализ МБТ – ТГ

            mofo_0019_sourceid = CreateDataSource(
                "Анализ МБТ"
                , "19"
                , "МОФО"
                , currentYear
                , ParamKindTypes.Year
                , 0
                , 0
                , ref withWarning
                , ref withError);

            #endregion

            #region ФНС_0015_Форма 1НОМ – ТГ, 0 месяц

            fns_0015_sourceid = CreateDataSource(
                "Форма 1НОМ"
                , "15"
                , "ФНС"
                , currentYear
                , ParamKindTypes.YearMonth
                , 0
                , 0
                , ref withWarning
                , ref withError);

            #endregion

            #region ФНС_0006_Форма 4НМ – ТГ, 0 месяц

            fns_0006_sourceid = CreateDataSource(
                "Форма 4НМ"
                , "6"
                , "ФНС"
                , currentYear
                , ParamKindTypes.YearMonth
                , 0
                , 0
                , ref withWarning
                , ref withError);

            #endregion

            #region ФНС_0023_Форма 4НОМ – ТГ, 0 месяц

            fns_0023_sourceid = CreateDataSource(
                "Форма 4НОМ"
                , "23"
                , "ФНС"
                , currentYear
                , ParamKindTypes.YearMonth
                , 0
                , 0
                , ref withWarning
                , ref withError);

            #endregion

            #region ФО_0051_ Финансовый паспорт МО_ТГ

            fo_0051_sourceid = CreateDataSource(
                "Финансовый паспорт МО"
                , "51"
                , "ФО"
                , currentYear
                , ParamKindTypes.Year
                , 0
                , 0
                , ref withWarning
                , ref withError);

            #endregion

            #region ФО_0046_Мониторинг местных бюджетов  – ТГ, 0 квартал

            fo_0046_sourceid = CreateDataSource(
                "Мониторинг местных бюджетов"
                , "46"
                , "ФО"
                , currentYear
                , ParamKindTypes.YearQuarter
                , 0
                , 0
                , ref withWarning
                , ref withError);

            #endregion


            if (withError)
                return TransferDBToNewYearState.Error;

            return withWarning
                       ? TransferDBToNewYearState.SuccessfullyWithWarning
                       : TransferDBToNewYearState.Successfully;
        }

        /// <summary>
        /// Создание источника
        /// </summary>
        /// <param name="dataName">Наименование поступающей информации</param>
        /// <param name="dataCode">Порядковый номер поступающей информации</param>
        /// <param name="supplierCode">Код поставщика данных</param>
        /// <param name="year">Вид параметров:год</param>
        /// <param name="paramKindType">Вид параметров источника данных</param>
        /// <param name="quarter">Вид параметров:квартал</param>
        /// <param name="month">Вид параметров:месяц</param>
        /// <param name="withWarning">Добавление источника с предупреждениями</param>
        /// <param name="withError">Добавление источника с ошибками</param>
        /// <returns>ID нового источника</returns>
        private int CreateDataSource(string dataName, string dataCode, string supplierCode, int year, ParamKindTypes paramKindType, int quarter, int month, ref bool withWarning, ref bool withError)
        {
            IDataSource dataSource = SchemeClass.Instance.DataSourceManager.DataSources.CreateElement();
            try
            {
                dataSource.DataName = dataName;
                dataSource.DataCode = dataCode;
                dataSource.SupplierCode = supplierCode;
                dataSource.Year = year;
                dataSource.ParametersType = paramKindType;
                dataSource.Month = month;
                dataSource.Quarter = quarter;
                int? varSourceID = SchemeClass.Instance.DataSourceManager.DataSources.FindDataSource(dataSource);
                if (varSourceID != null)
                {
                    WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeWarning,
                                      String.Format("Источник {0} был создан ранее.", SchemeClass.Instance.DataSourceManager.GetDataSourceName((int)varSourceID)),
                                      (int)varSourceID);
                    withWarning = true;
                    return (int)varSourceID;
                }

                varSourceID = SchemeClass.Instance.DataSourceManager.DataSources.Add(dataSource);
                WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeCreateSource,
                                  String.Format("Создание источника - {0}",
                                                SchemeClass.Instance.DataSourceManager.GetDataSourceName(
                                                    (int)varSourceID)), (int)varSourceID);

                return (int)varSourceID;
            }
            catch (Exception e)
            {
                WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeError,
                                  String.Format("При создании источника возникло исключение {0}",
                                                Krista.Diagnostics.KristaDiagnostics.ExpandException(e)), 0);
                withError = true;
                return -1;
            }
        }

        #endregion Добавление источников

        #region Экспорт/импорт классификаторов

        delegate void ExportImportSourceDelegate(ref bool withWarning, ref bool withError);
        delegate bool ExportImportClassifierDelegate(IExportImporter exportImporter, string clsKey, string clsName,
                                                   int clsSourceId, string uniqueAttributesNames, ref bool withWarning, ref bool withError);

        private TransferDBToNewYearState ExportImportClassifiers()
        {
            bool withWarning = false;
            bool withError = false;

            // ФО_0029_Проект бюджета – ОФГ
            ExportImportSourceDelegateMethod(ExportImportFO_0029Classifiers, ref withWarning, ref withError);
            // ФО_Анализ данных – ТГ
            ExportImportSourceDelegateMethod(ExportImportFO_0006Classifiers, ref withWarning, ref withError);
            // МОФО_0019_Анализ МБТ
            ExportImportSourceDelegateMethod(ExportImportMOFO_0019Classifiers, ref withWarning, ref withError);
            // ФО_0016_Мониторинг БК и КУ – ТГ
            ExportImportSourceDelegateMethod(ExportImportFO_0016Classifiers, ref withWarning, ref withError);
            // ФО_0021_Мониторинг ФП и КУ – ТГ 
            ExportImportSourceDelegateMethod(ExportImportFO_0021Classifiers, ref withWarning, ref withError);
            // ФО_0039_Оценка качества ОиОБП – ТГ
            ExportImportSourceDelegateMethod(ExportImportFO_0039Classifiers, ref withWarning, ref withError);
            // ФО_0027_Мониторинг месячного отчета – ТГ
            ExportImportSourceDelegateMethod(ExportImportFO_0027Classifiers, ref withWarning, ref withError);
            // ФО_0042_ Оценка качества ФМ – ТГ
            ExportImportSourceDelegateMethod(ExportImportFO_0042Classifiers, ref withWarning, ref withError);
            // ФНС_0015_Форма 1НОМ – ТГ
            ExportImportSourceDelegateMethod(ExportImportFNS_0015Classifiers, ref withWarning, ref withError);
            // ФНС_0006_ Форма 4НМ – ТГ
            ExportImportSourceDelegateMethod(ExportImportFNS_0006Classifiers, ref withWarning, ref withError);
            // ФНС_0023_ Форма 4НОМ– ТГ
            ExportImportSourceDelegateMethod(ExportImportFNS_0023Classifiers, ref withWarning, ref withError);
            // ФО_0051_ Финансовый паспорт МО_ТГ
            ExportImportSourceDelegateMethod(ExportImportFO_0051Classifiers, ref withWarning, ref withError);
            // ФО_0046_Мониторинг местных бюджетов  – ТГ, 0 квартал.
            ExportImportSourceDelegateMethod(ExportImportFO_0046Classifiers, ref withWarning, ref withError);
            
            if (withError)
                return TransferDBToNewYearState.Error;

            return withWarning
                       ? TransferDBToNewYearState.SuccessfullyWithWarning
                       : TransferDBToNewYearState.Successfully;
        }

        private void ExportImportSourceDelegateMethod(ExportImportSourceDelegate action, ref bool withWarning, ref bool withError)
        {
            action.Invoke(ref withWarning, ref withError);
        }

        private bool ExportImportClassifierDelegateMethod(ExportImportClassifierDelegate action, string clsKey, string clsName,
                                                   int clsSourceId, string uniqueAttributesNames, ref bool withWarning, ref bool withError)
        {
            IEntity entity = SchemeClass.Instance.RootPackage.FindEntityByName(clsKey);
            if (entity == null)
            {
                /*WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeWarning,
                                  String.Format("Классификатор {0} не найден в схеме.", clsName),
                                  clsSourceId);*/

                Trace.TraceWarning(String.Format("Классификатор {0} не найден в схеме.", clsName));
                withWarning = true;
                return false;
            }

            // для каждого классификатора создаем новые параметры экспорта
            using (IExportImporter exportImporter = SchemeClass.Instance.GetXmlExportImportManager().GetExportImporter(ObjectType.Classifier))
            {
                return action.Invoke(exportImporter, clsKey, clsName, clsSourceId, uniqueAttributesNames, ref withWarning, ref withError);
            }
        }

        /// <summary>
        /// Экспорт классификаторов по источнику ФО_0029_Проект бюджета – ОФГ
        /// </summary>
        /// <param name="exportImporter"></param>
        /// <param name="withWarning"></param>
        /// <param name="withError"></param>
        private void ExportImportFO_0029Classifiers(ref bool withWarning, ref bool withError)
        {
            if (fo_0029_sourceid == -1)
                return;

            #region Перенос классификаторов по источнику ФО_0029_Проект бюджета – ОФГ

            // Налоги.Объекты налогообложения
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "804890b3-9887-423c-83db-b4c386550395"
                , "Налоги.Объекты налогообложения"
                , fo_0029_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("804890b3-9887-423c-83db-b4c386550395", "Налоги.Объекты налогообложения",
                                    ref withWarning, ref withError);
            }

            // Организации.Планирование
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "aeabb871-e583-439b-a329-b1f6ce78f212"
                , "Организации.Планирование"
                , fo_0029_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("aeabb871-e583-439b-a329-b1f6ce78f212", "Организации.Планирование", ref withWarning,
                                    ref withError);
            }

            // КД.Планирование
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "a6e33772-325a-4932-a0aa-7ce82f0b3921"
                , "КД.Планирование"
                , fo_0029_sourceid
                , "Name, CodeStr"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("a6e33772-325a-4932-a0aa-7ce82f0b3921", "КД.Планирование", ref withWarning,
                                    ref withError);
            }

            // Районы.Планирование
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "1f34cc90-16fd-4fcf-b994-0c8a680d7e23"
                , "Районы.Планирование"
                , fo_0029_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("1f34cc90-16fd-4fcf-b994-0c8a680d7e23", "Районы.Планирование", ref withWarning,
                                    ref withError);
            }

            // Администратор.Планирование
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "dd69b4e1-f257-49ce-b553-442d094ae39a"
                , "Администратор.Планирование"
                , fo_0029_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("dd69b4e1-f257-49ce-b553-442d094ae39a", "Администратор.Планирование",
                                    ref withWarning, ref withError);
            }

            // Показатели. Экономические показатели
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "2b00869f-0524-427a-a7f8-7d2750f85984"
                , "Показатели.Экономические показатели"
                , fo_0029_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("2b00869f-0524-427a-a7f8-7d2750f85984", "Показатели.Экономические показатели",
                                    ref withWarning, ref withError);
            }

            // Показатели. Объекты налогообложения
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "90f457ed-92af-49b9-9e89-3d589deea377"
                , "Показатели.Объекты налогообложения"
                , fo_0029_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("90f457ed-92af-49b9-9e89-3d589deea377", "Показатели.Объекты налогообложения",
                                    ref withWarning, ref withError);
            }

            // КИФ.Планирование
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "a531f087-b785-4ab6-8934-6f7f29ea4660"
                , "КИФ.Планирование"
                , fo_0029_sourceid
                , "Name, CodeStr"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("a531f087-b785-4ab6-8934-6f7f29ea4660", "КИФ.Планирование", ref withWarning,
                                    ref withError);
            }

            // Мероприятия.МОФО_Дополнительные мероприятия
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "423dc740-e94a-461b-9e74-044fff6d254d"
                , "Мероприятия.МОФО_Дополнительные мероприятия"
                , fo_0029_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("423dc740-e94a-461b-9e74-044fff6d254d",
                                    "Мероприятия.МОФО_Дополнительные мероприятия", ref withWarning, ref withError);
            }

            // Показатели. МОФО_Дополнительные мероприятия
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "7523e440-b2bc-42f7-83ad-d51ab936d935"
                , "Показатели.МОФО_Дополнительные мероприятия"
                , fo_0029_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("7523e440-b2bc-42f7-83ad-d51ab936d935", "Показатели.МОФО_Дополнительные мероприятия",
                                    ref withWarning, ref withError);
            }

            #endregion
        }

        private void ExportImportFO_0006Classifiers(ref bool withWarning, ref bool withError)
        {
            if (fo_0006_sourceid == -1)
                return;

            // КД.Анализ
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "2553274b-4cee-4d20-a9a6-eef173465d8b"
                , "КД.Анализ"
                , fo_0006_sourceid
                , "Name, CodeStr"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("2553274b-4cee-4d20-a9a6-eef173465d8b", "КД.Анализ", ref withWarning, ref withError);
            }

            // Районы.Анализ
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "383f887a-3ebb-4dba-8abb-560b5777436f"
                , "Районы.Анализ"
                , fo_0006_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("383f887a-3ebb-4dba-8abb-560b5777436f", "Районы.Анализ", ref withWarning,
                                    ref withError);
            }

            // Администратор.Анализ
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "bb3d9a88-8088-49ba-abcd-85c53e29ca57"
                , "Администратор.Анализ"
                , fo_0006_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("bb3d9a88-8088-49ba-abcd-85c53e29ca57", "Администратор.Анализ", ref withWarning,
                                    ref withError);
            }

            // Районы.Служебный для закачки СКИФ
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "e9a95119-21f1-43d8-8dc2-8d4af7c195d0"
                , "Районы.Служебный для закачки СКИФ"
                , fo_0006_sourceid
                , "Name, CodeStr"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("e9a95119-21f1-43d8-8dc2-8d4af7c195d0", "Районы.Служебный для закачки СКИФ",
                                    ref withWarning, ref withError);
            }

            // Районы.Служебный для закачки
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "e9d2898d-fc2d-4626-834a-ed1ac98a1673"
                , "Районы.Служебный для закачки"
                , fo_0006_sourceid
                , "OKATO, Name"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("e9d2898d-fc2d-4626-834a-ed1ac98a1673", "Районы.Служебный для закачки",
                                    ref withWarning, ref withError);
            }
        }

        private void ExportImportMOFO_0019Classifiers(ref bool withWarning, ref bool withError)
        {
            if (mofo_0019_sourceid == -1)
                return;

            // Показатели.МОФО_Анализ МБТ
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "44c21391-6dd0-4cfd-b92f-b4fc5e47eec4"
                , "Показатели.МОФО_Анализ МБТ"
                , mofo_0019_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("44c21391-6dd0-4cfd-b92f-b4fc5e47eec4", "Показатели.МОФО_Анализ МБТ",
                                    ref withWarning, ref withError);
            }

            // Субвенции.МОФО_Анализ МБТ
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "59cc697d-50bb-4906-ad75-2319cf49544b"
                , "Субвенции.МОФО_Анализ МБТ"
                , mofo_0019_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("59cc697d-50bb-4906-ad75-2319cf49544b", "Субвенции.МОФО_Анализ МБТ", ref withWarning,
                                    ref withError);
            }
        }

        private void ExportImportFO_0016Classifiers(ref bool withWarning, ref bool withError)
        {
            if (fo_0016_sourceid == -1)
                return;

            // Показатели.БККУ
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "9b24eb23-d5e2-4c66-a18f-c7e0ba1dbebc"
                , "Показатели.БККУ"
                , fo_0016_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("9b24eb23-d5e2-4c66-a18f-c7e0ba1dbebc", "Показатели.БККУ", ref withWarning,
                                    ref withError);
            }
        }

        private void ExportImportFO_0021Classifiers(ref bool withWarning, ref bool withError)
        {
            if (fo_0021_sourceid == -1)
                return;

            // Показатели.ФО_ФПКУ
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "36544887-1e2e-4fe9-8417-329fafbd5fda"
                , "Показатели.ФО_ФПКУ"
                , fo_0021_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("36544887-1e2e-4fe9-8417-329fafbd5fda", "Показатели.ФО_ФПКУ", ref withWarning,
                                    ref withError);
            }
        }

        private void ExportImportFO_0039Classifiers(ref bool withWarning, ref bool withError)
        {
            if (fo_0039_sourceid == -1)
                return;

            // Показатели.Оценка качества ОиОБП
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "42634373-98b3-48dd-ba61-e25eb8ee1112"
                , "Показатели.Оценка качества ОиОБП"
                , fo_0039_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("42634373-98b3-48dd-ba61-e25eb8ee1112", "Показатели.Оценка качества ОиОБП",
                                    ref withWarning, ref withError);
            }
        }

        private void ExportImportFO_0027Classifiers(ref bool withWarning, ref bool withError)
        {
            if (fo_0027_sourceid == -1)
                return;

            // Показатели.БККУ_МесОтч
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "2861694c-6f51-40b2-a08c-f0d6f337b1f8"
                , "Показатели.БККУ_МесОтч"
                , fo_0027_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("2861694c-6f51-40b2-a08c-f0d6f337b1f8", "Показатели.БККУ_МесОтч", ref withWarning,
                                    ref withError);
            }
        }

        private void ExportImportFO_0042Classifiers(ref bool withWarning, ref bool withError)
        {
            if (fo_0042_sourceid == -1)
                return;

            // Показатели.Оценка качества ФМ
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "bac04429-05eb-46cc-ad00-9a31f9a68713"
                , "Показатели.Оценка качества ФМ"
                , fo_0042_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("bac04429-05eb-46cc-ad00-9a31f9a68713", "Показатели.Оценка качества ФМ",
                                    ref withWarning, ref withError);
            }
        }

        private void ExportImportFNS_0015Classifiers(ref bool withWarning, ref bool withError)
        {
            if (fns_0015_sourceid == -1)
                return;

            // Доходы.Группы ФНС
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "b9169eb6-de81-420b-8a2b-05ffa2fd35c1"
                , "Доходы.Группы ФНС"
                , fns_0015_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("b9169eb6-de81-420b-8a2b-05ffa2fd35c1", "Доходы.Группы ФНС", ref withWarning,
                                    ref withError);
            }
        }

        private void ExportImportFNS_0006Classifiers(ref bool withWarning, ref bool withError)
        {
            if (fns_0006_sourceid == -1)
                return;

            // Доходы.Группы ФНС
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "b9169eb6-de81-420b-8a2b-05ffa2fd35c1"
                , "Доходы.Группы ФНС"
                , fns_0006_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("b9169eb6-de81-420b-8a2b-05ffa2fd35c1", "Доходы.Группы ФНС", ref withWarning,
                    ref withError);
            }
        }

        private void ExportImportFNS_0023Classifiers(ref bool withWarning, ref bool withError)
        {
            if (fns_0023_sourceid == -1)
                return;

            // Доходы.Группы ФНС
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "b9169eb6-de81-420b-8a2b-05ffa2fd35c1"
                , "Доходы.Группы ФНС"
                , fns_0023_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("b9169eb6-de81-420b-8a2b-05ffa2fd35c1", "Доходы.Группы ФНС", ref withWarning,
                    ref withError);
            }

            // Задолженность.ФНС
            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "516ec293-bf4c-4ff8-a2c5-bc04acb70a81"
                , "Задолженность.ФНС"
                , fns_0023_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("516ec293-bf4c-4ff8-a2c5-bc04acb70a81", "Задолженность.ФНС", ref withWarning,
                                    ref withError);
            }
        }

        private void ExportImportFO_0046Classifiers(ref bool withWarning, ref bool withError)
        {
            if (fo_0046_sourceid == -1)
                return;

            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
                , "42a22625-2cca-4c95-ae5c-981cdc4d1f6a"
                , "Показатели.Мониторинг местных бюджетов"
                , fo_0046_sourceid
                , "Name, Code"
                , ref withWarning
                , ref withError))
            {
                InvalidateDimension("42a22625-2cca-4c95-ae5c-981cdc4d1f6a", "Показатели.Мониторинг местных бюджетов",
                                    ref withWarning, ref withError);
            }
        }

        private void ExportImportFO_0051Classifiers(ref bool withWarning, ref bool withError)
        {
            if (fo_0051_sourceid == -1)
                return;

            if (ExportImportClassifierDelegateMethod(
                ExportImportClassifier
               , "05e7ad82-8d16-4563-ab7e-5f5d00e81aab"
               , "Показатели.Паспорт МО"
               , fo_0051_sourceid
               , "Name, Code"
               , ref withWarning
               , ref withError))
            {
                InvalidateDimension("05e7ad82-8d16-4563-ab7e-5f5d00e81aab", "Показатели.Паспорт МО", ref withWarning,
                                    ref withError);
            }
        }

        /// <summary>
        /// Экспортирует классификатор по последнему источнику и импортирует на источник нового года
        /// </summary>
        /// <param name="exportImporter"></param>
        /// <param name="clsKey">Уникальный идентификатор классификатора</param>
        /// <param name="clsName">Русское наименование</param>
        /// <param name="clsSourceId">Источник нового года</param>
        /// <param name="uniqueAttributesNames">Атрибуты для обновления</param>
        /// <param name="withWarning"></param>
        /// <param name="withError"></param>
        private bool ExportImportClassifier(IExportImporter exportImporter, string clsKey, string clsName,
                                                   int clsSourceId, string uniqueAttributesNames, ref bool withWarning, ref bool withError)
        {
            try
            {
                IEntity entity = SchemeClass.Instance.RootPackage.FindEntityByName(clsKey);
                if (entity == null)
                {
                    WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeWarning,
                                      String.Format("Классификатор {0} не найден в схеме.", clsName),
                                      clsSourceId);
                    withWarning = true;
                    return false;
                }

                // Проверяем, есть ли данные в классификаторе по новому источнику
                // Если данные есть, ничего с этим классификатором н еделаем
                using (IDataUpdater dataUpdater = entity.GetDataUpdater(String.Format("sourceID = {0} and RowType = 0", clsSourceId), null))
                {
                    DataTable table = new DataTable();
                    dataUpdater.Fill(ref table);
                    if (table.Rows.Count > 0)
                    {
                        WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeWarning,
                                          String.Format("В классификаторе {0} по источику {1} уже есть данные.", clsName, clsSourceId),
                                          clsSourceId);
                        withWarning = true;
                        return false;
                    }
                }

                // Источник нового года
                IDataSource dataSource = SchemeClass.Instance.DataSourceManager.DataSources[clsSourceId];
                if (dataSource == null)
                {
                    WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeWarning,
                                      String.Format("Источник не {0} найден.", clsSourceId),
                                      clsSourceId);
                    withWarning = true;
                    return false;
                }

                int lastSourceID = GetLastSourceId(entity, dataSource);
                if (lastSourceID == -1)
                {
                    WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeWarning,
                                      String.Format("В классификаторе {0} не найден источник для операции экспорта/импорта.", entity.FullCaption),
                                      clsSourceId);
                    withWarning = true;
                    return false;
                }

                string tempFileName = Path.Combine(Path.GetTempPath(), GetFileName(entity.FullCaption, clsSourceId) + ".xml");

                // Экспорт.
                ImportPatams importParams = new ImportPatams
                {
                    refreshDataByAttributes = true,
                    uniqueAttributesNames = uniqueAttributesNames
                };

                try
                {
                    using (FileStream stream = new FileStream(tempFileName, FileMode.Create))
                    {
                        string dataQuery = String.Format("(RowType = 0) and (SourceID = {0}) and (id > -1)",
                                                         lastSourceID);

                        ExportImportClsParams exportImportParams = new ExportImportClsParams();
                        exportImportParams.DataSource = lastSourceID;
                        exportImportParams.Filter = dataQuery;
                        exportImportParams.FilterParams = null;
                        exportImportParams.TaskID = -1;
                        exportImportParams.ClsObjectKey = clsKey;
                        exportImportParams.SelectedRowsId = new List<int>();

                        exportImporter.ExportData(stream, importParams, exportImportParams);

                        WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeExportClassifierData,
                                          String.Format(
                                              "Классификатор {0} экспортирован во временный файл {1} по источнику {2}",
                                              clsName, tempFileName,
                                              SchemeClass.Instance.DataSourceManager.GetDataSourceName(lastSourceID)),
                                          lastSourceID);
                    }

                }
                catch (Exception e)
                {
                    WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeError,
                                      String.Format(
                                          "При экпорте классификатора {0} по источнику {1} возникло исключеие {2}.",
                                          entity.FullCaption,
                                          SchemeClass.Instance.DataSourceManager.GetDataSourceName(lastSourceID),
                                          Krista.Diagnostics.KristaDiagnostics.ExpandException(e)),
                                      clsSourceId);
                    withError = true;
                    return false;
                }

                // Импорт. 

                try
                {
                    using (FileStream stream = new FileStream(tempFileName, FileMode.Open, FileAccess.Read))
                    {
                        ExportImportClsParams exportImportParams = new ExportImportClsParams
                        {
                            DataSource = clsSourceId,
                            Filter = null,
                            FilterParams = null,
                            TaskID = -1,
                            ClsObjectKey = clsKey,
                            RefVariant = -1
                        };

                        exportImporter.ImportData(stream, exportImportParams);
                        WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeImportClassifierData,
                                          String.Format(
                                              "Классификатор {0} импортирован из временного файла {1} по источнику {2}",
                                              clsName, tempFileName,
                                              SchemeClass.Instance.DataSourceManager.GetDataSourceName(clsSourceId)),
                                          lastSourceID);
                    }
                }
                catch (Exception e)
                {
                    WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeError,
                                      String.Format(
                                          "При импорте классификатора {0} по источнику {1} возникло исключеие {2}.",
                                          entity.FullCaption,
                                          SchemeClass.Instance.DataSourceManager.GetDataSourceName(clsSourceId),
                                          Krista.Diagnostics.KristaDiagnostics.ExpandException(e)),
                                      clsSourceId);
                    withError = true;
                    return false;
                }

                // Удаляем временный файл
                File.Delete(tempFileName);

                return true;
            }
            catch (Exception e)
            {
                WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeError,
                                  String.Format("При экспорте/импорте классификатора {0} возникло исключение {1}", clsName, e), 0);
                withError = true;
                return false;
            }
        }

        private static string GetFileName(string fullCaption, int sourceid)
        {
            string currentFilterCaption = SchemeClass.Instance.DataSourceManager.GetDataSourceName(sourceid);
            if (currentFilterCaption != string.Empty)
            {
                currentFilterCaption = currentFilterCaption.Replace('\\', '_');
                return fullCaption + '_' + currentFilterCaption;
            }
            return fullCaption;
        }

        /// <summary>
        /// Находим по параметру источника последний заполненный источник
        /// </summary>
        /// <param name="entity">Классификатор</param>
        /// <param name="dataSource">Источник нового года</param>
        /// <returns></returns>
        private static int GetLastSourceId(IEntity entity, IDataSource dataSource)
        {
            int lastSourceID = -1;
            using (IDatabase db = SchemeClass.Instance.SchemeDWH.DB)
            {
                int lastYear = 0;
                int lastQuater = 0;
                int lastMonth = 0;
                IDbCommand cmd = ((Database)db).InitCommand(null);
                cmd.CommandText = String.Format("select distinct SourceID from {0} {1}", entity.FullDBName, "where ID <> -1 and RowType = 0");
                //conn.Open();
                IDataReader dr = cmd.ExecuteReader();

                try
                {
                    while (dr.Read())
                    {
                        IDataSource ds =
                            SchemeClass.Instance.DataSourceManager.DataSources[Convert.ToInt32(dr.GetValue(0))];
                        if (ds == null)
                            continue;

                        if (ds.DataCode != dataSource.DataCode || ds.SupplierCode != dataSource.SupplierCode ||
                            ds.DataName != dataSource.DataName || ds.ParametersType != dataSource.ParametersType)
                            continue;

                        lastSourceID = GetLastSourceId(ds, dataSource, ref lastYear, ref lastQuater, ref lastMonth,
                                                       lastSourceID);
                    }
                }
                finally
                {
                    dr.Close();
                }

                try
                {
                    // Если не нашли источник с такими же параметрами, но за более ранний период
                    // то ищем прсто последний источник
                    if (lastSourceID == -1)
                    {
                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            IDataSource ds = SchemeClass.Instance.DataSourceManager.DataSources[Convert.ToInt32(dr.GetValue(0))];
                            if (ds == null)
                                continue;

                            lastSourceID = GetLastSourceId(ds, dataSource, ref lastYear, ref lastQuater, ref lastMonth,
                                                           lastSourceID);
                        }
                    }
                }
                finally
                {
                    dr.Close();
                }
            }

            return lastSourceID;
        }

        private static int GetLastSourceId(IDataSource ds, IDataSource dataSource, ref int lastYear, ref int lastQuater, ref int lastMonth, int lastSourceID)
        {
            switch (dataSource.ParametersType)
            {
                case ParamKindTypes.Year:
                    if (ds.Year > lastYear && ds.Year < dataSource.Year)
                    {
                        lastYear = ds.Year;
                        lastSourceID = ds.ID;
                    }
                    break;
                case ParamKindTypes.YearQuarter:
                    if (ds.Year >= lastYear && ds.Quarter >= lastQuater && ds.Year < dataSource.Year)
                    {
                        lastYear = ds.Year;
                        lastQuater = ds.Quarter;
                        lastSourceID = ds.ID;
                    }
                    break;
                case ParamKindTypes.YearMonth:
                    if (ds.Year >= lastYear && ds.Month >= lastMonth && ds.Year < dataSource.Year)
                    {
                        lastYear = ds.Year;
                        lastMonth = ds.Month;
                        lastSourceID = ds.ID;
                    }
                    break;
                default:
                    WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeWarning,
                                      String.Format("Необработанный параметр источника {0}.", dataSource.ParametersType),
                                      dataSource.ID);
                    break;
            }

            return lastSourceID;
        }

        #endregion Экспорт/импорт классификаторов

        #region Требование на расчет

        /// <summary>
        /// Обработанным классификаторам выставляем требование на расчет
        /// </summary>
        /// <returns></returns>
        private static TransferDBToNewYearState InvalidateObjects()
        {
            bool withWarning = false;
            bool withError = false;

            // Выставляем требование на расчет кубам из постановки

            // ФО_Оценка качества ФМ_Показатели
            InvalidateCube("1fae5902-07e5-4e3a-a5c6-32e5020f0672", "ФО_Оценка качества ФМ_Показатели", ref withWarning,
                           ref withError);

            // ФО_Оценка качества ФМ_Исходные данные
            InvalidateCube("5fac684c-5434-467c-a858-ec2b957484cb", "ФО_Оценка качества ФМ_Исходные данные",
                           ref withWarning, ref withError);

            // ФО_Оценка качества ОиОБП_Показатели
            InvalidateCube("45dc2bed-eabe-4bca-83e1-8c5ce6633a08", "ФО_Оценка качества ОиОБП_Показатели", ref withWarning,
                           ref withError);

            // ФО_Оценка качества ОиОБП_Исходные данные
            InvalidateCube("0351e4b5-b577-46ba-b026-a53e4129f915", "ФО_Оценка качества ОиОБП_Исходные данные",
                           ref withWarning, ref withError);

            // ФО_БККУ_МесОтч
            InvalidateCube("f86b0bb9-070f-471c-b4c6-2d2c57ddb8e1", "ФО_БККУ_МесОтч", ref withWarning, ref withError);

            // ФО_ФПКУ_Показатели
            InvalidateCube("8657caec-d2d6-4bca-a750-86c5d1596c88", "ФО_ФПКУ_Показатели", ref withWarning, ref withError);

            // ФО_ФПКУ_Исходные данные
            InvalidateCube("a64dc569-a873-4b9e-8df2-06bde39425b0", "ФО_ФПКУ_Исходные данные", ref withWarning,
                           ref withError);

            // ФО_БККУ_Показатели
            InvalidateCube("734d1ed4-70fd-4783-858e-f4a513039daa", "ФО_БККУ_Показатели", ref withWarning, ref withError);

            // ФО_БККУ_Исходные данные
            InvalidateCube("30bef692-9e0f-412e-8248-b3a5c213f008", "ФО_БККУ_Исходные данные", ref withWarning,
                           ref withError);

            // ФО_Показатели к проекту доходов
            InvalidateCube("ae037b7d-ed0c-4f26-b91f-82bf30861f37", "ФО_Показатели к проекту доходов", ref withWarning,
                           ref withError);

            // ФО_Проект доходов без расщепления
            InvalidateCube("8d3bf2e7-1aaf-419a-9e3a-4ab7ec9f89c5", "ФО_Проект доходов без расщепления", ref withWarning,
                           ref withError);

            // ФО_Результат доходов без расщепления
            InvalidateCube("80319561-787b-4791-a85d-5a26b7a1c19f", "ФО_Результат доходов без расщепления",
                           ref withWarning, ref withError);

            // ФО_Результат доходов с расщеплением
            InvalidateCube("3f71b13b-3e87-45ad-8f72-1d023da07d10", "ФО_Результат доходов с расщеплением",
                           ref withWarning, ref withError);

            // ФНС_4 НОМ_Районы
            InvalidateCube("e62bcbc0-3f8d-4e0c-93bd-da6ae7bf5753", "ФНС_4 НОМ_Районы", ref withWarning, ref withError);

            // ФНС_4 НОМ_Сводный
            InvalidateCube("63eb10c5-1626-4d46-a377-8ac2f2c24902", "ФНС_4 НОМ_Сводный", ref withWarning, ref withError);

            // ФНС_1 НОМ_Сводный
            InvalidateCube("285e06aa-f281-4945-b50e-9876f89424d5", "ФНС_1 НОМ_Сводный", ref withWarning, ref withError);

            // ФНС_1 НОМ_Районы 
            InvalidateCube("167b2c71-2549-447a-a177-75fd16e21db6", "ФНС_1 НОМ_Районы", ref withWarning, ref withError);

            // ФНС_4 НМ_Районы
            InvalidateCube("b51ee6f4-9a3f-4950-a76b-53661b610bd3", "ФНС_4 НМ_Районы", ref withWarning, ref withError);

            // ФНС_4 НМ_Сводный 
            InvalidateCube("8b5517d1-79ba-4fdd-8259-411e220540d5", "ФНС_4 НМ_Сводный", ref withWarning, ref withError);

            // ФО_Показатели_Финпаспорт МО
            InvalidateCube("bdefaedf-2eab-4bbb-ada3-725ad36533f3", "Показатели.ФО_Паспорт МО", ref withWarning,
                           ref withError);

            // ФО_Мониторинг местных бюджетов_Показатели
            InvalidateCube("e88457bc-ae76-44e6-a4d5-a02475b93cff", "Показатели.ФО_Мониторинг местных бюджетов",
                           ref withWarning, ref withError);

            if (withError)
                return TransferDBToNewYearState.Error;

            return withWarning
                       ? TransferDBToNewYearState.SuccessfullyWithWarning
                       : TransferDBToNewYearState.Successfully;
        }

        /// <summary>
        /// Выставление требования на расчет измерению
        /// </summary>
        /// <param name="clsObjectKey">Уникальный идентификатор</param>
        /// <param name="clsName">Наименование</param>
        /// <param name="withWarning"></param>
        /// <param name="withError"></param>
        private static void InvalidateDimension(string clsObjectKey, string clsName, ref bool withWarning, ref bool withError)
        {
            try
            {
                IEntity entity = SchemeClass.Instance.RootPackage.FindEntityByName(clsObjectKey);
                if (entity == null)
                {
                    /*WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeWarning,
                                      String.Format("Классификатор {0} не найден в схеме.", clsName),
                                      0);*/

                    Trace.TraceWarning(String.Format("Классификатор {0} не найден в схеме.", clsName),
                                      0);
                    withWarning = true;
                    return;
                }

                SchemeClass.Instance.Processor.InvalidateDimension(entity, "Krista.FM.Server.Scheme.Classes.Classifier", InvalidateReason.ClassifierChanged, entity.OlapName);
                WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeInvalidateDimension,
                                  String.Format("Измерению {0} выставлено требование на расчет", entity.OlapName), 0);
            }
            catch (Exception e)
            {
                WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeError,
                                  String.Format(
                                      "При выставлении измерению {0} требования на расчет возникло исключение {1}",
                                      clsName, Krista.Diagnostics.KristaDiagnostics.ExpandException(e)), 0);
                withError = true;
            }
        }

        /// <summary>
        /// Отправка куба на расчет
        /// </summary>
        /// <param name="clsObjectKey"></param>
        /// <param name="cubeName"></param>
        /// <param name="withWarning"></param>
        /// <param name="withError"></param>
        private static void InvalidateCube(string clsObjectKey, string cubeName, ref bool withWarning, ref bool withError)
        {
            try
            {
                // Могут конечно быть виртуальные кубы...
                IEntity entity = SchemeClass.Instance.RootPackage.FindEntityByName(clsObjectKey);
                if (entity == null)
                {
                    /*WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeWarning,
                                      String.Format("Таблица фактов {0} не найдена в схеме.", clsObjectKey),
                                      0);*/

                    Trace.TraceWarning(String.Format("Таблица фактов {0} не найдена в схеме. Куб {1} не будет отправлен на расчет", clsObjectKey, cubeName),
                                      0);
                    withWarning = true;
                    return;
                }

                SchemeClass.Instance.Processor.InvalidatePartition(entity, "Krista.FM.Server.Scheme.Classes.Classifier", InvalidateReason.UserPleasure, cubeName);
                try
                {
                    WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeInvalidateCube,
                                      String.Format("Куб {0} отправлен на расчет", cubeName), 0);
                }
                catch (Exception)
                {
                    // У скл сервера иногда происходит трабл в процедуре usp_Generator, 
                    // Когда для одной таблицы ее вызывают из разных транзакций, 
                    // переодически возникает deadlock. Чтобы не пугать пользователя глушим тут ошибку
                }
            }
            catch (Exception e)
            {
                WriteIntoProtocol(TransferDBToNewYearEventKind.tnyeError,
                                  String.Format("При отправке куба {0} на расчет возникло исключение {1}", cubeName,
                                                Krista.Diagnostics.KristaDiagnostics.ExpandException(e)), 0);
                withError = true;
            }
        }

        #endregion Требование на расчет

        #region Протоколирование

        /// <summary>
        /// Запись события в протокол переходда на новый год
        /// </summary>
        /// <param name="kind">Тип события</param>
        /// <param name="eventMsg">Текст сообщения</param>
        /// <param name="sourceID"></param>
        private static void WriteIntoProtocol(TransferDBToNewYearEventKind kind, string eventMsg, int sourceID)
        {
            ITransferDBToNewYearProtocol log = null;
            try
            {
                log = (ITransferDBToNewYearProtocol)SchemeClass.Instance.GetProtocol("Krista.FM.Server.Scheme.dll");
                log.WriteEventIntoTransferDBToNewYearProtocol(kind, sourceID, eventMsg);
            }
            finally
            {
                if (log != null)
                {
                    log.Dispose();
                }
            }
        }

        #endregion Протоколирование
    }

    #endregion
}

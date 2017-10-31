using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps.LeasePump
{
    /// <summary>
    /// УФК_0004_Аренда
    /// Отчет о поступлении доходов в республиканский бюджет от продажи и использования имущества, 
    /// находящегося в государственной собственности, или от деятельности государственных организаций, 
    /// контролируемых Минимуществом РТ (.xls)
    /// </summary>
    public class LeasePumpModule : DataPumpModuleBase
    {
        #region Поля

        // КД
        private IDbDataAdapter daKD;
        private DataSet dsKD;
        // Организации.Арендаторы
        private IDbDataAdapter daOrgLessees;
        private DataSet dsOrgLessees;
        // Организации.Арендодатели
        private IDbDataAdapter daOrgLessors;
        private DataSet dsOrgLessors;
        // Факты
        private IDbDataAdapter daIncomesLease;
        private DataSet dsIncomesLease;

        private IClassifier clsKD;
        private IClassifier clsOrgLessees;
        private IClassifier clsOrgLessors;
        private IFactTable fctIncomesLease;

        private int nullKD;
        private int nullOrgLesses;
        private int nullOrgLessors;

        private ExcelHelper excelHelper;
        private int totalFiles;
        private int filesCount;

        private Dictionary<string, int> kdList = null;//new Dictionary<string, int>(1000);

        #endregion Поля


        #region Константы

        // Количество записей для занесения в базу
        private const int constMaxQueryRecords = 10000;
        // Название страницы екселя, откуда брать данные
        private const string constExcelSheetName = "арен.имущ.";

        #endregion Константы


        #region Инициализация

        /// <summary>
        /// Конструктор
        /// </summary>
        public LeasePumpModule()
            : base()
        {

        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (excelHelper != null) excelHelper.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion Инициализация


        #region Структуры, перечисления

        /// <summary>
        /// Тип строки отчета
        /// </summary>
        private enum ReportRowKind
        {
            /// <summary>
            /// Строка организации - арендодателя
            /// </summary>
            Lessor,

            /// <summary>
            /// Строка организации - арендодателя, содержащая суммы
            /// </summary>
            LessorWithSums,

            /// <summary>
            /// Строка организации-арендатора
            /// </summary>
            Lessee,

            /// <summary>
            /// Строка "Итого"
            /// </summary>
            Total,

            /// <summary>
            /// Неизвестная организация
            /// </summary>
            Unknown
        }

        #endregion Структуры, перечисления


        #region Закачка данных

        /// <summary>
        /// Возвращает скорректированную дату отчета
        /// </summary>
        /// <param name="sheet">Страница с данными отчета</param>
        /// <returns>Дата</returns>
        private int GetDate(object sheet)
        {
            int result = Convert.ToInt32(
                CommonRoutines.LongDateToNewDate(excelHelper.GetCell(sheet, "A6").Value));
            return (result / 100) * 100;
        }

        /// <summary>
        /// Определяет тип строки отчета
        /// </summary>
        /// <param name="sheet">Страница с данными</param>
        /// <param name="rowIndex">Номер строки</param>
        /// <param name="rightMargin">Правая граница таблицы</param>
        /// <returns>Тип строки</returns>
        private ReportRowKind GetReportRowKind(object sheet, int rowIndex, int rightMargin)
        {
            ExcelCell cell = excelHelper.GetCell(sheet, rowIndex, 2);

            // Если все ячейки сумм пустые, то это строка арендодателя, иначе - строка арендатора
            for (int i = 3; i < rightMargin; i++)
            {
                if (excelHelper.GetCell(sheet, rowIndex, i).Value != string.Empty)
                {
                    if (cell.Value.ToUpper() == "ИТОГО")
                    {
                        return ReportRowKind.Total;
                    }
                    else if (cell.Font.Bold)
                    {
                        return ReportRowKind.LessorWithSums;
                    }
                    else
                    {
                        return ReportRowKind.Lessee;
                    }
                }
            }

            if (!cell.Font.Bold)
            {
                return ReportRowKind.Lessee;
            }

            return ReportRowKind.Lessor;
        }

        /// <summary>
        /// Возвращает нижнюю границу таблицы отчета
        /// </summary>
        /// <param name="sheet">Страница с данными</param>
        /// <returns>Номер строки нижней границы</returns>
        private int GetReportBottomMargin(object sheet)
        {
            int i = 13;
            for (; excelHelper.GetCell(sheet, i, 2).Value.ToUpper() != "ВСЕГО"; i++) { }
            return i;
        }

        /// <summary>
        /// Закачивает файлы
        /// </summary>
        /// <param name="dir">Каталог с файлами</param>
        protected override void ProcessFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            excelHelper.InitCellFont = true;
            object excelObj = excelHelper.OpenExcel(false);

            try
            {
                FileInfo[] files = dir.GetFiles("*.xls", SearchOption.AllDirectories);
                if (files.GetLength(0) == 0)
                {
                    throw new Exception("Отсутствуют данные для закачки.");
                }

                for (int i = 0; i < files.GetLength(0); i++)
                {
                    filesCount++;
                    SetProgress(totalFiles, filesCount,
                        string.Format("Обработка файла {0}...", files[i].FullName),
                        string.Format("Файл {0} из {1}", filesCount, totalFiles));

                    WriteEventIntoDataPumpProtocol(
                        DataPumpEventKind.dpeStartFilePumping, string.Format("Старт закачки файла {0}.", files[i].FullName));

                    // Счетчик строк с данными отчета
                    int rowsCount = 0;
                    // Счетчик закачанных строк
                    int pumpedRowsCount = 0;

                    try
                    {
                        // Получаем нужную страницу отчета
                        object sheet = excelHelper.GetSheet(
                            excelHelper.GetWorkbook(excelObj, files[i].FullName, true), constExcelSheetName);

                        int date = GetDate(sheet);

                        // Бежим по строке заголовка, в которой находятся коды дохода закачиваем их.
                        // Заодно находим и правую границу таблицы отчета
                        int rightMargin = PumpKD(sheet);

                        int parentOrgCode = -1;
                        int childOrgCode = 0;
                        int lessorOrgID = nullOrgLessors;
                        int lesseeOrgID = nullOrgLesses;
                        ReportRowKind rowKind = ReportRowKind.Unknown;
                        int bottomMargin = GetReportBottomMargin(sheet);

                        // Качаем данные, начиная с 13 строки
                        for (int j = 13; j < bottomMargin; j++)
                        {
                            if (rowKind == ReportRowKind.LessorWithSums &&
                                excelHelper.GetCell(sheet, j, 2).Value.ToUpper() == "ИТОГО")
                            {
                                continue;
                            }

                            ExcelCell orgCode = excelHelper.GetCell(sheet, j, 1);
                            ExcelCell orgName = excelHelper.GetCell(sheet, j, 2);
                            rowKind = GetReportRowKind(sheet, j, rightMargin);

                            switch (rowKind)
                            {
                                // Закачиваем организацию-арендодателя
                                case ReportRowKind.Lessor:
                                case ReportRowKind.LessorWithSums:
                                    // Код ХХХ.ХХ формируется по графе А. Для записей, у которых в графе А нет кода, 
                                    // код формируется следующим образом: символы ХХХ первой части маски формируются по 
                                    // коду предыдущей записи, далее - по порядку, в котором встречаются
                                    if (orgCode.Value != string.Empty)
                                    {
                                        parentOrgCode = Convert.ToInt32(orgCode.Value.PadLeft(3, '0').PadRight(5, '0'));
                                        childOrgCode = 0;
                                        lessorOrgID = PumpOriginalRow(dsOrgLessors, clsOrgLessors, 
                                            new object[] { "CODE", parentOrgCode, "NAME", orgName.Value });
                                    }
                                    else
                                    {
                                        childOrgCode++;
                                        lessorOrgID = PumpOriginalRow(dsOrgLessors, clsOrgLessors, 
                                            new object[] { "CODE", parentOrgCode + childOrgCode, "NAME", orgName.Value });
                                    }

                                    // В строке с наименованием арендодателя сумм нет, так что переходим к следующей
                                    if (rowKind == ReportRowKind.Lessor)
                                    {
                                        continue;
                                    }

                                    break;

                                // Закачиваем организацию-арендатора
                                case ReportRowKind.Lessee:
                                    lesseeOrgID = PumpOriginalRow(dsOrgLessees, clsOrgLessees, 
                                        new object[] { /*"CODE", orgCode.Value,*/ "NAME", orgName.Value });

                                    break;
                            }

                            // Закачиваем суммы
                            for (int k = 5; k < rightMargin; k += 2)
                            {
                                string forMonth = excelHelper.GetCell(sheet, j, k).Value;
                                string fromBeginYear = excelHelper.GetCell(sheet, j, k + 1).Value;

                                if (forMonth.Trim('0', ',') == string.Empty && fromBeginYear.Trim('0', ',') == string.Empty) continue;

                                switch (rowKind)
                                {
                                    case ReportRowKind.Lessee:
                                        PumpRow(dsIncomesLease.Tables[0], new object[] {
                                            "SOURCEKEY", j,
                                            "FORMONTH", forMonth.PadLeft(1, '0'),
                                            "FROMBEGINYEAR", fromBeginYear.PadLeft(1, '0'),
                                            "REFKD", FindRowID(dsKD.Tables[0], 
                                                new object[] { "CODESTR", excelHelper.GetCell(sheet, 9, k).Value }, nullKD ),
                                            "REFORGLESSEES", lesseeOrgID,
                                            "REFORGLESSORS", nullOrgLessors,
                                            "RefYearDayUNV", date });
                                        break;

                                    case ReportRowKind.Total:
                                    case ReportRowKind.LessorWithSums:
                                        PumpRow(dsIncomesLease.Tables[0], new object[] {
                                            "SOURCEKEY", j,
                                            "FORMONTH", forMonth.PadLeft(1, '0'),
                                            "FROMBEGINYEAR", fromBeginYear.PadLeft(1, '0'),
                                            "REFKD", FindRowID(dsKD.Tables[0], 
                                                new object[] { "CODESTR", excelHelper.GetCell(sheet, 9, k).Value }, nullKD ),
                                            "REFORGLESSEES", nullOrgLesses,
                                            "REFORGLESSORS", lessorOrgID,
                                            "RefYearDayUNV", date });
                                        break;
                                }

                                pumpedRowsCount++;
                            }

                            rowsCount++;
                        }

                        WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeSuccessfullFinishFilePump, string.Format(
                                "Закачка файла {0} завершена. Обработано строк: {1}, закачано строк: {2}.",
                                files[i].FullName, rowsCount, pumpedRowsCount));
                    }
                    catch (ThreadAbortException)
                    {
                        WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeCriticalError, string.Format(
                                "Закачка файла {0} прервана пользователем. На момент прерывания достигнуты следующие результаты. " +
                                "Обработано строк: {1}, закачано строк: {2}. Данные не сохранены.",
                                files[i].FullName, rowsCount, pumpedRowsCount));
                        throw;
                    }
                    catch (Exception ex)
                    {
                        WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                                "Закачка файла {0} закончена с ошибками. " +
                                "На момент возникновения ошибки достигнуты следующие результаты. " +
                                "Обработано строк: {1}, закачано строк: {2}. Данные не сохранены.",
                                files[i].FullName, rowsCount, pumpedRowsCount), ex);
                        throw;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (excelHelper != null)
                {
                    excelHelper.CloseExcel(ref excelObj);
                    excelHelper.Dispose();
                }
            }
        }

        /// <summary>
        /// Закачивает КД из заголовка таблицы отчета
        /// </summary>
        /// <param name="sheet">Страница с данными</param>
        /// <returns>Крайняя правая граница таблицы</returns>
        private int PumpKD(object sheet)
        {
            // Считываем первую часть наименования КД из заголовка "Всего"
            string mainName = excelHelper.GetCell(sheet, "E8").Value;

            // Теперь считываем из заголовков нижнего уровня коды КД и остатки наименования
            // Начинаем с 5-го столбца
            int i = 5;
            for (; excelHelper.GetCell(sheet, 9, i).Value != string.Empty; i += 2)
            {
                PumpOriginalRow(dsKD, clsKD, new object[] { 
                    "CODESTR", excelHelper.GetCell(sheet, 9, i).Value,
                    "NAME", mainName + " " + excelHelper.GetCell(sheet, 10, i).Value });
            }

            return i;
        }

        /// <summary>
        /// Запрос данных из базы
        /// </summary>
        protected override void QueryData()
        {
            InitClsDataSet(ref daKD, ref dsKD, clsKD, false, string.Empty);
            InitClsDataSet(ref daOrgLessees, ref dsOrgLessees, clsOrgLessees, false, string.Empty);
            InitClsDataSet(ref daOrgLessors, ref dsOrgLessors, clsOrgLessors, false, string.Empty);

            InitFactDataSet(ref daIncomesLease, ref dsIncomesLease, fctIncomesLease);

            FillRowsCache(ref kdList, dsKD.Tables[0], "CODESTR");
            InitNullClsRows();
        }

        /// <summary>
        /// Внести изменения в базу
        /// </summary>
        protected override void UpdateData()
        {
            UpdateDataSet(daKD, dsKD, clsKD);
            UpdateDataSet(daOrgLessees, dsOrgLessees, clsOrgLessees);
            UpdateDataSet(daOrgLessors, dsOrgLessors, clsOrgLessors);
            UpdateDataSet(daIncomesLease, dsIncomesLease, fctIncomesLease);
        }

        /// <summary>
        /// Инициализирует строки классификаторов "Неизвестные данные"
        /// </summary>
        private void InitNullClsRows()
        {
            nullKD = clsKD.UpdateFixedRows(this.DB, this.SourceID);
            nullOrgLesses = clsOrgLessees.UpdateFixedRows(this.DB, this.SourceID);
            nullOrgLessors = clsOrgLessors.UpdateFixedRows(this.DB, this.SourceID);
        }

        /// <summary>
        /// Инициализация объектов БД
        /// </summary>
        private const string D_KD_LEASE_GUID = "5c01b8dd-3086-4568-b925-bfc4e08387f5";
        private const string D_ORGANIZATIONS_LESSEES_GUID = "f8ea4ba0-55ea-4ce0-8dbb-472b1171cb4a";
        private const string D_ORGANIZATIONS_LESSORS_GUID = "035abb61-601d-4a09-a8b6-6e6d3a6099d8";
        private const string F_F_INCOMES_LEASE_GUID = "e4c60fc4-1c11-47f3-a104-e89c015f3be5";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {
                clsKD = this.Scheme.Classifiers[D_KD_LEASE_GUID],
                clsOrgLessees = this.Scheme.Classifiers[D_ORGANIZATIONS_LESSEES_GUID],
                clsOrgLessors = this.Scheme.Classifiers[D_ORGANIZATIONS_LESSORS_GUID] };

            this.UsedFacts = new IFactTable[] {
                fctIncomesLease = this.Scheme.FactTables[F_F_INCOMES_LEASE_GUID] };
        }

        /// <summary>
        /// Функция выполнения завершающих действий этап
        /// </summary>
        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsKD);
            ClearDataSet(ref dsOrgLessees);
            ClearDataSet(ref dsOrgLessors);
            ClearDataSet(ref dsIncomesLease);
        }

        /// <summary>
        /// Закачка данных
        /// </summary>
        protected override void DirectPumpData()
        {
            totalFiles = this.RootDir.GetFiles("*.xls", SearchOption.AllDirectories).GetLength(0);
            filesCount = 0;

            PumpDataYTemplate();
        }

        #endregion Закачка данных
    }
}

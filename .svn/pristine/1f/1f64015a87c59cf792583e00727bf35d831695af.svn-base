using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps.SKIFMonthRepPump
{
    // Модуль закачки ежемесячных отчетов формата DBF

    /// <summary>
    /// ФО_0002_Ежемесячные отчеты.
    /// Закачка данных СКИФ
    /// </summary>
    public partial class SKIFMonthRepPumpModule : SKIFRepPumpModuleBase
    {
        #region Функции закачки шаблона

        /// <summary>
        /// Закачивает классификатор из шаблона формы 50
        /// </summary>
        /// <param name="kl">Поле KL</param>
        /// <param name="kst">Поле KST</param>
        /// <param name="kbk">Поле KBK</param>
        /// <param name="n2">Поле N2</param>
        /// <returns>ИД классификатора</returns>
        private int PumpClsFromPattern50(int kl, int kst, string kbk, string n2)
        {
            int id = -1;
            string restrictedKbk = kbk.TrimStart('0').PadRight(1, '0');
            string kbkEx = GetCacheCode(kbk, kl, kst);
            switch (kl)
            {
                // КД
                case 1:
                    if (!ToPumpBlock(Block.bIncomes))
                        break;
                    id = PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, kbkEx,
                        new object[] { "CODEStr", restrictedKbk, "NAME", n2, "KL", kl, "KST", kst });
                    break;
                // ФКР
                case 2:
                    if (!ToPumpBlock(Block.bOutcomes))
                        break;
                    id = PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, kbkEx,
                        new object[] { "CODE", restrictedKbk, "NAME", n2, "KL", kl, "KST", kst });
                    break;
                // КИВнутрФ
                case 4:
                    if (!ToPumpBlock(Block.bInnerFinSources))
                        break;
                    if (this.DataSource.Year >= 2005)
                        id = PumpCachedRow(srcInFinCache, dsSrcInFin.Tables[0], clsSrcInFin, kbkEx,
                            new object[] { "CODESTR", kbk, "NAME", n2, "KL", kl, "KST", kst });
                    else 
                        id = PumpCachedRow(srcInFinCache, dsSrcInFin.Tables[0], clsSrcInFin, kbkEx,
                            new object[] { "CODE", restrictedKbk, "NAME", n2, "KL", kl, "KST", kst });
                    break;
                // КИВнешФ
                case 5:
                    if (!ToPumpBlock(Block.bOuterFinSources))
                        break;
                    if (this.DataSource.Year >= 2005)
                        id = PumpOriginalRow(dsSrcOutFin, clsSrcOutFin,
                            new object[] { "CODESTR", kbk, "NAME", n2, "KL", kl, "KST", kst });
                    else
                        id = PumpOriginalRow(dsSrcOutFin, clsSrcOutFin,
                            new object[] { "CODEStr", restrictedKbk, "NAME", n2, "KL", kl, "KST", kst });
                    break;
            }
            return id;
        }

        /// <summary>
        /// Закачивает классификатор из шаблона формы 51
        /// </summary>
        /// <param name="kl">Поле KL</param>
        /// <param name="kst">Поле KST</param>
        /// <param name="kbk">Поле KBK</param>
        /// <param name="n2">Поле N2</param>
        /// <returns>ИД классификатора</returns>
        private int PumpClsFromPattern51(int kl, int kst, string kbk, string n2)
        {
            int id = -1;
            string kbkEx = GetCacheCode(kbk, kl, kst);
            string longCode = kbk + kst.ToString();
            string fkr = string.Empty;
            string ekr = string.Empty;

            if ((kl >= 115 && kl <= 137) || (kl >= 5 && kl <= 20) || (kl == 44) || (kl == 51) ||
                (kl >= 21 && kl <= 43) || (kl >= 45 && kl <= 50) || (kl >= 52 && kl <= 84))
            {
                if (kbk.Length >= 4)
                    fkr = kbk.Substring(0, 4);
                else
                    fkr = kbk;
                fkr = fkr.TrimStart('0').PadLeft(1, '0');
                if (kbk.Length >= 6)
                    ekr = kbk.Substring(kbk.Length - 6);
                else
                    ekr = kbk;
                ekr = ekr.TrimStart('0').PadLeft(1, '0');
            }

            // Администратор.МесОтч
            if (kl == 1)
            {
                if (!ToPumpBlock(Block.bIncomesRefs))
                    return id;
                string kvsr = kbk.Substring(kbk.Length - 3).TrimStart('0').PadRight(1, '0');
                id = PumpCachedRow(kvsrCache, dsKVSR.Tables[0], clsKVSR, kbkEx,
                    new object[] { "CODE", kvsr, "NAME", n2, "KL", kl, "KST", kst });
            }
            // Показатели.МесОтч_СпрРасходы
            else if ((kl >= 5 && kl <= 20) || (kl == 44) || (kl == 51))
            {
                if (!ToPumpBlock(Block.bOutcomesRefs))
                    return id;
                if (fkr.Trim('0') != string.Empty)
                    id = PumpCachedRow(fkrBookCache, dsFKRBook.Tables[0], clsFKRBook, fkr, "CODE",
                        new object[] { "NAME", n2, "KL", kl, "KST", kst });
                else
                    id = PumpCachedRow(ekrBookCache, dsEKRBook.Tables[0], clsEKRBook, ekr, "CODE",
                        new object[] { "NAME", n2, "KL", kl, "KST", kst });
            }
            // Показатели.МесОтч_СпрРасходыДоп
            else if ((kl >= 21 && kl <= 43) || (kl >= 45 && kl <= 50) || (kl >= 52 && kl <= 84))
            {
                if (!ToPumpBlock(Block.bOutcomesRefsAdd))
                    return id;
                id = PumpCachedRow(marksOutcomesCache, dsMarksOutcomes.Tables[0], clsMarksOutcomes, kbkEx,
                    new object[] { "LONGCODE", longCode, "FKR", fkr, "EKR", ekr, "NAME", n2, "KL", kl, "KST", kst });
            }
            // Показатели.МесОтч_СпрВнутрДолг
            else if (kl >= 85 && kl <= 103)
            {
                if (!ToPumpBlock(Block.bInnerFinSourcesRefs))
                    return id;
                if (this.DataSource.Year >= 2003)
                    id = PumpOriginalRow(dsMarksInDebt, clsMarksInDebt, new object[] { 
                        "LONGCODE", longCode, "SRCINFIN", kbk.Substring(0, 5),
                        "GVRMINDEBT", kbk.Substring(5), "NAME", n2, "KL", kl, "KST", kst });
                else
                    id = PumpOriginalRow(dsMarksInDebt, clsMarksInDebt, new object[] { 
                        "LONGCODE", longCode, "SRCINFIN", kbk.Substring(0, 4),
                        "GVRMINDEBT", kbk.Substring(4), "NAME", n2, "KL", kl, "KST", kst });
            }
            // Показатели.МесОтч_СпрВнешДолг
            else if (kl >= 104 && kl <= 108)
            {
                if (!ToPumpBlock(Block.bOuterFinSourcesRefs))
                    return id;
                id = PumpOriginalRow(dsMarksOutDebt, clsMarksOutDebt, new object[] { 
                    "LONGCODE", longCode, "SRCOUTFIN", kbk, "GVRMOUTDEBT", 0, "NAME", n2, "KL", kl, "KST", kst });
            }
            // Показатели.МесОтч_СпрЗадолженность
            else if (kl >= 115 && kl <= 137)
            {
                if (!ToPumpBlock(Block.bArrearsRefs))
                    return id;
                id = PumpOriginalRow(dsMarksArrears, clsMarksArrears, new object[] { 
                    "LONGCODE", longCode, "FKR", fkr, "EKR", ekr, "NAME", n2, "KL", kl, "KST", kst });
            }
            return id;
        }

        /// <summary>
        /// Закачивает классификатор из шаблона
        /// </summary>
        /// <param name="fileFormNo">Номер формы из названия файла</param>
        /// <param name="kl">Поле KL</param>
        /// <param name="kst">Поле KST</param>
        /// <param name="kbk">Поле KBK</param>
        /// <param name="n2">Поле N2</param>
        /// <returns>ИД классификатора</returns>
        protected override int PumpClsFromPattern(int fileFormNo, int kl, int kst, string kbk, string n2)
        {
            if (fileFormNo == 50)
            {
                return PumpClsFromPattern50(kl, kst, kbk, n2);
            }
            else if (fileFormNo == 51)
            {
                return PumpClsFromPattern51(kl, kst, kbk, n2);
            }
            return -1;
        }

        #endregion Функции закачки шаблона

        #region Функции общей организации закачки блоков

        /// <summary>
        /// Закачивает блок "Дефицит Профицит"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        /// <param name="file">Файл дбф (для контроля номера формы и т.д.)</param>
        private void PumpMonthRepDefProfDBF(string progressMsg, DataSet ds, FileInfo file)
        {
            if (!CommonRoutines.CheckValueEntry(GetFileFormNo(file), new int[] { 50 })) return;

            WriteToTrace("Старт закачки Блок \"ДефицитПрофицит\".", TraceMessageKind.Information);

            PumpDBFReportData("Блок \"ДефицитПрофицит\"", ds.Tables[0], file, 50, "3",
                daMonthRepDefProf, dsMonthRepDefProf.Tables[0], fctMonthRepDefProf, 
                new DataTable[] { },
                new IClassifier[] { },
                null,
                new string[] { },
                new int[] { },
                progressMsg, null, null, false, BlockProcessModifier.MRStandard, regionCache, nullRegions, null, region4PumpCache);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepDefProf, ref dsMonthRepDefProf);

            WriteToTrace("Закачка Блок \"ДефицитПрофицит\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "Доходы"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        /// <param name="file">Файл дбф (для контроля номера формы и т.д.)</param>
        private void PumpMonthRepIncomesDBF(string progressMsg, DataSet ds, FileInfo file)
        {
            if (!CommonRoutines.CheckValueEntry(GetFileFormNo(file), new int[] { 50 })) return;

            WriteToTrace("Старт закачки Блок \"Доходы\".", TraceMessageKind.Information);

            PumpDBFReportData("Блок \"Доходы\"", ds.Tables[0], file, 50, "1",
                daMonthRepIncomes, dsMonthRepIncomes.Tables[0], fctMonthRepIncomes, 
                new DataTable[] { dsKD.Tables[0], dsKD.Tables[0] },
                new IClassifier[] { clsKD, clsKD },
                new int[] { 2004, 2005 },
                new string[] { "REFKD", "REFKD" },
                new int[] { nullKD, nullKD },
                progressMsg,
                new Dictionary<string, int>[] { kdCache },
                null, false, BlockProcessModifier.MRStandard, regionCache, nullRegions, null, region4PumpCache);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepIncomes, ref dsMonthRepIncomes);

            WriteToTrace("Закачка Блок \"Доходы\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "Источники внешнего финансирования"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        /// <param name="file">Файл дбф (для контроля номера формы и т.д.)</param>
        private void PumpMonthRepOutFinDBF(string progressMsg, DataSet ds, FileInfo file)
        {
            if (!CommonRoutines.CheckValueEntry(GetFileFormNo(file), new int[] { 50 })) return;

            WriteToTrace("Старт закачки Блок \"ИсточникиВнешФин\".", TraceMessageKind.Information);

            PumpDBFReportData("Блок \"ИсточникиВнешФин\"", ds.Tables[0], file, 50, "5",
                daMonthRepOutFin, dsMonthRepOutFin.Tables[0], fctMonthRepOutFin,
                new DataTable[] { dsSrcOutFin.Tables[0], dsSrcOutFin.Tables[0] },
                new IClassifier[] { clsSrcOutFin, clsSrcOutFin },
                new int[] { 2004, 2005 },
                new string[] { "RefSOF", "RefSOF" },
                new int[] { nullSrcOutFin, nullSrcOutFin },
                progressMsg, null, null, false, BlockProcessModifier.MRStandard, regionCache, nullRegions, null, region4PumpCache);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepOutFin, ref dsMonthRepOutFin);

            WriteToTrace("Закачка Блок \"ИсточникиВнешФин\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "Источники внутреннего финансирования"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        /// <param name="file">Файл дбф (для контроля номера формы и т.д.)</param>
        private void PumpMonthRepInFinDBF(string progressMsg, DataSet ds, FileInfo file)
        {
            if (!CommonRoutines.CheckValueEntry(GetFileFormNo(file), new int[] { 50 })) return;

            WriteToTrace("Старт закачки Блок \"ИсточникиВнутрФин\".", TraceMessageKind.Information);

            PumpDBFReportData("Блок \"ИсточникиВнутрФин\"", ds.Tables[0], file, 50, "4",
                daMonthRepInFin, dsMonthRepInFin.Tables[0], fctMonthRepInFin,
                new DataTable[] { dsSrcInFin.Tables[0], dsSrcInFin.Tables[0], dsSrcInFin.Tables[0] },
                new IClassifier[] { clsSrcInFin, clsSrcInFin, clsSrcInFin },
                new int[] { 2002, 2003, 2005 },
                new string[] { "RefSIF", "RefSIF", "RefSIF" },
                new int[] { nullSrcInFin, nullSrcInFin, nullSrcInFin },
                progressMsg,
                new Dictionary<string, int>[] { srcInFinCache },
                null, false, BlockProcessModifier.MRStandard, regionCache, nullRegions, null, region4PumpCache);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepInFin, ref dsMonthRepInFin);

            WriteToTrace("Закачка Блок \"ИсточникиВнутрФин\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "Расходы"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        /// <param name="file">Файл дбф (для контроля номера формы и т.д.)</param>
        private void PumpMonthRepOutcomesDBF(string progressMsg, DataSet ds, FileInfo file)
        {
            if (!CommonRoutines.CheckValueEntry(GetFileFormNo(file), new int[] { 50 })) return;

            WriteToTrace("Старт закачки Блок \"Расходы\".", TraceMessageKind.Information);

            PumpDBFReportData("Блок \"Расходы\"", ds.Tables[0], file, 50, "2",
                daMonthRepOutcomes, dsMonthRepOutcomes.Tables[0], fctMonthRepOutcomes,
                new DataTable[] { dsFKR.Tables[0], dsEKR.Tables[0] },
                new IClassifier[] { clsFKR, clsEKR },
                new int[] { 2005 },
                new string[] { "REFFKR", "REFEKR" },
                new int[] { nullFKR, nullEKR },
                progressMsg,
                new Dictionary<string, int>[] { fkrCache, ekrCache },
                null, false, BlockProcessModifier.MROutcomes, regionCache, nullRegions, null, region4PumpCache);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepOutcomes, ref dsMonthRepOutcomes);

            WriteToTrace("Закачка Блок \"Расходы\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "СправВнешнийДолг"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        /// <param name="file">Файл дбф (для контроля номера формы и т.д.)</param>
        private void PumpMonthRepOutDebtBooksDBF(string progressMsg, DataSet ds, FileInfo file)
        {
            if (!CommonRoutines.CheckValueEntry(GetFileFormNo(file), new int[] { 51 })) return;

            WriteToTrace("Старт закачки Блок \"СправВнешнийДолг\".", TraceMessageKind.Information);

            PumpDBFReportData("Блок \"СправВнешнийДолг\"", ds.Tables[0], file, 51, "104..108",
                daMonthRepOutDebtBooks, dsMonthRepOutDebtBooks.Tables[0], fctMonthRepOutDebtBooks,
                new DataTable[] { dsMarksOutDebt.Tables[0] },
                new IClassifier[] { clsMarksOutDebt },
                new int[] { 2005 },
                new string[] { "REFMARKSOUTDEBT" },
                new int[] { nullMarksOutDebt },
                progressMsg, null, null, true, BlockProcessModifier.MRCommonBooks, regionCache, nullRegions, null, region4PumpCache);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepOutDebtBooks, ref dsMonthRepOutDebtBooks);

            WriteToTrace("Закачка Блок \"СправВнешнийДолг\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "СправВнутреннийДолг"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        /// <param name="file">Файл дбф (для контроля номера формы и т.д.)</param>
        private void PumpMonthRepInDebtBooksDBF(string progressMsg, DataSet ds, FileInfo file)
        {
            if (!CommonRoutines.CheckValueEntry(GetFileFormNo(file), new int[] { 51 })) return;

            WriteToTrace("Старт закачки Блок \"СправВнутреннийДолг\".", TraceMessageKind.Information);

            PumpDBFReportData("Блок \"СправВнутреннийДолг\"", ds.Tables[0], file, 51, "85..103",
                daMonthRepInDebtBooks, dsMonthRepInDebtBooks.Tables[0], fctMonthRepInDebtBooks,
                new DataTable[] { dsMarksInDebt.Tables[0] },
                new IClassifier[] { clsMarksInDebt },
                new int[] { 2005 },
                new string[] { "REFMARKSINDEBT" },
                new int[] { nullMarksInDebt },
                progressMsg, null, null, true, BlockProcessModifier.MRCommonBooks, regionCache, nullRegions, null, region4PumpCache);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepInDebtBooks, ref dsMonthRepInDebtBooks);

            WriteToTrace("Закачка Блок \"СправВнутреннийДолг\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "СправДоходы"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        /// <param name="file">Файл дбф (для контроля номера формы и т.д.)</param>
        private void PumpMonthRepIncomesBooksDBF(string progressMsg, DataSet ds, FileInfo file)
        {
            if (!CommonRoutines.CheckValueEntry(GetFileFormNo(file), new int[] { 51 })) return;

            WriteToTrace("Старт закачки Блок \"СправДоходы\".", TraceMessageKind.Information);

            PumpDBFReportData("Блок \"СправДоходы\"", ds.Tables[0], file, 51, "1",
                daMonthRepIncomesBooks, dsMonthRepIncomesBooks.Tables[0], fctMonthRepIncomesBooks,
                new DataTable[] { dsKVSR.Tables[0] },
                new IClassifier[] { clsKVSR },
                new int[] { 2005 },
                new string[] { "REFKVSR" },
                new int[] { nullKVSR },
                progressMsg,
                new Dictionary<string, int>[] { kvsrCache },
                null, false, BlockProcessModifier.MRCommonBooks, regionCache, nullRegions, null, region4PumpCache);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepIncomesBooks, ref dsMonthRepIncomesBooks);

            WriteToTrace("Закачка Блок \"СправДоходы\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "СправЗадолженность"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        /// <param name="file">Файл дбф (для контроля номера формы и т.д.)</param>
        private void PumpMonthRepArrearsBooksDBF(string progressMsg, DataSet ds, FileInfo file)
        {
            if (!CommonRoutines.CheckValueEntry(GetFileFormNo(file), new int[] { 51 })) return;

            WriteToTrace("Старт закачки Блок \"СправЗадолженность\".", TraceMessageKind.Information);

            PumpDBFReportData("Блок \"СправЗадолженность\"", ds.Tables[0], file, 51, "115..137",
                daMonthRepArrearsBooks, dsMonthRepArrearsBooks.Tables[0], fctMonthRepArrearsBooks,
                new DataTable[] { dsMarksArrears.Tables[0] },
                new IClassifier[] { clsMarksArrears },
                new int[] { 2005 },
                new string[] { "REFMARKSARREARS" },
                new int[] { nullMarksArrears },
                progressMsg, null, null, true, BlockProcessModifier.MRCommonBooks, regionCache, nullRegions, null, region4PumpCache);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepArrearsBooks, ref dsMonthRepArrearsBooks);

            WriteToTrace("Закачка Блок \"СправЗадолженность\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "СправРасходы"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        /// <param name="file">Файл дбф (для контроля номера формы и т.д.)</param>
        private void PumpMonthRepOutcomesBooksDBF(string progressMsg, DataSet ds, FileInfo file)
        {
            if (!CommonRoutines.CheckValueEntry(GetFileFormNo(file), new int[] { 51 })) return;

            WriteToTrace("Старт закачки Блок \"СправРасходы\".", TraceMessageKind.Information);

            PumpDBFReportData("Блок \"СправРасходы\"", ds.Tables[0], file, 51, "5..20;44;51",
                daMonthRepOutcomesBooks, dsMonthRepOutcomesBooks.Tables[0], fctMonthRepOutcomesBooks,
                new DataTable[] { dsFKRBook.Tables[0], dsEKRBook.Tables[0] },
                new IClassifier[] { clsFKRBook, clsEKRBook },
                null,
                new string[] { "REFFKR", "REFEKR" },
                new int[] { nullFKRBook, nullEKRBook },
                progressMsg,
                new Dictionary<string, int>[] { fkrBookCache, ekrBookCache },
                null, false, BlockProcessModifier.MROutcomesBooks, regionCache, nullRegions,
                new string[] { "0..4", "-1..6" }, region4PumpCache);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepOutcomesBooks, ref dsMonthRepOutcomesBooks);

            WriteToTrace("Закачка Блок \"СправРасходы\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "СправРасходыДоп"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        /// <param name="file">Файл дбф (для контроля номера формы и т.д.)</param>
        private void PumpMonthRepOutcomesBooksExDBF(string progressMsg, DataSet ds, FileInfo file)
        {
            if (!CommonRoutines.CheckValueEntry(GetFileFormNo(file), new int[] { 51 })) return;

            WriteToTrace("Старт закачки Блок \"СправРасходыДоп\".", TraceMessageKind.Information);

            PumpDBFReportData("Блок \"СправРасходыДоп\"", ds.Tables[0], file, 51, "21..43;45..50;52..84",
                daMonthRepOutcomesBooksEx, dsMonthRepOutcomesBooksEx.Tables[0], fctMonthRepOutcomesBooksEx,
                new DataTable[] { dsMarksOutcomes.Tables[0] },
                new IClassifier[] { clsMarksOutcomes },
                new int[] { 2005 },
                new string[] { "REFMARKSOUTCOMES" },
                new int[] { nullMarksOutcomes },
                progressMsg,
                new Dictionary<string, int>[] { marksOutcomesCache },
                null, true, BlockProcessModifier.MROutcomesBooksEx, regionCache, nullRegions, null, region4PumpCache);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daMonthRepOutcomesBooksEx, ref dsMonthRepOutcomesBooksEx);

            WriteToTrace("Закачка Блок \"СправРасходыДоп\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Возвращает все формы файлов, разрешенные к закачке
        /// </summary>
        /// <returns>Массив номеров форм</returns>
        protected override int[] GetAllFormNo()
        {
            return new int[] { 50, 51 };
        }

        /// <summary>
        /// Закачивает отчет формата DBF
        /// </summary>
        /// <param name="file">Файл отчета</param>
        protected override void PumpDBFReport(FileInfo file, string progressMsg)
        {
            IDbDataAdapter da = null;
            DataSet ds = new DataSet();
            InitDataSet(this.DbfDB, ref da, ref ds, string.Format(
                "select * from {0} where not " +
                "(P1 = 0 and P2 = 0 and P3 = 0 and P4 = 0 and P5 = 0 and P6 = 0 and P7 = 0 and P8 = 0 and P9 = 0)",
                file.Name));

            // Закачка районов
            PumpRegionsDBF(dsRegions.Tables[0], clsRegions, regionCache, nullRegions, 
                dsRegions4Pump.Tables[0], clsRegions4Pump, region4PumpCache);

            if (ToPumpBlock(Block.bIncomes))
                PumpMonthRepIncomesDBF(progressMsg, ds, file);
            if (ToPumpBlock(Block.bOutcomes))
                PumpMonthRepOutcomesDBF(progressMsg, ds, file);
            if (ToPumpBlock(Block.bDefProf))
                PumpMonthRepDefProfDBF(progressMsg, ds, file);
            if (ToPumpBlock(Block.bInnerFinSources))
                PumpMonthRepInFinDBF(progressMsg, ds, file);
            if (ToPumpBlock(Block.bOuterFinSources))
                PumpMonthRepOutFinDBF(progressMsg, ds, file);
            if (ToPumpBlock(Block.bIncomesRefs))
                PumpMonthRepIncomesBooksDBF(progressMsg, ds, file);
            if (ToPumpBlock(Block.bOutcomesRefs))
                PumpMonthRepOutcomesBooksDBF(progressMsg, ds, file);
            if (ToPumpBlock(Block.bOutcomesRefsAdd))
                PumpMonthRepOutcomesBooksExDBF(progressMsg, ds, file);
            if (ToPumpBlock(Block.bInnerFinSourcesRefs))
                PumpMonthRepInDebtBooksDBF(progressMsg, ds, file);
            if (ToPumpBlock(Block.bOuterFinSourcesRefs))
                PumpMonthRepOutDebtBooksDBF(progressMsg, ds, file);
            if (ToPumpBlock(Block.bArrearsRefs))
                PumpMonthRepArrearsBooksDBF(progressMsg, ds, file);
        }

        #endregion Функции общей организации закачки блоков
    }
}
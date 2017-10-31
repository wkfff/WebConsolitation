using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps.SKIFYearRepPump
{
    // Модуль функций закачки отчетов формата dbf

    /// <summary>
    /// ФО_0005_Ежегодные отчеты.
    /// Закачка данных СКИФ
    /// </summary>
    public partial class SKIFYearRepPumpModule : SKIFRepPumpModuleBase
    {
        #region Функции закачки шаблона

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
            int id = -1;
            string restrictedKbk = kbk.TrimStart('0').PadRight(1, '0');
            string kbkEx = GetCacheCode(kbk, kl, kst);

            switch (fileFormNo)
            {
                case 2:
                    // Доходы
                    if ((this.DataSource.Year == 2000 && kl >= 8 && kl <= 13) ||
                        ((this.DataSource.Year == 2001 || this.DataSource.Year == 2002) && kl >= 8 && kl <= 14) ||
                        (this.DataSource.Year == 2003 && kl >= 8 && kl <= 20) ||
                        (this.DataSource.Year == 2004 && kl >= 8 && kl <= 21))
                    {
                        if (ToPumpBlock(Block.bIncomes))
                            id = PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, restrictedKbk,
                                new object[] { "CODESTR", restrictedKbk, "NAME", n2, "KL", kl, "KST", kst });
                    }
                    // Источники финансирования
                    else if ((this.DataSource.Year == 2000 && kl >= 22 && kl <= 24) ||
                        ((this.DataSource.Year == 2001 || this.DataSource.Year == 2002) && kl >= 20 && kl <= 21) ||
                        (this.DataSource.Year == 2003 && kl >= 27 && kl <= 30) ||
                        (this.DataSource.Year == 2004 && kl >= 28 && kl <= 31))
                    {
                        if (ToPumpBlock(Block.bFinSources))
                            id = PumpCachedRow(kifCache, dsKIF2004.Tables[0], clsKIF2004, restrictedKbk,
                                new object[] { "CODESTR", restrictedKbk, "NAME", n2, "KL", kl, "KST", kst });
                    }
                    // Расходы
                    else if ((this.DataSource.Year == 2000 && kl >= 25 && kl <= 254) ||
                       (this.DataSource.Year == 2001 && kl >= 22 && kl <= 186) ||
                       (this.DataSource.Year == 2002 && kl >= 22 && kl <= 188) ||
                       (this.DataSource.Year == 2003 && kl >= 31 && kl <= 214) ||
                       (this.DataSource.Year == 2004 && kl >= 32 && kl <= 250))
                    {
                        if (ToPumpBlock(Block.bOutcomes))
                        {
                            // ФКР
                            if (kbk.EndsWith("000000000000000") && kbk != "7980000000000000000")
                            {
                                string code = kbk.Substring(0, 4);
                                if (code != "0000")
                                {
                                    id = PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, GetCacheCode(code, kl, kst),
                                        new object[] { "CODE", code, "NAME", n2, "KL", kl, "KST", kst });
                                }
                            }
                            // КЦСР
                            else if (kbk.EndsWith("000000000"))
                            {
                                string code = kbk.Substring(7, 3).PadRight(7, '0');
                                id = PumpCachedRow(kcsrCache, dsKCSR.Tables[0], clsKCSR, GetCacheCode(code, kl, kst),
                                    new object[] { "CODE", code, "NAME", n2, "KL", kl, "KST", kst });
                            }
                            // КВР
                            else if (kbk.EndsWith("000000"))
                            {
                                string code = kbk.Substring(10, 3);
                                id = PumpCachedRow(kvrCache, dsKVR.Tables[0], clsKVR, GetCacheCode(code, kl, kst),
                                    new object[] { "CODE", code, "NAME", n2, "KL", kl, "KST", kst });
                            }
                            // ЭКР
                            else if (kbk.Substring(13, 6) != "000000")
                            {
                                string code = kbk.Substring(13, 6);
                                id = PumpCachedRow(ekrCache, dsEKR.Tables[0], clsEKR, GetCacheCode(code, kl, kst),
                                    new object[] { "CODE", code, "NAME", n2, "KL", kl, "KST", kst });
                            }
                        }
                    }
                    break;

                case 3:
                    if (ToPumpBlock(Block.bNet))
                        id = PumpCachedRow(marksNetCache, dsMarksNet.Tables[0], clsMarksNet, kbkEx, new object[] { 
                            "FKR", kbk.Substring(0, 4), "KCSR", kbk.Substring(7, 3), "SUBKCSR", kbk.Substring(10, 1), 
                            "KVR", kbk.Substring(11, 3), "SUBKVR", kbk.Substring(14, 2), "KNEC", kbk.Substring(16, 3), 
                            "CODESTR", kbk, "NAME", n2, "KL", kl, "KST", kst });
                    break;
            }

            return id;
        }

        #endregion Функции закачки шаблона

        #region Функции общей организации закачки блоков

        /// <summary>
        /// Закачивает блок "Дефицит Профицит"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        /// <param name="file">Файл дбф (для контроля номера формы и т.д.)</param>
        private void PumpYearRepDefProfDBF(string progressMsg, DataSet ds, FileInfo file)
        {
            if (!CommonRoutines.CheckValueEntry(GetFileFormNo(file), new int[] { 2 })) return;

            WriteToTrace("Старт закачки Блок \"ДефицитПрофицит\".", TraceMessageKind.Information);

            string kl = string.Empty;
            switch (this.DataSource.Year)
            {
                case 2000: kl = "254";
                    break;

                case 2001: kl = "186";
                    break;

                case 2002: kl = "188";
                    break;

                case 2003: kl = "214";
                    break;

                default: kl = "250";
                    break;
            }

            PumpDBFReportData("Блок \"ДефицитПрофицит\"", ds.Tables[0], file, 2, kl,
                daFOYRDefProf, dsFOYRDefProf.Tables[0], fctFOYRDefProf, 
                new DataTable[] { },
                new IClassifier[] { },
                null,
                new string[] { },
                new int[] { },
                progressMsg, null,
                new string[] { "!7980000000000000000", "-1" },
                false, BlockProcessModifier.YRDefProf, regionCache, nullRegions, null, null);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daFOYRDefProf, ref dsFOYRDefProf);

            WriteToTrace("Закачка Блок \"ДефицитПрофицит\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "Доходы"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        /// <param name="file">Файл дбф (для контроля номера формы и т.д.)</param>
        private void PumpYearRepIncomesDBF(string progressMsg, DataSet ds, FileInfo file)
        {
            if (!CommonRoutines.CheckValueEntry(GetFileFormNo(file), new int[] { 2 })) return;

            WriteToTrace("Старт закачки Блок \"Доходы\".", TraceMessageKind.Information);

            string kl = string.Empty;
            switch (this.DataSource.Year)
            {
                case 2000: kl = "8..13";
                    break;

                case 2001:
                case 2002: kl = "8..14";
                    break;

                case 2003: kl = "8..20";
                    break;

                default: kl = "8..21";
                    break;
            }

            PumpDBFReportData("Блок \"Доходы\"", ds.Tables[0], file, 2, kl,
                daFOYRIncomes, dsFOYRIncomes.Tables[0], fctFOYRIncomes,
                new DataTable[] { dsKD.Tables[0], dsKD.Tables[0] },
                new IClassifier[] { clsKD, clsKD },
                new int[] { 2004, 2005 },
                new string[] { "REFKD", "REFKD" },
                new int[] { nullKD, nullKD },
                progressMsg,
                new Dictionary<string, int>[] { kdCache },
                null, false, BlockProcessModifier.YRIncomes, regionCache, nullRegions, null, null);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daFOYRIncomes, ref dsFOYRIncomes);

            WriteToTrace("Закачка Блок \"Доходы\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "Источники финансирования"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        /// <param name="file">Файл дбф (для контроля номера формы и т.д.)</param>
        private void PumpYearRepSrcFinDBF(string progressMsg, DataSet ds, FileInfo file)
        {
            if (!CommonRoutines.CheckValueEntry(GetFileFormNo(file), new int[] { 2 })) return;

            WriteToTrace("Старт закачки Блок \"Источники финансирования\".", TraceMessageKind.Information);

            string kl = string.Empty;
            switch (this.DataSource.Year)
            {
                case 2000: kl = "22..24";
                    break;

                case 2001:
                case 2002: kl = "20..21";
                    break;

                case 2003: kl = "27..30";
                    break;

                default: kl = "28..31";
                    break;
            }

            PumpDBFReportData("Блок \"Источники финансирования\"", ds.Tables[0], file, 2, kl,
                daFOYRSrcFin, dsFOYRSrcFin.Tables[0], fctFOYRSrcFin,
                new DataTable[] { dsKIF2004.Tables[0], dsKIF2005.Tables[0] },
                new IClassifier[] { clsKIF2004, clsKIF2005 },
                new int[] { 2004, 2005 },
                new string[] { "REFKIF2004", "REFKIF2005" },
                new int[] { nullKIF2004, nullKIF2005 },
                progressMsg,
                new Dictionary<string, int>[] { kifCache },
                null, false, BlockProcessModifier.YRSrcFin, regionCache, nullRegions, null, null);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daFOYRSrcFin, ref dsFOYRSrcFin);

            WriteToTrace("Закачка Блок \"Источники финансирования\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "Недостачи Хищения"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        /// <param name="file">Файл дбф (для контроля номера формы и т.д.)</param>
        private void PumpYearRepEmbezzlesDBF(string progressMsg, DataSet ds, FileInfo file)
        {
            if (!CommonRoutines.CheckValueEntry(GetFileFormNo(file), new int[] { 8 })) return;

            WriteToTrace("Старт закачки Блок \"Недостачи Хищения\".", TraceMessageKind.Information);

            PumpDBFReportData("Блок \"Источники финансирования\"", ds.Tables[0], file, 8, string.Empty,
                daFOYREmbezzles, dsFOYREmbezzles.Tables[0], fctFOYREmbezzles,
                new DataTable[] { dsMarksEmbezzles.Tables[0], dsMeansType.Tables[0] },
                new IClassifier[] { fxcMarksEmbezzles, fxcMeansType },
                new int[] { 2004 },
                new string[] { "REFMARKS", "REFMEANSTYPE" },
                new int[] { 0, 0 },
                progressMsg,
                new Dictionary<string, int>[] { marksEmbezzlesCache, meansTypeCache },
                null, false, BlockProcessModifier.YREmbezzles, regionCache, nullRegions, null, null);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daFOYREmbezzles, ref dsFOYREmbezzles);

            WriteToTrace("Закачка Блок \"Недостачи Хищения\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "Расходы"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        /// <param name="file">Файл дбф (для контроля номера формы и т.д.)</param>
        private void PumpYearRepOutcomesDBF(string progressMsg, DataSet ds, FileInfo file)
        {
            if (!CommonRoutines.CheckValueEntry(GetFileFormNo(file), new int[] { 2 })) return;

            WriteToTrace("Старт закачки Блок \"Расходы\".", TraceMessageKind.Information);

            string kl = string.Empty;
            switch (this.DataSource.Year)
            {
                case 2000: kl = "25..254";
                    break;

                case 2001: kl = "22..186";
                    break;

                case 2002: kl = "22..188";
                    break;

                case 2003: kl = "31..214";
                    break;

                default: kl = "32..250";
                    break;
            }

            PumpDBFReportData("Блок \"Расходы\"", ds.Tables[0], file, 2, kl,
                daFOYROutcomes, dsFOYROutcomes.Tables[0], fctFOYROutcomes,
                new DataTable[] { dsKVR.Tables[0], dsKCSR.Tables[0], dsFKR.Tables[0], dsEKR.Tables[0], null },
                new IClassifier[] { clsKVR, clsKCSR, clsFKR, clsEKR, null },
                new int[] { 2004 },
                new string[] { "REFKVR", "REFKCSR", "REFFKR", "RefEKRFOYR", "RefEKRFOYR" },
                new int[] { nullKVR, nullKCSR, nullFKR, nullEKR, nullEKR },
                progressMsg,
                new Dictionary<string, int>[] { kvrCache, kcsrCache, fkrCache, ekrCache, null },
                new string[] { "#!9600;#!9800", "0..4", "00", "2..3", "000000", "13..18", "000", "7..9", 
                    "000", "10..12", "7980000000000000000", "-1" },
                false, BlockProcessModifier.YROutcomes, regionCache, nullRegions,
                new string[] { "10..12", "7..9:0000000", "0..4", "13..18", "0..0" }, null);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daFOYROutcomes, ref dsFOYROutcomes);

            WriteToTrace("Закачка Блок \"Расходы\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Закачивает блок "Сеть Штаты Контингент"
        /// </summary>
        /// <param name="progressMsg">Строка прогресса</param>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        /// <param name="file">Файл дбф (для контроля номера формы и т.д.)</param>
        private void PumpYearRepNetDBF(string progressMsg, DataSet ds, FileInfo file)
        {
            if (!CommonRoutines.CheckValueEntry(GetFileFormNo(file), new int[] { 3 })) return;

            WriteToTrace("Старт закачки Блок \"Сеть Штаты Контингент\".", TraceMessageKind.Information);

            PumpDBFReportData("Блок \"Сеть Штаты Контингент\"", ds.Tables[0], file, 3, string.Empty,
                daFOYRNet, dsFOYRNet.Tables[0], fctFOYRNet, 
                new DataTable[] { dsMarksNet.Tables[0] },
                new IClassifier[] { clsMarksNet },
                new int[] { 2004 },
                new string[] { "REFMARKS" },
                new int[] { nullMarksNet },
                progressMsg,
                new Dictionary<string, int>[] { marksNetCache },
                new string[] { "000", "7..9", "000", "11..13" },
                false, BlockProcessModifier.YRNet, regionCache, nullRegions, null, null);

            // Сохранение данных
            UpdateData();
            ClearDataSet(daFOYRNet, ref dsFOYRNet);

            WriteToTrace("Закачка Блок \"Сеть Штаты Контингент\" закончена.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Возвращает все формы файлов, разрешенные к закачке
        /// </summary>
        /// <returns>Массив номеров форм</returns>
        protected override int[] GetAllFormNo()
        {
            return new int[] { 2, 3, 8 };
        }

        /// <summary>
        /// Закачивает отчет формата DBF
        /// </summary>
        /// <param name="file">Файл отчета</param>
        protected override void PumpDBFReport(FileInfo file, string progressMsg)
        {
            // Закачка районов
            PumpRegionsDBF(dsRegions.Tables[0], clsRegions, regionCache, nullRegions, null, null, null);

            IDbDataAdapter da = null;
            DataSet ds = new DataSet();

            if (!file.Name.EndsWith("8"))
            {
                InitDataSet(this.DbfDB, ref da, ref ds, string.Format(
                    "select * from {0} where not (P1 = 0 and P2 = 0 and P3 = 0 and P4 = 0)", file.Name));
            }
            else
            {
                InitDataSet(this.DbfDB, ref da, ref ds, string.Format(
                    "select * from {0} where not (P1 = 0 and P2 = 0 and P3 = 0 and P4 = 0 and P5 = 0 and P6 = 0 and P7 = 0 and P8 = 0 and P9 = 0)", 
                    file.Name));
            }

            // Блок "Дефицит Профицит"
            if (ToPumpBlock(Block.bDefProf))
                PumpYearRepDefProfDBF(progressMsg, ds, file);

            // Блок "Доходы"
            if (ToPumpBlock(Block.bIncomes))
                PumpYearRepIncomesDBF(progressMsg, ds, file);

            // Блок "Источники финансирования"
            if (ToPumpBlock(Block.bFinSources))
                PumpYearRepSrcFinDBF(progressMsg, ds, file);

            // Блок "Недостачи Хищения"
            if (ToPumpBlock(Block.bNet))
                PumpYearRepEmbezzlesDBF(progressMsg, ds, file);

            // Блок "Расходы"
            if (ToPumpBlock(Block.bOutcomes))
                PumpYearRepOutcomesDBF(progressMsg, ds, file);

            // Блок "Сеть Штаты Контингент"
            if (ToPumpBlock(Block.bNet))
                PumpYearRepNetDBF(progressMsg, ds, file);
        }

        #endregion Функции общей организации закачки блоков
    }
}
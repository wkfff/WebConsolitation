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
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.SKIFMonthRepPump
{
    // формат ФК - самого ФК
    public partial class SKIFMonthRepPumpModule : SKIFRepPumpModuleBase
    {

        #region работа с классификаторами

        #region 428

        private delegate void ProcessFK2TxtRowDelegate(string dataRow);

        private void ProcessFK2TxtClsFiles428(DirectoryInfo dir, string fileMask, string startMark, string endMark,
                ProcessFK2TxtRowDelegate fkTxtRowDg)
        {
            FileInfo[] files = dir.GetFiles(fileMask, SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                WriteToTrace(string.Format("начало закачки файла {0}", file.Name), TraceMessageKind.Information);
                string[] reportData = CommonRoutines.GetTxtReportData(file, CommonRoutines.GetTxtWinCodePage());
                bool toPumpRow = false;
                int rowIndex = 0;
                foreach (string row in reportData)
                {
                    try
                    {
                        rowIndex++;
                        string auxRow = row.Replace("\n", string.Empty).Trim();
                        if (auxRow.StartsWith(endMark))
                            toPumpRow = false;
                        if (toPumpRow)
                            fkTxtRowDg(auxRow);
                        if (auxRow.StartsWith(startMark))
                            toPumpRow = true;
                    }
                    catch (Exception exp)
                    {
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                            string.Format("Ошибка при обработке строки {0} отчета {1}: {2}", rowIndex, file.Name, exp.Message));
                    }
                }
                UpdateData();
                WriteToTrace(string.Format("завершение закачки файла {0}", file.Name), TraceMessageKind.Information);
            }
        }

        private void PumpFk2TxtKD(string clsRow)
        {
            string code = clsRow.Split('|')[0];
            code = code.PadLeft(20, '0');
            PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, code,
                new object[] { "CodeStr", code, "Name", clsRow.Split('|')[1], "Kl", "0", "Kst", "10" });
        }

        private void PumpFk2TxtR(string clsRow)
        {
            string code = clsRow.Split('|')[0].PadRight(14, '0');
            if (clsRow.Split('|')[0] == "7900")
                return;
            PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, code,
                new object[] { "Code", code, "Name", clsRow.Split('|')[1] });
        }

        private void PumpFk2TxtEkr(string clsRow)
        {
            string code = clsRow.Split('|')[0];
            PumpCachedRow(ekrCache, dsEKR.Tables[0], clsEKR, code,
                new object[] { "Code", code, "Name", clsRow.Split('|')[1] });
        }

        private void PumpFk2TxtSif(string clsRow)
        {
            string code = clsRow.Split('|')[0];
            if (code.StartsWith("02"))
                return;
            code = code.PadLeft(20, '0');
            PumpCachedRow(srcInFinCache, dsSrcInFin.Tables[0], clsSrcInFin, code,
                new object[] { "CodeStr", code, "Name", clsRow.Split('|')[1], "Kl", "0", "Kst", "10" });
        }

        private void PumpFk2TxtSof(string clsRow)
        {
            string code = clsRow.Split('|')[0];
            if (!code.StartsWith("02"))
                return;
            code = code.PadLeft(20, '0');
            PumpCachedRow(srcOutFinCache, dsSrcOutFin.Tables[0], clsSrcOutFin, code,
                new object[] { "CodeStr", code, "Name", clsRow.Split('|')[1], "Kl", "0", "Kst", "10" });
        }

        private void PumpFk2TxtAccount(string clsRow)
        {
            string code = clsRow.Split('|')[0];
            PumpCachedRow(marksAccountCache, dsMarksAccount.Tables[0], clsMarksAccount, code,
                new object[] { "Code", code, "Name", clsRow.Split('|')[1] });
        }

        private void PumpFK2TxtCls428(DirectoryInfo dir)
        {
            if (toPumpIncomes)
                ProcessFK2TxtClsFiles428(dir, "ST01*01.txt", "ГР=03", "#", new ProcessFK2TxtRowDelegate(PumpFk2TxtKD));
            if (toPumpOutcomes)
            {
                ProcessFK2TxtClsFiles428(dir, "ST01*02.txt", "ГР=03", "ГР=04", new ProcessFK2TxtRowDelegate(PumpFk2TxtR));
                ProcessFK2TxtClsFiles428(dir, "ST01*02.txt", "ГР=06", "#", new ProcessFK2TxtRowDelegate(PumpFk2TxtEkr));
            }
            if (toPumpInnerFinSources)
                ProcessFK2TxtClsFiles428(dir, "ST01*03.txt", "ГР=03", "#", new ProcessFK2TxtRowDelegate(PumpFk2TxtSif));
            if (toPumpOuterFinSources)
                ProcessFK2TxtClsFiles428(dir, "ST01*03.txt", "ГР=03", "#", new ProcessFK2TxtRowDelegate(PumpFk2TxtSof));
            if (toPumpAccount)
                ProcessFK2TxtClsFiles428(dir, "ST01*04.txt", "ГР=01", "#", new ProcessFK2TxtRowDelegate(PumpFk2TxtAccount));
        }

        #endregion 428

        #region 487

        private void PumpFk2TxtMarksOutcomes(string clsRow)
        {
            string code = clsRow.Split('|')[0];
            int codeInt = Convert.ToInt32(code);
            if ((codeInt >= 100 && codeInt < 10500) || (codeInt >= 12100 && codeInt < 12400) || (codeInt >= 13000 && codeInt <= 13602) ||
                (codeInt >= 13000 && codeInt <= 14999 && this.DataSource.Year >= 2012))
            {
                string longCode = code.PadRight(22, '0');
                PumpCachedRow(marksOutcomesCache, dsMarksOutcomes.Tables[0], clsMarksOutcomes, longCode,
                    new object[] { "LongCode", longCode, "Name", clsRow.Split('|')[1], "FKR", "0", "KCSR", "0", 
                                   "KVR", "0", "EKR", "0", "Kl", "1", "Kst", code });
            }
        }

        private void PumpFk2TxtSifRefs(string clsRow)
        {
            string code = clsRow.Split('|')[0];
            int codeInt = Convert.ToInt32(code);
            if ((codeInt >= 10500 && codeInt < 10600) || (codeInt >= 10700 && codeInt < 10800) || (codeInt == 12400))
            {
                string longCode = code.PadRight(22, '0');
                PumpCachedRow(scrInFinSourcesRefCache, dsMarksInDebt.Tables[0], clsMarksInDebt, longCode,
                    new object[] { "LongCode", longCode, "Name", clsRow.Split('|')[1], "SrcInFin", "0", "GvrmInDebt", "0", 
                                   "Kl", "1", "Kst", code });
            }
        }

        private void PumpFk2TxtSofRefs(string clsRow)
        {
            string code = clsRow.Split('|')[0];
            int codeInt = Convert.ToInt32(code);
            if (codeInt >= 10600 && codeInt < 10700)
            {
                string longCode = code.PadRight(22, '0');
                PumpCachedRow(scrOutFinSourcesRefCache, dsMarksOutDebt.Tables[0], clsMarksOutDebt, longCode,
                    new object[] { "LongCode", longCode, "Name", clsRow.Split('|')[1], "SrcInFin", "0", "GvrmInDebt", "0", 
                                   "Kl", "1", "Kst", code });
            }
        }

        private void PumpFk2TxtArrearsRefs(string clsRow)
        {
            string code = clsRow.Split('|')[0];
            int codeInt = Convert.ToInt32(code);
            if (codeInt >= 10900 && codeInt < 12100)
            {
                string longCode = code.PadRight(22, '0');
                PumpCachedRow(arrearsCache, dsMarksArrears.Tables[0], clsMarksArrears, longCode,
                    new object[] { "LongCode", longCode, "Name", clsRow.Split('|')[1], "FKR", "0", "KCSR", "0", 
                                   "KVR", "0", "EKR", "0", "Kl", "1", "Kst", code });
            }
        }

        private void PumpFk2TxtExcessRefs(string clsRow)
        {
            string code = clsRow.Split('|')[0];
            int codeInt = Convert.ToInt32(code);
            if (codeInt >= 10800 && codeInt < 10900)
            {
                string longCode = code.PadRight(22, '0');
                PumpCachedRow(excessCache, dsMarksExcess.Tables[0], clsMarksExcess, longCode,
                    new object[] { "LongCode", longCode, "Name", clsRow.Split('|')[1], "FKR", "0", "KCSR", "0", 
                                   "KVR", "0", "EKR", "0", "Kl", "1", "Kst", code });
            }
        }

        private void PumpFK2TxtCls487(DirectoryInfo dir)
        {
            if (toPumpOutcomesRefsAdd)
                ProcessFK2TxtClsFiles428(dir, "ST01*01.txt", "ГР=01", "ГР=02", new ProcessFK2TxtRowDelegate(PumpFk2TxtMarksOutcomes));
            if (toPumpInnerFinSourcesRefs)
                ProcessFK2TxtClsFiles428(dir, "ST01*01.txt", "ГР=01", "ГР=02", new ProcessFK2TxtRowDelegate(PumpFk2TxtSifRefs));
            if (toPumpOuterFinSourcesRefs)
                ProcessFK2TxtClsFiles428(dir, "ST01*01.txt", "ГР=01", "ГР=02", new ProcessFK2TxtRowDelegate(PumpFk2TxtSofRefs));
            if (toPumpArrearsRefs)
                ProcessFK2TxtClsFiles428(dir, "ST01*01.txt", "ГР=01", "ГР=02", new ProcessFK2TxtRowDelegate(PumpFk2TxtArrearsRefs));
            if (toPumpExcessRefs)
                ProcessFK2TxtClsFiles428(dir, "ST01*01.txt", "ГР=01", "ГР=02", new ProcessFK2TxtRowDelegate(PumpFk2TxtExcessRefs));
        }

        #endregion 487

        #endregion работа с классификаторами

        #region работа с фактами

        private string GetFormFK2Txt(string fileName)
        {
            if (fileName.ToUpper().Contains("428"))
                return "428";
            else if (fileName.ToUpper().Contains("487"))
                return "487";
            else
                return string.Empty;
        }

        #region 428

        private void PumpRegionFK2Txt(string row)
        {
            string regionCode = row.Split('=')[1].Split('-')[0].Trim().PadLeft(10, '0');
            string regionName = row.Split('=')[1].Split('-')[1].Trim();
            string key = string.Format("{0}|{1}", regionCode, regionName);
            PumpCachedRow(region4PumpCache, dsRegions4Pump.Tables[0], clsRegions4Pump, key,
                new object[] { "CodeStr", regionCode, "Name", regionName, "RefDocType", "3", "SOURCEID", regForPumpSourceID });
            refRegionTxt = PumpCachedRow(regionCache, dsRegions.Tables[0], clsRegions, key,
                new object[] { "CodeStr", regionCode, "Name", regionName, "BudgetKind", "КБС", "BudgetName", "Консолидированный бюджет субъекта" });
        }

        private void ProcessFK2TxtFactFiles428(DirectoryInfo dir, string fileMask, string startMark, string endMark,
                ProcessFK2TxtRowDelegate fkTxtRowDg)
        {
            FileInfo[] files = dir.GetFiles(fileMask, SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                if (CommonRoutines.TrimNumbers(file.Name.Split('.')[0]) != string.Empty)
                    return;
                WriteToTrace(string.Format("начало закачки файла {0}", file.Name), TraceMessageKind.Information);
                string[] reportData = CommonRoutines.GetTxtReportData(file, CommonRoutines.GetTxtWinCodePage());
                bool toPumpRow = false;
                int rowIndex = 0;
                foreach (string row in reportData)
                {
                    try
                    {
                        rowIndex++;
                        string auxRow = row.Replace("\n", string.Empty).Trim();
                        if (auxRow.ToUpper().StartsWith("ИСТ"))
                            PumpRegionFK2Txt(auxRow);
                        if (auxRow.StartsWith(endMark))
                            toPumpRow = false;
                        if (toPumpRow)
                            fkTxtRowDg(auxRow);
                        if (auxRow.StartsWith(startMark))
                            toPumpRow = true;
                    }
                    catch (Exception exp)
                    {
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                            string.Format("Ошибка при обработке строки {0} отчета {1}: {2}", rowIndex, file.Name, exp.Message));
                    }
                }
                UpdateData();
                WriteToTrace(string.Format("завершение закачки файла {0}", file.Name), TraceMessageKind.Information);
            }
        }

        private void PumpFk2TxtIncomes(string dataRow)
        {
            string[] rowValues = dataRow.Split('|');
            string code = rowValues[2];
            code = code.PadLeft(20, '0');
            int refKd = PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, code,
                new object[] { "CodeStr", code, "Name", constDefaultClsName, "Kl", "0", "Kst", "10" });

            PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 3, "FactReport", 13, "ExcSumPRep", 4, "ExcSumFRep", 14, 
                            "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1, "RefKD", refKd });
            PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 5, "FactReport", 15, "ExcSumPRep", 6, "ExcSumFRep", 16, 
                            "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefKD", refKd });
            PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 7, "FactReport", 17, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefKD", refKd });
            PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 8, "FactReport", 18, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, "RefKD", refKd });
            PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 9, "FactReport", 19, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, "RefKD", refKd });
            PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 10, "FactReport", 20, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, "RefKD", refKd });
            PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 11, "FactReport", 21, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, "RefKD", refKd });
            PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 12, "FactReport", 22, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8, "RefKD", refKd });
        }

        private void PumpFk2TxtOutcomes(string dataRow)
        {
            if (!toPumpOutcomes)
                return;

            string[] rowValues = dataRow.Split('|');

            string fkrCode = rowValues[2].PadRight(14, '0');
            int refFkr = PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, fkrCode,
                new object[] { "Code", fkrCode, "Name", constDefaultClsName });
            string ekrCode = rowValues[5];

            // не качаем нулевой экр ( исключение - фкр - 9600 - тогда качаем)
            if (ekrCode.Trim('0') == string.Empty)
                if (rowValues[2] != "9600")
                    return;

            int refEkr = PumpCachedRow(ekrCache, dsEKR.Tables[0], clsEKR, ekrCode,
                new object[] { "Code", ekrCode, "Name", constDefaultClsName });

            PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 6, "FactReport", 16,  "ExcSumPRep", 7, "ExcSumFRep", 17, 
                            "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1, "RefFKR", refFkr, "RefEKR", refEkr });
            PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 8, "FactReport", 18,  "ExcSumPRep", 9, "ExcSumFRep", 19, 
                            "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefFKR", refFkr, "RefEKR", refEkr });
            PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 10, "FactReport", 20, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefFKR", refFkr, "RefEKR", refEkr });
            PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 11, "FactReport", 21, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, "RefFKR", refFkr, "RefEKR", refEkr });
            PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 12, "FactReport", 22, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, "RefFKR", refFkr, "RefEKR", refEkr });
            PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 13, "FactReport", 23, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, "RefFKR", refFkr, "RefEKR", refEkr });
            PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 14, "FactReport", 24, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, "RefFKR", refFkr, "RefEKR", refEkr });
            PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 15, "FactReport", 25, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8, "RefFKR", refFkr, "RefEKR", refEkr });
        }

        private void PumpFk2TxtDefProf(string dataRow)
        {
            if (!toPumpDefProf)
                return;

            string[] rowValues = dataRow.Split('|');

            PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                new object[] { "YearPlanReport", 6, "FactReport", 16, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1 });
            PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                new object[] { "YearPlanReport", 8, "FactReport", 18, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2 });
            PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                new object[] { "YearPlanReport", 10, "FactReport", 20, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3 });
            PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                new object[] { "YearPlanReport", 11, "FactReport", 21, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11 });
            PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                new object[] { "YearPlanReport", 12, "FactReport", 22, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4 });
            PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                new object[] { "YearPlanReport", 13, "FactReport", 23, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5 });
            PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                new object[] { "YearPlanReport", 14, "FactReport", 24, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6 });
            PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                new object[] { "YearPlanReport", 15, "FactReport", 25, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8 });

        }

        private void PumpFk2TxtOutcomesBlock(string dataRow)
        {
            string[] rowValues = dataRow.Split('|');
            if (rowValues[0].Trim() == "450")
                PumpFk2TxtDefProf(dataRow);
            else
                PumpFk2TxtOutcomes(dataRow);
        }

        private void PumpFk2TxtSifFact(string dataRow)
        {
            if (!toPumpInnerFinSources)
                return;

            string[] rowValues = dataRow.Split('|');
            string code = rowValues[2];
            code = code.PadLeft(20, '0');

            // не качаем строчку - 700|***|01000000000000000|
            if ((rowValues[0] == "700") && (rowValues[2] == "01000000000000000"))
                return;

            int refSrcInFin = PumpCachedRow(srcInFinCache, dsSrcInFin.Tables[0], clsSrcInFin, code,
                new object[] { "CodeStr", code, "Name", constDefaultClsName, "Kl", "0", "Kst", "10" });

            PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 3, "FactReport", 13, "ExcSumPRep", 4, "ExcSumFRep", 14, 
                            "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1, "RefSIF", refSrcInFin });
            PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 5, "FactReport", 15, "ExcSumPRep", 6, "ExcSumFRep", 16, 
                            "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefSIF", refSrcInFin });
            PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 7, "FactReport", 17, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefSIF", refSrcInFin });
            PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 8, "FactReport", 18, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, "RefSIF", refSrcInFin });
            PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 9, "FactReport", 19, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, "RefSIF", refSrcInFin });
            PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 10, "FactReport", 20, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, "RefSIF", refSrcInFin });
            PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 11, "FactReport", 21, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, "RefSIF", refSrcInFin });
            PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 12, "FactReport", 22, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8, "RefSIF", refSrcInFin });
        }

        private void PumpFk2TxtSofFact(string dataRow)
        {
            if (!toPumpOuterFinSources)
                return;

            string[] rowValues = dataRow.Split('|');
            string code = rowValues[2];
            code = code.PadLeft(20, '0');

            int refSrcOutFin = PumpCachedRow(srcOutFinCache, dsSrcOutFin.Tables[0], clsSrcOutFin, code,
                new object[] { "CodeStr", code, "Name", constDefaultClsName, "Kl", "0", "Kst", "10" });

            PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 3, "FactReport", 13, "ExcSumPRep", 4, "ExcSumFRep", 14, 
                            "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1, "RefSOF", refSrcOutFin });
            PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 5, "FactReport", 15, "ExcSumPRep", 6, "ExcSumFRep", 16, 
                            "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefSOF", refSrcOutFin });
            PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 7, "FactReport", 17, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefSOF", refSrcOutFin });
            PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 8, "FactReport", 18, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, "RefSOF", refSrcOutFin });
            PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 9, "FactReport", 19, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, "RefSOF", refSrcOutFin });
            PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 10, "FactReport", 20, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, "RefSOF", refSrcOutFin });
            PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 11, "FactReport", 21, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, "RefSOF", refSrcOutFin });
            PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 12, "FactReport", 22, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8, "RefSOF", refSrcOutFin });
        }

        private void PumpFk2TxtSourcesBlock(string dataRow)
        {
            string[] rowValues = dataRow.Split('|');
            string code = rowValues[2];
            if (code.StartsWith("02"))
                PumpFk2TxtSofFact(dataRow);
            else
                PumpFk2TxtSifFact(dataRow);
        }

        private void PumpFk2TxtAccountFact(string dataRow)
        {
            string[] rowValues = dataRow.Split('|');
            string code = rowValues[0];
            int refMarksAccount = PumpCachedRow(marksAccountCache, dsMarksAccount.Tables[0], clsMarksAccount, code,
                new object[] { "Code", code, "Name", constDefaultClsName });

            PumpFactRow(dsMonthRepAccount.Tables[0], rowValues,
                new object[] { "ArrivalRep", 1, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefAccount", refMarksAccount });
            PumpFactRow(dsMonthRepAccount.Tables[0], rowValues,
                new object[] { "ArrivalRep", 2, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, "RefAccount", refMarksAccount });
            PumpFactRow(dsMonthRepAccount.Tables[0], rowValues,
                new object[] { "ArrivalRep", 3, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, "RefAccount", refMarksAccount });
            PumpFactRow(dsMonthRepAccount.Tables[0], rowValues,
                new object[] { "ArrivalRep", 4, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, "RefAccount", refMarksAccount });
            PumpFactRow(dsMonthRepAccount.Tables[0], rowValues,
                new object[] { "ArrivalRep", 5, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, "RefAccount", refMarksAccount });
            PumpFactRow(dsMonthRepAccount.Tables[0], rowValues,
                new object[] { "ArrivalRep", 6, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8, "RefAccount", refMarksAccount });
        }

        private void PumpFK2TxtFact428(DirectoryInfo dir)
        {
            if (toPumpIncomes)
                ProcessFK2TxtFactFiles428(dir, "*01.txt", "РД=01", "#", new ProcessFK2TxtRowDelegate(PumpFk2TxtIncomes));
            ProcessFK2TxtFactFiles428(dir, "*02.txt", "РД=02", "#", new ProcessFK2TxtRowDelegate(PumpFk2TxtOutcomesBlock));
            ProcessFK2TxtFactFiles428(dir, "*03.txt", "РД=03", "#", new ProcessFK2TxtRowDelegate(PumpFk2TxtSourcesBlock));
            if (toPumpAccount)
                ProcessFK2TxtFactFiles428(dir, "*04.txt", "РД=04", "#", new ProcessFK2TxtRowDelegate(PumpFk2TxtAccountFact));
        }

        #endregion 428

        #region 487

        private void Pump487FK2Txt(DataTable dt, string[] rowValues, string refClsName, int refClsValue)
        {
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 6, "FactReport", 18, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 7, "FactReport", 19, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 12, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 8, "FactReport", 20, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 9, "FactReport", 21, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 13, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 10, "FactReport", 22, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 11, "FactReport", 23, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 17, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 12, "FactReport", 24, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 13, "FactReport", 25, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 14, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 14, "FactReport", 26, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 15, "FactReport", 27, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 15, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 16, "FactReport", 28, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 17, "FactReport", 29, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 16, refClsName, refClsValue });
        }

        private void PumpFk2TxtOutcomesRefsFact(string dataRow)
        {
            if (!toPumpOutcomesRefsAdd)
                return;
            string[] rowValues = dataRow.Split('|');
            string code = dataRow.Split('|')[0];
            string longCode = code.PadRight(22, '0');
            int refOutcomes = PumpCachedRow(marksOutcomesCache, dsMarksOutcomes.Tables[0], clsMarksOutcomes, longCode,
                new object[] { "LongCode", longCode, "Name", constDefaultClsName, "FKR", "0", "KCSR", "0", 
                                   "KVR", "0", "EKR", "0", "Kl", "1", "Kst", code });
            Pump487FK2Txt(dsMonthRepOutcomesBooksEx.Tables[0], rowValues, "RefMarksOutcomes", refOutcomes);
        }

        private void PumpFk2TxtSifRefsFact(string dataRow)
        {
            if (!toPumpInnerFinSourcesRefs)
                return;
            string[] rowValues = dataRow.Split('|');
            string code = dataRow.Split('|')[0];
            string longCode = code.PadRight(22, '0');
            int refInFin = PumpCachedRow(scrInFinSourcesRefCache, dsMarksInDebt.Tables[0], clsMarksInDebt, longCode,
                new object[] { "LongCode", longCode, "Name", constDefaultClsName, "SrcInFin", "0", "GvrmInDebt", "0", 
                                   "Kl", "1", "Kst", code });
            Pump487FK2Txt(dsMonthRepInDebtBooks.Tables[0], rowValues, "RefMarksInDebt", refInFin);
        }

        private void PumpFk2TxtSofRefsFact(string dataRow)
        {
            if (!toPumpOuterFinSourcesRefs)
                return;
            string[] rowValues = dataRow.Split('|');
            string code = dataRow.Split('|')[0];
            string longCode = code.PadRight(22, '0');
            int refOutFin = PumpCachedRow(scrOutFinSourcesRefCache, dsMarksOutDebt.Tables[0], clsMarksOutDebt, longCode,
                new object[] { "LongCode", longCode, "Name", constDefaultClsName, "SrcInFin", "0", "GvrmInDebt", "0", 
                                   "Kl", "1", "Kst", code });
            Pump487FK2Txt(dsMonthRepOutDebtBooks.Tables[0], rowValues, "RefMarksOutDebt", refOutFin);
        }

        private void PumpFk2TxtArrearsRefsFact(string dataRow)
        {
            if (!toPumpArrearsRefs)
                return;
            string[] rowValues = dataRow.Split('|');
            string code = dataRow.Split('|')[0];
            string longCode = code.PadRight(22, '0');
            int refArrears = PumpCachedRow(arrearsCache, dsMarksArrears.Tables[0], clsMarksArrears, longCode,
                new object[] { "LongCode", longCode, "Name", constDefaultClsName, "FKR", "0", "KCSR", "0", 
                                   "KVR", "0", "EKR", "0", "Kl", "1", "Kst", code });
            Pump487FK2Txt(dsMonthRepArrearsBooks.Tables[0], rowValues, "RefMarksArrears", refArrears);
        }

        private void PumpFk2TxtExcessRefsFact(string dataRow)
        {
            if (!toPumpExcessRefs)
                return;
            string[] rowValues = dataRow.Split('|');
            string code = dataRow.Split('|')[0];
            string longCode = code.PadRight(22, '0');
            int refMarks = PumpCachedRow(excessCache, dsMarksExcess.Tables[0], clsMarksExcess, longCode,
                new object[] { "LongCode", longCode, "Name", constDefaultClsName, "FKR", "0", "KCSR", "0", 
                                   "KVR", "0", "EKR", "0", "Kl", "1", "Kst", code });
            Pump487FK2Txt(dsMonthRepExcessBooks.Tables[0], rowValues, "RefMarks", refMarks);
        }

        private void PumpFk2Txt487Facts(string dataRow)
        {
            string[] rowValues = dataRow.Split('|');
            string code = dataRow.Split('|')[0];
            int codeInt = Convert.ToInt32(code);
            // справ расходы доп
            if ((codeInt >= 100 && codeInt < 10500) || (codeInt >= 12100 && codeInt < 12400) || (codeInt >= 13000 && codeInt <= 13602) ||
                (codeInt >= 13000 && codeInt <= 14999 && this.DataSource.Year >= 2012))
                PumpFk2TxtOutcomesRefsFact(dataRow);
            // показатели - внутр долг
            else if ((codeInt >= 10500 && codeInt < 10600) || (codeInt >= 10700 && codeInt < 10800) || (codeInt == 12400))
                PumpFk2TxtSifRefsFact(dataRow);
            // показатели - внешн долг
            else if (codeInt >= 10600 && codeInt < 10700)
                PumpFk2TxtSofRefsFact(dataRow);
            // задолженности
            else if (codeInt >= 10900 && codeInt < 12100)
                PumpFk2TxtArrearsRefsFact(dataRow);
            // остатки
            else if (codeInt >= 10800 && codeInt < 10900)
                PumpFk2TxtExcessRefsFact(dataRow);
        }
        
        private void PumpFK2TxtFact487(DirectoryInfo dir)
        {
            ProcessFK2TxtFactFiles428(dir, "*01.txt", "РД=01", "#", new ProcessFK2TxtRowDelegate(PumpFk2Txt487Facts));
        }

        #endregion 487

        #endregion работа с фактами

        #region общая организация закачки

        private void ProcessFK2TxtDir(DirectoryInfo dir)
        {
            if (formTxt == "428")
            {
                PumpFK2TxtCls428(dir);
                PumpFK2TxtFact428(dir);
            }
            else if (formTxt == "487")
            {
                PumpFK2TxtCls487(dir);
                PumpFK2TxtFact487(dir);
            }
        }

        private void PumpFK2TxtDir(DirectoryInfo dir)
        {
            FileInfo[] archFiles = dir.GetFiles("*.exe", SearchOption.AllDirectories);
            foreach (FileInfo archFile in archFiles)
            {
                WriteToTrace(string.Format("начало закачки архива {0}", archFile.Name), TraceMessageKind.Information);
                DirectoryInfo tempDir = CommonRoutines.ExtractArchiveFileToTempDir(archFile.FullName,
                    FilesExtractingOption.SingleDirectory, ArchivatorName.Rar);
                try
                {
                    formTxt = GetFormFK2Txt(archFile.Name);
                    ProcessFK2TxtDir(tempDir);
                    UpdateData();
                }
                finally
                {
                    CommonRoutines.DeleteDirectory(tempDir);
                }
                WriteToTrace(string.Format("завершение закачки архива {0}", archFile.Name), TraceMessageKind.Information);
            }
        }

        protected override void PumpFK2TxtReports(DirectoryInfo dir)
        {
            refMeansTypeTxt = 1;
            refDateTxt = this.DataSource.Year * 10000 + this.DataSource.Month * 100;
            if (dir.GetFiles("*.exe", SearchOption.AllDirectories).GetLength(0) == 0)
                return;

            toPumpIncomes = (ToPumpBlock(Block.bIncomes));
            toPumpOutcomes = (ToPumpBlock(Block.bOutcomes));
            toPumpDefProf = (ToPumpBlock(Block.bDefProf));
            toPumpInnerFinSources = (ToPumpBlock(Block.bInnerFinSources));
            toPumpOuterFinSources = (ToPumpBlock(Block.bOuterFinSources));
            toPumpAccount = (ToPumpBlock(Block.bAccount));
            toPumpIncomesRefs = (ToPumpBlock(Block.bIncomesRefs));
            toPumpOutcomesRefs = (ToPumpBlock(Block.bOutcomesRefs));
            toPumpOutcomesRefsAdd = (ToPumpBlock(Block.bOutcomesRefsAdd));
            toPumpInnerFinSourcesRefs = (ToPumpBlock(Block.bInnerFinSourcesRefs));
            toPumpOuterFinSourcesRefs = (ToPumpBlock(Block.bOuterFinSourcesRefs));
            toPumpArrearsRefs = (ToPumpBlock(Block.bArrearsRefs));
            toPumpExcessRefs = (ToPumpBlock(Block.bExcessRefs));

            PumpFK2TxtDir(dir);
        }

        #endregion общая организация закачки

    }
}

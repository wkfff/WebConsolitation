using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

using Krista.FM.Providers.DataAccess;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FKMonthRepPump
{
    public partial class FKMonthRepPumpModule : CorrectedPumpModuleBase
    {

        #region поля 

        private int refDateTxt;
        private string formTxt;
        private int refRegionTxt;
        private Dictionary<string, string> ekrNamesCache = new Dictionary<string, string>();
        private Dictionary<string, string> fkrNamesCache = new Dictionary<string, string>();
        private Dictionary<string, string> kifNamesCache = new Dictionary<string, string>();

        #endregion поля

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
                new object[] { "CodeStr", code, "Name", clsRow.Split('|')[1], "StringCode", "10" });
        }

        private void PumpFk2TxtFkr(string clsRow)
        {
            string code = clsRow.Split('|')[0].TrimStart('0').PadLeft(1, '0');
            if (!fkrNamesCache.ContainsKey(code))
                fkrNamesCache.Add(code, clsRow.Split('|')[1]);
        }

        private void PumpFk2TxtEkr(string clsRow)
        {
            string code = clsRow.Split('|')[0].TrimStart('0').PadLeft(1, '0');
            if (!ekrNamesCache.ContainsKey(code))
                ekrNamesCache.Add(code, clsRow.Split('|')[1]);
        }

        private void PumpFk2TxtFinSource(string clsRow)
        {
            string code = clsRow.Split('|')[0];
            if (!kifNamesCache.ContainsKey(code))
                kifNamesCache.Add(code, clsRow.Split('|')[1]);
        }

        private void PumpFK2TxtCls428(DirectoryInfo dir)
        {
            if (toPumpIncomes)
                ProcessFK2TxtClsFiles428(dir, "ST01*01.txt", "ГР=03", "#", new ProcessFK2TxtRowDelegate(PumpFk2TxtKD));
            if (toPumpOutcomes)
            {
                ProcessFK2TxtClsFiles428(dir, "ST01*02.txt", "ГР=03 ПРз", "ГР=04 ЦСР", new ProcessFK2TxtRowDelegate(PumpFk2TxtFkr));
                ProcessFK2TxtClsFiles428(dir, "ST01*02.txt", "ГР=06 ЭКР", "#", new ProcessFK2TxtRowDelegate(PumpFk2TxtEkr));
            }
            if (toPumpFinSources)
                ProcessFK2TxtClsFiles428(dir, "ST01*03.txt", "ГР=03", "#", new ProcessFK2TxtRowDelegate(PumpFk2TxtFinSource));
        }

        #endregion 428

        #region 487

        private void PumpFk2TxtOutcomesRefs(string clsRow)
        {
            string code = clsRow.Split('|')[0];
            int codeInt = Convert.ToInt32(code);
            if ((codeInt >= 100 && codeInt < 10500) || (codeInt >= 12100 && codeInt < 12400) || (codeInt >= 13000 && codeInt <= 13999) ||
                (codeInt >= 13000 && codeInt <= 14999 && this.DataSource.Year >= 2012))
            {
                string longCode = code.PadRight(22, '0');
                PumpCachedRow(outcomesRefsClsCache, dsOutcomesRefsCls.Tables[0], clsOutcomesRefsCls, longCode,
                    new object[] { "LongCode", longCode, "Name", clsRow.Split('|')[1], "FKR", "0", "KCSR", "0", 
                                   "KVR", "0", "EKR", "0", "Kl", "1", "Kst", code });
            }
        }

        private void PumpFk2TxtIFSRefs(string clsRow)
        {
            string code = clsRow.Split('|')[0];
            int codeInt = Convert.ToInt32(code);
            if ((codeInt >= 10500 && codeInt < 10600) || (codeInt >= 10700 && codeInt < 10800) || (codeInt == 12400))
            {
                string longCode = code.PadRight(22, '0');
                PumpCachedRow(iFSRefsClsCache, dsIFSRefsCls.Tables[0], clsIFSRefsCls, longCode,
                    new object[] { "LongCode", longCode, "Name", clsRow.Split('|')[1], "SrcInFin", "0", "GvrmInDebt", "0", 
                                   "Kl", "1", "Kst", code });
            }
        }

        private void PumpFk2TxtOFSRefs(string clsRow)
        {
            string code = clsRow.Split('|')[0];
            int codeInt = Convert.ToInt32(code);
            if (codeInt >= 10600 && codeInt < 10700)
            {
                string longCode = code.PadRight(22, '0');
                PumpCachedRow(oFSRefsClsCache, dsOFSRefsCls.Tables[0], clsOFSRefsCls, longCode,
                    new object[] { "LongCode", longCode, "Name", clsRow.Split('|')[1], "SrcOutFin", "0", "GvrmOutDebt", "0", 
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
                PumpCachedRow(arrearsRefsClsCache, dsArrearsRefsCls.Tables[0], clsArrearsRefsCls, longCode,
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
                PumpCachedRow(excessRefsClsCache, dsExcessRefsCls.Tables[0], clsExcessRefsCls, longCode,
                    new object[] { "LongCode", longCode, "Name", clsRow.Split('|')[1], "FKR", "0", "KCSR", "0", 
                                   "KVR", "0", "EKR", "0", "Kl", "1", "Kst", code });
            }
        }

        private void PumpFK2TxtCls487(DirectoryInfo dir)
        {
            if (toPumpOutcomesRefs)
                ProcessFK2TxtClsFiles428(dir, "ST01*01.txt", "ГР=01", "ГР=02", new ProcessFK2TxtRowDelegate(PumpFk2TxtOutcomesRefs));
            if (toPumpIFSRefs)
                ProcessFK2TxtClsFiles428(dir, "ST01*01.txt", "ГР=01", "ГР=02", new ProcessFK2TxtRowDelegate(PumpFk2TxtIFSRefs));
            if (toPumpOFSRefs)
                ProcessFK2TxtClsFiles428(dir, "ST01*01.txt", "ГР=01", "ГР=02", new ProcessFK2TxtRowDelegate(PumpFk2TxtOFSRefs));
            if (toPumpArrearsRefs)
                ProcessFK2TxtClsFiles428(dir, "ST01*01.txt", "ГР=01", "ГР=02", new ProcessFK2TxtRowDelegate(PumpFk2TxtArrearsRefs));
            if (toPumpExcessRefs)
                ProcessFK2TxtClsFiles428(dir, "ST01*01.txt", "ГР=01", "ГР=02", new ProcessFK2TxtRowDelegate(PumpFk2TxtExcessRefs));
        }

        #endregion 487

        #endregion работа с классификаторами

        #region работа с фактами

        // возвращает тру - если суммы ненулевые, фолс - все суммы нулевые, закачивать не надо
        private bool GetRowValuesTxt(ref object[] mapping, string[] rowValues)
        {
            bool zeroSums = true;
            for (int i = 0; i <= mapping.GetLength(0) - 1; i += 2)
            {
                try
                {
                    if (!mapping[i].ToString().ToUpper().Contains("REP"))
                        continue;
                    string sum = rowValues[Convert.ToInt32(mapping[i + 1])];
                    decimal sumDec = Convert.ToDecimal(sum.Replace('.', ','));
                    mapping[i + 1] = sumDec;
                    if (sumDec != 0)
                        zeroSums = false;
                }
                catch (Exception exp)
                {
                    throw new Exception(string.Format("При получении значения поля '{0}' произошла ошибка: {1}",
                        mapping[i].ToString(), exp.Message));
                }
            }
            return (!zeroSums);
        }

        private void PumpFactRow(DataTable dt, string[] rowValues, object[] mapping)
        {
            if (!GetRowValuesTxt(ref mapping, rowValues))
                return;
            PumpRow(dt, mapping);
        }

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

        private void PumpFK2TxtTerr(string row)
        {
            string regionCode = row.Split('=')[1].Split('-')[0].Trim().PadLeft(10, '0');
            string regionName = row.Split('=')[1].Split('-')[1].Trim();
            string key = string.Format("{0}|{1}", regionCode, regionName);
            refRegionTxt = PumpCachedRow(terrCache, dsTerritoryFK.Tables[0], clsTerritoryFK, key,
                new object[] { "Code", regionCode, "Name", regionName });
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
                            PumpFK2TxtTerr(auxRow);
                        if (auxRow.ToUpper().StartsWith("ДТ="))
                        {
                            string repDate = auxRow.Split('=')[1].Trim();
                            string sourceDate = string.Format("01.{0}.{1}", (this.DataSource.Month + 1).ToString().PadLeft(2, '0'), this.DataSource.Year % 100);
                            if (repDate != sourceDate)
                            {
                                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                                    string.Format("Дата отчета ({0}) не соответствует источнику закачки ({1}).", repDate, sourceDate));
                                break;
                            }
                        }
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
                new object[] { "CodeStr", code, "Name", constDefaultClsName });

            PumpFactRow(dsFKMRIncomes.Tables[0], rowValues,
                new object[] { "AssignedReport", 3, "FactReport", 13, "ExcSumPRep", 4, "ExcSumFRep", 14, "RefMeansType", 1, 
                    "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 1, "RefKD", refKd });
            PumpFactRow(dsFKMRIncomes.Tables[0], rowValues,
                new object[] { "AssignedReport", 5, "FactReport", 15, "ExcSumPRep", 6, "ExcSumFRep", 16, "RefMeansType", 1, 
                    "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 2, "RefKD", refKd });
            PumpFactRow(dsFKMRIncomes.Tables[0], rowValues,
                new object[] { "AssignedReport", 7, "FactReport", 17, "RefMeansType", 1, 
                    "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 3, "RefKD", refKd });
            PumpFactRow(dsFKMRIncomes.Tables[0], rowValues,
                new object[] { "AssignedReport", 8, "FactReport", 18, "RefMeansType", 1, 
                    "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 11, "RefKD", refKd });
            PumpFactRow(dsFKMRIncomes.Tables[0], rowValues,
                new object[] { "AssignedReport", 9, "FactReport", 19, "RefMeansType", 1, 
                    "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 4, "RefKD", refKd });
            PumpFactRow(dsFKMRIncomes.Tables[0], rowValues,
                new object[] { "AssignedReport", 10, "FactReport", 20, "RefMeansType", 1, 
                    "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 5, "RefKD", refKd });
            PumpFactRow(dsFKMRIncomes.Tables[0], rowValues,
                new object[] { "AssignedReport", 11, "FactReport", 21, "RefMeansType", 1, 
                    "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 6, "RefKD", refKd });
            PumpFactRow(dsFKMRIncomes.Tables[0], rowValues,
                new object[] { "AssignedReport", 12, "FactReport", 22, "RefMeansType", 1, 
                    "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 8, "RefKD", refKd });
        }

        private int PumpFk2TxtOutcomesCls(string[] rowValues)
        {
            string kvsr = rowValues[1].PadLeft(3, '0').Replace('*', '0');
            string fkr = rowValues[2].PadLeft(4, '0');
            string kcsr = rowValues[3].PadLeft(7, '0');
            string kvr = rowValues[4].PadLeft(3, '0');
            string ekr = rowValues[5].PadLeft(3, '0');
            string codeStr = string.Format("{0}{1}{2}{3}{4}", kvsr, fkr, kcsr, kvr, ekr);
            string name = string.Empty;
            if (ekr.TrimStart('0') != string.Empty)
                name = ekrNamesCache[ekr.TrimStart('0').PadLeft(1, '0')];
            else
                name = fkrNamesCache[fkr.TrimStart('0').PadLeft(1, '0')];
            return PumpCachedRow(clsOutcomesCache, dsClsOutcomes.Tables[0], clsOutcomes, codeStr,
                new object[] { "CodeStr", codeStr, "Name", name, "kvsr", kvsr, "fkr", fkr, "kcsr", kcsr, "kvr", kvr, "ekr", ekr });
        }

        private void PumpFk2TxtOutcomes(string dataRow)
        {
            if (!toPumpOutcomes)
                return;
            string[] rowValues = dataRow.Split('|');
            int refOutcomes = PumpFk2TxtOutcomesCls(rowValues);

            PumpFactRow(dsFKMROutcomes.Tables[0], rowValues,
                new object[] { "AssignedReport", 6, "FactReport", 16,  "ExcSumPRep", 7, "ExcSumFRep", 17, 
                            "RefMeansType", 1, "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 1, 
                            "RefFKR", nullFKR, "RefEKR2005", nullEKR2005, "refMarks", nullMarksOutcomes, "RefR", refOutcomes });
            PumpFactRow(dsFKMROutcomes.Tables[0], rowValues,
                new object[] { "AssignedReport", 8, "FactReport", 18,  "ExcSumPRep", 9, "ExcSumFRep", 19, 
                            "RefMeansType", 1, "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 2, 
                            "RefFKR", nullFKR, "RefEKR2005", nullEKR2005, "refMarks", nullMarksOutcomes, "RefR", refOutcomes });
            PumpFactRow(dsFKMROutcomes.Tables[0], rowValues,
                new object[] { "AssignedReport", 10, "FactReport", 20, 
                            "RefMeansType", 1, "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 3, 
                            "RefFKR", nullFKR, "RefEKR2005", nullEKR2005, "refMarks", nullMarksOutcomes, "RefR", refOutcomes });
            PumpFactRow(dsFKMROutcomes.Tables[0], rowValues,
                new object[] { "AssignedReport", 11, "FactReport", 21, 
                            "RefMeansType", 1, "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 11, 
                            "RefFKR", nullFKR, "RefEKR2005", nullEKR2005, "refMarks", nullMarksOutcomes, "RefR", refOutcomes });
            PumpFactRow(dsFKMROutcomes.Tables[0], rowValues,
                new object[] { "AssignedReport", 12, "FactReport", 22, 
                            "RefMeansType", 1, "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 4, 
                            "RefFKR", nullFKR, "RefEKR2005", nullEKR2005, "refMarks", nullMarksOutcomes, "RefR", refOutcomes });
            PumpFactRow(dsFKMROutcomes.Tables[0], rowValues,
                new object[] { "AssignedReport", 13, "FactReport", 23, 
                            "RefMeansType", 1, "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 5, 
                            "RefFKR", nullFKR, "RefEKR2005", nullEKR2005, "refMarks", nullMarksOutcomes, "RefR", refOutcomes });
            PumpFactRow(dsFKMROutcomes.Tables[0], rowValues,
                new object[] { "AssignedReport", 14, "FactReport", 24, 
                            "RefMeansType", 1, "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 6, 
                            "RefFKR", nullFKR, "RefEKR2005", nullEKR2005, "refMarks", nullMarksOutcomes, "RefR", refOutcomes });
            PumpFactRow(dsFKMROutcomes.Tables[0], rowValues,
                new object[] { "AssignedReport", 15, "FactReport", 25, 
                            "RefMeansType", 1, "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 8, 
                            "RefFKR", nullFKR, "RefEKR2005", nullEKR2005, "refMarks", nullMarksOutcomes, "RefR", refOutcomes });
        }

        private void PumpFk2TxtDefProf(string dataRow)
        {
            if (!toPumpDefProf)
                return;
            string[] rowValues = dataRow.Split('|');
            PumpFactRow(dsFKMRDefProf.Tables[0], rowValues,
                new object[] { "AssignedReport", 6, "FactReport", 16, "RefMeansType", 1, 
                            "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 1 });
            PumpFactRow(dsFKMRDefProf.Tables[0], rowValues,
                new object[] { "AssignedReport", 8, "FactReport", 18, "RefMeansType", 1, 
                            "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 2 });
            PumpFactRow(dsFKMRDefProf.Tables[0], rowValues,
                new object[] { "AssignedReport", 10, "FactReport", 20, "RefMeansType", 1, 
                            "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 3 });
            PumpFactRow(dsFKMRDefProf.Tables[0], rowValues,
                new object[] { "AssignedReport", 11, "FactReport", 21, "RefMeansType", 1, 
                            "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 11 });
            PumpFactRow(dsFKMRDefProf.Tables[0], rowValues,
                new object[] { "AssignedReport", 12, "FactReport", 22, "RefMeansType", 1, 
                            "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 4 });
            PumpFactRow(dsFKMRDefProf.Tables[0], rowValues,
                new object[] { "AssignedReport", 13, "FactReport", 23, "RefMeansType", 1, 
                            "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 5 });
            PumpFactRow(dsFKMRDefProf.Tables[0], rowValues,
                new object[] { "AssignedReport", 14, "FactReport", 24, "RefMeansType", 1, 
                            "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 6 });
            PumpFactRow(dsFKMRDefProf.Tables[0], rowValues,
                new object[] { "AssignedReport", 15, "FactReport", 25, "RefMeansType", 1, 
                            "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 8 });
        }

        private void PumpFk2TxtOutcomesBlock(string dataRow)
        {
            string[] rowValues = dataRow.Split('|');
            if (rowValues[0].Trim() == "450")
                PumpFk2TxtDefProf(dataRow);
            else
                PumpFk2TxtOutcomes(dataRow);
        }

        private void PumpFk2TxtOutcomes(DirectoryInfo dir)
        {
            ProcessFK2TxtFactFiles428(dir, "*02.txt", "РД=02", "#", new ProcessFK2TxtRowDelegate(PumpFk2TxtOutcomesBlock));
            ekrNamesCache.Clear();
            fkrNamesCache.Clear();
        }

        private void PumpFk2TxtSourcesBlock(string dataRow)
        {
            string[] rowValues = dataRow.Split('|');
            string code = rowValues[2];
            string name = constDefaultClsName;
            if (kifNamesCache.ContainsKey(code))
                name = kifNamesCache[code];
            code = string.Format("000{0}", code);
            int refKif = PumpCachedRow(kifCache, dsKIF.Tables[0], clsKIF, code,
                new object[] { "CodeStr", code, "Name", name, "StringCode", rowValues[0].Trim() });

            PumpFactRow(dsFKMRSrcFin.Tables[0], rowValues,
                new object[] { "AssignedReport", 3, "FactReport", 13, "ExcSumPRep", 4, "ExcSumFRep", 14, "RefMeansType", 1, 
                    "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 1, "RefKIF", refKif });
            PumpFactRow(dsFKMRSrcFin.Tables[0], rowValues,
                new object[] { "AssignedReport", 5, "FactReport", 15, "ExcSumPRep", 6, "ExcSumFRep", 16, "RefMeansType", 1, 
                    "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 2, "RefKIF", refKif });
            PumpFactRow(dsFKMRSrcFin.Tables[0], rowValues,
                new object[] { "AssignedReport", 7, "FactReport", 17, "RefMeansType", 1, 
                    "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 3, "RefKIF", refKif });
            PumpFactRow(dsFKMRSrcFin.Tables[0], rowValues,
                new object[] { "AssignedReport", 8, "FactReport", 18, "RefMeansType", 1, 
                    "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 11, "RefKIF", refKif });
            PumpFactRow(dsFKMRSrcFin.Tables[0], rowValues,
                new object[] { "AssignedReport", 9, "FactReport", 19, "RefMeansType", 1, 
                    "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 4, "RefKIF", refKif });
            PumpFactRow(dsFKMRSrcFin.Tables[0], rowValues,
                new object[] { "AssignedReport", 10, "FactReport", 20, "RefMeansType", 1, 
                    "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 5, "RefKIF", refKif });
            PumpFactRow(dsFKMRSrcFin.Tables[0], rowValues,
                new object[] { "AssignedReport", 11, "FactReport", 21, "RefMeansType", 1, 
                    "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 6, "RefKIF", refKif });
            PumpFactRow(dsFKMRSrcFin.Tables[0], rowValues,
                new object[] { "AssignedReport", 12, "FactReport", 22, "RefMeansType", 1, 
                    "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBudgetLevels", 8, "RefKIF", refKif });
        }

        private void PumpFK2TxtFact428(DirectoryInfo dir)
        {
            if (toPumpIncomes)
                ProcessFK2TxtFactFiles428(dir, "*01.txt", "РД=01", "#", new ProcessFK2TxtRowDelegate(PumpFk2TxtIncomes));
            PumpFk2TxtOutcomes(dir);
            if (toPumpFinSources)
            {
                ProcessFK2TxtFactFiles428(dir, "*03.txt", "РД=03", "#", new ProcessFK2TxtRowDelegate(PumpFk2TxtSourcesBlock));
                kifNamesCache.Clear();
            }
        }

        #endregion 428

        #region 487

        private void Pump487FK2Txt(DataTable dt, string[] rowValues, string refClsName, int refClsValue)
        {
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 6, "FactReport", 18, "RefMeansType", 1, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBdgtLevels", 2, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 7, "FactReport", 19, "RefMeansType", 1, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBdgtLevels", 12, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 8, "FactReport", 20, "RefMeansType", 1, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBdgtLevels", 3, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 9, "FactReport", 21, "RefMeansType", 1, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBdgtLevels", 13, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 10, "FactReport", 22, "RefMeansType", 1, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBdgtLevels", 11, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 11, "FactReport", 23, "RefMeansType", 1, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBdgtLevels", 17, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 12, "FactReport", 24, "RefMeansType", 1, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBdgtLevels", 4, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 13, "FactReport", 25, "RefMeansType", 1, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBdgtLevels", 14, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 14, "FactReport", 26, "RefMeansType", 1, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBdgtLevels", 5, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 15, "FactReport", 27, "RefMeansType", 1, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBdgtLevels", 15, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 16, "FactReport", 28, "RefMeansType", 1, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBdgtLevels", 6, refClsName, refClsValue });
            PumpFactRow(dt, rowValues,
                new object[] { "AssignedReport", 17, "FactReport", 29, "RefMeansType", 1, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefTerritory", refRegionTxt, "RefBdgtLevels", 16, refClsName, refClsValue });
        }

        private void PumpFk2TxtOutcomesRefsFact(string dataRow)
        {
            if (!toPumpOutcomesRefs)
                return;
            string[] rowValues = dataRow.Split('|');
            string code = dataRow.Split('|')[0];
            string longCode = code.PadRight(22, '0');
            int refOutcomes = PumpCachedRow(outcomesRefsClsCache, dsOutcomesRefsCls.Tables[0], clsOutcomesRefsCls, longCode,
                new object[] { "LongCode", longCode, "Name", constDefaultClsName, "FKR", "0", "KCSR", "0", 
                                   "KVR", "0", "EKR", "0", "Kl", "1", "Kst", code });
            Pump487FK2Txt(dsOutcomesRefsFact.Tables[0], rowValues, "RefMarks", refOutcomes);
        }

        private void PumpFk2TxtIFSRefsFact(string dataRow)
        {
            if (!toPumpIFSRefs)
                return;
            string[] rowValues = dataRow.Split('|');
            string code = dataRow.Split('|')[0];
            string longCode = code.PadRight(22, '0');
            int refInFin = PumpCachedRow(iFSRefsClsCache, dsIFSRefsCls.Tables[0], clsIFSRefsCls, longCode,
                new object[] { "LongCode", longCode, "Name", constDefaultClsName, "SrcInFin", "0", "GvrmInDebt", "0", 
                                   "Kl", "1", "Kst", code });
            Pump487FK2Txt(dsIFSRefsFact.Tables[0], rowValues, "RefMarks", refInFin);
        }

        private void PumpFk2TxtOFSRefsFact(string dataRow)
        {
            if (!toPumpOFSRefs)
                return;
            string[] rowValues = dataRow.Split('|');
            string code = dataRow.Split('|')[0];
            string longCode = code.PadRight(22, '0');
            int refOutFin = PumpCachedRow(oFSRefsClsCache, dsOFSRefsCls.Tables[0], clsOFSRefsCls, longCode,
                new object[] { "LongCode", longCode, "Name", constDefaultClsName, "SrcInFin", "0", "GvrmInDebt", "0", 
                                   "Kl", "1", "Kst", code });
            Pump487FK2Txt(dsOFSRefsFact.Tables[0], rowValues, "RefMarks", refOutFin);
        }

        private void PumpFk2TxtArrearsRefsFact(string dataRow)
        {
            if (!toPumpArrearsRefs)
                return;
            string[] rowValues = dataRow.Split('|');
            string code = dataRow.Split('|')[0];
            string longCode = code.PadRight(22, '0');
            int refArrears = PumpCachedRow(arrearsRefsClsCache, dsArrearsRefsCls.Tables[0], clsArrearsRefsCls, longCode,
                new object[] { "LongCode", longCode, "Name", constDefaultClsName, "FKR", "0", "KCSR", "0", 
                                   "KVR", "0", "EKR", "0", "Kl", "1", "Kst", code });
            Pump487FK2Txt(dsArrearsRefsFact.Tables[0], rowValues, "RefMarks", refArrears);
        }

        private void PumpFk2TxtExcessRefsFact(string dataRow)
        {
            if (!toPumpExcessRefs)
                return;
            string[] rowValues = dataRow.Split('|');
            string code = dataRow.Split('|')[0];
            string longCode = code.PadRight(22, '0');
            int refMarks = PumpCachedRow(excessRefsClsCache, dsExcessRefsCls.Tables[0], clsExcessRefsCls, longCode,
                new object[] { "LongCode", longCode, "Name", constDefaultClsName, "FKR", "0", "KCSR", "0", 
                                   "KVR", "0", "EKR", "0", "Kl", "1", "Kst", code });
            Pump487FK2Txt(dsExcessRefsFact.Tables[0], rowValues, "RefMarks", refMarks);
        }

        private void PumpFk2Txt487Facts(string dataRow)
        {
            string[] rowValues = dataRow.Split('|');
            string code = dataRow.Split('|')[0];
            int codeInt = Convert.ToInt32(code);
            if ((codeInt >= 100 && codeInt < 10500) || (codeInt >= 12100 && codeInt < 12400) || (codeInt >= 13000 && codeInt <= 13999) ||
                (codeInt >= 13000 && codeInt <= 14999 && this.DataSource.Year >= 2012))
                PumpFk2TxtOutcomesRefsFact(dataRow);
            else if ((codeInt >= 10500 && codeInt < 10600) || (codeInt >= 10700 && codeInt < 10800) || (codeInt == 12400))
                PumpFk2TxtIFSRefsFact(dataRow);
            else if (codeInt >= 10600 && codeInt < 10700)
                PumpFk2TxtOFSRefsFact(dataRow);
            else if (codeInt >= 10900 && codeInt < 12100)
                PumpFk2TxtArrearsRefsFact(dataRow);
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

            DirectoryInfo[] dirs = dir.GetDirectories("*428*", SearchOption.AllDirectories);
            formTxt = "428";
            ProcessFK2TxtDir(dirs[0]);
            dirs = dir.GetDirectories("*487*", SearchOption.AllDirectories);
            formTxt = "487";
            ProcessFK2TxtDir(dir);
        }

        private void PumpFK2TxtReports(DirectoryInfo dir)
        {
            refDateTxt = this.DataSource.Year * 10000 + this.DataSource.Month * 100;
            if ((dir.GetFiles("*.exe", SearchOption.AllDirectories).GetLength(0) == 0) &&
                (dir.GetFiles("*.txt", SearchOption.AllDirectories).GetLength(0) == 0))
                return;
            PumpFK2TxtDir(dir);
        }

        #endregion общая организация закачки

    }

}


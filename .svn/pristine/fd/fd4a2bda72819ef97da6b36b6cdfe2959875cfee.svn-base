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
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.Common.Xml;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.SKIFMonthRepPump
{
    // закачка шаблонов мес отч - из кит шаблонов бюджета
    public partial class SKIFMonthRepPumpModule : SKIFRepPumpModuleBase
    {
        // с февраля 2007 года - появился новый шаблон - (36.10.14.10)
        private bool isNewRefsPattern = false;

        #region общие функции 

        private string GetKitRefsPatternName()
        {
            if (isNewRefsPattern)
                return "(36.10.14.10)";
            else
                return "(36.10.14.00)";
        }

        private FileInfo GetKitPattern(DirectoryInfo dir, string patternName)
        {
            string patternMask = string.Concat(patternName, "*.kit");
            FileInfo[] patterns = dir.GetFiles(patternMask, SearchOption.AllDirectories);
            if (patterns.GetLength(0) == 0)
                return null;
            else
                return patterns[0]; 
        }

        private XmlNode GetTemplateNode(XmlDocument patternDoc, string templateName)
        {
            string xPath = string.Format("Template/TemplateList/{0}", templateName);
            return patternDoc.SelectSingleNode(xPath);
        }

        // получить индексы колонок кит шаблона (column1 соответствует v1 в данных)
        private void GetKitColumnsIndices(XmlNode templateNode, object[] mapping)
        {
            for (int i = 0; i < mapping.GetLength(0); i += 2)
            {
                string xPath = string.Format("TableInfo/Columns/*[FieldName[@X=\"{0}\"]]", mapping[i + 1].ToString());
                XmlNode columnNode = templateNode.SelectSingleNode(xPath);
                mapping[i + 1] = string.Concat("v", columnNode.Name.Substring(6));
            }
        }

        // получить данные колонки (индексы колонок по нечетным номерам маппинга)
        private object[] GetKitColumnsData(XmlNode dataNode, object[] mapping)
        {
            object[] columsData = (object[])mapping.Clone();
            for (int i = 0; i < mapping.GetLength(0); i += 2)
            {
                XmlNode columnNode = dataNode.SelectSingleNode(mapping[i + 1].ToString());
                columsData[i + 1] = columnNode.Attributes["X"].Value;
            }
            return columsData;
        }

        // в некоторых кодах встречается буковка А - нужно приводить их к английской A
        private void ReplaceA(ref string code)
        {
            code = code.Replace('А', 'A');
            code = code.Replace('а', 'a');
        }

        #region проверка данных расходы

        private bool CheckKitOutcomesColumnsData2007(ref object[] columnsData, IClassifier cls)
        {
            columnsData[1] = columnsData[1].ToString().TrimStart('0').PadLeft(1, '0');
            if (cls == clsFKR)
                // КФСР1 не равно 7900 и КЭСР1 =0.
                return ((columnsData[1].ToString() != "7900") && (columnsData[5].ToString() == "000"));
            else
                // КФСР1 не равно 7900 и КЭСР1 не равно 0.
                return ((columnsData[5].ToString() != "7900") && (columnsData[1].ToString() != "0"));
        }

        private bool CheckKitOutcomesColumnsData2008(ref object[] columnsData, IClassifier cls)
        {
            // КодСтрокиВыч не равен 450
            if (columnsData[5].ToString() == "450")
                return false;
            string code = columnsData[1].ToString();
            if (cls == clsFKR)
            {
                // 18-20 символы поля Код30 равны 0
                if (code.Substring(17, 3) != "000")
                    return false;
                // Код = 4-7 символы поля Код30
                columnsData[1] = code.Substring(3, 4).TrimStart('0').PadLeft(1, '0');
            }
            else
            {
                // Код = 18-20 символы поля Код30
                columnsData[1] = code.Substring(17, 3).TrimStart('0').PadLeft(1, '0');
                if (columnsData[1].ToString() == "0")
                    columnsData[3] = constDefaultClsName;
            }
            return true;
        }

        private bool CheckKitOutcomesColumnsData(ref object[] columnsData, IClassifier cls)
        {
            if (this.DataSource.Year >= 2008)
                return CheckKitOutcomesColumnsData2008(ref columnsData, cls);
            else
                return CheckKitOutcomesColumnsData2007(ref columnsData, cls);
        }

        #endregion проверка данных расходы

        #region проверка данных справ расходы 

        private bool CheckKitOutcomesRefsColumnsData(ref object[] columnsData, IClassifier cls)
        {
            int intCode = Convert.ToInt32(columnsData[5].ToString());
            if (cls == clsFKRBook)
                // Целое1 = 2-16, КФСР <>0
                return ((columnsData[1].ToString() != "0000") && (intCode >= 2) && (intCode <= 16));
            else
                // Целое1  = 2-16, КФСР =0
                return ((columnsData[7].ToString() == "0000") && (intCode >= 2) && (intCode <= 16));
        }

        #endregion проверка данных справ расходы

        #region проверка данных справ расходы доп

        private bool CheckKitOutcomesRefsAddColumnsDataNew(ref object[] columnsData)
        {
            int kst = Convert.ToInt32(columnsData[11]);
            if (this.DataSource.Year >= 2010)
            {
                // kst = [100; 9600), [11300; 11600)
                if (!(((kst >= 100) && (kst < 9600)) || ((kst >= 11300) && (kst < 11600))))
                    return false;
            }
            else if (this.DataSource.Year >= 2009)
            {
                // kst = [0100-9700)
                if ((kst < 100) || (kst >= 9700))
                    return false;
            }
            else if (this.DataSource.Year >= 2008)
            {
                // kst = [0100-8200)
                if ((kst < 100) || (kst >= 8200))
                    return false;
            }
            else
            {
                // kst = 100-2910, 4300-5620
                if (!(((kst >= 100) && (kst <= 2910)) || ((kst >= 4300) && (kst <= 5620))))
                    return false;
            }
            // ФКР = Код30Выч  (4-7 знаки)
            columnsData[1] = columnsData[1].ToString().Substring(3, 4);
            // ЭКР= Код30Выч  (18-20 знаки)
            columnsData[3] = columnsData[3].ToString().Substring(17, 3);
            // КВР = Код30Выч  (15-17 знаки)
            columnsData[5] = columnsData[5].ToString().Substring(14, 3);
            // КЦСР= Код30Выч  (8-14 знаки)
            columnsData[7] = columnsData[7].ToString().Substring(7, 7);
            // Код = ФКР+КЦСР+КВР+ЭКР+KST
            string longCode = string.Concat(columnsData[1].ToString(), columnsData[7].ToString(),
                columnsData[5].ToString(), columnsData[3].ToString(), columnsData[11].ToString());
            ReplaceA(ref longCode);
            columnsData = (object[])CommonRoutines.ConcatArrays(new object[] { "LongCode", longCode, "Kl", "0" }, columnsData);
            return true;
        }

        private bool CheckKitOutcomesRefsAddColumnsDataOld(ref object[] columnsData)
        {
            // Целое1  = 17-75
            int kl = Convert.ToInt32(columnsData[7]);
            if ((kl < 17) || (kl > 75))
                return false;
            // LongCode = ФКР+ЭКР+KST
            string longCode = string.Concat(columnsData[1].ToString(), columnsData[3].ToString(), columnsData[9].ToString());
            ReplaceA(ref longCode);
            columnsData = (object[])CommonRoutines.ConcatArrays(new object[] { "LongCode", longCode }, columnsData);
            return true;
        }

        private bool CheckKitOutcomesRefsAddColumnsData(ref object[] columnsData)
        {
            if (isNewRefsPattern)
                return CheckKitOutcomesRefsAddColumnsDataNew(ref columnsData);
            else
                return CheckKitOutcomesRefsAddColumnsDataOld(ref columnsData);
        }

        #endregion проверка данных справ расходы доп

        #region проверка данных справ источники внутреннего финансирования

        private bool CheckKitInnerFinSourcesRefsColumnsDataNew(ref object[] columnsData)
        {
            int kst = Convert.ToInt32(columnsData[5]);
            if (this.DataSource.Year >= 2010)
            {
                // kst = [9600, 9700), [9800, 9900), [11600]
                if (!(((kst >= 9600) && (kst < 9700)) || ((kst >= 9800) && (kst < 9900)) || (kst == 11600)))
                    return false;
            }
            else if (this.DataSource.Year >= 2009)
            {
                // kst = [9700, 9800), [9900, 10000)
                if (!(((kst >= 9700) && (kst < 9800)) || ((kst >= 9900) && (kst < 10000))))
                    return false;
            }
            else if (this.DataSource.Year >= 2008)
            {
                // kst = [8200, 8300), [8400, 8600)
                if (!(((kst >= 8200) && (kst < 8300)) || ((kst >= 8400) && (kst < 8600))))
                    return false;
            }
            else
            {
                // kst = [4000, 4060]
                if ((kst < 4000) || (kst > 4060))
                    return false;
            }
            // LongCode = SrcInFin + GvrmInDebt + KST
            string longCode = string.Concat(columnsData[1].ToString(), "0", columnsData[5].ToString());
            ReplaceA(ref longCode);
            columnsData = (object[])CommonRoutines.ConcatArrays(new object[] { "LongCode", longCode, "Kl", "0" }, columnsData);
            return true;
        }

        private bool CheckKitInnerFinSourcesRefsColumnsDataOld(ref object[] columnsData)
        {
            // LongCode = SrcInFin + GvrmInDebt + KST
            string longCode = string.Concat(columnsData[1].ToString(), columnsData[3].ToString(), columnsData[9].ToString());
            ReplaceA(ref longCode);
            columnsData = (object[])CommonRoutines.ConcatArrays(new object[] { "LongCode", longCode }, columnsData);
            return true;
        }

        private bool CheckKitInnerFinSourcesRefsColumnsData(ref object[] columnsData)
        {
            if (isNewRefsPattern)
                return CheckKitInnerFinSourcesRefsColumnsDataNew(ref columnsData);
            else
                return CheckKitInnerFinSourcesRefsColumnsDataOld(ref columnsData);
        }

        #endregion проверка данных справ источники внутреннего финансирования

        #region проверка данных справ источники внешнего финансирования

        private bool CheckKitOuterFinSourcesRefsColumnsDataNew(ref object[] columnsData)
        {
            int kst = Convert.ToInt32(columnsData[5]);
            if (this.DataSource.Year >= 2010)
            {
                // kst = [9700, 9800)
                if ((kst < 9700) || (kst >= 9800))
                    return false;
            }
            else if (this.DataSource.Year >= 2009)
            {
                // kst = [9800, 9900)
                if ((kst < 9800) || (kst >= 9900))
                    return false;
            }
            else if (this.DataSource.Year >= 2008)
            {
                // kst = [8300, 8400)
                if ((kst < 8300) || (kst >= 8400))
                    return false;
            }
            else
            {
                // kst = [4100, 4210]
                if ((kst < 4100) || (kst > 4210))
                    return false;
            }
            // LongCode = SrcOutFin + GvrmOutDebt + KST
            string longCode = string.Concat(columnsData[1].ToString(), "0", columnsData[5].ToString());
            ReplaceA(ref longCode);
            columnsData = (object[])CommonRoutines.ConcatArrays(new object[] { "LongCode", longCode, "Kl", "0" }, columnsData);
            return true;
        }

        private bool CheckKitOuterFinSourcesRefsColumnsDataOld(ref object[] columnsData)
        {
            // LongCode = SrcOutFin + GvrmOutDebt + KST
            string longCode = string.Concat(columnsData[1].ToString(), columnsData[3].ToString(), columnsData[9].ToString());
            ReplaceA(ref longCode);
            columnsData = (object[])CommonRoutines.ConcatArrays(new object[] { "LongCode", longCode }, columnsData);
            return true;
        }

        private bool CheckKitOuterFinSourcesRefsColumnsData(ref object[] columnsData)
        {
            if (isNewRefsPattern)
                return CheckKitOuterFinSourcesRefsColumnsDataNew(ref columnsData);
            else
                return CheckKitOuterFinSourcesRefsColumnsDataOld(ref columnsData);
        }

        #endregion проверка данных справ источники внешнего финансирования

        #region проверка данных справ задолженности

        private bool CheckKitArrearsRefsColumnsDataOld(ref object[] columnsData)
        {
            // LongCode = ФКР+ЭКР+KST
            string longCode = string.Concat(columnsData[1].ToString(), columnsData[3].ToString(), columnsData[9].ToString());
            ReplaceA(ref longCode);
            columnsData = (object[])CommonRoutines.ConcatArrays(new object[] { "LongCode", longCode }, columnsData);
            return true;
        }

        private bool CheckKitArrearsRefsColumnsDataNew(ref object[] columnsData)
        {
            int kst = Convert.ToInt32(columnsData[11]);
            if (this.DataSource.Year >= 2010)
            {
                // kst [10000; 11300)
                if ((kst < 10000) || (kst >= 11300))
                    return false;
            }
            else if (this.DataSource.Year >= 2009)
            {
                // kst >= 10100
                if (kst < 10100)
                    return false;
            }
            else if (this.DataSource.Year >= 2008)
            {
                // kst >= 8600
                if (kst < 8600)
                    return false;
            }
            else
            {
                // kst >= 7000
                if (kst < 7000)
                    return false;
            }
            // ФКР = Код30Выч  (4-7 знаки)
            columnsData[1] = columnsData[1].ToString().Substring(3, 4);
            // ЭКР= Код30Выч  (18-20 знаки)
            columnsData[3] = columnsData[3].ToString().Substring(17, 3);
            // КВР = Код30Выч  (15-17 знаки)
            columnsData[5] = columnsData[5].ToString().Substring(14, 3);
            // КЦСР= Код30Выч  (8-14 знаки)
            columnsData[7] = columnsData[7].ToString().Substring(7, 7);
            // Код = ФКР+КЦСР+КВР+ЭКР+KST
            string longCode = string.Concat(columnsData[1].ToString(), columnsData[7].ToString(),
                columnsData[5].ToString(), columnsData[3].ToString(), columnsData[11].ToString());
            ReplaceA(ref longCode);
            columnsData = (object[])CommonRoutines.ConcatArrays(new object[] { "LongCode", longCode, "Kl", "0" }, columnsData);
            return true;
        }

        private bool CheckKitArrearsRefsColumnsData(ref object[] columnsData)
        {
            if (isNewRefsPattern)
                return CheckKitArrearsRefsColumnsDataNew(ref columnsData);
            else
                return CheckKitArrearsRefsColumnsDataOld(ref columnsData);
        }

        #endregion проверка данных справ задолженности

        #region проверка данных справ остатки

        private bool CheckKitExcessRefsColumnsData(ref object[] columnsData)
        {
            int kst = Convert.ToInt32(columnsData[11]);
            if (this.DataSource.Year >= 2010)
            {
                // kst = [9900;10000)
                if ((kst < 9900) || (kst >= 10000))
                    return false;
            }
            else if (this.DataSource.Year >= 2009)
            {
                // kst = [10000-10100)
                if ((kst < 10000) || (kst >= 10100))
                    return false;
            }
            // kst = [8500-8600)
            else if ((kst < 8500) || (kst >= 8600))
                return false;
            // ФКР = Код30Выч  (4-7 знаки)
            columnsData[1] = columnsData[1].ToString().Substring(3, 4);
            // ЭКР= Код30Выч  (18-20 знаки)
            columnsData[3] = columnsData[3].ToString().Substring(17, 3);
            // КВР = Код30Выч  (15-17 знаки)
            columnsData[5] = columnsData[5].ToString().Substring(14, 3);
            // КЦСР= Код30Выч  (8-14 знаки)
            columnsData[7] = columnsData[7].ToString().Substring(7, 7);
            // Код = ФКР+КЦСР+КВР+ЭКР+KST
            string longCode = string.Concat(columnsData[1].ToString(), columnsData[7].ToString(),
                columnsData[5].ToString(), columnsData[3].ToString(), columnsData[11].ToString());
            ReplaceA(ref longCode);
            columnsData = (object[])CommonRoutines.ConcatArrays(new object[] { "LongCode", longCode, "Kl", "0" }, columnsData);
            return true;
        }

        #endregion проверка данных справ остатки

        // проверка - нужно ли закачивать эту строку
        private bool CheckColumnsData(ref object[] columnsData, IClassifier cls)
        {
            switch (block)
            {
                case Block.bOutcomes:
                    return CheckKitOutcomesColumnsData(ref columnsData, cls);
                case Block.bInnerFinSources:
                    // КодСтрокиВыч не равно 620
                    return (columnsData[7].ToString() != "620");
                case Block.bOuterFinSources:
                    // КодСтрокиВыч равно 620
                    return (columnsData[7].ToString() == "620");
                case Block.bOutcomesRefs:
                    return CheckKitOutcomesRefsColumnsData(ref columnsData, cls);
                case Block.bOutcomesRefsAdd:
                    return CheckKitOutcomesRefsAddColumnsData(ref columnsData);
                case Block.bInnerFinSourcesRefs:
                    return CheckKitInnerFinSourcesRefsColumnsData(ref columnsData);
                case Block.bOuterFinSourcesRefs:
                    return CheckKitOuterFinSourcesRefsColumnsData(ref columnsData);
                case Block.bArrearsRefs:
                    return CheckKitArrearsRefsColumnsData(ref columnsData);
                case Block.bExcessRefs:
                    return CheckKitExcessRefsColumnsData(ref columnsData);
                default: 
                    return true;
            }
        }

        private const string AUX_COLUMN_NAME = "AuxColumn";
        private object[] DeleteAuxColumns(object[] columnsData)
        {
            for (int i = 0; i < columnsData.GetLength(0); i += 2)
                if (columnsData[i].ToString().StartsWith(AUX_COLUMN_NAME))
                    return (object[])CommonRoutines.RemoveArrayPart(columnsData, i, columnsData.GetLength(0) - i);
            return columnsData;
        }

        private void PumpKitData(XmlNode templateNode, object[] mapping, DataTable dt, 
            Dictionary<string, int> cache, IClassifier cls)
        {
            XmlNodeList dataNodes = templateNode.SelectNodes("Matrix/Strms/S0/Cells/*");
            for (int i = 0; i < dataNodes.Count; i++)
            {
                XmlNode dataNode = dataNodes.Item(i);
                object[] columnsData = GetKitColumnsData(dataNode, mapping);
                if (!CheckColumnsData(ref columnsData, cls))
                    continue;
                columnsData = DeleteAuxColumns(columnsData);
                PumpCachedRow(cache, dt, cls, columnsData[1].ToString(), columnsData);
            }
        }

        // маппинг - соответствие полей нашего объекта наименованиям колонок кит шаблона (имя поля нашего объекта, имя кит колонки, ..., ...  )
        private void ProcessKitPattern(DirectoryInfo dir, string patternName, object[] mapping, string templateName, DataTable dt, 
            Dictionary<string, int> cache, IClassifier cls)
        {
            FileInfo pattern = GetKitPattern(dir, patternName);
            if (pattern == null)
                return;
            XmlDocument patternDoc = new XmlDocument();
            try
            {
                patternDoc.Load(pattern.FullName);
                XmlNode templateNode = GetTemplateNode(patternDoc, templateName);
                if (templateNode == null)
                    return;
                GetKitColumnsIndices(templateNode, mapping);
                PumpKitData(templateNode, mapping, dt, cache, cls);
            }
            finally
            {
                XmlHelper.ClearDomDocument(ref patternDoc);
            }
        }

        #endregion общие функции

        #region функции закачки классификаторов

        #region доходы

        private void PumpKitIncomes(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'КД.МесОтч'");
            object[] mapping = new string[] { "CodeStr", "Код30", "Name", "ПримечаниеВыч", "Kl", "КодСтрокиЦелое", "Kst", "КодСтрокиВыч" };
            string patternName = string.Empty;
            if (this.DataSource.Year >= 2008)
                patternName = "(36.10.07.00)";
            else
                patternName = "(36.10.28.00)";
            ProcessKitPattern(dir, patternName, mapping, "Template_0", dsKD.Tables[0], kdCache, clsKD);

            mapping = new string[] { "CodeStr", "Код30", "Name", "ПримечаниеВыч", "Kl", "КодСтрокиЦелое", "Kst", "КодСтрокиВыч" };
            if (this.DataSource.Year >= 2008)
                patternName = "(36.10.17.00)";
            else
                patternName = "(36.10.28.06)";
            ProcessKitPattern(dir, patternName, mapping, "Template_0", dsKD.Tables[0], kdCache, clsKD);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'КД.МесОтч'");
        }

        #endregion доходы

        #region расходы

        #region фкр

        private object[] GetFkrMapping()
        {
            if (this.DataSource.Year >= 2008)
                return new string[] { "Code", "Код30", "Name", "ПримечаниеВыч", "AuxColumn", "КодСтрокиВыч" };
            else
                return new string[] { "Code", "КФСР1", "Name", "ПримечаниеВыч", "AuxColumn", "КЭСР1" };
        }

        private void PumpKitFkr(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'ФКР.МесОтч'");
            object[] mapping = GetFkrMapping();
            string patternName = string.Empty;
            if (this.DataSource.Year >= 2008)
                patternName = "(36.10.07.00)";
            else
                patternName = "(36.10.28.00)";
            ProcessKitPattern(dir, patternName, mapping, "Template_1", dsFKR.Tables[0], fkrCache, clsFKR);

            mapping = GetFkrMapping();
            if (this.DataSource.Year >= 2008)
                patternName = "(36.10.17.00)";
            else
                patternName = "(36.10.28.06)";
            ProcessKitPattern(dir, "(36.10.28.06)", mapping, "Template_1", dsFKR.Tables[0], fkrCache, clsFKR);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'ФКР.МесОтч'");
        }

        #endregion фкр

        #region экр

        private object[] GetEkrMapping()
        {
            if (this.DataSource.Year >= 2008)
                return new string[] { "Code", "Код30", "Name", "ПримечаниеВыч", "AuxColumn", "КодСтрокиВыч" };
            else
                return new string[] { "Code", "КЭСР1", "Name", "ПримечаниеВыч", "AuxColumn", "КФСР1" };
        }

        private void PumpKitEkr(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'ЭКР.МесОтч'");
            object[] mapping = GetEkrMapping();
            string patternName = string.Empty;
            if (this.DataSource.Year >= 2008)
                patternName = "(36.10.07.00)";
            else
                patternName = "(36.10.28.00)";
            ProcessKitPattern(dir, patternName, mapping, "Template_1", dsEKR.Tables[0], ekrCache, clsEKR);

            mapping = GetEkrMapping();
            if (this.DataSource.Year >= 2008)
                patternName = "(36.10.17.00)";
            else
                patternName = "(36.10.28.06)";
            ProcessKitPattern(dir, patternName, mapping, "Template_1", dsEKR.Tables[0], ekrCache, clsEKR);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'ЭКР.МесОтч'");
        }

        #endregion экр

        private void PumpKitOutcomes(DirectoryInfo dir)
        {
            PumpKitFkr(dir);
            PumpKitEkr(dir);
        }

        #endregion расходы

        #region источники внутреннего финансирования

        private void PumpKitInnerFinSources(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'КИВнутрФ.МесОтч2005'");
            object[] mapping = new string[] { "CodeStr", "Код30", "Name", "ПримечаниеВыч", "Kl", "КодСтрокиЦелое", "Kst", "КодСтрокиВыч" };
            string patternName = string.Empty;
            string templateName = string.Empty;
            if (this.DataSource.Year >= 2008)
            {
                patternName = "(36.10.07.00)";
                templateName = "Template_2";
            }
            else
            {
                patternName = "(36.10.28.06)";
                templateName = "Template_3";
            }
            ProcessKitPattern(dir, patternName, mapping, templateName, dsSrcInFin.Tables[0], srcInFinCache, clsSrcInFin);

            mapping = new string[] { "CodeStr", "Код30", "Name", "ПримечаниеВыч", "Kl", "КодСтрокиЦелое", "Kst", "КодСтрокиВыч" };
            if (this.DataSource.Year >= 2008)
            {
                patternName = "(36.10.17.00)";
                templateName = "Template_2";
            }
            else
            {
                patternName = "(36.10.28.00)";
                templateName = "Template_4";
            }
            ProcessKitPattern(dir, patternName, mapping, templateName, dsSrcInFin.Tables[0], srcInFinCache, clsSrcInFin);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'КИВнутрФ.МесОтч2005'");
        }

        #endregion источники внутреннего финансирования

        #region источники внешнего финансирования

        private void PumpKitOuterFinSources(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'КИВнешФ.МесОтч2005'");
            object[] mapping = new string[] { "CodeStr", "Код30", "Name", "ПримечаниеВыч", "Kl", "КодСтрокиЦелое", "Kst", "КодСтрокиВыч" };
            string patternName = string.Empty;
            string templateName = string.Empty;
            if (this.DataSource.Year >= 2008)
            {
                patternName = "(36.10.07.00)";
                templateName = "Template_2";
            }
            else
            {
                patternName = "(36.10.28.06)";
                templateName = "Template_3";
            }
            ProcessKitPattern(dir, patternName, mapping, templateName, dsSrcOutFin.Tables[0], srcOutFinCache, clsSrcOutFin);

            mapping = new string[] { "CodeStr", "Код30", "Name", "ПримечаниеВыч", "Kl", "КодСтрокиЦелое", "Kst", "КодСтрокиВыч" };
            if (this.DataSource.Year >= 2008)
            {
                patternName = "(36.10.17.00)";
                templateName = "Template_2";
            }
            else
            {
                patternName = "(36.10.28.00)";
                templateName = "Template_4";
            }
            ProcessKitPattern(dir, patternName, mapping, templateName, dsSrcOutFin.Tables[0], srcOutFinCache, clsSrcOutFin);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'КИВнешФ.МесОтч2005'");
        }

        #endregion источники внешнего финансирования

        #region справ доходы

        private void PumpKitIncomesRefs(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Администратор.МесОтч'");
            object[] mapping = new string[] { "Code", "КЭСР", "Name", "ПримечаниеВыч", "Kl", "Целое1", "Kst", "КодСтрокиЦелое" };
            ProcessKitPattern(dir, "(36.10.14.00)", mapping, "Template_0", dsKVSR.Tables[0], kvsrCache, clsKVSR);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Администратор.МесОтч'");
        }

        #endregion справ доходы

        #region справ расходы

        private void PumpKitFkrBook(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'ФКР.МесОтч_СправРасходы'");
            object[] mapping = new string[] { "Code", "КФСР", "Name", "ПримечаниеВыч", "AuxColumn", "Целое1" };
            ProcessKitPattern(dir, "(36.10.14.00)", mapping, "Template_1", dsFKRBook.Tables[0], fkrBookCache, clsFKRBook);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'ФКР.МесОтч_СправРасходы'");
        }

        private void PumpKitEkrBook(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'ЭКР.МесОтч_СправРасходы'");
            object[] mapping = new string[] { "Code", "КЭСР", "Name", "ПримечаниеВыч", "AuxColumn1", "Целое1", "AuxColumn2", "КФСР" };
            ProcessKitPattern(dir, "(36.10.14.00)", mapping, "Template_1", dsEKRBook.Tables[0], ekrBookCache, clsEKRBook);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'ЭКР.МесОтч_СправРасходы'");
        }

        private void PumpKitOutcomesRefs(DirectoryInfo dir)
        {
            PumpKitFkrBook(dir);
            PumpKitEkrBook(dir);
        }

        #endregion справ расходы

        #region справ расходы доп

        private object[] GetKitOutcomesRefsAddMapping()
        {
            if (isNewRefsPattern)
                return new string[] { "FKR", "Код30Выч", "EKR", "Код30Выч", "KVR", "Код30Выч", "KCSR", "Код30Выч", 
                    "Name", "ПримечаниеВыч", "Kst", "КодСтрокиЦелое" };
            else
                return new string[] { "FKR", "КФСР", "EKR", "КЭСР", "Name", "ПримечаниеВыч", "Kl", "Целое1", "Kst", "КодСтрокиЦелое" };
        }

        private string GetKitOutcomesRefsAddTemplate()
        {
            if (isNewRefsPattern)
                return "Template_0";
            else
                return "Template_1";
        }

        private void PumpKitOutcomesRefsAdd(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Показатели.МесОтч_СпрРасходы'");
            object[] mapping = GetKitOutcomesRefsAddMapping();
            ProcessKitPattern(dir, GetKitRefsPatternName(), mapping, GetKitOutcomesRefsAddTemplate(), 
                dsMarksOutcomes.Tables[0], marksOutcomesCache, clsMarksOutcomes);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Показатели.МесОтч_СпрРасходы'");
        }

        #endregion справ расходы доп

        #region справ источники внутреннего финансирования

        private object[] GetKitInnerFinSourcesRefsMapping()
        {
            if (isNewRefsPattern)
                return new string[] { "SrcInFin", "Код30Выч", "Name", "ПримечаниеВыч", "Kst", "КодСтрокиЦелое" };
            else
                return new string[] { "SrcInFin", "Код30", "GvrmInDebt", "Целое2", "Name", "ПримечаниеВыч", 
                    "Kl", "КодСтрокиЦелое", "Kst", "Целое1" };
        }

        private string GetKitInnerFinSourcesRefsTemplate()
        {
            if (isNewRefsPattern)
                return "Template_0";
            else
                return "Template_2";
        }

        private void PumpKitInnerFinSourcesRefs(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Показатели.МесОтч_СпрВнутрДолг'");
            object[] mapping = GetKitInnerFinSourcesRefsMapping();
            ProcessKitPattern(dir, GetKitRefsPatternName(), mapping, GetKitInnerFinSourcesRefsTemplate(),
                dsMarksInDebt.Tables[0], scrInFinSourcesRefCache, clsMarksInDebt);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Показатели.МесОтч_СпрВнутрДолг'");
        }

        #endregion справ источники внутреннего финансирования

        #region справ источники внешнего финансирования

        private object[] GetKitOuterFinSourcesRefsMapping()
        {
            if (isNewRefsPattern)
                return new string[] { "SrcOutFin", "Код30Выч", "Name", "ПримечаниеВыч", "Kst", "КодСтрокиЦелое" };
            else
                return new string[] { "SrcOutFin", "Код30", "GvrmOutDebt", "Целое2", "Name", "ПримечаниеВыч", 
                    "Kl", "КодСтрокиЦелое", "Kst", "Целое1" };
        }

        private string GetKitOuterFinSourcesRefsTemplate()
        {
            if (isNewRefsPattern)
                return "Template_0";
            else
                return "Template_3";
        }

        private void PumpKitOuterFinSourcesRefs(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Показатели.МесОтч_СпрВнешДолг'");
            object[] mapping = GetKitOuterFinSourcesRefsMapping();
            ProcessKitPattern(dir, GetKitRefsPatternName(), mapping, GetKitOuterFinSourcesRefsTemplate(),
                dsMarksOutDebt.Tables[0], scrOutFinSourcesRefCache, clsMarksOutDebt);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Показатели.МесОтч_СпрВнешДолг'");
        }

        #endregion справ источники внешнего финансирования

        #region справ задолженности

        private object[] GetKitArrearsRefsMapping()
        {
            if (isNewRefsPattern)
                return new string[] { "FKR", "Код30Выч", "EKR", "Код30Выч", "KVR", "Код30Выч", "KCSR", "Код30Выч", 
                    "Name", "ПримечаниеВыч", "Kst", "КодСтрокиЦелое" };
            else
                return new string[] { "FKR", "КФСР", "EKR", "КЭСР", "Name", "ПримечаниеВыч", "Kl", "КодСтрокиЦелое", "Kst", "Целое1" };
        }

        private string GetKitArrearsRefsTemplate()
        {
            if (isNewRefsPattern)
                return "Template_0";
            else
                return "Template_4";
        }

        private void PumpKitArrearsRefs(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Показатели.МесОтч_СпрЗадолженность'");
            object[] mapping = GetKitArrearsRefsMapping();
            ProcessKitPattern(dir, GetKitRefsPatternName(), mapping, GetKitArrearsRefsTemplate(),
                dsMarksArrears.Tables[0], arrearsCache, clsMarksArrears);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Показатели.МесОтч_СпрЗадолженность'");
        }

        #endregion справ задолженности

        #region справ остатки

        private object[] GetKitExcessRefsMapping()
        {
            return new string[] { "FKR", "Код30Выч", "EKR", "Код30Выч", "KVR", "Код30Выч", "KCSR", "Код30Выч", 
                "Name", "ПримечаниеВыч", "Kst", "КодСтрокиЦелое" };
        }

        private void PumpKitExcessRefs(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Показатели.МесОтч_СпрОстатки'");
            object[] mapping = GetKitExcessRefsMapping();
            ProcessKitPattern(dir, GetKitRefsPatternName(), mapping, "Template_0",
                dsMarksExcess.Tables[0], excessCache, clsMarksExcess);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Показатели.МесОтч_СпрОстатки'");
        }

        #endregion справ остатки

        #endregion функции закачки классификаторов

        #region общая организация закачки

        protected override void PumpKITPattern(DirectoryInfo dir)
        {
            this.isKitPatterns = true;
            if (ToPumpBlock(Block.bIncomes))
            {
                block = Block.bIncomes;
                PumpKitIncomes(dir);
            }
            if (ToPumpBlock(Block.bOutcomes))
            {
                block = Block.bOutcomes;
                PumpKitOutcomes(dir);
            }
            if (ToPumpBlock(Block.bInnerFinSources))
            {
                block = Block.bInnerFinSources;
                PumpKitInnerFinSources(dir);
            }
            if (ToPumpBlock(Block.bOuterFinSources))
            {
                block = Block.bOuterFinSources;
                PumpKitOuterFinSources(dir);
            }
            if (ToPumpBlock(Block.bIncomesRefs))
            {
                block = Block.bIncomesRefs;
                PumpKitIncomesRefs(dir);
            }
            if (ToPumpBlock(Block.bOutcomesRefs))
            {
                block = Block.bOutcomesRefs;
                PumpKitOutcomesRefs(dir);
            }
            isNewRefsPattern = (this.DataSource.Year * 100 + this.DataSource.Month >= 200702);
            if (ToPumpBlock(Block.bOutcomesRefsAdd))
            {
                block = Block.bOutcomesRefsAdd;
                PumpKitOutcomesRefsAdd(dir);
            }
            if (ToPumpBlock(Block.bInnerFinSourcesRefs))
            {
                block = Block.bInnerFinSourcesRefs;
                PumpKitInnerFinSourcesRefs(dir);
            }
            if (ToPumpBlock(Block.bOuterFinSourcesRefs))
            {
                block = Block.bOuterFinSourcesRefs;
                PumpKitOuterFinSourcesRefs(dir);
            }
            if (ToPumpBlock(Block.bArrearsRefs))
            {
                block = Block.bArrearsRefs;
                PumpKitArrearsRefs(dir);
            }
            if (ToPumpBlock(Block.bExcessRefs))
            {
                block = Block.bExcessRefs;
                PumpKitExcessRefs(dir);
            }
            UpdateData();
        }

        #endregion общая организация закачки

    }

}

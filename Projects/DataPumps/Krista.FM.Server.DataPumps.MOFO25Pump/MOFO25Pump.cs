using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.MOFO25Pump
{

    // МОФО - 0025 - Начисления по земельному налогу
    public class MOFO25PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Районы.Планирование (d_Regions_Plan)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> cacheRegions = null;

        #endregion Классификаторы

        #region Факты

        // Показатели.МОФО_Начисления земельного налога (f_Marks_LandTax)
        private IDbDataAdapter daLandTax;
        private DataSet dsLandTax;
        private IFactTable fctLandTax;

        #endregion Факты

        private int sourceID = -1;
        private List<int> deletedSourceIDList = null;
        private List<int> deletedRegionsList = null;
        // пустая запись в таблице фактов, добавляется
        // когда файле по одному муниципальному образованию нет данных
        private DataRow nullFactRow = null;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        private void SetNewSourceID()
        {
            sourceID = this.AddDataSource("ФО", "0029", ParamKindTypes.Year, string.Empty,
                this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;

            if (deletedSourceIDList == null)
                deletedSourceIDList = new List<int>();

            if (!deletedSourceIDList.Contains(sourceID) && this.DeleteEarlierData)
            {
                DirectDeleteFactData(new IFactTable[] { fctLandTax }, -1, sourceID, string.Empty);
                deletedSourceIDList.Add(sourceID);
            }
        }

        private void FillRegionsCache(DataTable dt)
        {
            cacheRegions = new Dictionary<string, int>();
            foreach (DataRow row in dt.Rows)
            {
                string codeLine = Convert.ToString(row["CodeLine"]);
                // если поле CodeLine заполнено, то это МР или ГО, его просто добавляем в кэш
                if (codeLine == string.Empty)
                {
                    // если нет, то это поселение. в этом случае порядковый номер CodeLine будет составной:
                    // в начале идут первые 1 или 2 цифры из поля Code, дополненные до 3-х знаков нулями;
                    // в конце - последние 2 цифры из поля Code
                    string code = Convert.ToString(row["Code"]);
                    if (code.Length < 5)
                        continue;
                    codeLine = code.Substring(0, code.Length - 4).PadLeft(3, '0') + code.Substring(code.Length - 2);
                }
                if (!cacheRegions.ContainsKey(codeLine))
                {
                    cacheRegions.Add(codeLine, Convert.ToInt32(row["ID"]));
                }
            }
        }

        protected override void QueryData()
        {
            SetNewSourceID();

            string constraint = string.Format("SourceId = {0}", sourceID);
            InitDataSet(ref daRegions, ref dsRegions, clsRegions, constraint);
            InitFactDataSet(ref daLandTax, ref dsLandTax, fctLandTax);

            FillRegionsCache(dsRegions.Tables[0]);
        }

        #region GUIDs

        private const string D_REGIONS_PLAN_GUID = "1f34cc90-16fd-4fcf-b994-0c8a680d7e23";
        private const string F_MARKS_LAND_TAX_GUID = "54955310-2967-454d-ac46-87e227faf7cc";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsRegions = this.Scheme.Classifiers[D_REGIONS_PLAN_GUID];
            fctLandTax = this.Scheme.FactTables[F_MARKS_LAND_TAX_GUID];

            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { fctLandTax };
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daLandTax, dsLandTax, fctLandTax);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsLandTax);
            ClearDataSet(ref dsRegions);
        }

        #endregion Работа с базой и кэшами

        #region Работа с Xls

        private decimal CleanFactValue(string value)
        {
            decimal factValue = 0;
            Decimal.TryParse(CommonRoutines.TrimLetters(value).Replace('.', ','), out factValue);
            return factValue;
        }

        private void PumpFactRow(string value, int refDate, int refRegions, int refMarks)
        {
            decimal factValue = CleanFactValue(value);
            if (factValue == 0)
                return;

            if (refMarks != 0)
                factValue *= 1000;

            object[] mapping = new object[] {
                "Value", factValue,
                "RefYearDayUNV", refDate,
                "RefRegions", refRegions,
                "RefMarks", refMarks,
                "SourceID", sourceID
            };

            PumpRow(dsLandTax.Tables[0], mapping);
        }

        private void DeleteEarlierDataByRegions(int refRegions)
        {
            if (!deletedRegionsList.Contains(refRegions) && !this.DeleteEarlierData)
            {
                string constr = string.Format("RefRegions = {0}", refRegions);
                DirectDeleteFactData(new IFactTable[] { fctLandTax }, -1, sourceID, constr);
                deletedRegionsList.Add(refRegions);
            }
        }

        private int GetRefRegions(string value)
        {
            // код берем из названия муниципального образования, например:
            // 064  Городской округ  Реутов - код 064
            // 02904 Сельское поселение Данковское - код 02904
            string code = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();

            if (!cacheRegions.ContainsKey(code))
            {
                throw new PumpDataFailedException(string.Format(
                    "Не найдено муниципальное образование с кодом {0} в справочнике «Районы.Планирование».", code));
            }

            DeleteEarlierDataByRegions(cacheRegions[code]);

            return cacheRegions[code];
        }

        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refDate)
        {
            string regionValue = excelDoc.GetValue(curRow, 2).Trim();
            int refRegions = GetRefRegions(regionValue);
            if (regionValue.ToUpper().Contains("ГОРОДСКОЙ ОКРУГ"))
                nullFactRow["RefRegions"] = refRegions;

            int shift = 3;
            for (int i = 0; i <= 6; i++)
                PumpFactRow(excelDoc.GetValue(curRow, shift + i).Trim(), refDate, refRegions, i);
        }

        private bool SkipRow(string cellValue)
        {
            if (cellValue.ToUpper().Contains("МУНИЦИПАЛЬНЫЙ РАЙОН"))
            {
                nullFactRow["RefRegions"] = GetRefRegions(cellValue);
                return true;
            }

            return (cellValue == string.Empty);
        }

        private void PumpXlsSheet(ExcelHelper excelDoc, int refDate)
        {
            bool toPump = false;
            for (int curRow = 1; ; curRow++)
                try
                {
                    string cellValue = excelDoc.GetValue(curRow, 2).Trim();
                    if (SkipRow(cellValue))
                        continue;

                    if (cellValue.ToUpper().StartsWith("ГЛАВА"))
                        break;

                    if (toPump)
                        PumpXlsRow(excelDoc, curRow, refDate);

                    if (cellValue == "1")
                        toPump = true;
                }
                catch (Exception ex)
                {
                    throw new PumpDataFailedException(string.Format(
                        "При обработке листа '{0}' возникла ошибка ({1})",
                        excelDoc.GetWorksheetName(), ex.Message));
                }
        }

        private DataRow GetNullFactRow(int refDate)
        {
            DataRow row = dsLandTax.Tables[0].NewRow();

            row["Value"] = DBNull.Value;
            row["RefMarks"] = 1;
            row["RefYearDayUNV"] = refDate;
            row["PumpID"] = this.PumpID;
            row["SourceID"] = sourceID;

            return row;
        }

        private int GetRefDate()
        {
            return (this.DataSource.Year + 1) * 10000 + 1;
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                int refDate = GetRefDate();
                nullFactRow = GetNullFactRow(refDate);

                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.EnableEvents = false;
                excelDoc.OpenDocument(file.FullName);

                excelDoc.SetWorksheet(1);
                PumpXlsSheet(excelDoc, refDate);

                if (dsLandTax.Tables[0].Rows.Count == 0)
                    dsLandTax.Tables[0].Rows.Add(nullFactRow);
                UpdateData();
            }
            catch (PumpDataFailedException ex)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                    "Ошибка при обработке файла '{0}': {1} Файл будет пропущен.", file.Name, ex.Message));
            }
            finally
            {
                ClearDataSet(daLandTax, ref dsLandTax);
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Xls

        #region Перекрытые методы

        protected override void DeleteEarlierPumpedData()
        {
            // заглушка
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedRegionsList = new List<int>();
            try
            {
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
                UpdateData();
            }
            finally
            {
                deletedRegionsList.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы

        #endregion Закачка данных

    }

}

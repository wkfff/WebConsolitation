using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.MOFO29Pump
{

    // МОФО - 0029 - Начисления по НИФЛ
    public class MOFO29PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Вариант.МОФО_Начисления по НИФЛ (d_Variant_PropertyTax)
        private IDbDataAdapter daVariant;
        private DataSet dsVariant;
        private IClassifier clsVariant;
        private Dictionary<string, int> cacheVariant = null;
        private int maxVariantCode = 0;
        // Районы.Планирование (d_Regions_Plan)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> cacheRegions = null;

        #endregion Классификаторы

        #region Факты

        // Факт.МОФО_Начисления по НИФЛ (f_F_PropertyTax)
        private IDbDataAdapter daPropertyTax;
        private DataSet dsPropertyTax;
        private IFactTable fctPropertyTax;

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
                this.DataSource.Year + 1, 0, string.Empty, 0, string.Empty).ID;

            if (deletedSourceIDList == null)
                deletedSourceIDList = new List<int>();

            if (!deletedSourceIDList.Contains(sourceID) && this.DeleteEarlierData)
            {
                DirectDeleteFactData(new IFactTable[] { fctPropertyTax }, -1, sourceID, string.Empty);
                deletedSourceIDList.Add(sourceID);
            }
        }

        private int GetMaxCode(IEntity cls)
        {
            string query = string.Format("select max(Code) from {0}", cls.FullDBName);
            object result = this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { });
            if ((result == null) || (result == DBNull.Value))
                return 0;
            return Convert.ToInt32(result);
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

        private void FillCaches()
        {
            FillRowsCache(ref cacheVariant, dsVariant.Tables[0], "Name");
            FillRegionsCache(dsRegions.Tables[0]);
        }

        protected override void QueryData()
        {
            SetNewSourceID();

            string constraint = string.Format("SourceID = {0}", sourceID);
            InitDataSet(ref daRegions, ref dsRegions, clsRegions, constraint);
            InitDataSet(ref daVariant, ref dsVariant, clsVariant, string.Empty);
            InitFactDataSet(ref daPropertyTax, ref dsPropertyTax, fctPropertyTax);

            FillCaches();
            maxVariantCode = GetMaxCode(clsVariant);
        }

        #region GUIDs

        private const string D_VARIANT_PROPERTY_TAX_GUID = "5eedf8c9-5adc-446a-b563-4dfebc853da3";
        private const string D_REGIONS_PLAN_GUID = "1f34cc90-16fd-4fcf-b994-0c8a680d7e23";
        private const string F_F_PROPERTY_TAX_GUID = "4288382c-5e6a-4d88-81b6-68e8e342f8c7";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsVariant = this.Scheme.Classifiers[D_VARIANT_PROPERTY_TAX_GUID];
            clsRegions = this.Scheme.Classifiers[D_REGIONS_PLAN_GUID];
            fctPropertyTax = this.Scheme.FactTables[F_F_PROPERTY_TAX_GUID];

            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { };
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daVariant, dsVariant, clsVariant);
            UpdateDataSet(daPropertyTax, dsPropertyTax, fctPropertyTax);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsPropertyTax);
            ClearDataSet(ref dsVariant);
            ClearDataSet(ref dsRegions);
        }

        #endregion Работа с базой и кэшами

        #region Работа с Xls

        private object CleanFactValue(string value, decimal multiplier)
        {
            if (value == string.Empty)
                return DBNull.Value;
            decimal factValue = 0;
            Decimal.TryParse(CommonRoutines.TrimLetters(value).Replace('.', ','), out factValue);
            return (factValue * multiplier);
        }

        private void PumpFactRow(string minRateTax, string maxRateTax, string noteMin, string noteMax,
            string addTax, int year, int refRegions, int refVariant, int refMarks)
        {
            if ((minRateTax == string.Empty) && (maxRateTax == string.Empty) && (addTax == string.Empty))
                return;

            decimal minTax = 0;
            Decimal.TryParse(minRateTax, out minTax);
            decimal maxTax = 0;
            Decimal.TryParse(maxRateTax, out maxTax);

            decimal rateTax = 0;
            if (minTax == 0)
                rateTax = maxTax;
            else if (maxTax == 0)
                rateTax = minTax;
            else
                rateTax = (minTax + maxTax) / 2;

            object[] mapping = new object[] {
                "MinRateTax", CleanFactValue(minRateTax, 1),
                "MaxRateTax", CleanFactValue(maxRateTax, 1),
                "RateTax", rateTax,
                "AddTax", CleanFactValue(addTax, 1000),
                "Note", noteMin,
                "NoteMax", noteMax,
                "RefYearDayUNV", year * 10000 + 1,
                "RefRegions", refRegions,
                "RefVariant", refVariant,
                "RefMarks", refMarks,
                "SourceID", sourceID
            };

            PumpRow(dsPropertyTax.Tables[0], mapping);
        }

        private void DeleteEarlierDataByRegions(int refRegions)
        {
            if (!deletedRegionsList.Contains(refRegions) && !this.DeleteEarlierData)
            {
                string constr = string.Format("RefRegions = {0}", refRegions);
                DirectDeleteFactData(new IFactTable[] { fctPropertyTax }, -1, sourceID, constr);
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

        private int PumpVariant()
        {
            string name = string.Format("Дата сбора {0} (данные за {1}-{0}гг.)",
                this.DataSource.Year + 1, this.DataSource.Year);

            if (cacheVariant.ContainsKey(name))
                return cacheVariant[name];

            object[] mapping = new object[] {
                "Name", name,
                "Code", ++maxVariantCode,
                "Year", this.DataSource.Year + 1,
                "VariantComment", name
            };

            return PumpCachedRow(cacheVariant, dsVariant.Tables[0], clsVariant, name, mapping);
        }

        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int year, int refVariant)
        {
            string regionValue = excelDoc.GetValue(curRow, 2).Trim();
            int refRegions = GetRefRegions(regionValue);
            if (regionValue.ToUpper().Contains("ГОРОДСКОЙ ОКРУГ"))
                nullFactRow["RefRegions"] = refRegions;

            for (int i = 0; i <= 2; i++)
            {
                int shift = i * 7;
                string minRateTax = excelDoc.GetValue(curRow + 1, shift + 4).Trim();
                string maxRateTax = excelDoc.GetValue(curRow, shift + 4).Trim();
                string noteMin = excelDoc.GetValue(curRow + 1, shift + 5).Trim();
                string noteMax = excelDoc.GetValue(curRow, shift + 5).Trim();
                string addTax = excelDoc.GetValue(curRow, shift + 9).Trim();
                PumpFactRow(minRateTax, maxRateTax, noteMin, noteMax, addTax, year, refRegions, refVariant, i);

                minRateTax = excelDoc.GetValue(curRow + 1, shift + 7).Trim();
                maxRateTax = excelDoc.GetValue(curRow, shift + 7).Trim();
                noteMin = excelDoc.GetValue(curRow + 1, shift + 8).Trim();
                noteMax = excelDoc.GetValue(curRow, shift + 8).Trim();
                PumpFactRow(minRateTax, maxRateTax, noteMin, noteMax, string.Empty, year + 1, refRegions, refVariant, i);
            }
        }

        private void GetSectionBorders(ExcelHelper excelDoc, ref int firstRow, ref int lastRow)
        {
            int curRow = 1;
            int emptyStrCount = 0;
            for (; emptyStrCount < 5; curRow++)
            {
                string cellValue = excelDoc.GetValue(curRow, 2).Trim();
                if (cellValue == string.Empty)
                {
                    emptyStrCount++;
                    continue;
                }
                emptyStrCount = 0;

                if (cellValue == "1")
                {
                    firstRow = curRow + 1;
                    continue;
                }
            }
            lastRow = curRow - emptyStrCount;
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

        private void PumpXlsSheet(ExcelHelper excelDoc, int year, int refVariant)
        {
            int firstRow = 0;
            int lastRow = 0;
            GetSectionBorders(excelDoc, ref firstRow, ref lastRow);
            for (int curRow = firstRow; curRow < lastRow; curRow++)
                try
                {
                    string cellValue = excelDoc.GetValue(curRow, 2).Trim();
                    if (SkipRow(cellValue))
                        continue;

                    PumpXlsRow(excelDoc, curRow, year, refVariant);
                }
                catch (Exception ex)
                {
                    throw new PumpDataFailedException(string.Format(
                        "При обработке листа '{0}' возникла ошибка ({1})",
                        excelDoc.GetWorksheetName(), ex.Message));
                }
        }

        private int GetYear(string filename)
        {
            // из названия файла nifГГ_ННН.xls берем ГГ - год
            return (2000 + Convert.ToInt32(CommonRoutines.TrimLetters(filename.Split('_')[0])));
        }

        private DataRow GetNullFactRow(int year, int refVariant)
        {
            DataRow row = dsPropertyTax.Tables[0].NewRow();

            row["MinRateTax"] = DBNull.Value;
            row["MaxRateTax"] = DBNull.Value;
            row["AddTax"] = DBNull.Value;
            row["Note"] = DBNull.Value;
            row["NoteMax"] = DBNull.Value;
            row["RateTax"] = DBNull.Value;
            row["RefYearDayUNV"] = year * 10000 + 1;
            row["RefMarks"] = -1;
            row["RefVariant"] = refVariant;
            row["PumpID"] = this.PumpID;
            row["SourceID"] = sourceID;

            return row;
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                int year = GetYear(file.Name);
                int refVariant = PumpVariant();
                nullFactRow = GetNullFactRow(year, refVariant);

                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.EnableEvents = false;
                excelDoc.OpenDocument(file.FullName);

                PumpXlsSheet(excelDoc, year, refVariant);

                if (dsPropertyTax.Tables[0].Rows.Count == 0)
                    dsPropertyTax.Tables[0].Rows.Add(nullFactRow);
                UpdateData();
            }
            catch (PumpDataFailedException ex)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                    "Ошибка при обработке файла '{0}': {1} Файл будет пропущен.", file.Name, ex.Message));
            }
            finally
            {
                ClearDataSet(daPropertyTax, ref dsPropertyTax);
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

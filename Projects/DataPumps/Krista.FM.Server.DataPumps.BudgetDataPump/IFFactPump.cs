using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.BudgetDataPump
{

    // факт источников финансирования
    public partial class BudgetDataPumpModule : BudgetPumpModuleBase
    {
        private IDbDataAdapter daIFFact;
        private DataSet dsIFFact;

        private void QueryBudgetIFFactData(string constr)
        {
            // Запрос данных
            dsBudgetFacts.Relations.Clear();
            if (dsBudgetFacts.Tables.Contains("Table"))
            {
                if (dsBudgetFacts.Tables["Table"] != null)
                {
                    dsBudgetFacts.Tables["Table"].Clear();
                }
            }
            string query = string.Empty;

            string otQueryString = string.Empty;
            if (MajorDBVersion >= 38)
                otQueryString = "cast(d.OperationType as varchar(10)) OperationType, ";

            if (MajorDBVersion >= 35)
            {
                if (tDetailTableName == "CorrectionDetail")
                {
                    query = string.Format(
                        "select cast(d.ID as varchar(10)) id, cast(d.Summa as double precision) CREDIT, " +
                        "cast(d.FACIALACC_CLS as varchar(10)) FACIALACC_CLS, cast(d.KVR as varchar(10)) KVR, " +
                        "cast(d.KESR as varchar(10)) KESR, " +
                        "cast(d.PROGINDEX as varchar(10)) PROGINDEX, cast(d.KVSR as varchar(10)) KVSR, " +
                        "cast(d.KCSR as varchar(10)) KCSR, cast(d.KFSR as varchar(10)) KFSR, " +
                        "cast(d.SUBKESR as varchar(10)) SUBKESR, cast(d.FACT as varchar(10)) FACT, " +
                        "cast(d.MEANSTYPE as varchar(10)) MEANSTYPE, cast(d.FINSOURCE as varchar(10)) FINSOURCE, " +
                        "cast(d.FINTYPE as varchar(10)) FINTYPE, " +
                        "cast(d.IFS as varchar(10)) IFS, " +
                        "cast(d.DIRECTIONCLS as varchar(10)) DIRECTIONCLS, " +
                        "cast(d.RECORDINDEX as varchar(10)) RECORDINDEX, " +
                        otQueryString +
                        "cast(d.BudgetRef as varchar(10)) BudgetRef, cast(d.RegionCLS as varchar(10)) RegionCLS, " +
                        "cast(d.FundsSource as varchar(10)) FundsSource, " +
                        "b.Name BudgetBudgetName, cast(b.AYear as varchar(10)) BudgetBudgetYear " +
                        "from CorrectionDetail d left join Budgets_s b on (d.BudgetRef = b.ID) {0} " +
                        " and (d.DocumentRecordIndex is null or d.DocumentRecordIndex = 0) ", constr);
                }
                else
                {
                    query = string.Format(
                        "select cast(d.ID as varchar(10)) id, cast(d.CREDIT as double precision) CREDIT, " +
                        "cast(d.SOURCEFACIALACC_CLS as varchar(10)) SOURCEFACIALACC_CLS, cast(d.SOURCEKVR as varchar(10)) SOURCEKVR, " +
                        "cast(d.SOURCEKESR as varchar(10)) SOURCEKESR, cast(d.DESTFACIALACC_CLS as varchar(10)) DESTFACIALACC_CLS, " +
                        "cast(d.DESTKVR as varchar(10)) DESTKVR, cast(d.DESTKESR as varchar(10)) DESTKESR, " +
                        "cast(d.PROGINDEX as varchar(10)) PROGINDEX, cast(d.SOURCEKVSR as varchar(10)) SOURCEKVSR, " +
                        "cast(d.SOURCEKCSR as varchar(10)) SOURCEKCSR, cast(d.SOURCEKFSR as varchar(10)) SOURCEKFSR, " +
                        "cast(d.SOURCESUBKESR as varchar(10)) SOURCESUBKESR, cast(d.SOURCEFACT as varchar(10)) SOURCEFACT, " +
                        "cast(d.SOURCEMEANSTYPE as varchar(10)) SOURCEMEANSTYPE, cast(d.SOURCEFINSOURCE as varchar(10)) SOURCEFINSOURCE, " +
                        "cast(d.SOURCEFINTYPE as varchar(10)) SOURCEFINTYPE, " +
                        "cast(d.DESTKVSR as varchar(10)) DESTKVSR, cast(d.DESTKCSR as varchar(10)) DESTKCSR, " +
                        "cast(d.DESTKFSR as varchar(10)) DESTKFSR, cast(d.DESTSUBKESR as varchar(10)) DESTSUBKESR, " +
                        "cast(d.DESTFACT as varchar(10)) DESTFACT, cast(d.DESTMEANSTYPE as varchar(10)) DESTMEANSTYPE, " +
                        "cast(d.DESTIFS as varchar(10)) DESTIFS, cast(d.SOURCEIFS as varchar(10)) SOURCEIFS, " +
                        "cast(d.DESTFINSOURCE as varchar(10)) DESTFINSOURCE, cast(d.DESTFINTYPE as varchar(10)) DESTFINTYPE, " +
                        "cast(d.SOURCEDIRECTIONCLS as varchar(10)) SOURCEDIRECTIONCLS, cast(d.DestFundsSource as varchar(10)) DestFundsSource, " +
                        "cast(d.DESTDIRECTIONCLS as varchar(10)) DESTDIRECTIONCLS, cast(d.RECORDINDEX as varchar(10)) RECORDINDEX, " +
                        "cast(d.BudgetRef as varchar(10)) BudgetRef, cast(d.SourceRegionCLS as varchar(10)) SourceRegionCLS, " +
                        "cast(d.DestRegionCLS as varchar(10)) DestRegionCLS, cast(d.SourceFundsSource as varchar(10)) SourceFundsSource, " +
                        "b.Name BudgetBudgetName, cast(b.AYear as varchar(10)) BudgetBudgetYear " +
                        "from FACIALFINDETAIL d left join Budgets_s b on (d.BudgetRef = b.ID) {0} ", constr);
                }
            }
            InitLocalDataAdapter(this.BudgetDB, ref daBudgetFacts, query);
            daBudgetFacts.Fill(dsBudgetFacts);
        }

        private string GetBudgetIFFactConstr()
        {
            string constrByPumpParams = string.Empty;

            if (tDetailTableName == "FacialFinDetail")
            {
                constrByPumpParams = GetDateConstrByPumpParams("d.ACCEPTDATE", true);
                if (constrByPumpParams == string.Empty)
                    constrByPumpParams = string.Format("(d.acceptdate between {0}0000 and {0}1232) ", this.DataSource.Year);
                else
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                        string.Format("Закачка выполняется с ограничением по дате: {0}..{1}", this.dateMin, this.dateMax));
                constrByPumpParams += " and ";
            }
            constrByPumpParams += " (d.clsType = '1') ";

            if (tCaptionTableName == "CorrectionCaption")
                constrByPumpParams += " and (d.DocumentRecordIndex is null or d.DocumentRecordIndex = 0) ";

            if (this.PumpMode == BudgetDataPumpMode.Update)
                constrByPumpParams += GetDateConstr("d.");
            constrByPumpParams = string.Concat(" where ", constrByPumpParams);
            return constrByPumpParams;
        }

        /// <summary>
        /// Запрос данных о количестве записей казначейства из базы бюджета (деталь)
        /// </summary>
        private int QueryBudgetIFFactCount()
        {
            string idConstr = "select count(d.id) from " + tDetailTableName + " d ";
            idConstr += GetBudgetIFFactConstr() + " and (d.clsType = '1') ";
            return Convert.ToInt32(this.BudgetDB.ExecQuery(idConstr, QueryResultTypes.Scalar));
        }

        /// <summary>
        /// Запрос данных о количестве записей казначейства из базы бюджета (мастер)
        /// </summary>
        private int QueryFacialFinCaptionIFFactCount()
        {
            treasuryConstr = string.Format(
                "where c.ACCEPTDATE is not null and c.REJECT_CLS is null and c.BUDGETREF = {0} {1}",
                this.BudgetRef, GetDateConstr("c."));
            CheckBudgetTableDate(tCaptionTableName, "c.", "ACCEPTDATE", treasuryConstr);
            treasuryConstr += string.Format(" and (c.ACCEPTDATE between {0}0000 and {0}1232) ", this.DataSource.Year);
            if (isTreasury)
                treasuryConstr += " and not((c.ProgIndex = 63) and (c.OperationDirection = 3)) ";

            FillCaptionAcceptedRecords(treasuryConstr);
            return captionAcceptedRecords.Count;
        }

        /// <summary>
        /// Запрос наших данных
        /// </summary>
        private void QueryFMIFFact()
        {
            ClearDataSet(ref dsIFFact);
            if (this.PumpMode == BudgetDataPumpMode.Update)
            {
                InitDataSet(ref daIFFact, ref dsIFFact, fctIFFact, false, string.Format("SOURCEID = {0}", this.SourceID), string.Empty);
                FillRowsCache(ref factCache, dsIFFact.Tables[0], new string[] { "SOURCEKEY" });
            }
            else
            {
                InitFactDataSet(ref daIFFact, ref dsIFFact, fctIFFact);
            }
            ClearDataSet(ref dsProgIndexFacial);
            InitDataSet(ref daProgIndexFacial, ref dsProgIndexFacial, fxcProgIndexFacial, true, string.Empty, string.Empty);
        }

        /// <summary>
        /// Сбрасывает закачанные данные в базу
        /// </summary>
        private void UpdateFMIFFact()
        {
            try
            {
                UpdateData();
                UpdateDataSet(daIFFact, dsIFFact, fctIFFact);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Устанавливает поля классификаторов строки
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="recordType">Тип записи</param>
        private void SetClsFieldsIFFact(DataRow factRow, DataRow treasuryRow, int recordType)
        {
            string clsPrefix = GetClsPrefix(recordType);

            SetOrgFields(treasuryRow, factRow, string.Format("{0}FACIALACC_CLS", clsPrefix));

            // Тип средств 
            factRow["REFMEANSTYPE"] = FindCachedRowID(meansTypeCache, Convert.ToInt32(treasuryRow[string.Format("{0}MEANSTYPE", clsPrefix)]), nullMeansType);
            // ТипФинансир 
            factRow["REFFINTYPE"] = FindCachedRowID(finTypeCache, Convert.ToInt32(treasuryRow[string.Format("{0}FINTYPE", clsPrefix)]), nullFinType);

            if (MajorDBVersion >= 35)
            {
                factRow["RefFundsSource"] = FindCachedRowID(fundsSourceCache,
                    Convert.ToInt32(treasuryRow[string.Format("{0}FundsSource", clsPrefix)]), nullFundsSource);
                factRow["RefBudget"] = FindBudgetBudgetRef(treasuryRow["BudgetBudgetName"].ToString(),
                    treasuryRow["BudgetBudgetYear"].ToString());
                string fieldName = string.Format("{0}RegionCLS", clsPrefix);
                factRow["RefRegions"] = FindCachedRowID(regionsCache, Convert.ToInt32(treasuryRow[fieldName]), nullRegions);
            }

            factRow["REFKIF"] = FindCachedRowID(ifsCache, Convert.ToInt32(treasuryRow[string.Format("{0}IFS", clsPrefix)]), nullKIF2005);
        }

        /// <summary>
        /// Добавляет или апдейтит строку таблицы казначейства
        /// </summary>
        /// <param name="treasuryRow">Строка таблицы бюджета</param>
        /// <param name="recordType">Тип записи</param>
        /// <param name="sum">Сумма</param>
        /// <param name="updatedRowsCount">Счетчик обновленных записей</param>
        /// <param name="addedRowsCount">Счетчик добавленных записей</param>
        private void PumpIFFactRow(DataRow treasuryRow, int acceptDate, int recordType,
            ref int updatedRowsCount, ref int addedRowsCount, DataRow fmRow)
        {
            double sum = GetDoubleCellValue(treasuryRow, "CREDIT", 0);
            if (sum == 0)
                return;

            // не качаем временный тип средств
            if (IsTempMeansType(treasuryRow, recordType))
                return;

            if (tDetailTableName == "CorrectionDetail")
            {
                if (treasuryRow["ProgIndex"].ToString() == "315")
                    sum *= -1;
            }

            // Если работаем в режиме обновления, то сначала поищем такую запись
            if (this.PumpMode == BudgetDataPumpMode.Update)
            {
                fmRow = FindCachedRow(factCache, new string[] { Convert.ToString(treasuryRow["ID"]) });
                if (fmRow != null)
                    updatedRowsCount++;
            }

            if (fmRow == null)
            {
                if (dsIFFact.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT * 2)
                {
                    UpdateFMIFFact();
                    ClearDataSet(daIFFact, ref dsFacialFinDetail);
                }
                // закачивается новая строка
                fmRow = dsIFFact.Tables[0].NewRow();
                fmRow.BeginEdit();
                fmRow["SOURCEKEY"] = treasuryRow["ID"];
                fmRow["RECORDTYPE"] = recordType;
                dsIFFact.Tables[0].Rows.Add(fmRow);
                addedRowsCount++;
            }
            else
                // существующая строка обновляется
                updatedRowsCount++;

            fmRow["PUMPID"] = this.PumpID;
            fmRow["SOURCEID"] = this.SourceID;
            fmRow["REFPROGINDEXFACIAL"] = FindRowID(dsProgIndexFacial.Tables[0],
                string.Format("CODESTR = '{0}'", treasuryRow["PROGINDEX"]), -1);
            fmRow["RefYearDayUNV"] = acceptDate;

            SetSumFields(fmRow, sum, recordType);

            SetClsFieldsIFFact(fmRow, treasuryRow, recordType);
            fmRow.EndEdit();
        }

        private void PumpIFFactRows(ref int addedRowsCount, ref int updatedRowsCount,
            DataRow treasuryRow, FacialFinCaptionRow captionRow)
        {
            int recType;
            if (tCaptionTableName == "CorrectionCaption")
            {
                if (MajorDBVersion >= 38)
                {
                    // записи должны удовлетворять условиям
                    int captionPI = captionRow.ProgIndex;
                    int detailPI = Convert.ToInt32(treasuryRow["ProgIndex"]);
                    int detailOT = Convert.ToInt32(treasuryRow["OperationType"]);
                    if (!((captionPI == 313 && detailPI == 316 && detailOT == 1) ||
                          (captionPI == 313 && detailPI == 315 && detailOT == -1) ||
                          (captionPI == 312 && detailPI == 316 && detailOT == -1) ||
                          (captionPI == 312 && detailPI == 315 && detailOT == 1)))
                        return;
                }
                // для уведомлений тип записи определяем по прогиндексу
                recType = GetTreasuryRecordType(captionRow.ProgIndex);
                PumpIFFactRow(treasuryRow, captionRow.AcceptDate, recType, ref updatedRowsCount, ref addedRowsCount, null);
            }
            else
            {
                // для казначейства типа записи определяется по дест или сурс тайпу
                if (captionRow.SourceType == 1)
                    PumpIFFactRow(treasuryRow, captionRow.AcceptDate, 1, ref updatedRowsCount, ref addedRowsCount, null);
                if (captionRow.DestType == -1)
                    PumpIFFactRow(treasuryRow, captionRow.AcceptDate, 2, ref updatedRowsCount, ref addedRowsCount, null);
                if (captionRow.DestType == 1)
                    PumpIFFactRow(treasuryRow, captionRow.AcceptDate, 3, ref updatedRowsCount, ref addedRowsCount, null);
                if (captionRow.SourceType == -1)
                    PumpIFFactRow(treasuryRow, captionRow.AcceptDate, 4, ref updatedRowsCount, ref addedRowsCount, null);
            }
        }

        private void PumpIFFact(ref int addedRowsCount, ref int updatedRowsCount, ref int processedRecCount,
            ref string skippedRecsMessage)
        {
            int zeroDateCount = 0;
            DataRow treasuryRow;
            int selectedRecCount = 0;

            try
            {
                int totalRecs = QueryBudgetIFFactCount();
                if (totalRecs == 0)
                    return;

                string budgetTreasuryConstr = GetBudgetIFFactConstr() + " and (d.clsType = '1') ";
                WriteToTrace(string.Format("Ограничение на {0}: {1}", tDetailTableName, budgetTreasuryConstr), TraceMessageKind.Information);
                do
                {
                    QueryBudgetIFFactData(budgetTreasuryConstr);
                    selectedRecCount = dsBudgetFacts.Tables["Table"].Rows.Count;
                    if (selectedRecCount == 0)
                        continue;

                    // Обработка полученных данных
                    for (int i = 0; i < selectedRecCount; i++)
                    {
                        processedRecCount++;
                        treasuryRow = dsBudgetFacts.Tables["Table"].Rows[i];

                        int recordIndex = Convert.ToInt32(treasuryRow["RECORDINDEX"]);
                        if (!captionAcceptedRecords.ContainsKey(recordIndex))
                            continue;

                        FacialFinCaptionRow captionRow = captionAcceptedRecords[recordIndex];

                        // Проверка даты
                        int date = Convert.ToInt32(captionRow.AcceptDate);
                        if (!CheckDate(date))
                            continue;
                        if (captionRow.AcceptDate % 10000 == 0)
                        {
                            zeroDateCount++;
                            continue;
                        }
                        try
                        {
                            PumpIFFactRows(ref addedRowsCount, ref updatedRowsCount, treasuryRow, captionRow);
                            SetProgress(totalRecs, processedRecCount, string.Format(
                                    "Обработка данных базы {0} ({1} из {2})...",
                                    this.DatabasePath, filesCount, totalFiles),
                                string.Format("{0}. Запись {1} из {2}", currentBlockName, processedRecCount, totalRecs));
                        }
                        catch (Exception ex)
                        {
                            WriteToTrace(string.Format("СТРОКА {0} - {1}", treasuryRow["ID"], ex), TraceMessageKind.Error);
                            throw new PumpDataFailedException(string.Format("СТРОКА {0} - {1}", treasuryRow["ID"], ex.Message));
                        }
                    }

                    if (this.PumpMode != BudgetDataPumpMode.Update)
                    {
                        UpdateFMIFFact();
                        ClearDataSet(ref dsBudgetFacts);
                        ClearDataSet(ref dsIFFact);
                        daIFFact.Fill(dsIFFact);
                    }
                    GC.GetTotalMemory(true);
                    WriteToTrace(String.Format("Обработано {0} записей из {1}", processedRecCount, totalRecs),
                        TraceMessageKind.Information);
                }
                while (processedRecCount < totalRecs);

                UpdateFMIFFact();
            }
            finally
            {
                ClearDataSet(ref dsIFFact);
                ClearDataSet(ref dsProgIndexFacial);
                captionAcceptedRecords.Clear();
            }
        }
    }
}
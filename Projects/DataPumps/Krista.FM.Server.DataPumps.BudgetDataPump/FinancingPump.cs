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
    // Закачка Финансирование

    public partial class BudgetDataPumpModule : BudgetPumpModuleBase
    {
        private IDbDataAdapter daFinancing;
        private DataSet dsFinancing;
        private IDbDataAdapter daProgIndexFinDoc;
        private DataSet dsProgIndexFinDoc;


        /// <summary>
        /// Запрос данных из базы бюджета
        /// </summary>
        private int QueryBudgetFinancingData()
        {
            // Запрос данных
            dsBudgetFacts.Relations.Clear();

            string financingConstr = string.Format(
                "where f.ACCEPTDATE is not null and f.BUDGETREF = {0} {1}",
                this.BudgetRef, GetDateConstr("f."));
            CheckBudgetTableDate("FinDocDetail", "f.", "ACCEPTDATE", financingConstr);

            string constrByPumpParams = GetDateConstrByPumpParams("f.ACCEPTDATE", true);
            if (constrByPumpParams == string.Empty)
                financingConstr += " and (f.ACCEPTDATE between 20000101 and 20201217)";
            else
            {
                financingConstr += " and " + constrByPumpParams;
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                    string.Format("Закачка выполняется с ограничением по дате: {0}..{1}", this.dateMin, this.dateMax));
            }

            string query = string.Format(
                "select cast(f.ID as varchar(10)) id, cast(f.ACCEPTDATE as varchar(10)) ACCEPTDATE, " +
                "cast(f.PROGINDEX as varchar(10)) PROGINDEX, cast(f.SUMMA as double precision) SUMMA, " +
                "cast(f.CREDIT as double precision) CREDIT, cast(f.DEBIT as double precision) DEBIT, " +
                "cast(f.KFSR as varchar(10)) KFSR, cast(f.KCSR as varchar(10)) KCSR, cast(f.KESR as varchar(10)) KESR, " +
                "cast(f.KVR as varchar(10)) KVR, cast(f.KVSR as varchar(10)) KVSR, cast(f.SUBKESR as varchar(10)) SUBKESR, " +
                "cast(f.FACT as varchar(10)) FACT, cast(f.FINSOURCE as varchar(10)) FINSOURCE, cast(f.REGIONCLS as varchar(10)) REGIONCLS, " +
                "cast(f.DOCTYPE as varchar(10)) DOCTYPE, cast(f.FINTYPE as varchar(10)) FINTYPE, cast(f.MEANSTYPE as varchar(10)) MEANSTYPE, " +
                "cast(f.FACIALACC_CLS as varchar(10)) FACIALACC_CLS, cast(f.ACCOUNTREF as varchar(10)) ACCOUNTREF, " +
                "cast(f.DIRECTIONCLS as varchar(10)) DIRECTIONCLS, cast(f.IFS as varchar(10)) IFS, cast(p.ID as varchar(10)) PID, " +
                "cast(p.ACCEPTDATE as varchar(10)) PAD, cast(p.REJECT_CLS as varchar(10)) PRC " +
                "from FinDocDetail f left join PayDoc32 p on (p.RecordIndex = f.ID) {0}", financingConstr);
            WriteToTrace("Запрос Финансирование: " + query, TraceMessageKind.Information);
            InitLocalDataAdapter(this.BudgetDB, ref daBudgetFacts, query);
            daBudgetFacts.Fill(dsBudgetFacts);

            return dsBudgetFacts.Tables["Table"].Rows.Count;
        }

        /// <summary>
        /// Запрос наших данных
        /// </summary>
        private void QueryFMFinancingData()
        {
            ClearDataSet(ref dsFinancing);
            ClearDataSet(ref dsProgIndexFinDoc);

            if (this.PumpMode == BudgetDataPumpMode.Update)
            {
                InitDataSet(ref daFinancing, ref dsFinancing, fctFinDocDetail, false,
                    string.Format("SOURCEID = {0}", this.SourceID), string.Empty);

                FillRowsCache(ref factCache, dsFinancing.Tables[0], new string[] { "SOURCEKEY" });
            }
            else
            {
                InitFactDataSet(ref daFinancing, ref dsFinancing, fctFinDocDetail);
            }

            InitDataSet(ref daProgIndexFinDoc, ref dsProgIndexFinDoc, fxcProgIndexFinDoc, 
                true, string.Empty, string.Empty);
        }

        /// <summary>
        /// Сбрасывает закачанные данные в базу
        /// </summary>
        private void UpdateFMFinancingData()
        {
            try
            {
                UpdateData();

                UpdateDataSet(daFinancing, dsFinancing, fctFinDocDetail);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Закачивает строку фактов
        /// </summary>
        /// <param name="FinancingRow">Строка фактов бюджета</param>
        /// <param name="factRow">Строка фактов ФМ</param>
        /// <param name="addedRowsCount">Счетчик добавленных записей</param>
        /// <param name="updatedRowsCount">Счетчик обновленных записей</param>
        private void PumpFinancingRow(DataRow financingRow, DataRow factRow, ref int addedRowsCount,
            ref int updatedRowsCount)
        {
            factRow["SOURCEKEY"] = financingRow["ID"];
            factRow["SUMMA"] = financingRow["SUMMA"];
            factRow["CREDIT"] = financingRow["CREDIT"];
            factRow["DEBIT"] = financingRow["DEBIT"];

            int date = Convert.ToInt32(financingRow["ACCEPTDATE"].ToString().Split('.')[0]);
            factRow["RefYearDayUNV"] = date;

            factRow["PROGINDEX"] = financingRow["PROGINDEX"];
            factRow["REFPROGINDEXFINDOC"] = FindRowID(dsProgIndexFinDoc.Tables[0],
                string.Format("CODESTR = '{0}'", financingRow["PROGINDEX"]), -1);

            if (GetIntCellValue(financingRow, "FACIALACC_CLS", 0) != 0)
            {
                SetOrgFields(financingRow, factRow, "FACIALACC_CLS");
            }
            else
            {
                DataRow facialAcc = FindCachedRow(facialAccCache, Convert.ToInt32(financingRow["FACIALACC_CLS"]));

                if (facialAcc != null)
                {
                    // Лицевые счета
                    factRow["REFFACIALACC"] = facialAcc["ID"];
                }
                else
                {
                    // Лицевые счета
                    factRow["REFFACIALACC"] = nullFacialAcc;
                }

            }

            // Вид расходов
            factRow["REFKVR"] = FindCachedRowID(kvrCache,
                Convert.ToInt32(financingRow["KVR"]), nullKVR);

            // Администратор 
            factRow["REFKVSR"] = FindCachedRowID(kvsrCache, Convert.ToInt32(financingRow["KVSR"]), nullKVSR);

            // Мероприятие 
            factRow["REFFACT"] = FindCachedRowID(factClsCache, Convert.ToInt32(financingRow["FACT"]), nullFact);

            // Источник финансирования 
            factRow["REFNOTIFYTYPES"] = FindRowID(dsNotifyTypes.Tables[0],
                new object[] { "SOURCEKEY", financingRow["FINSOURCE"] }, nullNotifyTypes);

            // ТипФинансир 
            factRow["REFFINTYPE"] = FindCachedRowID(finTypeCache, Convert.ToInt32(financingRow["FINTYPE"]), nullFinType);

            // Район 
            factRow["REFREGIONS"] = FindRowID(dsRegions.Tables[0],
                new object[] { "SOURCEKEY", financingRow["REGIONCLS"] }, nullRegions);

            // Тип средств
            factRow["REFMEANSTYPE"] = FindCachedRowID(meansTypeCache, Convert.ToInt32(financingRow["MEANSTYPE"]), nullMeansType);

            // ФКР
            factRow["REFFKR"] = FindCachedRowID(fkrCache, Convert.ToInt32(financingRow["KFSR"]), nullFKR);
            // ЭКР
            factRow["REFEKR"] = FindCachedRowID(ekrCache, Convert.ToInt32(financingRow["KESR"]), nullEKR);
            // КЦСР
            factRow["REFKCSR"] = FindCachedRowID(kcsrCache, Convert.ToInt32(financingRow["KCSR"]), nullKCSR);
            // СубЭКР 
            factRow["REFSUBKESR"] = FindCachedRowID(subEkrCache, Convert.ToInt32(financingRow["SUBKESR"]), nullSubKESR);

            switch (this.CurrentDBVersion)
            {
                case "27.02":
                case "28.00":
                case "29.01":
                case "29.02":
                case "30.00":
                case "30.01":
                    // Источник финансирования
                    factRow["REFNOTIFYTYPES"] = nullNotifyTypes;

                    // Направление 
                    factRow["REFDIRECTION"] = nullDirection;
                    break;

                default:
                    // Источник финансирования
                    factRow["REFNOTIFYTYPES"] = FindRowID(dsNotifyTypes.Tables[0],
                        new object[] { "SOURCEKEY", financingRow["FINSOURCE"] }, nullNotifyTypes);

                    // Направление 
                    factRow["REFDIRECTION"] = FindCachedRowID(directionCache, Convert.ToInt32(financingRow["DIRECTIONCLS"]), nullDirection);
                    break;
            }

            // Источники внутреннего финансирования
            switch (this.CurrentDBVersion)
            {
                case "27.02":
                case "28.00":
                case "29.01":
                case "29.02":
                case "30.00":
                case "30.01":
                case "31.00":
                case "31.01":
                    factRow["REFKIF"] = nullKIF2005;
                    break;

                default:
                    factRow["REFKIF"] = FindCachedRowID(ifsCache, 
                        Convert.ToInt32(financingRow["IFS"]), nullKIF2005);
                    break;
            }
        }

        /// <summary>
        /// Закачивает данные из бюджетной базы в нашу
        /// </summary>
        private void PumpFinancing(ref int addedRowsCount, ref int updatedRowsCount, ref int processedRecCount, 
            ref string skippedRecsMessage)
        {
            DataRow row;
            bool isAdded;
            int totalRecs = 0;
            DataRow financingRow = null;
            int rejectPayDocCount = 0;


            try
            {
                totalRecs = dsBudgetFacts.Tables["Table"].Rows.Count;

                // Обработка полученных данных
                ClassTypes objClassType = fctFinDocDetail.ClassType;
                string generatorName = fctFinDocDetail.GeneratorName;
                for (int i = 0; i < totalRecs; i++)
                {
                    processedRecCount++;

                    financingRow = dsBudgetFacts.Tables["Table"].Rows[i];

                    int docType = Convert.ToInt32(financingRow["DOCTYPE"]);
                    if (docType >= 1010 && docType <= 1019 && !(!financingRow.IsNull("PID") &&
                        !financingRow.IsNull("PAD") && financingRow.IsNull("PRC")))
                    {
                        rejectPayDocCount++;
                        continue;
                    }

                    // Проверка даты
                    int date = Convert.ToInt32(financingRow["ACCEPTDATE"].ToString().Split('.')[0]);
                    if (!CheckDate(date))
                    {
                        continue;
                    }

                    try
                    {
                        row = GetRowForUpdate(Convert.ToInt32(financingRow["ID"]),
                            dsFinancing.Tables[0], factCache, objClassType, generatorName, out isAdded);
                        if (isAdded)
                        {
                            addedRowsCount++;
                        }
                        else
                        {
                            updatedRowsCount++;
                        }

                        PumpFinancingRow(financingRow, row, ref addedRowsCount, ref updatedRowsCount);

                        SetProgress(totalRecs, i + 1, string.Format(
                                "Обработка данных базы {0} ({1} из {2})...",
                                this.DatabasePath, filesCount, totalFiles),
                            string.Format("{0}. Запись {1} из {2}", currentBlockName, i + 1, totalRecs));

                        // Если накопилось много записей, то сбрасываем в базу
                        if (processedRecCount >= MAX_DS_RECORDS_AMOUNT)
                        {
                            UpdateFMFinancingData();
                            if (this.PumpMode != BudgetDataPumpMode.Update)
                            {
                                ClearDataSet(ref dsFinancing);
                                daFinancing.Fill(dsFinancing);
                            }
                            processedRecCount = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToTrace(string.Format("СТРОКА {0} - {1}", financingRow["ID"], ex), TraceMessageKind.Error);
                        throw new PumpDataFailedException(string.Format("СТРОКА {0} - {1}", financingRow["ID"], ex.Message));
                    }
                }

                // Сохранение данных
                UpdateFMFinancingData();
            }
            finally
            {
                skippedRecsMessage = string.Format(", с отклоненным платежным поручением: {0}.", rejectPayDocCount);

                ClearDataSet(ref dsFinancing);
                ClearDataSet(ref dsProgIndexFinDoc);
            }
        }
    }
}
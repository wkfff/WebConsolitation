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
    // Закачка Операции со счетами

    public partial class BudgetDataPumpModule : BudgetPumpModuleBase
    {
        private IDbDataAdapter daAccountOperations;
        private DataSet dsAccountOperations;


        /// <summary>
        /// Запрос данных из базы бюджета
        /// </summary>
        private int QueryBudgetAccountOperationsData()
        {
            // Запрос данных
            dsBudgetFacts.Relations.Clear();

            string accountOperationsConstr = string.Format(
                "where ACCEPTDATE is not null and BUDGETREF = {0}", this.BudgetRef);
            CheckBudgetTableDate("DFACCOUNTOPERATION", string.Empty, "ACCEPTDATE", accountOperationsConstr);

            string constrByPumpParams = GetDateConstrByPumpParams("ACCEPTDATE", true);
            if (constrByPumpParams == string.Empty)
                accountOperationsConstr += " and (acceptdate between 20000101 and 20201217)";
            else
            {
                accountOperationsConstr += " and " + constrByPumpParams;
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                    string.Format("Закачка выполняется с ограничением по дате: {0}..{1}", this.dateMin, this.dateMax));
            }

            string query = string.Empty;
            if (MajorDBVersion >= 35)
            {
                query = string.Format(
                    "select cast(ID as varchar(10)) id, cast(SDFACCOUNT as varchar(10)) SDFACCOUNT, " +
                    "cast(DDFACCOUNT as varchar(10)) DDFACCOUNT, cast(ACCEPTDATE as varchar(10)) ACCEPTDATE, " +
                    "cast(SUMMA as double precision) SUMMA, cast(REGIONCLS as varchar(10)) REGIONCLS, NOTE, " +
                    "cast(PROGINDEX as varchar(10)) progIndex, cast(RECORDINDEX as varchar(10)) recordIndex, " +
                    "cast(MEANSTYPE as varchar(10)) meansType, cast(FundsSource as varchar(10)) fundsSource " +
                    "from DFACCOUNTOPERATION {0}", accountOperationsConstr);
            }
            WriteToTrace("Запрос Операции со счетами: " + query, TraceMessageKind.Information);
            InitLocalDataAdapter(this.BudgetDB, ref daBudgetFacts, query);
            daBudgetFacts.Fill(dsBudgetFacts);

            return dsBudgetFacts.Tables["Table"].Rows.Count;
        }

        /// <summary>
        /// Запрос наших данных
        /// </summary>
        private void QueryFMAccountOperationsData()
        {
            ClearDataSet(ref dsAccountOperations);

            if (this.PumpMode == BudgetDataPumpMode.Update)
            {
                InitDataSet(ref daAccountOperations, ref dsAccountOperations, fctAccountOperations, false,
                    string.Format("SOURCEID = {0}", this.SourceID), string.Empty);

                FillRowsCache(ref factCache, dsAccountOperations.Tables[0], new string[] { "SOURCEKEY" });
            }
            else
            {
                InitFactDataSet(ref daAccountOperations, ref dsAccountOperations, fctAccountOperations);
            }
        }

        /// <summary>
        /// Сбрасывает закачанные данные в базу
        /// </summary>
        private void UpdateFMAccountOperationsData()
        {
            try
            {
                UpdateData();

                UpdateDataSet(daAccountOperations, dsAccountOperations, fctAccountOperations);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Закачивает строку фактов
        /// </summary>
        /// <param name="accountOperationsRow">Строка фактов бюджета</param>
        /// <param name="factRow">Строка фактов ФМ</param>
        /// <param name="addedRowsCount">Счетчик добавленных записей</param>
        /// <param name="updatedRowsCount">Счетчик обновленных записей</param>
        private void PumpAccountOperationsRow(DataRow accountOperationsRow, ref int addedRowsCount, 
            ref int updatedRowsCount)
        {
            DataRow[] factRow = null;

            // Если работаем в режиме обновления, то сначала поищем такую запись
            if (this.PumpMode == BudgetDataPumpMode.Update)
            {
                DataRow[] rows = dsAccountOperations.Tables[0].Select(string.Format(
                    "SOURCEKEY = {0} and SOURCEID = {1}", accountOperationsRow["ID"], this.SourceID));
                if (rows.GetLength(0) > 0)
                {
                    factRow = rows;
                    updatedRowsCount++;
                }
            }

            if (factRow == null)
            {
                // По одной исходной записи формируем две в нашей таблице фактов.
                factRow = new DataRow[2];
                for (int i = 0; i < factRow.GetLength(0); i++)
                {
                    factRow[i] = dsAccountOperations.Tables[0].NewRow();
                    factRow[i]["SOURCEKEY"] = accountOperationsRow["ID"];
                    dsAccountOperations.Tables[0].Rows.Add(factRow[i]);
                }
                addedRowsCount++;
            }

            int count = factRow.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                factRow[i]["PUMPID"] = this.PumpID;
                factRow[i]["SOURCEID"] = this.SourceID;
                factRow[i]["RefYearDayUNV"] = accountOperationsRow["ACCEPTDATE"].ToString().Split('.')[0];
                factRow[i]["NOTE"] = GetStringCellValue(accountOperationsRow, "NOTE", "Неуказанное направление");
                factRow[i]["PROGINDEX"] = accountOperationsRow["PROGINDEX"];
                factRow[i]["RECORDINDEX"] = GetIntCellValue(accountOperationsRow, "RECORDINDEX", -1);

                switch (i)
                {
                    // Запись 1: SDFAccount как "Расчетный счет", DDFAccount как "Корреспонтидующий расчетный счет"
                    // Сумму записываем в поле "Кредит".
                    case 0:
                        factRow[i]["CREDIT"] = accountOperationsRow["SUMMA"];
                        factRow[i]["DEBIT"] = 0;
                        break;

                    // Запись 2: DDFAccount как "Расчетный счет", SDFAccount как "Корреспонтидующий расчетный счет"
                    // Сумму записываем в поле "Дебет".
                    case 1:
                        factRow[i]["CREDIT"] = 0;
                        factRow[i]["DEBIT"] = accountOperationsRow["SUMMA"];
                        break;
                }

                // Районы
                factRow[i]["REFREGIONS"] = FindRowID(dsRegions.Tables[0], string.Format(
                    "SOURCEKEY = {0}", Convert.ToString(accountOperationsRow["REGIONCLS"])), nullRegions);

                // Тип средств
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
                        factRow[i]["REFMEANSTYPE"] = nullMeansType;
                        break;
                    default:
                        factRow[i]["REFMEANSTYPE"] = FindCachedRowID(meansTypeCache, Convert.ToInt32(accountOperationsRow["MEANSTYPE"]), nullMeansType);
                        if (MajorDBVersion >= 35)
                            factRow[i]["RefFundsSource"] = FindCachedRowID(fundsSourceCache, Convert.ToInt32(accountOperationsRow["FundsSource"]), nullFundsSource);
                        break;
                }
            }
        }

        /// <summary>
        /// Закачивает данные из бюджетной базы в нашу
        /// </summary>
        private void PumpAccountOperations(ref int addedRowsCount, ref int updatedRowsCount, ref int processedRecCount,
            ref string skippedRecsMessage)
        {
            DataRow accountOperationsRow = null;


            try
            {
                int totalRecs = dsBudgetFacts.Tables["Table"].Rows.Count;

                // Обработка полученных данных
                for (int i = 0; i < totalRecs; i++)
                {
                    processedRecCount++;

                    accountOperationsRow = dsBudgetFacts.Tables["Table"].Rows[i];

                    // Проверка даты
                    int date = Convert.ToInt32(accountOperationsRow["ACCEPTDATE"].ToString().Split('.')[0]);
                    if (!CheckDate(date))
                    {
                        continue;
                    }

                    try
                    {
                        PumpAccountOperationsRow(accountOperationsRow, ref addedRowsCount, ref updatedRowsCount);

                        SetProgress(totalRecs, i + 1, string.Format(
                                "Обработка данных базы {0} ({1} из {2})...",
                                this.DatabasePath, filesCount, totalFiles),
                            string.Format("{0}. Запись {1} из {2}", currentBlockName, i + 1, totalRecs));

                        // Если накопилось много записей, то сбрасываем в базу
                        if (processedRecCount >= MAX_DS_RECORDS_AMOUNT)
                        {
                            UpdateFMAccountOperationsData();
                            if (this.PumpMode != BudgetDataPumpMode.Update)
                            {
                                ClearDataSet(ref dsAccountOperations);
                                daAccountOperations.Fill(dsAccountOperations);
                            }
                            processedRecCount = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToTrace(string.Format("СТРОКА {0} - {1}", accountOperationsRow["ID"], ex), TraceMessageKind.Error);
                        throw new PumpDataFailedException(string.Format("СТРОКА {0} - {1}", accountOperationsRow["ID"], ex.Message));
                    }
                }

                // Сохранение данных
                UpdateFMAccountOperationsData();
            }
            finally
            {
                ClearDataSet(ref dsAccountOperations);
            }
        }
    }
}
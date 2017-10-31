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
    // Закачка данных плана расходов бюджета

    public partial class BudgetDataPumpModule : BudgetPumpModuleBase
    {
        private IDbDataAdapter daOutcomesPlan;
        private DataSet dsOutcomesPlan = new DataSet();

        private IDbDataAdapter daProgIndexBudget;
        private DataSet dsProgIndexBudget;

        /// <summary>
        /// Запрашивает данные OutcomesData
        /// </summary>
        /// <param name="constr">Ограничение выборки</param>
        /// <param name="typeFieldName">Наименование столбца типа в IncomesNotif</param>
        private void QueryOutcomesDataTable()
        {
            string queryStr = string.Empty;
            string typeFieldName = "TYPE";
            if (MajorDBVersion >= 35)
                typeFieldName = "aType";
            else
                switch (GetDbmsName())
                {
                    case BudgetDBMSName.Interbase: typeFieldName = "TYPE";
                        break;

                    case BudgetDBMSName.Oracle: typeFieldName = "ATYPE";
                        break;
                }
            string constr = string.Format(
                "where n.dat is not null and d.budgetref = {0} and n.RejectCls is null {1} {2} {3}",
                this.BudgetRef, GetOutcomesPlanConstr(typeFieldName), GetDateConstr("d."), " and (d.clsType = '0')");
            CheckBudgetTableDate("BudgetData d left join BudNotify n on (d.recordindex = n.ID) ", string.Empty, "DAT", constr);

            string constrByPumpParams = GetDateConstrByPumpParams("n.DAT", true);
            if (constrByPumpParams == string.Empty)
                constr += " and (n.DAT between 20000101 and 20201217)";
            else
            {
                constr += " and " + constrByPumpParams;
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                    string.Format("Закачка выполняется с ограничением по дате: {0}..{1}", this.dateMin, this.dateMax));
            }

            string qsTransfert = string.Empty;
            string qsTransfertJoin = string.Empty;
            if (MajorDBVersion >= 38)
            {
                qsTransfert = " cast(tr.code as varchar(10)) TransfertCLS, ";
                qsTransfertJoin = " left join transfertcls tr on (d.transfertcls = tr.ID) ";
            }

            if (MajorDBVersion >= 37)
            {
                // у bud_notify добавляется поле BA_LBO_Flag
                queryStr = string.Format(
                    "select cast(d.ID as varchar(10)) id, cast(d.BUDGETREF as varchar(10)) budgetRef, cast(d.KFSR as varchar(10)) KFSR, " +
                    "cast(d.KVSR as varchar(10)) kvsr, cast(d.KCSR as varchar(10)) kcsr, cast(d.KVR as varchar(10)) kvr, " +
                    "cast(d.KESR as varchar(10)) KESR, cast(d.SUBKESR as varchar(10)) SUBKESR, " +
                    "cast(d.REGIONCLS as varchar(10)) REGIONCLS, cast(d.AssignmentKind as varchar(10)) AssignmentKind, " +
                    "cast(d.BudgetRef as varchar(10)) budgetRef, cast(d.FACT as varchar(10)) fact, cast(d.DirectionCls as varchar(10)) DirectionCls, " +
                    "cast(d.FACIALACC_CLS as varchar(10)) FACIALACC_CLS, cast(d.MONTH01 as double precision) MONTH01, " +
                    "cast(d.MONTH02 as double precision) MONTH02, cast(d.MONTH03 as double precision) MONTH03, " +
                    "cast(d.MONTH04 as double precision) MONTH04, cast(d.MONTH05 as double precision) MONTH05, " +
                    "cast(d.MONTH06 as double precision) MONTH06, cast(d.MONTH07 as double precision) MONTH07, " +
                    "cast(d.MONTH08 as double precision) MONTH08, cast(d.MONTH09 as double precision) MONTH09, " +
                    "cast(d.MONTH10 as double precision) MONTH10, cast(d.MONTH11 as double precision) MONTH11, " +
                    "cast(d.MONTH12 as double precision) MONTH12, cast(d.QUARTER1 as double precision) QUARTER1, " +
                    "cast(d.QUARTER2 as double precision) QUARTER2, cast(d.QUARTER3 as double precision) QUARTER3, " +
                    "cast(d.QUARTER4 as double precision) QUARTER4, cast(d.SummaYear1 as double precision) SummaYear1, " +
                    "cast(d.SummaYear2 as double precision) SummaYear2, cast(d.SummaYear3 as double precision) SummaYear3, " +
                    "cast(d.MEANSTYPE as varchar(10)) meansType, cast(d.FINTYPE as varchar(10)) finType, " +
                    qsTransfert +
                    "cast(d.Progindex as varchar(10)) Progindex, " +
                    "cast(n.DAT as varchar(10)) ACCEPTDATE, cast(n.{0} as varchar(10)) NTYPE, cast(n.NOTIFYTYPE as varchar(10)) NOTIFYTYPE, " +
                    "cast(n.AssignmentSource as varchar(10)) AssignmentSource, cast(n.PlanKind as varchar(10)) PlanKind, cast(n.PlanDocType as varchar(10)) PlanDocType, " +
                    "cast(n.BA_LBO_Flag as varchar(10)) BA_LBO_Flag, b.Name BudgetBudgetName, " +
                    "cast(b.AYear as varchar(10)) BudgetBudgetYear " +
                    "from BudgetData d left join BudNotify n on (d.recordindex = n.ID) left join Budgets_s b on (d.BudgetRef = b.ID) " +
                    qsTransfertJoin + " {1}", typeFieldName, constr);
            }
            else if (MajorDBVersion >= 35)
            {
                queryStr = string.Format(
                    "select d.ID, d.BUDGETREF, d.KFSR, d.KVSR, d.KCSR, d.KVR, d.KESR, d.SUBKESR, " +
                    "d.REGIONCLS, d.AssignmentKind, d.BudgetRef, cast(d.DirectionCls as varchar(10)) DirectionCls, " +
                    "d.FACT, d.FACIALACC_CLS, cast(d.MONTH01 as double precision) MONTH01, " +
                    "cast(d.MONTH02 as double precision) MONTH02, cast(d.MONTH03 as double precision) MONTH03, " +
                    "cast(d.MONTH04 as double precision) MONTH04, cast(d.MONTH05 as double precision) MONTH05, " +
                    "cast(d.MONTH06 as double precision) MONTH06, cast(d.MONTH07 as double precision) MONTH07, " +
                    "cast(d.MONTH08 as double precision) MONTH08, cast(d.MONTH09 as double precision) MONTH09, " +
                    "cast(d.MONTH10 as double precision) MONTH10, cast(d.MONTH11 as double precision) MONTH11, " +
                    "cast(d.MONTH12 as double precision) MONTH12, cast(d.QUARTER1 as double precision) QUARTER1, " +
                    "cast(d.QUARTER2 as double precision) QUARTER2, cast(d.QUARTER3 as double precision) QUARTER3, " +
                    "cast(d.QUARTER4 as double precision) QUARTER4, cast(d.SummaYear1 as double precision) SummaYear1, " +
                    "cast(d.SummaYear2 as double precision) SummaYear2, cast(d.SummaYear3 as double precision) SummaYear3, " +
                    "d.MEANSTYPE, d.FINTYPE, n.DAT ACCEPTDATE, " +
                    "cast(d.Progindex as varchar(10)) Progindex, " +
                    "n.{0} NTYPE, n.NOTIFYTYPE, n.AssignmentSource, n.PlanKind, cast(n.PlanDocType as varchar(10)) PlanDocType, " +
                    "b.Name BudgetBudgetName, b.AYear BudgetBudgetYear " +
                    "from BudgetData d left join BudNotify n on (d.recordindex = n.ID) left join Budgets_s b on (d.BudgetRef = b.ID) {1}",
                    typeFieldName, constr);
            }
            WriteToTrace("Запрос План расходов: " + queryStr, TraceMessageKind.Information);
            InitLocalDataAdapter(this.BudgetDB, ref daBudgetFacts, queryStr);
            daBudgetFacts.Fill(dsBudgetFacts);
        }


        /// <summary>
        /// Формирует ограничение для выборки плана доходов по константам бюджета
        /// </summary>
        /// <returns></returns>
        private string GetOutcomesPlanConstr(string typeFieldName)
        {
            string result = string.Empty;

            // Вариант росписи
            int budgetList = Convert.ToInt32(GetBudgetConst(BudgetConst.IncomesVariant, "0"));

            // Уведомления по варианту росписи
            int budgetListNotify = Convert.ToInt32(GetBudgetConst(BudgetConst.IncomesVariantNotify, "0"));

            if (MajorDBVersion >= 35)
            {
                if (budgetListNotify == 0)
                {
                    result = string.Format(
                        " and ((n.{0} = 0 and n.VARIANT = {1}) or (n.{0} = 1))",
                        typeFieldName, budgetList);
                }
                else if (budgetListNotify == 1)
                {
                    result = string.Format(
                        " and ((n.{0} = 0 and n.VARIANT = {1}) or (n.{0} = 1 and n.VARIANT = {1}))",
                        typeFieldName, budgetList);
                }
            }
            else
            {
                if (budgetListNotify == 0)
                {
                    result = string.Format(
                        " and ((n.\"{0}\" = 0 and n.VARIANT = {1}) or (n.\"{0}\" = 1))",
                        typeFieldName, budgetList);
                }
                else if (budgetListNotify == 1)
                {
                    result = string.Format(
                        " and ((n.\"{0}\" = 0 and n.VARIANT = {1}) or (n.\"{0}\" = 1 and n.VARIANT = {1}))",
                        typeFieldName, budgetList);
                }
            }
            return result;
        }

        /// <summary>
        /// Запрос данных из базы бюджета
        /// </summary>
        private int QueryBudgetOutcomesPlanData()
        {
            dsBudgetFacts.Relations.Clear();

            QueryOutcomesDataTable();

            return dsBudgetFacts.Tables["Table"].Rows.Count;
        }

        /// <summary>
        /// Запрос наших данных
        /// </summary>
        private void QueryFMOutcomesPlanData()
        {
            ClearDataSet(ref dsOutcomesPlan);

            if (this.PumpMode == BudgetDataPumpMode.Update)
            {
                InitDataSet(ref daOutcomesPlan, ref dsOutcomesPlan, fctBudgetData, false,
                    string.Format("SOURCEID = {0}", this.SourceID), string.Empty);

                FillRowsCache(ref factCache, dsOutcomesPlan.Tables[0], new string[] { "SOURCEKEY", "RefDateUNV" });
            }
            else
            {
                InitFactDataSet(ref daOutcomesPlan, ref dsOutcomesPlan, fctBudgetData);
            }
            ClearDataSet(ref dsProgIndexBudget);
            InitDataSet(ref daProgIndexBudget, ref dsProgIndexBudget, fxcProgIndexBudget, true, string.Empty, string.Empty);
        }

        /// <summary>
        /// Сбрасывает закачанные данные в базу
        /// </summary>
        private void UpdateFMOutcomesPlanData()
        {
            UpdateData();

            UpdateDataSet(daOutcomesPlan, dsOutcomesPlan, fctBudgetData);
        }

        /// <summary>
        /// Закачивает строку Плана расходов
        /// </summary>
        /// <param name="planRow">Строка План расходов</param>
        /// <param name="facialAccPlan">Строка классификатора Лицевые счета</param>
        /// <param name="sum">Сумма</param>
        /// <param name="refYearMonth">Дата</param>
        /// <param name="updatedRowsCount">Счетчик обновленных строк</param>
        /// <param name="addedRowsCount">Счетчик добавленных строк</param>
        private void PumpOutcomesPlanRow(DataRow planRow, double sum, string refYearMonth, string refYearMonthUNV, 
            ref int updatedRowsCount, ref int addedRowsCount)
        {
            if (sum == 0) return;

            DataRow row = null;

            // Если работаем в режиме обновления, то сначала поищем такую запись
            if (this.PumpMode == BudgetDataPumpMode.Update)
            {
                row = FindCachedRow(factCache, new string[] { Convert.ToString(planRow["ID"]), refYearMonthUNV });
                if (row != null)
                {
                    updatedRowsCount++;
                }
            }
            if (row == null)
            {
                row = dsOutcomesPlan.Tables[0].NewRow();
                dsOutcomesPlan.Tables[0].Rows.Add(row);
                addedRowsCount++;
            }

            row["PUMPID"] = this.PumpID;
            row["SOURCEID"] = this.SourceID;
            row["SOURCEKEY"] = planRow["ID"];
            row["RefYearDayUNV"] = refYearMonthUNV;
            row["RefDateUNV"] = planRow["ACCEPTDATE"].ToString().Split('.')[0];
            row["SUMME"] = sum;

            row["FlagPlane"] = flagPlane;

            SetOrgFields(planRow, row, "FACIALACC_CLS");

            // Роспись\уведомления
            row["REFNOTIFTYPE"] = planRow["NTYPE"];

            // Администратор 
            row["REFKVSR"] = FindCachedRowID(kvsrCache, Convert.ToInt32(planRow["KVSR"]), nullKVSR);

            // Вид расходов
            row["REFKVR"] = FindCachedRowID(kvrCache, Convert.ToInt32(planRow["KVR"]), nullKVR);

            // Районы 
            row["REFREGIONS"] = FindRowID(dsRegions.Tables[0],
                new object[] { "SOURCEKEY", planRow["REGIONCLS"] }, nullRegions);

            // Мероприятие 
            row["REFFACT"] = FindCachedRowID(factClsCache, Convert.ToInt32(planRow["FACT"]), nullFact);

            // ТипУвед 
            row["REFNOTIFYTYPES"] = FindRowID(dsNotifyTypes.Tables[0],
                new object[] { "SOURCEKEY", planRow["NOTIFYTYPE"] }, nullNotifyTypes);

            // ТипФинансир 
            row["REFFINTYPE"] = FindCachedRowID(finTypeCache, Convert.ToInt32(planRow["FINTYPE"]), nullFinType);

            // Тип средств
            row["REFMEANSTYPE"] = FindCachedRowID(meansTypeCache, Convert.ToInt32(planRow["MEANSTYPE"]), nullMeansType);

            // ФКР
            row["REFFKR"] = FindCachedRowID(fkrCache, Convert.ToInt32(planRow["KFSR"]), nullFKR);
            // ЭКР
            row["REFEKR"] = FindCachedRowID(ekrCache, Convert.ToInt32(planRow["KESR"]), nullEKR);
            // КЦСР
            row["REFKCSR"] = FindCachedRowID(kcsrCache, Convert.ToInt32(planRow["KCSR"]), nullKCSR);
            // СубЭКР 
            row["REFSUBKESR"] = FindCachedRowID(subEkrCache, Convert.ToInt32(planRow["SUBKESR"]), nullSubKESR);

            // Направление 
            row["REFDIRECTION"] = FindCachedRowID(directionCache, Convert.ToInt32(planRow["DIRECTIONCLS"]), nullDirection);

            if (MajorDBVersion >= 35)
            {
                row["RefAsgmtKind"] = FindCachedRowID(asgmtKindCache, Convert.ToInt32(planRow["AssignmentKind"]), nullAsgmtKind);
                row["RefAsgmtSrce"] = FindCachedRowID(asgmtSrceCache, Convert.ToInt32(planRow["AssignmentSource"]), nullAsgmtSrce);
                row["RefFundsSource"] = FindCachedRowID(fundsSourceCache, Convert.ToInt32(planRow["AssignmentSource"]), nullFundsSource);
                row["RefBudget"] = FindBudgetBudgetRef(planRow["BudgetBudgetName"].ToString(),
                    planRow["BudgetBudgetYear"].ToString());
                row["RefPlanKind"] = FindCachedRowID(planKindCache, Convert.ToInt32(planRow["PlanKind"]), nullPlanKind);
                row["RefPlanDoc"] = FindCachedRowID(planDocTypeCache, Convert.ToInt32(planRow["PlanDocType"]), nullPlanDocType);
            }

            if (MajorDBVersion >= 37)
                row["RefBaLbo"] = Convert.ToInt32(planRow["BA_LBO_Flag"]);

            string transfert = "-1";
            if (MajorDBVersion >= 38)
            {
                transfert = planRow["TransfertCLS"].ToString();
                if (transfert == string.Empty)
                    transfert = "-1";
            }
            row["RefTransf"] = FindCachedRowID(transfertCache, Convert.ToInt32(transfert), nullTransfert);

            row["RefProgIndex"] = FindRowID(dsProgIndexBudget.Tables[0], string.Format("CODESTR = '{0}'", planRow["PROGINDEX"]), -1);
        }

        private void PumpOutcomesPlanRow(DataRow planRow, double sum, string refYearMonth, 
            ref int updatedRowsCount, ref int addedRowsCount)
        {
            PumpOutcomesPlanRow(planRow, sum, refYearMonth, refYearMonth, ref updatedRowsCount, ref addedRowsCount);
        }

        /// <summary>
        /// Закачивает данные плана расходов бюджета
        /// </summary>
        private void PumpOutcomesPlan(ref int addedRowsCount, ref int updatedRowsCount, ref int processedRecCount,
            ref string skippedRecsMessage)
        {
            DataRow planRow;
            try
            {
                // FMQ00005755 суммы в БД всегда в рублх вне зависимости от того, что суммы  в тысячах в интерфейсе
                int sumFactor = 1;
                // Ввод бюджета по месяцам или по квараталам
                bool outcomesByMonths = GetBudgetConst(BudgetConst.OutcomesByMonths, "0") == "0";

                int totalRecs = dsBudgetFacts.Tables["Table"].Rows.Count;

                // Обработка полученных данных
                for (int i = 0; i < totalRecs; i++)
                {
                    planRow = dsBudgetFacts.Tables["Table"].Rows[i];
                    // Проверка даты

                    int date = Convert.ToInt32(planRow["ACCEPTDATE"].ToString().Split('.')[0]);
                    if (!CheckDate(date))
                    {
                        processedRecCount++;
                        continue;
                    }

                    double monthsSum = 0;

                    try
                    {
                        if (MajorDBVersion >= 35)
                        {
                            // c 2008 года - алгоритм другой, качаем все суммы с разным признаком
                            if (this.DataSource.Year >= 2008)
                            {
                                double sum = 0;
                                // месяца
                                flagPlane = 3;
                                for (int j = 1; j <= 12; j++)
                                {
                                    string month = string.Format("MONTH{0}", j.ToString().PadLeft(2, '0'));
                                    sum = GetDoubleCellValue(planRow, month, 0) * sumFactor;
                                    PumpOutcomesPlanRow(planRow, sum,
                                        string.Format("{0}{1}00", this.BudgetYear, j.ToString().PadLeft(2, '0')),
                                        ref updatedRowsCount, ref addedRowsCount);
                                }

                                // кварталы
                                flagPlane = 1;
                                for (int j = 1; j <= 4; j++)
                                {
                                    string quarter = "QUARTER" + j.ToString();
                                    sum = GetDoubleCellValue(planRow, quarter, 0) * sumFactor;
                                    PumpOutcomesPlanRow(planRow, sum,
                                        string.Format("{0}999{1}", this.BudgetYear, j.ToString()),
                                        ref updatedRowsCount, ref addedRowsCount);
                                }

                                // год
                                flagPlane = 0;
                                string year = "SummaYear1";
                                sum = GetDoubleCellValue(planRow, year, 0) * sumFactor;
                                string refYearMonth = string.Format("{0}0000", this.BudgetYear);
                                string refYearMonthUNV = string.Format("{0}0001", this.BudgetYear);
                                PumpOutcomesPlanRow(planRow, sum, refYearMonth, refYearMonthUNV,
                                    ref updatedRowsCount, ref addedRowsCount);
                            }
                            else
                            {
                                // если есть данные по месяцам - качаем только по месяцам, если нет то, если по кварталам, 
                                // если нет то по текущему году (два последующих года качаем всегда)
                                bool toPump = true;
                                // месяца
                                for (int j = 1; j <= 12; j++)
                                {
                                    string month = string.Format("MONTH{0}", j.ToString().PadLeft(2, '0'));
                                    double sum = GetDoubleCellValue(planRow, month, 0) * sumFactor;
                                    if (sum != 0)
                                        toPump = false;
                                    PumpOutcomesPlanRow(planRow, sum,
                                        string.Format("{0}{1}00", this.BudgetYear, j.ToString().PadLeft(2, '0')),
                                        ref updatedRowsCount, ref addedRowsCount);
                                }
                                // кварталы
                                if (toPump)
                                    for (int j = 1; j <= 4; j++)
                                    {
                                        string quarter = "QUARTER" + j.ToString();
                                        double sum = GetDoubleCellValue(planRow, quarter, 0) * sumFactor;
                                        if (sum != 0)
                                            toPump = false;
                                        PumpOutcomesPlanRow(planRow, sum,
                                            string.Format("{0}999{1}", this.BudgetYear, j.ToString()),
                                            ref updatedRowsCount, ref addedRowsCount);
                                    }
                                // год
                                if (toPump)
                                {
                                    string year = "SummaYear1";
                                    double sum = GetDoubleCellValue(planRow, year, 0) * sumFactor;
                                    string refYearMonth = string.Format("{0}0000", this.BudgetYear);
                                    string refYearMonthUNV = string.Format("{0}0001", this.BudgetYear);
                                    PumpOutcomesPlanRow(planRow, sum, refYearMonth, refYearMonthUNV,
                                        ref updatedRowsCount, ref addedRowsCount);
                                }
                                // два последующих года - качаем всегда
                                for (int j = 2; j <= 3; j++)
                                {
                                    string year = "SummaYear" + j.ToString();
                                    double sum = GetDoubleCellValue(planRow, year, 0) * sumFactor;
                                    string refYearMonth = string.Format("{0}0000", this.BudgetYear + j - 1);
                                    string refYearMonthUNV = string.Format("{0}0001", this.BudgetYear + j - 1);
                                    PumpOutcomesPlanRow(planRow, sum, refYearMonth, refYearMonthUNV,
                                        ref updatedRowsCount, ref addedRowsCount);
                                }
                            }
                        }
                        else
                        {

                            // Суммы по кварталам = Quarter1 ... Quarter4 и суммы по месяцам = Month1 ... Month12 
                            // все ракладываются по одной записи (на каждую сумму новая запись). 
                            // Нулевые суммы не раскладываются. И устанавливается конкретный год и месяц

                            if (outcomesByMonths)
                            {
                                for (int j = 1; j <= 12; j++)
                                {
                                    string month = string.Format("MONTH{0}", j.ToString().PadLeft(2, '0'));
                                    double sum = GetDoubleCellValue(planRow, month, 0) * sumFactor;

                                    PumpOutcomesPlanRow(planRow, sum,
                                        string.Format("{0}{1}00", this.BudgetYear, j.ToString().PadLeft(2, '0')),
                                        ref updatedRowsCount, ref addedRowsCount);

                                    monthsSum += sum;

                                    if (j % 3 == 0)
                                    {
                                        double quarterSum = System.Math.Round(
                                            GetDoubleCellValue(planRow, "QUARTER" + (j / 3).ToString(), 0) * sumFactor, 2);

                                        if (quarterSum != System.Math.Round(monthsSum, 2))
                                        {
                                            string msg = string.Format(
                                                "Сумма месяцев {0} не совпадает с суммой {1} квартала {2} (строка {3}).",
                                                monthsSum, (j / 3).ToString(), quarterSum, planRow["ID"]);
                                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, msg);
                                        }

                                        monthsSum = 0;
                                    }
                                }
                            }
                            else
                            {
                                for (int j = 1; j <= 4; j++)
                                {
                                    string quarter = "QUARTER" + j.ToString();

                                    PumpOutcomesPlanRow(planRow,
                                        GetDoubleCellValue(planRow, quarter, 0) * sumFactor,
                                        string.Format("{0}999{1}", this.BudgetYear, j.ToString()),
                                        ref updatedRowsCount, ref addedRowsCount);
                                }
                            }
                        }
                        // Счетчик закачанных записей
                        processedRecCount++;
                        SetProgress(totalRecs, i + 1,
                            string.Format("Обработка данных базы {0} ({1} из {2})...",
                                this.DatabasePath, filesCount, totalFiles),
                            string.Format("{0}. Запись {1} из {2}", currentBlockName, i + 1, totalRecs));

                        // Если накопилось много записей, то сбрасываем в базу
                        if (processedRecCount * 4 >= MAX_DS_RECORDS_AMOUNT)
                        {
                            UpdateFMOutcomesPlanData();
                            if (this.PumpMode != BudgetDataPumpMode.Update)
                            {
                                ClearDataSet(ref dsOutcomesPlan);
                                daOutcomesPlan.Fill(dsOutcomesPlan);
                            }
                            processedRecCount = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToTrace(string.Format("СТРОКА {0} - {1}", planRow["ID"], ex), TraceMessageKind.Error);
                        throw new PumpDataFailedException(string.Format("СТРОКА {0} - {1}", planRow["ID"], ex.Message));
                    }
                }

                // Сохранение данных
                UpdateFMOutcomesPlanData();
            }
            finally
            {
                ClearDataSet(ref dsOutcomesPlan);
            }
        }

        #region лимиты

        private int QueryBudgetOutcomesPlanDataLimit()
        {
            dsBudgetFacts.Relations.Clear();
            QueryOutcomesDataLimitTable();
            return dsBudgetFacts.Tables["Table"].Rows.Count;
        }

        // запрос данных из LimitFinDetail (новосиб)
        private void QueryOutcomesDataLimitTable()
        {
            string queryStr = string.Empty;
            string constr = string.Format("where d.budgetref = {0} and n.Reject_Cls is null {1} and (d.clsType = '0') ",
                this.BudgetRef, GetDateConstr("d."));

            string constrByPumpParams = GetDateConstrByPumpParams("d.AcceptDate", true);
            if (constrByPumpParams == string.Empty)
                constr += " and (d.AcceptDate between 20000101 and 20201217)";
            else
            {
                constr += " and " + constrByPumpParams;
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                    string.Format("Закачка выполняется с ограничением по дате: {0}..{1}", this.dateMin, this.dateMax));
            }
            string qsTransfert = string.Empty;
            string qsTransfertJoin = string.Empty;
            if (MajorDBVersion >= 38)
            {
                qsTransfert = "cast(tr.code as varchar(10)) TransfertCLS, ";
                qsTransfertJoin = " left join transfertcls tr on (d.transfertcls = tr.ID) ";
            }

            queryStr = string.Format(
                "select cast(d.ID as varchar(10)) id, cast(d.BUDGETREF as varchar(10)) budgetRef, " +
                "cast(d.AMonth as varchar(10)) AMonth, cast(d.AYear as varchar(10)) AYear, " +
                "cast(d.AcceptDate as varchar(10)) AcceptDate, cast(d.ACCEPTSUMMA as double precision) ACCEPTSUMMA, " +
                "cast(n.PlanDocType as varchar(10)) PlanDocType, cast(d.FacialAcc as varchar(10)) FACIALACC_CLS, " +
                "cast(d.KVSR as varchar(10)) KVSR, cast(d.KFSR as varchar(10)) KFSR, " +
                "cast(d.KCSR as varchar(10)) KCSR, cast(d.KVR as varchar(10)) KVR, " +
                "cast(d.KESR as varchar(10)) KESR, cast(d.SubKESR as varchar(10)) SubKESR, " +
                "cast(d.RegionCLS as varchar(10)) RegionCLS, cast(d.FinSource as varchar(10)) NOTIFYTYPE, " +
                "cast(d.Fact as varchar(10)) Fact, cast(d.MeansType as varchar(10)) MeansType, " +
                qsTransfert +
                "cast(d.Progindex as varchar(10)) Progindex, " +
                "cast(d.FundsSource as varchar(10)) FundsSource, cast(d.FinType as varchar(10)) FinType, cast(n.PlanDocType as varchar(10)) PlanDocType, " +
                "cast(b.AYear as varchar(10)) BudgetBudgetYear, b.Name BudgetBudgetName " +
                "from LimitFinDetail d left join LimitNotifyCaption n on (d.recordindex = n.ID) left join Budgets_s b on (d.BudgetRef = b.ID) " +
                qsTransfertJoin + " {0}", constr);
            WriteToTrace("Запрос План расходов (лимит): " + queryStr, TraceMessageKind.Information);
            InitLocalDataAdapter(this.BudgetDB, ref daBudgetFacts, queryStr);
            daBudgetFacts.Fill(dsBudgetFacts);
        }

        private void PumpOutcomesPlanRowLimit(DataRow planRow, double sum, string refYearMonth, string refYearMonthUNV,
            ref int updatedRowsCount, ref int addedRowsCount)
        {
            if (sum == 0) 
                return;
            DataRow row = null;
            // Если работаем в режиме обновления, то сначала поищем такую запись
            if (this.PumpMode == BudgetDataPumpMode.Update)
            {
                row = FindCachedRow(factCache, new string[] { Convert.ToString(planRow["ID"]), refYearMonth });
                if (row != null)
                    updatedRowsCount++;
            }
            if (row == null)
            {
                row = dsOutcomesPlan.Tables[0].NewRow();
                dsOutcomesPlan.Tables[0].Rows.Add(row);
                addedRowsCount++;
            }

            row["PUMPID"] = this.PumpID;
            row["SOURCEID"] = this.SourceID;
            row["SOURCEKEY"] = planRow["ID"];
            row["RefYearDayUNV"] = refYearMonthUNV;
            row["RefDateUNV"] = refYearMonth;
            row["SUMME"] = sum;
            row["FlagPlane"] = 2;
            SetOrgFields(planRow, row, "FACIALACC_CLS");
            if (planRow["PlanDocType"].ToString().Trim().StartsWith("1"))
                row["REFNOTIFTYPE"] = 0;
            else
                row["REFNOTIFTYPE"] = 1;
            row["REFKVSR"] = FindCachedRowID(kvsrCache, Convert.ToInt32(planRow["KVSR"]), nullKVSR);
            row["REFKVR"] = FindCachedRowID(kvrCache, Convert.ToInt32(planRow["KVR"]), nullKVR);
            row["REFREGIONS"] = FindRowID(dsRegions.Tables[0], new object[] { "SOURCEKEY", planRow["REGIONCLS"] }, nullRegions);
            row["REFFACT"] = FindCachedRowID(factClsCache, Convert.ToInt32(planRow["FACT"]), nullFact);
            row["REFNOTIFYTYPES"] = FindRowID(dsNotifyTypes.Tables[0],
                new object[] { "SOURCEKEY", planRow["NOTIFYTYPE"] }, nullNotifyTypes);
            row["REFFINTYPE"] = FindCachedRowID(finTypeCache, Convert.ToInt32(planRow["FINTYPE"]), nullFinType);
            row["REFMEANSTYPE"] = FindCachedRowID(meansTypeCache, Convert.ToInt32(planRow["MEANSTYPE"]), nullMeansType);
            row["REFFKR"] = FindCachedRowID(fkrCache, Convert.ToInt32(planRow["KFSR"]), nullFKR);
            row["REFEKR"] = FindCachedRowID(ekrCache, Convert.ToInt32(planRow["KESR"]), nullEKR);
            row["REFKCSR"] = FindCachedRowID(kcsrCache, Convert.ToInt32(planRow["KCSR"]), nullKCSR);
            row["REFSUBKESR"] = FindCachedRowID(subEkrCache, Convert.ToInt32(planRow["SUBKESR"]), nullSubKESR);
            row["REFDIRECTION"] = nullDirection;
            row["RefFundsSource"] = FindCachedRowID(fundsSourceCache, Convert.ToInt32(planRow["FundsSource"]), nullFundsSource);
            row["RefAsgmtKind"] = nullAsgmtKind;
            row["RefAsgmtSrce"] = nullAsgmtSrce;
            row["RefBudget"] = FindBudgetBudgetRef(planRow["BudgetBudgetName"].ToString(),
                planRow["BudgetBudgetYear"].ToString());
            row["RefPlanKind"] = nullPlanKind;
            row["RefBaLbo"] = 2;
            row["RefVariant"] = -1;
            row["RefPlanDoc"] = FindCachedRowID(planDocTypeCache, Convert.ToInt32(planRow["PlanDocType"]), nullPlanDocType);
            row["RefProgIndex"] = FindRowID(dsProgIndexBudget.Tables[0], string.Format("CODESTR = '{0}'", planRow["PROGINDEX"]), -1);

            string transfert = "-1";
            if (MajorDBVersion >= 38)
            {
                transfert = planRow["TransfertCLS"].ToString();
                if (transfert == string.Empty)
                    transfert = "-1";
            }
            row["RefTransf"] = FindCachedRowID(transfertCache, Convert.ToInt32(transfert), nullTransfert);
        }

        private void PumpOutcomesPlanLimit(ref int addedRowsCount, ref int updatedRowsCount, ref int processedRecCount,
            ref string skippedRecsMessage)
        {
            DataRow planRow;
            try
            {
                int totalRecs = dsBudgetFacts.Tables["Table"].Rows.Count;
                for (int i = 0; i < totalRecs; i++)
                {
                    planRow = dsBudgetFacts.Tables["Table"].Rows[i];
                    int date = Convert.ToInt32(planRow["ACCEPTDATE"].ToString().Split('.')[0]);
                    if (!CheckDate(date))
                    {
                        processedRecCount++;
                        continue;
                    }
                    try
                    {
                        double sum = GetDoubleCellValue(planRow, "ACCEPTSUMMA", 0);
                        string refYearMonth = planRow["ACCEPTDATE"].ToString().Split('.')[0];
                        string refYearMonthUNV = string.Format("{0}{1}00", planRow["AYear"].ToString(), planRow["AMonth"].ToString().PadLeft(2, '0'));
                        PumpOutcomesPlanRowLimit(planRow, sum, refYearMonth, refYearMonthUNV,
                            ref updatedRowsCount, ref addedRowsCount);
                        processedRecCount++;
                        SetProgress(totalRecs, i + 1,
                            string.Format("Обработка данных базы {0} ({1} из {2})...",
                                this.DatabasePath, filesCount, totalFiles),
                            string.Format("{0}. Запись {1} из {2}", currentBlockName, i + 1, totalRecs));
                        if (processedRecCount >= MAX_DS_RECORDS_AMOUNT)
                        {
                            UpdateFMOutcomesPlanData();
                            if (this.PumpMode != BudgetDataPumpMode.Update)
                            {
                                ClearDataSet(ref dsOutcomesPlan);
                                daOutcomesPlan.Fill(dsOutcomesPlan);
                            }
                            processedRecCount = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToTrace(string.Format("СТРОКА {0} - {1}", planRow["ID"], ex), TraceMessageKind.Error);
                        throw new PumpDataFailedException(string.Format("СТРОКА {0} - {1}", planRow["ID"], ex.Message));
                    }
                }
                UpdateFMOutcomesPlanData();
            }
            finally
            {
                ClearDataSet(ref dsOutcomesPlan);
            }
        }

        #endregion лимиты

    }
}
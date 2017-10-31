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
    // источники финансирования - план

    public partial class BudgetDataPumpModule : BudgetPumpModuleBase
    {
        private IDbDataAdapter daIFPlan;
        private DataSet dsIFPlan = new DataSet();

        private void QueryIFPlanDataTable()
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
                this.BudgetRef, GetOutcomesPlanConstr(typeFieldName), GetDateConstr("d."), " and (d.clsType = '1')");
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

            queryStr = string.Format(
                "select cast(d.ID as varchar(10)) id, cast(d.BUDGETREF as varchar(10)) budgetRef, " +
                "cast(d.SUBKESR as varchar(10)) SUBKESR, " +
                "cast(d.REGIONCLS as varchar(10)) REGIONCLS, cast(d.AssignmentKind as varchar(10)) AssignmentKind, " +
                "cast(d.FACT as varchar(10)) fact, " +
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
                "cast(d.MEANSTYPE as varchar(10)) meansType, " +
                "cast(d.Progindex as varchar(10)) Progindex, " +
                "cast(d.IFS as varchar(10)) IFS, " + 
                "cast(n.DAT as varchar(10)) ACCEPTDATE, cast(n.{0} as varchar(10)) NTYPE, cast(n.NOTIFYTYPE as varchar(10)) NOTIFYTYPE, " +
                "cast(n.AssignmentSource as varchar(10)) AssignmentSource, cast(n.PlanKind as varchar(10)) PlanKind, cast(n.PlanDocType as varchar(10)) PlanDocType, " +
                "b.Name BudgetBudgetName, " +
                "cast(b.AYear as varchar(10)) BudgetBudgetYear " +
                "from BudgetData d left join BudNotify n on (d.recordindex = n.ID) left join Budgets_s b on (d.BudgetRef = b.ID) {1}",
                typeFieldName, constr);

            WriteToTrace("Запрос ИФ План: " + queryStr, TraceMessageKind.Information);
            InitLocalDataAdapter(this.BudgetDB, ref daBudgetFacts, queryStr);
            daBudgetFacts.Fill(dsBudgetFacts);
        }

        private int QueryBudgetIFPlanData()
        {
            dsBudgetFacts.Relations.Clear();
            QueryIFPlanDataTable();
            return dsBudgetFacts.Tables["Table"].Rows.Count;
        }

        private void QueryFMIFPlanData()
        {
            ClearDataSet(ref dsIFPlan);
            if (this.PumpMode == BudgetDataPumpMode.Update)
            {
                InitDataSet(ref daIFPlan, ref dsIFPlan, fctIFPlan, false, string.Format("SOURCEID = {0}", this.SourceID), string.Empty);
                FillRowsCache(ref factCache, dsIFPlan.Tables[0], new string[] { "SOURCEKEY", "RefDateUNV" });
            }
            else
            {
                InitFactDataSet(ref daIFPlan, ref dsIFPlan, fctIFPlan);
            }
            ClearDataSet(ref dsProgIndexBudget);
            InitDataSet(ref daProgIndexBudget, ref dsProgIndexBudget, fxcProgIndexBudget, true, string.Empty, string.Empty);
        }

        private void UpdateFMIFPlanData()
        {
            UpdateData();
            UpdateDataSet(daIFPlan, dsIFPlan, fctIFPlan);
        }

        private void PumpIFPlanRow(DataRow planRow, double sum, string refYearMonth, string refYearMonthUNV,
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
                row = dsIFPlan.Tables[0].NewRow();
                dsIFPlan.Tables[0].Rows.Add(row);
                addedRowsCount++;
            }

            row["PUMPID"] = this.PumpID;
            row["SOURCEID"] = this.SourceID;
            row["SOURCEKEY"] = planRow["ID"];
            row["RefYearDayUNV"] = refYearMonthUNV;
            row["RefDateUNV"] = planRow["ACCEPTDATE"].ToString().Split('.')[0];
            row["SUMMA"] = sum;
            row["FlagPlane"] = flagPlane;
            SetOrgFields(planRow, row, "FACIALACC_CLS");
            row["REFNOTIFTYPE"] = planRow["NTYPE"];
            row["REFREGIONS"] = FindRowID(dsRegions.Tables[0], new object[] { "SOURCEKEY", planRow["REGIONCLS"] }, nullRegions);
            row["REFFACT"] = FindCachedRowID(factClsCache, Convert.ToInt32(planRow["FACT"]), nullFact);
            row["REFNOTIFYTYPES"] = FindRowID(dsNotifyTypes.Tables[0], new object[] { "SOURCEKEY", planRow["NOTIFYTYPE"] }, nullNotifyTypes);
            row["REFMEANSTYPE"] = FindCachedRowID(meansTypeCache, Convert.ToInt32(planRow["MEANSTYPE"]), nullMeansType);
            row["REFSUBKESR"] = FindCachedRowID(subEkrCache, Convert.ToInt32(planRow["SUBKESR"]), nullSubKESR);
            row["REFKIF"] = FindCachedRowID(ifsCache, Convert.ToInt32(planRow["IFS"]), nullKIF2005);

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

            row["RefProgIndex"] = FindRowID(dsProgIndexBudget.Tables[0], string.Format("CODESTR = '{0}'", planRow["PROGINDEX"]), -1);
        }

        private void PumpIFPlanRow(DataRow planRow, double sum, string refYearMonth,
            ref int updatedRowsCount, ref int addedRowsCount)
        {
            PumpIFPlanRow(planRow, sum, refYearMonth, refYearMonth, ref updatedRowsCount, ref addedRowsCount);
        }

        private void PumpIFPlan(ref int addedRowsCount, ref int updatedRowsCount, ref int processedRecCount,
            ref string skippedRecsMessage)
        {
            DataRow planRow;
            try
            {
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

                                    PumpIFPlanRow(planRow, sum,
                                        string.Format("{0}999{1}", this.BudgetYear, j.ToString()),
                                        ref updatedRowsCount, ref addedRowsCount);
                                }

                                // год
                                flagPlane = 0;
                                string year = "SummaYear1";
                                sum = GetDoubleCellValue(planRow, year, 0) * sumFactor;
                                string refYearMonth = string.Format("{0}0000", this.BudgetYear);
                                string refYearMonthUNV = string.Format("{0}0001", this.BudgetYear);
                                PumpIFPlanRow(planRow, sum, refYearMonth, refYearMonthUNV,
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
                                    PumpIFPlanRow(planRow, sum,
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
                                        PumpIFPlanRow(planRow, sum,
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
                                    PumpIFPlanRow(planRow, sum, refYearMonth, refYearMonthUNV,
                                        ref updatedRowsCount, ref addedRowsCount);
                                }
                                // два последующих года - качаем всегда
                                for (int j = 2; j <= 3; j++)
                                {
                                    string year = "SummaYear" + j.ToString();
                                    double sum = GetDoubleCellValue(planRow, year, 0) * sumFactor;
                                    string refYearMonth = string.Format("{0}0000", this.BudgetYear + j - 1);
                                    string refYearMonthUNV = string.Format("{0}0001", this.BudgetYear + j - 1);
                                    PumpIFPlanRow(planRow, sum, refYearMonth, refYearMonthUNV,
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
                                    PumpIFPlanRow(planRow, sum,
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
                                    PumpIFPlanRow(planRow,
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
                            UpdateFMIFPlanData();
                            if (this.PumpMode != BudgetDataPumpMode.Update)
                            {
                                ClearDataSet(ref dsIFPlan);
                                daOutcomesPlan.Fill(dsIFPlan);
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
                UpdateFMIFPlanData();
            }
            finally
            {
                ClearDataSet(ref dsIFPlan);
            }
        }

    }
}
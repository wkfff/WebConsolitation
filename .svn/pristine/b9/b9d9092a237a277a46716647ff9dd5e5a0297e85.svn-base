using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using System.Threading;

using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps.BudgetDataPump
{
	// Закачка данных плана доходов бюджета

    public partial class BudgetDataPumpModule : BudgetPumpModuleBase
	{
		private IDbDataAdapter daIncomesPlan;
		private DataSet dsIncomesPlan = new DataSet();
        private int flagPlane = 0; 

        /// <summary>
        /// Запрашивает данные IncomesData
        /// </summary>
        private void QueryIncomesDataTable()
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

            string dateField = "ACCEPTDATE";
            if ((MajorDBVersion * 100 + MinorDBVersion) >= 3703)
                dateField = "DAT";

            string constr = string.Format(
                "where n.{0} is not null and d.budgetref = {1} {2} {3}",
                dateField, this.BudgetRef, GetIncomesPlanConstr(typeFieldName), GetDateConstr("d."));
            if ((MajorDBVersion * 100 + MinorDBVersion) >= 3703)
                constr += " and (d.clsType = '2') ";

            if ((MajorDBVersion * 100 + MinorDBVersion) >= 3703)
                CheckBudgetTableDate("BudgetData d left join BudNotify n on (d.recordindex = n.ID) ", string.Empty, dateField, constr);
            else
                CheckBudgetTableDate("IncomesData d left join IncomesNotif n on (d.recordindex = n.ID)", string.Empty, dateField, constr);

            string constrByPumpParams = GetDateConstrByPumpParams("n." + dateField, true);
            if (constrByPumpParams == string.Empty)
                constr += string.Format(" and (n.{0} between 20000101 and 20201217)", dateField);
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

            constr += " and (n.rejectCls is null) ";
            if ((MajorDBVersion * 100 + MinorDBVersion) >= 3703)
            {
                // качается из budget_data
                queryStr = string.Format(
                    "select cast(d.ID as varchar(10)) id, cast(d.BUDGETREF as varchar(10)) budgetRef, cast(d.KD as varchar(10)) kd, " +
                        "cast(d.REGIONCLS as varchar(10)) regionCls, " +
                        "cast(d.FundsSource as varchar(10)) FundsSource, cast(d.ProgIndex as varchar(10)) ProgIndex, " +
                        "cast(d.ASSIGNMENTKIND as varchar(10)) ASSIGNMENTKIND, cast(d.MONTH01 as double precision) MONTH01, " +
                        "cast(d.MONTH02 as double precision) MONTH02, cast(d.MONTH03 as double precision) MONTH03, " +
                        "cast(d.MONTH04 as double precision) MONTH04, cast(d.MONTH05 as double precision) MONTH05, " +
                        "cast(d.MONTH06 as double precision) MONTH06, cast(d.MONTH07 as double precision) MONTH07, " +
                        "cast(d.MONTH08 as double precision) MONTH08, cast(d.MONTH09 as double precision) MONTH09, " +
                        "cast(d.MONTH10 as double precision) MONTH10, cast(d.MONTH11 as double precision) MONTH11, " +
                        "cast(d.MONTH12 as double precision) MONTH12, cast(d.QUARTER1 as double precision) QUARTER1, " +
                        "cast(d.QUARTER2 as double precision) QUARTER2, cast(d.QUARTER3 as double precision) QUARTER3, " +
                        "cast(d.QUARTER4 as double precision) QUARTER4, cast(d.SummaYear1 as double precision) SummaYear1, " +
                        "cast(d.SummaYear2 as double precision) SummaYear2, cast(d.SummaYear3 as double precision) SummaYear3, " +
                        "cast(d.MEANSTYPE as varchar(10)) MEANSTYPE, cast(d.FACIALACC_CLS as varchar(10)) FACIALACC_CLS, " +
                        qsTransfert + 
                        "cast(n.PlanDocType as varchar(10)) PlanDocType, " +
                        "cast(n.DAT as varchar(10)) ACCEPTDATE, cast(n.VARIANT as varchar(10)) variant, " +
                        "cast(n.{0} as varchar(10)) NTYPE, cast(n.ASSIGNMENTSOURCE as varchar(10)) ASSIGNMENTSOURCE " +
                        "from BudgetData d left join BudNotify n on (d.recordindex = n.ID) " + 
                        qsTransfertJoin + " {1}",
                    typeFieldName, constr);
            }
            else if (MajorDBVersion >= 36)
            {
                queryStr = string.Format(
                    "select d.ID, d.BUDGETREF, d.KD, d.REGIONCLS, d.FundsSource, d.ProgIndex, d.ASSIGNMENTKIND, " +
                        "cast(d.MONTH01 as double precision) MONTH01, " +
                        "cast(d.MONTH02 as double precision) MONTH02, cast(d.MONTH03 as double precision) MONTH03, " +
                        "cast(d.MONTH04 as double precision) MONTH04, cast(d.MONTH05 as double precision) MONTH05, " +
                        "cast(d.MONTH06 as double precision) MONTH06, cast(d.MONTH07 as double precision) MONTH07, " +
                        "cast(d.MONTH08 as double precision) MONTH08, cast(d.MONTH09 as double precision) MONTH09, " +
                        "cast(d.MONTH10 as double precision) MONTH10, cast(d.MONTH11 as double precision) MONTH11, " +
                        "cast(d.MONTH12 as double precision) MONTH12, cast(d.QUARTER1 as double precision) QUARTER1, " +
                        "cast(d.QUARTER2 as double precision) QUARTER2, cast(d.QUARTER3 as double precision) QUARTER3, " +
                        "cast(d.QUARTER4 as double precision) QUARTER4, cast(d.SummaYear1 as double precision) SummaYear1, " +
                        "cast(d.SummaYear2 as double precision) SummaYear2, cast(d.SummaYear3 as double precision) SummaYear3, " +
                        "cast(n.PlanDocType as varchar(10)) PlanDocType, " +
                        "d.MEANSTYPE, d.FACIALACC_CLS, n.ACCEPTDATE, n.VARIANT, n.{0} NTYPE, n.ASSIGNMENTSOURCE " +
                        "from IncomesData d left join IncomesNotif n on (d.recordindex = n.ID) {1}",
                    typeFieldName, constr);
            }
            else if (MajorDBVersion >= 35)
            {
                queryStr = string.Format(
                    "select d.ID, d.BUDGETREF, d.KD, d.REGIONCLS, d.FundsSource, " +
                        "cast(d.MONTH01 as double precision) MONTH01, " +
                        "cast(d.MONTH02 as double precision) MONTH02, cast(d.MONTH03 as double precision) MONTH03, " +
                        "cast(d.MONTH04 as double precision) MONTH04, cast(d.MONTH05 as double precision) MONTH05, " +
                        "cast(d.MONTH06 as double precision) MONTH06, cast(d.MONTH07 as double precision) MONTH07, " +
                        "cast(d.MONTH08 as double precision) MONTH08, cast(d.MONTH09 as double precision) MONTH09, " +
                        "cast(d.MONTH10 as double precision) MONTH10, cast(d.MONTH11 as double precision) MONTH11, " +
                        "cast(d.MONTH12 as double precision) MONTH12, cast(d.QUARTER1 as double precision) QUARTER1, " +
                        "cast(d.QUARTER2 as double precision) QUARTER2, cast(d.QUARTER3 as double precision) QUARTER3, " +
                        "cast(d.QUARTER4 as double precision) QUARTER4, cast(d.SummaYear1 as double precision) SummaYear1, " +
                        "cast(d.SummaYear2 as double precision) SummaYear2, cast(d.SummaYear3 as double precision) SummaYear3, " +
                        "cast(n.PlanDocType as varchar(10)) PlanDocType, " +
                        "d.MEANSTYPE, d.FACIALACC_CLS, n.ACCEPTDATE, n.VARIANT, n.{0} NTYPE " +
                    "from IncomesData d left join IncomesNotif n on (d.recordindex = n.ID) {1}",
                    typeFieldName, constr);
            }
            WriteToTrace("Запрос План доходов: " + queryStr, TraceMessageKind.Information);
            InitLocalDataAdapter(this.BudgetDB, ref daBudgetFacts, queryStr);
            daBudgetFacts.Fill(dsBudgetFacts);
        }

        /// <summary>
        /// Формирует ограничение для выборки плана доходов по константам бюджета
        /// </summary>
        /// <returns></returns>
        private string GetIncomesPlanConstr(string typeFieldName)
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
		private int QueryBudgetIncomesPlanData()
		{
            ClearDataSet(ref dsBudgetKESR);
            InitLocalDataAdapter(this.BudgetDB, ref daBudgetKESR, "select cast(id as varChar(10)) id, name from KESR");
            daBudgetKESR.Fill(dsBudgetKESR);

            FillRowsCache(ref budgetKesrCache, dsBudgetKESR.Tables[0], "ID");

            QueryIncomesDataTable();

            return dsBudgetFacts.Tables["Table"].Rows.Count;
		}

		/// <summary>
		/// Запрос наших данных
		/// </summary>
		private void QueryFMIncomesPlanData()
		{
            ClearDataSet(ref dsIncomesPlan);

            if (this.PumpMode == BudgetDataPumpMode.Update)
            {
                InitDataSet(ref daIncomesPlan, ref dsIncomesPlan, fctIncomesPlan, false,
                    string.Format("SOURCEID = {0}", this.SourceID), string.Empty);

                FillRowsCache(ref factCache, dsIncomesPlan.Tables[0], new string[] { "SOURCEKEY", "RefDateUNV" });
            }
            else
            {
                InitFactDataSet(ref daIncomesPlan, ref dsIncomesPlan, fctIncomesPlan);
            }
		}

		/// <summary>
		/// Сбрасывает закачанные данные в базу
		/// </summary>
		private void UpdateFMIncomesPlanData()
		{
            UpdateData();

            UpdateDataSet(daIncomesPlan, dsIncomesPlan, fctIncomesPlan);
		}

		/// <summary>
		/// Закачивает строку Плана доходов
		/// </summary>
		/// <param name="planRow">Строка План доходов</param>
		/// <param name="sum">Сумма</param>
		/// <param name="refYearMonth">Месяц/квартал</param>
		/// <param name="updatedRowsCount">Количество обновленных записей</param>
		/// <param name="addedRowsCount">Количество добавленных записей</param>
		private void PumpIncomesPlanRow(DataRow planRow, double sum, 
            string refYearMonth, string refYearMonthUNV, ref int updatedRowsCount, ref int addedRowsCount)
		{
			if (sum == 0) return;

			DataRow row = null;

            // Если работаем в режиме обновления, то сначала поищем такую запись
            if (this.PumpMode == BudgetDataPumpMode.Update)
            {
                row = FindCachedRow(factCache, new string[] { Convert.ToString(planRow["ID"]), refYearMonth });
                if (row != null)
                {
                    updatedRowsCount++;
                }
            }
            if (row == null)
            {
                row = dsIncomesPlan.Tables[0].NewRow();
                row["SOURCEKEY"] = planRow["ID"];
                row["RefYearDayUNV"] = refYearMonthUNV;
                dsIncomesPlan.Tables[0].Rows.Add(row);
                addedRowsCount++;
            }

            row["PUMPID"] = this.PumpID;
            row["SOURCEID"] = this.SourceID;
            row["RefDateUNV"] = planRow["ACCEPTDATE"].ToString().Split('.')[0];
            row["SUMMA"] = sum;

            row["FlagPlane"] = flagPlane;

            DataRow kdRowData = FindCachedRow(kdCache, Convert.ToInt32(planRow["KD"]));

            if (kdRowData != null)
                row["RefKDASBudget"] = GetRowID(kdRowData, nullKD);

            row["REFINCOMINGSITEMS"] = GetIntCellValue(kdRowData, "ITEMCODE", -1);

            switch (this.CurrentDBVersion)
            {
                case "27.02":
                case "28.00":
                case "29.01":
                case "29.02":
                case "30.00":
                case "30.01":

                    // Администратор доходов
                    row["REFKVSR"] = nullKVSR;

                    // Виды доходов
                    row["REFINCOMESKINDS"] = nullIncomesKinds;

                    // Программы
                    row["REFPROGRAMS"] = nullPrograms;

                    // ЭКД	
                    row["REFEKD"] = nullEKD;

                    break;

                default:
                    // Администратор доходов
                    row["REFKVSR"] = kdRowData["KVSR"];

                    // Виды доходов
                    row["REFINCOMESKINDS"] = FindCachedRowID(incomesKindsCache, new string[] { 
                        Convert.ToString(kdRowData["DESCRIPTIONCODE"]), Convert.ToString(kdRowData["NAME"]) }, nullIncomesKinds);

                    // Программы
                    row["REFPROGRAMS"] = kdRowData["PROGRAMCODE"];

                    // ЭКД
                    DataRow kesrRow = FindCachedRow(budgetKesrCache, Convert.ToInt32(kdRowData["KESR"]));
                    row["REFEKD"] = PumpOriginalRow(dsEKD, clsEKD,
                        new object[] { "CODE", kesrRow["ID"], "NAME", kesrRow["NAME"] });

                    break;
            }

            // Роспись/уведомления
            row["REFNOTIFTYPE"] = planRow["NTYPE"];

            // Тип средств
            switch (this.CurrentDBVersion)
            {
                case "27.02":
                    row["REFMEANSTYPE"] = nullMeansType;
                    break;

                default:
                    row["REFMEANSTYPE"] = FindCachedRowID(meansTypeCache, Convert.ToInt32(planRow["MEANSTYPE"]), nullMeansType);
                    break;
            }
            

            // Районы
            row["REFREGIONS"] = FindRowID(dsRegions.Tables[0], string.Format("SOURCEKEY = {0}", planRow["REGIONCLS"]), nullRegions);

            if (MajorDBVersion >= 35)
                row["RefFundsSource"] = FindCachedRowID(fundsSourceCache, Convert.ToInt32(planRow["FundsSource"]), nullFundsSource);

            if (MajorDBVersion >= 36)
            {
                row["ProgIndex"] = Convert.ToInt32(planRow["ProgIndex"]);
                row["RefAsgmtKind"] = FindCachedRowID(asgmtKindCache, Convert.ToInt32(planRow["AssignmentKind"]), nullAsgmtKind);
                row["RefAsgmtSrce"] = FindCachedRowID(asgmtSrceCache, Convert.ToInt32(planRow["AssignmentSource"]), nullAsgmtSrce);
            }

            // лицевые счета
            row["RefFacialAcc"] = FindCachedRowID(facialAccCache, Convert.ToInt32(planRow["FACIALACC_CLS"]), nullFacialAcc);

            row["RefPlanDoc"] = FindCachedRowID(planDocTypeCache, Convert.ToInt32(planRow["PlanDocType"]), nullPlanDocType);

            string transfert = "-1";
            if (MajorDBVersion >= 38)
            {
                transfert = planRow["TransfertCLS"].ToString();
                if (transfert == string.Empty)
                    transfert = "-1";
            }
            row["RefTransf"] = FindCachedRowID(transfertCache, Convert.ToInt32(transfert), nullTransfert);
		}

        private void PumpIncomesPlanRow(DataRow planRow, double sum,
            string refYearMonth, ref int updatedRowsCount, ref int addedRowsCount)
        {
            PumpIncomesPlanRow(planRow, sum, refYearMonth, refYearMonth, ref updatedRowsCount, ref addedRowsCount);
        }

		/// <summary>
		/// Закачивает данные плана доходов бюджета
		/// </summary>
        private void PumpIncomesPlan(ref int addedRowsCount, ref int updatedRowsCount, ref int processedRecCount,
            ref string skippedRecsMessage)
		{
            DataRow planRow = null;

            try
			{
                // FMQ00005755 суммы в БД всегда в рублх вне зависимости от того, что суммы  в тысячах в интерфейсе
                int sumFactor = 1;
                // Ввод бюджета по месяцам или по квараталам
                bool incomesByMonths = GetBudgetConst(BudgetConst.IncomesByMonths, "0") == "0";

                int totalRecs = dsBudgetFacts.Tables["Table"].Rows.Count;

				// Обработка полученных данных
                for (int i = 0; i < totalRecs; i++)
				{
					planRow = dsBudgetFacts.Tables["Table"].Rows[i];

                    int date = Convert.ToInt32(planRow["ACCEPTDATE"].ToString().Split('.')[0]);
                    // Проверка даты
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
                                string refYearMonth = string.Empty;
                                // месяца
                                flagPlane = 2;
                                for (int j = 1; j <= 12; j++)
                                {
                                    string month = string.Format("MONTH{0}", j.ToString().PadLeft(2, '0'));
                                    sum = GetDoubleCellValue(planRow, month, 0) * sumFactor;
                                    refYearMonth = string.Format("{0}{1}00", this.BudgetYear, j.ToString().PadLeft(2, '0'));
                                    PumpIncomesPlanRow(planRow, sum, refYearMonth, ref updatedRowsCount, ref addedRowsCount);
                                }
                                // кварталы
                                flagPlane = 1;
                                for (int j = 1; j <= 4; j++)
                                {
                                    string quarter = "QUARTER" + j.ToString();
                                    sum = GetDoubleCellValue(planRow, quarter, 0) * sumFactor;
                                    refYearMonth = string.Format("{0}999{1}", this.BudgetYear, j.ToString());
                                    PumpIncomesPlanRow(planRow, sum, refYearMonth, ref updatedRowsCount, ref addedRowsCount);
                                }
                                // год
                                flagPlane = 0;
                                string year = "SummaYear1";
                                sum = GetDoubleCellValue(planRow, year, 0) * sumFactor;
                                refYearMonth = string.Format("{0}0000", this.BudgetYear);
                                string refYearMonthUNV = string.Format("{0}0001", this.BudgetYear);
                                PumpIncomesPlanRow(planRow, sum, refYearMonth, refYearMonthUNV,
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
                                    string refYearMonth = string.Format("{0}{1}00", this.BudgetYear, j.ToString().PadLeft(2, '0'));
                                    PumpIncomesPlanRow(planRow, sum, refYearMonth, ref updatedRowsCount, ref addedRowsCount);
                                }
                                // кварталы
                                if (toPump)
                                    for (int j = 1; j <= 4; j++)
                                    {
                                        string quarter = "QUARTER" + j.ToString();
                                        double sum = GetDoubleCellValue(planRow, quarter, 0) * sumFactor;
                                        if (sum != 0)
                                            toPump = false;
                                        string refYearMonth = string.Format("{0}999{1}", this.BudgetYear, j.ToString());
                                        PumpIncomesPlanRow(planRow, sum, refYearMonth, ref updatedRowsCount, ref addedRowsCount);
                                    }
                                // год
                                if (toPump)
                                {
                                    string year = "SummaYear1";
                                    double sum = GetDoubleCellValue(planRow, year, 0) * sumFactor;
                                    string refYearMonth = string.Format("{0}0000", this.BudgetYear);
                                    string refYearMonthUNV = string.Format("{0}0001", this.BudgetYear);
                                    PumpIncomesPlanRow(planRow, sum, refYearMonth, refYearMonthUNV,
                                        ref updatedRowsCount, ref addedRowsCount);
                                }
                                // два последующих года - качаем всегда
                                for (int j = 2; j <= 3; j++)
                                {
                                    string year = "SummaYear" + j.ToString();
                                    double sum = GetDoubleCellValue(planRow, year, 0) * sumFactor;
                                    string refYearMonth = string.Format("{0}0000", this.BudgetYear + j - 1);
                                    string refYearMonthUNV = string.Format("{0}0001", this.BudgetYear + j - 1);
                                    PumpIncomesPlanRow(planRow, sum, refYearMonth, refYearMonthUNV,
                                        ref updatedRowsCount, ref addedRowsCount);
                                }
                            }
                        }
                        else
                        {
                            // Суммы по кварталам = Quarter1 ... Quarter4 и суммы по месяцам = Month1 ... Month12 
                            // все ракладываются по одной записи (на каждую сумму новая запись). 
                            // Нулевые суммы не раскладываются. И устанавливается конкретный год и месяц

                            if (incomesByMonths)
                            {
                                for (int j = 1; j <= 12; j++)
                                {
                                    string month = string.Format("MONTH{0}", j.ToString().PadLeft(2, '0'));
                                    double sum = GetDoubleCellValue(planRow, month, 0) * sumFactor;

                                    PumpIncomesPlanRow(planRow, sum,
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

                                    PumpIncomesPlanRow(planRow,
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
                        if (processedRecCount >= MAX_DS_RECORDS_AMOUNT)
						{
							UpdateFMIncomesPlanData();
                            if (this.PumpMode != BudgetDataPumpMode.Update)
                            {
                                ClearDataSet(ref dsIncomesPlan);
                                daIncomesPlan.Fill(dsIncomesPlan);
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
                UpdateFMIncomesPlanData();
			}
			finally
			{
                ClearDataSet(ref dsIncomesPlan);
                ClearDataSet(ref dsBudgetKESR);
                if (budgetKesrCache != null)
                    budgetKesrCache.Clear();
			}
		}
	}
}

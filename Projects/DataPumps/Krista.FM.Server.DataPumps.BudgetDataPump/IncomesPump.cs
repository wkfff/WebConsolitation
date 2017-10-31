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
	// Закачка данных доходов бюджета

    public partial class BudgetDataPumpModule : BudgetPumpModuleBase
	{
		private IDbDataAdapter daIncomes32;
        private DataSet dsIncomes32;
		private IDbDataAdapter daProgIndexIncomes;
		private DataSet dsProgIndexIncomes;
        private IDbDataAdapter daBudgetKESR;
        private DataSet dsBudgetKESR;

        private Dictionary<int, DataRow> budgetKesrCache = null;

        private List<string> incorrectFactRecords = new List<string>();


		/// <summary>
		/// Запрос данных из базы бюджета (incomes32)
		/// </summary>
		private int QueryBudgetIncomesData()
		{
            ClearDataSet(ref dsBudgetKESR);
            InitLocalDataAdapter(this.BudgetDB, ref daBudgetKESR, "select cast(id as varChar(10)) id, name from KESR");
            daBudgetKESR.Fill(dsBudgetKESR);

            FillRowsCache(ref budgetKesrCache, dsBudgetKESR.Tables[0], "ID");

            string incomesConstr = string.Format(
                "where i.idate is not null and i.budgetref = {0} {1}",
                this.BudgetRef, GetDateConstr("i."));
            CheckBudgetTableDate("Incomes32", "i.", "IDATE", incomesConstr);

            string constrByPumpParams = GetDateConstrByPumpParams("i.IDATE", true);
            if (constrByPumpParams == string.Empty)
                incomesConstr += string.Format(" and (i.IDATE between {0}0000 and {0}1232)", this.DataSource.Year);
            else
            {
                incomesConstr += " and " + constrByPumpParams;
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                    string.Format("Закачка выполняется с ограничением по дате: {0}..{1}", this.dateMin, this.dateMax));
            }
            incomesConstr += " and (i.REJECTCLS is null) and (i.ClsType = '2') ";

            string qsTransfert = string.Empty;
            string qsTransfertJoin = string.Empty;
            if (MajorDBVersion >= 38)
            {
                qsTransfert = " cast(tr.code as varchar(10)) TransfertCLS, ";
                qsTransfertJoin = " left join transfertcls tr on (i.transfertcls = tr.ID) ";
            }

            try
            {
                string query = string.Empty;
                if (MajorDBVersion >= 35)
                {
                    query = string.Format(
                        "select cast(i.ID as varchar(10)) Id, cast(i.IDATE as varChar(10)) iDate, cast(i.DEBIT as double precision) DEBIT, " +
                        "cast(i.CREDIT as double precision) CREDIT, " +
                        "cast(i.ACCOUNTINDEX as varChar(10)) ACCOUNTINDEX, cast(i.ACCOUNTREF as varChar(10)) ACCOUNTREF, " +
                        "cast(i.KD as varChar(10)) kd, cast(i.REGIONCLS as varChar(10)) regionCls, " +
                        "cast(i.INCACCOUNT as varChar(10)) incAccount, cast(i.PROGINDEX as varChar(10)) progIndex, " +
                        "cast(i.RECORDINDEX as varChar(10)) recordIndex, cast(i.BUDGETREF as varChar(10)) budgetRef, " +
                        "cast(i.MEANSTYPE as varChar(10)) meansType, cast(i.KVSR as varChar(10)) kvsr, cast(i.KFSR as varChar(10)) kfsr, " +
                        "cast(i.KESR as varChar(10)) kesr, cast(i.KVR as varChar(10)) kvr, cast(i.KCSR as varChar(10)) kcsr, " +
                        qsTransfert +
                        "cast(i.ELDOCREF as varChar(10)) eldocRef, cast(i.FEDERALFUNDSREF as varChar(10)) federalFundsRef, " +
                        "cast(i.FundsSource as varChar(10)) fundsSource, cast(i.directioncls as varChar(10)) directionCls, " +
                        "cast(i.BudgetRef as varChar(10)) budgetRef, cast(i.BuhOperationCLS as varChar(10)) buhOperationCls, " +
                        "cast(i.FACIALACC_CLS as varChar(10)) facialAcc_Cls, b.Name BudgetBudgetName, cast(b.AYear as varChar(10)) BudgetBudgetYear " +
                        "from incomes32 i left join Budgets_s b on (i.BudgetRef = b.ID) " +
                        qsTransfertJoin + " {0}", incomesConstr);
                }
                WriteToTrace("Запрос Доходы: " + query, TraceMessageKind.Information);
                InitLocalDataAdapter(this.BudgetDB, ref daBudgetFacts, query);
                daBudgetFacts.Fill(dsBudgetFacts);
            }
            catch
            {
                WriteToTrace("SQL Query: " + daBudgetFacts.SelectCommand.CommandText, TraceMessageKind.Error);
                throw;
            }

            return dsBudgetFacts.Tables["Table"].Rows.Count;
        }

        #region запрос к казначейству


        private string GetBudgetIncomesTreasuryConstr()
        {
            string constrByPumpParams = string.Empty;
            constrByPumpParams = GetDateConstrByPumpParams("d.ACCEPTDATE", true);
            if (constrByPumpParams == string.Empty)
                constrByPumpParams = string.Format("(d.acceptdate between {0}0000 and {0}1232) ", this.DataSource.Year);
            else
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                    string.Format("Закачка выполняется с ограничением по дате: {0}..{1}", this.dateMin, this.dateMax));
            if (this.PumpMode == BudgetDataPumpMode.Update)
                constrByPumpParams += GetDateConstr("d.");
            constrByPumpParams = string.Concat(" where ", constrByPumpParams);
            return constrByPumpParams;
        }

        /// <summary>
        /// Запрос данных из базы бюджета (facialFinDetail)
        /// </summary>
        private int QueryBudgetIncomesFacialFinDetailData()
        {
            // получаем данные мастера
            QueryFacialFinCaptionCount();

            ClearDataSet(ref dsBudgetKESR);
            InitLocalDataAdapter(this.BudgetDB, ref daBudgetKESR, "select cast(id as varChar(10)) id, name from KESR");
            daBudgetKESR.Fill(dsBudgetKESR);
            FillRowsCache(ref budgetKesrCache, dsBudgetKESR.Tables[0], "ID");

            string incomesConstr = string.Format("{0} and {1}", GetBudgetIncomesTreasuryConstr(), "d.clsType = '2'");
            if (tDetailTableName != "CorrectionDetail")
                CheckBudgetTableDate(tDetailTableName, "d.", "ACCEPTDATE", incomesConstr);

            try
            {
                if (MajorDBVersion < 35)
                {
                    InitLocalDataAdapter(this.BudgetDB, ref daBudgetFacts, "select * from facialFinDetail where 1 = 0");
                    daBudgetFacts.Fill(dsBudgetFacts);
                }
                else
                {
                    string qsTransfert = string.Empty;
                    string qsTransfertTreasury = string.Empty;
                    string qsTransfertJoin = string.Empty;
                    string qsTransfertTreasuryJoin = string.Empty;
                    if (MajorDBVersion >= 38)
                    {
                        qsTransfert = " cast(tr.code as varchar(10)) TransfertCLS, ";
                        qsTransfertTreasury = " cast(tr1.code as varchar(10)) DESTTransfertCLS, cast(tr2.code as varchar(10)) SourceTransfertCLS, ";
                        qsTransfertJoin = " left join transfertcls tr on (d.TransfertCLS = tr.ID) ";
                        qsTransfertTreasuryJoin = " left join transfertcls tr1 on (d.DESTTransfertCLS = tr1.ID) " +
                                                  " left join transfertcls tr2 on (d.SourceTransfertCLS = tr2.ID) ";
                    }

                    string query = string.Empty;
                    if (tDetailTableName == "FacialFinDetail")
                    {
                        query = string.Format(
                            "select cast(d.ID as varchar(10)) id, cast(d.AcceptDate as varchar(10)) IDATE, cast(d.CREDIT as double precision) CREDIT, " +
                            "cast(d.DestKd as varChar(10)) destKd, cast(d.SourceKd as varchar(10)) SourceKd, cast(d.DestREGIONCLS as varchar(10)) destRegionCls, " +
                            "cast(d.SourceREGIONCLS as varchar(10)) sourceRegionCls, cast(d.progindex as varchar(10)) progIndex, " +
                            "cast(d.recordIndex as varchar(10)) recordIndex, cast(d.budgetRef as varchar(10)) budgetRef, " +
                            "cast(d.DestMEANSTYPE as varchar(10)) destMeansType, cast(d.SourceMEANSTYPE as varchar(10)) sourceMeansType, " +
                            "cast(d.DestKVSR as varchar(10)) destKvsr, cast(d.DestKFSR as varchar(10)) destKFSR, " +
                            "cast(d.DestKESR as varchar(10)) destKESR, cast(d.DestKVR as varchar(10)) destKVR, " +
                            "cast(d.DestKCSR as varchar(10)) destKCSR, cast(d.SOURCEFACIALACC_CLS as varchar(10)) SOURCEFACIALACC_CLS, " +
                            "cast(d.DESTFACIALACC_CLS as varchar(10)) DESTFACIALACC_CLS, cast(d.SourceKVSR as varchar(10)) sourceKVSR, " +
                            "cast(d.SourceKFSR as varchar(10)) sourceKFSR, cast(d.SourceKESR as varchar(10)) sourceKESR, " +
                            "cast(d.SourceKVR as varchar(10)) sourceKVR, cast(d.SourceKCSR as varchar(10)) sourceKCSR, " +
                            "cast(d.DestFundsSource as varchar(10)) DestFundsSource, cast(d.SourceFundsSource as varchar(10)) SourceFundsSource, " +
                            qsTransfertTreasury +
                            "cast(d.DestDirectioncls as varchar(10)) DestDirectioncls, cast(d.SourceDirectioncls as varchar(10)) SourceDirectioncls, " +
                            "cast(d.BuhOperationCLS as varchar(10)) BuhOperationCLS, cast(f.ELDOCREF as varchar(10)) ELDOCREF, " +
                            "cast(f.FEDERALFUNDSREF as varchar(10)) FEDERALFUNDSREF, cast(f.OperationDirection as varchar(10)) OperationDirection, " +
                            "b.Name BudgetBudgetName, cast(b.AYear as varchar(10)) BudgetBudgetYear " +
                            "from FACIALFINDETAIL d " +
                            "left join Budgets_s b on (d.BudgetRef = b.ID) " +
                            qsTransfertTreasuryJoin + 
                            "left join FacialFinCaption f on (d.recordindex = f.ID) {0}", incomesConstr);
                        WriteToTrace("Запрос Доходы Казначейство: " + query, TraceMessageKind.Information);
                    }
                    else
                    {
                        // коррекция
                        query = string.Format(
                            "select cast(d.ID as varchar(10)) id, cast(d.Summa as double precision) CREDIT, " +
                            "cast(d.Kd as varChar(10)) Kd, cast(d.REGIONCLS as varchar(10)) RegionCls, " +
                            "cast(d.progindex as varchar(10)) progIndex, " +
                            "cast(d.recordIndex as varchar(10)) recordIndex, cast(d.budgetRef as varchar(10)) budgetRef, " +
                            "cast(d.MEANSTYPE as varchar(10)) MeansType, " +
                            "cast(d.KVSR as varchar(10)) Kvsr, cast(d.KFSR as varchar(10)) KFSR, " +
                            "cast(d.KESR as varchar(10)) KESR, cast(d.KVR as varchar(10)) KVR, " +
                            "cast(d.KCSR as varchar(10)) KCSR, " +
                            "cast(d.FACIALACC_CLS as varchar(10)) FACIALACC_CLS, " +
                            "cast(d.FundsSource as varchar(10)) FundsSource, " +
                            qsTransfert +
                            "cast(d.Directioncls as varchar(10)) Directioncls, " +
                            "cast(d.BuhOperationCLS as varchar(10)) BuhOperationCLS, cast(f.ELDOCREF as varchar(10)) ELDOCREF, " +
                            "cast(f.FEDERALFUNDSREF as varchar(10)) FEDERALFUNDSREF, cast(f.OperationDirection as varchar(10)) OperationDirection, " +
                            "b.Name BudgetBudgetName, cast(b.AYear as varchar(10)) BudgetBudgetYear " +
                            "from CorrectionDetail d " +
                            "left join Budgets_s b on (d.BudgetRef = b.ID) " +
                             qsTransfertJoin +
                            "left join FacialFinCaption f on (d.recordindex = f.ID) where d.clsType = '2' {0} ", 
                            " and (d.DocumentRecordIndex is null or d.DocumentRecordIndex = 0) ");
                        WriteToTrace("Запрос Доходы Уточнения: " + query, TraceMessageKind.Information);
                    }

                    InitLocalDataAdapter(this.BudgetDB, ref daBudgetFacts, query);
                    daBudgetFacts.Fill(dsBudgetFacts);
                }
            }
            catch
            {
                WriteToTrace("SQL Query: " + daBudgetFacts.SelectCommand.CommandText, TraceMessageKind.Error);
                throw;
            }
            return dsBudgetFacts.Tables["Table"].Rows.Count;
        }

        #endregion запрос к казначейству

        /// <summary>
		/// Запрос наших данных
		/// </summary>
		private void QueryFMIncomesData()
		{
            ClearDataSet(ref dsIncomes32);
            ClearDataSet(ref dsProgIndexIncomes);

            if (this.PumpMode == BudgetDataPumpMode.Update)
            {
                InitDataSet(ref daIncomes32, ref dsIncomes32, fctIncomes32, false,
                    string.Format("SOURCEID = {0}", this.SourceID), string.Empty);

                FillRowsCache(ref factCache, dsIncomes32.Tables[0], new string[] { "SOURCEKEY" });
            }
            else
            {
                InitFactDataSet(ref daIncomes32, ref dsIncomes32, fctIncomes32);
            }

            InitDataSet(ref daProgIndexIncomes, ref dsProgIndexIncomes, fxcProgIndexIncomes, true, string.Empty, string.Empty);
		}

		/// <summary>
		/// Сбрасывает закачанные данные в базу
		/// </summary>
		private void UpdateFMIncomesData()
		{
            try
            {
                UpdateData();

                UpdateDataSet(daIncomes32, dsIncomes32, fctIncomes32);
            }
            catch
            {
                throw;
            }
		}

        /// <summary>
        /// Закачивает строку факта доходов
        /// </summary>
        /// <param name="incomesRow">Строка Incomes32</param>
        /// <param name="kdCodeField">Наименование поля кода КД</param>
        /// <param name="errIncomesCode">Код ошибочных доходов</param>
        /// <param name="addedRowsCount">Счетчик добавленных записей</param>
        /// <param name="updatedRowsCount">Счетчик обновленных записей</param>
        private void PumpIncomesRow(DataRow incomesRow, string kdCodeField, string errIncomesCode, ref int addedRowsCount, 
            ref int updatedRowsCount, ClassTypes objClassType, string generatorName, string clsPrefix, string sumFieldName, FacialFinCaptionRow captionRow)
        {
            DataRow kdRowData = FindCachedRow(kdCache, Convert.ToInt32(incomesRow[clsPrefix + "KD"]));
            if (kdRowData == null)
            {
                string factID = incomesRow["ID"].ToString();
                string kdID = incomesRow[clsPrefix + "KD"].ToString();
                incorrectFactRecords.Add(string.Format("{0} (ссылка на кд: {1})", factID, kdID));
                return;
            }

            if ((tDetailTableName == "FacialFinDetail") || (tDetailTableName == "CorrectionDetail"))
            {
                // не качаем служебные лицевые счета
                DataRow facialRow = FindCachedRow(facialAccCache, Convert.ToInt32(incomesRow[clsPrefix + "FACIALACC_CLS"]));
                if (facialRow == null)
                    return;
                if (Convert.ToInt32(facialRow["ServiceAccount"]) == 0)
                    return;
                // не качаем временный тип средств
                if (IsTempMeansType(incomesRow, clsPrefix))
                    return;
            }


            bool isAdded;

            DataRow row = GetRowForUpdate(Convert.ToInt32(incomesRow["ID"]),
                dsIncomes32.Tables[0], factCache, objClassType, generatorName, out isAdded);
            if (isAdded)
            {
                addedRowsCount++;
            }
            else
            {
                updatedRowsCount++;
            }

            row["SOURCEKEY"] = incomesRow["ID"];
            if (sumFieldName == string.Empty)
            {
                row["DEBIT"] = incomesRow["DEBIT"];
                row["CREDIT"] = incomesRow["CREDIT"];
            }
            else
            {
                row["DEBIT"] = 0;
                row["CREDIT"] = 0;
                decimal sum = Convert.ToDecimal(incomesRow["CREDIT"].ToString().PadLeft(1, '0'));
                if (tDetailTableName == "CorrectionDetail")
                {
                    if (incomesRow["ProgIndex"].ToString() == "315")
                        sum *= -1;
                }
                row[sumFieldName] = sum;
            }
            row["PROGINDEX"] = incomesRow["PROGINDEX"];

            if (tDetailTableName == "CorrectionDetail")
                row["RefYearDayUNV"] = captionRow.AcceptDate;
            else
                row["RefYearDayUNV"] = incomesRow["IDATE"].ToString().Split('.')[0];

            row["RefKDASBudget"] = nullKD;
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

                    row["REFFEDERALFUNDS"] = -1;

                    //  Способ получения данных о доходах = 1, если заполнены поля ELNUM или 
                    // ELPAKETNUM (пришли из банка) 2 противном случае (введены вручную)
                    if (!incomesRow.IsNull("ELNUM") || !incomesRow.IsNull("ELPACKETNUM"))
                    {
                        row["REFISELDOC"] = 1;
                    }
                    else
                    {
                        row["REFISELDOC"] = 2;
                    }


                    // Виды доходов
                    row["REFINCOMESKINDS"] = nullIncomesKinds;

                    // ЭКД
                    row["REFEKD"] = nullEKD;

                    // Программы
                    row["REFPROGRAMS"] = nullPrograms;

                    // ЭКР 
                    row["RefEKRASBudget"] = nullEKR;

                    // ФКР
                    row["RefFKRASBudget"] = nullFKR;

                    // Целевые статьи
                    row["RefKCSRASBudget"] = nullKCSR;

                    // Виды расходов
                    row["REFKVR"] = nullKVR;

                    break;

                default:
                    // Федеральные средства = 1 если пусто и 2 если это поле чем-то заполнено
                    if (this.CurrentDBVersion == "31.00")
                    {
                        row["REFFEDERALFUNDS"] = -1;
                    }
                    else
                    {
                        if (incomesRow.IsNull("FEDERALFUNDSREF"))
                        {
                            row["REFFEDERALFUNDS"] = 1;
                        }
                        else
                        {
                            row["REFFEDERALFUNDS"] = 2;
                        }
                    }

                    // Способ получения данных о доходах = 1 если ELDOCREF is not null (пришли из банка), 
                    // 2 противном случае (введены вручную)
                    if (!incomesRow.IsNull("ELDOCREF"))
                    {
                        row["REFISELDOC"] = 1;
                    }
                    else
                    {
                        row["REFISELDOC"] = 2;
                    }

                    // Виды доходов
                    if (kdRowData == null)
                        row["REFINCOMESKINDS"] = nullIncomesKinds;
                    else
                        row["REFINCOMESKINDS"] = FindCachedRowID(incomesKindsCache, new string[] { 
                        Convert.ToString(kdRowData["DESCRIPTIONCODE"]), Convert.ToString(kdRowData["NAME"]) },
                            nullIncomesKinds);

                    // ЭКР
                    DataRow kesrRow = FindCachedRow(budgetKesrCache, Convert.ToInt32(incomesRow[clsPrefix + "KESR"]));
                    row["RefEKRASBudget"] = PumpOriginalRow(dsEKR, clsEKR,
                        new object[] { "CODE", kesrRow["ID"], "NAME", kesrRow["NAME"] });

                    // ЭКД
                    if (kdRowData == null)
                        row["REFEKD"] = nullEKD;
                    else
                    {
                        kesrRow = FindCachedRow(budgetKesrCache, Convert.ToInt32(kdRowData["KESR"]));
                        row["REFEKD"] = PumpOriginalRow(dsEKD, clsEKD,
                            new object[] { "CODE", kesrRow["ID"], "NAME", kesrRow["NAME"] });
                    }

                    // Программы
                    if (kdRowData == null)
                        row["REFPROGRAMS"] = nullPrograms;
                    else
                        row["REFPROGRAMS"] = kdRowData["PROGRAMCODE"];

                    // ФКР
                    row["RefFKRASBudget"] = FindCachedRowID(fkrCache, Convert.ToInt32(incomesRow[clsPrefix + "KFSR"]), nullFKR);

                    // Целевые статьи
                    row["RefKCSRASBudget"] = FindCachedRowID(kcsrCache, Convert.ToInt32(incomesRow[clsPrefix + "KCSR"]), nullKCSR);

                    // Виды расходов
                    row["REFKVR"] = FindCachedRowID(kvrCache, Convert.ToInt32(incomesRow[clsPrefix + "KVR"]), nullKVR);

                    break;
            }

            // Проверяем значение кода дохода на соответствие константе Бюджета "Код ошибочных доходов". 
            // Если код дохода наш оттуда, то ставим блок доходов "Ошибочные".
            if (errIncomesCode != string.Empty && (kdRowData != null) && 
                Convert.ToString(kdRowData[kdCodeField]).EndsWith(errIncomesCode))
            {
                row["REFPROGINDEXINCOMES"] = 1002;
            }
            else
            {
                // Блок доходов определяется по ProgIndex
                DataRow[] rows = dsProgIndexIncomes.Tables[0].Select(string.Format("PROGINDEX = '{0}'",
                    Convert.ToString(incomesRow["PROGINDEX"])));

                // Если нашли - ставим ссылку, eсли не нашли - ставим код "Прочие".
                if (rows.GetLength(0) > 0)
                {
                    row["REFPROGINDEXINCOMES"] = rows[0]["ID"];
                }
                else
                {
                    row["REFPROGINDEXINCOMES"] = 1001;
                }
            }

            // Администратор доходов
            row["REFKVSRINCOME"] = GetIntCellValue(kdRowData, "KVSR", nullKVSR);

            // Районы
            row["REFREGIONS"] = FindRowID(dsRegions.Tables[0], string.Format("SOURCEKEY = {0}", incomesRow[clsPrefix + "REGIONCLS"]), nullRegions);

            // Слои доходов
            if (tDetailTableName == "Incomes32")
                row["REFBUDGETACCOUNTS"] = FindRowID(dsBudgetAccounts.Tables[0], string.Format(
                    "SOURCEKEY = {0}", incomesRow["ACCOUNTINDEX"]), nullBudgetAccounts);
            else
                row["REFBUDGETACCOUNTS"] = nullBudgetAccounts;

            // Тип средств
            row["REFMEANSTYPE"] = FindCachedRowID(meansTypeCache, Convert.ToInt32(incomesRow[clsPrefix + "MEANSTYPE"]), nullMeansType);

            // лицевые счета
            row["RefFacialAcc"] = FindCachedRowID(facialAccCache, Convert.ToInt32(incomesRow[clsPrefix + "FACIALACC_CLS"]), nullFacialAcc);

            if (MajorDBVersion >= 35)
            {
                row["RefFundsSource"] = FindCachedRowID(fundsSourceCache, Convert.ToInt32(incomesRow[clsPrefix + "FundsSource"]), nullFundsSource);
                row["RefDirection"] = FindCachedRowID(directionCache, Convert.ToInt32(incomesRow[clsPrefix + "directioncls"]), nullDirection);
                row["RefBudget"] = FindBudgetBudgetRef(incomesRow["BudgetBudgetName"].ToString(),
                    incomesRow["BudgetBudgetYear"].ToString());
                row["RefBuhOperations"] = FindCachedRowID(buhOperationsCache, Convert.ToInt32(incomesRow["BuhOperationCLS"]), nullBuhOperations);
            }

            string transfert = "-1";
            if (MajorDBVersion >= 38)
            {
                transfert = incomesRow[clsPrefix + "TransfertCLS"].ToString();
                if (transfert == string.Empty)
                    transfert = "-1";
            }
            row["RefTransf"] = FindCachedRowID(transfertCache, Convert.ToInt32(transfert), nullTransfert);
        }

		/// <summary>
		/// Закачивает данные из бюджетной базы в нашу
		/// </summary>
        private void PumpIncomes(ref int addedRowsCount, ref int updatedRowsCount, ref int processedRecCount,
            ref string skippedRecsMessage)
		{
			int zeroDateCount = 0;
            DataRow incomesRow = null;
			try
			{
                string kdCodeField = "CODESTR";
                if (this.BudgetYear <= 2004)
                {
                    kdCodeField = "CODE";
                }

                // Код ошибочных доходов
                string errIncomesCode = GetBudgetConst(BudgetConst.ErroneousIncomesCode, string.Empty);

                int totalRecs = dsBudgetFacts.Tables["Table"].Rows.Count;
				// Обработка полученных данных
                ClassTypes objClassType = fctIncomes32.ClassType;
                string generatorName = fctIncomes32.GeneratorName;
                for (int i = 0; i < totalRecs; i++)
				{
                    processedRecCount++;

					incomesRow = dsBudgetFacts.Tables["Table"].Rows[i];

                    if (tDetailTableName != "CorrectionDetail")
                    {
                        int date = Convert.ToInt32(incomesRow["IDATE"].ToString().Split('.')[0]);
                        // Проверка даты
                        if (!CheckDate(date))
                        {
                            continue;
                        }
                        if (date % 10000 == 0)
                        {
                            zeroDateCount++;
                            continue;
                        }
                    }

					try
					{
                        if (tDetailTableName == "Incomes32")
                        {
                            // качаем из incomes32
                            PumpIncomesRow(incomesRow, kdCodeField, errIncomesCode,
                                ref addedRowsCount, ref updatedRowsCount, objClassType, generatorName, string.Empty, string.Empty, null);
                        }
                        else
                        {
                            // качаем из facialFinDetail или CorrectionDetail
                            int recordIndex = Convert.ToInt32(incomesRow["RECORDINDEX"]);
                            if (!captionAcceptedRecords.ContainsKey(recordIndex))
                                continue;
                            FacialFinCaptionRow captionRow = captionAcceptedRecords[recordIndex];
                            if (tDetailTableName == "FacialFinDetail")
                            {
                                if (captionRow.DestType == 1)
                                {
                                    // кредит
                                    PumpIncomesRow(incomesRow, kdCodeField, errIncomesCode,
                                        ref addedRowsCount, ref updatedRowsCount, objClassType, generatorName, "Dest", "Credit", captionRow);
                                }
                                if (captionRow.SourceType == -1)
                                {
                                    // дебет
                                    PumpIncomesRow(incomesRow, kdCodeField, errIncomesCode,
                                        ref addedRowsCount, ref updatedRowsCount, objClassType, generatorName, "Source", "Debit", captionRow);
                                }
                            }
                            else
                            {
                                // коррекция
                                if (captionRow.ProgIndex == 312)
                                {
                                    // кредит
                                    PumpIncomesRow(incomesRow, kdCodeField, errIncomesCode,
                                        ref addedRowsCount, ref updatedRowsCount, objClassType, generatorName, string.Empty, "Credit", captionRow);
                                }
                                if (captionRow.ProgIndex == 313)
                                {
                                    // дебет
                                    PumpIncomesRow(incomesRow, kdCodeField, errIncomesCode,
                                        ref addedRowsCount, ref updatedRowsCount, objClassType, generatorName, string.Empty, "Debit", captionRow);
                                }
                            }
                        }

						SetProgress(totalRecs, i + 1, string.Format(
                                "Обработка данных базы {0} ({1} из {2})...", 
                                this.DatabasePath, filesCount, totalFiles),
                            string.Format("{0} Запись {1} из {2}", currentBlockName, i + 1, totalRecs));

                        // Если накопилось много записей, то сбрасываем в базу
						if (processedRecCount >= MAX_DS_RECORDS_AMOUNT)
						{
							UpdateFMIncomesData();
                            if (this.PumpMode != BudgetDataPumpMode.Update)
                            {
                                ClearDataSet(ref dsIncomes32);
                                daIncomes32.Fill(dsIncomes32);
                            }
                            processedRecCount = 0;
						}
					}
					catch (Exception ex)
					{
                        WriteToTrace(string.Format("СТРОКА {0} - {1}", incomesRow["ID"], ex), TraceMessageKind.Error);
						throw new PumpDataFailedException(string.Format("СТРОКА {0} - {1}", incomesRow["ID"], ex.Message));
					}
				}

                UpdateFMIncomesData();

                if (incorrectFactRecords.Count != 0)
                {
                    string message = string.Format("{0} {1} Список ID записей: {2}",
                        "В таблице фактов Incomes32 найдены записи, ссылающиеся на некорректные KD (объединяющий код: UnionFlag = 1), ",
                        "данные по этим записям закачаны не будут. ",
                        string.Join(", ", incorrectFactRecords.ToArray()));
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, message);
                }
			}
			finally
			{
                ClearDataSet(ref dsIncomes32);
                ClearDataSet(ref dsProgIndexIncomes);
                ClearDataSet(ref dsBudgetKESR);
                if (budgetKesrCache != null)
                    budgetKesrCache.Clear();
                incorrectFactRecords.Clear();
                if (captionAcceptedRecords != null)
                    captionAcceptedRecords.Clear();
			}
		}
	}
}
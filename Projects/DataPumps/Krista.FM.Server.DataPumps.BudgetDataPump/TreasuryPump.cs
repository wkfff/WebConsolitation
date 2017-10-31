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
    // Закачка данных казначейства

    public partial class BudgetDataPumpModule : BudgetPumpModuleBase
    {
        private IDbDataAdapter daFacialFinDetail;
        private DataSet dsFacialFinDetail;
        private IDbDataAdapter daProgIndexFacial;
        private DataSet dsProgIndexFacial;
        string treasuryConstr = string.Empty;

        // Список ИД записей заголовка, удовлетворяющих всем условиям
        Dictionary<int, FacialFinCaptionRow> captionAcceptedRecords;

        double nullFkrSum = 0;
        double sumForCheckDebit = 0;
        double sumForCheckReturnDebit = 0;

        string tCaptionTableName = string.Empty;
        string tDetailTableName = string.Empty;

        bool toShowIncorrectDate = true;

        bool isTreasury = false;

        /// <summary>
        /// Строка заголовка казначейства с необходимыми данными
        /// </summary>
        private sealed class FacialFinCaptionRow
        {
            public int AcceptDate;
            public int ProgIndex;
            public int DestType;
            public int SourceType;

            public FacialFinCaptionRow(int acceptDate, int progIndex, int destType, int sourceType)
            {
                this.AcceptDate = acceptDate;
                this.ProgIndex = progIndex;
                this.DestType = destType;
                this.SourceType = sourceType;
            }
        }

        /// <summary>
        /// Запрос данных из базы бюджета
        /// </summary>
        private void QueryBudgetTreasuryData(string constr)
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
            string qsTransfert = string.Empty;
            string qsTransfertTreasury = string.Empty;
            string qsTransfertJoin = string.Empty;
            string qsTransfertTreasuryJoin = string.Empty;
            if (MajorDBVersion >= 38)
            {
                otQueryString = "cast(d.OperationType as varchar(10)) OperationType, ";
                qsTransfert = "cast(tr.code as varchar(10)) TransfertCLS, ";
                qsTransfertTreasury = "cast(tr1.code as varchar(10)) DESTTransfertCLS, cast(tr2.code as varchar(10)) SourceTransfertCLS, ";
                qsTransfertJoin = " left join transfertcls tr on (d.transfertcls = tr.ID) ";
                qsTransfertTreasuryJoin = " left join transfertcls tr1 on (d.DESTTransfertCLS = tr1.ID) " +
                                          " left join transfertcls tr2 on (d.SourceTransfertCLS = tr2.ID) ";
            }

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
                        "cast(d.DIRECTIONCLS as varchar(10)) DIRECTIONCLS, " +
                        "cast(d.RECORDINDEX as varchar(10)) RECORDINDEX, " +
                        qsTransfert +
                        otQueryString +
                        "cast(d.BudgetRef as varchar(10)) BudgetRef, cast(d.RegionCLS as varchar(10)) RegionCLS, " +
                        "cast(d.FundsSource as varchar(10)) FundsSource, " +
                        "b.Name BudgetBudgetName, cast(b.AYear as varchar(10)) BudgetBudgetYear " +
                        "from CorrectionDetail d left join Budgets_s b on (d.BudgetRef = b.ID) " +
                        qsTransfertJoin + " {0} " +
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
                        qsTransfertTreasury +
                        "cast(d.DESTKVSR as varchar(10)) DESTKVSR, cast(d.DESTKCSR as varchar(10)) DESTKCSR, " +
                        "cast(d.DESTKFSR as varchar(10)) DESTKFSR, cast(d.DESTSUBKESR as varchar(10)) DESTSUBKESR, " +
                        "cast(d.DESTFACT as varchar(10)) DESTFACT, cast(d.DESTMEANSTYPE as varchar(10)) DESTMEANSTYPE, " +
                        "cast(d.DESTFINSOURCE as varchar(10)) DESTFINSOURCE, cast(d.DESTFINTYPE as varchar(10)) DESTFINTYPE, " +
                        "cast(d.SOURCEDIRECTIONCLS as varchar(10)) SOURCEDIRECTIONCLS, cast(d.DestFundsSource as varchar(10)) DestFundsSource, " +
                        "cast(d.DESTDIRECTIONCLS as varchar(10)) DESTDIRECTIONCLS, cast(d.RECORDINDEX as varchar(10)) RECORDINDEX, " +
                        "cast(d.BudgetRef as varchar(10)) BudgetRef, cast(d.SourceRegionCLS as varchar(10)) SourceRegionCLS, " +
                        "cast(d.DestRegionCLS as varchar(10)) DestRegionCLS, cast(d.SourceFundsSource as varchar(10)) SourceFundsSource, " +
                        "b.Name BudgetBudgetName, cast(b.AYear as varchar(10)) BudgetBudgetYear " +
                        "from FACIALFINDETAIL d left join Budgets_s b on (d.BudgetRef = b.ID) " +
                        qsTransfertTreasuryJoin + " {0} ", constr);
                }
            }
            InitLocalDataAdapter(this.BudgetDB, ref daBudgetFacts, query);
            daBudgetFacts.Fill(dsBudgetFacts);
        }

        private string GetBudgetTreasuryConstr()
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
            constrByPumpParams += " (d.clsType = '0') ";

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
        private int QueryBudgetTreasuryDataCount()
        {
            string idConstr = "select count(d.id) from " + tDetailTableName + " d ";
            idConstr += GetBudgetTreasuryConstr() + " and (d.clsType = '0') ";
            return Convert.ToInt32(this.BudgetDB.ExecQuery(idConstr, QueryResultTypes.Scalar));
        }

        /// <summary>
        /// Запрос данных о количестве записей казначейства из базы бюджета (мастер)
        /// </summary>
        private int QueryFacialFinCaptionCount()
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
        private void QueryFMTreasuryData()
        {
            ClearDataSet(ref dsFacialFinDetail);
            InitFactDataSet(ref daFacialFinDetail, ref dsFacialFinDetail, fctFacialFinDetail);
            ClearDataSet(ref dsProgIndexFacial);
            InitDataSet(ref daProgIndexFacial, ref dsProgIndexFacial, fxcProgIndexFacial, true, string.Empty, string.Empty);
        }

        /// <summary>
        /// Сбрасывает закачанные данные в базу
        /// </summary>
        private void UpdateFMTreasuryData()
        {
            try
            {
                UpdateData();

                UpdateDataSet(daFacialFinDetail, dsFacialFinDetail, fctFacialFinDetail);
            }
            catch
            {
                throw;
            }
        }

        private void ShowIncorrectDate()
        {
            string query = string.Format("select distinct acceptDate from {0} c where not (c.ACCEPTDATE between {1}0000 and {1}1232) ",
                tCaptionTableName, this.DataSource.Year);
            DataTable dt = (DataTable)this.BudgetDB.ExecQuery(query, QueryResultTypes.DataTable);
            string dates = string.Empty;
            foreach (DataRow row in dt.Rows)
                dates += string.Format("{0}, ", row["acceptDate"].ToString());
            if (dates != string.Empty)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Найдены записи с датой принятия, отличной от закачиваемого года, эти записи закачаны не были. Кол-во дат: {0}. Список дат: {1}",
                                  dt.Rows.Count, dates));
                toShowIncorrectDate = false;
            }
        }

        /// <summary>
        /// Заполняет список ИД записей заголовка, удовлетворяющих всем условиям
        /// </summary>
        private void FillCaptionAcceptedRecords(string constr)
        {
            // получаем кол-во записей 
            string query = string.Format("select count(c.ID) from {0} c {1}", 
                tCaptionTableName, constr);
            int recCnt = Convert.ToInt32(this.BudgetDB.ExecQuery(query, QueryResultTypes.Scalar));

            // инициализируем коллекцию captionNotAcceptedRecords здесь,
            // в соответствии с фактическим количеством значений
            if (captionAcceptedRecords != null) 
                captionAcceptedRecords.Clear();

            captionAcceptedRecords = new Dictionary<int, FacialFinCaptionRow>(recCnt);
            
            if (recCnt == 0)
                return; 

            // выводим предупреждение о записях с некорректной датой
            if (toShowIncorrectDate)
                ShowIncorrectDate();

            // получаем датаридер
            IDbCommand cmd = this.BudgetDB.Connection.CreateCommand();
            query = string.Format(
                "select cast(c.ID as varchar(10)) id, cast(c.ACCEPTDATE as varchar(10)) ACCEPTDATE, " +
                    "cast(c.PROGINDEX as varchar(10)) PROGINDEX, " +
                    "cast(c.DESTTYPE as varchar(10)) DESTTYPE, cast(c.SOURCETYPE as varchar(10)) SOURCETYPE " +
                "from {0} c {1}", tCaptionTableName, constr);
            WriteToTrace("Запрос Казначейство (мастер): " + query, TraceMessageKind.Information);
            cmd.CommandText = query;

            IDataReader rdr = null;
            try
            {
                rdr = cmd.ExecuteReader();
                // теперь инициализируем данные в кэше
                object[] values = new object[5];
                while (rdr.Read())
                {
                    rdr.GetValues(values);
                    int id = Convert.ToInt32(values[0]);// ID
                    if (!captionAcceptedRecords.ContainsKey(id))
                    {
                        captionAcceptedRecords.Add(id,
                            new FacialFinCaptionRow(
                                Convert.ToInt32(values[1].ToString().Replace(".", string.Empty)), // AcceptDate
                                Convert.ToInt32(values[2]), // ProgIndex
                                Convert.ToInt32(values[3]), // DestType
                                Convert.ToInt32(values[4])  // SourceType
                         ));
                    }
                }
            }
            finally
            {
                if (rdr != null) rdr.Close();
                if (cmd != null) cmd.Dispose();
            }
        }

        /// <summary>
        /// Устанавливает поля сумм строки
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="sum">Сумма</param>
        /// <param name="recordType">Тип записи</param>
        private static void SetSumFields(DataRow factRow, double sum, int recordType)
        {
            switch (recordType)
            {
                // Расход
                case 1:
                    factRow["DEBIT"] = sum;
                    factRow["RETURNDEBIT"] = 0;
                  //  factRow["CREDIT"] = 0;
                   // factRow["RETURNCREDIT"] = 0;
                    break;
                // Возврат расхода
                case 2:
                    factRow["DEBIT"] = 0;
                    factRow["RETURNDEBIT"] = sum;
                  //  factRow["CREDIT"] = 0;
                   // factRow["RETURNCREDIT"] = 0;
                    break;
                // Приход
                case 3:
                    factRow["DEBIT"] = 0;
                    factRow["RETURNDEBIT"] = 0;
                    factRow["CREDIT"] = sum;
                    factRow["RETURNCREDIT"] = 0;
                    break;
                // Возврат прихода
                case 4:
                    factRow["DEBIT"] = 0;
                    factRow["RETURNDEBIT"] = 0;
                    factRow["CREDIT"] = 0;
                    factRow["RETURNCREDIT"] = sum;
                    break;
            }
        }

        private string GetClsPrefix(int recordType)
        {
            if (tCaptionTableName == "CorrectionCaption")
                return string.Empty;
            string clsPrefix = string.Empty;
            switch (recordType)
            {
                case 1:
                case 4:
                    clsPrefix = "SOURCE";
                    break;
                case 2:
                case 3:
                    clsPrefix = "DEST";
                    break;
            }
            return clsPrefix;
        }

        /// <summary>
        /// Устанавливает поля классификаторов строки
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="recordType">Тип записи</param>
        private void SetClsFields(DataRow factRow, DataRow treasuryRow, int recordType)
        {
            string clsPrefix = GetClsPrefix(recordType);

            SetOrgFields(treasuryRow, factRow, string.Format("{0}FACIALACC_CLS", clsPrefix));

            // Вид расходов
            factRow["REFKVR"] = FindCachedRowID(kvrCache,
                Convert.ToInt32(treasuryRow[string.Format("{0}KVR", clsPrefix)]), nullKVR);

            // Администратор 
            factRow["REFKVSR"] = FindCachedRowID(kvsrCache, Convert.ToInt32(treasuryRow[string.Format("{0}KVSR", clsPrefix)]), nullKVSR);

            // Мероприятие 
            factRow["REFFACT"] = FindCachedRowID(factClsCache,
                Convert.ToInt32(treasuryRow[string.Format("{0}FACT", clsPrefix)]), nullFact);

            // Тип средств 
            factRow["REFMEANSTYPE"] = FindCachedRowID(meansTypeCache, Convert.ToInt32(treasuryRow[string.Format("{0}MEANSTYPE", clsPrefix)]), nullMeansType);
            // Источник финансирования 
            factRow["REFNOTIFYTYPES"] = FindRowID(dsNotifyTypes.Tables[0],
                new object[] { "SOURCEKEY", treasuryRow[string.Format("{0}FINSOURCE", clsPrefix)] }, nullNotifyTypes);
            // ТипФинансир 
            factRow["REFFINTYPE"] = FindCachedRowID(finTypeCache, Convert.ToInt32(treasuryRow[string.Format("{0}FINTYPE", clsPrefix)]), nullFinType);
            // ФКР
            factRow["REFFKR"] = FindCachedRowID(fkrCache, Convert.ToInt32(treasuryRow[string.Format("{0}KFSR", clsPrefix)]), nullFKR);
            // ЭКР
            factRow["REFEKR"] = FindCachedRowID(ekrCache, Convert.ToInt32(treasuryRow[string.Format("{0}KESR", clsPrefix)]), nullEKR);
            // КЦСР
            factRow["REFKCSR"] = FindCachedRowID(kcsrCache, Convert.ToInt32(treasuryRow[string.Format("{0}KCSR", clsPrefix)]), nullKCSR);
            // СубЭКР 
            factRow["REFSUBKESR"] = FindCachedRowID(subEkrCache, Convert.ToInt32(treasuryRow[string.Format("{0}SUBKESR", clsPrefix)]), nullSubKESR);

            switch (this.CurrentDBVersion)
            {
                case "27.02":
                case "28.00":
                case "29.01":
                case "29.02":
                case "30.00":
                case "30.01":
                    // Направление 
                    factRow["REFDIRECTION"] = nullDirection;
                    break;

                default:
                    // Направление 
                    factRow["REFDIRECTION"] = FindCachedRowID(directionCache, Convert.ToInt32(treasuryRow[string.Format("{0}DIRECTIONCLS", clsPrefix)]), nullDirection);
                    break;
            }

            if (MajorDBVersion >= 35)
            {
                factRow["RefFundsSource"] = FindCachedRowID(fundsSourceCache,
                    Convert.ToInt32(treasuryRow[string.Format("{0}FundsSource", clsPrefix)]), nullFundsSource);
                factRow["RefBudget"] = FindBudgetBudgetRef(treasuryRow["BudgetBudgetName"].ToString(),
                    treasuryRow["BudgetBudgetYear"].ToString());
                string fieldName = string.Format("{0}RegionCLS", clsPrefix);
                factRow["RefRegions"] = FindCachedRowID(regionsCache, Convert.ToInt32(treasuryRow[fieldName]), nullRegions);
            }

            string transfert = "-1";
            if (MajorDBVersion >= 38)
            {
                transfert = treasuryRow[string.Format("{0}TransfertCLS", clsPrefix)].ToString();
                if (transfert == string.Empty)
                    transfert = "-1";
            }
            factRow["RefTransf"] = FindCachedRowID(transfertCache, Convert.ToInt32(transfert), nullTransfert);
        }

        // проверяем на заполненность расходной клс (фкр) - если заполненo - возвращаем тру
        private bool CheckTreasuryCls(DataRow treasuryRow, int recordType)
        {
            string clsPrefix = GetClsPrefix(recordType);
            return (Convert.ToInt32(treasuryRow[string.Format("{0}KFSR", clsPrefix)]) != 0);
        }

        // не качаем временный тип средств
        private bool IsTempMeansType(DataRow treasuryRow, int recordType)
        {
            string clsPrefix = GetClsPrefix(recordType);
            string meansType = treasuryRow[string.Format("{0}MEANSTYPE", clsPrefix)].ToString().Trim();
            return (meansType == tempMeansType);
        }

        private bool IsTempMeansType(DataRow treasuryRow, string clsPrefix)
        {
            string meansType = treasuryRow[string.Format("{0}MEANSTYPE", clsPrefix)].ToString().Trim();
            return (meansType == tempMeansType);
        }

        private void CheckBudgetTreasuryRow(DataRow budgetRow, double sum, int recordType)
        {
            int rowId = Convert.ToInt32(budgetRow["Id"]);
            if (!fmTreasuryCache.ContainsKey(rowId))
            {
                // выводим предупреждение
                string message = string.Format("Запись бюджета с ID '{0}' ({1}) отсутствует в базе данных 'ФМ'.", rowId, tDetailTableName);
                CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeWarning, message, this.PumpID, this.SourceID);
                if (recordType == 1)
                    sumForCheckDebit += sum;
                else
                    sumForCheckReturnDebit += sum;
            }
            else
            {
                // помечаем запись в кэше - типа запись у нас есть, не удаленная, и прошла все проверки
                string cacheValue = fmTreasuryCache[rowId];
                fmTreasuryCache[rowId] = cacheValue.Remove(cacheValue.Length - 1) + "1";
            }
        }

        /// <summary>
        /// Добавляет или апдейтит строку таблицы казначейства
        /// </summary>
        /// <param name="treasuryRow">Строка таблицы бюджета</param>
        /// <param name="recordType">Тип записи</param>
        /// <param name="sum">Сумма</param>
        /// <param name="updatedRowsCount">Счетчик обновленных записей</param>
        /// <param name="addedRowsCount">Счетчик добавленных записей</param>
        private void PumpTreasuryRow(DataRow treasuryRow, int acceptDate, int recordType, 
            ref int updatedRowsCount, ref int addedRowsCount, DataRow fmRow)
        {
          //  int rowId = Convert.ToInt32(treasuryRow["Id"]);
         //   if (fmTreasuryCache.ContainsKey(rowId))
         //   {
                // помечаем запись в кэше - типа запись у нас есть, не удаленная - если останется 2 - не прошла все проверки
           //     string cacheValue = fmTreasuryCache[rowId];
           //     fmTreasuryCache[rowId] = cacheValue.Remove(cacheValue.Length - 1) + "2";
          //  }

            double sum = GetDoubleCellValue(treasuryRow, "CREDIT", 0);
            if (sum == 0)
                return;

            // не качаем временный тип средств
            if (IsTempMeansType(treasuryRow, recordType))
                return;

            // проверяем на заполненность расходной клс (фкр) - если заполненo - закачиваем
            if (this.Region == RegionName.Novosibirsk)
            {
                if (!CheckTreasuryCls(treasuryRow, recordType))
                {
                    nullFkrSum += sum;
                    return;
                }
            }

            // если запущен этап проверки - сверяем строку бюджета с нашей, 
            // также помечаем в кэше казначейства, что строка бюджета не удалена (по сути строка - присутствует в кэше)
            // если строки бюджета нет у нас - выводим предупреждение
            if (isCheckStage)
            {
                CheckBudgetTreasuryRow(treasuryRow, sum, recordType);
                return;
            }

            if (tDetailTableName == "CorrectionDetail")
            {
                if (treasuryRow["ProgIndex"].ToString() == "315")
                    sum *= -1;
            }

            if (fmRow == null)
            {
                if (dsFacialFinDetail.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT * 2)
                {
                    UpdateFMTreasuryData();
                    ClearDataSet(daFacialFinDetail, ref dsFacialFinDetail);
                }
                // закачивается новая строка
                fmRow = dsFacialFinDetail.Tables[0].NewRow();
                fmRow.BeginEdit();
                fmRow["SOURCEKEY"] = treasuryRow["ID"];
                fmRow["RECORDTYPE"] = recordType;
                dsFacialFinDetail.Tables[0].Rows.Add(fmRow);
                addedRowsCount++;
            }
            else
                // существующая строка обновляется
                updatedRowsCount++;

            fmRow["PUMPID"] = this.PumpID;
            fmRow["SOURCEID"] = this.SourceID;
            fmRow["PROGINDEX"] = treasuryRow["PROGINDEX"];
            fmRow["REFPROGINDEXFACIAL"] = FindRowID(dsProgIndexFacial.Tables[0], 
                string.Format("CODESTR = '{0}'", treasuryRow["PROGINDEX"]), -1);
            fmRow["RefYearDayUNV"] = acceptDate;

            SetSumFields(fmRow, sum, recordType);

            SetClsFields(fmRow, treasuryRow, recordType);
            fmRow.EndEdit();
        }

        private int GetTreasuryRecordType(int progIndex)
        {
            if ((progIndex == 63) || (progIndex == 68) || (progIndex == 73) || (progIndex == 64) || (progIndex == 313))
                return 1;
            else
                return 2;
        }

        private void PumpTreasuryRows(ref int addedRowsCount, ref int updatedRowsCount, 
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
                PumpTreasuryRow(treasuryRow, captionRow.AcceptDate, recType, ref updatedRowsCount, ref addedRowsCount, null);
            }
            else
            {
                // для казначейства типа записи определяется по дест или сурс тайпу
                if (captionRow.SourceType == 1)
                    PumpTreasuryRow(treasuryRow, captionRow.AcceptDate, 1, ref updatedRowsCount, ref addedRowsCount, null);
                if (captionRow.DestType == -1)
                    PumpTreasuryRow(treasuryRow, captionRow.AcceptDate, 2, ref updatedRowsCount, ref addedRowsCount, null);
                if (captionRow.DestType == 1)
                    PumpTreasuryRow(treasuryRow, captionRow.AcceptDate, 3, ref updatedRowsCount, ref addedRowsCount, null);
                if (captionRow.SourceType == -1)
                    PumpTreasuryRow(treasuryRow, captionRow.AcceptDate, 4, ref updatedRowsCount, ref addedRowsCount, null);
            }
        }

        private void UpdateFmRow(DataRow treasuryRow, DataRow fmRow, ref int addedRowsCount, ref int updatedRowsCount)
        {
            int recordIndex = Convert.ToInt32(treasuryRow["RecordIndex"]);
            FacialFinCaptionRow captionRow = captionAcceptedRecords[recordIndex];
            try
            {
                PumpTreasuryRow(treasuryRow, captionRow.AcceptDate, Convert.ToInt32(fmRow["RecordType"]),
                    ref updatedRowsCount, ref addedRowsCount, fmRow);
            }
            catch (Exception ex)
            {
                throw new PumpDataFailedException(string.Format("Ошибка при обновлении строки (id бюджета {0}): {1}",
                    treasuryRow["ID"], ex.Message));
            }
        }

        private void UpdateFmRows(ref int addedRowsCount, ref int updatedRowsCount)
        {
            WriteToTrace("Начало обновления данных...", TraceMessageKind.Information);
            int totalRecs = Convert.ToInt32(this.DB.ExecQuery(string.Format("select count(id) from {0} where SOURCEID = {1}",
                fctFacialFinDetail.FullDBName, this.SourceID), QueryResultTypes.Scalar));
            if (totalRecs == 0)
                return;
            int firstID = Convert.ToInt32(this.DB.ExecQuery(string.Format("select min(id) from {0} where SOURCEID = {1}",
                fctFacialFinDetail.FullDBName, this.SourceID), QueryResultTypes.Scalar));
            int lastID = firstID + MAX_DS_RECORDS_AMOUNT * 2 - 1;
            int processedRecCount = 0;
            IDbDataAdapter da = null;
            DataSet ds = null;
            List<string> updatedRowsList = new List<string>();
            do
            {
                string idConstr = string.Format("ID >= {0} and ID <= {1} and SOURCEID = {2}",
                    firstID, lastID, this.SourceID);
                firstID = lastID + 1;
                lastID += MAX_DS_RECORDS_AMOUNT * 2;
                InitDataSet(ref da, ref ds, fctFacialFinDetail, idConstr);
                foreach (DataRow fmRow in ds.Tables[0].Rows)
                {
                    processedRecCount++;
                    SetProgress(totalRecs, processedRecCount, "Обновление данных казначейства",
                        string.Format("Запись {0} из {1}", processedRecCount, totalRecs));
                    string sourceKey = fmRow["SourceKey"].ToString();
                    if (!factCache.ContainsKey(sourceKey))
                        continue;
                    if (!updatedRowsList.Contains(sourceKey))
                        updatedRowsList.Add(sourceKey);
                    DataRow treasuryRow = factCache[sourceKey];
                    UpdateFmRow(treasuryRow, fmRow, ref addedRowsCount, ref updatedRowsCount);
                }
                UpdateDataSet(da, ds, fctFacialFinDetail);
                ClearDataSet(ref ds);
            }
            while (processedRecCount < totalRecs);
            // также могли быть добавлены новые записи 
            WriteToTrace("Добавление данных...", TraceMessageKind.Information);
            foreach (KeyValuePair<string, DataRow> factItem in factCache)
            {
                if (updatedRowsList.Contains(factItem.Key))
                    continue;
                DataRow treasuryRow = factItem.Value;
                try
                {
                    int recordIndex = Convert.ToInt32(treasuryRow["RecordIndex"]);
                    FacialFinCaptionRow captionRow = captionAcceptedRecords[recordIndex];
                    PumpTreasuryRows(ref addedRowsCount, ref updatedRowsCount, treasuryRow, captionRow);
                }
                catch (Exception exp)
                {
                    throw new PumpDataFailedException(string.Format("Ошибка при добавлении строки (id бюджета {0}): {1}", 
                        treasuryRow["Id"], exp.Message));
                }
            }
        }

        /// <summary>
        /// Закачивает данные из бюджетной базы в нашу
        /// </summary>
        private void PumpTreasury(ref int addedRowsCount, ref int updatedRowsCount, ref int processedRecCount,
            ref string skippedRecsMessage)
        {
            int zeroDateCount = 0;
            DataRow treasuryRow;
            int selectedRecCount = 0;
            nullFkrSum = 0;

            try
            {
                int totalRecs = QueryBudgetTreasuryDataCount();
                if (totalRecs == 0)
                    return;

                string budgetTreasuryConstr = GetBudgetTreasuryConstr() + " and (d.clsType = '0') ";
                WriteToTrace(string.Format("Ограничение на {0}: {1}", tDetailTableName, budgetTreasuryConstr), TraceMessageKind.Information);
                int firstID = Convert.ToInt32(this.BudgetDB.ExecQuery(
                    string.Format("select min(d.id) from {0} d {1}", tDetailTableName, budgetTreasuryConstr), QueryResultTypes.Scalar));
                int lastID = firstID + MAX_DS_RECORDS_AMOUNT * 2 - 1;

                string idConstr = string.Empty;
                do
                {
                    idConstr = budgetTreasuryConstr;
                    // Ограничение запроса для выборки порции данных
                    if (this.PumpMode != BudgetDataPumpMode.Update)
                        // для коррекции ограничение по айди не требуется
                        if (tCaptionTableName != "CorrectionCaption")
                            idConstr += string.Format(" and d.ID >= {0} and d.ID <= {1}", firstID, lastID);
                    firstID = lastID + 1;
                    lastID += MAX_DS_RECORDS_AMOUNT * 2;
                    QueryBudgetTreasuryData(idConstr);
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

                        // обновление - не закачиваем, требующие обновления строки сохраняем в кэш, 
                        // потом проходим по существующим нашим данным, и если строка требует обновления - обновляем
                        // такой алгоритм гораздо лучше, чем для каждой строки, требующей обновления выполнять селект
                        // (при большом объеме закачанных данных - не дождаться)
                        if (this.PumpMode == BudgetDataPumpMode.Update)
                        {
                            if (factCache == null)
                                factCache = new Dictionary<string, DataRow>();
                            factCache.Add(treasuryRow["ID"].ToString(), treasuryRow);
                            continue;
                        }

                        try
                        {
                            PumpTreasuryRows(ref addedRowsCount, ref updatedRowsCount, treasuryRow, captionRow);
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
                        UpdateFMTreasuryData();
                        ClearDataSet(ref dsBudgetFacts);
                        ClearDataSet(ref dsFacialFinDetail);
                        daFacialFinDetail.Fill(dsFacialFinDetail);
                    }
                    GC.GetTotalMemory(true);
                    WriteToTrace(String.Format("Обработано {0} записей из {1}", processedRecCount, totalRecs),
                        TraceMessageKind.Information);
                }
                while (processedRecCount < totalRecs);

                // если выполняется режим обновления, данные, которые нужно обновить хранятся в factCache
                // идем по нашим данным и при совпадении id = sourceId обновляем строку
                if (this.PumpMode == BudgetDataPumpMode.Update)
                    UpdateFmRows(ref addedRowsCount, ref updatedRowsCount);

                // Сохранение данных
                UpdateFMTreasuryData();

                // выводим общую сумму по пропущенным записям по причине нулевого фкр
                if (nullFkrSum != 0)
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                        string.Format("Найдены записи с нулевым ФКР на общую сумму {0}, эти записи закачаны не были.", nullFkrSum));

            }
            finally
            {
                ClearDataSet(ref dsFacialFinDetail);
                ClearDataSet(ref dsProgIndexFacial);
                captionAcceptedRecords.Clear();
                if (factCache != null)
                    factCache.Clear();
            }
        }
    }
}
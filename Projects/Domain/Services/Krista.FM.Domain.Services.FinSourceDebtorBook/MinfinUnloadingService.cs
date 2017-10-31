using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook
{
    public class MinfinUnloadingService
    {

        private readonly IScheme scheme;

        private DataTable dtContractTypes;

        private Dictionary<int, decimal> exchangeRates;

        private int debtorBookVariant;

        private string regionCode;

        private string regionName;

        public MinfinUnloadingService(IScheme scheme)
        {
            this.scheme = scheme;
            dtContractTypes = GetReferencebook(DomainObjectsKeys.d_S_ViewContract);
            exchangeRates = new Dictionary<int, decimal>();
            regionCode = scheme.GlobalConstsManager.Consts["RegionMFRF"].Value.ToString().Substring(0, 2);
            regionName = scheme.GlobalConstsManager.Consts["RegionNom"].Value.ToString();
        }

        #region отчет для минфина

        /// <summary>
        /// данные для выгрузки в минфин (текстовый файл)
        /// каждая строка - строка в файле
        /// </summary>
        /// <returns></returns>
        public String GetMinfinData(DateTime reportDate)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(regionName);
            sb.AppendLine(regionCode);
            sb.AppendLine(reportDate.ToShortDateString());

            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                debtorBookVariant = GetDebtBookVariant(reportDate, db);
                sb.AppendLine(GetCapitalData());
                sb.AppendLine(GetOrgCreditsData(reportDate, db));
                sb.AppendLine(GetGuaranteeData(reportDate, db));
                sb.AppendLine(GetBudCreditsData(reportDate, db));
                sb.AppendLine(GetRD5Data());
                sb.AppendLine("РД=6");
                foreach (KeyValuePair<int, decimal> kvp in exchangeRates)
                {
                    if (kvp.Key == -1)
                        continue;
                    sb.AppendLine(string.Format("{0}|{1}|{2}", GetCurrencyCode(kvp.Key, db), GetCurrencyNominal(kvp.Key, db), kvp.Value));
                }
                sb.AppendLine("#");
                DataTable dtUsers = scheme.UsersManager.GetUsers();
                DataRow currentUserRow =
                    dtUsers.Select(string.Format("ID = {0}", scheme.UsersManager.GetCurrentUserID()))[0];
                string userName = string.Format("{0} {1} {2}{3}{4}",
                                                currentUserRow["LASTNAME"], currentUserRow["FIRSTNAME"],
                                                currentUserRow["PATRONYMIC"], Environment.NewLine,
                                                currentUserRow["DESCRIPTION"]);
                sb.AppendLine(userName);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Данные по ценным бумагам
        /// </summary>
        /// <returns></returns>
        private string GetCapitalData()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("РД=1");
            sb.AppendLine("||0|ИТОГО|||||||||||||||||0.00||0.00|||||0.00||||0.00|0.00|||||0.00|0.00|0.00");
            sb.AppendLine("||1|ИТОГО|||||||||||||||||0.00||0.00|||||0.00||||0.00|0.00|||||0.00|0.00|0.00");
            sb.Append("|||ВСЕГО|||||||||||||||||0.00||0.00|||||0.00||||0.00|0.00|||||0.00|0.00|0.00");
            return sb.ToString();
        }

        private string GetRD5Data()
        {
            //код ОКАТО
            string regionConst = scheme.GlobalConstsManager.Consts["OKTMO"].Value.ToString().Substring(0, 2);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("РД=5");
            sb.AppendLine(string.Format("{0}|0|ИТОГО||||||||||||||||||||0.00", regionConst));
            sb.AppendLine(string.Format("{0}|1|ИТОГО||||||||||||||||||||0.00", regionConst));
            sb.AppendLine(string.Format("{0}||ВСЕГО||||||||||||||||||||0.00", regionConst));
            sb.AppendLine("|0|ИТОГО||||||||||||||||||||0.00");
            sb.AppendLine("|1|ИТОГО||||||||||||||||||||0.00");
            sb.Append("||ВСЕГО||||||||||||||||||||0.00");
            return sb.ToString();
        }

        /// <summary>
        /// данные по кредитам полученным от организаций
        /// </summary>
        /// <param name="reportDate"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private string GetOrgCreditsData(DateTime reportDate, IDatabase db)
        {
            IEntity factAttract = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_FactAttractCI);
            IEntity factDebt = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_FactDebtCI);
            IEntity planService = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_PlanServiceCI);
            IEntity factPercent = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_FactPercentCI);
            IEntity chargePenaltyDebt = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_ChargePenaltyDebtCI);
            IEntity factPenaltyDebt = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_FactPenaltyDebtCI);
            IEntity chargePenaltyPercent = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_CIChargePenaltyPercent);
            IEntity factPenaltyPercent = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_FactPenaltyPercentCI);
            IEntity creditsEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_Creditincome);
            IEntity setlementsCreditsEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincomePos);
            IEntity debtorBookCreditincomeEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincome);

            //код ОКАТО
            string regionConst = scheme.GlobalConstsManager.Consts["OKTMO"].Value.ToString().Substring(0, 2);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("РД=2");

            IDbDataParameter[] queryParams = new IDbDataParameter[3];
            queryParams[0] = db.CreateParameter("p0", reportDate);
            queryParams[1] = db.CreateParameter("p1", new DateTime(reportDate.Year, 01, 01));
            queryParams[2] = db.CreateParameter("p2", new DateTime(reportDate.Year, 01, 01));
            // итоги в рублях и валюте
            decimal rubResults = 0;
            decimal currencyResult = 0;
            using (IDataUpdater upd = creditsEntity.GetDataUpdater("RefSTypeCredit = 0 and RefVariant = 0 and (StartDate <= ? and (RenewalDate >= ? or (RenewalDate is null and EndDate >= ?)))", null,
                queryParams))
            {
                // получаем кредиты от кредитных организаций, удовлетворяющие условию
                DataTable dtCredits = new DataTable();
                upd.Fill(ref dtCredits);
                DataTable dtContractList = GetCreditContractList(reportDate, creditsEntity, db);
                
                // собираем информацию по кредитам, отсортированным по валюте (сперва идут рублевые, потом другие)
                foreach (DataRow row in dtCredits.Select(string.Empty, "RefOKV ASC"))
                {
                    int refOkv = Convert.ToInt32(row["RefOKV"]);
                    if (!exchangeRates.ContainsKey(refOkv))
                        exchangeRates.Add(refOkv, GetExchangeRate(reportDate, refOkv, db));
                    // признак внешнего - внутреннего долга
                    string currencySign = refOkv == -1 ? "0" : "1";
                    //признак документа, на основании которого возникло долговое обязательств 
                    string baseDoc = GetContractTypeCode(GetContractData(dtContractList, string.Format("RefCreditInc = {0} and BaseDoc = true", row["ID"]), "RefViewContract"));
                    //номер документа, на основании которого возникло долговое обязательство 
                    string numberContract = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and BaseDoc = true", row["ID"]), "NumberContract");
                    //дата документа, на основании которого возникло долговое обязательство
                    string contractDate = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and BaseDoc = true", row["ID"]), "DataContract");
                    //документ, на основании которого возникло долговое обязательство
                    string functionContract = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and BaseDoc = true", row["ID"]), "FunctionContract") == "1" ? "0" : "1";
                    //номера кредитных договоров или соглашений, утративших силу
                    string loseValidityContractsNum = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and FunctionContract = false", row["ID"]), "NumberContract", "1");
                    //даты кредитных договоров или соглашений, утративших силу
                    string loseValidityContractsDate = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and FunctionContract = false", row["ID"]), "DataContract", "1");
                    //признак пролонгации
                    int isRenewal = row.IsNull("RenewalDate") ? 0 : 1;
                    //номер договора о пролонгации
                    string renewalNum = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and FunctionContract = false", row["ID"]), "NumberContract", "2");
                    //дата договора о пролонгации
                    string renevalDate = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and FunctionContract = false", row["ID"]), "DataContract", "2");
                    //номера дополнительных договоров (в случае внесения не обусловленных пролонгацией)
                    string nonRenewalNumExtra = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and BaseDoc = false", row["ID"]), "NumberContract", "1");
                    //даты дополнительных договоров (в случае внесения не обусловленных пролонгацией изменений)
                    string ninRenewalDateExtra = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and BaseDoc = false", row["ID"]), "DataContract", "1");
                    //номера мировых соглашений (в случае внесения не обусловленных пролонгацией изменений)
                    string nonRenewalNumAgreement = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and BaseDoc = false", row["ID"]), "NumberContract", "3");
                    //даты мировых соглашений (в случае внесения не обусловленных пролонгацией изменений)
                    string nonRenewalDateAgreement = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and BaseDoc = false", row["ID"]), "DataContract", "3");
                    //наименование кредитора
                    string creditorName = GetOrganizationName(Convert.ToInt32(row["RefOrganizations"]), db);
                    //код валюты обязательства
                    string currencyCode = GetCurrencyCode(Convert.ToInt32(row["RefOkv"]), db);
                    //дата (период) получения кредита
                    string suppyCredit = GetSupplyCredit(db, scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_FactAttractCI), Convert.ToInt32(row["ID"]), "RefCreditInc");
                    //процентная ставка по кредиту
                    string creditPercent = GetPercent(db, scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_JournalPercentCI), Convert.ToInt32(row["ID"]), "RefCreditInc");
                    //дата (период) погашения кредита
                    string creditEndDate = Convert.ToDateTime(row["EndDate"]).ToShortDateString();

                    decimal sum =
                        GetDetailSum(row, reportDate, factAttract, db, "RefCreditInc", "FactDate") -
                        GetDetailSum(row, reportDate, factDebt, db, "RefCreditInc", "FactDate") +
                        GetDetailSum(row, reportDate, planService, db, "RefCreditInc", "EndDate") -
                        GetDetailSum(row, reportDate, factPercent, db, "RefCreditInc", "FactDate") +
                        GetDetailSum(row, reportDate, chargePenaltyDebt, db, "RefCreditInc", "StartDate") -
                        GetDetailSum(row, reportDate, factPenaltyDebt, db, "RefCreditInc", "FactDate") +
                        GetDetailSum(row, reportDate, chargePenaltyPercent, db, "RefCreditInc", "StartDate") -
                        GetDetailSum(row, reportDate, factPenaltyPercent, db, "RefCreditInc", "FactDate");
                    if (refOkv != -1)
                        sum = sum * exchangeRates[refOkv];
                    sum = Math.Round(sum, 2, MidpointRounding.AwayFromZero);
                    if (refOkv == -1)
                        rubResults += sum;
                    else
                        currencyResult += sum;

                    sb.AppendLine(string.Format(CultureInfo.InvariantCulture,
                        "{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}|{19}|{20}",
                        regionConst, currencySign, baseDoc, numberContract, contractDate, functionContract,
                        loseValidityContractsNum, loseValidityContractsDate, isRenewal, renewalNum, renevalDate, nonRenewalNumExtra,
                        ninRenewalDateExtra, nonRenewalNumAgreement, nonRenewalDateAgreement, creditorName, currencyCode, suppyCredit,
                        creditPercent, creditEndDate, GetFormatSum(sum)));
                }
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0}|0|ИТОГО|||||||||||||||||||{1}", regionConst, GetFormatSum(rubResults)));
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0}|1|ИТОГО|||||||||||||||||||{1}", regionConst, GetFormatSum(currencyResult)));
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0}||ВСЕГО|||||||||||||||||||{1}", regionConst, GetFormatSum(rubResults + currencyResult)));
            }

            Dictionary<string, decimal> currencySum = new Dictionary<string, decimal>();
            // данные по поселениям
            using (IDataUpdater upd = setlementsCreditsEntity.GetDataUpdater(string.Format("RefVariant = {0} and RefTypeCredit = 0", debtorBookVariant), null))
            {
                DataTable dtSettlements = new DataTable();
                upd.Fill(ref dtSettlements);
                foreach (DataRow row in dtSettlements.Select(string.Empty, "RefOKV ASC"))
                {
                    int refOkv = Convert.ToInt32(row["RefOKV"]);
                    string currencyCode = GetCurrencyCode(refOkv, db);
                    decimal sum = Convert.ToDecimal(row["CapitalDebt"]);
                    sum += Convert.ToDecimal(row["ServiceDebt"]);

                    if (!currencySum.ContainsKey(currencyCode))
                        currencySum.Add(currencyCode, sum);
                    else
                        currencySum[currencyCode] += sum;
                }
            }

            using (IDataUpdater upd = debtorBookCreditincomeEntity.GetDataUpdater(string.Format("RefVariant = {0} and RefTypeCredit = 0 and RefRegion in (select id from d_Regions_Analysis where RefTerr = 4 or RefTerr = 7)",
                debtorBookVariant), null))
            {
                DataTable dtSettlements = new DataTable();
                upd.Fill(ref dtSettlements);
                foreach (DataRow row in dtSettlements.Select(string.Empty, "RefOKV ASC"))
                {
                    int refOkv = Convert.ToInt32(row["RefOKV"]);
                    string currencyCode = GetCurrencyCode(refOkv, db);
                    decimal sum = Convert.ToDecimal(row["CapitalDebt"]);
                    sum += Convert.ToDecimal(row["ServiceDebt"]);

                    if (!currencySum.ContainsKey(currencyCode))
                        currencySum.Add(currencyCode, sum);
                    else
                        currencySum[currencyCode] += sum;
                }
            }
            foreach (KeyValuePair<string, decimal> kvp in currencySum)
            {
                string currencySign = kvp.Key == "RUB" ? "0" : "1";
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "|{0}||||||||||||||{1}||||{2}", currencySign, kvp.Key, GetFormatSum(kvp.Value)));
            }
            GetCurrencySum(currencySum, ref rubResults, ref currencyResult);

            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "|0|ИТОГО|||||||||||||||||{0}", GetFormatSum(rubResults)));
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "|1|ИТОГО|||||||||||||||||{0}", GetFormatSum(currencyResult)));
            sb.Append(string.Format(CultureInfo.InvariantCulture, "||ВСЕГО|||||||||||||||||{0}", GetFormatSum(rubResults + currencyResult)));

            return sb.ToString();
        }

        /// <summary>
        /// данные по кредитам, полученным от других бюджетов
        /// </summary>
        /// <param name="reportDate"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private string GetBudCreditsData(DateTime reportDate, IDatabase db)
        {
            IEntity factAttract = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_FactAttractCI);
            IEntity factDebt = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_FactDebtCI);
            IEntity planService = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_PlanServiceCI);
            IEntity factPercent = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_FactPercentCI);
            IEntity chargePenaltyDebt = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_ChargePenaltyDebtCI);
            IEntity factPenaltyDebt = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_FactPenaltyDebtCI);
            IEntity chargePenaltyPercent = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_CIChargePenaltyPercent);
            IEntity factPenaltyPercent = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_FactPenaltyPercentCI);
            IEntity creditsEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_Creditincome);
            IEntity setlementsCreditsEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincomePos);
            IEntity debtorBookCreditincomeEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincome);
            //код ОКАТО
            string regionConst = scheme.GlobalConstsManager.Consts["OKTMO"].Value.ToString().Substring(0, 2);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("РД=4");
            sb.AppendLine(string.Format("{0}|ВСЕГО|||||||||||||0||||||0.00", regionConst));
            IDbDataParameter[] queryParams = new IDbDataParameter[3];
            queryParams[0] = db.CreateParameter("p0", reportDate);
            queryParams[1] = db.CreateParameter("p1", new DateTime(reportDate.Year, 01, 01));
            queryParams[2] = db.CreateParameter("p2", new DateTime(reportDate.Year, 01, 01));
            decimal rubResults = 0;
            decimal sum = 0;
            using (IDataUpdater upd = creditsEntity.GetDataUpdater("RefSTypeCredit = 1 and RefVariant = 0 and (StartDate <= ? and (RenewalDate >= ? or (RenewalDate is null and EndDate >= ?)))", null,
                queryParams))
            {
                DataTable dtCredits = new DataTable();
                upd.Fill(ref dtCredits);
                DataTable dtContractList = GetCreditContractList(reportDate, creditsEntity, db);
                // собираем информацию по кредитам, отсортированным по валюте (сперва идут рублевые, потом другие)
                foreach (DataRow row in dtCredits.Select(string.Empty, "RefOKV ASC"))
                {
                    int refOkv = Convert.ToInt32(row["RefOKV"]);
                    if (!exchangeRates.ContainsKey(refOkv))
                        exchangeRates.Add(refOkv, GetExchangeRate(reportDate, refOkv, db));
                    //признак документа, на основании которого возникло долговое обязательств 
                    string baseDoc = GetContractTypeCode(GetContractData(dtContractList, string.Format("RefCreditInc = {0} and BaseDoc = true", row["ID"]), "RefViewContract"));
                    //номер документа, на основании которого возникло долговое обязательство 
                    string numberContract = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and BaseDoc = true", row["ID"]), "NumberContract");
                    //дата документа, на основании которого возникло долговое обязательство
                    string contractDate = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and BaseDoc = true", row["ID"]), "DataContract");
                    //документ, на основании которого возникло долговое обязательство
                    string functionContract = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and FunctionContract = true", row["ID"]), "FunctionContract");
                    //номера кредитных договоров или соглашений, утративших силу
                    string loseValidityContractsNum = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and FunctionContract = false", row["ID"]), "NumberContract", "6");
                    //даты кредитных договоров или соглашений, утративших силу
                    string loseValidityContractsDate = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and FunctionContract = false", row["ID"]), "DataContract", "6"); ;
                    //признак пролонгации
                    int isRenewal = row.IsNull("RenewalDate") ? 0 : 1;
                    //номер договора о пролонгации
                    string renewalNum = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and FunctionContract = true", row["ID"]), "NumberContract", "7");
                    //дата договора о пролонгации
                    string renevalDate = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and FunctionContract = true", row["ID"]), "DataContract", "7");
                    //номера дополнительных договоров (в случае внесения не обусловленных пролонгацией)
                    string nonRenewalNumExtra = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and FunctionContract = false", row["ID"]), "NumberContract", "7", "8", "9", "11");
                    //даты дополнительных договоров (в случае внесения не обусловленных пролонгацией изменений)
                    string nonRenewalDateExtra = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and FunctionContract = false", row["ID"]), "DataContract", "7", "8", "9", "11");
                    //номера мировых соглашений (в случае внесения не обусловленных пролонгацией изменений)
                    string nonRenewalNumAgreement = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and FunctionContract = false", row["ID"]), "NumberContract", "10");
                    //даты мировых соглашений (в случае внесения не обусловленных пролонгацией изменений)
                    string nonRenewalDateAgreement = GetContractData(dtContractList, string.Format("RefCreditInc = {0} and FunctionContract = false", row["ID"]), "DataContract", "10");
                    //Признак (0 – бюджетная ссуда, 1 – бюджетный кредит)
                    string crediType = "1";
                    //Код уровня бюджета
                    string budgetLevel = "1";
                    //Наименование уровня субъекта РФ или МР из бюджета которого получена ссуда
                    string creditorName = string.Empty;//GetOrganizationName(Convert.ToInt32(row["RefOrganizations"]), db);
                    //код валюты обязательства
                    string currencyCode = GetCurrencyCode(Convert.ToInt32(row["RefOkv"]), db);
                    //Дата получения бюджетной ссуды, кредита
                    string creditPercent = Convert.ToDateTime(row["StartDate"]).ToShortDateString();
                    //дата (период) погашения бюджетной ссуды, кредита
                    string creditEndDate = Convert.ToDateTime(row["EndDate"]).ToShortDateString();

                    sum =
                        GetDetailSum(row, reportDate, factAttract, db, "RefCreditInc", "FactDate") -
                        GetDetailSum(row, reportDate, factDebt, db, "RefCreditInc", "FactDate") +
                        GetDetailSum(row, reportDate, planService, db, "RefCreditInc", "EndDate") -
                        GetDetailSum(row, reportDate, factPercent, db, "RefCreditInc", "FactDate") +
                        GetDetailSum(row, reportDate, chargePenaltyDebt, db, "RefCreditInc", "StartDate") -
                        GetDetailSum(row, reportDate, factPenaltyDebt, db, "RefCreditInc", "FactDate") +
                        GetDetailSum(row, reportDate, chargePenaltyPercent, db, "RefCreditInc", "StartDate") -
                        GetDetailSum(row, reportDate, factPenaltyPercent, db, "RefCreditInc", "FactDate");
                    
                    rubResults += sum;

                    sum = Math.Round(sum, 2, MidpointRounding.AwayFromZero);
                    sb.AppendLine(string.Format(CultureInfo.InvariantCulture,
                        "{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}|{19}|{20}",
                        regionConst, baseDoc, numberContract, contractDate, functionContract, loseValidityContractsNum,
                        loseValidityContractsDate,
                        isRenewal, renewalNum, renevalDate, nonRenewalNumExtra, nonRenewalDateExtra,
                        nonRenewalNumAgreement, nonRenewalDateAgreement, crediType, budgetLevel,
                        creditorName, currencyCode, creditPercent, creditEndDate, GetFormatSum(sum)));
                }
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0}|ВСЕГО|||||||||||||1||||||{1}", regionConst, GetFormatSum(rubResults)));
            }

            sum = 0;
            // данные по поселениям
            using (IDataUpdater upd = setlementsCreditsEntity.GetDataUpdater(string.Format("RefVariant = {0} and RefTypeCredit = 1", debtorBookVariant), null))
            {
                DataTable dtSettlements = new DataTable();
                upd.Fill(ref dtSettlements);
                
                foreach (DataRow row in dtSettlements.Rows)
                {
                    sum += Convert.ToDecimal(row["CapitalDebt"]);
                    sum += Convert.ToDecimal(row["ServiceDebt"]);
                }
            }
            using (IDataUpdater upd = debtorBookCreditincomeEntity.GetDataUpdater(string.Format("RefVariant = {0} and RefTypeCredit = 1 and RefRegion in (select id from d_Regions_Analysis where RefTerr = 4 or RefTerr = 7)",
                debtorBookVariant), null))
            {
                DataTable dtSettlements = new DataTable();
                upd.Fill(ref dtSettlements);
                foreach (DataRow row in dtSettlements.Rows)
                {
                    sum += Convert.ToDecimal(row["CapitalDebt"]);
                    sum += Convert.ToDecimal(row["ServiceDebt"]);
                }
            }
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "||||||||||||||0||||||{0}", GetFormatSum(sum)));
            sb.Append("||||||||||||||1||||||0.00");

            return sb.ToString();
        }

        private string GetGuaranteeData(DateTime reportDate, IDatabase db)
        {
            IEntity principalContractEntity =
                scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_PrincipalContrGrnt);
            IEntity factDebtPr = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_FactDebtPrGrnt);
            IEntity planServicePrEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_PlanServicePrGrnt);
            IEntity factPercentPrEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_FactPercentPrGrnt);
            IEntity factAttractPrEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_FactAttractPrGrnt);
            IEntity factAttractEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_FactAttractGrnt);
            IEntity debtorBookGuarantissuedEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBGuarantissued);
            //код ОКАТО
            string regionConst = scheme.GlobalConstsManager.Consts["OKTMO"].Value.ToString().Substring(0, 2);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("РД=3");
            IDbDataParameter[] queryParams = new IDbDataParameter[3];
            queryParams[0] = db.CreateParameter("p0", reportDate);
            queryParams[1] = db.CreateParameter("p1", new DateTime(reportDate.Year, 01,01));
            queryParams[2] = db.CreateParameter("p2", new DateTime(reportDate.Year, 01, 01));
            IEntity guaranteeEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_Guarantissued);
            IEntity setlementsCreditsEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBGuarantissuedPos);
            DataTable dtContractList = GetGuaranteeContractList(reportDate, guaranteeEntity, db);

            decimal rubResults = 0;
            decimal currencyResult = 0;
            using (IDataUpdater upd = guaranteeEntity.GetDataUpdater("RefVariant = 0 and (StartDate <= ? and (RenewalDate >= ? or (RenewalDate is null and EndDate >= ?)))", null,
                queryParams))
            {
                DataTable dtGuarantee = new DataTable();
                upd.Fill(ref dtGuarantee);
                foreach (DataRow row in dtGuarantee.Select(string.Empty, "RefOKV ASC"))
                {
                    int refOkv = Convert.ToInt32(row["RefOKV"]);
                    if (!exchangeRates.ContainsKey(refOkv))
                        exchangeRates.Add(refOkv, GetExchangeRate(reportDate, refOkv, db));

                    string okv = Convert.ToInt32(row["RefOKV"]) == -1 ? "0" : "1";
                    //признак документа, на основании которого возникло долговое обязательство по гарантии 
                    string baseContractSign = GetContractTypeCode(GetContractData(dtContractList, "BaseDoc = true", "RefViewContract"));
                    //номер документа, на основании которого возникло долговое обязательство по гарантии
                    string baseContractNumber = GetContractData(dtContractList, string.Format("RefGrnt = {0} and BaseDoc = true", row["ID"]), "NumberContract");
                    //дата документа, на основании которого возникло долговое обязательство по гарантии
                    string baseContractDate = GetContractData(dtContractList, string.Format("RefGrnt = {0} and BaseDoc = true", row["ID"]), "DataContract");
                    //
                    string baseContractLoseSign = GetContractData(dtContractList, string.Format("RefGrnt = {0} and BaseDoc = true", row["ID"]), "FunctionContract") == "1" ? "0" : "1";
                    //номера договоров о предоставлении гарантий утративших силу  
                    string loseContractsNumber = GetContractData(dtContractList, string.Format("RefGrnt = {0} and FunctionContract = false", row["ID"]), "NumberContract", "4");
                    //даты договоров о предоставлении гарантий утративших силу  
                    string loseContractsDate = GetContractData(dtContractList, string.Format("RefGrnt = {0} and FunctionContract = false", row["ID"]), "DataContract", "4");
                    //признак пролонгации
                    string renewalSign = row.IsNull("RenewalDate") ? "0" : "1";
                    //номер договора о пролонгации 
                    string renewalNumber = GetContractData(dtContractList, string.Format("RefGrnt = {0} and FunctionContract = true", row["ID"]), "NumberContract", "5");
                    //дата договора о пролонгации
                    string renewalDate = GetContractData(dtContractList, string.Format("RefGrnt = {0} and FunctionContract = true", row["ID"]), "DataContract", "5");
                    //номера дополнительных договоров 
                    string nonRenewalNumber = GetContractData(dtContractList, string.Format("RefGrnt = {0} and BaseDoc = false", row["ID"]), "NumberContract", "5");
                    //даты дополнительных договоров
                    string nonRenewalDate = GetContractData(dtContractList, string.Format("RefGrnt = {0} and BaseDoc = false", row["ID"]), "DataContract", "5");
                    //наименование гаранта
                    string guarantName = GetOrganizationName(Convert.ToInt32(row["RefOrganizationsPlan2"]), db);
                    //код организационно-правовой формы принципала
                    string orgPrincipal = GetOrganizationOkopf(Convert.ToInt32(row["RefOrganizations"]), db);
                    //наименование принципала
                    string principalName = GetOrganizationName(Convert.ToInt32(row["RefOrganizations"]), db);
                    //код организационно-правовой формы бенефициара
                    string orgBeneficiar = GetOrganizationOkopf(Convert.ToInt32(row["RefOrganizationsPlan3"]), db);
                    //наименование бенефициара
                    string benificiarName = GetOrganizationName(Convert.ToInt32(row["RefOrganizationsPlan3"]), db);
                    //код валюты обязательства в соответствии с Общероссийским  классификатором валют
                    string currencyCode = GetCurrencyCode(Convert.ToInt32(row["RefOKV"]), db);
                    //Дата или момент вступления гарантии в силу
                    string dateDoc = Convert.ToDateTime(row["DateDoc"]).ToShortDateString();
                    //срок действия гарантии
                    string endDate = Convert.ToDateTime(row["EndDate"]).ToShortDateString();
                    //срок или период предъявления требований по гарантии
                    string dateDemand = row["DateDemand"].ToString();
                    //срок или дата исполнения гарантии
                    string datePerformance = row["DatePerformance"].ToString();
                    // итог
                    decimal sum = 0;
                    if (refOkv == -1)
                    {
                        decimal sumA1 =
                            GetDetailSum(row, new DateTime(2020, 12, 12), principalContractEntity, db, "RefGrnt", "EndDate") -
                            GetDetailSum(row, reportDate, factDebtPr, db, "RefGrnt", "FactDate");
                        decimal sumA2 = row.IsNull("DebtSum") ? 0 : Convert.ToDecimal(row["DebtSum"]);
                        sum = sumA1 > sumA2 ? sumA2 : sumA1;
                        object rowCount = db.ExecQuery(
                            string.Format("select Count(id) from {0} where RefGrnt = {1}", planServicePrEntity.FullDBName, row["ID"]), QueryResultTypes.Scalar);
                        decimal sumB1 = GetDetailSum(row, new DateTime(2020, 12, 12), planServicePrEntity, db, "RefGrnt", "EndDate") -
                            GetDetailSum(row, reportDate, factPercentPrEntity, db, "RefGrnt", "FactDate");
                        decimal sumB2 = (row.IsNull("DebtSum") ? 0 : Convert.ToDecimal(row["Sum"])) -
                                        (row.IsNull("DebtSum") ? 0 : Convert.ToDecimal(row["DebtSum"]));
                        sum += Convert.ToInt32(rowCount) == 0 || sumB1 > sumB2 ? sumB2 : sumB1;
                    }
                    else
                    {
                        sum = GetDetailSum(row, reportDate, factAttractPrEntity, db, "RefGrnt", "FactDate") -
                              GetDetailSum(row, reportDate, factDebtPr, db, "RefGrnt", "FactDate") -
                              GetGuaranteeDetailSum(row, reportDate, factAttractEntity, db, "RefGrnt", "FactDate", "RefTypSum = 1") +
                              GetGuaranteeDetailSum(row, reportDate, planServicePrEntity, db, "RefGrnt", "EndDate",
                                                    new string[] {"CurrencySum", "CurrencyMargin", "CurrencyCommission"}) -
                              GetGuaranteeDetailSum(row, reportDate, factPercentPrEntity, db, "RefGrnt", "FactDate",
                                                    new string[] {"CurrencySum", "CurrencyMargin", "CurrencyCommission"}) -
                              GetGuaranteeDetailSum(row, reportDate, factAttractEntity, db, "RefGrnt", "FactDate",
                                                    "(RefTypSum = 2 or RefTypSum = 5 or RefTypSum = 6 or RefTypSum = 7)");
                        sum = sum * exchangeRates[refOkv];
                    }
                    sum = Math.Round(sum, 2, MidpointRounding.AwayFromZero);
                    sb.AppendLine(string.Format(CultureInfo.InvariantCulture, 
                        "{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}|{19}|{20}|{21}|{22}|{23}",
                        regionConst, okv, baseContractSign, baseContractNumber, baseContractDate, baseContractLoseSign,
                        loseContractsNumber, loseContractsDate, renewalSign, renewalNumber, renewalDate, nonRenewalNumber, nonRenewalDate,
                        guarantName, orgPrincipal, principalName, orgBeneficiar, benificiarName, currencyCode, dateDoc,
                        endDate, dateDemand, datePerformance, GetFormatSum(sum)));
                    if (refOkv == -1)
                        rubResults += sum;
                    else
                        currencyResult += sum;
                }
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0}|0|ИТОГО|||||||||||||||||||||{1}", regionConst, GetFormatSum(rubResults)));
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0}|1|ИТОГО|||||||||||||||||||||{1}", regionConst, GetFormatSum(currencyResult)));
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0}||ВСЕГО|||||||||||||||||||||{1}", regionConst, GetFormatSum(rubResults + currencyResult)));
                
            }
            // данные по поселениям
            Dictionary<string, decimal> currencySum = new Dictionary<string, decimal>();
            using (IDataUpdater upd = setlementsCreditsEntity.GetDataUpdater(string.Format("RefVariant = {0}", debtorBookVariant), null))
            {
                DataTable dtSettlements = new DataTable();
                upd.Fill(ref dtSettlements);
                foreach (DataRow row in dtSettlements.Select(string.Empty, "RefOKV ASC"))
                {
                    int refOkv = Convert.ToInt32(row["RefOKV"]);
                    string currencyCode = GetCurrencyCode(refOkv, db);
                    decimal sum = Convert.ToDecimal(row["TotalDebt"]);
                    if (refOkv == -1)
                        rubResults += sum;
                    else
                        currencyResult += sum;

                    if (!currencySum.ContainsKey(currencyCode))
                        currencySum.Add(currencyCode, sum);
                    else
                        currencySum[currencyCode] += sum;
                }
            }
            using (IDataUpdater upd = debtorBookGuarantissuedEntity.GetDataUpdater(string.Format("RefVariant = {0} and RefRegion in (select id from d_Regions_Analysis where RefTerr = 4 or RefTerr = 7)",
                debtorBookVariant), null))
            {
                DataTable dtSettlements = new DataTable();
                upd.Fill(ref dtSettlements);
                foreach (DataRow row in dtSettlements.Select(string.Empty, "RefOKV ASC"))
                {
                    int refOkv = Convert.ToInt32(row["RefOKV"]);
                    string currencyCode = GetCurrencyCode(refOkv, db);
                    decimal sum = Convert.ToDecimal(row["TotalDebt"]);
                    if (refOkv == -1)
                        rubResults += sum;
                    else
                        currencyResult += sum;

                    if (!currencySum.ContainsKey(currencyCode))
                        currencySum.Add(currencyCode, sum);
                    else
                        currencySum[currencyCode] += sum;
                }
            }
            foreach (KeyValuePair<string, decimal> kvp in currencySum)
            {
                string currencySign = kvp.Key == "RUB" ? "0" : "1";
                sb.AppendLine(string.Format("|{0}||||||||||||||||{1}|||||{2}", currencySign, kvp.Key, GetFormatSum(kvp.Value)));
            }
            GetCurrencySum(currencySum, ref rubResults, ref currencyResult);

            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "|0|ИТОГО|||||||||||||||||||||{0}", GetFormatSum(rubResults)));
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "|1|ИТОГО|||||||||||||||||||||{0}", GetFormatSum(currencyResult)));
            sb.Append(string.Format(CultureInfo.InvariantCulture, "||ВСЕГО|||||||||||||||||||||{0}", GetFormatSum(rubResults + currencyResult)));
            return sb.ToString();
        }

        #endregion

        #region вспомогательные методы для получения данных

        /// <summary>
        /// список всех договоров для всех кредитов, удовлетворяющих условию выборки по датам и вариантам
        /// </summary>
        /// <param name="reportDate"></param>
        /// <param name="creditsEntity"></param>
        /// <returns></returns>
        private DataTable GetCreditContractList(DateTime reportDate, IEntity creditsEntity, IDatabase db)
        {
            IEntity contractListEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_ListContractCl);
            using (IDataUpdater du = contractListEntity.GetDataUpdater())
            {
                DataTable dtData = new DataTable();
                du.Fill(ref dtData);
                return dtData;
            }
        }

        /// <summary>
        /// список всех договоров для всех кредитов, удовлетворяющих условию выборки по датам и вариантам
        /// </summary>
        /// <param name="reportDate"></param>
        /// <param name="creditsEntity"></param>
        /// <returns></returns>
        private DataTable GetGuaranteeContractList(DateTime reportDate, IEntity guaranteeEntity, IDatabase db)
        {
            IEntity contractListEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_ListContractGrnt);
            using (IDataUpdater du = contractListEntity.GetDataUpdater())
            {
                DataTable dtData = new DataTable();
                du.Fill(ref dtData);
                return dtData;
            }
        }

        /// <summary>
        /// получение кода вида договора
        /// </summary>
        /// <param name="refContractType"></param>
        /// <returns></returns>
        private string GetContractTypeCode(string refContractType)
        {
            refContractType = refContractType.Split('^')[0];
            if (string.IsNullOrEmpty(refContractType))
                return string.Empty;
            return dtContractTypes.Select(string.Format("ID = {0}", refContractType))[0]["Code"].ToString(); 
        }

        private DataTable GetReferencebook(string key)
        {
            IEntity viewContractEntity = scheme.RootPackage.FindEntityByName(key);
            using (IDataUpdater du = viewContractEntity.GetDataUpdater())
            {
                DataTable dt = new DataTable();
                du.Fill(ref dt);
                return dt;
            }
        }

        /// <summary>
        /// название организации по ссылке на id
        /// </summary>
        /// <param name="refOrganization"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private string GetOrganizationName(int refOrganization, IDatabase db)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Organizations_Plan);
            return db.ExecQuery(string.Format("select Name from {0} where id = {1}", entity.FullDBName, refOrganization),
                         QueryResultTypes.Scalar).ToString();
        }

        /// <summary>
        /// код окопф организации по ссылке на id
        /// </summary>
        /// <param name="refOrganization"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private string GetOrganizationOkopf(int refOrganization, IDatabase db)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Organizations_Plan);
            object queryResult = db.ExecQuery(string.Format("select OKOPF from {0} where id = {1}", entity.FullDBName, refOrganization),
                         QueryResultTypes.Scalar);
            if (queryResult == null || queryResult == DBNull.Value)
                return "0";
            return queryResult.ToString();
        }

        /// <summary>
        /// возвращает код валюты
        /// </summary>
        /// <param name="refOkv"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private string GetCurrencyCode(int refOkv, IDatabase db)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_OKV_Currency);
            return db.ExecQuery(string.Format("select CodeLetter from {0} where id = {1}", entity.FullDBName, refOkv),
                QueryResultTypes.Scalar).ToString();
        }

        private string GetCurrencyNominal(int refOkv, IDatabase db)
        {
            if (refOkv == -1)
                return "1";
            IEntity entity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_S_ExchangeRate);
            return db.ExecQuery(string.Format("select Nominal from {0} where RefOKV = {1}", entity.FullDBName, refOkv),
                QueryResultTypes.Scalar).ToString();
        }

        private string GetSupplyCredit(IDatabase db, IEntity entity, int creditId, string refColumnName)
        {
            object queryResult =
                db.ExecQuery(string.Format("select Min(FactDate) from {0} where {1} = {2}", entity.FullDBName, creditId, refColumnName), QueryResultTypes.Scalar);
            if (queryResult != null && queryResult != DBNull.Value)
                return Convert.ToDateTime(queryResult).ToShortDateString();
            return string.Empty;
        }

        private string GetPercent(IDatabase db, IEntity entity, int creditId, string refColumnName)
        {
            object queryResult =
                db.ExecQuery(string.Format("select CreditPercent from {0} where {1} = {2} and ChargeDate = (select Min(ChargeDate) from {0} where {1} = {2})",
                entity.FullDBName, refColumnName, creditId), QueryResultTypes.Scalar);
            if (queryResult != null && queryResult != DBNull.Value)
            {
                return string.Format("{0}%", queryResult);
            }
            return string.Empty;
        }

        /// <summary>
        /// курс валюты ближайший к указанной дате
        /// </summary>
        /// <param name="lastDate"></param>
        /// <param name="refOkv"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private decimal GetExchangeRate(DateTime lastDate, int refOkv, IDatabase db)
        {
            IEntity exchangeRate = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_S_ExchangeRate);
            string query = string.Format(
                    "select ExchangeRate from {0} where RefOKV = {1} and DateFixing = (select Max(DateFixing) from {0} where DateFixing <= ?)",
                    exchangeRate.FullDBName, refOkv);
            object queryResult = db.ExecQuery(query, QueryResultTypes.Scalar, db.CreateParameter("p0", lastDate));
            return queryResult == null || queryResult == DBNull.Value ? 0 : Convert.ToDecimal(queryResult);
        }

        /// <summary>
        /// сумма всех значений указанной детали до определенной даты. 
        /// </summary>
        /// <param name="masterRow"></param>
        /// <param name="reportDate"></param>
        /// <param name="detailEntity"></param>
        /// <param name="db"></param>
        /// <param name="refColumnName"></param>
        /// <param name="dateColumnName"></param>
        /// <returns></returns>
        private decimal GetDetailSum(DataRow masterRow, DateTime reportDate, IEntity detailEntity, IDatabase db, string refColumnName, string dateColumnName)
        {
            string sumColumnName = Convert.ToInt32(masterRow["RefOKV"]) == -1 ? "Sum" : "CurrencySum";
            string query = string.Format("select Sum({0}) from {1} where {2} = {3} and {4} <= ?",
                sumColumnName, detailEntity.FullDBName, refColumnName, masterRow["ID"], dateColumnName);
            object queryResult = db.ExecQuery(query, QueryResultTypes.Scalar, db.CreateParameter("p0", reportDate));
            return queryResult == null || queryResult == DBNull.Value ? 0 :
                Convert.ToDecimal(queryResult);
        }

        private decimal GetGuaranteeDetailSum(DataRow masterRow, DateTime reportDate, IEntity detailEntity, IDatabase db, string refColumnName, string dateColumnName, string addFilter)
        {
            string sumColumnName = Convert.ToInt32(masterRow["RefOKV"]) == -1 ? "Sum" : "CurrencySum";
            decimal sum = 0;
            string query = string.Format("select Sum({0}) from {1} where {2} = {3} and {4} <= ? and {5}",
                sumColumnName, detailEntity.FullDBName, refColumnName, masterRow["ID"], dateColumnName, addFilter);
            object queryResult = db.ExecQuery(query, QueryResultTypes.Scalar, db.CreateParameter("p0", reportDate));
            sum += queryResult == null || queryResult == DBNull.Value ? 0 :
                Convert.ToDecimal(queryResult);
            return sum;
        }

        private decimal GetGuaranteeDetailSum(DataRow masterRow, DateTime reportDate, IEntity detailEntity, IDatabase db, string refColumnName, string dateColumnName, params string[] sumColumns)
        {
            decimal sum = 0;
            foreach (string sumColumnName in sumColumns)
            {
                string query = string.Format("select Sum({0}) from {1} where {2} = {3} and {4} <= ?",
                                             sumColumnName, detailEntity.FullDBName, refColumnName, masterRow["ID"],
                                             dateColumnName);
                object queryResult = db.ExecQuery(query, QueryResultTypes.Scalar, db.CreateParameter("p0", reportDate));
                sum += queryResult == null || queryResult == DBNull.Value
                           ? 0
                           : Convert.ToDecimal(queryResult);
            }
            return sum;
        }

        /// <summary>
        /// получение данных из детали договоров
        /// </summary>
        /// <param name="dtContracts"></param>
        /// <param name="filter"></param>
        /// <param name="columnData"></param>
        /// <returns></returns>
        private string GetContractData(DataTable dtContracts, string filter, string columnData, params string[] contractTypeCodes)
        {
            List<string> values = new List<string>();
            foreach (DataRow row in dtContracts.Select(filter))
            {
                // проверяем, подпадает ли код записи под выборку
                bool useRowData = true;
                string contractCode = GetContractTypeCode(row["RefViewContract"].ToString());
                foreach (string code in contractTypeCodes)
                    if (string.Compare(code, contractCode) == 0)
                    {
                        useRowData = true;
                        break;
                    }
                    else
                        useRowData = false;
                if (!useRowData)
                    continue;
                // получаем данные
                if (row[columnData] is DateTime)
                    values.Add(Convert.ToDateTime(row[columnData]).ToShortDateString());
                else
                    values.Add(row[columnData].ToString());
            }
            return string.Join("^", values.ToArray());
        }

        /// <summary>
        /// вариант, по которому берем данные в долговой книге
        /// </summary>
        /// <param name="reportDate"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private int GetDebtBookVariant(DateTime reportDate, IDatabase db)
        {
            IEntity variant = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Variant_Schuldbuch);
            string query =
                string.Format("select id from {0} where ReportDate = (select Max(ReportDate) from {0} where ReportDate <= ?)",
                variant.FullDBName);
            object queryResult = db.ExecQuery(query, QueryResultTypes.Scalar, db.CreateParameter("p0", reportDate));
            return queryResult == null || queryResult == DBNull.Value ? -1 : Convert.ToInt32(queryResult);
        }

        private void GetCurrencySum(Dictionary<string, decimal> curencySum, ref decimal rubSum, ref decimal currencySum)
        {
            rubSum = 0;
            currencySum = 0;
            foreach (KeyValuePair<string, decimal> kvp in curencySum)
            {
                if (kvp.Key == "RUB")
                    rubSum += kvp.Value;
                else
                    currencySum += kvp.Value;
            }
        }

        private string GetFormatSum(decimal sum)
        {
            string sumStr = sum.ToString(CultureInfo.InvariantCulture).Contains(".")
                                        ? sum.ToString(CultureInfo.InvariantCulture)
                                        : string.Concat(sum.ToString(), ".00");
            return sumStr;
        }

        #endregion
    }
}

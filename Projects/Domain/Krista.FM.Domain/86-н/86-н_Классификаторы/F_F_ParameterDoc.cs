using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Krista.FM.Domain.MappingAttributes;

using Newtonsoft.Json;

namespace Krista.FM.Domain
{
    public class F_F_ParameterDoc : FactTable
    {
        public static readonly string Key = "0b2dffa6-80e4-49c3-9be8-9bf06f26de3e";
        private IList<F_ResultWork_CashPay> activityResultCashPay;
        private IList<F_ResultWork_CashReceipts> activityResultCashReceipts;
        private IList<F_ResultWork_FinNFinAssets> activityResultFinNFin;
        private IList<F_ResultWork_ShowService> activityResultServices;
        private IList<F_F_ShowService2016> activityResultServices2016;
        private IList<F_ResultWork_Staff> activityResultStaff;
        private IList<F_ResultWork_UseProperty> activityResultUse;
        private IList<F_Fin_CapFunds> capitalConstructionFunds;
        private IList<F_Doc_Docum> documents;
        private IList<F_Fin_finActPlan> financialActivityPlan;
        private IList<F_Fact_InspectionEvent> inspectionEvent;
        private IList<F_Fin_othGrantFunds> otherGrantFunds;
        private IList<F_Org_Passport> passports;
        private IList<F_Fin_realAssFunds> realAssetsFunds;
        private IList<F_Fin_Smeta> smetas;
        private IList<F_F_GosZadanie> stateTasks;
        private IList<F_F_GosZadanie2016> stateTasks2016;
        private IList<F_F_RequestAccount> stateTasksRequestAccounts;
        private IList<F_F_OrderControl> stateTasksSupervisionProcedures;
        private IList<F_F_BaseTermination> stateTasksTerminations;
        private IList<F_F_BaseTermination2016> stateTasksTerminations2016;
        private IList<F_F_OrderControl2016> stateTasksSupervisionProcedures2016;
        private IList<F_F_RequestAccount2016> stateTasksRequestAccounts2016;
        private IList<F_F_OtherInfo> stateTasksOtherInfos;
        private IList<F_F_Reports> stateTasksReportses;
        private IList<F_Report_HeadAttribute> reportHeadAttribute;
        private IList<F_Report_Bal0503121> annualBalanceF0503121;
        private IList<F_Report_Bal0503127> annualBalanceF0503127;
        private IList<F_Report_BalF0503130> annualBalanceF0503130;
        private IList<F_Report_BalF0503137> annualBalanceF0503137;
        private IList<F_Report_BalF0503721> annualBalanceF0503721;
        private IList<F_Report_BalF0503730> annualBalanceF0503730;
        private IList<F_Report_BalF0503737> annualBalanceF0503737;
        private IList<T_F_TofkList> tofkList;
        private IList<T_F_PaymentDetails> paymentDetails;
        private IList<T_F_LicenseDetails> licenseDetails;
        private IList<T_F_AccreditationDetails> accreditationDetails;
        private IList<T_F_ExtHeader> extHeader;
        private IList<F_F_FinancialIndex> financialIndex;
        private IList<F_F_TemporaryResources> temporaryResources;
        private IList<F_F_Reference> reference;
        private IList<F_Fin_ExpensePaymentIndex> expensePaymentIndex;
        private IList<F_F_PlanPaymentIndex> planPaymentIndex;

        public F_F_ParameterDoc()
        {
            documents = new List<F_Doc_Docum>();

            passports = new List<F_Org_Passport>();
            smetas = new List<F_Fin_Smeta>();

            stateTasks = new List<F_F_GosZadanie>();
            stateTasks2016 = new List<F_F_GosZadanie2016>();
            stateTasksRequestAccounts = new List<F_F_RequestAccount>();
            stateTasksSupervisionProcedures = new List<F_F_OrderControl>();
            stateTasksTerminations = new List<F_F_BaseTermination>();
            stateTasksTerminations2016 = new List<F_F_BaseTermination2016>();
            stateTasksSupervisionProcedures2016 = new List<F_F_OrderControl2016>();
            stateTasksRequestAccounts2016 = new List<F_F_RequestAccount2016>();
            stateTasksOtherInfos = new List<F_F_OtherInfo>();
            stateTasksReportses = new List<F_F_Reports>();

            otherGrantFunds = new List<F_Fin_othGrantFunds>();
            realAssetsFunds = new List<F_Fin_realAssFunds>();
            capitalConstructionFunds = new List<F_Fin_CapFunds>();

            financialActivityPlan = new List<F_Fin_finActPlan>();

            activityResultStaff = new List<F_ResultWork_Staff>();
            activityResultUse = new List<F_ResultWork_UseProperty>();
            activityResultFinNFin = new List<F_ResultWork_FinNFinAssets>();
            activityResultServices = new List<F_ResultWork_ShowService>();
            activityResultCashReceipts = new List<F_ResultWork_CashReceipts>();
            activityResultCashPay = new List<F_ResultWork_CashPay>();
            activityResultServices2016 = new List<F_F_ShowService2016>();

            inspectionEvent = new List<F_Fact_InspectionEvent>();

            reportHeadAttribute = new List<F_Report_HeadAttribute>();
            annualBalanceF0503121 = new List<F_Report_Bal0503121>();
            annualBalanceF0503127 = new List<F_Report_Bal0503127>();
            annualBalanceF0503130 = new List<F_Report_BalF0503130>();
            annualBalanceF0503137 = new List<F_Report_BalF0503137>();
            annualBalanceF0503721 = new List<F_Report_BalF0503721>();
            annualBalanceF0503730 = new List<F_Report_BalF0503730>();
            annualBalanceF0503737 = new List<F_Report_BalF0503737>();

            tofkList = new List<T_F_TofkList>();
            paymentDetails = new List<T_F_PaymentDetails>();
            licenseDetails = new List<T_F_LicenseDetails>();
            accreditationDetails = new List<T_F_AccreditationDetails>();

            extHeader = new List<T_F_ExtHeader>();

            financialIndex = new List<F_F_FinancialIndex>();
            temporaryResources = new List<F_F_TemporaryResources>();
            reference = new List<F_F_Reference>();
            expensePaymentIndex = new List<F_Fin_ExpensePaymentIndex>();
            planPaymentIndex = new List<F_F_PlanPaymentIndex>();
        }

        public virtual int SourceID { get; set; }

        public virtual int TaskID { get; set; }

        public virtual string Note { get; set; }

        public virtual bool PlanThreeYear { get; set; }

        public virtual FX_FX_PartDoc RefPartDoc { get; set; }

        public virtual FX_Org_SostD RefSost { get; set; }

        public virtual D_Org_Structure RefUchr { get; set; }

        public virtual FX_Fin_YearForm RefYearForm { get; set; }

        public virtual DateTime? OpeningDate { get; set; }

        public virtual DateTime? CloseDate { get; set; }

        public virtual DateTime? FormationDate { get; set; }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_Doc_Docum> Documents
        {
            get { return documents; }
            set { documents = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_Org_Passport> Passports
        {
            get { return passports; }
            set { passports = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_Fin_Smeta> Smetas
        {
            get { return smetas; }
            set { smetas = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_Fact_InspectionEvent> InspectionEvent
        {
            get { return inspectionEvent; }
            set { inspectionEvent = value; }
        }

        #region Государственное задание

        /// <summary>
        /// следствие ошибки при проектировании заголовка документов
        /// данные гз теперь вынужденно вынесены на уровень заголовка
        /// </summary>
        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_F_GosZadanie> StateTasks
        {
            get { return stateTasks; }
            set { stateTasks = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParameter")]
        public virtual IList<F_F_GosZadanie2016> StateTasks2016
        {
            get { return stateTasks2016; }
            set { stateTasks2016 = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefFactGZ")]
        public virtual IList<F_F_BaseTermination> StateTasksTerminations
        {
            get { return stateTasksTerminations; }
            set { stateTasksTerminations = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefFactGZ")]
        public virtual IList<F_F_OrderControl> StateTasksSupervisionProcedures
        {
            get { return stateTasksSupervisionProcedures; }
            set { stateTasksSupervisionProcedures = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefFactGZ")]
        public virtual IList<F_F_RequestAccount> StateTasksRequestAccounts
        {
            get { return stateTasksRequestAccounts; }
            set { stateTasksRequestAccounts = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefFactGZ")]
        public virtual IList<F_F_BaseTermination2016> StateTasksTerminations2016
        {
            get { return stateTasksTerminations2016; }
            set { stateTasksTerminations2016 = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefFactGZ")]
        public virtual IList<F_F_OrderControl2016> StateTasksSupervisionProcedures2016
        {
            get { return stateTasksSupervisionProcedures2016; }
            set { stateTasksSupervisionProcedures2016 = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefFactGZ")]
        public virtual IList<F_F_RequestAccount2016> StateTasksRequestAccounts2016
        {
            get { return stateTasksRequestAccounts2016; }
            set { stateTasksRequestAccounts2016 = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefFactGZ")]
        public virtual IList<F_F_OtherInfo> StateTasksOtherInfo
        {
            get { return stateTasksOtherInfos; }
            set { stateTasksOtherInfos = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefFactGZ")]
        public virtual IList<F_F_Reports> StateTasksReports
        {
            get { return stateTasksReportses; }
            set { stateTasksReportses = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<T_F_ExtHeader> StateTasksExtHeader
        {
            get { return extHeader; }
            set { extHeader = value; }
        }

        #endregion

        #region ПФХД

        #region ActionGrant

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_Fin_othGrantFunds> OtherGrantFundses
        {
            get { return otherGrantFunds; }
            set { otherGrantFunds = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_Fin_realAssFunds> RealAssFundses
        {
            get { return realAssetsFunds; }
            set { realAssetsFunds = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_Fin_CapFunds> CapitalConstructionFundses
        {
            get { return capitalConstructionFunds; }
            set { capitalConstructionFunds = value; }
        }

        #endregion

        #region FinancialActivityPlan

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_Fin_finActPlan> FinancialActivityPlan
        {
            get { return financialActivityPlan; }
            set { financialActivityPlan = value; }
        }

        #endregion

        #endregion

        #region Информация о результатах деятельности и об использовании имущества

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_ResultWork_Staff> ActivityResultStaff
        {
            get { return activityResultStaff; }
            set { activityResultStaff = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_ResultWork_UseProperty> ActivityResultUse
        {
            get { return activityResultUse; }
            set { activityResultUse = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_ResultWork_FinNFinAssets> ActivityResultFinNFin
        {
            get { return activityResultFinNFin; }
            set { activityResultFinNFin = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_ResultWork_ShowService> ActivityResultServices
        {
            get { return activityResultServices; }
            set { activityResultServices = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_F_ShowService2016> ActivityResultServices2016
        {
            get { return activityResultServices2016; }
            set { activityResultServices2016 = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_ResultWork_CashReceipts> ActivityResultCashReceipts
        {
            get { return activityResultCashReceipts; }
            set { activityResultCashReceipts = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_ResultWork_CashPay> ActivityResultCashPay
        {
            get { return activityResultCashPay; }
            set { activityResultCashPay = value; }
        }

        #endregion

        #region Бухгалтерская отчетность

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_Report_HeadAttribute> ReportHeadAttribute
        {
            get { return reportHeadAttribute; }
            set { reportHeadAttribute = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_Report_Bal0503121> AnnualBalanceF0503121
        {
            get { return annualBalanceF0503121; }
            set { annualBalanceF0503121 = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_Report_Bal0503127> AnnualBalanceF0503127
        {
            get { return annualBalanceF0503127; }
            set { annualBalanceF0503127 = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_Report_BalF0503130> AnnualBalanceF0503130
        {
            get { return annualBalanceF0503130; }
            set { annualBalanceF0503130 = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_Report_BalF0503137> AnnualBalanceF0503137
        {
            get { return annualBalanceF0503137; }
            set { annualBalanceF0503137 = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_Report_BalF0503721> AnnualBalanceF0503721
        {
            get { return annualBalanceF0503721; }
            set { annualBalanceF0503721 = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_Report_BalF0503730> AnnualBalanceF0503730
        {
            get { return annualBalanceF0503730; }
            set { annualBalanceF0503730 = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_Report_BalF0503737> AnnualBalanceF0503737
        {
            get { return annualBalanceF0503737; }
            set { annualBalanceF0503737 = value; }
        }

        #endregion

        #region Иная информация об учреждении

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParameterDoc")]
        public virtual IList<T_F_TofkList> TofkList
        {
            get { return tofkList; }
            set { tofkList = value; }
        }
        
        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParameterDoc")]
        public virtual IList<T_F_PaymentDetails> PaymentDetails
        {
            get { return paymentDetails; }
            set { paymentDetails = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParameterDoc")]
        public virtual IList<T_F_LicenseDetails> LicenseDetails
        {
            get { return licenseDetails; }
            set { licenseDetails = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParameterDoc")]
        public virtual IList<T_F_AccreditationDetails> AccreditationDetails
        {
            get { return accreditationDetails; }
            set { accreditationDetails = value; }
        }

        #endregion

        #region ПФХД2017
        
        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_F_FinancialIndex> FinancialIndex
        {
            get { return financialIndex; }
            set { financialIndex = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_F_TemporaryResources> TemporaryResources
        {
            get { return temporaryResources; }
            set { temporaryResources = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_F_Reference> Reference
        {
            get { return reference; }
            set { reference = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_Fin_ExpensePaymentIndex> ExpensePaymentIndex
        {
            get { return expensePaymentIndex; }
            set { expensePaymentIndex = value; }
        }

        [IgnoreDataMember]
        [JsonIgnore]
        [ReferenceField("RefParametr")]
        public virtual IList<F_F_PlanPaymentIndex> PlanPaymentIndex
        {
            get { return planPaymentIndex; }
            set { planPaymentIndex = value; }
        }

        #endregion
    }
}

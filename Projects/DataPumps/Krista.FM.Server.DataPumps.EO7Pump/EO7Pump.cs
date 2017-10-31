using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ADODB;
using System.Xml;
using System.Runtime.Remoting;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Text.RegularExpressions;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;
using DataExchangeCoreTLB;
using MSXML2;
using ExchangePacketDummyLibrary;
using UDataExchangeServer;

namespace Krista.FM.Server.DataPumps.EO7Pump
{
    // ЭО 7 - закачка ГосЗаказа
    public class EO7PumpModule : CorrectedPumpModuleBase
    {

        #region поля

        #region классификаторы

        // Госзаказ.Заявки на закупку (d.StOrder.Request)
        private IDbDataAdapter daRequest;
        private DataSet dsRequest;
        private IClassifier clsRequest;
        private Dictionary<int, DataRow> cacheRequest = null;
        private int nullRequest = -1;
        // Госзаказ.Период размещения заказа (d.StOrder.RegPeriod)
        private IDbDataAdapter daRegPeriod;
        private DataSet dsRegPeriod;
        private IClassifier clsRegPeriod;
        private Dictionary<int, DataRow> cacheRegPeriod = null;
        private int nullRegPeriod = -1;
        // Госзаказ.Строки заявок заказчиков (d.StOrder.AskItems)
        private IDbDataAdapter daAskItems;
        private DataSet dsAskItems;
        private IClassifier clsAskItems;
        private Dictionary<int, DataRow> cacheAskItems = null;
        private int nullAskItems = -1;
        // Госзаказ.Каталог продукции (d.StOrder.Product)
        private IDbDataAdapter daProduct;
        private DataSet dsProduct;
        private IClassifier clsProduct;
        private Dictionary<int, DataRow> cacheProduct = null;
        private int nullProduct = -1;
        // Госзаказ.Единицы измерения (d.StOrder.Units)
        private IDbDataAdapter daUnits;
        private DataSet dsUnits;
        private IClassifier clsUnits;
        private Dictionary<int, DataRow> cacheUnits = null;
        private int nullUnits = -1;
        // Госзаказ.ОКДП (d.StOrder.OKDP)
        private IDbDataAdapter daOkdp;
        private DataSet dsOkdp;
        private IClassifier clsOkdp;
        private Dictionary<int, DataRow> cacheOkdp = null;
        private int nullOkdp = -1;
        // Госзаказ.ОКП (d.StOrder.OKP)
        private IDbDataAdapter daOkp;
        private DataSet dsOkp;
        private IClassifier clsOkp;
        private Dictionary<int, DataRow> cacheOkp = null;
        private int nullOkp = -1;
        // Госзаказ.Номенклатура продукции (d.StOrder.RangeProduct)
        private IDbDataAdapter daRangeProduct;
        private DataSet dsRangeProduct;
        private IClassifier clsRangeProduct;
        private Dictionary<int, DataRow> cacheRangeProduct = null;
        private int nullRangeProduct = -1;
        // Госзаказ.КВР (d.StOrder.KVR)
        private IDbDataAdapter daKvr;
        private DataSet dsKvr;
        private IClassifier clsKvr;
        private Dictionary<int, DataRow> cacheKvr = null;
        private int nullKvr = -1;
        // Госзаказ.ЭКР (d.StOrder.EKR)
        private IDbDataAdapter daEkr;
        private DataSet dsEkr;
        private IClassifier clsEkr;
        private Dictionary<int, DataRow> cacheEkr = null;
        private int nullEkr = -1;
        // Госзаказ.Мероприятия (d.StOrder.Measure)
        private IDbDataAdapter daMeasure;
        private DataSet dsMeasure;
        private IClassifier clsMeasure;
        private Dictionary<int, DataRow> cacheMeasure = null;
        private int nullMeasure = -1;
        // Госзаказ.Распределение финансирования (d.StOrder.FinanceDistribution)
        private IDbDataAdapter daFinDistr;
        private DataSet dsFinDistr;
        private IClassifier clsFinDistr;
        private Dictionary<int, DataRow> cacheFinDistr = null;
        // Госзаказ.КИФ (d.StOrder.KIF)
        private IDbDataAdapter daKif;
        private DataSet dsKif;
        private IClassifier clsKif;
        private Dictionary<int, DataRow> cacheKif = null;
        private int nullKif = -1;
        // Госзаказ.Организации (d.StOrder.Organization)
        private IDbDataAdapter daOrg;
        private DataSet dsOrg;
        private IClassifier clsOrg;
        private Dictionary<int, DataRow> cacheOrg = null;
        private int nullOrg = -1;
        // Госзаказ.Закупки (d.StOrder.Tender)
        private IDbDataAdapter daTender;
        private DataSet dsTender;
        private IClassifier clsTender;
        private Dictionary<int, DataRow> cacheTender = null;
        private int nullTender = -1;
        // Госзаказ.Лоты (d.StOrder.Lot)
        private IDbDataAdapter daLot;
        private DataSet dsLot;
        private IClassifier clsLot;
        private Dictionary<int, DataRow> cacheLot = null;
        private int nullLot = -1;
        // Госзаказ.Строки лотов (d.StOrder.LotItems)
        private IDbDataAdapter daLotItems;
        private DataSet dsLotItems;
        private IClassifier clsLotItems;
        private Dictionary<int, DataRow> cacheLotItems = null;
        private int nullLotItems = -1;
        // Госзаказ.Предложения поставщиков (d.StOrder.Offer)
        private IDbDataAdapter daOffer;
        private DataSet dsOffer;
        private IClassifier clsOffer;
        private Dictionary<int, DataRow> cacheOffer = null;
        private int nullOffer = -1;
        // Госзаказ.Строки предложений поставщиков (d.StOrder.BidItems)
        private IDbDataAdapter daBidItems;
        private DataSet dsBidItems;
        private IClassifier clsBidItems;
        private Dictionary<int, DataRow> cacheBidItems = null;
        // Госзаказ.Параметры продукции (d.StOrder.ParameterItems)
        private IDbDataAdapter daParameterItems;
        private DataSet dsParameterItems;
        private IClassifier clsParameterItems;
        private Dictionary<int, DataRow> cacheParameterItems = null;
        // Госзаказ.Контракты (d.StOrder.Contract)
        private IDbDataAdapter daContract;
        private DataSet dsContract;
        private IClassifier clsContract;
        private Dictionary<int, DataRow> cacheContract = null;
        private int nullContract = -1;
        // Госзаказ.Строки контрактов (d.StOrder.ContractItems)
        private IDbDataAdapter daContractItems;
        private DataSet dsContractItems;
        private IClassifier clsContractItems;
        private Dictionary<int, DataRow> cacheContractItems = null;
        // Госзаказ.Состояния документов (d.StOrder.DocumentsStatus)
        private IDbDataAdapter daDocStatus;
        private DataSet dsDocStatus;
        private IClassifier clsDocStatus;
        private Dictionary<int, DataRow> cacheDocStatus = null;
        private int nullDocStatus = -1;
        // Госзаказ.Типы документов (d_StOrder_DocType)
        private IDbDataAdapter daDocType;
        private DataSet dsDocType;
        private IClassifier clsDocType;
        private Dictionary<int, DataRow> cacheDocType = null;
        private int nullDocType = -1;
        // Госзаказ.ОКАТО (d_StOrder_OKATO)
        private IDbDataAdapter daOkato;
        private DataSet dsOkato;
        private IClassifier clsOkato;
        private Dictionary<int, DataRow> cacheOkato = null;
        private int nullOkato = -1;
        // Госзаказ.Протоколы (d_StOrder_Protocol)
        private IDbDataAdapter daProtocol;
        private DataSet dsProtocol;
        private IClassifier clsProtocol;
        private Dictionary<int, DataRow> cacheProtocol = null;
        private int nullProtocol = -1;
        // Госзаказ.Решения комиссии (d_StOrder_ResolvedLot)
        private IDbDataAdapter daResolvedLot;
        private DataSet dsResolvedLot;
        private IClassifier clsResolvedLot;
        private Dictionary<int, DataRow> cacheResolvedLot = null;
        private int nullResolvedLot = -1;
        // Госзаказ.Документация к закупке (d_StOrder_TenderFiles)
        private IDbDataAdapter daTenderFiles;
        private DataSet dsTenderFiles;
        private IClassifier clsTenderFiles;
        private Dictionary<int, DataRow> cacheTenderFiles = null;
        // Госзаказ.Пользователи официального сайта (d_StOrder_SiteUser)
        private IDbDataAdapter daSiteUser;
        private DataSet dsSiteUser;
        private IClassifier clsSiteUser;
        private Dictionary<int, DataRow> cacheSiteUser = null;
        private int nullSiteUser = -1;
        // Госзаказ.Группы пользователей (d_StOrder_SiteGroup)
        private IDbDataAdapter daSiteGroup;
        private DataSet dsSiteGroup;
        private IClassifier clsSiteGroup;
        private Dictionary<int, DataRow> cacheSiteGroup = null;
        private int nullSiteGroup = -1;
        // Госзаказ.Вхождение в группы (d_StOrder_SiteGroupMemberShip)
        private IDbDataAdapter daSiteGroupMember;
        private DataSet dsSiteGroupMember;
        private IClassifier clsSiteGroupMember;
        private Dictionary<int, DataRow> cacheSiteGroupMember = null;
        private int nullSiteGroupMember = -1;
        // Госзаказ.Разрешения на смену состояний (d_StOrder_SiteStatePermission)
        private IDbDataAdapter daSiteStatePermission;
        private DataSet dsSiteStatePermission;
        private IClassifier clsSiteStatePermission;
        private Dictionary<int, DataRow> cacheSiteStatePermission = null;
        private int nullSiteStatePermission = -1;
        // Госзаказ.Категории организаций (d_StOrder_CategoryOrganization)
        private IDbDataAdapter daCategoryOrg;
        private DataSet dsCategoryOrg;
        private IClassifier clsCategoryOrg;
        private Dictionary<int, DataRow> cacheCategoryOrg = null;
        private int nullCategoryOrg = -1;
        // Мониторинг.Основание по закону (d_StOrder_CauseOnAct)
        private IDbDataAdapter daCauseOnAct;
        private DataSet dsCauseOnAct;
        private IClassifier clsCauseOnAct;
        private Dictionary<int, DataRow> cacheCauseOnAct = null;
        private int nullCauseOnAct = -1;
        // Мониторинг.Сведения об ИспПрекрДействия контракта (d_StOrder_OrderExecution)
        private IDbDataAdapter daOrderExec;
        private DataSet dsOrderExec;
        private IClassifier clsOrderExec;
        private Dictionary<int, DataRow> cacheOrderExec = null;
        private int nullOrderExec = -1;
        // Мониторинг.Финансирование контрактов (d_StOrder_SponsorshipContract)
        private IDbDataAdapter daSponsor;
        private DataSet dsSponsor;
        private IClassifier clsSponsor;
        private Dictionary<int, DataRow> cacheSponsor = null;
        private int nullSponsor = -1;
        // Мониторинг.Лоты поставщиков для закупки (d_StOrder_PurchaseLotBids)
        private IDbDataAdapter daLotBids;
        private DataSet dsLotBids;
        private IClassifier clsLotBids;
        private Dictionary<int, DataRow> cacheLotBids = null;
        private int nullLotBids = -1;
        // Мониторинг.Документы исполнения (d_StOrder_Agreements)
        private IDbDataAdapter daAgreements;
        private DataSet dsAgreements;
        private IClassifier clsAgreements;
        private Dictionary<int, DataRow> cacheAgreements = null;
        private int nullAgreements = -1;
        // Мониторинг.Территории (d_StOrder_Territory)
        private IDbDataAdapter daTerr;
        private DataSet dsTerr;
        private IClassifier clsTerr;
        private Dictionary<int, DataRow> cacheTerr = null;
        // Мониторинг.ЭО_Мониторинг_Закупки малого объема (f_StOrder_TenderLowSize)
        private IDbDataAdapter daTenderLowSize;
        private DataSet dsTenderLowSize;
        private IFactTable fctTenderLowSize;
        private Dictionary<int, DataRow> cacheTenderLowSize = null;

        #endregion классификаторы

        private int refTerr = -1;
        private ExchangePacketInboundDummyClass inBoundPacketDummy = null;

        // регулярное выражение - используется при обработке всех значений (любое кол - во разделителей)
        Regex regExSeparators = new Regex("\\s+");

        DataExchangeServiceClassClass exchangeService = null;

        #endregion поля

        #region константы 

        #region имена сущностей (для обменника)

        private const string REQUEST_ENTITY_NAME = "D.STORDER.REQUEST";
        private const string REG_PERIOD_ENTITY_NAME = "D.STORDER.REGPERIOD";
        private const string ASK_ITEMS_ENTITY_NAME = "D.STORDER.ASKITEMS";
        private const string PRODUCT_ENTITY_NAME = "D.STORDER.PRODUCT";
        private const string UNITS_ENTITY_NAME = "D.STORDER.UNITS";
        private const string OKDP_ENTITY_NAME = "D.STORDER.OKDP";
        private const string OKP_ENTITY_NAME = "D.STORDER.OKP";
        private const string RANGE_PRODUCT_ENTITY_NAME = "D.STORDER.RANGEPRODUCT";
        private const string KVR_ENTITY_NAME = "D.STORDER.KVR";
        private const string EKR_ENTITY_NAME = "D.STORDER.EKR";
        private const string MEASURE_ENTITY_NAME = "D.STORDER.MEASURE";
        private const string FIN_DISTR_ENTITY_NAME = "D.STORDER.FINANCEDISTRIBUTION";
        private const string KIF_ENTITY_NAME = "D.STORDER.KIF";
        private const string ORG_ENTITY_NAME = "D.STORDER.ORGANIZATION";
        private const string TENDER_ENTITY_NAME = "D.STORDER.TENDER";
        private const string LOT_ENTITY_NAME = "D.STORDER.LOT";
        private const string LOT_ITEMS_ENTITY_NAME = "D.STORDER.LOTITEMS";
        private const string OFFER_ENTITY_NAME = "D.STORDER.OFFER";
        private const string BID_ITEMS_ENTITY_NAME = "D.STORDER.BIDITEMS";
        private const string PARAMETER_ITEMS_ENTITY_NAME = "D.STORDER.PARAMETERITEMS";
        private const string CONTRACT_ENTITY_NAME = "D.STORDER.CONTRACT";
        private const string CONTRACT_ITEMS_ENTITY_NAME = "D.STORDER.CONTRACTITEMS";
        private const string DOC_STATUS_ENTITY_NAME = "FX.FX.STATES";
        private const string DOC_TYPE_ENTITY_NAME = "D.STORDER.DOCTYPE";
        private const string OKATO_ENTITY_NAME = "D.STORDER.OKATO";
        private const string PROTOCOL_ENTITY_NAME = "D.STORDER.PROTOCOL";
        private const string RESOLVED_LOT_ENTITY_NAME = "D.STORDER.RESOLVEDLOT";
        private const string TENDER_FILES_ENTITY_NAME = "D.STORDER.TENDERFILES";
        private const string SITE_USER_ENTITY_NAME = "D.STORDER.SITEUSER";
        private const string SITE_GROUP_ENTITY_NAME = "D.STORDER.SITEGROUP";
        private const string SITE_GROUP_MEMBER_ENTITY_NAME = "D.STORDER.SITEGROUPMEMBERSHIP";
        private const string SITE_STATE_PERMISSION_ENTITY_NAME = "D.STORDER.SITESTATEPERMISSION";
        private const string CATEGORY_ORG_ENTITY_NAME = "D.STORDER.CATEGORYORGANIZATION";
        private const string CAUSE_ON_ACT_ENTITY_NAME = "D.STORDER.CAUSEONACT";
        private const string ORDER_EXEC_ENTITY_NAME = "D.STORDER.ORDEREXECUTION";
        private const string SPONSOR_ENTITY_NAME = "D.STORDER.SPONSORSHIPCONTRACT";
        private const string LOT_BIDS_ENTITY_NAME = "D.STORDER.PURCHASELOTBIDS";
        private const string AGREEMENTS_ENTITY_NAME = "D.STORDER.AGREEMENTS";
        private const string TERR_ENTITY_NAME = "D.STORDER.SITEAUTOMATIONSUBJECTS";
        private const string TENDER_LOW_SIZE_ENTITY_NAME = "F.STORDER.TENDERLOWSIZE";

        #endregion имена сущностей (для обменника)

        #endregion константы

        #region закачка данных

        #region работа с базой и кэшами

        #region GUID

        private const string D_REQUEST_GUID = "9523e53c-09f7-4135-aba1-94e91ca8e3c6";
        private const string D_REG_PERIOD_GUID = "3c7aff61-814a-46b2-a337-dc048d013114";
        private const string D_ASK_ITEMS_GUID = "2f59df06-aec4-424b-8dda-380b6cb7b9b6";
        private const string D_PRODUCT_GUID = "e134f3b4-7f9c-4e13-b146-e0ff6a54f724";
        private const string D_UNITS_GUID = "6b07f538-0d77-4b77-b76a-4a4fa4a378be";
        private const string D_OKDP_GUID = "5de0589b-2e8c-4842-93c5-96e30d331846";
        private const string D_OKP_GUID = "3ca397aa-2edb-4652-9734-71bb7641ecd0";
        private const string D_RANGE_PRODUCT_GUID = "6d81d937-ed7d-456c-8507-286518202eb4";
        private const string D_KVR_GUID = "039961f6-62a3-48e4-8d1e-23ac4ed9b1f1";
        private const string D_EKR_GUID = "a899b9f7-87d6-4d59-992c-819c99ad0a70";
        private const string D_MEASURE_GUID = "7d14a06d-84b9-4cc8-ae41-83d61e37df1d";
        private const string D_FIN_DISTR_GUID = "7c1264db-68c6-4669-84bd-0a575d86cc9d";
        private const string D_KIF_GUID = "57f0eb84-e614-4e65-b825-ccfede3e7c40";
        private const string D_ORG_GUID = "0cce5ec1-fbf2-452f-8db2-168376c76bcd";
        private const string D_TENDER_GUID = "b5066d9d-ab58-4c96-ab03-060fee52f562";
        private const string D_LOT_GUID = "b54155a9-7bec-48b3-95c5-104aefce5e02";
        private const string D_LOT_ITEMS_GUID = "b0db161c-c330-4d1f-a57d-80fdb7004a32";
        private const string D_OFFER_GUID = "1e2894d3-eadc-44d3-b142-d9658e4f3b4c";
        private const string D_BID_ITEMS_GUID = "66fff387-21de-4794-b477-f371a401abb9";
        private const string D_PARAMETER_ITEMS_GUID = "c5d827ca-b33a-416e-ba4e-0bb4af667a6f";
        private const string D_CONTRACT_GUID = "39d001cf-f0eb-4e63-88f4-7e835aee403c";
        private const string D_CONTRACT_ITEMS_GUID = "90bc1fda-d546-4b61-89c9-411abec91324";
        private const string D_DOC_STATUS_GUID = "29197602-24b1-47e6-910d-15829ae5109c";
        private const string D_DOC_TYPE_GUID = "085fca41-34e6-4711-a312-28d7732c2559";
        private const string D_OKATO_GUID = "5e511071-8487-448a-ba74-ed9c9d05df29";
        private const string D_PROTOCOL_GUID = "f04eee44-31b6-447f-956e-82f3a3a48609";
        private const string D_RESOLVED_LOT_GUID = "e4122971-f64f-4e7f-8b4f-e5fc1334b8c4";
        private const string D_TENDER_FILES_GUID = "8178a3ab-0038-4b07-b6b3-1f5f9c411c16";
        private const string D_SITE_USER_GUID = "ec0c6586-de57-4f5f-bf19-2142b3c2741b";
        private const string D_SITE_GROUP_GUID = "f49b15bb-568c-413a-8ea0-317f14094449";
        private const string D_SITE_GROUP_MEMBER_GUID = "3f536561-6359-4fca-a9a7-f0418c6d1e53";
        private const string D_SITE_STATE_PERMISSION_GUID = "7ea40053-c3e2-4052-8b37-d011c53d46f4";
        private const string D_CATEGORY_ORG_GUID = "41b15185-ae87-4a9d-8080-c8439d7759ed";
        private const string D_CAUSE_ON_ACT_GUID = "aba81e9b-2366-4f41-803b-e01d4561b989";
        private const string D_ORDER_EXEC_GUID = "a9037960-3742-4269-8279-33d3e5fdfb83";
        private const string D_SPONSOR_GUID = "0b2cf244-2da8-489f-99fc-26e03a0dd821";
        private const string D_LOT_BIDS_GUID = "249e4b6f-f791-4fc3-bd76-65de3e9f8998";
        private const string D_AGREEMENTS_GUID = "c6a59d4d-f6d9-4b33-a2ae-5e92817cfbcb";
        private const string D_TERR_GUID = "ce940102-9e33-40f0-8a6a-989fd6e09534";
        private const string F_TENDER_LOW_SIZE_GUID = "4f9b6a0d-3f64-42e6-971d-ffd32fc08789";

        #endregion GUID

        protected override void InitDBObjects()
        {
            clsDocStatus = this.Scheme.Classifiers[D_DOC_STATUS_GUID];
            clsDocType = this.Scheme.Classifiers[D_DOC_TYPE_GUID];
            clsOkato = this.Scheme.Classifiers[D_OKATO_GUID];
            clsTenderFiles = this.Scheme.Classifiers[D_TENDER_FILES_GUID];
            clsUnits = this.Scheme.Classifiers[D_UNITS_GUID];
            clsOkp = this.Scheme.Classifiers[D_OKP_GUID];
            clsOkdp = this.Scheme.Classifiers[D_OKDP_GUID];
            clsKvr = this.Scheme.Classifiers[D_KVR_GUID];
            clsKif = this.Scheme.Classifiers[D_KIF_GUID];
            clsTerr = this.Scheme.Classifiers[D_TERR_GUID];

            this.UsedClassifiers = new IClassifier[] {
                clsBidItems = this.Scheme.Classifiers[D_BID_ITEMS_GUID],
                clsAskItems = this.Scheme.Classifiers[D_ASK_ITEMS_GUID], 
                clsContractItems = this.Scheme.Classifiers[D_CONTRACT_ITEMS_GUID],
                clsFinDistr = this.Scheme.Classifiers[D_FIN_DISTR_GUID], 
                clsResolvedLot = this.Scheme.Classifiers[D_RESOLVED_LOT_GUID],
                clsLotItems = this.Scheme.Classifiers[D_LOT_ITEMS_GUID], 
                clsLot = this.Scheme.Classifiers[D_LOT_GUID], 
                clsOffer = this.Scheme.Classifiers[D_OFFER_GUID],
                clsProtocol = this.Scheme.Classifiers[D_PROTOCOL_GUID], 
                clsTender = this.Scheme.Classifiers[D_TENDER_GUID], 
                clsOrderExec = this.Scheme.Classifiers[D_ORDER_EXEC_GUID], 
                clsSponsor = this.Scheme.Classifiers[D_SPONSOR_GUID], 
                clsContract = this.Scheme.Classifiers[D_CONTRACT_GUID],
                clsRequest = this.Scheme.Classifiers[D_REQUEST_GUID], 
                clsSiteUser = this.Scheme.Classifiers[D_SITE_USER_GUID],
                clsSiteGroupMember = this.Scheme.Classifiers[D_SITE_GROUP_MEMBER_GUID],
                clsSiteGroup = this.Scheme.Classifiers[D_SITE_GROUP_GUID],
                clsCategoryOrg = this.Scheme.Classifiers[D_CATEGORY_ORG_GUID],
                clsAgreements = this.Scheme.Classifiers[D_AGREEMENTS_GUID],
                clsOrg = this.Scheme.Classifiers[D_ORG_GUID], 
                clsParameterItems = this.Scheme.Classifiers[D_PARAMETER_ITEMS_GUID],
                clsProduct = this.Scheme.Classifiers[D_PRODUCT_GUID], 
                clsRangeProduct = this.Scheme.Classifiers[D_RANGE_PRODUCT_GUID], 
                clsCauseOnAct = this.Scheme.Classifiers[D_CAUSE_ON_ACT_GUID],
                clsSiteStatePermission = this.Scheme.Classifiers[D_SITE_STATE_PERMISSION_GUID],
                clsLotBids = this.Scheme.Classifiers[D_LOT_BIDS_GUID] };


            this.AssociateClassifiers = (IClassifier[])CommonRoutines.ConcatArrays(this.UsedClassifiers,
                new IClassifier[] {
                    clsEkr = this.Scheme.Classifiers[D_EKR_GUID],
                    clsMeasure = this.Scheme.Classifiers[D_MEASURE_GUID],
                    clsRegPeriod = this.Scheme.Classifiers[D_REG_PERIOD_GUID] });

            this.CubeClassifiers = this.AssociateClassifiers;

            this.UsedFacts = new IFactTable[] { fctTenderLowSize = this.Scheme.FactTables[F_TENDER_LOW_SIZE_GUID] };

        }

        protected override void DeleteEarlierPumpedData()
        {
            if (this.DeleteEarlierData)
                DirectDeleteClsData(new IClassifier[] { this.Scheme.Classifiers[D_TENDER_FILES_GUID] }, -1, -1, string.Empty);
            base.DeleteEarlierPumpedData();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheRequest, dsRequest.Tables[0], "SourceKey");
            FillRowsCache(ref cacheRegPeriod, dsRegPeriod.Tables[0], "SourceKey");
            FillRowsCache(ref cacheAskItems, dsAskItems.Tables[0], "SourceKey");
            FillRowsCache(ref cacheProduct, dsProduct.Tables[0], "SourceKey");
            FillRowsCache(ref cacheUnits, dsUnits.Tables[0], "SourceKey");
            FillRowsCache(ref cacheOkdp, dsOkdp.Tables[0], "SourceKey");
            FillRowsCache(ref cacheOkp, dsOkp.Tables[0], "SourceKey");
            FillRowsCache(ref cacheRangeProduct, dsRangeProduct.Tables[0], "SourceKey");
            FillRowsCache(ref cacheKvr, dsKvr.Tables[0], "SourceKey");
            FillRowsCache(ref cacheEkr, dsEkr.Tables[0], "SourceKey");
            FillRowsCache(ref cacheMeasure, dsMeasure.Tables[0], "SourceKey");
            FillRowsCache(ref cacheFinDistr, dsFinDistr.Tables[0], "SourceKey");
            FillRowsCache(ref cacheKif, dsKif.Tables[0], "SourceKey");
            FillRowsCache(ref cacheOrg, dsOrg.Tables[0], "SourceKey");
            FillRowsCache(ref cacheTender, dsTender.Tables[0], "SourceKey");
            FillRowsCache(ref cacheLot, dsLot.Tables[0], "SourceKey");
            FillRowsCache(ref cacheLotItems, dsLotItems.Tables[0], "SourceKey");
            FillRowsCache(ref cacheOffer, dsOffer.Tables[0], "SourceKey");
            FillRowsCache(ref cacheBidItems, dsBidItems.Tables[0], "SourceKey");
            FillRowsCache(ref cacheParameterItems, dsParameterItems.Tables[0], "SourceKey");
            FillRowsCache(ref cacheContract, dsContract.Tables[0], "SourceKey");
            FillRowsCache(ref cacheContractItems, dsContractItems.Tables[0], "SourceKey");
            FillRowsCache(ref cacheDocStatus, dsDocStatus.Tables[0], "SourceKey");
            FillRowsCache(ref cacheDocType, dsDocType.Tables[0], "SourceKey");
            FillRowsCache(ref cacheOkato, dsOkato.Tables[0], "SourceKey");
            FillRowsCache(ref cacheProtocol, dsProtocol.Tables[0], "SourceKey");
            FillRowsCache(ref cacheResolvedLot, dsResolvedLot.Tables[0], "SourceKey");
            FillRowsCache(ref cacheTenderFiles, dsTenderFiles.Tables[0], "SourceKey");
            FillRowsCache(ref cacheSiteUser, dsSiteUser.Tables[0], "SourceKey");
            FillRowsCache(ref cacheSiteGroup, dsSiteGroup.Tables[0], "SourceKey");
            FillRowsCache(ref cacheSiteGroupMember, dsSiteGroupMember.Tables[0], "SourceKey");
            FillRowsCache(ref cacheSiteStatePermission, dsSiteStatePermission.Tables[0], "SourceKey");
            FillRowsCache(ref cacheCategoryOrg, dsCategoryOrg.Tables[0], "SourceKey");
            FillRowsCache(ref cacheCauseOnAct, dsCauseOnAct.Tables[0], "SourceKey");
            FillRowsCache(ref cacheOrderExec, dsOrderExec.Tables[0], "SourceKey");
            FillRowsCache(ref cacheSponsor, dsSponsor.Tables[0], "SourceKey");
            FillRowsCache(ref cacheLotBids, dsLotBids.Tables[0], "SourceKey");
            FillRowsCache(ref cacheAgreements, dsAgreements.Tables[0], "SourceKey");
            FillRowsCache(ref cacheTenderLowSize, dsTenderLowSize.Tables[0], "SourceKey");
            FillRowsCache(ref cacheTerr, dsTerr.Tables[0], "SourceKey");
        }

        private void InitUpdatedFixedRows()
        {
            nullOrg = clsOrg.UpdateFixedRows(this.DB, this.SourceID);
            nullRangeProduct = clsRangeProduct.UpdateFixedRows(this.DB, this.SourceID);
            nullMeasure = clsMeasure.UpdateFixedRows(this.DB, this.SourceID);
            nullRegPeriod = clsRegPeriod.UpdateFixedRows(this.DB, this.SourceID);
            nullProtocol = clsProtocol.UpdateFixedRows(this.DB, this.SourceID);
            nullResolvedLot = clsResolvedLot.UpdateFixedRows(this.DB, this.SourceID);
            nullSiteUser = clsSiteUser.UpdateFixedRows(this.DB, this.SourceID);
            nullSiteGroup = clsSiteGroup.UpdateFixedRows(this.DB, this.SourceID);
            nullSiteGroupMember = clsSiteGroupMember.UpdateFixedRows(this.DB, this.SourceID);
            nullSiteStatePermission = clsSiteStatePermission.UpdateFixedRows(this.DB, this.SourceID);
            nullCategoryOrg = clsCategoryOrg.UpdateFixedRows(this.DB, this.SourceID);
            nullTender = clsTender.UpdateFixedRows(this.DB, this.SourceID);
            nullLot = clsLot.UpdateFixedRows(this.DB, this.SourceID);
            nullProduct = clsProduct.UpdateFixedRows(this.DB, this.SourceID);
            nullLotItems = clsLotItems.UpdateFixedRows(this.DB, this.SourceID);
            nullRequest = clsRequest.UpdateFixedRows(this.DB, this.SourceID);
            nullContract = clsContract.UpdateFixedRows(this.DB, this.SourceID);
            nullAskItems = clsAskItems.UpdateFixedRows(this.DB, this.SourceID);
            nullCauseOnAct = clsCauseOnAct.UpdateFixedRows(this.DB, this.SourceID);
            nullOrderExec = clsOrderExec.UpdateFixedRows(this.DB, this.SourceID);
            nullSponsor = clsSponsor.UpdateFixedRows(this.DB, this.SourceID);
            nullLotBids = clsLotBids.UpdateFixedRows(this.DB, this.SourceID);
        }

        protected override void QueryData()
        {
            InitClsDataSet(ref daRequest, ref dsRequest, clsRequest);
            InitClsDataSet(ref daRegPeriod, ref dsRegPeriod, clsRegPeriod);
            InitClsDataSet(ref daAskItems, ref dsAskItems, clsAskItems);
            InitClsDataSet(ref daProduct, ref dsProduct, clsProduct);
            InitClsDataSet(ref daRangeProduct, ref dsRangeProduct, clsRangeProduct);
            InitClsDataSet(ref daMeasure, ref dsMeasure, clsMeasure);
            InitClsDataSet(ref daFinDistr, ref dsFinDistr, clsFinDistr);
            InitClsDataSet(ref daOrg, ref dsOrg, clsOrg);
            InitClsDataSet(ref daTender, ref dsTender, clsTender);
            InitClsDataSet(ref daLot, ref dsLot, clsLot);
            InitClsDataSet(ref daLotItems, ref dsLotItems, clsLotItems);
            InitClsDataSet(ref daOffer, ref dsOffer, clsOffer);
            InitClsDataSet(ref daBidItems, ref dsBidItems, clsBidItems);
            InitClsDataSet(ref daParameterItems, ref dsParameterItems, clsParameterItems);
            InitClsDataSet(ref daContract, ref dsContract, clsContract);
            InitClsDataSet(ref daContractItems, ref dsContractItems, clsContractItems);
            InitClsDataSet(ref daProtocol, ref dsProtocol, clsProtocol);
            InitClsDataSet(ref daResolvedLot, ref dsResolvedLot, clsResolvedLot);
            InitClsDataSet(ref daSiteUser, ref dsSiteUser, clsSiteUser);
            InitClsDataSet(ref daSiteGroup, ref dsSiteGroup, clsSiteGroup);
            InitClsDataSet(ref daSiteGroupMember, ref dsSiteGroupMember, clsSiteGroupMember);
            InitClsDataSet(ref daSiteStatePermission, ref dsSiteStatePermission, clsSiteStatePermission);
            InitClsDataSet(ref daCategoryOrg, ref dsCategoryOrg, clsCategoryOrg);
            InitClsDataSet(ref daCauseOnAct, ref dsCauseOnAct, clsCauseOnAct);
            InitClsDataSet(ref daOrderExec, ref dsOrderExec, clsOrderExec);
            InitClsDataSet(ref daSponsor, ref dsSponsor, clsSponsor);
            InitClsDataSet(ref daLotBids, ref dsLotBids, clsLotBids);
            InitClsDataSet(ref daAgreements, ref dsAgreements, clsAgreements);

            InitDataSet(ref daTenderLowSize, ref dsTenderLowSize, fctTenderLowSize, false, string.Empty, string.Empty);
            InitDataSet(ref daDocStatus, ref dsDocStatus, clsDocStatus, false, string.Empty, string.Empty);
            InitDataSet(ref daDocType, ref dsDocType, clsDocType, false, string.Empty, string.Empty);
            InitDataSet(ref daOkato, ref dsOkato, clsOkato, false, string.Empty, string.Empty);
            InitDataSet(ref daTenderFiles, ref dsTenderFiles, clsTenderFiles, false, string.Empty, string.Empty);
            InitDataSet(ref daUnits, ref dsUnits, clsUnits, false, string.Empty, string.Empty);
            InitDataSet(ref daOkdp, ref dsOkdp, clsOkdp, false, string.Empty, string.Empty);
            InitDataSet(ref daOkp, ref dsOkp, clsOkp, false, string.Empty, string.Empty);
            InitDataSet(ref daKif, ref dsKif, clsKif, false, string.Empty, string.Empty);
            InitDataSet(ref daKvr, ref dsKvr, clsKvr, false, string.Empty, string.Empty);
            InitDataSet(ref daEkr, ref dsEkr, clsEkr, false, string.Empty, string.Empty);
            InitDataSet(ref daTerr, ref dsTerr, clsTerr, false, string.Empty, string.Empty);

            InitUpdatedFixedRows();
            FillCaches();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daProtocol, dsProtocol, clsProtocol);
            UpdateDataSet(daResolvedLot, dsResolvedLot, clsResolvedLot);
            UpdateDataSet(daTenderFiles, dsTenderFiles, clsTenderFiles);
            UpdateDataSet(daSiteUser, dsSiteUser, clsSiteUser);
            UpdateDataSet(daSiteGroup, dsSiteGroup, clsSiteGroup);
            UpdateDataSet(daSiteGroupMember, dsSiteGroupMember, clsSiteGroupMember);
            UpdateDataSet(daSiteStatePermission, dsSiteStatePermission, clsSiteStatePermission);
            UpdateDataSet(daCategoryOrg, dsCategoryOrg, clsCategoryOrg);
            UpdateDataSet(daDocType, dsDocType, clsDocType);
            UpdateDataSet(daOkato, dsOkato, clsOkato);
            UpdateDataSet(daCauseOnAct, dsCauseOnAct, clsCauseOnAct);

            UpdateDataSet(daTerr, dsTerr, clsTerr);
            UpdateDataSet(daRequest, dsRequest, clsRequest);
            UpdateDataSet(daRegPeriod, dsRegPeriod, clsRegPeriod);
            UpdateDataSet(daAskItems, dsAskItems, clsAskItems);
            UpdateDataSet(daProduct, dsProduct, clsProduct);
            UpdateDataSet(daUnits, dsUnits, clsUnits);
            UpdateDataSet(daOkdp, dsOkdp, clsOkdp);
            UpdateDataSet(daOkp, dsOkp, clsOkp);
            UpdateDataSet(daRangeProduct, dsRangeProduct, clsRangeProduct);
            UpdateDataSet(daKvr, dsKvr, clsKvr);
            UpdateDataSet(daEkr, dsEkr, clsEkr);
            UpdateDataSet(daMeasure, dsMeasure, clsMeasure);
            UpdateDataSet(daFinDistr, dsFinDistr, clsFinDistr);
            UpdateDataSet(daKif, dsKif, clsKif);
            UpdateDataSet(daOrg, dsOrg, clsOrg);
            UpdateDataSet(daTender, dsTender, clsTender);
            UpdateDataSet(daLot, dsLot, clsLot);
            UpdateDataSet(daLotItems, dsLotItems, clsLotItems);
            UpdateDataSet(daOffer, dsOffer, clsOffer);
            UpdateDataSet(daBidItems, dsBidItems, clsBidItems);
            UpdateDataSet(daParameterItems, dsParameterItems, clsParameterItems);
            UpdateDataSet(daContract, dsContract, clsContract);
            UpdateDataSet(daContractItems, dsContractItems, clsContractItems);
            UpdateDataSet(daDocStatus, dsDocStatus, clsDocStatus);
            UpdateDataSet(daOrderExec, dsOrderExec, clsOrderExec);
            UpdateDataSet(daSponsor, dsSponsor, clsSponsor);
            UpdateDataSet(daLotBids, dsLotBids, clsLotBids);
            UpdateDataSet(daAgreements, dsAgreements, clsAgreements);
            UpdateDataSet(daTenderLowSize, dsTenderLowSize, fctTenderLowSize);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsProtocol);
            ClearDataSet(ref dsResolvedLot);
            ClearDataSet(ref dsTenderFiles);
            ClearDataSet(ref dsSiteUser);
            ClearDataSet(ref dsSiteGroup);
            ClearDataSet(ref dsSiteGroupMember);
            ClearDataSet(ref dsSiteStatePermission);
            ClearDataSet(ref dsCategoryOrg);
            ClearDataSet(ref dsCauseOnAct);
            ClearDataSet(ref dsOrderExec);
            ClearDataSet(ref dsSponsor);
            ClearDataSet(ref dsLotBids);
            ClearDataSet(ref dsAgreements);
            ClearDataSet(ref dsTenderLowSize);

            ClearDataSet(ref dsFinDistr);
            ClearDataSet(ref dsAskItems);
            ClearDataSet(ref dsBidItems);
            ClearDataSet(ref dsContractItems);
            ClearDataSet(ref dsLotItems);
            ClearDataSet(ref dsOffer);
            ClearDataSet(ref dsParameterItems);
            ClearDataSet(ref dsLot);
            ClearDataSet(ref dsRequest);
            ClearDataSet(ref dsTender);
            ClearDataSet(ref dsContract);
            ClearDataSet(ref dsProduct);
            ClearDataSet(ref dsDocStatus);
            ClearDataSet(ref dsOrg);
            ClearDataSet(ref dsOkdp);
            ClearDataSet(ref dsRangeProduct);
            ClearDataSet(ref dsUnits);
            ClearDataSet(ref dsOkp);
            ClearDataSet(ref dsEkr);
            ClearDataSet(ref dsKvr);
            ClearDataSet(ref dsMeasure);
            ClearDataSet(ref dsKif);
            ClearDataSet(ref dsRegPeriod);
            ClearDataSet(ref dsDocType);
            ClearDataSet(ref dsOkato);
            ClearDataSet(ref dsTerr);
        }

        #endregion работа с базой и кэшами

        #region обработка полученных данных

        #region общие методы

        private Recordset GetRecordset(string entityName)
        {
            IDataExchangeSeekerRecordset recordSet = inBoundPacketDummy.FindFirstRecordset(entityName, true, true);
            if (recordSet == null)
                return null;
            Recordset rs = new Recordset();
            MSXML2.DOMDocument doc = new DOMDocument();
            doc.loadXML(recordSet.XMLContent.ownerDocument.xml);
            try
            {
                rs.Open(doc, Type.Missing, CursorTypeEnum.adOpenStatic,
                    LockTypeEnum.adLockBatchOptimistic, (int)ADODB.CommandTypeEnum.adCmdFile);
            }
            finally
            {
                doc.loadXML(string.Empty);
                doc = null;
            }
            return rs;
        }

        // убираем переносы строк и двойные пробелы
        private void ProcessValue(ref object value)
        {
            if (value == DBNull.Value)
                return;
            value = value.ToString().Trim().Replace("\r", " ").Replace("\n", " ");
            value = regExSeparators.Replace(value.ToString(), " ");
        }

        private object[] GetRsMapping(Fields fields, object[] mapping, string[] dateFieldNames)
        {
            for (int i = 0; i <= mapping.GetLength(0) - 1; i += 2)
                try
                {
                    mapping[i + 1] = fields[mapping[i + 1].ToString()].Value;
                    ProcessValue(ref mapping[i + 1]);
                    if (dateFieldNames.GetLength(0) != 0)
                        if (CommonRoutines.CheckValueEntry(mapping[i].ToString(), dateFieldNames))
                        {
                            string dateStr = mapping[i + 1].ToString().Split(' ')[0];
                            int date = CommonRoutines.ShortDateToNewDate(dateStr);
                            if ((dateStr != string.Empty) && ((date <= 0) || (date >= 20200000) || (date <= 19900000)))
                            {
                                string message = string.Format("Запись с Id '{0}' содержит некорректную дату ({1}) в поле '{2}'.",
                                    mapping[1], dateStr, mapping[i]);
                                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, message);
                                mapping[i + 1] = -1;
                            }
                            else
                                mapping[i + 1] = date;
                        }
                }
                catch (Exception exp)
                {
                    throw new Exception(string.Format("При получении значения поля '{0}' произошла ошибка: {1}", 
                        mapping[i + 1].ToString(), exp.Message));
                }
            return mapping;
        }

        private void UpdateSourceKeyRow(DataRow row, object[] mapping)
        {
            for (int i = 0; i <= mapping.GetLength(0) - 1; i += 2)
                row[mapping[i].ToString()] = mapping[i + 1];
        }

        private void ProcessRecordset(Recordset rs, object[] mapping, Dictionary<int, DataRow> cache, DataTable dt, IEntity cls,
            string[] clsRefsNames, Dictionary<int, DataRow>[] clsCaches, int[] nullClsRefs, string[] dateFieldNames)
        {
            if (rs == null)
                return;
            try
            {
                if ((rs.BOF && rs.EOF))
                    return;
                rs.MoveFirst();
                while (!rs.EOF)
                {
                    object[] rowMapping = GetRsMapping(rs.Fields, (object[])mapping.Clone(), dateFieldNames);
                    int sourceKey = Convert.ToInt32(rowMapping[1]);
                    DataRow row = null;
                    if (cache.ContainsKey(sourceKey))
                    {
                        row = cache[sourceKey];
                        UpdateSourceKeyRow(row, rowMapping);
                    }
                    else
                    {
                        if (cls == clsCategoryOrg)
                        {
                            string codeStr = rowMapping[5].ToString().TrimStart('0').PadLeft(1, '0');
                            if ((codeStr != "210") && (codeStr != "101") && (codeStr != "102"))
                            {
                                rs.MoveNext();
                                continue;
                            }
                        }
                        PumpCachedRow(cache, dt, cls, sourceKey, rowMapping);
                        row = cache[sourceKey];
                    }
                    if (clsRefsNames != null)
                        SetClsRefs(row, clsRefsNames, clsCaches, nullClsRefs);
                    rs.MoveNext();
                }
            }
            finally
            {
                if (rs.State == 1)
                    rs.Close();
                rs = null;
            }
        }

        private void ProcessRecordset(Recordset rs, object[] mapping, Dictionary<int, DataRow> cache, DataTable dt, IEntity cls,
            string[] clsRefsNames, Dictionary<int, DataRow>[] clsCaches, int[] nullClsRefs)
        {
            ProcessRecordset(rs, mapping, cache, dt, cls, clsRefsNames, clsCaches, nullClsRefs, new string[] { });
        }

        private void ProcessRecordset(Recordset rs, object[] mapping, Dictionary<int, DataRow> cache,
            DataTable dt, IEntity cls)
        {
            ProcessRecordset(rs, mapping, cache, dt, cls, null, null, null, new string[] { });
        }

        private void SetDefaultValue(DataTable dt, string[] fieldNames)
        {
            foreach (DataRow row in dt.Rows)
                foreach (string fieldName in fieldNames)
                    if (row[fieldName] == DBNull.Value)
                        row[fieldName] = constDefaultClsName;
        }

        private void SetFxClsRef(DataTable dt, string fieldName, Dictionary<int, int> fxClsCache)
        {
            foreach (DataRow row in dt.Rows)
                if (row[fieldName] == DBNull.Value)
                    row[fieldName] = -1;
                else if (!fxClsCache.ContainsKey(Convert.ToInt32(row[fieldName])))
                    row[fieldName] = -1;
        }

        private void SetClsRefs(DataRow row, string[] clsRefsNames, Dictionary<int, DataRow>[] clsCaches, int[] nullClsRefs)
        {
            for (int i = 0; i <= clsRefsNames.GetLength(0) - 1; i++)
                if (row[clsRefsNames[i]] == DBNull.Value)
                    row[clsRefsNames[i]] = nullClsRefs[i];
                else
                {
                    int clsSourceKey = Convert.ToInt32(row[clsRefsNames[i]]);
                    if (!clsCaches[i].ContainsKey(clsSourceKey))
                        row[clsRefsNames[i]] = nullClsRefs[i];
                    else
                        row[clsRefsNames[i]] = Convert.ToInt32(clsCaches[i][clsSourceKey]["Id"]);
                }
        }

        // установить код - порядковый номер
        private void SetCodeField(DataTable dt, string fieldName)
        {
            DataRow[] rows = dt.Select(string.Empty, string.Format("{0} Asc", fieldName));
            if (rows.GetLength(0) == 0)
                return;
            int startCode = 0;
            if (rows[rows.GetLength(0) - 1][fieldName] != DBNull.Value)
                startCode = Convert.ToInt32(rows[rows.GetLength(0) - 1][fieldName]);
            int code = 1;
            foreach (DataRow row in dt.Rows)
            {
                if (row[fieldName] != DBNull.Value)
                    continue;
                row[fieldName] = startCode + code;
                code++;
            }
        }

        private void SetClsHierarchy(DataTable dt, Dictionary<int, DataRow> cache)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (row["SourceParentId"] == DBNull.Value)
                {
                    row["ParentId"] = DBNull.Value;
                    continue;
                }
                int sourceParentId = Convert.ToInt32(row["SourceParentId"]);
                if (cache.ContainsKey(sourceParentId))
                    row["ParentId"] = Convert.ToInt32(cache[sourceParentId]["Id"]);
                else
                    row["ParentId"] = DBNull.Value;
            }
        }

        #endregion общие методы

        #region работа с классификаторами

        #region этап 1

        private void PumpRegPeriod()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Период размещения заказа'");
            Recordset rs = GetRecordset(REG_PERIOD_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "CodePeriod", "CODEPERIOD", "Name", "Name", 
                "TypePeriod", "TypePeriod", "CodePeriod1", "CodePeriod1", "CodePeriod2", "CodePeriod2", 
                "CodePeriod3", "CodePeriod3", "Interval", "Interval", "DateModify", "MODIFYDATE"};
            ProcessRecordset(rs, mapping, cacheRegPeriod, dsRegPeriod.Tables[0], clsRegPeriod);
            SetDefaultValue(dsRegPeriod.Tables[0], new string[] { "TypePeriod" });
            foreach (DataRow row in dsRegPeriod.Tables[0].Rows)
            {
                row["CodePeriod1"] = row["CodePeriod1"].ToString().TrimStart('0');
                if (row["CodePeriod2"] == DBNull.Value)
                    row["CodePeriod2"] = "-2";
                row["CodePeriod2"] = row["CodePeriod2"].ToString().TrimStart('0');
                if (row["CodePeriod3"] == DBNull.Value)
                    row["CodePeriod3"] = "-2";
                row["CodePeriod3"] = row["CodePeriod3"].ToString().TrimStart('0');
            }
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Период размещения заказа'");
        }

        private void PumpTerr()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Территории'");
            Recordset rs = GetRecordset(TERR_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Code", "CODE", "Name", "Name", "Website", "Website", 
                "Note", "Note", "DateModify", "MODIFYDATE"};
            ProcessRecordset(rs, mapping, cacheTerr, dsTerr.Tables[0], clsTerr);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Территории'");
        }

        private void PumpKif()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'КИФ'");
            Recordset rs = GetRecordset(KIF_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Code", "CODE", "Name", "Name", 
                "Note", "Note", "DateModify", "MODIFYDATE"};
            ProcessRecordset(rs, mapping, cacheKif, dsKif.Tables[0], clsKif);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'КИФ'");
        }

        private void PumpMeasure()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Мероприятия'");
            Recordset rs = GetRecordset(MEASURE_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Code", "CODE", "Name", "Name", 
                "Note", "Note", "DateModify", "MODIFYDATE"};
            ProcessRecordset(rs, mapping, cacheMeasure, dsMeasure.Tables[0], clsMeasure);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Мероприятия'");
        }

        private void PumpKvr()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'КВР'");
            Recordset rs = GetRecordset(KVR_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Code", "CODE", "Name", "Name", 
                "Note", "Note", "DateModify", "MODIFYDATE"};
            ProcessRecordset(rs, mapping, cacheKvr, dsKvr.Tables[0], clsKvr);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'КВР'");
        }

        private void PumpEkr()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'ЭКР'");
            Recordset rs = GetRecordset(EKR_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Code", "CODE", "Name", "Name", 
                "Note", "Note", "SourceParentId", "ParentId", "DateModify", "MODIFYDATE"};
            ProcessRecordset(rs, mapping, cacheEkr, dsEkr.Tables[0], clsEkr);
            UpdateDataSet(daEkr, dsEkr, clsEkr);
            SetClsHierarchy(dsEkr.Tables[0], cacheEkr);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'ЭКР'");
        }

        private void PumpOkp()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'ОКП'");
            Recordset rs = GetRecordset(OKP_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Code", "CODE", "Name", "Name", 
                "Note", "Note", "SourceParentId", "ParentId", "DateModify", "MODIFYDATE"};
            ProcessRecordset(rs, mapping, cacheOkp, dsOkp.Tables[0], clsOkp);
            UpdateDataSet(daOkp, dsOkp, clsOkp);
            SetClsHierarchy(dsOkp.Tables[0], cacheOkp);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'ОКП'");
        }

        private void PumpUnits()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Единицы измерения'");
            Recordset rs = GetRecordset(UNITS_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Code", "CODE", "Name", "Name", 
                "SYMBOL", "SYMBOL", "DateModify", "MODIFYDATE"};
            ProcessRecordset(rs, mapping, cacheUnits, dsUnits.Tables[0], clsUnits);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Единицы измерения'");
        }

        private void PumpRangeProduct()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Номенклатура продукции'");
            Recordset rs = GetRecordset(RANGE_PRODUCT_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "CodeStr", "CODE", "Name", "Name", 
                "NOTE", "NOTE", "SourceParentId", "ParentId", "DateModify", "MODIFYDATE", "RefTerritory", "AUTOMATIONSUBJECTID" };
            ProcessRecordset(rs, mapping, cacheRangeProduct, dsRangeProduct.Tables[0], clsRangeProduct,
                new string[] { "RefTerritory" }, new Dictionary<int, DataRow>[] { cacheTerr }, new int[] { -1 });
            UpdateDataSet(daRangeProduct, dsRangeProduct, clsRangeProduct);
            SetClsHierarchy(dsRangeProduct.Tables[0], cacheRangeProduct);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Номенклатура продукции'");
        }

        private void PumpOkdp()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'ОКДП'");
            Recordset rs = GetRecordset(OKDP_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "CODESTR", "CODESTR", "Name", "Name", 
                "NOTE", "NOTE", "SourceParentId", "ParentId", "DateModify", "MODIFYDATE"};
            ProcessRecordset(rs, mapping, cacheOkdp, dsOkdp.Tables[0], clsOkdp);
            UpdateDataSet(daOkdp, dsOkdp, clsOkdp);
            SetClsHierarchy(dsOkdp.Tables[0], cacheOkdp);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'ОКДП'");
        }

        private void PumpDocStatus()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Состояния документов'");
            Recordset rs = GetRecordset(DOC_STATUS_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "CODE", "CODE", "Name", "Name", "DateModify", "MODIFYDATE"};
            ProcessRecordset(rs, mapping, cacheDocStatus, dsDocStatus.Tables[0], clsDocStatus);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Состояния документов'");
        }

        private void PumpDocType()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Типы документов'");
            Recordset rs = GetRecordset(DOC_TYPE_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Code", "Code", "Name", "Name", "DateModify", "MODIFYDATE" };
            ProcessRecordset(rs, mapping, cacheDocType, dsDocType.Tables[0], clsDocType);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Типы документов'");
        }

        private void PumpOkato()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'OKATO'");
            Recordset rs = GetRecordset(OKATO_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "CodeStr", "CodeStr", "Name", "Name", 
                "TypeRegion", "TypeRegion", "SourceParentId", "ParentId", "DateModify", "MODIFYDATE"};
            ProcessRecordset(rs, mapping, cacheOkato, dsOkato.Tables[0], clsOkato);
            UpdateDataSet(daOkato, dsOkato, clsOkato);
            SetClsHierarchy(dsOkato.Tables[0], cacheOkato);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'OKATO'");
        }

        private void PumpSiteGroup()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Группы пользователей'");
            Recordset rs = GetRecordset(SITE_GROUP_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Name", "Name", "Note", "Note", "DateModify", "MODIFYDATE"};
            ProcessRecordset(rs, mapping, cacheSiteGroup, dsSiteGroup.Tables[0], clsSiteGroup);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Группы пользователей'");
        }

        private void PumpCauseOnAct()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Основание по закону'");
            Recordset rs = GetRecordset(CAUSE_ON_ACT_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Code", "Code", "Name", "Name", "Note", "Note", "DateModify", "MODIFYDATE" };
            ProcessRecordset(rs, mapping, cacheCauseOnAct, dsCauseOnAct.Tables[0], clsCauseOnAct);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Основание по закону'");
        }

        #endregion этап 1

        #region этап 2

        private void PumpOrg()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Организации'");
            Recordset rs = GetRecordset(ORG_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "CODE", "CODE", "Name", "Name", "SourceParentId", "ParentId", 
                "ADDRESS", "ADDRESS", "DateModify", "MODIFYDATE", "REFOKATO", "REFOKATO", "KPP", "KPP", "RefTerritory", "AUTOMATIONSUBJECTID" };
            ProcessRecordset(rs, mapping, cacheOrg, dsOrg.Tables[0], clsOrg,
                new string[] { "REFOKATO", "RefTerritory" }, new Dictionary<int, DataRow>[] { cacheOkato, cacheTerr }, new int[] { nullOkato, -1 });
            UpdateDataSet(daOrg, dsOrg, clsOrg);
            SetClsHierarchy(dsOrg.Tables[0], cacheOrg);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Организации'");
        }

        private void PumpProduct()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Каталог продукции'");
            Recordset rs = GetRecordset(PRODUCT_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "CODE", "CODE", "Name", "Name", "DESCRIPTION", "DESCRIPTION", 
                "REFUNITS", "REFUNITS", "REFOKDP", "REFOKDP", "REFOKP", "REFOKP", "RefRangeProduct", "RefRangeProduct", "DateModify", "MODIFYDATE", 
                "RefTerritory", "AUTOMATIONSUBJECTID" };
            ProcessRecordset(rs, mapping, cacheProduct, dsProduct.Tables[0], clsProduct,
                new string[] { "REFUNITS", "REFOKDP", "RefRangeProduct", "REFOKP", "RefTerritory" },
                new Dictionary<int, DataRow>[] { cacheUnits, cacheOkdp, cacheRangeProduct, cacheOkp, cacheTerr }, 
                new int[] { nullUnits, nullOkdp, nullRangeProduct, nullOkp, -1});
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Каталог продукции'");
        }

        private void PumpSiteStatePermission()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Разрешения на смену состояний'");
            Recordset rs = GetRecordset(SITE_STATE_PERMISSION_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "RefDocType", "RefDocType", "RefSiteGroup", "RefSiteGroup", 
                "RefStateFrom", "RefStateFrom", "RefStateTo", "RefStateTo", "DateModify", "MODIFYDATE" };
            ProcessRecordset(rs, mapping, cacheSiteStatePermission, dsSiteStatePermission.Tables[0], clsSiteStatePermission,
                new string[] { "RefDocType", "RefSiteGroup", "RefStateFrom", "RefStateTo" },
                new Dictionary<int, DataRow>[] { cacheDocType, cacheSiteGroup, cacheDocStatus, cacheDocStatus },
                new int[] { nullDocType, nullSiteGroup, nullDocStatus, nullDocStatus });
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Разрешения на смену состояний'");
        }

        #endregion этап 2

        #region этап 3

        private void PumpContract()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Контракты'");
            Recordset rs = GetRecordset(CONTRACT_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Name", "Name", "DATAREESTR", "DATAREESTR", "RefYearDayUNV", "CONTRACTDATE", 
                "REFCUSTOMER", "REFCUSTOMER", "REFSUPPLIER", "REFSUPPLIER", "REFDOCSTATUS", "REFDOCSTATUS", "DateModify", "MODIFYDATE", 
                "TenderPreferences", "TenderPreferences", "RefTenderPreferences", "RefTenderPreferences", "CONTRACTCOST", "CONTRACTCOST", 
                "RefTypeTender", "RefTypeTender", "RefAcceptDate", "RefAcceptDate", "RefCause", "RefCause", "RefDateEnd", "RefDateEnd", 
                "VERSIONNO", "VERSIONNO", "RefTerritory", "AUTOMATIONSUBJECTID", "FinSource", "FinSource", 
                "RefTenderDate", "RefTenderDate" };
            ProcessRecordset(rs, mapping, cacheContract, dsContract.Tables[0], clsContract,
                new string[] { "REFCUSTOMER", "REFSUPPLIER", "REFDOCSTATUS", "RefTypeTender", "RefCause", "RefTerritory" },
                new Dictionary<int, DataRow>[] { cacheOrg, cacheOrg, cacheDocStatus, cacheDocType, cacheCauseOnAct, cacheTerr },
                new int[] { nullOrg, nullOrg, nullDocStatus, nullDocType, nullCauseOnAct, -1 },
                new string[] { "RefYearDayUNV", "RefAcceptDate", "RefDateEnd", "RefTenderDate" });
            SetCodeField(dsContract.Tables[0], "Code");

            // обновляем поля из OrderExec
            rs = GetRecordset(ORDER_EXEC_ENTITY_NAME);
            mapping = new object[] { "SourceKey", "RefContract", "RefExeType", "ExecutionType", "ActualPayment", "ActualPayment",
                "CancellationCause", "CancellationCause", "RefExeStatus", "RefDocStatus", "RefExeDate", "RefExecutionDate" };
            ProcessRecordset(rs, mapping, cacheContract, dsContract.Tables[0], clsContract,
                new string[] { "RefExeStatus" },
                new Dictionary<int, DataRow>[] { cacheDocStatus },
                new int[] { nullDocStatus }, new string[] { "RefExeDate" });

            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Контракты'");
        }

        private void PumpTender()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Закупки'");
            Recordset rs = GetRecordset(TENDER_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "CODESTR", "CODESTR", "Name", "Name", "REFDOCSTATUS", "REFDOCSTATUS", 
                "REFTYPETENDER", "REFTYPETENDER", "REFORGANIZER", "REFORGANIZER", "REFSPORGANIZER", "REFSPORGANIZER", 
                "REFPERIOD", "REFPERIOD", "DateModify", "MODIFYDATE", "RefTerritory", "AUTOMATIONSUBJECTID", 
                "RefExecutor", "RefExecutor", "RefDatePublication", "RefDatePublication", "RefDateOpening", "RefDateOpening", 
                "RefDateConsider", "RefDateConsider", "RefDateMatching", "RefDateMatching", "REFFORSMB", "Forsmallbusiness" };
            ProcessRecordset(rs, mapping, cacheTender, dsTender.Tables[0], clsTender,
                new string[] { "REFDOCSTATUS", "REFORGANIZER", "REFSPORGANIZER", "REFPERIOD", "REFTYPETENDER", "RefExecutor", "RefTerritory" },
                new Dictionary<int, DataRow>[] { cacheDocStatus, cacheOrg, cacheOrg, cacheRegPeriod, cacheDocType, cacheOrg, cacheTerr }, 
                new int[] { nullDocStatus, nullOrg, nullOrg, nullRegPeriod, nullDocType, nullOrg, -1 },
                new string[] { "RefDatePublication", "RefDateOpening", "RefDateConsider", "RefDateMatching" });
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Закупки'");
        }

        private void PumpRequest()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Заявки на закупку'");
            Recordset rs = GetRecordset(REQUEST_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Name", "Name", "RequestCost", "RequestCost", 
                "REFCUSTOMER", "REFCUSTOMER", "REFDOCSTATUS", "REFDOCSTATUS", "REFPERIOD", "REFPERIOD", 
                "REFTYPETENDER", "REFTYPETENDER", "DateModify", "MODIFYDATE", 
                "REFDOCTYPE", "REFDOCTYPE", "REFEXECUTOR", "REFEXECUTOR", "REFORGANIZER", "REFORGANIZER", 
                "PurchaseYear", "PurchaseYear", "RefDocDate", "RefDocDate", "REFFORSMB", "FORSMALLBUSINESS", "RefTerritory", "AUTOMATIONSUBJECTID" };
            ProcessRecordset(rs, mapping, cacheRequest, dsRequest.Tables[0], clsRequest,
                new string[] { "REFCUSTOMER", "REFDOCSTATUS", "REFPERIOD", "REFTYPETENDER", "REFDOCTYPE", "REFEXECUTOR", "REFORGANIZER", "RefTerritory" },
                new Dictionary<int, DataRow>[] { cacheOrg, cacheDocStatus, cacheRegPeriod, cacheDocType, cacheDocType, cacheOrg, cacheOrg, cacheTerr },
                new int[] { nullOrg, nullDocStatus, nullRegPeriod, nullDocType, nullDocType, nullOrg, nullOrg, -1 },
                new string[] { "RefDocDate" });
            foreach (DataRow row in dsRequest.Tables[0].Rows)
            {
                if (row["PurchaseYear"].ToString().Length < 8)
                    row["PurchaseYear"] = row["PurchaseYear"].ToString() + "0001";
                int date = Convert.ToInt32(row["PurchaseYear"].ToString().Trim().PadLeft(1, '0'));
                if ((date >= 20200000) || (date <= 19900000))
                {
                    string message = string.Format("Запись с Id '{0}' содержит некорректную дату {1} в поле 'PurchaseYear'.",
                        row["SourceKey"], date);
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, message);
                    row["PurchaseYear"] = -1;
                }
            }

            SetCodeField(dsRequest.Tables[0], "Code");
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Заявки на закупку'");
        }

        private void ProcessCategoryOrg()
        {
            Dictionary<int, DataRow> refOrgCache = new Dictionary<int, DataRow>();
            try
            {
                DataRow[] rows = dsCategoryOrg.Tables[0].Select(string.Empty, "CodeStr ASC");
                foreach (DataRow row in rows)
                {
                    int refOrg = Convert.ToInt32(row["RefOrganization"]);
                    if (refOrg == nullOrg)
                        continue;
                    if (!refOrgCache.ContainsKey(refOrg))
                    {
                        refOrgCache.Add(refOrg, row);
                        continue;
                    }
                    DataRow cacheRow = refOrgCache[refOrg];
                    cacheRow["CodeStr"] = string.Format("{0}{1}", cacheRow["CodeStr"], row["CodeStr"]);
                    cacheRow["Name"] = string.Format("{0} и {1}", cacheRow["Name"], row["Name"]);
                    row.Delete();
                }
                UpdateDataSet(daCategoryOrg, dsCategoryOrg, clsCategoryOrg);
            }
            finally
            {
                refOrgCache.Clear();
            }
        }

        private void PumpCategoryOrg()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Категории организаций'");
            Recordset rs = GetRecordset(CATEGORY_ORG_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Name", "Name", "CodeStr", "CodeStr", 
                "RefOrganization", "RefOrganization", "DateModify", "MODIFYDATE", "RefTerritory", "AUTOMATIONSUBJECTID" };
            ProcessRecordset(rs, mapping, cacheCategoryOrg, dsCategoryOrg.Tables[0], clsCategoryOrg,
                new string[] { "RefOrganization", "RefTerritory" }, 
                new Dictionary<int, DataRow>[] { cacheOrg, cacheTerr }, new int[] { nullOrg, -1 });
            UpdateData();
            ProcessCategoryOrg();
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Категории организаций'");
        }

        private void PumpSiteUser()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Пользователи официального сайта'");
            Recordset rs = GetRecordset(SITE_USER_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Login", "Login",
                "RefDateRegister", "RefDateRegister", "RefDateDiscard", "RefDateDiscard", "RefTerritory", "AUTOMATIONSUBJECTID", 
                "RefPerson", "RefPerson", "RefOrganization", "RefOrganization", "DateModify", "MODIFYDATE", "RefIsActive", "IsActive" };
            ProcessRecordset(rs, mapping, cacheSiteUser, dsSiteUser.Tables[0], clsSiteUser,
                new string[] { "RefPerson", "RefOrganization", "RefTerritory" },
                new Dictionary<int, DataRow>[] { cacheOrg, cacheOrg, cacheTerr }, new int[] { nullOrg, nullOrg, -1 },
                new string[] { "RefDateRegister", "RefDateDiscard" });
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Пользователи официального сайта'");
        }

        private void PumpAgreements()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Документы исполнения'");
            Recordset rs = GetRecordset(AGREEMENTS_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Name", "Name", "Designation", "Designation",
                "DateModify", "MODIFYDATE", "AgreementCost", "AgreementCost", "ActualPayment", "ActualPayment", "Note", "Note", 
                "RefCustomer", "RefCustomer", "RefSupplier", "RefSupplier", "RefAgreementDate", "RefAgreementDate", 
                "RefAgreementBeginDate", "RefAgreementBeginDate", "RefAgreementEndDate", "RefAgreementEndDate", 
                "RefDocType", "RefDocType", "RefDocStatus", "RefDocStatus", "RefTerritory", "AUTOMATIONSUBJECTID" };
            ProcessRecordset(rs, mapping, cacheAgreements, dsAgreements.Tables[0], clsAgreements,
                new string[] { "RefCustomer", "RefSupplier", "RefDocType", "RefDocStatus", "RefTerritory" },
                new Dictionary<int, DataRow>[] { cacheOrg, cacheOrg, cacheDocType, cacheDocStatus, cacheTerr }, 
                new int[] { nullOrg, nullOrg, nullDocType, nullDocStatus, -1 },
                new string[] { "RefAgreementDate", "RefAgreementBeginDate", "RefAgreementEndDate" });
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Документы исполнения'");
        }

        #endregion этап 3

        #region этап 4

        private void PumpLot()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Лоты'");
            Recordset rs = GetRecordset(LOT_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "CODESTR", "CODESTR", "Name", "Name", "LotCost", "LotCost", 
                "REFTENDER", "REFTENDER", "REFDATEEND", "REFDATEEND", "DateModify", "MODIFYDATE", "RefTerritory", "AUTOMATIONSUBJECTID",  
                "Offersecuritysize", "Offersecuritysize", "Contractsecuritysize", "Contractsecuritysize" };
            ProcessRecordset(rs, mapping, cacheLot, dsLot.Tables[0], clsLot,
                new string[] { "REFTENDER", "RefTerritory" }, new Dictionary<int, DataRow>[] { cacheTender, cacheTerr }, 
                new int[] { nullTender, -1 }, new string[] { "REFDATEEND" });
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Лоты'");
        }

        private void PumpParameterItems()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Параметры продукции'");
            Recordset rs = GetRecordset(PARAMETER_ITEMS_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "CODESTR", "CODESTR", "Name", "Name", "Dimensions", "Dimensions", 
                "UNIT", "UNIT", "RefProduct", "RefProduct", "DateModify", "MODIFYDATE" };
            ProcessRecordset(rs, mapping, cacheParameterItems, dsParameterItems.Tables[0], clsParameterItems,
                new string[] { "RefProduct" }, new Dictionary<int, DataRow>[] { cacheProduct }, new int[] { nullProduct });
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Параметры продукции'");
        }

        private void PumpOffer()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Предложения поставщиков'");
            Recordset rs = GetRecordset(OFFER_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Name", "Name",
                "REFDOCSTATUS", "REFDOCSTATUS", "REFSUPPLIER", "REFSUPPLIER", "DateModify", "MODIFYDATE", 
                "RefTerritory", "AUTOMATIONSUBJECTID", "RefTypeTender", "RefTypeTender" };
            ProcessRecordset(rs, mapping, cacheOffer, dsOffer.Tables[0], clsOffer,
                new string[] { "REFDOCSTATUS", "REFSUPPLIER", "RefTerritory", "RefTypeTender" },
                new Dictionary<int, DataRow>[] { cacheDocStatus, cacheOrg, cacheTerr, cacheDocType },
                new int[] { nullDocStatus, nullOrg, -1, nullDocType });
            SetCodeField(dsOffer.Tables[0], "Code");
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Предложения поставщиков'");
        }

        private void PumpTenderFiles()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Документация к закупке'");
            Recordset rs = GetRecordset(TENDER_FILES_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "RefDocType", "RefDocType", 
                "REFTENDER", "REFTENDER", "DateModify", "MODIFYDATE", "RefTerritory", "AUTOMATIONSUBJECTID" };
            ProcessRecordset(rs, mapping, cacheTenderFiles, dsTenderFiles.Tables[0], clsTenderFiles,
                new string[] { "RefDocType", "REFTENDER", "RefTerritory" },
                new Dictionary<int, DataRow>[] { cacheDocType, cacheTender, cacheTerr }, 
                new int[] { nullDocType, nullTender, -1 });
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Документация к закупке'");
        }

        private void PumpSiteGroupMember()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Вхождение в группы'");
            Recordset rs = GetRecordset(SITE_GROUP_MEMBER_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "RefSiteUser", "RefSiteUser", 
                "RefSiteGroup", "RefSiteGroup", "DateModify", "MODIFYDATE" };
            ProcessRecordset(rs, mapping, cacheSiteGroupMember, dsSiteGroupMember.Tables[0], clsSiteGroupMember,
                new string[] { "RefSiteUser", "RefSiteGroup" },
                new Dictionary<int, DataRow>[] { cacheSiteUser, cacheSiteGroup }, 
                new int[] { nullSiteUser, nullSiteGroup });
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Вхождение в группы'");
        }

        private void PumpProtocol()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Протоколы'");
            Recordset rs = GetRecordset(PROTOCOL_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "CodeStr", "CodeStr", "RefTender", "RefTender",
                "RefTypeProtocol", "RefTypeProtocol", "RefDocStatus", "RefDocStatus", "RefDateSession", "RefDateSession", 
                "DateModify", "MODIFYDATE", "RefTerritory", "AUTOMATIONSUBJECTID" };
            ProcessRecordset(rs, mapping, cacheProtocol, dsProtocol.Tables[0], clsProtocol,
                new string[] { "RefTender", "RefTypeProtocol", "RefDocStatus", "RefTerritory" },
                new Dictionary<int, DataRow>[] { cacheTender, cacheDocType, cacheDocStatus, cacheTerr },
                new int[] { nullTender, nullDocType, nullDocStatus, -1 }, new string[] { "RefDateSession"});
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Протоколы'");
        }

        private void PumpOrderExec()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Сведения об исполнении контрактов'");
/*            Recordset rs = GetRecordset(ORDER_EXEC_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "RefContract", "RefContract", "DateModify", "MODIFYDATE" };
            ProcessRecordset(rs, mapping, cacheOrderExec, dsOrderExec.Tables[0], clsOrderExec,
                new string[] { "RefContract" },
                new Dictionary<int, DataRow>[] { cacheContract },
                new int[] { nullContract });*/
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Сведения об исполнении контрактов'");
        }

        private void PumpSponsor()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Распределение финансирования для контракта'");
            Recordset rs = GetRecordset(SPONSOR_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Summa", "Summa",
                "RefFinPeriod", "RefFinPeriod", 
                "RefContract", "RefContract", "DateModify", "MODIFYDATE", "RefTerritory", "AUTOMATIONSUBJECTID" };
            ProcessRecordset(rs, mapping, cacheSponsor, dsSponsor.Tables[0], clsSponsor,
                new string[] { "RefContract", "RefFinPeriod", "RefTerritory" },
                new Dictionary<int, DataRow>[] { cacheContract, cacheRegPeriod, cacheTerr },
                new int[] { nullContract, nullRegPeriod, -1 });
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Распределение финансирования для контракта'");
        }

        private void PumpTenderLowSize()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки 'ЭО_Мониторинг_Закупки малого объема'");
            Recordset rs = GetRecordset(TENDER_LOW_SIZE_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Quantity", "Quantity", "Price", "Price", "Cost", "Cost", 
                "RefAgreements", "RefAgreements", "RefProduct", "RefProduct", "DateModify", "MODIFYDATE", "RefTerritory", "AUTOMATIONSUBJECTID" };
            ProcessRecordset(rs, mapping, cacheTenderLowSize, dsTenderLowSize.Tables[0], fctTenderLowSize,
                new string[] { "RefAgreements", "RefProduct", "RefTerritory" },
                new Dictionary<int, DataRow>[] { cacheAgreements, cacheProduct, cacheTerr },
                new int[] { nullAgreements, nullProduct, -1 });
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки 'ЭО_Мониторинг_Закупки малого объема'");
        }

        #endregion этап 4

        #region этап 5

        private void PumpLotItems()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Строки лотов'");
            Recordset rs = GetRecordset(LOT_ITEMS_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", 
                "REFLOT", "REFLOT", "REFPRODUCT", "REFPRODUCT", "DateModify", "MODIFYDATE", "RefTerritory", "AUTOMATIONSUBJECTID" };
            ProcessRecordset(rs, mapping, cacheLotItems, dsLotItems.Tables[0], clsLotItems,
                new string[] { "REFLOT", "REFPRODUCT", "RefTerritory" },
                new Dictionary<int, DataRow>[] { cacheLot, cacheProduct, cacheTerr }, 
                new int[] { nullLot, nullProduct, -1 });
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Строки лотов'");
        }

        private void PumpResolvedLot()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Решения комиссии'");
            Recordset rs = GetRecordset(RESOLVED_LOT_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "RefProtocol", "RefProtocol",
                "Code", "CodeStr", "Name", "Name", "RefLot", "RefLot", "DateModify", "MODIFYDATE", "RefTerritory", "AUTOMATIONSUBJECTID" };
            ProcessRecordset(rs, mapping, cacheResolvedLot, dsResolvedLot.Tables[0], clsResolvedLot,
                new string[] { "RefProtocol", "RefLot", "RefTerritory" },
                new Dictionary<int, DataRow>[] { cacheProtocol, cacheLot, cacheTerr }, 
                new int[] { nullProtocol, nullLot, -1 });
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Решения комиссии'");
        }

        private void PumpLotBids()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Лоты поставщиков для закупки'");
            Recordset rs = GetRecordset(LOT_BIDS_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Spriority", "Spriority",
                "Refusal", "RefusalFlag", "RefOffer", "RefOffer",
                "RefLot", "RefLot", "DateModify", "MODIFYDATE", "RefTerritory", "AUTOMATIONSUBJECTID" };
            ProcessRecordset(rs, mapping, cacheLotBids, dsLotBids.Tables[0], clsLotBids,
                new string[] { "RefOffer", "RefLot", "RefTerritory" },
                new Dictionary<int, DataRow>[] { cacheOffer, cacheLot, cacheTerr },
                new int[] { nullOffer, nullLot, -1 });
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Лоты поставщиков для закупки'");
        }

        #endregion этап 5

        #region этап 6

        private void PumpContractItems()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Строки контрактов'");
            Recordset rs = GetRecordset(CONTRACT_ITEMS_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Quantity", "Quantity", "Price", "Price", "Cost", "Cost", 
                "REFPRODUCT", "REFPRODUCT", "REFCONTRACT", "REFCONTRACT", "REFLOTITEMS", "REFLOTITEMS", "DateModify", "MODIFYDATE", 
                "RefTerritory", "AUTOMATIONSUBJECTID" };
            ProcessRecordset(rs, mapping, cacheContractItems, dsContractItems.Tables[0], clsContractItems,
                new string[] { "REFPRODUCT", "REFCONTRACT", "REFLOTITEMS", "RefTerritory" },
                new Dictionary<int, DataRow>[] { cacheProduct, cacheContract, cacheLotItems, cacheTerr }, 
                new int[] { nullProduct, nullContract, nullLotItems, -1 });
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Строки контрактов'");
        }

        private void PumpBidItems()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Строки предложения поставщика'");
            Recordset rs = GetRecordset(BID_ITEMS_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Quantity", "Quantity", "Price", "Price", "Cost", "Cost", "LOTOFFERRANGE", "LOTOFFERRANGE",
                "REFPRODUCT", "REFPRODUCT", "REFOFFER", "REFOFFER", "REFLOTITEMS", "REFLOTITEMS", "DateModify", "MODIFYDATE", 
                "RefTerritory", "AUTOMATIONSUBJECTID" };
            ProcessRecordset(rs, mapping, cacheBidItems, dsBidItems.Tables[0], clsBidItems,
                new string[] { "REFPRODUCT", "REFOFFER", "REFLOTITEMS", "RefTerritory" },
                new Dictionary<int, DataRow>[] { cacheProduct, cacheOffer, cacheLotItems, cacheTerr },
                new int[] { nullProduct, nullOffer, nullLotItems, -1 });
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Строки предложения поставщика'");
        }

        private void PumpAskItems()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Строки заявок заказчиков'");
            Recordset rs = GetRecordset(ASK_ITEMS_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "Quantity", "Quantity", "Price", "Price", "Cost", "Cost", 
                "REFREQUEST", "REFREQUEST", "REFMEASURE", "REFMEASURE", "REFKVR", "REFKVR", 
                "REFEKR", "REFEKR", "REFPRODUCT", "REFPRODUCT", "REFLOTITEMS", "REFLOTITEMS", "DateModify", "MODIFYDATE", "RefTerritory", "AUTOMATIONSUBJECTID" };
            ProcessRecordset(rs, mapping, cacheAskItems, dsAskItems.Tables[0], clsAskItems,
                new string[] { "REFREQUEST", "REFMEASURE", "REFKVR", "REFEKR", "REFPRODUCT", "REFLOTITEMS", "RefTerritory" },
                new Dictionary<int, DataRow>[] { cacheRequest, cacheMeasure, cacheKvr, cacheEkr, cacheProduct, cacheLotItems, cacheTerr },
                new int[] { nullRequest, nullMeasure, nullKvr, nullEkr, nullProduct, nullLotItems, -1 });
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Строки заявок заказчиков'");
        }

        #endregion этап 6

        #region этап 7

        private void PumpFinDistr()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Распределение финансирования'");
            Recordset rs = GetRecordset(FIN_DISTR_ENTITY_NAME);
            object[] mapping = new object[] { "SourceKey", "Id", "ACCEPTEDAMOUNT", "ACCEPTEDAMOUNT", 
                "REFASKITEMS", "REFASKITEMS", "REFKIF", "REFKIF", "DateModify", "MODIFYDATE", "RefTerritory", "AUTOMATIONSUBJECTID" };
            ProcessRecordset(rs, mapping, cacheFinDistr, dsFinDistr.Tables[0], clsFinDistr,
            new string[] { "REFASKITEMS", "REFKIF", "RefTerritory" },
                new Dictionary<int, DataRow>[] { cacheAskItems, cacheKif, cacheTerr }, 
                new int[] { nullAskItems, nullKif, -1 });
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Распределение финансирования'");
        }

        #endregion этап 7

        #endregion работа с классификаторами

        private void DoUniqueNames(DataSet ds)
        {
            DataTable dt = ds.Tables[0];
            if (!dt.Columns.Contains("Name"))
                return;
            // убираем точки с конца - а то при добавлении новых записей возможна некорректная ситуация
            foreach (DataRow row in ds.Tables[0].Rows)
                row["Name"] = row["Name"].ToString().TrimEnd('.');

            bool isHier = dt.Columns.Contains("ParentId");
            // Ключ - "наименование|ссылка на родителя", Значение - кол - во дописываемых точек (в случае неуникальности наименования)
            Dictionary<string, string> dtCache = new Dictionary<string, string>();
            try
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string key = string.Format("{0}|", row["Name"].ToString().ToUpper());
                    if (isHier)
                        key += row["ParentId"];
                    if (!dtCache.ContainsKey(key))
                    {
                        dtCache.Add(key, string.Empty);
                    }
                    else
                    {
                        dtCache[key] += ".";
                        row["Name"] += dtCache[key];
                    }
                }
            }
            finally
            {
                dtCache.Clear();
            }
        }

        private void PumpInboundPacket()
        {
            // этап 1
            PumpRegPeriod();
            PumpTerr();
            UpdateData();
            PumpKif();
            PumpMeasure();
            PumpKvr();
            PumpEkr();
            PumpOkp();
            PumpUnits();
            PumpRangeProduct();
            PumpOkdp();
            PumpDocStatus();
            PumpDocType();
            PumpOkato();
            PumpSiteGroup();
            PumpCauseOnAct();
            UpdateData();
            // этап 2
            PumpOrg();
            PumpProduct();
            PumpSiteStatePermission();
            UpdateData();
            // этап 3
            PumpContract();
            PumpTender();
            PumpRequest();
            PumpCategoryOrg();
            PumpSiteUser();
            PumpAgreements();
            UpdateData();
            // этап 4
            PumpLot();
            PumpParameterItems();
            PumpOffer();
            PumpTenderFiles();
            PumpSiteGroupMember();
            PumpProtocol();
            PumpOrderExec();
            PumpSponsor();
            PumpTenderLowSize();
            UpdateData();
            // этап 5
            PumpLotItems();
            PumpResolvedLot();
            PumpLotBids();
            UpdateData();
            // этап 6
            PumpContractItems();
            PumpBidItems();
            PumpAskItems();
            UpdateData();
            // этап 7
            PumpFinDistr();
            UpdateData();
        }

        #endregion обработка полученных данных

        #region запрос данных

        private DateTime GetEntityMaxDate(string entityDbName)
        {
            object maxDate = DBNull.Value;
            if (!this.DeleteEarlierData)
                maxDate = this.DB.ExecQuery(string.Format("select max(DateModify) FROM {0}", entityDbName), 
                    QueryResultTypes.Scalar, new IDbDataParameter[] { });
            if (maxDate == DBNull.Value)
                return new DateTime(1990, 1, 1, 0, 0, 0);
            else
                return Convert.ToDateTime(maxDate);
        }

        private void FillOutboundPacket(IDataExchangeOutboundPacket packet, int partIndex)
        {
            switch (partIndex)
            {
                case 1:
                    packet.AddRequestByDate(REG_PERIOD_ENTITY_NAME, GetEntityMaxDate(clsRegPeriod.FullDBName));
                    packet.AddRequestByDate(TERR_ENTITY_NAME, GetEntityMaxDate(clsTerr.FullDBName));
                    packet.AddRequestByDate(KIF_ENTITY_NAME, GetEntityMaxDate(clsKif.FullDBName));
                    packet.AddRequestByDate(MEASURE_ENTITY_NAME, GetEntityMaxDate(clsMeasure.FullDBName));
                    packet.AddRequestByDate(KVR_ENTITY_NAME, GetEntityMaxDate(clsKvr.FullDBName));
                    packet.AddRequestByDate(EKR_ENTITY_NAME, GetEntityMaxDate(clsEkr.FullDBName));
                    packet.AddRequestByDate(OKP_ENTITY_NAME, GetEntityMaxDate(clsOkp.FullDBName));
                    packet.AddRequestByDate(UNITS_ENTITY_NAME, GetEntityMaxDate(clsUnits.FullDBName));
                    packet.AddRequestByDate(RANGE_PRODUCT_ENTITY_NAME, GetEntityMaxDate(clsRangeProduct.FullDBName));
                    packet.AddRequestByDate(OKDP_ENTITY_NAME, GetEntityMaxDate(clsOkdp.FullDBName));
                    packet.AddRequestByDate(DOC_STATUS_ENTITY_NAME, GetEntityMaxDate(clsDocStatus.FullDBName));
                    packet.AddRequestByDate(DOC_TYPE_ENTITY_NAME, GetEntityMaxDate(clsDocType.FullDBName));
                    packet.AddRequestByDate(OKATO_ENTITY_NAME, GetEntityMaxDate(clsOkato.FullDBName));
                    packet.AddRequestByDate(SITE_GROUP_ENTITY_NAME, GetEntityMaxDate(clsSiteGroup.FullDBName));
                    packet.AddRequestByDate(CAUSE_ON_ACT_ENTITY_NAME, GetEntityMaxDate(clsCauseOnAct.FullDBName));
                    break;
                case 2:
                    packet.AddRequestByDate(ORG_ENTITY_NAME, GetEntityMaxDate(clsOrg.FullDBName));
                    packet.AddRequestByDate(PRODUCT_ENTITY_NAME, GetEntityMaxDate(clsProduct.FullDBName));
                    packet.AddRequestByDate(SITE_STATE_PERMISSION_ENTITY_NAME, GetEntityMaxDate(clsSiteStatePermission.FullDBName));
                    break;
                case 3:
                    packet.AddRequestByDate(CONTRACT_ENTITY_NAME, GetEntityMaxDate(clsContract.FullDBName));
                    packet.AddRequestByDate(TENDER_ENTITY_NAME, GetEntityMaxDate(clsTender.FullDBName));
                    packet.AddRequestByDate(REQUEST_ENTITY_NAME, GetEntityMaxDate(clsRequest.FullDBName));
                    packet.AddRequestByDate(CATEGORY_ORG_ENTITY_NAME, GetEntityMaxDate(clsCategoryOrg.FullDBName));
                    packet.AddRequestByDate(SITE_USER_ENTITY_NAME, GetEntityMaxDate(clsSiteUser.FullDBName));
                    packet.AddRequestByDate(AGREEMENTS_ENTITY_NAME, GetEntityMaxDate(clsAgreements.FullDBName));
                    break;
                case 4:
                    packet.AddRequestByDate(LOT_ENTITY_NAME, GetEntityMaxDate(clsLot.FullDBName));
                    packet.AddRequestByDate(PARAMETER_ITEMS_ENTITY_NAME, GetEntityMaxDate(clsParameterItems.FullDBName));
                    packet.AddRequestByDate(OFFER_ENTITY_NAME, GetEntityMaxDate(clsOffer.FullDBName));
                    packet.AddRequestByDate(TENDER_FILES_ENTITY_NAME, GetEntityMaxDate(clsTenderFiles.FullDBName));
                    packet.AddRequestByDate(SITE_GROUP_MEMBER_ENTITY_NAME, GetEntityMaxDate(clsSiteGroupMember.FullDBName));
                    packet.AddRequestByDate(PROTOCOL_ENTITY_NAME, GetEntityMaxDate(clsProtocol.FullDBName));
                    packet.AddRequestByDate(ORDER_EXEC_ENTITY_NAME, GetEntityMaxDate(clsOrderExec.FullDBName));
                    packet.AddRequestByDate(SPONSOR_ENTITY_NAME, GetEntityMaxDate(clsSponsor.FullDBName));
                    packet.AddRequestByDate(TENDER_LOW_SIZE_ENTITY_NAME, GetEntityMaxDate(fctTenderLowSize.FullDBName));
                    break;
                case 5:
                    packet.AddRequestByDate(LOT_ITEMS_ENTITY_NAME, GetEntityMaxDate(clsLotItems.FullDBName));
                    packet.AddRequestByDate(RESOLVED_LOT_ENTITY_NAME, GetEntityMaxDate(clsResolvedLot.FullDBName));
                    packet.AddRequestByDate(LOT_BIDS_ENTITY_NAME, GetEntityMaxDate(clsLotBids.FullDBName));
                    break;
                case 6:
                    packet.AddRequestByDate(CONTRACT_ITEMS_ENTITY_NAME, GetEntityMaxDate(clsContractItems.FullDBName));
                    packet.AddRequestByDate(BID_ITEMS_ENTITY_NAME, GetEntityMaxDate(clsBidItems.FullDBName));
                    packet.AddRequestByDate(ASK_ITEMS_ENTITY_NAME, GetEntityMaxDate(clsAskItems.FullDBName));
                    break;
                case 7:
                    packet.AddRequestByDate(FIN_DISTR_ENTITY_NAME, GetEntityMaxDate(clsFinDistr.FullDBName));
                    break;
            }
        }

        private void PostOutboundPacket(string exchangeTarget, int partIndex)
        {
            IDataExchangeOutboundPacket packet = exchangeService.CreateOutboundPacket(exchangeTarget);
            packet.ExchangeSource = "KRISTA.FINMON";
            packet.ExchangeTarget = exchangeTarget;
            try
            {
                FillOutboundPacket(packet, partIndex);
                packet.Post();
            }
            finally
            {
                Marshal.ReleaseComObject(packet);
            }
        }

        private void PostPacket()
        {
            exchangeService = new DataExchangeServiceClassClass();
            try
            {
                string exchangeTarget = "FM";
                // получаем данные по частям (при больших объемах возникала ошибка - с нехваткой памяти)
                PostOutboundPacket(exchangeTarget, 1);
                PostOutboundPacket(exchangeTarget, 2);
                PostOutboundPacket(exchangeTarget, 3);
                PostOutboundPacket(exchangeTarget, 4);
                PostOutboundPacket(exchangeTarget, 5);
                PostOutboundPacket(exchangeTarget, 6);
                PostOutboundPacket(exchangeTarget, 7);
            }
            finally
            {
                Marshal.ReleaseComObject(exchangeService);
            }
        }

        #endregion запрос данных

        #region перекрытые методы закачек

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            FileInfo[] files = dir.GetFiles("*.xml", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                string fileName = file.Name;
                WriteToTrace(string.Format("начало закачки файла {0}", fileName), TraceMessageKind.Information);
                inBoundPacketDummy = new ExchangePacketInboundDummyClass();
                try
                {
                    inBoundPacketDummy.DeserializeFrom(file.FullName);
                    PumpInboundPacket();
                }
                finally
                {
                    Marshal.ReleaseComObject(inBoundPacketDummy);
                }
                WriteToTrace(string.Format("завершение закачки файла {0}", fileName), TraceMessageKind.Information);
            }
            // делаем наименования уникальными в таблице (приписываются точки) - в отчетах эксперта проблемы из за одинаковых имен
            DataSet[] dss = new DataSet[] { dsRegPeriod, dsKif, dsMeasure, dsKvr, dsEkr, dsOkp, dsUnits, dsRangeProduct, 
                dsOkdp, dsDocStatus, dsDocType, dsOkato, dsSiteGroup, dsCauseOnAct, dsOrg, dsProduct, dsSiteStatePermission, 
                dsContract, dsTender, dsRequest, dsSiteUser, dsAgreements, dsLot, dsOffer, 
                dsTenderFiles, dsSiteGroupMember, dsProtocol, dsSponsor, dsTenderLowSize, dsLotItems, dsLotBids, 
                dsContractItems, dsBidItems, dsAskItems, dsFinDistr };
            foreach (DataSet ds in dss)
                DoUniqueNames(ds);
            UpdateData();
        }

        private void ProcessPackets(DirectoryInfo dir)
        {
            try
            {
                string variant = "Мониторинг закупок_Аналитика";
                SetDataSource(ParamKindTypes.Variant, string.Empty, 0, 0, variant, 0, string.Empty);
                PumpDataSource(dir);
            }
            finally
            {
                FileInfo[] files = dir.GetFiles("*.xml", SearchOption.AllDirectories);
                foreach (FileInfo file in files)
                    file.Delete();
            }
        }

        private void SetRegValue(string valueName, string value)
        {

            RegistryKey key = Registry.LocalMachine.CreateSubKey("Software\\Krista\\FM\\DataPumps\\EO7Pump");
            key.DeleteValue(valueName, false);
            key.SetValue(valueName, value);
        }

        protected override void DirectPumpData()
        {
            toSetHierarchy = false;
            // если файлов в каталоге для закачки нет - формируем пакет с запросом и постим его
            // затем вызывается метод DataExchangeSite - ProcessPacket, который сохраняет пакет в папке для закачки
            // и запускает закачку
            // если есть файлы (входящие пакеты) - закачиваем их
            if (this.RootDir.GetFiles("*.xml", SearchOption.AllDirectories).GetLength(0) == 0)
            {
                // записываем нужную инфу в реестр
                SetRegValue("RootDir", this.RootDir.FullName);
                SetRegValue("SchemeName", this.Scheme.Name);
                string tcp = string.Format("tcp://{0}:{1}/FMServer/Server.rem", this.Scheme.Server.Machine,
                    this.Scheme.Server.GetConfigurationParameter("ServerPort"));
                SetRegValue("tcp", tcp);

                PostPacket();
                return;
            }
            else
                ProcessPackets(this.RootDir);
        }

        #endregion перекрытые методы закачек

        #endregion закачка данных

        #region расчет кубов

        private const string TENDER_CUBE_GUID = "b0db161c-c330-4d1f-a57d-80fdb7004a32";
        private const string TENDER_CUBE_NAME = "ЭО_Мониторинг_Закупки";
        private const string REQUEST_CUBE_GUID = "2f59df06-aec4-424b-8dda-380b6cb7b9b6";
        private const string REQUEST_CUBE_NAME = "ЭО_Мониторинг_Заявки и Планы";
        private const string CONTRACT_CUBE_GUID = "39d001cf-f0eb-4e63-88f4-7e835aee403c";
        private const string CONTRACT_CUBE_NAME = "ЭО_Мониторинг_Контракты";
        private const string OFFER_CUBE_GUID = "66fff387-21de-4794-b477-f371a401abb9";
        private const string OFFER_CUBE_NAME = "ЭО_Мониторинг_Предложения поставщиков";
        private const string FIN_SOURCES_CUBE_GUID = "7c1264db-68c6-4669-84bd-0a575d86cc9d";
        private const string FIN_SOURCES_CUBE_NAME = "ЭО_Мониторинг_Источники финансирования";
        private const string SITE_USERS_CUBE_GUID = "ec0c6586-de57-4f5f-bf19-2142b3c2741b";
        private const string SITE_USERS_CUBE_NAME = "ЭО_Мониторинг_Пользователи сайта";
        private const string TENDER_PLAN_CUBE_GUID = "2f59df06-aec4-424b-8dda-380b6cb7b9b6";
        private const string TENDER_PLAN_CUBE_NAME = "ЭО_Мониторинг_Заявки и Планы по продукции";
        private const string TENDER_FILES_CUBE_GUID = "8178a3ab-0038-4b07-b6b3-1f5f9c411c16";
        private const string TENDER_FILES_CUBE_NAME = "ЭО_Мониторинг_Документация к закупке";
        private const string PRODUCT_CONTRACT_CUBE_GUID = "90bc1fda-d546-4b61-89c9-411abec91324";
        private const string PRODUCT_CONTRACT_CUBE_NAME = "ЭО_Мониторинг_Контракты по продукции";
        private const string FIN_CONTRACT_CUBE_GUID = "0b2cf244-2da8-489f-99fc-26e03a0dd821";
        private const string FIN_CONTRACT_CUBE_NAME = "ЭО_Мониторинг_Финансирование контрактов";
        private const string TENDER_LOW_SIZE_CUBE_GUID = "4f9b6a0d-3f64-42e6-971d-ffd32fc08789";
        private const string TENDER_LOW_SIZE_CUBE_NAME = "ЭО_Мониторинг_Закупки малого объема";
        private const string LOT_CUBE_GUID = "b5066d9d-ab58-4c96-ab03-060fee52f562";
        private const string LOT_CUBE_NAME = "ЭО_Мониторинг_Лоты";
        protected override void DirectProcessCube()
        {
            // расчет клс, используемых в закачке
            base.DirectProcessCube();
            // расчет измерений
            this.CubeClassifiers = new IClassifier[] { };
            
            // расчет кубов 
            this.CubeClassifiers = new IClassifier[] { };
            cubesForProcess = new string[] { TENDER_CUBE_GUID, TENDER_CUBE_NAME, REQUEST_CUBE_GUID, REQUEST_CUBE_NAME, 
                CONTRACT_CUBE_GUID, CONTRACT_CUBE_NAME, 
                OFFER_CUBE_GUID, OFFER_CUBE_NAME, FIN_SOURCES_CUBE_GUID, FIN_SOURCES_CUBE_NAME, 
                SITE_USERS_CUBE_GUID, SITE_USERS_CUBE_NAME, TENDER_PLAN_CUBE_GUID, TENDER_PLAN_CUBE_NAME, 
                TENDER_FILES_CUBE_GUID, TENDER_FILES_CUBE_NAME, PRODUCT_CONTRACT_CUBE_GUID, PRODUCT_CONTRACT_CUBE_NAME, 
                FIN_CONTRACT_CUBE_GUID, FIN_CONTRACT_CUBE_NAME, TENDER_LOW_SIZE_CUBE_GUID, TENDER_LOW_SIZE_CUBE_NAME, LOT_CUBE_GUID, LOT_CUBE_NAME };
            base.DirectProcessCube();
        }

        #endregion расчет кубов

    }

    #region DataExchangeSite

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("0F64A300-0D75-4ca4-96D8-7BFBB852181D")]
    public class DataExchangeSite : IDataExchangeSite, IDataExchangeSiteAsync
    {

        [MethodImpl(MethodImplOptions.Synchronized)]
        [ComVisible(true)]
        public void AsyncAbort()
        {
            // пока заглушка
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        [ComVisible(true)]
        public IDataExchangeSiteAsync GetAsyncInterface()
        {
            // заглушка 
            return null;
        }

        private string GetRegValue(string valueName)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\Krista\\FM\\DataPumps\\EO7Pump", false);
            return key.GetValue(valueName).ToString();
        }

        private const string REG_PERIOD_ENTITY_NAME = "D.STORDER.REGPERIOD";
        private const string ORG_ENTITY_NAME = "D.STORDER.ORGANIZATION";
        private const string CONTRACT_ENTITY_NAME = "D.STORDER.CONTRACT";
        private const string LOT_ENTITY_NAME = "D.STORDER.LOT";
        private const string LOT_ITEMS_ENTITY_NAME = "D.STORDER.LOTITEMS";
        private const string CONTRACT_ITEMS_ENTITY_NAME = "D.STORDER.CONTRACTITEMS";
        private const string FIN_DISTR_ENTITY_NAME = "D.STORDER.FINANCEDISTRIBUTION";
        private int GetPartIndex(IDataExchangeInboundPacket inboundPacket)
        {
            string[] entities = new string[] { REG_PERIOD_ENTITY_NAME, ORG_ENTITY_NAME, CONTRACT_ENTITY_NAME, 
                LOT_ENTITY_NAME, LOT_ITEMS_ENTITY_NAME, CONTRACT_ITEMS_ENTITY_NAME, FIN_DISTR_ENTITY_NAME };
            for (int i = 0; i < entities.GetLength(0); i++)
            {
                string entityName = entities[i];
                IDataExchangeSeekerRecordset recordSet = inboundPacket.FindFirstRecordset(entityName, true, true);
                if (recordSet != null)
                    return i + 1;
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        [ComVisible(true)]
        public void ProcessPacket(IDataExchangeInboundPacket inboundPacket)
        {
            // сохраняем пакет в папку для закачки и вызываем закачку
            string exchangeSource = inboundPacket.ExchangeSource;
            string rootDir = GetRegValue("RootDir");
            string tcp = GetRegValue("tcp");
            string schemeName = GetRegValue("SchemeName");

            int partIndex = GetPartIndex(inboundPacket);
            string fileName = string.Format("{0}\\{1}-{2}.xml", rootDir, exchangeSource, partIndex);
            try
            {
                (inboundPacket as IDataExchangePacketSerializer).SerializeTo(fileName);
            }
            catch
            {
                throw new Exception(string.Format("Ошибка при сохранении в файл: {0}", fileName));
            }

            DirectoryInfo sourceDir = new DirectoryInfo(rootDir);
            // закачиваем только после передачи последней части
            if (sourceDir.GetFiles("*.xml", SearchOption.TopDirectoryOnly).GetLength(0) == 7)
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                Assembly assembly = AppDomain.CurrentDomain.Load("Krista.FM.Server.DataPumps.EO7Pump");
                string assName = assembly.Location;
                psi.RedirectStandardOutput = true;
                psi.UseShellExecute = false;
                string workDir = assName.Substring(0, assName.LastIndexOf('\\'));
                psi.WorkingDirectory = workDir;
                DirectoryInfo dir = new DirectoryInfo(workDir);
                FileInfo[] fi = dir.GetFiles("runEO7Pump.bat", SearchOption.AllDirectories);
                if (fi.GetLength(0) == 0)
                {
                    // запускаем обычную закачку
                    psi.Arguments = string.Format("\"{0}\" \"{1}\" {2} {3} \"{4}\"",
                        tcp, schemeName, "EO7Pump", "PumpData", string.Empty);
                    psi.FileName = string.Concat(workDir, "\\Krista.FM.Server.DataPumpHost.exe");
                }
                else
                {
                    // запускаем закачку через бат файл, он в свою очередь запускает 32 битную консоль (для запуска на 64 бит ос)
                    psi.FileName = fi[0].FullName;
                }
                Process prc = Process.Start(psi);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        [ComVisible(true)]
        public void UndeliveredPacket(IDataExchangeOutboundPacket outboundPacket, string reasons)
        {
            // пока заглушка
        }

    }

    #endregion IDataExchangeSite

}


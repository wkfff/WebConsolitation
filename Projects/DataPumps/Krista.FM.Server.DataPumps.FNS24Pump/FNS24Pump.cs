using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;
using System.Xml;

namespace Krista.FM.Server.DataPumps.FNS24Pump
{
    // ФНС 24 - Реестр ЮЛ и ИП - закачка
    public partial class FNS24PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Организации.ЕГРЮЛ (b_Org_EGRUL)
        private IDbDataAdapter daEGRUL;
        private DataSet dsEGRUL;
        private IClassifier clsEGRUL;
        private Dictionary<string, DataRow[]> cacheEGRUL = null;
        private Dictionary<int, string> cacheEGRULOgrn = null;
        private int? clsEGRULVersion = -1;
        // Индивидуальные предприниматели.ЕГРИП (b_IP_EGRIP)
        private IDbDataAdapter daEGRIP;
        private DataSet dsEGRIP;
        private IClassifier clsEGRIP;
        private Dictionary<string, DataRow[]> cacheEGRIP = null;
        private int? clsEGRIPVersion = -1;
        // ОК.КЛАДР (b_OK_KLADR)
        private IDbDataAdapter daOKKladr;
        private DataSet dsOKKladr;
        private IClassifier clsOKKladr;
        private Dictionary<string, string> cacheOKKladrCode = null;
        private Dictionary<string, string> cacheOKKladrIndex = null;
        private int? clsOKKladrVersion = -1;

        // ОКОПФ.Сопоставимый (b_OKOPF_Bridge)
        private IDbDataAdapter daOkopfBridge;
        private DataSet dsOkopfBridge;
        private IClassifier clsOkopfBridge;
        private Dictionary<string, int> cacheOkopfBridge = null;
        private int? clsOkopfVersion = -1;
        // ОКСМ.Сопоставимый (b_OKSM_Bridge)
        private IDbDataAdapter daOksmBridge;
        private DataSet dsOksmBridge;
        private IClassifier clsOksmBridge;
        private Dictionary<string, int> cacheOksmBridge = null;
        private int? clsOksmVersion = -1;
        // КОПФ.Сопоставимый (b_KOPF_Bridge)
        private IDbDataAdapter daKopfBridge;
        private DataSet dsKopfBridge;
        private IClassifier clsKopfBridge;
        private Dictionary<string, int> cacheKopfBridge = null;
        private int? clsKopfVersion = -1;

        #region классификаторы

        // Виды.Банковские счета (b_Types_BankAcc)
        private IDbDataAdapter daTypesBankAcc;
        private DataSet dsTypesBankAcc;
        private IClassifier clsTypesBankAcc;
        private Dictionary<string, int> cacheTypesBankAcc = null;
        private int? clsTypesBankAccVersion = -1;
        // Виды.Лицензии (b_Types_Licenz)
        private IDbDataAdapter daTypesLicenz;
        private DataSet dsTypesLicenz;
        private IEntity clsTypesLicenz;
        private Dictionary<string, int> cacheTypesLicenz = null;
        private int? clsTypesLicenzVersion = -1;
        // Виды.Регистрации (b_Types_Register)
        private IDbDataAdapter daTypesRegister;
        private DataSet dsTypesRegister;
        private IClassifier clsTypesRegister;
        private Dictionary<string, int> cacheTypesRegister = null;
        private int? clsTypesRegisterVersion = -1;

        // Документы.Основание приобретения дееспособности (b_Doc_BaseAbility)
        private IDbDataAdapter daDocBaseAbility;
        private DataSet dsDocBaseAbility;
        private IClassifier clsDocBaseAbility;
        private Dictionary<string, int> cacheDocBaseAbility = null;
        private int? clsDocBaseAbilityVersion = -1;
        // Документы.Дающие право на проживание в РФ (b_Doc_HabitatRF)
        private IDbDataAdapter daDocHabitatRF;
        private DataSet dsDocHabitatRF;
        private IClassifier clsDocHabitatRF;
        private Dictionary<string, int> cacheDocHabitatRF = null;
        private int? clsDocHabitatRFVersion = -1;
        // Документы.Удостоверяющие личность (b_Doc_KindPerson)
        private IDbDataAdapter daDocKindPerson;
        private DataSet dsDocKindPerson;
        private IClassifier clsDocKindPerson;
        private Dictionary<string, int> cacheDocKindPerson = null;
        private int? clsDocKindPersonVersion = -1;
        // Документы.Подтверждающие приобретение дееспособности (b_Doc_RightAbility)
        private IDbDataAdapter daDocRightAbility;
        private DataSet dsDocRightAbility;
        private IClassifier clsDocRightAbility;
        private Dictionary<string, int> cacheDocRightAbility = null;
        private int? clsDocRightAbilityVersion = -1;

        // Индивидуальные предприниматели.Виды деятельности (b_IP_KindFunction)
        private IDbDataAdapter daIPKindFunction;
        private DataSet dsIPKindFunction;
        private IClassifier clsIPKindFunction;
        private Dictionary<string, int> cacheIPKindFunction = null;
        private int? clsIPKindFunctionVersion = -1;
        // Индивидуальные предприниматели.Виды статусов (b_IP_Status)
        private IDbDataAdapter daIPStatus;
        private DataSet dsIPStatus;
        private IClassifier clsIPStatus;
        private Dictionary<string, int> cacheIPStatus = null;
        private int? clsIPStatusVersion = -1;

        // ОК.Гражданство (b_OK_Citizen)
        private IDbDataAdapter daOKCitizen;
        private DataSet dsOKCitizen;
        private IClassifier clsOKCitizen;
        private Dictionary<string, int> cacheOKCitizen = null;
        private int? clsOKCitizenVersion = -1;
        // ОКИН.Языки народов РФ и иностранные языки (b_OKIN_Language)
        private IDbDataAdapter daOKINLanguage;
        private DataSet dsOKINLanguage;
        private IClassifier clsOKINLanguage;
        private Dictionary<string, int> cacheOKINLanguage = null;
        private int? clsOKINLanguageVersion = -1;

        // Организации.Адреса (d_Org_Adress)
        private IDbDataAdapter daOrgAdress;
        private DataSet dsOrgAdress;
        private IClassifier clsOrgAdress;
        private Dictionary<int, DataRow> cacheOrgAdress = null;
        // Организации.Виды капитала (b_Org_KindCapital)
        private IDbDataAdapter daOrgKindCapital;
        private DataSet dsOrgKindCapital;
        private IClassifier clsOrgKindCapital;
        private Dictionary<string, int> cacheOrgKindCapital = null;
        private int? clsOrgKindCapitalVersion = -1;
        // Организации.Виды подразделений (b_Org_KindSubdivision)
        private IDbDataAdapter daOrgKindSubdivision;
        private DataSet dsOrgKindSubdivision;
        private IClassifier clsOrgKindSubdivision;
        private Dictionary<string, int> cacheOrgKindSubdivision = null;
        private int? clsOrgKindSubdivisionVersion = -1;
        // Организации.Виды статусов (b_Org_Status)
        private IDbDataAdapter daOrgStatus;
        private DataSet dsOrgStatus;
        private IClassifier clsOrgStatus;
        private Dictionary<string, int> cacheOrgStatus = null;
        private int? clsOrgStatusVersion = -1;
        // Организации.Лицензирующие органы (b_Org_LicenceOrgan)
        private IDbDataAdapter daOrgLicenceOrgan;
        private DataSet dsOrgLicenceOrgan;
        private IClassifier clsOrgLicenceOrgan;
        private Dictionary<string, int> cacheOrgLicenceOrgan = null;
        private int? clsOrgLicenceOrganVersion = -1;
        // Организации.Налоговые органы (b_Org_MNS)
        private IDbDataAdapter daOrgMNS;
        private DataSet dsOrgMNS;
        private IClassifier clsOrgMNS;
        private Dictionary<string, int> cacheOrgMNS = null;
        private int? clsOrgMNSVersion = -1;
        // Организации.Органы ПФ (b_Org_OrganPF)
        private IDbDataAdapter daOrgOrganPF;
        private DataSet dsOrgOrganPF;
        private IClassifier clsOrgOrganPF;
        private Dictionary<string, int> cacheOrgOrganPF = null;
        private int? clsOrgOrganPFVersion = -1;
        // Организации.Органы ФСС (b_Org_OrganFSS)
        private IDbDataAdapter daOrgOrganFSS;
        private DataSet dsOrgOrganFSS;
        private IClassifier clsOrgOrganFSS;
        private Dictionary<string, int> cacheOrgOrganFSS = null;
        private int? clsOrgOrganFSSVersion = -1;
        // Организации.Органы ФОМС (b_Org_OrganFOMS)
        private IDbDataAdapter daOrgOrganFOMS;
        private DataSet dsOrgOrganFOMS;
        private IClassifier clsOrgOrganFOMS;
        private Dictionary<string, int> cacheOrgOrganFOMS = null;
        private int? clsOrgOrganFOMSVersion = -1;
        // Организации.Регистрирующие органы (b_Org_RegisterOrgan)
        private IDbDataAdapter daOrgRegisterOrgan;
        private DataSet dsOrgRegisterOrgan;
        private IClassifier clsOrgRegisterOrgan;
        private Dictionary<string, int> cacheOrgRegisterOrgan = null;
        private int? clsOrgRegisterOrganVersion = -1;

        // Состояние.Лицензии (b_State_Licence)
        private IDbDataAdapter daStateLicence;
        private DataSet dsStateLicence;
        private IClassifier clsStateLicence;
        private Dictionary<string, int> cacheStateLicence = null;
        private int? clsStateLicenceVersion = -1;
        // Состояние.Регистрационные записи (b_State_RegisterNote)
        private IDbDataAdapter daStateRegisterNote;
        private DataSet dsStateRegisterNote;
        private IClassifier clsStateRegisterNote;
        private Dictionary<string, int> cacheStateRegisterNote = null;
        private int? clsStateRegisterNoteVersion = -1;

        // Тип.Банковские счета (b_Kind_BankAcc)
        private IDbDataAdapter daKindBankAcc;
        private DataSet dsKindBankAcc;
        private IClassifier clsKindBankAcc;
        private Dictionary<string, int> cacheKindBankAcc = null;
        private int? clsKindBankAccVersion = -1;

        #endregion классификаторы

        #region детали для юридических лиц

        // Организации.Учредители юридические лица (t_Org_FounderOrg)
        private IDbDataAdapter daFounderOrgDet;
        private DataSet dsFounderOrgDet;
        private IEntity clsFounderOrgDet;
        private Dictionary<string, DataRow[]> cacheFounderOrgDet = null;
        // Организации.Учредители физические лица (t_Org_FounderFL)
        private IDbDataAdapter daFounderFLDet;
        private DataSet dsFounderFLDet;
        private IEntity clsFounderFLDet;
        private Dictionary<string, DataRow[]> cacheFounderFLDet = null;
        // Организации.Учредители иностранные организации (t_Org_FounderForeignOrg)
        private IDbDataAdapter daFounderForeignOrgDet;
        private DataSet dsFounderForeignOrgDet;
        private IEntity clsFounderForeignOrgDet;
        private Dictionary<string, DataRow[]> cacheFounderForeignOrgDet = null;
        // Организации.Правопреемники (t_Org_Assign)
        private IDbDataAdapter daAssignDet;
        private DataSet dsAssignDet;
        private IEntity clsAssignDet;
        private Dictionary<string, DataRow[]> cacheAssignDet = null;
        // Организации.Предшественники (t_Organizations_Ancestor)
        private IDbDataAdapter daAncestorDet;
        private DataSet dsAncestorDet;
        private IEntity clsAncestorDet;
        private Dictionary<string, DataRow[]> cacheAncestorDet = null;
        // Организации.Подразделения (t_Org_Subdivision)
        private IDbDataAdapter daSubdivisionDet;
        private DataSet dsSubdivisionDet;
        private IEntity clsSubdivisionDet;
        private Dictionary<string, DataRow[]> cacheSubdivisionDet = null;
        // Организации.Лицензии (t_Org_Licence)
        private IDbDataAdapter daLicenceDet;
        private DataSet dsLicenceDet;
        private IEntity clsLicenceDet;
        private Dictionary<string, DataRow[]> cacheLicenceDet = null;
        // Сведения о записях в ЕГРЮЛ (t_Org_Note)
        private IDbDataAdapter daNoteDet;
        private DataSet dsNoteDet;
        private IEntity clsNoteDet;
        private Dictionary<string, DataRow[]> cacheNoteDet = null;
        // Организации.ОКВЭД (t_Org_OKVED)
        private IDbDataAdapter daOKVEDDet;
        private DataSet dsOKVEDDet;
        private IEntity clsOKVEDDet;
        private Dictionary<string, DataRow[]> cacheOKVEDDet = null;
        // Организации.Счета (t_Org_Accounts)
        private IDbDataAdapter daAccountsDet;
        private DataSet dsAccountsDet;
        private IEntity clsAccountsDet;
        private Dictionary<string, DataRow[]> cacheAccountsDet = null;
        // Организации.Лица действующие без доверенности (t_Org_FaceProxy)
        private IDbDataAdapter daFaceProxyDet;
        private DataSet dsFaceProxyDet;
        private IEntity clsFaceProxyDet;
        private Dictionary<string, DataRow[]> cacheFaceProxyDet = null;

        #endregion детали для юридических лиц

        #region детали для индивидуальных предпринимателей

        // Индивидуальные предприниматели.Лицензии (t_IP_Licence)
        private IDbDataAdapter daIPLicenceDet;
        private DataSet dsIPLicenceDet;
        private IEntity clsIPLicenceDet;
        private Dictionary<string, DataRow[]> cacheIPLicenceDet = null;
        // Индивидуальные предприниматели.Сведения о записях в ЕГРИП (t_IP_Note)
        private IDbDataAdapter daIPNoteDet;
        private DataSet dsIPNoteDet;
        private IEntity clsIPNoteDet;
        private Dictionary<string, DataRow[]> cacheIPNoteDet = null;
        // Индивидуальные предприниматели.ОКВЭД (t_IP_OKVED)
        private IDbDataAdapter daIPOkvedDet;
        private DataSet dsIPOkvedDet;
        private IEntity clsIPOkvedDet;
        private Dictionary<string, DataRow[]> cacheIPOkvedDet = null;
        // Индивидуальные предприниматели.Счета (t_IP_Accounts)
        private IDbDataAdapter daIPAccountsDet;
        private DataSet dsIPAccountsDet;
        private IEntity clsIPAccountsDet;
        private Dictionary<string, DataRow[]> cacheIPAccountsDet = null;

        #endregion детали для индивидуальных предпринимателей

        #endregion Классификаторы

        List<string> oksmAbsentCodes = new List<string>();

        private bool isDeleted = false;
        private bool updateMode = true;

        private Database dbfDatabase = null;
        protected DBDataAccess dbDataAccess = new DBDataAccess();

        private string pumpedFilename = string.Empty;
        private string currentOgrn = string.Empty;
        private string currentIDDoc = string.Empty;

        #endregion Поля

        #region Закачка данных

        #region Закачка юридических лиц

        private Dictionary<string, Detail> InitULDetails()
        {
            Dictionary<string, Detail> details = new Dictionary<string, Detail>();

            details.Add("FounderOrg", new Detail(clsFounderOrgDet, dsFounderOrgDet,
                cacheFounderOrgDet, new string[] { "OGRN", "INN", "INN20", "RefOrgEGRUL" }));
            details.Add("FounderFL", new Detail(clsFounderFLDet, dsFounderFLDet,
                cacheFounderFLDet, new string[] { "FIO", "RefOrgEGRUL" }));
            details.Add("FounderForeignOrg", new Detail(clsFounderForeignOrgDet, dsFounderForeignOrgDet,
                cacheFounderForeignOrgDet, new string[] { "NameP", "RefOrgEGRUL" }));
            details.Add("Assign", new Detail(clsAssignDet, dsAssignDet,
                cacheAssignDet, new string[] { "OGRN", "INN", "INN20", "RefOrgEGRUL" }));
            details.Add("Ancestor", new Detail(clsAncestorDet, dsAncestorDet,
                cacheAncestorDet, new string[] { "OGRN", "INN", "INN20", "RefOrgEGRUL" }));
            details.Add("Subdivision", new Detail(clsSubdivisionDet, dsSubdivisionDet,
                cacheSubdivisionDet, new string[] { "Adress", "RefOrgEGRUL" }));
            details.Add("Licence", new Detail(clsLicenceDet, dsLicenceDet,
                cacheLicenceDet, new string[] { "Nomer", "RefOrgEGRUL" }));
            details.Add("Note", new Detail(clsNoteDet, dsNoteDet,
                cacheNoteDet, new string[] { "NumberReg", "RefOrgEGRUL" }));
            details.Add("OKVED", new Detail(clsOKVEDDet, dsOKVEDDet,
                cacheOKVEDDet, new string[] { "Code", "RefOrgEGRUL" }));
            details.Add("Accounts", new Detail(clsAccountsDet, dsAccountsDet,
                cacheAccountsDet, new string[] { "OGRN", "INN", "INN20", "RefOrgEGRUL" }));
            details.Add("FaceProxy", new Detail(clsFaceProxyDet, dsFaceProxyDet,
                cacheFaceProxyDet, new string[] { "FIO", "RefOrgEGRUL" }));

            return details;
        }

        private void DeleteULData()
        {
            DeleteTableData(clsFounderOrgDet, -1, -1, string.Empty);
            DeleteTableData(clsFounderFLDet, -1, -1, string.Empty);
            DeleteTableData(clsFounderForeignOrgDet, -1, -1, string.Empty);
            DeleteTableData(clsAssignDet, -1, -1, string.Empty);
            DeleteTableData(clsAncestorDet, -1, -1, string.Empty);
            DeleteTableData(clsSubdivisionDet, -1, -1, string.Empty);
            DeleteTableData(clsLicenceDet, -1, -1, string.Empty);
            DeleteTableData(clsNoteDet, -1, -1, string.Empty);
            DeleteTableData(clsOKVEDDet, -1, -1, string.Empty);
            DeleteTableData(clsFaceProxyDet, -1, -1, string.Empty);

            IClassifier[] usedClassifiers = new IClassifier[] {
                clsEGRUL, clsDocKindPerson, clsOrgKindSubdivision, clsTypesBankAcc, 
                clsKindBankAcc, clsDocHabitatRF, clsOrgKindCapital, clsOrgStatus };
            DirectDeleteClsData(usedClassifiers, -1, -1, string.Empty);
        }

        private void PumpULReport(FileInfo ulReport)
        {
            if (dsOksmBridge.Tables[0].Rows.Count <= 1)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    "Не заполнен классификатор 'ОКСМ.Сопоставимый'. Заполните этот классификатор и запустите закачку еще раз.");
                throw new Exception("Не заполнен классификатор 'ОКСМ.Сопоставимый'");
            }

            if (dsOkopfBridge.Tables[0].Rows.Count <= 1)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    "Не заполнен классификатор 'ОКОПФ.Сопоставимый'. Заполните этот классификатор и запустите закачку еще раз.");
                throw new Exception("Не заполнен классификатор 'ОКОПФ.Сопоставимый'");
            }

            if (dsKopfBridge.Tables[0].Rows.Count <= 1)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    "Не заполнен классификатор 'КОПФ.Сопоставимый'. Заполните этот классификатор и запустите закачку еще раз.");
                throw new Exception("Не заполнен классификатор 'КОПФ.Сопоставимый'");
            }

            if ((this.DeleteEarlierData) && (!isDeleted))
            {
                updateMode = false;
                DeleteULData();
                QueryData();
                isDeleted = true;
            }

            XmlTextReader reader = GetXMLReader(ulReport.FullName);
            pumpedFilename = ulReport.Name;

            bool isStartUL = false;
            bool isFinishUL = false;
            bool toUlUpr = false;
            bool toUlAddr = false;
            bool isNational = false;
            bool toRul = false;
            bool toPreem = false;
            bool toPredsh = false;
            bool toUlOb = false;
            bool toDOLGNFL = false;
            bool toErgul = false;
            //bool toIUl = false;

            // признаки для пропуска записей соответствующих классификаторов
            // в случае обнаружения некорректных ИНН
            bool toSkipUl = false;
            bool toSkipFounderOrg = false;
            bool toSkipFounderFL = false;
            bool toSkipAssign = false;
            bool toSkipAncestor = false;
            bool toSkipAccounts = false;
            bool toSkipFaceProxy = false;

            // основные данные и детали
            Dictionary<string, object> ulMapping = new Dictionary<string, object>();
            Dictionary<string, Detail> ulDetails = InitULDetails();
            Dictionary<string, object> founderOrgMapping = new Dictionary<string, object>();
            Dictionary<string, object> founderFLMapping = new Dictionary<string, object>();
            Dictionary<string, object> founderForeignOrgMapping = new Dictionary<string, object>();
            Dictionary<string, object> assignMapping = new Dictionary<string, object>();
            Dictionary<string, object> ancestorMapping = new Dictionary<string, object>();
            Dictionary<string, object> subdivisionMapping = new Dictionary<string, object>();
            Dictionary<string, object> licenceMapping = new Dictionary<string, object>();
            Dictionary<string, object> noteMapping = new Dictionary<string, object>();
            Dictionary<string, object> okvedMapping = new Dictionary<string, object>();
            Dictionary<string, object> accountsMapping = new Dictionary<string, object>();
            Dictionary<string, object> faceProxyMapping = new Dictionary<string, object>();

            // устанавливаем всем имеющимся записям статус "Не изменилась"
            SetOldStatus(dsEGRUL);
            foreach (Detail detail in ulDetails.Values)
            {
                SetOldStatus(detail.dataSet);
            }

            OrgParams orgParams = new OrgParams();
            Adress adress = new Adress();
            Contact contact = new Contact();
            FIO fio = new FIO();

            try
            {
                while (reader.Read())
                    switch (reader.NodeType)
                    {
                        #region XmlNodeType.Element
                        case XmlNodeType.Element:
                            switch (reader.LocalName)
                            {
                                #region ELEM_UL
                                case ELEM_UL:
                                    // очищаем вспомогательные переменные 
                                    ulMapping.Clear();
                                    AddMappingElement("SOURCEID", clsEGRULVersion, ulMapping);
                                    orgParams.ClearParams();
                                    currentOgrn = reader.GetAttribute(ATTR_OGRN);
                                    currentIDDoc = reader.GetAttribute(ATTR_IDDOK);
                                    string inn = reader.GetAttribute(ATTR_INN);
                                    string inn20 = reader.GetAttribute(ATTR_KPP);
                                    if (!ValidateDecimal(currentOgrn))
                                    {
                                        WriteIncorrectOgrnUL(inn, inn20);
                                        currentOgrn = "0";
                                    }
                                    AddMappingElement("OGRN", currentOgrn, ulMapping);
                                    if (!ValidateLong(inn) && !string.IsNullOrEmpty(inn))
                                    {
                                        WriteIncorrectInn(inn, clsEGRUL);
                                        toSkipUl = true;
                                    }
                                    AddMappingElement("INN", TryGetLongValue(inn), ulMapping);
                                    AddMappingElement("INN20", TryGetLongValue(inn20), ulMapping);
                                    break;
                                #endregion ELEM_UL

                                #region ELEM_ACCOUNT
                                case ELEM_ACCOUNT:
                                    AddMappingElement("NUMACC", reader.GetAttribute(ATTR_NUM), accountsMapping);
                                    AddMappingElement("OGRN", TryGetDecimalValue(reader.GetAttribute(ATTR_OGRN)), accountsMapping);
                                    string innAccount = reader.GetAttribute(ATTR_INN);
                                    if (!ValidateLong(innAccount) && !string.IsNullOrEmpty(innAccount))
                                    {
                                        WriteIncorrectInn(innAccount, clsAccountsDet);
                                        toSkipAccounts = true;
                                    }
                                    AddMappingElement("INN", TryGetLongValue(reader.GetAttribute(ATTR_INN)), accountsMapping);
                                    AddMappingElement("INN20", TryGetLongValue(reader.GetAttribute(ATTR_KPP)), accountsMapping);
                                    AddMappingElement("BIK", reader.GetAttribute(ATTR_BIK), accountsMapping);
                                    AddMappingElement("NUMCONTRACT", reader.GetAttribute(ATTR_NUM_CONTRACT), accountsMapping);
                                    AddMappingElement("DATESTARTCONTR", reader.GetAttribute(ATTR_DTSTART), accountsMapping);
                                    AddMappingElement("DATEENDCONTR", reader.GetAttribute(ATTR_DTEND), accountsMapping);

                                    string id_VA = reader.GetAttribute(ATTR_ID_VA);
                                    if (string.IsNullOrEmpty(id_VA))
                                        id_VA = "0";
                                    string name_VA = reader.GetAttribute(ATTR_NAME_VA);
                                    if (string.IsNullOrEmpty(name_VA))
                                        name_VA = constDefaultClsName;
                                    int refTypeAcc = PumpCachedRow(cacheTypesBankAcc, dsTypesBankAcc.Tables[0], clsTypesBankAcc, id_VA,
                                        new object[] { "CODE", id_VA, "NAME", name_VA, "SOURCEID", clsTypesBankAccVersion });
                                    AddMappingElement("REFTYPEACC", refTypeAcc, accountsMapping);

                                    string ID_TA = reader.GetAttribute(ATTR_ID_TA);
                                    if (string.IsNullOrEmpty(ID_TA))
                                        ID_TA = "0";
                                    string name_TA = reader.GetAttribute(ATTR_NAME_TA);
                                    if (string.IsNullOrEmpty(name_TA))
                                        name_TA = constDefaultClsName;
                                    int refKindAcc = PumpCachedRow(cacheKindBankAcc, dsKindBankAcc.Tables[0], clsKindBankAcc, ID_TA,
                                        new object[] { "CODE", ID_TA, "NAME", name_TA, "SOURCEID", clsKindBankAccVersion });
                                    AddMappingElement("REFKINDACC", refKindAcc, accountsMapping);
                                    break;
                                #endregion ELEM_ACCOUNT
                                #region ELEM_ADDRESS
                                case ELEM_ADDRESS:
                                    adress.index = reader.GetAttribute(ATTR_INDEKS);
                                    adress.dom = reader.GetAttribute(ATTR_DOM);
                                    adress.korp = reader.GetAttribute(ATTR_KORP);
                                    adress.kvart = reader.GetAttribute(ATTR_KVART);
                                    string okato = reader.GetAttribute(ATTR_OKATO);
                                    if (string.IsNullOrEmpty(okato))
                                        okato = "0";
                                    if (toUlUpr)
                                    {
                                        AddMappingElement("OKATODIRECT", okato, ulMapping);
                                    }
                                    else if (toUlAddr)
                                    {
                                        AddMappingElement("OKATO", okato, ulMapping);
                                    }
                                    else if (toRul)
                                    {
                                        AddMappingElement("OKATO", okato, founderOrgMapping);
                                    }
                                    else if (toPreem)
                                    {
                                        AddMappingElement("OKATO", okato, assignMapping);
                                    }
                                    else if (toPredsh)
                                    {
                                        AddMappingElement("OKATO", okato, ancestorMapping);
                                    }
                                    else if (toUlOb)
                                    {
                                        AddMappingElement("OKATO", okato, subdivisionMapping);
                                    }
                                    break;
                                #endregion ELEM_ADDRESS
                                #region ELEM_CONTACT
                                case ELEM_CONTACT:
                                    contact.kodGorod = reader.GetAttribute(ATTR_KODGOROD);
                                    contact.telefon = reader.GetAttribute(ATTR_TELEFON);
                                    contact.fax = reader.GetAttribute(ATTR_FAX);

                                    if (toUlUpr)
                                        AddMappingElement("CONTACTDIRECT", contact.GetContact(), ulMapping);
                                    else if (toUlAddr)
                                        AddMappingElement("CONTACT", contact.GetContact(), ulMapping);
                                    else if (toUlOb)
                                        AddMappingElement("CONTACT", contact.GetContact(), subdivisionMapping);
                                    break;
                                #endregion ELEM_CONTACT
                                #region ELEM_DOCFL
                                case ELEM_DOCFL:
                                    string ser = reader.GetAttribute(ATTR_SER);
                                    string num = reader.GetAttribute(ATTR_NUM);
                                    string docNum = string.Empty;
                                    if (!string.IsNullOrEmpty(ser))
                                        docNum += string.Format("Серия: {0}", ser);
                                    if (!string.IsNullOrEmpty(num))
                                        docNum += string.Format("Номер: {0}", num);

                                    if (toDOLGNFL)
                                    {
                                        AddMappingElement("DOCNUM", docNum, faceProxyMapping);
                                        AddMappingElement("DATEDOC", reader.GetAttribute(ATTR_DT), faceProxyMapping);
                                        AddMappingElement("NAMEORG", reader.GetAttribute(ATTR_NAMEORG), faceProxyMapping);
                                        AddMappingElement("KODORG", reader.GetAttribute(ATTR_KODORG), faceProxyMapping);
                                    }
                                    else
                                    {
                                        AddMappingElement("DOCNUM", docNum, founderFLMapping);
                                        AddMappingElement("DATEDOC", reader.GetAttribute(ATTR_DT), founderFLMapping);
                                        AddMappingElement("NAMEORG", reader.GetAttribute(ATTR_NAMEORG), founderFLMapping);
                                        AddMappingElement("KODORG", reader.GetAttribute(ATTR_KODORG), founderFLMapping);
                                    }
                                    break;
                                #endregion ELEM_DOCFL
                                #region ELEM_DOLGNFL
                                case ELEM_DOLGNFL:
                                    toDOLGNFL = true;
                                    DateTime dateZapFace = Convert.ToDateTime(reader.GetAttribute(ATTR_DTSTART));
                                    AddMappingElement("DATEZAP", dateZapFace, faceProxyMapping);
                                    AddMappingElement("JOB", reader.GetAttribute(ATTR_DOLGN), faceProxyMapping);
                                    break;
                                #endregion ELEM_DOLGNFL
                                #region ELEM_FL
                                case ELEM_FL:
                                    fio.Fam = reader.GetAttribute(ATTR_FAM_FL);
                                    fio.Name = reader.GetAttribute(ATTR_NAME_FL);
                                    fio.Otch = reader.GetAttribute(ATTR_OTCH_FL);
                                    string innFl = reader.GetAttribute(ATTR_INN);
                                    if (toDOLGNFL)
                                    {
                                        if (!ValidateLong(innFl) && !string.IsNullOrEmpty(innFl))
                                        {
                                            WriteIncorrectInn(innFl, clsFaceProxyDet);
                                            toSkipFaceProxy = true;
                                        }
                                        AddMappingElement("INN", TryGetLongValue(innFl), faceProxyMapping);
                                        AddMappingElement("FIO", fio.GetFIO(), faceProxyMapping);
                                    }
                                    else
                                    {
                                        if (!ValidateLong(innFl) && !string.IsNullOrEmpty(innFl))
                                        {
                                            WriteIncorrectInn(innFl, clsFounderFLDet);
                                            toSkipFounderFL = true;
                                        }
                                        AddMappingElement("INN", TryGetLongValue(innFl), founderFLMapping);
                                        AddMappingElement("FIO", fio.GetFIO(), founderFLMapping);
                                    }
                                    fio.ClearFIO();
                                    break;
                                #endregion ELEM_FL
                                #region ELEM_FOMS
                                case ELEM_FOMS:
                                    AddMappingElement("DATESTARTFOMS", reader.GetAttribute(ATTR_DTSTART), ulMapping);
                                    AddMappingElement("DATEFINISHFOMS", reader.GetAttribute(ATTR_DTEND), ulMapping);
                                    AddMappingElement("REGNUMFOMS", reader.GetAttribute(ATTR_REGN_FOMS), ulMapping);
                                    break;
                                #endregion ELEM_FOMS
                                #region ELEM_FSS
                                case ELEM_FSS:
                                    AddMappingElement("DATESTARTFSS", reader.GetAttribute(ATTR_DTSTART), ulMapping);
                                    AddMappingElement("DATEFINISHFSS", reader.GetAttribute(ATTR_DTEND), ulMapping);
                                    AddMappingElement("REGNUMFSS", reader.GetAttribute(ATTR_REGN_FSS), ulMapping);
                                    break;
                                #endregion ELEM_FSS
                                #region ELEM_GOROD
                                case ELEM_GOROD:
                                    adress.gorod = reader.GetAttribute(ATTR_NAME);
                                    adress.gorodId = reader.GetAttribute(ATTR_ID);
                                    adress.gorodCode = reader.GetAttribute(ATTR_KOD_KL);
                                    break;
                                #endregion ELEM_GOROD
                                #region ELEM_IUL
                                case ELEM_IUL:
                                    string namepForeignOrg = reader.GetAttribute(ATTR_NAMEP);
                                    if (string.IsNullOrEmpty(namepForeignOrg))
                                        namepForeignOrg = constDefaultClsName;
                                    DateTime dateUchrForeign = Convert.ToDateTime(reader.GetAttribute(ATTR_DTSTART));
                                    AddMappingElement("NAMEP", namepForeignOrg, founderForeignOrgMapping);
                                    AddMappingElement("DATEUCHR", dateUchrForeign, founderForeignOrgMapping);
                                    AddMappingElement("DATEREG", reader.GetAttribute(ATTR_DTREG), founderForeignOrgMapping);
                                    AddMappingElement("ADRESS", reader.GetAttribute(ATTR_ADRESIN), founderForeignOrgMapping);
                                    AddMappingElement("SUMMA", TryGetDecimalValue(reader.GetAttribute(ATTR_SUMMA)), founderForeignOrgMapping);
                                    //toIUl = true;
                                    break;
                                #endregion ELEM_IUL
                                #region ELEM_LANG
                                case ELEM_LANG:
                                    string kodLang = reader.GetAttribute(ATTR_KOD_LANG);
                                    if (string.IsNullOrEmpty(kodLang))
                                        kodLang = "0";
                                    int langCode = 0;
                                    Int32.TryParse(kodLang, out langCode);
                                    string nameLang = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(nameLang))
                                        nameLang = constDefaultClsName;
                                    int refLang = PumpCachedRow(cacheOKINLanguage, dsOKINLanguage.Tables[0], clsOKINLanguage, langCode.ToString(),
                                        new object[] { "CODE", langCode, "NAME", nameLang, "SOURCEID", clsOKINLanguageVersion });
                                    if (isNational)
                                        AddMappingElement("REFNATLANG", refLang, ulMapping);
                                    else
                                        AddMappingElement("REFFORLANG", refLang, ulMapping);
                                    break;
                                #endregion ELEM_LANG
                                #region ELEM_LICENZ
                                case ELEM_LICENZ:
                                    AddMappingElement("NOMER", reader.GetAttribute(ATTR_NUMLIC), licenceMapping);
                                    AddMappingElement("DATELICENCE", reader.GetAttribute(ATTR_DTRESH), licenceMapping);
                                    AddMappingElement("DATESTARTLICENCE", reader.GetAttribute(ATTR_DTSTART), licenceMapping);
                                    AddMappingElement("DATEFINISHLICENCE", reader.GetAttribute(ATTR_DTEND), licenceMapping);
                                    AddMappingElement("DATESTOPLIC", reader.GetAttribute(ATTR_DTSTOP), licenceMapping);
                                    AddMappingElement("DATERESUME", reader.GetAttribute(ATTR_DTSTARTNOV), licenceMapping);
                                    break;
                                #endregion ELEM_LICENZ
                                #region ELEM_LICORG
                                case ELEM_LICORG:
                                    string licID = reader.GetAttribute(ATTR_ID);
                                    if (string.IsNullOrEmpty(licID))
                                        licID = "0";
                                    string licName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(licName))
                                        licName = constDefaultClsName;

                                    int refOrgLic = PumpCachedRow(cacheOrgLicenceOrgan, dsOrgLicenceOrgan.Tables[0], clsOrgLicenceOrgan, licID,
                                        new object[] { "CODE", licID, "NAME", licName, "SOURCEID", clsOrgLicenceOrganVersion });
                                    AddMappingElement("REFORGLIC", refOrgLic, licenceMapping);
                                    break;
                                #endregion ELEM_LICORG
                                #region ELEM_MNS
                                case ELEM_MNS:
                                    AddMappingElement("DATESTARTMNS", reader.GetAttribute(ATTR_DTSTART), ulMapping);
                                    AddMappingElement("DATEFINISHMNS", reader.GetAttribute(ATTR_DTEND), ulMapping);
                                    break;
                                #endregion ELEM_MNS
                                #region ELEM_NAMEI
                                case ELEM_NAMEI:
                                    AddMappingElement("FOREIGNNAME", reader.GetAttribute(ATTR_NAME), ulMapping);
                                    isNational = false;
                                    break;
                                #endregion ELEM_NAMEI
                                #region ELEM_NAMEN
                                case ELEM_NAMEN:
                                    AddMappingElement("NATIONALNAME", reader.GetAttribute(ATTR_NAME), ulMapping);
                                    isNational = true;
                                    break;
                                #endregion ELEM_NAMEN
                                #region ELEM_NASPUNKT
                                case ELEM_NASPUNKT:
                                    adress.nasPunkt = reader.GetAttribute(ATTR_NAME);
                                    adress.nasPunktId = reader.GetAttribute(ATTR_ID);
                                    adress.nasPunktCode = reader.GetAttribute(ATTR_KOD_KL);
                                    break;
                                #endregion ELEM_NASPUNKT
                                #region ELEM_OKSM
                                case ELEM_OKSM:
                                    string oksmID = reader.GetAttribute(ATTR_ID);
                                    if (string.IsNullOrEmpty(oksmID))
                                        oksmID = "0";
                                    int refOKSM = FindCachedRow(cacheOksmBridge, oksmID, -1);

                                    if (refOKSM == -1)
                                        if (!oksmAbsentCodes.Contains(oksmID))
                                            oksmAbsentCodes.Add(oksmID);
                                    //WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                                    //    string.Format("Запись с кодом {0} не обнаружена в классификаторе 'ОКСМ.Cопоставимый'", oksmID));

                                    AddMappingElement("REFOKSMBRIDGE", refOKSM, founderForeignOrgMapping);
                                    break;
                                #endregion ELEM_OKSM
                                #region ELEM_OKVED
                                case ELEM_OKVED:
                                    string codOKVED = reader.GetAttribute(ATTR_KOD_OKVED);
                                    FormatMainOKVED(ref codOKVED);
                                    AddMappingElement("CODE", codOKVED, okvedMapping);
                                    AddMappingElement("NAME", reader.GetAttribute(ATTR_NAME), okvedMapping);
                                    string mainOKVED = reader.GetAttribute(ATTR_MAIN);
                                    if (mainOKVED == "0")
                                        AddMappingElement("SIGN", "Дополнительный вид экономической деятельности", okvedMapping);
                                    else
                                    {
                                        AddMappingElement("MAINOKVED", codOKVED, ulMapping);
                                        AddMappingElement("SIGN", "Основной вид экономической деятельности", okvedMapping);
                                    }
                                    ulDetails["OKVED"].AddNewRow(okvedMapping);
                                    okvedMapping = new Dictionary<string, object>();
                                    break;
                                #endregion ELEM_OKVED
                                #region ELEM_OPF
                                case ELEM_OPF:
                                    string spr = reader.GetAttribute(ATTR_SPR);
                                    string opfCode = reader.GetAttribute(ATTR_KOD_OPF);
                                    if (string.IsNullOrEmpty(opfCode))
                                        opfCode = "-1";
                                    if (spr == "OKOPF")
                                    {
                                        AddMappingElement("REFKOPFBRIDGE", -1, ulMapping);
                                        AddMappingElement("REFOKOPFBRIDGE", FindCachedRow(cacheOkopfBridge, opfCode, -1), ulMapping);
                                    }
                                    else
                                    {
                                        AddMappingElement("REFOKOPFBRIDGE", -1, ulMapping);
                                        AddMappingElement("REFKOPFBRIDGE", FindCachedRow(cacheKopfBridge, opfCode, -1), ulMapping);
                                    }
                                    break;
                                #endregion ELEM_OPF
                                #region ELEM_ORGAN_FOMS
                                case ELEM_ORGAN_FOMS:
                                    string fomsKod = reader.GetAttribute(ATTR_KOD);
                                    if (string.IsNullOrEmpty(fomsKod))
                                        fomsKod = "0";
                                    int fomsCode = 0;
                                    Int32.TryParse(fomsKod, out fomsCode);

                                    string fomsName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(fomsName))
                                        fomsName = constDefaultClsName;

                                    int refOrgFOMS = PumpCachedRow(cacheOrgOrganFOMS, dsOrgOrganFOMS.Tables[0], clsOrgOrganFOMS, fomsCode.ToString(),
                                        new object[] { "CODE", fomsCode, "NAME", fomsName, "SOURCEID", clsOrgOrganFOMSVersion });
                                    AddMappingElement("REFORGFOMS", refOrgFOMS, ulMapping);
                                    break;
                                #endregion ELEM_ORGAN_FOMS
                                #region ELEM_ORGAN_FSS
                                case ELEM_ORGAN_FSS:
                                    string fssKod = reader.GetAttribute(ATTR_KOD);
                                    if (string.IsNullOrEmpty(fssKod))
                                        fssKod = "0";
                                    int fssCode = 0;
                                    Int32.TryParse(fssKod, out fssCode);
                                    string fssName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(fssName))
                                        fssName = constDefaultClsName;

                                    int refOrgFSS = PumpCachedRow(cacheOrgOrganFSS, dsOrgOrganFSS.Tables[0], clsOrgOrganFSS, fssCode.ToString(),
                                        new object[] { "CODE", fssCode, "NAME", fssName, "SOURCEID", clsOrgOrganFSSVersion });
                                    AddMappingElement("REFORGFSS", refOrgFSS, ulMapping);
                                    break;
                                #endregion ELEM_ORGAN_FSS
                                #region ELEM_ORGAN_MNS
                                case ELEM_ORGAN_MNS:
                                    string mnsKod = reader.GetAttribute(ATTR_KOD);
                                    if (string.IsNullOrEmpty(mnsKod))
                                        mnsKod = "0";
                                    int mnsCode = 0;
                                    Int32.TryParse(mnsKod, out mnsCode);
                                    string mnsName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(mnsName))
                                        mnsName = constDefaultClsName;
                                    int refOrgMns = PumpCachedRow(cacheOrgMNS, dsOrgMNS.Tables[0], clsOrgMNS, mnsCode.ToString(),
                                        new object[] { "CODE", mnsCode, "NAME", mnsName, "SOURCEID", clsOrgMNSVersion });
                                    AddMappingElement("REFORGMNS", refOrgMns, ulMapping);
                                    break;
                                #endregion ELEM_ORGAN_MNS
                                #region ELEM_ORGAN_PF
                                case ELEM_ORGAN_PF:
                                    string pfKod = reader.GetAttribute(ATTR_KOD);
                                    if (string.IsNullOrEmpty(pfKod))
                                        pfKod = "0";
                                    int pfCode = 0;
                                    Int32.TryParse(pfKod, out pfCode);
                                    string pfName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(pfName))
                                        pfName = constDefaultClsName;

                                    int refOrgPF = PumpCachedRow(cacheOrgOrganPF, dsOrgOrganPF.Tables[0], clsOrgOrganPF, pfCode.ToString(),
                                        new object[] { "CODE", pfCode, "NAME", pfName, "SOURCEID", clsOrgOrganPFVersion });
                                    AddMappingElement("REFORGPF", refOrgPF, ulMapping);
                                    break;
                                #endregion ELEM_ORGAN_PF
                                #region ELEM_PF
                                case ELEM_PF:
                                    AddMappingElement("DATESTARTPF", reader.GetAttribute(ATTR_DTSTART), ulMapping);
                                    AddMappingElement("DATEFINISHPF", reader.GetAttribute(ATTR_DTEND), ulMapping);
                                    AddMappingElement("REGNUMPF", reader.GetAttribute(ATTR_REGN_PF), ulMapping);
                                    break;
                                #endregion ELEM_PF
                                #region ELEM_PREDSH
                                case ELEM_PREDSH:
                                    toPredsh = true;
                                    AddMappingElement("OGRN", TryGetDecimalValue(reader.GetAttribute(ATTR_OGRN)), ancestorMapping);
                                    string innAncestor = reader.GetAttribute(ATTR_INN);
                                    if (!ValidateLong(innAncestor) && !string.IsNullOrEmpty(innAncestor))
                                    {
                                        WriteIncorrectInn(innAncestor, clsAncestorDet);
                                        toSkipAncestor = true;
                                    }
                                    AddMappingElement("INN", TryGetLongValue(innAncestor), ancestorMapping);
                                    AddMappingElement("INN20", TryGetLongValue(reader.GetAttribute(ATTR_KPP)), ancestorMapping);
                                    string namepAncestor = reader.GetAttribute(ATTR_NAMEP);
                                    if (string.IsNullOrEmpty(namepAncestor))
                                        namepAncestor = constDefaultClsName;
                                    AddMappingElement("NAMEP", namepAncestor, ancestorMapping);
                                    AddMappingElement("DATEOGRN", reader.GetAttribute(ATTR_DTOGRN), ancestorMapping);
                                    AddMappingElement("REGNUM", reader.GetAttribute(ATTR_NUMST), ancestorMapping);
                                    AddMappingElement("DATESTARTUL", reader.GetAttribute(ATTR_DTREG), ancestorMapping);
                                    break;
                                #endregion ELEM_PREDSH
                                #region ELEM_PREEM
                                case ELEM_PREEM:
                                    toPreem = true;
                                    AddMappingElement("OGRN", TryGetDecimalValue(reader.GetAttribute(ATTR_OGRN)), assignMapping);
                                    string innAssign = reader.GetAttribute(ATTR_INN);
                                    if (!ValidateLong(innAssign) && !string.IsNullOrEmpty(innAssign))
                                    {
                                        WriteIncorrectInn(innAssign, clsAssignDet);
                                        toSkipAssign = true;
                                    }
                                    AddMappingElement("INN", TryGetLongValue(innAssign), assignMapping);
                                    AddMappingElement("INN20", TryGetLongValue(reader.GetAttribute(ATTR_KPP)), assignMapping);
                                    string namepAssign = reader.GetAttribute(ATTR_NAMEP);
                                    if (string.IsNullOrEmpty(namepAssign))
                                        namepAssign = constDefaultClsName;
                                    AddMappingElement("NAMEP", namepAssign, assignMapping);
                                    AddMappingElement("DATEOGRN", reader.GetAttribute(ATTR_DTOGRN), assignMapping);
                                    AddMappingElement("REGNUM", reader.GetAttribute(ATTR_NUMST), assignMapping);
                                    AddMappingElement("DATESTARTUL", reader.GetAttribute(ATTR_DTREG), assignMapping);
                                    break;
                                #endregion ELEM_PREEM
                                #region ELEM_RAION
                                case ELEM_RAION:
                                    adress.raion = reader.GetAttribute(ATTR_NAME);
                                    adress.raionId = reader.GetAttribute(ATTR_ID);
                                    adress.raionCode = reader.GetAttribute(ATTR_KOD_KL);
                                    break;
                                #endregion ELEM_RAION
                                #region ELEM_REESTR_AO
                                case ELEM_REESTR_AO:
                                    AddMappingElement("OGRNREESTRAO", reader.GetAttribute(ATTR_OGRN), ulMapping);
                                    string nameReestrAO = reader.GetAttribute(ATTR_NAMEP);
                                    if (string.IsNullOrEmpty(nameReestrAO))
                                        nameReestrAO = constDefaultClsName;
                                    AddMappingElement("NAMEREESTRAO", nameReestrAO, ulMapping);
                                    break;
                                #endregion ELEM_REESTR_AO
                                #region ELEM_REGEGRUL
                                case ELEM_REGEGRUL:
                                    AddMappingElement("REGID", reader.GetAttribute(ATTR_IDREG), noteMapping);
                                    AddMappingElement("NUMBERREG", TryGetDecimalValue(reader.GetAttribute(ATTR_REGNUM)), noteMapping);
                                    AddMappingElement("DATEREG", reader.GetAttribute(ATTR_DTREG), noteMapping);
                                    AddMappingElement("DATENOTE", reader.GetAttribute(ATTR_DTZAP), noteMapping);
                                    toErgul = true;
                                    break;
                                #endregion ELEM_REGEGRUL
                                #region ELEM_REGION
                                case ELEM_REGION:
                                    adress.region = reader.GetAttribute(ATTR_NAME);
                                    adress.regionId = reader.GetAttribute(ATTR_ID);
                                    adress.regionCode = reader.GetAttribute(ATTR_KOD_KL);
                                    break;
                                #endregion ELEM_REGION
                                #region ELEM_REGORG
                                case ELEM_REGORG:
                                    string regorgID = reader.GetAttribute(ATTR_ID);
                                    string regorgName = reader.GetAttribute(ATTR_NAME);

                                    if (string.IsNullOrEmpty(regorgID))
                                        regorgID = "0";

                                    if (string.IsNullOrEmpty(regorgName))
                                        regorgName = constDefaultClsName;

                                    int refOrg = PumpCachedRow(cacheOrgRegisterOrgan, dsOrgRegisterOrgan.Tables[0], clsOrgRegisterOrgan, regorgID,
                                        new object[] { "CODE", regorgID, "NAME", regorgName, "SOURCEID", clsOrgRegisterOrganVersion });
                                    if (isStartUL)
                                    {
                                        AddMappingElement("REFORGROSTART", refOrg, ulMapping);
                                    }
                                    else if (isFinishUL)
                                    {
                                        AddMappingElement("REFORGROFINISH", refOrg, ulMapping);
                                    }
                                    else if (toRul)
                                    {
                                        AddMappingElement("REFORGRO", refOrg, founderOrgMapping);
                                    }
                                    else if (toPreem)
                                    {
                                        AddMappingElement("REFORGRO", refOrg, assignMapping);
                                    }
                                    else if (toPredsh)
                                    {
                                        AddMappingElement("REFORGRO", refOrg, ancestorMapping);
                                    }
                                    else if (toErgul)
                                    {
                                        AddMappingElement("REFORGRO", refOrg, noteMapping);
                                    }
                                    else if (toUlOb)
                                    {
                                        AddMappingElement("REFORGRO", refOrg, subdivisionMapping);
                                    }
                                    else
                                    {
                                        AddMappingElement("REFORGRO", refOrg, ulMapping);
                                    }
                                    break;
                                #endregion ELEM_REGORG
                                #region ELEM_RUL
                                case ELEM_RUL:
                                    toRul = true;
                                    AddMappingElement("OGRN", TryGetDecimalValue(reader.GetAttribute(ATTR_OGRN)), founderOrgMapping);
                                    string innFounderOrg = reader.GetAttribute(ATTR_INN);
                                    if (!ValidateLong(innFounderOrg) && !string.IsNullOrEmpty(innFounderOrg))
                                    {
                                        WriteIncorrectInn(innFounderOrg, clsFounderOrgDet);
                                        toSkipFounderOrg = true;
                                    }
                                    AddMappingElement("INN", TryGetLongValue(innFounderOrg), founderOrgMapping);
                                    AddMappingElement("INN20", TryGetLongValue(reader.GetAttribute(ATTR_KPP)), founderOrgMapping);
                                    string namepFounderOrg = reader.GetAttribute(ATTR_NAMEP);
                                    if (string.IsNullOrEmpty(namepFounderOrg))
                                        namepFounderOrg = constDefaultClsName;
                                    AddMappingElement("NAMEP", namepFounderOrg, founderOrgMapping);
                                    AddMappingElement("DATEUCHR", Convert.ToDateTime(reader.GetAttribute(ATTR_DTSTART)), founderOrgMapping);
                                    AddMappingElement("SUMMA", TryGetDecimalValue(reader.GetAttribute(ATTR_SUMMA)), founderOrgMapping);
                                    AddMappingElement("DATEOGRN", reader.GetAttribute(ATTR_DTOGRN), founderOrgMapping);
                                    AddMappingElement("REGNUM", reader.GetAttribute(ATTR_NUMST), founderOrgMapping);
                                    AddMappingElement("DATESTARTUL", reader.GetAttribute(ATTR_DTREG), founderOrgMapping);
                                    break;
                                #endregion ELEM_RUL
                                #region ELEM_SOSTLIC
                                case ELEM_SOSTLIC:
                                    licID = reader.GetAttribute(ATTR_ID);
                                    if (string.IsNullOrEmpty(licID))
                                        licID = "0";
                                    licName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(licName))
                                        licName = constDefaultClsName;
                                    int refStateLicence = PumpCachedRow(cacheStateLicence, dsStateLicence.Tables[0], clsStateLicence, licID,
                                        new object[] { "CODE", licID, "NAME", licName, "SOURCEID", clsStateLicenceVersion });
                                    AddMappingElement("REFSTATELICENCE", refStateLicence, licenceMapping);
                                    break;
                                #endregion ELEM_SOSTLIC
                                #region ELEM_SOSTZAP
                                case ELEM_SOSTZAP:
                                    string soszapID = reader.GetAttribute(ATTR_ID);
                                    if (string.IsNullOrEmpty(soszapID))
                                        soszapID = "0";
                                    string soszapName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(soszapName))
                                        soszapName = constDefaultClsName;
                                    string key = string.Format("{0}|{1}", soszapID, soszapName);
                                    int refSosZap = PumpCachedRow(cacheStateRegisterNote, dsStateRegisterNote.Tables[0], clsStateRegisterNote, key,
                                        new object[] { "CODE", soszapID, "NAME", soszapName, "SOURCEID", clsStateRegisterNoteVersion });
                                    if (toErgul)
                                        AddMappingElement("REFSTATEREG", refSosZap, noteMapping);
                                    break;
                                #endregion ELEM_SOSTZAP
                                #region ELEM_STATUS
                                case ELEM_STATUS:
                                    DateTime date = Convert.ToDateTime(reader.GetAttribute(ATTR_DTSTART));
                                    AddMappingElement("DATESTATUS", date, ulMapping);
                                    // ссылка на статус компании
                                    string statusCode = reader.GetAttribute(ATTR_ID);
                                    if (string.IsNullOrEmpty(statusCode))
                                        statusCode = "0";
                                    string statusName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(statusName))
                                        statusName = constDefaultClsName;

                                    int refStatus = PumpCachedRow(cacheOrgStatus, dsOrgStatus.Tables[0], clsOrgStatus, statusCode,
                                        new object[] { "CODE", statusCode, "NAME", statusName, "SOURCEID", clsOrgStatusVersion });
                                    AddMappingElement("REFORGSTATUS", refStatus, ulMapping);
                                    break;
                                #endregion ELEM_STATUS
                                #region ELEM_STREET
                                case ELEM_STREET:
                                    adress.street = reader.GetAttribute(ATTR_NAME);
                                    adress.streetId = reader.GetAttribute(ATTR_ID);
                                    adress.streetCode = reader.GetAttribute(ATTR_KOD_ST);
                                    break;
                                #endregion ELEM_STREET
                                #region ELEM_UCHR
                                case ELEM_UCHR:
                                    int cntRul = TryGetIntValue(reader.GetAttribute(ATTR_CNT_RUL));
                                    int cntIul = TryGetIntValue(reader.GetAttribute(ATTR_CNT_IUL));
                                    int cntUchrFl = TryGetIntValue(reader.GetAttribute(ATTR_CNT_UCHRFL));
                                    AddMappingElement("CNTRUL", cntRul, ulMapping);
                                    AddMappingElement("CNTIUL", cntIul, ulMapping);
                                    AddMappingElement("CNTUCHRFL", cntUchrFl, ulMapping);
                                    break;
                                #endregion ELEM_UCHR
                                #region ELEM_UCHRFL
                                case ELEM_UCHRFL:
                                    //toUCHRFL = true;
                                    DateTime dateUchrFl = Convert.ToDateTime(reader.GetAttribute(ATTR_DTSTART));
                                    AddMappingElement("SUMMA", TryGetDecimalValue(reader.GetAttribute(ATTR_SUMMA)), founderFLMapping);
                                    AddMappingElement("DATEUCHR", dateUchrFl, founderFLMapping);
                                    break;
                                #endregion ELEM_UCHRFL
                                #region ELEM_UL_ADDRESS
                                case ELEM_UL_ADDRESS:
                                    toUlAddr = true;
                                    DateTime dateAdress = Convert.ToDateTime(reader.GetAttribute(ATTR_DTSTART));
                                    AddMappingElement("DATEADRESS", dateAdress, ulMapping);
                                    AddMappingElement("NAMEISPORG", reader.GetAttribute(ATTR_NAMEISPORG), ulMapping);
                                    break;
                                #endregion ELEM_UL_ADDRESS
                                #region ELEM_UL_CAPITAL
                                case ELEM_UL_CAPITAL:
                                    DateTime dateCapital = Convert.ToDateTime(reader.GetAttribute(ATTR_DTSTART));
                                    AddMappingElement("DATECAPITAL", dateCapital, ulMapping);
                                    AddMappingElement("SUMCAPITAL", TryGetDecimalValue(reader.GetAttribute(ATTR_SUMMA)), ulMapping);
                                    break;
                                #endregion ELEM_UL_CAPITAL
                                #region ELEM_UL_FINISH
                                case ELEM_UL_FINISH:
                                    AddMappingElement("REGNUMFINISH", reader.GetAttribute(ATTR_REGNUM), ulMapping);
                                    AddMappingElement("DATEFINISHUL", reader.GetAttribute(ATTR_DTREG), ulMapping);
                                    isFinishUL = true;
                                    break;
                                #endregion ELEM_UL_FINISH
                                #region ELEM_UL_NAME
                                case ELEM_UL_NAME:
                                    string namep = reader.GetAttribute(ATTR_NAMEP);
                                    if (string.IsNullOrEmpty(namep))
                                        namep = constDefaultClsName;
                                    DateTime dateName = Convert.ToDateTime(reader.GetAttribute(ATTR_DTSTART));
                                    AddMappingElement("NAMEP", namep, ulMapping);
                                    AddMappingElement("DATENAME", dateName, ulMapping);
                                    AddMappingElement("SHORTNAME", reader.GetAttribute(ATTR_NAMES), ulMapping);
                                    AddMappingElement("FIRMNAME", reader.GetAttribute(ATTR_NAMEF), ulMapping);
                                    break;
                                #endregion ELEM_UL_NAME
                                #region ELEM_UL_OB
                                case ELEM_UL_OB:
                                    toUlOb = true;
                                    string vidOb = reader.GetAttribute(ATTR_VID_OB);
                                    if (string.IsNullOrEmpty(vidOb))
                                        vidOb = "0";
                                    int refOrgSubdiv = PumpCachedRow(cacheOrgKindSubdivision, dsOrgKindSubdivision.Tables[0], clsOrgKindSubdivision, vidOb,
                                        new object[] { "CODE", vidOb, "SOURCEID", clsOrgKindSubdivisionVersion });
                                    DateTime dateZap = Convert.ToDateTime(reader.GetAttribute(ATTR_DTSTART));
                                    AddMappingElement("REFORGSUBDIV", refOrgSubdiv, subdivisionMapping);
                                    AddMappingElement("DATEZAP", dateZap, subdivisionMapping);
                                    break;
                                #endregion ELEM_UL_OB
                                #region ELEM_UL_START
                                case ELEM_UL_START:
                                    AddMappingElement("REGNUM", reader.GetAttribute(ATTR_REGNUM), ulMapping);
                                    AddMappingElement("DATESTARTUL", reader.GetAttribute(ATTR_DTREG), ulMapping);
                                    isStartUL = true;
                                    break;
                                #endregion ELEM_UL_START
                                #region ELEM_UL_UPR
                                case ELEM_UL_UPR:
                                    toUlUpr = true;
                                    string nameDirect = reader.GetAttribute(ATTR_NAMEP);
                                    if (string.IsNullOrEmpty(nameDirect))
                                        nameDirect = constDefaultClsName;
                                    inn = reader.GetAttribute(ATTR_INN);
                                    if (string.IsNullOrEmpty(inn))
                                        inn = "0";
                                    inn20 = reader.GetAttribute(ATTR_KPP);
                                    if (string.IsNullOrEmpty(inn20))
                                        inn20 = "0";
                                    DateTime dateDirect = Convert.ToDateTime(reader.GetAttribute(ATTR_DTSTART));
                                    AddMappingElement("OGRNDIRECT", reader.GetAttribute(ATTR_OGRN), ulMapping);
                                    AddMappingElement("NAMEDIRECT", nameDirect, ulMapping);
                                    AddMappingElement("DATEOGRNDIRECT", reader.GetAttribute(ATTR_DTREG), ulMapping);
                                    AddMappingElement("INNDIRECT", inn, ulMapping);
                                    AddMappingElement("INN20DIRECT", inn20, ulMapping);
                                    AddMappingElement("DATEDIRECT", dateDirect, ulMapping);
                                    break;
                                #endregion ELEM_UL_UPR
                                #region ELEM_VIDADR
                                case ELEM_VIDADR:
                                    AddMappingElement("VIDADR", reader.GetAttribute(ATTR_NAME), ulMapping);
                                    break;
                                #endregion ELEM_VIDADR
                                #region ELEM_VIDCAP
                                case ELEM_VIDCAP:
                                    string capName = reader.GetAttribute(ATTR_NAME);
                                    string capCode = reader.GetAttribute(ATTR_ID);
                                    if (string.IsNullOrEmpty(capCode))
                                        capCode = "0";
                                    if (string.IsNullOrEmpty(capName))
                                        capName = constDefaultClsName;

                                    int refOrgKindCapital = PumpCachedRow(cacheOrgKindCapital, dsOrgKindCapital.Tables[0], clsOrgKindCapital, capName,
                                        new object[] { "CODE", capCode, "NAME", capName, "SOURCEID", clsOrgKindCapitalVersion });
                                    AddMappingElement("REFORGKINDCAPITAL", refOrgKindCapital, ulMapping);
                                    break;
                                #endregion ELEM_VIDCAP
                                #region ELEM_VIDDOC
                                case ELEM_VIDDOC:
                                    string vidDocID = reader.GetAttribute(ATTR_ID);
                                    if (string.IsNullOrEmpty(vidDocID))
                                        vidDocID = "0";
                                    string vidDocName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(vidDocName))
                                        vidDocName = constDefaultClsName;
                                    int refDocKindPerson = PumpCachedRow(cacheDocKindPerson, dsDocKindPerson.Tables[0], clsDocKindPerson, vidDocID,
                                        new object[] { "CODE", vidDocID, "NAME", vidDocName, "SOURCEID", clsDocKindPersonVersion });

                                    if (toDOLGNFL)
                                        AddMappingElement("REFDOCKINDPERSON", refDocKindPerson, faceProxyMapping);
                                    else
                                        AddMappingElement("REFDOCKINDPERS", refDocKindPerson, founderFLMapping);
                                    break;
                                #endregion ELEM_VIDDOC
                                #region ELEM_VIDLIC
                                case ELEM_VIDLIC:
                                    string licenzCode = reader.GetAttribute(ATTR_ID);
                                    if (!ValidateLong(licenzCode))
                                        licenzCode = "0";
                                    string licenzName = reader.GetAttribute(ATTR_NAME_VLIC);
                                    int refLicenz = PumpCachedRow(cacheTypesLicenz, dsTypesLicenz.Tables[0], clsTypesLicenz, licenzCode,
                                        new object[] { "CODE", licenzCode, "NAME", licenzName, "SOURCEID", clsTypesLicenzVersion });
                                    AddMappingElement("REFTYPE", refLicenz, licenceMapping);
                                    break;
                                #endregion ELEM_VIDLIC
                                #region ELEM_VIDREG
                                case ELEM_VIDREG:
                                    string vidregName = reader.GetAttribute(ATTR_NAME);
                                    string vidregID = reader.GetAttribute(ATTR_ID);

                                    if (string.IsNullOrEmpty(vidregID))
                                        vidregID = "0";

                                    if (string.IsNullOrEmpty(vidregName))
                                        vidregName = constDefaultClsName;

                                    int refTypeReg = PumpCachedRow(cacheTypesRegister, dsTypesRegister.Tables[0], clsTypesRegister, vidregID,
                                        new object[] { "CODE", vidregID, "NAME", vidregName, "SOURCEID", clsTypesRegisterVersion });
                                    if (isStartUL)
                                    {
                                        AddMappingElement("REFTYPEREGSTART", refTypeReg, ulMapping);
                                    }
                                    else if (toErgul)
                                    {
                                        AddMappingElement("REFTYPEREG", refTypeReg, noteMapping);
                                    }
                                    else
                                    {
                                        AddMappingElement("REFTYPEREGFINISH", refTypeReg, ulMapping);
                                    }
                                    break;
                                #endregion ELEM_VIDREG
                            }
                            break;
                        #endregion XmlNodeType.Element

                        #region XmlNodeType.EndElement
                        case XmlNodeType.EndElement:
                            switch (reader.LocalName)
                            {
                                #region ELEM_UL
                                case ELEM_UL:
                                    if (!toSkipUl)
                                    {
                                        // запоминаем старый ID организации, если он есть
                                        DataRow newRow = MappingToRow(ulMapping, dsEGRUL);
                                        string key = GetComplexCacheKey(newRow, new string[] { "OGRN", "INN", "INN20" }, "|");
                                        int refOldOrg = -1;
                                        if (cacheEGRUL.ContainsKey(key))
                                        {
                                            DataRow[] oldRows = GetLastRows(cacheEGRUL[key]);
                                            if (oldRows != null)
                                                refOldOrg = Convert.ToInt32(oldRows[0]["ID"]);
                                        }

                                        // закачиваем организацию и получаем новый ID (может совпадать со старым)
                                        int refNewOrg = PumpDataRow(newRow, clsEGRUL, dsEGRUL, cacheEGRUL, key);

                                        // закачиваем детали
                                        foreach (Detail detail in ulDetails.Values)
                                        {
                                            PumpDetailRows(detail, refNewOrg, refOldOrg, "RefOrgEGRUL");
                                        }
                                    }
                                    toSkipUl = false;
                                    ClearDetails(ulDetails);
                                    break;
                                #endregion ELEM_UL

                                #region ELEM_ACCOUNT
                                case ELEM_ACCOUNT:
                                    if (!toSkipAccounts)
                                    {
                                        ulDetails["Accounts"].AddNewRow(accountsMapping);
                                    }
                                    toSkipAccounts = false;
                                    accountsMapping = new Dictionary<string, object>();
                                    break;
                                #endregion ELEM_ACCOUNT
                                #region ELEM_DOLGNFL
                                case ELEM_DOLGNFL:
                                    if (!toSkipFaceProxy)
                                    {
                                        if (!faceProxyMapping.ContainsKey("FIO"))
                                            faceProxyMapping.Add("FIO", constDefaultClsName);
                                        if ((faceProxyMapping["FIO"] == DBNull.Value) || (string.IsNullOrEmpty(faceProxyMapping["FIO"].ToString())))
                                            faceProxyMapping["FIO"] = constDefaultClsName;
                                        ulDetails["FaceProxy"].AddNewRow(faceProxyMapping);
                                    }
                                    toDOLGNFL = false;
                                    toSkipFaceProxy = false;
                                    faceProxyMapping = new Dictionary<string, object>();
                                    break;
                                #endregion ELEM_DOLGNFL
                                #region ELEM_IUL
                                case ELEM_IUL:
                                    ulDetails["FounderForeignOrg"].AddNewRow(founderForeignOrgMapping);
                                    founderForeignOrgMapping = new Dictionary<string, object>();
                                    //toIUl = false;
                                    break;
                                #endregion ELEM_IUL
                                #region ELEM_LICENZ
                                case ELEM_LICENZ:
                                    ulDetails["Licence"].AddNewRow(licenceMapping);
                                    licenceMapping = new Dictionary<string, object>();
                                    break;
                                #endregion ELEM_LICENZ
                                #region ELEM_PREDSH
                                case ELEM_PREDSH:
                                    if (!toSkipAncestor)
                                    {
                                        AddMappingElement("REFADRESS", PumpOrgAdress(adress), ancestorMapping);
                                        AddMappingElement("ADRESS", adress.GetAdress(), ancestorMapping);
                                        ulDetails["Ancestor"].AddNewRow(ancestorMapping);
                                    }
                                    toPredsh = false;
                                    toSkipAncestor = false;
                                    ancestorMapping = new Dictionary<string, object>();
                                    break;
                                #endregion ELEM_PREDSH
                                #region ELEM_PREEM
                                case ELEM_PREEM:
                                    if (!toSkipAssign)
                                    {
                                        AddMappingElement("REFADRESS", PumpOrgAdress(adress), assignMapping);
                                        AddMappingElement("ADRESS", adress.GetAdress(), assignMapping);
                                        ulDetails["Assign"].AddNewRow(assignMapping);
                                    }
                                    toPreem = false;
                                    toSkipAssign = false;
                                    assignMapping = new Dictionary<string, object>();
                                    break;
                                #endregion ELEM_PREEM
                                #region ELEM_REGEGRUL
                                case ELEM_REGEGRUL:
                                    ulDetails["Note"].AddNewRow(noteMapping);
                                    noteMapping = new Dictionary<string, object>();
                                    toErgul = false;
                                    break;
                                #endregion ELEM_REGEGRUL
                                #region ELEM_RUL
                                case ELEM_RUL:
                                    if (!toSkipFounderOrg)
                                    {
                                        AddMappingElement("REFADRESS", PumpOrgAdress(adress), founderOrgMapping);
                                        AddMappingElement("ADRESS", adress.GetAdress(), founderOrgMapping);
                                        ulDetails["FounderOrg"].AddNewRow(founderOrgMapping);
                                    }
                                    toRul = false;
                                    toSkipFounderOrg = false;
                                    founderOrgMapping = new Dictionary<string, object>();
                                    break;
                                #endregion ELEM_RUL
                                #region ELEM_UCHRFL
                                case ELEM_UCHRFL:
                                    if (!toSkipFounderFL)
                                    {
                                        ulDetails["FounderFL"].AddNewRow(founderFLMapping);
                                    }
                                    toSkipFounderFL = false;
                                    founderFLMapping = new Dictionary<string, object>();
                                    break;
                                #endregion ELEM_UCHRFL
                                #region ELEM_UL_ADDRESS
                                case ELEM_UL_ADDRESS:
                                    toUlAddr = false;
                                    AddMappingElement("REFADRESS", PumpOrgAdress(adress), ulMapping);
                                    AddMappingElement("ADRESS", adress.GetAdress(), ulMapping);
                                    break;
                                #endregion ELEM_UL_ADDRESS
                                #region ELEM_UL_FINISH
                                case ELEM_UL_FINISH:
                                    isFinishUL = false;
                                    break;
                                #endregion ELEM_UL_FINISH
                                #region ELEM_UL_OB
                                case ELEM_UL_OB:
                                    AddMappingElement("REFADRESS", PumpOrgAdress(adress), subdivisionMapping);
                                    AddMappingElement("ADRESS", adress.GetAdress(), subdivisionMapping);
                                    ulDetails["Subdivision"].AddNewRow(subdivisionMapping);
                                    subdivisionMapping = new Dictionary<string, object>();
                                    toUlOb = false;
                                    break;
                                #endregion ELEM_UL_OB
                                #region ELEM_UL_START
                                case ELEM_UL_START:
                                    isStartUL = false;
                                    break;
                                #endregion ELEM_UL_START
                                #region ELEM_UL_UPR
                                case ELEM_UL_UPR:
                                    toUlUpr = false;
                                    AddMappingElement("REFADRESSDIRECT", PumpOrgAdress(adress), ulMapping);
                                    AddMappingElement("ADRESSDIRECT", adress.GetAdress(), ulMapping);
                                    break;
                                #endregion ELEM_UL_UPR
                            }
                            break;
                        #endregion XmlNodeType.EndElement
                    }
            }
            finally
            {
                oksmAbsentCodes.Clear();
                reader.Close();
            }
        }

        #endregion Закачка юридических лиц

        #region Закачка индивидуальных предпринимателей

        private Dictionary<string, Detail> InitIPDetails()
        {
            Dictionary<string, Detail> details = new Dictionary<string, Detail>();

            details.Add("IPLicence", new Detail(clsIPLicenceDet, dsIPLicenceDet,
                cacheIPLicenceDet, new string[] { "Nomer", "RefIPEGRIP" }));
            details.Add("IPNote", new Detail(clsIPNoteDet, dsIPNoteDet,
                cacheIPNoteDet, new string[] { "NumberReg", "RefIPEGRIP" }));
            details.Add("IPOkved", new Detail(clsIPOkvedDet, dsIPOkvedDet,
                cacheIPOkvedDet, new string[] { "Code", "RefIPEGRIP" }));
            details.Add("IPAccounts", new Detail(clsIPAccountsDet, dsIPAccountsDet,
                cacheIPAccountsDet, new string[] { "NumAcc", "RefIPEGRIP" }));

            return details;
        }

        private void DeleteIPData()
        {
            DeleteTableData(clsIPLicenceDet, -1, -1, string.Empty);
            DeleteTableData(clsIPNoteDet, -1, -1, string.Empty);
            DeleteTableData(clsIPOkvedDet, -1, -1, string.Empty);
            DeleteTableData(clsIPAccountsDet, -1, -1, string.Empty);

            IClassifier[] usedClassifiers = new IClassifier[] {
                clsEGRIP, clsIPKindFunction, clsIPStatus, clsOKCitizen };
            DirectDeleteClsData(usedClassifiers, -1, -1, string.Empty);
        }

        private void PumpIPReport(FileInfo ipReport)
        {
            if (dsOksmBridge.Tables[0].Rows.Count <= 1)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    "Не заполнен классификатор 'ОКСМ.Сопоставимый'. Заполните этот классификатор и запустите закачку еще раз.");
                throw new Exception("Не заполнен классификатор 'ОКСМ.Сопоставимый'");
            }

            if ((this.DeleteEarlierData) && (!isDeleted))
            {
                updateMode = false;
                DeleteIPData();
                QueryData();
                isDeleted = true;
            }

            XmlTextReader reader = GetXMLReader(ipReport.FullName);
            pumpedFilename = ipReport.Name;

            bool toRegOld = false;
            bool toNote = false;

            // признаки для пропуска записей соответствующих классификаторов
            // в случае обнаружения некорректных ИНН
            bool toSkipIp = false;
            bool toSkipAccounts = false;

            // основные данные и детали
            Dictionary<string, object> ipMapping = new Dictionary<string, object>();
            Dictionary<string, Detail> ipDetails = InitIPDetails();
            Dictionary<string, object> ipLicenceMapping = new Dictionary<string, object>();
            Dictionary<string, object> ipNoteMapping = new Dictionary<string, object>();
            Dictionary<string, object> ipOkvedMapping = new Dictionary<string, object>();
            Dictionary<string, object> ipAccountsMapping = new Dictionary<string, object>();

            // устанавливаем всем имеющимся записям статус "Не изменилась"
            SetOldStatus(dsEGRIP);
            foreach (Detail detail in ipDetails.Values)
            {
                SetOldStatus(detail.dataSet);
            }

            // параметры записи индивидуальных предпренимателей
            IPParams ipParams = new IPParams();
            IPLicenzDetailParams ipLicenzDetailParams = new IPLicenzDetailParams();
            IPChangesDetail ipChangesDetail = new IPChangesDetail();
            OldRegParams oldRegParams = new OldRegParams();
            Adress adress = new Adress();
            Contact contact = new Contact();
            FIO fio = new FIO();

            try
            {
                while (reader.Read())
                    switch (reader.NodeType)
                    {
                        #region XmlNodeType.Element
                        case XmlNodeType.Element:
                            switch (reader.LocalName)
                            {
                                #region ELEM_IP
                                case ELEM_IP:
                                    // очищаем временные коллекции и переменные
                                    ipMapping.Clear();
                                    currentOgrn = reader.GetAttribute(ATTR_OGRNIP);
                                    currentIDDoc = reader.GetAttribute(ATTR_IDDOK);
                                    string inn = reader.GetAttribute(ATTR_INN);
                                    if (!ValidateDecimal(currentOgrn))
                                    {
                                        WriteIncorrectOgrnIP(inn);
                                        currentOgrn = "0";
                                    }
                                    if (!ValidateLong(inn) && !string.IsNullOrEmpty(inn))
                                    {
                                        WriteIncorrectInn(inn, clsEGRIP);
                                        toSkipIp = true;
                                    }
                                    ipParams.ClearParams();
                                    ipParams.OGRNIP = Convert.ToString(TryGetDecimalValue(currentOgrn));
                                    ipParams.INN = Convert.ToString(TryGetLongValue(inn));
                                    AddMappingElement("SOURCEID", clsEGRIPVersion, ipMapping);
                                    AddMappingElement("OGRNIP", ipParams.OGRNIP, ipMapping);
                                    AddMappingElement("INN", ipParams.INN, ipMapping);
                                    break;
                                #endregion ELEM_IP

                                #region ELEM_ACCOUNT
                                case ELEM_ACCOUNT:
                                    AddMappingElement("NUMACC", reader.GetAttribute(ATTR_NUM), ipAccountsMapping);
                                    AddMappingElement("OGRN", reader.GetAttribute(ATTR_OGRN), ipAccountsMapping);
                                    string innAccounts = reader.GetAttribute(ATTR_INN);
                                    if (!ValidateLong(innAccounts) && !string.IsNullOrEmpty(innAccounts))
                                    {
                                        WriteIncorrectInn(innAccounts, clsIPAccountsDet);
                                        toSkipAccounts = true;
                                    }
                                    AddMappingElement("INN", TryGetLongValue(innAccounts), ipAccountsMapping);
                                    AddMappingElement("INN20", TryGetLongValue(reader.GetAttribute(ATTR_KPP)), ipAccountsMapping);
                                    AddMappingElement("BIK", reader.GetAttribute(ATTR_BIK), ipAccountsMapping);
                                    AddMappingElement("NUMCONTRACT", reader.GetAttribute(ATTR_NUM_CONTRACT), ipAccountsMapping);
                                    AddMappingElement("DATESTARTCONTR", reader.GetAttribute(ATTR_DTSTART), ipAccountsMapping);
                                    AddMappingElement("DATEENDCONTR", reader.GetAttribute(ATTR_DTEND), ipAccountsMapping);

                                    string id_VA = reader.GetAttribute(ATTR_ID_VA);
                                    if (string.IsNullOrEmpty(id_VA))
                                        id_VA = "0";
                                    string name_VA = reader.GetAttribute(ATTR_NAME_VA);
                                    if (string.IsNullOrEmpty(name_VA))
                                        name_VA = constDefaultClsName;
                                    int refTypeAcc = PumpCachedRow(cacheTypesBankAcc, dsTypesBankAcc.Tables[0], clsTypesBankAcc, id_VA,
                                        new object[] { "CODE", id_VA, "NAME", name_VA, "SOURCEID", clsTypesBankAccVersion });
                                    AddMappingElement("REFTYPEACC", refTypeAcc, ipAccountsMapping);

                                    string ID_TA = reader.GetAttribute(ATTR_ID_TA);
                                    if (string.IsNullOrEmpty(ID_TA))
                                        ID_TA = "0";
                                    string name_TA = reader.GetAttribute(ATTR_NAME_TA);
                                    if (string.IsNullOrEmpty(name_TA))
                                        name_TA = constDefaultClsName;
                                    int refKindAcc = PumpCachedRow(cacheKindBankAcc, dsKindBankAcc.Tables[0], clsKindBankAcc, ID_TA,
                                        new object[] { "CODE", ID_TA, "NAME", name_TA, "SOURCEID", clsKindBankAccVersion });
                                    AddMappingElement("REFKINDACC", refKindAcc, ipAccountsMapping);
                                    break;
                                #endregion ELEM_ACCOUNT
                                #region ELEM_ADDRESS
                                case ELEM_ADDRESS:
                                    string okato = reader.GetAttribute(ATTR_OKATO);
                                    if (string.IsNullOrEmpty(okato))
                                        okato = "0";
                                    AddMappingElement("OKATO", okato, ipMapping);

                                    adress.index = reader.GetAttribute(ATTR_INDEKS);
                                    adress.dom = reader.GetAttribute(ATTR_DOM);
                                    adress.korp = reader.GetAttribute(ATTR_KORP);
                                    adress.kvart = reader.GetAttribute(ATTR_KVART);
                                    break;
                                #endregion ELEM_ADDRESS
                                #region ELEM_CITIZEN
                                case ELEM_CITIZEN:
                                    AddMappingElement("DTSTARTCITIZEN", reader.GetAttribute(ATTR_DTSTART), ipMapping);
                                    break;
                                #endregion ELEM_CITIZEN
                                #region ELEM_CONTACT
                                case ELEM_CONTACT:
                                    contact.kodGorod = reader.GetAttribute(ATTR_KODGOROD);
                                    contact.telefon = reader.GetAttribute(ATTR_TELEFON);
                                    contact.fax = reader.GetAttribute(ATTR_FAX);
                                    AddMappingElement("CONTACT", contact.GetContact(), ipMapping);
                                    break;
                                #endregion ELEM_CONTACT
                                #region ELEM_DOCDSN
                                case ELEM_DOCDSN:
                                    AddMappingElement("ABILITYDOCNUM", reader.GetAttribute(ATTR_NUM), ipMapping);
                                    AddMappingElement("ABILITYDOCDATE", reader.GetAttribute(ATTR_DT), ipMapping);
                                    AddMappingElement("ABILITYNAMEORG", reader.GetAttribute(ATTR_NAMEORG), ipMapping);
                                    AddMappingElement("DTSTARTABILITY", reader.GetAttribute(ATTR_DTSTART), ipMapping);
                                    break;
                                #endregion ELEM_DOCDSN
                                #region ELEM_DOCFL
                                case ELEM_DOCFL:
                                    AddMappingElement("DATEDOC", reader.GetAttribute(ATTR_DT), ipMapping);
                                    string ser = reader.GetAttribute(ATTR_SER);
                                    string num = reader.GetAttribute(ATTR_NUM);
                                    string docNum = string.Empty;
                                    if (!string.IsNullOrEmpty(ser))
                                        docNum += string.Format("Серия: {0}", ser);
                                    if (!string.IsNullOrEmpty(num))
                                        docNum += string.Format("Номер: {0}", num);
                                    AddMappingElement("DOCNUM", docNum, ipMapping);
                                    AddMappingElement("NAMEORG", reader.GetAttribute(ATTR_NAMEORG), ipMapping);
                                    AddMappingElement("KODORG", reader.GetAttribute(ATTR_KODORG), ipMapping);
                                    AddMappingElement("DTSTARTDOCFL", reader.GetAttribute(ATTR_DTSTART), ipMapping);
                                    break;
                                #endregion ELEM_DOCFL
                                #region ELEM_DOCREZ
                                case ELEM_DOCREZ:
                                    AddMappingElement("HABITATDOCDATEEND", reader.GetAttribute(ATTR_DTENDDOK), ipMapping);
                                    AddMappingElement("HABITATNAMEORG", reader.GetAttribute(ATTR_NAMEORG), ipMapping);
                                    AddMappingElement("HABITATDOCDATE", reader.GetAttribute(ATTR_DT), ipMapping);
                                    AddMappingElement("HABITATDOCNUM", reader.GetAttribute(ATTR_NUM), ipMapping);
                                    AddMappingElement("DTSTARTDOCHABITAT", reader.GetAttribute(ATTR_DTSTART), ipMapping);
                                    break;
                                #endregion ELEM_DOCREZ
                                #region ELEM_DOKDN
                                case ELEM_DOKDN:
                                    string docdnID = reader.GetAttribute(ATTR_ID);
                                    string docdnName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(docdnID))
                                        docdnID = "0";
                                    if (string.IsNullOrEmpty(docdnName))
                                        docdnName = constDefaultClsName;
                                    int refDocRightAbility = PumpCachedRow(cacheDocRightAbility, dsDocRightAbility.Tables[0], clsDocRightAbility, docdnID,
                                        new object[] { "CODE", docdnID, "NAME", docdnName, "SOURCEID", clsDocRightAbilityVersion });
                                    AddMappingElement("REFDOCRIGHTABILITY", refDocRightAbility, ipMapping);
                                    break;
                                #endregion ELEM_DOKDN
                                #region ELEM_FL
                                case ELEM_FL:
                                    fio.Name = reader.GetAttribute(ATTR_NAME_FL);
                                    fio.Fam = reader.GetAttribute(ATTR_FAM_FL);
                                    fio.Otch = reader.GetAttribute(ATTR_OTCH_FL);
                                    AddMappingElement("FIO", fio.GetFIO(), ipMapping);
                                    fio.ClearFIO();

                                    fio.Name = reader.GetAttribute(ATTR_NAMELAT);
                                    fio.Fam = reader.GetAttribute(ATTR_FAMLAT);
                                    fio.Otch = reader.GetAttribute(ATTR_OTCHLAT);
                                    AddMappingElement("FIOLAT", fio.GetFIO(), ipMapping);
                                    fio.ClearFIO();
                                    string sex = reader.GetAttribute(ATTR_SEX);
                                    if (sex == "1")
                                        AddMappingElement("REFSEX", 2, ipMapping);
                                    else
                                        AddMappingElement("REFSEX", 1, ipMapping);
                                    AddMappingElement("DTSTARTFL", reader.GetAttribute(ATTR_DTSTART), ipMapping);
                                    break;
                                #endregion ELEM_FL
                                #region ELEM_FL_ADDR
                                case ELEM_FL_ADDR:
                                    AddMappingElement("DTSTARTADDR", reader.GetAttribute(ATTR_DTSTART), ipMapping);
                                    break;
                                #endregion ELEM_FL_ADDR
                                #region ELEM_FOMS
                                case ELEM_FOMS:
                                    AddMappingElement("REGNUMFOMS", reader.GetAttribute(ATTR_REGN_FOMS), ipMapping);
                                    AddMappingElement("DATESTARTFOMS", reader.GetAttribute(ATTR_DTSTART), ipMapping);
                                    AddMappingElement("DATEFINISHFOMS", reader.GetAttribute(ATTR_DTEND), ipMapping);
                                    break;
                                #endregion ELEM_FOMS
                                #region ELEM_FSS
                                case ELEM_FSS:
                                    AddMappingElement("REGNUMFSS", reader.GetAttribute(ATTR_REGN_FSS), ipMapping);
                                    AddMappingElement("DATESTARTFSS", reader.GetAttribute(ATTR_DTSTART), ipMapping);
                                    AddMappingElement("DATEFINISHFSS", reader.GetAttribute(ATTR_DTEND), ipMapping);
                                    break;
                                #endregion ELEM_FSS
                                #region ELEM_GOROD
                                case ELEM_GOROD:
                                    adress.gorod = reader.GetAttribute(ATTR_NAME);
                                    adress.gorodId = reader.GetAttribute(ATTR_ID);
                                    adress.gorodCode = reader.GetAttribute(ATTR_KOD_KL);
                                    break;
                                #endregion ELEM_GOROD
                                #region ELEM_LICENZ
                                case ELEM_LICENZ:
                                    AddMappingElement("NOMER", reader.GetAttribute(ATTR_NUMLIC), ipLicenceMapping);
                                    AddMappingElement("DATELICENCE", reader.GetAttribute(ATTR_DTRESH), ipLicenceMapping);
                                    AddMappingElement("DATESTARTLICENCE", reader.GetAttribute(ATTR_DTSTART), ipLicenceMapping);
                                    AddMappingElement("DATEFINISHLICENCE", reader.GetAttribute(ATTR_DTEND), ipLicenceMapping);
                                    AddMappingElement("DATESTOPLIC", reader.GetAttribute(ATTR_DTSTOP), ipLicenceMapping);
                                    AddMappingElement("DATERESUME", reader.GetAttribute(ATTR_DTSTARTNOV), ipLicenceMapping);
                                    break;
                                #endregion ELEM_LICENZ
                                #region ELEM_LICORG
                                case ELEM_LICORG:
                                    string licOrgID = reader.GetAttribute(ATTR_ID);
                                    string licOrgName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(licOrgName))
                                        licOrgName = constDefaultClsName;
                                    if (string.IsNullOrEmpty(licOrgID))
                                        licOrgID = "0";

                                    int refOrgLic = PumpCachedRow(cacheOrgLicenceOrgan, dsOrgLicenceOrgan.Tables[0], clsOrgLicenceOrgan, licOrgID,
                                        new object[] { "CODE", licOrgID, "NAME", licOrgName, "SOURCEID", clsOrgLicenceOrganVersion });
                                    AddMappingElement("REFORGLIC", refOrgLic, ipLicenceMapping);
                                    break;
                                #endregion ELEM_LICORG
                                #region ELEM_MNS
                                case ELEM_MNS:
                                    AddMappingElement("DATESTARTMNS", reader.GetAttribute(ATTR_DTSTART), ipMapping);
                                    AddMappingElement("DATEFINISHMNS", reader.GetAttribute(ATTR_DTEND), ipMapping);
                                    break;
                                #endregion ELEM_MNS
                                #region ELEM_NASPUNKT
                                case ELEM_NASPUNKT:
                                    adress.nasPunkt = reader.GetAttribute(ATTR_NAME);
                                    adress.nasPunktId = reader.GetAttribute(ATTR_ID);
                                    adress.nasPunktCode = reader.GetAttribute(ATTR_KOD_KL);
                                    break;
                                #endregion ELEM_NASPUNKT
                                #region ELEM_OKSM
                                case ELEM_OKSM:
                                    // страна
                                    ipParams.refOKSM = reader.GetAttribute(ATTR_ID);
                                    int refOKSM = FindCachedRow(cacheOksmBridge, ipParams.refOKSM, -1);
                                    if (refOKSM == -1)
                                    {
                                        if (!oksmAbsentCodes.Contains(ipParams.refOKSM))
                                            oksmAbsentCodes.Add(ipParams.refOKSM);
                                        //WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                                        //    string.Format("Запись с кодом {0} не обнаружена в классификаторе 'ОКСМ.Cопоставимый'", ipParams.refOKSM));
                                    }
                                    AddMappingElement("REFOKSMBRIDGE", refOKSM, ipMapping);
                                    break;
                                #endregion ELEM_OKSM
                                #region ELEM_OKVED
                                case ELEM_OKVED:
                                    bool isMainOKVED = reader.GetAttribute(ATTR_MAIN) == "1";
                                    if (isMainOKVED)
                                    {
                                        string mainOkved = reader.GetAttribute(ATTR_KOD_OKVED);
                                        FormatMainOKVED(ref mainOkved);
                                        ipParams.mainOKVED = mainOkved;
                                        AddMappingElement("MAINOKVED", ipParams.mainOKVED, ipMapping);
                                    }

                                    string kodOkved = reader.GetAttribute(ATTR_KOD_OKVED);
                                    if (string.IsNullOrEmpty(kodOkved))
                                        kodOkved = "0";
                                    FormatMainOKVED(ref kodOkved);
                                    AddMappingElement("CODE", kodOkved, ipOkvedMapping);
                                    string nameOkved = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(nameOkved.Trim()))
                                        nameOkved = constDefaultClsName;
                                    AddMappingElement("NAME", nameOkved, ipOkvedMapping);
                                    if (!isMainOKVED)
                                    {
                                        AddMappingElement("SIGN", "Дополнительный вид экономической деятельности", ipOkvedMapping);
                                    }
                                    else
                                    {
                                        AddMappingElement("SIGN", "Основной вид экономической деятельности", ipOkvedMapping);
                                    }
                                    ipDetails["IPOkved"].AddNewRow(ipOkvedMapping);
                                    ipOkvedMapping = new Dictionary<string, object>();
                                    break;
                                #endregion ELEM_OKVED
                                #region ELEM_ORGAN_FOMS
                                case ELEM_ORGAN_FOMS:
                                    string fomsKod = reader.GetAttribute(ATTR_KOD);
                                    if (string.IsNullOrEmpty(fomsKod))
                                        fomsKod = "0";
                                    int fomsCode = 0;
                                    Int32.TryParse(fomsKod, out fomsCode);

                                    string fomsName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(fomsName))
                                        fomsName = constDefaultClsName;

                                    int refOrgFOMS = PumpCachedRow(cacheOrgOrganFOMS, dsOrgOrganFOMS.Tables[0], clsOrgOrganFOMS, fomsCode.ToString(),
                                        new object[] { "CODE", fomsCode, "NAME", fomsName, "SOURCEID", clsOrgOrganFOMSVersion });
                                    AddMappingElement("REFORGFOMS", refOrgFOMS, ipMapping);
                                    break;
                                #endregion ELEM_ORGAN_FOMS
                                #region ELEM_ORGAN_FSS
                                case ELEM_ORGAN_FSS:
                                    string fssKod = reader.GetAttribute(ATTR_KOD);
                                    if (string.IsNullOrEmpty(fssKod))
                                        fssKod = "0";
                                    int fssCode = 0;
                                    Int32.TryParse(fssKod, out fssCode);
                                    string fssName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(fssName))
                                        fssName = constDefaultClsName;

                                    int refOrgFSS = PumpCachedRow(cacheOrgOrganFSS, dsOrgOrganFSS.Tables[0], clsOrgOrganFSS, fssCode.ToString(),
                                        new object[] { "CODE", fssCode, "NAME", fssName, "SOURCEID", clsOrgOrganFSSVersion });
                                    AddMappingElement("REFORGFSS", refOrgFSS, ipMapping);
                                    break;
                                #endregion ELEM_ORGAN_FSS
                                #region ELEM_ORGAN_MNS
                                case ELEM_ORGAN_MNS:
                                    string mnsKod = reader.GetAttribute(ATTR_KOD);
                                    if (string.IsNullOrEmpty(mnsKod))
                                        mnsKod = "0";
                                    int mnsCode = 0;
                                    Int32.TryParse(mnsKod, out mnsCode);
                                    string mnsName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(mnsName))
                                        mnsName = constDefaultClsName;
                                    int refOrgMNS = PumpCachedRow(cacheOrgMNS, dsOrgMNS.Tables[0], clsOrgMNS, mnsCode.ToString(),
                                        new object[] { "CODE", mnsCode, "NAME", mnsName, "SOURCEID", clsOrgMNSVersion });
                                    AddMappingElement("REFORGMNS", refOrgMNS, ipMapping);
                                    break;
                                #endregion ELEM_ORGAN_MNS
                                #region ELEM_ORGAN_PF
                                case ELEM_ORGAN_PF:
                                    string pfKod = reader.GetAttribute(ATTR_KOD);
                                    if (string.IsNullOrEmpty(pfKod))
                                        pfKod = "0";
                                    int pfCode = 0;
                                    Int32.TryParse(pfKod, out pfCode);
                                    string pfName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(pfName))
                                        pfName = constDefaultClsName;

                                    int refOrgPF = PumpCachedRow(cacheOrgOrganPF, dsOrgOrganPF.Tables[0], clsOrgOrganPF, pfCode.ToString(),
                                        new object[] { "CODE", pfCode, "NAME", pfName, "SOURCEID", clsOrgOrganPFVersion });
                                    AddMappingElement("REFORGPF", refOrgPF, ipMapping);
                                    break;
                                #endregion ELEM_ORGAN_PF
                                #region ELEM_OSNDN
                                case ELEM_OSNDN:
                                    string osndnID = reader.GetAttribute(ATTR_ID);
                                    string osndnName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(osndnID))
                                        osndnID = "0";
                                    if (string.IsNullOrEmpty(osndnName))
                                        osndnName = constDefaultClsName;
                                    int refDocBaseAbility = PumpCachedRow(cacheDocBaseAbility, dsDocBaseAbility.Tables[0], clsDocBaseAbility, osndnID,
                                        new object[] { "CODE", osndnID, "NAME", osndnName, "SOURCEID", clsDocBaseAbilityVersion });
                                    AddMappingElement("REFDOCBASEABILITY", refDocBaseAbility, ipMapping);
                                    break;
                                #endregion ELEM_OSNDN
                                #region ELEM_PF
                                case ELEM_PF:
                                    AddMappingElement("REGNUMPF", reader.GetAttribute(ATTR_REGN_PF), ipMapping);
                                    AddMappingElement("DATESTARTPF", reader.GetAttribute(ATTR_DTSTART), ipMapping);
                                    AddMappingElement("DATEFINISHPF", reader.GetAttribute(ATTR_DTEND), ipMapping);
                                    break;
                                #endregion ELEM_PF
                                #region ELEM_RAION
                                case ELEM_RAION:
                                    adress.raion = reader.GetAttribute(ATTR_NAME);
                                    adress.raionId = reader.GetAttribute(ATTR_ID);
                                    adress.raionCode = reader.GetAttribute(ATTR_KOD_KL);
                                    break;
                                #endregion ELEM_RAION
                                #region ELEM_REGEGRIP
                                case ELEM_REGEGRIP:
                                    toNote = true;
                                    AddMappingElement("IDREG", reader.GetAttribute(ATTR_IDREG), ipNoteMapping);
                                    AddMappingElement("NUMBERREG", reader.GetAttribute(ATTR_REGNUM), ipNoteMapping);
                                    AddMappingElement("DATEREG", reader.GetAttribute(ATTR_DTREG), ipNoteMapping);
                                    AddMappingElement("DATENOTE", reader.GetAttribute(ATTR_DTZAP), ipNoteMapping);
                                    break;
                                #endregion ELEM_REGEGRIP
                                #region ELEM_REGION
                                case ELEM_REGION:
                                    adress.region = reader.GetAttribute(ATTR_NAME);
                                    adress.regionId = reader.GetAttribute(ATTR_ID);
                                    adress.regionCode = reader.GetAttribute(ATTR_KOD_KL);
                                    break;
                                #endregion ELEM_REGION
                                #region ELEM_REGOLD
                                case ELEM_REGOLD:
                                    toRegOld = true;
                                    oldRegParams.regNum = reader.GetAttribute(ATTR_NUMOLD);
                                    oldRegParams.regData = reader.GetAttribute(ATTR_DTREG);
                                    break;
                                #endregion ELEM_REGOLD
                                #region ELEM_REGORG
                                case ELEM_REGORG:
                                    // налоговый орган
                                    string regorgID = reader.GetAttribute(ATTR_ID);
                                    string regorgName = reader.GetAttribute(ATTR_NAME);

                                    if (string.IsNullOrEmpty(regorgID))
                                        regorgID = "0";

                                    if (string.IsNullOrEmpty(regorgName))
                                        regorgName = constDefaultClsName;

                                    if (toRegOld)
                                    {
                                        oldRegParams.regOrg = regorgName;
                                    }
                                    else if (toNote)
                                    {
                                        int refOrgRO = PumpCachedRow(cacheOrgRegisterOrgan, dsOrgRegisterOrgan.Tables[0], clsOrgRegisterOrgan, regorgID,
                                            new object[] { "CODE", regorgID, "NAME", regorgName, "SOURCEID", clsOrgRegisterOrganVersion });
                                        AddMappingElement("REFORGRO", refOrgRO, ipNoteMapping);
                                    }
                                    else
                                    {
                                        int refOrgRO = PumpCachedRow(cacheOrgRegisterOrgan, dsOrgRegisterOrgan.Tables[0], clsOrgRegisterOrgan, regorgID,
                                            new object[] { "CODE", regorgID, "NAME", regorgName, "SOURCEID", clsOrgRegisterOrganVersion });
                                        AddMappingElement("REFORGRO", refOrgRO, ipMapping);
                                    }
                                    break;
                                #endregion ELEM_REGORG
                                #region ELEM_SOSTLIC
                                case ELEM_SOSTLIC:
                                    string sostlicID = reader.GetAttribute(ATTR_ID);
                                    string sostlicName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(sostlicID))
                                        sostlicID = "0";
                                    if (string.IsNullOrEmpty(sostlicName))
                                        sostlicName = constDefaultClsName;

                                    int refStateLicence = PumpCachedRow(cacheStateLicence, dsStateLicence.Tables[0], clsStateLicence, sostlicID,
                                        new object[] { "CODE", sostlicID, "NAME", sostlicName, "SOURCEID", clsStateLicenceVersion });
                                    AddMappingElement("REFSTATELICENCE", refStateLicence, ipLicenceMapping);
                                    break;
                                #endregion ELEM_SOSTLIC
                                #region ELEM_SOSTZAP
                                case ELEM_SOSTZAP:
                                    string soszapID = reader.GetAttribute(ATTR_ID);
                                    if (string.IsNullOrEmpty(soszapID))
                                        soszapID = "0";
                                    string soszapName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(soszapName))
                                        soszapName = constDefaultClsName;
                                    string keyStateRegister = string.Format("{0}|{1}", soszapID, soszapName);
                                    int refSosZap = PumpCachedRow(cacheStateRegisterNote, dsStateRegisterNote.Tables[0], clsStateRegisterNote, keyStateRegister,
                                        new object[] { "CODE", soszapID, "NAME", soszapName, "SOURCEID", clsStateRegisterNoteVersion });
                                    AddMappingElement("REFSTATEREG", refSosZap, ipNoteMapping);
                                    break;
                                #endregion ELEM_SOSTZAP
                                #region ELEM_STATUS
                                case ELEM_STATUS:
                                    // вид статуса
                                    string statusID = reader.GetAttribute(ATTR_ID);
                                    if (string.IsNullOrEmpty(statusID))
                                        statusID = "0";
                                    string statusName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(statusName))
                                        statusName = constDefaultClsName;

                                    string keyIPStatus = string.Format("{0}|{1}", statusID, statusName);
                                    int refIpStatus = PumpCachedRow(cacheIPStatus, dsIPStatus.Tables[0], clsIPStatus, keyIPStatus,
                                        new object[] { "CODE", statusID, "NAME", statusName, "SOURCEID", clsIPStatusVersion });
                                    AddMappingElement("REFIPSTATUS", refIpStatus, ipMapping);
                                    break;
                                #endregion ELEM_STATUS
                                #region ELEM_STREET
                                case ELEM_STREET:
                                    adress.street = reader.GetAttribute(ATTR_NAME);
                                    adress.streetId = reader.GetAttribute(ATTR_ID);
                                    adress.streetCode = reader.GetAttribute(ATTR_KOD_ST);
                                    break;
                                #endregion ELEM_STREET
                                #region ELEM_SVSV
                                case ELEM_SVSV:
                                    string SER_SV = reader.GetAttribute(ATTR_SER_SV);
                                    if (string.IsNullOrEmpty(SER_SV))
                                        SER_SV = string.Empty;

                                    string NUM_SV = reader.GetAttribute(ATTR_NUM_SV);
                                    if (string.IsNullOrEmpty(NUM_SV))
                                        NUM_SV = string.Empty;
                                    string svsv = string.Format("Серия {0} номер {1}",
                                        reader.GetAttribute(ATTR_SER_SV), reader.GetAttribute(ATTR_NUM_SV));
                                    AddMappingElement("CERTIFICATE", svsv, ipNoteMapping);
                                    break;
                                #endregion ELEM_SVSV
                                #region ELEM_VIDCITIZEN
                                case ELEM_VIDCITIZEN:
                                    // гражданство
                                    string vidCitizenID = reader.GetAttribute(ATTR_ID);
                                    string vidCitizenName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(vidCitizenName))
                                        vidCitizenName = constDefaultClsName;
                                    int refOkCitizen = PumpCachedRow(cacheOKCitizen, dsOKCitizen.Tables[0], clsOKCitizen, vidCitizenID,
                                        new object[] { "CODE", vidCitizenID, "NAME", vidCitizenName, "SOURCEID", clsOKCitizenVersion });
                                    AddMappingElement("REFOKCITIZEN", refOkCitizen, ipMapping);
                                    break;
                                #endregion ELEM_VIDCITIZEN
                                #region ELEM_VIDDOK
                                case ELEM_VIDDOK:
                                    string vidDocID = reader.GetAttribute(ATTR_ID);
                                    string vidDocName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(vidDocID))
                                        vidDocID = "0";
                                    if (string.IsNullOrEmpty(vidDocName))
                                        vidDocName = constDefaultClsName;
                                    int refDocKindPerson = PumpCachedRow(cacheDocKindPerson, dsDocKindPerson.Tables[0], clsDocKindPerson, vidDocID,
                                        new object[] { "CODE", vidDocID, "NAME", vidDocName, "SOURCEID", clsDocKindPersonVersion });
                                    AddMappingElement("REFDOCKINDPERSON", refDocKindPerson, ipMapping);
                                    break;
                                #endregion ELEM_VIDDOK
                                #region ELEM_VIDDOKREZ
                                case ELEM_VIDDOKREZ:
                                    string vidDocRezID = reader.GetAttribute(ATTR_ID);
                                    string vidDocRezName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(vidDocRezID))
                                        vidDocRezID = "0";
                                    if (string.IsNullOrEmpty(vidDocRezName))
                                        vidDocRezName = constDefaultClsName;
                                    int refDocHabitat = PumpCachedRow(cacheDocHabitatRF, dsDocHabitatRF.Tables[0], clsDocHabitatRF, vidDocRezID,
                                        new object[] { "CODE", vidDocRezID, "NAME", vidDocRezName, "SOURCEID", clsDocHabitatRFVersion });
                                    AddMappingElement("REFDOCHABITATRF", refDocHabitat, ipMapping);
                                    break;
                                #endregion ELEM_VIDDOKREZ
                                #region ELEM_VIDIP
                                case ELEM_VIDIP:
                                    // вид деятельности
                                    string vidIpID = reader.GetAttribute(ATTR_ID);
                                    string vidIpName = reader.GetAttribute(ATTR_NAME);
                                    if (string.IsNullOrEmpty(vidIpName))
                                        vidIpName = constDefaultClsName;

                                    int refIpKindFunction = PumpCachedRow(cacheIPKindFunction, dsIPKindFunction.Tables[0], clsIPKindFunction, vidIpID,
                                        new object[] { "CODE", vidIpID, "NAME", vidIpName, "SOURCEID", clsIPKindFunctionVersion });
                                    AddMappingElement("REFIPKINDFUNCTION", refIpKindFunction, ipMapping);
                                    break;
                                #endregion ELEM_VIDIP
                                #region ELEM_VIDLIC
                                case ELEM_VIDLIC:
                                    string licenzCode = reader.GetAttribute(ATTR_ID);
                                    if (!ValidateLong(licenzCode))
                                        licenzCode = "0";
                                    string licenzName = reader.GetAttribute(ATTR_NAME_VLIC);
                                    int refLicenz = PumpCachedRow(cacheTypesLicenz, dsTypesLicenz.Tables[0], clsTypesLicenz, licenzCode,
                                        new object[] { "CODE", licenzCode, "NAME", licenzName, "SOURCEID", clsTypesLicenzVersion });
                                    AddMappingElement("REFTYPE", refLicenz, ipLicenceMapping);
                                    break;
                                #endregion ELEM_VIDLIC
                                #region ELEM_VIDREG
                                case ELEM_VIDREG:
                                    string vidregName = reader.GetAttribute(ATTR_NAME);
                                    string vidregID = reader.GetAttribute(ATTR_ID);

                                    if (string.IsNullOrEmpty(vidregID))
                                        vidregID = "0";

                                    if (string.IsNullOrEmpty(vidregName))
                                        vidregName = constDefaultClsName;

                                    int refTypeReg = PumpCachedRow(cacheTypesRegister, dsTypesRegister.Tables[0], clsTypesRegister, vidregID,
                                        new object[] { "CODE", vidregID, "NAME", vidregName, "SOURCEID", clsTypesRegisterVersion });

                                    AddMappingElement("REFTYPESREG", refTypeReg, ipNoteMapping);
                                    break;
                                #endregion ELEM_VIDREG
                            }
                            break;
                        #endregion XmlNodeType.Element

                        #region XmlNodeType.EndElement
                        case XmlNodeType.EndElement:
                            switch (reader.LocalName)
                            {
                                #region ELEM_IP
                                case ELEM_IP:
                                    if (!toSkipIp)
                                    {
                                        // запоминаем старый ID индивидуального предпринимателя, если он есть
                                        DataRow newRow = MappingToRow(ipMapping, dsEGRIP);
                                        string key = GetComplexCacheKey(newRow, new string[] { "OGRNIP", "INN" }, "|");
                                        int refOldIp = -1;
                                        if (cacheEGRIP.ContainsKey(key))
                                        {
                                            DataRow[] oldRows = GetLastRows(cacheEGRIP[key]);
                                            if (oldRows != null)
                                                refOldIp = Convert.ToInt32(oldRows[0]["ID"]);
                                        }

                                        // закачиваем ИП и получаем новый ID (может совпадать со старым)
                                        int refNewIp = PumpDataRow(newRow, clsEGRIP, dsEGRIP, cacheEGRIP, key);

                                        // закачиваем детали
                                        foreach (Detail detail in ipDetails.Values)
                                        {
                                            PumpDetailRows(detail, refNewIp, refOldIp, "RefIPEGRIP");
                                        }
                                    }
                                    toSkipIp = false;
                                    ClearDetails(ipDetails);
                                    break;
                                #endregion ELEM_IP

                                #region ELEM_ACCOUNT
                                case ELEM_ACCOUNT:
                                    if (!toSkipAccounts)
                                    {
                                        ipDetails["IPAccounts"].AddNewRow(ipAccountsMapping);
                                    }
                                    toSkipAccounts = false;
                                    ipAccountsMapping = new Dictionary<string, object>();
                                    break;
                                #endregion ELEM_ACCOUNT
                                #region ELEM_ADDRESS
                                case ELEM_ADDRESS:
                                    AddMappingElement("ADRESS", adress.GetAdress(), ipMapping);
                                    AddMappingElement("REFADRESS", PumpOrgAdress(adress), ipMapping);
                                    break;
                                #endregion ELEM_ADDRESS
                                #region ELEM_LICENZ
                                case ELEM_LICENZ:
                                    ipDetails["IPLicence"].AddNewRow(ipLicenceMapping);
                                    ipLicenceMapping = new Dictionary<string, object>();
                                    break;
                                #endregion ELEM_LICENZ
                                #region ELEM_REGEGRIP
                                case ELEM_REGEGRIP:
                                    toNote = false;
                                    ipDetails["IPNote"].AddNewRow(ipNoteMapping);
                                    ipNoteMapping = new Dictionary<string, object>();
                                    break;
                                #endregion ELEM_REGEGRIP
                                #region ELEM_REGOLD
                                case ELEM_REGOLD:
                                    AddMappingElement("OLDREG", oldRegParams.GetOldReg(), ipMapping);
                                    toRegOld = false;
                                    break;
                                #endregion ELEM_REGOLD
                            }
                            break;
                        #endregion XmlNodeType.EndElement
                    }
            }
            finally
            {
                oksmAbsentCodes.Clear();
                reader.Close();
            }
        }

        #endregion Закачка индивидуальных предпринимателей

        #region Закачка кладр

        private bool ToPumpKladr()
        {
            return (Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "cbPumpKladr", "False")));
        }

        private void PumpKLADRReport(FileInfo report)
        {
            IDbDataAdapter da = null;
            DataSet ds = null;
            try
            {
                // Подключаемся к источнику
                dbDataAccess.ConnectToDataSource(ref dbfDatabase, report.Directory.FullName, ODBCDriverName.Microsoft_dBase_Driver);
                InitDataSet(dbfDatabase, ref da, ref ds, string.Format("SELECT * FROM {0}", report.Name));

                int totalRows = ds.Tables[0].Rows.Count;
                for (int curRow = 0; curRow < totalRows; curRow++)
                {
                    this.SetProgress(totalRows, curRow + 1,
                        string.Format("Обработка файла {0}...", report.Name),
                        string.Format("{0} из {1}", curRow + 1, totalRows));

                    DataRow row = ds.Tables[0].Rows[curRow];

                    if (!ValidateLong(row["CODE"].ToString()))
                        row["CODE"] = 0;

                    if (!ValidateLong(row["UNO"].ToString()))
                        row["UNO"] = 0;

                    if (!ValidateLong(row["OCATD"].ToString()))
                        row["OCATD"] = 0;

                    if (string.IsNullOrEmpty(row["NAME"].ToString()))
                    {
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                            "Поле \'NAME\' не заполнено. Строка {0} будет пропущена.", curRow));
                        continue;
                    }

                    object[] mapping = new object[] {
                        "Code", row["CODE"],
                        "Name", row["NAME"],
                        "ShortName", row["SOCR"],
                        "Indeks", row["INDEX"],
                        "CodeMNS", row["GNINMB"],
                        "CodeMNSUchastok", row["UNO"],
                        "OKATO", row["OCATD"],
                        "SourceID", clsOKKladrVersion
                    };

                    PumpRow(dsOKKladr.Tables[0], clsOKKladr, mapping);
                    if (dsOKKladr.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                    {
                        UpdateDataSet(daOKKladr, dsOKKladr, clsOKKladr);
                        ClearDataSet(daOKKladr, dsOKKladr.Tables[0]);
                    }
                }
            }
            finally
            {
                ClearDataSet(ref ds);
                if (dbfDatabase != null)
                {
                    dbfDatabase.Close();
                    dbfDatabase = null;
                }
                if (dbDataAccess != null)
                    dbDataAccess.Dispose();
                GC.Collect();
            }
        }

        #endregion Закачка кладр

        #region Перекрытые методы закачки

        protected override void DirectClsHierarchySetting()
        {
            base.DirectClsHierarchySetting();
            switch (this.State)
            {
                case PumpProcessStates.PumpData:
                    clsOKKladr.DivideClassifierCode(this.SourceID);
                    break;
                case PumpProcessStates.ProcessData:
                    if (toFillOkatoField)
                        clsEGRUL.DivideClassifierCode(-1);
                    break;
            }
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, "RUV_M*.xml", new ProcessFileDelegate(PumpULReport), false);
            ProcessFilesTemplate(dir, "RIV_M*.xml", new ProcessFileDelegate(PumpIPReport), false);
            if (ToPumpKladr())
                ProcessFilesTemplate(dir, "KLADR.DBF", new ProcessFileDelegate(PumpKLADRReport), false);
        }

        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        protected override void DeleteEarlierPumpedData()
        {
            // чиста глушим метод, удаление произойдет позднее
        }

        #endregion Перекрытые методы закачки

        #region Внутренние объекты

        private enum Status
        {
            NoChanges = 1,
            NewRow = 2,
            Changed = 3
        }

        // деталь
        private class Detail
        {
            public IEntity clsObject = null;
            public DataSet dataSet;
            public Dictionary<string, DataRow[]> cache;
            public string[] keyFields;
            private List<DataRow> newRows;

            public Detail(IEntity cls, DataSet ds, Dictionary<string, DataRow[]> cache, string[] keyFields)
            {
                this.clsObject = cls;
                this.dataSet = ds;
                this.cache = cache;
                this.keyFields = keyFields;
                this.newRows = new List<DataRow>();
            }

            private DataRow MappingToRow(Dictionary<string, object> mapping)
            {
                DataRow row = this.dataSet.Tables[0].NewRow();
                row.BeginEdit();
                foreach (string key in mapping.Keys)
                {
                    row[key] = mapping[key];
                }
                row.EndEdit();
                return row;
            }

            public void AddNewRow(Dictionary<string, object> mapping)
            {
                this.newRows.Add(this.MappingToRow(mapping));
            }

            public List<DataRow> GetNewRows()
            {
                return this.newRows;
            }

            public void ClearNewRows()
            {
                this.newRows.Clear();
            }
        }

        // сведения о старой регистрации
        private class OldRegParams
        {
            internal string regNum = string.Empty;
            internal string regData = string.Empty;
            internal string regOrg = string.Empty;

            internal string GetOldReg()
            {
                List<string> oldRegParams = new List<string>();
                string oldReg = string.Empty;
                if (!string.IsNullOrEmpty(regNum))
                    oldRegParams.Add(string.Format("Регистрационный номер: {0}", regNum));
                if (!string.IsNullOrEmpty(regData))
                    oldRegParams.Add(string.Format("Дата регистрации: {0}", regData));
                if (!string.IsNullOrEmpty(regOrg))
                    oldRegParams.Add(string.Format("Регистрирующий орган: {0}", regOrg));
                if (oldRegParams.Count > 0)
                    oldReg = string.Join("; ", oldRegParams.ToArray());

                ClearParams();
                return oldReg;
            }

            internal void ClearParams()
            {
                regNum = string.Empty;
                regData = string.Empty;
                regOrg = string.Empty;
            }
        }

        // параметры организаций
        private class OrgParams
        {
            public string ogrn = string.Empty;
            public string nationalName = string.Empty;
            public List<string> UlUprAdress = new List<string>();
            public string UprContact = string.Empty;
            public string UprOKATO = string.Empty;

            public string address = string.Empty;
            public string Contact = string.Empty;
            public string region = string.Empty;
            public string gorod = string.Empty;
            public string raion = string.Empty;
            public string naspunkt = string.Empty;
            public string street = string.Empty;
            public string dom = string.Empty;
            public string korp = string.Empty;
            public string kvart = string.Empty;
            public string foms = string.Empty;
            public string anotherOkved = string.Empty;
            public string startUL = string.Empty;
            public string finishUL = string.Empty;
            public string dolgnFL = string.Empty;
            public string pfReg = string.Empty;
            public string fssReg = string.Empty;

            public OrgParams()
            {

            }

            public void ClearParams()
            {
                ogrn = string.Empty;
                nationalName = string.Empty;
                UlUprAdress.Clear();
                UprOKATO = string.Empty;
                address = string.Empty;
                Contact = string.Empty;
                region = string.Empty;
                gorod = string.Empty;
                raion = string.Empty;
                naspunkt = string.Empty;
                street = string.Empty;
                dom = string.Empty;
                korp = string.Empty;
                kvart = string.Empty;
                foms = string.Empty;
                anotherOkved = string.Empty;
                startUL = string.Empty;
                finishUL = string.Empty;
                dolgnFL = string.Empty;
                pfReg = string.Empty;
                fssReg = string.Empty;
            }
        }

        // адрес организации
        private class Adress
        {
            public string index = string.Empty;
            public string region = string.Empty;
            public string regionId = string.Empty;
            public string regionCode = string.Empty;
            public string raion = string.Empty;
            public string raionId = string.Empty;
            public string raionCode = string.Empty;
            public string gorod = string.Empty;
            public string gorodId = string.Empty;
            public string gorodCode = string.Empty;
            public string nasPunkt = string.Empty;
            public string nasPunktId = string.Empty;
            public string nasPunktCode = string.Empty;
            public string street = string.Empty;
            public string streetId = string.Empty;
            public string streetCode = string.Empty;
            public string dom = string.Empty;
            public string korp = string.Empty;
            public string kvart = string.Empty;

            public string GetAdress()
            {
                List<string> adressList = new List<string>();
                if (!string.IsNullOrEmpty(index))
                    adressList.Add(index);
                if (!string.IsNullOrEmpty(region))
                    adressList.Add(region);
                if (!string.IsNullOrEmpty(gorod))
                    adressList.Add(gorod);
                if (!string.IsNullOrEmpty(raion))
                    adressList.Add(raion);
                if (!string.IsNullOrEmpty(nasPunkt))
                    adressList.Add(nasPunkt);
                if (!string.IsNullOrEmpty(street))
                    adressList.Add(street);
                if (!string.IsNullOrEmpty(dom))
                    adressList.Add(dom);
                if (!string.IsNullOrEmpty(korp))
                    adressList.Add(korp);
                if (!string.IsNullOrEmpty(kvart))
                    adressList.Add(kvart);
                ClearParams();
                return string.Join("; ", adressList.ToArray());
            }

            internal void ClearParams()
            {
                index = string.Empty;
                region = string.Empty;
                regionId = string.Empty;
                regionCode = string.Empty;
                raion = string.Empty;
                raionId = string.Empty;
                raionCode = string.Empty;
                gorod = string.Empty;
                gorodId = string.Empty;
                gorodCode = string.Empty;
                nasPunkt = string.Empty;
                nasPunktId = string.Empty;
                nasPunktCode = string.Empty;
                street = string.Empty;
                streetId = string.Empty;
                streetCode = string.Empty;
                dom = string.Empty;
                korp = string.Empty;
                kvart = string.Empty;
            }
        }

        // контактный телефон
        private class Contact
        {
            public string kodGorod = string.Empty;
            public string telefon = string.Empty;
            public string fax = string.Empty;

            public void ClearParams()
            {
                kodGorod = string.Empty;
                telefon = string.Empty;
                fax = string.Empty;
            }

            public string GetContact()
            {
                List<string> contactList = new List<string>();
                if (!string.IsNullOrEmpty(kodGorod) && !string.IsNullOrEmpty(telefon))
                    contactList.Add(string.Format("Телефон: ({0}){1}", kodGorod, telefon));
                if (!string.IsNullOrEmpty(fax))
                    contactList.Add(string.Format("Факс: {0}", fax));

                ClearParams();
                return string.Join(" ", contactList.ToArray());
            }
        }

        // Индивидуальные предприниматели.Лицензии
        private class IPLicenzDetailParams
        {
            public string Numlic = string.Empty;
            public string DTRESH = string.Empty;
            public string DTSTART = string.Empty;
            public string DTEND = string.Empty;
            public string LicOrgName = string.Empty;
            public string VLic = string.Empty;
            public string SOSTLIC = string.Empty;
            public string OGRNIP = string.Empty;

            public IPLicenzDetailParams()
            {

            }

            public void ClearParams()
            {
                Numlic = string.Empty;
                DTRESH = string.Empty;
                DTSTART = string.Empty;
                DTEND = string.Empty;
                LicOrgName = string.Empty;
                VLic = string.Empty;
                SOSTLIC = string.Empty;
                OGRNIP = string.Empty;
            }
        }

        // Индивидуальные предприниматели.Изменения
        private class IPChangesDetail
        {
            public string IDREG = string.Empty;
            public string REGNUM = string.Empty;
            public string DTREG = string.Empty;
            public string DTZAP = string.Empty;
            public string VIDREG = string.Empty;
            public string REGORG = string.Empty;
            public string SOSTZAP = string.Empty;
            public string SVSV = string.Empty;
            public string OGRNIP = string.Empty;

            public IPChangesDetail()
            {

            }

            public void ClearParams()
            {
                IDREG = string.Empty;
                REGNUM = string.Empty;
                DTREG = string.Empty;
                DTZAP = string.Empty;
                VIDREG = string.Empty;
                REGORG = string.Empty;
                SOSTZAP = string.Empty;
                SVSV = string.Empty;
                OGRNIP = string.Empty;
            }
        }

        // параметры индивидуальных предпринимателей
        private class IPParams
        {
            public string OGRNIP = string.Empty;
            public string INN = string.Empty;
            public string FIO = string.Empty;
            public string FIOLat = string.Empty;
            public string mainOKVED = string.Empty;
            public List<string> dopOKVED = new List<string>();
            public List<string> regOld = new List<string>();
            public List<string> docDsn = new List<string>();
            public List<string> regPF = new List<string>();
            public List<string> regFss = new List<string>();
            public List<string> regFoms = new List<string>();
            public string refOKSM = string.Empty;
            public string sex = string.Empty;

            public IPParams()
            {

            }

            public void ClearParams()
            {
                OGRNIP = string.Empty;
                INN = string.Empty;
                FIO = string.Empty;
                FIOLat = string.Empty;
                mainOKVED = string.Empty;
                dopOKVED = new List<string>();
                regOld.Clear(); ;
                docDsn.Clear();
                regPF.Clear();
                regFss.Clear(); ;
                regFoms.Clear();
                refOKSM = string.Empty;
                sex = string.Empty;
            }
        }

        // фамилия, имя, отчество
        private class FIO
        {
            internal string Name = string.Empty;
            internal string Fam = string.Empty;
            internal string Otch = string.Empty;

            internal string GetFIO()
            {
                string fio = string.Empty;
                if (!string.IsNullOrEmpty(Fam))
                    fio = Fam;
                if (!string.IsNullOrEmpty(Name))
                    fio = fio + " " + Name;
                if (!string.IsNullOrEmpty(Otch))
                    fio = fio + " " + Otch;
                return fio;
            }

            internal void ClearFIO()
            {
                string Name = string.Empty;
                string Fam = string.Empty;
                string Otch = string.Empty;
            }
        }

        #endregion Внутренние объекты

        #endregion Закачка данных

    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FNS24Pump
{
    public partial class FNS24PumpModule : CorrectedPumpModuleBase
    {

        #region Работа с базой и кэшами

        // запрос уже существующих данных
        protected override void QueryData()
        {
            clsEGRULVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsEGRUL.ObjectKey);
            InitDataSet(ref daEGRUL, ref dsEGRUL, clsEGRUL, false, string.Format("SourceID = {0}", clsEGRULVersion), string.Empty);
            clsEGRIPVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsEGRIP.ObjectKey);
            InitDataSet(ref daEGRIP, ref dsEGRIP, clsEGRIP, false, string.Format("SourceID = {0}", clsEGRIPVersion), string.Empty);
            clsOKKladrVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsOKKladr.ObjectKey);
            InitDataSet(ref daOKKladr, ref dsOKKladr, clsOKKladr, false, string.Format("SourceID = {0}", clsOKKladrVersion), string.Empty);

            clsOkopfVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsOkopfBridge.ObjectKey);
            InitDataSet(ref daOkopfBridge, ref dsOkopfBridge, clsOkopfBridge, false, string.Format("SourceID = {0}", clsOkopfVersion), string.Empty);
            clsOksmVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsOksmBridge.ObjectKey);
            InitDataSet(ref daOksmBridge, ref dsOksmBridge, clsOksmBridge, false, string.Format("SourceID = {0}", clsOksmVersion), string.Empty);
            clsKopfVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsKopfBridge.ObjectKey);
            InitDataSet(ref daKopfBridge, ref dsKopfBridge, clsKopfBridge, false, string.Format("SourceID = {0}", clsKopfVersion), string.Empty);

            FillCaches();

            QueryClsData();
            QueryULDetailsData();
            QueryIPDetailsData();

            cacheStateRegisterNote.Remove("0");
            DataRow[] delRows = dsStateRegisterNote.Tables[0].Select("Code = 0");
            if (delRows.Length > 0)
            {
                foreach (DataRow row in delRows)
                    row.Delete();
                dsStateRegisterNote.AcceptChanges();
            }
        }

        // заполненить кэш, в котором записи сгруппированны по ключевым полям
        private void FillGroupedCache(ref Dictionary<string, DataRow[]> cache, DataTable dt, string[] keyFields)
        {
            cache = new Dictionary<string, DataRow[]>();
            DataRow[] rows = dt.Select(string.Empty, "Last desc");
            int rowsCount = rows.GetLength(0);
            for (int curRow = 0; curRow < rowsCount; curRow++)
            {
                DataRow row = rows[curRow];
                if (row.RowState != DataRowState.Deleted)
                {
                    string key = GetComplexCacheKey(row, keyFields, "|");
                    if (!cache.ContainsKey(key))
                        cache.Add(key, new DataRow[] { row });
                    else
                        cache[key] = (DataRow[])CommonRoutines.ConcatArrays(cache[key], new DataRow[] { row });
                }
            }
        }

        // заполнение кэшей
        private void FillCaches()
        {
            FillGroupedCache(ref cacheEGRUL, dsEGRUL.Tables[0], new string[] { "OGRN", "INN", "INN20" });
            FillGroupedCache(ref cacheEGRIP, dsEGRIP.Tables[0], new string[] { "OGRNIP", "INN" });

            FillRowsCache(ref cacheOkopfBridge, dsOkopfBridge.Tables[0], "Code");
            FillRowsCache(ref cacheOksmBridge, dsOksmBridge.Tables[0], "Code");
            FillRowsCache(ref cacheKopfBridge, dsKopfBridge.Tables[0], "Code");
        }

        // запись закаченных данных в базу
        protected override void UpdateData()
        {
            if (oksmAbsentCodes.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string absentCode in oksmAbsentCodes)
                {
                    sb.AppendLine(string.Format("Запись с кодом {0} не обнаружена в классификаторе 'ОКСМ.Cопоставимый'", absentCode));
                }
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, sb.ToString());
            }

            UpdateClsData();

            UpdateDataSet(daEGRUL, dsEGRUL, clsEGRUL);
            UpdateDataSet(daEGRIP, dsEGRIP, clsEGRIP);
            UpdateDataSet(daOKKladr, dsOKKladr, clsOKKladr);

            UpdateULDetailsData();
            UpdateIPDetailsData();
        }

        #region GUID

        private const string B_ORG_EGRUL_GUID = "7473679b-3ebb-43ca-999d-7f8fdd3efb34";
        private const string B_IP_EGRIP_GUID = "e7ac5579-1974-4a9f-8d83-80a8beace782";
        private const string B_OK_KLADR_GUID = "8b8ac6f4-d568-482b-87e4-eb156ef6070a";

        private const string B_OKOPF_BRIDGE_GUID = "89a8954e-ff48-4a82-aaaa-27bcd59f3b23";
        private const string B_OKSM_BRIDGE_GUID = "7ee163e1-082b-4134-a930-ea6bc3da5e51";
        private const string B_KOPF_BRIDGE_GUID = "a64a6b39-35e3-4220-9c3b-69870fd939ea";

        #endregion GUID
        protected override void InitDBObjects()
        {
            clsEGRUL = this.Scheme.Classifiers[B_ORG_EGRUL_GUID];
            clsEGRIP = this.Scheme.Classifiers[B_IP_EGRIP_GUID];
            clsOKKladr = this.Scheme.Classifiers[B_OK_KLADR_GUID];

            clsOkopfBridge = this.Scheme.Classifiers[B_OKOPF_BRIDGE_GUID];
            clsOksmBridge = this.Scheme.Classifiers[B_OKSM_BRIDGE_GUID];
            clsKopfBridge = this.Scheme.Classifiers[B_KOPF_BRIDGE_GUID];

            InitClsObjects();
            InitULDetailsObjects();
            InitIPDetailsObject();

            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { };
        }

        // очистка всех данных по окончании закачки
        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsEGRUL);
            ClearDataSet(ref dsEGRIP);
            ClearDataSet(ref dsOKKladr);

            ClearDataSet(ref dsOkopfBridge);
            ClearDataSet(ref dsOksmBridge);
            ClearDataSet(ref dsKopfBridge);

            ClearClsData();
            ClearULDetailsData();
            ClearIPDetailsData();
        }

        #region классификаторы

        private void FillClsCaches()
        {
            FillRowsCache(ref cacheTypesBankAcc, dsTypesBankAcc.Tables[0], "Code");
            FillRowsCache(ref cacheTypesLicenz, dsTypesLicenz.Tables[0], "Code", "ID");
            FillRowsCache(ref cacheTypesRegister, dsTypesRegister.Tables[0], "Code");

            FillRowsCache(ref cacheDocRightAbility, dsDocRightAbility.Tables[0], "Code");
            FillRowsCache(ref cacheDocHabitatRF, dsDocHabitatRF.Tables[0], "Code");
            FillRowsCache(ref cacheDocBaseAbility, dsDocBaseAbility.Tables[0], "Code");
            FillRowsCache(ref cacheDocKindPerson, dsDocKindPerson.Tables[0], "Code");

            FillRowsCache(ref cacheIPKindFunction, dsIPKindFunction.Tables[0], "Code");
            FillRowsCache(ref cacheIPStatus, dsIPStatus.Tables[0], new string[] { "Code", "Name" }, "|", "ID");

            FillRowsCache(ref cacheOKCitizen, dsOKCitizen.Tables[0], "Code");
            FillRowsCache(ref cacheOKINLanguage, dsOKINLanguage.Tables[0], "Code");

            FillRowsCache(ref cacheOrgKindCapital, dsOrgKindCapital.Tables[0], "NAME");
            FillRowsCache(ref cacheOrgStatus, dsOrgStatus.Tables[0], "Code");
            FillRowsCache(ref cacheOrgMNS, dsOrgMNS.Tables[0], "Code");
            FillRowsCache(ref cacheOrgOrganPF, dsOrgOrganPF.Tables[0], "Code");
            FillRowsCache(ref cacheOrgOrganFSS, dsOrgOrganFSS.Tables[0], "Code");
            FillRowsCache(ref cacheOrgOrganFOMS, dsOrgOrganFOMS.Tables[0], "Code");
            FillRowsCache(ref cacheOrgRegisterOrgan, dsOrgRegisterOrgan.Tables[0], "Code");

            FillRowsCache(ref cacheOrgKindSubdivision, dsOrgKindSubdivision.Tables[0], "Code");
            FillRowsCache(ref cacheOrgLicenceOrgan, dsOrgLicenceOrgan.Tables[0], "Code");

            FillRowsCache(ref cacheStateLicence, dsStateLicence.Tables[0], "Code");
            FillRowsCache(ref cacheStateRegisterNote, dsStateRegisterNote.Tables[0], new string[] { "Code", "Name" }, "|", "ID");

            FillRowsCache(ref cacheKindBankAcc, dsKindBankAcc.Tables[0], "Code");
        }

        private void QueryClsData()
        {
            clsTypesBankAccVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsTypesBankAcc.ObjectKey);
            InitDataSet(ref daTypesBankAcc, ref dsTypesBankAcc, clsTypesBankAcc, false, string.Format("SourceID = {0}", clsTypesBankAccVersion), string.Empty);
            clsTypesRegisterVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsTypesRegister.ObjectKey);
            InitDataSet(ref daTypesRegister, ref dsTypesRegister, clsTypesRegister, false, string.Format("SourceID = {0}", clsTypesRegisterVersion), string.Empty);
            clsTypesLicenzVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsTypesLicenz.ObjectKey);
            InitDataSet(ref daTypesLicenz, ref dsTypesLicenz, clsTypesLicenz, false, string.Format("SourceID = {0}", clsTypesLicenzVersion), string.Empty);

            clsDocRightAbilityVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsDocRightAbility.ObjectKey);
            InitDataSet(ref daDocRightAbility, ref dsDocRightAbility, clsDocRightAbility, false, string.Format("SourceID = {0}", clsDocRightAbilityVersion), string.Empty);
            clsDocHabitatRFVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsDocHabitatRF.ObjectKey);
            InitDataSet(ref daDocHabitatRF, ref dsDocHabitatRF, clsDocHabitatRF, false, string.Format("SourceID = {0}", clsDocHabitatRFVersion), string.Empty);
            clsDocBaseAbilityVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsDocBaseAbility.ObjectKey);
            InitDataSet(ref daDocBaseAbility, ref dsDocBaseAbility, clsDocBaseAbility, false, string.Format("SourceID = {0}", clsDocBaseAbilityVersion), string.Empty);
            clsDocKindPersonVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsDocKindPerson.ObjectKey);
            InitDataSet(ref daDocKindPerson, ref dsDocKindPerson, clsDocKindPerson, false, string.Format("SourceID = {0}", clsDocKindPersonVersion), string.Empty);

            clsIPKindFunctionVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsIPKindFunction.ObjectKey);
            InitDataSet(ref daIPKindFunction, ref dsIPKindFunction, clsIPKindFunction, false, string.Format("SourceID = {0}", clsIPKindFunctionVersion), string.Empty);
            clsIPStatusVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsIPStatus.ObjectKey);
            InitDataSet(ref daIPStatus, ref dsIPStatus, clsIPStatus, false, string.Format("SourceID = {0}", clsIPStatusVersion), string.Empty);


            clsOKCitizenVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsOKCitizen.ObjectKey);
            InitDataSet(ref daOKCitizen, ref dsOKCitizen, clsOKCitizen, false, string.Format("SourceID = {0}", clsOKCitizenVersion), string.Empty);
            clsOKINLanguageVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsOKINLanguage.ObjectKey);
            InitDataSet(ref daOKINLanguage, ref dsOKINLanguage, clsOKINLanguage, false, string.Format("SourceID = {0}", clsOKINLanguageVersion), string.Empty);

            InitDataSet(ref daOrgAdress, ref dsOrgAdress, clsOrgAdress, false, string.Empty, string.Empty);
            clsOrgKindCapitalVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsOrgKindCapital.ObjectKey);
            InitDataSet(ref daOrgKindCapital, ref dsOrgKindCapital, clsOrgKindCapital, false, string.Format("SourceID = {0}", clsOrgKindCapitalVersion), string.Empty);
            clsOrgKindSubdivisionVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsOrgKindSubdivision.ObjectKey);
            InitDataSet(ref daOrgKindSubdivision, ref dsOrgKindSubdivision, clsOrgKindSubdivision, false, string.Format("SourceID = {0}", clsOrgKindSubdivisionVersion), string.Empty);
            clsOrgStatusVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsOrgStatus.ObjectKey);
            InitDataSet(ref daOrgStatus, ref dsOrgStatus, clsOrgStatus, false, string.Format("SourceID = {0}", clsOrgStatusVersion), string.Empty);
            clsOrgLicenceOrganVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsOrgLicenceOrgan.ObjectKey);
            InitDataSet(ref daOrgLicenceOrgan, ref dsOrgLicenceOrgan, clsOrgLicenceOrgan, false, string.Format("SourceID = {0}", clsOrgLicenceOrganVersion), string.Empty);
            clsOrgMNSVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsOrgMNS.ObjectKey);
            InitDataSet(ref daOrgMNS, ref dsOrgMNS, clsOrgMNS, false, string.Format("SourceID = {0}", clsOrgMNSVersion), string.Empty);
            clsOrgOrganPFVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsOrgOrganPF.ObjectKey);
            InitDataSet(ref daOrgOrganPF, ref dsOrgOrganPF, clsOrgOrganPF, false, string.Format("SourceID = {0}", clsOrgOrganPFVersion), string.Empty);
            clsOrgOrganFSSVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsOrgOrganFSS.ObjectKey);
            InitDataSet(ref daOrgOrganFSS, ref dsOrgOrganFSS, clsOrgOrganFSS, false, string.Format("SourceID = {0}", clsOrgOrganFSSVersion), string.Empty);
            clsOrgOrganFOMSVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsOrgOrganFOMS.ObjectKey);
            InitDataSet(ref daOrgOrganFOMS, ref dsOrgOrganFOMS, clsOrgOrganFOMS, false, string.Format("SourceID = {0}", clsOrgOrganFOMSVersion), string.Empty);
            clsOrgRegisterOrganVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsOrgRegisterOrgan.ObjectKey);
            InitDataSet(ref daOrgRegisterOrgan, ref dsOrgRegisterOrgan, clsOrgRegisterOrgan, false, string.Format("SourceID = {0}", clsOrgRegisterOrganVersion), string.Empty);

            clsStateLicenceVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsStateLicence.ObjectKey);
            InitDataSet(ref daStateLicence, ref dsStateLicence, clsStateLicence, false, string.Format("SourceID = {0}", clsStateLicenceVersion), string.Empty);
            clsStateRegisterNoteVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsStateRegisterNote.ObjectKey);
            InitDataSet(ref daStateRegisterNote, ref dsStateRegisterNote, clsStateRegisterNote, false, string.Format("SourceID = {0}", clsStateRegisterNoteVersion), string.Empty);

            clsKindBankAccVersion = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsKindBankAcc.ObjectKey);
            InitDataSet(ref daKindBankAcc, ref dsKindBankAcc, clsKindBankAcc, false, string.Format("SourceID = {0}", clsKindBankAccVersion), string.Empty);

            FillClsCaches();
        }

        #region GUIDs

        private const string B_TYPES_BANK_ACC_GUID = "0bea0d85-39b7-48a7-833e-08d96464538b";
        private const string B_TYPES_LICENZ_GUID = "ee392814-f487-47be-a46c-7849bb0c20cc";
        private const string B_TYPES_REGISTER_GUID = "1db68e14-980c-46a0-9825-86126815b905";

        private const string B_DOC_RIGHT_ABILITY_GUID = "be714fcf-872c-47f9-9e59-88fabffbcae9";
        private const string B_DOC_HABITAT_RF_GUID = "4647e022-1a79-48e0-a476-784ce6053f26";
        private const string B_DOC_BASE_ABILITY_GUID = "7257d20b-2029-48f2-a017-caafffa4253e";
        private const string B_DOC_KIND_PERSON_GUID = "e64f6489-5e28-4fba-a563-ecb10df0e189";

        private const string B_IP_KIND_FUNCTION_GUID = "5b804e21-1ab5-42ed-93f3-8e48c8d93ac2";
        private const string B_IP_STATUS_GUID = "a2ab2bfc-94c2-4ba7-9b9b-b9e28daee4f8";

        private const string B_OK_CITIZEN_GUID = "d1b88d2e-bda8-4053-b60b-8d4bf0f6aec6";
        private const string B_OKIN_LANGUAGE_GUID = "f8008184-26cb-4eaf-a4d8-b2b8d38c3673";

        private const string D_ORG_ADRESS_GUID = "c49761eb-7667-4506-82a0-cb2f42b6882a";
        private const string B_ORG_KIND_CAPITAL_GUID = "b56e2ea8-1b98-41a0-a464-b7d2c5b7d040";
        private const string B_ORG_KIND_SUBDIVISION_GUID = "feab066d-196a-461c-96ad-8428aae6fd72";
        private const string B_ORG_STATUS_GUID = "908c6017-76aa-4d94-9387-e1e85623d7f3";
        private const string B_ORG_LICENCE_ORGAN_GUID = "bc037f52-1202-417d-92a3-45225b61fa75";
        private const string B_ORG_MNS_GUID = "69c967f0-31a7-4c3c-b2d8-801ae3cd2bf0";
        private const string B_ORG_ORGAN_PF_GUID = "7ef09fd5-2e8b-4c41-8405-afb3fc4e2d05";
        private const string B_ORG_ORGAN_FSS_GUID = "e202c371-fce5-4967-8410-5954a1bfd7b3";
        private const string B_ORG_ORGAN_FOMS_GUID = "ce240a8f-a9c2-46e0-a6c3-9b5841a7385a";
        private const string B_ORG_REGISTER_ORGAN_GUID = "6af6b4aa-c512-4abe-ab46-75535179c11d";

        private const string B_STATE_LICENCE_GUID = "e05c0b38-0eda-4bc3-86d1-d4e63fd8ed13";
        private const string B_STATE_REGISTER_NOTE_GUID = "1188f6b3-c3fa-4a87-a53d-6d7b68dadfbf";

        private const string B_KIND_BANK_ACC_GUID = "b29317b6-0f77-43e6-bb62-df8130cde87b";

        #endregion GUIDs
        private void InitClsObjects()
        {
            clsTypesBankAcc = this.Scheme.Classifiers[B_TYPES_BANK_ACC_GUID];
            clsTypesLicenz = this.Scheme.Classifiers[B_TYPES_LICENZ_GUID];
            clsTypesRegister = this.Scheme.Classifiers[B_TYPES_REGISTER_GUID];

            clsDocRightAbility = this.Scheme.Classifiers[B_DOC_RIGHT_ABILITY_GUID];
            clsDocHabitatRF = this.Scheme.Classifiers[B_DOC_HABITAT_RF_GUID];
            clsDocBaseAbility = this.Scheme.Classifiers[B_DOC_BASE_ABILITY_GUID];
            clsDocKindPerson = this.Scheme.Classifiers[B_DOC_KIND_PERSON_GUID];

            clsIPKindFunction = this.Scheme.Classifiers[B_IP_KIND_FUNCTION_GUID];
            clsIPStatus = this.Scheme.Classifiers[B_IP_STATUS_GUID];

            clsOKCitizen = this.Scheme.Classifiers[B_OK_CITIZEN_GUID];
            clsOKINLanguage = this.Scheme.Classifiers[B_OKIN_LANGUAGE_GUID];

            clsOrgAdress = this.Scheme.Classifiers[D_ORG_ADRESS_GUID];
            clsOrgKindCapital = this.Scheme.Classifiers[B_ORG_KIND_CAPITAL_GUID];
            clsOrgKindSubdivision = this.Scheme.Classifiers[B_ORG_KIND_SUBDIVISION_GUID];
            clsOrgStatus = this.Scheme.Classifiers[B_ORG_STATUS_GUID];
            clsOrgLicenceOrgan = this.Scheme.Classifiers[B_ORG_LICENCE_ORGAN_GUID];
            clsOrgMNS = this.Scheme.Classifiers[B_ORG_MNS_GUID];
            clsOrgOrganPF = this.Scheme.Classifiers[B_ORG_ORGAN_PF_GUID];
            clsOrgOrganFSS = this.Scheme.Classifiers[B_ORG_ORGAN_FSS_GUID];
            clsOrgOrganFOMS = this.Scheme.Classifiers[B_ORG_ORGAN_FOMS_GUID];
            clsOrgRegisterOrgan = this.Scheme.Classifiers[B_ORG_REGISTER_ORGAN_GUID];            

            clsStateLicence = this.Scheme.Classifiers[B_STATE_LICENCE_GUID];
            clsStateRegisterNote = this.Scheme.Classifiers[B_STATE_REGISTER_NOTE_GUID];

            clsKindBankAcc = this.Scheme.Classifiers[B_KIND_BANK_ACC_GUID];
        }

        private void UpdateClsData()
        {
            UpdateDataSet(daTypesBankAcc, dsTypesBankAcc, clsTypesBankAcc);
            UpdateDataSet(daTypesLicenz, dsTypesLicenz, clsTypesLicenz);
            UpdateDataSet(daTypesRegister, dsTypesRegister, clsTypesRegister);

            UpdateDataSet(daDocRightAbility, dsDocRightAbility, clsDocRightAbility);
            UpdateDataSet(daDocHabitatRF, dsDocHabitatRF, clsDocHabitatRF);
            UpdateDataSet(daDocBaseAbility, dsDocBaseAbility, clsDocBaseAbility);
            UpdateDataSet(daDocKindPerson, dsDocKindPerson, clsDocKindPerson);

            UpdateDataSet(daIPKindFunction, dsIPKindFunction, clsIPKindFunction);
            UpdateDataSet(daIPStatus, dsIPStatus, clsIPStatus);

            UpdateDataSet(daOKCitizen, dsOKCitizen, clsOKCitizen);
            UpdateDataSet(daOKINLanguage, dsOKINLanguage, clsOKINLanguage);

            UpdateDataSet(daOrgAdress, dsOrgAdress, clsOrgAdress);
            UpdateDataSet(daOrgKindSubdivision, dsOrgKindSubdivision, clsOrgKindSubdivision);
            UpdateDataSet(daOrgKindCapital, dsOrgKindCapital, clsOrgKindCapital);
            UpdateDataSet(daOrgStatus, dsOrgStatus, clsOrgStatus);
            UpdateDataSet(daOrgLicenceOrgan, dsOrgLicenceOrgan, clsOrgLicenceOrgan);
            UpdateDataSet(daOrgMNS, dsOrgMNS, clsOrgMNS);
            UpdateDataSet(daOrgOrganPF, dsOrgOrganPF, clsOrgOrganPF);
            UpdateDataSet(daOrgOrganFSS, dsOrgOrganFSS, clsOrgOrganFSS);
            UpdateDataSet(daOrgOrganFOMS, dsOrgOrganFOMS, clsOrgOrganFOMS);
            UpdateDataSet(daOrgRegisterOrgan, dsOrgRegisterOrgan, clsOrgRegisterOrgan);

            UpdateDataSet(daStateLicence, dsStateLicence, clsStateLicence);
            UpdateDataSet(daStateRegisterNote, dsStateRegisterNote, clsStateRegisterNote);

            UpdateDataSet(daKindBankAcc, dsKindBankAcc, clsKindBankAcc);
        }

        private void ClearClsData()
        {
            ClearDataSet(ref dsTypesBankAcc);
            ClearDataSet(ref dsTypesRegister);
            ClearDataSet(ref dsTypesLicenz);

            ClearDataSet(ref dsDocRightAbility);
            ClearDataSet(ref dsDocHabitatRF);
            ClearDataSet(ref dsDocBaseAbility);
            ClearDataSet(ref dsDocKindPerson);

            ClearDataSet(ref dsIPKindFunction);
            ClearDataSet(ref dsIPStatus);

            ClearDataSet(ref dsOKCitizen);
            ClearDataSet(ref dsOKINLanguage);

            ClearDataSet(ref dsOrgAdress);
            ClearDataSet(ref dsOrgKindCapital);
            ClearDataSet(ref dsOrgKindSubdivision);
            ClearDataSet(ref dsOrgStatus);
            ClearDataSet(ref dsOrgLicenceOrgan);
            ClearDataSet(ref dsOrgMNS);
            ClearDataSet(ref dsOrgOrganPF);
            ClearDataSet(ref dsOrgOrganFSS);
            ClearDataSet(ref dsOrgOrganFOMS);
            ClearDataSet(ref dsOrgRegisterOrgan);

            ClearDataSet(ref dsStateLicence);
            ClearDataSet(ref dsStateRegisterNote);

            ClearDataSet(ref dsKindBankAcc);
        }

        #endregion классификаторы

        #region детали для юридических лиц

        private void FillULDetailsCaches()
        {
            FillGroupedCache(ref cacheFounderOrgDet, dsFounderOrgDet.Tables[0], new string[] { "OGRN", "INN", "INN20", "RefOrgEGRUL" });
            FillGroupedCache(ref cacheFounderFLDet, dsFounderFLDet.Tables[0], new string[] { "FIO", "RefOrgEGRUL" });
            FillGroupedCache(ref cacheFounderForeignOrgDet, dsFounderForeignOrgDet.Tables[0], new string[] { "NameP", "RefOrgEGRUL" });
            FillGroupedCache(ref cacheAssignDet, dsAssignDet.Tables[0], new string[] { "OGRN", "INN", "INN20", "RefOrgEGRUL" });
            FillGroupedCache(ref cacheAncestorDet, dsAncestorDet.Tables[0], new string[] { "OGRN", "INN", "INN20", "RefOrgEGRUL" });
            FillGroupedCache(ref cacheSubdivisionDet, dsSubdivisionDet.Tables[0], new string[] { "Adress", "RefOrgEGRUL" });
            FillGroupedCache(ref cacheLicenceDet, dsLicenceDet.Tables[0], new string[] { "Nomer", "RefOrgEGRUL" });
            FillGroupedCache(ref cacheNoteDet, dsNoteDet.Tables[0], new string[] { "NumberReg", "RefOrgEGRUL" });
            FillGroupedCache(ref cacheOKVEDDet, dsOKVEDDet.Tables[0], new string[] { "Code", "RefOrgEGRUL" });
            FillGroupedCache(ref cacheAccountsDet, dsAccountsDet.Tables[0], new string[] { "OGRN", "INN", "INN20", "RefOrgEGRUL" });
            FillGroupedCache(ref cacheFaceProxyDet, dsFaceProxyDet.Tables[0], new string[] { "FIO", "RefOrgEGRUL" });
        }

        private void QueryULDetailsData()
        {
            InitDataSet(ref daFounderOrgDet, ref dsFounderOrgDet, clsFounderOrgDet, false, string.Empty, string.Empty);
            InitDataSet(ref daFounderFLDet, ref dsFounderFLDet, clsFounderFLDet, false, string.Empty, string.Empty);
            InitDataSet(ref daFounderForeignOrgDet, ref dsFounderForeignOrgDet, clsFounderForeignOrgDet, false, string.Empty, string.Empty);
            InitDataSet(ref daAssignDet, ref dsAssignDet, clsAssignDet, false, string.Empty, string.Empty);
            InitDataSet(ref daAncestorDet, ref dsAncestorDet, clsAncestorDet, false, string.Empty, string.Empty);
            InitDataSet(ref daSubdivisionDet, ref dsSubdivisionDet, clsSubdivisionDet, false, string.Empty, string.Empty);
            InitDataSet(ref daLicenceDet, ref dsLicenceDet, clsLicenceDet, false, string.Empty, string.Empty);
            InitDataSet(ref daNoteDet, ref dsNoteDet, clsNoteDet, false, string.Empty, string.Empty);
            InitDataSet(ref daOKVEDDet, ref dsOKVEDDet, clsOKVEDDet, false, string.Empty, string.Empty);
            InitDataSet(ref daAccountsDet, ref dsAccountsDet, clsAccountsDet, false, string.Empty, string.Empty);
            InitDataSet(ref daFaceProxyDet, ref dsFaceProxyDet, clsFaceProxyDet, false, string.Empty, string.Empty);
            FillULDetailsCaches();
        }

        #region GUIDs

        private const string T_ORG_FOUNDER_ORG_GUID = "0a8fda8b-999e-4a98-bc2e-870e453a4c55";
        private const string T_ORG_FOUNDER_FL_GUID = "0edd731c-eb1b-4193-8744-d4a8f3de171e";
        private const string T_ORG_FOUNDER_FOREIGN_ORG_GUID = "50ab2dc4-e586-45c5-a81e-b008ad358140";
        private const string T_ORG_ASSIGN_GUID = "79e02c78-a9f7-428a-b170-6faca92b64e2";
        private const string T_ORGANIZATIONS_ANCESTOR_GUID = "8d62ff82-4f8f-4d79-9c45-bc9028a1beb8";
        private const string T_ORG_SUBDIVISION_GUID = "34d0a8cb-da60-4ee4-914f-d9331e65400c";
        private const string T_ORG_LICENCE_GUID = "aaaead39-e36b-4b47-b912-f9bbdbab0cb9";
        private const string T_ORG_NOTE_GUID = "50ba9556-3d9e-4eec-97da-548f37d3108b";
        private const string T_ORG_OKVED_GUID = "06e2c32b-395f-49e7-963a-c3edbdb9a194";
        private const string T_ORG_ACCOUNTS_GUID = "11557496-74a1-48f4-ab3b-57ef19c3e99b";
        private const string T_ORG_FACE_PROXY_GUID = "d8e0818d-ff29-4057-aa3f-2e12171aa473";

        #endregion GUIDs
        private void InitULDetailsObjects()
        {
            foreach (IEntityAssociation association in clsEGRUL.Associated.Values)
            {
                if (association.AssociationClassType == AssociationClassTypes.MasterDetail)
                {
                    switch (association.RoleData.ObjectKey)
                    {
                        case T_ORG_FOUNDER_ORG_GUID:
                            clsFounderOrgDet = association.RoleData;
                            break;
                        case T_ORG_FOUNDER_FL_GUID:
                            clsFounderFLDet = association.RoleData;
                            break;
                        case T_ORG_FOUNDER_FOREIGN_ORG_GUID:
                            clsFounderForeignOrgDet = association.RoleData;
                            break;
                        case T_ORG_ASSIGN_GUID:
                            clsAssignDet = association.RoleData;
                            break;
                        case T_ORGANIZATIONS_ANCESTOR_GUID:
                            clsAncestorDet = association.RoleData;
                            break;
                        case T_ORG_SUBDIVISION_GUID:
                            clsSubdivisionDet = association.RoleData;
                            break;
                        case T_ORG_LICENCE_GUID:
                            clsLicenceDet = association.RoleData;
                            break;
                        case T_ORG_NOTE_GUID:
                            clsNoteDet = association.RoleData;
                            break;
                        case T_ORG_OKVED_GUID:
                            clsOKVEDDet = association.RoleData;
                            break;
                        case T_ORG_ACCOUNTS_GUID:
                            clsAccountsDet = association.RoleData;
                            break;
                        case T_ORG_FACE_PROXY_GUID:
                            clsFaceProxyDet = association.RoleData;
                            break;
                    }
                }
            }
        }

        private void UpdateULDetailsData()
        {
            UpdateDataSet(daFounderOrgDet, dsFounderOrgDet, clsFounderOrgDet);
            UpdateDataSet(daFounderFLDet, dsFounderFLDet, clsFounderFLDet);
            UpdateDataSet(daFounderForeignOrgDet, dsFounderForeignOrgDet, clsFounderForeignOrgDet);
            UpdateDataSet(daAssignDet, dsAssignDet, clsAssignDet);
            UpdateDataSet(daAncestorDet, dsAncestorDet, clsAncestorDet);
            UpdateDataSet(daSubdivisionDet, dsSubdivisionDet, clsSubdivisionDet);
            UpdateDataSet(daLicenceDet, dsLicenceDet, clsLicenceDet);
            UpdateDataSet(daNoteDet, dsNoteDet, clsNoteDet);
            UpdateDataSet(daOKVEDDet, dsOKVEDDet, clsOKVEDDet);
            UpdateDataSet(daAccountsDet, dsAccountsDet, clsAccountsDet);
            UpdateDataSet(daFaceProxyDet, dsFaceProxyDet, clsFaceProxyDet);
        }

        private void ClearULDetailsData()
        {
            ClearDataSet(ref dsFounderOrgDet);
            ClearDataSet(ref dsFounderFLDet);
            ClearDataSet(ref dsFounderForeignOrgDet);
            ClearDataSet(ref dsAssignDet);
            ClearDataSet(ref dsAncestorDet);
            ClearDataSet(ref dsSubdivisionDet);
            ClearDataSet(ref dsLicenceDet);
            ClearDataSet(ref dsNoteDet);
            ClearDataSet(ref dsOKVEDDet);
            ClearDataSet(ref dsAccountsDet);
            ClearDataSet(ref dsFaceProxyDet);
        }

        #endregion детали для юридических лиц

        #region детали для индивидуальных предпринимателей

        private void FillIPDetailsCaches()
        {
            FillGroupedCache(ref cacheIPLicenceDet, dsIPLicenceDet.Tables[0], new string[] { "Nomer", "RefIPEGRIP" });
            FillGroupedCache(ref cacheIPNoteDet, dsIPNoteDet.Tables[0], new string[] { "NumberReg", "RefIPEGRIP" });
            FillGroupedCache(ref cacheIPOkvedDet, dsIPOkvedDet.Tables[0], new string[] { "Code", "RefIPEGRIP" });
            FillGroupedCache(ref cacheIPAccountsDet, dsIPAccountsDet.Tables[0], new string[] { "NumAcc", "RefIPEGRIP" });
        }

        private void QueryIPDetailsData()
        {
            InitDataSet(ref daIPLicenceDet, ref dsIPLicenceDet, clsIPLicenceDet, false, string.Empty, string.Empty);
            InitDataSet(ref daIPNoteDet, ref dsIPNoteDet, clsIPNoteDet, false, string.Empty, string.Empty);
            InitDataSet(ref daIPOkvedDet, ref dsIPOkvedDet, clsIPOkvedDet, false, string.Empty, string.Empty);
            InitDataSet(ref daIPAccountsDet, ref dsIPAccountsDet, clsIPAccountsDet, false, string.Empty, string.Empty);
            FillIPDetailsCaches();
        }

        #region GUIDs

        private const string T_IP_LICENCE_GUID = "8c3e731d-11bf-41cf-988c-1023fd7926ef";
        private const string T_IP_NOTE_GUID = "aeea6c63-a34b-4cfc-bff7-3db3c90d0fd8";
        private const string T_IP_OKVED_GUID = "01578c8a-fdb6-42fc-bd47-d805b6bf623a";
        private const string T_IP_ACCOUNTS_GUID = "2dffffea-de39-4532-a4ac-ee8b7c41ea71";

        #endregion GUIDs
        private void InitIPDetailsObject()
        {
            foreach (IEntityAssociation association in clsEGRIP.Associated.Values)
            {
                if (association.AssociationClassType == AssociationClassTypes.MasterDetail)
                {
                    switch (association.RoleData.ObjectKey)
                    {
                        case T_IP_LICENCE_GUID:
                            clsIPLicenceDet = association.RoleData;
                            break;
                        case T_IP_NOTE_GUID:
                            clsIPNoteDet = association.RoleData;
                            break;
                        case T_IP_OKVED_GUID:
                            clsIPOkvedDet = association.RoleData;
                            break;
                        case T_IP_ACCOUNTS_GUID:
                            clsIPAccountsDet = association.RoleData;
                            break;
                    }
                }
            }
        }

        private void UpdateIPDetailsData()
        {
            UpdateDataSet(daIPLicenceDet, dsIPLicenceDet, clsIPLicenceDet);
            UpdateDataSet(daIPNoteDet, dsIPNoteDet, clsIPNoteDet);
            UpdateDataSet(daIPOkvedDet, dsIPOkvedDet, clsIPOkvedDet);
            UpdateDataSet(daIPAccountsDet, dsIPAccountsDet, clsIPAccountsDet);
        }

        private void ClearIPDetailsData()
        {
            ClearDataSet(ref dsIPLicenceDet);
            ClearDataSet(ref dsIPNoteDet);
            ClearDataSet(ref dsIPOkvedDet);
            ClearDataSet(ref dsIPAccountsDet);
        }

        #endregion детали для индивидуальных предпринимателей

        #endregion Работа с базой и кэшами

        #region Общие методы

        private void FormatMainOKVED(ref string okved)
        {
            okved = okved.Replace(".", string.Empty).PadRight(6, '0');
        }

        private bool ValidateDecimal(string value)
        {
            decimal decimalValue = 0;
            return Decimal.TryParse(value, out decimalValue);
        }

        private decimal TryGetDecimalValue(string value)
        {
            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {
                return 0;
            }
        }

        private bool ValidateLong(string value)
        {
            long intValue = 0;
            return Int64.TryParse(value, out intValue);
        }

        private long TryGetLongValue(object value)
        {
            try
            {
                return Convert.ToInt64(value);
            }
            catch
            {
                return 0;
            }
        }

        private int TryGetIntValue(object value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }

        private object CheckEmptyString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return DBNull.Value;
            return value;
        }

        private void WriteIncorrectOgrnUL(string innOrg, string kppOrg)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                "В файле {0} обнаружено некорректное значение ОГРН = '{1}' у организации с ИНН = {2} и КПП = {3}. " +
                "ОГРН будет присвоено значение 0. (IDDOC = {4})",
                pumpedFilename, currentOgrn, innOrg, kppOrg, currentIDDoc));
            this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithWarnings;
        }

        private void WriteIncorrectOgrnIP(string innOrg)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                "В файле {0} обнаружено некорректное значение ОГРН = '{1}' у ИП с ИНН = {2}. " +
                "ОГРН будет присвоено значение 0. (IDDOC = {3})",
                pumpedFilename, currentOgrn, innOrg, currentIDDoc));
            this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithWarnings;
        }

        private void WriteIncorrectInn(string innOrg, IEntity clsObj)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                "В файле {0} обнаружено некорректное значение ИНН = '{1}' классификатора \"{2}\". " +
                "Запись с таким ИНН будет пропущена. (IDDOC = {3}; ОГРН = {4})",
                pumpedFilename, innOrg, clsObj.OlapName, currentIDDoc, currentOgrn));
            this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithWarnings;
        }

        private void AddMappingElement(string elementKey, object elementValue, Dictionary<string, object> mapping)
        {
            if (elementValue == null)
                elementValue = DBNull.Value;
            if (elementValue.ToString() == string.Empty)
                elementValue = DBNull.Value;
            elementKey = elementKey.ToUpper();
            if (mapping.ContainsKey(elementKey))
                return;
            mapping.Add(elementKey, elementValue);
        }

        private DataRow MappingToRow(Dictionary<string, object> mapping, DataSet dataSet)
        {
            DataRow row = dataSet.Tables[0].NewRow();
            row.BeginEdit();
            foreach (string key in mapping.Keys)
            {
                row[key] = mapping[key];
            }
            row.EndEdit();
            return row;
        }

        private void ClearDetails(Dictionary<string, Detail> details)
        {
            foreach (Detail detail in details.Values)
            {
                detail.ClearNewRows();
            }
        }

        private XmlTextReader GetXMLReader(string reportFullName)
        {
            FileStream fs = new FileStream(reportFullName, FileMode.Open);
            return new XmlTextReader(fs);
        }

        #endregion Общие методы

        #region Методы закачки данных

        private int PumpOrgAdress(Adress adress)
        {
            if (!string.IsNullOrEmpty(adress.streetId) && (adress.streetId.Length > 15))
                adress.streetId = adress.streetId.Substring(0, 15);

            adress.regionCode = adress.regionCode.PadRight(11, '0');

            object[] mapping = new object[] { "INDEKS", adress.index,
                "IDREGION", CheckEmptyString(adress.regionId), "KODKLREGION", adress.regionCode, "NAMEREGION", adress.region,
                "IDRAION", CheckEmptyString(adress.raionId), "KODKLRAION", adress.raionCode, "NAMERAION", adress.raion,
                "IDGOROD", CheckEmptyString(adress.gorodId), "KODKLGOROD", adress.gorodCode, "NAMEGOROD", adress.gorod,
                "IDNASPUNKT", CheckEmptyString(adress.nasPunktId), "KODKLNASPUNKT", CheckEmptyString(adress.nasPunktCode), "NAMENASPUNKT", adress.nasPunkt,
                "IDSTREET", CheckEmptyString(adress.streetId), "KODSTREET", CheckEmptyString(adress.streetCode), "NAMESTREET", adress.street
            };
            return PumpRow(dsOrgAdress.Tables[0], clsOrgAdress, mapping);
        }

        // сравнить два значения
        private bool CompareValues(object newValue, object oldValue)
        {
            decimal oldDigitValue;
            decimal newDigitValue;
            if (Decimal.TryParse(oldValue.ToString(), out oldDigitValue) &&
                Decimal.TryParse(newValue.ToString(), out newDigitValue))
            {
                return oldDigitValue.Equals(newDigitValue);
            }
            DateTime oldDateTimeValue;
            DateTime newDateTimeValue;
            if (DateTime.TryParse(oldValue.ToString(), out oldDateTimeValue) &&
                DateTime.TryParse(newValue.ToString(), out newDateTimeValue))
            {
                return oldDateTimeValue.Equals(newDateTimeValue);
            }
            return oldValue.ToString().Equals(newValue.ToString());
        }

        // массив полей, неучаствующих в проверке
        private List<string> SKIP_FIELDS = new List<string>(new string[] {
            "ID", "ROWTYPE", "SOURCEID", "PARENTID", "LAST", "CODE1", "CODE2", "OKATO", "OKATODIRECT" });
        // проверить наличие изменений в новой записи относительно последней актуальной
        private bool CheckChanges(DataRow newRow, DataRow[] lastRows)
        {
            DataColumnCollection columns = newRow.Table.Columns;
            foreach (DataRow lastRow in lastRows)
            {
                bool isChanged = false;
                foreach (DataColumn column in columns)
                {
                    string columnName = column.ColumnName.ToUpper();
                    if (SKIP_FIELDS.Contains(columnName) || columnName.StartsWith("REF"))
                        continue;
                    if (!CompareValues(newRow[columnName], lastRow[columnName]))
                    {
                        isChanged = true;
                        break;
                    }
                }
                if (!isChanged)
                    return false;
            }
            return true;
        }

        // получить последнюю актуальную запись
        private DataRow[] GetLastRows(DataRow[] rows)
        {
            List<DataRow> lastRows = new List<DataRow>();
            foreach (DataRow row in rows)
            {
                if (Convert.ToInt32(row["Last"]) == 1)
                    lastRows.Add(row);
            }
            if (lastRows.Count > 0)
                return lastRows.ToArray();
            return null;
        }

        // обновить статус у записей
        private void UpdateStatus(DataRow[] rows, int last, Status status, object parentId)
        {
            foreach (DataRow row in rows)
            {
                row["Last"] = last;
                row["RefStatus"] = status;
                if (parentId != null)
                    row["ParentID"] = parentId;
            }
        }

        private void SetOldStatus(DataSet ds)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                int refStatus = Convert.ToInt32(row["RefStatus"]);
                int last = Convert.ToInt32(row["Last"]);
                if ((refStatus == 2) && (last == 1))
                    row["RefStatus"] = Status.NoChanges;
            }
        }

        private int PumpRow(IEntity obj, DataTable dt, DataRow row)
        {
            if (row == null)
                return -1;

            row["ID"] = GetGeneratorNextValue(obj);

            // классификаторы ЕГРЮЛ и ЕГРИП в этом месте пропускаем, т.к. они сопоставимые и
            // формируются не по текущему источнику, а по текущей версии
            if ((obj != clsEGRUL) && (obj != clsEGRIP))
            {
                if (dt.Columns.Contains("SOURCEID"))
                    row["SOURCEID"] = this.SourceID;
            }

            if (dt.Columns.Contains("PUMPID"))
                row["PUMPID"] = this.PumpID;

            if (dt.Columns.Contains("TASKID"))
                row["TASKID"] = -1;

            dt.Rows.Add(row);

            if (obj != null)
                return Convert.ToInt32(row["ID"]);
            return -1;
        }
        
        private int PumpDataRow(DataRow newRow, IEntity cls, DataSet ds, Dictionary<string, DataRow[]> cache, string key)
        {
            if (!cache.ContainsKey(key) || !updateMode)
            {
                // если в кэше записей по ключу нет или режим обновления выключен (в случае удаления старых данных),
                // то просто добавляем новую запись со статусом "Новое значение"
                UpdateStatus(new DataRow[] { newRow }, 1, Status.NewRow, DBNull.Value);
                return PumpRow(cls, ds.Tables[0], newRow);
            }
            else
            {
                // ищем последние актуальные записи в кэше (Last = 1)
                DataRow[] lastRows = GetLastRows(cache[key]);

                // сбрасываем статус, далее в процессе он установится какой надо
                // если не установится, значит запись удалена и статуса у неё не будет
                UpdateStatus(cache[key], 0, Status.Changed, null);

                if (lastRows == null)
                {
                    // если таких записей нет, то просто закачиваем новую запись со статусом "Новое значение"
                    UpdateStatus(new DataRow[] { newRow }, 1, Status.NewRow, DBNull.Value);
                    return PumpRow(cls, ds.Tables[0], newRow);
                }
                // если такие записи найдены, проверяем наличие изменений в новой записи
                else if (!CheckChanges(newRow, lastRows))
                {
                    // если изменений не было, меняем статус у старых записей на "Не изменилась"
                    UpdateStatus(lastRows, 1, Status.NoChanges, null);
                    return Convert.ToInt32(lastRows[0]["ID"]);
                }
                else
                {
                    // если изменения были, закачиваем новую запись со статусом "Изменено" и Last = 1
                    UpdateStatus(new DataRow[] { newRow }, 1, Status.Changed, DBNull.Value);
                    int refId = PumpRow(cls, ds.Tables[0], newRow);
                    // у старых записей меняем Last = 0, статус "Изменено", ссылка ParentID на новую запись
                    UpdateStatus(cache[key], 0, Status.Changed, refId);
                    return refId;
                }
            }
        }

        private void PumpDetailRows(Detail detail, int refNewId, int refOldId, string refField)
        {
            foreach (DataRow newRow in detail.GetNewRows())
            {
                // проверяем, обновилась ли у нас организация
                if ((refOldId != -1) && (refNewId != refOldId))
                {
                    // если обновилась, то нужно все детали старой организации перебросить на новую
                    newRow[refField] = refOldId;
                    string oldKey = GetComplexCacheKey(newRow, detail.keyFields, "|");
                    if (detail.cache.ContainsKey(oldKey))
                    {
                        foreach (DataRow oldRow in detail.cache[oldKey])
                            oldRow[refField] = refNewId;
                        newRow[refField] = refNewId;
                        string newKey = GetComplexCacheKey(newRow, detail.keyFields, "|");
                        detail.cache.Add(newKey, detail.cache[oldKey]);
                        detail.cache.Remove(oldKey);
                    }
                }

                // закачиваем деталь
                newRow[refField] = refNewId;
                string detailKey = GetComplexCacheKey(newRow, detail.keyFields, "|");
                PumpDataRow(newRow, detail.clsObject, detail.dataSet, detail.cache, detailKey);
            }
        }

        #endregion Методы закачки данных

    }
}

using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFK;
using Krista.FM.Client.Reports.Database.FactTables.EGRUL;
using Krista.FM.Client.Reports.Database.ClsBridge.EGRUL;
using Krista.FM.Client.Reports.Database.ClsFx;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 0001_ЕГРИП_ВЫПИСКА ПО ИП
        /// </summary>
        public DataTable[] GetEGRIP0001ReportData(Dictionary<string, string> reportParams)
        {
            DataTable[] tables = new DataTable[6];
            // 1. Выборки из мастер таблицы ЕГРИП
            EGRIPDataObject dataObject = new EGRIPDataObject();
            dataObject.InitObject(scheme);
            dataObject.useSummaryRow = false;
            dataObject.mainFilter[b_IP_EGRIP.ID] = reportParams[ReportConsts.ParamOrgID];
            // Основная информация
            dataObject.AddDataColumn(b_IP_EGRIP.OGRNIP);
            dataObject.AddDataColumn(b_IP_EGRIP.INN);
            dataObject.AddDataColumn(b_IP_EGRIP.FIO);
            dataObject.AddDataColumn(b_IP_EGRIP.Contact);
            // Регистрационные данные
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefIPKindFunction,
                b_IP_KindFunction.InternalKey,
                b_IP_KindFunction.Name);
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefIPStatus,
                b_IP_Status.InternalKey,
                b_IP_Status.Name);
            dataObject.AddDataColumn(b_IP_EGRIP.DateFinishMNS);
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefOrgRO,
                b_Org_RegisterOrgan.InternalKey,
                b_Org_RegisterOrgan.Name);
            dataObject.AddDataColumn(b_IP_EGRIP.OldReg);
            // Сведения о данных физического лица
            dataObject.AddDataColumn(b_IP_EGRIP.DtStartFL);
            dataObject.AddDataColumn(b_IP_EGRIP.FIO);
            dataObject.AddDataColumn(b_IP_EGRIP.FIOLat);
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefSex,
                fx_FX_Sex.InternalKey,
                fx_FX_Sex.Name);
            // Сведения о месте жительства в Российской Федерации
            dataObject.AddDataColumn(b_IP_EGRIP.DtStartAddr);
            dataObject.AddDataColumn(b_IP_EGRIP.Adress);
            dataObject.AddDataColumn(b_IP_EGRIP.OKATO);
            // Cведения о документе, удостоверяющего личность 
            dataObject.AddDataColumn(b_IP_EGRIP.DtStartDocFL);
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefDocKindPerson,
                b_Doc_KindPerson.InternalKey,
                b_Doc_KindPerson.Name);
            dataObject.AddDataColumn(b_IP_EGRIP.DocNum);
            dataObject.AddDataColumn(b_IP_EGRIP.DateDoc);
            dataObject.AddDataColumn(b_IP_EGRIP.NameOrg);
            dataObject.AddDataColumn(b_IP_EGRIP.KodOrg);
            // Сведения о гражданстве
            dataObject.AddDataColumn(b_IP_EGRIP.DtStartCitizen);
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefOKCitizen,
                b_OK_Citizen.InternalKey,
                b_OK_Citizen.Name);
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefOKSMBridge,
                b_OKSM_Bridge.InternalKey,
                b_OKSM_Bridge.Name);
            // Документ, подтверждающий право проживания в РФ
            dataObject.AddDataColumn(b_IP_EGRIP.DtStartDocHabitat);
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefDocHabitatRF,
                b_Doc_HabitatRF.InternalKey,
                b_Doc_HabitatRF.Name);
            dataObject.AddDataColumn(b_IP_EGRIP.HabitatDocNum);
            dataObject.AddDataColumn(b_IP_EGRIP.HabitatDocDate);
            dataObject.AddDataColumn(b_IP_EGRIP.HabitatNameOrg);
            dataObject.AddDataColumn(b_IP_EGRIP.HabitatDocDateEnd);
            // Документ, подтверждающий приобретение дееспособности несовершеннолетним
            dataObject.AddDataColumn(b_IP_EGRIP.DtStartAbility);
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefDocRightAbility,
                b_Doc_RightAbility.InternalKey,
                b_Doc_RightAbility.Name);
            dataObject.AddDataColumn(b_IP_EGRIP.AbilityDocNum);
            dataObject.AddDataColumn(b_IP_EGRIP.AbilityNameOrg);
            dataObject.AddDataColumn(b_IP_EGRIP.AbilityDocDate);
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefDocBaseAbility,
                b_Doc_BaseAbility.InternalKey,
                b_Doc_BaseAbility.Name);
            // Сведения о постановке на учет в налоговом органе
            dataObject.AddDataColumn(b_IP_EGRIP.DateStartMNS);
            dataObject.AddDataColumn(b_IP_EGRIP.DateFinishMNS);
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefOrgMNS,
                b_Org_MNS.InternalKey,
                Combine(b_Org_MNS.Code, b_Org_MNS.Name));
            // ПФ
            dataObject.AddDataColumn(b_IP_EGRIP.RegNumPF);
            dataObject.AddDataColumn(b_IP_EGRIP.DateStartPF);
            dataObject.AddDataColumn(b_IP_EGRIP.DateFinishPF);
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefOrgPF,
                b_Org_OrganPF.InternalKey,
                Combine(b_Org_OrganPF.Code, b_Org_OrganPF.Name));
            // ФСС
            dataObject.AddDataColumn(b_IP_EGRIP.RegNumFSS);
            dataObject.AddDataColumn(b_IP_EGRIP.DateStartFSS);
            dataObject.AddDataColumn(b_IP_EGRIP.DateFinishFSS);
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefOrgFSS,
                b_Org_OrganFSS.InternalKey,
                Combine(b_Org_OrganFSS.Code, b_Org_OrganFSS.Name));
            // ФОМС
            dataObject.AddDataColumn(b_IP_EGRIP.RegNumFOMS);
            dataObject.AddDataColumn(b_IP_EGRIP.DateStartFOMS);
            dataObject.AddDataColumn(b_IP_EGRIP.DateFinishFOMS);
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefOrgFOMS,
                b_Org_OrganFOMS.InternalKey,
                Combine(b_Org_OrganFOMS.Code, b_Org_OrganFOMS.Name));
            DataTable tableData = dataObject.FillData();
            // 2. Таблицы, ссылющиеся на эту запись
            DataRow rowOrg = GetLastRow(tableData);
            if (rowOrg != null)
            {
                string refIP = Convert.ToString(rowOrg[b_IP_EGRIP.ID]);
                // Сведения о видах экономической деятельности
                EGRIPDataObject dataOrgKind = new EGRIPDataObject();
                dataOrgKind.InitObject(scheme);
                dataOrgKind.useSummaryRow = false;
                dataOrgKind.ObjectKey = t_IP_OKVED.InternalKey;
                dataOrgKind.mainFilter[t_IP_OKVED.RefIPEGRIP] = refIP;
                dataOrgKind.AddParamColumn(EGRULDataObject.ColumnOKVEDMaskCode, t_Org_OKVED.Code);
                dataOrgKind.AddDataColumn(t_IP_OKVED.Sign);
                dataOrgKind.AddDataColumn(t_IP_OKVED.Name);
                dataOrgKind.AddDataColumn(t_IP_OKVED.Code);
                dataOrgKind.sortString = FormSortString(StrSortDown(t_IP_OKVED.Sign), StrSortUp(t_IP_OKVED.Code));
                tables[1] = dataOrgKind.FillData();
                // Сведения о лицензиях
                EGRIPDataObject dataLicense = new EGRIPDataObject();
                dataLicense.InitObject(scheme);
                dataLicense.useSummaryRow = false;
                dataLicense.ObjectKey = t_IP_Licence.InternalKey;
                dataLicense.mainFilter[t_IP_Licence.RefIPEGRIP] = refIP;
                dataLicense.AddDataColumn(t_IP_Licence.Nomer);
                dataLicense.AddParamColumn(
                    EGRULDataObject.ColumnCommonBookData,
                    t_IP_Licence.RefType,
                    b_Types_Licenz.InternalKey,
                    b_Types_Licenz.Name);
                dataLicense.AddDataColumn(t_IP_Licence.DateLicence);
                dataLicense.AddDataColumn(t_IP_Licence.DateStartLicence);
                dataLicense.AddDataColumn(t_IP_Licence.DateFinishLicence);
                dataLicense.AddDataColumn(t_IP_Licence.DateStopLic);
                dataLicense.AddDataColumn(t_IP_Licence.DateResume);
                dataLicense.AddParamColumn(
                    EGRULDataObject.ColumnCommonBookData,
                    t_IP_Licence.RefStateLicence,
                    b_State_Licence.InternalKey,
                    b_State_Licence.Name);
                dataLicense.AddParamColumn(
                    EGRULDataObject.ColumnCommonBookData,
                    t_IP_Licence.RefOrgLic,
                    b_Org_LicenceOrgan.InternalKey,
                    b_Org_LicenceOrgan.Name);
                dataLicense.sortString = StrSortUp(t_IP_Licence.DateLicence);
                tables[2] = dataLicense.FillData();
                // Сведения о счетах
                EGRIPDataObject dataAccounts = new EGRIPDataObject();
                dataAccounts.InitObject(scheme);
                dataAccounts.useSummaryRow = false;
                dataAccounts.ObjectKey = t_IP_Accounts.InternalKey;
                dataAccounts.mainFilter[t_IP_Accounts.RefIPEGRIP] = refIP;
                dataAccounts.AddDataColumn(t_IP_Accounts.NumAcc);
                dataAccounts.AddDataColumn(t_IP_Accounts.OGRN);
                dataAccounts.AddDataColumn(t_IP_Accounts.INN);
                dataAccounts.AddDataColumn(t_IP_Accounts.INN20);
                dataAccounts.AddDataColumn(t_IP_Accounts.BIK);
                dataAccounts.AddDataColumn(t_IP_Accounts.NumContract);
                dataAccounts.AddDataColumn(t_IP_Accounts.DateStartContr);
                dataAccounts.AddDataColumn(t_IP_Accounts.DateEndContr);
                dataAccounts.AddParamColumn(
                    EGRULDataObject.ColumnCommonBookData,
                    t_IP_Accounts.RefTypeAcc,
                    b_Types_BankAcc.InternalKey,
                    b_Types_BankAcc.Name);
                dataAccounts.AddParamColumn(
                    EGRULDataObject.ColumnCommonBookData,
                    t_IP_Accounts.RefKindAcc,
                    b_Kind_BankAcc.InternalKey,
                    b_Kind_BankAcc.Name);
                dataAccounts.sortString = StrSortUp(t_IP_Accounts.DateStartContr);
                tables[3] = dataAccounts.FillData();
                // Сведения о записях в ЕГРИП
                EGRULDataObject dataNotes = new EGRULDataObject();
                dataNotes.InitObject(scheme);
                dataNotes.useSummaryRow = false;
                dataNotes.ObjectKey = t_IP_Note.InternalKey;
                dataNotes.mainFilter[t_IP_Note.RefIPEGRIP] = refIP;
                dataNotes.AddDataColumn(t_IP_Note.DateReg, ReportConsts.ftDateTime);
                dataNotes.AddDataColumn(t_IP_Note.NumberReg);
                dataNotes.AddDataColumn(t_IP_Note.DateNote);
                dataNotes.AddDataColumn(t_IP_Note.IDReg);
                dataNotes.AddParamColumn(
                    EGRULDataObject.ColumnCommonBookData,
                    b_IP_EGRIP.RefOrgRO,
                    b_Org_RegisterOrgan.InternalKey,
                    Combine(b_Org_RegisterOrgan.Code, b_Org_RegisterOrgan.Name));
                dataNotes.AddParamColumn(
                    EGRULDataObject.ColumnCommonBookData,
                    t_IP_Note.RefTypesReg,
                    b_Types_Register.InternalKey,
                    b_Types_Register.Name);
                dataNotes.AddDataColumn(t_IP_Note.Certificate);
                dataNotes.AddParamColumn(
                    EGRULDataObject.ColumnCommonBookData,
                    t_IP_Note.RefStateReg,
                    b_State_RegisterNote.InternalKey,
                    b_State_RegisterNote.Name);
                dataNotes.sortString = StrSortUp(t_IP_Note.DateReg);
                tables[4] = dataNotes.FillData();
            }
            else
            {
                for (int i = 1; i < tables.Length - 1; i++)
                {
                    tables[i] = CreateReportCaptionTable(50);
                }
            }

            tables[0] = tableData;
            DataRow rowCaption = CreateReportParamsRow(tables);
            rowCaption[0] = DateTime.Now.ToShortDateString();
            rowCaption[1] = reportParams[ReportConsts.ParamReportList];
            return tables;
        }
    }
}

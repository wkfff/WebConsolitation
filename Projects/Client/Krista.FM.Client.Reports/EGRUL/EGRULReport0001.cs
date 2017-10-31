using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFK;
using Krista.FM.Client.Reports.Database.FactTables.EGRUL;
using Krista.FM.Client.Reports.Database.ClsBridge.EGRUL;
using Krista.FM.Client.Reports.Database.ClsBridge;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 0001_ЕГРЮЛ_ВЫБОР ОРГАНИЗАЦИЙ МУНИЦИПАЛЬНОГО ОБРАЗОВАНИЯ
        /// </summary>
        public DataTable[] GetEGRUL0001ReportData(Dictionary<string, string> reportParams)
        {
            DataTable[] tables = new DataTable[13];
            // 1. Выборки из мастер таблицы ЕГРЮЛ
            EGRULDataObject dataObject = new EGRULDataObject();
            dataObject.InitObject(scheme);
            dataObject.useSummaryRow = false;
            dataObject.mainFilter[b_Org_EGRUL.ID] = reportParams[ReportConsts.ParamOrgID];
            // Основная информация
            dataObject.AddDataColumn(b_Org_EGRUL.OGRN);
            dataObject.AddDataColumn(b_Org_EGRUL.INN);
            dataObject.AddDataColumn(b_Org_EGRUL.INN20);
            dataObject.AddDataColumn(b_Org_EGRUL.NameP);
            dataObject.AddDataColumn(b_Org_EGRUL.ShortName);
            dataObject.AddDataColumn(b_Org_EGRUL.Adress);
            // Сведения о наименовании  юридического лица
            dataObject.AddDataColumn(b_Org_EGRUL.DateName);
            dataObject.AddDataColumn(b_Org_EGRUL.NameP);
            dataObject.AddDataColumn(b_Org_EGRUL.ShortName);            
            dataObject.AddDataColumn(b_Org_EGRUL.FirmName);
            dataObject.AddDataColumn(b_Org_EGRUL.NationalName);
            dataObject.AddDataColumn(b_Org_EGRUL.ForeignName);
            // Сведения об организационно-правовой форме
            dataObject.AddParamColumn(EGRULDataObject.ColumnKOPFData, EGRULDataObject.KOPFTemplate);
            dataObject.AddParamColumn(EGRULDataObject.ColumnKOPFData, b_OKOPF_Bridge.Code);
            dataObject.AddParamColumn(EGRULDataObject.ColumnKOPFData, b_OKOPF_Bridge.Name);
            // Сведения о постановке на учет в налоговом органе
            dataObject.AddParamColumn(EGRULDataObject.ColumnOrgMNSData, b_Org_MNS.Code);
            dataObject.AddParamColumn(EGRULDataObject.ColumnOrgMNSData, b_Org_MNS.Name);
            dataObject.AddDataColumn(b_Org_EGRUL.DateStartMNS);
            dataObject.AddDataColumn(b_Org_EGRUL.DateFinishMNS);
            // Сведения о состоянии юридического лица
            dataObject.AddParamColumn(EGRULDataObject.ColumnOrgStatus, b_Org_Status.Name);
            dataObject.AddParamColumn(EGRULDataObject.ColumnOrgROData, b_Org_RegisterOrgan.Code);
            dataObject.AddParamColumn(EGRULDataObject.ColumnOrgROData, b_Org_RegisterOrgan.Name);
            dataObject.AddDataColumn(b_Org_EGRUL.DateStatus);
            // Сведения об адресе (месте нахождения) юридического лица
            dataObject.AddDataColumn(b_Org_EGRUL.DateAdress);
            dataObject.AddDataColumn(b_Org_EGRUL.NameIspOrg);
            dataObject.AddDataColumn(b_Org_EGRUL.VidAdr);
            dataObject.AddDataColumn(b_Org_EGRUL.Adress);
            dataObject.AddDataColumn(b_Org_EGRUL.OKATO);
            dataObject.AddDataColumn(b_Org_EGRUL.Contact);
            // Сведения об образовании юридического лица
            dataObject.AddParamColumn(EGRULDataObject.ColumnRegTypeStartData, b_Types_Register.Name);
            dataObject.AddDataColumn(b_Org_EGRUL.DateStartUL);
            dataObject.AddDataColumn(b_Org_EGRUL.RegNum);
            dataObject.AddParamColumn(EGRULDataObject.ColumnOrgROStartData, b_Org_RegisterOrgan.Code);
            dataObject.AddParamColumn(EGRULDataObject.ColumnOrgROStartData, b_Org_RegisterOrgan.Name);
            // Сведения о прекращении деятельности
            dataObject.AddParamColumn(EGRULDataObject.ColumnRegTypeFinishData, b_Types_Register.Name);
            dataObject.AddDataColumn(b_Org_EGRUL.DateFinishUL);
            dataObject.AddDataColumn(b_Org_EGRUL.RegNumFinish);
            dataObject.AddParamColumn(EGRULDataObject.ColumnOrgROFinishData, b_Org_RegisterOrgan.Code);
            dataObject.AddParamColumn(EGRULDataObject.ColumnOrgROFinishData, b_Org_RegisterOrgan.Name);
            // Регистрирующий орган, в котором находится регистрационное дело
            dataObject.AddParamColumn(EGRULDataObject.ColumnOrgROData, b_Org_RegisterOrgan.Code);
            dataObject.AddParamColumn(EGRULDataObject.ColumnOrgROData, b_Org_RegisterOrgan.Name);
            // Сведения о капитале
            dataObject.AddDataColumn(b_Org_EGRUL.DateCapital);
            dataObject.AddDataColumn(b_Org_EGRUL.SumCapital);
            dataObject.AddParamColumn(EGRULDataObject.ColumnOrgKindCapitalData, b_Org_KindCapital.Name);
            // Сведения об учредителях
            dataObject.AddDataColumn(b_Org_EGRUL.CNTUchrFL);
            // Сведения об учредителях - Российских ЮЛ
            dataObject.AddDataColumn(b_Org_EGRUL.CNTRUL);
            //Сведения об учредителях - иностранных ЮЛ
            dataObject.AddDataColumn(b_Org_EGRUL.CNTIUL);
            // Сведения о держателе реестра акционеров АО
            dataObject.AddDataColumn(b_Org_EGRUL.OGRNReestrAO);
            dataObject.AddDataColumn(b_Org_EGRUL.NameReestrAO);
            // Сведения об управляющей компании
            dataObject.AddDataColumn(b_Org_EGRUL.DateDirect);
            dataObject.AddDataColumn(b_Org_EGRUL.NameDirect);
            dataObject.AddDataColumn(b_Org_EGRUL.OGRNDirect);
            dataObject.AddDataColumn(b_Org_EGRUL.DateOGRNDirect);
            dataObject.AddDataColumn(b_Org_EGRUL.INNDirect);
            dataObject.AddDataColumn(b_Org_EGRUL.INN20Direct);
            dataObject.AddDataColumn(b_Org_EGRUL.AdressDirect);
            dataObject.AddDataColumn(b_Org_EGRUL.OKATODirect);
            dataObject.AddDataColumn(b_Org_EGRUL.AdressDirect);
            dataObject.AddDataColumn(b_Org_EGRUL.ContactDirect);
            // Сведения о регистрации в ПФ России
            dataObject.AddDataColumn(b_Org_EGRUL.RegNumPF);
            dataObject.AddDataColumn(b_Org_EGRUL.DateStartPF);
            dataObject.AddDataColumn(b_Org_EGRUL.DateFinishPF);
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCodeNameData, 
                b_Org_EGRUL.RefOrgPF, 
                b_Org_OrganPF.InternalKey, 
                String.Empty);
            // Сведения о регистрации в ФСС России
            dataObject.AddDataColumn(b_Org_EGRUL.RegNumFSS);
            dataObject.AddDataColumn(b_Org_EGRUL.DateStartFSS);
            dataObject.AddDataColumn(b_Org_EGRUL.DateFinishFSS);
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCodeNameData,
                b_Org_EGRUL.RefOrgFSS,
                b_Org_OrganFSS.InternalKey,
                String.Empty);
            // Сведения о регистрации в ФОМС России
            dataObject.AddDataColumn(b_Org_EGRUL.RegNumFOMS);
            dataObject.AddDataColumn(b_Org_EGRUL.DateStartFOMS);
            dataObject.AddDataColumn(b_Org_EGRUL.DateFinishFOMS);
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCodeNameData,
                b_Org_EGRUL.RefOrgFOMS,
                b_Org_OrganFOMS.InternalKey,
                String.Empty);
            DataTable tableData = dataObject.FillData();
            // 2. Таблицы, ссылющиеся на эту запись
            DataRow rowOrg = GetLastRow(tableData);
            if (rowOrg != null)
            {
                string refOrg = Convert.ToString(rowOrg[b_Org_EGRUL.ID]);
                // Сведения о видах экономической деятельности
                EGRULDataObject dataOrgKind = new EGRULDataObject();
                dataOrgKind.InitObject(scheme);
                dataOrgKind.useSummaryRow = false;
                dataOrgKind.ObjectKey = t_Org_OKVED.InternalKey;
                dataOrgKind.mainFilter[t_Org_OKVED.RefOrgEGRUL] = refOrg;
                dataOrgKind.AddParamColumn(EGRULDataObject.ColumnOKVEDMaskCode, t_Org_OKVED.Code);
                dataOrgKind.AddDataColumn(t_Org_OKVED.Sign);
                dataOrgKind.AddDataColumn(t_Org_OKVED.Name);
                dataOrgKind.AddDataColumn(t_Org_OKVED.Code);
                dataOrgKind.sortString = FormSortString(StrSortDown(t_Org_OKVED.Sign), StrSortUp(t_Org_OKVED.Code));
                tables[1] = dataOrgKind.FillData();
                // Сведения об учредителях
                EGRULDataObject dataFounder = new EGRULDataObject();
                dataFounder.InitObject(scheme);
                dataFounder.useSummaryRow = false;
                dataFounder.ObjectKey = t_Org_FounderFL.InternalKey;
                dataFounder.mainFilter[t_Org_FounderFL.RefOrgEGRUL] = refOrg;
                dataFounder.AddDataColumn(t_Org_FounderFL.FIO);
                dataFounder.AddDataColumn(t_Org_FounderFL.DateUchr);
                dataFounder.AddDataColumn(t_Org_FounderFL.INN);
                dataFounder.AddParamColumn(EGRULDataObject.ColumnDocumentDescr, t_Org_FounderFL.RefDocKindPers);
                dataFounder.AddDataColumn(t_Org_FounderFL.Summa);
                dataFounder.sortString = StrSortUp(t_Org_FounderFL.DateUchr);
                tables[2] = dataFounder.FillData();
                // Сведения об учредителях - Российских ЮЛ
                EGRULDataObject dataOrgRU = new EGRULDataObject();
                dataOrgRU.InitObject(scheme);
                dataOrgRU.useSummaryRow = false;
                dataOrgRU.ObjectKey = t_Org_FounderOrg.InternalKey;
                dataOrgRU.mainFilter[t_Org_FounderOrg.RefOrgEGRUL] = refOrg;
                dataOrgRU.AddDataColumn(t_Org_FounderOrg.NameP);
                dataOrgRU.AddDataColumn(t_Org_FounderOrg.DateUchr);
                dataOrgRU.AddDataColumn(t_Org_FounderOrg.OGRN);
                dataOrgRU.AddDataColumn(t_Org_FounderOrg.DateOGRN);
                dataOrgRU.AddDataColumn(t_Org_FounderOrg.INN);
                dataOrgRU.AddDataColumn(t_Org_FounderOrg.INN20);
                dataOrgRU.AddDataColumn(t_Org_FounderOrg.Adress);
                dataOrgRU.AddDataColumn(t_Org_FounderOrg.OKATO);
                dataOrgRU.AddDataColumn(t_Org_FounderOrg.RegNum);
                dataOrgRU.AddDataColumn(t_Org_FounderOrg.DateStartUL);
                dataOrgRU.AddParamColumn(EGRULDataObject.ColumnOrgROFullData);
                dataOrgRU.AddDataColumn(t_Org_FounderOrg.Summa);
                dataOrgRU.sortString = StrSortUp(t_Org_FounderOrg.DateUchr);
                tables[3] = dataOrgRU.FillData();
                // Сведения об учредителях - иностранных ЮЛ
                EGRULDataObject dataOrgForeign = new EGRULDataObject();
                dataOrgForeign.InitObject(scheme);
                dataOrgForeign.useSummaryRow = false;
                dataOrgForeign.ObjectKey = t_Org_FounderForeignOrg.InternalKey;
                dataOrgForeign.mainFilter[t_Org_FounderForeignOrg.RefOrgEGRUL] = refOrg;
                dataOrgForeign.AddDataColumn(t_Org_FounderForeignOrg.NameP);
                dataOrgForeign.AddDataColumn(t_Org_FounderForeignOrg.DateUchr);
                dataOrgForeign.AddDataColumn(t_Org_FounderForeignOrg.DateReg);
                dataOrgForeign.AddDataColumn(t_Org_FounderForeignOrg.Summa);
                dataOrgForeign.AddDataColumn(t_Org_FounderForeignOrg.Adress);
                dataOrgForeign.AddParamColumn(
                    EGRULDataObject.ColumnCommonBookData,
                    t_Org_FounderForeignOrg.RefOKSMBridge,
                    b_OKSM_Bridge.InternalKey,
                    b_OKSM_Bridge.Name);
                dataOrgForeign.sortString = StrSortUp(t_Org_FounderForeignOrg.DateUchr);
                tables[4] = dataOrgForeign.FillData();
                // Сведения о юр.лицах - предшественниках при реорганизации
                EGRULDataObject dataAncestor = new EGRULDataObject();
                dataAncestor.InitObject(scheme);
                dataAncestor.useSummaryRow = false;
                dataAncestor.ObjectKey = t_Organizations_Ancestor.InternalKey;
                dataAncestor.mainFilter[t_Organizations_Ancestor.RefOrgEGRUL] = refOrg;
                dataAncestor.AddDataColumn(t_Organizations_Ancestor.NameP);
                dataAncestor.AddDataColumn(t_Organizations_Ancestor.OGRN);
                dataAncestor.AddDataColumn(t_Organizations_Ancestor.DateOGRN);
                dataAncestor.AddDataColumn(t_Organizations_Ancestor.INN);
                dataAncestor.AddDataColumn(t_Organizations_Ancestor.INN20);
                dataAncestor.AddDataColumn(t_Organizations_Ancestor.RegNum);
                dataAncestor.AddDataColumn(t_Organizations_Ancestor.DateStartUL);
                dataAncestor.AddParamColumn(EGRULDataObject.ColumnOrgROData, b_Org_RegisterOrgan.Code);
                dataAncestor.AddDataColumn(t_Organizations_Ancestor.Adress);
                dataAncestor.AddDataColumn(t_Organizations_Ancestor.OKATO);
                dataAncestor.sortString = StrSortUp(t_Organizations_Ancestor.DateStartUL);
                tables[5] = dataAncestor.FillData();
                // Сведения о юр.лицах - преемниках при реорганизации
                EGRULDataObject dataAssign = new EGRULDataObject();
                dataAssign.InitObject(scheme);
                dataAssign.useSummaryRow = false;
                dataAssign.ObjectKey = t_Org_Assign.InternalKey;
                dataAssign.mainFilter[t_Org_Assign.RefOrgEGRUL] = refOrg;
                dataAssign.AddDataColumn(t_Org_Assign.NameP);
                dataAssign.AddDataColumn(t_Org_Assign.OGRN);
                dataAssign.AddDataColumn(t_Org_Assign.DateOGRN);
                dataAssign.AddDataColumn(t_Org_Assign.INN);
                dataAssign.AddDataColumn(t_Org_Assign.INN20);
                dataAssign.AddDataColumn(t_Org_Assign.RegNum);
                dataAssign.AddDataColumn(t_Org_Assign.DateStartUL);
                dataAssign.AddParamColumn(EGRULDataObject.ColumnOrgROData, b_Org_RegisterOrgan.Code);
                dataAssign.AddDataColumn(t_Org_Assign.Adress);
                dataAssign.AddDataColumn(t_Org_Assign.OKATO);
                dataAssign.sortString = StrSortUp(t_Org_Assign.DateStartUL);
                tables[6] = dataAssign.FillData();
                // Сведения о физ.лицах, имеющих право действовать без доверенности от имени юр.лица
                EGRULDataObject dataProxy = new EGRULDataObject();
                dataProxy.InitObject(scheme);
                dataProxy.useSummaryRow = false;
                dataProxy.ObjectKey = t_Org_FaceProxy.InternalKey;
                dataProxy.mainFilter[t_Org_FaceProxy.RefOrgEGRUL] = refOrg;
                dataProxy.AddDataColumn(t_Org_FaceProxy.FIO);
                dataProxy.AddDataColumn(t_Org_FaceProxy.DateZap);
                dataProxy.AddDataColumn(t_Org_FaceProxy.Job);
                dataProxy.AddDataColumn(t_Org_FaceProxy.INN);
                dataProxy.AddParamColumn(EGRULDataObject.ColumnDocumentDescr, t_Org_FaceProxy.RefDocKindPerson);
                dataProxy.sortString = StrSortUp(t_Org_FaceProxy.DateZap);
                tables[7] = dataProxy.FillData();
                // Сведения об обособленных подразделениях организации
                EGRULDataObject dataSubdivision = new EGRULDataObject();
                dataSubdivision.InitObject(scheme);
                dataSubdivision.useSummaryRow = false;
                dataSubdivision.ObjectKey = t_Org_Subdivision.InternalKey;
                dataSubdivision.mainFilter[t_Org_Subdivision.RefOrgEGRUL] = refOrg;
                dataSubdivision.AddDataColumn(t_Org_Subdivision.DateZap);
                dataSubdivision.AddDataColumn(t_Org_Subdivision.Adress);
                dataSubdivision.AddDataColumn(t_Org_Subdivision.OKATO);
                dataSubdivision.AddDataColumn(t_Org_Subdivision.Contact);
                dataSubdivision.AddParamColumn(
                    EGRULDataObject.ColumnCommonBookData,
                    t_Org_Subdivision.RefOrgSubdiv,
                    b_Org_KindSubdivision.InternalKey,
                    b_Org_KindSubdivision.Name);
                dataSubdivision.sortString = StrSortUp(t_Org_Subdivision.DateZap);
                tables[8] = dataSubdivision.FillData();
                // Сведения о лицензиях
                EGRULDataObject dataLicense = new EGRULDataObject();
                dataLicense.InitObject(scheme);
                dataLicense.useSummaryRow = false;
                dataLicense.ObjectKey = t_Org_Licence.InternalKey;
                dataLicense.mainFilter[t_Org_Licence.RefOrgEGRUL] = refOrg;
                dataLicense.AddDataColumn(t_Org_Licence.Nomer);
                dataLicense.AddParamColumn(
                    EGRULDataObject.ColumnCommonBookData,
                    t_Org_Licence.RefType,
                    b_Types_Licenz.InternalKey,
                    b_Types_Licenz.Name);
                dataLicense.AddDataColumn(t_Org_Licence.DateLicence);
                dataLicense.AddDataColumn(t_Org_Licence.DateStartLicence);
                dataLicense.AddDataColumn(t_Org_Licence.DateFinishLicence);
                dataLicense.AddDataColumn(t_Org_Licence.DateStopLic);
                dataLicense.AddDataColumn(t_Org_Licence.DateResume);
                dataLicense.AddParamColumn(
                    EGRULDataObject.ColumnCommonBookData,
                    t_Org_Licence.RefStateLicence,
                    b_State_Licence.InternalKey,
                    b_State_Licence.Name);
                dataLicense.AddParamColumn(
                    EGRULDataObject.ColumnCommonBookData,
                    t_Org_Licence.RefOrgLic,
                    b_Org_LicenceOrgan.InternalKey,
                    b_Org_LicenceOrgan.Name);
                dataLicense.sortString = StrSortUp(t_Org_Licence.DateLicence);
                tables[9] = dataLicense.FillData();
                // Сведения о счетах
                EGRULDataObject dataAccounts = new EGRULDataObject();
                dataAccounts.InitObject(scheme);
                dataAccounts.useSummaryRow = false;
                dataAccounts.ObjectKey = t_Org_Accounts.InternalKey;
                dataAccounts.mainFilter[t_Org_Accounts.RefOrgEGRUL] = refOrg;
                dataAccounts.AddDataColumn(t_Org_Accounts.NumAcc);
                dataAccounts.AddDataColumn(t_Org_Accounts.OGRN);
                dataAccounts.AddDataColumn(t_Org_Accounts.INN);
                dataAccounts.AddDataColumn(t_Org_Accounts.INN20);
                dataAccounts.AddDataColumn(t_Org_Accounts.BIK);
                dataAccounts.AddDataColumn(t_Org_Accounts.NumContract);
                dataAccounts.AddDataColumn(t_Org_Accounts.DateStartContr);
                dataAccounts.AddDataColumn(t_Org_Accounts.DateEndContr);
                dataAccounts.AddParamColumn(
                    EGRULDataObject.ColumnCommonBookData,
                    t_Org_Accounts.RefTypeAcc,
                    b_Types_BankAcc.InternalKey,
                    b_Types_BankAcc.Name);
                dataAccounts.AddParamColumn(
                    EGRULDataObject.ColumnCommonBookData,
                    t_Org_Accounts.RefKindAcc,
                    b_Kind_BankAcc.InternalKey,
                    b_Kind_BankAcc.Name);
                dataAccounts.sortString = StrSortUp(t_Org_Accounts.DateStartContr);
                tables[10] = dataAccounts.FillData();
                // Сведения о записях в ЕГРЮЛ
                EGRULDataObject dataNotes = new EGRULDataObject();
                dataNotes.InitObject(scheme);
                dataNotes.useSummaryRow = false;
                dataNotes.ObjectKey = t_Org_Note.InternalKey;
                dataNotes.mainFilter[t_Org_Note.RefOrgEGRUL] = refOrg;
                dataNotes.AddDataColumn(t_Org_Note.DateReg, ReportConsts.ftDateTime);
                dataNotes.AddDataColumn(t_Org_Note.NumberReg);
                dataNotes.AddDataColumn(t_Org_Note.DateNote);
                dataNotes.AddDataColumn(t_Org_Note.RegID);
                dataNotes.AddParamColumn(EGRULDataObject.ColumnOrgROFullData, b_Org_RegisterOrgan.Name);
                dataNotes.AddParamColumn(
                    EGRULDataObject.ColumnCommonBookData,
                    t_Org_Note.RefTypeReg,
                    b_Types_Register.InternalKey,
                    b_Types_Register.Name);
                dataNotes.AddDataColumn(t_Org_Note.Certificate);
                dataNotes.AddParamColumn(
                    EGRULDataObject.ColumnCommonBookData,
                    t_Org_Note.RefStateReg,
                    b_State_RegisterNote.InternalKey,
                    b_State_RegisterNote.Name);
                dataNotes.sortString = StrSortUp(t_Org_Note.DateReg);
                tables[11] = dataNotes.FillData();
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

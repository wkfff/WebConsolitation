using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFK;
using Krista.FM.Client.Reports.Database.ClsBridge.EGRUL;
using Krista.FM.Client.Reports.Database.ClsBridge;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 0003_ЕГРЮЛ_ВЫБОР ОРГАНИЗАЦИЙ ПО ПОКАЗАТЕЛЯМ 
        /// </summary>
        public DataTable[] GetEGRUL0003ReportData(Dictionary<string, string> reportParams)
        {
            DataTable[] tables = new DataTable[2];
            EGRULDataObject dataObject = new EGRULDataObject();
            dataObject.InitObject(scheme);
            dataObject.useSummaryRow = false;
            dataObject.externalFilter = reportParams[ReportConsts.ParamMasterFilter];
            // 00 - ОГРН
            dataObject.AddDataColumn(b_Org_EGRUL.OGRN);
            // 01 - ИНН
            dataObject.AddDataColumn(b_Org_EGRUL.INN);
            // 02 - КПП
            dataObject.AddDataColumn(b_Org_EGRUL.INN20);
            // 03 - Полное наименование
            dataObject.AddDataColumn(b_Org_EGRUL.NameP);
            // 04 - Сокращенное наименование
            dataObject.AddDataColumn(b_Org_EGRUL.ShortName);
            // 05 - Фирменное наименование ЮЛ
            dataObject.AddDataColumn(b_Org_EGRUL.FirmName);
            // 06 - Наименование ЮЛ на национальном языке народов РФ
            dataObject.AddDataColumn(b_Org_EGRUL.NationalName);
            // 07 - Национальный язык
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_Org_EGRUL.RefNatLang,
                b_OKIN_Language.InternalKey,
                b_OKIN_Language.Name);
            // 08 - Наименование ЮЛ на иностранном языке
            dataObject.AddDataColumn(b_Org_EGRUL.ForeignName);
            // 09 - Иностранный язык
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_Org_EGRUL.RefForLang,
                b_OKIN_Language.InternalKey,
                b_OKIN_Language.Name);
            // 10 - Дата внесения записи о наименовании
            dataObject.AddDataColumn(b_Org_EGRUL.DateName);
            // 11 - ОКОПФ
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_Org_EGRUL.RefOKOPFBridge,
                b_OKOPF_Bridge.InternalKey,
                Combine(b_OKOPF_Bridge.Code, b_OKOPF_Bridge.Name));
            // 12 - КОПФ
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_Org_EGRUL.RefKOPFBridge,
                b_KOPF_Bridge.InternalKey,
                 Combine(b_KOPF_Bridge.Code, b_KOPF_Bridge.Name));
            // 13 - Адрес
            dataObject.AddDataColumn(b_Org_EGRUL.Adress);
            // 14 - Дата записи об адресе
            dataObject.AddDataColumn(b_Org_EGRUL.DateAdress);
            // 15 - Наименование органа адресата
            dataObject.AddDataColumn(b_Org_EGRUL.NameIspOrg);
            // 16 - Вид адреса
            dataObject.AddDataColumn(b_Org_EGRUL.VidAdr);
            // 17 - ОКАТО
            dataObject.AddDataColumn(b_Org_EGRUL.OKATO);
            // 18 - Контактная информация
            dataObject.AddDataColumn(b_Org_EGRUL.Contact);
            // 19 - Основной ОКВЭД
            dataObject.AddParamColumn(EGRULDataObject.ColumnOKVEDMaskCode, b_Org_EGRUL.MainOKVED);
            // 20 - Регистрирующий орган, в котором находится регистрационное дело
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_Org_EGRUL.RefOrgRO,
                b_Org_RegisterOrgan.InternalKey,
                Combine(b_Org_RegisterOrgan.Code, b_Org_RegisterOrgan.Name));
            // 21 - Статус организации
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_Org_EGRUL.RefOrgStatus,
                b_Org_Status.InternalKey,
                b_Org_Status.Name);
            // 22 - Дата присвоения статуса
            dataObject.AddDataColumn(b_Org_EGRUL.DateStatus);
            // 23 - Вид капитала
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_Org_EGRUL.RefOrgKindCapital,
                b_Org_KindCapital.InternalKey,
                b_Org_KindCapital.Name);
            // 24 - Размер капитала (в рублях)
            dataObject.AddDataColumn(b_Org_EGRUL.SumCapital);
            // 25 - Дата записи о капитале
            dataObject.AddDataColumn(b_Org_EGRUL.DateCapital);
            // 26 - Cпособ образования ЮЛ (вид регистрации)
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_Org_EGRUL.RefTypeRegStart,
                b_Types_Register.InternalKey,
                b_Types_Register.Name);
            // 27 - Дата создания ЮЛ
            dataObject.AddDataColumn(b_Org_EGRUL.DateStartUL);
            // 28 - Регистрационный номер создания ЮЛ
            dataObject.AddDataColumn(b_Org_EGRUL.RegNum);
            // 29 - Орган зарегистрировавший создание ЮЛ
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_Org_EGRUL.RefOrgROStart,
                b_Org_RegisterOrgan.InternalKey,
                Combine(b_Org_RegisterOrgan.Code, b_Org_RegisterOrgan.Name));
            // 30 - Способ прекращения деятельности ЮЛ (вид регистрации)
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_Org_EGRUL.RefTypeRegFinish,
                b_Types_Register.InternalKey,
                b_Types_Register.Name);
            // 31 - Дата регистрации
            dataObject.AddDataColumn(b_Org_EGRUL.DateFinishUL);
            // 32 - Регистрационный номер ликвидации ЮЛ
            dataObject.AddDataColumn(b_Org_EGRUL.RegNumFinish);
            // 33 - Орган зарегистрировавший ликвидацию ЮЛ
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_Org_EGRUL.RefOrgROFinish,
                b_Org_RegisterOrgan.InternalKey,
                Combine(b_Org_RegisterOrgan.Code, b_Org_RegisterOrgan.Name));
            // 34 - Количество учредителей - физических лиц
            dataObject.AddDataColumn(b_Org_EGRUL.CNTUchrFL);
            // 35 - Количество учредителей  - российских юридических лиц
            dataObject.AddDataColumn(b_Org_EGRUL.CNTRUL);
            // 36 - Количество учредителей - иностранных юридических лиц
            dataObject.AddDataColumn(b_Org_EGRUL.CNTIUL);
            // 37 - Наименование держателя реестра акционеров АО
            dataObject.AddDataColumn(b_Org_EGRUL.NameReestrAO);
            // 38 - Наименование юридического лица, являющегося держателем реестра акционеров АО
            dataObject.AddDataColumn(b_Org_EGRUL.OGRNReestrAO);
            // 39 - Наименование управляющей компании
            dataObject.AddDataColumn(b_Org_EGRUL.NameDirect);
            // 40 - Дата записи об управляющей компании
            dataObject.AddDataColumn(b_Org_EGRUL.DateDirect);
            // 41 - ОГРН управляющей компании
            dataObject.AddDataColumn(b_Org_EGRUL.OGRNDirect);
            // 42 - Дата присвоения ОГРН управляющей компании
            dataObject.AddDataColumn(b_Org_EGRUL.DateOGRNDirect);
            // 43 - ИНН управляющей компании
            dataObject.AddDataColumn(b_Org_EGRUL.INNDirect);
            // 44 - КПП управляющей компании
            dataObject.AddDataColumn(b_Org_EGRUL.INN20Direct);
            // 45 - Адрес управляющей компании
            dataObject.AddDataColumn(b_Org_EGRUL.AdressDirect);
            // 46 - ОКАТО управляющей компании
            dataObject.AddDataColumn(b_Org_EGRUL.OKATODirect);
            // 47 - Контактная информация управляющей компании
            dataObject.AddDataColumn(b_Org_EGRUL.ContactDirect);
            // 48 - Дата постановки на учет в налоговом органе
            dataObject.AddDataColumn(b_Org_EGRUL.DateStartMNS);
            // 49 - Налоговый орган
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_Org_EGRUL.RefOrgMNS,
                b_Org_MNS.InternalKey,
                Combine(b_Org_MNS.Code, b_Org_MNS.Name));
            // 50 - Дата снятия с учета в налоговом органе
            dataObject.AddDataColumn(b_Org_EGRUL.DateFinishMNS);
            // 51 - Дата постановки на учет в ПФ
            dataObject.AddDataColumn(b_Org_EGRUL.DateStartPF);
            // 52 - Регистрационный номер в ПФ
            dataObject.AddDataColumn(b_Org_EGRUL.RegNumPF);
            // 53 - Орган ПФ
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_Org_EGRUL.RefOrgPF,
                b_Org_OrganPF.InternalKey,
                Combine(b_Org_OrganPF.Code, b_Org_OrganPF.Name));
            // 54 - Дата снятия с учета в ПФ
            dataObject.AddDataColumn(b_Org_EGRUL.DateFinishPF);
            // 55 - Дата постановки на учет в ФСС
            dataObject.AddDataColumn(b_Org_EGRUL.DateStartFSS);
            // 56 - Регистрационный номер в ФСС
            dataObject.AddDataColumn(b_Org_EGRUL.RegNumFSS);
            // 57 - Орган ФСС
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_Org_EGRUL.RefOrgFSS,
                b_Org_OrganFSS.InternalKey,
                Combine(b_Org_OrganFSS.Code, b_Org_OrganFSS.Name));
            // 58 - Дата снятия с учета в ФСС
            dataObject.AddDataColumn(b_Org_EGRUL.DateFinishFSS);
            // 59 - Дата постановки на учет в ФОМС
            dataObject.AddDataColumn(b_Org_EGRUL.DateStartFOMS);
            // 60 - Регистрационный номер в ФОМС
            dataObject.AddDataColumn(b_Org_EGRUL.RegNumFOMS);
            // 61 - Орган ФОМС
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_Org_EGRUL.RefOrgFOMS,
                b_Org_OrganFOMS.InternalKey,
                Combine(b_Org_OrganFOMS.Code, b_Org_OrganFOMS.Name));
            // 62 - Дата снятия с учета в ФОМС
            dataObject.AddDataColumn(b_Org_EGRUL.DateFinishFOMS);
            tables[0] = dataObject.FillData();

            DataRow rowCaption = CreateReportParamsRow(tables);
            rowCaption[0] = DateTime.Now.ToShortDateString();
            rowCaption[1] = reportParams[ReportConsts.ParamReportList];
            return tables;
        }
    }
}

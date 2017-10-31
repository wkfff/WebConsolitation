using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.UFK;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.Database.ClsBridge.EGRUL;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 0002_ЕГРИП_ВЫБОР ОРГАНИЗАЦИЙ ПО ПОКАЗАТЕЛЯМ 
        /// </summary>
        public DataTable[] GetEGRIP0002ReportData(Dictionary<string, string> reportParams)
        {
            DataTable[] tables = new DataTable[2];
            EGRIPDataObject dataObject = new EGRIPDataObject();
            dataObject.InitObject(scheme);
            dataObject.useSummaryRow = false;
            dataObject.externalFilter = reportParams[ReportConsts.ParamMasterFilter];
            // 00 - ОГРНИП
            dataObject.AddDataColumn(b_IP_EGRIP.OGRNIP);
            // 01 - ИНН
            dataObject.AddDataColumn(b_IP_EGRIP.INN);
            // 02 - Фамилия Имя Отчество
            dataObject.AddDataColumn(b_IP_EGRIP.FIO);
            // 03 - Дата записи о ФИО
            dataObject.AddDataColumn(b_IP_EGRIP.DtStartFL);
            // 04 - ФИО латинскими буквами
            dataObject.AddDataColumn(b_IP_EGRIP.FIOLat);
            // 05 - Пол
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefSex,
                fx_FX_Sex.InternalKey,
                fx_FX_Sex.Name);
            // 06 - Дата записи об адресе
            dataObject.AddDataColumn(b_IP_EGRIP.DtStartAddr);
            // 07 - Адрес
            dataObject.AddDataColumn(b_IP_EGRIP.Adress);
            // 08 - ОКАТО
            dataObject.AddDataColumn(b_IP_EGRIP.OKATO);
            // 09 - Контактная информация 
            dataObject.AddDataColumn(b_IP_EGRIP.Contact);
            // 10 - Вид деятельности
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefIPKindFunction,
                b_IP_KindFunction.InternalKey,
                b_IP_KindFunction.Name);
            // 11 - Сведения о статусе
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefIPStatus,
                b_IP_Status.InternalKey,
                b_IP_Status.Name);
            // 12 - Регистрирующий орган
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefOrgRO,
                b_Org_RegisterOrgan.InternalKey,
                Combine(b_Org_RegisterOrgan.Code, b_Org_RegisterOrgan.Name));
            // 13 - Сведения о старой регистрации
            dataObject.AddDataColumn(b_IP_EGRIP.OldReg);
            // 14 - Основной ОКВЭД
            dataObject.AddParamColumn(EGRULDataObject.ColumnOKVEDMaskCode, b_IP_EGRIP.MainOKVED);
            // 15 - Дата записи о документе удостоверяющем личность
            dataObject.AddDataColumn(b_IP_EGRIP.DtStartDocFL);
            // 16 - Документ удостоверяющий личность
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefDocKindPerson,
                b_Doc_KindPerson.InternalKey,
                b_Doc_KindPerson.Name);
            // 17 - Серия и номер документа
            dataObject.AddDataColumn(b_IP_EGRIP.DocNum);
            // 18 - Дата выдачи документа
            dataObject.AddDataColumn(b_IP_EGRIP.DateDoc);
            // 19 - Орган выдавший документ 
            dataObject.AddDataColumn(b_IP_EGRIP.NameOrg);
            // 20 - Код подразделения выдавшего документ 
            dataObject.AddDataColumn(b_IP_EGRIP.KodOrg);
            // 21 - Дата записи о документе удостоверяющем личность
            dataObject.AddDataColumn(b_IP_EGRIP.DtStartCitizen);
            // 22 - Вид гражданства
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefOKCitizen,
                b_OK_Citizen.InternalKey,
                b_OK_Citizen.Name);
            // 23 - Страна гражданства
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefOKSMBridge,
                b_OKSM_Bridge.InternalKey,
                b_OKSM_Bridge.Name);
            // 24 - Дата записи о документе подтверждающем право проживания в РФ
            dataObject.AddDataColumn(b_IP_EGRIP.DtStartDocHabitat);
            // 25 - Документ дающий право на проживание в РФ
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefDocHabitatRF,
                b_Doc_HabitatRF.InternalKey,
                b_Doc_HabitatRF.Name);
            // 26 - Номер документа 
            dataObject.AddDataColumn(b_IP_EGRIP.HabitatDocNum);
            // 27 - Дата выдачи документа 
            dataObject.AddDataColumn(b_IP_EGRIP.HabitatDocDate);
            // 28 - Орган зарегистрировавший создание ЮЛ
            dataObject.AddDataColumn(b_IP_EGRIP.HabitatNameOrg);
            // 29 - Дата окончания действия документа
            dataObject.AddDataColumn(b_IP_EGRIP.HabitatDocDateEnd);
            // 30 - Дата внесения записи о дееспособности 
            dataObject.AddDataColumn(b_IP_EGRIP.DtStartAbility);
            // 31 - Документ подтверждающий приобретение дееспособности
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefDocRightAbility,
                b_Doc_RightAbility.InternalKey,
                b_Doc_RightAbility.Name);
            // 32 - Номер документа 
            dataObject.AddDataColumn(b_IP_EGRIP.AbilityDocNum);
            // 33 - Орган выдавший документ 
            dataObject.AddDataColumn(b_IP_EGRIP.AbilityNameOrg);
            // 34 - Дата документа 
            dataObject.AddDataColumn(b_IP_EGRIP.AbilityDocDate);
            // 35 - Документ основание приобретения дееспособности
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefDocBaseAbility,
                b_Doc_BaseAbility.InternalKey,
                b_Doc_BaseAbility.Name);
            // 36 - Дата постановки на учет в налоговом органе
            dataObject.AddDataColumn(b_IP_EGRIP.DateStartMNS);
            // 37 - Дата снятия с учета
            dataObject.AddDataColumn(b_IP_EGRIP.DateFinishMNS);
            // 38 - Наименование налогового органа
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefOrgMNS,
                b_Org_MNS.InternalKey,
                Combine(b_Org_MNS.Code, b_Org_MNS.Name));
            // 39 - Регномер ПФ
            dataObject.AddDataColumn(b_IP_EGRIP.RegNumPF);
            // 40 - Дата регистрации
            dataObject.AddDataColumn(b_IP_EGRIP.DateStartPF);
            // 41 - Дата снятия с учета
            dataObject.AddDataColumn(b_IP_EGRIP.DateFinishPF);
            // 42 - Наименование территориального органа ПФ
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefOrgPF,
                b_Org_OrganPF.InternalKey,
                Combine(b_Org_OrganPF.Code, b_Org_OrganPF.Name));
            // 43 - Регномер ФСС
            dataObject.AddDataColumn(b_IP_EGRIP.RegNumFSS);
            // 44 - Дата регистрации
            dataObject.AddDataColumn(b_IP_EGRIP.DateStartFSS);
            // 45 - Дата снятия с учета
            dataObject.AddDataColumn(b_IP_EGRIP.DateFinishFSS);
            // 46 - Наименование исполнительного органа ФСС
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefOrgFSS,
                b_Org_OrganFSS.InternalKey,
                Combine(b_Org_OrganFSS.Code, b_Org_OrganFSS.Name));
            // 47 - Регномер ФОМС
            dataObject.AddDataColumn(b_IP_EGRIP.RegNumFOMS);
            // 48 - Дата регистрации
            dataObject.AddDataColumn(b_IP_EGRIP.DateStartFOMS);
            // 49 - Дата снятия с учета
            dataObject.AddDataColumn(b_IP_EGRIP.DateFinishFOMS);
            // 50 - Наименование территориального ФОМС
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                b_IP_EGRIP.RefOrgFOMS,
                b_Org_OrganFOMS.InternalKey,
                Combine(b_Org_OrganFOMS.Code, b_Org_OrganFOMS.Name));

            tables[0] = dataObject.FillData();

            DataRow rowCaption = CreateReportParamsRow(tables);
            rowCaption[0] = DateTime.Now.ToShortDateString();
            rowCaption[1] = reportParams[ReportConsts.ParamReportList];
            return tables;
        }
    }
}

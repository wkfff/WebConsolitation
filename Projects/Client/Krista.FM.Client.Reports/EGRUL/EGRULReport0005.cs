using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.Database.FactTables.EGRUL;
using Krista.FM.Client.Reports.Database.ClsBridge.EGRUL;
using Krista.FM.Client.Reports.Database.ClsData.EGRUL;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Common;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        private void SetEGRUL0005AncestorFilter(
            CommonDataObject cdo,
            Dictionary<string, string> reportParams, 
            bool isMasterTable = false)
        {
            var fieldLast = b_Org_EGRUL.Last;
            var fieldOKOPF = b_Org_EGRUL.RefOKOPFBridge;
            var fieldKOPF = b_Org_EGRUL.RefKOPFBridge;
            var fieldRegion = b_Org_EGRUL.RefRegion;
            var fieldINN = b_Org_EGRUL.INN;
            var fieldOrg = b_Org_EGRUL.ID;
            var fieldOkato = b_Org_EGRUL.OKATO;

            if (!isMasterTable)
            {
                fieldLast = GetExternalFieldName(fieldLast, 1);
                fieldOKOPF = GetExternalFieldName(fieldOKOPF, 1);
                fieldKOPF = GetExternalFieldName(fieldKOPF, 1);
                fieldRegion = GetExternalFieldName(fieldRegion, 1);
                fieldINN = GetExternalFieldName(fieldINN, 1);
                fieldOkato = GetExternalFieldName(fieldOkato, 1);
                fieldOrg = GetExternalFieldName(fieldOrg, 1);
            }
            else
            {
                fieldOrg = GetInternalFieldName(fieldOrg);
            }

            var hasOKOPFFilter = reportParams[ReportConsts.ParamOKOPF] != ReportConsts.UndefinedKey;
            var hasKOPFFilter = reportParams[ReportConsts.ParamKOPF] != ReportConsts.UndefinedKey;
            var hasINNFilter = reportParams[ReportConsts.ParamINN] != ReportConsts.UndefinedKey;
            var hasRegionFilter = reportParams[ReportConsts.ParamRegionComparable] != ReportConsts.UndefinedKey;
            var hasOrgFilter = reportParams[ReportConsts.ParamOrgName] != ReportConsts.UndefinedKey;
            var hasOkatoFilter = reportParams[ReportConsts.ParamOKATO] != ReportConsts.UndefinedKey;

            if (hasOKOPFFilter)
            {
                cdo.mainFilter[fieldOKOPF] = reportParams[ReportConsts.ParamOKOPF];
            }

            if (hasKOPFFilter)
            {
                cdo.mainFilter[fieldKOPF] = reportParams[ReportConsts.ParamKOPF];
            }

            if (hasOKOPFFilter && hasKOPFFilter)
            {
                cdo.optionalFilters.Add(fieldOKOPF);
                cdo.optionalFilters.Add(fieldKOPF);
            }

            if (hasINNFilter)
            {
                cdo.mainFilter[fieldINN] = reportParams[ReportConsts.ParamINN];
            }

            if (hasOkatoFilter)
            {
                cdo.mainFilter[fieldOkato] = reportParams[ReportConsts.ParamOKATO];
            }

            if (hasRegionFilter)
            {
                cdo.mainFilter[fieldRegion] = reportParams[ReportConsts.ParamRegionComparable];
            }

            if (hasOrgFilter)
            {
                cdo.fixedFilter = String.Format("{0} in (select temp.{1} from {2} temp where temp.{3} in ({4}))",
                    fieldOrg, 
                    t_Org_FounderOrg.RefOrgEGRUL, 
                    ConvertorSchemeLink.GetEntity(t_Org_FounderOrg.InternalKey).FullDBName,
                    t_Org_FounderOrg.ID, 
                    reportParams[ReportConsts.ParamOrgName]);
            }

            cdo.mainFilter[fieldLast] = "1";
        }

        /// <summary>
        /// ОТЧЕТ 0005_ВЫБОР ОРГАНИЗАЦИЙ ПО КОДУ КЛАДР СУБЪЕКТА И ОКОПФ
        /// </summary>
        public DataTable[] GetEGRUL0005ReportData(Dictionary<string, string> reportParams)
        {
            var tables = new DataTable[2];

            var subdivFieldList = new[]
                                      {
                                          d_Org_Adress.Indeks, 
                                          d_Org_Adress.KodKLRegion,
                                          d_Org_Adress.NameRegion,
                                          d_Org_Adress.KodKLRaion,
                                          d_Org_Adress.NameRaion,
                                          d_Org_Adress.KodKLGorod,
                                          d_Org_Adress.NameGorod,
                                          d_Org_Adress.KodKLNasPunkt,
                                          d_Org_Adress.NameNasPunkt,
                                          d_Org_Adress.KodStreet,
                                          d_Org_Adress.NameStreet
                                      };

            var addrFieldList = String.Join(",", subdivFieldList).Trim(',');

            // Сведения о руководителе
            var dataFaces = new EGRULDetailTest();
            dataFaces.InitObject(scheme);
            dataFaces.useSummaryRow = false;
            dataFaces.clearQueryFields = true;
            dataFaces.ObjectKey = t_Org_FaceProxy.InternalKey;
            SetEGRUL0005AncestorFilter(dataFaces, reportParams);
            dataFaces.AddDataColumn(t_Org_FaceProxy.FIO);
            dataFaces.AddDataColumn(t_Org_FaceProxy.Job);
            dataFaces.AddDataColumn(t_Org_FaceProxy.RefOrgEGRUL);
            dataFaces.AddDataColumn(t_Org_FaceProxy.ID);
            dataFaces.sortString = StrSortUp(t_Org_FaceProxy.FIO);
            var tblFaces = dataFaces.FillData();
            // Сведения об учредителях - Российских ЮЛ
            var dataOrgRU = new EGRULDetailTest();
            dataOrgRU.InitObject(scheme);
            dataOrgRU.useSummaryRow = false;
            dataOrgRU.ObjectKey = t_Org_FounderOrg.InternalKey;
            SetEGRUL0005AncestorFilter(dataOrgRU, reportParams);
            dataOrgRU.mainFilter[GetInternalFieldName(t_Org_FounderOrg.Last)] = "1";
            dataOrgRU.AddDataColumn(t_Org_FounderOrg.NameP);
            dataOrgRU.AddDataColumn(t_Org_FounderOrg.OGRN);
            dataOrgRU.AddDataColumn(t_Org_FounderOrg.INN);
            dataOrgRU.AddDataColumn(t_Org_FounderOrg.INN20);
            dataOrgRU.AddParamColumn(EGRULDataObject.ColumnOKATOMask);
            dataOrgRU.AddParamColumn(EGRULDataObject.ColumnFieldSequence, addrFieldList, "2", String.Empty);
            dataOrgRU.AddCalcColumn(CalcColumnType.cctUndefined);
            dataOrgRU.AddCalcColumn(CalcColumnType.cctUndefined);
            dataOrgRU.AddCalcColumn(CalcColumnType.cctUndefined);
            dataOrgRU.AddCalcColumn(CalcColumnType.cctUndefined);
            dataOrgRU.AddCalcColumn(CalcColumnType.cctUndefined);
            dataOrgRU.AddCalcColumn(CalcColumnType.cctUndefined);
            dataOrgRU.AddCalcColumn(CalcColumnType.cctUndefined);
            dataOrgRU.AddCalcColumn(CalcColumnType.cctUndefined);
            dataOrgRU.AddCalcColumn(CalcColumnType.cctUndefined);
            dataOrgRU.AddCalcColumn(CalcColumnType.cctUndefined);
            dataOrgRU.AddParamColumn(EGRULDataObject.ColumnAddressPart, "1");
            dataOrgRU.AddParamColumn(EGRULDataObject.ColumnAddressPart, "2");
            dataOrgRU.AddParamColumn(EGRULDataObject.ColumnAddressPart, "3");
            dataOrgRU.AddDataColumn(t_Org_OKVED.RefOrgEGRUL);
            var tblOrgRu = dataOrgRU.FillData();
            // Сведения о видах экономической деятельности
            var dataOrgKind = new EGRULDetailTest();
            dataOrgKind.InitObject(scheme);
            dataOrgKind.useSummaryRow = false;
            dataOrgKind.clearQueryFields = true;
            dataOrgKind.ObjectKey = t_Org_OKVED.InternalKey;
            SetEGRUL0005AncestorFilter(dataOrgKind, reportParams);
            dataOrgKind.mainFilter[t_Org_OKVED.Sign] = "='Дополнительный вид экономической деятельности'";
            dataOrgKind.AddParamColumn(EGRULDataObject.ColumnOKVEDMaskCode, t_Org_OKVED.Code);
            dataOrgKind.AddDataColumn(t_Org_OKVED.RefOrgEGRUL);
            dataOrgKind.AddDataColumn(t_Org_OKVED.Code);
            dataOrgKind.AddDataColumn(t_Org_OKVED.ID);
            dataOrgKind.sortString = StrSortUp(t_Org_OKVED.Code);
            var tblActivityKind = dataOrgKind.FillData();

            // Сведения об обособленных подразделениях организации
            var dataSubdivision = new EGRULDetailTest();
            dataSubdivision.InitObject(scheme);
            dataSubdivision.useSummaryRow = false;
            dataSubdivision.ObjectKey = t_Org_Subdivision.InternalKey;
            SetEGRUL0005AncestorFilter(dataSubdivision, reportParams);
            dataSubdivision.mainFilter[GetInternalFieldName(t_Org_Subdivision.Last)] = "1";
            dataSubdivision.AddParamColumn(
                EGRULDataObject.ColumnCommonBookData,
                t_Org_Subdivision.RefOrgSubdiv,
                b_Org_KindSubdivision.InternalKey,
                Combine(b_Org_KindSubdivision.Code, b_Org_KindSubdivision.Name));

            dataSubdivision.AddParamColumn(EGRULDataObject.ColumnOKATOMask);
            dataSubdivision.AddParamColumn(EGRULDataObject.ColumnFieldSequence, addrFieldList, "2", String.Empty);
            dataSubdivision.AddCalcColumn(CalcColumnType.cctUndefined);
            dataSubdivision.AddCalcColumn(CalcColumnType.cctUndefined);
            dataSubdivision.AddCalcColumn(CalcColumnType.cctUndefined);
            dataSubdivision.AddCalcColumn(CalcColumnType.cctUndefined);
            dataSubdivision.AddCalcColumn(CalcColumnType.cctUndefined);
            dataSubdivision.AddCalcColumn(CalcColumnType.cctUndefined);
            dataSubdivision.AddCalcColumn(CalcColumnType.cctUndefined);
            dataSubdivision.AddCalcColumn(CalcColumnType.cctUndefined);
            dataSubdivision.AddCalcColumn(CalcColumnType.cctUndefined);
            dataSubdivision.AddCalcColumn(CalcColumnType.cctUndefined);
            dataSubdivision.AddParamColumn(EGRULDataObject.ColumnAddressPart, "1");
            dataSubdivision.AddParamColumn(EGRULDataObject.ColumnAddressPart, "2");
            dataSubdivision.AddParamColumn(EGRULDataObject.ColumnAddressPart, "3");
            dataSubdivision.AddParamColumn(EGRULDataObject.ColumnCutContactInfo, t_Org_Subdivision.Contact);
            dataSubdivision.AddDataColumn(t_Org_Subdivision.RefOrgEGRUL);
            var tblSubDivision = dataSubdivision.FillData();

            var dataObject = new EGRULClsJoinDataObject();
            dataObject.InitObject(scheme);
            dataObject.cacheData.Add(dataSubdivision.ObjectKey, tblSubDivision);
            dataObject.cacheData.Add(dataOrgRU.ObjectKey, tblOrgRu);
            dataObject.cacheData.Add(dataFaces.ObjectKey, tblFaces);
            dataObject.cacheData.Add(dataOrgKind.ObjectKey, tblActivityKind);
            dataObject.useSummaryRow = false;
            SetEGRUL0005AncestorFilter(dataObject, reportParams, true);
            // 00 - позиция
            dataObject.AddCalcColumn(CalcColumnType.cctPosition);
            // 01 - ИНН
            dataObject.AddDataColumn(b_Org_EGRUL.INN);
            // 02 - КПП
            dataObject.AddDataColumn(b_Org_EGRUL.INN20);
            // 03 - ОГРН
            dataObject.AddDataColumn(b_Org_EGRUL.OGRN);
            // 04 - Полное наименование
            dataObject.AddDataColumn(b_Org_EGRUL.NameP);
            // 05 - Сокращенное наименование
            dataObject.AddDataColumn(b_Org_EGRUL.ShortName);
            // 06 - Код ОКОПФ
            dataObject.AddParamColumn(EGRULDataObject.ColumnKOPFData, b_OKOPF_Bridge.Code);
            // 07 - Наименование ОКОПФ
            dataObject.AddParamColumn(EGRULDataObject.ColumnKOPFData, b_OKOPF_Bridge.Name);
            // 08 - Дата создания ЮЛ
            dataObject.AddDataColumn(b_Org_EGRUL.DateStartUL);
            // 09 данные об учредителях
            dataObject.AddParamColumn(EGRULDataObject.ColumnDetailSequence, dataOrgRU.ObjectKey, "19", "0");
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            // 28 - ОКАТО
            dataObject.AddParamColumn(EGRULDataObject.ColumnOKATOMask);
            // 29-39 - Адресные поля
            dataObject.AddParamColumn(EGRULDataObject.ColumnFieldSequence, addrFieldList, "1", String.Empty);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            // 40 - Номер дома (владения)
            dataObject.AddParamColumn(EGRULDataObject.ColumnAddressPart, "1");
            // 41 - Корпус (строение)
            dataObject.AddParamColumn(EGRULDataObject.ColumnAddressPart, "2");
            // 42 - Квартира (офис)
            dataObject.AddParamColumn(EGRULDataObject.ColumnAddressPart, "3");
            // 43 - Контактный телефон
            dataObject.AddParamColumn(EGRULDataObject.ColumnCutContactInfo, b_Org_EGRUL.Contact);
            // 44 - 
            dataObject.AddParamColumn(EGRULDataObject.ColumnDetailSequence, dataFaces.ObjectKey, "2", "0");
            // 45 - 
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            // 46 - ОКВЭД основного вида деятельности
            dataObject.AddParamColumn(EGRULDataObject.ColumnOKVEDMaskCode, b_Org_EGRUL.MainOKVED);
            // 47 - ОКВЭД доп. вида деятельности
            dataObject.AddParamColumn(
                EGRULDataObject.ColumnDetailText, 
                dataOrgKind.ObjectKey, 
                t_Org_OKVED.Code, 
                "; ");
            // 48 обособленные подразделения
            dataObject.AddParamColumn(EGRULDataObject.ColumnDetailSequence, dataSubdivision.ObjectKey, "17", "0");
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            dataObject.AddCalcColumn(CalcColumnType.cctUndefined);
            tables[0] = dataObject.FillData();

            var rowCaption = CreateReportParamsRow(tables);
            rowCaption[0] = DateTime.Now.ToShortDateString();
            return tables;
        }
    }
}

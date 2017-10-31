using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.Common;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsBridge.EGRUL;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.Database.FactTables.EGRUL;
using Krista.FM.Client.Reports.UFK;

namespace Krista.FM.Client.Reports
{
    public partial class ReportDataServer
    {
        /// <summary>
        /// ОТЧЕТ 0002_ЕГРЮЛ_ВЫБОР ОРГАНИЗАЦИЙ МУНИЦИПАЛЬНОГО ОБРАЗОВАНИЯ
        /// </summary>
        public DataTable[] GetEGRUL0002ReportData(Dictionary<string, string> reportParams)
        {
            var regionKeys = reportParams[ReportConsts.ParamRegionComparable].Split(',');
            var tables = new DataTable[3];
            var rowCaption = CreateReportParamsRow(tables);
            var regionCount = 1;
            tables[0] = CreateReportCaptionTable(20);

            foreach (var regionKey in regionKeys)
            {
                var dataObject = new EGRULClsJoinDataObject();
                dataObject.InitObject(scheme);
                dataObject.useSummaryRow = false;
                dataObject.mainFilter[b_Org_EGRUL.RefRegion] = regionKey;
                dataObject.AddCalcColumn(CalcColumnType.cctPosition);
                dataObject.AddDataColumn(b_Org_EGRUL.NameP);
                dataObject.AddDataColumn(b_Org_EGRUL.ShortName);
                dataObject.AddDataColumn(b_Org_EGRUL.OGRN);
                dataObject.AddDataColumn(b_Org_EGRUL.INN);
                dataObject.AddDataColumn(b_Org_EGRUL.INN20);
                dataObject.AddDataColumn(b_Org_EGRUL.Adress);
                dataObject.AddParamColumn(EGRULDataObject.ColumnRegionName);
                dataObject.SetColumnNameParam(TempFieldNames.RegionName);
                dataObject.AddParamColumn(EGRULDataObject.ColumnSettleName);
                dataObject.AddParamColumn(EGRULDataObject.ColumnOrgAddress);
                dataObject.SetColumnNameParam(TempFieldNames.OrgName);
                dataObject.AddParamColumn(EGRULDataObject.ColumnOrgStatus, b_Org_Status.Name);
                dataObject.AddParamColumn(EGRULDataObject.ColumnOrgMNSData, b_Org_MNS.Name);
                dataObject.AddDataColumn(b_Org_EGRUL.DateStartMNS);
                dataObject.AddDataColumn(b_Org_EGRUL.DateFinishMNS);
                dataObject.AddParamColumn(EGRULDataObject.ColumnFaceData, t_Org_FaceProxy.FIO);
                dataObject.AddParamColumn(EGRULDataObject.ColumnFaceData, t_Org_FaceProxy.Job);
                dataObject.AddDataColumn(b_Org_EGRUL.Contact);
                // служебные
                dataObject.AddCalcNamedColumn(CalcColumnType.cctUndefined, TempFieldNames.SortStatus);
                dataObject.AddDataColumn(b_Org_EGRUL.ID);
                dataObject.AddParamColumn(EGRULDataObject.ColumnFaceData, TempFieldNames.RowType);
                dataObject.AddDataColumn(b_Org_EGRUL.Last);
                dataObject.sortString = FormSortString(StrSortUp(TempFieldNames.OrgName), StrSortUp(b_Org_EGRUL.NameP));
                var tableData = dataObject.FillData();
                var tableFace = dataObject.tblFaceData;

                if (regionCount == 1)
                {
                    tables[0] = tableData.Clone();
                    tables[1] = tableFace.Clone();
                }

                if (tableData.Rows.Count > 0)
                {
                    var rowTest = GetLastRow(tableData);
                    var testValue = Convert.ToString(rowTest[b_Org_EGRUL.Last]).ToLower();
                    var filterValue = "1";
                    
                    if (testValue == ReportConsts.strTrue || testValue == ReportConsts.strFalse)
                    {
                        filterValue = ReportConsts.strTrue;
                    }

                    var filterStr = String.Format("{0} = {1}", b_Org_EGRUL.Last, filterValue);
                    tableData = DataTableUtils.FilterDataSet(tableData, filterStr);
                }

                foreach (DataRow rowOrg in tableData.Rows)
                {
                    rowOrg[TempFieldNames.SortStatus] = regionCount;
                    tables[0].ImportRow(rowOrg);
                }

                foreach (DataRow rowFace in tableFace.Rows)
                {
                    tables[1].ImportRow(rowFace);
                }

                rowCaption[regionCount * 2 + 1] = GetBookValue(scheme, b_Regions_Bridge.InternalKey, regionKey);
                if (regionKey != ReportConsts.UndefinedKey)
                {
                    var refTerr = GetBookValue(scheme, b_Regions_Bridge.InternalKey, regionKey,
                                                  b_Regions_Bridge.RefTerrType);
                    var typeName = GetBookValue(scheme, fx_FX_TerritorialPartitionType.InternalKey, refTerr,
                                                   fx_FX_TerritorialPartitionType.FullName);
                    rowCaption[regionCount * 2 + 2] = typeName.ToLower();
                }

                regionCount++;
            }

            if (tables[1].Columns.Count == 0)
            {
                tables[1] = CreateReportCaptionTable(20);  
            }

            rowCaption[0] = regionKeys.Length;
            rowCaption[1] = Convert.ToBoolean(reportParams[ReportConsts.ParamOutputMode]);
            return tables;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Krista.FM.Client.Reports.Common.CommonParamForm;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.FNS;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.Lvls;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.MOFO;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.Variants;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.Month.Queries;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Reports
{
    enum SettleLvl
    {
        PurposeFonds,
        Fonds,
        FondsFractional,
        Federal,
        Municipal,
        ConsMunicipal,
        ConsMunicipalFractional,
        ConsSubject,
        ConsSubjectFractional,
        Summary,
        MRTotal,
        MR,
        VillageSettle,
        TownSettle,
        SettleFractional,
        Town,
        Subject,
        Settle,
        All
    }

    enum TerritoryType
    {
        Settle,
        Region,
        Town,
        Subject,
        MrGo
    }

    class ClsTreeRecord
    {
        public string KeyValue { get; set; }
        public string RefValue { get; set; }
        public int Lvl { get; set; }
        public DataRow Row { get; set; }
    }

    class ClsCutParams
    {
        public IEntity Entity { get; set; }
        public string KeyName { get; set; }
        public string RefName { get; set; }
        public string Key { get; set; }
        public string SortStr { get; set; }
        public int Lvl { get; set; }
    }

    class RegionSplitParams
    {
        public DataTable TblResult { get; set; }
        public IEnumerable<DataRow> RowsData { get; set; }
        public int DstColumnIndex { get; set; }
        public int SrcColumnIndex { get; set; }
        public int KeyValIndex { get; set; }
        public int DocValIndex { get; set; }
        public int LvlValIndex { get; set; }
        public bool UseDocumentTypes { get; set; }
        public bool UseLvlDepencity { get; set; }
        public bool IsSkifLevels { get; set; }
        public bool SubjectInTotalSum { get; set; }
        public bool IsFractional { get; set; }

        public RegionSplitParams()
        {
            DstColumnIndex = -1;
            SrcColumnIndex = -1;
            KeyValIndex = 0;
            DocValIndex = 4;
            LvlValIndex = 4;
            UseLvlDepencity = false;
            UseDocumentTypes = false;
            IsSkifLevels = true;
            SubjectInTotalSum = false;
            IsFractional = false;
        }
    }

    class ReportMonthMethods
    {
        public const int RegionCodeIndex = 0;
        public const int RegionTerrIndex = 1;
        public const int RegionNameIndex = 2;
        public const int RegionKeyIndex = 5;
        public const int RegionLvlIndex = 6;
        public const int RegionTypIndex = 7;
        public const int RegionFlgIndex = 8;
        public const int RegionOrdIndex = 9;

        public const int RowTypeData = 0;
        public const int RowTypeRegionSummary = 1;
        public const int RowTypeTotalSummary = 2;

        private readonly IScheme scheme;
        private readonly Dictionary<string, DataTable> entityData = new Dictionary<string, DataTable>();

        public const int RegionHeaderColumnCnt = 10;

        public ReportMonthMethods(IScheme scheme)
        {
            this.scheme = scheme;
        }

        public static string CheckBookValue(string bookValue)
        {
            return bookValue == ReportConsts.UndefinedKey ? String.Empty : bookValue;
        }

        public static int GetSelectedYear(string bookValue)
        {
            var yearCollection = ParamSingleYear.YearListCollection.GetValuesList();
            return bookValue.Length > 0 ? Convert.ToInt32(yearCollection[Convert.ToInt32(bookValue)]) : -1;
        }

        public static object GetSelectedRegionLvl(string bookValue)
        {
            var param = ParamStore.container.lstParams
                .Where(f => f.GetType() == typeof(ParamMultiRegionLvls))
                .FirstOrDefault();
            return param.GetListText(bookValue);
        }

        public static object GetSelectedBudgetLvl(string bookValue)
        {
            var param = ParamStore.container.lstParams
                .Where(f => f.GetType() == typeof(ParamSingleBdgtLvls)
                    || f.GetType() == typeof(ParamMultiBdgtLvls))
                .FirstOrDefault();
            return param.GetListText(bookValue);
        }

        public static object GetSelectedBudgetLvlFull(object bookValue)
        {
            var param = ParamStore.container.lstParams
                .Where(f => f.GetType() == typeof(ParamSingleBdgtLvlsFull) ||
                    f.GetType() == typeof(ParamMultiBdgtLvlsFull))
                .FirstOrDefault();
            return param.GetListText(bookValue);
        }

        public static object GetSelectedKVSR(string bookValue)
        {
            var param = ParamStore.container.lstParams
                .Where(f => f.GetType() == typeof(ParamKVSRBridge))
                .FirstOrDefault();
            return param.BookInfo.GetText(bookValue);
        }

        public static object GetSelectedVariant(string bookValue)
        {
            var param = ParamStore.container.lstParams
                .Where(f => f.GetType() == typeof(ParamVariantIncomePlan))
                .FirstOrDefault();
            return param.BookInfo.GetText(bookValue);
        }

        public static object GetSelectedRegion(string bookValue)
        {
            var param = ParamStore.container.lstParams
                .Where(f => f.GetType() == typeof(ParamRegionBridge) ||
                    f.GetType() == typeof(ParamRegionBridgeFull))
                .FirstOrDefault();
            return param.BookInfo.GetText(bookValue);
        }

        public static object GetSelectedActivity(string bookValue)
        {
            var param = ParamStore.container.lstParams
                .Where(f => f.GetType() == typeof(ParamActivityStatus))
                .FirstOrDefault();
            return param.GetListText(bookValue);
        }

        public static object GetSelectedIncomes(string bookValue)
        {
            var param = ParamStore.container.lstParams
                .Where(f => f.GetType() == typeof(ParamTypesIncomes))
                .FirstOrDefault();
            return param.BookInfo.GetText(bookValue);
        }

        public static object GetSelectedPersons(string bookValue)
        {
            var param = ParamStore.container.lstParams
                .Where(f => f.GetType() == typeof(ParamTypesPersons))
                .FirstOrDefault();
            return param.BookInfo.GetText(bookValue);
        }

        public static int GetPrecisionIndex(string enumValue)
        {
            return ReportDataServer.GetEnumItemIndex(new PrecisionNumberEnum(), enumValue);
        }

        public static string GetDividerDescr(string enumValue)
        {
            return ReportDataServer.GetEnumItemDescription(new SumDividerEnum(), enumValue);
        }

        public static bool WriteSettles(string enumValue)
        {
            return String.Compare(enumValue, RegionListTypeEnum.i1.ToString(), true) == 0;
        }

        public static int GetBudgetLvlItem(string lvlNum)
        {
            return Convert.ToInt32(lvlNum);
        }

        public static object GetSelectedVariantMOFOMarks(string bookValue)
        {
            var param = ParamStore.container.lstParams
                .Where(f => f.GetType() == typeof(ParamVariantMOFOMarks))
                .FirstOrDefault();
            return param.BookInfo.GetText(bookValue);
        }

        public static string GetBdgtLvlSKIFCodes(SettleLvl lvl)
        {
            switch (lvl)
            {
                case SettleLvl.Municipal:
                    return "4,5";
                case SettleLvl.ConsMunicipal:
                case SettleLvl.ConsMunicipalFractional:
                    return "4,5,6";
                case SettleLvl.ConsSubject:
                case SettleLvl.ConsSubjectFractional:
                    return "2";
                case SettleLvl.Subject:
                    return "3";
                case SettleLvl.MRTotal:
                    return "2";
                case SettleLvl.MR:
                    return "5";
                case SettleLvl.Settle:
                case SettleLvl.VillageSettle:
                case SettleLvl.TownSettle:
                case SettleLvl.SettleFractional:
                    return "6";
                case SettleLvl.Town:
                    return "4";
                default:
                    return "2,3,5,6,4";
            }
        }

        public static string GetBdgtLvlCodes(SettleLvl lvl)
        {
            switch (lvl)
            {
                case SettleLvl.PurposeFonds:
                    return "12, 13";
                case SettleLvl.Fonds:
                    return "7";
                case SettleLvl.FondsFractional:
                    return "8, 9, 10, 11";
                case SettleLvl.Federal:
                    return "1";
                case SettleLvl.Municipal:
                    return "5,15";
                case SettleLvl.ConsMunicipal:
                    return "14";
                case SettleLvl.ConsMunicipalFractional:
                    return "5,6,15";
                case SettleLvl.ConsSubject:
                    return "2";
                case SettleLvl.ConsSubjectFractional:
                    return "3,5,6,15";
                case SettleLvl.Subject:
                    return "3";
                case SettleLvl.MRTotal:
                    return "5,16,17";
                case SettleLvl.MR:
                    return "5";
                case SettleLvl.VillageSettle:
                    return "17";
                case SettleLvl.TownSettle:
                    return "16";
                case SettleLvl.Settle:
                    return "6";
                case SettleLvl.SettleFractional:
                    return "16,17";
                case SettleLvl.Town:
                    return "15";
                case SettleLvl.All:
                    return "0";
                default:
                    return "3,5,15,16,17";
            }
        }

        public static string GetTerritoryCode(TerritoryType tt)
        {
            switch (tt)
            {
                case TerritoryType.Town:
                    return "7";
                case TerritoryType.Subject:
                    return "3";
                case TerritoryType.Region:
                    return "4";
                case TerritoryType.MrGo:
                    return "4,7";
                case TerritoryType.Settle:
                    return "5,6,11";
            }

            return String.Empty;
        }

        public static string GetBdgtLvlSKIFCodes(int lvl)
        {
            switch (lvl)
            {
                case 0:
                    return "2";
                case 1:
                    return "3";
                default:
                    return "4,5,6";
            }
        }

        public static string GetBdgtLvlSKIFCodes(string lvlNum)
        {
            var lvl = GetBudgetLvlItem(lvlNum);
            return GetBdgtLvlSKIFCodes(lvl);
        }

        public static string GetBdgtLvlCodes(int lvl)
        {
            switch (lvl)
            {
                case 0:
                    return "3,5,15,16,17";
                case 1:
                    return "3";
                default:
                    return "5,15,16,17";
            }
        }

        public static string GetBdgtLvlCodes(string lvlNum)
        {
            var lvl = GetBudgetLvlItem(lvlNum);
            return GetBdgtLvlCodes(lvl);
        }

        private DataTable GetEntityData(string key)
        {
            if (!entityData.ContainsKey(key))
            {
                var dbHelper = new ReportDBHelper(scheme);
                var tblData = dbHelper.GetEntityData(key);
                entityData.Add(key, tblData);
            }

            return entityData[key];
        }

        public static int AbsColumnIndex(int relationColumnIndex)
        {
            return relationColumnIndex + RegionHeaderColumnCnt;
        }

        private List<int> GetDocumentTypesSkif(int lvl, bool isTown, bool IsFractional)
        {
            var lstResult = new List<int>();

            switch (lvl)
            {
                case 2:

                    if (IsFractional)
                    {
                        lstResult.Add(7);
                        lstResult.Add(8);
                        lstResult.Add(10);
                    }
                    else
                    {
                        lstResult.Add(5);                        
                    }

                    break;
                case 3:
                    lstResult.Add(isTown ? 7 : 10);
                    break;
                case 4:
                case 5:
                    lstResult.Add(8);
                    break;
                default:
                    lstResult.Add(-1);
                    break;
            }

            return lstResult;
        }

        public static int GetDocumentTypesSkif(SettleLvl lvl)
        {
            switch (lvl)
            {
                case SettleLvl.ConsSubject:
                    return 3;
                case SettleLvl.Town:
                    return 7;
                case SettleLvl.MR:
                    return 10;
                case SettleLvl.Settle:
                case SettleLvl.SettleFractional:
                    return 8;
                default:
                    return -1;
            }
        }

        private List<int> GetSkifLvlCodeCorellation(int lvl, bool isFractional)
        {
            var list = new List<int>();

            switch (lvl)
            {
                case 2:
                    return isFractional ? new List<int> { 4, 5, 6 } : new List<int> { 3 };
                case 3:
                    return new List<int> { 5, 4 };
                case 5:
                    return new List<int> { 6 };
                default:
                    return list;
            }
        }

        private List<int> GetBdgtLvlCodeCorellation(int lvl)
        {
            var list = new List<int>();

            switch (lvl)
            {
                case 2:
                    return new List<int> { 3 };
                case 3:
                    return new List<int> { 5, 15 };
                case 5:
                    return new List<int> { 17, 16 };
                default:
                    return list;
            }
        }

        public string GetKDHierarchyFilter(string key)
        {
            var fltValue = String.Empty;
            var kdBridgeEntity = ConvertorSchemeLink.GetEntity(b_KD_Bridge.InternalKey);

            var clsParams = new ClsCutParams
            {
                Key = Convert.ToString(key),
                Entity = kdBridgeEntity,
                RefName = b_KD_Bridge.ParentID,
                KeyName = b_KD_Bridge.ID
            };

            var rows = GetChildClsRecord(clsParams);

            foreach (var row in rows)
            {
                fltValue = ReportDataServer.Combine(fltValue, row.KeyValue, ",");
            }

            return String.Format("{0},{1}", key, fltValue.Trim(','));
        }

        public ClsCutParams CreateCutKDParams()
        {
            return new ClsCutParams
            {
                Entity = ConvertorSchemeLink.GetEntity(b_KD_Bridge.InternalKey),
                KeyName = b_KD_Bridge.ID,
                RefName = b_KD_Bridge.ParentID,
                SortStr = b_KD_Bridge.CodeStr
            };
        }

        public ClsCutParams CreateCutGroupKDParams()
        {
            return new ClsCutParams
            {
                Entity = ConvertorSchemeLink.GetEntity(b_D_GroupKD.InternalKey),
                KeyName = b_D_GroupKD.ID,
                RefName = b_D_GroupKD.ParentID,
                SortStr = b_D_GroupKD.CodeStr
            };
        }

        public string ClearKDValues(string keys, bool clearChilds = false)
        {
            var cutParams = CreateCutKDParams();
            cutParams.Key = keys;

            var clearKD = CutClsRecords(cutParams, clearChilds);

            if (clearKD.Length == 0)
            {
                clearKD = ReportConsts.UndefinedKey;
            }

            return clearKD;
        }

        public void ClearSettleRows(DataTable tblData, string lvlParam)
        {
            if (String.Compare(lvlParam, RegionListTypeEnum.i1.ToString(), true) == 0)
            {
                return;
            }

            var rowsSettle = tblData.Select(String.Format("{0} > 3 or {0} = 3 and {1} = {2} ",
                tblData.Columns[RegionLvlIndex].ColumnName,
                tblData.Columns[RegionFlgIndex].ColumnName,
                RowTypeRegionSummary));

            foreach (var dataRow in rowsSettle)
            {
                tblData.Rows.Remove(dataRow);
            }
        }

        public void SplitRegionData(RegionSplitParams splitParams)
        {
            var resultIndex = AbsColumnIndex(splitParams.DstColumnIndex);
            var tblResult = splitParams.TblResult;

            var rowSummary = tblResult.Rows[tblResult.Rows.Count - 1];

            if (Convert.ToInt32(rowSummary[RegionFlgIndex]) != RowTypeTotalSummary)
            {
                rowSummary = InsertSummaryRow(tblResult, "Итого", 2, RowTypeTotalSummary);
            }

            foreach (DataRow rowResult in tblResult.Rows)
            {
                var lvl = Convert.ToInt32(rowResult[RegionLvlIndex]);
                var key = Convert.ToInt32(rowResult[RegionKeyIndex]);
                var ter = Convert.ToInt32(rowResult[RegionTypIndex]);

                var lstDocType = GetDocumentTypesSkif(lvl, ter == 7, splitParams.IsFractional);

                var lvlList = splitParams.IsSkifLevels ?
                    GetSkifLvlCodeCorellation(lvl, splitParams.IsFractional) : GetBdgtLvlCodeCorellation(lvl);
                
                var regionRows =
                    from row in splitParams.RowsData
                    let keyVal = Convert.ToInt32(row[splitParams.KeyValIndex])
                    let lvlVal = Convert.ToInt32(row[splitParams.LvlValIndex])
                    let docVal = Convert.ToInt32(row[splitParams.DocValIndex])
                    where
                        (!splitParams.UseLvlDepencity || lvlList.Contains(lvlVal)) &&
                        keyVal == key &&
                        (!splitParams.UseDocumentTypes || lstDocType.Contains(docVal))
                    select row;

                rowResult[resultIndex] = regionRows.Sum(dataRow =>
                    ReportDataServer.GetDecimal(dataRow[splitParams.SrcColumnIndex]));
            }

            for (var i = 0; i < tblResult.Rows.Count; i++)
            {
                var rowResult = tblResult.Rows[i];
                var lvl = Convert.ToInt32(rowResult[RegionLvlIndex]);

                if (lvl == 4)
                {
                    var rowIndex = i + 1;
                    var nextLvl = lvl + 1;
                    decimal sumSettle = 0;

                    while (nextLvl > lvl && rowIndex < tblResult.Rows.Count)
                    {
                        var rowNext = tblResult.Rows[rowIndex];
                        nextLvl = Convert.ToInt32(rowNext[RegionLvlIndex]);

                        if (nextLvl > lvl)
                        {
                            sumSettle += ReportDataServer.GetDecimal(rowNext[resultIndex]);
                        }

                        rowIndex++;
                    }

                    rowResult[resultIndex] = sumSettle;
                }
            }

            for (var i = 0; i < tblResult.Rows.Count; i++)
            {
                var rowResult = tblResult.Rows[i];
                var calcType = Convert.ToInt32(rowResult[RegionFlgIndex]);

                if (calcType == RowTypeRegionSummary)
                {
                    var regionRow = tblResult.Rows[i - 1];
                    var settleRow = tblResult.Rows[i + 1];

                    rowResult[resultIndex] = regionRow[resultIndex];
                    regionRow[resultIndex] =
                        ReportDataServer.GetDecimal(regionRow[resultIndex]) +
                        ReportDataServer.GetDecimal(settleRow[resultIndex]);
                }
            }

            decimal totalSum = 0;

            for (var i = 0; i < tblResult.Rows.Count; i++)
            {
                var rowResult = tblResult.Rows[i];
                var ter = Convert.ToInt32(rowResult[RegionTypIndex]);
                var lvl = Convert.ToInt32(rowResult[RegionLvlIndex]);

                if (lvl == 2 && ter != 3)
                {
                    var rowIndex = i + 1;
                    var nextLvl = lvl + 1;
                    decimal sumDataBlock = 0;

                    while (nextLvl > lvl && rowIndex < tblResult.Rows.Count)
                    {
                        var rowNext = tblResult.Rows[rowIndex];
                        nextLvl = Convert.ToInt32(rowNext[RegionLvlIndex]);
                        var calcType = Convert.ToInt32(rowNext[RegionFlgIndex]);

                        if (nextLvl == 3 && calcType != RowTypeRegionSummary)
                        {
                            sumDataBlock += ReportDataServer.GetDecimal(rowNext[resultIndex]);
                        }

                        rowIndex++;
                    }

                    rowResult[resultIndex] = sumDataBlock;
                    totalSum += sumDataBlock;
                }

                if (splitParams.SubjectInTotalSum)
                {
                    if (lvl == 2 && ter == 3)
                    {
                        totalSum += ReportDataServer.GetDecimal(rowResult[resultIndex]);
                    }
                }
            }

            rowSummary[resultIndex] = totalSum;
        }

        public static DataRow FindMRRow(DataTable tbl, int settleKey)
        {
            var findRow = tbl.Select(String.Format("{0}='{1}'", tbl.Columns[RegionKeyIndex].ColumnName, settleKey));
            var rowIndex = -1;

            if (findRow.Length > 0)
            {
                rowIndex = tbl.Rows.IndexOf(findRow[0]);
            }

            while (rowIndex >= 0)
            {
                var row = tbl.Rows[rowIndex];
                var lvl = Convert.ToInt32(row[RegionLvlIndex]);
                var ter = Convert.ToInt32(row[RegionTypIndex]);

                if (lvl == 3 && ter == 4)
                {
                    return row;
                }

                rowIndex--;
            }

            return null;
        }

        public static DataTable CreateSubjectTable(DataTable tbl)
        {
            var tblResult = tbl.Clone();
            var rowsSubject = tbl.Select(String.Format("{0} = '3'", tbl.Columns[RegionTypIndex].ColumnName));

            if (rowsSubject.Length > 0)
            {
                var rowSubject = rowsSubject[0];
                tblResult.ImportRow(rowSubject);
                tbl.Rows.Remove(rowSubject);
            }

            return tblResult;
        }

        private DataRow InsertSummaryRow(DataTable tblResult, string name, int lvl, int rowType)
        {
            var rowRegionSummary = tblResult.Rows.Add();
            rowRegionSummary[RegionNameIndex] = name;
            rowRegionSummary[RegionFlgIndex] = rowType;
            rowRegionSummary[RegionLvlIndex] = lvl;
            rowRegionSummary[RegionKeyIndex] = ReportConsts.UndefinedKey;
            rowRegionSummary[RegionTypIndex] = ReportConsts.UndefinedKey;
            return rowRegionSummary;
        }

        public DataTable CreateRegionList(int columnCount, bool addSubjectRow = true)
        {
            var tblResult = new DataTable();

            for (var i = 0; i < columnCount + RegionHeaderColumnCnt; i++)
            {
                tblResult.Columns.Add(String.Format("Column{0}", i), typeof(string));
            }

            var entityKey = b_Regions_Bridge.InternalKey;
            var terrTypesKey = fx_FX_TerritorialPartitionType.InternalKey;
            var tblRegion = GetEntityData(entityKey);
            var tblTerType = GetEntityData(terrTypesKey);
            var roots = tblRegion.Select(
                String.Format("{0} is null", b_Regions_Bridge.ParentID),
                b_Regions_Bridge.RefTerrType);

            var clsParams = new ClsCutParams
            {
                Entity = ConvertorSchemeLink.GetEntity(entityKey),
                KeyName = b_Regions_Bridge.ID,
                RefName = b_Regions_Bridge.ParentID,
                Lvl = 2,
                SortStr = b_Regions_Bridge.Code
            };

            foreach (var dataRow in roots)
            {
                clsParams.Key = Convert.ToString(dataRow[b_Regions_Bridge.ID]);
                var leafs = GetChildClsRecord(clsParams);

                var maxLvl = leafs.Count() == 0 ? 0 : leafs.Max(l => l.Lvl);

                if (maxLvl < 4)
                {
                    continue;
                }

                var orderNum = 0;

                foreach (var clsRecord in leafs)
                {
                    var terrType = Convert.ToInt32(clsRecord.Row[b_Regions_Bridge.RefTerrType]);

                    if (clsRecord.Lvl == 2 && terrType != 0)
                    {
                        if (!addSubjectRow)
                        {
                            continue;
                        }
                    }

                    var rowResult = tblResult.Rows.Add();
                    var rowCls = clsRecord.Row;
                    rowResult[RegionCodeIndex] = rowCls[b_Regions_Bridge.CodeLine];
                    rowResult[RegionNameIndex] = rowCls[b_Regions_Bridge.Name];

                    // служебные
                    rowResult[RegionKeyIndex] = clsRecord.KeyValue;
                    rowResult[RegionLvlIndex] = clsRecord.Lvl;
                    rowResult[RegionTypIndex] = terrType;
                    rowResult[RegionFlgIndex] = RowTypeData;
                    rowResult[RegionOrdIndex] = Convert.ToString(orderNum++).PadLeft(5, '0');

                    // итоги для районного бюджета
                    if (clsRecord.Lvl == 3 && terrType == 4)
                    {
                        InsertSummaryRow(tblResult, "Районный бюджет", clsRecord.Lvl, RowTypeRegionSummary);
                    }

                    if (clsRecord.Lvl != 3 && clsRecord.Lvl != 5)
                    {
                        continue;
                    }

                    var terrFilter = String.Format("{0} = {1}", fx_FX_TerritorialPartitionType.ID, terrType);
                    var rowsTerrType = tblTerType.Select(terrFilter);

                    if (rowsTerrType.Length > 0)
                    {
                        var rowTerrCls = rowsTerrType[0];
                        rowResult[RegionTerrIndex] = rowTerrCls[fx_FX_TerritorialPartitionType.Name];
                    }
                }
            }

            return tblResult;
        }

        public IEnumerable<ClsTreeRecord> GetChildClsRecord(ClsCutParams cutParams)
        {
            var resultList = new List<ClsTreeRecord>();
            var tblData = GetEntityData(cutParams.Entity.ObjectKey);
            var rowsChilds = tblData.Select(
                String.Format("{0} = {1}", cutParams.RefName, cutParams.Key),
                cutParams.SortStr);

            foreach (DataRow rowChild in rowsChilds)
            {
                var lvl = cutParams.Lvl++;
                var rowCls = new ClsTreeRecord
                {
                    KeyValue = Convert.ToString(rowChild[cutParams.KeyName]),
                    RefValue = Convert.ToString(rowChild[cutParams.RefName]),
                    Lvl = lvl,
                    Row = rowChild
                };

                resultList.Add(rowCls);
                cutParams.Key = rowCls.KeyValue;
                var childList = GetChildClsRecord(cutParams);
                cutParams.Lvl = lvl;
                resultList.AddRange(childList);
            }

            return resultList;
        }

        public DataRow GetBookRow(IEntity entity, object refValue)
        {
            var dbHelper = new ReportDBHelper(scheme);
            var tblBook = dbHelper.GetEntityData(entity, String.Format("id = {0}", refValue));
            return tblBook.Rows.Count > 0 ? tblBook.Rows[0] : null;
        }

        public DataRow GetBookRow(IEntity entity, string fieldName, string fieldValue)
        {
            var dbHelper = new ReportDBHelper(scheme);
            var tblBook = dbHelper.GetEntityData(entity, String.Format("{0} = '{1}'", fieldName, fieldValue));
            return tblBook.Rows.Count > 0 ? tblBook.Rows[0] : null;
        }

        public string GetRegionCaptionText(string filter)
        {
            if (filter == String.Empty)
            {
                return String.Empty;
            }

            var dbHelper = new ReportDBHelper(scheme);
            var regions = dbHelper.GetEntityData(b_Regions_Bridge.InternalKey, String.Format("id in ({0})", filter));
            if (regions.Rows.Count == 0)
            {
                return String.Empty;
            }

            var dtTypes = dbHelper.GetEntityData(fx_FX_TerritorialPartitionType.InternalKey);

            return String.Join("; ", (from DataRow row in regions.Rows
                                      let types = dtTypes.Select(String.Format("id = {0}", row[b_Regions_Bridge.RefTerrType]))
                                      let type = types.Length > 0 ? types[0][fx_FX_TerritorialPartitionType.FullName] : String.Empty
                                      let caption = String.Format("{0} {1}", row[b_Regions_Bridge.CodeLine], type).Trim()
                                      select String.Format("{0} {1}", caption, row[b_Regions_Bridge.Name]).Trim()).ToArray());
        }

        public string GetNotNestedRegionCaptionText(string filter)
        {
            return GetRegionCaptionText(GetNotNestedID(b_Regions_Bridge.InternalKey, filter));
        }

        public object GetBridgeCaptionText(string key, string filter, string codeField, string nameField)
        {
            if (filter.Length > 0)
            {
                var entity = ConvertorSchemeLink.GetEntity(key);
                var refList = filter.Split(',');

                foreach (var refValue in refList)
                {
                    var rowBook = GetBookRow(entity, refValue);

                    if (rowBook != null)
                    {
                        if (codeField.Length == 0)
                        {
                            return String.Format("{0}", rowBook[nameField]);
                        }
                        {
                            return String.Format("{0} {1}", rowBook[codeField], rowBook[nameField]);
                        }
                    }
                }
            }

            return String.Empty;
        }

        public string GetStructureCharacterCaptionText(string filter)
        {
            if (filter.Length == 0)
            {
                return String.Empty;
            }

            var dbHelper = new ReportDBHelper(scheme);
            var dt = dbHelper.GetEntityData(
                fx_FX_StructureCharacter.InternalKey,
                String.Format("id in ({0})", filter));
            var rows = from DataRow row in dt.Rows
                       select Convert.ToString(row[fx_FX_StructureCharacter.Name]);
            return String.Join(", ", rows.ToArray());
        }

        public object GetPayerText(string filter)
        {
            return GetBridgeCaptionText(
                b_Org_PayersBridge.InternalKey,
                filter,
                b_Org_PayersBridge.INN,
                b_Org_PayersBridge.Name);
        }

        public object GetKDBridgeCaptionText(string filter)
        {
            if (filter.Length > 0)
            {
                var entity = ConvertorSchemeLink.GetEntity(b_KD_Bridge.InternalKey);
                var refList = filter.Split(',');

                foreach (var refValue in refList)
                {
                    var rowBook = GetBookRow(entity, refValue);

                    if (rowBook != null)
                    {
                        return String.Format("{0} {1}", rowBook[b_KD_Bridge.CodeStr], rowBook[b_KD_Bridge.Name]);
                    }
                }
            }

            return String.Empty;
        }

        public string GetKVSRBridgeCaptionText(string filter)
        {
            if (filter.Length == 0)
            {
                return String.Empty;
            }

            var entity = ConvertorSchemeLink.GetEntity(b_KVSR_Bridge.InternalKey);
            var dbHelper = new ReportDBHelper(scheme);
            var queryText = String.Format("select {0}, {1} from {2} where id in ({3})",
                                            b_KVSR_Bridge.Code,
                                            b_KVSR_Bridge.Name,
                                            entity.FullDBName,
                                            filter);
            var dt = dbHelper.GetTableData(queryText);
            var kvsrList = (from DataRow row in dt.Rows
                            let code = Convert.ToString(row[b_KVSR_Bridge.Code]).PadLeft(3, '0')
                            select String.Format("{0} {1}", code, row[b_KD_Bridge.Name])).ToArray();

            return String.Join(", ", kvsrList);
        }

        public string GetBudgetLevelCaptionText(string filter)
        {
            if (filter.Length == 0)
            {
                return String.Empty;
            }

            var dbHelper = new ReportDBHelper(scheme);
            var dt = dbHelper.GetEntityData(fx_FX_BudgetLevels.InternalKey, String.Format("id in ({0})", filter));
            var kvsrList = (from DataRow row in dt.Rows
                            orderby Convert.ToInt32(row[fx_FX_BudgetLevels.Code])
                            select Convert.ToString(row[fx_FX_BudgetLevels.Name])).ToArray();
            return String.Join(", ", kvsrList);
        }

        public string CutClsRecords(ClsCutParams cutParams, bool clearChilds = true)
        {
            const string splitter = ",";
            var result = String.Empty;
            var values = cutParams.Key.Replace(" ", String.Empty);
            var valuesList = values.Split(new[] { splitter }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var resultList = new List<string>();

            while (valuesList.Count > 0)
            {
                var parentKey = valuesList[0];
                cutParams.Key = parentKey;
                var childList = GetChildClsRecord(cutParams);

                if (clearChilds || childList.Count() == 0)
                {
                    resultList.Add(parentKey);
                }

                valuesList.Remove(parentKey);

                foreach (var childItem in childList)
                {
                    if (!clearChilds)
                    {
                        resultList.Add(childItem.KeyValue);
                    }

                    valuesList.Remove(childItem.KeyValue);
                }

                if (clearChilds)
                {
                    continue;
                }

                var fictiveLevels = true;
                while (fictiveLevels)
                {
                    fictiveLevels = false;

                    foreach (var childItem in childList)
                    {
                        if (resultList.Contains(childItem.RefValue))
                        {
                            fictiveLevels = true;
                            resultList.Remove(childItem.RefValue);
                        }
                    }
                }
            }

            result = resultList.Aggregate(result, (current, resultKey) =>
                String.Join(splitter, new[] { current, resultKey }));

            return result.Trim(splitter[0]);
        }

        public static DataTable FilterLastMonth(DataTable tblData, string fieldName)
        {
            tblData = DataTableUtils.SortDataSet(tblData, fieldName);

            if (tblData.Rows.Count > 0)
            {
                var maxPeriod = ReportDataServer.GetLastRow(tblData)[fieldName];
                var filterPeriod = String.Format("{0} = {1}", fieldName, maxPeriod);
                return DataTableUtils.FilterDataSet(tblData, filterPeriod);
            }

            return tblData;
        }

        public static string CreateRangeFilter(IEnumerable<int> values)
        {
            var strBuilder = new StringBuilder();

            foreach (var value in values)
            {
                strBuilder.Append(value);
                strBuilder.Append(",");
            }

            return strBuilder.ToString().Trim(',');
        }

        public int GetMaxMonth(string key, string unvField, int year)
        {
            const string sql = "select max({0}) from {1} where {2}";
            var filterHelper = new QFilterHelper();
            var dbHelper = new ReportDBHelper(scheme);
            var loYearBound = ReportDataServer.GetUNVYearStart(year);
            var hiYearBound = ReportDataServer.GetUNVYearEnd(year);
            var fltPeriod = filterHelper.PeriodFilter(unvField, loYearBound, hiYearBound);
            var entity = ConvertorSchemeLink.GetEntity(key);
            var selectStr = String.Format(sql, unvField, entity.FullDBName, fltPeriod);
            var value = dbHelper.GetScalarData(selectStr);
            return value != DBNull.Value
                       ? ReportDataServer.GetNormalDate(Convert.ToString(value)).Month
                       : 0;
        }

        public string GetDGroupCaptionText(string filter)
        {
            if (filter.Length == 0)
            {
                return String.Empty;
            }

            var entity = ConvertorSchemeLink.GetEntity(b_D_Group.InternalKey);
            var dbHelper = new ReportDBHelper(scheme);
            var queryText = String.Format("select {0}, {1} from {2} where id in ({3})",
                                            b_D_Group.Code,
                                            b_D_Group.Name,
                                            entity.FullDBName,
                                            filter);
            var dt = dbHelper.GetTableData(queryText);
            var bDList = (from DataRow row in dt.Rows
                          select String.Format("{0} {1}", row[b_D_Group.Code], row[b_D_Group.Name])).ToArray();

            return String.Join(", ", bDList);
        }

        public string GetIDByFilter(string entityKey, string filter)
        {
            if (filter.Length == 0)
            {
                return String.Empty;
            }

            const string fieldID = "ID";
            var entity = ConvertorSchemeLink.GetEntity(entityKey);
            var dbHelper = new ReportDBHelper(scheme);
            var dt = dbHelper.GetEntityData(entity, filter);
            var id = dt.Rows.Cast<DataRow>().Select(row => Convert.ToString(row[fieldID]));
            return String.Join(", ", id.Distinct().ToArray());
        }

        public string GetIDByField(string entityKey, string field, string filter)
        {
            if (filter.Length == 0)
            {
                return String.Empty;
            }

            var filterHelper = new QFilterHelper();
            var fltRange = filterHelper.RangeFilter(field, filter);
            return GetIDByFilter(entityKey, fltRange);
        }

        public string GetNestedIDByField(string entityKey, string field, string filter)
        {
            var strCodes = GetIDByField(entityKey, field, filter);
            if (strCodes.Length == 0)
            {
                return String.Empty;
            }

            const string fieldID = "ID";
            const string fieldParentID = "ParentID";
            var entity = ConvertorSchemeLink.GetEntity(entityKey);
            var parentID = strCodes.Split(',').Select(code => code.Trim()).Distinct().ToList();
            parentID.Sort();

            var clsParams = new ClsCutParams
            {
                Entity = entity,
                KeyName = fieldID,
                RefName = fieldParentID,
                SortStr = field
            };

            var id = new List<string>();
            foreach (var code in parentID)
            {
                clsParams.Lvl = 0;
                clsParams.Key = code;
                var leafs = GetChildClsRecord(clsParams);
                id.Add(code);
                id.AddRange(leafs.Select(clsRecord => clsRecord.KeyValue));
            }
            return String.Join(", ", id.Distinct().ToArray());
        }

        public string GetActiveSourceID(string entityKey, string id)
        {
            if (id.Length == 0)
            {
                return String.Empty;
            }

            var dbHelper = new ReportDBHelper(ConvertorSchemeLink.scheme);
            var source = dbHelper.GetActiveSource(entityKey);
            if (source != -1)
            {
                var filter = String.Format("sourceid = {0} and id in ({1})", source, id);
                return GetIDByFilter(entityKey, filter);
            }

            return id;
        }

        public string GetKDNestedID(string codeFilter)
        {
            const string entityKey = b_KD_Bridge.InternalKey;
            var id = GetNestedIDByField(entityKey, b_KD_Bridge.CodeStr, codeFilter);
            return GetActiveSourceID(entityKey, id);
        }

        public string GetArrearsFNSNestedID(string codeFilter)
        {
            const string entityKey = b_Arrears_FNSBridge.InternalKey;
            var id = GetNestedIDByField(entityKey, b_Arrears_FNSBridge.Code, codeFilter);
            return GetActiveSourceID(entityKey, id);
        }

        public string GetNotNestedID(string entityKey, string filter)
        {
            if (filter.Length == 0)
            {
                return String.Empty;
            }

            const string fieldID = "ID";
            const string fieldParentID = "ParentID";
            var entity = ConvertorSchemeLink.GetEntity(entityKey);
            var idList = filter.Split(',').Select(code => code.Trim()).Distinct().ToList();
            idList.Sort();

            var clsParams = new ClsCutParams
            {
                Entity = entity,
                KeyName = fieldID,
                RefName = fieldParentID,
                SortStr = fieldID
            };

            var i = 0;
            while (i < idList.Count())
            {
                var id = idList[i];
                clsParams.Lvl = 0;
                clsParams.Key = id;
                var leafs = GetChildClsRecord(clsParams);
                foreach (var clsRecord in leafs)
                {
                    idList.Remove(clsRecord.KeyValue);
                }

                var index = idList.BinarySearch(id);
                i = index >= 0 ? index + 1 : 0;
            }

            return String.Join(", ", idList.ToArray());
        }

        public string GetNotNestedKDCaptionText(string filter)
        {
            var notNestedID = GetNotNestedID(b_KD_Bridge.InternalKey, filter);
            if (notNestedID.Length == 0)
            {
                return String.Empty;
            }

            var entity = ConvertorSchemeLink.GetEntity(b_KD_Bridge.InternalKey);
            var dbHelper = new ReportDBHelper(scheme);
            var queryText = String.Format("select {0}, {1} from {2} where id in ({3}) order by {0}",
                                            b_KD_Bridge.CodeStr,
                                            b_KD_Bridge.Name,
                                            entity.FullDBName,
                                            notNestedID);
            var dt = dbHelper.GetTableData(queryText);
            var kdRows = (from DataRow row in dt.Rows
                          select String.Format("{0} {1}", row[b_KD_Bridge.CodeStr], row[b_KD_Bridge.Name])).ToArray();

            return String.Join(", ", kdRows);
        }

        public string GetArrearsFNSCaptionText(string filter, bool nestedCodes = false)
        {
            if (!nestedCodes)
            {
                filter = GetNotNestedID(b_Arrears_FNSBridge.InternalKey, filter);
            }

            if (filter.Length == 0)
            {
                return String.Empty;
            }

            var entity = ConvertorSchemeLink.GetEntity(b_Arrears_FNSBridge.InternalKey);
            var dbHelper = new ReportDBHelper(scheme);
            var queryText = String.Format("select {0}, {1} from {2} where id in ({3})",
                                            b_Arrears_FNSBridge.Code,
                                            b_Arrears_FNSBridge.Name,
                                            entity.FullDBName,
                                            filter);
            var dt = dbHelper.GetTableData(queryText);
            var arrears = (from DataRow row in dt.Rows
                           select String.Format("{0} {1}", row[b_Arrears_FNSBridge.Code], row[b_Arrears_FNSBridge.Name])).ToArray();

            return String.Join(", ", arrears);
        }

        public string CreateGroupKBKFilters(ref string filterKD, string filterGroup)
        {
            var dbHelper = new ReportDBHelper(scheme);
            var filterHelper = new QFilterHelper();
            var entKdBridge = ConvertorSchemeLink.GetEntity(b_KD_Bridge.InternalKey);
            var entGroupKBK = ConvertorSchemeLink.GetEntity(b_D_GroupKD.InternalKey);
            var clearKD = filterKD;

            if (filterKD.Length == 0 && filterGroup.Length == 0)
            {
                var tblGroups = dbHelper.GetEntityData(entGroupKBK, filterHelper.MoreEqualFilter(b_D_GroupKD.ID, 0));
                filterGroup = tblGroups.Rows.Cast<DataRow>().Aggregate(filterGroup, (current, rowGroup) =>
                    ReportDataServer.Combine(current, rowGroup[b_D_GroupKD.ID], ","));
                filterGroup = filterGroup.Trim(',');
            }

            var cutParams = CreateCutKDParams();

            if (filterGroup.Length > 0)
            {
                var fltGroupRange = filterHelper.RangeFilter(b_D_GroupKD.ID, filterGroup);
                var tblGroup = dbHelper.GetEntityData(entGroupKBK, fltGroupRange);
                tblGroup = DataTableUtils.SortDataSet(tblGroup, b_D_GroupKD.CodeStr);
                filterKD = String.Empty;
                var groupSummary = new List<string>();
                var existKBK = new List<string>();
                var cutGroupParams = CreateCutGroupKDParams();

                foreach (DataRow rowGroup in tblGroup.Rows)
                {
                    var refGroup = Convert.ToString(rowGroup[b_D_GroupKD.ID]);

                    if (groupSummary.Contains(refGroup))
                    {
                        continue;
                    }

                    cutGroupParams.Key = refGroup;
                    var childs = GetChildClsRecord(cutGroupParams);
                    var fltGroup = refGroup;

                    if (childs.Count() > 0)
                    {
                        fltGroup = childs.Aggregate(fltGroup, (current, clsRec) =>
                            ReportDataServer.Combine(current, clsRec.KeyValue, ","));
                        fltGroup = fltGroup.Trim(',');
                    }

                    fltGroupRange = filterHelper.RangeFilter(b_KD_Bridge.RefDGroupKD, fltGroup);
                    var tblKBK = dbHelper.GetEntityData(entKdBridge, fltGroupRange);
                    tblKBK = DataTableUtils.SortDataSet(tblKBK, b_KD_Bridge.CodeStr);

                    foreach (DataRow rowKBK in tblKBK.Rows)
                    {
                        cutParams.Key = Convert.ToString(rowKBK[b_KD_Bridge.ID]);

                        if (existKBK.Contains(cutParams.Key))
                        {
                            continue;
                        }

                        existKBK.Add(cutParams.Key);
                        var childsFilter = cutParams.Key;
                        clearKD = ReportDataServer.Combine(clearKD, childsFilter, ",");
                        clearKD = clearKD.Trim(',');
                        var childsKBK = GetChildClsRecord(cutParams);

                        if (childsKBK.Count() > 0)
                        {
                            childsFilter = childsKBK.Aggregate(childsFilter,
                                (current, clsRec) => ReportDataServer.Combine(current, clsRec.KeyValue, ","));
                            childsFilter = childsFilter.Trim(',');
                        }

                        filterKD = ReportDataServer.Combine(filterKD, childsFilter, ",");
                        filterKD = filterKD.Trim(',');
                    }

                    groupSummary.AddRange(childs.Select(child => child.KeyValue));
                }
            }

            if (filterKD.Length == 0)
            {
                filterKD = ReportConsts.UndefinedKey;
                clearKD = ReportConsts.UndefinedKey;
            }

            return clearKD;
        }

        public DataRow GetIncomeVariantRow(object variantId)
        {
            var entDVariant = ConvertorSchemeLink.GetEntity(d_Variant_PlanIncomes.InternalKey);
            return GetBookRow(entDVariant, variantId);
        }

        public int GetIncomeVariantYear(object variantId)
        {
            var rowVariant = GetIncomeVariantRow(variantId);
            return rowVariant == null ? -1 : Convert.ToInt32(rowVariant[d_Variant_PlanIncomes.RefYear]);
        }

        public string GetIncomeVariantName(object variantId)
        {
            var rowVariant = GetIncomeVariantRow(variantId);
            return rowVariant == null ? String.Empty : Convert.ToString(rowVariant[d_Variant_PlanIncomes.Name]);
        }

        public static string Years(int count)
        {
            var yearList = new List<string>();
            var yearId = ParamSingleYear.YearListCollection.GetValuesList().IndexOf(Convert.ToString(DateTime.Now.Year));
            count--;

            if (count < 0)
            {
                return String.Empty;
            }

            if (count > yearId)
            {
                count = yearId;
            }

            for (var i = count; i >= 0; i--)
            {
                yearList.Add(Convert.ToString(yearId - i));
            }

            return String.Join(", ", yearList.ToArray());
        }
    }
}

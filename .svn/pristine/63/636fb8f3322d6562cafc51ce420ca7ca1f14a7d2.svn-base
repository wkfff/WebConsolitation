using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Krista.FM.Client.Reports.Common;
using Krista.FM.Client.Reports.Database.ClsBridge;
using Krista.FM.Client.Reports.Database.ClsBridge.EGRUL;
using Krista.FM.Client.Reports.Database.ClsData.EGRUL;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.Database.FactTables.EGRUL;

namespace Krista.FM.Client.Reports
{
    class EGRULClsJoinDataObject : EGRULDataObject
    {
        protected override Dictionary<string, string> InnerJoins()
        {
            var joinTables = new Dictionary<string, string>
                                 {
                                     { d_Org_Adress.InternalKey, b_Org_EGRUL.RefAdress }
                                 };
            return joinTables;
        }
    }

    class EGRULDetailTest : EGRULDataObject
    {
        protected override Dictionary<string, string> InnerJoins()
        {
            var joinTables = new Dictionary<string, string>
                                 {
                                     { b_Org_EGRUL.InternalKey, t_Org_OKVED.RefOrgEGRUL },
                                     { d_Org_Adress.InternalKey, b_Org_EGRUL.RefAdress }
                                 };
            return joinTables;
        }

        protected override Dictionary<string, string> JoinRelations()
        {
            var joinTables = new Dictionary<string, string>
                                 {
                                     { d_Org_Adress.InternalKey, b_Org_EGRUL.InternalKey }
                                 };
            return joinTables;
        }
    }

    class EGRULDataObject : CommonDataObject
    {
        public const string ColumnRegionName = "RegionName";
        public const string ColumnSettleName = "SettleName";
        public const string ColumnOrgAddress = "OrgAddress";
        public const string ColumnOrgStatus = "OrgStatus";
        public const string ColumnFaceData = "FaceData";
        public const string ColumnKOPFData = "KOPFData";
        public const string ColumnOrgMNSData = "OrgMNSData";
        public const string ColumnOrgROData = "OrgROData";
        public const string ColumnRegTypeStartData = "RegTypeStartData";
        public const string ColumnRegTypeFinishData = "RegTypeFinishData";
        public const string ColumnOrgROStartData = "OrgROStartData";
        public const string ColumnOrgROFinishData = "OrgROFinishData";
        public const string ColumnOrgKindCapitalData = "OrgKindCapitalData";

        public const string ColumnCommonBookData = "CommonBookData";
        public const string ColumnOrgROFullData = "OrgROFullData";
        public const string ColumnDocumentDescr = "DocumentDescr";
        public const string ColumnCodeNameData = "CodeNameData";
        public const string ColumnOKVEDMaskCode = "OKVEDMaskCode";


        public const string ColumnJoinField = "ColumnJoinField";
        public const string ColumnAddressPart = "ColumnAddressPart";
        public const string ColumnFieldSequence = "ColumnFieldSequence";
        public const string ColumnDetailText = "ColumnDetailText";
        public const string ColumnDetailSequence = "ColumnDetailSequence";

        public const string ColumnOKATOMask = "ColumnOKATOMask";
        public const string ColumnCutContactInfo = "ColumnCutContactInfo";

        // вычисляшки
        public const string KOPFTemplate = "KOPFTemplate";

        private DataTable cacheRegions = new DataTable();

        public DataTable tblFaceData = new DataTable();

        private readonly Dictionary<int, string> cacheRegionNames = new Dictionary<int, string>();
        private readonly Dictionary<int, string> cacheSettleNames = new Dictionary<int, string>();

        public readonly Dictionary<string, DataTable> cacheData = new Dictionary<string, DataTable>();

        private readonly Collection<DataRow> rowsDetail = new Collection<DataRow>();

        protected override string GetMainTableKey()
        {
            return ObjectKey.Length > 0 ? ObjectKey : b_Org_EGRUL.InternalKey;
        }

        public override string GetParentRefName()
        {
            return t_Org_FaceProxy.RefOrgEGRUL;
        }

        protected override bool UseStrongFilters()
        {
            return true;
        }

        protected override void SetCalcFieldValues(DataRow destRow, DataRow sourceRow, int colIndex)
        {
            var currentParams = columnParamList[colIndex];

            if (colIndex == 0)
            {
                rowsDetail.Clear();
                rowsDetail.Add(destRow);
            }

            if (!currentParams.ContainsKey(ParamValue1))
            {
                return;
            }

            var fieldType = currentParams[ParamValue1];
            int id;

            if (sourceRow != null)
            {
                id = Convert.ToInt32(sourceRow[b_Org_EGRUL.ID]);
            }
            else
            {
                return;
            }

            if (fieldType == ColumnCommonBookData)
            {
                const int maxFieldCount = 10;
                const string formatStr = "{0} {1} {2}";
                var refField = currentParams[ParamValue2];
                var tableKey = currentParams[ParamValue3];
                var entity = GetEntity(tableKey);
                var fieldNames = currentParams[ParamValue4].Split(',');
                var fieldValues = new string[maxFieldCount];

                var refValue = sourceRow[refField];

                if (refValue != DBNull.Value && Convert.ToInt32(refValue) != -1)
                {
                    for (var f = 0; f < fieldNames.Length; f++)
                    {
                        fieldValues[f] = GetBookValue(entity, sourceRow[refField], fieldNames[f]);
                    }
                }

                destRow[colIndex] = String.Format(formatStr, fieldValues).Trim();
            }

            if (fieldType == ColumnCodeNameData)
            {
                var refField = currentParams[ParamValue2];
                var tableKey = currentParams[ParamValue3];
                var entity = GetEntity(tableKey);
                var codeStr = GetBookValue(entity, sourceRow[refField], "Code");
                var nameStr = GetBookValue(entity, sourceRow[refField], "Name");
                destRow[colIndex] = String.Format("{0} {1}", codeStr, nameStr);
            }

            if (fieldType == ColumnOrgROFullData)
            {
                var entity = GetEntity(b_Org_RegisterOrgan.InternalKey);
                destRow[colIndex] = String.Format("{0} {1}",
                    GetBookValue(entity, sourceRow[t_Org_FounderOrg.RefOrgRO], b_Org_RegisterOrgan.Code),
                    GetBookValue(entity, sourceRow[t_Org_FounderOrg.RefOrgRO], b_Org_RegisterOrgan.Name));
            }

            if (fieldType == ColumnAddressPart)
            {
                var partNum = Convert.ToInt32(currentParams[ParamValue2]);
                var address = Convert.ToString(sourceRow[b_Org_EGRUL.Adress]);
                var adressParts = address.Split(';');
                var streetName = Convert.ToString(destRow[colIndex - 1]);
                var streetPos = -1;

                for (var i = 0; i < adressParts.Length; i++)
                {
                    if (String.Compare(adressParts[i].Trim(), streetName, true) == 0)
                    {
                        streetPos = i;
                    }
                }

                if (streetPos >= 0 && streetPos + partNum < adressParts.Length)
                {
                    destRow[colIndex] = adressParts[streetPos + partNum];
                }
            }

            if (fieldType == ColumnDocumentDescr)
            {
                var refKindName = currentParams[ParamValue2];
                var bookValue = GetBookValue(
                    GetEntity(b_Doc_KindPerson.InternalKey), 
                    sourceRow[refKindName], 
                    b_Doc_KindPerson.Name);
                var docType = String.Empty;

                if (bookValue.Length > 0)
                {
                    docType = String.Format("Документ:{0}", sourceRow[t_Org_FounderFL.DocNum]);
                }

                var docNum = String.Empty;

                if (!sourceRow.IsNull(t_Org_FounderFL.DocNum))
                {
                    docNum = String.Format("Серия номер документа:{0}", sourceRow[t_Org_FounderFL.DocNum]);
                }

                var docDate = String.Empty;

                if (!sourceRow.IsNull(t_Org_FounderFL.DateDoc))
                {
                    docDate = String.Format("Дата выдачи:{0}", sourceRow[t_Org_FounderFL.DocNum]);
                }

                var nameOrg = String.Empty;

                if (!sourceRow.IsNull(t_Org_FounderFL.NameOrg))
                {
                    nameOrg = String.Format("Орган выдавший документ:{0}", sourceRow[t_Org_FounderFL.NameOrg]);
                }

                var kodOrg = String.Empty;

                if (!sourceRow.IsNull(t_Org_FounderFL.KodOrg))
                {
                    kodOrg = String.Format("Код подразделения:{0}", sourceRow[t_Org_FounderFL.NameOrg]);
                }

                destRow[colIndex] = String.Format("{0} {1} {2} {3} {4}", docType, docNum, docDate, nameOrg, kodOrg).Trim();
            }

            if (fieldType == ColumnRegionName || fieldType == ColumnSettleName)
            {
                var refRegion = Convert.ToInt32(sourceRow[b_Org_EGRUL.RefRegion]);

                if (fieldType == ColumnRegionName && cacheRegionNames.ContainsKey(refRegion))
                {
                    destRow[colIndex] = cacheRegionNames[refRegion];
                    return;
                }

                if (fieldType == ColumnSettleName && cacheSettleNames.ContainsKey(refRegion))
                {
                    destRow[colIndex] = cacheSettleNames[refRegion];
                    return;
                }

                var tblPath = GetHierarchy(refRegion);
                var selectStr = String.Format("{0}={1} or {0}={2}", b_Regions_Bridge.RefTerrType, 4, 7);

                if (fieldType == ColumnSettleName)
                {
                    selectStr = String.Format("{0}={1} or {0}={2} or {0}={3}", b_Regions_Bridge.RefTerrType, 5, 6, 11);
                }

                var tblSelectRegion = DataTableUtils.FilterDataSet(tblPath, selectStr);

                if (tblSelectRegion.Rows.Count > 0)
                {
                    var rowRegion = tblSelectRegion.Rows[0];
                    var refTerr = Convert.ToInt32(rowRegion[b_Regions_Bridge.RefTerrType]);
                    var terrTypeName = GetBookValue(
                        GetEntity(fx_FX_TerritorialPartitionType.InternalKey),
                        refTerr,
                        fx_FX_TerritorialPartitionType.FullName);
                    var territoryName = Convert.ToString(rowRegion[b_Regions_Bridge.Name]);
                    territoryName = String.Format("{0} {1}", territoryName, terrTypeName.ToLower());

                    if (fieldType == ColumnRegionName)
                    {
                        cacheRegionNames.Add(refRegion, territoryName);
                    }
                    else
                    {
                        cacheSettleNames.Add(refRegion, territoryName);
                    }

                    destRow[colIndex] = territoryName;
                }
                else
                {
                    if (fieldType == ColumnRegionName)
                    {
                        cacheRegionNames.Add(refRegion, String.Empty);
                    }
                    else
                    {
                        cacheSettleNames.Add(refRegion, String.Empty);
                    }
                }
            }

            if (fieldType == ColumnFieldSequence)
            {
                var fieldList = currentParams[ParamValue2].Split(',');
                var joinIndex = Convert.ToInt32(currentParams[ParamValue3]);

                for (var i = 0; i < fieldList.Length; i++)
                {
                    destRow[colIndex + i] = sourceRow[GetJoinFieldName(joinIndex, fieldList[i])];                     
                }
            }

            if (fieldType == ColumnOrgAddress)
            {
                destRow[colIndex] = sourceRow[GetJoinFieldName(1, d_Org_Adress.NameNasPunkt)]; 
            }

            if (fieldType == ColumnJoinField)
            {
                var fieldName = currentParams[ParamValue2];
                destRow[colIndex] = sourceRow[GetJoinFieldName(1, fieldName)];                 
            }

            if (fieldType == ColumnOrgStatus)
            {
                var fieldName = currentParams[ParamValue2];
                destRow[colIndex] = GetBookValue(
                    GetEntity(b_Org_Status.InternalKey),
                    sourceRow[b_Org_EGRUL.RefOrgStatus],
                    fieldName);
            }

            if (fieldType == ColumnFaceData)
            {
                if (tblFaceData.Columns.Count == 0)
                {
                    tblFaceData.Columns.Add(t_Org_FaceProxy.RefOrgEGRUL, typeof(Int32));
                    tblFaceData.Columns.Add(t_Org_FaceProxy.FIO, typeof(String));
                    tblFaceData.Columns.Add(t_Org_FaceProxy.Job, typeof(String));
                }

                var faceData = GetRefDetailRows(t_Org_FaceProxy.InternalKey, id);
                var fieldName = currentParams[ParamValue2];

                if (fieldName == TempFieldNames.RowType)
                {
                    destRow[colIndex] = faceData.Length;
                    return;
                }

                if (faceData.Length == 1)
                {
                    destRow[colIndex] = faceData[0][fieldName];
                }
                else
                {
                    if (fieldName == t_Org_FaceProxy.FIO)
                    {
                        foreach (var dataRow in faceData)
                        {
                            var rowFace = tblFaceData.Rows.Add();
                            rowFace[0] = dataRow[t_Org_FaceProxy.RefOrgEGRUL];
                            rowFace[1] = dataRow[t_Org_FaceProxy.FIO];
                            rowFace[2] = dataRow[t_Org_FaceProxy.Job];
                        }
                    }
                }
            }

            if (fieldType == ColumnKOPFData)
            {
                var fieldName = currentParams[ParamValue2];
                var destValue = String.Empty;
                var successDetermine = false;
                var key = String.Empty;
                var refValue = -1;

                if (sourceRow[b_Org_EGRUL.RefOKOPFBridge] != DBNull.Value)
                {
                    refValue = Convert.ToInt32(sourceRow[b_Org_EGRUL.RefOKOPFBridge]);
                    successDetermine = refValue != -1;
                    key = b_OKOPF_Bridge.InternalKey;

                    if (successDetermine)
                    {
                        destValue = "ОКОПФ";
                    }
                }

                if (!successDetermine && sourceRow[b_Org_EGRUL.RefKOPFBridge] != DBNull.Value)
                {
                    refValue = Convert.ToInt32(sourceRow[b_Org_EGRUL.RefKOPFBridge]);
                    successDetermine = refValue != -1;
                    key = b_KOPF_Bridge.InternalKey;

                    if (successDetermine)
                    {
                        destValue = "КОПФ";
                    }
                }

                if (successDetermine && fieldName != KOPFTemplate)
                {
                    destValue = GetBookValue(GetEntity(key), refValue, fieldName);
                }

                destRow[colIndex] = destValue;
            }

            if (fieldType == ColumnOrgMNSData)
            {
                var fieldName = currentParams[ParamValue2];
                destRow[colIndex] = GetBookValue(
                    GetEntity(b_Org_MNS.InternalKey), 
                    sourceRow[b_Org_EGRUL.RefOrgMNS],
                    fieldName);
            }

            if (fieldType == ColumnOrgROData)
            {
                var fieldName = currentParams[ParamValue2];
                destRow[colIndex] = GetBookValue(
                    GetEntity(b_Org_RegisterOrgan.InternalKey),
                    sourceRow[b_Org_EGRUL.RefOrgRO],
                    fieldName);
            }

            if (fieldType == ColumnRegTypeStartData)
            {
                var fieldName = currentParams[ParamValue2];
                destRow[colIndex] = GetBookValue(
                    GetEntity(b_Types_Register.InternalKey),
                    sourceRow[b_Org_EGRUL.RefTypeRegStart],
                    fieldName);
            }

            if (fieldType == ColumnRegTypeFinishData)
            {
                var fieldName = currentParams[ParamValue2];
                destRow[colIndex] = GetBookValue(
                    GetEntity(b_Types_Register.InternalKey),
                    sourceRow[b_Org_EGRUL.RefTypeRegFinish],
                    fieldName);
            }

            if (fieldType == ColumnOrgROStartData)
            {
                var fieldName = currentParams[ParamValue2];
                destRow[colIndex] = GetBookValue(
                    GetEntity(b_Org_RegisterOrgan.InternalKey),
                    sourceRow[b_Org_EGRUL.RefOrgROStart],
                    fieldName);
            }

            if (fieldType == ColumnOrgROFinishData)
            {
                var fieldName = currentParams[ParamValue2];
                destRow[colIndex] = GetBookValue(
                    GetEntity(b_Org_RegisterOrgan.InternalKey),
                    sourceRow[b_Org_EGRUL.RefOrgROFinish],
                    fieldName);
            }

            if (fieldType == ColumnOrgKindCapitalData)
            {
                var fieldName = currentParams[ParamValue2];
                destRow[colIndex] = GetBookValue(
                    GetEntity(b_Org_KindCapital.InternalKey),
                    sourceRow[b_Org_EGRUL.RefOrgKindCapital],
                    fieldName);
            }

            // масочка вывода данных по оквэд
            if (fieldType == ColumnOKVEDMaskCode)
            {
                var fieldCode = currentParams[ParamValue2];
                destRow[colIndex] = TextMasking(new[] { 2, 2, 2 }, sourceRow[fieldCode]);
            }

            if (fieldType == ColumnDetailText)
            {
                var entityKey = currentParams[ParamValue2];
                var fieldName = currentParams[ParamValue3];
                var splitter = currentParams[ParamValue4];

                if (cacheData.ContainsKey(entityKey))
                {
                    var dataList = cacheData[entityKey].Select(String.Format("{0}={1}", GetParentRefName(), id));
                    var strResult = String.Empty;
                    var mask = new[] {2, 2, 2};

                    foreach (var dataRow in dataList)
                    {
                        strResult = String.Join(splitter, new[] { strResult, TextMasking(mask, dataRow[fieldName]) });
                    }

                    destRow[colIndex] = strResult.Trim(splitter[0]).Trim();
                }
            }

            if (fieldType == ColumnOKATOMask)
            {
                destRow[colIndex] = TextMasking(new[] { 2, 3, 3, 3 }, sourceRow[b_Org_EGRUL.OKATO]);
            }

            if (fieldType == ColumnDetailSequence)
            {
                var entityKey = currentParams[ParamValue2];
                var fieldCount = Convert.ToInt32(currentParams[ParamValue3]);

                if (cacheData.ContainsKey(entityKey))
                {
                    var dataList = cacheData[entityKey].Select(String.Format("{0}={1}", GetParentRefName(), id));
                    var diffRowCount = dataList.Length - rowsDetail.Count;

                    if (diffRowCount > 0)
                    {
                        for (var i = 0; i < diffRowCount; i++)
                        {
                            rowsDetail.Add(destRow.Table.Rows.Add());
                        }
                    }

                    var rowCounter = 0;

                    foreach (var dataRow in dataList)
                    {
                        var rowData = rowsDetail[rowCounter++];

                        for (var i = 0; i < fieldCount; i++)
                        {
                            rowData[colIndex + i] = dataRow[i];  
                        }
                    }
                }
            }

            if (fieldType == ColumnCutContactInfo)
            {
                var fieldName = currentParams[ParamValue2];

                if (sourceRow[fieldName] != DBNull.Value)
                {
                    var contactInfo = Convert.ToString(sourceRow[fieldName]);
                    destRow[colIndex] = contactInfo
                        .Replace("Телефон", "т.")
                        .Replace("телефон", "т.")
                        .Replace("тел", "т.")
                        .Replace("Факс", "ф.")
                        .Replace("факс", "ф.");
                }
            }
        }

        private string TextMasking(IList<int> maskParts, object value)
        {
            if (value != DBNull.Value && maskParts != null)
            {
                var codeStr = Convert.ToString(value);
                var parts = new string[maskParts.Count];
                var currentLength = 0;
                var maskTemplate = String.Empty;

                for (var i = 0; i < maskParts.Count; i++)
                {
                    if (codeStr.Length >= currentLength + maskParts[i])
                    {
                        parts[i] = codeStr.Substring(currentLength, maskParts[i]);
                    }

                    currentLength += maskParts[i];
                    maskTemplate += "{" + i + "}.";
                }

                return String.Format(maskTemplate.Trim('.'), parts);
            }

            return String.Empty;
        }

        private DataRow[] GetRefDetailRows(string key, int id)
        {
            var detailEntity = GetEntity(key);
            return GetTableRows(detailEntity, id, true, String.Empty);
        }

        private DataTable GetRegionTable()
        {
            if (cacheRegions.Rows.Count == 0)
            {
                var dbHelper = new ReportDBHelper(scheme);
                var dt = dbHelper.GetEntityData(b_Regions_Bridge.InternalKey);
                cacheRegions = dt;
            }

            return cacheRegions;
        }

        private DataTable GetHierarchy(object id)
        {
            const string filter = "{0} = {1}";
            const string keyField = b_Regions_Bridge.ID;
            const string refField = b_Regions_Bridge.ParentID;
            var tblRegions = GetRegionTable();
            var tblResult = tblRegions.Clone();
            var tblRecord = DataTableUtils.FilterDataSet(tblRegions, String.Format(filter, keyField, id));
            
            while (tblRecord.Rows.Count > 0)
            {
                var rowData = tblRecord.Rows[0];
                tblResult.ImportRow(rowData);
                
                if (rowData[refField] != DBNull.Value)
                {
                    var parentId = Convert.ToInt32(rowData[refField]);
                    tblRecord = DataTableUtils.FilterDataSet(tblRegions, String.Format(filter, keyField, parentId));
                }
                else
                {
                    tblRecord.Rows.Clear();
                }
            }
            
            return tblResult;
        }
    }
}

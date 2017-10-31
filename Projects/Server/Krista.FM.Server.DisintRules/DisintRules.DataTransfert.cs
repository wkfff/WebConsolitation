using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DisintRules
{
    public partial class DisintRules
    {
        /// <summary>
        /// перенос данных по конкретному году
        /// </summary>
        /// <returns></returns>
        public int[] DataTransfert(int oldYear, int newYear)
        {
            // получаем все данные по году 
            // ищем все подчиненные записи
            // получаем по новому году код KD
            // сохраняем новые записи
            return CopyBKNormatives(oldYear, newYear);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int[] DataTransfert(int newYear, Dictionary<int, int> selectedRows)
        {
            // получаем все данные по ID 
            // ищем все подчиненные записи
            // получаем по новому году код KD
            // сохраняем новые записи
            return CopyBKNormatives(newYear, selectedRows);
        }
        /*
        private IEntity _analizKD;
        private IEntity AnalizKD
        {
            get
            {
                if (_analizKD == null)
                    _analizKD = activeScheme.Classifiers[d_KD_Analysis];
                return _analizKD;
            }
        }*/
        /*
        private IEntity _analizRegion;
        private IEntity AnalizRegion
        {
            get
            {
                if (_analizRegion == null)
                    _analizRegion = activeScheme.Classifiers[d_Regions_Analysis];
                return _analizRegion;
            }
        }
        */
        /// <summary>
        /// получение и копирование номативов по году
        /// </summary>
        /// <param name="oldYear"></param>
        /// <param name="newYear"></param>
        /// <returns></returns>
        private int[] CopyBKNormatives(int oldYear, int newYear)
        {
            if (oldYear == newYear)
                return new int[5];
            // получение нормативов и копий таблиц для новых данных
            DataTable normativesBK = GetNormatives(NormativesKind.NormativesBK);
            DataTable newNormativesBK = normativesBK.Clone();
            DataTable normativesRF = GetNormatives(NormativesKind.NormativesRegionRF);
            DataTable newNormativesRF = normativesRF.Clone();
            DataTable normativesMR = GetNormatives(NormativesKind.NormativesMR);
            DataTable newNormativesMR = normativesMR.Clone();
            DataTable normativesVarRF = GetNormatives(NormativesKind.VarNormativesRegionRF);
            DataTable newNormativesVarRF = normativesVarRF.Clone();
            DataTable normativesVarMR = GetNormatives(NormativesKind.VarNormativesMR);
            DataTable newNormativesVarMR = normativesVarMR.Clone();

            IEntity normativeEnttity = GetEntityObjectByName(NormativesKind.NormativesBK);

            int kdSource = GetDataSourceByYear(newYear);
            int regionSource = kdSource;
            DataRow[] rows = normativesBK.Select(string.Format("RefYearDayUNV = {0}", oldYear));

            DataTable dtNewKD = GetEmptyDataTable(GetKDAnalysis);
            DataTable dtNewRegions = GetEmptyDataTable(GetRegionsAnalysis);

            foreach (DataRow row in rows)
            {
                CopyChildNormative(row, newYear, NormativesKind.NormativesRegionRF, normativesRF,
                    ref newNormativesRF, ref dtNewKD);
                CopyChildNormative(row, newYear, NormativesKind.NormativesMR, normativesMR,
                    ref newNormativesMR, ref dtNewKD);
                CopyChildNormative(row, newYear, NormativesKind.VarNormativesRegionRF, normativesVarRF,
                    ref newNormativesVarRF, ref dtNewKD, ref dtNewRegions);
                CopyChildNormative(row, newYear, NormativesKind.VarNormativesMR, normativesVarMR,
                    ref newNormativesVarMR, ref dtNewKD, ref dtNewRegions);

                GetKD(row["RefKD"], ref dtNewKD);
                CopyNormativeRow(row, normativeEnttity, newYear, ref newNormativesBK);


            }
            // получаем или добавляем записи по источнику в классификаторы
            Dictionary<int, int> kdReferences = GetNewClassifiersRef(kdSource, dtNewKD, d_KD_Analysis);
            Dictionary<int, int> regionReferences = GetNewClassifiersRef(regionSource, dtNewRegions, d_Regions_Analysis);
            // удаляем записи дубликаты, если такие уже есть в нормативах
            DeleteDublicates(newYear, kdReferences, normativesBK, ref newNormativesBK);
            DeleteDublicates(newYear, kdReferences, normativesRF, ref newNormativesRF);
            DeleteDublicates(newYear, kdReferences, normativesMR, ref newNormativesMR);
            DeleteDublicates(newYear, kdReferences, normativesVarRF, ref newNormativesVarRF);
            DeleteDublicates(newYear, kdReferences, normativesVarMR, ref newNormativesVarMR);
            // проставляем ссылки на новые записи в классификаторах
            SetReferenceKD(kdReferences, ref newNormativesBK);
            SetReferenceKD(kdReferences, ref newNormativesRF);
            SetReferenceKD(kdReferences, ref newNormativesMR);
            SetReferenceKD(kdReferences, ref newNormativesVarRF);
            SetReferenceKD(kdReferences, ref newNormativesVarMR);

            SetReferenceRegions(regionReferences, ref newNormativesVarRF);
            SetReferenceRegions(regionReferences, ref newNormativesVarMR);
            // количество записей, скопированных в каждый из нормативов
            int[] copyRowsCount = new int[6];
            if (newNormativesBK.Rows.Count > 0)
            {
                copyRowsCount[0] += newNormativesBK.Rows.Count;
                copyRowsCount[1] = newNormativesBK.Rows.Count;
                ApplyChanges(NormativesKind.NormativesBK, newNormativesBK);
            }
            if (newNormativesRF.Rows.Count > 0)
            {
                copyRowsCount[0] += newNormativesRF.Rows.Count;
                copyRowsCount[2] = newNormativesRF.Rows.Count;
                ApplyChanges(NormativesKind.NormativesRegionRF, newNormativesRF);
            }
            if (newNormativesMR.Rows.Count > 0)
            {
                copyRowsCount[0] += newNormativesMR.Rows.Count;
                copyRowsCount[3] = newNormativesMR.Rows.Count;
                ApplyChanges(NormativesKind.NormativesMR, newNormativesMR);
            }
            if (newNormativesVarRF.Rows.Count > 0)
            {
                copyRowsCount[0] += newNormativesVarRF.Rows.Count;
                copyRowsCount[4] = newNormativesVarRF.Rows.Count;
                ApplyChanges(NormativesKind.VarNormativesRegionRF, newNormativesVarRF);
            }
            if (newNormativesVarMR.Rows.Count > 0)
            {
                copyRowsCount[0] += newNormativesVarMR.Rows.Count;
                copyRowsCount[5] = newNormativesVarMR.Rows.Count;
                ApplyChanges(NormativesKind.VarNormativesMR, newNormativesVarMR);
            }
            return copyRowsCount;
        }

        /// <summary>
        /// получение и копирование нормативов по выделеным записям
        /// </summary>
        private int[] CopyBKNormatives(int newYear, Dictionary<int, int> selectedRows)
        {
            DataTable normativesBK = GetNormatives(NormativesKind.NormativesBK);
            DataTable newNormativesBK = normativesBK.Clone();
            DataTable normativesRF = GetNormatives(NormativesKind.NormativesRegionRF);
            DataTable newNormativesRF = normativesRF.Clone();
            DataTable normativesMR = GetNormatives(NormativesKind.NormativesMR);
            DataTable newNormativesMR = normativesMR.Clone();
            DataTable normativesVarRF = GetNormatives(NormativesKind.VarNormativesRegionRF);
            DataTable newNormativesVarRF = normativesVarRF.Clone();
            DataTable normativesVarMR = GetNormatives(NormativesKind.VarNormativesMR);
            DataTable newNormativesVarMR = normativesVarMR.Clone();
            IEntity normativeEnttity = GetEntityObjectByName(NormativesKind.NormativesBK);

            int kdSource = GetDataSourceByYear(newYear);
            int regionSource = kdSource;

            // таблицы с новыми нормативами
            // таблицы с новыми кодами КД и районами
            DataTable dtNewKD = GetEmptyDataTable(GetKDAnalysis);
            DataTable dtNewRegions = GetEmptyDataTable(GetRegionsAnalysis);
            
            // получаем копии нормативов с новыми ID записей
            foreach (KeyValuePair<int, int> kvp in selectedRows)
            {
                DataRow[] rows = normativesBK.Select(string.Format("RefKD = {0} and RefYearDayUNV = {1}", kvp.Key, kvp.Value));
                foreach (DataRow row in rows)
                {
                    if (newYear != kvp.Value)
                    {
                        CopyChildNormative(row, newYear, NormativesKind.NormativesRegionRF, normativesRF,
                            ref newNormativesRF, ref dtNewKD);
                        CopyChildNormative(row, newYear, NormativesKind.NormativesMR, normativesMR,
                            ref newNormativesMR, ref dtNewKD);
                        CopyChildNormative(row, newYear, NormativesKind.VarNormativesRegionRF, normativesVarRF,
                            ref newNormativesVarRF, ref dtNewKD, ref dtNewRegions);
                        CopyChildNormative(row, newYear, NormativesKind.VarNormativesMR, normativesVarMR,
                            ref newNormativesVarMR, ref dtNewKD, ref dtNewRegions);
                        GetKD(row["RefKD"], ref dtNewKD);
                        CopyNormativeRow(row, normativeEnttity, newYear, ref newNormativesBK);
                    }
                }
            }
            Dictionary<int, int> kdReferences = GetNewClassifiersRef(kdSource, dtNewKD, d_KD_Analysis);
            Dictionary<int, int> regionReferences = GetNewClassifiersRef(regionSource, dtNewRegions, d_Regions_Analysis);
            // удаляем записи дубликаты, если такие уже есть в нормативах
            DeleteDublicates(newYear, kdReferences, normativesBK, ref newNormativesBK);
            DeleteDublicates(newYear, kdReferences, normativesRF, ref newNormativesRF);
            DeleteDublicates(newYear, kdReferences, normativesMR, ref newNormativesMR);
            DeleteDublicates(newYear, kdReferences, normativesVarRF, ref newNormativesVarRF);
            DeleteDublicates(newYear, kdReferences, normativesVarMR, ref newNormativesVarMR);
            // проставляем ссылки на новые записи в классификаторах
            if (kdReferences.Count != 0)
            {
                SetReferenceKD(kdReferences, ref newNormativesBK);
                SetReferenceKD(kdReferences, ref newNormativesRF);
                SetReferenceKD(kdReferences, ref newNormativesMR);
                SetReferenceKD(kdReferences, ref newNormativesVarRF);
                SetReferenceKD(kdReferences, ref newNormativesVarMR);
            }
            if (regionReferences.Count != 0)
            {
                SetReferenceRegions(regionReferences, ref newNormativesVarRF);
                SetReferenceRegions(regionReferences, ref newNormativesVarMR);
            }

            // количество записей, скопированных в каждый из нормативов
            int[] copyRowsCount = new int[6];
            if (newNormativesBK.Rows.Count > 0)
            {
                copyRowsCount[0] += newNormativesBK.Rows.Count;
                copyRowsCount[1] = newNormativesBK.Rows.Count;
                ApplyChanges(NormativesKind.NormativesBK, newNormativesBK);
            }
            if (newNormativesRF.Rows.Count > 0)
            {
                copyRowsCount[0] += newNormativesRF.Rows.Count;
                copyRowsCount[2] = newNormativesRF.Rows.Count;
                ApplyChanges(NormativesKind.NormativesRegionRF, newNormativesRF);
            }
            if (newNormativesMR.Rows.Count > 0)
            {
                copyRowsCount[0] += newNormativesMR.Rows.Count;
                copyRowsCount[3] = newNormativesMR.Rows.Count;
                ApplyChanges(NormativesKind.NormativesMR, newNormativesMR);
            }
            if (newNormativesVarRF.Rows.Count > 0)
            {
                copyRowsCount[0] += newNormativesVarRF.Rows.Count;
                copyRowsCount[4] = newNormativesVarRF.Rows.Count;
                ApplyChanges(NormativesKind.VarNormativesRegionRF, newNormativesVarRF);
            }
            if (newNormativesVarMR.Rows.Count > 0)
            {
                copyRowsCount[0] += newNormativesVarMR.Rows.Count;
                copyRowsCount[5] = newNormativesVarMR.Rows.Count;
                ApplyChanges(NormativesKind.VarNormativesMR, newNormativesVarMR);
            }

            return copyRowsCount;
        }

        /// <summary>
        /// gj
        /// </summary>
        private void CopyChildNormative(DataRow parentRow, int newYear, NormativesKind normative,
            DataTable normatives, ref DataTable dtNormative, ref DataTable dtKD)
        {
            // объект схемы, соответствующий нормативу
            IEntity normativeEnttity = GetEntityObjectByName(normative);

            DataRow[] rows = normatives.Select(string.Format("RefYearDayUNV = {0} and RefKD = {1}",
                parentRow["RefYearDayUNV"], parentRow["RefKD"]));
            foreach (DataRow row in rows)
            {
                // для каждой записи норматива получаем соответсвующие записи в классификаторах данных или просто их добавляем, если нет
                GetKD(row["RefKD"], ref dtKD);

                CopyNormativeRow(row, normativeEnttity, newYear, ref dtNormative);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CopyChildNormative(DataRow parentRow, int newYear, NormativesKind normative,
            DataTable normatives, ref DataTable dtNormative, ref DataTable dtKD, ref DataTable dtRegions)
        {
            // объект схемы, соответствующий нормативу
            IEntity normativeEnttity = GetEntityObjectByName(normative);

            DataRow[] rows = normatives.Select(string.Format("RefYearDayUNV = {0} and RefKD = {1}",
                parentRow["RefYearDayUNV"], parentRow["RefKD"]));
            foreach (DataRow row in rows)
            {
                // для каждой записи норматива получаем соответсвующие записи в классификаторах данных или просто их добавляем, если нет
                GetKD(row["RefKD"], ref dtKD);
                GetRegionBySource(row["RefRegions"], ref dtRegions);

                CopyNormativeRow(row, normativeEnttity, newYear, ref dtNormative);
            }
        }

        private void CopyNormativeRow(DataRow normativeRow, IEntity normativeEntity,
            int newYear, ref DataTable copyTable)
        {
            DataRow newRow = copyTable.NewRow();
            newRow.ItemArray = normativeRow.ItemArray;
            for (int i = 1; i <= 17; i++)
            {
                if (copyTable.Columns.Contains(i.ToString()))
                    newRow[i.ToString()] = DBNull.Value;// normativeEntity.GetGeneratorNextValue;
            }
            newRow["RefYearDayUNV"] = newYear;
            copyTable.Rows.Add(newRow);
        }

        /// <summary>
        /// получение источника за определенный год в классификаторе КД.Анализ
        /// если такого нет, то добавляем
        /// </summary>
        /// <param name="year"></param>
        /// <param name="normativeEnttity"></param>
        /// <returns></returns>
        private int GetDataSourceByYear(int year)
        {
            IDataSource ds;
            // ничего не нашли, добавляем источник
            ds = activeScheme.DataSourceManager.DataSources.CreateElement();
            ds.SupplierCode = "ФО";
            ds.Year = year;
            ds.ParametersType = ParamKindTypes.Year;
            ds.DataName = "Анализ данных";
            ds.DataCode = "6";
            if (!activeScheme.DataSourceManager.DataSources.Contains(ds))
                return activeScheme.DataSourceManager.DataSources.Add(ds);

            return Convert.ToInt32(activeScheme.DataSourceManager.DataSources.FindDataSource(ds));
        }

        /// <summary>
        /// получаем основные параметры записи классификатора КД.Анализ
        /// </summary>
        /// <param name="kdID"></param>
        /// <param name="dtKD"></param>
        private void GetKD(object kdID, ref DataTable dtKD)
        {
            using (IDatabase db = activeScheme.SchemeDWH.DB)
            {
                IDbDataParameter[] queryParams = new IDbDataParameter[1];
                queryParams[0] = db.CreateParameter("ID", kdID);
                string query = "select ID, SourceID, CodeStr, Name from d_KD_Analysis where ID = ?";
                dtKD.Merge((DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, queryParams));
            }
        }

        /// <summary>
        /// получаем основные параметры записи классификатора Регионы.Анализ
        /// </summary>
        /// <param name="regionID"></param>
        /// <param name="dtRegions"></param>
        private void GetRegionBySource(object regionID, ref DataTable dtRegions)
        {
            using (IDatabase db = activeScheme.SchemeDWH.DB)
            {
                IDbDataParameter[] queryParams = new IDbDataParameter[1];
                queryParams[0] = db.CreateParameter("ID", regionID);
                string query = "select ID, SourceID, Code, Name from d_Regions_Analysis where ID = ?";
                dtRegions.Merge((DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, queryParams));
            }
        }

        private void SetReferenceKD(Dictionary<int, int> references, ref DataTable newNormative)
        {
            foreach (DataRow row in newNormative.Rows)
            {
                // меняем ссылки на новые
                if (references.ContainsKey(Convert.ToInt32(row["RefKD"])))
                    row["RefKD"] = references[Convert.ToInt32(row["RefKD"])];
            }
        }

        private void SetReferenceRegions(Dictionary<int, int> references, ref DataTable newNormative)
        {
            foreach (DataRow row in newNormative.Rows)
            {
                if (references.ContainsKey(Convert.ToInt32(row["RefRegions"])))
                    row["RefRegions"] = references[Convert.ToInt32(row["RefRegions"])];
            }
        }


        private Dictionary<int, int> GetNewClassifiersRef(object sourceID, DataTable classifiersRows,
            string classifierName)
        {
            using(IDatabase db = activeScheme.SchemeDWH.DB)
            {
                IEntity classifier = activeScheme.Classifiers[classifierName];
                Dictionary<int, int> classifiersNewIDs = new Dictionary<int, int>();
                string codeCoulumnName = classifier.FullName.Contains("KD") ? "CodeStr" : "Code";
                foreach (DataRow classifierRow in classifiersRows.Rows)
                {
                    string query = string.Format("select ID from {0} where {1} = ? and Name = ? and SourceID = {2}",
                        classifier.FullDBName, codeCoulumnName, sourceID);
                    IDbDataParameter[] queryParams = new IDbDataParameter[2];
                    queryParams[0] = db.CreateParameter(codeCoulumnName, classifierRow[codeCoulumnName]);
                    queryParams[1] = db.CreateParameter("Name", classifierRow["Name"]);
                    object rowID = db.ExecQuery(query, QueryResultTypes.Scalar, queryParams);
                    if (!classifiersNewIDs.ContainsKey(Convert.ToInt32(classifierRow["ID"])))
                    {
                        if (rowID == null || rowID == DBNull.Value)
                        {
                            // для разных классификаторов разный набор параметров
                            //queryParams = new IDbDataParameter[classifier.Attributes.Count];
                            List<IDbDataParameter> qParams = new List<IDbDataParameter>();
                            List<string> columnsNames = new List<string>();
                            List<string> signs = new List<string>();
                            foreach (IDataAttribute attr in classifier.Attributes.Values)
                            {
                                IDbDataParameter param = null;
                                switch (attr.Name)
                                {
                                    case "ID":
                                        int id = classifier.GetGeneratorNextValue;
                                        param = db.CreateParameter(attr.Name, id);
                                        classifiersNewIDs.Add(Convert.ToInt32(classifierRow["ID"]), id);
                                        break;
                                    case "SourceID":
                                        param = db.CreateParameter(attr.Name, sourceID);
                                        break;
                                    case "Name":
                                        param = db.CreateParameter(attr.Name, classifierRow["Name"]);
                                        break;
                                    default:
                                        if (attr.Name == codeCoulumnName)
                                            param = db.CreateParameter(attr.Name, classifierRow[codeCoulumnName]);
                                        else
                                        {
                                            if (attr.DefaultValue != null)
                                                param = db.CreateParameter(attr.Name, attr.DefaultValue);
                                        }
                                        break;
                                }
                                if (param != null)
                                {
                                    qParams.Add(param);

                                    columnsNames.Add(attr.Name);
                                    signs.Add("?");
                                }
                            }
                            query = string.Format("insert into {0} ({1}) values ({2})",
                                classifier.FullDBName, string.Join(", ", columnsNames.ToArray()), string.Join(", ", signs.ToArray()));
                            db.ExecQuery(query, QueryResultTypes.NonQuery, qParams.ToArray());
                        }
                        else
                            classifiersNewIDs.Add(Convert.ToInt32(classifierRow["ID"]), Convert.ToInt32(rowID));
                    }
                }
                return classifiersNewIDs;
            }
        }
        // зпрос на получение основных данных по таблице КД
        private const string GetKDAnalysis = "select ID, SourceID, CodeStr, Name from d_KD_Analysis";
        // запрос на получение данных по таблице Районы
        private const string GetRegionsAnalysis = "select ID, SourceID, Code, Name from d_Regions_Analysis";
        /// <summary>
        /// получение пустой таблицы по запросу
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private DataTable GetEmptyDataTable(string query)
        {
            using (IDatabase db = activeScheme.SchemeDWH.DB)
            {
                string filter = " where 1 = 2";
                return (DataTable)db.ExecQuery(query + filter, QueryResultTypes.DataTable);
            }
        }

        private void DeleteDublicates(int newYear, Dictionary<int, int> references, DataTable normatives, ref DataTable newNormative)
        {
            foreach (KeyValuePair<int, int> kvp in references)
            {
                // ищем в нормативах те записи, которые хотим скопировать по кд и году
                DataRow[] rows = normatives.Select(string.Format("RefKD = {0} and RefYearDayUNV = {1}", kvp.Value, newYear));
                // записи, которые хотим скопировать
                DataRow[] newRows = newNormative.Select(string.Format("RefKD = {0} and RefYearDayUNV = {1}", kvp.Key, newYear));
                if (rows.Length > 0)
                    foreach (DataRow row in newRows)
                        row.Delete();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Krista.FM.Server.OLAP.BatchOperations.BatchOperationFO28;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.OLAP.BatchOperations
{
    public abstract class BatchOperationFO28Base : BatchOperationAbstract
    {
        protected IScheme scheme;

        protected BatchOperationFO28Base(Guid batchId, IMDProcessingProtocol protocol, IScheme scheme)
            : base(batchId, protocol)
        {
            this.scheme = scheme;
        }

        private DataTable FO28QueryAuxCubeTableData(string masterQuery, string detailQuery, string masterName,
            string detailName, string refMasterFieldName)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                string query =
                    string.Format(
                        "select {0}, {1} from {2} m left join {3} d on (d.{4} = m.id) where not (d.sum is null) and (d.sum <> 0)",
                        masterQuery, detailQuery, masterName, detailName, refMasterFieldName);
                return
                    (DataTable)
                    db.ExecQuery(query, QueryResultTypes.DataTable, new IDbDataParameter[] { });
            }
        }

        private void FO28PumpAuxCubeTableData(IDataUpdater du, ref DataSet ds, IEntity obj, DataTable sourceTable)
        {
            List<string> columnNames = new List<string>();
            try
            {
                int columnCount = sourceTable.Columns.Count;
                for (int i = 0; i < columnCount; i++)
                    columnNames.Add(sourceTable.Columns[i].ColumnName.ToUpper());
                foreach (DataRow sourceRow in sourceTable.Rows)
                {
                    DateTime dateTime = Convert.ToDateTime(sourceRow["REFYEARDAYUNV"]);
                    int date = dateTime.Year * 10000 + dateTime.Month * 100 + dateTime.Day;
                    if (date > 20201231)
                        continue;
                    string num = string.Format("{0} {1} {2}", sourceRow["NUM"], sourceRow["FACTDATE"], sourceRow["SUM"]);
                    DataRow newRow = ds.Tables[0].NewRow();
                    for (int i = 0; i < columnCount; i++)
                    {
                        string columnName = columnNames[i];
                        if (columnName == "REFYEARDAYUNV")
                        {
                            newRow[columnName] = date;
                            continue;
                        }
                        if (columnName == "NUM")
                        {
                            newRow[columnName] = num;
                            continue;
                        }
                        if ((columnName == "SUM") || (columnName == "FACTDATE"))
                            continue;
                        newRow[columnName] = sourceRow[columnName];
                    }
                    newRow["TaskId"] = -1;
                    ds.Tables[0].Rows.Add(newRow);
                }
                du.Update(ref ds);
            }
            finally
            {
                columnNames.Clear();
            }
        }

        // сформировать служебную таблицу, на основе которой будет строиться куб
        // detailMapping - массив пар: список, запрашиваемых у детали полей - имя таблицы детали
        protected void FO28FormAuxCubeTable(IEntity fctAuxCubeTable, string masterName, string masterQuery,
            string[] detailMapping, string refMasterFieldName)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                string queryDel = string.Format("delete from {0}", fctAuxCubeTable.FullDBName);
                db.ExecQuery(queryDel, QueryResultTypes.NonQuery);
                IDataUpdater duAuxCubeTable = fctAuxCubeTable.GetDataUpdater();
                DataSet dsAuxCubeTable = new DataSet();
                duAuxCubeTable.Fill(ref dsAuxCubeTable);
                try
                {
                    int count = detailMapping.GetLength(0);
                    for (int i = 0; i < count; i += 2)
                    {
                        DataTable dt = FO28QueryAuxCubeTableData(masterQuery, detailMapping[i],
                                                                 masterName, detailMapping[i + 1], refMasterFieldName);
                        try
                        {
                            FO28PumpAuxCubeTableData(duAuxCubeTable, ref dsAuxCubeTable, fctAuxCubeTable, dt);
                        }
                        finally
                        {
                            dt.Clear();
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new FO28BaseException(e.Message);
                }
                finally
                {
                    dsAuxCubeTable.Clear();
                    dsAuxCubeTable.Dispose();
                }
            }
        }

    }
}

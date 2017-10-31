using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Common.OfficeHelpers;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Common.ReportHelpers
{
    public class OfficeReportsHelper : ReportsHelper
    {
        private readonly IScheme scheme;
        private readonly IOfficeApplication officeWrapper;

        internal OfficeReportsHelper(IScheme scheme, IOfficeApplication officeWrapper)
        {
            this.scheme = scheme;
            this.officeWrapper = officeWrapper;
        }

        public override void CreateReport(object reportParam, string templateName)
        {
            officeWrapper.CreateAsTemplate(templateName);
            
            // получили запросы
            Dictionary<int, string> sqlQueries = GetSqlDefinition();
            // получаем некоторые параметры
            //int paramsCounter = 0;
            foreach (KeyValuePair<int, string> kvp in sqlQueries)
            {
                // выполняем отдельный запрос, получаем данные, передаем эти данные в макрос методу,
                // который должен заполнять
                ADODB.Recordset rs;
                List<IDbDataParameter> queryParams = new List<IDbDataParameter>();
                using (IDatabase db = scheme.SchemeDWH.DB)
                {
                    string query = kvp.Value;
                    queryParams.Add(new System.Data.OleDb.OleDbParameter("p0", reportParam));

                    DataTable dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, queryParams.ToArray());
                    rs = DataTableConverter.ConvertToRecordset(dt);
                }
                officeWrapper.RunMacros(string.Format("GetRecordSet{0}", kvp.Key + 1), rs);
            }
            officeWrapper.RunMacros("FillReport");

            officeWrapper.Visible = true;
        }

        public override void CreateReport(string templateName, DataTable dtReportData)
        {
            officeWrapper.CreateAsTemplate(templateName);
            ADODB.Recordset rs = DataTableConverter.ConvertToRecordset(dtReportData);
            officeWrapper.RunMacros("GetRecordSet", rs);
            officeWrapper.RunMacros("FillReport");
            officeWrapper.Visible = true;
        }

        public override OfficeDocument CreateReport(string templateName, DataTable[] tables)
        {
            OfficeDocument document = officeWrapper.CreateAsTemplate(templateName);
            int tableIndex = 1;
            foreach (DataTable table in tables)
            {
                ADODB.Recordset rs = DataTableConverter.ConvertToRecordset(table);
                officeWrapper.RunMacros(string.Format("GetRecordSet{0}", tableIndex), rs);
                tableIndex++;
            }
            officeWrapper.RunMacros("FillReport");
            return document;
        }

        public override void ShowReport()
        {
            officeWrapper.Visible = true;
        }

        public override void Save(string fileName)
        {
            officeWrapper.SaveChanges(null, fileName);
        }

        public override void Quit()
        {
            officeWrapper.Quit();
        }

        private Dictionary<int, string> GetSqlDefinition()
        {
            // получаем некие запросы из макроса. 
            // они будут в одной строке с разделителем между запросами ';'
            Dictionary<int, string> sqlQueries = new Dictionary<int, string>();
            string queriesStr = officeWrapper.RunMacros("GetSQLDefinition").ToString();

            string[] queries = queriesStr.Split(';');

            // теперь отсеем "левые" запросы
            int counter = 0;
            foreach (string str in queries)
            {
                // запишем запросы по их порядковым номерам
                sqlQueries.Add(counter, str);
                counter++;
            }
            return sqlQueries;
        }

        private IDbDataParameter CreateDbParam(IDatabase db, ADODB.DataTypeEnum adoParamType, string paramName, object paramValue)
        {
            IDbDataParameter param = null;
            switch (adoParamType)
            {
                case ADODB.DataTypeEnum.adBoolean:
                    param = db.CreateParameter(paramName, paramValue, DbType.Boolean);
                    break;
                case ADODB.DataTypeEnum.adBSTR:
                    param = db.CreateParameter(paramName, paramValue, DbType.String);
                    break;
                case ADODB.DataTypeEnum.adChar:
                    param = db.CreateParameter(paramName, paramValue, DbType.Byte);
                    break;
                case ADODB.DataTypeEnum.adDate:
                    param = db.CreateParameter(paramName, paramValue, DbType.Date);
                    break;
                case ADODB.DataTypeEnum.adDecimal:
                    param = db.CreateParameter(paramName, paramValue, DbType.Decimal);
                    break;
                case ADODB.DataTypeEnum.adDouble:
                    param = db.CreateParameter(paramName, paramValue, DbType.Double);
                    break;
                case ADODB.DataTypeEnum.adGUID:
                    param = db.CreateParameter(paramName, paramValue, DbType.Guid);
                    break;
                case ADODB.DataTypeEnum.adInteger:
                    param = db.CreateParameter(paramName, paramValue, DbType.Int64);
                    break;
                default:
                    // не знаем какой параметр, поэтому все руками делаем. Выбор параметра через какой нибудь мастер
                    break;
            }
            return param;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                officeWrapper.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

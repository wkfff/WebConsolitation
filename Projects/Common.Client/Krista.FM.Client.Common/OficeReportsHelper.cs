using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Common
{
    /// <summary>
    /// общий класс для всех отчетов
    /// </summary>
    public abstract class ReportsHelper : DisposableObject
    {
        internal IScheme scheme = null;

        public virtual void CreateReport()
        {
            
        }

        public virtual void CreateReport(object reportParam, string templateName)
        {

        }

        public virtual void CreateReport(string templateName, DataTable dtReportData)
        {
            
        }

        public virtual void CreateReport(string templateName, DataTable[] tables)
        {
            
        }

        protected override void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing)
                {
                    // Вызываем сборщик мусора для немедленной очистки памяти
                    GC.GetTotalMemory(true);
                }
            }
        }
    }

    public class OficeReportsHelper : ReportsHelper
    {
        internal OficeApplicationWrapper oficeWrapper = null;

        private string filter = string.Empty;
        private IDbDataParameter[] filterParams = null;

        public OficeReportsHelper(IScheme scheme)
        {
            base.scheme = scheme;
        }

        internal virtual OficeApplicationWrapper GetOficeWrapper()
        {
            throw new Exception("The method or operation is not implemented.");
        }
        
        public override void CreateReport(object reportParam, string templateName)
        {
            oficeWrapper = GetOficeWrapper();
            oficeWrapper.CreateAsTemplate(templateName);
            // получили запросы
            Dictionary<int, string> sqlQueries = GetSQLDefinition();
            // получаем некоторые параметры
            //int paramsCounter = 0;
            foreach (KeyValuePair<int, string> kvp in sqlQueries)
            {
                // выполняем отдельный запрос, получаем данные, передаем эти данные в макрос методу,
                // который должен заполнять
                ADODB.Recordset rs = null;
                List<IDbDataParameter> queryParams = new List<IDbDataParameter>();
                using (IDatabase db = scheme.SchemeDWH.DB)
                {
                    string query = kvp.Value.Replace("?", ":p0");
                    queryParams.Add(new System.Data.OleDb.OleDbParameter("p0", reportParam));

                    DataTable dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, queryParams.ToArray());
                    rs = DataTableConverter.ConvertToRecordset(dt);
                }
                oficeWrapper.RunMacros(string.Format("GetRecordSet{0}", kvp.Key + 1), rs);
            }
            oficeWrapper.RunMacros("FillReport");

            oficeWrapper.DocumentVisible = true;
            oficeWrapper.ReleaseOficeApplication();
        }

        public override void CreateReport(string templateName, DataTable dtReportData)
        {
            oficeWrapper = GetOficeWrapper();
            oficeWrapper.CreateAsTemplate(templateName);
            ADODB.Recordset rs = DataTableConverter.ConvertToRecordset(dtReportData);
            oficeWrapper.RunMacros("GetRecordSet", rs);
            oficeWrapper.RunMacros("FillReport");
            oficeWrapper.DocumentVisible = true;
            oficeWrapper.ReleaseOficeApplication();
        }

        public override void CreateReport(string templateName, DataTable[] tables)
        {
            oficeWrapper = GetOficeWrapper();
            oficeWrapper.CreateAsTemplate(templateName);
            int tableIndex = 1;
            foreach (DataTable table in tables)
            {
                ADODB.Recordset rs = DataTableConverter.ConvertToRecordset(table);
                oficeWrapper.RunMacros(string.Format("GetRecordSet{0}", tableIndex), rs);
                tableIndex++;
            }
            oficeWrapper.RunMacros("FillReport");
            oficeWrapper.DocumentVisible = true;
            oficeWrapper.ReleaseOficeApplication();
        }

        private Dictionary<int, string> GetSQLDefinition()
        {
            // получаем некие запросы из макроса. 
            // они будут в одной строке с разделителем между запросами ';'
            Dictionary<int, string> sqlQueries = new Dictionary<int, string>();
            string queriesStr = oficeWrapper.RunMacros("GetSQLDefinition").ToString();

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

        protected virtual bool GetFileName(string startFileName, ref string finishFileName)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    public class WordReportHelper : OficeReportsHelper
    {
        public WordReportHelper(IScheme scheme)
            : base(scheme)
        {
            //base(scheme);
        }

        protected override bool GetFileName(string startFileName, ref string finishFileName)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Шаблоны для отчетов(*.doc, *.dot)|*.doc;*.dot";

            dlg.FileName = ExportImportHelper.GetCorrectFileName(startFileName);

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                finishFileName = dlg.FileName;
                dlg.Dispose();
                return true;
            }
            else
            {
                dlg.Dispose();
                return false;
            }
        }

        internal override OficeApplicationWrapper GetOficeWrapper()
        {
            return new WordApplicationWrapper();
        }
    }

    public class ExcelReportHelper : OficeReportsHelper
    {
        public ExcelReportHelper(IScheme scheme)
            : base(scheme)
        {
            //base(scheme);
        }

        protected override bool GetFileName(string startFileName, ref string finishFileName)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Шаблоны для отчетов(*.xls, *.xlt)|*.xls;*.xlt";

            dlg.FileName = ExportImportHelper.GetCorrectFileName(startFileName);

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                finishFileName = dlg.FileName;
                dlg.Dispose();
                return true;
            }
            else
            {
                dlg.Dispose();
                return false;
            }
        }

        internal override OficeApplicationWrapper GetOficeWrapper()
        {
            return new ExcelApplicationWrapper();
        }
    }

    internal abstract class OficeApplicationWrapper
    {
        internal OficeApplicationWrapper()
        {

        }

        /// <summary>
        /// создание документа по шаблону
        /// </summary>
        internal virtual void CreateAsTemplate(string templatePath)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal virtual object RunMacros(string macrosName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal virtual object RunMacros(string macrosName, ADODB.Recordset rs)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal virtual bool DocumentVisible
        {
            get { throw new Exception("The method or operation is not implemented."); }
            set { throw new Exception("The method or operation is not implemented."); }
        }

        internal virtual void ReleaseOficeApplication()
        {

        }
    }

    internal class ExcelApplicationWrapper : OficeApplicationWrapper
    {
        object excelApplication = null;
        ExcelHelper excelHelper = null;

        internal ExcelApplicationWrapper()
        {
            excelHelper = new ExcelHelper(false);
            excelApplication = excelHelper.GetOfficeObject();
        }

        internal override void CreateAsTemplate(string templatePath)
        {
            object workBooks = ReflectionHelper.GetProperty(excelApplication, "Workbooks", null);
            ReflectionHelper.CallMethod(workBooks, "Add", templatePath);
        }

        internal override object RunMacros(string macrosName)
        {
            object[] paramArray = new object[31];
            for (int i = 0; i <= 30; i++)
            {
                paramArray[i] = Type.Missing;
            }
            paramArray[0] = macrosName;
            return ReflectionHelper.CallMethod(excelApplication, "Run", paramArray);
        }

        internal override object RunMacros(string macrosName, ADODB.Recordset rs)
        {
            object[] paramArray = new object[31];
            for (int i = 0; i <= 30; i++)
            {
                paramArray[i] = Type.Missing;
            }
            paramArray[0] = macrosName;
            paramArray[1] = rs;
            return ReflectionHelper.CallMethod(excelApplication, "Run", paramArray);
        }

        internal override bool DocumentVisible
        {
            get
            {
                return (bool)ReflectionHelper.GetProperty(excelApplication, "Visible", null);
            }
            set
            {
                ReflectionHelper.SetProperty(excelApplication, "Visible", value);
            }
        }

        internal override void ReleaseOficeApplication()
        {

        }
    }

    internal class WordApplicationWrapper : OficeApplicationWrapper
    {
        object wordApplication = null;
        WordHelper wordHelper = null;

        internal WordApplicationWrapper()
        {
            wordHelper = new WordHelper(false);
            wordApplication = wordHelper.GetOfficeObject();
        }

        internal override void CreateAsTemplate(string templatePath)
        {
            object[] paramArray = new object[4];
            paramArray[0] = templatePath;
            paramArray[1] = false;
            paramArray[2] = 0;
            paramArray[3] = true;
            object documents = ReflectionHelper.GetProperty(wordApplication, "Documents", null);
            ReflectionHelper.CallMethod(documents, "Add", paramArray);
        }

        internal override object RunMacros(string macrosName)
        {
            object[] paramArray = new object[31];
            for (int i = 1; i <= 30; i++)
            {
                paramArray[i] = Type.Missing;
            }
            paramArray[0] = macrosName;
            try
            {
                return ReflectionHelper.CallMethod(wordApplication, "[DispID=445]", paramArray);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        internal override object RunMacros(string macrosName, ADODB.Recordset rs)
        {
            object[] paramArray = new object[31];
            for (int i = 1; i <= 30; i++)
            {
                paramArray[i] = Type.Missing;
            }
            paramArray[0] = macrosName;
            paramArray[1] = rs;

            return ReflectionHelper.CallMethod(wordApplication, "Run", paramArray); 
        }

        internal override bool DocumentVisible
        {
            get
            {
                return (bool)ReflectionHelper.GetProperty(wordApplication, "Visible", null);//appl.Visible;
            }
            set
            {
                ReflectionHelper.SetProperty(wordApplication, "Visible", value);//appl.Visible = value;
            }
        }

        internal override void ReleaseOficeApplication()
        {
            if ((wordApplication != null) && (Marshal.IsComObject(wordApplication)))
            {
                Marshal.ReleaseComObject(wordApplication);
                GC.GetTotalMemory(true);
            }
        }
    }

    internal class DataTableConverter
    {
        /// <summary>
        /// Преобразовать тип данных .NET в тип данных ADODB
        /// </summary>
        /// <param name="columnType"></param>
        /// <returns></returns>
        private static ADODB.DataTypeEnum TranslateType(Type columnType)
        {
            switch (columnType.UnderlyingSystemType.ToString())
            {
                case "System.Boolean":
                    return ADODB.DataTypeEnum.adBoolean;

                case "System.Byte":
                    return ADODB.DataTypeEnum.adUnsignedTinyInt;

                case "System.Char":
                    return ADODB.DataTypeEnum.adChar;

                case "System.DateTime":
                    return ADODB.DataTypeEnum.adDate;

                case "System.Decimal":
                    return ADODB.DataTypeEnum.adCurrency;

                case "System.Double":
                    return ADODB.DataTypeEnum.adDouble;

                case "System.Int16":
                    return ADODB.DataTypeEnum.adSmallInt;

                case "System.Int32":
                    return ADODB.DataTypeEnum.adInteger;

                case "System.Int64":
                    return ADODB.DataTypeEnum.adBigInt;

                case "System.SByte":
                    return ADODB.DataTypeEnum.adTinyInt;

                case "System.Single":
                    return ADODB.DataTypeEnum.adSingle;

                case "System.UInt16":
                    return ADODB.DataTypeEnum.adUnsignedSmallInt;

                case "System.UInt32":
                    return ADODB.DataTypeEnum.adUnsignedInt;

                case "System.UInt64":
                    return ADODB.DataTypeEnum.adUnsignedBigInt;

                case "System.String":
                default:
                    return ADODB.DataTypeEnum.adVarChar;
            }
        }

        /// <summary>
        /// Преобразовать DataTable в ADODB.RecordSet
        /// </summary>
        /// <param name="inTable">DataTable  с исходными данными</param>
        /// <returns>ADODB.RecordSet</returns>
        static public ADODB.Recordset ConvertToRecordset(DataTable inTable)
        {
            // создаем новый объект ADODB.Recordset
            ADODB.Recordset result = new ADODB.Recordset();
            result.CursorLocation = ADODB.CursorLocationEnum.adUseClient;

            ADODB.Fields resultFields = result.Fields;
            DataColumnCollection inColumns = inTable.Columns;
            // Создаем в нем поля
            foreach (DataColumn inColumn in inColumns)
            {
                resultFields.Append(inColumn.ColumnName,
                    TranslateType(inColumn.DataType),
                    inColumn.MaxLength,
                    inColumn.AllowDBNull ? ADODB.FieldAttributeEnum.adFldIsNullable :
                                           ADODB.FieldAttributeEnum.adFldUnspecified,
                    null);
            }

            result.Open(Missing.Value,
                Missing.Value,
                ADODB.CursorTypeEnum.adOpenStatic,
                ADODB.LockTypeEnum.adLockOptimistic,
                0);
            // Переносим данные из DataTable в RecordSet
            foreach (DataRow dr in inTable.Rows)
            {
                result.AddNew(Missing.Value, Missing.Value);
                for (int columnIndex = 0; columnIndex < inColumns.Count; columnIndex++)
                {
                    resultFields[columnIndex].Value = dr[columnIndex];
                }
            }
            return result;
        }
    }
}

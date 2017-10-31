using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.TemplatesService;
using Krista.FM.Client.MobileReports.Common;

namespace Krista.FM.Client.MobileReports.Core
{
    public static class SQLBuilder
    {
        /// <summary>
        /// ��������� SQL ������ � �������
        /// </summary>
        /// <returns></returns>
        public static string BuildSQLScripts(IScheme scheme, CategoryInfo repositoryStructure)
        {
            string result = string.Empty;
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                int maxRecordCount = 100000;
                
                result += "\n\n-- ���� ������ ������� Templates\n\n";
                result += TemplatesSQLHelper.GetSQLQueryTable(repositoryStructure);

                /*result += "\n\n-- ���� ������ ������� Templates\n\n";
                string sqlScript = string.Format("SELECT ID, ParentID, Type, Code, Name, Description, Document FROM Templates WHERE RefTemplatesTypes = {0}", (int)TemplateTypes.IPhone);
                DataTable dt = db.SelectData(sqlScript, maxRecordCount);
                dt.TableName = "Templates";
                result += GetSQLValues(dt);*/

                result += "\n\n-- ���� ������ ������� Objects\n\n";
                string  sqlScript = string.Format("SELECT ID, ObjectKey, Name, Caption, Description, ObjectType FROM Objects WHERE ObjectType = {0}", (int)SysObjectsTypes.Template);
                DataTable dt = db.SelectData(sqlScript, maxRecordCount);
                dt.TableName = "Objects";
                result += GetSQLValues(dt);

                result += "\n\n-- ���� ������ ������� Groups\n\n";
                sqlScript = "SELECT ID, Name, Description, Blocked FROM Groups";
                dt = db.SelectData(sqlScript, maxRecordCount);
                dt.TableName = "Groups";
                result += GetSQLValues(dt);

                result += "\n\n-- ���� ������ ������� Memberships\n\n";
                sqlScript = "SELECT ID, RefUsers, RefGroups FROM Memberships";
                dt = db.SelectData(sqlScript, maxRecordCount);
                dt.TableName = "Memberships";
                result += GetSQLValues(dt);

                result += "\n\n-- ���� ������ ������� Permissions\n\n";
                result += PermissionsSQLHelper.GetSQLQueryTable(repositoryStructure);
                /*
                result += "\n\n-- ���� ������ ������� Permissions\n\n";
                sqlScript = "SELECT ID, RefObjects, RefGroups, RefUsers FROM Permissions";
                dt = db.SelectData(sqlScript, maxRecordCount);
                dt.TableName = "Permissions";
                result += GetSQLValues(dt);*/

                result += "\n\n-- ���� ������ ������� Users\n\n";
                sqlScript = "SELECT ID, Name, Blocked, PwdHashSHA FROM Users";
                dt = db.SelectData(sqlScript, maxRecordCount);
                dt.TableName = "Users";
                result += GetSQLValues(dt);
            }
            return result;
        }

        /// <summary>
        /// � dt ������ ���� ��������� ���� TableName, �� ���� ����� ��� �������. �������� SQL ������ �� 
        /// ���������� �������. 
        /// </summary>
        /// <param name="dt">������� � �������</param>
        /// <returns>������ SQL ������ �� ���������� �� �������</returns>
        public static string GetSQLValues(DataTable dt)
        {
            return GetSQLValues(dt, new string[0]);
        }

        /// <summary>
        /// � dt ������ ���� ��������� ���� TableName, �� ���� ����� ��� �������.�������� SQL ������ �� 
        /// ���������� �������. � columnsName ���������� ����� ������� �� ������� ����� ������. 
        /// ���� ������ ������, ������ ������� �� ���� �������.
        /// </summary>
        /// <param name="dt">������� � �������</param>
        /// <param name="columnsName">����� ������� �� ������� ����� ������, ���� ������ ������, ������ ������� �� ���� �������</param>
        /// <returns>������ SQL ������ �� ���������� �� �������</returns>
        public static string GetSQLValues(DataTable dt, string[] columnsCaption)
        {
            string result = string.Empty;
            if ((dt == null) || (dt.Columns.Count == 0) || (dt.Rows.Count == 0))
                return result;

            List<string> columnsCaptionList = null;
            if (columnsCaption.Length == 0)
                //���� ������ ������, ������ �������� ����� �� ���� ��������
                columnsCaptionList = GetColumnsCaption(dt);
            else
                columnsCaptionList = new List<string>(columnsCaption);

            //�������� ������������ �������
            //��������� �������
            string sqlColumnsCaption = string.Empty;
            foreach (string columnCaption in columnsCaptionList)
            {
                if (sqlColumnsCaption == string.Empty)
                    sqlColumnsCaption = string.Format("`{0}`", columnCaption);
                else
                    sqlColumnsCaption += string.Format(", `{0}`", columnCaption);
            }

            //�������� �����
            string sqlRowsValue = string.Empty;
            foreach (DataRow row in dt.Rows)
            {
                string sqlRowValue = string.Empty;
                foreach (string columnCaption in columnsCaptionList)
                {
                    DataColumn dataColumn = dt.Columns[columnCaption];
                    if (dataColumn == null)
                    {
                        string exception = string.Format("� ������� {0} �� ���������� ������� � ����������: {1}",
                            dt.TableName, columnCaption);
                        throw new Exception(exception);
                    }
                    object rowValue = row[dataColumn];
                    string value = (rowValue is DBNull) ? Consts.sqlNULL : rowValue.ToString();
                    if (dataColumn.DataType.Name.ToLower() == "byte[]")
                    {
                        if (!(rowValue is DBNull))
                        {
                            byte[] document = (byte[])rowValue;
                            value = Encoding.UTF8.GetString(document);
                        }
                    }
                    if (value != Consts.sqlNULL)
                        value = string.Format("'{0}'", value);

                    if (sqlRowValue == string.Empty)
                        sqlRowValue = String.Format("{0}", value);
                    else
                        sqlRowValue += String.Format(", {0}", value);
                }
                if (sqlRowsValue == string.Empty)
                    sqlRowsValue = String.Format("({0})", sqlRowValue);
                else
                    sqlRowsValue += String.Format(", ({0})", sqlRowValue);
            }
            //�������� ������
            result = string.Format("INSERT INTO `{0}` ({1}) VALUES {2};", dt.TableName, sqlColumnsCaption, sqlRowsValue);
            //���������� �����
            return result.Replace(@"\", @"\\");
        }

        /// <summary>
        /// �������� ��������� ������� �������
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static List<string> GetColumnsCaption(DataTable dt)
        {
            List<string> result = new List<string>();
            if (dt == null)
                return result;

            foreach (DataColumn column in dt.Columns)
            {
                result.Add(column.Caption);
            }
            return result;
        }
    }
}

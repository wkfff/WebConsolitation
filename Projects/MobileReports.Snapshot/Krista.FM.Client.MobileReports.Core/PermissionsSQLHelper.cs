using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.MobileReports.Common;

namespace Krista.FM.Client.MobileReports.Core
{
    /// <summary>
    /// Т.к. в Workplace введена упрощенная система назначения прав на отчеты, 
    /// таблицу с ними формирует данный класс, а не берем из БД.
    /// </summary>
    public static class PermissionsSQLHelper
    {
        public static string GetSQLQueryTable(CategoryInfo repositoryStructure)
        {
            int rowIndex = 0;
            string sqlRowsValue = GetSQLValues(repositoryStructure, ref rowIndex);

            //уберем лишние кавычки, иначе в БД - NULL будет считаться строкой
            sqlRowsValue = sqlRowsValue.Replace("'NONE'", Consts.sqlNULL);

            string tableName = "Permissions";
            string sqlColumnsCaption = "ID, RefObjects, RefGroups, RefUsers";
            //собираем скрипт
            return string.Format("INSERT INTO `{0}` ({1}) VALUES {2};", tableName, sqlColumnsCaption,
                sqlRowsValue);
        }

        private static string GetSQLValues(CategoryInfo currentCategory, ref int rowIndex)
        {
            string result = string.Empty;
            //если категория не корневая, смотрим ее права
            if (currentCategory.RefObject != -1)    
                result = GetSQLValue(currentCategory, ref rowIndex);

            foreach (ReportInfo report in currentCategory.ChildrenReports)
            {
                string value = GetSQLValue(report, ref rowIndex);
                if (result == string.Empty)
                    result = value;
                else
                {
                    if (!String.IsNullOrEmpty(value))
                        result += string.Format(", {0}", value);
                }
            }

            foreach (CategoryInfo category in currentCategory.ChildrenCategory)
            {
                string value = GetSQLValues(category, ref rowIndex);
                if (result == string.Empty)
                    result = value;
                else
                {
                    if (!String.IsNullOrEmpty(value))
                        result += string.Format(", {0}", value);
                }
            }
            return result.Trim();
        }

        private static string GetSQLValue(BaseElementInfo elementInfo, ref int rowIndex)
        {
            string result = string.Empty;

            foreach (int groupPermission in elementInfo.GroupsPermisions)
            {
                rowIndex++;
                string value = string.Format("('{0}', '{1}', '{2}', {3})", rowIndex, elementInfo.RefObject, 
                    groupPermission, Consts.sqlNULL);
                if (result == string.Empty)
                    result = value;
                else
                {
                    if (!String.IsNullOrEmpty(value))
                        result += string.Format(", {0}", value);
                }
            }

            foreach (int userPermission in elementInfo.UsersPermisions)
            {
                rowIndex++;
                string value = string.Format("('{0}', '{1}', {2}, '{3}')", rowIndex, elementInfo.RefObject,
                    Consts.sqlNULL, userPermission);
                if (result == string.Empty)
                    result = value;
                else
                {
                    if (!String.IsNullOrEmpty(value))
                        result += string.Format(", {0}", value);
                }
            }
            return result.Trim();
        }
    }
}

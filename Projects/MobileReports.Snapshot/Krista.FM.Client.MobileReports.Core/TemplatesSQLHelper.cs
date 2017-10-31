using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.MobileReports.Common;
using Krista.FM.ServerLibrary.TemplatesService;
using Krista.FM.Common.Xml;

namespace Krista.FM.Client.MobileReports.Core
{
    /// <summary>
    /// Формирует таблицу с отчетами и их свойствами
    /// </summary>
    public static class TemplatesSQLHelper
    {
        /// <summary>
        /// По структуре репозитория, формирует SQL скрипт
        /// </summary>
        /// <param name="repositoryStructure"></param>
        /// <returns></returns>
        public static string GetSQLQueryTable(CategoryInfo repositoryStructure)
        {
            string sqlRowsValue = GetSQLValues(repositoryStructure);
            //уберем лишние кавычки, иначе в БД - NULL будет считаться строкой
            sqlRowsValue = sqlRowsValue.Replace("'NULL'", Consts.sqlNULL);

            string tableName = "Templates";
            string sqlColumnsCaption = "ID, ParentID, Type, Code, Name, Description, Document, SelfHashCode, DescendantsHashCode, ForumDiscussionID, TerritorialTagsID";
            //собираем скрипт
            return string.Format("INSERT INTO `{0}` ({1}) VALUES {2};", tableName, sqlColumnsCaption,
                sqlRowsValue);
        }

        private static string GetSQLValues(CategoryInfo currentCategory)
        {
            string result = string.Empty;
            //если категория не корневая, смотрим ее права
            if (currentCategory.RefObject != -1)
                result = GetSQLRowValue(currentCategory, TemplateDocumentTypes.Group);

            foreach (ReportInfo report in currentCategory.ChildrenReports)
            {
                string value = GetSQLRowValue(report, TemplateDocumentTypes.WebReport);
                if (result == string.Empty)
                    result = value;
                else
                    result += string.Format(", {0}", value);
            }

            foreach (CategoryInfo category in currentCategory.ChildrenCategory)
            {
                string value = GetSQLValues(category);
                if (result == string.Empty)
                    result = value;
                else
                    result += string.Format(", {0}", value);
            }
            return result;
        }

        private static string GetSQLRowValue(BaseElementInfo elementInfo, TemplateDocumentTypes documentType)
        {
            IPhoteTemplateDescriptor templateDescriptor = elementInfo.GetTemplateDescriptor(false);
            string parentID = elementInfo.ParentId >= 0 ? elementInfo.ParentId.ToString() : Consts.sqlNULL;
            int descendantsHashCode = (documentType == TemplateDocumentTypes.Group) ?
                ((CategoryInfo)elementInfo).GetDescendantsHashCode() : 0;

            return string.Format("('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}')", elementInfo.Id,
                parentID, (int)documentType, elementInfo.Code, elementInfo.Name,
                elementInfo.Description, XmlHelper.Obj2XmlStr(templateDescriptor),
                elementInfo.SelfHashCode, descendantsHashCode, elementInfo.ForumDiscussionID, elementInfo.TerritorialTagsID);
        }
    }
}

using System;
using System.Data;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes.Visitors
{
    public enum ErrorType
    {
        /// <summary>
        /// ќшибка св€занна€ с метаданными
        /// </summary>
        MetaDataError,
        /// <summary>
        /// ќшибка св€занна€ с рел€ционной базой данных
        /// </summary>
        DataBaseError
    }

    internal class ValidationVisitor
    {
        private readonly DataTable messagesTable;
        private readonly DataColumn columnPackage;
        private readonly DataColumn columnObject;
        private readonly DataColumn columnMessage;
        private readonly DataColumn columnTypeError;

        internal ValidationVisitor()
        {
            messagesTable = new DataTable();
            columnTypeError = new DataColumn("TypeError", typeof(string));
            columnPackage = new DataColumn("Package", typeof(string));
            columnObject = new DataColumn("Object", typeof(string));
            columnMessage = new DataColumn("Message", typeof(string));
            messagesTable.Columns.Add(columnTypeError);
            messagesTable.Columns.Add(columnPackage);
            messagesTable.Columns.Add(columnObject);
            messagesTable.Columns.Add(columnMessage);
        }

        internal DataTable ResultTable
        {
            get { return messagesTable; }
        }

        protected void LogError(ErrorType errorType, IPackage package, ICommonDBObject obj, string message, params object[] parameters)
        {
            DataRow row = messagesTable.NewRow();
            row[columnTypeError] = errorType.ToString();
            row[columnPackage] = package.Name;
            row[columnObject] = obj.FullName;
            row[columnMessage] = String.Format(message, parameters);
            messagesTable.Rows.Add(row);
        }
    }
}

using System;
using System.Collections.Generic;

namespace Krista.FM.Server.Dashboards.MinSportSupport
{
    public class UseHandBooks
    {
        private readonly string code;
        private readonly int layout;
        private readonly string handBookColumnName;
        private readonly string handBookElementColumnName;
        private readonly string handBookTotalElement;
        private readonly string handBookParent;

        public string Code
        {
            get { return code; }
        }

        public int Layout
        {
            get { return layout; }
        }

        public string HandBookParent
        {
            get { return handBookParent; }
        }

        public string HandBookColumnName
        {
            get { return handBookColumnName; }
        }

        public string HandBookElementColumnName
        {
            get { return handBookElementColumnName; }
        }

        public string HandBookTotalElement
        {
            get { return handBookTotalElement; }
        }

        public UseHandBooks(string code, int layout, string handBookColumnName, string handBookElementColumnName, string handBookTotalElement, string parent)
        {
            this.code = code;
            this.layout = layout;
            this.handBookColumnName = handBookColumnName;
            this.handBookElementColumnName = handBookElementColumnName;
            this.handBookTotalElement = handBookTotalElement;
            handBookParent = parent;
        }
    }

    public class CrossHandBooks
    {
        private readonly string handBookOnRows;
        private readonly string handBookOnColumns;

        public string HandBookOnRows
        {
            get { return handBookOnRows; }
        }

        public string HandBookOnColumns
        {
            get { return handBookOnColumns; }
        }

        public CrossHandBooks(string handBookOnRows, string handBookOnColumns)
        {
            this.handBookOnRows = handBookOnRows;
            this.handBookOnColumns = handBookOnColumns;
        }
    }

    /// <summary>
    /// Статический класс для управления используемыми справочниками
    /// </summary>
    public static class UseHandBooksManager
    {
        private static Dictionary<string, UseHandBooks> dictionaryHandBooks;

        public static void SetupHandBooksManager()
        {
            dictionaryHandBooks = new Dictionary<string, UseHandBooks>();
            var i = 1;
            foreach (var handBook in XmlWorker.GetFactorUseHandBooks())
            {
                dictionaryHandBooks.Add(handBook,
                               new UseHandBooks(handBook, XmlWorker.GetHandBookLayout(handBook), String.Format("Справочник_{0}", i),
                                                String.Format("ЭлементСправочника_{0}", i),
                                                XmlWorker.GetHandBookTotalCode(handBook), XmlWorker.GetHandBookParent(handBook)));
                i++;
            }
        }

        public static string HasHandBookParent(string code)
        {
            return dictionaryHandBooks[code].HandBookParent;
        }

        public static string GetColumnNameHandBook(string code)
        {
            return dictionaryHandBooks[code].HandBookColumnName ;
        }

        public static string GetColumnNameElement(string code)
        {
            return dictionaryHandBooks[code].HandBookElementColumnName;
        }

        public static string GetTotalElement(string code)
        {
            return dictionaryHandBooks[code].HandBookTotalElement;
        }

        public static int GetLayout(string code)
        {
            return dictionaryHandBooks[code].Layout;
        }

        public static string GetHandBookCodeForColumnName(string columnName)
        {
            
            foreach (KeyValuePair<string, UseHandBooks> kvp in dictionaryHandBooks)
            {
                if (kvp.Value.HandBookColumnName == columnName)
                {
                    return kvp.Value.Code;
                }
                if (kvp.Value.HandBookElementColumnName == columnName)
                {
                    return kvp.Value.HandBookTotalElement;
                }
            }
            return "Значение не найдено!!!";
        }
    }
}

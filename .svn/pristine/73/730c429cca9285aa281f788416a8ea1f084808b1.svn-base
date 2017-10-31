using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.Core
{
    /// <summary>
    /// Сборник атрибутов элементов измерений
    /// </summary>
    public class MemberAttributesDigest
    {
        #region Поля

        private Dictionary<string, string> uniqueNames;
        private Dictionary<string, string> memberLevels;
        private Dictionary<string, string> memberTypes;
        private Dictionary<string, string> shortNames;
        private Dictionary<string, string> fullNames;
        private DataProvider provider;
        private string queryId;
        private DataTable dt;
        private string queryText;

        private const string NameColumnCaption = "Имя";
        private const string UniqueNameColumnCaption = "Уникальное имя";
        private const string LevelColumnCaption = "Уровень";
        private const string TypeColumnCaption = "Тип";
        private const string shortNameColumnCaption = "Краткое имя";
        private const string fullNameColumnCaption = "Полное имя";

        private string queryPath = String.Empty;

        #endregion

        #region Свойства

        public Dictionary<string, string> UniqueNames
        {
            get
            {
                if (uniqueNames == null || uniqueNames.Count == 0)
                {
                    FillUniqueNames();
                }
                return uniqueNames;
            }
        }

        public Dictionary<string, string> MemberLevels
        {
            get
            {
                if (memberLevels == null || memberLevels.Count == 0)
                {
                    FillUniqueNames();
                }
                return memberLevels;
            }
        }

        public Dictionary<string, string> MemberTypes
        {
            get
            {
                if (memberTypes == null || memberTypes.Count == 0)
                {
                    FillUniqueNames();
                }
                return memberTypes;
            }
        }

        public Dictionary<string, string> ShortNames
        {
            get
            {
                if (shortNames == null || shortNames.Count == 0)
                {
                    FillUniqueNames();
                }
                return shortNames;
            }
        }

        public Dictionary<string, string> FullNames
        {
            get
            {
                if (fullNames == null || fullNames.Count == 0)
                {
                    FillUniqueNames();
                }
                return fullNames;
            }
        }

        #endregion

        public MemberAttributesDigest(DataProvider dataProvider, string queryID)
        {
            provider = dataProvider;
            queryId = queryID;

            queryText = DataProvider.GetQueryText(queryId);
        }

        public MemberAttributesDigest(DataProvider dataProvider, string queryID, string path)
        {
            provider = dataProvider;
            queryId = queryID;
            queryPath = path;

            queryText = DataProvider.GetQueryText(queryId, queryPath);
        }

        public string GetMemberUniqueName(string memberName)
        {
            if (UniqueNames.ContainsKey(memberName))
            {
                return UniqueNames[memberName];
            }
            return String.Empty;
        }

        public string GetMemberLevel(string memberName)
        {
            if (MemberLevels.ContainsKey(memberName))
            {
                return MemberLevels[memberName];
            }
            return String.Empty;
        }

        public string GetMemberType(string memberName)
        {
            if (MemberTypes.ContainsKey(memberName))
            {
                return MemberTypes[memberName];
            }
            return String.Empty;
        }

        public string GetMemberName(string memberUniqueName)
        {
            foreach (string key in UniqueNames.Keys)
            {
                if (UniqueNames[key] == memberUniqueName)
                {
                    return key;
                }
            }
            return String.Empty;
        }

        public string GetShortName(string fullName)
        {
            if (ShortNames.ContainsKey(fullName))
            {
                return ShortNames[fullName];
            }
            return fullName;
        }

        public string GetFullName(string shortName)
        {
            foreach (string key in ShortNames.Keys)
            {
                if (ShortNames[key] == shortName)
                {
                    return key;
                }
            }
            return shortName;
        }

        private void FillUniqueNames()
        {
            uniqueNames = new Dictionary<string, string>();
            memberLevels = new Dictionary<string, string>();
            memberTypes = new Dictionary<string, string>();
            shortNames = new Dictionary<string, string>();
            fullNames = new Dictionary<string, string>();

            dt = new DataTable();
            provider.GetDataTableForChart(queryText, "Dummy", dt);

            foreach (DataRow row in dt.Rows)
            {
                if (dt.Columns.Contains(NameColumnCaption))
                {
                    string name = row[NameColumnCaption].ToString();

                    if (dt.Columns.Contains(UniqueNameColumnCaption) && row[UniqueNameColumnCaption] != DBNull.Value)
                    {
                        AddUniqueDictionaryPair(uniqueNames, name, row[UniqueNameColumnCaption].ToString());
                    }

                    if (dt.Columns.Contains(LevelColumnCaption) && row[LevelColumnCaption] != DBNull.Value)
                    {
                        AddUniqueDictionaryPair(memberLevels, name, row[LevelColumnCaption].ToString());
                    }

                    if (dt.Columns.Contains(TypeColumnCaption) && row[TypeColumnCaption] != DBNull.Value)
                    {
                        AddUniqueDictionaryPair(memberTypes, name, row[TypeColumnCaption].ToString());
                    }

                    if (dt.Columns.Contains(shortNameColumnCaption) && row[shortNameColumnCaption] != DBNull.Value &&
                        dt.Columns.Contains(fullNameColumnCaption) && row[fullNameColumnCaption] != DBNull.Value)
                    {
                        string fullName = row[fullNameColumnCaption].ToString();
                        string shortName = row[shortNameColumnCaption].ToString();

                        AddUniqueDictionaryPair(shortNames, fullName, shortName);
                    }
                }
            }

            dt.Dispose();
        }

        private static void AddUniqueDictionaryPair(Dictionary<string, string> dictionary, string key, string value)
        {
            string uniqueKey = GetDictionaryUniqueKey(dictionary, key);
            dictionary.Add(uniqueKey, value);
        }

        private static string GetDictionaryUniqueKey(Dictionary<string, string> dictionary, string key)
        {
            string newKey = key;
            while (dictionary.ContainsKey(newKey))
            {
                newKey += " ";
            }
            return newKey;
        }
    }
}
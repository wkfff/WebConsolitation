using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Xml;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.Core.QueryGenerators
{
    /// <summary>
    /// Генератор элементов по кодам КБК
    /// </summary>
    public class KbkMemberGenerator
    {
        #region Поля

        XmlDocument xmlDoc = new XmlDocument();
        private DataProvider provider;
        private DataTable kbkMembersDt;
        private DescendantsGenerator descendantsGenerator;
        private string dimension;
        private string aggregateType;
        private string memberDetailListString;

        private string kbkMemberDeclaration;
        private string codeProperty;
        private string codeComparingRule;

        private Collection<string> memberDeclarationList;
        private Collection<string> memberList;
        string memberDetailList = String.Empty;

        private Dictionary<string, int> indicatorLevels;

        private int indicatorLevel;

        #endregion

        #region Свойства

        /// <summary>
        /// Секция объявления элементов в виде строки
        /// </summary>
        public string MemberDeclarationListString
        {
            get
            {
                string memberDeclarationListString = String.Empty;
                foreach (string memberDeclaration in memberDeclarationList)
                {
                    memberDeclarationListString += String.Format("{0}\n ", memberDeclaration);
                }
                return memberDeclarationListString;
            }
        }

        /// <summary>
        /// Множество элементов в виде строки
        /// </summary>
        public string MemberListString
        {
            get
            {
                string memberListString = String.Empty;
                foreach (string member in memberList)
                {
                    memberListString += String.Format("{0}, ", member);
                }
                return memberListString.TrimEnd(' ').TrimEnd(',');
            }
        }

        /// <summary>
        /// Множество элементов с подчиненными в виде строки
        /// </summary>
        public string MemberDetailListString
        {
            get
            {
                return memberDetailListString;
            }
        }

        /// <summary>
        /// Определение свойства Код
        /// </summary>
        public string CodeProperty
        {
            get { return codeProperty; }
            set { codeProperty = value; }
        }

        /// <summary>
        /// Условие сравнения кода
        /// </summary>
        public string CodeComparingRule
        {
            get { return codeComparingRule; }
            set { codeComparingRule = value; }
        }

        #endregion

        /// <summary>
        /// Генератор элементов по кодам КБК
        /// </summary>
        /// <param name="provider">провайдер доступа к данным</param>
        /// <param name="xmlDocPath">xml с кодами КБК</param>
        /// <param name="descendantsGenerator">конструкция Descendants для задания множества выбираемых элементов</param>
        /// <param name="aggregateType">тип аггрегации элементов (Sum/Aggregate)</param>
        public KbkMemberGenerator(DataProvider provider, string xmlDocPath, DescendantsGenerator descendantsGenerator, string aggregateType)
        {
            xmlDoc.Load(xmlDocPath);

            this.provider = provider;
            this.descendantsGenerator = descendantsGenerator;
            this.aggregateType = aggregateType;
            dimension = descendantsGenerator.Dimension;

            memberList = new Collection<string>();
            memberDeclarationList = new Collection<string>();

            codeProperty = String.Format("{0}.CurrentMember.Properties(\"Код\")", dimension);
            codeComparingRule = "or ([Measures].[Код] = \"{0}\")";
        }

        /// <summary>
        /// Генарация элементов для конкретного года
        /// </summary>
        /// <param name="yearNum">год</param>
        public void GenerateQuery(int yearNum)
        {
            kbkMemberDeclaration = String.Empty;
            memberDetailListString = String.Empty;

            memberList.Clear();
            memberDeclarationList.Clear();

            memberDetailList = String.Empty;
            indicatorLevels = new Dictionary<string, int>();

            indicatorLevel = 0;

            if (xmlDoc.ChildNodes.Count > 0)
            {
                XmlNode root = xmlDoc.ChildNodes[0];
                foreach (XmlNode yearNode in root.ChildNodes)
                {
                    string year = GetStringAttrValue(yearNode, "name", String.Empty);
                    // ищем нужный год
                    if (year.ToLower() == yearNum.ToString())
                    {
                        // генерируем элементы запроса
                        GenerateMembers(yearNode, indicatorLevel);
                    }
                }
            }

            if (memberDetailList != String.Empty)
            {
                kbkMemberDeclaration += String.Format("member {0}.[Детализированный список ] as 'Generate({1}, {0}.CurrentMember.UniqueName, \",\")'", dimension, memberDetailList.TrimEnd('+'));
            }

            string query = String.Format("with member [Measures].[Код] as '{3}' {1} select {{{2}, {0}.[Детализированный список ] }} on columns from [ФО_АС Бюджет_План расходов]",
            dimension, kbkMemberDeclaration, MemberListString, codeProperty);
            
            kbkMembersDt = new DataTable();
            provider.GetDataTableForChart(query, "dummy", kbkMembersDt);

            if (kbkMembersDt.Rows.Count > 0)
            {
                foreach (DataColumn column in kbkMembersDt.Columns)
                {
                    string kbkMember = GetStringDTValue(kbkMembersDt, column.ColumnName);
                    if (kbkMember != String.Empty)
                    {
                        if (column.ColumnName == "Детализированный список ")
                        {
                            memberDetailListString = GetStringDTValue(kbkMembersDt, column.ColumnName);
                        }
                        else
                        {
                            memberDeclarationList.Add(kbkMember);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Генерация элементов запроса
        /// </summary>
        /// <param name="rootNode">корневой элемент структуры</param>
        /// <param name="level">уровень вложенности</param>
        private void GenerateMembers(XmlNode rootNode, int level)
        {
            foreach (XmlNode indicatorNode in rootNode.ChildNodes)
            {
                string indicatorName = GetStringAttrValue(indicatorNode, "name", String.Empty);
                bool collapse = true;

//                if (indicatorNode.Attributes != null && indicatorNode.Attributes["collapseGroup"] != null)
//                {
//                    collapse = Convert.ToBoolean(GetStringAttrValue(indicatorNode, "collapseGroup", String.Empty));
//                }

                AddIndicatorLevel(indicatorName, level);

                // проверяем тип элемента (групповой или КБК-элемент)
                if (indicatorNode.ChildNodes.Count > 0 && indicatorNode.ChildNodes[0].Name != "KBK" && indicatorNode.ChildNodes[0].Name != "Code")
                {
                    GenerateGroupMembers(indicatorNode, collapse);
                    GenerateMembers(indicatorNode, level + 1);
                }
                else
                {
                    GenerateKBKMembers(indicatorNode);
                }
            }
        }

        /// <summary>
        /// Генерация группового элемента
        /// </summary>
        /// <param name="indicatorNode">узел элемента</param>
        /// <param name="detailCollapse">детализировать группу</param>
        private void GenerateGroupMembers(XmlNode indicatorNode, bool detailCollapse)
        {
            string indicatorName = GetStringAttrValue(indicatorNode, "name", String.Empty);
            string memberCaption = String.Format("{0}.[{1}_]", dimension, indicatorName);

            if (detailCollapse)
            {
                memberDetailList += String.Format("{{ {0}.[{1}_] }} +", dimension, indicatorName);
                memberList.Add(memberCaption);
            }
            
            string indicatorSet = String.Empty;
            foreach (XmlNode groupIndicator in indicatorNode.ChildNodes)
            {
                indicatorSet += String.Format(",{0}.[{1}_]", dimension, GetStringAttrValue(groupIndicator, "name", String.Empty));
            }

            string declaration = String.Format(@"
                            member {0}.[{1}_]
                            as '""member {0}.[{1}_] as ''{2}
                                (
                                    {{  
                                       {3}
                                    }}  
                                )''""'
                             ", dimension, indicatorName, aggregateType, indicatorSet.Trim(','));
            
            kbkMemberDeclaration += declaration;
        }
        
        /// <summary>
        /// Генерация КБК-элемента
        /// </summary>
        /// <param name="indicatorNode"></param>
        private void GenerateKBKMembers(XmlNode indicatorNode)
        {
            string indicatorName = GetStringAttrValue(indicatorNode, "name", String.Empty);
            string memberCaption = String.Format("{0}.[{1}_]", dimension, indicatorName);

            memberDetailList += String.Format("{{ {0}.[{1}_] }} + [множество {1}] +", dimension, indicatorName);

            memberList.Add(memberCaption);

            string codeFilter = String.Empty;
            foreach (XmlNode kbkNode in indicatorNode.ChildNodes)
            {
                codeFilter += String.Format(codeComparingRule, kbkNode.InnerText);
            }

            string declaration = String.Format(@"
                            set [множество {1}]
                            as 'Filter  
                                (
                                    {2},
                                    not ({0}.CurrentMember is {0}.CurrentMember.Parent.DataMember) and
                                    (false {3})
                                )'

                            member {0}.[{1}_]
                            as '""member {0}.[{1}_] as ''{4}
                                (
                                    {{"" +
                                        Generate   
                                        (
                                            {{
                                                [множество {1}]
                                            }},
                                            {0}.CurrentMember.UniqueName,
                                            "",""
                                        ) + ""
                                    }}  
                                )''""'
                             ", dimension, indicatorName, descendantsGenerator, codeFilter, aggregateType);


            kbkMemberDeclaration += declaration;
        }

        private static string GetStringDTValue(DataTable dt, string columnName)
        {
            if (dt.Columns.Contains(columnName) && dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != String.Empty)
            {
                return dt.Rows[0][columnName].ToString();
            }
            return String.Empty;
        }

        public static string GetStringAttrValue(XmlNode xn, string attrName, string defaultValue)
        {
            try
            {
                if (xn == null || xn.Attributes == null)
                {
                    return defaultValue;
                }
                if (xn.Attributes.GetNamedItem(attrName) == null)
                {
                    return defaultValue;
                }

                string value = xn.Attributes[attrName].Value;
                if (value != String.Empty)
                {
                    return value;
                }
            }
            catch
            {
                return defaultValue;
            }
            return defaultValue;
        }

        private void AddIndicatorLevel(string indicatorName, int level)
        {
            if (!indicatorLevels.ContainsKey(indicatorName))
            {
                indicatorLevels.Add(indicatorName, level);
            }
        }

        /// <summary>
        /// Получить уровень элемента по наименованию
        /// </summary>
        /// <param name="indicatorName">наименование</param>
        /// <returns>уровень</returns>
        public int GetIndicatorLevel(string indicatorName)
        {
            if (indicatorLevels.ContainsKey(indicatorName))
            {
                return indicatorLevels[indicatorName];
            }

            return -1;
        }
    }
}

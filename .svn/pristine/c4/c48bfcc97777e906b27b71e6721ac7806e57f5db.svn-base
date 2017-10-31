using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Krista.FM.Common
{
    /// <summary>
    /// Работа с коллекцией правил.
    /// </summary>
    public class CheckParametrsCollection
    {
        private static List<CheckRule> ruleCollection;

        public static List<CheckRule> RuleCollection
        {
            get
            {
                if (ruleCollection == null)
                    LoadParamsFromXML();

                return ruleCollection;
            }
        }

        /// <summary>
        /// Заполнение коллекции правил проверки параметров
        /// </summary>
        private static void LoadParamsFromXML()
        {
            ruleCollection = new List<CheckRule>();

            XmlDocument document = new XmlDocument();
            document = Krista.FM.Common.Xml.XmlHelper.Load(AppDomain.CurrentDomain.BaseDirectory + "ParametersChecks.xml");

            if (document == null)
                throw new Exception("Не удалось найти файл параметров.");

            XmlNodeList nodeList = document.SelectNodes("ServerConfiguration//ParameterChecks//Check");

            try
            {
                foreach (XmlNode node in nodeList)
                {
                    // имя параметра
                    string parametr = String.Empty;
                    // класс проверки параметра
                    string className = String.Empty;
                    // если true - проверяем на не попадание в область допустимых значений
                    // false - значение по-умолчанию
                    bool invalid = false;
                    // значение параметра
                    string value = String.Empty;
                    // сообщение о некорректности параметра
                    string errorMessage = String.Empty;

                    if (node.Attributes["invalid"] != null)
                        invalid = true;

                    if (node.Attributes["parametr"] != null)
                        parametr = node.Attributes["parametr"].Value;

                    if (node.Attributes["class"] != null)
                        className = node.Attributes["class"].Value;

                    XmlNode valueNode = node.SelectSingleNode("Value");
                    if (valueNode != null)
                        value = valueNode.InnerText;

                    XmlNode errorMessageNode = node.SelectSingleNode("ErrorMessage");
                    if (errorMessageNode != null)
                        errorMessage = errorMessageNode.InnerText;

                    object[] args = new object[] { parametr, value, errorMessage, invalid};

                    Type type = Type.GetType(String.Format("Krista.FM.Common.{0}", className));

                    // отражением получаем нужный класс проверки
                    CheckRule rule = (CheckRule)type.InvokeMember(className,
                        System.Reflection.BindingFlags.CreateInstance |
                        System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Public,
                        null, null, args);


                    // заполняем поля
                    ruleCollection.Add(rule);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

        }
    }
}

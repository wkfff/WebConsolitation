using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;

namespace Krista.FM.Client.iMonitoringWM.Common
{
    public class XmlHelper
    {
        /// <summary>
        /// Получить строковый атрибут из узла
        /// </summary>
        /// <param name="node">узел</param>
        /// <param name="attributeName">имя атрибута</param>
        /// <param name="defaultValue">значение поумолчанию</param>
        /// <returns></returns>
        static public string GetStrXmlAttribute(XmlNode node, string attributeName, string defaultValue)
        {
            string result = defaultValue;
            if (node != null)
            {
                try
                {
                    result = node.Attributes[attributeName].Value;
                }
                catch
                {
                }
            }
            return result;
        }

        /// <summary>
        /// Получить булевский атрибут из узла
        /// </summary>
        /// <param name="node">узел</param>
        /// <param name="attributeName">имя атрибута</param>
        /// <param name="defaultValue">значение поумолчанию</param>
        /// <returns></returns>
        static public bool GetBoolXmlAttribute(XmlNode node, string attributeName, bool defaultValue)
        {
            string strValue = GetStrXmlAttribute(node, attributeName, defaultValue.ToString());
            bool result = defaultValue;

            try
            {
                result = bool.Parse(strValue);
            }
            catch
            {
                //Если произошло исключение, значит значение отличается от "true" и "false",
                //возможно маковские "yes" или "no"
                if (strValue.ToUpper() == "YES")
                    result = true;
                if (strValue.ToUpper() == "NO")
                    result = false;
            }

            return result;
        }

        /// <summary>
        /// Получить Дату со временем из узла
        /// </summary>
        /// <param name="node">узел</param>
        /// <param name="attributeName">имя атрибута</param>
        /// <param name="defaultValue">значение поумолчанию</param>
        /// <returns></returns>
        static public DateTime GetDateTimeXmlAttribute(XmlNode node, string attributeName, DateTime defaultValue)
        {
            string strValue = GetStrXmlAttribute(node, attributeName, defaultValue.ToString());
            return DateTime.Parse(strValue, new System.Globalization.CultureInfo("ru-ru"));
        }
    }
}

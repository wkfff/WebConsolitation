using System;
using System.Text;
using System.Xml;
using Krista.FM.Common.Xml;

namespace Krista.FM.Common.DimSync
{
    /// <summary>
    /// Влияние узла на потомков
    /// </summary>
    public enum NodeInfluence
    {
        /// <summary>
        /// Узел никак не влияет на потомков
        /// </summary>
        None,

        /// <summary>
        /// Узел автоматически включает все свои дочерние элементы (зеленый шарик)
        /// </summary>
        Children,

        /// <summary>
        /// Узел автоматически включает все нижележащие узлы (красный шарик)
        /// </summary>
        Descendants,

        /// <summary>
        /// Узел и все его потомки исключаются из рассмотрения (белый шарик)
        /// </summary>
        Exclude
    }

    public enum LevelState
    {
        /// <summary>
        /// Уровень отключен, выбор его элементов запрещен
        /// </summary>
        Disabled,

        /// <summary>
        /// Уровень включен, элементы можно выбирать
        /// </summary>
        Enabled,

        /// <summary>
        /// Уровень включен, все его элементы также включены
        /// </summary>
        Forced
    }

    public struct DimAttr
    {
        public const string Name = "name";
        public const string Checked = "checked";
        public const string Influence = "influence";
        public const string UnderInfluence = "underinfluence";
        public const string LevelState = "levelstate";
        public const string UniqueName = "unique_name";
        public const string DefaultValue = "defaultvalue";
    }

    public static class AuxDimUtils
    {
        /// <summary>
        /// Выделяет из полного имени измерения отдельно собственно измерение и иерархию
        /// </summary>
        /// <param name="fullDimName">Исходное полное имя</param>
        /// <param name="firstName">Имя измерения</param>
        /// <param name="lastName">Имя иерархии</param>
        /// <param name="delimiter">Разделитель имени измерения и иерархии</param>
        public static void ParseDimensionName(string fullDimName, out string firstName, out string lastName, string delimiter)
        {
            string[] s = fullDimName.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
            firstName = s[0];
            try
            {
                lastName = s[1];
            }
            catch (Exception)
            {
                lastName = string.Empty;
            }
        }

        /// <summary>
        /// Копирует состояние элементов из одного документа в другой
        /// </summary>
        /// <param name="oldMembers">Исходный документ</param>
        /// <param name="newMembers">Новый документ</param>
        public static void CopyMembersState(XmlDocument oldMembers, XmlDocument newMembers)
        {
            if ((oldMembers == null) || (newMembers == null))
            {
                return;
            }

            CheckByLevels(oldMembers, newMembers);
            CheckByMembers(oldMembers, newMembers);
            CheckByInfluence(oldMembers, newMembers);
        }

        /// <summary>
        /// Проставляет элементам признаки выделения всех дочерних и подчиненных
        /// </summary>
        public static void SetCheckedIndication(XmlDocument document)
        {
            XmlNodeList list = document.SelectNodes("function_result/Members//Member[@allchildrenchecked]");
            foreach (XmlNode node in list)
            {
                node.Attributes.RemoveNamedItem("allchildrenchecked");
            }

            list = document.SelectNodes("function_result/Members//Member[@alldescendantschecked]");
            foreach (XmlNode node in list)
            {
                node.Attributes.RemoveNamedItem("alldescendantschecked");
            }

            list = document.SelectNodes("function_result/Members//Member[(*) and not (./Member[@checked='false'])]");
            foreach (XmlNode node in list)
            {
                XmlHelper.SetAttribute(node, "allchildrenchecked", "true");
            }

            list = document.SelectNodes("function_result/Members//Member[(*) and not (.//Member[@checked='false'])]");
            foreach (XmlNode node in list)
            {
                XmlHelper.SetAttribute(node, "alldescendantschecked", "true");
            }
        }

        /// <summary>
        /// Копирует атрибуты влияния из отдельных узлов в специальную секцию
        /// </summary>
        public static void CopyInfluences(XmlDocument document)
        {
            XmlNode influencesRoot = InitInfluencesNode(document);
            if (influencesRoot == null)
            {
                return;
            }

            string xpath = string.Format("function_result/Members//Member[@{0}!={1}]", DimAttr.Influence, (int)NodeInfluence.None);
            XmlNodeList list = document.SelectNodes(xpath);
            if (list == null)
            {
                return;
            }

            foreach (XmlNode node in list)
            {
                int influenceInt = XmlHelper.GetIntAttrValue(node, DimAttr.Influence, 0);
                string umiqueName = XmlHelper.GetStringAttrValue(node, DimAttr.UniqueName, string.Empty);
                XmlNode infNode = document.CreateNode(XmlNodeType.Element, "Influence", string.Empty);
                XmlHelper.SetAttribute(infNode, "type", influenceInt.ToString());
                XmlHelper.SetAttribute(infNode, DimAttr.UniqueName, umiqueName);
                influencesRoot.AppendChild(infNode);
            }
        }

        /// <summary>
        /// Удаляет из документа все незначимые узлы
        /// </summary>
        /// <param name="considerInfluence">Учитывать ли признаки влияния или удалять без разбору</param>
        public static void CutAllInvisible(XmlDocument document, bool considerInfluence)
        {
            // удалим всех потомков исключенных элементов
            string xpath = string.Format("function_result/Members//Member[@{0}='{1}']", DimAttr.Influence, NodeInfluence.Exclude);
            XmlNodeList list = document.SelectNodes(xpath);
            foreach (XmlNode node in list)
            {
                while (node.HasChildNodes)
                {
                    node.RemoveChild(node.FirstChild);
                }

                XmlHelper.SetAttribute(node, DimAttr.Checked, "false");
            }

            // элементы, которые не выделены сами и не имеют выделенных потомков, заслуживают удаления
            // однако если учитываем признаки влияния и у такого элемента он есть, то удалять не надо 
            if (considerInfluence)
            {
                xpath = string.Format(
                        "function_result/Members//Member[(@{0}='false') and not(.//Member[@{0}='true']) and (@{1}='{2}')]",
                        DimAttr.Checked,
                        DimAttr.Influence,
                        ((int)NodeInfluence.None).ToString());
            }
            else
            {
                xpath = string.Format(
                    "function_result/Members//Member[(@{0}='false') and not(.//Member[@{0}='true'])]", DimAttr.Checked);
            }

            list = document.SelectNodes(xpath);
            for (int i = 0; i < list.Count; i++)
            {
                XmlNode node = list[i];
                node.ParentNode.RemoveChild(node);
            }
        }

        private static void CheckByInfluence(XmlDocument oldMembers, XmlDocument newMembers)
        {
            XmlNode influencesNode = InitInfluencesNode(newMembers);
            XmlNodeList oldList = oldMembers.SelectNodes("function_result/Influences/Influence[@type!='0']");
            if (oldList == null)
            {
                return;
            }

            foreach (XmlNode oldNode in oldList)
            {
                XmlNode newNode = newMembers.ImportNode(oldNode, true);
                influencesNode.AppendChild(newNode);
                string uniqueName = XmlHelper.GetStringAttrValue(oldNode, DimAttr.UniqueName, string.Empty);
                if (uniqueName == string.Empty)
                {
                    continue;
                }

                EncodeXPathString(ref uniqueName);
                string xpath = string.Format("function_result/Members//Member[@{0}='{1}']", DimAttr.UniqueName, uniqueName);
                XmlNode node = newMembers.SelectSingleNode(xpath);
                if (node == null)
                {
                    continue;
                }

                int influenceInt = XmlHelper.GetIntAttrValue(oldNode, "type", 0);
                XmlHelper.SetAttribute(node, DimAttr.Influence, influenceInt.ToString());

                string value = "false";
                switch ((NodeInfluence)influenceInt)
                {
                    case NodeInfluence.None:
                        continue;

                    case NodeInfluence.Children:
                        xpath = string.Format("./*[(@{0}='false') and (@{1}='3')]", DimAttr.Checked, DimAttr.Influence);
                        value = "true";
                        break;

                    case NodeInfluence.Descendants:
                        xpath = string.Format(".//*[(@{0}='false') and (@{1}='3')]", DimAttr.Checked, DimAttr.Influence);
                        value = "true";
                        break;

                    case NodeInfluence.Exclude:
                        XmlHelper.SetAttribute(node, DimAttr.Checked, "false");
                        xpath = ".//*";
                        value = "false";
                        break;
                }

                XmlNodeList children = node.SelectNodes(xpath);
                if (children == null)
                {
                    continue;
                }

                foreach (XmlNode child in children)
                {
                    XmlHelper.SetAttribute(child, DimAttr.Checked, value);
                }
            }
        }

        private static void CheckByMembers(XmlDocument oldMembers, XmlDocument newMembers)
        {
            string xpath = string.Format("function_result/Members//Member[@{0}]", DimAttr.Name);
            XmlNodeList newMemberList = newMembers.SelectNodes(xpath);
            foreach (XmlNode node in newMemberList)
            {
                XmlHelper.SetAttribute(node, DimAttr.Influence, ((int)NodeInfluence.None).ToString());
            }

            xpath = string.Format("function_result/Members//Member[@{0}='true']", DimAttr.Checked);
            XmlNodeList oldMemberList = oldMembers.SelectNodes(xpath);
            foreach (XmlNode oldMember in oldMemberList)
            {
                string uniqueName = XmlHelper.GetStringAttrValue(oldMember, DimAttr.UniqueName, string.Empty);
                if (uniqueName == string.Empty)
                {
                    continue;
                }

                EncodeXPathString(ref uniqueName);
                xpath = string.Format("function_result/Members//Member[@{0}='{1}']", DimAttr.UniqueName, uniqueName);
                XmlNode newMember = newMembers.SelectSingleNode(xpath);
                if (newMember != null)
                {
                    XmlHelper.SetAttribute(newMember, DimAttr.Checked, "true");
                }
            }

            /* При создании формы сбора одному из элементов проставляется признак "значение по умолчанию". 
             * Чтобы он не пропал, его надо переписать в новый документ.*/
            xpath = string.Format("function_result/Members//Member[@{0}]", DimAttr.DefaultValue);
            XmlNode defaultNode = oldMembers.SelectSingleNode(xpath);
            if (defaultNode != null)
            {
                string uniqueName = XmlHelper.GetStringAttrValue(defaultNode, DimAttr.UniqueName, string.Empty);
                EncodeXPathString(ref uniqueName);
                xpath = string.Format("function_result/Members//Member[@{0}='{1}']", DimAttr.UniqueName, uniqueName);
                XmlNode newMember = newMembers.SelectSingleNode(xpath);
                if (newMember != null)
                {
                    XmlHelper.SetAttribute(newMember, DimAttr.DefaultValue, "true");
                }
            }
        }

        private static void CheckByLevels(XmlDocument oldMembers, XmlDocument newMembers)
        {
            StringBuilder sb = new StringBuilder("function_result/Members");
            XmlNodeList newLevelsList = newMembers.SelectNodes("function_result/Levels/Level");
            foreach (XmlNode levelNode in newLevelsList)
            {
                string levelName = XmlHelper.GetStringAttrValue(levelNode, DimAttr.Name, string.Empty);
                string xpath = string.Format("function_result/Levels/Level[@{0}='{1}']", DimAttr.Name, levelName);
                XmlNode oldLevelNode = oldMembers.SelectSingleNode(xpath);
                int stateInt = (oldLevelNode != null) ? XmlHelper.GetIntAttrValue(oldLevelNode, DimAttr.LevelState, 1) : 0;
                XmlHelper.SetAttribute(levelNode, DimAttr.LevelState, stateInt.ToString());

                LevelState state = (LevelState)stateInt;
                string stateValue = (state == LevelState.Forced) ? "true" : "false";

                sb.Append("/*");
                XmlNodeList membersList = newMembers.SelectNodes(sb.ToString());
                foreach (XmlNode memberNode in membersList)
                {
                    XmlHelper.SetAttribute(memberNode, DimAttr.Checked, stateValue);
                }
            }
        }

        private static void EncodeXPathString(ref string xpath)
        {
            xpath = xpath.Replace('"', '\"');
        }

        private static XmlNode InitInfluencesNode(XmlDocument document)
        {
            XmlNode root = document.SelectSingleNode("function_result");
            if (root == null)
            {
                return null;
            }

            XmlNode result = root.SelectSingleNode("Influences");
            if (result != null)
            {
                while (result.HasChildNodes)
                {
                    result.RemoveChild(result.FirstChild);
                }
            }
            else
            {
                result = document.CreateNode(XmlNodeType.Element, "Influences", string.Empty);
                root.AppendChild(result);
            }

            return result;
        }
    }
}

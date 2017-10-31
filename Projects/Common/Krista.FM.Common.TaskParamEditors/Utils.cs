using System;
using System.Text;
using System.Xml;
using Krista.FM.Common.Xml;

namespace Krista.FM.Common.TaskParamEditors
{
    internal static class Utils
    {
        internal static void ParseDimensionName(string fullDimName, out string firstName, out string lastName, string delimiter)
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

        internal static void CopyMembersState(XmlDocument oldMembers, XmlDocument newMembers)
        {
            if ((oldMembers == null) || (newMembers == null))
                return;
            CheckByLevels(oldMembers, newMembers);
            CheckByMembers(oldMembers, newMembers);
            CheckByInfluence(oldMembers, newMembers);
        }

        private static void CheckByInfluence(XmlDocument oldMembers, XmlDocument newMembers)
        {
            XmlNode influencesNode = InitInfluencesNode(newMembers);
            XmlNodeList oldList = oldMembers.SelectNodes("function_result/Influences/Infuence[@type!='0']");
            if (oldList == null)
                return;

            foreach (XmlNode oldNode in oldList)
            {
                influencesNode.AppendChild(oldNode.CloneNode(true));
                string uName = XmlHelper.GetStringAttrValue(oldNode, Attr.UniqueName, string.Empty);
                if (uName == string.Empty)
                    continue;
                EncodeXPathString(ref uName);
                string xpath = string.Format("function_result/Members//Member[@{0}='{1}']", Attr.UniqueName, uName);
                XmlNode node = newMembers.SelectSingleNode(xpath);
                if (node == null)
                    continue;
                int influenceInt = XmlHelper.GetIntAttrValue(oldNode, "type", 0);
                XmlHelper.SetAttribute(node, "type", influenceInt.ToString());
                
                string value = "false";
                switch ((NodeInfluence) influenceInt)
                {
                    case NodeInfluence.None: 
                        continue;
                    
                    case NodeInfluence.Children:
                        xpath = string.Format("./*[(@{0}='false') and (@{1}='3')]", Attr.Checked, Attr.Influence);
                        value = "true";
                        break;

                    case NodeInfluence.Descendants:
                        xpath = string.Format(".//*[(@{0}='false') and (@{1}='3')]", Attr.Checked, Attr.Influence);
                        value = "true";
                        break;

                    case NodeInfluence.Exclude:
                        XmlHelper.SetAttribute(node, Attr.Checked, "false");
                        xpath = ".//*";
                        value = "false";
                        break;
                }

                XmlNodeList children = node.SelectNodes(xpath);
                if (children == null)
                    continue;
                foreach (XmlNode child in children)
                    XmlHelper.SetAttribute(child, Attr.Checked, value);
            }
        }

        private static void CheckByMembers(XmlDocument oldMembers, XmlDocument newMembers)
        {
            string xpath = string.Format("function_result/Members//Member[@{0}]", Attr.Name);
            XmlNodeList newMemberList = newMembers.SelectNodes(xpath);
            foreach (XmlNode node in newMemberList)
                XmlHelper.SetAttribute(node, Attr.Influence, ((int) NodeInfluence.None).ToString());

            xpath = string.Format("function_result/Members//Member[@{0}='true']", Attr.Checked);
            XmlNodeList oldMemberList = oldMembers.SelectNodes(xpath);
            foreach (XmlNode oldMember in oldMemberList)
            {
                string uName = XmlHelper.GetStringAttrValue(oldMember, Attr.UniqueName, string.Empty);
                if (uName == string.Empty)
                    continue;
                EncodeXPathString(ref uName);
                xpath = string.Format("function_result/Members//Member[@{0}='{1}']", Attr.UniqueName, uName);
                XmlNode newMember = newMembers.SelectSingleNode(xpath);
                if (newMember != null)
                    XmlHelper.SetAttribute(newMember, Attr.Checked, "true");
            }

            /* При создании формы сбора одному из элементов проставляется признак "значение по умолчанию". 
             * Чтобы он не пропал, его надо переписать в новый документ.*/
            xpath = string.Format("function_result/Members//Member[@{0}]", Attr.DefaultValue);
            XmlNode defaultNode = oldMembers.SelectSingleNode(xpath);
            if (defaultNode != null)
            {
                string uName = XmlHelper.GetStringAttrValue(defaultNode, Attr.UniqueName, string.Empty);
                EncodeXPathString(ref uName);
                xpath = string.Format("function_result/Members//Member[@{0}='{1}']", Attr.UniqueName, uName);
                XmlNode newMember = newMembers.SelectSingleNode(xpath);
                if (newMember != null)
                    XmlHelper.SetAttribute(newMember, Attr.DefaultValue, "true");
            }
        }

        private static void CheckByLevels(XmlDocument oldMembers, XmlDocument newMembers)
        {
            StringBuilder sb = new StringBuilder("function_result/Members");
            XmlNodeList newLevelsList = newMembers.SelectNodes("function_result/Levels/Level");
            foreach (XmlNode levelNode in newLevelsList)
            {
                string levelName = XmlHelper.GetStringAttrValue(levelNode, Attr.Name, string.Empty);
                string xpath = string.Format("function_result/Levels/Level[@{0}='{1}']", Attr.Name, levelName);
                XmlNode oldLevelNode = oldMembers.SelectSingleNode(xpath);
                int stateInt = (oldLevelNode != null) ? XmlHelper.GetIntAttrValue(oldLevelNode, Attr.LevelState, 1) : 0;
                XmlHelper.SetAttribute(levelNode, Attr.LevelState, stateInt.ToString());

                LevelState state = (LevelState) stateInt;
                string stateValue = (state == LevelState.Forced) ? "true" : "false";

                sb.Append("/*");
                XmlNodeList membersList = newMembers.SelectNodes(sb.ToString());
                foreach (XmlNode memberNode in membersList)
                    XmlHelper.SetAttribute(memberNode, Attr.Checked, stateValue);
            }
        }

        private static void EncodeXPathString(ref string xpath)
        {
            xpath = xpath.Replace(@"\", @"\\").Replace('"', '\"');
        }

        private static XmlNode InitInfluencesNode(XmlDocument document)
        {
            XmlNode root = document.SelectSingleNode("function_result");
            if (root == null)
                return null;

            XmlNode result = root.SelectSingleNode("Influences");
            if (result != null)
            {
                while (result.HasChildNodes)
                    result.RemoveChild(result.FirstChild);
            }
            else
            {
                result = document.CreateNode(XmlNodeType.Text, "Influences", string.Empty);
                root.AppendChild(result);
            }
            return result;
        }

        internal static void SetCheckedIndication(XmlDocument document)
        {
            XmlNodeList list = document.SelectNodes("function_result/Members//Member[@allchildrenchecked]");
            foreach (XmlNode node in list)
                node.Attributes.RemoveNamedItem("allchildrenchecked");

            list = document.SelectNodes("function_result/Members//Member[@alldescendantschecked]");
            foreach (XmlNode node in list)
                node.Attributes.RemoveNamedItem("alldescendantschecked");

            list = document.SelectNodes("function_result/Members//Member[(*) and not (./Member[@checked='false'])]");
            foreach (XmlNode node in list)
                XmlHelper.SetAttribute(node, Attr.Checked, "true");
            
            list = document.SelectNodes("function_result/Members//Member[(*) and not (.//Member[@checked='false'])]");
            foreach (XmlNode node in list)
                XmlHelper.SetAttribute(node, "alldescendantschecked", "true");
        }

        /// <summary>
        /// Копирует атрибуты влияния из отдельных узлов в специальную секцию
        /// </summary>
        /// <param name="document"></param>
        internal static void CopyInfluences(XmlDocument document)
        {
            XmlNode influencesRoot = InitInfluencesNode(document);
            if (influencesRoot == null)
                return;
            
            string xpath = string.Format("function_result/Members//Member[@{0}!={1}]", Attr.Influence, (int) NodeInfluence.None);
            XmlNodeList list = document.SelectNodes(xpath);
            if (list == null)
                return;

            foreach (XmlNode node in list)
            {
                int influenceInt = XmlHelper.GetIntAttrValue(node, Attr.Influence, 0);
                string uName = XmlHelper.GetStringAttrValue(node, Attr.UniqueName, string.Empty);
                XmlNode infNode = document.CreateNode(XmlNodeType.Text, "Influence", string.Empty);
                influencesRoot.AppendChild(infNode);
                XmlHelper.SetAttribute(infNode, "type", influenceInt.ToString());
                XmlHelper.SetAttribute(infNode, Attr.UniqueName, uName);
            }
            
        }

        internal static void CutAllInvisible(XmlDocument document, bool considerInfluence)
        {
            // удалим всех потомков исключенных элементов
            string xpath = string.Format("function_result/Members//Member[@{0}='{1}']", Attr.Influence, NodeInfluence.Exclude);
            XmlNodeList list = document.SelectNodes(xpath);
            foreach (XmlNode node in list)
            {
                while (node.HasChildNodes)
                    node.RemoveChild(node.FirstChild);
                XmlHelper.SetAttribute(node, Attr.Checked, "false");
            }

            // элементы, которые не выделены сами и не имеют выделенных потомков, заслуживают удаления
            // однако если учитываем признаки влияния и у такого элемента он есть, то удалять не надо 
            if (considerInfluence)
            {
                xpath = string.Format(
                        "function_result/Members//Member[(@{0}='false') and not(.//Member[@{0}='true']) and (@{1}='{2}')]",
                        Attr.Checked, Attr.Influence, ((int) NodeInfluence.None).ToString());
            }
            else
                xpath = string.Format("function_result/Members//Member[(@{0}='false') and not(.//Member[@{0}='true'])]", Attr.Checked);
            list = document.SelectNodes(xpath);
            for (int i = 0; i < list.Count; i++)
            {
                XmlNode node = list[i];
                node.ParentNode.RemoveChild(node);
            }
        }

       
    }
}

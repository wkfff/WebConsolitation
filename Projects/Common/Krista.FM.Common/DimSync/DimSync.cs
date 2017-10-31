using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Krista.FM.Common.Xml;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Common.DimSync
{
    public class DimSync
    {
        private readonly IScheme scheme;
        private XmlDocument metadata;

        public DimSync(IScheme scheme)
        {
            if (scheme != null)
            {
                this.scheme = scheme;
            }
            else
            {
                throw new Exception("Параметр scheme имеет значение null");
            }
        }

        public string LastError { get; set; }
        
        public bool LoadMetadata()
        {
            string metadataStr = scheme.PlaningProvider.GetMetaData();
            if (string.IsNullOrEmpty(metadataStr))
            {
                return false;
            }

            metadata = new XmlDocument();
            try
            {
                metadata.LoadXml(metadataStr);
            }
            catch (Exception e)
            {
                LastError = Diagnostics.KristaDiagnostics.ExpandException(e);
                return false;
            }

            return true;
        }

        public bool Sync(string fullDimName, string oldValue, out string newValue)
        {
            LastError = string.Empty;
            newValue = string.Empty;

            XmlDocument oldMembers = null;
            if (oldValue != string.Empty)
            {
                oldMembers = new XmlDocument();
                try
                {
                    oldMembers.LoadXml(oldValue);
                }
                catch 
                {
                    oldMembers = null;
                }
            }

            string dimName;
            string hierName;
            AuxDimUtils.ParseDimensionName(fullDimName, out dimName, out hierName, ".");

            string levelNames = GetLevelNames(dimName, hierName);
            if (levelNames == string.Empty)
            {
                LastError = "Не удалось получить список уровней измерения";
                return false;
            }

            string members = scheme.PlaningProvider.GetMembers("0", string.Empty, dimName, hierName, levelNames, string.Empty);
            XmlDocument newMembers = new XmlDocument();
            try
            {
                newMembers.LoadXml(members);
            }
            catch (Exception e)
            {
                LastError = Diagnostics.KristaDiagnostics.ExpandException(e);
                return false;
            }

            if (oldMembers != null)
            {
                AuxDimUtils.CopyMembersState(oldMembers, newMembers);
            }

            newValue = newMembers.OuterXml;
            return true;
        }

        private string GetLevelNames(string dimName, string hierName)
        {
            XmlNode hierNode = GetHierarchyNode(dimName, hierName);
            if (hierNode == null)
            {
                return string.Empty;
            }

            XmlNodeList levelsList = hierNode.SelectNodes("Levels/Level");
            StringBuilder sb = new StringBuilder();
            foreach (XmlNode levelNode in levelsList)
            {
                string levelName = XmlHelper.GetStringAttrValue(levelNode, DimAttr.Name, string.Empty);
                if (sb.ToString() != string.Empty)
                {
                    sb.Append("$$$");
                }

                sb.Append(levelName);
            }

            return sb.ToString();
        }

        private XmlNode GetHierarchyNode(string dimName, string hierName)
        {
            string xpath = string.Format("function_result/SharedDimensions/Dimension[@{0}='{1}']", DimAttr.Name, dimName);
            XmlNode dimNode = metadata.SelectSingleNode(xpath);
            if (dimNode == null)
            {
                return null;
            }

            xpath = string.Format("Hierarchy[@{0}='{1}']", DimAttr.Name, hierName);
            XmlNode hierNode = dimNode.SelectSingleNode(xpath);
            return hierNode;
        }
    }
}

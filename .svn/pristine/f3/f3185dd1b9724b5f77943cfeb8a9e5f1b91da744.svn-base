using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Krista.FM.Common.Xml;

namespace Krista.FM.Client.MDXExpert.Data
{
    [Serializable]
    [XmlInclude(typeof(MemberFilter))]
    public class MemberFilterCollection : IEnumerable
    {
        private List<MemberFilter> _items;

        public List<MemberFilter> Items
        {
            get { return _items; }
        }

        public MemberFilter this[int index]
        {
            get
            {
                return _items[index];
            }
        }
        public MemberFilterCollection()
        {
            this._items = new List<MemberFilter>();
        }

        public int Count
        {
            get { return this._items.Count; }
        }

        public void Add(MemberFilter mFilter)
        {
            this._items.Add(mFilter);
        }

        public void Add(string memberName, Dictionary<string, string> memberProperties)
        {
            MemberFilter mFilter = new MemberFilter();
            mFilter.MemberName = memberName;
            mFilter.Properties = memberProperties;
            this._items.Add(mFilter);
        }

        public bool Remove(MemberFilter filter)
        {
            return this._items.Remove(filter);
        }

        public void Clear()
        {
            this._items.Clear();
        }

        public void SaveXml(XmlNode parentNode)
        {
            XmlNode root = XmlHelper.AddChildNode(parentNode, "memberFilters", "", null);

            foreach (MemberFilter filter in this)
            {
                XmlNode filterNode = XmlHelper.AddChildNode(root, "filter", new string[2] {"name", filter.MemberName});
                XmlNode mProperties = XmlHelper.AddChildNode(filterNode, "properties", "", null);
                foreach (KeyValuePair<string, string> prop in filter.Properties)
                {
                    XmlHelper.AddChildNode(mProperties, "property", new string[2] { "name", prop.Key },
                                                                    new string[2] { "value", prop.Value });
                }
            }
        }

        public void LoadXml(XmlNode node)
        {
            if (node == null)
                return;
            XmlNodeList filters = node.SelectNodes("filter");
            foreach(XmlNode filterNode in filters)
            {
                MemberFilter mFilter = new MemberFilter();
                mFilter.MemberName = XmlHelper.GetStringAttrValue(filterNode, "name", "");
                XmlNode propertiesNode = filterNode.SelectSingleNode("properties");
                if (propertiesNode != null)
                {
                    XmlNodeList properties = propertiesNode.SelectNodes("property");
                    foreach(XmlNode prop in properties)
                    {
                        mFilter.Properties.Add(XmlHelper.GetStringAttrValue(prop, "name", ""),
                                               XmlHelper.GetStringAttrValue(prop, "value", ""));
                    }
                }


                this.Add(mFilter);


            }

        }

        public IEnumerator GetEnumerator()
        {
            return this._items.GetEnumerator();
        }
    }
}

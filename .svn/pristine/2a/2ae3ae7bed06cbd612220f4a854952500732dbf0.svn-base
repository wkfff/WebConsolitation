using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Common.Xml;

namespace Krista.FM.Client.MDXExpert
{

    public class LegendContentCollection : List<LegendContent>
    {

        public LegendContent this[string name]
        {
            get { return GetItemByName(name); }
        }

        public LegendContentCollection()
        {

        }

        public LegendContent Add(string legendName, LegendContentType contentType)
        {
            LegendContent item = new LegendContent(legendName, contentType);
            this.Add(item);
            return item;
        }

        public void Remove(string legendName)
        {
            LegendContent item = this[legendName];
            if (item != null)
                this.Remove(item);
        }

        private LegendContent GetItemByName(string name)
        {
            foreach(LegendContent content in this)
            {
                if (content.LegendName == name)
                    return content;
            }
            return null;
        }

        public void Save(XmlNode collectionNode)
        {
            if (collectionNode == null)
                return;

            foreach (LegendContent rule in this)
            {
                XmlNode contentNode = XmlHelper.AddChildNode(collectionNode, Consts.legendContent);
                XmlHelper.SetAttribute(contentNode, "name", rule.LegendName);
                XmlHelper.SetAttribute(contentNode, "type", rule.ContentType.ToString());
            }
        }

        public void Load(XmlNode collectionNode)
        {
            if (collectionNode == null)
                return;

            this.Clear();
            XmlNodeList contentNodes = collectionNode.SelectNodes(Consts.legendContent);

            foreach (XmlNode contentNode in contentNodes)
            {
                this.Add(XmlHelper.GetStringAttrValue(contentNode, "name", ""),
                         (LegendContentType) Enum.Parse(typeof (LegendContentType),
                                                        XmlHelper.GetStringAttrValue(contentNode, "type", "Values")));
            }
        }
    }

    /// <summary>
    /// Класс определяет что будет отображаться в легенде
    /// </summary>
    public class LegendContent
    {
        private string _legendName;
        private LegendContentType _contentType;

        /// <summary>
        /// Имя легенды
        /// </summary>
        public string LegendName
        {
            get { return this._legendName; }
            set { this._legendName = value; }
        }

        /// <summary>
        /// Тип отображения интервалов в легенде
        /// </summary>
        public LegendContentType ContentType
        {
            get { return this._contentType; }
            set { this._contentType = value; }
        }


        public LegendContent()
        {
        }

        public LegendContent(string legendName, LegendContentType contentType)
        {
            this._legendName = legendName;
            this._contentType = contentType;
        }

        public override string ToString()
        {
            return String.Empty;
        }
    }

    /// <summary>
    /// Информация об интервалах, которая будет показываться в легенде 
    /// </summary>
    public enum LegendContentType
    {
        [Description("Имена интервалов")]
        Name,
        [Description("Значения интервалов")]
        Values,
        [Description("Имена и значения")]
        NameAndValues
    }
}

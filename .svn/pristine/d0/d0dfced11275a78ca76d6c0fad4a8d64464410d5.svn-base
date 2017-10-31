using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes
{
    internal class TableEntity : Entity
    {
        public TableEntity(string key, ServerSideObject owner, string semantic, string name, ServerSideObjectStates state)
            : base(key, owner, semantic, name, ClassTypes.Table, SubClassTypes.Regular, state, SchemeClass.ScriptingEngineFactory.EntityScriptingEngine)
        {
            tagElementName = "Table";
        }

        /// <summary>
        /// Префикс таблицы.
        /// </summary>
        public override string TablePrefix
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return "t"; }
        }

        /// <summary>
        /// Полное имя объекта
        /// </summary>
        public override string FullName
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return TablePrefix + "." + base.FullName; }
        }

        public override bool CurrentUserCanViewThisObject()
        {
            return false;
        }

        internal override void Save2Xml(XmlNode node)
        {
            base.Save2Xml(node);

            //Уникальные ключи
            if ((UniqueKeys != null) && (UniqueKeys.Count > 0))
            {
                XmlNode uniqueKeysNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "UniqueKeyList", null);
                ((UniqueKeyCollection)this.UniqueKeys).Save2Xml(uniqueKeysNode);
                node.AppendChild(uniqueKeysNode);
            }
        }

        public override bool UniqueKeyAvailable
        {
            get { return true; }
        }
    }
}

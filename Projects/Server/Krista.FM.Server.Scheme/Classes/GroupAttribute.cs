using System.Collections.Generic;
using System.Xml;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes
{
    internal class GroupAttribute : DataAttribute, IGroupAttribute
    {
        private Dictionary<string , IDataAttribute> attributes;

        public GroupAttribute(string key, string name, ServerSideObject owner, ServerSideObjectStates state) 
            : base(key, name, owner, state)
        {
            attributes = new Dictionary<string, IDataAttribute>();
        }

        public GroupAttribute(string key, ServerSideObject owner, XmlNode xmlAttribute, ServerSideObjectStates state) 
            : base(key, owner, xmlAttribute, state)
        {
        }

        public Dictionary<string, IDataAttribute> Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }
    }
}

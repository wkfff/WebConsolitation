using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Krista.FM.Update.Framework.Conditions
{
    [Serializable]
    public class UpdateCondition : IUpdateCondition
    {
        public UpdateCondition()
        {
            Attributes = new Dictionary<string, string>();
        }

        public IDictionary<string, string> Attributes { get; private set; }

        public virtual bool IsMet(IUpdateTask task)
        {
            return false;
        }

        public virtual XElement ToXml()
        {
            XElement condition = new XElement(GetType().Name);

            foreach (var attribute in Attributes)
            {
                condition.SetAttributeValue(attribute.Key, attribute.Value);
            }

            return condition;
        }
    }
}

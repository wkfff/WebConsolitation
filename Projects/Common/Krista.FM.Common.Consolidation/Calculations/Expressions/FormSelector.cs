using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class FormSelector : ConsRelationExpression
    {
        public FormSelector(string name)
        {
            Name = name;
        }

        public FormSelector()
        {
        }

        [DataMember]
        [ProtoMember(1)]
        public string Name { get; set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class SectionSelector : ConsRelationExpression
    {
        public SectionSelector(string name)
        {
            Name = name;
        }

        public SectionSelector()
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

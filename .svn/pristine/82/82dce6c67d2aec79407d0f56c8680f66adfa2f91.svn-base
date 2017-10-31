using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class DependRowSelector : ConsRelationExpression
    {
        public DependRowSelector(ConsRelationExpression dependCond)
        {
            DependCond = dependCond;
        }

        public DependRowSelector()
        {
        }

        [DataMember]
        [ProtoMember(1)]
        public ConsRelationExpression DependCond { get; set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

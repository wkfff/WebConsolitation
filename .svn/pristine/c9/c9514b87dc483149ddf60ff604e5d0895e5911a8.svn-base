using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class TotalRowGenRelation : ConsRelationExpression
    {
        public TotalRowGenRelation(LeftValueSelector lvalueSelector, ConsRelationExpression consSelector)
        {
            LvalueSelector = lvalueSelector;
            ConsSelector = consSelector;
        }

        public TotalRowGenRelation()
        {
        }

        [DataMember]
        [ProtoMember(1)]
        public LeftValueSelector LvalueSelector { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public ConsRelationExpression ConsSelector { get; set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

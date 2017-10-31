using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class ConsRowGenRelation : ConsRelationExpression
    {
        public ConsRowGenRelation(ConsRelationExpression lvalueSelector, ConsRelationExpression consSelector)
        {
            LValueSelector = lvalueSelector;
            ConsSelector = consSelector;
        }

        public ConsRowGenRelation()
        {
        }

        [DataMember]
        [ProtoMember(1)]
        public ConsRelationExpression LValueSelector { get; set; }

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

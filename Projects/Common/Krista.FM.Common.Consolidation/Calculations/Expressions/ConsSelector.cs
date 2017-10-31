using System.Collections.Generic;
using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class ConsSelector : ConsRelationExpression
    {
        public ConsSelector(ConsRelationExpression dependSelector, List<ConsRelationExpression> rightValueExpressions)
        {
            DependSelector = dependSelector;
            RValueExpressions = rightValueExpressions;
        }

        public ConsSelector()
        {
        }

        [DataMember]
        [ProtoMember(1)]
        public ConsRelationExpression DependSelector { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public List<ConsRelationExpression> RValueExpressions { get; set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

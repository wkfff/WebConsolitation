using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class RightValueSelector : ConsRelationExpression
    {
        public RightValueSelector(ConsRelationExpression dependSelector, ConsRelationExpression expr)
        {
            DependSelector = dependSelector;
            Expr = expr;
        }

        public RightValueSelector()
        {
        }

        [DataMember]
        [ProtoMember(1)]
        public ConsRelationExpression DependSelector { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public ConsRelationExpression Expr { get; set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

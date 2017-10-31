using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class AssignRelation : ConsRelationExpression
    {
        public AssignRelation()
        {
        }

        public AssignRelation(ConsRelationExpression lvalueSelector, ConsRelationExpression expr)
        {
            LValueSelector = lvalueSelector;
            Expr = expr;
        }

        [DataMember]
        [ProtoMember(1)]
        public ConsRelationExpression LValueSelector { get; set; }

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

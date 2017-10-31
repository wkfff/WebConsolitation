using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class UnaryExpression : ConsRelationExpression
    {
        public UnaryExpression(UnaryExpressionTypes type, ConsRelationExpression exp)
        {
            Exp = exp;
            Type = type;
        }

        public UnaryExpression()
        {
        }

        [DataMember]
        [ProtoMember(1)]
        public ConsRelationExpression Exp { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public UnaryExpressionTypes Type { get; set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

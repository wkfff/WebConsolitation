using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class VerifyRelation : ConsRelationExpression
    {
        public VerifyRelation(BinaryExpressionTypes type, ConsRelationExpression left, ConsRelationExpression right)
        {
            Type = type;
            Left = left;
            Right = right;
        }

        public VerifyRelation()
        {
        }

        [DataMember]
        [ProtoMember(1)]
        public BinaryExpressionTypes Type { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public ConsRelationExpression Left { get; set; }

        [DataMember]
        [ProtoMember(3)]
        public ConsRelationExpression Right { get; set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class BinaryExpression : ConsRelationExpression
    {
        public BinaryExpression(BinaryExpressionTypes type, ConsRelationExpression left, ConsRelationExpression right)
        {
            Left = left;
            Right = right;
            Type = type;
        }

        public BinaryExpression()
        {
        }

        [DataMember]
        [ProtoMember(1)]
        public ConsRelationExpression Left { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public ConsRelationExpression Right { get; set; }

        [DataMember]
        [ProtoMember(3)]
        public BinaryExpressionTypes Type { get; set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

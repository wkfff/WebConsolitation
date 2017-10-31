using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class UndependRowSelector : ConsRelationExpression
    {
        public UndependRowSelector(ConsRelationExpression undependCond)
        {
            UndependCond = undependCond;
        }

        public UndependRowSelector()
        {
        }

        [DataMember]
        [ProtoMember(1)]
        public ConsRelationExpression UndependCond { get; set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

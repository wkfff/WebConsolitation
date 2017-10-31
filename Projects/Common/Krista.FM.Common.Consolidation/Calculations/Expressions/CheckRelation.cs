using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class CheckRelation : ConsRelationExpression
    {
        public CheckRelation(ConsRelationExpression undependRowSelector)
        {
            UndependRowSelector = undependRowSelector;
        }

        public CheckRelation()
        {
        }

        [DataMember]
        [ProtoMember(1)]
        public ConsRelationExpression UndependRowSelector { get; set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

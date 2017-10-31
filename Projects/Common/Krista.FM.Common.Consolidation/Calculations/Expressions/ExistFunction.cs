using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class ExistFunction : ConsRelationExpression
    {
        public ExistFunction(ExistFunctionTypes type, ConsRelationExpression dependSelector)
        {
            DependSelector = dependSelector;
            Type = type;
        }

        public ExistFunction()
        {
        }

        [DataMember]
        [ProtoMember(1)]
        public ConsRelationExpression DependSelector { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public ExistFunctionTypes Type { get; set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

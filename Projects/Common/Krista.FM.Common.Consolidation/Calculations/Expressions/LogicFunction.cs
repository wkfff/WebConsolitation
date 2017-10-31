using System.Collections.Generic;
using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class LogicFunction : ConsRelationExpression
    {
        public LogicFunction(ExistFunctionTypes type, List<ConsRelationExpression> parameters)
        {
            Parameters = parameters;
            Type = type;
        }

        public LogicFunction()
        {
        }

        [DataMember]
        [ProtoMember(1)]
        public List<ConsRelationExpression> Parameters { get; set; }

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

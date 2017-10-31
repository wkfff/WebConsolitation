using System.Collections.Generic;
using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class LeftValueSelector : ConsRelationExpression
    {
        public LeftValueSelector(UndependRowSelector undependRowSelector, List<string> colsSelector)
        {
            UndependRowSelector = undependRowSelector;
            ColsSelector = colsSelector;
        }

        public LeftValueSelector()
        {
        }

        [DataMember]
        [ProtoMember(1)]
        public UndependRowSelector UndependRowSelector { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public List<string> ColsSelector { get; set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

using System.Collections.Generic;
using System.Runtime.Serialization;

using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class ColsSelector : ConsRelationExpression
    {
        public ColsSelector(List<string> columns)
        {
            Columns = columns;
        }

        [DataMember]
        [ProtoMember(1)]
        public List<string> Columns { get; set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

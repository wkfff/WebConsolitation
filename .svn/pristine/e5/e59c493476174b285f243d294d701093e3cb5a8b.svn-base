using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    [ProtoInclude(1, typeof(UndependParamColumn))]
    [ProtoInclude(2, typeof(DependParamColumn))]
    public class ParamColumn : ParamExpression
    {
        public ParamColumn(string column)
        {
            Column = column;
        }

        public ParamColumn()
        {
        }

        [DataMember]
        [ProtoMember(3)]
        public string Column { get; set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

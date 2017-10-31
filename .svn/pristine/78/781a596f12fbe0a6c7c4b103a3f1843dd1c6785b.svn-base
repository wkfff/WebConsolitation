using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    [ProtoInclude(1, typeof(DependContextParamColumn))]
    public class DependParamColumn : ParamColumn
    {
        public DependParamColumn(string column)
            : base(column)
        {
        }

        public DependParamColumn() : base(null)
        {
        }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class DependContextParamColumn : DependParamColumn
    {
        public DependContextParamColumn(string column)
            : base(column)
        {
        }

        public DependContextParamColumn() : base(null)
        {
        }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

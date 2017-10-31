using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class UndependParamColumn : ParamColumn
    {
        public UndependParamColumn(string column)
            : base(column)
        {
        }

        public UndependParamColumn()
        {
        }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

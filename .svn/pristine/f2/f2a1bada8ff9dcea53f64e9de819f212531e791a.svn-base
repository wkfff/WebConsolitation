using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class LayerSelector : ConsRelationExpression
    {
        public LayerSelector(ConsRelationExpression layerCond)
        {
            LayerCond = layerCond;
        }

        public LayerSelector()
        {
        }

        [DataMember]
        [ProtoMember(1)]
        public ConsRelationExpression LayerCond { get; set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

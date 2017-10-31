using System.Runtime.Serialization;
using ProtoBuf;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    [DataContract][ProtoContract]
    public class DependSelector : ConsRelationExpression
    {
        public DependSelector(ConsRelationExpression dependRowSelector, ConsRelationExpression layerSelector, ConsRelationExpression formSelector, ConsRelationExpression sectionSelector)
        {
            DependRowSelector = dependRowSelector;
            LayerSelector = layerSelector;
            FormSelector = formSelector;
            SectionSelector = sectionSelector;
        }

        public DependSelector()
        {
        }

        [DataMember]
        [ProtoMember(1)]
        public ConsRelationExpression DependRowSelector { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public ConsRelationExpression LayerSelector { get; set; }

        [DataMember]
        [ProtoMember(3)]
        public ConsRelationExpression FormSelector { get; set; }

        [DataMember]
        [ProtoMember(4)]
        public ConsRelationExpression SectionSelector { get; set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

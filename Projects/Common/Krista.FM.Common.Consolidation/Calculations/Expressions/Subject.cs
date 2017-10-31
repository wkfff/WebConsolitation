using System.Collections.Generic;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    public class Subject : ConsRelationExpression
    {
        public Subject(List<ConsRelationExpression> refs)
        {
            Refs = refs;
        }

        public Subject()
        {
        }

        public List<ConsRelationExpression> Refs { get; private set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

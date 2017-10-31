using System.Collections.Generic;

namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    public class ClassMethod : ConsRelationExpression
    {
        public ClassMethod(string name, List<ConsRelationExpression> parameters)
        {
            Name = name;
            Parameters = parameters;
        }

        public ClassMethod()
        {
        }

        public string Name { get; set; }

        public List<ConsRelationExpression> Parameters { get; set; }

        [System.Diagnostics.DebuggerStepThrough]
        public override void Accept(ConsRelationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
